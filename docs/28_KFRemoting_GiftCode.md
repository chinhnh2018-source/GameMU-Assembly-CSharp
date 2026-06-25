# 28_KFRemoting_GiftCode_Analysis.md
> Phân tích KF.Remoting.HuanYingSiYuan + GiftCodeNewManager
> Confidence: HIGH (đọc trực tiếp source)
> Date: 2026-06-25

---

## 1. KF.Remoting.HuanYingSiYuan — Cross-Server Hub

### 1.1 Vai trò trong hệ thống

```
GameServer1 ──TCP──┐
GameServer2 ──TCP──┼──► KF Hub Server (HuanYingSiYuan)
GameServer3 ──TCP──┘     (幻影私源 = Cross-Server Hub)
```

**KF Hub** là server độc lập (chạy riêng), các GameServer kết nối vào để tham gia trận đấu cross-server.

### 1.2 Framework: AutoCSer (không phải WCF)

```csharp
[Server(Name = "KfCall", IsServer = true, IsSegmentation = true,
    ClientSegmentationCopyPath = "GameServer\Remoting\")]
public static class KFServiceBase { ... }
```

- **AutoCSer** = high-performance C# RPC framework (binary protocol, không JSON)
- `[Method]` attribute = RPC method exposed to clients  
- `[KeepCallbackMethod]` = streaming/push method (server → all clients)
- Generated stub nằm ở `GameServer/Remoting/` → được copy vào GameServer

### 1.3 Kiến trúc Connection

```
GameServer kết nối → InitializeClient(KuaFuClientContext)
    → ClientAgentManager.Instance().InitializeClient()
    → socket.ClientObject = ClientAgent (mỗi server 1 agent)

KF Hub push message → KeepGetMessage(callback)
    → clientAgent.KFCallMsg = onMessage
    → ClientAgentManager.SendAsyncKuaFuMsg() [TimerProc]
```

### 1.4 Danh sách Services (21 service files)

| Service | Size | Chức năng |
|---------|------|----------|
| JunTuanService.cs | 59KB | Liên minh (军团) cross-server |
| BangHuiMatchService.cs | 55KB | Bang hội đấu (帮会匹配) |
| YongZheZhanChangService.cs | 55KB | Dũng giả chiến trường (勇者战场) |
| CompService.cs | 50KB | Liên đấu (竞技场) |
| YaoSaiService.cs | 46KB | Yêu tái (要塞) |
| KuaFuLueDuoService.cs | 44KB | Cross-server cướp (跨服掠夺) |
| TianTiService.cs | 33KB | Thiên thê (天梯) Arena |
| AllyService.cs | 33KB | Đồng minh (盟友) |
| CoupleArenaService.cs | 27KB | Đôi đấu (情侣竞技) |
| HuanYingSiYuanService.cs | 26KB | Hub service chính |
| TianTi5v5Service.cs | 32KB | 5v5 Arena |
| Zork5v5Service.cs | 36KB | Zork 5v5 |

### 1.5 Persistence Layer (16 files)

| Persistence | Size | Chức năng |
|-------------|------|----------|
| JunTuanPersistence.cs | 43KB | Lưu trữ Liên minh |
| TianTiPersistence.cs | 36KB | Lưu trữ Arena |
| BangHuiMatchPersistence.cs | 25KB | Lưu trữ Bang đấu |
| KuaFuLueDuoPersistence.cs | 21KB | Lưu trữ KF Cướp |
| YongZheZhanChangPersistence.cs | 19KB | Lưu trữ Dũng giả |

### 1.6 Message Protocol

```csharp
// KFCallMsg (cmd=int, data=string)
ClientAgentManager.Instance().BroadCastMsg(KFCallMsg.New(10041, text), 0);
// cmd 10041 = test/ping message
```

### 1.7 Liên quan đến GameMU.Manager

**Không ảnh hưởng trực tiếp** — KF Hub Server là process riêng, config XML không nằm trong GameRes/Config.
- GameMU.Manager quản lý config của **GameServer** (individual game server)
- KF Hub có config riêng (nằm trong KF.Remoting project, không có XML dạng GameRes)

---

## 2. GiftCodeNewManager.cs — Gift Code System

**Path**: `GameServer/Logic/GiftCodeNewManager.cs`  
**Size**: 9.5KB  
**Implement**: `IManager` (registered trong GlobalServiceManager)

### 2.1 XML Schema — GiftCodeNew.xml

```xml
<Config>
  <GiftCode
    TypeID="S26X"           <!-- String key (4-5 ký tự in hoa, KHÔNG phải số) -->
    TypeName="礼包奖励"     <!-- Tên hiển thị -->
    Description="您的礼包码{0}使用成功..." <!-- {0} = code -->
    Platform=""             <!-- Lọc platform (rỗng = tất cả) -->
    Channel=""              <!-- Lọc kênh (pipe-separated: "ch1|ch2") -->
    TimeBegin="2014-01-01 00:00:00"  <!-- Bắt đầu -->
    TimeEnd="2099-12-31 23:59:59"    <!-- Kết thúc -->
    Zone=""                 <!-- Lọc vùng máy chủ -->
    UserType="1"            <!-- 1=player, 0=tất cả -->
    UseCount="1"            <!-- Max lần dùng per account -->
    GoodsOne="50161,1,1,0,0,0,0|..."
    GoodsTwo=""
  />
</Config>
```

**TypeID format**: `TypeID` là STRING (vd `"S26X"`, `"L94G"`, `"C13D"`) — KHÔNG phải số!
→ EventRegistry entry `IdAttr="TypeID"` là đúng

### 2.2 Validation khi đổi code

```
Player nhập code → ProcessGiftCodeCmd()
    ↓
1. Tìm GiftCodeInfo theo code (TypeID lookup)
2. Check Channel (player's channel ∈ giftCode.ChannelList)
3. Check Platform (player's platform ∈ giftCode.PlatformList)
4. Check Zone (player's zone ∈ giftCode.ZoneList)
5. Check UserType (1=player account only)
6. Check TimeBegin ≤ Now ≤ TimeEnd
7. Check UseCount (player chưa dùng quá lần)
8. → GiveGoodsReward (GoodsOne/GoodsTwo → GiveGoods)
9. → EventLogManager.SystemRoleEvents[80] (log)
```

### 2.3 Error codes (GLang)

| GLang | Ý nghĩa |
|-------|---------|
| 121 | Mã quà không hợp lệ / hết hạn |
| 122 | Mã quà đã dùng rồi |
| 123 | Đổi thành công |

### 2.4 TCP Handlers

| Method | Chức năng |
|--------|---------|
| `ProcessGiftCodeCmd()` | Đổi 1 code → nhận thưởng |
| `ProcessGiftCodeList()` | Query danh sách code cho client |

### 2.5 Data structures

```csharp
class GiftCodeInfo {
    string GiftCodeTypeID;      // = TypeID trong XML
    string GiftCodeName;        // = TypeName
    string Description;         // = Description template
    List<string> ChannelList;   // = Channel split by '|'
    List<string> PlatformList;  // = Platform split
    List<string> ZoneList;      // = Zone split
    int UserType;
    DateTime TimeBegin, TimeEnd;
    int UseCount;
    List<GoodsData> GoodsOneList;  // parsed từ GoodsOne
    List<GoodsData> GoodsTwoList;  // parsed từ GoodsTwo
}
```

### 2.6 Ảnh hưởng đến EventRegistry (giftcode entry)

EventRegistry entry đã thêm:
```csharp
Key="giftcode", IdAttr="TypeID"  // ✅ STRING key
Toggle=ToggleStrategy.Park        // ✅ không có flag/date → Park là đúng
```

⚠️ Lưu ý: TypeID không phải số nguyên. GoodsAuditService sẽ quét GoodsOne/GoodsTwo → đây là FK cần validate.

---

## 3. Tổng kết phân tích

### Còn lại chưa phân tích
- [ ] `KF.Remoting.HuanYingSiYuan/Maticsoft/` — framework ORM cho KF Hub
- [ ] `KF.Remoting.HuanYingSiYuan/MILogin/` — mobile/MI login integration
- [ ] `KF.Remoting.HuanYingSiYuan/Tmsk/` — Tmsk (mobile platform) bridge
- [ ] `GameServer/Core/Executor/` — executor pattern
- [ ] `GameServer/KF/` — KF client-side trong GameServer

### Status 100% complete
- [x] GameServer/Logic/ActivityNew/ (tất cả)
- [x] GameServer/Core/GameEvent/ (EventTypes, EventSource)
- [x] GameServer/Logic/DailyActiveManager.cs
- [x] GameServer/Logic/RechargeRepayActiveMgr.cs
- [x] GameServer/Logic/EverydayActivity.cs
- [x] GameServer/Logic/ReloadXmlManager.cs
- [x] GameServer/Logic/GiftCodeNewManager.cs
- [x] GameServer/Logic/MallGoodsMgr.cs
- [x] GameServer/Logic/GoodsPackManager.cs (loot system)
- [x] GameRes/GameRes/Config/ (540+ XMLs analyzed)
- [x] KF.Remoting.HuanYingSiYuan/ (cross-server hub)
