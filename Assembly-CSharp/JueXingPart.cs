using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class JueXingPart : UserControl, IMUEventManagerHandler
{
	private void InitTextInPrefabs()
	{
		this.InitAttr();
		this.BtnXiangQian.Label.text = Global.GetLang("觉醒石镶嵌");
		this.BtnJiHuo.Label.text = Global.GetLang("觉醒石激活");
		this.BtnMoHua.Label.text = Global.GetLang("觉醒魔化");
		this.BtnXiangQian.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnJiHuo.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnMoHua.Label.color = NGUIMath.HexToColorEx(8350293U);
		ActivityTipManager.RegActivityTipItem(18003, new ActivityTipEventHandler(this.OnActivityStateChanged));
		Global.SetSprite(base.gameObject);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnXiangQian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenChildWindow(JueXingWindowType.JueXing_XiangQian);
		};
		this.BtnJiHuo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenChildWindow(JueXingWindowType.JueXing_JiHuo);
		};
		this.BtnMoHua.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenChildWindow(JueXingWindowType.JueXing_MoHua);
		};
		this.BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow();
		};
		bool flag = ConfigSystemParam.GetSystemParamIntByName("AwakenLevelUpOpen") == 1L;
		if (flag)
		{
			this.BtnMoHua.gameObject.SetActive(true);
			this.JueXingZhiChenIcon.SetActive(true);
			this.JueXingZhiChenNum.gameObject.SetActive(true);
		}
		else
		{
			this.BtnMoHua.gameObject.SetActive(false);
			this.JueXingZhiChenIcon.SetActive(false);
			this.JueXingZhiChenNum.gameObject.SetActive(false);
		}
		this.SetBtnActieve(this.BtnXiangQian);
	}

	private new void Start()
	{
		JueXingData.ResetSelfJueXingEquips();
		this.GetServerData();
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(18003, new ActivityTipEventHandler(this.OnActivityStateChanged));
		base.OnDestroy();
	}

	private void OnActivityStateChanged(int type, ActivityTipItem args)
	{
		if (type == 18003 && null != this.TipIcon)
		{
			this.TipIcon.SetActive(args.IsActive);
		}
	}

	private void OpenChildWindow(JueXingWindowType type)
	{
		if (this.m_currentOpenType == type)
		{
			return;
		}
		GameObject windowByType = this.GetWindowByType(this.m_currentOpenType);
		if (windowByType != null)
		{
			windowByType.SetActive(false);
		}
		this.m_currentOpenType = type;
		switch (this.m_currentOpenType)
		{
		case JueXingWindowType.JueXing_XiangQian:
			this.SetBtnActieve(this.BtnXiangQian);
			if (this.m_xiangQianWindow == null)
			{
				this.m_xiangQianWindow = U3DUtils.NEW<JueXingPartXiangQian>();
				this.m_xiangQianWindow.transform.SetParent(this.TabTrans, false);
				this.m_xiangQianWindow.SetJueXingContent(JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.AttackType), JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.DefenseType));
			}
			else
			{
				this.m_xiangQianWindow.gameObject.SetActive(true);
			}
			break;
		case JueXingWindowType.JueXing_JiHuo:
			this.SetBtnActieve(this.BtnJiHuo);
			if (this.m_jihuoWindow == null)
			{
				this.m_jihuoWindow = U3DUtils.NEW<JueXingPartJiHuo>();
				this.m_jihuoWindow.transform.SetParent(this.TabTrans, false);
				this.m_jihuoWindow.InitInfos();
			}
			this.m_jihuoWindow.gameObject.SetActive(true);
			break;
		case JueXingWindowType.JueXing_MoHua:
			this.SetBtnActieve(this.BtnMoHua);
			if (this.m_mohuaWindow == null)
			{
				this.m_mohuaWindow = U3DUtils.NEW<JueXingPartMoHua>();
				this.m_mohuaWindow.transform.SetParent(this.TabTrans, false);
			}
			else
			{
				this.m_mohuaWindow.RefreshData();
				this.m_mohuaWindow.gameObject.SetActive(true);
			}
			break;
		}
	}

	private GameObject GetWindowByType(JueXingWindowType type)
	{
		GameObject result = null;
		switch (this.m_currentOpenType)
		{
		case JueXingWindowType.JueXing_XiangQian:
			result = this.m_xiangQianWindow.gameObject;
			break;
		case JueXingWindowType.JueXing_JiHuo:
			result = this.m_jihuoWindow.gameObject;
			break;
		case JueXingWindowType.JueXing_MoHua:
			result = this.m_mohuaWindow.gameObject;
			break;
		}
		return result;
	}

	public void InitAttr()
	{
		this.JueXingZhiChenNum.text = JueXingData.GetJueXingZhiChenNum().ToString();
	}

	public void SetBtnActieve(GButton btn)
	{
		if (btn == this.m_BtnLastSelect)
		{
			return;
		}
		if (null != btn)
		{
			btn.Label.color = NGUIMath.HexToColorEx(16434701U);
			btn.normalSprite = "tabhover";
			btn.Refresh();
		}
		if (null != this.m_BtnLastSelect)
		{
			this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(8350293U);
			this.m_BtnLastSelect.normalSprite = "tabnormal";
			this.m_BtnLastSelect.Refresh();
		}
		this.m_BtnLastSelect = btn;
	}

	private void ClearTabChildren()
	{
		if (this.TabTrans != null && this.TabTrans.childCount != 0)
		{
			int i = 0;
			int childCount = this.TabTrans.childCount;
			while (i < childCount)
			{
				Object.Destroy(this.TabTrans.GetChild(i).gameObject);
				i++;
			}
		}
	}

	private void OpenHelpWindow()
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		this.m_helpPart.SetHelpInfo(JueXingData.GetHelpData().list);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
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
		MUEventManager.AddEventListener<JueXingShiData>("CMD_SPR_JUEXING_INFO", new Action<JueXingShiData>(this.GetJueXingShiInfo));
		MUEventManager.AddEventListener<int, int>("CMD_SPR_JUEXING_MOHUA", new Action<int, int>(this.ServerMoHua));
		MUEventManager.AddEventListener("CMD_SPR_JUEXING_HUISHOU", new Action(this.ServerHuiShou));
		MUEventManager.AddEventListener<int, int>("CMD_SPR_JUEXING_JIHUO", new Action<int, int>(this.JueXingShiServerJiHuo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<JueXingShiData>("CMD_SPR_JUEXING_INFO", new Action<JueXingShiData>(this.GetJueXingShiInfo));
		MUEventManager.RemoveEventListener<int, int>("CMD_SPR_JUEXING_MOHUA", new Action<int, int>(this.ServerMoHua));
		MUEventManager.RemoveEventListener("CMD_SPR_JUEXING_HUISHOU", new Action(this.ServerHuiShou));
		MUEventManager.RemoveEventListener<int, int>("CMD_SPR_JUEXING_JIHUO", new Action<int, int>(this.JueXingShiServerJiHuo));
	}

	private void GetServerData()
	{
		GameInstance.Game.JueXingGetInfo();
	}

	private void GetJueXingShiInfo(JueXingShiData data)
	{
		this.OpenChildWindow(JueXingData.GetOpenWindowType());
		JueXingData.SetOpenWindowType(JueXingWindowType.JueXing_XiangQian);
	}

	private void ServerMoHua(int jieShu, int xingShu)
	{
		this.JueXingZhiChenNum.text = JueXingData.GetJueXingZhiChenNum().ToString();
	}

	private void JueXingShiServerJiHuo(int taozhuangId, int jueXingShiId)
	{
		if (this.m_jihuoWindow != null)
		{
			this.m_jihuoWindow.JueXingShiServerJiHuo(taozhuangId, jueXingShiId);
		}
		if (this.m_xiangQianWindow != null && (taozhuangId == JueXingData.GetSelfJueXingData().AttackEquip || taozhuangId == JueXingData.GetSelfJueXingData().DefenseEquip))
		{
			this.m_xiangQianWindow.ReRefersh();
		}
	}

	private void ServerHuiShou()
	{
		this.JueXingZhiChenNum.text = JueXingData.GetJueXingZhiChenNum().ToString();
		if (this.m_mohuaWindow != null)
		{
			this.m_mohuaWindow.RefreshData();
		}
	}

	private JueXingWindowType m_currentOpenType;

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnXiangQian;

	public GButton BtnJiHuo;

	public GButton BtnMoHua;

	public GButton BtnHelp;

	public UILabel JueXingZhiChenNum;

	public GameObject JueXingZhiChenIcon;

	public Transform TabTrans;

	public GameObject TipIcon;

	private JueXingPartXiangQian m_xiangQianWindow;

	private JueXingPartJiHuo m_jihuoWindow;

	private JueXingPartMoHua m_mohuaWindow;

	private GButton m_BtnLastSelect;

	protected GChildWindow m_helpWindow;

	protected CommonHelpWindow m_helpPart;
}
