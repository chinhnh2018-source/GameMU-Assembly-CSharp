using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class FileModel : PageModel
{
    private readonly XmlEventService _svc;
    private readonly LinkResolutionService _links;
    public FileModel(XmlEventService svc, LinkResolutionService links)
    {
        _svc = svc;
        _links = links;
    }

    public EventFileDef Def { get; private set; } = null!;
    public List<EventRecord> Records { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public string Key { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string? Q { get; set; }
    [BindProperty(SupportsGet = true)] public string? State { get; set; } // all|on|off
    // FK lookups for each record
    public Dictionary<string, List<ResolvedLink>> ForwardLinks { get; } = new();
    public Dictionary<string, List<BackReference>> BackRefs { get; } = new();
    // Known FK fields map: fieldName -> ForeignKeyRef
    public Dictionary<string, ForeignKeyRef> FkFields { get; } = new();
    // Cache: TargetKey -> EventFileDef
    public Dictionary<string, EventFileDef?> TargetDefs { get; } = new();

    public IActionResult OnGet()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;

        // Build FK field map
        foreach (var fk in def.ForeignKeys)
            FkFields[fk.Field] = fk;
        foreach (var fk in def.ForeignKeys)
            if (!string.IsNullOrEmpty(fk.TargetKey))
                TargetDefs[fk.TargetKey] = EventRegistry.Get(fk.TargetKey);

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

        // Resolve FK links for shown records
        foreach (var rec in Records)
        {
            var fwd = _links.GetForwardLinks(def, rec);
            if (fwd.Count > 0) ForwardLinks[rec.Id] = fwd;

            var bk = _links.GetBackReferences(def, rec.Id);
            if (bk.Count > 0) BackRefs[rec.Id] = bk;
        }

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
