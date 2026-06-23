using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuZhiGouPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ListOBC = this.m_ListView.ItemsSource;
		this.m_DraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.OnDragFinished);
		this.LoadChongZhiItemConf();
	}

	private void AddXmlThemeActivityZhiGou(string XmlList)
	{
		this.m_DicThemeActivityZhiGou.Clear();
		XElement xelement = XElement.Parse(XmlList);
		List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityZhiGou");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ThemeActivityZhiGou themeActivityZhiGou = new ThemeActivityZhiGou();
			themeActivityZhiGou.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityZhiGou.Day = Global.GetXElementAttributeStr(xelementList[i], "Day");
			themeActivityZhiGou.ZhiGouID = Global.GetXElementAttributeInt(xelementList[i], "ZhiGouID");
			themeActivityZhiGou.ChongZhiID = Global.GetXElementAttributeInt(xelementList[i], "ChongZhiID");
			themeActivityZhiGou.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			themeActivityZhiGou.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
			themeActivityZhiGou.SinglePurchase = Global.GetXElementAttributeInt(xelementList[i], "SinglePurchase");
			themeActivityZhiGou.TitlePic = Global.GetXElementAttributeStr(xelementList[i], "TitlePic");
			themeActivityZhiGou.Background = Global.GetXElementAttributeInt(xelementList[i], "Background");
			if (!this.m_DicThemeActivityZhiGou.ContainsKey(themeActivityZhiGou.ID))
			{
				this.m_DicThemeActivityZhiGou.Add(themeActivityZhiGou.ID, themeActivityZhiGou);
			}
		}
	}

	public void SetData(string XmlList)
	{
		this.AddXmlThemeActivityZhiGou(XmlList);
	}

	public void AddList(Dictionary<int, int> dic)
	{
		if (this.m_ListOBC.Count > 0)
		{
			for (int i = 0; i < this.m_ListOBC.Count; i++)
			{
				ZhuTiFuZhiGouItem component = this.m_ListOBC.GetAt(i).GetComponent<ZhuTiFuZhiGouItem>();
				if (component != null)
				{
					component.Number = component.Numbers - dic[component.ID];
					if (component.Numbers - dic[component.ID] > 0)
					{
						component.m_Btn.isEnabled = true;
					}
					else
					{
						component.m_Btn.isEnabled = false;
						component.m_Btn.target.spriteName = "yilingqu";
						component.m_Btn.target.transform.localScale = new Vector3(90f, 66f, 1f);
						component.m_Btn.Text = string.Empty;
					}
				}
			}
		}
		else
		{
			int num = 0;
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime serverStartTime = Global.GetServerStartTime();
			DateTime dateTime;
			dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, 1, 1, 1);
			DateTime dateTime2;
			dateTime2..ctor(serverStartTime.Year, serverStartTime.Month, serverStartTime.Day, 1, 1, 1);
			int num2 = (dateTime - dateTime2).Days + 1;
			Dictionary<int, ThemeActivityZhiGou>.Enumerator enumerator = this.m_DicThemeActivityZhiGou.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num3 = num2;
				KeyValuePair<int, ThemeActivityZhiGou> keyValuePair = enumerator.Current;
				if (num3 >= keyValuePair.Value.Day.Split(new char[]
				{
					','
				})[0].SafeToInt32(0))
				{
					int num4 = num2;
					KeyValuePair<int, ThemeActivityZhiGou> keyValuePair2 = enumerator.Current;
					if (num4 <= keyValuePair2.Value.Day.Split(new char[]
					{
						','
					})[1].SafeToInt32(0))
					{
						num++;
						ZhuTiFuZhiGouItem item = U3DUtils.NEW<ZhuTiFuZhiGouItem>();
						this.m_ListOBC.AddNoUpdate(item);
						ZhuTiFuZhiGouItem item7 = item;
						KeyValuePair<int, ThemeActivityZhiGou> keyValuePair3 = enumerator.Current;
						item7.ID = keyValuePair3.Value.ID;
						Dictionary<string, ZhuTiFuZhiGouPart.ChongzhiInfo> dictionary = this.chongzhiInfoDict;
						KeyValuePair<int, ThemeActivityZhiGou> keyValuePair4 = enumerator.Current;
						if (dictionary.ContainsKey(keyValuePair4.Value.ChongZhiID.ToString()))
						{
							ZhuTiFuZhiGouItem item2 = item;
							Dictionary<string, ZhuTiFuZhiGouPart.ChongzhiInfo> dictionary2 = this.chongzhiInfoDict;
							KeyValuePair<int, ThemeActivityZhiGou> keyValuePair5 = enumerator.Current;
							item2.JiaGe = int.Parse(dictionary2[keyValuePair5.Value.ChongZhiID.ToString()].money);
						}
						ZhuTiFuZhiGouItem item3 = item;
						KeyValuePair<int, ThemeActivityZhiGou> keyValuePair6 = enumerator.Current;
						item3.Time = keyValuePair6.Value.Day.Split(new char[]
						{
							','
						})[1].SafeToInt32(0) - num2 + 1;
						string goodsIDs = string.Empty;
						KeyValuePair<int, ThemeActivityZhiGou> keyValuePair7 = enumerator.Current;
						if (!string.IsNullOrEmpty(keyValuePair7.Value.GoodsTwo))
						{
							KeyValuePair<int, ThemeActivityZhiGou> keyValuePair8 = enumerator.Current;
							string goodsOne = keyValuePair8.Value.GoodsOne;
							string text = "@";
							KeyValuePair<int, ThemeActivityZhiGou> keyValuePair9 = enumerator.Current;
							goodsIDs = goodsOne + text + keyValuePair9.Value.GoodsTwo;
						}
						else
						{
							KeyValuePair<int, ThemeActivityZhiGou> keyValuePair10 = enumerator.Current;
							goodsIDs = keyValuePair10.Value.GoodsOne;
						}
						ObservableCollection itemsSource = item.m_ListGoods.ItemsSource;
						Super.LoadGoodsList(goodsIDs, itemsSource);
						GGoodIcon[] componentsInChildren = item.m_ListGoods.GetComponentsInChildren<GGoodIcon>();
						if (componentsInChildren != null)
						{
							for (int j = 0; j < componentsInChildren.Length; j++)
							{
								if (componentsInChildren[j].GetComponent<UIPanel>() != null)
								{
									Object.Destroy(componentsInChildren[j].GetComponent<UIPanel>());
								}
							}
						}
						if (item.GetComponent<UIPanel>() != null)
						{
							Object.Destroy(item.GetComponent<UIPanel>());
						}
						if (dic != null)
						{
							KeyValuePair<int, ThemeActivityZhiGou> keyValuePair11 = enumerator.Current;
							if (dic.ContainsKey(keyValuePair11.Value.ID))
							{
								KeyValuePair<int, ThemeActivityZhiGou> keyValuePair12 = enumerator.Current;
								int singlePurchase = keyValuePair12.Value.SinglePurchase;
								KeyValuePair<int, ThemeActivityZhiGou> keyValuePair13 = enumerator.Current;
								int num5 = singlePurchase - dic[keyValuePair13.Value.ID];
								ZhuTiFuZhiGouItem item4 = item;
								KeyValuePair<int, ThemeActivityZhiGou> keyValuePair14 = enumerator.Current;
								item4.Numbers = keyValuePair14.Value.SinglePurchase;
								item.Number = num5;
								if (num5 > 0)
								{
									ZhuTiFuZhiGouItem item5 = item;
									KeyValuePair<int, ThemeActivityZhiGou> keyValuePair15 = enumerator.Current;
									item5.ChongZhiID = keyValuePair15.Value.ChongZhiID.ToString();
									ZhuTiFuZhiGouItem item6 = item;
									KeyValuePair<int, ThemeActivityZhiGou> keyValuePair16 = enumerator.Current;
									item6.ZhiGouID = keyValuePair16.Value.ZhiGouID;
									item.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
									{
										string productId = string.Empty;
										if (this.chongzhiInfoDict.ContainsKey(item.ChongZhiID))
										{
											productId = this.chongzhiInfoDict[item.ChongZhiID].productId;
											int money = int.Parse(this.chongzhiInfoDict[item.ChongZhiID].money);
											this.ChongZhi(money, productId, item.ZhiGouID);
											return;
										}
										Super.HintMainText(Global.GetLang("对不起网络错误"), 10, 3);
									};
								}
								else
								{
									item.m_Btn.isEnabled = false;
									item.m_Btn.target.spriteName = "yilingqu";
									item.m_Btn.target.transform.localScale = new Vector3(90f, 66f, 1f);
									item.m_Btn.Text = string.Empty;
								}
							}
						}
					}
				}
			}
			this.AddPage(num);
			this.RefreshPage(0);
		}
	}

	public void OnDragFinished()
	{
		int num = 792;
		Vector3 localPosition = this.m_DraggablePanel.transform.localPosition;
		if (Math.Abs(Math.Abs(localPosition.x) - (float)(num * this.CurrentSelectedPage)) > 80f)
		{
			if (localPosition.x > (float)(-(float)num * this.CurrentSelectedPage))
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
				if (this.CurrentSelectedPage >= this.m_ListSpPage.Count)
				{
					this.CurrentSelectedPage = this.m_ListSpPage.Count;
					return;
				}
			}
			this.m_SpringPanel.target.x = (float)(-(float)num * this.CurrentSelectedPage);
			this.m_SpringPanel.enabled = true;
			this.RefreshPage(this.CurrentSelectedPage);
		}
	}

	private void AddPage(int count)
	{
		float num = 50f;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = U3DUtils.Clone(this.m_ListPage.gameObject, this.m_GamePage.gameObject);
			this.m_ListPage.AddNoUpdate(gameObject);
			gameObject.SetActive(true);
			float num2 = (float)i * num - (float)(count - 1) * num / 2f;
			gameObject.transform.localPosition = new Vector3(num2, 0f, -0.2f);
			if (gameObject.GetComponent<UISprite>() != null)
			{
				this.m_ListSpPage.Add(gameObject.GetComponent<UISprite>());
			}
		}
		this.m_GamePage.SetActive(false);
	}

	private void RefreshPage(int key)
	{
		for (int i = 0; i < this.m_ListSpPage.Count; i++)
		{
			if (key != i)
			{
				this.m_ListSpPage[i].spriteName = "Page1";
			}
			else
			{
				this.m_ListSpPage[i].spriteName = "Page0";
			}
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		PlatSDKMgr.Pay(money, productId, zhiZhouId);
	}

	public void LoadChongZhiItemConf()
	{
		string text = string.Empty;
		string rechargeItemCfgTypeByPlatform = Global.GetRechargeItemCfgTypeByPlatform();
		XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
		List<XElement> list = new List<XElement>();
		foreach (XElement xelement in gameResXml.Elements())
		{
			if (xelement.Attribute("TypeID").Value.ToString() == rechargeItemCfgTypeByPlatform)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					list.Add(xelement2);
				}
				break;
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			text = Global.GetXElementAttributeStr(list[i], "ID");
			ZhuTiFuZhiGouPart.ChongzhiInfo chongzhiInfo;
			if (this.chongzhiInfoDict.ContainsKey(text))
			{
				chongzhiInfo = this.chongzhiInfoDict[text];
			}
			else
			{
				chongzhiInfo = new ZhuTiFuZhiGouPart.ChongzhiInfo();
			}
			chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
			chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
			chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
			chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
			chongzhiInfo.productId = string.Empty + text;
			if (text == "10000")
			{
				chongzhiInfo.Type = ZhuTiFuZhiGouPart.ChongzhiInfo.ChongZhiType.YueKa;
			}
			else
			{
				chongzhiInfo.Type = ZhuTiFuZhiGouPart.ChongzhiInfo.ChongZhiType.Normal;
			}
			this.chongzhiInfoDict.Add(text, chongzhiInfo);
		}
	}

	public ListBox m_ListPage;

	public ListBox m_ListView;

	public GameObject m_GamePage;

	public UIDraggablePanel m_DraggablePanel;

	public UILabel m_Time;

	public SpringPanel m_SpringPanel;

	private List<UISprite> m_ListSpPage = new List<UISprite>();

	private ObservableCollection m_ListOBC;

	private int CurrentSelectedPage;

	private Dictionary<int, ThemeActivityZhiGou> m_DicThemeActivityZhiGou = new Dictionary<int, ThemeActivityZhiGou>();

	private Dictionary<string, ZhuTiFuZhiGouPart.ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, ZhuTiFuZhiGouPart.ChongzhiInfo>();

	public class ChongzhiInfo
	{
		public string Icon = string.Empty;

		public string money = string.Empty;

		public string zuanshiCount = string.Empty;

		public string productId = string.Empty;

		public string freeDiamond = string.Empty;

		public ZhuTiFuZhiGouPart.ChongzhiInfo.ChongZhiType Type;

		public enum ChongZhiType
		{
			Normal,
			YueKa
		}
	}
}
