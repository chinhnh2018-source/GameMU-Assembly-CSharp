using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Ornament;
using GameServer.Logic.Talent;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.JingJiChang
{
	public class JingJiChangManager : JingJiChangConstants, IManager
	{
		private JingJiChangManager()
		{
		}

		public static JingJiChangManager getInstance()
		{
			return JingJiChangManager.instance;
		}

		public bool initialize()
		{
			this.loadStaticData();
			this.initCmdProcessor();
			this.initListener();
			return true;
		}

		private void loadStaticData()
		{
			this.jingjiMainConfig.LoadFromXMlFile("Config/JingJi.xml", "", "ID", 0);
			this.junxianConfig.LoadFromXMlFile("Config/JunXian.xml", "", "Level", 0);
			this.jingjiFuBenId = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiFuBenID", -1);
			this.jingjiBuffId = (int)GameManager.systemParamsList.GetParamValueIntByName("JingJiBuff", -1);
			GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this.jingjiFuBenId, out this.jingjiFuBenItem);
			this.nJingJiChangMapCode = this.jingjiFuBenItem.GetIntValue("MapCode", -1);
			this.jingjiFuBenMinZhuanSheng = this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1);
		}

		private void initCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(578, 2, JingJiDetailCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(1340, 2, JingJiGetRoleLooksCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(579, 4, JingJiRequestChallengeCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(582, 2, JingJiChallengeInfoCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(583, 1, JingJiRankingRewardCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(584, 1, JingJiRemoveCDCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(585, 2, JingJiGetBuffCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(586, 1, JingJiJunxianLevelupCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(587, 1, JingJiLeaveFuBenCmdProcessor.getInstance());
			TCPCmdDispatcher.getInstance().registerProcessor(634, 1, JingJiStartFightCmdProcessor.getInstance());
		}

		private void initListener()
		{
			GlobalEventSource.getInstance().registerListener(9, JingJiPlayerLevelupEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(10, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(11, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(12, JingJiPlayerLogoutEventListener.getInstance());
			GlobalEventSource.getInstance().registerListener(13, JingJiPlayerLeaveFuBenEventListener.getInstance());
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
			this.removeListener();
			if (null != this.jingjichangInstances)
			{
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.Clear();
				}
			}
			return true;
		}

		private void removeListener()
		{
			GlobalEventSource.getInstance().removeListener(9, JingJiPlayerLevelupEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(10, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(11, JingJiFuBenEndEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(12, JingJiPlayerLogoutEventListener.getInstance());
			GlobalEventSource.getInstance().removeListener(13, JingJiPlayerLeaveFuBenEventListener.getInstance());
		}

		public JingJiDetailData getDetailData(GameClient player, int requestType = 0)
		{
			JingJiDetailData jingJiDetailData = new JingJiDetailData();
			JingJiDetailData result;
			if (player.ClientData.Level < this.jingjiFuBenItem.GetIntValue("MinLevel", -1) && player.ClientData.ChangeLifeCount == this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1))
			{
				jingJiDetailData.state = ResultCode.Illegal;
				result = jingJiDetailData;
			}
			else if (requestType != 0 && requestType != 1)
			{
				jingJiDetailData.state = ResultCode.Illegal;
				result = jingJiDetailData;
			}
			else if (player.ClientData.CurrentLifeV <= 0 || player.ClientData.CurrentAction == 12)
			{
				jingJiDetailData.state = ResultCode.Dead_Error;
				result = jingJiDetailData;
			}
			else if (player.ClientData.HideGM > 0)
			{
				jingJiDetailData.state = ResultCode.Illegal;
				result = jingJiDetailData;
			}
			else
			{
				PlayerJingJiData playerJingJiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == playerJingJiData.baseProps)
				{
					PlayerJingJiData playerJingJiData2 = this.createJingJiData(player);
					Global.sendToDB<byte, byte[]>(10142, DataHelper.ObjectToBytes<PlayerJingJiData>(playerJingJiData2), player.ServerId);
					playerJingJiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				}
				jingJiDetailData.ranking = playerJingJiData.ranking;
				jingJiDetailData.winCount = playerJingJiData.winCount;
				jingJiDetailData.nextRewardTime = playerJingJiData.nextRewardTime;
				jingJiDetailData.nextChallengeTime = playerJingJiData.nextChallengeTime;
				jingJiDetailData.maxwincount = playerJingJiData.MaxWinCnt;
				int[] array = new int[JingJiChangConstants.CanChallengeNum];
				if (requestType == 0)
				{
					if (jingJiDetailData.ranking >= 1 && jingJiDetailData.ranking <= 3)
					{
						int num = 0;
						for (int i = 1; i <= 4; i++)
						{
							if (i != jingJiDetailData.ranking)
							{
								array[num++] = i;
							}
						}
					}
					else
					{
						int num2 = -1;
						if (jingJiDetailData.ranking == -1)
						{
							num2 = this.jingjiMainConfig.SystemXmlItemDict.Values.Max((SystemXmlItem x) => x.GetIntValue("MaxRank", -1));
							array[0] = -num2;
							array[1] = -num2 * 2;
							array[2] = --num2 * 3;
						}
						else
						{
							foreach (SystemXmlItem systemXmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
							{
								if (jingJiDetailData.ranking >= systemXmlItem.GetIntValue("MinRank", -1) && jingJiDetailData.ranking <= systemXmlItem.GetIntValue("MaxRank", -1))
								{
									num2 = systemXmlItem.GetIntValue("Coefficient", -1);
									break;
								}
							}
							for (int i = 0; i < JingJiChangConstants.CanChallengeNum; i++)
							{
								array[i] = jingJiDetailData.ranking - (i + 1) * num2;
							}
						}
					}
				}
				else if (jingJiDetailData.ranking >= 1 && jingJiDetailData.ranking <= 3)
				{
					int num = 0;
					for (int i = 1; i <= 4; i++)
					{
						if (i != jingJiDetailData.ranking)
						{
							array[num++] = i;
						}
					}
				}
				else
				{
					int num = 0;
					for (int i = 1; i <= 3; i++)
					{
						if (i != jingJiDetailData.ranking)
						{
							array[num++] = i;
						}
					}
				}
				List<PlayerJingJiMiniData> beChallengerData = Global.sendToDB<List<PlayerJingJiMiniData>, byte[]>(10141, DataHelper.ObjectToBytes<int[]>(array), player.ServerId);
				jingJiDetailData.beChallengerData = beChallengerData;
				jingJiDetailData.freeChallengeNum = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
				FuBenData fuBenData = Global.GetFuBenData(player, this.jingjiFuBenId);
				int num3;
				int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num3);
				int useFreeChallengeNum = (fuBenEnterNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? fuBenEnterNum : this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
				jingJiDetailData.useFreeChallengeNum = useFreeChallengeNum;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
				int vipLevel = player.ClientData.VipLevel;
				jingJiDetailData.vipChallengeNum = paramValueIntArrayByName[vipLevel];
				int useVipChallengeNum = (fuBenEnterNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? 0 : (fuBenEnterNum - this.jingjiFuBenItem.GetIntValue("EnterNumber", -1));
				jingJiDetailData.useVipChallengeNum = useVipChallengeNum;
				jingJiDetailData.state = ResultCode.Success;
				result = jingJiDetailData;
			}
			return result;
		}

		public int getJingJiMapCode()
		{
			return (this.jingjiFuBenItem != null) ? this.jingjiFuBenItem.GetIntValue("MapCode", -1) : -1;
		}

		public bool CanGradeJunXian(GameClient player)
		{
			int junxian = this.getJunxian(player);
			bool result;
			if (junxian + 1 > this.junxianConfig.SystemXmlItemDict.Count)
			{
				result = false;
			}
			else
			{
				int intValue = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetIntValue("NeedShengWang", -1);
				result = (this.getShengWangValue(player) >= intValue && GlobalNew.IsGongNengOpened(player, 11, false));
			}
			return result;
		}

		public int upGradeJunXian(GameClient player)
		{
			int num = this.check(player);
			int result;
			if (num != ResultCode.Success)
			{
				result = num;
			}
			else
			{
				int junxian = this.getJunxian(player);
				if (junxian + 1 > this.junxianConfig.SystemXmlItemDict.Count)
				{
					result = ResultCode.Illegal;
				}
				else
				{
					string stringValue = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetStringValue("NeedGoods");
					List<List<int>> list = ConfigHelper.ParserIntArrayList(stringValue, true, '|', ',');
					for (int i = 0; i < list.Count; i++)
					{
						int num2 = list[i][0];
						int num3 = list[i][1];
						int totalGoodsCountByID = Global.GetTotalGoodsCountByID(player, num2);
						if (totalGoodsCountByID < num3)
						{
							return ResultCode.GoodsNotEnough;
						}
					}
					int intValue = this.junxianConfig.SystemXmlItemDict[junxian + 1].GetIntValue("NeedShengWang", -1);
					if (!this.consumeShengWang(player, intValue))
					{
						result = ResultCode.ShengWang_Not_Enough_Error;
					}
					else if (!GlobalNew.IsGongNengOpened(player, 11, false))
					{
						result = ResultCode.Junxian_Null_Error;
					}
					else
					{
						bool flag = false;
						bool flag2 = false;
						for (int i = 0; i < list.Count; i++)
						{
							int num2 = list[i][0];
							int num3 = list[i][1];
							if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, player, num2, num3, false, out flag, out flag2, false))
							{
								LogManager.WriteLog(2, string.Format("进阶军衔改变军衔时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", num2, num3), null, true);
							}
							GoodsData goodsData = new GoodsData();
							goodsData.GoodsID = num2;
							goodsData.GCount = num3;
						}
						this.modifyJunxian(player);
						if (ResultCode.Success == this.activeJunXianBuff(player, true))
						{
							Global.BroadcastClientMUShengWang(player, this.getJunxian(player));
						}
						if (!GlobalNew.IsGongNengOpened(player, 11, false))
						{
							result = ResultCode.Junxian_Null_Error;
						}
						else
						{
							player._IconStateMgr.CheckJingJiChangJunXian(player);
							player._IconStateMgr.SendIconStateToClient(player);
							result = ResultCode.Success;
						}
					}
				}
			}
			return result;
		}

		public int activeJunXianBuff(GameClient player, bool replace)
		{
			int num = this.check(player);
			int result;
			if (num != ResultCode.Success)
			{
				result = num;
			}
			else
			{
				int junxian = this.getJunxian(player);
				if (junxian <= 0)
				{
					result = ResultCode.Junxian_Null_Error;
				}
				else if (!GlobalNew.IsGongNengOpened(player, 11, false))
				{
					result = ResultCode.Junxian_Null_Error;
				}
				else if (replace)
				{
					if (!this.consumeShengWang(player, this.junxianConfig.SystemXmlItemDict[junxian].GetIntValue("XiaoHaoShengWang", -1)))
					{
						result = ResultCode.ShengWang_Not_Enough_Error;
					}
					else
					{
						this.installJunXianBuff(player);
						result = ResultCode.Success;
					}
				}
				else if (this.isHasJunXianBuff(player))
				{
					result = ResultCode.HasJunxianBuff_Error;
				}
				else if (!this.consumeShengWang(player, this.junxianConfig.SystemXmlItemDict[junxian].GetIntValue("XiaoHaoShengWang", -1)))
				{
					result = ResultCode.ShengWang_Not_Enough_Error;
				}
				else
				{
					this.installJunXianBuff(player);
					result = ResultCode.Success;
				}
			}
			return result;
		}

		private bool consumeShengWang(GameClient player, int consumeValue)
		{
			int shengWangValue = this.getShengWangValue(player);
			bool result;
			if (this.getShengWangValue(player) < consumeValue)
			{
				result = false;
			}
			else
			{
				this.changeShengWangValue(player, -consumeValue);
				result = true;
			}
			return result;
		}

		private int GetRobotMinAttack(int nOccu, EMagicSwordTowardType eType, PlayerJingJiData data)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? ((int)data.extProps[9]) : ((int)data.extProps[7]));
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = (int)data.extProps[7];
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = (int)data.extProps[9];
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		private int GetRobotMaxAttack(int nOccu, EMagicSwordTowardType eType, PlayerJingJiData data)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? ((int)data.extProps[10]) : ((int)data.extProps[10]));
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = (int)data.extProps[10];
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = (int)data.extProps[10];
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		private int GetRobotAttackType(int nOccu, EMagicSwordTowardType eType)
		{
			int result;
			switch (eType)
			{
			case EMagicSwordTowardType.EMST_Not:
				result = ((nOccu == 1) ? 1 : 0);
				break;
			case EMagicSwordTowardType.EMST_Strength:
				result = 0;
				break;
			case EMagicSwordTowardType.EMST_Intelligence:
				result = 1;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		private bool isHasJunXianBuff(GameClient player)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(player, 87);
			return bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L);
		}

		private int getJunxianBuffId(GameClient player)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("JunXianBufferGoodsIDs", ',');
			int junxian = this.getJunxian(player);
			return paramValueIntArrayByName[junxian];
		}

		private void installJunXianBuff(GameClient player)
		{
			int num = this.getJunxian(player) - 1;
			int num2 = -1;
			BufferData bufferDataByID = Global.GetBufferDataByID(player, 87);
			if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				num2 = (int)bufferDataByID.BufferVal;
			}
			if (num2 != num)
			{
				double[] array = new double[]
				{
					(double)num
				};
				if (array[0] < 1.0 && player.CodeRevision < 1)
				{
					array[0] = 1.0;
				}
				Global.UpdateBufferData(player, BufferItemTypes.MU_JINGJICHANG_JUNXIAN, array, 0, true);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, player);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, player, true, false, 7);
			}
		}

		public void GetNextRewardTime(GameClient player)
		{
			lock (player)
			{
				long[] array = Global.sendToDB<long[], byte[]>(10148, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == array)
				{
					player.ClientData.JingJiNextRewardTime = -1L;
				}
				else
				{
					long num = array[1];
					if (num < 1L)
					{
						player.ClientData.JingJiNextRewardTime = -1L;
					}
					else
					{
						player.ClientData.JingJiNextRewardTime = num;
					}
				}
			}
		}

		public bool CanGetrankingReward(GameClient player)
		{
			if (-1L == player.ClientData.JingJiNextRewardTime)
			{
				this.GetNextRewardTime(player);
			}
			return TimeUtil.NOW() >= player.ClientData.JingJiNextRewardTime;
		}

		public void rankingReward(GameClient player, out int result, out long nextRewardTime)
		{
			result = this.check(player);
			nextRewardTime = 0L;
			if (result == ResultCode.Success)
			{
				int value;
				int num3;
				string text;
				lock (player)
				{
					long[] array = Global.sendToDB<long[], byte[]>(10148, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
					int num = (int)array[0];
					long num2 = array[1];
					if (num == -2)
					{
						result = ResultCode.Illegal;
						return;
					}
					if (TimeUtil.NOW() < num2)
					{
						result = ResultCode.RankingReward_CD_Error;
						return;
					}
					this.getRankingRewardValue(player, num, out value, out num3, out text);
					num2 = TimeUtil.NOW() + JingJiChangConstants.RankingReward_CD_Time;
					nextRewardTime = num2;
					Global.sendToDB<int, byte[]>(10149, DataHelper.ObjectToBytes<long[]>(new long[]
					{
						(long)player.ClientData.RoleID,
						num2
					}), player.ServerId);
					this.GetNextRewardTime(player);
				}
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)num3, true, true, false, "none");
				this.addGoods(player, text);
				this.changeShengWangValue(player, value);
				player.ClientData.AddAwardRecord(RoleAwardMsg.JingJiChang, text, false);
				GameManager.ClientMgr.NotifyGetAwardMsg(player, RoleAwardMsg.JingJiChang, "");
				player._IconStateMgr.CheckJingJiChangJiangLi(player);
				player._IconStateMgr.SendIconStateToClient(player);
			}
		}

		private void addGoods(GameClient player, string goodsInfos)
		{
			string[] array = goodsInfos.Split(new char[]
			{
				'|'
			});
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				int goodsID = Convert.ToInt32(array3[0]);
				int goodsNum = Convert.ToInt32(array3[1]);
				int binding = Convert.ToInt32(array3[2]);
				int forgeLevel = Convert.ToInt32(array3[3]);
				int nAppendPropLev = Convert.ToInt32(array3[4]);
				int lucky = Convert.ToInt32(array3[5]);
				int excellenceProperty = Convert.ToInt32(array3[6]);
				Global.AddGoodsDBCommand(TCPOutPacketPool.getInstance(), player, goodsID, goodsNum, 0, "", forgeLevel, binding, 0, "", false, 1, "竞技场排行榜奖励", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceProperty, nAppendPropLev, 0, null, null, 0, true);
			}
		}

		private void changeShengWangValue(GameClient player, int value)
		{
			GameManager.ClientMgr.ModifyShengWangValue(player, value, "竞技场", true, true);
		}

		private int getShengWangValue(GameClient player)
		{
			return GameManager.ClientMgr.GetShengWangValue(player);
		}

		private void modifyJunxian(GameClient player)
		{
			GameManager.ClientMgr.ModifyShengWangLevelValue(player, 1, "改变军衔", true, true);
		}

		private int getJunxian(GameClient player)
		{
			int shengWangLevelValue = GameManager.ClientMgr.GetShengWangLevelValue(player);
			return (shengWangLevelValue < 0) ? 0 : shengWangLevelValue;
		}

		private void getRankingRewardValue(GameClient player, int ranking, out int addShengWangValue, out int addExpValue, out string goodsInfos)
		{
			addShengWangValue = -1;
			addExpValue = -1;
			goodsInfos = null;
			foreach (SystemXmlItem systemXmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
			{
				if (ranking == -1 && systemXmlItem.GetStringValue("MaxRank").Equals(""))
				{
					addShengWangValue = systemXmlItem.GetIntValue("ShengWang2", -1);
					addExpValue = systemXmlItem.GetIntValue("ExpCoefficient2", -1);
					goodsInfos = systemXmlItem.GetStringValue("GoodsID");
					break;
				}
				if (ranking >= systemXmlItem.GetIntValue("MinRank", -1) && ranking <= systemXmlItem.GetIntValue("MaxRank", -1))
				{
					addShengWangValue = systemXmlItem.GetIntValue("ShengWang2", -1);
					addExpValue = systemXmlItem.GetIntValue("ExpCoefficient2", -1);
					goodsInfos = systemXmlItem.GetStringValue("GoodsID");
					break;
				}
			}
		}

		public bool isInJingJiFuBen(GameClient player)
		{
			return player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1);
		}

		private int check(GameClient player)
		{
			int num = ResultCode.Success;
			int result;
			if ((player.ClientData.Level < this.jingjiFuBenItem.GetIntValue("MinLevel", -1) && player.ClientData.ChangeLifeCount == this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1)) || (player.ClientData.IsFlashPlayer == 1 && player.ClientData.MapCode == 6090))
			{
				num = ResultCode.Illegal;
				result = num;
			}
			else if (player.ClientData.CurrentLifeV <= 0 || player.ClientData.CurrentAction == 12)
			{
				num = ResultCode.Dead_Error;
				result = num;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public int removeCD(GameClient player)
		{
			int num = this.check(player);
			int result;
			if (num != ResultCode.Success)
			{
				result = num;
			}
			else
			{
				PlayerJingJiData playerJingJiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
				if (null == playerJingJiData.baseProps)
				{
					result = ResultCode.Illegal;
				}
				else
				{
					long num2 = (playerJingJiData.nextChallengeTime - TimeUtil.NOW()) / 1000L;
					if (num2 <= 0L)
					{
						result = ResultCode.Success;
					}
					else
					{
						int num3 = (int)Math.Ceiling((double)num2 * GameManager.systemParamsList.GetParamValueDoubleByName("CDXiaoHaoZhuanShi", 0.0));
						if (num3 > 0)
						{
							if (player.ClientData.UserMoney < num3)
							{
								return ResultCode.Money_Not_Enough_Error;
							}
							if (!GameManager.ClientMgr.SubUserMoney(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().tcpClientPool, TCPOutPacketPool.getInstance(), player, num3, "竞技场消除CD", true, true, false, DaiBiSySType.None))
							{
								return ResultCode.Pay_Error;
							}
						}
						Global.sendToDB<bool, byte[]>(10147, DataHelper.ObjectToBytes<int>(player.ClientData.RoleID), player.ServerId);
						num = ResultCode.Success;
						result = num;
					}
				}
			}
			return result;
		}

		public PlayerJingJiData createJingJiData(GameClient player)
		{
			PlayerJingJiData playerJingJiData = new PlayerJingJiData();
			playerJingJiData.roleId = player.ClientData.RoleID;
			playerJingJiData.roleName = Global.FormatRoleName4(player);
			playerJingJiData.name = player.ClientData.RoleName;
			playerJingJiData.zoneId = player.ClientData.ZoneID;
			playerJingJiData.level = player.ClientData.Level;
			playerJingJiData.changeLiveCount = player.ClientData.ChangeLifeCount;
			playerJingJiData.occupationId = player.ClientData.Occupation;
			playerJingJiData.SubOccupation = player.ClientData.SubOccupation;
			playerJingJiData.OccupationList = player.ClientData.OccupationList;
			playerJingJiData.nextChallengeTime = 0L;
			playerJingJiData.nextRewardTime = TimeUtil.NOW() + JingJiChangConstants.RankingReward_CD_Time;
			playerJingJiData.combatForce = player.ClientData.CombatForce;
			playerJingJiData.equipDatas = this.getSaveEquipData(player);
			playerJingJiData.skillDatas = this.getSaveSkillData(player);
			playerJingJiData.baseProps = this.getBaseProps(player);
			playerJingJiData.extProps = this.getExtProps(player);
			playerJingJiData.sex = player.ClientData.RoleSex;
			playerJingJiData.wingData = null;
			if (player.ClientData.MyWingData != null && player.ClientData.MyWingData.WingID > 0)
			{
				playerJingJiData.wingData = player.ClientData.MyWingData;
			}
			playerJingJiData.settingFlags = Global.GetRoleParamsInt64FromDB(player, "SettingBitFlags");
			playerJingJiData.JunTuanId = player.ClientData.JunTuanId;
			playerJingJiData.JunTuanName = player.ClientData.JunTuanName;
			playerJingJiData.JunTuanZhiWu = player.ClientData.JunTuanZhiWu;
			playerJingJiData.LingDi = player.ClientData.LingDi;
			playerJingJiData.ShenShiEquipData = null;
			FuWenTabData roleFuWenTabData = ShenShiManager.getInstance().GetRoleFuWenTabData(player);
			if (null != roleFuWenTabData)
			{
				playerJingJiData.ShenShiEquipData = new SkillEquipData
				{
					SkillEquip = roleFuWenTabData.SkillEquip,
					ShenShiActiveList = roleFuWenTabData.ShenShiActiveList
				};
			}
			playerJingJiData.PassiveEffectList = player.ClientData.PassiveEffectList;
			playerJingJiData.CompType = player.ClientData.CompType;
			playerJingJiData.CompZhiWu = player.ClientData.CompZhiWu;
			return playerJingJiData;
		}

		public void onPlayerLevelup(GameClient player)
		{
			int num;
			int num2;
			if (GameManager.MagicSwordMgr.IsMagicSword(player))
			{
				num = MagicSwordData.InitLevel;
				num2 = MagicSwordData.InitChangeLifeCount;
			}
			else
			{
				num = this.jingjiFuBenItem.GetIntValue("MinLevel", -1);
				num2 = this.jingjiFuBenItem.GetIntValue("MinZhuanSheng", -1);
			}
			if (player.ClientData.Level == num && player.ClientData.ChangeLifeCount == num2 && (player.ClientData.IsFlashPlayer != 1 || player.ClientData.MapCode != 6090))
			{
				PlayerJingJiData playerJingJiData = this.createJingJiData(player);
				Global.sendToDB<byte, byte[]>(10142, DataHelper.ObjectToBytes<PlayerJingJiData>(playerJingJiData), player.ServerId);
				if (GameManager.ClientMgr.GetShengWangValue(player) <= 0)
				{
					GameManager.ClientMgr.SaveShengWangValue(player, 0, true);
					GameManager.ClientMgr.NotifySelfParamsValueChange(player, RoleCommonUseIntParamsIndexs.ShengWang, 0);
				}
				if (GameManager.ClientMgr.GetShengWangLevelValue(player) != -1)
				{
					GameManager.ClientMgr.ModifyShengWangLevelValue(player, 0, "初始化军衔二", true, true);
					Global.BroadcastClientMUShengWang(player, this.getJunxian(player));
				}
			}
		}

		private List<PlayerJingJiSkillData> getSaveSkillData(GameClient client)
		{
			List<PlayerJingJiSkillData> list = new List<PlayerJingJiSkillData>();
			List<SkillData> skillDataList = client.ClientData.SkillDataList;
			List<PlayerJingJiSkillData> result;
			if (skillDataList == null || skillDataList.Count == 0)
			{
				result = list;
			}
			else
			{
				int nOccupation = Global.CalcOriginalOccupationID(client);
				EMagicSwordTowardType magicSwordTypeByWeapon = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(client.ClientData.Occupation, client.UsingEquipMgr.GetWeaponEquipList(), client);
				int[] jingJiChangeSkillList = JingJiChangConstants.GetJingJiChangeSkillList(client, Global.CalcOriginalOccupationID(nOccupation), magicSwordTypeByWeapon);
				foreach (int num in jingJiChangeSkillList)
				{
					foreach (SkillData skillData in skillDataList)
					{
						if (num == skillData.SkillID)
						{
							PlayerJingJiSkillData playerJingJiSkillData = new PlayerJingJiSkillData();
							playerJingJiSkillData.skillID = skillData.SkillID;
							playerJingJiSkillData.skillLevel = skillData.SkillLevel;
							playerJingJiSkillData.skillLevel += TalentManager.GetSkillLevel(client, playerJingJiSkillData.skillID);
							playerJingJiSkillData.skillLevel = Math.Min(playerJingJiSkillData.skillLevel, Global.MaxSkillLevel);
							playerJingJiSkillData.skillLevel = Global.GMax(0, playerJingJiSkillData.skillLevel);
							list.Add(playerJingJiSkillData);
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}

		private double[] getBaseProps(GameClient player)
		{
			return new double[]
			{
				RoleAlgorithm.GetStrength(player, true),
				RoleAlgorithm.GetIntelligence(player, true),
				RoleAlgorithm.GetDexterity(player, true),
				RoleAlgorithm.GetConstitution(player, true)
			};
		}

		private double[] getExtProps(GameClient player)
		{
			double[] array = new double[177];
			array[0] = RoleAlgorithm.GetStrong(player);
			array[1] = RoleAlgorithm.GetAttackSpeed(player);
			array[2] = RoleAlgorithm.GetMoveSpeed(player);
			array[3] = RoleAlgorithm.GetMinADefenseV(player);
			array[4] = RoleAlgorithm.GetMaxADefenseV(player);
			array[5] = RoleAlgorithm.GetMinMDefenseV(player);
			array[6] = RoleAlgorithm.GetMaxMDefenseV(player);
			array[7] = RoleAlgorithm.GetMinAttackV(player);
			array[8] = RoleAlgorithm.GetMaxAttackV(player);
			array[9] = RoleAlgorithm.GetMinMagicAttackV(player);
			array[10] = RoleAlgorithm.GetMaxMagicAttackV(player);
			array[11] = player.RoleBuffer.GetExtProp(11);
			array[12] = player.RoleBuffer.GetExtProp(12);
			array[13] = RoleAlgorithm.GetMaxLifeV(player);
			array[14] = RoleAlgorithm.GetMaxLifePercentV(player);
			array[15] = RoleAlgorithm.GetMaxMagicV(player);
			array[16] = RoleAlgorithm.GetMaxMagicPercent(player);
			array[17] = RoleAlgorithm.GetLuckV(player);
			array[18] = RoleAlgorithm.GetHitV(player);
			array[19] = RoleAlgorithm.GetDodgeV(player);
			array[20] = RoleAlgorithm.GetLifeRecoverAddPercentV(player);
			array[21] = RoleAlgorithm.GetMagicRecoverAddPercentV(player);
			array[22] = RoleAlgorithm.GetLifeRecoverValPercentV(player);
			array[23] = RoleAlgorithm.GetMagicRecoverValPercentV(player);
			array[24] = RoleAlgorithm.GetSubAttackInjurePercent(player);
			array[25] = RoleAlgorithm.GetSubAttackInjureValue(player);
			array[26] = RoleAlgorithm.GetAddAttackInjurePercent(player);
			array[27] = RoleAlgorithm.GetAddAttackInjureValue(player);
			array[28] = RoleAlgorithm.GetIgnoreDefensePercent(player);
			array[29] = RoleAlgorithm.GetDamageThornPercent(player);
			array[30] = RoleAlgorithm.GetDamageThorn(player);
			array[31] = RoleAlgorithm.GetPhySkillIncrease(player);
			array[32] = 0.0;
			array[33] = RoleAlgorithm.GetMagicSkillIncrease(player);
			array[34] = 0.0;
			array[35] = RoleAlgorithm.GetFatalAttack(player);
			array[36] = RoleAlgorithm.GetDoubleAttack(player);
			array[37] = RoleAlgorithm.GetDecreaseInjurePercent(player);
			array[38] = RoleAlgorithm.GetDecreaseInjureValue(player);
			array[39] = RoleAlgorithm.GetCounteractInjurePercent(player);
			array[40] = RoleAlgorithm.GetCounteractInjureValue(player);
			array[41] = RoleAlgorithm.GetIgnoreDefenseRate(player);
			array[42] = player.RoleBuffer.GetExtProp(42);
			array[43] = player.RoleBuffer.GetExtProp(43);
			array[44] = RoleAlgorithm.GetLifeStealV(player);
			array[51] = RoleAlgorithm.GetDeLuckyAttack(player);
			array[52] = RoleAlgorithm.GetDeFatalAttack(player);
			array[53] = RoleAlgorithm.GetDeDoubleAttack(player);
			array[56] = RoleAlgorithm.GetFrozenPercent(player);
			array[57] = RoleAlgorithm.GetPalsyPercent(player);
			array[58] = RoleAlgorithm.GetSpeedDownPercent(player);
			array[59] = RoleAlgorithm.GetBlowPercent(player);
			array[61] = RoleAlgorithm.GetSavagePercent(player);
			array[62] = RoleAlgorithm.GetColdPercent(player);
			array[63] = RoleAlgorithm.GetRuthlessPercent(player);
			array[64] = RoleAlgorithm.GetDeSavagePercent(player);
			array[65] = RoleAlgorithm.GetDeColdPercent(player);
			array[66] = RoleAlgorithm.GetDeRuthlessPercent(player);
			array[97] = RoleAlgorithm.GetExtProp(player, 97);
			array[98] = RoleAlgorithm.GetExtProp(player, 98);
			array[99] = RoleAlgorithm.GetExtProp(player, 99);
			array[100] = RoleAlgorithm.GetExtProp(player, 100);
			array[118] = RoleAlgorithm.GetExtProp(player, 118);
			for (int i = 119; i < 177; i++)
			{
				array[i] = RoleAlgorithm.GetExtProp(player, i);
			}
			return array;
		}

		private List<PlayerJingJiEquipData> getSaveEquipData(GameClient client)
		{
			List<PlayerJingJiEquipData> list = new List<PlayerJingJiEquipData>();
			List<GoodsData> goodsDataList = client.ClientData.GoodsDataList;
			if (null != goodsDataList)
			{
				foreach (GoodsData goodsData in goodsDataList)
				{
					if (this.canSaveEquip(goodsData))
					{
						list.Add(new PlayerJingJiEquipData
						{
							EquipId = goodsData.GoodsID,
							ExcellenceInfo = goodsData.ExcellenceInfo,
							Forge_level = goodsData.Forge_level,
							BagIndex = goodsData.BagIndex
						});
					}
				}
			}
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						GoodsData goodsData2 = client.ClientData.DamonGoodsDataList[i];
						if (goodsData2.GCount > 0 && 0 != goodsData2.Using)
						{
							list.Add(new PlayerJingJiEquipData
							{
								EquipId = goodsData2.GoodsID,
								ExcellenceInfo = goodsData2.ExcellenceInfo,
								Forge_level = goodsData2.Forge_level
							});
						}
					}
				}
			}
			if (null != client.ClientData.FashionGoodsDataList)
			{
				lock (client.ClientData.FashionGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.FashionGoodsDataList.Count; i++)
					{
						if (client.ClientData.FashionGoodsDataList[i].GCount > 0 && client.ClientData.FashionGoodsDataList[i].Using != 0 && client.ClientData.FashionGoodsDataList[i].Site == 6000)
						{
							GoodsData goodsData3 = client.ClientData.FashionGoodsDataList[i];
							list.Add(new PlayerJingJiEquipData
							{
								EquipId = goodsData3.GoodsID,
								ExcellenceInfo = goodsData3.ExcellenceInfo,
								Forge_level = goodsData3.Forge_level
							});
						}
					}
				}
			}
			return list;
		}

		private bool canSaveEquip(GoodsData equip)
		{
			bool result;
			if (equip.Site != 0 && equip.Site != 5000)
			{
				result = false;
			}
			else if (equip.Using > 0)
			{
				int goodsCatetoriy = Global.GetGoodsCatetoriy(equip.GoodsID);
				result = (goodsCatetoriy >= 0 && goodsCatetoriy < 49 && goodsCatetoriy != 5 && goodsCatetoriy != 6 && goodsCatetoriy != 22 && goodsCatetoriy != 23);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsJingJiChangMap(int nMapCode)
		{
			return nMapCode == this.nJingJiChangMapCode;
		}

		public int requestChallenge(GameClient player, int beChallengerId, int beChallengerRanking, int enterType)
		{
			int num = this.check(player);
			int result;
			if (num != ResultCode.Success)
			{
				result = num;
			}
			else if (this.IsJingJiChangMap(player.ClientData.MapCode))
			{
				num = ResultCode.Challenge_CD_Error;
				result = num;
			}
			else
			{
				JingJiBeChallengeData jingJiBeChallengeData = Global.sendToDB<JingJiBeChallengeData, byte[]>(10143, DataHelper.ObjectToBytes<int[]>(new int[]
				{
					player.ClientData.RoleID,
					beChallengerId,
					beChallengerRanking
				}), player.ServerId);
				num = jingJiBeChallengeData.state;
				if (num == -1)
				{
					int num2 = (int)GameManager.systemParamsList.GetParamValueDoubleByName("VIPJingJiCD", 0.0);
					if (num2 > 0 && player.ClientData.VipLevel >= num2)
					{
						num = ResultCode.Success;
					}
				}
				if (num != ResultCode.Success)
				{
					switch (num)
					{
					case -5:
						num = ResultCode.CantChallenger;
						break;
					case -4:
						num = ResultCode.BeChallenger_Lock_Error;
						break;
					case -3:
						num = ResultCode.BeChallenger_Ranking_Change_Error;
						break;
					case -2:
						num = ResultCode.BeChallenger_Null_Error;
						break;
					case -1:
						num = ResultCode.Challenge_CD_Error;
						break;
					default:
						num = ResultCode.Illegal;
						break;
					}
					result = num;
				}
				else
				{
					num = this.checkEnterNum(player, enterType);
					if (num != ResultCode.Success)
					{
						result = num;
					}
					else
					{
						num = this.enterJingJiChang(player, jingJiBeChallengeData.beChallengerData, JingJiFuBenType.NORMAL);
						result = num;
					}
				}
			}
			return result;
		}

		public bool checkAction(GameClient player)
		{
			bool result;
			if (player.ClientData.StallDataItem != null)
			{
				result = false;
			}
			else
			{
				int currentAction = player.ClientData.CurrentAction;
				switch (currentAction)
				{
				case 3:
				case 4:
				case 6:
				case 9:
					break;
				case 5:
				case 7:
				case 8:
					goto IL_52;
				default:
					if (currentAction != 24)
					{
						goto IL_52;
					}
					break;
				}
				return false;
				IL_52:
				result = true;
			}
			return result;
		}

		public int checkEnterNum(GameClient player, int enterType)
		{
			int result = ResultCode.Success;
			int intValue = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			FuBenData fuBenData = Global.GetFuBenData(player, this.jingjiFuBenId);
			int num;
			int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			if (enterType == JingJiChangConstants.Enter_Type_Free)
			{
				if (fuBenEnterNum >= intValue)
				{
					return ResultCode.FreeNum_Error;
				}
			}
			else if (enterType == JingJiChangConstants.Enter_Type_Vip)
			{
				if (fuBenEnterNum >= intValue)
				{
					int num2 = fuBenEnterNum - intValue;
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
					int vipLevel = player.ClientData.VipLevel;
					int num3 = paramValueIntArrayByName[vipLevel];
					if (num3 <= num2)
					{
						return ResultCode.VipNum_Error;
					}
					int num4 = (int)GameManager.systemParamsList.GetParamValueIntByName("VIPGouMaiJingJi", -1);
					if (player.ClientData.UserMoney < num4)
					{
						return ResultCode.Money_Not_Enough_Error;
					}
					if (!GameManager.ClientMgr.SubUserMoney(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().tcpClientPool, TCPOutPacketPool.getInstance(), player, num4, "竞技场额外进入", true, true, false, DaiBiSySType.None))
					{
						result = ResultCode.Pay_Error;
					}
					else
					{
						result = ResultCode.Success;
					}
				}
			}
			return result;
		}

		public int JingJiChangStartFight(GameClient client)
		{
			int result;
			if (this.IsHaveFuBen(client.ClientData.FuBenSeqID))
			{
				JingJiChangInstance jingJiChangInstance = null;
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.TryGetValue(client.ClientData.FuBenSeqID, out jingJiChangInstance);
				}
				if (null == jingJiChangInstance)
				{
					result = -1;
				}
				else
				{
					if (jingJiChangInstance.getState() == JingJiFuBenState.INITIALIZED)
					{
						jingJiChangInstance.ResetJingJiTime();
						jingJiChangInstance.switchState(JingJiFuBenState.WAITING_CHANGEMAP_FINISH);
					}
					result = 0;
				}
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public int enterJingJiChang(GameClient client, PlayerJingJiData beChallengerData, JingJiFuBenType type = JingJiFuBenType.NORMAL)
		{
			ProcessTask.ProcessAddTaskVal(client, TaskTypes.JingJiChang, -1, 1, new object[0]);
			GameMap gameMap = null;
			int intValue = this.jingjiFuBenItem.GetIntValue("MapCode", -1);
			int result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(intValue, out gameMap))
			{
				result = ResultCode.Map_Error;
			}
			else
			{
				string[] array = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
				if (array == null || array.Length < 2)
				{
					result = ResultCode.FubenSeqId_Error;
				}
				else
				{
					int num = Global.SafeConvertToInt32(array[1]);
					client.ClientData.FuBenSeqID = num;
					FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, client.ClientData.FuBenSeqID, 0, this.jingjiFuBenId);
					Robot robot = this.createRobot(client, beChallengerData, this.jingjiFuBenItem.GetIntValue("MapCode", -1));
					JingJiChangInstance jingJiChangInstance = new JingJiChangInstance(client, robot, num, type);
					lock (this.jingjichangInstances)
					{
						this.jingjichangInstances.Add(jingJiChangInstance.getFuBenSeqId(), jingJiChangInstance);
					}
					ScheduleExecutor2.Instance.scheduleExecute(jingJiChangInstance, 0, 100);
					GameManager.ClientMgr.UserFullLife(client, "进入竞技场", false);
					GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), client, -1, intValue, gameMap.DefaultBirthPosX, gameMap.DefaultBirthPosY, client.ClientData.RoleDirection, 123);
					if (JingJiFuBenType.NORMAL == type)
					{
						Global.UpdateFuBenData(client, this.jingjiFuBenId, 1, 0);
						client._IconStateMgr.CheckJingJiChangLeftTimes(client);
						client._IconStateMgr.SendIconStateToClient(client);
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JoinJingJiChangTimes));
					}
					result = ResultCode.Success;
				}
			}
			return result;
		}

		public void createSkillIDs(List<PlayerJingJiSkillData> skillDatas, Robot robot)
		{
			if (skillDatas != null && skillDatas.Count != 0)
			{
				int[] array = new int[skillDatas.Count];
				for (int i = 0; i < skillDatas.Count; i++)
				{
					array[i] = skillDatas[i].skillID;
					int num;
					if (!robot.skillInfos.TryGetValue(skillDatas[i].skillID, out num) || skillDatas[i].skillLevel > num)
					{
						robot.skillInfos[skillDatas[i].skillID] = skillDatas[i].skillLevel;
					}
				}
				robot.MonsterInfo.SkillIDs = robot.skillInfos.Keys.ToArray<int>();
			}
		}

		private double GetRobotExtProps(int nIndex, double[] extProps)
		{
			double result;
			if (nIndex > extProps.Length - 1)
			{
				result = 0.0;
			}
			else
			{
				result = extProps[nIndex];
			}
			return result;
		}

		public Robot createRobot(GameClient player, PlayerJingJiData beChallengerData, int mapCode)
		{
			int num = (int)GameManager.MonsterIDMgr.GetNewID(mapCode);
			RoleDataMini roleDataMini = this.createRoleDataMini(num, beChallengerData, mapCode);
			Robot robot = new Robot(player, roleDataMini);
			robot.Lucky = (int)beChallengerData.extProps[17];
			robot.DoubleValue = (int)beChallengerData.extProps[36];
			robot.FatalValue = (int)beChallengerData.extProps[35];
			robot.DeLucky = this.GetRobotExtProps(51, beChallengerData.extProps);
			robot.DeDoubleValue = this.GetRobotExtProps(53, beChallengerData.extProps);
			robot.DeFatalValue = this.GetRobotExtProps(52, beChallengerData.extProps);
			robot.SavageValue = this.GetRobotExtProps(61, beChallengerData.extProps);
			robot.ColdValue = this.GetRobotExtProps(62, beChallengerData.extProps);
			robot.RuthlessValue = this.GetRobotExtProps(63, beChallengerData.extProps);
			robot.DeSavageValue = this.GetRobotExtProps(64, beChallengerData.extProps);
			robot.DeColdValue = this.GetRobotExtProps(65, beChallengerData.extProps);
			robot.DeRuthlessValue = this.GetRobotExtProps(66, beChallengerData.extProps);
			robot.FireAttack = (int)this.GetRobotExtProps(69, beChallengerData.extProps);
			robot.WaterAttack = (int)this.GetRobotExtProps(70, beChallengerData.extProps);
			robot.LightningAttack = (int)this.GetRobotExtProps(71, beChallengerData.extProps);
			robot.SoilAttack = (int)this.GetRobotExtProps(72, beChallengerData.extProps);
			robot.IceAttack = (int)this.GetRobotExtProps(73, beChallengerData.extProps);
			robot.WindAttack = (int)this.GetRobotExtProps(74, beChallengerData.extProps);
			robot.FirePenetration = (double)((int)this.GetRobotExtProps(75, beChallengerData.extProps));
			robot.WaterPenetration = (double)((int)this.GetRobotExtProps(76, beChallengerData.extProps));
			robot.LightningPenetration = (double)((int)this.GetRobotExtProps(77, beChallengerData.extProps));
			robot.SoilPenetration = (double)((int)this.GetRobotExtProps(78, beChallengerData.extProps));
			robot.IcePenetration = (double)((int)this.GetRobotExtProps(79, beChallengerData.extProps));
			robot.WindPenetration = (double)((int)this.GetRobotExtProps(80, beChallengerData.extProps));
			robot.ElementPenetration = (double)((int)this.GetRobotExtProps(118, beChallengerData.extProps));
			robot.DeFirePenetration = (double)((int)this.GetRobotExtProps(81, beChallengerData.extProps));
			robot.DeWaterPenetration = (double)((int)this.GetRobotExtProps(82, beChallengerData.extProps));
			robot.DeLightningPenetration = (double)((int)this.GetRobotExtProps(83, beChallengerData.extProps));
			robot.DeSoilPenetration = (double)((int)this.GetRobotExtProps(84, beChallengerData.extProps));
			robot.DeIcePenetration = (double)((int)this.GetRobotExtProps(85, beChallengerData.extProps));
			robot.DeWindPenetration = (double)((int)this.GetRobotExtProps(86, beChallengerData.extProps));
			this.createSkillIDs(beChallengerData.skillDatas, robot);
			robot.RoleID = num;
			robot.UniqueID = Global.GetUniqueID();
			robot.PlayerId = beChallengerData.roleId;
			robot.Name = string.Format("Role_{0}", robot.RoleID);
			robot.MonsterInfo.VSName = beChallengerData.roleName;
			robot.MonsterInfo.SpriteSpeedTickList = new int[]
			{
				148,
				222,
				0,
				222,
				222,
				0,
				185,
				0,
				0,
				0,
				0,
				100,
				148
			};
			robot.MonsterInfo.EachActionFrameRange = new int[]
			{
				3,
				3,
				0,
				3,
				3,
				0,
				3,
				0,
				0,
				0,
				0,
				1,
				3
			};
			robot.MonsterInfo.EffectiveFrame = new int[]
			{
				-1,
				-1,
				-1,
				1,
				1,
				0,
				1,
				-1,
				-1,
				-1,
				-1,
				-1,
				-1
			};
			robot.Sex = beChallengerData.sex;
			robot.VLife = beChallengerData.extProps[13];
			robot.VMana = beChallengerData.extProps[15];
			robot.MonsterInfo.VLifeMax = beChallengerData.extProps[13];
			robot.MonsterInfo.VManaMax = beChallengerData.extProps[15];
			robot.MoveSpeed = beChallengerData.extProps[2];
			int num2 = (beChallengerData.occupationId - beChallengerData.changeLiveCount > 10) ? ((beChallengerData.occupationId - beChallengerData.changeLiveCount) / 10 - 1) : beChallengerData.occupationId;
			EMagicSwordTowardType magicSwordTypeByWeapon = GameManager.MagicSwordMgr.GetMagicSwordTypeByWeapon(num2, robot.getRoleDataMini().GoodsDataList, null);
			robot.MonsterInfo.MinAttack = this.GetRobotMinAttack(num2, magicSwordTypeByWeapon, beChallengerData);
			robot.MonsterInfo.MaxAttack = this.GetRobotMaxAttack(num2, magicSwordTypeByWeapon, beChallengerData);
			robot.MonsterInfo.Defense = (int)beChallengerData.extProps[4];
			robot.MonsterInfo.MDefense = (int)beChallengerData.extProps[6];
			robot.MonsterInfo.HitV = beChallengerData.extProps[18];
			robot.MonsterInfo.Dodge = beChallengerData.extProps[19];
			robot.MonsterInfo.RecoverLifeV = beChallengerData.extProps[20];
			robot.MonsterInfo.RecoverMagicV = beChallengerData.extProps[21];
			robot.MonsterInfo.ExtProps = new double[177];
			for (int i = 0; i < beChallengerData.extProps.Length; i++)
			{
				robot.MonsterInfo.ExtProps[i] = beChallengerData.extProps[i];
				robot.DynamicData.ExtProps[i] = beChallengerData.extProps[i];
			}
			robot.MonsterInfo.VLevel = beChallengerData.level;
			robot.MonsterInfo.ChangeLifeCount = beChallengerData.changeLiveCount;
			robot.MonsterInfo.VExperience = 0;
			robot.MonsterInfo.VMoney = 0;
			robot.MonsterInfo.SeekRange = 100;
			robot.MonsterInfo.EquipmentBody = -1;
			robot.MonsterInfo.EquipmentWeapon = -1;
			robot.MonsterInfo.ToOccupation = num2;
			robot.MonsterInfo.FallGoodsPackID = -1;
			robot.MonsterType = 1801;
			robot.MonsterInfo.BattlePersonalJiFen = 0;
			robot.MonsterInfo.BattleZhenYingJiFen = 0;
			robot.MonsterInfo.FallBelongTo = 0;
			robot.MonsterInfo.DaimonSquareJiFen = 0;
			robot.MonsterInfo.BloodCastJiFen = 0;
			robot.MonsterInfo.WolfScore = 0;
			robot.MonsterInfo.AttackType = this.GetRobotAttackType(num2, magicSwordTypeByWeapon);
			robot.Camp = -1;
			robot.PetAiControlType = -1;
			robot.NextSeekEnemyTicks = 500L;
			robot.OwnerClient = null;
			robot.OwnerMonster = null;
			robot.FrozenPercent = this.GetRobotExtProps(56, beChallengerData.extProps);
			robot.PalsyPercent = this.GetRobotExtProps(57, beChallengerData.extProps);
			robot.SpeedDownPercent = this.GetRobotExtProps(58, beChallengerData.extProps);
			robot.BlowPercent = this.GetRobotExtProps(59, beChallengerData.extProps);
			robot.DeFrozenPercent = this.GetRobotExtProps(97, beChallengerData.extProps);
			robot.DePalsyPercent = this.GetRobotExtProps(98, beChallengerData.extProps);
			robot.DeSpeedDownPercent = this.GetRobotExtProps(99, beChallengerData.extProps);
			robot.DeBlowPercent = this.GetRobotExtProps(100, beChallengerData.extProps);
			return robot;
		}

		private RoleDataMini createRoleDataMini(int roleId, PlayerJingJiData data, int mapCode)
		{
			int num = (int)this.GetRobotExtProps(119, data.extProps);
			RoleDataMini roleDataMini = new RoleDataMini
			{
				RoleID = roleId,
				RoleName = data.name,
				ZoneID = data.zoneId,
				RoleSex = data.sex,
				Occupation = data.occupationId,
				SubOccupation = data.SubOccupation,
				OccupationList = data.OccupationList,
				Level = data.level,
				MapCode = mapCode,
				MaxLifeV = (int)data.extProps[13],
				LifeV = (int)data.extProps[13],
				MaxMagicV = (int)data.extProps[15],
				MagicV = (int)data.extProps[15],
				BodyCode = this.FindEquipCode(data.equipDatas, 1),
				WeaponCode = this.FindEquipCode(data.equipDatas, 0),
				GoodsDataList = JingJiChangManager.GetUsingGoodsList(data.equipDatas),
				ChangeLifeLev = data.changeLiveCount,
				ChangeLifeCount = data.changeLiveCount,
				BufferMiniInfo = new List<BufferDataMini>(),
				MyWingData = data.wingData,
				SettingBitFlags = data.settingFlags,
				JunTuanId = data.JunTuanId,
				JunTuanName = data.JunTuanName,
				JunTuanZhiWu = data.JunTuanZhiWu,
				LingDi = data.LingDi,
				HuiJiData = data.HuiJiData,
				ShenShiEquipData = data.ShenShiEquipData,
				PassiveEffectList = data.PassiveEffectList,
				CurrentArmorV = num,
				MaxArmorV = num
			};
			roleDataMini.BodyCode = Global.GMax(roleDataMini.RoleSex, roleDataMini.BodyCode);
			roleDataMini.WeaponCode = Global.GMax(0, roleDataMini.WeaponCode);
			return roleDataMini;
		}

		public static List<GoodsData> GetUsingGoodsList(List<PlayerJingJiEquipData> equipDatas)
		{
			int num = 0;
			int num2 = -1;
			HashSet<int> hashSet = new HashSet<int>();
			List<GoodsData> list = new List<GoodsData>();
			if (null != equipDatas)
			{
				for (int i = 0; i < equipDatas.Count; i++)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(equipDatas[i].EquipId);
					GoodsData goodsData = new GoodsData();
					goodsData.GoodsID = equipDatas[i].EquipId;
					goodsData.ExcellenceInfo = equipDatas[i].ExcellenceInfo;
					goodsData.Forge_level = equipDatas[i].Forge_level;
					goodsData.BagIndex = equipDatas[i].BagIndex;
					goodsData.Using = 1;
					if (goodsCatetoriy >= 11 && goodsCatetoriy <= 21)
					{
						hashSet.Add(goodsData.BagIndex);
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipDatas[i].EquipId, out systemXmlItem))
						{
							num++;
							int intValue = systemXmlItem.GetIntValue("HandType", -1);
							if (2 != intValue)
							{
								num2 = intValue;
							}
							else
							{
								num++;
							}
						}
					}
					list.Add(goodsData);
				}
				if (num >= 3 && hashSet.Count == 1)
				{
					for (int i = 0; i < list.Count; i++)
					{
						int goodsCatetoriy2 = Global.GetGoodsCatetoriy(list[i].GoodsID);
						if (goodsCatetoriy2 >= 11 && goodsCatetoriy2 <= 21)
						{
							SystemXmlItem systemXmlItem = null;
							if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(list[i].GoodsID, out systemXmlItem))
							{
								int intValue = systemXmlItem.GetIntValue("HandType", -1);
								if (2 == intValue)
								{
									if (-1 == num2)
									{
										list[i].BagIndex = 1;
										break;
									}
									list[i].BagIndex = num2;
									break;
								}
							}
						}
					}
				}
			}
			return list;
		}

		private int FindEquipCode(List<PlayerJingJiEquipData> equipDatas, int category)
		{
			int result;
			if (equipDatas == null)
			{
				result = -1;
			}
			else
			{
				lock (equipDatas)
				{
					for (int i = 0; i < equipDatas.Count; i++)
					{
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(equipDatas[i].EquipId, out systemXmlItem))
						{
							if ((category >= 0 && category <= 4) || (category >= 7 && category <= 21))
							{
								return systemXmlItem.GetIntValue("EquipCode", -1);
							}
						}
					}
				}
				result = -1;
			}
			return result;
		}

		public bool IsHaveFuBen(int nFuBenSeqID)
		{
			bool result = false;
			lock (this.jingjichangInstances)
			{
				if (this.jingjichangInstances.ContainsKey(nFuBenSeqID))
				{
					result = true;
				}
			}
			return result;
		}

		public void onChallengeEndForPlayerDead(GameClient player, Monster monster)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1) && monster.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance jingJiChangInstance = null;
				lock (this.jingjichangInstances)
				{
					this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out jingJiChangInstance);
				}
				if (null != jingJiChangInstance)
				{
					lock (jingJiChangInstance)
					{
						if (jingJiChangInstance.getState() != JingJiFuBenState.STOP_CD && jingJiChangInstance.getState() != JingJiFuBenState.STOPED && jingJiChangInstance.getState() != JingJiFuBenState.DESTROYED)
						{
							Robot robot = jingJiChangInstance.getRobot();
							robot.stopAttack();
							this.processFailed(player, robot, new JingJiChallengeResultData
							{
								playerId = player.ClientData.RoleID,
								robotId = robot.PlayerId,
								isWin = false
							}, jingJiChangInstance.type);
							jingJiChangInstance.switchState(JingJiFuBenState.STOP_CD);
						}
					}
				}
			}
		}

		public void onChallengeEndForMonsterDead(GameClient player, Monster monster)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1) && monster.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				if (monster is Robot)
				{
					JingJiChangInstance jingJiChangInstance = null;
					lock (this.jingjichangInstances)
					{
						if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out jingJiChangInstance))
						{
							return;
						}
					}
					if (monster.VLife <= 0.0 && player.ClientData.CurrentLifeV > 0)
					{
						lock (jingJiChangInstance)
						{
							if (jingJiChangInstance.getState() != JingJiFuBenState.STOP_CD && jingJiChangInstance.getState() != JingJiFuBenState.STOPED && jingJiChangInstance.getState() != JingJiFuBenState.DESTROYED)
							{
								Robot robot = jingJiChangInstance.getRobot();
								robot.stopAttack();
								this.processWin(player, robot, new JingJiChallengeResultData
								{
									playerId = player.ClientData.RoleID,
									robotId = robot.PlayerId,
									isWin = true
								}, jingJiChangInstance.type);
								jingJiChangInstance.switchState(JingJiFuBenState.STOP_CD);
							}
						}
					}
					else
					{
						this.onChallengeEndForPlayerDead(player, monster);
					}
				}
			}
		}

		public void onChallengeEndForPlayerLeaveFuBen(GameClient player)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance jingJiChangInstance = null;
				lock (this.jingjichangInstances)
				{
					if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out jingJiChangInstance))
					{
						return;
					}
				}
				lock (jingJiChangInstance)
				{
					if (jingJiChangInstance.getState() != JingJiFuBenState.STOP_CD && jingJiChangInstance.getState() != JingJiFuBenState.STOPED && jingJiChangInstance.getState() != JingJiFuBenState.DESTROYED)
					{
						Robot robot = jingJiChangInstance.getRobot();
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, jingJiChangInstance.type);
						if (player.ClientData.CurrentLifeV <= 0)
						{
							this.relive(player);
						}
						jingJiChangInstance.switchState(JingJiFuBenState.DESTROYED);
					}
				}
			}
		}

		public void onChallengeEndForPlayerLogout(GameClient player)
		{
			if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0 && this.IsHaveFuBen(player.ClientData.FuBenSeqID) && player.ClientData.MapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
			{
				JingJiChangInstance jingJiChangInstance = null;
				lock (this.jingjichangInstances)
				{
					if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out jingJiChangInstance))
					{
						return;
					}
				}
				lock (jingJiChangInstance)
				{
					if (jingJiChangInstance.getState() != JingJiFuBenState.STOP_CD && jingJiChangInstance.getState() != JingJiFuBenState.STOPED && jingJiChangInstance.getState() != JingJiFuBenState.DESTROYED)
					{
						Robot robot = jingJiChangInstance.getRobot();
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, jingJiChangInstance.type);
						if (player.ClientData.CurrentLifeV <= 0)
						{
							this.relive(player);
						}
						jingJiChangInstance.switchState(JingJiFuBenState.DESTROYED);
					}
				}
			}
		}

		private void processWin(GameClient player, Robot robot, JingJiChallengeResultData resultData, JingJiFuBenType type)
		{
			if (JingJiFuBenType.NORMAL != type)
			{
				GlobalEventSource.getInstance().fireEvent(new JingJiChangWinEventObject(player, robot, (int)type));
			}
			else
			{
				int challengeEndRanking = this.getChallengeEndRanking(resultData, player.ServerId);
				int num;
				int num2;
				int num3;
				this.getChallengeReward(player, challengeEndRanking, true, out num, out num2, out num3);
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)num2, true, true, false, "none");
				this.changeShengWangValue(player, num);
				JingJiSaveData jingJiSaveData = new JingJiSaveData();
				jingJiSaveData.isWin = true;
				jingJiSaveData.nextChallengeTime = ((num3 > 0) ? (TimeUtil.NOW() + (long)(num3 * 1000)) : 0L);
				jingJiSaveData.roleId = player.ClientData.RoleID;
				jingJiSaveData.level = player.ClientData.Level;
				jingJiSaveData.changeLiveCount = player.ClientData.ChangeLifeCount;
				jingJiSaveData.combatForce = player.ClientData.CombatForce;
				jingJiSaveData.equipDatas = this.getSaveEquipData(player);
				jingJiSaveData.skillDatas = this.getSaveSkillData(player);
				jingJiSaveData.baseProps = this.getBaseProps(player);
				jingJiSaveData.extProps = this.getExtProps(player);
				jingJiSaveData.robotId = robot.PlayerId;
				jingJiSaveData.wingData = null;
				jingJiSaveData.Occupation = player.ClientData.Occupation;
				jingJiSaveData.SubOccupation = player.ClientData.SubOccupation;
				jingJiSaveData.ShenShiEuipSkill = null;
				if (player.ClientData.MyWingData != null && player.ClientData.MyWingData.WingID > 0)
				{
					jingJiSaveData.wingData = player.ClientData.MyWingData;
				}
				jingJiSaveData.settingFlags = Global.GetRoleParamsInt64FromDB(player, "SettingBitFlags");
				jingJiSaveData.HuiJiData = player.ClientData.HuiJiData;
				FuWenTabData roleFuWenTabData = ShenShiManager.getInstance().GetRoleFuWenTabData(player);
				if (null != roleFuWenTabData)
				{
					jingJiSaveData.ShenShiEuipSkill = new SkillEquipData
					{
						SkillEquip = roleFuWenTabData.SkillEquip,
						ShenShiActiveList = roleFuWenTabData.ShenShiActiveList
					};
				}
				jingJiSaveData.PassiveEffectList = player.PassiveEffectList;
				int num4 = Global.sendToDB<int, byte[]>(10145, DataHelper.ObjectToBytes<JingJiSaveData>(jingJiSaveData), player.ServerId);
				if (num4 > 0)
				{
					this.LianShengGongGao(player, robot.MonsterInfo.VSName, false, num4);
				}
				player.sendCmd(580, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					num,
					num2,
					challengeEndRanking
				}), false);
				SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(player, ESevenDayGoalFuncType.JingJiChangRank);
				sevenDayGoalEventObject.Arg1 = challengeEndRanking;
				GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
				SevenDayGoalEventObject eventObj = SevenDayGoalEvPool.Alloc(player, ESevenDayGoalFuncType.WinJingJiChangTimes);
				GlobalEventSource.getInstance().fireEvent(eventObj);
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(player, OrnamentGoalType.OGT_JingJiChallenge, new int[0]));
				ProcessTask.ProcessAddTaskVal(player, TaskTypes.JingJiChang_Win, -1, 1, new object[0]);
			}
		}

		private void processFailed(GameClient player, Robot robot, JingJiChallengeResultData resultData, JingJiFuBenType type)
		{
			if (JingJiFuBenType.NORMAL != type)
			{
				GlobalEventSource.getInstance().fireEvent(new JingJiChangFailedEventObject(player, robot, (int)type));
			}
			else
			{
				int challengeEndRanking = this.getChallengeEndRanking(resultData, player.ServerId);
				int num;
				int num2;
				int num3;
				this.getChallengeReward(player, challengeEndRanking, false, out num, out num2, out num3);
				GameManager.ClientMgr.ProcessRoleExperience(player, (long)num2, true, true, false, "none");
				this.changeShengWangValue(player, num);
				int num4 = Global.sendToDB<int, byte[]>(10145, DataHelper.ObjectToBytes<JingJiSaveData>(new JingJiSaveData
				{
					isWin = false,
					nextChallengeTime = ((num3 > 0) ? (TimeUtil.NOW() + (long)(num3 * 1000)) : 0L),
					robotId = robot.PlayerId,
					roleId = player.ClientData.RoleID
				}), player.ServerId);
				if (num4 > 0)
				{
					this.LianShengGongGao(player, robot.MonsterInfo.VSName, false, num4);
				}
				player.sendCmd(580, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					0,
					num,
					num2,
					challengeEndRanking
				}), false);
			}
		}

		private void LianShengGongGao(GameClient player, string robotName, bool isWin, int winCount)
		{
		}

		private void getChallengeReward(GameClient player, int ranking, bool isWin, out int addShengWangValue, out int addExpValue, out int challengeCD)
		{
			addShengWangValue = -1;
			addExpValue = -1;
			challengeCD = -1;
			foreach (SystemXmlItem systemXmlItem in this.jingjiMainConfig.SystemXmlItemDict.Values)
			{
				int changeLifeCount = player.ClientData.ChangeLifeCount;
				double num;
				if (changeLifeCount == 0)
				{
					num = 1.0;
				}
				else
				{
					num = Data.ChangeLifeEverydayExpRate[changeLifeCount];
				}
				addExpValue = (int)((double)systemXmlItem.GetIntValue("ExpCoefficient1", -1) * num);
				if (ranking == -1 && systemXmlItem.GetStringValue("MaxRank").Equals(""))
				{
					if (isWin)
					{
						addShengWangValue = systemXmlItem.GetIntValue("ShengWang1", -1);
						challengeCD = systemXmlItem.GetIntValue("CD", -1);
					}
					else
					{
						addShengWangValue = systemXmlItem.GetIntValue("ShengWang1", -1) / 2;
						addExpValue /= 2;
						challengeCD = systemXmlItem.GetIntValue("CD", -1);
					}
					break;
				}
				if (ranking >= systemXmlItem.GetIntValue("MinRank", -1) && ranking <= systemXmlItem.GetIntValue("MaxRank", -1))
				{
					if (isWin)
					{
						addShengWangValue = systemXmlItem.GetIntValue("ShengWang1", -1);
						challengeCD = systemXmlItem.GetIntValue("CD", -1);
					}
					else
					{
						addShengWangValue = systemXmlItem.GetIntValue("ShengWang1", -1) / 2;
						addExpValue /= 2;
						challengeCD = systemXmlItem.GetIntValue("CD", -1);
					}
					break;
				}
			}
			int num2 = (int)GameManager.systemParamsList.GetParamValueDoubleByName("VIPJingJiCD", 0.0);
			if (num2 > 0 && player.ClientData.VipLevel >= num2)
			{
				challengeCD = 0;
			}
		}

		private int getChallengeEndRanking(JingJiChallengeResultData resultData, int serverId)
		{
			return Global.sendToDB<int, byte[]>(10144, DataHelper.ObjectToBytes<JingJiChallengeResultData>(resultData), serverId);
		}

		private void relive(GameClient player)
		{
			player.ClientData.CurrentLifeV = player.ClientData.LifeV;
			player.ClientData.CurrentMagicV = player.ClientData.MagicV;
			Global.ClientRealive(player, (int)player.CurrentPos.X, (int)player.CurrentPos.Y, player.ClientData.RoleDirection);
		}

		public void onJingJiFuBenStartCD(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			player.sendCmd(581, "", false);
		}

		public void onJingJiFuBenStarted(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			Robot robot = instance.getRobot();
			GameMap gameMap = GameManager.MapMgr.DictMaps[this.jingjiFuBenItem.GetIntValue("MapCode", -1)];
			int gridX = gameMap.CorrectWidthPointToGridPoint(JingJiChangConstants.RobotBothX) / gameMap.MapGridWidth;
			int gridY = gameMap.CorrectHeightPointToGridPoint(JingJiChangConstants.RobotBothY) / gameMap.MapGridHeight;
			GameManager.MonsterZoneMgr.AddDynamicRobot(player.CurrentMapCode, robot, player.ClientData.CopyMapID, 1, gridX, gridY, 1, 0, 1, player.ClientData.RoleID);
		}

		public void onRobotBron(Robot robot)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient((int)robot.Tag);
			if (null != gameClient)
			{
				this.SendMySelfJingJiFakeRoleItem(gameClient, robot);
				robot.startAttack();
				GameManager.ClientMgr.BroadSpecialMapAIEvent(gameClient.ClientData.MapCode, gameClient.ClientData.CopyMapID, 1, 0);
			}
		}

		public void SendMySelfJingJiFakeRoleItem(GameClient player, Robot robot)
		{
			RoleDataMini roleDataMini = robot.getRoleDataMini();
			roleDataMini.PosX = (int)player.CurrentPos.X;
			roleDataMini.PosY = (int)player.CurrentPos.Y;
			player.sendCmd<RoleDataMini>(110, roleDataMini, false);
		}

		public void onJingJiFuBenStopForTimeOutCD(JingJiChangInstance instance)
		{
			if (null != instance)
			{
				lock (instance)
				{
					GameClient player = instance.getPlayer();
					Robot robot = instance.getRobot();
					if (player != null && null != robot)
					{
						GameManager.MonsterZoneMgr.DestroyCopyMapMonsters(player.ClientData.MapCode, player.ClientData.CopyMapID);
						robot.stopAttack();
						this.processFailed(player, robot, new JingJiChallengeResultData
						{
							playerId = player.ClientData.RoleID,
							robotId = robot.PlayerId,
							isWin = false
						}, instance.type);
					}
				}
			}
		}

		public void onJingJiFuBenStoped(JingJiChangInstance instance)
		{
			GameClient player = instance.getPlayer();
			if (player != null && !player.LogoutState)
			{
				if (player.CurrentMapCode == this.jingjiFuBenItem.GetIntValue("MapCode", -1))
				{
					if (player.ClientData.CurrentLifeV <= 0)
					{
						this.relive(player);
					}
					if (!Global.CanChangeMap(player, player.ClientData.LastMapCode, player.ClientData.LastPosX, player.ClientData.LastPosY, true))
					{
						player.ClientData.LastMapCode = GameManager.MainMapCode;
						player.ClientData.LastPosX = 0;
						player.ClientData.LastPosY = 0;
					}
					GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), player, -1, player.ClientData.LastMapCode, player.ClientData.LastPosX, player.ClientData.LastPosY, player.ClientData.RoleDirection, 123);
					player.sendCmd(587, string.Format("{0}", (int)instance.type), false);
				}
			}
		}

		public void onJingJiFuBenDestroy(JingJiChangInstance instance)
		{
			ScheduleExecutor2.Instance.scheduleCancle(instance);
			lock (this.jingjichangInstances)
			{
				this.jingjichangInstances.Remove(instance.getFuBenSeqId());
			}
			instance.release();
		}

		public void onLeaveFuBenForStopCD(GameClient player)
		{
			JingJiChangInstance jingJiChangInstance = null;
			lock (this.jingjichangInstances)
			{
				if (!this.jingjichangInstances.TryGetValue(player.ClientData.FuBenSeqID, out jingJiChangInstance))
				{
					return;
				}
			}
			lock (jingJiChangInstance)
			{
				if (jingJiChangInstance.getState() == JingJiFuBenState.STOP_CD)
				{
					Robot robot = jingJiChangInstance.getRobot();
					robot.stopAttack();
					jingJiChangInstance.switchState(JingJiFuBenState.STOPED);
				}
			}
		}

		public int GetLeftEnterCount(GameClient client)
		{
			int intValue = this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			FuBenData fuBenData = Global.GetFuBenData(client, this.jingjiFuBenId);
			int num;
			int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			int num2 = (fuBenEnterNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? fuBenEnterNum : this.jingjiFuBenItem.GetIntValue("EnterNumber", -1);
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJingJi", ',');
			int vipLevel = client.ClientData.VipLevel;
			int num3 = paramValueIntArrayByName[vipLevel];
			int num4 = (fuBenEnterNum <= this.jingjiFuBenItem.GetIntValue("EnterNumber", -1)) ? 0 : (fuBenEnterNum - this.jingjiFuBenItem.GetIntValue("EnterNumber", -1));
			return intValue + num3 - num2 - num4;
		}

		private static JingJiChangManager instance = new JingJiChangManager();

		private SystemXmlItems rewardConfig = new SystemXmlItems();

		private SystemXmlItems jingjiMainConfig = new SystemXmlItems();

		private SystemXmlItems junxianConfig = new SystemXmlItems();

		private SystemXmlItem jingjiFuBenItem = null;

		public int nJingJiChangMapCode = 0;

		private Dictionary<int, JingJiChangInstance> jingjichangInstances = new Dictionary<int, JingJiChangInstance>();

		private int jingjiFuBenId = -1;

		private int jingjiBuffId = -1;

		private int jingjiFuBenMinZhuanSheng = -1;

		private string[] junxianBuffTimeConfig;
	}
}
