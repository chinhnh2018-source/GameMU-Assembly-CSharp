using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShengWuShuoMing : UserControl
{
	private void InitTextInPrefabs()
	{
		this.CurProtitle.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("升阶说明 ：")
		}));
		this.CurPro_11.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、圣物每个部件对应一种")
		})) + string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("部件碎片。")
		}));
		this.CurPro_22.text = string.Format("{0}{1}{2}{3}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("2、升阶失败会扣除所需")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"c20A11",
				Global.GetLang("绑金")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("与")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"c20A11",
				Global.GetLang("50%的部分碎片。")
			})
		});
		this.Lowertitle.text = string.Format("{0}", Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("额外加成 ：")
		}));
		this.LowerProcondition.text = string.Format("{0}{1}{2}{3}{4}{5}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("1、当圣物的")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("6个部位")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("全部升至")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("指定阶数")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("，即可激活")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("额外属性。")
			})
		});
		this.LowerProcondition.transform.localPosition = new Vector3(-225f, -65f, 0f);
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
