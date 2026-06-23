using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class RoleShiZhuangHelp : UserControl
{
	private void InitTextInPrefabs()
	{
		this.help.text = string.Concat(new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("【规则介绍】")
			}),
			"\r\n",
			Global.GetLang("1、称号无论是否为佩戴属性均生效"),
			"\r\n",
			Global.GetLang("2、多个称号属性可叠加，收集称号越多获得越强属性")
		});
		this.help.transform.localScale = new Vector3(20f, 20f, 1f);
		this.help.lineWidth = 470;
		this.help.transform.localPosition = new Vector3(-237f, 60f, -5f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
	}

	public UILabel help;

	public GButton Close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
