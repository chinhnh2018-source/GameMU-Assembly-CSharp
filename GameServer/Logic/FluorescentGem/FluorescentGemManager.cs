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

namespace GameServer.Logic.FluorescentGem
{
	public class FluorescentGemManager
	{
		private void LoadFluorescentGemLevelTypeConfigData()
		{
			try
			{
				lock (this.FluorescentGemLevelTypeConfigDict)
				{
					string text = "Config/Gem/GemDigType.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.FluorescentGemLevelTypeConfigDict.Clear();
						foreach (XElement xml in enumerable)
						{
							FluorescentGemLevelTypeConfigData fluorescentGemLevelTypeConfigData = new FluorescentGemLevelTypeConfigData();
							int key = (int)Global.GetSafeAttributeLong(xml, "Type");
							fluorescentGemLevelTypeConfigData._NeedFluorescentPoint = (int)Global.GetSafeAttributeLong(xml, "CostYingGuangFenMo");
							fluorescentGemLevelTypeConfigData._NeedDiamond = (int)Global.GetSafeAttributeLong(xml, "CostZuanShi");
							this.FluorescentGemLevelTypeConfigDict.Add(key, fluorescentGemLevelTypeConfigData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadFluorescentGemLevelTypeConfigData", new object[0])));
			}
		}

		private void LoadFluorescentGemDigConfigData()
		{
			try
			{
				lock (this.FluorescentGemDigConfigDict)
				{
					string text = "Config/Gem/GemDig.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.FluorescentGemDigConfigDict.Clear();
						foreach (XElement xelement2 in enumerable)
						{
							int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
							List<FluorescentGemDigConfigData> list = new List<FluorescentGemDigConfigData>();
							IEnumerable<XElement> enumerable2 = xelement2.Elements();
							foreach (XElement xml in enumerable2)
							{
								list.Add(new FluorescentGemDigConfigData
								{
									_GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID"),
									_StartValue = (int)Global.GetSafeAttributeLong(xml, "StartValues"),
									_EndValue = (int)Global.GetSafeAttributeLong(xml, "EndValues")
								});
							}
							this.FluorescentGemDigConfigDict.Add(key, list);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-FluorescentGemDigConfigDict", new object[0])));
			}
		}

		private void LoadFluorescentGemUpConfigData()
		{
			try
			{
				lock (this.FluorescentGemUpConfigDict)
				{
					string text = "Config/Gem/GemLevelup.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.FluorescentGemUpConfigDict.Clear();
						foreach (XElement xml in enumerable)
						{
							FluorescentGemUpConfigData fluorescentGemUpConfigData = new FluorescentGemUpConfigData();
							int key = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
							fluorescentGemUpConfigData._ElementsType = (int)Global.GetSafeAttributeLong(xml, "ElementsTypeID");
							fluorescentGemUpConfigData._GemType = (int)Global.GetSafeAttributeLong(xml, "GemTypeID");
							fluorescentGemUpConfigData._Level = (int)Global.GetSafeAttributeLong(xml, "Level");
							fluorescentGemUpConfigData._OldGoodsID = (int)Global.GetSafeAttributeLong(xml, "OldGoodsID");
							fluorescentGemUpConfigData._NewGoodsID = (int)Global.GetSafeAttributeLong(xml, "NewGoodsID");
							fluorescentGemUpConfigData._NeedOldGoodsCount = (int)Global.GetSafeAttributeLong(xml, "NeedOldGoodsNum");
							fluorescentGemUpConfigData._NeedLevelOneGoodsCount = (int)Global.GetSafeAttributeLong(xml, "NeedOneLevelNum");
							fluorescentGemUpConfigData._NeedGold = (int)Global.GetSafeAttributeLong(xml, "CostBandJinBi");
							this.FluorescentGemUpConfigDict.Add(key, fluorescentGemUpConfigData);
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadFluorescentGemLevelTypeConfigData", new object[0])));
			}
		}

		private bool IsSameGem(FluorescentGemUpConfigData data1, FluorescentGemUpConfigData data2)
		{
			return data1 != null && null != data2 && (data1._ElementsType == data2._ElementsType && data1._GemType == data2._GemType);
		}

		private bool CheckEquipPositionIndex(int nIndex)
		{
			return nIndex > 0 && nIndex < 11;
		}

		private bool CheckGemTypeIndex(int nIndex)
		{
			return nIndex > 0 && nIndex < 4;
		}

		private int GetFluorescentPointByGoodsID(int nGoodsID)
		{
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemXmlItem))
			{
				result = 0;
			}
			else
			{
				result = systemXmlItem.GetIntValue("ChangeYingGuang", -1);
			}
			return result;
		}

		private int GetFluorescentGemBagSpace(GameClient client)
		{
			int result;
			if (null == client)
			{
				result = 0;
			}
			else
			{
				result = 220 - client.ClientData.FluorescentGemData.GemBagList.Count<GoodsData>();
			}
			return result;
		}

		private void ResetBagAllGoods(GameClient client)
		{
			lock (client.ClientData.FluorescentGemData)
			{
				List<GoodsData> gemBagList = client.ClientData.FluorescentGemData.GemBagList;
				Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
				List<GoodsData> list = new List<GoodsData>();
				for (int i = 0; i < gemBagList.Count; i++)
				{
					gemBagList[i].BagIndex = 1;
					int goodsGridNumByID = Global.GetGoodsGridNumByID(gemBagList[i].GoodsID);
					if (goodsGridNumByID > 1)
					{
						GoodsData goodsData = null;
						string key = string.Format("{0}_{1}_{2}_{3}", new object[]
						{
							gemBagList[i].GoodsID,
							gemBagList[i].Binding,
							Global.DateTimeTicks(gemBagList[i].Starttime),
							Global.DateTimeTicks(gemBagList[i].Endtime)
						});
						if (dictionary.TryGetValue(key, out goodsData))
						{
							int num = Global.GMin(goodsGridNumByID - goodsData.GCount, gemBagList[i].GCount);
							goodsData.GCount += num;
							gemBagList[i].BagIndex = 1;
							goodsData.BagIndex = 1;
							gemBagList[i].GCount -= num;
							if (!Global.ResetBagGoodsData(client, gemBagList[i]))
							{
								break;
							}
							if (goodsData.GCount >= goodsGridNumByID)
							{
								if (gemBagList[i].GCount > 0)
								{
									dictionary[key] = gemBagList[i];
								}
								else
								{
									dictionary.Remove(key);
									list.Add(gemBagList[i]);
								}
							}
							else if (gemBagList[i].GCount <= 0)
							{
								list.Add(gemBagList[i]);
							}
						}
						else
						{
							dictionary[key] = gemBagList[i];
						}
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					gemBagList.Remove(list[i]);
				}
				gemBagList.Sort(delegate(GoodsData x, GoodsData y)
				{
					int goodsYinLiangNumByID = Global.GetGoodsYinLiangNumByID(x.GoodsID);
					int goodsYinLiangNumByID2 = Global.GetGoodsYinLiangNumByID(y.GoodsID);
					int result;
					if (goodsYinLiangNumByID2 == goodsYinLiangNumByID)
					{
						result = y.GCount - x.GCount;
					}
					else
					{
						result = goodsYinLiangNumByID - goodsYinLiangNumByID2;
					}
					return result;
				});
				int num2 = 0;
				for (int i = 0; i < gemBagList.Count; i++)
				{
					bool flag2 = 0 == 0;
					gemBagList[i].BagIndex = num2++;
					if (!Global.ResetBagGoodsData(client, gemBagList[i]))
					{
						break;
					}
				}
			}
		}

		public Dictionary<int, GoodsData> GetBagDict(GameClient client)
		{
			Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
			foreach (GoodsData goodsData in client.ClientData.FluorescentGemData.GemBagList)
			{
				if (goodsData.BagIndex < 220)
				{
					dictionary[goodsData.BagIndex] = goodsData;
				}
			}
			return dictionary;
		}

		public Dictionary<int, Dictionary<int, GoodsData>> GetEquipDict(GameClient client)
		{
			Dictionary<int, Dictionary<int, GoodsData>> dictionary = new Dictionary<int, Dictionary<int, GoodsData>>();
			foreach (GoodsData goodsData in client.ClientData.FluorescentGemData.GemEquipList)
			{
				int key;
				int key2;
				this.ParsePosAndType(goodsData.BagIndex, out key, out key2);
				Dictionary<int, GoodsData> dictionary2 = null;
				if (!dictionary.TryGetValue(key, out dictionary2))
				{
					dictionary2 = new Dictionary<int, GoodsData>();
					dictionary[key] = dictionary2;
				}
				dictionary2[key2] = goodsData;
			}
			return dictionary;
		}

		private EFluorescentGemDigErrorCode FluorescentGemDig(GameClient client, int nLevelType, int nDigType, out List<int> gemList)
		{
			gemList = new List<int>();
			try
			{
				if (null == client)
				{
					return EFluorescentGemDigErrorCode.Error;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return EFluorescentGemDigErrorCode.Error;
				}
				if (!this.FluorescentGemLevelTypeConfigDict.ContainsKey(nLevelType))
				{
					return EFluorescentGemDigErrorCode.LevelTypeError;
				}
				if (nDigType < 0 || nDigType > 1)
				{
					return EFluorescentGemDigErrorCode.DigType;
				}
				int num = 0;
				switch (nDigType)
				{
				case 0:
					num = 1;
					break;
				case 1:
					num = 10;
					break;
				}
				lock (client.ClientData.FluorescentGemData)
				{
					if (this.GetFluorescentGemBagSpace(client) < num)
					{
						return EFluorescentGemDigErrorCode.BagNotEnoughTen;
					}
					FluorescentGemLevelTypeConfigData fluorescentGemLevelTypeConfigData = null;
					lock (this.FluorescentGemLevelTypeConfigDict)
					{
						if (!this.FluorescentGemLevelTypeConfigDict.TryGetValue(nLevelType, out fluorescentGemLevelTypeConfigData) || null == fluorescentGemLevelTypeConfigData)
						{
							return EFluorescentGemDigErrorCode.LevelTypeDataError;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedFluorescentPoint > 0)
					{
						if (client.ClientData.FluorescentPoint < fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num)
						{
							return EFluorescentGemDigErrorCode.PointNotEnough;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedDiamond > 0)
					{
						if (!MoneyUtil.CheckHasMoney(client, 163, fluorescentGemLevelTypeConfigData._NeedDiamond * num) && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, fluorescentGemLevelTypeConfigData._NeedDiamond * num, DaiBiSySType.JingLingLieQu))
						{
							return EFluorescentGemDigErrorCode.DiamondNotEnough;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedFluorescentPoint > 0)
					{
						if (!this.DecFluorescentPoint(client, fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num, "宝石挖掘扣除", false))
						{
							return EFluorescentGemDigErrorCode.UpdatePointError;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedDiamond > 0)
					{
						if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -fluorescentGemLevelTypeConfigData._NeedDiamond * num, "荧光宝石挖掘", false, DaiBiSySType.YingGuanShiChouQu))
						{
							return EFluorescentGemDigErrorCode.UpdateDiamondError;
						}
					}
					List<FluorescentGemDigConfigData> list = null;
					if (!this.FluorescentGemDigConfigDict.TryGetValue(nLevelType, out list) || null == list)
					{
						return EFluorescentGemDigErrorCode.DigDataError;
					}
					for (int i = 0; i < num; i++)
					{
						int randomNumber = Global.GetRandomNumber(1, 100001);
						int num2 = 0;
						for (int j = 0; j < list.Count; j++)
						{
							if (randomNumber >= list[j]._StartValue && randomNumber <= list[j]._EndValue)
							{
								num2 = list[j]._GoodsID;
								break;
							}
						}
						if (!this.CheckIsFluorescentGemByGoodsID(num2))
						{
							return EFluorescentGemDigErrorCode.NotGem;
						}
						int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num2, 1, 0, "", 0, 0, 7000, "", true, 1, "荧光宝石挖掘", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (num3 < 0)
						{
							return EFluorescentGemDigErrorCode.AddGoodsError;
						}
						gemList.Add(num2);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "挖掘", "系统", client.ClientData.RoleName, "修改", num2, client.ClientData.ZoneID, client.strUserID, nLevelType, client.ServerId, null);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "消耗", "系统", client.ClientData.RoleName, "修改", fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num, client.ClientData.ZoneID, client.strUserID, fluorescentGemLevelTypeConfigData._NeedDiamond * num, client.ServerId, null);
				}
				return EFluorescentGemDigErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemDigErrorCode.Error;
		}

		private EFluorescentGemDigErrorCode FluorescentGemDig_BigNum(GameClient client, int nLevelType, int nDigType, out Dictionary<int, int> gemDict)
		{
			gemDict = new Dictionary<int, int>();
			try
			{
				if (null == client)
				{
					return EFluorescentGemDigErrorCode.Error;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return EFluorescentGemDigErrorCode.Error;
				}
				if (!this.FluorescentGemLevelTypeConfigDict.ContainsKey(nLevelType))
				{
					return EFluorescentGemDigErrorCode.LevelTypeError;
				}
				if (nDigType != 2)
				{
					return EFluorescentGemDigErrorCode.DigType;
				}
				int num = 50;
				lock (client.ClientData.FluorescentGemData)
				{
					if (this.GetFluorescentGemBagSpace(client) < num)
					{
						return EFluorescentGemDigErrorCode.BagNotEnoughTen;
					}
					FluorescentGemLevelTypeConfigData fluorescentGemLevelTypeConfigData = null;
					lock (this.FluorescentGemLevelTypeConfigDict)
					{
						if (!this.FluorescentGemLevelTypeConfigDict.TryGetValue(nLevelType, out fluorescentGemLevelTypeConfigData) || null == fluorescentGemLevelTypeConfigData)
						{
							return EFluorescentGemDigErrorCode.LevelTypeDataError;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedFluorescentPoint > 0)
					{
						if (client.ClientData.FluorescentPoint < fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num)
						{
							return EFluorescentGemDigErrorCode.PointNotEnough;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedDiamond > 0)
					{
						if (!MoneyUtil.CheckHasMoney(client, 163, fluorescentGemLevelTypeConfigData._NeedDiamond * num) && !HuanLeDaiBiManager.GetInstance().HuanledaibiReplaceEnough(client, fluorescentGemLevelTypeConfigData._NeedDiamond * num, DaiBiSySType.JingLingLieQu))
						{
							return EFluorescentGemDigErrorCode.DiamondNotEnough;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedFluorescentPoint > 0)
					{
						if (!this.DecFluorescentPoint(client, fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num, "宝石挖掘扣除", false))
						{
							return EFluorescentGemDigErrorCode.UpdatePointError;
						}
					}
					if (fluorescentGemLevelTypeConfigData._NeedDiamond > 0)
					{
						if (!GameManager.ClientMgr.ModifyLuckStarValue(client, -fluorescentGemLevelTypeConfigData._NeedDiamond * num, "荧光宝石挖掘", false, DaiBiSySType.YingGuanShiChouQu))
						{
							return EFluorescentGemDigErrorCode.UpdateDiamondError;
						}
					}
					List<FluorescentGemDigConfigData> list = null;
					if (!this.FluorescentGemDigConfigDict.TryGetValue(nLevelType, out list) || null == list)
					{
						return EFluorescentGemDigErrorCode.DigDataError;
					}
					for (int i = 0; i < num; i++)
					{
						int randomNumber = Global.GetRandomNumber(1, 100001);
						int num2 = 0;
						for (int j = 0; j < list.Count; j++)
						{
							if (randomNumber >= list[j]._StartValue && randomNumber <= list[j]._EndValue)
							{
								num2 = list[j]._GoodsID;
								break;
							}
						}
						if (!this.CheckIsFluorescentGemByGoodsID(num2))
						{
							return EFluorescentGemDigErrorCode.NotGem;
						}
						if (!gemDict.ContainsKey(num2))
						{
							gemDict[num2] = 0;
						}
						Dictionary<int, int> dictionary;
						int key;
						(dictionary = gemDict)[key = num2] = dictionary[key] + 1;
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "挖掘", "系统", client.ClientData.RoleName, "修改", num2, client.ClientData.ZoneID, client.strUserID, nLevelType, client.ServerId, null);
					}
					foreach (KeyValuePair<int, int> keyValuePair in gemDict)
					{
						int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, keyValuePair.Key, keyValuePair.Value, 0, "", 0, 0, 7000, "", true, 1, "荧光宝石挖掘", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (num3 < 0)
						{
							return EFluorescentGemDigErrorCode.AddGoodsError;
						}
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "消耗", "系统", client.ClientData.RoleName, "修改", fluorescentGemLevelTypeConfigData._NeedFluorescentPoint * num, client.ClientData.ZoneID, client.strUserID, fluorescentGemLevelTypeConfigData._NeedDiamond * num, client.ServerId, null);
				}
				return EFluorescentGemDigErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemDigErrorCode.Error;
		}

		private EFluorescentGemResolveErrorCode FluorescentGemResolve(GameClient client, int nBagIndex, int nResolveCount)
		{
			try
			{
				if (null == client)
				{
					return EFluorescentGemResolveErrorCode.Error;
				}
				lock (client.ClientData.FluorescentGemData)
				{
					GoodsData goodsData;
					if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == nBagIndex)) == null)
					{
						return EFluorescentGemResolveErrorCode.GoodsNotExist;
					}
					if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
					{
						return EFluorescentGemResolveErrorCode.NotGem;
					}
					if (nResolveCount <= 0 || nResolveCount > goodsData.GCount)
					{
						return EFluorescentGemResolveErrorCode.ResolveCountError;
					}
					int nAddPoint = this.GetFluorescentPointByGoodsID(goodsData.GoodsID) * nResolveCount;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, nResolveCount, false, false))
					{
						return EFluorescentGemResolveErrorCode.ResolveError;
					}
					this.AddFluorescentPoint(client, nAddPoint, "宝石分解获得", true);
				}
				return EFluorescentGemResolveErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemResolveErrorCode.Error;
		}

		public int GenerateBagIndex(int pos, int type)
		{
			return pos * 100 + type;
		}

		public void ParsePosAndType(int bagIndex, out int pos, out int type)
		{
			pos = bagIndex / 100;
			type = bagIndex % 100;
		}

		private EFluorescentGemUpErrorCode FluorescentGemUp(GameClient client, FluorescentGemUpTransferData upData, out int nNewGoodsDBID)
		{
			nNewGoodsDBID = -1;
			try
			{
				if (null == client)
				{
					return EFluorescentGemUpErrorCode.Error;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return EFluorescentGemUpErrorCode.Error;
				}
				GoodsData goodsData = null;
				lock (client.ClientData.FluorescentGemData)
				{
					if (upData._UpType == 0)
					{
						if (this.GetFluorescentGemBagSpace(client) < 1)
						{
							return EFluorescentGemUpErrorCode.BagNotEnoughOne;
						}
						if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == upData._BagIndex)) == null)
						{
							return EFluorescentGemUpErrorCode.GoodsNotExist;
						}
					}
					else
					{
						if (!this.CheckEquipPositionIndex(upData._Position))
						{
							return EFluorescentGemUpErrorCode.PositionIndexError;
						}
						if (!this.CheckGemTypeIndex(upData._GemType))
						{
							return EFluorescentGemUpErrorCode.GemTypeError;
						}
						if ((goodsData = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(upData._Position, upData._GemType))) == null)
						{
							return EFluorescentGemUpErrorCode.GoodsNotExist;
						}
					}
					if (null == goodsData)
					{
						return EFluorescentGemUpErrorCode.GoodsNotExist;
					}
					if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
					{
						return EFluorescentGemUpErrorCode.NotGem;
					}
					FluorescentGemUpConfigData fluorescentGemUpConfigData = null;
					if (!this.FluorescentGemUpConfigDict.TryGetValue(goodsData.GoodsID, out fluorescentGemUpConfigData) || null == fluorescentGemUpConfigData)
					{
						return EFluorescentGemUpErrorCode.UpDataError;
					}
					if (fluorescentGemUpConfigData._NewGoodsID <= 0)
					{
						return EFluorescentGemUpErrorCode.MaxLevel;
					}
					FluorescentGemUpConfigData fluorescentGemUpConfigData2 = null;
					if (!this.FluorescentGemUpConfigDict.TryGetValue(fluorescentGemUpConfigData._NewGoodsID, out fluorescentGemUpConfigData2) || null == fluorescentGemUpConfigData2)
					{
						return EFluorescentGemUpErrorCode.NextLevelDataError;
					}
					if (client.ClientData.Money1 + client.ClientData.YinLiang < fluorescentGemUpConfigData2._NeedGold)
					{
						return EFluorescentGemUpErrorCode.GoldNotEnough;
					}
					int num = 0;
					num = fluorescentGemUpConfigData._NeedLevelOneGoodsCount * 3;
					int num2 = 0;
					if (upData._UpType == 1)
					{
						num2 += fluorescentGemUpConfigData._NeedLevelOneGoodsCount;
					}
					using (Dictionary<int, int>.Enumerator enumerator = upData._DecGoodsDict.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, int> item = enumerator.Current;
							GoodsData goodsData2;
							if ((goodsData2 = client.ClientData.FluorescentGemData.GemBagList.Find(delegate(GoodsData _g)
							{
								int bagIndex = _g.BagIndex;
								KeyValuePair<int, int> item2 = item;
								return bagIndex == item2.Key;
							})) == null)
							{
								return EFluorescentGemUpErrorCode.DecGoodsNotExist;
							}
							int gcount = goodsData2.GCount;
							KeyValuePair<int, int> item3 = item;
							if (gcount < item3.Value)
							{
								return EFluorescentGemUpErrorCode.DecGoodsNotEnough;
							}
							FluorescentGemUpConfigData fluorescentGemUpConfigData3 = null;
							if (this.FluorescentGemUpConfigDict.TryGetValue(goodsData2.GoodsID, out fluorescentGemUpConfigData3) && null != fluorescentGemUpConfigData3)
							{
								if (this.IsSameGem(fluorescentGemUpConfigData, fluorescentGemUpConfigData3))
								{
									int num3 = num2;
									int needLevelOneGoodsCount = fluorescentGemUpConfigData3._NeedLevelOneGoodsCount;
									item3 = item;
									num2 = num3 + needLevelOneGoodsCount * item3.Value;
								}
							}
						}
					}
					if (num != num2)
					{
						return EFluorescentGemUpErrorCode.GemNotEnough;
					}
					using (Dictionary<int, int>.Enumerator enumerator = upData._DecGoodsDict.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, int> item = enumerator.Current;
							GoodsData goodsData2;
							if ((goodsData2 = client.ClientData.FluorescentGemData.GemBagList.Find(delegate(GoodsData _g)
							{
								int bagIndex = _g.BagIndex;
								KeyValuePair<int, int> item4 = item;
								return bagIndex == item4.Key;
							})) == null)
							{
								return EFluorescentGemUpErrorCode.DecGoodsError;
							}
							ClientManager clientMgr = GameManager.ClientMgr;
							SocketListener mySocketListener = Global._TCPManager.MySocketListener;
							TCPClientPool tcpClientPool = Global._TCPManager.tcpClientPool;
							TCPOutPacketPool tcpOutPacketPool = Global._TCPManager.TcpOutPacketPool;
							GoodsData goodsData3 = goodsData2;
							KeyValuePair<int, int> item3 = item;
							if (!clientMgr.NotifyUseGoods(mySocketListener, tcpClientPool, tcpOutPacketPool, client, goodsData3, item3.Value, false, false))
							{
								return EFluorescentGemUpErrorCode.DecGoodsError;
							}
						}
					}
					if (upData._UpType == 0)
					{
						if (!Global.SubBindTongQianAndTongQian(client, fluorescentGemUpConfigData2._NeedGold, "荧光宝石升级"))
						{
							return EFluorescentGemUpErrorCode.GoldNotEnough;
						}
						int num4 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, fluorescentGemUpConfigData._NewGoodsID, 1, 0, "", 0, 0, 7000, "", false, 1, "荧光宝石升级", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (num4 < 0)
						{
							return EFluorescentGemUpErrorCode.AddGoodsError;
						}
						nNewGoodsDBID = num4;
						GameManager.logDBCmdMgr.AddDBLogInfo(num4, "荧光宝石", "背包宝石升级", "系统", client.ClientData.RoleName, "修改", goodsData.GoodsID, client.ClientData.ZoneID, client.strUserID, fluorescentGemUpConfigData._NewGoodsID, client.ServerId, null);
					}
					else
					{
						if (!Global.SubBindTongQianAndTongQian(client, fluorescentGemUpConfigData2._NeedGold, "荧光宝石升级"))
						{
							return EFluorescentGemUpErrorCode.GoldNotEnough;
						}
						if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
						{
							_RoleID = client.ClientData.RoleID,
							_Position = upData._Position,
							_GemType = upData._GemType
						}, 1))
						{
							return EFluorescentGemUpErrorCode.DecGoodsError;
						}
						if (!this.NotifyEquipGem(client, new FluorescentGemSaveDBData
						{
							_RoleID = client.ClientData.RoleID,
							_GoodsID = fluorescentGemUpConfigData._NewGoodsID,
							_Position = upData._Position,
							_GemType = upData._GemType
						}))
						{
							return EFluorescentGemUpErrorCode.EquipError;
						}
						this.UpdateProps(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "装备栏宝石升级", "系统", client.ClientData.RoleName, "修改", goodsData.GoodsID, client.ClientData.ZoneID, client.strUserID, fluorescentGemUpConfigData._NewGoodsID, client.ServerId, null);
					}
				}
				return EFluorescentGemUpErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemUpErrorCode.Error;
		}

		private EFluorescentGemEquipErrorCode FluorescentGemEquip(GameClient client, int nBagIndex, int nPositionIndex, int nGemType)
		{
			try
			{
				if (null == client)
				{
					return EFluorescentGemEquipErrorCode.Error;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return EFluorescentGemEquipErrorCode.Error;
				}
				GoodsData goodsData;
				if ((goodsData = client.ClientData.FluorescentGemData.GemBagList.Find((GoodsData _g) => _g.BagIndex == nBagIndex)) == null)
				{
					return EFluorescentGemEquipErrorCode.GoodsNotExist;
				}
				if (!this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
				{
					return EFluorescentGemEquipErrorCode.NotGem;
				}
				if (!this.CheckEquipPositionIndex(nPositionIndex))
				{
					return EFluorescentGemEquipErrorCode.PositionIndexError;
				}
				if (!this.CheckGemTypeIndex(nGemType))
				{
					return EFluorescentGemEquipErrorCode.GemTypeError;
				}
				FluorescentGemUpConfigData fluorescentGemUpConfigData = null;
				if (!this.FluorescentGemUpConfigDict.TryGetValue(goodsData.GoodsID, out fluorescentGemUpConfigData) || null == fluorescentGemUpConfigData)
				{
					return EFluorescentGemEquipErrorCode.GemDataError;
				}
				if (nGemType != fluorescentGemUpConfigData._GemType)
				{
					return EFluorescentGemEquipErrorCode.GemTypeError;
				}
				GoodsData goodsData2 = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(nPositionIndex, nGemType));
				if (goodsData2 != null)
				{
					EFluorescentGemUnEquipErrorCode efluorescentGemUnEquipErrorCode = this.FluorescentGemUnEquip(client, 0, nPositionIndex, nGemType);
					if (efluorescentGemUnEquipErrorCode != EFluorescentGemUnEquipErrorCode.Success)
					{
						return EFluorescentGemEquipErrorCode.UnEquipError;
					}
				}
				GoodsData goodsData3 = Global.CopyGoodsData(goodsData);
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, false))
				{
					return EFluorescentGemEquipErrorCode.DecGoodsError;
				}
				if (!this.NotifyEquipGem(client, new FluorescentGemSaveDBData
				{
					_RoleID = client.ClientData.RoleID,
					_GoodsID = goodsData3.GoodsID,
					_Position = nPositionIndex,
					_GemType = nGemType,
					_Bind = goodsData3.Binding
				}))
				{
					return EFluorescentGemEquipErrorCode.EquipError;
				}
				this.UpdateProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				return EFluorescentGemEquipErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemEquipErrorCode.Error;
		}

		private EFluorescentGemUnEquipErrorCode FluorescentGemUnEquip(GameClient client, int nUnEquipType, int nPositionIndex, int nGemType)
		{
			try
			{
				if (null == client)
				{
					return EFluorescentGemUnEquipErrorCode.Error;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(6))
				{
					return EFluorescentGemUnEquipErrorCode.Error;
				}
				switch (nUnEquipType)
				{
				case 0:
					lock (client.ClientData.FluorescentGemData)
					{
						if (this.GetFluorescentGemBagSpace(client) < 1)
						{
							return EFluorescentGemUnEquipErrorCode.BagNotEnoughOne;
						}
						if (!this.CheckEquipPositionIndex(nPositionIndex))
						{
							return EFluorescentGemUnEquipErrorCode.PositionIndexError;
						}
						if (!this.CheckGemTypeIndex(nGemType))
						{
							return EFluorescentGemUnEquipErrorCode.GemTypeError;
						}
						GoodsData goodsData;
						if ((goodsData = client.ClientData.FluorescentGemData.GemEquipList.Find((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(nPositionIndex, nGemType))) == null)
						{
							return EFluorescentGemUnEquipErrorCode.GoodsNotExist;
						}
						if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
						{
							_RoleID = client.ClientData.RoleID,
							_GoodsID = goodsData.GoodsID,
							_Position = nPositionIndex,
							_GemType = nGemType,
							_Bind = goodsData.Binding
						}, 0))
						{
							return EFluorescentGemUnEquipErrorCode.UnEquipError;
						}
					}
					break;
				case 1:
					lock (client.ClientData.FluorescentGemData)
					{
						if (this.GetFluorescentGemBagSpace(client) < 3)
						{
							return EFluorescentGemUnEquipErrorCode.BagNotEnoughThree;
						}
						if (!this.CheckEquipPositionIndex(nPositionIndex))
						{
							return EFluorescentGemUnEquipErrorCode.PositionIndexError;
						}
						List<GoodsData> list = new List<GoodsData>();
						List<int> list2 = new List<int>();
						foreach (GoodsData goodsData2 in client.ClientData.FluorescentGemData.GemEquipList)
						{
							int num;
							int item;
							this.ParsePosAndType(goodsData2.BagIndex, out num, out item);
							if (num == nPositionIndex)
							{
								list.Add(goodsData2);
								list2.Add(item);
							}
						}
						for (int i = 0; i < list.Count; i++)
						{
							GoodsData goodsData = list[i];
							if (null != goodsData)
							{
								if (!this.NotifyUnEquipGem(client, new FluorescentGemSaveDBData
								{
									_RoleID = client.ClientData.RoleID,
									_GoodsID = goodsData.GoodsID,
									_Position = nPositionIndex,
									_GemType = list2[i],
									_Bind = goodsData.Binding
								}, 0))
								{
									return EFluorescentGemUnEquipErrorCode.UnEquipError;
								}
							}
						}
					}
					break;
				}
				this.UpdateProps(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				return EFluorescentGemUnEquipErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EFluorescentGemUnEquipErrorCode.Error;
		}

		private bool NotifyEquipGem(GameClient client, FluorescentGemSaveDBData data)
		{
			bool result;
			if (client == null || null == data)
			{
				result = false;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(6))
			{
				result = false;
			}
			else
			{
				byte[] cmd = DataHelper.ObjectToBytes<FluorescentGemSaveDBData>(data);
				if (!Global.sendToDB<bool, byte[]>(10209, cmd, client.ServerId))
				{
					result = false;
				}
				else
				{
					GoodsData equipGoods = new GoodsData();
					equipGoods.GoodsID = data._GoodsID;
					equipGoods.GCount = 1;
					equipGoods.Binding = data._Bind;
					equipGoods.Site = 7001;
					equipGoods.BagIndex = this.GenerateBagIndex(data._Position, data._GemType);
					lock (client.ClientData.FluorescentGemData)
					{
						client.ClientData.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == equipGoods.BagIndex);
						client.ClientData.FluorescentGemData.GemEquipList.Add(equipGoods);
					}
					client.sendCmd<FluorescentGemEquipChangesTransferData>(997, new FluorescentGemEquipChangesTransferData
					{
						_Position = data._Position,
						_GemType = data._GemType,
						_GoodsData = equipGoods
					}, false);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "镶嵌", "系统", client.ClientData.RoleName, "修改", data._GoodsID, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
					result = true;
				}
			}
			return result;
		}

		private bool NotifyUnEquipGem(GameClient client, FluorescentGemSaveDBData data, int nOP)
		{
			bool result;
			if (client == null || null == data)
			{
				result = false;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(6))
			{
				result = false;
			}
			else if (nOP < 0 || nOP > 1)
			{
				result = false;
			}
			else
			{
				byte[] cmd = DataHelper.ObjectToBytes<FluorescentGemSaveDBData>(data);
				if (!Global.sendToDB<bool, byte[]>(10210, cmd, client.ServerId))
				{
					result = false;
				}
				else
				{
					lock (client.ClientData.FluorescentGemData)
					{
						client.ClientData.FluorescentGemData.GemEquipList.RemoveAll((GoodsData _g) => _g.BagIndex == this.GenerateBagIndex(data._Position, data._GemType));
					}
					client.sendCmd<FluorescentGemEquipChangesTransferData>(997, new FluorescentGemEquipChangesTransferData
					{
						_Position = data._Position,
						_GemType = data._GemType,
						_GoodsData = null
					}, false);
					if (nOP == 0)
					{
						int num = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, data._GoodsID, 1, 0, "", 0, data._Bind, 7000, "", true, 0, "荧光宝石卸下", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (num < 0)
						{
							return false;
						}
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光宝石", "卸下", "系统", client.ClientData.RoleName, "修改", data._GoodsID, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, null);
					result = true;
				}
			}
			return result;
		}

		public void LoadFluorescentGemConfigData()
		{
			this.LoadFluorescentGemDigConfigData();
			this.LoadFluorescentGemLevelTypeConfigData();
			this.LoadFluorescentGemUpConfigData();
		}

		public bool IsOpenFluorescentGem(GameClient client)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("FluorescentGem"))
			{
				LogManager.WriteLog(2, string.Format("版本控制未开启荧光宝石功能, RoleID={0}", client.ClientData.RoleID), null, true);
				result = false;
			}
			else
			{
				result = GlobalNew.IsGongNengOpened(client, 68, false);
			}
			return result;
		}

		public bool CheckIsFluorescentGemByGoodsID(int nGoodsID)
		{
			SystemXmlItem systemXmlItem = null;
			return GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(nGoodsID, out systemXmlItem) && systemXmlItem.GetIntValue("Categoriy", -1) == 901;
		}

		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				if (client.ClientData.FluorescentGemData == null)
				{
					client.ClientData.FluorescentGemData = new FluorescentGemData();
				}
				if (client.ClientData.FluorescentGemData.GemBagList == null)
				{
					client.ClientData.FluorescentGemData.GemBagList = new List<GoodsData>();
				}
				if (client.ClientData.FluorescentGemData.GemEquipList == null)
				{
					client.ClientData.FluorescentGemData.GemEquipList = new List<GoodsData>();
				}
				HashSet<int> hashSet = new HashSet<int>();
				foreach (GoodsData goodsData in client.ClientData.FluorescentGemData.GemBagList)
				{
					if (hashSet.Contains(goodsData.BagIndex))
					{
						this.ResetBagAllGoods(client);
						break;
					}
					hashSet.Add(goodsData.BagIndex);
				}
				this.UpdateProps(client);
			}
		}

		private void UpdateProps(GameClient client)
		{
			if (client != null)
			{
				EquipPropItem equipPropItem = new EquipPropItem();
				foreach (GoodsData goodsData in client.ClientData.FluorescentGemData.GemEquipList)
				{
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
					{
						if (this.CheckIsFluorescentGemByGoodsID(goodsData.GoodsID))
						{
							EquipPropItem equipPropItem2 = GameManager.EquipPropsMgr.FindEquipPropItem(goodsData.GoodsID);
							int num = 0;
							while (equipPropItem2 != null && num < 177)
							{
								equipPropItem.ExtProps[num] += equipPropItem2.ExtProps[num];
								num++;
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					17,
					equipPropItem
				});
			}
		}

		public bool AddFluorescentPoint(GameClient client, int nAddPoint, string reasonStr, bool notifyClient = true)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (nAddPoint <= 0)
			{
				result = false;
			}
			else
			{
				int num = client.ClientData.FluorescentPoint + nAddPoint;
				if (!this.UpdateFluorescentPoint2DB(client, num))
				{
					result = false;
				}
				else
				{
					client.ClientData.FluorescentPoint = num;
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光粉末", reasonStr, "系统", client.ClientData.RoleName, "修改", nAddPoint, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.Fluorescent, (long)nAddPoint, (long)num, reasonStr);
					if (notifyClient)
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FluorescentGem, num);
					}
					result = true;
				}
			}
			return result;
		}

		public bool DecFluorescentPoint(GameClient client, int nDecPoint, string reasonStr, bool isGM = false)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else if (nDecPoint <= 0)
			{
				result = false;
			}
			else
			{
				long num = (long)client.ClientData.FluorescentPoint;
				long num2 = num - (long)nDecPoint;
				if (num2 < -2147483648L)
				{
					result = false;
				}
				else
				{
					int num3 = client.ClientData.FluorescentPoint - nDecPoint;
					if (!isGM && num3 < 0)
					{
						result = false;
					}
					else if (!this.UpdateFluorescentPoint2DB(client, num3))
					{
						result = false;
					}
					else
					{
						client.ClientData.FluorescentPoint = num3;
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荧光粉末", reasonStr, "系统", client.ClientData.RoleName, "修改", nDecPoint, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.Fluorescent, (long)(-(long)nDecPoint), (long)num3, reasonStr);
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FluorescentGem, num3);
						result = true;
					}
				}
			}
			return result;
		}

		public bool UpdateFluorescentPoint2DB(GameClient client, int nTotalPoint)
		{
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				string s = string.Format("{0}:{1}", client.ClientData.RoleID, nTotalPoint);
				byte[] bytes = new UTF8Encoding().GetBytes(s);
				result = Global.sendToDB<bool, byte[]>(10208, bytes, client.ServerId);
			}
			return result;
		}

		public bool ModifyFluorescentPoint2DB(int rid, int nPointChg)
		{
			string s = string.Format("{0}:{1}", rid, nPointChg);
			byte[] bytes = new UTF8Encoding().GetBytes(s);
			return Global.sendToDB<bool, byte[]>(10211, bytes, 0);
		}

		public GoodsData AddFluorescentGemData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
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
			this.AddFluorescentGemData(client, goodsData);
			return goodsData;
		}

		public void AddFluorescentGemData(GameClient client, GoodsData gd)
		{
			if (null != gd)
			{
				if (null == client.ClientData.FluorescentGemData)
				{
					client.ClientData.FluorescentGemData = new FluorescentGemData();
				}
				lock (client.ClientData.FluorescentGemData)
				{
					client.ClientData.FluorescentGemData.GemBagList.Add(gd);
				}
			}
		}

		public void RemoveFluorescentGemData(GameClient client, GoodsData goodsData)
		{
			if (7000 == goodsData.Site)
			{
				lock (client.ClientData.FluorescentGemData)
				{
					client.ClientData.FluorescentGemData.GemBagList.RemoveAll((GoodsData _g) => _g.BagIndex == goodsData.BagIndex);
				}
			}
		}

		public int GetIdleSlotOfFluorescentGemBag(GameClient client)
		{
			int num = -1;
			int result;
			if (client.ClientData.FluorescentGemData == null || null == client.ClientData.FluorescentGemData)
			{
				result = num;
			}
			else if (null == client.ClientData.GoodsDataList)
			{
				result = num;
			}
			else
			{
				List<int> usedBagIndex = new List<int>();
				client.ClientData.FluorescentGemData.GemBagList.ForEach(delegate(GoodsData _g)
				{
					usedBagIndex.Add(_g.BagIndex);
				});
				for (int i = 0; i < 220; i++)
				{
					if (usedBagIndex.IndexOf(i) < 0)
					{
						num = i;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.FluorescentGemData.GemBagList.Count <= 220;
		}

		public GoodsData GetGoodsByID(GameClient client, int goodsID, int bingding, string startTime, string endTime, ref int startIndex)
		{
			GoodsData result;
			if (null == client)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				lock (client.ClientData.FluorescentGemData)
				{
					foreach (GoodsData goodsData in client.ClientData.FluorescentGemData.GemEquipList)
					{
						if (goodsData.GoodsID == goodsID && goodsData.Binding == bingding && Global.DateTimeEqual(goodsData.Endtime, endTime) && Global.DateTimeEqual(goodsData.Starttime, startTime))
						{
							list.Add(goodsData);
						}
					}
					if (list == null || list.Count <= 0)
					{
						return null;
					}
					list.Sort((GoodsData x, GoodsData y) => x.BagIndex - y.BagIndex);
					if (startIndex >= list.Count)
					{
						return null;
					}
					int num = startIndex;
					if (num < list.Count)
					{
						startIndex = num + 1;
						return list[num];
					}
				}
				result = null;
			}
			return result;
		}

		public TCPProcessCmdResults ProcessResetBagCmd(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
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
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				this.ResetBagAllGoods(gameClient);
				gameClient.sendCmd<Dictionary<int, GoodsData>>(nID, this.GetBagDict(gameClient), false);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessFluorescentGemDig(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int nLevelType = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					gameClient.sendCmd<FluorescentGemDigTransferData>(nID, new FluorescentGemDigTransferData
					{
						_Result = -2
					}, false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				List<int> gemList = null;
				Dictionary<int, int> gemNumDict = null;
				EFluorescentGemDigErrorCode result;
				if (num2 == 2)
				{
					result = this.FluorescentGemDig_BigNum(gameClient, nLevelType, num2, out gemNumDict);
				}
				else
				{
					result = this.FluorescentGemDig(gameClient, nLevelType, num2, out gemList);
				}
				gameClient.sendCmd<FluorescentGemDigTransferData>(nID, new FluorescentGemDigTransferData
				{
					_Result = (int)result,
					_GemList = gemList,
					_GemNumDict = gemNumDict
				}, false);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessFluorescentGemResolve(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int nBagIndex = Convert.ToInt32(array[1]);
				int nResolveCount = Convert.ToInt32(array[2]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					gameClient.sendCmd(nID, string.Format("{0}", -2), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				EFluorescentGemResolveErrorCode efluorescentGemResolveErrorCode = this.FluorescentGemResolve(gameClient, nBagIndex, nResolveCount);
				string data2 = string.Format("{0}", (int)efluorescentGemResolveErrorCode);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessFluorescentGemUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			FluorescentGemUpTransferData fluorescentGemUpTransferData = null;
			try
			{
				fluorescentGemUpTransferData = DataHelper.BytesToObject<FluorescentGemUpTransferData>(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				if (null == fluorescentGemUpTransferData)
				{
					LogManager.WriteLog(2, string.Format("指令结构解析错误:FluorescentGemUpTransferData, CMD={0}", (TCPGameServerCmds)nID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != fluorescentGemUpTransferData._RoleID)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fluorescentGemUpTransferData._RoleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}", -2, -1), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int num = -1;
				EFluorescentGemUpErrorCode efluorescentGemUpErrorCode = this.FluorescentGemUp(gameClient, fluorescentGemUpTransferData, out num);
				string data2 = string.Format("{0}:{1}", (int)efluorescentGemUpErrorCode, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessFluorescentGemEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 4)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int nBagIndex = Convert.ToInt32(array[1]);
				int nPositionIndex = Convert.ToInt32(array[2]);
				int nGemType = Convert.ToInt32(array[3]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, -2.ToString(), nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				EFluorescentGemEquipErrorCode efluorescentGemEquipErrorCode = this.FluorescentGemEquip(gameClient, nBagIndex, nPositionIndex, nGemType);
				string data2 = string.Format("{0}", (int)efluorescentGemEquipErrorCode);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessFluorescentGemUnEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3 && array.Length != 4)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int nPositionIndex = Convert.ToInt32(array[1]);
				int num2 = Convert.ToInt32(array[2]);
				int nGemType = 0;
				if (num2 == 0)
				{
					nGemType = Convert.ToInt32(array[3]);
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenFluorescentGem(gameClient))
				{
					gameClient.sendCmd(nID, string.Format("{0}", -2), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				EFluorescentGemUnEquipErrorCode efluorescentGemUnEquipErrorCode = this.FluorescentGemUnEquip(gameClient, num2, nPositionIndex, nGemType);
				string data2 = string.Format("{0}", (int)efluorescentGemUnEquipErrorCode);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public void GMClearGemBag(GameClient client)
		{
			if (null != client)
			{
				List<GoodsData> list = new List<GoodsData>(client.ClientData.FluorescentGemData.GemBagList);
				for (int i = 0; i < list.Count; i++)
				{
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, list[i], list[i].GCount, false, false))
					{
					}
				}
				list.Clear();
				client.ClientData.FluorescentGemData.GemEquipList.Clear();
			}
		}

		public void GMAddFluorescentPoint(GameClient client, int nPoint)
		{
			if (null != client)
			{
				this.AddFluorescentPoint(client, nPoint, "GM命令增加", true);
			}
		}

		public void GMDecFluorescentPoint(GameClient client, int nPoint)
		{
			if (null != client)
			{
				this.DecFluorescentPoint(client, nPoint, "GM命令减少", true);
			}
		}

		private Dictionary<int, FluorescentGemLevelTypeConfigData> FluorescentGemLevelTypeConfigDict = new Dictionary<int, FluorescentGemLevelTypeConfigData>();

		private Dictionary<int, List<FluorescentGemDigConfigData>> FluorescentGemDigConfigDict = new Dictionary<int, List<FluorescentGemDigConfigData>>();

		private Dictionary<int, FluorescentGemUpConfigData> FluorescentGemUpConfigDict = new Dictionary<int, FluorescentGemUpConfigData>();
	}
}
