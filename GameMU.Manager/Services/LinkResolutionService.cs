using System.Text.RegularExpressions;
using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

/// <summary>
/// Tra c?u liên k?t gi?a các record XML — c? "FK -> dích" và "dích <- d?n xu?t".
/// </summary>
public class LinkResolutionService
{
    private readonly XmlEventService _svc;

    public LinkResolutionService(XmlEventService svc) => _svc = svc;

    /// <summary>L?y danh sách record dích mà m?t record dang tham chi?u t?i (FK -> target).</summary>
    public List<ResolvedLink> GetForwardLinks(EventFileDef def, EventRecord rec)
    {
        var result = new List<ResolvedLink>();
        foreach (var fk in def.ForeignKeys)
        {
            if (!rec.Attributes.TryGetValue(fk.Field, out var raw) || string.IsNullOrWhiteSpace(raw))
                continue;

            var targetDef = EventRegistry.Get(fk.TargetKey);
            if (targetDef == null) continue;

            var ids = ExtractIds(raw, fk);
            foreach (var id in ids)
            {
                var targetRec = _svc.GetRecord(targetDef, id);
                result.Add(new ResolvedLink
                {
                    SourceField = fk.Field,
                    SourceValue = id,
                    TargetDef = targetDef,
                    TargetRec = targetRec,
                    Description = fk.Description,
                    Resolved = targetRec != null
                });
            }
        }
        return result;
    }

    /// <summary>L?y danh sách record trong các file khác dang tham chi?u d?n record này (back-reference).</summary>
    public List<BackReference> GetBackReferences(EventFileDef def, string id)
    {
        var result = new List<BackReference>();
        foreach (var otherDef in EventRegistry.Files)
        {
            if (otherDef.Key == def.Key) continue;
            if (otherDef.ForeignKeys.Count == 0) continue;

            List<EventRecord> records;
            try { records = _svc.LoadRecords(otherDef); }
            catch { continue; }

            foreach (var fk in otherDef.ForeignKeys)
            {
                foreach (var rec in records)
                {
                    if (!rec.Attributes.TryGetValue(fk.Field, out var raw) || string.IsNullOrWhiteSpace(raw))
                        continue;
                    var ids = ExtractIds(raw, fk);
                    if (ids.Contains(id))
                    {
                        result.Add(new BackReference
                        {
                            SourceDef = otherDef,
                            SourceRec = rec,
                            SourceField = fk.Field,
                            Description = fk.Description
                        });
                    }
                }
            }
        }
        return result;
    }

    private static HashSet<string> ExtractIds(string raw, ForeignKeyRef fk)
    {
        var set = new HashSet<string>();
        if (fk.ParseRewardList)
        {
            // "id,sl,...|id,sl,..." -> l?y s? d?u m?i c?m
            foreach (var chunk in raw.Split('|', StringSplitOptions.RemoveEmptyEntries))
            {
                var first = chunk.Split(',', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim();
                if (IsRealId(first)) set.Add(first!);
            }
        }
        else if (fk.MultiValue)
        {
            foreach (var part in raw.Split(fk.MultiSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                var t = part.Trim();
                if (IsRealId(t)) set.Add(t);
            }
        }
        else
        {
            var t = raw.Trim();
            if (IsRealId(t)) set.Add(t);
        }
        return set;
    }

    private static bool IsRealId(string? s) =>
        !string.IsNullOrWhiteSpace(s) && Regex.IsMatch(s.Trim(), @"^\d+$") && s.Trim() != "0";
}

public class ResolvedLink
{
    public string SourceField { get; init; } = "";
    public string SourceValue { get; init; } = "";
    public EventFileDef? TargetDef { get; init; }
    public EventRecord? TargetRec { get; init; }
    public string Description { get; init; } = "";
    public bool Resolved { get; init; }
}

public class BackReference
{
    public EventFileDef SourceDef { get; init; } = null!;
    public EventRecord SourceRec { get; init; } = null!;
    public string SourceField { get; init; } = "";
    public string Description { get; init; } = "";
}
