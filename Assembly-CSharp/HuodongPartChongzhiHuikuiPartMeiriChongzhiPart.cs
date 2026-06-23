using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HuodongPartChongzhiHuikuiPartMeiriChongzhiPart : UserControl
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
		this.LoadBanner();
		this.ItemCollection = this.List.ItemsSource;
		if (null != this.draggablePanel)
		{
			this.defaultPosition = this.draggablePanel.transform.localPosition;
			this.clipRange = this.draggablePanel.clipRange;
		}
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 27);
	}

	private void OnEnable()
	{
		this.ResetDraggablePanel();
	}

	private void ResetDraggablePanel()
	{
		if (null != this.draggablePanel)
		{
			this.draggablePanel.transform.localPosition = this.defaultPosition;
			this.draggablePanel.clipRange = this.clipRange;
		}
	}

	public void LoadBanner()
	{
		if (null != this.banner)
		{
			this.banner.URL = string.Format("NetImages/GameRes/Images/Fuli/{0}", (!Global.IsInWeekendRechargePeriod()) ? "chongzhiHuikui_02.jpg" : "chongzhiHuikui_weekend.jpg");
		}
	}

	public void ReSetUIByCommd(string[] strArr)
	{
		this.RefreshDepositListState(strArr);
		if (!this.loadWeekendOnce)
		{
			this.loadWeekendOnce = true;
			this.GetWeekendAwardInfo();
		}
	}

	private void RefreshDepositListState(string[] strArr)
	{
		bool flag = Global.IsInWeekendRechargePeriod();
		string[] array = strArr[0].Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			int levelIndex = this.GetLevelIndex(i);
			GameObject at = this.ItemCollection.GetAt(levelIndex);
			if (null != at)
			{
				HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem component = at.GetComponent<HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem>();
				component.LingquFlag = Convert.ToInt32(array[i]);
				if (component.LingquFlag == 0)
				{
					component.m_LblShowText.effectStyle = 1;
					component.m_LblShowText.text = Global.GetColorStringForNGUIText(new object[]
					{
						this.fontColor,
						((!flag) ? Global.GetLang("今日充值") : Global.GetLang("充值满")) + component.TxtValue.Text
					});
					component.m_LblShowText.gameObject.SetActive(true);
					component.m_SprZuanShi.gameObject.SetActive(true);
					component.m_Condition.gameObject.SetActive(false);
				}
			}
		}
	}

	public void InitPartData(string xmlPath)
	{
		XElement isolateResXml = Global.GetIsolateResXml(xmlPath);
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "GiftList");
		if (xelement == null)
		{
			return;
		}
		this.TxtHint.Text = Global.GetXElementAttributeStr(xelement, "Description");
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Award");
		this.state = new StateObject01();
		this.state.AwardID = 5000;
		this.state.LingquState = 1;
		this.ShowList(xelementList);
	}

	private void ShowList(List<XElement> xmlList)
	{
		if (xmlList == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		int num = (this.state != null) ? this.state.AwardID : 0;
		for (int i = 0; i < xmlList.Count; i++)
		{
			XElement xelement = xmlList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem item = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartMeiriChongzhiPartItem>();
			item.m_nJiangLiID = xelementAttributeInt;
			item.TxtValue.Text = StringUtil.substitute(Global.GetLang("{0}"), new object[]
			{
				Global.GetXElementAttributeStr(xelement, "MinYuanBao")
			});
			item.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			item.SubmitBtn.BtnTag = xelementAttributeInt.ToString();
			item.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (item.LingquFlag != 1)
				{
					return;
				}
				GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 27, Convert.ToInt32(item.SubmitBtn.BtnTag));
			};
			if (!this.isLingqu(i))
			{
				if (num >= Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinYuanBao")))
				{
				}
			}
			if (item.LingquFlag == 0)
			{
				item.m_LblShowText.text = Global.GetLang("今日充值") + item.TxtValue.Text;
				item.m_LblShowText.gameObject.SetActive(true);
				item.m_Condition.gameObject.SetActive(false);
			}
			this.ItemCollection.Add(item);
		}
	}

	private bool isLingqu(int index)
	{
		return false;
	}

	private IEnumerator LoadWeekendExtraAwardList(int[] levelIndexes, string[] goodsStrings)
	{
		if (levelIndexes == null || levelIndexes.Length <= 0)
		{
			yield break;
		}
		if (goodsStrings == null || goodsStrings.Length <= 0)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.3f);
		int count = Mathf.Min(this.ItemCollection.Count, levelIndexes.Length);
		int insertPos = 0;
		int addCount = 0;
		int counter = -1;
		for (int i = 0; i < count; i++)
		{
			counter++;
			if (counter % 2 == 0)
			{
				yield return null;
			}
			insertPos = levelIndexes[i] + addCount;
			string goodIDs = goodsStrings[i];
			WeekendDepositExtraAwardItem item = U3DUtils.NEW<WeekendDepositExtraAwardItem>();
			item.index = insertPos;
			item.goodsIDs = goodIDs;
			this.ItemCollection.Insert(insertPos, item);
			addCount++;
		}
		yield break;
	}

	private void ClearWeekendItems()
	{
		if (null == this.List)
		{
			return;
		}
		ObservableCollection itemsSource = this.List.ItemsSource;
		WeekendDepositExtraAwardItem[] componentsInChildren = this.List.GetComponentsInChildren<WeekendDepositExtraAwardItem>();
		if (componentsInChildren == null)
		{
			return;
		}
		int num = 0;
		foreach (WeekendDepositExtraAwardItem weekendDepositExtraAwardItem in componentsInChildren)
		{
			if (null != weekendDepositExtraAwardItem)
			{
				itemsSource.RemoveAt(weekendDepositExtraAwardItem.index - num);
				num++;
			}
		}
	}

	private void GetWeekendAwardInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetWeekendAwardInfo();
	}

	public void SetWeekendAwardInfo(string awardInfo)
	{
		Super.HideNetWaiting();
		this.ClearWeekendItems();
		string[] goodsStrings = null;
		this.ParseWeekendAwardInfo(awardInfo, out this.levelIndexes, out goodsStrings);
		base.StartCoroutine<bool>(this.LoadWeekendExtraAwardList(this.levelIndexes, goodsStrings));
	}

	private bool ParseWeekendAwardInfo(string awardInfo, out int[] levelIndexes, out string[] goodsStrings)
	{
		levelIndexes = null;
		goodsStrings = null;
		if (string.IsNullOrEmpty(awardInfo) || awardInfo.Equals("0"))
		{
			return false;
		}
		this.GetWeekendAwardConfig();
		string[] array = awardInfo.Split(new char[]
		{
			'|'
		});
		if (array == null || array.Length <= 0)
		{
			return false;
		}
		levelIndexes = new int[array.Length];
		goodsStrings = new string[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			string[] array2 = text.Split(new char[]
			{
				'$'
			});
			if (array2 != null && array2.Length >= 3)
			{
				int awardID = Global.SafeConvertToInt32(array2[1]);
				string[] array3 = array2[2].Split(new char[]
				{
					','
				});
				if (array3 != null && array3.Length > 0)
				{
					string text2 = string.Empty;
					for (int j = 0; j < array3.Length; j++)
					{
						string goodsStringByAwardID = this.GetGoodsStringByAwardID(awardID, Global.SafeConvertToInt32(array3[j]));
						if (!string.IsNullOrEmpty(goodsStringByAwardID))
						{
							text2 = text2 + goodsStringByAwardID + "|";
						}
					}
					if (!string.IsNullOrEmpty(text2))
					{
						levelIndexes[i] = Global.SafeConvertToInt32(array2[0]);
						goodsStrings[i] = text2;
					}
				}
			}
		}
		return true;
	}

	public void ReloadDailyRewards()
	{
		this.loadWeekendOnce = false;
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 27);
	}

	private int GetLevelIndex(int oriIndex)
	{
		if (this.levelIndexes == null || this.levelIndexes.Length <= 0)
		{
			return oriIndex;
		}
		int num = 0;
		for (int i = 0; i < this.levelIndexes.Length; i++)
		{
			int num2 = this.levelIndexes[i];
			if (num2 <= oriIndex)
			{
				num++;
			}
		}
		return oriIndex + num;
	}

	private string GetGoodsStringByAwardID(int awardID, int ID)
	{
		if (this.dic_weekendGoods == null || this.dic_weekendGoods.Count <= 0)
		{
			return string.Empty;
		}
		string result = null;
		this.dic_weekendGoods.TryGetValue(awardID + "_" + ID, ref result);
		return result;
	}

	private void GetWeekendAwardConfig()
	{
		string xmlName = "Config/Gifts/ZhouMoChongZhi.xml";
		XElement isolateResXml = Global.GetIsolateResXml(xmlName);
		if (isolateResXml == null)
		{
			return;
		}
		if (this.dic_weekendGoods == null || this.dic_weekendGoods.Count <= 0)
		{
			this.dic_weekendGoods = new Dictionary<string, string>();
		}
		this.dic_weekendGoods.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "ZhouMoChongZhi");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			List<XElement> xelementList2 = Global.GetXElementList(xelement, "Award");
			for (int j = 0; j < xelementList2.Count; j++)
			{
				XElement xelement2 = xelementList2[j];
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "ID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Goods");
				if (!this.dic_weekendGoods.ContainsKey(xelementAttributeInt + "_" + xelementAttributeInt2))
				{
					this.dic_weekendGoods.Add(xelementAttributeInt + "_" + xelementAttributeInt2, xelementAttributeStr);
				}
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox List;

	public TextBlock TxtHint;

	public UIPanel draggablePanel;

	private Vector3 defaultPosition = Vector3.zero;

	private Vector4 clipRange = Vector4.zero;

	public ShowNetImage banner;

	private StateObject01 state;

	private Dictionary<string, string> dic_weekendGoods;

	private int[] levelIndexes;

	private bool loadWeekendOnce;

	private string fontColor = "f2e0c5";

	private ObservableCollection _ItemCollection;
}
