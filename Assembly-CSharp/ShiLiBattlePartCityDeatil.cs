using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartCityDeatil : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblCityName.text = Global.GetLang("百世城");
		this.btnLingDi.Text = Global.GetLang("领地信息");
		this.btnZhuJiang.Text = Global.GetLang("主将信息");
		this.btnZhanLing.Text = Global.GetLang("占领信息");
		this.lblNumWord.text = Global.GetLang("本势力战场人数 :");
		this.btnGo.Text = Global.GetLang("立即前往");
		this.lblOwnWord.text = Global.GetLang("当前归属 :");
		this.lblShopWord.text = Global.GetLang("领地商城出售 :");
		this.lblZhanLingWord.text = Global.GetLang("本势力占领百分比 :");
		this.btnLingDi.Label.lineWidth = 115;
		this.btnZhuJiang.Label.lineWidth = 115;
		this.btnZhanLing.Label.lineWidth = 115;
		this.lblOwnType.pivot = 3;
		this.lblOwnType.transform.localPosition = new Vector3(-80f, this.lblOwnType.transform.localPosition.y, this.lblOwnType.transform.localPosition.z);
		this.lblZhanLingSelf.pivot = 3;
		this.lblZhanLingSelf.transform.localPosition = new Vector3(-40f, -110f, this.lblZhanLingSelf.transform.localPosition.z);
		this.lblNum.pivot = 3;
		this.lblNum.transform.localPosition = new Vector3(5f, this.lblNum.transform.localPosition.y, this.lblNum.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnGo.gameObject.SetActive(false);
		this.btnZhuJiang.gameObject.SetActive(false);
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnLingDi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectType(CityInfoSelectType.LingDi);
		};
		this.btnZhuJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectType(CityInfoSelectType.ZhuJiang);
		};
		this.btnZhanLing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectType(CityInfoSelectType.ZhanLing);
		};
		this.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendEnterBattle();
		};
		this.SetSelectType(CityInfoSelectType.LingDi);
	}

	private void SetSelectType(CityInfoSelectType type)
	{
		if (this.m_currentSelectType == type)
		{
			return;
		}
		this.m_currentSelectType = type;
		this.SetButtonSelect(this.btnLingDi, this.m_currentSelectType == CityInfoSelectType.LingDi);
		this.SetButtonSelect(this.btnZhuJiang, this.m_currentSelectType == CityInfoSelectType.ZhuJiang);
		this.SetButtonSelect(this.btnZhanLing, this.m_currentSelectType == CityInfoSelectType.ZhanLing);
		this.goLingDiContent.SetActive(this.m_currentSelectType == CityInfoSelectType.LingDi);
		this.goZhuJiangContent.SetActive(this.m_currentSelectType == CityInfoSelectType.ZhuJiang);
		this.goZhanLingContent.SetActive(this.m_currentSelectType == CityInfoSelectType.ZhanLing);
		if (this.m_currentSelectType == CityInfoSelectType.ZhanLing && !this.m_beLoadZhanLingItems)
		{
			this.LoadJuDianItems();
			this.m_beLoadZhanLingItems = true;
		}
		else if (this.m_currentSelectType == CityInfoSelectType.ZhuJiang && !this.m_beLoadZhuJiangItems)
		{
			this.LoadZhuJiangItems();
			this.m_beLoadZhuJiangItems = true;
		}
	}

	private void LoadTest()
	{
		this.m_serverCityData = new CompBattleCifyData();
		this.m_serverCityData.RoleNum = 500;
		this.m_serverCityData.StrongholdDict[1] = 3;
		this.m_serverCityData.StrongholdDict[2] = 1;
		this.m_serverCityData.StrongholdDict[3] = 2;
		this.m_serverCityData.StrongholdDict[4] = 2;
		this.m_serverCityData.StrongholdDict[5] = 1;
		this.m_serverCityData.StrongholdDict[6] = 3;
		for (int i = 0; i < 5; i++)
		{
			CompBattleZhuJiangInfo compBattleZhuJiangInfo = new CompBattleZhuJiangInfo();
			compBattleZhuJiangInfo.Name = Global.GetLang("测试") + i;
			compBattleZhuJiangInfo.CompZhiWu = i + 1;
			compBattleZhuJiangInfo.Occupation = i;
			compBattleZhuJiangInfo.RoleSex = i % 2;
			this.m_serverCityData.ZhuJiangList.Add(compBattleZhuJiangInfo);
		}
		this.InitData(this.m_serverCityData);
	}

	private void LoadJuDianItems()
	{
		for (int i = 0; i < this.m_allJuDian.Count; i++)
		{
			ShiLiBattlePartZhanLingItem shiLiBattlePartZhanLingItem = U3DUtils.NEW<ShiLiBattlePartZhanLingItem>();
			shiLiBattlePartZhanLingItem.gameObject.name = "item" + i;
			shiLiBattlePartZhanLingItem.transform.SetParent(this.gridJueDian.transform);
			shiLiBattlePartZhanLingItem.transform.localScale = Vector3.one;
			shiLiBattlePartZhanLingItem.transform.localPosition = Vector3.zero;
			int type = 0;
			this.m_serverCityData.StrongholdDict.TryGetValue(this.m_allJuDian[i].ID, ref type);
			shiLiBattlePartZhanLingItem.SetJuDianInfo(this.m_allJuDian[i], (ShiLiType)type);
		}
		this.gridJueDian.Reposition();
	}

	private void LoadZhuJiangItems()
	{
		if (this.m_serverCityData.ZhuJiangList == null)
		{
			return;
		}
		this.ReSortShiLiInfo(this.m_serverCityData.ZhuJiangList);
		for (int i = 0; i < this.m_serverCityData.ZhuJiangList.Count; i++)
		{
			ShiLiBattlePartZhuJiangItem shiLiBattlePartZhuJiangItem = U3DUtils.NEW<ShiLiBattlePartZhuJiangItem>();
			shiLiBattlePartZhuJiangItem.gameObject.name = "item" + i;
			shiLiBattlePartZhuJiangItem.transform.SetParent(this.gridZhuJiang.transform);
			shiLiBattlePartZhuJiangItem.transform.localScale = Vector3.one;
			shiLiBattlePartZhuJiangItem.transform.localPosition = Vector3.zero;
			shiLiBattlePartZhuJiangItem.SetZhuJiangInfo(this.m_serverCityData.ZhuJiangList[i]);
		}
		this.gridZhuJiang.Reposition();
	}

	private void ReSortShiLiInfo(List<CompBattleZhuJiangInfo> lstZhuJiang)
	{
		lstZhuJiang.Sort(delegate(CompBattleZhuJiangInfo zhu1, CompBattleZhuJiangInfo zhu2)
		{
			float num = (float)(zhu1.CompZhiWu - zhu2.CompZhiWu);
			if (num > 0f)
			{
				return 1;
			}
			if (num < 0f)
			{
				return -1;
			}
			return 0;
		});
	}

	private void LoadShopItems()
	{
		string[] array = this.m_cityInfo.Reward.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			GGoodIcon ggoodIcon = this.LoadRewardItemGoodsIcon(array[i], false, true, true);
			ggoodIcon.transform.SetParent(this.gridShopItem.transform);
			ggoodIcon.transform.localScale = Vector3.one;
			ggoodIcon.transform.localPosition = Vector3.zero;
		}
		this.gridShopItem.Reposition();
	}

	private void InitData(CompBattleCifyData cityData)
	{
		this.m_serverCityData = cityData;
		this.lblNum.text = cityData.RoleNum.ToString() + " / " + this.m_cityInfo.MaxEnterNum;
		List<float> list = new List<float>();
		list.Add(0f);
		list.Add(0f);
		list.Add(0f);
		list.Add(0f);
		foreach (KeyValuePair<int, int> keyValuePair in cityData.StrongholdDict)
		{
			float jueDianRate = this.GetJueDianRate(keyValuePair.Key);
			Dictionary<int, int>.Enumerator enumerator;
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			int value = keyValuePair2.Value;
			if (value < 4)
			{
				list[value] += jueDianRate;
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"势力错误"
				});
			}
		}
		int num = 0;
		float num2 = 0f;
		for (int i = 1; i < list.Count; i++)
		{
			if (list[i] > num2)
			{
				num = i;
				num2 = list[i];
			}
		}
		if (num == 0)
		{
			this.lblOwnType.text = Global.GetLang("暂无");
		}
		else
		{
			this.lblOwnType.text = string.Concat(new object[]
			{
				ShiLiData.GetShiLiNameByType((ShiLiType)cityData.OwnCompType),
				"(",
				Mathf.RoundToInt(num2 * 100f),
				"%)"
			});
		}
		int selfCompType = ShiLiData.GetSelfCompType();
		this.lblZhanLingSelf.text = Mathf.RoundToInt(list[selfCompType] * 100f) + "%";
		this.LoadShopItems();
	}

	private void SetButtonSelect(GButton btn, bool beSelected)
	{
		if (beSelected)
		{
			this.SetButtonSprite(btn, "tab_hover");
			btn.Label.color = NGUIMath.HexToColorEx(15917758U);
		}
		else
		{
			this.SetButtonSprite(btn, "tab_normal");
			btn.Label.color = NGUIMath.HexToColorEx(7958368U);
		}
	}

	private void SetButtonSprite(GButton btn, string spriteName)
	{
		btn.target.spriteName = spriteName;
		btn.normalSprite = spriteName;
		btn.hoverSprite = spriteName;
		btn.pressedSprite = spriteName;
	}

	public void SetmCityInfo(MUForceCraft cityInfo)
	{
		this.m_cityInfo = cityInfo;
		this.lblCityName.text = this.m_cityInfo.Name;
		this.m_allJuDian = ShiLiData.GetAllForceStronghold().GetForceStrongholdsByMapCode(cityInfo.MapCode);
		this.SendGetBattleCityData();
	}

	private void SetBattleState(CompBattleGameStates state)
	{
		if (state == CompBattleGameStates.Start)
		{
			this.btnGo.gameObject.SetActive(true);
			this.btnZhuJiang.gameObject.SetActive(true);
		}
		else
		{
			this.btnGo.gameObject.SetActive(false);
			this.btnZhuJiang.gameObject.SetActive(false);
		}
	}

	public float GetJueDianRate(int juDianId)
	{
		MUForceStronghold muforceStronghold = this.m_allJuDian.Find((MUForceStronghold info) => info.ID == juDianId);
		if (muforceStronghold == null)
		{
			return 0f;
		}
		return muforceStronghold.Rate;
	}

	public GGoodIcon LoadRewardItemGoodsIcon(string goodsStr, bool isOccupation = false, bool autoListen = true, bool activeBackground = true)
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
		GGoodIcon ggoodIcon = this.CreateGoodsIcon(dummyGoodsDataMu, false, true);
		if (autoListen && null != ggoodIcon)
		{
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	public GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool activeBackground = true)
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

	private void MouseLeftButtonUp(MouseEvent evt)
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

	private void OnOpenPaiDuiWindow(int num)
	{
		if (this.m_paiDuiWindow == null)
		{
			this.m_paiDuiWindow = U3DUtils.NEW<GChildWindow>();
			this.m_paiDuiWindow.IsShowModal = true;
			this.m_paiDuiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_paiDuiWindow, Global.GetLang("排队"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_paiDuiWindow);
		}
		if (this.m_paiDuiPart == null)
		{
			this.m_paiDuiPart = U3DUtils.NEW<ShiLiBattlePartPaiDui>();
			this.m_paiDuiPart.ResetNum(num);
			this.m_paiDuiPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ClosePaiDuiWindow();
			};
		}
		this.m_paiDuiWindow.SetContent(this.m_paiDuiWindow.BodyPresenter, this.m_paiDuiPart, 0.0, 0.0, true);
	}

	private void ClosePaiDuiWindow()
	{
		this.SendCancelPaiDui();
		if (null != this.m_paiDuiPart)
		{
			this.m_paiDuiPart.transform.parent = null;
			Object.Destroy(this.m_paiDuiPart.gameObject);
			this.m_paiDuiPart = null;
		}
		if (null != this.m_paiDuiWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_paiDuiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_paiDuiWindow, true);
			this.m_paiDuiWindow = null;
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
		MUEventManager.AddEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
		MUEventManager.AddEventListener<CompBattleCifyData>("CMD_SPR_COMP_BATTLE_CITY_DATA", new Action<CompBattleCifyData>(this.ServerGetBattleCityData));
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_BATTLE_ENTER_PAIDUI", new Action<int>(this.ServerBattlePaiDui));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
		MUEventManager.RemoveEventListener<CompBattleCifyData>("CMD_SPR_COMP_BATTLE_CITY_DATA", new Action<CompBattleCifyData>(this.ServerGetBattleCityData));
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_BATTLE_ENTER_PAIDUI", new Action<int>(this.ServerBattlePaiDui));
	}

	private void SendGetBattleCityData()
	{
		if (this.m_cityInfo != null)
		{
			GameInstance.Game.ShiLiGetCompBattleCifyData(this.m_cityInfo.ID);
			Super.ShowNetWaiting(null);
		}
	}

	private void SendEnterBattle()
	{
		if (this.m_cityInfo != null)
		{
			ShiLiData.SetBattleCity(this.m_cityInfo);
			GameInstance.Game.ShiLiEnterCompBattle(this.m_cityInfo.ID);
			Super.ShowNetWaiting(null);
		}
	}

	private void ServerGetBattleCityData(CompBattleCifyData cityData)
	{
		this.InitData(cityData);
		this.SendGetBattleState();
	}

	private void SendGetBattleState()
	{
		GameInstance.Game.ShiLiGetCompBattleState();
		Super.ShowNetWaiting(null);
	}

	private void ServerGetBattleState(CompBattleGameStates state)
	{
		this.SetBattleState(state);
	}

	private void SendCancelPaiDui()
	{
		GameInstance.Game.ShiLiEnterCompBattle(0);
	}

	private void ServerBattlePaiDui(int num)
	{
		if (this.m_paiDuiPart == null)
		{
			this.OnOpenPaiDuiWindow(num);
		}
		else
		{
			this.m_paiDuiPart.ResetNum(num);
		}
	}

	private const string TabSelectedSprite = "tab_hover";

	private const string TabNormalSprite = "tab_normal";

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblCityName;

	public UILabel lblNumWord;

	public UILabel lblNum;

	public GButton btnClose;

	public GButton btnLingDi;

	public GButton btnZhuJiang;

	public GButton btnZhanLing;

	public GButton btnGo;

	public UILabel lblOwnWord;

	public UILabel lblOwnType;

	public UILabel lblShopWord;

	public UILabel lblZhanLingWord;

	public UILabel lblZhanLingSelf;

	public GameObject goLingDiContent;

	public GameObject goZhuJiangContent;

	public GameObject goZhanLingContent;

	public UIGrid gridShopItem;

	public UIGrid gridZhuJiang;

	public UIGrid gridJueDian;

	private MUForceCraft m_cityInfo;

	private CompBattleCifyData m_serverCityData;

	private bool m_beLoadZhanLingItems;

	private bool m_beLoadZhuJiangItems;

	private CityInfoSelectType m_currentSelectType;

	private List<MUForceStronghold> m_allJuDian;

	protected GChildWindow m_paiDuiWindow;

	protected ShiLiBattlePartPaiDui m_paiDuiPart;
}
