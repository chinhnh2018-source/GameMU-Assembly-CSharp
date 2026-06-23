using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengMyHongBaoPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private new void Start()
	{
		this.origionalClipRange = this.mPanel.clipRange;
		this.mTab.TabIndex = 1;
		this.MyHongBaoTypaCallBack(EMyHongBaoType.MyQiang);
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.mTab.TabClick += this.mTab_TabClick;
	}

	private void mTab_TabClick(GameObject sender, int index)
	{
		this.mMyCurrentHongBaoType = (EMyHongBaoType)index;
		switch (index)
		{
		case 1:
			GameInstance.Game.SendMyHongBaoRequest(0);
			this.MyHongBaoTypaCallBack(EMyHongBaoType.MyQiang);
			break;
		case 2:
			GameInstance.Game.SendMyHongBaoRequest(1);
			this.MyHongBaoTypaCallBack(EMyHongBaoType.MyFa);
			break;
		case 3:
			GameInstance.Game.SendMyHongBaoRequest(2);
			this.MyHongBaoTypaCallBack(EMyHongBaoType.ZongLan);
			break;
		}
		if (this.mPanel != null)
		{
			this.mPanel.transform.localPosition = this.origionalPosition;
			this.mPanel.clipRange = this.origionalClipRange;
		}
	}

	private void MyHongBaoTypaCallBack(EMyHongBaoType type)
	{
		if (this.MyHongBaoTabTypeHandler != null)
		{
			this.MyHongBaoTabTypeHandler(this, new DPSelectedItemEventArgs
			{
				Type = (int)type
			});
		}
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.mListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		ListBox listBox = sender as ListBox;
		if (null != listBox)
		{
			GameObject itemByIndex = listBox.GetItemByIndex(listBox.SelectedIndex);
			if (null != itemByIndex)
			{
				ZhanMengHongBaoItem component = itemByIndex.GetComponent<ZhanMengHongBaoItem>();
				if (component.isChaKan)
				{
					this.OpenHongBaoUI(component.HongBaoType, component.HongBaoID, 2);
				}
			}
		}
	}

	public void InitUIDataByServerData(MyHongBaoData tmpData, bool isRefresh)
	{
		if (tmpData == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		if (isRefresh)
		{
			this.SetAllItemsInactive();
		}
		List<HongBaoItemData> list = tmpData.items;
		if (list == null || list.Count <= 0)
		{
			this.SetAllItemsInactive();
			return;
		}
		if (!isRefresh && !this.mIsFirstOpenWindow)
		{
			return;
		}
		this.mIsFirstOpenWindow = false;
		switch (tmpData.type)
		{
		case 0:
			list = this.SortMyQiangHongBao(list);
			break;
		case 1:
			this.SortByTime(list);
			break;
		case 2:
			this.SortByTime(list);
			break;
		}
		this.SetAllItemsInactive();
		if (list == null || list.Count <= 0)
		{
			return;
		}
		base.StopCoroutine("LoadItems");
		base.StartCoroutine<bool>(this.LoadItems(list, tmpData.type));
	}

	private IEnumerator LoadItems(List<HongBaoItemData> datas, int type)
	{
		for (int i = 0; i < datas.Count; i++)
		{
			if (i % 5 == 0)
			{
				yield return null;
			}
			if (this.mCacheZhanMengHongBaoItem.Count > 0 && i <= this.mCacheZhanMengHongBaoItem.Count - 1)
			{
				HongBaoItemData data = datas[i];
				ZhanMengHongBaoItem item = this.mCacheZhanMengHongBaoItem[i];
				NGUITools.SetActive(item.gameObject, true);
				item.InitItemDataByServerData(type, data);
				item.QiangHongBaoHandler = delegate(object s, DPSelectedItemEventArgs e)
				{
					int id = e.ID;
					int idtype = e.IDType;
					int flag = e.Flag;
					this.OpenHongBaoUI(flag, id, idtype);
				};
			}
			else
			{
				HongBaoItemData data2 = datas[i];
				ZhanMengHongBaoItem item2 = U3DUtils.NEW<ZhanMengHongBaoItem>();
				item2.InitItemDataByServerData(type, data2);
				item2.QiangHongBaoHandler = delegate(object s, DPSelectedItemEventArgs e)
				{
					int id = e.ID;
					int idtype = e.IDType;
					int flag = e.Flag;
					this.OpenHongBaoUI(flag, id, idtype);
				};
				item2.ChaHongBaoHandler = delegate(object s, DPSelectedItemEventArgs e)
				{
				};
				UIPanel temppanel = item2.transform.GetComponent<UIPanel>();
				if (temppanel != null)
				{
					Object.Destroy(temppanel);
				}
				this.mCacheZhanMengHongBaoItem.Add(item2);
				this.ItemCollection.AddNoUpdate(item2);
			}
		}
		yield break;
	}

	private void SetAllItemsInactive()
	{
		int count = this.mCacheZhanMengHongBaoItem.Count;
		if (count <= 0)
		{
			return;
		}
		for (int i = 0; i < count; i++)
		{
			ZhanMengHongBaoItem zhanMengHongBaoItem = this.mCacheZhanMengHongBaoItem[i];
			zhanMengHongBaoItem.CancelInvoke();
			NGUITools.SetActive(zhanMengHongBaoItem.gameObject, false);
		}
	}

	private List<HongBaoItemData> SortMyQiangHongBao(List<HongBaoItemData> datas)
	{
		if (datas == null || datas.Count <= 0)
		{
			return new List<HongBaoItemData>();
		}
		List<HongBaoItemData> list = new List<HongBaoItemData>();
		List<HongBaoItemData> list2 = new List<HongBaoItemData>();
		List<HongBaoItemData> list3 = new List<HongBaoItemData>();
		for (int i = 0; i < datas.Count; i++)
		{
			if (datas[i].hongBaoStatus <= 0)
			{
				list.Add(datas[i]);
			}
			else
			{
				list2.Add(datas[i]);
			}
		}
		if (list != null && list.Count > 0)
		{
			list.Sort(delegate(HongBaoItemData w1, HongBaoItemData w2)
			{
				if (w1.beginTime.CompareTo(w2.beginTime) == 1)
				{
					return -1;
				}
				if (w1.beginTime.CompareTo(w2.beginTime) == 0)
				{
					return 0;
				}
				return 1;
			});
		}
		if (list2 != null && list2.Count > 0)
		{
			list2.Sort(delegate(HongBaoItemData y1, HongBaoItemData y2)
			{
				if (y1.beginTime.CompareTo(y2.beginTime) == 1)
				{
					return -1;
				}
				if (y1.beginTime.CompareTo(y2.beginTime) == 0)
				{
					return 0;
				}
				return 1;
			});
		}
		list3.AddRange(list);
		list3.AddRange(list2);
		datas = list3;
		return datas;
	}

	private void SortByTime(List<HongBaoItemData> datas)
	{
		if (datas == null || datas.Count <= 0)
		{
			return;
		}
		datas.Sort(delegate(HongBaoItemData d1, HongBaoItemData d2)
		{
			if (d1.beginTime.CompareTo(d2.beginTime) == 1)
			{
				return -1;
			}
			if (d1.beginTime.CompareTo(d2.beginTime) == 0)
			{
				return 0;
			}
			return 1;
		});
	}

	private void OpenHongBaoUI(int hongBaoType, int hongBaoID, int type)
	{
		if (type == 1)
		{
			Global.Data.mIsChanKanHongBao = false;
			this.OpenQiangHongBaoUI(hongBaoType, hongBaoID);
		}
		else if (type == 2)
		{
			Global.Data.mIsChanKanHongBao = true;
			this.OpenChaKanHongBaoUI(hongBaoType, hongBaoID, 0);
		}
	}

	private void OpenQiangHongBaoUI(int hongBaoType, int hongBaoID)
	{
		GameInstance.Game.SendShowHongBaoRequest(hongBaoType, hongBaoID, 0);
	}

	public void MyHongBaoSort(int hongBaoId = 0)
	{
		GameInstance.Game.SendMyHongBaoRequest(0);
		this.MyHongBaoTypaCallBack(EMyHongBaoType.MyQiang);
	}

	public void MyFaHongBaoSort(int hongBaoId = 0)
	{
		GameInstance.Game.SendMyHongBaoRequest(1);
		this.MyHongBaoTypaCallBack(EMyHongBaoType.MyFa);
	}

	public void MyZongLanHongBaoSort(int hongBaoId = 0)
	{
		GameInstance.Game.SendMyHongBaoRequest(2);
		this.MyHongBaoTypaCallBack(EMyHongBaoType.ZongLan);
	}

	private void OpenChaKanHongBaoUI(int hongBaoType, int id, int type)
	{
		GameInstance.Game.SendHongBaoDetailsRequest(hongBaoType, id, 0);
	}

	protected override void OnDestroy()
	{
		this.mCacheZhanMengHongBaoItem.Clear();
		base.StopCoroutine("LoadItems");
		this.MyHongBaoTabTypeHandler = null;
		this.mListBox = null;
		this.mTab = null;
	}

	private const int kMyQiangHongBao = 0;

	private const int kMyFaHongBao = 1;

	private const int kHongBaoZongLan = 2;

	public DPSelectedItemEventHandler MyHongBaoTabTypeHandler;

	public ListBox mListBox;

	public UITab mTab;

	public UIPanel mPanel;

	private ObservableCollection _ItemCollection;

	public bool mIsFirstOpenWindow;

	public EMyHongBaoType mMyCurrentHongBaoType;

	private List<ZhanMengHongBaoItem> mCacheZhanMengHongBaoItem = new List<ZhanMengHongBaoItem>();

	private Vector3 origionalPosition = Vector3.zero;

	private Vector4 origionalClipRange = Vector4.zero;
}
