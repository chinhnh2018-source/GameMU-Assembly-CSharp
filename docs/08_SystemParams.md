# Hệ thống tham số SystemParams.xml

## Daily Active Params
| Param | Value | Mô tả |
|-------|-------|------|
| `VIPHuoYueAdd` | `0,1,2,2,3,3,5,5,7,7,10,10,15,15,15,15` | Bonus điểm VIP (index 0-15) |
| `EveryDayActivityOpen` | `1,1\|2,1\|3,1\|4,1` | Server 1-4 mở hoạt động hằng ngày |
| `VIPRiChangYiJianWanCheng` | `3` | Số hoạt động có thể hoàn thành nhanh |

## Recharge Params
| Param | Value | Mô tả |
|-------|-------|------|
| `ChongZhiSecondTaskID` | `20005` | Task ID phụ |
| `EveryDayChongZhiDuiHuan` | `20000:1` | 20000 YuanBao đổi 1 lần |
| `JieRiChongZhiQiangGou` | `1,1\|2,1\|3,1\|4,1` | Giới hạn mua lễ hội |
| `ZhouMoChongZhiTime` | `6,00:00:00\|7,23:59:59` | Thời gian nạp cuối tuần |

## Festival Params
| Param | Value | Mô tả |
|-------|-------|------|
| `SuperChongZhiFanLi` | `2016-12-09 00:00:00\|2016-12-15 23:59:59` | Thời gian Super nạp |
| `SuperChongZhiFanLiOpen` | `1,1\|2,0\|3,0\|4,0` | Server mở Super FanLi |
| `ThemeActivity*Goods` | `51304,51305,51306` | Items lễ hội |

## Parser Format
```csharp
// Comma separated values
GetParamValueIntArrayByName("VIPHuoYueAdd", ',')

// Pipe and comma: "1,1|2,0|3,0|4,0"  
// Format: ServerID,OpenFlag|ServerID,OpenFlag...
```

## Validation Rules
1. Server check: `EveryDayActivityOpen` chứa ServerID hiện tại
2. Time check: `SuperChongZhiFanLi` so với `NowDateTime()`
3. VIP check: `VIPHuoYueAdd[vipLevel]` để tính điểm bonus