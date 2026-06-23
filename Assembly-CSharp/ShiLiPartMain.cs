using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class ShiLiPartMain : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblZongBu.text = Global.GetLang("势力总部");
		this.lblZhengDuo.text = Global.GetLang("势力争夺");
		this.lblJiHuiSuo.text = Global.GetLang("势力集会所");
		this.lblRenWu.text = Global.GetLang("任务大厅");
		this.lblShangDian.text = Global.GetLang("势力商店");
		this.lblGongGaoTitle.text = Global.GetLang("公告");
		Global.SetSprite(base.gameObject);
		this.lblDi.text = Global.GetLang("敌对势力");
		this.lblDi.pivot = 5;
		this.lblDi.transform.localPosition = new Vector3(-20f, -57f, -1f);
		this.lblDiDuiName.pivot = 3;
		this.lblDiDuiName.transform.localPosition = new Vector3(-15f, -57f, -1f);
		this.lblGongGaoTitle.pivot = 4;
		this.lblGongGaoTitle.transform.localPosition = new Vector3(4f, 85f, -1f);
		this.btnEidtorConfig.Label.text = Global.GetLang("提交");
		this.inputContent.label.text = Global.GetLang("点击输入");
		this.lblTitle.text = Global.GetLang("{c39550}修改公告{-}");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.btnZongBu).onClick = new UIEventListener.VoidDelegate(this.OnClickZongBu);
		UIEventListener.Get(this.btnZhengDuo).onClick = new UIEventListener.VoidDelegate(this.OnClickZhengDuo);
		UIEventListener.Get(this.btnJiHuiSuo).onClick = new UIEventListener.VoidDelegate(this.OnClickJiHuiSuo);
		UIEventListener.Get(this.btnRenWu).onClick = new UIEventListener.VoidDelegate(this.OnClickRenWu);
		UIEventListener.Get(this.btnShangDian).onClick = new UIEventListener.VoidDelegate(this.OnClickShangDian);
		UIEventListener.Get(this.gongGaoBox).onClick = new UIEventListener.VoidDelegate(this.ShowGongGaoEditor);
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnEditorClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.editor.SetActive(false);
		};
		this.btnEidtorConfig.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.inputContent.text == string.Empty)
			{
				Super.HintMainText(Global.GetLang("公告不能为空"), 10, 3);
				return;
			}
			if (Global.IncludeReplaceFilterFileds(this.inputContent.text))
			{
				Super.HintMainText(Global.GetLang("您输入的内容含有敏感词汇，请重新输入!"), 10, 3);
				return;
			}
			if (this.inputContent.text.Length > 30)
			{
				Super.HintMainText(Global.GetLang("您输入的公告超过了30汉字，请重新输入！"), 10, 3);
				this.inputContent.selected = true;
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.inputContent.text, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 10, 3);
					return;
				}
				if (result.is_dirty > 0)
				{
					Super.HintMainText(Global.GetLang("公告不能包含国家规定禁止的词汇!"), 10, 3);
					this.inputContent.selected = true;
					return;
				}
				this.inputContent.text = result.msg;
				this.inputContent.text = Global.StringReplaceAll(this.inputContent.text, "'", string.Empty);
				this.inputContent.text = Global.StringReplaceAll(this.inputContent.text, "|", string.Empty);
				this.inputContent.text = Global.StringReplaceAll(this.inputContent.text, "$", string.Empty);
				this.inputContent.text = Global.StringReplaceAll(this.inputContent.text, ":", string.Empty);
				this.inputContent.text = Global.StringReplaceAll(this.inputContent.text, Global.GetLang("："), string.Empty);
				this.SendSetBulletin(this.inputContent.text);
			});
		};
		this.Init();
		this.SendGetCompData();
	}

	private void Init()
	{
		this.lblGongGao.text = ShiLiData.GetSelfCompData().kfCompData.Bulletin;
		if (this.lblGongGao.text == string.Empty)
		{
			this.lblGongGao.text = Global.GetLang("势力首领很懒，走后什么都没有留下");
		}
		MUComp mucompById = ShiLiData.GetMUCompById(ShiLiData.GetSelfCompData().kfCompData.EnemyCompType);
		if (mucompById == null)
		{
			this.lblDiDuiName.text = Global.GetLang("暂无敌对");
		}
		else
		{
			this.lblDiDuiName.text = mucompById.CompName;
		}
	}

	private void ShowGongGaoEditor(GameObject go)
	{
		if (GameInstance.Game.CurrentSession.roleData.CompZhiWu != 1)
		{
			Super.HintMainText(Global.GetLang("无权修改势力公告"), 10, 3);
			return;
		}
		this.inputContent.text = ShiLiData.GetSelfCompData().kfCompData.Bulletin;
		this.editor.SetActive(true);
	}

	public void OnClickZongBu(GameObject go)
	{
		this.OpenShiLiZhiWuWindow();
	}

	public void OnOpenBattleWindowEX()
	{
		if (ShiLiData.IsCompBattleOpen())
		{
			this.OnOpenBattleWindow();
		}
		else
		{
			Super.HintMainText(Global.GetLang("功能未开启，敬请期待"), 10, 3);
		}
	}

	public void OnClickZhengDuo(GameObject go)
	{
		if (ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(120403, 1) && !ConfigVersionSystemOpen.IsVersionSystemOpen(120403))
		{
			if (ShiLiData.IsCompBattleOpen())
			{
				this.OnOpenBattleWindow();
			}
			else
			{
				Super.HintMainText(Global.GetLang("功能未开启，敬请期待"), 10, 3);
			}
			return;
		}
		PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
		{
			ID = 1531,
			Index = 1
		});
	}

	public void OnClickJiHuiSuo(GameObject go)
	{
		this.OpenShiLiInfoWindow();
	}

	public void OnClickRenWu(GameObject go)
	{
		ShiLiData.OpenRenWuWindow();
	}

	public void OnClickShangDian(GameObject go)
	{
		this.OnOpenShopWindow();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.CloseShiLiInfoWindow();
	}

	private void OpenShiLiInfoWindow()
	{
		if (this.m_shiLiInfoWindow == null)
		{
			this.m_shiLiInfoWindow = U3DUtils.NEW<GChildWindow>();
			this.m_shiLiInfoWindow.IsShowModal = true;
			this.m_shiLiInfoWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_shiLiInfoWindow, Global.GetLang("势力详情"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_shiLiInfoWindow);
		}
		if (this.m_shiLiInfoPart == null)
		{
			this.m_shiLiInfoPart = U3DUtils.NEW<ShiLiPartShiLiInfo>();
			this.m_shiLiInfoPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShiLiInfoWindow();
			};
		}
		this.m_shiLiInfoWindow.SetContent(this.m_shiLiInfoWindow.BodyPresenter, this.m_shiLiInfoPart, 0.0, 0.0, true);
	}

	private void CloseShiLiInfoWindow()
	{
		if (null != this.m_shiLiInfoPart)
		{
			this.m_shiLiInfoPart.transform.parent = null;
			Object.Destroy(this.m_shiLiInfoPart.gameObject);
			this.m_shiLiInfoPart = null;
		}
		if (null != this.m_shiLiInfoWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_shiLiInfoWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_shiLiInfoWindow, true);
			this.m_shiLiInfoWindow = null;
		}
	}

	private void OpenShiLiZhiWuWindow()
	{
		if (this.m_shiLiZhiWuWindow == null)
		{
			this.m_shiLiZhiWuWindow = U3DUtils.NEW<GChildWindow>();
			this.m_shiLiZhiWuWindow.IsShowModal = true;
			this.m_shiLiZhiWuWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_shiLiZhiWuWindow, Global.GetLang("势力职务"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_shiLiZhiWuWindow);
		}
		if (this.m_shiLiZhiWuPart == null)
		{
			this.m_shiLiZhiWuPart = U3DUtils.NEW<ShiLiPartZhiWu>();
			this.m_shiLiZhiWuPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShiLiZhiWuWindow();
			};
		}
		this.m_shiLiZhiWuWindow.SetContent(this.m_shiLiZhiWuWindow.BodyPresenter, this.m_shiLiZhiWuPart, 0.0, 0.0, true);
		this.m_shiLiZhiWuPart.GetZhiWuInfo(GameInstance.Game.CurrentSession.roleData.CompType, true);
	}

	private void CloseShiLiZhiWuWindow()
	{
		if (null != this.m_shiLiZhiWuPart)
		{
			this.m_shiLiZhiWuPart.transform.parent = null;
			Object.Destroy(this.m_shiLiZhiWuPart.gameObject);
			this.m_shiLiZhiWuPart = null;
		}
		if (null != this.m_shiLiZhiWuWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_shiLiZhiWuWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_shiLiZhiWuWindow, true);
			this.m_shiLiZhiWuWindow = null;
		}
	}

	private void OnOpenSelectWindow()
	{
		if (this.m_selectWindow == null)
		{
			this.m_selectWindow = U3DUtils.NEW<GChildWindow>();
			this.m_selectWindow.IsShowModal = true;
			this.m_selectWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_selectWindow, Global.GetLang("回收"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_selectWindow);
		}
		if (this.m_selectPart == null)
		{
			this.m_selectPart = U3DUtils.NEW<ShiLiPartSelect>();
			this.m_selectPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseSelectWindow();
			};
		}
		this.m_selectWindow.SetContent(this.m_selectWindow.BodyPresenter, this.m_selectPart, 0.0, 0.0, true);
	}

	private void CloseSelectWindow()
	{
		if (null != this.m_selectPart)
		{
			this.m_selectPart.transform.parent = null;
			Object.Destroy(this.m_selectPart.gameObject);
			this.m_selectPart = null;
		}
		if (null != this.m_selectWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_selectWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_selectWindow, true);
			this.m_selectWindow = null;
		}
	}

	private void OnOpenShopWindow()
	{
		if (this.m_shopWindow == null)
		{
			this.m_shopWindow = U3DUtils.NEW<GChildWindow>();
			this.m_shopWindow.IsShowModal = true;
			this.m_shopWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_shopWindow, Global.GetLang("势力商城"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_shopWindow);
		}
		if (this.m_shopPart == null)
		{
			this.m_shopPart = U3DUtils.NEW<MUDuiHuanPart>();
			this.m_shopPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_shopPart.InitPartData(9, 0);
			this.m_shopPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShopWindow();
				return false;
			};
		}
		this.m_shopWindow.SetContent(this.m_shopWindow.BodyPresenter, this.m_shopPart, 0.0, 0.0, true);
	}

	private void CloseShopWindow()
	{
		if (null != this.m_shopPart)
		{
			this.m_shopPart.transform.parent = null;
			Object.Destroy(this.m_shopPart.gameObject);
			this.m_shopPart = null;
		}
		if (null != this.m_shopWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_shopWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_shopWindow, true);
			this.m_shopWindow = null;
		}
	}

	private void OnOpenBattleWindow()
	{
		if (this.m_battleWindow == null)
		{
			this.m_battleWindow = U3DUtils.NEW<GChildWindow>();
			this.m_battleWindow.IsShowModal = true;
			this.m_battleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_battleWindow, Global.GetLang("势力商城"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_battleWindow);
		}
		if (this.m_battlePart == null)
		{
			this.m_battlePart = U3DUtils.NEW<ShiLiBattlePartMain>();
			this.m_battlePart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_battlePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseBattleWindow();
			};
		}
		this.m_battleWindow.SetContent(this.m_battleWindow.BodyPresenter, this.m_battlePart, 0.0, 0.0, true);
	}

	private void CloseBattleWindow()
	{
		if (null != this.m_battlePart)
		{
			this.m_battlePart.transform.parent = null;
			Object.Destroy(this.m_battlePart.gameObject);
			this.m_battlePart = null;
		}
		if (null != this.m_battleWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_battleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_battleWindow, true);
			this.m_battleWindow = null;
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
		MUEventManager.AddEventListener<CompData>("CMD_SPR_COMP_DATA", new Action<CompData>(this.ServerGetCompDataResult));
		MUEventManager.AddEventListener("CMD_SPR_COMP_SET_BULLETIN", new Action(this.ServerSetBulletinResult));
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_CHANGE_ZHIWU", new Action<int>(this.ServerZhiWuChange));
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_JOIN", new Action<int>(this.ServerJoinCompReslut));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompData>("CMD_SPR_COMP_DATA", new Action<CompData>(this.ServerGetCompDataResult));
		MUEventManager.RemoveEventListener("CMD_SPR_COMP_SET_BULLETIN", new Action(this.ServerSetBulletinResult));
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_CHANGE_ZHIWU", new Action<int>(this.ServerZhiWuChange));
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_JOIN", new Action<int>(this.ServerJoinCompReslut));
	}

	private void SendGetCompData()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.ShiLiGetCompData();
	}

	private void SendSetBulletin(string content)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.ShiLiSetBulltin(content);
	}

	private void SendEnterMap()
	{
		MUComp mucompById = ShiLiData.GetMUCompById(GameInstance.Game.CurrentSession.roleData.CompType);
		if (mucompById != null)
		{
			GameInstance.Game.EnterCompMap(mucompById.MapCode, 0, 0, 0, 0);
		}
	}

	private void ServerGetCompDataResult(CompData data)
	{
		this.Init();
	}

	private void ServerSetBulletinResult()
	{
		this.editor.SetActive(false);
		Super.HintMainText(Global.GetLang("修改成功"), 10, 3);
		ShiLiData.GetSelfCompData().kfCompData.Bulletin = this.inputContent.text;
		this.lblGongGao.text = ShiLiData.GetSelfCompData().kfCompData.Bulletin;
		if (this.lblGongGao.text == string.Empty)
		{
			this.lblGongGao.text = Global.GetLang("势力首领很懒，走后什么都没有留下");
		}
	}

	private void ServerJoinCompReslut(int compType)
	{
		this.CloseShiLiZhiWuWindow();
		GameInstance.Game.ShiLiGetCompData();
	}

	private void ServerZhiWuChange(int type)
	{
		ShiLiData.GetSelfCompData().kfCompData.CompType = type;
		this.Init();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton btnClose;

	public GameObject btnZongBu;

	public GameObject btnZhengDuo;

	public GameObject btnJiHuiSuo;

	public GameObject btnRenWu;

	public GameObject btnShangDian;

	public UILabel lblZongBu;

	public UILabel lblZhengDuo;

	public UILabel lblJiHuiSuo;

	public UILabel lblRenWu;

	public UILabel lblShangDian;

	public UILabel lblGongGaoTitle;

	public UILabel lblGongGao;

	public UILabel lblDiDuiName;

	public GameObject gongGaoBox;

	public GameObject editor;

	public GButton btnEditorClose;

	public GButton btnEidtorConfig;

	public UIInput inputContent;

	public UILabel lblDi;

	public UILabel lblTitle;

	protected GChildWindow m_shiLiInfoWindow;

	protected ShiLiPartShiLiInfo m_shiLiInfoPart;

	protected GChildWindow m_shiLiZhiWuWindow;

	protected ShiLiPartZhiWu m_shiLiZhiWuPart;

	protected GChildWindow m_selectWindow;

	protected ShiLiPartSelect m_selectPart;

	protected GChildWindow m_shopWindow;

	protected MUDuiHuanPart m_shopPart;

	protected GChildWindow m_battleWindow;

	protected ShiLiBattlePartMain m_battlePart;
}
