using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AngelTempleGuWuPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.GoldParams = ConfigSystemParam.GetSystemParamIntArrayByName("AngelTempleGoldBuff", ',');
		this.ZuanShiParams = ConfigSystemParam.GetSystemParamIntArrayByName("AngelTempleZuanshiBuff", ',');
		this._chkJinBi.Text = Global.GetLang("金币鼓舞");
		this.strDaiBi = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(this.strDaiBi, "ZuanShiGuWu", this.ZuanShiParams[0]);
		this._chkZuanShi.Text = string.Format(Global.GetLang("{0}鼓舞"), this.strDaiBi);
		this._Submit.Text = Global.GetLang("鼓舞");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				int num = (!this._chkZuanShi.Check) ? 0 : 1;
				if (this.ZuanShiParams != null && num == 1)
				{
					if (this.messageBoxWindow != null)
					{
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					}
					this.strDaiBi = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(this.strDaiBi, "ZuanShiGuWu", this.ZuanShiParams[0]);
					this.messageBoxWindow = (this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.ZuanShiParams[0], this.strDaiBi)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null));
					this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							this.OnSubmit();
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 1
							});
						}
						return true;
					};
					return;
				}
				this.OnSubmit();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
		this._chkJinBi.CheckChanged = new BaseEventHandler2(this.OnCheckChanged);
		this._chkZuanShi.CheckChanged = new BaseEventHandler2(this.OnCheckChanged);
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	protected void Close(object sender, MouseEvent e)
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

	public void InitPartData()
	{
		this._chkJinBi.Check = true;
		this.OnCheckChanged(this._chkJinBi, BaseEventArgs.Empty);
	}

	private void OnSubmit()
	{
		int type = (!this._chkZuanShi.Check) ? 0 : 1;
		if (this.GoldParams != null && type == 0 && Global.IsBufferExist(85))
		{
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("当前已有金币鼓舞效果，是否替换？"), delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					GameInstance.Game.SpriteAngelTempleGuLiInfoCmd(type);
				}
			}, new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			});
		}
		else if (this.ZuanShiParams != null && type == 1 && Global.IsBufferExist(86))
		{
			Super.ShowMessageBoxEx(Global.GetLang("提示"), string.Format(Global.GetLang("当前已有{0}鼓舞效果，是否替换？"), this.strDaiBi), delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					GameInstance.Game.SpriteAngelTempleGuLiInfoCmd(type);
				}
			}, new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			});
		}
		else
		{
			GameInstance.Game.SpriteAngelTempleGuLiInfoCmd(type);
		}
	}

	public void OnCheckChanged(object sender, BaseEventArgs e)
	{
		if (this.GoldParams != null && this.ZuanShiParams != null && this.GoldParams.Length == 3 && this.ZuanShiParams.Length == 3)
		{
			if (this._chkJinBi.Check)
			{
				string text = UIHelper.FormatSecsShort((long)this.GoldParams[2], "-");
				string text2 = Global.FormatBuffDesc(this.GoldParams[1]);
				this._Desc.Text = string.Format(Global.GetLang("花费{{00ff00}}{0}金币{{-}},获得{{00ff00}}{1}{{-}},效果持续{{00ff00}}{2}{{-}}"), this.GoldParams[0], text2, text);
			}
			else
			{
				string text3 = UIHelper.FormatSecsShort((long)this.ZuanShiParams[2], "-");
				string text4 = Global.FormatBuffDesc(this.ZuanShiParams[1]);
				this._Desc.Text = string.Format(Global.GetLang("花费{{00ff00}}{0}{1}{{-}},获得{{00ff00}}{2}{{-}},效果持续{{00ff00}}{3}{{-}}"), new object[]
				{
					this.ZuanShiParams[0],
					this.strDaiBi,
					text4,
					text3
				});
			}
		}
	}

	public GCheckBox _chkJinBi;

	public GCheckBox _chkZuanShi;

	public GButton _Close;

	public ShowNetImage _Bak;

	public GButton _Submit;

	public TextBlock _Title;

	public TextBlock _Desc;

	private int[] GoldParams;

	private int[] ZuanShiParams;

	private GChildWindow messageBoxWindow;

	public DPSelectedItemEventHandler DPSelectedItem;

	private string strDaiBi = Global.GetLang("钻石");
}
