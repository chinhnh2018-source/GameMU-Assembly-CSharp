using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class FaQiJieMengWindow : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ServerTitle.lineWidth = 0;
		this.ZhanMengTitle.lineWidth = 0;
		this.ServerTitle.pivot = 5;
		this.ZhanMengTitle.pivot = 5;
		this.jieMengCost = ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("AlignCostMoney", true));
		this.title.text = Global.GetLang("添加盟友");
		string text = string.Format("{0}{1}{2}", Global.GetLang("发起结盟需要消耗"), this.jieMengCost, Global.GetLang("战盟资金"));
		this.tipsLabel.Text = text;
		this.ServerTitle.text = Global.GetLang("输入服务器：");
		this.ZhanMengTitle.text = Global.GetLang("输入战盟名：");
		this.inputServer.label.text = Global.GetLang("服务器名");
		this.InputZhanMengNum.label.text = Global.GetLang("战盟名字");
		this.btnSend.Label.text = Global.GetLang("发送申请");
		this.InitInputBox();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnSend.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			switch (this.IsInputValueNull())
			{
			case 1:
				Super.HintMainText(Global.GetLang("输入服务器有误，请重试！"), 10, 3);
				break;
			case 2:
				Super.HintMainText(Global.GetLang("战盟名字不存在或等级未达到5级"), 10, 3);
				break;
			case 3:
				if (this.CanSendJieMengRequest())
				{
					GameInstance.Game.SendFaQiJieMengRequest(ConvertExt.SafeConvertToInt32(this.GetZoneID()), this.GetZhanMengName());
				}
				else
				{
					Super.HintMainText(Global.GetLang("战盟资金不足"), 10, 3);
				}
				break;
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.gameObject.SetActive(false);
		};
	}

	public void InitInputBox()
	{
		this.inputServer.Text = Global.GetLang("请在此输入");
		this.InputZhanMengNum.Text = Global.GetLang("发送申请");
		this.inputServer.text = string.Empty;
		this.InputZhanMengNum.text = string.Empty;
	}

	private string GetZoneID()
	{
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(Global.Data.roleData.ZoneID, out ztBuffServerInfo))
		{
			if (Global.Data != null && Global.Data.ServerData != null && Global.Data.ServerData.ServerListData != null && Global.Data.ServerData.ServerListData.listServerData != null)
			{
				int count = Global.Data.ServerData.ServerListData.listServerData.Count;
				for (int i = 0; i < count; i++)
				{
					FistLevelServerListData fistLevelServerListData = Global.Data.ServerData.ServerListData.listServerData[i];
					if (Global.Data.ServerData.LastServer == null || fistLevelServerListData.nFirstLevelServerID == Global.Data.ServerData.LastServer.nFirstLevelServerID)
					{
						int count2 = fistLevelServerListData.listServerData.Count;
						for (int j = 0; j < count2; j++)
						{
							SecondLevelServerListData secondLevelServerListData = fistLevelServerListData.listServerData[j];
							int count3 = secondLevelServerListData.listServerData.Count;
							for (int k = 0; k < count3; k++)
							{
								if (secondLevelServerListData.listServerData[k].strServerName == this.inputServer.text)
								{
									return secondLevelServerListData.listServerData[k].nServerID.ToString();
								}
							}
						}
					}
				}
			}
			return "-1";
		}
		return (!string.IsNullOrEmpty(this.inputServer.text)) ? this.inputServer.text : Global.GetLang("输入错误");
	}

	private string GetZhanMengName()
	{
		return (!string.IsNullOrEmpty(this.InputZhanMengNum.text)) ? this.InputZhanMengNum.text : Global.GetLang("输入错误");
	}

	private int IsInputValueNull()
	{
		int result;
		if (string.IsNullOrEmpty(this.inputServer.text))
		{
			result = 1;
		}
		else if (string.IsNullOrEmpty(this.InputZhanMengNum.text))
		{
			result = 2;
		}
		else
		{
			result = 3;
		}
		return result;
	}

	private bool CanSendJieMengRequest()
	{
		if (this.jieMengCost == 0)
		{
			this.jieMengCost = ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("AlignCostMoney", true));
		}
		return Global.zhanmengZiJin >= (long)this.jieMengCost;
	}

	public void NotifyResult()
	{
		base.gameObject.SetActive(false);
	}

	public UILabel title;

	public UILabel tips;

	public UILabel ServerTitle;

	public UILabel ZhanMengTitle;

	public TextBox inputServer;

	public TextBox InputZhanMengNum;

	public TextBlock tipsLabel;

	public GButton btnClose;

	public GButton btnSend;

	private int jieMengCost;
}
