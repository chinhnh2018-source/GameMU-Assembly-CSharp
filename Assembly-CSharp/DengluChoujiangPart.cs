using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class DengluChoujiangPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ListDengLuHaoLiObC = this.m_ListDengLuHaoLi.ItemsSource;
		this.m_ListMeiRiZaiXianObC = this.m_ListMeiRiZaiXian.ItemsSource;
		if (null != this.m_GTab)
		{
			this.m_GTab.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.m_LblWindowTitle.text = e.Title;
				if (e.ID == 0)
				{
					this.m_nCurrentTabIndex = 0;
				}
				else if (e.ID == 1)
				{
					this.m_nCurrentTabIndex = 1;
				}
				else if (e.ID == 2)
				{
					this.m_nCurrentTabIndex = 2;
				}
				else if (e.ID == 3)
				{
					this.m_nCurrentTabIndex = 3;
					if (Global.Data.MyHuoDongData == null)
					{
						GameInstance.Game.SpriteGetHuoDongData();
					}
				}
				else if (e.ID == 4)
				{
					this.m_nCurrentTabIndex = 4;
				}
				else if (e.ID == 5)
				{
					this.m_nCurrentTabIndex = 5;
					if (Global.Data.MyHuoDongData != null)
					{
						this.GetMeiRiZaiXianChouJiangNum();
					}
					if (Global.Data.MyHuoDongData == null)
					{
						GameInstance.Game.SpriteGetHuoDongData();
					}
				}
				return true;
			};
		}
		if (null != this.m_BtnDengLuHaoLiTab)
		{
			this.m_BtnDengLuHaoLiTab.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
			};
		}
		if (null != this.m_BtnMeiRiZaiXianTab)
		{
			this.m_BtnMeiRiZaiXianTab.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
			};
		}
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		if (null != this.m_BtnDengLuHaoLiChouJiang)
		{
			this.m_BtnDengLuHaoLiChouJiang.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DengLuHaoLiChouJiang();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					IDType = 0
				});
			};
		}
		if (null != this.m_BtnMeiRiZaiXianChouJiang)
		{
			this.m_BtnMeiRiZaiXianChouJiang.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				GameInstance.Game.SpriteGetEveryDayOnLineAwardGiftCmd(1);
				this.MeiRiZaiXianChouJiang();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3,
					IDType = 0
				});
			};
		}
		this.InitControl();
	}

	private void DengLuHaoLiChouJiang()
	{
		for (int i = 0; i < this.m_ListDengLuHaoLiObC.Count; i++)
		{
			GameObject at = this.m_ListDengLuHaoLiObC.GetAt(i);
			if (null != at)
			{
				DengluChoujiangListItem component = at.gameObject.GetComponent<DengluChoujiangListItem>();
				if (null != component)
				{
					TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
					if (null != component2)
					{
						component.m_bBegining = true;
						component2.enabled = true;
					}
				}
			}
		}
	}

	private void MeiRiZaiXianChouJiang()
	{
		for (int i = 0; i < this.m_ListMeiRiZaiXianObC.Count; i++)
		{
			GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(i);
			if (null != at)
			{
				MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
				if (null != component)
				{
					TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
					if (null != component2 && component.m_bChouJiang)
					{
						component.m_bBegining = true;
						component2.enabled = true;
					}
				}
			}
		}
	}

	private void ChangeIcon(GameObject GObject)
	{
		if (null != GObject)
		{
			GGoodIcon component = GObject.gameObject.GetComponent<GGoodIcon>();
			component.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				"Images/Goods/4004.png"
			}), false, 0);
		}
	}

	public void InitControl()
	{
		if (null != this.m_LblLianXuDengLuNum)
		{
			this.m_LblLianXuDengLuNum.text = string.Format(Global.GetLang("连续登陆{0}天"), Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				"4"
			}));
		}
		if (null != this.m_LblLianXuDengLuInfo)
		{
			this.m_LblLianXuDengLuInfo.text = string.Format(Global.GetLang("连续登陆{0}天可获得{0}个物品"), Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				"7"
			}));
		}
		this.GetDengluChouJiangNum();
		this.ItemCollection = this.listBox.ItemsSource;
	}

	private GGoodIcon GetGoodsItemIcon(string strID, bool isDrag = false)
	{
		GoodsData goodsData = new GoodsData();
		goodsData.GoodsID = Convert.ToInt32(strID);
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(goodsData.GoodsID, false);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private void GetMeiRiZaiXianChouJiangNum()
	{
		this.m_nZaiXianShiChang = Global.Data.roleData.DayOnlineSecond;
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/MUNewRoleGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.m_ListMeiRiZaiXianObC.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			MeiRiZaiXianItem meiRiZaiXianItem = U3DUtils.NEW<MeiRiZaiXianItem>();
			meiRiZaiXianItem.m_ListJiangPinObC = meiRiZaiXianItem.m_ListJiangPin.ItemsSource;
			meiRiZaiXianItem.m_ListJiangPinObC.Clear();
			this.m_ListMeiRiZaiXianObC.AddNoUpdate(meiRiZaiXianItem);
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowGoods");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "AwardGoodsList");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "TimeSecs");
			meiRiZaiXianItem.m_LblShowTime.text = string.Format(Global.GetLang("{0}分钟"), xelementAttributeStr4);
			meiRiZaiXianItem.m_nMinTime = Convert.ToInt32(xelementAttributeStr4);
			if (Convert.ToInt32(xelementAttributeStr4) * 60 > this.m_nZaiXianShiChang)
			{
				meiRiZaiXianItem.m_ProgressBar.sliderValue = 1f;
				meiRiZaiXianItem.m_bChouJiang = true;
			}
			else
			{
				int num = Convert.ToInt32(xelementAttributeStr4) * 60;
				meiRiZaiXianItem.m_ProgressBar.sliderValue = (float)this.m_nZaiXianShiChang / (float)num;
			}
			this.m_nZaiXianShiChang -= Convert.ToInt32(xelementAttributeStr4) * 60;
			string[] array = xelementAttributeStr.Split(new char[]
			{
				','
			});
			int num2 = 0;
			foreach (string strID in array)
			{
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strID, false);
				meiRiZaiXianItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
				if (num2 == 0)
				{
					meiRiZaiXianItem.m_BeginPos = goodsItemIcon.gameObject.transform.localPosition;
				}
				num2++;
			}
			meiRiZaiXianItem.m_ListJiangPinObC.DelayUpdate();
		}
		this.m_ListMeiRiZaiXianObC.DelayUpdate();
	}

	private void GetDengluChouJiangNum()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/MULoginNumGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.m_ListDengLuHaoLiObC.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		for (int i = 0; i < xelementList.Count; i++)
		{
			DengluChoujiangListItem dengluChoujiangListItem = U3DUtils.NEW<DengluChoujiangListItem>();
			dengluChoujiangListItem.m_ListJiangPinObC = dengluChoujiangListItem.m_ListJiangPin.ItemsSource;
			dengluChoujiangListItem.m_ListJiangPinObC.Clear();
			this.m_ListDengLuHaoLiObC.AddNoUpdate(dengluChoujiangListItem);
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowGoods");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "AwardGoodsList");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "LoginTime");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				','
			});
			foreach (string strID in array)
			{
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strID, false);
				dengluChoujiangListItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
			}
			dengluChoujiangListItem.m_ListJiangPinObC.DelayUpdate();
		}
		this.m_ListDengLuHaoLiObC.DelayUpdate();
	}

	protected virtual void OnEnable()
	{
		base.StartCoroutine<bool>(this.TimeProc());
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			DateTime now = Global.GetCorrectDateTime();
			float dealTime = Time.time;
			this.MeiRiChouJiangStop(dealTime);
			this.DengLuHaoLiChouJiangStop();
			if (null != this.m_LblLianXuanZaiXianShiChang)
			{
				this.m_LblLianXuanZaiXianShiChang.text = string.Format(Global.GetLang("今日在线时长：{0}"), Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					UIHelper.FormatSecs((long)dealTime, string.Empty)
				}));
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	private void DengLuHaoLiChouJiangStop()
	{
		if (0 < this.m_ListDengLuHaoLiObC.Count)
		{
			for (int i = 0; i < this.m_ListDengLuHaoLiObC.Count; i++)
			{
				GameObject at = this.m_ListDengLuHaoLiObC.GetAt(i);
				if (null != at)
				{
					DengluChoujiangListItem component = at.gameObject.GetComponent<DengluChoujiangListItem>();
					if (null != component && component.m_bBegining)
					{
						if (60f < component.m_ListJiangPin.gameObject.transform.localPosition.y)
						{
							GameObject at2 = component.m_ListJiangPinObC.GetAt(0);
							this.ChangeIcon(at2);
							GameObject at3 = component.m_ListJiangPinObC.GetAt(component.m_ListJiangPinObC.Count - 1);
							this.ChangeIcon(at3);
						}
						TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
						component2.duration += 0.1f;
						if (5 < component.m_nLastTick && null != component2 && 40f > component.m_ListJiangPin.gameObject.transform.localPosition.y && component.m_bBegining)
						{
							component.m_bBegining = false;
							SpringPosition component3 = component.m_ListJiangPin.gameObject.GetComponent<SpringPosition>();
							component3.target.x = 0f;
							component3.target.y = 0f;
							component3.enabled = true;
							component2.enabled = false;
							component2.duration = 0.2f;
						}
						component.m_nLastTick++;
					}
				}
			}
		}
	}

	private void MeiRiChouJiangStop(float dealTime)
	{
		if (0 < this.m_ListMeiRiZaiXianObC.Count)
		{
			for (int i = 0; i < this.m_ListMeiRiZaiXianObC.Count; i++)
			{
				GameObject at = this.m_ListMeiRiZaiXianObC.GetAt(i);
				if (null != at)
				{
					MeiRiZaiXianItem component = at.gameObject.GetComponent<MeiRiZaiXianItem>();
					if (null != component.m_LblShowTime)
					{
						if (1f > component.m_ProgressBar.sliderValue)
						{
							component.m_ProgressBar.sliderValue = dealTime / (float)(component.m_nMinTime * 60);
						}
						if (1f <= component.m_ProgressBar.sliderValue)
						{
							component.m_bChouJiang = true;
						}
						if (component.m_bBegining)
						{
							if (60f < component.m_ListJiangPin.gameObject.transform.localPosition.y)
							{
								GameObject at2 = component.m_ListJiangPinObC.GetAt(0);
								this.ChangeIcon(at2);
								GameObject at3 = component.m_ListJiangPinObC.GetAt(component.m_ListJiangPinObC.Count - 1);
								this.ChangeIcon(at3);
							}
							TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
							component2.duration += 0.1f;
							if (5 < component.m_nLastTick && null != component2 && 40f > component.m_ListJiangPin.gameObject.transform.localPosition.y)
							{
								SpringPosition component3 = component.m_ListJiangPin.gameObject.GetComponent<SpringPosition>();
								component3.target.x = 0f;
								component3.target.y = -2f;
								component3.enabled = true;
								component2.enabled = false;
								component2.duration = 0.2f;
							}
							component.m_nLastTick++;
						}
					}
				}
			}
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteGetKaiFuOnline();
		this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
		this.Container.Children.Add(this.LoadingWin);
	}

	private KaiFuOnlineAwardData GetKaiFuOnlineAwardData(KaiFuOnlineInfoData list, int dayID)
	{
		if (list.KaiFuOnlineAwardDataList == null)
		{
			return null;
		}
		List<KaiFuOnlineAwardData> kaiFuOnlineAwardDataList = list.KaiFuOnlineAwardDataList;
		for (int i = 0; i < kaiFuOnlineAwardDataList.Count; i++)
		{
			if (kaiFuOnlineAwardDataList[i].DayID == dayID)
			{
				return kaiFuOnlineAwardDataList[i];
			}
		}
		return null;
	}

	private void InitList(KaiFuOnlineInfoData list)
	{
	}

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection.Clear();
		int num = pageIndex * 3;
		int num2 = num;
		while (num2 < this.ItemsList.Count && num2 < num + 3)
		{
			this.ItemCollection.AddNoUpdate(this.ItemsList[num2]);
			num2++;
		}
		this.ItemCollection.DelayUpdate();
		if (pageIndex <= 0)
		{
			this.PrevPageIcon.EnableIcon = false;
			this.CurrentSelectedPage = 0;
		}
		else
		{
			this.PrevPageIcon.EnableIcon = true;
		}
		if (pageIndex >= this.MaxPageCount - 1)
		{
			this.NextPageIcon.EnableIcon = false;
			this.CurrentSelectedPage = this.MaxPageCount - 1;
		}
		else
		{
			this.NextPageIcon.EnableIcon = true;
		}
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("DengluChoujiangPart_Timer");
		this.UITimer.Interval = TimeSpan.FromMilliseconds(1000.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.StopTimer();
	}

	public void OnGetInfoCompleted(int result, KaiFuOnlineInfoData list)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (list != null)
		{
			this.InitList(list);
			this.StartUITimer();
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	public int m_nCurrentTabIndex;

	public int m_nZaiXianShiChang;

	public GButton m_BtnClose;

	public GButton m_BtnDengLuHaoLiChouJiang;

	public GButton m_BtnMeiRiZaiXianChouJiang;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;

	public GTab m_GTab;

	public GButton m_BtnDengLuHaoLiTab;

	public GButton m_BtnMeiRiZaiXianTab;

	public UILabel m_LblWindowTitle;

	public UILabel m_LblLianXuDengLuNum;

	public UILabel m_LblLianXuDengLuInfo;

	public UILabel m_LblLianXuanZaiXianShiChang;

	private GIcon NextPageIcon;

	private GIcon PrevPageIcon;

	private LoadingWindow LoadingWin;

	private ListBox listBox = new ListBox();

	private List<DengluChoujiangListItem> ItemsList = new List<DengluChoujiangListItem>();

	private int CurrentSelectedPage;

	private int MaxPageCount;

	public ListBox m_ListDengLuHaoLi = new ListBox();

	private ObservableCollection m_ListDengLuHaoLiObC;

	public ListBox m_ListMeiRiZaiXian = new ListBox();

	private ObservableCollection m_ListMeiRiZaiXianObC;

	private DispatcherTimer UITimer;
}
