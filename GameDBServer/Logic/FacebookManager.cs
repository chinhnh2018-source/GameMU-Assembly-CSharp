using System;
using System.Collections.Generic;
using GameDBServer.Data;
using GameDBServer.DB;
using Server.Data;

namespace GameDBServer.Logic
{
	internal class FacebookManager
	{
		public static void initFacebook(string[] fields)
		{
			FacebookManager._FacebookAwards = new Dictionary<int, FacebookAwardData>();
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
						FacebookAwardData facebookAwardData = new FacebookAwardData();
						facebookAwardData.AwardID = Convert.ToInt32(array[0]);
						facebookAwardData.DbKey = array[1];
						facebookAwardData.OnlyNum = Convert.ToInt32(array[2]);
						facebookAwardData.DayMaxNum = Convert.ToInt32(array[3]);
						facebookAwardData.MailTitle = array[5];
						facebookAwardData.MailContent = array[6];
						facebookAwardData.MailUser = array[7];
						string text2 = array[4];
						if (text2.Length > 0)
						{
							facebookAwardData.AwardGoods = new List<GoodsData>();
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
								facebookAwardData.AwardGoods.Add(goodsData);
							}
						}
						FacebookManager._FacebookAwards.Add(facebookAwardData.AwardID, facebookAwardData);
					}
				}
				FacebookManager._isInitFacebook = true;
			}
		}

		private static FacebookAwardData getFacebookAward(int awardID)
		{
			FacebookAwardData result;
			if (FacebookManager._FacebookAwards.ContainsKey(awardID))
			{
				result = FacebookManager._FacebookAwards[awardID];
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
			if (num - FacebookManager.LastScanTicks >= 30000L && FacebookManager._isInitFacebook)
			{
				FacebookManager.LastScanTicks = num;
				List<FacebookAwardData> list = DBQuery.ScanNewGroupFacebookFromTable(dbMgr);
				if (list != null && list.Count > 0 && FacebookManager._FacebookAwards.Count > 0 && FacebookManager._isInitFacebook)
				{
					foreach (FacebookAwardData facebookAwardData in list)
					{
						bool flag = DBWriter.UpdateFacebookState(dbMgr, facebookAwardData.DbID, 1);
						if (flag)
						{
							int state = FacebookManager.SendAward(dbMgr, facebookAwardData.RoleID, facebookAwardData.AwardID);
							DBWriter.UpdateFacebookState(dbMgr, facebookAwardData.DbID, state);
						}
					}
				}
			}
		}

		public static int SendAward(DBManager dbMgr, int roleID, int awardID)
		{
			FacebookAwardData facebookAward = FacebookManager.getFacebookAward(awardID);
			int result;
			if (facebookAward == null)
			{
				result = -6;
			}
			else
			{
				if (facebookAward.OnlyNum > 0)
				{
					int num = DBQuery.FacebookOnlyNum(dbMgr, roleID, awardID);
					if (num > 0)
					{
						return -5;
					}
				}
				if (facebookAward.DayMaxNum > 0)
				{
					int num = DBQuery.FacebookDayNum(dbMgr, roleID, awardID);
					if (num >= facebookAward.DayMaxNum)
					{
						return -5;
					}
				}
				string text = "";
				if (null != facebookAward.AwardGoods)
				{
					foreach (GoodsData goodsData in facebookAward.AwardGoods)
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
					facebookAward.MailUser,
					roleID.ToString(),
					"",
					facebookAward.MailTitle.ToString(),
					facebookAward.MailContent.ToString(),
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
			return result;
		}

		private static Dictionary<int, FacebookAwardData> _FacebookAwards = new Dictionary<int, FacebookAwardData>();

		private static bool _isInitFacebook = false;

		private static long LastScanTicks = DateTime.Now.Ticks / 10000L;

		public enum FaceBookResultType
		{
			Default,
			Success,
			EnoPara = -1,
			EnoUser = -2,
			EnoRole = -3,
			EIp = -4,
			ECountMax = -5,
			EAware = -6,
			EBag = -7,
			Fail = -8
		}
	}
}
