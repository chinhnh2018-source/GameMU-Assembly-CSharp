using System;
using HSGameEngine.GameEngine.Logic;

public class HuobiPartItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnGo.Text = Global.GetLang("前 往");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public string Icon
	{
		get
		{
			return this.texIcon.spriteName;
		}
		set
		{
			this.texIcon.spriteName = value;
		}
	}

	public new string Count
	{
		get
		{
			return this.lblCount.text;
		}
		set
		{
			this.lblCount.text = value;
		}
	}

	public string Title
	{
		get
		{
			return this.lblTitle.text;
		}
		set
		{
			this.lblTitle.text = value;
		}
	}

	public string Desc
	{
		get
		{
			return this.lblDesc.text;
		}
		set
		{
			this.lblDesc.text = value;
		}
	}

	public bool EnableBtn
	{
		set
		{
			this.btnGo.gameObject.SetActive(value);
		}
	}

	public UISprite texIcon;

	public UILabel lblCount;

	public UILabel lblTitle;

	public UILabel lblDesc;

	public GButton btnGo;
}
