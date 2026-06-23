using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TuJianDetailItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("前往获得");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this._MonsterID
			});
		};
	}

	public bool IsActived
	{
		set
		{
			this.SubmitBtn.gameObject.SetActive(!value);
			this.SpriteActived.gameObject.SetActive(value);
		}
	}

	public int MonsterID
	{
		get
		{
			return this._MonsterID;
		}
		set
		{
			this._MonsterID = value;
		}
	}

	public int TujianID
	{
		get
		{
			return this._TujianID;
		}
		set
		{
			this._TujianID = value;
		}
	}

	public int MaxNum
	{
		get
		{
			return this._MaxNum;
		}
		set
		{
			this._MaxNum = value;
		}
	}

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	public bool IsSubmitBtnEnable
	{
		set
		{
			if (null != this.SubmitBtn)
			{
				this.SubmitBtn.isEnabled = value;
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL Goods;

	public TextBlock TxtName;

	public TextBlock TxtName2;

	public TextBlock TxtProps;

	public GButton SubmitBtn;

	public UISprite SpriteActived;

	private int _MonsterID;

	private int _TujianID;

	private int _MaxNum;

	private int _GoodsID;
}
