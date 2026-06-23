using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuZhuanXiangPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
	}

	public void AddList(string xmlList)
	{
		this.AddXmlThemeActivityFuLi(xmlList);
		Dictionary<int, ThemeActivityFuLi>.Enumerator enumerator = this.m_DicThemeActivityFuLi.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ZhuTiFuZhuanXiangItem zhuTiFuZhuanXiangItem = U3DUtils.NEW<ZhuTiFuZhuanXiangItem>();
			this.m_ObservableCollection.AddNoUpdate(zhuTiFuZhuanXiangItem);
			if (zhuTiFuZhuanXiangItem.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(zhuTiFuZhuanXiangItem.GetComponent<UIPanel>());
			}
			if (zhuTiFuZhuanXiangItem.GetComponent<UIDragPanelContents>() != null)
			{
				zhuTiFuZhuanXiangItem.gameObject.AddComponent<UIDragPanelContents>();
			}
			zhuTiFuZhuanXiangItem.GetComponent<UIDragPanelContents>().draggablePanel = zhuTiFuZhuanXiangItem.GetComponent<UIDraggablePanel>();
			UILabel title = zhuTiFuZhuanXiangItem.m_Title;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num = 1;
			KeyValuePair<int, ThemeActivityFuLi> keyValuePair = enumerator.Current;
			array[num] = Global.GetLang(keyValuePair.Value.Name);
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
			object[] array2 = new object[2];
			array2[0] = "dac7ae";
			int num2 = 1;
			KeyValuePair<int, ThemeActivityFuLi> keyValuePair2 = enumerator.Current;
			array2[num2] = Global.GetLang(keyValuePair2.Value.Depict);
			title.text = colorStringForNGUIText + Global.GetColorStringForNGUIText(array2);
			ZhuTiFuZhuanXiangItem zhuTiFuZhuanXiangItem2 = zhuTiFuZhuanXiangItem;
			KeyValuePair<int, ThemeActivityFuLi> keyValuePair3 = enumerator.Current;
			zhuTiFuZhuanXiangItem2.Link = keyValuePair3.Value.Link;
			KeyValuePair<int, ThemeActivityFuLi> keyValuePair4 = enumerator.Current;
			if (keyValuePair4.Value.Link < 0)
			{
				zhuTiFuZhuanXiangItem.m_Btn.isEnabled = false;
				zhuTiFuZhuanXiangItem.m_Btn.gameObject.SetActive(false);
			}
		}
	}

	private void AddXmlThemeActivityFuLi(string xmlList)
	{
		XElement xelement = XElement.Parse(xmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityFuLi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityFuLi themeActivityFuLi = new ThemeActivityFuLi();
			themeActivityFuLi.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityFuLi.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			themeActivityFuLi.TypeID = Global.GetXElementAttributeInt(xelementList[i], "TypeID");
			themeActivityFuLi.Depict = Global.GetXElementAttributeStr(xelementList[i], "Depict");
			themeActivityFuLi.Button = Global.GetXElementAttributeInt(xelementList[i], "Button");
			themeActivityFuLi.Function = Global.GetXElementAttributeInt(xelementList[i], "Function");
			themeActivityFuLi.Link = Global.GetXElementAttributeInt(xelementList[i], "Link");
			if (!this.m_DicThemeActivityFuLi.ContainsKey(themeActivityFuLi.ID))
			{
				this.m_DicThemeActivityFuLi.Add(themeActivityFuLi.ID, themeActivityFuLi);
			}
		}
	}

	public ListBox m_ListBox;

	private ObservableCollection m_ObservableCollection;

	private Dictionary<int, ThemeActivityFuLi> m_DicThemeActivityFuLi = new Dictionary<int, ThemeActivityFuLi>();
}
