using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	public class RegressActiveDayBuy : Activity, IEventListener
	{
		public void Dispose()
		{
			GlobalEventSource.getInstance().removeListener(36, this);
		}

		public bool Init()
		{
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config\\HuiGuiDayZhiGou.xml"));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config\\HuiGuiDayZhiGou.xml"));
			bool result;
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", "Config\\HuiGuiDayZhiGou.xml"), null, true);
				result = false;
			}
			else
			{
				this.ActivityType = 113;
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				try
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						RegressActiveDayBuyXML regressActiveDayBuyXML = new RegressActiveDayBuyXML();
						Dictionary<int, int> dictionary = new Dictionary<int, int>();
						regressActiveDayBuyXML.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
						string[] array = Global.GetSafeAttributeStr(xml, "Price").Split(new char[]
						{
							'|'
						});
						regressActiveDayBuyXML.ZhiGouID = Convert.ToInt32(array[2]);
						regressActiveDayBuyXML.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HuoDongLevel"));
						string[] array2 = Global.GetSafeAttributeStr(xml, "TotalYuanBao").Split(new char[]
						{
							','
						});
						dictionary.Add(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]));
						regressActiveDayBuyXML.TotalYuanBao = dictionary;
						regressActiveDayBuyXML.Day = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Day"));
						regressActiveDayBuyXML.Max = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Max"));
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID1");
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "GoodsID2");
						this.regressActiveDayBuyXML.Add(regressActiveDayBuyXML.ID, regressActiveDayBuyXML);
						this.ActZhiGouIDSet.Add(regressActiveDayBuyXML.ZhiGouID);
						UserMoneyMgr.getInstance().CheckChargeItemConfigLogic(regressActiveDayBuyXML.ZhiGouID, regressActiveDayBuyXML.Max, safeAttributeStr, safeAttributeStr2, string.Format("三周年直购 ID={0}", regressActiveDayBuyXML.ID));
					}
					if (this.regressActiveDayBuyXML == null || this.ActZhiGouIDSet == null)
					{
						return false;
					}
					base.PredealDateTime();
					GlobalEventSource.getInstance().registerListener(36, this);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = true;
			}
			return result;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public bool CheckValidChargeItem(GameClient client, int zhigouID)
		{
			bool result;
			if (!this.ActZhiGouIDSet.Contains(zhigouID))
			{
				result = true;
			}
			else
			{
				RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
				if (null == regressActiveOpen)
				{
					result = false;
				}
				else if (!regressActiveOpen.InActivityTime())
				{
					result = true;
				}
				else if (!regressActiveOpen.CanGiveAward())
				{
					result = false;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					string s = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
					int num = Global.GetOffsetDay(DateTime.Parse(s)) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
					string arg = "2011-11-11 00$00$00";
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, arg, regressActiveOpen.FromDate.Replace(':', '$'));
					string[] array;
					if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
					{
						result = false;
					}
					else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
					{
						result = false;
					}
					else
					{
						int num2 = Convert.ToInt32(array[1]);
						if (num2 < 0)
						{
							num2 = 0;
						}
						string text;
						if (!UserRegressActiveManager.GetRegressMinRegtime(client, out text) || text == null || text.Equals(""))
						{
							result = false;
						}
						else
						{
							int num3;
							int userActiveFile = regressActiveOpen.GetUserActiveFile(text, out num3);
							if (0 == userActiveFile)
							{
								result = false;
							}
							else
							{
								foreach (RegressActiveDayBuyXML regressActiveDayBuyXML in this.regressActiveDayBuyXML.Values)
								{
									if (regressActiveDayBuyXML.ZhiGouID == zhigouID)
									{
										foreach (KeyValuePair<int, int> keyValuePair in regressActiveDayBuyXML.TotalYuanBao)
										{
											if (keyValuePair.Value == -1 && num2 < keyValuePair.Key)
											{
												return false;
											}
											if ((keyValuePair.Value != -1 && num2 > keyValuePair.Value) || num2 < keyValuePair.Key)
											{
												return false;
											}
										}
										if (num + 1 != regressActiveDayBuyXML.Day)
										{
											return false;
										}
										return true;
									}
								}
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		public Dictionary<int, int> BuildRegressZhiGouInfoForClient(GameClient client)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			Dictionary<int, int> result;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = null;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = null;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				string s = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int num = Global.GetOffsetDay(DateTime.Parse(s)) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
				string arg = "2011-11-11 00$00$00";
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, arg, regressActiveOpen.FromDate.Replace(':', '$'));
				string[] array;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
				{
					result = null;
				}
				else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
				{
					result = null;
				}
				else
				{
					int num2 = Convert.ToInt32(array[1]);
					if (num2 < 0)
					{
						num2 = 0;
					}
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					foreach (RegressActiveDayBuyXML regressActiveDayBuyXML in this.regressActiveDayBuyXML.Values)
					{
						bool flag = true;
						foreach (KeyValuePair<int, int> keyValuePair in regressActiveDayBuyXML.TotalYuanBao)
						{
							if (keyValuePair.Value == -1 && num2 < keyValuePair.Key)
							{
								flag = false;
								break;
							}
							if (keyValuePair.Value != -1 && (num2 > keyValuePair.Value || num2 < keyValuePair.Key))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							if (num + 1 == regressActiveDayBuyXML.Day)
							{
								dictionary[regressActiveDayBuyXML.ID] = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, regressActiveDayBuyXML.ZhiGouID);
							}
						}
					}
					result = dictionary;
				}
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 36)
			{
				ChargeItemBaseEventObject chargeItemBaseEventObject = eventObject as ChargeItemBaseEventObject;
				if (this.CheckValidChargeItem(chargeItemBaseEventObject.Player, chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID))
				{
					Dictionary<int, int> cmdData = this.BuildRegressZhiGouInfoForClient(chargeItemBaseEventObject.Player);
					chargeItemBaseEventObject.Player.sendCmd<Dictionary<int, int>>(2077, cmdData, false);
					if (chargeItemBaseEventObject.Player._IconStateMgr.CheckThemeZhiGou(chargeItemBaseEventObject.Player))
					{
						chargeItemBaseEventObject.Player._IconStateMgr.SendIconStateToClient(chargeItemBaseEventObject.Player);
					}
				}
			}
		}

		public bool CheckClientCanBuy(GameClient client)
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			bool result;
			if (regressActiveOpen == null || !regressActiveOpen.InActivityTime())
			{
				result = false;
			}
			else if (!regressActiveOpen.CanGiveAward())
			{
				result = false;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				string s = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second).ToString("yyyy-MM-dd HH:mm:ss");
				int num = Global.GetOffsetDay(DateTime.Parse(s)) - Global.GetOffsetDay(DateTime.Parse(regressActiveOpen.FromDate));
				string arg = "2011-11-11 00$00$00";
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, arg, regressActiveOpen.FromDate.Replace(':', '$'));
				string[] array;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14136, strcmd, out array, 0))
				{
					result = false;
				}
				else if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != client.ClientData.RoleID)
				{
					result = false;
				}
				else
				{
					int num2 = Convert.ToInt32(array[1]);
					if (num2 < 0)
					{
						num2 = 0;
					}
					foreach (RegressActiveDayBuyXML regressActiveDayBuyXML in this.regressActiveDayBuyXML.Values)
					{
						bool flag = true;
						foreach (KeyValuePair<int, int> keyValuePair in regressActiveDayBuyXML.TotalYuanBao)
						{
							if (keyValuePair.Value == -1 && num2 < keyValuePair.Key)
							{
								flag = false;
								break;
							}
							if (keyValuePair.Value != -1 && (num2 > keyValuePair.Value || num2 < keyValuePair.Key))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							if (num == regressActiveDayBuyXML.Day)
							{
								int chargeItemPurchaseNum = UserMoneyMgr.getInstance().GetChargeItemPurchaseNum(client, regressActiveDayBuyXML.ZhiGouID);
								if (regressActiveDayBuyXML.Max <= 0 || chargeItemPurchaseNum < regressActiveDayBuyXML.Max)
								{
									return true;
								}
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		protected const string RegressActiveDayBuyXml = "Config\\HuiGuiDayZhiGou.xml";

		private Dictionary<int, RegressActiveDayBuyXML> regressActiveDayBuyXML = new Dictionary<int, RegressActiveDayBuyXML>();

		public HashSet<int> ActZhiGouIDSet = new HashSet<int>();
	}
}
