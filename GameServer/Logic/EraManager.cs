using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class EraManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static EraManager getInstance()
		{
			return EraManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1090, 1, 1, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1091, 2, 2, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1092, 2, 2, EraManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, 88, false) || !GlobalNew.IsGongNengOpened(client, 120400, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1090:
					result = this.ProcessGetEraDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1091:
					result = this.ProcessEraDonateCmd(client, nID, bytes, cmdParams);
					break;
				case 1092:
					result = this.ProcessEraAwardCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public void OnLogin(GameClient client)
		{
			if (JunTuanClient.getInstance().GetCurrentEraID() <= 0)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					13,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					13,
					1,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public bool InitConfig()
		{
			return this.LoadEraUIConfigFile() && this.LoadEraTaskConfigFile() && this.LoadEraRewardConfigFile() && this.LoadEraDropConfigFile() && this.LoadEraContributionConfigFile();
		}

		public bool LoadEraUIConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraUI.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraUI.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, EraUIConfig> dictionary = new Dictionary<int, EraUIConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					EraUIConfig eraUIConfig = new EraUIConfig();
					eraUIConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					eraUIConfig.EraID = (int)Global.GetSafeAttributeLong(xml, "EraID");
					DateTime.TryParse(Global.GetSafeAttributeStr(xml, "StartTime"), out eraUIConfig.StartTime);
					DateTime.TryParse(Global.GetSafeAttributeStr(xml, "EndTime"), out eraUIConfig.EndTime);
					dictionary[eraUIConfig.ID] = eraUIConfig;
				}
				lock (this.ConfigMutex)
				{
					this.EraUIConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EraUI.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEraTaskConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraTask.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraTask.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> dictionary = new Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					EraTaskConfig eraTaskConfig = new EraTaskConfig();
					eraTaskConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					eraTaskConfig.EraID = (int)Global.GetSafeAttributeLong(xml, "EraID");
					eraTaskConfig.EraStage = (int)Global.GetSafeAttributeLong(xml, "EraStage");
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(eraTaskConfig.EraID, eraTaskConfig.EraStage);
					List<EraTaskConfig> list = null;
					if (!dictionary.TryGetValue(key, out list))
					{
						list = new List<EraTaskConfig>();
						dictionary[key] = list;
					}
					eraTaskConfig.Reward = (int)Global.GetSafeAttributeLong(xml, "Reward");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "CompletionCondition");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						if (array3.Length == 2)
						{
							int key2 = Convert.ToInt32(array3[0]);
							int value = Convert.ToInt32(array3[1]);
							eraTaskConfig.CompletionCondition.Add(new KeyValuePair<int, int>(key2, value));
						}
					}
					list.Add(eraTaskConfig);
				}
				lock (this.ConfigMutex)
				{
					this.EraTaskConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EraTask.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEraRewardConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraReward.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraReward.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, EraAwardConfig> dictionary = new Dictionary<int, EraAwardConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					EraAwardConfig eraAwardConfig = new EraAwardConfig();
					eraAwardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					eraAwardConfig.EraID = (int)Global.GetSafeAttributeLong(xml, "EraID");
					eraAwardConfig.EraName = Global.GetSafeAttributeStr(xml, "Name");
					eraAwardConfig.AwardType = (int)Global.GetSafeAttributeLong(xml, "Type");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "StartTime");
					if (!string.IsNullOrEmpty(safeAttributeStr))
					{
						DateTime.TryParse(safeAttributeStr, out eraAwardConfig.StartTime);
					}
					string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "EndTime");
					if (!string.IsNullOrEmpty(safeAttributeStr2))
					{
						DateTime.TryParse(safeAttributeStr2, out eraAwardConfig.EndTime);
					}
					eraAwardConfig.EraRanking = (int)Global.GetSafeAttributeLong(xml, "EraRanking");
					eraAwardConfig.Progress = (int)Global.GetSafeAttributeLong(xml, "Progress");
					eraAwardConfig.Contribution = (int)Global.GetSafeAttributeLong(xml, "Contribution");
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "LeaderReward"), ref eraAwardConfig.LeaderReward, '|', ',');
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "MasterReward"), ref eraAwardConfig.MasterReward, '|', ',');
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "Reward"), ref eraAwardConfig.Reward, '|', ',');
					dictionary[eraAwardConfig.ID] = eraAwardConfig;
				}
				lock (this.ConfigMutex)
				{
					this.EraAwardConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EraReward.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEraDropConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraDrop.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraDrop.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, List<EraDropConfig>> dictionary = new Dictionary<int, List<EraDropConfig>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int num = (int)Global.GetSafeAttributeLong(xml, "ID");
					List<EraDropConfig> list = null;
					if (!dictionary.TryGetValue(num, out list))
					{
						list = new List<EraDropConfig>();
						dictionary[num] = list;
					}
					EraDropConfig eraDropConfig = new EraDropConfig();
					eraDropConfig.ID = num;
					eraDropConfig.EraID = (int)Global.GetSafeAttributeLong(xml, "EraID");
					eraDropConfig.EraStage = (int)Global.GetSafeAttributeLong(xml, "EraStage");
					eraDropConfig.Fixedaward = Global.GetSafeAttributeStr(xml, "Fixedaward");
					eraDropConfig.MaxList = (int)Global.GetSafeAttributeLong(xml, "MaxList");
					int num2 = 0;
					FallGoodsItem fallGoodsItem = null;
					eraDropConfig.fallGoodsItemList = new List<FallGoodsItem>();
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i].Trim();
						if (!(text == ""))
						{
							string[] array2 = text.Split(new char[]
							{
								','
							});
							if (array2.Length == 7)
							{
								fallGoodsItem = null;
								try
								{
									fallGoodsItem = new FallGoodsItem
									{
										GoodsID = Convert.ToInt32(array2[0]),
										BasePercent = num2,
										SelfPercent = (int)(Convert.ToDouble(array2[1]) * 100000.0),
										Binding = Convert.ToInt32(array2[2]),
										LuckyRate = (int)Convert.ToDouble(array2[3]),
										FallLevelID = Convert.ToInt32(array2[4]),
										ZhuiJiaID = Convert.ToInt32(array2[5]),
										ExcellencePropertyID = Convert.ToInt32(array2[6])
									};
									num2 += fallGoodsItem.SelfPercent;
								}
								catch (Exception)
								{
									fallGoodsItem = null;
								}
								if (null == fallGoodsItem)
								{
									LogManager.WriteLog(2, string.Format("解析纪元掉落项时发生错误, GoodsPackID={0}, GoodsID={1}", eraDropConfig.ID, text), null, true);
								}
								else
								{
									eraDropConfig.fallGoodsItemList.Add(fallGoodsItem);
								}
							}
						}
					}
					list.Add(eraDropConfig);
				}
				lock (this.ConfigMutex)
				{
					this.EraDropConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EraDrop.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadEraContributionConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/EraContribution.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/EraContribution.xml"));
				if (null == xelement)
				{
					return false;
				}
				List<EraContributionConfig> list = new List<EraContributionConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					EraContributionConfig eraContributionConfig = new EraContributionConfig();
					eraContributionConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					eraContributionConfig.EraID = (int)Global.GetSafeAttributeLong(xml, "EraID");
					eraContributionConfig.ProgressID = (int)Global.GetSafeAttributeLong(xml, "ProgressID");
					eraContributionConfig.GoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
					eraContributionConfig.Contribution = (int)Global.GetSafeAttributeLong(xml, "Contribution");
					int[] safeAttributeIntArray = Global.GetSafeAttributeIntArray(xml, "MonsterID", -1, ',');
					foreach (int item in safeAttributeIntArray)
					{
						eraContributionConfig.MonsterIDSet.Add(item);
					}
					list.Add(eraContributionConfig);
				}
				lock (this.ConfigMutex)
				{
					this.EraContributionList = list;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/EraContribution.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void EraTimer_Work()
		{
			EraManager.<>c__DisplayClass9 CS$<>8__locals1 = new EraManager.<>c__DisplayClass9();
			if (!GameManager.IsKuaFuServer)
			{
				CS$<>8__locals1.curEraID = JunTuanClient.getInstance().GetCurrentEraID();
				if (CS$<>8__locals1.curEraID > 0)
				{
					int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
					if (offsetDay != this.CheckRankAwardDayID && this.InRankAwardTime())
					{
						List<KFEraRankData> junTuanEraRankData = JunTuanClient.getInstance().GetJunTuanEraRankData(false);
						if (null != junTuanEraRankData)
						{
							int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("era_rank_award", 0);
							if (CS$<>8__locals1.curEraID != gameConfigItemInt)
							{
								List<JunTuanBaseData> junTuanBaseDataList = JunTuanClient.getInstance().GetJunTuanBaseDataList(true);
								using (List<JunTuanBaseData>.Enumerator enumerator = junTuanBaseDataList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										EraManager.<>c__DisplayClassc CS$<>8__locals2 = new EraManager.<>c__DisplayClassc();
										CS$<>8__locals2.CS$<>8__localsa = CS$<>8__locals1;
										CS$<>8__locals2.baseData = enumerator.Current;
										if (CS$<>8__locals2.baseData.BhList != null && CS$<>8__locals2.baseData.BhList.Count != 0)
										{
											KFEraData junTuanEraData = JunTuanClient.getInstance().GetJunTuanEraData(CS$<>8__locals2.baseData.JunTuanId, false);
											if (null == junTuanEraData)
											{
												LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 获取纪元数据失败", CS$<>8__locals1.curEraID, CS$<>8__locals2.baseData.JunTuanId), null, true);
											}
											else
											{
												int RankNum = junTuanEraRankData.FindIndex((KFEraRankData x) => x.JunTuanID == CS$<>8__locals2.baseData.JunTuanId);
												RankNum = ((RankNum != -1) ? (RankNum + 1) : RankNum);
												Dictionary<int, EraAwardConfig> dictionary = null;
												lock (this.ConfigMutex)
												{
													dictionary = this.EraAwardConfigDict;
												}
												List<EraAwardConfig> list = dictionary.Values.ToList<EraAwardConfig>();
												EraAwardConfig eraAwardConfig = list.Find((EraAwardConfig x) => x.AwardType == 2 && x.EraID == CS$<>8__locals1.curEraID && x.EraRanking == RankNum);
												if (null == eraAwardConfig)
												{
													LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 获取奖励配置失败", CS$<>8__locals1.curEraID, CS$<>8__locals2.baseData.JunTuanId, RankNum), null, true);
												}
												else
												{
													if (eraAwardConfig.Progress == 4)
													{
														if (junTuanEraData.EraStageProcess != 100)
														{
															LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 进度不满足要求", CS$<>8__locals1.curEraID, CS$<>8__locals2.baseData.JunTuanId, RankNum), null, true);
															continue;
														}
													}
													else if (eraAwardConfig.Progress > 0 && eraAwardConfig.Progress >= (int)junTuanEraData.EraStage)
													{
														LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 进度不满足要求", CS$<>8__locals1.curEraID, CS$<>8__locals2.baseData.JunTuanId, RankNum), null, true);
														continue;
													}
													List<JunTuanRoleData> junTuanRoleList = JunTuanClient.getInstance().GetJunTuanRoleList(CS$<>8__locals2.baseData.BhList[0], CS$<>8__locals2.baseData.JunTuanId);
													if (null == junTuanRoleList)
													{
														LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 获取juntuanRoleList失败", CS$<>8__locals1.curEraID, CS$<>8__locals2.baseData.JunTuanId, RankNum), null, true);
													}
													else
													{
														foreach (int num in CS$<>8__locals2.baseData.BhList)
														{
															BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, num, 0);
															if (null == bangHuiDetailData)
															{
																LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 获取bhData失败", new object[]
																{
																	CS$<>8__locals1.curEraID,
																	CS$<>8__locals2.baseData.JunTuanId,
																	RankNum,
																	num
																}), null, true);
															}
															else
															{
																foreach (JunTuanRoleData junTuanRoleData in junTuanRoleList)
																{
																	if (junTuanRoleData.BhId != num)
																	{
																		LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} bhRole.BhId != bhid失败", new object[]
																		{
																			CS$<>8__locals1.curEraID,
																			CS$<>8__locals2.baseData.JunTuanId,
																			RankNum,
																			num,
																			junTuanRoleData.RoleId
																		}), null, true);
																	}
																	else
																	{
																		int eraDonateValueOffline = GameManager.ClientMgr.GetEraDonateValueOffline(junTuanRoleData.RoleId);
																		if (eraAwardConfig.Contribution > 0 && eraDonateValueOffline < eraAwardConfig.Contribution)
																		{
																			LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} 贡献{5} 贡献度不满足要求", new object[]
																			{
																				CS$<>8__locals1.curEraID,
																				CS$<>8__locals2.baseData.JunTuanId,
																				RankNum,
																				num,
																				junTuanRoleData.RoleId,
																				eraDonateValueOffline
																			}), null, true);
																		}
																		else
																		{
																			LogManager.WriteLog(2, string.Format("纪元{0}奖励 军团{1} 排行{2} 战盟{3} 角色{4} 贡献{5} 成功！", new object[]
																			{
																				CS$<>8__locals1.curEraID,
																				CS$<>8__locals2.baseData.JunTuanId,
																				RankNum,
																				num,
																				junTuanRoleData.RoleId,
																				eraDonateValueOffline
																			}), null, true);
																			string sContent = (RankNum != -1) ? string.Format(GLang.GetLang(109, new object[0]), eraAwardConfig.EraName, RankNum) : string.Format(GLang.GetLang(110, new object[0]), eraAwardConfig.EraName);
																			if (junTuanRoleData.JuTuanZhiWu == 1)
																			{
																				List<GoodsData> goodsData = Global.ConvertToGoodsDataList(eraAwardConfig.LeaderReward.Items, -1);
																				Global.UseMailGivePlayerAward3(junTuanRoleData.RoleId, goodsData, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
																			}
																			else if (junTuanRoleData.JuTuanZhiWu == 2)
																			{
																				List<GoodsData> goodsData = Global.ConvertToGoodsDataList(eraAwardConfig.MasterReward.Items, -1);
																				Global.UseMailGivePlayerAward3(junTuanRoleData.RoleId, goodsData, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
																			}
																			else
																			{
																				List<GoodsData> goodsData = Global.ConvertToGoodsDataList(eraAwardConfig.Reward.Items, -1);
																				Global.UseMailGivePlayerAward3(junTuanRoleData.RoleId, goodsData, GLang.GetLang(111, new object[0]), sContent, 0, 0, 0);
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
								this.CheckRankAwardDayID = offsetDay;
								GameManager.GameConfigMgr.SetGameConfigItem("era_rank_award", CS$<>8__locals1.curEraID.ToString());
								Global.UpdateDBGameConfigg("era_rank_award", CS$<>8__locals1.curEraID.ToString());
							}
						}
					}
				}
			}
		}

		private bool InRankAwardTime()
		{
			int currentEraID = JunTuanClient.getInstance().GetCurrentEraID();
			bool result;
			if (currentEraID <= 0)
			{
				result = false;
			}
			else
			{
				Dictionary<int, EraAwardConfig> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.EraAwardConfigDict;
				}
				foreach (EraAwardConfig eraAwardConfig in dictionary.Values)
				{
					if (eraAwardConfig.EraID == currentEraID && eraAwardConfig.AwardType == 2)
					{
						DateTime t = TimeUtil.NowDateTime();
						if (t >= eraAwardConfig.StartTime && t <= eraAwardConfig.EndTime)
						{
							return true;
						}
					}
				}
				result = false;
			}
			return result;
		}

		private int CheckCanGetAward(GameClient client, EraAwardConfig awardConfig, KFEraData kfEraData)
		{
			int result;
			if (null == kfEraData)
			{
				result = -11003;
			}
			else
			{
				List<int> eraAwardStateData = this.GetEraAwardStateData(client);
				if (eraAwardStateData == null || eraAwardStateData.Count == 0)
				{
					result = -11003;
				}
				else
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					for (int i = 1; i < eraAwardStateData.Count - 1; i += 2)
					{
						int key = eraAwardStateData[i];
						int value = eraAwardStateData[i + 1];
						dictionary.Add(key, value);
					}
					int num = 0;
					if (dictionary.TryGetValue(awardConfig.ID, out num))
					{
						result = -200;
					}
					else if (awardConfig.Contribution > 0 && GameManager.ClientMgr.GetEraDonateValue(client) < awardConfig.Contribution)
					{
						result = -12;
					}
					else if (awardConfig.EraID != kfEraData.EraID)
					{
						result = -12;
					}
					else
					{
						if (awardConfig.Progress == 4)
						{
							if (kfEraData.EraStageProcess != 100)
							{
								return -12;
							}
						}
						else if (awardConfig.Progress > 0 && awardConfig.Progress >= (int)kfEraData.EraStage)
						{
							return -12;
						}
						if (awardConfig.AwardType == 1)
						{
							if (0 < awardConfig.Progress && awardConfig.Progress <= kfEraData.EraTimePointList.Count)
							{
								int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
								DateTime t = new DateTime(DataHelper.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L);
								DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "10182");
								DateTime t2 = kfEraData.EraTimePointList[awardConfig.Progress - 1];
								if (t > t2 || roleParamsDateTimeFromDB > t2)
								{
									return -2006;
								}
							}
						}
						else
						{
							if (awardConfig.AwardType == 2)
							{
								return -12;
							}
							if (awardConfig.AwardType == 3)
							{
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public void OnChangeEraID(int CurrentJunTuanEraID)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient.ClientData.Faction > 0 && gameClient.ClientData.JunTuanId > 0)
				{
					int eraDonateValue = GameManager.ClientMgr.GetEraDonateValue(gameClient);
					GameManager.ClientMgr.NotifySelfParamsValueChange(gameClient, RoleCommonUseIntParamsIndexs.EraDonate, eraDonateValue);
				}
			}
			if (0 == CurrentJunTuanEraID)
			{
				GameManager.ClientMgr.NotifyAllActivityState(13, 0, "", "", 0);
			}
			else
			{
				GameManager.ClientMgr.NotifyAllActivityState(13, 1, "", "", 0);
			}
		}

		public void CheckAllJunTuanEraIcon(int juntuanID)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient.ClientData.JunTuanId == juntuanID)
				{
					if (gameClient._IconStateMgr.CheckJunTuanEraIcon(gameClient))
					{
						gameClient._IconStateMgr.SendIconStateToClient(gameClient);
					}
				}
			}
		}

		public void OnJunTuanZhiWuChanged(GameClient client)
		{
			int eraDonateValue = GameManager.ClientMgr.GetEraDonateValue(client);
			GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.EraDonate, eraDonateValue);
		}

		public bool CheckJunTuanEraIcon(GameClient client)
		{
			bool result;
			if (0 == client.ClientData.JunTuanId)
			{
				result = false;
			}
			else
			{
				KFEraData junTuanEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, true);
				if (null != junTuanEraData)
				{
					Dictionary<int, EraAwardConfig> dictionary = null;
					lock (this.ConfigMutex)
					{
						dictionary = this.EraAwardConfigDict;
					}
					foreach (EraAwardConfig eraAwardConfig in dictionary.Values)
					{
						if (eraAwardConfig.EraID == JunTuanClient.getInstance().GetCurrentEraID())
						{
							int num = this.CheckCanGetAward(client, eraAwardConfig, junTuanEraData);
							if (num == 0)
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		public List<int> GetEraAwardStateData(GameClient client)
		{
			int currentEraID = JunTuanClient.getInstance().GetCurrentEraID();
			List<int> result;
			if (currentEraID <= 0)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "45");
				if (roleParamsIntListFromDB.Count < 1)
				{
					for (int i = roleParamsIntListFromDB.Count; i < 1; i++)
					{
						roleParamsIntListFromDB.Add(0);
					}
					roleParamsIntListFromDB[0] = currentEraID;
				}
				if (roleParamsIntListFromDB[0] != currentEraID)
				{
					roleParamsIntListFromDB.Clear();
					for (int i = roleParamsIntListFromDB.Count; i < 1; i++)
					{
						roleParamsIntListFromDB.Add(0);
					}
					roleParamsIntListFromDB[0] = currentEraID;
				}
				result = roleParamsIntListFromDB;
			}
			return result;
		}

		private void SaveEraAwardStateData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "45", true);
		}

		public int GetEraFallGoodsMaxCount(IObject attacker, int goodsPackID)
		{
			int result;
			if (!(attacker is GameClient))
			{
				result = 0;
			}
			else if ((attacker as GameClient).ClientData.JunTuanId == 0)
			{
				result = 0;
			}
			else
			{
				Dictionary<int, List<EraDropConfig>> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.EraDropConfigDict;
				}
				List<EraDropConfig> list = null;
				if (!dictionary.TryGetValue(goodsPackID, out list))
				{
					result = 0;
				}
				else
				{
					KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData((attacker as GameClient).ClientData.JunTuanId, false);
					if (null == kfEraData)
					{
						result = 0;
					}
					else
					{
						EraDropConfig eraDropConfig = list.Find((EraDropConfig x) => x.EraID == kfEraData.EraID && x.EraStage == (int)kfEraData.EraStage);
						if (null == eraDropConfig)
						{
							result = 0;
						}
						else
						{
							result = eraDropConfig.MaxList;
						}
					}
				}
			}
			return result;
		}

		public List<FallGoodsItem> GetEraFallGoodsItem(GameClient client, int goodsPackID)
		{
			List<FallGoodsItem> result;
			if (client.ClientData.JunTuanId == 0)
			{
				result = null;
			}
			else
			{
				Dictionary<int, List<EraDropConfig>> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.EraDropConfigDict;
				}
				List<EraDropConfig> list = null;
				if (!dictionary.TryGetValue(goodsPackID, out list))
				{
					result = null;
				}
				else
				{
					KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
					if (null == kfEraData)
					{
						result = null;
					}
					else
					{
						EraDropConfig eraDropConfig = list.Find((EraDropConfig x) => x.EraID == kfEraData.EraID && x.EraStage == (int)kfEraData.EraStage);
						if (null == eraDropConfig)
						{
							result = null;
						}
						else
						{
							result = eraDropConfig.fallGoodsItemList;
						}
					}
				}
			}
			return result;
		}

		private EraData BuildEraData4Client(GameClient client)
		{
			KFEraData junTuanEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
			List<KFEraRankData> junTuanEraRankData = JunTuanClient.getInstance().GetJunTuanEraRankData(false);
			EraData eraData = new EraData();
			if (null != junTuanEraData)
			{
				eraData.EraID = junTuanEraData.EraID;
				eraData.EraStage = junTuanEraData.EraStage;
				eraData.EraStageProcess = junTuanEraData.EraStageProcess;
				eraData.FastEraStage = junTuanEraData.FastEraStage;
				eraData.FastEraStateProcess = junTuanEraData.FastEraStateProcess;
				eraData.EraTaskList = junTuanEraData.EraTaskList;
			}
			if (null != junTuanEraRankData)
			{
				foreach (KFEraRankData kferaRankData in junTuanEraRankData)
				{
					EraRankData eraRankData = new EraRankData();
					eraRankData.RankValue = kferaRankData.RankValue;
					eraRankData.JunTuanID = kferaRankData.JunTuanID;
					JunTuanBaseData junTuanBaseDataByJunTuanID = JunTuanManager.getInstance().GetJunTuanBaseDataByJunTuanID(kferaRankData.JunTuanID);
					eraRankData.JunTuanName = ((junTuanBaseDataByJunTuanID == null) ? "" : junTuanBaseDataByJunTuanID.JunTuanName);
					eraRankData.EraStage = kferaRankData.EraStage;
					eraRankData.EraStageProcess = kferaRankData.EraStageProcess;
					eraData.EraRankList.Add(eraRankData);
				}
			}
			Dictionary<int, EraAwardConfig> dictionary = null;
			lock (this.ConfigMutex)
			{
				dictionary = this.EraAwardConfigDict;
			}
			foreach (EraAwardConfig eraAwardConfig in dictionary.Values)
			{
				if (eraAwardConfig.EraID == JunTuanClient.getInstance().GetCurrentEraID())
				{
					int num = this.CheckCanGetAward(client, eraAwardConfig, junTuanEraData);
					if (num != 0)
					{
						if (num == -200)
						{
							eraData.EraAwardStateDict[eraAwardConfig.ID] = 1;
						}
						else
						{
							eraData.EraAwardStateDict[eraAwardConfig.ID] = 2;
						}
					}
				}
			}
			return eraData;
		}

		public bool ProcessGetEraDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
				{
					return true;
				}
				client.sendCmd<EraData>(nID, this.BuildEraData4Client(client), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEraDonateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int donateStage = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
				{
					return true;
				}
				if (this.InRankAwardTime())
				{
					num = -2001;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
					return true;
				}
				List<EraContributionConfig> list = null;
				Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> dictionary = null;
				lock (this.ConfigMutex)
				{
					list = this.EraContributionList;
					dictionary = this.EraTaskConfigDict;
				}
				KFEraData kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
				if (null == kfEraData)
				{
					num = -11003;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
					return true;
				}
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(kfEraData.EraID, donateStage);
				List<EraTaskConfig> list2 = null;
				if (!dictionary.TryGetValue(key, out list2))
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
					return true;
				}
				if ((int)kfEraData.EraStage < donateStage)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
					return true;
				}
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				foreach (EraTaskConfig eraTaskConfig in list2)
				{
					foreach (KeyValuePair<int, int> keyValuePair in eraTaskConfig.CompletionCondition)
					{
						int value = 0;
						if (!dictionary2.TryGetValue(keyValuePair.Key, out value))
						{
							int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, keyValuePair.Key);
							dictionary2.Add(keyValuePair.Key, totalGoodsCountByID);
						}
					}
				}
				foreach (EraTaskConfig eraTaskConfig in list2)
				{
					List<KeyValuePair<int, int>> list3 = new List<KeyValuePair<int, int>>();
					foreach (KeyValuePair<int, int> keyValuePair in eraTaskConfig.CompletionCondition)
					{
						int value = 0;
						dictionary2.TryGetValue(keyValuePair.Key, out value);
						list3.Add(new KeyValuePair<int, int>(keyValuePair.Key, value));
					}
					int value2 = list3[0].Value;
					int value3 = list3[1].Value;
					int value4 = list3[2].Value;
					if (value2 != 0 || value3 != 0 || 0 != value4)
					{
						if (!JunTuanClient.getInstance().EraDonate(client.ClientData.JunTuanId, eraTaskConfig.ID, value2, value3, value4))
						{
							num = -11003;
							client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
							return true;
						}
						int num3 = 0;
						using (List<KeyValuePair<int, int>>.Enumerator enumerator2 = list3.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<int, int> item = enumerator2.Current;
								int num4 = 0;
								KeyValuePair<int, int> item3 = item;
								if (num4 != item3.Value)
								{
									bool flag2 = false;
									bool flag3 = false;
									ClientManager clientMgr = GameManager.ClientMgr;
									SocketListener mySocketListener = Global._TCPManager.MySocketListener;
									TCPClientPool tcpClientPool = Global._TCPManager.tcpClientPool;
									TCPOutPacketPool tcpOutPacketPool = Global._TCPManager.TcpOutPacketPool;
									item3 = item;
									int key2 = item3.Key;
									item3 = item;
									if (clientMgr.NotifyUseGoods(mySocketListener, tcpClientPool, tcpOutPacketPool, client, key2, item3.Value, false, out flag2, out flag3, false))
									{
										EraContributionConfig eraContributionConfig = list.Find(delegate(EraContributionConfig x)
										{
											bool result;
											if (x.EraID == kfEraData.EraID && x.ProgressID == donateStage)
											{
												int goodsID = x.GoodsID;
												KeyValuePair<int, int> item2 = item;
												result = (goodsID == item2.Key);
											}
											else
											{
												result = false;
											}
											return result;
										});
										if (null != eraContributionConfig)
										{
											int num5 = num3;
											int contribution = eraContributionConfig.Contribution;
											item3 = item;
											num3 = num5 + contribution * item3.Value;
										}
									}
								}
							}
						}
						GameManager.ClientMgr.ModifyEraDonateValue(client, num3, "捐献", true, true, false);
					}
				}
				kfEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
				if (null == kfEraData)
				{
					num = -11003;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, 0, 0), false);
					return true;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, kfEraData.EraStage, kfEraData.EraStageProcess), false);
				client.sendCmd<EraData>(1090, this.BuildEraData4Client(client), false);
				if (client._IconStateMgr.CheckJunTuanEraIcon(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEraAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				if (client.ClientData.JunTuanId == 0 || JunTuanClient.getInstance().GetCurrentEraID() <= 0)
				{
					return true;
				}
				Dictionary<int, EraAwardConfig> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.EraAwardConfigDict;
				}
				EraAwardConfig eraAwardConfig = null;
				if (!dictionary.TryGetValue(num3, out eraAwardConfig))
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				if (eraAwardConfig.AwardType == 2)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				KFEraData junTuanEraData = JunTuanClient.getInstance().GetJunTuanEraData(client.ClientData.JunTuanId, false);
				num = this.CheckCanGetAward(client, eraAwardConfig, junTuanEraData);
				if (num != 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				List<AwardsItemData> items = eraAwardConfig.LeaderReward.Items;
				if (items != null && !Global.CanAddGoodsNum(client, items.Count))
				{
					num = -100;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				if (items != null)
				{
					foreach (AwardsItemData awardsItemData in items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "军团纪元", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				List<int> eraAwardStateData = this.GetEraAwardStateData(client);
				if (null != eraAwardStateData)
				{
					if (eraAwardConfig.AwardType == 1 || eraAwardConfig.AwardType == 3)
					{
						eraAwardStateData.Add(num3);
						eraAwardStateData.Add(1);
					}
					this.SaveEraAwardStateData(client, eraAwardStateData);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
				if (client._IconStateMgr.CheckJunTuanEraIcon(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private const string EraUI_FileName = "Config/EraUI.xml";

		private const string EraTask_FileName = "Config/EraTask.xml";

		private const string EraReward_FileName = "Config/EraReward.xml";

		private const string EraDrop_FileName = "Config/EraDrop.xml";

		private const string EraContribution_FileName = "Config/EraContribution.xml";

		private object ConfigMutex = new object();

		public Dictionary<int, EraUIConfig> EraUIConfigDict = new Dictionary<int, EraUIConfig>();

		public Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>> EraTaskConfigDict = new Dictionary<KeyValuePair<int, int>, List<EraTaskConfig>>();

		public Dictionary<int, EraAwardConfig> EraAwardConfigDict = new Dictionary<int, EraAwardConfig>();

		public Dictionary<int, List<EraDropConfig>> EraDropConfigDict = new Dictionary<int, List<EraDropConfig>>();

		public List<EraContributionConfig> EraContributionList = new List<EraContributionConfig>();

		private int CheckRankAwardDayID;

		private static EraManager instance = new EraManager();
	}
}
