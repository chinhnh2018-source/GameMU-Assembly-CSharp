using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.UserMoneyCharge;
using Server.Data;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriSuperInputActivity : Activity
	{
		public bool Init()
		{
			try
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("SuperChongZhiFanLi");
				if (string.IsNullOrEmpty(paramValueByName))
				{
					LogManager.WriteLog(1, string.Format("解析大型节日充值返利活动配置文件中的SuperChongZhiFanLi失败", new object[0]), null, true);
					return false;
				}
				string[] array = paramValueByName.Split(new char[]
				{
					'|'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(1, string.Format("解析大型节日充值返利活动配置文件中的SuperChongZhiFanLi失败", new object[0]), null, true);
					return false;
				}
				this.FromDate = array[0];
				this.ToDate = array[1];
				this.ActivityType = 71;
				this.AwardStartDate = this.FromDate;
				this.AwardEndDate = this.ToDate;
				base.PredealDateTime();
				string text = GameManager.GameConfigMgr.GetGameConfigItemStr("platformtype", "app");
				text = text.ToLower();
				string sectionKey = string.Empty;
				if (text == "app")
				{
					sectionKey = "dl_app";
				}
				else if (text == "yueyu")
				{
					sectionKey = "dl_yueyu";
				}
				else if (text == "andrid" || text == "android" || text == "yyb")
				{
					sectionKey = "dl_android";
				}
				else
				{
					sectionKey = "dl_app";
				}
				this.JieriSuperInputDict.Clear();
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/MU_ChongZhiFanLi.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/MU_ChongZhiFanLi.xml"));
				if (xelement != null)
				{
					IEnumerable<XElement> enumerable = xelement.Elements().First((XElement _xml) => _xml.Attribute("TypeID").Value.ToString().ToLower() == sectionKey).Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (null != xelement2)
						{
							JieriSuperInputData jieriSuperInputData = new JieriSuperInputData();
							jieriSuperInputData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							jieriSuperInputData.MutiNum = (int)Global.GetSafeAttributeLong(xelement2, "Num");
							jieriSuperInputData.PurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "SinglePurchase");
							jieriSuperInputData.FullPurchaseNum = (int)Global.GetSafeAttributeLong(xelement2, "FullPurchase");
							DateTime d;
							DateTime.TryParse(Global.GetSafeAttributeStr(xelement2, "Data"), out d);
							TimeSpan t;
							TimeSpan.TryParse(Global.GetSafeAttributeStr(xelement2, "BeginTime"), out t);
							TimeSpan t2;
							TimeSpan.TryParse(Global.GetSafeAttributeStr(xelement2, "EndTime"), out t2);
							jieriSuperInputData.BeginTime = d + t;
							jieriSuperInputData.EndTime = d + t2;
							this.JieriSuperInputDict[jieriSuperInputData.ID] = jieriSuperInputData;
						}
					}
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("SuperChongZhiFanLiOpen");
				if (!string.IsNullOrEmpty(paramValueByName2))
				{
					string[] array2 = paramValueByName2.Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array2)
					{
						string[] array4 = text2.Split(new char[]
						{
							','
						});
						if (array4.Length == 2)
						{
							dictionary[Global.SafeConvertToInt32(array4[0])] = Global.SafeConvertToInt32(array4[1]);
						}
					}
				}
				dictionary.TryGetValue(UserMoneyMgr.getInstance().GetActivityPlatformType(), out this.PlatformOpenStateVavle);
				if (!this.InActivityTime())
				{
					GameManager.ClientMgr.NotifyAllActivityState(11, 0, "", "", 0);
				}
				else
				{
					GameManager.ClientMgr.NotifyAllActivityState(11, this.PlatformOpenStateVavle, "", "", 0);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/MU_ChongZhiFanLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void SaveFullPurchaseList(List<int> countList)
		{
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				string paramValue = string.Join<int>(",", countList.ToArray());
				GameManager.GameConfigMgr.SetGameConfigItem("czfl_fullpurnum", paramValue);
				Global.UpdateDBGameConfigg("czfl_fullpurnum", paramValue);
			}
		}

		public List<int> GetFullPurchaseList(DateTime now)
		{
			List<int> result;
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("czfl_fullpurnum", "");
				string[] array = gameConfigItemStr.Split(new char[]
				{
					','
				});
				List<int> list = new List<int>();
				foreach (string str in array)
				{
					list.Add(Global.SafeConvertToInt32(str));
				}
				if (list.Count != 5)
				{
					for (int j = list.Count; j < 5; j++)
					{
						list.Add(0);
					}
				}
				int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				JieriSuperInputData jieriSuperInputDataByNowDateTime = this.GetJieriSuperInputDataByNowDateTime(now, false);
				if (null != jieriSuperInputDataByNowDateTime)
				{
					if (list[0] != offsetDay)
					{
						list[0] = offsetDay;
						list[1] = jieriSuperInputDataByNowDateTime.ID;
						list[2] = jieriSuperInputDataByNowDateTime.FullPurchaseNum;
						list[3] = 0;
						list[4] = 0;
						this.SaveFullPurchaseList(list);
					}
					else if (list[1] != jieriSuperInputDataByNowDateTime.ID)
					{
						if (now >= jieriSuperInputDataByNowDateTime.BeginTime && now <= jieriSuperInputDataByNowDateTime.EndTime)
						{
							list[1] = jieriSuperInputDataByNowDateTime.ID;
							int num = list[3] - list[4];
							list[2] = jieriSuperInputDataByNowDateTime.FullPurchaseNum + num;
							list[3] = 0;
							list[4] = 0;
							this.SaveFullPurchaseList(list);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					11,
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
					11,
					this.PlatformOpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		private JieriSuperInputData GetJieriSuperInputDataByNowDateTime(DateTime now, bool skipBegin = false)
		{
			JieriSuperInputData jieriSuperInputData = null;
			List<JieriSuperInputData> list = this.JieriSuperInputDict.Values.ToList<JieriSuperInputData>().FindAll((JieriSuperInputData x) => x.BeginTime.DayOfYear == now.DayOfYear);
			list.Sort(delegate(JieriSuperInputData left, JieriSuperInputData right)
			{
				int result2;
				if (left.BeginTime.Ticks < right.BeginTime.Ticks)
				{
					result2 = -1;
				}
				else if (left.BeginTime.Ticks > right.BeginTime.Ticks)
				{
					result2 = 1;
				}
				else
				{
					result2 = 0;
				}
				return result2;
			});
			JieriSuperInputData result;
			if (list.Count == 0)
			{
				result = jieriSuperInputData;
			}
			else
			{
				foreach (JieriSuperInputData jieriSuperInputData2 in list)
				{
					if (!skipBegin || !(now >= jieriSuperInputData2.BeginTime))
					{
						if (now <= jieriSuperInputData2.EndTime)
						{
							jieriSuperInputData = jieriSuperInputData2;
							break;
						}
					}
				}
				if (null == jieriSuperInputData)
				{
					jieriSuperInputData = list[list.Count - 1];
				}
				result = jieriSuperInputData;
			}
			return result;
		}

		public string ExecuteSuperInput(GameClient client)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			string result = "";
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			lock (JieriSuperInputActivity._SuperInputMutex)
			{
				if (!this.InActivityTime() || this.PlatformOpenStateVavle == 0)
				{
					num4 = -400;
				}
				else
				{
					JieriSuperInputData jieriSuperInputDataByNowDateTime = this.GetJieriSuperInputDataByNowDateTime(dateTime, false);
					if (jieriSuperInputDataByNowDateTime == null || dateTime < jieriSuperInputDataByNowDateTime.BeginTime)
					{
						num4 = -400;
					}
					else
					{
						List<int> fullPurchaseList = this.GetFullPurchaseList(dateTime);
						num = fullPurchaseList[2];
						num2 = fullPurchaseList[3];
						if (num2 >= num)
						{
							num4 = -16;
						}
						else
						{
							string arg = jieriSuperInputDataByNowDateTime.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							string arg2 = jieriSuperInputDataByNowDateTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							string text = string.Format("has_{0}_{1}_{2}", arg, arg2, jieriSuperInputDataByNowDateTime.ID);
							if (jieriSuperInputDataByNowDateTime.PurchaseNum > 0)
							{
								string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									client.ClientData.RoleID,
									text,
									this.ActivityType,
									"0"
								});
								string[] array = Global.ExecuteDBCmd(10221, strcmd, 0);
								if (array == null || array.Length == 0)
								{
									return result;
								}
								num3 = Global.SafeConvertToInt32(array[3]);
							}
							if (jieriSuperInputDataByNowDateTime.PurchaseNum > 0 && num3 >= jieriSuperInputDataByNowDateTime.PurchaseNum)
							{
								num4 = -16;
							}
							else
							{
								string keyStr = string.Format("res_{0}_{1}_{2}", arg, arg2, jieriSuperInputDataByNowDateTime.ID);
								string[] array = Global.QeuryUserActivityInfo(client, keyStr, this.ActivityType, "0");
								if (array == null || array.Length == 0)
								{
									return result;
								}
								int num5 = Global.SafeConvertToInt32(array[3]);
								num2++;
								List<int> list;
								(list = fullPurchaseList)[3] = list[3] + 1;
								this.SaveFullPurchaseList(fullPurchaseList);
								Global.UpdateUserActivityInfo(client, text, 71, (long)(++num3), dateTime.ToString("yyyy-MM-dd HH$mm$ss"));
								Global.UpdateUserActivityInfo(client, keyStr, 71, (long)(num5 + 1), dateTime.ToString("yyyy-MM-dd HH$mm$ss"));
							}
						}
					}
				}
			}
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				num4,
				num3,
				num,
				num2
			});
		}

		public string BuildSuperInputFanLiActInfoForClient(GameClient client)
		{
			string text = "";
			string result;
			if (!this.InActivityTime() || this.PlatformOpenStateVavle == 0)
			{
				text = string.Format("{0},{1},{2}", 0, 0, 0);
				result = text;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				JieriSuperInputData jieriSuperInputDataByNowDateTime = this.GetJieriSuperInputDataByNowDateTime(dateTime, false);
				if (null == jieriSuperInputDataByNowDateTime)
				{
					text = string.Format("{0},{1},{2}", 0, 0, 0);
					result = text;
				}
				else if (dateTime < jieriSuperInputDataByNowDateTime.BeginTime)
				{
					text = string.Format("{0},{1},{2}", 0, jieriSuperInputDataByNowDateTime.FullPurchaseNum, 0);
					result = text;
				}
				else
				{
					List<int> fullPurchaseList = this.GetFullPurchaseList(dateTime);
					int num = fullPurchaseList[2];
					int num2 = fullPurchaseList[3];
					int num3 = 0;
					string arg = jieriSuperInputDataByNowDateTime.BeginTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
					string arg2 = jieriSuperInputDataByNowDateTime.EndTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
					string text2 = string.Format("has_{0}_{1}_{2}", arg, arg2, jieriSuperInputDataByNowDateTime.ID);
					if (jieriSuperInputDataByNowDateTime.PurchaseNum > 0)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							text2,
							this.ActivityType,
							"0"
						});
						string[] array = Global.ExecuteDBCmd(10221, strcmd, 0);
						if (array == null || array.Length == 0)
						{
							return text;
						}
						num3 = Global.SafeConvertToInt32(array[3]);
					}
					if (fullPurchaseList[3] >= fullPurchaseList[2])
					{
						JieriSuperInputData jieriSuperInputDataByNowDateTime2 = this.GetJieriSuperInputDataByNowDateTime(dateTime, true);
						if (null != jieriSuperInputDataByNowDateTime2)
						{
							return string.Format("{0},{1},{2}", 0, jieriSuperInputDataByNowDateTime2.FullPurchaseNum, jieriSuperInputDataByNowDateTime2.FullPurchaseNum);
						}
					}
					text = string.Format("{0},{1},{2}", num3, num, num2);
					result = text;
				}
			}
			return result;
		}

		public void FilterSingleChargeData(SingleChargeData data)
		{
			if (null != data)
			{
				if (this.PlatformOpenStateVavle == 0)
				{
					data.SuperInputFanLiKey = "";
					data.SuperInputFanLiDict.Clear();
				}
				else
				{
					data.SuperInputFanLiDict.Clear();
					data.SuperInputFanLiKey = string.Format("{0}_{1}", this.FromDate, this.ToDate);
					foreach (JieriSuperInputData jieriSuperInputData in this.JieriSuperInputDict.Values)
					{
						data.SuperInputFanLiDict[jieriSuperInputData.ID] = jieriSuperInputData;
					}
				}
			}
		}

		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney, int superInputFanLi)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			JieriSuperInputData jieriSuperInputData = null;
			if (this.JieriSuperInputDict.TryGetValue(superInputFanLi, out jieriSuperInputData))
			{
				GameClient gameClient = null;
				TMSKSocket tmsksocket = GameManager.OnlineUserSession.FindSocketByUserID(userid);
				if (null != tmsksocket)
				{
					gameClient = GameManager.ClientMgr.FindClient(tmsksocket);
				}
				lock (JieriSuperInputActivity._SuperInputMutex)
				{
					List<int> fullPurchaseList = this.GetFullPurchaseList(dateTime);
					if (fullPurchaseList[0] == TimeUtil.GetOffsetDay(dateTime) && fullPurchaseList[1] == superInputFanLi)
					{
						List<int> list;
						(list = fullPurchaseList)[4] = list[4] + 1;
						this.SaveFullPurchaseList(fullPurchaseList);
					}
				}
				if (null == gameClient)
				{
					if (jieriSuperInputData.MutiNum > 0)
					{
						GameManager.ClientMgr.AddOfflineUserMoney(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, roleid, roleid.ToString(), (jieriSuperInputData.MutiNum - 1) * Global.TransMoneyToYuanBao(addMoney), "节日超级充值返利钻石(离线)", 0, userid);
					}
				}
				else
				{
					if (jieriSuperInputData.MutiNum > 0)
					{
						GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, (jieriSuperInputData.MutiNum - 1) * Global.TransMoneyToYuanBao(addMoney), "节日超级充值返利钻石", ActivityTypes.None, "");
					}
					if (jieriSuperInputData.PurchaseNum > 0 && dateTime >= jieriSuperInputData.BeginTime && dateTime <= jieriSuperInputData.EndTime)
					{
						string cmdData = this.BuildSuperInputFanLiActInfoForClient(gameClient);
						gameClient.sendCmd(1622, cmdData, false);
					}
				}
			}
		}

		protected const string JieriSuperInputActivityData_fileName = "Config/MU_ChongZhiFanLi.xml";

		private static object _SuperInputMutex = new object();

		protected Dictionary<int, JieriSuperInputData> JieriSuperInputDict = new Dictionary<int, JieriSuperInputData>();

		public int PlatformOpenStateVavle = 0;
	}
}
