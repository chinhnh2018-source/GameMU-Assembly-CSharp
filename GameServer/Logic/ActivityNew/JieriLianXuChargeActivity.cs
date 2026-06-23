using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriLianXuChargeActivity : Activity
	{
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.CfgFile));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.CfgFile));
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
				Dictionary<int, JieriLianXuChargeActivity._ChargeLvl> dictionary = new Dictionary<int, JieriLianXuChargeActivity._ChargeLvl>();
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							int num = (int)Global.GetSafeAttributeLong(xelement3, "Group");
							int needCharge = (int)Global.GetSafeAttributeLong(xelement3, "NeedZuanShi");
							JieriLianXuChargeActivity._ChargeLvl chargeLvl = null;
							if (!dictionary.TryGetValue(num, out chargeLvl))
							{
								chargeLvl = new JieriLianXuChargeActivity._ChargeLvl();
								chargeLvl.Id = num;
								chargeLvl.NeedCharge = needCharge;
								dictionary[num] = chargeLvl;
							}
							JieriLianXuChargeActivity._DayAward dayAward = new JieriLianXuChargeActivity._DayAward();
							dayAward.LianXuDay = (int)Global.GetSafeAttributeLong(xelement3, "Day");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项为空", this.CfgFile), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									dayAward.AwardGoods.GoodsDataList.AddRange(HuodongCachingMgr.ParseGoodsDataList(array, "连续充值活动goods1配置"));
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项失败", this.CfgFile), null, true);
								}
								else
								{
									dayAward.AwardGoods.GoodsDataList.AddRange(HuodongCachingMgr.ParseGoodsDataList(array, "连续充值活动goods2配置"));
								}
							}
							chargeLvl.AwardList.Add(dayAward);
						}
					}
					this.chargeLvlList.AddRange(dictionary.Values.ToList<JieriLianXuChargeActivity._ChargeLvl>());
					base.PredealDateTime();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", this.CfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public string QueryMyActInfo(GameClient client)
		{
			string result;
			if ((!this.InActivityTime() && !this.InAwardTime()) || client == null)
			{
				result = string.Format("{0}", 2);
			}
			else
			{
				List<JieriLianXuChargeActivity._AwardInfo> list = this._GetMyActInfoFromDB(client);
				if (list == null)
				{
					result = string.Format("{0}", 4);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(0);
					foreach (JieriLianXuChargeActivity._AwardInfo awardInfo in list)
					{
						stringBuilder.Append(":").Append(awardInfo.AwardId);
						stringBuilder.Append(",").Append(awardInfo.LianXuDay);
						stringBuilder.Append(",").Append(awardInfo.AwardFlag);
					}
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		public JieriLianXuChargeErrorCode HandleGetAward(GameClient client, int awardId, int day)
		{
			JieriLianXuChargeErrorCode result;
			if (!this.InAwardTime() || client == null)
			{
				result = JieriLianXuChargeErrorCode.NotAwardTime;
			}
			else
			{
				JieriLianXuChargeActivity._ChargeLvl chargeLvl = this.chargeLvlList.Find((JieriLianXuChargeActivity._ChargeLvl _cl) => _cl.Id == awardId);
				if (chargeLvl == null)
				{
					result = JieriLianXuChargeErrorCode.ConfigError;
				}
				else
				{
					JieriLianXuChargeActivity._DayAward dayAward = chargeLvl.AwardList.Find((JieriLianXuChargeActivity._DayAward _da) => _da.LianXuDay == day);
					if (dayAward == null)
					{
						result = JieriLianXuChargeErrorCode.ConfigError;
					}
					else
					{
						List<JieriLianXuChargeActivity._AwardInfo> list = this._GetMyActInfoFromDB(client);
						if (list == null)
						{
							result = JieriLianXuChargeErrorCode.DBFailed;
						}
						else
						{
							JieriLianXuChargeActivity._AwardInfo awardInfo = list.Find((JieriLianXuChargeActivity._AwardInfo _info) => _info.AwardId == awardId);
							if (awardInfo == null)
							{
								result = JieriLianXuChargeErrorCode.ConfigError;
							}
							else if (awardInfo.LianXuDay < day || Global.GetIntSomeBit(awardInfo.AwardFlag, day) == 1)
							{
								result = JieriLianXuChargeErrorCode.NotMeetAwardCond;
							}
							else
							{
								if (dayAward.AwardGoods != null && dayAward.AwardGoods.GoodsDataList != null && dayAward.AwardGoods.GoodsDataList.Count > 0)
								{
									int newGoodsCount = dayAward.AwardGoods.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
									if (!Global.CanAddGoodsNum(client, newGoodsCount))
									{
										return JieriLianXuChargeErrorCode.NoBagSpace;
									}
								}
								int awardFlag = Global.SetIntSomeBit(day, awardInfo.AwardFlag, true);
								if (!this._UpdateAwardFlag2DB(client, awardId, awardFlag))
								{
									result = JieriLianXuChargeErrorCode.DBFailed;
								}
								else
								{
									awardInfo.AwardFlag = awardFlag;
									base.GiveAward(client, dayAward.AwardGoods);
									if (client._IconStateMgr.CheckJieriLianXuCharge(client))
									{
										client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
										client._IconStateMgr.SendIconStateToClient(client);
									}
									result = JieriLianXuChargeErrorCode.Success;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool _UpdateAwardFlag2DB(GameClient client, int awardId, int awardFlag)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					awardId,
					this.FromDate.Replace(':', '$'),
					this.ToDate.Replace(':', '$'),
					awardFlag
				});
				string[] array = Global.ExecuteDBCmd(13215, strcmd, client.ServerId);
				result = (array != null && array.Length == 1 && Convert.ToInt32(array[0]) > 0);
			}
			return result;
		}

		private List<JieriLianXuChargeActivity._AwardInfo> _GetMyActInfoFromDB(GameClient client)
		{
			List<JieriLianXuChargeActivity._AwardInfo> result;
			if (client == null)
			{
				result = null;
			}
			else if (!this.InActivityTime() && !this.InAwardTime())
			{
				result = null;
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(client.ClientData.RoleID);
				stringBuilder.Append(':').Append(client.ClientData.ZoneID);
				stringBuilder.Append(':').Append(this.FromDate.Replace(':', '$'));
				stringBuilder.Append(':').Append(this.ToDate.Replace(':', '$'));
				stringBuilder.Append(':');
				foreach (JieriLianXuChargeActivity._ChargeLvl chargeLvl in this.chargeLvlList)
				{
					stringBuilder.Append(chargeLvl.Id).Append('_');
				}
				string[] array = Global.ExecuteDBCmd(13214, stringBuilder.ToString(), client.ServerId);
				if (array == null || array.Length != 2)
				{
					result = null;
				}
				else
				{
					int[] eachDayChargeArray = this._ParseEachDayCharge(array[0]);
					Dictionary<int, int> dictionary = this._ParseAwardFlagOfEachLvl(array[1]);
					List<JieriLianXuChargeActivity._AwardInfo> list = new List<JieriLianXuChargeActivity._AwardInfo>();
					foreach (JieriLianXuChargeActivity._ChargeLvl chargeLvl in this.chargeLvlList)
					{
						JieriLianXuChargeActivity._AwardInfo awardInfo = new JieriLianXuChargeActivity._AwardInfo();
						awardInfo.LianXuDay = this._CalcLianXuChargeDay(eachDayChargeArray, chargeLvl.NeedCharge);
						awardInfo.AwardId = chargeLvl.Id;
						awardInfo.AwardFlag = 0;
						if (dictionary.ContainsKey(chargeLvl.Id))
						{
							awardInfo.AwardFlag = dictionary[chargeLvl.Id];
						}
						list.Add(awardInfo);
					}
					result = list;
				}
			}
			return result;
		}

		private int _CalcLianXuChargeDay(int[] eachDayChargeArray, int atLeastCharge)
		{
			int result;
			if (eachDayChargeArray == null || atLeastCharge <= 0)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < eachDayChargeArray.Length; i++)
				{
					if (eachDayChargeArray[i] >= atLeastCharge)
					{
						num++;
					}
				}
				result = num;
			}
			return result;
		}

		private Dictionary<int, int> _ParseAwardFlagOfEachLvl(string strAwardIdAndFlag)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (!string.IsNullOrEmpty(strAwardIdAndFlag))
			{
				string[] array = strAwardIdAndFlag.Split(new char[]
				{
					'$'
				});
				foreach (string text in array)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						int key = Convert.ToInt32(array3[0]);
						int value = Convert.ToInt32(array3[1]);
						dictionary[key] = value;
					}
				}
			}
			return dictionary;
		}

		private int[] _ParseEachDayCharge(string strMoneyOfDays)
		{
			int[] result;
			if (string.IsNullOrEmpty(strMoneyOfDays))
			{
				result = null;
			}
			else
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				string[] array = strMoneyOfDays.Split(new char[]
				{
					'$'
				});
				foreach (string text in array)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						string text2 = array3[0];
						int num = Convert.ToInt32(array3[1]);
						if (dictionary.ContainsKey(text2))
						{
							Dictionary<string, int> dictionary2;
							string key;
							(dictionary2 = dictionary)[key = text2] = dictionary2[key] + num;
						}
						else
						{
							dictionary.Add(text2, num);
						}
					}
				}
				DateTime dateTime = DateTime.Parse(this.FromDate);
				DateTime dateTime2 = DateTime.Parse(this.ToDate);
				DateTime d = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				DateTime d2 = new DateTime(dateTime2.Year, dateTime2.Month, dateTime2.Day);
				int num2 = (int)(d2 - d).TotalDays + 1;
				if (num2 <= 0)
				{
					result = null;
				}
				else
				{
					int[] array4 = new int[num2];
					for (int j = 0; j < num2; j++)
					{
						string key2 = d.AddDays((double)j).ToString("yyyy-MM-dd");
						if (dictionary.ContainsKey(key2))
						{
							array4[j] = dictionary[key2];
						}
						else
						{
							array4[j] = 0;
						}
					}
					result = array4;
				}
			}
			return result;
		}

		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null || !this.InAwardTime())
			{
				result = false;
			}
			else
			{
				List<JieriLianXuChargeActivity._AwardInfo> list = this._GetMyActInfoFromDB(client);
				if (list == null)
				{
					result = false;
				}
				else
				{
					using (List<JieriLianXuChargeActivity._AwardInfo>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							JieriLianXuChargeActivity._AwardInfo info = enumerator.Current;
							JieriLianXuChargeActivity._ChargeLvl chargeLvl = this.chargeLvlList.Find((JieriLianXuChargeActivity._ChargeLvl _cl) => _cl.Id == info.AwardId);
							if (chargeLvl == null)
							{
								return false;
							}
							foreach (JieriLianXuChargeActivity._DayAward dayAward in chargeLvl.AwardList)
							{
								if (dayAward.LianXuDay <= info.LianXuDay && Global.GetIntSomeBit(info.AwardFlag, dayAward.LianXuDay) == 0)
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

		private readonly string CfgFile = "Config/JieRiGifts/JieRiLianXu.xml";

		private List<JieriLianXuChargeActivity._ChargeLvl> chargeLvlList = new List<JieriLianXuChargeActivity._ChargeLvl>();

		private class _AwardInfo
		{
			public int AwardId;

			public int LianXuDay;

			public int AwardFlag;
		}

		private class _DayAward
		{
			public int LianXuDay;

			public AwardItem AwardGoods = new AwardItem();
		}

		private class _ChargeLvl
		{
			public int Id;

			public int NeedCharge;

			public List<JieriLianXuChargeActivity._DayAward> AwardList = new List<JieriLianXuChargeActivity._DayAward>();
		}
	}
}
