using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class HuoDongChongZhiFanLiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.RequestFanLiData();
		this.m_Time.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("12月9日 - 12月12日")
		});
		this.m_Title1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("活动期间，每次充值均可获得                        奖励")
		});
		this.m_TiShiXianGouCount.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("本轮账号剩余限购次数")
		});
		this.m_TiShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"6b441a",
			Global.GetLang("温馨提示：月卡充值不参与本次活动，原有首次充值赠送绑钻资格保留")
		});
		this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ShowChongZhiWindow();
		};
		this.mBtnLeft.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.PreviousItem();
		};
		this.mBtnRight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NextItemHandler(null, null);
		};
	}

	public void XmlTime(string xmlPath = "0")
	{
		XElement xelement;
		if (xmlPath.Equals("0"))
		{
			xelement = Global.GetGameResXml("Config/JieRiGifts/JieRiChongZhiFanLi.xml");
		}
		else
		{
			xelement = XElement.Parse(xmlPath);
		}
		if (xelement == null)
		{
			return;
		}
		XElement xelement2 = Global.GetXElement(xelement, "Activities");
		DateTime dateTime = DateTime.Parse(Global.GetXElementAttributeStr(xelement2, "AwardStartDate"));
		DateTime dateTime2 = DateTime.Parse(Global.GetXElementAttributeStr(xelement2, "AwardEndDate"));
		string text = string.Concat(new object[]
		{
			dateTime.Month,
			Global.GetLang("月"),
			dateTime.Day,
			Global.GetLang("日")
		});
		string text2 = string.Concat(new object[]
		{
			dateTime2.Month,
			Global.GetLang("月"),
			dateTime2.Day,
			Global.GetLang("日")
		});
		this.m_Time.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			"活动时间："
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			text
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			"-"
		})) + Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			text2
		}));
		this.LoadQiangGouPanelInfo();
	}

	private void LoadQiangGouPanelInfo()
	{
		XElement gameResXml = Global.GetGameResXml("Config/JieRiGifts/MU_ChongZhiFanLi.xml");
		if (string.IsNullOrEmpty(this.PlatformType))
		{
			return;
		}
		foreach (XElement xelement in gameResXml.Elements())
		{
			if (xelement.Attribute("TypeID").Value.ToString() == this.PlatformType)
			{
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement element in enumerable)
				{
					ChongZhiFanLiData chongZhiFanLiData = new ChongZhiFanLiData(element);
					if (!this.fanLiDict.ContainsKey(chongZhiFanLiData.ID))
					{
						this.fanLiDict.Add(chongZhiFanLiData.ID, chongZhiFanLiData);
					}
				}
				break;
			}
		}
		this.LoadQiangGouItem();
	}

	private void LoadQiangGouItem()
	{
		if (this.fanLiDict.Count <= 0)
		{
			return;
		}
		this.mChongZhiFanLiXMLDatas = this.FilterChongZhiFanLiXMLDictByTime();
		if (this.mChongZhiFanLiXMLDatas.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置有误，没有今天的活动配置！"
			});
			return;
		}
		this.HideErJiWinodw();
		if (this.item == null)
		{
			this.item = U3DUtils.NEW<HuoDongChongZhiFanLiItem>();
			this.item.transform.SetParent(this.mPanel.transform);
			this.item.transform.localPosition = new Vector3(17f, 10f, 0f);
			this.item.transform.localScale = Vector3.one;
		}
		this.item.InitData(this.mChongZhiFanLiXMLDatas[0]);
		this.item.EndOrSellOutHandler = new DPSelectedItemEventHandler(this.NextItemHandler);
		this.CurItemIndex = 0;
		this.mBtnLeft.gameObject.SetActive(false);
		this.CurrentState = this.item.CurState;
		if (this.mChongZhiFanLiXMLDatas.Count == 1)
		{
			this.mBtnLeft.gameObject.SetActive(false);
			this.mBtnRight.gameObject.SetActive(false);
			if (this.IsTodayLastFanLiHuoDongOver())
			{
				this.item.ResetLblTipTime = Global.GetLang("今日活动已结束");
				base.InvokeRepeating("CountDownTomorrow", 0f, 1f);
			}
		}
	}

	private void CountDownTomorrow()
	{
		if (this.mChongZhiFanLiXMLDatas.Count == 1)
		{
			bool flag = ChongChiFanLiUtils.IsNewDay(this.mChongZhiFanLiXMLDatas[0].Data.Day);
			if (flag)
			{
				base.CancelInvoke("CountDownTomorrow");
				this.LoadQiangGouItem();
			}
		}
	}

	private bool IsTodayLastFanLiHuoDongOver()
	{
		if (this.mChongZhiFanLiXMLDatas.Count != 1)
		{
			return false;
		}
		long num = ChongChiFanLiUtils.GetEndTime(this.mChongZhiFanLiXMLDatas[0].Data, this.mChongZhiFanLiXMLDatas[0].EndTime).Ticks - Global.GetCorrectDateTime().Ticks;
		int num2 = (int)(num / 10000000L);
		return num2 <= 0;
	}

	private List<ChongZhiFanLiData> FilterChongZhiFanLiXMLDictByTime()
	{
		List<ChongZhiFanLiData> list = new List<ChongZhiFanLiData>();
		List<ChongZhiFanLiData> list2 = new List<ChongZhiFanLiData>();
		Dictionary<int, ChongZhiFanLiData>.Enumerator enumerator = this.fanLiDict.GetEnumerator();
		ChongZhiFanLiData chongZhiFanLiData = null;
		while (enumerator.MoveNext())
		{
			DateTime dateTime;
			dateTime..ctor(Global.GetCorrectDateTime().Year, Global.GetCorrectDateTime().Month, Global.GetCorrectDateTime().Day, Global.GetCorrectDateTime().Hour, Global.GetCorrectDateTime().Minute, Global.GetCorrectDateTime().Second);
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair = enumerator.Current;
			int year = keyValuePair.Value.Data.Year;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair2 = enumerator.Current;
			int month = keyValuePair2.Value.Data.Month;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair3 = enumerator.Current;
			int day = keyValuePair3.Value.Data.Day;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair4 = enumerator.Current;
			int hour = keyValuePair4.Value.BeginTime.Hour;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair5 = enumerator.Current;
			int minute = keyValuePair5.Value.BeginTime.Minute;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair6 = enumerator.Current;
			DateTime dateTime2;
			dateTime2..ctor(year, month, day, hour, minute, keyValuePair6.Value.BeginTime.Second);
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair7 = enumerator.Current;
			int year2 = keyValuePair7.Value.Data.Year;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair8 = enumerator.Current;
			int month2 = keyValuePair8.Value.Data.Month;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair9 = enumerator.Current;
			int day2 = keyValuePair9.Value.Data.Day;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair10 = enumerator.Current;
			int hour2 = keyValuePair10.Value.EndTime.Hour;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair11 = enumerator.Current;
			int minute2 = keyValuePair11.Value.EndTime.Minute;
			KeyValuePair<int, ChongZhiFanLiData> keyValuePair12 = enumerator.Current;
			DateTime dateTime3;
			dateTime3..ctor(year2, month2, day2, hour2, minute2, keyValuePair12.Value.EndTime.Second);
			if (dateTime2 < dateTime)
			{
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair13 = enumerator.Current;
				chongZhiFanLiData = keyValuePair13.Value;
			}
			else if (dateTime2 > dateTime)
			{
				if (dateTime2.Day == dateTime.Day)
				{
					List<ChongZhiFanLiData> list3 = list;
					KeyValuePair<int, ChongZhiFanLiData> keyValuePair14 = enumerator.Current;
					list3.Add(keyValuePair14.Value);
				}
			}
			else
			{
				List<ChongZhiFanLiData> list4 = list;
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair15 = enumerator.Current;
				list4.Add(keyValuePair15.Value);
			}
			if (dateTime2 <= dateTime && dateTime < dateTime3)
			{
				List<ChongZhiFanLiData> list5 = list;
				KeyValuePair<int, ChongZhiFanLiData> keyValuePair16 = enumerator.Current;
				list5.Add(keyValuePair16.Value);
			}
		}
		if (list.Count <= 0 && chongZhiFanLiData != null)
		{
			list.Add(chongZhiFanLiData);
		}
		list2.AddRange(list);
		return list2;
	}

	private void NextItemHandler(object sender = null, DPSelectedItemEventArgs args = null)
	{
		if (this.IsOutOfCfgCount)
		{
			return;
		}
		if (args != null && (args.Flag == this.DeleteFlag || args.Flag == this.RefreshFlag))
		{
			MUDebug.LogError<string>(new string[]
			{
				"Next args.Flag == DeleteFlag || args.Flag == RefreshFlag"
			});
			this.RemoveEndChongZhiFanLiData(this.CurItemIndex);
			this.CurItemIndex = 0;
			this.RecordStateDict.Clear();
			if (this.IsMax)
			{
				this.mBtnLeft.gameObject.SetActive(true);
				this.mBtnRight.gameObject.SetActive(false);
				this.ErJiBak.gameObject.SetActive(false);
			}
			this.mBtnLeft.gameObject.SetActive(false);
			if (!this.IsOutOfCfgCount)
			{
				this.ErJiBak.gameObject.SetActive(true);
			}
			if (args.Flag == this.DeleteFlag && this.mChongZhiFanLiXMLDatas.Count == 1)
			{
				this.item.ResetLblTipTime = Global.GetLang("今日活动已结束");
			}
			else
			{
				if (this.item != null)
				{
					this.item.InitData(this.GetXMLDataByIndex(this.CurItemIndex));
					this.CurrentState = this.item.CurState;
				}
				this.RecordCurrentState(this.CurItemIndex, this.CurrentState);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"Next null null null null null null"
			});
			this.RecordCurrentState(this.CurItemIndex, this.CurrentState);
			this.CurItemIndex++;
			if (this.IsMax)
			{
				this.mBtnLeft.gameObject.SetActive(true);
				this.mBtnRight.gameObject.SetActive(false);
				this.ErJiBak.gameObject.SetActive(false);
			}
			if (!this.IsOutOfCfgCount)
			{
				this.ErJiBak.gameObject.SetActive(true);
			}
			if (this.item != null)
			{
				this.item.InitData(this.GetXMLDataByIndex(this.CurItemIndex));
				this.CurrentState = this.item.CurState;
			}
			if (this.IsLastHaveTimeOrBuyCount())
			{
				this.mBtnLeft.gameObject.SetActive(true);
			}
		}
		this.HideErJiWinodw();
	}

	private void HideErJiWinodw()
	{
		if (this.mChongZhiFanLiXMLDatas != null && this.mChongZhiFanLiXMLDatas.Count == 1)
		{
			this.ErJiBak.gameObject.SetActive(false);
		}
	}

	private void PreviousItem()
	{
		if (this.PreviousItemIndex < 0)
		{
			this.mBtnLeft.gameObject.SetActive(false);
			return;
		}
		this.CurItemIndex--;
		if (this.IsMin)
		{
			this.mBtnLeft.gameObject.SetActive(false);
		}
		this.mBtnRight.gameObject.SetActive(true);
		this.ErJiBak.gameObject.SetActive(true);
		if (this.item != null)
		{
			this.item.InitData(this.GetXMLDataByIndex(this.CurItemIndex));
			this.CurrentState = this.item.CurState;
		}
	}

	private void RecordCurrentState(int index, ChongZhiFanLiState state)
	{
		if (!this.RecordStateDict.ContainsKey(this.CurItemIndex))
		{
			this.RecordStateDict.Add(this.CurItemIndex, state);
		}
		else
		{
			this.RecordStateDict[index] = state;
		}
	}

	private bool IsLastHaveTimeOrBuyCount()
	{
		int num = 0;
		if (this.RecordStateDict.Count > 0)
		{
			foreach (KeyValuePair<int, ChongZhiFanLiState> keyValuePair in this.RecordStateDict)
			{
				if (keyValuePair.Value != ChongZhiFanLiState.End)
				{
					num++;
				}
			}
		}
		return num > 0;
	}

	public ChongZhiFanLiState CurrentState
	{
		get
		{
			return this.mChongZhiFanLiState;
		}
		set
		{
			this.mChongZhiFanLiState = value;
			switch (this.mChongZhiFanLiState)
			{
			case ChongZhiFanLiState.WillBegin:
				if (this.item != null && this.item.ItemConfigData != null)
				{
					this.m_TiShiXianGouCount.text = string.Format(Global.GetLang("本轮账号剩余限购{0}次"), this.item.ItemConfigData.SinglePurchase);
				}
				break;
			case ChongZhiFanLiState.Selling:
				this.RequestFanLiData();
				break;
			case ChongZhiFanLiState.End:
				this.RemoveEndChongZhiFanLiData(this.CurItemIndex);
				this.NextItemHandler(null, null);
				break;
			}
		}
	}

	private void RefreshTiShiXianGouCount(int boughtNum)
	{
		ChongZhiFanLiData xmldataByIndex = this.GetXMLDataByIndex(this.CurItemIndex);
		if (xmldataByIndex == null)
		{
			return;
		}
		int singlePurchase = xmldataByIndex.SinglePurchase;
		int num = singlePurchase - boughtNum;
		this.m_TiShiXianGouCount.text = string.Format(Global.GetLang("本轮账号剩余限购{0}次"), num);
	}

	private ChongZhiFanLiData GetXMLDataByIndex(int index)
	{
		if (this.mChongZhiFanLiXMLDatas.Count <= 0)
		{
			return null;
		}
		return this.mChongZhiFanLiXMLDatas[index];
	}

	private void RemoveEndChongZhiFanLiData(int index)
	{
		if (this.mChongZhiFanLiXMLDatas.Count == 1)
		{
			return;
		}
		this.mChongZhiFanLiXMLDatas.RemoveAt(index);
	}

	private int ChongZhiFanLiXMLDataCount
	{
		get
		{
			return this.mChongZhiFanLiXMLDatas.Count;
		}
	}

	private int CurItemIndex
	{
		get
		{
			return this.mCurItemIndex;
		}
		set
		{
			this.mCurItemIndex = value;
			if (this.mCurItemIndex >= this.ChongZhiFanLiXMLDataCount)
			{
				this.mCurItemIndex = this.ChongZhiFanLiXMLDataCount;
			}
			if (this.mCurItemIndex < 0)
			{
				this.mCurItemIndex = 0;
			}
		}
	}

	private bool IsOutOfCfgCount
	{
		get
		{
			return this.NextItemIndex > this.ChongZhiFanLiXMLDataCount - 1;
		}
	}

	private int PreviousItemIndex
	{
		get
		{
			return this.CurItemIndex - 1;
		}
	}

	private int NextItemIndex
	{
		get
		{
			return this.CurItemIndex + 1;
		}
	}

	private bool IsMax
	{
		get
		{
			return this.CurItemIndex >= this.ChongZhiFanLiXMLDataCount - 1;
		}
	}

	private bool IsMin
	{
		get
		{
			return this.CurItemIndex <= 0;
		}
	}

	private string PlatformType
	{
		get
		{
			string empty = string.Empty;
			return "dl_android";
		}
	}

	private void RequestFanLiData()
	{
		bool flag = HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi);
		if (flag)
		{
			GameInstance.Game.SendDataHuoDongFanLi();
		}
	}

	public void RespondFanLiData(string field)
	{
		if (field == null)
		{
			return;
		}
		string[] array = field.Split(new char[]
		{
			','
		});
		int num = Global.SafeConvertToInt32(array[0]);
		int num2 = Global.SafeConvertToInt32(array[1]);
		int num3 = Global.SafeConvertToInt32(array[2]);
		MUDebug.LogError<string>(new string[]
		{
			string.Concat(new object[]
			{
				"个人已买：",
				num,
				"  总分数：",
				num2,
				"  已购总分数：",
				num3
			})
		});
		if (this.CurrentState == ChongZhiFanLiState.Selling || this.CurrentState == ChongZhiFanLiState.End)
		{
			this.RefreshTiShiXianGouCount(num);
			if (this.item != null)
			{
				this.item.RespondLeftNum(num2, num3);
			}
		}
		else
		{
			this.RefreshTiShiXianGouCount(0);
			if (this.item != null)
			{
				this.item.RespondLeftNum(num2, 0);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("CountDownTomorrow");
		base.OnDestroy();
	}

	public GButton m_Btn;

	public UILabel m_Time;

	public UILabel m_Title1;

	public UILabel m_TiShiXianGouCount;

	public UILabel m_TiShi;

	public Dictionary<int, ChongZhiFanLiData> fanLiDict = new Dictionary<int, ChongZhiFanLiData>();

	public GButton mBtnLeft;

	public GButton mBtnRight;

	public GameObject ErJiBak;

	public UIPanel mPanel;

	private HuoDongChongZhiFanLiItem item;

	private List<ChongZhiFanLiData> mChongZhiFanLiXMLDatas = new List<ChongZhiFanLiData>();

	private int DeleteFlag = 1;

	private int RefreshFlag = 2;

	private Dictionary<int, ChongZhiFanLiState> RecordStateDict = new Dictionary<int, ChongZhiFanLiState>();

	private ChongZhiFanLiState mChongZhiFanLiState;

	private int mCurItemIndex;
}
