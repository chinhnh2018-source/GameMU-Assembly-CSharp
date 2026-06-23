using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.Core;
using GameDBServer.Core.GameEvent;
using GameDBServer.Core.GameEvent.EventObjectImpl;
using GameDBServer.Data;
using GameDBServer.DB;
using GameDBServer.DB.DBController;
using GameDBServer.Logic;
using GameDBServer.Logic.Activity;
using GameDBServer.Logic.FluorescentGem;
using GameDBServer.Logic.GuardStatue;
using GameDBServer.Logic.MerlinMagicBook;
using GameDBServer.Logic.Name;
using GameDBServer.Logic.Ornament;
using GameDBServer.Logic.Rank;
using GameDBServer.Logic.Talent;
using GameDBServer.Logic.Tarot;
using GameDBServer.Logic.Ten;
using GameDBServer.Logic.Wing;
using GameDBServer.Tools;
using GameServer.Core.AssemblyPatch;
using MySQLDriverCS;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Server
{
	public class TCPCmdHandler
	{
		public static TCPProcessCmdResults ProcessCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			TCPProcessCmdResults tcpprocessCmdResults = TCPProcessCmdResults.RESULT_FAILED;
			tcpOutPacket = null;
			TCPProcessCmdResults tcpprocessCmdResults2 = AssemblyPatchManager.getInstance().ProcessMsg(client, nID, data, count);
			TCPProcessCmdResults result;
			if (TCPProcessCmdResults.RESUTL_CONTINUE != tcpprocessCmdResults2)
			{
				result = tcpprocessCmdResults2;
			}
			else
			{
				if (nID <= 927)
				{
					if (nID <= 404)
					{
						if (nID > 157)
						{
							if (nID <= 259)
							{
								if (nID != 163)
								{
									switch (nID)
									{
									case 191:
										tcpprocessCmdResults = TCPCmdHandler.ProcessGetDJPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 192:
									case 193:
									case 194:
									case 203:
										goto IL_2F47;
									case 195:
										tcpprocessCmdResults = TCPCmdHandler.ProcessQueryNameByIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 196:
										tcpprocessCmdResults = TCPCmdHandler.ProcessAddHorseCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 197:
										tcpprocessCmdResults = TCPCmdHandler.ProcessAddPetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 198:
										tcpprocessCmdResults = TCPCmdHandler.ProcessGetHorseListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 199:
										tcpprocessCmdResults = TCPCmdHandler.ProcessGetOtherHorseListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 200:
										tcpprocessCmdResults = TCPCmdHandler.ProcessGetPetListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 201:
										tcpprocessCmdResults = TCPCmdHandler.ProcessModHorseCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 202:
										tcpprocessCmdResults = TCPCmdHandler.ProcessModPetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 204:
										tcpprocessCmdResults = TCPCmdHandler.ProcessGetGoodsListBySiteCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									default:
										switch (nID)
										{
										case 257:
											tcpprocessCmdResults = TCPCmdHandler.ProcessGetRandomNameNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
											goto IL_2F5B;
										case 258:
											goto IL_2F47;
										case 259:
											tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuBenHistDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
											goto IL_2F5B;
										default:
											goto IL_2F47;
										}
										break;
									}
								}
							}
							else
							{
								if (nID == 269)
								{
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetPaiHangListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								switch (nID)
								{
								case 275:
									break;
								case 276:
								case 277:
								case 278:
								case 279:
								case 280:
								case 281:
								case 282:
								case 283:
								case 284:
								case 285:
								case 286:
								case 288:
								case 290:
								case 292:
								case 293:
								case 296:
								case 301:
								case 307:
								case 311:
								case 312:
								case 316:
								case 320:
								case 321:
								case 322:
								case 323:
								case 324:
								case 329:
								case 330:
								case 331:
								case 332:
								case 333:
								case 334:
								case 336:
								case 337:
								case 338:
								case 339:
								case 347:
								case 348:
								case 349:
								case 351:
								case 352:
								case 353:
								case 354:
								case 356:
								case 357:
								case 358:
								case 359:
								case 360:
								case 361:
								case 362:
								case 367:
								case 368:
								case 369:
								case 370:
								case 371:
									goto IL_2F47;
								case 287:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetChongZhiJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 289:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuBenHistListDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 291:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetOtherHorseDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 294:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBangHuiListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 295:
									tcpprocessCmdResults = TCPCmdHandler.ProcessCreateBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 297:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryBangHuiDetailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 298:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBangHuiBulletinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 299:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBHMemberDataListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 300:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBHVerifyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 302:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddBHMemberCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 303:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveBHMemberCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 304:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQuitFromBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 305:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDestroyBangHuiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 306:
									tcpprocessCmdResults = TCPCmdHandler.ProcessBangHuiVerifyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 308:
									tcpprocessCmdResults = TCPCmdHandler.ProcessChgBHMemberZhiWuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 309:
									tcpprocessCmdResults = TCPCmdHandler.ProcessChgBHMemberChengHaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 310:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSearchRolesFromDBCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 313:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBangGongHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 314:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDonateBGMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 315:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDonateBGGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 317:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBangQiInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 318:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRenameBangQiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 319:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpLevelBangQiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 325:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBHLingDiInfoDictByBHIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 326:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSetLingDiTaxCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 327:
									tcpprocessCmdResults = TCPCmdHandler.ProcessTakeLingDiTaxMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 328:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetHuangDiBHInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 335:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryQiZhenGeBuyHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 340:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetHuangDiRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 341:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddHuangFeiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 342:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveHuangFeiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 343:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetHuangFeiDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 344:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 345:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 346:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSendToLaoFangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 350:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddLingDiTaxMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 355:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetGoodsByDbIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 363:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetUserMailListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 364:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetUserMailDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 365:
									tcpprocessCmdResults = TCPCmdHandler.ProcessFetchMailGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 366:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteUserMailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 372:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryInputFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 373:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryInputJiaSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 374:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryInputKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 375:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryLevelKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 376:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryEquipKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 377:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryHorseKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 378:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryJingMaiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 379:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryAwardHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 380:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteInputFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 381:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteInputJiaSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 382:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteInputKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 383:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteLevelKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 384:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteEquipKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 385:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteHorseKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 386:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteJingMaiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								default:
									switch (nID)
									{
									case 402:
										tcpprocessCmdResults = TCPCmdHandler.ProcessQueryShengXiaoGuessHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									case 403:
										goto IL_2F47;
									case 404:
										tcpprocessCmdResults = TCPCmdHandler.ProcessQueryShengXiaoGuessHistCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									default:
										goto IL_2F47;
									}
									break;
								}
							}
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetOtherAttrib2DataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						if (nID <= 125)
						{
							switch (nID)
							{
							case 98:
								tcpprocessCmdResults = TCPCmdHandler.ProcessPreRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 99:
								tcpprocessCmdResults = TCPCmdHandler.ProcessUnPreRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 100:
								break;
							case 101:
								tcpprocessCmdResults = TCPCmdHandler.ProcessGetRoleListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 102:
								tcpprocessCmdResults = TCPCmdHandler.ProcessCreateRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 103:
								tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveRoleCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 104:
								tcpprocessCmdResults = TCPCmdHandler.ProcessInitGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 125)
								{
									tcpprocessCmdResults = TCPCmdHandler.ProcessNewTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
						else
						{
							switch (nID)
							{
							case 140:
								tcpprocessCmdResults = TCPCmdHandler.ProcessCompleteTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 141:
								break;
							case 142:
								tcpprocessCmdResults = TCPCmdHandler.ProcessGetFriendsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 143:
								tcpprocessCmdResults = TCPCmdHandler.ProcessAddFriendCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 144:
								tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveFriendCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 154)
								{
									tcpprocessCmdResults = TCPCmdHandler.ProcessAbandonTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								if (nID == 157)
								{
									tcpprocessCmdResults = TCPCmdHandler.ProcessSpriteChatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
					}
					else if (nID <= 631)
					{
						if (nID <= 524)
						{
							if (nID == 418)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessTakeLingDiDailyAwardCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 452:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryZaJinDanHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 453:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryZaJinDanHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 454:
							case 455:
							case 456:
							case 457:
							case 458:
							case 459:
							case 460:
							case 466:
							case 474:
							case 477:
							case 478:
							case 479:
							case 494:
							case 495:
							case 496:
							case 497:
							case 498:
							case 499:
							case 500:
							case 502:
							case 505:
							case 506:
							case 507:
							case 508:
							case 510:
							case 511:
								break;
							case 461:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 462:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriDengLuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 463:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 464:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 465:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriCZLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 467:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriXiaoFeiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 468:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriCZKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 469:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 470:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriDengLuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 471:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 472:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 473:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriCZLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 475:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriXiaoFeiKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 476:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriCZKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 480:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 481:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 482:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 483:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 484:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuPKKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 485:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryHeFuWCKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 486:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryXinCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 487:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 488:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuVIPCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 489:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuCZSongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 490:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 491:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuPKKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 492:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteHeFuWCKingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 493:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteXinCZFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 501:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 503:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryYueDuChouJiangHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 504:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryYueDuChouJiangHistoryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 509:
								tcpprocessCmdResults = TCPCmdHandler.ProcessChangeLifeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 512:
								tcpprocessCmdResults = TCPCmdHandler.ProcessGetUsingGoodsDataListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								if (nID == 524)
								{
									tcpprocessCmdResults = TCPCmdHandler.ProcessCompleteFlashSceneCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
						else
						{
							if (nID == 528)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessAdmiredPlayerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							if (nID == 560)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessSetAutoAssignPropertyPointCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 629:
								tcpprocessCmdResults = NewZoneActiveMgr.ProcessGetNewzoneActiveAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 630:
								goto IL_2F5B;
							case 631:
								tcpprocessCmdResults = NewZoneActiveMgr.ProcessQueryActiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
						}
					}
					else if (nID <= 684)
					{
						if (nID == 649)
						{
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetUserMailCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						if (nID == 671)
						{
							tcpprocessCmdResults = CFirstChargeMgr.ProcessQueryUserFirstCharge(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 683:
							tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieriTotalConsumeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 684:
							tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriTotalConsumeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
					}
					else
					{
						if (nID == 711)
						{
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetBangHuiFuBenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 908:
							tcpprocessCmdResults = TCPCmdHandler.ProcessQueryThemeDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 909:
							tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteThemeDaLiBaoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							if (nID == 927)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriFanLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
				}
				else if (nID <= 13321)
				{
					if (nID <= 10224)
					{
						if (nID <= 1240)
						{
							switch (nID)
							{
							case 947:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryDanBiChongZhiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 948:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteDanBiChongZhiJiangLiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 1111:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGVoiceSetPrioritysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 1112:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGVoiceGetPrioritysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								default:
									if (nID == 1240)
									{
										tcpprocessCmdResults = TCPCmdHandler.ProcessChgJunTuanMemberZhiWuCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
										goto IL_2F5B;
									}
									break;
								}
								break;
							}
						}
						else
						{
							if (nID == 1500)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessGetInputPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							switch (nID)
							{
							case 1806:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryJieRiMeiRiLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 1807:
								tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteJieriMeiRiLeiJiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 10001:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdatePosCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10002:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateExpLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10003:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateInterPowerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10004:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateMoney1Cmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10005:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10006:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10007:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10008:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdatePKModeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10009:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdatePKValCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10010:
									tcpprocessCmdResults = TCPCmdHandler.ProcessModKeysCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10011:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateUserMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10012:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateUserYinLiangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10013:
									tcpprocessCmdResults = TCPCmdHandler.ProcessMoveGoodsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10014:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateLeftFightSecsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10015:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRoleOnLineGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10016:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRoleHeartGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10017:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRoleOffLineGameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10018:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetChatMsgListCmd(client, dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10019:
									tcpprocessCmdResults = TCPCmdHandler.ProcessHorseOnCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10020:
									tcpprocessCmdResults = TCPCmdHandler.ProcessHorseOffCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10021:
									tcpprocessCmdResults = TCPCmdHandler.ProcessPetOutCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10022:
									tcpprocessCmdResults = TCPCmdHandler.ProcessPetInCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10023:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetAddDJPointCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10024:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpDianJiangLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10025:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRegUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10026:
									tcpprocessCmdResults = TCPCmdHandler.ProcessBanRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10027:
									tcpprocessCmdResults = TCPCmdHandler.ProcessBanRoleChatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10028:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBanRoleChatDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10029:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddBullMsgCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10030:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveBullMsgCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10031:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBullMsgDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10032:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateOnlineTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10033:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGameConfigDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10034:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGameConfigItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10035:
									tcpprocessCmdResults = TCPCmdHandler.ProcessResetBigGuanCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10036:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddSkillCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10037:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpSkillInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10038:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateJingMaiExpCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10039:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateSkillIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10040:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateAutoDrinkCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10041:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateDailyTaskDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10042:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateDailyJingMaiCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10043:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateNumSkillIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10044:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdatePBInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10045:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateHuoDongInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10046:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSubChongZhiJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10047:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUseLiPinMaCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10048:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateFuBenDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10049:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuBenSeqIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10050:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleDailyDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10051:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBufferItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10052:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUnDelRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10053:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddFuBenHistDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10054:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateLianZhanCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10055:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateKillBossCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10056:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleStatCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10057:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateYaBiaoDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10058:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateYaBiaoDataStateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10059:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBattleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10060:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddMallBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10061:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetLiPinMaInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10062:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateCZTaskIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10063:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetTotalOnlineNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10064:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBattleNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10065:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateHeroIndexCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10066:
									tcpprocessCmdResults = TCPCmdHandler.ProcessForceReloadPaiHangCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10067:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddYinPiaoBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10068:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDelRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10069:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryUserMoneyByNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10070:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryBHMGRListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10071:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBangGongCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10072:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBHTongQianCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10073:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBHJunQiListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10074:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBHLingDiDictCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10075:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateLingDiForBHCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10076:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetLeaderRoleIDByBHIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10077:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddBHTongQianCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10078:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddQiZhenGeBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10079:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateJieBiaoInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10080:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddRefreshQiZhenRecCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10081:
									tcpprocessCmdResults = TCPCmdHandler.ProcessClrCachingRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10082:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddMoneyWarningCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10083:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10084:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddYinLiangBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10085:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddBangGongBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10086:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSendUserMailCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10087:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetUserMailDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10088:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetRoleIDByRoleNameCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10089:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDBQueryLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10090:
									tcpprocessCmdResults = TCPCmdHandler.ProcessDBUpdateLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10091:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateDailyVipDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10092:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateYangGongBKDailyJiFenDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10093:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateSingleTimeAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10094:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddShengXiaoGuessHisotryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10095:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateUserGoldCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10096:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddGoldBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10097:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleBagNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10098:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSetLingDiWarRequestCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10099:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateGoodsLimitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10100:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10101:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddQiangGouBuyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10102:
									goto IL_2F5B;
								case 10103:
									goto IL_2F5B;
								case 10104:
									goto IL_2F5B;
								case 10105:
									goto IL_2F5B;
								case 10106:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetBangHuiMiniDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10107:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddBuyItemFromNpcCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10108:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddZaJinDanHisotryCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10109:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryQiangGouBuyItemInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10110:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryFirstChongZhiDaLiByUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10111:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryKaiFuOnlineAwardRoleIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10112:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddKaiFuOnlineAwardCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10113:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddGiveUserMoneyItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10114:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryKaiFuOnlineAwardListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10115:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddExchange1ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10116:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddExchange2ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10117:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddExchange3ItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10118:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddFallGoodsItemCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10119:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRolePropsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10120:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryDayChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10121:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryDayChongZhiDaLiByUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10122:
									tcpprocessCmdResults = TCPCmdHandler.ProcessClrAllCachingRoleDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10123:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryXingYunChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10124:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteXingYunChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10125:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExcuteAddYueDuChouJiangInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10126:
									tcpprocessCmdResults = TCPCmdHandler.ProcessExecuteChangeOccupationCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10127:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryBloodCastleEnterCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10128:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBloodCastleEnterCountCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10129:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryFuBenHisInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10130:
									tcpprocessCmdResults = TCPCmdHandler.ProcessCleanDataWhenFreshPlayerLogOutCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10131:
									tcpprocessCmdResults = TCPCmdHandler.ProcessFinishFreshPlayerStatusCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10132:
									tcpprocessCmdResults = TCPCmdHandler.ProcessChangeTaskStarLevelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10133:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleSomeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10134:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10135:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryRoleDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10136:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleDayActivityPoinCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10137:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryEveryDayOnLineAwardGiftInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10151:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdatePushMessageInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10152:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryPushMsgUerListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10153:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddWingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10154:
									tcpprocessCmdResults = TCPCmdHandler.ProcessModWingCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10155:
									tcpprocessCmdResults = TCPCmdHandler.ProcessReferPictureJudgeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10156:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryMoJingExchangeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10157:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateMoJingExchangeInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10160:
									tcpprocessCmdResults = RechargeRepayActiveMgr.ProcessQueryActiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10161:
									tcpprocessCmdResults = RechargeRepayActiveMgr.ProcessGetActiveAwards(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10162:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateAccountActiveCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10163:
									tcpprocessCmdResults = CGetOldResourceManager.ProcessQueryGetResourceInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10164:
									tcpprocessCmdResults = CGetOldResourceManager.ProcessUpdateGetResourceInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10165:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateGoodsCmd2(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10166:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateStarConstellationCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10167:
									tcpprocessCmdResults = Global.SaveConsumeLog(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10168:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryVipLevelAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10169:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateVipLevelAwardFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10171:
									tcpprocessCmdResults = CFirstChargeMgr.FirstChargeConfig(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10172:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateBangHuiFuBenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10173:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddRoleStoreYinliang(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10174:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddRoleStoreMoney(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10175:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGMUpdateBangLevel(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10176:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateLingYu(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10177:
									tcpprocessCmdResults = GroupMailManager.RequestNewGroupMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10178:
									tcpprocessCmdResults = GroupMailManager.ModifyGMailRecord(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10179:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryRoleMoneyInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10180:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAutoCompletionTaskByTaskID(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10181:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRoleBuyYueKaButOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10182:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRoleHuobiOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10183:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUsrSetSecPwd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10184:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetUsrSecondPassword(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10185:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateMarriageDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10186:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetMarriageDataCmd(dbMgr, pool, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10187:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10188:
									tcpprocessCmdResults = TCPCmdHandler.ProcessAddMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10189:
									tcpprocessCmdResults = TCPCmdHandler.ProcessRemoveMarryParty(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10190:
									tcpprocessCmdResults = TCPCmdHandler.ProcessIncMarryPartyJoin(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10191:
									tcpprocessCmdResults = TCPCmdHandler.ProcessClearMarryPartyJoin(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10203:
									tcpprocessCmdResults = MerlinMagicBookManager.getInstance().ProcessInsertMerlinDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10204:
									tcpprocessCmdResults = MerlinMagicBookManager.getInstance().ProcessUpdateMerlinDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10205:
									tcpprocessCmdResults = MerlinMagicBookManager.getInstance().ProcessQueryMerlinDataCmd(dbMgr, pool, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10206:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateHolyItemDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10207:
									tcpprocessCmdResults = FluorescentGemManager.getInstance().ProcessResetBagDataCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10208:
									tcpprocessCmdResults = FluorescentGemManager.getInstance().ProcessUpdateFluorescentPointCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10209:
									tcpprocessCmdResults = FluorescentGemManager.getInstance().ProcessEquipGemCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10210:
									tcpprocessCmdResults = FluorescentGemManager.getInstance().ProcessUnEquipGemCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10211:
									tcpprocessCmdResults = FluorescentGemManager.getInstance().ProcessModifyFluorescentPointCmd(dbMgr, client, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10220:
									tcpprocessCmdResults = TCPCmdHandler.ProcessQueryRoleMiniInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10221:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprQueryUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10222:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSprUpdateUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10223:
									tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateZhengDuoUsedTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 10224:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGetZhengDuoUsedTimeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
						}
					}
					else if (nID <= 13121)
					{
						switch (nID)
						{
						case 11000:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetServerListCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11001:
							tcpprocessCmdResults = TCPCmdHandler.ProcessOnlineServerHeartCmd(client, dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11002:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetServerIdCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11003:
							tcpprocessCmdResults = TCPCmdHandler.ProcessSetServerDayDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11004:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetServerDayDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 11005:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetServerPTIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 13000:
								tcpprocessCmdResults = TCPCmdHandler.ProcessGMSetTaskCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13001:
								tcpprocessCmdResults = TCPCmdHandler.ProcessQueryUserIdValueCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 13108:
									tcpprocessCmdResults = TalentManager.ProcTalentModify(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13109:
									tcpprocessCmdResults = TalentManager.ProcTalentEffectModify(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13110:
									tcpprocessCmdResults = TalentManager.ProcTalentEffectClear(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13111:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGmBanCheck(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13112:
									tcpprocessCmdResults = TCPCmdHandler.ProcessGmBanLog(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13113:
									tcpprocessCmdResults = TCPCmdHandler.ProcessTenInitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13114:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSpreadAwardGetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13115:
									tcpprocessCmdResults = TCPCmdHandler.ProcessSpreadAwardUpdateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13120:
									tcpprocessCmdResults = TCPCmdHandler.ProcessActivateStateGetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13121:
									tcpprocessCmdResults = TCPCmdHandler.ProcessActivateStateSetCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
							break;
						}
					}
					else
					{
						switch (nID)
						{
						case 13150:
							tcpprocessCmdResults = TCPCmdHandler.ProcessInputPointsExchangeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13151:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateInputPointsCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13152:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateInputPointsUserIDCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13153:
						case 13154:
						case 13155:
						case 13156:
						case 13157:
						case 13158:
						case 13159:
						case 13165:
						case 13166:
						case 13167:
						case 13168:
						case 13169:
						case 13174:
							break;
						case 13160:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateSpecActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13161:
							tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteSpecActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13162:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetSpecJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13163:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateSpecJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13164:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetSpecActInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13170:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateEveryActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13171:
							tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteEveryActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13172:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetEveryJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13173:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateEveryJiFenCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13175:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateSpecPriorityActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13176:
							tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteSpecPriorityActCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 13177:
							tcpprocessCmdResults = TCPCmdHandler.ProcessQueryPeriodChongZhiMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 13200:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveActHandler>.Instance().ProcRoleJieriGiveToOther(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13201:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveActHandler>.Instance().ProcessGetJieriGiveAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13202:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveActHandler>.Instance().ProcQueryJieriGiveInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13203:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcLoadJieriGiveKingRank(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13204:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcLoadRoleJieriGiveKing(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13205:
								tcpprocessCmdResults = SingletonTemplate<JieriGiveKingActHandler>.Instance().ProcGetJieriGiveKingAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13206:
								tcpprocessCmdResults = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcLoadJieriRecvKingRank(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13207:
								tcpprocessCmdResults = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcLoadRoleJieriRecvKing(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13208:
								tcpprocessCmdResults = SingletonTemplate<JieriRecvKingActHandler>.Instance().ProcGetJieriRecvKingAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13209:
							case 13212:
							case 13213:
							case 13216:
							case 13217:
							case 13218:
							case 13219:
							case 13220:
							case 13221:
							case 13222:
								break;
							case 13210:
								tcpprocessCmdResults = SingletonTemplate<GuardStatueHandler>.Instance().ProcUpdateRoleGuardStatue(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13211:
								tcpprocessCmdResults = SingletonTemplate<GuardStatueHandler>.Instance().ProcUpdateRoleGuardSoul(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13214:
								tcpprocessCmdResults = SingletonTemplate<JieriLianXuChargeActHandler>.Instance().ProcQueryActInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13215:
								tcpprocessCmdResults = SingletonTemplate<JieriLianXuChargeActHandler>.Instance().ProcUpdateAward(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 13223:
								tcpprocessCmdResults = OrnamentManager.ProcessUpdateOrnamentDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							default:
								switch (nID)
								{
								case 13320:
									tcpprocessCmdResults = UserMoneyMgr.ProcessGetChargeItemData(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								case 13321:
									tcpprocessCmdResults = UserMoneyMgr.ProcessDelChargeItemData(dbMgr, pool, nID, data, count, out tcpOutPacket);
									goto IL_2F5B;
								}
								break;
							}
							break;
						}
					}
				}
				else if (nID <= 20003)
				{
					if (nID <= 14021)
					{
						if (nID == 13400)
						{
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateOnePieceTreasureLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						switch (nID)
						{
						case 14001:
							tcpprocessCmdResults = SingletonTemplate<NameManager>.Instance().ProcChangeName(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14002:
							tcpprocessCmdResults = SingletonTemplate<NameManager>.Instance().ProcQueryEachRoleInfo(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14003:
						case 14004:
						case 14007:
						case 14008:
							break;
						case 14005:
							tcpprocessCmdResults = SingletonTemplate<JieriPlatChargeKingActHandler>.Instance().ProcGetJieriPlatChargeKingList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14006:
							tcpprocessCmdResults = SingletonTemplate<NameManager>.Instance().ProcChangeBangHuiName(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14009:
							tcpprocessCmdResults = SingletonTemplate<NameManager>.Instance().ProcAddBangHuiChangeNameTimes(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							switch (nID)
							{
							case 14020:
								tcpprocessCmdResults = TCPCmdHandler.ProcessBHMatchLoadSupportFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							case 14021:
								tcpprocessCmdResults = TCPCmdHandler.ProcessBHMatchUpdateSupportFlagCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
					else
					{
						switch (nID)
						{
						case 14101:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateFuMoAcceptMap(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14102:
							tcpprocessCmdResults = TCPCmdHandler.ProcessAddFuMoMoneyGiveMail(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14103:
							tcpprocessCmdResults = TCPCmdHandler.ProcessFuMoMailIndexCount(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14104:
							tcpprocessCmdResults = TCPCmdHandler.ProcessAddFuMoMoneyGiveMailTemp(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14105:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuMoMoneyMapAcceptNum(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14106:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuMoMoneyMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14107:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetFuMoMoneyMailMapList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14108:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateFuMoMoneyMailMap(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14109:
							tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteFuMoMail(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14110:
							tcpprocessCmdResults = TCPCmdHandler.ProcessDeleteFuMoMailList(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14111:
							tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateFuMoMailReadState(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14112:
						case 14113:
						case 14114:
						case 14121:
						case 14123:
						case 14124:
						case 14125:
						case 14126:
						case 14127:
						case 14128:
						case 14129:
							break;
						case 14115:
							tcpprocessCmdResults = TCPCmdHandler.ProcessRebornYinJiInsertInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14116:
							tcpprocessCmdResults = TCPCmdHandler.ProcessRebornYinJiUpdateCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14117:
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetRebornYinJiInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14118:
							tcpprocessCmdResults = RebornEquip.ProcessUpdateRoleRebornBagNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14119:
							tcpprocessCmdResults = RebornEquip.ProcessUpdateRebornStorageInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14120:
							tcpprocessCmdResults = RebornEquip.ProcessUpdateRoleRebornShowEquipCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14122:
							tcpprocessCmdResults = RebornEquip.ProcessUpdateRoleRebornShowModelCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14130:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessGetRegressActiveMinTime(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14131:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessUpdateEverySignCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14132:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessSprQueryUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14133:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessDBQueryUserAllLimitGoodsUsedNumInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14134:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessDBQueryLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14135:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessDBUpdateUserLimitGoodsUsedNumCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14136:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessRergressQueryUserInputMoneyCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14137:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessSprQueryDayUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						case 14138:
							tcpprocessCmdResults = UserRegressActiveManager.ProcessUpdateSprQueryDayUserActivityInfoCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						default:
							if (nID == 20000)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessAddItemLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							if (nID == 20003)
							{
								tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateRoleKuaFuDayLogCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
								goto IL_2F5B;
							}
							break;
						}
					}
				}
				else if (nID <= 20314)
				{
					if (nID == 20100)
					{
						tcpprocessCmdResults = TarotManager.ProcessUpdateTarotDataCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					switch (nID)
					{
					case 20304:
						tcpprocessCmdResults = TCPCmdHandler.ProcessGetRoleParamCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					case 20305:
						tcpprocessCmdResults = TCPCmdHandler.ProcessUpdateWebOldPlayerCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					default:
						if (nID == 20314)
						{
							tcpprocessCmdResults = TCPCmdHandler.ProcessGetGoodsListBySiteRangeCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
							goto IL_2F5B;
						}
						break;
					}
				}
				else
				{
					if (nID == 20398)
					{
						tcpprocessCmdResults = TCPCmdHandler.ProcessSubRoleHuobiOffline(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					if (nID == 21000)
					{
						tcpprocessCmdResults = TCPCmdHandler.ProcessFacebookInitCmd(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
					if (nID == 30010)
					{
						tcpprocessCmdResults = TCPCmdHandler.ProcessGetZoneIdByRid(dbMgr, pool, nID, data, count, out tcpOutPacket);
						goto IL_2F5B;
					}
				}
				IL_2F47:
				tcpprocessCmdResults = TCPCmdDispatcher.getInstance().dispathProcessor(client, nID, data, count);
				IL_2F5B:
				result = tcpprocessCmdResults;
			}
			return result;
		}

		private static TCPProcessCmdResults ProcessGetUsrSecondPassword(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (dbuserInfo == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的user不存在，CMD={0}, userID={1}", (TCPGameServerCmds)nID, text2), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}", text2, dbuserInfo.SecPwd), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUsrSetSecPwd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				string secPwd = array[1];
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (dbuserInfo == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的user不存在，CMD={0}, userID={1}", (TCPGameServerCmds)nID, text2), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				if (DBWriter.UpdateUsrSecondPassword(dbMgr, text2, secPwd))
				{
					dbuserInfo.SecPwd = secPwd;
					flag = true;
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, flag ? "0:1" : "0", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRoleHuobiOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				int num2 = Convert.ToInt32(array[2]);
				if (string.IsNullOrEmpty(text2))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("被修改货币的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = dbroleInfo.RoleName;
				if (!("jinbi" == text2))
				{
					if (!("bangjin" == text2))
					{
						if (!("zuanshi" == text2))
						{
							if (!("bangzuan" == text2))
							{
								if ("mojing" == text2)
								{
									string text3 = "TianDiJingYuan";
									long num3 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text3, (long)num2, null);
									string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text3, num3);
									byte[] bytes = new UTF8Encoding().GetBytes(s);
									return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
								}
								if (!("chengjiu" == text2))
								{
									if (!("shengwang" == text2))
									{
										if (!("xinghun" == text2))
										{
											if ("lingjing" == text2)
											{
												string text3 = "MUMoHe";
												long num3 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text3, (long)num2, null);
												string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text3, num3);
												byte[] bytes = new UTF8Encoding().GetBytes(s);
												return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
											}
											if (!("fenmo" == text2))
											{
												if ("zaizao" == text2)
												{
													string text3 = "ZaiZaoPoint";
													long num3 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text3, (long)num2, null);
													string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text3, num3);
													byte[] bytes = new UTF8Encoding().GetBytes(s);
													return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
												}
												LogManager.WriteLog(LogTypes.Error, string.Format("未注册的GM修改货币类型,Rolename:{0},Huobi:{1},Modify:{2}", roleName, text2, num2), null, true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSubRoleHuobiOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Global.SafeConvertToInt32(array[0], 10);
				int num2 = Global.SafeConvertToInt32(array[1], 10);
				long num3 = Global.SafeConvertToInt64(array[2], 10);
				if (num2 == 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("被修改货币的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = dbroleInfo.RoleName;
				int num4 = num2;
				if (num4 <= 40)
				{
					if (num4 <= 8)
					{
						if (num4 != 1)
						{
							if (num4 == 8)
							{
								long num5 = (long)dbroleInfo.YinLiang + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string s = string.Format("{0}:{1}", dbroleInfo.RoleID, num3);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateUserYinLiangCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
						}
						else
						{
							long num5 = (long)dbroleInfo.Money1 + num3;
							if (num5 < -2147483648L || num5 > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							string s = string.Format("{0}:{1}", dbroleInfo.RoleID, num5);
							byte[] bytes = new UTF8Encoding().GetBytes(s);
							return TCPCmdHandler.ProcessUpdateMoney1Cmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
						}
					}
					else
					{
						switch (num4)
						{
						case 13:
						{
							string text2 = "TianDiJingYuan";
							long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
							if (num5 < -2147483648L || num5 > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
							string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
							byte[] bytes = new UTF8Encoding().GetBytes(s);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
						}
						case 14:
							break;
						case 15:
						{
							string text2 = "ZJDJiFen";
							long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
							if (num5 < -2147483648L || num5 > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
							string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
							byte[] bytes = new UTF8Encoding().GetBytes(s);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
						}
						default:
							if (num4 == 40)
							{
								DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
								if (null == dbuserInfo)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								long num5 = (long)dbuserInfo.Money + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string s = string.Format("{0}:{1}", dbroleInfo.RoleID, num3);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateUserMoneyCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							break;
						}
					}
				}
				else if (num4 <= 101)
				{
					if (num4 != 50)
					{
						if (num4 == 101)
						{
							string text2 = "ZaiZaoPoint";
							long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
							if (num5 < -2147483648L || num5 > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
							string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
							byte[] bytes = new UTF8Encoding().GetBytes(s);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
						}
					}
					else
					{
						long num5 = (long)dbroleInfo.Gold + num3;
						if (num5 < -2147483648L || num5 > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						string s = string.Format("{0}:{1}", dbroleInfo.RoleID, num3);
						byte[] bytes = new UTF8Encoding().GetBytes(s);
						return TCPCmdHandler.ProcessUpdateUserGoldCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
					}
				}
				else
				{
					switch (num4)
					{
					case 106:
					{
						string text2 = "MUMoHe";
						long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
						if (num5 < -2147483648L || num5 > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
						string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
						byte[] bytes = new UTF8Encoding().GetBytes(s);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
					}
					case 107:
					{
						string text2 = "ElementPowder";
						long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
						if (num5 < -2147483648L || num5 > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
						string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
						byte[] bytes = new UTF8Encoding().GetBytes(s);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
					}
					case 108:
						break;
					case 109:
					{
						long num5 = (long)dbroleInfo.FluorescentPoint + num3;
						if (num5 < -2147483648L || num5 > 2147483647L)
						{
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
						int num6 = (int)num5;
						int num7 = FluorescentGemDBOperate.UpdateFluorescentPoint(dbMgr, dbroleInfo.RoleID, num6) ? dbroleInfo.RoleID : 0;
						lock (dbroleInfo)
						{
							dbroleInfo.FluorescentPoint = num6;
						}
						string s = string.Format("{0}:{1}", num7, num6);
						byte[] bytes = new UTF8Encoding().GetBytes(s);
						return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
					}
					default:
						if (num4 != 119)
						{
							switch (num4)
							{
							case 129:
							{
								string text2 = "10187";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 130:
							{
								long num5 = (long)dbroleInfo.AlchemyInfo.BaseData.Element + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								string s = string.Format("{0}:{1}", dbroleInfo.RoleID, num3);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return SingletonTemplate<AlchemyManager>.Instance().ProcessUpdateAlchemyElement(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 131:
							{
								string text2 = "10194";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 132:
							{
								string text2 = "10208";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 133:
							{
								string text2 = "10209";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 134:
							{
								string text2 = "10217";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							case 135:
							{
								string text2 = "10204";
								long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
								if (num5 < -2147483648L || num5 > 2147483647L)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
									return TCPProcessCmdResults.RESULT_DATA;
								}
								num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
								string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
								byte[] bytes = new UTF8Encoding().GetBytes(s);
								return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
							}
							}
						}
						else
						{
							string text2 = "10153";
							long num5 = Global.GetRoleParamsInt64(dbroleInfo, text2) + num3;
							if (num5 < -2147483648L || num5 > 2147483647L)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
								return TCPProcessCmdResults.RESULT_DATA;
							}
							num5 = Global.ModifyRoleParamLongByName(dbMgr, dbroleInfo, text2, num3, null);
							string s = string.Format("{0}:{1}:{2}", dbroleInfo.RoleID, text2, num5);
							byte[] bytes = new UTF8Encoding().GetBytes(s);
							return TCPCmdHandler.ProcessUpdateRoleParamCmd(dbMgr, pool, nID, bytes, bytes.Length, out tcpOutPacket);
						}
						break;
					}
				}
				LogManager.WriteLog(LogTypes.Error, " -modifyRoleHuobi 未注册的货币类型:" + num2, null, true);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRoleBuyYueKaButOffline(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string name = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					string[] array2 = null;
					string value = "";
					string roleParamByName = Global.GetRoleParamByName(dbroleInfo, name);
					if (!string.IsNullOrEmpty(roleParamByName))
					{
						array2 = roleParamByName.Split(new char[]
						{
							','
						});
						if (array2.Length == 5 && array2[0] == "1")
						{
							value = roleParamByName;
						}
					}
					string value2;
					if (string.IsNullOrEmpty(value))
					{
						value2 = string.Format("1,{0},{1},{2},0", num2, num2 + 30, num2);
					}
					else
					{
						value2 = string.Format("{0},{1},{2},{3},{4}", new object[]
						{
							array2[0],
							array2[1],
							Convert.ToInt32(array2[2]) + 30,
							array2[3],
							array2[4]
						});
					}
					Global.UpdateRoleParamByName(dbMgr, dbroleInfo, name, value2, null);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 0), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRegUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int serverLineID = Convert.ToInt32(array[1]);
				int num = Convert.ToInt32(array[2]);
				int num2 = 1;
				long num3 = 0L;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (dbuserInfo == null)
				{
					num2 = -10;
				}
				else
				{
					num2 = 1;
					if (!UserOnlineManager.RegisterUserID(userID, serverLineID, num))
					{
						num2 = 0;
					}
					else if (num == 0)
					{
						lock (dbuserInfo)
						{
							num3 = dbuserInfo.LogoutServerTicks;
						}
					}
				}
				string data2 = string.Format("{0}:{1}", num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetRoleListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				string s;
				if (null == dbuserInfo)
				{
					s = string.Format("{0}:{1}", 0, "");
				}
				else
				{
					int num2 = 0;
					string text2 = "";
					lock (dbuserInfo)
					{
						for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
						{
							if (dbuserInfo.ListRoleZoneIDs[i] == num)
							{
								int num3 = GameDBManager.PreDelRoleMgr.CalcPreDeleteRoleLeftSeconds(dbuserInfo.ListRolePreRemoveTime[i]);
								num2++;
								text2 += string.Format("{0}${1}${2}${3}${4}${5}${6}|", new object[]
								{
									dbuserInfo.ListRoleIDs[i],
									dbuserInfo.ListRoleSexes[i],
									dbuserInfo.ListRoleOccups[i],
									dbuserInfo.ListRoleNames[i],
									dbuserInfo.ListRoleLevels[i],
									dbuserInfo.ListRoleChangeLifeCount[i],
									num3
								});
							}
						}
					}
					text2 = text2.Trim(new char[]
					{
						'|'
					});
					s = string.Format("{0}:{1}", num2, text2);
				}
				byte[] bytes = new UTF8Encoding().GetBytes(s);
				tcpOutPacket = pool.Pop();
				tcpOutPacket.PacketCmdID = 101;
				tcpOutPacket.FinalWriteData(bytes, 0, bytes.Length);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetRandomNameNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string randomName = PreNamesManager.GetRandomName(num);
				string data2 = string.Format("{0}:{1}", num, randomName);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessCreateRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 7 && array.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				string userName = array[1];
				int num = Convert.ToInt32(array[2]);
				int num2 = Convert.ToInt32(array[3]);
				string text3 = array[4].Split(new char[]
				{
					'$'
				})[0];
				int num3 = Convert.ToInt32(array[5]);
				int nMagicSwordParam = Convert.ToInt32(array[6]);
				bool flag = false;
				bool flag2 = false;
				if (num2 == 5 && array.Length == 8)
				{
					flag2 = (Global.SafeConvertToInt32(array[7], 10) > 0);
				}
				string data2;
				if (DBWriter.CheckRoleCountFull(dbMgr))
				{
					data2 = string.Format("{0}:{1}", -2, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，服务器角色已满，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, text3))
				{
					data2 = string.Format("{0}:{1}", -3, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，角色名在数据库中乱码，CMD={0}, RoleName={1}", (TCPGameServerCmds)nID, text3), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_Ex(text3) || dbMgr.IsRolenameExist(text3))
				{
					data2 = string.Format("{0}:{1}", -1, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加角色时，角色重名，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = 1;
				int num5 = 0;
				int num6 = 0;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (null != dbuserInfo)
				{
					lock (dbuserInfo)
					{
						for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
						{
							if (dbuserInfo.ListRoleZoneIDs[i] == num3)
							{
								if (num2 == 3)
								{
									if (!flag)
									{
										if (dbuserInfo.ListRoleChangeLifeCount[i] > 3 || (dbuserInfo.ListRoleChangeLifeCount[i] == 3 && dbuserInfo.ListRoleLevels[i] >= 1))
										{
											flag = true;
											num4 = 1;
											num5 = 2;
										}
									}
								}
								else if (num2 == 5)
								{
									if (!flag2)
									{
										if (dbuserInfo.ListRoleChangeLifeCount[i] > 3 || (dbuserInfo.ListRoleChangeLifeCount[i] == 3 && dbuserInfo.ListRoleLevels[i] >= 1))
										{
											flag2 = true;
											num4 = 1;
											num5 = 2;
										}
									}
								}
								num6++;
							}
						}
					}
				}
				if (num2 == 3 && !flag)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(text3);
					data2 = string.Format("{0}:{1}", -5, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("创建魔剑士条件不足，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 == 5 && !flag2)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(text3);
					data2 = string.Format("{0}:{1}", -5, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("创建召唤师条件不足，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num6 >= 4)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(text3);
					data2 = string.Format("{0}:{1}", -1000, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Warning, string.Format("添加新角色失败, 数量超过了4个，是否外挂操作，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, -1000), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (PreNamesManager.SetUsedPreName(text3))
				{
					DBWriter.UpdatePreNameUsedState(dbMgr, text3, 1);
				}
				int num7 = DBWriter.CreateRole(dbMgr, text2, userName, num, num2, text3, num3, 50, 1, nMagicSwordParam, 50);
				if (0 > num7)
				{
					SingletonTemplate<NameUsedMgr>.Instance().DelCannotUse_Ex(text3);
					data2 = string.Format("{0}:{1}", num7, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num7), null, true);
				}
				else
				{
					DBWriter.UpdateRolePBInfo(dbMgr, num7, 60);
					DBWriter.UpdateRoleRebornStorageInfo(dbMgr, num7, 60);
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num7);
					if (null != dbroleInfo)
					{
						Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					}
					dbuserInfo = dbMgr.GetDBUserInfo(text2);
					if (null != dbuserInfo)
					{
						lock (dbuserInfo)
						{
							dbuserInfo.ListRoleIDs.Add(num7);
							dbuserInfo.ListRoleSexes.Add(num);
							dbuserInfo.ListRoleOccups.Add(num2);
							dbuserInfo.ListRoleNames.Add(text3);
							dbuserInfo.ListRoleLevels.Add(num4);
							dbuserInfo.ListRoleZoneIDs.Add(num3);
							dbuserInfo.ListRoleChangeLifeCount.Add(num5);
							dbuserInfo.ListRolePreRemoveTime.Add("");
						}
					}
					int num8 = DBQuery.QueryVipLevelAwardFlagInfoByUserID(dbMgr, text2, dbroleInfo.RoleID, dbroleInfo.ZoneID);
					if (num8 > 0)
					{
						DBWriter.UpdateVipLevelAwardFlagInfoByRoleID(dbMgr, num7, num8, dbroleInfo.ZoneID);
						dbroleInfo.VipAwardFlag = num8;
					}
					data2 = string.Format("{0}:{1}", 1, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						num7,
						num,
						num2,
						text3,
						num4,
						num5
					}));
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 102);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUnPreRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (dbuserInfo == null || null == dbroleInfo)
				{
					data2 = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.PreDelRoleMgr.IfInPreDeleteState(num))
				{
					data2 = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.PreDelRoleMgr.RemovePreDeleteRole(dbuserInfo, dbroleInfo))
				{
					data2 = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("恢复预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 99);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 99);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessPreRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				DateTime now = DateTime.Now;
				string data2;
				if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(num))
				{
					data2 = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 98);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				bool userRole = DBQuery.GetUserRole(dbMgr, userID, num);
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (dbroleInfo != null && dbroleInfo.ZhanDuiZhiWu > 0)
				{
					data2 = string.Format("{0}:{1}", -4029, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (userRole)
				{
					flag = DBWriter.PreRemoveRole(dbMgr, num, now);
				}
				if (!flag || dbuserInfo == null || null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("预删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 98);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					int num2 = dbuserInfo.ListRoleIDs.IndexOf(num);
					if (num2 >= 0 && num2 < dbuserInfo.ListRoleIDs.Count)
					{
						dbuserInfo.ListRolePreRemoveTime[num2] = now.ToString();
					}
				}
				GameDBManager.PreDelRoleMgr.AddPreDeleteRole(num, now);
				int num3 = GameDBManager.PreDelRoleMgr.CalcPreDeleteRoleLeftSeconds(now.ToString());
				data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 98);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveRoleCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (dbroleInfo != null && dbroleInfo.ZhanDuiZhiWu > 0)
				{
					data2 = string.Format("{0}:{1}", -4029, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				bool userRole = DBQuery.GetUserRole(dbMgr, userID, num);
				if (userRole)
				{
					flag = DBWriter.RemoveRole(dbMgr, num);
				}
				if (!flag || dbuserInfo == null || null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", -1, 0);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 103);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GameDBManager.PreDelRoleMgr.HandleDeleteRole(dbuserInfo, dbroleInfo);
				data2 = string.Format("{0}:{1}", num, dbroleInfo.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, 103);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessInitGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				int num = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				string channel = array[3];
				RoleDataEx roleDataEx = new RoleDataEx();
				bool flag = false;
				if (DBQuery.IsBlackUserID(dbMgr, text2))
				{
					flag = true;
					roleDataEx.RoleID = -70;
					LogManager.WriteLog(LogTypes.Error, string.Format("用户被禁止登陆，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				if (null == dbuserInfo)
				{
					flag = true;
					roleDataEx.RoleID = -2;
					LogManager.WriteLog(LogTypes.Error, string.Format("获取用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
				}
				else
				{
					bool flag2 = false;
					foreach (int num3 in dbuserInfo.ListRoleIDs)
					{
						if (num3 == num)
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						flag = true;
						roleDataEx.RoleID = -2;
						LogManager.WriteLog(LogTypes.Error, string.Format("获取用户角色数据失败，CMD={0}, UserID={1}, RoleID={2}", (TCPGameServerCmds)nID, text2, num), null, true);
					}
					else
					{
						lock (dbuserInfo)
						{
							roleDataEx.UserMoney = dbuserInfo.Money;
							roleDataEx.PushMessageID = dbuserInfo.PushMessageID;
						}
						if (num2 > 0)
						{
							SingletonTemplate<RoleMapper>.Instance().SetTempRoleID(num, num2);
						}
					}
				}
				if (dbuserInfo != null && !flag)
				{
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null == dbroleInfo)
					{
						roleDataEx.RoleID = -1;
						LogManager.WriteLog(LogTypes.Error, string.Format("获取用户数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					else if (BanManager.IsBanRoleName(Global.FormatRoleName(dbroleInfo)) > 0 || BanManager.IsBanRoleName(dbroleInfo.RoleID + "$rid") > 0)
					{
						roleDataEx.RoleID = -10;
						LogManager.WriteLog(LogTypes.Error, string.Format("被游戏管理员禁止登陆，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					else if (dbroleInfo.BanLogin > 0)
					{
						roleDataEx.RoleID = -10;
						LogManager.WriteLog(LogTypes.Error, string.Format("被游戏管理员禁止登陆，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					else if (GameDBManager.PreDelRoleMgr.IfInPreDeleteState(num))
					{
						roleDataEx.RoleID = -11;
						LogManager.WriteLog(LogTypes.Error, string.Format("角色处在预删除状态，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					else
					{
						dbroleInfo.Channel = channel;
						dbroleInfo.LastTime = TimeUtil.NOW();
						if (dbroleInfo.IsFlashPlayer == 1)
						{
							lock (dbuserInfo)
							{
								int index = -1;
								for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
								{
									if (dbuserInfo.ListRoleIDs[i] == dbroleInfo.RoleID)
									{
										index = i;
										break;
									}
								}
								DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, num);
								dbroleInfo.Experience = 0L;
								dbroleInfo.MainTaskID = 0;
								dbroleInfo.MainQuickBarKeys = "";
								DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, num);
								dbroleInfo.Level = 1;
								dbuserInfo.ListRoleLevels[index] = 1;
								DBWriter.UpdateRoleGoodsForFlashPlayerWhenLogOut(dbMgr, num);
								dbroleInfo.GoodsDataList = new List<GoodsData>();
								DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, num);
								dbroleInfo.DoingTaskList = new List<TaskData>();
								dbroleInfo.OldTasks = new List<OldTaskData>();
							}
						}
						CacheManager.AddRoleMiniInfo((long)num, dbroleInfo.ZoneID, dbroleInfo.UserID);
						Global.DBRoleInfo2RoleDataEx(dbroleInfo, roleDataEx);
						Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
						GlobalEventSource.getInstance().fireEvent(new PlayerLoginEventObject(dbroleInfo));
						roleDataEx.userMiniData = dbuserInfo.GetUserMiniData(text2, num, dbroleInfo.ZoneID);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRoleOnLineGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int serverLineID = Convert.ToInt32(array[1]);
				int loginNum = Convert.ToInt32(array[2]);
				string text2 = array[3];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayOfYear = DateTime.Now.DayOfYear;
				int loginDayID = 0;
				int loginDayNum = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.ServerLineID = serverLineID;
					dbroleInfo.LoginNum = loginNum;
					dbroleInfo.LastTime = DateTime.Now.Ticks / 10000L;
					if (dayOfYear != dbroleInfo.LoginDayID)
					{
						dbroleInfo.LoginDayNum++;
						dbroleInfo.LoginDayID = dayOfYear;
					}
					loginDayID = dbroleInfo.LoginDayID;
					loginDayNum = dbroleInfo.LoginDayNum;
					dbroleInfo.LastIP = text2;
				}
				DBWriter.UpdateRoleLoginInfo(dbMgr, num, loginNum, loginDayID, loginDayNum, dbroleInfo.UserID, dbroleInfo.ZoneID, text2);
				RoleOnlineManager.UpdateRoleOnlineTicks(num);
				DBWriter.InsertCityInfo(dbMgr, text2, dbroleInfo.UserID);
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateAccountActiveCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserActiveInfo.getInstance().UpdateAccountActiveInfo(dbMgr, array[0]);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRoleHeartGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RoleOnlineManager.UpdateRoleOnlineTicks(num);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRoleOffLineGameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string ip = array[2];
				int activeVal = Convert.ToInt32(array[3]);
				long logoutServerTicks = 0L;
				if (array.Length >= 5)
				{
					logoutServerTicks = Convert.ToInt64(array[4]);
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int pkMode = 0;
				int horseDbID = 0;
				int petDbID = 0;
				int onlineSecs = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.ServerLineID = -1;
					dbroleInfo.LogOffTime = DateTime.Now.Ticks / 10000L;
					pkMode = dbroleInfo.PKMode;
					horseDbID = dbroleInfo.HorseDbID;
					petDbID = dbroleInfo.PetDbID;
					onlineSecs = Math.Min((int)((dbroleInfo.LogOffTime - dbroleInfo.LastTime) / 1000L), 86400);
				}
				dbroleInfo.RankValue.Clear();
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null != dbuserInfo)
				{
					lock (dbuserInfo)
					{
						dbuserInfo.LogoutServerTicks = logoutServerTicks;
					}
				}
				DBWriter.UpdateRoleLogOff(dbMgr, num, dbroleInfo.UserID, dbroleInfo.ZoneID, ip, onlineSecs);
				DBWriter.UpdatePKMode(dbMgr, num, pkMode);
				DBWriter.UpdateRoleHorse(dbMgr, num, horseDbID);
				DBWriter.UpdateRolePet(dbMgr, num, petDbID);
				RoleOnlineManager.RemoveRoleOnlineTicks(num);
				DBWriter.UpdateCityInfoLogoutTime(dbMgr, ip, dbroleInfo.UserID, onlineSecs, activeVal);
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetServerListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误 , CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				string userID = array[1];
				int num = Convert.ToInt32(array[2]);
				int rolesCount = 0;
				DBUserInfo dbuserInfo = dbMgr.dbUserMgr.FindDBUserInfo(userID);
				if (null != dbuserInfo)
				{
					lock (dbuserInfo)
					{
						rolesCount = dbuserInfo.ListRoleNames.Count;
					}
				}
				ServerListData serverListData = new ServerListData
				{
					RetCode = 0,
					RolesCount = rolesCount,
					LineDataList = null
				};
				List<LineData> list = new List<LineData>();
				if (num != 20111128)
				{
					serverListData.RetCode = -1;
				}
				else
				{
					List<LineItem> lineItemList = LineManager.GetLineItemList();
					for (int i = 0; i < lineItemList.Count; i++)
					{
						list.Add(Global.LineItemToLineData(lineItemList[i]));
					}
					serverListData.LineDataList = list;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ServerListData>(serverListData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessOnlineServerHeartCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误 , CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int onlineNum = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				if (num2 <= 0)
				{
					UserOnlineManager.ClearUserIDsByServerLineID(num);
				}
				LineManager.UpdateLineHeart(client, num, onlineNum, "");
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetServerIdCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] array;
			try
			{
				array = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			array = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetServerPTIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] array;
			try
			{
				array = DataHelper.ObjectToBytes<int>(GameDBManager.PTID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			array = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetServerDayDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] array;
			try
			{
				int num = DataHelper.BytesToObject<int>(data, 0, count);
				ServerDayData serverDayData = new ServerDayData();
				try
				{
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("select dayid,cdate,worldlevel from t_server_days where dayid={0}", num);
						using (MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]))
						{
							if (mySQLDataReader.Read())
							{
								serverDayData.Dayid = Convert.ToInt32(mySQLDataReader["dayid"].ToString());
								serverDayData.CDate = mySQLDataReader["cdate"].ToString();
								serverDayData.WorldLevel = Convert.ToInt32(mySQLDataReader["worldlevel"].ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				array = DataHelper.ObjectToBytes<ServerDayData>(serverDayData);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			array = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSetServerDayDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			byte[] array;
			try
			{
				ServerDayData serverDayData = DataHelper.BytesToObject<ServerDayData>(data, 0, count);
				try
				{
					if (null != serverDayData)
					{
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("replace into t_server_days(dayid,cdate,worldlevel) values({0},'{1}',{2})", serverDayData.Dayid, serverDayData.CDate, serverDayData.WorldLevel);
							myDbConnection.ExecuteNonQuery(sql, 0);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				array = DataHelper.ObjectToBytes<int>(1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			array = DataHelper.ObjectToBytes<int>(GameDBManager.ZoneID);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessNewTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int npcID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				int num4 = Convert.ToInt32(array[4]);
				DateTime now = DateTime.Now;
				string addtime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long num5 = now.Ticks / 10000L;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num6 = DBWriter.NewTask(dbMgr, num, npcID, num2, addtime, num3, num4);
				string data2;
				if (num6 < 0)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num5,
						num6
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("添加任务失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.DoingTaskList)
						{
							dbroleInfo.DoingTaskList = new List<TaskData>();
						}
						dbroleInfo.DoingTaskList.Add(new TaskData
						{
							DbID = num6,
							DoingTaskID = num2,
							DoingTaskVal1 = 0,
							DoingTaskVal2 = 0,
							DoingTaskFocus = num3,
							AddDateTime = num5,
							StarLevel = num4
						});
					}
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num5,
						num6
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdatePosCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				int num5 = Convert.ToInt32(array[4]);
				string text2 = "";
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long num6 = DateTime.Now.Ticks / 10000L;
				text2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num2,
					num3,
					num4,
					num5
				});
				bool flag = true;
				lock (dbroleInfo)
				{
					dbroleInfo.Position = text2;
				}
				bool flag3 = true;
				if (flag)
				{
					flag3 = DBWriter.UpdateRolePosition(dbMgr, num, text2);
				}
				if (!flag3)
				{
					text2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色位置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					text2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateExpLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				long experience = Convert.ToInt64(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleExpLevel(dbMgr, num, num2, experience))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色经验和级别失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					string text2 = "";
					lock (dbroleInfo)
					{
						dbroleInfo.Level = num2;
						dbroleInfo.Experience = experience;
						text2 = dbroleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					if (text2 != "")
					{
						DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
						if (null != dbuserInfo)
						{
							lock (dbuserInfo)
							{
								for (int i = 0; i < dbuserInfo.ListRoleLevels.Count; i++)
								{
									if (dbuserInfo.ListRoleIDs[i] == num)
									{
										dbuserInfo.ListRoleLevels[i] = num2;
									}
								}
							}
						}
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateInterPowerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int interPower = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = true;
				lock (dbroleInfo)
				{
					dbroleInfo.InterPower = interPower;
				}
				bool flag3 = true;
				if (flag)
				{
					flag3 = DBWriter.UpdateRoleInterPower(dbMgr, num, interPower);
				}
				string data2;
				if (!flag3)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色内力失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateMoney1Cmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleMoney1(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.Money1 = num2;
					}
					Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					data2 = string.Format("{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 19)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int quality = Convert.ToInt32(array[3]);
				string props = array[4];
				int num3 = Convert.ToInt32(array[5]);
				int binding = Convert.ToInt32(array[6]);
				int num4 = Convert.ToInt32(array[7]);
				string text2 = array[8];
				int num5 = Convert.ToInt32(array[9]);
				string text3 = array[10];
				text3 = text3.Replace("$", ":");
				string text4 = array[11];
				text4 = text4.Replace("$", ":");
				int addPropIndex = Convert.ToInt32(array[12]);
				int bornIndex = Convert.ToInt32(array[13]);
				int lucky = Convert.ToInt32(array[14]);
				int strong = Convert.ToInt32(array[15]);
				int num6 = Convert.ToInt32(array[16]);
				int num7 = Convert.ToInt32(array[17]);
				int num8 = Convert.ToInt32(array[18]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num9 = DBWriter.NewGoods(dbMgr, num, goodsID, num2, quality, props, num3, binding, num4, text2, num5, text3, text4, addPropIndex, bornIndex, lucky, strong, num6, num7, num8);
				string data2;
				if (num9 < 0)
				{
					data2 = string.Format("{0}:{1}", num9, text);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色添加新物品失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.GoodsDataList)
						{
							dbroleInfo.GoodsDataList = new List<GoodsData>();
						}
						dbroleInfo.GoodsDataList.Add(new GoodsData
						{
							Id = num9,
							GoodsID = goodsID,
							Using = 0,
							Forge_level = num3,
							Starttime = text3,
							Endtime = text4,
							Site = num4,
							Quality = quality,
							Props = props,
							GCount = num2,
							Binding = binding,
							Jewellist = text2,
							BagIndex = num5,
							AddPropIndex = addPropIndex,
							BornIndex = bornIndex,
							Lucky = lucky,
							Strong = strong,
							ExcellenceInfo = num6,
							AppendPropLev = num7,
							ChangeLifeLevForEquip = num8
						});
						if (-1000 == num4)
						{
							dbroleInfo.MyPortableBagData.GoodsUsedGridNum++;
						}
					}
					data2 = string.Format("{0}:{1}", num9, text);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 23)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (null == Global.GetGoodsDataByDbID(dbroleInfo, num2))
				{
					data2 = string.Format("{0}:{1}", num2, -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateGoods(dbMgr, num2, array, 2);
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num2, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新物品数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null != dbroleInfo.GoodsDataList)
						{
							for (int i = 0; i < dbroleInfo.GoodsDataList.Count; i++)
							{
								if (dbroleInfo.GoodsDataList[i].Id == num2)
								{
									int num4 = DataHelper.ConvertToInt32(array[9], dbroleInfo.GoodsDataList[i].GCount);
									if (num4 > 0)
									{
										int num5 = DataHelper.ConvertToInt32(array[6], dbroleInfo.GoodsDataList[i].Site);
										if (dbroleInfo.GoodsDataList[i].Site == 0 && -1000 == num5)
										{
											dbroleInfo.MyPortableBagData.GoodsUsedGridNum++;
										}
										else if (dbroleInfo.GoodsDataList[i].Site != 0 || num5 != -1)
										{
											if (dbroleInfo.GoodsDataList[i].Site == -1000 && num5 == 0)
											{
												dbroleInfo.MyPortableBagData.GoodsUsedGridNum--;
											}
											else if (dbroleInfo.GoodsDataList[i].Site != -1 || num5 != 0)
											{
												if (dbroleInfo.GoodsDataList[i].Site == 15000 && 15001 == num5)
												{
													dbroleInfo.RebornGirdData.GoodsUsedGridNum++;
												}
												else if (dbroleInfo.GoodsDataList[i].Site != 15000 || num5 != -1)
												{
													if (dbroleInfo.GoodsDataList[i].Site == 15001 && num5 == 15000)
													{
														dbroleInfo.RebornGirdData.GoodsUsedGridNum--;
													}
													else if (dbroleInfo.GoodsDataList[i].Site == -1 && num5 == 15000)
													{
													}
												}
											}
										}
										dbroleInfo.GoodsDataList[i].Using = DataHelper.ConvertToInt32(array[2], dbroleInfo.GoodsDataList[i].Using);
										dbroleInfo.GoodsDataList[i].Forge_level = DataHelper.ConvertToInt32(array[3], dbroleInfo.GoodsDataList[i].Forge_level);
										dbroleInfo.GoodsDataList[i].Starttime = DataHelper.ConvertToStr(array[4], dbroleInfo.GoodsDataList[i].Starttime);
										dbroleInfo.GoodsDataList[i].Endtime = DataHelper.ConvertToStr(array[5], dbroleInfo.GoodsDataList[i].Endtime);
										dbroleInfo.GoodsDataList[i].Site = num5;
										dbroleInfo.GoodsDataList[i].Quality = DataHelper.ConvertToInt32(array[7], dbroleInfo.GoodsDataList[i].Quality);
										dbroleInfo.GoodsDataList[i].Props = DataHelper.ConvertToStr(array[8], dbroleInfo.GoodsDataList[i].Props);
										dbroleInfo.GoodsDataList[i].GCount = num4;
										dbroleInfo.GoodsDataList[i].Jewellist = DataHelper.ConvertToStr(array[10], dbroleInfo.GoodsDataList[i].Jewellist);
										dbroleInfo.GoodsDataList[i].BagIndex = DataHelper.ConvertToInt32(array[11], dbroleInfo.GoodsDataList[i].BagIndex);
										dbroleInfo.GoodsDataList[i].SaleMoney1 = DataHelper.ConvertToInt32(array[12], dbroleInfo.GoodsDataList[i].SaleMoney1);
										dbroleInfo.GoodsDataList[i].SaleYuanBao = DataHelper.ConvertToInt32(array[13], dbroleInfo.GoodsDataList[i].SaleYuanBao);
										dbroleInfo.GoodsDataList[i].SaleYinPiao = DataHelper.ConvertToInt32(array[14], dbroleInfo.GoodsDataList[i].SaleYinPiao);
										dbroleInfo.GoodsDataList[i].Binding = DataHelper.ConvertToInt32(array[15], dbroleInfo.GoodsDataList[i].Binding);
										dbroleInfo.GoodsDataList[i].AddPropIndex = DataHelper.ConvertToInt32(array[16], dbroleInfo.GoodsDataList[i].AddPropIndex);
										dbroleInfo.GoodsDataList[i].BornIndex = DataHelper.ConvertToInt32(array[17], dbroleInfo.GoodsDataList[i].BornIndex);
										dbroleInfo.GoodsDataList[i].Lucky = DataHelper.ConvertToInt32(array[18], dbroleInfo.GoodsDataList[i].Lucky);
										dbroleInfo.GoodsDataList[i].Strong = DataHelper.ConvertToInt32(array[19], dbroleInfo.GoodsDataList[i].Strong);
										dbroleInfo.GoodsDataList[i].ExcellenceInfo = DataHelper.ConvertToInt32(array[20], dbroleInfo.GoodsDataList[i].ExcellenceInfo);
										dbroleInfo.GoodsDataList[i].AppendPropLev = DataHelper.ConvertToInt32(array[21], dbroleInfo.GoodsDataList[i].AppendPropLev);
										dbroleInfo.GoodsDataList[i].ChangeLifeLevForEquip = DataHelper.ConvertToInt32(array[22], dbroleInfo.GoodsDataList[i].ChangeLifeLevForEquip);
									}
									else
									{
										if (GameDBManager.Flag_t_goods_delete_immediately)
										{
											DBWriter.MoveGoodsDataToBackupTable(dbMgr, num2);
										}
										if (dbroleInfo.GoodsDataList[i].Site == -1000)
										{
											dbroleInfo.MyPortableBagData.GoodsUsedGridNum--;
										}
										dbroleInfo.GoodsDataList.RemoveAt(i);
									}
									break;
								}
							}
						}
					}
					data2 = string.Format("{0}:{1}", num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int dbID = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateTask(dbMgr, dbID, array, 3);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色更新任务数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null != dbroleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbroleInfo.DoingTaskList.Count; i++)
							{
								if (dbroleInfo.DoingTaskList[i].DoingTaskID == num2)
								{
									dbroleInfo.DoingTaskList[i].DoingTaskFocus = DataHelper.ConvertToInt32(array[3], dbroleInfo.DoingTaskList[i].DoingTaskFocus);
									dbroleInfo.DoingTaskList[i].DoingTaskVal1 = DataHelper.ConvertToInt32(array[4], dbroleInfo.DoingTaskList[i].DoingTaskVal1);
									dbroleInfo.DoingTaskList[i].DoingTaskVal2 = DataHelper.ConvertToInt32(array[5], dbroleInfo.DoingTaskList[i].DoingTaskVal2);
									break;
								}
							}
						}
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessCompleteTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int npcID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int dbID = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.CompleteTask(dbMgr, num, npcID, num2, dbID, num3))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("角色完成任务数据失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					bool flag = false;
					lock (dbroleInfo)
					{
						if (num3 > 0 && num2 > dbroleInfo.MainTaskID)
						{
							dbroleInfo.MainTaskID = num2;
							flag = true;
						}
						if (null != dbroleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbroleInfo.DoingTaskList.Count; i++)
							{
								if (dbroleInfo.DoingTaskList[i].DoingTaskID == num2)
								{
									dbroleInfo.DoingTaskList.RemoveAt(i);
									break;
								}
							}
						}
						if (null == dbroleInfo.OldTasks)
						{
							dbroleInfo.OldTasks = new List<OldTaskData>();
						}
						int num4 = -1;
						for (int i = 0; i < dbroleInfo.OldTasks.Count; i++)
						{
							if (dbroleInfo.OldTasks[i].TaskID == num2)
							{
								num4 = i;
								break;
							}
						}
						if (num4 >= 0)
						{
							dbroleInfo.OldTasks[num4].DoCount++;
						}
						else
						{
							dbroleInfo.OldTasks.Add(new OldTaskData
							{
								TaskID = num2,
								DoCount = 1
							});
						}
					}
					if (flag && num3 > 0)
					{
						DBWriter.UpdateRoleMainTaskID(dbMgr, num, num2);
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
					if (num3 > 0)
					{
						Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFriendsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<FriendData> list = null;
				list = new List<FriendData>();
				int i;
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.FriendDataList)
					{
						dbroleInfo.FriendDataList = new List<FriendData>();
					}
					for (i = 0; i < dbroleInfo.FriendDataList.Count; i++)
					{
						list.Add(new FriendData
						{
							DbID = dbroleInfo.FriendDataList[i].DbID,
							OtherRoleID = dbroleInfo.FriendDataList[i].OtherRoleID,
							FriendType = dbroleInfo.FriendDataList[i].FriendType
						});
					}
				}
				List<FriendData> list2 = new List<FriendData>();
				i = 0;
				while (i < list.Count)
				{
					FriendData friendData = list[i];
					DBRoleInfo dbroleInfo2 = dbMgr.FindDBRoleInfo(ref friendData.OtherRoleID);
					if (null != dbroleInfo2)
					{
						list[i].OtherRoleName = Global.FormatRoleName(dbroleInfo2);
						list[i].OtherLevel = dbroleInfo2.Level;
						list[i].Occupation = dbroleInfo2.Occupation;
						list[i].OnlineState = Global.GetRoleOnlineState(dbroleInfo2);
						list[i].Position = dbroleInfo2.Position;
						list[i].FriendChangeLifeLev = dbroleInfo2.ChangeLifeCount;
						list[i].FriendCombatForce = dbroleInfo2.CombatForce;
						list[i].SpouseId = ((dbroleInfo2.MyMarriageData != null) ? dbroleInfo2.MyMarriageData.nSpouseID : 0);
						list[i].ZhanDuiID = dbroleInfo2.ZhanDuiID;
						goto IL_427;
					}
					if (DBQuery.GetFriendData(dbMgr, friendData))
					{
						goto IL_427;
					}
					if (!DBWriter.RemoveFriend(dbMgr, friendData.DbID, num))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("删除好友时失败，CMD={0}, RoleID={1}, friendDbID={2}", (TCPGameServerCmds)nID, num, friendData.DbID), null, true);
					}
					else
					{
						lock (dbroleInfo)
						{
							if (null == dbroleInfo.FriendDataList)
							{
								dbroleInfo.FriendDataList = new List<FriendData>();
							}
							int num2 = -1;
							for (int j = 0; j < dbroleInfo.FriendDataList.Count; j++)
							{
								if (dbroleInfo.FriendDataList[j].DbID == friendData.DbID)
								{
									num2 = j;
									break;
								}
							}
							if (num2 >= 0)
							{
								dbroleInfo.FriendDataList.RemoveAt(num2);
							}
						}
					}
					string gmCmd = string.Format("-removefriend {0} {1}", num, friendData.DbID);
					ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
					IL_439:
					i++;
					continue;
					IL_427:
					list2.Add(list[i]);
					goto IL_439;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<FriendData>>(list2, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddFriendCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dbID = Convert.ToInt32(array[0]);
				int num = Convert.ToInt32(array[1]);
				string text2 = array[2];
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				List<FriendData> list = new List<FriendData>();
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.FriendDataList)
					{
						for (int i = 0; i < dbroleInfo.FriendDataList.Count; i++)
						{
							list.Add(dbroleInfo.FriendDataList[i]);
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					string text3 = string.Empty;
					DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref list[i].OtherRoleID);
					if (null != dbroleInfo2)
					{
						text3 = Global.FormatRoleName(dbroleInfo2);
					}
					if (!string.IsNullOrEmpty(text3) && text3 == text2 && list[i].FriendType == num2)
					{
						flag = true;
					}
					if (list[i].FriendType == 0)
					{
						num3++;
					}
					else if (list[i].FriendType == 1)
					{
						num4++;
					}
					else
					{
						num5++;
					}
				}
				bool flag3 = !flag;
				if (flag3)
				{
					if (num2 == 0)
					{
						if (num3 >= 50)
						{
							flag3 = false;
						}
					}
					else if (num2 == 1)
					{
						if (num4 >= 20)
						{
							flag3 = false;
						}
					}
					else if (num5 >= 20)
					{
						flag3 = false;
					}
				}
				FriendData instance;
				if (flag3)
				{
					int num6 = dbMgr.DBRoleMgr.FindDBRoleID(text2);
					if (-1 == num6)
					{
						instance = new FriendData
						{
							DbID = -10000,
							OtherRoleID = 0,
							OtherRoleName = "",
							OtherLevel = 1,
							Occupation = 0,
							OnlineState = 0,
							Position = "",
							FriendType = num2
						};
						LogManager.WriteLog(LogTypes.Error, string.Format("添加好友找有时查找对方角色ID失败，CMD={0}, RoleID={1}, OtherName={2}", (TCPGameServerCmds)nID, num, text2), null, true);
					}
					else
					{
						int num7 = DBWriter.AddFriend(dbMgr, dbID, num, num6, num2);
						if (num7 < 0)
						{
							instance = new FriendData
							{
								DbID = num7,
								OtherRoleID = 0,
								OtherRoleName = "",
								OtherLevel = 1,
								Occupation = 0,
								OnlineState = 0,
								Position = "",
								FriendType = 0
							};
							LogManager.WriteLog(LogTypes.Error, string.Format("添加好友到数据库失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						}
						else
						{
							lock (dbroleInfo)
							{
								if (null == dbroleInfo.FriendDataList)
								{
									dbroleInfo.FriendDataList = new List<FriendData>();
								}
								int num8 = -1;
								for (int i = 0; i < dbroleInfo.FriendDataList.Count; i++)
								{
									if (dbroleInfo.FriendDataList[i].DbID == num7)
									{
										num8 = i;
										break;
									}
								}
								if (num8 >= 0)
								{
									dbroleInfo.FriendDataList[num8].OtherRoleID = num6;
									dbroleInfo.FriendDataList[num8].FriendType = num2;
								}
								else
								{
									dbroleInfo.FriendDataList.Add(new FriendData
									{
										DbID = num7,
										OtherRoleID = num6,
										FriendType = num2
									});
								}
							}
							DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num6);
							if (null == dbroleInfo2)
							{
								instance = new FriendData
								{
									DbID = -10000,
									OtherRoleID = 0,
									OtherRoleName = "",
									OtherLevel = 1,
									Occupation = 0,
									OnlineState = 0,
									Position = "",
									FriendType = num2
								};
							}
							else
							{
								instance = new FriendData
								{
									DbID = num7,
									OtherRoleID = dbroleInfo2.RoleID,
									OtherRoleName = Global.FormatRoleName(dbroleInfo2),
									OtherLevel = dbroleInfo2.Level,
									Occupation = dbroleInfo2.Occupation,
									OnlineState = Global.GetRoleOnlineState(dbroleInfo2),
									Position = dbroleInfo2.Position,
									FriendType = num2,
									FriendChangeLifeLev = dbroleInfo2.ChangeLifeCount,
									FriendCombatForce = dbroleInfo2.CombatForce,
									SpouseId = ((dbroleInfo2.MyMarriageData != null) ? dbroleInfo2.MyMarriageData.nSpouseID : 0)
								};
							}
						}
					}
				}
				else
				{
					instance = new FriendData
					{
						DbID = (flag ? -10002 : -10001),
						OtherRoleID = 0,
						OtherRoleName = "",
						OtherLevel = 1,
						Occupation = 0,
						OnlineState = 0,
						Position = "",
						FriendType = num2
					};
					LogManager.WriteLog(LogTypes.Error, string.Format("添加好友时已经存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FriendData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveFriendCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num2), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.RemoveFriend(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除好友时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num2), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.FriendDataList)
						{
							dbroleInfo.FriendDataList = new List<FriendData>();
						}
						int num3 = -1;
						for (int i = 0; i < dbroleInfo.FriendDataList.Count; i++)
						{
							if (dbroleInfo.FriendDataList[i].DbID == num)
							{
								num3 = i;
								break;
							}
						}
						if (num3 >= 0)
						{
							dbroleInfo.FriendDataList.RemoveAt(num3);
						}
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdatePKModeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!true)
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色PK模式时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.PKMode = num2;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdatePKValCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdatePKValues(dbMgr, num, num2, num3))
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						-1
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色PK值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.PKValue = num2;
						dbroleInfo.PKPoint = num3;
					}
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAbandonTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int dbID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.DeleteTask(dbMgr, num, num2, dbID))
				{
					data2 = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("删除任务时失败，CMD={0}, RoleID={1}, TaskID={2}", (TCPGameServerCmds)nID, num, num2), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null != dbroleInfo.DoingTaskList)
						{
							for (int i = 0; i < dbroleInfo.DoingTaskList.Count; i++)
							{
								if (dbroleInfo.DoingTaskList[i].DoingTaskID == num2)
								{
									dbroleInfo.DoingTaskList.RemoveAt(i);
									break;
								}
							}
						}
					}
					data2 = string.Format("{0}", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGMSetTaskCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				List<int> list = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (list != null && list.Count >= 2)
				{
					int num = list[0];
					int num2 = list[list.Count - 1];
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null != dbroleInfo)
					{
						lock (dbroleInfo)
						{
							DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, num);
							DBWriter.GMSetTask(dbMgr, num, num2, list);
							DBWriter.UpdateRoleMainTaskID(dbMgr, num, num2);
							dbroleInfo.MainTaskID = num2;
							dbroleInfo.OldTasks = new List<OldTaskData>();
							dbroleInfo.DoingTaskList = new List<TaskData>();
							for (int i = 1; i < list.Count; i++)
							{
								dbroleInfo.OldTasks.Add(new OldTaskData
								{
									TaskID = list[i],
									DoCount = 1
								});
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			byte[] array = DataHelper.ObjectToBytes<int>(0);
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessModKeysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[2];
				string data2;
				if (!DBWriter.UpdateRoleKeys(dbMgr, num, num2, text2))
				{
					data2 = string.Format("{0}", -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色映射键时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (num2 == 0)
						{
							dbroleInfo.MainQuickBarKeys = text2;
						}
						else
						{
							dbroleInfo.OtherQuickBarKeys = text2;
						}
					}
					data2 = string.Format("{0}", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateUserMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2 && array.Length != 3 && array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = 0;
				string text2 = "";
				if (array.Length >= 3)
				{
					num3 = Global.SafeConvertToInt32(array[2], 10);
				}
				if (array.Length >= 4)
				{
					text2 = array[3];
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				string data2;
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = num3;
				if (num4 == 23)
				{
					DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
					DateTime dateTime2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
					int num5 = 0;
					string keystr = text2;
					string s = "";
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 23, keystr, out num5, out s);
					if (num5 > 0)
					{
						int offsetDay = Global.GetOffsetDay(DateTime.Now);
						if (Global.GetOffsetDay(DateTime.Parse(s)) == offsetDay)
						{
							data2 = string.Format("{0}:{1}", num, -5);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					num5++;
					lock (dbroleInfo)
					{
						int num6 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num3, keystr, (long)num5, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num6 < 0)
						{
							num6 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num3, keystr, (long)num5, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						}
						if (num6 < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("更新玩家合服充值返利领取记录失败！！！！！！！！，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						}
					}
				}
				bool flag2 = false;
				int num7 = 0;
				lock (dbuserInfo)
				{
					dbuserInfo.Money += num2;
					num7 = dbuserInfo.Money;
					if (flag2)
					{
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (num2 != 0)
					{
						if (!DBWriter.UpdateUserInfo(dbMgr, dbuserInfo))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的元宝失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
							data2 = string.Format("{0}:{1}", num, -4);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				int num8 = 0;
				int num9 = 0;
				DBQuery.QueryUserMoneyByUserID(dbMgr, dbuserInfo.UserID, out num8, out num9);
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				data2 = string.Format("{0}:{1}:{2}", num, num7, num9);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateUserYinLiangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.YinLiang += num2;
					num3 = dbroleInfo.YinLiang;
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateRoleYinLiang(dbMgr, num, num3))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色银两失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", num, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessMoveGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3 && array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int site = 0;
				if (array.Length == 4)
				{
					site = Convert.ToInt32(array[3]);
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时查找角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						-1
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时查找物品拥有者角色失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num2), null, true);
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						-2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (null == Global.GetGoodsDataByDbID(dbroleInfo2, num3))
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						-1000
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = DBWriter.MoveGoods(dbMgr, num, num3, num2, site);
				if (num4 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("移动物品时修改数据库失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GoodsData goodsData = null;
				lock (dbroleInfo2)
				{
					if (null != dbroleInfo2.GoodsDataList)
					{
						for (int i = 0; i < dbroleInfo2.GoodsDataList.Count; i++)
						{
							if (dbroleInfo2.GoodsDataList[i].Id == num3)
							{
								goodsData = dbroleInfo2.GoodsDataList[i];
								dbroleInfo2.GoodsDataList.Remove(goodsData);
								goodsData.Site = site;
								break;
							}
						}
					}
				}
				if (null == goodsData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						-1000
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.GoodsDataList)
					{
						dbroleInfo.GoodsDataList = new List<GoodsData>();
					}
					dbroleInfo.GoodsDataList.Add(goodsData);
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					num3,
					0
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateLeftFightSecsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleLeftFightSecs(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色的剩余挂机时间时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.LeftFightSeconds = num2;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryNameByIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				int num2 = Convert.ToInt32(array[2]);
				int num3 = -1;
				if (num > 0)
				{
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null == dbroleInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num3 = dbroleInfo.ServerLineID;
				}
				int num4 = -1;
				int num5 = dbMgr.DBRoleMgr.FindDBRoleID(text2);
				if (num5 == -1)
				{
					DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(text2);
					if (dbroleInfo2 != null)
					{
						num5 = dbroleInfo2.RoleID;
					}
				}
				if (-1 != num5)
				{
					DBRoleInfo dbroleInfo3 = dbMgr.GetDBRoleInfo(ref num5);
					if (null != dbroleInfo3)
					{
						int roleOnlineState = Global.GetRoleOnlineState(dbroleInfo3);
						if (1 == roleOnlineState)
						{
							num4 = 0;
							if (dbroleInfo3.ServerLineID != num3)
							{
								num4 = dbroleInfo3.ServerLineID;
							}
						}
					}
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num,
					text2,
					num2,
					num5,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryUserMoneyByNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				string data2;
				if (userID == "")
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						userID,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBQuery.GetUserInputMoney(dbMgr, userID, dbroleInfo.ZoneID, "2000-01-01 00:00:00", "2050-01-01 00:00:00");
				if (num2 < 0)
				{
					num2 = 0;
				}
				int num3 = Global.TransMoneyToYuanBao(num2);
				int num4 = num3;
				int num5 = num2;
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					userID,
					num4,
					num5
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSpriteChatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (Global.ProcessGMMsg(array))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (array.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[8]);
				List<LineItem> lineItemList = LineManager.GetLineItemList();
				if (null != lineItemList)
				{
					for (int i = 0; i < lineItemList.Count; i++)
					{
						if (lineItemList[i].LineID != num)
						{
							if (num == 0)
							{
								ChatMsgManager.AddChatMsg(lineItemList[i].LineID, text);
							}
							else if (num == -1000)
							{
								if (TCPManager.CurrentClient != null && TCPManager.CurrentClient.LineId != lineItemList[i].LineID)
								{
									ChatMsgManager.AddChatMsg(lineItemList[i].LineID, text);
								}
							}
							else if (lineItemList[i].LineID < 9000)
							{
								ChatMsgManager.AddChatMsg(lineItemList[i].LineID, text);
							}
						}
					}
				}
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetChatMsgListCmd(GameServerClient client, DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int onlineNum = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				if (num2 <= 0)
				{
					UserOnlineManager.ClearUserIDsByServerLineID(num);
				}
				LineManager.UpdateLineHeart(client, num, onlineNum, array[3]);
				tcpOutPacket = ChatMsgManager.GetWaitingChatMsg(pool, nID, num);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddHorseCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int horseID = Convert.ToInt32(array[1]);
				int bodyID = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string addtime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long addDateTime = now.Ticks / 10000L;
				HorseData horseData = null;
				int num2 = DBWriter.NewHorse(dbMgr, num, horseID, bodyID, addtime);
				if (num2 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的坐骑失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.HorsesDataList)
						{
							dbroleInfo.HorsesDataList = new List<HorseData>();
						}
						horseData = new HorseData
						{
							DbID = num2,
							HorseID = horseID,
							BodyID = bodyID,
							AddDateTime = addDateTime
						};
						dbroleInfo.HorsesDataList.Add(horseData);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horseData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddPetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int petID = Convert.ToInt32(array[1]);
				string petName = array[2];
				int petType = Convert.ToInt32(array[3]);
				string text2 = array[4];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string addtime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long addDateTime = now.Ticks / 10000L;
				PetData petData = null;
				int num2 = DBWriter.NewPet(dbMgr, num, petID, petName, petType, text2, addtime);
				if (num2 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的宠物失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.PetsDataList)
						{
							dbroleInfo.PetsDataList = new List<PetData>();
						}
						petData = new PetData
						{
							DbID = num2,
							PetID = petID,
							PetName = petName,
							PetType = petType,
							FeedNum = 0,
							ReAliveNum = 0,
							AddDateTime = addDateTime,
							PetProps = text2
						};
						dbroleInfo.PetsDataList.Add(petData);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PetData>(petData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetHorseListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<HorseData> instance = null;
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.HorsesDataList)
					{
						instance = dbroleInfo.HorsesDataList.GetRange(0, dbroleInfo.HorsesDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetOtherHorseListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<HorseData> instance = null;
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo2)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo2)
				{
					if (null != dbroleInfo2.HorsesDataList)
					{
						instance = dbroleInfo2.HorsesDataList.GetRange(0, dbroleInfo2.HorsesDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<HorseData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetPetListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<PetData> instance = null;
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.PetsDataList)
					{
						instance = dbroleInfo.PetsDataList.GetRange(0, dbroleInfo.PetsDataList.Count);
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<PetData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessModHorseCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 11)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				HorseData horseData = null;
				int num3 = DBWriter.UpdateHorse(dbMgr, num2, array, 2);
				if (num3 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新时坐骑失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null != dbroleInfo.HorsesDataList)
						{
							for (int i = 0; i < dbroleInfo.HorsesDataList.Count; i++)
							{
								if (dbroleInfo.HorsesDataList[i].DbID == num2)
								{
									int num4 = DataHelper.ConvertToInt32(array[2], 0);
									if (num4 <= 0)
									{
										dbroleInfo.HorsesDataList[i].HorseID = DataHelper.ConvertToInt32(array[3], dbroleInfo.HorsesDataList[i].HorseID);
										dbroleInfo.HorsesDataList[i].BodyID = DataHelper.ConvertToInt32(array[4], dbroleInfo.HorsesDataList[i].BodyID);
										dbroleInfo.HorsesDataList[i].PropsNum = DataHelper.ConvertToStr(array[5], dbroleInfo.HorsesDataList[i].PropsNum);
										dbroleInfo.HorsesDataList[i].PropsVal = DataHelper.ConvertToStr(array[6], dbroleInfo.HorsesDataList[i].PropsVal);
										dbroleInfo.HorsesDataList[i].JinJieFailedNum = DataHelper.ConvertToInt32(array[7], dbroleInfo.HorsesDataList[i].JinJieFailedNum);
										dbroleInfo.HorsesDataList[i].JinJieTempTime = DataHelper.ConvertToTicks(array[8], dbroleInfo.HorsesDataList[i].JinJieTempTime);
										dbroleInfo.HorsesDataList[i].JinJieTempNum = DataHelper.ConvertToInt32(array[9], dbroleInfo.HorsesDataList[i].JinJieTempNum);
										dbroleInfo.HorsesDataList[i].JinJieFailedDayID = DataHelper.ConvertToInt32(array[10], dbroleInfo.HorsesDataList[i].JinJieFailedDayID);
										horseData = dbroleInfo.HorsesDataList[i];
									}
									else
									{
										horseData = dbroleInfo.HorsesDataList[i];
										horseData.HorseID = -1;
										dbroleInfo.HorsesDataList.RemoveAt(i);
									}
									break;
								}
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(horseData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessModPetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 10)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				PetData petDataByDbID = Global.GetPetDataByDbID(dbroleInfo, num2);
				if (null != petDataByDbID)
				{
					int num3 = DBWriter.UpdatePet(dbMgr, num2, array, 2);
					if (num3 < 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新时宠物失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					}
					else
					{
						lock (dbroleInfo)
						{
							int num4 = DataHelper.ConvertToInt32(array[7], 0);
							if (num4 <= 0)
							{
								petDataByDbID.PetName = DataHelper.ConvertToStr(array[2], petDataByDbID.PetName);
								petDataByDbID.PetType = DataHelper.ConvertToInt32(array[3], petDataByDbID.PetType);
								petDataByDbID.FeedNum = DataHelper.ConvertToInt32(array[4], petDataByDbID.FeedNum);
								petDataByDbID.ReAliveNum = DataHelper.ConvertToInt32(array[5], petDataByDbID.ReAliveNum);
								petDataByDbID.PetProps = DataHelper.ConvertToStr(array[6], petDataByDbID.PetProps);
								petDataByDbID.AddDateTime = DataHelper.ConvertToTicks(array[8], petDataByDbID.AddDateTime);
								petDataByDbID.Level = DataHelper.ConvertToInt32(array[9], petDataByDbID.Level);
							}
							else
							{
								petDataByDbID.PetID = -1;
								dbroleInfo.PetsDataList.Remove(petDataByDbID);
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PetData>(petDataByDbID, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessHorseOnCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.HorseDbID = num2;
					dbroleInfo.LastHorseID = num2;
				}
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessHorseOffCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.HorseDbID = 0;
				}
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessPetOutCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.PetDbID = num2;
				}
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessPetInCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.PetDbID = 0;
				}
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetGoodsListBySiteCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int site = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<GoodsData> instance = null;
				lock (dbroleInfo)
				{
					instance = Global.GetGoodsDataListBySite(dbroleInfo, site);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetGoodsListBySiteRangeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int siteBegin = Convert.ToInt32(array[1]);
				int siteEnd = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<GoodsData> instance = null;
				lock (dbroleInfo)
				{
					instance = Global.GetGoodsDataListBySiteRange(dbroleInfo, siteBegin, siteEnd);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetAddDJPointCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dbID = -1;
				lock (dbroleInfo)
				{
					dbID = dbroleInfo.RoleDJPointData.DbID;
					dbroleInfo.RoleDJPointData.DJPoint += num2;
					dbroleInfo.RoleDJPointData.Total++;
					dbroleInfo.RoleDJPointData.Wincnt += ((num2 > 0) ? 1 : 0);
				}
				int num3 = DBWriter.AddRoleDJPoint(dbMgr, dbID, num, num2);
				if (num3 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新一个用户角色的战将积分时错误，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (dbroleInfo.RoleDJPointData.DbID < 0)
						{
							dbroleInfo.RoleDJPointData.DbID = dbID;
						}
					}
				}
				string data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetDJPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DJPointsData djpointsData = new DJPointsData
				{
					SelfDJPointData = Global.GetRoleDJPointData(dbroleInfo),
					HotDJPointDataList = GameDBManager.SysDJPointsHotList.GetDJPointsHostList(dbMgr)
				};
				for (int i = 0; i < djpointsData.HotDJPointDataList.Count; i++)
				{
					DBRoleInfo dbroleInfo2;
					if (num == djpointsData.HotDJPointDataList[i].RoleID)
					{
						dbroleInfo2 = dbroleInfo;
					}
					else
					{
						dbroleInfo2 = dbMgr.GetDBRoleInfo(ref djpointsData.HotDJPointDataList[i].RoleID);
					}
					if (null == dbroleInfo2)
					{
						djpointsData.HotDJPointDataList[i].djRoleInfoData = null;
					}
					else
					{
						DJRoleInfoData djRoleInfoData = new DJRoleInfoData
						{
							RoleID = ((dbroleInfo2 != null) ? dbroleInfo2.RoleID : -1),
							RoleName = ((dbroleInfo2 != null) ? dbroleInfo2.RoleName : "未知"),
							Level = ((dbroleInfo2 != null) ? dbroleInfo2.Level : 0),
							Occupation = ((dbroleInfo2 != null) ? dbroleInfo2.Occupation : 0),
							OnlineState = Global.GetRoleOnlineState(dbroleInfo2)
						};
						djpointsData.HotDJPointDataList[i].djRoleInfoData = djRoleInfoData;
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DJPointsData>(djpointsData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpDianJiangLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int jingMaiBodyLevel = Convert.ToInt32(array[2]);
				int jingMaiID = Convert.ToInt32(array[3]);
				int jingMaiLevel = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpRoleJingMai(dbMgr, num, num2, jingMaiBodyLevel, jingMaiID, jingMaiLevel);
				if (num3 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("写入一个用户角色的经脉时错误，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.JingMaiDataList)
						{
							dbroleInfo.JingMaiDataList = new List<JingMaiData>();
						}
						if (num2 <= 0)
						{
							dbroleInfo.JingMaiDataList.Add(new JingMaiData
							{
								DbID = num3,
								JingMaiID = jingMaiID,
								JingMaiLevel = jingMaiLevel,
								JingMaiBodyLevel = jingMaiBodyLevel
							});
						}
						else
						{
							for (int i = 0; i < dbroleInfo.JingMaiDataList.Count; i++)
							{
								if (dbroleInfo.JingMaiDataList[i].DbID == num2)
								{
									dbroleInfo.JingMaiDataList[i].JingMaiLevel = jingMaiLevel;
								}
							}
						}
					}
				}
				string data2 = string.Format("{0}:{1}", num3, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessBanRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = array[0];
				int state = Convert.ToInt32(array[1]);
				BanManager.BanRoleName(roleName, state);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessBanRoleChatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = array[0];
				int banHours = Convert.ToInt32(array[1]);
				BanChatManager.AddBanRoleName(roleName, banHours);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBanRoleChatDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				tcpOutPacket = BanChatManager.GetBanChatDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddBullMsgCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string msgID = array[0];
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int interval = Global.SafeConvertToInt32(array[3], 10);
				string bulletinText = array[4];
				BulletinMsgData bulletinMsgData = GameDBManager.BulletinMsgMgr.AddBulletinMsg(msgID, fromDate, toDate, interval, bulletinText);
				DBWriter.NewBulletinText(dbMgr, msgID, fromDate, toDate, interval, bulletinText);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveBullMsgCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string msgID = array[0];
				GameDBManager.BulletinMsgMgr.RemoveBulletinMsg(msgID);
				DBWriter.RemoveBulletinText(dbMgr, msgID);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBullMsgDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				tcpOutPacket = GameDBManager.BulletinMsgMgr.GetBulletinMsgDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateOnlineTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int totalOnlineSecs = Convert.ToInt32(array[1]);
				int antiAddictionSecs = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = true;
				lock (dbroleInfo)
				{
					dbroleInfo.TotalOnlineSecs = totalOnlineSecs;
					dbroleInfo.AntiAddictionSecs = antiAddictionSecs;
				}
				if (flag)
				{
					DBWriter.UpdateRoleOnlineSecs(dbMgr, num, totalOnlineSecs, antiAddictionSecs);
					Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				}
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGameConfigDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				tcpOutPacket = GameDBManager.GameConfigMgr.GetGameConfigDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGameConfigItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0];
				string text3 = array[1];
				GameDBManager.GameConfigMgr.UpdateGameConfigItem(text2, text3);
				DBWriter.UpdateGameConfig(dbMgr, text2, text3);
				string gmCmd = string.Format("-config {0} {1}", text2, text3);
				ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessResetBigGuanCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				long num2 = Convert.ToInt64(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					dbroleInfo.BiGuanTime = num2;
				}
				DBWriter.UpdateRoleBiGuanTime(dbMgr, num, num2);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddSkillCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int skillID = Convert.ToInt32(array[1]);
				int skillLevel = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddSkill(dbMgr, num, skillID, skillLevel);
				if (num2 > 0)
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.SkillDataList)
						{
							dbroleInfo.SkillDataList = new List<SkillData>();
						}
						dbroleInfo.SkillDataList.Add(new SkillData
						{
							DbID = num2,
							SkillID = skillID,
							SkillLevel = skillLevel,
							UsedNum = 0
						});
					}
				}
				string data2 = string.Format("{0}", num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpSkillInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int skillLevel = Convert.ToInt32(array[2]);
				int usedNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.SkillDataList)
					{
						for (int i = 0; i < dbroleInfo.SkillDataList.Count; i++)
						{
							if (dbroleInfo.SkillDataList[i].DbID == num2)
							{
								dbroleInfo.SkillDataList[i].SkillLevel = skillLevel;
								dbroleInfo.SkillDataList[i].UsedNum = usedNum;
								break;
							}
						}
					}
				}
				bool flag2 = DBWriter.UpdateSkillInfo(dbMgr, num2, skillLevel, usedNum);
				string data2 = string.Format("{0}", flag2 ? 1 : 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateJingMaiExpCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int jingMaiExpNum = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalJingMaiExp = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.JingMaiExpNum = jingMaiExpNum;
					dbroleInfo.TotalJingMaiExp += num2;
					totalJingMaiExp = dbroleInfo.TotalJingMaiExp;
				}
				bool flag2 = DBWriter.UpdateJingMaiExp(dbMgr, num, jingMaiExpNum, totalJingMaiExp);
				string data2 = string.Format("{0}", flag2 ? 1 : 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateSkillIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleDefSkillID(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色缺省技能ID失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.DefaultSkillID = num2;
					}
					data2 = string.Format("{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateJieBiaoInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleJieBiaoInfo(dbMgr, num, num2, num3))
				{
					data2 = string.Format("{0}:{1}:{2}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色劫镖信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.JieBiaoDayID = num2;
						dbroleInfo.JieBiaoDayNum = num3;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateAutoDrinkCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleAutoDrink(dbMgr, num, num2, num3))
				{
					data2 = string.Format("{0}:{1}:{2}", num, -1, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色自动喝药设置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.AutoLifeV = num2;
						dbroleInfo.AutoMagicV = num3;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBufferItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				long startTime = Convert.ToInt64(array[2]);
				int bufferSecs = Convert.ToInt32(array[3]);
				long bufferVal = Convert.ToInt64(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateRoleBufferItem(dbMgr, num, num2, startTime, bufferSecs, bufferVal);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色Buffer项失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.BufferDataList)
						{
							dbroleInfo.BufferDataList = new List<BufferData>();
						}
						int num4 = -1;
						for (int i = 0; i < dbroleInfo.BufferDataList.Count; i++)
						{
							if (dbroleInfo.BufferDataList[i].BufferID == num2)
							{
								num4 = i;
								break;
							}
						}
						if (-1 == num4)
						{
							dbroleInfo.BufferDataList.Add(new BufferData
							{
								BufferID = num2,
								StartTime = startTime,
								BufferSecs = bufferSecs,
								BufferVal = bufferVal,
								BufferType = 0
							});
						}
						else
						{
							dbroleInfo.BufferDataList[num4].BufferID = num2;
							dbroleInfo.BufferDataList[num4].StartTime = startTime;
							dbroleInfo.BufferDataList[num4].BufferSecs = bufferSecs;
							dbroleInfo.BufferDataList[num4].BufferVal = bufferVal;
						}
					}
					data2 = string.Format("{0}:{1}", num, num3);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUnDelRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
				}
				string roleName = array[0];
				DBWriter.UnRemoveRole(dbMgr, roleName);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDelRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleName = array[0];
				DBWriter.RemoveRoleByName(dbMgr, roleName);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateDailyTaskDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int huanID = Convert.ToInt32(array[1]);
				string text2 = array[2];
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				int extDayID = Convert.ToInt32(array[5]);
				int extNum = Convert.ToInt32(array[6]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.MyDailyTaskDataList)
					{
						dbroleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
					}
					bool flag2 = false;
					DailyTaskData dailyTaskData = null;
					for (int i = 0; i < dbroleInfo.MyDailyTaskDataList.Count; i++)
					{
						if (dbroleInfo.MyDailyTaskDataList[i].TaskClass == num3)
						{
							flag2 = true;
							dailyTaskData = dbroleInfo.MyDailyTaskDataList[i];
							break;
						}
					}
					if (!flag2)
					{
						dailyTaskData = new DailyTaskData();
						dbroleInfo.MyDailyTaskDataList.Add(dailyTaskData);
					}
					dailyTaskData.HuanID = huanID;
					dailyTaskData.RecTime = text2;
					dailyTaskData.RecNum = num2;
					dailyTaskData.TaskClass = num3;
					dailyTaskData.ExtDayID = extDayID;
					dailyTaskData.ExtNum = extNum;
				}
				DBWriter.UpdateRoleDailyTaskData(dbMgr, num, huanID, text2, num2, num3, extDayID, extNum);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateDailyJingMaiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string jmTime = array[1];
				int jmNum = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.MyDailyJingMaiData)
					{
						dbroleInfo.MyDailyJingMaiData = new DailyJingMaiData();
					}
					dbroleInfo.MyDailyJingMaiData.JmTime = jmTime;
					dbroleInfo.MyDailyJingMaiData.JmNum = jmNum;
				}
				DBWriter.UpdateRoleDailyJingMaiData(dbMgr, num, jmTime, jmNum);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateNumSkillIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleNumSkillID(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色自动喝药设置失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.NumSkillID = num2;
					}
					data2 = string.Format("{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdatePBInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int extGridNum = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateRolePBInfo(dbMgr, num, extGridNum);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.MyPortableBagData.ExtGridNum = extGridNum;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleBagNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bagNum = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateRoleBagNum(dbMgr, num, bagNum);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色随身仓库信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.BagNum = bagNum;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateHuoDongInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 22)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				lock (dbroleInfo)
				{
					flag = dbroleInfo.ExistsMyHuodongData;
				}
				if (!flag)
				{
					DBWriter.CreateHuoDong(dbMgr, num);
					lock (dbroleInfo)
					{
						dbroleInfo.ExistsMyHuodongData = true;
					}
				}
				int num2 = DBWriter.UpdateHuoDong(dbMgr, num, array, 1);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色送礼活动信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.MyHuodongData.LastWeekID = DataHelper.ConvertToStr(array[1], dbroleInfo.MyHuodongData.LastWeekID);
						dbroleInfo.MyHuodongData.LastDayID = DataHelper.ConvertToStr(array[2], dbroleInfo.MyHuodongData.LastDayID);
						dbroleInfo.MyHuodongData.LoginNum = DataHelper.ConvertToInt32(array[3], dbroleInfo.MyHuodongData.LoginNum);
						dbroleInfo.MyHuodongData.NewStep = DataHelper.ConvertToInt32(array[4], dbroleInfo.MyHuodongData.NewStep);
						dbroleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(array[5], dbroleInfo.MyHuodongData.StepTime);
						dbroleInfo.MyHuodongData.LastMTime = DataHelper.ConvertToInt32(array[6], dbroleInfo.MyHuodongData.LastMTime);
						dbroleInfo.MyHuodongData.CurMID = DataHelper.ConvertToStr(array[7], dbroleInfo.MyHuodongData.CurMID);
						dbroleInfo.MyHuodongData.CurMTime = DataHelper.ConvertToInt32(array[8], dbroleInfo.MyHuodongData.CurMTime);
						dbroleInfo.MyHuodongData.SongLiID = DataHelper.ConvertToInt32(array[9], dbroleInfo.MyHuodongData.SongLiID);
						dbroleInfo.MyHuodongData.LoginGiftState = DataHelper.ConvertToInt32(array[10], dbroleInfo.MyHuodongData.LoginGiftState);
						dbroleInfo.MyHuodongData.OnlineGiftState = DataHelper.ConvertToInt32(array[11], dbroleInfo.MyHuodongData.OnlineGiftState);
						dbroleInfo.MyHuodongData.LastLimitTimeHuoDongID = DataHelper.ConvertToInt32(array[12], dbroleInfo.MyHuodongData.LastLimitTimeHuoDongID);
						dbroleInfo.MyHuodongData.LastLimitTimeDayID = DataHelper.ConvertToInt32(array[13], dbroleInfo.MyHuodongData.LastLimitTimeDayID);
						dbroleInfo.MyHuodongData.LimitTimeLoginNum = DataHelper.ConvertToInt32(array[14], dbroleInfo.MyHuodongData.LimitTimeLoginNum);
						dbroleInfo.MyHuodongData.LimitTimeGiftState = DataHelper.ConvertToInt32(array[15], dbroleInfo.MyHuodongData.LimitTimeGiftState);
						dbroleInfo.MyHuodongData.EveryDayOnLineAwardStep = DataHelper.ConvertToInt32(array[16], dbroleInfo.MyHuodongData.EveryDayOnLineAwardStep);
						dbroleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = DataHelper.ConvertToInt32(array[17], dbroleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID);
						dbroleInfo.MyHuodongData.SeriesLoginGetAwardStep = DataHelper.ConvertToInt32(array[18], dbroleInfo.MyHuodongData.SeriesLoginGetAwardStep);
						dbroleInfo.MyHuodongData.SeriesLoginAwardDayID = DataHelper.ConvertToInt32(array[19], dbroleInfo.MyHuodongData.SeriesLoginAwardDayID);
						dbroleInfo.MyHuodongData.SeriesLoginAwardGoodsID = DataHelper.ConvertToStr(array[20], dbroleInfo.MyHuodongData.SeriesLoginAwardGoodsID);
						dbroleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = DataHelper.ConvertToStr(array[21], dbroleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID);
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateInputPointsUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = Convert.ToString(array[0]);
				int num = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace("$", ":");
				string toDate = array[3].Replace("$", ":");
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
				string data2;
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
					data2 = string.Format("{0}:{1}", text2, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text3 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, text2, 64, huoDongKeyString, out num2, out text3);
				if (num2 > 0)
				{
					flag = false;
				}
				else
				{
					int num3 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, text2, 64, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}", text2, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, text2, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
						data2 = string.Format("{0}:{1}", text2, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool flag2 = false;
				int num4 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.InputPoints = 0;
					}
					if (num < 0 && dbuserInfo.InputPoints < Math.Abs(num))
					{
						flag2 = true;
					}
					else
					{
						dbuserInfo.InputPoints = Math.Max(0, dbuserInfo.InputPoints + num);
						num4 = dbuserInfo.InputPoints;
					}
				}
				if (flag2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
					data2 = string.Format("{0}:{1}", text2, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num != 0)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, text2, num4))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, text2), null, true);
						data2 = string.Format("{0}:{1}", text2, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", text2, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateInputPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace("$", ":");
				string toDate = array[3].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 64, huoDongKeyString, out num3, out text2);
				if (num3 > 0)
				{
					flag = false;
				}
				else
				{
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 64, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool flag2 = false;
				int num5 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.InputPoints = 0;
					}
					if (num2 < 0 && dbuserInfo.InputPoints < Math.Abs(num2))
					{
						flag2 = true;
					}
					else
					{
						dbuserInfo.InputPoints = Math.Max(0, dbuserInfo.InputPoints + num2);
						num5 = dbuserInfo.InputPoints;
					}
				}
				if (flag2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, num5))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", num, num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateSpecJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace("$", ":");
				string toDate = array[3].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 44, huoDongKeyString, out num3, out text2);
				if (num3 > 0)
				{
					flag = false;
				}
				else
				{
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 44, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool flag2 = false;
				int num5 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.SpecJiFen = 0;
					}
					if (num2 < 0 && dbuserInfo.SpecJiFen < Math.Abs(num2))
					{
						flag2 = true;
					}
					else
					{
						dbuserInfo.SpecJiFen = Math.Max(0, dbuserInfo.SpecJiFen + num2);
						num5 = dbuserInfo.SpecJiFen;
					}
				}
				if (flag2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, num5))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的专享活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", num, num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateEveryJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string fromDate = array[2].Replace("$", ":");
				string toDate = array[3].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 47, huoDongKeyString, out num3, out text2);
				if (num3 > 0)
				{
					flag = false;
				}
				else
				{
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 47, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bool flag2 = false;
				int num5 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.EveryJiFen = 0;
					}
					if (num2 < 0 && dbuserInfo.EveryJiFen < Math.Abs(num2))
					{
						flag2 = true;
					}
					else
					{
						dbuserInfo.EveryJiFen = Math.Max(0, dbuserInfo.EveryJiFen + num2);
						num5 = dbuserInfo.EveryJiFen;
					}
				}
				if (flag2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, num5))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的每日活动积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", num, num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSubChongZhiJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num3 = 0;
				lock (dbuserInfo)
				{
					if (dbuserInfo.GiftJiFen >= Math.Abs(num2))
					{
						dbuserInfo.GiftJiFen = Math.Max(0, dbuserInfo.GiftJiFen - Math.Abs(num2));
						num3 = dbuserInfo.GiftJiFen;
					}
					else
					{
						flag = true;
					}
				}
				if (flag)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("扣除充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateUserGiftJiFen(dbMgr, userID, num3))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新用户的充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, -3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUseLiPinMaCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int songLiID = Convert.ToInt32(array[1]);
				string text2 = array[2];
				text2 = text2.ToUpper();
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2;
				if (text2.Substring(0, 2) == "NZ")
				{
					num2 = LiPinMaManager.UseLiPinMa2(dbMgr, num, songLiID, text2, dbroleInfo.ZoneID);
				}
				else if (text2.Substring(0, 2) == "NX")
				{
					num2 = LiPinMaManager.UseLiPinMaNX(dbMgr, num, songLiID, text2, dbroleInfo.ZoneID);
				}
				else
				{
					num2 = LiPinMaManager.UseLiPinMa(dbMgr, num, songLiID, text2, false);
				}
				string data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteSpecActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.SpecActInfoDict)
					{
						if (num2 == 0)
						{
							dbroleInfo.SpecActInfoDict.Clear();
						}
						else
						{
							List<int> list = new List<int>();
							foreach (KeyValuePair<int, SpecActInfoDB> keyValuePair in dbroleInfo.SpecActInfoDict)
							{
								if (keyValuePair.Value.GroupID == num2)
								{
									list.Add(keyValuePair.Key);
								}
							}
							foreach (int key in list)
							{
								dbroleInfo.SpecActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteSpecialActivityData(dbMgr, num, num2);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteEveryActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.EverydayActInfoDict)
					{
						if (num2 == 0)
						{
							dbroleInfo.EverydayActInfoDict.Clear();
						}
						else
						{
							List<int> list = new List<int>();
							foreach (KeyValuePair<int, EverydayActInfoDB> keyValuePair in dbroleInfo.EverydayActInfoDict)
							{
								if (keyValuePair.Value.GroupID == num2 && (num3 == 0 || keyValuePair.Key == num3))
								{
									list.Add(keyValuePair.Key);
								}
							}
							foreach (int key in list)
							{
								dbroleInfo.EverydayActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteEverydayActivityData(dbMgr, num, num2, num3);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetSpecActInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, SpecActInfoDB>>(dbroleInfo.SpecActInfoDict, pool, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateSpecActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				SpecActInfoDB specActInfoDB = new SpecActInfoDB();
				specActInfoDB.GroupID = Convert.ToInt32(array[1]);
				specActInfoDB.ActID = Convert.ToInt32(array[2]);
				specActInfoDB.PurNum = Convert.ToInt32(array[3]);
				specActInfoDB.CountNum = Convert.ToInt32(array[4]);
				specActInfoDB.Active = Convert.ToInt16(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.SpecActInfoDict)
					{
						dbroleInfo.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
					}
					dbroleInfo.SpecActInfoDict[specActInfoDB.ActID] = specActInfoDB;
				}
				DBWriter.UpdateSpecialActivityData(dbMgr, num, specActInfoDB);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateSpecPriorityActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				SpecPriorityActInfoDB specPriorityActInfoDB = new SpecPriorityActInfoDB();
				specPriorityActInfoDB.TeQuanID = Convert.ToInt32(array[1]);
				specPriorityActInfoDB.ActID = Convert.ToInt32(array[2]);
				specPriorityActInfoDB.PurNum = Convert.ToInt32(array[3]);
				specPriorityActInfoDB.CountNum = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(specPriorityActInfoDB.TeQuanID, specPriorityActInfoDB.ActID);
					if (null == dbroleInfo.SpecPriorityActInfoDict)
					{
						dbroleInfo.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
					}
					dbroleInfo.SpecPriorityActInfoDict[key] = specPriorityActInfoDB;
				}
				DBWriter.UpdateSpecialPriorityActivityData(dbMgr, num, specPriorityActInfoDB);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteSpecPriorityActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.SpecPriorityActInfoDict)
					{
						if (num2 == 0)
						{
							dbroleInfo.SpecPriorityActInfoDict.Clear();
						}
						else
						{
							List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
							foreach (KeyValuePair<KeyValuePair<int, int>, SpecPriorityActInfoDB> keyValuePair in dbroleInfo.SpecPriorityActInfoDict)
							{
								if (keyValuePair.Value.TeQuanID == num2)
								{
									list.Add(new KeyValuePair<int, int>(num2, keyValuePair.Value.ActID));
								}
							}
							foreach (KeyValuePair<int, int> key in list)
							{
								dbroleInfo.SpecPriorityActInfoDict.Remove(key);
							}
						}
					}
				}
				DBWriter.DeleteSpecialPriorityActivityData(dbMgr, num, num2);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateEveryActCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				EverydayActInfoDB everydayActInfoDB = new EverydayActInfoDB();
				everydayActInfoDB.GroupID = Convert.ToInt32(array[1]);
				everydayActInfoDB.ActID = Convert.ToInt32(array[2]);
				everydayActInfoDB.PurNum = Convert.ToInt32(array[3]);
				everydayActInfoDB.CountNum = Convert.ToInt32(array[4]);
				everydayActInfoDB.ActiveDay = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.EverydayActInfoDict)
					{
						dbroleInfo.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
					}
					dbroleInfo.EverydayActInfoDict[everydayActInfoDB.ActID] = everydayActInfoDB;
				}
				DBWriter.UpdateEverydayActivityData(dbMgr, num, everydayActInfoDB);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateFuBenDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int dayID = Convert.ToInt32(array[2]);
				int enterNum = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				int num4 = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.FuBenDataList)
					{
						dbroleInfo.FuBenDataList = new List<FuBenData>();
					}
					bool flag2 = false;
					for (int i = 0; i < dbroleInfo.FuBenDataList.Count; i++)
					{
						if (dbroleInfo.FuBenDataList[i].FuBenID == num2)
						{
							dbroleInfo.FuBenDataList[i].FuBenID = num2;
							dbroleInfo.FuBenDataList[i].DayID = dayID;
							dbroleInfo.FuBenDataList[i].EnterNum = enterNum;
							dbroleInfo.FuBenDataList[i].QuickPassTimer = num3;
							dbroleInfo.FuBenDataList[i].FinishNum = num4;
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						dbroleInfo.FuBenDataList.Add(new FuBenData
						{
							FuBenID = num2,
							DayID = dayID,
							EnterNum = enterNum,
							QuickPassTimer = num3,
							FinishNum = num4
						});
					}
				}
				DBWriter.UpdateFuBenData(dbMgr, num, num2, dayID, enterNum, num3, num4);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuBenSeqIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int fuBenSeqID = FuBenSeqIDMgr.GetFuBenSeqID();
				string data2 = string.Format("{0}:{1}", num, fuBenSeqID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuBenHistDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int fuBenID = Convert.ToInt32(array[1]);
				FuBenHistData instance = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenHistData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddFuBenHistDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string roleName = array[1];
				int fuBenID = Convert.ToInt32(array[2]);
				int num2 = Convert.ToInt32(array[3]);
				FuBenHistData fuBenHistData = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				string data2;
				if (fuBenHistData == null || num2 < fuBenHistData.UsedSecs)
				{
					int num3 = DBWriter.InsertNewFuBenHist(dbMgr, fuBenID, num, roleName, num2);
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}", num, num3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					FuBenHistManager.AddFuBenHistData(fuBenID, num, roleName, num2);
				}
				data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateLianZhanCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateLianZhan(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色连斩值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.LianZhan = num2;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleDailyDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 14)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int expDayID = Convert.ToInt32(array[1]);
				int todayExp = Convert.ToInt32(array[2]);
				int lingLiDayID = Convert.ToInt32(array[3]);
				int todayLingLi = Convert.ToInt32(array[4]);
				int killBossDayID = Convert.ToInt32(array[5]);
				int todayKillBoss = Convert.ToInt32(array[6]);
				int fuBenDayID = Convert.ToInt32(array[7]);
				int todayFuBenNum = Convert.ToInt32(array[8]);
				int wuXingDayID = Convert.ToInt32(array[9]);
				int wuXingNum = Convert.ToInt32(array[10]);
				int rebornExpDayID = Convert.ToInt32(array[11]);
				int rebornExpMonster = Convert.ToInt32(array[12]);
				int rebornExpSale = Convert.ToInt32(array[13]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateRoleDailyData(dbMgr, num, expDayID, todayExp, lingLiDayID, todayLingLi, killBossDayID, todayKillBoss, fuBenDayID, todayFuBenNum, wuXingDayID, wuXingNum, rebornExpDayID, rebornExpMonster, rebornExpSale);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色日常数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.MyRoleDailyData)
						{
							dbroleInfo.MyRoleDailyData = new RoleDailyData();
						}
						dbroleInfo.MyRoleDailyData.ExpDayID = expDayID;
						dbroleInfo.MyRoleDailyData.TodayExp = todayExp;
						dbroleInfo.MyRoleDailyData.LingLiDayID = lingLiDayID;
						dbroleInfo.MyRoleDailyData.TodayLingLi = todayLingLi;
						dbroleInfo.MyRoleDailyData.KillBossDayID = killBossDayID;
						dbroleInfo.MyRoleDailyData.TodayKillBoss = todayKillBoss;
						dbroleInfo.MyRoleDailyData.FuBenDayID = fuBenDayID;
						dbroleInfo.MyRoleDailyData.TodayFuBenNum = todayFuBenNum;
						dbroleInfo.MyRoleDailyData.WuXingDayID = wuXingDayID;
						dbroleInfo.MyRoleDailyData.WuXingNum = wuXingNum;
						dbroleInfo.MyRoleDailyData.RebornExpDayID = rebornExpDayID;
						dbroleInfo.MyRoleDailyData.RebornExpMonster = rebornExpMonster;
						dbroleInfo.MyRoleDailyData.RebornExpSale = rebornExpSale;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateKillBossCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateKillBoss(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色杀BOSS总数量时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.KillBoss = num2;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleStatCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int equipJiFen = Convert.ToInt32(array[1]);
				int xueWeiNum = Convert.ToInt32(array[2]);
				int skillLearnedNum = Convert.ToInt32(array[3]);
				int horseJiFen = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleStat(dbMgr, num, equipJiFen, xueWeiNum, skillLearnedNum, horseJiFen))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色的统计数据时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetPaiHangListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2 && array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int paiHangType = Convert.ToInt32(array[1]);
				int pageShowNum = -1;
				if (array.Length == 3)
				{
					pageShowNum = Convert.ToInt32(array[2]);
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (num != 0 && null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				PaiHangData paiHangData = PaiHangManager.GetPaiHangData(paiHangType, pageShowNum);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PaiHangData>(paiHangData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateYaBiaoDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int yaBiaoID = Convert.ToInt32(array[1]);
				long startTime = Convert.ToInt64(array[2]);
				int state = Convert.ToInt32(array[3]);
				int lineID = Convert.ToInt32(array[4]);
				int touBao = Convert.ToInt32(array[5]);
				int yaBiaoDayID = Convert.ToInt32(array[6]);
				int yaBiaoNum = Convert.ToInt32(array[7]);
				int takeGoods = Convert.ToInt32(array[8]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				lock (dbroleInfo)
				{
					if (null != dbroleInfo.MyYaBiaoData)
					{
						num2 = (dbroleInfo.MyYaBiaoData.State = state);
					}
				}
				if (num2 > 0)
				{
					state = num2;
				}
				int num3 = DBWriter.UpdateYaBiaoData(dbMgr, num, yaBiaoID, startTime, state, lineID, touBao, yaBiaoDayID, yaBiaoNum, takeGoods);
				string data2;
				if (num3 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num3);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.MyYaBiaoData)
						{
							dbroleInfo.MyYaBiaoData = new YaBiaoData();
						}
						dbroleInfo.MyYaBiaoData.YaBiaoID = yaBiaoID;
						dbroleInfo.MyYaBiaoData.StartTime = startTime;
						dbroleInfo.MyYaBiaoData.State = state;
						dbroleInfo.MyYaBiaoData.LineID = lineID;
						dbroleInfo.MyYaBiaoData.TouBao = touBao;
						dbroleInfo.MyYaBiaoData.YaBiaoDayID = yaBiaoDayID;
						dbroleInfo.MyYaBiaoData.YaBiaoNum = yaBiaoNum;
						dbroleInfo.MyYaBiaoData.TakeGoods = takeGoods;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateYaBiaoDataStateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int state = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateYaBiaoDataState(dbMgr, num, state);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null == dbroleInfo.MyYaBiaoData)
						{
							dbroleInfo.MyYaBiaoData = new YaBiaoData();
						}
						dbroleInfo.MyYaBiaoData.State = state;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetOtherAttrib2DataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[1]);
				RoleDataEx roleDataEx = new RoleDataEx();
				DBRoleInfo dballRoleInfo = dbMgr.GetDBAllRoleInfo(roleID);
				if (null == dballRoleInfo)
				{
					roleDataEx.RoleID = -1;
				}
				else
				{
					Global.DBRoleInfo2RoleDataEx(dballRoleInfo, roleDataEx);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBattleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				long num2 = Convert.ToInt64(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleBattleNameInfo(dbMgr, num, num2, num3))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色押镖数据值时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.BattleNameStart = num2;
						dbroleInfo.BattleNameIndex = num3;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddMallBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftMoney = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewMallBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftMoney);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的商城购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddQiZhenGeBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftMoney = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewQiZhenGeBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftMoney);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的奇珍阁购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetLiPinMaInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int songLiID = Convert.ToInt32(array[1]);
				string text2 = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2;
				if (text2.Length < 3)
				{
					num2 = -1020;
				}
				else if (text2.Substring(0, 2) == "NZ")
				{
					num2 = LiPinMaManager.GetLiPinMaPingTaiID2(dbMgr, songLiID, text2, dbroleInfo.ZoneID);
				}
				else if (text2.Substring(0, 2) == "NX")
				{
					num2 = LiPinMaManager.GetLiPinMaPingTaiIDNX(dbMgr, songLiID, text2, dbroleInfo.ZoneID);
				}
				else
				{
					num2 = LiPinMaManager.GetLiPinMaPingTaiID(dbMgr, songLiID, text2);
				}
				string data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetInputPointsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace("$", ":");
				string toDate = array[2].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 64, huoDongKeyString, out num2, out text2);
				if (num2 > 0)
				{
					flag = false;
				}
				else
				{
					int num3 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 64, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserInputPoints(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的充值点失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num4 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.InputPoints = 0;
					}
					else
					{
						num4 = dbuserInfo.InputPoints;
					}
				}
				data2 = string.Format("{0}:{1}", num, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetSpecJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace("$", ":");
				string toDate = array[2].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 44, huoDongKeyString, out num2, out text2);
				if (num2 > 0)
				{
					flag = false;
				}
				else
				{
					int num3 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 44, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserSpecJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的专享活动充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num4 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.SpecJiFen = 0;
					}
					else
					{
						num4 = dbuserInfo.SpecJiFen;
					}
				}
				data2 = string.Format("{0}:{1}", num, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetEveryJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace("$", ":");
				string toDate = array[2].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				bool flag = true;
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, userID, 47, huoDongKeyString, out num2, out text2);
				if (num2 > 0)
				{
					flag = false;
				}
				else
				{
					int num3 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, userID, 47, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				if (flag)
				{
					if (!DBWriter.UpdateUserEveryJiFen(dbMgr, userID, 0))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("清空用户的每日活动充值积分失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
						data2 = string.Format("{0}:{1}", num, 0);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num4 = 0;
				lock (dbuserInfo)
				{
					if (flag)
					{
						dbuserInfo.EveryJiFen = 0;
					}
					else
					{
						num4 = dbuserInfo.EveryJiFen;
					}
				}
				data2 = string.Format("{0}:{1}", num, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetChongZhiJiFenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = dbroleInfo.UserID;
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(userID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("查找用户数据失败，CMD={0}, UserID={1}", (TCPGameServerCmds)nID, userID), null, true);
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				lock (dbuserInfo)
				{
					num2 = dbuserInfo.GiftJiFen;
				}
				data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateCZTaskIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleCZTaskID(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色充值任务ID时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.CZTaskID = num2;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetTotalOnlineNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int totalOnlineNum = LineManager.GetTotalOnlineNum();
				string data2 = string.Format("{0}", totalOnlineNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuBenHistListDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = FuBenHistManager.GetFuBenHistListData(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBattleNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateBattleNum(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色角斗场称号次数时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.BattleNum = num2;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateHeroIndexCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateHeroIndex(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}:{2}", num, num2, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色英雄逐擂到达层数时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.HeroIndex = num2;
					}
					data2 = string.Format("{0}:{1}:{2}", num, num2, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessForceReloadPaiHangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				PaiHangManager.ProcessPaiHang(dbMgr, true);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetOtherHorseDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				HorseData instance = null;
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo2)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo2)
				{
					if (null != dbroleInfo2.HorsesDataList)
					{
						for (int i = 0; i < dbroleInfo2.HorsesDataList.Count; i++)
						{
							if (dbroleInfo2.HorsesDataList[i].DbID == dbroleInfo2.HorseDbID)
							{
								instance = dbroleInfo2.HorsesDataList[i];
								break;
							}
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HorseData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddYinPiaoBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftYinPiaoNum = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewYinPiaoBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftYinPiaoNum);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的银票购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBangHuiListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int isVerify = Convert.ToInt32(array[1]);
				int startIndex = Convert.ToInt32(array[2]);
				int endIndex = Convert.ToInt32(array[3]);
				BangHuiListData bangHuiListData = GameDBManager.BangHuiListMgr.GetBangHuiListData(dbMgr, isVerify, startIndex, endIndex);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiListData>(bangHuiListData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBangHuiFuBenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(array[0]);
				string data2 = DBQuery.QueryBangFuBenByID(dbMgr, bhid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBangHuiFuBenCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(array[0]);
				int fubenid = Convert.ToInt32(array[1]);
				int state = Convert.ToInt32(array[2]);
				int openday = Convert.ToInt32(array[3]);
				string killers = array[4];
				DBWriter.UpdateBangHuiFuBen(dbMgr, bhid, fubenid, state, openday, killers);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessCreateBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1].Split(new char[]
				{
					'$'
				})[0];
				string bhBulletin = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				lock (dbroleInfo)
				{
					if (dbroleInfo.Faction > 0 || !string.IsNullOrEmpty(dbroleInfo.BHName))
					{
						num2 = -1001;
					}
				}
				if (num2 >= 0 && !SingletonTemplate<NameManager>.Instance().IsNameCanUseInDb(dbMgr, text2))
				{
					num2 = -1031;
				}
				if (num2 >= 0 && !SingletonTemplate<NameUsedMgr>.Instance().AddCannotUse_BangHui_Ex(text2))
				{
					num2 = -1031;
				}
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}:{2}", num2, num, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				int zoneID = GameDBManager.ZoneID;
				lock (Global.BangHuiMutex)
				{
					num3 = DBQuery.FindBangHuiByRoleID(dbMgr, num);
					if (num3 <= 0)
					{
						num3 = DBQuery.FindJoinBangHuiByRoleID(dbMgr, num);
						if (num3 <= 0)
						{
							num3 = DBWriter.CreateBangHui(dbMgr, num, zoneID, dbroleInfo.Level, text2, bhBulletin, Convert.ToInt32(array[3]));
							if (num3 < 0)
							{
								num2 = -1031;
							}
						}
						else
						{
							num2 = -1021;
						}
					}
					else
					{
						num2 = -1011;
					}
				}
				if (num2 >= 0)
				{
					lock (dbroleInfo)
					{
						dbroleInfo.Faction = num3;
						dbroleInfo.BHName = Global.FormatBangHuiName(zoneID, text2);
						dbroleInfo.BHZhiWu = 1;
					}
					DBWriter.UpdateRoleBangHuiInfo(dbMgr, num, num3, Global.FormatBangHuiName(zoneID, text2), 1);
					GameDBManager.BangHuiJunQiMgr.AddBangHuiJunQi(num3, text2, 1);
				}
				data2 = string.Format("{0}:{1}:{2}", num2, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBangHuiMiniDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(array[0]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiMiniData>(null, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMiniData instance = new BangHuiMiniData
				{
					BHID = bangHuiDetailData.BHID,
					BHName = bangHuiDetailData.BHName,
					ZoneID = bangHuiDetailData.ZoneID
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiMiniData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryBangHuiDetailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2 && array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				int num2 = 0;
				int num3 = 0;
				if (4 == array.Length)
				{
					num2 = Convert.ToInt32(array[2]);
					num3 = Convert.ToInt32(array[3]);
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				if (bangHuiDetailData == null || num <= 0)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData.MgrItemList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, bhid);
				if (num2 == 1)
				{
					dbroleInfo.BGDayID1 = num3;
					dbroleInfo.BGMoney = 0;
					dbroleInfo.BGDayID2 = num3;
					dbroleInfo.BGGoods = 0;
					DBWriter.UpdateRoleBangGong(dbMgr, num, dbroleInfo.BGDayID1, dbroleInfo.BGMoney, dbroleInfo.BGDayID2, dbroleInfo.BGGoods, dbroleInfo.BangGong);
				}
				bangHuiDetailData.TodayZhanGongForGold = dbroleInfo.BGMoney;
				bangHuiDetailData.TodayZhanGongForDiamond = dbroleInfo.BGGoods;
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBangHuiBulletinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string bhBulletinMsg = array[2];
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, num2, num);
				if (bangHuiMgrItemItemDataByID != null && bangHuiMgrItemItemDataByID.BHZhiwu > 0)
				{
					DBWriter.UpdateBangHuiBulletin(dbMgr, num2, bhBulletinMsg);
				}
				string data2 = string.Format("{0}:{1}:{2}", 0, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGVoiceSetPrioritysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string prioritys = array[2];
				BangHuiCacheData bangHuiCacheData = GameDBManager.BangHuiListMgr.GetBangHuiCacheData(num2);
				if (bangHuiCacheData != null && (bangHuiCacheData.LeaderId == (long)num || (bangHuiCacheData.Query(num2) && bangHuiCacheData.LeaderId == (long)num)))
				{
					bangHuiCacheData.UpdateGVoicePrioritys(prioritys);
				}
				string data2 = string.Format("{0}:{1}:{2}", 0, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGVoiceGetPrioritysCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				int bhid = DataHelper.BytesToObject<int>(data, 0, count);
				string arg = "";
				BangHuiCacheData bangHuiCacheData = GameDBManager.BangHuiListMgr.GetBangHuiCacheData(bhid);
				if (bangHuiCacheData != null)
				{
					arg = bangHuiCacheData.GVoicePrioritys;
				}
				string data2 = string.Format("{0}", arg);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateZhengDuoUsedTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int[] array = null;
			try
			{
				array = DataHelper.BytesToObject<int[]>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				int bhid = Convert.ToInt32(array[0]);
				int weekDay = Convert.ToInt32(array[1]);
				int num = Convert.ToInt32(array[2]);
				DBWriter.UpdateZhengDuoUsedTime(dbMgr, bhid, weekDay, num);
				byte[] array2 = DataHelper.ObjectToBytes<int>(num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array2, 0, array2.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetZhengDuoUsedTimeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			int value;
			try
			{
				value = DataHelper.BytesToObject<int>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				int bhid = Convert.ToInt32(value);
				int[] instance = DBQuery.QueryZhengDuoUsedTime(dbMgr, bhid);
				byte[] array = DataHelper.ObjectToBytes<int[]>(instance);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBHMemberDataListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				List<BangHuiMemberData> bangHuiMemberDataList = DBQuery.GetBangHuiMemberDataList(dbMgr, bhid);
				if (null != bangHuiMemberDataList)
				{
					int i = 0;
					while (i < bangHuiMemberDataList.Count)
					{
						DBRoleInfo dbroleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != dbroleInfo)
						{
							goto IL_137;
						}
						dbroleInfo = dbMgr.GetDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != dbroleInfo)
						{
							goto IL_137;
						}
						IL_162:
						i++;
						continue;
						IL_137:
						bangHuiMemberDataList[i].LogOffTime = dbroleInfo.LogOffTime;
						bangHuiMemberDataList[i].OnlineState = Global.GetRoleOnlineState(dbroleInfo);
						goto IL_162;
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<BangHuiMemberData>>(bangHuiMemberDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBHVerifyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, num2, num);
				if (null == bangHuiMgrItemItemDataByID)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-10,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemItemDataByID.BHZhiwu != 1)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-10,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiVerify(dbMgr, num, num2, num3);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryBHMGRListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						-1,
						""
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = DBQuery.GetBangHuiMgrItemItemStringList(dbMgr, num2);
				text2 = text2.TrimEnd(new char[]
				{
					','
				});
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					bangHuiDetailData.IsVerify,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessBangHuiVerifyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				dbroleInfo.BHVerify = num2;
				DBWriter.UpdateRoleBangHuiVerify(dbMgr, num, num2);
				string data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddBHMemberCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				string text2 = array[3];
				int num4 = Convert.ToInt32(array[4]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num3);
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num4 > 0)
				{
					if (dbroleInfo.BHVerify > 0)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1050,
							num,
							num2,
							num3
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num5 = 50;
				if (DBQuery.QueryBHMemberNum(dbMgr, num2) >= num5)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1060,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (dbroleInfo.Faction > 0)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1020,
							num,
							num2,
							num3
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					dbroleInfo.Faction = num2;
					dbroleInfo.BHName = Global.FormatBangHuiName(bangHuiDetailData.ZoneID, bangHuiDetailData.BHName);
					dbroleInfo.BHZhiWu = 0;
					dbroleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbroleInfo.RoleID, dbroleInfo.Faction, dbroleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbroleInfo.RoleID, 0);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveBHMemberCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				string text2 = array[3];
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, num2, num);
				string data2;
				if (null == bangHuiMgrItemItemDataByID)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemItemDataByID.BHZhiwu <= 0)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1002,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num3);
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.BHZhiWu > 0 && bangHuiMgrItemItemDataByID.BHZhiwu >= dbroleInfo.BHZhiWu)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1002,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					dbroleInfo.Faction = 0;
					dbroleInfo.BHName = "";
					dbroleInfo.BHZhiWu = 0;
					dbroleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbroleInfo.RoleID, dbroleInfo.Faction, dbroleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbroleInfo.RoleID, 0);
				DBWriter.ClearLastBangHuiInfoByRoleID(dbMgr, dbroleInfo.RoleID);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQuitFromBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}", -1000, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}", -1001, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (dbroleInfo.Faction != num2)
					{
						data2 = string.Format("{0}:{1}:{2}", -1020, num, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (1 == dbroleInfo.BHZhiWu)
					{
						data2 = string.Format("{0}:{1}:{2}", -1030, num, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					dbroleInfo.Faction = 0;
					dbroleInfo.BHName = "";
					dbroleInfo.BHZhiWu = 0;
					dbroleInfo.JunTuanZhiWu = 0;
				}
				DBWriter.UpdateRoleBangHuiInfo(dbMgr, dbroleInfo.RoleID, dbroleInfo.Faction, dbroleInfo.BHName, 0);
				DBWriter.UpdateRoleJunTuanInfo(dbMgr, dbroleInfo.RoleID, 0);
				data2 = string.Format("{0}:{1}:{2}", 0, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDestroyBangHuiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}", -1000, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}", -1010, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}", -1020, num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDestroyMgr.DoDestroyBangHui(dbMgr, num2);
				data2 = string.Format("{0}:{1}:{2}", 0, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessChgBHMemberZhiWuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				string data2;
				if (num == num3)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1002,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1001,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1010,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, num2, num);
				if (null == bangHuiMgrItemItemDataByID)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1020,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemItemDataByID.BHZhiwu != 1 || dbroleInfo.BHZhiWu != 1)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1030,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num3);
				if (null == dbroleInfo2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1040,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo2.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						-1060,
						num,
						num2,
						num3,
						num4,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<BangHuiMgrItemData> bangHuiMgrItemItemDataList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, num2);
				int num5 = 0;
				lock (Global.BangHuiMutex)
				{
					if (num4 > 0)
					{
						DBWriter.ClearBangHuiMemberZhiWu(dbMgr, num2, num4);
						num5 = Global.GetDBRoleInfoByZhiWu(bangHuiMgrItemItemDataList, num4);
						if (num5 > 0)
						{
							DBRoleInfo dbroleInfo3 = dbMgr.DBRoleMgr.FindDBRoleInfo(ref num5);
							if (null != dbroleInfo3)
							{
								lock (dbroleInfo3)
								{
									dbroleInfo3.BHZhiWu = 0;
								}
							}
						}
					}
					bool flag3 = false;
					lock (dbroleInfo2)
					{
						if (dbroleInfo2.Faction == num2)
						{
							flag3 = true;
							dbroleInfo2.BHZhiWu = num4;
						}
					}
					if (flag3)
					{
						DBWriter.UpdateBangHuiMemberZhiWu(dbMgr, num2, num3, num4);
						if (1 == num4)
						{
							DBWriter.UpdateBangHuiRoleID(dbMgr, num3, num2);
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					0,
					num,
					num2,
					num3,
					num4,
					num5
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessChgJunTuanMemberZhiWuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			try
			{
				tcpOutPacket = null;
				List<int> list = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (list.Count < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}", (TCPGameServerCmds)nID, list.Count), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(list[0]);
				int num = Convert.ToInt32(list[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}", -1001);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int faction = dbroleInfo.Faction;
				if (dbroleInfo.Faction != faction)
				{
					data2 = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, faction, roleID);
				if (null == bangHuiMgrItemItemDataByID)
				{
					data2 = string.Format("{0}", -1020);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemItemDataByID.BHZhiwu != 1 || dbroleInfo.BHZhiWu != 1)
				{
					data2 = string.Format("{0}", -1030);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> range = list.GetRange(2, list.Count - 2);
				DBWriter.ClearBangHuiZhiWuNotInList(dbMgr, faction, range);
				DBWriter.ChangeJunTuanZhiWuList(dbMgr, faction, num, range);
				List<BangHuiMemberData> bangHuiMemberDataList = DBQuery.GetBangHuiMemberDataList(dbMgr, faction);
				if (null != bangHuiMemberDataList)
				{
					for (int i = 0; i < bangHuiMemberDataList.Count; i++)
					{
						DBRoleInfo dbroleInfo2 = dbMgr.DBRoleMgr.FindDBRoleInfo(ref bangHuiMemberDataList[i].RoleID);
						if (null != dbroleInfo2)
						{
							dbroleInfo2.JunTuanZhiWu = (range.Contains(bangHuiMemberDataList[i].RoleID) ? num : 0);
						}
					}
				}
				data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessChgBHMemberChengHaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				string text2 = array[3];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1001,
						num,
						num2,
						num3,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						num,
						num2,
						num3,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiMgrItemData bangHuiMgrItemItemDataByID = DBQuery.GetBangHuiMgrItemItemDataByID(dbMgr, num2, num);
				if (null == bangHuiMgrItemItemDataByID)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1020,
						num,
						num2,
						num3,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiMgrItemItemDataByID.BHZhiwu <= 0)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1030,
						num,
						num2,
						num3,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiMemberChengHao(dbMgr, num2, num3, text2);
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					num,
					num2,
					num3,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSearchRolesFromDBCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				int num2 = Convert.ToInt32(array[2]);
				List<SearchRoleData> list = null;
				int num3 = -1;
				if (text2.Length > 0)
				{
					num3 = dbMgr.DBRoleMgr.FindDBRoleID(text2);
					if (-1 != num3)
					{
						DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num3);
						if (null != dbroleInfo)
						{
							list = new List<SearchRoleData>();
							SearchRoleData item = new SearchRoleData
							{
								RoleID = dbroleInfo.RoleID,
								RoleName = Global.FormatRoleName(dbroleInfo.ZoneID, dbroleInfo.RoleName),
								RoleSex = dbroleInfo.RoleSex,
								Level = dbroleInfo.Level,
								Occupation = dbroleInfo.Occupation,
								Faction = dbroleInfo.Faction,
								BHName = dbroleInfo.BHName
							};
							list.Add(item);
						}
					}
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(list, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBangGongHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiBagData>(bangHuiBagData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiBagData.BbangGongHistList = DBQuery.GetBangHuiBagHistList(dbMgr, bhid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiBagData>(bangHuiBagData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDonateBGMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1001,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiBangGong(dbMgr, num2, 0, 0, 0, 0, 0, num3);
				DBWriter.AddBangGongHistItem(dbMgr, num2, num, 0, 0, 0, 0, 0, num3, num4);
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					num,
					num2,
					num3,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDonateBGGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int goods1Num = Convert.ToInt32(array[2]);
				int goods2Num = Convert.ToInt32(array[3]);
				int goods3Num = Convert.ToInt32(array[4]);
				int goods4Num = Convert.ToInt32(array[5]);
				int goods5Num = Convert.ToInt32(array[6]);
				int num3 = Convert.ToInt32(array[7]);
				int tongQian = Convert.ToInt32(array[8]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1001,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiBangGong(dbMgr, num2, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, tongQian);
				DBWriter.AddBangGongHistItem(dbMgr, num2, num, goods1Num, goods2Num, goods3Num, goods4Num, goods5Num, tongQian, num3);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBangGongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				int num5 = Convert.ToInt32(array[4]);
				int num6 = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num7 = 0;
				lock (dbroleInfo)
				{
					if (num6 < 0 && dbroleInfo.BangGong < Math.Abs(num6))
					{
						flag = true;
					}
					else
					{
						dbroleInfo.BangGong = Math.Max(0, dbroleInfo.BangGong + num6);
						num7 = dbroleInfo.BangGong;
					}
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num6 != 0)
				{
					if (!DBWriter.UpdateRoleBangGong(dbMgr, num, num2, num3, num4, num5, num7))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色帮贡失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", num, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					lock (dbroleInfo)
					{
						dbroleInfo.BGDayID1 = num2;
						dbroleInfo.BGMoney = num3;
						dbroleInfo.BGDayID2 = num4;
						dbroleInfo.BGGoods = num5;
					}
				}
				data2 = string.Format("{0}:{1}", num, num7);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBHTongQianCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}", -1000, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					data2 = string.Format("{0}:{1}", -1010, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < num2 + GameDBManager.GameConfigMgr.GetGameConfigItemInt("ZhanMengZiJinInitialValue", 20000))
				{
					data2 = string.Format("{0}:{1}", -1110, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.SubBangHuiTongQian(dbMgr, bhid, num2);
				data2 = string.Format("{0}:{1}", 0, bangHuiDetailData.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddBHTongQianCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				int addMoney = Convert.ToInt32(array[2]);
				if (num > 0)
				{
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
					if (null == dbroleInfo)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}", -1000, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddBangHuiTongQian(dbMgr, bhid, addMoney);
				data2 = string.Format("{0}:{1}", 0, bangHuiDetailData.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBangQiInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				BangQiInfoData bangQiInfoData = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangQiInfoData>(bangQiInfoData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangQiInfoData = DBQuery.QueryBangQiInfoByID(dbMgr, bhid);
				bangQiInfoData.BHLingDiOwnDict = DBQuery.GetBHLingDiOwnDataDict(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangQiInfoData>(bangQiInfoData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRenameBangQiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				string qiName = array[2];
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiBagData)
				{
					data2 = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < num2)
				{
					data2 = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiName(dbMgr, bhid, qiName, num2);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiName(bhid, qiName);
				data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpLevelBangQiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int bhid = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				int num4 = Convert.ToInt32(array[4]);
				int num5 = Convert.ToInt32(array[5]);
				int num6 = Convert.ToInt32(array[6]);
				int num7 = Convert.ToInt32(array[7]);
				int num8 = Convert.ToInt32(array[8]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num != bangHuiDetailData.BZRoleID)
				{
					data2 = string.Format("{0}", -9368);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.QiLevel + 1 != num8)
				{
					data2 = string.Format("{0}", -1005);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiBagData bangHuiBagData = DBQuery.QueryBangHuiBagDataByID(dbMgr, bhid);
				if (null == bangHuiBagData)
				{
					data2 = string.Format("{0}", -1010);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.TongQian < num7)
				{
					data2 = string.Format("{0}", -1110);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods1Num < num2)
				{
					data2 = string.Format("{0}", -1111);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods2Num < num3)
				{
					data2 = string.Format("{0}", -1112);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods3Num < num4)
				{
					data2 = string.Format("{0}", -1113);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods4Num < num5)
				{
					data2 = string.Format("{0}", -1114);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiBagData.Goods5Num < num6)
				{
					data2 = string.Format("{0}", -1115);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiLevel(dbMgr, bhid, num8, num2, num3, num4, num5, num6, num7);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, num8);
				data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGMUpdateBangLevel(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(array[0]);
				int num = Convert.ToInt32(array[1]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}", -1000);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBangHuiQiLevel(dbMgr, bhid, num, 0, 0, 0, 0, 0, 0);
				GameDBManager.BangHuiJunQiMgr.UpdateBangHuiQiLevel(bhid, num);
				data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBHJunQiListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiJunQiMgr.GetBangHuiJunQiItemsDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBHLingDiDictCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiLingDiMgr.GetBangHuiLingDiItemsDictTCPOutPacket(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateLingDiForBHCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string data2;
				if (num2 <= 0)
				{
					BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.ClearLingDiBangHuiInfo(num);
					if (null != bangHuiLingDiInfoData)
					{
						DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
					}
					if (num == 2)
					{
						bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.ClearLingDiBangHuiInfo(1);
						if (null != bangHuiLingDiInfoData)
						{
							DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
						}
					}
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
					if (null == bangHuiDetailData)
					{
						data2 = string.Format("{0}", -1000);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddBangHuiLingDi(num2, bangHuiDetailData.ZoneID, bangHuiDetailData.BHName, num);
					if (null != bangHuiLingDiInfoData)
					{
						DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
					}
					if (num == 2)
					{
						bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddBangHuiLingDi(num2, bangHuiDetailData.ZoneID, bangHuiDetailData.BHName, 1);
						if (null != bangHuiLingDiInfoData)
						{
							DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
						}
					}
				}
				data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetLeaderRoleIDByBHIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bhid = Convert.ToInt32(array[0]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}", 0, "", "");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}", bangHuiDetailData.BZRoleID, Global.FormatRoleName(bangHuiDetailData.ZoneID, bangHuiDetailData.BZRoleName), Global.FormatBangHuiName(bangHuiDetailData.ZoneID, bangHuiDetailData.BHName));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetBHLingDiInfoDictByBHIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				Dictionary<int, BangHuiLingDiInfoData> instance = null;
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BHID != num2 || bangHuiDetailData.BZRoleID != num)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BangHuiLingDiInfoData>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = GameDBManager.BangHuiLingDiMgr.GetBangHuiLingDiInfosDictTCPOutPacket(pool, num2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSetLingDiTaxCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1005,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.UpdateBangHuiLingDiTax(num2, num3, num4);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					num,
					num2,
					num3,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSetLingDiWarRequestCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.UpdateBangHuiLingDiWarRequest(num, text2);
				string data2;
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						-1,
						-1,
						num,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					-1,
					num,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessTakeLingDiDailyAwardCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(num3);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayOfYear = DateTime.Now.DayOfYear;
				if (dayOfYear == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-1040,
							num,
							num2,
							num3
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.TakeLingDiDailyAward(num2, num3);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1050,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessTakeLingDiTaxMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1000,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1005,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(num3);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1010,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1020,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if ((double)num4 > (double)bangHuiLingDiInfoData.TotalTax * 0.25)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1030,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayOfYear = DateTime.Now.DayOfYear;
				if (dayOfYear == bangHuiLingDiInfoData.TakeDayID)
				{
					if (bangHuiLingDiInfoData.TakeDayNum >= 1)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1040,
							num,
							num2,
							num3,
							num4
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.TakeLingDiTaxMoney(num2, num3, num4);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-1050,
						num,
						num2,
						num3,
						num4
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				DBWriter.AddBangHuiTongQian(dbMgr, num2, num4);
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					num,
					num2,
					num3,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddLingDiTaxMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						num3
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.AddLingDiTaxMoney(num, num2, num3);
				if (null != bangHuiLingDiInfoData)
				{
					DBWriter.UpdateBHLingDi(dbMgr, bangHuiLingDiInfoData);
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetHuangDiBHInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				BangHuiDetailData bangHuiDetailData = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(6);
				if (bangHuiLingDiInfoData == null || bangHuiLingDiInfoData.BHID <= 0)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bangHuiLingDiInfoData.BHID);
				if (null == bangHuiDetailData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bangHuiDetailData.MgrItemList = DBQuery.GetBangHuiMgrItemItemDataList(dbMgr, bangHuiDetailData.BHID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BangHuiDetailData>(bangHuiDetailData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryQiZhenGeBuyHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				List<QizhenGeBuItemData> instance = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<QizhenGeBuItemData>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				instance = QiZhenGeBuManager.GetQizhenGeBuItemDataList(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<QizhenGeBuItemData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetHuangDiRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				RoleDataEx roleDataEx = new RoleDataEx();
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo)
				{
					roleDataEx.RoleID = -1;
				}
				else
				{
					Global.DBRoleInfo2RoleDataEx(dbroleInfo, roleDataEx);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataEx>(roleDataEx, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddHuangFeiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string text2 = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbroleInfo.Faction);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbroleInfo.Faction)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBQuery.QueryHuangFeiCount(dbMgr) >= 3)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1030,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleToHuangFei(dbMgr, num2, 1);
				DBRoleInfo dbroleInfo2 = dbMgr.DBRoleMgr.FindDBRoleInfo(ref num2);
				if (null != dbroleInfo2)
				{
					lock (dbroleInfo2)
					{
						dbroleInfo2.HuangHou = 1;
					}
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveHuangFeiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string text2 = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbroleInfo.Faction);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbroleInfo.Faction)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleToHuangFei(dbMgr, num2, 0);
				DBRoleInfo dbroleInfo2 = dbMgr.DBRoleMgr.FindDBRoleInfo(ref num2);
				if (null != dbroleInfo2)
				{
					lock (dbroleInfo2)
					{
						dbroleInfo2.HuangHou = 0;
					}
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetHuangFeiDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				List<SearchRoleData> instance = DBQuery.QueryHuangFeiDataList(dbMgr);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSendToLaoFangCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string text2 = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, dbroleInfo.Faction);
				string data2;
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1000,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1005,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiLingDiInfoData bangHuiLingDiInfoData = GameDBManager.BangHuiLingDiMgr.FindBangHuiLingDiByID(2);
				if (null == bangHuiLingDiInfoData)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1010,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiLingDiInfoData.BHID != dbroleInfo.Faction)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1020,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (HuangDiTeQuanMgr.FindHuangDiToOtherRoleDict(nID, num2))
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1025,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!HuangDiTeQuanMgr.CanExecuteHuangDiTeQuanNow(nID))
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1026,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!HuangDiTeQuanMgr.AddHuanDiTeQuan(nID, num2))
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1030,
						num,
						num2,
						text2
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateHuangDiTeQuan(dbMgr, HuangDiTeQuanMgr.GetHuangDiTeQuanItem());
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddRefreshQiZhenRecCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int oldUserMoney = Convert.ToInt32(array[1]);
				int leftUserMoney = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddRefreshQiZhenGeRec(dbMgr, num, oldUserMoney, leftUserMoney);
				string data2 = string.Format("{0}:{1}", 0, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessClrCachingRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				if (num == 0)
				{
					num = dbMgr.DBRoleMgr.FindDBRoleID(text2);
					if (num > 0)
					{
						dbMgr.DBRoleMgr.ReleaseDBRoleInfoByID(num);
					}
				}
				else if (num == 1)
				{
					dbMgr.dbUserMgr.RemoveDBUserInfo(text2);
				}
				else if (num > 100)
				{
					dbMgr.DBRoleMgr.ReleaseDBRoleInfoByID(num);
				}
				string data2 = string.Format("{0}:{1}", 0, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessClrAllCachingRoleDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				dbMgr.DBRoleMgr.ClearAllDBroleInfo();
				string data2 = string.Format("{0}:{1}", 0, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddMoneyWarningCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int usedMoney = Convert.ToInt32(array[1]);
				int goodsMoney = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.AddMoneyWarning(dbMgr, num, usedMoney, goodsMoney);
				string data2 = string.Format("{0}:{1}", 0, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetGoodsByDbIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsDbID = Convert.ToInt32(array[1]);
				GoodsData instance = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsData>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				instance = Global.GetGoodsDataByDbID(dbroleInfo, goodsDbID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int num = Convert.ToInt32(array[1]);
				int num2 = 0;
				int num3 = 0;
				DBQuery.QueryUserMoneyByUserID(dbMgr, userID, out num2, out num3);
				string data2 = string.Format("{0}", num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryUserIdValueCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int[] array2 = new int[2];
				DBQuery.QueryUserUserIdValue(dbMgr, userID, out array2[0], out array2[1]);
				array2[0] *= GameDBManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 1);
				byte[] array3 = DataHelper.ObjectToBytes<int[]>(array2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array3, 0, array3.Length, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryDayChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				int zoneID = Convert.ToInt32(array[1]);
				int num = 0;
				int num2 = 0;
				DBQuery.QueryTodayUserMoneyByUserID(dbMgr, userID, zoneID, out num, out num2);
				int num3 = 0;
				int num4 = 0;
				DBQuery.QueryTodayUserMoneyByUserID2(dbMgr, userID, zoneID, out num3, out num4);
				string data2 = string.Format("{0}", num2 + num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryPeriodChongZhiMoneyCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string s = array[1].Replace('$', ':');
				string s2 = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime fromDate;
				DateTime.TryParse(s, out fromDate);
				DateTime toDate;
				DateTime.TryParse(s2, out toDate);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int rankValue = dbroleInfo.RankValue.GetRankValue(key);
				string data2 = string.Format("{0}", rankValue);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddBuyItemFromNpcCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftMoney = Convert.ToInt32(array[4]);
				int moneyType = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewBuyItemFromNpc(dbMgr, num, goodsID, goodsNum, totalPrice, leftMoney, moneyType);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddYinLiangBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftYinLiang = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewYinLiangBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftYinLiang);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的银两购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddBangGongBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftBangGong = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewBangGongBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftBangGong);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的帮贡购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetUserMailListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				List<MailData> instance = Global.LoadUserMailItemDataList(dbMgr, rid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<MailData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetUserMailCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int excludeReadState = Convert.ToInt32(array[1]);
				int limitCount = Convert.ToInt32(array[2]);
				int instance = Global.LoadUserMailItemDataCount(dbMgr, rid, excludeReadState, limitCount);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetUserMailDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int mailID = Convert.ToInt32(array[1]);
				MailData instance = Global.LoadMailItemData(dbMgr, rid, mailID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MailData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSendUserMailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 10 && array.Length != 11)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[2]);
				if (array.Length == 11)
				{
					int num3 = Convert.ToInt32(array[10]);
					if (num3 != 0 && DBManager.getInstance().GetDBRoleInfo(ref num2) == null)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num4 = 0;
				int num5 = Global.AddMail(dbMgr, array, out num4);
				string data2 = string.Format("{0}:{1}:{2}", num, num5, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessFetchMailGoodsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				bool flag = Global.UpdateHasFetchMailGoodsStat(dbMgr, num, num2);
				string data2 = string.Format("{0}:{1}:{2}", num, num2, flag ? 1 : -1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteUserMailCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				bool flag = false;
				string[] array2 = text2.Split(new char[]
				{
					','
				});
				string text3 = null;
				if (array2 != null)
				{
					for (int i = 0; i < array2.Length; i++)
					{
						flag = Global.DeleteMail(dbMgr, num, array2[i]);
						if (flag)
						{
							string str = array2[i] + ",";
							text3 += str;
						}
					}
				}
				string data2 = string.Format("{0}:{1}:{2}", num, text3, flag ? 1 : -1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetRoleIDByRoleNameCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[0].Replace('$', ':');
				int num = Global.FindDBRoleID(dbMgr, text2);
				string data2 = string.Format("{0}:{1}", num, num, text2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryInputFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				double num2 = Convert.ToDouble(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num3 = dbroleInfo.RankValue.GetRankValue(key);
				if (num3 < 0)
				{
					num3 = 0;
				}
				int num4 = num3;
				int num5 = (int)((double)num4 * num2);
				data2 = string.Format("{0}:{1}:{2}", 1, num, num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryInputJiaSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1002, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int rankValue = dbroleInfo.RankValue.GetRankValue(key);
				if (rankValue <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1004, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = rankValue;
				if (num3 < num2)
				{
					data2 = string.Format("{0}:{1}:0", -1005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryInputKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, 3);
				foreach (InputKingPaiHangData inputKingPaiHangData in inputKingPaiHangListByHuoDongLimit)
				{
					Global.GetUserMaxLevelRole(dbMgr, inputKingPaiHangData.UserID, out inputKingPaiHangData.MaxLevelRoleName, out inputKingPaiHangData.MaxLevelRoleZoneID);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<InputKingPaiHangData>>(inputKingPaiHangListByHuoDongLimit, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryLevelKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, array, 5, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryEquipKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, array, 6, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryHorseKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, array, 7, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryJingMaiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.GetHuoDongPaiHangForKing(dbMgr, pool, nID, array, 8, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteInputFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				double num2 = Convert.ToDouble(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int rankValue = dbroleInfo.RankValue.GetRankValue(key);
				if (rankValue <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 2, huoDongKeyString, out num3, out text2);
					if (num3 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 2, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1006, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int num5 = rankValue;
				data2 = string.Format("{0}:{1}:{2}", 1, num, num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteInputJiaSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1002, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int rankValue = dbroleInfo.RankValue.GetRankValue(key);
				if (rankValue <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = rankValue;
				if (num3 < num2)
				{
					data2 = string.Format("{0}:{1}:0", -10007, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 3, huoDongKeyString, out num4, out text2);
					if (num4 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num5 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 3, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num5 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1007, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteInputKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, 3);
				int num2 = -1;
				int num3 = 0;
				for (int j = 0; j < inputKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (dbroleInfo.UserID == inputKingPaiHangListByHuoDongLimit[j].UserID)
					{
						num2 = inputKingPaiHangListByHuoDongLimit[j].PaiHang;
						num3 = inputKingPaiHangListByHuoDongLimit[j].PaiHangValue;
					}
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1003, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num3 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10007, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 4, huoDongKeyString, out num4, out text2);
					if (num4 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num5 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 4, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num5 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteLevelKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, array, 5, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteEquipKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, array, 6, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteHorseKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, array, 7, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteJingMaiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return Global.ProcessHuoDongForKing(dbMgr, pool, nID, array, 8, out tcpOutPacket);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryAwardHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Global.SafeConvertToInt32(array[3], 10);
				int num3 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				if (2 == num2 || 3 == num2 || 4 == num2 || 69 == num2)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, num2, huoDongKeyString, out num4, out text2);
					if (69 == num2)
					{
						if (num4 > 0)
						{
							num4 = (int)Global.GetBitRangeValue((long)num4, (num3 - 1) / 2 * 7, 7);
						}
					}
				}
				else
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, num2, huoDongKeyString, out num4, out text2);
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num2,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprQueryUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string keystr = array[1];
				int num2 = Global.SafeConvertToInt32(array[2], 10);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long num3 = 0L;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, num2, keystr, out num3, out text2);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					awardHistoryForUser,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSprUpdateUserActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string keystr = array[1];
				int num2 = Global.SafeConvertToInt32(array[2], 10);
				long num3 = Global.SafeConvertToInt64(array[3], 10);
				string lastgettime = array[4];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = 0;
				lock (dbroleInfo)
				{
					num4 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, num3, lastgettime);
					if (num4 < 0)
					{
						num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, num2, keystr, num3, lastgettime);
					}
				}
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num4,
					num,
					num2,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDBQueryLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int num2 = 0;
				int num3 = 0;
				int num4 = DBQuery.QueryLimitGoodsUsedNumByRoleID(dbMgr, num, goodsID, out num2, out num3);
				string data2;
				if (num4 < 0)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num4,
						num2,
						num3
					});
					LogManager.WriteLog(LogTypes.Error, string.Format("通过角色ID和物品ID查询物品每日的已经购买数量失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						0,
						num2,
						num3
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDBUpdateLimitGoodsUsedNumCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int dayID = Convert.ToInt32(array[2]);
				int usedNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddLimitGoodsBuyItem(dbMgr, num, goodsID, dayID, usedNum);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加限购物品的历史记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateDailyVipDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int dayID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int usedTimes = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					if (null == dbroleInfo.VipDailyDataList)
					{
						dbroleInfo.VipDailyDataList = new List<VipDailyData>();
					}
					bool flag2 = false;
					VipDailyData vipDailyData = null;
					for (int i = 0; i < dbroleInfo.VipDailyDataList.Count; i++)
					{
						if (dbroleInfo.VipDailyDataList[i].PriorityType == num2)
						{
							flag2 = true;
							vipDailyData = dbroleInfo.VipDailyDataList[i];
							break;
						}
					}
					if (!flag2)
					{
						vipDailyData = new VipDailyData();
						dbroleInfo.VipDailyDataList.Add(vipDailyData);
					}
					vipDailyData.DayID = dayID;
					vipDailyData.PriorityType = num2;
					vipDailyData.UsedTimes = usedTimes;
				}
				string data2;
				if (DBWriter.AddVipDailyData(dbMgr, num, num2, dayID, usedTimes) >= 0)
				{
					data2 = string.Format("1:{0}:{1}", num, num2);
				}
				else
				{
					data2 = string.Format("-1:{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateYangGongBKDailyJiFenDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int dayID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (DBWriter.AddYangGongBKDailyJiFenData(dbMgr, num, num2, dayID, (long)num3) >= 0)
				{
					data2 = string.Format("1:{0}:{1}", num, num2);
					if (null == dbroleInfo.YangGongBKDailyJiFen)
					{
						dbroleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData();
					}
					dbroleInfo.YangGongBKDailyJiFen.DayID = dayID;
					dbroleInfo.YangGongBKDailyJiFen.JiFen = num2;
					dbroleInfo.YangGongBKDailyJiFen.AwardHistory = (long)num3;
				}
				else
				{
					data2 = string.Format("-1:{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateSingleTimeAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				long num2 = Convert.ToInt64(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleOnceAwardFlag(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色充值任务ID时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.OnceAwardFlag = num2;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddShengXiaoGuessHisotryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] array2 = array[0].Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[0].Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						int num = Convert.ToInt32(array3[0]);
						DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
						if (null == dbroleInfo)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("记录竞猜历史是需要处理的的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						}
						else
						{
							string roleName = dbroleInfo.RoleName;
							int zoneID = dbroleInfo.ZoneID;
							string[] array4 = array3[1].Split(new char[]
							{
								'|'
							});
							for (int j = 0; j < array4.Length; j++)
							{
								string[] array5 = array4[j].Split(new char[]
								{
									'_'
								});
								if (array5.Length == 5)
								{
									int guessKey = Convert.ToInt32(array5[0]);
									int mortgage = Convert.ToInt32(array5[1]);
									int resultKey = Convert.ToInt32(array5[2]);
									int gainNum = Convert.ToInt32(array5[3]);
									int leftMortgage = Convert.ToInt32(array5[4]);
									int num2 = DBWriter.AddNewShengXiaoGuessHistory(dbMgr, num, roleName, zoneID, guessKey, mortgage, resultKey, gainNum, leftMortgage);
									if (num2 < 0)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("添加新的生肖竞猜记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
									}
								}
							}
						}
					}
				}
				string data2 = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryShengXiaoGuessHistCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				List<ShengXiaoGuessHistory> instance = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ShengXiaoGuessHistory>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				instance = DBQuery.QueryShengXiaoGuessHistoryDataList(dbMgr, Convert.ToInt32(array[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ShengXiaoGuessHistory>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateUserGoldCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				int num3 = 0;
				lock (dbroleInfo)
				{
					dbroleInfo.Gold += num2;
					num3 = dbroleInfo.Gold;
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0)
				{
					if (!DBWriter.UpdateRoleGold(dbMgr, num, num3))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", num, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddGoldBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftGold = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.AddNewGoldBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftGold);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的金币购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateGoodsLimitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int dayID = Convert.ToInt32(array[2]);
				int usedNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateGoodsLimit(dbMgr, num, goodsID, dayID, usedNum))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色物品限制时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					Global.UpdateGoodsLimitByID(dbroleInfo, goodsID, dayID, usedNum);
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleParamCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string name = array[1];
				string value = array[2];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Global.UpdateRoleParamByName(dbMgr, dbroleInfo, name, value, null);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetRoleParamCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				string name = array[1];
				string roleParams = DBWriter.GetRoleParams(roleID, name);
				string data2 = string.Format("{0}", roleParams);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateWebOldPlayerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string roleID = array[0];
				string chouJiangType = "v" + array[1];
				string addDay = array[2].Replace('$', ':');
				bool flag = DBWriter.UpdateWebOldPlayer(roleID, chouJiangType, addDay);
				string data2 = string.Format("{0}", 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddQiangGouBuyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int goodsNum = Convert.ToInt32(array[2]);
				int totalPrice = Convert.ToInt32(array[3]);
				int leftMoney = Convert.ToInt32(array[4]);
				int qiangGouId = Convert.ToInt32(array[5]);
				int actStartDay = Convert.ToInt32(array[6]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = Global.AddNewQiangGouBuyItem(dbMgr, num, goodsID, goodsNum, totalPrice, leftMoney, qiangGouId, actStartDay);
				string data2;
				if (num2 < 0)
				{
					data2 = string.Format("{0}:{1}", num, num2);
					LogManager.WriteLog(LogTypes.Error, string.Format("添加新的限时抢购购买记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryQiangGouBuyItemInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int goodsID = Convert.ToInt32(array[1]);
				int qiangGouId = Convert.ToInt32(array[2]);
				int random = Convert.ToInt32(array[3]);
				int actStartDay = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				int num3 = 0;
				Global.QueryQiangGouBuyItemInfo(dbMgr, num, goodsID, qiangGouId, random, actStartDay, out num2, out num3);
				string data2 = string.Format("{0}:{1}:{2}", num, num2, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddZaJinDanHisotryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (!GameDBManager.DisableSomeLog)
				{
					string[] array2 = array[0].Split(new char[]
					{
						';'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split(new char[]
						{
							'_'
						});
						if (array3.Length >= 12)
						{
							int num = Convert.ToInt32(array3[0]);
							string roleName;
							string text2;
							Global.GetRoleNameAndUserID(dbMgr, num, out roleName, out text2);
							int zoneID = Convert.ToInt32(array3[2]);
							int timesselected = Convert.ToInt32(array3[3]);
							int usedyuanbao = Convert.ToInt32(array3[4]);
							int usedjindan = Convert.ToInt32(array3[5]);
							int gaingoodsid = Convert.ToInt32(array3[6]);
							int gaingoodsnum = Convert.ToInt32(array3[7]);
							int gaingold = Convert.ToInt32(array3[8]);
							int gainyinliang = Convert.ToInt32(array3[9]);
							int gainexp = Convert.ToInt32(array3[10]);
							string srtProp = array3[11];
							int num2 = DBWriter.AddNewZaJinDanHistory(dbMgr, num, roleName, zoneID, timesselected, usedyuanbao, usedjindan, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp, srtProp);
							if (num2 < 0)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("添加新的砸金蛋记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
							}
						}
					}
				}
				string data2 = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryZaJinDanHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				List<ZaJinDanHistory> instance = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ZaJinDanHistory>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				instance = DBQuery.QueryZaJinDanHistoryDataList(dbMgr, Convert.ToInt32(array[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<ZaJinDanHistory>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryFirstChongZhiDaLiByUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int firstChongZhiDaLiNum = DBQuery.GetFirstChongZhiDaLiNum(dbMgr, dbroleInfo.UserID);
				data2 = string.Format("{0}", firstChongZhiDaLiNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryDayChongZhiDaLiByUserIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = 0;
				data2 = string.Format("{0}", num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryKaiFuOnlineAwardRoleIDCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int dayID = Convert.ToInt32(array[0]);
				int num = 0;
				int kaiFuOnlineAwardRoleID = DBQuery.GetKaiFuOnlineAwardRoleID(dbMgr, dayID, out num);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref kaiFuOnlineAwardRoleID);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-1,
						0,
						"",
						0
					});
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						kaiFuOnlineAwardRoleID,
						dbroleInfo.ZoneID,
						dbroleInfo.RoleName,
						num
					});
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddKaiFuOnlineAwardCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int dayID = Convert.ToInt32(array[1]);
				int yuanBao = Convert.ToInt32(array[2]);
				int totalRoleNum = Convert.ToInt32(array[3]);
				int zoneID = Convert.ToInt32(array[4]);
				int num = DBWriter.AddKaiFuOnlineAward(dbMgr, rid, dayID, yuanBao, totalRoleNum, zoneID);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryKaiFuOnlineAwardListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(array[1]);
				List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList = DBQuery.GetKaiFuOnlineAwardDataList(dbMgr, zoneID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<KaiFuOnlineAwardData>>(kaiFuOnlineAwardDataList, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddGiveUserMoneyItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int yuanBao = Convert.ToInt32(array[1]);
				string giveType = array[2];
				int num = DBWriter.AddSystemGiveUserMoney(dbMgr, rid, yuanBao, giveType);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddExchange1ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int goodsid = Convert.ToInt32(array[1]);
				int goodsnum = Convert.ToInt32(array[2]);
				int leftgoodsnum = Convert.ToInt32(array[3]);
				int otherroleid = Convert.ToInt32(array[4]);
				string result = array[5];
				int num = DBWriter.AddExchange1Item(dbMgr, rid, goodsid, goodsnum, leftgoodsnum, otherroleid, result);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddExchange2ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int yinliang = Convert.ToInt32(array[1]);
				int leftyinliang = Convert.ToInt32(array[2]);
				int otherroleid = Convert.ToInt32(array[3]);
				int num = DBWriter.AddExchange2Item(dbMgr, rid, yinliang, leftyinliang, otherroleid);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddExchange3ItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int yuanbao = Convert.ToInt32(array[1]);
				int leftyuanbao = Convert.ToInt32(array[2]);
				int otherroleid = Convert.ToInt32(array[3]);
				int num = DBWriter.AddExchange3Item(dbMgr, rid, yuanbao, leftyuanbao, otherroleid);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddFallGoodsItemCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 12)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int autoid = Convert.ToInt32(array[1]);
				int goodsdbid = Convert.ToInt32(array[2]);
				int goodsid = Convert.ToInt32(array[3]);
				int goodsnum = Convert.ToInt32(array[4]);
				int binding = Convert.ToInt32(array[5]);
				int quality = Convert.ToInt32(array[6]);
				int forgelevel = Convert.ToInt32(array[7]);
				string jewellist = array[8];
				string mapname = array[9];
				string goodsgrid = array[10];
				string fromname = array[11];
				int num = DBWriter.AddFallGoodsItem(dbMgr, rid, autoid, goodsdbid, goodsid, goodsnum, binding, quality, forgelevel, jewellist, mapname, goodsgrid, fromname);
				string data2 = string.Format("{0}", num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRolePropsCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				long num3 = Convert.ToInt64(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				switch (num2)
				{
				case 1:
					flag = DBWriter.UpdateRoleBanProps(dbMgr, num, "banchat", num3);
					break;
				case 2:
					flag = DBWriter.UpdateRoleBanProps(dbMgr, num, "banlogin", num3);
					break;
				case 3:
					flag = DBWriter.UpdateRoleBanProps(dbMgr, num, "ban_trade_to_ticks", num3);
					break;
				}
				string data2;
				if (!flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色属性值时失败，CMD={0}, RoleID={1}, PropIndex={2}", (TCPGameServerCmds)nID, num, num2), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						switch (num2)
						{
						case 1:
							dbroleInfo.BanChat = (int)num3;
							break;
						case 2:
							dbroleInfo.BanLogin = (int)num3;
							break;
						case 3:
							dbroleInfo.BanTradeToTicks = num3;
							break;
						}
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryThemeDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 151, huoDongKeyString, out num2, out text2);
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 9, huoDongKeyString, out num2, out text2);
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriDengLuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 10, huoDongKeyString, out num3, out text2);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num3,
					num2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 11, huoDongKeyString, out num3, out text2);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num3,
					num2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 12, huoDongKeyString, out num3, out text2);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num4 = dbroleInfo.RankValue.GetRankValue(key);
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = num4;
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					num,
					num2,
					num5,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriCZLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 13, huoDongKeyString, out num2, out text2);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num3 = dbroleInfo.RankValue.GetRankValue(key);
				if (num3 < 0)
				{
					num3 = 0;
				}
				int num4 = num3;
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num4,
					num2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieRiMeiRiLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 70, huoDongKeyString, out num3, out text2);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num4 = dbroleInfo.RankValue.GetRankValue(key);
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = num4;
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num5,
					num3
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriTotalConsumeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 40, huoDongKeyString, out num2, out text2);
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int num3 = dbroleInfo.RankValue.GetRankValue(key);
				if (num3 < 0)
				{
					num3 = 0;
				}
				int num4 = num3;
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num4,
					num2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriXiaoFeiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				int num2 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				List<InputKingPaiHangData> usedMoneyKingPaiHangListByHuoDongLimit = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, list.Count);
				foreach (InputKingPaiHangData inputKingPaiHangData in usedMoneyKingPaiHangListByHuoDongLimit)
				{
					string text2;
					Global.GetRoleNameAndUserID(dbMgr, Global.SafeConvertToInt32(inputKingPaiHangData.UserID, 10), out inputKingPaiHangData.MaxLevelRoleName, out text2);
				}
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int num3 = dbroleInfo.RankValue.GetRankValue(key);
				if (num3 < 0)
				{
					num3 = 0;
				}
				int yuanBao = num3;
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text3 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 15, huoDongKeyString, out num4, out text3);
				int num5 = -1;
				for (int j = 0; j < usedMoneyKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (dbroleInfo.RoleID.ToString() == usedMoneyKingPaiHangListByHuoDongLimit[j].UserID)
					{
						num5 = usedMoneyKingPaiHangListByHuoDongLimit[j].PaiHang;
					}
				}
				if (num3 <= 0)
				{
					num5 = -1;
				}
				if (0 == num2)
				{
					JieriCZKingData instance = new JieriCZKingData
					{
						YuanBao = yuanBao,
						ListPaiHang = usedMoneyKingPaiHangListByHuoDongLimit,
						State = num4
					};
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(instance, pool, nID);
				}
				else if (1 == num2)
				{
					string data2 = string.Format("{0}:{1}:{2}", 1, num, (num5 > 0 && num4 == 0) ? "1" : "0");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessInputPointsExchangeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				num3 = Math.Max(0, num3);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 64, huoDongKeyString, out num4, out text2);
				if (num4 <= 0)
				{
					int num5 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 64, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num5 < 0)
					{
						data2 = string.Format("{0}:{1}", num, -1006);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", num4, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteDanBiChongZhiJiangLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 69)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				long num4 = 0L;
				string text2 = "";
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num5 = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 69, huoDongKeyString, out num4, out text2);
				num4 = ((num4 > 0L) ? num4 : 0L);
				long num6 = num4;
				long bitRangeValue = Global.GetBitRangeValue(num4, 0, (num3 - 1) * 7);
				num6 >>= (num3 - 1) * 7;
				num6 += 1L;
				num6 <<= (num3 - 1) * 7;
				num4 = num6 + bitRangeValue;
				num5 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 69, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				if (num5 < 0)
				{
					num5 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 69, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				}
				if (num5 < 0)
				{
					data2 = string.Format("{0}:{1}:0", -1008, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryDanBiChongZhiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, int> userInputMoneyCount = dbroleInfo.RankValue.GetUserInputMoneyCount(dbMgr, dbroleInfo.UserID, dbroleInfo.ZoneID, fromDate, toDate);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				long num2 = 0L;
				string text2 = "";
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 69, huoDongKeyString, out num2, out text2);
				for (int i = 0; i < array2.Length; i += 2)
				{
					string key = string.Format("{0}_{1}", array2[i], array2[i + 1]);
					int num3 = Convert.ToInt32(array2[i]);
					int num4 = Convert.ToInt32(array2[i + 1]);
					if (num4 == -1)
					{
						num4 = int.MaxValue;
					}
					int num5 = 0;
					foreach (KeyValuePair<int, int> keyValuePair in userInputMoneyCount)
					{
						int key2 = keyValuePair.Key;
						if (key2 <= num4 && key2 >= num3)
						{
							num5 += keyValuePair.Value;
						}
					}
					int num6 = 0;
					if (num2 > 0L)
					{
						num6 = (int)Global.GetBitRangeValue(num2, i / 2 * 7, 7);
					}
					dictionary[key] = num5.ToString() + "_" + num6;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(dictionary, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryJieriCZKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				int num2 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, list.Count);
				foreach (InputKingPaiHangData inputKingPaiHangData in inputKingPaiHangListByHuoDongLimit)
				{
					Global.GetUserMaxLevelRole(dbMgr, inputKingPaiHangData.UserID, out inputKingPaiHangData.MaxLevelRoleName, out inputKingPaiHangData.MaxLevelRoleZoneID);
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num3 = dbroleInfo.RankValue.GetRankValue(key);
				if (num3 < 0)
				{
					num3 = 0;
				}
				int yuanBao = num3;
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 16, huoDongKeyString, out num4, out text2);
				int num5 = -1;
				for (int j = 0; j < inputKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (dbroleInfo.UserID == inputKingPaiHangListByHuoDongLimit[j].UserID)
					{
						num5 = inputKingPaiHangListByHuoDongLimit[j].PaiHang;
					}
				}
				if (num3 <= 0)
				{
					num5 = -1;
				}
				if (0 == num2)
				{
					JieriCZKingData instance = new JieriCZKingData
					{
						YuanBao = yuanBao,
						ListPaiHang = inputKingPaiHangListByHuoDongLimit,
						State = num4
					};
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(instance, pool, nID);
				}
				else if (1 == num2)
				{
					string data2 = string.Format("{0}:{1}:{2}", 1, num, (num5 > 0 && num4 == 0) ? "1" : "0");
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteThemeDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 151, huoDongKeyString, out num2, out text2);
					if (num2 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num2 = 1;
					int num3 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 151, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num2 = 0;
				string text2 = "";
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 9, huoDongKeyString, out num2, out text2);
					if (num2 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num2 = 1;
					int num3 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 9, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num3 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriDengLuCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				num3 = Math.Max(0, num3);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				int awardHistoryForRole = DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 10, huoDongKeyString, out num4, out text2);
				int bitValue = Global.GetBitValue(num3);
				if ((num4 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 < num3)
				{
					data2 = string.Format("{0}:{1}:0", -10077, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					num4 |= 1 << num3 - 1;
					if (awardHistoryForRole < 0)
					{
						int num5 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 10, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num5 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num5 = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 10, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num5 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", num4, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10099, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 11, huoDongKeyString, out num3, out text2);
					if (num3 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num3 = 1;
					int num4 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 11, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 12, huoDongKeyString, out num4, out text2);
				int bitValue = Global.GetBitValue(num3);
				if ((num4 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num5 = dbroleInfo.RankValue.GetRankValue(key);
				if (num5 < 0)
				{
					num5 = 0;
				}
				int num6 = num5;
				if (num6 < num2)
				{
					data2 = string.Format("{0}:{1}:0", -10088, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					num4 |= 1 << num3 - 1;
					if (awardHistoryForUser < 0)
					{
						int num7 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 12, huoDongKeyString, (long)num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num7 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 12, huoDongKeyString, (long)num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriMeiRiLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 70, huoDongKeyString, out num4, out text2);
				int bitValue = Global.GetBitValue(num3);
				if ((num4 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num5 = dbroleInfo.RankValue.GetRankValue(key);
				if (num5 < 0)
				{
					num5 = 0;
				}
				int num6 = num5;
				if (num6 < num2)
				{
					data2 = string.Format("{0}:{1}:0", -10088, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					num4 |= 1 << num3 - 1;
					if (awardHistoryForUser < 0)
					{
						int num7 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 70, huoDongKeyString, (long)num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num7 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 70, huoDongKeyString, (long)num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriCZLeiJiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				int num2 = Convert.ToInt32(array[4]);
				num2 = Math.Max(0, num2);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 13, huoDongKeyString, out num3, out text2);
				int bitValue = Global.GetBitValue(num2);
				if ((num3 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num4 = dbroleInfo.RankValue.GetRankValue(key);
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = num4;
				int num6 = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if (num5 < list[j])
					{
						break;
					}
					num6 = j;
				}
				if (num6 < num2 - 1)
				{
					data2 = string.Format("{0}:{1}:0", -10088, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					num3 |= 1 << num2 - 1;
					if (awardHistoryForUser < 0)
					{
						int num7 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 13, huoDongKeyString, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num7 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 13, huoDongKeyString, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", num3, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriTotalConsumeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				int num2 = Convert.ToInt32(array[4]);
				num2 = Math.Max(0, num2);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				int awardHistoryForUser = DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 40, huoDongKeyString, out num3, out text2);
				int bitValue = Global.GetBitValue(num2);
				if ((num3 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RankDataKey key = new RankDataKey(RankType.Consume, fromDate, toDate, null);
				int num4 = dbroleInfo.RankValue.GetRankValue(key);
				if (num4 < 0)
				{
					num4 = 0;
				}
				int num5 = num4;
				int num6 = -1;
				for (int j = 0; j < list.Count; j++)
				{
					if (num5 < list[j])
					{
						break;
					}
					num6 = j;
				}
				if (num6 < num2 - 1)
				{
					data2 = string.Format("{0}:{1}:0", -10088, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					num3 |= 1 << num2 - 1;
					if (awardHistoryForUser < 0)
					{
						int num7 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 40, huoDongKeyString, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num7 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 40, huoDongKeyString, (long)num3, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num7 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", num3, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriXiaoFeiKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> usedMoneyKingPaiHangListByHuoDongLimit = Global.GetUsedMoneyKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, list.Count);
				int num2 = -1;
				int num3 = 0;
				for (int j = 0; j < usedMoneyKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (dbroleInfo.RoleID.ToString() == usedMoneyKingPaiHangListByHuoDongLimit[j].UserID)
					{
						num2 = usedMoneyKingPaiHangListByHuoDongLimit[j].PaiHang;
						num3 = usedMoneyKingPaiHangListByHuoDongLimit[j].PaiHangValue;
					}
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1003, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num3 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10007, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 15, huoDongKeyString, out num4, out text2);
					if (num4 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num5 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 15, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num5 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriCZKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, list.Count);
				int num2 = -1;
				int num3 = 0;
				for (int j = 0; j < inputKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (dbroleInfo.UserID == inputKingPaiHangListByHuoDongLimit[j].UserID)
					{
						num2 = inputKingPaiHangListByHuoDongLimit[j].PaiHang;
						num3 = inputKingPaiHangListByHuoDongLimit[j].PaiHangValue;
					}
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1003, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num3 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10007, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 16, huoDongKeyString, out num4, out text2);
					if (num4 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num5 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 16, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num5 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuPKKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					string data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int state = 0;
				string text2 = "";
				DBQuery.GetAwardHistoryForRole(dbMgr, num, dbroleInfo.ZoneID, 24, huoDongKeyString, out state, out text2);
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num2);
				HeFuPKKingData instance = new HeFuPKKingData
				{
					RoleID = ((dbroleInfo2 != null) ? dbroleInfo2.RoleID : 0),
					RoleName = ((dbroleInfo2 != null) ? dbroleInfo2.RoleName : ""),
					ZoneID = ((dbroleInfo2 != null) ? dbroleInfo2.ZoneID : 0),
					State = state
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HeFuPKKingData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuWCKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1].Replace('$', ':');
				string text3 = array[2].Replace('$', ':');
				int bhid = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					string data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingaward", 0);
				int state = gameConfigItemInt;
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, bhid);
				HeFuPKKingData instance = new HeFuPKKingData
				{
					RoleID = ((bangHuiDetailData != null) ? bangHuiDetailData.BHID : 0),
					RoleName = ((bangHuiDetailData != null) ? bangHuiDetailData.BHName : ""),
					ZoneID = ((bangHuiDetailData != null) ? bangHuiDetailData.ZoneID : 0),
					State = state
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HeFuPKKingData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryHeFuCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryXinCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string s = array[1].Replace('$', ':');
				string text2 = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				DateTime now = DateTime.Now;
				string fromDate = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string toDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, 5);
				foreach (InputKingPaiHangData inputKingPaiHangData in inputKingPaiHangListByHuoDongLimit)
				{
					Global.GetUserMaxLevelRole(dbMgr, inputKingPaiHangData.UserID, out inputKingPaiHangData.MaxLevelRoleName, out inputKingPaiHangData.MaxLevelRoleZoneID);
				}
				int state = 0;
				string text3 = "";
				DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(s, out dateTime);
				int num2 = 0;
				if (now.Ticks > dateTime.Ticks + 864000000000L)
				{
					DateTime addDaysDataTime = Global.GetAddDaysDataTime(now, -1, true);
					fromDate = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
					toDate = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
					List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit2 = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, 5);
					RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
					int num3 = dbroleInfo.RankValue.GetRankValue(key);
					if (num3 < 0)
					{
						num3 = 0;
					}
					num2 = num3;
					int num4 = 0;
					for (int j = 0; j < inputKingPaiHangListByHuoDongLimit2.Count; j++)
					{
						if (inputKingPaiHangListByHuoDongLimit2[j].UserID == dbroleInfo.UserID)
						{
							num4 = inputKingPaiHangListByHuoDongLimit2[j].PaiHang;
							break;
						}
					}
					if (num4 > 0)
					{
						double num5 = (double)list[num4 - 1] / 100.0;
						num2 = (int)(num5 * (double)num2);
					}
					else
					{
						num2 = 0;
					}
					string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 30, huoDongKeyString, out state, out text3);
				}
				JieriCZKingData instance = new JieriCZKingData
				{
					YuanBao = num2,
					ListPaiHang = inputKingPaiHangListByHuoDongLimit,
					State = state
				};
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JieriCZKingData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuDaLiBaoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuVIPCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuCZSongCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuPKKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != dbroleInfo.RoleID)
				{
					data2 = string.Format("{0}:{1}:0", -10089, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num3 = 0;
				string text2 = "";
				lock (dbroleInfo)
				{
					DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 24, huoDongKeyString, out num3, out text2);
					if (num3 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num3 = 1;
					int num4 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, 24, huoDongKeyString, 1, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num4 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuWCKingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1].Replace('$', ':');
				string text3 = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (dbroleInfo.Faction != num2)
				{
					data2 = string.Format("{0}:{1}:0", -10065, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				BangHuiDetailData bangHuiDetailData = DBQuery.QueryBangHuiInfoByID(dbMgr, num2);
				if (null == bangHuiDetailData)
				{
					data2 = string.Format("{0}:{1}:0", -10066, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (bangHuiDetailData.BZRoleID != num)
				{
					data2 = string.Format("{0}:{1}:0", -10067, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int gameConfigItemInt = GameDBManager.GameConfigMgr.GetGameConfigItemInt("hefuwckingaward", 0);
				int num3 = gameConfigItemInt;
				lock (dbroleInfo)
				{
					if (num3 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num3 = 1;
					GameDBManager.GameConfigMgr.UpdateGameConfigItem("hefuwckingaward", num3.ToString());
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteHeFuCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteXinCZFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string s = array[1].Replace('$', ':');
				string text2 = array[2].Replace('$', ':');
				string[] array2 = array[3].Split(new char[]
				{
					'_'
				});
				List<int> list = new List<int>();
				foreach (string str in array2)
				{
					list.Add(Global.SafeConvertToInt32(str, 10));
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				string text3 = "";
				DateTime now = DateTime.Now;
				DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(s, out dateTime);
				int num3 = 0;
				if (now.Ticks <= dateTime.Ticks + 864000000000L)
				{
					data2 = string.Format("{0}:{1}:0", -1002, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime addDaysDataTime = Global.GetAddDaysDataTime(now, -1, true);
				string fromDate = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 0, 0, 0).ToString("yyyy-MM-dd HH:mm:ss");
				string toDate = new DateTime(addDaysDataTime.Year, addDaysDataTime.Month, addDaysDataTime.Day, 23, 59, 59).ToString("yyyy-MM-dd HH:mm:ss");
				List<InputKingPaiHangData> inputKingPaiHangListByHuoDongLimit = Global.GetInputKingPaiHangListByHuoDongLimit(dbMgr, fromDate, toDate, list, 5);
				RankDataKey key = new RankDataKey(RankType.Charge, fromDate, toDate, null);
				int num4 = dbroleInfo.RankValue.GetRankValue(key);
				if (num4 < 0)
				{
					num4 = 0;
				}
				num3 = num4;
				int num5 = 0;
				for (int j = 0; j < inputKingPaiHangListByHuoDongLimit.Count; j++)
				{
					if (inputKingPaiHangListByHuoDongLimit[j].UserID == dbroleInfo.UserID)
					{
						num5 = inputKingPaiHangListByHuoDongLimit[j].PaiHang;
						break;
					}
				}
				if (num5 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -1003, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				double num6 = (double)list[num5 - 1] / 100.0;
				num3 = (int)(num6 * (double)num3);
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				if (num3 <= 0)
				{
					data2 = string.Format("{0}:{1}:0", -10006, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				lock (dbuserInfo)
				{
					DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 30, huoDongKeyString, out num2, out text3);
					if (num2 > 0)
					{
						data2 = string.Format("{0}:{1}:0", -10005, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num7 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbroleInfo.UserID, 30, huoDongKeyString, 1L, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
					if (num7 < 0)
					{
						data2 = string.Format("{0}:{1}:0", -1008, num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				data2 = string.Format("{0}:{1}:{2}", 1, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryActivityInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (null == text2)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime dateTime = new DateTime(2000, 1, 1, 0, 0, 0);
				DateTime.TryParse(text2, out dateTime);
				DateTime addDaysDataTime = Global.GetAddDaysDataTime(dateTime, 3, true);
				string text3 = addDaysDataTime.ToString("yyyy-MM-dd HH:mm:ss");
				string huoDongKeyString = Global.GetHuoDongKeyString(text2, text3);
				int userInputMoney = DBQuery.GetUserInputMoney(dbMgr, dbroleInfo.UserID, dbroleInfo.ZoneID, text2, text3);
				int num2 = Global.TransMoneyToYuanBao(userInputMoney);
				int num3 = 0;
				string text4 = "";
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, 31, huoDongKeyString, out num3, out text4);
				DateTime now = DateTime.Now;
				dateTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
				string startTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
				addDaysDataTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
				text3 = addDaysDataTime.ToString("yyyy-MM-dd HH:mm:ss");
				userInputMoney = DBQuery.GetUserInputMoney(dbMgr, dbroleInfo.UserID, dbroleInfo.ZoneID, startTime, text3);
				int num4 = Global.TransMoneyToYuanBao(userInputMoney);
				data2 = string.Format("{0}:{1}:{2}", num2, num3, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryXingYunChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				string text2 = array[3].Replace("$", ":");
				string text3 = array[4].Replace("$", ":");
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}::", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int userInputMoney = DBQuery.GetUserInputMoney(dbMgr, dbroleInfo.UserID, dbroleInfo.ZoneID, text2, text3);
				int num4 = Global.TransMoneyToYuanBao(userInputMoney);
				int num5 = num4 / num3;
				int num6 = 0;
				string text4 = "";
				string huoDongKeyString = Global.GetHuoDongKeyString(text2, text3);
				int activitytype = 0;
				if (num2 == 1)
				{
					activitytype = 31;
				}
				else if (num2 == 2)
				{
					activitytype = 32;
				}
				DBQuery.GetAwardHistoryForUser(dbMgr, dbroleInfo.UserID, activitytype, huoDongKeyString, out num6, out text4);
				data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num5,
					num6,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteXingYunChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				string fromDate = array[3].Replace("$", ":");
				string toDate = array[4].Replace("$", ":");
				int activitytype = 0;
				if (num2 == 1)
				{
					activitytype = 31;
				}
				else if (num2 == 2)
				{
					activitytype = 32;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbuserInfo)
				{
					if (num3 == 0)
					{
						int num4 = DBWriter.AddHongDongAwardRecordForUser(dbMgr, dbuserInfo.UserID, activitytype, huoDongKeyString, (long)(num3 + 1), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num4 < 0)
						{
							data2 = string.Format("{0}:{1}", -1006, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num4 = DBWriter.UpdateHongDongAwardRecordForUser(dbMgr, dbuserInfo.UserID, activitytype, huoDongKeyString, (long)(num3 + 1), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num4 < 0)
						{
							data2 = string.Format("{0}:{1}", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}", 1, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessBHMatchLoadSupportFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				int minSeasonID = Convert.ToInt32(array[1]);
				int minRound = Convert.ToInt32(array[2]);
				List<BHMatchSupportData> instance = DBQuery.LoadBHMatchSupportFlagData(dbMgr, rid, minSeasonID, minRound);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<BHMatchSupportData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessBHMatchUpdateSupportFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			BHMatchSupportData bhmatchSupportData = null;
			try
			{
				bhmatchSupportData = DataHelper.BytesToObject<BHMatchSupportData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string data2;
				if (!DBWriter.UpdateBHMatchSupportFlagData(dbMgr, bhmatchSupportData))
				{
					data2 = string.Format("{0}:{1}", -1008, bhmatchSupportData.rid);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", 1, bhmatchSupportData.rid);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryYueDuChouJiangHistoryCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				List<YueDuChouJiangData> instance = null;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<YueDuChouJiangData>>(instance, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				instance = DBQuery.QueryYueDuChouJiangHistoryDataList(dbMgr, Convert.ToInt32(array[1]));
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<YueDuChouJiangData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExcuteAddYueDuChouJiangInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string[] array2 = array[0].Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Split(new char[]
					{
						'_'
					});
					if (array3.Length >= 8)
					{
						int num = Convert.ToInt32(array3[0]);
						string roleName = array3[1];
						int zoneID = Convert.ToInt32(array3[2]);
						int gaingoodsid = Convert.ToInt32(array3[3]);
						int gaingoodsnum = Convert.ToInt32(array3[4]);
						int gaingold = Convert.ToInt32(array3[5]);
						int gainyinliang = Convert.ToInt32(array3[6]);
						int gainexp = Convert.ToInt32(array3[7]);
						int num2 = DBWriter.AddNewYueDuChouJiangHistory(dbMgr, num, roleName, zoneID, gaingoodsid, gaingoodsnum, gaingold, gainyinliang, gainexp);
						if (num2 < 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("添加新的月度抽奖记录失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						}
					}
				}
				string data2 = string.Format("{0}:{1}", 1, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteChangeOccupationCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateRoleOccupation(dbMgr, num, num2))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色经验和级别失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					string text2 = "";
					lock (dbroleInfo)
					{
						dbroleInfo.Occupation = num2;
						text2 = dbroleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					if (text2 != "")
					{
						DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
						if (null != dbuserInfo)
						{
							lock (dbuserInfo)
							{
								for (int i = 0; i < dbuserInfo.ListRoleOccups.Count; i++)
								{
									if (dbuserInfo.ListRoleIDs[i] == num)
									{
										dbuserInfo.ListRoleOccups[i] = num2;
									}
								}
							}
						}
					}
					data2 = string.Format("{0}:{1}", num, num2);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetUsingGoodsDataListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				RoleData4Selector roleData4Selector = SingletonTemplate<RoleManager>.Instance().GetRoleData4Selector(roleID, false);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData4Selector>(roleData4Selector, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryBloodCastleEnterCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleid = Convert.ToInt32(array[0]);
				int nDate = Convert.ToInt32(array[1]);
				int activityid = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleid);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, roleid, nDate, activityid);
				data2 = string.Format("{0}:{1}", 1, bloodCastleEnterCount);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateBloodCastleEnterCountCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nRoleID = Convert.ToInt32(array[0]);
				int nDate = Convert.ToInt32(array[1]);
				int nType = Convert.ToInt32(array[2]);
				int nCount = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = DBWriter.UpdateBloodCastleEnterCount(dbMgr, nRoleID, nDate, nType, nCount, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				data2 = string.Format("{0}:{1}", 1, flag);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryFuBenHisInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int fuBenID = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				FuBenHistData fuBenHistData = FuBenHistManager.FindFuBenHistDataByID(fuBenID);
				if (fuBenHistData != null)
				{
					data2 = string.Format("{0}:{1}:{2}", 1, fuBenHistData.RoleName, fuBenHistData.UsedSecs);
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}", -1, "", 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessCompleteFlashSceneCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = DBWriter.UpdateRoleInfoForFlashPlayerFlag(dbMgr, num, 0);
				data2 = string.Format("{0}:{1}", num, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessFinishFreshPlayerStatusCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int index = -1;
				for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
				{
					if (dbuserInfo.ListRoleIDs[i] == dbroleInfo.RoleID)
					{
						index = i;
						break;
					}
				}
				dbroleInfo.CombatForce = 0;
				DBWriter.UpdateRoleCombatForce(dbMgr, num, 0);
				dbroleInfo.IsFlashPlayer = 0;
				DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.Experience = 0L;
				dbroleInfo.MainTaskID = 0;
				dbroleInfo.MainQuickBarKeys = "";
				DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.Level = 1;
				dbuserInfo.ListRoleLevels[index] = 1;
				if (DBWriter.UpdateRoleInfoForFlashPlayerFlag(dbMgr, num, 0))
				{
					data2 = string.Format("{0}:{1}", num, 1);
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessCleanDataWhenFreshPlayerLogOutCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int index = -1;
				for (int i = 0; i < dbuserInfo.ListRoleIDs.Count; i++)
				{
					if (dbuserInfo.ListRoleIDs[i] == dbroleInfo.RoleID)
					{
						index = i;
						break;
					}
				}
				DBWriter.UpdateRoleExpForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.Experience = 0L;
				dbroleInfo.MainTaskID = 0;
				dbroleInfo.MainQuickBarKeys = "";
				DBWriter.UpdateRoleLevForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.Level = 1;
				dbuserInfo.ListRoleLevels[index] = 1;
				DBWriter.UpdateRoleGoodsForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.GoodsDataList = new List<GoodsData>();
				DBWriter.UpdateRoleTasksForFlashPlayerWhenLogOut(dbMgr, num);
				dbroleInfo.DoingTaskList = new List<TaskData>();
				dbroleInfo.OldTasks = new List<OldTaskData>();
				data2 = string.Format("{0}:{1}", num, 1);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessChangeTaskStarLevelCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int starLevel = Convert.ToInt32(array[2]);
				string data2 = "";
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				if (DBWriter.UpdateRoleTasksStarLevel(dbMgr, num, num2, starLevel))
				{
					for (int i = 0; i < dbroleInfo.DoingTaskList.Count; i++)
					{
						if (dbroleInfo.DoingTaskList[i].DbID == num2)
						{
							dbroleInfo.DoingTaskList[i].StarLevel = starLevel;
							data2 = string.Format("{0}:{1}", num, 1);
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					data2 = string.Format("{0}:{1}", num, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessChangeLifeCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string data2 = "";
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}", "", -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.UpdateRoleChangeLifeInfo(dbMgr, num, num2))
				{
					string text2 = "";
					lock (dbroleInfo)
					{
						dbroleInfo.ChangeLifeCount = num2;
						text2 = dbroleInfo.UserID;
					}
					Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
					data2 = string.Format("{0}:{1}", num, 1);
					dbroleInfo.ChangeLifeCount = num2;
					if (text2 != "")
					{
						DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(text2);
						if (null != dbuserInfo)
						{
							lock (dbuserInfo)
							{
								for (int i = 0; i < dbuserInfo.ListRoleChangeLifeCount.Count; i++)
								{
									if (dbuserInfo.ListRoleIDs[i] == num)
									{
										dbuserInfo.ListRoleChangeLifeCount[i] = num2;
									}
								}
							}
						}
					}
				}
				else
				{
					data2 = string.Format("{0}:{1}", num, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAdmiredPlayerCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int nDate = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:{2}", num, -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo2 = dbMgr.GetDBRoleInfo(ref num2);
				if (null == dbroleInfo2)
				{
					data2 = string.Format("{0}:{1}:{2}", num, -1, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBQuery.QueryPlayerAdmiredAnother(dbMgr, num, num2, nDate);
				if (num3 == num2)
				{
					data2 = string.Format("{0}:{1}:{2}", num, -2, 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = dbroleInfo2.AdmiredCount + 1;
				if (DBWriter.UpdateRoleAdmiredInfo1(dbMgr, num2, num4) && DBWriter.UpdateRoleAdmiredInfo2(dbMgr, num, num2, nDate))
				{
					dbroleInfo2.AdmiredCount = num4;
					data2 = string.Format("{0}:{1}:{2}", num, 1, dbroleInfo2.AdmiredCount);
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}", num, -1, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryRoleMiniInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RoleMiniInfo instance = null;
			try
			{
				long num = DataHelper.BytesToObject<long>(data, 0, count);
				if (num > 0L)
				{
					instance = CacheManager.GetRoleMiniInfo(num);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleMiniInfo>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleMiniInfo>(instance, pool, 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleSomeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int combatForce = Convert.ToInt32(array[1]);
				int lev = Convert.ToInt32(array[2]);
				int changeCount = Convert.ToInt32(array[3]);
				int yinLiang = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					dbroleInfo.CombatForce = combatForce;
				}
				DBWriter.UpdateRoleYinLiang(dbMgr, num, yinLiang);
				DBWriter.UpdateRoleCombatForce(dbMgr, num, combatForce);
				DBWriter.UpdateRoleLevel(dbMgr, num, lev);
				DBWriter.UpdateRoleChangeLifeInfo(dbMgr, num, changeCount);
				string data2 = string.Format("{0}:{1}", num, 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int activityid = Convert.ToInt32(array[0]);
				bool flag = true;
				List<int> dayActivityTotlePoint = DBQuery.GetDayActivityTotlePoint(dbMgr, activityid);
				DBRoleInfo dbroleInfo = null;
				if (dayActivityTotlePoint != null && dayActivityTotlePoint.Count == 2)
				{
					int num = dayActivityTotlePoint[0];
					for (dbroleInfo = dbMgr.GetDBRoleInfo(ref num); dbroleInfo == null; dbroleInfo = dbMgr.GetDBRoleInfo(ref num))
					{
						DBWriter.DeleteRoleDayActivityInfo(dbMgr, dayActivityTotlePoint[0], activityid);
						dayActivityTotlePoint = DBQuery.GetDayActivityTotlePoint(dbMgr, activityid);
						if (dayActivityTotlePoint == null || dayActivityTotlePoint.Count != 2)
						{
							flag = false;
							break;
						}
					}
				}
				string data2;
				if (flag && dbroleInfo != null)
				{
					string arg = Global.FormatRoleName(dbroleInfo);
					data2 = string.Format("{0}:{1}", dayActivityTotlePoint[1], arg);
				}
				else
				{
					data2 = string.Format("{0}:{1}", -1, null);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryRoleDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				string data2 = "";
				int num3 = -1;
				if (num2 < 0 || num2 > 5)
				{
					data2 = string.Format("{0}:{1}:{2}", num3, -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 == 0)
				{
					int roleDayActivityPoint = DBQuery.GetRoleDayActivityPoint(dbMgr, num, 1);
					int roleDayActivityPoint2 = DBQuery.GetRoleDayActivityPoint(dbMgr, num, 2);
					int roleDayActivityPoint3 = DBQuery.GetRoleDayActivityPoint(dbMgr, num, 3);
					int roleDayActivityPoint4 = DBQuery.GetRoleDayActivityPoint(dbMgr, num, 4);
					int roleDayActivityPoint5 = DBQuery.GetRoleDayActivityPoint(dbMgr, num, 5);
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						roleDayActivityPoint,
						roleDayActivityPoint2,
						roleDayActivityPoint3,
						roleDayActivityPoint4,
						roleDayActivityPoint5
					});
				}
				else
				{
					num3 = DBQuery.GetRoleDayActivityPoint(dbMgr, num, num2);
					if (num3 == 1)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							num3,
							-1,
							-1,
							-1,
							-1
						});
					}
					else if (num3 == 2)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							num3,
							-1,
							-1,
							-1
						});
					}
					else if (num3 == 3)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							num3,
							-1,
							-1
						});
					}
					else if (num3 == 4)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							-1,
							num3,
							-1
						});
					}
					else if (num3 == 5)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-1,
							-1,
							-1,
							-1,
							num3
						});
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleDayActivityPoinCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 8)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int nDate = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				int num4 = Convert.ToInt32(array[4]);
				int num5 = Convert.ToInt32(array[5]);
				int num6 = Convert.ToInt32(array[6]);
				long nValue = Convert.ToInt64(array[7]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2 = "";
				if (num2 == 0)
				{
					int bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, 1);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, 1, bloodCastleEnterCount, (long)num3);
					bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, 2);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, 2, bloodCastleEnterCount, (long)num4);
					bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, 3);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, 3, bloodCastleEnterCount, (long)num5);
					bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, 4);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, 4, bloodCastleEnterCount, (long)num6);
					bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, 5);
					DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, 5, bloodCastleEnterCount, nValue);
				}
				else
				{
					int bloodCastleEnterCount = DBQuery.GetBloodCastleEnterCount(dbMgr, num, nDate, num2);
					if (num2 == 1)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, num2, bloodCastleEnterCount + 1, (long)num3);
					}
					else if (num2 == 2)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, num2, bloodCastleEnterCount + 1, (long)num4);
					}
					else if (num2 == 3)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, num2, bloodCastleEnterCount + 1, (long)num5);
					}
					else if (num2 == 4)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, num2, bloodCastleEnterCount + 1, (long)num6);
					}
					else if (num2 == 5)
					{
						DBWriter.UpdateRoleDayActivityPoint(dbMgr, num, nDate, num2, bloodCastleEnterCount + 1, nValue);
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryEveryDayOnLineAwardGiftInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				List<int> list = DBQuery.QueryPlayerEveryDayOnLineAwardGiftInfo(dbMgr, num);
				string data2;
				if (list != null)
				{
					data2 = string.Format("{0}:{1}:{2}", 1, list[0], list[1]);
				}
				else
				{
					data2 = string.Format("{0}:{1}:{2}", -1, -1, -1);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSetAutoAssignPropertyPointCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.SetRoleAutoAssignPropertyPoint(dbMgr, num, num2);
				if (num3 == 1)
				{
					dbroleInfo.AutoAssignPropertyPoint = num2;
				}
				string data2 = string.Format("{0}:{1}", num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdatePushMessageInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.SetUserPushMessageID(dbMgr, dbroleInfo.UserID, text2);
				if (num2 == 1)
				{
					dbroleInfo.PushMsgID = text2;
				}
				string data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryPushMsgUerListCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nCondition = Convert.ToInt32(array[0]);
				List<PushMessageData> instance = new List<PushMessageData>();
				instance = DBQuery.QueryPushMsgUerList(dbMgr, nCondition);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<PushMessageData>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddWingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int wingID = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DateTime now = DateTime.Now;
				string addtime = now.ToString("yyyy-MM-dd HH:mm:ss");
				long addDateTime = now.Ticks / 10000L;
				WingData instance = null;
				int num2 = DBWriter.NewWing(dbMgr, num, wingID, 0, addtime, dbroleInfo.RoleName, dbroleInfo.Occupation);
				if (num2 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("添加一个新的翅膀失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.MyWingData = new WingData
						{
							DbID = num2,
							WingID = wingID,
							ForgeLevel = 0,
							AddDateTime = addDateTime,
							JinJieFailedNum = 0,
							StarExp = 0,
							ZhuLingNum = 0,
							ZhuHunNum = 0
						};
						instance = dbroleInfo.MyWingData;
					}
					WingPaiHangManager.getInstance().createWingData(num);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<WingData>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessModWingCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 9)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int id = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateWing(dbMgr, id, array, 2);
				if (num2 < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新时翅膀失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						if (null != dbroleInfo.MyWingData)
						{
							dbroleInfo.MyWingData.Using = DataHelper.ConvertToInt32(array[2], dbroleInfo.MyWingData.Using);
							dbroleInfo.MyWingData.WingID = DataHelper.ConvertToInt32(array[3], dbroleInfo.MyWingData.WingID);
							dbroleInfo.MyWingData.ForgeLevel = DataHelper.ConvertToInt32(array[4], dbroleInfo.MyWingData.ForgeLevel);
							dbroleInfo.MyWingData.JinJieFailedNum = DataHelper.ConvertToInt32(array[5], dbroleInfo.MyWingData.JinJieFailedNum);
							dbroleInfo.MyWingData.StarExp = DataHelper.ConvertToInt32(array[6], dbroleInfo.MyWingData.StarExp);
							dbroleInfo.MyWingData.ZhuLingNum = DataHelper.ConvertToInt32(array[7], dbroleInfo.MyWingData.ZhuLingNum);
							dbroleInfo.MyWingData.ZhuHunNum = DataHelper.ConvertToInt32(array[8], dbroleInfo.MyWingData.ZhuHunNum);
						}
					}
					WingRankingInfo wingData = WingPaiHangManager.getInstance().getWingData(num);
					if (null != wingData)
					{
						if (wingData.nWingID != dbroleInfo.MyWingData.WingID || wingData.nStarNum != dbroleInfo.MyWingData.ForgeLevel)
						{
							wingData.nWingID = dbroleInfo.MyWingData.WingID;
							wingData.nStarNum = dbroleInfo.MyWingData.ForgeLevel;
							WingPaiHangManager.getInstance().ModifyWingPaihangData(wingData, false);
						}
					}
				}
				string data2 = string.Format("{0}:{1}", num, num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessReferPictureJudgeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = DBWriter.UpdateRoleReferPictureJudgeInfo(dbMgr, num, num2, num3);
				if (num4 <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						int num5 = 0;
						if (null != dbroleInfo.PictureJudgeReferInfo)
						{
							if (!dbroleInfo.PictureJudgeReferInfo.TryGetValue(num2, out num5))
							{
								dbroleInfo.PictureJudgeReferInfo.Add(num2, num3);
							}
							else
							{
								dbroleInfo.PictureJudgeReferInfo[num2] = num3;
							}
						}
						else
						{
							dbroleInfo.PictureJudgeReferInfo = new Dictionary<int, int>();
							dbroleInfo.PictureJudgeReferInfo.Add(num2, num3);
						}
					}
				}
				string data2 = string.Format("{0}", num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryMoJingExchangeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int nDayID = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, int> instance = DBQuery.QueryMoJingExchangeDict(dbMgr, num, nDayID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, int>>(instance, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateMoJingExchangeInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int nExchangeid = Convert.ToInt32(array[1]);
				int nDayID = Convert.ToInt32(array[2]);
				int nNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBWriter.UpdateMoJingExchangeDict(dbMgr, num, nExchangeid, nDayID, nNum);
				if (num2 <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				string data2 = string.Format("{0}", num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateGoodsCmd2(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			UpdateGoodsArgs updateGoodsArgs = null;
			try
			{
				updateGoodsArgs = DataHelper.BytesToObject<UpdateGoodsArgs>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<int>(nID, -1);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				if (null == updateGoodsArgs)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int roleID = updateGoodsArgs.RoleID;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbroleInfo, updateGoodsArgs.DbID);
				if (null == goodsDataByDbID)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, roleID), null, true);
					client.sendCmd<int>(nID, -1);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int num = DBWriter.UpdateGoods2(dbMgr, roleID, goodsDataByDbID, updateGoodsArgs);
				if (num <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新物品属性信息失败，CMD={0}, RoleID={1}, dbid={2}", (TCPGameServerCmds)nID, roleID, updateGoodsArgs.DbID), null, true);
				}
				client.sendCmd<int>(nID, num);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<int>(nID, -1);
			return TCPProcessCmdResults.RESULT_OK;
		}

		private static TCPProcessCmdResults ProcessUpdateStarConstellationCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num4 = DBWriter.UpdateRoleStarConstellationInfo(dbMgr, num, num2, num3);
				if (num4 <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("更新图鉴提交信息失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						int num5 = 0;
						if (null != dbroleInfo.StarConstellationInfo)
						{
							if (!dbroleInfo.StarConstellationInfo.TryGetValue(num2, out num5))
							{
								dbroleInfo.StarConstellationInfo.Add(num2, num3);
							}
							else
							{
								dbroleInfo.StarConstellationInfo[num2] = num3;
							}
						}
						else
						{
							dbroleInfo.StarConstellationInfo = new Dictionary<int, int>();
							dbroleInfo.StarConstellationInfo.Add(num2, num3);
						}
					}
				}
				string data2 = string.Format("{0}", num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryVipLevelAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = DBQuery.QueryVipLevelAwardFlagInfo(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID);
				string data2 = string.Format("{0}", num2);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateVipLevelAwardFlagCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string data2;
				if (!DBWriter.UpdateVipLevelAwardFlagInfo(dbMgr, dbroleInfo.UserID, num2, dbroleInfo.ZoneID))
				{
					data2 = string.Format("{0}:{1}", num, -1);
					LogManager.WriteLog(LogTypes.Error, string.Format("更新角色VIP等级奖励标记事时失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
				}
				else
				{
					lock (dbroleInfo)
					{
						dbroleInfo.VipAwardFlag = num2;
					}
					data2 = string.Format("{0}:{1}", num, 0);
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateOnePieceTreasureLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string keyTime = array[0];
				int logType = Convert.ToInt32(array[1]);
				int addValue = Convert.ToInt32(array[2]);
				DBWriter.UpdateOnePieceTreasureLog(dbMgr, keyTime, logType, addValue);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13400);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddItemLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 10)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.insertItemLog(dbMgr, array);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateRoleKuaFuDayLogCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			RoleKuaFuDayLogData roleKuaFuDayLogData = null;
			try
			{
				roleKuaFuDayLogData = DataHelper.BytesToObject<RoleKuaFuDayLogData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				if (null == roleKuaFuDayLogData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("解析数据结果RoleKuaFuDayLogData失败, CMD={0}, Recv={1}", (TCPGameServerCmds)nID, data.Length), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBWriter.UpdateRoleKuaFuDayLog(dbMgr, roleKuaFuDayLogData);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddRoleStoreYinliang(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				long num2 = Convert.ToInt64(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				long num4 = 0L;
				lock (dbroleInfo)
				{
					if (num2 < 0L && num3 == 0 && dbroleInfo.store_yinliang < Math.Abs(num2))
					{
						flag = true;
					}
					else
					{
						dbroleInfo.store_yinliang += num2;
						num4 = dbroleInfo.store_yinliang;
					}
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0L)
				{
					if (!DBWriter.UpdateRoleStoreYinLiang(dbMgr, num, num4))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色仓库金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", -2, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				data2 = string.Format("{0}:{1}", num, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddRoleStoreMoney(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				long num2 = Convert.ToInt64(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				bool flag = false;
				long num4 = 0L;
				lock (dbroleInfo)
				{
					if (num2 < 0L && num3 == 0 && dbroleInfo.store_money < Math.Abs(num2))
					{
						flag = true;
					}
					else
					{
						dbroleInfo.store_money += num2;
						num4 = dbroleInfo.store_money;
					}
				}
				string data2;
				if (flag)
				{
					data2 = string.Format("{0}:{1}", -1, -1);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 != 0L)
				{
					if (!DBWriter.UpdateRoleStoreMoney(dbMgr, num, num4))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("更新角色仓库绑定金币失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						data2 = string.Format("{0}:{1}", -2, -2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				Global.WriteRoleInfoLog(dbMgr, dbroleInfo);
				data2 = string.Format("{0}:{1}", num, num4);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateLingYu(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int level = Convert.ToInt32(array[2]);
				int suit = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num3 = DBWriter.UpdateLingYu(dbMgr, num, num2, level, suit);
				if (num3 >= 0)
				{
					lock (dbroleInfo)
					{
						LingYuData lingYuData = new LingYuData();
						lingYuData.Type = num2;
						lingYuData.Level = level;
						lingYuData.Suit = suit;
						dbroleInfo.LingYuDict[num2] = lingYuData;
					}
					string data2 = string.Format("{0}:{1}", num, num3);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				LogManager.WriteLog(LogTypes.Error, string.Format("更新翎羽失败，CMD={0}, RoleID={1}, type={2}", (TCPGameServerCmds)nID, num, num2), null, true);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddFuMoMoneyGiveMail(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				int num2 = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				string content = array[4];
				int sendjob = Convert.ToInt32(array[5]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (DBWriter.InsertFoMoMailData(dbMgr, num, text2, sendjob, num2, num3, content, today))
				{
					if (FuMoMailManager.getInstance().InsertFuMoMailCached(dbMgr, num, text2, sendjob, num2, num3, content, today))
					{
						int num4 = FuMoMailManager.getInstance().MaxLimitContorl(num2);
						if (num4 > 0)
						{
							if (-1 == FuMoMailManager.getInstance().DelFuMoMailFromLimitContorl(dbMgr, num2, num4))
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
								return TCPProcessCmdResults.RESULT_DATA;
							}
						}
						text = string.Format("{0}:{1}", num, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuMoMoneyMapAcceptNum(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int nDate = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				text = string.Format("{0}:{1}", num, FuMoMailManager.getInstance().GetFuMoTempDataAcceptFromCached(nDate, num));
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddFuMoMoneyGiveMailTemp(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string text2 = array[1];
				int num2 = Convert.ToInt32(array[2]);
				int accept = Convert.ToInt32(array[3]);
				int give = Convert.ToInt32(array[4]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.InsertFoMoMailDataTemp(dbMgr, num, text2, num2, accept, give))
				{
					if (FuMoMailManager.getInstance().InsertAcceptMapCached(num, text2, num2, accept, give))
					{
						text = string.Format("{0}:{1}:{2}", num2, num, text2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					LogManager.WriteLog(LogTypes.Error, string.Format("插入缓存失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuMoMoneyMailList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref rid);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				Dictionary<int, List<FuMoMailData>> fuMoMailItemDataListFromCached = FuMoMailManager.getInstance().GetFuMoMailItemDataListFromCached(rid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, List<FuMoMailData>>>(fuMoMailItemDataListFromCached, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetFuMoMoneyMailMapList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref rid);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(array[1]);
				Dictionary<int, FuMoMailTemp> fuMoTempDataListFromCached = FuMoMailManager.getInstance().GetFuMoTempDataListFromCached(nDate, rid);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, FuMoMailTemp>>(fuMoTempDataListFromCached, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateFuMoMoneyMailMap(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				string recid_list = array[3];
				if (DBWriter.UpdateRoleStoreFuMoMoneyGiveNum(dbMgr, num, num2, nDate, recid_list))
				{
					if (FuMoMailManager.getInstance().UpdateGiveAndListCached(num, num2, nDate, recid_list))
					{
						text = string.Format("{0}:{1}", num, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateFuMoAcceptMap(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int nDate = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				if (DBWriter.UpdateRoleStoreFuMoMoneyAcceptNum(dbMgr, num, nDate, num2) && FuMoMailManager.getInstance().UpdateAcceptCached(num, num2, nDate))
				{
					text = string.Format("{0}:{1}", num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteFuMoMail(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = Convert.ToInt32(array[1]);
				if (DBWriter.DeleteMailFuMoByMailID(dbMgr, num2) && FuMoMailManager.getInstance().UpdataRemoveMailListCached(num2, num))
				{
					text = string.Format("{0}:{1}", num, num2);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessDeleteFuMoMailList(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = array[1];
				string[] mailidList = text2.Split(new char[]
				{
					'_'
				});
				string text3 = FuMoMailManager.getInstance().MakeDelListSQL(mailidList);
				if (DBWriter.DeleteMailFuMoByMailIDList(dbMgr, num, text3))
				{
					if (FuMoMailManager.getInstance().UpdataRemoveMailListCached(mailidList, num))
					{
						text = string.Format("{0}:{1}", num, text3);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateFuMoMailReadState(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = Convert.ToInt32(array[1]);
				string today = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
				if (DBWriter.UpdateIsReadFoMoMailData(dbMgr, num2, today))
				{
					if (FuMoMailManager.getInstance().UpdataReadStateCached(num, num2, today))
					{
						text = string.Format("{0}:{1}", num, num2);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessFuMoMailIndexCount(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int rid = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref rid);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<int>(DBQuery.GetMailMaxConutFromTable(dbMgr, rid), pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryRoleMoneyInfo(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.dbUserMgr.FindDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的账号不存在，CMD={0}, dbRoleInfo.UserID={1}", (TCPGameServerCmds)nID, dbroleInfo.UserID), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num2 = 0;
				foreach (int num3 in dbuserInfo.ListRoleIDs)
				{
					string roleParamByName = DBQuery.GetRoleParamByName(dbMgr, num3, "TotalCostMoney");
					string[] array2 = roleParamByName.Split(new char[]
					{
						','
					});
					if (array2 == null || array2.Length != 2)
					{
						int userUsedMoney = DBQuery.GetUserUsedMoney(dbMgr, num3, "2014-01-01 00:00:00", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						string value = string.Format("{0},{1}", 1, userUsedMoney);
						DBWriter.UpdateRoleParams(dbMgr, num3, "TotalCostMoney", value, null);
						num2 += userUsedMoney;
					}
					else
					{
						num2 += Convert.ToInt32(array2[1]);
					}
				}
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					dbroleInfo.UserID,
					num,
					dbroleInfo.RoleName,
					dbuserInfo.RealMoney,
					num2,
					dbuserInfo.Money,
					dbroleInfo.TotalOnlineSecs,
					dbroleInfo.ChangeLifeCount * 100 + dbroleInfo.Level,
					DataHelper.ConvertToTicks(dbroleInfo.RegTime)
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAutoCompletionTaskByTaskID(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				List<int> list = DataHelper.BytesToObject<List<int>>(data, 0, count);
				if (list == null || list.Count < 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("任务列表异常, CMD={0}", (TCPGameServerCmds)nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = list[0];
				int mainTaskID = list[list.Count - 1];
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("发起请求的角色不存在，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				lock (dbroleInfo)
				{
					if (!DBWriter.WirterAutoCompletionTaskByTaskID(dbMgr, num, list))
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("插入历史任务标记失败，CMD={0}, RoleID={1}", (TCPGameServerCmds)nID, num), null, true);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
						return TCPProcessCmdResults.RESULT_FAILED;
					}
					DBWriter.UpdateRoleMainTaskID(dbMgr, num, mainTaskID);
					dbroleInfo.MainTaskID = mainTaskID;
					if (null == dbroleInfo.OldTasks)
					{
						dbroleInfo.OldTasks = new List<OldTaskData>();
					}
					for (int i = 1; i < list.Count; i++)
					{
						dbroleInfo.OldTasks.Add(new OldTaskData
						{
							TaskID = list[i],
							DoCount = 1
						});
					}
				}
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", 0), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}", -1), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateMarriageDataCmd(DBManager dbMgr, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MarriageData marriageData = null;
			int nRoleID = -1;
			bool flag = false;
			try
			{
				nRoleID = BitConverter.ToInt32(data, 0);
				marriageData = DataHelper.BytesToObject<MarriageData>(data, 4, count - 4);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			try
			{
				bool flag2 = false;
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						if (dbroleInfo.MyMarriageData.ChangTime != marriageData.ChangTime && marriageData.byGoodwilllevel != 0)
						{
							flag2 = true;
						}
						dbroleInfo.MyMarriageData.nSpouseID = marriageData.nSpouseID;
						dbroleInfo.MyMarriageData.byMarrytype = marriageData.byMarrytype;
						dbroleInfo.MyMarriageData.nRingID = marriageData.nRingID;
						dbroleInfo.MyMarriageData.nGoodwillexp = marriageData.nGoodwillexp;
						dbroleInfo.MyMarriageData.byGoodwillstar = marriageData.byGoodwillstar;
						dbroleInfo.MyMarriageData.byGoodwilllevel = marriageData.byGoodwilllevel;
						dbroleInfo.MyMarriageData.nGivenrose = marriageData.nGivenrose;
						dbroleInfo.MyMarriageData.strLovemessage = marriageData.strLovemessage;
						dbroleInfo.MyMarriageData.byAutoReject = marriageData.byAutoReject;
						dbroleInfo.MyMarriageData.ChangTime = marriageData.ChangTime;
					}
				}
				else if (marriageData.byGoodwilllevel != 0)
				{
					flag2 = true;
				}
				flag = DBWriter.UpdateMarriageData(dbMgr, nRoleID, marriageData.nSpouseID, marriageData.byMarrytype, marriageData.nRingID, marriageData.nGoodwillexp, marriageData.byGoodwillstar, marriageData.byGoodwilllevel, marriageData.nGivenrose, marriageData.strLovemessage, marriageData.byAutoReject, marriageData.ChangTime);
				RingRankingInfo ringData = RingPaiHangManager.getInstance().getRingData(nRoleID);
				if (null != ringData)
				{
					ringData.nRingID = marriageData.nRingID;
					ringData.byGoodwillstar = (int)marriageData.byGoodwillstar;
					ringData.byGoodwilllevel = (int)marriageData.byGoodwilllevel;
					ringData.strAddTime = marriageData.ChangTime;
				}
				if (flag && flag2)
				{
					RingPaiHangManager.getInstance().createRingData(nRoleID, ringData);
				}
				client.sendCmd<bool>(nID, flag);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			client.sendCmd<bool>(nID, flag);
			return TCPProcessCmdResults.RESULT_OK;
		}

		private static TCPProcessCmdResults ProcessGetMarriageDataCmd(DBManager dbMgr, TCPOutPacketPool pool, GameServerClient client, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			MarriageData marriageData = null;
			int nRoleID = -1;
			string value = null;
			try
			{
				value = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				nRoleID = Convert.ToInt32(value);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref nRoleID);
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						marriageData = dbroleInfo.MyMarriageData;
					}
				}
				else
				{
					marriageData = DBQuery.GetMarriageData(dbMgr, nRoleID);
				}
				if (null != marriageData)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarriageData>(marriageData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				marriageData = new MarriageData();
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarriageData>(marriageData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessExecuteJieriFanLiCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 5)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string fromDate = array[1].Replace('$', ':');
				string toDate = array[2].Replace('$', ':');
				int num2 = Convert.ToInt32(array[3]);
				int num3 = Convert.ToInt32(array[4]);
				num3 = Math.Max(0, num3);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				string data2;
				if (null == dbroleInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string huoDongKeyString = Global.GetHuoDongKeyString(fromDate, toDate);
				int num4 = 0;
				string text2 = "";
				int awardHistoryForRole = DBQuery.GetAwardHistoryForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, num2, huoDongKeyString, out num4, out text2);
				if (num3 == 0)
				{
					data2 = string.Format("{0}:{1}", num2, num4);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int bitValue = Global.GetBitValue(num3);
				if ((num4 & bitValue) == bitValue)
				{
					data2 = string.Format("{0}:{1}:0", -10005, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBUserInfo dbuserInfo = dbMgr.GetDBUserInfo(dbroleInfo.UserID);
				if (null == dbuserInfo)
				{
					data2 = string.Format("{0}:{1}:0", -1001, num);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				lock (dbroleInfo)
				{
					num4 |= 1 << num3 - 1;
					if (awardHistoryForRole < 0)
					{
						int num5 = DBWriter.AddHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, num2, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num5 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
					else
					{
						int num5 = DBWriter.UpdateHongDongAwardRecordForRole(dbMgr, dbroleInfo.RoleID, dbroleInfo.ZoneID, num2, huoDongKeyString, num4, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
						if (num5 < 0)
						{
							data2 = string.Format("{0}:{1}:0", -1008, num);
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
							return TCPProcessCmdResults.RESULT_DATA;
						}
					}
				}
				data2 = string.Format("{0}:{1}:{2}", num4, num, num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessQueryMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				tcpOutPacket = GameDBManager.MarryPartyDataC.GetPartyList(pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessAddMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 7)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				int partyType = Convert.ToInt32(array[1]);
				long num = Convert.ToInt64(array[2]);
				int husbandRoleID = Convert.ToInt32(array[3]);
				int wifeRoleID = Convert.ToInt32(array[4]);
				string husbandName = Convert.ToString(array[5]);
				string wifeName = Convert.ToString(array[6]);
				MarryPartyData marryPartyData = GameDBManager.MarryPartyDataC.AddParty(roleID, partyType, num, husbandRoleID, wifeRoleID, husbandName, wifeName);
				if (marryPartyData != null)
				{
					DateTime dateTime = new DateTime(num * 10000L);
					string startTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
					DBWriter.AddMarryParty(dbMgr, roleID, partyType, startTime, husbandRoleID, wifeRoleID);
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MarryPartyData>(marryPartyData, pool, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRemoveMarryParty(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				if (GameDBManager.MarryPartyDataC.RemoveParty(num))
				{
					DBWriter.RemoveMarryParty(dbMgr, num);
					string data2 = string.Format("{0}", 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessIncMarryPartyJoin(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int joinerID = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				if (GameDBManager.MarryPartyDataC.IncPartyJoin(num))
				{
					DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref joinerID);
					if (null != dbroleInfo)
					{
						lock (dbroleInfo)
						{
							dbroleInfo.MyMarryPartyJoinList[num] = num2;
						}
					}
					DBWriter.IncMarryPartyJoin(dbMgr, num, joinerID, num2);
					string data2 = string.Format("{0}", 0);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessClearMarryPartyJoin(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						dbroleInfo.MyMarryPartyJoinList.Clear();
					}
				}
				if (num2 > 0)
				{
					DBWriter.ClearMarryPartyJoin(dbMgr, (num2 == 1) ? num : 0);
				}
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessUpdateHolyItemDataCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				sbyte b = Convert.ToSByte(array[1]);
				sbyte b2 = Convert.ToSByte(array[2]);
				sbyte b3 = Convert.ToSByte(array[3]);
				int num2 = Convert.ToInt32(array[4]);
				int num3 = Convert.ToInt32(array[5]);
				if (b2 < 1 || b2 > 6)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("sPart_slot错误, cmd={0} sPart_slot={1}, roleid={2}", (TCPGameServerCmds)nID, b2, num), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null != dbroleInfo)
				{
					lock (dbroleInfo)
					{
						HolyItemData holyItemData = null;
						bool flag2 = dbroleInfo.MyHolyItemDataDic.TryGetValue(b, out holyItemData);
						if (!flag2)
						{
							holyItemData = new HolyItemData();
						}
						holyItemData.m_sType = b;
						HolyItemPartData holyItemPartData = null;
						if (!holyItemData.m_PartArray.TryGetValue(b2, out holyItemPartData))
						{
							holyItemPartData = new HolyItemPartData();
							holyItemData.m_PartArray.Add(b2, holyItemPartData);
						}
						holyItemPartData.m_sSuit = b3;
						holyItemPartData.m_nSlice = num2;
						holyItemPartData.m_nFailCount = num3;
						if (!flag2)
						{
							dbroleInfo.MyHolyItemDataDic.Add(b, holyItemData);
						}
					}
				}
				string data2 = DBWriter.UpdateHolyItemData(dbMgr, num, b, b2, b3, num2, num3) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGmBanCheck(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 2;
				char span = '#';
				if (!CheckHelper.CheckTCPCmdFields2(nID, data, count, out array, length, span))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				string banIDs = array[1];
				string data2 = BanManager.GmBanCheckAdd(dbMgr, roleID, banIDs).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGmBanLog(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 7;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(array[0]);
				string userID = array[1];
				int roleID = Convert.ToInt32(array[2]);
				int banType = Convert.ToInt32(array[3]);
				string banID = array[4];
				int banCount = Convert.ToInt32(array[5]);
				string deviceID = array[6];
				string data2 = BanManager.GmBanLogAdd(dbMgr, zoneID, userID, roleID, banType, banID, banCount, deviceID).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessTenInitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					'#'
				});
				if (array.Length <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				TenManager.initTen(array);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 13113);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSpreadAwardGetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 2;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(array[0]);
				int roleID = Convert.ToInt32(array[1]);
				string data2 = SpreadManager.GetAward(dbMgr, zoneID, roleID).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessSpreadAwardUpdateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 4;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(array[0]);
				int roleID = Convert.ToInt32(array[1]);
				int awardType = Convert.ToInt32(array[2]);
				string award = array[3];
				string data2 = SpreadManager.UpdateAward(dbMgr, zoneID, roleID, awardType, award).ToString();
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessActivateStateGetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 1;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string userID = array[0];
				string data2 = DBQuery.ActivateStateGet(dbMgr, userID) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessActivateStateSetCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string[] array = null;
			try
			{
				int length = 3;
				if (!CheckHelper.CheckTCPCmdFields(nID, data, count, out array, length))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int zoneID = Convert.ToInt32(array[0]);
				string userID = array[1];
				int roleID = Convert.ToInt32(array[2]);
				string data2 = DBWriter.ActivateStateSet(dbMgr, zoneID, userID, roleID) ? "1" : "0";
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessFacebookInitCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					'#'
				});
				if (array.Length <= 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				FacebookManager.initFacebook(array);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "1", nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 21000);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetZoneIdByRid(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			string value = text;
			int roleID = Convert.ToInt32(value);
			try
			{
				DBRoleInfo dbroleInfo = dbMgr.DBRoleMgr.FindDBRoleInfo(ref roleID);
				string data2;
				if (null == dbroleInfo)
				{
					List<string> list = new List<string>();
					list = DBWriter.GetUserZoneID(dbMgr, roleID);
					data2 = string.Format("{0}:{1}", list[0], list[1]);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				data2 = string.Format("{0}:{1}", dbroleInfo.UserID, dbroleInfo.ZoneID);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, 1.ToString(), nID);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRebornYinJiUpdateCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string stampInfo = array[1];
				int usePoint = Convert.ToInt32(array[2]);
				int resetNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.UpdateRebornYinJiInfo(num, stampInfo, resetNum, usePoint))
				{
					if (RebornStampManager.UpdateUserRebornInfo(num, stampInfo, resetNum, usePoint))
					{
						text = string.Format("{0}", num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessGetRebornYinJiInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref roleID);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				RebornStampData userRebornInfoFromCached = RebornStampManager.GetUserRebornInfoFromCached(roleID);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RebornStampData>(userRebornInfoFromCached, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private static TCPProcessCmdResults ProcessRebornYinJiInsertInfoCmd(DBManager dbMgr, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = Convert.ToInt32(array[0]);
				string stampInfo = array[1];
				int usePoint = Convert.ToInt32(array[2]);
				int resetNum = Convert.ToInt32(array[3]);
				DBRoleInfo dbroleInfo = dbMgr.GetDBRoleInfo(ref num);
				if (null == dbroleInfo)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "-1", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (DBWriter.InsertRebornYinJiInfo(num, stampInfo, resetNum, usePoint))
				{
					if (RebornStampManager.InsertUserRebornInfo(num, stampInfo, resetNum, usePoint))
					{
						text = string.Format("{0}", num);
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}
	}
}
