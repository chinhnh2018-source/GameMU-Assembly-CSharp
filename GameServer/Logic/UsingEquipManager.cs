using System;
using System.Collections.Generic;
using GameServer.Logic.Ornament;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class UsingEquipManager
	{
		public bool CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, bool hintClient = false)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
				{
					result = false;
				}
				else if (!Global.IsCanEquipOrUseByOccupation(client, goodsData.GoodsID))
				{
					result = false;
				}
				else
				{
					int num = this.EquipFirstPropCondition(client, systemXmlItem);
					if (num == -1)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(556, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (num == -2)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(557, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (num == -3)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(558, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else if (num == -4)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(559, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = false;
					}
					else
					{
						int num2 = this._CanUsingEquip(client, goodsData, toBagIndex, systemXmlItem);
						if (num2 < 0)
						{
							if (hintClient)
							{
								string stringValue = systemXmlItem.GetStringValue("Title");
								if (-3 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(560, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-2 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(561, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-1 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(562, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-5 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(563, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-4 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(564, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-44 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(565, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-444 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(566, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								else if (-55 == num2)
								{
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(567, new object[0]), new object[]
									{
										stringValue
									}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								}
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		public int _CanUsingChongWu(int nCategoriy)
		{
			List<GoodsData> list = null;
			if (9 == nCategoriy)
			{
				if (this.EquipDict.TryGetValue(9, out list))
				{
					if (list != null && list.Count > 0)
					{
						return -4;
					}
				}
				if (!this.EquipDict.TryGetValue(10, out list))
				{
					return 0;
				}
			}
			if (10 == nCategoriy)
			{
				if (this.EquipDict.TryGetValue(10, out list))
				{
					if (list != null && list.Count > 0)
					{
						return -4;
					}
				}
				if (!this.EquipDict.TryGetValue(9, out list))
				{
					return 0;
				}
			}
			int result;
			if (list != null && list.Count > 0)
			{
				result = -2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public int _CanUsingEquip(GameClient client, GoodsData goodsData, int toBagIndex, SystemXmlItem systemGoods = null)
		{
			if (null == systemGoods)
			{
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemGoods))
				{
					return -1;
				}
			}
			int intValue = systemGoods.GetIntValue("Categoriy", -1);
			if (!RebornEquip.IsRebornEquip(goodsData.GoodsID))
			{
				if ((intValue < 0 || intValue >= 49) && intValue != 340)
				{
					return -2;
				}
			}
			else
			{
				if (intValue < 30 || intValue > 38)
				{
					return -2;
				}
				int intValue2 = systemGoods.GetIntValue("ToReborn", -1);
				int intValue3 = systemGoods.GetIntValue("ToRebornLevel", -1);
				if (client.ClientData.RebornCount < intValue2 && client.ClientData.RebornLevel < intValue3)
				{
					return -4;
				}
				if (goodsData.GCount <= 0)
				{
					return -5;
				}
			}
			int intValue4 = systemGoods.GetIntValue("HandType", -1);
			if (intValue < 22 && intValue >= 11)
			{
				int intValue5 = systemGoods.GetIntValue("ActionType", -1);
				int num = WeaponAdornManager.VerifyWeaponCanEquip(Global.CalcOriginalOccupationID(client), intValue4, intValue5, this.EquipDict);
				if (num < 0)
				{
					return num;
				}
			}
			if (intValue <= 38 && intValue >= 37)
			{
				int num = RebornEquip.VerifyWeaponCanEquip(client.UsingEquipMgr.EquipDict);
				if (num < 0)
				{
					return num;
				}
			}
			bool flag = GameFuncControlManager.IsGameFuncDisabled(11);
			List<GoodsData> list = null;
			int result;
			if (!this.EquipDict.TryGetValue(intValue, out list))
			{
				if (intValue == 23 && !flag)
				{
					result = OrnamentManager.getInstance()._CanUsingOrnament(client, toBagIndex, list);
				}
				else if (intValue == 9 || intValue == 10)
				{
					result = this._CanUsingChongWu(intValue);
				}
				else if (GoodsUtil.CanEquip(intValue, goodsData.Site))
				{
					result = 0;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				int count = list.Count;
				if (intValue < 22 && intValue >= 11)
				{
					if (intValue4 == 2 || GameManager.MagicSwordMgr.IsMagicSword(client))
					{
						if (count >= 2)
						{
							return -3;
						}
						return 0;
					}
				}
				else if (intValue == 6)
				{
					if (count >= 2)
					{
						return -3;
					}
					return 0;
				}
				else if (intValue == 36)
				{
					if (count >= 2)
					{
						return -3;
					}
					return 0;
				}
				else if (intValue == 9 || intValue == 10)
				{
					int num = this._CanUsingChongWu(intValue);
					if (num < 0)
					{
						return num;
					}
				}
				else if (intValue == 23 && !flag)
				{
					return OrnamentManager.getInstance()._CanUsingOrnament(client, toBagIndex, list);
				}
				result = ((list.Count < 1) ? 0 : -3);
			}
			return result;
		}

		public int EquipFirstPropCondition(GameClient client, SystemXmlItem systemGoods = null)
		{
			int intValue = systemGoods.GetIntValue("Strength", -1);
			int intValue2 = systemGoods.GetIntValue("Intelligence", -1);
			int intValue3 = systemGoods.GetIntValue("Dexterity", -1);
			int intValue4 = systemGoods.GetIntValue("Constitution", -1);
			int result;
			if (intValue > 0 && (double)intValue > RoleAlgorithm.GetStrength(client, true))
			{
				result = -1;
			}
			else if (intValue2 > 0 && (double)intValue2 > RoleAlgorithm.GetIntelligence(client, true))
			{
				result = -2;
			}
			else if (intValue3 > 0 && (double)intValue3 > RoleAlgorithm.GetDexterity(client, true))
			{
				result = -3;
			}
			else if (intValue4 > 0 && (double)intValue4 > RoleAlgorithm.GetConstitution(client, true))
			{
				result = -4;
			}
			else
			{
				result = 1;
			}
			return result;
		}

		public void RefreshEquip(GoodsData goodsData)
		{
			if (null != goodsData)
			{
				SystemXmlItem systemXmlItem = null;
				if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
				{
					int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
					if ((intValue >= 0 && intValue < 49) || intValue == 340)
					{
						List<GoodsData> list = null;
						if (!this.EquipDict.TryGetValue(intValue, out list))
						{
							list = new List<GoodsData>();
							this.EquipDict[intValue] = list;
						}
						if (goodsData.Using <= 0)
						{
							list.Remove(goodsData);
							if (intValue == 5 || (intValue >= 11 && intValue <= 21) || (intValue >= 37 && intValue <= 38))
							{
								lock (this.WeaponStrongList)
								{
									this.WeaponStrongList.Remove(goodsData);
								}
								lock (this.WeaponEquipList)
								{
									this.WeaponEquipList.Remove(goodsData);
								}
							}
							else
							{
								lock (this.EquipList)
								{
									this.EquipList.Remove(goodsData);
								}
							}
						}
						else
						{
							if (list.IndexOf(goodsData) < 0)
							{
								list.Add(goodsData);
							}
							if (intValue == 5 || (intValue >= 11 && intValue <= 21) || (intValue >= 37 && intValue <= 38))
							{
								lock (this.WeaponStrongList)
								{
									if (this.WeaponStrongList.IndexOf(goodsData) < 0)
									{
										this.WeaponStrongList.Add(goodsData);
									}
								}
								if ((intValue >= 11 && intValue <= 21) || (intValue >= 37 && intValue <= 38))
								{
									lock (this.WeaponEquipList)
									{
										if (this.WeaponEquipList.IndexOf(goodsData) < 0)
										{
											this.WeaponEquipList.Add(goodsData);
										}
									}
								}
							}
							else
							{
								lock (this.EquipList)
								{
									if (this.EquipList.IndexOf(goodsData) < 0)
									{
										this.EquipList.Add(goodsData);
									}
								}
							}
						}
					}
				}
			}
		}

		public void RefreshEquips(GameClient client)
		{
			if (client.ClientData.GoodsDataList != null && client.ClientData.GoodsDataList.Count > 0)
			{
				lock (client.ClientData.GoodsDataList)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
					{
						if (client.ClientData.GoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.GoodsDataList[i], client.ClientData.GoodsDataList[i].BagIndex, null) < 0)
							{
								list.Add(client.ClientData.GoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.GoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						GoodsData goodsData = list[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.RebornGoodsDataList != null && client.ClientData.RebornGoodsDataList.Count > 0)
			{
				lock (client.ClientData.RebornGoodsDataList)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.RebornGoodsDataList.Count; i++)
					{
						if (client.ClientData.RebornGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.RebornGoodsDataList[i], client.ClientData.RebornGoodsDataList[i].BagIndex, null) < 0)
							{
								list.Add(client.ClientData.RebornGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.RebornGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						GoodsData goodsData = list[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.FashionGoodsDataList != null && client.ClientData.FashionGoodsDataList.Count > 0)
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.FashionGoodsDataList[i], client.ClientData.FashionGoodsDataList[i].BagIndex, null) >= 0)
							{
								this.RefreshEquip(client.ClientData.FashionGoodsDataList[i]);
							}
						}
					}
				}
			}
			if (client.ClientData.DamonGoodsDataList != null && client.ClientData.DamonGoodsDataList.Count > 0)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.DamonGoodsDataList[i], client.ClientData.DamonGoodsDataList[i].BagIndex, null) < 0)
							{
								list.Add(client.ClientData.DamonGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.DamonGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						GoodsData goodsData = list[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
			if (client.ClientData.OrnamentGoodsDataList != null && client.ClientData.OrnamentGoodsDataList.Count > 0)
			{
				lock (client.ClientData.OrnamentGoodsDataList)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.OrnamentGoodsDataList.Count; i++)
					{
						if (client.ClientData.OrnamentGoodsDataList[i].Using > 0)
						{
							if (this._CanUsingEquip(client, client.ClientData.OrnamentGoodsDataList[i], client.ClientData.OrnamentGoodsDataList[i].BagIndex, null) < 0)
							{
								list.Add(client.ClientData.OrnamentGoodsDataList[i]);
							}
							else
							{
								this.RefreshEquip(client.ClientData.OrnamentGoodsDataList[i]);
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						GoodsData goodsData = list[i];
						goodsData.Using = 0;
						Global.ResetBagGoodsData(client, goodsData);
					}
				}
			}
		}

		public void AttackSomebody(GameClient client)
		{
			if (this.WeaponStrongList.Count > 0)
			{
				GoodsData goodsData = null;
				lock (this.WeaponStrongList)
				{
					goodsData = this.WeaponStrongList[Global.GetRandomNumber(0, this.WeaponStrongList.Count)];
				}
				GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
			}
		}

		public void InjuredSomebody(GameClient client)
		{
			if (this.EquipList.Count > 0)
			{
				GoodsData goodsData = null;
				lock (this.EquipList)
				{
					goodsData = this.EquipList[Global.GetRandomNumber(0, this.EquipList.Count)];
				}
				GameManager.ClientMgr.AddEquipStrong(client, goodsData, 1);
			}
		}

		public void GMAddEquipStrong(GameClient client, int val)
		{
			if (this.EquipList.Count > 0)
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (this.EquipList)
				{
					list.AddRange(this.EquipList);
				}
				lock (this.WeaponStrongList)
				{
					list.AddRange(this.WeaponStrongList);
				}
				foreach (GoodsData goodsData in list)
				{
					GameManager.ClientMgr.AddEquipStrong(client, goodsData, val * 500);
				}
			}
		}

		public GoodsData GetGoodsDataByCategoriy(GameClient client, int categoriy)
		{
			List<GoodsData> list = null;
			GoodsData result;
			if (!this.EquipDict.TryGetValue(categoriy, out list))
			{
				result = null;
			}
			else if (list == null || list.Count <= 0)
			{
				result = null;
			}
			else
			{
				result = list[0];
			}
			return result;
		}

		public List<GoodsData> GetGoodsByCategoriyList(List<int> categoriyList)
		{
			List<GoodsData> result;
			if (categoriyList == null || categoriyList.Count == 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (this.EquipDict)
				{
					foreach (KeyValuePair<int, List<GoodsData>> keyValuePair in this.EquipDict)
					{
						int key = keyValuePair.Key;
						if (categoriyList.Contains(key) && keyValuePair.Value != null)
						{
							list.AddRange(keyValuePair.Value);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public List<GoodsData> GetGoodsByIDRange(List<Tuple<int, int>> idRange)
		{
			List<GoodsData> result;
			if (idRange == null || idRange.Count == 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> resultList = new List<GoodsData>();
				lock (this.WeaponStrongList)
				{
					this.WeaponStrongList.ForEach(delegate(GoodsData data)
					{
						if (idRange.Exists((Tuple<int, int> range) => range.Item1 <= data.GoodsID && range.Item2 >= data.GoodsID))
						{
							resultList.Add(data);
						}
					});
				}
				lock (this.EquipList)
				{
					this.EquipList.ForEach(delegate(GoodsData data)
					{
						if (idRange.Exists((Tuple<int, int> range) => range.Item1 <= data.GoodsID && range.Item2 >= data.GoodsID))
						{
							resultList.Add(data);
						}
					});
				}
				result = resultList;
			}
			return result;
		}

		public List<GoodsData> GetWeaponStrongList()
		{
			return this.WeaponStrongList;
		}

		public List<GoodsData> GetWeaponEquipList()
		{
			return this.WeaponEquipList;
		}

		public int GetUsingEquipAllAppendPropLeva()
		{
			int num = 0;
			foreach (GoodsData goodsData in this.WeaponStrongList)
			{
				if (goodsData != null && goodsData.Using > 0)
				{
					num += goodsData.AppendPropLev;
				}
			}
			foreach (GoodsData goodsData in this.EquipList)
			{
				if (goodsData != null && goodsData.Using > 0)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if (goodsCatetoriy != 9 && goodsCatetoriy != 10 && !GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods && goodsCatetoriy != 8)
					{
						num += goodsData.AppendPropLev;
					}
				}
			}
			return num;
		}

		public int GetUsingEquipAllForge()
		{
			int num = 0;
			foreach (GoodsData goodsData in this.WeaponStrongList)
			{
				if (goodsData != null && goodsData.Using > 0)
				{
					num += goodsData.Forge_level;
				}
			}
			foreach (GoodsData goodsData in this.EquipList)
			{
				if (goodsData != null && goodsData.Using > 0)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if (goodsCatetoriy != 9 && goodsCatetoriy != 10 && !GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods && goodsCatetoriy != 8)
					{
						num += goodsData.Forge_level;
					}
				}
			}
			return num;
		}

		public List<int> GetUsingEquipForge()
		{
			List<int> list = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsData in this.WeaponStrongList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						list.Add(goodsData.Forge_level);
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsData in this.EquipList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy != 9 && goodsCatetoriy != 10 && !GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods && goodsCatetoriy != 8)
						{
							list.Add(goodsData.Forge_level);
						}
					}
				}
			}
			return list;
		}

		public List<int> GetUsingEquipAppend()
		{
			List<int> list = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsData in this.WeaponStrongList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						list.Add(goodsData.AppendPropLev);
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsData in this.EquipList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy != 9 && goodsCatetoriy != 10 && !GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods && goodsCatetoriy != 8)
						{
							list.Add(goodsData.AppendPropLev);
						}
					}
				}
			}
			return list;
		}

		public List<int> GetUsingEquipExcellencePropNum()
		{
			List<int> list = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsData in this.WeaponStrongList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						list.Add(Global.GetEquipExcellencePropNum(goodsData));
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsData in this.EquipList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy != 9 && goodsCatetoriy != 10)
						{
							list.Add(Global.GetEquipExcellencePropNum(goodsData));
						}
					}
				}
			}
			return list;
		}

		public List<int> GetUsingEquipSuit()
		{
			List<int> list = new List<int>();
			lock (this.WeaponStrongList)
			{
				foreach (GoodsData goodsData in this.WeaponStrongList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						list.Add(Global.GetEquipGoodsSuitID(goodsData.GoodsID));
					}
				}
			}
			lock (this.EquipList)
			{
				foreach (GoodsData goodsData in this.EquipList)
				{
					if (goodsData != null && goodsData.Using > 0)
					{
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy != 9 && goodsCatetoriy != 10)
						{
							list.Add(Global.GetEquipGoodsSuitID(goodsData.GoodsID));
						}
					}
				}
			}
			return list;
		}

		public void RightEquipIndex(ref int index)
		{
			List<GoodsData> list = null;
			GoodsData goodsData = null;
			int num = 0;
			lock (this.EquipDict)
			{
				int i = 11;
				while (i < 22)
				{
					if (this.EquipDict.TryGetValue(i, out list))
					{
						if (list.Count != 0)
						{
							num += list.Count;
							if (num >= 2)
							{
								return;
							}
							if (num == 1 && list.Count > 0)
							{
								goodsData = list[0];
							}
						}
					}
					IL_8E:
					i++;
					continue;
					goto IL_8E;
				}
			}
			if (num == 1 && goodsData != null)
			{
				if (goodsData.BagIndex == index && goodsData.BagIndex == 1)
				{
					index = 0;
				}
				else if (goodsData.BagIndex == index && goodsData.BagIndex == 0)
				{
					index = 1;
				}
			}
		}

		public void RightAnelIndex(ref int index, int Categoriy)
		{
			if (Categoriy == 6)
			{
				List<GoodsData> list = null;
				GoodsData goodsData = null;
				int num = 0;
				lock (this.EquipDict)
				{
					if (this.EquipDict.TryGetValue(Categoriy, out list))
					{
						num += list.Count;
						if (num >= 2)
						{
							return;
						}
						if (num == 1 && list.Count > 0)
						{
							goodsData = list[0];
						}
					}
				}
				if (num == 1 && goodsData != null)
				{
					if (goodsData.BagIndex == index && goodsData.BagIndex == 1)
					{
						index = 0;
					}
					else if (goodsData.BagIndex == index && goodsData.BagIndex == 0)
					{
						index = 1;
					}
				}
			}
		}

		public void RebornRightAnelIndex(ref int index, int Categoriy)
		{
			List<GoodsData> list = null;
			GoodsData goodsData = null;
			int num = 0;
			lock (this.EquipDict)
			{
				if (this.EquipDict.TryGetValue(Categoriy, out list))
				{
					num += list.Count;
					if (num >= 2)
					{
						return;
					}
					if (num == 1 && list.Count > 0)
					{
						goodsData = list[0];
					}
				}
			}
			if (num == 1 && goodsData != null)
			{
				if (goodsData.BagIndex == index && goodsData.BagIndex == 1)
				{
					index = 0;
				}
				else if (goodsData.BagIndex == index && goodsData.BagIndex == 0)
				{
					index = 1;
				}
			}
		}

		public int GetUsingEquipArchangelWeaponSuit()
		{
			int num = 0;
			foreach (GoodsData goodsData in this.WeaponStrongList)
			{
				if (goodsData != null && goodsData.Using > 0)
				{
					if (Data.IsDaTianShiGoods(goodsData.GoodsID))
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
						{
							int intValue = systemXmlItem.GetIntValue("SuitID", -1);
							if (num < intValue)
							{
								num = intValue;
							}
						}
					}
				}
			}
			return num;
		}

		private Dictionary<int, List<GoodsData>> EquipDict = new Dictionary<int, List<GoodsData>>();

		private GoodsData WeaponEquip = null;

		private List<GoodsData> WeaponStrongList = new List<GoodsData>();

		private List<GoodsData> WeaponEquipList = new List<GoodsData>();

		private List<GoodsData> EquipList = new List<GoodsData>();
	}
}
