using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;

public class MUJieRiMeiRiPingTaiChongZhiKingItem : UserControl
{
	public int XmlID
	{
		get
		{
			return this.mID;
		}
		set
		{
			this.mID = value;
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodList.ItemsSource;
		this.btnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.RequestMeiRiPlatformAward(this.XmlID);
		};
		this.lingquBtn.gameObject.SetActive(false);
	}

	private void InitTextInPrefabs()
	{
		this.lingquBtn.Label.text = Global.GetLang("领  取");
		this.btnChongZhi.Label.text = Global.GetLang("充  值");
	}

	public int Id
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public int ZoneID
	{
		get
		{
			return this._ZoneID;
		}
		set
		{
			this._ZoneID = value;
		}
	}

	public string Rank
	{
		get
		{
			if (this._rank == "None")
			{
				return "0";
			}
			return this._rank;
		}
		set
		{
			this._rank = value;
			if (this._rank == "None")
			{
				this.mRank.gameObject.SetActive(false);
			}
			else
			{
				this.mRank.Text = string.Format("{0} {1} {2}", Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("第")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"ffd801",
					this._rank.ToString()
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("名")
				}));
			}
		}
	}

	public string RoleName
	{
		set
		{
			if (value == "None")
			{
				this.mRoleName.gameObject.SetActive(false);
			}
			else
			{
				ZtBuffServerInfo ztBuffServerInfo = null;
				if (this.ZoneID > 0 && Global.GetNowServerIsZhuTiFu(this.ZoneID, out ztBuffServerInfo))
				{
					this.mRoleName.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"6d8599",
						Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, value, 0)
					});
				}
				else if (this.ZoneID < 0 || string.IsNullOrEmpty(value))
				{
					this.mRoleName.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"6d8599",
						Global.GetLang("无")
					});
				}
				else
				{
					this.mRoleName.Text = Global.GetColorStringForNGUIText(new object[]
					{
						"6d8599",
						string.Format("S{0}.{1}", this.ZoneID, value)
					});
				}
			}
		}
	}

	public int Need
	{
		get
		{
			return this.need;
		}
		set
		{
			this.need = value;
			this.labNeed.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("累计充值{0}钻石"), this.Need)
			});
		}
	}

	public int Day
	{
		get
		{
			return this._day;
		}
		set
		{
			this._day = value;
		}
	}

	public JieriAwardGiftGainState AwardGiftGainState
	{
		get
		{
			return this.m_AwardGiftGainState;
		}
		set
		{
			this.m_AwardGiftGainState = value;
			switch (this.m_AwardGiftGainState)
			{
			case JieriAwardGiftGainState.CanGain:
				this.lingquBtn.gameObject.SetActive(true);
				this.stateSpriteGained.gameObject.SetActive(false);
				this.btnChongZhi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.Gained:
				this.stateSpriteGained.gameObject.SetActive(true);
				this.lingquBtn.gameObject.SetActive(false);
				this.btnChongZhi.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.btnChongZhi.gameObject.SetActive(true);
				this.lingquBtn.gameObject.SetActive(false);
				this.stateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.OverTime:
				this.btnChongZhi.isEnabled = false;
				break;
			}
		}
	}

	public TextBlock mRank;

	public TextBlock mRoleName;

	public ListBox goodList;

	public TextBlock labNeed;

	public GButton lingquBtn;

	public GButton btnChongZhi;

	public UISprite stateSpriteGained;

	private int mID;

	private ObservableCollection _ItemCollection;

	private int id;

	private int _ZoneID;

	private string _rank = string.Empty;

	private int need;

	private int _day;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
}
