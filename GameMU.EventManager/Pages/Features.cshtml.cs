using GameMU.EventManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameMU.EventManager.Pages;

public class FeaturesModel : PageModel
{
    public string? Q { get; set; }
    public List<FeatureItem> Items { get; private set; } = new();

    public void OnGet(string? q)
    {
        Q = q;
        IEnumerable<FeatureItem> items = FeatureCatalog.Items;
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim().ToLowerInvariant();
            items = items.Where(i =>
                i.Feature.ToLowerInvariant().Contains(s) ||
                i.Category.ToLowerInvariant().Contains(s) ||
                i.Manager.ToLowerInvariant().Contains(s) ||
                i.Xmls.Any(x => x.ToLowerInvariant().Contains(s)));
        }
        Items = items.OrderBy(i => i.Category).ThenBy(i => i.Feature).ToList();
    }
}
