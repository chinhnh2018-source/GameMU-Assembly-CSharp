using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JingLingQiyuanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.gameObject.SetActive(false);
		long correctLocalTime = Global.GetCorrectLocalTime();
		List<GoodsData> equipPet = Global.Data.equipPet;
		XElement petGroupPropertyXML = JingLingQiyuanPart.GetPetGroupPropertyXML();
		List<XElement> list = petGroupPropertyXML.XElementList("PetGroupProperty");
		List<XElement> list2 = new List<XElement>();
		for (int i = 0; i < list.Count; i++)
		{
			XElement xelement = list[i];
			string xelementAttrStr = xelement.GetXElementAttrStr("PetGoods");
			for (int j = 0; j < equipPet.Count; j++)
			{
				if (xelement.GetXElementAttrStr("PetGoods").Contains(equipPet[j].GoodsID.ToString()) && !list2.Contains(xelement))
				{
					list2.Add(xelement);
				}
			}
		}
		if (equipPet.Count < 4)
		{
			list2 = list;
		}
		this.Lst = new List<QiyuanItemData>();
		for (int k = 0; k < list2.Count; k++)
		{
			XElement xelement2 = list2[k];
			QiyuanItemData qiyuanItemData = new QiyuanItemData();
			this.Lst.Add(qiyuanItemData);
			string[] array = xelement2.GetXElementAttrStr("PetGoods").Split(new char[]
			{
				'|'
			});
			bool flag = true;
			for (int l = 1; l <= 4; l++)
			{
				QiyuanItemDataItem qiyuanItemDataItem = new QiyuanItemDataItem();
				if (l <= array.Length)
				{
					qiyuanItemData._QiyuanItemDataItem.Add(qiyuanItemDataItem);
					string text = array[l - 1];
					string text2 = text.Split(new char[]
					{
						','
					})[0];
					int num = 1;
					if (text.Split(new char[]
					{
						','
					}).Length > 1)
					{
						num = int.Parse(text.Split(new char[]
						{
							','
						})[1]);
					}
					int num2 = 0;
					GoodsData goodsData = null;
					foreach (GoodsData goodsData2 in equipPet)
					{
						if (goodsData2.GoodsID.ToString() == text2)
						{
							num2++;
							goodsData = goodsData2;
						}
					}
					qiyuanItemDataItem.ID = int.Parse(text2);
					qiyuanItemDataItem._GoodsData = goodsData;
					if (num2 < num)
					{
						flag = false;
						qiyuanItemDataItem.IsGray = true;
					}
				}
			}
			string text3 = string.Empty;
			foreach (string text4 in xelement2.GetXElementAttrStr("GroupProperty").Split(new char[]
			{
				'|'
			}))
			{
				string[] array3 = text4.Split(new char[]
				{
					','
				});
				if (array3.Length == 2)
				{
					string extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array3[0], false);
					if (!string.IsNullOrEmpty(extPropIndexesDescriptionByWord))
					{
						if (!ConfigExtPropIndexes.GetPercentByWord(array3[0]))
						{
							text3 = text3 + extPropIndexesDescriptionByWord + " + " + array3[1];
						}
						else
						{
							string text5 = text3;
							text3 = string.Concat(new object[]
							{
								text5,
								extPropIndexesDescriptionByWord,
								" + ",
								float.Parse(array3[1]) * 100f,
								"%"
							});
						}
						text3 += ";";
					}
				}
			}
			qiyuanItemData.Contetnt = text3;
			if (flag)
			{
				qiyuanItemData.IsTongGuo = flag;
			}
		}
		base.gameObject.SetActive(true);
		base.StartCoroutine<bool>(this.InnitData());
	}

	public IEnumerator InnitData()
	{
		this.Lst = Enumerable.ToList<QiyuanItemData>(Enumerable.OrderByDescending<QiyuanItemData, int>(this.Lst, (QiyuanItemData s) => Enumerable.Count<QiyuanItemDataItem>(s._QiyuanItemDataItem, (QiyuanItemDataItem q) => !q.IsGray)));
		IEnumerator<QiyuanItemData> en_List = Enumerable.OrderByDescending<QiyuanItemData, bool>(this.Lst, (QiyuanItemData s) => s.IsTongGuo).GetEnumerator();
		int ReturnCount = 0;
		while (en_List.MoveNext())
		{
			QiyuanItemData item = en_List.Current;
			JingLingQiyuanItem _JingLingQiyuanItem = U3DUtils.NEW<JingLingQiyuanItem>();
			_JingLingQiyuanItem.transform.SetParent(this.TableZuHe.transform, false);
			for (int i = 1; i <= 4; i++)
			{
				if (i <= item._QiyuanItemDataItem.Count)
				{
					GGoodIcon icon = _JingLingQiyuanItem.ImageArr[i - 1];
					icon.Width = 64.0;
					icon.Height = 64.0;
					if (item._QiyuanItemDataItem[i - 1]._GoodsData != null)
					{
						icon.ItemObject = item._QiyuanItemDataItem[i - 1]._GoodsData.Clone();
					}
					icon.GoodsID = item._QiyuanItemDataItem[i - 1].ID;
					GoodsData _GoodsData = item._QiyuanItemDataItem[i - 1]._GoodsData;
					GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(item._QiyuanItemDataItem[i - 1].ID);
					icon.GoodImg.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
					{
						goodVO.IconCode
					});
					UIEventListener.Get(icon.gameObject).onClick = delegate(GameObject s)
					{
						if (null != s)
						{
							GGoodIcon component = s.GetComponent<GGoodIcon>();
							if (null != component)
							{
								GoodsData goodsData = component.ItemObject as GoodsData;
								if (goodsData != null)
								{
									GTipServiceEx.ShowTip(component, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
								}
							}
						}
					};
					UIDragPanelContents _UIDragPanelContents = icon.gameObject.GetComponent<UIDragPanelContents>();
					if (null == _UIDragPanelContents)
					{
						_UIDragPanelContents = icon.gameObject.AddComponent<UIDragPanelContents>();
					}
					_UIDragPanelContents.draggablePanel = this.mUIDraggablePanel;
					if (item._QiyuanItemDataItem[i - 1].IsGray)
					{
						icon.GoodImg.ToGrayBitmap = true;
					}
				}
				if (i > item._QiyuanItemDataItem.Count)
				{
					GGoodIcon icon2 = _JingLingQiyuanItem.ImageArr[i - 1];
					icon2.gameObject.SetActive(false);
				}
			}
			_JingLingQiyuanItem.SetText(item.Contetnt, item.IsTongGuo);
			if (++ReturnCount % 5 == 0)
			{
				yield return null;
			}
		}
		this.TableZuHe.Reposition();
		yield break;
	}

	public static XElement GetPetGroupPropertyXML()
	{
		return Global.GetGameResXml(string.Format("Config/PetGroupProperty.xml", new object[0]));
	}

	public UITable TableZuHe;

	public GButton btnClose;

	[SerializeField]
	private UIDraggablePanel mUIDraggablePanel;

	private List<QiyuanItemData> Lst;
}
