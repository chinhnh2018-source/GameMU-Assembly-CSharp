using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class FileModel : PageModel
{
    private readonly XmlEventService _svc;
    public FileModel(XmlEventService svc) => _svc = svc;

    public EventFileDef Def { get; private set; } = null!;
    public List<EventRecord> Records { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public string Key { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public string? State { get; set; } // all|on|off

    public IActionResult OnGet()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;
        if (!_svc.FileExists(def)) { TempData["err"] = "Khong tim thay file: " + def.RelativePath; Records = new(); return Page(); }

        var all = _svc.LoadRecords(def);
        IEnumerable<EventRecord> q = all;
        if (!string.IsNullOrWhiteSpace(Q))
        {
            var s = Q.Trim().ToLowerInvariant();
            q = q.Where(r => r.Id.ToLowerInvariant().Contains(s)
                          || r.Name.ToLowerInvariant().Contains(s)
                          || r.Attributes.Values.Any(v => v.ToLowerInvariant().Contains(s)));
        }
        if (State == "on") q = q.Where(r => r.Enabled);
        else if (State == "off") q = q.Where(r => !r.Enabled);
        Records = q.ToList();
        return Page();
    }

    public IActionResult OnPostToggle(string key, string id, bool enable)
    {
        var def = EventRegistry.Get(key);
        if (def == null) return NotFound();
        try { _svc.Toggle(def, id, enable); TempData["msg"] = $"Da {(enable ? "BAT" : "TAT")} muc {id}."; }
        catch (Exception ex) { TempData["err"] = ex.Message; }
        return RedirectToPage(new { key, Q, State });
    }

    public IActionResult OnPostDelete(string key, string id)
    {
        var def = EventRegistry.Get(key);
        if (def == null) return NotFound();
        try { _svc.DeleteRecord(def, id); TempData["msg"] = $"Da xoa muc {id}."; }
        catch (Exception ex) { TempData["err"] = ex.Message; }
        return RedirectToPage(new { key, Q, State });
    }
}
