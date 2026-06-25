using System.IO.Compression;
using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

/// <summary>
/// Import/Export XML Config files dưới dạng ZIP bundle.
/// Export: đóng gói các XML files được chọn + toàn bộ backups vào 1 file ZIP.
/// Import: giải nén ZIP và ghi đè lên Config directory (sau khi backup).
/// </summary>
public class ZipService
{
    private readonly XmlEventService _svc;

    public ZipService(XmlEventService svc) => _svc = svc;

    /// <summary>
    /// Xuất TẤT CẢ XML files của EventRegistry vào 1 file ZIP.
    /// Trả về byte[] ZIP để download.
    /// </summary>
    public byte[] ExportAll(IEnumerable<string>? filterKeys = null)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            var defs = filterKeys != null
                ? EventRegistry.Files.Where(d => filterKeys.Contains(d.Key))
                : EventRegistry.Files;

            foreach (var def in defs)
            {
                var path = _svc.FullPath(def);
                if (!File.Exists(path)) continue;

                // Preserve relative path structure in ZIP
                var entryName = def.RelativePath.Replace('\\', '/');
                var entry = zip.CreateEntry(entryName, CompressionLevel.Optimal);
                using var stream = entry.Open();
                using var src = File.OpenRead(path);
                src.CopyTo(stream);
            }

            // Include manifest
            var manifest = zip.CreateEntry("_manifest.json");
            using var mw = new System.IO.StreamWriter(manifest.Open());
            var manifestData = new
            {
                ExportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ConfigRoot = _svc.ConfigRoot,
                FileCount = EventRegistry.Files.Count(d => File.Exists(_svc.FullPath(d))),
                Files = EventRegistry.Files
                    .Where(d => File.Exists(_svc.FullPath(d)))
                    .Select(d => new { d.Key, d.RelativePath, d.DisplayName })
            };
            mw.Write(System.Text.Json.JsonSerializer.Serialize(manifestData,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
        }
        return ms.ToArray();
    }

    /// <summary>
    /// Nhập ZIP và ghi đè XML files (tự backup trước).
    /// Trả về (importedCount, skippedCount, errors).
    /// </summary>
    public (int imported, int skipped, List<string> errors) ImportZip(Stream zipStream)
    {
        int imported = 0, skipped = 0;
        var errors = new List<string>();
        var root = _svc.ConfigRoot;

        using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
        foreach (var entry in zip.Entries)
        {
            if (entry.Name.StartsWith("_")) continue;              // skip manifest etc.
            if (!entry.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                skipped++;
                continue;
            }

            var destPath = Path.GetFullPath(Path.Combine(root, entry.FullName));
            // Security: ensure dest is within configRoot (path traversal prevention)
            if (!destPath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"[SECURITY] Đường dẫn không hợp lệ: {entry.FullName}");
                continue;
            }

            try
            {
                // Backup existing file if it exists
                if (File.Exists(destPath))
                {
                    var def = EventRegistry.Files.FirstOrDefault(d =>
                        string.Equals(d.RelativePath, entry.FullName, StringComparison.OrdinalIgnoreCase));
                    if (def != null) _svc.TriggerBackup(def);
                }

                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

                // Write file
                using var dest = File.Create(destPath);
                using var src = entry.Open();
                src.CopyTo(dest);

                imported++;
            }
            catch (Exception ex)
            {
                errors.Add($"[LỖI] {entry.FullName}: {ex.Message}");
            }
        }

        return (imported, skipped, errors);
    }
}
