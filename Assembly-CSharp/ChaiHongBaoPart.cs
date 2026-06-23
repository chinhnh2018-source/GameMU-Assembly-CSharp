using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChaiHongBaoPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mLblMessage.Text = Global.GetLang("金鸡报晓，紫燕衔春");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		UIEventListener.Get(this.mChaiIcon).onClick = delegate(GameObject s)
		{
			GameInstance.Game.SendHongBaoDetailsRequest(this.hongBaoType, this.hongBaoID, 1);
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void InitValue()
	{
	}

	public void NotifyInitDataByServerData(ShowHongBaoData data)
	{
		if (data == null)
		{
			return;
		}
		this.hongBaoType = data.type;
		this.hongBaoID = data.hongBaoID;
		int num = this.hongBaoType;
		switch (num)
		{
		case 101:
			this.mHongBaoName.spriteName = "chongzhihongbao";
			this.mLblName.Text = string.Empty;
			this.mLblHongBaoNum.Text = string.Empty;
			break;
		case 102:
			this.mHongBaoName.spriteName = "quanminhongbao";
			this.mLblName.Text = string.Empty;
			this.mLblHongBaoNum.Text = string.Empty;
			break;
		case 103:
			this.mHongBaoName.spriteName = "tequanhongbao";
			this.mLblName.Text = Global.GetLang("特权红包");
			this.mLblHongBaoNum.Text = string.Empty;
			break;
		default:
			if (num != 0)
			{
				if (num == 1)
				{
					this.mHongBaoName.spriteName = "hongbao_font";
					string text = string.Format("{0}{1}", data.sender, Global.GetLang("的手气红包"));
					this.mLblName.Text = text;
					string text2 = string.Format("{0}{1}/{2}", Global.GetLang("红包数量："), data.SumHongBaoNum - data.yiLingNum, data.SumHongBaoNum);
					this.mLblHongBaoNum.Text = text2;
				}
			}
			else
			{
				this.mHongBaoName.spriteName = "hongbao_font";
				string text3 = string.Format("{0}{1}", data.sender, Global.GetLang("的普通红包"));
				this.mLblName.Text = text3;
				string text4 = string.Format("{0}{1}/{2}", Global.GetLang("红包数量："), data.SumHongBaoNum - data.yiLingNum, data.SumHongBaoNum);
				this.mLblHongBaoNum.Text = text4;
			}
			break;
		}
		this.mLblMessage.Text = Global.GetLang(data.message);
	}

	protected override void OnDestroy()
	{
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mHongBaoName = null;
		this.mLblName = null;
		this.mLblMessage = null;
		this.mChaiIcon = null;
		this.hongBaoID = 0;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public UISprite mHongBaoName;

	public TextBlock mLblName;

	public TextBlock mLblMessage;

	public TextBlock mLblHongBaoNum;

	public GameObject mChaiIcon;

	private int hongBaoID;

	private int hongBaoType = -1;
}
