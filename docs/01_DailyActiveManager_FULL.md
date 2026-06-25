# DailyActiveManager — Reverse Engineering FULL

> Nguồn: `GameServer/Logic/DailyActiveManager.cs` (809 dòng) + `DailyActiveTypes.cs` + `DailyActiveDataField1.cs`
> Độ tin cậy: **HIGH** — đọc trực tiếp từ code thực tế.

---

## 1. Bảng DailyActiveTypes (ID → Tên)

| ID | Hằng số | Trigger | Field DB |
|----|---------|---------|---------|
| 100 | `LoginGameCount` | `ProcessLoginForDailyActive()` | `DailyActiveDayLoginNum` |
| 200 | `SeriesLogin` | `ProcessOnlineForDailyActive()` | `DailyActiveOnline` |
| 300 | `MallBuyCount` | `ProcessBuyItemInMallForDailyActive()` | `DailyActiveBuyItemInMall` |
| 400 | `CompleteDailyTaskCount1` | `ProcessCompleteDailyTaskForDailyActive()` | `DailyActiveCompleteDailyTask` |
| 401 | `CompleteDailyTaskCount2` | `ProcessCompleteDailyTaskForDailyActive()` | `DailyActiveCompleteDailyTask` |
| 500 | `CompleteNormalCopyMapCount1` | `ProcessCompleteCopyMapForDailyActive(lev=1)` | `DailyActiveCompleteCopyMap1` |
| 600 | `CompleteHardCopyMapCount1` | `ProcessCompleteCopyMapForDailyActive(lev=2)` | `DailyActiveCompleteCopyMap2` |
| 700 | `CompleteDifficltCopyMapCount1` | `ProcessCompleteCopyMapForDailyActive(lev=3)` | `DailyActiveCompleteCopyMap3` |
| 800 | `CompleteBloodCastle` | `ProcessCompleteDailyActivityForDailyActive(type=1)` | `DailyActiveCompleteBloodCastle` |
| 900 | `CompleteDaimonSquare` | `ProcessCompleteDailyActivityForDailyActive(type=2)` | `DailyActiveCompleteDaimonSquare` |
| 1000 | `CompleteBattle` | `ProcessCompleteDailyActivityForDailyActive(type=3)` | `DailyActiveCompleteBattle` |
| 1100 | `EquipForge` | `ProcessDailyActiveEquipForge()` | `DailyActiveEquipForge` |
| 1200 | `EquipAppend` | `ProcessDailyActiveEquipAppend()` | `DailyActiveEquipAppend` |
| 1300 | `KillMonster1` | `ProcessDailyActiveKillMonster()` | `DailyActiveTotalKilledMonsterNum` |
| 1301 | `KillMonster2` | `ProcessDailyActiveKillMonster()` | `DailyActiveTotalKilledMonsterNum` |
| 1302 | `KillMonster3` | `ProcessDailyActiveKillMonster()` | `DailyActiveTotalKilledMonsterNum` |
| 1400 | `KillBoss` | `CheckDailyActiveKillBoss()` | `DailyActiveTotalKilledBossNum` |
| 1500 | `CompleteChangeLife` | `ProcessDailyActiveChangeLife()` | `DailyActiveChangeLife` |
| 1600 | `MergeFruit` | `ProcessDailyActiveMergeFruit()` | `DailyActiveMergeFruit` |

---

## 2. Cấu trúc DailyActiveDataField1 (enum - index trong DailyActiveInfo1[])

```csharp
enum DailyActiveDataField1 {
    DailyActiveValue          = 0,  // Tổng điểm hoạt động
    DailyActiveTotalKilledMonsterNum = 1,
    DailyActiveDayLoginNum    = 2,
    DailyActiveBuyItemInMall  = 3,
    DailyActiveCompleteDailyTask = 4,
    DailyActiveCompleteCopyMap1  = 5,  // Normal
    DailyActiveCompleteCopyMap2  = 6,  // Hard
    DailyActiveCompleteCopyMap3  = 7,  // Difficult
    DailyActiveCompleteBloodCastle = 8,
    DailyActiveCompleteDaimonSquare = 9,
    DailyActiveCompleteBattle  = 10,
    DailyActiveEquipForge      = 11,
    DailyActiveEquipAppend     = 12,
    DailyActiveChangeLife      = 13,
    DailyActiveMergeFruit      = 14,
    DailyActiveOnline          = 15,
    DailyActiveTotalKilledBossNum = 16
}
```

**DB Key**: `"DailyActiveInfo1"` → `List<uint>`, index = enum value.

---

## 3. CheckLevCondition (điều kiện cấp độ)

```csharp
// DailyActiveManager.cs:754
bool CheckLevCondition(GameClient client, int daTpye) {
    // Đọc XML systemDailyActiveInfo[daTpye]
    int minZhuansheng = xml.GetIntValue("MinZhuanshengleve", -1);
    if (client.ChangeLifeCount < minZhuansheng) return false;
    if (client.ChangeLifeCount == minZhuansheng) {
        int minLeve = xml.GetIntValue("Minleve", -1);
        if (client.Level < minLeve) return false;
    }
    return true;
}
```

**Áp dụng cho**: Tất cả 19 loại trên (MinZhuanshengleve + Minleve từ EveryDayActivity.xml).

---

## 4. Flag Storage (DailyActiveFlag)

```
DB Key: "DailyActiveFlag" → List<ulong> (bitmask)
- Mỗi ID chiếm 2 bits:
  - bit[index+0] = "Completed" flag
  - bit[index+1] = "Award Fetched" flag
- index lấy từ m_DailyActiveInfo[ID]
- DailyActiveAwardFlag: int bitmask (GetBitValue(nID+1))
```

---

## 5. Reset Daily (CleanDailyActiveInfo)

```csharp
// DailyActiveManager.cs:786
void CleanDailyActiveInfo(GameClient client) {
    // Reset DailyActiveFlag → List<ulong> rỗng
    // Reset DailyActiveInfo1 → List<uint> rỗng
    // Lưu DailyActiveDayID = TimeUtil.NowDateTime().DayOfYear
    // Reset DailyActiveAwardFlag = 0
}
```

**Trigger**: So sánh `DailyActiveDayID` với `DayOfYear` khi login.

---

## 6. Packet 558 (NotifyClientDailyActiveData)

Struct `DailyActiveData` gửi tới client:
```
RoleID, DailyActiveValues, TotalKilledMonsterCount,
DailyActiveTotalLoginCount, DailyActiveOnLineTimer,
DailyActiveInforFlags (List<ushort>), NowCompletedDailyActiveID,
TotalKilledBossCount, PassNormalCopySceneNum, PassHardCopySceneNum,
PassDifficultCopySceneNum, BuyItemInMall, CompleteDailyTaskCount,
CompleteBloodCastleCount, CompleteDaimonSquareCount, CompleteBattleCount,
EquipForge, EquipAppend, ChangeLife, MergeFruit, GetAwardFlag
```

---

## 7. VIP Bonus Points

```csharp
// DailyActiveManager.cs:128
int[] vipBonus = systemParamsList.GetParamValueIntArrayByName("VIPHuoYueAdd", ',');
// VIPHuoYueAdd = "0,1,2,2,3,3,5,5,7,7,10,10,15,15,15,15"
// index = vipLevel → bonus thêm vào điểm hoạt động
```

---

## 8. XML field → Check field mapping

| ID | XML Field kiểm tra | Mô tả |
|----|-------------------|-------|
| 100 | `Login` | Số lần đăng nhập |
| 200 | `Online` | Phút online |
| 300 | `Consumption` | Số YuanBao tiêu |
| 400/401 | `RiChang` | Hoàn thành daily task |
| 500/600/700 | `KillRaid` | Số lần clear dungeon |
| 800/900/1000 | `HuoDongLimit` | Số lần dự hoạt động |
| 1100 | `QiangHuaLimit` | Số lần cường hóa |
| 1200 | `ZhuiJiaLimit` | Số lần truy gia |
| 1300-1302 | `KillMonster` | Số quái đã giết |
| 1400 | `KillBoss` | Số boss đã giết |
| 1500 | `ZhuanShengLimit` | Số lần chuyển sinh |
| 1600 | `HeChengLimit` | Số lần hợp thành |
