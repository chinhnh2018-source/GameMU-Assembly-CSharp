using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class MeiRiBiZuoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_JinRiKeZuoBtn.Text = Global.GetLang("今日可做");
		this.m_ZiYuanHuiShouBtn.Text = Global.GetLang("资源找回");
		this.dailyPrivilegeBtn.Text = Global.GetLang("每日专享");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.dailyPrivilegeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(0);
		};
		this.m_JinRiKeZuoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.m_ZiYuanHuiShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		ActivityTipManager.RegActivityTipItem(7001, delegate(int s, ActivityTipItem e)
		{
			this._ZiYuanTipIcon.gameObject.SetActive(e.IsActive);
		});
		this.RefreshMoney();
		this.InitTabbars();
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(7001, null);
	}

	private void InitPartData(int type = 1)
	{
		this.m_JinRiKeZuoBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_ZiYuanHuiShouBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.SetPart(type);
	}

	private void InitTabbars()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.DailyPrivilege, ref num, ref num2, ref num3);
		if (null != this.list_btn)
		{
			ObservableCollection itemsSource = this.list_btn.ItemsSource;
			this.dailyPrivilegeBtn.gameObject.SetActive(flag);
			if (flag)
			{
				itemsSource.AddNoUpdate(this.dailyPrivilegeBtn);
			}
			itemsSource.AddNoUpdate(this.m_JinRiKeZuoBtn);
			itemsSource.AddNoUpdate(this.m_ZiYuanHuiShouBtn);
		}
		this.InitPartData((!flag) ? 1 : 0);
	}

	public void RefreshMoney()
	{
		this.m_TextGold.text = Global.GetRoleOwnNumByMoneyType(8).ToString();
		this.m_TextDiamond.text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
	}

	public void SetPart(int type)
	{
		switch (type)
		{
		case 0:
			this.SetBtnStat(this.dailyPrivilegeBtn, true);
			this.SetBtnStat(this.m_JinRiKeZuoBtn, false);
			this.SetBtnStat(this.m_ZiYuanHuiShouBtn, false);
			if (null == this.dailyPrivilege)
			{
				this.dailyPrivilege = U3DUtils.NEW<DailyPrivilege>();
				this.dailyPrivilege.Init();
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.dailyPrivilege.gameObject, true);
			}
			break;
		case 1:
			this.SetBtnStat(this.m_JinRiKeZuoBtn, true);
			this.SetBtnStat(this.dailyPrivilegeBtn, false);
			this.SetBtnStat(this.m_ZiYuanHuiShouBtn, false);
			if (this.m_JinRiKeZuoPart == null)
			{
				this.m_JinRiKeZuoPart = U3DUtils.NEW<JinRiKeZuoPart>();
				this.m_JinRiKeZuoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_JinRiKeZuoPart.gameObject, true);
			}
			break;
		case 2:
			this.SetBtnStat(this.m_ZiYuanHuiShouBtn, true);
			this.SetBtnStat(this.dailyPrivilegeBtn, false);
			this.SetBtnStat(this.m_JinRiKeZuoBtn, false);
			if (this.m_ZiYuanZhaoHuiPart == null)
			{
				this.m_ZiYuanZhaoHuiPart = U3DUtils.NEW<ZiYuanZhaoHuiPart>();
				this.m_ZiYuanZhaoHuiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ZiYuanZhaoHuiPart.gameObject, true);
			}
			break;
		}
		if (null != this.dailyPrivilege)
		{
			this.dailyPrivilege.gameObject.SetActive(0 == type);
		}
		if (this.m_JinRiKeZuoPart != null)
		{
			this.m_JinRiKeZuoPart.gameObject.SetActive(type == 1);
		}
		if (this.m_ZiYuanZhaoHuiPart != null)
		{
			this.m_ZiYuanZhaoHuiPart.gameObject.SetActive(type == 2);
			if (type == 2)
			{
				this.m_ZiYuanZhaoHuiPart.RequestServerData();
			}
		}
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	public ListBox list_btn;

	public GButton m_JinRiKeZuoBtn;

	public GButton m_ZiYuanHuiShouBtn;

	public GButton m_CloseBtn;

	public GameObject m_Pnl;

	public Transform _ZiYuanTipIcon;

	public UILabel m_TextGold;

	public UILabel m_TextDiamond;

	public DPSelectedItemEventHandler DPSelectedItem;

	[HideInInspector]
	public JinRiKeZuoPart m_JinRiKeZuoPart;

	[HideInInspector]
	public ZiYuanZhaoHuiPart m_ZiYuanZhaoHuiPart;

	public GButton dailyPrivilegeBtn;

	[HideInInspector]
	public DailyPrivilege dailyPrivilege;
}
