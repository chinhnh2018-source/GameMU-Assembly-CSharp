using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteDataManager
{
	public static void Clear()
	{
		TeamCompeteDataManager.ZhengBaRequestFlag = 0L;
		TeamCompeteDataManager.HaveMonthPaiHangAwards = 0;
		TeamCompeteDataManager.TodayFightCount = 0;
		TeamCompeteDataManager.MainZhanDuiData = null;
		TeamCompeteDataManager.TianTi5v5PiPeiStateData = null;
		TeamCompeteDataManager.DictTouXiangPath.Clear();
		TeamCompeteDataManager.DictCircleTouXiangPath.Clear();
		TeamCompeteDataManager.RefreshJingCaiDianCallBack = null;
	}

	public static void SendInviteTeanMemberMsg(int roleId)
	{
		if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.IsInShiLiZhengBaMap() || Global.IsInShiLiZhengBaBattleMap())
		{
			Super.HintMainText(Global.GetLang("跨服地图不能使用此功能"), 10, 3);
			return;
		}
		GameInstance.Game.SendInviteTeamMemberMsg(roleId.ToString(), null);
	}

	public static void ClearInivteDatas()
	{
		if (TeamCompeteDataManager.InviteDatas.Count > 0)
		{
			TeamCompeteDataManager.InviteDatas.Clear();
		}
		ActivityTipManager.SetActivityTipItemActive(3035, TeamCompeteDataManager.InviteDatas.Count > 0);
	}

	public static void Invite(bool isOpenInvite = false)
	{
		if (isOpenInvite && TeamCompeteDataManager.InviteDatas.Count > 0)
		{
			TeamCompeteDataManager.InviteDatas.RemoveAt(0);
		}
		ActivityTipManager.SetActivityTipItemActive(3035, TeamCompeteDataManager.InviteDatas.Count > 0);
	}

	public static void ClearRequestJoinDatas()
	{
		if (TeamCompeteDataManager.RequestJoinDatas.Count > 0)
		{
			TeamCompeteDataManager.RequestJoinDatas.Clear();
		}
		ActivityTipManager.SetActivityTipItemActive(3034, TeamCompeteDataManager.RequestJoinDatas.Count > 0);
	}

	public static void JoinTeamCompete(bool isClick = false)
	{
		if (isClick && TeamCompeteDataManager.RequestJoinDatas.Count > 0)
		{
			TeamCompeteDataManager.RequestJoinDatas.RemoveAt(0);
		}
		ActivityTipManager.SetActivityTipItemActive(3034, TeamCompeteDataManager.RequestJoinDatas.Count > 0);
	}

	public static TianTi5v5ZhanDuiData MainZhanDuiData
	{
		get
		{
			return TeamCompeteDataManager._MainZhanDuiData;
		}
		set
		{
			TeamCompeteDataManager._MainZhanDuiData = value;
		}
	}

	public static TianTi5v5PiPeiState TianTi5v5PiPeiStateData
	{
		get
		{
			return TeamCompeteDataManager._TianTi5v5PiPeiState;
		}
		set
		{
			TeamCompeteDataManager._TianTi5v5PiPeiState = value;
		}
	}

	public static string GetDuanWeiNameByID(int duanWeiId)
	{
		DuanWeiVO duanWeiXmlDataById = IConfigbase<ConfigTeamCompete>.Instance.GetDuanWeiXmlDataById(duanWeiId);
		if (duanWeiXmlDataById == null)
		{
			return Global.GetLang("暂无段位");
		}
		return Global.GetString(new object[]
		{
			duanWeiXmlDataById.TypeName,
			duanWeiXmlDataById.Level,
			Global.GetLang("段")
		});
	}

	public static int GetDuanWeiNeedJiFenByID(int duanWeiId)
	{
		int num = duanWeiId + 1;
		DuanWeiVO duanWeiXmlDataById = IConfigbase<ConfigTeamCompete>.Instance.GetDuanWeiXmlDataById(num);
		if (duanWeiXmlDataById == null)
		{
			duanWeiXmlDataById = IConfigbase<ConfigTeamCompete>.Instance.GetDuanWeiXmlDataById(num - 1);
			if (duanWeiXmlDataById == null)
			{
				return 0;
			}
		}
		return duanWeiXmlDataById.NeedDuanWeiJiFen;
	}

	public static int GetDuanWeiRongYaoNumByID(int duanWeiId)
	{
		DuanWeiVO duanWeiXmlDataById = IConfigbase<ConfigTeamCompete>.Instance.GetDuanWeiXmlDataById(duanWeiId);
		if (duanWeiXmlDataById == null)
		{
			return 0;
		}
		return duanWeiXmlDataById.RongYaoNum;
	}

	public static RoleData4Selector GetRoleData4Selector(byte[] bytes)
	{
		return DataHelper.BytesToObject<RoleData4Selector>(bytes, 0, bytes.Length);
	}

	public static void PopupConfirmBattleWindow()
	{
		if (Global.Data != null && Global.Data.roleData.ZhanDuiZhiWu == 1)
		{
			return;
		}
		if (TeamCompeteDataManager.IsPopupConfirmBattleWindow)
		{
			return;
		}
		TeamCompeteDataManager.IsPopupConfirmBattleWindow = true;
		TeamCompeteDataManager.CloseAllTipWindow();
		if (TeamCompeteDataManager.SecondConfirmWindowCallBack != null)
		{
			TeamCompeteDataManager.SecondConfirmWindowCallBack.Invoke();
		}
		string message = string.Format(Global.GetLang("战队队长发起组队竞技邀请，是否加入？"), new object[0]);
		TeamCompeteDataManager.popupConfirmBattleWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.popupConfirmBattleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.popupConfirmBattleWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupConfirmBattleWindow);
			if (messageBoxReturn == 0)
			{
				if (TeamCompeteDataManager.IsSomeoneRefuse)
				{
					TeamCompeteDataManager.IsSomeoneRefuse = false;
					Super.HintMainText(Global.GetLang("当前战队邀请已失效"), 10, 3);
				}
				else
				{
					GameInstance.Game.SendIsAcceptBattleMsg(1);
				}
				TeamCompeteDataManager.IsPopupConfirmBattleWindow = false;
			}
			else
			{
				GameInstance.Game.SendIsAcceptBattleMsg(2);
				TeamCompeteDataManager.IsPopupConfirmBattleWindow = false;
			}
			return true;
		};
	}

	public static void Respond_CMD_SPR_KF5V5_AGREE_ZHANDUI_REQUEST(MUSocketConnectEventArgs e)
	{
		if (Global.Data.roleData == null || Global.Data.roleData.ZhanDuiZhiWu != 1 || Global.Data.roleData.ZhanDuiID <= 0)
		{
			return;
		}
		int num = e.fields[0].SafeToInt32(0);
		if (num < 0)
		{
			TeamCompeteDataManager.ErrorTips(num);
			return;
		}
		if (Global.Data.roleData != null && Global.Data.roleData.ZhanDuiZhiWu != 1)
		{
			Super.HintMainText(Global.GetLang("队长同意加入战队"), 10, 3);
		}
	}

	public static void Respond_CMD_SPR_KF5V5_NOTIFY_ZHANDUI_REQUEST(MUSocketConnectEventArgs e)
	{
		if (Global.Data.roleData == null || Global.Data.roleData.ZhanDuiZhiWu != 1 || Global.Data.roleData.ZhanDuiID <= 0)
		{
			return;
		}
		int roleId = e.fields[0].SafeToInt32(0);
		string roleName = e.fields[1];
		int roleOccu = e.fields[2].SafeToInt32(0);
		if (TeamCompeteDataManager.RequestJoinDatas.Count > 0 && TeamCompeteDataManager.RequestJoinDatas.Exists((TeamCompeteRequestJoinTeamData result) => result.RoleID == roleId))
		{
			return;
		}
		TeamCompeteRequestJoinTeamData teamCompeteRequestJoinTeamData = new TeamCompeteRequestJoinTeamData();
		teamCompeteRequestJoinTeamData.RoleID = roleId;
		teamCompeteRequestJoinTeamData.RoleName = roleName;
		teamCompeteRequestJoinTeamData.RoleOccu = roleOccu;
		TeamCompeteDataManager.RequestJoinDatas.Add(teamCompeteRequestJoinTeamData);
	}

	public static void PopupRequestJoinTeamWindow()
	{
		string text = string.Empty;
		int roleId = 0;
		if (TeamCompeteDataManager.RequestJoinDatas.Count > 0)
		{
			text = TeamCompeteDataManager.RequestJoinDatas[0].RoleName;
			roleId = TeamCompeteDataManager.RequestJoinDatas[0].RoleID;
			TeamCompeteDataManager.CloseAllTipWindow();
			string message = string.Format(Global.GetLang("玩家【{0}】请求加入战队，是否同意？"), text);
			TeamCompeteDataManager.popupRequestJoinWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
			TeamCompeteDataManager.popupRequestJoinWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = TeamCompeteDataManager.popupRequestJoinWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupRequestJoinWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendAcceptRequestJoinTeamMsg(roleId);
				}
				else
				{
					TeamCompeteDataManager.JoinTeamCompete(true);
				}
				return true;
			};
		}
	}

	public static void Respond_CMD_SPR_KF5V5_NOTIFY_ZHANDUI_INVITE(MUSocketConnectEventArgs e)
	{
		int teamID = e.fields[0].SafeToInt32(0);
		string playerName = e.fields[1];
		string teamName = e.fields[2];
		if (TeamCompeteDataManager.InviteDatas.Count > 0 && TeamCompeteDataManager.InviteDatas.Exists((TeamCompeteInviteData result) => result.Name == playerName && result.TeamName == teamName))
		{
			return;
		}
		TeamCompeteInviteData teamCompeteInviteData = new TeamCompeteInviteData();
		teamCompeteInviteData.TeamID = teamID;
		teamCompeteInviteData.Name = playerName;
		teamCompeteInviteData.TeamName = teamName;
		TeamCompeteDataManager.InviteDatas.Add(teamCompeteInviteData);
	}

	public static void PopupEnterDaTaoShaSceneWindow()
	{
		if (TeamCompeteDataManager.EnterDaTaoShaSceneWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterDaTaoShaSceneWindow);
			TeamCompeteDataManager.EnterDaTaoShaSceneWindow = null;
		}
		string lang = Global.GetLang("队长邀请你参加魔界大逃杀，是否参加？");
		TeamCompeteDataManager.EnterDaTaoShaSceneWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), lang, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.EnterDaTaoShaSceneWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.EnterDaTaoShaSceneWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterDaTaoShaSceneWindow);
			if (messageBoxReturn == 0)
			{
				if (PlayZone.GlobalPlayZone.gamePayerRolePart != null)
				{
					PlayZone.GlobalPlayZone.CloseGamePayerRoleWindow();
				}
				GameInstance.Game.RequestEnterDaTaoShaScene();
			}
			return true;
		};
	}

	public static void PopupEnterTeamZhengBaSceneWindow()
	{
		if (TeamCompeteDataManager.EnterTeamZhengBaSceneWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterTeamZhengBaSceneWindow);
			TeamCompeteDataManager.EnterTeamZhengBaSceneWindow = null;
		}
		string lang = Global.GetLang("队长邀请你参加战队争霸赛，是否参加？");
		TeamCompeteDataManager.EnterTeamZhengBaSceneWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), lang, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.EnterTeamZhengBaSceneWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.EnterTeamZhengBaSceneWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterTeamZhengBaSceneWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SendTeamZhengBaEnter();
			}
			return true;
		};
	}

	public static void PopupEnterMoYuDuoBaoWindow()
	{
		int mapCode = Global.Data.roleData.MapCode;
		if (!MoYuDuoBaoData.beCanShowDuoBaoEnter(mapCode))
		{
			return;
		}
		TeamCompeteDataManager.CloseAllTipWindow();
		string lang = Global.GetLang("魔域夺宝活动开启，您的队伍已参赛，是否进入战场");
		TeamCompeteDataManager.EnterMoYuDuoBaoWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), lang, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.EnterMoYuDuoBaoWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.EnterMoYuDuoBaoWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterMoYuDuoBaoWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SendDuoBaoEnter();
			}
			return true;
		};
	}

	public static void PopupConfirmInviteJoinTeamWindow()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int teamId = 0;
		if (TeamCompeteDataManager.InviteDatas.Count > 0)
		{
			text = TeamCompeteDataManager.InviteDatas[0].Name;
			text2 = TeamCompeteDataManager.InviteDatas[0].TeamName;
			teamId = TeamCompeteDataManager.InviteDatas[0].TeamID;
			TeamCompeteDataManager.CloseAllTipWindow();
			string message = string.Format(Global.GetLang("玩家【{0}】邀请你加入【{1}】战队，是否同意？"), text, text2);
			TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
			TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendAcceptInviteInfoMsg(teamId);
				}
				else
				{
					TeamCompeteDataManager.Invite(true);
				}
				return true;
			};
		}
	}

	public static void PopupLeaveTeamWindow()
	{
		TeamCompeteDataManager.CloseAllTipWindow();
		string message = string.Format(Global.GetLang("是否确定要退出战队？"), new object[0]);
		TeamCompeteDataManager.popupLeaveTeamWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.popupLeaveTeamWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.popupLeaveTeamWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupLeaveTeamWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.SendLeaveTeamMsg();
			}
			return true;
		};
	}

	public static void PopupTimeOutWindow()
	{
		TeamCompeteDataManager.CloseAllTipWindow();
		string message = string.Format(Global.GetLang("很抱歉，没有匹配到对手"), new object[0]);
		TeamCompeteDataManager.popupTimeOutWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 0, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(5f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		TeamCompeteDataManager.popupTimeOutWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = TeamCompeteDataManager.popupTimeOutWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupTimeOutWindow);
			if (messageBoxReturn == 0)
			{
			}
			return true;
		};
	}

	public static void CloseAllTipWindow()
	{
		if (TeamCompeteDataManager.popupConfirmBattleWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupConfirmBattleWindow);
			TeamCompeteDataManager.popupConfirmBattleWindow = null;
		}
		if (TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow);
			TeamCompeteDataManager.popupConfirmInviteJoinTeamWindow = null;
		}
		if (TeamCompeteDataManager.popupLeaveTeamWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupLeaveTeamWindow);
			TeamCompeteDataManager.popupLeaveTeamWindow = null;
		}
		if (TeamCompeteDataManager.popupTimeOutWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupTimeOutWindow);
			TeamCompeteDataManager.popupTimeOutWindow = null;
		}
		if (TeamCompeteDataManager.popupRequestJoinWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.popupRequestJoinWindow);
			TeamCompeteDataManager.popupRequestJoinWindow = null;
		}
		if (TeamCompeteDataManager.EnterTeamZhengBaSceneWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterTeamZhengBaSceneWindow);
			TeamCompeteDataManager.EnterTeamZhengBaSceneWindow = null;
		}
		if (TeamCompeteDataManager.EnterMoYuDuoBaoWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterMoYuDuoBaoWindow);
			TeamCompeteDataManager.EnterMoYuDuoBaoWindow = null;
		}
		if (TeamCompeteDataManager.EnterDaTaoShaSceneWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, TeamCompeteDataManager.EnterDaTaoShaSceneWindow);
			TeamCompeteDataManager.EnterDaTaoShaSceneWindow = null;
		}
	}

	public static void ErrorTips(int ret)
	{
		Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
	}

	public static bool IsLevelEnough
	{
		get
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TeamLevelLimit", ',');
			return Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 1000 >= systemParamIntArrayByName[0] * 1000 + systemParamIntArrayByName[1];
		}
	}

	public static bool IsDiamondEnough
	{
		get
		{
			return (long)Global.Data.roleData.UserMoney >= ConfigSystemParam.GetSystemParamIntByName("TeamNeedZuan");
		}
	}

	public static bool IsEnoughLenth(string name)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TeamBattleNameRange", ',');
		if (0 < name.Length && name.Length < systemParamIntArrayByName[0])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称不能少于{0}个字符，请重新输入！"), systemParamIntArrayByName[0]), 10, 3);
			return false;
		}
		if (name.Length > systemParamIntArrayByName[1])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称已超过{0}个字符，请重新输入！"), systemParamIntArrayByName[1]), 10, 3);
			return false;
		}
		return true;
	}

	public static string GetTouXiangPathByOccu(int occu)
	{
		if (TeamCompeteDataManager.DictTouXiangPath.Count <= 0)
		{
			TeamCompeteDataManager.DictTouXiangPath.Add(0, "NetImages/Face/00_0.png.qj");
			TeamCompeteDataManager.DictTouXiangPath.Add(1, "NetImages/Face/10_0.png.qj");
			TeamCompeteDataManager.DictTouXiangPath.Add(2, "NetImages/Face/20_0.png.qj");
			TeamCompeteDataManager.DictTouXiangPath.Add(3, "NetImages/Face/30_0.png.qj");
			TeamCompeteDataManager.DictTouXiangPath.Add(5, "NetImages/Face/50_0.png.qj");
		}
		string empty = string.Empty;
		if (TeamCompeteDataManager.DictTouXiangPath.TryGetValue(occu, ref empty))
		{
			return empty;
		}
		MUDebug.LogError<string>(new string[]
		{
			"头像不存在！"
		});
		return empty;
	}

	public static string GetCircleTouXiangPathByOccu(int occu)
	{
		if (TeamCompeteDataManager.DictCircleTouXiangPath.Count <= 0)
		{
			TeamCompeteDataManager.DictCircleTouXiangPath.Add(0, "NetImages/Face/00_1.png.qj");
			TeamCompeteDataManager.DictCircleTouXiangPath.Add(1, "NetImages/Face/10_1.png.qj");
			TeamCompeteDataManager.DictCircleTouXiangPath.Add(2, "NetImages/Face/20_1.png.qj");
			TeamCompeteDataManager.DictCircleTouXiangPath.Add(3, "NetImages/Face/30_1.png.qj");
			TeamCompeteDataManager.DictCircleTouXiangPath.Add(5, "NetImages/Face/50_1.png.qj");
		}
		string empty = string.Empty;
		if (TeamCompeteDataManager.DictCircleTouXiangPath.TryGetValue(occu, ref empty))
		{
			return empty;
		}
		MUDebug.LogError<string>(new string[]
		{
			"头像不存在！"
		});
		return empty;
	}

	public static bool IsInTeamZhengBaActivityOpen()
	{
		bool flag = TeamCompeteDataManager.IsInTeamZhengBaActivity();
		if (flag)
		{
			Super.HintMainText(Global.GetLang("战队争霸期间不允许进行该操作"), 10, 3);
		}
		return flag;
	}

	public static bool IsInTeamZhengBaActivity()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		Dictionary<int, TeamMatchVo>.Enumerator enumerator = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchXMLDict().GetEnumerator();
		List<string[]> list = new List<string[]>();
		List<int> list2 = new List<int>();
		while (enumerator.MoveNext())
		{
			List<string[]> list3 = list;
			KeyValuePair<int, TeamMatchVo> keyValuePair = enumerator.Current;
			list3.Add(keyValuePair.Value.TimePoints.Split(new char[]
			{
				','
			}));
			List<int> list4 = list2;
			KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
			list4.Add(keyValuePair2.Key);
		}
		TeamCompeteDataManager.DayOfOpenActivity = list[0][0].SafeToInt32(0);
		string[] array = list[0][1].Split(new char[]
		{
			'-'
		});
		if (correctDateTime.Day != TeamCompeteDataManager.DayOfOpenActivity)
		{
			return correctDateTime.Day < TeamCompeteDataManager.DayOfOpenActivity && false;
		}
		DateTime begin = TeamCompeteDataManager.ParseStringToDateTime(list[0][1].Split(new char[]
		{
			'-'
		})[0]);
		DateTime end = TeamCompeteDataManager.ParseStringToDateTime(list[list.Count - 1][1].Split(new char[]
		{
			'-'
		})[1]);
		TeamCompeteDataManager.IsInCurrentDayActivityTime(begin, end);
		switch (TeamCompeteDataManager.mTimeStatus)
		{
		case TeamCompeteDataManager.TimeStatus.NotBegin:
			return false;
		case TeamCompeteDataManager.TimeStatus.Beginning:
			return true;
		case TeamCompeteDataManager.TimeStatus.End:
			return false;
		default:
			return false;
		}
	}

	private static DateTime ParseStringToDateTime(string timeOfDay)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			timeOfDay
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	private static void IsInCurrentDayActivityTime(DateTime begin, DateTime end)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime > begin)
		{
			TeamCompeteDataManager.mTimeStatus = TeamCompeteDataManager.TimeStatus.NotBegin;
		}
		else if (begin <= correctDateTime && correctDateTime <= end)
		{
			TeamCompeteDataManager.mTimeStatus = TeamCompeteDataManager.TimeStatus.Beginning;
		}
		else
		{
			TeamCompeteDataManager.mTimeStatus = TeamCompeteDataManager.TimeStatus.End;
		}
	}

	public static bool IsTeamLeader
	{
		get
		{
			return Global.Data.roleData != null && TeamCompeteDataManager.HaveTeam && Global.Data.roleData.ZhanDuiZhiWu == 1;
		}
	}

	public static int TeamID
	{
		get
		{
			if (Global.Data.roleData != null)
			{
				return Global.Data.roleData.ZhanDuiID;
			}
			return -1;
		}
	}

	public static bool HaveTeam
	{
		get
		{
			return Global.Data.roleData != null && Global.Data.roleData.ZhanDuiID > 0;
		}
	}

	public static bool CanAttack
	{
		get
		{
			return TeamCompeteDataManager.status == DaTaoShaStatus.Kill && TeamCompeteDataManager.status == DaTaoShaStatus.CrazyKill;
		}
	}

	public static bool CanReciveJoinTeamRequestInCurrentMap
	{
		get
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TeamApply", ',');
			int num = systemParamIntArrayByName.IndexOf(Global.Data.roleData.MapCode);
			return num >= 0;
		}
	}

	public static string ServerTeamName(int serverId, string name)
	{
		return Global.FormatRoleNameZoneid(serverId, name, 0, 1);
	}

	public static Action RefreshJingCaiDianCallBack = null;

	public static long ZhengBaRequestFlag = 0L;

	public static Action<bool, bool, bool> AllAcceptCallBack = null;

	public static int HaveMonthPaiHangAwards = 0;

	public static int TodayFightCount = 0;

	public static Action SecondConfirmWindowCallBack = null;

	public static Action<List<ZhanDuiZhengBaZhanDuiData>> ZhangBaGuessCallBack = null;

	private static List<TeamCompeteInviteData> InviteDatas = new List<TeamCompeteInviteData>();

	private static int inviteCount = 0;

	private static List<TeamCompeteRequestJoinTeamData> RequestJoinDatas = new List<TeamCompeteRequestJoinTeamData>();

	private static int requestJoinCount = 0;

	private static TianTi5v5ZhanDuiData _MainZhanDuiData = null;

	private static TianTi5v5PiPeiState _TianTi5v5PiPeiState = null;

	private static GChildWindow popupConfirmBattleWindow = null;

	private static bool IsPopupConfirmBattleWindow = false;

	private static GChildWindow popupRequestJoinWindow = null;

	private static GChildWindow EnterDaTaoShaSceneWindow = null;

	private static GChildWindow EnterTeamZhengBaSceneWindow = null;

	private static GChildWindow EnterMoYuDuoBaoWindow = null;

	private static GChildWindow popupConfirmInviteJoinTeamWindow = null;

	private static GChildWindow popupLeaveTeamWindow = null;

	private static GChildWindow popupTimeOutWindow = null;

	public static bool IsSomeoneRefuse = false;

	private static Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();

	private static Dictionary<int, string> DictCircleTouXiangPath = new Dictionary<int, string>();

	private static TeamCompeteDataManager.TimeStatus mTimeStatus = TeamCompeteDataManager.TimeStatus.NotBegin;

	private static int DayOfOpenActivity = 0;

	public static DaTaoShaStatus status = DaTaoShaStatus.None;

	private enum TimeStatus
	{
		NotBegin,
		Beginning,
		End
	}
}
