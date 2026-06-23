using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuiGuiHuoDongPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("提示");
		this.lblContent.text = Global.GetLang("本合服内最老角色创建时间，为该账号的创建时间");
		this.lblBtnOK.text = Global.GetLang("确定");
		this.lblCreateTime.pivot = 5;
		this.lblCreateTime.transform.localPosition = new Vector3(170f, this.lblCreateTime.transform.localPosition.y, this.lblCreateTime.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.goHelp.SetActive(false);
		UIEventListener.Get(this.btnQianDao).onClick = new UIEventListener.VoidDelegate(this.OnClickQianDao);
		UIEventListener.Get(this.btnLeiJi).onClick = new UIEventListener.VoidDelegate(this.OnClickLeiJi);
		UIEventListener.Get(this.btnZhiGou).onClick = new UIEventListener.VoidDelegate(this.OnClickZhiGou);
		UIEventListener.Get(this.btnShop).onClick = new UIEventListener.VoidDelegate(this.OnClickShop);
		UIEventListener.Get(this.btnClose).onClick = new UIEventListener.VoidDelegate(this.OnClickClose);
		UIEventListener.Get(this.btnHelp).onClick = new UIEventListener.VoidDelegate(this.OnClickHelp);
		UIEventListener.Get(this.btnOK).onClick = new UIEventListener.VoidDelegate(this.OnCloseHelp);
		UIEventListener.Get(this.btnOKBg).onClick = new UIEventListener.VoidDelegate(this.OnCloseHelp);
		this.lblCreateTime.text = string.Empty;
		this.GetInfo();
	}

	private void OnCloseHelp(GameObject go)
	{
		this.goHelp.SetActive(false);
	}

	private void OnClickHelp(GameObject go)
	{
		this.goHelp.SetActive(true);
	}

	private void OnClickClose(GameObject go)
	{
		if (this.ChildWindowClose != null)
		{
			this.ChildWindowClose(this, null);
		}
	}

	private void OnClickShop(GameObject go)
	{
		this.OpenStoreWindow();
	}

	private void OnClickZhiGou(GameObject go)
	{
		this.OpenZhiGouWindow();
	}

	private void OnClickLeiJi(GameObject go)
	{
		this.OpenLeiJiWindow();
	}

	private void OnClickQianDao(GameObject go)
	{
		this.OpenQianDaoWindow();
	}

	private void OpenLeiJiWindow()
	{
		if (this.m_LeiJiWindow == null)
		{
			this.m_LeiJiWindow = U3DUtils.NEW<GChildWindow>();
			this.m_LeiJiWindow.IsShowModal = true;
			this.m_LeiJiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_LeiJiWindow, Global.GetLang("累积充值"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_LeiJiWindow);
		}
		if (this.m_LeiJiPart == null)
		{
			this.m_LeiJiPart = U3DUtils.NEW<HuiGuiLeiJiPart>();
			this.m_LeiJiPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseLeiJiWindow();
			};
		}
		this.m_LeiJiWindow.SetContent(this.m_LeiJiWindow.BodyPresenter, this.m_LeiJiPart, 0.0, 0.0, true);
	}

	private void CloseLeiJiWindow()
	{
		if (null != this.m_LeiJiPart)
		{
			this.m_LeiJiPart.transform.parent = null;
			Object.Destroy(this.m_LeiJiPart.gameObject);
			this.m_LeiJiPart = null;
		}
		if (null != this.m_LeiJiWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_LeiJiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_LeiJiWindow, true);
			this.m_LeiJiWindow = null;
		}
	}

	private void OpenQianDaoWindow()
	{
		if (this.m_QianDaoWindow == null)
		{
			this.m_QianDaoWindow = U3DUtils.NEW<GChildWindow>();
			this.m_QianDaoWindow.IsShowModal = true;
			this.m_QianDaoWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_QianDaoWindow, Global.GetLang("签到奖励"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_QianDaoWindow);
		}
		if (this.m_QianDaoPart == null)
		{
			this.m_QianDaoPart = U3DUtils.NEW<HuiGuiQianDaoPart>();
			this.m_QianDaoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseQianDaoWindow();
			};
		}
		this.m_QianDaoWindow.SetContent(this.m_QianDaoWindow.BodyPresenter, this.m_QianDaoPart, 0.0, 0.0, true);
	}

	private void CloseQianDaoWindow()
	{
		if (null != this.m_QianDaoPart)
		{
			this.m_QianDaoPart.transform.parent = null;
			Object.Destroy(this.m_QianDaoPart.gameObject);
			this.m_QianDaoPart = null;
		}
		if (null != this.m_QianDaoWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_QianDaoWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_QianDaoWindow, true);
			this.m_QianDaoWindow = null;
		}
	}

	private void OpenZhiGouWindow()
	{
		if (this.m_ZhiGouWindow == null)
		{
			this.m_ZhiGouWindow = U3DUtils.NEW<GChildWindow>();
			this.m_ZhiGouWindow.IsShowModal = true;
			this.m_ZhiGouWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_ZhiGouWindow, Global.GetLang("签到奖励"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_ZhiGouWindow);
		}
		if (this.m_ZhiGouPart == null)
		{
			this.m_ZhiGouPart = U3DUtils.NEW<HuiGuiZhiGouPart>();
			this.m_ZhiGouPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseZhiGouWindow();
			};
		}
		this.m_ZhiGouWindow.SetContent(this.m_ZhiGouWindow.BodyPresenter, this.m_ZhiGouPart, 0.0, 0.0, true);
	}

	private void CloseZhiGouWindow()
	{
		if (null != this.m_ZhiGouPart)
		{
			this.m_ZhiGouPart.transform.parent = null;
			Object.Destroy(this.m_ZhiGouPart.gameObject);
			this.m_ZhiGouPart = null;
		}
		if (null != this.m_ZhiGouWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_ZhiGouWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_ZhiGouWindow, true);
			this.m_ZhiGouWindow = null;
		}
	}

	private void OpenStoreWindow()
	{
		if (this.m_StoreWindow == null)
		{
			this.m_StoreWindow = U3DUtils.NEW<GChildWindow>();
			this.m_StoreWindow.IsShowModal = true;
			this.m_StoreWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_StoreWindow, Global.GetLang("签到奖励"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_StoreWindow);
		}
		if (this.m_StorePart == null)
		{
			this.m_StorePart = U3DUtils.NEW<HuiGuiStorePart>();
			this.m_StorePart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseStoreWindow();
			};
		}
		this.m_StoreWindow.SetContent(this.m_StoreWindow.BodyPresenter, this.m_StorePart, 0.0, 0.0, true);
	}

	private void CloseStoreWindow()
	{
		if (null != this.m_StorePart)
		{
			this.m_StorePart.transform.parent = null;
			Object.Destroy(this.m_StorePart.gameObject);
			this.m_StorePart = null;
		}
		if (null != this.m_StoreWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_StoreWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_StoreWindow, true);
			this.m_StoreWindow = null;
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_GETFILE", new Action(this.ServeGetHuiGuiInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener("CMD_SPR_REGRESSACTIVE_GETFILE", new Action(this.ServeGetHuiGuiInfo));
	}

	private void ServeGetHuiGuiInfo()
	{
		this.lblCreateTime.text = Global.GetLang("我的创建时间:") + HuiGuiData.GetSelfCreateTime();
	}

	private void GetInfo()
	{
		GameInstance.Game.SendHuiGuiInfo();
	}

	public GameObject btnQianDao;

	public GameObject btnLeiJi;

	public GameObject btnZhiGou;

	public GameObject btnShop;

	public GameObject btnClose;

	public GameObject btnHelp;

	public GameObject btnOK;

	public GameObject btnOKBg;

	public UILabel lblCreateTime;

	public UILabel lblTitle;

	public UILabel lblContent;

	public UILabel lblBtnOK;

	public GameObject goHelp;

	protected GChildWindow m_LeiJiWindow;

	protected HuiGuiLeiJiPart m_LeiJiPart;

	protected GChildWindow m_QianDaoWindow;

	protected HuiGuiQianDaoPart m_QianDaoPart;

	protected GChildWindow m_ZhiGouWindow;

	protected HuiGuiZhiGouPart m_ZhiGouPart;

	protected GChildWindow m_StoreWindow;

	protected HuiGuiStorePart m_StorePart;
}
