using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.MUWings
{
	public class ZhuLingZhuHunManager
	{
		private ZhuLingZhuHunManager()
		{
		}

		public static void LoadConfig()
		{
			string text = "Config/ZhuLingType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", text), null, true);
			}
			else
			{
				XElement xelement2 = xelement.Element("Types");
				if (xelement2 == null)
				{
					LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", text), null, true);
				}
				else
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xml in enumerable)
					{
						int num = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID");
						int num2 = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "CostBandJinBi"));
						string[] array = safeAttributeStr.Split(new char[]
						{
							','
						});
						if (array.Length != 2)
						{
							LogManager.WriteLog(2, string.Format("加载{0}时出错!!! ID={1} 消耗物品配置错误", text, num), null, true);
						}
						else
						{
							int num3 = Convert.ToInt32(array[0]);
							int num4 = Convert.ToInt32(array[1]);
							if (num == 1)
							{
								ZhuLingZhuHunManager.ZhuLingCostGoodsID = num3;
								ZhuLingZhuHunManager.ZhuLingCostGoodsNum = num4;
								ZhuLingZhuHunManager.ZhuLingCostJinBi = num2;
							}
							else if (num == 2)
							{
								ZhuLingZhuHunManager.ZhuHunCostGoodsID = num3;
								ZhuLingZhuHunManager.ZhuHunCostGoodsNum = num4;
								ZhuLingZhuHunManager.ZhuHunCostJinBi = num2;
							}
						}
					}
				}
			}
			text = "Config/MaxWinZhuLing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
			xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", text), null, true);
			}
			else
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					ZhuLingZhuHunLimit zhuLingZhuHunLimit = new ZhuLingZhuHunLimit();
					zhuLingZhuHunLimit.SuitID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "SuitID"));
					zhuLingZhuHunLimit.ZhuLingLimit = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "PlainZhuLing"));
					zhuLingZhuHunLimit.ZhuHunLimit = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "SeniorZhuLing"));
					ZhuLingZhuHunManager.Limit.Add(zhuLingZhuHunLimit);
				}
			}
			text = "Config/WinZhuLing.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
			xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
			if (xelement == null)
			{
				LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件不存在", text), null, true);
			}
			else
			{
				for (int i = 0; i < 6; i++)
				{
					ZhuLingZhuHunManager.Effect.Add(new ZhuLingZhuHunEffect());
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num5 = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "TypeID"));
					int num6 = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Occupation"));
					if (num6 < 0 || num6 >= ZhuLingZhuHunManager.Effect.Count<ZhuLingZhuHunEffect>())
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!! 职业配置有问题", text), null, true);
					}
					else
					{
						ZhuLingZhuHunManager.Effect[num6].Occupation = num6;
						if (num5 == 1)
						{
							ZhuLingZhuHunManager.Effect[num6].MaxAttackV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MaxAttackV"));
							ZhuLingZhuHunManager.Effect[num6].MaxMAttackV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MaxMAttackV"));
							ZhuLingZhuHunManager.Effect[num6].MaxDefenseV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MaxDefenseV"));
							ZhuLingZhuHunManager.Effect[num6].MaxMDefenseV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MaxMDefenseV"));
							ZhuLingZhuHunManager.Effect[num6].LifeV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LifeV"));
							ZhuLingZhuHunManager.Effect[num6].HitV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HitV"));
							ZhuLingZhuHunManager.Effect[num6].DodgeV = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "DodgeV"));
						}
						else if (num5 == 2)
						{
							ZhuLingZhuHunManager.Effect[num6].AllAttribute = Global.GetSafeAttributeDouble(xml, "AllAttribute");
						}
					}
				}
			}
		}

		public static double GetZhuLingPct(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!GlobalNew.IsGongNengOpened(client, 51, false))
			{
				result = num;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (limit == null)
				{
					result = num;
				}
				else
				{
					num = (double)client.ClientData.MyWingData.ZhuLingNum / (double)limit.ZhuLingLimit;
					result = num;
				}
			}
			return result;
		}

		public static bool IfZhuLingPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 51, false))
			{
				result = false;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				result = (limit != null && client.ClientData.MyWingData.ZhuLingNum >= limit.ZhuLingLimit);
			}
			return result;
		}

		public static void SetZhuLingMax_GM(GameClient client)
		{
			ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
			if (limit != null)
			{
				client.ClientData.MyWingData.ZhuLingNum = limit.ZhuLingLimit;
				MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
				ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				if (client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public static ZhuLingZhuHunError ReqZhuLing(GameClient client)
		{
			int zhuLingNum = client.ClientData.MyWingData.ZhuLingNum;
			int yinLiang = client.ClientData.YinLiang;
			int money = client.ClientData.Money1;
			ZhuLingZhuHunError result;
			if (!GlobalNew.IsGongNengOpened(client, 51, false))
			{
				result = ZhuLingZhuHunError.ZhuLingNotOpen;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (limit == null)
				{
					result = ZhuLingZhuHunError.ErrorConfig;
				}
				else if (client.ClientData.MyWingData.ZhuLingNum >= limit.ZhuLingLimit)
				{
					result = ZhuLingZhuHunError.ZhuLingFull;
				}
				else if (Global.GetTotalGoodsCountByID(client, ZhuLingZhuHunManager.ZhuLingCostGoodsID) < ZhuLingZhuHunManager.ZhuLingCostGoodsNum)
				{
					result = ZhuLingZhuHunError.ZhuLingMaterialNotEnough;
				}
				else if (Global.GetTotalBindTongQianAndTongQianVal(client) < ZhuLingZhuHunManager.ZhuLingCostJinBi)
				{
					result = ZhuLingZhuHunError.ZhuLingJinBiNotEnough;
				}
				else if (!Global.SubBindTongQianAndTongQian(client, ZhuLingZhuHunManager.ZhuLingCostJinBi, "注灵消耗金币"))
				{
					result = ZhuLingZhuHunError.DBSERVERERROR;
				}
				else
				{
					string text = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
					{
						-ZhuLingZhuHunManager.ZhuLingCostJinBi,
						yinLiang,
						client.ClientData.YinLiang,
						money,
						client.ClientData.Money1
					});
					bool flag = true;
					bool flag2 = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ZhuLingZhuHunManager.ZhuLingCostGoodsID, ZhuLingZhuHunManager.ZhuLingCostGoodsNum, false, out flag, out flag2, false))
					{
						result = ZhuLingZhuHunError.DBSERVERERROR;
					}
					else
					{
						GoodsData goodsData = new GoodsData
						{
							GoodsID = ZhuLingZhuHunManager.ZhuLingCostGoodsID,
							GCount = ZhuLingZhuHunManager.ZhuLingCostGoodsNum
						};
						text += EventLogManager.AddGoodsDataPropString(goodsData);
						int num = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum + 1, client.ClientData.MyWingData.ZhuHunNum);
						if (num < 0)
						{
							result = ZhuLingZhuHunError.DBSERVERERROR;
						}
						else
						{
							client.ClientData.MyWingData.ZhuLingNum++;
							ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							EventLogManager.AddWingZhuLingEvent(client, zhuLingNum, client.ClientData.MyWingData.ZhuLingNum, text);
							if (client._IconStateMgr.CheckReborn(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = ZhuLingZhuHunError.Success;
						}
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessReqZhuLing(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ZhuLingZhuHunError zhuLingZhuHunError = ZhuLingZhuHunManager.ReqZhuLing(gameClient);
				string data2 = string.Format("{0}:{1}:{2}", num, (int)zhuLingZhuHunError, gameClient.ClientData.MyWingData.ZhuLingNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessReqZhuLing", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static double GetZhuHunPct(GameClient client)
		{
			double num = 0.0;
			double result;
			if (!GlobalNew.IsGongNengOpened(client, 52, false))
			{
				result = num;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (limit == null)
				{
					result = num;
				}
				else
				{
					num = (double)client.ClientData.MyWingData.ZhuHunNum / (double)limit.ZhuHunLimit;
					result = num;
				}
			}
			return result;
		}

		public static bool IfZhuHunPerfect(GameClient client)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 52, false))
			{
				result = false;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				result = (limit != null && client.ClientData.MyWingData.ZhuHunNum >= limit.ZhuHunLimit);
			}
			return result;
		}

		public static void SetZhuHunMax_GM(GameClient client)
		{
			ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
			if (limit != null)
			{
				client.ClientData.MyWingData.ZhuHunNum = limit.ZhuHunLimit;
				MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
				ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				if (client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public static ZhuLingZhuHunError ReqZhuHun(GameClient client)
		{
			int zhuHunNum = client.ClientData.MyWingData.ZhuHunNum;
			int yinLiang = client.ClientData.YinLiang;
			int money = client.ClientData.Money1;
			ZhuLingZhuHunError result;
			if (!GlobalNew.IsGongNengOpened(client, 52, false))
			{
				result = ZhuLingZhuHunError.ZhuHunNotOpen;
			}
			else
			{
				ZhuLingZhuHunLimit limit = ZhuLingZhuHunManager.GetLimit(client.ClientData.MyWingData.WingID);
				if (limit == null)
				{
					result = ZhuLingZhuHunError.ErrorConfig;
				}
				else if (client.ClientData.MyWingData.ZhuHunNum >= limit.ZhuHunLimit)
				{
					result = ZhuLingZhuHunError.ZhuHunFull;
				}
				else if (Global.GetTotalGoodsCountByID(client, ZhuLingZhuHunManager.ZhuHunCostGoodsID) < ZhuLingZhuHunManager.ZhuHunCostGoodsNum)
				{
					result = ZhuLingZhuHunError.ZhuHunMaterialNotEnough;
				}
				else if (Global.GetTotalBindTongQianAndTongQianVal(client) < ZhuLingZhuHunManager.ZhuHunCostJinBi)
				{
					result = ZhuLingZhuHunError.ZhuHunJinBiNotEnough;
				}
				else if (!Global.SubBindTongQianAndTongQian(client, ZhuLingZhuHunManager.ZhuHunCostJinBi, "注魂消耗"))
				{
					result = ZhuLingZhuHunError.DBSERVERERROR;
				}
				else
				{
					string text = EventLogManager.NewResPropString(ResLogType.SubJinbi, new object[]
					{
						-ZhuLingZhuHunManager.ZhuHunCostJinBi,
						yinLiang,
						client.ClientData.YinLiang,
						money,
						client.ClientData.Money1
					});
					bool flag = true;
					bool flag2 = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ZhuLingZhuHunManager.ZhuHunCostGoodsID, ZhuLingZhuHunManager.ZhuHunCostGoodsNum, false, out flag, out flag2, false))
					{
						result = ZhuLingZhuHunError.DBSERVERERROR;
					}
					else
					{
						GoodsData goodsData = new GoodsData
						{
							GoodsID = ZhuLingZhuHunManager.ZhuHunCostGoodsID,
							GCount = ZhuLingZhuHunManager.ZhuHunCostGoodsNum
						};
						text += EventLogManager.AddGoodsDataPropString(goodsData);
						int num = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.JinJieFailedNum, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum + 1);
						if (num < 0)
						{
							result = ZhuLingZhuHunError.DBSERVERERROR;
						}
						else
						{
							client.ClientData.MyWingData.ZhuHunNum++;
							ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							EventLogManager.AddWingZhuHunEvent(client, zhuHunNum, client.ClientData.MyWingData.ZhuHunNum, text);
							if (client._IconStateMgr.CheckReborn(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = ZhuLingZhuHunError.Success;
						}
					}
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessReqZhuHun(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ZhuLingZhuHunError zhuLingZhuHunError = ZhuLingZhuHunManager.ReqZhuHun(gameClient);
				string data2 = string.Format("{0}:{1}:{2}", num, (int)zhuLingZhuHunError, gameClient.ClientData.MyWingData.ZhuHunNum);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessReqZhuHun", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static void UpdateZhuLingZhuHunProps(GameClient client)
		{
			if (null != client.ClientData.MyWingData)
			{
				if (client.ClientData.MyWingData.WingID > 0)
				{
					ZhuLingZhuHunEffect effect = ZhuLingZhuHunManager.GetEffect(Global.CalcOriginalOccupationID(client));
					if (effect != null)
					{
						double num = 0.0;
						double num2 = 0.0;
						double num3 = 0.0;
						double num4 = 0.0;
						double num5 = 0.0;
						double num6 = 0.0;
						double num7 = 0.0;
						double num8 = 0.0;
						double num9 = 0.0;
						double num10 = 0.0;
						double num11 = 0.0;
						if (client.ClientData.MyWingData.Using == 1)
						{
							num = (double)(effect.MaxAttackV * client.ClientData.MyWingData.ZhuLingNum);
							num3 = (double)(effect.MaxMAttackV * client.ClientData.MyWingData.ZhuLingNum);
							num5 = (double)(effect.MaxDefenseV * client.ClientData.MyWingData.ZhuLingNum);
							num7 = (double)(effect.MaxMDefenseV * client.ClientData.MyWingData.ZhuLingNum);
							num9 = (double)(effect.LifeV * client.ClientData.MyWingData.ZhuLingNum);
							num10 = (double)(effect.HitV * client.ClientData.MyWingData.ZhuLingNum);
							num11 = (double)(effect.DodgeV * client.ClientData.MyWingData.ZhuLingNum);
							double allAttribute = effect.AllAttribute;
							SystemXmlItem systemXmlItem = WingPropsCacheManager.GetWingPropsCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID);
							SystemXmlItem systemXmlItem2 = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel);
							if (systemXmlItem == null)
							{
								systemXmlItem = new SystemXmlItem();
							}
							if (systemXmlItem2 == null)
							{
								systemXmlItem2 = new SystemXmlItem();
							}
							num += (systemXmlItem.GetDoubleValue("MaxAttackV") + systemXmlItem2.GetDoubleValue("MaxAttackV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num2 += (systemXmlItem.GetDoubleValue("MinAttackV") + systemXmlItem2.GetDoubleValue("MinAttackV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num3 += (systemXmlItem.GetDoubleValue("MaxMAttackV") + systemXmlItem2.GetDoubleValue("MaxMAttackV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num4 += (systemXmlItem.GetDoubleValue("MinMAttackV") + systemXmlItem2.GetDoubleValue("MinMAttackV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num5 += (systemXmlItem.GetDoubleValue("MaxDefenseV") + systemXmlItem2.GetDoubleValue("MaxDefenseV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num6 += (systemXmlItem.GetDoubleValue("MinDefenseV") + systemXmlItem2.GetDoubleValue("MinDefenseV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num7 += (systemXmlItem.GetDoubleValue("MaxMDefenseV") + systemXmlItem2.GetDoubleValue("MaxMDefenseV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num8 += (systemXmlItem.GetDoubleValue("MinMDefenseV") + systemXmlItem2.GetDoubleValue("MinMDefenseV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num9 += (systemXmlItem.GetDoubleValue("MaxLifeV") + systemXmlItem2.GetDoubleValue("MaxLifeV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num10 += (systemXmlItem.GetDoubleValue("HitV") + systemXmlItem2.GetDoubleValue("HitV")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
							num11 += (systemXmlItem.GetDoubleValue("Dodge") + systemXmlItem2.GetDoubleValue("Dodge")) * (allAttribute * (double)client.ClientData.MyWingData.ZhuHunNum);
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							8,
							num
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							7,
							num2
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							10,
							num3
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							9,
							num4
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							4,
							num5
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							3,
							num6
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							6,
							num7
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							5,
							num8
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							18,
							num10
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							13,
							num9
						});
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							6,
							19,
							num11
						});
					}
				}
			}
		}

		private static ZhuLingZhuHunEffect GetEffect(int Occupation)
		{
			foreach (ZhuLingZhuHunEffect zhuLingZhuHunEffect in ZhuLingZhuHunManager.Effect)
			{
				if (zhuLingZhuHunEffect.Occupation == Occupation)
				{
					return zhuLingZhuHunEffect;
				}
			}
			return null;
		}

		private static ZhuLingZhuHunLimit GetLimit(int suit)
		{
			foreach (ZhuLingZhuHunLimit zhuLingZhuHunLimit in ZhuLingZhuHunManager.Limit)
			{
				if (zhuLingZhuHunLimit.SuitID == suit)
				{
					return zhuLingZhuHunLimit;
				}
			}
			return null;
		}

		private static int ZhuLingCostGoodsID = 0;

		private static int ZhuLingCostGoodsNum = 0;

		private static int ZhuLingCostJinBi = 0;

		private static int ZhuHunCostGoodsID = 0;

		private static int ZhuHunCostGoodsNum = 0;

		private static int ZhuHunCostJinBi = 0;

		private static List<ZhuLingZhuHunLimit> Limit = new List<ZhuLingZhuHunLimit>();

		private static List<ZhuLingZhuHunEffect> Effect = new List<ZhuLingZhuHunEffect>();
	}
}
