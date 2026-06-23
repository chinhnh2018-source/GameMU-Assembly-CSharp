using System;
using System.Collections.Generic;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.UserActivate
{
	public class UserActivateManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		public static UserActivateManager getInstance()
		{
			return UserActivateManager.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1040, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1041, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1040:
				result = this.ProcessCmdActivateInfo(client, nID, bytes, cmdParams);
				break;
			case 1041:
				result = this.ProcessCmdActivateAward(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool ProcessCmdActivateInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(8))
				{
					client.sendCmd<int>(1040, -8, false);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 5))
				{
					return false;
				}
				int num = int.Parse(cmdParams[0]);
				string userID = cmdParams[1];
				int num2 = Convert.ToInt32(cmdParams[2]);
				string b = cmdParams[3].ToLower();
				string error = cmdParams[4];
				string checkInfo = this.GetCheckInfo(userID, error, num2);
				if (checkInfo != b)
				{
					client.sendCmd<int>(1040, -1, false);
					return true;
				}
				PlatformTypes platformType = GameCoreInterface.getinstance().GetPlatformType();
				if (platformType != 1)
				{
					client.sendCmd<int>(1040, -2, false);
					return true;
				}
				if (num2 != 0)
				{
					client.sendCmd<int>(1040, -7, false);
					return true;
				}
				int cmdData = this.DBActivateStateGet(client);
				client.sendCmd<int>(1040, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdActivateAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 5))
				{
					return false;
				}
				int roleId = int.Parse(cmdParams[0]);
				string userID = cmdParams[1];
				int activateType = Convert.ToInt32(cmdParams[2]);
				string activateInfo = cmdParams[3].ToLower();
				string error = cmdParams[4];
				UserActivateManager.EUserActivateState cmdData = this.ActivateAward(client, roleId, userID, activateType, activateInfo, error);
				client.sendCmd<int>(1041, (int)cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private UserActivateManager.EUserActivateState ActivateAward(GameClient client, int roleId, string userID, int activateType, string activateInfo, string error)
		{
			string checkInfo = this.GetCheckInfo(userID, error, activateType);
			UserActivateManager.EUserActivateState result;
			if (checkInfo != activateInfo)
			{
				result = UserActivateManager.EUserActivateState.ECheck;
			}
			else if (activateType != 0)
			{
				result = UserActivateManager.EUserActivateState.EnoBind;
			}
			else
			{
				PlatformTypes platformType = GameCoreInterface.getinstance().GetPlatformType();
				if (platformType != 1)
				{
					result = UserActivateManager.EUserActivateState.EPlatform;
				}
				else
				{
					int num = this.DBActivateStateGet(client);
					if (num == 1)
					{
						result = UserActivateManager.EUserActivateState.EIsAward;
					}
					else
					{
						List<GoodsData> awardList = this.GetAwardList();
						if (awardList == null || awardList.Count <= 0)
						{
							result = UserActivateManager.EUserActivateState.ENoAward;
						}
						else if (!Global.CanAddGoodsDataList(client, awardList))
						{
							result = UserActivateManager.EUserActivateState.EBag;
						}
						else if (!this.DBActivateStateSet(client))
						{
							result = UserActivateManager.EUserActivateState.EFail;
						}
						else
						{
							for (int i = 0; i < awardList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "账号绑定奖励", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
							}
							result = UserActivateManager.EUserActivateState.Success;
						}
					}
				}
			}
			return result;
		}

		private string GetCheckInfo(string userID, string error, int activateType)
		{
			userID = userID.ToLower().Replace("apps", "");
			string text = "WwSiia943ui3Wej5NrqUI3rfqrf83quj";
			string text2 = string.Format("{0}error={1}&ret={2}&uid={3}", new object[]
			{
				text,
				error,
				activateType,
				userID
			});
			text2 = MD5Helper.get_md5_string(text2);
			return text2.ToLower();
		}

		private List<GoodsData> GetAwardList()
		{
			List<GoodsData> list = new List<GoodsData>();
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("App_BindPhoneAward");
			List<GoodsData> result;
			if (string.IsNullOrEmpty(paramValueByName))
			{
				result = null;
			}
			else
			{
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					list = GoodsHelper.ParseGoodsDataList(array, "SystemParams.xml");
				}
				result = list;
			}
			return result;
		}

		private int DBActivateStateGet(GameClient client)
		{
			int result = 0;
			string strcmd = string.Format("{0}", client.strUserID);
			string[] array = Global.ExecuteDBCmd(13120, strcmd, client.ServerId);
			if (array != null && array.Length == 1)
			{
				result = int.Parse(array[0]);
			}
			return result;
		}

		private bool DBActivateStateSet(GameClient client)
		{
			bool result = false;
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleID);
			string[] array = Global.ExecuteDBCmd(13121, strcmd, client.ServerId);
			if (array != null && array.Length == 1)
			{
				result = (array[0] == "1");
			}
			return result;
		}

		private static UserActivateManager instance = new UserActivateManager();

		private enum EUserActivateState
		{
			NotOpen = -8,
			EnoBind,
			EBag,
			ENoAward,
			EFail,
			EIsAward,
			EPlatform,
			ECheck,
			Default,
			Success
		}
	}
}
