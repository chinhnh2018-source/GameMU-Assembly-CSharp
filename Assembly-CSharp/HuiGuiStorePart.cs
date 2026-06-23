using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class HuiGuiStorePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("专属商城");
		this.lblTime.text = Global.GetLang("活动时间：");
		this.lblContent.text = Global.GetLang("活动描述：活动期间，专属商城会出售大量稀有物品");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, null);
			}
		};
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		this.lblTime.text = Global.GetLang("活动时间：") + selfHuoDongLevel.BeginTime + " - " + selfHuoDongLevel.FinishTime;
		this.LoadStoreItems();
		this.SendHuiGuiStoreInfo();
	}

	private void LoadStoreItems()
	{
		this.m_lstItems.Clear();
		MUHuiGuiHuoDong selfHuoDongLevel = HuiGuiData.GetSelfHuoDongLevel();
		List<MUHuiGuiStore> storeByLevelAndDay = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetStoreByLevelAndDay(selfHuoDongLevel.HuoDongLevel, HuiGuiData.GetCurrentHuiGuiDay());
		for (int i = 0; i < storeByLevelAndDay.Count; i++)
		{
			HuiGuiStorePartItem huiGuiStorePartItem = U3DUtils.NEW<HuiGuiStorePartItem>();
			huiGuiStorePartItem.transform.SetParent(this.itemContainer);
			huiGuiStorePartItem.transform.localScale = Vector3.one;
			int num = i / 2;
			int num2 = i % 2;
			huiGuiStorePartItem.transform.localPosition = new Vector3(240f * (float)num2, 0f - 100f * (float)num, 0f);
			huiGuiStorePartItem.StoreInfo = storeByLevelAndDay[i];
			this.m_lstItems.Add(huiGuiStorePartItem);
			this.ResetItemBuyNum(huiGuiStorePartItem, HuiGuiData.cacheStoreInfo);
			huiGuiStorePartItem.OnBuyItem = new Action<HuiGuiStorePartItem>(this.OnBuyItem);
		}
	}

	private void OnBuyItem(HuiGuiStorePartItem item)
	{
		if (item.StoreInfo != null)
		{
			GoodVO goodVo = item.GoodVo;
			int price = item.StoreInfo.Price;
			int buyMaxNum = item.GetBuyMaxNum();
			HuiGuiData.OpenBuyItemWindow(item.StoreInfo.ID, goodVo, price, buyMaxNum, new Action<int, GoodVO, int>(this.OnBuyConfig));
		}
	}

	public void OnBuyConfig(int id, GoodVO goodVo, int num)
	{
		HuiGuiData.CloseBuyItemWindow();
		this.SendStoreBuy(id, goodVo, num);
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				id,
				"  ",
				goodVo.Title,
				"  ",
				num
			})
		});
	}

	private void ResetItemBuyNum(HuiGuiStorePartItem item, Dictionary<int, int> data)
	{
		int num = 0;
		if (data != null)
		{
			data.TryGetValue(item.StoreInfo.ID, ref num);
		}
		item.AddBuyNum(num);
	}

	private HuiGuiStorePartItem GetStoreItemById(int id)
	{
		return this.m_lstItems.Find((HuiGuiStorePartItem info) => info.StoreInfo.ID == id);
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
		MUEventManager.AddEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSTOREINFO", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiStoreInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_STOREBUY", new Action(this.ServeStoreBuy));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSTOREINFO", new Action<Dictionary<int, int>>(this.ServeGetHuiGuiStoreInfo));
		MUEventManager.AddEventListener("CMD_SPR_REGRESSACTIVE_STOREBUY", new Action(this.ServeStoreBuy));
	}

	private void SendHuiGuiStoreInfo()
	{
		GameInstance.Game.SendGetHuiGuiStoreInfo();
	}

	private void ServeGetHuiGuiStoreInfo(Dictionary<int, int> data)
	{
		for (int i = 0; i < this.m_lstItems.Count; i++)
		{
			this.ResetItemBuyNum(this.m_lstItems[i], data);
		}
	}

	private void ServeStoreBuy()
	{
		if (this.m_sendLeiJiPartItem != null)
		{
			this.m_sendLeiJiPartItem.AddBuyNum(this.m_buyNum);
		}
	}

	private void SendStoreBuy(int id, GoodVO goodVo, int num)
	{
		this.m_sendLeiJiPartItem = this.GetStoreItemById(id);
		this.m_buyNum = num;
		GameInstance.Game.SendHuiGuiBuyItem(id, HuiGuiData.GetSelfHuoDongLevel().HuoDongLevel, goodVo.ID, num);
	}

	private const float CellWidth = 240f;

	private const float CellHeight = 100f;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTime;

	public UILabel lblContent;

	public GButton btnClose;

	public Transform itemContainer;

	private List<HuiGuiStorePartItem> m_lstItems = new List<HuiGuiStorePartItem>();

	private HuiGuiStorePartItem m_sendLeiJiPartItem;

	private int m_buyNum;
}
