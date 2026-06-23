using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeQuanActivityShopPart : UserControl, BaseTeQuanActivityPart
{
	protected void OnEnable()
	{
		SpringPanel.Begin(this._ShopViewDragPanel.gameObject, new Vector3(0f, 0f, 0f), 10f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	public void InitShop(int TeQuanID)
	{
		if (TeQuanID != this.mTeQuanID)
		{
			this.mPathHaveInit = false;
		}
		if (!this.mPathHaveInit)
		{
			this.mTeQuanID = TeQuanID;
			this.mPathHaveInit = true;
			base.StartCoroutine<bool>(this.initShop(TeQuanID));
		}
	}

	private GameObject GetItemRoot(int index)
	{
		GameObject gameObject = null;
		int num = index / 3;
		if (num < this.mShopItemsRoots.size)
		{
			gameObject = this.mShopItemsRoots[num];
			if (null == gameObject)
			{
				this.mShopItemsRoots.RemoveAt(index);
			}
		}
		if (null == gameObject && index % 3 == 0)
		{
			gameObject = new GameObject();
			gameObject.transform.localPosition = Vector3.zero;
			this.mShopItemsRoots.Add(gameObject);
		}
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = (index / 3).ToString();
		return gameObject;
	}

	private TeQuanActivityShopItem GetItem(int index)
	{
		TeQuanActivityShopItem teQuanActivityShopItem = null;
		if (index < this.mShopItems.size)
		{
			teQuanActivityShopItem = this.mShopItems[index];
			if (null == teQuanActivityShopItem)
			{
				this.mShopItems.RemoveAt(index);
			}
		}
		if (null == teQuanActivityShopItem)
		{
			teQuanActivityShopItem = U3DUtils.NEW<TeQuanActivityShopItem>();
			this.mShopItems.Add(teQuanActivityShopItem);
		}
		return teQuanActivityShopItem;
	}

	private IEnumerator initShop(int TeQuanID)
	{
		BetterList<TeQuanShangChengVO> datas = IConfigbase<ConfigTeQuan>.Instance.GetShangChengVOItemsBuyTeQuanID(TeQuanID);
		if (0 < datas.size)
		{
			GameObject go = null;
			Super.ShowNetWaiting(null);
			for (int i = 0; i < datas.size; i++)
			{
				TeQuanActivityShopItem item = this.GetItem(i);
				if (null != item)
				{
					item.SetData(datas[i]);
					go = this.GetItemRoot(i);
					if (i % 3 == 0)
					{
						if (null == go.transform.parent || go.transform.parent != this._ShopViewRoot)
						{
							go.transform.SetParent(this._ShopViewRoot, false);
						}
						go.transform.localPosition = new Vector3(0f, (float)(-64 - go.name.SafeToInt32(0) * 96), 0f);
						if (i != 0 && i % 6 == 0)
						{
							yield return null;
						}
					}
					if (null == item.transform.parent || item.transform.parent != go.transform)
					{
						item.transform.SetParent(go.transform, false);
					}
					item.transform.localScale = Vector3.one;
					item.transform.localPosition = new Vector3((float)(0 + i % 3 * 260), 0f, 0f);
					item.DragPanel = this._ShopViewDragPanel;
					item.Hander = new DPSelectedItemEventHandler(this.ShopItemCallback);
					SpecPriorityActInfo inf = this.GetSpecPriorityActInfoByID(datas[i].ID);
					if (inf != null)
					{
						item.CanBuyNum = inf.LeftPurNum;
					}
					else
					{
						item.CanBuyNum = datas[i].GouMaiCiShu;
					}
				}
			}
			if (datas.size < this.mShopItems.size)
			{
				for (int j = this.mShopItems.size - 1; j >= datas.size; j--)
				{
					if (null != this.mShopItems[j])
					{
						GameObject obj = this.mShopItems[j].gameObject;
						if (null != obj)
						{
							Object.Destroy(obj);
						}
					}
					this.mShopItems.RemoveAt(j);
				}
			}
			Super.HideNetWaiting();
		}
		yield break;
	}

	private SpecPriorityActInfo GetSpecPriorityActInfoByID(int ID)
	{
		if (this.mInfList != null)
		{
			for (int i = 0; i < this.mInfList.size; i++)
			{
				if (ID == this.mInfList[i].ActID)
				{
					return this.mInfList[i];
				}
			}
		}
		return null;
	}

	private void ShopItemCallback(object sender, DPSelectedItemEventArgs args)
	{
		if (!this.PartOpen)
		{
			Super.HintMainText(Global.GetLang("当前活动暂未激活"), 10, 3);
			return;
		}
		int id = args.ID;
		int canUse = args.CanUse;
		int buyFrom = args.buyFrom;
		int TQID = args.MyID;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
		if (0 >= canUse)
		{
			Super.HintMainText(Global.GetLang("剩余购买次数不足"), 10, 3);
		}
		else
		{
			HuiGuiData.OpenBuyItemWindow(id, goodsXmlNodeByID, buyFrom, canUse, delegate(int e, GoodVO s, int num)
			{
				if (!this.PartOpen)
				{
					Super.HintMainText(Global.GetLang("当前活动暂未激活"), 10, 3);
					return;
				}
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetRoleTeQuanBuyOrGetAward(this.ID, TQID, num);
				HuiGuiData.CloseBuyItemWindow();
			});
		}
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshPart(SpecPriorityActInfo inf)
	{
	}

	public void RefreshPart(BetterList<SpecPriorityActInfo> infList)
	{
		this.mInfList = infList;
		for (int i = 0; i < this.mShopItems.size; i++)
		{
			if (null != this.mShopItems[i])
			{
				SpecPriorityActInfo specPriorityActInfoByID = this.GetSpecPriorityActInfoByID(this.mShopItems[i].ID);
				if (specPriorityActInfoByID != null)
				{
					this.mShopItems[i].CanBuyNum = specPriorityActInfoByID.LeftPurNum;
				}
			}
		}
	}

	public bool PartOpen { get; set; }

	public int ID { get; set; }

	[SerializeField]
	private UIDraggablePanel _ShopViewDragPanel;

	[SerializeField]
	private Transform _ShopViewRoot;

	private int mTeQuanID;

	private BetterList<SpecPriorityActInfo> mInfList;

	private BetterList<TeQuanActivityShopItem> mShopItems = new BetterList<TeQuanActivityShopItem>();

	private BetterList<GameObject> mShopItemsRoots = new BetterList<GameObject>();

	private bool mPathHaveInit;
}
