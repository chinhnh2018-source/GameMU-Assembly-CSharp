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
            ListColumns=new[]{"ID","MonstersID","MapCode","GoodsList","TimePoints"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                // Da doi soat: 100% gia tri (90021,90024,9999001...) la Item.ID trong Goods.xml.
                new() { Field="GoodsList", TargetKey="", Description="Vật phẩm rơi (id đầu mỗi cụm '|') -> Goods.xml", ParseRewardList=true }
            }
        },
        new() {
            Key="theme-zhuansheng", RelativePath="ThemeActivityZhuanSheng.xml", DisplayName="Sự kiện chủ đề - Chuyển sinh",
            Category="Sự kiện chủ đề", ItemElement="ThemeActivityZhuanSheng", IdAttr="ID", NameAttr=null,
            ListColumns=new[]{"ID","MonstersID","MapID","MinLevel","MaxLevel","TimePoints"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                // Da doi soat: 100% khop Goods.xml.
                new() { Field="GoodsList", TargetKey="", Description="Vật phẩm thưởng (id đầu mỗi cụm '|') -> Goods.xml", ParseRewardList=true }
            }
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
            ListColumns=new[]{"Type","ID","Level","NpcID","GoodsList","Show"},
            Toggle=ToggleStrategy.Park,
            ForeignKeys=new() {
                // Da doi soat: 100% gia tri (9999001,1005091...) la Item.ID trong Goods.xml.
                new() { Field="GoodsList", TargetKey="", Description="Vật phẩm rơi (id mỗi cụm '|') -> Goods.xml", ParseRewardList=true }
            }
        },
        new() {
            Key="activity-tip", RelativePath="Activity/ActivityTip.xml", DisplayName="Gợi ý hoạt động (Tip)",
            Category="Hoạt động khác", ItemElement="Tip", IdAttr="ID", NameAttr="Name",
            ListColumns=new[]{"ID","Name","MinLevel","WeekDays","StartDay","OpenDay"},
            Toggle=ToggleStrategy.Park
        },

        // ===== Ánh xạ tiền tệ / vật phẩm =====
        new() {
            Key="get-goods", RelativePath="GetGoods.xml", DisplayName="Ánh xạ loại tiền tệ -> Vật phẩm (GetGoods)",
            Category="Ánh xạ vật phẩm", ItemElement="GetGoods", IdAttr="ID", NameAttr="Name",
            Description="Bảng ánh xạ loại tiền tệ/điểm (Coin, Kim cương, Ma tinh...) sang Item.ID trong Goods.xml. Đã đối soát 100% khớp Goods.xml.",
            ListColumns=new[]{"ID","Name","Goods"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="Goods", TargetKey="", Description="Item.ID trong Goods.xml (vật phẩm đại diện loại tiền tệ)", MultiValue=false }
            }
        },

        // ===== TỪ ĐIỂN ID GỐC (master): goods, monster, magic, map, npc, dungeon, task, xilian =====
        // ===== Vật phẩm (master lớn nhất) =====
        // LƯU Ý: Goods.xml ~15MB / 9.720 Item. XmlEventService.LoadRecords đã có cache theo mtime
        // + chỉ mục ById nên back-reference/forward-link không parse lại 15MB mỗi lần quét.
        new() {
            Key="goods", RelativePath="Goods.xml", DisplayName="Vật phẩm (Goods)",
            Category="Từ điển gốc", ItemElement="Item", IdAttr="ID", NameAttr="Title",
            Description="Từ điển ~9.720 vật phẩm — master được hầu hết file Config tham chiếu (GoodsList, Award, NeedGoods...).",
            ListColumns=new[]{"ID","Title","Categoriy","PriceOne","JinJie","XiLian"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="JinJie", TargetKey="goods",  TargetIdAttr="ID", Description="Thăng cấp/tiến giai -> Item.ID kế tiếp (self-ref). Khớp 100% (453/453).", MultiValue=false },
                new() { Field="XiLian", TargetKey="xilian", TargetIdAttr="ID", Description="Trỏ bảng tẩy luyện XiLianShuXing.xml. Khớp 100% (261/261). LƯU Ý: KHÔNG trỏ Goods (chỉ 3%).", MultiValue=false }
            }
        },

        // ===== Quái / Boss =====
        new() {
            Key="monster", RelativePath="Monsters.xml", DisplayName="Quái & Boss (Monsters)",
            Category="Từ điển gốc", ItemElement="Monster", IdAttr="ID", NameAttr="SName",
            Description="Từ điển ~2.064 quái/boss. Tham chiếu kỹ năng (SkillIDs->Magic), bản đồ (MapCode->Map), bảng rơi đồ (FallID).",
            ListColumns=new[]{"ID","SName","Level","MapCode","FallID","SkillIDs"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="SkillIDs", TargetKey="magic",   TargetIdAttr="ID",   Description="Kỹ năng của quái (->Magics.xml). Khớp 100% (187/187).", MultiValue=true, MultiSeparator=',' },
                new() { Field="MapCode",  TargetKey="map",     TargetIdAttr="Code", Description="Bản đồ quái xuất hiện (->MapConfig.xml). Khớp 99% (379/382, miss là map sự kiện).", MultiValue=true, MultiSeparator=',' },
                new() { Field="FallID",   TargetKey="monster", TargetIdAttr="ID",   Description="Chuỗi rơi đồ: trỏ tới một Monster khác (self-ref). Khớp 92% (392/427, miss là boss đặc biệt/liên server).", MultiValue=true, MultiSeparator=',' }
            }
        },

        // ===== Kỹ năng =====
        new() {
            Key="magic", RelativePath="Magics.xml", DisplayName="Kỹ năng (Magics)",
            Category="Từ điển gốc", ItemElement="Magic", IdAttr="ID", NameAttr="Name",
            Description="Từ điển ~448 kỹ năng. Được Monster.SkillIDs, vật phẩm học kỹ năng... tham chiếu.",
            ListColumns=new[]{"ID","Name","SkillType","ToOcuupation","MagicType"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="NextMagicID", TargetKey="magic", TargetIdAttr="ID", Description="Kỹ năng cấp kế tiếp (tự trỏ trong Magics.xml).", MultiValue=false }
            }
        },

        // ===== Bản đồ =====
        new() {
            Key="map", RelativePath="MapConfig.xml", DisplayName="Bản đồ (MapConfig)",
            Category="Từ điển gốc", ItemElement="Map", IdAttr="Code", NameAttr=null,
            Description="Từ điển bản đồ (khóa = Code). Được Monster.MapCode, NPC.MapCode, FuBen.MapCode tham chiếu.",
            ListColumns=new[]{"Code","BirthPosX","BirthPosY","BirthRadius"},
            Toggle=ToggleStrategy.None
        },

        // ===== NPC =====
        new() {
            Key="npc", RelativePath="npcs.xml", DisplayName="NPC",
            Category="Từ điển gốc", ItemElement="NPC", IdAttr="ID", NameAttr="SName",
            Description="Từ điển ~365 NPC. Tham chiếu bản đồ (MapCode->Map) và nhiệm vụ (Tasks->Task). Lưu ý: SaleID là ID cấu hình cửa hàng, KHÔNG phải Goods.ID.",
            ListColumns=new[]{"ID","SName","Function","MapCode","Tasks","SaleID"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="MapCode", TargetKey="map",  TargetIdAttr="Code", Description="Bản đồ NPC đứng (->MapConfig.xml). Khớp 98% (43/44).", MultiValue=true, MultiSeparator=',' },
                new() { Field="Tasks",   TargetKey="task", TargetIdAttr="ID",   Description="Nhiệm vụ gắn với NPC (->EraTask/LegionTasks). Khớp 100% (4/4).", MultiValue=true, MultiSeparator=',' }
                // KHÔNG map SaleID -> goods: đối soát cho 0% khớp (SaleID là ID bảng bán hàng riêng).
            }
        },

        // ===== Phụ bản (Dungeon) =====
        new() {
            Key="dungeon", RelativePath="FuBen.xml", DisplayName="Phụ bản (FuBen)",
            Category="Từ điển gốc", ItemElement="Copy", IdAttr="ID", NameAttr="CopyName",
            Description="Từ điển ~290 phụ bản. Tham chiếu bản đồ (MapCode->Map), boss (BossID->Monster), phần thưởng (RewardGoods->Goods).",
            ListColumns=new[]{"ID","CopyName","MapCode","BossID","MinLevel","RewardGoods"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="MapCode",     TargetKey="map",     TargetIdAttr="Code", Description="Bản đồ phụ bản (->MapConfig.xml). Khớp 100% (290/290).", MultiValue=false },
                new() { Field="BossID",      TargetKey="monster", TargetIdAttr="ID",   Description="Boss của phụ bản (->Monsters.xml). Khớp 98% (59/60, miss là boss liên server).", MultiValue=false },
                new() { Field="RewardGoods", TargetKey="",        Description="Vật phẩm thưởng (->Goods.xml), dạng 'id|id|id'. Khớp 100% (29/29).", MultiValue=true, MultiSeparator='|' }
            }
        },

        // ===== Nhiệm vụ =====
        new() {
            Key="task", RelativePath="EraTask.xml", DisplayName="Nhiệm vụ - Kỷ nguyên (EraTask)",
            Category="Từ điển gốc", ItemElement="EraTask", IdAttr="ID", NameAttr="TaskName",
            Description="Nhiệm vụ theo kỷ nguyên. Được NPC.Tasks tham chiếu.",
            ListColumns=new[]{"ID","EraID","EraStage","TaskName","Description"},
            Toggle=ToggleStrategy.None
        },
        new() {
            Key="task-legion", RelativePath="LegionTasks.xml", DisplayName="Nhiệm vụ - Liên minh (LegionTasks)",
            Category="Từ điển gốc", ItemElement="LegionTasks", IdAttr="ID", NameAttr="Name",
            Description="Nhiệm vụ liên minh/quân đoàn (13 mục). Phần thưởng/kiểu hoàn thành theo TypeID.",
            ListColumns=new[]{"ID","Name","CompleteType","Exp","Describtion"},
            Toggle=ToggleStrategy.None
        },

        // ===== Tẩy luyện (XiLian) — master mới, được Goods.XiLian trỏ tới =====
        new() {
            Key="xilian", RelativePath="XiLianShuXing.xml", DisplayName="Thuộc tính tẩy luyện (XiLianShuXing)",
            Category="Từ điển gốc", ItemElement="XiLian", IdAttr="ID", NameAttr="Name",
            Description="Bảng thuộc tính tẩy luyện cho trang bị. Được Goods.XiLian trỏ tới (khớp 100% - 261/261). NeedGoods->Goods.ID.",
            ListColumns=new[]{"ID","Name","NeedGoods","NeedJinBi","NeedZuanShi"},
            Toggle=ToggleStrategy.None,
            ForeignKeys=new() {
                new() { Field="NeedGoods", TargetKey="", Description="Vật phẩm cần để tẩy luyện (->Goods.xml), dạng 'id,sl'.", ParseRewardList=true }
            }
        },

        // =================================================================================
        //  GHI CHÚ: def "goods" (master vật phẩm) đã được đăng ký ở ĐẦU block này, kèm 2 FK
        //  tự-trỏ JinJie (->goods) và XiLian (->xilian). Khi dán, đảm bảo def "goods" và
        //  "xilian" đều có mặt để forward-link/back-reference resolve đúng.
        // =================================================================================
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
