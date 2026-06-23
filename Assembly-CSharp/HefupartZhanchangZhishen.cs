using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class HefupartZhanchangZhishen : UserControl
{
	private void InitTextInPrefabs()
	{
		this.labDaojishi.text = Global.GetLang("开启倒计时:");
		this.btnLingqu.Text = Global.GetLang("领取");
		this.btnJoin.Text = Global.GetLang("参与活动");
		this.daojishiLabelTime.Pivot = 3;
		this.daojishiLabelTime.X = -165.0;
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
		this.btnJoin.Label.lineWidth = 120;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.rewardList.ItemsSource;
		this.InitTime();
		this.InitRewardItem();
		this.GetBeginTime();
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 24);
		this.btnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnLingqu.isEnabled)
			{
				return;
			}
			GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 24, 0);
		};
		this.btnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 105
				});
			}
		};
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTime()
	{
		this.startTicks = Global.GetCorrectLocalTime();
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(4, 23, 59, 59);
		this.lingquStartTime = Global.GetServerMergeHuodongTimeDateTime(3, 0, 0, 0);
		this.lingquEndTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.startTimeStr = this.startTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.endTimeStr = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.lingquStartTimeStr = this.lingquStartTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.lingquEndTimeStr = this.lingquEndTime.toString("yyyy-MM-dd HH:mm:ss");
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.lingquStartTimeStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.lingquEndTimeStr
		});
	}

	private void InitRewardItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/PKJiangLi.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		XElement xelement = xelementList[0];
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDOne");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsIDTwo");
		string goodsIDs = string.Empty;
		if (!string.IsNullOrEmpty(xelementAttributeStr2))
		{
			goodsIDs = xelementAttributeStr + "@" + xelementAttributeStr2;
		}
		else
		{
			goodsIDs = xelementAttributeStr;
		}
		this.loadGoodsList(goodsIDs);
	}

	private void GetBeginTime()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ArenaBattle.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		XElement xelement = xelementList[0];
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "TimePoints");
		List<int> list = UIHelper.ParserTimeArrayString2(xelementAttributeStr);
		this.TimeSec = list[0];
		this.needtime = this.needTime();
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	private long needTime()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int num = correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second;
		if (num <= this.TimeSec)
		{
			return (long)(this.TimeSec - num);
		}
		return (long)(this.TimeSec + 86400 - num);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime > this.startTicks)
		{
			int num = (int)((correctLocalTime - this.startTicks) / 1000L);
			if ((long)num >= this.needtime)
			{
				this.daojishiLabelTime.text = string.Empty;
				base.CancelInvoke("TickProc");
				this.GetBeginTime();
			}
			else
			{
				this.daojishiLabelTime.Text = Global.GetLang("剩余") + Global.GetTimeStrBySecEx((double)(this.needtime - (long)num), true, -1);
			}
		}
	}

	public void InitZhanchangZhishen(int roleID, int state)
	{
		if (Global.Data.roleData.RoleID == roleID)
		{
			this.btnLingqu.isEnabled = true;
		}
		else
		{
			this.btnLingqu.isEnabled = false;
		}
	}

	public void SetBtnstate()
	{
		this.btnLingqu.isEnabled = false;
	}

	private void loadGoodsList(string goodsIDs)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.loadOtherJiangLiGoodsList(goodsIDs, false);
			}
			else
			{
				this.loadOtherJiangLiGoodsList(array[0], true);
				this.loadOtherJiangLiGoodsList(array[1], false);
			}
		}
	}

	private void loadOtherJiangLiGoodsList(string goodsStr, bool isOcc = false)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.addGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.ItemCollection.Add(icon);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox rewardList;

	public GButton btnLingqu;

	public GButton btnJoin;

	public TextBlock daojishiLabelTime;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime lingquStartTime;

	private DateTime lingquEndTime;

	private string startTimeStr;

	private string endTimeStr;

	private string lingquEndTimeStr;

	private string lingquStartTimeStr;

	private long startTicks;

	private int TimeSec;

	private long needtime;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock labDaojishi;

	private ObservableCollection _ItemCollection;
}
