using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PKLoversPartRule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ShuoMing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("活动说明")
		});
		this.GuiZe.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("战斗规则")
		});
		this.ShuoMingLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、活动时间：每周五·六"),
			"17e43e",
			Global.GetLang("13:30-15:30 \r\n"),
			"dac7ae",
			Global.GetLang("2、只有结婚的玩家才能参与竞技，战斗为"),
			"17e43e",
			Global.GetLang("2v2")
		});
		this.GuiZeLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、战斗开始会在场景中刷新"),
			"FF0000",
			Global.GetLang("“真爱天使羊”"),
			"dac7ae",
			Global.GetLang("，击杀 \r\n 后获得"),
			"FF0000",
			Global.GetLang("“真爱祝福” \r\n"),
			"dac7ae",
			Global.GetLang("2、杀死持有"),
			"FF0000",
			Global.GetLang("“真爱祝福”"),
			"dac7ae",
			Global.GetLang("的玩家，可以抢夺对方的"),
			"FF0000",
			Global.GetLang("“真\r\n爱祝福”\r\n"),
			"dac7ae",
			"của đối phương\r\n",
			"dac7ae",
			Global.GetLang("3、战斗开始1分钟后随机刷新"),
			"3681f3",
			Global.GetLang("“勇气天使羊”"),
			"dac7ae",
			Global.GetLang("，击杀后获得"),
			"3681f3",
			Global.GetLang("“勇气祝福”"),
			"dac7ae",
			Global.GetLang("，持有"),
			"3681f3",
			Global.GetLang("“勇气祝福”"),
			"dac7ae",
			Global.GetLang("对持有"),
			"FF0000",
			Global.GetLang("“真爱\r\n祝福”"),
			"dac7ae",
			Global.GetLang("的玩家伤害提升 \r\n"),
			"dac7ae",
			Global.GetLang("4、持有"),
			"FF0000",
			Global.GetLang("“真爱祝福”"),
			"dac7ae",
			Global.GetLang("1分钟或在战斗时间结束时依然\r\n持有")
		});
		this.ShuoMing.pivot = 3;
		this.GuiZe.pivot = 3;
		this.ShuoMingLab.lineWidth = 430;
		this.ShuoMingLab.pivot = 0;
		this.GuiZeLab.pivot = 0;
		this.ShuoMing.transform.localPosition = new Vector3(-216f, 183f, -1f);
		this.ShuoMingLab.transform.localPosition = new Vector3(-216f, 170f, -1f);
		this.GuiZe.transform.localPosition = new Vector3(-216f, 40f, -1f);
		this.GuiZeLab.transform.localPosition = new Vector3(-216f, 25f, -1f);
		this.GuiZeLab.transform.localScale = new Vector3(16f, 16f, 1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandle(this, new DPSelectedItemEventArgs());
		};
	}

	public DPSelectedItemEventHandler CloseHandle;

	public GButton Close;

	public UILabel ShuoMing;

	public UILabel ShuoMingLab;

	public UILabel GuiZe;

	public UILabel GuiZeLab;
}
