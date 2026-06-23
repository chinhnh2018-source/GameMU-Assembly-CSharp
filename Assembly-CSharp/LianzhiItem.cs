using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class LianzhiItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Title.MakePixelPerfect();
	}

	public bool SelectStat
	{
		get
		{
			return this.SelectBorder.gameObject.activeSelf;
		}
		set
		{
			this.SelectBorder.gameObject.SetActive(value);
		}
	}

	public TaskAwardsData AwardsData
	{
		get
		{
			return this._AwardsData;
		}
		set
		{
			this._AwardsData = value;
		}
	}

	public LianzhiTypes Type
	{
		get
		{
			return this._Type;
		}
		set
		{
			this._Type = value;
			TaskAwardsData taskAwardsData = new TaskAwardsData();
			if (value == LianzhiTypes.Diamond)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ZuanShiLianZhi", ',');
				if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length != 5)
				{
					return;
				}
				this.Title.spriteName = this.TitleNames[0];
				this.TextAwardFrom.Name = UIHelper.GetAwardsName(AwardsTypes.ZuanShi, 1);
				this.LianzhiFromMoneyValue = systemParamIntArrayByName[0];
				this.TextAwardFrom.Value = (long)this.LianzhiFromMoneyValue;
				if (Global.Data.roleData.UserMoney >= this.LianzhiFromMoneyValue)
				{
					this.TextAwardFrom.textColor = 16777215U;
				}
				else
				{
					this.TextAwardFrom.textColor = 16711680U;
				}
				taskAwardsData.Experienceaward = Global.GetExpMultiByZhuanShengExpXiShu((long)systemParamIntArrayByName[1]);
				taskAwardsData.XingHunaward = systemParamIntArrayByName[2];
				taskAwardsData.Moneyaward = systemParamIntArrayByName[3];
				UIHelper.AddAwardData(this.AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
				int systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPZuanShiLianZhi");
				int nowNum = systemParamIntArrayByName[4] + systemParamVipLeveValue;
				int lianzhiNum = this.FanBei_CiShu(nowNum);
				this.LianzhiNum = lianzhiNum;
			}
			else if (value == LianzhiTypes.BindDiamond)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BangZuanLianZhi", ',');
				if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length != 3)
				{
					return;
				}
				this.Title.spriteName = this.TitleNames[1];
				this.TextAwardFrom.Name = UIHelper.GetAwardsName(AwardsTypes.BindZuanShi, 1);
				this.LianzhiFromMoneyValue = systemParamIntArrayByName[0];
				this.TextAwardFrom.Value = (long)this.LianzhiFromMoneyValue;
				if (Global.Data.roleData.Gold >= this.LianzhiFromMoneyValue)
				{
					this.TextAwardFrom.textColor = 16777215U;
				}
				else
				{
					this.TextAwardFrom.textColor = 16711680U;
				}
				taskAwardsData.XingHunaward = systemParamIntArrayByName[1];
				UIHelper.AddAwardData(this.AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
				int systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPBangZuanLianZhi");
				int nowNum2 = systemParamIntArrayByName[2] + systemParamVipLeveValue;
				int lianzhiNum2 = this.FanBei_CiShu(nowNum2);
				this.LianzhiNum = lianzhiNum2;
			}
			else if (value == LianzhiTypes.Gold)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JinBiLianZhi", ',');
				if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length != 3)
				{
					return;
				}
				this.Title.spriteName = this.TitleNames[2];
				this.TextAwardFrom.Name = UIHelper.GetAwardsName(AwardsTypes.BindJinBi, 1);
				this.LianzhiFromMoneyValue = systemParamIntArrayByName[0];
				this.TextAwardFrom.Value = (long)this.LianzhiFromMoneyValue;
				if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= this.LianzhiFromMoneyValue)
				{
					this.TextAwardFrom.textColor = 16777215U;
				}
				else
				{
					this.TextAwardFrom.textColor = 16711680U;
				}
				taskAwardsData.Experienceaward = Global.GetExpMultiByZhuanShengExpXiShu((long)systemParamIntArrayByName[1]);
				UIHelper.AddAwardData(this.AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
				int systemParamVipLeveValue = Global.GetSystemParamVipLeveValue("VIPJinBiLianZhi");
				int nowNum3 = systemParamIntArrayByName[2] + systemParamVipLeveValue;
				int lianzhiNum3 = this.FanBei_CiShu(nowNum3);
				this.LianzhiNum = lianzhiNum3;
			}
			this.AwardsData = taskAwardsData;
		}
	}

	private int FanBei_CiShu(int NowNum)
	{
		if (Global.isFanbei(6))
		{
			NowNum *= 2;
		}
		if (Global.isFanbei(209))
		{
			double num = 0.0;
			if (double.TryParse(Global.JieriFanbeiInfo[209].ExtArg1, ref num))
			{
				NowNum += (int)num;
			}
		}
		return NowNum;
	}

	public int LianzhiFromMoneyValue
	{
		get
		{
			return this._LianzhiFromMoneyValue;
		}
		set
		{
			this._LianzhiFromMoneyValue = value;
		}
	}

	public int LianzhiNum
	{
		get
		{
			return this._LianzhiNum;
		}
		set
		{
			this._LianzhiNum = value;
			string text = "ffffff";
			if (this.LianzhiNum <= 0)
			{
				text = "ff0000";
			}
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				this.LianzhiNum.ToString()
			});
			this.TxtLianzhiNum.Text = string.Format(Global.GetLang("今日可用：{0}次"), colorStringForNGUIText);
		}
	}

	public ListBox AwardsList;

	public CText TextAwardFrom;

	public TextBlock TxtLianzhiNum;

	public UISprite SelectBorder;

	public UISprite Title;

	private string[] TitleNames = new string[]
	{
		"title1",
		"title2",
		"title3"
	};

	private TaskAwardsData _AwardsData;

	private LianzhiTypes _Type = LianzhiTypes.None;

	private int _LianzhiFromMoneyValue;

	private int _LianzhiNum;
}
