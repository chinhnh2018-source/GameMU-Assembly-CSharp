using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class BloodCastleEndPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._TiShiText.Text = string.Empty;
		this._Submit.Text = string.Empty;
		this._Submit.Text = Global.GetLang("领取奖励");
		this._TiShiText.Text = Global.GetLang("选择两倍或三倍时可以获得更多经验奖励！");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			int index = 1;
			if (this.checkBox2.isChecked)
			{
				index = 2;
			}
			else if (this.checkBox3.isChecked)
			{
				index = 3;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Index = index
				});
			}
		};
		this.checkBox1.Check = true;
		this.checkBox1.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.SetTishiText(1);
		};
		this.checkBox2.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.SetTishiText(2);
		};
		this.checkBox3.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.SetTishiText(3);
		};
	}

	protected void Close(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -1,
				IDType = -1
			});
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void InitPartData(string Title, bool success, int score, int expr, int jinBi, string awardsString1, string awardsString2)
	{
		this.checkBox1.Check = true;
		this._Title.Text = Title + ((!success) ? Global.GetLang("失败") : Global.GetLang("胜利"));
		this._Score.Text = string.Format(Global.GetLang("{0}"), score);
		this.Exp = expr;
		this._AwardExpr.Text = string.Format(Global.GetLang("{0}"), expr);
		this._AwardJinBi.Text = string.Format(Global.GetLang("{0}"), jinBi);
		if (success)
		{
			this._AwardGoodsList.Visibility = true;
			if (!string.IsNullOrEmpty(awardsString1))
			{
				awardsString2 = awardsString1 + "|" + awardsString2;
			}
			this._AwardGoodsList.ItemsSource.Clear();
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(awardsString2, 0, int.MaxValue);
			UIHelper.AddAwardGoods(this._AwardGoodsList.ItemsSource, goodsList, null, false, "bagGrid4_bak", false);
		}
		else
		{
			this._AwardGoodsList.Visibility = false;
		}
	}

	private void SetTishiText(int index)
	{
		if (index == 1)
		{
			this._TiShiText.Text = Global.GetLang("选择两倍或三倍时可以获得更多经验奖励！");
		}
		else
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BloodCastleExp", ',');
			string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "EMoDuoBei", systemParamIntArrayByName[index - 2]);
			this._TiShiText.Text = string.Format(Global.GetLang("消耗{0}{1}领取{2}倍经验！"), systemParamIntArrayByName[index - 2], text, index);
		}
		this.SetExpText(index);
	}

	private void SetExpText(int index)
	{
		if (index > 1)
		{
			this._AwardExpr.Text = string.Format(Global.GetLang("{0} x {1}"), this.Exp, index);
		}
		else
		{
			this._AwardExpr.Text = string.Format(Global.GetLang("{0}"), this.Exp);
		}
	}

	public GButton _Close;

	public ShowNetImage _Bak;

	public GButton _Submit;

	public GCheckBox checkBox1;

	public GCheckBox checkBox2;

	public GCheckBox checkBox3;

	public UISprite _Title_Bak;

	public TextBlock _Title;

	public TextBlock _Score;

	public TextBlock _AwardExpr;

	public TextBlock _TiShiText;

	public TextBlock _AwardJinBi;

	public ListBox _AwardGoodsList;

	private int Exp;

	public DPSelectedItemEventHandler DPSelectedItem;
}
