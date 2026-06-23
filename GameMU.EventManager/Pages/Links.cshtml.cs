using System.Globalization;
using System.Text.RegularExpressions;
using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class LinksModel : PageModel
{
    private readonly XmlEventService _svc;
    public LinksModel(XmlEventService svc) => _svc = svc;

    public bool RootExists { get; private set; }
    public string ConfigRoot { get; private set; } = "";

    public List<XmlLink> Links => LinkRegistry.Links;

    // Kết quả đối soát SpecialActivity.GroupID ↔ SpecialActivityTime.GroupID
    public List<ActivityGroup> Groups { get; } = new();
    public List<string> OrphanGroups { get; } = new();   // GroupID có Activity nhưng thiếu <Time>
    public List<string> UnusedTimes { get; } = new();    // GroupID có <Time> nhưng không Activity nào dùng

    // Đối soát GoodsID
    public int GoodsTotal { get; private set; }
    public int GoodsRefCount { get; private set; }
    public List<string> MissingGoods { get; } = new();

    private static readonly string[] DateFormats =
    { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd" };

    public void OnGet()
    {
        ConfigRoot = _svc.ConfigRoot;
        RootExists = _svc.RootExists;
        if (!RootExists) return;

        var actDef = EventRegistry.Get("special-activity");
        var timeDef = EventRegistry.Get("special-activity-time");
        if (actDef == null || timeDef == null) return;

        var activities = Safe(() => _svc.LoadRecords(actDef));
        var times = Safe(() => _svc.LoadRecords(timeDef));

        // Map khung thời gian theo GroupID
        var timeByGroup = new Dictionary<string, (DateTime? from, DateTime? to)>();
        foreach (var t in times)
        {
            var gid = t.Attributes.GetValueOrDefault("GroupID");
            if (string.IsNullOrWhiteSpace(gid)) continue;
            timeByGroup[gid] = (Parse(t.Attributes.GetValueOrDefault("FromDate")),
                                Parse(t.Attributes.GetValueOrDefault("ToDate")));
        }

        // Gom Activity theo GroupID
        var usedGroups = new HashSet<string>();
        foreach (var grp in activities.GroupBy(a => a.Attributes.GetValueOrDefault("GroupID") ?? ""))
        {
            if (string.IsNullOrWhiteSpace(grp.Key)) continue;
            usedGroups.Add(grp.Key);

            var hasTime = timeByGroup.TryGetValue(grp.Key, out var win);
            var g = new ActivityGroup
            {
                GroupId = grp.Key,
                Name = grp.First().Attributes.GetValueOrDefault("Name") ?? "",
                ActivityCount = grp.Count(),
                HasTime = hasTime,
                From = hasTime ? win.from : null,
                To = hasTime ? win.to : null
            };
            Groups.Add(g);
            if (!hasTime) OrphanGroups.Add(grp.Key);
        }

        foreach (var gid in timeByGroup.Keys)
            if (!usedGroups.Contains(gid)) UnusedTimes.Add(gid);

        Groups.Sort((a, b) => string.Compare(b.GroupId, a.GroupId, StringComparison.Ordinal));

        // Đối soát GoodsID trong SpecialActivity với Goods.xml
        var goods = LoadGoodsIds();
        GoodsTotal = goods.Count;
        var refs = new HashSet<string>();
        foreach (var a in activities)
            foreach (var key in new[] { "GoodsOne", "GoodsTwo", "GoodsThr" })
            {
                var raw = a.Attributes.GetValueOrDefault(key);
                if (string.IsNullOrWhiteSpace(raw)) continue;
                foreach (var chunk in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
                {
                    var id = chunk.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
                    if (!string.IsNullOrEmpty(id) && Regex.IsMatch(id, @"^\d+$") && id != "0")
                        refs.Add(id);
                }
            }
        GoodsRefCount = refs.Count;
        if (goods.Count > 0)
            foreach (var id in refs)
                if (!goods.Contains(id)) MissingGoods.Add(id);
    }

    private HashSet<string> LoadGoodsIds()
    {
        var path = Path.Combine(ConfigRoot, "Goods.xml");
        var set = new HashSet<string>();
        if (!System.IO.File.Exists(path)) return set;
        try
        {
            var text = System.IO.File.ReadAllText(path);
            foreach (Match m in Regex.Matches(text, "<Item\\s+ID=\"(\\d+)\""))
                set.Add(m.Groups[1].Value);
        }
        catch { }
        return set;
    }

    private static List<EventRecord> Safe(Func<List<EventRecord>> f)
    {
        try { return f(); } catch { return new(); }
    }

    private static DateTime? Parse(string? s)
    {
        if (string.IsNullOrWhiteSpace(s) || s.Trim() == "-1") return null;
        if (DateTime.TryParseExact(s.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)) return dt;
        if (DateTime.TryParse(s.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
        return null;
    }
}

public class ActivityGroup
{
    public string GroupId { get; set; } = "";
    public string Name { get; set; } = "";
    public int ActivityCount { get; set; }
    public bool HasTime { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
