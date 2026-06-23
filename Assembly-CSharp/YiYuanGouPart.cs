using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class YiYuanGouPart : UserControl
{
	public bool ShuaXin
	{
		get
		{
			return this.m_ShuaXin;
		}
		set
		{
			this.m_ShuaXin = value;
		}
	}

	protected override void OnDestroy()
	{
		this.StopTimer();
		base.OnDestroy();
	}

	protected override void InitializeComponent()
	{
		this.m_Title.text = Global.GetLang("活动");
		this.m_ChaoJiBtn.Text = Global.GetLang("购买");
		this.back.URL = "NetImages/GameRes/Images/VIPJieRiLiBao/back.png";
		this.m_ListBoxOBC = this.m_List.ItemsSource;
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void ChaoJiXmlInit(string XmlData = "Config/OneDollarBuy.xml")
	{
		XElement gameResXml = Global.GetGameResXml(XmlData);
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "OneDollarBuy");
		if (xelement == null)
		{
			return;
		}
		this.m_OneDollarBuy.ID = Global.GetXElementAttributeInt(xelement, "ID");
		this.m_OneDollarBuy.BeginTime = Global.GetXElementAttributeStr(xelement, "BeginTime");
		this.m_OneDollarBuy.FinishTime = Global.GetXElementAttributeStr(xelement, "FinishTime");
		this.m_OneDollarBuy.ZhiGouID = Global.GetXElementAttributeInt(xelement, "ZhiGouID");
		this.m_OneDollarBuy.ChongZhiID = Global.GetXElementAttributeInt(xelement, "ChongZhiID");
		this.m_OneDollarBuy.YuanJia = Global.GetXElementAttributeStr(xelement, "YuanJia");
		this.m_OneDollarBuy.XianJia = Global.GetXElementAttributeInt(xelement, "XianJia");
		this.m_OneDollarBuy.GoodsID1 = Global.GetXElementAttributeStr(xelement, "GoodsID1");
		this.m_OneDollarBuy.GoodsID2 = Global.GetXElementAttributeStr(xelement, "GoodsID2");
		this.m_OneDollarBuy.SinglePurchase = Global.GetXElementAttributeInt(xelement, "SinglePurchase");
	}

	public void XingYunXmlInit(string XmlData = "Config/YiYuanChongZhi.xml")
	{
		XElement gameResXml = Global.GetGameResXml(XmlData);
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "YiYuanChongZhi");
		if (xelement == null)
		{
			return;
		}
		this.m_YiYuanChongZhi.ID = Global.GetXElementAttributeInt(xelement, "ID");
		this.m_YiYuanChongZhi.BeginTime = Global.GetXElementAttributeStr(xelement, "BeginTime");
		this.m_YiYuanChongZhi.FinishTime = Global.GetXElementAttributeStr(xelement, "FinishTime");
		this.m_YiYuanChongZhi.UserList = Global.GetXElementAttributeStr(xelement, "UserList");
		this.m_YiYuanChongZhi.MinZhuanShi = Global.GetXElementAttributeInt(xelement, "MinZhuanShi");
		this.m_YiYuanChongZhi.GoodsID1 = Global.GetXElementAttributeStr(xelement, "GoodsID1");
		this.m_YiYuanChongZhi.GoodsID2 = Global.GetXElementAttributeStr(xelement, "GoodsID2");
	}

	public void SetXingYun(string ret)
	{
		Super.HideNetWaiting();
		this.m_ret = int.Parse(ret);
		this.XingYunXmlInit("Config/YiYuanChongZhi.xml");
		NGUITools.SetActive(this.m_PanelXingYun.gameObject, true);
		NGUITools.SetActive(this.m_PanelChaoJi.gameObject, false);
		this.m_XIngYunTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动期间，幸运首充用户可获得折扣宝卷，进入支付立刻抵扣！")
		});
		string beginTime = this.m_YiYuanChongZhi.BeginTime;
		string finishTime = this.m_YiYuanChongZhi.FinishTime;
		long num = DateTime.Parse(this.m_YiYuanChongZhi.FinishTime).Ticks / 10000L;
		long correctLocalTime = Global.GetCorrectLocalTime();
		this.m_Time = (num - correctLocalTime) / 1000L;
		this.StartTimer(1);
		this.refreshXingYun(ret);
		this.m_XingYunBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_ret == 1)
			{
				GameInstance.Game.LingJiangXingYunYiYuan();
			}
			else if (this.m_ret == 0)
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			}
		};
		if (this.m_ShuaXin)
		{
			return;
		}
		if (this.m_List == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.m_YiYuanChongZhi.GoodsID2))
		{
			this.AddItemXingYun(this.m_YiYuanChongZhi.GoodsID1 + "|" + this.GuoLv(this.m_YiYuanChongZhi.GoodsID2));
		}
		else
		{
			this.AddItemXingYun(this.m_YiYuanChongZhi.GoodsID1);
		}
	}

	public void refreshXingYun(string ret)
	{
		if (int.Parse(ret) == 1)
		{
			this.m_XingYunBtn.Text = Global.GetLang("领取");
			UISprite componentInChildren = this.m_XingYunBtn.GetComponentInChildren<UISprite>();
			if (null != componentInChildren)
			{
				componentInChildren.spriteName = "Btn_on";
			}
			this.m_XingYunBtn.normalSprite = "Btn_on";
			this.m_XingYunBtn.hoverSprite = "Btn_off";
			this.m_XingYunBtn.pressedSprite = "Btn_off";
			this.m_XingYunBtn.disabledSprite = "Btn_off";
		}
		else if (int.Parse(ret) == 2)
		{
			NGUITools.SetActive(this.m_XingYunBtn.gameObject, false);
			NGUITools.SetActive(this.m_Yilingqu.gameObject, true);
		}
		else if (int.Parse(ret) == 0)
		{
			this.m_XingYunBtn.Text = Global.GetLang("充值");
			UISprite componentInChildren2 = this.m_XingYunBtn.GetComponentInChildren<UISprite>();
			if (null != componentInChildren2)
			{
				componentInChildren2.spriteName = "btn_chongzhi_normal";
			}
			this.m_XingYunBtn.normalSprite = "btn_chongzhi_normal";
			this.m_XingYunBtn.hoverSprite = "btn_chongzhi_hover";
			this.m_XingYunBtn.pressedSprite = "btn_chongzhi_hover";
			this.m_XingYunBtn.disabledSprite = "btn_chongzhi_hover";
		}
	}

	public void refreshChaoJi(int num)
	{
		if (num >= this.m_OneDollarBuy.SinglePurchase)
		{
			NGUITools.SetActive(this.m_ChaoJiBtn.gameObject, false);
			NGUITools.SetActive(this.m_ChaoJiYiGouMai.gameObject, true);
		}
		else
		{
			this.m_XingYunBtn.Text = Global.GetLang("购买");
			this.m_ChaoJiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				string productId = string.Empty;
				if (!this.chongzhiInfoDict.ContainsKey(this.m_OneDollarBuy.ChongZhiID.ToString()))
				{
					MUDebug.Log<string>(new string[]
					{
						"配置表错误读取不到ChongZhiID---" + this.m_OneDollarBuy.ChongZhiID.ToString()
					});
					return;
				}
				productId = this.chongzhiInfoDict[this.m_OneDollarBuy.ChongZhiID.ToString()].productId;
				int money = int.Parse(this.chongzhiInfoDict[this.m_OneDollarBuy.ChongZhiID.ToString()].money);
				this.ChongZhi(money, productId, this.m_OneDollarBuy.ZhiGouID);
			};
		}
	}

	public void SetChaoJi(string[] str)
	{
		Super.HideNetWaiting();
		if (this.m_List != null)
		{
			this.m_ListBoxOBC.Clear();
			this.m_ListBoxOBC = this.m_List.ItemsSource;
		}
		this.ChaoJiXmlInit("Config/OneDollarBuy.xml");
		if (str.Length > 0)
		{
			if (int.Parse(str[0]) == this.m_OneDollarBuy.ID)
			{
				if (int.Parse(str[1]) >= this.m_OneDollarBuy.SinglePurchase)
				{
					this.m_ChaoJiBtn.isEnabled = false;
					NGUITools.SetActive(this.m_ChaoJiBtn.gameObject, false);
					NGUITools.SetActive(this.m_ChaoJiYiGouMai.gameObject, true);
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"ID不匹配"
				});
			}
		}
		NGUITools.SetActive(this.m_PanelChaoJi.gameObject, true);
		NGUITools.SetActive(this.m_PanelXingYun.gameObject, false);
		if (!string.IsNullOrEmpty(this.m_OneDollarBuy.GoodsID2))
		{
			this.AddItem(this.m_OneDollarBuy.GoodsID1 + "|" + this.GuoLv(this.m_OneDollarBuy.GoodsID2));
		}
		else
		{
			this.AddItem(this.m_OneDollarBuy.GoodsID1);
		}
		if (this.m_OneDollarBuy.YuanJia.Equals(string.Empty))
		{
			NGUITools.SetActive(this.m_ChaoJiYuanJiaImg.gameObject, false);
			NGUITools.SetActive(this.m_ChaoJiHongXian.gameObject, false);
		}
		else
		{
			this.m_ChaoJiYuanJia.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("{0}元"), this.m_OneDollarBuy.YuanJia)
			});
		}
		this.m_ChaoJiXianJia.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("{0}元"), this.m_OneDollarBuy.XianJia)
		});
		this.m_ChaoJiXianGouNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("个人限购{0}次"), this.m_OneDollarBuy.SinglePurchase)
		});
		this.refreshChaoJi(int.Parse(str[1]));
		string beginTime = this.m_OneDollarBuy.BeginTime;
		string finishTime = this.m_OneDollarBuy.FinishTime;
		long num = DateTime.Parse(this.m_OneDollarBuy.FinishTime).Ticks / 10000L;
		long correctLocalTime = Global.GetCorrectLocalTime();
		this.m_Time = (num - correctLocalTime) / 1000L;
		this.StartTimer(2);
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
			YiYuanGouPart.ChongzhiInfo chongzhiInfo;
			if (this.chongzhiInfoDict.ContainsKey(text))
			{
				chongzhiInfo = this.chongzhiInfoDict[text];
			}
			else
			{
				chongzhiInfo = new YiYuanGouPart.ChongzhiInfo();
			}
			chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
			chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
			chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
			chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
			chongzhiInfo.productId = string.Empty + text;
			if (text == "10000")
			{
				chongzhiInfo.Type = YiYuanGouPart.ChongzhiInfo.ChongZhiType.YueKa;
			}
			else
			{
				chongzhiInfo.Type = YiYuanGouPart.ChongzhiInfo.ChongZhiType.Normal;
			}
			this.chongzhiInfoDict.Add(text, chongzhiInfo);
		}
	}

	private string GuoLv(string goodsId)
	{
		string text = string.Empty;
		string[] array = goodsId.Split(new char[]
		{
			'|'
		});
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		bool flag = true;
		for (int i = 0; i < array.Length; i++)
		{
			string goodStr = array[i].Split(new char[]
			{
				','
			})[0];
			if (!MUJieripartChongzhiKingItem.IsTongGuo(goodStr, roleOcc))
			{
				if (flag)
				{
					text += array[i];
					flag = false;
				}
				else
				{
					text = text + "|" + array[i];
				}
			}
		}
		return text;
	}

	private void AddItemXingYun(string str)
	{
		this.chongzhiInfoDict.Clear();
		this.LoadChongZhiItemConf();
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return;
		}
		string text = string.Empty;
		if (!string.IsNullOrEmpty(this.m_YiYuanChongZhi.GoodsID2))
		{
			text = this.m_YiYuanChongZhi.GoodsID1 + "@" + this.m_YiYuanChongZhi.GoodsID2;
		}
		else
		{
			text = this.m_YiYuanChongZhi.GoodsID1;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.LoadGoodsList(text, this.m_ListBoxOBC);
		}
		ListBox componentInChildren = this.m_List.gameObject.GetComponentInChildren<ListBox>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren);
		}
		GGoodIcon[] componentsInChildren = this.m_List.GetComponentsInChildren<GGoodIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (i >= 4)
			{
				NGUITools.SetActive(componentsInChildren[i].gameObject, false);
				break;
			}
			if (i >= componentsInChildren.Length)
			{
				break;
			}
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			UILabel uilabel = this.m_ListName[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(int.Parse(array2[0]));
			uilabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("{0}"), goodsXmlNodeByID.Title)
			});
			componentsInChildren[i].transform.localPosition = this.m_Vers[i];
		}
	}

	private void AddItem(string str)
	{
		this.chongzhiInfoDict.Clear();
		this.LoadChongZhiItemConf();
		if (this.m_List == null)
		{
			return;
		}
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return;
		}
		string text = string.Empty;
		if (!string.IsNullOrEmpty(this.m_OneDollarBuy.GoodsID2))
		{
			text = this.m_OneDollarBuy.GoodsID1 + "@" + this.m_OneDollarBuy.GoodsID2;
		}
		else
		{
			text = this.m_OneDollarBuy.GoodsID1;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.LoadGoodsList(text, this.m_ListBoxOBC);
		}
		ListBox componentInChildren = this.m_List.gameObject.GetComponentInChildren<ListBox>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren);
		}
		GGoodIcon[] componentsInChildren = this.m_List.GetComponentsInChildren<GGoodIcon>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (i >= 4)
			{
				NGUITools.SetActive(componentsInChildren[i].gameObject, false);
				break;
			}
			if (i >= componentsInChildren.Length)
			{
				break;
			}
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			UILabel uilabel = this.m_ListName[i];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(int.Parse(array2[0]));
			uilabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format(Global.GetLang("{0}"), goodsXmlNodeByID.Title)
			});
			componentsInChildren[i].transform.localPosition = this.m_Vers[i];
		}
	}

	public void StopTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer.Dispose();
			this.m_Timer = null;
		}
		this.Visibility = false;
	}

	public void StartTimer(int key)
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer = null;
		}
		this.m_Timer = new DispatcherTimer("YiYuanGouPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		if (key == 2)
		{
			this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		}
		else if (key == 1)
		{
			this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_TickXingYun);
		}
		this.m_Timer.Start();
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.m_Time > 0L)
		{
			this.m_Time -= 1L;
			DateTime dateTime;
			dateTime..ctor(this.m_Time);
			int num = (int)(this.m_Time / 86400L);
			int num2 = (int)(this.m_Time / 3600L % 24L);
			int num3 = (int)(this.m_Time % 3600L / 60L);
			int num4 = (int)(this.m_Time % 3600L % 60L);
			if (num4 < 0)
			{
				PlayZone.GlobalPlayZone.CloseYiYuanGouWindow();
			}
			if (0 < num)
			{
				this.m_ChaoJiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：{0}天{1}时{2}分{3}秒"), new object[]
					{
						num,
						num2,
						num3,
						num4
					}))
				});
			}
			else if (0 < num2)
			{
				this.m_ChaoJiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：{0}时{1}分{2}秒"), num2, num3, num4))
				});
			}
			else if (0 < num3)
			{
				this.m_ChaoJiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：{0}分{1}秒"), num3, num4))
				});
			}
			else
			{
				this.m_ChaoJiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：{0}秒"), num4))
				});
			}
		}
	}

	private void Timer_TickXingYun(object sender, object e)
	{
		if (this.m_Time > 0L)
		{
			this.m_Time -= 1L;
			DateTime dateTime;
			dateTime..ctor(this.m_Time);
			int num = (int)(this.m_Time / 86400L);
			int num2 = (int)(this.m_Time / 3600L % 24L);
			int num3 = (int)(this.m_Time % 3600L / 60L);
			int num4 = (int)(this.m_Time % 3600L % 60L);
			if (num4 < 0)
			{
				PlayZone.GlobalPlayZone.CloseXingYunYiYuanWindow();
			}
			if (0 < num)
			{
				this.m_XingYunTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余时间：{0}天{1}时{2}分{3}秒"), new object[]
					{
						num,
						num2,
						num3,
						num4
					})
				});
			}
			else if (0 < num2)
			{
				this.m_XingYunTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余时间：{0}时{1}分{2}秒"), num2, num3, num4)
				});
			}
			else if (0 < num3)
			{
				this.m_XingYunTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余时间：{0}分{1}秒"), num3, num4)
				});
			}
			else
			{
				this.m_XingYunTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("剩余时间：{0}秒"), num4)
				});
			}
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			productId + "=" + money
		});
		MUDebug.Log<string>(new string[]
		{
			"越南安卓包一元购活动里的充值"
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
	}

	public GButton m_BtnClose;

	public ListBox m_List;

	private ObservableCollection m_ListBoxOBC;

	public ShowNetImage back;

	public GameObject m_PanelChaoJi;

	public GameObject m_PanelXingYun;

	public UILabel m_ChaoJiTime;

	public UILabel m_Title;

	public UILabel m_ChaoJiYuanJia;

	public UILabel m_ChaoJiXianJia;

	public GButton m_ChaoJiBtn;

	public UILabel m_ChaoJiXianGouNum;

	public UISprite m_ChaoJiHongXian;

	public UISprite m_ChaoJiYuanJiaImg;

	public UISprite m_ChaoJiYiGouMai;

	public List<UILabel> m_ListName = new List<UILabel>();

	public UISprite m_Yilingqu;

	private int m_ret;

	public UILabel m_XingYunTime;

	public GButton m_XingYunBtn;

	public UILabel m_XIngYunTitle;

	private long m_Time = -1L;

	private DispatcherTimer m_Timer;

	public DPSelectedItemEventHandler DPSelectedItem;

	public YiYuanGouPart.OneDollarBuy m_OneDollarBuy = default(YiYuanGouPart.OneDollarBuy);

	public YiYuanGouPart.YiYuanChongZhi m_YiYuanChongZhi = default(YiYuanGouPart.YiYuanChongZhi);

	public Dictionary<string, YiYuanGouPart.ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, YiYuanGouPart.ChongzhiInfo>();

	private Vector3[] m_Vers = new Vector3[]
	{
		new Vector3(-108.51f, 60f, -0.1f),
		new Vector3(36.1f, 60f, -0.1f),
		new Vector3(-108.51f, -95.4f, -0.1f),
		new Vector3(36.1f, -95.4f, -0.1f)
	};

	private bool m_ShuaXin;

	public class ChongzhiInfo
	{
		public string Icon = string.Empty;

		public string money = string.Empty;

		public string zuanshiCount = string.Empty;

		public string productId = string.Empty;

		public string freeDiamond = string.Empty;

		public YiYuanGouPart.ChongzhiInfo.ChongZhiType Type;

		public enum ChongZhiType
		{
			Normal,
			YueKa
		}
	}

	public struct OneDollarBuy
	{
		public int ID;

		public string BeginTime;

		public string FinishTime;

		public int ZhiGouID;

		public int ChongZhiID;

		public string YuanJia;

		public int XianJia;

		public string GoodsID1;

		public string GoodsID2;

		public int SinglePurchase;
	}

	public struct YiYuanChongZhi
	{
		public int ID;

		public string BeginTime;

		public string FinishTime;

		public string UserList;

		public int MinZhuanShi;

		public string GoodsID1;

		public string GoodsID2;
	}
}
