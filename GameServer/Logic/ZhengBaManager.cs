using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.JingJiChang;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ZhengBaManager : SingletonTemplate<ZhengBaManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener
	{
		private ZhengBaManager()
		{
		}

		public bool initialize()
		{
			bool result;
			if (!this._Config.Load(Global.GameResPath("Config\\Match.xml"), Global.GameResPath("Config\\Sustain.xml"), Global.GameResPath("Config\\MatchBirthPoint.xml")))
			{
				result = false;
			}
			else
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config\\MatchAward.xml"));
				foreach (XElement xml in xelement.Elements())
				{
					ZhengBaMatchAward zhengBaMatchAward = new ZhengBaMatchAward();
					zhengBaMatchAward.AwardId = (int)Global.GetSafeAttributeLong(xml, "ID");
					zhengBaMatchAward.Name = Global.GetSafeAttributeStr(xml, "Name");
					zhengBaMatchAward.FinalPassDay = (int)Global.GetSafeAttributeLong(xml, "FinalPassDay");
					zhengBaMatchAward.GoodsList = GoodsHelper.ParseGoodsDataList(Global.GetSafeAttributeStr(xml, "Award").Split(new char[]
					{
						'|'
					}), "Config\\MatchAward.xml");
					Debug.Assert(zhengBaMatchAward.FinalPassDay >= 0 && zhengBaMatchAward.FinalPassDay <= 7);
					Debug.Assert(zhengBaMatchAward.GoodsList != null);
					this._MatchAwardList.Add(zhengBaMatchAward);
				}
				foreach (ZhengBaSupportConfig zhengBaSupportConfig in this._Config.SupportConfigList)
				{
					string text = (string)zhengBaSupportConfig.WinAwardTag;
					string text2 = (string)zhengBaSupportConfig.FailAwardTag;
					List<GoodsData> list = GoodsHelper.ParseGoodsDataList(text.Split(new char[]
					{
						'|'
					}), "Config\\Sustain.xml");
					List<GoodsData> list2 = GoodsHelper.ParseGoodsDataList(text2.Split(new char[]
					{
						'|'
					}), "Config\\Sustain.xml");
					zhengBaSupportConfig.FailAwardTag = list2;
					zhengBaSupportConfig.WinAwardTag = list;
					int num = 0;
					foreach (GoodsData goodsData in list)
					{
						SystemXmlItem systemXmlItem = null;
						List<MagicActionItem> list3 = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) || !GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out list3))
						{
							LogManager.WriteLog(1000, string.Format("众神争霸goods={0}找不到对应的action", new object[0]), null, true);
						}
						else
						{
							foreach (MagicActionItem magicActionItem in list3)
							{
								if (magicActionItem.MagicActionID == MagicActionIDs.ADD_ZHENGBADIANSHU)
								{
									num += (int)magicActionItem.MagicActionParams[0] * goodsData.GCount;
								}
							}
						}
					}
					zhengBaSupportConfig.WinPoint = num;
					num = 0;
					foreach (GoodsData goodsData in list2)
					{
						SystemXmlItem systemXmlItem = null;
						List<MagicActionItem> list3 = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem) || !GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out list3))
						{
							LogManager.WriteLog(2, string.Format("众神争霸goods={0}找不到对应的action", goodsData.GoodsID), null, true);
						}
						else
						{
							foreach (MagicActionItem magicActionItem in list3)
							{
								if (magicActionItem.MagicActionID == MagicActionIDs.ADD_ZHENGBADIANSHU)
								{
									num += (int)magicActionItem.MagicActionParams[0] * goodsData.GCount;
								}
							}
						}
					}
					zhengBaSupportConfig.FailPoint = num;
				}
				DateTime dateTime = TimeUtil.NowDateTime();
				int num2 = ZhengBaUtils.MakeMonth(dateTime);
				List<ZhengBaPkLogData> list4 = Global.sendToDB<List<ZhengBaPkLogData>, string>(14014, string.Format("{0}:{1}", num2, 100), 0);
				Dictionary<int, List<ZhengBaSupportLogData>> dictionary = Global.sendToDB<Dictionary<int, List<ZhengBaSupportLogData>>, string>(14013, string.Format("{0}:{1}", num2, 30), 0);
				List<ZhengBaWaitYaZhuAwardData> list5 = Global.sendToDB<List<ZhengBaWaitYaZhuAwardData>, string>(14016, string.Format("{0}", num2), 0);
				if (list4 != null)
				{
					list4.RemoveAll((ZhengBaPkLogData _log) => !_log.UpGrade);
					foreach (ZhengBaPkLogData item in list4)
					{
						this.PkLogQ.Enqueue(item);
					}
				}
				if (dictionary != null)
				{
					foreach (KeyValuePair<int, List<ZhengBaSupportLogData>> keyValuePair in dictionary)
					{
						Queue<ZhengBaSupportLogData> queue = new Queue<ZhengBaSupportLogData>();
						this.SupportLogs[keyValuePair.Key] = queue;
						foreach (ZhengBaSupportLogData item2 in keyValuePair.Value)
						{
							queue.Enqueue(item2);
						}
					}
				}
				if (list5 != null)
				{
					this.WaitAwardOfYaZhuList = list5;
				}
				this.SyncData.Month = num2;
				this.SyncData.RoleModTime = DateTime.MinValue;
				this.SyncData.SupportModTime = DateTime.MinValue;
				this.SyncData.RealActDay = -1;
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.TimerProc", new EventHandler(this.SyncCenterData)), 20000, 10000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.TimerProc", new EventHandler(this.CheckYaZhuAward)), 20000, 120000);
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhengBaManager.UpdateCopyScene", new EventHandler(this.UpdateCopyScene)), 10000, 100);
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1350, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1351, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1352, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1353, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1354, 4, 4, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1355, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1357, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1359, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1360, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1361, 1, 1, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1362, 2, 2, SingletonTemplate<ZhengBaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10020, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10021, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10022, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10023, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10024, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(10, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<ZhengBaManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10020, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10021, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10022, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10023, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10024, 36, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(10, SingletonTemplate<ZhengBaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<ZhengBaManager>.Instance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1350:
					return this.HandleGetMainInfo(client, nID, bytes, cmdParams);
				case 1351:
					return this.HandleGetAllPkLog(client, nID, bytes, cmdParams);
				case 1352:
					return this.HandleGetAllPkState(client, nID, bytes, cmdParams);
				case 1353:
					return this.HandleGet16PkState(client, nID, bytes, cmdParams);
				case 1354:
					return this.HandleSupport(client, nID, bytes, cmdParams);
				case 1355:
					return false;
				case 1357:
					return this.HandleEnter(client, nID, bytes, cmdParams);
				case 1359:
					return this.HandleGetMiniState(client, nID, bytes, cmdParams);
				case 1360:
					return this.HandleQueryJoinHint(client, nID, bytes, cmdParams);
				case 1361:
					return this.ProcessGetAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1362:
					return this.HandleAdmireStatue(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		private bool HandleQueryJoinHint(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = 0;
			DateTime dateTime = TimeUtil.NowDateTime();
			int num2 = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime());
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ZhengBaHintFlag");
			lock (this.Mutex)
			{
				if (this.SyncData.Month == num2 && roleParamsInt32FromDB != num2 && this.SyncData.IsThisMonthInActivity && this.IsGongNengOpened() && this.RoleDataDict.ContainsKey(client.ClientData.RoleID))
				{
					if (this.SyncData.RealActDay <= 0)
					{
						num = 1;
					}
					else
					{
						bool flag2;
						if (this.SyncData.RealActDay == 1)
						{
							flag2 = (dateTime.TimeOfDay.Ticks >= this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
						}
						else
						{
							flag2 = true;
						}
						if (!flag2)
						{
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
				}
			}
			client.sendCmd(nID, num.ToString(), false);
			if (num == 1)
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaHintFlag", num2, true);
			}
			return true;
		}

		private bool HandleGetMiniState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int realDay = 0;
			DateTime d = DateTime.MinValue;
			bool flag = false;
			bool flag2 = false;
			lock (this.Mutex)
			{
				d = TimeUtil.NowDateTime().Add(this.DiffKfCenter);
				realDay = this.SyncData.RealActDay;
				flag = this.SyncData.TodayIsPking;
				flag2 = this.SyncData.IsThisMonthInActivity;
			}
			ZhengBaMiniStateData zhengBaMiniStateData = new ZhengBaMiniStateData();
			zhengBaMiniStateData.IsZhengBaOpened = this.IsGongNengOpened();
			zhengBaMiniStateData.IsThisMonthInActivity = flag2;
			bool result;
			if (!zhengBaMiniStateData.IsZhengBaOpened || realDay < 0)
			{
				client.sendCmd<ZhengBaMiniStateData>(nID, zhengBaMiniStateData, false);
				result = true;
			}
			else
			{
				if (!flag2)
				{
					DateTime d2 = d.AddMonths(1);
					d2 = new DateTime(d2.Year, d2.Month, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					d2 = d2.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					zhengBaMiniStateData.PkStartWaitSec = (long)(d2 - d).TotalSeconds;
				}
				else if (realDay == 0)
				{
					DateTime d3 = DateTime.MinValue;
					if (d.AddDays(1.0).Month == d.Month)
					{
						d3 = new DateTime(d.Year, d.Month, Math.Max(d.Day + 1, ZhengBaConsts.StartMonthDay), 0, 0, 0);
					}
					else if (d.AddMonths(1).Year == d.Year)
					{
						d3 = new DateTime(d.Year, d.Month + 1, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					}
					else
					{
						d3 = new DateTime(d.Year + 1, 1, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					}
					d3 = d3.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					zhengBaMiniStateData.PkStartWaitSec = (long)(d3 - d).TotalSeconds;
				}
				else if (realDay >= 1 && realDay <= 7)
				{
					ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == realDay);
					if (d.TimeOfDay.Ticks <= zhengBaMatchConfig.DayBeginTick)
					{
						zhengBaMiniStateData.PkStartWaitSec = (zhengBaMatchConfig.DayBeginTick - d.TimeOfDay.Ticks) / 10000000L;
					}
					else if (d.TimeOfDay.Ticks >= zhengBaMatchConfig.DayEndTick || (d.TimeOfDay.Ticks - zhengBaMatchConfig.DayBeginTick > 600000000L && !flag))
					{
						bool flag4 = realDay == 7 || d.AddDays(1.0).Month != d.Month;
						DateTime d4 = flag4 ? new DateTime(d.AddMonths(1).Year, d.AddMonths(1).Month, ZhengBaConsts.StartMonthDay, 0, 0, 0) : new DateTime(d.Year, d.Month, d.Day + 1, 0, 0, 0);
						long dayBeginTick;
						if (!flag4)
						{
							dayBeginTick = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == realDay + 1).DayBeginTick;
						}
						else
						{
							dayBeginTick = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick;
						}
						d4 = d4.AddTicks(dayBeginTick);
						zhengBaMiniStateData.PkStartWaitSec = (long)(d4 - d).TotalSeconds;
					}
					else
					{
						int num = zhengBaMatchConfig.WaitSeconds + zhengBaMatchConfig.FightSeconds + zhengBaMatchConfig.ClearSeconds + zhengBaMatchConfig.IntervalSeconds;
						long num2 = (d.TimeOfDay.Ticks - zhengBaMatchConfig.DayBeginTick) / 10000000L;
						long num3 = num2 % (long)num;
						if (num3 < (long)(zhengBaMatchConfig.WaitSeconds + zhengBaMatchConfig.FightSeconds + zhengBaMatchConfig.ClearSeconds))
						{
							zhengBaMiniStateData.LoopEndWaitSec = (long)(zhengBaMatchConfig.WaitSeconds + zhengBaMatchConfig.FightSeconds + zhengBaMatchConfig.ClearSeconds) - num3;
						}
						else
						{
							zhengBaMiniStateData.NextLoopWaitSec = (long)num - num3;
						}
					}
				}
				else
				{
					DateTime d2 = d.AddMonths(1);
					d2 = new DateTime(d2.Year, d2.Month, ZhengBaConsts.StartMonthDay, 0, 0, 0);
					d2 = d2.AddTicks(this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1).DayBeginTick);
					zhengBaMiniStateData.PkStartWaitSec = (long)(d2 - d).TotalSeconds;
				}
				zhengBaMiniStateData.PkStartWaitSec = Math.Max(zhengBaMiniStateData.PkStartWaitSec, 0L);
				zhengBaMiniStateData.NextLoopWaitSec = Math.Max(zhengBaMiniStateData.NextLoopWaitSec, 0L);
				zhengBaMiniStateData.LoopEndWaitSec = Math.Max(zhengBaMiniStateData.LoopEndWaitSec, 0L);
				client.sendCmd<ZhengBaMiniStateData>(nID, zhengBaMiniStateData, false);
				result = true;
			}
			return result;
		}

		private bool HandleGetMainInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				ZhengBaMainInfoData zhengBaMainInfoData = new ZhengBaMainInfoData();
				lock (this.Mutex)
				{
					zhengBaMainInfoData.RealActDay = this.SyncData.RealActDay;
					zhengBaMainInfoData.RankResultOfDay = this.SyncData.RankResultOfDay;
					zhengBaMainInfoData.Top16List = this.Top16RoleList;
					zhengBaMainInfoData.MaxOpposeGroup = this.MaxOpposeGroup;
					zhengBaMainInfoData.MaxSupportGroup = this.MaxSupportGroup;
					zhengBaMainInfoData.CanGetAwardId = 0;
					bool flag2 = false;
					bool flag3;
					if (this.SyncData.RealActDay >= 7)
					{
						flag3 = !this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade == 1);
					}
					else
					{
						flag3 = true;
					}
					if (!flag3)
					{
						flag2 = true;
					}
					else if (dateTime.AddDays(1.0).Month != dateTime.Month)
					{
						ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
						if (zhengBaMatchConfig != null && dateTime.TimeOfDay.Ticks > zhengBaMatchConfig.DayEndTick + (long)((ulong)-1294967296) && !this.SyncData.TodayIsPking)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						TianTiPaiHangRoleData tianTiPaiHangRoleData = null;
						if (this.RoleDataDict.TryGetValue(client.ClientData.RoleID, out tianTiPaiHangRoleData))
						{
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ZhengBaAwardFlag");
							if (roleParamsInt32FromDB <= 0 || roleParamsInt32FromDB / 100 != this.SyncData.Month)
							{
								int day = ZhengBaUtils.WhichDayResultByGrade(tianTiPaiHangRoleData.ZhengBaGrade);
								ZhengBaMatchAward zhengBaMatchAward = this._MatchAwardList.Find((ZhengBaMatchAward _m) => _m.FinalPassDay == day);
								if (zhengBaMatchAward != null)
								{
									if (zhengBaMatchAward.GoodsList.Count > 0)
									{
										if (Global.CanAddGoodsDataList(client, zhengBaMatchAward.GoodsList))
										{
											foreach (GoodsData goodsData in zhengBaMatchAward.GoodsList)
											{
												Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "众神争霸", false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
											}
										}
										else
										{
											Global.UseMailGivePlayerAward3(client.ClientData.RoleID, zhengBaMatchAward.GoodsList, GLang.GetLang(539, new object[0]), string.Format(GLang.GetLang(540, new object[0]), zhengBaMatchAward.Name), 0, 0, 0);
										}
									}
									Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaAwardFlag", this.SyncData.Month * 100 + zhengBaMatchAward.AwardId, true);
									zhengBaMainInfoData.CanGetAwardId = zhengBaMatchAward.AwardId;
								}
							}
						}
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaJoinIconFlag", ZhengBaUtils.MakeMonth(dateTime), true);
					this.CheckTipsIconState(client);
				}
				client.sendCmd<ZhengBaMainInfoData>(nID, zhengBaMainInfoData, false);
				result = true;
			}
			return result;
		}

		private bool HandleGetAllPkLog(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				List<ZhengBaPkLogData> list = new List<ZhengBaPkLogData>();
				lock (this.Mutex)
				{
					list.AddRange(this.PkLogQ);
				}
				list.Reverse();
				client.sendCmd<List<ZhengBaPkLogData>>(nID, list, false);
				result = true;
			}
			return result;
		}

		private bool HandleGetAllPkState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				List<TianTiPaiHangRoleData> list = new List<TianTiPaiHangRoleData>();
				lock (this.Mutex)
				{
					list.AddRange(this.RoleDataList);
				}
				client.sendCmd<List<TianTiPaiHangRoleData>>(nID, list, false);
				result = true;
			}
			return result;
		}

		private bool HandleGet16PkState(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int unionGroup = Convert.ToInt32(cmdParams[1]);
				client.sendCmd<ZhengBaUnionGroupData>(nID, this.Get16PkState(client, unionGroup), false);
				result = true;
			}
			return result;
		}

		private ZhengBaUnionGroupData Get16PkState(GameClient client, int unionGroup)
		{
			int group1 = 0;
			int group2 = 0;
			ZhengBaUtils.SplitUnionGroup(unionGroup, ref group1, ref group2);
			List<ZhengBaSupportAnalysisData> list = null;
			List<ZhengBaSupportLogData> list2 = null;
			lock (this.Mutex)
			{
				list = this.SupportDatas;
				Queue<ZhengBaSupportLogData> collection = null;
				if (this.SupportLogs.TryGetValue(unionGroup, out collection))
				{
					list2 = new List<ZhengBaSupportLogData>(collection);
					list2.Reverse();
				}
			}
			ZhengBaUnionGroupData zhengBaUnionGroupData = new ZhengBaUnionGroupData();
			zhengBaUnionGroupData.UnionGroup = unionGroup;
			zhengBaUnionGroupData.SupportLogs = list2;
			zhengBaUnionGroupData.SupportDatas = new List<ZhengBaSupportAnalysisData>();
			ZhengBaSupportAnalysisData zhengBaSupportAnalysisData = list.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group1);
			ZhengBaSupportAnalysisData zhengBaSupportAnalysisData2 = list.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group2);
			if (zhengBaSupportAnalysisData != null)
			{
				zhengBaUnionGroupData.SupportDatas.Add(zhengBaSupportAnalysisData);
			}
			if (zhengBaSupportAnalysisData2 != null)
			{
				zhengBaUnionGroupData.SupportDatas.Add(zhengBaSupportAnalysisData2);
			}
			zhengBaUnionGroupData.SupportFlags = new List<ZhengBaSupportFlagData>();
			List<ZhengBaSupportFlagData> zhengBaSupportFlags = client.ClientData.ZhengBaSupportFlags;
			ZhengBaSupportFlagData zhengBaSupportFlagData = zhengBaSupportFlags.Find((ZhengBaSupportFlagData _s) => _s.UnionGroup == unionGroup && _s.Group == group1);
			ZhengBaSupportFlagData zhengBaSupportFlagData2 = zhengBaSupportFlags.Find((ZhengBaSupportFlagData _s) => _s.UnionGroup == unionGroup && _s.Group == group2);
			if (zhengBaSupportFlagData != null)
			{
				zhengBaUnionGroupData.SupportFlags.Add(zhengBaSupportFlagData);
			}
			if (zhengBaSupportFlagData2 != null)
			{
				zhengBaUnionGroupData.SupportFlags.Add(zhengBaSupportFlagData2);
			}
			int supportDay = 0;
			for (int i = 7; i >= 1; i--)
			{
				if (ZhengBaUtils.IsValidPkGroup(group1, group2, i))
				{
					supportDay = i - 1;
					break;
				}
			}
			ZhengBaSupportConfig zhengBaSupportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _s) => _s.RankOfDay == supportDay);
			if (zhengBaSupportConfig != null)
			{
				zhengBaUnionGroupData.WinZhengBaPoint = zhengBaSupportConfig.WinPoint;
			}
			return zhengBaUnionGroupData;
		}

		private bool HandleSupport(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int unionGroup = Convert.ToInt32(cmdParams[1]);
				int group = Convert.ToInt32(cmdParams[2]);
				int num = Convert.ToInt32(cmdParams[3]);
				if (num != 1 && num != 2 && num != 3)
				{
					result = true;
				}
				else
				{
					int group1 = 0;
					int group2 = 0;
					ZhengBaUtils.SplitUnionGroup(unionGroup, ref group1, ref group2);
					if (group < 1 || group > 16)
					{
						result = true;
					}
					else if (group1 < 1 || group1 > 16)
					{
						result = true;
					}
					else if (group2 < 1 || group2 > 16)
					{
						result = true;
					}
					else if (group1 >= group2)
					{
						result = true;
					}
					else if (group != group1 && group != group2)
					{
						result = true;
					}
					else if (this.SyncData.RealActDay < 3 || this.SyncData.RealActDay > 7)
					{
						result = true;
					}
					else
					{
						DateTime now = TimeUtil.NowDateTime();
						ZhengBaSupportConfig supportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _s) => _s.TimeList.Exists((ZhengBaSupportConfig.TimeConfig _t) => _t.RealDay == this.SyncData.RealActDay && _t.DayBeginTicks < now.TimeOfDay.Ticks && _t.DayEndTicks > now.TimeOfDay.Ticks));
						if (supportConfig == null)
						{
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-2001,
								unionGroup,
								group,
								num
							}), false);
							result = true;
						}
						else if (Global.GetUnionLevel(client, false) < Global.GetUnionLevel(supportConfig.MinChangeLife, supportConfig.MinLevel, false))
						{
							client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-19,
								unionGroup,
								group,
								num
							}), false);
							result = true;
						}
						else if (!ZhengBaUtils.IsValidPkGroup(group1, group2, supportConfig.RankOfDay + 1))
						{
							result = true;
						}
						else
						{
							lock (this.Mutex)
							{
								bool flag2;
								if (this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == group1 && _r.ZhengBaState == 1))
								{
									flag2 = this.Top16RoleList.Exists((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == group2 && _r.ZhengBaState == 1);
								}
								else
								{
									flag2 = false;
								}
								if (!flag2)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-12,
										unionGroup,
										group,
										num
									}), false);
									return true;
								}
							}
							if (num == 3)
							{
								int num2 = client.ClientData.ZhengBaSupportFlags.Count((ZhengBaSupportFlagData _s) => _s.RankOfDay == supportConfig.RankOfDay && _s.IsYaZhu);
								if (num2 >= supportConfig.MaxTimes)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-16,
										unionGroup,
										group,
										num
									}), false);
									return true;
								}
							}
							ZhengBaSupportFlagData zhengBaSupportFlagData = client.ClientData.ZhengBaSupportFlags.Find((ZhengBaSupportFlagData _f) => _f.UnionGroup == unionGroup && _f.Group == group);
							if (zhengBaSupportFlagData != null)
							{
								if ((zhengBaSupportFlagData.IsOppose || zhengBaSupportFlagData.IsSupport) && (num == 2 || num == 1))
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-5,
										unionGroup,
										group,
										num
									}), false);
									return true;
								}
								if (num == 3)
								{
									bool flag3;
									if (!zhengBaSupportFlagData.IsYaZhu)
									{
										flag3 = (client.ClientData.ZhengBaSupportFlags.Count((ZhengBaSupportFlagData _f) => _f.UnionGroup == unionGroup && _f.IsYaZhu) < 1);
									}
									else
									{
										flag3 = false;
									}
									if (!flag3)
									{
										client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											-5,
											unionGroup,
											group,
											num
										}), false);
										return true;
									}
								}
							}
							if (num == 3 && !Global.SubBindTongQianAndTongQian(client, supportConfig.CostJinBi, "众神争霸押注"))
							{
								client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-9,
									unionGroup,
									group,
									num
								}), false);
								result = true;
							}
							else
							{
								ZhengBaSupportLogData zhengBaSupportLogData = new ZhengBaSupportLogData();
								zhengBaSupportLogData.FromRoleId = client.ClientData.RoleID;
								zhengBaSupportLogData.FromZoneId = client.ClientData.ZoneID;
								zhengBaSupportLogData.FromRolename = client.ClientData.RoleName;
								zhengBaSupportLogData.SupportType = num;
								zhengBaSupportLogData.ToUnionGroup = unionGroup;
								zhengBaSupportLogData.ToGroup = group;
								zhengBaSupportLogData.Time = now;
								zhengBaSupportLogData.FromServerId = GameCoreInterface.getinstance().GetLocalServerId();
								zhengBaSupportLogData.Month = ZhengBaUtils.MakeMonth(now);
								zhengBaSupportLogData.RankOfDay = supportConfig.RankOfDay;
								int num3 = TianTiClient.getInstance().ZhengBaSupport(zhengBaSupportLogData);
								if (num3 < 0)
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										num3,
										unionGroup,
										group,
										num
									}), false);
									result = true;
								}
								else if (!Global.sendToDB<bool, ZhengBaSupportLogData>(14011, zhengBaSupportLogData, 0))
								{
									client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										-15,
										unionGroup,
										group,
										num
									}), false);
									result = true;
								}
								else
								{
									if (zhengBaSupportFlagData == null)
									{
										zhengBaSupportFlagData = new ZhengBaSupportFlagData();
										zhengBaSupportFlagData.UnionGroup = unionGroup;
										zhengBaSupportFlagData.Group = group;
										zhengBaSupportFlagData.RankOfDay = supportConfig.RankOfDay;
										client.ClientData.ZhengBaSupportFlags.Add(zhengBaSupportFlagData);
									}
									if (num == 1)
									{
										zhengBaSupportFlagData.IsSupport = true;
									}
									else if (num == 2)
									{
										zhengBaSupportFlagData.IsOppose = true;
									}
									else if (num == 3)
									{
										zhengBaSupportFlagData.IsYaZhu = true;
									}
									lock (this.Mutex)
									{
										Queue<ZhengBaSupportLogData> queue = null;
										if (!this.SupportLogs.TryGetValue(unionGroup, out queue))
										{
											queue = (this.SupportLogs[unionGroup] = new Queue<ZhengBaSupportLogData>());
										}
										queue.Enqueue(zhengBaSupportLogData);
										while (queue.Count > 30)
										{
											queue.Dequeue();
										}
										if (num == 3)
										{
											ZhengBaWaitYaZhuAwardData zhengBaWaitYaZhuAwardData = new ZhengBaWaitYaZhuAwardData();
											zhengBaWaitYaZhuAwardData.Month = zhengBaSupportLogData.Month;
											zhengBaWaitYaZhuAwardData.FromRoleId = client.ClientData.RoleID;
											zhengBaWaitYaZhuAwardData.UnionGroup = unionGroup;
											zhengBaWaitYaZhuAwardData.Group = group;
											zhengBaWaitYaZhuAwardData.RankOfDay = zhengBaSupportLogData.RankOfDay;
											this.WaitAwardOfYaZhuList.Add(zhengBaWaitYaZhuAwardData);
										}
										ZhengBaSupportAnalysisData zhengBaSupportAnalysisData = this.SupportDatas.Find((ZhengBaSupportAnalysisData _s) => _s.UnionGroup == unionGroup && _s.Group == group);
										if (zhengBaSupportAnalysisData == null)
										{
											zhengBaSupportAnalysisData = new ZhengBaSupportAnalysisData
											{
												UnionGroup = unionGroup,
												Group = group
											};
											this.SupportDatas.Add(zhengBaSupportAnalysisData);
										}
										if (num == 1)
										{
											zhengBaSupportAnalysisData.TotalSupport++;
										}
										else if (num == 2)
										{
											zhengBaSupportAnalysisData.TotalOppose++;
										}
										else if (num == 3)
										{
											zhengBaSupportAnalysisData.TotalYaZhu++;
										}
									}
									client.sendCmd<ZhengBaUnionGroupData>(1353, this.Get16PkState(client, unionGroup), false);
									result = true;
								}
							}
						}
					}
				}
			}
			return result;
		}

		private bool HandleEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened())
			{
				result = true;
			}
			else
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				if ((long)num != client.ClientSocket.ClientKuaFuServerLoginData.GameId || client.ClientSocket.ClientKuaFuServerLoginData.GameType != 12)
				{
					client.sendCmd(nID, string.Format("{0}", -2001), false);
					result = true;
				}
				else if (num2 != 1 && num2 != 2)
				{
					client.sendCmd(nID, string.Format("{0}", -18), false);
					result = true;
				}
				else
				{
					int num3 = TianTiClient.getInstance().ZhengBaRequestEnter(client.ClientData.RoleID, num, num2);
					if (num3 < 0)
					{
						client.sendCmd(nID, string.Format("{0}", num3), false);
						result = true;
					}
					else
					{
						if (num2 == 1)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else if (num2 == 2)
						{
							client.ClientSocket.ClientKuaFuServerLoginData.RoleId = 0;
							client.ClientSocket.ClientKuaFuServerLoginData.GameId = 0L;
							client.ClientSocket.ClientKuaFuServerLoginData.GameType = 0;
							client.ClientSocket.ClientKuaFuServerLoginData.ServerId = 0;
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool ProcessGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int offsetDayNow = TimeUtil.GetOffsetDayNow();
				long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10151");
				int num2 = (int)(roleParamsInt64FromDB % 10000L);
				int admireCount = (int)(roleParamsInt64FromDB / 10000L);
				if (num2 != offsetDayNow)
				{
					admireCount = 0;
				}
				client.sendCmd<LangHunLingYuKingShowData>(nID, new LangHunLingYuKingShowData
				{
					AdmireCount = admireCount,
					RoleData4Selector = this.ZhengBaKingData
				}, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool HandleAdmireStatue(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = 0;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			int offsetDayNow = TimeUtil.GetOffsetDayNow();
			MoBaiData moBaiData = null;
			if (!Data.MoBaiDataInfoList.TryGetValue(3, out moBaiData))
			{
				cmdData = -3;
			}
			else if (client.ClientData.ChangeLifeCount < moBaiData.MinZhuanSheng || (client.ClientData.ChangeLifeCount == moBaiData.MinZhuanSheng && client.ClientData.Level < moBaiData.MinLevel))
			{
				cmdData = -19;
			}
			else
			{
				int num3 = moBaiData.AdrationMaxLimit;
				long num4 = Global.GetRoleParamsInt64FromDB(client, "10151");
				int num5 = (int)(num4 % 10000L);
				int num6 = (int)(num4 / 10000L);
				if (num5 != offsetDayNow)
				{
					num6 = 0;
				}
				int vipLevel = client.ClientData.VipLevel;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (vipLevel > VIPEumValue.VIPENUMVALUE_MAXLEVEL || paramValueIntArrayByName.Length < 1)
				{
					cmdData = -3;
				}
				else
				{
					num3 += paramValueIntArrayByName[vipLevel];
					double num7 = 0.0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(12);
						if (null != config)
						{
							num7 += config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num7 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_Admire);
					}
					num7 = Math.Max(1.0, num7);
					num3 = (int)((double)num3 * num7);
					if (this.ZhengBaKingRoleId == num)
					{
						num3++;
					}
					if (num6 >= num3)
					{
						cmdData = -16;
					}
					else if (num2 == 1 && Global.GetTotalBindTongQianAndTongQianVal(client) < moBaiData.NeedJinBi)
					{
						cmdData = -9;
					}
					else if (num2 == 2 && client.ClientData.UserMoney < moBaiData.NeedZuanShi)
					{
						cmdData = -10;
					}
					else
					{
						double num8 = (client.ClientData.ChangeLifeCount == 0) ? 1.0 : Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
						if (num2 == 1)
						{
							Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜众神之王");
							long num9 = (long)(num8 * (double)moBaiData.JinBiExpAward);
							if (num9 > 0L)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, num9, true, true, false, "none");
							}
							if (moBaiData.JinBiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (moBaiData.LingJingAwardByJinBi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜众神之王", true, true, false);
							}
						}
						if (num2 == 2)
						{
							GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜众神之王", true, true, false, DaiBiSySType.None);
							int num10 = (int)(num8 * (double)moBaiData.ZuanShiExpAward);
							if (num10 > 0)
							{
								GameManager.ClientMgr.ProcessRoleExperience(client, (long)num10, true, true, false, "none");
							}
							if (moBaiData.ZuanShiZhanGongAward > 0)
							{
								GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.CoupleWishMoBai, 0);
							}
							if (moBaiData.LingJingAwardByZuanShi > 0)
							{
								GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜众神之王", true, true, false);
							}
						}
						num6++;
						num4 = (long)(num6 * 10000 + offsetDayNow);
						Global.SaveRoleParamsInt64ValueToDB(client, "10151", num4, true);
					}
				}
			}
			client.sendCmd<int>(nID, cmdData, false);
			return true;
		}

		public void SyncCenterData(object sender, EventArgs e)
		{
			ZhengBaSyncData zhengBaRankData = TianTiClient.getInstance().GetZhengBaRankData(this.SyncData);
			if (zhengBaRankData != null)
			{
				if (zhengBaRankData.LastKingData != null)
				{
					TianTiPaiHangRoleData tianTiPaiHangRoleData = null;
					if (zhengBaRankData.LastKingData != null && zhengBaRankData.LastKingData.TianTiPaiHangRoleData != null)
					{
						ZhengBaRoleInfoData zhengBaRoleInfoData = zhengBaRankData.LastKingData;
						tianTiPaiHangRoleData = DataHelper.BytesToObject<TianTiPaiHangRoleData>(zhengBaRoleInfoData.TianTiPaiHangRoleData, 0, zhengBaRoleInfoData.TianTiPaiHangRoleData.Length);
					}
					if (tianTiPaiHangRoleData != null)
					{
						this.ZhengBaKingRoleId = tianTiPaiHangRoleData.RoleId;
						this.ZhengBaKingData = tianTiPaiHangRoleData.RoleData4Selector.Clone();
					}
					else
					{
						this.ZhengBaKingRoleId = 0;
						this.ZhengBaKingData = null;
					}
					NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, FakeRoleNpcId.ZhengBaKing);
					if (null != npc)
					{
						if (this.ZhengBaKingData == null)
						{
							npc.ShowNpc = true;
							GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
							FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.ZhengBaKing, true);
						}
						else
						{
							npc.ShowNpc = false;
							GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
							FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.ZhengBaKing, true);
							FakeRoleManager.ProcessNewFakeRole(this.ZhengBaKingData, npc.MapCode, FakeRoleTypes.ZhengBaKing, (int)npc.CurrentDir, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, FakeRoleNpcId.ZhengBaKing);
						}
					}
				}
				if (zhengBaRankData.RoleList != null)
				{
					List<TianTiPaiHangRoleData> list = new List<TianTiPaiHangRoleData>();
					Dictionary<int, TianTiPaiHangRoleData> dictionary = new Dictionary<int, TianTiPaiHangRoleData>();
					Dictionary<int, PlayerJingJiData> dictionary2 = new Dictionary<int, PlayerJingJiData>();
					foreach (ZhengBaRoleInfoData zhengBaRoleInfoData in zhengBaRankData.RoleList)
					{
						if (null != zhengBaRoleInfoData.TianTiPaiHangRoleData)
						{
							TianTiPaiHangRoleData tianTiPaiHangRoleData2 = DataHelper.BytesToObject<TianTiPaiHangRoleData>(zhengBaRoleInfoData.TianTiPaiHangRoleData, 0, zhengBaRoleInfoData.TianTiPaiHangRoleData.Length);
							if (tianTiPaiHangRoleData2 != null)
							{
								tianTiPaiHangRoleData2.RoleId = zhengBaRoleInfoData.RoleId;
								tianTiPaiHangRoleData2.DuanWeiRank = zhengBaRoleInfoData.DuanWeiRank;
								tianTiPaiHangRoleData2.ZhengBaGrade = zhengBaRoleInfoData.Grade;
								tianTiPaiHangRoleData2.ZhengBaGroup = zhengBaRoleInfoData.Group;
								tianTiPaiHangRoleData2.ZhengBaState = zhengBaRoleInfoData.State;
								list.Add(tianTiPaiHangRoleData2);
								dictionary.Add(tianTiPaiHangRoleData2.RoleId, tianTiPaiHangRoleData2);
								if (null != zhengBaRoleInfoData.PlayerJingJiMirrorData)
								{
									PlayerJingJiData playerJingJiData = DataHelper.BytesToObject<PlayerJingJiData>(zhengBaRoleInfoData.PlayerJingJiMirrorData, 0, zhengBaRoleInfoData.PlayerJingJiMirrorData.Length);
									if (playerJingJiData != null)
									{
										dictionary2[tianTiPaiHangRoleData2.RoleId] = playerJingJiData;
									}
								}
							}
						}
					}
					list.Sort(delegate(TianTiPaiHangRoleData _l, TianTiPaiHangRoleData _r)
					{
						int result;
						if (_l.ZhengBaGrade < _r.ZhengBaGrade)
						{
							result = -1;
						}
						else if (_l.ZhengBaGrade > _r.ZhengBaGrade)
						{
							result = 1;
						}
						else if (_l.ZhengBaState < _r.ZhengBaState)
						{
							result = -1;
						}
						else if (_l.ZhengBaState > _r.ZhengBaState)
						{
							result = 1;
						}
						else
						{
							result = _l.DuanWeiRank - _r.DuanWeiRank;
						}
						return result;
					});
					List<TianTiPaiHangRoleData> list2 = list.FindAll((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade <= 16);
					TianTiPaiHangRoleData tianTiPaiHangRoleData = list2.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGrade == 1);
					if (tianTiPaiHangRoleData != null)
					{
						this.SetZhongShengRole(tianTiPaiHangRoleData.RoleId);
					}
					lock (this.Mutex)
					{
						foreach (TianTiPaiHangRoleData tianTiPaiHangRoleData2 in list)
						{
							if (tianTiPaiHangRoleData2.RoleData4Selector != null)
							{
								tianTiPaiHangRoleData2.RoleData4Selector.GoodsDataList = null;
								tianTiPaiHangRoleData2.RoleData4Selector.MyWingData = null;
							}
						}
						this.RoleDataList = list;
						this.RoleDataDict = dictionary;
						this.Top16RoleList = list2;
						this.MirrorDatas = dictionary2;
					}
				}
				if (zhengBaRankData.SupportList != null)
				{
					List<ZhengBaSupportAnalysisData> supportList = zhengBaRankData.SupportList;
					lock (this.Mutex)
					{
						this.SupportDatas = supportList;
						List<KeyValuePair<int, int>> list3 = new List<KeyValuePair<int, int>>();
						List<KeyValuePair<int, int>> list4 = new List<KeyValuePair<int, int>>();
						using (List<ZhengBaSupportAnalysisData>.Enumerator enumerator3 = this.SupportDatas.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								ZhengBaSupportAnalysisData data = enumerator3.Current;
								if (data.RankOfDay == zhengBaRankData.RankResultOfDay)
								{
									TianTiPaiHangRoleData tianTiPaiHangRoleData3;
									if ((tianTiPaiHangRoleData3 = this.Top16RoleList.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == data.Group)) != null && tianTiPaiHangRoleData3.ZhengBaState == 1)
									{
										list3.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Key == data.Group);
										list4.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Key == data.Group);
										list3.Add(new KeyValuePair<int, int>(data.Group, data.TotalSupport));
										list4.Add(new KeyValuePair<int, int>(data.Group, data.TotalOppose));
									}
								}
							}
						}
						list3.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Value <= 0);
						list4.RemoveAll((KeyValuePair<int, int> _kvp) => _kvp.Value <= 0);
						list3.Sort((KeyValuePair<int, int> _l, KeyValuePair<int, int> _r) => _r.Value - _l.Value);
						list4.Sort((KeyValuePair<int, int> _l, KeyValuePair<int, int> _r) => _r.Value - _l.Value);
						int num = 0;
						int num2 = 0;
						if (list3.Count > 0)
						{
							num = list3[0].Key;
						}
						if (list4.Count > 0)
						{
							num2 = list4[0].Key;
							if (num2 == num)
							{
								num2 = 0;
								if (list4.Count > 1)
								{
									num2 = list4[1].Key;
								}
							}
						}
						this.MaxSupportGroup = num;
						this.MaxOpposeGroup = num2;
					}
				}
				lock (this.Mutex)
				{
					if (this.SyncData.Month != zhengBaRankData.Month)
					{
						this.SupportLogs.Clear();
						this.PkLogQ.Clear();
						this.WaitAwardOfYaZhuList.Clear();
					}
					zhengBaRankData.RoleList = null;
					zhengBaRankData.SupportList = null;
					this.SyncData = zhengBaRankData;
					this.DiffKfCenter = zhengBaRankData.CenterTime - TimeUtil.NowDateTime();
				}
			}
		}

		public void SetZhongShengRole(int roleid)
		{
			int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("ZhongShenZhiShenRole", 0);
			if (gameConfigItemInt != roleid)
			{
				Global.UpdateDBGameConfigg("ZhongShenZhiShenRole", roleid.ToString());
				GameManager.GameConfigMgr.SetGameConfigItem("ZhongShenZhiShenRole", roleid.ToString());
				GameClient gameClient = GameManager.ClientMgr.FindClient(gameConfigItemInt);
				if (gameClient != null)
				{
					this.CheckZhongShenChengHao(gameClient);
				}
				GameClient gameClient2 = GameManager.ClientMgr.FindClient(roleid);
				if (gameClient2 != null)
				{
					this.CheckZhongShenChengHao(gameClient2);
				}
			}
		}

		private void CheckZhongShenChengHao(GameClient client)
		{
			if (client != null)
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("ZhongShenZhiShenRole", 0);
				BufferData bufferDataByID = Global.GetBufferDataByID(client, 111);
				if (client.ClientData.RoleID != gameConfigItemInt)
				{
					if (bufferDataByID != null && bufferDataByID.BufferVal != 0L)
					{
						double[] array = new double[1];
						double[] actionParams = array;
						Global.UpdateBufferData(client, BufferItemTypes.ZhongShenZhiShen_ChengHao, actionParams, 0, true);
					}
				}
				else if (bufferDataByID == null || bufferDataByID.BufferVal == 0L)
				{
					double[] actionParams = new double[]
					{
						1.0
					};
					Global.UpdateBufferData(client, BufferItemTypes.ZhongShenZhiShen_ChengHao, actionParams, 0, true);
				}
			}
		}

		private void CheckYaZhuAward(object sender, EventArgs e)
		{
			lock (this.Mutex)
			{
				if (this.WaitAwardOfYaZhuList.Count > 0)
				{
					using (List<ZhengBaWaitYaZhuAwardData>.Enumerator enumerator = this.WaitAwardOfYaZhuList.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ZhengBaWaitYaZhuAwardData waitAward = enumerator.Current;
							TianTiPaiHangRoleData tianTiPaiHangRoleData = this.Top16RoleList.Find((TianTiPaiHangRoleData _r) => _r.ZhengBaGroup == waitAward.Group);
							if (tianTiPaiHangRoleData != null)
							{
								ZhengBaSupportConfig zhengBaSupportConfig = this._Config.SupportConfigList.Find((ZhengBaSupportConfig _m) => _m.RankOfDay == waitAward.RankOfDay);
								if (zhengBaSupportConfig != null)
								{
									string text = string.Empty;
									List<GoodsData> goodsData;
									if (tianTiPaiHangRoleData.ZhengBaGrade <= ZhengBaUtils.GetDayUpGrade(waitAward.RankOfDay + 1))
									{
										text = ((tianTiPaiHangRoleData.ZhengBaGrade == 1) ? GLang.GetLang(541, new object[0]) : GLang.GetLang(542, new object[0]));
										text = string.Format(text, tianTiPaiHangRoleData.RoleName, zhengBaSupportConfig.WinPoint);
										goodsData = (zhengBaSupportConfig.WinAwardTag as List<GoodsData>);
									}
									else
									{
										if (tianTiPaiHangRoleData.ZhengBaState != 2)
										{
											continue;
										}
										text = GLang.GetLang(543, new object[0]);
										text = string.Format(text, tianTiPaiHangRoleData.RoleName, zhengBaSupportConfig.FailPoint);
										goodsData = (zhengBaSupportConfig.FailAwardTag as List<GoodsData>);
									}
									if (Global.UseMailGivePlayerAward3(waitAward.FromRoleId, goodsData, GLang.GetLang(539, new object[0]), text, 0, 0, 0))
									{
										Global.sendToDB<bool, string>(14017, string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											waitAward.Month,
											waitAward.FromRoleId,
											waitAward.UnionGroup,
											waitAward.Group
										}), 0);
										waitAward.FromRoleId = -1;
									}
								}
							}
						}
					}
					this.WaitAwardOfYaZhuList.RemoveAll((ZhengBaWaitYaZhuAwardData _w) => _w.FromRoleId == -1);
				}
			}
		}

		public void CheckTipsIconState(GameClient client)
		{
			if (client != null)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				int num = ZhengBaUtils.MakeMonth(TimeUtil.NowDateTime());
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ZhengBaJoinIconFlag");
				bool bIconState = false;
				lock (this.Mutex)
				{
					if (this.SyncData.Month == num && roleParamsInt32FromDB != num && this.SyncData.IsThisMonthInActivity && this.IsGongNengOpened() && this.RoleDataDict.ContainsKey(client.ClientData.RoleID))
					{
						bIconState = true;
					}
				}
				if (client._IconStateMgr.AddFlushIconState(15010, bIconState))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public void OnLogin(GameClient client)
		{
			if (client != null)
			{
				this.CheckZhongShenChengHao(client);
				this.CheckGongNengCanOpen(client);
				this.CheckTipsIconState(client);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					if (dateTime.Day > ZhengBaConsts.StartMonthDay)
					{
						int num = ZhengBaUtils.MakeMonth(dateTime);
						List<ZhengBaSupportFlagData> list = Global.sendToDB<List<ZhengBaSupportFlagData>, string>(14015, string.Format("{0}:{1}", client.ClientData.RoleID, num), client.ServerId);
						client.ClientData.ZhengBaSupportFlags.Clear();
						if (list != null)
						{
							client.ClientData.ZhengBaSupportFlags.AddRange(list);
						}
					}
				}
			}
		}

		public bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(9) && GameManager.GameConfigMgr.GetGameConfigItemInt("ZhengBaOpenedFlag", 0) == 1;
		}

		public void CheckGongNengCanOpen(GameClient client)
		{
			if (client != null)
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(9))
				{
					int num = 1;
					if (GameManager.GameConfigMgr.GetGameConfigItemInt("ZhengBaOpenedFlag", 0) != num && TianTiManager.getInstance().IsGongNengOpened(client, false))
					{
						Global.UpdateDBGameConfigg("ZhengBaOpenedFlag", num.ToString());
						GameManager.GameConfigMgr.SetGameConfigItem("ZhengBaOpenedFlag", num.ToString());
						string text = GLang.GetLang(544, new object[0]);
						text = string.Format(text, client.ClientData.RoleName);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, text, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		public void OnNewDay(GameClient client)
		{
			if (client != null && !client.ClientSocket.IsKuaFuLogin)
			{
				if (TimeUtil.NowDateTime().Day == 1)
				{
					client.ClientData.ZhengBaSupportFlags.Clear();
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(9))
			{
				if (eventObject.EventType == 10020)
				{
					this.HandleSupportLog((eventObject as KFZhengBaSupportEvent).Data);
				}
				else if (eventObject.EventType == 10021)
				{
					this.HandlePkLog((eventObject as KFZhengBaPkLogEvent).Log);
				}
				else if (eventObject.EventType == 10022)
				{
					this.HandleNtfEnter((eventObject as KFZhengBaNtfEnterEvent).Data);
				}
				else if (eventObject.EventType == 10023)
				{
					this.HandleMirrirFight((eventObject as KFZhengBaMirrorFightEvent).Data);
				}
				else if (eventObject.EventType == 10024)
				{
					this.HandleBulletinJoin((eventObject as KFZhengBaBulletinJoinEvent).Data);
				}
			}
			eventObject.Handled = true;
		}

		private void HandleBulletinJoin(ZhengBaBulletinJoinData data)
		{
			this.SyncCenterData(null, null);
			if (!GameFuncControlManager.IsGameFuncDisabled(9))
			{
				if (data.NtfType == 1)
				{
					ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
					DateTime dateTime = new DateTime(zhengBaMatchConfig.DayBeginTick);
					string text = GLang.GetLang(545, new object[0]);
					text = string.Format(text, new object[]
					{
						ZhengBaConsts.StartMonthDay,
						dateTime.Hour,
						dateTime.Minute,
						data.Args1
					});
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, text, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
				}
				else if (data.NtfType == 2)
				{
					ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == 1);
					DateTime dateTime = new DateTime(zhengBaMatchConfig.DayBeginTick);
					DateTime dateTime2 = new DateTime(zhengBaMatchConfig.DayEndTick);
					string mailMsg = GLang.GetLang(546, new object[0]);
					mailMsg = string.Format(mailMsg, new object[]
					{
						ZhengBaConsts.StartMonthDay,
						dateTime.Hour,
						dateTime.Minute,
						dateTime2.Hour,
						dateTime2.Minute
					});
					List<int> list = new List<int>();
					lock (this.Mutex)
					{
						List<int> list2 = this.RoleDataDict.Keys.ToList<int>();
						if (list2 != null)
						{
							list.AddRange(list2);
						}
					}
					list.ForEach(delegate(int _rid)
					{
						Global.UseMailGivePlayerAward3(_rid, null, GLang.GetLang(539, new object[0]), mailMsg, 0, 1, 0);
					});
				}
				else if (data.NtfType == 3)
				{
					string text2 = GLang.GetLang(547, new object[0]);
					Global.UseMailGivePlayerAward3(data.Args1, null, GLang.GetLang(539, new object[0]), text2, 0, 1, 0);
				}
				else if (data.NtfType == 4)
				{
					if (data.Args1 >= 1 && data.Args1 < 7)
					{
						string text2 = string.Format(GLang.GetLang(548, new object[0]), data.Args1);
						Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, text2, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		private void HandleMirrirFight(ZhengBaMirrorFightData data)
		{
			if (data.ToServerId == GameCoreInterface.getinstance().GetLocalServerId())
			{
				lock (this.Mutex)
				{
					ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
					if (zhengBaMatchConfig != null)
					{
						PlayerJingJiData playerJingJiData = null;
						if (!this.MirrorDatas.TryGetValue(data.RoleId, out playerJingJiData))
						{
							LogManager.WriteLog(2, string.Format("镜像出战，找不到镜像, server={0}, rid={1}, gameid={2}", data.ToServerId, data.RoleId, data.GameId), null, true);
						}
						else
						{
							ZhengBaManager.GameSideInfo gameSideInfo = null;
							if (!this.GameId2FuBenSeq.TryGetValue(data.GameId, out gameSideInfo))
							{
								gameSideInfo = new ZhengBaManager.GameSideInfo();
								gameSideInfo.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
								this.GameId2FuBenSeq[data.GameId] = gameSideInfo;
							}
							ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene = null;
							if (!this.FuBenSeq2CopyScenes.TryGetValue(gameSideInfo.FuBenSeq, out zhengBaCopyScene))
							{
								zhengBaCopyScene = new ZhengBaManager.ZhengBaCopyScene();
								zhengBaCopyScene.FuBenSeq = gameSideInfo.FuBenSeq;
								zhengBaCopyScene.GameId = data.GameId;
								zhengBaCopyScene.MapCode = zhengBaMatchConfig.MapCode;
								this.FuBenSeq2CopyScenes[gameSideInfo.FuBenSeq] = zhengBaCopyScene;
							}
							if (zhengBaCopyScene.RoleId1 <= 0)
							{
								zhengBaCopyScene.RoleId1 = data.RoleId;
								zhengBaCopyScene.IsMirror1 = true;
								zhengBaCopyScene.JingJiData1 = playerJingJiData;
								zhengBaCopyScene.Robot1 = null;
							}
							else if (zhengBaCopyScene.RoleId2 <= 0)
							{
								zhengBaCopyScene.RoleId2 = data.RoleId;
								zhengBaCopyScene.IsMirror2 = true;
								zhengBaCopyScene.JingJiData2 = playerJingJiData;
								zhengBaCopyScene.Robot2 = null;
							}
						}
					}
				}
			}
		}

		private void HandleSupportLog(ZhengBaSupportLogData data)
		{
			if (Global.sendToDB<bool, ZhengBaSupportLogData>(14011, data, 0))
			{
				lock (this.Mutex)
				{
					Queue<ZhengBaSupportLogData> queue = null;
					if (!this.SupportLogs.TryGetValue(data.ToUnionGroup, out queue))
					{
						queue = (this.SupportLogs[data.ToUnionGroup] = new Queue<ZhengBaSupportLogData>());
					}
					queue.Enqueue(data);
					while (queue.Count > 30)
					{
						queue.Dequeue();
					}
				}
			}
		}

		private void HandlePkLog(ZhengBaPkLogData data)
		{
			if (data.PkResult != 0)
			{
				if (data.UpGrade)
				{
					if (Global.sendToDB<bool, ZhengBaPkLogData>(14012, data, 0))
					{
						lock (this.Mutex)
						{
							this.PkLogQ.Enqueue(data);
							while (this.PkLogQ.Count > 100)
							{
								this.PkLogQ.Dequeue();
							}
						}
					}
				}
			}
		}

		private void HandleNtfEnter(ZhengBaNtfEnterData data)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(data.RoleId1);
			if (gameClient != null && !gameClient.ClientSocket.IsKuaFuLogin)
			{
				bool flag = false;
				lock (this.Mutex)
				{
					flag = this.MirrorDatas.ContainsKey(data.RoleId1);
				}
				gameClient.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId1;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)data.GameId;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.GameType = 12;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
				gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerIp = data.ToServerIp;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerPort = data.ToServerPort;
				gameClient.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
				gameClient.sendCmd(1356, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					data.GameId,
					data.Day,
					data.Loop,
					flag ? 1 : 0
				}), false);
			}
			GameClient gameClient2 = GameManager.ClientMgr.FindClient(data.RoleId2);
			if (gameClient2 != null && !gameClient2.ClientSocket.IsKuaFuLogin)
			{
				bool flag = false;
				lock (this.Mutex)
				{
					flag = this.MirrorDatas.ContainsKey(data.RoleId2);
				}
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId2;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.GameId = (long)data.GameId;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.GameType = 12;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerIp = data.ToServerIp;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerPort = data.ToServerPort;
				gameClient2.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
				gameClient2.sendCmd(1356, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					data.GameId,
					data.Day,
					data.Loop,
					flag ? 1 : 0
				}), false);
			}
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 10)
			{
				this.HandleClientDead(((PlayerDeadEventObject)eventObject).getPlayer());
			}
			if (eventObject.getEventType() == 11)
			{
				this.HandleMonsterDead(((MonsterDeadEventObject)eventObject).getAttacker(), ((MonsterDeadEventObject)eventObject).getMonster());
			}
		}

		private void HandleClientDead(GameClient player)
		{
			this.OnLogout(player);
		}

		private void HandleMonsterDead(GameClient player, Monster monster)
		{
			if (player != null)
			{
				if (monster != null)
				{
					Robot robot = monster as Robot;
					if (robot != null)
					{
						if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0)
						{
							lock (this.Mutex)
							{
								ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene = null;
								if (this.FuBenSeq2CopyScenes.TryGetValue(player.ClientData.FuBenSeqID, out zhengBaCopyScene))
								{
									if (player.ClientData.MapCode == zhengBaCopyScene.MapCode)
									{
										if (monster.CurrentMapCode == zhengBaCopyScene.MapCode)
										{
											if (zhengBaCopyScene.m_eStatus == 2)
											{
												if (zhengBaCopyScene.Robot1 != null)
												{
													zhengBaCopyScene.Robot1.stopAttack();
												}
												if (zhengBaCopyScene.Robot2 != null)
												{
													zhengBaCopyScene.Robot2.stopAttack();
												}
												zhengBaCopyScene.Winner = player.ClientData.RoleID;
												zhengBaCopyScene.m_eStatus = 3;
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

		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			return TianTiClient.getInstance().ZhengBaKuaFuLogin(kuaFuServerLoginData.RoleId, (int)kuaFuServerLoginData.GameId) >= 0;
		}

		public bool KuaFuInitGame(GameClient client)
		{
			bool result;
			lock (this.Mutex)
			{
				ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (zhengBaMatchConfig == null)
				{
					result = false;
				}
				else
				{
					int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					if (num < 0)
					{
						result = false;
					}
					else
					{
						ZhengBaManager.GameSideInfo gameSideInfo = null;
						if (!this.GameId2FuBenSeq.TryGetValue(num, out gameSideInfo))
						{
							gameSideInfo = new ZhengBaManager.GameSideInfo();
							gameSideInfo.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							this.GameId2FuBenSeq[num] = gameSideInfo;
						}
						int posX = 0;
						int posY = 0;
						if (!this.GetBirthPoint(zhengBaMatchConfig.MapCode, ++gameSideInfo.CurrSide, out posX, out posY))
						{
							LogManager.WriteLog(2, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, zhengBaMatchConfig.MapCode, gameSideInfo.CurrSide), null, true);
							result = false;
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = gameSideInfo.FuBenSeq;
							client.ClientData.MapCode = zhengBaMatchConfig.MapCode;
							client.ClientData.PosX = posX;
							client.ClientData.PosY = posY;
							client.ClientData.FuBenSeqID = gameSideInfo.FuBenSeq;
							client.ClientData.BattleWhichSide = gameSideInfo.CurrSide;
							result = true;
						}
					}
				}
			}
			return result;
		}

		public void OnLogout(GameClient player)
		{
			if (player != null && player.ClientSocket.IsKuaFuLogin)
			{
				if (player.ClientData.CopyMapID > 0 && player.ClientData.FuBenSeqID > 0)
				{
					lock (this.Mutex)
					{
						ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene = null;
						if (this.FuBenSeq2CopyScenes.TryGetValue(player.ClientData.FuBenSeqID, out zhengBaCopyScene))
						{
							if (player.ClientData.MapCode == zhengBaCopyScene.MapCode)
							{
								if (zhengBaCopyScene.m_eStatus < 2)
								{
									if (zhengBaCopyScene.FirstLeaveRoleId <= 0 && (zhengBaCopyScene.RoleId1 == player.ClientData.RoleID || zhengBaCopyScene.RoleId2 == player.ClientData.RoleID))
									{
										zhengBaCopyScene.FirstLeaveRoleId = player.ClientData.RoleID;
									}
								}
								else if (zhengBaCopyScene.m_eStatus == 2)
								{
									if (zhengBaCopyScene.Robot1 != null)
									{
										zhengBaCopyScene.Robot1.stopAttack();
									}
									if (zhengBaCopyScene.Robot2 != null)
									{
										zhengBaCopyScene.Robot2.stopAttack();
									}
									zhengBaCopyScene.Winner = 0;
									if (player.ClientData.RoleID == zhengBaCopyScene.RoleId1 && zhengBaCopyScene.RoleId2 > 0)
									{
										zhengBaCopyScene.Winner = zhengBaCopyScene.RoleId2;
									}
									else if (player.ClientData.RoleID == zhengBaCopyScene.RoleId2 && zhengBaCopyScene.RoleId1 > 0)
									{
										zhengBaCopyScene.Winner = zhengBaCopyScene.RoleId1;
									}
									zhengBaCopyScene.m_eStatus = 3;
								}
							}
						}
					}
				}
			}
		}

		private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int x = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].X;
				int y = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].Y;
				int radius = this._Config.BirthPointList[side % this._Config.BirthPointList.Count].Radius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, x, y, radius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		public void AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (sceneType == 36)
			{
				ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (zhengBaMatchConfig != null)
				{
					int fuBenSeqID = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					lock (this.Mutex)
					{
						ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene = null;
						if (!this.FuBenSeq2CopyScenes.TryGetValue(fuBenSeqID, out zhengBaCopyScene))
						{
							zhengBaCopyScene = new ZhengBaManager.ZhengBaCopyScene();
							zhengBaCopyScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
							zhengBaCopyScene.FuBenSeq = fuBenSeqID;
							zhengBaCopyScene.MapCode = mapCode;
							this.FuBenSeq2CopyScenes[fuBenSeqID] = zhengBaCopyScene;
						}
						if (zhengBaCopyScene.CopyMap == null)
						{
							copyMap.IsKuaFuCopy = true;
							copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((zhengBaMatchConfig.WaitSeconds + zhengBaMatchConfig.FightSeconds + zhengBaMatchConfig.ClearSeconds) * 1000));
							zhengBaCopyScene.CopyMap = copyMap;
						}
						if (zhengBaCopyScene.RoleId1 <= 0)
						{
							zhengBaCopyScene.RoleId1 = client.ClientData.RoleID;
							zhengBaCopyScene.IsMirror1 = false;
						}
						else if (zhengBaCopyScene.RoleId2 <= 0)
						{
							zhengBaCopyScene.RoleId2 = client.ClientData.RoleID;
							zhengBaCopyScene.IsMirror2 = false;
						}
					}
				}
			}
		}

		public void RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (copyMap != null && sceneType == 36)
			{
				lock (this.Mutex)
				{
					ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene = null;
					if (this.FuBenSeq2CopyScenes.TryGetValue(copyMap.FuBenSeqID, out zhengBaCopyScene))
					{
						this.FuBenSeq2CopyScenes.Remove(copyMap.FuBenSeqID);
						this.GameId2FuBenSeq.Remove(zhengBaCopyScene.GameId);
					}
				}
			}
		}

		private void ProcessEnd(ZhengBaManager.ZhengBaCopyScene scene, DateTime now, long nowTicks, int clearSec)
		{
			scene.m_eStatus = 4;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(clearSec * 1000);
			scene.StateTimeData.GameType = 12;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			if (scene.CopyMap != null)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			}
			if (scene.Robot1 != null)
			{
				scene.Robot1.stopAttack();
			}
			if (scene.Robot2 != null)
			{
				scene.Robot2.stopAttack();
			}
			List<ZhengBaNtfPkResultData> list = TianTiClient.getInstance().ZhengBaPkResult(scene.GameId, scene.Winner, scene.FirstLeaveRoleId);
			if (list != null)
			{
				foreach (ZhengBaNtfPkResultData zhengBaNtfPkResultData in list)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(zhengBaNtfPkResultData.RoleID);
					if (gameClient != null && gameClient.ClientData.MapCode == scene.MapCode)
					{
						gameClient.sendCmd<ZhengBaNtfPkResultData>(1358, zhengBaNtfPkResultData, false);
					}
				}
			}
		}

		public void UpdateCopyScene(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			if (num >= this.NextHeartBeatMs)
			{
				this.NextHeartBeatMs = num + 100L;
				ZhengBaMatchConfig zhengBaMatchConfig = this._Config.MatchConfigList.Find((ZhengBaMatchConfig _m) => _m.Day == this.SyncData.RealActDay);
				if (zhengBaMatchConfig != null)
				{
					lock (this.Mutex)
					{
						foreach (ZhengBaManager.ZhengBaCopyScene zhengBaCopyScene in this.FuBenSeq2CopyScenes.Values.ToList<ZhengBaManager.ZhengBaCopyScene>())
						{
							DateTime now = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (zhengBaCopyScene.m_eStatus == 0)
							{
								zhengBaCopyScene.m_lPrepareTime = num2;
								zhengBaCopyScene.m_lBeginTime = num2 + 30000L;
								zhengBaCopyScene.m_lEndTime = num2 + (long)(zhengBaMatchConfig.FightSeconds * 1000);
								zhengBaCopyScene.m_eStatus = 1;
								zhengBaCopyScene.StateTimeData.GameType = 12;
								zhengBaCopyScene.StateTimeData.State = zhengBaCopyScene.m_eStatus;
								zhengBaCopyScene.StateTimeData.EndTicks = zhengBaCopyScene.m_lBeginTime;
								if (zhengBaCopyScene.CopyMap != null)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengBaCopyScene.StateTimeData, zhengBaCopyScene.CopyMap);
								}
							}
							else if (zhengBaCopyScene.m_eStatus == 1)
							{
								if (zhengBaCopyScene.RoleId1 > 0 && zhengBaCopyScene.RoleId2 > 0)
								{
									zhengBaCopyScene.m_eStatus = 2;
									zhengBaCopyScene.StateTimeData.GameType = 12;
									zhengBaCopyScene.StateTimeData.State = zhengBaCopyScene.m_eStatus;
									zhengBaCopyScene.StateTimeData.EndTicks = zhengBaCopyScene.m_lEndTime;
									if (zhengBaCopyScene.CopyMap != null)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengBaCopyScene.StateTimeData, zhengBaCopyScene.CopyMap);
										zhengBaCopyScene.CopyMap.AddGuangMuEvent(1, 0);
										GameManager.ClientMgr.BroadSpecialMapAIEvent(zhengBaCopyScene.CopyMap.MapCode, zhengBaCopyScene.CopyMap.CopyMapID, 1, 0);
										zhengBaCopyScene.CopyMap.AddGuangMuEvent(2, 0);
										GameManager.ClientMgr.BroadSpecialMapAIEvent(zhengBaCopyScene.CopyMap.MapCode, zhengBaCopyScene.CopyMap.CopyMapID, 2, 0);
									}
									if (!zhengBaCopyScene.IsMirror1 || !zhengBaCopyScene.IsMirror2)
									{
										if (zhengBaCopyScene.IsMirror1)
										{
											GameClient gameClient = GameManager.ClientMgr.FindClient(zhengBaCopyScene.RoleId2);
											if (gameClient != null && gameClient.ClientData.MapCode == zhengBaCopyScene.MapCode)
											{
												zhengBaCopyScene.Robot1 = JingJiChangManager.getInstance().createRobot(gameClient, zhengBaCopyScene.JingJiData1, zhengBaCopyScene.MapCode);
												GameMap gameMap = GameManager.MapMgr.DictMaps[zhengBaCopyScene.MapCode];
												int side = 0;
												ZhengBaManager.GameSideInfo gameSideInfo = null;
												if (this.GameId2FuBenSeq.TryGetValue(zhengBaCopyScene.FuBenSeq, out gameSideInfo))
												{
													side = ++gameSideInfo.CurrSide;
												}
												int value;
												int value2;
												this.GetBirthPoint(zhengBaCopyScene.MapCode, side, out value, out value2);
												int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
												int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
												GameManager.MonsterZoneMgr.AddDynamicRobot(zhengBaCopyScene.MapCode, zhengBaCopyScene.Robot1, zhengBaCopyScene.CopyMap.CopyMapID, 1, gridX, gridY, 1, 0, 36, zhengBaCopyScene.RoleId2);
											}
										}
										else if (zhengBaCopyScene.IsMirror2)
										{
											GameClient gameClient2 = GameManager.ClientMgr.FindClient(zhengBaCopyScene.RoleId1);
											if (gameClient2 != null && gameClient2.ClientData.MapCode == zhengBaCopyScene.MapCode)
											{
												zhengBaCopyScene.Robot2 = JingJiChangManager.getInstance().createRobot(gameClient2, zhengBaCopyScene.JingJiData2, zhengBaCopyScene.MapCode);
												GameMap gameMap = GameManager.MapMgr.DictMaps[zhengBaCopyScene.MapCode];
												int side = 0;
												ZhengBaManager.GameSideInfo gameSideInfo = null;
												if (this.GameId2FuBenSeq.TryGetValue(zhengBaCopyScene.FuBenSeq, out gameSideInfo))
												{
													side = ++gameSideInfo.CurrSide;
												}
												int value;
												int value2;
												this.GetBirthPoint(zhengBaCopyScene.MapCode, side, out value, out value2);
												int gridX = gameMap.CorrectWidthPointToGridPoint(value) / gameMap.MapGridWidth;
												int gridY = gameMap.CorrectHeightPointToGridPoint(value2) / gameMap.MapGridHeight;
												GameManager.MonsterZoneMgr.AddDynamicRobot(zhengBaCopyScene.MapCode, zhengBaCopyScene.Robot2, zhengBaCopyScene.CopyMap.CopyMapID, 1, gridX, gridY, 1, 0, 36, zhengBaCopyScene.RoleId1);
											}
										}
									}
								}
								else if (num2 >= zhengBaCopyScene.m_lBeginTime)
								{
									zhengBaCopyScene.Winner = 0;
									if (zhengBaCopyScene.RoleId1 > 0 && zhengBaCopyScene.FirstLeaveRoleId != zhengBaCopyScene.RoleId1)
									{
										zhengBaCopyScene.Winner = zhengBaCopyScene.RoleId1;
									}
									else if (zhengBaCopyScene.RoleId2 > 0 && zhengBaCopyScene.FirstLeaveRoleId != zhengBaCopyScene.RoleId2)
									{
										zhengBaCopyScene.Winner = zhengBaCopyScene.RoleId2;
									}
									zhengBaCopyScene.m_eStatus = 3;
								}
							}
							else if (zhengBaCopyScene.m_eStatus == 2)
							{
								if (zhengBaCopyScene.FirstLeaveRoleId > 0)
								{
									zhengBaCopyScene.Winner = 0;
									zhengBaCopyScene.m_eStatus = 3;
								}
								else if (num2 >= zhengBaCopyScene.m_lEndTime)
								{
									zhengBaCopyScene.Winner = 0;
									if (zhengBaCopyScene.IsMirror1 && zhengBaCopyScene.IsMirror2)
									{
										TianTiPaiHangRoleData tianTiPaiHangRoleData = null;
										TianTiPaiHangRoleData tianTiPaiHangRoleData2 = null;
										if (this.RoleDataDict.TryGetValue(zhengBaCopyScene.RoleId1, out tianTiPaiHangRoleData) && this.RoleDataDict.TryGetValue(zhengBaCopyScene.RoleId2, out tianTiPaiHangRoleData2))
										{
											if (tianTiPaiHangRoleData.DuanWeiRank < tianTiPaiHangRoleData2.DuanWeiRank)
											{
												zhengBaCopyScene.Winner = tianTiPaiHangRoleData.RoleId;
											}
											else
											{
												zhengBaCopyScene.Winner = tianTiPaiHangRoleData2.RoleId;
											}
										}
									}
									else if (zhengBaCopyScene.IsMirror1 || zhengBaCopyScene.IsMirror2)
									{
										Robot robot = zhengBaCopyScene.IsMirror1 ? zhengBaCopyScene.Robot1 : zhengBaCopyScene.Robot2;
										GameClient gameClient3 = GameManager.ClientMgr.FindClient(zhengBaCopyScene.IsMirror1 ? zhengBaCopyScene.RoleId2 : zhengBaCopyScene.RoleId1);
										if (gameClient3 != null && robot != null)
										{
											int num3 = (int)RoleAlgorithm.GetMaxLifeV(gameClient3);
											if (num3 > 0 && robot.MonsterInfo.VLifeMax > 0.0)
											{
												if ((double)gameClient3.ClientData.CurrentLifeV * 1.0 / (double)num3 >= robot.VLife * 1.0 / robot.MonsterInfo.VLifeMax)
												{
													zhengBaCopyScene.Winner = gameClient3.ClientData.RoleID;
												}
												else
												{
													zhengBaCopyScene.Winner = robot.getRoleDataMini().RoleID;
												}
											}
											else
											{
												zhengBaCopyScene.Winner = gameClient3.ClientData.RoleID;
											}
										}
										else
										{
											zhengBaCopyScene.Winner = (zhengBaCopyScene.IsMirror1 ? zhengBaCopyScene.RoleId2 : zhengBaCopyScene.RoleId1);
										}
									}
									else
									{
										GameClient gameClient4 = GameManager.ClientMgr.FindClient(zhengBaCopyScene.RoleId1);
										GameClient gameClient5 = GameManager.ClientMgr.FindClient(zhengBaCopyScene.RoleId2);
										if (gameClient4 != null && gameClient5 != null)
										{
											int num4 = (int)RoleAlgorithm.GetMaxLifeV(gameClient4);
											int num5 = (int)RoleAlgorithm.GetMaxLifeV(gameClient5);
											if (num4 > 0 && num5 > 0)
											{
												if ((double)gameClient4.ClientData.CurrentLifeV * 1.0 / (double)num4 >= (double)gameClient5.ClientData.CurrentLifeV * 1.0 / (double)num5)
												{
													zhengBaCopyScene.Winner = gameClient4.ClientData.RoleID;
												}
												else
												{
													zhengBaCopyScene.Winner = gameClient5.ClientData.RoleID;
												}
											}
										}
									}
									zhengBaCopyScene.m_eStatus = 3;
								}
								else if (!zhengBaCopyScene.IsMirror1 || !zhengBaCopyScene.IsMirror2)
								{
									if (zhengBaCopyScene.Robot1 != null)
									{
										zhengBaCopyScene.Robot1.onUpdate();
									}
									else if (zhengBaCopyScene.Robot2 != null)
									{
										zhengBaCopyScene.Robot2.onUpdate();
									}
								}
							}
							else if (zhengBaCopyScene.m_eStatus == 3)
							{
								this.ProcessEnd(zhengBaCopyScene, now, num, zhengBaMatchConfig.ClearSeconds);
							}
							else if (zhengBaCopyScene.m_eStatus == 4)
							{
								if (num2 >= zhengBaCopyScene.m_lLeaveTime)
								{
									zhengBaCopyScene.m_eStatus = 5;
									if (zhengBaCopyScene.CopyMap != null)
									{
										zhengBaCopyScene.CopyMap.SetRemoveTicks(zhengBaCopyScene.m_lLeaveTime);
										try
										{
											List<GameClient> clientsList = zhengBaCopyScene.CopyMap.GetClientsList();
											if (clientsList != null && clientsList.Count > 0)
											{
												for (int i = 0; i < clientsList.Count; i++)
												{
													GameClient gameClient6 = clientsList[i];
													if (gameClient6 != null)
													{
														KuaFuManager.getInstance().GotoLastMap(gameClient6);
													}
												}
											}
										}
										catch (Exception ex)
										{
											DataHelper.WriteExceptionLogEx(ex, "众神争霸系统清场调度异常");
										}
									}
									else
									{
										this.FuBenSeq2CopyScenes.Remove(zhengBaCopyScene.FuBenSeq);
									}
								}
							}
						}
					}
				}
			}
		}

		private Dictionary<int, ZhengBaManager.ZhengBaCopyScene> FuBenSeq2CopyScenes = new Dictionary<int, ZhengBaManager.ZhengBaCopyScene>();

		private Dictionary<int, ZhengBaManager.GameSideInfo> GameId2FuBenSeq = new Dictionary<int, ZhengBaManager.GameSideInfo>();

		private long NextHeartBeatMs = TimeUtil.NOW();

		private ZhengBaConfig _Config = new ZhengBaConfig();

		private List<ZhengBaMatchAward> _MatchAwardList = new List<ZhengBaMatchAward>();

		private object Mutex = new object();

		private ZhengBaSyncData SyncData = new ZhengBaSyncData();

		private TimeSpan DiffKfCenter = TimeSpan.Zero;

		private Dictionary<int, TianTiPaiHangRoleData> RoleDataDict = new Dictionary<int, TianTiPaiHangRoleData>();

		private List<TianTiPaiHangRoleData> RoleDataList = new List<TianTiPaiHangRoleData>();

		private List<TianTiPaiHangRoleData> Top16RoleList = new List<TianTiPaiHangRoleData>();

		private List<ZhengBaSupportAnalysisData> SupportDatas = new List<ZhengBaSupportAnalysisData>();

		private Dictionary<int, PlayerJingJiData> MirrorDatas = new Dictionary<int, PlayerJingJiData>();

		private int MaxSupportGroup = 0;

		private int MaxOpposeGroup = 0;

		private Queue<ZhengBaPkLogData> PkLogQ = new Queue<ZhengBaPkLogData>();

		private Dictionary<int, Queue<ZhengBaSupportLogData>> SupportLogs = new Dictionary<int, Queue<ZhengBaSupportLogData>>();

		private List<ZhengBaWaitYaZhuAwardData> WaitAwardOfYaZhuList = new List<ZhengBaWaitYaZhuAwardData>();

		private int ZhengBaKingRoleId;

		private RoleData4Selector ZhengBaKingData;

		private class ZhengBaCopyScene
		{
			public int FuBenSeq;

			public int GameId;

			public int MapCode;

			public CopyMap CopyMap;

			public int RoleId1;

			public bool IsMirror1;

			public PlayerJingJiData JingJiData1;

			public Robot Robot1;

			public int RoleId2;

			public bool IsMirror2;

			public PlayerJingJiData JingJiData2;

			public Robot Robot2;

			public long m_lPrepareTime = 0L;

			public long m_lBeginTime = 0L;

			public long m_lEndTime = 0L;

			public long m_lLeaveTime = 0L;

			public GameSceneStatuses m_eStatus = 0;

			public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

			public int Winner = 0;

			public int FirstLeaveRoleId = 0;
		}

		private class GameSideInfo
		{
			public int FuBenSeq;

			public int CurrSide;
		}
	}
}
