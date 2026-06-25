# Hệ thống Shop & Traity cửa hàng

## Mall.xml Schema
| Thuộc tính | Kiểu | Mô tả |
|-----------|-----|------|
| ID | int | ID sản phẩm (Unique) |
| GoodsID | int | Item template ID |
| TabID | int | 10000=ZhenQi, 20000=Premium, 30000=Special, 40000=Luxury, 50000=Currency |
| Price | int | Giá tiền |
| OrigPrice | int | Giá gốc |
| ZhenQi | int | Điểm hoạt động |
| SinglePurchase | int | Số lần mua/-1=vô hạn |
| PubStartTime/EndTime | datetime | Thời gian bán |

## Currency Mapping
| Loại | Mô tả |
|------|------|
| 1010, 1110 | Gold (vàng) |
| 2000-2005 | Bind Gold (vàng khấu trừ) |
| 50010-50013 | Coin (xu) |
| 50020 | Diamond (kim cương) |

## Điều kiện mua hàng
- `TabID`: Phân loại shop
- `SinglePurchase`: Giới hạn mua (GoalType 1 = đổi cấp)
- `Price`: `"300|600"` = ZhenQi|YuanBao

## Hạn mức
- Daily: Reset theo `DayID = DayOfYear`
- Weekly: Reset theo `Weekday` + `DayOfYear`
- Lifetime: Không reset, dựa vào DB key `HasBought_*`