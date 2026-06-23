using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ZhuTiFuJingYanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
	}

	public void AddList(string XmlList)
	{
		this.AddXmlThemeActivityJingYan(XmlList);
		Dictionary<int, ThemeActivityJingYan>.Enumerator enumerator = this.m_DicThemeActivityJingYan.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ZhuTiFuJingYanItem zhuTiFuJingYanItem = U3DUtils.NEW<ZhuTiFuJingYanItem>();
			this.m_ObservableCollection.AddNoUpdate(zhuTiFuJingYanItem);
			UILabel title = zhuTiFuJingYanItem.m_Title;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num = 1;
			KeyValuePair<int, ThemeActivityJingYan> keyValuePair = enumerator.Current;
			array[num] = Global.GetLang(keyValuePair.Value.Name);
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
			object[] array2 = new object[2];
			array2[0] = "dac7ae";
			int num2 = 1;
			KeyValuePair<int, ThemeActivityJingYan> keyValuePair2 = enumerator.Current;
			array2[num2] = Global.GetLang(keyValuePair2.Value.Depict);
			title.text = colorStringForNGUIText + Global.GetColorStringForNGUIText(array2);
		}
	}

	private void AddXmlThemeActivityJingYan(string XmlList)
	{
		XElement xelement = XElement.Parse(XmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityJingYan");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityJingYan themeActivityJingYan = new ThemeActivityJingYan();
			themeActivityJingYan.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityJingYan.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			themeActivityJingYan.TypeID = Global.GetXElementAttributeInt(xelementList[i], "TypeID");
			themeActivityJingYan.Depict = Global.GetXElementAttributeStr(xelementList[i], "Depict");
			themeActivityJingYan.Button = Global.GetXElementAttributeInt(xelementList[i], "Button");
			themeActivityJingYan.Function = Global.GetXElementAttributeInt(xelementList[i], "Function");
			themeActivityJingYan.Link = Global.GetXElementAttributeInt(xelementList[i], "Link");
			if (!this.m_DicThemeActivityJingYan.ContainsKey(themeActivityJingYan.ID))
			{
				this.m_DicThemeActivityJingYan.Add(themeActivityJingYan.ID, themeActivityJingYan);
			}
		}
	}

	public ListBox m_ListBox;

	private ObservableCollection m_ObservableCollection;

	private Dictionary<int, ThemeActivityJingYan> m_DicThemeActivityJingYan = new Dictionary<int, ThemeActivityJingYan>();
}
