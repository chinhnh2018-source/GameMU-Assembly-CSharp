using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public class RegressActiveOpen : Activity
	{
		public bool Init()
		{
			RegressActiveOpen.OpenStateVavle = 0;
			this.ActivityType = 110;
			string text = Global.GameResPath("Config\\HuiGuiHuoDong.xml");
			XElement xelement = XElement.Load(text);
			if (null == xelement)
			{
				LogManager.WriteLog(1000, string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text), null, true);
			}
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				int offsetDay = Global.GetOffsetDay(now);
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RegressActiveOpenXML regressActiveOpenXML = new RegressActiveOpenXML();
					regressActiveOpenXML.ID = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "ID"));
					regressActiveOpenXML.HuoDongLevel = Convert.ToInt32(Global.GetSafeAttributeStr(xml, "HuoDongLevel"));
					regressActiveOpenXML.BeginTime = Global.GetSafeAttributeStr(xml, "BeginTime");
					regressActiveOpenXML.FinishTime = Global.GetSafeAttributeStr(xml, "FinishTime");
					int offsetDay2 = Global.GetOffsetDay(DateTime.Parse(regressActiveOpenXML.BeginTime));
					int offsetDay3 = Global.GetOffsetDay(DateTime.Parse(regressActiveOpenXML.FinishTime));
					if (offsetDay2 <= offsetDay && offsetDay <= offsetDay3)
					{
						RegressActiveOpen.OpenStateVavle = 1;
					}
					regressActiveOpenXML.RegisterBegin = Global.GetSafeAttributeStr(xml, "RegisterBegin");
					if (regressActiveOpenXML.RegisterBegin.Equals(""))
					{
						regressActiveOpenXML.RegisterBegin = "2000-01-01 00:00:00";
					}
					regressActiveOpenXML.RegisterFinish = Global.GetSafeAttributeStr(xml, "RegisterFinish");
					if (regressActiveOpenXML.RegisterFinish.Equals(""))
					{
						regressActiveOpenXML.RegisterFinish = "3000-01-01 00:00:00";
					}
					this.FromDate = regressActiveOpenXML.BeginTime;
					this.ToDate = regressActiveOpenXML.FinishTime;
					this.AwardStartDate = regressActiveOpenXML.BeginTime;
					this.AwardEndDate = regressActiveOpenXML.FinishTime;
					this.regressActiveOpenXML.Add(regressActiveOpenXML.ID, regressActiveOpenXML);
				}
				if (this.regressActiveOpenXML == null)
				{
					return false;
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return true;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					0,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					16,
					RegressActiveOpen.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public int GetUserActiveFile(string Regtime, out int ConfID)
		{
			ConfID = 0;
			foreach (RegressActiveOpenXML regressActiveOpenXML in this.regressActiveOpenXML.Values)
			{
				long num = DataHelper.ConvertToTicks(regressActiveOpenXML.RegisterBegin);
				long num2 = DataHelper.ConvertToTicks(regressActiveOpenXML.RegisterFinish);
				long num3 = DataHelper.ConvertToTicks(Regtime);
				if (num <= num3 && num3 < num2)
				{
					ConfID = regressActiveOpenXML.ID;
					return regressActiveOpenXML.HuoDongLevel;
				}
			}
			return 0;
		}

		protected const string RegressActiveOpenXml = "Config\\HuiGuiHuoDong.xml";

		private Dictionary<int, RegressActiveOpenXML> regressActiveOpenXML = new Dictionary<int, RegressActiveOpenXML>();

		public static int OpenStateVavle = 0;
	}
}
