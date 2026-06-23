using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class EveryDayHuodongPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.LoadChongZhiItemConf();
		this.m_BtnhuodongListOBC = this.m_BtnhuodongList.ItemsSource;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void GetEveryDayData()
	{
		Super.ShowNetWaiting(null);
		base.StartCoroutine(this.iGetData());
	}

	protected IEnumerator iGetData()
	{
		GameInstance.Game.GetEveryDayData();
		yield return new WaitForSeconds(0.1f);
		GameInstance.Game.GetCurrentEveryDay();
		yield break;
	}

	public void OnXmlDataResult(JieriXmlData jieriXmlData)
	{
		if (jieriXmlData == null || Global.everyDayXML_Version == jieriXmlData.Version)
		{
			return;
		}
		if (Global.Data.everyDayData == null || Global.everyDayXML_Version != jieriXmlData.Version)
		{
			Global.Data.everyDayData = jieriXmlData;
		}
		Global.everyDayXML_Version = jieriXmlData.Version;
		this.InitEveryDayXml();
	}

	public void SetZhuanXiangData(EveryDayActivityData mEveryDayActivityData = null)
	{
		if (mEveryDayActivityData == null)
		{
			return;
		}
		if (mEveryDayActivityData.EveryDayActInfoList == null)
		{
			return;
		}
		this.EVeryAddBtns(mEveryDayActivityData);
	}

	public void SetLingquState(int ret, int ActID, int LeftPurNum, int ShowNum1, int ShowNum2 = 0)
	{
		BtnObjEVeryDay btnObjEVeryDay = null;
		if (ret == 0)
		{
			if (this.m_ListBtns.TryGetValue(ActID, ref btnObjEVeryDay) && null != btnObjEVeryDay.Pni)
			{
				switch (btnObjEVeryDay.MUIType)
				{
				case UIType.Qianggou:
					if (LeftPurNum <= 0)
					{
						btnObjEVeryDay.IsOK = false;
					}
					break;
				case UIType.Other:
					btnObjEVeryDay.IsOK = false;
					break;
				case UIType.ChongZhi:
					if (LeftPurNum <= 0)
					{
						btnObjEVeryDay.IsOK = false;
					}
					break;
				case UIType.ZhiGou:
					if (LeftPurNum <= 0)
					{
						btnObjEVeryDay.IsOK = false;
					}
					break;
				}
				btnObjEVeryDay.Pni.SetShengyuNum(ActID, LeftPurNum, ShowNum1, ShowNum2);
			}
		}
		else if (ret == -10)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
	}

	private void EVeryAddBtns(EveryDayActivityData mEveryDayActivityData = null)
	{
		Super.HideNetWaiting();
		this.mlistSpecActInfo = mEveryDayActivityData.EveryDayActInfoList;
		for (int i = 0; i < this.mlistSpecActInfo.Count; i++)
		{
			EveryDayActivity everyDayActivity = default(EveryDayActivity);
			if (Global.g_everyDayDic.TryGetValue(this.mlistSpecActInfo[i].ActID, ref everyDayActivity))
			{
				BtnObjEVeryDay btnObjEVeryDay = U3DUtils.NEW<BtnObjEVeryDay>();
				btnObjEVeryDay.name = Global.GetLang(everyDayActivity.Name);
				btnObjEVeryDay.type = everyDayActivity.GoalType;
				btnObjEVeryDay.ActID = this.mlistSpecActInfo[i].ActID;
				btnObjEVeryDay.Label = Global.GetLang(everyDayActivity.Name);
				if (this.mlistSpecActInfo[i].State == 0)
				{
					btnObjEVeryDay.IsOK = true;
				}
				this.m_BtnhuodongListOBC.AddNoUpdate(btnObjEVeryDay);
				UIPanel component = btnObjEVeryDay.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
				UIDragPanelContents component2 = btnObjEVeryDay.GetComponent<UIDragPanelContents>();
				if (null != component2)
				{
					component2.draggablePanel = this.m_BtnhuodongList.gameObject.GetComponentInParent<UIDraggablePanel>();
				}
				if (i == 0)
				{
					this.AddItem(btnObjEVeryDay);
					btnObjEVeryDay.IsActiveItem = true;
					Super.HideNetWaiting();
				}
				if (!this.m_ListBtns.ContainsKey(this.mlistSpecActInfo[i].ActID))
				{
					this.m_ListBtns.Add(this.mlistSpecActInfo[i].ActID, btnObjEVeryDay);
				}
			}
		}
		this.m_BtnhuodongList.repositionNow = true;
		this.m_BtnhuodongList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.BtnhuodongListChange);
	}

	private void AddItem(BtnObjEVeryDay Btnitem)
	{
		for (int i = 0; i < this.mlistSpecActInfo.Count; i++)
		{
			EveryDayActInfo mSpecActInfo = this.mlistSpecActInfo[i];
			if (mSpecActInfo != null)
			{
				if (mSpecActInfo.ActID == Btnitem.ActID)
				{
					EveryDayActivity mEveryDayActivity = default(EveryDayActivity);
					string text = string.Empty;
					string text2 = string.Empty;
					string effect = string.Empty;
					if (Global.g_everyDayDic.TryGetValue(Btnitem.ActID, ref mEveryDayActivity))
					{
						EveryDayHuodongItem everyDayHuodongItem = U3DUtils.NEW<EveryDayHuodongItem>();
						U3DUtils.AddChild(this.m_ItemPosition, everyDayHuodongItem.gameObject, false);
						Btnitem.Pni = everyDayHuodongItem;
						everyDayHuodongItem.transform.localPosition = Vector3.zero;
						everyDayHuodongItem.transform.localScale = Vector3.one;
						Btnitem.m_Button.Text = Btnitem.name;
						int num = 0;
						if (!string.IsNullOrEmpty(mEveryDayActivity.GoodsTwo))
						{
							text = mEveryDayActivity.GoodsOne + "@" + mEveryDayActivity.GoodsTwo;
							string[] array = StringUtil.trim(mEveryDayActivity.GoodsOne).Split(new char[]
							{
								'|'
							});
							string[] array2 = StringUtil.trim(mEveryDayActivity.GoodsTwo).Split(new char[]
							{
								'|'
							});
							num += array.Length;
							num += array2.Length;
						}
						else
						{
							text = mEveryDayActivity.GoodsOne;
							string[] array3 = text.Split(new char[]
							{
								'|'
							});
							num += array3.Length;
						}
						everyDayHuodongItem.SetListBox(num);
						text2 = mEveryDayActivity.GoodsThr;
						effect = mEveryDayActivity.EffectiveTime;
						ZhuanXiangType goalType = (ZhuanXiangType)mEveryDayActivity.GoalType;
						if (goalType != ZhuanXiangType.XianshiQianggou)
						{
							if (goalType != ZhuanXiangType.ChongzhiDuihuan)
							{
								if (goalType != ZhuanXiangType.ChaoJiZhiGou)
								{
									Super.LoadGoodsList(text, everyDayHuodongItem.jiangliListBoxOBC);
									Super.LoadOtherGoodsList(text2, everyDayHuodongItem.jiangliListBoxOBC, effect);
									Btnitem.MUIType = UIType.Other;
									everyDayHuodongItem.InitItemData(UIType.Other, (ZhuanXiangType)mEveryDayActivity.GoalType, mSpecActInfo, mEveryDayActivity, string.Empty);
								}
								else
								{
									if (!string.IsNullOrEmpty(text))
									{
										Super.LoadGoodsList(text, everyDayHuodongItem.qianggouListBoxOBC);
									}
									else if (!string.IsNullOrEmpty(text2))
									{
										Super.LoadOtherGoodsList(text2, everyDayHuodongItem.qianggouListBoxOBC, effect);
									}
									Btnitem.MUIType = UIType.ZhiGou;
									string price = string.Empty;
									if (mEveryDayActivity.Price.Split(new char[]
									{
										'|'
									}).Length == 3 && this.chongzhiInfoDict.ContainsKey(mEveryDayActivity.Price.Split(new char[]
									{
										'|'
									})[1]))
									{
										price = this.chongzhiInfoDict[mEveryDayActivity.Price.Split(new char[]
										{
											'|'
										})[1]].money;
									}
									everyDayHuodongItem.InitItemData(UIType.ZhiGou, (ZhuanXiangType)mEveryDayActivity.GoalType, mSpecActInfo, mEveryDayActivity, price);
								}
							}
							else
							{
								Super.LoadGoodsList(text, everyDayHuodongItem.jiangliListBoxOBC);
								Super.LoadOtherGoodsList(text2, everyDayHuodongItem.jiangliListBoxOBC, effect);
								Btnitem.MUIType = UIType.ChongZhi;
								everyDayHuodongItem.InitItemData(UIType.ChongZhi, (ZhuanXiangType)mEveryDayActivity.GoalType, mSpecActInfo, mEveryDayActivity, string.Empty);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(text))
							{
								Super.LoadGoodsList(text, everyDayHuodongItem.qianggouListBoxOBC);
							}
							else if (!string.IsNullOrEmpty(text2))
							{
								Super.LoadOtherGoodsList(text2, everyDayHuodongItem.qianggouListBoxOBC, effect);
							}
							Btnitem.MUIType = UIType.Qianggou;
							everyDayHuodongItem.InitItemData(UIType.Qianggou, (ZhuanXiangType)mEveryDayActivity.GoalType, mSpecActInfo, mEveryDayActivity, string.Empty);
						}
						everyDayHuodongItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
						{
							if (this.m_ListBtns[e.Flag].type == 14)
							{
								string productId = string.Empty;
								if (this.chongzhiInfoDict.ContainsKey(mEveryDayActivity.Price.Split(new char[]
								{
									'|'
								})[1]))
								{
									productId = this.chongzhiInfoDict[e.Type.ToString()].productId;
									int money = int.Parse(this.chongzhiInfoDict[e.Type.ToString()].money);
									this.ChongZhi(money, productId, e.IDType);
									return;
								}
								Super.HintMainText(Global.GetLang("对不起网络错误"), 10, 3);
								return;
							}
							else
							{
								if (this.m_ListBtns[e.Flag].type == 2 && mSpecActInfo.ShowNum < int.Parse(mEveryDayActivity.Price.Split(new char[]
								{
									'|'
								})[0]))
								{
									Super.HintMainText(Global.GetLang("对不起，每日积分不足"), 10, 3);
									return;
								}
								this.DPSelectedItem(s, e);
								return;
							}
						};
						UIPanel component = everyDayHuodongItem.gameObject.GetComponent<UIPanel>();
						if (component)
						{
							Object.Destroy(component);
						}
					}
				}
			}
		}
	}

	private void BtnhuodongListChange(object sender, object e)
	{
		if (this.m_BtnhuodongList.SelectedIndex >= 0 && this.m_BtnhuodongList.SelectedItem != null)
		{
			BtnObjEVeryDay component = this.m_BtnhuodongList.SelectedItem.GetComponent<BtnObjEVeryDay>();
			this.SetUIState(component.ActID);
		}
	}

	private void SetUIState(int actID)
	{
		foreach (KeyValuePair<int, BtnObjEVeryDay> keyValuePair in this.m_ListBtns)
		{
			if (actID == keyValuePair.Key)
			{
				Dictionary<int, BtnObjEVeryDay> listBtns = this.m_ListBtns;
				Dictionary<int, BtnObjEVeryDay>.Enumerator enumerator;
				KeyValuePair<int, BtnObjEVeryDay> keyValuePair2 = enumerator.Current;
				if (listBtns[keyValuePair2.Key].Pni == null)
				{
					Dictionary<int, BtnObjEVeryDay> listBtns2 = this.m_ListBtns;
					KeyValuePair<int, BtnObjEVeryDay> keyValuePair3 = enumerator.Current;
					this.AddItem(listBtns2[keyValuePair3.Key]);
				}
				Dictionary<int, BtnObjEVeryDay> listBtns3 = this.m_ListBtns;
				KeyValuePair<int, BtnObjEVeryDay> keyValuePair4 = enumerator.Current;
				listBtns3[keyValuePair4.Key].IsActiveItem = true;
			}
			else
			{
				Dictionary<int, BtnObjEVeryDay> listBtns4 = this.m_ListBtns;
				Dictionary<int, BtnObjEVeryDay>.Enumerator enumerator;
				KeyValuePair<int, BtnObjEVeryDay> keyValuePair5 = enumerator.Current;
				listBtns4[keyValuePair5.Key].IsActiveItem = false;
			}
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"每日活动充值productID=",
				productId,
				";money=",
				money
			})
		});
		MUDebug.Log<string>(new string[]
		{
			"YN_Android每日活动里的充值"
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
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
			EveryDayHuodongPart.ChongzhiInfo chongzhiInfo;
			if (this.chongzhiInfoDict.ContainsKey(text))
			{
				chongzhiInfo = this.chongzhiInfoDict[text];
			}
			else
			{
				chongzhiInfo = new EveryDayHuodongPart.ChongzhiInfo();
			}
			chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
			chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
			chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
			chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
			chongzhiInfo.productId = string.Empty + text;
			if (text == "10000")
			{
				chongzhiInfo.Type = EveryDayHuodongPart.ChongzhiInfo.ChongZhiType.YueKa;
			}
			else
			{
				chongzhiInfo.Type = EveryDayHuodongPart.ChongzhiInfo.ChongZhiType.Normal;
			}
			chongzhiInfo.productId = Global.GetXElementAttributeStr(list[i], "productIdAn");
			this.chongzhiInfoDict.Add(text, chongzhiInfo);
		}
	}

	private void InitEveryDayXml()
	{
		if (Global.Data.everyDayData == null)
		{
			return;
		}
		Global.g_everyDayDic.Clear();
		XElement xelement = XElement.Parse(Global.Data.everyDayData.XmlList[0]);
		if (xelement == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"未读取到配置表EveryDayActivity.xml"
			});
			return;
		}
		if (Global.g_everyDayDic == null)
		{
			Global.g_everyDayDic = new Dictionary<int, EveryDayActivity>();
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "EveryDayActivity");
		if (xelementList.Count <= 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"未读取到配置表字段EveryDayActivity"
			});
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			EveryDayActivity everyDayActivity = default(EveryDayActivity);
			everyDayActivity.ActivityID = Global.GetXElementAttributeInt(xelementList[i], "ActivityID");
			everyDayActivity.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			everyDayActivity.GoalType = Global.GetXElementAttributeInt(xelementList[i], "GoalType");
			everyDayActivity.GoalNum = Global.GetXElementAttributeStr(xelementList[i], "GoalNum");
			everyDayActivity.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			everyDayActivity.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
			everyDayActivity.GoodsThr = Global.GetXElementAttributeStr(xelementList[i], "GoodsThr");
			everyDayActivity.EffectiveTime = Global.GetXElementAttributeStr(xelementList[i], "EffectiveTime");
			everyDayActivity.Price = Global.GetXElementAttributeStr(xelementList[i], "Price");
			everyDayActivity.PurchaseNum = Global.GetXElementAttributeInt(xelementList[i], "PurchaseNum");
			if (!Global.g_everyDayDic.ContainsKey(everyDayActivity.ActivityID))
			{
				Global.g_everyDayDic.Add(everyDayActivity.ActivityID, everyDayActivity);
			}
			else
			{
				Global.g_everyDayDic[everyDayActivity.ActivityID] = everyDayActivity;
			}
		}
	}

	public ListBox m_BtnhuodongList;

	public GButton btnClose;

	public GameObject m_ItemPosition;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection m_BtnhuodongListOBC;

	private Dictionary<int, BtnObjEVeryDay> m_ListBtns = new Dictionary<int, BtnObjEVeryDay>();

	private Dictionary<string, EveryDayHuodongPart.ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, EveryDayHuodongPart.ChongzhiInfo>();

	private List<EveryDayActInfo> mlistSpecActInfo = new List<EveryDayActInfo>();

	public class ChongzhiInfo
	{
		public string Icon = string.Empty;

		public string money = string.Empty;

		public string zuanshiCount = string.Empty;

		public string productId = string.Empty;

		public string freeDiamond = string.Empty;

		public EveryDayHuodongPart.ChongzhiInfo.ChongZhiType Type;

		public enum ChongZhiType
		{
			Normal,
			YueKa
		}
	}
}
