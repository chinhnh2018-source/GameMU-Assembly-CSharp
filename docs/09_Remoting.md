# Remoting Service Architecture

## HuanYingSiYuanService (Cross-server events)
- **GameType**: 1 (HuanYingSiYuan)
- **Background Thread**: CheckRoleTimerProc mỗi 1.428s
- **Timeout**: CheckGameFuBenTimerProc mỗi 1000s

### Collections
| Collection | Type | Mô tả |
|-----------|------|------|
| HuanYingSiYuanFuBenDataDict | ConcurrentDictionary<int, FuBenData> | 4096 slot |
| RoleIdKuaFuRoleDataDict | ConcurrentDictionary<RoleKey, RoleData> | Role state tracking |
| UserId2RoleIdActiveDict | ConcurrentDictionary<string, int> | User đang active |

### States
```csharp
enum KuaFuRoleStates {
    SignedUp = 1,
    Waiting = 2,
    EnterGame = 3,
    Completed = 4
}

enum GameFuBenState {
    Prepare = 0,
    Waiting = 1,
    Start = 2,
    End = 3
}
```

## Kết nối Remoting
- **Port**: `Config/KFSetting.xml` → `ServerPort`
- **Protocol**: TCP/IP remoting (Binary formatter)
- **DB**: MySQL (`t_giftcode`, `t_uselog`)

## EventRegistry (GameMU.Manager)
```csharp
// Danh mục file XML
new EventDef {
    SourceFile = "EveryDayActivity.xml",
    ListColumns = new[]{"ActivityID","Name","GoalType","GoalNum","GoodsOne"},
    PrimaryKey = "ActivityID",
    ToggleStrategy = ToggleStrategy.DateWindow
}

// Hardcode switches
case 301: ShowActivityWindow(600 + BloodCastle)
case 400/401: ShowZaDanWindow/Synthesis
```

## XmlEventService.cs
- `ReadFromFile(string fileName)` → `XElement` cache
- `WriteToFile(string fileName, XElement element)` 
- Mỗi file có sidecar `.json` lưu trạng thái toggle