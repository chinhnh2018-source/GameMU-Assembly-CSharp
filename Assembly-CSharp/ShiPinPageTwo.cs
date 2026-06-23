using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiPinPageTwo : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.ChangeObjPos(1, -70f, this._NoUpFull.transform);
		this._IconDraggablePanel.dragEffect = 0;
		this._IconDraggablePanel.restrictWithinPanel = false;
	}

	public override void OnActive(bool active)
	{
		base.OnActive(active);
		float time = (!this.m_Active) ? 0.5f : 0.01f;
		this.m_Active = active;
		base.gameObject.SetActive(active);
		if (active)
		{
			Super.ShowNetWaiting(null);
			base.StartCoroutine<bool>(this.CarryHanderWaitForSeconds(new ShiPinPageTwo.voidDelegate(Super.HideNetWaiting), time));
			if (this.m_xmlLstdata != null)
			{
				base.StartCoroutine<bool>(this.InitIcon(this.m_xmlLstdata));
				this.m_xmlLstdata = null;
			}
		}
		else
		{
			Super.HideNetWaiting();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Super.HideNetWaiting();
	}

	private void InitPage()
	{
		if (this.m_PageList.Count == 0)
		{
			int itemPerPage = this._IconListBox.ItemPerPage;
			if (1 < itemPerPage)
			{
				this.m_PageList.Add(this._PageSp);
				this.m_OBC.AddNoUpdate(this._PageSp);
				this._PageSp.spriteName = "Page0";
				this.tempPaneStat = this._PageSp;
				for (int i = 1; i < itemPerPage; i++)
				{
					GameObject gameObject = U3DUtils.Clone(this._PageSp.transform.parent.gameObject, this._PageSp.gameObject);
					if (null != gameObject)
					{
						UISprite component = gameObject.GetComponent<UISprite>();
						if (null != component)
						{
							component.spriteName = "Page1";
							this.m_PageList.Add(component);
							this.m_OBC.AddNoUpdate(component);
						}
					}
				}
				for (int j = 0; j < this.m_PageList.Count; j++)
				{
					this.m_PageList[j].transform.localScale = Vector3.one * 18f;
				}
				this._PageListBox.repositionNow = true;
				Vector3 localPosition = this._PageListBox.transform.localPosition;
				localPosition.x -= this._PageListBox.cellWidth * (float)itemPerPage / 2f;
				this._PageListBox.transform.localPosition = localPosition;
			}
			else
			{
				NGUITools.SetActive(this._PageSp, false);
			}
		}
	}

	private IEnumerator CarryHanderWaitForSeconds(ShiPinPageTwo.voidDelegate hander, float time)
	{
		yield return new WaitForSeconds(time);
		hander();
		yield break;
	}

	private void InitPrefabText()
	{
		this.mHelpLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("未佩戴的饰品原始属性生效且可叠加")
		});
		this._TitleLabels[0].text = string.Empty;
		this._TitleLabels[1].text = string.Empty;
		this._XiaoHaoLabel.text = string.Empty;
		this._XiaoHaoLabel.text = Global.GetLang("获得奖励");
		this._ActiveCondition.text = string.Empty;
		this._ActiveBtn.Label.text = string.Empty;
		this._ActiveCondition.text = Global.GetLang("激活条件");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_ObservableCollectionProp = this._PropListBox.ItemsSource;
		this.m_ObservableCollectionIcon = this._IconListBox.ItemsSource;
		this._IconDraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.OnIconDragFinsh);
		this._IconDraggablePanel.onDragIng = new UIDraggablePanel.OnDragIng(this.OnDranging);
		this.m_OBC = this._PageListBox.ItemsSource;
		this._ActiveBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this._ActiveBtn.Label.text == Global.GetLang("激活"))
			{
				if (0 < this.m_SelectGoodsID)
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendActiveShiPin(this.m_SelectGoodsID, 0);
				}
			}
			else if (this._ActiveBtn.Label.text == Global.GetLang("背包"))
			{
				PlayZone.GlobalPlayZone.CloseShiPinPart();
				PlayZone.GlobalPlayZone.ShowGamePayerRoleWindow(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao);
			}
		};
	}

	private void OnDranging()
	{
		if (Math.Abs(Math.Abs(this._IconDraggablePanel.transform.localPosition.x) - (float)(504 * this.CurrentSelectedPage)) > 2f)
		{
			for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
			{
				if (null != this.m_ShiPinIconitemLst[i])
				{
					this.m_ShiPinIconitemLst[i].CanShowTips = false;
				}
			}
		}
	}

	protected void RefreshBagPageText()
	{
		if (this.tempPaneStat != null)
		{
			this.tempPaneStat.spriteName = "Page1";
		}
		if (0 <= this.CurrentSelectedPage && this.m_PageList.Count > this.CurrentSelectedPage)
		{
			this.m_PageList[this.CurrentSelectedPage].spriteName = "Page0";
			this.tempPaneStat = this.m_PageList[this.CurrentSelectedPage];
		}
	}

	private void OnIconDragFinsh()
	{
		float num = 504f;
		Vector3 localPosition = this._IconDraggablePanel.transform.localPosition;
		if (Math.Abs(Math.Abs(localPosition.x) - num * (float)this.CurrentSelectedPage) > 60f)
		{
			if (localPosition.x > -num * (float)this.CurrentSelectedPage)
			{
				this.CurrentSelectedPage--;
				if (this.CurrentSelectedPage <= 0)
				{
					this.CurrentSelectedPage = 0;
				}
			}
			else
			{
				this.CurrentSelectedPage++;
				if (this.CurrentSelectedPage >= this._IconListBox.ItemPerPage)
				{
					this.CurrentSelectedPage = this._IconListBox.ItemPerPage - 1;
				}
			}
		}
		this._IconSpringPanel.target.x = -num * (float)this.CurrentSelectedPage;
		this._IconSpringPanel.enabled = true;
		this.RefreshBagPageText();
		for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
		{
			if (null != this.m_ShiPinIconitemLst[i])
			{
				this.m_ShiPinIconitemLst[i].CanShowTips = true;
			}
		}
	}

	private void RefreshGoodsName(int goodsId)
	{
		this._TitleLabels[0].text = Global.GetGoodsNameByID(goodsId, true);
		this.RefreshDicDescription(goodsId);
		this.m_SelectGoodsID = goodsId;
		this.RefreshActiveAward(goodsId);
		this._TitleLabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("激活条件")
		});
	}

	private void ChangeObjPos(int Type, float pos, Transform tr)
	{
		Vector3 localPosition = tr.localPosition;
		if (Type == 0)
		{
			localPosition.x = pos;
		}
		else if (Type == 1)
		{
			localPosition.y = pos;
		}
		else if (Type == 2)
		{
			localPosition.z = pos;
		}
		tr.localPosition = localPosition;
	}

	private void RefreshDicDescription(int GoodsID)
	{
		string text = string.Empty;
		if (this.m_OrnamentXmlConfig != null)
		{
			OrnamentXmlData data = this.m_OrnamentXmlConfig.GetXmlByGoodsId(GoodsID);
			OrnamentData ornamentData = this.m_DataOrnamentDataLst.Find((OrnamentData e) => e.ID == GoodsID);
			if (data != null)
			{
				string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					data.Description
				});
				if (data.Type != 1)
				{
					int num = (0 > data.GoalNum) ? 0 : data.GoalNum;
					int num2 = 0;
					if (ornamentData != null)
					{
						num2 = ((ornamentData.Param1 > num) ? num : ornamentData.Param1);
					}
					text = string.Format("{0}{1}", colorStringForNGUIText, string.Format("({0}/{1})", num2, num));
					if (num > num2)
					{
						NGUITools.SetActive(this._ActiveBtn, false);
						NGUITools.SetActive(this._UpFull, false);
						NGUITools.SetActive(this._NoUpFull, true);
						NGUITools.SetActive(this._ActiveObj, true);
					}
					else
					{
						GoodsData goodsData = Global.GetRoleDecorationList().Find((GoodsData l) => l.GoodsID == data.GoodsID);
						if (goodsData != null)
						{
							NGUITools.SetActive(this._ActiveObj, false);
							NGUITools.SetActive(this._UpFull, true);
							NGUITools.SetActive(this._NoUpFull, false);
						}
						else
						{
							this._ActiveBtn.Label.text = Global.GetLang("激活");
							this.ChangeBtnState(this._ActiveBtn, true);
							NGUITools.SetActive(this._ActiveBtn, true);
							NGUITools.SetActive(this._ActiveObj, true);
							NGUITools.SetActive(this._UpFull, false);
							NGUITools.SetActive(this._NoUpFull, false);
						}
					}
				}
				else if (Global.GetRoleDecorationList().Find((GoodsData l) => l.GoodsID == data.GoodsID) == null)
				{
					text = string.Format("{0}", colorStringForNGUIText);
					this._ActiveBtn.Label.text = Global.GetLang("背包");
					this.ChangeBtnState(this._ActiveBtn, true);
					NGUITools.SetActive(this._ActiveBtn, true);
					NGUITools.SetActive(this._ActiveObj, true);
					NGUITools.SetActive(this._NoUpFull, false);
					NGUITools.SetActive(this._UpFull, false);
				}
				else
				{
					text = string.Format("{0}", colorStringForNGUIText);
					NGUITools.SetActive(this._ActiveObj, false);
					NGUITools.SetActive(this._UpFull, true);
					NGUITools.SetActive(this._NoUpFull, false);
				}
			}
		}
		this._ActiveCondition.text = text;
	}

	private void ChangeBtnState(GButton btn, bool CanClick)
	{
		if (null != btn)
		{
			btn.disabledSprite = ((!CanClick) ? "Btn2" : "Btn1");
			btn.isEnabled = CanClick;
		}
	}

	private void RefreshProperty(int GoodsID)
	{
		this.m_ObservableCollectionProp.Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
		if (goodsXmlNodeByID != null)
		{
			List<ShiPinPageTwo.PropData> list = new List<ShiPinPageTwo.PropData>();
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			if (equipProps != null && 0 < equipProps.Length)
			{
				for (int i = 1; i < equipProps.Length; i++)
				{
					if (0.0 < equipProps[i])
					{
						dictionary.Add(i, equipProps[i]);
					}
				}
			}
			if (0 < dictionary.Count)
			{
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int extPropIndexesShowListByID = ConfigExtPropIndexes.GetExtPropIndexesShowListByID(keyValuePair.Key);
					ShiPinPageTwo.PropData propData = default(ShiPinPageTwo.PropData);
					propData.ShowList = extPropIndexesShowListByID;
					object[] array = new object[2];
					array[0] = "e3b36c";
					int num = 1;
					Dictionary<int, double>.Enumerator enumerator;
					KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
					array[num] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(keyValuePair2.Key, false) + Global.GetLang("：");
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
					object[] array2 = new object[2];
					array2[0] = "dac7ae";
					int num2 = 1;
					KeyValuePair<int, double> keyValuePair3 = enumerator.Current;
					object obj;
					if (ConfigExtPropIndexes.GetPercentByID(keyValuePair3.Key))
					{
						string text = "{0}%";
						KeyValuePair<int, double> keyValuePair4 = enumerator.Current;
						obj = string.Format(text, ShiPinPart.CutDoubleValue2(keyValuePair4.Value * 100.0));
					}
					else
					{
						KeyValuePair<int, double> keyValuePair5 = enumerator.Current;
						obj = ShiPinPart.ToInt(keyValuePair5.Value).ToString();
					}
					array2[num2] = obj;
					propData.str = colorStringForNGUIText + Global.GetColorStringForNGUIText(array2);
					list.Add(propData);
				}
			}
			if (0 < list.Count)
			{
				list.Sort((ShiPinPageTwo.PropData x, ShiPinPageTwo.PropData y) => x.ShowList.CompareTo(y.ShowList));
				for (int j = 0; j < list.Count; j++)
				{
					ShiPinPageTwo.PropData propData2 = list[j];
					ShiPinPropertyItem shiPinPropertyItem = U3DUtils.NEW<ShiPinPropertyItem>();
					shiPinPropertyItem.Label1 = propData2.str;
					shiPinPropertyItem.bShowUp = false;
					shiPinPropertyItem.Label2 = string.Empty;
					this.m_ObservableCollectionProp.AddNoUpdate(shiPinPropertyItem);
					shiPinPropertyItem.DraggablePanel = this._PropDraggablePanel;
				}
				this._PropListBox.repositionNow = true;
			}
		}
	}

	private void RefreshActiveAward(int GoodsID)
	{
		OrnamentXmlData xmlByGoodsId = this.m_OrnamentXmlConfig.GetXmlByGoodsId(GoodsID);
		if (xmlByGoodsId != null)
		{
			if (xmlByGoodsId.GoalAward > 0)
			{
				this._CostLabel.text = xmlByGoodsId.GoalAward.ToString();
				NGUITools.SetActive(this._CostLabel.transform.parent, true);
			}
			else
			{
				NGUITools.SetActive(this._CostLabel.transform.parent, false);
			}
			this.RefreshDicDescription(GoodsID);
		}
	}

	private void IconitemClick(object sender, DPSelectedItemEventArgs s)
	{
		if (s != null)
		{
			if (s.IDType == 10)
			{
				this.GoodsID = s.ID;
			}
			for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
			{
				this.m_ShiPinIconitemLst[i].BSelect = (this.m_ShiPinIconitemLst[i].GoodsId == s.ID);
			}
		}
		else if (this.m_ShiPinIconitemLst != null && 0 < this.m_ShiPinIconitemLst.Count)
		{
			this.m_ShiPinIconitemLst[0].BSelect = true;
			this.RefreshProperty(this.m_ShiPinIconitemLst[0].GoodsId);
			this.RefreshGoodsName(this.m_ShiPinIconitemLst[0].GoodsId);
		}
	}

	private IEnumerator InitIcon(List<OrnamentXmlData> xmlLstdata)
	{
		if (xmlLstdata != null)
		{
			this.m_ObservableCollectionIcon.Clear();
			int xmlLstListCount = xmlLstdata.Count;
			if (0 < xmlLstListCount)
			{
				Super.ShowNetWaiting(null);
				this._IconListBox.ItemPerPage = Super.ToInt((double)xmlLstListCount / (double)this.PageCount, 0);
				this._IconListBox.maxPerLine = this._IconListBox.ItemPerPage * this.PageLineCount;
				for (int i = 0; i < this.PageCount * this._IconListBox.ItemPerPage; i++)
				{
					GameObject empty = new GameObject();
					empty.name = string.Format("{0}", this.GetIndex(i, this._IconListBox.ItemPerPage * this.PageLineCount));
					if (i % 12 == 1)
					{
						yield return null;
					}
					this.m_ObservableCollectionIcon.AddNoUpdate(empty);
				}
				this._IconListBox.transform.parent.GetComponent<BoxCollider>().enabled = (24 > xmlLstListCount);
				for (int j = 0; j < xmlLstListCount; j++)
				{
					if (j % 4 == 1)
					{
						yield return null;
					}
					OrnamentXmlData dataItem = xmlLstdata[j];
					ShiPinIconitem icon = U3DUtils.NEW<ShiPinIconitem>();
					icon.HaveTips = true;
					icon.GoodsId = dataItem.GoodsID;
					if (24 <= xmlLstListCount)
					{
						icon.DraggablePanel = this._IconDraggablePanel;
					}
					int index = this.GetPageInListBoxIndex(j);
					this._IconListBox.Replace(index, icon.gameObject);
					this.m_ShiPinIconitemLst.Add(icon);
					icon.Icon.GoodImg.ToGrayBitmap = !dataItem.HaveActive;
					icon.ShowLock = false;
					icon.IsGoods = true;
					icon.Tips = false;
					icon.Hander = new DPSelectedItemEventHandler(this.IconitemClick);
					icon.LockColliderEnable = false;
					icon._Type = dataItem.Type;
					icon.BShowWarning = (dataItem.CanActive && !dataItem.HaveActive);
				}
			}
			if (24 <= xmlLstListCount)
			{
				this._IconListBox.transform.parent.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				this._IconListBox.transform.parent.GetComponent<BoxCollider>().enabled = false;
			}
			this._IconListBox.repositionNow = true;
			this.InitPage();
			this.IconitemClick(null, null);
			Super.HideNetWaiting();
		}
		yield break;
	}

	private string GetIndex(int i, int PageCountOfline)
	{
		if (i < this.PageLineCount)
		{
			return string.Format("{0}_{1}_{2}", 0, 0, i);
		}
		int num = i % PageCountOfline / this.PageLineCount;
		int num2 = i % this.PageLineCount;
		int num3 = i / PageCountOfline;
		return string.Format("{0}_{1}_{2}", num, num3, num2);
	}

	private string GetPageIndex(int i)
	{
		int num = i / this.PageCount;
		int num2 = i % this.PageLineCount;
		int num3 = (i - num * this.PageCount) / this.PageLineCount;
		return string.Format("{0}_{1}_{2}", num, num3, num2);
	}

	private int GetPageInListBoxIndex(int index)
	{
		string pageIndex = this.GetPageIndex(index);
		for (int i = 0; i < this.m_ObservableCollectionIcon.Count; i++)
		{
			GameObject at = this.m_ObservableCollectionIcon.GetAt(i);
			if (null != at && pageIndex == at.name)
			{
				return i;
			}
		}
		return -1;
	}

	public void RefreshShiPinActive()
	{
		ShiPinIconitem shiPinIconitem = this.m_ShiPinIconitemLst.Find((ShiPinIconitem e) => e.GoodsId == this.m_SelectGoodsID);
		if (null != shiPinIconitem)
		{
			shiPinIconitem.BShowWarning = false;
			this.GoodsID = shiPinIconitem.GoodsId;
			shiPinIconitem.Icon.GoodImg.ToGrayBitmap = false;
		}
	}

	public void RefreshShiPinIcons(List<OrnamentData> data, OrnamentXmlConfig xmlData)
	{
		this.m_DataOrnamentDataLst = data;
		this.m_OrnamentXmlConfig = xmlData;
		List<OrnamentXmlData> list = new List<OrnamentXmlData>();
		List<OrnamentXmlData> ornamentXmlDataLst = xmlData.GetOrnamentXmlDataLst();
		int count = ornamentXmlDataLst.Count;
		if (0 < count)
		{
			for (int i = 0; i < count; i++)
			{
				OrnamentXmlData dataItem = ornamentXmlDataLst[i];
				if (dataItem != null)
				{
					if (dataItem.Type == 1)
					{
						if (!Global.GetDecorationHaveActive(dataItem.GoodsID) && 0 < Global.GetRoleGoodsNumberCountByGoodsID(dataItem.GoodsID))
						{
							dataItem.CanActive = true;
						}
					}
					else
					{
						OrnamentData ornamentData = data.Find((OrnamentData e) => e.ID == dataItem.GoodsID);
						if (ornamentData != null && 6 <= ornamentData.ID)
						{
							if (dataItem.GoalNum <= ornamentData.Param1)
							{
								dataItem.CanActive = true;
							}
							else
							{
								dataItem.CanActive = false;
							}
						}
					}
				}
				if (!Global.GetDecorationHaveActive(dataItem.GoodsID))
				{
					if (dataItem.Show == 1)
					{
						list.Add(dataItem);
					}
				}
				else
				{
					dataItem.HaveActive = true;
					list.Add(dataItem);
				}
			}
		}
		if (0 < list.Count)
		{
			List<OrnamentXmlData> list2 = list.FindAll((OrnamentXmlData e) => true == e.HaveActive);
			List<OrnamentXmlData> list3 = list.FindAll((OrnamentXmlData e) => e.CanActive && false == e.HaveActive);
			List<OrnamentXmlData> list4 = list.FindAll((OrnamentXmlData e) => !e.CanActive && false == e.HaveActive);
			List<OrnamentXmlData> list5 = new List<OrnamentXmlData>();
			if (0 < list2.Count)
			{
				list2.Sort((OrnamentXmlData x, OrnamentXmlData y) => x.List - y.List);
				list5.AddRange(list2);
			}
			if (0 < list3.Count)
			{
				list3.Sort((OrnamentXmlData x, OrnamentXmlData y) => x.List - y.List);
				list5.AddRange(list3);
			}
			if (0 < list4.Count)
			{
				list4.Sort((OrnamentXmlData x, OrnamentXmlData y) => x.List - y.List);
				list5.AddRange(list4);
			}
			if (this.m_Active)
			{
				base.StartCoroutine<bool>(this.InitIcon(list5));
			}
			else
			{
				this.m_xmlLstdata = list5;
			}
		}
	}

	public int GoodsID
	{
		get
		{
			return this.m_GoodsID;
		}
		set
		{
			this.m_GoodsID = value;
			this.RefreshGoodsName(this.m_GoodsID);
			this.RefreshProperty(this.m_GoodsID);
		}
	}

	public UILabel[] _TitleLabels;

	public GameObject _UpFull;

	public GameObject _NoUpFull;

	public GameObject _ActiveObj;

	public GButton _ActiveBtn;

	public UILabel _XiaoHaoLabel;

	public UILabel _CostLabel;

	public ListBox _PropListBox;

	public ListBox _IconListBox;

	public ListBox _PageListBox;

	public UIDraggablePanel _PropDraggablePanel;

	public UIDraggablePanel _IconDraggablePanel;

	public SpringPanel _IconSpringPanel;

	public UISprite _PageSp;

	public UILabel _ActiveCondition;

	[SerializeField]
	private UILabel mHelpLabel;

	private int PageCount = 24;

	private int PageLineCount = 6;

	private int m_GoodsID;

	private int m_SelectGoodsID;

	private ObservableCollection m_ObservableCollectionProp;

	private ObservableCollection m_ObservableCollectionIcon;

	private ObservableCollection m_OBC;

	private List<ShiPinIconitem> m_ShiPinIconitemLst = new List<ShiPinIconitem>();

	private List<OrnamentXmlData> m_xmlLstdata;

	private bool m_Active;

	private int CurrentSelectedPage;

	private List<UISprite> m_PageList = new List<UISprite>();

	private UISprite tempPaneStat;

	private OrnamentXmlConfig m_OrnamentXmlConfig;

	private List<OrnamentData> m_DataOrnamentDataLst;

	private struct PropData
	{
		public int ShowList;

		public string str;
	}

	public delegate void voidDelegate();
}
