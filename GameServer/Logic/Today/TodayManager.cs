using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.TuJian;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.Today
{
	public class TodayManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		public static TodayManager getInstance()
		{
			return TodayManager.instance;
		}

		public bool initialize()
		{
			TodayManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1030, 1, 1, TodayManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1031, 2, 2, TodayManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			case 1030:
				result = this.ProcessCmdTodayData(client, nID, bytes, cmdParams);
				break;
			case 1031:
				result = this.ProcessCmdTodayAward(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool ProcessCmdTodayData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				string todayData = this.GetTodayData(client);
				client.sendCmd(1030, todayData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdTodayAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				bool isAll = int.Parse(cmdParams[0]) > 0;
				int todayID = int.Parse(cmdParams[1]);
				string cmdData = this.TodayAward(client, isAll, todayID);
				client.sendCmd(1031, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private string GetTodayData(GameClient client)
		{
			string format = "{0}:{1}";
			string result;
			if (!this.IsGongNengOpened())
			{
				result = string.Format(format, -11, 0);
			}
			else
			{
				List<TodayInfo> list = this.InitToday(client);
				if (ListExt.IsNullOrEmpty<TodayInfo>(list))
				{
					result = string.Format(format, -11, 0);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					int i = 0;
					while (i < list.Count)
					{
						stringBuilder.Append(string.Format("{0}*{1}", list[i].ID, list[i].NumEnd));
						i++;
						if (i < list.Count)
						{
							stringBuilder.Append("|");
						}
					}
					result = string.Format(format, 1, stringBuilder.ToString());
				}
			}
			return result;
		}

		private string TodayAward(GameClient client, bool isAll, int todayID)
		{
			string format = "{0}:{1}";
			string result;
			if (!this.IsGongNengOpened())
			{
				result = string.Format(format, -11, 0);
			}
			else
			{
				TodayInfo todayInfo = null;
				if (!isAll)
				{
					todayInfo = this.GetTadayInfoByID(client, todayID);
					if (todayInfo == null)
					{
						return string.Format(format, -3, 0);
					}
					if (todayInfo.NumMax - todayInfo.NumEnd <= 0)
					{
						return string.Format(format, -4, 0);
					}
				}
				List<TodayInfo> list = new List<TodayInfo>();
				if (isAll)
				{
					list = this.InitToday(client);
				}
				else
				{
					list.Add(todayInfo);
				}
				if (ListExt.IsNullOrEmpty<TodayInfo>(list))
				{
					result = string.Format(format, -3, 0);
				}
				else
				{
					IEnumerable<TodayInfo> source = from info in list
					where info.FuBenID > 0 && client.ClientData.FuBenID > 0 && client.ClientData.FuBenID == info.FuBenID && info.NumMax - info.NumEnd > 0 && info.NumEnd >= 0
					select info;
					if (source.Any<TodayInfo>())
					{
						result = string.Format(format, -6, 0);
					}
					else
					{
						IEnumerable<TodayInfo> enumerable = from info in list
						where info.NumMax - info.NumEnd > 0
						select info;
						if (!enumerable.Any<TodayInfo>())
						{
							result = string.Format(format, -5, 0);
						}
						else
						{
							int num = 0;
							foreach (TodayInfo todayInfo2 in enumerable)
							{
								num += todayInfo2.AwardInfo.GoodsList.Count;
							}
							if (!Global.CanAddGoodsNum(client, num))
							{
								result = string.Format(format, -2, 0);
							}
							else
							{
								foreach (TodayInfo todayInfo2 in enumerable)
								{
									SystemXmlItem fuBenInfo = null;
									if (todayInfo2.Type == 6)
									{
										TaskData taoTask = TodayManager.GetTaoTask(client);
										if (taoTask != null)
										{
											if (!Global.CancelTask(client, taoTask.DbID, taoTask.DoingTaskID))
											{
												return string.Format(format, -8, 0);
											}
										}
									}
									else if (todayInfo2.Type == 9)
									{
										BufferData bufferDataByID = Global.GetBufferDataByID(client, 34);
										if (bufferDataByID != null)
										{
											bufferDataByID.BufferVal = 0L;
											bufferDataByID.BufferSecs = 0;
											GameManager.ClientMgr.NotifyBufferData(client, bufferDataByID);
										}
									}
									else if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(todayInfo2.FuBenID, out fuBenInfo))
									{
										return string.Format(format, -7, 0);
									}
									if (!this.SetFinishNum(client, todayInfo2, fuBenInfo))
									{
										return string.Format(format, -1, 0);
									}
								}
								TodayAwardInfo todayAwardInfo = new TodayAwardInfo();
								foreach (TodayInfo todayInfo2 in enumerable)
								{
									int num2 = todayInfo2.NumMax - todayInfo2.NumEnd;
									for (int i = 0; i < todayInfo2.AwardInfo.GoodsList.Count; i++)
									{
										GoodsData goodsData = todayInfo2.AwardInfo.GoodsList[i];
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount * num2, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, "每日专享", "1900-01-01 12:00:00", goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, 0, null, null, 0, true);
									}
									todayAwardInfo.AddAward(todayInfo2.AwardInfo, num2);
								}
								if (todayAwardInfo.Exp > 0.0)
								{
									GameManager.ClientMgr.ProcessRoleExperience(client, (long)todayAwardInfo.Exp, true, true, false, "none");
									GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(30, new object[0]), new object[]
									{
										todayAwardInfo.Exp
									}), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlyErr, 0);
								}
								if (todayAwardInfo.GoldBind > 0.0)
								{
									GameManager.ClientMgr.AddMoney1(client, (int)todayAwardInfo.GoldBind, "每日专享", true);
								}
								if (todayAwardInfo.MoJing > 0.0)
								{
									GameManager.ClientMgr.ModifyTianDiJingYuanValue(client, (int)todayAwardInfo.MoJing, "每日专享", true, true, false);
								}
								if (todayAwardInfo.ChengJiu > 0.0)
								{
									GameManager.ClientMgr.ModifyChengJiuPointsValue(client, (int)todayAwardInfo.ChengJiu, "每日专享", true, true);
								}
								if (todayAwardInfo.ShengWang > 0.0)
								{
									GameManager.ClientMgr.ModifyShengWangValue(client, (int)todayAwardInfo.ShengWang, "每日专享", true, true);
								}
								if (todayAwardInfo.ZhanGong > 0.0)
								{
									int num3 = (int)todayAwardInfo.ZhanGong;
									GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref num3, AddBangGongTypes.Today, 0);
								}
								if (todayAwardInfo.DiamondBind > 0.0 || todayAwardInfo.ExtDiamondBind > 0.0)
								{
									GameManager.ClientMgr.AddUserGold(client, (int)(todayAwardInfo.DiamondBind + todayAwardInfo.ExtDiamondBind), "每日专享");
								}
								if (todayAwardInfo.XingHun > 0.0)
								{
									GameManager.ClientMgr.ModifyStarSoulValue(client, (int)todayAwardInfo.XingHun, "每日专享", true, true);
								}
								if (todayAwardInfo.YuanSuFenMo > 0.0)
								{
									GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, (int)todayAwardInfo.YuanSuFenMo, "每日专享", true, false);
								}
								if (todayAwardInfo.ShouHuDianShu > 0.0)
								{
									SingletonTemplate<GuardStatueManager>.Instance().AddGuardPoint(client, (int)todayAwardInfo.ShouHuDianShu, "每日专享");
								}
								if (todayAwardInfo.ZaiZao > 0.0)
								{
									GameManager.ClientMgr.ModifyZaiZaoValue(client, (int)todayAwardInfo.ZaiZao, "每日专享", true, true, false);
								}
								if (todayAwardInfo.LingJing > 0.0)
								{
									GameManager.ClientMgr.ModifyMUMoHeValue(client, (int)todayAwardInfo.LingJing, "每日专享", true, true, false);
								}
								if (todayAwardInfo.RongYao > 0.0)
								{
									GameManager.ClientMgr.ModifyTianTiRongYaoValue(client, (int)todayAwardInfo.RongYao, "每日专享", true);
								}
								result = this.GetTodayData(client);
							}
						}
					}
				}
			}
			return result;
		}

		private List<TodayInfo> InitToday(GameClient client)
		{
			List<TodayInfo> list = new List<TodayInfo>();
			int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
			int taskID = client.ClientData.MainTaskID;
			IEnumerable<TodayInfo> enumerable = from info in TodayManager._todayInfoList
			where level >= info.LevelMin && level <= info.LevelMax && taskID >= info.TaskMin
			select info;
			foreach (TodayInfo info2 in enumerable)
			{
				TodayInfo todayInfo = new TodayInfo(info2);
				todayInfo.NumEnd = this.GetFinishNum(client, todayInfo);
				todayInfo.NumMax = this.GetMaxNum(client, todayInfo);
				list.Add(todayInfo);
			}
			return list;
		}

		private TodayInfo GetTadayInfoByID(GameClient client, int id)
		{
			TodayInfo todayInfo = null;
			int taskID = client.ClientData.MainTaskID;
			int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
			IEnumerable<TodayInfo> source = from info in TodayManager._todayInfoList
			where info.ID == id && level >= info.LevelMin && level <= info.LevelMax && taskID >= info.TaskMin
			select info;
			TodayInfo result;
			if (!source.Any<TodayInfo>())
			{
				result = null;
			}
			else
			{
				TodayInfo todayInfo2 = source.First<TodayInfo>();
				if (todayInfo2 != null)
				{
					todayInfo = new TodayInfo(todayInfo2);
					todayInfo.NumEnd = this.GetFinishNum(client, todayInfo);
					todayInfo.NumMax = this.GetMaxNum(client, todayInfo);
				}
				result = todayInfo;
			}
			return result;
		}

		private int GetMaxNum(GameClient client, TodayInfo todayInfo)
		{
			int val = 0;
			switch (todayInfo.Type)
			{
			case 1:
				val = todayInfo.NumMax;
				break;
			case 2:
				val = todayInfo.NumMax;
				break;
			case 3:
			case 4:
			case 5:
				val = todayInfo.NumMax;
				break;
			case 6:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				if (null == dailyTaskData)
				{
					val = Global.MaxTaofaTaskNumForMU;
				}
				else
				{
					val = Global.GetMaxDailyTaskNum(client, 9, dailyTaskData);
				}
				break;
			}
			case 7:
			case 8:
			case 9:
				val = todayInfo.NumMax;
				break;
			case 10:
			case 11:
			case 12:
				val = todayInfo.NumMax;
				break;
			}
			return Math.Max(0, val);
		}

		private int GetFinishNum(GameClient client, TodayInfo todayInfo)
		{
			int result = 0;
			FuBenData fuBenData = this.GetFuBenData(client, todayInfo.FuBenID);
			switch (todayInfo.Type)
			{
			case 1:
			case 2:
				result = fuBenData.EnterNum;
				break;
			case 3:
			case 4:
			case 5:
				result = fuBenData.FinishNum;
				break;
			case 6:
			{
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, 9);
				result = ((dailyTaskData == null) ? 0 : dailyTaskData.RecNum);
				break;
			}
			case 7:
				result = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 2);
				break;
			case 8:
				result = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 1);
				break;
			case 9:
				result = Global.QueryDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, 6);
				break;
			case 10:
			case 11:
			case 12:
				result = fuBenData.EnterNum;
				break;
			}
			return result;
		}

		private FuBenData GetFuBenData(GameClient client, int fuBenID)
		{
			bool flag = false;
			FuBenData fuBenData = Global.GetFuBenData(client, fuBenID);
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (null == fuBenData)
			{
				fuBenData = Global.AddFuBenData(client, fuBenID, dayOfYear, 0, 0, 0);
			}
			if (fuBenData.DayID != dayOfYear)
			{
				fuBenData.DayID = dayOfYear;
				fuBenData.EnterNum = 0;
				fuBenData.FinishNum = 0;
				flag = true;
			}
			if (flag)
			{
				GameManager.ClientMgr.NotifyFuBenData(client, fuBenData);
			}
			return fuBenData;
		}

		private bool SetFinishNum(GameClient client, TodayInfo todayInfo, SystemXmlItem fuBenInfo)
		{
			int num = todayInfo.NumMax - todayInfo.NumEnd;
			switch (todayInfo.Type)
			{
			case 1:
			case 2:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			case 3:
			case 4:
			case 5:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			case 6:
			{
				DailyTaskData dailyTaskData = null;
				Global.GetDailyTaskData(client, 9, out dailyTaskData, true);
				dailyTaskData.RecNum = todayInfo.NumMax;
				Global.UpdateDBDailyTaskData(client, dailyTaskData, true);
				break;
			}
			case 7:
			{
				int nType = 2;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 8:
			{
				int nType = 1;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 9:
			{
				int nType = 6;
				Global.UpdateDayActivityEnterCountToDB(client, client.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, nType, todayInfo.NumMax);
				break;
			}
			case 10:
			case 11:
			case 12:
				Global.UpdateFuBenData(client, todayInfo.FuBenID, num, num);
				break;
			}
			FuBenData fuBenData = Global.GetFuBenData(client, todayInfo.FuBenID);
			if (fuBenData != null && (fuBenData.EnterNum != 0 || fuBenData.FinishNum != 0))
			{
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				RoleDailyData myRoleDailyData = client.ClientData.MyRoleDailyData;
				if (myRoleDailyData == null || dayOfYear != myRoleDailyData.FuBenDayID)
				{
					myRoleDailyData.FuBenDayID = dayOfYear;
					myRoleDailyData.TodayFuBenNum = 0;
				}
				int num2 = todayInfo.NumMax - todayInfo.NumEnd;
				myRoleDailyData.TodayFuBenNum += num2;
				int intValue = fuBenInfo.GetIntValue("FuBenLevel", -1);
				DailyActiveManager.ProcessCompleteCopyMapForDailyActive(client, intValue, num2);
				ChengJiuManager.ProcessCompleteCopyMapForChengJiu(client, intValue, num2);
			}
			return true;
		}

		public static TaskData GetTaoTask(GameClient client)
		{
			TaskData result;
			if (null == client.ClientData.TaskDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.TaskDataList)
				{
					for (int i = 0; i < client.ClientData.TaskDataList.Count; i++)
					{
						TaskData taskData = client.ClientData.TaskDataList[i];
						SystemXmlItem systemXmlItem = null;
						if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemXmlItem))
						{
							int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
							if (intValue == 9)
							{
								return taskData;
							}
						}
					}
				}
				result = null;
			}
			return result;
		}

		public bool IsGongNengOpened()
		{
			return !GameFuncControlManager.IsGameFuncDisabled(6) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Today");
		}

		public static void InitConfig()
		{
			string text = Global.GameResPath("Config/JianFu.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					List<TodayInfo> list = new List<TodayInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							TodayInfo todayInfo = new TodayInfo();
							todayInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							todayInfo.Type = todayInfo.ID / 100;
							todayInfo.Name = Global.GetDefAttributeStr(xelement2, "Name", "0");
							todayInfo.FuBenID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "FuBenID", "0"));
							todayInfo.HuoDongID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "HuoDongID", "0"));
							todayInfo.LevelMin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinZhuanSheng", "0")) * 100 + Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinLevel", "0"));
							todayInfo.LevelMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MaxZhuanSheng", "0")) * 100 + Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MaxLevel", "0"));
							todayInfo.TaskMin = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinTasks", "0"));
							todayInfo.NumMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0"));
							TodayAwardInfo todayAwardInfo = new TodayAwardInfo();
							todayAwardInfo.Exp = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "Exp", "0"));
							todayAwardInfo.GoldBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "BandJinBi", "0"));
							todayAwardInfo.MoJing = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "MoJing", "0"));
							todayAwardInfo.ChengJiu = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ChengJiu", "0"));
							todayAwardInfo.ShengWang = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ShengWang", "0"));
							todayAwardInfo.ZhanGong = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ZhanGong", "0"));
							todayAwardInfo.DiamondBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "BandZuanShi", "0"));
							todayAwardInfo.XingHun = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "XingHun", "0"));
							todayAwardInfo.YuanSuFenMo = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "YuanSuFenMo", "0"));
							todayAwardInfo.ShouHuDianShu = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ShouHuDianShu", "0"));
							todayAwardInfo.ZaiZao = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ZaiZao", "0"));
							todayAwardInfo.LingJing = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "LingJing", "0"));
							todayAwardInfo.RongYao = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "RongYao", "0"));
							todayAwardInfo.ExtDiamondBind = (double)Convert.ToInt64(Global.GetDefAttributeStr(xelement2, "ExtraBandZuanShi", "0"));
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "Goods", "0");
							if (!string.IsNullOrEmpty(defAttributeStr) && !defAttributeStr.Equals("0"))
							{
								string[] fields = defAttributeStr.Split(new char[]
								{
									'|'
								});
								todayAwardInfo.GoodsList = GoodsHelper.ParseGoodsDataList(fields, text);
							}
							todayInfo.AwardInfo = todayAwardInfo;
							list.Add(todayInfo);
						}
					}
					TodayManager._todayInfoList = list;
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		private static TodayManager instance = new TodayManager();

		private static List<TodayInfo> _todayInfoList = new List<TodayInfo>();
	}
}
