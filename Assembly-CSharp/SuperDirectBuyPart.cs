using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class SuperDirectBuyPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._DraggablePanel.onDragIng = delegate()
		{
			for (int i = 0; i < this.m_ListBoxItems.Count; i++)
			{
				if (null != this.m_ListBoxItems[i])
				{
					this.m_ListBoxItems[i].GoodsTipsCanShow = false;
				}
			}
		};
	}

	public void InitXml(string XmlData)
	{
		XElement xelement = XElement.Parse(XmlData);
		if (xelement != null)
		{
			XElement xelement2 = Global.GetXElement(xelement, "Activities");
			if (xelement2 != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
				string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
				this.m_JieRiChongZhiQiangGouData1 = new JieRiChongZhiQiangGouData1(xelementAttributeStr, xelementAttributeStr2, xelementAttributeStr3, xelementAttributeStr4);
			}
			List<XElement> xelementList = Global.GetXElementList(xelement, "GiftList");
			if (this.m_JieRiChongZhiQiangGouData2 == null)
			{
				this.m_JieRiChongZhiQiangGouData2 = new List<JieRiChongZhiQiangGouData2>();
			}
			if (xelementList.Count == 1)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					if (xelementList != null)
					{
						this.m_JieRiChongZhiQiangGouData2.Add(new JieRiChongZhiQiangGouData2(xelementList[i]));
					}
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("越南产品！你配错了！打死")
				});
				MUDebug.Log<string>(new string[]
				{
					string.Format("GiftList.Count = {0}", xelementList.Count)
				});
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetSuperDirectBuyData();
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"YN超级直购：productId=",
				productId,
				"; money=",
				money
			})
		});
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南Android超级直购第三方充值：(8,1,zhiZhouId)productId=",
				productId,
				"; money=",
				money,
				";zhiZhouId=",
				zhiZhouId
			})
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
	}

	private bool CheckTime(DateTime BeginTime, DateTime EndTime, string day)
	{
		if (0L >= EndTime.Ticks - Global.GetCorrectDateTime().Ticks)
		{
			return false;
		}
		string[] array = day.Split(new char[]
		{
			','
		});
		int num = int.Parse(array[0]);
		int addDays = int.Parse(array[1]);
		return 0L < this.GetAddDaysDataTime(BeginTime, addDays, true).Ticks - Global.GetCorrectDateTime().Ticks && 0L >= this.GetAddDaysDataTime(BeginTime, num - 1, true).Ticks - Global.GetCorrectDateTime().Ticks;
	}

	public DateTime GetAddDaysDataTime(DateTime dateTime, int addDays, bool roundDay = true)
	{
		if (roundDay)
		{
			dateTime..ctor(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
		}
		return new DateTime(dateTime.Ticks + (long)addDays * 10000L * 1000L * 24L * 60L * 60L);
	}

	private void InitView()
	{
		if (this.m_JieRiChongZhiQiangGouData2.Count == 1 && this.m_ListBoxItems.Count == 0)
		{
			this.m_ListBoxItems.Clear();
			JieRiChongZhiQiangGouData2 jieRiChongZhiQiangGouData = this.m_JieRiChongZhiQiangGouData2[0];
			if (jieRiChongZhiQiangGouData != null)
			{
				int currentSelectedPage = 0;
				List<JieRiChongZhiQiangGouData3> GiftList = jieRiChongZhiQiangGouData._GiftList;
				int j;
				GiftList.Sort((JieRiChongZhiQiangGouData3 j, JieRiChongZhiQiangGouData3 k) => j.ID - k.ID);
				for (int i = 0; i < GiftList.Count; i++)
				{
					JieRiChongZhiQiangGouData3 it = GiftList[i];
					if (this.m_JieriCZQGDatalst != null)
					{
						JieriCZQGData jieriCZQGData = this.m_JieriCZQGDatalst.Find((JieriCZQGData e) => e.ID == it.ID);
						if (jieriCZQGData != null)
						{
							it.XianGouLeftNum = it.SinglePurchase - jieriCZQGData.PurchaseNum;
						}
					}
				}
				if (0 < GiftList.Count)
				{
					for (int l = 0; l < GiftList.Count; l++)
					{
						JieRiChongZhiQiangGouData3 jieRiChongZhiQiangGouData2 = GiftList[l];
						if (this.CheckTime(this.m_JieRiChongZhiQiangGouData1.FromDate, this.m_JieRiChongZhiQiangGouData1.ToDate, jieRiChongZhiQiangGouData2.Day))
						{
							SuperDirectBuy superDirectBuy = U3DUtils.NEW<SuperDirectBuy>();
							superDirectBuy.RefreshContent(jieRiChongZhiQiangGouData2, this.m_JieRiChongZhiQiangGouData1);
							superDirectBuy.Hander = delegate(object e, DPSelectedItemEventArgs s)
							{
								JieRiChongZhiQiangGouData3 jieRiChongZhiQiangGouData3 = GiftList.Find((JieRiChongZhiQiangGouData3 d) => d.ID == s.ID);
								if (jieRiChongZhiQiangGouData3 != null)
								{
									this.ChongZhi(jieRiChongZhiQiangGouData3.XianJia, jieRiChongZhiQiangGouData3.productId, jieRiChongZhiQiangGouData3.ZhiGouID);
								}
								else
								{
									Super.HintMainText(Global.GetLang("充值ID有误"), 10, 3);
								}
							};
							superDirectBuy.TitleImage = jieRiChongZhiQiangGouData2.TitlePic;
							superDirectBuy.Effect = jieRiChongZhiQiangGouData2.Effect;
							superDirectBuy.BgImageURL = jieRiChongZhiQiangGouData2.Background;
							superDirectBuy.DragPanelContents = this._DraggablePanel;
							superDirectBuy.ChongZhiID = jieRiChongZhiQiangGouData2.ChongZhiID;
							superDirectBuy.SinglePurchase = jieRiChongZhiQiangGouData2.SinglePurchase;
							if (this.m_ObservableCollection == null)
							{
								this.m_ObservableCollection = this._ListBox.ItemsSource;
							}
							this.m_ObservableCollection.AddNoUpdate(superDirectBuy);
							this.m_ListBoxItems.Add(superDirectBuy);
							superDirectBuy.DestoryPanel();
						}
					}
				}
				this._ListBox.repositionNow = true;
				if (this.m_ObservableCollection.Count == 1)
				{
					this._ListBox.transform.localPosition = new Vector3(74f, 0f, 0f);
				}
				for (j = 0; j < this.m_ListBoxItems.Count; j++)
				{
					if (this.m_ListBoxItems[j].BuyCount > 0)
					{
						currentSelectedPage = j;
						break;
					}
				}
				this.CurrentSelectedPage = currentSelectedPage;
			}
		}
	}

	public void RefreshUI(List<JieriCZQGData> data)
	{
		this.m_JieriCZQGDatalst = data;
		this.InitView();
		if (data != null)
		{
			for (int i = 0; i < data.Count; i++)
			{
				JieriCZQGData DItem = data[i];
				SuperDirectBuy superDirectBuy = this.m_ListBoxItems.Find((SuperDirectBuy e) => e.ID == DItem.ID);
				if (null != superDirectBuy)
				{
					superDirectBuy.BuyCount = superDirectBuy.SinglePurchase - DItem.PurchaseNum;
				}
			}
		}
	}

	public UISprite _PageSp;

	public ListBox _ListBox;

	public ListBox _Page1;

	public UIDraggablePanel _DraggablePanel;

	public SpringPanel _SpringPanel;

	private ObservableCollection m_ObservableCollection;

	private ObservableCollection m_ObC1;

	private JieRiChongZhiQiangGouData1 m_JieRiChongZhiQiangGouData1;

	private List<SuperDirectBuy> m_ListBoxItems = new List<SuperDirectBuy>();

	private List<UISprite> m_PageList = new List<UISprite>();

	private List<JieRiChongZhiQiangGouData2> m_JieRiChongZhiQiangGouData2;

	private int CurrentSelectedPage;

	private List<JieriCZQGData> m_JieriCZQGDatalst;

	private int Distance = 792;

	private UISprite tempPaneStat;
}
