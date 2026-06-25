# TCPCmdHandler — Packet ID Mapping (Clarification)

> Nguồn: `GameServer/Server/TCPCmdHandler.cs` (44,104 dòng)
> Độ tin cậy: **HIGH**

---

## ⚠️ Đính chính quan trọng

Trong `docs/11_CallGraph.md` có ghi "LinkID 301/302/303: Activity windows hardcode".

Sau khi đọc code thực tế: **Packet ID 301/302/303 trong TCPCmdHandler là BangHui (Guild) commands**, không liên quan EventCalendar LinkID.

---

## Packet ID range 297–355 trong TCPCmdHandler (BangHui module)

| Packet ID | Handler | Chức năng |
|-----------|---------|----------|
| 297 | ProcessQueryBangHuiDetailCmd | Query chi tiết bang hội |
| 298 | ProcessUpdateBangHuiBulletinCmd | Cập nhật thông báo bang |
| 299 | ProcessGetBHMemberDataListCmd2 | Lấy danh sách thành viên |
| 300 | ProcessUpdateBHVerifyCmd | Cập nhật xác nhận bang |
| **301** | **ProcessApplyToBHMemberCmd** | **Xin vào bang hội** |
| **302** | **ProcessAddBHMemberCmd** | **Thêm thành viên bang** |
| **303** | **ProcessRemoveBHMemberCmd** | **Xóa thành viên bang** |
| 304 | ProcessQuitFromBangHuiCmd | Rời bang |
| 305 | ProcessDestroyBangHuiCmd | Giải tán bang |
| 306 | ProcessBangHuiVerifyCmd | Xác nhận bang |
| 308 | ProcessChgBHMemberZhiWuCmd | Đổi chức vụ thành viên |
| 309 | ProcessChgBHMemberChengHaoCmd | Đổi danh hiệu thành viên |

---

## EventCalendar LinkID — Thực tế trong EventCalendar.xml

`LinkID` trong `EventCalendar.xml` là mã **điều hướng nội bộ** trong client-side (Unity), không phải Packet ID trong TCPCmdHandler. LinkID được đọc và xử lý bởi client để mở đúng giao diện.

```xml
<!-- Ví dụ EventCalendar.xml -->
<EventCalendar ID="1" Weekday="1" Level="1,50" Time="00:00-00:02|..."
    LinkID="301" EventAward="8002,8014" />
```

→ `LinkID=301` ở đây là mã giao diện phía client, không phải Packet 301.

---

## Packet 558 — DailyActive (Xác nhận)

```csharp
// DailyActiveManager.cs:233
GameManager.ClientMgr.SendToClient(client, buffer, 558);
```

→ Packet 558 gửi `DailyActiveData` struct tới client. **Đã xác nhận HIGH confidence**.

---

## Key Packet IDs liên quan Event

| Packet ID | Chức năng | Source |
|-----------|----------|--------|
| 558 | DailyActiveData → client | DailyActiveManager.cs:233 |
| 1507 | FetchEverydayActAward response | EverydayActivity.cs:38 |
| 36 (EventType) | ChargeItemBaseEvent | EverydayActivity.cs:18 |
| 13173 (DB Cmd) | Everyday recharge exchange | EverydayActivity.cs:69 |
