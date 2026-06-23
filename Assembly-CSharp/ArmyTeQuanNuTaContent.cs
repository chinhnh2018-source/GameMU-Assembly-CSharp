using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ArmyTeQuanNuTaContent : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
		this.Starts();
	}

	private void OnEnable()
	{
		for (int i = 0; i < this.m_ProgressBars.Length; i++)
		{
			this.ShowProgressBar(i, false);
		}
	}

	private void InitTextInPrefabs()
	{
		this.m_FanRongDu.Text = Global.GetLang("繁荣度：");
		this.m_DiTu.Text = Global.GetLang("弩塔地图");
		this.m_FanRongDu.Pivot = 5;
		this.m_FanRongDu.X = 80.0;
		this.m_FanRongDuValue.Pivot = 3;
		this.m_FanRongDuValue.X = 85.0;
	}

	private void InitBtnCallBack()
	{
	}

	private void Starts()
	{
		for (int i = 0; i < this.m_Icons.Length; i++)
		{
			this.m_Icons[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < this.m_BuShuScripts.Length; j++)
		{
			this.m_BuShuScripts[j].RefreshIconCallBack = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					GameInstance.Game.SendSetShouWeiRequest(e.ID - 1, 0);
				}
				else if (e.IDType == 2)
				{
					this.OpenBuyWindow(e.Level, e.ID);
				}
			};
		}
		for (int k = 0; k < this.m_BuShuScripts.Length; k++)
		{
			this.m_BuShuScripts[k].RefreshIconMaskCallBack = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.maskIndex = e.ID - 1;
				if (this.m_ProgressBars[this.maskIndex].gameObject.activeSelf)
				{
					this.m_ProgressBars[this.maskIndex].gameObject.SetActive(false);
				}
			};
		}
	}

	public void RefreshFanRongDu(int value)
	{
		this.m_LeftFanRongDu = value;
		this.m_FanRongDuValue.Text = value.ToString();
	}

	private void OpenBuyWindow(int diamond, int id)
	{
		if (null == this.confirmWindow)
		{
			this.confirmWindow = U3DUtils.NEW<GChildWindow>();
			this.confirmWindow.ModalType = ChildWindowModalType.Translucent;
			this.confirmWindow.IsShowModal = true;
			Super.InitChildWindow(this.confirmWindow, "ArmyTeQuanConfirmWindow");
			Super.GData.GlobalPlayZone.Children.Add(this.confirmWindow);
		}
		this.confirmPart = U3DUtils.NEW<ArmyTeQuanConfirmWindow>();
		this.confirmWindow.Body.Add(this.confirmPart);
		this.confirmPart.InitValue(diamond);
		this.confirmPart.OptionCallBack = delegate(object s1, DPSelectedItemEventArgs args)
		{
			if (args.ID == -1)
			{
				this.CloseBuyWindow();
			}
			else if (args.ID == 1)
			{
				GameInstance.Game.SendSetShouWeiRequest(id - 1, 1);
				this.CloseBuyWindow();
			}
			else if (args.ID == 0)
			{
				this.CloseBuyWindow();
			}
		};
	}

	private void CloseBuyWindow()
	{
		if (null != this.confirmPart)
		{
			Object.Destroy(this.confirmPart.gameObject);
			this.confirmPart = null;
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.confirmWindow);
		}
	}

	public void InitVaue(int index, int status, DateTime relifeTime, int costDiamond)
	{
		status++;
		this.m_BuShuScripts[index].InitTypeValue(status, relifeTime, costDiamond);
		if (status == 2)
		{
			this.ShowFuHuoShouWei(index + 1);
			this.ShowProgressBar(index, true);
		}
		if (status == 3)
		{
			this.ShowShouWei(index + 1);
		}
	}

	private void RefreshBuShuButton(int index)
	{
		this.m_BuShuScripts[index - 1].SetButtonDisable();
	}

	public void RefreshUIStatus(int result, int index)
	{
		bool flag = false;
		switch (result + 16)
		{
		case 0:
			Super.HintMainText(Global.GetLang("不能使用钻石部署守卫，只能使用繁荣度"), 10, 3);
			break;
		default:
			switch (result + 2)
			{
			case 0:
				Super.HintMainText(Global.GetLang("不是领主"), 10, 3);
				goto IL_137;
			case 1:
				Super.HintMainText(Global.GetLang("找不到跨服服务器"), 10, 3);
				goto IL_137;
			case 3:
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetRoleArmyGroupData(Global.Data.RoleID);
				flag = true;
				goto IL_137;
			}
			Super.HintMainText(string.Format("{0}{1}", Global.GetLang("未知错误码："), result), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("繁荣度不足"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("不在领地采集地图"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("不存在领地数据"), 10, 3);
			break;
		}
		IL_137:
		if (!flag)
		{
			return;
		}
		switch (index)
		{
		case 1:
			this.ShowProgressBar(0, false);
			break;
		case 2:
			this.ShowProgressBar(1, false);
			break;
		case 3:
			this.ShowProgressBar(2, false);
			break;
		case 4:
			this.ShowProgressBar(3, false);
			break;
		}
		this.ShowShouWei(index);
		this.CloseCountDown(index);
	}

	private void CloseCountDown(int index)
	{
		this.m_BuShuScripts[index - 1].CloseCountDown();
	}

	private void ShowShouWei(int index)
	{
		int num = index;
		int num2 = num - 1;
		if (!this.m_Icons[num2].gameObject.activeSelf)
		{
			this.m_Icons[num2].gameObject.SetActive(true);
		}
		this.m_Icons[num2].spriteName = num.ToString();
		this.RefreshBuShuButton(num);
	}

	private void ShowProgressBar(int index, bool isShow)
	{
		if (this.m_ProgressBars.Length <= 0)
		{
			return;
		}
		this.m_ProgressBars[index].gameObject.SetActive(isShow);
	}

	private void ShowFuHuoShouWei(int index)
	{
		int num = index;
		int num2 = num - 1;
		if (!this.m_Icons[num2].gameObject.activeSelf)
		{
			this.m_Icons[num2].gameObject.SetActive(true);
		}
		this.m_Icons[num2].spriteName = num.ToString();
	}

	protected override void OnDestroy()
	{
		this.m_FanRongDu = null;
		this.m_FanRongDuValue = null;
		this.m_DiTu = null;
		this.m_BuShuScripts = null;
	}

	public TextBlock m_FanRongDu;

	public TextBlock m_FanRongDuValue;

	public TextBlock m_DiTu;

	public UISprite[] m_Icons;

	public UISprite[] m_ProgressBars;

	public ArmyTeQuanpartBuShu[] m_BuShuScripts;

	private float[] maskCount = new float[4];

	private int maskIndex;

	private int m_LeftFanRongDu;

	public ArmyTeQuanConfirmWindow confirmPart;

	private GChildWindow confirmWindow;
}
