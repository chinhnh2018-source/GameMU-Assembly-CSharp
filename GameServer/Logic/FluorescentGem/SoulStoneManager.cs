using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.FluorescentGem
{
	public class SoulStoneManager : SingletonTemplate<SoulStoneManager>, IManager, ICmdProcessorEx, ICmdProcessor
	{
		private SoulStoneManager()
		{
			this.JingHuaCategorys.Add(910);
			this.EquipCategorys.Add(911);
			this.EquipCategorys.Add(912);
			this.EquipCategorys.Add(913);
			this.EquipCategorys.Add(914);
			this.EquipCategorys.Add(915);
			this.EquipCategorys.Add(916);
			this.EquipCategorys.Add(917);
			this.EquipCategorys.Add(918);
			this.EquipCategorys.Add(919);
			this.EquipCategorys.Add(920);
			this.EquipCategorys.Add(921);
			this.EquipCategorys.Add(922);
			this.EquipCategorys.Add(923);
			this.EquipCategorys.Add(924);
			this.EquipCategorys.Add(925);
			this.EquipCategorys.Add(926);
			this.EquipCategorys.Add(927);
			this.EquipCategorys.Add(928);
		}

		public bool initialize()
		{
			bool result;
			if (!this.LoadRandType() || !this.LoadRandInfo() || !this.LoadExp() || !this.LoadStoneType() || !this.LoadStoneGroup())
			{
				LogManager.WriteLog(2, "SoulStoneManager.initialize failed!", null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1321, 3, 3, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1322, 4, 4, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1323, 3, 3, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1324, 1, 1, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1320, 1, 1, SingletonTemplate<SoulStoneManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(8))
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1320:
					result = this.ProcessSoulStoneQueryGet(client, nID, bytes, cmdParams);
					break;
				case 1321:
					result = this.ProcessSoulStoneGet(client, nID, bytes, cmdParams);
					break;
				case 1322:
					result = this.ProcessSoulStoneLevelUp(client, nID, bytes, cmdParams);
					break;
				case 1323:
					result = this.ProcessSoulStoneModEquip(client, nID, bytes, cmdParams);
					break;
				case 1324:
					result = this.ProcessSoulStoneResetBag(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public void LoadJingHuaExpConfig()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("HunShiExp", '|');
			if (paramValueStringListByName != null)
			{
				foreach (string text in paramValueStringListByName)
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						dictionary[Convert.ToInt32(array[0])] = Convert.ToInt32(array[1]);
					}
				}
			}
			List<string> paramValueStringListByName2 = GameManager.systemParamsList.GetParamValueStringListByName("HunShiOpen", '|');
			if (paramValueStringListByName2 != null)
			{
				for (int i = 0; i < paramValueStringListByName2.Count; i++)
				{
					string[] array = paramValueStringListByName2[i].Split(new char[]
					{
						','
					});
					if (array.Length == 2)
					{
						int zhuanSheng = Convert.ToInt32(array[0]);
						int level = Convert.ToInt32(array[1]);
						dictionary2[i + 1] = Global.GetUnionLevel(zhuanSheng, level, false);
					}
				}
			}
			this.jinghuaExpDict = dictionary;
			this.equipLvlLimitDict = dictionary2;
		}

		private bool LoadRandType()
		{
			try
			{
				this.defaultRandId = int.MaxValue;
				XElement xelement = XElement.Load(Global.GameResPath("Config/Gem/HunShiType.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					SoulStoneRandConfig soulStoneRandConfig = new SoulStoneRandConfig();
					soulStoneRandConfig.RandId = (int)Global.GetSafeAttributeLong(xml, "ID");
					soulStoneRandConfig.NeedLangHunFenMo = (int)Global.GetSafeAttributeLong(xml, "NeedLangHunFenMo");
					soulStoneRandConfig.SuccessRate = Global.GetSafeAttributeDouble(xml, "SuccessRate");
					long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "SuccessTo", -1);
					Debug.Assert(safeAttributeLongArray != null);
					for (int i = 0; i < safeAttributeLongArray.Length; i++)
					{
						soulStoneRandConfig.SuccessTo.Add((int)safeAttributeLongArray[i]);
					}
					safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "FailTo", -1);
					Debug.Assert(safeAttributeLongArray != null);
					for (int i = 0; i < safeAttributeLongArray.Length; i++)
					{
						soulStoneRandConfig.FailTo.Add((int)safeAttributeLongArray[i]);
					}
					string[] array = Global.GetSafeAttributeStr(xml, "AddedGoodsNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(array != null && array.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						soulStoneRandConfig.AddedNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(array[i]);
					}
					soulStoneRandConfig.AddedRate = Global.GetSafeAttributeDouble(xml, "AddedGoodsOdds");
					soulStoneRandConfig.AddedGoods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "AddedGoods").Split(new char[]
					{
						','
					}), 0);
					array = Global.GetSafeAttributeStr(xml, "ReduceNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(array != null && array.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						soulStoneRandConfig.ReduceNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(array[i]);
					}
					soulStoneRandConfig.ReduceRate = Global.GetSafeAttributeDouble(xml, "ReduceOdds");
					soulStoneRandConfig.ReduceValue = (int)Global.GetSafeAttributeLong(xml, "ReduceNum");
					array = Global.GetSafeAttributeStr(xml, "AdvanceSuccessNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(array != null && array.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						soulStoneRandConfig.UpSucRateNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(array[i]);
					}
					soulStoneRandConfig.UpSucRateTo = Global.GetSafeAttributeDouble(xml, "AdvanceSuccessRate");
					array = Global.GetSafeAttributeStr(xml, "HoldTypeNeed").Split(new char[]
					{
						'|'
					});
					Debug.Assert(array != null && array.Length == 5);
					for (int i = 0; i < 5; i++)
					{
						soulStoneRandConfig.FailHoldNeedDict[i + ESoulStoneExtCostType.MoJing] = Convert.ToInt32(array[i]);
					}
					safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "HoldTypeFailTo", -1);
					Debug.Assert(safeAttributeLongArray != null);
					for (int i = 0; i < safeAttributeLongArray.Length; i++)
					{
						soulStoneRandConfig.FailToIfHold.Add((int)safeAttributeLongArray[i]);
					}
					this.randDict.Add(soulStoneRandConfig.RandId, soulStoneRandConfig);
					this.defaultRandId = Math.Min(this.defaultRandId, soulStoneRandConfig.RandId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "load config file Config/Gem/HunShiType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		private bool LoadRandInfo()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/Gem/HunShi.xml"));
				foreach (XElement xelement2 in xelement.Elements())
				{
					SoulStoneRandConfig soulStoneRandConfig = null;
					int num = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					if (!this.randDict.TryGetValue(num, out soulStoneRandConfig))
					{
						throw new Exception("can't find typeid=" + num + ", please check Config/Gem/HunShiType.xml");
					}
					soulStoneRandConfig.RandMinNumber = int.MaxValue;
					soulStoneRandConfig.RandMaxNumber = int.MinValue;
					foreach (XElement xml in xelement2.Elements())
					{
						SoulStoneRandInfo soulStoneRandInfo = new SoulStoneRandInfo();
						soulStoneRandInfo.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						soulStoneRandInfo.Goods = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "Goods").Split(new char[]
						{
							','
						}), 0);
						soulStoneRandInfo.RandBegin = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						soulStoneRandInfo.RandEnd = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						soulStoneRandConfig.RandMinNumber = Math.Min(soulStoneRandConfig.RandMinNumber, soulStoneRandInfo.RandBegin);
						soulStoneRandConfig.RandMaxNumber = Math.Max(soulStoneRandConfig.RandMaxNumber, soulStoneRandInfo.RandEnd);
						soulStoneRandConfig.RandStoneList.Add(soulStoneRandInfo);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "load config file Config/Gem/HunShiType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		private bool LoadExp()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/Gem/HunShiExp.xml"));
				foreach (XElement xelement2 in xelement.Elements())
				{
					SoulStoneExpConfig soulStoneExpConfig = new SoulStoneExpConfig();
					soulStoneExpConfig.Suit = (int)Global.GetSafeAttributeLong(xelement2, "SuitID");
					soulStoneExpConfig.MinLevel = int.MaxValue;
					soulStoneExpConfig.MaxLevel = int.MinValue;
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					foreach (XElement xml in xelement2.Elements())
					{
						int i = (int)Global.GetSafeAttributeLong(xml, "ID");
						int value = (int)Global.GetSafeAttributeLong(xml, "Exp");
						soulStoneExpConfig.MinLevel = Math.Min(soulStoneExpConfig.MinLevel, i);
						soulStoneExpConfig.MaxLevel = Math.Max(soulStoneExpConfig.MaxLevel, i);
						dictionary.Add(i, value);
					}
					int num = 0;
					int num2 = 0;
					for (int i = soulStoneExpConfig.MinLevel; i <= soulStoneExpConfig.MaxLevel; i++)
					{
						if (!dictionary.TryGetValue(i, out num2))
						{
							num2 = 0;
						}
						soulStoneExpConfig.Lvl2Exp.Add(i, num2 + num);
						num += num2;
					}
					this.suitExpDict.Add(soulStoneExpConfig.Suit, soulStoneExpConfig);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "load config file Config/Gem/HunShiExp.xml failed", ex, true);
				return false;
			}
			return true;
		}

		private bool LoadStoneType()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/Gem/HunShiGoodsType.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					int value = (int)Global.GetSafeAttributeLong(xml, "ID");
					long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "Goods", -1);
					for (int i = 0; i < safeAttributeLongArray.Length; i++)
					{
						this.stone2TypeDict.Add((int)safeAttributeLongArray[i], value);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "load config file Config/Gem/HunShiGoodsType.xml failed", ex, true);
				return false;
			}
			return true;
		}

		private bool LoadStoneGroup()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/Gem/HunShiGroup.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					SoulStoneGroupConfig soulStoneGroupConfig = new SoulStoneGroupConfig();
					soulStoneGroupConfig.Group = (int)Global.GetSafeAttributeLong(xml, "ID");
					long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "HunShiGoodsType", -1);
					for (int i = 0; i < safeAttributeLongArray.Length; i++)
					{
						soulStoneGroupConfig.NeedTypeList.Add((int)safeAttributeLongArray[i]);
					}
					string[] array = Global.GetSafeAttributeStr(xml, "GroupProperty").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						ExtPropIndexes key;
						if (Enum.TryParse<ExtPropIndexes>(array2[0], out key))
						{
							double value = Convert.ToDouble(array2[1]);
							soulStoneGroupConfig.AttrValue.Add((int)key, value);
						}
						else
						{
							LogManager.WriteLog(2, string.Concat(new string[]
							{
								"can't parse ",
								soulStoneGroupConfig.Group.ToString(),
								" ",
								array2[0],
								" as ExtPropIndexes"
							}), null, true);
						}
					}
					this.groupDict.Add(soulStoneGroupConfig.Group, soulStoneGroupConfig);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "load config file Config/Gem/HunShiGroup.xml failed", ex, true);
				return false;
			}
			return true;
		}

		private List<SoulStoneExtFuncItem> GetExtFuncItems()
		{
			JieRiFuLiActivity jieriFuLiActivity = HuodongCachingMgr.GetJieriFuLiActivity();
			List<SoulStoneExtFuncItem> result;
			if (jieriFuLiActivity == null)
			{
				result = null;
			}
			else
			{
				object obj = null;
				if (!jieriFuLiActivity.IsOpened(EJieRiFuLiType.SoulStoneExtFunc, out obj))
				{
					result = null;
				}
				else if (obj == null)
				{
					result = null;
				}
				else
				{
					List<Tuple<int, int>> list = obj as List<Tuple<int, int>>;
					if (list == null || list.Count <= 0)
					{
						result = null;
					}
					else
					{
						List<SoulStoneExtFuncItem> list2 = new List<SoulStoneExtFuncItem>();
						for (int i = 0; i < list.Count; i++)
						{
							list2.Add(new SoulStoneExtFuncItem
							{
								FuncType = list[i].Item1,
								CostType = list[i].Item2
							});
						}
						result = list2;
					}
				}
			}
			return result;
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return client != null && GlobalNew.IsGongNengOpened(client, 73, false);
		}

		private int GenerateBagIndex(int cycle, int grid)
		{
			return cycle * 100 + grid;
		}

		private void ParseCycleAndGrid(int bagIndex, out int cycle, out int grid)
		{
			cycle = bagIndex / 100;
			grid = bagIndex % 100;
		}

		public void CheckOpen(GameClient client)
		{
			if (client != null)
			{
				if (!client.ClientData.IsSoulStoneOpened)
				{
					if (this.IsGongNengOpened(client))
					{
						client.ClientData.IsSoulStoneOpened = true;
						string roleParamByName = Global.GetRoleParamByName(client, "SoulStoneRandId");
						int key = 0;
						if (string.IsNullOrEmpty(roleParamByName) || !int.TryParse(roleParamByName, out key) || !this.randDict.ContainsKey(key))
						{
							Global.SaveRoleParamsInt32ValueToDB(client, "SoulStoneRandId", this.defaultRandId, true);
						}
						this.ResetSoulStoneBag(client);
						this.UpdateProps(client);
					}
				}
			}
		}

		public bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.SoulStoneInBag.Count <= 100;
		}

		public int GetIdleSlotOfBag(GameClient client)
		{
			byte[] array = new byte[100];
			for (int i = 0; i < client.ClientData.SoulStoneInBag.Count; i++)
			{
				int bagIndex = client.ClientData.SoulStoneInBag[i].BagIndex;
				if (bagIndex >= 0 && bagIndex < 100)
				{
					array[bagIndex] = 1;
				}
			}
			for (int i = 0; i < 100; i++)
			{
				if (array[i] == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private void AddSoulStoneGoods(GameClient client, GoodsData gd, int site)
		{
			if (client != null && gd != null)
			{
				gd.Site = site;
				if (site == 8000)
				{
					client.ClientData.SoulStoneInBag.Add(gd);
				}
				else if (site == 8001)
				{
					client.ClientData.SoulStoneInUsing.Add(gd);
				}
			}
		}

		public GoodsData AddSoulStoneGoods(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			this.AddSoulStoneGoods(client, goodsData, goodsData.Site);
			return goodsData;
		}

		public void RemoveSoulStoneGoods(GameClient client, GoodsData goodsData, int site)
		{
			if (goodsData != null && client != null)
			{
				if (goodsData.Site == 8000)
				{
					client.ClientData.SoulStoneInBag.Remove(goodsData);
				}
				else if (goodsData.Site == 8001)
				{
					client.ClientData.SoulStoneInUsing.Remove(goodsData);
				}
			}
		}

		private GoodsData GetSoulStoneByDbId(GameClient client, int site, int dbid)
		{
			GoodsData result;
			if (site == 8000)
			{
				result = client.ClientData.SoulStoneInBag.Find((GoodsData _g) => _g.Id == dbid);
			}
			else if (site == 8001)
			{
				result = client.ClientData.SoulStoneInUsing.Find((GoodsData _g) => _g.Id == dbid);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void UpdateProps(GameClient client)
		{
			if (client != null)
			{
				EquipPropItem equipPropItem = new EquipPropItem();
				List<int>[] eachCycleStones = new List<int>[4];
				for (int i = 1; i <= 3; i++)
				{
					eachCycleStones[i] = new List<int>();
				}
				foreach (GoodsData goodsData in client.ClientData.SoulStoneInUsing)
				{
					int num = 0;
					int num2 = 0;
					this.ParseCycleAndGrid(goodsData.BagIndex, out num, out num2);
					if (num >= 1 && num <= 3 && this.stone2TypeDict.ContainsKey(goodsData.GoodsID))
					{
						eachCycleStones[num].Add(this.stone2TypeDict[goodsData.GoodsID]);
					}
					int num3 = (goodsData.ElementhrtsProps != null) ? goodsData.ElementhrtsProps[0] : 1;
					EquipPropItem equipPropItem2 = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
					int i = 0;
					while (equipPropItem2 != null && i < equipPropItem2.ExtProps.Length)
					{
						equipPropItem.ExtProps[i] += equipPropItem2.ExtProps[i] * (double)num3;
						i++;
					}
				}
				foreach (KeyValuePair<int, SoulStoneGroupConfig> keyValuePair in this.groupDict)
				{
					SoulStoneGroupConfig value = keyValuePair.Value;
					if (value.NeedTypeList != null && value.NeedTypeList.Count > 0)
					{
						int cycle;
						for (cycle = 1; cycle <= 3; cycle++)
						{
							if (value.NeedTypeList.All((int _t) => eachCycleStones[cycle].Contains(_t)))
							{
								foreach (KeyValuePair<int, double> keyValuePair2 in value.AttrValue)
								{
									if (keyValuePair2.Key >= 0 && keyValuePair2.Key < equipPropItem.ExtProps.Length)
									{
										equipPropItem.ExtProps[keyValuePair2.Key] += keyValuePair2.Value;
									}
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.SoulStone,
					equipPropItem.ExtProps
				});
			}
		}

		private int ExtCostTypeHadValue(GameClient client, ESoulStoneExtCostType costType)
		{
			int result = 0;
			if (costType == ESoulStoneExtCostType.MoJing)
			{
				result = GameManager.ClientMgr.GetTianDiJingYuanValue(client);
			}
			else if (costType == ESoulStoneExtCostType.XingHun)
			{
				result = client.ClientData.StarSoul;
			}
			else if (costType == ESoulStoneExtCostType.ChengJiu)
			{
				result = GameManager.ClientMgr.GetChengJiuPointsValue(client);
			}
			else if (costType == ESoulStoneExtCostType.ShengWang)
			{
				result = GameManager.ClientMgr.GetShengWangValue(client);
			}
			else if (costType == ESoulStoneExtCostType.ZuanShi)
			{
				result = client.ClientData.UserMoney;
			}
			return result;
		}

		private bool DoExtCostType(GameClient client, ESoulStoneExtCostType costType, int val)
		{
			bool result;
			if (val <= 0)
			{
				result = true;
			}
			else
			{
				if (costType == ESoulStoneExtCostType.MoJing)
				{
					GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, -val, "聚魂额外消耗", true, true, false);
				}
				else if (costType == ESoulStoneExtCostType.XingHun)
				{
					GameManager.ClientMgr.ModifyStarSoulValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ChengJiu)
				{
					GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ShengWang)
				{
					GameManager.ClientMgr.ModifyShengWangValue(client, -val, "聚魂额外消耗", true, true);
				}
				else if (costType == ESoulStoneExtCostType.ZuanShi)
				{
					GameManager.ClientMgr.SubUserMoney(client, val, "聚魂额外消耗", true, true, true, true, DaiBiSySType.None);
				}
				result = true;
			}
			return result;
		}

		private bool ProcessSoulStoneQueryGet(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				client.sendCmd<SoulStoneQueryGetData>(nID, new SoulStoneQueryGetData
				{
					CurrRandId = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId"),
					ExtFuncList = this.GetExtFuncItems()
				}, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		private bool ProcessSoulStoneResetBag(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client))
				{
					return true;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				this.ResetSoulStoneBag(client);
				client.sendCmd<List<GoodsData>>(nID, client.ClientData.SoulStoneInBag, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		private void ResetSoulStoneBag(GameClient client)
		{
			if (client != null)
			{
				client.ClientData.SoulStoneInBag.Sort(delegate(GoodsData _left, GoodsData _right)
				{
					int equipGoodsSuitID = Global.GetEquipGoodsSuitID(_left.GoodsID);
					int equipGoodsSuitID2 = Global.GetEquipGoodsSuitID(_right.GoodsID);
					int result;
					if (equipGoodsSuitID > equipGoodsSuitID2)
					{
						result = -1;
					}
					else if (equipGoodsSuitID < equipGoodsSuitID2)
					{
						result = 1;
					}
					else
					{
						int num = 0;
						if (_left.ElementhrtsProps != null && _right.ElementhrtsProps != null)
						{
							num = _left.ElementhrtsProps[0] - _right.ElementhrtsProps[0];
						}
						if (num > 0)
						{
							result = -1;
						}
						else if (num < 0)
						{
							result = 1;
						}
						else
						{
							result = _left.GoodsID - _right.GoodsID;
						}
					}
					return result;
				});
				for (int i = 0; i < client.ClientData.SoulStoneInBag.Count; i++)
				{
					client.ClientData.SoulStoneInBag[i].BagIndex = i;
				}
			}
		}

		private bool ProcessSoulStoneModEquip(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				ESoulStoneErrorCode esoulStoneErrorCode = this.handleModEquip(client, num2, num3);
				string cmdData = string.Format("{0}:{1}:{2}", (int)esoulStoneErrorCode, num2, num3);
				client.sendCmd(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		private ESoulStoneErrorCode handleModEquip(GameClient client, int bagIndex, int newDbId)
		{
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else
			{
				int cycle = 0;
				int grid = 0;
				this.ParseCycleAndGrid(bagIndex, out cycle, out grid);
				if (cycle < 1 || cycle > 3 || grid < 1 || grid > 6)
				{
					result = ESoulStoneErrorCode.VisitParamsError;
				}
				else
				{
					GoodsData newGd = null;
					if (newDbId != -1)
					{
						newGd = client.ClientData.SoulStoneInBag.Find((GoodsData _g) => _g.Id == newDbId);
						if (newGd == null)
						{
							return ESoulStoneErrorCode.VisitParamsError;
						}
						Dictionary<int, int> dictionary = this.equipLvlLimitDict;
						if (dictionary == null)
						{
							return ESoulStoneErrorCode.ConfigError;
						}
						if (!dictionary.ContainsKey(cycle) || dictionary[cycle] > Global.GetUnionLevel(client, false))
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(newGd.GoodsID, out systemXmlItem))
						{
							return ESoulStoneErrorCode.ConfigError;
						}
						int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
						if (!this.EquipCategorys.Contains(intValue))
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
						GoodsData goodsData = client.ClientData.SoulStoneInUsing.Find(delegate(GoodsData _g)
						{
							int num3 = 0;
							int num4 = 0;
							this.ParseCycleAndGrid(_g.BagIndex, out num3, out num4);
							return num3 == cycle && num4 != grid && _g.GoodsID == newGd.GoodsID;
						});
						if (goodsData != null)
						{
							return ESoulStoneErrorCode.CanNotEquip;
						}
					}
					GoodsData goodsData2 = client.ClientData.SoulStoneInUsing.Find((GoodsData _g) => _g.BagIndex == bagIndex);
					if (goodsData2 != null)
					{
						if (!this.CanAddGoodsNum(client, 1))
						{
							return ESoulStoneErrorCode.BagNoSpace;
						}
						int num = this.GetIdleSlotOfBag(client);
						if (num < 0)
						{
							return ESoulStoneErrorCode.BagNoSpace;
						}
						int num2 = 8000;
						string[] array = null;
						string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							client.ClientData.RoleID,
							goodsData2.Id,
							"*",
							"*",
							"*",
							"*",
							num2,
							"*",
							"*",
							goodsData2.GCount,
							"*",
							num,
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
							"*"
						});
						TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
						if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED || array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
						{
							return ESoulStoneErrorCode.DbFailed;
						}
						this.RemoveSoulStoneGoods(client, goodsData2, goodsData2.Site);
						goodsData2.BagIndex = num;
						goodsData2.Site = num2;
						this.AddSoulStoneGoods(client, goodsData2, goodsData2.Site);
						GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1, goodsData2.Id, goodsData2.Using, goodsData2.Site, goodsData2.GCount, goodsData2.BagIndex, 1);
					}
					if (newGd != null)
					{
						int num = bagIndex;
						int num2 = 8001;
						string[] array = null;
						string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							client.ClientData.RoleID,
							newGd.Id,
							"*",
							"*",
							"*",
							"*",
							num2,
							"*",
							"*",
							newGd.GCount,
							"*",
							num,
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
							"*"
						});
						TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
						if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED && array.Length > 0 && Convert.ToInt32(array[1]) >= 0)
						{
							this.RemoveSoulStoneGoods(client, newGd, newGd.Site);
							newGd.BagIndex = num;
							newGd.Site = num2;
							this.AddSoulStoneGoods(client, newGd, num2);
							GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 4, newGd.Id, newGd.Using, newGd.Site, newGd.GCount, newGd.BagIndex, 1);
						}
						else if (goodsData2 == null)
						{
							return ESoulStoneErrorCode.DbFailed;
						}
					}
					this.UpdateProps(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					result = ESoulStoneErrorCode.Success;
				}
			}
			return result;
		}

		private bool ProcessSoulStoneLevelUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				int num3 = Convert.ToInt32(cmdParams[2]);
				string[] array = cmdParams[3].Split(new char[]
				{
					','
				});
				List<int> list = new List<int>();
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						list.Add(Convert.ToInt32(array[i]));
					}
				}
				list = list.Distinct<int>().ToList<int>();
				int num4;
				int num5;
				ESoulStoneErrorCode esoulStoneErrorCode = this.handleSoulStoneLevelUp(client, num2, num3, list, out num4, out num5);
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)esoulStoneErrorCode,
					num2,
					num3,
					num4,
					num5
				});
				client.sendCmd(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		private ESoulStoneErrorCode handleSoulStoneLevelUp(GameClient client, int target, int site, List<int> srcList, out int currLvl, out int currExp)
		{
			currLvl = 0;
			currExp = 0;
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else if (srcList == null || srcList.Count <= 0)
			{
				result = ESoulStoneErrorCode.VisitParamsError;
			}
			else if (srcList.IndexOf(target) >= 0)
			{
				result = ESoulStoneErrorCode.VisitParamsError;
			}
			else
			{
				GoodsData soulStoneByDbId = this.GetSoulStoneByDbId(client, site, target);
				if (soulStoneByDbId == null)
				{
					result = ESoulStoneErrorCode.VisitParamsError;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(soulStoneByDbId.GoodsID, out systemXmlItem))
					{
						result = ESoulStoneErrorCode.ConfigError;
					}
					else if (!this.EquipCategorys.Contains(systemXmlItem.GetIntValue("Categoriy", -1)))
					{
						result = ESoulStoneErrorCode.VisitParamsError;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("SuitID", -1);
						SoulStoneExpConfig soulStoneExpConfig = null;
						if (!this.suitExpDict.TryGetValue(intValue, out soulStoneExpConfig))
						{
							result = ESoulStoneErrorCode.ConfigError;
						}
						else if (soulStoneByDbId.ElementhrtsProps == null)
						{
							LogManager.WriteLog(2, string.Format("roleid={0}, dbid={1}的魂石等级和经验为null", client.ClientData.RoleID, target), null, true);
							result = ESoulStoneErrorCode.UnknownFailed;
						}
						else if (soulStoneByDbId.ElementhrtsProps[0] >= soulStoneExpConfig.MaxLevel)
						{
							result = ESoulStoneErrorCode.LevelIsFull;
						}
						else
						{
							int num = 0;
							foreach (int dbid in srcList)
							{
								GoodsData soulStoneByDbId2 = this.GetSoulStoneByDbId(client, 8000, dbid);
								if (soulStoneByDbId2 == null)
								{
									return ESoulStoneErrorCode.VisitParamsError;
								}
								SystemXmlItem systemXmlItem2 = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(soulStoneByDbId2.GoodsID, out systemXmlItem2))
								{
									return ESoulStoneErrorCode.ConfigError;
								}
								if (910 == systemXmlItem2.GetIntValue("Categoriy", -1))
								{
									Dictionary<int, int> dictionary = this.jinghuaExpDict;
									if (dictionary == null || !dictionary.ContainsKey(soulStoneByDbId2.GoodsID))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									num += dictionary[soulStoneByDbId2.GoodsID] * soulStoneByDbId2.GCount;
								}
								else
								{
									int intValue2 = systemXmlItem2.GetIntValue("SuitID", -1);
									SoulStoneExpConfig soulStoneExpConfig2 = null;
									if (!this.suitExpDict.TryGetValue(intValue2, out soulStoneExpConfig2))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									if (soulStoneByDbId2.ElementhrtsProps == null)
									{
										LogManager.WriteLog(2, string.Format("roleid={0}, dbid={1}的魂石等级和经验为null", client.ClientData.RoleID, soulStoneByDbId2.Id), null, true);
										return ESoulStoneErrorCode.UnknownFailed;
									}
									int num2;
									if (!soulStoneExpConfig2.Lvl2Exp.TryGetValue(soulStoneByDbId2.ElementhrtsProps[0], out num2))
									{
										return ESoulStoneErrorCode.ConfigError;
									}
									num += num2 * soulStoneByDbId2.GCount + soulStoneByDbId2.ElementhrtsProps[1] * soulStoneByDbId2.GCount;
								}
							}
							foreach (int dbid in srcList)
							{
								GoodsData soulStoneByDbId2 = this.GetSoulStoneByDbId(client, 8000, dbid);
								if (soulStoneByDbId2 != null)
								{
									if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, soulStoneByDbId2, soulStoneByDbId2.GCount, false, false))
									{
									}
								}
							}
							List<int> elementhrtsProps;
							(elementhrtsProps = soulStoneByDbId.ElementhrtsProps)[1] = elementhrtsProps[1] + num;
							while (soulStoneByDbId.ElementhrtsProps[0] < soulStoneExpConfig.MaxLevel)
							{
								int num3 = 0;
								int num4 = 0;
								if (!soulStoneExpConfig.Lvl2Exp.TryGetValue(soulStoneByDbId.ElementhrtsProps[0], out num3) || !soulStoneExpConfig.Lvl2Exp.TryGetValue(soulStoneByDbId.ElementhrtsProps[0] + 1, out num4))
								{
									break;
								}
								int num5 = num4 - num3;
								if (soulStoneByDbId.ElementhrtsProps[1] < num5)
								{
									break;
								}
								(elementhrtsProps = soulStoneByDbId.ElementhrtsProps)[0] = elementhrtsProps[0] + 1;
								(elementhrtsProps = soulStoneByDbId.ElementhrtsProps)[1] = elementhrtsProps[1] - num5;
							}
							UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
							{
								RoleID = client.ClientData.RoleID,
								DbID = target,
								WashProps = null
							};
							updateGoodsArgs.ElementhrtsProps = new List<int>();
							updateGoodsArgs.ElementhrtsProps.Add(soulStoneByDbId.ElementhrtsProps[0]);
							updateGoodsArgs.ElementhrtsProps.Add(soulStoneByDbId.ElementhrtsProps[1]);
							Global.UpdateGoodsProp(client, soulStoneByDbId, updateGoodsArgs, true);
							if (soulStoneByDbId.Site == 8001)
							{
								this.UpdateProps(client);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							}
							currLvl = soulStoneByDbId.ElementhrtsProps[0];
							currExp = soulStoneByDbId.ElementhrtsProps[1];
							result = ESoulStoneErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		private bool ProcessSoulStoneGet(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				string[] array = cmdParams[2].Split(new char[]
				{
					','
				});
				List<int> list = null;
				if (array.Length > 0)
				{
					list = new List<int>();
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							list.Add(Convert.ToInt32(array[i]));
						}
					}
					list = list.Distinct<int>().ToList<int>();
				}
				List<SoulStoneExtFuncItem> extFuncItems = this.GetExtFuncItems();
				SoulStoneGetData soulStoneGetData = new SoulStoneGetData();
				soulStoneGetData.RequestTimes = num2;
				soulStoneGetData.RealDoTimes = 0;
				if (num2 != 1 && num2 != 10)
				{
					soulStoneGetData.Error = 2;
				}
				else
				{
					soulStoneGetData.Stones = new List<int>();
					soulStoneGetData.ExtGoods = new List<int>();
					for (int j = 1; j <= num2; j++)
					{
						List<int> list2 = null;
						List<int> list3 = null;
						ESoulStoneErrorCode esoulStoneErrorCode = this.handleSoulStoneGetOne(client, list, extFuncItems, out list2, out list3, j);
						if (esoulStoneErrorCode != ESoulStoneErrorCode.Success)
						{
							if (soulStoneGetData.RealDoTimes == 0)
							{
								soulStoneGetData.Error = (int)esoulStoneErrorCode;
							}
							break;
						}
						soulStoneGetData.Error = 0;
						soulStoneGetData.RealDoTimes++;
						if (list2 != null)
						{
							soulStoneGetData.Stones.AddRange(list2);
						}
						if (list3 != null)
						{
							soulStoneGetData.ExtGoods.AddRange(list3);
						}
					}
				}
				soulStoneGetData.NewRandId = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId");
				client.sendCmd<SoulStoneGetData>(nID, soulStoneGetData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				return false;
			}
			return true;
		}

		private ESoulStoneErrorCode handleSoulStoneGetOne(GameClient client, List<int> selectExtFuncs, List<SoulStoneExtFuncItem> openedExtFuncs, out List<int> goodsIdList, out List<int> extGoodsList, int currTimes)
		{
			goodsIdList = new List<int>();
			extGoodsList = new List<int>();
			ESoulStoneErrorCode result;
			if (!this.IsGongNengOpened(client))
			{
				result = ESoulStoneErrorCode.NotOpen;
			}
			else if (selectExtFuncs != null && selectExtFuncs.Count > 0 && !selectExtFuncs.All((int _type) => openedExtFuncs != null && openedExtFuncs.Exists((SoulStoneExtFuncItem _item) => _item.FuncType == _type)))
			{
				result = ESoulStoneErrorCode.SelectExtFuncNotOpen;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "SoulStoneRandId");
				SoulStoneRandConfig soulStoneRandConfig = null;
				if (!this.randDict.TryGetValue(roleParamsInt32FromDB, out soulStoneRandConfig))
				{
					result = ESoulStoneErrorCode.ConfigError;
				}
				else
				{
					SoulStoneExtFuncItem soulStoneExtFuncItem = null;
					SoulStoneExtFuncItem soulStoneExtFuncItem2 = null;
					SoulStoneExtFuncItem soulStoneExtFuncItem3 = null;
					SoulStoneExtFuncItem soulStoneExtFuncItem4 = null;
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(1))
					{
						soulStoneExtFuncItem = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 1);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(2))
					{
						soulStoneExtFuncItem2 = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 2);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(3))
					{
						soulStoneExtFuncItem3 = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 3);
					}
					if (selectExtFuncs != null && openedExtFuncs != null && selectExtFuncs.Contains(4))
					{
						soulStoneExtFuncItem4 = openedExtFuncs.Find((SoulStoneExtFuncItem _item) => _item.FuncType == 4);
					}
					Dictionary<ESoulStoneExtCostType, int> dictionary = new Dictionary<ESoulStoneExtCostType, int>
					{
						{
							ESoulStoneExtCostType.MoJing,
							0
						},
						{
							ESoulStoneExtCostType.XingHun,
							0
						},
						{
							ESoulStoneExtCostType.ChengJiu,
							0
						},
						{
							ESoulStoneExtCostType.ShengWang,
							0
						},
						{
							ESoulStoneExtCostType.ZuanShi,
							0
						}
					};
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					int num = soulStoneRandConfig.NeedLangHunFenMo;
					if (soulStoneExtFuncItem != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)soulStoneExtFuncItem.CostType;
						int num2;
						if (soulStoneRandConfig.AddedNeedDict.TryGetValue(costType, out num2) && num2 > 0)
						{
							int num3 = this.ExtCostTypeHadValue(client, costType);
							if (num3 < num2 + dictionary[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary2;
							ESoulStoneExtCostType key;
							(dictionary2 = dictionary)[key = costType] = dictionary2[key] + num2;
							if ((double)Global.GetRandomNumber(1, 100) * 1.0 / 100.0 <= soulStoneRandConfig.AddedRate)
							{
								flag = true;
							}
						}
					}
					if (soulStoneExtFuncItem2 != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)soulStoneExtFuncItem2.CostType;
						int num2;
						if (soulStoneRandConfig.ReduceNeedDict.TryGetValue(costType, out num2) && num2 > 0)
						{
							int num3 = this.ExtCostTypeHadValue(client, costType);
							if (num3 < num2 + dictionary[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary2;
							ESoulStoneExtCostType key;
							(dictionary2 = dictionary)[key = costType] = dictionary2[key] + num2;
							if ((double)Global.GetRandomNumber(1, 100) * 1.0 / 100.0 <= soulStoneRandConfig.ReduceRate)
							{
								num -= soulStoneRandConfig.ReduceValue;
							}
						}
					}
					if (soulStoneExtFuncItem3 != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)soulStoneExtFuncItem3.CostType;
						int num2;
						if (soulStoneRandConfig.UpSucRateNeedDict.TryGetValue(costType, out num2) && num2 > 0)
						{
							int num3 = this.ExtCostTypeHadValue(client, costType);
							if (num3 < num2 + dictionary[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary2;
							ESoulStoneExtCostType key;
							(dictionary2 = dictionary)[key = costType] = dictionary2[key] + num2;
							flag2 = true;
						}
					}
					if (soulStoneExtFuncItem4 != null)
					{
						ESoulStoneExtCostType costType = (ESoulStoneExtCostType)soulStoneExtFuncItem4.CostType;
						int num2;
						if (soulStoneRandConfig.UpSucRateNeedDict.TryGetValue(costType, out num2) && num2 > 0)
						{
							int num3 = this.ExtCostTypeHadValue(client, costType);
							if (num3 < num2 + dictionary[costType])
							{
								return ESoulStoneErrorCode.ExtCostNotEnough;
							}
							Dictionary<ESoulStoneExtCostType, int> dictionary2;
							ESoulStoneExtCostType key;
							(dictionary2 = dictionary)[key = costType] = dictionary2[key] + num2;
							flag3 = true;
						}
					}
					num = Math.Max(0, num);
					if (num > 0 && num > Global.GetRoleParamsInt32FromDB(client, "LangHunFenMo"))
					{
						result = ESoulStoneErrorCode.LangHunFenMoNotEnough;
					}
					else if (!this.CanAddGoodsNum(client, 1 + (flag ? 1 : 0)))
					{
						result = ESoulStoneErrorCode.BagNoSpace;
					}
					else
					{
						foreach (KeyValuePair<ESoulStoneExtCostType, int> keyValuePair in dictionary)
						{
							this.DoExtCostType(client, keyValuePair.Key, keyValuePair.Value);
						}
						GameManager.ClientMgr.ModifyLangHunFenMoValue(client, -num, "聚魂", true, true);
						int randomNumber = Global.GetRandomNumber(soulStoneRandConfig.RandMinNumber, soulStoneRandConfig.RandMaxNumber);
						foreach (SoulStoneRandInfo soulStoneRandInfo in soulStoneRandConfig.RandStoneList)
						{
							if (soulStoneRandInfo.RandBegin <= randomNumber && randomNumber <= soulStoneRandInfo.RandEnd)
							{
								GoodsData goodsData = Global.CopyGoodsData(soulStoneRandInfo.Goods);
								List<int> list = new List<int>();
								list.Add(1);
								list.Add(0);
								goodsData.Site = 8000;
								goodsData.ElementhrtsProps = list;
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, 0, "", goodsData.Forge_level, goodsData.Binding, goodsData.Site, "", false, 1, "聚魂", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, list, 0, true);
								goodsIdList.Add(goodsData.GoodsID);
								break;
							}
						}
						if (flag)
						{
							GoodsData goodsData2 = Global.CopyGoodsData(soulStoneRandConfig.AddedGoods);
							List<int> list = new List<int>();
							list.Add(1);
							list.Add(0);
							goodsData2.Site = 8000;
							goodsData2.ElementhrtsProps = list;
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, 0, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", false, 1, "聚魂", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, list, 0, true);
							extGoodsList.Add(goodsData2.GoodsID);
						}
						double num4 = flag2 ? soulStoneRandConfig.UpSucRateTo : soulStoneRandConfig.SuccessRate;
						double num5 = (double)Global.GetRandomNumber(1, 101) * 1.0 / 100.0;
						int num6;
						if (num5 <= num4)
						{
							num6 = soulStoneRandConfig.SuccessTo[Global.GetRandomNumber(0, soulStoneRandConfig.SuccessTo.Count)];
						}
						else if (flag3)
						{
							num6 = soulStoneRandConfig.FailToIfHold[Global.GetRandomNumber(0, soulStoneRandConfig.FailToIfHold.Count)];
						}
						else
						{
							num6 = soulStoneRandConfig.FailTo[Global.GetRandomNumber(0, soulStoneRandConfig.FailTo.Count)];
						}
						Global.SaveRoleParamsInt32ValueToDB(client, "SoulStoneRandId", num6, true);
						if (this.bOpenStoneGetLog)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.AppendFormat("rolename={0} 第{1}次聚魂, 再随机成功配置几率={2}, 产生几率={3}, 随机组变化{4}--->{5},", new object[]
							{
								client.ClientData.RoleName,
								currTimes,
								num4,
								num5,
								roleParamsInt32FromDB,
								num6
							});
							stringBuilder.Append("消耗[");
							stringBuilder.Append("狼魂粉末:" + num + ",");
							stringBuilder.Append("魔晶:" + dictionary[ESoulStoneExtCostType.MoJing] + ",");
							stringBuilder.Append("星魂:" + dictionary[ESoulStoneExtCostType.XingHun] + ",");
							stringBuilder.Append("成就:" + dictionary[ESoulStoneExtCostType.ChengJiu] + ",");
							stringBuilder.Append("声望:" + dictionary[ESoulStoneExtCostType.ShengWang] + ",");
							stringBuilder.Append("钻石:" + dictionary[ESoulStoneExtCostType.ZuanShi] + "]");
							stringBuilder.AppendLine();
							LogManager.WriteLog(2, stringBuilder.ToString(), null, true);
						}
						result = ESoulStoneErrorCode.Success;
					}
				}
			}
			return result;
		}

		public void GM_Test(GameClient client, string[] args)
		{
			if (client != null)
			{
				if (args.Length >= 2)
				{
					if (args[1] == "addlanghun")
					{
						if (args.Length >= 3)
						{
							int addValue = Convert.ToInt32(args[2]);
							GameManager.ClientMgr.ModifyLangHunFenMoValue(client, addValue, "GM", true, true);
						}
					}
					else if (args[1] == "juhun")
					{
						if (args.Length >= 3)
						{
							int num = Convert.ToInt32(args[2]);
							List<int> list = new List<int>();
							if (args.Length >= 4)
							{
								string[] array = args[3].Split(new char[]
								{
									','
								});
								for (int i = 0; i < array.Length; i++)
								{
									list.Add(Convert.ToInt32(array[i]));
								}
							}
							List<SoulStoneExtFuncItem> extFuncItems = this.GetExtFuncItems();
							for (int j = 1; j <= num; j++)
							{
							}
						}
					}
					else if (args[1] == "modequip")
					{
						if (args.Length >= 4)
						{
							int bagIndex = Convert.ToInt32(args[2]);
							int newDbId = Convert.ToInt32(args[3]);
							this.handleModEquip(client, bagIndex, newDbId);
						}
					}
					else if (args[1] == "resetbag")
					{
						this.ResetSoulStoneBag(client);
					}
					else if (args[1] == "lvlup")
					{
						if (args.Length >= 5)
						{
							int target = Convert.ToInt32(args[2]);
							int site = Convert.ToInt32(args[3]);
							List<int> list2 = new List<int>();
							string[] array = args[4].Split(new char[]
							{
								','
							});
							for (int i = 0; i < array.Length; i++)
							{
								list2.Add(Convert.ToInt32(array[i]));
							}
							int num2;
							int num3;
							this.handleSoulStoneLevelUp(client, target, site, list2, out num2, out num3);
						}
					}
				}
			}
		}

		private int defaultRandId;

		private Dictionary<int, SoulStoneRandConfig> randDict = new Dictionary<int, SoulStoneRandConfig>();

		private Dictionary<int, SoulStoneExpConfig> suitExpDict = new Dictionary<int, SoulStoneExpConfig>();

		private Dictionary<int, int> stone2TypeDict = new Dictionary<int, int>();

		private Dictionary<int, SoulStoneGroupConfig> groupDict = new Dictionary<int, SoulStoneGroupConfig>();

		private HashSet<int> EquipCategorys = new HashSet<int>();

		private HashSet<int> JingHuaCategorys = new HashSet<int>();

		private Dictionary<int, int> jinghuaExpDict = null;

		private Dictionary<int, int> equipLvlLimitDict = null;

		private bool bOpenStoneGetLog = false;
	}
}
