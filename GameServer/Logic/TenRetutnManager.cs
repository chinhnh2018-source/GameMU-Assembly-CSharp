using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class TenRetutnManager : IManager
	{
		public static TenRetutnManager getInstance()
		{
			return TenRetutnManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
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

		public bool InitConfig()
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.SystemOpen = false;
				string text = Global.GameResPath("Config/TenRetutnAward.xml");
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return true;
				}
				try
				{
					this.RuntimeData.SystemOpen = false;
					this.RuntimeData._tenAwardDic.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							TenRetutnAwardsData tenRetutnAwardsData = new TenRetutnAwardsData();
							tenRetutnAwardsData.ID = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "ID"));
							tenRetutnAwardsData.MailUser = GLang.GetLang(112, new object[0]);
							tenRetutnAwardsData.MailTitle = Global.GetSafeAttributeStr(xelement2, "MailTitle");
							tenRetutnAwardsData.MailContent = Global.GetSafeAttributeStr(xelement2, "MailContent");
							ConfigParser.ParseAwardsItemList(Global.GetDefAttributeStr(xelement2, "GoodsID1", ""), ref tenRetutnAwardsData.GoodsID1, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetDefAttributeStr(xelement2, "GoodsID2", ""), ref tenRetutnAwardsData.GoodsID2, '|', ',');
							tenRetutnAwardsData.UserList = Global.GetSafeAttributeStr(xelement2, "UserList");
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "BeginTime", "2019-12-31");
							string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "FinishTime", "2011-11-11");
							tenRetutnAwardsData.BeginTimeStr = defAttributeStr.Replace(':', '$');
							tenRetutnAwardsData.FinishTimeStr = defAttributeStr2.Replace(':', '$');
							if (DateTime.TryParse(defAttributeStr, out tenRetutnAwardsData.BeginTime) && DateTime.TryParse(defAttributeStr2, out tenRetutnAwardsData.FinishTime) && TimeUtil.NowDateTime() < tenRetutnAwardsData.FinishTime)
							{
								tenRetutnAwardsData.SystemOpen = true;
								this.RuntimeData._tenAwardDic.Add(tenRetutnAwardsData.ID, tenRetutnAwardsData);
								text = Global.GameResPath("Config/" + tenRetutnAwardsData.UserList);
								if (File.Exists(text))
								{
									string[] array = File.ReadAllLines(text);
									foreach (string text2 in array)
									{
										if (!string.IsNullOrEmpty(text2))
										{
											tenRetutnAwardsData._tenUserIdAwardsDict[text2.ToLower()] = false;
										}
									}
								}
								if (tenRetutnAwardsData._tenUserIdAwardsDict.Count == 0)
								{
									tenRetutnAwardsData.SystemOpen = false;
								}
							}
							this.RuntimeData.SystemOpen |= tenRetutnAwardsData.SystemOpen;
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1, "加载Config/TenRetutnAward.xml时文件出现异常!!!", ex, true);
				}
			}
			return true;
		}

		public void GiveAwards(GameClient client)
		{
			if (this.RuntimeData.SystemOpen)
			{
				DateTime t = TimeUtil.NowDateTime();
				string key = client.strUserID.ToLower();
				List<TenRetutnAwardsData> list = new List<TenRetutnAwardsData>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (TenRetutnAwardsData tenRetutnAwardsData in this.RuntimeData._tenAwardDic.Values)
					{
						if (tenRetutnAwardsData.SystemOpen && t >= tenRetutnAwardsData.BeginTime && t <= tenRetutnAwardsData.FinishTime)
						{
							bool flag2;
							if (tenRetutnAwardsData._tenUserIdAwardsDict.TryGetValue(key, out flag2) && !flag2)
							{
								list.Add(tenRetutnAwardsData);
							}
						}
					}
				}
				foreach (TenRetutnAwardsData tenRetutnAwardsData2 in list)
				{
					string keyStr = string.Format("{0}_{1}_{2}_{3}", new object[]
					{
						tenRetutnAwardsData2.BeginTimeStr,
						tenRetutnAwardsData2.FinishTimeStr,
						tenRetutnAwardsData2.ID,
						client.ClientData.ZoneID
					});
					string[] array = Global.QeuryUserActivityInfo(client, keyStr, 999, "0");
					if (array != null && array.Length != 0)
					{
						int num = Global.SafeConvertToInt32(array[0]);
						int num2 = Global.SafeConvertToInt32(array[3]);
						if (num2 > 0)
						{
							lock (this.RuntimeData.Mutex)
							{
								tenRetutnAwardsData2._tenUserIdAwardsDict[key] = true;
							}
						}
						else
						{
							List<AwardsItemData> list2 = new List<AwardsItemData>(tenRetutnAwardsData2.GoodsID1.Items);
							foreach (AwardsItemData awardsItemData in tenRetutnAwardsData2.GoodsID2.Items)
							{
								if (Global.IsCanGiveRewardByOccupation(client, awardsItemData.GoodsID))
								{
									list2.Add(awardsItemData);
								}
							}
							num = Global.UseMailGivePlayerAward2(client, list2, tenRetutnAwardsData2.MailTitle, tenRetutnAwardsData2.MailContent, 0, 0, 0);
							if (num >= 0)
							{
								Global.UpdateUserActivityInfo(client, keyStr, 999, 1L, t.ToString("yyyy-MM-dd HH$mm$ss"));
							}
							lock (this.RuntimeData.Mutex)
							{
								tenRetutnAwardsData2._tenUserIdAwardsDict[key] = true;
							}
						}
					}
				}
			}
		}

		private static TenRetutnManager instance = new TenRetutnManager();

		public TenRetutnData RuntimeData = new TenRetutnData();
	}
}
