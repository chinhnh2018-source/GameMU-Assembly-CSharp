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
