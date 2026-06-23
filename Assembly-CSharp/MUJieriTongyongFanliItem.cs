using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class MUJieriTongyongFanliItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodList.ItemsSource;
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_ActivityTypes == ActivityTypes.JieriShouli)
			{
				GameInstance.Game.GetJieriShouliRewardCmd(this.Id);
			}
			else
			{
				GameInstance.Game.SpriteFetchActivityAward((int)this.m_ActivityTypes, this.Id);
			}
		};
	}

	private void InitTextInPrefabs()
	{
		this.lingquBtn.Label.text = Global.GetLang("领取");
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
		}
	}

	public int NeedExt
	{
		get
		{
			return this.needExt;
		}
		set
		{
			this.needExt = value;
		}
	}

	public string Need1
	{
		get
		{
			return this._need;
		}
		set
		{
			this._need = value;
			this.labNeed.text = value;
		}
	}

	public ActivityTypes ActivityTypes
	{
		get
		{
			return this.m_ActivityTypes;
		}
		set
		{
			this.m_ActivityTypes = value;
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
				this.lingquBtn.isEnabled = true;
				this.stateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.Gained:
				this.stateSpriteGained.gameObject.SetActive(true);
				this.lingquBtn.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.lingquBtn.isEnabled = false;
				this.stateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			}
		}
	}

	public ListBox goodList;

	public TextBlock labNeed;

	public GButton lingquBtn;

	public UISprite stateSpriteGained;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;

	private int id;

	private int need;

	private int needExt;

	private string _need = string.Empty;

	private ActivityTypes m_ActivityTypes = ActivityTypes.JieriWing;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
}
