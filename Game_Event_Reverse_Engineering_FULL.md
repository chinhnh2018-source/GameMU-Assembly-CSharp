
# Prompt phân tích project game C# và xây dựng hệ thống quản trị sự kiện

Bạn là Senior Software Architect, Game Backend Engineer và Reverse Engineer.

Tôi có source code game C# đã hoàn thiện (có thể là code cũ, code được dịch ngược từ dnSpy). Trong project có nhiều file XML cấu hình.

## Mục tiêu
Phân tích toàn bộ project để khôi phục tài liệu nghiệp vụ và xây dựng một mô hình quản trị dữ liệu có thể CRUD được.

# 1. Phân tích các hệ thống sự kiện
- Sự kiện hàng ngày
- Sự kiện hàng tuần
- Sự kiện tháng
- Sự kiện lễ hội
- Sự kiện nạp tiền
- Shop
- Khuyến mãi

# 2. Phân tích luồng hoạt động
Tìm:
- Entry Point
- Packet
- HTTP API
- Timer
- Scheduler
- Business Logic
- Call Graph

Ví dụ:

Packet
↓
Handler
↓
Service
↓
Manager
↓
XML Loader
↓
Reward
↓
Inventory
↓
Save DB

# 3. Mapping XML

Lập bảng:

| XML | Loader | Class sử dụng | Chức năng |
|------|--------|--------|--------|

Ví dụ:
DailyEvent.xml
WeeklyEvent.xml
RechargeReward.xml
Shop.xml
Festival.xml
VIP.xml

# 4. Reverse Schema XML

Ví dụ:

<Event>
    <ID>
    <Name>
    <StartTime>
    <EndTime>
    <Condition>
    <Reward>
</Event>

Giải thích:
- Required
- Nullable
- Default value

# 5. Phân tích thời gian

Tìm:
- DateTime.Now
- Timer
- Task.Delay
- Thread.Sleep
- Scheduler

Xác định:
- reset ngày
- reset tuần
- reset tháng
- giờ mở boss
- giờ đóng event

# 6. Phân tích Item

TemplateID
ItemID
RewardID
GoodsID
CurrencyID

Sơ đồ:

Event
↓
RewardID
↓
ItemTemplate.xml
↓
Inventory

# 7. Phân tích Shop

Currency:
- Gold
- Bind Gold
- Coin
- Cash

Điều kiện:
- VIP
- Level
- Job

Giới hạn:
- Daily
- Weekly
- Lifetime

# 8. Phân tích nạp tiền

Payment
↓
Verify
↓
RechargeRecord
↓
Reward
↓
Mail
↓
Inventory
↓
Save DB

# 9. Sinh tài liệu nghiệp vụ

Mô tả:
- Chức năng
- Quy tắc
- Điều kiện
- Reward
- Packet
- XML
- Database
- Manager/Class
- Sequence Diagram
- Flow Chart
- State Machine

# 10. Thiết kế Data Model CRUD

DailyEventConfig
WeeklyEventConfig
FestivalConfig
RechargeRewardConfig
ShopItemConfig
GiftCodeConfig

Tách:
- DTO
- Entity
- Repository
- Service
- Manager
- Validator

# 11. Sinh công cụ Admin CRUD

Web Admin ASP.NET Core + Vue
hoặc WinForms/WPF

Chức năng:
- Add
- Edit
- Delete
- Clone
- Enable/Disable
- Import XML
- Export XML

# 12. Sinh sơ đồ tổng thể

Player
↓
Packet
↓
Handler
↓
Manager
↓
XML Config
↓
Reward
↓
Inventory
↓
DB

Sinh:
- Class Diagram
- Sequence Diagram
- ERD
- Dependency Graph
- Call Graph
- Event Timeline

# 13. Báo cáo cuối cùng

/docs

01_EventSystem.md
02_DailyEvent.md
03_WeeklyEvent.md
04_Festival.md
05_Recharge.md
06_Shop.md
07_GiftCode.md
08_XMLMapping.md
09_Database.md
10_ClassDiagram.md
11_CallGraph.md
12_AdminCRUDDesign.md
13_MigrationPlan.md

# Yêu cầu quan trọng

- Không đoán mò.
- Chỉ kết luận khi tìm thấy code thực tế.
- Ghi mức độ tin cậy High/Medium/Low.
- Liệt kê file cần đọc tiếp.
- Sinh cây phụ thuộc giữa:
  - Class
  - Packet
  - XML
  - Database
  - Reward
  - Timer
  - Scheduler
- Phát hiện hardcode.
- Đề xuất EventService + Repository + ConfigProvider.
- Thiết kế Event Center thống nhất.
- Đưa ra kế hoạch migrate từng bước.
