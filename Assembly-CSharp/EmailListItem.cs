using System;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;

public class EmailListItem : UserControl
{
	public int EmailID
	{
		get
		{
			return this._EmailID;
		}
		set
		{
			this._EmailID = value;
		}
	}

	public int Type
	{
		get
		{
			return this._Type;
		}
		set
		{
			this._Type = value;
			this.SetTypeView();
		}
	}

	public string Time
	{
		get
		{
			return this._Time;
		}
		set
		{
			this._Time = value;
		}
	}

	public string Title
	{
		get
		{
			return this.m_EmailBiaoTi.text;
		}
		set
		{
			this.m_EmailBiaoTi.text = value;
		}
	}

	public string From
	{
		get
		{
			return this._From;
		}
		set
		{
			this._From = value;
		}
	}

	public string NeiRong
	{
		get
		{
			return this._NeiRong;
		}
		set
		{
			this._NeiRong = value;
		}
	}

	public bool State
	{
		get
		{
			return this._State;
		}
		set
		{
			this._State = value;
			this.SetStateView();
		}
	}

	public bool CheckState
	{
		get
		{
			return this._CheckState;
		}
		set
		{
			this._CheckState = value;
		}
	}

	public void OnValueChanged(bool active)
	{
		if (null != this.checkbox)
		{
			this.checkbox.gameObject.SetActive(active);
		}
	}

	public int TongQianNum
	{
		get
		{
			return this._TongQianNum;
		}
		set
		{
			this._TongQianNum = value;
		}
	}

	public int YinLiangNum
	{
		get
		{
			return this._YinLiangNum;
		}
		set
		{
			this._YinLiangNum = value;
		}
	}

	public string GoodsIDList
	{
		get
		{
			return this._GoodsIDList;
		}
		set
		{
			this._GoodsIDList = value;
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public ImageBrush SelectBackground
	{
		set
		{
		}
	}

	public double SelectWidth
	{
		get
		{
			return 0.0;
		}
		set
		{
			this.SelectWidth = 0.0;
		}
	}

	public double SelectHeight
	{
		get
		{
			return 0.0;
		}
		set
		{
			this.SelectHeight = 0.0;
		}
	}

	public void SetStateView()
	{
		if (this._State)
		{
		}
	}

	public void SetTypeView()
	{
	}

	protected override void InitializeComponent()
	{
	}

	public UILabel m_EmailBiaoTi;

	public UILabel m_LblEmailRecvTime;

	public UILabel m_Time;

	public UISprite m_SpriteStateNoRead;

	public UISprite m_SpriteStateIsRead;

	public UISprite m_HotSpriteBak;

	public GCheckBox m_ChkSelectEmail;

	public UITexture checkbox;

	public MailData mailData;

	private int _EmailID = -1;

	private int _Type = -1;

	private string _Time = string.Empty;

	private string _From = string.Empty;

	private string _NeiRong = string.Empty;

	private bool _State;

	private bool _CheckState;

	private int _TongQianNum;

	private int _YinLiangNum;

	private string _GoodsIDList;
}
