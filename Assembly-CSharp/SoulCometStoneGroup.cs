using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class SoulCometStoneGroup : UserControl
{
	private void InitTextInPrefabs()
	{
		this.title.Text = Global.GetLang("魂石组合");
	}

	protected override void InitializeComponent()
	{
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.InitConfig();
		this.InitTextInPrefabs();
	}

	private void InitConfig()
	{
		this.GetStoneGroupConfig();
		this.GetStoneGoodsTypeConfig();
	}

	public void InitStoneGroupList(Dictionary<int, GoodsData> dic_goods)
	{
		if (null == this.groupItemList)
		{
			return;
		}
		ObservableCollection itemsSource = this.groupItemList.ItemsSource;
		itemsSource.Clear();
		List<SoulCometStoneGroupItemAttribute> soulCometStoneGroupList = this.GetSoulCometStoneGroupList(dic_goods);
		if (soulCometStoneGroupList == null || soulCometStoneGroupList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < soulCometStoneGroupList.Count; i++)
		{
			SoulCometStoneGroupItemAttribute soulCometStoneGroupItemAttribute = soulCometStoneGroupList[i];
			if (soulCometStoneGroupItemAttribute != null && soulCometStoneGroupItemAttribute.list_groupItemAttr != null)
			{
				SoulCometStoneGroupItem soulCometStoneGroupItem = U3DUtils.NEW<SoulCometStoneGroupItem>();
				soulCometStoneGroupItem.soulCometStoneGroupAttribute = soulCometStoneGroupItemAttribute;
				itemsSource.AddNoUpdate(soulCometStoneGroupItem);
			}
		}
	}

	private void GetStoneGroupConfig()
	{
		if (this.list_groupAttr != null && this.list_groupAttr.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/HunShiGroup.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HunShiGroup");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		this.list_groupAttr = new List<SoulCometStoneGroupAttribute>(xelementList.Count);
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			SoulCometStoneGroupAttribute soulCometStoneGroupAttribute = new SoulCometStoneGroupAttribute();
			soulCometStoneGroupAttribute.id = Global.GetXElementAttributeInt(xelement, "ID");
			soulCometStoneGroupAttribute.name = Global.GetXElementAttributeStr(xelement, "Name");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "HunShiGoodsType");
			List<int> list = null;
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					','
				});
				if (array != null && array.Length > 0)
				{
					list = new List<int>(array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						list.Add(Convert.ToInt32(array[j]));
					}
				}
			}
			soulCometStoneGroupAttribute.list_id = list;
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GroupProperty");
			soulCometStoneGroupAttribute.properties = this.GetStoneGroupExtraProperties(xelementAttributeStr2);
			this.list_groupAttr.Add(soulCometStoneGroupAttribute);
		}
	}

	private void GetStoneGoodsTypeConfig()
	{
		if (this.dic_goodsAttr != null && this.dic_goodsAttr.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/HunShiGoodsType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HunShiGoodsType");
		if (xelementList == null || xelementList.Count <= 0)
		{
			return;
		}
		this.dic_goodsAttr = new Dictionary<int, SoulCometStoneGoodsTypeAttribute>();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Intro");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ShowGoods");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Goods");
			if (!this.dic_goodsAttr.ContainsKey(xelementAttributeInt))
			{
				SoulCometStoneGoodsTypeAttribute soulCometStoneGoodsTypeAttribute = new SoulCometStoneGoodsTypeAttribute(xelementAttributeInt, xelementAttributeStr, xelementAttributeInt2);
				this.dic_goodsAttr.Add(xelementAttributeInt, soulCometStoneGoodsTypeAttribute);
			}
		}
	}

	private List<SoulCometStoneGroupItemAttribute> GetSoulCometStoneGroupList(Dictionary<int, GoodsData> dic_goods)
	{
		if (this.dic_goodsAttr == null || this.dic_goodsAttr.Count <= 0 || this.list_groupAttr == null || this.list_groupAttr.Count <= 0)
		{
			return null;
		}
		List<SoulCometStoneGroupItemAttribute> list = new List<SoulCometStoneGroupItemAttribute>(this.list_groupAttr.Count);
		for (int i = 0; i < this.list_groupAttr.Count; i++)
		{
			SoulCometStoneGroupAttribute soulCometStoneGroupAttribute = this.list_groupAttr[i];
			if (soulCometStoneGroupAttribute != null && soulCometStoneGroupAttribute.list_id != null && soulCometStoneGroupAttribute.list_id.Count > 0)
			{
				SoulCometStoneGroupItemAttribute soulCometStoneGroupItemAttribute = new SoulCometStoneGroupItemAttribute();
				int num = 0;
				for (int j = 0; j < soulCometStoneGroupAttribute.list_id.Count; j++)
				{
					int num2 = soulCometStoneGroupAttribute.list_id[j];
					GoodsData goodsData;
					bool flag;
					if (dic_goods == null || dic_goods.Count <= 0)
					{
						goodsData = this.GetStoneGroupDefaultGoods(num2);
						flag = true;
					}
					else
					{
						goodsData = dic_goods.GetValue(num2);
						flag = (null == goodsData);
						if (goodsData == null)
						{
							goodsData = this.GetStoneGroupDefaultGoods(num2);
						}
						if (!flag)
						{
							num++;
						}
					}
					SoulCometStoneGroupGoodsItemAttribute soulCometStoneGroupGoodsItemAttribute = new SoulCometStoneGroupGoodsItemAttribute(flag, goodsData);
					if (soulCometStoneGroupGoodsItemAttribute != null)
					{
						soulCometStoneGroupItemAttribute.list_groupItemAttr.Add(soulCometStoneGroupGoodsItemAttribute);
					}
				}
				soulCometStoneGroupItemAttribute.perfect = (num == soulCometStoneGroupAttribute.list_id.Count);
				soulCometStoneGroupItemAttribute.properties = soulCometStoneGroupAttribute.properties;
				list.Add(soulCometStoneGroupItemAttribute);
			}
		}
		return Enumerable.ToList<SoulCometStoneGroupItemAttribute>(Enumerable.OrderByDescending<SoulCometStoneGroupItemAttribute, int>(list, (SoulCometStoneGroupItemAttribute g) => Enumerable.Count<SoulCometStoneGroupGoodsItemAttribute>(g.list_groupItemAttr, (SoulCometStoneGroupGoodsItemAttribute r) => !r.gray)));
	}

	private GoodsData GetStoneGroupDefaultGoods(int id)
	{
		if (this.dic_goodsAttr == null || this.dic_goodsAttr.Count <= 0)
		{
			return null;
		}
		SoulCometStoneGoodsTypeAttribute value = this.dic_goodsAttr.GetValue(id);
		if (value == null || value.defaultGoodsID <= 0)
		{
			return null;
		}
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(value.defaultGoodsID);
		GoodsData goodsData = dummyGoodsData;
		List<int> list = new List<int>();
		list.Add(1);
		list.Add(0);
		goodsData.ElementhrtsProps = list;
		return dummyGoodsData;
	}

	private string GetStoneGroupExtraProperties(string propStr)
	{
		if (string.IsNullOrEmpty(propStr))
		{
			return string.Empty;
		}
		string text = string.Empty;
		foreach (string text2 in propStr.Split(new char[]
		{
			'|'
		}))
		{
			for (int j = 0; j < ExtPropIndexes.ShengWuIndexNames.Length; j++)
			{
				if (ExtPropIndexes.ShengWuIndexNames[j].ToUpper() == text2.Split(new char[]
				{
					','
				})[0].ToUpper())
				{
					float num = 0f;
					if (!float.TryParse(text2.Split(new char[]
					{
						','
					})[1], ref num))
					{
						text = text + ExtPropIndexes.ShengWuChineseNames[j].Trim() + ":" + text2.Split(new char[]
						{
							','
						})[1];
					}
					else if (float.Parse(text2.Split(new char[]
					{
						','
					})[1]) < 1f)
					{
						string text3 = text;
						text = string.Concat(new object[]
						{
							text3,
							ExtPropIndexes.ShengWuChineseNames[j].Trim(),
							" + ",
							float.Parse(text2.Split(new char[]
							{
								','
							})[1]) * 100f,
							"%"
						});
					}
					else
					{
						text = text + ExtPropIndexes.ShengWuChineseNames[j].Trim() + " + " + text2.Split(new char[]
						{
							','
						})[1];
					}
					text += "|";
				}
			}
		}
		return SoulCometStoneGroup.ProcessStr(text);
	}

	public static string ProcessStr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return string.Empty;
		}
		if (str.Length > 0 && str.Substring(str.Length - 1) == "|")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock title;

	public ListBox groupItemList;

	public GButton closeBtn;

	private Dictionary<int, SoulCometStoneGoodsTypeAttribute> dic_goodsAttr;

	private List<SoulCometStoneGroupAttribute> list_groupAttr;
}
