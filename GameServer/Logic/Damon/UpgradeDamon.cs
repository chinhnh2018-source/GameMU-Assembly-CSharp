using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic.Damon
{
	public class UpgradeDamon
	{
		public static void LoadUpgradeAttr()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("PetQiangHuaProps");
			string[] array = paramValueByName.Split(new char[]
			{
				'|'
			});
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2 != null || array2.Length != 2)
					{
						UpgradeDamon.UpgradeAttrDict[int.Parse(array2[0])] = double.Parse(array2[1]);
					}
				}
			}
		}

		public static double GetPetQiangPer(int nPropIndex)
		{
			double result = 0.0;
			UpgradeDamon.UpgradeAttrDict.TryGetValue(nPropIndex, out result);
			return result;
		}

		public static TCPProcessCmdResults UpgradeDamonProcess(TCPOutPacketPool pool, GameClient client, GoodsData goodsData, out TCPOutPacket tcpOutPacket, int nID, TCPClientPool tcpClientPool, TCPManager tcpMgr)
		{
			tcpOutPacket = null;
			SystemXmlItem systemXmlItem = null;
			TCPProcessCmdResults result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) || null == systemXmlItem)
			{
				string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-13,
					client.ClientData.RoleID,
					goodsData.Id,
					0,
					0
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
				result = TCPProcessCmdResults.RESULT_DATA;
			}
			else
			{
				int num = systemXmlItem.GetIntValue("SuitID", -1) * 10 + 9;
				if (goodsData.Forge_level >= num)
				{
					string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-4,
						client.ClientData.RoleID,
						goodsData.Id,
						goodsData.Forge_level,
						goodsData.Binding
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else if (goodsData.Site != 5000)
				{
					string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						-9,
						client.ClientData.RoleID,
						goodsData.Id,
						goodsData.Forge_level,
						goodsData.Binding
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
					result = TCPProcessCmdResults.RESULT_DATA;
				}
				else
				{
					SystemXmlItem systemXmlItem2 = null;
					GameManager.SystemDamonUpgrade.SystemXmlItemDict.TryGetValue(goodsData.Forge_level + 2, out systemXmlItem2);
					if (null == systemXmlItem2)
					{
						string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							-6,
							client.ClientData.RoleID,
							goodsData.Id,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
						result = TCPProcessCmdResults.RESULT_DATA;
					}
					else
					{
						int intValue = systemXmlItem2.GetIntValue("NeedEXP", -1);
						long num2 = (long)GameManager.ClientMgr.GetMUMoHeValue(client);
						if (num2 < (long)intValue)
						{
							string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								-11,
								client.ClientData.RoleID,
								goodsData.Id,
								goodsData.Forge_level,
								goodsData.Binding
							});
							tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
							result = TCPProcessCmdResults.RESULT_DATA;
						}
						else
						{
							GameManager.ClientMgr.ModifyMUMoHeValue(client, -intValue, "精灵升级", true, true, false);
							int num3 = 1;
							string[] array = null;
							string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								goodsData.Id,
								"*",
								goodsData.Forge_level + 1,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								num3,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array, client.ServerId);
							if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
							{
								string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									-10,
									client.ClientData.RoleID,
									goodsData.Id,
									0,
									0
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
							else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
							{
								string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									-10,
									client.ClientData.RoleID,
									goodsData.Id,
									0,
									0
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
							else
							{
								int @using = goodsData.Using;
								if (goodsData.Using > 0)
								{
									goodsData.Using = 0;
									Global.RefreshEquipProp(client, goodsData);
								}
								goodsData.Forge_level++;
								goodsData.Binding = num3;
								JingLingQiYuanManager.getInstance().RefreshProps(client, true);
								if (@using != goodsData.Using)
								{
									goodsData.Using = @using;
									if (Global.RefreshEquipProp(client, goodsData))
									{
										GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, client, true, false, 7);
									}
								}
								Global.ModRoleGoodsEvent(client, goodsData, 0, "强化", false);
								EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "强化");
								string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
								{
									1,
									client.ClientData.RoleID,
									goodsData.Id,
									goodsData.Forge_level,
									num3
								});
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
								result = TCPProcessCmdResults.RESULT_DATA;
							}
						}
					}
				}
			}
			return result;
		}

		private static Dictionary<int, double> UpgradeAttrDict = new Dictionary<int, double>();
	}
}
