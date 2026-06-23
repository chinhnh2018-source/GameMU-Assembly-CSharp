using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Tmsk.Xml;

public class HefuPartDengluhaoli : UserControl
{
	public ObservableCollection ItemCollectionHaoli
	{
		get
		{
			return this._ItemCollectionHaoli;
		}
		set
		{
			this._ItemCollectionHaoli = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
		this.labNormal.text = Global.GetLang("登录领取");
		this.labVIP.text = Global.GetLang("VIP 领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollectionHaoli = this.haoliList.ItemsSource;
		this.InitTime();
		this.InitHaoliItem();
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 20);
	}

	private void InitHaoliItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HeFuLiBao.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		XElement xelement = xelementList[0];
		DengluHaoliItem dengluHaoliItem = U3DUtils.NEW<DengluHaoliItem>();
		dengluHaoliItem.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
		dengluHaoliItem.Awardtype = Awardtype.Normal;
		this.ItemCollectionHaoli.Add(dengluHaoliItem);
		this.CurrentItems.Add(dengluHaoliItem);
		DengluHaoliItem dengluHaoliItem2 = U3DUtils.NEW<DengluHaoliItem>();
		dengluHaoliItem2.GoodsIDs = Global.GetXElementAttributeStr(xelement, "VIPGoodsIDs");
		dengluHaoliItem2.Awardtype = Awardtype.VIP;
		if (!Global.IsVip())
		{
			dengluHaoliItem2.lingquBtn.isEnabled = false;
		}
		this.ItemCollectionHaoli.Add(dengluHaoliItem2);
		this.CurrentItems.Add(dengluHaoliItem2);
	}

	private void InitTime()
	{
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.lingquEndTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		DateTime dateTime;
		dateTime..ctor(this.endTime.Year, this.endTime.Month, this.endTime.Day, 0, 0, 0);
		this.startTimeStr = this.startTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.endTimeStr = this.endTime.ToString("yyyy-MM-dd HH:mm:ss");
		this.lingquEndTimeStr = this.lingquEndTime.ToString("yyyy-MM-dd HH:mm:ss");
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
			this.startTimeStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.lingquEndTimeStr
		});
	}

	public void InitDataFromServerInfo(int flag)
	{
		DengluHaoliItem dengluHaoliItem = this.CurrentItems[0];
		DengluHaoliItem dengluHaoliItem2 = this.CurrentItems[1];
		int intSomeBit = Global.GetIntSomeBit(flag, 1);
		int intSomeBit2 = Global.GetIntSomeBit(flag, 2);
		int intSomeBit3 = Global.GetIntSomeBit(flag, 3);
		if (intSomeBit == 1 && Global.InLimitTimeRange(this.startTimeStr, this.endTimeStr))
		{
			dengluHaoliItem.state = AwardGiftGainState.CanGain;
			if (Global.IsVip())
			{
				dengluHaoliItem2.state = AwardGiftGainState.CanGain;
			}
		}
		if (intSomeBit2 == 1)
		{
			dengluHaoliItem.state = AwardGiftGainState.Gained;
		}
		if (intSomeBit2 == 0)
		{
			dengluHaoliItem.state = AwardGiftGainState.CanGain;
		}
		if (intSomeBit3 == 1 && Global.IsVip())
		{
			dengluHaoliItem2.state = AwardGiftGainState.Gained;
		}
		if (intSomeBit3 == 0 && Global.IsVip())
		{
			dengluHaoliItem2.state = AwardGiftGainState.CanGain;
		}
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox haoliList;

	public UIPanel haoliPanel;

	public TextBlock labNormal;

	public TextBlock labVIP;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime lingquEndTime;

	private string startTimeStr;

	private string endTimeStr;

	private string lingquEndTimeStr;

	private ObservableCollection _ItemCollectionHaoli;

	private List<DengluHaoliItem> CurrentItems = new List<DengluHaoliItem>();

	public enum HeFuLoginFlagTypes
	{
		HeFuLogin_Null,
		HeFuLogin_Login,
		HeFuLogin_NormalAward,
		HeFuLogin_VIPAward
	}
}
