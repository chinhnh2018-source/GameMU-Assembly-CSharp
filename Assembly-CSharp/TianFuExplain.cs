using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TianFuExplain : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(this.ZheZhao.gameObject).onClick = delegate(GameObject s)
		{
			base.gameObject.SetActive(false);
		};
	}

	public void setTianFuShuoMing(int type)
	{
		switch (type)
		{
		case 1:
			this.TitleName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("野蛮")
			});
			this.TianFuLeiXing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("[野蛮一击]")
			});
			this.ShangHaiShuoMing.text = Global.GetLang("造成3倍伤害");
			this.JiaDianShuoMing.text = string.Concat(new string[]
			{
				Global.GetLang("分配1点野蛮天赋"),
				"\n",
				Global.GetLang("{17e43e}增加{-}野蛮一击几率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}冷血一击抵抗率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}野蛮一击抵抗率")
			});
			break;
		case 2:
			this.TitleName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("冷血")
			});
			this.TianFuLeiXing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("[冷血一击]")
			});
			this.ShangHaiShuoMing.text = Global.GetLang("造成2倍伤害，目标减速50%，持续4秒");
			this.JiaDianShuoMing.text = string.Concat(new string[]
			{
				Global.GetLang("分配1点冷血天赋"),
				"\n",
				Global.GetLang("{17e43e}增加{-}冷血一击几率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}无情一击抵抗率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}冷血一击抵抗率")
			});
			break;
		case 3:
			this.TitleName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("无情")
			});
			this.TianFuLeiXing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("[无情一击]")
			});
			this.ShangHaiShuoMing.text = Global.GetLang("造成1.5倍伤害，并恢复部分生命值");
			this.JiaDianShuoMing.text = string.Concat(new string[]
			{
				Global.GetLang("分配1点无情天赋"),
				"\n",
				Global.GetLang("{17e43e}增加{-}无情一击几率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}野蛮一击抵抗率"),
				"\n",
				Global.GetLang("{17e43e}增加{-}无情一击抵抗率")
			});
			break;
		}
	}

	public UILabel TitleName;

	public UISprite ZheZhao;

	public UILabel TianFuLeiXing;

	public UILabel ShangHaiShuoMing;

	public UILabel JiaDianShuoMing;

	public DPSelectedItemEventHandler DPSelectedItem;
}
