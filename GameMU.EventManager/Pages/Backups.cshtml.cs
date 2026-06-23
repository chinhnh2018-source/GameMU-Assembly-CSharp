using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class BackupsModel : PageModel
{
    private readonly XmlEventService _svc;
    public BackupsModel(XmlEventService svc) => _svc = svc;
    public List<string> Backups { get; private set; } = new();
    public string Root { get; private set; } = "";
    public void OnGet() { Backups = _svc.ListBackups(); Root = _svc.ConfigRoot; }
}
