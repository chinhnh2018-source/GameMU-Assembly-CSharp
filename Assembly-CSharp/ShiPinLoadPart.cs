using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiPinLoadPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private IEnumerator CarryHanderWaitForSeconds(ShiPinLoadPart.voidDelegate hander, float time)
	{
		yield return new WaitForSeconds(time);
		hander();
		yield break;
	}

	private IEnumerator InitIcon(List<GoodsData> dataList)
	{
		if (dataList != null)
		{
			Super.ShowNetWaiting(null);
			this.m_ObservableCollection.Clear();
			this.m_ShiPinIconitemLst.Clear();
			for (int i = 0; i < this.m_IconCount; i++)
			{
				if (i % 10 == 1)
				{
					yield return null;
				}
				GoodsData d = null;
				ShiPinIconitem icon = U3DUtils.NEW<ShiPinIconitem>();
				if (i < dataList.Count)
				{
					d = dataList[i];
					if (d != null)
					{
						icon.GoodsId = d.GoodsID;
						icon.HaveTips = false;
						icon.IsGoods = true;
					}
				}
				icon.IsEmptyGoods = (i >= dataList.Count);
				this.m_ObservableCollection.AddNoUpdate(icon);
				icon.DraggablePanel = this._UIDraggablePanel;
				icon.Hander = new DPSelectedItemEventHandler(this.IconItemClick);
				icon.Tips = false;
				icon.Index = i;
				this.m_ShiPinIconitemLst.Add(icon);
			}
			this._ListBox.repositionNow = true;
			this.IconItemClick(null, null);
			Super.HideNetWaiting();
		}
		yield break;
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
		}
		return ggoodIcon;
	}

	private void InitPrefabText()
	{
		this._NameLabel.text = string.Empty;
		this._PropLabel.text = string.Empty;
		this._Loadbtn.Label.text = Global.GetLang("佩戴");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_ObservableCollection = this._ListBox.ItemsSource;
		this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this._ListBox.SelectionChanged = delegate(object e, MouseEvent s)
		{
			if (e != null)
			{
				GameObject gameObject = e as GameObject;
				if (null != gameObject)
				{
					ShiPinIconitem component = gameObject.GetComponent<ShiPinIconitem>();
				}
			}
		};
		this._UIDraggablePanel.onDragFinished = delegate()
		{
			if (this.m_ShiPinIconitemLst != null && 0 < this.m_ShiPinIconitemLst.Count)
			{
				for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
				{
					if (null != this.m_ShiPinIconitemLst[i])
					{
						this.m_ShiPinIconitemLst[i].CanShowTips = true;
					}
				}
			}
		};
		this._UIDraggablePanel.onDragIng = delegate()
		{
			if (this._UIDraggablePanel.currentMomentum.y != 0f && this.m_ShiPinIconitemLst != null && 0 < this.m_ShiPinIconitemLst.Count)
			{
				for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
				{
					if (null != this.m_ShiPinIconitemLst[i])
					{
						this.m_ShiPinIconitemLst[i].CanShowTips = false;
					}
				}
			}
		};
		this._Loadbtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					IDType = 20,
					ID = this.m_SelectGoodsID
				});
			}
		};
	}

	private void refreshPropName(int GoodsId)
	{
		if (0 < GoodsId)
		{
			this._NameLabel.text = Global.GetGoodsNameByID(GoodsId, true);
			this.ChangeBtnState(this._Loadbtn, true);
		}
		else
		{
			this._NameLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("暂无属性")
			});
			this.ChangeBtnState(this._Loadbtn, false);
		}
		this.m_SelectGoodsID = GoodsId;
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
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
		if (goodsXmlNodeByID != null)
		{
			List<ShiPinLoadPart.PropData> list = new List<ShiPinLoadPart.PropData>();
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			if (equipProps != null && 0 < equipProps.Length)
			{
				for (int i = 1; i < equipProps.Length; i++)
				{
					if (0.0 < equipProps[i])
					{
						dictionary.Add(i, equipProps[i] * ((double)this.SelectCaoWeiLevel * 0.2 + 0.8));
					}
				}
			}
			if (0 < dictionary.Count)
			{
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int extPropIndexesShowListByID = ConfigExtPropIndexes.GetExtPropIndexesShowListByID(keyValuePair.Key);
					ShiPinLoadPart.PropData propData = default(ShiPinLoadPart.PropData);
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
						obj = string.Format(text, keyValuePair4.Value * 100.0);
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
			string text2 = string.Empty;
			if (0 < list.Count)
			{
				list.Sort((ShiPinLoadPart.PropData x, ShiPinLoadPart.PropData y) => x.ShowList.CompareTo(y.ShowList));
				for (int j = 0; j < list.Count; j++)
				{
					text2 = text2 + list[j].str + Environment.NewLine;
				}
			}
			this._PropLabel.text = text2;
		}
		else
		{
			this._PropLabel.text = string.Empty;
		}
	}

	public void RefreshIcons(List<GoodsData> dataList, int Count)
	{
		this.m_IconCount = Count;
		if (dataList != null && 0 < dataList.Count)
		{
			base.StartCoroutine<bool>(this.InitIcon(dataList));
		}
	}

	private void IconItemClick(object sender, DPSelectedItemEventArgs s)
	{
		if (s != null)
		{
			for (int i = 0; i < this.m_ShiPinIconitemLst.Count; i++)
			{
				this.m_ShiPinIconitemLst[i].BSelect = (i == s.Index);
			}
			if (s.IDType == 10)
			{
				this.refreshPropName(s.ID);
				this.RefreshProperty(s.ID);
				ShiPinIconitem shiPinIconitem = this.m_ShiPinIconitemLst.Find((ShiPinIconitem e) => s.ID == e.GoodsId);
				GGoodIcon icon = shiPinIconitem.Icon;
				if (0 < this._GoodsRoot.childCount)
				{
					for (int j = this._GoodsRoot.childCount - 1; j >= 0; j--)
					{
						Transform child = this._GoodsRoot.GetChild(j);
						if (null != child)
						{
							Object.Destroy(child.gameObject);
						}
					}
				}
				if (null != icon)
				{
					GoodsData data = icon.ItemObject as GoodsData;
					GGoodIcon ggoodIcon = this.initGood(data, false);
					ggoodIcon.transform.SetParent(this._GoodsRoot, false);
				}
			}
		}
		else if (0 < this.m_ShiPinIconitemLst.Count)
		{
			ShiPinIconitem shiPinIconitem2 = this.m_ShiPinIconitemLst[0];
			shiPinIconitem2.BSelect = true;
			if (null != shiPinIconitem2)
			{
				GGoodIcon icon2 = shiPinIconitem2.Icon;
				if (null != icon2)
				{
					GoodsData data2 = icon2.ItemObject as GoodsData;
					if (0 < this._GoodsRoot.childCount)
					{
						for (int k = this._GoodsRoot.childCount - 1; k >= 0; k--)
						{
							Transform child2 = this._GoodsRoot.GetChild(k);
							if (null != child2)
							{
								Object.Destroy(child2.gameObject);
							}
						}
					}
					GGoodIcon ggoodIcon2 = this.initGood(data2, false);
					ggoodIcon2.transform.SetParent(this._GoodsRoot, false);
				}
			}
			this.refreshPropName(shiPinIconitem2.GoodsId);
			this.RefreshProperty(shiPinIconitem2.GoodsId);
		}
	}

	public UIScrollBar _ScrollBar;

	public ListBox _ListBox;

	public GButton _CloseBtn;

	public GButton _Loadbtn;

	public Transform _GoodsRoot;

	public UIDraggablePanel _UIDraggablePanel;

	public UILabel _NameLabel;

	public UILabel _PropLabel;

	[HideInInspector]
	public int SelectCaoWeiLevel = 1;

	private ObservableCollection m_ObservableCollection;

	private List<ShiPinIconitem> m_ShiPinIconitemLst = new List<ShiPinIconitem>();

	private int m_SelectGoodsID;

	private int m_IconCount = 10;

	public DPSelectedItemEventHandler Hander;

	private struct PropData
	{
		public int ShowList;

		public string str;
	}

	public delegate void voidDelegate();
}
