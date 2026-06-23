using System;
using GameServer.Logic.BocaiSys;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class HuanLeDaiBiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		private HuanLeDaiBiManager()
		{
		}

		public static HuanLeDaiBiManager GetInstance()
		{
			return HuanLeDaiBiManager.instance;
		}

		public bool initialize()
		{
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
			return false;
		}

		public bool startup()
		{
			this.GetHuanLeDaiBi(0);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2087, 3, 3, HuanLeDaiBiManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (nID == 2087)
				{
					int num = 0;
					int num2 = Convert.ToInt32(cmdParams[1]);
					int num3 = Convert.ToInt32(cmdParams[2]);
					if (num2 < 1)
					{
						num = 11;
					}
					else if (num3 == 1)
					{
						this.ExchangeHuanlebi(client, num2, ref num);
					}
					else if (num3 == 2)
					{
						this.ExchangeLuckStar(client, num2, ref num);
					}
					else
					{
						num = 19;
					}
					client.sendCmd(nID, num.ToString(), false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		private void ExchangeLuckStar(GameClient client, int ExchangeNum, ref int mgsData)
		{
			try
			{
				if (!GameManager.ClientMgr.SubUserMoney(client, ExchangeNum, "兑换幸运之星", true, true, true, true, DaiBiSySType.None))
				{
					mgsData = 12;
				}
				else if (!GameManager.ClientMgr.ModifyLuckStarValue(client, ExchangeNum, "兑换幸运之星", false, DaiBiSySType.None))
				{
					mgsData = 20;
				}
				else if (!GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, ExchangeNum, "兑换幸运之星", true, true, false))
				{
					LogManager.WriteLog(2, string.Format("[ljl_幸运之星] 兑换幸运之星 添加魔晶失败id={0},name={1}，num={2}", client.ClientData.RoleID, client.ClientData.RoleName, ExchangeNum), null, true);
				}
			}
			catch (Exception ex)
			{
				mgsData = 100;
				LogManager.WriteLog(9, string.Format("[ljl_幸运之星]{0}", ex.ToString()), null, true);
			}
		}

		private void ExchangeHuanlebi(GameClient client, int ExchangeNum, ref int mgsData)
		{
			try
			{
				GoodsData huanLeDaiBi = this.GetHuanLeDaiBi(ExchangeNum);
				mgsData = 0;
				if (ExchangeNum < 1)
				{
					mgsData = 11;
				}
				else if (client.ClientData.UserMoney < ExchangeNum)
				{
					mgsData = 12;
				}
				else if (!Global.CanAddGoods3(client, huanLeDaiBi.GoodsID, ExchangeNum, huanLeDaiBi.Binding, "1900-01-01 12:00:00", true))
				{
					mgsData = 13;
				}
				if (mgsData == 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(client, ExchangeNum, "兑换欢乐币", true, true, true, true, DaiBiSySType.None))
					{
						mgsData = 12;
					}
					else
					{
						int num = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, huanLeDaiBi.GoodsID, huanLeDaiBi.GCount, huanLeDaiBi.Quality, huanLeDaiBi.Props, huanLeDaiBi.Forge_level, huanLeDaiBi.Binding, huanLeDaiBi.Site, huanLeDaiBi.Jewellist, true, 1, "兑换欢乐币", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						LogManager.WriteLog(0, string.Format("[ljl_欢乐代币]兑换欢乐币ret={1}，num={0},,id={2},name={3}", new object[]
						{
							ExchangeNum,
							num,
							client.ClientData.RoleID,
							client.ClientData.RoleName
						}), null, true);
					}
				}
			}
			catch (Exception ex)
			{
				mgsData = 100;
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
		}

		public GoodsData GetHuanLeDaiBi(int GCount = 0)
		{
			try
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("HuanLeDaiBi");
				GoodsData goodsData = GlobalNew.ParseGoodsData(paramValueByName);
				goodsData.GCount = GCount;
				return goodsData;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "欢乐代币格式不对", null, true);
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return null;
		}

		public bool UseHuanledaibi(GameClient client, int UseNum)
		{
			try
			{
				int goodsID = this.GetHuanLeDaiBi(0).GoodsID;
				if (this.HuanledaibiEnough(client, UseNum))
				{
					bool flag;
					return GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, UseNum, false, out flag, out flag, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool HuanledaibiEnough(GameClient client, int UseNum)
		{
			try
			{
				int goodsID = this.GetHuanLeDaiBi(0).GoodsID;
				return Global.GetTotalGoodsCountByID(client, goodsID) >= UseNum;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool HuanledaibiReplaceEnough(GameClient client, int UseNum, DaiBiSySType type)
		{
			try
			{
				if (!BoCaiConfigMgr.CanReplaceMoney(type))
				{
					return false;
				}
				return this.HuanledaibiEnough(client, UseNum);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		public bool UseReplaceMoney(GameClient client, int UseNum, DaiBiSySType type, string info, bool isLuckStar = false)
		{
			try
			{
				if (UseNum == 0)
				{
					return true;
				}
				if (!BoCaiConfigMgr.CanReplaceMoney(type))
				{
					return false;
				}
				if (this.UseHuanledaibi(client, UseNum))
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "欢乐代币代替钻石", info, client.ClientData.RoleName, "系统", "减少", UseNum, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
					LogManager.WriteLog(0, string.Format("[ljl_欢乐代币]{0}", string.Format("msg={0},subNum={1},type={2},isLuckStar={3},id={4},name={5}", new object[]
					{
						info,
						UseNum,
						type,
						isLuckStar,
						client.ClientData.RoleID,
						client.ClientData.RoleName
					})), null, true);
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		private static HuanLeDaiBiManager instance = new HuanLeDaiBiManager();
	}
}
