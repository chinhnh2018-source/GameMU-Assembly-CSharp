using System;
using System.Collections.Generic;
using System.Globalization;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic.Ten
{
	internal class TenManager
	{
		public static void initTen(string[] fields)
		{
			TenManager._tenAwards = new Dictionary<int, TenAwardData>();
			if (fields != null && fields.Length > 0)
			{
				foreach (string text in fields)
				{
					if (text != null)
					{
						string[] array = text.Split(new char[]
						{
							':'
						});
						TenAwardData tenAwardData = new TenAwardData();
						tenAwardData.AwardID = Convert.ToInt32(array[0]);
						tenAwardData.DbKey = array[1];
						tenAwardData.OnlyNum = Convert.ToInt32(array[2]);
						tenAwardData.DayMaxNum = Convert.ToInt32(array[3]);
						tenAwardData.MailTitle = array[5];
						tenAwardData.MailContent = array[6];
						tenAwardData.MailUser = array[7];
						tenAwardData.BeginTime = DateTime.ParseExact(array[8], "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
						tenAwardData.EndTime = DateTime.ParseExact(array[9], "yyyyMMddHHmmss", CultureInfo.CurrentCulture);
						tenAwardData.RoleLevel = Convert.ToInt32(array[10]);
						string text2 = array[4];
						if (text2.Length > 0)
						{
							tenAwardData.AwardGoods = new List<GoodsData>();
							string[] array2 = text2.Split(new char[]
							{
								'|'
							});
							foreach (string text3 in array2)
							{
								string[] array4 = text3.Split(new char[]
								{
									','
								});
								GoodsData goodsData = new GoodsData();
								goodsData.GoodsID = Convert.ToInt32(array4[0]);
								goodsData.GCount = Convert.ToInt32(array4[1]);
								goodsData.Binding = Convert.ToInt32(array4[2]);
								tenAwardData.AwardGoods.Add(goodsData);
							}
						}
						TenManager._tenAwards.Add(tenAwardData.AwardID, tenAwardData);
					}
				}
				TenManager._isInitTen = true;
			}
		}

		private static TenAwardData getTenAward(int awardID)
		{
			TenAwardData result;
			if (TenManager._tenAwards.ContainsKey(awardID))
			{
				result = TenManager._tenAwards[awardID];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static void ScanLastGroup(DBManager dbMgr)
		{
			long num = DateTime.Now.Ticks / 10000L;
			if (num - TenManager.LastScanTicks >= 30000L && TenManager._isInitTen)
			{
				TenManager.LastScanTicks = num;
				List<TenAwardData> list = DBQuery.ScanNewGroupTenFromTable(dbMgr);
				if (list != null && list.Count > 0 && TenManager._tenAwards.Count > 0 && TenManager._isInitTen)
				{
					foreach (TenAwardData tenAwardData in list)
					{
						bool flag = DBWriter.UpdateTenState(dbMgr, tenAwardData.DbID, 1);
						if (flag)
						{
							int state = TenManager.SendAward(dbMgr, tenAwardData.UserID, tenAwardData.RoleID, tenAwardData.AwardID);
							DBWriter.UpdateTenState(dbMgr, tenAwardData.DbID, state);
						}
					}
				}
			}
		}

		public static int SendAward(DBManager dbMgr, string userID, int roleID, int awardID)
		{
			TenAwardData tenAward = TenManager.getTenAward(awardID);
			int result;
			if (tenAward == null)
			{
				result = -6;
			}
			else
			{
				DateTime now = DateTime.Now;
				if (now < tenAward.BeginTime || now > tenAward.EndTime)
				{
					result = -9;
				}
				else
				{
					DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleID);
					if (dbroleInfo == null)
					{
						result = -3;
					}
					else if (dbroleInfo.ChangeLifeCount * 100 + dbroleInfo.Level < tenAward.RoleLevel)
					{
						result = -10;
					}
					else
					{
						if (tenAward.OnlyNum > 0)
						{
							int num = DBQuery.TenOnlyNum(dbMgr, userID, awardID);
							if (num > 0)
							{
								return -5;
							}
						}
						if (tenAward.DayMaxNum > 0)
						{
							int num = DBQuery.TenDayNum(dbMgr, userID, awardID);
							if (num >= tenAward.DayMaxNum)
							{
								return -5;
							}
						}
						string text = "";
						if (null != tenAward.AwardGoods)
						{
							foreach (GoodsData goodsData in tenAward.AwardGoods)
							{
								int gcount = goodsData.GCount;
								text += string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}_{12}_{13}_{14}_{15}", new object[]
								{
									goodsData.GoodsID,
									goodsData.Forge_level,
									goodsData.Quality,
									goodsData.Props,
									gcount,
									0,
									0,
									goodsData.Jewellist,
									goodsData.AddPropIndex,
									goodsData.Binding,
									goodsData.BornIndex,
									goodsData.Lucky,
									goodsData.Strong,
									goodsData.ExcellenceInfo,
									goodsData.AppendPropLev,
									goodsData.ChangeLifeLevForEquip
								});
								if (text.Length > 0)
								{
									text += "|";
								}
							}
						}
						string[] fields = new string[]
						{
							"-1",
							tenAward.MailUser,
							roleID.ToString(),
							"",
							tenAward.MailTitle.ToString(),
							tenAward.MailContent.ToString(),
							"0",
							"0",
							"0",
							text
						};
						int num2 = 0;
						int num3 = Global.AddMail(dbMgr, fields, out num2);
						if (num3 > 0)
						{
							string arg = string.Format("{0}|{1}", roleID.ToString(), num3);
							string gmCmd = string.Format("-notifymail {0}", arg);
							ChatMsgManager.AddGMCmdChatMsg(-1, gmCmd);
							result = num3;
						}
						else
						{
							result = -8;
						}
					}
				}
			}
			return result;
		}

		private static Dictionary<int, TenAwardData> _tenAwards = new Dictionary<int, TenAwardData>();

		private static bool _isInitTen = false;

		private static long LastScanTicks = DateTime.Now.Ticks / 10000L;

		public enum TenResultType
		{
			Default,
			Success,
			EnoPara = -1,
			EnoRole = -3,
			EIp = -4,
			ECountMax = -5,
			EAware = -6,
			EBag = -7,
			Fail = -8,
			ETimeOut = -9,
			ELevelLimit = -10
		}
	}
}
