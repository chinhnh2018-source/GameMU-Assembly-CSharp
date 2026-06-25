namespace GameMU.EventManager.Services;

/// <summary>
/// Một "chức năng lớn" (module nghiệp vụ) gom nhiều category/file XML lại theo mục đích vận hành.
/// </summary>
public class EventModule
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "";              // tên bootstrap-icon (không kèm 'bi-')
    public string Description { get; init; } = "";
    public string? Page { get; init; }                   // nếu là trang riêng (vd "/Calendar"); null = trang chủ lọc theo module
    public string[] Categories { get; init; } = Array.Empty<string>();

    public bool IsContent => Categories.Length > 0;       // module gom file XML (hiển thị trên trang chủ)
}

/// <summary>
/// Danh mục các chức năng lớn của Web App — dùng cho sidebar và trang chủ.
/// </summary>
public static class ModuleRegistry
{
    public static readonly List<EventModule> Modules = new()
    {
        new() { Id="overview", Name="Tổng quan", Icon="speedometer2", Page="/Index",
                Description="Trạng thái mọi cấu hình, số mục mỗi file" },

        new() { Id="calendar", Name="Lịch sự kiện", Icon="calendar-range", Page="/Calendar",
                Description="Timeline trực quan, phát hiện trùng giờ" },

        new() { Id="events", Name="Sự kiện & Hoạt động", Icon="calendar2-week",
                Description="Blood Castle, Devil Square, hằng ngày, tab hoạt động",
                Categories=new[]{ "Lịch sự kiện", "Hoạt động đặc biệt", "Hoạt động hằng ngày", "Tab hoạt động", "Hoạt động khác" } },

        new() { Id="festival", Name="Lễ hội & Chủ đề", Icon="gift",
                Description="Sự kiện chủ đề, 7 ngày, hồi quy / gộp server",
                Categories=new[]{ "Sự kiện chủ đề", "Sự kiện 7 ngày", "Hồi quy / Gộp server" } },

        new() { Id="system", Name="Bật/Tắt hệ thống", Icon="toggles",
                Description="Mở/đóng nhanh các tính năng hệ thống",
                Categories=new[]{ "Mở hệ thống" } },

        new() { Id="params", Name="Tham số cân bằng", Icon="sliders",
                Description="Tỉ lệ, giới hạn, cường hóa, truyền thừa...",
                Categories=new[]{ "Tham số hệ thống" } },

        new() { Id="refdata", Name="Ánh xạ vật phẩm", Icon="box-seam",
                Description="Bảng ánh xạ tiền tệ/điểm -> vật phẩm trong Goods.xml",
                Categories=new[]{ "Ánh xạ vật phẩm" } },

        new() { Id="features", Name="Bản đồ tính năng → XML", Icon="diagram-3", Page="/Features",
                Description="186 lớp GameServer ↔ file XML" },

        new() { Id="links", Name="Liên kết XML", Icon="share", Page="/Links",
                Description="Quan hệ giữa các file + đối soát GroupID, GoodsID" },

        new() { Id="goods-audit", Name="Đối soát mã vật phẩm", Icon="box-seam", Page="/GoodsAudit",
                Description="Quét toàn bộ Config, đối soát mọi tham chiếu với Goods.xml" },

        new() { Id="backups", Name="Sao lưu & Khôi phục", Icon="clock-history", Page="/Backups",
                Description="Lịch sử bản sao lưu tự động" },
    };

    public static EventModule? Get(string? id) => Modules.FirstOrDefault(m => m.Id == id);

    /// <summary>Tìm module lớn chứa một category cụ thể (dùng để highlight sidebar khi đang ở trang File/Edit).</summary>
    public static EventModule? ForCategory(string category) =>
        Modules.FirstOrDefault(m => m.Categories.Contains(category));
}
