using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.TuJian
{
	public class TuJianManager : SingletonTemplate<TuJianManager>
	{
		private TuJianManager()
		{
		}

		public void LoadConfig()
		{
			bool flag = false;
			if (!this.loadTuJianType() || !this.loadTuJianItem())
			{
				flag = true;
			}
			bool flag2 = true;
			if (flag2 && !flag)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				foreach (KeyValuePair<int, TuJianItem> keyValuePair in this.TuJianItems)
				{
					int key = keyValuePair.Key;
					int num = keyValuePair.Value.TypeID;
					if (!dictionary.ContainsKey(num))
					{
						dictionary.Add(num, 0);
					}
					Dictionary<int, int> dictionary2;
					int key2;
					(dictionary2 = dictionary)[key2 = num] = dictionary2[key2] + 1;
				}
				foreach (KeyValuePair<int, int> keyValuePair2 in dictionary)
				{
					int num = keyValuePair2.Key;
					int value = keyValuePair2.Value;
					TuJianType tuJianType = null;
					if (!this.TuJianTypes.TryGetValue(num, out tuJianType) || tuJianType.ItemCnt != value)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				LogManager.WriteLog(2, "Config/TuJianType.xml Config/TuJianItems.xml 配置文件出错，请检查文件是否存在或者配置的item个数是否一致", null, true);
			}
		}

		private bool loadTuJianType()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath("Config/TuJianType.xml"));
				if (xelement == null)
				{
					LogManager.WriteLog(2, string.Format("{0}不存在!", "Config/TuJianType.xml"), null, true);
					return false;
				}
				this.TuJianTypes.Clear();
				IEnumerable<XElement> enumerable = xelement.Elements("TuJian").Elements<XElement>();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						TuJianType tuJianType = new TuJianType();
						tuJianType.TypeID = (int)Global.GetSafeAttributeDouble(xelement2, "ID");
						tuJianType.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						tuJianType.ItemCnt = (int)Global.GetSafeAttributeDouble(xelement2, "TuJianNum");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "KaiQiLevel");
						string[] array = safeAttributeStr.Split(new char[]
						{
							','
						});
						tuJianType.OpenChangeLife = Global.SafeConvertToInt32(array[0]);
						tuJianType.OpenLevel = Global.SafeConvertToInt32(array[1]);
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "ShuXiangJiaCheng");
						tuJianType.AttrValue = this.analyseToAttrValues(safeAttributeStr2);
						this.TuJianTypes.Add(tuJianType.TypeID, tuJianType);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0}读取出错!", "Config/TuJianType.xml"), ex, true);
				return false;
			}
			return true;
		}

		private bool loadTuJianItem()
		{
			try
			{
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath("Config/TuJianItems.xml"));
				if (null == xelement)
				{
					LogManager.WriteLog(2, string.Format("{0}不存在!", "Config/TuJianItems.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (null != xelement2)
					{
						TuJianItem tuJianItem = new TuJianItem();
						tuJianItem.TypeID = (int)Global.GetSafeAttributeDouble(xelement2, "Type");
						tuJianItem.ItemID = (int)Global.GetSafeAttributeDouble(xelement2, "ID");
						tuJianItem.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "NeedGoods");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							tuJianItem.CostGoodsID = Global.SafeConvertToInt32(array[0]);
							tuJianItem.CostGoodsCnt = Global.SafeConvertToInt32(array[1]);
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "ShuXing");
						tuJianItem.AttrValue = this.analyseToAttrValues(safeAttributeStr2);
						this.TuJianItems.Add(tuJianItem.ItemID, tuJianItem);
						if (!this.TuJianTypes.ContainsKey(tuJianItem.TypeID))
						{
							LogManager.WriteLog(2, string.Format("{0}配置了不存在的图鉴类型Type={1}", "Config/TuJianItems.xml", tuJianItem.TypeID), null, true);
							return false;
						}
						this.TuJianTypes[tuJianItem.TypeID].ItemList.Add(tuJianItem.ItemID);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0}读取出错!", "Config/TuJianItems.xml"), ex, true);
				return false;
			}
			return true;
		}

		private _AttrValue analyseToAttrValues(string strAttrs)
		{
			_AttrValue result;
			if (string.IsNullOrEmpty(strAttrs))
			{
				result = null;
			}
			else
			{
				string[] array = strAttrs.Split(new char[]
				{
					'|'
				});
				if (array == null || array.Length == 0)
				{
					result = null;
				}
				else
				{
					_AttrValue attrValue = new _AttrValue();
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						if (array3 != null && array3.Length == 2)
						{
							string a = array3[0].ToLower();
							string text2 = array3[1];
							string[] array4 = text2.Split(new char[]
							{
								'-'
							});
							if (a == "defense")
							{
								if (array4 != null && array4.Length == 2)
								{
									attrValue.MinDefense = Global.SafeConvertToInt32(array4[0]);
									attrValue.MaxDefense = Global.SafeConvertToInt32(array4[1]);
								}
							}
							else if (a == "mdefense")
							{
								if (array4 != null && array4.Length == 2)
								{
									attrValue.MinMDefense = Global.SafeConvertToInt32(array4[0]);
									attrValue.MaxMDefense = Global.SafeConvertToInt32(array4[1]);
								}
							}
							else if (a == "attack")
							{
								if (array4 != null && array4.Length == 2)
								{
									attrValue.MinAttack = Global.SafeConvertToInt32(array4[0]);
									attrValue.MaxAttack = Global.SafeConvertToInt32(array4[1]);
								}
							}
							else if (a == "mattack")
							{
								if (array4 != null && array4.Length == 2)
								{
									attrValue.MinMAttack = Global.SafeConvertToInt32(array4[0]);
									attrValue.MaxMAttack = Global.SafeConvertToInt32(array4[1]);
								}
							}
							else if (a == "hitv")
							{
								attrValue.HitV = Global.SafeConvertToInt32(array4[0]);
							}
							else if (a == "dodge")
							{
								attrValue.Dodge = Global.SafeConvertToInt32(array4[0]);
							}
							else if (a == "maxlifev")
							{
								attrValue.MaxLifeV = Global.SafeConvertToInt32(array4[0]);
							}
						}
					}
					result = attrValue;
				}
			}
			return result;
		}

		public void UpdateTuJianProps(GameClient client)
		{
			if (client != null)
			{
				if (client.ClientData.PictureJudgeReferInfo != null && client.ClientData.PictureJudgeReferInfo.Count != 0)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					_AttrValue attrValue = new _AttrValue();
					foreach (KeyValuePair<int, int> keyValuePair in client.ClientData.PictureJudgeReferInfo)
					{
						int key = keyValuePair.Key;
						int value = keyValuePair.Value;
						TuJianItem tuJianItem = null;
						if (this.TuJianItems.TryGetValue(key, out tuJianItem))
						{
							if (value >= tuJianItem.CostGoodsCnt)
							{
								if (!dictionary.ContainsKey(tuJianItem.TypeID))
								{
									dictionary.Add(tuJianItem.TypeID, 0);
								}
								Dictionary<int, int> dictionary2;
								int typeID;
								(dictionary2 = dictionary)[typeID = tuJianItem.TypeID] = dictionary2[typeID] + 1;
								attrValue.Add(tuJianItem.AttrValue);
								if (client.ClientData.ActivedTuJianItem != null && !client.ClientData.ActivedTuJianItem.Contains(key))
								{
									client.ClientData.ActivedTuJianItem.Add(key);
								}
							}
						}
					}
					foreach (KeyValuePair<int, int> keyValuePair in dictionary)
					{
						TuJianType tuJianType = null;
						if (this.TuJianTypes.TryGetValue(keyValuePair.Key, out tuJianType))
						{
							if (keyValuePair.Value >= tuJianType.ItemCnt)
							{
								attrValue.Add(tuJianType.AttrValue);
								if (client.ClientData.ActivedTuJianType != null && !client.ClientData.ActivedTuJianType.Contains(keyValuePair.Key))
								{
									client.ClientData.ActivedTuJianType.Add(keyValuePair.Key);
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						7,
						attrValue.MinAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						8,
						attrValue.MaxAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						9,
						attrValue.MinMAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						10,
						attrValue.MaxMAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						3,
						attrValue.MinDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						4,
						attrValue.MaxDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						5,
						attrValue.MinMDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						6,
						attrValue.MaxMDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						18,
						attrValue.HitV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						13,
						attrValue.MaxLifeV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						19,
						attrValue.Dodge
					});
				}
			}
		}

		public void HandleActiveTuJian(GameClient client, string[] itemArr)
		{
			if (itemArr != null && itemArr.Length != 0 && client != null)
			{
				bool flag = false;
				foreach (string value in itemArr)
				{
					int num = Convert.ToInt32(value);
					TuJianItem tuJianItem = null;
					TuJianType tuJianType = null;
					if (this.TuJianItems.TryGetValue(num, out tuJianItem) && this.TuJianTypes.TryGetValue(tuJianItem.TypeID, out tuJianType))
					{
						if (client.ClientData.ChangeLifeCount >= tuJianType.OpenChangeLife && (client.ClientData.ChangeLifeCount != tuJianType.OpenChangeLife || client.ClientData.Level >= tuJianType.OpenLevel))
						{
							int num2 = 0;
							if (client.ClientData.PictureJudgeReferInfo.ContainsKey(num))
							{
								num2 = client.ClientData.PictureJudgeReferInfo[num];
							}
							if (num2 < tuJianItem.CostGoodsCnt)
							{
								int val = tuJianItem.CostGoodsCnt - num2;
								int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, tuJianItem.CostGoodsID);
								if (totalGoodsCountByID > 0)
								{
									int num3 = Math.Min(val, totalGoodsCountByID);
									bool flag2 = false;
									bool flag3 = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, tuJianItem.CostGoodsID, num3, false, out flag2, out flag3, false))
									{
										string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, num, num2 + num3);
										string[] array = Global.ExecuteDBCmd(10155, strcmd, client.ServerId);
										if (array == null || array.Length != 1 || Convert.ToInt32(array[0]) <= 0)
										{
											LogManager.WriteLog(2, string.Format("角色RoleID={0}，RoleName={1} 激活图鉴Item={2}时，与db通信失败，物品已扣除GoodsID={3},Cnt={4}", new object[]
											{
												client.ClientData.RoleID,
												client.ClientData.RoleName,
												num,
												tuJianItem.CostGoodsID,
												num3
											}), null, true);
										}
										else
										{
											flag = true;
											if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(num))
											{
												client.ClientData.PictureJudgeReferInfo.Add(num, num2 + num3);
											}
											else
											{
												client.ClientData.PictureJudgeReferInfo[num] = num2 + num3;
											}
											ProcessTask.ProcessAddTaskVal(client, TaskTypes.JiHuoTuJian, -1, 1, new object[0]);
										}
									}
								}
							}
						}
					}
				}
				if (flag)
				{
					this.UpdateTuJianProps(client);
					SingletonTemplate<GuardStatueManager>.Instance().OnActiveTuJian(client);
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		public bool GM_OneKeyActiveTuJianType(GameClient client, int typeId, out string failedMsg)
		{
			failedMsg = string.Empty;
			bool result;
			if (client == null)
			{
				failedMsg = "unknown";
				result = false;
			}
			else
			{
				TuJianType tuJianType = null;
				if (!this.TuJianTypes.TryGetValue(typeId, out tuJianType))
				{
					failedMsg = "图鉴类型找不到: " + typeId.ToString();
					result = false;
				}
				else if (client.ClientData.ChangeLifeCount < tuJianType.OpenChangeLife || (client.ClientData.ChangeLifeCount == tuJianType.OpenChangeLife && client.ClientData.Level < tuJianType.OpenLevel))
				{
					failedMsg = string.Concat(new object[]
					{
						"该项图鉴未开启，类型=",
						typeId.ToString(),
						" ,需求转生：",
						tuJianType.OpenChangeLife,
						" , 等级：",
						tuJianType.OpenLevel
					});
					result = false;
				}
				else
				{
					bool flag = false;
					foreach (int num in tuJianType.ItemList)
					{
						TuJianItem tuJianItem = null;
						if (this.TuJianItems.TryGetValue(num, out tuJianItem))
						{
							if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(num) || client.ClientData.PictureJudgeReferInfo[num] < tuJianItem.CostGoodsCnt)
							{
								string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, num, tuJianItem.CostGoodsCnt);
								string[] array = Global.ExecuteDBCmd(10155, strcmd, client.ServerId);
								if (array == null || array.Length != 1 || Convert.ToInt32(array[0]) <= 0)
								{
									failedMsg = "数据库异常";
									return false;
								}
								flag = true;
								if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(num))
								{
									client.ClientData.PictureJudgeReferInfo.Add(num, tuJianItem.CostGoodsCnt);
								}
								else
								{
									client.ClientData.PictureJudgeReferInfo[num] = tuJianItem.CostGoodsCnt;
								}
							}
						}
					}
					if (flag)
					{
						client.sendCmd(DataHelper.ObjectToTCPOutPacket<Dictionary<int, int>>(client.ClientData.PictureJudgeReferInfo, Global._TCPManager.TcpOutPacketPool, 612), true);
						this.UpdateTuJianProps(client);
						SingletonTemplate<GuardStatueManager>.Instance().OnActiveTuJian(client);
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					result = true;
				}
			}
			return result;
		}

		private const string TuJianType_fileName = "Config/TuJianType.xml";

		private const string TuJianItem_fileName = "Config/TuJianItems.xml";

		private Dictionary<int, TuJianType> TuJianTypes = new Dictionary<int, TuJianType>();

		private Dictionary<int, TuJianItem> TuJianItems = new Dictionary<int, TuJianItem>();
	}
}
