using GameMU.EventManager.Models;

namespace GameMU.EventManager.Services;

///
/// Danh mục tất cả file XML sự kiện được quản lý, phân tích từ GameRes/Config.
///
public static class EventRegistry
{
 public static readonly List Files = new()
 {
 // ===== Tham số hệ thống (từ điển lớn) =====
 new() {
 Key="system-params", RelativePath="SystemParams.xml", DisplayName="Tham số hệ thống (SystemParams)",
 Category="Tham số hệ thống", ItemElement="Param", IdAttr="Name", NameAttr="Name",
 Description="Từ điển ~939 tham số điều khiển rất nhiều chức năng (cường hóa, truy gia, truyền thừa, cánh, tỉ lệ...). Mỗi tham số kèm chú thích. Sửa giá trị trực tiếp; comment được giữ nguyên.",
 ListColumns=new\[\]{"Name","Value"},
 Toggle=ToggleStrategy.None
 },

 // ===== Lịch sự kiện =====
 new() {
 Key="event-calendar", RelativePath="EventCalendar.xml", DisplayName="Lịch sự kiện (Event Calendar)",
 Category="Lịch sự kiện", ItemElement="EventCalendar", IdAttr="ID", NameAttr="EventName",
 Description="Các sự kiện định kỳ theo thứ/khung giờ (Blood Castle, Devil Square...). Tắt = gỡ khỏi lịch (bảo lưu để bật lại).",
 ListColumns=new\[\]{"ID","EventName","Weekday","Level","LinkID","EventAward"},
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
 ListColumns=new\[\]{"ID","GroupID","Name","Type","Price","PurchaseNum"},
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
 ListColumns=new\[\]{"GroupID","FromDate","ToDate","ServerOpenFromDate","ServerOpenToDate"},
 Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate"
 },

 // ===== Hoạt động hằng ngày =====
 new() {
 Key="everyday-activity", RelativePath="EveryDayActivity/EveryDayActivity.xml", DisplayName="Hoạt động hằng ngày",
 Category="Hoạt động hằng ngày", ItemElement="EveryDayActivity", IdAttr="ActivityID", NameAttr="Name",
 Description="Phần thưởng/mục tiêu hằng ngày theo GoalType.",
 ListColumns=new\[\]{"ActivityID","Name","GoalType","GoalNum","Price","PurchaseNum"},
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
 ListColumns=new\[\]{"GroupID","TypeID","Name","NeedType","NeedNum","ActivityID"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 new() { Field="TypeID", TargetKey="everyday-activity-type", Description="Loại hoạt động hằng ngày", MultiValue=false },
 new() { Field="ActivityID", TargetKey="everyday-activity", Description="Danh sách ActivityID cách nhau bởi '\|'", MultiValue=true, MultiSeparator='\|' }
 }
 },
 new() {
 Key="everyday-activity-type", RelativePath="EveryDayActivity/EveryDayActivityType.xml", DisplayName="Hoạt động hằng ngày - Loại",
 Category="Hoạt động hằng ngày", ItemElement="EveryDayActivityType", IdAttr="TypeID", NameAttr="Name",
 ListColumns=new\[\]{"TypeID","Name","OpenLevel","CloseLevel"},
 Toggle=ToggleStrategy.Park
 },

 // ===== Mở hệ thống =====
 new() {
 Key="version-system-open", RelativePath="VersionSystemOpen.xml", DisplayName="Mở hệ thống theo phiên bản (IsOpen)",
 Category="Mở hệ thống", ItemElement="Version", IdAttr="ID", NameAttr="SystemName",
 Description="Bật/tắt từng hệ thống bằng cờ IsOpen (1=mở, 0=đóng).",
 ListColumns=new\[\]{"ID","SystemName","IsOpen"},
 Toggle=ToggleStrategy.Flag, FlagAttr="IsOpen", FlagOn="1", FlagOff="0"
 },
 new() {
 Key="system-open", RelativePath="SystemOpen.xml", DisplayName="Mở hệ thống (điều kiện/thời gian)",
 Category="Mở hệ thống", ItemElement="System", IdAttr="ID", NameAttr="Name",
 Description="Cấu hình mở từng chức năng theo điều kiện kích hoạt.",
 ListColumns=new\[\]{"Order","ID","Name","TriggerCondition","TimeParameters","NotOpenShow"},
 Toggle=ToggleStrategy.Park
 },

 // ===== Sự kiện chủ đề (Theme) =====
 new() {
 Key="theme-open", RelativePath="ThemeActivityOpen.xml", DisplayName="Sự kiện chủ đề - Bật/Tắt tổng (Open)",
 Category="Sự kiện chủ đề", ItemElement="ThemeActivityOpen", IdAttr="ID", NameAttr="Title",
 Description="Công tắc tổng cho sự kiện chủ đề (Open=1/0).",
 ListColumns=new\[\]{"ID","Title","Open","Logo"},
 Toggle=ToggleStrategy.Flag, FlagAttr="Open", FlagOn="1", FlagOff="0"
 },
 new() {
 Key="theme-type", RelativePath="ThemeActivityType.xml", DisplayName="Sự kiện chủ đề - Loại",
 Category="Sự kiện chủ đề", ItemElement="ThemeActivityType", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Type","Name","EndData"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="theme-zhigou", RelativePath="ThemeActivityZhiGou.xml", DisplayName="Sự kiện chủ đề - Gói nạp (ZhiGou)",
 Category="Sự kiện chủ đề", ItemElement="ThemeActivityZhiGou", IdAttr="ID", NameAttr=null,
 ListColumns=new\[\]{"ID","Day","ZhiGouID","ChongZhiID","SinglePurchase"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 new() { Field="GoodsOne", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true },
 new() { Field="GoodsTwo", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm)", ParseRewardList=true }
 }
 },
 new() {
 Key="theme-boss", RelativePath="ThemeActivityBOSS.xml", DisplayName="Sự kiện chủ đề - BOSS",
 Category="Sự kiện chủ đề", ItemElement="ThemeActivityBOSS", IdAttr="ID", NameAttr=null,
 ListColumns=new\[\]{"ID","MonstersID","MapCode","GoodsList","TimePoints"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 // Da doi soat: 100% gia tri (90021,90024,9999001...) la Item.ID trong Goods.xml.
 new() { Field="GoodsList", TargetKey="", Description="Vật phẩm rơi (id đầu mỗi cụm '\|') -> Goods.xml", ParseRewardList=true }
 }
 },
 new() {
 Key="theme-zhuansheng", RelativePath="ThemeActivityZhuanSheng.xml", DisplayName="Sự kiện chủ đề - Chuyển sinh",
 Category="Sự kiện chủ đề", ItemElement="ThemeActivityZhuanSheng", IdAttr="ID", NameAttr=null,
 ListColumns=new\[\]{"ID","MonstersID","MapID","MinLevel","MaxLevel","TimePoints"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 // Da doi soat: 100% khop Goods.xml.
 new() { Field="GoodsList", TargetKey="", Description="Vật phẩm thưởng (id đầu mỗi cụm '\|') -> Goods.xml", ParseRewardList=true }
 }
 },

 // ===== Hoạt động hồi quy =====
 new() {
 Key="huigui-huodong", RelativePath="HuiGuiHuoDong.xml", DisplayName="Hoạt động hồi quy (HuiGui)",
 Category="Hồi quy / Gộp server", ItemElement="HuiGuiHuoDong", IdAttr="ID", NameAttr=null,
 Description="Sự kiện hồi quy người chơi, điều khiển bằng BeginTime/FinishTime.",
 ListColumns=new\[\]{"ID","HuoDongLevel","BeginTime","FinishTime","RegisterBegin","RegisterFinish"},
 Toggle=ToggleStrategy.DateWindow, FromAttr="BeginTime", ToAttr="FinishTime"
 },

 // ===== Sự kiện 7 ngày =====
 new() {
 Key="sevenday-goal", RelativePath="SevenDay/SevenDayGoal.xml", DisplayName="7 ngày - Mục tiêu",
 Category="Sự kiện 7 ngày", ItemElement="Goal", IdAttr="ID", NameAttr="Describe",
 ListColumns=new\[\]{"ID","Day","GoalType","FunctionType","Describe","ShowNum"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 new() { Field="Award", TargetKey="", Description="Vật phẩm thưởng (id đầu cụm '\|')", ParseRewardList=true }
 }
 },
 new() {
 Key="sevenday-type", RelativePath="SevenDay/SevenDayActivityType.xml", DisplayName="7 ngày - Loại hoạt động",
 Category="Sự kiện 7 ngày", ItemElement="ActivityType", IdAttr="ActivityType", NameAttr="Name",
 ListColumns=new\[\]{"ActivityType","Name","Tiptype","XML"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="sevenday-qianggou", RelativePath="SevenDay/SevenDayQiangGou.xml", DisplayName="7 ngày - Mua giới hạn",
 Category="Sự kiện 7 ngày", ItemElement="Goods", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Day","Name","GoodsID","Price","Purchase"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 new() { Field="GoodsID", TargetKey="", Description="ID vật phẩm trong Goods.xml", MultiValue=false }
 }
 },

 // ===== Tab hoạt động =====
 new() {
 Key="huodong-tab", RelativePath="HuoDongTab.xml", DisplayName="Tab hoạt động (HuoDong)",
 Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Name","GLXml"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="kuafu-huodong-tab", RelativePath="KuaFuHuoDongTab.xml", DisplayName="Tab hoạt động liên server",
 Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Name","GLXml"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="zhandui-huodong-tab", RelativePath="ZhanDuiHuoDongTab.xml", DisplayName="Tab hoạt động đội (ZhanDui)",
 Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Name","GLXml"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="zhanmeng-huodong-tab", RelativePath="ZhanMengHuoDongTab.xml", DisplayName="Tab hoạt động công hội",
 Category="Tab hoạt động", ItemElement="HuoDong", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Name"},
 Toggle=ToggleStrategy.Park
 },

 // ===== Hoạt động khác (Activity folder) =====
 new() {
 Key="activity-copy", RelativePath="Activity/Copy.xml", DisplayName="Phụ bản hoạt động (Copy)",
 Category="Hoạt động khác", ItemElement="Copy", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"Type","ID","Name","Level","MaxLevel"},
 Toggle=ToggleStrategy.Park
 },
 new() {
 Key="activity-boss", RelativePath="Activity/BossInfo.xml", DisplayName="BOSS hoạt động",
 Category="Hoạt động khác", ItemElement="Boss", IdAttr="ID", NameAttr="Description",
 ListColumns=new\[\]{"Type","ID","Level","NpcID","GoodsList","Show"},
 Toggle=ToggleStrategy.Park,
 ForeignKeys=new() {
 // Da doi soat: 100% gia tri (9999001,1005091...) la Item.ID trong Goods.xml.
 new() { Field="GoodsList", TargetKey="", Description="Vật phẩm rơi (id mỗi cụm '\|') -> Goods.xml", ParseRewardList=true }
 }
 },
 new() {
 Key="activity-tip", RelativePath="Activity/ActivityTip.xml", DisplayName="Gợi ý hoạt động (Tip)",
 Category="Hoạt động khác", ItemElement="Tip", IdAttr="ID", NameAttr="Name",
 ListColumns=new\[\]{"ID","Name","MinLevel","WeekDays","StartDay","OpenDay"},
 Toggle=ToggleStrategy.Park
 },


// ===================================================================
// EventRegistry.JieRiHeFu.COMPLETE.snippet.cs
// Thêm toàn bộ block này vào Services/EventRegistry.cs
// Vị trí: sau entry "huigui-huodong"
// ===================================================================

// ===== Quà lễ hội - JieRi Type XML (mapping) =====
new() { Key="jieri-type",      RelativePath="JieRiGifts/JieRiType.xml",         DisplayName="Lễ hội - Bảng loại (Type mapping)",
  Category="Quà lễ hội", ItemElement="Type", IdAttr="ID", NameAttr="Name",
  Description="Mapping ActivityType ID → file cấu hình lễ hội (ID 9-17). PeiZhi = tên file XML.",
  ListColumns=new[]{"ID","Name","PeiZhi"},
  Toggle=ToggleStrategy.Park },

new() { Key="jieri-mujieri-type", RelativePath="JieRiGifts/MuJieRiType.xml",   DisplayName="Lễ hội - Bảng loại mở rộng",
  Category="Quà lễ hội", ItemElement="Type", IdAttr="ID", NameAttr="Name",
  Description="Mapping các ActivityType lễ hội mở rộng (ID 50-77).",
  ListColumns=new[]{"ID","Name","PeiZhi"},
  Toggle=ToggleStrategy.Park },

// ===== Quà lễ hội - JieRi cơ bản (ID 9-17) =====
// Schema: <Activities ActivityType="N" FromDate="..." ToDate="..."/>
//         <GiftList> <Award TimeOl="N" GoodsOne="..." /> </GiftList>
new() { Key="jieri-libo",      RelativePath="JieRiGifts/JieRiLiBao.xml",        DisplayName="Lễ hội - Đại lễ bao (ID 9)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="TimeOl",
  Description="Thưởng đăng nhập lễ hội theo ngày thứ N",
  ListColumns=new[]{"TimeOl","GoodsOne","GoodsTwo","GoodsThr"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-denglu",    RelativePath="JieRiGifts/JieRiDengLu.xml",       DisplayName="Lễ hội - Đăng nhập 7 ngày (ID 10)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="TimeOl",
  Description="Thưởng tích lũy đăng nhập 1-7 ngày trong lễ hội",
  ListColumns=new[]{"TimeOl","GoodsOne","GoodsTwo","GoodsThr"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-vip",       RelativePath="JieRiGifts/JieRiVip.xml",          DisplayName="Lễ hội - VIP đặc quyền (ID 11)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-czsong",    RelativePath="JieRiGifts/JieRiChongZhiSong.xml", DisplayName="Lễ hội - Nạp tặng (ID 12)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-leiji",     RelativePath="JieRiGifts/JieRiLeiJi.xml",        DisplayName="Lễ hội - Tích lũy nạp (ID 13)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Condition",
  Description="Thưởng khi tích lũy nạp đạt mức (Condition = min amount)",
  ListColumns=new[]{"Condition","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-baoxiang",  RelativePath="JieRiGifts/JieRiBaoXiang.xml",     DisplayName="Lễ hội - Hộp quà thẻ chữ (ID 14)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-xiaofei-king", RelativePath="JieRiGifts/JieRiXiaoFeiKing.xml", DisplayName="Lễ hội - Vua tiêu thụ (ID 15)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  Description="Xếp hạng vua tiêu → Rank thưởng",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-cz-king",   RelativePath="JieRiGifts/JieRiChongZhiKing.xml", DisplayName="Lễ hội - Vua nạp (ID 16)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-boss",      RelativePath="JieRiGifts/JieRiBOSS.xml",         DisplayName="Lễ hội - Boss công thành (ID 17)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

// ===== Quà lễ hội mở rộng (ID 40-77) =====
// JieRiChongZhiQiangGou: schema đặc biệt - Award có ID, Day, Type, ZhiGouID, ChongZhiID
new() { Key="jieri-cz-qianggou", RelativePath="JieRiGifts/JieRiChongZhiQiangGou.xml", DisplayName="Lễ hội - Nạp cướp mua (ID 67)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  Description="Mua theo ZhiGouID, yêu cầu nạp ChongZhiID trước. Day=N,N là ngày áp dụng.",
  ListColumns=new[]{"ID","Day","TypeName","ZhiGouID","ChongZhiID","GoodsOne","SinglePurchase"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

// JieRiMeiRiLeiJi: schema Award có ID, Day, MinYuanBao (điều kiện nạp tối thiểu/ngày)
new() { Key="jieri-meiri-leiji", RelativePath="JieRiGifts/JieRiMeiRiLeiJi.xml", DisplayName="Lễ hội - Tích lũy nạp mỗi ngày (ID 70)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  Description="Award có Day (ngày thứ N trong event) + MinYuanBao (ngưỡng nạp)",
  ListColumns=new[]{"ID","Day","MinYuanBao","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

// Các JieRi khác - schema tương tự Award+TimeOl hoặc Award+ID
new() { Key="jieri-duobei",    RelativePath="JieRiGifts/JieRiDuoBei.xml",       DisplayName="Lễ hội - Bonus đa bội",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-fuli",      RelativePath="JieRiGifts/JieRiFuLi.xml",         DisplayName="Lễ hội - Phúc lợi (ID 66)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-day-cz",    RelativePath="JieRiGifts/JieRiDayChongZhi.xml",  DisplayName="Lễ hội - Nạp ngày",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-day-xiaofei", RelativePath="JieRiGifts/JieRiDayXiaoFei.xml", DisplayName="Lễ hội - Tiêu ngày",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-danbi-cz",  RelativePath="JieRiGifts/JieRiDanBiChongZhi.xml", DisplayName="Lễ hội - Đơn bút nạp (ID 69)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-leiji-xiaofei", RelativePath="JieRiGifts/JieRiLeiJiXiaoFei.xml", DisplayName="Lễ hội - Tích lũy tiêu",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Condition",
  ListColumns=new[]{"Condition","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-lianxu",    RelativePath="JieRiGifts/JieRiLianXu.xml",       DisplayName="Lễ hội - Nạp liên tiếp (ID 75)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-shouqu",    RelativePath="JieRiGifts/JieRiShouQu.xml",       DisplayName="Lễ hội - Thu nhận (ID 61/62)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-shouqu-king", RelativePath="JieRiGifts/JieRiShouQuKing.xml", DisplayName="Lễ hội - Vua thu nhận",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-zengsong",  RelativePath="JieRiGifts/JieRiZengSong.xml",     DisplayName="Lễ hội - Tặng (ID 50/51)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-zengsong-king", RelativePath="JieRiGifts/JieRiZengSongKing.xml", DisplayName="Lễ hội - Vua tặng",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-hongbao",   RelativePath="JieRiGifts/JieRiQuanMinHongBao.xml", DisplayName="Lễ hội - Hồng bao toàn dân",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-cz-hongbao", RelativePath="JieRiGifts/JieRiChongZhiHongBao.xml", DisplayName="Lễ hội - Hồng bao nạp (ID 73)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-hongbao-king", RelativePath="JieRiGifts/JieRiHongBaoBang.xml", DisplayName="Lễ hội - Hồng bao bảng vương (ID 74)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-cz-meiri-wang", RelativePath="JieRiGifts/JieRiMeiRiChongZhiWang.xml", DisplayName="Lễ hội - Ngày nạp vương",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  Description="Xếp hạng top nạp mỗi ngày trong lễ hội (13KB)",
  ListColumns=new[]{"ID","Day","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-vip-fanli", RelativePath="JieRiGifts/VIPFanLi.xml",           DisplayName="Lễ hội - VIP hoàn lễ (ID 58)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-vip-youhui", RelativePath="JieRiGifts/VIPYouHuiLiBao.xml",   DisplayName="Lễ hội - VIP ưu đãi lễ bao (ID 68)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-wing",      RelativePath="JieRiGifts/WingFanLi.xml",          DisplayName="Lễ hội - Cánh hoàn lễ (ID 55)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-qianghua",  RelativePath="JieRiGifts/QiangHuaFanLi.xml",      DisplayName="Lễ hội - Cường hóa hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-chengjiu",  RelativePath="JieRiGifts/ChengJiuFanLi.xml",      DisplayName="Lễ hội - Thành tựu hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-huiji",     RelativePath="JieRiGifts/JieRiHuiJiFanLi.xml",    DisplayName="Lễ hội - Hội kích hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-fuwen",     RelativePath="JieRiGifts/JieRiFuWenFanLi.xml",    DisplayName="Lễ hội - Phù văn hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-hunyin",    RelativePath="JieRiGifts/HunYinFanLi.xml",         DisplayName="Lễ hội - Hôn nhân hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-datianshen", RelativePath="JieRiGifts/DaTianShiFanLi.xml",    DisplayName="Lễ hội - Đại thiên thần hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-hushenfu",  RelativePath="JieRiGifts/HuShenFuFanLi.xml",      DisplayName="Lễ hội - Hộ thần phù hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-junxian",   RelativePath="JieRiGifts/JunXianFanLi.xml",        DisplayName="Lễ hội - Quân hàm hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-zhuijia",   RelativePath="JieRiGifts/ZhuiJiaFanLi.xml",        DisplayName="Lễ hội - Truy gia hoàn",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-pingtai-king", RelativePath="JieRiGifts/PingTaiChongZhiKing.xml", DisplayName="Lễ hội - Vua nạp platform (ID 100)",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-cz-duihuan", RelativePath="JieRiGifts/ChongZhiDuiHuan.xml",   DisplayName="Lễ hội - Nạp đổi vật phẩm",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="jieri-lv-type",   RelativePath="JieRiGifts/JieRiLvType.xml",         DisplayName="Lễ hội - Loại thư lv",
  Category="Quà lễ hội", ItemElement="Type", IdAttr="ID",
  ListColumns=new[]{"ID"},
  Toggle=ToggleStrategy.None },

new() { Key="jieri-lv",        RelativePath="JieRiGifts/JieRiLv.xml",             DisplayName="Lễ hội - Thư thưởng",
  Category="Quà lễ hội", ItemElement="Award", IdAttr="ID",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

// ===== Hợp phục (HeFuGifts) =====
// Schema: <Activities ActivityType="N" FromDate="..." ToDate="..."/>
//         <GiftList> <Award .../> </GiftList>
new() { Key="hefu-type",       RelativePath="HeFuGifts/HeFuType.xml",             DisplayName="Hợp phục - Bảng loại (Type mapping)",
  Category="Hợp phục / Gộp server", ItemElement="Type", IdAttr="ID", NameAttr="Name",
  Description="Mapping ActivityType ID 20-27 → file cấu hình hợp phục. PeiZhi = tên file.",
  ListColumns=new[]{"ID","Name","PeiZhi"},
  Toggle=ToggleStrategy.Park },

new() { Key="hefu-libo",       RelativePath="HeFuGifts/HeFuLiBao.xml",            DisplayName="Hợp phục - Đăng nhập hào lễ (ID 20)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-denglu",     RelativePath="HeFuGifts/HeFuDengLu.xml",           DisplayName="Hợp phục - Tích lũy đăng nhập (ID 21)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-xiangou",    RelativePath="HeFuGifts/HeFuQiangGou.xml",         DisplayName="Hợp phục - Shop giới hạn mua (ID 22)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="ID",
  Description="Award có ID, ZhiGouID, ChongZhiID, SinglePurchase tương tự JieRiChongZhiQiangGou",
  ListColumns=new[]{"ID","GoodsOne","SinglePurchase"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-fanli",      RelativePath="HeFuGifts/HeFuFanLi.xml",            DisplayName="Hợp phục - Nạp hoàn tiền (ID 23)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="Condition",
  ListColumns=new[]{"Condition","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-pk",         RelativePath="HeFuGifts/PKJiangLi.xml",            DisplayName="Hợp phục - Vua PK (ID 24)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-zhanchang",  RelativePath="HeFuGifts/HeFuZhangChang.xml",       DisplayName="Hợp phục - Vì chiến (ID 25)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-boss",       RelativePath="HeFuGifts/HeFuBOSS.xml",             DisplayName="Hợp phục - Boss chiến (ID 26)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-luolan",     RelativePath="HeFuGifts/HeFuLuoLan.xml",           DisplayName="Hợp phục - Luo Lan tranh bá (ID 27)",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-vip",        RelativePath="HeFuGifts/VIPLiBao.xml",             DisplayName="Hợp phục - VIP lễ bao",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-wangcheng",  RelativePath="HeFuGifts/WangChengJiangLi.xml",     DisplayName="Hợp phục - Vương thành thưởng",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="Rank",
  ListColumns=new[]{"Rank","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

new() { Key="hefu-czsong",     RelativePath="HeFuGifts/ChongZhiSong.xml",         DisplayName="Hợp phục - Nạp tặng",
  Category="Hợp phục / Gộp server", ItemElement="Award", IdAttr="TimeOl",
  ListColumns=new[]{"TimeOl","GoodsOne"},
  Toggle=ToggleStrategy.DateWindow, FromAttr="FromDate", ToAttr="ToDate" },

// ===== Daily Active (chưa có trong registry) =====
new() { Key="daily-active-config", RelativePath="DailyActiveInfor.xml",            DisplayName="Cấu hình nhiệm vụ hằng ngày",
  Category="Hoạt động hằng ngày", ItemElement="Tab", IdAttr="DailyActiveID", NameAttr="Name",
  Description="19 nhiệm vụ DailyActive (ID 100-1600). Mỗi tab = 1 nhiệm vụ với điều kiện hoàn thành + điểm thưởng.",
  ListColumns=new[]{"DailyActiveID","Name","Award","Minleve","Login","Online","Consumption"},
  Toggle=ToggleStrategy.Park },

new() { Key="daily-active-award", RelativePath="DailyActiveAward.xml",             DisplayName="Phần thưởng điểm hoạt động",
  Category="Hoạt động hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Đổi điểm HuoYue → vật phẩm. NeedhuoYue = điểm cần có. GoodsID = phần thưởng.",
  ListColumns=new[]{"ID","NeedhuoYue","GoodsID"},
  Toggle=ToggleStrategy.None },

// ===== Gift Code (214KB - cần pagination) =====
new() { Key="giftcode",        RelativePath="GiftCodeNew.xml",                     DisplayName="Gift Code",
  Category="Quà & Mã quà", ItemElement="GiftCode", IdAttr="TypeID", NameAttr="TypeName",
  Description="Bảng mã quà tặng (214KB). Mỗi GiftCode có TypeID (mã), TimeBegin/End, UseCount, GoodsOne.",
  ListColumns=new[]{"TypeID","TypeName","Note","TimeBegin","TimeEnd","UseCount","GoodsOne"},
  Toggle=ToggleStrategy.Park },

 
// ===================================================================
// EventRegistry.Expansion2.snippet.cs
// Thêm 28 entries: SevenDay (3) + RiChangGifts (6) + Arena (5) +
//                  TempleMirage (4) + AngelTemple (5) + VIP (3) + ZhanGong (2)
// Vị trí: sau entries đã có (cuối danh sách, trước // ===== Từ điển gốc)
// ===================================================================

// ===== Sự kiện 7 ngày — files bổ sung =====
new() { Key="sevenday-login",    RelativePath="SevenDay/SevenDayLogin.xml",    DisplayName="7 ngày - Thưởng đăng nhập",
  Category="Sự kiện 7 ngày", ItemElement="Award", IdAttr="ID",
  Description="Thưởng mỗi ngày trong 7 ngày đầu. ActivityTypeID=1. GoodsOne pipe-separated.",
  ListColumns=new[]{"ID","ActivityTypeID","GoodsOne","GoodsTwo"},
  Toggle=ToggleStrategy.Park },

new() { Key="sevenday-recharge", RelativePath="SevenDay/SevenDayChongZhi.xml", DisplayName="7 ngày - Nạp tiền",
  Category="Sự kiện 7 ngày", ItemElement="Award", IdAttr="ID",
  Description="Thưởng nạp tiền trong 7 ngày đầu. MinYuanBao = ngưỡng nạp tối thiểu.",
  ListColumns=new[]{"ID","MinYuanBao","GoodsOne"},
  Toggle=ToggleStrategy.Park },

new() { Key="sevenday-goal-type", RelativePath="SevenDay/GoalType.xml",        DisplayName="7 ngày - Loại mục tiêu",
  Category="Sự kiện 7 ngày", ItemElement="GoalType", IdAttr="ID", NameAttr="Name",
  Description="Mapping GoalTypeID → tên hiển thị và điều kiện. Dùng với SevenDayGoal.xml.",
  ListColumns=new[]{"ID","Name"},
  Toggle=ToggleStrategy.None },

// ===== Quà hằng ngày (RiChangGifts) =====
// JieRiType.xml trong RiChangGifts là KHÁC với JieRiGifts/JieRiType.xml
new() { Key="richanggift-type",  RelativePath="RiChangGifts/JieRiType.xml",    DisplayName="Quà hằng ngày - Bảng loại",
  Category="Quà hằng ngày", ItemElement="Type", IdAttr="ID", NameAttr="Name",
  Description="Mapping loại quà hằng ngày → file config. Khác với JieRiGifts/JieRiType.xml.",
  ListColumns=new[]{"ID","Name","PeiZhi"},
  Toggle=ToggleStrategy.Park },

new() { Key="richanggift-cz",    RelativePath="RiChangGifts/DayChongZhi.xml",  DisplayName="Quà hằng ngày - Nạp ngày (ID 27)",
  Category="Quà hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Thưởng nạp hằng ngày theo mốc MinYuanBao. GoodsIDs = 7-field format.",
  ListColumns=new[]{"ID","MinYuanBao","GoodsIDs"},
  Toggle=ToggleStrategy.Park,
  ForeignKeys=new() { new() { Field="GoodsIDs", TargetKey="goods", TargetIdAttr="ID", ParseRewardList=true } } },

new() { Key="richanggift-level", RelativePath="RiChangGifts/LevelAward.xml",   DisplayName="Quà hằng ngày - Thưởng cấp độ",
  Category="Quà hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Thưởng khi đạt cấp độ trong RiChangGifts.",
  ListColumns=new[]{"ID","Level","GoodsOne"},
  Toggle=ToggleStrategy.Park },

new() { Key="richanggift-dig1",  RelativePath="RiChangGifts/NewDig1.xml",      DisplayName="Quà hằng ngày - Đào vàng 1",
  Category="Quà hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Config hoạt động đào vàng loại 1.",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.Park },

new() { Key="richanggift-dig2",  RelativePath="RiChangGifts/NewDig2.xml",      DisplayName="Quà hằng ngày - Đào vàng 2",
  Category="Quà hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Config hoạt động đào vàng loại 2.",
  ListColumns=new[]{"ID","GoodsOne","Condition"},
  Toggle=ToggleStrategy.Park },

new() { Key="richanggift-shen",  RelativePath="RiChangGifts/ShenZhuangAward.xml", DisplayName="Quà hằng ngày - Thần Trang thưởng",
  Category="Quà hằng ngày", ItemElement="Award", IdAttr="ID",
  Description="Thưởng liên quan đến Thần Trang trong quà hằng ngày.",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.Park },

// ===== Đấu trường (Arena) =====
new() { Key="arena-config",   RelativePath="Arena/Arena.xml",               DisplayName="Đấu trường - Cấu hình chung",
  Category="Đấu trường", ItemElement="Item", IdAttr="ID",
  Description="Config đấu trường: giờ mở, điều kiện vào, cooldown...",
  ListColumns=new[]{"ID","TimePoints","MinLevel","PrepareTime"},
  Toggle=ToggleStrategy.None },

new() { Key="arena-award",    RelativePath="Arena/ArenaAward.xml",          DisplayName="Đấu trường - Phần thưởng theo rank",
  Category="Đấu trường", ItemElement="Goods", IdAttr="MinRank",
  Description="Exp+Reputation khi thắng/thua theo khoảng rank (MinRank-MaxRank).",
  ListColumns=new[]{"MinRank","MaxRank","WinExp","WinReputation","LoseExp"},
  Toggle=ToggleStrategy.None },

new() { Key="arena-cd",       RelativePath="Arena/ArenaCD.xml",             DisplayName="Đấu trường - Cooldown",
  Category="Đấu trường", ItemElement="Item", IdAttr="ID",
  Description="Cooldown giữa các trận đấu trường theo VIP level.",
  ListColumns=new[]{"ID","VIPLevel","CD"},
  Toggle=ToggleStrategy.None },

new() { Key="arena-rank",     RelativePath="Arena/MilitaryRank.xml",        DisplayName="Đấu trường - Quân hàm",
  Category="Đấu trường", ItemElement="Item", IdAttr="ID",
  Description="Quân hàm theo điểm/rank đấu trường.",
  ListColumns=new[]{"ID","Name","MinScore","MaxScore","Award"},
  Toggle=ToggleStrategy.None },

new() { Key="arena-3day",     RelativePath="Arena/ThreeDayArenaAward.xml",  DisplayName="Đấu trường - Thưởng 3 ngày",
  Category="Đấu trường", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng theo hạng sau 3 ngày đấu trường.",
  ListColumns=new[]{"ID","Rank","GoodsOne"},
  Toggle=ToggleStrategy.None },

// ===== Đền Thiên Thần (AngelTemple) =====
new() { Key="angel-temple",      RelativePath="AngelTemple.xml",            DisplayName="Đền Thiên Thần - Config chính",
  Category="Đền & Dungeon", ItemElement="Item", IdAttr="ID",
  Description="Config Đền Thiên Thần: TimePoints, BossID, Award (pipe-sep IDs), MaxEnterNum.",
  ListColumns=new[]{"ID","TimePoints","MinLevel","BossID","Award","MaxEnterNum"},
  Toggle=ToggleStrategy.None },

new() { Key="angel-award",       RelativePath="AngelTempleAward.xml",       DisplayName="Đền Thiên Thần - Phần thưởng",
  Category="Đền & Dungeon", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng khi hoàn thành Đền Thiên Thần.",
  ListColumns=new[]{"ID","GoodsOne","GoodsTwo"},
  Toggle=ToggleStrategy.None },

new() { Key="angel-lucky",       RelativePath="AngelTempleLuckyAward.xml",  DisplayName="Đền Thiên Thần - Thưởng may mắn",
  Category="Đền & Dungeon", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng may mắn (lucky) trong Đền Thiên Thần.",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.None },

new() { Key="angel-auction-award", RelativePath="AngelTempleAuctionAward.xml", DisplayName="Đền Thiên Thần - Thưởng đấu giá",
  Category="Đền & Dungeon", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng đấu giá trong Đền Thiên Thần.",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.None },

new() { Key="angel-auction-intro", RelativePath="AngelTempleAuctionIntro.xml", DisplayName="Đền Thiên Thần - Giới thiệu đấu giá",
  Category="Đền & Dungeon", ItemElement="Item", IdAttr="ID",
  Description="Giới thiệu item đấu giá trong Đền Thiên Thần.",
  ListColumns=new[]{"ID","GoodsID","BasePrice"},
  Toggle=ToggleStrategy.None },

// ===== Đền Mirage (TempleMirage) =====
new() { Key="temple-mirage",     RelativePath="TempleMirage.xml",           DisplayName="Đền Mirage - Config",
  Category="Đền & Dungeon", ItemElement="Item", IdAttr="ID",
  Description="Config Đền Mirage (幻影私源): map, boss, thời gian, điều kiện vào.",
  ListColumns=new[]{"ID","MapCode","TimePoints","BossID","MinLevel","MaxEnterNum"},
  Toggle=ToggleStrategy.None },

new() { Key="temple-mirage-rebirth", RelativePath="TempleMirageRebirth.xml", DisplayName="Đền Mirage - Hồi sinh",
  Category="Đền & Dungeon", ItemElement="Item", IdAttr="ID",
  Description="Config hồi sinh trong Đền Mirage.",
  ListColumns=new[]{"ID","Condition","GoodsID"},
  Toggle=ToggleStrategy.None },

new() { Key="continuity-kill",   RelativePath="ContinuityKillAward.xml",    DisplayName="Đền Mirage - Thưởng kill liên tiếp",
  Category="Đền & Dungeon", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng giết quái liên tiếp (kill streak) trong Đền Mirage.",
  ListColumns=new[]{"ID","KillCount","GoodsOne"},
  Toggle=ToggleStrategy.None },

new() { Key="holy-grail",        RelativePath="HolyGrail.xml",              DisplayName="Chén Thánh - Config",
  Category="Đền & Dungeon", ItemElement="Item", IdAttr="ID",
  Description="Config Chén Thánh (HolyGrail) - reward system trong Đền Mirage.",
  ListColumns=new[]{"ID","GoodsOne"},
  Toggle=ToggleStrategy.None },

// ===== VIP =====
new() { Key="vip-config",        RelativePath="MuVip.xml",                  DisplayName="VIP - Config cấp độ",
  Category="VIP", ItemElement="Item", IdAttr="ID",
  Description="Config VIP: mức exp, quyền lợi, giới hạn shop theo cấp VIP.",
  ListColumns=new[]{"ID","Exp","Benefit","ShopLimit"},
  Toggle=ToggleStrategy.None },

new() { Key="vip-daily-award",   RelativePath="VipDailyAwards.xml",         DisplayName="VIP - Thưởng hằng ngày",
  Category="VIP", ItemElement="Award", IdAttr="ID",
  Description="Phần thưởng hằng ngày theo cấp VIP.",
  ListColumns=new[]{"ID","VIPLevel","GoodsOne"},
  Toggle=ToggleStrategy.None },

new() { Key="vip-tab",           RelativePath="VipTab.xml",                 DisplayName="VIP - Tab đặc quyền",
  Category="VIP", ItemElement="Item", IdAttr="ID",
  Description="Config tab đặc quyền VIP trong UI.",
  ListColumns=new[]{"ID","Name","VIPRequired"},
  Toggle=ToggleStrategy.None },

// ===== ZhanGong Mall =====
new() { Key="zhangong-mall-type", RelativePath="ZhanGongMallType.xml",      DisplayName="Shop Chiến Công - Loại",
  Category="Shop & Mall", ItemElement="Type", IdAttr="ID", NameAttr="Name",
  Description="Mapping loại shop Chiến Công → danh sách tab mua.",
  ListColumns=new[]{"ID","Name"},
  Toggle=ToggleStrategy.None },

new() { Key="zhangong-mall",     RelativePath="ZhanGongMall.xml",           DisplayName="Shop Chiến Công - Hàng hóa",
  Category="Shop & Mall", ItemElement="Item", IdAttr="ID",
  Description="Danh sách vật phẩm bán trong Shop Chiến Công (dùng ZhanGong points).",
  ListColumns=new[]{"ID","GoodsID","Price","PurchaseNum"},
  Toggle=ToggleStrategy.None,
  ForeignKeys=new() { new() { Field="GoodsID", TargetKey="goods", TargetIdAttr="ID" } } },

    // ===== Ánh xạ tiền tệ / vật phẩm =====
 new() {
 Key="get-goods", RelativePath="GetGoods.xml", DisplayName="Ánh xạ loại tiền tệ -> Vật phẩm (GetGoods)",
 Category="Ánh xạ vật phẩm", ItemElement="GetGoods", IdAttr="ID", NameAttr="Name",
 Description="Bảng ánh xạ loại tiền tệ/điểm (Coin, Kim cương, Ma tinh...) sang Item.ID trong Goods.xml. Đã đối soát 100% khớp Goods.xml.",
 ListColumns=new\[\]{"ID","Name","Goods"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="Goods", TargetKey="", Description="Item.ID trong Goods.xml (vật phẩm đại diện loại tiền tệ)", MultiValue=false }
 }
 },

 // ===== TỪ ĐIỂN ID GỐC (master): goods, monster, magic, map, npc, dungeon, task, xilian =====
 // ===== Vật phẩm (master lớn nhất) =====
 // LƯU Ý: Goods.xml ~15MB / 9.720 Item. XmlEventService.LoadRecords đã có cache theo mtime
 // \+ chỉ mục ById nên back-reference/forward-link không parse lại 15MB mỗi lần quét.
 new() {
 Key="goods", RelativePath="Goods.xml", DisplayName="Vật phẩm (Goods)",
 Category="Từ điển gốc", ItemElement="Item", IdAttr="ID", NameAttr="Title",
 Description="Từ điển ~9.720 vật phẩm — master được hầu hết file Config tham chiếu (GoodsList, Award, NeedGoods...).",
 ListColumns=new\[\]{"ID","Title","Categoriy","PriceOne","JinJie","XiLian"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="JinJie", TargetKey="goods", TargetIdAttr="ID", Description="Thăng cấp/tiến giai -> Item.ID kế tiếp (self-ref). Khớp 100% (453/453).", MultiValue=false },
 new() { Field="XiLian", TargetKey="xilian", TargetIdAttr="ID", Description="Trỏ bảng tẩy luyện XiLianShuXing.xml. Khớp 100% (261/261). LƯU Ý: KHÔNG trỏ Goods (chỉ 3%).", MultiValue=false }
 }
 },

 // ===== Quái / Boss =====
 new() {
 Key="monster", RelativePath="Monsters.xml", DisplayName="Quái & Boss (Monsters)",
 Category="Từ điển gốc", ItemElement="Monster", IdAttr="ID", NameAttr="SName",
 Description="Từ điển ~2.064 quái/boss. Tham chiếu kỹ năng (SkillIDs->Magic), bản đồ (MapCode->Map), bảng rơi đồ (FallID).",
 ListColumns=new\[\]{"ID","SName","Level","MapCode","FallID","SkillIDs"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="SkillIDs", TargetKey="magic", TargetIdAttr="ID", Description="Kỹ năng của quái (->Magics.xml). Khớp 100% (187/187).", MultiValue=true, MultiSeparator=',' },
 new() { Field="MapCode", TargetKey="map", TargetIdAttr="Code", Description="Bản đồ quái xuất hiện (->MapConfig.xml). Khớp 99% (379/382, miss là map sự kiện).", MultiValue=true, MultiSeparator=',' },
 new() { Field="FallID", TargetKey="monster", TargetIdAttr="ID", Description="Chuỗi rơi đồ: trỏ tới một Monster khác (self-ref). Khớp 92% (392/427, miss là boss đặc biệt/liên server).", MultiValue=true, MultiSeparator=',' }
 }
 },

 // ===== Kỹ năng =====
 new() {
 Key="magic", RelativePath="Magics.xml", DisplayName="Kỹ năng (Magics)",
 Category="Từ điển gốc", ItemElement="Magic", IdAttr="ID", NameAttr="Name",
 Description="Từ điển ~448 kỹ năng. Được Monster.SkillIDs, vật phẩm học kỹ năng... tham chiếu.",
 ListColumns=new\[\]{"ID","Name","SkillType","ToOcuupation","MagicType"},
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
 ListColumns=new\[\]{"Code","BirthPosX","BirthPosY","BirthRadius"},
 Toggle=ToggleStrategy.None
 },

 // ===== NPC =====
 new() {
 Key="npc", RelativePath="npcs.xml", DisplayName="NPC",
 Category="Từ điển gốc", ItemElement="NPC", IdAttr="ID", NameAttr="SName",
 Description="Từ điển ~365 NPC. Tham chiếu bản đồ (MapCode->Map) và nhiệm vụ (Tasks->Task). Lưu ý: SaleID là ID cấu hình cửa hàng, KHÔNG phải Goods.ID.",
 ListColumns=new\[\]{"ID","SName","Function","MapCode","Tasks","SaleID"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="MapCode", TargetKey="map", TargetIdAttr="Code", Description="Bản đồ NPC đứng (->MapConfig.xml). Khớp 98% (43/44).", MultiValue=true, MultiSeparator=',' },
 new() { Field="Tasks", TargetKey="task", TargetIdAttr="ID", Description="Nhiệm vụ gắn với NPC (->EraTask/LegionTasks). Khớp 100% (4/4).", MultiValue=true, MultiSeparator=',' }
 // KHÔNG map SaleID -> goods: đối soát cho 0% khớp (SaleID là ID bảng bán hàng riêng).
 }
 },

 // ===== Phụ bản (Dungeon) =====
 new() {
 Key="dungeon", RelativePath="FuBen.xml", DisplayName="Phụ bản (FuBen)",
 Category="Từ điển gốc", ItemElement="Copy", IdAttr="ID", NameAttr="CopyName",
 Description="Từ điển ~290 phụ bản. Tham chiếu bản đồ (MapCode->Map), boss (BossID->Monster), phần thưởng (RewardGoods->Goods).",
 ListColumns=new\[\]{"ID","CopyName","MapCode","BossID","MinLevel","RewardGoods"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="MapCode", TargetKey="map", TargetIdAttr="Code", Description="Bản đồ phụ bản (->MapConfig.xml). Khớp 100% (290/290).", MultiValue=false },
 new() { Field="BossID", TargetKey="monster", TargetIdAttr="ID", Description="Boss của phụ bản (->Monsters.xml). Khớp 98% (59/60, miss là boss liên server).", MultiValue=false },
 new() { Field="RewardGoods", TargetKey="", Description="Vật phẩm thưởng (->Goods.xml), dạng 'id\|id\|id'. Khớp 100% (29/29).", MultiValue=true, MultiSeparator='\|' }
 }
 },

 // ===== Nhiệm vụ =====
 new() {
 Key="task", RelativePath="EraTask.xml", DisplayName="Nhiệm vụ - Kỷ nguyên (EraTask)",
 Category="Từ điển gốc", ItemElement="EraTask", IdAttr="ID", NameAttr="TaskName",
 Description="Nhiệm vụ theo kỷ nguyên. Được NPC.Tasks tham chiếu.",
 ListColumns=new\[\]{"ID","EraID","EraStage","TaskName","Description"},
 Toggle=ToggleStrategy.None
 },
 new() {
 Key="task-legion", RelativePath="LegionTasks.xml", DisplayName="Nhiệm vụ - Liên minh (LegionTasks)",
 Category="Từ điển gốc", ItemElement="LegionTasks", IdAttr="ID", NameAttr="Name",
 Description="Nhiệm vụ liên minh/quân đoàn (13 mục). Phần thưởng/kiểu hoàn thành theo TypeID.",
 ListColumns=new\[\]{"ID","Name","CompleteType","Exp","Describtion"},
 Toggle=ToggleStrategy.None
 },

 // ===== Tẩy luyện (XiLian) — master mới, được Goods.XiLian trỏ tới =====
 new() {
 Key="xilian", RelativePath="XiLianShuXing.xml", DisplayName="Thuộc tính tẩy luyện (XiLianShuXing)",
 Category="Từ điển gốc", ItemElement="XiLian", IdAttr="ID", NameAttr="Name",
 Description="Bảng thuộc tính tẩy luyện cho trang bị. Được Goods.XiLian trỏ tới (khớp 100% - 261/261). NeedGoods->Goods.ID.",
 ListColumns=new\[\]{"ID","Name","NeedGoods","NeedJinBi","NeedZuanShi"},
 Toggle=ToggleStrategy.None,
 ForeignKeys=new() {
 new() { Field="NeedGoods", TargetKey="", Description="Vật phẩm cần để tẩy luyện (->Goods.xml), dạng 'id,sl'.", ParseRewardList=true }
 }
 },

 // =================================================================================
 // GHI CHÚ: def "goods" (master vật phẩm) đã được đăng ký ở ĐẦU block này, kèm 2 FK
 // tự-trỏ JinJie (->goods) và XiLian (->xilian). Khi dán, đảm bảo def "goods" và
 // "xilian" đều có mặt để forward-link/back-reference resolve đúng.
 // =================================================================================
 };

 public static readonly string\[\] FestivalFiles = new\[\]
 {
 "JieRiBOSS","JieRiBaoXiang","JieRiChongZhiFanLi","JieRiChongZhiHongBao","JieRiChongZhiKing",
 "JieRiChongZhiQiangGou","JieRiChongZhiSong","JieRiDanBiChongZhi","JieRiDayChongZhi","JieRiDayXiaoFei",
 "JieRiDengLu","JieRiDuoBei","JieRiFuLi","JieRiFuWenFanLi","JieRiHongBaoBang","JieRiHuiJiFanLi",
 "JieRiLeiJi","JieRiLeiJiXiaoFei","JieRiLiBao","JieRiLianXu","JieRiMeiRiChongZhiWang","JieRiMeiRiLeiJi",
 "JieRiQuanMinHongBao","JieRiShouQu","JieRiShouQuKing","JieRiVip","JieRiXiaoFeiKing","JieRiZengSong","JieRiZengSongKing"
 };

 public static readonly string\[\] HeFuFiles = new\[\]
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
 ListColumns = new\[\] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
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
 ListColumns = new\[\] { "ActivityType", "FromDate", "ToDate", "AwardStartDate", "AwardEndDate" },
 Toggle = ToggleStrategy.DateWindow, FromAttr = "FromDate", ToAttr = "ToDate"
 });
 }
 }

 public static EventFileDef? Get(string key) => Files.FirstOrDefault(f => f.Key == key);

 public static IEnumerable\> ByCategory() => Files.GroupBy(f => f.Category);

 /// Nhãn tiếng Việt cho các thuộc tính phổ biến (hiển thị trợ giúp).
 public static readonly Dictionary AttrLabels = new(StringComparer.OrdinalIgnoreCase)
 {
 \["ID"\] = "Mã", \["GroupID"\] = "Mã nhóm", \["ActivityID"\] = "Mã hoạt động", \["TypeID"\] = "Mã loại",
 \["Name"\] = "Tên", \["EventName"\] = "Tên sự kiện", \["SystemName"\] = "Tên hệ thống", \["Title"\] = "Tiêu đề",
 \["Value"\] = "Giá trị (tham số)",
 \["Weekday"\] = "Thứ trong tuần (1-7)", \["Level"\] = "Khoảng cấp (min,max)", \["Time"\] = "Khung giờ chạy",
 \["LinkID"\] = "ID liên kết chơi", \["EventAward"\] = "Phần thưởng (id)", \["CompletedTaskID"\] = "Task cần hoàn thành",
 \["VipLevel"\] = "Cấp VIP yêu cầu", \["FromDate"\] = "Bắt đầu", \["ToDate"\] = "Kết thúc",
 \["BeginTime"\] = "Bắt đầu", \["FinishTime"\] = "Kết thúc", \["RegisterBegin"\] = "Mở đăng ký", \["RegisterFinish"\] = "Đóng đăng ký",
 \["ServerOpenFromDate"\] = "Tính từ ngày mở server (từ)", \["ServerOpenToDate"\] = "Tính từ ngày mở server (đến)",
 \["AwardStartDate"\] = "Nhận thưởng từ", \["AwardEndDate"\] = "Nhận thưởng đến",
 \["IsOpen"\] = "Mở? (1/0)", \["Open"\] = "Mở? (1/0)", \["GoalType"\] = "Loại mục tiêu", \["GoalNum"\] = "Yêu cầu mục tiêu",
 \["GoodsOne"\] = "Vật phẩm 1 (id,sl,binding,...)", \["GoodsTwo"\] = "Vật phẩm 2", \["GoodsThr"\] = "Vật phẩm 3",
 \["Price"\] = "Giá (loại\|giá\|zhiGouID)", \["PurchaseNum"\] = "Số lần mua (-1=vô hạn)",
 \["Day"\] = "Ngày áp dụng", \["Type"\] = "Loại", \["TriggerCondition"\] = "Điều kiện kích hoạt",
 \["TimeParameters"\] = "Tham số thời gian", \["NotOpenShow"\] = "Hiện khi chưa mở", \["Order"\] = "Thứ tự",
 \["TimePoints"\] = "Các mốc thời gian", \["MonstersID"\] = "ID quái", \["MapCode"\] = "Mã bản đồ", \["MapID"\] = "Mã bản đồ",
 \["NeedLevel"\] = "Cấp yêu cầu", \["NeedVIP"\] = "VIP yêu cầu", \["NeedChongZhi"\] = "Nạp yêu cầu",
 };
}