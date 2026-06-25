# Database Schema — RoleData (ProtoBuf)

> Nguồn: `GameServer/Server/Data/RoleData.cs` (561 dòng), `RoleDailyData.cs`, `GoodsData.cs`, `DailyActiveData.cs`
> Độ tin cậy: **HIGH** — [ProtoContract] = đây là schema truyền qua mạng và lưu DB.

---

## 1. RoleData (ProtoContract — 180 fields)

Đây là "God object" của nhân vật. Truyền client↔server qua ProtoBuf.

### Nhóm Identity
| Proto | Field | Type | Mô tả |
|-------|-------|------|-------|
| 1 | RoleID | int | Primary key |
| 2 | RoleName | string | Tên nhân vật |
| 3 | RoleSex | int | Giới tính |
| 4 | Occupation | int | Nghề nghiệp |
| 250 | UserPTID | int | ID platform |
| 251 | WorldRoleID | string | ID liên server |
| 252 | Channel | string | Kênh đăng ký |

### Nhóm Stats & Level
| Proto | Field | Type | Mô tả |
|-------|-------|------|-------|
| 5 | Level | int | Cấp độ |
| 9 | Experience | long | Kinh nghiệm |
| 110 | ChangeLifeCount | int | Số lần chuyển sinh |
| 114 | CombatForce | int | Chiến lực |
| 121 | VIPLevel | int | Cấp VIP |
| 163 | RebornCombatForce | int | Chiến lực sau reborn |
| 164 | RebornCount | int | Số lần reborn |
| 165 | RebornLevel | int | Cấp reborn |

### Nhóm Tiền tệ (MoneyData = LongCollection)
| Proto | Field | Type | Ghi chú |
|-------|-------|------|---------|
| 30 | UserMoney | int | YuanBao (kim cương) |
| 31 | Money1 (Gold) | int | Vàng |
| 32 | Money2 (Silver) | int | Bạc |
| 149 | MoneyData | LongCollection | Tất cả loại tiền (IsRequired=true) |

### Nhóm Position
| Proto | Field | Type |
|-------|-------|------|
| 12 | MapCode | int |
| 13 | PosX | int |
| 14 | PosY | int |
| 15 | RoleDirection | int |

### Nhóm Inventory
| Proto | Field | Type | Mô tả |
|-------|-------|------|-------|
| 23 | GoodsDataList | List\<GoodsData\> | Túi chính |
| 154 | GoodsLimitDataList | List\<GoodsLimitData\> | Túi giới hạn |
| 167 | RebornGoodsDataList | List\<GoodsData\> | Túi reborn |
| 465 | MountStoreList | List\<GoodsData\> | Kho ngựa |
| 468 | MountEquipList | List\<GoodsData\> | Trang bị ngựa |
| 543 | HolyGoodsDataList | List\<GoodsData\> | Trang bị thần thánh |

### Nhóm Guild / Social
| Proto | Field | Type | Mô tả |
|-------|-------|------|-------|
| 75 | BHName | string | Tên bang hội |
| 77 | BHZhiWu | int | Chức vụ bang hội |
| 78 | BangGong | int | Điểm bang công |
| 140 | JunTuanId | int | ID công hội |
| 437 | SpouseId | int | ID vợ/chồng |

### Nhóm Event-related
| Proto | Field | Type | Mô tả |
|-------|-------|------|-------|
| 97 | JieriStartDay | string | Ngày bắt đầu Jieri |
| 98 | JieriDaysNum | int | Số ngày đã tham gia Jieri |
| 99 | HefuStartDay | string | Ngày bắt đầu HeFu |
| 116 | DayOnlineSecond | int | Giây online ngày hiện tại |
| 117 | SeriesLoginNum | int | Số ngày login liên tiếp |
| 88 | OnceAwardFlag | long | Bitmask phần thưởng 1-lần |
| 138 | ActivityList | List\<ActivityData\> | Danh sách hoạt động |

### Nhóm DB Keys (qua RoleParamName)
Các key DB quan trọng cho event system (lưu bằng `Global.GetRoleParamsXxx`):

| DB Key | Type | Mô tả |
|--------|------|-------|
| `"DailyActiveFlag"` | `List<ulong>` | Bitmask 2-bit/activity |
| `"DailyActiveInfo1"` | `List<uint>` | Counter từng hoạt động |
| `"DailyActiveAwardFlag"` | `int` | Bitmask nhận thưởng |
| `"DailyActiveDayID"` | `int` | DayOfYear reset |
| `"SevenDayFlag"` | bitmask | Ngày đã login |
| `"10175"` | `int` | Tổng mua mall (cho WebOldPlayer) |

---

## 2. RoleDailyData (reset hàng ngày)

```csharp
class RoleDailyData {
    int ExpDayID;          // DayID khi tính EXP
    int TodayExp;          // EXP kiếm hôm nay
    int LingLiDayID;       // DayID linh lực
    int TodayLingLi;       // Linh lực hôm nay
    int KillBossDayID;     // DayID giết boss
    int TodayKillBoss;     // Số boss giết hôm nay ← dùng trong CheckDailyActiveKillBoss
    int FuBenDayID;        // DayID dungeon
    int TodayFuBenNum;     // Số dungeon hôm nay
    int WuXingDayID;       // DayID ngũ hành
    int WuXingNum;         // Số ngũ hành hôm nay
    int RebornExpDayID;    // DayID reborn EXP
    int RebornExpMonster;  // Reborn EXP từ quái
    int RebornExpSale;     // Reborn EXP từ bán
}
```

---

## 3. GoodsData (ProtoContract — 26 fields)

```csharp
class GoodsData {
    int Id;              // DB ID (=-1 nếu chưa lưu)
    int GoodsID;         // Template ID (→ Goods.xml)
    int Using;           // Đang dùng? (1=equipped, 0=bag)
    int Forge_level;     // Cấp cường hóa
    string Starttime;    // Thời hạn bắt đầu ("1900-01-01 12:00:00" = vĩnh viễn)
    string Endtime;      // Thời hạn kết thúc
    int Site;            // Vị trí trang bị (slot)
    int Quality;         // Phẩm chất
    string Props;        // Thuộc tính thêm (serialized)
    int GCount;          // Số lượng
    int Binding;         // Khóa? (1=bound, 0=free)
    string Jewellist;    // Danh sách đính ngọc
    int BagIndex;        // Vị trí trong túi
    int SaleMoney1;      // Giá bán (vàng)
    int SaleYuanBao;     // Giá bán (kim cương)
    int SaleYinPiao;     // Giá bán (bạc phiếu)
    int AddPropIndex;    // Index thuộc tính truy gia
    int BornIndex;       // Index thuộc tính sinh ra
    int Lucky;           // Độ may mắn
    int Strong;          // Độ bền
    int ExcellenceInfo;  // Info xuất sắc (bitmask)
    int AppendPropLev;   // Cấp truy gia
    int ChangeLifeLevForEquip; // Cấp chuyển sinh của trang bị
    List<int> WashProps; // Thuộc tính tẩy luyện
    List<int> ElementhrtsProps; // Thuộc tính ngũ hành
    int JuHunID;         // ID tụ hồn
}
```

**GoodsIDs XML format** (7 trường, khớp với 7 field đầu):
```
GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo
```

---

## 4. DailyActiveData (Packet 558 — internal)

```csharp
internal class DailyActiveData {
    int RoleID;
    long DailyActiveValues;          // Tổng điểm hoạt động
    long TotalKilledMonsterCount;
    long DailyActiveTotalLoginCount;
    int DailyActiveOnLineTimer;      // Giây online
    List<ushort> DailyActiveInforFlags; // Bitmask flags (encoded 2-bit/activity)
    int NowCompletedDailyActiveID;   // ID vừa hoàn thành (-1 nếu không)
    int TotalKilledBossCount;
    int PassNormalCopySceneNum;      // Dungeon thường
    int PassHardCopySceneNum;        // Dungeon khó
    int PassDifficultCopySceneNum;   // Dungeon cực khó
    int BuyItemInMall;               // Số lần mua shop
    int CompleteDailyTaskCount;      // Daily task
    int CompleteBloodCastleCount;    // Blood Castle
    int CompleteDaimonSquareCount;   // Daimon Square
    int CompleteBattleCount;         // Battle (TianTi?)
    int EquipForge;                  // Lần cường hóa
    int EquipAppend;                 // Lần truy gia
    int ChangeLife;                  // Lần chuyển sinh
    int MergeFruit;                  // Lần hợp thành
    int GetAwardFlag;                // Bitmask thưởng đã nhận
}
```

---

## 5. EverydayActInfoDB (EverydayActivity mới)

```csharp
class EverydayActInfoDB {
    int ActID;       // ID hoạt động
    int FinishCount; // Số lần hoàn thành
    int AwardCount;  // Số lần nhận thưởng
    // DateTime LastTime (từ timestamp)
}

class EverydayActGroupInfoDB {
    int GroupID;       // GroupID (khóa với EveryDayActivityGroup.xml)
    DateTime FromDate;
    DateTime ToDate;
}
```

---

## 6. Tóm tắt Dependency Graph

```
RoleData
  ├── GoodsDataList[]       → Goods.xml (GoodsID)
  ├── SkillDataList[]       → Magics.xml
  ├── FuBenDataList[]       → FuBen.xml
  ├── MyWingData            → Wing/*.xml
  ├── ActivityList[]        → ActivityNew/*
  ├── MoneyData             → GetGoods.xml (currency mapping)
  └── RoleParamsDB (key-value)
       ├── DailyActiveFlag  → EveryDayActivity.xml (bitmask)
       ├── DailyActiveInfo1 → counters per activity
       ├── DailyActiveAwardFlag → award bitmask
       └── SevenDayFlag     → SevenDay/*.xml

RoleDailyData (reset daily)
  └── TodayKillBoss → DailyActiveManager.CheckDailyActiveKillBoss()
```
