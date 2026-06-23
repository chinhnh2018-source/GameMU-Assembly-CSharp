using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class GoodsPackManager
	{
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		public int GetNextGoodsID()
		{
			return (int)(Interlocked.Increment(ref this.BaseGoodsID) & 2147483647L);
		}

		public int GetNextRoleGoodsPackID()
		{
			return (int)(Interlocked.Increment(ref this.BaseRoleGoodsPackID) & 2147483647L);
		}

		private void InitGlobalFallGoodsLimitDict()
		{
			lock (this._GlobalFallGoodsLimitDict)
			{
				if (this._GlobalFallGoodsLimitDict.Count <= 0)
				{
					XElement xelement = ConfigHelper.Load(Global.GameResPath("Config/EraDropLimit.xml"));
					if (null != xelement)
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							int key = (int)Global.GetSafeAttributeLong(xml, "DropID");
							int value = (int)Global.GetSafeAttributeLong(xml, "DropLimit");
							this._GlobalFallGoodsLimitDict[key] = value;
						}
					}
				}
			}
		}

		private int GetGlobalFallGoodsLimitNum(int goodsPackID)
		{
			int result = 0;
			lock (this._GlobalFallGoodsLimitDict)
			{
				this.InitGlobalFallGoodsLimitDict();
				this._GlobalFallGoodsLimitDict.TryGetValue(goodsPackID, out result);
			}
			return result;
		}

		private bool JudgeModifyGlobalFallGoodsLimit(int goodsPackID)
		{
			int globalFallGoodsLimitNum = this.GetGlobalFallGoodsLimitNum(goodsPackID);
			bool result;
			if (globalFallGoodsLimitNum <= 0)
			{
				result = true;
			}
			else
			{
				lock (this.GlobalFallGoodsNumDict)
				{
					int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
					if (this.GlobalFallGoodsLimitDayID != offsetDay)
					{
						this.GlobalFallGoodsNumDict.Clear();
						this.GlobalFallGoodsLimitDayID = offsetDay;
					}
					int num = 0;
					this.GlobalFallGoodsNumDict.TryGetValue(goodsPackID, out num);
					if (num >= globalFallGoodsLimitNum)
					{
						return false;
					}
					num = (this.GlobalFallGoodsNumDict[goodsPackID] = num + 1);
				}
				result = true;
			}
			return result;
		}

		private List<FallGoodsItem> GetNormalFallGoodsItem(int goodsPackID)
		{
			List<FallGoodsItem> list = null;
			lock (this._FallGoodsItemsDict)
			{
				this._FallGoodsItemsDict.TryGetValue(goodsPackID, out list);
			}
			List<FallGoodsItem> result;
			if (null != list)
			{
				result = list;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					FallGoodsItem fallGoodsItem = null;
					list = new List<FallGoodsItem>();
					string stringValue = systemXmlItem.GetStringValue("GoodsID");
					string[] array = stringValue.Split(new char[]
					{
						'|'
					});
					int num = 0;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i].Trim();
						if (!(text == ""))
						{
							string[] array2 = text.Split(new char[]
							{
								','
							});
							if (array2.Length == 7)
							{
								fallGoodsItem = null;
								try
								{
									fallGoodsItem = new FallGoodsItem
									{
										GoodsID = Convert.ToInt32(array2[0]),
										BasePercent = num,
										SelfPercent = (int)(Convert.ToDouble(array2[1]) * 100000.0),
										Binding = Convert.ToInt32(array2[2]),
										LuckyRate = (int)Convert.ToDouble(array2[3]),
										FallLevelID = Convert.ToInt32(array2[4]),
										ZhuiJiaID = Convert.ToInt32(array2[5]),
										ExcellencePropertyID = Convert.ToInt32(array2[6])
									};
									num += fallGoodsItem.SelfPercent;
								}
								catch (Exception)
								{
									fallGoodsItem = null;
								}
								if (null == fallGoodsItem)
								{
									LogManager.WriteLog(2, string.Format("解析掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, text), null, true);
								}
								else
								{
									list.Add(fallGoodsItem);
								}
							}
						}
					}
					if (num > 100000)
					{
						LogManager.WriteLog(0, string.Format("解析掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
					}
					lock (this._FallGoodsItemsDict)
					{
						this._FallGoodsItemsDict[goodsPackID] = list;
					}
					result = list;
				}
			}
			return result;
		}

		private List<FallGoodsItem> ParseGoodsDataList(int goodsPackID, string goodsData)
		{
			List<FallGoodsItem> list = new List<FallGoodsItem>();
			string[] array = goodsData.Split(new char[]
			{
				'|'
			});
			FallGoodsItem fallGoodsItem = null;
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (!(text == ""))
				{
					string[] array2 = text.Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						fallGoodsItem = null;
						try
						{
							fallGoodsItem = new FallGoodsItem
							{
								GoodsID = Convert.ToInt32(array2[0]),
								BasePercent = num,
								SelfPercent = (int)(Convert.ToDouble(array2[1]) * 100000.0),
								Binding = Convert.ToInt32(array2[2]),
								LuckyRate = (int)Convert.ToDouble(array2[3]),
								FallLevelID = Convert.ToInt32(array2[4]),
								ZhuiJiaID = Convert.ToInt32(array2[5]),
								ExcellencePropertyID = Convert.ToInt32(array2[6])
							};
							num += fallGoodsItem.SelfPercent;
						}
						catch (Exception)
						{
							fallGoodsItem = null;
						}
						if (null == fallGoodsItem)
						{
							LogManager.WriteLog(2, string.Format("解析掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, text), null, true);
						}
						else
						{
							list.Add(fallGoodsItem);
						}
					}
				}
			}
			if (num > 100000)
			{
				LogManager.WriteLog(0, string.Format("解析掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
			}
			return list;
		}

		public List<GoodsData> GetFixedGoodsDataList(List<FallGoodsItem> fixedFallGoodsItemList, int count)
		{
			List<GoodsData> result;
			if (null == fixedFallGoodsItemList)
			{
				result = null;
			}
			else if (count <= 0)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> list = new List<FallGoodsItem>();
				list.AddRange(fixedFallGoodsItemList);
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < count; i++)
				{
					if (fixedFallGoodsItemList.Count <= 0)
					{
						break;
					}
					int num = 0;
					foreach (FallGoodsItem fallGoodsItem in list)
					{
						num += fallGoodsItem.SelfPercent;
					}
					int num2 = Global.GetRandomNumber(0, num);
					foreach (FallGoodsItem fallGoodsItem in list)
					{
						if (num2 < fallGoodsItem.SelfPercent)
						{
							int quality = 0;
							int fallGoodsLevel = this.GetFallGoodsLevel(fallGoodsItem.FallLevelID);
							int bornIndex = 0;
							int lucky = 0;
							int luckyGoodsID = this.GetLuckyGoodsID(fallGoodsItem.LuckyRate);
							if (luckyGoodsID > 0)
							{
								int luckyGoodsID2 = GameManager.GoodsPackMgr.GetLuckyGoodsID(luckyGoodsID);
								if (luckyGoodsID2 >= 1)
								{
									lucky = 1;
								}
							}
							int zhuiJiaGoodsLevelID = this.GetZhuiJiaGoodsLevelID(fallGoodsItem.ZhuiJiaID);
							int excellencePropertysID = this.GetExcellencePropertysID(fallGoodsItem.GoodsID, fallGoodsItem.ExcellencePropertyID);
							string props = "";
							GoodsData item = new GoodsData
							{
								Id = this.GetNextGoodsID(),
								GoodsID = fallGoodsItem.GoodsID,
								Forge_level = fallGoodsLevel,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Quality = quality,
								Props = props,
								GCount = 1,
								Binding = fallGoodsItem.Binding,
								BornIndex = bornIndex,
								Lucky = lucky,
								AppendPropLev = zhuiJiaGoodsLevelID,
								ExcellenceInfo = excellencePropertysID
							};
							list.Remove(fallGoodsItem);
							list2.Add(item);
							break;
						}
						num2 -= fallGoodsItem.SelfPercent;
					}
				}
				result = list2;
			}
			return result;
		}

		public void ResetLimitTimeRange()
		{
			this._LimitTimeStartDayTime = new DateTime(2000, 1, 1);
			this._LimitTimeEndDayTime = new DateTime(2000, 1, 1);
		}

		private bool JugeInLimitTimeRange()
		{
			if (2000 == this._LimitTimeStartDayTime.Year)
			{
				this._LimitTimeStartDayTime = Global.GetJieriStartDay();
			}
			if (2000 == this._LimitTimeEndDayTime.Year)
			{
				this._LimitTimeEndDayTime = Global.GetAddDaysDataTime(Global.GetJieriStartDay(), Math.Max(0, Global.GetJieriDaysNum()), true);
			}
			DateTime dateTime = TimeUtil.NowDateTime();
			return dateTime.Ticks >= this._LimitTimeStartDayTime.Ticks && dateTime.Ticks < this._LimitTimeEndDayTime.Ticks;
		}

		private List<FallGoodsItem> GetLimitTimeFallGoodsItem(int goodsPackID)
		{
			List<FallGoodsItem> result;
			if (!this.JugeInLimitTimeRange())
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> list = null;
				lock (this._LimitTimeFallGoodsItemsDict)
				{
					this._LimitTimeFallGoodsItemsDict.TryGetValue(goodsPackID, out list);
				}
				if (null != list)
				{
					result = list;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemLimitTimeMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
					{
						result = null;
					}
					else
					{
						FallGoodsItem fallGoodsItem = null;
						list = new List<FallGoodsItem>();
						string stringValue = systemXmlItem.GetStringValue("GoodsID");
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						int num = 0;
						for (int i = 0; i < array.Length; i++)
						{
							string text = array[i].Trim();
							if (!(text == ""))
							{
								string[] array2 = text.Split(new char[]
								{
									','
								});
								if (array2.Length == 7)
								{
									fallGoodsItem = null;
									try
									{
										fallGoodsItem = new FallGoodsItem
										{
											GoodsID = Convert.ToInt32(array2[0]),
											BasePercent = num,
											SelfPercent = (int)(Convert.ToDouble(array2[1]) * 100000.0),
											Binding = Convert.ToInt32(array2[2]),
											LuckyRate = (int)Convert.ToDouble(array2[3]),
											FallLevelID = Convert.ToInt32(array2[4]),
											ZhuiJiaID = Convert.ToInt32(array2[5]),
											ExcellencePropertyID = Convert.ToInt32(array2[6])
										};
										num += fallGoodsItem.SelfPercent;
									}
									catch (Exception)
									{
										fallGoodsItem = null;
									}
									if (null == fallGoodsItem)
									{
										LogManager.WriteLog(2, string.Format("解析节日掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", goodsPackID, text), null, true);
									}
									else
									{
										list.Add(fallGoodsItem);
									}
								}
							}
						}
						if (num > 100000)
						{
							LogManager.WriteLog(2, string.Format("解析节日掉落项时发生概率溢出100000错误, GoodsPackID={0}", goodsPackID), null, true);
						}
						lock (this._LimitTimeFallGoodsItemsDict)
						{
							this._LimitTimeFallGoodsItemsDict[goodsPackID] = list;
						}
						result = list;
					}
				}
			}
			return result;
		}

		public List<FallGoodsItem> GetFixedFallGoodsItemList(int goodsPackID)
		{
			List<FallGoodsItem> list = null;
			lock (this._FixedGoodsItemsDict)
			{
				this._FixedGoodsItemsDict.TryGetValue(goodsPackID, out list);
			}
			List<FallGoodsItem> result;
			if (null != list)
			{
				result = list;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					string stringValue = systemXmlItem.GetStringValue("Fixedaward");
					list = this.ParseGoodsDataList(goodsPackID, stringValue);
					lock (this._FixedGoodsItemsDict)
					{
						this._FixedGoodsItemsDict[goodsPackID] = list;
					}
					result = list;
				}
			}
			return result;
		}

		public int GetFallGoodsMaxCount(int goodsPackID)
		{
			Tuple<int, int> tuple = null;
			lock (this._FallGoodsMaxCountDict)
			{
				if (this._FallGoodsMaxCountDict.TryGetValue(goodsPackID, out tuple))
				{
					return tuple.Item2;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
			{
				result = -1;
			}
			else
			{
				string stringValue = systemXmlItem.GetStringValue("MaxList");
				string[] array = stringValue.Split(new char[]
				{
					','
				});
				if (array.Length != 2)
				{
					tuple = new Tuple<int, int>(0, 0);
				}
				else
				{
					tuple = new Tuple<int, int>(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]));
				}
				lock (this._FallGoodsMaxCountDict)
				{
					this._FallGoodsMaxCountDict[goodsPackID] = tuple;
				}
				result = tuple.Item2;
			}
			return result;
		}

		public int GetFixedFallGoodsMaxCount(int goodsPackID)
		{
			Tuple<int, int> tuple = null;
			lock (this._FallGoodsMaxCountDict)
			{
				if (this._FallGoodsMaxCountDict.TryGetValue(goodsPackID, out tuple))
				{
					return tuple.Item1;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
			{
				result = -1;
			}
			else
			{
				string stringValue = systemXmlItem.GetStringValue("MaxList");
				string[] array = stringValue.Split(new char[]
				{
					','
				});
				tuple = new Tuple<int, int>(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]));
				lock (this._FallGoodsMaxCountDict)
				{
					this._FallGoodsMaxCountDict[goodsPackID] = tuple;
				}
				result = tuple.Item1;
			}
			return result;
		}

		private int GetLimitTimeFallGoodsMaxCount(int goodsPackID)
		{
			int num = -1;
			lock (this._LimitTimeFallGoodsMaxCountDict)
			{
				if (this._LimitTimeFallGoodsMaxCountDict.TryGetValue(goodsPackID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemLimitTimeMonsterGoodsList.SystemXmlItemDict.TryGetValue(goodsPackID, out systemXmlItem))
			{
				result = -1;
			}
			else
			{
				num = systemXmlItem.GetIntValue("MaxList", -1);
				lock (this._LimitTimeFallGoodsMaxCountDict)
				{
					this._LimitTimeFallGoodsMaxCountDict[goodsPackID] = num;
				}
				result = num;
			}
			return result;
		}

		private FallQualityItem GetFallQualityItem(int fallQualityID)
		{
			FallQualityItem fallQualityItem = null;
			lock (this._FallGoodsQualityDict)
			{
				this._FallGoodsQualityDict.TryGetValue(fallQualityID, out fallQualityItem);
			}
			FallQualityItem result;
			if (null != fallQualityItem)
			{
				result = fallQualityItem;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoodsQuality.SystemXmlItemDict.TryGetValue(fallQualityID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					fallQualityItem = new FallQualityItem
					{
						ID = fallQualityID,
						QualityBasePercent = new double[5],
						QualitySelfPercent = new double[5]
					};
					string stringValue = systemXmlItem.GetStringValue("Quality");
					if (!string.IsNullOrEmpty(stringValue))
					{
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (array.Length == 5)
						{
							fallQualityItem.QualitySelfPercent = Global.StringArray2DoubleArray(array);
							double num = 0.0;
							for (int i = 0; i < fallQualityItem.QualitySelfPercent.Length; i++)
							{
								fallQualityItem.QualityBasePercent[i] = num;
								num += fallQualityItem.QualitySelfPercent[i];
							}
							if (num > 1.0)
							{
								LogManager.WriteLog(2, string.Format("解析掉落项的品质掉落概率溢出1.0错误, fallQualityID={0}", fallQualityID), null, true);
							}
						}
					}
					lock (this._FallGoodsQualityDict)
					{
						this._FallGoodsQualityDict[fallQualityID] = fallQualityItem;
					}
					result = fallQualityItem;
				}
			}
			return result;
		}

		private FallLevelItem GetFallLevelItem(int fallLevelID)
		{
			FallLevelItem fallLevelItem = null;
			lock (this._FallGoodsLevelDict)
			{
				this._FallGoodsLevelDict.TryGetValue(fallLevelID, out fallLevelItem);
			}
			FallLevelItem result;
			if (null != fallLevelItem)
			{
				result = fallLevelItem;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoodsLevel.SystemXmlItemDict.TryGetValue(fallLevelID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					fallLevelItem = new FallLevelItem
					{
						ID = fallLevelID,
						LevelBasePercent = new double[21],
						LevelSelfPercent = new double[21]
					};
					string stringValue = systemXmlItem.GetStringValue("Level");
					if (!string.IsNullOrEmpty(stringValue))
					{
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (array.Length == 21)
						{
							fallLevelItem.LevelSelfPercent = Global.StringArray2DoubleArray(array);
							double num = 0.0;
							for (int i = 0; i < fallLevelItem.LevelSelfPercent.Length; i++)
							{
								fallLevelItem.LevelBasePercent[i] = num;
								num += fallLevelItem.LevelSelfPercent[i];
							}
							if (num > 1.0)
							{
								LogManager.WriteLog(2, string.Format("解析掉落项的级别掉落概率溢出1.0错误, fallLevelID={0}", fallLevelID), null, true);
							}
						}
					}
					lock (this._FallGoodsLevelDict)
					{
						this._FallGoodsLevelDict[fallLevelID] = fallLevelItem;
					}
					result = fallLevelItem;
				}
			}
			return result;
		}

		private FallBornIndexItem GetFallBornIndexItem(int fallBornIndexID)
		{
			FallBornIndexItem fallBornIndexItem = null;
			lock (this._FallGoodsBornIndexDict)
			{
				this._FallGoodsBornIndexDict.TryGetValue(fallBornIndexID, out fallBornIndexItem);
			}
			FallBornIndexItem result;
			if (null != fallBornIndexItem)
			{
				result = fallBornIndexItem;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoodsBornIndex.SystemXmlItemDict.TryGetValue(fallBornIndexID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					fallBornIndexItem = new FallBornIndexItem
					{
						ID = fallBornIndexID,
						LevelBasePercent = new double[12],
						LevelSelfPercent = new double[12]
					};
					string stringValue = systemXmlItem.GetStringValue("Born");
					if (!string.IsNullOrEmpty(stringValue))
					{
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (array.Length == 12)
						{
							fallBornIndexItem.LevelSelfPercent = Global.StringArray2DoubleArray(array);
							double num = 0.0;
							for (int i = 0; i < fallBornIndexItem.LevelSelfPercent.Length; i++)
							{
								fallBornIndexItem.LevelBasePercent[i] = num;
								num += fallBornIndexItem.LevelSelfPercent[i];
							}
							if (num > 1.0)
							{
								LogManager.WriteLog(2, string.Format("解析掉落项的天生掉落概率溢出1.0错误, fallBornIndexID={0}", fallBornIndexID), null, true);
							}
						}
					}
					lock (this._FallGoodsBornIndexDict)
					{
						this._FallGoodsBornIndexDict[fallBornIndexID] = fallBornIndexItem;
					}
					result = fallBornIndexItem;
				}
			}
			return result;
		}

		private ZhuiJiaIDItem GetZhuiJiaIDItem(int zhuiJiaID)
		{
			ZhuiJiaIDItem zhuiJiaIDItem = null;
			lock (this._ZhuiJiaIDDict)
			{
				this._ZhuiJiaIDDict.TryGetValue(zhuiJiaID, out zhuiJiaIDItem);
			}
			ZhuiJiaIDItem result;
			if (null != zhuiJiaIDItem)
			{
				result = zhuiJiaIDItem;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoodsZhuiJia.SystemXmlItemDict.TryGetValue(zhuiJiaID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					zhuiJiaIDItem = new ZhuiJiaIDItem
					{
						ID = zhuiJiaID,
						LevelBasePercent = new double[11],
						LevelSelfPercent = new double[11]
					};
					string stringValue = systemXmlItem.GetStringValue("ZhuiJiaLevel");
					if (!string.IsNullOrEmpty(stringValue))
					{
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (array.Length == 21)
						{
							zhuiJiaIDItem.LevelSelfPercent = Global.StringArray2DoubleArray(array);
							double num = 0.0;
							for (int i = 0; i < zhuiJiaIDItem.LevelSelfPercent.Length; i++)
							{
								zhuiJiaIDItem.LevelBasePercent[i] = num;
								num += zhuiJiaIDItem.LevelSelfPercent[i];
							}
							if (num > 1.0)
							{
								LogManager.WriteLog(2, string.Format("解析掉落项的级别追加概率溢出1.0错误, zhuiJiaID={0}", zhuiJiaID), null, true);
							}
						}
					}
					lock (this._ZhuiJiaIDDict)
					{
						this._ZhuiJiaIDDict[zhuiJiaID] = zhuiJiaIDItem;
					}
					result = zhuiJiaIDItem;
				}
			}
			return result;
		}

		public ExcellencePropertyGroupItem GetExcellencePropertyGroupItem(int excellencePropertyGroupID)
		{
			ExcellencePropertyGroupItem excellencePropertyGroupItem = null;
			lock (this._ExcellencePropertyGroupItemDict)
			{
				this._ExcellencePropertyGroupItemDict.TryGetValue(excellencePropertyGroupID, out excellencePropertyGroupItem);
			}
			ExcellencePropertyGroupItem result;
			if (null != excellencePropertyGroupItem)
			{
				result = excellencePropertyGroupItem;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoodsExcellenceProperty.SystemXmlItemDict.TryGetValue(excellencePropertyGroupID, out systemXmlItem))
				{
					result = null;
				}
				else
				{
					excellencePropertyGroupItem = new ExcellencePropertyGroupItem
					{
						ID = excellencePropertyGroupID,
						Max = systemXmlItem.GetIntArrayValue("MAX", ','),
						ExcellencePropertyItems = this.ParseExcellencePropertyItems(systemXmlItem)
					};
					lock (this._ExcellencePropertyGroupItemDict)
					{
						this._ExcellencePropertyGroupItemDict[excellencePropertyGroupID] = excellencePropertyGroupItem;
					}
					result = excellencePropertyGroupItem;
				}
			}
			return result;
		}

		private ExcellencePropertyItem[] ParseExcellencePropertyItems(SystemXmlItem goodsExcellencePropertyItem)
		{
			ExcellencePropertyItem[] array = null;
			int num = 0;
			string stringValue = goodsExcellencePropertyItem.GetStringValue("ExcellenceProperty");
			if (!string.IsNullOrEmpty(stringValue))
			{
				string[] array2 = stringValue.Split(new char[]
				{
					'|'
				});
				if (array2 != null && array2.Length > 0)
				{
					array = new ExcellencePropertyItem[array2.Length];
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split(new char[]
						{
							','
						});
						if (2 == array3.Length)
						{
							array[i] = new ExcellencePropertyItem
							{
								Num = Global.SafeConvertToInt32(array3[0]),
								BasePercent = num,
								SelfPercent = (int)(Global.SafeConvertToDouble(array3[1]) * 100000.0)
							};
							num += array[i].SelfPercent;
						}
					}
				}
			}
			return array;
		}

		public int ResetCachingItems()
		{
			int num = GameManager.SystemMonsterGoodsList.ReloadLoadFromXMlFile();
			lock (this._FallGoodsItemsDict)
			{
				this._FallGoodsItemsDict.Clear();
			}
			num = GameManager.SystemLimitTimeMonsterGoodsList.ReloadLoadFromXMlFile();
			lock (this._LimitTimeFallGoodsItemsDict)
			{
				this._LimitTimeFallGoodsItemsDict.Clear();
			}
			lock (this._FixedGoodsItemsDict)
			{
				this._FixedGoodsItemsDict.Clear();
			}
			lock (this._FallGoodsMaxCountDict)
			{
				this._FallGoodsMaxCountDict.Clear();
			}
			lock (this._LimitTimeFallGoodsMaxCountDict)
			{
				this._LimitTimeFallGoodsMaxCountDict.Clear();
			}
			int result;
			if (num < 0)
			{
				result = num;
			}
			else
			{
				num = GameManager.SystemGoodsQuality.ReloadLoadFromXMlFile();
				lock (this._FallGoodsQualityDict)
				{
					this._FallGoodsQualityDict.Clear();
				}
				if (num < 0)
				{
					result = num;
				}
				else
				{
					num = GameManager.SystemGoodsLevel.ReloadLoadFromXMlFile();
					lock (this._FallGoodsLevelDict)
					{
						this._FallGoodsLevelDict.Clear();
					}
					if (num < 0)
					{
						result = num;
					}
					else
					{
						num = GameManager.SystemGoodsBornIndex.ReloadLoadFromXMlFile();
						lock (this._FallGoodsBornIndexDict)
						{
							this._FallGoodsBornIndexDict.Clear();
						}
						if (num < 0)
						{
							result = num;
						}
						else
						{
							num = GameManager.SystemGoodsZhuiJia.ReloadLoadFromXMlFile();
							lock (this._ZhuiJiaIDDict)
							{
								this._ZhuiJiaIDDict.Clear();
							}
							if (num < 0)
							{
								result = num;
							}
							else
							{
								num = GameManager.SystemGoodsExcellenceProperty.ReloadLoadFromXMlFile();
								lock (this._ExcellencePropertyGroupItemDict)
								{
									this._ExcellencePropertyGroupItemDict.Clear();
								}
								if (num < 0)
								{
									result = num;
								}
								else
								{
									lock (this._CacheShiQuGoodsDict)
									{
										this._CacheShiQuGoodsDict.Clear();
									}
									lock (this._GlobalFallGoodsLimitDict)
									{
										this._GlobalFallGoodsLimitDict.Clear();
									}
									result = num;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public Dictionary<int, GoodsPackItem> GoodsPackDict
		{
			get
			{
				return this._GoodsPackDict;
			}
			set
			{
				this._GoodsPackDict = value;
			}
		}

		private int GetFallGoodsQuality(int fallQualityID)
		{
			int num = 0;
			int result;
			if (-1 == fallQualityID)
			{
				result = num;
			}
			else
			{
				FallQualityItem fallQualityItem = this.GetFallQualityItem(fallQualityID);
				if (null == fallQualityItem)
				{
					result = num;
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < fallQualityItem.QualitySelfPercent.Length; i++)
					{
						int num2 = (int)(fallQualityItem.QualityBasePercent[i] * 100000.0);
						int num3 = (int)(fallQualityItem.QualitySelfPercent[i] * 100000.0);
						if (randomNumber > num2 && randomNumber <= num2 + num3)
						{
							num = i;
							break;
						}
					}
					result = num;
				}
			}
			return result;
		}

		public int GetFallGoodsLevel(int fallLevelID)
		{
			int num = 0;
			int result;
			if (-1 == fallLevelID)
			{
				result = num;
			}
			else
			{
				FallLevelItem fallLevelItem = this.GetFallLevelItem(fallLevelID);
				if (null == fallLevelItem)
				{
					result = num;
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < fallLevelItem.LevelSelfPercent.Length; i++)
					{
						int num2 = (int)(fallLevelItem.LevelBasePercent[i] * 100000.0);
						int num3 = (int)(fallLevelItem.LevelSelfPercent[i] * 100000.0);
						if (randomNumber > num2 && randomNumber <= num2 + num3)
						{
							num = i;
							break;
						}
					}
					result = num;
				}
			}
			return result;
		}

		private int GetFallGoodsBornIndex(IObject attacker, int fallBornIndexID, int goodsID)
		{
			int num = 0;
			int result;
			if (!(attacker is GameClient))
			{
				result = num;
			}
			else if (!DBRoleBufferManager.ProcessFallTianSheng(attacker as GameClient))
			{
				result = num;
			}
			else
			{
				num = Global.GetBornIndexOnFallGoods(goodsID);
				result = num;
			}
			return result;
		}

		public int GetLuckyGoodsID(int luckyPercent)
		{
			int randomNumber = Global.GetRandomNumber(1, 100001);
			int result;
			if (randomNumber <= luckyPercent * 100000)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int GetZhuiJiaGoodsLevelID(int zhuiJiaID)
		{
			int num = 0;
			int result;
			if (-1 == zhuiJiaID)
			{
				result = num;
			}
			else
			{
				ZhuiJiaIDItem zhuiJiaIDItem = this.GetZhuiJiaIDItem(zhuiJiaID);
				if (null == zhuiJiaIDItem)
				{
					result = num;
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(1, 100001);
					for (int i = 0; i < zhuiJiaIDItem.LevelSelfPercent.Length; i++)
					{
						int num2 = (int)(zhuiJiaIDItem.LevelBasePercent[i] * 100000.0);
						int num3 = (int)(zhuiJiaIDItem.LevelSelfPercent[i] * 100000.0);
						if (randomNumber > num2 && randomNumber <= num2 + num3)
						{
							num = i;
							break;
						}
					}
					result = num;
				}
			}
			return result;
		}

		public int GetExcellencePropertysID(int GoodsID, int excellencePropertyGroupID)
		{
			int result;
			if (ZuoQiManager.CheckIsZuoQiByGoodsID(GoodsID))
			{
				result = excellencePropertyGroupID;
			}
			else if (RebornEquip.IsRebornEquip(GoodsID))
			{
				result = excellencePropertyGroupID;
			}
			else
			{
				int num = 0;
				if (-1 == excellencePropertyGroupID)
				{
					result = num;
				}
				else
				{
					ExcellencePropertyGroupItem excellencePropertyGroupItem = this.GetExcellencePropertyGroupItem(excellencePropertyGroupID);
					if (excellencePropertyGroupItem == null || excellencePropertyGroupItem.ExcellencePropertyItems == null || excellencePropertyGroupItem.Max == null || excellencePropertyGroupItem.Max.Length <= 0)
					{
						result = num;
					}
					else
					{
						List<int> list = new List<int>();
						int num2 = 0;
						int randomNumber = Global.GetRandomNumber(1, 100001);
						int i;
						for (i = 0; i < excellencePropertyGroupItem.ExcellencePropertyItems.Length; i++)
						{
							if (randomNumber > excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent && randomNumber <= excellencePropertyGroupItem.ExcellencePropertyItems[i].BasePercent + excellencePropertyGroupItem.ExcellencePropertyItems[i].SelfPercent)
							{
								num2 = excellencePropertyGroupItem.ExcellencePropertyItems[i].Num;
								break;
							}
						}
						if (num2 > 0 && num2 <= excellencePropertyGroupItem.Max.Length)
						{
							int num3 = 0;
							do
							{
								int randomNumber2 = Global.GetRandomNumber(0, excellencePropertyGroupItem.Max.Length);
								if (list.IndexOf(randomNumber2) < 0)
								{
									list.Add(randomNumber2);
									num3++;
								}
							}
							while (num3 != num2);
						}
						i = 0;
						while (i < list.Count && i < excellencePropertyGroupItem.Max.Length)
						{
							num |= 1 << excellencePropertyGroupItem.Max[list[i]];
							i++;
						}
						result = num;
					}
				}
			}
			return result;
		}

		public List<FallGoodsItem> GetFallGoodsItemByPercent(List<FallGoodsItem> gallGoodsItemList, int maxFallCount, int fallAlgorithm, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> result;
			if (null == gallGoodsItemList)
			{
				result = gallGoodsItemList;
			}
			else if (gallGoodsItemList.Count <= 0)
			{
				result = gallGoodsItemList;
			}
			else
			{
				List<FallGoodsItem> list = new List<FallGoodsItem>();
				if (0 == fallAlgorithm)
				{
					bool flag = robotDropRate < 1.0;
					for (int i = 0; i < gallGoodsItemList.Count; i++)
					{
						int num = gallGoodsItemList[i].SelfPercent;
						if (flag)
						{
							double num2 = (double)num * robotDropRate;
							num = (int)num2;
						}
						int randomNumber = Global.GetRandomNumber(1, 100001);
						if (randomNumber <= num)
						{
							list.Add(gallGoodsItemList[i]);
						}
					}
					if (list.Count > maxFallCount)
					{
						list = Global.RandomSortList<FallGoodsItem>(list);
						list = list.GetRange(0, maxFallCount);
					}
				}
				else
				{
					for (int i = 0; i < maxFallCount; i++)
					{
						int randomNumber = Global.GetRandomNumber(1, 100001);
						FallGoodsItem fallGoodsItem = this.PickUpGoodsItemByPercent(gallGoodsItemList, randomNumber);
						if (null != fallGoodsItem)
						{
							list.Add(fallGoodsItem);
						}
					}
				}
				result = list;
			}
			return result;
		}

		private FallGoodsItem PickUpGoodsItemByPercent(List<FallGoodsItem> gallGoodsItemList, int randPercent)
		{
			FallGoodsItem result = null;
			for (int i = 0; i < gallGoodsItemList.Count; i++)
			{
				if (randPercent > gallGoodsItemList[i].BasePercent && randPercent <= gallGoodsItemList[i].BasePercent + gallGoodsItemList[i].SelfPercent)
				{
					result = gallGoodsItemList[i];
					break;
				}
			}
			return result;
		}

		public List<GoodsData> GetGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> normalFallGoodsItem = this.GetNormalFallGoodsItem(goodsPackID);
			List<GoodsData> result;
			if (null == normalFallGoodsItem)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fixedFallGoodsItemList = this.GetFixedFallGoodsItemList(goodsPackID);
				List<GoodsData> fixedGoodsDataList = this.GetFixedGoodsDataList(fixedFallGoodsItemList, this.GetFixedFallGoodsMaxCount(goodsPackID));
				List<FallGoodsItem> fallGoodsItemByPercent = this.GetFallGoodsItemByPercent(normalFallGoodsItem, maxFallCount, 0, robotDropRate);
				if (fallGoodsItemByPercent.Count <= 0)
				{
					if (fixedGoodsDataList == null || fixedGoodsDataList.Count <= 0)
					{
						return null;
					}
				}
				else
				{
					fallGoodsItemByPercent.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
				}
				List<GoodsData> list = new List<GoodsData>();
				if (fixedGoodsDataList != null && fixedGoodsDataList.Count > 0)
				{
					int num = GoodsPackManager.MaxFallCount - fallGoodsItemByPercent.Count;
					if (num > 0)
					{
						num = Global.GMin(num, fixedGoodsDataList.Count);
						for (int i = 0; i < num; i++)
						{
							GoodsData goodsData = fixedGoodsDataList[i];
							goodsData.Id = this.GetNextGoodsID();
							goodsData.Binding = Math.Max(goodsData.Binding, forceBinding);
							list.Add(goodsData);
						}
					}
				}
				for (int i = 0; i < fallGoodsItemByPercent.Count; i++)
				{
					int quality = 0;
					int fallGoodsLevel = this.GetFallGoodsLevel(fallGoodsItemByPercent[i].FallLevelID);
					int bornIndex = 0;
					int lucky = 0;
					int luckyGoodsID = this.GetLuckyGoodsID(fallGoodsItemByPercent[i].LuckyRate);
					if (luckyGoodsID > 0)
					{
						int luckyGoodsID2 = GameManager.GoodsPackMgr.GetLuckyGoodsID(luckyGoodsID);
						if (luckyGoodsID2 >= 1)
						{
							lucky = 1;
						}
					}
					int zhuiJiaGoodsLevelID = this.GetZhuiJiaGoodsLevelID(fallGoodsItemByPercent[i].ZhuiJiaID);
					int excellencePropertysID = this.GetExcellencePropertysID(fallGoodsItemByPercent[i].GoodsID, fallGoodsItemByPercent[i].ExcellencePropertyID);
					string props = "";
					GoodsData item = new GoodsData
					{
						Id = this.GetNextGoodsID(),
						GoodsID = fallGoodsItemByPercent[i].GoodsID,
						Using = 0,
						Forge_level = fallGoodsLevel,
						Starttime = "1900-01-01 12:00:00",
						Endtime = "1900-01-01 12:00:00",
						Site = 0,
						Quality = quality,
						Props = props,
						GCount = 1,
						Binding = Math.Max(fallGoodsItemByPercent[i].Binding, forceBinding),
						Jewellist = "",
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = bornIndex,
						Lucky = lucky,
						Strong = 0,
						ExcellenceInfo = excellencePropertysID,
						AppendPropLev = zhuiJiaGoodsLevelID,
						ChangeLifeLevForEquip = 0
					};
					list.Add(item);
				}
				result = list;
			}
			return result;
		}

		private List<GoodsData> GetEraGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<GoodsData> result;
			if (!(attacker is GameClient))
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> eraFallGoodsItem = EraManager.getInstance().GetEraFallGoodsItem(attacker as GameClient, goodsPackID);
				if (null == eraFallGoodsItem)
				{
					result = null;
				}
				else
				{
					List<FallGoodsItem> fallGoodsItemByPercent = this.GetFallGoodsItemByPercent(eraFallGoodsItem, maxFallCount, 0, robotDropRate);
					if (fallGoodsItemByPercent.Count <= 0)
					{
						result = null;
					}
					else
					{
						fallGoodsItemByPercent.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
						List<GoodsData> list = new List<GoodsData>();
						for (int i = 0; i < fallGoodsItemByPercent.Count; i++)
						{
							int quality = 0;
							int fallGoodsLevel = this.GetFallGoodsLevel(fallGoodsItemByPercent[i].FallLevelID);
							int bornIndex = 0;
							int luckyGoodsID = this.GetLuckyGoodsID(fallGoodsItemByPercent[i].LuckyRate);
							int zhuiJiaGoodsLevelID = this.GetZhuiJiaGoodsLevelID(fallGoodsItemByPercent[i].ZhuiJiaID);
							int excellencePropertysID = this.GetExcellencePropertysID(fallGoodsItemByPercent[i].GoodsID, fallGoodsItemByPercent[i].ExcellencePropertyID);
							string props = "";
							GoodsData item = new GoodsData
							{
								Id = this.GetNextGoodsID(),
								GoodsID = fallGoodsItemByPercent[i].GoodsID,
								Using = 0,
								Forge_level = fallGoodsLevel,
								Starttime = "1900-01-01 12:00:00",
								Endtime = "1900-01-01 12:00:00",
								Site = 0,
								Quality = quality,
								Props = props,
								GCount = 1,
								Binding = Math.Max(fallGoodsItemByPercent[i].Binding, forceBinding),
								Jewellist = "",
								BagIndex = 0,
								AddPropIndex = 0,
								BornIndex = bornIndex,
								Lucky = luckyGoodsID,
								Strong = 0,
								ExcellenceInfo = excellencePropertysID,
								AppendPropLev = zhuiJiaGoodsLevelID,
								ChangeLifeLevForEquip = 0
							};
							list.Add(item);
						}
						if (list.Count != 0)
						{
							if (!this.JudgeModifyGlobalFallGoodsLimit(goodsPackID))
							{
								return null;
							}
						}
						result = list;
					}
				}
			}
			return result;
		}

		private List<GoodsData> GetLimitTimeGoodsDataList(IObject attacker, int goodsPackID, int maxFallCount, int forceBinding, double robotDropRate = 1.0)
		{
			List<FallGoodsItem> limitTimeFallGoodsItem = this.GetLimitTimeFallGoodsItem(goodsPackID);
			List<GoodsData> result;
			if (null == limitTimeFallGoodsItem)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemByPercent = this.GetFallGoodsItemByPercent(limitTimeFallGoodsItem, maxFallCount, 0, robotDropRate);
				if (fallGoodsItemByPercent.Count <= 0)
				{
					result = null;
				}
				else
				{
					fallGoodsItemByPercent.Sort((FallGoodsItem item1, FallGoodsItem item2) => item2.SelfPercent - item1.SelfPercent);
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < fallGoodsItemByPercent.Count; i++)
					{
						int quality = 0;
						int fallGoodsLevel = this.GetFallGoodsLevel(fallGoodsItemByPercent[i].FallLevelID);
						int bornIndex = 0;
						int luckyGoodsID = this.GetLuckyGoodsID(fallGoodsItemByPercent[i].LuckyRate);
						int zhuiJiaGoodsLevelID = this.GetZhuiJiaGoodsLevelID(fallGoodsItemByPercent[i].ZhuiJiaID);
						int excellencePropertysID = this.GetExcellencePropertysID(fallGoodsItemByPercent[i].GoodsID, fallGoodsItemByPercent[i].ExcellencePropertyID);
						string props = "";
						GoodsData item = new GoodsData
						{
							Id = this.GetNextGoodsID(),
							GoodsID = fallGoodsItemByPercent[i].GoodsID,
							Using = 0,
							Forge_level = fallGoodsLevel,
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							Quality = quality,
							Props = props,
							GCount = 1,
							Binding = Math.Max(fallGoodsItemByPercent[i].Binding, forceBinding),
							Jewellist = "",
							BagIndex = 0,
							AddPropIndex = 0,
							BornIndex = bornIndex,
							Lucky = luckyGoodsID,
							Strong = 0,
							ExcellenceInfo = excellencePropertysID,
							AppendPropLev = zhuiJiaGoodsLevelID,
							ChangeLifeLevForEquip = 0
						};
						list.Add(item);
					}
					result = list;
				}
			}
			return result;
		}

		private bool JugeFuBenMapFall(MapGrid mapGrid, int copyMapID, int newGridX, int newGridY)
		{
			bool result;
			if (copyMapID <= 0)
			{
				result = false;
			}
			else
			{
				bool flag = true;
				List<object> list = mapGrid.FindObjects(newGridX, newGridY);
				if (null != list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is GoodsPackItem)
						{
							if ((list[i] as GoodsPackItem).CopyMapID == copyMapID)
							{
								flag = false;
								break;
							}
						}
					}
				}
				result = flag;
			}
			return result;
		}

		private bool JugeTaskTargetFall(MapGrid mapGrid, int copyMapID, int newGridX, int newGridY, GoodsPackItem goodsPackData)
		{
			bool result = true;
			List<object> list = mapGrid.FindObjects(newGridX, newGridY);
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GoodsPackItem)
					{
						if ((list[i] as GoodsPackItem).CopyMapID == copyMapID)
						{
							if ((list[i] as GoodsPackItem).OnlyID <= 0 || (list[i] as GoodsPackItem).OnlyID == goodsPackData.OnlyID)
							{
								result = false;
								break;
							}
						}
					}
				}
			}
			return result;
		}

		private Point FindABlankPoint(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			Point result = new Point(centerPoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), centerPoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			int num = (int)centerPoint.X;
			int num2 = (int)centerPoint.Y;
			for (int i = 1; i <= 5; i++)
			{
				for (int j = num - i; j <= num + i; j++)
				{
					int num3 = j;
					int num4 = num2 - i;
					string key = string.Format("{0}_{1}", num3, num4);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, num3, num4))
						{
							if (mapGrid.CanMove(objType, num3, num4, 0, 0))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
						}
					}
				}
				for (int j = num - i; j <= num + i; j++)
				{
					int num3 = j;
					int num4 = num2 + i;
					string key = string.Format("{0}_{1}", num3, num4);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, num3, num4))
						{
							if (mapGrid.CanMove(objType, num3, num4, 0, 0))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
						}
					}
				}
				for (int k = num2 - i + 1; k <= num2 + i - 1; k++)
				{
					int num4 = k;
					int num3 = num - i;
					string key = string.Format("{0}_{1}", num3, num4);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, num3, num4))
						{
							if (mapGrid.CanMove(objType, num3, num4, 0, 0))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
						}
					}
				}
				for (int k = num2 - i + 1; k <= num2 + i - 1; k++)
				{
					int num4 = k;
					int num3 = num + i;
					string key = string.Format("{0}_{1}", num3, num4);
					if (!dict.ContainsKey(key))
					{
						if (!Global.InOnlyObs(objType, mapCode, num3, num4))
						{
							if (mapGrid.CanMove(objType, num3, num4, 0, 0))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								dict[key] = true;
								result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
								return result;
							}
						}
					}
				}
			}
			return result;
		}

		private Point FindABlankPointEx(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID, IObject attacker)
		{
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			Point currentPos = new Point(centerPoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), centerPoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			int num = (int)centerPoint.X;
			int num2 = (int)centerPoint.Y;
			int i = 0;
			while (i < 200)
			{
				int num3 = (int)Global.ClientViewGridArray[i] + num;
				int num4 = (int)Global.ClientViewGridArray[i + 1] + num2;
				if (!Global.InOnlyObs(objType, mapCode, num3, num4))
				{
					string key = string.Format("{0}_{1}", num3, num4);
					if (!dict.ContainsKey(key))
					{
						Point result;
						if (mapGrid.CanMove(objType, num3, num4, 0, 0))
						{
							dict[key] = true;
							result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
						}
						else
						{
							if (!this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								goto IL_181;
							}
							dict[key] = true;
							result = new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
						}
						return result;
					}
				}
				IL_182:
				i += 2;
				continue;
				IL_181:
				goto IL_182;
			}
			if (null != attacker)
			{
				currentPos = attacker.CurrentPos;
				num = (int)currentPos.X;
				num2 = (int)currentPos.Y;
				i = 0;
				while (i < 200)
				{
					int num3 = (int)Global.ClientViewGridArray[i] + num;
					int num4 = (int)Global.ClientViewGridArray[i + 1] + num2;
					if (!Global.InOnlyObs(objType, mapCode, num3, num4))
					{
						string key = string.Format("{0}_{1}", num3, num4);
						if (!dict.ContainsKey(key))
						{
							if (mapGrid.CanMove(objType, num3, num4, 0, 0))
							{
								dict[key] = true;
								return new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
							}
							if (this.JugeFuBenMapFall(mapGrid, copyMapID, num3, num4))
							{
								dict[key] = true;
								return new Point((double)(num3 * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(num4 * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
							}
						}
					}
					IL_2D0:
					i += 2;
					continue;
					goto IL_2D0;
				}
			}
			return currentPos;
		}

		public Point GetFallGoodsPosition(ObjectTypes objType, int mapCode, Dictionary<string, bool> dict, Point centerPoint, int copyMapID, IObject attacker)
		{
			return this.FindABlankPointEx(objType, mapCode, dict, centerPoint, copyMapID, attacker);
		}

		private GoodsPackItem GetMonsterGoodsPackItem(GameClient client, int ownerRoleID, string ownerRoleName, int goodsPackID, List<int> teamRoleIDs, int mapCode, int copyMapID, int toX, int toY, int forceBinding, string monsterName, int belongTo, int fallLevel, int teamID)
		{
			int num = this.GetFallGoodsMaxCount(goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> list = this.GetGoodsDataList(client, goodsPackID, num, forceBinding, 1.0);
			num = this.GetLimitTimeFallGoodsMaxCount(goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> limitTimeGoodsDataList = this.GetLimitTimeGoodsDataList(client, goodsPackID, num, forceBinding, 1.0);
			num = EraManager.getInstance().GetEraFallGoodsMaxCount(client, goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> eraGoodsDataList = this.GetEraGoodsDataList(client, goodsPackID, num, forceBinding, 1.0);
			GoodsPackItem result;
			if (list == null && limitTimeGoodsDataList == null && null == eraGoodsDataList)
			{
				result = null;
			}
			else
			{
				if (null == list)
				{
					list = new List<GoodsData>();
				}
				if (null != limitTimeGoodsDataList)
				{
					list.AddRange(limitTimeGoodsDataList);
				}
				if (null != eraGoodsDataList)
				{
					list.AddRange(eraGoodsDataList);
				}
				GoodsPackItem goodsPackItem = new GoodsPackItem
				{
					AutoID = this.GetNextAutoID(),
					GoodsPackID = goodsPackID,
					OwnerRoleID = ownerRoleID,
					OwnerRoleName = ownerRoleName,
					GoodsPackType = 0,
					ProduceTicks = TimeUtil.NOW(),
					LockedRoleID = -1,
					GoodsDataList = list,
					TeamRoleIDs = teamRoleIDs,
					MapCode = mapCode,
					FallPoint = new Point((double)toX, (double)toY),
					CopyMapID = copyMapID,
					KilledMonsterName = monsterName,
					BelongTo = belongTo,
					FallLevel = fallLevel,
					TeamID = teamID
				};
				lock (this._GoodsPackDict)
				{
					this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
				}
				result = goodsPackItem;
			}
			return result;
		}

		public List<GoodsPackItem> GetMonsterGoodsPackItemList(IObject attacker, int ownerRoleID, string ownerRoleName, int goodsPackID, List<int> teamRoleIDs, int mapCode, int copyMapID, int toX, int toY, int forceBinding, string monsterName, int belongTo, int fallLevel, int teamID, int monsterType = -1, List<long> teamRoleDamages = null)
		{
			int num = this.GetFallGoodsMaxCount(goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			double num2 = 1.0;
			if (ownerRoleID > 0)
			{
				num2 = RobotTaskValidator.getInstance().GetRobotSceneDropRate(attacker as GameClient, mapCode, num2, monsterType);
			}
			List<GoodsData> list = this.GetGoodsDataList(attacker, goodsPackID, num, forceBinding, num2);
			num = this.GetLimitTimeFallGoodsMaxCount(goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> limitTimeGoodsDataList = this.GetLimitTimeGoodsDataList(attacker, goodsPackID, num, forceBinding, num2);
			num = EraManager.getInstance().GetEraFallGoodsMaxCount(attacker, goodsPackID);
			if (num <= 0)
			{
				num = GoodsPackManager.MaxFallCount;
			}
			List<GoodsData> eraGoodsDataList = this.GetEraGoodsDataList(attacker, goodsPackID, num, forceBinding, 1.0);
			List<GoodsPackItem> result;
			if (list == null && limitTimeGoodsDataList == null && null == eraGoodsDataList)
			{
				result = null;
			}
			else
			{
				if (null == list)
				{
					list = new List<GoodsData>();
				}
				if (null != limitTimeGoodsDataList)
				{
					list.AddRange(limitTimeGoodsDataList);
				}
				if (null != eraGoodsDataList)
				{
					list.AddRange(eraGoodsDataList);
				}
				Dictionary<string, bool> dict = new Dictionary<string, bool>();
				List<GoodsPackItem> list2 = new List<GoodsPackItem>();
				for (int i = 0; i < list.Count; i++)
				{
					List<GoodsData> list3 = new List<GoodsData>();
					list3.Add(list[i]);
					GoodsPackItem goodsPackItem = new GoodsPackItem
					{
						AutoID = this.GetNextAutoID(),
						GoodsPackID = goodsPackID,
						OwnerRoleID = ownerRoleID,
						OwnerRoleName = ownerRoleName,
						GoodsPackType = 0,
						ProduceTicks = TimeUtil.NOW(),
						LockedRoleID = -1,
						GoodsDataList = list3,
						TeamRoleIDs = teamRoleIDs,
						MapCode = mapCode,
						CopyMapID = copyMapID,
						KilledMonsterName = monsterName,
						BelongTo = belongTo,
						FallLevel = fallLevel,
						TeamID = teamID,
						TeamRoleDamages = teamRoleDamages
					};
					goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, dict, new Point((double)toX, (double)toY), copyMapID, attacker);
					list2.Add(goodsPackItem);
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
					}
				}
				result = list2;
			}
			return result;
		}

		private GoodsPackItem GetRoleGoodsPackItem(int ownerRoleID, string ownerRoleName, int goodsPackID, List<GoodsData> goodsDataList, int mapCode, int copyMapID, int toGridX, int toGridY, string fromRoleName)
		{
			GoodsPackItem goodsPackItem = new GoodsPackItem
			{
				AutoID = this.GetNextAutoID(),
				GoodsPackID = goodsPackID,
				OwnerRoleID = ownerRoleID,
				OwnerRoleName = ownerRoleName,
				GoodsPackType = 0,
				ProduceTicks = TimeUtil.NOW(),
				LockedRoleID = -1,
				GoodsDataList = goodsDataList,
				TeamRoleIDs = null,
				MapCode = mapCode,
				CopyMapID = copyMapID,
				KilledMonsterName = fromRoleName,
				BelongTo = -1,
				FallLevel = 0,
				TeamID = -1
			};
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, dict, new Point((double)toGridX, (double)toGridY), copyMapID, null);
			lock (this._GoodsPackDict)
			{
				this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
			}
			return goodsPackItem;
		}

		public List<GoodsPackItem> GetRoleGoodsPackItemList(int ownerRoleID, string ownerRoleName, List<GoodsData> goodsDataList, int mapCode, int copyMapID, int toGridX, int toGridY, string fromRoleName)
		{
			Dictionary<string, bool> dict = new Dictionary<string, bool>();
			List<GoodsPackItem> list = new List<GoodsPackItem>();
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				list2.Add(goodsDataList[i]);
				GoodsPackItem goodsPackItem = new GoodsPackItem
				{
					AutoID = this.GetNextAutoID(),
					GoodsPackID = this.GetNextRoleGoodsPackID(),
					OwnerRoleID = ownerRoleID,
					OwnerRoleName = ownerRoleName,
					GoodsPackType = 0,
					ProduceTicks = TimeUtil.NOW(),
					LockedRoleID = -1,
					GoodsDataList = list2,
					TeamRoleIDs = null,
					MapCode = mapCode,
					CopyMapID = copyMapID,
					KilledMonsterName = fromRoleName,
					BelongTo = -1,
					FallLevel = 0,
					TeamID = -1
				};
				goodsPackItem.FallPoint = this.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, mapCode, dict, new Point((double)toGridX, (double)toGridY), copyMapID, null);
				list.Add(goodsPackItem);
				lock (this._GoodsPackDict)
				{
					this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
				}
			}
			return list;
		}

		private GoodsPackItem GetBattleGoodsPackItem(int ownerRoleID, string ownerRoleName, int goodsPackID, List<GoodsData> awardsGoodsDataList, List<GoodsData> giveGoodsDataList, int mapCode, int copyMapID, int toX, int toY)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (null != awardsGoodsDataList)
			{
				for (int i = 0; i < awardsGoodsDataList.Count; i++)
				{
					list.Add(awardsGoodsDataList[i]);
				}
			}
			for (int i = 0; i < giveGoodsDataList.Count; i++)
			{
				list.Add(new GoodsData
				{
					Id = this.GetNextGoodsID(),
					GoodsID = giveGoodsDataList[i].GoodsID,
					Using = giveGoodsDataList[i].Using,
					Forge_level = giveGoodsDataList[i].Forge_level,
					Starttime = giveGoodsDataList[i].Starttime,
					Endtime = giveGoodsDataList[i].Endtime,
					Site = giveGoodsDataList[i].Site,
					Quality = giveGoodsDataList[i].Quality,
					Props = giveGoodsDataList[i].Props,
					GCount = giveGoodsDataList[i].GCount,
					Binding = giveGoodsDataList[i].Binding,
					Jewellist = giveGoodsDataList[i].Jewellist,
					BagIndex = 0,
					AddPropIndex = 0,
					BornIndex = 0,
					Lucky = 0,
					Strong = 0,
					ExcellenceInfo = 0,
					AppendPropLev = 0,
					ChangeLifeLevForEquip = 0
				});
			}
			GoodsPackItem goodsPackItem = new GoodsPackItem
			{
				AutoID = this.GetNextAutoID(),
				GoodsPackID = goodsPackID,
				OwnerRoleID = ownerRoleID,
				OwnerRoleName = ownerRoleName,
				GoodsPackType = 0,
				ProduceTicks = TimeUtil.NOW(),
				LockedRoleID = -1,
				GoodsDataList = list,
				TeamRoleIDs = null,
				MapCode = mapCode,
				FallPoint = new Point((double)toX, (double)toY),
				CopyMapID = copyMapID,
				KilledMonsterName = "",
				BelongTo = -1,
				FallLevel = 0,
				TeamID = -1
			};
			lock (this._GoodsPackDict)
			{
				this._GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
			}
			return goodsPackItem;
		}

		private int FindGoodsID2RoleID(GoodsPackItem goodsPackItem, int goodsDbID)
		{
			int result = -1;
			if (null != goodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (!goodsPackItem.GoodsIDToRolesDict.TryGetValue(goodsDbID, out result))
						{
							result = -1;
						}
					}
				}
			}
			return result;
		}

		private void AddGoodsID2RoleID(GoodsPackItem goodsPackItem, int goodsDbID, int roleID)
		{
			if (null != goodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					goodsPackItem.GoodsIDToRolesDict[goodsDbID] = roleID;
				}
			}
		}

		private void SendRandMessage(GameClient[] clientsArray, string msgText)
		{
			foreach (GameClient gameClient in clientsArray)
			{
				if (null != gameClient)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyChatBox, 200);
				}
			}
		}

		private void JugeGoodsID2RoleID(GameClient client, GoodsPackItem goodsPackItem, int goodsDbID, int goodsID)
		{
			int num = -1;
			GameClient gameClient = null;
			string goodsNameByID = Global.GetGoodsNameByID(goodsID);
			GameClient[] array = new GameClient[goodsPackItem.TeamRoleIDs.Count];
			int[] array2 = new int[goodsPackItem.TeamRoleIDs.Count];
			for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
			{
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(goodsPackItem.TeamRoleIDs[i]);
				if (null == gameClient2)
				{
					array[i] = null;
					array2[i] = -1;
				}
				else
				{
					int randomNumber = Global.GetRandomNumber(1, 101);
					array[i] = gameClient2;
					array2[i] = randomNumber;
					if (randomNumber > num)
					{
						num = randomNumber;
						gameClient = gameClient2;
					}
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				GameClient gameClient2 = array[i];
				if (null != gameClient2)
				{
					this.SendRandMessage(array, StringUtil.substitute(GLang.GetLang(379, new object[0]), new object[]
					{
						Global.FormatRoleName(gameClient2, gameClient2.ClientData.RoleName),
						goodsNameByID,
						array2[i]
					}));
				}
			}
			if (null != gameClient)
			{
				this.AddGoodsID2RoleID(goodsPackItem, goodsDbID, gameClient.ClientData.RoleID);
				this.SendRandMessage(array, StringUtil.substitute(GLang.GetLang(380, new object[0]), new object[]
				{
					Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName),
					goodsNameByID
				}));
			}
		}

		private void JugeGoodsID2RoleIDByDamageRandom(GameClient client, GoodsPackItem goodsPackItem, int goodsDbID, int goodsID)
		{
			GameClient gameClient = null;
			string goodsNameByID = Global.GetGoodsNameByID(goodsID);
			List<GameClient> list = new List<GameClient>(goodsPackItem.TeamRoleIDs.Count);
			List<long> list2 = new List<long>(goodsPackItem.TeamRoleIDs.Count);
			for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
			{
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(goodsPackItem.TeamRoleIDs[i]);
				if (null != gameClient2)
				{
					list.Add(gameClient2);
					list2.Add(goodsPackItem.TeamRoleDamages[i]);
					gameClient = gameClient2;
				}
			}
			long num = list2.Sum();
			long num2 = (long)((double)num * Global.GetRandom());
			for (int i = 0; i < list.Count; i++)
			{
				if (num2 <= list2[i])
				{
					gameClient = list[i];
					break;
				}
				num2 -= list2[i];
			}
			if (null != gameClient)
			{
				this.AddGoodsID2RoleID(goodsPackItem, goodsDbID, gameClient.ClientData.RoleID);
				this.SendRandMessage(list.ToArray(), GLang.GetLang(688, new object[]
				{
					Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName),
					goodsNameByID
				}));
			}
		}

		public GoodsPackItem FindGoodsPackItem(int autoID)
		{
			GoodsPackItem result = null;
			lock (this._GoodsPackDict)
			{
				if (!this._GoodsPackDict.TryGetValue(autoID, out result))
				{
					return null;
				}
			}
			return result;
		}

		public static bool IsFallTongQianGoods(int goodsID)
		{
			int num = (int)GameManager.systemParamsList.GetParamValueIntByName("FallTongQianGoodsID", -1);
			return num == goodsID;
		}

		private static bool ProcessFallTongQian(TCPOutPacketPool pool, GameClient client, int goodsID, int goodsNum, int fallLevel)
		{
			bool result;
			if (!GoodsPackManager.IsFallTongQianGoods(goodsID))
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemDropMoney.SystemXmlItemDict.TryGetValue(fallLevel, out systemXmlItem))
				{
					result = true;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("MinMoney", -1);
					int intValue2 = systemXmlItem.GetIntValue("MaxMoney", -1);
					for (int i = 0; i < goodsNum; i++)
					{
						int num = Global.GetRandomNumber(intValue, intValue2);
						num = Global.FilterValue(client, num);
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, pool, client, num, "拾取金币", false);
					}
					result = true;
				}
			}
			return result;
		}

		public bool AutoAddThingIntoBag(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsPackItem goodsPackItem, GoodsData goodsData)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				GameClient gameClient = null;
				int num = this.FindGoodsID2RoleID(goodsPackItem, goodsData.Id);
				if (-1 == num)
				{
					gameClient = client;
				}
				else
				{
					gameClient = GameManager.ClientMgr.FindClient(num);
				}
				if (null == gameClient)
				{
					result = false;
				}
				else if (Global.CanAddGoods(gameClient, goodsData.GoodsID, 1, goodsData.Binding, "1900-01-01 12:00:00", true, false) || GoodsPackManager.IsFallTongQianGoods(goodsData.GoodsID))
				{
					lock (this._GoodsPackDict)
					{
						goodsPackItem.GoodsIDDict[goodsData.Id] = true;
					}
					if (!GoodsPackManager.ProcessFallTongQian(pool, gameClient, goodsData.GoodsID, goodsData.GCount, goodsPackItem.FallLevel))
					{
						int num2 = Global.AddGoodsDBCommand_Hook(pool, gameClient, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "杀怪掉落后自动拾取", true, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
						if (0 == num2)
						{
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, Global.ModifyGoodsLogName(goodsData), "杀怪掉落后自动拾取", Global.GetMapName(client.ClientData.MapCode), "系统", "销毁", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsData);
						}
					}
					GameManager.ClientMgr.NotifySelfGetThing(sl, pool, gameClient, goodsData.Id);
					SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PickUpEquipCount);
					sevenDayGoalEventObject.Arg1 = goodsData.GoodsID;
					sevenDayGoalEventObject.Arg2 = goodsData.GCount;
					GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private bool CanAutoFightGetThings(GameClient client, GoodsPackItem goodsPackItem, GoodsData goodsData)
		{
			bool result;
			if (!client.ClientData.AutoFighting)
			{
				result = false;
			}
			else if (client.ClientData.AutoFightGetThings <= 0)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) || null == systemXmlItem)
				{
					LogManager.WriteLog(1, string.Format("处理自动挂机拾取物品到背包时，获取物品xml信息失败: GoodsID={0}", goodsData.GoodsID), null, true);
					result = false;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
					if (501 == intValue)
					{
						if (4 != ((byte)client.ClientData.AutoFightGetThings & 4))
						{
							return false;
						}
					}
					else if (180 == intValue)
					{
						if (8 != ((byte)client.ClientData.AutoFightGetThings & 8))
						{
							return false;
						}
					}
					else if (2 != ((byte)client.ClientData.AutoFightGetThings & 2))
					{
						return false;
					}
					result = true;
				}
			}
			return result;
		}

		public bool AutoGetThings(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsPackItem goodsPackItem)
		{
			if (client.ClientData.AutoFighting)
			{
				if (goodsPackItem.TeamRoleIDs != null)
				{
					return false;
				}
			}
			bool result;
			if (goodsPackItem.GoodsDataList == null || goodsPackItem.GoodsDataList.Count <= 0)
			{
				result = true;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
				{
					if (this.CanAutoFightGetThings(client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
					{
						if (this.AutoAddThingIntoBag(sl, pool, client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
						{
							num++;
						}
					}
					else if (Data.AutoGetThing > 0)
					{
						if (this.AutoAddThingIntoBag(sl, pool, client, goodsPackItem, goodsPackItem.GoodsDataList[i]))
						{
							num++;
						}
					}
				}
				result = (num >= goodsPackItem.GoodsDataList.Count);
			}
			return result;
		}

		public string FormatTeamRoleIDs(GoodsPackItem goodsPackItem)
		{
			string text = "";
			string result;
			if (null == goodsPackItem)
			{
				result = text;
			}
			else
			{
				if (goodsPackItem.TeamRoleIDs != null && goodsPackItem.TeamRoleIDs.Count > 0)
				{
					for (int i = 0; i < goodsPackItem.TeamRoleIDs.Count; i++)
					{
						if (text.Length > 0)
						{
							text += ",";
						}
						text += goodsPackItem.TeamRoleIDs[i].ToString();
					}
				}
				result = text;
			}
			return result;
		}

		public List<GoodsPackItem> ProcessMonster(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster)
		{
			List<GoodsPackItem> result;
			if (attacker is GameClient)
			{
				result = this.ProcessMonsterByClient(sl, pool, attacker as GameClient, monster);
			}
			else
			{
				result = this.ProcessMonsterByMonster(sl, pool, attacker as Monster, monster);
			}
			return result;
		}

		public List<GoodsPackItem> ProcessMonsterByClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster)
		{
			JunTuanManager.getInstance().AddJunTuanTaskValue(client, monster, 1, 1);
			RebornManager.getInstance().ProcessRebornMonsterFallGoods(client, monster);
			List<GoodsPackItem> result;
			if (!Global.FilterFallGoods(client))
			{
				result = null;
			}
			else if (monster.MonsterInfo.FallGoodsPackID < 0)
			{
				result = null;
			}
			else
			{
				bool flag = true;
				if (monster.CurrentMapCode == GameManager.ArenaBattleMgr.BattleMapCode)
				{
					flag = false;
				}
				GameClient gameClient = client;
				if (3 == monster.MonsterInfo.FallBelongTo && MoYuLongXue.InMoYuMap(monster.CurrentMapCode))
				{
					int num = MoYuLongXue.KillerRid(monster);
					gameClient = GameManager.ClientMgr.FindClient(num);
					if (null == gameClient)
					{
						RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, num), 0);
						if (null == roleDataEx)
						{
							LogManager.WriteLog(2, "MoYuLongXue :: 道具归属权，但是查不到角色数据。", null, true);
							gameClient = client;
						}
						gameClient = new GameClient
						{
							ClientData = new SafeClientData
							{
								RoleData = roleDataEx
							}
						};
					}
					flag = false;
				}
				if (4 == monster.MonsterInfo.FallBelongTo && ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(monster.CurrentMapCode))
				{
					gameClient = GameManager.ClientMgr.FindClient(ZhuanShengShiLian.KillerRid(client, monster));
					if (null == gameClient)
					{
						gameClient = client;
					}
				}
				SceneUIClasses mapSceneType = Global.GetMapSceneType(monster.CurrentMapCode);
				if (5 == monster.MonsterInfo.FallBelongTo && mapSceneType == 48)
				{
					CompMapClientContextData bossTopDamageClientContext = CompManager.getInstance().GetBossTopDamageClientContext(monster);
					if (null == bossTopDamageClientContext)
					{
						gameClient = client;
					}
					else
					{
						gameClient = GameManager.ClientMgr.FindClient(bossTopDamageClientContext.RoleId);
						if (null == gameClient)
						{
							RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bossTopDamageClientContext.RoleId), bossTopDamageClientContext.ServerId);
							if (null == roleDataEx)
							{
								LogManager.WriteLog(2, "CompBoss :: 道具归属权，但是查不到角色数据。", null, true);
								gameClient = client;
							}
							gameClient = new GameClient
							{
								ClientData = new SafeClientData
								{
									RoleData = roleDataEx
								}
							};
						}
					}
					flag = false;
				}
				int teamID = -1;
				List<int> list = null;
				List<long> teamRoleDamages = null;
				if (gameClient.ClientData.TeamID > 0 && flag)
				{
					TeamData teamData = GameManager.TeamMgr.FindData(gameClient.ClientData.TeamID);
					if (teamData != null && teamData.GetThingOpt > 0)
					{
						lock (teamData)
						{
							teamID = teamData.TeamID;
							list = new List<int>();
							long num2 = 0L;
							for (int i = 0; i < teamData.TeamRoles.Count; i++)
							{
								if (teamData.TeamRoles[i].RoleID == gameClient.ClientData.RoleID)
								{
									list.Add(teamData.TeamRoles[i].RoleID);
								}
								else
								{
									GameClient gameClient2 = GameManager.ClientMgr.FindClient(teamData.TeamRoles[i].RoleID);
									if (null != gameClient2)
									{
										if (gameClient2.ClientData.MapCode == monster.CurrentMapCode)
										{
											if (gameClient2.ClientData.CopyMapID == monster.CurrentCopyMapID)
											{
												if (Global.InCircle(new Point((double)gameClient2.ClientData.PosX, (double)gameClient2.ClientData.PosY), monster.SafeCoordinate, 800.0))
												{
													if (teamData.GetThingOpt == 2 && GoodsPackManager.TeamShareMode_MaxDamage)
													{
														long attackerDamage = monster.GetAttackerDamage(teamData.TeamRoles[i].RoleID);
														if (attackerDamage > num2)
														{
															num2 = attackerDamage;
															gameClient = gameClient2;
														}
													}
													else
													{
														list.Add(teamData.TeamRoles[i].RoleID);
													}
												}
											}
										}
									}
								}
							}
							if (list.Count <= 1)
							{
								list = null;
							}
							else if (teamData.GetThingOpt == 3 && GoodsPackManager.TeamShareMode_RandomByDamage)
							{
								teamRoleDamages = monster.GetAttackerDamageList(list);
							}
						}
					}
				}
				int forceBinding = -1;
				Point currentGrid = monster.CurrentGrid;
				List<GoodsPackItem> monsterGoodsPackItemList = this.GetMonsterGoodsPackItemList(gameClient, gameClient.ClientData.RoleID, Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName), monster.MonsterInfo.FallGoodsPackID, list, monster.CurrentMapCode, monster.CurrentCopyMapID, (int)currentGrid.X, (int)currentGrid.Y, forceBinding, monster.MonsterInfo.VSName, monster.MonsterInfo.FallBelongTo, monster.MonsterInfo.VLevel, teamID, monster.MonsterType, teamRoleDamages);
				if (monsterGoodsPackItemList == null || monsterGoodsPackItemList.Count <= 0)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < monsterGoodsPackItemList.Count; i++)
					{
						this.ProcessGoodsPackItem(gameClient, monster, monsterGoodsPackItemList[i], forceBinding);
						bool bNeedSend = true;
						if (monster.MonsterInfo.ExtensionID == 1800 || monster.MonsterInfo.ExtensionID == 1900 || monster.MonsterInfo.ExtensionID == 2900 || monster.MonsterInfo.ExtensionID == 3900 || monster.MonsterInfo.ExtensionID == 4900 || monster.MonsterInfo.ExtensionID == 5900 || monster.MonsterInfo.ExtensionID == 6900 || monster.MonsterInfo.ExtensionID == 7900 || monster.MonsterInfo.ExtensionID == 8900)
						{
							bNeedSend = false;
						}
						Global.BroadcastGetGoodsHint(gameClient, monsterGoodsPackItemList[i].GoodsDataList[0], monster.MonsterInfo.VSName, monster.CurrentMapCode, bNeedSend);
					}
					result = monsterGoodsPackItemList;
				}
			}
			return result;
		}

		public List<GoodsPackItem> ProcessMonsterByMonster(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster monster)
		{
			List<GoodsPackItem> result;
			if (monster.MonsterInfo.FallGoodsPackID < 0)
			{
				result = null;
			}
			else
			{
				int forceBinding = 0;
				Point currentGrid = monster.CurrentGrid;
				List<GoodsPackItem> monsterGoodsPackItemList = this.GetMonsterGoodsPackItemList(attacker, -1, "", monster.MonsterInfo.FallGoodsPackID, null, attacker.MonsterZoneNode.MapCode, attacker.CopyMapID, (int)currentGrid.X, (int)currentGrid.Y, forceBinding, monster.MonsterInfo.VSName, monster.MonsterInfo.FallBelongTo, monster.MonsterInfo.VLevel, -1, -1, null);
				if (monsterGoodsPackItemList == null || monsterGoodsPackItemList.Count <= 0)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < monsterGoodsPackItemList.Count; i++)
					{
						this.ProcessGoodsPackItem(attacker, monster, monsterGoodsPackItemList[i], forceBinding);
					}
					result = monsterGoodsPackItemList;
				}
			}
			return result;
		}

		public void ProcessTaskDropByTargetNum(GameClient client, string goodsData, Monster monster)
		{
			if (!string.IsNullOrEmpty(goodsData))
			{
				List<FallGoodsItem> list = this.ParseGoodsDataList(0, goodsData);
				list = this.GetFallGoodsItemByPercent(list, int.MaxValue, 0, 1.0);
				List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(list);
				if (null != goodsDataListFromFallGoodsItemList)
				{
					Dictionary<string, bool> dict = new Dictionary<string, bool>();
					List<GoodsPackItem> list2 = new List<GoodsPackItem>();
					for (int i = 0; i < goodsDataListFromFallGoodsItemList.Count; i++)
					{
						if (Global.IsRoleOccupationMatchGoods(client, goodsDataListFromFallGoodsItemList[i].GoodsID))
						{
							List<GoodsData> list3 = new List<GoodsData>();
							list3.Add(goodsDataListFromFallGoodsItemList[i]);
							GoodsPackItem goodsPackItem = new GoodsPackItem
							{
								AutoID = GameManager.GoodsPackMgr.GetNextAutoID(),
								GoodsPackID = 0,
								OwnerRoleID = client.ClientData.RoleID,
								OwnerRoleName = client.ClientData.RoleName,
								GoodsPackType = 0,
								ProduceTicks = TimeUtil.NOW(),
								LockedRoleID = -1,
								GoodsDataList = list3,
								TeamRoleIDs = null,
								MapCode = ((monster == null) ? client.ClientData.MapCode : monster.CurrentMapCode),
								CopyMapID = ((monster == null) ? client.ClientData.CopyMapID : monster.CurrentCopyMapID),
								KilledMonsterName = null,
								BelongTo = 1,
								FallLevel = 0,
								TeamID = -1,
								OnlyID = client.ClientData.RoleID
							};
							double num = (monster == null) ? client.CurrentGrid.X : monster.CurrentGrid.X;
							double num2 = (monster == null) ? client.CurrentGrid.Y : monster.CurrentGrid.Y;
							goodsPackItem.FallPoint = GameManager.GoodsPackMgr.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, client.ClientData.MapCode, dict, new Point((double)((int)num), (double)((int)num2)), client.ClientData.CopyMapID, client);
							list2.Add(goodsPackItem);
							lock (GameManager.GoodsPackMgr.GoodsPackDict)
							{
								GameManager.GoodsPackMgr.GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
							}
						}
					}
					for (int j = 0; j < list2.Count; j++)
					{
						GameManager.GoodsPackMgr.ProcessGoodsPackItem(client, client, list2[j], 1);
					}
				}
			}
		}

		public void ProcessGoodsPackItem(IObject attacker, IObject obj, GoodsPackItem goodsPackItem, int forceBinding)
		{
			if (null != goodsPackItem)
			{
				GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].MoveObject(-1, -1, (int)goodsPackItem.FallPoint.X, (int)goodsPackItem.FallPoint.Y, goodsPackItem);
				this.WriteFallGoodsRecords(goodsPackItem);
			}
		}

		public void ProcessRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, string enemyName, out string strDropList)
		{
			strDropList = "";
			if (Global.CanMapLostEquip(client.ClientData.MapCode))
			{
				if (Global.FilterFallGoods(client))
				{
					if (null != otherClient.ClientData.GoodsDataList)
					{
						lock (otherClient.ClientData.GoodsDataList)
						{
							if (otherClient.ClientData.GoodsDataList.Count <= 0)
							{
								return;
							}
						}
						int num = 1;
						int num2 = 3;
						if (Global.IsRedName(otherClient))
						{
							int num3 = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MaxFallRedRoleBagRate", -1));
							int num4 = Global.GMax(0, (int)GameManager.systemParamsList.GetParamValueIntByName("MaxFallRedRoleUsingRate", -1));
							num3 *= otherClient.ClientData.PKPoint / 100 - 1;
							num4 *= otherClient.ClientData.PKPoint / 100 - 1;
							List<GoodsData> list = new List<GoodsData>();
							if (num3 > 0)
							{
								List<GoodsData> fallGoodsList = Global.GetFallGoodsList(otherClient);
								if (fallGoodsList != null && fallGoodsList.Count > 0)
								{
									int num5 = 0;
									for (int i = 0; i < fallGoodsList.Count; i++)
									{
										int randomNumber = Global.GetRandomNumber(1, 100001);
										if (randomNumber <= num3)
										{
											GoodsData goodsData = fallGoodsList[i];
											if (null != goodsData)
											{
												int gcount = 1;
												if (Global.GetGoodsDefaultCount(goodsData.GoodsID) > 1)
												{
													gcount = goodsData.GCount;
												}
												if (GameManager.ClientMgr.FallRoleGoods(sl, Global._TCPManager.tcpClientPool, pool, otherClient, goodsData))
												{
													num5++;
													goodsData = Global.CopyGoodsData(goodsData);
													goodsData.Id = this.GetNextGoodsID();
													goodsData.GCount = gcount;
													list.Add(goodsData);
												}
											}
											if (num5 >= num2)
											{
												break;
											}
										}
									}
								}
							}
							if (num4 > 0)
							{
								List<GoodsData> usingGoodsList = Global.GetUsingGoodsList(otherClient, 0);
								if (usingGoodsList != null && usingGoodsList.Count > 0)
								{
									int num6 = 0;
									for (int i = 0; i < usingGoodsList.Count; i++)
									{
										int goodsCatetoriy = Global.GetGoodsCatetoriy(usingGoodsList[i].GoodsID);
										int randomNumber = Global.GetRandomNumber(1, 100001);
										int num7 = num4;
										if (randomNumber <= num7)
										{
											GoodsData goodsData = usingGoodsList[i];
											if (null != goodsData)
											{
												int gcount = goodsData.GCount;
												if (GameManager.ClientMgr.FallRoleGoods(sl, Global._TCPManager.tcpClientPool, pool, otherClient, goodsData))
												{
													num6++;
													goodsData.Id = this.GetNextGoodsID();
													goodsData.GCount = gcount;
													list.Add(goodsData);
													Global.NotifyChangeEquip(Global._TCPManager, pool, otherClient, goodsData, 1);
													goodsData.Using = 0;
													otherClient.UsingEquipMgr.RefreshEquip(goodsData);
												}
											}
											if (num6 >= num)
											{
												break;
											}
										}
									}
									if (num6 > 0)
									{
										Global.RefreshEquipProp(otherClient);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, pool, otherClient);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, pool, otherClient, true, false, 7);
									}
								}
							}
							if (list.Count > 0)
							{
								strDropList = EventLogManager.MakeGoodsDataPropString(list);
								Point currentGrid = otherClient.CurrentGrid;
								List<GoodsPackItem> roleGoodsPackItemList = this.GetRoleGoodsPackItemList(client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), list, otherClient.ClientData.MapCode, otherClient.ClientData.CopyMapID, (int)currentGrid.X, (int)currentGrid.Y, enemyName);
								if (roleGoodsPackItemList != null && roleGoodsPackItemList.Count > 0)
								{
									StringBuilder stringBuilder = new StringBuilder();
									for (int i = 0; i < roleGoodsPackItemList.Count; i++)
									{
										this.ProcessGoodsPackItem(client, otherClient, roleGoodsPackItemList[i], 0);
										stringBuilder.AppendFormat("{0}", Global.GetGoodsNameByID(roleGoodsPackItemList[i].GoodsDataList[0].GoodsID));
										if (i != roleGoodsPackItemList.Count - 1)
										{
											stringBuilder.Append(" ");
										}
									}
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(383, new object[0]), new object[]
									{
										enemyName,
										stringBuilder.ToString()
									}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
								}
							}
						}
					}
				}
			}
		}

		public void ProcessRoleAbandonGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int toGridX, int toGridY)
		{
			List<GoodsData> list = new List<GoodsData>();
			list.Add(goodsData);
			Point currentGrid = client.CurrentGrid;
			List<GoodsPackItem> roleGoodsPackItemList = this.GetRoleGoodsPackItemList(client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), list, client.ClientData.MapCode, client.ClientData.CopyMapID, (int)currentGrid.X, (int)currentGrid.Y, Global.FormatRoleName(client, client.ClientData.RoleName));
			if (roleGoodsPackItemList != null && roleGoodsPackItemList.Count > 0)
			{
				for (int i = 0; i < roleGoodsPackItemList.Count; i++)
				{
					this.ProcessGoodsPackItem(client, null, roleGoodsPackItemList[i], 0);
				}
			}
		}

		private GameClient GetBattleRandomClient(List<BattleRoleItem> battleRoleItemList)
		{
			int randomNumber = Global.GetRandomNumber(0, 101);
			int num = 0;
			int num2 = 0;
			while (num2 < battleRoleItemList.Count && num2 < 10)
			{
				if ((double)randomNumber > battleRoleItemList[num2].Percent)
				{
					break;
				}
				num++;
				num2++;
			}
			GameClient result;
			if (num < 0)
			{
				result = null;
			}
			else
			{
				int randomNumber2 = Global.GetRandomNumber(0, num);
				result = battleRoleItemList[randomNumber2].Client;
			}
			return result;
		}

		private void AddBattleBufferAndFlags(GameClient client, int bufferType)
		{
			double[] array = new double[2];
			array[0] = 1440.0;
			if (0 == bufferType)
			{
				array[1] = 20.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 1;
			}
			else if (1 == bufferType)
			{
				array[1] = 15.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 2;
			}
			else
			{
				if (2 != bufferType)
				{
					return;
				}
				array[1] = 10.0;
				client.ClientData.BattleNameStart = TimeUtil.NOW();
				client.ClientData.BattleNameIndex = 3;
			}
			Global.UpdateBufferData(client, BufferItemTypes.AntiRole, array, 0, true);
			GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
			GameManager.ClientMgr.NotifyRoleBattleNameInfo(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.UpdateBattleNum(client, 1, false);
		}

		public void ProcessBattle(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, List<GoodsData> giveGoodsDataList, int fallGoodsPackID, int fallNum)
		{
		}

		public void SendMySelfGoodsPackItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				GoodsPackItem goodsPackItem = null;
				int num = 0;
				while (num < objsList.Count && num < 30)
				{
					if (objsList[num] is GoodsPackItem)
					{
						goodsPackItem = (objsList[num] as GoodsPackItem);
						if (goodsPackItem.OnlyID <= 0 || goodsPackItem.OnlyID == client.ClientData.RoleID)
						{
							if (goodsPackItem.GoodsDataList.Count > 0)
							{
								GoodsData goodsData = goodsPackItem.GoodsDataList[0];
								if (null != goodsData)
								{
									string teamRoleIDs = this.FormatTeamRoleIDs(goodsPackItem);
									int num2 = goodsData.ExcellenceInfo;
									lock (RebornEquip.SuperiorDrop)
									{
										if (RebornEquip.IsRebornEquip(goodsData.GoodsID) && RebornEquip.SuperiorDrop != null)
										{
											if (RebornEquip.SuperiorDrop.ContainsKey(num2))
											{
												num2 = RebornEquip.SuperiorDrop[num2].ShowColor;
											}
										}
									}
									GameManager.ClientMgr.NotifyMySelfNewGoodsPack(sl, pool, client, (goodsPackItem.BelongTo <= 0) ? -1 : goodsPackItem.OwnerRoleID, goodsPackItem.OwnerRoleName, goodsPackItem.AutoID, goodsPackItem.GoodsPackID, goodsPackItem.MapCode, (int)goodsPackItem.FallPoint.X, (int)goodsPackItem.FallPoint.Y, goodsData.GoodsID, goodsData.GCount, goodsPackItem.ProduceTicks, goodsPackItem.TeamID, teamRoleIDs, goodsData.Lucky, num2, goodsData.AppendPropLev, goodsData.Forge_level);
								}
							}
						}
					}
					num++;
				}
			}
		}

		public void DelMySelfGoodsPackItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GoodsPackItem)
					{
						GoodsPackItem goodsPackItem = objsList[i] as GoodsPackItem;
						if (this.CanOpenGoodsPack(goodsPackItem, client.ClientData.RoleID))
						{
							List<GoodsData> goodsDataList = goodsPackItem.GoodsDataList;
							if (goodsDataList != null)
							{
								for (int j = 0; j < goodsDataList.Count; j++)
								{
									if (!GoodsPackManager.IsFallTongQianGoods(goodsDataList[j].GoodsID) && !Global.CanAddGoods(client, goodsDataList[j].GoodsID, goodsDataList[j].GCount, goodsDataList[j].Binding, "1900-01-01 12:00:00", true, false))
									{
										return;
									}
								}
							}
							GameManager.ClientMgr.NotifyMySelfDelGoodsPack(sl, pool, client, goodsPackItem.AutoID);
						}
					}
				}
			}
		}

		public void ProcessAllGoodsPackItems(SocketListener sl, TCPOutPacketPool pool)
		{
			List<GoodsPackItem> list = new List<GoodsPackItem>();
			lock (this._GoodsPackDict)
			{
				foreach (GoodsPackItem item in this._GoodsPackDict.Values)
				{
					list.Add(item);
				}
			}
			long num = TimeUtil.NOW();
			GoodsPackItem goodsPackItem = null;
			for (int i = 0; i < list.Count; i++)
			{
				goodsPackItem = list[i];
				if (num - goodsPackItem.ProduceTicks >= (long)(Data.PackDestroyTimeTick * 1000))
				{
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict.Remove(goodsPackItem.AutoID);
					}
					GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
				}
			}
		}

		private bool CanOpenGoodsPack(GoodsPackItem goodsPackItem, int roleID)
		{
			bool result;
			if (goodsPackItem.OnlyID > 0 && goodsPackItem.OnlyID != roleID)
			{
				result = false;
			}
			else if (goodsPackItem.BelongTo <= 0)
			{
				result = true;
			}
			else
			{
				long num = TimeUtil.NOW();
				if (num - goodsPackItem.ProduceTicks >= (long)(Data.GoodsPackOvertimeTick * 1000))
				{
					result = true;
				}
				else
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (-1 != goodsPackItem.TeamRoleIDs.IndexOf(roleID))
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != gameClient)
							{
								bool flag = true;
								if (gameClient.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode)
								{
									flag = false;
								}
								TeamData teamData = GameManager.TeamMgr.FindData(gameClient.ClientData.TeamID);
								if (teamData != null)
								{
									if (teamData.GetThingOpt > 0 && flag)
									{
										return true;
									}
								}
							}
						}
					}
					result = (goodsPackItem.OwnerRoleID < 0 || goodsPackItem.OwnerRoleID == roleID);
				}
			}
			return result;
		}

		public void UnLockGoodsPackItem(GameClient client)
		{
			if (null != client.ClientData.LockedGoodsPackItem)
			{
				lock (this._GoodsPackDict)
				{
					if (this._GoodsPackDict.ContainsKey(client.ClientData.LockedGoodsPackItem.AutoID))
					{
						client.ClientData.LockedGoodsPackItem.LockedRoleID = -1;
					}
				}
				client.ClientData.LockedGoodsPackItem = null;
			}
		}

		public List<GoodsData> GetLeftGoodsDataList(GoodsPackItem goodsPackItem)
		{
			List<GoodsData> result;
			if (goodsPackItem.GoodsDataList == null)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
				{
					if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsPackItem.GoodsDataList[i].Id))
					{
						list.Add(goodsPackItem.GoodsDataList[i]);
					}
				}
				result = list;
			}
			return result;
		}

		private void ProcessTeamGoodsPack(GameClient client, GoodsPackItem goodsPackItem)
		{
			if (null != goodsPackItem)
			{
				if (TimeUtil.NOW() - goodsPackItem.ProduceTicks < (long)(Data.GoodsPackOvertimeTick * 1000))
				{
					if (null != goodsPackItem.TeamRoleIDs)
					{
						if (goodsPackItem.GoodsIDToRolesDict.Count <= 0)
						{
							if (null == this.ProhibitRollHashSet)
							{
								this.ProhibitRollHashSet = new HashSet<int>();
								try
								{
									int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ProhibitRoll", ',');
									if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
									{
										foreach (int num in paramValueIntArrayByName)
										{
											this.ProhibitRollHashSet.Add(num);
										}
									}
								}
								catch
								{
								}
							}
							for (int j = 0; j < goodsPackItem.GoodsDataList.Count; j++)
							{
								int num = goodsPackItem.GoodsDataList[j].GoodsID;
								if (!this.ProhibitRollHashSet.Contains(num))
								{
									if (null == goodsPackItem.TeamRoleDamages)
									{
										this.JugeGoodsID2RoleID(client, goodsPackItem, goodsPackItem.GoodsDataList[j].Id, num);
									}
									else
									{
										this.JugeGoodsID2RoleIDByDamageRandom(client, goodsPackItem, goodsPackItem.GoodsDataList[j].Id, num);
									}
								}
							}
						}
					}
				}
			}
		}

		public GoodsPackListData ProcessClickOnGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID, out TCPOutPacket tcpOutPacket, int nID, int openState, bool tcpPacketData)
		{
			tcpOutPacket = null;
			int retError = 0;
			long leftTicks = 0L;
			long packTicks = -1L;
			List<GoodsData> list = null;
			GoodsPackItem goodsPackItem = null;
			lock (this._GoodsPackDict)
			{
				if (this._GoodsPackDict.TryGetValue(autoID, out goodsPackItem))
				{
					if (openState > 0)
					{
						if (goodsPackItem != null)
						{
							List<GoodsData> leftGoodsDataList = this.GetLeftGoodsDataList(goodsPackItem);
							if (leftGoodsDataList != null)
							{
								for (int i = 0; i < leftGoodsDataList.Count; i++)
								{
									if (!GoodsPackManager.IsFallTongQianGoods(leftGoodsDataList[i].GoodsID) && !Global.CanAddGoods(client, leftGoodsDataList[i].GoodsID, leftGoodsDataList[i].GCount, leftGoodsDataList[i].Binding, "1900-01-01 12:00:00", true, false))
									{
										return null;
									}
								}
							}
						}
						if (this.CanOpenGoodsPack(goodsPackItem, client.ClientData.RoleID))
						{
							if (-1 == goodsPackItem.LockedRoleID || goodsPackItem.LockedRoleID == client.ClientData.RoleID)
							{
								goodsPackItem.LockedRoleID = client.ClientData.RoleID;
								client.ClientData.LockedGoodsPackItem = goodsPackItem;
								list = this.GetLeftGoodsDataList(goodsPackItem);
								this.ProcessTeamGoodsPack(client, goodsPackItem);
								goodsPackItem.OpenPackTicks = TimeUtil.NOW();
								if (null != goodsPackItem.TeamRoleIDs)
								{
									long num = 0L;
									goodsPackItem.RolesTicksDict.TryGetValue(client.ClientData.RoleID, out num);
									packTicks = 15000L - num;
								}
							}
							else
							{
								retError = -3;
								goodsPackItem = null;
							}
						}
						else
						{
							long num2 = TimeUtil.NOW();
							leftTicks = (long)(Data.GoodsPackOvertimeTick * 1000) - (num2 - goodsPackItem.ProduceTicks);
							if (goodsPackItem.TeamRoleIDs != null && -1 != goodsPackItem.TeamRoleIDs.IndexOf(client.ClientData.RoleID))
							{
								packTicks = -2L;
							}
							retError = -2;
							goodsPackItem = null;
							LogManager.WriteLog(0, "retError : -2", null, true);
						}
					}
					else
					{
						long num = 0L;
						goodsPackItem.RolesTicksDict.TryGetValue(client.ClientData.RoleID, out num);
						goodsPackItem.RolesTicksDict[client.ClientData.RoleID] = num + (TimeUtil.NOW() - goodsPackItem.OpenPackTicks);
						goodsPackItem.LockedRoleID = -1;
						client.ClientData.LockedGoodsPackItem = null;
						goodsPackItem = null;
					}
				}
				else
				{
					retError = -1;
				}
			}
			List<GoodsData> goodsDataList = null;
			if (goodsPackItem != null)
			{
				goodsDataList = list;
			}
			GoodsPackListData goodsPackListData = new GoodsPackListData
			{
				AutoID = autoID,
				GoodsDataList = goodsDataList,
				OpenState = openState,
				RetError = retError,
				LeftTicks = leftTicks,
				PackTicks = packTicks
			};
			if (tcpPacketData)
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, pool, nID);
			}
			return goodsPackListData;
		}

		private GoodsPackItem GetLockedGoodsPackItem(GameClient client, int autoID)
		{
			GoodsPackItem goodsPackItem = null;
			GoodsPackItem result;
			if (!this._GoodsPackDict.TryGetValue(autoID, out goodsPackItem))
			{
				result = null;
			}
			else if (goodsPackItem.LockedRoleID != client.ClientData.RoleID)
			{
				result = null;
			}
			else
			{
				result = goodsPackItem;
			}
			return result;
		}

		public void ProcessGetThing(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID, int goodsDbID, out bool bRet)
		{
			bRet = true;
			List<GoodsData> list = null;
			GoodsPackItem goodsPackItem = null;
			lock (this._GoodsPackDict)
			{
				goodsPackItem = this.GetLockedGoodsPackItem(client, autoID);
				if (null == goodsPackItem)
				{
					return;
				}
				string killedMonsterName = goodsPackItem.KilledMonsterName;
				list = new List<GoodsData>();
				if (-1 == goodsDbID)
				{
					for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
					{
						if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsPackItem.GoodsDataList[i].Id))
						{
							list.Add(goodsPackItem.GoodsDataList[i]);
						}
					}
				}
				else if (!goodsPackItem.GoodsIDDict.ContainsKey(goodsDbID))
				{
					for (int i = 0; i < goodsPackItem.GoodsDataList.Count; i++)
					{
						if (goodsPackItem.GoodsDataList[i].Id == goodsDbID)
						{
							list.Add(goodsPackItem.GoodsDataList[i]);
							break;
						}
					}
				}
			}
			if (list != null && list.Count > 0)
			{
				GameClient gameClient = null;
				for (int i = 0; i < list.Count; i++)
				{
					int num = this.FindGoodsID2RoleID(goodsPackItem, list[i].Id);
					if (-1 == num)
					{
						gameClient = client;
					}
					else
					{
						gameClient = GameManager.ClientMgr.FindClient(num);
					}
					if (null != gameClient)
					{
						if (!GoodsPackManager.IsFallTongQianGoods(list[i].GoodsID))
						{
							if (!RebornEquip.IsRebornType(list[i].GoodsID))
							{
								if (!Global.CanAddGoods(gameClient, list[i].GoodsID, 1, list[i].Binding, "1900-01-01 12:00:00", true, false))
								{
									bRet = false;
									break;
								}
							}
							else if (!RebornEquip.CanAddGoodsToReborn(gameClient, list[i].GoodsID, 1, list[i].Binding, "1900-01-01 12:00:00", true, false))
							{
								bRet = false;
								break;
							}
						}
						lock (this._GoodsPackDict)
						{
							goodsPackItem.GoodsIDDict[list[i].Id] = true;
						}
						if (!GoodsPackManager.ProcessFallTongQian(pool, gameClient, list[i].GoodsID, list[i].GCount, goodsPackItem.FallLevel))
						{
							int num2 = Global.AddGoodsDBCommand_Hook(pool, gameClient, list[i].GoodsID, list[i].GCount, list[i].Quality, list[i].Props, list[i].Forge_level, list[i].Binding, 0, list[i].Jewellist, true, 1, "杀怪掉落后手动拾取", true, list[i].Endtime, list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, list[i].ExcellenceInfo, list[i].AppendPropLev, list[i].ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
							if (0 == num2)
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, Global.ModifyGoodsLogName(list[i]), "杀怪掉落后手动拾取", Global.GetMapName(client.ClientData.MapCode), "系统", "销毁", list[i].GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, list[i]);
							}
							if (MoYuLongXue.InMoYuMap(goodsPackItem.MapCode))
							{
								if (MoYuLongXue.IsBHGoods(list[i].GoodsID))
								{
									string text;
									if (client.ClientData.Faction > 0)
									{
										text = string.Format(GLang.GetLang(4007, new object[0]), new object[]
										{
											client.ClientData.BHName,
											client.ClientData.RoleName,
											goodsPackItem.KilledMonsterName,
											Global.GetGoodsName(list[i])
										});
									}
									else
									{
										text = string.Format(GLang.GetLang(4008, new object[0]), client.ClientData.RoleName, goodsPackItem.KilledMonsterName, Global.GetGoodsName(list[i]));
									}
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, text, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
								}
							}
							if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(goodsPackItem.MapCode))
							{
								if (ZhuanShengShiLian.IsShiLianGoods(list[i].GoodsID))
								{
									string text = string.Format(GLang.GetLang(4012, new object[0]), client.ClientData.RoleName, Global.GetGoodsName(list[i]));
									ZhuanShengShiLian.BroadMsg(goodsPackItem.MapCode, text);
								}
							}
							if (ThemeBoss.getInstance().IsThemeBossScene(goodsPackItem.MapCode))
							{
								if (ThemeBoss.getInstance().IsThemeBossGoods(list[i].GoodsID))
								{
									string text = string.Format(GLang.GetLang(4015, new object[0]), Global.FormatRoleNameWithZoneId(client), Global.GetGoodsName(list[i]));
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, text, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
								}
							}
						}
						GameManager.ClientMgr.NotifySelfGetThing(sl, pool, gameClient, goodsDbID);
						SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PickUpEquipCount);
						sevenDayGoalEventObject.Arg1 = list[i].GoodsID;
						sevenDayGoalEventObject.Arg2 = list[i].GCount;
						GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
					}
				}
				bool flag3 = false;
				lock (this._GoodsPackDict)
				{
					flag3 = (goodsPackItem.GoodsIDDict.Count >= goodsPackItem.GoodsDataList.Count);
				}
				if (flag3)
				{
					lock (this._GoodsPackDict)
					{
						this._GoodsPackDict.Remove(autoID);
					}
					GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
					List<object> all9Clients = Global.GetAll9Clients(goodsPackItem);
					GameManager.ClientMgr.NotifyOthersDelGoodsPack(sl, pool, all9Clients, client.ClientData.MapCode, autoID, client.ClientData.RoleID);
				}
			}
		}

		public void ExternalRemoveGoodsPack(GoodsPackItem goodsPackItem)
		{
			GameManager.MapGridMgr.DictGrids[goodsPackItem.MapCode].RemoveObject(goodsPackItem);
			List<object> all9Clients = Global.GetAll9Clients(goodsPackItem);
			GameManager.ClientMgr.NotifyOthersDelGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, all9Clients, goodsPackItem.MapCode, goodsPackItem.AutoID, -1);
		}

		public List<FallGoodsItem> GetFallGoodsItemList(int goodsPackID)
		{
			List<FallGoodsItem> normalFallGoodsItem = this.GetNormalFallGoodsItem(goodsPackID);
			List<FallGoodsItem> result;
			if (null == normalFallGoodsItem)
			{
				result = null;
			}
			else
			{
				result = normalFallGoodsItem;
			}
			return result;
		}

		public List<FallGoodsItem> GetRandomFallGoodsItemList(int goodsPackID, int maxFallCount, bool isGood)
		{
			List<FallGoodsItem> normalFallGoodsItem = this.GetNormalFallGoodsItem(goodsPackID);
			List<FallGoodsItem> result;
			if (null == normalFallGoodsItem)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> list = Global.RandomSortList<FallGoodsItem>(normalFallGoodsItem);
				if (maxFallCount > 0)
				{
					while (list.Count > maxFallCount)
					{
						list.RemoveAt(list.Count - 1);
					}
				}
				for (int i = 0; i < normalFallGoodsItem.Count; i++)
				{
					normalFallGoodsItem[i].IsGood = isGood;
				}
				result = list;
			}
			return result;
		}

		public List<GoodsData> GetGoodsDataListFromFallGoodsItemList(List<FallGoodsItem> fallGoodsItemList)
		{
			List<GoodsData> result;
			if (fallGoodsItemList == null || fallGoodsItemList.Count <= 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				for (int i = 0; i < fallGoodsItemList.Count; i++)
				{
					int quality = 0;
					int fallGoodsLevel = this.GetFallGoodsLevel(fallGoodsItemList[i].FallLevelID);
					int bornIndex = 0;
					int luckyGoodsID = this.GetLuckyGoodsID(fallGoodsItemList[i].LuckyRate);
					int zhuiJiaGoodsLevelID = this.GetZhuiJiaGoodsLevelID(fallGoodsItemList[i].ZhuiJiaID);
					int excellencePropertysID = this.GetExcellencePropertysID(fallGoodsItemList[i].GoodsID, fallGoodsItemList[i].ExcellencePropertyID);
					string props = "";
					GoodsData item = new GoodsData
					{
						Id = i,
						GoodsID = fallGoodsItemList[i].GoodsID,
						Using = 0,
						Forge_level = fallGoodsLevel,
						Starttime = "1900-01-01 12:00:00",
						Endtime = "1900-01-01 12:00:00",
						Site = 0,
						Quality = quality,
						Props = props,
						GCount = 1,
						Binding = fallGoodsItemList[i].Binding,
						Jewellist = "",
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = bornIndex,
						Lucky = luckyGoodsID,
						Strong = 0,
						ExcellenceInfo = excellencePropertysID,
						AppendPropLev = zhuiJiaGoodsLevelID,
						ChangeLifeLevForEquip = 0
					};
					list.Add(item);
				}
				result = list;
			}
			return result;
		}

		private GoodsPackItem FindGoodsPackItemByPos(Point grid, GameClient gameClient)
		{
			MapGrid mapGrid = null;
			GoodsPackItem result;
			if (!GameManager.MapGridMgr.DictGrids.TryGetValue(gameClient.ClientData.MapCode, out mapGrid))
			{
				result = null;
			}
			else if (null == mapGrid)
			{
				result = null;
			}
			else
			{
				List<object> list = mapGrid.FindObjects((int)grid.X, (int)grid.Y);
				if (null != list)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] is GoodsPackItem)
						{
							if (gameClient.ClientData.CopyMapID <= 0)
							{
								return list[i] as GoodsPackItem;
							}
							if ((list[i] as GoodsPackItem).CopyMapID == gameClient.ClientData.CopyMapID)
							{
								return list[i] as GoodsPackItem;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		public void ProcessClickGoodsPackWhenMovingEnd(GameClient client)
		{
			GoodsPackItem goodsPackItem = this.FindGoodsPackItemByPos(client.CurrentGrid, client);
			if (null != goodsPackItem)
			{
				TCPOutPacket tcpoutPacket = null;
				try
				{
					GoodsPackListData goodsPackListData = GameManager.GoodsPackMgr.ProcessClickOnGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, out tcpoutPacket, 147, 1, false);
					if (null != goodsPackListData)
					{
						if (0 == goodsPackListData.RetError)
						{
							bool flag = true;
							GameManager.GoodsPackMgr.ProcessGetThing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, -1, out flag);
							this.UnLockGoodsPackItem(client);
							if (flag)
							{
								this.TakeFallGoodsRecords(goodsPackItem, client);
								goodsPackListData.GoodsDataList = null;
								TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
								if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
						else if (goodsPackListData.RetError == -1)
						{
							GameManager.GoodsPackMgr.ExternalRemoveGoodsPack(goodsPackItem);
						}
						else
						{
							goodsPackListData.GoodsDataList = null;
							TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
							if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
				finally
				{
				}
			}
		}

		private void InitShiQuGoodsList()
		{
			lock (this._CacheShiQuGoodsDict)
			{
				if (this._CacheShiQuGoodsDict.Count <= 0)
				{
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ShiQuGoodsList");
					if (!string.IsNullOrEmpty(paramValueByName))
					{
						string[] array = paramValueByName.Split(new char[]
						{
							'|'
						});
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
							for (int j = 0; j < array2.Length; j++)
							{
								dictionary[Global.SafeConvertToInt32(array2[j])] = true;
							}
							this._CacheShiQuGoodsDict[i] = dictionary;
						}
					}
				}
			}
		}

		private int GetPickUpShiQuGoodsType(int goodsID)
		{
			lock (this._CacheShiQuGoodsDict)
			{
				foreach (int num in this._CacheShiQuGoodsDict.Keys)
				{
					Dictionary<int, bool> dictionary = this._CacheShiQuGoodsDict[num];
					if (dictionary.ContainsKey(goodsID))
					{
						return num;
					}
				}
			}
			return -1;
		}

		private bool CanPickUpGoodss(GameClient client, GoodsPackItem goodsPackItem)
		{
			bool result;
			if (client.ClientData.AutoFightGetThings == 0)
			{
				result = false;
			}
			else if (goodsPackItem.GoodsDataList.Count <= 0)
			{
				result = false;
			}
			else
			{
				this.InitShiQuGoodsList();
				GoodsData goodsData = goodsPackItem.GoodsDataList[0];
				if (goodsPackItem.OnlyID > 0 && goodsPackItem.OnlyID != client.ClientData.RoleID)
				{
					result = false;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) || null == systemXmlItem)
					{
						LogManager.WriteLog(1, string.Format("处理拾取物品到背包时，获取物品xml信息失败: GoodsID={0}", goodsData.GoodsID), null, true);
						result = false;
					}
					else if (!GoodsPackManager.IsFallTongQianGoods(goodsData.GoodsID) && !Global.CanAddGoods(client, goodsData.GoodsID, goodsData.GCount, goodsData.Binding, "1900-01-01 12:00:00", true, false))
					{
						result = false;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
						if (intValue >= 0 && intValue < 49)
						{
							int equipColor = Global.GetEquipColor(goodsData);
							int intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, equipColor - 1);
							result = (1 == intSomeBit);
						}
						else
						{
							int pickUpShiQuGoodsType = this.GetPickUpShiQuGoodsType(goodsData.GoodsID);
							int intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 24);
							if (1 == intSomeBit)
							{
								if (0 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 25);
							if (1 == intSomeBit)
							{
								if (1 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 26);
							if (1 == intSomeBit)
							{
								if (2 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 27);
							if (1 == intSomeBit)
							{
								if (3 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 28);
							if (1 == intSomeBit)
							{
								if (4 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							intSomeBit = Global.GetIntSomeBit(client.ClientData.AutoFightGetThings, 29);
							if (1 == intSomeBit)
							{
								if (-1 == pickUpShiQuGoodsType)
								{
									return true;
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		private List<GoodsPackItem> FindGoodsPackItemListByPos(Point grid, int girdNum, GameClient gameClient)
		{
			MapGrid mapGrid = null;
			List<GoodsPackItem> result;
			if (!GameManager.MapGridMgr.DictGrids.TryGetValue(gameClient.ClientData.MapCode, out mapGrid))
			{
				result = null;
			}
			else if (null == mapGrid)
			{
				result = null;
			}
			else
			{
				int num = (int)grid.X - girdNum;
				int num2 = (int)grid.X + girdNum;
				int num3 = (int)grid.Y - girdNum;
				int num4 = (int)grid.Y + girdNum;
				num = Global.GMax(num, 0);
				num3 = Global.GMax(num3, 0);
				num2 = Global.GMin(num2, mapGrid.MapGridXNum - 1);
				num4 = Global.GMin(num4, mapGrid.MapGridYNum - 1);
				List<GoodsPackItem> list = new List<GoodsPackItem>();
				for (int i = num; i <= num2; i++)
				{
					for (int j = num3; j <= num4; j++)
					{
						List<object> list2 = mapGrid.FindGoodsPackItems(i, j);
						if (null != list2)
						{
							int k = 0;
							while (k < list2.Count)
							{
								if (list2[k] is GoodsPackItem)
								{
									if (this.CanOpenGoodsPack(list2[k] as GoodsPackItem, gameClient.ClientData.RoleID))
									{
										if (this.CanPickUpGoodss(gameClient, list2[k] as GoodsPackItem))
										{
											if (gameClient.ClientData.CopyMapID > 0)
											{
												if ((list2[k] as GoodsPackItem).CopyMapID == gameClient.ClientData.CopyMapID)
												{
													list.Add(list2[k] as GoodsPackItem);
												}
											}
											else
											{
												list.Add(list2[k] as GoodsPackItem);
											}
										}
									}
								}
								IL_1BB:
								k++;
								continue;
								goto IL_1BB;
							}
						}
					}
				}
				result = list;
			}
			return result;
		}

		public void ProcessClickGoodsPackWhenMovingToOtherGrid(GameClient client, int gridNum = 1)
		{
			List<GoodsPackItem> list = this.FindGoodsPackItemListByPos(client.CurrentGrid, gridNum, client);
			if (list != null && list.Count > 0)
			{
				lock (client.ClientData.PickUpGoodsPackMutex)
				{
					for (int i = 0; i < list.Count; i++)
					{
						GoodsPackItem goodsPackItem = list[i];
						TCPOutPacket tcpoutPacket = null;
						try
						{
							GoodsPackListData goodsPackListData = GameManager.GoodsPackMgr.ProcessClickOnGoodsPack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, out tcpoutPacket, 147, 1, false);
							if (null != goodsPackListData)
							{
								if (0 == goodsPackListData.RetError)
								{
									bool flag2 = true;
									GameManager.GoodsPackMgr.ProcessGetThing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsPackItem.AutoID, -1, out flag2);
									this.UnLockGoodsPackItem(client);
									if (!flag2)
									{
										break;
									}
									this.TakeFallGoodsRecords(goodsPackItem, client);
									goodsPackListData.GoodsDataList = null;
									TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
									if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
									{
									}
								}
								else if (goodsPackListData.RetError == -1)
								{
									GameManager.GoodsPackMgr.ExternalRemoveGoodsPack(goodsPackItem);
								}
								else
								{
									goodsPackListData.GoodsDataList = null;
									TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GoodsPackListData>(goodsPackListData, Global._TCPManager.TcpOutPacketPool, 147);
									if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
									{
									}
								}
							}
						}
						finally
						{
						}
					}
				}
			}
		}

		private void WriteFallGoodsRecords(GoodsPackItem goodsPackItem)
		{
			GoodsData goodsData = goodsPackItem.GoodsDataList[0];
			SystemXmlItem systemXmlItem = Global.CanBroadcastOrEventGoods(goodsData.GoodsID);
			if (null != systemXmlItem)
			{
				GameManager.DBCmdMgr.AddDBCmd(10118, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}", new object[]
				{
					goodsPackItem.OwnerRoleID,
					goodsPackItem.AutoID,
					-1,
					goodsData.GoodsID,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.Quality,
					goodsData.Forge_level,
					goodsData.Jewellist,
					Global.GetMapName(goodsPackItem.MapCode),
					goodsPackItem.CurrentGrid.ToString(),
					goodsPackItem.KilledMonsterName
				}), null, 0);
				string resList = EventLogManager.NewGoodsDataPropString(goodsData);
				EventLogManager.AddFallGoodsEvent(goodsPackItem.MapCode, goodsPackItem.OwnerRoleID, goodsPackItem.KilledMonsterName, resList);
			}
		}

		private void TakeFallGoodsRecords(GoodsPackItem goodsPackItem, GameClient client)
		{
			GoodsData goodsData = goodsPackItem.GoodsDataList[0];
			SystemXmlItem systemXmlItem = Global.CanBroadcastOrEventGoods(goodsData.GoodsID);
			if (null != systemXmlItem)
			{
				GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "拾取物品", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, goodsData);
				GameManager.DBCmdMgr.AddDBCmd(10118, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}", new object[]
				{
					client.ClientData.RoleID,
					goodsPackItem.AutoID,
					1,
					goodsData.GoodsID,
					goodsData.GCount,
					goodsData.Binding,
					goodsData.Quality,
					goodsData.Forge_level,
					goodsData.Jewellist,
					Global.GetMapName(goodsPackItem.MapCode),
					goodsPackItem.CurrentGrid.ToString(),
					goodsPackItem.KilledMonsterName
				}), null, client.ServerId);
			}
		}

		private long BaseAutoID = 0L;

		private long BaseGoodsID = 0L;

		private long BaseRoleGoodsPackID = 0L;

		private int GlobalFallGoodsLimitDayID;

		private Dictionary<int, int> _GlobalFallGoodsLimitDict = new Dictionary<int, int>();

		private Dictionary<int, int> GlobalFallGoodsNumDict = new Dictionary<int, int>();

		private Dictionary<int, List<FallGoodsItem>> _FallGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		private Dictionary<int, List<FallGoodsItem>> _LimitTimeFallGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		private DateTime _LimitTimeStartDayTime = new DateTime(2000, 1, 1);

		private DateTime _LimitTimeEndDayTime = new DateTime(2000, 1, 1);

		private Dictionary<int, List<FallGoodsItem>> _FixedGoodsItemsDict = new Dictionary<int, List<FallGoodsItem>>();

		private Dictionary<int, Tuple<int, int>> _FallGoodsMaxCountDict = new Dictionary<int, Tuple<int, int>>();

		private Dictionary<int, int> _LimitTimeFallGoodsMaxCountDict = new Dictionary<int, int>();

		private Dictionary<int, FallQualityItem> _FallGoodsQualityDict = new Dictionary<int, FallQualityItem>();

		private Dictionary<int, FallLevelItem> _FallGoodsLevelDict = new Dictionary<int, FallLevelItem>();

		private Dictionary<int, FallBornIndexItem> _FallGoodsBornIndexDict = new Dictionary<int, FallBornIndexItem>();

		private Dictionary<int, ZhuiJiaIDItem> _ZhuiJiaIDDict = new Dictionary<int, ZhuiJiaIDItem>();

		private Dictionary<int, ExcellencePropertyGroupItem> _ExcellencePropertyGroupItemDict = new Dictionary<int, ExcellencePropertyGroupItem>();

		public static int TeamShareMode_Flags = 1;

		public static bool TeamShareMode_MaxDamage = false;

		public static bool TeamShareMode_RandomByDamage = false;

		public static int MaxFallCount = 10000;

		private Dictionary<int, GoodsPackItem> _GoodsPackDict = new Dictionary<int, GoodsPackItem>();

		private HashSet<int> ProhibitRollHashSet;

		private Dictionary<int, Dictionary<int, bool>> _CacheShiQuGoodsDict = new Dictionary<int, Dictionary<int, bool>>();
	}
}
