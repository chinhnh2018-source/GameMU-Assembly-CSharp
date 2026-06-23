using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

/// <summary>
/// Danh mục tất cả file XML sự kiện được quản lý, phân tích từ GameRes/Config.
/// </summary>
public static class EventRegistry
{
    public static readonly List<EventFileDef> Files = new()
    {
        // ===== Tham số hệ thống (từ điển lớn) =====
        new() {
            Key="system-params", RelativePath="SystemParams.xml", DisplayName="Tham số hệ thống (SystemParams)",
            Category="Tham số hệ thống", ItemElement="Param", IdAttr="Name", NameAttr="Name",
            Description="Từ điển ~939 tham số điều khiển rất nhiều chức năng (cường hóa, truy gia, truyền thừa, cánh, tỉ lệ...). Mỗi tham số kèm chú thích. Sửa giá trị trực tiếp; comment được giữ nguyên.",
            ListColumns=new[]{"Name","Value"},
            Toggle=ToggleStrategy.None
        },

        // ===== Lịch sự kiện =====
        new() {
            Key="event-calendar", RelativePath="EventCalendar.xml", DisplayName="Lịch sự kiện (Event Calendar)",
            Category="Lịch sự kiện", ItemElement="EventCalendar", IdAttr="ID", NameAttr="EventName",
            Description="Các sự kiện định kỳ theo thứ/khung giờ (Blood Castle, Devil Square...). Tắt = gỡ khỏi lịch (bảo lưu để bật lại).",
            ListColumns=new[]{"ID","EventName","Weekday","Level","LinkID","EventAward"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                // LinkID KHONG phai khoa ngoai: la ma dieu huong hard-code trong PlayZone.ProcessGuideRequest (switch).
                new() { Field="EventAward", TargetKey="", Description="Mã vật phẩm thưởng (Super.LoadGoodsList -> Goods.xml)", MultiValue=true, MultiSeparator=',' }
            }
        },

        // ===== Hoạt động đặc biệt =====
        new() {
            Key="special-activity", RelativePath="SpecialActivity/SpecialActivity.xml", DisplayName="Hoạt động đặc biệt - Gói",
            Category="Hoạt động đặc biệt", ItemElement="Activity", IdAttr="ID", NameAttr="Name",
            Description="Các gói/ưu đãi đặc biệt, gom theo GroupID. Thời gian chạy nằm ở file 'Khung thời gian'.",
            ListColumns=new[]{"ID","GroupID","Name","Type","Price","PurchaseNum"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                new() { Field="GroupID", TargetKey="special-activity-time", Description="GroupID ghép với khung thời gian FromDate/ToDate", MultiValue=false },
                new() { Field="GoodsOne", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
                new() { Field="GoodsTwo", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
                new() { Field="GoodsThr", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true }
            }
        },
        new() {
            Key="special-activity-time", RelativePath="SpecialActivity/SpecialActivityTime.xml", DisplayName="Hoạt động đặc biệt - Khung thời gian",
            Category="Hoạt động đặc biệt", ItemElement="Time", IdAttr="GroupID", NameAttr=null,
            Description="Khung thời gian chạy cho từng GroupID. Bật/tắt = mở/đóng khung ngày.",
            ListColumns=new[]{"GroupID","FromDate","ToDate","ServerOpenFromDate","ServerOpenToDate"},
            Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate"
        },

        // ===== Hoạt động hằng ngày =====
        new() {
            Key="everyday-activity", RelativePath="EveryDayActivity/EveryDayActivity.xml", DisplayName="Hoạt động hằng ngày",
            Category="Hoạt động hằng ngày", ItemElement="EveryDayActivity", IdAttr="ActivityID", NameAttr="Name",
            Description="Phần thưởng/mục tiêu hằng ngày theo GoalType.",
            ListColumns=new[]{"ActivityID","Name","GoalType","GoalNum","Price","PurchaseNum"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                // GoalType la enum hanh vi (config.Type, so sanh ==1/2/14), KHONG phai FK toi EveryDayActivityType.
                // Lien ket Type that su nam o EveryDayActivityGroup.TypeID.
                new() { Field="GoodsOne", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
                new() { Field="GoodsTwo", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
                new() { Field="GoodsThr", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true }
            }
        },
        new() {
            Key="everyday-activity-group", RelativePath="EveryDayActivity/EveryDayActivityGroup.xml", DisplayName="Hoạt động hằng ngày - Nhóm",
            Category="Hoạt động hằng ngày", ItemElement="EveryDayActivityGroup", IdAttr="GroupID", NameAttr="Name",
            ListColumns=new[]{"GroupID","TypeID","Name","NeedType","NeedNum","ActivityID"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                new() { Field="TypeID", TargetKey="everyday-activity-type", Description="Loại hoạt động hằng ngày", MultiValue=false },
                new() { Field="ActivityID", TargetKey="everyday-activity", Description="Danh sách ActivityID cách nhau bởi '|'", MultiValue=true, MultiSeparator='|' }
            }
        },
        new() {
            Key="everyday-activity-type", RelativePath="EveryDayActivity/EveryDayActivityType.xml", DisplayName="Hoạt động hằng ngày - Loại",
            Category="Hoạt động hằng ngày", ItemElement="EveryDayActivityType", IdAttr="TypeID", NameAttr="Name",
            ListColumns=new[]{"TypeID","Name","OpenLevel","CloseLevel"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Mở hệ thống =====
        new() {
            Key="version-system-open", RelativePath="VersionSystemOpen.xml", DisplayName="Mở hệ thống theo phiên bản (IsOpen)",
            Category="Mở hệ thống", ItemElement="Version", IdAttr="ID", NameAttr="SystemName",
            Description="Bật/tắt từng hệ thống bằng cờ IsOpen (1=mở, 0=đóng).",
            ListColumns=new[]{"ID","SystemName","IsOpen"},
            Toggle=ToggleStrategy.Flag, FlagAttr="IsOpen", FlagOn="1", FlagOff="0"
        },
        new() {
            Key="system-open", RelativePath="SystemOpen.xml", DisplayName="Mở hệ thống (điều kiện/thời gian)",
            Category="Mở hệ thống", ItemElement="System", IdAttr="ID", NameAttr="Name",
            Description="Cấu hình mở từng chức năng theo điều kiện kích hoạt.",
            ListColumns=new[]{"Order","ID","Name","TriggerCondition","TimeParameters","NotOpenShow"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Sự kiện chủ đề (Theme) =====
        new() {
            Key="theme-open", RelativePath="ThemeActivityOpen.xml", DisplayName="Sự kiện chủ đề - Bật/Tắt tổng (Open)",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityOpen", IdAttr="ID", NameAttr="Title",
            Description="Công tắc tổng cho sự kiện chủ đề (Open=1/0).",
            ListColumns=new[]{"ID","Title","Open","Logo"},
            Toggle=ToggleStrategy.Flag, FlagAttr="Open", FlagOn="1", FlagOff="0"
        },
        new() {
            Key="theme-type", RelativePath="ThemeActivityType.xml", DisplayName="Sự kiện chủ đề - Loại",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityType", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Type","Name","EndData"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="theme-zhigou", RelativePath="ThemeActivityZhiGou.xml", DisplayName="Sự kiện chủ đề - Gói nạp (ZhiGou)",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityZhiGou", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","Day","ZhiGouID","ChongZhiID","SinglePurchase"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                new() { Field="GoodsOne", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
                new() { Field="GoodsTwo", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true }
            }
        },
        new() {
            Key="theme-boss", RelativePath="ThemeActivityBOSS.xml", DisplayName="Sự kiện chủ đề - BOSS",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityBOSS", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","MonstersID","MapCode","Num","TimePoints"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="theme-zhuansheng", RelativePath="ThemeActivityZhuanSheng.xml", DisplayName="Sự kiện chủ đề - Chuyển sinh",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityZhuanSheng", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","MonstersID","MapID","MinLevel","MaxLevel","TimePoints"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Hoạt động hồi quy =====
        new() {
            Key="huigui-huodong", RelativePath="HuiGuiHuoDong.xml", DisplayName="Hoạt động hồi quy (HuiGui)",
            Category="Hồi quy / Gộp server", ItemElement="HuiGuiHuoDong", IdAttr="ID", NameAttr=null,
            Description="Sự kiện hồi quy người chơi, điều khiển bằng BeginTime/FinishTime.",
            ListColumns=new[]{"ID","HuoDongLevel","BeginTime","FinishTime","RegisterBegin","RegisterFinish"},
            Toggle=ToggleStrategy.DateWindow, FromAttr="BeginTime", ToAttr="FinishTime"
        },

        // ===== Sự kiện 7 ngày =====
        new() {
            Key="sevenday-goal", RelativePath="SevenDay/SevenDayGoal.xml", DisplayName="7 ngày - Mục tiêu",
            Category="Sự kiện 7 ngày", ItemElement="Goal", IdAttr="ID", NameAttr="Describe",
            ListColumns=new[]{"ID","Day","GoalType","FunctionType","Describe","ShowNum"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                new() { Field="Award", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm '|')", ParseRewardList=true }
            }
        },
        new() {
            Key="sevenday-type", RelativePath="SevenDay/SevenDayActivityType.xml", DisplayName="7 ngày - Loại hoạt động",
            Category="Sự kiện 7 ngày", ItemElement="ActivityType", IdAttr="ActivityType", NameAttr="Name",
            ListColumns=new[]{"ActivityType","Name","Tiptype","XML"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="sevenday-qianggou", RelativePath="SevenDay/SevenDayQiangGou.xml", DisplayName="7 ngày - Mua giới hạn",
            Category="Sự kiện 7 ngày", ItemElement="Goods", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Day","Name","GoodsID","Price","Purchase"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                new() { Field="GoodsID", TargetKey="", Description="ID vật phẩm trong Goods.xml", MultiValue=false }
            }
        },

        // ===== Tab hoạt động =====
        new() {
            Key="huodong-tab", RelativePath="HuoDongTab.xml", DisplayName="Tab hoạt động (HuoDong)",
            Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="kuafu-huodong-tab", RelativePath="KuaFuHuoDongTab.xml", DisplayName="Tab hoạt động liên server",
            Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="zhandui-huodong-tab", RelativePath="ZhanDuiHuoDongTab.xml", DisplayName="Tab hoạt động đội (ZhanDui)",
            Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="zhanmeng-huodong-tab", RelativePath="ZhanMengHuoDongTab.xml", DisplayName="Tab hoạt động công hội",
            Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Hoạt động khác (Activity folder) =====
        new() {
            Key="activity-copy", RelativePath="Activity/Copy.xml", DisplayName="Phụ bản hoạt động (Copy)",
            Category="Hoạt động khác", ItemElement="Copy", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"Type","ID","Name","Level","MaxLevel"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="activity-boss", RelativePath="Activity/BossInfo.xml", DisplayName="BOSS hoạt động",
            Category="Hoạt động khác", ItemElement="Boss", IdAttr="ID", NameAttr="Description",
            ListColumns=new[]{"Type","ID","Level","Description","NpcID","Show"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="activity-tip", RelativePath="Activity/ActivityTip.xml", DisplayName="Gợi ý hoạt động (Tip)",
            Category="Hoạt động khác", ItemElement="Tip", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","MinLevel","WeekDays","StartDay","OpenDay"},
            Toggle=ToggleStrategy.Park
        },
    };

    public static readonly string[] FestivalFiles = new[]
    {
        "JieRiBOSS","JieRiBaoXiang","JieRiChongZhiFanLi","JieRiChongZhiHongBao","JieRiChongZhiKing",
        "JieRiChongZhiQiangGou","JieRiChongZhiSong","JieRiDanBiChongZhi","JieRiDayChongZhi","JieRiDayXiaoFei",
        "JieRiDengLu","JieRiDuoBei","JieRiFuLi","JieRiFuWenFanLi","JieRiHongBaoBang","JieRiHuiJiFanLi",
        "JieRiLeiJi","JieRiLeiJiXiaoFei","JieRiLiBao","JieRiLianXu","JieRiMeiRiChongZhiWang","JieRiMeiRiLeiJi",
        "JieRiQuanMinHongBao","JieRiShouQu","JieRiShouQuKing","JieRiVip","JieRiXiaoFeiKing","JieRiZengSong","JieRiZengSongKing"
    };

    public static readonly string[] HeFuFiles = new[]
    {
        "HeFuBOSS","HeFuFanLi","HeFuLiBao","HeFuLuoLan","HeFuDengLu","HeFuZhangChang"
    };

    static EventRegistry()
    {
        foreach (var f in FestivalFiles)
        {
            Files.Add(new EventFileDef
            {
                Key = "jieri-" + f.ToLowerInvariant(),
                RelativePath = $"JieRiGifts/{f}.xml",
                DisplayName = $"Quà lễ hội - {f}",
                Category = "Quà lễ hội (JieRi)",
                Description = "Sự kiện lễ hội điều khiển bằng FromDate/ToDate và AwardStartDate/AwardEndDate.",
                ItemElement = "Activities", IdAttr = "ActivityType", NameAttr = null,
                ListColumns = new[] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
                Toggle = ToggleStrategy.DateWindow, FromAttr = "FromDate", ToAttr = "ToDate"
            });
        }
        foreach (var f in HeFuFiles)
        {
            Files.Add(new EventFileDef
            {
                Key = "hefu-" + f.ToLowerInvariant(),
                RelativePath = $"HeFuGifts/{f}.xml",
                DisplayName = $"Gộp server - {f}",
                Category = "Hồi quy / Gộp server",
                Description = "Sự kiện gộp server (HeFu). Một số file có FromDate/ToDate để điều khiển thời gian.",
                ItemElement = "Activities", IdAttr = "ActivityType", NameAttr = null,
                ListColumns = new[] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
                Toggle = ToggleStrategy.DateWindow, FromAttr = "FromDate", ToAttr = "ToDate"
            });
        }
    }

    public static EventFileDef? Get(string key) => Files.FirstOrDefault(f => f.Key == key);

    public static IEnumerable<IGrouping<string, EventFileDef>> ByCategory() => Files.GroupBy(f => f.Category);

    /// <summary>Nhãn tiếng Việt cho các thuộc tính phổ biến (hiển thị trợ giúp).</summary>
    public static readonly Dictionary<string, string> AttrLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ID"] = "Mã", ["GroupID"] = "Mã nhóm", ["ActivityID"] = "Mã hoạt động", ["TypeID"] = "Mã loại",
        ["Name"] = "Tên", ["EventName"] = "Tên sự kiện", ["SystemName"] = "Tên hệ thống", ["Title"] = "Tiêu đề",
        ["Value"] = "Giá trị (tham số)",
        ["Weekday"] = "Thứ trong tuần (1-7)", ["Level"] = "Khoảng cấp (min,max)", ["Time"] = "Khung giờ chạy",
        ["LinkID"] = "ID liên kết chơi", ["EventAward"] = "Phần thưởng (id)", ["CompletedTaskID"] = "Task cần hoàn thành",
        ["VipLevel"] = "Cấp VIP yêu cầu", ["FromDate"] = "Bắt đầu", ["ToDate"] = "Kết thúc",
        ["BeginTime"] = "Bắt đầu", ["FinishTime"] = "Kết thúc", ["RegisterBegin"] = "Mở đăng ký", ["RegisterFinish"] = "Đóng đăng ký",
        ["ServerOpenFromDate"] = "Tính từ ngày mở server (từ)", ["ServerOpenToDate"] = "Tính từ ngày mở server (đến)",
        ["AwardStartDate"] = "Nhận thưởng từ", ["AwardEndDate"] = "Nhận thưởng đến",
        ["IsOpen"] = "Mở? (1/0)", ["Open"] = "Mở? (1/0)", ["GoalType"] = "Loại mục tiêu", ["GoalNum"] = "Yêu cầu mục tiêu",
        ["GoodsOne"] = "Vật phẩm 1 (id,sl,binding,...)", ["GoodsTwo"] = "Vật phẩm 2", ["GoodsThr"] = "Vật phẩm 3",
        ["Price"] = "Giá (loại|giá|zhiGouID)", ["PurchaseNum"] = "Số lần mua (-1=vô hạn)",
        ["Day"] = "Ngày áp dụng", ["Type"] = "Loại", ["TriggerCondition"] = "Điều kiện kích hoạt",
        ["TimeParameters"] = "Tham số thời gian", ["NotOpenShow"] = "Hiện khi chưa mở", ["Order"] = "Thứ tự",
        ["TimePoints"] = "Các mốc thời gian", ["MonstersID"] = "ID quái", ["MapCode"] = "Mã bản đồ", ["MapID"] = "Mã bản đồ",
        ["NeedLevel"] = "Cấp yêu cầu", ["NeedVIP"] = "VIP yêu cầu", ["NeedChongZhi"] = "Nạp yêu cầu",
    };
}
