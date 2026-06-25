# SpecialActivity.xml Schema

## Activity Attributes
| Thuộc tính | Kiểu | Mô tả |
|-----------|-----|------|
| ID | int | Unique ID (6763+) |
| GroupID | string | Nhóm event (20180227 - date format) |
| Name | string | Tên hoạt động |
| Day | "1,1\|2,2\|3,3" | Ngày trong tuần |
| Type | int | 14=ZhiGou (mua trực tiếp), 2=ChongZhiDuiHuan |
| Price | "Cost\|Currency\|Param" | Ví dụ: "1\|11\|7812" = 1YuanBao/11Tianfu/... |

## Điều kiện (Need*)
Format: "min,max\|min,max" hoặc "-1" (không yêu cầu)

| Trường | Loại | Mô tả | Ví dụ |
|-------|------|------|------|
| NeedWing | int | Cấp độ cánh | "1,0\|7,10" = cánh 1-7 hoặc 10+ |
| NeedMerlin | int | Cấp Merlin | "1,0\|11,10" |
| NeedShengWu | int | Cấp thần pháp | "1\|130" |
| NeedRing | int | Cấp hôn lễ | "1,0\|5,10" |
| NeedShouHuSheng | int | Cấp thủy thần | "1,0\|5,10" |
| NeedLevel | int | Cấp độ nhân vật | "2,1\|7,100" |
| NeedVIP | int | Cấp VIP | "0\|6" |
| NeedChengJiu | int | Cấp thành tựu | "1\|7" |

## Type Mapping
| Type | Tên | Quy tắc |
|------|-----|--------|
| 2 | ChongZhiDuiHuan | Nạp đổi |
| 14 | ZhiGou | Mua trực tiếp |

## Ví dụ Activity
```xml
<Activity ID="6763" GroupID="20180227" Name="冬日特惠" Day="1,1"
    NeedLevel="-1" NeedVIP="-1" NeedChongZhi="-1" NeedWing="-1" 
    Type="14" GoodsOne="63220,1,1,0,0,0,0|2031,88,1,0,0,0,0"
    Price="1|11|7812" PurchaseNum="5" />
```

## Time Config (SpecialActivityTime.xml)
- `FromDate/ToDate`: Thời gian mở đóng cửa
- Không có trong file, dùng JSON sidecar hoặc SystemParams