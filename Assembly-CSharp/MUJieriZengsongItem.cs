using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;

public class MUJieriZengsongItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodList.ItemsSource;
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetJieriZengsongRewardCmd(this.Id);
		};
	}

	private void InitTextInPrefabs()
	{
		this.lingquBtn.Label.text = Global.GetLang("领 取");
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
				string.Format(Global.GetLang("赠送{0}个礼物"), this.Need)
			});
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
				this.lingquBtn.isEnabled = true;
				this.lingquBtn.gameObject.SetActive(true);
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

	public ObservableCollection ItemCollection;

	private int id;

	private int need;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
}
