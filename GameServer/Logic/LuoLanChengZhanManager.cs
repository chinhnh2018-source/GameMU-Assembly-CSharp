using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class LuoLanChengZhanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		public static LuoLanChengZhanManager getInstance()
		{
			return LuoLanChengZhanManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(700, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(701, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(702, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(706, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(708, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(709, 1, 1, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1342, 2, 2, LuoLanChengZhanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, LuoLanChengZhanManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, LuoLanChengZhanManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, LuoLanChengZhanManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, LuoLanChengZhanManager.getInstance());
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
			switch (nID)
			{
			case 700:
				return this.ProcessChengZhanJingJiaCmd(client, nID, bytes, cmdParams);
			case 701:
				return this.ProcessGetChengZhanDailyAwardsCmd(client, nID, bytes, cmdParams);
			case 702:
				return this.ProcessLuoLanChengZhanCmd(client, nID, bytes, cmdParams);
			case 703:
			case 704:
			case 705:
			case 707:
				break;
			case 706:
				return this.ProcessGetLuoLanChengZhuInfoCmd(client, nID, bytes, cmdParams);
			case 708:
				return this.ProcessLuoLanChengZhanRequestInfoListCmd(client, nID, bytes, cmdParams);
			case 709:
				return this.ProcessQueryZhanMengZiJinCmd(client, nID, bytes, cmdParams);
			default:
				if (nID == 1342)
				{
					return this.ProcessGetLuoLanKingLooks(client, nID, bytes, cmdParams);
				}
				break;
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			switch (eventObject.getEventType())
			{
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 22:
			{
				PreInstallJunQiEventObject preInstallJunQiEventObject = eventObject as PreInstallJunQiEventObject;
				if (null != preInstallJunQiEventObject)
				{
					this.OnPreInstallJunQi(preInstallJunQiEventObject.Player, preInstallJunQiEventObject.NPCID);
					eventObject.Handled = true;
				}
				break;
			}
			case 23:
			{
				PreBangHuiAddMemberEventObject preBangHuiAddMemberEventObject = eventObject as PreBangHuiAddMemberEventObject;
				if (null != preBangHuiAddMemberEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiAddMember(preBangHuiAddMemberEventObject);
				}
				break;
			}
			case 24:
			{
				PreBangHuiRemoveMemberEventObject preBangHuiRemoveMemberEventObject = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != preBangHuiRemoveMemberEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(preBangHuiRemoveMemberEventObject);
				}
				break;
			}
			}
		}

		public bool InitConfig()
		{
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.SiegeWarfareEveryDayAwardsDict.Clear();
					text = "Config/SiegeWarfareEveryDayAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						SiegeWarfareEveryDayAwardsItem siegeWarfareEveryDayAwardsItem = new SiegeWarfareEveryDayAwardsItem();
						siegeWarfareEveryDayAwardsItem.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						siegeWarfareEveryDayAwardsItem.ZhiWu = (int)Global.GetSafeAttributeLong(xml, "Status");
						siegeWarfareEveryDayAwardsItem.DayZhanGong = (int)Global.GetSafeAttributeLong(xml, "DayZhanGong");
						siegeWarfareEveryDayAwardsItem.DayExp = Global.GetSafeAttributeLong(xml, "DayExp");
						siegeWarfareEveryDayAwardsItem.DayGoods.AddNoRepeat(Global.GetSafeAttributeStr(xml, "DayGoods"));
						if (!this.RuntimeData.SiegeWarfareEveryDayAwardsDict.ContainsKey(siegeWarfareEveryDayAwardsItem.ZhiWu))
						{
							this.RuntimeData.SiegeWarfareEveryDayAwardsDict.Add(siegeWarfareEveryDayAwardsItem.ZhiWu, siegeWarfareEveryDayAwardsItem);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.MapBirthPointListDict.Clear();
					text = "Config/SiegeWarfareBirthPoint.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						MapBirthPoint mapBirthPoint = new MapBirthPoint();
						mapBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						mapBirthPoint.Type = (int)Global.GetSafeAttributeLong(xml, "Type");
						mapBirthPoint.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						mapBirthPoint.BirthPosX = (int)Global.GetSafeAttributeLong(xml, "BirthPosX");
						mapBirthPoint.BirthPosY = (int)Global.GetSafeAttributeLong(xml, "BirthPosY");
						mapBirthPoint.BirthRangeX = (int)Global.GetSafeAttributeLong(xml, "BirthRangeX");
						mapBirthPoint.BirthRangeY = (int)Global.GetSafeAttributeLong(xml, "BirthRangeY");
						List<MapBirthPoint> list;
						if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(mapBirthPoint.Type, out list))
						{
							list = new List<MapBirthPoint>();
							this.RuntimeData.MapBirthPointListDict.Add(mapBirthPoint.Type, list);
						}
						list.Add(mapBirthPoint);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					this.RuntimeData.QiZhiBuffOwnerDataList.Clear();
					this.RuntimeData.QiZhiBuffDisableParamsDict.Clear();
					this.RuntimeData.QiZhiBuffEnableParamsDict.Clear();
					text = "Config/QiZuoConfig.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						QiZhiConfig qiZhiConfig = new QiZhiConfig();
						qiZhiConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "NPCID");
						qiZhiConfig.BufferID = (int)Global.GetSafeAttributeLong(xml, "BufferID");
						qiZhiConfig.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						qiZhiConfig.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						List<int> list2 = Global.StringToIntList(Global.GetSafeAttributeStr(xml, "UseAuthority"), ',');
						foreach (int item in list2)
						{
							qiZhiConfig.UseAuthority.Add(item);
						}
						this.RuntimeData.NPCID2QiZhiConfigDict[qiZhiConfig.NPCID] = qiZhiConfig;
						this.RuntimeData.QiZhiBuffOwnerDataList.Add(new LuoLanChengZhanQiZhiBuffOwnerData
						{
							NPCID = qiZhiConfig.NPCID,
							OwnerBHName = ""
						});
						this.RuntimeData.QiZhiBuffDisableParamsDict[qiZhiConfig.BufferID] = new double[]
						{
							0.0,
							(double)qiZhiConfig.BufferID
						};
						this.RuntimeData.QiZhiBuffEnableParamsDict[qiZhiConfig.BufferID] = new double[]
						{
							0.0,
							(double)qiZhiConfig.BufferID
						};
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.MapCode = 0;
					this.RuntimeData.MapCode_LongTa = 0;
					QiZhiConfig qiZhiConfig2;
					if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.SuperQiZhiNpcId, out qiZhiConfig2))
					{
						this.RuntimeData.SuperQiZhiOwnerBirthPosX = qiZhiConfig2.PosX;
						this.RuntimeData.SuperQiZhiOwnerBirthPosY = qiZhiConfig2.PosY;
					}
					text = "Config/SiegeWarfare.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					using (IEnumerator<XElement> enumerator = enumerable.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							XElement xml = enumerator.Current;
							this.RuntimeData.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode1");
							this.RuntimeData.MapCode_LongTa = (int)Global.GetSafeAttributeLong(xml, "MapCode2");
							this.RuntimeData.GongNengOpenDaysFromKaiFu = (int)Global.GetSafeAttributeLong(xml, "KaiFuDay");
							this.RuntimeData.ApplyZhangMengZiJin = (long)((int)Global.GetSafeAttributeLong(xml, "ApplyZhangMengZiJin"));
							this.RuntimeData.BidZhangMengZiJin = (long)((int)Global.GetSafeAttributeLong(xml, "BidZhangMengZiJin"));
							this.RuntimeData.MaxZhanMengNum = (int)Global.GetSafeAttributeLong(xml, "MaxZhanMengNum");
							this.RuntimeData.WeekPoints = Global.String2IntArray(Global.GetSafeAttributeStr(xml, "WeekPoints"), '|');
							this.RuntimeData.TimePoints = DateTime.Parse(Global.GetSafeAttributeStr(xml, "TimePoints"));
							this.RuntimeData.EnrollTime = Global.GetSafeAttributeLong(xml, "EnrollTime");
							this.RuntimeData.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
							this.RuntimeData.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
							this.RuntimeData.MinRequestNum = (int)Global.GetSafeAttributeLong(xml, "MinRequestNum");
							this.RuntimeData.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
							this.RuntimeData.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
							this.RuntimeData.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
							this.RuntimeData.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
							this.RuntimeData.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
							this.RuntimeData.ExpAward = Global.GetSafeAttributeLong(xml, "ExpAward");
							this.RuntimeData.ZhanGongAward = (int)Global.GetSafeAttributeLong(xml, "ZhanGongAward");
							this.RuntimeData.ZiJin = (int)Global.GetSafeAttributeLong(xml, "ZiJin");
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					text = "Config/SiegeWarfareExp.xml";
					this._LevelAwardsMgr.LoadFromXMlFile(text, "", "ID", 0);
					this.ParseWeekDaysTimes();
					this.InitLuoLanChengZhuInfo();
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			return true;
		}

		public void LoadDataFromDB()
		{
			int roleID = 0;
			lock (this.RuntimeData.Mutex)
			{
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
				if (null != itemByLingDiID)
				{
					this.WangZuBHid = itemByLingDiID.BHID;
					this.WangZuBHName = (itemByLingDiID.BHName = this.UpdateWangZuBHNameFromDBServer(itemByLingDiID.BHID));
					if (itemByLingDiID.WarRequest != this.RuntimeData.WarRequestStr)
					{
						this.RuntimeData.WarRequstDict = this.GetWarRequstMap(itemByLingDiID.WarRequest);
						this.RuntimeData.WarRequestStr = itemByLingDiID.WarRequest;
					}
					BangHuiDetailData bangHuiDetailDataAuto = this.GetBangHuiDetailDataAuto(itemByLingDiID.BHID, -1);
					if (null != bangHuiDetailDataAuto)
					{
						roleID = bangHuiDetailDataAuto.BZRoleID;
					}
				}
				else
				{
					this.WangZuBHid = 0;
					this.WangZuBHName = "";
					this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
					this.RuntimeData.WarRequestStr = null;
				}
				this.RuntimeData.LongTaOwnerData.OwnerBHid = this.WangZuBHid;
				this.RuntimeData.LongTaOwnerData.OwnerBHName = this.WangZuBHName;
				this.RuntimeData.LuoLanChengZhuBHID = this.WangZuBHid;
				this.RuntimeData.LuoLanChengZhuBHName = this.WangZuBHName;
				this.ResetBHID2SiteDict();
			}
			this.ReShowLuolanKing(roleID);
		}

		private LuoLanChengZhuInfo GetLuoLanChengZhuInfo(GameClient client)
		{
			int roleID = 0;
			if (null != client)
			{
				roleID = client.ClientData.RoleID;
			}
			LuoLanChengZhuInfo luoLanChengZhuInfo = new LuoLanChengZhuInfo();
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
			LuoLanChengZhuInfo result;
			if (itemByLingDiID == null || itemByLingDiID.BHID <= 0)
			{
				result = luoLanChengZhuInfo;
			}
			else
			{
				BangHuiDetailData bangHuiDetailDataAuto = this.GetBangHuiDetailDataAuto(itemByLingDiID.BHID, roleID);
				if (null != bangHuiDetailDataAuto)
				{
					luoLanChengZhuInfo.BHID = bangHuiDetailDataAuto.BHID;
					luoLanChengZhuInfo.BHName = bangHuiDetailDataAuto.BHName;
					luoLanChengZhuInfo.ZoneID = bangHuiDetailDataAuto.ZoneID;
					if (null != bangHuiDetailDataAuto.MgrItemList)
					{
						foreach (BangHuiMgrItemData bangHuiMgrItemData in bangHuiDetailDataAuto.MgrItemList)
						{
							if (bangHuiMgrItemData.BHZhiwu == 1)
							{
								RoleDataEx kingRoleData = this.KingRoleData;
								if (kingRoleData != null && kingRoleData.RoleID == bangHuiMgrItemData.RoleID)
								{
									RoleData4Selector item = Global.RoleDataEx2RoleData4Selector(kingRoleData);
									luoLanChengZhuInfo.RoleInfoList.Add(item);
									luoLanChengZhuInfo.ZhiWuList.Add(bangHuiMgrItemData.BHZhiwu);
								}
							}
						}
					}
				}
				result = luoLanChengZhuInfo;
			}
			return result;
		}

		public BangHuiDetailData GetBangHuiDetailDataAuto(int bhid, int roleID = -1)
		{
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			if (null != bangHuiDetailData)
			{
				if (roleID <= 0 && bangHuiDetailData.BZRoleID > 0)
				{
					bangHuiDetailData = Global.GetBangHuiDetailData(bangHuiDetailData.BZRoleID, bhid, 0);
				}
			}
			return bangHuiDetailData;
		}

		public void ParseWeekDaysTimes()
		{
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.WeekPoints != null && this.RuntimeData.WeekPoints.Length > 0)
				{
					this.WangChengZhanWeekDaysByConfig = true;
				}
				string str = string.Format("{0}-{1}", this.RuntimeData.TimePoints.ToString("HH:mm"), this.RuntimeData.TimePoints.AddSeconds((double)this.RuntimeData.FightingSecs).ToString("HH:mm"));
				this.WangChengZhanFightingDayTimes = Global.ParseDateTimeRangeStr(str);
				this.RuntimeData.NoRequestTimeEnd = this.RuntimeData.TimePoints.AddSeconds((double)this.RuntimeData.FightingSecs).TimeOfDay;
				this.RuntimeData.NoRequestTimeStart = this.RuntimeData.TimePoints.AddSeconds((double)(-(double)this.RuntimeData.EnrollTime)).TimeOfDay;
				this.MaxTakingHuangGongSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("LuoLanHoldTime", -1);
				this.MaxTakingHuangGongSecs *= 1000;
			}
		}

		private void InitLuoLanChengZhuInfo()
		{
			this.LoadDataFromDB();
			HuodongCachingMgr.UpdateHeFuWCKingBHID(this.GetWangZuBHid());
			this.NotifyAllWangChengMapInfoData();
			FashionManager.getInstance().UpdateLuoLanChengZhuFasion(this.WangZuBHid);
			BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
			if (null != itemByLingDiID)
			{
			}
		}

		public void BangHuiLingDiItemsDictFromDBServer()
		{
			if (!this.IsInWangChengFightingTime(TimeUtil.NowDateTime()))
			{
				this.InitLuoLanChengZhuInfo();
			}
		}

		public string UpdateWangZuBHNameFromDBServer(int bhid)
		{
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, 0);
			string result;
			if (null == bangHuiMiniData)
			{
				result = GLang.GetLang(6, new object[0]);
			}
			else
			{
				result = bangHuiMiniData.BHName;
			}
			return result;
		}

		public int GetWangZuBHid()
		{
			return this.WangZuBHid;
		}

		public string GetWangZuBHName()
		{
			return this.WangZuBHName;
		}

		private bool IsDayOfWeek(int weekDayID)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (null == this.RuntimeData.WeekPoints)
				{
					return false;
				}
				for (int i = 0; i < this.RuntimeData.WeekPoints.Length; i++)
				{
					if (this.RuntimeData.WeekPoints[i] == weekDayID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsInWangChengFightingTime(DateTime now)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				int dayOfWeek = (int)now.DayOfWeek;
				if (!this.IsDayOfWeek(dayOfWeek))
				{
					result = false;
				}
				else
				{
					int num = 0;
					result = Global.JugeDateTimeInTimeRange(now, this.WangChengZhanFightingDayTimes, out num, false);
				}
			}
			return result;
		}

		public void GMStartHuoDongNow()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.WeekPoints[0] = (int)TimeUtil.NowDateTime().DayOfWeek;
					this.RuntimeData.TimePoints = TimeUtil.NowDateTime();
					this.ParseWeekDaysTimes();
				}
			}
			catch (Exception ex)
			{
			}
		}

		public void GMSetLuoLanChengZhu(int newBHid)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					this.LastTheOnlyOneBangHui = newBHid;
					this.RuntimeData.LongTaOwnerData.OwnerBHid = newBHid;
					this.RuntimeData.LongTaOwnerData.OwnerBHName = this.UpdateWangZuBHNameFromDBServer(newBHid);
					this.WangZuBHid = this.RuntimeData.LongTaOwnerData.OwnerBHid;
					this.WangZuBHName = this.RuntimeData.LongTaOwnerData.OwnerBHName;
					this.HandleHuangChengResultEx(true);
					this.NotifyAllWangChengMapInfoData();
				}
			}
			catch (Exception ex)
			{
			}
		}

		public bool IsWangChengZhanOver()
		{
			return !this.WaitingHuangChengResult;
		}

		public bool IsInBattling()
		{
			return WangChengZhanStates.None != this.WangChengZhanState;
		}

		private void NotifyAllLuoLanChengZhanJingJiaResult()
		{
			lock (this.RuntimeData.Mutex)
			{
				bool flag2 = this.CanRequest();
				if (this.RuntimeData.CanRequestState != flag2)
				{
					this.RuntimeData.CanRequestState = flag2;
					if (!flag2)
					{
						string text = GLang.GetLang(40, new object[0]);
						List<LuoLanChengZhanRequestInfoEx> list = this.GetWarRequestInfoList();
						list = list.FindAll((LuoLanChengZhanRequestInfoEx x) => x.BHID > 0);
						for (int i = 0; i < list.Count; i++)
						{
							text += string.Format(GLang.GetLang(41, new object[0]), this.GetBHName(list[i].BHID));
							if (i < list.Count - 1)
							{
								text += GLang.GetLang(42, new object[0]);
							}
						}
						if (list.Count > 0)
						{
							Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.Bulletin, text, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
						}
					}
				}
			}
		}

		public void ProcessWangChengZhanResult()
		{
			try
			{
				if (Global.GetBangHuiFightingLineID() == GameManager.ServerLineID)
				{
					Global.UpdateLuoLanChengZhanWeekDays(false);
					DateTime now = TimeUtil.NowDateTime();
					if (WangChengZhanStates.None == this.WangChengZhanState)
					{
						if (this.IsInWangChengFightingTime(now))
						{
							this._MapEventMgr.ClearAllMapEvents();
							this.WangChengZhanState = WangChengZhanStates.Fighting;
							this.BangHuiTakeHuangGongTicks = now.Ticks;
							this.RuntimeData.FightEndTime = now.AddSeconds((double)this.RuntimeData.FightingSecs);
							this.WaitingHuangChengResult = true;
							this.RuntimeData.SuperQiZhiOwnerBhid = 0;
							this.NotifyAllWangChengMapInfoData();
							Global.BroadcastHuangChengBattleStart();
						}
						else
						{
							this.ClearMapClients(false);
							this.NotifyAllLuoLanChengZhanJingJiaResult();
						}
					}
					else
					{
						this.UpdateQiZhiBuffParams(now);
						if (this.IsInWangChengFightingTime(now))
						{
							bool flag = this.TryGenerateNewHuangChengBangHui();
							if (flag)
							{
								this.HandleHuangChengResultEx(false);
							}
							else
							{
								this.ProcessTimeAddRoleExp();
							}
						}
						else
						{
							this.ClearMapClients(true);
							this.WangChengZhanState = WangChengZhanStates.None;
							this.WaitingHuangChengResult = false;
							this.TryGenerateNewHuangChengBangHui();
							this.HandleHuangChengResultEx(true);
							JunQiManager.ProcessDelAllJunQiByMapCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this.RuntimeData.MapCode);
							this.NotifyAllWangChengMapInfoData();
							this.GiveLuoLanChengZhanAwards();
							this.ResetRequestInfo();
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		private void UpdateQiZhiBuffParams(DateTime now)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (int key in this.RuntimeData.QiZhiBuffEnableParamsDict.Keys)
				{
					this.RuntimeData.QiZhiBuffEnableParamsDict[key][0] = (double)((int)(this.RuntimeData.FightEndTime - now).TotalSeconds);
				}
			}
		}

		private void GiveLuoLanChengZhanAwards()
		{
			LuoLanChengZhanResultInfo luoLanChengZhanResultInfo = new LuoLanChengZhanResultInfo();
			LuoLanChengZhanResultInfo luoLanChengZhanResultInfo2 = new LuoLanChengZhanResultInfo();
			luoLanChengZhanResultInfo.BHID = (luoLanChengZhanResultInfo2.BHID = this.WangZuBHid);
			luoLanChengZhanResultInfo.BHName = (luoLanChengZhanResultInfo2.BHName = this.WangZuBHName);
			luoLanChengZhanResultInfo.ExpAward = this.RuntimeData.ExpAward;
			luoLanChengZhanResultInfo.ZhanGongAward = this.RuntimeData.ZhanGongAward;
			luoLanChengZhanResultInfo.ZhanMengZiJin = this.RuntimeData.ZiJin;
			luoLanChengZhanResultInfo2.ExpAward = this.RuntimeData.ExpAward / 2L;
			luoLanChengZhanResultInfo2.ZhanGongAward = this.RuntimeData.ZhanGongAward / 2;
			luoLanChengZhanResultInfo2.ZhanMengZiJin = this.RuntimeData.ZiJin / 2;
			GameClient firstClient = GameManager.ClientMgr.GetFirstClient();
			lock (this.RuntimeData.Mutex)
			{
				foreach (LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo in this.RuntimeData.WarRequstDict.Values)
				{
					int bhid = luoLanChengZhanRequestInfo.BHID;
					int zhanMengZiJin;
					if (luoLanChengZhanRequestInfo.BHID == this.WangZuBHid)
					{
						zhanMengZiJin = luoLanChengZhanResultInfo.ZhanMengZiJin;
					}
					else
					{
						zhanMengZiJin = luoLanChengZhanResultInfo2.ZhanMengZiJin;
					}
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(luoLanChengZhanRequestInfo.BHID, 0);
					if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, firstClient, bhid, zhanMengZiJin))
					{
						LogManager.WriteLog(3, string.Format("罗兰城战奖励战盟资金失败,bhid={0}, bidMoney={1}", bhid, zhanMengZiJin), null, true);
					}
				}
			}
			List<object> list = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
			if (null == list)
			{
				list = new List<object>();
			}
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
			list.AddRange(mapClients);
			if (list != null && list.Count > 0)
			{
				byte[] array = DataHelper.ObjectToBytes<LuoLanChengZhanResultInfo>(luoLanChengZhanResultInfo);
				TCPOutPacket tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 707);
				byte[] array2 = DataHelper.ObjectToBytes<LuoLanChengZhanResultInfo>(luoLanChengZhanResultInfo2);
				TCPOutPacket tcpoutPacket2 = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array2, 0, array2.Length, 707);
				for (int i = 0; i < list.Count; i++)
				{
					GameClient gameClient = list[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.Faction == this.WangZuBHid)
						{
							GameManager.ClientMgr.ProcessRoleExperience(gameClient, luoLanChengZhanResultInfo.ExpAward, true, true, false, "none");
							int zhanGongAward = luoLanChengZhanResultInfo.ZhanGongAward;
							GameManager.ClientMgr.AddBangGong(gameClient, ref zhanGongAward, AddBangGongTypes.BG_ChengZhan, 0);
							gameClient.sendCmd(tcpoutPacket, false);
						}
						else
						{
							GameManager.ClientMgr.ProcessRoleExperience(gameClient, luoLanChengZhanResultInfo2.ExpAward, true, true, false, "none");
							int zhanGongAward = luoLanChengZhanResultInfo2.ZhanGongAward;
							GameManager.ClientMgr.AddBangGong(gameClient, ref zhanGongAward, AddBangGongTypes.BG_ChengZhan, 0);
							gameClient.sendCmd(tcpoutPacket2, false);
						}
					}
				}
				Global.PushBackTcpOutPacket(tcpoutPacket);
				Global.PushBackTcpOutPacket(tcpoutPacket2);
			}
		}

		public void ResetRequestInfo()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
				this.RuntimeData.WarRequestStr = this.GeWarRequstString(this.RuntimeData.WarRequstDict);
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
				if (null != itemByLingDiID)
				{
					itemByLingDiID.WarRequest = this.RuntimeData.WarRequestStr;
					this.SetCityWarRequestToDBServer(itemByLingDiID.LingDiID, itemByLingDiID.WarRequest);
				}
				this.ResetBHID2SiteDict();
				this.RuntimeData.LongTaBHRoleCountList.Clear();
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID = 0;
					this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHName = "";
				}
			}
		}

		public bool TryGenerateNewHuangChengBangHui()
		{
			List<string> list = new List<string>();
			int theOnlyOneBangHui = this.GetTheOnlyOneBangHui(list);
			if (!this.WaitingHuangChengResult)
			{
				foreach (string text in list)
				{
					LogManager.WriteLog(2, text, null, true);
				}
			}
			this.NotifyLongTaRoleDataList();
			this.NotifyLongTaOwnerData();
			lock (this.RuntimeData.Mutex)
			{
				if (theOnlyOneBangHui <= 0 || theOnlyOneBangHui == this.RuntimeData.LongTaOwnerData.OwnerBHid)
				{
					this.LastTheOnlyOneBangHui = -1;
					return false;
				}
				if (this.LastTheOnlyOneBangHui != theOnlyOneBangHui)
				{
					this.LastTheOnlyOneBangHui = theOnlyOneBangHui;
					this.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
					return false;
				}
				if (this.LastTheOnlyOneBangHui > 0)
				{
					long num = TimeUtil.NOW();
					if (num - this.BangHuiTakeHuangGongTicks > (long)this.MaxTakingHuangGongSecs)
					{
						this.RuntimeData.LongTaOwnerData.OwnerBHid = this.LastTheOnlyOneBangHui;
						this.RuntimeData.LongTaOwnerData.OwnerBHName = this.UpdateWangZuBHNameFromDBServer(theOnlyOneBangHui);
						if (this.WaitingHuangChengResult)
						{
							foreach (string text in list)
							{
								LogManager.WriteLog(2, text, null, true);
							}
						}
						return true;
					}
				}
			}
			return false;
		}

		public int GetTheOnlyOneBangHui(List<string> logList)
		{
			List<GameClient> list = new List<GameClient>();
			List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(this.RuntimeData.MapCode_LongTa);
			int num = -1;
			int result;
			if (null == mapGameClients)
			{
				result = num;
			}
			else
			{
				int mapCode_LongTa = this.RuntimeData.MapCode_LongTa;
				foreach (GameClient gameClient in mapGameClients)
				{
					bool flag = false;
					GameClient gameClient;
					if (gameClient.ClientData.CurrentLifeV > 0)
					{
						if (!gameClient.ClientData.WaitingNotifyChangeMap && !gameClient.ClientData.WaitingForChangeMap)
						{
							if (gameClient.ClientData.MapCode == mapCode_LongTa && Global.IsPosReachable(mapCode_LongTa, gameClient.ClientData.PosX, gameClient.ClientData.PosY))
							{
								flag = true;
								list.Add(gameClient);
								string item = string.Format("龙塔地图有效玩家#bhid={8},client={6}({7}),mapCode:{0},lifev={9},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode_LongTa,
									gameClient.ClientData.MapCode,
									gameClient.ClientData.WaitingNotifyChangeMap,
									gameClient.ClientData.WaitingForChangeMap,
									gameClient.ClientData.PosX,
									gameClient.ClientData.PosY,
									gameClient.ClientData.RoleID,
									gameClient.ClientData.RoleName,
									gameClient.ClientData.Faction,
									gameClient.ClientData.CurrentLifeV
								});
								logList.Add(item);
							}
						}
					}
					if (!flag)
					{
						string item = string.Format("龙塔地图无效玩家#bhid={8},client={6}({7}),mapCode:{0},lifev={9},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
						{
							mapCode_LongTa,
							gameClient.ClientData.MapCode,
							gameClient.ClientData.WaitingNotifyChangeMap,
							gameClient.ClientData.WaitingForChangeMap,
							gameClient.ClientData.PosX,
							gameClient.ClientData.PosY,
							gameClient.ClientData.RoleID,
							gameClient.ClientData.RoleName,
							gameClient.ClientData.Faction,
							gameClient.ClientData.CurrentLifeV
						});
						logList.Add(item);
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					List<LuoLanChengZhanRoleCountData> list2 = new List<LuoLanChengZhanRoleCountData>(this.RuntimeData.MaxZhanMengNum);
					for (int i = 0; i < list.Count; i++)
					{
						GameClient gameClient = list[i];
						int bhid = gameClient.ClientData.Faction;
						if (bhid > 0)
						{
							LuoLanChengZhanRoleCountData luoLanChengZhanRoleCountData = list2.Find((LuoLanChengZhanRoleCountData x) => x.BHID == bhid);
							if (null == luoLanChengZhanRoleCountData)
							{
								list2.Add(new LuoLanChengZhanRoleCountData
								{
									BHID = bhid,
									RoleCount = 1
								});
							}
							else
							{
								luoLanChengZhanRoleCountData.RoleCount++;
							}
						}
					}
					this.RuntimeData.LongTaBHRoleCountList = list2;
					if (list2.Count == 1)
					{
						num = list2[0].BHID;
					}
				}
				result = num;
			}
			return result;
		}

		private List<LuoLanChengZhanRequestInfoEx> GetWarRequestInfoList()
		{
			List<LuoLanChengZhanRequestInfoEx> list = new List<LuoLanChengZhanRequestInfoEx>();
			lock (this.RuntimeData.Mutex)
			{
				foreach (LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo in this.RuntimeData.WarRequstDict.Values)
				{
					list.Add(new LuoLanChengZhanRequestInfoEx
					{
						Site = luoLanChengZhanRequestInfo.Site,
						BHID = luoLanChengZhanRequestInfo.BHID,
						BHName = this.GetBHName(luoLanChengZhanRequestInfo.BHID),
						BidMoney = luoLanChengZhanRequestInfo.BidMoney
					});
				}
			}
			return list;
		}

		public void NotifyAllWangChengMapInfoData()
		{
			WangChengMapInfoData wangChengMapInfoData = this.FormatWangChengMapInfoData();
			GameManager.ClientMgr.NotifyAllWangChengMapInfoData(wangChengMapInfoData);
		}

		public void NotifyLongTaRoleDataList()
		{
			byte[] array;
			lock (this.RuntimeData.Mutex)
			{
				array = DataHelper.ObjectToBytes<List<LuoLanChengZhanRoleCountData>>(this.RuntimeData.LongTaBHRoleCountList);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 703);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		public void NotifyLongTaOwnerData()
		{
			byte[] array;
			lock (this.RuntimeData.Mutex)
			{
				array = DataHelper.ObjectToBytes<LuoLanChengZhanLongTaOwnerData>(this.RuntimeData.LongTaOwnerData);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 705);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		public void UpdateQiZhiBangHui(int npcExtentionID, int bhid, string bhName)
		{
			int num = 0;
			int num2 = 0;
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					if (this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID == npcExtentionID)
					{
						num = this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID;
						this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID = bhid;
						this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHName = bhName;
						break;
					}
				}
				QiZhiConfig qiZhiConfig;
				if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out qiZhiConfig))
				{
					num2 = qiZhiConfig.BufferID;
				}
			}
			if (bhid != num)
			{
				if (npcExtentionID == this.RuntimeData.SuperQiZhiNpcId)
				{
					this.RuntimeData.SuperQiZhiOwnerBhid = bhid;
				}
				try
				{
					List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
					List<object> mapClients2 = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
					mapClients.AddRange(mapClients2);
					EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(num2);
					if (null != equipPropItem)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								bool add = false;
								if (gameClient.ClientData.Faction == num)
								{
									add = false;
								}
								else if (gameClient.ClientData.Faction == bhid)
								{
									add = true;
								}
								this.UpdateQiZhiBuff4GameClient(gameClient, equipPropItem, num2, add);
							}
						}
					}
					this.NotifyQiZhiBuffOwnerDataList();
				}
				catch (Exception ex)
				{
					LogManager.WriteException("旗帜状态变化,设置旗帜Buff时发生异常:" + ex.ToString());
				}
			}
		}

		private void UpdateQiZhiBuff4GameClient(GameClient client, EquipPropItem item, int bufferID, bool add)
		{
			try
			{
				if (add)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						item.ExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffEnableParamsDict[bufferID], 1, true);
				}
				else
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						PropsCacheManager.ConstExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffDisableParamsDict[bufferID], 1, true);
				}
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void NotifyQiZhiBuffOwnerDataList()
		{
			byte[] array;
			lock (this.RuntimeData.Mutex)
			{
				array = DataHelper.ObjectToBytes<List<LuoLanChengZhanQiZhiBuffOwnerData>>(this.RuntimeData.QiZhiBuffOwnerDataList);
			}
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 704);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode, -1, false);
			GameManager.ClientMgr.BroadSpecialMapMessage(tcpOutPacket, this.RuntimeData.MapCode_LongTa, -1, true);
		}

		private void HandleHuangChengResultEx(bool isBattleOver = false)
		{
			if (isBattleOver)
			{
				BangHuiDetailData oldBangHuiDetailData = (this.WangZuBHid > 0) ? this.GetBangHuiDetailDataAuto(this.WangZuBHid, -1) : null;
				BangHuiDetailData newBangHuiDetailData = (this.RuntimeData.LongTaOwnerData.OwnerBHid > 0) ? this.GetBangHuiDetailDataAuto(this.RuntimeData.LongTaOwnerData.OwnerBHid, -1) : null;
				EventLogManager.AddLuoLanChengZhanEvent(oldBangHuiDetailData, newBangHuiDetailData);
				this.WangZuBHid = this.RuntimeData.LongTaOwnerData.OwnerBHid;
				this.WangZuBHName = this.RuntimeData.LongTaOwnerData.OwnerBHName;
				if (this.WangZuBHid <= 0)
				{
					JunQiManager.HandleLuoLanChengZhanResult(7, this.RuntimeData.MapCode, 0, "", true, false);
					JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
					Global.BroadcastWangChengFailedHint();
					this.ClearDbKingNpc();
					this.InitLuoLanChengZhuInfo();
					return;
				}
				JunQiManager.HandleLuoLanChengZhanResult(7, this.RuntimeData.MapCode, this.WangZuBHid, this.WangZuBHName, true, false);
				JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
				this.ClearDbKingNpc();
				this.InitLuoLanChengZhuInfo();
				HeFuLuoLanActivity heFuLuoLanActivity = HuodongCachingMgr.GetHeFuLuoLanActivity();
				if (heFuLuoLanActivity != null && heFuLuoLanActivity.InActivityTime())
				{
					string text = GameManager.GameConfigMgr.GetGameConfigItemStr("hefu_luolan_guildid", "");
					if (text.Split(new char[]
					{
						'|'
					}).Length < 2)
					{
						if (text.Length > 0)
						{
							text += "|";
						}
						int num = 0;
						BangHuiDetailData bangHuiDetailDataAuto = this.GetBangHuiDetailDataAuto(this.WangZuBHid, -1);
						if (null != bangHuiDetailDataAuto)
						{
							num = bangHuiDetailDataAuto.BZRoleID;
						}
						text = text + this.WangZuBHid.ToString() + "," + num.ToString();
						Global.UpdateDBGameConfigg("hefu_luolan_guildid", text);
					}
				}
			}
			if (this.LastTheOnlyOneBangHui > 0)
			{
				Global.BroadcastHuangChengOkHintEx(this.RuntimeData.LongTaOwnerData.OwnerBHName, isBattleOver);
			}
		}

		public void NotifyClientWangChengMapInfoData(GameClient client)
		{
			WangChengMapInfoData wangChengMapInfoData = this.GetWangChengMapInfoData(client);
			GameManager.ClientMgr.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
		}

		public WangChengMapInfoData GetWangChengMapInfoData(GameClient client)
		{
			return this.FormatWangChengMapInfoData();
		}

		public WangChengMapInfoData FormatWangChengMapInfoData()
		{
			string nextBattleTime = GLang.GetLang(43, new object[0]);
			long fightingEndTime = 0L;
			if (WangChengZhanStates.None == this.WangChengZhanState)
			{
				nextBattleTime = this.GetNextCityBattleTime();
			}
			else
			{
				fightingEndTime = this.GetBattleEndMs();
			}
			return new WangChengMapInfoData
			{
				FightingEndTime = fightingEndTime,
				FightingState = (this.WaitingHuangChengResult ? 1 : 0),
				NextBattleTime = nextBattleTime,
				WangZuBHName = this.WangZuBHName,
				WangZuBHid = this.WangZuBHid
			};
		}

		public bool ProcessChengZhanJingJiaCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				if (num2 < 1 || num2 > this.RuntimeData.MaxZhanMengNum)
				{
					num = -18;
				}
				else if (!this.CanRequest())
				{
					num = -2001;
				}
				else if (faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					num = -1002;
				}
				else
				{
					int num4 = -1;
					int num5 = 0;
					lock (this.RuntimeData.Mutex)
					{
						if (this.WangZuBHid == faction)
						{
							num = -5;
							goto IL_34C;
						}
						BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
						if (null != itemByLingDiID)
						{
							if (itemByLingDiID.WarRequest != this.RuntimeData.WarRequestStr)
							{
								this.RuntimeData.WarRequstDict = this.GetWarRequstMap(itemByLingDiID.WarRequest);
								this.RuntimeData.WarRequestStr = itemByLingDiID.WarRequest;
							}
						}
						else
						{
							this.RuntimeData.WarRequstDict = new Dictionary<int, LuoLanChengZhanRequestInfo>();
							this.RuntimeData.WarRequestStr = null;
						}
						int num6;
						if (this.RuntimeData.BHID2SiteDict.TryGetValue(faction, out num6) && num6 != num2)
						{
							num = -1004;
							goto IL_34C;
						}
						LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo;
						if (!this.RuntimeData.WarRequstDict.TryGetValue(num2, out luoLanChengZhanRequestInfo))
						{
							luoLanChengZhanRequestInfo = new LuoLanChengZhanRequestInfo();
							luoLanChengZhanRequestInfo.Site = num2;
							this.RuntimeData.WarRequstDict.Add(num2, luoLanChengZhanRequestInfo);
						}
						else
						{
							num4 = luoLanChengZhanRequestInfo.BHID;
							num5 = luoLanChengZhanRequestInfo.BidMoney;
						}
						if ((long)num3 < (long)num5 + this.RuntimeData.BidZhangMengZiJin)
						{
							num = -4;
							goto IL_34C;
						}
						int num7 = 0;
						int subMoney;
						if (num4 == faction)
						{
							subMoney = num3 - num5;
						}
						else
						{
							subMoney = num3;
						}
						if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, out num7))
						{
							num = -9;
							goto IL_34C;
						}
						luoLanChengZhanRequestInfo.BHID = faction;
						luoLanChengZhanRequestInfo.BidMoney = num3;
						this.RuntimeData.WarRequestStr = this.GeWarRequstString(this.RuntimeData.WarRequstDict);
						itemByLingDiID.WarRequest = this.RuntimeData.WarRequestStr;
						this.SetCityWarRequestToDBServer(itemByLingDiID.LingDiID, itemByLingDiID.WarRequest);
						this.ResetBHID2SiteDict();
					}
					if (num4 != faction && num4 > 0 && num5 > 0)
					{
						if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num4, num5))
						{
							LogManager.WriteLog(3, string.Format("返还罗兰城战竞价资金失败,bhid={0}, bidMoney={1}", num4, num5), null, true);
						}
					}
					GameManager.ClientMgr.NotifyAllLuoLanChengZhanRequestInfoList(this.GetWarRequestInfoList());
				}
				IL_34C:
				client.sendCmd(nID, string.Format("{0}", num), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessLuoLanChengZhanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				int unionLevel = Global.GetUnionLevel(client, false);
				if (unionLevel < Global.GetUnionLevel(this.RuntimeData.MinZhuanSheng, this.RuntimeData.MinLevel, false))
				{
					num = -19;
				}
				else if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					num = -2001;
				}
				else if (faction <= 0)
				{
					num = -1000;
				}
				else
				{
					bool flag = false;
					lock (this.RuntimeData.Mutex)
					{
						if (faction == this.WangZuBHid)
						{
							flag = true;
						}
						else
						{
							foreach (LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo in this.RuntimeData.WarRequstDict.Values)
							{
								if (luoLanChengZhanRequestInfo.BHID == faction)
								{
									flag = true;
									break;
								}
							}
						}
					}
					int toMapCode;
					int maxX;
					int mapY;
					if (!flag)
					{
						num = -1003;
					}
					else if (!this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out toMapCode, out maxX, out mapY))
					{
						num = -3;
					}
					else
					{
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, maxX, mapY, -1, 0);
					}
				}
				client.sendCmd(nID, string.Format("{0}", num), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetChengZhanDailyAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int faction = client.ClientData.Faction;
				int num3 = 7;
				if (faction <= 0 || client.ClientData.Faction != faction)
				{
					num = -12;
				}
				else if (7 != num3)
				{
					num = -13;
				}
				else
				{
					BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(num3);
					SiegeWarfareEveryDayAwardsItem siegeWarfareEveryDayAwardsItem;
					if (itemByLingDiID.BHID != faction)
					{
						num = -12;
					}
					else if (this.IsInBattling())
					{
						num = -2002;
					}
					else if (!this.RuntimeData.SiegeWarfareEveryDayAwardsDict.TryGetValue(client.ClientData.BHZhiWu, out siegeWarfareEveryDayAwardsItem))
					{
						num = -1005;
					}
					else
					{
						int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "SiegeWarfareEveryDayAwardDayID");
						if (roleParamsInt32FromDB == Global.GetOffsetDayNow())
						{
							num = -200;
						}
						else
						{
							List<GoodsData> list = Global.ConvertToGoodsDataList(siegeWarfareEveryDayAwardsItem.DayGoods.Items, -1);
							if (Global.CanAddGoodsDataList(client, list))
							{
								for (int i = 0; i < list.Count; i++)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "罗兰城战胜利战盟每日奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
									GoodsData goodsData = list[i];
									GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "罗兰城战胜利战盟每日奖励", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								}
							}
							else
							{
								num = -100;
							}
							long dayExp = siegeWarfareEveryDayAwardsItem.DayExp;
							int dayZhanGong = siegeWarfareEveryDayAwardsItem.DayZhanGong;
							if (num >= 0)
							{
								Global.SaveRoleParamsInt32ValueToDB(client, "SiegeWarfareEveryDayAwardDayID", Global.GetOffsetDayNow(), true);
								if (dayExp > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, dayExp, true, true, false, "none");
									long experience = client.ClientData.Experience;
									GameManager.SystemServerEvents.AddEvent(string.Format("角色根据领地特权领取经验, roleID={0}({1}), exp={2}, newExp={3}, bhid={4}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										dayExp,
										dayExp,
										faction
									}), EventLevels.Record);
								}
								if (dayZhanGong > 0)
								{
									if (GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref dayZhanGong, AddBangGongTypes.BG_ChengZhan, 0))
									{
										if (0 != dayZhanGong)
										{
											GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战功", "罗兰城战每日奖励", "系统", client.ClientData.RoleName, "增加", dayZhanGong, client.ClientData.ZoneID, client.strUserID, client.ClientData.BangGong, client.ServerId, null);
										}
									}
								}
							}
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}", num), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetLuoLanChengZhuInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				LuoLanChengZhuInfo luoLanChengZhuInfo = this.GetLuoLanChengZhuInfo(client);
				if (client.ClientData.Faction == luoLanChengZhuInfo.BHID && luoLanChengZhuInfo.BHID > 0)
				{
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "SiegeWarfareEveryDayAwardDayID");
					if (roleParamsInt32FromDB != Global.GetOffsetDayNow())
					{
						luoLanChengZhuInfo.isGetReward = false;
					}
				}
				client.sendCmd<LuoLanChengZhuInfo>(nID, luoLanChengZhuInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessLuoLanChengZhanRequestInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GameManager.ClientMgr.NotifyLuoLanChengZhanRequestInfoList(client, this.GetWarRequestInfoList());
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessQueryZhanMengZiJinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, client.ClientData.Faction);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetLuoLanKingLooks(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[1]);
				RoleDataEx kingRoleData = this.KingRoleData;
				if (kingRoleData != null && kingRoleData.RoleID == num)
				{
					RoleData4Selector cmdData = Global.RoleDataEx2RoleData4Selector(kingRoleData);
					client.sendCmd<RoleData4Selector>(nID, cmdData, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public Dictionary<int, LuoLanChengZhanRequestInfo> GetWarRequstMap(string warReqString)
		{
			Dictionary<int, LuoLanChengZhanRequestInfo> dictionary = null;
			try
			{
				byte[] array = Convert.FromBase64String(warReqString);
				dictionary = DataHelper.BytesToObject<Dictionary<int, LuoLanChengZhanRequestInfo>>(array, 0, array.Length);
			}
			catch (Exception ex)
			{
			}
			if (null == dictionary)
			{
				dictionary = new Dictionary<int, LuoLanChengZhanRequestInfo>();
			}
			return dictionary;
		}

		public string GeWarRequstString(Dictionary<int, LuoLanChengZhanRequestInfo> warRequstMap)
		{
			string result = "";
			try
			{
				byte[] inArray = DataHelper.ObjectToBytes<Dictionary<int, LuoLanChengZhanRequestInfo>>(warRequstMap);
				return Convert.ToBase64String(inArray);
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		public int SetCityWarRequestToDBServer(int lingDiID, string nowWarRequest)
		{
			int num = -200;
			string strcmd = string.Format("{0}:{1}", lingDiID, nowWarRequest);
			string[] array = Global.ExecuteDBCmd(10098, strcmd, 0);
			int result;
			if (array == null || array.Length != 5)
			{
				result = num;
			}
			else
			{
				num = Global.SafeConvertToInt32(array[0]);
				JunQiManager.NotifySyncBangHuiLingDiItemsDict();
				result = num;
			}
			return result;
		}

		public bool CanRequest()
		{
			DateTime d = TimeUtil.NowDateTime();
			bool result;
			if ((d - Global.GetKaiFuTime()).TotalDays < (double)this.RuntimeData.GongNengOpenDaysFromKaiFu)
			{
				result = false;
			}
			else
			{
				if (this.IsDayOfWeek((int)d.DayOfWeek))
				{
					TimeSpan timeOfDay = d.TimeOfDay;
					if (timeOfDay >= this.RuntimeData.NoRequestTimeStart && timeOfDay <= this.RuntimeData.NoRequestTimeEnd)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		private void ResetBHID2SiteDict()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.BHID2SiteDict.Clear();
				if (this.WangZuBHid > 0)
				{
					this.RuntimeData.BHID2SiteDict[this.WangZuBHid] = 0;
				}
				foreach (LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo in this.RuntimeData.WarRequstDict.Values)
				{
					this.RuntimeData.BHID2SiteDict[luoLanChengZhanRequestInfo.BHID] = luoLanChengZhanRequestInfo.Site;
				}
			}
		}

		public bool IsExistCityWarToday()
		{
			bool result;
			if (!this.IsDayOfWeek((int)TimeUtil.NowDateTime().DayOfWeek))
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.WarRequstDict.Count == 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public long GetBattleEndMs()
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int hour = dateTime.Hour;
			int minute = dateTime.Minute;
			int num = hour * 60 + minute;
			int num2 = 0;
			Global.JugeDateTimeInTimeRange(TimeUtil.NowDateTime(), this.WangChengZhanFightingDayTimes, out num2, true);
			return dateTime.AddMinutes((double)Math.Max(0, num2 - num)).Ticks / 10000L;
		}

		public string GetNextCityBattleTime()
		{
			string lang = GLang.GetLang(6, new object[0]);
			string result;
			if (this.WangChengZhanFightingDayTimes != null && this.WangChengZhanFightingDayTimes.Length > 0)
			{
				result = this.RuntimeData.WangChengZhanFightingDateTime.ToString("yyyy-MM-dd ") + string.Format("{0:00}:{1:00}", this.WangChengZhanFightingDayTimes[0].FromHour, this.WangChengZhanFightingDayTimes[0].FromMinute);
			}
			else
			{
				result = lang;
			}
			return result;
		}

		public string GetCityBattleTimeAndBangHuiListString()
		{
			string result;
			if (this.WangChengZhanFightingDayTimes == null || this.WangChengZhanFightingDayTimes.Length <= 0)
			{
				result = "";
			}
			else
			{
				string text = "";
				lock (this.RuntimeData.Mutex)
				{
					text = text + this.RuntimeData.WangChengZhanFightingDateTime.ToString("yyyy-MM-dd ") + string.Format("{0:00}:{1:00}", this.WangChengZhanFightingDayTimes[0].FromHour, this.WangChengZhanFightingDayTimes[0].FromMinute);
					text += "|";
					foreach (LuoLanChengZhanRequestInfo luoLanChengZhanRequestInfo in this.RuntimeData.WarRequstDict.Values)
					{
						text += string.Format(" {0}", this.GetBHName(luoLanChengZhanRequestInfo.BHID));
					}
				}
				result = text;
			}
			return result;
		}

		private string GetBHName(int bangHuiID)
		{
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bangHuiID, 0);
			string result;
			if (null != bangHuiMiniData)
			{
				result = bangHuiMiniData.BHName;
			}
			else
			{
				result = GLang.GetLang(6, new object[0]);
			}
			return result;
		}

		private void ProcessTimeAddRoleExp()
		{
			long num = TimeUtil.NOW();
			if (num - this.LastAddBangZhanAwardsTicks >= 10000L)
			{
				this.LastAddBangZhanAwardsTicks = num;
				this.NotifyQiZhiBuffOwnerDataList();
				List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
				if (null != mapClients)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							this._LevelAwardsMgr.ProcessBangZhanAwards(gameClient);
						}
					}
				}
				mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
				if (null != mapClients)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							this._LevelAwardsMgr.ProcessBangZhanAwards(gameClient);
						}
					}
				}
			}
		}

		public bool GetZhanMengBirthPoint(GameClient client, int toMapCode, out int mapCode, out int posX, out int posY)
		{
			mapCode = GameManager.MainMapCode;
			posX = -1;
			posY = -1;
			int faction = client.ClientData.Faction;
			lock (this.RuntimeData.Mutex)
			{
				int key;
				if (!this.RuntimeData.BHID2SiteDict.TryGetValue(faction, out key))
				{
					return true;
				}
				int num = 0;
				if (toMapCode == this.RuntimeData.MapCode_LongTa)
				{
					Point randomPoint;
					for (;;)
					{
						randomPoint = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapCode_LongTa);
						if (!Global.InObs(ObjectTypes.OT_CLIENT, this.RuntimeData.MapCode_LongTa, (int)randomPoint.X, (int)randomPoint.Y, 0, 0))
						{
							break;
						}
						if (num++ >= 1000)
						{
							goto IL_EE;
						}
					}
					mapCode = this.RuntimeData.MapCode_LongTa;
					posX = (int)randomPoint.X;
					posY = (int)randomPoint.Y;
					return true;
				}
				IL_EE:
				num = 0;
				if (client.ClientData.Faction == this.RuntimeData.SuperQiZhiOwnerBhid && toMapCode == this.RuntimeData.MapCode)
				{
					for (;;)
					{
						mapCode = toMapCode;
						posX = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosX - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosX + 400);
						posY = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosY - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosY + 400);
						if (!Global.InObs(ObjectTypes.OT_CLIENT, toMapCode, posX, posY, 0, 0))
						{
							break;
						}
						if (num++ >= 100)
						{
							goto IL_1B1;
						}
					}
					return true;
				}
				IL_1B1:
				List<MapBirthPoint> list;
				if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(key, out list) || list.Count == 0)
				{
					return true;
				}
				num = 0;
				for (;;)
				{
					int randomNumber = Global.GetRandomNumber(0, list.Count);
					MapBirthPoint mapBirthPoint = list[randomNumber];
					mapCode = mapBirthPoint.MapCode;
					posX = mapBirthPoint.BirthPosX + Global.GetRandomNumber(-mapBirthPoint.BirthRangeX, mapBirthPoint.BirthRangeX);
					posY = mapBirthPoint.BirthPosY + Global.GetRandomNumber(-mapBirthPoint.BirthRangeY, mapBirthPoint.BirthRangeY);
					if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY, 0, 0))
					{
						break;
					}
					if (num++ >= 1000)
					{
						goto Block_10;
					}
				}
				return true;
				Block_10:;
			}
			return true;
		}

		public bool ClientRelive(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			if (mapCode == this.RuntimeData.MapCode || mapCode == this.RuntimeData.MapCode_LongTa)
			{
				int num;
				int num2;
				int num3;
				if (this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out num, out num2, out num3))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, num2, num3, -1);
					if (num != client.ClientData.MapCode)
					{
						GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, -1);
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num, num2, num3, -1, 1);
					}
					else
					{
						Global.ClientRealive(client, num2, num3, -1);
					}
					return true;
				}
			}
			return false;
		}

		public bool ClientInitGame(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			if (mapCode == this.RuntimeData.MapCode || mapCode == this.RuntimeData.MapCode_LongTa)
			{
				int mapCode2;
				int posX;
				int posY;
				if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					client.ClientData.MapCode = GameManager.MainMapCode;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
				}
				else if (this.GetZhanMengBirthPoint(client, this.RuntimeData.MapCode, out mapCode2, out posX, out posY))
				{
					client.ClientData.MapCode = mapCode2;
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
				}
			}
			return true;
		}

		public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			if (toNewMapCode == this.RuntimeData.MapCode || toNewMapCode == this.RuntimeData.MapCode_LongTa)
			{
				if (this.WangChengZhanState != WangChengZhanStates.Fighting)
				{
					toNewMapCode = GameManager.MainMapCode;
					toNewPosX = -1;
					toNewPosY = -1;
				}
				else if (client.ClientData.MapCode != this.RuntimeData.MapCode_LongTa)
				{
					int num;
					int num2;
					int num3;
					if (this.GetZhanMengBirthPoint(client, toNewMapCode, out num, out num2, out num3))
					{
						toNewMapCode = num;
						toNewPosX = num2;
						toNewPosY = num3;
					}
				}
			}
			return true;
		}

		public bool OnPreInstallJunQi(GameClient client, int npcID)
		{
			bool result;
			if (!this.IsInBattling())
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(44, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else if (client.ClientData.Faction <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(45, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				int num = 0;
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
					{
						if (this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID == npcID)
						{
							num = this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID;
							break;
						}
					}
				}
				if (num > 0)
				{
					result = false;
				}
				else if (!JunQiManager.CanInstallJunQiNow(client.ClientData.MapCode, npcID - 2130706432, client.ClientData.Faction))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(46, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public void ClearMapClients(bool resetTimeOnly = false)
		{
			if (resetTimeOnly)
			{
				this.RuntimeData.LastClearMapTicks = TimeUtil.NOW();
			}
			else
			{
				long num = TimeUtil.NOW();
				if (num - this.RuntimeData.LastClearMapTicks > 60000L)
				{
					this.RuntimeData.LastClearMapTicks = num;
					List<object> mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode);
					if (mapClients != null && mapClients.Count > 0)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								GameManager.ClientMgr.NotifyChangMap2NormalMap(gameClient);
							}
						}
					}
					mapClients = GameManager.ClientMgr.GetMapClients(this.RuntimeData.MapCode_LongTa);
					if (mapClients != null && mapClients.Count > 0)
					{
						for (int i = 0; i < mapClients.Count; i++)
						{
							GameClient gameClient = mapClients[i] as GameClient;
							if (gameClient != null)
							{
								GameManager.ClientMgr.NotifyChangMap2NormalMap(gameClient);
							}
						}
					}
				}
			}
		}

		public void OnInstallJunQi(GameClient client, int npcID)
		{
			int zoneID = 0;
			if (this.RuntimeData.InstallJunQiNeedMoney > 0)
			{
				if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.InstallJunQiNeedMoney, out zoneID))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(47, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
					return;
				}
			}
			string junQiNameByBHID = JunQiManager.GetJunQiNameByBHID(client.ClientData.Faction);
			int junQiLevelByBHID = JunQiManager.GetJunQiLevelByBHID(client.ClientData.Faction);
			bool flag = JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.MapCode, client.ClientData.Faction, zoneID, client.ClientData.BHName, npcID - 2130706432, junQiNameByBHID, junQiLevelByBHID, 24);
			if (flag)
			{
				this.UpdateQiZhiBangHui(npcID - 2130706432, client.ClientData.Faction, client.ClientData.BHName);
				Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(48, new object[0]), new object[]
				{
					Global.FormatRoleName(client, client.ClientData.RoleName),
					Global.GetServerLineName2(),
					Global.GetMapName(client.ClientData.MapCode)
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
			}
		}

		public void OnProcessJunQiDead(int npcID, int bhid)
		{
			this.UpdateQiZhiBangHui(npcID, 0, "");
		}

		private void ResetQiZhiBuff(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			List<int> list = new List<int>();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.QiZhiBuffOwnerDataList.Count; i++)
				{
					QiZhiConfig qiZhiConfig;
					if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.QiZhiBuffOwnerDataList[i].NPCID, out qiZhiConfig))
					{
						int bufferID = qiZhiConfig.BufferID;
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(bufferID);
						if (null != equipPropItem)
						{
							bool add = false;
							if (mapCode == this.RuntimeData.MapCode || mapCode == this.RuntimeData.MapCode_LongTa)
							{
								if (this.RuntimeData.QiZhiBuffOwnerDataList[i].OwnerBHID == client.ClientData.Faction)
								{
									add = true;
								}
							}
							this.UpdateQiZhiBuff4GameClient(client, equipPropItem, bufferID, add);
						}
					}
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			this.ResetQiZhiBuff(client);
			if (client.ClientData.MapCode == this.RuntimeData.MapCode)
			{
				this._MapEventMgr.PlayMapEvents(client);
			}
			this.BroadcastLuoLanChengZhuLoginHint(client);
			if (client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.Faction > 0)
			{
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanChengZhu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanGuiZu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
			}
		}

		public void UpdateChengHaoBuff(GameClient client)
		{
			if (client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.Faction > 0)
			{
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanChengZhu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.LuoLanGuiZu_Title, new double[]
					{
						1.0
					}, 1, true);
				}
			}
			else
			{
				BufferItemTypes bufferItemType = BufferItemTypes.LuoLanChengZhu_Title;
				double[] actionParams = new double[1];
				Global.UpdateBufferData(client, bufferItemType, actionParams, 1, true);
				BufferItemTypes bufferItemType2 = BufferItemTypes.LuoLanGuiZu_Title;
				actionParams = new double[1];
				Global.UpdateBufferData(client, bufferItemType2, actionParams, 1, true);
			}
		}

		public void BroadcastLuoLanChengZhuLoginHint(GameClient client)
		{
			long num = TimeUtil.NOW();
			if (!GameManager.IsKuaFuServer && num >= Data.NextBroadCastTickDict[2])
			{
				Data.NextBroadCastTickDict[2] = num + Data.LuoLanKingGongGaoCD * 1000L;
				if (this.RuntimeData.LuoLanChengZhuClient != client && client.ClientData.Faction == this.RuntimeData.LuoLanChengZhuBHID && client.ClientData.BHZhiWu == 1)
				{
					if (num > this.RuntimeData.LuoLanChengZhuLastLoginTicks + 60000L)
					{
						this.RuntimeData.LuoLanChengZhuLastLoginTicks = num;
						this.RuntimeData.LuoLanChengZhuClient = client;
						string msgText = StringUtil.substitute(GLang.GetLang(49, new object[0]), new object[]
						{
							Global.FormatRoleName(client, client.ClientData.RoleName)
						});
						Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.Bulletin, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			bool result = false;
			bool flag = false;
			lock (this.RuntimeData.Mutex)
			{
				foreach (QiZhiConfig qiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
				{
					if (qiZhiConfig.NPCID == npcExtentionID)
					{
						if (Math.Abs(client.ClientData.PosX - qiZhiConfig.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - qiZhiConfig.PosY) <= 1000)
						{
							flag = true;
						}
						result = true;
						break;
					}
				}
			}
			if (flag)
			{
				Global.InstallJunQi(client, npcID, 24);
			}
			return result;
		}

		public void AddGuangMuEvent(int guangMuID, int show)
		{
			this._MapEventMgr.AddGuangMuEvent(guangMuID, show);
		}

		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (!this.IsInBattling())
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.BHID2SiteDict.ContainsKey(e.BHID))
					{
						e.Result = false;
					}
				}
				if (!e.Result)
				{
					GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(50, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (!this.IsInBattling())
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.BHID2SiteDict.ContainsKey(e.BHID))
					{
						e.Result = false;
					}
				}
				if (!e.Result)
				{
					GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(51, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private RoleDataEx KingRoleData
		{
			get
			{
				RoleDataEx kingRoleData;
				lock (this.kingRoleDataMutex)
				{
					kingRoleData = this._kingRoleData;
				}
				return kingRoleData;
			}
			set
			{
				lock (this.kingRoleDataMutex)
				{
					this._kingRoleData = value;
				}
			}
		}

		public void ReShowLuolanKing(int roleID = 0)
		{
			if (roleID <= 0)
			{
				roleID = LuoLanChengZhanManager.getInstance().GetLuoLanChengZhuRoleID();
			}
			if (roleID <= 0)
			{
				this.RestoreLuolanKingNpc();
			}
			else
			{
				this.ReplaceLuolanKingNpc(roleID);
			}
		}

		public void ClearDbKingNpc()
		{
			this.KingRoleData = null;
			Global.sendToDB<bool, string>(13232, string.Format("{0}", 2), 0);
		}

		public void ReplaceLuolanKingNpc(int roleId)
		{
			RoleDataEx roleDataEx = this.KingRoleData;
			this.KingRoleData = null;
			if (roleDataEx == null || roleDataEx.RoleID != roleId)
			{
				roleDataEx = Global.sendToDB<RoleDataEx, KingRoleGetData>(13230, new KingRoleGetData
				{
					KingType = 2
				}, 0);
				if (roleDataEx == null || roleDataEx.RoleID != roleId)
				{
					RoleDataEx roleDataEx2 = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleId), 0);
					if (roleDataEx2 == null || roleDataEx2.RoleID <= 0)
					{
						return;
					}
					roleDataEx = roleDataEx2;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 2,
						RoleDataEx = roleDataEx
					}, 0))
					{
					}
				}
			}
			if (roleDataEx != null && roleDataEx.RoleID > 0)
			{
				this.KingRoleData = roleDataEx;
				NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 131);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang2, false);
					FakeRoleManager.ProcessNewFakeRole(new SafeClientData
					{
						RoleData = roleDataEx
					}, npc.MapCode, FakeRoleTypes.DiaoXiang2, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 131);
				}
			}
		}

		public void RestoreLuolanKingNpc()
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 131);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang2, false);
			}
		}

		public int GetLuoLanChengZhuRoleID()
		{
			int result = 0;
			lock (this.RuntimeData.Mutex)
			{
				BangHuiLingDiItemData itemByLingDiID = JunQiManager.GetItemByLingDiID(7);
				if (null != itemByLingDiID)
				{
					BangHuiDetailData bangHuiDetailDataAuto = this.GetBangHuiDetailDataAuto(itemByLingDiID.BHID, -1);
					if (null != bangHuiDetailDataAuto)
					{
						result = bangHuiDetailDataAuto.BZRoleID;
					}
				}
			}
			return result;
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				RoleDataEx kingRoleData = this.KingRoleData;
				if (kingRoleData != null && kingRoleData.RoleID == roleId)
				{
					kingRoleData.RoleName = newName;
					if (!Global.sendToDB<bool, KingRolePutData>(13231, new KingRolePutData
					{
						KingType = 2,
						RoleDataEx = kingRoleData
					}, 0))
					{
					}
					this.KingRoleData = null;
					this.ReShowLuolanKing(0);
				}
			}
		}

		public const SceneUIClasses ManagerType = 24;

		private static LuoLanChengZhanManager instance = new LuoLanChengZhanManager();

		public LuoLanChengZhanData RuntimeData = new LuoLanChengZhanData();

		public LevelAwardsMgr _LevelAwardsMgr = new LevelAwardsMgr();

		private MapEventMgr _MapEventMgr = new MapEventMgr();

		private bool WaitingHuangChengResult = false;

		private long BangHuiTakeHuangGongTicks = TimeUtil.NOW();

		private string WangZuBHName = "";

		private int WangZuBHid = -1;

		public object ApplyWangChengWarMutex = new object();

		public int MaxTakingHuangGongSecs = 5000;

		public bool WangChengZhanWeekDaysByConfig = false;

		public DateTimeRange[] WangChengZhanFightingDayTimes = null;

		public WangChengZhanStates WangChengZhanState = WangChengZhanStates.None;

		private int LastTheOnlyOneBangHui = -1;

		private long LastAddBangZhanAwardsTicks = 0L;

		private object kingRoleDataMutex = new object();

		private RoleDataEx _kingRoleData = null;
	}
}
