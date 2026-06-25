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
    public string? Comment { get; private set; }
    // FK mapping: field -> target key (for badge hints)
    public Dictionary<string, string?> FkTargets { get; } = new();

    [BindProperty(SupportsGet = true)] public string Key { get; set; } = "";
    [BindProperty(SupportsGet = true)] public string? Id { get; set; }

    public IActionResult OnGet()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;

        // Build FK targets map
        foreach (var fk in def.ForeignKeys)
            FkTargets[fk.Field] = string.IsNullOrEmpty(fk.TargetKey) ? null : fk.TargetKey;

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
            if (rec == null) { TempData["err"] = "Không tìm thấy mục " + Id; return RedirectToPage("/File", new { key = Key }); }
            Values = rec.Attributes;
            Comment = rec.Comment;
            Fields = ResolveFieldOrder(def, rec);
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        var def = EventRegistry.Get(Key);
        if (def == null) return NotFound();
        Def = def;

        // Thu thập mọi field từ form (prefix f_)
        var values = new Dictionary<string, string>();
        foreach (var k in Request.Form.Keys)
        {
            if (k.StartsWith("f_"))
            {
                var attr = k.Substring(2);
                // chỉ chấp nhận tên thuộc tính XML hợp lệ
                if (System.Text.RegularExpressions.Regex.IsMatch(attr, @"^[A-Za-z_][A-Za-z0-9_.-]*$"))
                    values[attr] = Request.Form[k].ToString();
            }
        }

        try
        {
            // ── Validate trước khi ghi ──────────────────────────────
            var validator = new XmlValidationService(_svc.ConfigRoot);
            var existing = _svc.LoadRecords(def);
            validator.Validate(def, values, string.IsNullOrEmpty(Id), existing);

            // ── Ghi XML ─────────────────────────────────────────────
            if (string.IsNullOrEmpty(Id))
            {
                if (!values.TryGetValue(def.IdAttr, out var nid) || string.IsNullOrWhiteSpace(nid))
                { TempData["err"] = $"Phải nhập {def.IdAttr}."; return RedirectToPage(new { key = Key }); }
                _svc.AddRecord(def, values);
                TempData["msg"] = $"Đã thêm mục {nid}.";
            }
            else
            {
                _svc.UpdateRecord(def, Id, values);
                TempData["msg"] = $"Đã lưu mục {Id}.";
            }
        }
        catch (InvalidOperationException ex)
        {
            // Lỗi validation — hiển thị cho user
            TempData["err"] = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            TempData["err"] = ex.Message;
        }
        return RedirectToPage("/File", new { key = Key });
    }

    private static List<string> ResolveFieldOrder(EventFileDef def, EventRecord? rec)
    {
        if (rec != null) return rec.Attributes.Keys.ToList();
        return def.ListColumns.ToList();
    }

    public string? Label(string attr) => EventRegistry.AttrLabels.GetValueOrDefault(attr);
}
