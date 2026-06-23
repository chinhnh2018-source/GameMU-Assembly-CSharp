using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class FaceBookManager : IManager
	{
		public static FaceBookManager getInstance()
		{
			return FaceBookManager.instance;
		}

		public bool initialize()
		{
			return FaceBookManager.initFacebook();
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

		public static bool initFacebook()
		{
			string uri = "Config/FacebookAward.xml";
			XElement xelement = ConfigHelper.Load(Global.GameResPath(uri));
			bool result;
			if (null == xelement)
			{
				LogManager.WriteLog(2, "加载Config/FacebookAward.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					FaceBookManager._FacebookAwards.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							FacebookAwardData facebookAwardData = new FacebookAwardData();
							facebookAwardData.AwardID = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "ID"));
							facebookAwardData.AwardName = Global.GetSafeAttributeStr(xelement2, "Name");
							facebookAwardData.DbKey = Global.GetSafeAttributeStr(xelement2, "DbKey");
							facebookAwardData.DayMaxNum = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "DayMaxNum"));
							facebookAwardData.OnlyNum = Convert.ToInt32(Global.GetSafeAttributeLong(xelement2, "OnlyNum"));
							facebookAwardData.MailUser = GLang.GetLang(112, new object[0]);
							facebookAwardData.MailTitle = Global.GetSafeAttributeStr(xelement2, "MailTitle");
							facebookAwardData.MailContent = Global.GetSafeAttributeStr(xelement2, "MailContent");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "AwardGoods");
							if (safeAttributeStr.Length > 0)
							{
								facebookAwardData.AwardGoods = new List<GoodsData>();
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								foreach (string text in array)
								{
									string[] array3 = text.Split(new char[]
									{
										','
									});
									GoodsData goodsData = new GoodsData();
									goodsData.Id = Convert.ToInt32(array3[0]);
									goodsData.GCount = Convert.ToInt32(array3[1]);
									goodsData.Binding = Convert.ToInt32(array3[2]);
									facebookAwardData.AwardGoods.Add(goodsData);
								}
							}
							FaceBookManager._FacebookAwards.Add(facebookAwardData.AwardID, facebookAwardData);
						}
					}
					FaceBookManager.initFacebookDb();
				}
				catch (Exception)
				{
					LogManager.WriteLog(2, "加载Config/FacebookAward.xml时文件出现异常!!!", null, true);
					Process.GetCurrentProcess().Kill();
					return false;
				}
				result = true;
			}
			return result;
		}

		private static void initFacebookDb()
		{
			string text = "";
			foreach (FacebookAwardData facebookAwardData in FaceBookManager._FacebookAwards.Values)
			{
				if (text.Length > 0)
				{
					text += "#";
				}
				string text2 = "";
				if (facebookAwardData.AwardGoods != null && facebookAwardData.AwardGoods.Count > 0)
				{
					foreach (GoodsData goodsData in facebookAwardData.AwardGoods)
					{
						if (text2.Length > 0)
						{
							text2 += "|";
						}
						text2 += string.Format("{0},{1},{2}", goodsData.Id, goodsData.GCount, goodsData.Binding);
					}
				}
				text += string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					facebookAwardData.AwardID,
					facebookAwardData.DbKey,
					facebookAwardData.OnlyNum,
					facebookAwardData.DayMaxNum,
					text2,
					facebookAwardData.MailTitle,
					facebookAwardData.MailContent,
					facebookAwardData.MailUser
				});
			}
			string[] array = null;
			Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 21000, text, out array, 0);
		}

		private static FaceBookManager instance = new FaceBookManager();

		private static Dictionary<int, FacebookAwardData> _FacebookAwards = new Dictionary<int, FacebookAwardData>();
	}
}
