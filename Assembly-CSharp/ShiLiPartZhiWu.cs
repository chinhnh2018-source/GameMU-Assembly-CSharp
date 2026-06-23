using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Contract.KuaFuData;
using UnityEngine;

public class ShiLiPartZhiWu : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnZhuanHuan.Text = Global.GetLang("转换势力");
		this.btnAllZhiWu.Label.text = Global.GetLang("全部职务");
		Global.SetSprite(base.gameObject);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnPaiHang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenPaiHangWindow(RankType.ThisWeak);
		};
		this.btnPaiHangLast.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenPaiHangWindow(RankType.LastWeak);
		};
		this.btnDiDui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenDiDuiWindow();
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenHelpWindow();
		};
		this.btnZhuanHuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenZhuanHuanWindow();
		};
		this.btnAllZhiWu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAllZhiWuWindow();
		};
		if (ShiLiData.IsCompBattleOpen())
		{
			this.btnAllZhiWu.gameObject.SetActive(true);
		}
		else
		{
			this.btnAllZhiWu.gameObject.SetActive(false);
		}
		this.lblJunXianNum.text = ShiLiData.GetSelfJunXian().ToString();
	}

	private CompZhiWuData GetTest()
	{
		return new CompZhiWuData();
	}

	public void GetZhiWuInfo(int CompType, bool beSelf)
	{
		this.m_compType = CompType;
		if (beSelf)
		{
			this.goSelfContent.SetActive(true);
			this.lblJunXianNum.text = ShiLiData.GetSelfJunXian().ToString();
		}
		else
		{
			this.goSelfContent.SetActive(false);
		}
		this.SendGetZhiWu(this.m_compType);
	}

	private void Init(CompZhiWuData zhiWuData)
	{
		this.m_zhiWuData = zhiWuData;
		if (zhiWuData == null || zhiWuData.CompRoleData == null)
		{
			return;
		}
		KFCompRoleData compRoleDataByZhiWu = ShiLiData.GetCompRoleDataByZhiWu(zhiWuData.CompRoleData, 1, this.m_compType);
		KFCompRoleData compRoleDataByZhiWu2 = ShiLiData.GetCompRoleDataByZhiWu(zhiWuData.CompRoleData, 2, this.m_compType);
		KFCompRoleData compRoleDataByZhiWu3 = ShiLiData.GetCompRoleDataByZhiWu(zhiWuData.CompRoleData, 3, this.m_compType);
		this.player1.SetCompAndLevel(this.m_compType, 1);
		this.player2.SetCompAndLevel(this.m_compType, 2);
		this.player3.SetCompAndLevel(this.m_compType, 3);
		this.player1.InitData(compRoleDataByZhiWu);
		this.player2.InitData(compRoleDataByZhiWu2);
		this.player3.InitData(compRoleDataByZhiWu3);
	}

	private void OnOpenZhuanHuanWindow()
	{
		this.OnShiLiSelectWindow();
	}

	private void OnOpenHelpWindow()
	{
		ShiLiData.OpenHelpWindow(ShiLiHelpType.HelpCompIntroLevel);
	}

	private void OnOpenDiDuiWindow()
	{
		if (this.m_diDuiWindow == null)
		{
			this.m_diDuiWindow = U3DUtils.NEW<GChildWindow>();
			this.m_diDuiWindow.IsShowModal = true;
			this.m_diDuiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_diDuiWindow, Global.GetLang("势力敌对"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_diDuiWindow);
		}
		if (this.m_diDuiPart == null)
		{
			this.m_diDuiPart = U3DUtils.NEW<ShiLiPartDiDui>();
			this.m_diDuiPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseDiDuiWindow();
			};
		}
		this.m_diDuiWindow.SetContent(this.m_diDuiWindow.BodyPresenter, this.m_diDuiPart, 0.0, 0.0, true);
	}

	private void CloseDiDuiWindow()
	{
		if (null != this.m_diDuiPart)
		{
			this.m_diDuiPart.transform.parent = null;
			Object.Destroy(this.m_diDuiPart.gameObject);
			this.m_diDuiPart = null;
		}
		if (null != this.m_diDuiWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_diDuiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_diDuiWindow, true);
			this.m_diDuiWindow = null;
		}
	}

	private void OnOpenPaiHangWindow(RankType type)
	{
		if (this.m_paiHangWindow == null)
		{
			this.m_paiHangWindow = U3DUtils.NEW<GChildWindow>();
			this.m_paiHangWindow.IsShowModal = true;
			this.m_paiHangWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_paiHangWindow, Global.GetLang("回收"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_paiHangWindow);
		}
		if (this.m_paiHangPart == null)
		{
			this.m_paiHangPart = U3DUtils.NEW<ShiLiPartPaiHang>();
			this.m_paiHangPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ClosePaiHangWindow();
			};
		}
		this.m_paiHangWindow.SetContent(this.m_paiHangWindow.BodyPresenter, this.m_paiHangPart, 0.0, 0.0, true);
		this.m_paiHangPart.GetRankInfo(type);
	}

	private void ClosePaiHangWindow()
	{
		if (null != this.m_paiHangPart)
		{
			this.m_paiHangPart.transform.parent = null;
			Object.Destroy(this.m_paiHangPart.gameObject);
			this.m_paiHangPart = null;
		}
		if (null != this.m_paiHangWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_paiHangWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_paiHangWindow, true);
			this.m_paiHangWindow = null;
		}
	}

	private void OnShiLiSelectWindow()
	{
		ShiLiData.OnShiLiSelectWindow();
	}

	private void OpenAllZhiWuWindow()
	{
		if (this.m_allZhiWuWindow == null)
		{
			this.m_allZhiWuWindow = U3DUtils.NEW<GChildWindow>();
			this.m_allZhiWuWindow.IsShowModal = true;
			this.m_allZhiWuWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_allZhiWuWindow, Global.GetLang("所有职务"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_allZhiWuWindow);
		}
		if (this.m_allZhiWuPart == null)
		{
			this.m_allZhiWuPart = U3DUtils.NEW<ShiLiBattleParAllZhiWu>();
			this.m_allZhiWuPart.InitInfo(this.m_zhiWuData);
			this.m_allZhiWuPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAllZhiWuWindow();
			};
		}
		this.m_allZhiWuWindow.SetContent(this.m_allZhiWuWindow.BodyPresenter, this.m_allZhiWuPart, 0.0, 0.0, true);
	}

	private void CloseAllZhiWuWindow()
	{
		if (null != this.m_allZhiWuPart)
		{
			this.m_allZhiWuPart.transform.parent = null;
			Object.Destroy(this.m_allZhiWuPart.gameObject);
			this.m_allZhiWuPart = null;
		}
		if (null != this.m_allZhiWuWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_allZhiWuWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_allZhiWuWindow, true);
			this.m_allZhiWuWindow = null;
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
		MUEventManager.AddEventListener<CompZhiWuData>("CMD_SPR_COMP_ZHIWU", new Action<CompZhiWuData>(this.ServerGetZhiWuData));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompZhiWuData>("CMD_SPR_COMP_ZHIWU", new Action<CompZhiWuData>(this.ServerGetZhiWuData));
	}

	private void SendGetZhiWu(int CompType)
	{
		ShiLiPartZhiWu.m_currentSend = this;
		GameInstance.Game.ShiLiGetZhiWuInfo(CompType);
	}

	private void ServerGetZhiWuData(CompZhiWuData zhiWuData)
	{
		if (ShiLiPartZhiWu.m_currentSend != this)
		{
			return;
		}
		ShiLiPartZhiWu.m_currentSend = null;
		if (zhiWuData == null)
		{
			this.player1.InitData(null);
			this.player2.InitData(null);
			this.player3.InitData(null);
		}
		else
		{
			this.Init(zhiWuData);
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblJunXianNum;

	public GButton btnClose;

	public GButton btnPaiHang;

	public GButton btnPaiHangLast;

	public GButton btnDiDui;

	public GButton btnZhuanHuan;

	public GButton btnAllZhiWu;

	public GButton btnHelp;

	public ShiLiPartZhiWuPlayer player1;

	public ShiLiPartZhiWuPlayer player2;

	public ShiLiPartZhiWuPlayer player3;

	public GameObject goSelfContent;

	private CompZhiWuData m_zhiWuData;

	private int m_compType;

	protected GChildWindow m_diDuiWindow;

	protected ShiLiPartDiDui m_diDuiPart;

	protected GChildWindow m_paiHangWindow;

	protected ShiLiPartPaiHang m_paiHangPart;

	protected GChildWindow m_allZhiWuWindow;

	protected ShiLiBattleParAllZhiWu m_allZhiWuPart;

	public static ShiLiPartZhiWu m_currentSend;
}
