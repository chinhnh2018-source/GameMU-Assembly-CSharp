# Hệ thống 7 ngày (SevenDay) Event

## Files
| File | Records | Mô tả |
|------|---------|------|
| SevenDayLogin.xml | 7 awards | Đăng nhập |
| SevenDayChongZhi.xml | ? | Nạp tiền |
| SevenDayGoal.xml | ? | Goal chung |
| SevenDayQiangGou.xml | ? | Mua nhanh |

## Schema SevenDayLogin.xml
```xml
<Award ID="1-7" ActivityTypeID="1" GoodsOne="..." GoodsTwo="" GoodsThr="" EffectiveTime="" />
```

## Mapping SevenDay
| AwardID | Gift |
|--------|------|
| 1 | 50160+5050 |
| 2 | 50160+2016 |
| 3 | 50160+50017 |
| 4 | 50160+2001 |
| 5 | 50160+2003 |
| 6 | 50160+2017 |
| 7 | 50160+1030903 (liên hệ ExcellenceInfo) |

## Time Config
- `FromDate/ToDate`: Thời gian mở đóng cửa event
- `EffectiveTime`: Thời gian item có hiệu lực (dùng cho time-limited item)

## DB Key
- `SevenDayFlag`: Bitmask 7 ngày (cũng dùng 2-bit như DailyActive)