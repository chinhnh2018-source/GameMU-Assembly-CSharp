using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class BaoDianPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.LoadBaoDianList();
		this.InitTabPages();
	}

	private void LoadBaoDianList()
	{
		XElement gameResXml = Global.GetGameResXml("Config/BaoDianTab.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "BaoDian");
		for (int i = 0; i < xelementList.Count; i++)
		{
			if (Global.GetXElementAttributeInt(xelementList[i], "Show") == 1)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "Name");
				this.baodianTabConfig.Add(xelementAttributeInt, xelementAttributeStr);
			}
		}
		List<XElement> list = null;
		XElement gameResXml2 = Global.GetGameResXml("Config/BaoDian.xml");
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "BaoDian");
		for (int j = 0; j < xelementList2.Count; j++)
		{
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList2[j], "TabType");
			if (!this.baodianConfig.TryGetValue(xelementAttributeInt2, ref list))
			{
				list = new List<XElement>();
				this.baodianConfig.Add(xelementAttributeInt2, list);
			}
			list.Add(xelementList2[j]);
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList2[j], "ID");
			this.baodianPromptDict.Add(xelementAttributeInt3, xelementList2[j]);
		}
	}

	private void InitTabPages()
	{
		List<XElement> listContent = null;
		this.gTabControl.ClearPages();
		this.gTabControl.AddTabPage(this.baodianTabConfig.Count);
		int num = 0;
		foreach (KeyValuePair<int, string> keyValuePair in this.baodianTabConfig)
		{
			if (this.baodianConfig.TryGetValue(keyValuePair.Key, ref listContent))
			{
				BaoDianPartPage baoDianPartPage = U3DUtils.NEW<BaoDianPartPage>();
				baoDianPartPage.TabID = keyValuePair.Key;
				this.gTabControl.SetTabButtonName(keyValuePair.Value, keyValuePair.Key);
				this.gTabControl.AddPageContent(baoDianPartPage.gameObject, keyValuePair.Key);
				baoDianPartPage.SetListContent(listContent);
				baoDianPartPage.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
			}
			else if (keyValuePair.Key != 6)
			{
				if (keyValuePair.Key == 100)
				{
					GameObject gameObject = new GameObject("parentObj");
					gameObject.layer = base.gameObject.layer;
					this.gTabControl.SetTabButtonName(keyValuePair.Value, num);
					this.gTabControl.AddPageContent(gameObject, num);
					GonggaoPart.CreateGonggaoContentObj(gameObject);
					gameObject.transform.localPosition = new Vector3(65f, -10f, 0f);
				}
			}
			num++;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GTabControl gTabControl;

	public GButton btnClose;

	private Dictionary<int, List<XElement>> baodianConfig = new Dictionary<int, List<XElement>>();

	private Dictionary<int, string> baodianTabConfig = new Dictionary<int, string>();

	private Dictionary<int, XElement> baodianPromptDict = new Dictionary<int, XElement>();
}
