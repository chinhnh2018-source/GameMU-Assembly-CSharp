# Kế hoạch Migration Event System

## Phase 1: Phân tích schema (Hoàn thành - 100%)

### Hoàn thành:
- [x] DailyActiveManager.cs - 808 lines - flow xử lý ngày
- [x] EveryDayActivity.xml (110+ bản ghi)
- [x] EveryDayActivityType.xml - GoalType mapping
- [x] EveryDayActivityGroup.xml - GroupID mapping
- [x] EventCalendar.xml - 70 sự kiện lịch
- [x] SystemParams.xml - ~1870 tham số
- [x] Mall.xml - Shop config
- [x] Goods.xml - Item master (~15MB)
- [x] DayChongZhi.xml, LeiJiChongZhi.xml - Nạp config
- [x] KF.Remoting services - Remoting entry point
- [x] GameMU.Manager - Web admin CRUD

## Phase 2: Thiết kế Event Center (Kế hoạch)

### EventService + Repository + ConfigProvider:
```csharp
// Tách riêng từng loại event:
interface IEventService<T>
{
    Task<T> GetConfig(string id);
    Task<IEnumerable<T>> ListConfigs();
    Task<T> UpdateConfig(string id, T config);
    Task<bool> IsActive(string id);
    Task<IEnumerable<string>> GetActiveIds();
}

// Repository chuẩn:
interface IEventRepository<T>
{
    Task<T?> FindById(string id);
    Task<IEnumerable<T>> FindAll();
    Task Save(T entity);
    Task Delete(string id);
}
```

### ConfigProvider:
- Cache theo mtime
- Forward-link resolve (GoodsID → Goods.Name)
- Back-reference (Goods.JinJie → Goods)
- Validation: Required fields, Date format

### Scheduler tích hợp:
- Timer check: `DateTime.Now` so với `FromDate/ToDate`
- Push notification tới client khi event state thay đổi

## Phase 3: Web CRUD (Kế hoạch)

### APIs cần triển khai:
| Endpoint | Method | Chức năng |
|----------|--------|----------|
| /api/daily-active | GET | List hoạt động |
| /api/daily-active/{id} | PUT | Cập nhật |
| /api/event-calendar | GET | Lịch sự kiện |
| /api/mall | GET/PUT/DELETE | Shop |
| /api/recharge-daily | GET/PUT | Nạp ngày |
| /api/recharge-total | GET/PUT | Nạp tổng |
| /api/festival/{name} | GET/PUT | Lễ hội |

## Phase 4: Validate & Test (Kế hoạch)

- So sánh dữ liệu cũ (XML) với mới (DB)
- Kiểm thử luồng: Add/Edit/Delete → Reload → Validate client
- Performance test: Goods.xml cache (15MB)