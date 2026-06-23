using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class GoodsAuditModel : PageModel
{
    private readonly GoodsAuditService _audit;
    private readonly XmlEventService _svc;
    public GoodsAuditModel(GoodsAuditService audit, XmlEventService svc)
    {
        _audit = audit;
        _svc = svc;
    }

    public bool RootExists { get; private set; }
    public GoodsAuditResult Result { get; private set; } = new();
    [BindProperty(SupportsGet = true)] public string? Q { get; set; }

    public IActionResult OnGet(bool refresh = false)
    {
        RootExists = _svc.RootExists;
        if (!RootExists) return Page();
        Result = _audit.Get(refresh);
        return Page();
    }

    public IActionResult OnPostRefresh()
    {
        _audit.Get(forceRefresh: true);
        return RedirectToPage();
    }
}
