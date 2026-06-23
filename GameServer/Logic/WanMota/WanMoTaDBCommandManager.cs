using System;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.WanMota
{
	public class WanMoTaDBCommandManager
	{
		public static int LayerChangeDBCommand(GameClient client, int nLayerCount)
		{
			long num = TimeUtil.NOW();
			string strParams = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				num,
				nLayerCount,
				"*",
				"*",
				"*"
			});
			ModifyWanMotaData cmdInfo = new ModifyWanMotaData
			{
				strParams = strParams,
				strSweepReward = "*"
			};
			string[] array = Global.SendToDB<ModifyWanMotaData>(10158, cmdInfo, client.ServerId);
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

		public static int SweepBeginDBCommand(GameClient client, int nLayerCount)
		{
			long num = TimeUtil.NOW();
			string strParams = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				"*",
				"*",
				"1",
				"",
				num
			});
			ModifyWanMotaData cmdInfo = new ModifyWanMotaData
			{
				strParams = strParams,
				strSweepReward = "*"
			};
			string[] array = Global.SendToDB<ModifyWanMotaData>(10158, cmdInfo, client.ServerId);
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

		public static int UpdateSweepAwardDBCommand(GameClient client, int nSweepLayerCount)
		{
			int result;
			if (null == client.ClientData.LayerRewardData)
			{
				result = -1;
			}
			else
			{
				string strSweepReward = "";
				lock (client.ClientData.LayerRewardData)
				{
					if (-1 != nSweepLayerCount)
					{
						byte[] inArray = DataHelper.ObjectToBytes<LayerRewardData>(client.ClientData.LayerRewardData);
						strSweepReward = Convert.ToBase64String(inArray);
					}
				}
				string strParams = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					nSweepLayerCount,
					"*",
					"*"
				});
				ModifyWanMotaData cmdInfo = new ModifyWanMotaData
				{
					strParams = strParams,
					strSweepReward = strSweepReward
				};
				string[] array = Global.SendToDB<ModifyWanMotaData>(10158, cmdInfo, client.ServerId);
				if (array == null || array.Length != 2)
				{
					result = -1;
				}
				else
				{
					client.ClientData.WanMoTaProp.nSweepLayer = nSweepLayerCount;
					client.ClientData.WanMoTaProp.strSweepReward = strSweepReward;
					result = Convert.ToInt32(array[1]);
				}
			}
			return result;
		}
	}
}
