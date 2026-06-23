using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class MUJieriLianxuChongzhiItemGoodsItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.lingquBtn.Text = Global.GetLang("领取");
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.Id,
				Index = this.NeedDays.SafeToInt32(0)
			});
		};
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
				this.LabNeedDays.gameObject.SetActive(false);
				this.GoodIcon.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.Gained:
				this.lingquBtn.gameObject.SetActive(false);
				this.stateSpriteGained.gameObject.SetActive(true);
				this.LabNeedDays.gameObject.SetActive(true);
				this.GoodIcon.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.lingquBtn.gameObject.SetActive(false);
				this.stateSpriteGained.gameObject.SetActive(false);
				this.LabNeedDays.gameObject.SetActive(true);
				this.GoodIcon.gameObject.SetActive(true);
				break;
			}
		}
	}

	public int Id
	{
		get
		{
			return this._id;
		}
		set
		{
			this._id = value;
		}
	}

	public string NeedDays
	{
		get
		{
			return this._needDays;
		}
		set
		{
			this._needDays = value;
			this.LabNeedDays.text = string.Format(Global.GetLang("累计充值{0}天"), this.NeedDays);
		}
	}

	public GGoodIcon GoodIcon;

	public GButton lingquBtn;

	public TextBlock LabNeedDays;

	public UISprite stateSpriteGained;

	public DPSelectedItemEventHandler DPSelectedItem;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;

	private int _id;

	private string _needDays = string.Empty;
}
