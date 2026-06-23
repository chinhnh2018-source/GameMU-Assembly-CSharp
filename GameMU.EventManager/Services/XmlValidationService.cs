using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services
{
    public class XmlValidationService
    {
        private readonly string _configRoot;

        public XmlValidationService(string configRoot)
        {
            _configRoot = configRoot;
        }

        /// <summary>
        /// Thực hiện kiểm tra toàn bộ ràng buộc nghiệp vụ của XML trước khi lưu file
        /// </summary>
        public void Validate(EventFileDef def, Dictionary<string, string> values, bool isNew, List<EventRecord> existingRecords)
        {
            // Rule 1: Kiểm tra trùng lặp khóa chính (ID) đối với bản ghi thêm mới
            if (isNew && values.TryGetValue(def.IdAttr, out var newId))
            {
                if (existingRecords.Any(r => r.Id == newId))
                {
                    throw new InvalidOperationException($"[LỖI TRÙNG ID] Mã định danh '{newId}' đã tồn tại trong tệp '{def.RelativePath}'. Vui lòng sử dụng ID khác!");
                }
            }

            // Rule 2: Kiểm tra ràng buộc thời gian hợp lệ (FromDate < ToDate)
            ValidateTimeWindow(values);

            // Rule 3: Kiểm tra giới hạn tỷ lệ phần trăm (0 - 100) đối với thông số SystemParams
            if (def.Key == "system-params")
            {
                ValidateSystemParams(values);
            }

            // Rule 4: Kiểm tra ràng buộc liên file (Mã vật phẩm GoodsID phải tồn tại trong Goods.xml)
            ValidateGoodsReference(values);
        }

        private void ValidateTimeWindow(Dictionary<string, string> values)
        {
            string[] startKeys = { "FromDate", "BeginTime", "StartTime" };
            string[] endKeys = { "ToDate", "FinishTime", "EndTime" };

            string startAttr = startKeys.FirstOrDefault(values.ContainsKey);
            string endAttr = endKeys.FirstOrDefault(values.ContainsKey);

            if (startAttr != null && endAttr != null)
            {
                var startVal = values[startAttr].Trim();
                var endVal = values[endAttr].Trim();

                if (startVal != "-1" && endVal != "-1" && startVal != "2000-01-01 00:00:00" && endVal != "2000-01-01 00:00:00")
                {
                    if (DateTime.TryParse(startVal, out var start) && DateTime.TryParse(endVal, out var end))
                    {
                        if (start >= end)
                        {
                            throw new InvalidOperationException($"[LỖI LOGIC THỜI GIAN] Thời gian bắt đầu '{startVal}' không thể lớn hơn hoặc bằng thời gian kết thúc '{endVal}'!");
                        }
                    }
                }
            }
        }

        private void ValidateSystemParams(Dictionary<string, string> values)
        {
            if (values.TryGetValue("Name", out var name) && values.TryGetValue("Value", out var valStr))
            {
                if (name.Contains("Percent") || name.Contains("Rate"))
                {
                    if (double.TryParse(valStr, out var percent))
                    {
                        if (percent < 0 || percent > 100)
                        {
                            throw new InvalidOperationException($"[LỖI THÔNG SỐ CÂN BẰNG] Tham số tỷ lệ '{name}' bắt buộc phải nằm trong khoảng từ [0] đến [100]%. Giá trị '{valStr}' bạn nhập không hợp lệ!");
                        }
                    }
                }
            }
        }

        private void ValidateGoodsReference(Dictionary<string, string> values)
        {
            string[] goodsKeys = { "GoodsID", "GoodsId", "ItemID", "ItemId", "AwardID", "AwardId" };
            string foundKey = goodsKeys.FirstOrDefault(values.ContainsKey);

            if (foundKey != null && !string.IsNullOrWhiteSpace(values[foundKey]))
            {
                var goodsIdRaw = values[foundKey].Trim();
                if (goodsIdRaw == "-1") return;

                // Tách chuỗi nếu cấu hình chứa danh sách quà (VD: "1001|1|1002|5")
                var idsToCheck = goodsIdRaw.Contains('|')
                    ? goodsIdRaw.Split('|').Where((val, index) => index % 2 == 0).ToList()
                    : new List<string> { goodsIdRaw };

                var goodsXmlPath = Path.Combine(_configRoot, "Goods.xml");
                if (File.Exists(goodsXmlPath))
                {
                    try
                    {
                        var doc = XDocument.Load(goodsXmlPath);
                        var validIds = doc.Descendants("Goods")
                                          .Select(g => g.Attribute("ID")?.Value)
                                          .Where(id => id != null)
                                          .ToHashSet();

                        foreach (var id in idsToCheck)
                        {
                            if (!validIds.Contains(id))
                            {
                                throw new InvalidOperationException($"[LỖI LIÊN KẾT XML] Vật phẩm có ID '{id}' được cấu hình trong trường '{foundKey}' không tồn tại trong danh mục vật phẩm gốc 'Goods.xml'!");
                            }
                        }
                    }
                    catch (Exception ex) when (!(ex is InvalidOperationException))
                    {
                        // Bỏ qua lỗi cú pháp của Goods.xml nếu tệp gốc bị hỏng để tránh kẹt ứng dụng
                    }
                }
            }
        }
    }
}
