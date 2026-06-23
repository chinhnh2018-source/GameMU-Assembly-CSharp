using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class FashionManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static FashionManager getInstance()
		{
			if (FashionManager.instance.State == 0)
			{
				FashionManager.instance.initialize();
			}
			return FashionManager.instance;
		}

		public bool initialize()
		{
			bool result;
			if (!this.InitConfig())
			{
				this.State = -1;
				result = false;
			}
			else
			{
				this.State = 1;
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(710, 4, 4, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1610, 2, 2, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1611, 2, 2, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1841, 3, 3, FashionManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(41, FashionManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(41, FashionManager.getInstance());
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

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (nID != 710)
			{
				switch (nID)
				{
				case 1610:
					result = this.ProcessFashionBagForgeLevUpCmd(client, nID, bytes, cmdParams);
					break;
				case 1611:
					result = this.ProcessFashionBagActiveCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = (nID != 1841 || this.ProcessModifyBuffFashionCmd(client, nID, bytes, cmdParams));
					break;
				}
			}
			else
			{
				result = this.ProcessModifyFashionCmd(client, nID, bytes, cmdParams);
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num == 41)
			{
				GameClient player = (eventObject as PlayerLoginGameEventObject).getPlayer();
				this.InitFashion(player);
			}
		}

		public bool InitConfig()
		{
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.FashionTabDict.Clear();
					text = "Config/FashionTab.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						FashionTabData fashionTabData = new FashionTabData();
						fashionTabData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						fashionTabData.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						fashionTabData.Categoriy = (int)Global.GetSafeAttributeLong(xelement2, "Categoriy");
						this.RuntimeData.FashionTabDict.Add(fashionTabData.ID, fashionTabData);
					}
					Data.ClearMiniBufferDataIds();
					this.RuntimeData.SpecialTitleDict.Clear();
					text = "Config/SpecialTitle.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						int key = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						int num = (int)Global.GetSafeAttributeLong(xelement2, "BuffID");
						this.RuntimeData.SpecialTitleDict.Add(key, num);
						Data.AddMiniBufferDataIds(new int[]
						{
							num
						});
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.FashingDict.Clear();
					text = "Config/Fashion.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						FashionData fashionData = new FashionData();
						fashionData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						fashionData.TabID = (int)Global.GetSafeAttributeLong(xelement2, "Tab");
						fashionData.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						fashionData.GoodsID = (int)Global.GetSafeAttributeLong(xelement2, "Goods");
						fashionData.Type = (int)Global.GetSafeAttributeLong(xelement2, "Type");
						fashionData.Time = (int)Global.GetSafeAttributeLong(xelement2, "Time");
						fashionData.RandomType = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "Random", "-1"));
						fashionData.EndTime = DateTime.MinValue;
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Term");
						if (!string.IsNullOrEmpty(safeAttributeStr) && 0 != string.Compare(safeAttributeStr, "-1"))
						{
							DateTime.TryParse(safeAttributeStr, out fashionData.EndTime);
						}
						this.RuntimeData.FashingDict.Add(fashionData.ID, fashionData);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				string[] array = new string[]
				{
					"Config/ShiZhuangLevelup.xml",
					"Config/FashionWings.xml",
					"Config/HorseFashion.xml",
					"Config/JiaoYinShiZhuangShengJi.xml",
					"Config/WuQiShiZhuangShengJi.xml",
					"Config/TransfigurationFashion.xml"
				};
				int[] array2 = new int[]
				{
					3,
					1,
					4,
					5,
					6,
					7
				};
				try
				{
					this.RuntimeData.FashionBagDict.Clear();
					for (int i = 0; i < array.Length; i++)
					{
						int fasionTab = array2[i];
						text = array[i];
						string uri = Global.GameResPath(text);
						XElement xelement = XElement.Load(uri);
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							FashionBagData fashionBagData = new FashionBagData();
							fashionBagData.FasionTab = fasionTab;
							fashionBagData.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
							fashionBagData.GoodsID = (int)Global.GetSafeAttributeLong(xelement2, "GoodsID");
							fashionBagData.ForgeLev = (int)Global.GetSafeAttributeLong(xelement2, "level");
							fashionBagData.LimitTime = (int)Global.GetSafeAttributeLong(xelement2, "Time");
							string text2 = Global.GetSafeAttributeStr(xelement2, "NeedGoods");
							string[] array3 = text2.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								fashionBagData.NeedGoodsID = Global.SafeConvertToInt32(array3[0]);
								fashionBagData.NeedGoodsCount = Global.SafeConvertToInt32(array3[1]);
							}
							text2 = Global.GetSafeAttributeStr(xelement2, "ProPerty");
							array3 = text2.Split(new char[]
							{
								'|'
							});
							foreach (string text3 in array3)
							{
								string[] array5 = text3.Split(new char[]
								{
									','
								});
								if (array5.Length == 2)
								{
									ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array5[0]);
									if (propIndexByPropName != ExtPropIndexes.Max)
									{
										fashionBagData.ExtProps[(int)propIndexByPropName] = Global.SafeConvertToDouble(array5[1]);
									}
								}
							}
							text2 = ConfigHelper.GetElementAttributeValue(xelement2, "AttackSkill", "");
							if (!string.IsNullOrEmpty(text2))
							{
								fashionBagData.AttackSkill = Global.StringToIntList(text2, ',');
							}
							text2 = ConfigHelper.GetElementAttributeValue(xelement2, "MagicSkill", "");
							if (!string.IsNullOrEmpty(text2))
							{
								fashionBagData.MagicSkill = Global.StringToIntList(text2, ',');
							}
							fashionBagData.BianShenEffect = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Effect", 0L);
							fashionBagData.BianShenDuration = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Duration", 0L);
							KeyValuePair<int, int> key2 = new KeyValuePair<int, int>(fashionBagData.GoodsID, fashionBagData.ForgeLev);
							if (this.RuntimeData.FashionBagDict.ContainsKey(key2))
							{
								LogManager.WriteLog(1000, string.Format("道具ID重复或者已经配置为其它类型的时装,xml={0}", xelement2), null, true);
							}
							this.RuntimeData.FashionBagDict[key2] = fashionBagData;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			return true;
		}

		public bool ProcessModifyFashionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int tabID = Convert.ToInt32(cmdParams[1]);
				int fashionID = Convert.ToInt32(cmdParams[2]);
				FashionModeTypes mode = (FashionModeTypes)Convert.ToInt32(cmdParams[3]);
				int num2 = this.ModifyFashion(client, tabID, fashionID, mode);
				client.sendCmd(nID, string.Format("{0}", num2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessModifyBuffFashionCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int buffID = Convert.ToInt32(cmdParams[1]);
				FashionModeTypes mode = (FashionModeTypes)Convert.ToInt32(cmdParams[2]);
				int num2 = this.ModifyBuffFashion(client, buffID, mode);
				client.sendCmd(nID, string.Format("{0}", num2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void InitLuoLanChengZhuFashion(GameClient client)
		{
			if (client.ClientSocket.IsKuaFuLogin)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				Dictionary<int, BangHuiLingDiItemData> dictionary = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(client.ClientSocket.ServerId);
				BangHuiLingDiItemData bangHuiLingDiItemData = null;
				int num = 7;
				if (dictionary == null || !dictionary.TryGetValue(num, out bangHuiLingDiItemData))
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				if (bangHuiLingDiItemData == null || client.ClientData.Faction != bangHuiLingDiItemData.BHID)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
			}
			else
			{
				int num = 7;
				BangHuiLingDiItemData bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(num);
				if (bangHuiLingDiItemData == null || bangHuiLingDiItemData.BHID <= 0)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
				if (client.ClientData.Faction != bangHuiLingDiItemData.BHID || client.ClientData.BHZhiWu != 1)
				{
					this.DelLuoLanZhiYi(client);
					return;
				}
			}
			this.GetFashionByMagic(client, 1, true);
		}

		public void DelLuoLanZhiYi(GameClient gameclient)
		{
			this.DelFashionByMagic(gameclient, 1);
		}

		public void DelFashionByMagic(GameClient client, int nFashionID)
		{
			if (client != null)
			{
				FashionData fashionData = null;
				if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
				{
					LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
				}
				else
				{
					GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, fashionData.GoodsID);
					if (fashionDataByGoodsID != null)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							4,
							fashionDataByGoodsID.Id,
							fashionDataByGoodsID.GoodsID,
							0,
							fashionDataByGoodsID.Site,
							fashionDataByGoodsID.GCount,
							fashionDataByGoodsID.BagIndex,
							""
						});
						Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
					}
				}
			}
		}

		public void GetFashionByMagic(GameClient client, int nFashionID, bool isAddTime = true)
		{
			FashionData fashionData = null;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
			}
			else
			{
				int goodsID = fashionData.GoodsID;
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
				{
					LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, GoodsID={0}", goodsID), null, true);
				}
				else
				{
					DateTime minValue = DateTime.MinValue;
					GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, goodsID);
					int intValue = systemXmlItem.GetIntValue("GridNum", -1);
					string text;
					string text2;
					if (fashionData.Time > 0)
					{
						if (fashionDataByGoodsID != null)
						{
							if (DateTime.TryParse(fashionDataByGoodsID.Endtime, out minValue))
							{
								fashionDataByGoodsID.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
							}
							Global.DestroyGoods(client, fashionDataByGoodsID);
						}
						text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
						if (minValue > DateTime.MinValue && isAddTime)
						{
							text2 = minValue.AddSeconds((double)fashionData.Time).ToString("yyyy-MM-dd HH:mm:ss");
						}
						else
						{
							text2 = TimeUtil.NowDateTime().AddSeconds((double)fashionData.Time).ToString("yyyy-MM-dd HH:mm:ss");
						}
					}
					else if (fashionData.EndTime != DateTime.MinValue)
					{
						text = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
						text2 = fashionData.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						text = "1900-01-01 12:00:00";
						text2 = "1900-01-01 12:00:00";
					}
					if (fashionDataByGoodsID == null || fashionData.Time > 0)
					{
						GoodsData goodsData = new GoodsData
						{
							GoodsID = goodsID,
							GCount = 1,
							Binding = 1,
							Starttime = text,
							Endtime = text2,
							Site = 6000
						};
						goodsData.Id = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsID, intValue, 0, "", 0, 0, 6000, "", true, 1, "使用指定道具后获取", true, text2, 0, 0, 0, 0, 0, 0, 0, true, null, null, text, 0, true);
						string resList = EventLogManager.NewGoodsDataPropString(goodsData);
						EventLogManager.AddTitleEvent(client, 1, fashionData.Time, resList);
						FashionManager.NotifyFashionList(client);
					}
				}
			}
		}

		public void GetFashionByMagic(GameClient client, int nFashionID, string endTime)
		{
			FashionData fashionData = null;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
			}
			else
			{
				int goodsID = fashionData.GoodsID;
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
				{
					LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, GoodsID={0}", goodsID), null, true);
				}
				else
				{
					DateTime minValue = DateTime.MinValue;
					GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, goodsID);
					int intValue = systemXmlItem.GetIntValue("GridNum", -1);
					if (fashionDataByGoodsID == null || fashionDataByGoodsID.Endtime != endTime)
					{
						if (fashionDataByGoodsID != null)
						{
							fashionDataByGoodsID.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
							Global.DestroyGoods(client, fashionDataByGoodsID);
						}
						string strStartTime = "1900-01-01 12:00:00";
						Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsID, intValue, 0, "", 0, 0, 6000, "", true, 1, "使用指定道具后获取", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
						FashionManager.NotifyFashionList(client);
					}
				}
			}
		}

		public static void NotifyFashionList(GameClient client)
		{
			byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(client.ClientData.FashionGoodsDataList);
			GameManager.ClientMgr.SendToClient(client, buffer, 946);
		}

		public bool FashionCanAdd(GameClient client, int nFashionID)
		{
			FashionData fashionData = null;
			bool result;
			if (!this.RuntimeData.FashingDict.TryGetValue(nFashionID, out fashionData))
			{
				LogManager.WriteLog(2, string.Format("Fashion配置文件中，配置的时装物品不存在, ID={0}", nFashionID), null, true);
				result = false;
			}
			else
			{
				if (fashionData.Time <= 0)
				{
					int goodsID = fashionData.GoodsID;
					GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, goodsID);
					if (fashionDataByGoodsID != null)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public int ModifyFashion(GameClient client, int tabID, int fashionID, FashionModeTypes mode)
		{
			int num = 0;
			if (mode <= FashionModeTypes.None || mode >= FashionModeTypes.Max)
			{
				num = -5;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					FashionData fashionData;
					if (!this.RuntimeData.FashingDict.TryGetValue(fashionID, out fashionData))
					{
						num = -20;
					}
					else if (mode == FashionModeTypes.Load)
					{
						num = this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID);
						if (num >= 0)
						{
							num = this.LoadFashion(client, fashionData);
						}
					}
					else if (mode == FashionModeTypes.Unload)
					{
						if (this.RuntimeData.FashionTabDict.ContainsKey(tabID))
						{
							num = this.UnloadFashion(client, fashionData, false);
						}
					}
				}
			}
			return num;
		}

		public int ModifyBuffFashion(GameClient client, int buffID, FashionModeTypes mode)
		{
			int result = 0;
			if (mode <= FashionModeTypes.None || mode >= FashionModeTypes.Max)
			{
				result = -5;
			}
			else
			{
				BufferData bufferData = null;
				foreach (BufferData bufferData2 in client.ClientData.BufferDataList)
				{
					if (bufferData2.BufferID == buffID)
					{
						bufferData = bufferData2;
						break;
					}
				}
				if (null == bufferData)
				{
					return -20;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10163");
				if (mode == FashionModeTypes.Load)
				{
					if (roleParamsInt32FromDB != bufferData.BufferID)
					{
						this.ModifyBuffFashionTitleID(client, bufferData.BufferID, true, true);
					}
					return 0;
				}
				if (mode == FashionModeTypes.Unload)
				{
					if (roleParamsInt32FromDB != bufferData.BufferID)
					{
						return 0;
					}
					this.ModifyBuffFashionTitleID(client, 0, true, true);
					return 0;
				}
			}
			return result;
		}

		public int ValidateFashion(GameClient client, int fashionType, int GoodsID)
		{
			int result;
			if (fashionType == 1)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -3001;
				}
				else
				{
					int num = 7;
					BangHuiLingDiItemData bangHuiLingDiItemData = null;
					if (client.ClientSocket.IsKuaFuLogin)
					{
						Dictionary<int, BangHuiLingDiItemData> dictionary = JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer(client.ServerId);
						if (dictionary != null)
						{
							dictionary.TryGetValue(num, out bangHuiLingDiItemData);
						}
					}
					else
					{
						bangHuiLingDiItemData = JunQiManager.GetItemByLingDiID(num);
					}
					if (bangHuiLingDiItemData == null || bangHuiLingDiItemData.BHID <= 0)
					{
						result = -3001;
					}
					else if (client.ClientData.Faction != bangHuiLingDiItemData.BHID || client.ClientData.BHZhiWu != 1)
					{
						result = -3001;
					}
					else
					{
						result = 0;
					}
				}
			}
			else if (fashionType == 2)
			{
				GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, GoodsID);
				if (fashionDataByGoodsID != null)
				{
					result = 0;
				}
				else
				{
					result = -12;
				}
			}
			else if (fashionType == 3)
			{
				if (client.ClientData.MyMarriageData.byMarrytype > 0)
				{
					result = 0;
				}
				else
				{
					result = -3002;
				}
			}
			else if (fashionType == 4)
			{
				if (client.ClientData.Faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != BangHuiMatchManager.getInstance().RuntimeData.ChengHaoBHid_Gold)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else if (fashionType == 5)
			{
				if (client.ClientData.Faction <= 0)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != KuaFuLueDuoManager.getInstance().RuntimeData.ChengHaoBHid)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else if (fashionType == 6)
			{
				if (client.ClientData.Faction <= 0)
				{
					result = -3003;
				}
				else if ((long)client.ClientData.Faction != KuaFuLueDuoManager.getInstance().RuntimeData.ChengHaoBHid)
				{
					result = -3003;
				}
				else
				{
					result = 0;
				}
			}
			else
			{
				result = -3;
			}
			return result;
		}

		public void InitFashion(GameClient client)
		{
			this.InitLuoLanChengZhuFashion(client);
			int num = this.GetFashionWingsID(client);
			if (num > 0)
			{
				FashionData fashionData = null;
				if (this.RuntimeData.FashingDict.TryGetValue(num, out fashionData))
				{
					if (this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID) >= 0)
					{
						this.LoadFashion(client, fashionData);
					}
					else
					{
						this.UnloadFashion(client, fashionData, false);
					}
				}
			}
			num = this.GetFashionTitleID(client);
			if (num > 0)
			{
				FashionData fashionData = null;
				if (this.RuntimeData.FashingDict.TryGetValue(num, out fashionData))
				{
					if (this.ValidateFashion(client, fashionData.Type, fashionData.GoodsID) >= 0)
					{
						this.LoadFashion(client, fashionData);
					}
					else
					{
						this.UnloadFashion(client, fashionData, false);
					}
				}
			}
			this.InitFashionBag(client);
			this.RefreshTitleFashionProps(client);
		}

		private void RefreshTitleFashionProps(GameClient client)
		{
			bool flag = false;
			if (null != client.ClientData.FashionGoodsDataList)
			{
				List<GoodsData> list;
				lock (client.ClientData.FashionGoodsDataList)
				{
					list = new List<GoodsData>(client.ClientData.FashionGoodsDataList);
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (GoodsData goodsData in list)
					{
						foreach (FashionData fashionData in this.RuntimeData.FashingDict.Values)
						{
							if (fashionData.GoodsID == goodsData.GoodsID && fashionData.TabID == 2)
							{
								EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
								if (null != equipPropItem)
								{
									client.ClientData.PropsCacheManager.SetExtProps(new object[]
									{
										PropsSystemTypes.FashionByGoodsProps,
										fashionData.TabID,
										fashionData.ID,
										equipPropItem.ExtProps
									});
									flag = true;
								}
							}
						}
						foreach (FashionBagData fashionBagData in this.RuntimeData.FashionBagDict.Values)
						{
							int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
							if (fashionBagData.GoodsID == goodsData.GoodsID && goodsData.Forge_level == fashionBagData.ForgeLev && (GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods || goodsCatetoriy == 8))
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.FashionByGoodsProps,
									3,
									fashionBagData.GoodsID,
									fashionBagData.ExtProps
								});
								flag = true;
							}
						}
					}
				}
			}
			if (flag)
			{
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			}
		}

		public void InitFashionBag(GameClient client)
		{
			GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 24);
			if (goodsDataByCategoriy != null && goodsDataByCategoriy.Site != 6000)
			{
				if (!Global.CanAddGoods(client, goodsDataByCategoriy.GoodsID, 1, goodsDataByCategoriy.Binding, "1900-01-01 12:00:00", true, false))
				{
					if (Global.UseMailGivePlayerAward(client, goodsDataByCategoriy, GLang.GetLang(377, new object[0]), GLang.GetLang(378, new object[0]), 1.0))
					{
						Global.DestroyGoods(client, goodsDataByCategoriy);
					}
				}
				else
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						client.ClientData.RoleID,
						2,
						goodsDataByCategoriy.Id,
						goodsDataByCategoriy.GoodsID,
						0,
						goodsDataByCategoriy.Site,
						goodsDataByCategoriy.GCount,
						goodsDataByCategoriy.BagIndex,
						""
					});
					Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
				}
			}
		}

		public bool FashionBagCanActive(GameClient client, GoodsData goodsData)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			bool result;
			if (!GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods && goodsCatetoriy != 8)
			{
				result = false;
			}
			else if (client.ClientData.FashionGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				List<GoodsData> list;
				lock (client.ClientData.FashionGoodsDataList)
				{
					list = new List<GoodsData>(client.ClientData.FashionGoodsDataList);
				}
				foreach (GoodsData goodsData2 in list)
				{
					if (goodsData2.GoodsID == goodsData.GoodsID && !Global.IsTimeLimitGoods(goodsData2))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		private bool ProcessFashionBagActiveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, num3);
				if (null == goodsByDbID)
				{
					num = -1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, num3), false);
					return true;
				}
				if (!this.FashionBagCanActive(client, goodsByDbID))
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, num3), false);
					return true;
				}
				FashionBagData fashionBagData = null;
				lock (this.RuntimeData.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(goodsByDbID.GoodsID, goodsByDbID.Forge_level);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out fashionBagData))
					{
						num = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, num3), false);
						return true;
					}
				}
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByDbID, 1, false, true))
				{
					num = -6;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, num3), false);
					return true;
				}
				DateTime minValue = DateTime.MinValue;
				GoodsData fashionDataByGoodsID = FashionManager.GetFashionDataByGoodsID(client, goodsByDbID.GoodsID);
				int num4 = 0;
				int forgeLevel = 0;
				if (null != fashionDataByGoodsID)
				{
					num4 = fashionDataByGoodsID.Using;
					forgeLevel = fashionDataByGoodsID.Forge_level;
				}
				string strStartTime;
				string endTime;
				if (fashionBagData.LimitTime > 0)
				{
					if (fashionDataByGoodsID != null)
					{
						if (DateTime.TryParse(fashionDataByGoodsID.Endtime, out minValue))
						{
							fashionDataByGoodsID.Endtime = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
						}
						Global.DestroyGoods(client, fashionDataByGoodsID);
					}
					strStartTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
					if (minValue > DateTime.MinValue)
					{
						endTime = minValue.AddSeconds((double)fashionBagData.LimitTime).ToString("yyyy-MM-dd HH:mm:ss");
					}
					else
					{
						endTime = TimeUtil.NowDateTime().AddSeconds((double)fashionBagData.LimitTime).ToString("yyyy-MM-dd HH:mm:ss");
					}
				}
				else
				{
					strStartTime = "1900-01-01 12:00:00";
					endTime = "1900-01-01 12:00:00";
				}
				int num5;
				if (fashionDataByGoodsID != null)
				{
					num5 = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsByDbID.GoodsID, 1, 0, "", forgeLevel, goodsByDbID.Binding, 6000, "", true, 1, "时装激活", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
					if (num4 > 0)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							1,
							num5,
							goodsByDbID.GoodsID,
							1,
							6000,
							1,
							0,
							""
						});
						Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
					}
				}
				else
				{
					num5 = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsByDbID.GoodsID, 1, 0, "", 0, goodsByDbID.Binding, 6000, "", true, 1, "时装激活", true, endTime, 0, 0, 0, 0, 0, 0, 0, true, null, null, strStartTime, 0, true);
				}
				FashionManager.NotifyFashionList(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, num5), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessFashionBagForgeLevUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				GoodsData fashionGoodsDataByDbID = FashionManager.GetFashionGoodsDataByDbID(client, num3);
				if (null == fashionGoodsDataByDbID)
				{
					num = -1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				if (Global.IsTimeLimitGoods(fashionGoodsDataByDbID))
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				FashionBagData fashionBagData = null;
				FashionBagData fashionBagData2 = null;
				lock (this.RuntimeData.Mutex)
				{
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(fashionGoodsDataByDbID.GoodsID, fashionGoodsDataByDbID.Forge_level);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out fashionBagData))
					{
						num = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							num3,
							0
						}), false);
						return true;
					}
					key = new KeyValuePair<int, int>(fashionGoodsDataByDbID.GoodsID, fashionGoodsDataByDbID.Forge_level + 1);
					if (!this.RuntimeData.FashionBagDict.TryGetValue(key, out fashionBagData2))
					{
						num = -23;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							num3,
							0
						}), false);
						return true;
					}
				}
				int totalBindGoodsCountByID = Global.GetTotalBindGoodsCountByID(client, fashionBagData2.NeedGoodsID);
				int totalNotBindGoodsCountByID = Global.GetTotalNotBindGoodsCountByID(client, fashionBagData2.NeedGoodsID);
				if (fashionBagData2.NeedGoodsCount > totalBindGoodsCountByID + totalNotBindGoodsCountByID)
				{
					num = -6;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				int num4 = fashionBagData2.NeedGoodsCount;
				int num5;
				if (fashionBagData2.NeedGoodsCount > totalBindGoodsCountByID)
				{
					num5 = totalBindGoodsCountByID;
					num4 = fashionBagData2.NeedGoodsCount - totalBindGoodsCountByID;
				}
				else
				{
					num5 = fashionBagData2.NeedGoodsCount;
					num4 = 0;
				}
				bool flag2 = false;
				bool flag3 = false;
				if (num5 > 0)
				{
					if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, fashionBagData2.NeedGoodsID, num5, false, out flag2, out flag3, false))
					{
						num = -6;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							num3,
							0
						}), false);
						return true;
					}
				}
				if (num4 > 0)
				{
					if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, fashionBagData2.NeedGoodsID, num4, false, out flag2, out flag3, false))
					{
						num = -6;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							num3,
							0
						}), false);
						return true;
					}
				}
				fashionGoodsDataByDbID.Forge_level++;
				string[] array = null;
				string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					fashionGoodsDataByDbID.Id,
					"*",
					fashionGoodsDataByDbID.Forge_level,
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
					"*",
					fashionGoodsDataByDbID.Binding,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out array, client.ServerId);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					num = -15;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
				{
					num = -15;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				this.RefreshTitleFashionProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					num3,
					fashionGoodsDataByDbID.Forge_level
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private int LoadFashion(GameClient client, FashionData fashionData)
		{
			EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
			int result;
			if (null == equipPropItem)
			{
				result = -3;
			}
			else if (fashionData.TabID == 1)
			{
				int num = this.GetFashionWingsID(client);
				if (num != fashionData.ID)
				{
					this.ModifyFashionWingsID(client, fashionData.ID, false, true);
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.FashionByGoodsProps,
					fashionData.TabID,
					0,
					equipPropItem.ExtProps
				});
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				result = 0;
			}
			else if (fashionData.TabID == 2)
			{
				int num = this.GetFashionTitleID(client);
				if (num != fashionData.ID)
				{
					this.ModifyFashionTitleID(client, fashionData.ID, false, true);
				}
				result = 0;
			}
			else
			{
				result = -3;
			}
			return result;
		}

		private int UnloadFashion(GameClient client, FashionData fashionData, bool bIsRemove)
		{
			int num = 0;
			if (fashionData.TabID == 2)
			{
				num = this.GetFashionTitleID(client);
			}
			else if (fashionData.TabID == 1)
			{
				num = this.GetFashionWingsID(client);
			}
			int result;
			if (num != fashionData.ID)
			{
				result = 0;
			}
			else
			{
				int nID = 0;
				if (bIsRemove)
				{
					nID = -1;
				}
				if (fashionData.TabID == 1)
				{
					this.ModifyFashionWingsID(client, nID, false, true);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.FashionByGoodsProps,
						fashionData.TabID,
						0,
						PropsCacheManager.ConstExtProps
					});
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
				else if (fashionData.TabID == 2)
				{
					this.ModifyFashionTitleID(client, nID, false, true);
				}
				result = 0;
			}
			return result;
		}

		public bool FashionActiveByMagic(GameClient client, double[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = 38;
				int num3 = 0;
				if (cmdParams.Length < 2 || cmdParams.Length > 3)
				{
					LogManager.WriteLog(2, string.Format("称号道具使用参数异常！！！", new object[0]), null, true);
					return false;
				}
				if (cmdParams.Length > 2)
				{
					num2 = Convert.ToInt32(cmdParams[1]);
					num3 = Convert.ToInt32(cmdParams[2]);
				}
				else
				{
					num3 = Convert.ToInt32(cmdParams[1]);
				}
				List<FashionData> list = new List<FashionData>();
				Dictionary<int, FashionData> dictionary = new Dictionary<int, FashionData>();
				lock (this.RuntimeData.Mutex)
				{
					dictionary = new Dictionary<int, FashionData>(this.RuntimeData.FashingDict);
				}
				foreach (FashionData fashionData in dictionary.Values)
				{
					if (fashionData.RandomType == num && FashionManager.GetFashionDataByGoodsID(client, fashionData.GoodsID) == null)
					{
						list.Add(fashionData);
					}
				}
				if (list.Count > 0)
				{
					int randomNumber = Global.GetRandomNumber(0, list.Count);
					FashionData fashionData2 = list[randomNumber];
					this.GetFashionByMagic(client, list[randomNumber].ID, true);
				}
				else
				{
					int num4 = num2;
					if (num4 <= 31)
					{
						switch (num4)
						{
						case 23:
							GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, num3, "道具脚本", true, false);
							break;
						case 24:
							break;
						case 25:
							GameManager.ClientMgr.ModifyMUMoHeValue(client, num3, "道具脚本", false, true, false);
							break;
						default:
							if (num4 == 31)
							{
								GameManager.FluorescentGemMgr.AddFluorescentPoint(client, num3, "道具脚本", true);
							}
							break;
						}
					}
					else if (num4 != 34)
					{
						if (num4 == 38)
						{
							GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, num3, "道具脚本", true, true, false);
						}
					}
					else
					{
						GameManager.ClientMgr.ModifyLangHunFenMoValue(client, num3, "道具脚本", true, true);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public int GetFashionWingsID(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "FashionWingsID", "2020-12-12 12:12:12");
		}

		public int GetFashionTitleID(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "FashionTitleID", "2020-12-12 12:12:12");
		}

		public void ModifyFashionWingsID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "FashionWingsID", nID, true, "2020-12-12 12:12:12");
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FashionWingsID, nID);
				string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 26, nID);
				client.sendOthersCmd(427, cmdData);
			}
		}

		public void ModifyFashionTitleID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "FashionTitleID", nID, true, "2020-12-12 12:12:12");
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FashionTitleID, nID);
				string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 30, nID);
				client.sendOthersCmd(427, cmdData);
			}
		}

		public void ModifyBuffFashionTitleID(GameClient client, int nID, bool writeToDB = true, bool notifyClient = true)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "10163", nID, writeToDB);
			if (notifyClient)
			{
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.BuffFashionID, nID);
				string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 40, nID);
				client.sendOthersCmd(427, cmdData);
			}
		}

		public int GetBufferIDBySpecialTitleID(int titleID)
		{
			lock (this.RuntimeData.Mutex)
			{
				int result;
				if (this.RuntimeData.SpecialTitleDict.TryGetValue(titleID, out result))
				{
					return result;
				}
			}
			return 0;
		}

		public void UpdateLuoLanChengZhuFasion(int bhid)
		{
			int roleID = -1;
			int num = 0;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			lock (this.RuntimeData.Mutex)
			{
				foreach (FashionData fashionData in this.RuntimeData.FashingDict.Values)
				{
					if (fashionData.Type == 1)
					{
						num = fashionData.ID;
						break;
					}
				}
			}
			if (bangHuiDetailData != null && num > 0)
			{
				int num2 = 0;
				int num3 = 0;
				GameClient gameClient = GameManager.ClientMgr.FindClient(this.RuntimeData.LuoLanChengZhuRoleID);
				if (gameClient != null && bangHuiDetailData.BZRoleID != gameClient.ClientData.RoleID)
				{
					this.DelLuoLanZhiYi(gameClient);
					num2 = gameClient.ClientData.Faction;
				}
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
				if (null != gameClient2)
				{
					this.GetFashionByMagic(gameClient2, 1, false);
					num3 = gameClient2.ClientData.Faction;
				}
				this.RuntimeData.LuoLanChengZhuRoleID = bangHuiDetailData.BZRoleID;
				if (num2 != num3)
				{
					int num4 = 0;
					GameClient nextClient;
					while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num4, false)) != null)
					{
						if (nextClient.ClientData.Faction == num3 && num3 != 0)
						{
							if (nextClient.ClientData.RoleID == this.RuntimeData.LuoLanChengZhuRoleID)
							{
								Global.UpdateBufferData(nextClient, BufferItemTypes.LuoLanChengZhu_Title, new double[]
								{
									1.0
								}, 1, false);
							}
							else
							{
								Global.UpdateBufferData(nextClient, BufferItemTypes.LuoLanGuiZu_Title, new double[]
								{
									1.0
								}, 1, false);
							}
						}
						else
						{
							GameClient client = nextClient;
							BufferItemTypes bufferItemType = BufferItemTypes.LuoLanChengZhu_Title;
							double[] actionParams = new double[1];
							Global.UpdateBufferData(client, bufferItemType, actionParams, 1, false);
							GameClient client2 = nextClient;
							BufferItemTypes bufferItemType2 = BufferItemTypes.LuoLanGuiZu_Title;
							actionParams = new double[1];
							Global.UpdateBufferData(client2, bufferItemType2, actionParams, 1, false);
						}
					}
				}
			}
		}

		public FashionBagData GetFashionBagData(GameClient client, GoodsData goodsData)
		{
			KeyValuePair<int, int> key = new KeyValuePair<int, int>(goodsData.GoodsID, goodsData.Forge_level);
			lock (this.RuntimeData.Mutex)
			{
				FashionBagData result;
				if (this.RuntimeData.FashionBagDict.TryGetValue(key, out result))
				{
					return result;
				}
			}
			return null;
		}

		public static GoodsData GetFashionGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].Id == id)
						{
							return client.ClientData.FashionGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static GoodsData GetFashionDataByGoodsID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].GoodsID == id)
						{
							return client.ClientData.FashionGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public void AddFashionGoodsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 6000)
			{
				if (null == client.ClientData.FashionGoodsDataList)
				{
					client.ClientData.FashionGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.FashionGoodsDataList)
				{
					client.ClientData.FashionGoodsDataList.Add(goodsData);
				}
				this.RefreshTitleFashionProps(client);
			}
		}

		public GoodsData AddFashionGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			this.AddFashionGoodsData(client, goodsData);
			return goodsData;
		}

		public void RemoveFashionGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.FashionGoodsDataList)
			{
				if (null != goodsData)
				{
					FashionData fashionData = null;
					foreach (FashionData fashionData2 in FashionManager.getInstance().RuntimeData.FashingDict.Values)
					{
						if (fashionData2.GoodsID == goodsData.GoodsID)
						{
							fashionData = fashionData2;
							break;
						}
					}
					if (fashionData != null)
					{
						this.UnloadFashion(client, fashionData, true);
					}
					lock (client.ClientData.FashionGoodsDataList)
					{
						if (null != fashionData)
						{
							EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(fashionData.GoodsID);
							if (null != equipPropItem)
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.FashionByGoodsProps,
									fashionData.TabID,
									fashionData.ID,
									PropsCacheManager.ConstExtProps
								});
							}
						}
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy).FashionGoods || goodsCatetoriy == 8)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.FashionByGoodsProps,
								3,
								goodsData.GoodsID,
								PropsCacheManager.ConstExtProps
							});
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
						client.ClientData.FashionGoodsDataList.Remove(goodsData);
						string resList = EventLogManager.NewGoodsDataPropString(goodsData);
						EventLogManager.AddTitleEvent(client, 0, 0, resList);
					}
					this.RefreshTitleFashionProps(client);
				}
			}
		}

		public static int GetFashionGoodsDataCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.FashionGoodsDataList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.FashionGoodsDataList.Count;
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessGetFashionList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (1 != array.Length)
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
				byte[] array2 = null;
				List<GoodsData> fashionGoodsDataList = gameClient.ClientData.FashionGoodsDataList;
				if (null != fashionGoodsDataList)
				{
					lock (fashionGoodsDataList)
					{
						if (fashionGoodsDataList.Count > 100)
						{
							CompressdGoodsDataList compressdGoodsDataList = new CompressdGoodsDataList(gameClient.ClientData.FashionGoodsDataList);
							array2 = DataHelper.ObjectToBytes<CompressdGoodsDataList>(compressdGoodsDataList);
						}
					}
				}
				if (null == array2)
				{
					array2 = DataHelper.ObjectToBytes<List<GoodsData>>(fashionGoodsDataList);
				}
				GameManager.ClientMgr.SendToClient(gameClient, array2, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessGetBuffFashionList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (1 != array.Length)
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
				byte[] buffer = DataHelper.ObjectToBytes<List<BufferData>>(gameClient.ClientData.BufferDataList);
				GameManager.ClientMgr.SendToClient(gameClient, buffer, nID);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		private int State = 0;

		private static FashionManager instance = new FashionManager();

		public FashionNamagerData RuntimeData = new FashionNamagerData();
	}
}
