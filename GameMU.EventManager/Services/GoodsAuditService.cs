using System.Text.RegularExpressions;

namespace GameMU.EventManager.Services;

/// <summary>K?t qu? doi soat 1 (file, tru?ng) tham chi?u Goods.xml.</summary>
public class GoodsRefRow
{
    public string File { get; init; } = "";
    public string Field { get; init; } = "";
    public int TotalRefs { get; set; }          // t?ng s? ID tham chi?u (da b? 0/-1, da distinct theo record)
    public int Missing { get; set; }            // s? ID khong t?n t?i trong Goods.xml
    public List<string> MissingSamples { get; } = new();  // vai ID thi?u dau tien
    public double MatchRate => TotalRefs == 0 ? 1 : (double)(TotalRefs - Missing) / TotalRefs;
    public bool IsManaged { get; set; }         // file co dang du?c EventRegistry qu?n ly khong
    public string? ManagedKey { get; set; }     // key registry n?u co
}

public class GoodsAuditResult
{
    public int GoodsTotal { get; set; }
    public int FilesScanned { get; set; }
    public int FieldsFound { get; set; }
    public DateTime ScannedAt { get; set; }
    public List<GoodsRefRow> Rows { get; } = new();   // moi (file, field)
    // Lien ket hong THAT SU: truong khop cao (>=90%) nhung con vai ma khong ton tai.
    public List<GoodsRefRow> Broken => Rows.Where(r => r.Missing > 0 && r.MatchRate >= 0.90)
                                           .OrderByDescending(r => r.Missing).ToList();
    // Truong khop thap: ten giong goods nhung gia tri phan lon KHONG khop -> nhieu kha nang
    // tham chieu id-space khac (so luong, diem, ma goi...), KHONG phai Goods.xml.
    public List<GoodsRefRow> Suspect => Rows.Where(r => r.Missing > 0 && r.MatchRate < 0.90)
                                            .OrderBy(r => r.MatchRate).ToList();
    public List<GoodsRefRow> Clean => Rows.Where(r => r.Missing == 0).OrderByDescending(r => r.TotalRefs).ToList();
}

/// <summary>
/// Quet toan b? GameRes/Config, phat hi?n cac tru?ng tham chi?u ma v?t ph?m
/// va doi soat tr?c ti?p v?i Goods.xml. K?t qu? du?c cache (refresh th? cong).
/// </summary>
public class GoodsAuditService
{
    private readonly XmlEventService _svc;
    private GoodsAuditResult? _cache;
    private readonly object _lock = new();

    public GoodsAuditService(XmlEventService svc) => _svc = svc;

    // Tap ten tru?ng goods da DOI SOAT (quet dinh lu?ng ~100% kh?p Goods.xml).
    private static readonly HashSet<string> GoodsFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "GoodsID","GoodsId","GoodsIDs","GoodsID1","GoodsIDOne","ItemID","ItemId",
        "GoodsOne","GoodsTwo","GoodsThr","GoodsFour","Goods","GoodsList","GoodsIDs",
        "GoodsCost","CostGoods","CostGoldGoods","NeedGoods","LossItem","XiaoHuiGoods",
        "Award","Awards","AwardGoods","DayAward","EventAward","LiBaoAward","AchievementReward",
        "Items","PetGoods","HorseGoods","OrnamentGoods","ProtectGoods","AddedGoods","ShowGoods",
        "NewGoods","OldGoods","NewGoodsID","OldGoodsID","ReplaceGoods","DuiHuanGoodsIDs",
        "WinGoods","LoseGoods","WinRewardItem","LoseRewardItem","FightAward","KillAward",
        "Reward","RewardGoods","LeaderReward","MasterReward","FirstGoodsID","FirstWinRankReward",
        "GLGoods","VIPGoodsIDs","KillExtraAward","SeasonReward","WinAward","LoseAward","ShowAward"
    };

    public GoodsAuditResult Get(bool forceRefresh = false)
    {
        lock (_lock)
        {
            if (_cache != null && !forceRefresh) return _cache;
            _cache = Scan();
            return _cache;
        }
    }

    private GoodsAuditResult Scan()
    {
        var res = new GoodsAuditResult { ScannedAt = DateTime.Now };
        var root = _svc.ConfigRoot;
        if (!Directory.Exists(root)) return res;

        // 1) Goods.xml ID set
        var goods = LoadGoodsIds(root);
        res.GoodsTotal = goods.Count;
        if (goods.Count == 0) return res;

        // 2) Map ten file -> key registry (de gan link "dang quan ly")
        var managed = EventRegistry.Files
            .GroupBy(f => Path.GetFileName(f.RelativePath), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Key, StringComparer.OrdinalIgnoreCase);

        var attrRe = new Regex("(\\w+)=\"([^\"]*)\"", RegexOptions.Compiled);

        foreach (var path in Directory.EnumerateFiles(root, "*.xml", SearchOption.AllDirectories))
        {
            var baseName = Path.GetFileName(path);
            if (baseName.Equals("Goods.xml", StringComparison.OrdinalIgnoreCase)) continue;
            if (path.Contains("_EventManager")) continue;   // bo qua thu muc cong cu

            string content;
            try { content = File.ReadAllText(path); } catch { continue; }

            res.FilesScanned++;
            // gom theo field trong pham vi file nay
            var perField = new Dictionary<string, GoodsRefRow>(StringComparer.OrdinalIgnoreCase);

            foreach (Match m in attrRe.Matches(content))
            {
                var attr = m.Groups[1].Value;
                if (!GoodsFields.Contains(attr)) continue;
                var ids = ExtractIds(m.Groups[2].Value);
                if (ids.Count == 0) continue;

                if (!perField.TryGetValue(attr, out var row))
                {
                    var rel = Path.GetRelativePath(root, path).Replace('\\', '/');
                    row = new GoodsRefRow
                    {
                        File = rel, Field = attr,
                        IsManaged = managed.ContainsKey(baseName),
                        ManagedKey = managed.GetValueOrDefault(baseName)
                    };
                    perField[attr] = row;
                }
                foreach (var id in ids)
                {
                    row.TotalRefs++;
                    if (!goods.Contains(id))
                    {
                        row.Missing++;
                        if (row.MissingSamples.Count < 8 && !row.MissingSamples.Contains(id))
                            row.MissingSamples.Add(id);
                    }
                }
            }

            foreach (var row in perField.Values.Where(r => r.TotalRefs > 0))
                res.Rows.Add(row);
        }

        res.FieldsFound = res.Rows.Count;
        return res;
    }

    /// <summary>Tach ma v?t ph?m: tach theo '|', m?i c?m l?y s? d?u (sau ','). B? 0/-1.</summary>
    private static List<string> ExtractIds(string raw)
    {
        var ids = new List<string>();
        raw = raw.Trim();
        if (string.IsNullOrEmpty(raw)) return ids;
        foreach (var chunk in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
        {
            var first = chunk.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
            if (IsRealId(first)) ids.Add(first!);
        }
        return ids;
    }

    private static bool IsRealId(string? s) =>
        !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s.Trim(), @"^\d+$") && s.Trim() != "0";

    private static HashSet<string> LoadGoodsIds(string root)
    {
        var set = new HashSet<string>();
        var path = Path.Combine(root, "Goods.xml");
        if (!File.Exists(path)) return set;
        try
        {
            var text = File.ReadAllText(path);
            foreach (Match m in Regex.Matches(text, "<Item\\s+ID=\"(\\d+)\""))
                set.Add(m.Groups[1].Value);
        }
        catch { }
        return set;
    }
}
