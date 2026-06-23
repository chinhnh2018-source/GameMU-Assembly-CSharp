using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TerritoryFightPartRule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("查看规则")
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
			Global.GetLang("点击战场看规则")
		});
		this.AKaLunXiImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/akalunximap1.jpg.qj";
		this.AKaLunDongImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/akalundongmap1.jpg.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.AKaLunXiImage.gameObject).onClick = delegate(GameObject s)
		{
			this.OpenTFRuleWindowWindow(1);
		};
		UIEventListener.Get(this.AKaLunDongImage.gameObject).onClick = delegate(GameObject s)
		{
			this.OpenTFRuleWindowWindow(0);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
	}

	private void OpenTFRuleWindowWindow(int index)
	{
		if (this.TFRuleWindow == null)
		{
			this.TFRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.TFRuleWindow.IsShowModal = true;
			this.TFRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.TFRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.TFRuleWindow);
		}
		if (this.TFRulePart == null)
		{
			this.TFRulePart = U3DUtils.NEW<TerritoryFightPartRuleItem>();
			this.TFRulePart.DPSelectedClose = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseTFRuleWindow();
			};
			this.TFRulePart.SetSelectedIndex(index);
		}
		this.TFRuleWindow.SetContent(this.TFRuleWindow.BodyPresenter, this.TFRulePart, 0.0, 0.0, true);
	}

	private void CloseTFRuleWindow()
	{
		if (null != this.TFRulePart)
		{
			this.TFRulePart.transform.parent = null;
			Object.Destroy(this.TFRulePart.gameObject);
			this.TFRulePart = null;
		}
		if (null != this.TFRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.TFRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.TFRuleWindow, true);
			this.TFRuleWindow = null;
		}
	}

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedClose;

	public ShowNetImage AKaLunXiImage;

	public ShowNetImage AKaLunDongImage;

	public UILabel Title;

	public UILabel AKaLunXiLab;

	public UILabel AKaLunDongLab;

	public UILabel MiaoShu;

	protected GChildWindow TFRuleWindow;

	protected TerritoryFightPartRuleItem TFRulePart;
}
