using GameMU.EventManager.Models;
using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class IndexModel : PageModel
{
    private readonly XmlEventService _svc;
    public IndexModel(XmlEventService svc) => _svc = svc;

    public bool RootExists { get; private set; }
    public string ConfigRoot { get; private set; } = "";
    public Dictionary<string, int> Counts { get; } = new();
    public Dictionary<string, bool> Exists { get; } = new();

    public void OnGet()
    {
        ConfigRoot = _svc.ConfigRoot;
        RootExists = _svc.RootExists;
        if (!RootExists) return;
        foreach (var def in EventRegistry.Files)
        {
            Exists[def.Key] = _svc.FileExists(def);
            Counts[def.Key] = Exists[def.Key] ? _svc.CountRecords(def) : 0;
        }
    }
}
