using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Reborn
{
	internal class RebornStamp
	{
		public static RebornStamp getInstance()
		{
			return RebornStamp.instance;
		}

		public static bool ParseYinJiConfig()
		{
			string text = Global.GameResPath(RebornStampConsts.RebornStampZhu);
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				Dictionary<MainAttrType, Dictionary<int, int>> dictionary = new Dictionary<MainAttrType, Dictionary<int, int>>();
				Dictionary<MainAttrType, List<int>> dictionary2 = new Dictionary<MainAttrType, List<int>>();
				Dictionary<int, ChongShengYinJiZhu> dictionary3 = new Dictionary<int, ChongShengYinJiZhu>();
				Dictionary<MainAttrType, Dictionary<int, int>> dictionary4 = new Dictionary<MainAttrType, Dictionary<int, int>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					ChongShengYinJiZhu chongShengYinJiZhu = new ChongShengYinJiZhu();
					List<int> list = new List<int>();
					Dictionary<int, double> dictionary5 = new Dictionary<int, double>();
					chongShengYinJiZhu.ItemID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					chongShengYinJiZhu.MainType = (MainAttrType)Convert.ToInt32(Global.GetSafeAttributeStr(xml, "TypeZhu"));
					chongShengYinJiZhu.NeedLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "NeedLevel"));
					string[] array = Global.GetSafeAttributeStr(xml, "TypeFu").Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(Convert.ToInt32(array[i]));
					}
					chongShengYinJiZhu.MinorType = list;
					chongShengYinJiZhu.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Level"));
					string[] array2 = Global.GetSafeAttributeStr(xml, "ShuXing").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						dictionary5.Add((int)ConfigParser.GetPropIndexByPropName(array2[i].Split(new char[]
						{
							','
						})[0]), Convert.ToDouble(array2[i].Split(new char[]
						{
							','
						})[1]));
					}
					chongShengYinJiZhu.AttrList = dictionary5;
					dictionary3.Add(chongShengYinJiZhu.ItemID, chongShengYinJiZhu);
					if (dictionary.ContainsKey(chongShengYinJiZhu.MainType))
					{
						dictionary[chongShengYinJiZhu.MainType].Add(chongShengYinJiZhu.Level, chongShengYinJiZhu.ItemID);
					}
					else
					{
						Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
						dictionary6.Add(chongShengYinJiZhu.Level, chongShengYinJiZhu.ItemID);
						dictionary.Add(chongShengYinJiZhu.MainType, dictionary6);
					}
					if (dictionary4.ContainsKey(chongShengYinJiZhu.MainType))
					{
						dictionary4[chongShengYinJiZhu.MainType].Add(chongShengYinJiZhu.NeedLevel, chongShengYinJiZhu.ItemID);
					}
					else
					{
						Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
						dictionary6.Add(chongShengYinJiZhu.NeedLevel, chongShengYinJiZhu.ItemID);
						dictionary4.Add(chongShengYinJiZhu.MainType, dictionary6);
					}
					if (!dictionary2.ContainsKey(chongShengYinJiZhu.MainType))
					{
						dictionary2.Add(chongShengYinJiZhu.MainType, list);
					}
				}
				RebornStamp.CurrMainYinJi = dictionary;
				RebornStamp.TypeMap = dictionary2;
				RebornStamp.MainYinJi = dictionary3;
				RebornStamp.MainYinJiLevelUp = dictionary4;
				if (RebornStamp.CurrMainYinJi == null || RebornStamp.TypeMap == null || RebornStamp.MainYinJi == null || RebornStamp.MainYinJiLevelUp == null)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			text = Global.GameResPath(RebornStampConsts.RebornStampZi);
			xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				Dictionary<int, Dictionary<int, int>> dictionary7 = new Dictionary<int, Dictionary<int, int>>();
				Dictionary<int, int> dictionary8 = new Dictionary<int, int>();
				Dictionary<int, ChongShengYinJiZi> dictionary9 = new Dictionary<int, ChongShengYinJiZi>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					ChongShengYinJiZi chongShengYinJiZi = new ChongShengYinJiZi();
					List<int> list = new List<int>();
					Dictionary<int, double> dictionary5 = new Dictionary<int, double>();
					chongShengYinJiZi.ItemID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					chongShengYinJiZi.MinorType = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Type"));
					chongShengYinJiZi.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Level"));
					string[] array2 = Global.GetSafeAttributeStr(xml, "ShuXing").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array2.Length; i++)
					{
						string[] array3 = array2[i].Split(new char[]
						{
							','
						});
						if (array3.Length == 1)
						{
							dictionary5.Add(0, 0.0);
						}
						else
						{
							dictionary5.Add((int)ConfigParser.GetPropIndexByPropName(array3[0]), Convert.ToDouble(array3[1]));
						}
					}
					chongShengYinJiZi.AttrList = dictionary5;
					dictionary9.Add(chongShengYinJiZi.ItemID, chongShengYinJiZi);
					if (dictionary8.ContainsKey(chongShengYinJiZi.MinorType))
					{
						if (dictionary8[chongShengYinJiZi.MinorType] < chongShengYinJiZi.Level)
						{
							dictionary8[chongShengYinJiZi.MinorType] = chongShengYinJiZi.Level;
						}
					}
					else
					{
						dictionary8.Add(chongShengYinJiZi.MinorType, chongShengYinJiZi.Level);
					}
					if (dictionary7.ContainsKey(chongShengYinJiZi.MinorType))
					{
						dictionary7[chongShengYinJiZi.MinorType].Add(chongShengYinJiZi.Level, chongShengYinJiZi.ItemID);
					}
					else
					{
						Dictionary<int, int> dictionary6 = new Dictionary<int, int>();
						dictionary6.Add(chongShengYinJiZi.Level, chongShengYinJiZi.ItemID);
						dictionary7.Add(chongShengYinJiZi.MinorType, dictionary6);
					}
				}
				RebornStamp.MinorYinJiLevelUp = dictionary7;
				RebornStamp.MinorLevelLimit = dictionary8;
				RebornStamp.MinorYinJi = dictionary9;
				if (RebornStamp.MinorYinJiLevelUp == null || RebornStamp.MinorLevelLimit == null || RebornStamp.MinorYinJi == null)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			List<int> list2 = new List<int>();
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ChongShengYinJiChongZhi", ',');
			foreach (int item in paramValueIntArrayByName)
			{
				list2.Add(item);
			}
			RebornStamp.YinJiReset = list2;
			return RebornStamp.YinJiReset != null;
		}

		public static bool CheckTypeMatch(RebornStampData curr, int StampID, int StampType, out int Index, out int MainAttr)
		{
			Index = 0;
			MainAttr = 0;
			bool result;
			if (curr == null || curr.StampInfo == null || curr.StampInfo.Count < 0 || curr.StampInfo.Count > 16)
			{
				result = false;
			}
			else if (!RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[0]) || !RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[8]))
			{
				result = false;
			}
			else if (!RebornStamp.MinorYinJi.ContainsKey(StampID) || RebornStamp.MinorYinJi[StampID].MinorType != StampType)
			{
				result = false;
			}
			else
			{
				bool flag = false;
				int i = 2;
				int num = 10;
				while (i < 8)
				{
					if (StampType == curr.StampInfo[i])
					{
						flag = true;
						MainAttr = 1;
						Index = i;
						break;
					}
					if (StampType == curr.StampInfo[num])
					{
						flag = true;
						MainAttr = 2;
						Index = num;
						break;
					}
					i += 2;
					num += 2;
				}
				if (!flag)
				{
					result = false;
				}
				else
				{
					int num2 = 0;
					bool[] array = new bool[3];
					if (MainAttr == 1 && RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[0]))
					{
						for (i = 2; i < 8; i += 2)
						{
							foreach (int num3 in RebornStamp.TypeMap[(MainAttrType)curr.StampInfo[0]])
							{
								if (curr.StampInfo[i] == 0)
								{
									array[num2] = true;
								}
								else if (curr.StampInfo[i] == num3)
								{
									array[num2] = true;
									break;
								}
							}
							num2++;
						}
					}
					if (MainAttr == 2 && RebornStamp.TypeMap.ContainsKey((MainAttrType)curr.StampInfo[8]))
					{
						for (i = 10; i < 16; i += 2)
						{
							foreach (int num3 in RebornStamp.TypeMap[(MainAttrType)curr.StampInfo[8]])
							{
								if (curr.StampInfo[i] == 0)
								{
									array[num2] = true;
								}
								else if (curr.StampInfo[i] == num3)
								{
									array[num2] = true;
									break;
								}
							}
							num2++;
						}
					}
					bool[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						if (!array2[j])
						{
							flag = false;
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public static int GetCurrMinorLevelItemID(RebornStampData dbInfo, int Index)
		{
			int result;
			if (dbInfo.StampInfo[Index] == 0)
			{
				result = 0;
			}
			else
			{
				result = RebornStamp.MinorYinJiLevelUp[dbInfo.StampInfo[Index]][dbInfo.StampInfo[Index + 1]];
			}
			return result;
		}

		public static int GetCurrMainLevelItemID(RebornStampData dbInfo, int Index)
		{
			int result;
			if (dbInfo.StampInfo[Index] == 0)
			{
				result = 0;
			}
			else
			{
				result = RebornStamp.CurrMainYinJi[(MainAttrType)dbInfo.StampInfo[Index]][dbInfo.StampInfo[Index + 1]];
			}
			return result;
		}

		public static int GetMainYinJiLevelUpNum(RebornStampData dbInfo, int Index)
		{
			int num = 0;
			int result;
			if (Index != 0 && Index != 8)
			{
				result = num;
			}
			else
			{
				int currMainLevelItemID = RebornStamp.GetCurrMainLevelItemID(dbInfo, Index);
				if (Index == 0)
				{
					foreach (KeyValuePair<int, int> keyValuePair in RebornStamp.MainYinJiLevelUp[(MainAttrType)dbInfo.StampInfo[Index]])
					{
						if (keyValuePair.Value > currMainLevelItemID)
						{
							if (dbInfo.StampInfo[3] >= keyValuePair.Key && dbInfo.StampInfo[5] >= keyValuePair.Key && dbInfo.StampInfo[7] >= keyValuePair.Key)
							{
								num++;
							}
							if (num > 0 && (dbInfo.StampInfo[3] < keyValuePair.Key || dbInfo.StampInfo[5] < keyValuePair.Key || dbInfo.StampInfo[7] < keyValuePair.Key))
							{
								break;
							}
						}
					}
				}
				else if (Index == 8)
				{
					foreach (KeyValuePair<int, int> keyValuePair in RebornStamp.MainYinJiLevelUp[(MainAttrType)dbInfo.StampInfo[Index]])
					{
						if (keyValuePair.Value > currMainLevelItemID)
						{
							if (dbInfo.StampInfo[11] >= keyValuePair.Key && dbInfo.StampInfo[13] >= keyValuePair.Key && dbInfo.StampInfo[15] >= keyValuePair.Key)
							{
								num++;
							}
							if (num > 0 && (dbInfo.StampInfo[11] < keyValuePair.Key || dbInfo.StampInfo[13] < keyValuePair.Key || dbInfo.StampInfo[15] < keyValuePair.Key))
							{
								break;
							}
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static bool IsMainLevelUp(RebornStampData data, int MainAttr, out int ZhuID)
		{
			bool result;
			if (MainAttr == 1)
			{
				int num = RebornStamp.CurrMainYinJi[(MainAttrType)data.StampInfo[0]][data.StampInfo[1]];
				ZhuID = num;
				ChongShengYinJiZhu chongShengYinJiZhu;
				if (!RebornStamp.MainYinJi.TryGetValue(num + 1, out chongShengYinJiZhu))
				{
					result = false;
				}
				else
				{
					lock (RebornStamp.MainYinJi)
					{
						for (int i = 2; i < 8; i += 2)
						{
							if (RebornStamp.MainYinJi[num + 1].NeedLevel > data.StampInfo[i + 1])
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
			else if (MainAttr == 2)
			{
				int num = RebornStamp.CurrMainYinJi[(MainAttrType)data.StampInfo[8]][data.StampInfo[9]];
				ZhuID = num;
				ChongShengYinJiZhu chongShengYinJiZhu;
				if (!RebornStamp.MainYinJi.TryGetValue(num + 1, out chongShengYinJiZhu))
				{
					result = false;
				}
				else
				{
					lock (RebornStamp.MainYinJi)
					{
						for (int i = 10; i < 16; i += 2)
						{
							if (RebornStamp.MainYinJi[num + 1].NeedLevel > data.StampInfo[i + 1])
							{
								return false;
							}
						}
					}
					result = true;
				}
			}
			else
			{
				ZhuID = -1;
				result = false;
			}
			return result;
		}

		public void RefreshProps(GameClient client)
		{
			double[] array = new double[177];
			try
			{
				if (client.ClientData.RebornYinJi != null)
				{
					if (client.ClientData.RebornYinJi.StampInfo != null)
					{
						int num = 0;
						int num2 = 8;
						while (num < 8 && num2 < 16)
						{
							bool flag = false;
							if (num == 0)
							{
								int num3 = RebornStamp.GetCurrMainLevelItemID(client.ClientData.RebornYinJi, 0);
								if (num3 == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> keyValuePair in RebornStamp.MainYinJi[num3].AttrList)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								flag = true;
							}
							if (num2 == 8)
							{
								int num3 = RebornStamp.GetCurrMainLevelItemID(client.ClientData.RebornYinJi, 8);
								if (num3 == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> keyValuePair in RebornStamp.MainYinJi[num3].AttrList)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								flag = true;
							}
							if (!flag)
							{
								int num3 = RebornStamp.GetCurrMinorLevelItemID(client.ClientData.RebornYinJi, num);
								if (num3 == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> keyValuePair in RebornStamp.MinorYinJi[num3].AttrList)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
								num3 = RebornStamp.GetCurrMinorLevelItemID(client.ClientData.RebornYinJi, num2);
								if (num3 == 0)
								{
									break;
								}
								foreach (KeyValuePair<int, double> keyValuePair in RebornStamp.MinorYinJi[num3].AttrList)
								{
									array[keyValuePair.Key] += keyValuePair.Value;
								}
							}
							num += 2;
							num2 += 2;
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.RebornYinJi,
					array
				});
			}
		}

		private static bool InsertRebornYinJi(GameClient client, string YinJiInfo)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				YinJiInfo,
				0,
				0
			});
			string[] array;
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14115, strcmd, out array, client.ServerId);
			return tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED;
		}

		private static List<int> YinJiUpdateInfo(int StampType1, int StampType2)
		{
			List<int> list = new List<int>();
			lock (list)
			{
				if (RebornStamp.TypeMap.ContainsKey((MainAttrType)StampType1))
				{
					list.Add(StampType1);
					list.Add(1);
					foreach (int item in RebornStamp.TypeMap[(MainAttrType)StampType1])
					{
						list.Add(item);
						list.Add(0);
					}
				}
				if (RebornStamp.TypeMap.ContainsKey((MainAttrType)StampType2))
				{
					list.Add(StampType2);
					list.Add(1);
					foreach (int item in RebornStamp.TypeMap[(MainAttrType)StampType2])
					{
						list.Add(item);
						list.Add(0);
					}
				}
			}
			return list;
		}

		private static string MakeYinJiUpdateInfo(List<int> UpdateInfo)
		{
			string result;
			if (UpdateInfo.Count != 16)
			{
				result = "";
			}
			else
			{
				string text = "";
				int num = 0;
				foreach (int num2 in UpdateInfo)
				{
					num++;
					text += num2.ToString();
					if (num == 8)
					{
						text += "|";
					}
					else if (num < UpdateInfo.Count)
					{
						text += "_";
					}
				}
				result = text;
			}
			return result;
		}

		private static string MakeYinJiUpdateInfoByType(int StampType1, int StampType2)
		{
			return RebornStamp.MakeYinJiUpdateInfo(RebornStamp.YinJiUpdateInfo(StampType1, StampType2));
		}

		private static bool UpdateRebornYinJiInfo(GameClient client, string UpdateInfo, int UsePoint, int ResetNum)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				UpdateInfo,
				UsePoint,
				ResetNum
			});
			string[] array;
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 14116, strcmd, out array, client.ServerId);
			return tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED;
		}

		private static bool GetRebornYinJiInfo(GameClient client, out RebornStampData data)
		{
			string cmd = string.Format("{0}", client.ClientData.RoleID);
			data = Global.sendToDB<RebornStampData, string>(14117, cmd, client.ServerId);
			return data.RoleID == client.ClientData.RoleID && data != null;
		}

		public static ResOpcode ProcessRebornYinJiLevelUp(GameClient client, int RoldID, int StampID, int StampType, int UpNum, out int IDZhu, out int OutMainYinJiID1, out int OutUsePoint)
		{
			IDZhu = -1;
			OutMainYinJiID1 = 0;
			OutUsePoint = 0;
			RebornStampData rebornStampData;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out rebornStampData))
			{
				result = ResOpcode.ChooseGetInfoYinJiNotActive;
			}
			else
			{
				lock (rebornStampData)
				{
					int num = 0;
					int num2 = 0;
					if (!RebornStamp.CheckTypeMatch(rebornStampData, StampID, StampType, out num, out num2))
					{
						return ResOpcode.LevelUpYinJiCheckErr;
					}
					if (UpNum <= 0)
					{
						return ResOpcode.LevelUpYinJiUpNumErr;
					}
					lock (client.ClientData.PropPointMutex)
					{
						long num3 = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
						long num4 = num3 - (long)rebornStampData.UsePoint;
						if (num4 < (long)UpNum)
						{
							return ResOpcode.LevelUpYinJiPointErr;
						}
					}
					int num5 = RebornStamp.MinorLevelLimit[StampType] - rebornStampData.StampInfo[num + 1];
					if (0 >= num5)
					{
						return ResOpcode.LevelUpYinJiMaxLv;
					}
					if (num5 < UpNum)
					{
						return ResOpcode.LevelUpYinJiOverUpLvErr;
					}
					int currMinorLevelItemID = RebornStamp.GetCurrMinorLevelItemID(rebornStampData, num);
					OutMainYinJiID1 = currMinorLevelItemID + UpNum;
					ChongShengYinJiZi chongShengYinJiZi;
					if (!RebornStamp.MinorYinJi.TryGetValue(OutMainYinJiID1, out chongShengYinJiZi))
					{
						return ResOpcode.LevelUpYinJiMaxLv;
					}
					rebornStampData.StampInfo[num + 1] = chongShengYinJiZi.Level;
					if (RebornStamp.IsMainLevelUp(rebornStampData, num2, out IDZhu))
					{
						if (num2 == 1)
						{
							int currMainLevelItemID = RebornStamp.GetCurrMainLevelItemID(rebornStampData, 0);
							int num6 = currMainLevelItemID + RebornStamp.GetMainYinJiLevelUpNum(rebornStampData, 0);
							rebornStampData.StampInfo[1] = RebornStamp.MainYinJi[num6].Level;
							IDZhu = num6;
						}
						if (num2 == 2)
						{
							int currMainLevelItemID = RebornStamp.GetCurrMainLevelItemID(rebornStampData, 8);
							int num6 = currMainLevelItemID + RebornStamp.GetMainYinJiLevelUpNum(rebornStampData, 8);
							rebornStampData.StampInfo[9] = RebornStamp.MainYinJi[num6].Level;
							IDZhu = num6;
						}
					}
					string updateInfo = RebornStamp.MakeYinJiUpdateInfo(rebornStampData.StampInfo);
					OutUsePoint = rebornStampData.UsePoint + UpNum;
					if (!RebornStamp.UpdateRebornYinJiInfo(client, updateInfo, OutUsePoint, rebornStampData.ResetNum))
					{
						return ResOpcode.LevelUpYinJiSaveErr;
					}
				}
				client.ClientData.RebornYinJi = rebornStampData;
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = ResOpcode.Succ;
			}
			return result;
		}

		public static ResOpcode ProcessRebornYinJiGetInfo(GameClient client, int RoldID, out RebornStampData dbInfo)
		{
			dbInfo = null;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out dbInfo))
			{
				result = ResOpcode.GetYinJiInfoErr;
			}
			else
			{
				result = ResOpcode.Succ;
			}
			return result;
		}

		public static ResOpcode ProcessRebornYinJiReset(GameClient client, int RoldID)
		{
			RebornStampData rebornStampData;
			ResOpcode result;
			if (!RebornStamp.GetRebornYinJiInfo(client, out rebornStampData))
			{
				result = ResOpcode.ChooseGetInfoYinJiNotActive;
			}
			else
			{
				lock (rebornStampData)
				{
					int subMoney;
					if (rebornStampData.ResetNum < RebornStamp.YinJiReset.Count)
					{
						subMoney = RebornStamp.YinJiReset[rebornStampData.ResetNum];
					}
					else
					{
						subMoney = RebornStamp.YinJiReset[RebornStamp.YinJiReset.Count - 1];
					}
					rebornStampData.ResetNum++;
					if (!GameManager.ClientMgr.SubUserMoney(client, subMoney, "重生印记洗点", true, true, true, true, DaiBiSySType.None))
					{
						return ResOpcode.ResetYinJiZuanShiErr;
					}
					if (!RebornStamp.UpdateRebornYinJiInfo(client, "", 0, rebornStampData.ResetNum))
					{
						return ResOpcode.ResetYinJiInfoErr;
					}
				}
				RebornStampData rebornYinJi;
				if (RebornStamp.GetRebornYinJiInfo(client, out rebornYinJi))
				{
					client.ClientData.RebornYinJi = rebornYinJi;
				}
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = ResOpcode.Succ;
			}
			return result;
		}

		public static ResOpcode ProcessRebornYinJiChoose(GameClient client, int RoldID, int StampType1, int StampType2)
		{
			ResOpcode result;
			if (StampType1 < 1 || StampType1 > 6)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else if (StampType2 < 1 || StampType2 > 6)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else if (StampType1 == StampType2)
			{
				result = ResOpcode.ChooseYinJiTypeErr;
			}
			else
			{
				RebornStampData rebornStampData;
				if (RebornStamp.GetRebornYinJiInfo(client, out rebornStampData))
				{
					lock (rebornStampData)
					{
						if (rebornStampData.StampInfo != null && rebornStampData.StampInfo.Count > 0)
						{
							return ResOpcode.ChooseGetInfoYinJiIsActive;
						}
						if (!RebornStamp.UpdateRebornYinJiInfo(client, RebornStamp.MakeYinJiUpdateInfoByType(StampType1, StampType2), rebornStampData.UsePoint, rebornStampData.ResetNum))
						{
							return ResOpcode.ChooseYinJiIsActiveErr;
						}
						RebornStampData rebornYinJi;
						if (RebornStamp.GetRebornYinJiInfo(client, out rebornYinJi))
						{
							client.ClientData.RebornYinJi = rebornYinJi;
						}
						Global.RefreshEquipProp(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						return ResOpcode.ChooseGetInfoYinJiIsActive;
					}
				}
				if (!RebornStamp.InsertRebornYinJi(client, RebornStamp.MakeYinJiUpdateInfoByType(StampType1, StampType2)))
				{
					result = ResOpcode.ChooseYinJiIsActiveErr;
				}
				else
				{
					RebornStampData rebornYinJi;
					if (RebornStamp.GetRebornYinJiInfo(client, out rebornYinJi))
					{
						client.ClientData.RebornYinJi = rebornYinJi;
					}
					Global.RefreshEquipProp(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					result = ResOpcode.ChooseGetInfoYinJiIsActive;
				}
			}
			return result;
		}

		public static Dictionary<int, ChongShengYinJiZhu> MainYinJi = new Dictionary<int, ChongShengYinJiZhu>();

		public static Dictionary<MainAttrType, List<int>> TypeMap = new Dictionary<MainAttrType, List<int>>();

		public static Dictionary<MainAttrType, Dictionary<int, int>> CurrMainYinJi = new Dictionary<MainAttrType, Dictionary<int, int>>();

		public static Dictionary<MainAttrType, Dictionary<int, int>> MainYinJiLevelUp = new Dictionary<MainAttrType, Dictionary<int, int>>();

		public static Dictionary<int, ChongShengYinJiZi> MinorYinJi = new Dictionary<int, ChongShengYinJiZi>();

		public static Dictionary<int, Dictionary<int, int>> MinorYinJiLevelUp = new Dictionary<int, Dictionary<int, int>>();

		public static Dictionary<int, int> MinorLevelLimit = new Dictionary<int, int>();

		public static List<int> YinJiReset = new List<int>();

		private static RebornStamp instance = new RebornStamp();
	}
}
