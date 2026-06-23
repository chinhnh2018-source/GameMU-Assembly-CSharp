using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class WeedEndInputActivity : Activity
	{
		public override bool InActivityTime()
		{
			bool result;
			if (string.IsNullOrEmpty(this.FromDate) || string.IsNullOrEmpty(this.ToDate))
			{
				result = false;
			}
			else
			{
				int num = (int)TimeUtil.NowDateTime().DayOfWeek;
				string[] array = this.FromDate.Split(new char[]
				{
					','
				});
				string[] array2 = this.ToDate.Split(new char[]
				{
					','
				});
				if (0 == num)
				{
					num = 7;
				}
				int num2 = Convert.ToInt32(array[0]);
				int num3 = Convert.ToInt32(array2[0]);
				if (num < num2)
				{
					result = false;
				}
				else if (num > num3)
				{
					result = false;
				}
				else
				{
					string text = TimeUtil.NowDateTime().ToString("HH:mm:ss");
					if (num2 == num3)
					{
						if (text.CompareTo(array[1]) > 0 && text.CompareTo(array2[1]) < 0)
						{
							return true;
						}
					}
					else if (num == num2)
					{
						if (text.CompareTo(array[1]) > 0)
						{
							return true;
						}
					}
					else if (num == num3)
					{
						if (text.CompareTo(array2[1]) < 0)
						{
							return true;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public override bool InAwardTime()
		{
			return !string.IsNullOrEmpty(this.FromDate) && !string.IsNullOrEmpty(this.ToDate) && this.InActivityTime();
		}

		public override int GetParamsValidateCode()
		{
			return 1;
		}

		public void OnRoleLogin(GameClient client, bool isLogin)
		{
			if (this.InActivityTime())
			{
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				string text = "WeekEndInputFlag";
				string roleParamByName = Global.GetRoleParamByName(client, text);
				if (null != roleParamByName)
				{
					string[] array = roleParamByName.Split(new char[]
					{
						'#'
					});
					if (array.Length == 2)
					{
						int num = Convert.ToInt32(array[0]);
						if (offsetDay == num)
						{
							return;
						}
					}
				}
				string text2 = string.Format("{0}", offsetDay);
				text2 += '#';
				text2 += this.BuildRandAwardData(client);
				Global.SaveRoleParamsStringToDB(client, text, text2, true);
				if (!isLogin)
				{
				}
			}
		}

		public void SyncWeekEndInputData(GameClient client)
		{
			string cmdData = "";
			string name = "WeekEndInputFlag";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			if (string.IsNullOrEmpty(roleParamByName))
			{
				cmdData = string.Format("{0}:{1}", -1, 0);
			}
			else
			{
				string[] array = roleParamByName.Split(new char[]
				{
					'#'
				});
				if (array.Length == 2)
				{
					cmdData = string.Format("{0}:{1}", 0, array[1]);
				}
			}
			client.sendCmd(1501, cmdData, false);
		}

		public int GetWeekEndInputOpenDay(GameClient client)
		{
			int result = 0;
			string name = "WeekEndInputOD";
			string roleParamByName = Global.GetRoleParamByName(client, name);
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				result = Convert.ToInt32(roleParamByName);
			}
			return result;
		}

		public void UpdateWeekEndInputOpenDay(GameClient client)
		{
			if (this.InAwardTime())
			{
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				string roleParamsKey = "WeekEndInputOD";
				Global.SaveRoleParamsStringToDB(client, roleParamsKey, Convert.ToString(offsetDay), true);
			}
		}

		public override bool GiveAward(GameClient client, int NeedYuanBao)
		{
			bool result;
			if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				string name = "WeekEndInputFlag";
				string roleParamByName = Global.GetRoleParamByName(client, name);
				string[] array = roleParamByName.Split(new char[]
				{
					'#'
				});
				if (array.Length < 2)
				{
					result = false;
				}
				else
				{
					string[] array2 = array[1].Split(new char[]
					{
						'|'
					});
					if (array2.Length <= 0)
					{
						result = false;
					}
					else
					{
						foreach (string text in array2)
						{
							string[] array3 = text.Split(new char[]
							{
								'$'
							});
							int key = Convert.ToInt32(array3[1]);
							WeekEndInputTypeData weekEndInputTypeData = null;
							this.InputTypeDict.TryGetValue(key, out weekEndInputTypeData);
							if (weekEndInputTypeData != null && weekEndInputTypeData.MinZuanShi == NeedYuanBao)
							{
								List<WeekEndInputAwardData> list = null;
								this.AwardItemDict.TryGetValue(key, out list);
								if (null != list)
								{
									string[] array4 = array3[2].Split(new char[]
									{
										','
									});
									for (int j = 0; j < array4.Length; j++)
									{
										int num = Convert.ToInt32(array4[j]);
										if (num > 0 && num <= list.Count)
										{
											base.GiveAward(client, list[num - 1]);
										}
									}
								}
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public int GetNeedGoodsSpace(GameClient client, int NeedYuanBao)
		{
			int result;
			if (!this.InAwardTime())
			{
				result = 0;
			}
			else
			{
				string name = "WeekEndInputFlag";
				string roleParamByName = Global.GetRoleParamByName(client, name);
				string[] array = roleParamByName.Split(new char[]
				{
					'#'
				});
				if (array.Length < 2)
				{
					result = 0;
				}
				else
				{
					string[] array2 = array[1].Split(new char[]
					{
						'|'
					});
					if (array2.Length <= 0)
					{
						result = 0;
					}
					else
					{
						foreach (string text in array2)
						{
							string[] array3 = text.Split(new char[]
							{
								'$'
							});
							int key = Convert.ToInt32(array3[1]);
							WeekEndInputTypeData weekEndInputTypeData = null;
							this.InputTypeDict.TryGetValue(key, out weekEndInputTypeData);
							if (weekEndInputTypeData != null && weekEndInputTypeData.MinZuanShi == NeedYuanBao)
							{
								return weekEndInputTypeData.Num;
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public string BuildRandAwardData(GameClient client)
		{
			string text = "";
			string result;
			if (!this.InActivityTime())
			{
				result = text;
			}
			else
			{
				MeiRiChongZhiActivity meiRiChongZhiActivity = HuodongCachingMgr.GetMeiRiChongZhiActivity();
				if (null == meiRiChongZhiActivity)
				{
					result = text;
				}
				else
				{
					List<KeyValuePair<int, WeekEndInputTypeData>> list = new List<KeyValuePair<int, WeekEndInputTypeData>>();
					int changeLifeCount = client.ClientData.ChangeLifeCount;
					int level = client.ClientData.Level;
					foreach (KeyValuePair<int, WeekEndInputTypeData> keyValuePair in this.InputTypeDict)
					{
						if (keyValuePair.Value.MaxLevel >= level && keyValuePair.Value.MinLevel <= level && keyValuePair.Value.MaxZhuanSheng >= changeLifeCount && keyValuePair.Value.MinZhuanSheng <= changeLifeCount)
						{
							list.Add(new KeyValuePair<int, WeekEndInputTypeData>(keyValuePair.Key, keyValuePair.Value));
						}
					}
					foreach (KeyValuePair<int, WeekEndInputTypeData> keyValuePair in list)
					{
						List<WeekEndInputAwardData> list2 = null;
						this.AwardItemDict.TryGetValue(keyValuePair.Key, out list2);
						if (null != list2)
						{
							int idbyYuanBao = meiRiChongZhiActivity.GetIDByYuanBao(keyValuePair.Value.MinZuanShi);
							text += idbyYuanBao;
							text += "$";
							text += keyValuePair.Key;
							text += "$";
							int randBeginNum = list2[0].RandBeginNum;
							int num = list2[list2.Count - 1].RandEndNum;
							lock (this.AwardItemDict)
							{
								for (int i = 0; i < keyValuePair.Value.Num; i++)
								{
									int num2 = Global.GetRandomNumber(randBeginNum, num);
									for (int j = 0; j < list2.Count; j++)
									{
										if (list2[j].RandSkip)
										{
											num2 += list2[j].RandNumMinus;
										}
										if (!list2[j].RandSkip && num2 >= list2[j].RandBeginNum && num2 <= list2[j].RandEndNum)
										{
											list2[j].RandSkip = true;
											num -= list2[j].RandNumMinus;
											text += list2[j].id;
											if (i != keyValuePair.Value.Num - 1)
											{
												text += ",";
											}
											break;
										}
									}
								}
								text += "|";
								for (int j = 0; j < list2.Count; j++)
								{
									if (list2[j].RandSkip)
									{
									}
									list2[j].RandSkip = false;
								}
							}
						}
					}
					if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
					{
						text = text.Substring(0, text.Length - 1);
					}
					result = text;
				}
			}
			return result;
		}

		public bool ParseActivityTime(string ZhouMoChongZhiTime)
		{
			string[] array = ZhouMoChongZhiTime.Split(new char[]
			{
				'|'
			});
			bool result;
			if (array == null || array.Length != 2)
			{
				result = false;
			}
			else
			{
				string[] array2 = array[0].Split(new char[]
				{
					','
				});
				string[] array3 = array[1].Split(new char[]
				{
					','
				});
				if (array2 == null || array3 == null || array2.Length != 2 || array3.Length != 2)
				{
					result = false;
				}
				else
				{
					this.FromDate = array2[0] + ',' + array2[1];
					this.ToDate = array3[0] + ',' + array3[1];
					this.FromDate.Trim();
					this.ToDate.Trim();
					result = true;
				}
			}
			return result;
		}

		public bool Init()
		{
			try
			{
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhouMoChongZhiTime");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					if (!this.ParseActivityTime(paramValueByName))
					{
						return false;
					}
				}
				string xmlFileName = Global.IsolateResPath("Config/Gifts/ZhouMoChongZhiType.xml");
				XElement xelement = GeneralCachingXmlMgr.GetXElement(xmlFileName);
				if (null == xelement)
				{
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						WeekEndInputTypeData weekEndInputTypeData = new WeekEndInputTypeData();
						int key = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						weekEndInputTypeData.MinZhuanSheng = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "MinZhuanSheng"));
						weekEndInputTypeData.MinLevel = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "MinLevel"));
						weekEndInputTypeData.MaxZhuanSheng = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "MaxZhuanSheng"));
						weekEndInputTypeData.MaxLevel = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "MaxLevel"));
						weekEndInputTypeData.MinZuanShi = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "MinZuanShi"));
						weekEndInputTypeData.Num = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement2, "Num"));
						this.InputTypeDict[key] = weekEndInputTypeData;
					}
				}
				xmlFileName = Global.IsolateResPath("Config/Gifts/ZhouMoChongZhi.xml");
				XElement xelement3 = GeneralCachingXmlMgr.GetXElement(xmlFileName);
				if (null == xelement3)
				{
					return false;
				}
				IEnumerable<XElement> enumerable2 = xelement3.Elements();
				foreach (XElement xelement4 in enumerable2)
				{
					if (null != xelement4)
					{
						List<WeekEndInputAwardData> list = new List<WeekEndInputAwardData>();
						int key = (int)Global.GetSafeAttributeLong(xelement4, "ID");
						IEnumerable<XElement> enumerable3 = xelement4.Elements();
						foreach (XElement xml in enumerable3)
						{
							WeekEndInputAwardData weekEndInputAwardData = new WeekEndInputAwardData();
							weekEndInputAwardData.id = (int)Global.GetSafeAttributeLong(xml, "ID");
							weekEndInputAwardData.RandBeginNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xml, "BeginNum"));
							weekEndInputAwardData.RandEndNum = Global.GMax(0, (int)Global.GetSafeAttributeLong(xml, "EndNum"));
							weekEndInputAwardData.RandNumMinus = weekEndInputAwardData.RandEndNum - weekEndInputAwardData.RandBeginNum + 1;
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Goods");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length <= 0)
							{
								LogManager.WriteLog(1, string.Format("解析大型周末充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								weekEndInputAwardData.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "大型节日周末充值配置1");
							}
							list.Add(weekEndInputAwardData);
						}
						this.AwardItemDict[key] = list;
					}
				}
				this.ActivityType = 27;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "ZhouMoChongZhiType.xml|ZhouMoChongZhi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		protected Dictionary<int, WeekEndInputTypeData> InputTypeDict = new Dictionary<int, WeekEndInputTypeData>();

		protected Dictionary<int, List<WeekEndInputAwardData>> AwardItemDict = new Dictionary<int, List<WeekEndInputAwardData>>();
	}
}
