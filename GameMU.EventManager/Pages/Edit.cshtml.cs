using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class EditModel : PageModel
{
    private readonly XmlEventService _svc;
    public EditModel(XmlEventService svc) => _svc = svc;

    public EventFileDef Def { get; private set; } = null!;
    public bool IsNew { get; private set; }
    public List<string> Fields { get; private set; } = new();
    public Dictionary<string, string> Values { get; private set; } = new();

    [BindProperty(SupportsGet = true)] public string Key { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string? Id { get; set; }

    public IActionResult OnGet()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;

        if (string.IsNullOrEmpty(Id))
        {
            IsNew = true;
            var sample = _svc.LoadRecords(def).FirstOrDefault();
            Fields = sample != null ? sample.Attributes.Keys.ToList() : def.ListColumns.ToList();
            Values = Fields.ToDictionary(f => f, _ => "");
        }
        else
        {
            var rec = _svc.GetRecord(def, Id);
            if (rec == null) { TempData["err"] = "Khong tim thay muc " + Id; return RedirectToPage("/File", new { key = Key }); }
            Values = rec.Attributes;
            Fields = ResolveFieldOrder(def, rec);
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;

        // Thu thap moi field tu form (prefix f_)
        var values = new Dictionary<string, string>();
        foreach (var k in Request.Form.Keys)
        {
            if (k.StartsWith("f_"))
            {
                var attr = k.Substring(2);
                // chi chap nhan ten thuoc tinh XML hop le
                if (System.Text.RegularExpressions.Regex.IsMatch(attr, "^[A-Za-z_][A-Za-z0-9_.-]*$"))
                    values[attr] = Request.Form[k].ToString();
            }
        }

        try
        {
            if (string.IsNullOrEmpty(Id))
            {
                if (!values.TryGetValue(def.IdAttr, out var nid) || string.IsNullOrWhiteSpace(nid))
                { TempData["err"] = $"Phai nhap {def.IdAttr}."; return RedirectToPage(new { key = Key }); }
                _svc.AddRecord(def, values);
                TempData["msg"] = $"Da them muc {nid}.";
            }
            else
            {
                _svc.UpdateRecord(def, Id, values);
                TempData["msg"] = $"Da luu muc {Id}.";
            }
        }
        catch (Exception ex) { TempData["err"] = ex.Message; }
        return RedirectToPage("/File", new { key = Key });
    }

    private static List<string> ResolveFieldOrder(EventFileDef def, EventRecord? rec)
    {
        if (rec != null) return rec.Attributes.Keys.ToList();
        return def.ListColumns.ToList();
    }

    public string? Label(string attr) => EventRegistry.AttrLabels.GetValueOrDefault(attr);
}
