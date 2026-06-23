using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.TarotData;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Tarot
{
	internal class TarotManager : ICmdProcessorEx, ICmdProcessor
	{
		public static TarotManager getInstance()
		{
			return TarotManager.instance;
		}

		public void Initialize()
		{
			string text = Global.GameResPath("Config/Tarot.xml");
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
			}
			IEnumerable<XElement> enumerable = xelement.Elements();
			foreach (XElement xml in enumerable)
			{
				TarotManager.TarotTemplate tarotTemplate = new TarotManager.TarotTemplate();
				tarotTemplate.Level = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "Level"));
				if (tarotTemplate.Level != 0)
				{
					tarotTemplate.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					tarotTemplate.Name = Global.GetSafeAttributeStr(xml, "Name");
					tarotTemplate.GoodsID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "GoodsID"));
					string[] array = Global.GetSafeAttributeStr(xml, "NeedGoods").Split(new char[]
					{
						','
					});
					tarotTemplate.NeedGoodID = Convert.ToInt32(array[0]);
					tarotTemplate.NeedPartCount = Convert.ToInt32(array[1]);
					if (TarotManager.TarotNeedCardNum.ContainsKey(tarotTemplate.NeedGoodID))
					{
						Dictionary<int, int> tarotNeedCardNum;
						int needGoodID;
						(tarotNeedCardNum = TarotManager.TarotNeedCardNum)[needGoodID = tarotTemplate.NeedGoodID] = tarotNeedCardNum[needGoodID] + tarotTemplate.NeedPartCount;
					}
					else
					{
						TarotManager.TarotNeedCardNum.Add(tarotTemplate.NeedGoodID, tarotTemplate.NeedPartCount);
					}
					if (TarotManager.TarotMaxLevelDict.ContainsKey(tarotTemplate.GoodsID) && TarotManager.TarotMaxLevelDict[tarotTemplate.GoodsID] < tarotTemplate.Level)
					{
						TarotManager.TarotMaxLevelDict[tarotTemplate.GoodsID] = tarotTemplate.Level;
					}
					else
					{
						TarotManager.TarotMaxLevelDict.Add(tarotTemplate.GoodsID, tarotTemplate.Level);
					}
					TarotManager.TarotTemplates.Add(tarotTemplate);
				}
			}
			TarotManager.TarotCardIds = TarotManager.TarotMaxLevelDict.Keys.ToList<int>();
			string[] array2 = GameManager.systemParamsList.GetParamValueByName("TarotKingCost").Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			TarotManager.KingItemId = Convert.ToInt32(array2[0]);
			TarotManager.UseKingItemCount = Convert.ToInt32(array2[1]);
			TarotManager.KingBuffTime = (long)(Convert.ToInt32(array2[2]) * 60);
			string[] array3 = GameManager.systemParamsList.GetParamValueByName("TarotKingNum").Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string value in array3)
			{
				TarotManager.KingBuffValueList.Add(Convert.ToInt32(value));
			}
			TCPCmdDispatcher.getInstance().registerProcessorEx(1701, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1702, 2, 2, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1703, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1704, 1, 1, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1705, 3, 3, TarotManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1701:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					string text = string.Format("{0}:{1}", Convert.ToInt32(this.ProcessTarotUpCmd(client, num)), num);
					client.sendCmd(nID, text, false);
				}
				catch (Exception e)
				{
					string text = string.Format("{0}:{1}", -1, cmdParams[0]);
					client.sendCmd(nID, text, false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_TAROT_UPORINIT", false, false);
				}
				break;
			case 1702:
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					int num = Convert.ToInt32(cmdParams[0]);
					byte pos = Convert.ToByte(cmdParams[1]);
					string text = string.Format("{0}:{1}", Convert.ToInt32(this.ProcessSetTarotPosCmd(client, num, pos)), num);
					client.sendCmd(nID, text, false);
				}
				catch (Exception e)
				{
					string text = string.Format("{0}:{1}", -1, cmdParams[0]);
					client.sendCmd(nID, text, false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_SET_TAROTPOS", false, false);
				}
				break;
			case 1703:
				try
				{
					string text = string.Empty;
					int num2 = Convert.ToInt32(this.ProcessUseKingPrivilegeCmd(client, out text));
					client.sendCmd(nID, string.Format("{0}:{1}", num2, text), false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_USE_TAROTKINGPRIVILEGE", false, false);
				}
				break;
			case 1704:
				try
				{
					TarotSystemData tarotData = client.ClientData.TarotData;
					client.sendCmd<TarotSystemData>(nID, tarotData, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_USE_TAROTKINGPRIVILEGE", false, false);
				}
				break;
			case 1705:
				if (cmdParams == null || cmdParams.Length != 3)
				{
					return false;
				}
				try
				{
					int num3 = Convert.ToInt32(cmdParams[0]);
					int num = Convert.ToInt32(cmdParams[1]);
					int num4 = Convert.ToInt32(cmdParams[2]);
					int num5 = 0;
					int num2 = Convert.ToInt32(this.ProcessTarotMoneyCmd(client, num, num4, num3, out num5));
					string text = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num2,
						num,
						num5,
						num3
					});
					client.sendCmd(nID, text, false);
				}
				catch (Exception e)
				{
					client.sendCmd(nID, -1.ToString(), false);
					DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_TAROT_MONEY_NUM", false, false);
				}
				break;
			}
			return true;
		}

		public TarotManager.ETarotResult ProcessTarotUpCmd(GameClient client, int goodID)
		{
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, 79, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				TarotCardData currentTarot = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == goodID);
				if (currentTarot == null)
				{
					currentTarot = new TarotCardData();
					currentTarot.GoodId = goodID;
					tarotData.TarotCardDatas.Add(currentTarot);
				}
				if (currentTarot.Level >= TarotManager.TarotMaxLevelDict[goodID])
				{
					result = TarotManager.ETarotResult.MaxLevel;
				}
				else
				{
					TarotManager.TarotTemplate tarotTemplate = TarotManager.TarotTemplates.Find((TarotManager.TarotTemplate x) => x.GoodsID == goodID && x.Level == currentTarot.Level + 1);
					if (tarotTemplate == null)
					{
						result = TarotManager.ETarotResult.Error;
					}
					else if (currentTarot.TarotMoney < tarotTemplate.NeedPartCount)
					{
						result = TarotManager.ETarotResult.NeedPart;
					}
					else
					{
						currentTarot.TarotMoney -= tarotTemplate.NeedPartCount;
						currentTarot.Level++;
						this.UpdataPalyerTarotAttr(client);
						TarotManager.UpdateTarotData2DB(client, currentTarot, null);
						result = TarotManager.ETarotResult.Success;
					}
				}
			}
			return result;
		}

		public TarotManager.ETarotResult ProcessSetTarotPosCmd(GameClient client, int goodID, byte pos)
		{
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, 79, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else if (pos < 0 || pos > 6)
			{
				result = TarotManager.ETarotResult.Error;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				TarotCardData tarotCardData = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == goodID);
				if (tarotCardData == null)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else if (tarotCardData.Postion == pos)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else
				{
					if (pos > 0)
					{
						if (tarotCardData.Postion > 0)
						{
							return TarotManager.ETarotResult.Error;
						}
						TarotCardData tarotCardData2 = tarotData.TarotCardDatas.Find((TarotCardData x) => x.Postion == pos);
						if (tarotCardData2 != null)
						{
							tarotCardData2.Postion = 0;
						}
					}
					tarotCardData.Postion = pos;
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, tarotCardData, null);
					result = TarotManager.ETarotResult.Success;
				}
			}
			return result;
		}

		public TarotManager.ETarotResult ProcessUseKingPrivilegeCmd(GameClient client, out string strret)
		{
			strret = string.Empty;
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, 79, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				TarotSystemData tarotData = client.ClientData.TarotData;
				if (tarotData.KingData.StartTime > 0L)
				{
					tarotData.KingData = new TarotKingData();
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
					result = TarotManager.ETarotResult.Success;
				}
				else
				{
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, TarotManager.KingItemId);
					if (totalGoodsCountByID < TarotManager.UseKingItemCount)
					{
						result = TarotManager.ETarotResult.ItemNotEnough;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						if (Global.UseGoodsBindOrNot(client, TarotManager.KingItemId, TarotManager.UseKingItemCount, true, out flag, out flag2) < 1)
						{
							result = TarotManager.ETarotResult.NeedPart;
						}
						else
						{
							tarotData.KingData.StartTime = TimeUtil.NOW();
							tarotData.KingData.BufferSecs = TarotManager.KingBuffTime;
							TarotManager.TarotCardIds = Global.RandomSortList<int>(TarotManager.TarotCardIds);
							TarotManager.KingBuffValueList = Global.RandomSortList<int>(TarotManager.KingBuffValueList);
							tarotData.KingData.AddtionDict = new Dictionary<int, int>();
							int num = TarotManager.KingBuffValueList[0];
							if (num < 3)
							{
								result = TarotManager.ETarotResult.Error;
							}
							else
							{
								for (int i = 0; i < 3; i++)
								{
									int num2;
									if (i < 2)
									{
										num2 = Global.GetRandomNumber(0, num - 3);
										num -= num2;
									}
									else
									{
										num2 = num - 3;
									}
									int num3 = TarotManager.TarotCardIds[i];
									object obj = strret;
									strret = string.Concat(new object[]
									{
										obj,
										num3,
										":",
										num2 + 1,
										":"
									});
									tarotData.KingData.AddtionDict.Add(num3, num2 + 1);
								}
								this.UpdataPalyerTarotAttr(client);
								TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
								strret = strret.TrimEnd(new char[]
								{
									':'
								});
								result = TarotManager.ETarotResult.Success;
							}
						}
					}
				}
			}
			return result;
		}

		public TarotManager.ETarotResult ProcessTarotMoneyCmd(GameClient client, int goodId, int num, int dbid, out int resNum)
		{
			resNum = 0;
			TarotManager.ETarotResult result;
			if (!GlobalNew.IsGongNengOpened(client, 79, false))
			{
				result = TarotManager.ETarotResult.NotOpen;
			}
			else
			{
				GoodsData goodsByDbID = Global.GetGoodsByDbID(client, dbid);
				if (goodsByDbID == null)
				{
					result = TarotManager.ETarotResult.Error;
				}
				else if (TarotManager.TarotNeedCardNum.ContainsKey(goodId))
				{
					if (num < 0 || num > goodsByDbID.GCount)
					{
						result = TarotManager.ETarotResult.MoneyNumError;
					}
					else
					{
						TarotManager.TarotTemplate nextTemp = TarotManager.TarotTemplates.Find((TarotManager.TarotTemplate x) => x.NeedGoodID == goodId);
						if (nextTemp == null)
						{
							result = TarotManager.ETarotResult.NotFindGood;
						}
						else
						{
							TarotSystemData tarotData = client.ClientData.TarotData;
							TarotCardData tarotCardData = tarotData.TarotCardDatas.Find((TarotCardData x) => x.GoodId == nextTemp.GoodsID);
							if (tarotCardData == null)
							{
								tarotCardData = new TarotCardData();
								tarotCardData.GoodId = nextTemp.GoodsID;
								tarotData.TarotCardDatas.Add(tarotCardData);
							}
							int num2 = TarotManager.TarotNeedCardNum[goodId] - this.CountTarotNowToCurrLevel(goodId, tarotCardData.Level) - tarotCardData.TarotMoney;
							if (num2 == 0)
							{
								result = TarotManager.ETarotResult.HasMaxNum;
							}
							else
							{
								if (num > num2)
								{
									num = num2;
								}
								if (GameManager.ClientMgr.NotifyUseGoodsByDbId(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, dbid, num, false, false))
								{
									GoodsData goodsByDbID2 = Global.GetGoodsByDbID(client, dbid);
									if (null != goodsByDbID2)
									{
										resNum = goodsByDbID2.GCount;
									}
									tarotCardData.TarotMoney += num;
									EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.TarotMoney, LogRecordType.TarotMoney, new object[]
									{
										num,
										tarotCardData.TarotMoney,
										dbid,
										"塔罗牌货币增加"
									});
									this.UpdataPalyerTarotAttr(client);
									TarotManager.UpdateTarotData2DB(client, tarotCardData, null);
									GameManager.logDBCmdMgr.AddDBLogInfo(dbid, "塔罗牌货币", "塔罗牌", client.ClientData.RoleName, "系统", "修改", 0, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
									result = TarotManager.ETarotResult.Success;
								}
								else
								{
									result = TarotManager.ETarotResult.UseGoodError;
								}
							}
						}
					}
				}
				else
				{
					result = TarotManager.ETarotResult.GoodIdError;
				}
			}
			return result;
		}

		public int CountTarotNowToCurrLevel(int cardId, int level)
		{
			int num = 0;
			foreach (TarotManager.TarotTemplate tarotTemplate in TarotManager.TarotTemplates)
			{
				if (tarotTemplate.NeedGoodID == cardId && tarotTemplate.Level <= level)
				{
					num += tarotTemplate.NeedPartCount;
				}
			}
			return num;
		}

		public void RemoveTarotKingData(GameClient client)
		{
			TarotSystemData tarotData = client.ClientData.TarotData;
			if (tarotData.KingData.StartTime != 0L)
			{
				long num = TimeUtil.NOW();
				if (num - tarotData.KingData.StartTime >= tarotData.KingData.BufferSecs * 1000L)
				{
					tarotData.KingData = new TarotKingData();
					this.UpdataPalyerTarotAttr(client);
					TarotManager.UpdateTarotData2DB(client, null, tarotData.KingData);
				}
			}
		}

		public void UpdataPalyerTarotAttr(GameClient client)
		{
			EquipPropItem equipPropItem = new EquipPropItem();
			double[] extProps = equipPropItem.ExtProps;
			foreach (TarotCardData tarotCardData in client.ClientData.TarotData.TarotCardDatas)
			{
				EquipPropItem equipPropItem2 = GameManager.EquipPropsMgr.FindEquipPropItem(tarotCardData.GoodId);
				if (tarotCardData.Postion > 0)
				{
					for (int i = 0; i < extProps.Length; i++)
					{
						int num = 0;
						if (client.ClientData.TarotData.KingData.AddtionDict.ContainsKey(tarotCardData.GoodId))
						{
							num = client.ClientData.TarotData.KingData.AddtionDict[tarotCardData.GoodId];
						}
						extProps[i] += equipPropItem2.ExtProps[i] * (double)(tarotCardData.Level + num);
					}
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.TarotCard,
				0,
				extProps
			});
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
		}

		private static void UpdateTarotData2DB(GameClient client, TarotCardData tarotData, TarotKingData tarotKingBuffData)
		{
			string[] array = null;
			string arg = (tarotData == null) ? "-1" : tarotData.GetDataStrInfo();
			string arg2 = (tarotKingBuffData == null) ? "-1" : tarotKingBuffData.GetDataStrInfo();
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, arg, arg2);
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 20100, strcmd, out array, client.ServerId);
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		private static List<TarotManager.TarotTemplate> TarotTemplates = new List<TarotManager.TarotTemplate>();

		private static Dictionary<int, int> TarotMaxLevelDict = new Dictionary<int, int>();

		private static Dictionary<int, int> TarotNeedCardNum = new Dictionary<int, int>();

		private static List<int> TarotCardIds = new List<int>();

		private static int KingItemId = 0;

		private static long KingBuffTime = 0L;

		private static int UseKingItemCount = 0;

		private static List<int> KingBuffValueList = new List<int>();

		private static TarotManager instance = new TarotManager();

		public enum ETarotResult
		{
			Error = -1,
			Success,
			Fail,
			MaxLevel,
			NeedPart,
			PartSuitIsMax,
			NotOpen,
			PartNumError,
			PosError,
			ItemNotEnough,
			HasMaxNum,
			MoneyNumError,
			GoodIdError,
			UseGoodError,
			NotFindCard,
			NotFindGood
		}

		public class TarotTemplate
		{
			public int ID { get; set; }

			public string Name { get; set; }

			public int GoodsID { get; set; }

			public int Level { get; set; }

			public int NeedGoodID { get; set; }

			public int NeedPartCount { get; set; }
		}
	}
}
