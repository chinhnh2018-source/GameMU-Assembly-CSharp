using System;
using GameServer.Logic.TuJian;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	public class OccupationUtil
	{
		public static int GetOccuDamageType(int occuationIndex)
		{
			int result;
			switch (occuationIndex)
			{
			case 0:
			case 2:
			case 3:
				result = 0;
				break;
			case 1:
			case 4:
			case 5:
				result = 1;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		public static bool CostMoney(GameClient client, int moneyType, int modifyValue, ref string strCostList, string logMsg)
		{
			bool flag = false;
			if (moneyType <= 40)
			{
				if (moneyType <= 8)
				{
					if (moneyType == 1)
					{
						flag = GameManager.ClientMgr.SubMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg);
						goto IL_321;
					}
					if (moneyType == 8)
					{
						flag = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
						goto IL_321;
					}
				}
				else
				{
					switch (moneyType)
					{
					case 13:
						flag = GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					case 14:
						break;
					case 15:
						flag = Global.AddZaJinDanJiFen(client, -modifyValue, logMsg, false);
						goto IL_321;
					default:
						if (moneyType == 40)
						{
							flag = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false, false, false, DaiBiSySType.None);
							goto IL_321;
						}
						break;
					}
				}
			}
			else if (moneyType <= 109)
			{
				if (moneyType == 50)
				{
					flag = GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, modifyValue, logMsg, false);
					goto IL_321;
				}
				switch (moneyType)
				{
				case 99:
					flag = true;
					GameManager.ClientMgr.ModifyLangHunFenMoValue(client, -modifyValue, logMsg, true, true);
					goto IL_321;
				case 101:
					flag = GameManager.ClientMgr.ModifyZaiZaoValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 106:
					flag = GameManager.ClientMgr.ModifyMUMoHeValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 107:
					flag = GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, -modifyValue, logMsg, true, false);
					goto IL_321;
				case 108:
					flag = true;
					SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, -modifyValue, logMsg);
					goto IL_321;
				case 109:
					flag = GameManager.FluorescentGemMgr.DecFluorescentPoint(client, modifyValue, logMsg, false);
					goto IL_321;
				}
			}
			else
			{
				switch (moneyType)
				{
				case 119:
					flag = GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, -modifyValue, logMsg, true, true, false);
					goto IL_321;
				case 120:
					flag = true;
					GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, -modifyValue, logMsg, true, true);
					goto IL_321;
				default:
					switch (moneyType)
					{
					case 129:
						flag = GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					case 130:
						flag = GameManager.ClientMgr.ModifyAlchemyElementValue(client, -modifyValue, logMsg, true, false);
						goto IL_321;
					case 131:
						break;
					case 132:
						flag = GameManager.ClientMgr.ModifyJueXingValue(client, -modifyValue, logMsg, false);
						goto IL_321;
					case 133:
						flag = GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, -modifyValue, logMsg, true, true, false);
						goto IL_321;
					default:
						switch (moneyType)
						{
						case 139:
							flag = GameManager.ClientMgr.ModifyHunJingValue(client, -modifyValue, logMsg, true, true, false);
							goto IL_321;
						case 140:
							flag = GameManager.ClientMgr.ModifyMountPointValue(client, -modifyValue, logMsg, true, true, false);
							goto IL_321;
						}
						break;
					}
					break;
				}
			}
			LogManager.WriteLog(2, " CheckHasMoney 不支持的货币类型:" + moneyType, null, true);
			IL_321:
			if (flag)
			{
				strCostList += EventLogManager.AddResPropString(strCostList, (ResLogType)moneyType, new object[]
				{
					-modifyValue
				});
			}
			return flag;
		}
	}
}
