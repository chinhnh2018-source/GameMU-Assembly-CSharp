using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class BoCaiBuyHuoBiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		this.InitOnClick();
		this.NumberRefresh();
	}

	private void InitText()
	{
		this.btnBuy.Text = Global.GetLang("确定");
		this.btnOff.Text = Global.GetLang("取消");
		this.labTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("购买欢乐代币")
		});
		this.labPriceTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("购买代币:")
		});
		this.labConsumeTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("消耗钻石:")
		});
		this.labNumberTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("购买数量:")
		});
	}

	private void InitOnClick()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.btnHeiDi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.btnOff.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.btnBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.UserMoney < this.numbers)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			if (Global.GetZuanShi(ZuanShiPartClass.BuyHuanLeDaiBi))
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.numbers))
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
				}
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						Global.SetZuanShi(ZuanShiPartClass.BuyHuanLeDaiBi, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.handlerBuy(this, new DPSelectedItemEventArgs
						{
							Index = this.numbers
						});
					}
					return true;
				};
				return;
			}
			this.handlerBuy(this, new DPSelectedItemEventArgs
			{
				Index = this.numbers
			});
		};
		this.btnAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.numbers++;
			this.NumberRefresh();
		};
		this.btnRecude.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.numbers--;
			this.NumberRefresh();
		};
		this.btnNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object e2, DPSelectedItemEventArgs s2)
			{
				this.numbers = s2.ID;
				this.NumberRefresh();
			}, null, 0, -100);
		};
	}

	public void NumberRefresh()
	{
		int num = Mathf.Min(Global.Data.roleData.UserMoney, 99999);
		if (this.numbers >= num)
		{
			this.numbers = num;
			this.btnAdd.isEnabled = false;
		}
		else
		{
			this.btnAdd.isEnabled = true;
		}
		if (this.numbers <= 1)
		{
			this.numbers = 1;
			this.btnRecude.isEnabled = false;
		}
		else
		{
			this.btnRecude.isEnabled = true;
		}
		if (this.numbers <= 0)
		{
			this.btnBuy.isEnabled = false;
		}
		else
		{
			this.btnBuy.isEnabled = true;
		}
		this.labPrice.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.numbers
		});
		this.labConsume.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.numbers
		});
		this.btnNumber.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.numbers
		});
	}

	public UILabel labTitle;

	public UILabel labPriceTitle;

	public UILabel labConsumeTitle;

	public UILabel labNumberTitle;

	public UILabel labPrice;

	public UILabel labConsume;

	public GButton btnClose;

	public GButton btnAdd;

	public GButton btnRecude;

	public GButton btnNumber;

	public GButton btnBuy;

	public GButton btnOff;

	public GButton btnHeiDi;

	public DPSelectedItemEventHandler handlerClose;

	public DPSelectedItemEventHandler handlerBuy;

	private int numbers = 1;

	private GChildWindow messageBoxWindow;
}
