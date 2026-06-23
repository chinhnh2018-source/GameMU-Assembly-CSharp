using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services
{
    public class XmlValidationService
    {
        private readonly string _configRoot;

        // Cache danh sách ID vật phẩm trong Goods.xml (tránh đọc lại file ~9700 item mỗi lần lưu)
        private static HashSet<string>? _goodsCache;
        private static DateTime _goodsCacheStamp;

        // Các trường chứa MỘT mã vật phẩm
        private static readonly string[] SingleGoodsKeys = { "GoodsID", "GoodsId", "ItemID", "ItemId" };
        // Các trường chứa DANH SÁCH quà dạng "id,sl,...|id,sl,..." (mã vật phẩm là số đầu mỗi cụm)
        // Da doi soat dinh luong: ~100% gia tri cac truong nay khop Item.ID trong Goods.xml.
        private static readonly string[] RewardListKeys = {
            "GoodsOne", "GoodsTwo", "GoodsThr", "GoodsFour", "Goods", "GoodsList", "GoodsIDs",
            "Award", "Awards", "EventAward", "AwardGoods", "DayAward",
            "NeedGoods", "CostGoods", "Items"
        };

        public XmlValidationService(string configRoot)
        {
            _configRoot = configRoot;
        }

        /// <summary>Kiểm tra toàn bộ ràng buộc nghiệp vụ trước khi ghi file.</summary>
        public void Validate(EventFileDef def, Dictionary<string, string> values, bool isNew, List<EventRecord> existingRecords)
        {
            // 1) Trùng khóa chính (ID) khi thêm mới
            if (isNew && values.TryGetValue(def.IdAttr, out var newId) && !string.IsNullOrWhiteSpace(newId))
            {
                if (existingRecords.Any(r => r.Id == newId))
                    throw new InvalidOperationException($"[LỖI TRÙNG ID] Mã '{newId}' đã tồn tại trong '{def.RelativePath}'. Hãy dùng ID khác!");
            }

            // 2) Logic khung thời gian (FromDate < ToDate)
            ValidateTimeWindow(values);

            // 3) Giới hạn % cho tham số hệ thống
            if (def.Key == "system-params")
                ValidateSystemParams(values);

            // 4) Ràng buộc liên file: mã vật phẩm phải tồn tại trong Goods.xml
            ValidateGoodsReference(values);
        }

        private void ValidateTimeWindow(Dictionary<string, string> values)
        {
            string[] startKeys = { "FromDate", "BeginTime", "StartTime" };
            string[] endKeys = { "ToDate", "FinishTime", "EndTime" };
            var startAttr = startKeys.FirstOrDefault(values.ContainsKey);
            var endAttr = endKeys.FirstOrDefault(values.ContainsKey);
            if (startAttr == null || endAttr == null) return;

            var s = values[startAttr].Trim();
            var e = values[endAttr].Trim();
            if (s is "-1" or "" || e is "-1" or "") return;

            if (DateTime.TryParse(s, out var start) && DateTime.TryParse(e, out var end) && start.Year > 2001 && end.Year > 2001)
            {
                if (start >= end)
                    throw new InvalidOperationException($"[LỖI THỜI GIAN] Thời gian bắt đầu '{s}' không thể >= thời gian kết thúc '{e}'!");
            }
        }

        private void ValidateSystemParams(Dictionary<string, string> values)
        {
            if (values.TryGetValue("Name", out var name) && values.TryGetValue("Value", out var valStr))
            {
                if ((name.Contains("Percent") || name.Contains("Rate")) && double.TryParse(valStr, out var p))
                {
                    if (p < 0 || p > 100)
                        throw new InvalidOperationException($"[LỖI THAM SỐ] Tỷ lệ '{name}' phải trong khoảng [0..100]%. Giá trị '{valStr}' không hợp lệ!");
                }
            }
        }

        private void ValidateGoodsReference(Dictionary<string, string> values)
        {
            var candidates = new List<string>();

            foreach (var key in SingleGoodsKeys)
                if (values.TryGetValue(key, out var v) && IsRealId(v))
                    candidates.Add(v.Trim());

            foreach (var key in RewardListKeys)
                if (values.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v))
                    candidates.AddRange(ParseRewardGoodsIds(v));

            if (candidates.Count == 0) return;

            var valid = LoadGoodsIds();
            if (valid.Count == 0) return; // không đọc được Goods.xml → bỏ qua để không kẹt thao tác

            foreach (var id in candidates.Distinct())
                if (!valid.Contains(id))
                    throw new InvalidOperationException($"[LỖI LIÊN KẾT] Mã vật phẩm '{id}' không tồn tại trong 'Goods.xml'. Hãy kiểm tra lại phần thưởng!");
        }

        /// <summary>Tách mã vật phẩm từ chuỗi quà "id,sl,...|id,sl,..." (lấy số đầu mỗi cụm ngăn bởi '|').</summary>
        private static IEnumerable<string> ParseRewardGoodsIds(string raw)
        {
            foreach (var chunk in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var first = chunk.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
                if (IsRealId(first)) yield return first!;
            }
        }

        private static bool IsRealId(string? s) =>
            !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s.Trim(), @"^\d+$") && s.Trim() != "0";

        private HashSet<string> LoadGoodsIds()
        {
            var path = Path.Combine(_configRoot, "Goods.xml");
            if (!File.Exists(path)) return new HashSet<string>();

            var stamp = File.GetLastWriteTimeUtc(path);
            if (_goodsCache != null && stamp == _goodsCacheStamp) return _goodsCache;

            var set = new HashSet<string>();
            try
            {
                // Quét nhanh thuộc tính ID của các thẻ <Item> (Goods.xml có ~9700 item)
                var text = File.ReadAllText(path);
                foreach (Match m in Regex.Matches(text, "<Item\\s+ID=\"(\\d+)\""))
                    set.Add(m.Groups[1].Value);
            }
            catch { /* giữ set rỗng → bỏ qua kiểm tra */ }

            _goodsCache = set;
            _goodsCacheStamp = stamp;
            return set;
        }
    }
}
