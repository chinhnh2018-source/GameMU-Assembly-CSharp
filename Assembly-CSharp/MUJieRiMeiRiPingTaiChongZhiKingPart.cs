using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MUJieRiMeiRiPingTaiChongZhiKingPart : UserControl
{
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.IsShowRootRedpoint = false;
		this.ItemCollection = this.rewardList.ItemsSource;
		this.InitBakImg();
	}

	private void InitBakImg()
	{
		this.bak.ImageURL = string.Format("NetImages/GameRes/Images/VIPJieRiLiBao/danbichongzhipeitu.jpg", new object[0]);
	}

	private void InitPopuList()
	{
		this.mPopupItemDict.Clear();
		this.mUIPopupList.items.Clear();
		this.mUIPopupList.textScale = 0.6f;
		this.mUIPopupList.onLoadItemFinish = new UIPopupList.OnClickAction(this.OnLoadPopupChildrenItemsFinish);
		for (int i = 0; i < this.GetJieriDays(); i++)
		{
			string text = string.Format(Global.GetLang("       第  {0}  天"), i + 1);
			this.mUIPopupList.items.Add(text);
			if (!this.mPopupItemDict.ContainsKey(text))
			{
				this.mPopupItemDict.Add(text, i + 1);
			}
		}
		this.mUIPopupList.onSelectionChange = new UIPopupList.OnSelectionChange(this.SelectChange);
		this.mUIPopupList.selection = string.Format(Global.GetLang("       第  {0}  天"), this.GetJieriDays());
		this.mUIPopupList.textLabel.color = Color.yellow;
		this.mUIPopupList.hasHighlightLable = true;
		this.mUIPopupList.textColor = Color.white;
		this.mUIPopupList.highLightIndex = this.GetJieriDays() - 1;
	}

	private void SelectChange(string item)
	{
		if (this.mPopupItemDict.ContainsKey(item))
		{
			int num = this.mPopupItemDict[item];
			this.mUIPopupList.highLightIndex = num - 1;
			this.RequestInitGoodsInfo(num);
		}
	}

	private void OnLoadPopupChildrenItemsFinish()
	{
		this.RedPointsObj.Clear();
		List<UILabel> popupLblChildren = this.mUIPopupList.GetPopupLblChildren();
		for (int i = 0; i < popupLblChildren.Count; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(popupLblChildren[i].transform.parent);
			gameObject.transform.localPosition = new Vector3(144f, (float)(-4 + i * -23), popupLblChildren[i].transform.localPosition.z);
			gameObject.transform.localRotation = popupLblChildren[i].transform.localRotation;
			gameObject.transform.localScale = popupLblChildren[i].transform.localScale;
			UISprite uisprite = gameObject.AddComponent<UISprite>();
			uisprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("redPoint", true));
			if (uisprite)
			{
				uisprite.spriteName = "GanTanHao";
			}
			uisprite.MakePixelPerfect();
			gameObject.transform.name = "RedPoint" + (i + 1);
			gameObject.SetActive(false);
			this.RedPointsObj.Add(gameObject);
		}
		if (this.mRedPointDict != null && this.mRedPointDict.Count > 0)
		{
			foreach (KeyValuePair<int, int> keyValuePair in this.mRedPointDict)
			{
				int index = keyValuePair.Key - 1;
				Dictionary<int, int>.Enumerator enumerator;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				this.SetPopupChildrenRedPoint(index, keyValuePair2.Value == 0);
			}
		}
		this.IsShowRedPoint();
	}

	public void InitData(string strXML = null)
	{
		this.DayList.Clear();
		this.ItemsCfgDict.Clear();
		XElement xelement;
		if (string.IsNullOrEmpty(strXML))
		{
			xelement = Global.GetGameResXml("Config/JieRiGifts/JieRiMeiRiChongZhiWang.xml");
		}
		else
		{
			xelement = XElement.Parse(strXML);
		}
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
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间："),
			"ffffff",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("领取时间："),
			"ffffff",
			this.awardStartStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.awardEndStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容："),
			"ffffff",
			xelementAttributeStr5
		});
		this.goodList = Global.GetXElementList(xelement, "Award");
		for (int i = 0; i < this.goodList.Count; i++)
		{
			XElement xelement4 = this.goodList[i];
			if (xelement4 != null)
			{
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "Day");
				if (!this.DayList.Contains(xelementAttributeInt2))
				{
					this.DayList.Add(xelementAttributeInt2);
				}
			}
		}
		this.InitAllItemsDataByCfg(this.DayList);
		this.InitPopuList();
	}

	private bool IsMaxDay { get; set; }

	public int GetJieriDays()
	{
		int activitySumDays = this.ActivitySumDays;
		int num = (Global.GetCorrectDateTime() - this.StartDay).Days + 1;
		if (num > activitySumDays)
		{
			this.IsMaxDay = true;
			num = activitySumDays;
		}
		else
		{
			this.IsMaxDay = false;
		}
		return num;
	}

	private void InitAllItemsDataByCfg(List<int> dayList)
	{
		for (int i = 0; i < dayList.Count; i++)
		{
			List<MeiRiChongZhiKingItemData> list = new List<MeiRiChongZhiKingItemData>();
			for (int j = 0; j < this.goodList.Count; j++)
			{
				XElement xelement = this.goodList[j];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Day");
					if (dayList[i] == xelementAttributeInt)
					{
						MeiRiChongZhiKingItemData meiRiChongZhiKingItemData = new MeiRiChongZhiKingItemData(xelement, xelementAttributeInt);
						list.Add(meiRiChongZhiKingItemData);
					}
				}
			}
			if (!this.ItemsCfgDict.ContainsKey(dayList[i]))
			{
				this.ItemsCfgDict.Add(dayList[i], list);
			}
		}
	}

	private void InitItemData(List<MeiRiChongZhiKingItemData> dataList, List<InputKingPaiHangData> paiHangList, int flag)
	{
		this.CacheItemsDict.Clear();
		this.ItemCollection.Clear();
		int num = -1;
		int i;
		for (i = 0; i < dataList.Count; i++)
		{
			MUJieRiMeiRiPingTaiChongZhiKingItem mujieRiMeiRiPingTaiChongZhiKingItem = U3DUtils.NEW<MUJieRiMeiRiPingTaiChongZhiKingItem>();
			this.ItemCollection.Add(mujieRiMeiRiPingTaiChongZhiKingItem);
			mujieRiMeiRiPingTaiChongZhiKingItem.Id = i + 1;
			mujieRiMeiRiPingTaiChongZhiKingItem.Day = dataList[i].Day;
			InputKingPaiHangData inputKingPaiHangData = null;
			if (paiHangList != null)
			{
				inputKingPaiHangData = paiHangList.Find((InputKingPaiHangData result) => result.PaiHang == i + 1);
			}
			string text = string.Empty;
			mujieRiMeiRiPingTaiChongZhiKingItem.Rank = (i + 1).ToString();
			if (inputKingPaiHangData == null)
			{
				mujieRiMeiRiPingTaiChongZhiKingItem.ZoneID = -1;
				mujieRiMeiRiPingTaiChongZhiKingItem.RoleName = string.Empty;
				mujieRiMeiRiPingTaiChongZhiKingItem.XmlID = this.GetXmlIdByRankIdAndDay(this.nullRank, mujieRiMeiRiPingTaiChongZhiKingItem.Day);
			}
			else if (mujieRiMeiRiPingTaiChongZhiKingItem.Rank == inputKingPaiHangData.PaiHang.ToString())
			{
				mujieRiMeiRiPingTaiChongZhiKingItem.ZoneID = inputKingPaiHangData.MaxLevelRoleZoneID;
				mujieRiMeiRiPingTaiChongZhiKingItem.RoleName = inputKingPaiHangData.MaxLevelRoleName;
				num = inputKingPaiHangData.PaiHangValue;
				mujieRiMeiRiPingTaiChongZhiKingItem.XmlID = this.GetXmlIdByRankIdAndDay(inputKingPaiHangData.PaiHang, mujieRiMeiRiPingTaiChongZhiKingItem.Day);
				text = inputKingPaiHangData.UserID;
			}
			else
			{
				mujieRiMeiRiPingTaiChongZhiKingItem.ZoneID = -1;
				mujieRiMeiRiPingTaiChongZhiKingItem.RoleName = string.Empty;
				num = 0;
				mujieRiMeiRiPingTaiChongZhiKingItem.XmlID = this.GetXmlIdByRankIdAndDay(this.nullRank, mujieRiMeiRiPingTaiChongZhiKingItem.Day);
				text = string.Empty;
			}
			mujieRiMeiRiPingTaiChongZhiKingItem.Need = dataList[i].MinYuanBao;
			if (num >= mujieRiMeiRiPingTaiChongZhiKingItem.Need && inputKingPaiHangData != null && Global.Data.UserID == inputKingPaiHangData.UserID && mujieRiMeiRiPingTaiChongZhiKingItem.Rank == inputKingPaiHangData.PaiHang.ToString())
			{
				int intSomeBit = Global.GetIntSomeBit(flag, this._thisDay - 1);
				if (intSomeBit == 1)
				{
					mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else if (!this.IsMaxDay && intSomeBit == 0 && this._thisDay < this.GetJieriDays() && Global.Data.UserID == text)
				{
					mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
				else if (!this.IsMaxDay && this._thisDay == this.GetJieriDays() && Global.Data.UserID == text)
				{
					mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				}
				else if (this.IsMaxDay)
				{
					if (intSomeBit == 0 && this._thisDay <= this.GetJieriDays() && Global.Data.UserID == text)
					{
						mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
					}
					else
					{
						mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.OverTime;
					}
				}
				else
				{
					mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.OverTime;
				}
			}
			else
			{
				mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				if (this._thisDay < this.GetJieriDays() || this.IsMaxDay)
				{
					mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.OverTime;
				}
			}
			Super.LoadGoodsList(dataList[i].GoodsID, mujieRiMeiRiPingTaiChongZhiKingItem.ItemCollection);
			Super.LoadOtherGoodsList(dataList[i].GoodsThr, mujieRiMeiRiPingTaiChongZhiKingItem.ItemCollection, dataList[i].EffectiveTime);
			UIPanel component = mujieRiMeiRiPingTaiChongZhiKingItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			if (!this.CacheItemsDict.ContainsKey(mujieRiMeiRiPingTaiChongZhiKingItem.Day * 1000 + Global.SafeConvertToInt32(mujieRiMeiRiPingTaiChongZhiKingItem.Rank)))
			{
				this.CacheItemsDict.Add(mujieRiMeiRiPingTaiChongZhiKingItem.Day * 1000 + Global.SafeConvertToInt32(mujieRiMeiRiPingTaiChongZhiKingItem.Rank), mujieRiMeiRiPingTaiChongZhiKingItem);
			}
		}
	}

	private void SetPopupChildrenRedPoint(int index, bool isShow = true)
	{
		if (this.RedPointsObj != null && this.RedPointsObj.Count > 0 && index < this.RedPointsObj.Count)
		{
			GameObject gameObject = this.RedPointsObj[index];
			NGUITools.SetActive(gameObject, isShow);
		}
	}

	private void IsShowRedPoint()
	{
		int count = this.RedPointsObj.Count;
		if (this.RedPointsObj != null && count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				if (this.RedPointsObj[i] != null && this.RedPointsObj[i].activeSelf)
				{
					this.IsShowRootRedpoint = true;
					break;
				}
			}
		}
	}

	private bool IsShowRootRedpoint
	{
		get
		{
			return this.isRootRedPoint;
		}
		set
		{
			this.isRootRedPoint = value;
			this.mRootRedpoint.gameObject.SetActive(value);
		}
	}

	private int ActivitySumDays
	{
		get
		{
			DateTime dateTime;
			DateTime.TryParse(this.startTimeStr, ref dateTime);
			DateTime dateTime2;
			DateTime.TryParse(this.awardEndStr, ref dateTime2);
			return (dateTime2 - dateTime).Days;
		}
	}

	private DateTime StartDay
	{
		get
		{
			DateTime result;
			DateTime.TryParse(this.startTimeStr, ref result);
			return result;
		}
	}

	private int GetXmlIdByRankIdAndDay(int rankId, int day)
	{
		if (this.goodList != null)
		{
			for (int i = 0; i < this.goodList.Count; i++)
			{
				XElement xelement = this.goodList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Ranking");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Day");
					if (xelementAttributeInt == rankId && xelementAttributeInt2 == day)
					{
						return Global.GetXElementAttributeInt(xelement, "ID");
					}
				}
			}
		}
		return -1;
	}

	private void GetRankIdAndDayByXmlId(int XmlId, out int rankId, out int day)
	{
		rankId = -1;
		day = -1;
		if (this.goodList != null)
		{
			for (int i = 0; i < this.goodList.Count; i++)
			{
				XElement xelement = this.goodList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Ranking");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Day");
					if (xelementAttributeInt == XmlId)
					{
						rankId = xelementAttributeInt2;
						day = xelementAttributeInt3;
						break;
					}
				}
			}
		}
	}

	private void RefreshRedPoint(JieriPlatChargeKingEverydayData data)
	{
		string userId = Global.Data.UserID;
		int num = 0;
		if (data != null)
		{
			int num2 = (int)data.hasgettimes;
			Dictionary<int, List<InputKingPaiHangData>> paiHangDict = data.PaiHangDict;
			if (paiHangDict != null && paiHangDict.Count > 0)
			{
				Dictionary<int, List<InputKingPaiHangData>>.Enumerator enumerator = paiHangDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int resource = num2;
					KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair = enumerator.Current;
					int intSomeBit = Global.GetIntSomeBit(resource, keyValuePair.Key - 1);
					KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair2 = enumerator.Current;
					List<InputKingPaiHangData> value = keyValuePair2.Value;
					InputKingPaiHangData inputKingPaiHangData = value.Find((InputKingPaiHangData result) => result.UserID == userId);
					if (inputKingPaiHangData != null)
					{
						KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair3 = enumerator.Current;
						if (keyValuePair3.Key > this.GetJieriDays())
						{
							KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair4 = enumerator.Current;
							this.SaveRedPoint(keyValuePair4.Key, 1);
						}
						else
						{
							int paiHangValue = inputKingPaiHangData.PaiHangValue;
							Dictionary<int, List<MeiRiChongZhiKingItemData>> itemsCfgDict = this.ItemsCfgDict;
							KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair5 = enumerator.Current;
							List<MeiRiChongZhiKingItemData> list = itemsCfgDict[keyValuePair5.Key];
							int num3 = inputKingPaiHangData.PaiHang - 1;
							if (num3 < list.Count)
							{
								MeiRiChongZhiKingItemData meiRiChongZhiKingItemData = list[num3];
								if (paiHangValue >= meiRiChongZhiKingItemData.MinYuanBao)
								{
									if (intSomeBit == 1)
									{
										KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair6 = enumerator.Current;
										this.SaveRedPoint(keyValuePair6.Key, 1);
									}
									else
									{
										if (intSomeBit == 0)
										{
											KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair7 = enumerator.Current;
											if (keyValuePair7.Key < this.GetJieriDays() || this.IsMaxDay)
											{
												KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair8 = enumerator.Current;
												this.SaveRedPoint(keyValuePair8.Key, 0);
												num++;
												continue;
											}
										}
										KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair9 = enumerator.Current;
										this.SaveRedPoint(keyValuePair9.Key, 1);
									}
								}
								else
								{
									KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair10 = enumerator.Current;
									this.SaveRedPoint(keyValuePair10.Key, 1);
								}
							}
						}
					}
					else
					{
						KeyValuePair<int, List<InputKingPaiHangData>> keyValuePair11 = enumerator.Current;
						this.SaveRedPoint(keyValuePair11.Key, 1);
					}
				}
			}
		}
		this.IsShowRootRedpoint = (num > 0);
	}

	private void SaveRedPoint(int day, int flag)
	{
		if (this.mRedPointDict.ContainsKey(day))
		{
			this.mRedPointDict[day] = flag;
		}
		else
		{
			this.mRedPointDict.Add(day, flag);
		}
	}

	private void RefreshRootPoint()
	{
		int num = 0;
		if (this.mRedPointDict != null && this.mRedPointDict.Count > 0)
		{
			foreach (KeyValuePair<int, int> keyValuePair in this.mRedPointDict)
			{
				if (keyValuePair.Value == 0)
				{
					num++;
				}
			}
		}
		this.IsShowRootRedpoint = (num > 0);
	}

	private void ResetPopupListIndex()
	{
		if (this.IsFirstOpen && this.IsShowRootRedpoint && this.mRedPointDict != null && this.mRedPointDict.Count > 0)
		{
			int num = -1;
			foreach (KeyValuePair<int, int> keyValuePair in this.mRedPointDict)
			{
				if (keyValuePair.Value == 0)
				{
					Dictionary<int, int>.Enumerator enumerator;
					KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
					num = keyValuePair2.Key;
					break;
				}
			}
			if (num > 0)
			{
				this.mUIPopupList.selection = string.Format(Global.GetLang("       第  {0}  天"), num);
				this.mUIPopupList.highLightIndex = num - 1;
			}
			if (this.IsFirstOpen)
			{
				this.IsFirstOpen = false;
			}
		}
	}

	private bool IsDelayFiveMinutes()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		bool result;
		if (correctDateTime.Day != this.lastDateTime.Day)
		{
			this.lastDateTime = this.lastDateTime.AddMinutes(5.0);
			result = (correctDateTime >= this.lastDateTime);
		}
		else
		{
			result = true;
		}
		return result;
	}

	public void RequestInitGoodsInfo(int day)
	{
		if (this.ItemsCfgDict.ContainsKey(day))
		{
			GameInstance.Game.RequestMeiRiChongZhiKingInfo();
			this._thisDay = day;
		}
	}

	public void RespondLoadItemData(JieriPlatChargeKingEverydayData data)
	{
		if (data == null)
		{
			this.InitItemData(this.ItemsCfgDict[this._thisDay], null, 0);
		}
		else
		{
			this.RefreshRedPoint(data);
			int flag = (int)data.hasgettimes;
			Dictionary<int, List<InputKingPaiHangData>> paiHangDict = data.PaiHangDict;
			if (paiHangDict != null && paiHangDict.Count > 0)
			{
				this.lastDateTime = Global.GetCorrectDateTime();
				List<InputKingPaiHangData> paiHangList = null;
				if (paiHangDict.ContainsKey(this._thisDay))
				{
					paiHangList = paiHangDict[this._thisDay];
				}
				this.InitItemData(this.ItemsCfgDict[this._thisDay], paiHangList, flag);
			}
			else
			{
				this.InitItemData(this.ItemsCfgDict[this._thisDay], null, flag);
			}
			this.ResetPopupListIndex();
		}
	}

	public void RespondCompletedInfo(int result, int roleID, int xmlId)
	{
		if (result <= 0)
		{
			this.ErrorInfo(result);
			return;
		}
		int num = 0;
		int num2 = 0;
		this.GetRankIdAndDayByXmlId(xmlId, out num, out num2);
		MUJieRiMeiRiPingTaiChongZhiKingItem mujieRiMeiRiPingTaiChongZhiKingItem = this.CacheItemsDict[num2 * 1000 + num];
		if (mujieRiMeiRiPingTaiChongZhiKingItem != null)
		{
			mujieRiMeiRiPingTaiChongZhiKingItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			this.SaveRedPoint(num2, 1);
			this.RefreshRootPoint();
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励成功"), new object[0]), 0, -1, -1, 0);
	}

	private void ErrorInfo(int result)
	{
		if (result <= 0)
		{
			if (result == -10005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你已经领取过了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10006)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("活动期间充值额度为0，不能领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10007)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领取条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("现在不是领取时间"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
	}

	protected override void OnDestroy()
	{
		this.CacheItemsDict.Clear();
		this.IsFirstOpen = false;
		base.OnDestroy();
	}

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public TextBlock descText;

	public ListBox rewardList;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	public ShowNetImage bak;

	public UIPopupList mUIPopupList;

	private ObservableCollection _ItemCollection;

	public UISprite mRootRedpoint;

	private Dictionary<int, int> mRedPointDict = new Dictionary<int, int>();

	private bool IsFirstOpen = true;

	private Dictionary<string, int> mPopupItemDict = new Dictionary<string, int>();

	private List<GameObject> RedPointsObj = new List<GameObject>();

	private List<XElement> goodList;

	private List<int> DayList = new List<int>();

	private Dictionary<int, List<MeiRiChongZhiKingItemData>> ItemsCfgDict = new Dictionary<int, List<MeiRiChongZhiKingItemData>>();

	private int nullRank;

	private Dictionary<int, MUJieRiMeiRiPingTaiChongZhiKingItem> CacheItemsDict = new Dictionary<int, MUJieRiMeiRiPingTaiChongZhiKingItem>();

	private bool isRootRedPoint;

	private int _thisDay;

	private DateTime lastDateTime = default(DateTime);
}
