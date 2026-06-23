using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic.MerlinMagicBook
{
	public class MerlinMagicBookManager
	{
		private void LoadMerlinLevelUpConfigData()
		{
			try
			{
				lock (this.MerlinLevelUpConfigDict)
				{
					string text = "Config/Merlin/MagicBook.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.MerlinLevelUpConfigDict.Clear();
						foreach (XElement xml in enumerable)
						{
							if ((int)Global.GetSafeAttributeLong(xml, "Level") > 1)
							{
								MerlinLevelUpConfigData merlinLevelUpConfigData = new MerlinLevelUpConfigData();
								merlinLevelUpConfigData._Level = (int)Global.GetSafeAttributeLong(xml, "Level");
								merlinLevelUpConfigData._LuckyOne = (int)Global.GetSafeAttributeLong(xml, "LuckyOne");
								merlinLevelUpConfigData._LuckyTwo = (int)Global.GetSafeAttributeLong(xml, "LuckyTwo");
								merlinLevelUpConfigData._Rate = Global.GetSafeAttributeDouble(xml, "LuckyTwoRate");
								long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "NeedGoods", -1);
								if (safeAttributeLongArray.Length != 2)
								{
									LogManager.WriteLog(2, "梅林魔法书升阶数据有误，无法读取", null, true);
									break;
								}
								merlinLevelUpConfigData._NeedGoodsID = (int)safeAttributeLongArray[0];
								merlinLevelUpConfigData._NeedGoodsCount = (int)safeAttributeLongArray[1];
								merlinLevelUpConfigData._NeedDiamond = (int)Global.GetSafeAttributeLong(xml, "NeedZuanShi");
								this.MerlinLevelUpConfigDict[merlinLevelUpConfigData._Level] = merlinLevelUpConfigData;
							}
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinLevelUpConfigData", new object[0])));
			}
		}

		private void LoadMerlinStarUpConfigData()
		{
			try
			{
				lock (this.MerlinStarUpConfigDict)
				{
					string text = "Config/Merlin/MagicBookStar.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.MerlinStarUpConfigDict.Clear();
						foreach (XElement xml in enumerable)
						{
							MerlinStarUpConfigData merlinStarUpConfigData = new MerlinStarUpConfigData();
							merlinStarUpConfigData._Level = (int)Global.GetSafeAttributeLong(xml, "Level");
							merlinStarUpConfigData._StarNum = (int)Global.GetSafeAttributeLong(xml, "Star");
							merlinStarUpConfigData._MinAttackV = (int)Global.GetSafeAttributeLong(xml, "MinAttackV");
							merlinStarUpConfigData._MaxAttackV = (int)Global.GetSafeAttributeLong(xml, "MaxAttackV");
							merlinStarUpConfigData._MinMAttackV = (int)Global.GetSafeAttributeLong(xml, "MinMAttackV");
							merlinStarUpConfigData._MaxMAttackV = (int)Global.GetSafeAttributeLong(xml, "MaxMAttackV");
							merlinStarUpConfigData._MinDefenseV = (int)Global.GetSafeAttributeLong(xml, "MinDefenseV");
							merlinStarUpConfigData._MaxDefenseV = (int)Global.GetSafeAttributeLong(xml, "MaxDefenseV");
							merlinStarUpConfigData._MinMDefenseV = (int)Global.GetSafeAttributeLong(xml, "MinMDefenseV");
							merlinStarUpConfigData._MaxMDefenseV = (int)Global.GetSafeAttributeLong(xml, "MaxMDefenseV");
							merlinStarUpConfigData._HitV = (int)Global.GetSafeAttributeLong(xml, "HitV");
							merlinStarUpConfigData._DodgeV = (int)Global.GetSafeAttributeLong(xml, "Dodge");
							merlinStarUpConfigData._MaxHpV = (int)Global.GetSafeAttributeLong(xml, "MaxLifeV");
							merlinStarUpConfigData._ReviveP = Global.GetSafeAttributeDouble(xml, "Revive");
							merlinStarUpConfigData._MpRecoverP = Global.GetSafeAttributeDouble(xml, "MagicRecover");
							long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "NeedGoods", -1);
							if (safeAttributeLongArray.Length == 2)
							{
								merlinStarUpConfigData._NeedGoodsID = (int)safeAttributeLongArray[0];
								merlinStarUpConfigData._NeedGoodsCount = (int)safeAttributeLongArray[1];
							}
							merlinStarUpConfigData._NeedDiamond = (int)Global.GetSafeAttributeLong(xml, "NeedZuanShi");
							merlinStarUpConfigData._NeedExp = (int)Global.GetSafeAttributeLong(xml, "StarExp");
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GrowUp");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(2, "梅林魔法书升星成长经验与暴击率有误，无法读取", null, true);
								break;
							}
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							merlinStarUpConfigData._AddExp = new int[2];
							merlinStarUpConfigData._CritPercent = new double[2];
							if (array.Length == 2)
							{
								for (int i = 0; i < array.Length; i++)
								{
									string[] array2 = array[i].Split(new char[]
									{
										','
									});
									if (array2.Length == 2)
									{
										merlinStarUpConfigData._AddExp[i] = Convert.ToInt32(array2[0]);
										merlinStarUpConfigData._CritPercent[i] = Convert.ToDouble(array2[1]);
									}
								}
							}
							int merlinStarUpKey = this.GetMerlinStarUpKey(merlinStarUpConfigData._Level, merlinStarUpConfigData._StarNum);
							this.MerlinStarUpConfigDict[merlinStarUpKey] = merlinStarUpConfigData;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinStarUpConfigData", new object[0])));
			}
		}

		private void LoadMerlinSecretConfigData()
		{
			try
			{
				lock (this.MerlinSecretConfigDict)
				{
					string text = "Config/Merlin/MagicWord.xml";
					GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
					XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
					if (null == xelement)
					{
						LogManager.WriteLog(2, string.Format("加载{0}时出错!!!文件异常", text), null, true);
					}
					else
					{
						IEnumerable<XElement> enumerable = xelement.Elements();
						this.MerlinSecretConfigDict.Clear();
						foreach (XElement xml in enumerable)
						{
							MerlinSecretConfigData merlinSecretConfigData = new MerlinSecretConfigData();
							merlinSecretConfigData._Level = (int)Global.GetSafeAttributeLong(xml, "Level");
							long[] safeAttributeLongArray = Global.GetSafeAttributeLongArray(xml, "NeedGoods", -1);
							if (safeAttributeLongArray.Length != 2)
							{
								LogManager.WriteLog(2, "梅林魔法书秘语数据有误，无法读取", null, true);
								break;
							}
							merlinSecretConfigData._NeedGoodsID = (int)safeAttributeLongArray[0];
							merlinSecretConfigData._NeedGoodsCount = (int)safeAttributeLongArray[1];
							long[] safeAttributeLongArray2 = Global.GetSafeAttributeLongArray(xml, "Num", -1);
							merlinSecretConfigData._Num = new int[safeAttributeLongArray2.Length];
							for (int i = 0; i < safeAttributeLongArray2.Length; i++)
							{
								merlinSecretConfigData._Num[i] = Convert.ToInt32(safeAttributeLongArray2[i]);
							}
							this.MerlinSecretConfigDict[merlinSecretConfigData._Level] = merlinSecretConfigData;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinSecretConfigData", new object[0])));
			}
		}

		private int GetMerlinStarUpKey(int nLevel, int nStarNum)
		{
			return nLevel * 1000 + nStarNum;
		}

		private bool IsMerlinSecretTime(GameClient client)
		{
			long num = TimeUtil.NOW();
			return num - client.ClientData.MerlinData._ToTicks < 0L;
		}

		private void RefreshMerlinSecondAttr(GameClient client, int nLevel, int nStarNum)
		{
			int merlinStarUpKey = this.GetMerlinStarUpKey(nLevel, nStarNum);
			MerlinStarUpConfigData merlinStarUpConfigData = null;
			lock (this.MerlinStarUpConfigDict)
			{
				if (!this.MerlinStarUpConfigDict.TryGetValue(merlinStarUpKey, out merlinStarUpConfigData) || null == merlinStarUpConfigData)
				{
					return;
				}
			}
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				7,
				merlinStarUpConfigData._MinAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				8,
				merlinStarUpConfigData._MaxAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				9,
				merlinStarUpConfigData._MinMAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				10,
				merlinStarUpConfigData._MaxMAttackV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				3,
				merlinStarUpConfigData._MinDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				4,
				merlinStarUpConfigData._MaxDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				5,
				merlinStarUpConfigData._MinMDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				6,
				merlinStarUpConfigData._MaxMDefenseV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				18,
				merlinStarUpConfigData._HitV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				19,
				merlinStarUpConfigData._DodgeV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				13,
				merlinStarUpConfigData._MaxHpV
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				15,
				60,
				merlinStarUpConfigData._ReviveP
			});
		}

		private void RefreshMerlinSecretSecondAttr(GameClient client)
		{
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(0))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					56,
					client.ClientData.MerlinData._ActiveAttr[0] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(1))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					57,
					client.ClientData.MerlinData._ActiveAttr[1] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(2))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					58,
					client.ClientData.MerlinData._ActiveAttr[2] / 100.0
				});
			}
			if (client.ClientData.MerlinData._ActiveAttr.ContainsKey(3))
			{
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					15,
					59,
					client.ClientData.MerlinData._ActiveAttr[3] / 100.0
				});
			}
		}

		private void ResetActiveSecretAttr(GameClient client)
		{
			for (int i = 0; i < client.ClientData.MerlinData._ActiveAttr.Count; i++)
			{
				client.ClientData.MerlinData._ActiveAttr[i] = 0.0;
			}
		}

		private void ResetUnActiveSecretAttr(GameClient client)
		{
			for (int i = 0; i < client.ClientData.MerlinData._UnActiveAttr.Count; i++)
			{
				client.ClientData.MerlinData._UnActiveAttr[i] = 0.0;
			}
		}

		private EMerlinStarUpErrorCode MerlinStarUp(GameClient client, bool bIsDiamond, out int nIsCrit, out int nOutAddExp)
		{
			nIsCrit = 0;
			nOutAddExp = 0;
			int roleID = client.ClientData.RoleID;
			int starNum = client.ClientData.MerlinData._StarNum;
			int level = client.ClientData.MerlinData._Level;
			string text = "";
			int upStarMode = 0;
			try
			{
				if (level <= 0 || level > MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinStarUpErrorCode.LevelError;
				}
				if (starNum < 0)
				{
					return EMerlinStarUpErrorCode.StarError;
				}
				if (starNum >= MerlinSystemParamsConfigData._MaxStarNum)
				{
					return EMerlinStarUpErrorCode.MaxStarNum;
				}
				MerlinStarUpConfigData merlinStarUpConfigData = null;
				int merlinStarUpKey = this.GetMerlinStarUpKey(level, starNum + 1);
				lock (this.MerlinStarUpConfigDict)
				{
					if (!this.MerlinStarUpConfigDict.TryGetValue(merlinStarUpKey, out merlinStarUpConfigData) || null == merlinStarUpConfigData)
					{
						return EMerlinStarUpErrorCode.StarDataError;
					}
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(merlinStarUpConfigData._NeedGoodsID, out systemXmlItem))
				{
					return EMerlinStarUpErrorCode.NeedGoodsIDError;
				}
				if (merlinStarUpConfigData._NeedGoodsCount <= 0)
				{
					return EMerlinStarUpErrorCode.NeedGoodsCountError;
				}
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, merlinStarUpConfigData._NeedGoodsID);
				if (totalGoodsCountByID < merlinStarUpConfigData._NeedGoodsCount)
				{
					if (!bIsDiamond)
					{
						return EMerlinStarUpErrorCode.GoodsNotEnough;
					}
					upStarMode = 1;
				}
				switch (upStarMode)
				{
				case 0:
				{
					bool flag2 = false;
					bool flag3 = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, merlinStarUpConfigData._NeedGoodsID, merlinStarUpConfigData._NeedGoodsCount, false, out flag2, out flag3, false))
					{
						return EMerlinStarUpErrorCode.GoodsNotEnough;
					}
					GoodsData goodsData = new GoodsData
					{
						GoodsID = merlinStarUpConfigData._NeedGoodsID,
						GCount = merlinStarUpConfigData._NeedGoodsCount
					};
					text += EventLogManager.NewGoodsDataPropString(goodsData);
					break;
				}
				case 1:
				{
					int userMoney = client.ClientData.UserMoney;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, merlinStarUpConfigData._NeedDiamond, "梅林魔法书升星", true, true, false, DaiBiSySType.MeiLingZhiShu))
					{
						return EMerlinStarUpErrorCode.DiamondNotEnough;
					}
					text += EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
					{
						-merlinStarUpConfigData._NeedDiamond,
						userMoney,
						client.ClientData.UserMoney
					});
					break;
				}
				}
				int randomNumber = Global.GetRandomNumber(0, 10001);
				int num = (int)(merlinStarUpConfigData._CritPercent[1] * 10000.0);
				int i = 0;
				if (randomNumber < num)
				{
					i = merlinStarUpConfigData._AddExp[1];
					nIsCrit = 1;
				}
				else
				{
					i = merlinStarUpConfigData._AddExp[0];
				}
				nOutAddExp = i;
				while (i > 0)
				{
					int needExp = merlinStarUpConfigData._NeedExp;
					int num2 = needExp - client.ClientData.MerlinData._StarExp;
					if (i < num2)
					{
						client.ClientData.MerlinData._StarExp += i;
						break;
					}
					client.ClientData.MerlinData._StarNum++;
					client.ClientData.MerlinData._StarExp = 0;
					i -= num2;
					if (client.ClientData.MerlinData._StarNum >= MerlinSystemParamsConfigData._MaxStarNum)
					{
						if (level < MerlinSystemParamsConfigData._MaxLevelNum)
						{
							client.ClientData.MerlinData._StarExp += i;
						}
						break;
					}
					if (i <= 0)
					{
						break;
					}
					merlinStarUpKey = this.GetMerlinStarUpKey(level, client.ClientData.MerlinData._StarNum);
					lock (this.MerlinStarUpConfigDict)
					{
						if (!this.MerlinStarUpConfigDict.TryGetValue(merlinStarUpKey, out merlinStarUpConfigData))
						{
							break;
						}
					}
				}
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					roleID,
					"*",
					"*",
					client.ClientData.MerlinData._StarNum,
					client.ClientData.MerlinData._StarExp,
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
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				if (starNum != client.ClientData.MerlinData._StarNum)
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, level, starNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
				if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				EventLogManager.AddMerlinBookStarEvent(client, upStarMode, i, starNum, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, client.ClientData.MerlinData._StarExp, text);
				return EMerlinStarUpErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinStarUpErrorCode.Error;
		}

		private EMerlinLevelUpErrorCode MerlinLevelUp(GameClient client, bool bIsDiamond)
		{
			string text = "";
			int roleID = client.ClientData.RoleID;
			int level = client.ClientData.MerlinData._Level;
			int starNum = client.ClientData.MerlinData._StarNum;
			int levelUpFailNum = client.ClientData.MerlinData._LevelUpFailNum;
			int starExp = client.ClientData.MerlinData._StarExp;
			int nUpMode = 0;
			try
			{
				if (level <= 0)
				{
					return EMerlinLevelUpErrorCode.LevelError;
				}
				if (level >= MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinLevelUpErrorCode.MaxLevelNum;
				}
				if (starNum < MerlinSystemParamsConfigData._MaxStarNum)
				{
					return EMerlinLevelUpErrorCode.NotMaxStarNum;
				}
				MerlinLevelUpConfigData merlinLevelUpConfigData = null;
				lock (this.MerlinLevelUpConfigDict)
				{
					if (!this.MerlinLevelUpConfigDict.TryGetValue(level + 1, out merlinLevelUpConfigData) || null == merlinLevelUpConfigData)
					{
						return EMerlinLevelUpErrorCode.LevelDataError;
					}
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(merlinLevelUpConfigData._NeedGoodsID, out systemXmlItem))
				{
					return EMerlinLevelUpErrorCode.NeedGoodsIDError;
				}
				if (merlinLevelUpConfigData._NeedGoodsCount <= 0)
				{
					return EMerlinLevelUpErrorCode.NeedGoodsCountError;
				}
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, merlinLevelUpConfigData._NeedGoodsID);
				if (totalGoodsCountByID < merlinLevelUpConfigData._NeedGoodsCount)
				{
					if (!bIsDiamond)
					{
						return EMerlinLevelUpErrorCode.GoodsNotEnough;
					}
					nUpMode = 1;
				}
				switch (nUpMode)
				{
				case 0:
				{
					bool flag2 = false;
					bool flag3 = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, merlinLevelUpConfigData._NeedGoodsID, merlinLevelUpConfigData._NeedGoodsCount, false, out flag2, out flag3, false))
					{
						return EMerlinLevelUpErrorCode.GoodsNotEnough;
					}
					GoodsData goodsData = new GoodsData
					{
						GoodsID = merlinLevelUpConfigData._NeedGoodsID,
						GCount = merlinLevelUpConfigData._NeedGoodsCount
					};
					text += EventLogManager.NewGoodsDataPropString(goodsData);
					break;
				}
				case 1:
				{
					int userMoney = client.ClientData.UserMoney;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, merlinLevelUpConfigData._NeedDiamond, "梅林魔法书升阶", true, true, false, DaiBiSySType.MeiLingZhiShu))
					{
						return EMerlinLevelUpErrorCode.DiamondNotEnough;
					}
					text += EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
					{
						-merlinLevelUpConfigData._NeedDiamond,
						userMoney,
						client.ClientData.UserMoney
					});
					break;
				}
				}
				if (client.ClientData.MerlinData._LuckyPoint <= 0)
				{
					client.ClientData.MerlinData._LuckyPoint = merlinLevelUpConfigData._LuckyOne;
				}
				client.ClientData.MerlinData._LuckyPoint++;
				string strCmd;
				if (client.ClientData.MerlinData._LuckyPoint < merlinLevelUpConfigData._LuckyTwo)
				{
					client.ClientData.MerlinData._LevelUpFailNum++;
					strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
					{
						roleID,
						"*",
						client.ClientData.MerlinData._LevelUpFailNum,
						"*",
						"*",
						client.ClientData.MerlinData._LuckyPoint,
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
					this.UpdateMerlinMagicBookData2DB(client, strCmd);
					return EMerlinLevelUpErrorCode.Fail;
				}
				int randomNumber = Global.GetRandomNumber(0, 10001);
				int num = (int)(merlinLevelUpConfigData._Rate * 10000.0);
				if (randomNumber < num)
				{
					client.ClientData.MerlinData._Level++;
					client.ClientData.MerlinData._LevelUpFailNum = 0;
					client.ClientData.MerlinData._StarNum = 0;
					client.ClientData.MerlinData._LuckyPoint = 0;
					if (client.ClientData.MerlinData._Level >= MerlinSystemParamsConfigData._MaxLevelNum)
					{
						client.ClientData.MerlinData._StarExp = 0;
					}
					strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
					{
						roleID,
						client.ClientData.MerlinData._Level,
						client.ClientData.MerlinData._LevelUpFailNum,
						client.ClientData.MerlinData._StarNum,
						client.ClientData.MerlinData._StarExp,
						client.ClientData.MerlinData._LuckyPoint,
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
					this.UpdateMerlinMagicBookData2DB(client, strCmd);
					EventLogManager.AddMerlinBookStarEvent(client, 2, 0, starNum, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, client.ClientData.MerlinData._StarExp, text);
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, level, starNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
					EventLogManager.AddMerlinBookLevEvent(client, nUpMode, levelUpFailNum, client.ClientData.MerlinData._LevelUpFailNum, level, client.ClientData.MerlinData._Level, starNum, client.ClientData.MerlinData._StarNum, starExp, client.ClientData.MerlinData._StarExp, text);
					return EMerlinLevelUpErrorCode.Success;
				}
				client.ClientData.MerlinData._LevelUpFailNum++;
				strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					roleID,
					"*",
					client.ClientData.MerlinData._LevelUpFailNum,
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
					"*"
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				return EMerlinLevelUpErrorCode.Fail;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinLevelUpErrorCode.Error;
		}

		private EMerlinSecretAttrUpdateErrorCode MerlinSecretAttrUpdate(GameClient client)
		{
			int roleID = client.ClientData.RoleID;
			int level = client.ClientData.MerlinData._Level;
			try
			{
				if (level <= 0 || level > MerlinSystemParamsConfigData._MaxLevelNum)
				{
					return EMerlinSecretAttrUpdateErrorCode.LevelError;
				}
				MerlinSecretConfigData merlinSecretConfigData = null;
				lock (this.MerlinSecretConfigDict)
				{
					if (!this.MerlinSecretConfigDict.TryGetValue(level, out merlinSecretConfigData) || null == merlinSecretConfigData)
					{
						return EMerlinSecretAttrUpdateErrorCode.SecretDataError;
					}
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(merlinSecretConfigData._NeedGoodsID, out systemXmlItem))
				{
					return EMerlinSecretAttrUpdateErrorCode.NeedGoodsIDError;
				}
				if (merlinSecretConfigData._NeedGoodsCount <= 0)
				{
					return EMerlinSecretAttrUpdateErrorCode.NeedGoodsCountError;
				}
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, merlinSecretConfigData._NeedGoodsID);
				if (totalGoodsCountByID < merlinSecretConfigData._NeedGoodsCount)
				{
					return EMerlinSecretAttrUpdateErrorCode.GoodsNotEnough;
				}
				bool flag2 = false;
				bool flag3 = false;
				if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, merlinSecretConfigData._NeedGoodsID, merlinSecretConfigData._NeedGoodsCount, false, out flag2, out flag3, false))
				{
					return EMerlinSecretAttrUpdateErrorCode.GoodsNotEnough;
				}
				int randomNumber = Global.GetRandomNumber(0, merlinSecretConfigData._Num.Length);
				int i = merlinSecretConfigData._Num[randomNumber];
				this.ResetUnActiveSecretAttr(client);
				while (i > 0)
				{
					int num = (int)(client.ClientData.MerlinData._UnActiveAttr[0] + client.ClientData.MerlinData._UnActiveAttr[1] + client.ClientData.MerlinData._UnActiveAttr[2] + client.ClientData.MerlinData._UnActiveAttr[3]);
					int num2 = MerlinSystemParamsConfigData._MaxSecretAttrNum * 4;
					if (num >= num2)
					{
						break;
					}
					int randomNumber2 = Global.GetRandomNumber(0, 4);
					if (client.ClientData.MerlinData._UnActiveAttr[randomNumber2] < (double)MerlinSystemParamsConfigData._MaxSecretAttrNum)
					{
						Dictionary<int, double> unActiveAttr;
						int key;
						(unActiveAttr = client.ClientData.MerlinData._UnActiveAttr)[key = randomNumber2] = unActiveAttr[key] + 1.0;
						i--;
					}
				}
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					roleID,
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
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				return EMerlinSecretAttrUpdateErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinSecretAttrUpdateErrorCode.Error;
		}

		private EMerlinSecretAttrReplaceErrorCode MerlinSecretAttrReplace(GameClient client)
		{
			bool flag = false;
			try
			{
				for (int i = 0; i < client.ClientData.MerlinData._UnActiveAttr.Count; i++)
				{
					if (client.ClientData.MerlinData._UnActiveAttr[i] > 0.0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return EMerlinSecretAttrReplaceErrorCode.NotUpdate;
				}
				for (int i = 0; i < client.ClientData.MerlinData._ActiveAttr.Count; i++)
				{
					client.ClientData.MerlinData._ActiveAttr[i] = client.ClientData.MerlinData._UnActiveAttr[i];
				}
				this.ResetUnActiveSecretAttr(client);
				this.RefreshMerlinSecretSecondAttr(client);
				client.ClientData.MerlinData._ToTicks = TimeUtil.NOW() + (long)(MerlinSystemParamsConfigData._MaxSecretTime * 60 * 1000);
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					"*",
					"*",
					"*",
					client.ClientData.MerlinData._ToTicks,
					client.ClientData.MerlinData._ActiveAttr[0],
					client.ClientData.MerlinData._ActiveAttr[1],
					client.ClientData.MerlinData._ActiveAttr[2],
					client.ClientData.MerlinData._ActiveAttr[3],
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				return EMerlinSecretAttrReplaceErrorCode.Success;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return EMerlinSecretAttrReplaceErrorCode.Error;
		}

		private void MerlinSecretAttrNotReplace(GameClient client)
		{
			try
			{
				this.ResetUnActiveSecretAttr(client);
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
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
					client.ClientData.MerlinData._UnActiveAttr[0],
					client.ClientData.MerlinData._UnActiveAttr[1],
					client.ClientData.MerlinData._UnActiveAttr[2],
					client.ClientData.MerlinData._UnActiveAttr[3]
				});
				this.UpdateMerlinMagicBookData2DB(client, strCmd);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		private bool CreateMerlinMagicBookData2DB(GameClient client)
		{
			try
			{
				byte[] array = DataHelper.ObjectToBytes<MerlinGrowthSaveDBData>(client.ClientData.MerlinData);
				byte[] bytes = BitConverter.GetBytes(client.ClientData.RoleID);
				byte[] array2 = new byte[array.Length + 4];
				Array.Copy(bytes, array2, 4);
				Array.Copy(array, 0, array2, 4, array.Length);
				return Global.sendToDB<bool, byte[]>(10203, array2, client.ServerId);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool UpdateMerlinMagicBookData2DB(GameClient client, string strCmd)
		{
			byte[] bytes = new UTF8Encoding().GetBytes(strCmd);
			return Global.sendToDB<bool, byte[]>(10204, bytes, client.ServerId);
		}

		private static string FormatUpdateDBMerlinStr(params object[] args)
		{
			string result;
			if (args.Length != 15)
			{
				LogManager.WriteLog(2, string.Format("FormatUpdateDBMerlinStr, 参数个数不对{0}", args.Length), null, true);
				result = null;
			}
			else
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}:{14}", args);
			}
			return result;
		}

		public void RefreshMerlinExcellenceAttr(GameClient client, int nLevel, int nStarNum, bool bToAdd)
		{
			int merlinStarUpKey = this.GetMerlinStarUpKey(nLevel, nStarNum);
			MerlinStarUpConfigData merlinStarUpConfigData = null;
			lock (this.MerlinStarUpConfigDict)
			{
				if (!this.MerlinStarUpConfigDict.TryGetValue(merlinStarUpKey, out merlinStarUpConfigData) || null == merlinStarUpConfigData)
				{
					return;
				}
			}
			if (merlinStarUpConfigData._MpRecoverP > 0.0)
			{
				if (bToAdd)
				{
					client.ClientData.ExcellenceProp[16] += merlinStarUpConfigData._MpRecoverP;
				}
				else
				{
					client.ClientData.ExcellenceProp[16] -= merlinStarUpConfigData._MpRecoverP;
				}
			}
		}

		public void OnLoginInitMerlinMagicBook(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					if (null == client.ClientData.MerlinData)
					{
						client.ClientData.MerlinData = new MerlinGrowthSaveDBData();
					}
					if (client.ClientData.MerlinData._Level < 1)
					{
						client.ClientData.MerlinData._RoleID = client.ClientData.RoleID;
						client.ClientData.MerlinData._Level = 1;
						client.ClientData.MerlinData._Occupation = Global.CalcOriginalOccupationID(client);
						this.ResetActiveSecretAttr(client);
						this.ResetUnActiveSecretAttr(client);
						this.CreateMerlinMagicBookData2DB(client);
						this.CheckMerlinSecretAttr(client);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public void OnLoginAddAttr(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinSecretSecondAttr(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public void LoadMerlinConfigData()
		{
			this.LoadMerlinLevelUpConfigData();
			this.LoadMerlinStarUpConfigData();
			this.LoadMerlinSecretConfigData();
		}

		public void LoadMerlinSystemParamsConfigData()
		{
			try
			{
				MerlinSystemParamsConfigData._ReviveCDTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ChongShengCD"));
				MerlinSystemParamsConfigData._MaxSecretAttrNum = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MagicWordMax"));
				MerlinSystemParamsConfigData._MaxSecretTime = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("MagicWordTime"));
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("MagicBookLevel", ',');
				if (paramValueIntArrayByName.Length != 2)
				{
					LogManager.WriteLog(2, "梅林魔法书最大阶数星数有误，无法读取", null, true);
				}
				else
				{
					MerlinSystemParamsConfigData._MaxLevelNum = paramValueIntArrayByName[0];
					MerlinSystemParamsConfigData._MaxStarNum = paramValueIntArrayByName[1];
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("load xml file : {0} fail", string.Format("Config/SystemParams.xml-LoadMerlinSystemParamsConfigData", new object[0])));
			}
		}

		public bool IsOpenMerlin(GameClient client)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = false;
			}
			else if (null == client)
			{
				result = false;
			}
			else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MerlinMagicBook"))
			{
				LogManager.WriteLog(2, string.Format("版本控制未开启梅林魔法书功能, RoleID={0}", client.ClientData.RoleID), null, true);
				result = false;
			}
			else
			{
				result = GlobalNew.IsGongNengOpened(client, 61, false);
			}
			return result;
		}

		public void CheckMerlinSecretAttr(GameClient client)
		{
			if (this.IsOpenMerlin(client))
			{
				if (this.IsMerlinSecretTime(client))
				{
					client._IconStateMgr.AddFlushIconState(14201, false);
				}
				else
				{
					client._IconStateMgr.AddFlushIconState(14201, true);
				}
			}
		}

		public void DoMerlinSecretTime(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					long num = TimeUtil.NOW();
					if (num - this.nextCheckTime >= 5000L)
					{
						this.nextCheckTime = num;
						if (!this.IsMerlinSecretTime(client))
						{
							if (client.ClientData.MerlinData._ToTicks > 0L)
							{
								client.ClientData.MerlinData._ToTicks = 0L;
								this.ResetActiveSecretAttr(client);
								this.RefreshMerlinSecretSecondAttr(client);
								string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
								{
									client.ClientData.RoleID,
									"*",
									"*",
									"*",
									"*",
									"*",
									client.ClientData.MerlinData._ToTicks,
									client.ClientData.MerlinData._ActiveAttr[0],
									client.ClientData.MerlinData._ActiveAttr[1],
									client.ClientData.MerlinData._ActiveAttr[2],
									client.ClientData.MerlinData._ActiveAttr[3],
									"*",
									"*",
									"*",
									"*"
								});
								this.UpdateMerlinMagicBookData2DB(client, strCmd);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
								this.CheckMerlinSecretAttr(client);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public void InitMerlinMagicBook(GameClient client)
		{
			try
			{
				if (this.IsOpenMerlin(client))
				{
					if (null == client.ClientData.MerlinData)
					{
						client.ClientData.MerlinData = new MerlinGrowthSaveDBData();
					}
					if (client.ClientData.MerlinData._Level < 1)
					{
						client.ClientData.MerlinData._RoleID = client.ClientData.RoleID;
						client.ClientData.MerlinData._Level = 1;
						client.ClientData.MerlinData._Occupation = Global.CalcOriginalOccupationID(client);
						this.ResetActiveSecretAttr(client);
						this.ResetUnActiveSecretAttr(client);
						this.CreateMerlinMagicBookData2DB(client);
						this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
						this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						this.CheckMerlinSecretAttr(client);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
		}

		public TCPProcessCmdResults ProcQueryMerlinMagicBookData(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (gameClient.ClientData.MerlinData == null || gameClient.ClientData.MerlinData._RoleID <= 0)
				{
					gameClient.ClientData.MerlinData = Global.sendToDB<MerlinGrowthSaveDBData, string>(10205, string.Format("{0}", gameClient.ClientData.RoleID), gameClient.ServerId);
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MerlinGrowthSaveDBData>(gameClient.ClientData.MerlinData, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcMerlinMagicBookStarUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				bool bIsDiamond = num2 != 0;
				int num3 = 0;
				int num4 = 0;
				EMerlinStarUpErrorCode emerlinStarUpErrorCode = this.MerlinStarUp(gameClient, bIsDiamond, out num3, out num4);
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)emerlinStarUpErrorCode,
					gameClient.ClientData.MerlinData._StarNum,
					gameClient.ClientData.MerlinData._StarExp,
					num3,
					num4
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcMerlinMagicBookLevelUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				bool bIsDiamond = num2 != 0;
				EMerlinLevelUpErrorCode emerlinLevelUpErrorCode = this.MerlinLevelUp(gameClient, bIsDiamond);
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)emerlinLevelUpErrorCode,
					gameClient.ClientData.MerlinData._StarNum,
					gameClient.ClientData.MerlinData._StarExp,
					gameClient.ClientData.MerlinData._Level,
					gameClient.ClientData.MerlinData._LevelUpFailNum
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcMerlinSecretAttrUpdate(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EMerlinSecretAttrUpdateErrorCode emerlinSecretAttrUpdateErrorCode = this.MerlinSecretAttrUpdate(gameClient);
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					(int)emerlinSecretAttrUpdateErrorCode,
					gameClient.ClientData.MerlinData._UnActiveAttr[0],
					gameClient.ClientData.MerlinData._UnActiveAttr[1],
					gameClient.ClientData.MerlinData._UnActiveAttr[2],
					gameClient.ClientData.MerlinData._UnActiveAttr[3]
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcMerlinSecretAttrReplace(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				EMerlinSecretAttrReplaceErrorCode emerlinSecretAttrReplaceErrorCode = this.MerlinSecretAttrReplace(gameClient);
				string data2 = string.Format("{0}", (int)emerlinSecretAttrReplaceErrorCode);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcMerlinSecretAttrNotReplace(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient == null || gameClient.ClientData.RoleID != num)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (!this.IsOpenMerlin(gameClient))
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				this.MerlinSecretAttrNotReplace(gameClient);
				string data2 = string.Format("{0}", 0);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public void GMMerlinStarUp1(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			this.MerlinStarUp(client, false, out num, out num2);
		}

		public void GMMerlinStarUp2(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			this.MerlinStarUp(client, true, out num, out num2);
		}

		public void GMMerlinLevelUp1(GameClient client)
		{
			this.MerlinLevelUp(client, false);
		}

		public void GMMerlinLevelUp2(GameClient client)
		{
			this.MerlinLevelUp(client, true);
		}

		public void GMMerlinSecretUpdate(GameClient client)
		{
			this.MerlinSecretAttrUpdate(client);
		}

		public void GMMerlinSecretReplace(GameClient client)
		{
			this.MerlinSecretAttrReplace(client);
		}

		public void GMMerlinSecretNotReplace(GameClient client)
		{
			this.MerlinSecretAttrNotReplace(client);
		}

		public void GMMerlinInit(GameClient client)
		{
			this.InitMerlinMagicBook(client);
		}

		public string GMMerlinLevelUpToN(GameClient client, int nLevel)
		{
			string lang;
			if (client == null || !this.IsOpenMerlin(client))
			{
				lang = GLang.GetLang(495, new object[0]);
			}
			else if (nLevel < 1)
			{
				lang = GLang.GetLang(496, new object[0]);
			}
			else
			{
				nLevel = Math.Min(nLevel, MerlinSystemParamsConfigData._MaxLevelNum);
				int level = client.ClientData.MerlinData._Level;
				int starNum = client.ClientData.MerlinData._StarNum;
				client.ClientData.MerlinData._Level = nLevel;
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					nLevel,
					0,
					"*",
					"*",
					0,
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
				if (!this.UpdateMerlinMagicBookData2DB(client, strCmd))
				{
					lang = GLang.GetLang(497, new object[0]);
				}
				else
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, level, starNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					lang = GLang.GetLang(498, new object[0]);
				}
			}
			return lang;
		}

		public string GMMerlinStarUpToN(GameClient client, int nStarNum)
		{
			string lang;
			if (client == null || !this.IsOpenMerlin(client))
			{
				lang = GLang.GetLang(495, new object[0]);
			}
			else if (nStarNum < 0)
			{
				lang = GLang.GetLang(499, new object[0]);
			}
			else
			{
				nStarNum = Math.Min(nStarNum, MerlinSystemParamsConfigData._MaxStarNum);
				int level = client.ClientData.MerlinData._Level;
				int starNum = client.ClientData.MerlinData._StarNum;
				client.ClientData.MerlinData._StarNum = nStarNum;
				string strCmd = MerlinMagicBookManager.FormatUpdateDBMerlinStr(new object[]
				{
					client.ClientData.RoleID,
					"*",
					"*",
					nStarNum,
					0,
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
				if (!this.UpdateMerlinMagicBookData2DB(client, strCmd))
				{
					lang = GLang.GetLang(500, new object[0]);
				}
				else
				{
					this.RefreshMerlinSecondAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum);
					this.RefreshMerlinExcellenceAttr(client, level, starNum, false);
					this.RefreshMerlinExcellenceAttr(client, client.ClientData.MerlinData._Level, client.ClientData.MerlinData._StarNum, true);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					lang = GLang.GetLang(501, new object[0]);
				}
			}
			return lang;
		}

		private Dictionary<int, MerlinLevelUpConfigData> MerlinLevelUpConfigDict = new Dictionary<int, MerlinLevelUpConfigData>();

		private Dictionary<int, MerlinStarUpConfigData> MerlinStarUpConfigDict = new Dictionary<int, MerlinStarUpConfigData>();

		private Dictionary<int, MerlinSecretConfigData> MerlinSecretConfigDict = new Dictionary<int, MerlinSecretConfigData>();

		private long nextCheckTime = 0L;
	}
}
