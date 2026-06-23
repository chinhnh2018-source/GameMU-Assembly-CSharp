using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;

public class HefuPartChongzhiFanli : UserControl
{
	public ObservableCollection ItemCollectionChongZhi
	{
		get
		{
			return this._ItemCollectionChongZhi;
		}
		set
		{
			this._ItemCollectionChongZhi = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnLingQu.Text = Global.GetLang("领取");
		this.btnChongZhi.Text = Global.GetLang("充值");
		this.labWeiLing.text = Global.GetLang("累计未领取返利：");
		this.labRound.text = Global.GetLang("名次");
		this.labName.text = Global.GetLang("玩家名称");
		this.labPercent.text = Global.GetLang("返还比例");
		this.labNameWu.text = Global.GetLang("无");
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollectionChongZhi = this.RewardList.ItemsSource;
		this.InitTime();
		this.InitRewardItem();
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 23);
		this.btnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnLingQu.isEnabled)
			{
				return;
			}
			GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 23, 0);
		};
		this.btnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			MUDebug.Log<string>(new string[]
			{
				"充值界面"
			});
		};
	}

	private void InitTime()
	{
		this.startTime = Global.GetServerMergeHuodongTimeDateTime(0, 0, 0, 0);
		this.endTime = Global.GetServerMergeHuodongTimeDateTime(6, 23, 59, 59);
		this.lingquStartTime = Global.GetServerMergeHuodongTimeDateTime(1, 0, 0, 0);
		this.lingquEndTime = Global.GetServerMergeHuodongTimeDateTime(7, 23, 59, 59);
		DateTime dateTime;
		dateTime..ctor(this.endTime.Year, this.endTime.Month, this.endTime.Day, 0, 0, 0);
		DateTime dateTime2;
		dateTime2..ctor(this.lingquStartTime.Year, this.lingquStartTime.Month, this.lingquStartTime.Day, 0, 0, 0);
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
		XElement gameResXml = Global.GetGameResXml("Config/HeFuFanLi.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			HefuChongzhiFanliItem hefuChongzhiFanliItem = U3DUtils.NEW<HefuChongzhiFanliItem>();
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
			float num = Global.GetXElementAttributeFloat(xelement, "FanLi") * 100f;
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinYuanBao");
			if (num != 3f)
			{
				hefuChongzhiFanliItem.rank.text = xelementAttributeInt.ToString();
			}
			else
			{
				hefuChongzhiFanliItem.rank.text = Global.GetLang("无");
				hefuChongzhiFanliItem.labelDi.text = string.Empty;
				hefuChongzhiFanliItem.labelMing.text = string.Empty;
			}
			hefuChongzhiFanliItem.precent.text = string.Format("{0}%", num);
			hefuChongzhiFanliItem.roleName.text = Global.GetLang("无");
			this.ItemCollectionChongZhi.Add(hefuChongzhiFanliItem);
			this.CurrentItem.Add(hefuChongzhiFanliItem);
		}
		this.setBtnCanNot();
	}

	public void InitChongzhiFanliData(int myValue, string rank_name)
	{
		int count = this.CurrentItem.Count;
		this.RewardNum.text = myValue.ToString();
		if (myValue != 0 && Global.InLimitTimeRange(this.lingquStartTimeStr, this.lingquEndTimeStr))
		{
			this.setBtnCan();
		}
		string[] array = rank_name.Split(new char[]
		{
			'|'
		});
		int num = Convert.ToInt32(array[0]);
		if (num == 0)
		{
			this.CurrentItem[4].rank.text = Global.GetLang("无");
		}
		for (int i = 0; i < num; i++)
		{
			HefuChongzhiFanliItem hefuChongzhiFanliItem = this.CurrentItem[i];
			string[] array2 = array[i + 1].Split(new char[]
			{
				','
			});
			hefuChongzhiFanliItem.rank.text = array2[0];
			hefuChongzhiFanliItem.roleName.text = Global.GetLang(array2[2]);
		}
	}

	public void setBtnCanNot()
	{
		this.btnLingQu.isEnabled = false;
	}

	public void setBtnCan()
	{
		this.btnLingQu.isEnabled = true;
	}

	public void setMyvalue()
	{
		this.RewardNum.text = "0";
		this.btnLingQu.isEnabled = false;
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox RewardList;

	public GButton btnChongZhi;

	public GButton btnLingQu;

	public TextBlock RewardNum;

	public TextBlock labWeiLing;

	public TextBlock labRound;

	public TextBlock labName;

	public TextBlock labPercent;

	public TextBlock labNameWu;

	private DateTime startTime;

	private DateTime endTime;

	private DateTime lingquStartTime;

	private DateTime lingquEndTime;

	private string startTimeStr;

	private string endTimeStr;

	private string lingquEndTimeStr;

	private string lingquStartTimeStr;

	private List<HefuChongzhiFanliItem> CurrentItem = new List<HefuChongzhiFanliItem>();

	private ObservableCollection _ItemCollectionChongZhi;
}
