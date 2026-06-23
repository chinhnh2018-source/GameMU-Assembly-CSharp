using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class AngelTempleAwardPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
	}

	protected void Close(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void InitPartData(string Title, int paiMing, int awardGold, int awardShengWang, string luckPaiMingName, string goodsString)
	{
		this._Title.Text = Title;
		this._AwardShengWang.Text = string.Format(Global.GetLang("声望：{0}"), awardShengWang);
		this._AwardKilledJinBi.Text = string.Format(Global.GetLang("{0}"), luckPaiMingName);
		if (paiMing >= 0)
		{
			this._AwardJinBi.Text = string.Format(Global.GetLang("伤害排名：{0}"), paiMing);
			this._AwardGoodsList.Visibility = true;
			this._AwardGoodsList.ItemsSource.Clear();
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(goodsString, 0, int.MaxValue);
			UIHelper.AddAwardGoods(this._AwardGoodsList.ItemsSource, goodsList, null, false, "bagGrid4_bak", false);
		}
		else
		{
			this._AwardJinBi.Text = Global.GetLang("伤害排名：无");
			this._AwardGoodsList.Visibility = false;
		}
	}

	public GButton _Close;

	public ShowNetImage _Bak;

	public GButton _Submit;

	public UISprite _Title_Bak;

	public TextBlock _Title;

	public TextBlock _AwardJinBi;

	public TextBlock _AwardShengWang;

	public TextBlock _AwardKilledJinBi;

	public ListBox _AwardGoodsList;

	public DPSelectedItemEventHandler DPSelectedItem;
}
