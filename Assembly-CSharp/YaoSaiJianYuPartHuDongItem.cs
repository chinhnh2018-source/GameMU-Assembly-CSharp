using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class YaoSaiJianYuPartHuDongItem : UserControl
{
	public int RoleID
	{
		get
		{
			return this.roleid;
		}
		set
		{
			this.roleid = value;
		}
	}

	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
			this.BG.URL = string.Format("NetImages/GameRes/Images/YaoSaiJianYuTex/{0}.png.qj", value);
		}
	}

	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
			this.UIType.spriteName = value.ToString();
		}
	}

	public int JiangliCount
	{
		set
		{
			this.JiangLiNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				value
			});
		}
	}

	private void InitTextInPrefabs()
	{
		this.JiangLiLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("通关奖励:")
		});
		this.BtnDoIt.Label.text = Global.GetLang("开始干活");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnDoIt.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendPrisonHuDongData(this.RoleID, this.ID);
			Super.ShowNetWaiting(null);
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public ShowNetImage BG;

	public UISprite UIType;

	public UILabel JiangLiNum;

	public UILabel JiangLiLab;

	public GButton BtnDoIt;

	public DPSelectedItemEventHandler CloseHandler;

	private int roleid;

	private int id;

	private int type;
}
