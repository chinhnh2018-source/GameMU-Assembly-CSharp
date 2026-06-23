using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.Ten
{
	public class TenManager : IManager
	{
		public static TenManager getInstance()
		{
			return TenManager.instance;
		}

		public bool initialize()
		{
			return TenManager.initConfig();
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

		public static bool initConfig()
		{
			string text = Global.GameResPath("Config/TenAward.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			bool result;
			if (null == xelement)
			{
				result = false;
			}
			else
			{
				try
				{
					TenManager._tenAwardDic.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							TenAwardData tenAwardData = new TenAwardData();
							tenAwardData.AwardID = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "ID"));
							tenAwardData.AwardName = Global.GetSafeAttributeStr(xelement2, "Name");
							tenAwardData.DbKey = Global.GetSafeAttributeStr(xelement2, "DbKey");
							tenAwardData.DayMaxNum = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "DayMaxNum"));
							tenAwardData.OnlyNum = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "OnlyNum"));
							tenAwardData.MailUser = GLang.GetLang(112, new object[0]);
							tenAwardData.MailTitle = Global.GetSafeAttributeStr(xelement2, "MailTitle");
							tenAwardData.MailContent = Global.GetSafeAttributeStr(xelement2, "MailContent");
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "BeginDate", "");
							string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "EndDate", "");
							string defAttributeStr3 = Global.GetDefAttributeStr(xelement2, "Level", "0,1");
							if (string.IsNullOrEmpty(defAttributeStr))
							{
								tenAwardData.BeginTime = DateTime.MinValue;
							}
							else
							{
								tenAwardData.BeginTime = DateTime.Parse(defAttributeStr);
							}
							if (string.IsNullOrEmpty(defAttributeStr2))
							{
								tenAwardData.EndTime = DateTime.MaxValue;
							}
							else
							{
								tenAwardData.EndTime = DateTime.Parse(defAttributeStr2);
							}
							string[] array = defAttributeStr3.Split(new char[]
							{
								','
							});
							tenAwardData.RoleLevel = int.Parse(array[0]) * 100 + int.Parse(array[1]);
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "AwardGoods");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] fields = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								tenAwardData.AwardGoods = GoodsHelper.ParseGoodsDataList(fields, text);
							}
							TenManager._tenAwardDic.Add(tenAwardData.AwardID, tenAwardData);
						}
					}
					TenManager.initDb();
				}
				catch (Exception)
				{
					LogManager.WriteLog(2, "加载Config/TenAward.xml时文件出现异常!!!", null, true);
					Process.GetCurrentProcess().Kill();
					return false;
				}
				result = true;
			}
			return result;
		}

		public static void initDb()
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(6))
			{
				if (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("Ten"))
				{
					string text = "";
					foreach (TenAwardData tenAwardData in TenManager._tenAwardDic.Values)
					{
						if (text.Length > 0)
						{
							text += "#";
						}
						string text2 = "";
						if (tenAwardData.AwardGoods != null && tenAwardData.AwardGoods.Count > 0)
						{
							foreach (GoodsData goodsData in tenAwardData.AwardGoods)
							{
								if (text2.Length > 0)
								{
									text2 += "|";
								}
								text2 += string.Format("{0},{1},{2}", goodsData.GoodsID, goodsData.GCount, goodsData.Binding);
							}
						}
						text += string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
						{
							tenAwardData.AwardID,
							tenAwardData.DbKey,
							tenAwardData.OnlyNum,
							tenAwardData.DayMaxNum,
							text2,
							tenAwardData.MailTitle,
							tenAwardData.MailContent,
							tenAwardData.MailUser,
							tenAwardData.BeginTime.ToString("yyyyMMddHHmmss"),
							tenAwardData.EndTime.ToString("yyyyMMddHHmmss"),
							tenAwardData.RoleLevel
						});
					}
					string[] array = null;
					Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 13113, text, out array, 0);
				}
			}
		}

		private static TenManager instance = new TenManager();

		private static Dictionary<int, TenAwardData> _tenAwardDic = new Dictionary<int, TenAwardData>();
	}
}
