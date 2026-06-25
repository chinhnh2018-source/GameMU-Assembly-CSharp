# 20_XMLSchema_Complete.md
> Schema XML đọc trực tiếp từ GameRes. Confidence: HIGH
> Date: 2026-06-25

---

## 1. DailyActiveInfor.xml — Schema thực tế

**Path**: `GameRes/GameRes/Config/DailyActiveInfor.xml`  
**Loader**: `GameManager.systemDailyActiveInforMgr`

```xml
<Config>
  <DailyActive>
    <Tab
      ID="1"                          <!-- Thứ tự hiển thị UI -->
      DailyActiveID="100"             <!-- Khớp DailyActiveTypes.cs const -->
      Name="登录游戏"                 <!-- Tên hiển thị -->
      MinZhuanshengleve="0"           <!-- Yêu cầu mức đổi nghề tối thiểu -->
      Minleve="1"                     <!-- Yêu cầu cấp độ tối thiểu -->
      Login="1"                       <!-- Điều kiện: đăng nhập N lần -->
      Online=""                       <!-- Điều kiện: online N phút -->
      Consumption=""                  <!-- Điều kiện: tiêu N đồng -->
      RiChang=""                      <!-- Điều kiện: hoàn thành N nhiệm vụ ngày -->
      KillRaid=""                     <!-- Điều kiện: vào N lần dungeon -->
      HuoDongLimit=""                 <!-- Điều kiện: tham gia N lần event -->
      QiangHuaLimit=""                <!-- Điều kiện: cường hóa N lần -->
      ZhuiJiaLimit=""                 <!-- Điều kiện: truy gia N lần -->
      KillMonster=""                  <!-- Điều kiện: giết N quái -->
      KillBoss=""                     <!-- Điều kiện: giết N boss -->
      ZhuanShengLimit=""              <!-- Điều kiện: đổi nghề N lần -->
      TiQuYuanSu=""                   <!-- Điều kiện: rút nguyên tố N lần -->
      PeiYangZhuangBei=""             <!-- Điều kiện: nuôi trang bị N lần -->
      HeChengLimit=""                 <!-- Điều kiện: hợp thành "itemID,count" -->
      Award="5"                       <!-- Điểm thưởng khi hoàn thành -->
    />
  </DailyActive>
</Config>
```

**19 Tab records** khớp DailyActiveTypes:

| Tab | DailyActiveID | Name | Condition field | Value | Award pts |
|-----|--------------|------|----------------|-------|-----------|
| 1 | 100 | Đăng nhập | Login | 1 | 5 |
| 2 | 200 | Online 30 phút | Online | 30 | 5 |
| 3 | 300 | Mua Mall 1 viên | Consumption | 1 | 10 |
| 4 | 400 | Hoàn thành 10 NV ngày | RiChang | 10 | 10 |
| 5 | 500 | Vào 3 lần dungeon thường | KillRaid | 3 | 5 |
| 5 | 600 | Vào 3 lần dungeon khó | KillRaid | 3 | 10 |
| 5 | 700 | Vào 3 lần dungeon luyện ngục | KillRaid | 3 | 15 |
| 6 | 800 | Blood Castle 1 lần | HuoDongLimit | 1 | 5 |
| 6 | 900 | Daimon Square 1 lần | HuoDongLimit | 1 | 5 |
| 7 | 1000 | Trận doanh 1 lần | HuoDongLimit | 1 | ? |
| - | 1100 | Cường hóa | QiangHuaLimit | - | - |
| - | 1200 | Truy gia | ZhuiJiaLimit | - | - |
| - | 1300-1302 | Giết quái | KillMonster | - | - |
| - | 1400 | Giết Boss | KillBoss | - | - |
| - | 1500 | Đổi nghề | ZhuanShengLimit | - | - |
| 12 | 1600 | Hợp thành (MinZhuan=100) | HeChengLimit | 5002,1 | 5 |

---

## 2. JieRiType.xml — Mapping ID → XML file

**Path**: `GameRes/GameRes/Config/JieRiGifts/JieRiType.xml`

```xml
<Config>
  <Type ID="9"  Name="情人大礼包"  PeiZhi="JieRiLiBao.xml"/>       <!-- Lễ hội bao lì xì -->
  <Type ID="10" Name="七日登录豪礼" PeiZhi="JieRiDengLu.xml"/>      <!-- Đăng nhập 7 ngày -->
  <Type ID="11" Name="VIP大回馈"   PeiZhi="JieRiVip.xml"/>         <!-- VIP hoàn tiền -->
  <Type ID="12" Name="充值大回馈"   PeiZhi="JieRiChongZhiSong.xml"/><!-- Nạp tặng -->
  <Type ID="13" Name="累计充值豪礼" PeiZhi="JieRiLeiJi.xml"/>       <!-- Tích lũy nạp -->
  <Type ID="14" Name="字卡换礼盒"   PeiZhi="JieRiBaoXiang.xml"/>    <!-- Đổi lì xì bằng thẻ chữ -->
  <Type ID="15" Name="情人消费王"   PeiZhi="JieRiXiaoFeiKing.xml"/> <!-- Vua tiêu -->
  <Type ID="16" Name="情人充值王"   PeiZhi="JieRiChongZhiKing.xml"/><!-- Vua nạp -->
  <Type ID="17" Name="BOSS攻城"    PeiZhi="JieRiBOSS.xml"/>        <!-- Đánh Boss -->
</Config>
```

> **Note**: `PeiZhi` = tên file XML config. JieRiType.xml chỉ định nghĩa 9 types cơ bản (ID 9-17).
> Các type từ ID 40-77 không có trong JieRiType.xml → được load trực tiếp từ MuJieRiType.xml hoặc hardcode trong code.

---

## 3. HeFuType.xml — Mapping ID → XML file

**Path**: `GameRes/GameRes/Config/HeFuGifts/HeFuType.xml`

```xml
<Config>
  <Type ID="20" Name="登录豪礼"  PeiZhi="HeFuLiBao.xml"/>      <!-- Đăng nhập hào lễ -->
  <Type ID="21" Name="累计登录"  PeiZhi="HeFuDengLu.xml"/>     <!-- Tích lũy đăng nhập -->
  <Type ID="22" Name="商店限购"  PeiZhi="HeFuXianGou.xml"/>    <!-- Shop giới hạn mua -->
  <Type ID="23" Name="充值返利"  PeiZhi="HeFuFanLi.xml"/>      <!-- Nạp hoàn tiền -->
  <Type ID="24" Name="战场之神"  PeiZhi="PKJiangLi.xml"/>      <!-- Vua PK -->
  <Type ID="25" Name="为战而生"  PeiZhi="HeFuZhanChang.xml"/>  <!-- Vì chiến mà sống -->
  <Type ID="26" Name="BOSS之战"  PeiZhi="HeFuBOSS.xml"/>      <!-- Boss chiến -->
  <Type ID="27" Name="罗兰争霸"  PeiZhi="HeFuLuoLan.xml"/>    <!-- Luo Lan tranh bá -->
</Config>
```

---

## 4. JieRiDengLu.xml — Schema Phần thưởng Đăng nhập Lễ hội

**Path**: `GameRes/GameRes/Config/JieRiGifts/JieRiDengLu.xml`

```xml
<Config>
  <!-- Header: window thời gian activity -->
  <Activities
    ActivityType="10"
    FromDate="2019-12-09 00:00:00"
    ToDate="2019-12-16 23:59:59"
    AwardStartDate="2019-12-09 00:00:00"
    AwardEndDate="2019-12-16 23:59:59"
  />

  <!-- Danh sách phần thưởng theo ngày đăng nhập -->
  <GiftList Description="活动期间，累计登录达到相应天数即可免费领取超值好礼！">
    <Award
      TimeOl="1"       <!-- Ngày thứ N đăng nhập (1-7) -->
      GoodsOne="63262,50,1,0,0,0,0|9990001,200,1,0,0,0,0|64022,10000,1,0,0,0,0|63129,1,1,0,0,0,0"
      GoodsTwo=""      <!-- Optional: slot 2 -->
      GoodsThr=""      <!-- Optional: slot 3 -->
      EffectiveTime="" <!-- Optional: hiệu lực riêng -->
    />
    <!-- ... 7 Award records ... -->
  </GiftList>
</Config>
```

### GoodsOne format (pipe-separated, comma-separated per item):
```
GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo
63262,   50,     1,       0,           0,             0,     0
```
→ **7 fields** khớp với `GoodsData` struct trong code

---

## 5. EventTypes.cs — Tất cả Event Type cho dispatcher

**Path**: `GameServer/Core/GameEvent/EventTypes.cs`  
**Sử dụng**: `EventSource.FireEvent(EventTypes.xxx, ...)`

| Enum Value | Index | Mô tả |
|-----------|-------|-------|
| ZhanMengShiJian | 0 | Chiến minh sự kiện |
| XueSeChengBao | 1 | Thành bảo máu |
| EMoGuangChang | 2 | Quảng trường ác ma |
| HuangJinBuDui | 3 | Đội vàng |
| YeWaiBoss | 4 | Boss ngoại |
| PlayerLevelup | 9 | Level up |
| PlayerDead | 10 | Chết |
| MonsterDead | 11 | Quái chết |
| PlayerLogout | 12 | Đăng xuất |
| PlayerInitGame | 14 | Khởi tạo game |
| MonsterInjured | 17 | Quái bị thương |
| SevenDayGoal | 32 | 7-day goal check |
| **OnClientChargeItem** | **36** | **Nạp tiền! (EverydayActivity EventType 36)** |
| OrnamentGoal | 37 | Ornament goal |
| PlayerOnline | 38 | Online |
| GameRunning | 39 | Game chạy (timer?) |
| RoleKillMonster | 55 | Giết quái |
| OneSecsTimerEvent | 56 | Timer 1 giây |
| Max | 10000 | Max |

> **Xác nhận hardcode EventType 36 = `OnClientChargeItem`** — đây là charge event trong EverydayActivity.

---

## 6. ActivityManagerNew.cs — Pattern mới

Kế thừa `SingletonTemplate<ActivityManagerNew>` (Singleton pattern).

**TCP Handler pattern**:
```csharp
public TCPProcessCmdResults HandleXxx(TCPManager tcpMgr, TMSKSocket socket, 
    TCPClientPool pool, TCPRandKey randKey, TCPOutPacketPool outPool,
    int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
{
    // 1. Parse string array từ UTF8 data
    string[] array = text.Split(':');

    // 2. Validate array.Length

    // 3. Find GameClient by socket
    GameClient client = GameManager.ClientMgr.FindClient(socket);

    // 4. Get activity từ HuodongCachingMgr
    var activity = HuodongCachingMgr.GetXxxActivity();

    // 5. CheckCondition + HasEnoughBagSpace + GiveAward

    // 6. Return "{result}:{roleId}:{param}" format
    tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
    return TCPProcessCmdResults.RESULT_DATA;
}
```

**Error codes chuẩn hóa**:
| Code | Ý nghĩa |
|------|---------|
| -1 | Activity không tồn tại |
| -2 | Không trong thời gian award |
| -3 | Túi đồ không đủ chỗ |
| -7 | GiveAward thất bại |
| -10007 | CheckCondition thất bại |
| 1 | Thành công |

---

## 7. ActivityNew folder — 68 files phân loại

| Nhóm | Files | ActivityType range |
|------|-------|-------------------|
| JieRi (Lễ hội) | JieRiXxx*.cs (15+) | 9-17, 50-77 |
| HeFu (Hợp phục) | HeFuXxx*.cs | 20-27 |
| Theme | ThemeXxx*.cs | 150-157 |
| Special | SpecPriorityActivity.cs (71KB), SpecialActivity.cs (41KB) | 44, 49 |
| EveryDay | EverydayActivity.cs (48KB) | 46 |
| Weekend | WeedEndInputActivity.cs | - |
| OneDollar | OneDollarXxx*.cs | 45, 46 |
| Regress | (trong Logic/) UserRegressActiveManager.cs | 110-114 |

---

## 8. XML Schema Summary — tất cả các pattern

### Pattern 1: Type Mapping file
```xml
<Config>
  <Type ID="{ActivityTypes_int}" Name="{display}" PeiZhi="{datafile}.xml"/>
</Config>
```
Files: JieRiType.xml, HeFuType.xml, MuJieRiType.xml

### Pattern 2: Activity với GiftList
```xml
<Config>
  <Activities ActivityType="{n}" FromDate="{dt}" ToDate="{dt}"
              AwardStartDate="{dt}" AwardEndDate="{dt}" />
  <GiftList Description="{text}">
    <Award TimeOl="{day}" GoodsOne="{goods_pipe_str}" GoodsTwo="" GoodsThr="" EffectiveTime=""/>
  </GiftList>
</Config>
```
Files: JieRiDengLu.xml, JieRiLeiJi.xml, HeFuDengLu.xml, ...

### Pattern 3: DailyActive Tab
```xml
<Config>
  <DailyActive>
    <Tab ID="{ui}" DailyActiveID="{id}" Name="{name}"
         MinZhuanshengleve="{n}" Minleve="{n}"
         Login="{n}" Online="{n}" Consumption="{n}" ...
         Award="{pts}"/>
  </DailyActive>
</Config>
```

### Pattern 4: Recharge milestone
```xml
<Config>
  <Activities ActivityType="{n}" FromDate="{dt}" ToDate="{dt}"/>
  <AwardItems>
    <Award Condition="{min_amount}" GoodsOne="{goods}" ... />
  </AwardItems>
</Config>
```

### GoodsData format chuẩn (7 fields):
```
{GoodsID},{GCount},{Binding},{Forge_level},{AppendPropLev},{Lucky},{ExcellenceInfo}
```
Multiple items dùng `|` separator.

---

## 9. Checklist tiếp theo

- [ ] Đọc `EverydayActivity.cs` (48KB) — hệ thống EverydayActivity đầy đủ
- [ ] Đọc `SpecPriorityActivity.cs` (71KB) — SpecialActivity lớn nhất
- [ ] Đọc `GoodsPackManager.cs` (119KB) — reward distribution
- [ ] Đọc `GiftCodeNewManager.cs` — 214KB gift code XML
- [ ] Đọc `SystemParams.xml` sample — 147KB hệ thống params
- [ ] Phân tích `GameServer/Logic/GlobalServiceManager.cs` — scheduler chính
