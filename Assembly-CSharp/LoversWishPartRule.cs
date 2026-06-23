using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LoversWishPartRule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ShuoMing.transform.localPosition = new Vector3(0f, 170f, -1f);
		this.ShuoMingLab.pivot = 0;
		this.ShuoMingLab.lineWidth = 425;
		this.ShuoMingLab.transform.localPosition = new Vector3(-211f, 155f, -1f);
		this.ShuoMing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("规则说明")
		});
		this.ShuoMingLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、可以赠送"),
			"17e43e",
			Global.GetLang("本服情侣"),
			"dac7ae",
			Global.GetLang("（包括自己伴侣），也可跨服赠送给"),
			"17e43e",
			Global.GetLang("排行榜中的情侣\r\n"),
			"dac7ae",
			Global.GetLang("2、祝福值为"),
			"17e43e",
			Global.GetLang("双方共同积累"),
			"dac7ae",
			Global.GetLang("，赠送任意一方或双方\r\n效果相同\r\n"),
			"dac7ae",
			Global.GetLang("3、每周最后一天的"),
			"17e43e",
			Global.GetLang("23:30-00:10"),
			"dac7ae",
			Global.GetLang("为结算时间,"),
			"FF0000",
			Global.GetLang("不可\r\n赠送 \r\n"),
			"dac7ae",
			Global.GetLang("4、离婚会清空祝福值,再次结婚"),
			"FF0000",
			Global.GetLang("不会返还 \r\n"),
			"dac7ae",
			Global.GetLang("5、每周第一名获得雕像，离婚后双方都会失去雕\r\n像")
		});
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
}
