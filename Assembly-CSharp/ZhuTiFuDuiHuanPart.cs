using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuDuiHuanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ListCollection = this.m_Listbox.ItemsSource;
	}

	public void SetXml(string xmlList)
	{
		this.AddXmlThemeActivityDuiHuan(xmlList);
	}

	public void RefreshData(int id, int count)
	{
		for (int i = 0; i < this.m_ListCollection.Count; i++)
		{
			ZhuTiFuDuiHuanItem component = this.m_ListCollection.GetAt(i).GetComponent<ZhuTiFuDuiHuanItem>();
			if (component.ID == id)
			{
				if (count > 0)
				{
					component.Number = count;
					component.m_Btn.isEnabled = true;
				}
				else
				{
					component.m_Btn.isEnabled = false;
					component.m_Btn.target.spriteName = "yilingqu";
					component.m_Btn.target.transform.localScale = new Vector3(90f, 66f, 1f);
					component.m_Btn.Text = string.Empty;
					component.Number = 0;
				}
			}
			ObservableCollection itemsSource = component.m_ListGoods.ItemsSource;
			for (int j = 0; j < itemsSource.Count; j++)
			{
				GGoodIcon component2 = itemsSource.GetAt(j).GetComponent<GGoodIcon>();
				if (component2 != null)
				{
					GoodsData goodsData = component2.ItemObject as GoodsData;
					if (goodsData != null)
					{
						int num = 0;
						if (Global.Data.roleData.GoodsDataList != null)
						{
							for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
							{
								if (Global.Data.roleData.GoodsDataList[k].GoodsID == goodsData.GoodsID)
								{
									num += Global.Data.roleData.GoodsDataList[k].GCount;
								}
							}
						}
						if (num > goodsData.GCount)
						{
							if (num > 999)
							{
								component2.SecondText.text = "999/" + goodsData.GCount.ToString();
							}
							else
							{
								component2.SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
							}
							component2.SecondText.textColor = 4074946303U;
						}
						else
						{
							component2.SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
							component2.SecondText.textColor = 16711680U;
						}
					}
				}
			}
		}
	}

	public void AddList(string str)
	{
		string[] array = str.Split(new char[]
		{
			'|'
		});
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]) && !dictionary.ContainsKey(array[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0)))
			{
				dictionary.Add(array[i].Split(new char[]
				{
					','
				})[0].SafeToInt32(0), array[i].Split(new char[]
				{
					','
				})[1].SafeToInt32(0));
			}
		}
		this.m_ListCollection.Clear();
		Dictionary<int, ThemeActivityDuiHuan>.Enumerator enumerator = this.m_DicThemeActivityDuiHuan.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ZhuTiFuDuiHuanItem item = U3DUtils.NEW<ZhuTiFuDuiHuanItem>();
			ZhuTiFuDuiHuanItem item2 = item;
			KeyValuePair<int, ThemeActivityDuiHuan> keyValuePair = enumerator.Current;
			item2.ID = keyValuePair.Value.ID;
			this.m_ListCollection.AddNoUpdate(item);
			if (item.GetComponent<UIDragPanelContents>() == null)
			{
				item.gameObject.AddComponent<UIDragPanelContents>().draggablePanel = item.GetComponent<UIDraggablePanel>();
			}
			if (item.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(item.GetComponent<UIPanel>());
			}
			if (item.GetComponent<BoxCollider>() != null)
			{
				item.GetComponent<BoxCollider>().size = new Vector3(800f, 100f, 1f);
				item.GetComponent<BoxCollider>().center = Vector3.zero;
			}
			ObservableCollection itemsSource = item.m_ListGoods.ItemsSource;
			string text = string.Empty;
			KeyValuePair<int, ThemeActivityDuiHuan> keyValuePair2 = enumerator.Current;
			string[] array2 = keyValuePair2.Value.DuiHuanGoodsIDs.Split(new char[]
			{
				'|'
			});
			for (int j = 0; j < array2.Length; j++)
			{
				if (j < array2.Length - 1)
				{
					text = text + array2[j] + ",0,0,0,0,0|";
				}
				else
				{
					text = text + array2[j] + ",0,0,0,0,0";
				}
			}
			Super.LoadGoodsList(text, itemsSource);
			for (int k = 0; k < itemsSource.Count; k++)
			{
				GGoodIcon component = itemsSource.GetAt(k).GetComponent<GGoodIcon>();
				if (component != null)
				{
					GoodsData goodsData = component.ItemObject as GoodsData;
					if (goodsData != null)
					{
						int num = 0;
						if (Global.Data.roleData.GoodsDataList != null)
						{
							for (int l = 0; l < Global.Data.roleData.GoodsDataList.Count; l++)
							{
								if (Global.Data.roleData.GoodsDataList[l].GoodsID == goodsData.GoodsID)
								{
									num += Global.Data.roleData.GoodsDataList[l].GCount;
								}
							}
						}
						if (num > goodsData.GCount)
						{
							if (num > 999)
							{
								component.SecondText.text = "999/" + goodsData.GCount.ToString();
							}
							else
							{
								component.SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
							}
							component.SecondText.textColor = 4074946303U;
						}
						else
						{
							component.SecondText.text = num.ToString() + "/" + goodsData.GCount.ToString();
							component.SecondText.textColor = 16711680U;
						}
					}
				}
			}
			KeyValuePair<int, ThemeActivityDuiHuan> keyValuePair3 = enumerator.Current;
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(keyValuePair3.Value.NewGoodsID);
			Global.AddGoodsIcon(dummyGoodsData, item.m_GameGoods, false, GoodsOwnerTypes.SysGifts);
			if (dictionary.ContainsKey(item.ID))
			{
				if (dictionary[item.ID] > 0)
				{
					item.Number = dictionary[item.ID];
					item.m_Btn.isEnabled = true;
					item.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GameInstance.Game.SpriteFetchActivityAward(154, item.ID);
					};
				}
				else
				{
					item.m_Btn.isEnabled = false;
					item.m_Btn.target.spriteName = "yilingqu";
					item.m_Btn.target.transform.localScale = new Vector3(90f, 66f, 1f);
					item.m_Btn.Text = string.Empty;
					item.Number = 0;
				}
			}
		}
	}

	private void AddXmlThemeActivityDuiHuan(string xmlList)
	{
		XElement xelement = XElement.Parse(xmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityDuiHuan");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityDuiHuan themeActivityDuiHuan = new ThemeActivityDuiHuan();
			themeActivityDuiHuan.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityDuiHuan.NewGoodsID = Global.GetXElementAttributeStr(xelementList[i], "NewGoodsID");
			themeActivityDuiHuan.DuiHuanGoodsIDs = Global.GetXElementAttributeStr(xelementList[i], "DuiHuanGoodsIDs");
			themeActivityDuiHuan.DayMaxTimes = Global.GetXElementAttributeStr(xelementList[i], "DayMaxTimes");
			if (!this.m_DicThemeActivityDuiHuan.ContainsKey(themeActivityDuiHuan.ID))
			{
				this.m_DicThemeActivityDuiHuan.Add(themeActivityDuiHuan.ID, themeActivityDuiHuan);
			}
		}
	}

	public ListBox m_Listbox;

	private ObservableCollection m_ListCollection;

	private Dictionary<int, ThemeActivityDuiHuan> m_DicThemeActivityDuiHuan = new Dictionary<int, ThemeActivityDuiHuan>();
}
