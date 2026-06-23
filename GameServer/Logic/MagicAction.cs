using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.Building;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.NewBufferExt;
using GameServer.Logic.Talent;
using GameServer.Logic.TuJian;
using GameServer.Logic.YueKa;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	internal class MagicAction
	{
		private static bool ProcessActionSeveralTimes(IObject self, IObject obj, MagicActionIDs id, double[] actionParams, int binding, int actionGoodsID, bool bIsVerify, int timesNum)
		{
			bool flag = true;
			switch (id)
			{
			case MagicActionIDs.NEW_ADD_CHENGJIU:
				if (self is GameClient)
				{
					int modifyValue = (int)actionParams[0] * timesNum;
					ChengJiuManager.AddChengJiuPoints(self as GameClient, "脚本增加成就", modifyValue, true, true);
				}
				break;
			case MagicActionIDs.ADD_SHENGWANG:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int num = (int)actionParams[0] * timesNum;
					if (num > 0)
					{
						GameManager.ClientMgr.ModifyShengWangValue(gameClient, num, "脚本增加声望", true, true);
						string textMsg = string.Format(GLang.GetLang(429, new object[0]), gameClient.ClientData.RoleName, num);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, textMsg);
					}
				}
				break;
			case MagicActionIDs.ADD_XINGHUN:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int num = (int)actionParams[0] * timesNum;
					if (num > 0)
					{
						GameManager.ClientMgr.ModifyStarSoulValue(gameClient, num, "脚本增加星魂", true, true);
						string textMsg = string.Format(GLang.GetLang(430, new object[0]), gameClient.ClientData.RoleName, num);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, textMsg);
					}
				}
				break;
			case MagicActionIDs.NEW_PACK_JINGYUAN:
				if (self is GameClient)
				{
					int addValue = (int)actionParams[0] * timesNum;
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(self as GameClient, addValue, "脚本增加魔晶", true, true, false);
				}
				break;
			case MagicActionIDs.ADDYSFM:
			{
				int num2 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyYuanSuFenMoValue(self as GameClient, num2, "道具ADDYSFM", true, false);
				break;
			}
			case MagicActionIDs.ADD_LINGJING:
			{
				int num2 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyMUMoHeValue(self as GameClient, num2, "道具ADD_LINGJING", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_ZAIZAO:
			{
				int num2 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyZaiZaoValue(self as GameClient, num2, "道具ADD_ZAIZAO", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_RONGYAO:
			{
				int num2 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyTianTiRongYaoValue(self as GameClient, num2, "ADD_RONGYAO", true);
				break;
			}
			case MagicActionIDs.ADD_GUARDPOINT:
			{
				int point = (int)actionParams[0] * timesNum;
				SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(self as GameClient, point, "道具脚本");
				break;
			}
			case MagicActionIDs.NEW_ADD_YINGGUANG:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int minV = (int)actionParams[0];
					int num3 = (int)actionParams[1];
					int num = 0;
					for (int i = 0; i < timesNum; i++)
					{
						num += Global.GetRandomNumber(minV, num3 + 1);
					}
					GameManager.FluorescentGemMgr.AddFluorescentPoint(gameClient, num, "使用物品获得", false);
					GameManager.ClientMgr.NotifySelfParamsValueChange(gameClient, RoleCommonUseIntParamsIndexs.FluorescentGem, gameClient.ClientData.FluorescentPoint);
				}
				break;
			case MagicActionIDs.ADD_BANGGONG:
			{
				int num2 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.AddBangGong(self as GameClient, ref num2, AddBangGongTypes.ADD_BANGGONG, 0);
				break;
			}
			case MagicActionIDs.ADD_ZHANMENGGAIMING:
			{
				GameClient gameClient = self as GameClient;
				if (gameClient != null)
				{
					int num2 = (int)actionParams[0] * timesNum;
					int num4 = Global.sendToDB<int, string>(14009, string.Format("{0}:{1}", self.GetObjectID(), num2), gameClient.ServerId);
					if (num4 > 0)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟改名次数", "ADD_ZHANMENGGAIMING", "系统", gameClient.ClientData.RoleName, "修改", num2, gameClient.ClientData.ZoneID, gameClient.strUserID, num4, gameClient.ServerId, null);
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(gameClient.ClientData.RoleID, gameClient.ClientData.Faction, gameClient.ServerId);
						bangHuiDetailData.CanModNameTimes = num4;
					}
					else
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟改名次数", "ADD_ZHANMENGGAIMING", "系统", gameClient.ClientData.RoleName, "修改", 0, gameClient.ClientData.ZoneID, gameClient.strUserID, num4, gameClient.ServerId, null);
					}
				}
				break;
			}
			case MagicActionIDs.ADD_LANGHUN:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyLangHunFenMoValue(self as GameClient, num5, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_ZHENGBADIANSHU:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyZhengBaPointValue(self as GameClient, num5, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_WANGZHEDIANSHU:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyKingOfBattlePointValue(self as GameClient, num5, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_MEILIDIANSHU:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyOrnamentCharmPointValue(self as GameClient, num5, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_SHENLIJINGHUA:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(self as GameClient, num5, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.NEW_ADD_MONEY:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0] * timesNum, "脚本添加绑金", true);
				}
				break;
			case MagicActionIDs.NEW_ADD_YINLIANG:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0] * timesNum, "脚本增加金币一", false);
				}
				break;
			case MagicActionIDs.ADD_DJ:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int num6 = (int)actionParams[0] * timesNum;
					if (num6 >= 0)
					{
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num6), "ADD_DJ公式", ActivityTypes.None, "");
					}
					else
					{
						GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num6), "ADD_DJ公式", true, true, false, DaiBiSySType.None);
					}
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取元宝, roleID={0}({1}), UserMoney={2}, newUserMoney={3}", new object[]
					{
						gameClient.ClientData.RoleID,
						gameClient.ClientData.RoleName,
						gameClient.ClientData.UserMoney,
						num6
					}), EventLevels.Record);
				}
				break;
			case MagicActionIDs.ADD_BINDYUANBAO:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int num7 = (int)actionParams[0] * timesNum;
					if (num7 >= 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num7), "ADD_BINDYUANBAO");
					}
					else
					{
						GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Math.Abs(num7), "ADD_BINDYUANBAO", false);
					}
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金币, roleID={0}({1}), Gold={2}, newGold={3}", new object[]
					{
						gameClient.ClientData.RoleID,
						gameClient.ClientData.RoleName,
						gameClient.ClientData.Gold,
						num7
					}), EventLevels.Record);
				}
				break;
			case MagicActionIDs.ADD_GOODWILL:
				if (self is GameClient)
				{
					GameClient gameClient = self as GameClient;
					int num = (int)actionParams[0] * timesNum;
					if (num > 0)
					{
						if (bIsVerify)
						{
							flag = MarriageOtherLogic.getInstance().CanAddMarriageGoodWill(gameClient);
							if (!flag)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(67, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							MarriageOtherLogic.getInstance().UpdateMarriageGoodWill(gameClient, num);
						}
					}
				}
				break;
			case MagicActionIDs.FALL_BAOXIANG:
			case MagicActionIDs.FALL_BAOXIANG_2:
				GoodsBaoXiang.ProcessFallBaoXiang_StepTwo(self as GameClient, (int)actionParams[0], (int)actionParams[1], binding, actionGoodsID);
				break;
			case MagicActionIDs.MU_RANDOMSHIZHUANG:
				FashionManager.getInstance().FashionActiveByMagic(self as GameClient, actionParams);
				break;
			case MagicActionIDs.ADD_LINGDICAIJI_COUNT:
			{
				GameClient gameClient = self as GameClient;
				if (gameClient.ClientData.LingDiCaiJiNum > LingDiCaiJiManager.WeeklyNum)
				{
					gameClient.ClientData.LingDiCaiJiNum = LingDiCaiJiManager.WeeklyNum;
				}
				gameClient.ClientData.LingDiCaiJiNum -= Convert.ToInt32(actionParams[0] * (double)timesNum);
				Global.SaveRoleParamsInt32ValueToDB(gameClient, "10158", gameClient.ClientData.LingDiCaiJiNum, true);
				if (LingDiCaiJiManager.getInstance().GetLingDiType(gameClient.ClientData.MapCode) != 2)
				{
					gameClient.sendCmd(1828, (LingDiCaiJiManager.WeeklyNum - gameClient.ClientData.LingDiCaiJiNum).ToString(), false);
				}
				break;
			}
			case MagicActionIDs.ADD_NENGLIANG:
			{
				int addType = (int)actionParams[0];
				int addValue = (int)actionParams[1] * timesNum;
				BuildingManager.getInstance().ModifyNengLiangPointsValue(self as GameClient, addType, addValue, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_JINGLINGSHENJI:
			{
				int addValue = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyShenJiJiFenValue(self as GameClient, addValue, "道具脚本", true, true);
				break;
			}
			case MagicActionIDs.ADD_FUWENZHICHEN:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(self as GameClient, num5, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_JUEXINGZHICHEN:
			{
				int num5 = (int)actionParams[0] * timesNum;
				GameManager.ClientMgr.ModifyJueXingZhiChenValue(self as GameClient, num5, "道具脚本", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_JUEXING:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyJueXingValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), false);
				}
				break;
			case MagicActionIDs.ADD_HUNJING:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyHunJingValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_MOBI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyMoBiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), false);
				}
				break;
			case MagicActionIDs.ADD_JINGLINGJUEXINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyYuanSuJueXingShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_FUMOLINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyFuMoLingShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_FENYINJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornFengYinJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_CHONGSHENGJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornChongShengJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_XUANCAIJINGSHI:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornXuanCaiJinShiValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP1:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornCuiLianPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP2:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornDuanZaoPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_REBORNEQUIP3:
				if (obj is GameClient)
				{
					GameManager.ClientMgr.ModifyRebornNiePanPointValue(obj as GameClient, (int)actionParams[0] * timesNum, id.ToString(), true, true, false);
				}
				break;
			case MagicActionIDs.ADD_ZHANDUIRONGYAO:
			{
				int num5 = (int)actionParams[0] * timesNum;
				if (num5 < 2147483647)
				{
					GameManager.ClientMgr.ModifyTeamRongYaoValue(self as GameClient, num5, "道具脚本", false);
				}
				break;
			}
			case MagicActionIDs.ADD_TEAMPOINT:
			{
				int num5 = (int)actionParams[0] * timesNum;
				if (num5 < 2147483647)
				{
					GameManager.ClientMgr.ModifyTeamPointValue(self as GameClient, num5, "道具脚本", false);
				}
				break;
			}
			case MagicActionIDs.ADD_LUCKSTAR_MOJING:
			{
				int addValue = Math.Abs((int)actionParams[0] * timesNum);
				GameManager.ClientMgr.ModifyLuckStarValue(self as GameClient, addValue, "道具脚本", false, DaiBiSySType.None);
				GameManager.ClientMgr.ModifyTianDiJingYuanValue(self as GameClient, addValue, "脚本增加魔晶", true, true, false);
				break;
			}
			case MagicActionIDs.ADD_LUCKSTAR:
			{
				int addValue = Math.Abs((int)actionParams[0] * timesNum);
				GameManager.ClientMgr.ModifyLuckStarValue(self as GameClient, addValue, "道具脚本", false, DaiBiSySType.None);
				break;
			}
			}
			return flag;
		}

		public static bool ProcessAction(IObject self, IObject obj, MagicActionIDs id, double[] actionParams, int targetX = -1, int targetY = -1, int usedMaigcV = 0, int skillLevel = 1, int skillid = -1, int npcID = 0, int binding = 0, int direction = -1, int actionGoodsID = 0, bool bItemAddVal = false, bool bIsVerify = false, double manyRangeInjuredPercent = 1.0, int timesNum = 1, double shenShiInjurePercent = 0.0)
		{
			if (MagicAction.MaxHitNum == 0)
			{
				MagicAction.MaxHitNum = 8;
			}
			skillLevel = Global.GMin(Global.MaxSkillLevel, skillLevel);
			skillLevel = Global.GMax(0, skillLevel - 1);
			if (self is GameClient)
			{
				GameClient gameClient = self as GameClient;
				skillLevel += TalentManager.GetSkillLevel(gameClient, skillid);
			}
			bool result;
			if (id > MagicActionIDs.ActionSeveralTimesBegin && id < MagicActionIDs.ActionSeveralTimesEnd)
			{
				result = MagicAction.ProcessActionSeveralTimes(self, obj, id, actionParams, binding, actionGoodsID, bIsVerify, timesNum);
			}
			else
			{
				bool flag = true;
				switch (id)
				{
				case MagicActionIDs.FOREVER_ADDHIT:
					if (obj is GameClient)
					{
						double num = actionParams[skillLevel];
						(obj as GameClient).RoleBuffer.AddForeverExtProp(18, num);
					}
					break;
				case MagicActionIDs.RANDOM_ADDATTACK1:
				{
					double num2 = actionParams[skillLevel * 2];
					double num = actionParams[skillLevel * 2 + 1];
					int num3 = (int)(100.0 * num2);
					if (Global.GetRandomNumber(0, 101) < num3)
					{
						int index = 8;
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						if (1 == num4)
						{
							index = 10;
						}
						else if (2 == num4)
						{
						}
						(obj as GameClient).RoleOnceBuffer.AddTempExtProp(index, num, 0L);
					}
					else
					{
						flag = false;
					}
					break;
				}
				case MagicActionIDs.RANDOM_ADDATTACK2:
					if (self is GameClient)
					{
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						double num6 = actionParams[skillLevel * 2];
						double num7 = actionParams[skillLevel * 2 + 1];
						double num8 = (double)Global.GetRandomNumber((int)(num6 * 10.0), (int)(num7 * 10.0)) / 10.0;
						num8 = 1.0 + num8;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int num5 = (self as Monster).MonsterInfo.AttackType;
						double num6 = actionParams[2];
						double num7 = actionParams[3];
						double num8 = (double)Global.GetRandomNumber((int)(num6 * 10.0), (int)(num7 * 10.0)) / 10.0;
						num8 = 1.0 + num8;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
							}
						}
					}
					break;
				case MagicActionIDs.ATTACK_STRAIGHT:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					Point currentGrid = self.CurrentGrid;
					Point currentGrid2 = obj.CurrentGrid;
					Point gridPointByDirection = Global.GetGridPointByDirection(direction, (int)currentGrid.X, (int)currentGrid.Y);
					double num8 = actionParams[skillLevel];
					bool flag2 = gridPointByDirection.X != currentGrid2.X || gridPointByDirection.Y != currentGrid2.Y;
					if (!flag2)
					{
						num8 = 1.0;
					}
					if (self is GameClient)
					{
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int num5 = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (!(obj is JunQiItem))
							{
								if (obj is FakeRoleItem)
								{
								}
							}
						}
					}
					break;
				}
				case MagicActionIDs.ATTACK_FRONT:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					Point currentPos = self.CurrentPos;
					Point currentPos2 = obj.CurrentPos;
					int num9 = (int)Global.GetDirectionByTan(currentPos2.X, currentPos2.Y, currentPos.X, currentPos.Y);
					double num8 = actionParams[skillLevel];
					num8 = 0.5 * num8;
					bool flag2 = num9 != direction;
					if (!flag2)
					{
						num8 = 1.0;
					}
					if (self is GameClient)
					{
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int num5 = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, flag2, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.PUSH_STRAIGHT:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						int num10 = 2 + skillLevel;
						int num11 = num10;
						Point point = (self as GameClient).CurrentGrid;
						List<Point> gridPointByDirection2 = Global.GetGridPointByDirection(direction, (int)point.X, (int)point.Y, num10);
						double num12 = actionParams[skillLevel];
						byte b = 0;
						b |= 1;
						b |= 2;
						for (int i = 0; i < gridPointByDirection2.Count; i++)
						{
							if (Global.InObsByGridXY((self as GameClient).ObjectType, (self as GameClient).ClientData.MapCode, (int)gridPointByDirection2[i].X, (int)gridPointByDirection2[i].Y, 0, b))
							{
								break;
							}
							num10--;
						}
						if (num10 < num11)
						{
							point = gridPointByDirection2[num11 - num10 - 1];
						}
						Point point2 = point;
						if (!Global.CanQueueMoveObject(self as GameClient, direction, (int)point.X, (int)point.Y, 20, num10, b, out point2, false))
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
							Point point3 = new Point(point2.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), point2.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)point3.X, (int)point3.Y, (self as GameClient).ClientData.RoleDirection, 159, 3);
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, self as GameClient, 0, (int)num12, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
							Point point3 = new Point(gridPointByDirection2[gridPointByDirection2.Count - 1].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), gridPointByDirection2[gridPointByDirection2.Count - 1].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
							GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)point3.X, (int)point3.Y, (self as GameClient).ClientData.RoleDirection, 159, 3);
							if (num10 > 0)
							{
								Global.QueueMoveObject(self as GameClient, (self as GameClient).ClientData.RoleDirection, (int)point.X, (int)point.Y, 20, num10, (int)num12, b, false);
							}
						}
					}
					break;
				case MagicActionIDs.PUSH_CIRCLE:
					if (self is GameClient)
					{
						Point point = (self as GameClient).CurrentGrid;
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						double num12 = actionParams[skillLevel];
						GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
						byte b = 0;
						b |= 1;
						b |= 2;
						obj = null;
						for (int j = 0; j < 8; j++)
						{
							Global.QueueMoveObject(self as GameClient, j, (int)point.X, (int)point.Y, 20, 1, (int)num12, b, false);
						}
					}
					break;
				case MagicActionIDs.MAGIC_ATTACK:
				case MagicActionIDs.DS_ATTACK:
				case MagicActionIDs.PHY_ATTACK:
				{
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					double num13 = actionParams[skillLevel * 2];
					double num14 = actionParams[skillLevel * 2 + 1];
					double num15 = (double)Global.GetRandomNumber((int)num13, (int)num14 + 1);
					if (self is GameClient)
					{
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, (int)num15, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, (int)num15, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else
					{
						int num5 = (self as Monster).MonsterInfo.AttackType;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, (int)num15, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, (int)num15, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
							}
						}
					}
					break;
				}
				case MagicActionIDs.RANDOM_MOVE:
					if (self is GameClient)
					{
						double num16 = actionParams[skillLevel];
						int num3 = (int)(num16 * 100.0);
						if (Global.GetRandomNumber(0, 101) >= num3)
						{
							if (Global.GetRandomNumber(0, 101) >= 10)
							{
								Point randomPoint = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode, (int)randomPoint.X, (int)randomPoint.Y, 0, 0))
								{
									List<object> list = Global.GetAll9Clients(self as GameClient);
									GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, list);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)randomPoint.X, (int)randomPoint.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else
							{
								int num17 = GameManager.MainMapCode;
								int maxX = -1;
								int mapY = -1;
								GameMap gameMap = null;
								if (GameManager.MapMgr.DictMaps.TryGetValue(num17, out gameMap))
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num17, maxX, mapY, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.FIRE_WALL:
					if (self is GameClient)
					{
						double num8 = actionParams[2 + skillLevel];
						double[] array = new double[]
						{
							actionParams[0],
							actionParams[0] / actionParams[1],
							num8,
							(double)(self as GameClient).ClientData.RoleID
						};
						GameMap gameMap = GameManager.MapMgr.DictMaps[(self as GameClient).ClientData.MapCode];
						int num18 = targetX / gameMap.MapGridWidth;
						int num19 = targetY / gameMap.MapGridHeight;
						if (num18 > 0 && num19 > 0)
						{
							GameManager.GridMagicHelperMgr.AddMagicHelper(MagicActionIDs.FIRE_WALL, array, (self as GameClient).ClientData.MapCode, new Point((double)num18, (double)num19), 1, 1, self.CurrentCopyMapID);
						}
					}
					break;
				case MagicActionIDs.FIRE_CIRCLE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						int num5 = num4;
						double num8 = actionParams[skillLevel];
						if (!(obj is GameClient))
						{
							if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, num8, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.NEW_MAGIC_SUBINJURE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						double num20 = actionParams[skillLevel * 2];
						double num21 = actionParams[skillLevel * 2 + 1];
						int num22 = 0;
						int burst = 0;
						if (obj is GameClient)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 1, 0, out burst, out num22, true, 0.0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 1, 0, out burst, out num22, true, 0.0, 0, 0, 0.0);
						}
						num20 += (double)num22;
						double[] array2 = new double[]
						{
							num21,
							num20
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_MAGIC_SUBINJURE, array2, -1);
						(self as GameClient).ClientData.FSHuDunStart = TimeUtil.NOW();
						(self as GameClient).ClientData.FSHuDunSeconds = (int)num20;
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 0, (self as GameClient).ClientData.FSHuDunStart, (self as GameClient).ClientData.FSHuDunSeconds, 0.0);
						double[] array = new double[]
						{
							num20
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.FSAddHuDunNoShow, array, 1, true);
					}
					break;
				case MagicActionIDs.DS_ADDLIFE:
					if (self is GameClient)
					{
						double num23 = actionParams[0];
						double num24 = actionParams[1];
						double num25 = actionParams[2 + skillLevel];
						if (obj is GameClient)
						{
							double[] array = new double[]
							{
								num23,
								num24,
								num25
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.DSTimeAddLifeNoShow, array, 1, true);
						}
						else if (obj is Monster)
						{
							double[] array = new double[]
							{
								num23,
								num24,
								num25
							};
							Global.UpdateMonsterBufferData(obj as Monster, BufferItemTypes.DSTimeAddLifeNoShow, array);
						}
					}
					break;
				case MagicActionIDs.DS_CALL_GUARD:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num26 = (int)actionParams[0];
						int survivalTime = (int)actionParams[1];
						Monster monster = Global.GetPetMonsterByMonsterByType(gameClient, MonsterTypes.DSPetMonster);
						if (monster != null && monster.Alive && monster.MonsterInfo.ExtensionID == num26)
						{
							Global.RecalcDSMonsterProps(gameClient, monster, skillLevel, survivalTime);
							Point currentPos3 = gameClient.CurrentPos;
							GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, (int)currentPos3.X, (int)currentPos3.Y, (int)monster.Direction, 159, 0);
						}
						else
						{
							Global.SystemKillSummonMonster(gameClient, MonsterTypes.DSPetMonster);
							GameManager.LuaMgr.CallMonstersForGameClient(gameClient, num26, skillLevel, survivalTime, 1001, 1);
						}
					}
					else if (self is Monster)
					{
						Monster monster2 = self as Monster;
						int num26 = (int)actionParams[0];
						int survivalTime = (int)actionParams[1];
						Monster monster = monster2.CallMonster;
						if (monster != null && monster.Alive && monster.MonsterInfo.ExtensionID == num26)
						{
							Global.RecalcDSMonsterProps(monster2, monster, skillLevel, survivalTime);
							Point currentPos3 = monster2.CurrentPos;
							GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, (int)currentPos3.X, (int)currentPos3.Y, (int)monster.Direction, 159, 0);
						}
						else
						{
							Global.SystemKillMonster(monster);
							GameManager.LuaMgr.CallMonstersForMonster(monster2, num26, skillLevel, survivalTime, 1001, 1);
						}
					}
					break;
				case MagicActionIDs.TIME_DS_ADD_DEFENSE:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.TIME_DS_ADD_MDEFENSE:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.TIME_DS_SUB_DEFENSE:
					if (self is GameClient)
					{
						direction = ((direction < 0) ? (self as GameClient).ClientData.RoleDirection : direction);
						int num4 = Global.CalcOriginalOccupationID(self as GameClient);
						long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[skillLevel * 2] * 1000L * 10000L;
						double num28 = actionParams[skillLevel * 2 + 1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, -num28, num27);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, -num28, num27);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, -num28, num27);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, -num28, num27);
						}
						else if (obj is Monster)
						{
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK1:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC1:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_ATTACK2LIFE:
					if (self is GameClient)
					{
						int num29 = 0;
						if (obj is GameClient)
						{
							num29 = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							num29 = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						if (num29 > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double lifeV = (double)num29 * (actionParams[1] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, lifeV, "击中恢复， 脚本" + id.ToString());
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.INSTANT_MAGIC2LIFE:
					if (self is GameClient)
					{
						int num29 = 0;
						if (obj is GameClient)
						{
							num29 = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							num29 = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, (int)actionParams[0], 1.0, 0, 0, 1.0, 0, 0);
						}
						if (num29 > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double lifeV = (double)num29 * (actionParams[1] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, lifeV, "击中恢复， 脚本" + id.ToString());
						}
					}
					break;
				case MagicActionIDs.TIME_ATTACK:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ATTACK, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_MAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_MAGIC, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.FOREVER_ADDATTACK:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(7, actionParams[0]);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(8, actionParams[0]);
					break;
				case MagicActionIDs.FOREVER_ADDMAGICATTACK:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(9, actionParams[0]);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(10, actionParams[0]);
					break;
				case MagicActionIDs.TIME_ADDATTACK:
				{
					long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(7, actionParams[0] / 100.0, num27);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(8, actionParams[0] / 100.0, num27);
					break;
				}
				case MagicActionIDs.TIME_SUBATTACK:
				{
					long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(7, -(actionParams[0] / 100.0), num27);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(8, -(actionParams[0] / 100.0), num27);
					break;
				}
				case MagicActionIDs.TIME_ADDMAGIC:
				{
					long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(9, actionParams[0] / 100.0, num27);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(10, actionParams[0] / 100.0, num27);
					break;
				}
				case MagicActionIDs.TIME_SUBMAGIC:
				{
					long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(9, -(actionParams[0] / 100.0), num27);
					(obj as GameClient).RoleMultipliedBuffer.AddTempExtProp(10, -(actionParams[0] / 100.0), num27);
					break;
				}
				case MagicActionIDs.INSTANT_ADDLIFE1:
					if (obj is GameClient)
					{
						double num30 = actionParams[0] * (1.0 + RoleAlgorithm.GetPotionPercentV(obj as GameClient));
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num30, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDMAGIC1:
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							double num30 = actionParams[0] * (1.0 + RoleAlgorithm.GetPotionPercentV(obj as GameClient));
							GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num30, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE2:
					if (obj is GameClient)
					{
						double num31 = actionParams[0] / 100.0 * (double)(obj as GameClient).ClientData.LifeV;
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num31, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDMAGIC2:
					if (obj is GameClient)
					{
						double num31 = actionParams[0] / 100.0 * (double)(obj as GameClient).ClientData.MagicV;
						GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num31, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE3:
					if (obj is GameClient)
					{
						double num31 = (double)usedMaigcV + actionParams[0];
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num31, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_ADDLIFE4:
					if (obj is GameClient)
					{
						double num31 = (double)usedMaigcV * (actionParams[0] / 100.0);
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num31, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.INSTANT_COOLDOWN:
					if (self is GameClient)
					{
						GameManager.ClientMgr.RemoveCoolDown(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 0, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.TIME_SUBLIFE:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBLIFE, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_ADDLIFE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDLIFE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_SLOW:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SLOW, actionParams, -1);
					if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOthersMyAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (obj as GameClient).ClientData.RoleID, (obj as GameClient).ClientData.MapCode, (obj as GameClient).ClientData.RoleDirection, 0, (obj as GameClient).ClientData.PosX, (obj as GameClient).ClientData.PosY, -1, -1, -1, 0, 0, 114);
					}
					break;
				case MagicActionIDs.TIME_ADDDODGE:
				{
					long num27 = TimeUtil.NOW() * 10000L + (long)actionParams[1] * 1000L * 10000L;
					(obj as GameClient).RoleBuffer.AddTempExtProp(19, actionParams[0] / 100.0, num27);
					break;
				}
				case MagicActionIDs.TIME_FREEZE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_FREEZE, actionParams, -1);
					if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOthersMyAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (obj as GameClient).ClientData.RoleID, (obj as GameClient).ClientData.MapCode, (obj as GameClient).ClientData.RoleDirection, 0, (obj as GameClient).ClientData.PosX, (obj as GameClient).ClientData.PosY, -1, -1, -1, 0, 0, 114);
					}
					break;
				case MagicActionIDs.TIME_INJUE2LIFE:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_INJUE2LIFE, actionParams, -1);
					break;
				case MagicActionIDs.INSTANT_BURSTATTACK:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double num32 = (double)(obj as GameClient).ClientData.CurrentLifeV / (double)(obj as GameClient).ClientData.LifeV;
							double num33 = actionParams[1] / 100.0;
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, actionParams[0] / 100.0, 0, num32 <= num33, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							double num32 = (obj as Monster).VLife / (obj as Monster).MonsterInfo.VLifeMax;
							double num33 = actionParams[1] / 100.0;
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, actionParams[0] / 100.0, 0, num32 <= num33, 0, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							double num32 = (double)(obj as BiaoCheItem).CutLifeV / (double)(obj as BiaoCheItem).LifeV;
							double num33 = actionParams[1] / 100.0;
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, actionParams[0] / 100.0, 0, num32 <= num33, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							double num32 = (double)(obj as JunQiItem).CutLifeV / (double)(obj as JunQiItem).LifeV;
							double num33 = actionParams[1] / 100.0;
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, actionParams[0] / 100.0, 0, num32 <= num33, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							double num32 = (double)(obj as FakeRoleItem).CurrentLifeV / (double)(obj as FakeRoleItem).MyRoleDataMini.MaxLifeV;
							double num33 = actionParams[1] / 100.0;
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, actionParams[0] / 100.0, 0, num32 <= num33, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.FOREVER_ADDDRUGEFFECT:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.FOREVER_ADDDRUGEFFECT, actionParams, -1);
					break;
				case MagicActionIDs.INSTANT_REMOVESLOW:
					(obj as GameClient).RoleMagicHelper.RemoveMagicHelper(MagicActionIDs.TIME_SLOW);
					break;
				case MagicActionIDs.TIME_SUBINJUE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBINJUE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_ADDINJUE:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDINJUE, actionParams, -1);
					break;
				case MagicActionIDs.TIME_SUBINJUE1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_SUBINJUE1, actionParams, -1);
					break;
				case MagicActionIDs.TIME_ADDINJUE1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDINJUE1, actionParams, -1);
					break;
				case MagicActionIDs.TIME_DELAYATTACK:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_DELAYATTACK, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.TIME_DELAYMAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_DELAYMAGIC, actionParams, obj.GetObjectID());
					break;
				case MagicActionIDs.FOREVER_ADDDODGE:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(19, actionParams[0] / 100.0);
					break;
				case MagicActionIDs.TIME_INJUE2MAGIC:
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_INJUE2MAGIC, actionParams, -1);
					break;
				case MagicActionIDs.FOREVER_ADDMAGICV:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(15, actionParams[0]);
					break;
				case MagicActionIDs.FOREVER_ADDLIFE:
					(obj as GameClient).RoleBuffer.AddForeverExtProp(13, actionParams[0]);
					break;
				case MagicActionIDs.INSTANT_MOVE:
					if (self is GameClient)
					{
						if (self != obj)
						{
							if (obj is GameClient)
							{
								Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point extensionPointByObs = new Point((double)(obj as GameClient).ClientData.PosX, (double)(obj as GameClient).ClientData.PosY);
								if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
								{
									extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is Monster)
							{
								Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point extensionPointByObs = new Point((obj as Monster).SafeCoordinate.X, (obj as Monster).SafeCoordinate.Y);
								if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
								{
									extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is BiaoCheItem)
							{
								Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point extensionPointByObs = new Point((double)(obj as BiaoCheItem).PosX, (double)(obj as BiaoCheItem).PosY);
								if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
								{
									extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is JunQiItem)
							{
								Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point extensionPointByObs = new Point((double)(obj as JunQiItem).PosX, (double)(obj as JunQiItem).PosY);
								if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
								{
									extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
							else if (obj is FakeRoleItem)
							{
								Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
								Point extensionPointByObs = new Point((double)(obj as FakeRoleItem).MyRoleDataMini.PosX, (double)(obj as FakeRoleItem).MyRoleDataMini.PosY);
								if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
								{
									extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
									GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
								}
							}
						}
						else if (targetX != -1 && targetY != -1)
						{
							Point end = new Point((double)(self as GameClient).ClientData.PosX, (double)(self as GameClient).ClientData.PosY);
							Point extensionPointByObs = new Point((double)targetX, (double)targetY);
							if (end.X != extensionPointByObs.X || end.Y != extensionPointByObs.Y)
							{
								extensionPointByObs = Global.GetExtensionPointByObs(self as GameClient, extensionPointByObs, end, Data.MinAttackDistance);
								GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)extensionPointByObs.X, (int)extensionPointByObs.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
							}
						}
					}
					break;
				case MagicActionIDs.TIME_ADDMAGIC1:
					(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.TIME_ADDMAGIC1, actionParams, -1);
					break;
				case MagicActionIDs.GOTO_MAP:
					if (self is GameClient)
					{
						int num17 = (int)actionParams[0];
						GameManager.LuaMgr.GotoMap(self as GameClient, num17, -1, -1, -1);
					}
					break;
				case MagicActionIDs.INSTANT_MAP_POS:
				{
					Point randomPoint = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode);
					if (!Global.InObs(ObjectTypes.OT_CLIENT, (self as GameClient).ClientData.MapCode, (int)randomPoint.X, (int)randomPoint.Y, 0, 0))
					{
						List<object> list = Global.GetAll9Clients(self as GameClient);
						GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, list);
						GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (int)randomPoint.X, (int)randomPoint.Y, (self as GameClient).ClientData.RoleDirection, 159, 0);
					}
					break;
				}
				case MagicActionIDs.GOTO_LAST_MAP:
				{
					SceneUIClasses mapSceneType = Global.GetMapSceneType((self as GameClient).ClientData.MapCode);
					PreGotoLastMapEventObject preGotoLastMapEventObject = new PreGotoLastMapEventObject(self as GameClient, mapSceneType);
					GlobalEventSource4Scene.getInstance().fireEvent(preGotoLastMapEventObject, preGotoLastMapEventObject.SceneType);
					if (!preGotoLastMapEventObject.Handled || preGotoLastMapEventObject.Result)
					{
						if (Global.GotoLastMap(self as GameClient, 1))
						{
							if (self is GameClient)
							{
							}
						}
					}
					break;
				}
				case MagicActionIDs.ADD_HORSE:
				{
					int horseID = (int)actionParams[0];
					Global.AddHorseDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, horseID, 1);
					break;
				}
				case MagicActionIDs.ADD_PET:
				{
					int num34 = (int)actionParams[0];
					SystemXmlItem systemXmlItem = null;
					if (GameManager.systemPets.SystemXmlItemDict.TryGetValue(num34, out systemXmlItem))
					{
						string stringValue = systemXmlItem.GetStringValue("Name");
						int petType = 0;
						Global.AddPetDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, num34, stringValue, petType, "");
					}
					break;
				}
				case MagicActionIDs.ADD_PET_GRID:
				{
					int addGridNum = (int)actionParams[0];
					Global.ExtGridPortableBagDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, addGridNum);
					break;
				}
				case MagicActionIDs.ADD_SKILL:
				{
					int num35 = (int)actionParams[0];
					skillLevel = (int)actionParams[1];
					skillLevel = Global.GMax(1, skillLevel);
					skillLevel = Global.GMin(3, skillLevel);
					if (null == Global.GetSkillDataByID(self as GameClient, num35))
					{
						SystemXmlItem systemXmlItem2 = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(num35, out systemXmlItem2))
						{
							if (Global.MU_GetUpSkillLearnCondition(self as GameClient, num35, systemXmlItem2))
							{
								int num4 = Global.CalcOriginalOccupationID(self as GameClient);
								if (num4 == systemXmlItem2.GetIntValue("ToOcuupation", -1))
								{
									Global.AddSkillDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, num35, skillLevel);
									string skillNameByID = Global.GetSkillNameByID(num35);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(362, new object[0]), new object[]
									{
										skillNameByID
									}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
								}
							}
						}
					}
					break;
				}
				case MagicActionIDs.NEW_INSTANT_ATTACK:
					if (self is GameClient)
					{
						double num36 = (double)((int)actionParams[0]);
						double num37 = (double)((int)actionParams[1]);
						double num38 = (double)skillLevel * num37;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, num36, 0, false, (int)num38, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, num36, 0, false, (int)num38, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, num36, 0, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, num36, 0, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, num36, 0, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC:
					if (self is GameClient)
					{
						double num36 = (double)((int)actionParams[0]);
						double num37 = (double)((int)actionParams[1]);
						double num38 = (double)skillLevel * num37;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, num36, 1, false, (int)num38, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, num36, 1, false, (int)num38, 1.0, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, num36, 1, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, num36, 1, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, num36, 1, false, (int)num38, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_FOREVER_ADDATTACK:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(7, num);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(8, num);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDMAGICATTACK:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(9, num);
					(obj as GameClient).RoleBuffer.AddForeverExtProp(10, num);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDHIT:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(18, num);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDDODGE:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(19, num);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDMAGICV:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(15, num);
					break;
				}
				case MagicActionIDs.NEW_FOREVER_ADDLIFE:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					(obj as GameClient).RoleBuffer.AddForeverExtProp(13, num);
					break;
				}
				case MagicActionIDs.NEW_TIME_INJUE2MAGIC:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					double[] array2 = new double[]
					{
						num,
						actionParams[1]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_INJUE2MAGIC, array2, -1);
					break;
				}
				case MagicActionIDs.NEW_TIME_ATTACK:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					double[] array2 = new double[]
					{
						num,
						actionParams[1],
						actionParams[2]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_ATTACK, array2, obj.GetObjectID());
					break;
				}
				case MagicActionIDs.NEW_TIME_MAGIC:
				{
					double num39 = (double)((int)actionParams[0]);
					double num = (double)skillLevel * num39;
					double[] array2 = new double[]
					{
						num,
						actionParams[1],
						actionParams[2]
					};
					(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC, array2, obj.GetObjectID());
					break;
				}
				case MagicActionIDs.NEW_INSTANT_ADDLIFE:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							int num22 = 0;
							int burst = 0;
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, 0.0, 0, 0, 0.0);
							double num39 = (double)((int)actionParams[0]);
							double num = (double)skillLevel * num39 + (double)num22;
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.DB_ADD_DBL_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 18);
					Global.RemoveBufferData(self as GameClient, 36);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] array = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblExperience, array, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_DBL_MONEY:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblMoney, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_DBL_LINGLI:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblLingLi, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_LIFERESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.LifeVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_MAGICRESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MagicVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_LINGLIRESERVE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.LingLiVReserve, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_TEMPATTACK:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AddTempAttack, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_TEMPDEFENSE:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AddTempDefense, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_UPLIEFLIMIT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.UpLifeLimit, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_UPMAGICLIMIT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.UpMagicLimit, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_ADD_LINGLI:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddInterPower(obj as GameClient, (int)actionParams[0], false, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_EXP:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.ProcessRoleExperience(obj as GameClient, (long)((int)actionParams[0]), false, true, false, "none");
					}
					break;
				case MagicActionIDs.NEW_ADD_DAILYCXNUM:
					if (obj is GameClient)
					{
						int num40 = -(int)actionParams[0];
						Global.UpdateDailyJingMaiData(obj as GameClient, num40);
					}
					break;
				case MagicActionIDs.GOTO_NEXTMAP:
					if (obj is GameClient)
					{
						Global.ProcessGoToNextFuBenMap(obj as GameClient);
					}
					break;
				case MagicActionIDs.GET_AWARD:
					if (obj is GameClient)
					{
						Global.ProcessFuBenMapGetAward(obj as GameClient, false);
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ADDLIFE2:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double num41 = (RoleAlgorithm.GetMinMagicAttackV(self as GameClient) + RoleAlgorithm.GetMaxMagicAttackV(self as GameClient)) / 2.0;
							double num39 = (double)((int)actionParams[0]);
							double num = (double)skillLevel * num39 + num41 * (actionParams[0] / 100.0);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, num, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ATTACK3:
					if (self is GameClient)
					{
						double num36 = actionParams[0];
						double num37 = actionParams[1];
						double num42 = (double)skillLevel * num37;
						num36 += num42;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, 0, num36, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, num36, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC3:
					if (self is GameClient)
					{
						double num36 = actionParams[0];
						double num37 = actionParams[1];
						double num42 = (double)skillLevel * num37;
						num36 += num42;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, 0, num36, 0, 0, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, 0, num36, 0, 0, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, num36, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_ATTACK3:
					if (self is GameClient)
					{
						double num43 = actionParams[0];
						double num39 = actionParams[1];
						double num44 = (double)skillLevel * num39;
						num43 += num44;
						int num45 = 0;
						int minAttackV = (int)RoleAlgorithm.GetMinAttackV(self as GameClient);
						int maxAttackV = (int)RoleAlgorithm.GetMaxAttackV(self as GameClient);
						int num46 = (int)RoleAlgorithm.GetLuckV(self as GameClient);
						int num47 = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
						if (obj is GameClient)
						{
							num46 -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
							num47 -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
						}
						int num48 = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minAttackV, maxAttackV, num46, num47, out num45, 0.0);
						num48 = (int)((double)num48 * num43);
						double[] array2 = new double[]
						{
							(double)num48,
							actionParams[1],
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_ATTACK3, array2, obj.GetObjectID());
					}
					break;
				case MagicActionIDs.NEW_TIME_MAGIC3:
					if (self is GameClient)
					{
						double num43 = actionParams[0];
						double num39 = actionParams[1];
						double num44 = (double)skillLevel * num39;
						num43 += num44;
						int num45 = 0;
						int minAttackV2 = (int)RoleAlgorithm.GetMinMagicAttackV(self as GameClient);
						int maxAttackV2 = (int)RoleAlgorithm.GetMaxMagicAttackV(self as GameClient);
						int num46 = (int)RoleAlgorithm.GetLuckV(self as GameClient);
						int num47 = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
						if (obj is GameClient)
						{
							num46 -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
							num47 -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
						}
						int num49 = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minAttackV2, maxAttackV2, num46, num47, out num45, 0.0);
						num49 = (int)((double)num49 * num43);
						double[] array2 = new double[]
						{
							(double)num49,
							actionParams[1],
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC3, array2, obj.GetObjectID());
					}
					break;
				case MagicActionIDs.NEW_INSTANT_ADDLIFE3:
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							double num36 = actionParams[0];
							double num37 = actionParams[1];
							double num42 = (double)skillLevel * num37;
							num36 += num42;
							int num45 = 0;
							int minAttackV2 = (int)RoleAlgorithm.GetMinMagicAttackV(self as GameClient);
							int maxAttackV2 = (int)RoleAlgorithm.GetMaxMagicAttackV(self as GameClient);
							int num46 = (int)RoleAlgorithm.GetLuckV(self as GameClient);
							int num47 = (int)RoleAlgorithm.GetFatalAttack(self as GameClient);
							if (obj is GameClient)
							{
								num46 -= (int)RoleAlgorithm.GetDeLuckyAttack(obj as GameClient);
								num47 -= (int)RoleAlgorithm.GetDeFatalAttack(obj as GameClient);
							}
							int num49 = (int)RoleAlgorithm.CalcAttackValue(self as GameClient, minAttackV2, maxAttackV2, num46, num47, out num45, 0.0);
							num49 = (int)((double)num49 * num36);
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)num49, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_INJUE2MAGIC3:
					if (self is GameClient)
					{
						double num36 = actionParams[0];
						double num37 = actionParams[1];
						double num42 = (double)skillLevel * num37;
						num36 += num42;
						double[] array2 = new double[]
						{
							num36,
							actionParams[2]
						};
						(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_INJUE2MAGIC3, array2, -1);
						(self as GameClient).ClientData.FSHuDunStart = TimeUtil.NOW();
					}
					break;
				case MagicActionIDs.GOTO_WUXING_MAP:
				{
					int num50 = WuXingMapMgr.GetNeedGoodsIDByNPCID((self as GameClient).ClientData.MapCode, npcID - 2130706432);
					if (-1 != num50)
					{
						if (Global.GetTotalGoodsCountByID(self as GameClient, num50) > 0)
						{
							bool flag3 = false;
							bool flag4 = false;
							if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num50, 1, false, out flag3, out flag4, false))
							{
								int nextMapCodeByNPCID = WuXingMapMgr.GetNextMapCodeByNPCID((self as GameClient).ClientData.MapCode, npcID - 2130706432);
								if (-1 != nextMapCodeByNPCID)
								{
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(nextMapCodeByNPCID, out gameMap))
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, nextMapCodeByNPCID, -1, -1, -1, 0);
									}
								}
							}
							else
							{
								string goodsNameByID = Global.GetGoodsNameByID(num50);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(432, new object[0]), new object[]
								{
									goodsNameByID
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							string goodsNameByID = Global.GetGoodsNameByID(num50);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(433, new object[0]), new object[]
							{
								goodsNameByID
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					break;
				}
				case MagicActionIDs.GET_WUXING_AWARD:
					if (obj is GameClient)
					{
						WuXingMapMgr.ProcessWuXingAward(self as GameClient);
					}
					break;
				case MagicActionIDs.LEAVE_LAOFANG:
					if (self is GameClient)
					{
						Global.BroadcastLeaveLaoFangHint(self as GameClient, (self as GameClient).ClientData.MapCode);
						int num17 = GameManager.MainMapCode;
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(num17, out gameMap))
						{
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num17, -1, -1, -1, 0);
						}
					}
					break;
				case MagicActionIDs.GOTO_CAISHENMIAO:
					if (self is GameClient)
					{
						int fuBenID = (int)actionParams[0];
						int num50 = (int)actionParams[1];
						if (-1 != num50)
						{
							if (Global.GetTotalGoodsCountByID(self as GameClient, num50) > 0)
							{
								bool flag3 = false;
								bool flag4 = false;
								if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num50, 1, false, out flag3, out flag4, false))
								{
									Global.EnterCaiShenMiao(self as GameClient, fuBenID, flag3 ? 1 : 0);
								}
								else
								{
									string goodsNameByID = Global.GetGoodsNameByID(num50);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(434, new object[0]), new object[]
									{
										goodsNameByID
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
							else
							{
								string goodsNameByID = Global.GetGoodsNameByID(num50);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(435, new object[0]), new object[]
								{
									goodsNameByID
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 4);
							}
						}
					}
					break;
				case MagicActionIDs.RELOAD_COPYMONSTERS:
					if (self is GameClient)
					{
						if ((self as GameClient).ClientData.CopyMapID > 0)
						{
							int copyMapIDMonstersCount = GameManager.MonsterMgr.GetCopyMapIDMonstersCount((self as GameClient).ClientData.CopyMapID, 0);
							if (copyMapIDMonstersCount <= 0 && !GameManager.MonsterMgr.IsAnyMonsterAliveByCopyMapID((self as GameClient).ClientData.CopyMapID))
							{
								int num50 = (int)actionParams[0];
								if (-1 != num50)
								{
									if (Global.GetTotalGoodsCountByID(self as GameClient, num50) > 0)
									{
										bool flag3 = false;
										bool flag4 = false;
										if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num50, 1, false, out flag3, out flag4, false))
										{
											if ((self as GameClient).ClientData.FuBenSeqID > 0)
											{
												FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID((self as GameClient).ClientData.FuBenSeqID);
												if (null != fuBenInfoItem)
												{
													fuBenInfoItem.GoodsBinding = (flag3 ? 1 : 0);
												}
											}
											CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap((self as GameClient).ClientData.CopyMapID);
											if (null != copyMap)
											{
												copyMap.ClearKilledNormalDict();
												copyMap.ClearKilledBossDict();
											}
											GameManager.MonsterZoneMgr.ReloadCopyMapMonsters((self as GameClient).ClientData.MapCode, (self as GameClient).ClientData.CopyMapID);
										}
										else
										{
											string goodsNameByID = Global.GetGoodsNameByID(num50);
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(436, new object[0]), new object[]
											{
												goodsNameByID
											}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										}
									}
									else
									{
										string goodsNameByID = Global.GetGoodsNameByID(num50);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(437, new object[0]), new object[]
										{
											goodsNameByID
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 4);
									}
								}
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(438, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
					break;
				case MagicActionIDs.DB_ADD_MONTHVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						43200.0,
						1.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.INSTALL_JUNQI:
					Global.InstallJunQi(self as GameClient, npcID, 0);
					break;
				case MagicActionIDs.TAKE_SHELIZHIYUAN:
					Global.TakeSheLiZhiYuan(self as GameClient, npcID);
					break;
				case MagicActionIDs.DB_ADD_DBLSKILLUP:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.DblSkillUp, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_JIUHUA_ADDLIFE:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)((obj as GameClient).ClientData.LifeV - (obj as GameClient).ClientData.CurrentLifeV), string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.NEW_LIANZHAN_DELAY:
					if (obj is GameClient)
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(obj as GameClient, 11);
						if (null != bufferDataByID)
						{
							if (!Global.IsBufferDataOver(bufferDataByID, 0L))
							{
								if (1800 == bufferDataByID.BufferSecs)
								{
									bufferDataByID.BufferSecs += 3600;
									Global.UpdateDBBufferData(obj as GameClient, bufferDataByID);
									GameManager.ClientMgr.NotifyBufferData(obj as GameClient, bufferDataByID);
								}
							}
						}
					}
					break;
				case MagicActionIDs.DB_ADD_THREE_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 1);
					Global.RemoveBufferData(self as GameClient, 36);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] array = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.ThreeExperience, array, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_THREE_MONEY:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.ThreeMoney, actionParams, 0, true);
					break;
				case MagicActionIDs.DB_ADD_AF_PROTECT:
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.AutoFightingProtect, actionParams, 0, true);
					break;
				case MagicActionIDs.NEW_INSTANT_ATTACK4:
					if (self is GameClient)
					{
						double num36 = actionParams[0];
						double num37 = actionParams[1];
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, skillLevel, num36, num37, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, skillLevel, num36, num37, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_INSTANT_MAGIC4:
					if (self is GameClient)
					{
						double num36 = actionParams[0];
						double num37 = actionParams[1];
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, skillLevel, num36, num37, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, skillLevel, num36, num37, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_TIME_MAGIC4:
					if (self is GameClient)
					{
						double num43 = actionParams[0];
						double num39 = actionParams[1];
						int num22 = 0;
						int burst = 0;
						if (obj is GameClient)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, 0.0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, 0.0, 0, 0, 0.0);
						}
						if (num22 > 0)
						{
							double[] array2 = new double[]
							{
								(double)num22,
								actionParams[2],
								actionParams[3]
							};
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.NEW_TIME_MAGIC4, array2, obj.GetObjectID());
						}
					}
					break;
				case MagicActionIDs.NEW_YINLIANG_RNDBAO:
					if (self is GameClient)
					{
						int minV = (int)actionParams[0];
						int maxV = (int)actionParams[1];
						int randomNumber = Global.GetRandomNumber(minV, maxV);
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, randomNumber, "脚本增加金币二", false);
					}
					break;
				case MagicActionIDs.GOTO_LEAVELAOFANG:
					if (self is GameClient)
					{
						if ((self as GameClient).ClientData.PKPoint < Global.MinLeaveJailPKPoints)
						{
							Global.BroadcastLeaveLaoFangHint(self as GameClient, (self as GameClient).ClientData.MapCode);
							Global.ForceTakeOutLaoFangMap(self as GameClient, (self as GameClient).ClientData.PKPoint);
						}
						else
						{
							int num50 = (int)actionParams[0];
							if (-1 != num50)
							{
								int num51 = (int)Math.Round(Math.Pow((double)Math.Max((self as GameClient).ClientData.PKValue, 1), 1.5));
								if (Global.GetTotalGoodsCountByID(self as GameClient, num50) >= num51)
								{
									bool flag3 = false;
									bool flag4 = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num50, num51, false, out flag3, out flag4, false))
									{
										Global.BroadcastLeaveLaoFangHint2(self as GameClient, (self as GameClient).ClientData.MapCode);
										Global.ForceTakeOutLaoFangMap(self as GameClient, (self as GameClient).ClientData.PKPoint);
									}
									else
									{
										string goodsNameByID = Global.GetGoodsNameByID(num50);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(439, new object[0]), new object[]
										{
											goodsNameByID
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
								else
								{
									string goodsNameByID = Global.GetGoodsNameByID(num50);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(440, new object[0]), new object[]
									{
										num51,
										goodsNameByID
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_MAPBYGOODS:
					if (self is GameClient)
					{
						if (JunQiManager.GetLingDiIDBy2MapCode((self as GameClient).ClientData.MapCode) == 2)
						{
							HuangChengManager.HandleLeaveMapHuangDiRoleChanging(self as GameClient);
						}
						int num17 = (int)actionParams[0];
						int num50 = (int)actionParams[1];
						int num51 = (int)actionParams[2];
						if (num17 > 0)
						{
							if (-1 != num50)
							{
								if (Global.GetTotalGoodsCountByID(self as GameClient, num50) >= num51)
								{
									bool flag3 = false;
									bool flag4 = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num50, num51, false, out flag3, out flag4, false))
									{
										GameMap gameMap = null;
										if (GameManager.MapMgr.DictMaps.TryGetValue(num17, out gameMap))
										{
											GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num17, -1, -1, -1, 0);
										}
									}
									else
									{
										string goodsNameByID = Global.GetGoodsNameByID(num50);
										string mapName = Global.GetMapName(num17);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(441, new object[0]), new object[]
										{
											mapName,
											goodsNameByID
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
								else
								{
									string goodsNameByID = Global.GetGoodsNameByID(num50);
									string mapName = Global.GetMapName(num17);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(442, new object[0]), new object[]
									{
										mapName,
										num51,
										goodsNameByID
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.SUB_ZUIEZHI:
					if (self is GameClient)
					{
						int num52 = (int)actionParams[0];
						num52 = Global.GMax(0, num52);
						num52 = Global.GMax((self as GameClient).ClientData.PKValue - num52, 0);
						GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num52, (self as GameClient).ClientData.PKPoint, true);
					}
					break;
				case MagicActionIDs.UN_PACK:
					if (self is GameClient)
					{
						int goodsID = (int)actionParams[0];
						int goodsNum = (int)actionParams[1];
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, self as GameClient, goodsID, goodsNum, 0, "", 0, binding, 0, "", true, 1, "解开简单物品获取", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
					}
					break;
				case MagicActionIDs.GOTO_MAPBYVIP:
					if (self is GameClient)
					{
						int num53 = 1;
						if (actionParams.Length > 1)
						{
							num53 = (int)actionParams[1];
						}
						if (Global.GetVipType(self as GameClient) < num53)
						{
							GameManager.LuaMgr.Error(self as GameClient, GLang.GetLang(443, new object[0]), 0);
						}
						else
						{
							if (JunQiManager.GetLingDiIDBy2MapCode((self as GameClient).ClientData.MapCode) == 2)
							{
								HuangChengManager.HandleLeaveMapHuangDiRoleChanging(self as GameClient);
							}
							int num17 = (int)actionParams[0];
							if (num17 > 0)
							{
								if (DBRoleBufferManager.ProcessMonthVIP(self as GameClient) > 0.0)
								{
									GameMap gameMap = null;
									if (GameManager.MapMgr.DictMaps.TryGetValue(num17, out gameMap))
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num17, -1, -1, -1, 0);
									}
								}
								else
								{
									string mapName = Global.GetMapName(num17);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(444, new object[0]), new object[]
									{
										mapName
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_BATTLEMAP:
					if (self is GameClient)
					{
						Global.ClientEnterBattle(self as GameClient);
					}
					break;
				case MagicActionIDs.FALL_BAOXIANG2:
				{
					int num4 = Global.CalcOriginalOccupationID(self as GameClient);
					int fallID = (int)actionParams[num4];
					GoodsBaoXiang.ProcessFallBaoXiang(self as GameClient, fallID, (int)actionParams[3], binding, actionGoodsID);
					break;
				}
				case MagicActionIDs.GOTO_SHILIANTA:
					if (self is GameClient)
					{
						int num54 = -1;
						SystemXmlItem systemXmlItem3 = Global.FindShiLianTaFuBenIDByLevel(self as GameClient, out num54);
						if (null != systemXmlItem3)
						{
							GameClient gameClient = self as GameClient;
							int fuBenID = systemXmlItem3.GetIntValue("ID", -1);
							int num55 = systemXmlItem3.GetIntValue("GoodsNumber", -1);
							num55 = Global.GMax(1, num55);
							int shiLianLingValue = GameManager.ClientMgr.GetShiLianLingValue(gameClient);
							if (shiLianLingValue >= num55)
							{
								GameManager.ClientMgr.ModifyShiLianLingValue(gameClient, -num55, true, true);
								Global.EnterShiLianTaFuBen(gameClient, fuBenID, systemXmlItem3, 1);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(445, new object[0]), new object[]
								{
									shiLianLingValue,
									num55
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 24);
							}
						}
						else if ((self as GameClient).ClientData.Level < num54)
						{
							if (num54 > 0)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(448, new object[0]), new object[]
								{
									num54
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(449, new object[0]), new object[]
								{
									num54
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
						else
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(450, new object[0]), new object[]
							{
								num54
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					break;
				case MagicActionIDs.NEW_ADD_GOLD:
					if (obj is GameClient)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (int)actionParams[0], "NEW_ADD_GOLD");
					}
					break;
				case MagicActionIDs.GOTO_SHENGXIAOGUESSMAP:
					if (self is GameClient)
					{
						Global.ClientEnterShengXiaoGuessMap(self as GameClient);
					}
					break;
				case MagicActionIDs.GOTO_ARENABATTLEMAP:
					if (self is GameClient)
					{
						GameManager.ArenaBattleMgr.ClientEnterArenaBattle(self as GameClient);
					}
					break;
				case MagicActionIDs.USE_GOODSFORDLG:
					if (self is GameClient)
					{
						int windowType = (int)actionParams[0];
						int num56 = (int)actionParams[1];
						GameManager.ClientMgr.NotifyClientOpenWindow(self as GameClient, windowType, num56.ToString());
					}
					break;
				case MagicActionIDs.SUB_PKZHI:
					if (self is GameClient)
					{
						int num57 = (int)actionParams[0];
						int pkvalue = (self as GameClient).ClientData.PKValue;
						int num58 = (self as GameClient).ClientData.PKPoint;
						num58 = Global.GMax(0, num58 - num57);
						GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, pkvalue, num58, true);
					}
					break;
				case MagicActionIDs.CALL_MONSTER:
					if (self is GameClient)
					{
						int num26 = (int)actionParams[0];
						int num59 = (int)actionParams[1];
						Point currentGrid3 = (self as GameClient).CurrentGrid;
						GameManager.LuaMgr.AddDynamicMonsters(self as GameClient, num26, num59, (int)currentGrid3.X, (int)currentGrid3.Y, 3);
					}
					break;
				case MagicActionIDs.NEW_ADD_JIFEN:
					if (self is GameClient)
					{
						int addValue = (int)actionParams[0];
						GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(self as GameClient, addValue, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_LIESHA:
					if (self is GameClient)
					{
						int addValue = (int)actionParams[0];
						GameManager.ClientMgr.ModifyLieShaValue(self as GameClient, addValue, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_WUXING:
					if (self is GameClient)
					{
						int addValue = (int)actionParams[0];
						GameManager.ClientMgr.ModifyWuXingValue(self as GameClient, addValue, true, true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_ZHENQI:
					if (self is GameClient)
					{
						int addValue = (int)actionParams[0];
						GameManager.ClientMgr.ModifyZhenQiValue(self as GameClient, addValue, true, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TIANSHENG:
					if (self is GameClient)
					{
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.FallTianSheng, actionParams, 0, true);
					}
					break;
				case MagicActionIDs.ADD_XINGYUN:
					if (self is GameClient)
					{
						Global.AddWeaponLucky(self as GameClient, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.FALL_XINGYUN:
					if (self is GameClient)
					{
						Global.ProcessWeaponTongLing(self as GameClient);
					}
					break;
				case MagicActionIDs.NEW_PACK_SHILIAN:
					if (self is GameClient)
					{
						int addValue = (int)actionParams[0];
						GameManager.ClientMgr.ModifyShiLianLingValue(self as GameClient, addValue, true, true);
					}
					break;
				case MagicActionIDs.DB_NEW_ADD_ZHUFUTIME:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[0] / 60.0,
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ZhuFu, array, 0, true);
						Global.NotifySelfAddKaoHuoTime(self as GameClient, (int)array[0]);
					}
					break;
				case MagicActionIDs.NEW_ADD_MAPTIME:
					if (self is GameClient)
					{
						Global.AddExtLimitSecsByMapCode(self as GameClient, (int)actionParams[0], (int)actionParams[1]);
					}
					break;
				case MagicActionIDs.DB_ADD_WAWA_EXP:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.WaWaExp, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_TIME_LIFE_MAGIC:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[2],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddLifeMagic, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_INSTANT_LIFE_MAGIC:
					if (self is GameClient)
					{
						double num25 = actionParams[0];
						double magicV = actionParams[1];
						GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, num25, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
						GameManager.ClientMgr.AddSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, magicV, string.Format("道具{0}, 脚本{1}", actionGoodsID, id));
					}
					break;
				case MagicActionIDs.DB_ADD_MAXATTACKV:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddAttack, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMATTACKV:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMAttack, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXDSATTACKV:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.RemoveBufferData(self as GameClient, 39);
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddDSAttack, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXDEFENSEV:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddDefense, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMDEFENSEV:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMDefense, array, 0, true);
					}
					break;
				case MagicActionIDs.OPEN_QIAN_KUN_DAI:
					if (self is GameClient)
					{
					}
					break;
				case MagicActionIDs.RUN_LUA_SCRIPT:
					if (self is GameClient)
					{
						int fileID = (int)actionParams[0];
						string runLuaScriptFile = Global.GetRunLuaScriptFile(fileID);
						Global.ExcuteLuaFunction(self as GameClient, runLuaScriptFile, "run", null, null);
					}
					break;
				case MagicActionIDs.DB_ADD_EXP:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeExp, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_SEASONVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						129600.0,
						3.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.DB_ADD_HALFYEARVIP:
				{
					bool isVipBefore = Global.IsVip(self as GameClient);
					actionParams = new double[]
					{
						259200.0,
						6.0
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.MonthVIP, actionParams, 0, true);
					Global.BroadcastVIPMonthHint(self as GameClient, actionGoodsID);
					Global.TryGiveGuMuTimeLimitAwardOnBecomeVip(self as GameClient, isVipBefore);
					break;
				}
				case MagicActionIDs.GOTO_MINGJIEMAP:
					if (self is GameClient && actionParams.Length >= 2)
					{
						Global.GotoMingJieTimeLimitMap(self as GameClient, (int)actionParams[0], (int)actionParams[1]);
					}
					break;
				case MagicActionIDs.ADD_GUMUMAPTIME:
					if (self is GameClient && actionParams.Length >= 1)
					{
						Global.AddGuMuMapTime(self as GameClient, 0, (int)actionParams[0]);
					}
					break;
				case MagicActionIDs.ADD_BOSSCOPYENTERNUM:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num59 = (int)actionParams[0];
						if (num59 >= 0)
						{
							Global.UpdateBossFuBenExtraEnterNum(gameClient, num59);
							GameManager.LuaMgr.Hot(self as GameClient, string.Format(GLang.GetLang(451, new object[0]), num59), 0);
							Global.ExecNpcTalkText(gameClient, 2, 2130706641, 209, 1);
						}
					}
					break;
				case MagicActionIDs.GOTO_BOSSCOPYMAP:
					if (self is GameClient)
					{
						GameManager.LuaMgr.EnterBossFuBen(self as GameClient);
					}
					break;
				case MagicActionIDs.DB_ADD_FIVE_EXP:
				{
					Global.RemoveBufferData(self as GameClient, 1);
					Global.RemoveBufferData(self as GameClient, 18);
					Global.RemoveBufferData(self as GameClient, 46);
					double[] array = new double[]
					{
						actionParams[0],
						(double)actionGoodsID
					};
					Global.UpdateBufferData(self as GameClient, BufferItemTypes.FiveExperience, array, 0, true);
					break;
				}
				case MagicActionIDs.DB_ADD_RANDOM_EXP:
					if (self is GameClient)
					{
						int num60 = Global.GMax(0, (int)actionParams[0]);
						int maxV2 = Global.GMax(num60, (int)actionParams[1]);
						GameManager.ClientMgr.ProcessRoleExperience(self as GameClient, (long)Global.GetRandomNumber(num60, maxV2), false, false, false, "none");
					}
					break;
				case MagicActionIDs.GOTO_MAPBYYUANBAO:
					if (self is GameClient)
					{
						int num61 = (int)actionParams[0];
						int num17 = (int)actionParams[1];
						if (num61 > 0)
						{
							bool flag5 = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, self as GameClient, num61, "GOTO_MAPBYYUANBAO公式", true, true, false, DaiBiSySType.None);
							if (flag5)
							{
								GameManager.LuaMgr.GotoMap(self as GameClient, num17, -1, -1, -1);
							}
							else
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(431, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
							}
						}
					}
					break;
				case MagicActionIDs.ADD_DAILY_NUM:
					if (self is GameClient)
					{
						int taskClass = Global.GMax(3, (int)actionParams[0]);
						taskClass = Global.GMin(9, (int)actionParams[0]);
						int num59 = Global.GMax(1, (int)actionParams[1]);
						Global.AddExtNumByGoods(self as GameClient, taskClass, num59);
					}
					break;
				case MagicActionIDs.DB_TIME_LIFE_NOSHOW:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddLifeNoShow, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_TIME_MAGIC_NOSHOW:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.TimeAddMagicNoShow, array, 0, true);
					}
					break;
				case MagicActionIDs.GOTO_GUMUMAP:
					if (self is GameClient)
					{
						Global.GotoGuMuMap(self as GameClient);
					}
					break;
				case MagicActionIDs.DB_ADD_MULTIEXP:
					if (self is GameClient)
					{
						Global.RemoveBufferData(self as GameClient, 1);
						Global.RemoveBufferData(self as GameClient, 18);
						Global.RemoveBufferData(self as GameClient, 36);
						double[] array = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MutilExperience, array, 0, true);
					}
					break;
				case MagicActionIDs.RANDOM_SHENQIZHIHUN:
					if (self is GameClient)
					{
						int minV2 = (int)actionParams[0];
						int num62 = (int)actionParams[1];
						int randomNumber2 = Global.GetRandomNumber(minV2, num62 + 1);
						GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(self as GameClient, randomNumber2, true, true);
					}
					break;
				case MagicActionIDs.ADD_JIERI_BUFF:
					if (self is GameClient)
					{
						int num63 = (int)actionParams[0];
						double[] array2 = new double[]
						{
							(double)actionGoodsID,
							(double)num63
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.JieRiChengHao, array2, 0, true);
						Global.InitJieriChengHao(self as GameClient, true);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, true, false, 7);
					}
					break;
				case MagicActionIDs.DB_ADD_ERGUOTOU:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ErGuoTou, array, 0, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_ZHANHUN:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyZhanHunValue(self as GameClient, (int)actionParams[0], true, true);
					}
					break;
				case MagicActionIDs.NEW_ADD_RONGYU:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRongYuValue(self as GameClient, (int)actionParams[0], true, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPSTRENGTH:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPStrength, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPINTELLIGENCE:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPIntelligsence, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPDEXTERITY:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPDexterity, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPCONSTITUTION:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPConstitution, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_TEMPATTACKSPEED:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPATTACKSPEED, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LUCKYATTACK:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPLUCKYATTACK, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_FATALATTACK:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPFATALATTACK, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_DOUBLEATTACK:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.ADDTEMPDOUBLEATTACK, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LUCKYATTACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDLUCKYATTACKPERCENTTIMER, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_FATALATTACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDFATALATTACKPERCENTTIMER, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_DOUBLETACKPERCENTTIMER:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDDOUBLEATTACKPERCENTTIMER, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXHPVALUE:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDMAXHPVALUE, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_MAXMPVALUE:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDMAXMPVALUE, array, 0, true);
					}
					break;
				case MagicActionIDs.DB_ADD_LIFERECOVERPERCENT:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)actionGoodsID
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDLIFERECOVERPERCENT, array, 0, true);
					}
					break;
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK:
				{
					double num64 = actionParams[0];
					double num65 = actionParams[1];
					double num66 = actionParams[2];
					num64 += num66 * (double)skillLevel;
					num65 += num66 * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int num5 = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK:
				{
					double num64 = actionParams[0];
					double num65 = actionParams[1];
					double num66 = actionParams[2];
					num64 += num66 * (double)skillLevel;
					num65 += num66 * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int num5 = 1;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, (int)num64, (int)num65, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, skillLevel, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, (int)num64, (int)num65, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER:
					if (self is GameClient)
					{
						double num67 = actionParams[0];
						double num68 = actionParams[1];
						double num66 = actionParams[2];
						num68 += num66 * (double)skillLevel;
						double[] array2 = new double[]
						{
							num67,
							num68
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, actionParams, -1);
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTTIMER, array2, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HP_PERCENT_TIMER:
					if (self is GameClient)
					{
						double num67 = actionParams[0];
						double num68 = actionParams[1];
						double num66 = actionParams[2];
						num68 += num66 * (double)skillLevel;
						long num69 = TimeUtil.NOW() * 10000L;
						long num70 = (long)((double)num69 + num67 * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(14, num68, num70);
							double[] array2 = new double[]
							{
								num67,
								num68
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_MAXLIFEPERCENT, array2, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(14, num68, num70);
							double[] array2 = new double[]
							{
								num67,
								num68
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_MAXLIFEPERCENT, array2, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_TIMER:
					if (self is GameClient)
					{
						double num67 = actionParams[0];
						double num71 = actionParams[1];
						double num72 = actionParams[2];
						num71 += num72 * (double)skillLevel;
						long num69 = TimeUtil.NOW() * 10000L;
						long num70 = (long)((double)num69 + num67 * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, num71, num70);
							double[] array2 = new double[]
							{
								num67,
								num71
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDDEFENSETIMER, array2, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(3, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(4, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, num71, num70);
							double[] array2 = new double[]
							{
								num67,
								num71
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDDEFENSETIMER, array2, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_ATTACK_TIMER:
					if (self is GameClient)
					{
						double num67 = actionParams[0];
						double num71 = actionParams[1];
						double num72 = actionParams[2];
						num71 += num72 * (double)skillLevel;
						long num69 = TimeUtil.NOW() * 10000L;
						long num70 = (long)((double)num69 + num67 * 1000.0 * 10000.0);
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(7, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(8, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(9, num71, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(10, num71, num70);
							double[] array2 = new double[]
							{
								num67,
								num71
							};
							Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDATTACKTIMER, array2, 1, true);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(7, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(8, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(9, num71, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(10, num71, num70);
							double[] array2 = new double[]
							{
								num67,
								num71
							};
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_ADDATTACKTIMER, array2, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HP:
					if (self is GameClient)
					{
						double num73 = actionParams[0];
						double num72 = actionParams[1];
						num73 += num72 * (double)skillLevel;
						if (obj is GameClient)
						{
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, (double)((int)num73), string.Format("无情一击, 脚本{0}", id));
						}
						else
						{
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, (double)((int)num73), string.Format("无情一击, 脚本{0}", id));
						}
					}
					break;
				case MagicActionIDs.MU_BLINK_MOVE:
					if (self is GameClient)
					{
						double num74 = actionParams[0];
						double num75 = actionParams[1];
						long num27 = TimeUtil.NOW();
						DelayAction delayAction = new DelayAction();
						delayAction.m_DelayTime = (long)num74;
						delayAction.m_StartTime = num27;
						delayAction.m_Params[0] = (int)num75;
						delayAction.m_Client = (self as GameClient);
						List<object> list = Global.GetAll9Clients(self as GameClient);
						string text = string.Format("{0}", (self as GameClient).ClientData.RoleID);
						GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, text, 510);
						DelayActionManager.AddDelayAction(delayAction);
					}
					break;
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER1:
					if (self is GameClient)
					{
						double num67 = actionParams[0];
						double num68 = actionParams[1];
						double num66 = actionParams[2];
						num68 += num66 * (double)skillLevel;
						double[] array2 = new double[]
						{
							num67,
							num68
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, actionParams, -1);
							Global.UpdateBufferData(self as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTTIMER1, array2, 1, true);
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_SHUXING:
					if (self is GameClient)
					{
						DataHelper.WriteStackTraceLog("随机增加基础属性之一的功能尚未实现");
					}
					break;
				case MagicActionIDs.MU_RANDOM_STRENGTH:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							int num76 = (int)(actionParams[0] * 100.0);
							int minV3 = (int)actionParams[1];
							int num77 = (int)actionParams[2];
							int num78 = 0;
							int randomNumber3 = Global.GetRandomNumber(0, 101);
							if (randomNumber3 <= num76)
							{
								int num79 = Global.GetRandomNumber(minV3, num77 + 1);
								string lang = GLang.GetLang(452, new object[0]);
								num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropStrengthChangeless");
								if (bItemAddVal)
								{
									int fruitAddPropLimit = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Strength");
									num79 = UseFruitVerify.AddValueVerify(gameClient, num78, fruitAddPropLimit, num79);
									if (num79 <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											lang
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										flag = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (gameClient.ClientData.PropPointMutex)
									{
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropStrengthChangeless");
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropStrengthChangeless", num78 + num79, true);
										gameClient.ClientData.PropStrength += num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropStrength", gameClient.ClientData.PropStrength, true);
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "TotalPropPoint");
										gameClient.ClientData.TotalPropPoint = num78 + num79;
									}
									Global.SaveRoleParamsInt32ValueToDB(gameClient, "TotalPropPoint", num78 + num79, true);
									Global.RefreshEquipProp(gameClient);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										lang,
										num79
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_INTELLIGENCE:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							int num76 = (int)(actionParams[0] * 100.0);
							int minV3 = (int)actionParams[1];
							int num77 = (int)actionParams[2];
							int num78 = 0;
							int randomNumber3 = Global.GetRandomNumber(0, 101);
							if (randomNumber3 <= num76)
							{
								int num79 = Global.GetRandomNumber(minV3, num77 + 1);
								string lang = GLang.GetLang(453, new object[0]);
								num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropIntelligenceChangeless");
								if (bItemAddVal)
								{
									int fruitAddPropLimit = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Intelligence");
									num79 = UseFruitVerify.AddValueVerify(gameClient, num78, fruitAddPropLimit, num79);
									if (num79 <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											lang
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										flag = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (gameClient.ClientData.PropPointMutex)
									{
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropIntelligenceChangeless");
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropIntelligenceChangeless", num78 + num79, true);
										gameClient.ClientData.PropIntelligence += num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropIntelligence", gameClient.ClientData.PropIntelligence, true);
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "TotalPropPoint");
										gameClient.ClientData.TotalPropPoint = num78 + num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "TotalPropPoint", num78 + num79, true);
									}
									Global.RefreshEquipProp(gameClient);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										lang,
										num79
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_DEXTERITY:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							int num76 = (int)(actionParams[0] * 100.0);
							int minV3 = (int)actionParams[1];
							int num77 = (int)actionParams[2];
							int num78 = 0;
							int randomNumber3 = Global.GetRandomNumber(0, 101);
							if (randomNumber3 <= num76)
							{
								int num79 = Global.GetRandomNumber(minV3, num77 + 1);
								string lang = GLang.GetLang(454, new object[0]);
								num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropDexterityChangeless");
								if (bItemAddVal)
								{
									int fruitAddPropLimit = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Dexterity");
									num79 = UseFruitVerify.AddValueVerify(gameClient, num78, fruitAddPropLimit, num79);
									if (num79 <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											lang
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										flag = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (gameClient.ClientData.PropPointMutex)
									{
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropDexterityChangeless");
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropDexterityChangeless", num78 + num79, true);
										gameClient.ClientData.PropDexterity += num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropDexterity", gameClient.ClientData.PropDexterity, true);
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "TotalPropPoint");
										gameClient.ClientData.TotalPropPoint = num78 + num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "TotalPropPoint", num78 + num79, true);
									}
									Global.RefreshEquipProp(gameClient);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										lang,
										num79
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_RANDOM_CONSTITUTION:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							int num76 = (int)(actionParams[0] * 100.0);
							int minV3 = (int)actionParams[1];
							int num77 = (int)actionParams[2];
							int num78 = 0;
							int randomNumber3 = Global.GetRandomNumber(0, 101);
							if (randomNumber3 <= num76)
							{
								int num79 = Global.GetRandomNumber(minV3, num77 + 1);
								string lang = GLang.GetLang(455, new object[0]);
								num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropConstitutionChangeless");
								if (bItemAddVal)
								{
									int fruitAddPropLimit = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Constitution");
									num79 = UseFruitVerify.AddValueVerify(gameClient, num78, fruitAddPropLimit, num79);
									if (num79 <= 0)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(457, new object[0]), new object[]
										{
											lang
										}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										flag = false;
										break;
									}
								}
								if (!bIsVerify)
								{
									lock (gameClient.ClientData.PropPointMutex)
									{
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "PropConstitutionChangeless");
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropConstitutionChangeless", num78 + num79, true);
										gameClient.ClientData.PropConstitution += num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "PropConstitution", gameClient.ClientData.PropConstitution, true);
										num78 = Global.GetRoleParamsInt32FromDB(gameClient, "TotalPropPoint");
										gameClient.ClientData.TotalPropPoint = num78 + num79;
										Global.SaveRoleParamsInt32ValueToDB(gameClient, "TotalPropPoint", num78 + num79, true);
									}
									Global.RefreshEquipProp(gameClient);
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(456, new object[0]), new object[]
									{
										lang,
										num79
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK1:
				{
					double num80 = actionParams[0];
					double num81 = actionParams[1];
					int num82 = skillLevel + 1;
					num80 += num80 / 200.0 * (double)num82;
					num81 += num81 * (double)num82;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int num5 = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_PHYSICAL_ATTACK2:
				{
					double num80 = actionParams[0];
					double num81 = actionParams[1];
					double num2 = actionParams[2];
					int num82 = skillLevel + 1;
					num80 += num80 * num2 * (double)skillLevel;
					num81 += num81 * (double)skillLevel;
					direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
					int num5 = 0;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_ATTACK_DOWN:
					if (self is GameClient)
					{
						double num83 = actionParams[0];
						double num68 = actionParams[1];
						double num74 = actionParams[2];
						int num82 = skillLevel + 1;
						num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateJiTui, id);
						if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
						{
							return false;
						}
						ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
						long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
						double num = actionParams[1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(11, -num, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(12, -num, num70);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 6, TimeUtil.NOW(), (int)num74, 0.0);
						}
						else if (obj is Monster)
						{
							Monster monster = obj as Monster;
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(11, -num, num70);
								monster.TempPropsBuffer.AddTempExtProp(12, -num, num70);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 6, TimeUtil.NOW(), (int)num74, 0.0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HUNMI:
				{
					double num83 = actionParams[0];
					double num74 = actionParams[1];
					int num82 = skillLevel + 1;
					num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateHunMi, id);
					if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).ClientData.DongJieStart = TimeUtil.NOW();
							(obj as GameClient).ClientData.DongJieSeconds = (int)num74;
							long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
							(obj as GameClient).RoleBuffer.AddTempExtProp(50, 1.0, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(2, -10.0, num70);
							double num84 = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							if ((obj as GameClient).ClientData.HorseDbID > 0)
							{
								double horseSpeed = Global.GetHorseSpeed(obj as GameClient);
								num84 += horseSpeed;
							}
							(obj as GameClient).ClientData.MoveSpeed = num84;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 3, TimeUtil.NOW(), (int)num74, num84);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
						{
							monster.DongJieStart = TimeUtil.NOW();
							monster.DongJieSeconds = (int)num74;
							monster.XuanYunStart = TimeUtil.NOW();
							monster.XuanYunSeconds = (int)num74;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 3, TimeUtil.NOW(), (int)num74, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_MOVESPEED_DOWN:
				{
					double num83 = actionParams[0];
					double num68 = actionParams[1];
					double num74 = actionParams[2];
					int num82 = skillLevel + 1;
					num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateMoveSpeed, id);
					if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					double num = actionParams[1];
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(2, -num, num70);
							(obj as GameClient).buffManager.SetStatusBuff(118, TimeUtil.NOW(), (long)num74 * 1000L, 0L);
							double num84 = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							if ((obj as GameClient).ClientData.HorseDbID > 0)
							{
								double horseSpeed = Global.GetHorseSpeed(obj as GameClient);
								num84 += horseSpeed;
							}
							(obj as GameClient).ClientData.MoveSpeed = num84;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 4, TimeUtil.NOW(), (int)num74, num84);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							monster.TempPropsBuffer.AddTempExtProp(2, -num, num70);
							monster.SpeedDownStart = TimeUtil.NOW();
							monster.SpeedDownSeconds = (int)num74;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 4, TimeUtil.NOW(), (int)num74, monster.MoveSpeed);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_LIFE:
				{
					double num85 = actionParams[0];
					double num86 = actionParams[1];
					double num74 = actionParams[2];
					int num82 = skillLevel + 1;
					double num87 = num85 + num85 / 200.0 * (double)num82;
					double num88 = num86 + num86 * (double)num82;
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					double[] array = new double[]
					{
						num74,
						(double)skillid,
						(double)num82
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(14, num87, num70);
						(obj as GameClient).RoleBuffer.AddTempExtProp(13, num88, num70);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(14, num87, num70);
						(self as GameClient).RoleBuffer.AddTempExtProp(13, num88, num70);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDMAXLIFEPERCENTANDVALUE, array, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK1:
					if (0.0 != manyRangeInjuredPercent)
					{
						double num80 = actionParams[0];
						double num81 = actionParams[1];
						int num82 = skillLevel + 1;
						num80 += num80 / 200.0 * (double)num82;
						num81 += num81 * (double)num82;
						direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
						int num5 = 1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_MAGIC_ATTACK2:
					if (0.0 != manyRangeInjuredPercent)
					{
						double num80 = actionParams[0];
						double num81 = actionParams[1];
						double num2 = actionParams[2];
						int num82 = skillLevel + 1;
						num80 += num80 * num2 * (double)skillLevel;
						num81 += num81 * (double)skillLevel;
						direction = (int)((direction < 0) ? self.CurrentDir : ((Dircetions)direction));
						int num5 = 1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num80, (int)num81, 0, skillid, shenShiInjurePercent);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							}
						}
					}
					break;
				case MagicActionIDs.MU_ADD_HIT_DOWN:
				{
					double num83 = actionParams[0];
					double num68 = actionParams[1];
					double num74 = actionParams[2];
					int num82 = skillLevel + 1;
					num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateJiTui, id);
					if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					double num = actionParams[1];
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(18, -num, num70);
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 5, TimeUtil.NOW(), (int)num74, 0.0);
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
						{
							monster.TempPropsBuffer.AddTempExtProp(18, -num, num70);
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 5, TimeUtil.NOW(), (int)num74, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.MU_SUB_DAMAGE_PERCENT:
					if (self is GameClient)
					{
						double num85 = actionParams[0];
						double num86 = actionParams[1];
						double num74 = actionParams[2];
						int num82 = skillLevel + 1;
						double num87 = num85 + num85 / 200.0 * (double)num82;
						double num88 = num86 + num86 * (double)num82;
						double[] magicActionParams = new double[]
						{
							num74,
							num87
						};
						double[] magicActionParams2 = new double[]
						{
							num74,
							num88
						};
						double[] array = new double[]
						{
							num74,
							(double)skillid,
							(double)num82
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, magicActionParams, -1);
							(obj as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_VALUE, magicActionParams2, -1);
						}
						else
						{
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_PERCENT_TIMER, magicActionParams, -1);
							(self as GameClient).RoleMagicHelper.AddMagicHelper(MagicActionIDs.MU_SUB_DAMAGE_VALUE, magicActionParams2, -1);
						}
						Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_SUBDAMAGEPERCENTVALUETIMER, array, 1, true);
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_DOWN:
					if (self is GameClient)
					{
						double num83 = actionParams[0];
						double num68 = actionParams[1];
						double num74 = actionParams[2];
						int num82 = skillLevel + 1;
						double num89 = num83 + num83 / 200.0 * (double)num82;
						if ((double)Global.GetRandomNumber(0, 101) > num89 * 100.0)
						{
							return false;
						}
						long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
						double num = actionParams[1];
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(42, -num, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(43, -num, num70);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 7, TimeUtil.NOW(), (int)num74, 0.0);
						}
						else if (obj is Monster)
						{
							Monster monster = obj as Monster;
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(42, -num, num70);
								monster.TempPropsBuffer.AddTempExtProp(43, -num, num70);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 7, TimeUtil.NOW(), (int)num74, 0.0);
							}
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(42, -num, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(43, -num, num70);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, 7, TimeUtil.NOW(), (int)num74, 0.0);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_DEFENSE_ATTACK:
					if (self is GameClient)
					{
						double num85 = actionParams[0];
						double num86 = actionParams[1];
						double num74 = actionParams[2];
						long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
						int num82 = skillLevel + 1;
						double num87 = num85 + num85 / 200.0 * (double)num82;
						double num88 = num86 + num86 * (double)num82;
						double[] array = new double[]
						{
							num74,
							(double)skillid,
							(double)num82
						};
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(11, num87, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(12, num87, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(7, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(8, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(9, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(10, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(42, num87, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(43, num87, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(3, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(4, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(5, num88, num70);
							(obj as GameClient).RoleBuffer.AddTempExtProp(6, num88, num70);
						}
						else
						{
							(self as GameClient).RoleBuffer.AddTempExtProp(11, num87, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(12, num87, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(7, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(8, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(9, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(10, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(42, num87, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(43, num87, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(3, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(4, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(5, num88, num70);
							(self as GameClient).RoleBuffer.AddTempExtProp(6, num88, num70);
						}
						Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADDATTACKANDDEFENSEEPERCENTVALUETIMER, array, 1, true);
					}
					break;
				case MagicActionIDs.MU_ADD_JITUI:
				{
					double num83 = actionParams[0];
					double num75 = actionParams[1];
					double num90 = actionParams[2];
					int num82 = skillLevel + 1;
					num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateJiTui, id);
					if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					int num91 = (int)num75;
					int num5 = (int)num90;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, 0.0, 0, num91, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, 0.0, 0, num91, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, (double)num91, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, 0.0, 0, num91, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 0.0, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_DINGSHENG:
				{
					double num83 = actionParams[0];
					double num74 = actionParams[1];
					int num82 = skillLevel + 1;
					num83 = StateRate.GetNegativeRate(self, obj, num83, ExtPropIndexes.StateDingShen, id);
					if ((double)Global.GetRandomNumber(0, 101) > num83 * 100.0)
					{
						return false;
					}
					ZuoQiManager.getInstance().RoleDisMount(obj as GameClient, true);
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					if (obj is GameClient)
					{
						if (obj is GameClient)
						{
							(obj as GameClient).RoleBuffer.AddTempExtProp(47, 1.0, num70);
							double num84 = RoleAlgorithm.GetMoveSpeed(obj as GameClient);
							(obj as GameClient).ClientData.MoveSpeed = num84;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 8, TimeUtil.NOW(), (int)num74, num84);
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(2, -monster.MoveSpeed, num70);
								monster.DingShenStart = TimeUtil.NOW();
								monster.DingShenSeconds = (int)num74;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 8, TimeUtil.NOW(), (int)num74, monster.MoveSpeed);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_HIT_DODGE:
				{
					double num85 = actionParams[0];
					double num86 = actionParams[1];
					double num74 = actionParams[2];
					int num82 = skillLevel + 1;
					double num87 = num85 + num85 / 200.0 * (double)num82;
					double num88 = num86 + num86 * (double)num82;
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					double[] array = new double[]
					{
						num74,
						(double)skillid,
						(double)num82
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(54, num87, num70);
						(obj as GameClient).RoleBuffer.AddTempExtProp(18, num88, num70);
						(obj as GameClient).RoleBuffer.AddTempExtProp(55, num87, num70);
						(obj as GameClient).RoleBuffer.AddTempExtProp(19, num88, num70);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(54, num87, num70);
						(self as GameClient).RoleBuffer.AddTempExtProp(18, num88, num70);
						(self as GameClient).RoleBuffer.AddTempExtProp(55, num87, num70);
						(self as GameClient).RoleBuffer.AddTempExtProp(19, num88, num70);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADD_HIT_DODGE_PERCENT, array, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_CHENMO:
				{
					double[] array2 = new double[4];
					BufferItemTypes bufferItemTypes = BufferItemTypes.None;
					if (MagicActionIDs.MU_ADD_CHENMO == id)
					{
						bufferItemTypes = BufferItemTypes.TimeRANSHAONoShow;
					}
					if (!(obj is GameClient) || !(obj as GameClient).buffManager.IsBuffEnabled(116))
					{
						int num82 = skillLevel + 1;
						long num92 = 1000L;
						int execCount = (int)actionParams[1];
						double num20 = actionParams[1];
						double num93 = actionParams[0];
						int addVlue = 0;
						int num22 = 0;
						int burst = 0;
						int num5;
						if (self is GameClient)
						{
							num5 = Global.GetAttackType(self as GameClient);
							if (0 == num5)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == num5 || 2 == num5)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
							int roleID = (self as GameClient).ClientData.RoleID;
							array2[0] = num20;
							array2[1] = 1.0;
							array2[2] = (double)num22;
							array2[3] = (double)roleID;
						}
						num5 = -1;
						if (self is GameClient)
						{
							if (obj is GameClient)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, false, num93, 0, 0, 0, 0.0);
							}
							else if (obj is Monster)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
							}
							else if (obj is BiaoCheItem)
							{
								BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
							}
							else if (obj is JunQiItem)
							{
								JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
							}
							else if (obj is FakeRoleItem)
							{
								FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
							}
						}
						else if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
						}
						else if (!(obj is BiaoCheItem))
						{
							if (obj is JunQiItem)
							{
								JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
							}
						}
						if (num22 > 0)
						{
							if (obj is GameClient)
							{
								Global.UpdateBufferData(obj as GameClient, bufferItemTypes, array2, 1, true);
								MagicAction.ChenMoContextData context = new MagicAction.ChenMoContextData
								{
									Attacker = self.GetObjectID(),
									Self = obj,
									Injure = (int)array2[2],
									Occupation = (obj as GameClient).ClientData.Occupation,
									Stop = false,
									percent = num93
								};
								(obj as GameClient).TimedActionMgr.AddItem(TimeUtil.NOW() + num92, num92, execCount, 0, new Action<long, object>(MagicAction.ChenMoActionProc), context);
								(obj as GameClient).ClientData.ZhongDuStart = TimeUtil.NOW();
								(obj as GameClient).ClientData.ZhongDuSeconds = (int)num20;
								GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 1, TimeUtil.NOW(), (int)num20, 0.0);
							}
							else if (obj is Monster)
							{
								Global.UpdateMonsterBufferData(obj as Monster, bufferItemTypes, array2);
								MagicAction.ChenMoContextData context = new MagicAction.ChenMoContextData
								{
									Attacker = self.GetObjectID(),
									Self = obj,
									Injure = (int)array2[2],
									Stop = false,
									percent = num93
								};
								(obj as Monster).TimedActionMgr.AddItem(TimeUtil.NOW() + num92, num92, execCount, 0, new Action<long, object>(MagicAction.ChenMoActionProc), context);
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, 1, TimeUtil.NOW(), (int)num20, 0.0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.HUIFU:
					if (actionParams.Length >= 2)
					{
						if (self is GameClient)
						{
							GameClient gameClient = self as GameClient;
							int num94 = (int)((double)gameClient.ClientData.LifeV * actionParams[0] + actionParams[1]);
							gameClient.ClientData.CurrentLifeV += num94;
							if (gameClient.ClientData.CurrentLifeV > gameClient.ClientData.LifeV)
							{
								gameClient.ClientData.CurrentLifeV = gameClient.ClientData.LifeV;
							}
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
						}
					}
					break;
				case MagicActionIDs.MU_ADD_YISHANG:
					if (actionParams.Length >= 3)
					{
						if (Global.GetRandom() <= actionParams[0])
						{
							double[] array2 = new double[2];
							BufferItemTypes bufferItemTypes = BufferItemTypes.ZuZhou;
							array2[0] = actionParams[2];
							array2[1] = actionParams[1] * 1000.0;
							if (obj is GameClient)
							{
								Global.UpdateBufferData(obj as GameClient, bufferItemTypes, array2, 1, true);
							}
							else if (obj is Monster)
							{
								Global.UpdateMonsterBufferData(obj as Monster, bufferItemTypes, array2);
							}
						}
					}
					break;
				case MagicActionIDs.NODIE:
				{
					double[] array2 = new double[]
					{
						actionParams[1],
						actionParams[0]
					};
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						gameClient.buffManager.SetStatusBuff(120, TimeUtil.NOW(), (long)(1000 * (int)array2[0]), (long)((int)array2[1]));
						gameClient.buffManager.UpdateImmediately(self as GameClient, 120, TimeUtil.NOW());
					}
					break;
				}
				case MagicActionIDs.GOTO_ANGELTEMPLE:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient.ClientData.ChangeLifeCount < GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(459, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
						}
						else if (gameClient.ClientData.ChangeLifeCount == GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
						{
							if (gameClient.ClientData.Level < GameManager.AngelTempleMgr.m_AngelTempleData.MinLevel)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(460, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
						}
						if (!GameManager.AngelTempleMgr.CanEnterAngelTempleOnTime())
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(461, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (GameManager.AngelTempleMgr.m_AngelTempleScene.m_eStatus == AngelTempleStatus.FIGHT_STATUS_END)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(462, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Interlocked.Increment(ref GameManager.AngelTempleMgr.m_AngelTempleScene.m_nPlarerCount) > GameManager.AngelTempleMgr.m_AngelTempleData.MaxPlayerNum)
						{
							Interlocked.Decrement(ref GameManager.AngelTempleMgr.m_AngelTempleScene.m_nPlarerCount);
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(463, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else
						{
							gameClient.ClientData.bIsInAngelTempleMap = true;
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, GameManager.AngelTempleMgr.m_AngelTempleData.MapCode, -1, -1, -1, 0);
							int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
							int nType = 5;
							int num95 = Global.QueryDayActivityEnterCountToDB(gameClient, gameClient.ClientData.RoleID, dayOfYear, nType);
							num95++;
							Global.UpdateDayActivityEnterCountToDB(gameClient, gameClient.ClientData.RoleID, dayOfYear, nType, num95);
							LogManager.WriteLog(0, string.Format("{0} enter AngelTemple count={1} time={2}", gameClient.ClientData.RoleID, num95, TimeUtil.NowDateTime().ToLongDateString()), null, true);
						}
					}
					break;
				case MagicActionIDs.OPEN_TREASURE_BOX:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						double num96 = actionParams[0];
						double num97 = actionParams[1];
						double num98 = actionParams[2];
						GoodsData goodsData = Global.GetGoodsByID(gameClient, (int)num96);
						if (goodsData != null)
						{
							int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
							if (goodsCatetoriy == 701)
							{
								int i = 0;
								while ((double)i < num98)
								{
									GoodsBaoXiang.CreateGoodsBaseFallID(gameClient, (int)num97, (int)num98);
									i++;
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_BOOSZHIJIA:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							if (gameClient.ClientData.VipLevel < Data.BosshomeData.VIPLevLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(464, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (gameClient.ClientData.ChangeLifeCount < Data.BosshomeData.MinChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(465, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (gameClient.ClientData.ChangeLifeCount > Data.BosshomeData.MaxChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(466, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
							}
							else
							{
								if (gameClient.ClientData.ChangeLifeCount == GameManager.AngelTempleMgr.m_AngelTempleData.MinChangeLifeNum)
								{
									if (gameClient.ClientData.Level < Data.BosshomeData.MinLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(467, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
										break;
									}
									if (gameClient.ClientData.Level > Data.BosshomeData.MaxLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(468, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
										break;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Data.BosshomeData.EnterNeedDiamond, "进入BOSS之家", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(469, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, Data.BosshomeData.MapID, -1, -1, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.GOTO_HUANGJINSHENGDIAN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						if (gameClient != null)
						{
							if (gameClient.ClientData.VipLevel < Data.GoldtempleData.VIPLevLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(470, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else if (gameClient.ClientData.ChangeLifeCount < Data.GoldtempleData.MinChangeLifeLimit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(471, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
							}
							else
							{
								if (gameClient.ClientData.ChangeLifeCount == Data.GoldtempleData.MinChangeLifeLimit)
								{
									if (gameClient.ClientData.Level < Data.GoldtempleData.MinLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(472, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 39);
										break;
									}
									if (gameClient.ClientData.Level > Data.GoldtempleData.MaxLevel)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(473, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
										break;
									}
								}
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, Data.GoldtempleData.EnterNeedDiamond, "进入火龙突袭", true, true, false, DaiBiSySType.None))
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(474, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 30);
								}
								else
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, Data.GoldtempleData.MapID, -1, -1, -1, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.ADD_VIPEXP:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num99 = (int)actionParams[0];
						if (num99 > 0)
						{
							num99 += Global.GetRoleParamsInt32FromDB(gameClient, "VIPExp");
							Global.SaveRoleParamsInt32ValueToDB(gameClient, "VIPExp", num99, true);
							Global.ProcessVipLevelUp(gameClient);
						}
					}
					break;
				case MagicActionIDs.GET_AWARD_BLOODCASTLECOPYSCENE:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						BloodCastleDataInfo bloodCastleDataInfo = null;
						if (gameClient.ClientData.FuBenSeqID >= 0 && gameClient.ClientData.CopyMapID >= 0 && Data.BloodCastleDataInfoList.TryGetValue(gameClient.ClientData.FuBenID, out bloodCastleDataInfo) && bloodCastleDataInfo != null)
						{
							CopyMap bloodCastleCopySceneInfo = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneInfo(gameClient.ClientData.FuBenSeqID);
							if (bloodCastleCopySceneInfo != null)
							{
								int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(gameClient, "BloodCastleSceneid");
								BloodCastleScene bloodCastleCopySceneDataInfo = GameManager.BloodCastleCopySceneMgr.GetBloodCastleCopySceneDataInfo(bloodCastleCopySceneInfo, gameClient.ClientData.FuBenSeqID, roleParamsInt32FromDB);
								if (bloodCastleCopySceneDataInfo != null)
								{
									if (bloodCastleCopySceneDataInfo.m_eStatus == BloodCastleStatus.FIGHT_STATUS_END)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, StringUtil.substitute(GLang.GetLang(458, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
									else
									{
										bloodCastleCopySceneDataInfo.m_nRoleID = gameClient.ClientData.RoleID;
										GoodsData goodsByID = Global.GetGoodsByID(gameClient, 10000);
										if (goodsByID == null)
										{
											goodsByID = Global.GetGoodsByID(gameClient, 10001);
										}
										if (goodsByID == null)
										{
											goodsByID = Global.GetGoodsByID(gameClient, 10002);
										}
										if (goodsByID != null)
										{
											GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, goodsByID, 1, false, false);
											string[] awardItem = bloodCastleDataInfo.AwardItem1;
											if (awardItem != null && awardItem.Length > 0)
											{
												for (int i = 0; i < awardItem.Length; i++)
												{
													if (!string.IsNullOrEmpty(awardItem[i].Trim()))
													{
														string[] array3 = awardItem[i].Split(new char[]
														{
															','
														});
														if (!string.IsNullOrEmpty(array3[i].Trim()))
														{
															int goodsID2 = Convert.ToInt32(array3[0].Trim());
															int num100 = Convert.ToInt32(array3[1].Trim());
															int binding2 = Convert.ToInt32(array3[2].Trim());
															GoodsData goodsData = new GoodsData
															{
																Id = -1,
																GoodsID = goodsID2,
																Using = 0,
																Forge_level = 0,
																Starttime = "1900-01-01 12:00:00",
																Endtime = "1900-01-01 12:00:00",
																Site = 0,
																Quality = 0,
																Props = "",
																GCount = num100,
																Binding = binding2,
																Jewellist = "",
																BagIndex = 0,
																AddPropIndex = 0,
																BornIndex = 0,
																Lucky = 0,
																Strong = 0,
																ExcellenceInfo = 0,
																AppendPropLev = 0,
																ChangeLifeLevForEquip = 0
															};
															string lang2 = GLang.GetLang(23, new object[0]);
															if (!Global.CanAddGoodsNum(gameClient, num100))
															{
																Global.UseMailGivePlayerAward(gameClient, goodsData, GLang.GetLang(1, new object[0]), lang2, 1.0);
															}
															else
															{
																Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, goodsID2, num100, 0, "", 0, 0, 0, "", true, 1, lang2, "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
															}
														}
													}
												}
											}
											bloodCastleCopySceneDataInfo.m_eStatus = BloodCastleStatus.FIGHT_STATUS_END;
											bloodCastleCopySceneDataInfo.m_lEndTime = TimeUtil.NOW();
											bloodCastleCopySceneDataInfo.m_bIsFinishTask = true;
											GameManager.BloodCastleCopySceneMgr.CompleteBloodCastScene(gameClient, bloodCastleCopySceneDataInfo, bloodCastleDataInfo);
										}
									}
								}
							}
						}
					}
					break;
				case MagicActionIDs.ADDMONSTERSKILL:
					if (self is Monster)
					{
						int num26 = (int)actionParams[0];
						int num35 = (int)actionParams[1];
						int num101 = (int)actionParams[2];
						List<object> list = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CopyMapID, num26);
						for (int i = 0; i < list.Count; i++)
						{
							(list[i] as Monster).AddDynSkillID(num35, num101);
						}
						Debug.WriteLine(string.Format("Boss AI, Add monster skill, MonsterID={0}, SkillID={1}, Priority={2}", num26, skillid, num101));
					}
					break;
				case MagicActionIDs.REMOVEMONSTERSKILL:
					if (self is Monster)
					{
						int num26 = (int)actionParams[0];
						int num35 = (int)actionParams[1];
						List<object> list = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CopyMapID, num26);
						for (int i = 0; i < list.Count; i++)
						{
							(list[i] as Monster).RemoveDynSkill(num35);
						}
						Debug.WriteLine(string.Format("Boss AI, Remove monster skill, MonsterID={0}, SkillID={1}", num26, skillid));
					}
					break;
				case MagicActionIDs.BOSS_CALLMONSTERONE:
					if (self is Monster)
					{
						int num26 = (int)actionParams[0];
						int num59 = (int)actionParams[1];
						int num102 = (int)actionParams[2];
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue((self as Monster).CurrentMapCode, out gameMap))
						{
							Point currentGrid3 = (self as Monster).CurrentGrid;
							num102 = (num102 - 1) / gameMap.MapGridWidth + 1;
							GameManager.MonsterZoneMgr.AddDynamicMonsters((self as Monster).CurrentMapCode, num26, (self as Monster).CopyMapID, num59, (int)currentGrid3.X, (int)currentGrid3.Y, num102, 0, 0, null, null);
							Debug.WriteLine(string.Format("Boss AI, Call monster one, MonsterID={0}, AddNum={1}, Radius={2}, Grid={3}", new object[]
							{
								num26,
								num59,
								num102,
								currentGrid3
							}));
						}
					}
					break;
				case MagicActionIDs.BOSS_CALLMONSTERTWO:
					if (self is Monster)
					{
						int num26 = (int)actionParams[0];
						int num59 = (int)actionParams[1];
						int value = (int)actionParams[2];
						int value2 = (int)actionParams[3];
						int num102 = (int)actionParams[4];
						if (actionParams.Length >= 6)
						{
							int num103 = (int)actionParams[5];
						}
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue((self as Monster).CurrentMapCode, out gameMap))
						{
							Point currentGrid3 = new Point((double)gameMap.CorrectPointToGrid(value), (double)gameMap.CorrectPointToGrid(value2));
							num102 = (num102 - 1) / gameMap.MapGridWidth + 1;
							GameManager.MonsterZoneMgr.AddDynamicMonsters((self as Monster).CurrentMapCode, num26, (self as Monster).CopyMapID, num59, (int)currentGrid3.X, (int)currentGrid3.Y, num102, 0, 0, null, null);
							Debug.WriteLine(string.Format("Boss AI, Call monster two, MonsterID={0}, AddNum={1}, Radius={2}, Grid={3}", new object[]
							{
								num26,
								num59,
								num102,
								currentGrid3
							}));
						}
					}
					break;
				case MagicActionIDs.CLEAR_MONSTER_BUFFERID:
					if (self is Monster)
					{
						int num26 = (int)actionParams[0];
						int bufferID = (int)actionParams[1];
						List<object> list2 = GameManager.MonsterMgr.FindMonsterByExtensionID((self as Monster).CurrentCopyMapID, num26);
						if (list2 != null && list2.Count > 0)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								Global.RemoveMonsterBufferData(list2[i] as Monster, bufferID);
							}
						}
					}
					break;
				case MagicActionIDs.UP_LEVEL:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num104 = 0;
						int num99 = (int)actionParams[0];
						bool flag10 = true;
						if (num99 > 0)
						{
							if (gameClient.ClientData.ChangeLifeCount > GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
							{
								flag10 = false;
							}
							else if (gameClient.ClientData.ChangeLifeCount == GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
							{
								ChangeLifeDataInfo changeLifeDataInfo = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(gameClient, 0);
								if (changeLifeDataInfo == null)
								{
									flag10 = false;
								}
								else
								{
									num104 = changeLifeDataInfo.NeedLevel;
									if (gameClient.ClientData.Level >= num104)
									{
										flag10 = false;
									}
								}
							}
							else
							{
								ChangeLifeDataInfo changeLifeDataInfo = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(gameClient, gameClient.ClientData.ChangeLifeCount + 1);
								if (changeLifeDataInfo == null)
								{
									flag10 = false;
								}
								else
								{
									num104 = changeLifeDataInfo.NeedLevel;
									if (gameClient.ClientData.Level >= num104)
									{
										flag10 = false;
									}
								}
							}
							if (!flag10)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(64, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
								return false;
							}
							if (gameClient.ClientData.Level + num99 > num104)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(65, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
								return false;
							}
							for (int i = 0; i < num99; i++)
							{
								GameManager.ClientMgr.ProcessRoleExperience(gameClient, Global.GetCurrentLevelUpNeedExp(gameClient), true, true, false, "none");
							}
						}
					}
					break;
				case MagicActionIDs.ADD_GUANGMUI:
					if (self is Monster)
					{
						int guangMuID = (int)actionParams[0];
						int num105 = (int)actionParams[1];
						Monster monster = self as Monster;
						if (null != monster)
						{
							if (Global.GetMapSceneType(monster.CurrentMapCode) == 24)
							{
								LuoLanChengZhanManager.getInstance().AddGuangMuEvent(guangMuID, 1);
								GameManager.ClientMgr.BroadSpecialMapAIEvent(num105, self.CurrentCopyMapID, guangMuID, 1);
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(self.CurrentCopyMapID);
								if (null != copyMap)
								{
									copyMap.AddGuangMuEvent(guangMuID, 1);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(num105, self.CurrentCopyMapID, guangMuID, 1);
								}
							}
						}
					}
					break;
				case MagicActionIDs.CLEAR_GUANGMUI:
					if (self is Monster)
					{
						int guangMuID = (int)actionParams[0];
						int num105 = (int)actionParams[1];
						Monster monster = self as Monster;
						if (null != monster)
						{
							if (Global.GetMapSceneType(monster.CurrentMapCode) == 24)
							{
								LuoLanChengZhanManager.getInstance().AddGuangMuEvent(guangMuID, 0);
								GameManager.ClientMgr.BroadSpecialMapAIEvent(num105, self.CurrentCopyMapID, guangMuID, 0);
							}
							else
							{
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(self.CurrentCopyMapID);
								if (null != copyMap)
								{
									copyMap.AddGuangMuEvent(guangMuID, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(num105, self.CurrentCopyMapID, guangMuID, 0);
								}
							}
						}
					}
					break;
				case MagicActionIDs.FEIXUE:
				case MagicActionIDs.ZHONGDU:
				case MagicActionIDs.LINGHUN:
				case MagicActionIDs.RANSHAO:
				{
					double[] array2 = new double[4];
					BufferItemTypes bufferItemTypes = BufferItemTypes.None;
					if (MagicActionIDs.FEIXUE == id)
					{
						bufferItemTypes = BufferItemTypes.TimeFEIXUENoShow;
					}
					else if (MagicActionIDs.ZHONGDU == id)
					{
						bufferItemTypes = BufferItemTypes.TimeZHONGDUNoShow;
					}
					else if (MagicActionIDs.LINGHUN == id)
					{
						bufferItemTypes = BufferItemTypes.TimeLINGHUNoShow;
					}
					else if (MagicActionIDs.RANSHAO == id)
					{
						bufferItemTypes = BufferItemTypes.TimeRANSHAONoShow;
					}
					int addVlue = 0;
					int num82 = skillLevel + 1;
					long num92 = (long)(actionParams[0] * 1000.0);
					int execCount = (int)actionParams[1];
					double num20 = actionParams[0] * actionParams[1];
					double num93 = actionParams[2] + actionParams[2] / 200.0 * (double)num82;
					if (id != MagicActionIDs.RANSHAO)
					{
						addVlue = (int)actionParams[3] + (int)actionParams[3] * num82;
					}
					int num22 = 0;
					int burst = 0;
					int num5;
					if (self is GameClient)
					{
						num5 = Global.GetAttackType(self as GameClient);
						if (id != MagicActionIDs.RANSHAO)
						{
							if (0 == num5)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == num5 || 2 == num5)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as GameClient, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
						}
						else if (obj is GameClient)
						{
							num22 = (int)((double)(obj as GameClient).ClientData.LifeV * num93);
						}
						else if (obj is Monster)
						{
							num22 = (int)((obj as Monster).MonsterInfo.VLifeMax * num93);
						}
						int roleID = (self as GameClient).ClientData.RoleID;
						array2[0] = num20;
						array2[1] = actionParams[0];
						array2[2] = (double)num22;
						array2[3] = (double)roleID;
					}
					else if (self is Monster)
					{
						num5 = (self as Monster).MonsterInfo.ToOccupation;
						if (id != MagicActionIDs.RANSHAO)
						{
							if (num5 == 0 || 2 == num5)
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.AttackEnemy(self as Monster, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.AttackEnemy(self as Monster, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
							else if (1 == num5 || (5 == num5 && skillid == 11006))
							{
								if (obj is GameClient)
								{
									RoleAlgorithm.MAttackEnemy(self as Monster, obj as GameClient, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
								else if (obj is Monster)
								{
									RoleAlgorithm.MAttackEnemy(self as Monster, obj as Monster, false, 1.0, 0, 1.0, 0, 0, out burst, out num22, false, num93, addVlue, skillid, shenShiInjurePercent);
								}
							}
						}
						else if (obj is GameClient)
						{
							num22 = (int)((double)(obj as GameClient).ClientData.LifeV * num93);
						}
						else if (obj is Monster)
						{
							num22 = (int)((obj as Monster).MonsterInfo.VLifeMax * num93);
						}
						int roleID = (self as Monster).RoleID;
						array2[0] = num20;
						array2[1] = actionParams[0];
						array2[2] = (double)num22;
						array2[3] = (double)roleID;
					}
					num5 = -1;
					if (self is GameClient)
					{
						if (obj is GameClient)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, false, num93, 0, 0, 0, 0.0);
						}
						else if (obj is Monster)
						{
							GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
						}
						else if (obj is BiaoCheItem)
						{
							BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
						}
						else if (obj is FakeRoleItem)
						{
							FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, num93, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, burst, num22, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, true, num93, 0, 0, 0, 0.0);
					}
					else if (!(obj is BiaoCheItem))
					{
						if (obj is JunQiItem)
						{
							JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as JunQiItem, burst, num22, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					if (num22 > 0)
					{
						if (obj is GameClient)
						{
							Global.UpdateBufferData(obj as GameClient, bufferItemTypes, array2, 1, true);
							MagicAction.ZhongDuContextData context2 = new MagicAction.ZhongDuContextData
							{
								Attacker = self.GetObjectID(),
								Self = obj,
								Injure = (int)array2[2],
								Occupation = (obj as GameClient).ClientData.Occupation,
								percent = num93
							};
							(obj as GameClient).TimedActionMgr.AddItem(TimeUtil.NOW() + num92, num92, execCount, 0, new Action<long, object>(MagicAction.ZhongDuActionProc), context2);
							(obj as GameClient).ClientData.ZhongDuStart = TimeUtil.NOW();
							(obj as GameClient).ClientData.ZhongDuSeconds = (int)num20;
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as GameClient, 1, TimeUtil.NOW(), (int)num20, 0.0);
						}
						else if (obj is Monster)
						{
							Global.UpdateMonsterBufferData(obj as Monster, bufferItemTypes, array2);
							MagicAction.ZhongDuContextData context2 = new MagicAction.ZhongDuContextData
							{
								Attacker = self.GetObjectID(),
								Self = obj,
								Injure = (int)array2[2],
								percent = num93
							};
							(obj as Monster).TimedActionMgr.AddItem(TimeUtil.NOW() + num92, num92, execCount, 0, new Action<long, object>(MagicAction.ZhongDuActionProc), context2);
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, 1, TimeUtil.NOW(), (int)num20, 0.0);
						}
					}
					break;
				}
				case MagicActionIDs.HUZHAO:
				{
					BufferItemTypes bufferItemTypes = BufferItemTypes.TimeHUZHAONoShow;
					double[] array2 = new double[]
					{
						actionParams[1]
					};
					if (obj is GameClient)
					{
						Global.UpdateBufferData(obj as GameClient, bufferItemTypes, array2, 1, true);
						(obj as GameClient).MyBufferExtManager.AddBufferItem((int)bufferItemTypes, new HuZhaoBufferItem
						{
							InjuredV = (int)actionParams[0],
							MaxLifeV = (int)actionParams[1],
							RecoverLifePercent = actionParams[2]
						});
					}
					else if (obj is Monster)
					{
						Global.UpdateMonsterBufferData(obj as Monster, bufferItemTypes, array2);
						(obj as Monster).MyBufferExtManager.AddBufferItem((int)bufferItemTypes, new HuZhaoBufferItem
						{
							InjuredV = (int)actionParams[0],
							MaxLifeV = (int)actionParams[1],
							RecoverLifePercent = actionParams[2]
						});
					}
					break;
				}
				case MagicActionIDs.WUDIHUZHAO:
				{
					BufferItemTypes bufferItemTypes = BufferItemTypes.TimeWUDIHUZHAONoShow;
					double[] array2 = new double[]
					{
						actionParams[0]
					};
					if (obj is GameClient)
					{
						Global.UpdateBufferData(obj as GameClient, bufferItemTypes, array2, 1, true);
					}
					else if (obj is Monster)
					{
						Global.UpdateMonsterBufferData(obj as Monster, bufferItemTypes, array2);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL1:
				case MagicActionIDs.MU_FIRE_WALL9:
				case MagicActionIDs.MU_FIRE_WALL25:
				{
					double[] array2 = new double[5];
					int num106 = 0;
					if (self is GameClient)
					{
						num106 = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						num106 = monster.RoleID;
					}
					array2[0] = actionParams[0] * actionParams[1];
					array2[1] = actionParams[1];
					array2[2] = actionParams[3];
					array2[3] = (double)num106;
					array2[4] = actionParams[2];
					int num107 = 0;
					if (id != MagicActionIDs.MU_FIRE_WALL1)
					{
						if (id == MagicActionIDs.MU_FIRE_WALL9)
						{
							num107 = 1;
						}
						else if (id == MagicActionIDs.MU_FIRE_WALL25)
						{
							num107 = 2;
						}
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int num18 = targetX / gameMap.MapGridWidth;
					int num19 = targetY / gameMap.MapGridHeight;
					if (num18 > 0 && num19 > 0)
					{
						GameManager.GridMagicHelperMgr.AddMagicHelper(id, array2, self.CurrentMapCode, new Point((double)num18, (double)num19), num107, num107, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_X:
				case MagicActionIDs.MU_FIRE_SECTOR:
				case MagicActionIDs.MU_FIRE_STRAIGHT:
				{
					double[] array2 = new double[16];
					int num106 = 0;
					if (self is GameClient)
					{
						num106 = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						num106 = monster.RoleID;
					}
					int num82 = skillLevel + 1;
					array2[0] = actionParams[0] * actionParams[1];
					array2[1] = actionParams[1];
					array2[2] = actionParams[3] * (double)(1 + num82);
					array2[3] = (double)num106;
					array2[4] = actionParams[2] * (1.0 + (double)num82 / 200.0);
					if (id == MagicActionIDs.MU_FIRE_WALL_X)
					{
						array2[5] = actionParams[4];
					}
					else if (id == MagicActionIDs.MU_FIRE_SECTOR)
					{
						array2[5] = actionParams[4];
						array2[6] = actionParams[5];
						array2[7] = (double)self.CurrentDir;
					}
					else if (id == MagicActionIDs.MU_FIRE_STRAIGHT)
					{
						array2[5] = actionParams[4];
						array2[6] = actionParams[5];
						array2[7] = (double)targetX;
						array2[8] = (double)targetY;
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int num18 = targetX / gameMap.MapGridWidth;
					int num19 = targetY / gameMap.MapGridHeight;
					array2[15] = (double)gameMap.MapGridWidth;
					if (num18 > 0 && num19 > 0)
					{
						GameManager.GridMagicHelperMgrEx.AddMagicHelperEx(id, array2, self.CurrentMapCode, num18, num19, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_Y:
				{
					double[] array2 = new double[17];
					int num106 = 0;
					if (self is GameClient)
					{
						num106 = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						num106 = monster.RoleID;
					}
					int num82 = skillLevel + 1;
					array2[0] = actionParams[0];
					array2[1] = actionParams[1];
					array2[2] = actionParams[3] * (double)(1 + num82);
					array2[3] = (double)num106;
					array2[4] = actionParams[2] * (1.0 + (double)num82 / 200.0);
					Array.Copy(actionParams, 4, array2, 5, actionParams.Length - 4);
					int delayDecoration = 0;
					SystemXmlItem systemXmlItem2 = null;
					if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillid, out systemXmlItem2))
					{
						delayDecoration = systemXmlItem2.GetIntValue("DelayDecoration", -1);
					}
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int num18 = targetX / gameMap.MapGridWidth;
					int num19 = targetY / gameMap.MapGridHeight;
					array2[15] = (double)gameMap.MapGridWidth;
					array2[16] = (double)skillid;
					if (num18 > 0 && num19 > 0)
					{
						double num108 = array2[0] * array2[1];
						GameManager.GridMagicHelperMgrEx.AddGridMagic(id, array2, self.CurrentMapCode, num18, num19, delayDecoration, (int)num108, self.CurrentCopyMapID, MagicAction.MaxHitNum);
					}
					break;
				}
				case MagicActionIDs.MU_FIRE_WALL_ACTION:
				{
					double[] array2 = new double[actionParams.Length + 1];
					int num106 = 0;
					if (self is GameClient)
					{
						num106 = (self as GameClient).ClientData.RoleID;
					}
					else if (self is Monster)
					{
						Monster monster = self as Monster;
						num106 = monster.RoleID;
					}
					array2[0] = actionParams[0] * actionParams[1];
					array2[1] = actionParams[1];
					int gridWidthNum = (int)actionParams[2];
					int gridHeightNum = (int)actionParams[3];
					array2[2] = (double)num106;
					Array.Copy(actionParams, 4, array2, 3, actionParams.Length - 4);
					GameMap gameMap = GameManager.MapMgr.DictMaps[self.CurrentMapCode];
					int num18 = targetX / gameMap.MapGridWidth;
					int num19 = targetY / gameMap.MapGridHeight;
					if (num18 > 0 && num19 > 0)
					{
						GameManager.GridMagicHelperMgrEx.AddMagicHelperExAction(id, array2, self.CurrentMapCode, new Point((double)num18, (double)num19), gridWidthNum, gridHeightNum, self.CurrentCopyMapID);
					}
					break;
				}
				case MagicActionIDs.BOSS_ADDANIMATION:
				{
					int num105 = self.CurrentMapCode;
					int currentCopyMapID = self.CurrentCopyMapID;
					int num109 = (int)actionParams[0];
					int num110 = (int)actionParams[2];
					int num111 = (int)actionParams[3];
					int num112 = (int)actionParams[4];
					int num113 = (int)actionParams[5];
					int num114 = (int)actionParams[6];
					long num27 = TimeUtil.NOW() / 10000L;
					int bossAnimationCheckCode = Global.GetBossAnimationCheckCode(num109, num105, num111, num112, num113, num114, num27);
					string text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						-1,
						num109,
						num105,
						num111,
						num112,
						num113,
						num114,
						num27,
						bossAnimationCheckCode
					});
					GameManager.ClientMgr.BroadSpecialMapMessage(639, text, num105, currentCopyMapID);
					break;
				}
				case MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN:
				{
					double num115 = actionParams[0];
					double num74 = actionParams[1];
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					if (obj is GameClient)
					{
						GameClient gameClient2 = obj as GameClient;
						if (gameClient2 != null)
						{
							if (!gameClient2.buffManager.IsBuffEnabled(116) && !gameClient2.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(gameClient2))
								{
									ZuoQiManager.getInstance().RoleDisMount(gameClient2, true);
									gameClient2.RoleBuffer.AddTempExtProp(2, -num115, num70);
									gameClient2.buffManager.SetStatusBuff(118, TimeUtil.NOW(), (long)num74 * 1000L, 0L);
									double num84 = RoleAlgorithm.GetMoveSpeed(gameClient2);
									gameClient2.ClientData.MoveSpeed = num84;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, 4, TimeUtil.NOW(), (int)num74, num84);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							monster.TempPropsBuffer.AddTempExtProp(2, -num115, num70);
							monster.SpeedDownStart = TimeUtil.NOW();
							monster.SpeedDownSeconds = (int)num74;
							GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 4, TimeUtil.NOW(), (int)num74, monster.MoveSpeed);
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_PALSY:
				{
					double num115 = actionParams[0];
					double num74 = actionParams[1];
					if (obj is GameClient)
					{
						GameClient gameClient2 = obj as GameClient;
						if (gameClient2 != null)
						{
							if (!gameClient2.buffManager.IsBuffEnabled(116) && !gameClient2.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(gameClient2))
								{
									ZuoQiManager.getInstance().RoleDisMount(gameClient2, true);
									gameClient2.ClientData.DongJieStart = TimeUtil.NOW();
									gameClient2.ClientData.DongJieSeconds = (int)num74;
									long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
									gameClient2.RoleBuffer.AddTempExtProp(2, -num115, num70);
									gameClient2.buffManager.SetStatusBuff(117, TimeUtil.NOW(), (long)num74 * 1000L, 0L);
									double num84 = RoleAlgorithm.GetMoveSpeed(gameClient2);
									gameClient2.ClientData.MoveSpeed = num84;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, 3, gameClient2.ClientData.DongJieStart, gameClient2.ClientData.DongJieSeconds, num84);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.DongJieStart = TimeUtil.NOW();
								monster.DongJieSeconds = (int)num74;
								monster.XuanYunStart = TimeUtil.NOW();
								monster.XuanYunSeconds = (int)num74;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 3, monster.DongJieStart, monster.DongJieSeconds, 0.0);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_ADD_FROZEN:
				{
					double num115 = actionParams[0];
					double num74 = actionParams[1];
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					long startTicks = TimeUtil.NOW();
					if (obj is GameClient)
					{
						GameClient gameClient2 = obj as GameClient;
						if (gameClient2 != null)
						{
							if (!gameClient2.buffManager.IsBuffEnabled(116) && !gameClient2.buffManager.IsBuffEnabled(113))
							{
								if (!CaiJiLogic.IsCaiJiState(gameClient2))
								{
									ZuoQiManager.getInstance().RoleDisMount(gameClient2, true);
									gameClient2.RoleBuffer.AddTempExtProp(47, 1.0, num70);
									double num84 = RoleAlgorithm.GetMoveSpeed(gameClient2);
									gameClient2.ClientData.MoveSpeed = num84;
									GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, 8, startTicks, (int)num74, num84);
								}
							}
						}
					}
					else if (obj is Monster)
					{
						Monster monster = obj as Monster;
						if (null != monster)
						{
							if (101 == monster.MonsterType || 301 == monster.MonsterType || 1501 == monster.MonsterType || 1801 == monster.MonsterType)
							{
								monster.TempPropsBuffer.AddTempExtProp(2, -monster.MoveSpeed, num70);
								monster.DingShenStart = TimeUtil.NOW();
								monster.DingShenSeconds = (int)num74;
								GameManager.ClientMgr.NotifyMonsterStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, 8, startTicks, (int)num74, monster.MoveSpeed);
							}
						}
					}
					break;
				}
				case MagicActionIDs.MU_GETSHIZHUANG:
				{
					int nFashionID = (int)actionParams[0];
					FashionManager.getInstance().GetFashionByMagic(self as GameClient, nFashionID, true);
					break;
				}
				case MagicActionIDs.MU_ADD_BATI:
				{
					int num82 = skillLevel + 1;
					double num116 = actionParams[0];
					if (self is GameClient && num82 >= 30)
					{
						GameClient gameClient3 = self as GameClient;
						if (!gameClient3.buffManager.IsBuffEnabled(113))
						{
							long num70 = (long)(num116 * 1.0);
							gameClient3.buffManager.SetStatusBuff(113, TimeUtil.NOW(), num70, (long)num82);
						}
					}
					break;
				}
				case MagicActionIDs.ADD_SHENGWU:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						sbyte b2 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_NUM + 1));
						sbyte b3 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string text2 = HolyItemManager.SliceNameSet[(int)b2, (int)b3];
						int num95 = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(gameClient, b2, b3, num95);
					}
					break;
				case MagicActionIDs.ADD_SHENGBEI:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						sbyte b3 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string text2 = HolyItemManager.SliceNameSet[1, (int)b3];
						int num95 = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(gameClient, 1, b3, num95);
					}
					break;
				case MagicActionIDs.ADD_SHENGJIAN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						sbyte b3 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string text2 = HolyItemManager.SliceNameSet[2, (int)b3];
						int num95 = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(gameClient, 2, b3, num95);
					}
					break;
				case MagicActionIDs.ADD_SHENGGUAN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						sbyte b3 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string text2 = HolyItemManager.SliceNameSet[3, (int)b3];
						int num95 = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(gameClient, 3, b3, num95);
					}
					break;
				case MagicActionIDs.ADD_SHENGDIAN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						sbyte b3 = (sbyte)Global.GetRandomNumber(1, (int)(HolyItemManager.MAX_HOLY_PART_NUM + 1));
						string text2 = HolyItemManager.SliceNameSet[4, (int)b3];
						int num95 = Global.GetRandomNumber((int)actionParams[0], (int)actionParams[1] + 1);
						HolyItemManager.getInstance().GetHolyItemPart(gameClient, 4, b3, num95);
					}
					break;
				case MagicActionIDs.WOLF_SEARCH_ROAD:
					if (self is Monster)
					{
						Monster monster = self as Monster;
						Point firstCoordinate = monster.FirstCoordinate;
						int num117 = (int)actionParams[0];
						int num118 = (int)actionParams[1];
						int maxV3 = Math.Min(3, monster.AttackRange / 100);
						if (firstCoordinate.X + 1000.0 < (double)num117)
						{
							num117 -= monster.AttackRange - 100;
							int randomNumber4 = Global.GetRandomNumber(0, maxV3);
							num118 -= randomNumber4 * 100 - 100;
						}
						else if (firstCoordinate.X - 1000.0 > (double)num117)
						{
							num117 += monster.AttackRange - 100;
							int randomNumber4 = Global.GetRandomNumber(0, maxV3);
							num118 -= randomNumber4 * 100 - 100;
						}
						else
						{
							num118 -= monster.AttackRange - 100;
							int[] array4 = new int[]
							{
								-1,
								0,
								1
							};
							int randomNumber4 = Global.GetRandomNumber(0, 3);
							num117 += array4[randomNumber4] * 100;
						}
						Point startPoint = new Point((double)num117, (double)num118);
						List<int[]> patrolPath = GlobalNew.FindPath(startPoint, firstCoordinate, monster.CurrentMapCode);
						monster.PatrolPath = patrolPath;
						monster.Direction = (double)Global.GetRandomNumber(0, 8);
						monster.IsAutoSearchRoad = true;
					}
					break;
				case MagicActionIDs.WOLF_ATTACK_ROLE:
					if (self is Monster)
					{
						Monster monster = self as Monster;
						bool isAttackRole = (int)actionParams[0] > 0;
						monster.IsAttackRole = isAttackRole;
					}
					break;
				case MagicActionIDs.SELF_BURST:
					if (self is Monster)
					{
						Global.SystemKillMonster(self as Monster);
					}
					break;
				case MagicActionIDs.MU_ADD_PROPERTY:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						long num119 = TimeUtil.NOW();
						gameClient.bufferPropsManager.UpdateTimedPropsData(num119, num119, (int)actionParams[2] * 1000, 1, (int)actionParams[0], actionParams[1] * (double)(skillLevel + 1), (int)actionParams[3], 0);
						if (RoleAlgorithm.NeedNotifyClient((ExtPropIndexes)actionParams[0]))
						{
							gameClient.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
							{
								2
							});
						}
					}
					break;
				case MagicActionIDs.ADD_GAIMING:
				{
					int count = (int)actionParams[0];
					SingletonTemplate<NameManager>.Instance().GM_SetFreeModName((self as GameClient).ClientData.RoleID, count);
					break;
				}
				case MagicActionIDs.MU_ADD_DAMAGETHORN:
				{
					double num85 = actionParams[0];
					double num86 = actionParams[1];
					double num74 = actionParams[2];
					int num82 = skillLevel + 1;
					double num87 = num85 + num85 / 200.0 * (double)num82;
					double num88 = num86 + num86 * (double)num82;
					long num70 = TimeUtil.NOW() * 10000L + (long)num74 * 1000L * 10000L;
					double[] array = new double[]
					{
						num74,
						(double)skillid,
						(double)num82
					};
					if (obj is GameClient)
					{
						(obj as GameClient).RoleBuffer.AddTempExtProp(29, num87, num70);
						(obj as GameClient).RoleBuffer.AddTempExtProp(30, num88, num70);
					}
					else
					{
						(self as GameClient).RoleBuffer.AddTempExtProp(29, num87, num70);
						(self as GameClient).RoleBuffer.AddTempExtProp(30, num88, num70);
					}
					Global.UpdateBufferData(obj as GameClient, BufferItemTypes.MU_ADD_DAMAGE_THORN_PERCENT, array, 1, true);
					break;
				}
				case MagicActionIDs.MU_ADD_VAMPIRE:
				{
					double num85 = actionParams[0];
					double num86 = actionParams[1];
					double num120 = actionParams[2];
					int num29 = 0;
					int num82 = skillLevel + 1;
					num85 += num85 / 200.0 * (double)num82;
					num86 += num86 * (double)num82;
					int num5 = 1;
					if (self is GameClient)
					{
						num5 = Global.GetAttackType(self as GameClient);
						if (obj is GameClient)
						{
							num29 = GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, false, num85, (int)num86, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is Monster)
						{
							num29 = GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num85, (int)num86, 0, skillid, shenShiInjurePercent);
						}
						else if (obj is BiaoCheItem)
						{
							num29 = BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as BiaoCheItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						else if (obj is JunQiItem)
						{
							num29 = JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as JunQiItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
						if (num29 > 0 && (self as GameClient).ClientData.CurrentLifeV > 0)
						{
							double lifeV = (double)num29 * num120;
							GameManager.ClientMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, lifeV, "吸血攻击， 脚本" + id.ToString());
						}
						else if (obj is FakeRoleItem)
						{
							num29 = FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as GameClient, obj as FakeRoleItem, 0, 0, manyRangeInjuredPercent, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
						}
					}
					else if (obj is GameClient)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as GameClient, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, num82, 0.0, 0.0, false, num85, (int)num86, 0, skillid, shenShiInjurePercent);
					}
					else if (obj is Monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self as Monster, obj as Monster, 0, 0, manyRangeInjuredPercent, num5, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, num85, (int)num86, 0, skillid, shenShiInjurePercent);
					}
					break;
				}
				case MagicActionIDs.BUY_YUEKA:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						YueKaManager.HandleUserBuyYueKa(gameClient.strUserID, gameClient.ClientData.RoleID);
					}
					break;
				case MagicActionIDs.SET_WING:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num121 = (int)actionParams[0];
						int num122 = (int)actionParams[1];
						string text3 = string.Format("-setwingsuitstar {0} {1}", num121, num122);
						GameManager.systemGMCommands.GMSetWingSuitStar(gameClient, text3.Split(new char[]
						{
							' '
						}));
					}
					break;
				case MagicActionIDs.SET_MAX_WING:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						string text3 = string.Format("-setwingsuitstar {0} {1}", MUWingsManager.MaxWingID, MUWingsManager.MaxWingEnchanceLevel);
						GameManager.systemGMCommands.GMSetWingSuitStar(gameClient, text3.Split(new char[]
						{
							' '
						}));
						LingYuManager.SetLingYuMax_GM(gameClient);
						ZhuLingZhuHunManager.SetZhuLingMax_GM(gameClient);
						ZhuLingZhuHunManager.SetZhuHunMax_GM(gameClient);
					}
					break;
				case MagicActionIDs.SET_ALLGOODS_LV:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int forgelev = (int)actionParams[0];
						List<int> list3 = new List<int>();
						for (int i = 0; i <= 6; i++)
						{
							list3.Add(i);
						}
						for (int i = 11; i <= 21; i++)
						{
							list3.Add(i);
						}
						List<GoodsData> goodsByCategoriyList = gameClient.UsingEquipMgr.GetGoodsByCategoriyList(list3);
						if (null != goodsByCategoriyList)
						{
							foreach (GoodsData goodsData in goodsByCategoriyList)
							{
								GameManager.systemGMCommands.GMSetGoodsForgeLevel(gameClient, goodsData, forgelev, 1, false);
							}
							Global.RefreshEquipProp(gameClient);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, true, false, 7);
						}
					}
					break;
				case MagicActionIDs.SET_GOODS_LV:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int forgelev = (int)actionParams[0];
						int itemCat = (int)actionParams[1];
						List<int> list3 = new List<int>();
						if (1 <= itemCat && itemCat <= 5)
						{
							list3.Add(itemCat - 1);
						}
						else if (6 == itemCat || 7 == itemCat)
						{
							for (int i = 11; i <= 21; i++)
							{
								list3.Add(i);
							}
						}
						else if (8 == itemCat || 9 == itemCat)
						{
							list3.Add(6);
						}
						else if (10 == itemCat)
						{
							list3.Add(5);
						}
						List<GoodsData> goodsByCategoriyList = gameClient.UsingEquipMgr.GetGoodsByCategoriyList(list3);
						if (goodsByCategoriyList != null && goodsByCategoriyList.Count > 0)
						{
							if ((1 <= itemCat && itemCat <= 5) || 10 == itemCat)
							{
								GameManager.systemGMCommands.GMSetGoodsForgeLevel(gameClient, goodsByCategoriyList[0], forgelev, 1, true);
							}
							else if (6 == itemCat || 7 == itemCat)
							{
								GoodsData goodsData = goodsByCategoriyList.Find(delegate(GoodsData x)
								{
									int num126 = -1;
									int num127 = -1;
									SystemXmlItem systemXmlItem4 = null;
									if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(x.GoodsID, out systemXmlItem4))
									{
										num126 = systemXmlItem4.GetIntValue("HandType", -1);
										num127 = systemXmlItem4.GetIntValue("ActionType", -1);
									}
									return (6 == itemCat && num126 == 1) || (6 == itemCat && num126 == 2 && x.BagIndex == 0) || (7 == itemCat && num126 == 0) || (7 == itemCat && num126 == 2 && x.BagIndex == 1) || (num127 != 1 && num127 != 4);
								});
								if (null != goodsData)
								{
									GameManager.systemGMCommands.GMSetGoodsForgeLevel(gameClient, goodsData, forgelev, 1, true);
								}
							}
							else if (8 == itemCat || 9 == itemCat)
							{
								GoodsData goodsData;
								if (8 == itemCat)
								{
									goodsData = goodsByCategoriyList.Find((GoodsData x) => x.BagIndex == 0);
								}
								else
								{
									goodsData = goodsByCategoriyList.Find((GoodsData x) => x.BagIndex == 1);
								}
								if (null != goodsData)
								{
									GameManager.systemGMCommands.GMSetGoodsForgeLevel(gameClient, goodsData, forgelev, 1, true);
								}
							}
						}
					}
					break;
				case MagicActionIDs.SET_MAX_GUOSHI:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int fruitAddPropLimit2 = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Strength");
						int fruitAddPropLimit3 = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Intelligence");
						int fruitAddPropLimit4 = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Dexterity");
						int fruitAddPropLimit5 = UseFruitVerify.GetFruitAddPropLimit(gameClient, "Constitution");
						int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(gameClient, "PropStrengthChangeless");
						int roleParamsInt32FromDB3 = Global.GetRoleParamsInt32FromDB(gameClient, "PropIntelligenceChangeless");
						int roleParamsInt32FromDB4 = Global.GetRoleParamsInt32FromDB(gameClient, "PropDexterityChangeless");
						int roleParamsInt32FromDB5 = Global.GetRoleParamsInt32FromDB(gameClient, "PropConstitutionChangeless");
						int num123 = fruitAddPropLimit2 - roleParamsInt32FromDB2;
						double[] actionParams2 = new double[]
						{
							1.0,
							(double)num123,
							(double)num123
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_STRENGTH, actionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						num123 = fruitAddPropLimit3 - roleParamsInt32FromDB3;
						actionParams2 = new double[]
						{
							1.0,
							(double)num123,
							(double)num123
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_INTELLIGENCE, actionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						num123 = fruitAddPropLimit4 - roleParamsInt32FromDB4;
						actionParams2 = new double[]
						{
							1.0,
							(double)num123,
							(double)num123
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_DEXTERITY, actionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
						num123 = fruitAddPropLimit5 - roleParamsInt32FromDB5;
						actionParams2 = new double[]
						{
							1.0,
							(double)num123,
							(double)num123
						};
						MagicAction.ProcessAction(self, null, MagicActionIDs.MU_RANDOM_CONSTITUTION, actionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, true, false, 1.0, 1, 0.0);
					}
					break;
				case MagicActionIDs.SET_MAX_XINGHUN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						GameManager.StarConstellationMgr.ActivationStarConstellationAll(gameClient);
					}
					break;
				case MagicActionIDs.SET_MEILIN:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int nLevel = (int)actionParams[0];
						int nStarNum = (int)actionParams[1];
						GameManager.MerlinMagicBookMgr.GMMerlinLevelUpToN(gameClient, nLevel);
						GameManager.MerlinMagicBookMgr.GMMerlinStarUpToN(gameClient, nStarNum);
					}
					break;
				case MagicActionIDs.SET_MAINTASK:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int taskID = (int)actionParams[0];
						ProcessTask.GMSetMainTaskID(gameClient, taskID);
					}
					break;
				case MagicActionIDs.SET_LEVEL:
					if (self is GameClient)
					{
						GameClient gameClient = self as GameClient;
						int num124 = (int)actionParams[0];
						int num125 = (int)actionParams[1];
						string text3 = string.Format("-setlevel {0} {1} {2}", Global.FormatRoleName4(gameClient), num124, num125);
						GameManager.systemGMCommands.GMSetLevel(gameClient, text3.Split(new char[]
						{
							' '
						}));
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP:
					if (self is GameClient)
					{
						RebornManager.getInstance().ProcessRoleExperience(self as GameClient, (long)actionParams[0], MoneyTypes.RebornExp, true, true, false, "none");
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP_MONSTERS_MAX:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRebornExpMaxAddValue(self as GameClient, (long)((int)actionParams[0]), "道具脚本", MoneyTypes.RebornExpMonster, false, true, false);
					}
					break;
				case MagicActionIDs.DB_ADD_REBORNEXP_GOODS_MAX:
					if (self is GameClient)
					{
						GameManager.ClientMgr.ModifyRebornExpMaxAddValue(self as GameClient, (long)((int)actionParams[0]), "道具脚本", MoneyTypes.RebornExpSale, false, true, false);
					}
					break;
				case MagicActionIDs.DB_ADD_MULTI_REBORNEXP:
					if (self is GameClient)
					{
						double[] array = new double[]
						{
							actionParams[1],
							(double)((long)actionGoodsID << 32 | (long)actionParams[0])
						};
						Global.UpdateBufferData(self as GameClient, BufferItemTypes.RebornMutilExp, array, 0, true);
					}
					break;
				}
				result = flag;
			}
			return result;
		}

		public static void ZhongDuActionProc(long execTicks, object state)
		{
			MagicAction.ZhongDuContextData zhongDuContextData = state as MagicAction.ZhongDuContextData;
			if (null != zhongDuContextData)
			{
				GameClient gameClient = zhongDuContextData.Self as GameClient;
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(zhongDuContextData.Attacker);
				Monster monster = zhongDuContextData.Self as Monster;
				Monster monster2 = null;
				IObject @object;
				if (null == gameClient2)
				{
					if (gameClient != null)
					{
						monster2 = GameManager.MonsterMgr.FindMonster(gameClient.CurrentMapCode, zhongDuContextData.Attacker);
					}
					else if (monster != null)
					{
						monster2 = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, zhongDuContextData.Attacker);
					}
					@object = monster2;
				}
				else
				{
					@object = gameClient2;
				}
				if (null != @object)
				{
					if (zhongDuContextData.Self.CurrentMapCode == @object.CurrentMapCode && zhongDuContextData.Self.CurrentCopyMapID == @object.CurrentCopyMapID)
					{
						if (gameClient != null)
						{
							if (gameClient2 != null)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, gameClient, 0, zhongDuContextData.Injure, 1.0, zhongDuContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, zhongDuContextData.percent, 0, 0, 0, 0.0);
							}
							else if (monster2 != null)
							{
								GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster2, gameClient, 0, zhongDuContextData.Injure, 1.0, zhongDuContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, zhongDuContextData.percent, 0, 0, 0, 0.0);
							}
						}
						else if (monster != null)
						{
							if (gameClient2 != null)
							{
								GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, monster, 0, zhongDuContextData.Injure, 1.0, zhongDuContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, zhongDuContextData.percent, 0, 0, 0, 0.0);
							}
							else if (monster2 != null)
							{
								GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster2, monster, 0, zhongDuContextData.Injure, 1.0, zhongDuContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, zhongDuContextData.percent, 0, 0, 0, 0.0);
							}
						}
					}
				}
			}
		}

		public static void ChenMoActionProc(long execTicks, object state)
		{
			MagicAction.ChenMoContextData chenMoContextData = state as MagicAction.ChenMoContextData;
			if (null != chenMoContextData)
			{
				if (!chenMoContextData.Stop)
				{
					GameClient gameClient = chenMoContextData.Self as GameClient;
					GameClient gameClient2 = GameManager.ClientMgr.FindClient(chenMoContextData.Attacker);
					Monster monster = chenMoContextData.Self as Monster;
					Monster monster2 = null;
					IObject @object;
					if (null == gameClient2)
					{
						if (gameClient != null)
						{
							monster2 = GameManager.MonsterMgr.FindMonster(gameClient.CurrentMapCode, chenMoContextData.Attacker);
						}
						else if (monster != null)
						{
							monster2 = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, chenMoContextData.Attacker);
						}
						@object = monster2;
					}
					else
					{
						@object = gameClient2;
					}
					if (null != @object)
					{
						if (chenMoContextData.Self.CurrentMapCode == @object.CurrentMapCode && chenMoContextData.Self.CurrentCopyMapID == @object.CurrentCopyMapID)
						{
							if (gameClient != null)
							{
								if (gameClient.buffManager.IsBuffEnabled(116))
								{
									chenMoContextData.Stop = true;
								}
								else if (gameClient2 != null)
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, gameClient, 0, chenMoContextData.Injure, 1.0, chenMoContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, true, chenMoContextData.percent, 0, 0, 0, 0.0);
								}
								else if (monster2 != null)
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster2, gameClient, 0, chenMoContextData.Injure, 1.0, chenMoContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, chenMoContextData.percent, 0, 0, 0, 0.0);
								}
							}
							else if (monster != null)
							{
								if (gameClient2 != null)
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient2, monster, 0, chenMoContextData.Injure, 1.0, chenMoContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, chenMoContextData.percent, 0, 0, 0, 0.0);
								}
								else if (monster2 != null)
								{
									GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster2, monster, 0, chenMoContextData.Injure, 1.0, chenMoContextData.Occupation, false, 0, 0.0, 0, 0, 0, 0.0, 0.0, false, chenMoContextData.percent, 0, 0, 0, 0.0);
								}
							}
						}
					}
				}
			}
		}

		[ThreadStatic]
		public static int MaxHitNum;

		public class ZhongDuContextData
		{
			public IObject Self;

			public int Attacker;

			public int Injure;

			public int Occupation;

			public double percent;
		}

		public class ChenMoContextData
		{
			public IObject Self;

			public int Attacker;

			public int Injure;

			public int Occupation;

			public bool Stop;

			public double percent;
		}
	}
}
