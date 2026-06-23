using System;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class InputFanLiNew : Activity
	{
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/SanZhouNian_ChongZhiFanLi.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/SanZhouNian_ChongZhiFanLi.xml"));
				if (null == xelement)
				{
					return false;
				}
				this.ActivityType = 48;
				XElement xelement2 = xelement.Element("SanZhouNian_ChongZhiFanLi");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "HuoDongKaiQi");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "HuoDongGuanBi");
					this.AwardStartDate = this.FromDate;
					this.AwardEndDate = this.ToDate;
					this.InputFanLiNewData.ChongZhiJinEList = Global.GetSafeAttributeIntArray(xelement2, "ChongZhiJinE", -1, ',');
					this.InputFanLiNewData.FanZuanShuLiangList = Global.GetSafeAttributeIntArray(xelement2, "FanZuanShuLiang", -1, ',');
					this.InputFanLiNewData.XiaoFeiZuanShiList = Global.GetSafeAttributeIntArray(xelement2, "XiaoFeiZuanShi", -1, ',');
					this.OpenStateVavle = (((int)Global.GetSafeAttributeLong(xelement2, "HuoDongKaiGuan") > 0) ? 1 : 0);
					if (this.InputFanLiNewData.ChongZhiJinEList == null || this.InputFanLiNewData.FanZuanShuLiangList == null || null == this.InputFanLiNewData.XiaoFeiZuanShiList)
					{
						LogManager.WriteLog(1000, string.Format("{0}解析出现异常", "Config/SanZhouNian_ChongZhiFanLi.xml"), null, true);
						return false;
					}
					bool flag = this.InputFanLiNewData.ChongZhiJinEList.Length == this.InputFanLiNewData.FanZuanShuLiangList.Length;
					if (!(flag & this.InputFanLiNewData.ChongZhiJinEList.Length == this.InputFanLiNewData.XiaoFeiZuanShiList.Length))
					{
						LogManager.WriteLog(1000, string.Format("{0}解析出现异常", "Config/SanZhouNian_ChongZhiFanLi.xml"), null, true);
						return false;
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/SanZhouNian_ChongZhiFanLi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public void OnRoleLogin(GameClient client)
		{
			if (!this.InActivityTime())
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					15,
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
					15,
					this.OpenStateVavle,
					"",
					0,
					0
				});
				client.sendCmd(770, cmdData, false);
			}
		}

		public int GetAwardIndex(GameClient client, int chargeMoney, int consumeMoney)
		{
			int result = 0;
			for (int i = 0; i < this.InputFanLiNewData.ChongZhiJinEList.Length; i++)
			{
				int num = Global.TransMoneyToYuanBao(this.InputFanLiNewData.ChongZhiJinEList[i]);
				if (chargeMoney >= num && consumeMoney >= this.InputFanLiNewData.XiaoFeiZuanShiList[i])
				{
					result = i + 1;
				}
			}
			return result;
		}

		public override bool CanGiveAward(GameClient client, int index, int totalMoney)
		{
			return this.InAwardTime() && 0 != this.OpenStateVavle && index > 0;
		}

		public override bool GiveAward(GameClient client, int index)
		{
			bool result;
			if (index <= 0 || index > this.InputFanLiNewData.FanZuanShuLiangList.Length)
			{
				result = false;
			}
			else
			{
				int num = this.InputFanLiNewData.FanZuanShuLiangList[index - 1];
				GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "充值返利新", ActivityTypes.None, "");
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
				{
					num
				}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
				GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, num, "充值返利新"), null, client.ServerId);
				result = true;
			}
			return result;
		}

		protected const string InputFanLiNewData_fileName = "Config/SanZhouNian_ChongZhiFanLi.xml";

		protected InputFanLiNewConfig InputFanLiNewData = new InputFanLiNewConfig();

		protected int OpenStateVavle = 0;
	}
}
