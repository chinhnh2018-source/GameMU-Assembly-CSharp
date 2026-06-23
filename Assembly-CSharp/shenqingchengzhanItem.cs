using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;

public class shenqingchengzhanItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnTaijia.Text = Global.GetLang("抬价");
		if (Context.IsHaiwai)
		{
			this.lblFamilyName.text = string.Empty;
			this.btnJingjia.Text = Global.GetLang("竞价");
		}
		this.btnJingjia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.ChengZhanJingJia(this.Site, this.NeedMoney.SafeToInt32(0));
		};
		this.btnTaijia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.ChengZhanJingJia(this.Site, this.NeedMoney.SafeToInt32(0));
		};
	}

	public int Site
	{
		get
		{
			return this.site;
		}
		set
		{
			this.site = value;
		}
	}

	public string CurrentMoney
	{
		get
		{
			return this.lblCurrentMoney.text;
		}
		set
		{
			this.lblCurrentMoney.text = value;
		}
	}

	public string NeedMoney
	{
		get
		{
			return this.lblNeedMoney.text;
		}
		set
		{
			this.lblNeedMoney.text = value;
		}
	}

	public string FamilyName
	{
		get
		{
			return this.lblFamilyName.text;
		}
		set
		{
			this.lblFamilyName.text = value;
		}
	}

	public void RefreshBtnStatus(int status)
	{
		switch (status)
		{
		case 0:
			this.btnJingjia.enabled = false;
			this.btnTaijia.enabled = false;
			break;
		case 1:
			this.btnJingjia.gameObject.SetActive(true);
			this.btnTaijia.gameObject.SetActive(false);
			break;
		case 2:
			this.btnJingjia.gameObject.SetActive(false);
			this.btnTaijia.gameObject.SetActive(true);
			break;
		}
	}

	public UILabel lblCurrentMoney;

	public UILabel lblNeedMoney;

	public UILabel lblFamilyName;

	public GButton btnJingjia;

	public GButton btnTaijia;

	private int site;
}
