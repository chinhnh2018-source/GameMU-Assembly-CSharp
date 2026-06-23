using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Logic.Damon;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Goods
{
	public class PetSkillManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		public static PetSkillManager getInstance()
		{
			return PetSkillManager.instance;
		}

		public bool initialize()
		{
			PetSkillManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1037, 2, 2, PetSkillManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1038, 3, 3, PetSkillManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1039, 1, 1, PetSkillManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1065, 3, 3, PetSkillManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1037:
				result = this.ProcessCmdPetSkillUp(client, nID, bytes, cmdParams);
				break;
			case 1038:
				result = this.ProcessCmdPetSkillAwake(client, nID, bytes, cmdParams);
				break;
			case 1039:
				result = this.ProcessCmdPetSkillAwakeCost(client, nID, bytes, cmdParams);
				break;
			default:
				result = (nID != 1065 || this.ProcessCmdPetSkillInherit(client, nID, bytes, cmdParams));
				break;
			}
			return result;
		}

		private bool ProcessCmdPetSkillUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				int petID = Convert.ToInt32(cmdParams[0]);
				int pit = Convert.ToInt32(cmdParams[1]);
				int cmdData = (int)PetSkillManager.PetSkillUp(client, petID, pit);
				client.sendCmd<int>(1037, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdPetSkillAwake(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 3))
				{
					return false;
				}
				int petID = Convert.ToInt32(cmdParams[0]);
				int num = Convert.ToInt32(cmdParams[1]);
				int num2 = Convert.ToInt32(cmdParams[2]);
				List<int> list = new List<int>();
				if (num > 0)
				{
					list.Add(num);
				}
				if (num2 > 0)
				{
					list.Add(num2);
				}
				string arg = "";
				int num3 = (int)PetSkillManager.PetSkillAwake(client, petID, list, out arg);
				client.sendCmd(1038, string.Format("{0}:{1}", num3, arg), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdPetSkillAwakeCost(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int skillAwakeCost = PetSkillManager.GetSkillAwakeCost(PetSkillManager.GetUpCount(client));
				client.sendCmd<int>(1039, skillAwakeCost, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdPetSkillInherit(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 3))
				{
					return false;
				}
				int srcPetID = Convert.ToInt32(cmdParams[0]);
				int tarPetID = Convert.ToInt32(cmdParams[1]);
				int userMoney = Convert.ToInt32(cmdParams[2]);
				string arg = "";
				int num = (int)PetSkillManager.PetSkillInherit(client, srcPetID, tarPetID, userMoney, out arg);
				client.sendCmd(1065, string.Format("{0}:{1}", num, arg), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private static EPetSkillState PetSkillInherit(GameClient client, int srcPetID, int tarPetID, int userMoney, out string outProps)
		{
			outProps = "";
			EPetSkillState result;
			if (GameFuncControlManager.IsGameFuncDisabled(15))
			{
				result = EPetSkillState.EnoOpen;
			}
			else if (!PetSkillManager.IsGongNengOpened(client))
			{
				result = EPetSkillState.EnoOpen;
			}
			else
			{
				GoodsData goodsData = DamonMgr.GetDamonGoodsDataByDbID(client, srcPetID);
				GoodsData goodsData2 = DamonMgr.GetDamonGoodsDataByDbID(client, tarPetID);
				if (null == goodsData)
				{
					goodsData = Global.GetGoodsByDbID(client, srcPetID);
				}
				else if (goodsData.Site != 5000)
				{
					return EPetSkillState.EnoUsing;
				}
				if (null == goodsData2)
				{
					goodsData2 = Global.GetGoodsByDbID(client, tarPetID);
				}
				else if (goodsData2.Site != 5000)
				{
					return EPetSkillState.EnoUsing;
				}
				if (goodsData == null || goodsData.GCount <= 0 || goodsData2 == null || goodsData2.GCount <= 0)
				{
					result = EPetSkillState.EnoPet;
				}
				else
				{
					if (1 == userMoney)
					{
						if (client.ClientData.UserMoney < PetSkillManager.JingLingChuanChengXiaoHaoZhuanShi && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, PetSkillManager.JingLingChuanChengXiaoHaoZhuanShi))
						{
							return EPetSkillState.EnoDiamond;
						}
					}
					else if (Global.GetTotalBindTongQianAndTongQianVal(client) < PetSkillManager.JingLingChuanChengXiaoHaoJinBi)
					{
						return EPetSkillState.EnoGold;
					}
					List<PetSkillInfo> petSkillInfo = PetSkillManager.GetPetSkillInfo(goodsData);
					List<PetSkillInfo> petSkillInfo2 = PetSkillManager.GetPetSkillInfo(goodsData2);
					int pitLoop;
					for (pitLoop = 1; pitLoop < 4; pitLoop++)
					{
						PetSkillInfo petSkillInfo3 = petSkillInfo.Find((PetSkillInfo _g) => _g.Pit == pitLoop);
						PetSkillInfo petSkillInfo4 = petSkillInfo2.Find((PetSkillInfo _g) => _g.Pit == pitLoop);
						if (petSkillInfo3 == null || null == petSkillInfo4)
						{
							return EPetSkillState.EpitWrong;
						}
						if (petSkillInfo3.PitIsOpen && !petSkillInfo4.PitIsOpen)
						{
							return EPetSkillState.EpitWrong;
						}
					}
					if (1 == userMoney)
					{
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, PetSkillManager.JingLingChuanChengXiaoHaoZhuanShi, "精灵技能传承", true, true, false, DaiBiSySType.JingLingJiNengChuanCheng))
						{
							return EPetSkillState.EnoDiamond;
						}
					}
					else if (!Global.SubBindTongQianAndTongQian(client, PetSkillManager.JingLingChuanChengXiaoHaoJinBi, "精灵技能传承"))
					{
						return EPetSkillState.EnoGold;
					}
					int randomNumber = Global.GetRandomNumber(1, 101);
					if (randomNumber > PetSkillManager.JingLingChuanChengGoodsRate)
					{
						result = EPetSkillState.EnoInheritFail;
					}
					else
					{
						long num = PetSkillManager.DelGoodsReturnLingJing(goodsData2);
						UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
						{
							RoleID = client.ClientData.RoleID,
							DbID = tarPetID,
							WashProps = null
						};
						updateGoodsArgs.ElementhrtsProps = new List<int>();
						for (int i = 0; i < petSkillInfo2.Count; i++)
						{
							PetSkillInfo petSkillInfo5 = (i < petSkillInfo.Count) ? petSkillInfo[i] : petSkillInfo2[i];
							updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo5.PitIsOpen ? 1 : 0);
							updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo5.Level);
							updateGoodsArgs.ElementhrtsProps.Add((i < petSkillInfo.Count) ? petSkillInfo5.SkillID : 0);
						}
						Global.UpdateGoodsProp(client, goodsData2, updateGoodsArgs, true);
						UpdateGoodsArgs updateGoodsArgs2 = new UpdateGoodsArgs
						{
							RoleID = client.ClientData.RoleID,
							DbID = srcPetID,
							WashProps = null
						};
						updateGoodsArgs2.ElementhrtsProps = new List<int>();
						foreach (PetSkillInfo petSkillInfo5 in petSkillInfo)
						{
							PetSkillInfo petSkillInfo5;
							updateGoodsArgs2.ElementhrtsProps.Add(petSkillInfo5.PitIsOpen ? 1 : 0);
							updateGoodsArgs2.ElementhrtsProps.Add(1);
							updateGoodsArgs2.ElementhrtsProps.Add(0);
						}
						Global.UpdateGoodsProp(client, goodsData, updateGoodsArgs2, true);
						GameManager.ClientMgr.ModifyMUMoHeValue(client, (int)num, "精灵技能传承", true, true, false);
						if (goodsData.Using > 0 || goodsData2.Using > 0)
						{
							PetSkillManager.UpdateRolePetSkill(client);
						}
						outProps = string.Format("{0}:{1}", string.Join<int>(",", updateGoodsArgs2.ElementhrtsProps.ToArray()), string.Join<int>(",", updateGoodsArgs.ElementhrtsProps.ToArray()));
						result = EPetSkillState.Success;
					}
				}
			}
			return result;
		}

		private static EPetSkillState PetSkillUp(GameClient client, int petID, int pit)
		{
			EPetSkillState result;
			if (!PetSkillManager.IsGongNengOpened(client))
			{
				result = EPetSkillState.EnoOpen;
			}
			else
			{
				GoodsData damonGoodsDataByDbID = DamonMgr.GetDamonGoodsDataByDbID(client, petID);
				if (damonGoodsDataByDbID == null || damonGoodsDataByDbID.GCount <= 0)
				{
					result = EPetSkillState.EnoPet;
				}
				else if (damonGoodsDataByDbID.Site != 5000)
				{
					result = EPetSkillState.EnoUsing;
				}
				else if (pit < 1 || pit > 4)
				{
					result = EPetSkillState.EpitWrong;
				}
				else
				{
					List<PetSkillInfo> petSkillInfo = PetSkillManager.GetPetSkillInfo(damonGoodsDataByDbID);
					PetSkillInfo petSkillInfo2 = petSkillInfo.Find((PetSkillInfo _g) => _g.Pit == pit);
					if (!petSkillInfo2.PitIsOpen)
					{
						result = EPetSkillState.EpitNoOpen;
					}
					else if (petSkillInfo2.SkillID <= 0)
					{
						result = EPetSkillState.EpitSkillNull;
					}
					else
					{
						int psUpMaxLevel = PetSkillManager.GetPsUpMaxLevel();
						if (petSkillInfo2.Level >= psUpMaxLevel)
						{
							result = EPetSkillState.ElevelMax;
						}
						else
						{
							int level = petSkillInfo2.Level;
							int num = petSkillInfo2.Level + 1;
							int num2 = (int)PetSkillManager.GetPsUpCost(num);
							long num3 = (long)GameManager.ClientMgr.GetMUMoHeValue(client);
							if (num3 < (long)num2)
							{
								result = EPetSkillState.EnoLingJing;
							}
							else
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, -num2, "精灵技能升级", true, true, false);
								petSkillInfo2.Level = num;
								UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
								{
									RoleID = client.ClientData.RoleID,
									DbID = petID,
									WashProps = null
								};
								updateGoodsArgs.ElementhrtsProps = new List<int>();
								foreach (PetSkillInfo petSkillInfo3 in petSkillInfo)
								{
									updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo3.PitIsOpen ? 1 : 0);
									updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo3.Level);
									updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo3.SkillID);
								}
								Global.UpdateGoodsProp(client, damonGoodsDataByDbID, updateGoodsArgs, true);
								PetSkillManager.UpdateRolePetSkill(client);
								EventLogManager.AddPetSkillEvent(client, LogRecordType.PetSkill, EPetSkillLog.Up, new object[]
								{
									petID,
									damonGoodsDataByDbID.GoodsID,
									pit,
									level,
									num
								});
								result = EPetSkillState.Success;
							}
						}
					}
				}
			}
			return result;
		}

		public static List<PetSkillInfo> GetPetSkillInfo(GoodsData data)
		{
			List<PetSkillInfo> list = new List<PetSkillInfo>();
			if (data.ElementhrtsProps == null)
			{
				data.ElementhrtsProps = new List<int>
				{
					0,
					1,
					0,
					0,
					1,
					0,
					0,
					1,
					0,
					0,
					1,
					0
				};
			}
			int pit = 1;
			for (int i = 0; i < data.ElementhrtsProps.Count; i++)
			{
				PetSkillInfo petSkillInfo = new PetSkillInfo();
				petSkillInfo.PitIsOpen = (data.ElementhrtsProps[i++] > 0);
				if (!petSkillInfo.PitIsOpen)
				{
					int pitOpenLevel = PetSkillManager.GetPitOpenLevel(pit);
					if (data.Forge_level + 1 >= pitOpenLevel)
					{
						petSkillInfo.PitIsOpen = true;
					}
				}
				petSkillInfo.Pit = pit++;
				petSkillInfo.Level = data.ElementhrtsProps[i++];
				petSkillInfo.SkillID = data.ElementhrtsProps[i];
				list.Add(petSkillInfo);
			}
			return list;
		}

		private static EPetSkillState PetSkillAwake(GameClient client, int petID, List<int> lockPitList, out string result)
		{
			result = "";
			EPetSkillState result2;
			if (!PetSkillManager.IsGongNengOpened(client))
			{
				result2 = EPetSkillState.EnoOpen;
			}
			else
			{
				GoodsData damonGoodsDataByDbID = DamonMgr.GetDamonGoodsDataByDbID(client, petID);
				if (damonGoodsDataByDbID == null || damonGoodsDataByDbID.GCount <= 0)
				{
					result2 = EPetSkillState.EnoPet;
				}
				else if (damonGoodsDataByDbID.Site != 5000)
				{
					result2 = EPetSkillState.EnoUsing;
				}
				else
				{
					List<PetSkillInfo> petSkillList = PetSkillManager.GetPetSkillInfo(damonGoodsDataByDbID);
					int num = 0;
					if (lockPitList.Count > 0)
					{
						foreach (int num2 in lockPitList)
						{
							if (num2 > 4)
							{
								return EPetSkillState.EpitWrong;
							}
							if (!petSkillList[num2 - 1].PitIsOpen)
							{
								return EPetSkillState.EpitNoOpen;
							}
						}
						num = PetSkillManager.GetPitLockCost(lockPitList.Count);
						if (num > 0 && client.ClientData.UserMoney < num)
						{
							return EPetSkillState.EnoDiamond;
						}
					}
					int upCount = PetSkillManager.GetUpCount(client);
					int skillAwakeCost = PetSkillManager.GetSkillAwakeCost(upCount);
					long num3 = (long)GameManager.ClientMgr.GetMUMoHeValue(client);
					if (num3 < (long)skillAwakeCost)
					{
						result2 = EPetSkillState.EnoLingJing;
					}
					else
					{
						List<PetSkillInfo> list = petSkillList.FindAll((PetSkillInfo _g) => _g.PitIsOpen);
						if (list == null || list.Count <= 0)
						{
							result2 = EPetSkillState.EpitNoOpen;
						}
						else
						{
							List<PetSkillInfo> list2;
							if (lockPitList != null && lockPitList.Count > 0)
							{
								IEnumerable<PetSkillInfo> source = from info in list
								where info.PitIsOpen && lockPitList.IndexOf(info.Pit) < 0
								select info;
								if (!source.Any<PetSkillInfo>())
								{
									return EPetSkillState.EnoPitAwake;
								}
								list2 = source.ToList<PetSkillInfo>();
							}
							else
							{
								list2 = list;
							}
							IEnumerable<PetSkillInfo> source2 = from info in list2
							where info.PitIsOpen && info.SkillID <= 0
							select info;
							if (source2.Any<PetSkillInfo>())
							{
								List<PetSkillInfo> list3 = source2.ToList<PetSkillInfo>();
								list2 = list3;
							}
							int randomNumber = Global.GetRandomNumber(0, list2.Count);
							PetSkillInfo petSkillInfo = list2[randomNumber];
							List<int> list4 = new List<int>();
							IEnumerable<KeyValuePair<int, PetSkillAwakeInfo>> enumerable = from p in PetSkillManager._psDic
							where !(from g in petSkillList
							select g.SkillID).Contains(p.Value.SkillID)
							select p;
							if (!enumerable.Any<KeyValuePair<int, PetSkillAwakeInfo>>())
							{
								result2 = EPetSkillState.EnoSkillAwake;
							}
							else
							{
								int num4 = 0;
								int maxV = enumerable.Sum((KeyValuePair<int, PetSkillAwakeInfo> _s) => _s.Value.Rate);
								int randomNumber2 = Global.GetRandomNumber(0, maxV);
								int num5 = 0;
								foreach (KeyValuePair<int, PetSkillAwakeInfo> keyValuePair in enumerable)
								{
									num4 = keyValuePair.Key;
									int rate = keyValuePair.Value.Rate;
									num5 += keyValuePair.Value.Rate;
									if (num5 >= randomNumber2)
									{
										break;
									}
								}
								int skillID = petSkillInfo.SkillID;
								petSkillInfo.SkillID = num4;
								if (num > 0 && !GameManager.ClientMgr.SubUserMoney(client, num, "精灵技能领悟", true, true, true, true, DaiBiSySType.None))
								{
									result2 = EPetSkillState.EnoDiamond;
								}
								else
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, -skillAwakeCost, "精灵技能领悟", true, true, false);
									PetSkillManager.ModifyUpCount(client, upCount + 1);
									UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
									{
										RoleID = client.ClientData.RoleID,
										DbID = petID,
										WashProps = null
									};
									updateGoodsArgs.ElementhrtsProps = new List<int>();
									foreach (PetSkillInfo petSkillInfo2 in petSkillList)
									{
										updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo2.PitIsOpen ? 1 : 0);
										updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo2.Level);
										updateGoodsArgs.ElementhrtsProps.Add(petSkillInfo2.SkillID);
									}
									Global.UpdateGoodsProp(client, damonGoodsDataByDbID, updateGoodsArgs, true);
									result = string.Join<int>(",", updateGoodsArgs.ElementhrtsProps.ToArray());
									PetSkillManager.UpdateRolePetSkill(client);
									EventLogManager.AddPetSkillEvent(client, LogRecordType.PetSkill, EPetSkillLog.Awake, new object[]
									{
										petID,
										damonGoodsDataByDbID.GoodsID,
										petSkillInfo.Pit,
										skillID,
										num4
									});
									result2 = EPetSkillState.Success;
								}
							}
						}
					}
				}
			}
			return result2;
		}

		public static int GetUpCount(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "PetSkillUpCount");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count > 0)
			{
				num2 = roleParamsIntListFromDB[0];
			}
			int offsetDayNow = Global.GetOffsetDayNow();
			if (num2 == offsetDayNow)
			{
				num = roleParamsIntListFromDB[1];
			}
			else
			{
				PetSkillManager.ModifyUpCount(client, num);
			}
			return num;
		}

		public static void ModifyUpCount(GameClient client, int count)
		{
			List<int> list = new List<int>();
			list.AddRange(new int[]
			{
				Global.GetOffsetDayNow(),
				count
			});
			Global.SaveRoleParamsIntListToDB(client, list, "PetSkillUpCount", true);
		}

		public static long DelGoodsReturnLingJing(GoodsData goodsData)
		{
			long num = 0L;
			List<PetSkillInfo> petSkillInfo = PetSkillManager.GetPetSkillInfo(goodsData);
			IEnumerable<PetSkillInfo> enumerable = from info in petSkillInfo
			where info.PitIsOpen && info.Level > 0
			select info;
			long result;
			if (!enumerable.Any<PetSkillInfo>())
			{
				result = num;
			}
			else
			{
				using (IEnumerator<PetSkillInfo> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PetSkillInfo info = enumerator.Current;
						num += (from levelInfo in PetSkillManager._psLevelUpDic
						where levelInfo.Key <= info.Level
						select levelInfo.Value).Sum();
					}
				}
				result = num;
			}
			return result;
		}

		public static void UpdateRolePetSkill(GameClient client)
		{
			List<PassiveSkillData> list = new List<PassiveSkillData>();
			List<GoodsData> damonGoodsDataList = client.ClientData.DamonGoodsDataList;
			GoodsData goodsData = client.ClientData.DamonGoodsDataList.Find((GoodsData _g) => _g.Using > 0);
			if (goodsData != null)
			{
				List<PetSkillInfo> list2 = new List<PetSkillInfo>();
				List<PetSkillInfo> petSkillInfo = PetSkillManager.GetPetSkillInfo(goodsData);
				IEnumerable<PetSkillInfo> enumerable = from info in petSkillInfo
				where info.PitIsOpen && info.SkillID > 0
				select info;
				if (enumerable.Any<PetSkillInfo>())
				{
					foreach (PetSkillInfo petSkillInfo2 in enumerable)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(petSkillInfo2.SkillID, out systemXmlItem))
						{
							list.Add(new PassiveSkillData
							{
								skillId = petSkillInfo2.SkillID,
								skillLevel = petSkillInfo2.Level,
								triggerRate = (int)(systemXmlItem.GetDoubleValue("TriggerOdds") * 100.0),
								triggerType = systemXmlItem.GetIntValue("TriggerType", -1),
								coolDown = systemXmlItem.GetIntValue("CDTime", -1),
								triggerCD = systemXmlItem.GetIntValue("TriggerCD", -1)
							});
						}
					}
				}
			}
			client.passiveSkillModule.UpdateSkillList(list);
			JingLingQiYuanManager.getInstance().RefreshProps(client, true);
		}

		public static void InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("JingLingChuanChengGoodsRate");
			PetSkillManager.JingLingChuanChengGoodsRate = Global.SafeConvertToInt32(paramValueByName);
			paramValueByName = GameManager.systemParamsList.GetParamValueByName("JingLingChuanChengXiaoHaoJinBi");
			PetSkillManager.JingLingChuanChengXiaoHaoJinBi = Global.SafeConvertToInt32(paramValueByName);
			paramValueByName = GameManager.systemParamsList.GetParamValueByName("JingLingChuanChengXiaoHaoZhuanShi");
			PetSkillManager.JingLingChuanChengXiaoHaoZhuanShi = Global.SafeConvertToInt32(paramValueByName);
			PetSkillManager.LoadPsInfo();
			PetSkillManager.LoadPsUpInfo();
			PetSkillManager.LoadPitOpenLevel();
			PetSkillManager.LoadPitLockCost();
		}

		private static void LoadPsInfo()
		{
			string text = Global.GameResPath("Config/PetSkill.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					PetSkillManager._psDic.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							PetSkillAwakeInfo petSkillAwakeInfo = new PetSkillAwakeInfo();
							petSkillAwakeInfo.SkillID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SkillID", "0"));
							petSkillAwakeInfo.RateMin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "StartValues", "0"));
							petSkillAwakeInfo.RateMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndValues", "0"));
							petSkillAwakeInfo.Rate = petSkillAwakeInfo.RateMax - petSkillAwakeInfo.RateMin + 1;
							PetSkillManager._psDic.Add(petSkillAwakeInfo.SkillID, petSkillAwakeInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		public static void LoadPsUpInfo()
		{
			string text = Global.GameResPath("Config/PetSkillLevelup.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					PetSkillManager._psLevelUpDic.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Level", "0"));
							long value = Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "Cost", "0"));
							if (!PetSkillManager._psLevelUpDic.ContainsKey(key))
							{
								PetSkillManager._psLevelUpDic.Add(key, value);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		public static void LoadPitOpenLevel()
		{
			try
			{
				PetSkillManager._pitOpenDic.Clear();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("PatSkillCostLevel");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						int key = int.Parse(array3[0]);
						int value = int.Parse(array3[1]);
						if (!PetSkillManager._pitOpenDic.ContainsKey(key))
						{
							PetSkillManager._pitOpenDic.Add(key, value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", "PatSkillCostLevel"), null, true);
			}
		}

		public static void LoadPitLockCost()
		{
			try
			{
				PetSkillManager._pitLockDic.Clear();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("PatSkillCostZuanShi");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					foreach (string text in array)
					{
						string[] array3 = text.Split(new char[]
						{
							','
						});
						int key = int.Parse(array3[0]);
						int value = int.Parse(array3[1]);
						if (!PetSkillManager._pitLockDic.ContainsKey(key))
						{
							PetSkillManager._pitLockDic.Add(key, value);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", "PatSkillCostLevel"), null, true);
			}
		}

		public static PetSkillAwakeInfo GetPsInfo(int id)
		{
			PetSkillAwakeInfo result;
			if (PetSkillManager._psDic.ContainsKey(id))
			{
				result = PetSkillManager._psDic[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int GetPsUpMaxLevel()
		{
			int result;
			if (PetSkillManager._psLevelUpDic == null && PetSkillManager._psLevelUpDic.Count <= 0)
			{
				result = 0;
			}
			else
			{
				result = PetSkillManager._psLevelUpDic.Keys.Max();
			}
			return result;
		}

		public static long GetPsUpCost(int nextLevel)
		{
			long result;
			if (PetSkillManager._psLevelUpDic.ContainsKey(nextLevel))
			{
				result = PetSkillManager._psLevelUpDic[nextLevel];
			}
			else
			{
				result = 0L;
			}
			return result;
		}

		public static int GetPitOpenLevel(int pit)
		{
			int result;
			if (PetSkillManager._pitOpenDic.ContainsKey(pit))
			{
				result = PetSkillManager._pitOpenDic[pit];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static int GetPitLockCost(int count)
		{
			int result;
			if (PetSkillManager._pitLockDic.ContainsKey(count))
			{
				result = PetSkillManager._pitLockDic[count];
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static int GetSkillAwakeCost(int count)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("PatSkillCostLingJing", ',');
			if (count >= paramValueIntArrayByName.Length)
			{
				count = paramValueIntArrayByName.Length - 1;
			}
			return paramValueIntArrayByName[count];
		}

		public static bool IsGongNengOpened(GameClient client)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(9) && GlobalNew.IsGongNengOpened(client, 76, false) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("PetSkill");
		}

		private const int PIT_MIN = 1;

		private const int PIT_MAX = 4;

		private const int UP_LEVEL_MAX = 5;

		private const int STATUE_COUNT = 8;

		private const int RANDOM_SEED_AWAKE = 100000;

		public static int _gmRate = 1;

		private static PetSkillManager instance = new PetSkillManager();

		private static Dictionary<int, PetSkillAwakeInfo> _psDic = new Dictionary<int, PetSkillAwakeInfo>();

		private static Dictionary<int, long> _psLevelUpDic = new Dictionary<int, long>();

		private static Dictionary<int, int> _pitOpenDic = new Dictionary<int, int>();

		private static Dictionary<int, int> _pitLockDic = new Dictionary<int, int>();

		private static int JingLingChuanChengGoodsRate;

		private static int JingLingChuanChengXiaoHaoJinBi;

		private static int JingLingChuanChengXiaoHaoZhuanShi;
	}
}
