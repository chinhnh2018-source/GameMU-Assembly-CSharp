using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public static class SystemHelpMgr
	{
		public static int ActiveHelpID
		{
			get
			{
				return SystemHelpMgr._ActiveHelpID;
			}
			set
			{
				SystemHelpMgr._ActiveHelpID = value;
			}
		}

		public static int ActiveHintID
		{
			get
			{
				return SystemHelpMgr._ActiveHintID;
			}
			set
			{
				SystemHelpMgr._ActiveHintID = value;
			}
		}

		public static void OnAction(UIObjIDs id, HelpStateEvents e, int param = -1)
		{
			if (id > UIObjIDs.Max)
			{
				return;
			}
			int activeHelpID;
			if (SystemHelpMgr.ActiveHintID > 0)
			{
				if (id == UIObjIDs.HuoDong)
				{
					switch (SystemHelpMgr.ActiveHelpID)
					{
					case 1065:
					case 1066:
					case 1068:
					case 1069:
					case 1070:
					case 1071:
					case 1075:
					case 1076:
						SystemHelpPart.ShowHintText(false, null, null, Dircetions.DR_UP);
						SystemHelpMgr.ShowHint(-1, 0);
						break;
					}
				}
				else if (id == UIObjIDs.FuLi)
				{
					switch (SystemHelpMgr.ActiveHelpID)
					{
					case 1072:
					case 1073:
					case 1074:
						SystemHelpPart.ShowHintText(false, null, null, Dircetions.DR_UP);
						SystemHelpMgr.ShowHint(-1, 0);
						break;
					}
				}
				else if (id == UIObjIDs.GuaJi)
				{
					activeHelpID = SystemHelpMgr.ActiveHelpID;
					if (activeHelpID == 1064)
					{
						SystemHelpPart.ShowHintText(false, null, null, Dircetions.DR_UP);
						SystemHelpMgr.ShowHint(-1, 0);
					}
				}
			}
			if (id == UIObjIDs.Exception)
			{
				SystemHelpMgr.ActiveHelpID = -1;
				SystemHelpMgr.ActiveHelpState = -1;
				SystemHelpPart.HideMask();
			}
			if (SystemHelpMgr.ActiveHelpID <= 0)
			{
				return;
			}
			if (id == UIObjIDs.LianLuPart && e == HelpStateEvents.Inactived)
			{
				if ((SystemHelpMgr.ActiveHelpID >= 16 && SystemHelpMgr.ActiveHelpID <= 17) || (SystemHelpMgr.ActiveHelpID >= 30 && SystemHelpMgr.ActiveHelpID <= 32))
				{
					SystemHelpMgr.ActiveHelpID = -1;
					return;
				}
			}
			else if (id == UIObjIDs.HeChengPart && e == HelpStateEvents.Inactived && ((SystemHelpMgr.ActiveHelpID >= 34 && SystemHelpMgr.ActiveHelpID <= 35) || (SystemHelpMgr.ActiveHelpID >= 38 && SystemHelpMgr.ActiveHelpID <= 41) || (SystemHelpMgr.ActiveHelpID >= 48 && SystemHelpMgr.ActiveHelpID <= 49) || (SystemHelpMgr.ActiveHelpID >= 52 && SystemHelpMgr.ActiveHelpID <= 53)))
			{
				SystemHelpMgr.ActiveHelpID = -1;
				return;
			}
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"<color=yellow>",
					SystemHelpMgr.ActiveHelpID,
					" I D = ",
					id,
					"</color>"
				})
			});
			activeHelpID = SystemHelpMgr.ActiveHelpID;
			switch (activeHelpID)
			{
			case 14:
			case 18:
			case 28:
			case 33:
			case 36:
			case 46:
			case 50:
			case 197:
				break;
			case 15:
			case 29:
				if (id == UIObjIDs.GameLianLu)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 16:
			case 31:
				if (id == UIObjIDs.LianLuEquipItem)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 17:
				if (id == UIObjIDs.LianLuQiangHuaSubmit)
				{
					if (SystemHelpMgr.ActiveHelpState++ > 2)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.EndWizard(true);
					}
				}
				else if (id == UIObjIDs.LianLuEquipItem)
				{
				}
				return;
			case 19:
				if (id == UIObjIDs.GameBag)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveGoodsID = 7004;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 20:
				if (id == UIObjIDs.BagPart)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					SystemHelpMgr.ActiveHelpID++;
				}
				else if (id == UIObjIDs.GGoodTips && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 21:
				if (id == UIObjIDs.GGoodTips)
				{
					if (e == HelpStateEvents.Actived)
					{
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					else if (SystemHelpMgr.ActiveHelpID != 1000)
					{
						SystemHelpMgr.EndWizard(true);
					}
				}
				else if (id == UIObjIDs.UseGoodsBtn)
				{
					SystemHelpMgr.ActiveGoodsID = -1;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.ActiveHelpID = 1000;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 22:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 116)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 23:
			case 24:
				if (id == UIObjIDs.ZhuanZhiPart)
				{
					if (e == HelpStateEvents.Actived && SystemHelpMgr.ActiveHelpID == 23)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					else
					{
						SystemHelpMgr.ActiveHelpID = -1;
					}
				}
				else if (id == UIObjIDs.ZhuanZhiSubmit && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.EndWizard(true);
				}
				return;
			default:
				switch (activeHelpID)
				{
				case 701:
				case 711:
					SystemHelpMgr.ActiveHelpState = 0;
					if (id == UIObjIDs.GamePayerRolePart)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					else if (id == UIObjIDs.GamePayerRoleDetailPart)
					{
						SystemHelpMgr.ActiveHelpID += 2;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 702:
				case 712:
					if (id == UIObjIDs.GamePayerRolePart || id == UIObjIDs.HuiShouPart)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 703:
				case 713:
					if (id == UIObjIDs.RecyclePart)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					else if (id == UIObjIDs.HuiShouPart)
					{
						SystemHelpMgr.ActiveHelpID += 3;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 704:
				case 714:
					if (id == UIObjIDs.HuiShouPart)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 705:
				case 715:
					if (id == UIObjIDs.HuiShouDetailPatr)
					{
						SystemHelpMgr.ActiveHelpID += 2;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, SystemHelpMgr.ActiveHelpState);
					}
					return;
				case 706:
				case 716:
					if (id == UIObjIDs.HuiShouDetailPatr)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, SystemHelpMgr.ActiveHelpState);
					}
					return;
				case 707:
				case 717:
					if (id == UIObjIDs.HuiShouDetailPatr && e == HelpStateEvents.Inactived)
					{
						SystemHelpMgr.EndWizard(true);
					}
					return;
				default:
					switch (activeHelpID)
					{
					case 1000:
						if (id == UIObjIDs.LeaderRoleFace && param == 0)
						{
							SystemHelpMgr.EndWizard(true);
						}
						return;
					default:
						switch (activeHelpID)
						{
						case 600:
							break;
						default:
							switch (activeHelpID)
							{
							case 238:
							case 240:
								if (id == UIObjIDs.HuoDongPart && e == HelpStateEvents.Actived)
								{
									SystemHelpMgr.ActiveHelpID++;
									SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
								}
								else if (id == UIObjIDs.RiChangFuBenDetailpartEnter && e == HelpStateEvents.Inactived)
								{
									SystemHelpMgr.EndWizard(true);
								}
								return;
							case 239:
								if (id == UIObjIDs.RiChangFuBenDetailpart && e == HelpStateEvents.Clicked)
								{
									SystemHelpMgr.ActiveHelpID++;
									SystemHelpMgr.RootPart.DoAction(241, 1);
								}
								return;
							case 241:
								if (id == UIObjIDs.RiChangFuBenDetailpart && e == HelpStateEvents.Actived)
								{
									SystemHelpMgr.ActiveHelpID++;
									SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
								}
								return;
							case 242:
								if (id == UIObjIDs.RiChangFuBenDetailpartEnter && e == HelpStateEvents.Clicked)
								{
									SystemHelpMgr.EndWizard(true);
								}
								return;
							default:
								switch (activeHelpID)
								{
								case 400:
									break;
								default:
									switch (activeHelpID)
									{
									case 300:
										break;
									default:
										if (activeHelpID == 230)
										{
											if (id == UIObjIDs.HuoDongPart && e == HelpStateEvents.Actived)
											{
												SystemHelpMgr.EndWizard(true);
											}
											return;
										}
										if (activeHelpID == 250)
										{
											if (id == UIObjIDs.MainIconStartAutoFight && e == HelpStateEvents.Clicked)
											{
												SystemHelpMgr.EndWizard(true);
											}
											return;
										}
										if (activeHelpID != 11000)
										{
											if (SystemHelpMgr.ActiveHelpID < 10000)
											{
												SystemHelpMgr.ActiveHelpID++;
												SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
											}
											else
											{
												SystemHelpMgr.ActiveHelpID = -1;
												SystemHelpMgr.ActiveHelpState = -1;
											}
											return;
										}
										if (id == UIObjIDs.DecorationPlayEnd && param == 5001)
										{
											SystemHelpMgr.EndWizard(true);
										}
										return;
									case 303:
										if (id == UIObjIDs.TuJianPart)
										{
											SystemHelpMgr.EndWizard(true);
										}
										return;
									}
									break;
								case 402:
									if (id == UIObjIDs.XingHunPart)
									{
										SystemHelpMgr.ActiveHelpID++;
										SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
									}
									return;
								case 403:
									if (id == UIObjIDs.XingHunPart_BaiYang)
									{
										SystemHelpMgr.ActiveHelpID++;
										SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
									}
									return;
								case 404:
									if (id == UIObjIDs.XingHunPart_JiHuo)
									{
										SystemHelpMgr.EndWizard(true);
									}
									return;
								}
								break;
							}
							break;
						case 602:
							if (id == UIObjIDs.JingLingPart)
							{
								SystemHelpMgr.ActiveHelpID++;
								SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
							}
							return;
						case 603:
							if (id == UIObjIDs.JingLingPartDetailpart)
							{
								SystemHelpMgr.ActiveHelpID++;
								SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
							}
							return;
						case 604:
							if (id == UIObjIDs.JingLingGiftsPart)
							{
								SystemHelpMgr.ActiveHelpID++;
								SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
							}
							return;
						case 605:
							if (id == UIObjIDs.JingLingGiftsDetailPart)
							{
								SystemHelpMgr.ActiveHelpID++;
								SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
							}
							return;
						case 606:
							if (id == UIObjIDs.JinglingCangKuPart)
							{
								SystemHelpMgr.ActiveHelpID++;
								SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
							}
							return;
						case 607:
							if (id == UIObjIDs.JinglingCangKuDetailPart)
							{
								SystemHelpMgr.EndWizard(true);
							}
							return;
						}
						break;
					case 1007:
					case 1008:
					case 1009:
					case 1010:
					case 1011:
					case 1012:
						if (id == UIObjIDs.MainGameTaskBoxTaskDesc)
						{
							SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
						}
						return;
					}
					break;
				case 720:
					break;
				case 721:
					if (id == UIObjIDs.GameLianLu)
					{
						SystemHelpMgr.ActiveHelpID++;
					}
					return;
				case 722:
					if (id == UIObjIDs.LianLuTabBtn)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 723:
					if (id == UIObjIDs.LianLuTabBtn)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 724:
					if (id == UIObjIDs.LianLuEquipItem)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 725:
					if (id == UIObjIDs.LianLuEquipItem)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					if (id == UIObjIDs.LianLuChuanChengSubmit)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 726:
					if (id == UIObjIDs.LianLuEquipItem)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 727:
				case 728:
					if (id == UIObjIDs.LianLuChuanChengSubmit)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
					return;
				case 729:
					if (id == UIObjIDs.LianLuChuanChengSubmit)
					{
						SystemHelpMgr.EndWizard(true);
					}
					return;
				}
				break;
			case 26:
				if (id == UIObjIDs.WorldMapTab && param == 0)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 27:
				if (id == UIObjIDs.WorldMapTrans && param == 1)
				{
					SystemHelpMgr.EndWizard(true);
				}
				else if (id == UIObjIDs.WorldMapTab && param == 1)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.ActiveHelpID = -1;
				}
				return;
			case 30:
				if (id == UIObjIDs.LianLuTabBtn && param == 1)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 32:
				if (id == UIObjIDs.LianLuZhuiJiaSubmit)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.ActiveHelpID = -1;
					SystemHelpMgr.ShowHint(1066, 0);
				}
				return;
			case 34:
			case 37:
			case 47:
			case 51:
				if (id == UIObjIDs.HeChengPart)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 35:
			case 41:
			case 49:
			case 53:
				if (id == UIObjIDs.HeChengSubmit)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					if (SystemHelpMgr.ActiveHelpID == 53)
					{
						SystemHelpMgr.ShowHint(1068, 0);
					}
					else
					{
						SystemHelpMgr.ActiveHelpID = -1;
						SystemHelpMgr.ActiveHelpState = -1;
					}
				}
				return;
			case 38:
				if (id == UIObjIDs.HeChengTabBtn && param == 2)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 39:
				if (id == UIObjIDs.HeChengCaiLiao && param == 0)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 40:
				if (id == UIObjIDs.HeChengJingShiItem && param == 0)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 42:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 101)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 43:
				if (id == UIObjIDs.JiFenHuishowPart)
				{
					SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
					if (e == HelpStateEvents.Actived)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.ActiveHelpState = 0;
						int param2 = 1005002;
						int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
						if (num == 0)
						{
							param2 = 1005002;
						}
						else if (num == 1)
						{
							param2 = 1015002;
						}
						else if (num == 2)
						{
							param2 = 1025402;
						}
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, param2);
					}
					else
					{
						SystemHelpMgr.ActiveHelpID = -1;
					}
				}
				return;
			case 44:
			case 45:
				if (id == UIObjIDs.JiFenHuishowPutGoods && SystemHelpMgr.ActiveHelpID == 44)
				{
					if (param == 1005002 || param == 1015002 || param == 1025402)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
				}
				else if (id == UIObjIDs.JiFenHuishowSubmit && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.EndWizard(true);
				}
				else if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 48:
				if (id == UIObjIDs.HeChengTabBtn && param == 1)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 52:
				if (id == UIObjIDs.HeChengGoodsTarget && param == 1)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 54:
				if (id == UIObjIDs.MainGameTaskBox && param == 8)
				{
					SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 55:
				if (id == UIObjIDs.MainGameTaskBoxTaskDesc)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 60:
				if (id == UIObjIDs.LianLuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.ActiveHelpID = 1000;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 100:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 100)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 101:
				if (id == UIObjIDs.NPCSalePart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveGoodsID = 1011;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 102:
				if (id == UIObjIDs.GGoodTips && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.NPCSalePart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 103:
				if (id == UIObjIDs.GGoodTips && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.ActiveGoodsID = -1;
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.NPCSalePart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 104:
				if (id == UIObjIDs.NPCSalePart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 106:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 105)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 107:
				if (id == UIObjIDs.PortableBagPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveGoodsID = 7004;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 108:
				if (id == UIObjIDs.PortableBagPartGetGoods)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.PortableBagPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 110:
				if (id == UIObjIDs.PortableBagPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 112:
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 113:
				if (id == UIObjIDs.QiFuPartBtn01 && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 114:
				if (id == UIObjIDs.QiFuGiftsPartOK && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 115:
				if (id == UIObjIDs.QiFuCangKuPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveGoodsID = int.MaxValue;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 116:
				if (id == UIObjIDs.QiFuCangKuPartQuHui)
				{
					SystemHelpMgr.EndWizard(true);
				}
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 117:
				if (id == UIObjIDs.QiFuCangKuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 118:
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 119:
				if (id == UIObjIDs.QiFuPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 120:
				if (id == UIObjIDs.QiFuPartBtn01 && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.QiFuCangKuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 121:
				if (id == UIObjIDs.QiFuCangKuPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				if (id == UIObjIDs.QiFuCangKuPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 123:
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 124:
				if (id == UIObjIDs.FriendRecommendPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 125:
				if (id == UIObjIDs.FriendRecommendPartBtnAll && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 126:
				if (id == UIObjIDs.FriendRecommendPartBtnAdd && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				else if (id == UIObjIDs.FriendRecommendPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 127:
				if (id == UIObjIDs.FriendRecommendPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 128:
				if (id == UIObjIDs.FriendsPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 130:
				if (id == UIObjIDs.AutoPart && param == 1)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 131:
				if (id == UIObjIDs.AutoPartStart && param == 1)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 134:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 101)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 135:
				if (id == UIObjIDs.JiFenHuishowPart)
				{
					SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
					if (e == HelpStateEvents.Actived)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.ActiveHelpState = 0;
						int num2 = 10004;
						int num3 = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
						if (num3 == 0)
						{
							num2 = 10004;
						}
						else if (num3 == 1)
						{
							num2 = 10005;
						}
						else if (num3 == 2)
						{
							num2 = 10006;
						}
						SystemHelpMgr.ActiveGoodsID = num2;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, num2);
					}
					else
					{
						SystemHelpMgr.EndWizard(true);
					}
				}
				if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 136:
				if (id == UIObjIDs.JiFenHuishowPutGoods && (param == 10004 || param == 10005 || param == 10006))
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 137:
				if (id == UIObjIDs.JiFenHuishowSubmit && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 138:
				if (id == UIObjIDs.JiFenHuishowPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 140:
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 101)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.MUDuiHuanPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 141:
				if (id == UIObjIDs.MUDuiHuanPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.MUDuiHuanPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 142:
				if (id == UIObjIDs.MUDuiHuanPartListItemBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				if (id == UIObjIDs.MUDuiHuanPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 143:
				if (id == UIObjIDs.MUDuiHuanPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 145:
				if (id == UIObjIDs.LeaderRoleFace && param == 1)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 146:
				if (id == UIObjIDs.PaiHangPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.PaiHangPartMoBai && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 147:
				if (id == UIObjIDs.PaiHangPartMoBai && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 148:
				if (id == UIObjIDs.LeaderRoleFace && param == 0)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 149:
				if (id == UIObjIDs.HuoDongPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.HuoDongPartTabBtn && e == HelpStateEvents.Clicked && param == 7)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 150:
				if (id == UIObjIDs.HuoDongPartTabBtn && e == HelpStateEvents.Clicked && param == 7)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 160:
				if (id == UIObjIDs.LeaderRoleFace && param == 0)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 161:
				if (id == UIObjIDs.JingJiChangPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				if (id == UIObjIDs.JingJiChangPartTianZhanBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 162:
				if (id == UIObjIDs.JingJiChangPartTianZhanBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 170:
				if (id == UIObjIDs.SkillSelectPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 171:
				if (id == UIObjIDs.SkillSelectPart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 182:
				if (id == UIObjIDs.HuoDongPartTabBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				else if (id == UIObjIDs.RiChangFuBenDetailpart && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 183:
				if (id == UIObjIDs.RiChangFuBenDetailpart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 184:
				if (id == UIObjIDs.RiChangFuBenDetailpartEnter && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 190:
				SystemHelpMgr.ActiveHelpState = 0;
				if (id == UIObjIDs.NpcDialogPart && e == HelpStateEvents.Actived && param == 117)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 191:
				if ((id == UIObjIDs.ZhuanShengSubmit && e == HelpStateEvents.Clicked) || (id == UIObjIDs.ZhuanShengPart && e == HelpStateEvents.Actived))
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 192:
				if (((id == UIObjIDs.ZhuanShengSubmit && e == HelpStateEvents.Clicked) || (id == UIObjIDs.ZhuanShengPart && e == HelpStateEvents.Inactived)) && SystemHelpMgr.ActiveHelpState++ >= 2)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 198:
				if (id == UIObjIDs.GamePayerRolePart || id == UIObjIDs.LeaderRoleFace)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 200:
				if (id == UIObjIDs.ChiBangPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					SystemHelpMgr.ActiveHelpState = 0;
				}
				return;
			case 201:
				if (id == UIObjIDs.ChiBangPartTiShengBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.ActiveHelpState++;
					if (SystemHelpMgr.ActiveHelpState == 5)
					{
						SystemHelpMgr.ActiveHelpID++;
						SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
					}
				}
				return;
			case 202:
				if (id == UIObjIDs.ChiBangPartTiPeiDaiBtn && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 203:
				if (id == UIObjIDs.GamePayerRolePart && e == HelpStateEvents.Inactived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 210:
				if (id == UIObjIDs.RiChangFuBenPaTaFuBenPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.ActiveHelpID++;
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				}
				return;
			case 211:
				if (id == UIObjIDs.RiChangFuBenPaTaFuBenPartEnter && e == HelpStateEvents.Clicked)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			case 220:
				if (id == UIObjIDs.HuoDongPart && e == HelpStateEvents.Actived)
				{
					SystemHelpMgr.EndWizard(true);
				}
				return;
			}
			SystemHelpMgr.ActiveHelpState = 0;
			if (id == UIObjIDs.LeaderRoleFace && param == 1)
			{
				SystemHelpMgr.ActiveHelpID++;
				SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 1);
				SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
			}
		}

		public static void DoAction(int activeHelpID, int param)
		{
			if (activeHelpID != -1)
			{
				SystemHelpMgr.RootPart.DoAction(activeHelpID, param);
			}
			else
			{
				SystemHelpMgr.ActiveHintID = -1;
				SystemHelpMgr.ActiveHelpID = -1;
				SystemHelpMgr.ActiveHelpState = -1;
			}
		}

		private static void EndWizard(bool cancleMask = true)
		{
			if (cancleMask)
			{
				SystemHelpPart.HideMask();
			}
			SystemHelpMgr.ActiveHintID = -1;
			SystemHelpMgr.ActiveHelpID = -1;
			SystemHelpMgr.ActiveGoodsID = -1;
			SystemHelpMgr.ActiveHelpState = -1;
			if (SystemHelpMgr.PostTaskPlotID > 0)
			{
				PlayZone.GlobalPlayZone.TriggerTaskPlotByID(SystemHelpMgr.PostTaskPlotID);
			}
			else if (SystemHelpMgr.PostTaskPlotID > 0)
			{
				GongnengYugaoMgr.ReadyFlyingImgAnimation(SystemHelpMgr.RootPart as PlayZone, SystemHelpMgr.PostTaskPlotID);
			}
			if (SystemHelpMgr.NextHelpID > 0)
			{
				SystemHelpMgr.ShowHint(SystemHelpMgr.NextHelpID, 0);
			}
			else
			{
				SystemHelpMgr.PostTaskPlotID = -1;
				SystemHelpMgr.PostOpenID = -1;
				SystemHelpMgr.NextHelpID = -1;
			}
		}

		public static void Reset()
		{
			Global.CanGiveFakeEquips = false;
			Global.FakeEquipsIndex = 0;
			SystemHelpPart.HideMask();
			SystemHelpPart.ShowDirectionWizard(false, default(Vector3));
			SystemHelpPart.ShowHintText(false, null, null, Dircetions.DR_UP);
			SystemHelpPart.ShowHintTextNoTarget(false, null, 3);
			if (SystemHelpMgr.ActiveHelpID > 0)
			{
				if (SystemHelpMgr.RootPart != null)
				{
					SystemHelpMgr.RootPart.DoAction(SystemHelpMgr.ActiveHelpID, 0);
				}
				SystemHelpMgr.DoAction(-1, -1);
			}
		}

		public static void ShowHint(int HintID, int forceType = 0)
		{
			if (!SystemHelpMgr.ConfigLoaded)
			{
				SystemHelpMgr.LoadConfig();
			}
			SystemHelpKeysData systemHelpKeysData = null;
			if (HintID > 0 && SystemHelpMgr.SystemHelpDict.TryGetValue(HintID, ref systemHelpKeysData))
			{
				Super.CloseAllUIWindow();
				if (systemHelpKeysData.HintID > 0)
				{
					if (systemHelpKeysData.HintType == 0)
					{
						SystemHelpPart.ShowHintTextNoTarget(true, systemHelpKeysData.HintText, 3);
					}
					else if (systemHelpKeysData.HintType == 2)
					{
						SystemHelpPart.ShowHintTextNoTarget(true, systemHelpKeysData.HintText, 3);
					}
					else
					{
						SystemHelpMgr.ActiveHintText = systemHelpKeysData.HintText;
					}
				}
				else
				{
					SystemHelpMgr.ActiveHintID = -1;
					SystemHelpMgr.ActiveHintType = -1;
					SystemHelpMgr.ActiveHintText = null;
				}
				SystemHelpMgr.ActiveHelpID = HintID;
				SystemHelpMgr.ActiveHelpState = 0;
				SystemHelpMgr.PostTaskPlotID = systemHelpKeysData.PostTaskPlotID;
				SystemHelpMgr.PostOpenID = systemHelpKeysData.PostOpenID;
				if (systemHelpKeysData.NextIDs != null && systemHelpKeysData.NextIDs.Length > 0)
				{
					SystemHelpMgr.NextHelpID = systemHelpKeysData.NextIDs[0];
				}
				else
				{
					SystemHelpMgr.NextHelpID = -1;
				}
				SystemHelpMgr.RootPart.PreWizardExec();
				SystemHelpMgr.RootPart.DoAction(HintID, 1);
			}
			else if (forceType == 1)
			{
				Super.CloseAllUIWindow();
				SystemHelpMgr.ActiveHelpID = HintID;
				SystemHelpMgr.ActiveHintID = HintID;
				SystemHelpMgr.ActiveHintText = null;
				SystemHelpMgr.PostTaskPlotID = -1;
				SystemHelpMgr.PostOpenID = -1;
				SystemHelpMgr.NextHelpID = -1;
				SystemHelpMgr.RootPart.PreWizardExec();
				SystemHelpMgr.RootPart.DoAction(HintID, 1);
				SystemHelpMgr.ActiveHelpID = -1;
				SystemHelpMgr.ActiveHintID = -1;
			}
			else if (forceType == 2)
			{
				Super.CloseAllUIWindow();
				SystemHelpMgr.ActiveHelpID = HintID;
				SystemHelpMgr.ActiveHintID = HintID;
				SystemHelpMgr.ActiveHintText = null;
				SystemHelpMgr.PostTaskPlotID = -1;
				SystemHelpMgr.PostOpenID = -1;
				SystemHelpMgr.NextHelpID = -1;
				SystemHelpMgr.RootPart.PreWizardExec();
				SystemHelpMgr.RootPart.DoAction(HintID, 1);
			}
			else
			{
				SystemHelpMgr.ActiveHelpID = -1;
				SystemHelpMgr.ActiveHintID = -1;
				SystemHelpMgr.NextHelpID = -1;
				SystemHelpMgr.PostOpenID = -1;
				SystemHelpMgr.PostTaskPlotID = -1;
				SystemHelpMgr.ActiveHintText = null;
			}
		}

		public static void OnGridChanged(int gridX, int gridY)
		{
			if (SystemHelpMgr.TargetPositonHandler != null && Mathf.Abs(gridX - SystemHelpMgr.GridTargetX) <= SystemHelpMgr.GridRangeX && Mathf.Abs(gridY - SystemHelpMgr.GridTargetY) <= SystemHelpMgr.GridRangeY)
			{
				SystemHelpMgr.TargetPositonHandler.Invoke(null, EventArgs.Empty);
				SystemHelpMgr.TargetPositonHandler = null;
			}
		}

		public static void SetTargetGridEventHandler(int gridTargetX, int gridTargetY, int gridRangeX, int gridRangeY, EventHandler handler)
		{
			SystemHelpMgr.GridTargetX = gridTargetX;
			SystemHelpMgr.GridTargetY = gridTargetY;
			SystemHelpMgr.GridRangeX = gridRangeX;
			SystemHelpMgr.GridRangeY = gridRangeY;
			SystemHelpMgr.TargetPositonHandler = handler;
		}

		public static bool DisableNpcClick(int npcExtensionID)
		{
			return (SystemHelpMgr.ActiveHelpID == 18 && npcExtensionID == 116) || (SystemHelpMgr.ActiveHelpID == 22 && npcExtensionID == 114) || ((SystemHelpMgr.ActiveHelpID != 100 || npcExtensionID != 100) && ((SystemHelpMgr.ActiveHelpID != 134 && SystemHelpMgr.ActiveHelpID != 140) || npcExtensionID != 101) && (SystemHelpMgr.ActiveHelpID != 106 || npcExtensionID != 105) && ((npcExtensionID >= 91000 && npcExtensionID <= 91003) || Global.DicThemeActivityNpc.ContainsKey(npcExtensionID)));
		}

		public static bool CanAutoRoad()
		{
			return (!(null != PlayGameGuide.singleton) || !PlayGameGuide.singleton.Visibility) && (SystemHelpMgr.ActiveHelpID <= 0 || SystemHelpMgr.ActiveHelpID > 1000) && !Global.Data.WaitingForSystemHelp;
		}

		public static bool CanAutoFight()
		{
			return (!(null != PlayGameGuide.singleton) || !PlayGameGuide.singleton.Visibility) && !Global.Data.WaitingForSystemHelp;
		}

		public static void RegisterPart(ISystemHelpPart part)
		{
			SystemHelpMgr.RootPart = part;
		}

		public static void OnComponentEvent(ISystemHelpPart sender, HelpStateEvents events, int param = -1)
		{
		}

		public static void ProcessHelpEvent(ObjectItem item, SystemHelpKeysData data)
		{
		}

		public static void UpdateItem(ObjectItem item, HelpStateEvents e, int param)
		{
			if (e == HelpStateEvents.Enable && !item.Enabled)
			{
				item.Enabled = true;
				item.Changed = true;
			}
			else if (e == HelpStateEvents.Disable && item.Enabled)
			{
				item.Enabled = false;
				item.Changed = true;
			}
			else if (item.Value < 0 || item.Value >= 1000)
			{
				if (e == HelpStateEvents.Destory && !item.Destoryed)
				{
					item.Destoryed = true;
					item.Changed = true;
				}
				else if (e == HelpStateEvents.Clicked && !item.Clicked)
				{
					item.Clicked = true;
					item.Changed = true;
				}
			}
		}

		public static void ClearXMLData()
		{
			if (0 < SystemHelpMgr.SystemHelpDict.Count)
			{
				SystemHelpMgr.SystemHelpDict.Clear();
			}
			if (0 < SystemHelpMgr.SystemHelpModeDict.Count)
			{
				SystemHelpMgr.SystemHelpModeDict.Clear();
			}
		}

		public static void LoadConfig()
		{
			SystemHelpMgr.ConfigLoaded = true;
			XElement gameResXml = Global.GetGameResXml("Config/SystemHelp.Xml");
			if (gameResXml == null)
			{
				return;
			}
			int num = -1;
			IEnumerable<XElement> enumerable = gameResXml.Elements();
			foreach (XElement xelement in enumerable)
			{
				num++;
				SystemHelpKeysData systemHelpKeysData = new SystemHelpKeysData();
				systemHelpKeysData.ID = Global.GetXElementAttributeInt(xelement, "ID");
				systemHelpKeysData.OccupCondition = Global.GetXElementAttributeInt(xelement, "OccupCondition");
				systemHelpKeysData.TriggerCondition = Global.GetXElementAttributeInt(xelement, "TriggerCondition");
				systemHelpKeysData.PrevIDs = Global.GetXElementAttributeIntArray(xelement, "PrevID", "*");
				systemHelpKeysData.NextIDs = Global.GetXElementAttributeIntArray(xelement, "NextID", "*");
				systemHelpKeysData.TimeParameters = Global.GetXElementAttributeInt(xelement, "TimeParameters");
				systemHelpKeysData.HintText = Global.GetXElementAttributeStr(xelement, "Text");
				systemHelpKeysData.HintID = ((!string.IsNullOrEmpty(systemHelpKeysData.HintText)) ? systemHelpKeysData.ID : -1);
				systemHelpKeysData.HintType = Global.GetXElementAttributeInt(xelement, "HintType");
				systemHelpKeysData.PostTaskPlotID = Global.GetXElementAttributeInt(xelement, "PostTaskPlotID");
				SystemHelpMgr.SystemHelpDict.Add(systemHelpKeysData.ID, systemHelpKeysData);
				List<int> list;
				if (!SystemHelpMgr.SystemHelpModeDict.TryGetValue(systemHelpKeysData.TriggerCondition, ref list))
				{
					list = new List<int>();
					SystemHelpMgr.SystemHelpModeDict.Add(systemHelpKeysData.TriggerCondition, list);
				}
				list.Add(systemHelpKeysData.ID);
			}
		}

		public static bool ShowSystemHelp(SystemHelpModes mode, int param = -1)
		{
			if (!SystemHelpMgr.ConfigLoaded)
			{
				SystemHelpMgr.LoadConfig();
			}
			List<int> list;
			if (!SystemHelpMgr.SystemHelpModeDict.TryGetValue((int)mode, ref list) || list == null || list.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < list.Count; i++)
			{
				int num = list[i];
				bool flag = false;
				SystemHelpKeysData systemHelpKeysData;
				if (SystemHelpMgr.SystemHelpDict.TryGetValue(num, ref systemHelpKeysData))
				{
					int timeParameters = systemHelpKeysData.TimeParameters;
					if (mode == SystemHelpModes.ToLevel)
					{
						if (timeParameters == Global.Data.roleData.ChangeLifeCount)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.ToMap)
					{
						if (timeParameters == param)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.LeaveMap)
					{
						if (timeParameters == param)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.FirstLogin)
					{
						if (Global.Data.roleData.LoginNum <= 0)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.FirstGoods)
					{
						if (Global.Data.FirstNewGoodsIDList.Count > 0)
						{
							int num2 = Global.Data.FirstNewGoodsIDList[0];
							if (timeParameters == num2)
							{
								Global.Data.FirstNewGoodsIDList.RemoveAt(0);
								flag = true;
							}
						}
					}
					else if (mode == SystemHelpModes.Login)
					{
						flag = true;
					}
					else if (mode == SystemHelpModes.NewTask)
					{
						if (timeParameters == param)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.CompTask)
					{
						if (timeParameters == param)
						{
							flag = true;
						}
					}
					else if (mode == SystemHelpModes.LeaveSafeArea)
					{
						flag = true;
					}
					if (flag)
					{
						systemHelpKeysData.Active = true;
						Global.Data.GameScene.CancelAutoFight(0, true);
						Global.Data.GameScene.CancelAutoFindRoad(true);
						SystemHelpMgr.ShowHint(systemHelpKeysData.ID, 0);
						return true;
					}
				}
			}
			return false;
		}

		public static void TimerTick(long ticks)
		{
		}

		public static bool ValidateItemValue(ObjectItem item, int itemValue)
		{
			bool result = false;
			if (item.Changed)
			{
				item.Changed = false;
				if (itemValue > 1000 && itemValue < 1011)
				{
					switch (itemValue)
					{
					case 1002:
						result = item.Enabled;
						break;
					case 1003:
						result = !item.Enabled;
						break;
					case 1004:
						result = item.Actived;
						break;
					case 1005:
						result = !item.Enabled;
						break;
					case 1006:
						result = item.Destoryed;
						break;
					case 1007:
						result = item.Clicked;
						item.Clicked = false;
						break;
					case 1008:
						result = item.Clicked;
						break;
					}
				}
				else if (itemValue == item.Value)
				{
					result = true;
				}
			}
			return result;
		}

		public static void UpdateItemValue(KeyValuePair<SystemHelpKeysData, SystemHelpPart> pair)
		{
			if (pair.Key.State >= SystemHelpPartStates.Max)
			{
				return;
			}
			int state = (int)pair.Key.State;
			int num = pair.Key.ItemIDs[state];
			if (SystemHelpMgr.ComponentIDTable.ContainsKey(num))
			{
				int itemValue = pair.Key.ItemValues[state];
				ObjectItem objectItem = SystemHelpMgr.ComponentIDTable[num] as ObjectItem;
				if (objectItem.Target == null || objectItem.Destoryed)
				{
					SystemHelpMgr.ComponentPartTable.Remove(objectItem.Target);
					SystemHelpMgr.ComponentIDTable.Remove(num);
					return;
				}
				if (!SystemHelpMgr.ValidateItemValue(objectItem, itemValue))
				{
					return;
				}
				if (pair.Key.State == SystemHelpPartStates.Target)
				{
					pair.Key.State++;
					pair.Value.InitPart(objectItem.Target, pair.Key.ID);
				}
				if (pair.Key.State == SystemHelpPartStates.Show)
				{
					pair.Key.State++;
					pair.Value.ShowPart(objectItem.Target, pair.Key.ID);
				}
				if (pair.Key.State == SystemHelpPartStates.Hide)
				{
					pair.Key.State++;
					pair.Value.HidePart(objectItem.Target, pair.Key.ID);
				}
			}
		}

		public static int _ActiveHelpID = 0;

		private static int _ActiveHintID;

		public static int ActiveHintType;

		public static string ActiveHintText;

		public static int ActiveGoodsID = -1;

		public static int ActiveNpcID = -1;

		public static int NextHelpID = -1;

		public static int ActiveHelpState;

		public static bool EnableWizard;

		public static int PostTaskPlotID = -1;

		public static int PostOpenID = -1;

		public static int HintTextForSkillIcon = -1;

		private static Dictionary<int, List<int>> PrevDict = new Dictionary<int, List<int>>();

		private static Dictionary<int, List<int>> NextDict = new Dictionary<int, List<int>>();

		private static Dictionary<int, List<int>> SystemHelpModeDict = new Dictionary<int, List<int>>();

		private static Dictionary<int, SystemHelpKeysData> SystemHelpDict = new Dictionary<int, SystemHelpKeysData>();

		private static Dictionary<SystemHelpKeysData, SystemHelpPart> SystemHelpActiveList = new Dictionary<SystemHelpKeysData, SystemHelpPart>();

		private static Hashtable ComponentPartTable = new Hashtable();

		private static Hashtable ComponentIDTable = new Hashtable();

		private static List<int> ActiveCtrlIDList = new List<int>();

		private static Dictionary<int, CAnimation> TempAnimList = new Dictionary<int, CAnimation>();

		private static ISystemHelpPart RootPart = null;

		private static int GridTargetX = 0;

		private static int GridTargetY = 0;

		private static int GridRangeX = 0;

		private static int GridRangeY = 0;

		private static EventHandler TargetPositonHandler = null;

		private static bool ConfigLoaded = false;
	}
}
