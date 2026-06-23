# 04 — Giải nghĩa chi tiết các file sự kiện quan trọng

## 4.1. EventCalendar.xml — Lịch sự kiện
Ví dụ:
```xml
<EventCalendar ID="1" Weekday="1" Level="1,50" CompletedTaskID="-1" VipLevel="-1"
  Time="00:00-00:02|00:30-00:32|..." LinkID="301" EventName="血色城堡" EventAward="8002,8014,8011" />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `ID` | Mã dòng lịch |
| `Weekday` | Thứ áp dụng (1–7); thường dùng dạng mặt nạ/nhiều thứ |
| `Level` | Khoảng cấp `min,max` được tham gia |
| `CompletedTaskID` | Task phải hoàn thành mới hiện (`-1` = không yêu cầu) |
| `VipLevel` | Cấp VIP yêu cầu (`-1` = không) |
| `Time` | Danh sách khung giờ chạy, ngăn bằng `|`, mỗi khung `HH:mm-HH:mm` |
| `LinkID` | ID liên kết tới hoạt động/cổng vào tương ứng |
| `EventName` | Tên sự kiện hiển thị |
| `EventAward` | Danh sách ID phần thưởng (mô tả/biểu tượng) |

## 4.2. SpecialActivity.xml + SpecialActivityTime.xml — Hoạt động đặc biệt
`<Activity>`:
```xml
<Activity ID="6763" GroupID="20180227" Name="冬日特惠" Day="1,1" NeedLevel="-1" NeedVIP="-1"
  ... Type="14" Goal="-1" GoodsOne="63220,1,1,0,0,0,0|2031,88,1,0,0,0,0" GoodsTwo="" GoodsThr=""
  EffectiveTime="" Price="1|11|7812" PurchaseNum="5" />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `ID` | Mã hoạt động |
| `GroupID` | Mã nhóm/đợt (gắn với khung thời gian ở file Time) |
| `Name` | Tên |
| `Day` | Ngày áp dụng trong đợt `from,to` |
| `Need*` | Điều kiện tham gia (Level/VIP/ChongZhi=nạp/Wing=cánh/ChengJiu=thành tựu/JunXian=quân hàm/Merlin/ShengWu/Ring/ShouHuShen). `-1` = bỏ qua |
| `Type` | Loại hoạt động (vd `14` = gói trực mua/直购) |
| `Goal` | Mục tiêu (nếu loại hoạt động cần) |
| `GoodsOne/Two/Thr` | Vật phẩm thưởng, định dạng mỗi món `id,sl,binding,?,?,?,?`, nhiều món ngăn bằng `|` |
| `Price` | `loại|giá|ZhiGouID` (ID gói nạp) |
| `PurchaseNum` | Số lần mua tối đa (`-1` = vô hạn) |

`<Time>` (file SpecialActivityTime.xml):
```xml
<Time GroupID="20161101" ServerOpenFromDate="-1" ServerOpenToDate="-1"
  FromDate="2016-11-01 00:00:00" ToDate="2016-11-03 23:59:59" />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `GroupID` | Khớp với `GroupID` của các `<Activity>` |
| `FromDate`/`ToDate` | Khung thời gian chạy theo lịch tuyệt đối |
| `ServerOpenFromDate`/`ServerOpenToDate` | Khung tính theo **ngày mở server** (`-1` = không dùng) |

> ⇒ Một "đợt sự kiện đặc biệt" = nhiều `<Activity>` cùng `GroupID` + một `<Time>` cùng `GroupID`.
> Bật/tắt cả đợt thực chất là chỉnh khung `<Time>`.

## 4.3. EveryDayActivity.xml — Hoạt động hằng ngày
```xml
<EveryDayActivity ActivityID="1000" Name="等级返利" GoalType="5" GoalNum="4,1"
  GoodsOne="50160,2,1,0,0,0,0|2030,20,1,0,0,0,0" GoodsTwo="" GoodsThr="" EffectiveTime="" Price="" PurchaseNum="" />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `ActivityID` | Mã hoạt động hằng ngày |
| `Name` | Tên (vd "等级返利" = Hoàn lại theo cấp) |
| `GoalType` | Loại mục tiêu (liên kết `EveryDayActivityType.TypeID`) |
| `GoalNum` | Tham số mục tiêu, dạng `a,b` |
| `GoodsOne/Two/Thr` | Phần thưởng |
| `Price`, `PurchaseNum` | Dùng cho mục cần mua (có thể rỗng) |

`EveryDayActivityType.xml`: `TypeID, Name, OpenLevel ("cap|?"), CloseLevel`. Ví dụ TypeID `1`="转生等级"
(cấp chuyển sinh), `10`="充值" (nạp).

## 4.4. VersionSystemOpen.xml — Mở hệ thống (có cờ)
```xml
<Version ID="..." SystemName="..." IsOpen="1" />
```
`IsOpen=1` mở chức năng, `0` đóng. Đây là cờ bật/tắt trực tiếp, an toàn nhất.

## 4.5. SystemOpen.xml — Mở chức năng theo điều kiện
```xml
<System Order="1" ID="102" Name="人物" TriggerCondition="1" TimeParameters="0,0"
  SpecialOpenType="0" NotOpenShow="2" ... />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `Order` | Thứ tự hiển thị |
| `ID` | Mã chức năng |
| `Name` | Tên chức năng |
| `TriggerCondition` | Điều kiện kích hoạt mở |
| `TimeParameters` | Tham số thời gian `a,b` |
| `NotOpenShow` | Cách hiển thị khi chưa mở |
| `SpecialOpenType` | Kiểu mở đặc biệt |

## 4.6. JieRiGifts/* — Quà lễ hội (khung ngày)
```xml
<Activities ActivityType="66" FromDate="2018-02-28 00:00:00" ToDate="2018-03-05 23:59:59"
  AwardStartDate="..." AwardEndDate="..." />
```
| Thuộc tính | Ý nghĩa |
|---|---|
| `ActivityType` | Mã loại lễ hội (tra trong `JieRiType.xml`/`MuJieRiType.xml`) |
| `FromDate`/`ToDate` | Thời gian diễn ra sự kiện |
| `AwardStartDate`/`AwardEndDate` | Thời gian được nhận thưởng |

## 4.7. HuiGuiHuoDong.xml — Hoạt động hồi quy
```xml
<HuiGuiHuoDong ID="..." HuoDongLevel="..." BeginTime="..." FinishTime="..."
  RegisterBegin="..." RegisterFinish="..." />
```
Điều khiển bằng `BeginTime`/`FinishTime`; `RegisterBegin`/`RegisterFinish` là cửa sổ đăng ký.

## 4.8. Định dạng chuỗi Goods (thưởng) — lưu ý chung
Chuỗi vật phẩm thường có dạng: `GoodsID,SoLuong,Binding,p1,p2,p3,p4`, nhiều món ngăn bằng `|`.
Khi sửa qua web app, giữ đúng số trường và dấu phân tách để server parse được.
