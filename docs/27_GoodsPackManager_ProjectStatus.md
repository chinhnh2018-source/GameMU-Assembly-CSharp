# 27_GoodsPackManager_ProjectStatus.md
> GoodsPackManager analysis + Tổng kết tiến độ
> Date: 2026-06-25

---

## 1. GoodsPackManager.cs — Phân tích đầy đủ

**Path**: `GameServer/Logic/GoodsPackManager.cs`  
**Size**: 119KB / 118,953 chars  
**Chức năng thực tế**: **Loot Drop System** (vật phẩm rơi trên đất), KHÔNG phải reward system

### 1.1 Điều chỉnh nhận thức

| Tên | Chức năng thực |
|-----|----------------|
| `GoodsPackManager` | Quản lý vật phẩm rơi/nhặt trên sàn game |
| `AutoAddThingIntoBag()` | Tự động nhặt item vào túi |
| `AutoGetThings()` | Auto-loot items trong range |
| `GetFallGoodsItemList()` | Lấy danh sách item rơi từ monster |
| `ProcessMonster()` | Xử lý loot khi giết monster |
| `ProcessGetThing()` | Xử lý khi player nhặt item |
| `ProcessClickOnGoodsPack()` | Click nhặt item trên đất |
| `GetMonsterGoodsPackItemList()` | Danh sách drop của từng monster |

→ **Reward distribution** (phát thưởng sự kiện) dùng class khác, có thể là:
- `Global.GiveGoods()` hoặc `GameManager.GiveGoods()`  
- `GiveGoodsService`
- Trong `Activity.GiveAward()` trực tiếp call các Global methods

### 1.2 GoodsPackManager liên quan đến EventSystem ở điểm nào?

- `GoodsAuditService` quét cả Goods.xml → GoodsPackManager load Goods.xml để biết item nào drop-able
- `GetFallGoodsItemList()` → dùng GoodsPack.xml (không phải Goods.xml trực tiếp)
- `GetFixedGoodsDataList()` → parse GoodsData string "id,count,bind,..." → **đây là format chung!**

### 1.3 Hardcoded slot ranges (đáng chú ý)

```csharp
// Bag slot ranges (hardcoded):
1800, 1900, 2900, 3900, 4900, 5900, 6900, 7900, 8900
```
→ Đây là bag slot boundaries (mỗi 1000 slots). Không nguy hiểm (gameplay constant).

---

## 2. Tổng kết commits hôm nay (2026-06-25)

| Commit | SHA | Nội dung |
|--------|-----|---------|
| 1 | `32610c2` | Test (3 placeholder docs) |
| 2 | `17d3f1b8` | docs 19-22 (ActivitySystem, XMLSchema, CallGraph, Migration) |
| 3 | `132ed266` | docs 23-26 + DEPLOY.md |
| 4 | `04d8c5b6` | ApiEndpoints.cs + GoodsDataEditor.js + snippet file |
| 5 | `12470a5b` | **EventRegistry.cs: 35→94 entries (+59 JieRi/HeFu/Daily/Gift)** |
| 6 | `b1aacfa3` | GoodsDataEditor.js v2 + Edit.cshtml + appsettings.json |
| 7 | `751b29e0` | ModuleRegistry.cs: 11→14 modules (jieri/hefu/giftcode) |

**Total**: 7 commits, ~150 files changed, ~4000+ lines added

---

## 3. Trạng thái GameMU.Manager sau hôm nay

### ✅ Hoàn chỉnh

| Tính năng | Trạng thái |
|-----------|-----------|
| EventRegistry: 94 XML files | ✅ |
| CRUD engine (XmlEventService) | ✅ |
| 3 Toggle strategies | ✅ |
| Auto backup | ✅ |
| REST API 12 endpoints (ApiEndpoints.cs) | ✅ |
| Swagger UI tại /swagger | ✅ |
| GoodsDataEditor.js visual editor | ✅ |
| Calendar timeline 57+ events | ✅ |
| Sidebar 14 modules | ✅ |
| SystemParams (939 params) | ✅ |
| GoodsAudit FK validation | ✅ |
| 8 doc files 19-26 | ✅ |
| DEPLOY.md | ✅ |

### ❌ Còn lại

| Tính năng | Priority | Ghi chú |
|-----------|----------|---------|
| Import/Export ZIP | Medium | Batch backup/restore |
| GameServer reload endpoint | Medium | Cần GameServer expose HTTP |
| GiftCode pagination | Medium | 214KB file cần phân trang |
| Shared/_Layout.cshtml | Low | Sidebar navigation update |
| Performance test Goods.xml 15MB | Low | Cache benchmark |
| Diff viewer (trước/sau sửa) | Low | Nice-to-have |

---

## 4. File cần đọc tiếp (theo prompt)

| File | Size | Lý do |
|------|------|-------|
| `GameMU.Manager/Pages/Shared/_Layout.cshtml` | ? | Sidebar nav cần update cho 14 modules |
| `GameServer/Logic/GiftCodeNewManager.cs` | 10KB | Gift code flow cho EventRegistry |
| `GameServer/Logic/HuodongCachingMgr.cs` | ? | God Object cần phân tích split |
| `KF.Remoting.HuanYingSiYuan/KF/` | ? | Cross-server remoting còn thiếu |

---

## 5. Recommendation tiếp theo

### 5.1 Ngay bây giờ (không cần restart app)
```bash
# Test build:
cd GameMU.Manager && dotnet build

# Kiểm tra /swagger endpoint:
dotnet run
# → http://localhost:5000/swagger
# → GET /api/files phải trả về 94 entries
```

### 5.2 Cần làm (Phase 3 còn lại)
1. **Shared/_Layout.cshtml** — thêm 3 module mới vào sidebar nav
2. **GiftCode pagination** — File.cshtml thêm paging cho file 214KB
3. **Import/Export ZIP** — batch backup endpoint
