using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class BuyFundsPartGoodItem : UserControl
{
	public int ID
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

	public int MainID
	{
		get
		{
			return this.mainID;
		}
		set
		{
			this.mainID = value;
		}
	}

	public int FundType
	{
		get
		{
			return this.fundType;
		}
		set
		{
			this.fundType = value;
		}
	}

	public int RewardCount
	{
		get
		{
			return this.rewardCount;
		}
		set
		{
			this.rewardCount = value;
		}
	}

	public int RewardType
	{
		set
		{
			if (value == 0)
			{
				this.diamond.spriteName = "8013";
				this.needDiamond.text = Global.GetColorStringForNGUIText(new object[]
				{
					"28b1ff",
					string.Format(Global.GetLang("{0}钻石"), this.RewardCount)
				});
			}
			else if (value == 1)
			{
				this.diamond.spriteName = "8012";
				this.needDiamond.text = string.Format(Global.GetLang("{0}绑定钻石"), this.RewardCount);
			}
		}
	}

	public string Attribute
	{
		set
		{
			this.miaoshu.text = string.Format("{0}", value);
		}
	}

	public int State
	{
		set
		{
			if (value == 1)
			{
				this.yilingqu.gameObject.SetActive(true);
				this.weidacheng.gameObject.SetActive(false);
				this.BtnLingqu.gameObject.SetActive(false);
			}
			else if (value == 2)
			{
				this.IsCanGet = true;
				this.yilingqu.gameObject.SetActive(false);
				this.weidacheng.gameObject.SetActive(false);
				this.BtnLingqu.gameObject.SetActive(true);
			}
			else if (value == 3)
			{
				this.yilingqu.gameObject.SetActive(false);
				this.weidacheng.gameObject.SetActive(true);
				this.BtnLingqu.gameObject.SetActive(false);
			}
			else if (value == 4)
			{
				this.yilingqu.gameObject.SetActive(false);
				this.weidacheng.gameObject.SetActive(false);
				this.BtnLingqu.gameObject.SetActive(false);
			}
		}
	}

	public string GoalNum
	{
		get
		{
			return this.goalNum;
		}
		set
		{
			this.goalNum = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnLingqu.Label.text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsCanGet)
			{
				Super.HintMainText(Global.GetLang("当前奖项不能领取，属于违规操作！"), 10, 3);
				return;
			}
			GameInstance.Game.GetFundAward(this.FundType);
			this.DPSelectItem(this, new DPSelectedItemEventArgs
			{
				IDType = this.FundType,
				ID = this.MainID,
				Index = this.ID
			});
		};
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton BtnLingqu;

	public UISprite diamond;

	public UISprite weidacheng;

	public UISprite yilingqu;

	public UILabel needDiamond;

	public UILabel miaoshu;

	private int id;

	private int mainID;

	private int fundType = 1;

	private int rewardCount;

	private string goalNum;

	private bool IsCanGet;
}
