using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class ChengJiuManager : IManager
	{
		public static ChengJiuManager GetInstance()
		{
			return ChengJiuManager.Instance;
		}

		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(670, 2, UpGradeChengLevelCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU));
			return true;
		}

		public bool startup()
		{
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

		public static void InitChengJiuConfig()
		{
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.systemChengJiu.SystemXmlItemDict)
			{
				switch (keyValuePair.Value.GetIntValue("ID", -1))
				{
				case 7:
				{
					int intValue = keyValuePair.Value.GetIntValue("ChengJiuID", -1);
					if (intValue > 2050)
					{
						if (intValue > ChengJiuTypes.JunXianChengJiuEnd)
						{
							ChengJiuTypes.JunXianChengJiuEnd = keyValuePair.Key;
						}
					}
					break;
				}
				case 8:
				{
					int intValue = keyValuePair.Value.GetIntValue("ChengJiuID", -1);
					if (intValue > ChengJiuTypes.MainLineTaskEnd)
					{
						ChengJiuTypes.MainLineTaskEnd = keyValuePair.Key;
					}
					else if (intValue < ChengJiuTypes.MainLineTaskStart)
					{
						ChengJiuTypes.MainLineTaskStart = keyValuePair.Key;
					}
					break;
				}
				}
			}
		}

		public static void SetAchievementLevel(GameClient client, int level)
		{
			Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
			{
				(double)level - 1.0
			}, 0, true);
			client.ClientData.ChengJiuLevel = level;
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就等级", "GM", "系统", client.ClientData.RoleName, "修改", level, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
			ChengJiuManager.SetChengJiuLevel(client, client.ClientData.ChengJiuLevel, true);
			Global.BroadcastClientChuanQiChengJiu(client, level);
			GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiuLevel, client.ClientData.ChengJiuLevel);
		}

		public static void SetAchievementRuneLevel(GameClient client, int level)
		{
			AchievementRuneData achievementRuneData = new AchievementRuneData();
			AchievementRuneBasicData achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(level);
			achievementRuneData.RoleID = client.ClientData.RoleID;
			achievementRuneData.RuneID = achievementRuneBasicDataByID.RuneID;
			if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
			{
				achievementRuneData.UpResultType = 3;
			}
			ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
			client.ClientData.achievementRuneData = achievementRuneData;
			ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
		}

		public static void SetAchievementRuneCount(GameClient client, int count)
		{
			ChengJiuManager.ModifyAchievementRuneUpCount(client, count, true);
		}

		public static void SetAchievementRuneRate(GameClient client, int rate)
		{
			ChengJiuManager._runeRate = rate;
		}

		public static void initAchievementRune()
		{
			ChengJiuManager.LoadAchievementRuneBasicData();
			ChengJiuManager.LoadAchievementRuneSpecialData();
		}

		public static void initSetAchievementRuneProps(GameClient client)
		{
			if (GlobalNew.IsGongNengOpened(client, 53, false))
			{
				AchievementRuneData achievementRuneData = ChengJiuManager.GetAchievementRuneData(client);
				ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
			}
		}

		public static void LoadAchievementRuneBasicData()
		{
			string uri = "Config/ChengJiuFuWen.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ChengJiuFuWen.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ChengJiuManager._achievementRuneBasicList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							AchievementRuneBasicData achievementRuneBasicData = new AchievementRuneBasicData();
							achievementRuneBasicData.RuneID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							achievementRuneBasicData.RuneName = Convert.ToString(Global.GetDefAttributeStr(xelement2, "Name", ""));
							achievementRuneBasicData.LifeMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LifeV", "0"));
							achievementRuneBasicData.AttackMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddAttack", "0"));
							achievementRuneBasicData.DefenseMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddDefense", "0"));
							achievementRuneBasicData.DodgeMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Dodge", "0"));
							achievementRuneBasicData.AchievementCost = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "CostChengJiu", "0"));
							string text = Convert.ToString(Global.GetDefAttributeStr(xelement2, "QiangHua", ""));
							if (text.Length > 0)
							{
								achievementRuneBasicData.RateList = new List<int>();
								achievementRuneBasicData.AddNumList = new List<int[]>();
								string[] array = text.Split(new char[]
								{
									'|'
								});
								foreach (string text2 in array)
								{
									string[] array3 = text2.Split(new char[]
									{
										','
									});
									float num = float.Parse(array3[0]);
									achievementRuneBasicData.RateList.Add((int)(num * 100f));
									List<int> list = new List<int>();
									for (int j = 1; j < array3.Length; j++)
									{
										list.Add(int.Parse(array3[j]));
									}
									achievementRuneBasicData.AddNumList.Add(list.ToArray());
								}
							}
							ChengJiuManager._achievementRuneBasicList.Add(achievementRuneBasicData.RuneID, achievementRuneBasicData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ChengJiuFuWen.xml时文件出现异常!!!", ex, true);
				}
			}
		}

		public static void LoadAchievementRuneSpecialData()
		{
			string uri = "Config/ChengJiuSpecialAttribute.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ChengJiuSpecialAttribute.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					ChengJiuManager._achievementRuneSpecialList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							AchievementRuneSpecialData achievementRuneSpecialData = new AchievementRuneSpecialData();
							achievementRuneSpecialData.SpecialID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							achievementRuneSpecialData.RuneID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedFuWen", "0"));
							achievementRuneSpecialData.ZhuoYue = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "ZhuoYueYiJi", "0"));
							achievementRuneSpecialData.DiKang = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DiKangZhuoYueYiJi", "0"));
							ChengJiuManager._achievementRuneSpecialList.Add(achievementRuneSpecialData.RuneID, achievementRuneSpecialData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ChengJiuSpecialAttribute.xml时出现异常!!!", ex, true);
				}
			}
		}

		public static AchievementRuneBasicData GetAchievementRuneBasicDataByID(int id)
		{
			AchievementRuneBasicData result;
			if (ChengJiuManager._achievementRuneBasicList.ContainsKey(id))
			{
				result = ChengJiuManager._achievementRuneBasicList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static AchievementRuneSpecialData GetAchievementRuneSpecialDataByID(int id)
		{
			AchievementRuneSpecialData result;
			if (ChengJiuManager._achievementRuneSpecialList.ContainsKey(id))
			{
				result = ChengJiuManager._achievementRuneSpecialList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int GetAchievementRuneUpCount(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "AchievementRuneUpCount");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count > 0)
			{
				num2 = roleParamsIntListFromDB[0];
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (num2 == dayOfYear)
			{
				num = roleParamsIntListFromDB[1];
			}
			else
			{
				ChengJiuManager.ModifyAchievementRuneUpCount(client, num, true);
			}
			return num;
		}

		public static void ModifyAchievementRuneUpCount(GameClient client, int count, bool writeToDB = false)
		{
			List<int> list = new List<int>();
			list.AddRange(new int[]
			{
				TimeUtil.NowDateTime().DayOfYear,
				count
			});
			Global.SaveRoleParamsIntListToDB(client, list, "AchievementRuneUpCount", writeToDB);
		}

		public static int GetAchievementRuneDiamond(GameClient client, int upCount)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ChengJiuFuWenZuanShi", ',');
			if (upCount >= paramValueIntArrayByName.Length)
			{
				upCount = paramValueIntArrayByName.Length - 1;
			}
			return paramValueIntArrayByName[upCount];
		}

		public static AchievementRuneData GetAchievementRuneData(GameClient client)
		{
			AchievementRuneData result;
			if (!GlobalNew.IsGongNengOpened(client, 53, false))
			{
				result = null;
			}
			else
			{
				AchievementRuneData achievementRuneData = client.ClientData.achievementRuneData;
				if (achievementRuneData == null)
				{
					achievementRuneData = new AchievementRuneData();
					List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "AchievementRune");
					AchievementRuneBasicData achievementRuneBasicDataByID;
					if (roleParamsIntListFromDB == null || roleParamsIntListFromDB.Count <= 0)
					{
						achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(1);
						achievementRuneData.RoleID = client.ClientData.RoleID;
						achievementRuneData.RuneID = achievementRuneBasicDataByID.RuneID;
						ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
					}
					else
					{
						achievementRuneData.RoleID = client.ClientData.RoleID;
						achievementRuneData.RuneID = roleParamsIntListFromDB[0];
						achievementRuneData.LifeAdd = roleParamsIntListFromDB[1];
						achievementRuneData.AttackAdd = roleParamsIntListFromDB[2];
						achievementRuneData.DefenseAdd = roleParamsIntListFromDB[3];
						achievementRuneData.DodgeAdd = roleParamsIntListFromDB[4];
						if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
						{
							achievementRuneData.UpResultType = 3;
							achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(ChengJiuManager._achievementRuneBasicList.Count);
						}
						else
						{
							achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(achievementRuneData.RuneID);
						}
					}
					achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, ChengJiuManager.GetAchievementRuneUpCount(client));
					achievementRuneData.Achievement = achievementRuneBasicDataByID.AchievementCost;
					client.ClientData.achievementRuneData = achievementRuneData;
				}
				achievementRuneData.AchievementLeft = client.ClientData.ChengJiuPoints;
				if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
				{
					achievementRuneData.UpResultType = 3;
				}
				result = achievementRuneData;
			}
			return result;
		}

		public static void ModifyAchievementRuneData(GameClient client, AchievementRuneData data, bool writeToDB = false)
		{
			List<int> list = new List<int>();
			list.AddRange(new int[]
			{
				data.RuneID,
				data.LifeAdd,
				data.AttackAdd,
				data.DefenseAdd,
				data.DodgeAdd
			});
			Global.SaveRoleParamsIntListToDB(client, list, "AchievementRune", writeToDB);
		}

		public static AchievementRuneData UpAchievementRune(GameClient client, int runeID)
		{
			AchievementRuneData achievementRuneData = client.ClientData.achievementRuneData;
			AchievementRuneData result;
			if (achievementRuneData != null && achievementRuneData.UpResultType == 3)
			{
				achievementRuneData.UpResultType = -4;
				result = achievementRuneData;
			}
			else if (achievementRuneData == null || achievementRuneData.RuneID != runeID)
			{
				achievementRuneData.UpResultType = 0;
				result = achievementRuneData;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 53, false))
			{
				achievementRuneData.UpResultType = -1;
				result = achievementRuneData;
			}
			else
			{
				int[] array = null;
				AchievementRuneBasicData achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(runeID);
				int chengJiuPoints = client.ClientData.ChengJiuPoints;
				if (achievementRuneBasicDataByID.AchievementCost > chengJiuPoints)
				{
					achievementRuneData.UpResultType = -2;
					result = achievementRuneData;
				}
				else
				{
					string text = "";
					int userMoney = client.ClientData.UserMoney;
					int chengJiuPointsValue = GameManager.ClientMgr.GetChengJiuPointsValue(client);
					int achievementRuneUpCount = ChengJiuManager.GetAchievementRuneUpCount(client);
					int achievementRuneDiamond = ChengJiuManager.GetAchievementRuneDiamond(client, achievementRuneUpCount);
					if (achievementRuneDiamond > 0 && !GameManager.ClientMgr.SubUserMoney(client, achievementRuneDiamond, "成就符文提升", true, true, true, true, DaiBiSySType.ChengJieFuWen))
					{
						achievementRuneData.UpResultType = -3;
						result = achievementRuneData;
					}
					else
					{
						text = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
						{
							-achievementRuneDiamond,
							userMoney,
							client.ClientData.UserMoney
						});
						try
						{
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -achievementRuneBasicDataByID.AchievementCost, "成就符文提升", false, true);
						}
						catch (Exception)
						{
							achievementRuneData.UpResultType = -2;
							return achievementRuneData;
						}
						text += EventLogManager.AddResPropString(ResLogType.ChengJiu, new object[]
						{
							-achievementRuneBasicDataByID.AchievementCost,
							chengJiuPointsValue,
							GameManager.ClientMgr.GetChengJiuPointsValue(client)
						});
						int num = 0;
						int randomNumber = Global.GetRandomNumber(0, 100);
						for (int i = 0; i < achievementRuneBasicDataByID.RateList.Count; i++)
						{
							num += achievementRuneBasicDataByID.RateList[i];
							if (randomNumber <= num)
							{
								array = achievementRuneBasicDataByID.AddNumList[i];
								achievementRuneData.BurstType = i;
								break;
							}
						}
						achievementRuneData.LifeAdd += array[0] * ChengJiuManager._runeRate;
						achievementRuneData.LifeAdd = ((achievementRuneData.LifeAdd > achievementRuneBasicDataByID.LifeMax) ? achievementRuneBasicDataByID.LifeMax : achievementRuneData.LifeAdd);
						achievementRuneData.AttackAdd += array[1] * ChengJiuManager._runeRate;
						achievementRuneData.AttackAdd = ((achievementRuneData.AttackAdd > achievementRuneBasicDataByID.AttackMax) ? achievementRuneBasicDataByID.AttackMax : achievementRuneData.AttackAdd);
						achievementRuneData.DefenseAdd += array[2] * ChengJiuManager._runeRate;
						achievementRuneData.DefenseAdd = ((achievementRuneData.DefenseAdd > achievementRuneBasicDataByID.DefenseMax) ? achievementRuneBasicDataByID.DefenseMax : achievementRuneData.DefenseAdd);
						achievementRuneData.DodgeAdd += array[3] * ChengJiuManager._runeRate;
						achievementRuneData.DodgeAdd = ((achievementRuneData.DodgeAdd > achievementRuneBasicDataByID.DodgeMax) ? achievementRuneBasicDataByID.DodgeMax : achievementRuneData.DodgeAdd);
						if (achievementRuneData.LifeAdd < achievementRuneBasicDataByID.LifeMax || achievementRuneData.DefenseAdd < achievementRuneBasicDataByID.DefenseMax || achievementRuneData.AttackAdd < achievementRuneBasicDataByID.AttackMax || achievementRuneData.DodgeAdd < achievementRuneBasicDataByID.DodgeMax)
						{
							achievementRuneData.UpResultType = 1;
							achievementRuneData.Achievement = achievementRuneBasicDataByID.AchievementCost;
							achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, achievementRuneUpCount + 1);
						}
						else
						{
							achievementRuneData.RuneID++;
							achievementRuneData.LifeAdd = 0;
							achievementRuneData.AttackAdd = 0;
							achievementRuneData.DefenseAdd = 0;
							achievementRuneData.DodgeAdd = 0;
							achievementRuneData.UpResultType = 2;
							if (achievementRuneData.RuneID > ChengJiuManager._achievementRuneBasicList.Count)
							{
								achievementRuneData.UpResultType = 3;
								achievementRuneData.Achievement = 0;
								achievementRuneData.Diamond = 0;
							}
							else
							{
								achievementRuneBasicDataByID = ChengJiuManager.GetAchievementRuneBasicDataByID(achievementRuneData.RuneID);
								achievementRuneData.Achievement = achievementRuneBasicDataByID.AchievementCost;
								achievementRuneData.Diamond = ChengJiuManager.GetAchievementRuneDiamond(client, achievementRuneUpCount + 1);
							}
						}
						ChengJiuManager.ModifyAchievementRuneUpCount(client, achievementRuneUpCount + 1, true);
						ChengJiuManager.ModifyAchievementRuneData(client, achievementRuneData, true);
						client.ClientData.achievementRuneData = achievementRuneData;
						ChengJiuManager.SetAchievementRuneProps(client, achievementRuneData);
						EventLogManager.AddAchievementRuneEvent(client, achievementRuneData.RuneID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							achievementRuneData.LifeAdd,
							achievementRuneData.AttackAdd,
							achievementRuneData.DefenseAdd,
							achievementRuneData.DodgeAdd
						}), text);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						achievementRuneData.AchievementLeft = client.ClientData.ChengJiuPoints;
						result = achievementRuneData;
					}
				}
			}
			return result;
		}

		public static void SetAchievementRuneProps(GameClient client, AchievementRuneData achievementRuneData)
		{
			int num = achievementRuneData.LifeAdd;
			int num2 = achievementRuneData.AttackAdd;
			int num3 = achievementRuneData.DefenseAdd;
			int num4 = achievementRuneData.DodgeAdd;
			foreach (AchievementRuneBasicData achievementRuneBasicData in ChengJiuManager._achievementRuneBasicList.Values)
			{
				if (achievementRuneBasicData.RuneID < achievementRuneData.RuneID)
				{
					num += achievementRuneBasicData.LifeMax;
					num2 += achievementRuneBasicData.AttackMax;
					num3 += achievementRuneBasicData.DefenseMax;
					num4 += achievementRuneBasicData.DodgeMax;
				}
			}
			double num5 = 0.0;
			double num6 = 0.0;
			if (achievementRuneData.RuneID > 1)
			{
				AchievementRuneSpecialData achievementRuneSpecialDataByID = ChengJiuManager.GetAchievementRuneSpecialDataByID(achievementRuneData.RuneID - 1);
				num5 += achievementRuneSpecialDataByID.ZhuoYue;
				num6 += achievementRuneSpecialDataByID.DiKang;
			}
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				13,
				num
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				45,
				num2
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				46,
				num3
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				19,
				num4
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				35,
				num5
			});
			client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
			{
				4,
				52,
				num6
			});
		}

		public static void InitRoleChengJiuData(GameClient client)
		{
			client.ClientData.ContinuousDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin);
			client.ClientData.TotalDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
			client.ClientData.ChengJiuPoints = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ChengJiuPoints);
			client.ClientData.TotalKilledMonsterNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledMonsterNum);
			client.ClientData.ChengJiuLevel = ChengJiuManager.GetChengJiuLevel(client);
			if (client.ClientData.ChengJiuLevel > 0)
			{
				int chengJiuLevel = client.ClientData.ChengJiuLevel;
				Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
				{
					(double)chengJiuLevel - 1.0
				}, 0, true);
			}
			client._IconStateMgr.CheckChengJiuUpLevelState(client);
		}

		public static void SaveRoleChengJiuData(GameClient client)
		{
		}

		public static void InitFlagIndex()
		{
			ChengJiuManager._DictFlagIndex.Clear();
			int num = 0;
			for (int i = 100; i <= 108; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 200; i <= 204; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 300; i <= 304; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 350; i <= 356; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 400; i <= 405; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 500; i <= 508; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 600; i <= 608; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 700; i <= 708; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 800; i <= 803; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 900; i <= 905; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 1000; i <= 1005; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 1100; i <= 1105; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 1200; i <= 1210; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 1300; i <= 1308; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 1400; i <= 1411; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 2000; i <= 2004; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = 2050; i <= ChengJiuTypes.JunXianChengJiuEnd; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
			for (int i = ChengJiuTypes.MainLineTaskStart; i <= ChengJiuTypes.MainLineTaskEnd; i++)
			{
				ChengJiuManager._DictFlagIndex.Add(i, num);
				num += 2;
			}
		}

		protected static ushort GetChengJiuIDByIndex(int index)
		{
			for (int i = 0; i < ChengJiuManager._DictFlagIndex.Count; i++)
			{
				if (ChengJiuManager._DictFlagIndex.ElementAt(i).Value == index)
				{
					return (ushort)ChengJiuManager._DictFlagIndex.ElementAt(i).Key;
				}
			}
			return 0;
		}

		protected static int GetCompletedFlagIndex(int chengJiuID)
		{
			int num = -1;
			int result;
			if (ChengJiuManager._DictFlagIndex.TryGetValue(chengJiuID, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		protected static int GetAwardFlagIndex(int chengJiuID)
		{
			int num = -1;
			int result;
			if (ChengJiuManager._DictFlagIndex.TryGetValue(chengJiuID, out num))
			{
				result = num + 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public static void AddChengJiuPoints(GameClient client, string strFrom, int modifyValue = 1, bool forceUpdateBuffer = true, bool writeToDB = false)
		{
			GameManager.ClientMgr.ModifyChengJiuPointsValue(client, modifyValue, strFrom, writeToDB, true);
		}

		public static void SaveKilledMonsterNumToDB(GameClient client, bool bWriteDB = false)
		{
			ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.TotalKilledMonsterNum, ChengJiuExtraDataField.TotalKilledMonsterNum, bWriteDB);
		}

		public static uint GetChengJiuExtraDataByField(GameClient client, ChengJiuExtraDataField field)
		{
			List<uint> roleParamsUIntListFromDB = Global.GetRoleParamsUIntListFromDB(client, "ChengJiuData");
			uint result;
			if (field >= (ChengJiuExtraDataField)roleParamsUIntListFromDB.Count)
			{
				result = 0U;
			}
			else
			{
				result = roleParamsUIntListFromDB[(int)field];
			}
			return result;
		}

		public static void ModifyChengJiuExtraData(GameClient client, uint value, ChengJiuExtraDataField field, bool writeToDB = false)
		{
			List<uint> roleParamsUIntListFromDB = Global.GetRoleParamsUIntListFromDB(client, "ChengJiuData");
			while (roleParamsUIntListFromDB.Count < (int)(field + 1))
			{
				roleParamsUIntListFromDB.Add(0U);
			}
			roleParamsUIntListFromDB[(int)field] = value;
			Global.SaveRoleParamsUintListToDB(client, roleParamsUIntListFromDB, "ChengJiuData", writeToDB);
		}

		public static int GetChengJiuLevel(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "ChengJiuLevel");
		}

		public static void SetChengJiuLevel(GameClient client, int value, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "ChengJiuLevel", value, writeToDB);
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriAchievement) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
			GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ChengJiuLevel));
		}

		public int upGradeChengJiuBuffer(GameClient player)
		{
			return 1;
		}

		public static bool CanActiveNextChengHao(GameClient client)
		{
			return GameManager.ClientMgr.GetChengJiuPointsValue(client) >= ChengJiuManager.GetUpLevelNeedChengJiuPoint(client);
		}

		public static int TryToActiveNewChengJiuBuffer(GameClient client, bool notifyPropsChanged, int nChengJiuLevel = -1)
		{
			double num = 0.0;
			int newChengJiuBufferGoodsIndexIDAndDayXiaoHao = ChengJiuManager.GetNewChengJiuBufferGoodsIndexIDAndDayXiaoHao(client, client.ClientData.ChengJiuPoints, out num);
			if (-1 != nChengJiuLevel)
			{
				if (client.ClientData.ChengJiuLevel + 1 < nChengJiuLevel)
				{
					return -2;
				}
			}
			int upLevelNeedChengJiuPoint = ChengJiuManager.GetUpLevelNeedChengJiuPoint(client);
			int result;
			if (GameManager.ClientMgr.GetChengJiuPointsValue(client) < upLevelNeedChengJiuPoint)
			{
				result = -5;
			}
			else
			{
				int num2 = client.ClientData.ChengJiuLevel + 1;
				if (num2 > newChengJiuBufferGoodsIndexIDAndDayXiaoHao)
				{
					result = -1;
				}
				else
				{
					string stringValue = GameManager.systemChengJiuBuffer.SystemXmlItemDict[num2].GetStringValue("NeedGoods");
					List<List<int>> list = ConfigHelper.ParserIntArrayList(stringValue, true, '|', ',');
					for (int i = 0; i < list.Count; i++)
					{
						int num3 = list[i][0];
						int num4 = list[i][1];
						int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, num3);
						if (totalGoodsCountByID < num4)
						{
							return -6;
						}
					}
					int num5 = -1;
					BufferData bufferDataByID = Global.GetBufferDataByID(client, 31);
					if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
					{
						num5 = (int)bufferDataByID.BufferVal;
					}
					if (num5 == num2 && 0 != client.ClientData.ChengJiuLevel)
					{
						result = -3;
					}
					else
					{
						if (num5 >= 0)
						{
							if (num2 < num5)
							{
								return -4;
							}
						}
						if (num2 >= 0)
						{
							Global.UpdateBufferData(client, BufferItemTypes.ChengJiu, new double[]
							{
								(double)num2 - 1.0
							}, 0, true);
							bool flag = false;
							bool flag2 = false;
							for (int i = 0; i < list.Count; i++)
							{
								int num3 = list[i][0];
								int num4 = list[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, num4, false, out flag, out flag2, false))
								{
									LogManager.WriteLog(2, string.Format("提升成就等级时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", num3, num4), null, true);
								}
								GoodsData goodsData = new GoodsData();
								goodsData.GoodsID = num3;
								goodsData.GCount = num4;
							}
							GameManager.ClientMgr.ModifyChengJiuPointsValue(client, -upLevelNeedChengJiuPoint, "提升成就等级", false, true);
							GameManager.ClientMgr.SetChengJiuLevelValue(client, 1, "提升成就等级", true, true);
							if (client.ClientData.ChengJiuLevel >= 4)
							{
								Global.BroadcastClientChuanQiChengJiu(client, num2);
							}
						}
						if (notifyPropsChanged)
						{
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
						if (client._IconStateMgr.CheckChengJiuUpLevelState(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public static int GetNewChengJiuBufferGoodsIndexIDAndDayXiaoHao(GameClient client, int chengJiuPoints, out double dayXiaoHao)
		{
			int num = -1;
			dayXiaoHao = 0.0;
			for (int i = 0; i < GameManager.systemChengJiuBuffer.SystemXmlItemDict.Count; i++)
			{
				SystemXmlItem value = GameManager.systemChengJiuBuffer.SystemXmlItemDict.ElementAt(i).Value;
				int intValue = value.GetIntValue("ChengJiu", -1);
				if (chengJiuPoints >= intValue)
				{
					num = value.GetIntValue("ID", -1);
					dayXiaoHao = value.GetDoubleValue("DayXiaoHao");
				}
			}
			if (num < 0)
			{
				num = -1;
			}
			return num;
		}

		public static int GetUpLevelNeedChengJiuPoint(GameClient client)
		{
			SystemXmlItem systemXmlItem;
			int result;
			if (GameManager.systemChengJiuBuffer.SystemXmlItemDict.TryGetValue(client.ClientData.ChengJiuLevel + 1, out systemXmlItem))
			{
				result = systemXmlItem.GetIntValue("ChengJiu", -1);
			}
			else
			{
				result = int.MaxValue;
			}
			return result;
		}

		public static bool IsChengJiuCompleted(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, false);
		}

		public static bool IsChengJiuAwardFetched(GameClient client, int chengJiuID)
		{
			return ChengJiuManager.IsFlagIsTrue(client, chengJiuID, true);
		}

		public static void OnChengJiuCompleted(GameClient client, int chengJiuID)
		{
			ChengJiuManager.UpdateChengJiuFlag(client, chengJiuID, false);
			ChengJiuManager.GiveChengJiuAward(client, chengJiuID, "完成成就ID：" + chengJiuID);
			ChengJiuManager.NotifyClientChengJiuData(client, chengJiuID);
			GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteChengJiu));
			ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, -1, -1, TaskTypes.ChengJiuUpdate, null, chengJiuID, -1L, null);
		}

		public static void NotifyClientChengJiuData(GameClient client, int justCompletedChengJiu = -1)
		{
			ChengJiuData instance = new ChengJiuData
			{
				RoleID = client.ClientData.RoleID,
				ChengJiuPoints = (long)client.ClientData.ChengJiuPoints,
				TotalKilledMonsterNum = (long)((ulong)client.ClientData.TotalKilledMonsterNum),
				TotalLoginNum = (long)((ulong)client.ClientData.TotalDayLoginNum),
				ContinueLoginNum = (int)client.ClientData.ContinuousDayLoginNum,
				ChengJiuFlags = ChengJiuManager.GetChengJiuInfoArray(client),
				NowCompletedChengJiu = justCompletedChengJiu,
				TotalKilledBossNum = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledBossNum)),
				CompleteNormalCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum)),
				CompleteHardCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum)),
				CompleteDifficltCopyMapCount = (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum)),
				GuildChengJiu = (long)client.ClientData.BangGong,
				JunXianChengJiu = (long)GameManager.ClientMgr.GetShengWangLevelValue(client)
			};
			byte[] buffer = DataHelper.ObjectToBytes<ChengJiuData>(instance);
			GameManager.ClientMgr.SendToClient(client, buffer, 420);
		}

		protected static List<ushort> GetChengJiuInfoArray(GameClient client)
		{
			List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
			int num = 0;
			List<ushort> list = new List<ushort>();
			for (int i = 0; i < roleParamsUlongListFromDB.Count; i++)
			{
				ulong num2 = roleParamsUlongListFromDB[i];
				for (int j = 0; j < 64; j += 2)
				{
					ulong num3 = 3UL << j;
					ushort num4 = (ushort)((num2 & num3) >> j);
					ushort chengJiuIDByIndex = ChengJiuManager.GetChengJiuIDByIndex(num);
					ushort num5 = (ushort)(chengJiuIDByIndex << 2);
					ushort item = num5 | num4;
					list.Add(item);
					num += 2;
				}
			}
			return list;
		}

		public static int GiveChengJiuAward(GameClient client, int chengJiuID, string strFrom)
		{
			int result;
			if (!ChengJiuManager.IsChengJiuCompleted(client, chengJiuID))
			{
				result = -1;
			}
			else if (ChengJiuManager.IsChengJiuAwardFetched(client, chengJiuID))
			{
				result = -2;
			}
			else
			{
				ChengJiuManager.UpdateChengJiuFlag(client, chengJiuID, true);
				string text = "";
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				SystemXmlItem systemXmlItem = null;
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(chengJiuID, out systemXmlItem))
				{
					num = Math.Max(0, systemXmlItem.GetIntValue("BindZuanShi", -1));
					num2 = Math.Max(0, systemXmlItem.GetIntValue("BindMoney", -1));
					num3 = Math.Max(0, systemXmlItem.GetIntValue("ChengJiu", -1));
				}
				if (num > 0)
				{
					int gold = client.ClientData.Gold;
					GameManager.ClientMgr.AddUserGold(client, num, strFrom);
					text += EventLogManager.AddResPropString(ResLogType.BindZuanShi, new object[]
					{
						num,
						gold,
						client.ClientData.Gold
					});
				}
				if (num2 > 0)
				{
					num2 = Global.FilterValue(client, num2);
					long num4 = (long)client.ClientData.Money1;
					GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, "完成成就：" + chengJiuID, false);
					text += EventLogManager.AddResPropString(ResLogType.BindJinbi, new object[]
					{
						num2,
						num4,
						client.ClientData.Money1
					});
				}
				if (num3 > 0)
				{
					int chengJiuPointsValue = GameManager.ClientMgr.GetChengJiuPointsValue(client);
					ChengJiuManager.AddChengJiuPoints(client, strFrom, num3, true, true);
					text += EventLogManager.AddResPropString(ResLogType.ChengJiu, new object[]
					{
						num3,
						chengJiuPointsValue,
						GameManager.ClientMgr.GetChengJiuPointsValue(client)
					});
				}
				if (text.Length > 0)
				{
					text = text.Remove(0, 1);
				}
				EventLogManager.AddChengJiuAwardEvent(client, chengJiuID, text);
				result = 0;
			}
			return result;
		}

		public static long getChengJiuValue(GameClient client, AchievementType type)
		{
			if (type > AchievementType.Boss)
			{
				if (type <= AchievementType.EquipForge)
				{
					if (type <= AchievementType.CopyHard)
					{
						if (type == AchievementType.CopyNormal)
						{
							return (long)((ulong)(ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum) + ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum) + ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum)));
						}
						if (type != AchievementType.CopyHard)
						{
							goto IL_187;
						}
						return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum));
					}
					else
					{
						if (type == AchievementType.CopyDifficlt)
						{
							return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum));
						}
						if (type != AchievementType.EquipForge)
						{
							goto IL_187;
						}
					}
				}
				else if (type <= AchievementType.Merge)
				{
					if (type != AchievementType.EquipZhuLing && type != AchievementType.Merge)
					{
						goto IL_187;
					}
				}
				else
				{
					if (type == AchievementType.ShengWangLevel)
					{
						return (long)GameManager.ClientMgr.GetShengWangLevelValue(client);
					}
					if (type != AchievementType.MainLineTask)
					{
						goto IL_187;
					}
					return (long)client.ClientData.MainTaskID;
				}
				return 0L;
			}
			if (type <= AchievementType.LoginContinue)
			{
				if (type == AchievementType.PlayerLevel)
				{
					return (long)Global.GetUnionLevel(client, false);
				}
				if (type == AchievementType.ShenGe)
				{
					return (long)client.ClientData.ChangeLifeCount;
				}
				if (type == AchievementType.LoginContinue)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin));
				}
			}
			else if (type <= AchievementType.BindGoldCoin)
			{
				if (type == AchievementType.LoginTotal)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin));
				}
				if (type == AchievementType.BindGoldCoin)
				{
					return (long)client.ClientData.Money1;
				}
			}
			else
			{
				if (type == AchievementType.Monster)
				{
					return (long)((ulong)client.ClientData.TotalKilledMonsterNum);
				}
				if (type == AchievementType.Boss)
				{
					return (long)((ulong)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalKilledBossNum));
				}
			}
			IL_187:
			return 0L;
		}

		public static bool IsFlagIsTrue(GameClient client, int chengJiuID, bool forAward = false)
		{
			int num = ChengJiuManager.GetCompletedFlagIndex(chengJiuID);
			bool result;
			if (num < 0)
			{
				result = false;
			}
			else
			{
				if (forAward)
				{
					num++;
				}
				List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
				if (roleParamsUlongListFromDB.Count <= 0)
				{
					result = false;
				}
				else
				{
					int num2 = num / 64;
					if (num2 >= roleParamsUlongListFromDB.Count)
					{
						result = false;
					}
					else
					{
						int num3 = num % 64;
						ulong num4 = roleParamsUlongListFromDB[num2];
						ulong num5 = 1UL << num3;
						bool flag = (num4 & num5) > 0UL;
						result = flag;
					}
				}
			}
			return result;
		}

		public static bool UpdateChengJiuFlag(GameClient client, int chengJiuID, bool forAward = false)
		{
			int num = ChengJiuManager.GetCompletedFlagIndex(chengJiuID);
			bool result;
			if (num < 0)
			{
				result = false;
			}
			else
			{
				if (forAward)
				{
					num++;
				}
				List<ulong> roleParamsUlongListFromDB = Global.GetRoleParamsUlongListFromDB(client, "ChengJiuFlags");
				int i = num / 64;
				while (i > roleParamsUlongListFromDB.Count - 1)
				{
					roleParamsUlongListFromDB.Add(0UL);
				}
				int num2 = num % 64;
				ulong num3 = roleParamsUlongListFromDB[i];
				ulong num4 = 1UL << num2;
				roleParamsUlongListFromDB[i] = (num3 | num4);
				Global.SaveRoleParamsUlongListToDB(client, roleParamsUlongListFromDB, "ChengJiuFlags", true);
				result = true;
			}
			return result;
		}

		public static void OnFirstKillMonster(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 100))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 100);
			}
		}

		public static void OnFirstAddFriend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 101))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 101);
			}
		}

		public static void OnFirstInFaction(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 103))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 103);
			}
		}

		public static void OnFirstInTeam(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 102))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 102);
			}
		}

		public static void OnFirstHeCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 104))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 104);
			}
		}

		public static void OnFirstQiangHua(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 105))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 105);
			}
		}

		public static void OnFirstAppend(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 106))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 106);
			}
		}

		public static void OnFirstJiCheng(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 107))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 107);
			}
		}

		public static void OnFirstBaiTan(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 108))
			{
				ChengJiuManager.OnChengJiuCompleted(client, 108);
			}
		}

		public static void OnMonsterKilled(GameClient killer, Monster victim)
		{
			if (0U == killer.ClientData.TotalKilledMonsterNum)
			{
				killer.ClientData.TotalKilledMonsterNum = ChengJiuManager.GetChengJiuExtraDataByField(killer, ChengJiuExtraDataField.TotalKilledMonsterNum);
				if (0U == killer.ClientData.TotalKilledMonsterNum)
				{
					ChengJiuManager.OnFirstKillMonster(killer);
				}
			}
			killer.ClientData.TotalKilledMonsterNum += 1U;
			SafeClientData clientData = killer.ClientData;
			clientData.TimerKilledMonsterNum += 1;
			bool flag = false;
			if (killer.ClientData.ChangeLifeCount == 0)
			{
				if (killer.ClientData.TimerKilledMonsterNum > 200)
				{
					flag = true;
				}
			}
			else if (killer.ClientData.TimerKilledMonsterNum > 500)
			{
				flag = true;
			}
			if (flag)
			{
				killer.ClientData.TimerKilledMonsterNum = 0;
				ChengJiuManager.SaveKilledMonsterNumToDB(killer, flag);
			}
			ChengJiuManager.CheckMonsterChengJiu(killer);
			if (401 == victim.MonsterType)
			{
				if (!ChengJiuManager.IsChengJiuCompleted(killer, 803))
				{
					for (int i = 0; i < Data.KillBossCountForChengJiu.Length; i++)
					{
						if (victim.MonsterInfo.ExtensionID == Data.KillBossCountForChengJiu[i])
						{
							int num = (int)ChengJiuManager.GetChengJiuExtraDataByField(killer, ChengJiuExtraDataField.TotalKilledBossNum);
							ChengJiuManager.ModifyChengJiuExtraData(killer, (uint)(++num), ChengJiuExtraDataField.TotalKilledBossNum, true);
							ChengJiuManager.CheckBossChengJiu(killer, num);
						}
					}
				}
			}
		}

		public static void CheckMonsterChengJiu(GameClient client)
		{
			if (client.ClientData.TotalKilledMonsterNum >= client.ClientData.NextKilledMonsterChengJiuNum && 2147483647U != client.ClientData.NextKilledMonsterChengJiuNum)
			{
				uint nextKilledMonsterChengJiuNum = ChengJiuManager.CheckSingleConditionChengJiu(client, 700, 708, (long)((ulong)client.ClientData.TotalKilledMonsterNum), "KillMonster");
				client.ClientData.NextKilledMonsterChengJiuNum = nextKilledMonsterChengJiuNum;
				if (ChengJiuManager.IsChengJiuCompleted(client, 708))
				{
					client.ClientData.NextKilledMonsterChengJiuNum = 2147483647U;
				}
			}
		}

		public static void CheckBossChengJiu(GameClient client, int nNum)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 800, 803, (long)nNum, "KillBoss");
		}

		public static void OnTongQianIncrease(GameClient client)
		{
			if (0 > client.ClientData.MaxTongQianNum)
			{
				client.ClientData.MaxTongQianNum = Math.Max(0, Global.GetRoleParamsInt32FromDB(client, "MaxTongQianNum"));
			}
			if (client.ClientData.YinLiang >= client.ClientData.MaxTongQianNum)
			{
				client.ClientData.MaxTongQianNum = client.ClientData.YinLiang;
				Global.SaveRoleParamsInt32ValueToDB(client, "MaxTongQianNum", client.ClientData.MaxTongQianNum, false);
				if ((long)client.ClientData.MaxTongQianNum >= (long)((ulong)client.ClientData.NextTongQianChengJiuNum))
				{
					client.ClientData.NextTongQianChengJiuNum = ChengJiuManager.CheckSingleConditionChengJiu(client, 600, 608, (long)client.ClientData.MaxTongQianNum, "TongQianLimit");
				}
			}
		}

		public static void OnRoleLevelUp(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 200, 204, (long)client.ClientData.Level, "LevelLimit");
		}

		public static void OnRoleChangeLife(GameClient client)
		{
			ChengJiuManager.CheckSingleConditionChengJiu(client, 300, 304, (long)client.ClientData.ChangeLifeCount, "ZhuanShengLimit");
		}

		public static void OnRoleLogin(GameClient client, int preLoginDay)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear < preLoginDay && Math.Abs(dayOfYear - preLoginDay) < 2)
			{
				LogManager.WriteLog(2, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else if (dayOfYear != preLoginDay)
			{
				client.ClientData.TotalDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.TotalDayLogin);
				client.ClientData.ContinuousDayLoginNum = ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ContinuousDayLogin);
				client.ClientData.TotalDayLoginNum += 1U;
				GameManager.ClientMgr.NotifySelfPropertyValue(client, 137, (long)((ulong)client.ClientData.TotalDayLoginNum));
				int dayOfYear2 = TimeUtil.NowDateTime().AddDays(-1.0).DayOfYear;
				if (dayOfYear2 == preLoginDay)
				{
					client.ClientData.ContinuousDayLoginNum += 1U;
					client.ClientData.SeriesLoginNum++;
				}
				else
				{
					client.ClientData.ContinuousDayLoginNum = 1U;
					client.ClientData.SeriesLoginNum = 1;
				}
				if ("" != client.strUserID)
				{
					GameManager.DBCmdMgr.AddDBCmd(10162, string.Format("{0}", client.strUserID), null, client.ServerId);
				}
				Global.UpdateSeriesLoginInfo(client);
				ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.TotalDayLoginNum, ChengJiuExtraDataField.TotalDayLogin, true);
				ChengJiuManager.ModifyChengJiuExtraData(client, client.ClientData.ContinuousDayLoginNum, ChengJiuExtraDataField.ContinuousDayLogin, true);
				ChengJiuManager.CheckSingleConditionChengJiu(client, 400, 405, (long)((ulong)client.ClientData.ContinuousDayLoginNum), "LoginDayOne");
				ChengJiuManager.CheckSingleConditionChengJiu(client, 500, 508, (long)((ulong)client.ClientData.TotalDayLoginNum), "LoginDayTwo");
				DailyActiveManager.CleanDailyActiveInfo(client);
				if (!client.ClientData.DailyActiveDayLginSetFlag)
				{
					bool flag = false;
					DailyActiveManager.ProcessLoginForDailyActive(client, out flag);
				}
				client.ClientData.DailyActiveDayLginSetFlag = true;
				GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, -Global.GetRoleParamsInt32FromDB(client, "10255"), "首次登录重生槽免费次数重置", true, true, false);
			}
		}

		public static void OnRoleEquipmentQiangHua(GameClient client, int equipStarsNum)
		{
			int num = ChengJiuManager.CheckEquipmentChengJiu(client, 1200, 1210, (long)equipStarsNum, "QiangHuaLimit");
		}

		public static void OnRoleGoodsAppend(GameClient client, int AppendLev)
		{
			int num = ChengJiuManager.CheckEquipmentChengJiu(client, 1300, 1308, (long)AppendLev, "ZhuiJiaLimit");
		}

		public static void OnRoleGoodsHeCheng(GameClient client, int goodsIDCreated)
		{
			int num = ChengJiuManager.CheckEquipmentChengJiu(client, 1400, 1411, (long)goodsIDCreated, "HeChengLimit");
		}

		public static void ProcessCompleteCopyMapForChengJiu(GameClient client, int nCopyMapLev, int count = 1)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 905) || !ChengJiuManager.IsChengJiuCompleted(client, 1005) || !ChengJiuManager.IsChengJiuCompleted(client, 1105))
			{
				if (nCopyMapLev >= 0)
				{
					switch (nCopyMapLev)
					{
					case 1:
					{
						int num = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteNormalCopyMapNum);
						num++;
						num *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)num, ChengJiuExtraDataField.CompleteNormalCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 900, 905, (long)num, "KillRaid");
						break;
					}
					case 2:
					{
						int num = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteHardCopyMapNum);
						num++;
						num *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)num, ChengJiuExtraDataField.CompleteHardCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 1000, 1005, (long)num, "KillRaid");
						break;
					}
					case 3:
					{
						int num = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.CompleteDifficltCopyMapNum);
						num++;
						num *= count;
						ChengJiuManager.ModifyChengJiuExtraData(client, (uint)num, ChengJiuExtraDataField.CompleteDifficltCopyMapNum, true);
						ChengJiuManager.CheckSingleConditionChengJiu(client, 1100, 1105, (long)num, "KillRaid");
						break;
					}
					}
				}
			}
		}

		public static void OnRoleSkillLevelUp(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 356))
			{
				bool flag = false;
				int num = 0;
				for (int i = 0; i < client.ClientData.SkillDataList.Count; i++)
				{
					if (client.ClientData.SkillDataList[i].DbID == -1)
					{
						if (!flag)
						{
							num += client.ClientData.SkillDataList[i].SkillLevel;
							flag = true;
						}
					}
					else
					{
						num += client.ClientData.SkillDataList[i].SkillLevel;
					}
				}
				ChengJiuManager.CheckSingleConditionChengJiu(client, 350, 356, (long)num, "SkillLevel");
			}
		}

		public static void OnRoleGuildChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, 2004))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2000, 2004, (long)client.ClientData.BangGong, "ZhanGong");
			}
		}

		public static void OnRoleJunXianChengJiu(GameClient client)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, ChengJiuTypes.JunXianChengJiuEnd))
			{
				ChengJiuManager.CheckSingleConditionChengJiu(client, 2050, ChengJiuTypes.JunXianChengJiuEnd, (long)GameManager.ClientMgr.GetShengWangLevelValue(client), "JunXian");
			}
		}

		public static void ProcessCompleteMainTaskForChengJiu(GameClient client, int nTaskID)
		{
			if (!ChengJiuManager.IsChengJiuCompleted(client, ChengJiuTypes.MainLineTaskEnd))
			{
				if (nTaskID >= 0)
				{
					SystemXmlItem systemXmlItem = null;
					int i = ChengJiuTypes.MainLineTaskStart;
					while (i <= ChengJiuTypes.MainLineTaskEnd)
					{
						if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
						{
							if (null != systemXmlItem)
							{
								uint intValue = (uint)systemXmlItem.GetIntValue("RenWu", -1);
								if ((long)nTaskID >= (long)((ulong)intValue))
								{
									if (!ChengJiuManager.IsChengJiuCompleted(client, i))
									{
										ChengJiuManager.OnChengJiuCompleted(client, i);
									}
								}
							}
						}
						IL_89:
						i++;
						continue;
						goto IL_89;
					}
				}
			}
		}

		protected static uint CheckSingleConditionChengJiu(GameClient client, int chengJiuMinID, int chengJiuMaxID, long roleCurrentValue, string strCheckField)
		{
			SystemXmlItem systemXmlItem = null;
			uint num = 0U;
			int i = chengJiuMinID;
			while (i <= chengJiuMaxID)
			{
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
				{
					if (null != systemXmlItem)
					{
						num = (uint)systemXmlItem.GetIntValue(strCheckField, -1);
						if (roleCurrentValue < (long)((ulong)num))
						{
							break;
						}
						if (!ChengJiuManager.IsChengJiuCompleted(client, i))
						{
							ChengJiuManager.OnChengJiuCompleted(client, i);
						}
					}
				}
				IL_6A:
				i++;
				continue;
				goto IL_6A;
			}
			return num;
		}

		protected static int CheckEquipmentChengJiu(GameClient client, int chengJiuMinID, int chengJiuMaxID, long roleCurrentValue, string strCheckField)
		{
			SystemXmlItem systemXmlItem = null;
			int result = -1;
			int i = chengJiuMinID;
			while (i <= chengJiuMaxID)
			{
				if (GameManager.systemChengJiu.SystemXmlItemDict.TryGetValue(i, out systemXmlItem))
				{
					if (null != systemXmlItem)
					{
						string[] array = systemXmlItem.GetStringValue(strCheckField).Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							int num = Global.SafeConvertToInt32(array[0]);
							if (roleCurrentValue == (long)num)
							{
								int num2 = Global.SafeConvertToInt32(array[1]);
								if (num2 <= 1)
								{
									if (!ChengJiuManager.IsChengJiuCompleted(client, i))
									{
										ChengJiuManager.OnChengJiuCompleted(client, i);
										result = i;
									}
								}
							}
						}
					}
				}
				IL_C2:
				i++;
				continue;
				goto IL_C2;
			}
			return result;
		}

		public const string EncodingLatin1 = "latin1";

		private static Dictionary<int, int> _DictFlagIndex = new Dictionary<int, int>();

		private static Dictionary<int, AchievementRuneBasicData> _achievementRuneBasicList = new Dictionary<int, AchievementRuneBasicData>();

		private static Dictionary<int, AchievementRuneSpecialData> _achievementRuneSpecialList = new Dictionary<int, AchievementRuneSpecialData>();

		private static int _runeRate = 1;

		private static ChengJiuManager Instance = new ChengJiuManager();

		private enum AchievementRuneResultType
		{
			End = 3,
			Next = 2,
			Success = 1,
			Efail = 0,
			EnoOpen = -1,
			EnoAchievement = -2,
			EnoDiamond = -3,
			EOver = -4
		}
	}
}
