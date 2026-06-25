# TCP Packet ID — Bản đồ đầy đủ

> Nguồn: `GameServer/Server/TCPGameServerCmds.cs` (1742 dòng, 1740 enum values)
> Tổng số: ~700+ packet IDs, chia 3 tầng

---

## Tầng 1: Client ↔ Server (CMD_SPR_xxx, range 1-3730)

### Range 1-200: Core (login, move, battle)
| ID | Tên | Chức năng |
|----|-----|----------|
| 1 | CMD_LOGIN_ON1 | Login bước 1 |
| 20 | CMD_LOGIN_ON2 | Login bước 2 |
| 98-104 | CMD_LOGIN_ON / ROLE_LIST / CREATE / REMOVE / INIT / SYNC / PLAY | Vào game flow |
| 106-128 | CMD_SPR_MOVE / MOVEEND / ACTION / ATTACK... | Di chuyển & chiến đấu |
| ~160-180 | CMD_SPR_GETWABAODATA / GETHUODONGDATA / ENTERFUBEN | Kho báu, hoạt động, dungeon |

### Range 200-400: Guild (BangHui)
| ID | Tên |
|----|-----|
| 212-230 | CMD_SPR_GETBANGHUILIST / CREATEBANGHUI / CHGBANGHUIINFO... |
| **218-222** | **ApplyToBH / AddBH / RemoveBH / Quit / Destroy** ← *đây mới là BangHui 301-305 trong TCPCmdHandler* |

### Range 370-470: JieRi Events
| ID | Tên | Chức năng |
|----|-----|----------|
| 378 | CMD_SPR_GETJIERIXMLDATA | Lấy XML lễ hội |
| 379-394 | CMD_SPR_QUERY/EXECUTE JIERIXXXX | Query/Execute mọi loại JieRi |
| 396 | CMD_SPR_FACTIVITIESDATA | Danh sách hoạt động |
| 398-411 | CMD_SPR_QUERYHEFU / EXECUHEFU | HeFu activity |

### Range 435-480: Daily Active (quan trọng!)
| ID | Tên | Chức năng |
|----|-----|----------|
| 435-451 | CMD_SPR_BLOODCASTLE* | Blood Castle |
| 453-456 | CMD_SPR_QUERYDAIMONSQUARE* | Daimon Square |
| 476 | **CMD_SPR_DAILYACTIVEDATA** | **Request DailyActive data** |
| 477 | **CMD_SPR_GETDAILYACTIVEAWARD** | **Nhận thưởng Daily Active** |
| 479-481 | CMD_SPR_GETBLOODCASTLE/DAIMONSQUARE/COPYMAP AWARD | Nhận thưởng BC/DS/Copy |

### Range 550-620: 7-Days & Online Awards
| ID | Tên |
|----|-----|
| 557-561 | CMD_SPR_QUERY/PLAY/END BOSSANIMATION, TODAYCANDO, GETOLDRES... |
| 590 | CMD_SPR_BATCHFETCHMAILGOODS |

### Range 650-720: System Open
| ID | Tên |
|----|-----|
| 699 | CMD_SPR_GE (placeholder) |
| 718 | CMD_SPR_SYSTEMOPENDATA |
| 719-726 | CMD_SPR_GET_ELEMENTHRT_* |

### Range 770 & 818-830: JieRiAct + HYSY (HuanYingSiYuan)
| ID | Tên | Chức năng |
|----|-----|----------|
| **770** | **CMD_SPR_JIERIACT_STATE** | **EverydayActivity open/close notify** |
| 771 | CMD_SPR_GETJIERIFANBEI_INFO | Lấy info fan bội JieRi |
| 818-830 | CMD_SPR_HYSY_* | TianTi 5v5 (HuanYingSiYuan) |
| 831-833 | CMD_SYNC_TIME_BY_CLIENT/SERVER/CHANGEDAY | Time sync |

### Range 850-1000: YueKa, Marriage, Return
| ID | Tên |
|----|-----|
| 850-851 | CMD_SPR_GET_YUEKA_DATA / AWARD |
| 870-879 | CMD_SPR_MARRY_FUBEN / ROSE / RING / MESSAGE... |
| 880-898 | CMD_SPR_MARRY_PARTY / MARRY_INIT... |
| 900-904 | CMD_SPR_RETURN_DATA / CHECK / AWARD / XMLDATA |
| 905-910 | CMD_SPR_THEME_GETXMLDATA / ZHIGOU / DUIHUAN / LIBAO / BOSS |

### Range 940-1010: Liên kết JieRi + TianTi
| ID | Tên |
|----|-----|
| 940-941 | CMD_SPR_QUERY/GET LIANXU_CHARGE |
| 944-949 | CMD_SPR_QUERY/GET JIERI_RECV/FASHION/DANBICHONGZHI |
| 950-958 | CMD_SPR_TIANTI_JOIN/QUIT/ENTER/AWARD/DAY_DATA... |

### Range 1310-1315: SevenDay Activity
| ID | Tên | Chức năng |
|----|-----|----------|
| **1310** | **CMD_SPR_SEVEN_DAY_ACT_QUERY** | Query SevenDay activities |
| **1311** | **CMD_SPR_SEVEN_DAY_ACT_GET_AWARD** | Nhận thưởng 7 ngày |
| **1312** | **CMD_SPR_SEVEN_DAY_ACT_QIANG_GOU** | Mua SevenDay |

### Range 1495-1515: Activity (EverydayAct, SpecialAct, JieRiVIP)
| ID | Tên | Chức năng |
|----|-----|----------|
| 1495-1498 | CMD_SPR_SPEPRIORITY_ACTIVITY_* | SpecPriorityActivity |
| 1505 | **CMD_SPR_EVERYDAYACTIVITY_GETXMLDATA** | **Lấy XML EveryDay Activity** |
| 1506 | **CMD_SPR_EVERYDAYACTIVITY_QUERY** | **Query EverydayAct** |
| 1507 | **CMD_SPR_EVERYDAYACTIVITY_FETCHAWARD** | **Nhận thưởng EverydayAct** |
| 1510 | CMD_SPR_SPECIALACTIVITY_GETXMLDATA | SpecialActivity XML |
| 1511 | CMD_SPR_SPECIALACTIVITY_QUERY | SpecialActivity query |
| 1512 | CMD_SPR_SPECIALACTIVITY_FETCHAWARD | SpecialActivity award |
| 1515-1516 | CMD_SPR_JIERIVIPYOUHUI_QUERY/FETCHAWARD | JieRi VIP ưu đãi |

---

## Tầng 2: Server → DB (CMD_DB_xxx, range 10000+)

### Range 10000-10100: Core DB
| DB ID | Tên |
|-------|-----|
| 10000 | CMD_DB_START_CMD |
| 10001-10020 | UPDATE_POS, EXPLEVEL, INTERPOWER, MONEY, ADDGOODS, UPDATEGOODS... |
| 10040 | CMD_DB_UPDATEROLEDAILYDATA |
| 10046 | CMD_DB_UPDATEROLEPARAM |

### Range 13160-13180: Activity DB Commands (quan trọng!)
| DB ID | Tên | Chức năng |
|-------|-----|----------|
| 13160 | CMD_DB_UPDATE_SPECACT | SpecialActivity update |
| 13161 | CMD_DB_DELETE_SPECACT | SpecialActivity delete |
| **13170** | **CMD_DB_UPDATE_EVERYACT** | **EverydayAct update** |
| **13171** | **CMD_DB_DELETE_EVERYACT** | **EverydayAct delete** |
| **13172** | **CMD_DB_GET_EVERYJIFENINFO** | **Get JiFen ngày** |
| **13173** | **CMD_DB_UPDATE_EVERYJIFEN** | **Update JiFen (nạp/tiêu)** |
| 13175 | CMD_DB_UPDATE_SPEC_PRIORITY_ACT | SpecPriorityActivity |
| 13176 | CMD_DB_DELETE_SPEC_PRIORITY_ACT | SpecPriorityActivity delete |

### Range 13200-13240: JieRi DB
| DB ID | Tên |
|-------|-----|
| 13200-13209 | JIERI_GIVE / RECV / KING operations |
| 13210-13220 | GUARD_STATUE / LIANXU_CHARGE |
| 13220-13225 | SEVEN_DAY_ITEM / ORNAMENT_ITEM |

---

## Tầng 3: DB bên trong (range 20000+)
| Range | Chức năng |
|-------|----------|
| 20000-20004 | LogDB |
| 20100 | Tarot update |
| 20200-20300 | Aoyun (Olympics) |
| 29900-30000 | TaskList |
| 30100 | GetAttribAll |
| 30767 | ERR_RETURN |

---

## Key Packet IDs liên quan trực tiếp đến Event System

| ID | Tên CMD | Luồng |
|----|---------|-------|
| 163 | CMD_SPR_GETHUODONGDATA | Lấy HuoDong data |
| 378 | CMD_SPR_GETJIERIXMLDATA | JieRi XML → client |
| 396 | CMD_SPR_FACTIVITIESDATA | Danh sách festival |
| 476 | CMD_SPR_DAILYACTIVEDATA | DailyActive query |
| 477 | CMD_SPR_GETDAILYACTIVEAWARD | DailyActive award |
| **558** | *Gửi DailyActiveData Packet* | Server→client auto |
| **770** | CMD_SPR_JIERIACT_STATE | EverydayAct state |
| 905 | CMD_SPR_THEME_GETXMLDATA | Theme Activity XML |
| 1310-1312 | CMD_SPR_SEVEN_DAY_ACT_* | SevenDay activity |
| 1505-1507 | CMD_SPR_EVERYDAYACTIVITY_* | EverydayActivity |
| 1510-1512 | CMD_SPR_SPECIALACTIVITY_* | SpecialActivity |
| 13170-13173 | CMD_DB_EVERY* | EverydayAct DB |
