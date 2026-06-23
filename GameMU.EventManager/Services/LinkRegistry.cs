namespace GameMU.EventManager.Services;

/// <summary>
/// Một quan hệ tham chiếu giữa các file XML (kiểu "khóa ngoại").
/// </summary>
public class XmlLink
{
    public string SourceFile { get; init; } = "";   // file chứa trường tham chiếu
    public string SourceField { get; init; } = "";  // tên trường tham chiếu
    public string TargetFile { get; init; } = "";   // file đích
    public string TargetKey { get; init; } = "";    // khóa ở file đích
    public string Description { get; init; } = "";
    public bool LiveChecked { get; init; }          // web có đối soát trực tiếp hay không
}

/// <summary>
/// Khai báo các mối liên kết giữa file XML của GameServer — để web "thể hiện" quan hệ rõ ràng.
/// </summary>
public static class LinkRegistry
{
    public static readonly List<XmlLink> Links = new()
    {
        new() {
            SourceFile="SpecialActivity/SpecialActivity.xml", SourceField="GroupID",
            TargetFile="SpecialActivity/SpecialActivityTime.xml", TargetKey="GroupID",
            Description="Mỗi đợt hoạt động đặc biệt = nhiều <Activity> cùng GroupID, lịch chạy (FromDate/ToDate) nằm ở 1 <Time> cùng GroupID.",
            LiveChecked=true
        },
        new() {
            SourceFile="SpecialActivity/SpecialActivity.xml", SourceField="GoodsOne / GoodsTwo / GoodsThr",
            TargetFile="Goods.xml", TargetKey="Item.ID",
            Description="Phần thưởng/vật phẩm bán: số đầu mỗi cụm 'id,sl,...' phải là Item.ID hợp lệ trong Goods.xml.",
            LiveChecked=true
        },
        new() {
            SourceFile="EventCalendar.xml", SourceField="LinkID",
            TargetFile="(Map / Dungeon)", TargetKey="ID",
            Description="Trỏ tới phụ bản/bản đồ tương ứng của sự kiện định kỳ (Blood Castle, Devil Square...).",
            LiveChecked=false
        },
        new() {
            SourceFile="EventCalendar.xml", SourceField="EventAward",
            TargetFile="Goods.xml", TargetKey="Item.ID",
            Description="Danh sách phần thưởng của sự kiện định kỳ tham chiếu mã vật phẩm.",
            LiveChecked=false
        },
        new() {
            SourceFile="JieRiGifts/*.xml", SourceField="Goods* / Award",
            TargetFile="Goods.xml", TargetKey="Item.ID",
            Description="Quà lễ hội (tích nạp/tiêu, rương, hồng bao...) tham chiếu mã vật phẩm trong Goods.xml.",
            LiveChecked=false
        },
        new() {
            SourceFile="Mall.xml / QiangGou.xml", SourceField="GoodsID",
            TargetFile="Goods.xml", TargetKey="Item.ID",
            Description="Vật phẩm bán trong cửa hàng / tranh mua phải tồn tại trong Goods.xml.",
            LiveChecked=false
        },
    };
}
