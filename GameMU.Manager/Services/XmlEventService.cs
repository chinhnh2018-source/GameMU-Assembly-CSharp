using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

public class XmlEventService
{
    private readonly string _configRoot;
    private const string OffSentinel = "2000-01-01 00:00:00";
    private static readonly string[] DateFormats =
    {
        "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd"
    };

    public XmlEventService(IConfiguration config, IWebHostEnvironment env)
    {
        var p = config["EventManager:GameResConfigPath"] ?? "../GameMU-Assembly-CSharp/GameRes/GameRes/Config";
        _configRoot = Path.IsPathRooted(p) ? p : Path.GetFullPath(Path.Combine(env.ContentRootPath, p));
    }

    public string ConfigRoot => _configRoot;
    public bool RootExists => Directory.Exists(_configRoot);

    public string FullPath(EventFileDef def) => Path.Combine(_configRoot, def.RelativePath.Replace('/', Path.DirectorySeparatorChar));
    public bool FileExists(EventFileDef def) => File.Exists(FullPath(def));

    private string StateDir { get { var d = Path.Combine(_configRoot, "_EventManager", "state"); Directory.CreateDirectory(d); return d; } }
    private string BackupDir { get { var d = Path.Combine(_configRoot, "_EventManager", "backups"); Directory.CreateDirectory(d); return d; } }
    private string DisabledPath(EventFileDef def) => Path.Combine(StateDir, def.Key + ".disabled.xml");
    private string WindowStatePath(EventFileDef def) => Path.Combine(StateDir, def.Key + ".window.json");

    // ---------- Doc / status ----------

    public int CountRecords(EventFileDef def)
    {
        try { return LoadRecords(def).Count; } catch { return 0; }
    }

    // ---------- Cache LoadRecords (invalidate theo mtime file) ----------
    // Tránh parse lại file lớn (vd Goods.xml ~15MB) mỗi lần forward-link/back-reference quét.
    // Khóa = def.Key. Tự làm mới khi mtime của file gốc HOẶC file disabled sidecar thay đổi.
    private sealed class CacheEntry
    {
        public DateTime MainStamp;
        public DateTime DisabledStamp;
        public List<EventRecord> Records = new();
        public Dictionary<string, EventRecord> ById = new();
    }
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, CacheEntry> _cache = new();

    private static DateTime Stamp(string path) => File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;

    /// <summary>Xóa cache 1 file (gọi sau khi ghi). Bỏ qua key => xóa toàn bộ.</summary>
    public void InvalidateCache(EventFileDef? def = null)
    {
        if (def == null) _cache.Clear();
        else _cache.TryRemove(def.Key, out _);
    }

    public List<EventRecord> LoadRecords(EventFileDef def)
    {
        var path = FullPath(def);
        if (!File.Exists(path)) return new List<EventRecord>();

        var disabledPath = DisabledPath(def);
        var mainStamp = Stamp(path);
        var disabledStamp = def.Toggle == ToggleStrategy.Park ? Stamp(disabledPath) : DateTime.MinValue;

        if (_cache.TryGetValue(def.Key, out var hit)
            && hit.MainStamp == mainStamp && hit.DisabledStamp == disabledStamp)
            return hit.Records;

        var list = new List<EventRecord>();
        var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        foreach (var el in doc.Descendants(def.ItemElement))
            list.Add(BuildRecord(def, el, parked: false));

        if (def.Toggle == ToggleStrategy.Park && File.Exists(disabledPath))
        {
            var ddoc = XDocument.Load(disabledPath, LoadOptions.PreserveWhitespace);
            foreach (var el in ddoc.Descendants(def.ItemElement))
                list.Add(BuildRecord(def, el, parked: true));
        }

        var byId = new Dictionary<string, EventRecord>();
        foreach (var r in list) if (!string.IsNullOrEmpty(r.Id)) byId[r.Id] = r;

        _cache[def.Key] = new CacheEntry
        {
            MainStamp = mainStamp, DisabledStamp = disabledStamp, Records = list, ById = byId
        };
        return list;
    }

    private EventRecord BuildRecord(EventFileDef def, XElement el, bool parked)
    {
        var attrs = new Dictionary<string, string>();
        foreach (var a in el.Attributes()) attrs[a.Name.LocalName] = a.Value;

        var rec = new EventRecord
        {
            Id = attrs.TryGetValue(def.IdAttr, out var id) ? id : "",
            Name = def.NameAttr != null && attrs.TryGetValue(def.NameAttr, out var nm) ? nm : "",
            Attributes = attrs,
            Parked = parked,
            Comment = FindPrecedingComment(el)
        };

        switch (def.Toggle)
        {
            case ToggleStrategy.Flag:
                var fv = attrs.TryGetValue(def.FlagAttr ?? "", out var v) ? v.Trim() : "";
                rec.Enabled = fv == def.FlagOn;
                rec.Status = rec.Enabled ? "Bật" : "Tắt";
                break;
            case ToggleStrategy.DateWindow:
                var from = ParseDate(attrs.GetValueOrDefault(def.FromAttr));
                var to = ParseDate(attrs.GetValueOrDefault(def.ToAttr));
                var now = DateTime.Now;
                if (from == null && to == null) { rec.Enabled = false; rec.Status = "Chưa cấu hình"; }
                else if (to != null && to.Value.Year <= 2001) { rec.Enabled = false; rec.Status = "Tắt"; }
                else if (from != null && now < from) { rec.Enabled = true; rec.Status = $"Chưa bắt đầu ({from:dd/MM/yyyy})"; }
                else if (to != null && now > to) { rec.Enabled = false; rec.Status = $"Hết hạn ({to:dd/MM/yyyy})"; }
                else { rec.Enabled = true; rec.Status = "Đang chạy"; }
                break;
            case ToggleStrategy.None:
                rec.Enabled = true;
                rec.Status = "—";
                break;
            default: // Park
                rec.Enabled = !parked;
                rec.Status = parked ? "Tắt" : "Bật";
                break;
        }
        return rec;
    }

    private static string? FindPrecedingComment(XElement el)
    {
        var node = el.PreviousNode;
        while (node is XText) node = node.PreviousNode; // bỏ qua whitespace
        return node is XComment c ? c.Value.Trim() : null;
    }

    private static DateTime? ParseDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s) || s.Trim() == "-1") return null;
        if (DateTime.TryParseExact(s.Trim(), DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            return dt;
        if (DateTime.TryParse(s.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt)) return dt;
        return null;
    }

    public EventRecord? GetRecord(EventFileDef def, string id)
    {
        LoadRecords(def); // đảm bảo cache + chỉ mục ById đã sẵn sàng
        return _cache.TryGetValue(def.Key, out var e) && e.ById.TryGetValue(id, out var rec) ? rec : null;
    }

    // ---------- Sửa (giữ nguyên định dạng + comment) ----------

    public void UpdateRecord(EventFileDef def, string id, Dictionary<string, string> values)
    {
        var rec = GetRecord(def, id);
        var path = rec is { Parked: true } ? DisabledPath(def) : FullPath(def);
        var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var el = FindById(doc, def, id) ?? throw new InvalidOperationException($"Không tìm thấy bản ghi {id}");
        foreach (var kv in values) el.SetAttributeValue(kv.Key, kv.Value);
        SaveFaithful(def, doc, path);
    }

    // ---------- Thêm mới (định dạng lại sạch) ----------

    public void AddRecord(EventFileDef def, Dictionary<string, string> values)
    {
        var path = FullPath(def);
        var doc = File.Exists(path) ? XDocument.Load(path) : new XDocument();
        var root = doc.Root ?? throw new InvalidOperationException("File không có thẻ gốc");
        var el = new XElement(def.ItemElement);
        foreach (var kv in values)
            if (!string.IsNullOrEmpty(kv.Key)) el.SetAttributeValue(kv.Key, kv.Value);
        root.Add(el);
        SaveClean(def, doc, path);
    }

    // ---------- Xóa (giữ nguyên định dạng, gỡ cả comment kèm theo) ----------

    public void DeleteRecord(EventFileDef def, string id)
    {
        var rec = GetRecord(def, id);
        var path = rec is { Parked: true } ? DisabledPath(def) : FullPath(def);
        var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var el = FindById(doc, def, id);
        if (el == null) return;

        // gỡ comment + whitespace đứng ngay trước (nếu có)
        var prevWs = el.PreviousNode as XText;
        var beforeWs = prevWs?.PreviousNode ?? el.PreviousNode;
        if (beforeWs is XComment)
        {
            prevWs?.Remove();
            beforeWs.Remove();
        }
        el.Remove();
        SaveFaithful(def, doc, path);
    }

    // ---------- Bật / Tắt ----------

    public void Toggle(EventFileDef def, string id, bool enable)
    {
        switch (def.Toggle)
        {
            case ToggleStrategy.Flag: ToggleFlag(def, id, enable); break;
            case ToggleStrategy.DateWindow: ToggleDateWindow(def, id, enable); break;
            case ToggleStrategy.Park: TogglePark(def, id, enable); break;
        }
    }

    private void ToggleFlag(EventFileDef def, string id, bool enable)
    {
        var path = FullPath(def);
        var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var el = FindById(doc, def, id) ?? throw new InvalidOperationException($"Không tìm thấy {id}");
        el.SetAttributeValue(def.FlagAttr, enable ? def.FlagOn : def.FlagOff);
        SaveFaithful(def, doc, path);
    }

    private void ToggleDateWindow(EventFileDef def, string id, bool enable)
    {
        var path = FullPath(def);
        var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var el = FindById(doc, def, id) ?? throw new InvalidOperationException($"Không tìm thấy {id}");
        var store = LoadWindowState(def);

        if (!enable)
        {
            store[id] = new[] { el.Attribute(def.FromAttr)?.Value ?? "", el.Attribute(def.ToAttr)?.Value ?? "" };
            el.SetAttributeValue(def.FromAttr, OffSentinel);
            el.SetAttributeValue(def.ToAttr, OffSentinel);
        }
        else
        {
            if (store.TryGetValue(id, out var prev) && prev.Length == 2 &&
                ParseDate(prev[1]) is { } pto && pto.Year > 2001)
            {
                el.SetAttributeValue(def.FromAttr, prev[0]);
                el.SetAttributeValue(def.ToAttr, prev[1]);
            }
            else
            {
                var now = DateTime.Now;
                el.SetAttributeValue(def.FromAttr, now.ToString("yyyy-MM-dd 00:00:00"));
                el.SetAttributeValue(def.ToAttr, now.AddDays(7).ToString("yyyy-MM-dd 23:59:59"));
            }
            store.Remove(id);
        }
        SaveWindowState(def, store);
        SaveFaithful(def, doc, path);
    }

    private void TogglePark(EventFileDef def, string id, bool enable)
    {
        var livePath = FullPath(def);
        var disabledPath = DisabledPath(def);
        var liveDoc = XDocument.Load(livePath);
        var disDoc = File.Exists(disabledPath)
            ? XDocument.Load(disabledPath)
            : new XDocument(new XElement(liveDoc.Root!.Name));

        if (enable)
        {
            var el = FindById(disDoc, def, id);
            if (el != null) { el.Remove(); liveDoc.Root!.Add(el); }
        }
        else
        {
            var el = FindById(liveDoc, def, id);
            if (el != null) { el.Remove(); disDoc.Root!.Add(el); }
        }
        BackupOnce(def, livePath);
        WriteDoc(liveDoc, livePath, indent: true);
        WriteDoc(disDoc, disabledPath, indent: true);
    }

    // ---------- Helpers ----------

    private static XElement? FindById(XDocument doc, EventFileDef def, string id) =>
        doc.Descendants(def.ItemElement).FirstOrDefault(e => (e.Attribute(def.IdAttr)?.Value ?? "") == id);

    private Dictionary<string, string[]> LoadWindowState(EventFileDef def)
    {
        var p = WindowStatePath(def);
        if (!File.Exists(p)) return new();
        try { return JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText(p)) ?? new(); }
        catch { return new(); }
    }
    private void SaveWindowState(EventFileDef def, Dictionary<string, string[]> s) =>
        File.WriteAllText(WindowStatePath(def), JsonSerializer.Serialize(s, new JsonSerializerOptions { WriteIndented = true }));

    private void SaveFaithful(EventFileDef def, XDocument doc, string path)
    {
        BackupOnce(def, path);
        WriteDoc(doc, path, indent: false); // giữ nguyên whitespace/tab/comment gốc, chỉ đổi thuộc tính đã sửa
    }
    private void SaveClean(EventFileDef def, XDocument doc, string path)
    {
        BackupOnce(def, path);
        WriteDoc(doc, path, indent: true);
    }

    private void BackupOnce(EventFileDef def, string path)
    {
        if (!File.Exists(path)) return;
        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        File.Copy(path, Path.Combine(BackupDir, $"{def.Key}__{Path.GetFileName(path)}__{stamp}.bak"), overwrite: true);
    }

    private void WriteDoc(XDocument doc, string path, bool indent)
    {
        doc.Declaration ??= new XDeclaration("1.0", "utf-8", null);
        var settings = new XmlWriterSettings
        {
            Indent = indent,
            IndentChars = "  ",
            Encoding = new UTF8Encoding(true), // UTF-8 BOM như file gốc
            OmitXmlDeclaration = false
        };
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        using (var w = XmlWriter.Create(path, settings))
            doc.Save(w);
        _cache.Clear(); // lam moi cache sau moi lan ghi (ngoai co che invalidate theo mtime)
    }

    /// <summary>Tạo backup thủ công cho file gốc (không sửa nội dung).</summary>
    public void TriggerBackup(EventFileDef def)
    {
        var path = FullPath(def);
        if (!File.Exists(path)) throw new FileNotFoundException($"Không tìm thấy file: {path}");
        BackupOnce(def, path);
    }

    public List<string> ListBackups()
    {
        if (!Directory.Exists(BackupDir)) return new();
        return Directory.GetFiles(BackupDir).Select(Path.GetFileName).OrderByDescending(x => x).Take(50).ToList()!;
    }
}

