using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenDianRule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.CurProtitle.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("规则玩法")
		}));
		this.CurPro_11.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、守护最高等级受")
		})) + string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("战盟等级限制")
		}));
		this.CurPro_22.text = string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("2、如果战盟等级")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"c20A11",
				Global.GetLang("低于")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("激活守护的要求等级，则")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"c20A11",
				Global.GetLang("无法激活")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("全部属性")
			})
		});
		this.Lowertitle.text = string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("3、升级消耗")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("战功")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("，随每日升级")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("次数")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("增长，每日")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("0点")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("重置")
			})
		});
		this.LowerProcondition.text = string.Format("{0}{1}{2}{3}\r\n{4}{5}{6}{7}{8}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("4、所有神殿守护均升至")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("相同等级")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("时，可激活")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("额外加成")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("5、")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("退出")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("战盟后")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("神殿等级")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("保留")
			})
		});
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close_MouseLeftButtonUp);
	}

	public void Close_MouseLeftButtonUp(object sender, MouseEvent e)
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

	public UILabel CurProtitle;

	public UILabel CurPro_11;

	public UILabel CurPro_22;

	public UILabel Lowertitle;

	public UILabel LowerProcondition;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton Close;
}
