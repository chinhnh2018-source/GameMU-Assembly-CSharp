using System.Globalization;
using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class CalendarModel : PageModel
{
    private readonly XmlEventService _svc;
    public CalendarModel(XmlEventService svc) => _svc = svc;

    public bool RootExists { get; private set; }
    public string ConfigRoot { get; private set; } = "";

    public List<TimelineGroup> Groups { get; } = new();
    public List<TimelineItem> Inactive { get; } = new();   // chưa cấu hình hoặc đã tắt
    public List<OverlapWarning> Overlaps { get; } = new();

    public DateTime Now { get; private set; }
    public DateTime RangeStart { get; private set; }
    public DateTime RangeEnd { get; private set; }

    private static readonly string[] DateFormats =
    {
        "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd"
    };

    public void OnGet()
    {
        Now = DateTime.Now;
        ConfigRoot = _svc.ConfigRoot;
        RootExists = _svc.RootExists;
        if (!RootExists) return;

        var all = new List<TimelineItem>();

        foreach (var def in EventRegistry.Files.Where(d => d.Toggle == ToggleStrategy.DateWindow))
        {
            if (!_svc.FileExists(def)) continue;
            List<EventRecord> recs;
            try { recs = _svc.LoadRecords(def); } catch { continue; }

            foreach (var r in recs)
            {
                var from = ParseDate(r.Attributes.GetValueOrDefault(def.FromAttr));
                var to = ParseDate(r.Attributes.GetValueOrDefault(def.ToAttr));

                var item = new TimelineItem
                {
                    Category = def.Category,
                    FileKey = def.Key,
                    FileName = def.DisplayName,
                    Id = r.Id,
                    Name = string.IsNullOrWhiteSpace(r.Name) ? r.Id : r.Name,
                    From = from,
                    To = to
                };

                if (from == null && to == null) item.Status = "unconfigured";
                else if (to != null && to.Value.Year <= 2001) item.Status = "off";
                else if (from != null && Now < from.Value) item.Status = "upcoming";
                else if (to != null && Now > to.Value) item.Status = "ended";
                else item.Status = "running";

                all.Add(item);
            }
        }

        // Tách nhóm chưa cấu hình / đã tắt ra danh sách riêng
        Inactive.AddRange(all.Where(i => i.Status is "unconfigured" or "off")
                             .OrderBy(i => i.Category).ThenBy(i => i.Name));

        var dated = all.Where(i => i.Status is "running" or "upcoming" or "ended").ToList();

        // Tính khoảng thời gian hiển thị (trục timeline)
        var marks = new List<DateTime>();
        foreach (var i in dated)
        {
            if (i.From != null) marks.Add(i.From.Value);
            if (i.To != null) marks.Add(i.To.Value);
        }
        if (marks.Count > 0)
        {
            RangeStart = marks.Min();
            RangeEnd = marks.Max();
        }
        else
        {
            RangeStart = Now.AddDays(-7);
            RangeEnd = Now.AddDays(30);
        }
        if (Now < RangeStart) RangeStart = Now;
        if (Now > RangeEnd) RangeEnd = Now;

        var span = (RangeEnd - RangeStart).TotalDays;
        if (span < 1) { RangeStart = RangeStart.AddDays(-1); RangeEnd = RangeEnd.AddDays(1); span = (RangeEnd - RangeStart).TotalDays; }
        RangeStart = RangeStart.AddDays(-span * 0.04);
        RangeEnd = RangeEnd.AddDays(span * 0.04);

        Groups.AddRange(dated.GroupBy(i => i.Category)
            .Select(g => new TimelineGroup
            {
                Category = g.Key,
                Items = g.OrderBy(x => x.From ?? DateTime.MinValue).ToList()
            })
            .OrderBy(g => g.Category));

        // Phát hiện trùng giờ trong cùng một file (chỉ với sự kiện đang/sắp chạy có đủ 2 mốc)
        foreach (var fileGrp in dated.Where(i => i.Status is "running" or "upcoming")
                                     .GroupBy(i => i.FileKey))
        {
            var list = fileGrp.Where(i => i.From != null && i.To != null)
                              .OrderBy(i => i.From).ToList();
            for (int a = 0; a < list.Count; a++)
                for (int b = a + 1; b < list.Count; b++)
                    if (list[a].From < list[b].To && list[b].From < list[a].To)
                        Overlaps.Add(new OverlapWarning { FileName = fileGrp.First().FileName, A = list[a], B = list[b] });
        }
    }

    // ----- Helper định vị thanh bar theo phần trăm -----
    public double LeftPct(TimelineItem i)
    {
        var total = (RangeEnd - RangeStart).TotalSeconds;
        if (total <= 0) return 0;
        var start = (i.From ?? RangeStart);
        var p = (start - RangeStart).TotalSeconds / total * 100.0;
        return Math.Clamp(p, 0, 100);
    }

    public double WidthPct(TimelineItem i)
    {
        var total = (RangeEnd - RangeStart).TotalSeconds;
        if (total <= 0) return 2;
        var start = (i.From ?? RangeStart);
        var end = (i.To ?? RangeEnd);
        var p = (end - start).TotalSeconds / total * 100.0;
        return Math.Clamp(p, 1.5, 100 - LeftPct(i));
    }

    public double NowPct()
    {
        var total = (RangeEnd - RangeStart).TotalSeconds;
        if (total <= 0) return 0;
        return Math.Clamp((Now - RangeStart).TotalSeconds / total * 100.0, 0, 100);
    }

    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s) || s.Trim() == "-1") return null;
        if (DateTime.TryParseExact(s.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt;
        if (DateTime.TryParse(s.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
        return null;
    }
}

public class TimelineItem
{
    public string Category { get; set; } = "";
    public string FileKey { get; set; } = "";
    public string FileName { get; set; } = "";
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string Status { get; set; } = "";   // running|upcoming|ended|off|unconfigured
}

public class TimelineGroup
{
    public string Category { get; set; } = "";
    public List<TimelineItem> Items { get; set; } = new();
}

public class OverlapWarning
{
    public string FileName { get; set; } = "";
    public TimelineItem A { get; set; } = null!;
    public TimelineItem B { get; set; } = null!;
}
