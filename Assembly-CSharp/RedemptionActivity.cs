using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class RedemptionActivity : UserControl
{
	private void InitTextInPrefabs()
	{
		this.depositBtn.Text = Global.GetLang("充值");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.depositBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			MUDebug.Log<string>(new string[]
			{
				"充值界面"
			});
		};
		this.SetExchangeRate();
	}

	public string xmlName
	{
		get
		{
			return this._xmlName;
		}
		set
		{
			this._xmlName = value;
			this.InitData(this.xmlName);
			GameInstance.Game.GetRedemptionActivityInfo();
		}
	}

	private void InitData(string strXML)
	{
		XElement xelement = XElement.Parse(strXML);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.SetActivityTime(xelementAttributeStr, xelementAttributeStr2, xelementAttributeStr3, xelementAttributeStr4);
		ObservableCollection itemsSource = this.redemptionList.ItemsSource;
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < xelementList2.Count; i++)
		{
			XElement xelement3 = xelementList2[i];
			if (xelement3 == null)
			{
				return;
			}
			RedemptionItem redemptionItem = U3DUtils.NEW<RedemptionItem>();
			itemsSource.Add(redemptionItem);
			redemptionItem.redemptionID = Global.GetXElementAttributeInt(xelement3, "ID");
			redemptionItem.points = Global.GetXElementAttributeInt(xelement3, "NeedChongZhiDianShu");
			redemptionItem.goodsIDs = Global.GetXElementAttributeStr(xelement3, "NewGoodsID");
			redemptionItem.leftRedeemTimes = Global.GetXElementAttributeInt(xelement3, "MaxNum");
		}
	}

	private void SetExchangeRate()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("JieRiChongZhiDuiHuan", ':');
		if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length <= 0)
		{
			return;
		}
		this.exchangeRate.Text = systemParamStringArrayByName[0];
	}

	private void SetActivityTime(string startTime_activity, string endTime_activity, string startTime_sign, string endTime_sign)
	{
		this.activity_starttime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.font_activityTime,
			startTime_activity
		});
		this.activity_endtime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.font_activityTime,
			endTime_activity
		});
		this.sign_starttime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.font_activityTime,
			startTime_sign
		});
		this.sign_endtime.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.font_activityTime,
			endTime_sign
		});
	}

	public void SetRedemptionInfo(int points, string redemptionInfo)
	{
		this.SetLeftPoints(points);
		if (string.IsNullOrEmpty(redemptionInfo))
		{
			return;
		}
		string[] array = redemptionInfo.Split(new char[]
		{
			'|'
		});
		if (array == null || array.Length <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.redemptionList.ItemsSource;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject at = itemsSource.GetAt(i);
			if (!(null == at))
			{
				RedemptionItem component = at.GetComponent<RedemptionItem>();
				if (null != component)
				{
					component.leftRedeemTimes = Global.SafeConvertToInt32(array[i]);
				}
			}
		}
	}

	public void SetLeftPoints(int points)
	{
		this.totalPoints.Text = string.Format("{0}{1}", Global.GetLang("充值点数："), points);
	}

	public void SetRedemptionItemLeftTimes(int result, int index, int leftNum)
	{
		if (result < 0 || index <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.redemptionList.ItemsSource;
		GameObject at = itemsSource.GetAt(index - 1);
		if (null == at)
		{
			return;
		}
		RedemptionItem component = at.GetComponent<RedemptionItem>();
		if (null != component)
		{
			component.leftRedeemTimes = leftNum;
		}
	}

	public TextBlock activity_starttime;

	public TextBlock activity_endtime;

	public TextBlock sign_starttime;

	public TextBlock sign_endtime;

	public TextBlock exchangeRate;

	public TextBlock totalPoints;

	public ListBox redemptionList;

	public GButton depositBtn;

	private string font_activityTime = "e3b36c";

	private string _xmlName = string.Empty;
}
