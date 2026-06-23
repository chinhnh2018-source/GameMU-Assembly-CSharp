using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;

public class MUJieriMeiriLeichongItem : UserControl
{
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
			GameInstance.Game.SpriteFetchActivityAward(70, this.Day * 1000 + this.Id);
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

	public ListBox goodList;

	public TextBlock labNeed;

	public GButton lingquBtn;

	public GButton btnChongZhi;

	public UISprite stateSpriteGained;

	private ObservableCollection _ItemCollection;

	private int id;

	private int need;

	private int _day;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
}
