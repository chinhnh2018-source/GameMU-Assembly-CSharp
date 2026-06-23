using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class ShiLiPartSelect : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblName.text = string.Empty;
		this.lblShiLiInfo.text = string.Empty;
		this.btnChaKan.Text = Global.GetLang("查看核心");
		this.btnJiaRu.Text = Global.GetLang("加入势力");
		this.lblTuiJian.text = Global.GetLang("推荐势力礼包");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.configWindow.OnConfig = new Action(this.OnZhuanHuanShiLi);
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnTongMeng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectShiLi(ShiLiType.ZiYouTongMeng);
		};
		this.btnJiaoTuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectShiLi(ShiLiType.ShenShengJiaoTuan);
		};
		this.btnXieHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectShiLi(ShiLiType.ZhiMengXieHui);
		};
		this.btnChaKan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickChaKan();
		};
		this.btnJiaRu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickJiaRu();
		};
		this.m_lstPosition.Clear();
		this.m_lstPosition.Add(this.btnTongMeng.transform.localPosition);
		this.m_lstPosition.Add(this.btnJiaoTuan.transform.localPosition);
		this.m_lstPosition.Add(this.btnXieHui.transform.localPosition);
		this.btnTongMeng.gameObject.SetActive(false);
		this.btnJiaoTuan.gameObject.SetActive(false);
		this.btnXieHui.gameObject.SetActive(false);
		this.SendGetCompData();
	}

	private void InitInfo()
	{
		this.btnTongMeng.gameObject.SetActive(true);
		this.btnJiaoTuan.gameObject.SetActive(true);
		this.btnXieHui.gameObject.SetActive(true);
		ShiLiType shiLiType = ShiLiType.ShenShengJiaoTuan;
		if (ShiLiData.GetSelfCompData().SelectData != null && ShiLiData.GetSelfCompData().SelectData.RecommendCompList != null && ShiLiData.GetSelfCompData().SelectData.RecommendCompList.Count > 0)
		{
			shiLiType = (ShiLiType)ShiLiData.GetSelfCompData().SelectData.RecommendCompList[0];
			string recommendRewardInfos = ShiLiData.GetRecommendRewardInfos();
			if (this.m_goodsIcon != null)
			{
				Object.Destroy(this.m_goodsIcon.gameObject);
			}
			this.m_goodsIcon = ShiLiPartSelect.LoadRewardItemGoodsIcon(recommendRewardInfos, false, true, true);
			if (this.m_goodsIcon != null)
			{
				this.m_goodsIcon.transform.SetParent(this.icon.transform);
				this.m_goodsIcon.transform.localPosition = Vector3.zero;
				this.m_goodsIcon.transform.localScale = Vector3.one;
			}
			List<GButton> list = new List<GButton>();
			if (shiLiType == ShiLiType.ZiYouTongMeng)
			{
				list.Add(this.btnJiaoTuan);
				list.Add(this.btnTongMeng);
				list.Add(this.btnXieHui);
			}
			else if (shiLiType == ShiLiType.ShenShengJiaoTuan)
			{
				list.Add(this.btnTongMeng);
				list.Add(this.btnJiaoTuan);
				list.Add(this.btnXieHui);
			}
			else if (shiLiType == ShiLiType.ZhiMengXieHui)
			{
				list.Add(this.btnTongMeng);
				list.Add(this.btnXieHui);
				list.Add(this.btnJiaoTuan);
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					list[i].transform.localPosition = this.m_lstPosition[i];
				}
			}
		}
		this.OnSelectShiLi(shiLiType);
	}

	private void OnSelectShiLi(ShiLiType type)
	{
		if (this.m_SelectedType == type)
		{
			return;
		}
		if (type == ShiLiType.None)
		{
			return;
		}
		this.m_SelectedType = type;
		this.btnTongMeng.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		this.btnJiaoTuan.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		this.btnXieHui.gameObject.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		this.ShowTeXiao(type);
		GButton buttonByType = this.GetButtonByType(type);
		buttonByType.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		this.lblName.text = Global.GetLang("【") + ShiLiData.GetShiLiNameByType(type) + Global.GetLang("】");
		int num = type - ShiLiType.ShenShengJiaoTuan;
		string text = string.Empty;
		if (ShiLiData.GetSelfCompData().SelectData != null && ShiLiData.GetSelfCompData().SelectData.DaLingZhuNameList.Count == 3)
		{
			text = ShiLiData.GetSelfCompData().SelectData.DaLingZhuNameList[num];
		}
		if (text == string.Empty)
		{
			text = Global.GetLang("暂无首领");
		}
		this.lblShiLiInfo.text = Global.GetLang("势力首领:") + text;
		if (ShiLiData.GetSelfCompData().SelectData.RecommendCompList != null)
		{
			if (ShiLiData.GetSelfCompData().SelectData.RecommendCompList.IndexOf((int)this.m_SelectedType) > -1)
			{
				this.goTuiJian.SetActive(true);
			}
			else
			{
				this.goTuiJian.SetActive(false);
			}
		}
	}

	private void ShowTeXiao(ShiLiType type)
	{
		this.teXiaoJiaoTuan.SetActive(false);
		this.teXiaoTongMeng.SetActive(false);
		this.teXiaoXieHui.SetActive(false);
		GameObject teXiaoByType = this.GetTeXiaoByType(type);
		teXiaoByType.SetActive(true);
	}

	private GButton GetButtonByType(ShiLiType type)
	{
		GButton result = null;
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			result = this.btnJiaoTuan;
			break;
		case ShiLiType.ZiYouTongMeng:
			result = this.btnTongMeng;
			break;
		case ShiLiType.ZhiMengXieHui:
			result = this.btnXieHui;
			break;
		}
		return result;
	}

	private GameObject GetTeXiaoByType(ShiLiType type)
	{
		GameObject result = null;
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			result = this.teXiaoJiaoTuan;
			break;
		case ShiLiType.ZiYouTongMeng:
			result = this.teXiaoTongMeng;
			break;
		case ShiLiType.ZhiMengXieHui:
			result = this.teXiaoXieHui;
			break;
		}
		return result;
	}

	public static GGoodIcon LoadRewardItemGoodsIcon(string goodsStr, bool isOccupation = false, bool autoListen = true, bool activeBackground = true)
	{
		if (string.IsNullOrEmpty(goodsStr))
		{
			return null;
		}
		string[] array = goodsStr.Split(new char[]
		{
			','
		});
		if (array.Length != 7)
		{
			return null;
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon = ShiLiPartSelect.CreateGoodsIcon(dummyGoodsDataMu, false, true);
		if (autoListen && null != ggoodIcon)
		{
			ggoodIcon.addEventListener("click", new MouseEventHandler(ShiLiPartSelect.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	public static GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool activeBackground = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
		string text = "bagGrid4_bak";
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = ((!activeBackground) ? null : text);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodData.GoodsID;
		ggoodIcon.ItemObject = goodData;
		ggoodIcon.BoxTypes = -1;
		if (!grayShow)
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
		bool canUse = Global.CanUseGoods(goodData.GoodsID, false, true);
		Super.InitGoodsGIcon(ggoodIcon, goodData, canUse, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private static void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public void OnClickChaKan()
	{
		this.OpenShiLiZhiWuWindow((int)this.m_SelectedType);
	}

	public void OnClickJiaRu()
	{
		if (GameInstance.Game.CurrentSession.roleData.CompType > 0)
		{
			float compReplaceAmerce = ShiLiData.GetCompReplaceAmerce();
			int compReplaceNeed = ShiLiData.GetCompReplaceNeed();
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("势力贡献、势力军衔")
			});
			string message = StringUtil.substitute(Global.GetLang("您确定要转换势力吗？更换势力{0}将只保留{1}%且未领取的势力争霸战奖励将不能领取"), new object[]
			{
				colorStringForNGUIText,
				(int)(compReplaceAmerce * 100f)
			});
			this.configWindow.InitContent(message, compReplaceNeed);
			this.configWindow.gameObject.SetActive(true);
		}
		else
		{
			string message2 = StringUtil.substitute(Global.GetLang("您确定要加入【{0}】吗？更换势力需要消耗钻石，且部分内容将无法转移，请谨慎选择。"), new object[]
			{
				ShiLiData.GetShiLiNameByType(this.m_SelectedType)
			});
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message2, delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					this.SendJoinComp();
				}
			}, buttons);
		}
	}

	private void OnZhuanHuanShiLi()
	{
		if (GameInstance.Game.CurrentSession.roleData.CompType == (int)this.m_SelectedType)
		{
			Super.HintMainText(Global.GetLang("不能转换为当前势力"), 10, 3);
			return;
		}
		this.SendJoinComp();
	}

	private void OpenShiLiZhiWuWindow(int type)
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
		this.m_shiLiZhiWuPart.GetZhiWuInfo(type, false);
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
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_JOIN", new Action<int>(this.ServerJoinCompReslut));
		MUEventManager.AddEventListener<CompData>("CMD_SPR_COMP_DATA", new Action<CompData>(this.ServerGetCompDataResult));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_JOIN", new Action<int>(this.ServerJoinCompReslut));
		MUEventManager.RemoveEventListener<CompData>("CMD_SPR_COMP_DATA", new Action<CompData>(this.ServerGetCompDataResult));
	}

	private void SendJoinComp()
	{
		GameInstance.Game.ShiLiJoinComp((int)this.m_SelectedType);
	}

	private void SendGetCompData()
	{
		GameInstance.Game.ShiLiGetCompData();
	}

	private void ServerJoinCompReslut(int compType)
	{
		ShiLiData.CloseShiLiSelectWindow();
		Super.HintMainText(Global.GetLang("势力加入成功"), 10, 3);
		MUComp mucompById = ShiLiData.GetMUCompById(compType);
		if (mucompById != null)
		{
			GameInstance.Game.EnterCompMap(mucompById.MapCode, 0, 0, 0, 0);
		}
	}

	private void ServerGetCompDataResult(CompData data)
	{
		this.InitInfo();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton btnClose;

	public GButton btnTongMeng;

	public GButton btnJiaoTuan;

	public GButton btnXieHui;

	public GameObject teXiaoTongMeng;

	public GameObject teXiaoJiaoTuan;

	public GameObject teXiaoXieHui;

	public GButton btnChaKan;

	public GButton btnJiaRu;

	public UILabel lblName;

	public UILabel lblShiLiInfo;

	public GameObject goTuiJian;

	public UILabel lblTuiJian;

	public GameObject icon;

	public ShiLiPartConfigWindow configWindow;

	private ShiLiType m_SelectedType;

	private List<Vector3> m_lstPosition = new List<Vector3>();

	private GGoodIcon m_goodsIcon;

	protected GChildWindow m_shiLiZhiWuWindow;

	protected ShiLiPartZhiWu m_shiLiZhiWuPart;
}
