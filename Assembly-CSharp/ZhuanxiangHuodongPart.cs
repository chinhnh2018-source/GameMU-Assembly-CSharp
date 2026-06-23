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

public class ZhuanxiangHuodongPart : UserControl
{
	public void CloseZhuanXiangHuodongPart()
	{
		if (this.m_ListBtns != null)
		{
			foreach (KeyValuePair<int, BtnObjZhuanXiang> keyValuePair in this.m_ListBtns)
			{
				keyValuePair.Value.m_ZhuanxiangHuodongItem = null;
				Dictionary<int, BtnObjZhuanXiang>.Enumerator enumerator;
				KeyValuePair<int, BtnObjZhuanXiang> keyValuePair2 = enumerator.Current;
				keyValuePair2.Value.Clear();
			}
			this.m_ListBtns.Clear();
			this.m_ListBtns = null;
		}
	}

	protected override void InitializeComponent()
	{
		this.LoadChongZhiItemConf();
		base.InitializeComponent();
		this.m_BtnhuodongListOBC = this.m_BtnhuodongList.ItemsSource;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void GetZhuanXiangData()
	{
		Super.ShowNetWaiting(null);
		base.StartCoroutine(this.iGetData());
	}

	protected IEnumerator iGetData()
	{
		GameInstance.Game.GetZhuanXiangData();
		yield return new WaitForSeconds(0.1f);
		GameInstance.Game.GetCurrentZhuanXiang();
		yield break;
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
			ZhuanxiangHuodongPart.ChongzhiInfo chongzhiInfo;
			if (this.chongzhiInfoDict.ContainsKey(text))
			{
				chongzhiInfo = this.chongzhiInfoDict[text];
			}
			else
			{
				chongzhiInfo = new ZhuanxiangHuodongPart.ChongzhiInfo();
			}
			chongzhiInfo.Icon = Global.GetXElementAttributeStr(list[i], "Icon");
			chongzhiInfo.money = Global.GetXElementAttributeStr(list[i], "RMB");
			chongzhiInfo.zuanshiCount = Global.GetXElementAttributeStr(list[i], "ZuanShi");
			chongzhiInfo.freeDiamond = Global.GetXElementAttributeStr(list[i], "FirstBindZuanShi");
			chongzhiInfo.productId = string.Empty + text;
			if (text == "10000")
			{
				chongzhiInfo.Type = ZhuanxiangHuodongPart.ChongzhiInfo.ChongZhiType.YueKa;
				if (Context.IsHaiwai)
				{
					chongzhiInfo.Type = ZhuanxiangHuodongPart.ChongzhiInfo.ChongZhiType.Normal;
				}
			}
			else
			{
				chongzhiInfo.Type = ZhuanxiangHuodongPart.ChongzhiInfo.ChongZhiType.Normal;
			}
			if (Context.IsHaiwai)
			{
				chongzhiInfo.productId = Global.GetXElementAttributeStr(list[i], "productIdAn");
			}
			this.chongzhiInfoDict.Add(text, chongzhiInfo);
		}
	}

	public void OnXmlDataResult(JieriXmlData jieriXmlData)
	{
		if (jieriXmlData == null || Global.ZhuanxiangXML_Version == jieriXmlData.Version)
		{
			return;
		}
		if (Global.Data.ZhuanXiangData == null || Global.ZhuanxiangXML_Version != jieriXmlData.Version)
		{
			Global.Data.ZhuanXiangData = jieriXmlData;
		}
		Global.ZhuanxiangXML_Version = jieriXmlData.Version;
		this.InitXMLData();
	}

	public void SetZhuanXiangData(SpecialActivityData mSpecialActivityData = null)
	{
		if (mSpecialActivityData == null)
		{
			return;
		}
		if (mSpecialActivityData.SpecActInfoList == null)
		{
			return;
		}
		base.StartCoroutine<bool>(this.SetZhuanXiangData2(mSpecialActivityData));
	}

	public IEnumerator SetZhuanXiangData2(SpecialActivityData mSpecialActivityData = null)
	{
		this.mGroupID = mSpecialActivityData.GroupID;
		this.mlistSpecActInfo = mSpecialActivityData.SpecActInfoList;
		SpecActInfo mSpecActInfo = null;
		SpecialActivityXML mSpecialActivityXML = default(SpecialActivityXML);
		string goodsid = string.Empty;
		string goodsThr = string.Empty;
		string effectiveTime = string.Empty;
		for (int i = 0; i < this.mlistSpecActInfo.Count; i++)
		{
			mSpecActInfo = this.mlistSpecActInfo[i];
			if (mSpecActInfo != null)
			{
				if (i % 5 == 0 && i != 0)
				{
					yield return null;
				}
				if (Global.g_ZhuanXiangDic.TryGetValue(mSpecActInfo.ActID, ref mSpecialActivityXML))
				{
					BtnObjZhuanXiang Btnitem = U3DUtils.NEW<BtnObjZhuanXiang>();
					Btnitem.name = Global.GetLang(mSpecialActivityXML.Name);
					Btnitem.type = mSpecialActivityXML.Type;
					Btnitem.ActID = mSpecActInfo.ActID;
					Btnitem.Label = Global.GetLang(mSpecialActivityXML.Name);
					if (mSpecActInfo.State == 0)
					{
						Btnitem.IsOK = true;
					}
					this.m_BtnhuodongListOBC.AddNoUpdate(Btnitem);
					UIPanel p = Btnitem.GetComponent<UIPanel>();
					if (null != p)
					{
						Object.Destroy(p);
					}
					UIDragPanelContents pl = Btnitem.GetComponent<UIDragPanelContents>();
					if (null != pl)
					{
						pl.draggablePanel = this.m_BtnhuodongList.gameObject.GetComponentInParent<UIDraggablePanel>();
					}
					if (i == 0)
					{
						this.AddItem(Btnitem);
						Btnitem.IsActiveItem = true;
						Super.HideNetWaiting();
					}
					if (!this.m_ListBtns.ContainsKey(mSpecActInfo.ActID))
					{
						this.m_ListBtns.Add(mSpecActInfo.ActID, Btnitem);
					}
				}
			}
		}
		this.m_BtnhuodongList.repositionNow = true;
		this.m_BtnhuodongList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.BtnhuodongListChange);
		yield return null;
		yield break;
	}

	public void SetLingquState(int ret, int ActID, int LeftPurNum, int ShowNum1, int ShowNum2 = 0)
	{
		BtnObjZhuanXiang btnObjZhuanXiang = null;
		if (ret == 0)
		{
			if (this.m_ListBtns.TryGetValue(ActID, ref btnObjZhuanXiang) && null != btnObjZhuanXiang.Pni)
			{
				switch (btnObjZhuanXiang.MUIType)
				{
				case UIType.Qianggou:
					if (LeftPurNum <= 0)
					{
						btnObjZhuanXiang.IsOK = false;
					}
					break;
				case UIType.Other:
					btnObjZhuanXiang.IsOK = false;
					break;
				case UIType.ChongZhi:
					if (LeftPurNum <= 0)
					{
						btnObjZhuanXiang.IsOK = false;
					}
					foreach (BtnObjZhuanXiang btnObjZhuanXiang2 in this.m_ListBtns.Values)
					{
						if (btnObjZhuanXiang2 != null && btnObjZhuanXiang2.MUIType == UIType.ChongZhi)
						{
							btnObjZhuanXiang2.m_ZhuanxiangHuodongItem.ChongZhiJiFen = ShowNum1;
						}
					}
					break;
				case UIType.ZhiGou:
					if (LeftPurNum <= 0)
					{
						btnObjZhuanXiang.IsOK = false;
					}
					break;
				}
				btnObjZhuanXiang.Pni.SetShengyuNum(ActID, LeftPurNum, ShowNum1, ShowNum2);
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

	private void InitXMLData()
	{
		if (Global.Data.ZhuanXiangData == null)
		{
			return;
		}
		Global.g_ZhuanXiangTime.Clear();
		Global.g_ZhuanXiangDic.Clear();
		XElement xelement = XElement.Parse(Global.Data.ZhuanXiangData.XmlList[0]);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Time");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			if (xelement2 == null)
			{
				return;
			}
			SpecialActivityTime specialActivityTime = default(SpecialActivityTime);
			specialActivityTime.GroupID = Global.GetXElementAttributeInt(xelement2, "GroupID");
			specialActivityTime.ServerOpenFromDate = Global.GetXElementAttributeStr(xelement2, "ServerOpenFromDate");
			specialActivityTime.ServerOpenToDate = Global.GetXElementAttributeStr(xelement2, "ServerOpenToDate");
			specialActivityTime.FromDate = Global.GetXElementAttributeStr(xelement2, "FromDate");
			specialActivityTime.ToDate = Global.GetXElementAttributeStr(xelement2, "ToDate");
			if (!Global.g_ZhuanXiangTime.ContainsKey(specialActivityTime.GroupID))
			{
				Global.g_ZhuanXiangTime.Add(specialActivityTime.GroupID, specialActivityTime);
			}
		}
		XElement xelement3 = XElement.Parse(Global.Data.ZhuanXiangData.XmlList[1]);
		if (xelement3 == null)
		{
			return;
		}
		List<XElement> xelementList2 = Global.GetXElementList(xelement3, "Activity");
		for (int j = 0; j < xelementList2.Count; j++)
		{
			XElement xelement4 = xelementList2[j];
			if (xelement4 == null)
			{
				return;
			}
			SpecialActivityXML specialActivityXML = default(SpecialActivityXML);
			specialActivityXML.ID = Global.GetXElementAttributeInt(xelement4, "ID");
			specialActivityXML.GroupID = Global.GetXElementAttributeInt(xelement4, "GroupID");
			specialActivityXML.Name = Global.GetXElementAttributeStr(xelement4, "Name");
			specialActivityXML.Day = Global.GetXElementAttributeStr(xelement4, "Day");
			specialActivityXML.NeedLevel = Global.GetXElementAttributeStr(xelement4, "NeedLevel");
			specialActivityXML.NeedVIP = Global.GetXElementAttributeStr(xelement4, "NeedVIP");
			specialActivityXML.NeedChongZhi = Global.GetXElementAttributeStr(xelement4, "NeedChongZhi");
			specialActivityXML.NeedWing = Global.GetXElementAttributeStr(xelement4, "NeedWing");
			specialActivityXML.NeedChengJiu = Global.GetXElementAttributeStr(xelement4, "NeedChengJiu");
			specialActivityXML.NeedJunXian = Global.GetXElementAttributeStr(xelement4, "NeedJunXian");
			specialActivityXML.NeedMerlin = Global.GetXElementAttributeStr(xelement4, "NeedMerlin");
			specialActivityXML.NeedShengWu = Global.GetXElementAttributeStr(xelement4, "NeedShengWu");
			specialActivityXML.NeedRing = Global.GetXElementAttributeStr(xelement4, "NeedRing");
			specialActivityXML.NeedShouHuShen = Global.GetXElementAttributeStr(xelement4, "NeedShouHuShen");
			specialActivityXML.Type = Global.GetXElementAttributeInt(xelement4, "Type");
			specialActivityXML.Goal = Global.GetXElementAttributeStr(xelement4, "Goal");
			specialActivityXML.GoodsOne = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
			specialActivityXML.GoodsTwo = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
			specialActivityXML.GoodsThr = Global.GetXElementAttributeStr(xelement4, "GoodsThr");
			specialActivityXML.EffectiveTime = Global.GetXElementAttributeStr(xelement4, "EffectiveTime");
			specialActivityXML.Price = Global.GetXElementAttributeStr(xelement4, "Price");
			specialActivityXML.PurchaseNum = Global.GetXElementAttributeInt(xelement4, "PurchaseNum");
			if (!Global.g_ZhuanXiangDic.ContainsKey(specialActivityXML.ID))
			{
				Global.g_ZhuanXiangDic.Add(specialActivityXML.ID, specialActivityXML);
			}
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南测试用专享活动：productId=",
				productId,
				"; money=",
				money
			})
		});
		MUDebug.Log<string>(new string[]
		{
			"越南安卓包专享活动第三方充值接口(8, 1, zhiZhouId)：8, 1,zhiZhouId=" + zhiZhouId
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
	}

	private void BtnhuodongListChange(object sender, object e)
	{
		if (this.m_BtnhuodongList.SelectedIndex >= 0 && this.m_BtnhuodongList.SelectedItem != null)
		{
			BtnObjZhuanXiang component = this.m_BtnhuodongList.SelectedItem.GetComponent<BtnObjZhuanXiang>();
			this.SetUIState(component.ActID);
		}
	}

	private void SetUIState(int actID)
	{
		foreach (KeyValuePair<int, BtnObjZhuanXiang> keyValuePair in this.m_ListBtns)
		{
			if (actID == keyValuePair.Key)
			{
				Dictionary<int, BtnObjZhuanXiang> listBtns = this.m_ListBtns;
				Dictionary<int, BtnObjZhuanXiang>.Enumerator enumerator;
				KeyValuePair<int, BtnObjZhuanXiang> keyValuePair2 = enumerator.Current;
				if (listBtns[keyValuePair2.Key].Pni == null)
				{
					Dictionary<int, BtnObjZhuanXiang> listBtns2 = this.m_ListBtns;
					KeyValuePair<int, BtnObjZhuanXiang> keyValuePair3 = enumerator.Current;
					this.AddItem(listBtns2[keyValuePair3.Key]);
				}
				Dictionary<int, BtnObjZhuanXiang> listBtns3 = this.m_ListBtns;
				KeyValuePair<int, BtnObjZhuanXiang> keyValuePair4 = enumerator.Current;
				listBtns3[keyValuePair4.Key].IsActiveItem = true;
			}
			else
			{
				Dictionary<int, BtnObjZhuanXiang> listBtns4 = this.m_ListBtns;
				Dictionary<int, BtnObjZhuanXiang>.Enumerator enumerator;
				KeyValuePair<int, BtnObjZhuanXiang> keyValuePair5 = enumerator.Current;
				listBtns4[keyValuePair5.Key].IsActiveItem = false;
			}
		}
	}

	private void AddItem(BtnObjZhuanXiang Btnitem)
	{
		for (int i = 0; i < this.mlistSpecActInfo.Count; i++)
		{
			SpecActInfo specActInfo = this.mlistSpecActInfo[i];
			if (specActInfo != null)
			{
				if (specActInfo.ActID == Btnitem.ActID)
				{
					SpecialActivityXML mSpecialActivityXML = default(SpecialActivityXML);
					string text = string.Empty;
					string text2 = string.Empty;
					string effect = string.Empty;
					if (Global.g_ZhuanXiangDic.TryGetValue(Btnitem.ActID, ref mSpecialActivityXML))
					{
						ZhuanxiangHuodongItem zhuanxiangHuodongItem = U3DUtils.NEW<ZhuanxiangHuodongItem>();
						U3DUtils.AddChild(this.m_ItemPosition, zhuanxiangHuodongItem.gameObject, false);
						Btnitem.Pni = zhuanxiangHuodongItem;
						zhuanxiangHuodongItem.transform.localPosition = Vector3.zero;
						zhuanxiangHuodongItem.transform.localScale = Vector3.one;
						Btnitem.m_Button.Text = Btnitem.name;
						int num = 0;
						if (!string.IsNullOrEmpty(mSpecialActivityXML.GoodsTwo))
						{
							text = mSpecialActivityXML.GoodsOne + "@" + mSpecialActivityXML.GoodsTwo;
							string[] array = StringUtil.trim(mSpecialActivityXML.GoodsOne).Split(new char[]
							{
								'|'
							});
							string[] array2 = StringUtil.trim(mSpecialActivityXML.GoodsTwo).Split(new char[]
							{
								'|'
							});
							num += array.Length;
							num += array2.Length;
						}
						else
						{
							text = mSpecialActivityXML.GoodsOne;
							string[] array3 = text.Split(new char[]
							{
								'|'
							});
							num += array3.Length;
						}
						zhuanxiangHuodongItem.SetListBox(num);
						text2 = mSpecialActivityXML.GoodsThr;
						effect = mSpecialActivityXML.EffectiveTime;
						ZhuanXiangType type = (ZhuanXiangType)mSpecialActivityXML.Type;
						if (type != ZhuanXiangType.XianshiQianggou)
						{
							if (type != ZhuanXiangType.ChongzhiDuihuan)
							{
								if (type != ZhuanXiangType.ChaoJiZhiGou)
								{
									Super.LoadGoodsList(text, zhuanxiangHuodongItem.jiangliListBoxOBC);
									Super.LoadOtherGoodsList(text2, zhuanxiangHuodongItem.jiangliListBoxOBC, effect);
									Btnitem.MUIType = UIType.Other;
									zhuanxiangHuodongItem.InitItemData(UIType.Other, (ZhuanXiangType)mSpecialActivityXML.Type, specActInfo, mSpecialActivityXML, this.mGroupID, string.Empty);
								}
								else
								{
									if (!string.IsNullOrEmpty(text))
									{
										Super.LoadGoodsList(text, zhuanxiangHuodongItem.qianggouListBoxOBC);
									}
									else if (!string.IsNullOrEmpty(text2))
									{
										Super.LoadOtherGoodsList(text2, zhuanxiangHuodongItem.qianggouListBoxOBC, effect);
									}
									Btnitem.MUIType = UIType.ZhiGou;
									string price = string.Empty;
									if (mSpecialActivityXML.Price.Split(new char[]
									{
										'|'
									}).Length == 3 && this.chongzhiInfoDict.ContainsKey(mSpecialActivityXML.Price.Split(new char[]
									{
										'|'
									})[1]))
									{
										price = this.chongzhiInfoDict[mSpecialActivityXML.Price.Split(new char[]
										{
											'|'
										})[1]].money;
									}
									zhuanxiangHuodongItem.InitItemData(UIType.ZhiGou, (ZhuanXiangType)mSpecialActivityXML.Type, specActInfo, mSpecialActivityXML, this.mGroupID, price);
								}
							}
							else
							{
								Super.LoadGoodsList(text, zhuanxiangHuodongItem.jiangliListBoxOBC);
								Super.LoadOtherGoodsList(text2, zhuanxiangHuodongItem.jiangliListBoxOBC, effect);
								Btnitem.MUIType = UIType.ChongZhi;
								zhuanxiangHuodongItem.InitItemData(UIType.ChongZhi, (ZhuanXiangType)mSpecialActivityXML.Type, specActInfo, mSpecialActivityXML, this.mGroupID, string.Empty);
							}
						}
						else
						{
							if (!string.IsNullOrEmpty(text))
							{
								Super.LoadGoodsList(text, zhuanxiangHuodongItem.qianggouListBoxOBC);
							}
							else if (!string.IsNullOrEmpty(text2))
							{
								Super.LoadOtherGoodsList(text2, zhuanxiangHuodongItem.qianggouListBoxOBC, effect);
							}
							Btnitem.MUIType = UIType.Qianggou;
							zhuanxiangHuodongItem.InitItemData(UIType.Qianggou, (ZhuanXiangType)mSpecialActivityXML.Type, specActInfo, mSpecialActivityXML, this.mGroupID, string.Empty);
						}
						zhuanxiangHuodongItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
						{
							if (this.m_ListBtns[e.Flag].type != 14)
							{
								this.DPSelectedItem(s, e);
								return;
							}
							string productId = string.Empty;
							if (this.chongzhiInfoDict.ContainsKey(mSpecialActivityXML.Price.Split(new char[]
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
						};
						UIPanel component = zhuanxiangHuodongItem.gameObject.GetComponent<UIPanel>();
						if (component)
						{
							Object.Destroy(component);
						}
					}
				}
			}
		}
	}

	public ListBox huodongList;

	public ListBox m_BtnhuodongList;

	public GButton btnClose;

	public GameObject m_ItemPosition;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection m_BtnhuodongListOBC;

	public Dictionary<int, BtnObjZhuanXiang> m_ListBtns = new Dictionary<int, BtnObjZhuanXiang>();

	private Dictionary<string, ZhuanxiangHuodongPart.ChongzhiInfo> chongzhiInfoDict = new Dictionary<string, ZhuanxiangHuodongPart.ChongzhiInfo>();

	private List<SpecActInfo> mlistSpecActInfo = new List<SpecActInfo>();

	private int mGroupID;

	public class ChongzhiInfo
	{
		public string Icon = string.Empty;

		public string money = string.Empty;

		public string zuanshiCount = string.Empty;

		public string productId = string.Empty;

		public string freeDiamond = string.Empty;

		public ZhuanxiangHuodongPart.ChongzhiInfo.ChongZhiType Type;

		public enum ChongZhiType
		{
			Normal,
			YueKa
		}
	}
}
