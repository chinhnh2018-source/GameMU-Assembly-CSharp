# Hệ thống Lễ hội (Festival) & Hội nhập server (HeFu)

## Festival Files (28 files)
| File | Mô tả |
|------|------|
| JieRiBOSS.xml | Boss event |
| JieRiChongZhiFanLi.xml | Nạp thưởng |
| JieRiChongZhiHongBao.xml | Red package |
| JieRiChongZhiKing.xml | Nạp xuất sắc |
| JieRiDayChongZhi.xml | Nạp ngày |
| JieRiDayXiaoFei.xml | Tiêu ngày |
| JieRiDengLu.xml | Đăng nhập |
| ...

## HeFu Files (6 files)
| File | Mô tả |
|------|------|
| HeFuBOSS.xml | Boss |
| HeFuFanLi.xml | Phản hồi |
| HeFuLiBao.xml | Gift |
| HeFuLuoLan.xml | LuoLan |
| HeFuDengLu.xml | Đăng nhập |
| HeFuZhangChang.xml | Thời gian dài |

## Quy luật xử lý
```
FromDate/ToDate → Kiểm tra thời gian
    ↓
Nếu to.Year <= 2001 → Tắt (sentinel)
Nếu now < from → Chưa bắt đầu
Nếu now > to → Hết hạn
Nếu trong khoảng → Đang chạy
```

## Ví dụ LeiJiChongZhi.xml
```xml
<Award ID="1" MinYuanBao="4000000" GoodsOne="2100000,18,1,0,0,0,0|..." />
```

## Toggle Strategy
- `ToggleStrategy.DateWindow`: Bật/tắt bằng cách lưu trạng thái cửa sổ trong file JSON sidecar
- Khi tắt: `FromDate="2000-01-01 00:00:00"` (sentinel)
- Khi bật: Khôi phục ngày/giờ trước hoặc mặc định hôm nay