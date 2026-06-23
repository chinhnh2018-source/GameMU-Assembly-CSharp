using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class StallOutLineSetPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitControl();
	}

	private void InitControl()
	{
		GameInstance.Game.SpriteGetLiXianBaiTanSecsCmd();
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("LiXianNeedJinBi", true);
		this.SetLblNum();
		UIEventListener.Get(this.m_InPutGouMaiShiJian.gameObject).onInput = new UIEventListener.StringDelegate(this.InputChange);
		if (null != this.m_Slider)
		{
			this.m_Slider.onValueChange = new UISlider.OnValueChange(this.OnValueChange);
		}
		if (null != this.m_BtnGouMai)
		{
			this.m_BtnGouMai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.CheckInput();
				GameInstance.Game.SpriteBuyLiXianBaiTanSecsCmd(Convert.ToInt32(this.m_InPutGouMaiShiJian.label.text) * 3600);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			};
		}
		if (null != this.m_BtnKaiShiBaiTan)
		{
			this.m_BtnKaiShiBaiTan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (0 >= this.m_nOutLineTotalTime)
				{
					this.ShowMsg(Global.GetLang("您的离线摆摊时间不足，请购买后进行离线摆摊！"));
				}
				else
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2
					});
				}
			};
		}
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		if (null != this.m_BtnAdd)
		{
			this.m_BtnAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.m_nGouMaiShiJian++;
				if (12 <= this.m_nGouMaiShiJian)
				{
					this.m_nGouMaiShiJian = 12;
				}
				this.SetGouMaiShiJian();
			};
		}
		if (null != this.m_BtnSub)
		{
			this.m_BtnSub.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.m_nGouMaiShiJian--;
				if (0 >= this.m_nGouMaiShiJian)
				{
					this.m_nGouMaiShiJian = 0;
				}
				this.SetGouMaiShiJian();
			};
		}
	}

	public void ShowShengYuShiChang(int nSec)
	{
		if (nSec != 0)
		{
			this.m_nOutLineTotalTime = nSec;
			string text = UIHelper.FormatSecsHM((long)nSec, "-");
			string[] array = text.Split(new char[]
			{
				':'
			});
			this.m_LblXiaoShi.text = array[0];
			this.m_LblFenZhong.text = array[1];
		}
	}

	private void SetLblNum()
	{
		if (null != this.m_LblMoney)
		{
			this.m_LblMoney.text = string.Format(Global.GetLang("购买{0}离线时间消耗{1}"), Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				string.Format(Global.GetLang("{0}小时"), this.m_nGouMaiShiJian)
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"00ff00",
				string.Format(Global.GetLang("{0}金币"), Convert.ToInt32(ConfigSystemParam.GetSystemParamByName("LiXianNeedJinBi", true)) * this.m_nGouMaiShiJian)
			}));
		}
	}

	private void InputChange(GameObject gameobject, string text)
	{
		UILabel componentInChildren = gameobject.GetComponentInChildren<UILabel>();
		if (null != componentInChildren)
		{
			string text2 = componentInChildren.text;
			string[] array = text2.Split(new char[]
			{
				'|'
			});
			text2 = array[0];
			if (string.Empty == text2 || "|" == text2)
			{
				return;
			}
			int num = Convert.ToInt32(text2);
			if (0 >= num)
			{
				num = 0;
			}
			if (12 <= num)
			{
				num = 12;
			}
			this.m_nGouMaiShiJian = num;
			this.SetLblNum();
		}
	}

	private bool CheckInput()
	{
		string text = this.m_InPutGouMaiShiJian.label.text;
		string[] array = text.Split(new char[]
		{
			'|'
		});
		int num = 0;
		if (!int.TryParse(array[0], ref num))
		{
			this.ShowMsg(Global.GetLang("请输入正确的购买时间"));
			return false;
		}
		if (0 >= num)
		{
			this.ShowMsg(Global.GetLang("请输入正确的购买时间"));
			return false;
		}
		this.m_nGouMaiShiJian = num;
		return true;
	}

	private void SetGouMaiShiJian()
	{
		if (null != this.m_InPutGouMaiShiJian)
		{
			this.m_InPutGouMaiShiJian.label.text = Convert.ToString(this.m_nGouMaiShiJian);
			this.SetLblNum();
		}
	}

	private void OnValueChange(float val)
	{
		int num = 0;
		if (null != this.m_LblTime)
		{
			float num2 = val * 12f;
			num = (int)num2;
			if (0 >= num)
			{
				num = 1;
			}
			this.m_LblTime.text = string.Format(Global.GetLang("{0}小时"), Convert.ToString(num));
		}
		if (null != this.m_LblMoney)
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("OfflineVendorCost", true);
			if (1 <= num)
			{
				int num3 = num * Convert.ToInt32(systemParamByName);
				this.m_LblMoney.text = Convert.ToString(num3);
			}
		}
	}

	private void ShowMsg(string strMsg)
	{
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang(strMsg), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID != 0)
		{
			if (args.ID == 1)
			{
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnClose;

	public GButton m_BtnGouMai;

	public GButton m_BtnKaiShiBaiTan;

	public GButton m_BtnAdd;

	public GButton m_BtnSub;

	public UILabel m_LblTime;

	public UILabel m_LblMoney;

	public UILabel m_LblXiaoShi;

	public UILabel m_LblFenZhong;

	public UIInput m_InPutGouMaiShiJian;

	public UISlider m_Slider;

	private int m_nGouMaiShiJian;

	public int m_nOutLineTotalTime;
}
