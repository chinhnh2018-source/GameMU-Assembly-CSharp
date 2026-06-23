using System;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic.MUWings
{
	public static class MUWingsManager
	{
		public static int MaxWingID
		{
			get
			{
				return GameManager.SystemWingsUp.MaxKey;
			}
		}

		public static void InitFirstWing(GameClient client)
		{
			if (null == client.ClientData.MyWingData)
			{
				WingData myWingData = MUWingsManager.AddWingDBCommand(TCPOutPacketPool.getInstance(), client.ClientData.RoleID, 1, client.ServerId);
				client.ClientData.MyWingData = myWingData;
			}
		}

		public static WingData AddWingDBCommand(TCPOutPacketPool pool, int roleID, int WingID, int serverId)
		{
			TCPOutPacket tcpoutPacket = null;
			string strcmd = string.Format("{0}:{1}", roleID, WingID);
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, pool, 10153, strcmd, out tcpoutPacket, serverId);
			WingData result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = null;
			}
			else if (null == tcpoutPacket)
			{
				result = null;
			}
			else
			{
				WingData wingData = DataHelper.BytesToObject<WingData>(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
				Global.PushBackTcpOutPacket(tcpoutPacket);
				result = wingData;
			}
			return result;
		}

		public static int WingOnOffDBCommand(GameClient client, int dbID, int isUsing)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				dbID,
				isUsing,
				"*",
				"*",
				"*",
				"*",
				"*",
				"*"
			});
			string[] array = Global.ExecuteDBCmd(10154, strcmd, client.ServerId);
			int result;
			if (array == null || array.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(array[1]);
			}
			return result;
		}

		public static int WingUpStarDBCommand(GameClient client, int dbID, int nStarLevel, int nStarExp)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				dbID,
				"*",
				"*",
				nStarLevel,
				"*",
				nStarExp,
				"*",
				"*"
			});
			string[] array = Global.ExecuteDBCmd(10154, strcmd, client.ServerId);
			int result;
			if (array == null || array.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(array[1]);
			}
			return result;
		}

		public static int WingUpDBCommand(GameClient client, int dbID, int nWingLevel, int nFailNum, int nStarLevel, int nStarExp, int nZhuLingNum, int nZhuHunNum)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				dbID,
				"*",
				nWingLevel,
				nStarLevel,
				nFailNum,
				nStarExp,
				nZhuLingNum,
				nZhuHunNum
			});
			string[] array = Global.ExecuteDBCmd(10154, strcmd, client.ServerId);
			int result;
			if (array == null || array.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(array[1]);
			}
			return result;
		}

		public static bool UpdateWingDataProps(GameClient client, bool toAdd = true)
		{
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				result = false;
			}
			else if (client.ClientData.MyWingData.WingID <= 0)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = WingPropsCacheManager.GetWingPropsCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID);
				if (null == systemXmlItem)
				{
					result = false;
				}
				else
				{
					MUWingsManager.ChangeWingDataProps(client, systemXmlItem, toAdd);
					systemXmlItem = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel);
					if (null == systemXmlItem)
					{
						result = false;
					}
					else
					{
						MUWingsManager.ChangeWingDataProps(client, systemXmlItem, toAdd);
						result = true;
					}
				}
			}
			return result;
		}

		public static bool ChangeWingDataProps(GameClient client, SystemXmlItem baseXmlNode, bool toAdd = true)
		{
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				result = false;
			}
			else if (client.ClientData.MyWingData.WingID <= 0)
			{
				result = false;
			}
			else
			{
				double num = baseXmlNode.GetDoubleValue("MinAttackV");
				if (!toAdd)
				{
					num = 0.0 - num;
				}
				double num2 = baseXmlNode.GetDoubleValue("MaxAttackV");
				if (!toAdd)
				{
					num2 = 0.0 - num2;
				}
				client.ClientData.EquipProp.ExtProps[7] += num;
				client.ClientData.EquipProp.ExtProps[8] += num2;
				double num3 = baseXmlNode.GetDoubleValue("MinMAttackV");
				if (!toAdd)
				{
					num3 = 0.0 - num3;
				}
				double num4 = baseXmlNode.GetDoubleValue("MaxMAttackV");
				if (!toAdd)
				{
					num4 = 0.0 - num4;
				}
				client.ClientData.EquipProp.ExtProps[9] += num3;
				client.ClientData.EquipProp.ExtProps[10] += num4;
				double num5 = baseXmlNode.GetDoubleValue("MinDefenseV");
				if (!toAdd)
				{
					num5 = 0.0 - num5;
				}
				double num6 = baseXmlNode.GetDoubleValue("MaxDefenseV");
				if (!toAdd)
				{
					num6 = 0.0 - num6;
				}
				client.ClientData.EquipProp.ExtProps[3] += num5;
				client.ClientData.EquipProp.ExtProps[4] += num6;
				double num7 = baseXmlNode.GetDoubleValue("MinMDefenseV");
				if (!toAdd)
				{
					num7 = 0.0 - num7;
				}
				double num8 = baseXmlNode.GetDoubleValue("MaxMDefenseV");
				if (!toAdd)
				{
					num8 = 0.0 - num8;
				}
				client.ClientData.EquipProp.ExtProps[5] += num7;
				client.ClientData.EquipProp.ExtProps[6] += num8;
				double num9 = baseXmlNode.GetDoubleValue("MaxLifeV");
				if (!toAdd)
				{
					num9 = 0.0 - num9;
				}
				client.ClientData.EquipProp.ExtProps[13] += num9;
				double num10 = baseXmlNode.GetDoubleValue("SubAttackInjurePercent");
				if (!toAdd)
				{
					num10 = 0.0 - num10;
				}
				client.ClientData.EquipProp.ExtProps[24] += num10;
				double num11 = baseXmlNode.GetDoubleValue("AddAttackInjurePercent");
				if (!toAdd)
				{
					num11 = 0.0 - num11;
				}
				client.ClientData.EquipProp.ExtProps[26] += num11;
				double num12 = baseXmlNode.GetDoubleValue("Dodge");
				if (!toAdd)
				{
					num12 = 0.0 - num12;
				}
				client.ClientData.EquipProp.ExtProps[19] += num12;
				double num13 = baseXmlNode.GetDoubleValue("HitV");
				if (!toAdd)
				{
					num13 = 0.0 - num13;
				}
				client.ClientData.EquipProp.ExtProps[18] += num13;
				result = true;
			}
			return result;
		}

		public static SystemXmlItem GetWingUPCacheItem(int nLevel)
		{
			SystemXmlItem systemXmlItem = null;
			SystemXmlItem result;
			if (!GameManager.SystemWingsUp.SystemXmlItemDict.TryGetValue(nLevel, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				result = systemXmlItem;
			}
			return result;
		}

		public static bool IfWingPerfect(GameClient client)
		{
			return null != client.ClientData.MyWingData && client.ClientData.MyWingData.WingID >= MUWingsManager.MaxWingID && client.ClientData.MyWingData.ForgeLevel >= MUWingsManager.MaxWingEnchanceLevel;
		}

		public static int MaxWingEnchanceLevel = 10;
	}
}
