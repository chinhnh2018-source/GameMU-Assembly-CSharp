using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

/// <summary>
/// Danh muc tat ca file XML su kien duoc quan ly, phan tich tu GameRes/Config.
/// </summary>
public static class EventRegistry
{
    public static readonly List<EventFileDef> Files = new()
    {
        // ===== Lich su kien =====
        new() {
            Key="event-calendar", RelativePath="EventCalendar.xml", DisplayName="Lich su kien (Event Calendar)",
            Category="Lich su kien", ItemElement="EventCalendar", IdAttr="ID", NameAttr="EventName",
            Description="Cac su kien dinh ky theo thu/khung gio (Blood Castle, Devil Square...). Tat = go khoi lich (bao luu de bat lai).",
            ListColumns=new[]{"ID","EventName","Weekday","Level","LinkID","EventAward"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Hoat dong dac biet =====
        new() {
            Key="special-activity", RelativePath="SpecialActivity/SpecialActivity.xml", DisplayName="Hoat dong dac biet - Goi",
            Category="Hoat dong dac biet", ItemElement="Activity", IdAttr="ID", NameAttr="Name",
            Description="Cac goi/uu dai dac biet, gom theo GroupID. Thoi gian chay nam o file 'Khung thoi gian'.",
            ListColumns=new[]{"ID","GroupID","Name","Type","Price","PurchaseNum"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="special-activity-time", RelativePath="SpecialActivity/SpecialActivityTime.xml", DisplayName="Hoat dong dac biet - Khung thoi gian",
            Category="Hoat dong dac biet", ItemElement="Time", IdAttr="GroupID", NameAttr=null,
            Description="Khung thoi gian chay cho tung GroupID. Bat/tat = mo/dong khung ngay.",
            ListColumns=new[]{"GroupID","FromDate","ToDate","ServerOpenFromDate","ServerOpenToDate"},
            Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate"
        },

        // ===== Hoat dong hang ngay =====
        new() {
            Key="everyday-activity", RelativePath="EveryDayActivity/EveryDayActivity.xml", DisplayName="Hoat dong hang ngay",
            Category="Hoat dong hang ngay", ItemElement="EveryDayActivity", IdAttr="ActivityID", NameAttr="Name",
            Description="Phan thuong/muc tieu hang ngay theo GoalType.",
            ListColumns=new[]{"ActivityID","Name","GoalType","GoalNum","Price","PurchaseNum"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="everyday-activity-group", RelativePath="EveryDayActivity/EveryDayActivityGroup.xml", DisplayName="Hoat dong hang ngay - Nhom",
            Category="Hoat dong hang ngay", ItemElement="EveryDayActivityGroup", IdAttr="GroupID", NameAttr="Name",
            ListColumns=new[]{"GroupID","TypeID","Name","NeedType","NeedNum","ActivityID"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="everyday-activity-type", RelativePath="EveryDayActivity/EveryDayActivityType.xml", DisplayName="Hoat dong hang ngay - Loai",
            Category="Hoat dong hang ngay", ItemElement="EveryDayActivityType", IdAttr="TypeID", NameAttr="Name",
            ListColumns=new[]{"TypeID","Name","OpenLevel","CloseLevel"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Mo he thong =====
        new() {
            Key="version-system-open", RelativePath="VersionSystemOpen.xml", DisplayName="Mo he thong theo phien ban (IsOpen)",
            Category="Mo he thong", ItemElement="Version", IdAttr="ID", NameAttr="SystemName",
            Description="Bat/tat tung he thong bang co IsOpen (1=mo, 0=dong).",
            ListColumns=new[]{"ID","SystemName","IsOpen"},
            Toggle=ToggleStrategy.Flag, FlagAttr="IsOpen", FlagOn="1", FlagOff="0"
        },
        new() {
            Key="system-open", RelativePath="SystemOpen.xml", DisplayName="Mo he thong (dieu kien/thoi gian)",
            Category="Mo he thong", ItemElement="System", IdAttr="ID", NameAttr="Name",
            Description="Cau hinh mo tung chuc nang theo dieu kien kich hoat.",
            ListColumns=new[]{"Order","ID","Name","TriggerCondition","TimeParameters","NotOpenShow"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Su kien chu de (Theme) =====
        new() {
            Key="theme-open", RelativePath="ThemeActivityOpen.xml", DisplayName="Su kien chu de - Bat/Tat tong (Open)",
            Category="Su kien chu de", ItemElement="ThemeActivityOpen", IdAttr="ID", NameAttr="Title",
            Description="Cong tac tong cho su kien chu de (Open=1/0).",
            ListColumns=new[]{"ID","Title","Open","Logo"},
            Toggle=ToggleStrategy.Flag, FlagAttr="Open", FlagOn="1", FlagOff="0"
        },
        new() {
            Key="theme-type", RelativePath="ThemeActivityType.xml", DisplayName="Su kien chu de - Loai",
            Category="Su kien chu de", ItemElement="ThemeActivityType", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Type","Name","EndData"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="theme-zhigou", RelativePath="ThemeActivityZhiGou.xml", DisplayName="Su kien chu de - Goi nap (ZhiGou)",
            Category="Su kien chu de", ItemElement="ThemeActivityZhiGou", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","Day","ZhiGouID","ChongZhiID","SinglePurchase"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="theme-boss", RelativePath="ThemeActivityBOSS.xml", DisplayName="Su kien chu de - BOSS",
            Category="Su kien chu de", ItemElement="ThemeActivityBOSS", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","MonstersID","MapCode","Num","TimePoints"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="theme-zhuansheng", RelativePath="ThemeActivityZhuanSheng.xml", DisplayName="Su kien chu de - Chuyen sinh",
            Category="Su kien chu de", ItemElement="ThemeActivityZhuanSheng", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","MonstersID","MapID","MinLevel","MaxLevel","TimePoints"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Hoat dong hoi quy =====
        new() {
            Key="huigui-huodong", RelativePath="HuiGuiHuoDong.xml", DisplayName="Hoat dong hoi quy (HuiGui)",
            Category="Hoi quy / Gop server", ItemElement="HuiGuiHuoDong", IdAttr="ID", NameAttr=null,
            Description="Su kien hoi quy nguoi choi, dieu khien bang BeginTime/FinishTime.",
            ListColumns=new[]{"ID","HuoDongLevel","BeginTime","FinishTime","RegisterBegin","RegisterFinish"},
            Toggle=ToggleStrategy.DateWindow, FromAttr="BeginTime", ToAttr="FinishTime"
        },

        // ===== Su kien 7 ngay =====
        new() {
            Key="sevenday-goal", RelativePath="SevenDay/SevenDayGoal.xml", DisplayName="7 ngay - Muc tieu",
            Category="Su kien 7 ngay", ItemElement="Goal", IdAttr="ID", NameAttr="Describe",
            ListColumns=new[]{"ID","Day","GoalType","FunctionType","Describe","ShowNum"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="sevenday-type", RelativePath="SevenDay/SevenDayActivityType.xml", DisplayName="7 ngay - Loai hoat dong",
            Category="Su kien 7 ngay", ItemElement="ActivityType", IdAttr="ActivityType", NameAttr="Name",
            ListColumns=new[]{"ActivityType","Name","Tiptype","XML"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="sevenday-qianggou", RelativePath="SevenDay/SevenDayQiangGou.xml", DisplayName="7 ngay - Mua gioi han",
            Category="Su kien 7 ngay", ItemElement="Goods", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Day","Name","GoodsID","Price","Purchase"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Tab hoat dong =====
        new() {
            Key="huodong-tab", RelativePath="HuoDongTab.xml", DisplayName="Tab hoat dong (HuoDong)",
            Category="Tab hoat dong", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="kuafu-huodong-tab", RelativePath="KuaFuHuoDongTab.xml", DisplayName="Tab hoat dong lien server",
            Category="Tab hoat dong", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="zhandui-huodong-tab", RelativePath="ZhanDuiHuoDongTab.xml", DisplayName="Tab hoat dong doi (ZhanDui)",
            Category="Tab hoat dong", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","GLXml"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="zhanmeng-huodong-tab", RelativePath="ZhanMengHuoDongTab.xml", DisplayName="Tab hoat dong cong hoi",
            Category="Tab hoat dong", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Hoat dong khac (Activity folder) =====
        new() {
            Key="activity-copy", RelativePath="Activity/Copy.xml", DisplayName="Phu ban hoat dong (Copy)",
            Category="Hoat dong khac", ItemElement="Copy", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"Type","ID","Name","Level","MaxLevel"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="activity-boss", RelativePath="Activity/BossInfo.xml", DisplayName="BOSS hoat dong",
            Category="Hoat dong khac", ItemElement="Boss", IdAttr="ID", NameAttr="Description",
            ListColumns=new[]{"Type","ID","Level","Description","NpcID","Show"},
            Toggle=ToggleStrategy.Park
        },
        new() {
            Key="activity-tip", RelativePath="Activity/ActivityTip.xml", DisplayName="Goi y hoat dong (Tip)",
            Category="Hoat dong khac", ItemElement="Tip", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","MinLevel","WeekDays","StartDay","OpenDay"},
            Toggle=ToggleStrategy.Park
        },
    };

    // Cac file qua/le hoi & gop server theo khung ngay (FromDate/ToDate) - sinh tu danh sach.
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
        // Sinh dinh nghia cho qua le hoi (JieRiGifts/*) - khung ngay FromDate/ToDate.
        foreach (var f in FestivalFiles)
        {
            Files.Add(new EventFileDef
            {
                Key = "jieri-" + f.ToLowerInvariant(),
                RelativePath = $"JieRiGifts/{f}.xml",
                DisplayName = $"Qua le hoi - {f}",
                Category = "Qua le hoi (JieRi)",
                Description = "Su kien le hoi dieu khien bang FromDate/ToDate va AwardStartDate/AwardEndDate.",
                ItemElement = "Activities", IdAttr = "ActivityType", NameAttr = null,
                ListColumns = new[] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
                Toggle = ToggleStrategy.DateWindow, FromAttr = "FromDate", ToAttr = "ToDate"
            });
        }
        // Sinh dinh nghia cho gop server (HeFuGifts/*) co khung ngay.
        foreach (var f in HeFuFiles)
        {
            Files.Add(new EventFileDef
            {
                Key = "hefu-" + f.ToLowerInvariant(),
                RelativePath = $"HeFuGifts/{f}.xml",
                DisplayName = $"Gop server - {f}",
                Category = "Hoi quy / Gop server",
                Description = "Su kien gop server (HeFu). Mot so file co FromDate/ToDate de dieu khien thoi gian.",
                ItemElement = "Activities", IdAttr = "ActivityType", NameAttr = null,
                ListColumns = new[] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
                Toggle = ToggleStrategy.DateWindow, FromAttr = "FromDate", ToAttr = "ToDate"
            });
        }
    }

    public static EventFileDef? Get(string key) => Files.FirstOrDefault(f => f.Key == key);

    public static IEnumerable<IGrouping<string, EventFileDef>> ByCategory() =>
        Files.GroupBy(f => f.Category);

    /// <summary>Nhan tieng Viet cho cac thuoc tinh pho bien (hien thi help).</summary>
    public static readonly Dictionary<string, string> AttrLabels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ID"] = "Ma", ["GroupID"] = "Ma nhom", ["ActivityID"] = "Ma hoat dong", ["TypeID"] = "Ma loai",
        ["Name"] = "Ten", ["EventName"] = "Ten su kien", ["SystemName"] = "Ten he thong", ["Title"] = "Tieu de",
        ["Weekday"] = "Thu trong tuan (1-7)", ["Level"] = "Khoang cap (min,max)", ["Time"] = "Khung gio chay",
        ["LinkID"] = "ID lien ket choi", ["EventAward"] = "Phan thuong (id)", ["CompletedTaskID"] = "Task can hoan thanh",
        ["VipLevel"] = "Cap VIP yeu cau", ["FromDate"] = "Bat dau", ["ToDate"] = "Ket thuc",
        ["BeginTime"] = "Bat dau", ["FinishTime"] = "Ket thuc", ["RegisterBegin"] = "Mo dang ky", ["RegisterFinish"] = "Dong dang ky",
        ["ServerOpenFromDate"] = "Tinh tu ngay mo server (tu)", ["ServerOpenToDate"] = "Tinh tu ngay mo server (den)",
        ["AwardStartDate"] = "Nhan thuong tu", ["AwardEndDate"] = "Nhan thuong den",
        ["IsOpen"] = "Mo? (1/0)", ["Open"] = "Mo? (1/0)", ["GoalType"] = "Loai muc tieu", ["GoalNum"] = "Yeu cau muc tieu",
        ["GoodsOne"] = "Vat pham 1 (id,sl,binding,...)", ["GoodsTwo"] = "Vat pham 2", ["GoodsThr"] = "Vat pham 3",
        ["Price"] = "Gia (loai|gia|zhiGouID)", ["PurchaseNum"] = "So lan mua (-1=vo han)",
        ["Day"] = "Ngay ap dung", ["Type"] = "Loai", ["TriggerCondition"] = "Dieu kien kich hoat",
        ["TimeParameters"] = "Tham so thoi gian", ["NotOpenShow"] = "Hien khi chua mo", ["Order"] = "Thu tu",
        ["TimePoints"] = "Cac moc thoi gian", ["MonstersID"] = "ID quai", ["MapCode"] = "Ma ban do", ["MapID"] = "Ma ban do",
        ["NeedLevel"] = "Cap yeu cau", ["NeedVIP"] = "VIP yeu cau", ["NeedChongZhi"] = "Nap yeu cau",
    };
}
