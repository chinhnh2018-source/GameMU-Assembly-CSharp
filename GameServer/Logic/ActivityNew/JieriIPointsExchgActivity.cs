using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriIPointsExchgActivity : Activity
	{
		public void OnMoneyChargeEvent(string userid, int roleid, int addMoney)
		{
			if (this.InActivityTime())
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("JieRiChongZhiDuiHuan");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						':'
					});
					if (array.Length == 2)
					{
						int num = Convert.ToInt32(array[0]);
						if (num != 0)
						{
							double num2 = Convert.ToDouble(array[1]) / (double)num;
							int num3 = (int)(num2 * (double)Global.TransMoneyToYuanBao(addMoney));
							string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								roleid,
								num3,
								this.FromDate.Replace(':', '$'),
								this.ToDate.Replace(':', '$')
							});
							Global.ExecuteDBCmd(13151, strcmd, 0);
						}
					}
				}
			}
		}

		public override bool CheckCondition(GameClient client, int extTag)
		{
			IPointsExchgData pointsExchgData;
			bool result;
			if (!this.AwardItemDict.TryGetValue(extTag, out pointsExchgData))
			{
				result = false;
			}
			else if (this.GetIPointsLeftMergeNum(client, extTag) <= 0)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				string[] array = Global.ExecuteDBCmd(1500, strcmd, client.ServerId);
				result = (array != null && array.Length >= 2 && Convert.ToInt32(array[1]) >= pointsExchgData.MinAwardCondionValue);
			}
			return result;
		}

		public void NotifyInputPointsInfo(GameClient client, bool bPointsOnly = false)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
			string[] array = Global.ExecuteDBCmd(1500, strcmd, client.ServerId);
			if (array != null && array.Length >= 2)
			{
				string text = array[0] + ':' + array[1];
				if (bPointsOnly)
				{
					client.sendCmd(1502, text, false);
				}
				else
				{
					string cmdData = "";
					this.BuildInputPointsDataCmdForClient(client, text, out cmdData);
					client.sendCmd(1500, cmdData, false);
				}
			}
		}

		public override bool GiveAward(GameClient client, int _params)
		{
			IPointsExchgData pointsExchgData;
			bool result;
			if (!this.AwardItemDict.TryGetValue(_params, out pointsExchgData))
			{
				result = false;
			}
			else
			{
				int num = 0;
				if (pointsExchgData.MinAwardCondionValue > 0)
				{
					int num2 = -pointsExchgData.MinAwardCondionValue;
					string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						client.ClientData.RoleID,
						num2,
						this.FromDate.Replace(':', '$'),
						this.ToDate.Replace(':', '$')
					});
					string[] array = Global.ExecuteDBCmd(13151, strcmd, client.ServerId);
					if (array == null || array.Length < 2)
					{
						return false;
					}
					num = Convert.ToInt32(array[1]);
					if (num < 0)
					{
						return false;
					}
				}
				this.ModifyIPointsLeftMergeNum(client, _params, 1);
				base.GiveAward(client, pointsExchgData);
				this.NotifyInputPointsInfo(client, true);
				client._IconStateMgr.CheckJieRiActivity(client, false);
				client._IconStateMgr.SendIconStateToClient(client);
				string castResList = EventLogManager.NewResPropString(ResLogType.InputPoints, new object[]
				{
					-pointsExchgData.MinAwardCondionValue,
					num + pointsExchgData.MinAwardCondionValue,
					num
				});
				string strResList = EventLogManager.MakeGoodsDataPropString(pointsExchgData.GoodsDataList);
				EventLogManager.AddPurchaseEvent(client, 6, _params, castResList, strResList);
				result = true;
			}
			return result;
		}

		public bool Init()
		{
			try
			{
				string uri = "Config/JieRiGifts/ChongZhiDuiHuan.xml";
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							IPointsExchgData pointsExchgData = new IPointsExchgData();
							int key = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							pointsExchgData.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "NeedChongZhiDianShu"));
							pointsExchgData.AwardYuanBao = 0;
							pointsExchgData.DayMaxTimes = (int)Global.GetSafeAttributeLong(xelement3, "MaxNum");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "NewGoodsID");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取大型节日充值点兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("解析大型节日充值点兑换活动配置文件中的物品配置项1失败", new object[0]), null, true);
								}
								else
								{
									pointsExchgData.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日节日充值点兑换配置1");
								}
							}
							this.AwardItemDict[key] = pointsExchgData;
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/JieRiGifts/ChongZhiDuiHuan.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			IPointsExchgData pointsExchgData = null;
			this.AwardItemDict.TryGetValue(id, out pointsExchgData);
			int num = 0;
			if (pointsExchgData != null && pointsExchgData.GoodsDataList != null)
			{
				num += pointsExchgData.GoodsDataList.Count;
			}
			return Global.CanAddGoodsNum(client, num);
		}

		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				string[] array = null;
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.FromDate.Replace(':', '$'), this.ToDate.Replace(':', '$'));
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 1500, strcmd, out array, client.ServerId);
				if (null == array)
				{
					result = false;
				}
				else if (array == null || 2 != array.Length)
				{
					result = false;
				}
				else
				{
					int num = Convert.ToInt32(array[1]);
					if (num <= 0)
					{
						result = false;
					}
					else
					{
						foreach (KeyValuePair<int, IPointsExchgData> keyValuePair in this.AwardItemDict)
						{
							int key = keyValuePair.Key;
							IPointsExchgData value = keyValuePair.Value;
							if (value.MinAwardCondionValue <= num && this.GetIPointsLeftMergeNum(client, keyValuePair.Key) > 0)
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		public void BuildInputPointsDataCmdForClient(GameClient client, string strCmdDB, out string strCmdClient)
		{
			strCmdClient = strCmdDB;
			string[] array = strCmdDB.Split(new char[]
			{
				':'
			});
			if (null != array)
			{
				if (array != null && 2 == array.Length)
				{
					if (!this.InActivityTime())
					{
						strCmdClient = null;
						strCmdClient += array[0];
						strCmdClient += ':';
						strCmdClient += '0';
						strCmdClient += ':';
						foreach (KeyValuePair<int, IPointsExchgData> keyValuePair in this.AwardItemDict)
						{
							strCmdClient += Convert.ToString(keyValuePair.Value.DayMaxTimes);
							strCmdClient += "|";
						}
					}
					else
					{
						strCmdClient += ":";
						foreach (KeyValuePair<int, IPointsExchgData> keyValuePair in this.AwardItemDict)
						{
							strCmdClient += this.GetIPointsLeftMergeNum(client, keyValuePair.Key);
							strCmdClient += '|';
						}
					}
				}
			}
		}

		public int GetIPointsLeftMergeNum(GameClient client, int index)
		{
			JieriIPointsExchgActivity jieriIPointsExchgActivity = HuodongCachingMgr.GetJieriIPointsExchgActivity();
			int result;
			if (null == jieriIPointsExchgActivity)
			{
				result = 0;
			}
			else
			{
				IPointsExchgData pointsExchgData = null;
				this.AwardItemDict.TryGetValue(index, out pointsExchgData);
				if (pointsExchgData == null)
				{
					result = 0;
				}
				else
				{
					DateTime now = DateTime.Parse(this.FromDate);
					int offsetDay = Global.GetOffsetDay(now);
					int num = 0;
					int num2 = 0;
					string name = "InputPointExchg" + index;
					string roleParamByName = Global.GetRoleParamByName(client, name);
					if (null != roleParamByName)
					{
						string[] array = roleParamByName.Split(new char[]
						{
							','
						});
						if (2 == array.Length)
						{
							num = Convert.ToInt32(array[0]);
							num2 = Convert.ToInt32(array[1]);
						}
					}
					if (offsetDay == num)
					{
						result = pointsExchgData.DayMaxTimes - num2;
					}
					else
					{
						result = pointsExchgData.DayMaxTimes;
					}
				}
			}
			return result;
		}

		public int ModifyIPointsLeftMergeNum(GameClient client, int index, int addNum = 1)
		{
			DateTime now = DateTime.Parse(this.FromDate);
			int offsetDay = Global.GetOffsetDay(now);
			string text = "InputPointExchg" + index;
			string roleParamByName = Global.GetRoleParamByName(client, text);
			int num = 0;
			int num2 = 0;
			if (null != roleParamByName)
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (2 != array.Length)
				{
					return 0;
				}
				num = Convert.ToInt32(array[0]);
				num2 = Convert.ToInt32(array[1]);
			}
			if (offsetDay == num)
			{
				num2 += addNum;
			}
			else
			{
				num = offsetDay;
				num2 = addNum;
			}
			string valueString = string.Format("{0},{1}", num, num2);
			Global.SaveRoleParamsStringToDB(client, text, valueString, true);
			return num2;
		}

		protected Dictionary<int, IPointsExchgData> AwardItemDict = new Dictionary<int, IPointsExchgData>();
	}
}
