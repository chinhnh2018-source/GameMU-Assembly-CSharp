using System;
using HSGameEngine.GameEngine.Logic;

namespace GameServer.Logic
{
	public static class StdErrorCode
	{
		public static string GetErrMsg(int errCode, bool hefuluolan = false, bool inKuafuhuodong = false)
		{
			string result = Global.GetLang("未知错误...错误码[") + errCode + "]";
			if (errCode < 0)
			{
				switch (errCode + 47)
				{
				case 0:
					result = Global.GetLang("元素觉醒石不足");
					break;
				default:
					switch (errCode + 1046)
					{
					case 0:
						result = Global.GetLang("没有入侵者");
						break;
					case 1:
						result = Global.GetLang("进入人数已满");
						break;
					case 2:
						result = Global.GetLang("战斗已结束");
						break;
					case 3:
						result = Global.GetLang("竞价出价太少");
						break;
					case 4:
						result = Global.GetLang("目标服务器不在本轮竞价");
						break;
					case 5:
						result = Global.GetLang("战盟资金不足");
						break;
					case 6:
						result = Global.GetLang("服务器不存在(服务器不在线)");
						break;
					case 7:
						result = Global.GetLang("服务器不存在(不再同一分组");
						break;
					case 8:
						result = Global.GetLang("竞价失败,出价未上榜");
						break;
					case 9:
						result = Global.GetLang("参加活动中 不允许解散");
						break;
					case 10:
						result = Global.GetLang("已报名战盟联赛，暂时不允许解散");
						break;
					case 11:
						result = Global.GetLang("当前已达到发言人数上限");
						break;
					case 12:
						result = Global.GetLang("精英人数已达上限");
						break;
					case 13:
						result = Global.GetLang("加入战盟后才可以收发红包");
						break;
					case 14:
						result = Global.GetLang("拥有领地的军团不允许解散、转移军团长");
						break;
					case 15:
						result = Global.GetLang("军团战盟数量已满");
						break;
					case 16:
						result = Global.GetLang("军团繁荣度不够");
						break;
					case 17:
						result = Global.GetLang("任务不存在");
						break;
					case 18:
						result = Global.GetLang("任务未完成");
						break;
					case 19:
						result = Global.GetLang("军团名非法");
						break;
					case 20:
						result = Global.GetLang("军团已变化");
						break;
					case 21:
						result = Global.GetLang("不在军团中");
						break;
					case 22:
						result = Global.GetLang("军团长才能进行此操作");
						break;
					case 23:
						result = Global.GetLang("军团名已经存在");
						break;
					case 24:
						result = Global.GetLang("军团不存在");
						break;
					case 25:
						result = Global.GetLang("已经在申请列表中");
						break;
					case 26:
						result = Global.GetLang("已经在军团中");
						break;
					default:
						switch (errCode + 4038)
						{
						case 0:
							result = Global.GetLang("队长已离开活动，不能进入");
							break;
						case 1:
							result = Global.GetLang("活动已开始，不能进入！");
							break;
						case 2:
							result = Global.GetLang("战队轮空");
							break;
						case 3:
							result = Global.GetLang("战队未报名");
							break;
						case 4:
							result = Global.GetLang("战队已经报名");
							break;
						case 5:
							result = Global.GetLang("战队押注数量上限");
							break;
						case 6:
							result = Global.GetLang("已押注");
							break;
						case 7:
							result = Global.GetLang("战队不存在");
							break;
						case 8:
							result = Global.GetLang("未找到该玩家");
							break;
						case 9:
							result = Global.GetLang("队长已拒绝");
							break;
						case 10:
							result = Global.GetLang("战队人数已达上限");
							break;
						case 11:
							result = Global.GetLang("含有非法字符");
							break;
						case 12:
							result = Global.GetLang("战队人数不足");
							break;
						case 13:
							result = Global.GetLang("该战队已解散，请重新选择其他战队！");
							break;
						case 14:
							result = Global.GetLang("已经在其他战队中");
							break;
						case 15:
							result = Global.GetLang("名字已经存在");
							break;
						default:
							switch (errCode + 2008)
							{
							case 0:
								result = Global.GetLang("活动已进入战斗阶段，不能进入！");
								break;
							case 1:
								result = Global.GetLang("申请时间未冷却");
								break;
							case 2:
								result = Global.GetLang("加入战盟时间不足!");
								break;
							case 3:
								result = Global.GetLang("您的操作太快，请稍后再试！");
								break;
							case 4:
								result = Global.GetLang("当前处于惩罚时间内，不能参加任何跨服副本！");
								break;
							case 5:
								result = Global.GetLang("已超时");
								break;
							case 6:
								if (inKuafuhuodong)
								{
									result = Global.GetLang("您已报名（幻影寺院、勇者战场),禁止参加此类活动");
								}
								else
								{
									result = Global.GetLang("活动时间禁止操作");
								}
								break;
							case 7:
								if (hefuluolan)
								{
									result = Global.GetLang("活动未开启！");
								}
								else
								{
									result = Global.GetLang("当前时间没有比赛！");
								}
								break;
							default:
								switch (errCode + 11004)
								{
								case 0:
									result = Global.GetLang("功能未开放");
									break;
								case 1:
									result = Global.GetLang("服务器内部错误！");
									break;
								default:
									switch (errCode + 402)
									{
									case 0:
										result = Global.GetLang("战队时间限制");
										break;
									default:
										if (errCode != -101)
										{
											if (errCode != -100)
											{
												if (errCode != -500)
												{
													if (errCode != -200)
													{
														result = Global.GetLang("其他错误！错误码[") + errCode + "]";
													}
													else
													{
														result = Global.GetLang("已经领取！");
													}
												}
												else
												{
													result = Global.GetLang("眩晕状态不能变身！");
												}
											}
											else
											{
												result = Global.GetLang("背包不足");
											}
										}
										else
										{
											result = Global.GetLang("重生包裹格子不足");
										}
										break;
									case 2:
										result = Global.GetLang("功能暂未开启");
										break;
									}
									break;
								case 3:
									result = Global.GetLang("活动暂未开放！");
									break;
								case 4:
									result = Global.GetLang("服务器繁忙");
									break;
								}
								break;
							}
							break;
						case 17:
							result = Global.GetLang("服务器繁忙");
							break;
						case 18:
							result = Global.GetLang("玩家已离线");
							break;
						case 19:
							result = Global.GetLang("已经在战场中");
							break;
						case 20:
							result = Global.GetLang("时间有误");
							break;
						case 21:
							result = Global.GetLang("不是战队成员");
							break;
						case 22:
							result = Global.GetLang("不是战队队长");
							break;
						case 23:
							result = Global.GetLang("正在等待中");
							break;
						case 24:
							result = Global.GetLang("已经在战队中");
							break;
						case 25:
							result = Global.GetLang("战队不存在");
							break;
						case 26:
							result = Global.GetLang("已有战队");
							break;
						case 27:
							result = Global.GetLang("名称有误");
							break;
						case 30:
							result = Global.GetLang("战盟未报名");
							break;
						case 31:
							result = Global.GetLang("活动未开放");
							break;
						case 32:
							result = Global.GetLang("战斗已结束！");
							break;
						case 33:
							result = Global.GetLang("战盟已经报名！");
							break;
						case 34:
							result = Global.GetLang("已经达到最高等级！");
							break;
						case 36:
							result = Global.GetLang("已经在副本中！");
							break;
						case 37:
							result = Global.GetLang("已经在匹配队列中！");
							break;
						case 38:
							result = Global.GetLang("跨服副本不存在(已结束)！");
							break;
						}
						break;
					case 35:
						result = Global.GetLang("战盟名字已被占用");
						break;
					case 36:
						result = Global.GetLang("已经在战盟中");
						break;
					case 37:
						result = Global.GetLang("加入战盟太晚");
						break;
					case 39:
						result = Global.GetLang("圣域领主持有期间不能委任其他成员为战盟首领");
						break;
					case 40:
						result = Global.GetLang("罗兰城主持有期间不能委任其他成员为战盟首领");
						break;
					case 41:
						result = Global.GetLang("角色的战盟职务未配置相应奖励！");
						break;
					case 42:
						result = Global.GetLang("战盟已经竞标了另一个名额！");
						break;
					case 43:
						result = Global.GetLang("战盟没有参战资格！");
						break;
					case 44:
						result = Global.GetLang("战盟首领才能进行此操作！");
						break;
					case 45:
						result = Global.GetLang("战盟不存在！");
						break;
					case 46:
						result = Global.GetLang("不在战盟中！");
						break;
					}
					break;
				case 6:
					result = Global.GetLang("发红包频率过快");
					break;
				case 7:
					result = Global.GetLang("包含非法字符");
					break;
				case 8:
					result = Global.GetLang("每日活动充值积分不足");
					break;
				case 10:
					result = Global.GetLang("钻石交易限制");
					break;
				case 11:
					result = Global.GetLang("精英人数已达上限");
					break;
				case 12:
					result = Global.GetLang("副职业禁止操作");
					break;
				case 13:
					result = Global.GetLang("当前地图禁止切换职业");
					break;
				case 14:
					result = Global.GetLang("必须在安全区操作");
					break;
				case 15:
					result = Global.GetLang("魅力点不足");
					break;
				case 16:
					result = Global.GetLang("赠送已结束，系统正在结算排名");
					break;
				case 17:
					result = Global.GetLang("其他玩家正在赠送,请稍后再试");
					break;
				case 18:
					result = Global.GetLang("被祝福玩家未结婚");
					break;
				case 19:
					result = Global.GetLang("被祝福玩家不存在");
					break;
				case 20:
					result = Global.GetLang("被祝福玩家不能是自己");
					break;
				case 21:
					result = Global.GetLang("祝福寄语字符数超出限制");
					break;
				case 22:
					result = Global.GetLang("不可添加祝福寄语");
					break;
				case 23:
					result = Global.GetLang("专享积分不足！");
					break;
				case 24:
					result = Global.GetLang("已达到最高等级");
					break;
				case 25:
					if (Global.IsCompMiDongMap())
					{
						result = Global.GetLang("地图中人数已满，请尝试参与其他矿洞");
					}
					else
					{
						result = Global.GetLang("参与者数量已满！");
					}
					break;
				case 26:
					result = Global.GetLang("当前地图不允许此操作！");
					break;
				case 27:
					result = Global.GetLang("不存在");
					break;
				case 28:
					result = Global.GetLang("等级不足,无法进入");
					break;
				case 29:
					result = Global.GetLang("错误的参数");
					break;
				case 30:
					result = Global.GetLang("绑钻不足");
					break;
				case 31:
					result = Global.GetLang("无剩余次数");
					break;
				case 32:
					result = Global.GetLang("数据库操作失败");
					break;
				case 33:
					result = Global.GetLang("未选择消耗钱类型");
					break;
				case 34:
					result = Global.GetLang("拒绝操作");
					break;
				case 35:
					result = Global.GetLang("条件不足！");
					break;
				case 36:
					result = Global.GetLang("操作失败");
					break;
				case 37:
					result = Global.GetLang("钻石不足");
					break;
				case 38:
					result = Global.GetLang("金币不足！");
					break;
				case 39:
					result = Global.GetLang("物品未找到！");
					break;
				case 40:
					result = Global.GetLang("物品在使用,不允许操作");
					break;
				case 41:
					result = Global.GetLang("物品不足！");
					break;
				case 42:
					result = Global.GetLang("无效操作");
					break;
				case 44:
					result = Global.GetLang("配置错误");
					break;
				case 45:
					result = Global.GetLang("无效的索引值");
					break;
				case 46:
					result = Global.GetLang("无效的dbid");
					break;
				case 50:
					result = Global.GetLang("操作成功完成但结果无变化");
					break;
				case 51:
					result = Global.GetLang("操作成功完成但需等待结果");
					break;
				}
			}
			return result;
		}

		public const int Error_Success_No_Info = 0;

		public const int Error_Success = 1;

		public const int Error_Success_Bind = 2;

		public const int Error_Success_No_Change = 3;

		public const int Error_Success_Wait = 4;

		public const int Error_Success_Retry = 5;

		public const int Error_Invalid_DBID = -1;

		public const int Error_Invalid_Index = -2;

		public const int Error_Config_Fault = -3;

		public const int Error_Data_Overdue = -4;

		public const int Error_Invalid_Operation = -5;

		public const int Error_Goods_Not_Enough = -6;

		public const int Error_Goods_Is_Using = -7;

		public const int Error_Goods_Not_Find = -8;

		public const int Error_JinBi_Not_Enough = -9;

		public const int Error_ZuanShi_Not_Enough = -10;

		public const int Error_Operation_Faild = -11;

		public const int Error_Operation_Denied = -12;

		public const int Error_Type_Not_Match = -13;

		public const int Error_MoneyType_Not_Select = -14;

		public const int Error_DB_Faild = -15;

		public const int Error_No_Residue_Degree = -16;

		public const int Error_BangZuan_Not_Enough = -17;

		public const int Error_Invalid_Params = -18;

		public const int Error_Level_Limit = -19;

		public const int Error_Not_Exist = -20;

		public const int Error_Denied_In_Current_Map = -21;

		public const int Error_Player_Count_Limit = -22;

		public const int Error_Level_Reach_Max_Level = -23;

		public const int Error_SpecJiFen_Not_Enough = -24;

		public const int Error_Cannot_Have_Wish_Txt = -25;

		public const int Error_Wish_Txt_Length_Limit = -26;

		public const int Error_Cannot_Wish_Self = -27;

		public const int Error_Wish_Player_Not_Exist = -28;

		public const int Error_Wish_Player_Not_Marry = -29;

		public const int Error_Wish_Type_Is_In_CD = -30;

		public const int Error_Wish_In_Balance_Time = -31;

		public const int Error_CharmPoint_Not_Enough = -32;

		public const int Error_MaxLevel = -23;

		public const int Error_Must_In_SafeRegion = -33;

		public const int Error_Disable_ChangeOccupation_This_Map = -34;

		public const int Error_Denied_For_Minor_Occupation = -35;

		public const int Error_JingYingNumberNotEnough = -36;

		public const int Error_ZuanShi_Trade_Limit = -37;

		public const int Error_Trade_Limit = -38;

		public const int Error_EveryJiFen_Not_Enough = -39;

		public const int Error_Invalid_Character = -40;

		public const int Error_FaHongBao_Too_Much = -41;

		public const int Error_No_Surplus = -42;

		public const int Error_Skill_Not_Finish = -43;

		public const int Error_Too_Much = -44;

		public const int Error_Money_JueXing_Not_Enough = -45;

		public const int Error_Money_JueXingZhiChen_Not_Enough = -46;

		public const int Error_Money_YuanSuJueXingShi_Not_Enough = -47;

		public const int Error_10 = -10;

		public const int Error_Invalid_In_KuaFu = -12;

		public const int Error_20 = -20;

		public const int Error_30 = -30;

		public const int Error_40 = -40;

		public const int Error_50 = -50;

		public const int Error_60 = -60;

		public const int Error_70 = -70;

		public const int Error_80 = -80;

		public const int Error_BagNum_Not_Enough = -100;

		public const int Error_Has_Get = -200;

		public const int Error_Faild_No_Message = -201;

		public const int Error_Has_Ownen_ShengBei = -300;

		public const int Error_Too_Far = -301;

		public const int Error_Other_Has_Get = -302;

		public const int Error_CaiJi_Break = -303;

		public const int Error_GonegNeng_Not_Open = -400;

		public const int Error_Xuanyun_State = -500;

		public const int Error_ZhanMeng_Not_In_ZhanMeng = -1000;

		public const int Error_ZhanMeng_Not_Exist = -1001;

		public const int Error_ZhanMeng_ShouLing_Only = -1002;

		public const int Error_ZhanMeng_Is_Unqualified = -1003;

		public const int Error_ZhanMeng_Has_Bid_OtherSite = -1004;

		public const int Error_ZhanMeng_ZhiWu_Not_Config = -1005;

		public const int Error_ZhanMeng_Not_Allowed_Change_LuoLanChengZhu = -1006;

		public const int Error_ZhanMeng_Not_Allowed_Change_ShengYuChengZhu = -1007;

		public const int Error_ZhanMeng_Level_Limit = -1008;

		public const int Error_ZhanMeng_Enter_Too_Late = -1009;

		public const int Error_ZhanMeng_Has_In_ZhanMeng = -1010;

		public const int Error_ZhanMeng_Duplicate_Name = -1011;

		public const int Error_ZhanMeng_Has_In_JunTuan = -1020;

		public const int Error_ZhanMeng_Has_In_Request = -1021;

		public const int Error_JunTuan_Not_Exist = -1022;

		public const int Error_JunTuan_Name_Has_Exist = -1023;

		public const int Error_JunTuan_ShouLing_Only = -1024;

		public const int Error_ZhanMeng_Not_In_JunTuan = -1025;

		public const int Error_JunTuan_Changed = -1026;

		public const int Error_JunTuan_Name_Invalid = -1027;

		public const int Error_Task_Not_Complete = -1028;

		public const int Error_Task_Not_Exists = -1029;

		public const int Error_JunTuan_Point_Limit = -1030;

		public const int Error_JunTuan_BangHui_Count_Limit = -1031;

		public const int Error_JunTuan_Has_LingDi = -1032;

		public const int Error_HongBao_Need_In_ZhanMeng = -1033;

		public const int Error_JunTuan_JingJing_Too_Much = -1034;

		public const int Error_GVoice_Role_Too_Much = -1035;

		public const int Error_ZhanMeng_BHMatch_Join = -1036;

		public const int Error_ZhanMeng_KuaFuLueDuo_Join = -1037;

		public const int Error_KuaFuLueDuo_Bid_Faild = -1038;

		public const int Error_KuaFuLueDuo_Server_Not_In_Group = -1039;

		public const int Error_KuaFuLueDuo_Server_Not_Exists = -1040;

		public const int Error_ZhanMeng_ZiJin_Not_Enough = -1041;

		public const int Error_KuaFuLueDuo_Server_Not_In_Round = -1042;

		public const int Error_KuaFuLueDuo_Bid_Money_Too_Little = -1043;

		public const int Error_KuaFuLueDuo_GameOver = -1044;

		public const int Error_KuaFuLueDuo_RoleNum_Reach_Limit = -1045;

		public const int Error_KuaFuLueDuo_No_Attack = -1046;

		public const int Error_Not_In_valid_Time = -2001;

		public const int Error_Denied_In_Activity_Time = -2002;

		public const int Error_Time_Over = -2003;

		public const int Error_Time_Punish = -2004;

		public const int Error_Operate_Too_Fast = -2005;

		public const int Error_In_ZhanMeng_Time_Not_Enough = -2006;

		public const int Error_In_ZhanMeng_ShenQingCDTime_Not_Enough = -2007;

		public const int Error_FightTime_Cannot_Enter = -2008;

		public const int Error_Is_Not_LuoLanChengZhu = -3001;

		public const int Error_Is_Not_Married = -3002;

		public const int Error_Is_Not_BHMatchChampion = -3003;

		public const int Error_KuaFuFuBenNotExist = -4000;

		public const int Error_HasInQueue = -4001;

		public const int Error_HasInKuaFuFuBen = -4002;

		public const int Error_Invalid_GameType = -4003;

		public const int Error_Reach_Max_Level = -4004;

		public const int Error_ZhanMeng_Has_SignUp = -4005;

		public const int Error_Game_Over = -4006;

		public const int Error_Game_Not_Start = -4007;

		public const int Error_ZhanMeng_Not_SignUp = -4008;

		public const int Error_ZhanMeng_Bye = -4009;

		public const int Error_Invalid_Name = -4011;

		public const int Error_HasZhanDui = -4012;

		public const int HasNoTeam = -4013;

		public const int HasInZhanDui = -4014;

		public const int InKuaFuWating = -4015;

		public const int NotLeader = -4016;

		public const int NotTeamMember = -4017;

		public const int TimeErr1 = -4018;

		public const int InFuben = -4019;

		public const int OffLine = -4020;

		public const int Busy = -4021;

		public const int ZhanDuiTimeLimit = -402;

		public const int Error_Name_Has_Exists = -4023;

		public const int HasInOtherTeam = -4024;

		public const int Error_ZhanDui_Max_Role_Count = -4028;

		public const int Error_ZhanDui_Leader_Denied = -4029;

		public const int Error_RoleName_NotFind = -4030;

		public const int Error_ZhanDui_Not_Exists = -4031;

		public const int Error_ZhanDui_Has_Support = -4032;

		public const int Error_ZhanDui_Support_NumLimit = -4033;

		public const int Error_ZhanDui_Has_SignUp = -4034;

		public const int Error_ZhanDui_Not_SignUp = -4035;

		public const int Error_ZhanDui_Bye = -4036;

		public const int Error_ZhanDui_Has_InGame = -4037;

		public const int Error_ZhanDui_Leader_Leave = -4038;

		public const int Error_DB_TimeOut = -10000;

		public const int Error_Server_Busy = -11000;

		public const int Error_Server_Not_Registed = -11001;

		public const int Error_Duplicate_Key_ServerId = -11002;

		public const int Error_Server_Internal_Error = -11003;

		public const int Error_Not_Implement = -11004;

		public const int Error_Connection_Disabled = -11005;

		public const int Error_Connection_Closing = -11006;

		public const int Error_Redirect_Orignal_Server = -11007;

		public const int Error_Token_Expired = -100;

		public const int Error_Reborn_BagNum_Not_Enough = -101;

		public const int Error_Version_Not_Match = -2;

		public const int Error_Server_Connections_Limit = -100;

		public const int Error_Version_Not_Match2 = -3;

		public const int Error_Token_Expired2 = -1;

		public const int Error_Connection_Closing2 = -2;

		public const int Error_Operation_Denied_SameLine = -4011;

		public const int Error_ZhanDui_JieSuan = -4025;

		public const int Error_ZhanDui_Player_Not_Enough = -4026;

		public const int Error_Invalid_Name_ = -4027;
	}
}
