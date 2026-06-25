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
