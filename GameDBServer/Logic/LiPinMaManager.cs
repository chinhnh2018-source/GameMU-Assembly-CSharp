using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class LiPinMaManager
	{
		public static void LoadLiPinMaDB(DBManager dbMgr)
		{
			LiPinMaManager._LiPinMaDict = DBQuery.QueryLiPinMaDict(dbMgr);
		}

		public static void LoadLiPinMaFromFile(DBManager dbMgr, bool toAppend = false)
		{
			try
			{
				if (File.Exists("./礼品码_导入文件.txt"))
				{
					StreamReader streamReader = new StreamReader("./礼品码_导入文件.txt", Encoding.GetEncoding("gb2312"));
					if (null != streamReader)
					{
						if (!toAppend)
						{
							LiPinMaManager._LiPinMaDict = null;
							DBWriter.ClearAllLiPinMa(dbMgr);
						}
						Dictionary<string, LiPinMaItem> dictionary = new Dictionary<string, LiPinMaItem>();
						string text;
						while ((text = streamReader.ReadLine()) != null)
						{
							if (!string.IsNullOrEmpty(text))
							{
								string[] array = text.Split(new char[]
								{
									' '
								});
								if (array.Length == 5)
								{
									DBWriter.InsertNewLiPinMa(dbMgr, array[0], array[1], array[2], array[3], array[4], "0");
									LiPinMaItem liPinMaItem = new LiPinMaItem
									{
										LiPinMa = array[0],
										HuodongID = Convert.ToInt32(array[1]),
										MaxNum = Convert.ToInt32(array[2]),
										UsedNum = 0,
										PingTaiID = Convert.ToInt32(array[3]),
										PingTaiRepeat = Convert.ToInt32(array[4])
									};
									dictionary[liPinMaItem.LiPinMa] = liPinMaItem;
								}
							}
						}
						streamReader.Close();
						if (!toAppend || null == LiPinMaManager._LiPinMaDict)
						{
							LiPinMaManager._LiPinMaDict = dictionary;
						}
						else
						{
							Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
							foreach (string key in dictionary.Keys)
							{
								LiPinMaItem liPinMaItem = dictionary[key];
								lock (LiPinMaManager.Mutex)
								{
									liPinMaDict[key] = liPinMaItem;
								}
							}
						}
						File.Delete("./礼品码_导入文件.txt");
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public static int GetLiPinMaPingTaiID(DBManager dbMgr, int songLiID, string liPinMa)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				lock (LiPinMaManager.Mutex)
				{
					LiPinMaItem liPinMaItem = null;
					if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
					{
						result = -1020;
					}
					else
					{
						result = liPinMaItem.PingTaiID;
					}
				}
			}
			return result;
		}

		public static int UseLiPinMa(DBManager dbMgr, int roleID, int songLiID, string liPinMa, bool insertLiPinMa = false)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				int usedNum = 0;
				liPinMa = liPinMa.ToUpper();
				lock (LiPinMaManager.Mutex)
				{
					LiPinMaItem liPinMaItem = null;
					if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
					{
						return -1020;
					}
					if (liPinMaItem.HuodongID != songLiID)
					{
						return -1030;
					}
					if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
					{
						return -1040;
					}
					if (liPinMaItem.PingTaiRepeat <= 0)
					{
						int num = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
						if (num == liPinMaItem.PingTaiID)
						{
							return -10000;
						}
					}
					DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
					liPinMaItem.UsedNum++;
					usedNum = liPinMaItem.UsedNum;
				}
				DBWriter.UpdateLiPinMaUsedNum(dbMgr, liPinMa, usedNum);
				result = 0;
			}
			return result;
		}

		public static int GetLiPinMaPingTaiID2(DBManager dbMgr, int songLiID, string liPinMa, int roleZoneID)
		{
			liPinMa = liPinMa.ToUpper();
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int result;
			if (!LiPinMaParse.ParseLiPinMa2(liPinMa, out num, out num2, out num3, out num4))
			{
				result = -1020;
			}
			else if (num3 > 0 && roleZoneID != num3)
			{
				result = -1021;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public static int GetLiPinMaPingTaiIDNX(DBManager dbMgr, int songLiID, string liPinMa, int roleZoneID)
		{
			liPinMa = liPinMa.ToUpper();
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int result;
			if (!LiPinMaParse.ParseLiPinMaNX2(liPinMa, out num, out num2, out num3, out num4))
			{
				result = -1020;
			}
			else if (num3 > 0 && roleZoneID != num3)
			{
				result = -1021;
			}
			else
			{
				result = num;
			}
			return result;
		}

		public static int UseLiPinMa2(DBManager dbMgr, int roleID, int songLiID, string liPinMa, int roleZoneID)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				int pingTaiID = -1;
				int pingTaiRepeat = 0;
				int num = 0;
				int maxNum = 0;
				if (!LiPinMaParse.ParseLiPinMa2(liPinMa, out pingTaiID, out pingTaiRepeat, out num, out maxNum))
				{
					result = -1020;
				}
				else if (num > 0 && roleZoneID != num)
				{
					result = -1021;
				}
				else
				{
					lock (LiPinMaManager.Mutex)
					{
						LiPinMaItem liPinMaItem = null;
						bool flag2 = false;
						if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
						{
							liPinMaItem = new LiPinMaItem
							{
								LiPinMa = liPinMa,
								HuodongID = 1,
								MaxNum = maxNum,
								UsedNum = 0,
								PingTaiID = pingTaiID,
								PingTaiRepeat = pingTaiRepeat
							};
							liPinMaDict[liPinMa] = liPinMaItem;
							flag2 = true;
						}
						if (liPinMaItem.HuodongID != songLiID)
						{
							return -1030;
						}
						if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
						{
							return -1040;
						}
						if (liPinMaItem.PingTaiRepeat <= 0)
						{
							int num2 = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
							if (num2 == liPinMaItem.PingTaiID)
							{
								if (liPinMaItem.MaxNum <= 1 || !flag2)
								{
									return -10000;
								}
								int num3 = DBQuery.QueryUseNumByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
								if (num3 >= liPinMaItem.MaxNum)
								{
									return -1040;
								}
							}
						}
						DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
						liPinMaItem.UsedNum++;
						int usedNum = liPinMaItem.UsedNum;
					}
					DBWriter.InsertNewLiPinMa(dbMgr, liPinMa, songLiID.ToString(), "1", pingTaiID.ToString(), pingTaiRepeat.ToString(), "1");
					result = 0;
				}
			}
			return result;
		}

		public static int UseLiPinMaNX(DBManager dbMgr, int roleID, int songLiID, string liPinMa, int roleZoneID)
		{
			int result;
			if (null == LiPinMaManager._LiPinMaDict)
			{
				result = -1010;
			}
			else
			{
				Dictionary<string, LiPinMaItem> liPinMaDict = LiPinMaManager._LiPinMaDict;
				liPinMa = liPinMa.ToUpper();
				int pingTaiID = -1;
				int pingTaiRepeat = 0;
				int num = 0;
				int maxNum = 0;
				if (!LiPinMaParse.ParseLiPinMaNX2(liPinMa, out pingTaiID, out pingTaiRepeat, out num, out maxNum))
				{
					result = -1020;
				}
				else if (num > 0 && roleZoneID != num)
				{
					result = -1021;
				}
				else
				{
					lock (LiPinMaManager.Mutex)
					{
						LiPinMaItem liPinMaItem = null;
						bool flag2 = false;
						if (!liPinMaDict.TryGetValue(liPinMa, out liPinMaItem))
						{
							liPinMaItem = new LiPinMaItem
							{
								LiPinMa = liPinMa,
								HuodongID = 1,
								MaxNum = maxNum,
								UsedNum = 0,
								PingTaiID = pingTaiID,
								PingTaiRepeat = pingTaiRepeat
							};
							liPinMaDict[liPinMa] = liPinMaItem;
							flag2 = true;
						}
						if (liPinMaItem.HuodongID != songLiID)
						{
							return -1030;
						}
						if (liPinMaItem.MaxNum > 0 && liPinMaItem.UsedNum >= liPinMaItem.MaxNum)
						{
							return -1040;
						}
						if (liPinMaItem.PingTaiRepeat <= 0)
						{
							int num2 = DBQuery.QueryPingTaiIDByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
							if (num2 == liPinMaItem.PingTaiID)
							{
								if (liPinMaItem.MaxNum <= 1 || !flag2)
								{
									return -10000;
								}
								int num3 = DBQuery.QueryUseNumByHuoDongID(dbMgr, liPinMaItem.HuodongID, roleID, liPinMaItem.PingTaiID);
								if (num3 >= liPinMaItem.MaxNum)
								{
									return -1040;
								}
							}
						}
						DBWriter.AddUsedLiPinMa(dbMgr, liPinMaItem.HuodongID, liPinMaItem.LiPinMa, liPinMaItem.PingTaiID, roleID);
						liPinMaItem.UsedNum++;
						int usedNum = liPinMaItem.UsedNum;
					}
					DBWriter.InsertNewLiPinMa(dbMgr, liPinMa, songLiID.ToString(), "1", pingTaiID.ToString(), pingTaiRepeat.ToString(), "1");
					result = 0;
				}
			}
			return result;
		}

		private static object Mutex = new object();

		private static Dictionary<string, LiPinMaItem> _LiPinMaDict = null;
	}
}
