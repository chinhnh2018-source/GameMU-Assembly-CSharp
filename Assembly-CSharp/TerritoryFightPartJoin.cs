using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TerritoryFightPartJoin : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("参加战斗")
		});
		this.AKaLunXiLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("阿卡伦-西")
		});
		this.AKaLunDongLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("阿卡伦-东")
		});
		this.MiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("点击选择要加入的战场")
		});
		this.AKaLunXiImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/akalunximap1.jpg.qj";
		this.AKaLunDongImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/akalundongmap1.jpg.qj";
	}

	public void SetCurrentNum(int leftnum, int rightnum)
	{
		int maxEnterNum = TerritoryFightPart.GetDicLegionsWar()[1].MaxEnterNum;
		string text = "17e43e";
		string text2 = "17e43e";
		if (leftnum >= maxEnterNum)
		{
			text = "FF0000";
		}
		if (rightnum >= maxEnterNum)
		{
			text2 = "FF0000";
		}
		this.LeftRoleNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			string.Format(Global.GetLang("当前人数：{0}/{1}"), leftnum, maxEnterNum)
		});
		this.RightRoleNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			text2,
			string.Format(Global.GetLang("当前人数：{0}/{1}"), rightnum, maxEnterNum)
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.AKaLunXiImage.gameObject).onClick = delegate(GameObject s)
		{
			GameInstance.Game.EnterTerritoryFightGame(83003);
			Super.ShowNetWaiting(null);
		};
		UIEventListener.Get(this.AKaLunDongImage.gameObject).onClick = delegate(GameObject s)
		{
			GameInstance.Game.EnterTerritoryFightGame(83002);
			Super.ShowNetWaiting(null);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.JionFightingState();
		Super.ShowNetWaiting(null);
	}

	public DPSelectedItemEventHandler DPSelectedClose;

	public GButton BtnClose;

	public ShowNetImage AKaLunXiImage;

	public ShowNetImage AKaLunDongImage;

	public UILabel Title;

	public UILabel AKaLunXiLab;

	public UILabel AKaLunDongLab;

	public UILabel MiaoShu;

	public UILabel LeftRoleNum;

	public UILabel RightRoleNum;
}
