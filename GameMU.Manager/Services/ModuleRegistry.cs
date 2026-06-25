namespace GameMU.EventManager.Services;

/// <summary>
/// Một "chức năng lớn" (module nghiệp vụ) gom nhiều category/file XML lại theo mục đích vận hành.
/// </summary>
public class EventModule
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public string Icon { get; init; } = "";
    public string Description { get; init; } = "";
    public string? Page { get; init; }
    public string[] Categories { get; init; } = Array.Empty<string>();
    public bool IsContent => Categories.Length > 0;
}

/// <summary>
/// Danh mục 19 modules của Web App — dùng cho sidebar và trang chủ.
/// </summary>
public static class ModuleRegistry
{
    public static readonly List<EventModule> Modules = new()
    {
        new() { Id="overview",    Name="Tổng quan",           Icon="speedometer2",    Page="/Index",
            Description="Trạng thái mọi cấu hình, số mục mỗi file" },

        new() { Id="calendar",    Name="Lịch sự kiện",        Icon="calendar-range",  Page="/Calendar",
            Description="Timeline trực quan, phát hiện trùng giờ" },

        new() { Id="events",      Name="Sự kiện & Hoạt động", Icon="calendar2-week",
            Description="Blood Castle, Devil Square, hằng ngày, tab hoạt động",
            Categories=new[]{ "Lịch sự kiện", "Hoạt động đặc biệt",
                              "Hoạt động hằng ngày", "Tab hoạt động", "Hoạt động khác" } },

        new() { Id="festival",    Name="Lễ hội & Chủ đề",     Icon="gift",
            Description="Sự kiện chủ đề (Theme), sự kiện 7 ngày đầu, hồi quy / gộp server",
            Categories=new[]{ "Sự kiện chủ đề", "Sự kiện 7 ngày", "Hồi quy / Gộp server" } },

        new() { Id="jieri",       Name="Quà lễ hội (JieRi)",  Icon="balloon-heart",
            Description="Tất cả quà lễ hội: đăng nhập, tích lũy nạp, vua nạp, boss... (44 files)",
            Categories=new[]{ "Quà lễ hội" } },

        new() { Id="hefu",        Name="Hợp phục / Gộp server",Icon="people",
            Description="Sự kiện khi gộp server: đăng nhập, nạp hoàn, shop, boss... (12 files)",
            Categories=new[]{ "Hợp phục / Gộp server" } },

        new() { Id="richanggifts",Name="Quà hằng ngày",       Icon="calendar-check",
            Description="Quà nạp ngày, thưởng cấp độ, đào vàng, Thần Trang... (6 files)",
            Categories=new[]{ "Quà hằng ngày" } },

        new() { Id="arena",       Name="Đấu trường",           Icon="trophy",
            Description="Config Arena: thưởng theo rank, cooldown, quân hàm, 3-day award (5 files)",
            Categories=new[]{ "Đấu trường" } },

        new() { Id="dungeon",     Name="Đền & Dungeon",        Icon="building",
            Description="Đền Thiên Thần, Đền Mirage, Chén Thánh, Kill streak (9 files)",
            Categories=new[]{ "Đền & Dungeon" } },

        new() { Id="vip",         Name="VIP",                  Icon="star",
            Description="Config VIP, thưởng hằng ngày VIP, tab đặc quyền (3 files)",
            Categories=new[]{ "VIP" } },

        new() { Id="giftcode",    Name="Gift Code & Quà",      Icon="ticket-perforated",
            Description="Mã quà tặng GiftCodeNew.xml + thưởng điểm hoạt động (2 files)",
            Categories=new[]{ "Quà & Mã quà" } },

        new() { Id="shop",        Name="Shop & Mall",           Icon="shop",
            Description="Mall.xml, ZhanGong Mall, QiZhenGe — danh sách hàng hóa bán (2 files)",
            Categories=new[]{ "Shop & Mall" } },

        new() { Id="system",      Name="Bật/Tắt hệ thống",    Icon="toggles",
            Description="Mở/đóng nhanh các tính năng hệ thống (2 files)",
            Categories=new[]{ "Mở hệ thống" } },

        new() { Id="params",      Name="Tham số cân bằng",     Icon="sliders",
            Description="Từ điển ~939 tham số: cường hóa, truyền thừa, tỉ lệ... (1 file)",
            Categories=new[]{ "Tham số hệ thống" } },

        new() { Id="refdata",     Name="Từ điển gốc",          Icon="book",
            Description="Goods.xml, Monsters, Magic, Map, NPC, FuBen, Tasks... (9 files)",
            Categories=new[]{ "Từ điển gốc", "Ánh xạ vật phẩm" } },

        new() { Id="features",    Name="Bản đồ tính năng → XML",Icon="diagram-3",     Page="/Features",
            Description="186 lớp GameServer ↔ file XML" },

        new() { Id="links",       Name="Liên kết XML",          Icon="share",          Page="/Links",
            Description="Quan hệ giữa các file + đối soát GroupID, GoodsID" },



        new() { Id="goods-audit", Name="Đối soát mã vật phẩm", Icon="search",         Page="/GoodsAudit",
            Description="Quét toàn bộ Config, đối soát mọi tham chiếu với Goods.xml" },

        new() { Id="backups",     Name="Sao lưu & Khôi phục",  Icon="clock-history",  Page="/Backups",
            Description="Export ZIP, Import ZIP, lịch sử backup tự động" },
    };

    public static EventModule? Get(string? id) => Modules.FirstOrDefault(m => m.Id == id);
    public static EventModule? ForCategory(string category) =>
        Modules.FirstOrDefault(m => m.Categories.Contains(category));
}
