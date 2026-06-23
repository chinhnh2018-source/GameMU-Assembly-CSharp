using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class TeamCompeteNetManager
{
	public static void Respond_CMD_SPR_SPR_GET_KF5V5_INFO(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_SPR_GET_KF5V5_INFO", e);
	}

	public static void Respond_CMD_SPR_KF5V5_GETZHANDUI_LIST(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GETZHANDUI_LIST", e);
	}

	public static void Respond_CMD_SPR_KF5V5_CREATE_ZHANDUI(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CREATE_ZHANDUI", e);
	}

	public static void Respond_CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_MY_ZHANDUI_INFO", e);
	}

	public static void Respond_CMD_SPR_KF5V5_REQUEST_TO_ZHANDUI(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_REQUEST_TO_ZHANDUI", e);
	}

	public static void Respond_CMD_SPR_KF5V5_UPDATE_ZHANDUI_XUYAN(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_UPDATE_ZHANDUI_XUYAN", e);
	}

	public static void Respond_CMD_SPR_KF5V5_DELETE_MEMBER(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_DELETE_MEMBER", e);
	}

	public static void Respond_CMD_SPR_KF5V5_INVITE_TO_ZHANDUI(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			if (num == -19)
			{
				Super.HintMainText(Global.GetLang("进入战队所需等级不足，无法邀请加入战队"), 10, 3);
			}
			else
			{
				if (num == -12)
				{
					return;
				}
				TeamCompeteDataManager.ErrorTips(num);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("邀请成功"), 10, 3);
		}
	}

	public static void Respond_CMD_SPR_KF5V5_NOTIFY_ZHANDUI_INVITE(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_NOTIFY_ZHANDUI_INVITE", e);
	}

	public static void Respond_CMD_SPR_KF5V5_AGREE_ZHANDUI_INVITE(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_AGREE_ZHANDUI_INVITE", e);
	}

	public static void Respond_CMD_SPR_GETFRIENDS(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_GETFRIENDS", e);
	}

	public static void Respond_CMD_SPR_KF5V5_CHANGE_ZHANDUI_LEADER(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CHANGE_ZHANDUI_LEADER", e);
	}

	public static void Respond_CMD_SPR_KF5V5_QUIT_ZHANDUI(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUIT_ZHANDUI", e);
	}

	public static void Respond_CMD_SPR_KF5V5_JIESAN_ZHANDUI(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_JIESAN_ZHANDUI", e);
	}

	public static void Respond_CMD_SPR_KF5V5_ROLE_SIGN(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_SIGN", e);
	}

	public static void Respond_CMD_SPR_KF5V5_ROLE_CANCLE_SIGN(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ROLE_CANCLE_SIGN", e);
	}

	public static void Respond_CMD_SPR_KF5V5_NOTIFY_PIPEI_SUCCESS(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_NOTIFY_PIPEI_SUCCESS", e);
	}

	public static void Respond_CMD_SPR_KF5V5_ENTER_KF5V5_SCENE(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_ENTER_KF5V5_SCENE", e);
	}

	public static void Respond_CMD_SPR_KF5V5_GET_LOG_LIST(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GET_LOG_LIST", e);
	}

	public static void Respond_CMD_SPR_KF5V5_QUERY_DAY_PAIHANG(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_QUERY_DAY_PAIHANG", e);
	}

	public static void Respond_CMD_SPR_KF5V5_SIGNUP_STATE(MUSocketConnectEventArgs e)
	{
		TianTi5v5PiPeiState tianTi5v5PiPeiState = DataHelper.BytesToObject<TianTi5v5PiPeiState>(e.bytesData, 0, e.bytesData.Length);
		if (tianTi5v5PiPeiState == null)
		{
			return;
		}
		TeamCompeteDataManager.TianTi5v5PiPeiStateData = tianTi5v5PiPeiState;
		List<TianTi5v5PiPeiRoleState> roleList = TeamCompeteDataManager.TianTi5v5PiPeiStateData.RoleList;
		if (roleList != null && roleList.Count > 0)
		{
			for (int i = 0; i < roleList.Count; i++)
			{
				if (roleList[i].RoleID == Global.Data.roleData.RoleID && Global.Data.roleData.ZhanDuiZhiWu == 1)
				{
					if (PlayZone.GlobalPlayZone != null)
					{
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 1534
						});
					}
				}
				else if (roleList[i].RoleID == Global.Data.roleData.RoleID)
				{
					if (roleList[i].State == 1)
					{
						if (PlayZone.GlobalPlayZone != null)
						{
							PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
							{
								ID = 1534
							});
						}
					}
					else
					{
						TeamCompeteDataManager.PopupConfirmBattleWindow();
					}
				}
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int j = 0; j < roleList.Count; j++)
			{
				if (roleList[j].State == 2 || roleList[j].State == 3)
				{
					flag = true;
					break;
				}
			}
			for (int k = 0; k < roleList.Count; k++)
			{
				if (roleList[k].State == 0)
				{
					flag2 = true;
					break;
				}
			}
			for (int l = 0; l < roleList.Count; l++)
			{
				if (roleList[l].State == 4)
				{
					flag3 = true;
					break;
				}
			}
			TeamCompeteDataManager.IsSomeoneRefuse = (flag || flag3);
			if (TeamCompeteDataManager.AllAcceptCallBack != null)
			{
				TeamCompeteDataManager.AllAcceptCallBack.Invoke(flag, flag2, flag3);
			}
		}
	}

	public static void Respond_CMD_SPR_KF5V5_Notify_ZHANDUI_DATA_CHANGED(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_Notify_ZHANDUI_DATA_CHANGED", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_MAIN_INFO(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_MAIN_INFO", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_ZHANDUI_LIST(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_ZHANDUI_LIST", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_SUPPORT(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_SUPPORT", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_LOG(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_LOG", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_SCORE_INFO(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_SCORE_INFO", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_AWARD(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_AWARD", e);
	}

	public static void Respond_CMD_SPR_ZHANDUIZHENGBA_SUPPORT_LIST(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_SUPPORT_LIST", e);
	}

	public static void Respond_CMD_SPR_ESCAPE_STATE(MUSocketConnectEventArgs e)
	{
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_STATE", e);
	}

	public static void Respond_CMD_SPR_ESCAPE_INVITE_NOTIFY(MUSocketConnectEventArgs e)
	{
		List<EscapeBattleJoinRoleInfo> list = DataHelper.BytesToObject<List<EscapeBattleJoinRoleInfo>>(e.bytesData, 0, e.bytesData.Length);
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (Global.Data.roleData != null && list[i].RoleID == Global.Data.roleData.RoleID && Global.Data.roleData.ZhanDuiZhiWu != 1 && DaTaoShaDataManager.IsDisplayInviteWindowInCurrentMap() && PlayZone.GlobalPlayZone != null)
				{
					PlayZone.GlobalPlayZone.OpenDaTaoShaInvitePart(list);
				}
			}
		}
	}

	public static void Respond_CMD_SPR_ESCAPE_JOIN_INFO(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		List<EscapeBattleJoinRoleInfo> list = DataHelper.BytesToObject<List<EscapeBattleJoinRoleInfo>>(e.bytesData, 0, e.bytesData.Length);
		DaTaoShaDataManager.CacheEscapeBattleJoinRoleInfo = list;
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<List<EscapeBattleJoinRoleInfo>>("CMD_SPR_ESCAPE_JOIN_INFO", list);
	}

	public static void Respond_CMD_SPR_ESCAPE_SIDE_SCORE(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		EscapeBattleSideScore escapeBattleSideScore = DataHelper.BytesToObject<EscapeBattleSideScore>(e.bytesData, 0, e.bytesData.Length);
		if (escapeBattleSideScore == null)
		{
			return;
		}
		MUEventManager.SendEvent<EscapeBattleSideScore>("CMD_SPR_ESCAPE_SIDE_SCORE", escapeBattleSideScore);
	}

	public static void Respond_CMD_SPR_ESCAPE_RANK_INFO(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (DataHelper.BytesToObject<EscapeBattleRankInfo>(e.bytesData, 0, e.bytesData.Length) == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_RANK_INFO", e);
	}
}
