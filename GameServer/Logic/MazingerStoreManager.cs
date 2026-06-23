using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class MazingerStoreManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2
	{
		public bool InitConfig()
		{
			Dictionary<int, Dictionary<int, MazingerUpGrade>> dictionary = new Dictionary<int, Dictionary<int, MazingerUpGrade>>();
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
			string text = Global.GameResPath(MazingerStoreConst.MoShenMiBaoJie);
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					MazingerUpGrade mazingerUpGrade = new MazingerUpGrade();
					Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
					mazingerUpGrade.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					mazingerUpGrade.Stage = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MiBaoStageLevel"));
					mazingerUpGrade.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MiBaoType"));
					mazingerUpGrade.LuckyOne = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LuckyOne"));
					mazingerUpGrade.LuckyTwo = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "LuckyTwo"));
					mazingerUpGrade.Rate = Convert.ToDouble(Global.GetSafeAttributeStr(xml, "LuckyTwoRate"));
					string[] array = Global.GetSafeAttributeStr(xml, "NeedGoods").Split(new char[]
					{
						'|'
					});
					if (array.Length != 1)
					{
						dictionary3.Add(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
					}
					mazingerUpGrade.UseGoods = dictionary3;
					if (dictionary2.ContainsKey(mazingerUpGrade.Type))
					{
						if (dictionary2[mazingerUpGrade.Type] < mazingerUpGrade.Stage)
						{
							dictionary2[mazingerUpGrade.Type] = mazingerUpGrade.Stage;
						}
					}
					else
					{
						dictionary2.Add(mazingerUpGrade.Type, mazingerUpGrade.Stage);
					}
					if (dictionary.ContainsKey(mazingerUpGrade.Type))
					{
						if (dictionary[mazingerUpGrade.Type].ContainsKey(mazingerUpGrade.Stage))
						{
							dictionary[mazingerUpGrade.Type][mazingerUpGrade.Stage] = mazingerUpGrade;
						}
						else
						{
							dictionary[mazingerUpGrade.Type].Add(mazingerUpGrade.Stage, mazingerUpGrade);
						}
					}
					else
					{
						Dictionary<int, MazingerUpGrade> dictionary4 = new Dictionary<int, MazingerUpGrade>();
						dictionary4.Add(mazingerUpGrade.Stage, mazingerUpGrade);
						dictionary.Add(mazingerUpGrade.Type, dictionary4);
					}
				}
				this.MazingerGrade = dictionary;
				this.MazingerGradeLevelMax = dictionary2;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			bool result;
			if (this.MazingerGrade == null || this.MazingerGradeLevelMax == null)
			{
				result = false;
			}
			else
			{
				Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>> dictionary5 = new Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>>();
				Dictionary<int, Dictionary<int, int>> dictionary6 = new Dictionary<int, Dictionary<int, int>>();
				text = Global.GameResPath(MazingerStoreConst.MoShenMiBaoXing);
				xelement = XElement.Load(text);
				if (null == xelement)
				{
					LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
				}
				try
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						MazingerUpStar mazingerUpStar = new MazingerUpStar();
						Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
						Dictionary<int, double> dictionary7 = new Dictionary<int, double>();
						mazingerUpStar.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
						mazingerUpStar.Stage = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MiBaoStageLevel"));
						mazingerUpStar.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MibaoStarLevel"));
						mazingerUpStar.Type = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MiBaoType"));
						mazingerUpStar.UpExp = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "MibaoStarExp"));
						mazingerUpStar.Exp = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "GoodsExp"));
						string[] array = Global.GetSafeAttributeStr(xml, "NeedGoods").Split(new char[]
						{
							'|'
						});
						if (array.Length != 1)
						{
							dictionary3.Add(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]));
						}
						mazingerUpStar.UseGoods = dictionary3;
						string[] array2 = Global.GetSafeAttributeStr(xml, "MiBaoAttribute").Split(new char[]
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
								dictionary7.Add((int)ConfigParser.GetPropIndexByPropName(array4[0]), Convert.ToDouble(array4[1]));
							}
						}
						mazingerUpStar.Attr = dictionary7;
						if (dictionary6.ContainsKey(mazingerUpStar.Type))
						{
							if (dictionary6[mazingerUpStar.Type].ContainsKey(mazingerUpStar.Stage))
							{
								if (dictionary6[mazingerUpStar.Type][mazingerUpStar.Stage] < mazingerUpStar.Level)
								{
									dictionary6[mazingerUpStar.Type][mazingerUpStar.Stage] = mazingerUpStar.Level;
								}
							}
							else
							{
								dictionary6[mazingerUpStar.Type].Add(mazingerUpStar.Stage, mazingerUpStar.Level);
							}
						}
						else
						{
							Dictionary<int, int> dictionary8 = new Dictionary<int, int>();
							dictionary8.Add(mazingerUpStar.Stage, mazingerUpStar.Level);
							dictionary6.Add(mazingerUpStar.Type, dictionary8);
						}
						if (dictionary5.ContainsKey(mazingerUpStar.Type))
						{
							if (dictionary5[mazingerUpStar.Type].ContainsKey(mazingerUpStar.Stage))
							{
								if (!dictionary5[mazingerUpStar.Type][mazingerUpStar.Stage].ContainsKey(mazingerUpStar.Level))
								{
									dictionary5[mazingerUpStar.Type][mazingerUpStar.Stage].Add(mazingerUpStar.Level, mazingerUpStar);
								}
							}
							else
							{
								Dictionary<int, MazingerUpStar> dictionary9 = new Dictionary<int, MazingerUpStar>();
								dictionary9.Add(mazingerUpStar.Level, mazingerUpStar);
								dictionary5[mazingerUpStar.Type].Add(mazingerUpStar.Stage, dictionary9);
							}
						}
						else
						{
							Dictionary<int, Dictionary<int, MazingerUpStar>> dictionary10 = new Dictionary<int, Dictionary<int, MazingerUpStar>>();
							Dictionary<int, MazingerUpStar> dictionary9 = new Dictionary<int, MazingerUpStar>();
							dictionary9.Add(mazingerUpStar.Level, mazingerUpStar);
							dictionary10.Add(mazingerUpStar.Stage, dictionary9);
							dictionary5.Add(mazingerUpStar.Type, dictionary10);
						}
					}
					this.MazingerStar = dictionary5;
					this.MazingerStarLevelMax = dictionary6;
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
				result = (this.MazingerStar != null && dictionary6 != null);
			}
			return result;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2097, 2, 2, MazingerStoreManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public static MazingerStoreManager getInstance()
		{
			return MazingerStoreManager.instance;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 2097)
			{
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int clientType = Convert.ToInt32(cmdParams[0]);
					int clientOpt = Convert.ToInt32(cmdParams[1]);
					MazingerStore cmdData = this.ProcessMazingerStoreUpGrade(client, clientType, clientOpt);
					client.sendCmd<MazingerStore>(nID, cmdData, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, "-1", false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_MAZINGERSTORE_UPGRADE", false, false);
				}
			}
			return true;
		}

		public void UpdateProps(GameClient client)
		{
			double[] array = new double[177];
			try
			{
				if (client.ClientData.MazingerStoreDataInfo != null && client.ClientData.MazingerStoreDataInfo.Count != 0)
				{
					foreach (MazingerStoreData mazingerStoreData in client.ClientData.MazingerStoreDataInfo.Values)
					{
						if (this.MazingerStar.ContainsKey(mazingerStoreData.Type))
						{
							if (this.MazingerStar[mazingerStoreData.Type].ContainsKey(mazingerStoreData.Stage))
							{
								if (this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage].ContainsKey(mazingerStoreData.StarLevel))
								{
									foreach (KeyValuePair<int, double> keyValuePair in this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].Attr)
									{
										array[keyValuePair.Key] += keyValuePair.Value;
									}
								}
							}
						}
					}
				}
			}
			finally
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.MazingerStore,
					array
				});
			}
		}

		public List<double> GetSystemParamMibao()
		{
			List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("MibaoBaoji", ',');
			List<double> result;
			try
			{
				List<double> list = new List<double>();
				foreach (string value in paramValueStringListByName)
				{
					list.Add(Convert.ToDouble(value));
				}
				result = list;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result = null;
			}
			return result;
		}

		public MazingerStoreData CopyMazingerStoreMemData(GameClient client, int key)
		{
			return new MazingerStoreData
			{
				RoleID = client.ClientData.RoleID,
				Type = client.ClientData.MazingerStoreDataInfo[key].Type,
				Stage = client.ClientData.MazingerStoreDataInfo[key].Stage,
				StarLevel = client.ClientData.MazingerStoreDataInfo[key].StarLevel,
				Exp = client.ClientData.MazingerStoreDataInfo[key].Exp
			};
		}

		public bool UseXmlGoods(GameClient client, Dictionary<int, Dictionary<int, GoodsData>> TotleGoods)
		{
			foreach (Dictionary<int, GoodsData> dictionary in TotleGoods.Values)
			{
				using (Dictionary<int, GoodsData>.Enumerator enumerator2 = dictionary.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						KeyValuePair<int, GoodsData> keyValuePair = enumerator2.Current;
						bool flag;
						if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair.Value.GoodsID, keyValuePair.Key, 1, out flag))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public MazingerStore ProcessMazingerStoreUpGrade(GameClient client, int ClientType, int ClientOpt)
		{
			MazingerStore mazingerStore = new MazingerStore();
			mazingerStore.IsBoom = 0;
			bool flag = false;
			MazingerStore result;
			if (!GlobalNew.IsGongNengOpened(client, 118, false))
			{
				LogManager.WriteLog(2, string.Format("玩家魔神秘宝功能未开启, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
				mazingerStore.result = 12;
				result = mazingerStore;
			}
			else
			{
				if (ClientOpt < 0 || ClientOpt > 1)
				{
					mazingerStore.result = 2;
				}
				else if (ClientOpt == 0)
				{
					if (this.MazingerStar == null || this.MazingerStarLevelMax == null || !this.MazingerStar.ContainsKey(ClientType) || !this.MazingerStarLevelMax.ContainsKey(ClientType))
					{
						mazingerStore.result = 3;
					}
					else
					{
						if (client.ClientData.MazingerStoreDataInfo == null)
						{
							client.ClientData.MazingerStoreDataInfo = new Dictionary<int, MazingerStoreData>();
						}
						MazingerStoreData mazingerStoreData = null;
						bool flag2 = false;
						int num = 0;
						if (client.ClientData.MazingerStoreDataInfo.ContainsKey(ClientType))
						{
							num = this.MazingerStarLevelMax[ClientType][client.ClientData.MazingerStoreDataInfo[ClientType].Stage];
							if (client.ClientData.MazingerStoreDataInfo[ClientType].StarLevel >= num)
							{
								mazingerStore.result = 5;
								goto IL_B64;
							}
							mazingerStoreData = this.CopyMazingerStoreMemData(client, ClientType);
							flag2 = true;
						}
						else
						{
							mazingerStoreData = new MazingerStoreData();
							mazingerStoreData.RoleID = client.ClientData.RoleID;
							mazingerStoreData.Type = ClientType;
							mazingerStoreData.Exp = 0;
							mazingerStoreData.Stage = 1;
							mazingerStoreData.StarLevel = 0;
							num = this.MazingerStarLevelMax[ClientType][mazingerStoreData.Stage];
						}
						if (mazingerStoreData == null || !this.MazingerStar.ContainsKey(mazingerStoreData.Type) || !this.MazingerStar[mazingerStoreData.Type].ContainsKey(mazingerStoreData.Stage) || !this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage].ContainsKey(mazingerStoreData.StarLevel) || this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].UseGoods == null || this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].UseGoods.Count == 0)
						{
							mazingerStore.result = 6;
						}
						else
						{
							List<double> systemParamMibao = this.GetSystemParamMibao();
							if (systemParamMibao == null || systemParamMibao.Count != 2)
							{
								mazingerStore.result = 4;
							}
							else
							{
								int num2 = this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].Exp;
								double random = Global.GetRandom();
								if (random <= systemParamMibao[0])
								{
									num2 = Convert.ToInt32((double)num2 * systemParamMibao[1]);
									mazingerStore.IsBoom = 1;
								}
								foreach (KeyValuePair<int, int> keyValuePair in this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].UseGoods)
								{
									bool flag3;
									if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair.Key, keyValuePair.Value, 1, out flag3))
									{
										mazingerStore.result = 7;
										break;
									}
								}
								if (mazingerStore.result != 7)
								{
									int num3 = mazingerStoreData.Exp + num2;
									int i = mazingerStoreData.StarLevel;
									int upExp = this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][mazingerStoreData.StarLevel].UpExp;
									while (i < num)
									{
										if (num3 < upExp)
										{
											break;
										}
										i++;
										num3 -= upExp;
										if (num3 < 0)
										{
											num3 = 0;
											break;
										}
										if (this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage].ContainsKey(i))
										{
											upExp = this.MazingerStar[mazingerStoreData.Type][mazingerStoreData.Stage][i].UpExp;
										}
										if (!flag)
										{
											flag = true;
										}
									}
									mazingerStoreData.StarLevel = i;
									if (i >= num)
									{
										mazingerStoreData.Exp = 0;
									}
									else
									{
										mazingerStoreData.Exp = num3;
									}
									if (flag2)
									{
										int num4 = Global.sendToDB<int, MazingerStoreData>(14126, mazingerStoreData, client.ServerId);
										if (num4 < 0)
										{
											LogManager.WriteLog(2, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
											mazingerStore.result = 10;
											goto IL_B64;
										}
										GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升星", DateTime.Now.ToString(), mazingerStoreData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
										EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].StarLevel, mazingerStoreData.StarLevel, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].Exp, mazingerStoreData.Exp, "魔神秘宝升星");
										client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type] = mazingerStoreData;
									}
									else
									{
										int num4 = Global.sendToDB<int, MazingerStoreData>(14125, mazingerStoreData, client.ServerId);
										if (num4 < 0)
										{
											LogManager.WriteLog(2, string.Format("魔神秘宝插入数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
											mazingerStore.result = 10;
											goto IL_B64;
										}
										GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升星", DateTime.Now.ToString(), mazingerStoreData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
										EventLogManager.AddMazingerStoreEvent(client, 0, mazingerStoreData.StarLevel, 0, mazingerStoreData.Exp, "魔神秘宝升星");
										client.ClientData.MazingerStoreDataInfo.Add(mazingerStoreData.Type, mazingerStoreData);
									}
									mazingerStore.result = 1;
									mazingerStore.data = client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type];
								}
							}
						}
					}
				}
				else if (this.MazingerGrade == null || this.MazingerGradeLevelMax == null || !this.MazingerGrade.ContainsKey(ClientType) || !this.MazingerGradeLevelMax.ContainsKey(ClientType))
				{
					mazingerStore.result = 3;
				}
				else if (client.ClientData.MazingerStoreDataInfo == null || !client.ClientData.MazingerStoreDataInfo.ContainsKey(ClientType))
				{
					mazingerStore.result = 11;
				}
				else if (client.ClientData.MazingerStoreDataInfo[ClientType].Stage >= this.MazingerGradeLevelMax[ClientType])
				{
					mazingerStore.result = 5;
				}
				else
				{
					MazingerStoreData mazingerStoreData = this.CopyMazingerStoreMemData(client, ClientType);
					if (mazingerStoreData == null || !this.MazingerGrade.ContainsKey(mazingerStoreData.Type) || !this.MazingerGrade[mazingerStoreData.Type].ContainsKey(mazingerStoreData.Stage) || this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].UseGoods == null || this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].UseGoods.Count == 0)
					{
						mazingerStore.result = 6;
					}
					else
					{
						foreach (KeyValuePair<int, int> keyValuePair in this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].UseGoods)
						{
							bool flag3;
							if (!RebornStone.RebornUseGoodHasBinding(client, keyValuePair.Key, keyValuePair.Value, 1, out flag3))
							{
								mazingerStore.result = 7;
								break;
							}
						}
						if (mazingerStore.result != 7)
						{
							mazingerStoreData.Exp++;
							if (this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].LuckyOne + mazingerStoreData.Exp >= 110000)
							{
								mazingerStoreData.Stage++;
								mazingerStoreData.StarLevel = 0;
								mazingerStoreData.Exp = 0;
								flag = true;
							}
							else if (this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].LuckyOne + mazingerStoreData.Exp > this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].LuckyTwo)
							{
								if (Global.GetRandom() < this.MazingerGrade[mazingerStoreData.Type][mazingerStoreData.Stage].Rate)
								{
									mazingerStoreData.Stage++;
									mazingerStoreData.StarLevel = 0;
									mazingerStoreData.Exp = 0;
									flag = true;
									mazingerStore.IsBoom = 1;
								}
							}
							int num4 = Global.sendToDB<int, MazingerStoreData>(14126, mazingerStoreData, client.ServerId);
							if (num4 < 0)
							{
								LogManager.WriteLog(2, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
								mazingerStore.result = 10;
							}
							else
							{
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔神秘宝升阶", DateTime.Now.ToString(), mazingerStoreData.Type.ToString(), client.ClientData.RoleName, "系统", ClientType, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].Stage, mazingerStoreData.Stage, client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type].Exp, mazingerStoreData.Exp, "魔神秘宝升阶");
								client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type] = mazingerStoreData;
								mazingerStore.result = 1;
								mazingerStore.data = client.ClientData.MazingerStoreDataInfo[mazingerStoreData.Type];
							}
						}
					}
				}
				IL_B64:
				if (flag && mazingerStore.result == 1)
				{
					Global.RefreshEquipPropAndNotify(client);
				}
				result = mazingerStore;
			}
			return result;
		}

		public Dictionary<int, Dictionary<int, MazingerUpGrade>> MazingerGrade = new Dictionary<int, Dictionary<int, MazingerUpGrade>>();

		public Dictionary<int, int> MazingerGradeLevelMax = new Dictionary<int, int>();

		public Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>> MazingerStar = new Dictionary<int, Dictionary<int, Dictionary<int, MazingerUpStar>>>();

		public Dictionary<int, Dictionary<int, int>> MazingerStarLevelMax = new Dictionary<int, Dictionary<int, int>>();

		public List<double> MazingerRate = new List<double>();

		private static MazingerStoreManager instance = new MazingerStoreManager();
	}
}
