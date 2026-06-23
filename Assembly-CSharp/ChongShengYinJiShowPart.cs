using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiShowPart : UserControl
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
		this.InitTextInPrefabs();
		this.ItemCollection = this.mListBox.Items;
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.mLblTitle.Text = Global.GetLang("属性预览");
		this.mLblSumTitle.Text = Global.GetLang("印记点数：");
		this.mLblUsedTitle.Text = Global.GetLang("已用点数：");
		this.mBtnChange.Text = Global.GetLang("切换印记");
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.mBtnYinJi1.gameObject).onClick = delegate(GameObject s)
		{
			this.mCurrentTabId = this.Left;
			this.SetInitPart = this.Left;
		};
		UIEventListener.Get(this.mBtnYinJi2.gameObject).onClick = delegate(GameObject s)
		{
			this.mCurrentTabId = this.Right;
			this.SetInitPart = this.Right;
		};
		this.mBtnChange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChangeHandler != null)
			{
				this.ChangeHandler(null, null);
			}
		};
		this.mBtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowHelpPart();
		};
		this.mYinJiInfoPanel.OnClickHandler = delegate(int s)
		{
		};
		this.IsShowInfoPanel = false;
	}

	private long LeftCount(int usedPoint)
	{
		return Global.Data.roleData.MoneyData[151] - (long)usedPoint;
	}

	public void InitValue(RebornStampData data)
	{
		if (data == null)
		{
			return;
		}
		Global.Data.roleData.RebornYinJi = data;
		ChongShengYinJiData.ResetChongShengYinJiEffect();
		this.UsedYinJiCount = data.UsePoint;
		this.LeftYinJiCount = this.LeftCount(data.UsePoint);
		List<int> stampInfo = data.StampInfo;
		if (stampInfo == null || stampInfo.Count <= 0)
		{
			return;
		}
		this.DictTabYinJi.Clear();
		this.YinJi1List = stampInfo.GetRange(0, 8);
		this.YinJi2List = stampInfo.GetRange(8, stampInfo.Count - 8);
		this.DictTabYinJi[this.Left] = this.GetYinJiDatas(this.YinJi1List);
		this.LeftZhuYinJiName = this.DictTabYinJi[this.Left][0].Type;
		this.DictTabYinJi[this.Right] = this.GetYinJiDatas(this.YinJi2List);
		this.RightZhuYinJi2Name = this.DictTabYinJi[this.Right][0].Type;
		this.ShowYinJiInfo(this.mCurrentTabId);
		this.ShowYinJiInfo(this.Right);
		this.mCurrentTabId = this.Left;
		this.SetInitPart = this.Left;
	}

	private int LeftZhuYinJiName
	{
		get
		{
			return this.mLeftZhuYinJiName;
		}
		set
		{
			this.mLeftZhuYinJiName = value;
			this.mBtnYinJi1.spriteName = this.Names[value - 1];
		}
	}

	private int RightZhuYinJi2Name
	{
		get
		{
			return this.mRightZhuYinJi2Name;
		}
		set
		{
			this.mRightZhuYinJi2Name = value;
			this.mBtnYinJi2.spriteName = this.Names[value - 1];
		}
	}

	private int SetInitPart
	{
		set
		{
			if (value == this.Left)
			{
				this.mEffect.localEulerAngles = new Vector3(0f, 0f, 0f);
				this.mLeftBtnBg.spriteName = this.BtnBgs[0];
				this.mRightBtnBg.spriteName = this.BtnBgs[1];
				this.mLeftImg.URL = this.HighLightBak;
				this.mRightImg.URL = this.NormalBak;
				NGUITools.SetActive(this.mLeftMask, false);
				NGUITools.SetActive(this.mRightMask, true);
				this.mBtnYinJi1.spriteName = this.Names[this.LeftZhuYinJiName - 1];
				this.mBtnYinJi2.spriteName = this.DisableNames[this.RightZhuYinJi2Name - 1];
				List<ChongShengYinJiShowItem> list = null;
				if (this.CacheItemsDict.TryGetValue(this.Left, ref list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						ChongShengYinJiShowItem chongShengYinJiShowItem = list[i];
						chongShengYinJiShowItem.IsDisable = true;
					}
				}
				List<ChongShengYinJiShowItem> list2 = null;
				if (this.CacheItemsDict.TryGetValue(this.Right, ref list2))
				{
					for (int j = 0; j < list2.Count; j++)
					{
						ChongShengYinJiShowItem chongShengYinJiShowItem2 = list2[j];
						chongShengYinJiShowItem2.IsDisable = false;
					}
				}
			}
			else
			{
				this.mEffect.localEulerAngles = new Vector3(0f, 180f, 0f);
				this.mLeftBtnBg.spriteName = this.BtnBgs[1];
				this.mRightBtnBg.spriteName = this.BtnBgs[0];
				this.mLeftImg.URL = this.NormalBak;
				this.mRightImg.URL = this.HighLightBak;
				NGUITools.SetActive(this.mLeftMask, true);
				NGUITools.SetActive(this.mRightMask, false);
				this.mBtnYinJi1.spriteName = this.DisableNames[this.LeftZhuYinJiName - 1];
				this.mBtnYinJi2.spriteName = this.Names[this.RightZhuYinJi2Name - 1];
				List<ChongShengYinJiShowItem> list3 = null;
				if (this.CacheItemsDict.TryGetValue(this.Left, ref list3))
				{
					for (int k = 0; k < list3.Count; k++)
					{
						ChongShengYinJiShowItem chongShengYinJiShowItem3 = list3[k];
						chongShengYinJiShowItem3.IsDisable = false;
					}
				}
				List<ChongShengYinJiShowItem> list4 = null;
				if (this.CacheItemsDict.TryGetValue(this.Right, ref list4))
				{
					for (int l = 0; l < list4.Count; l++)
					{
						ChongShengYinJiShowItem chongShengYinJiShowItem4 = list4[l];
						chongShengYinJiShowItem4.IsDisable = true;
					}
				}
			}
			this.UpdateShuXing();
		}
	}

	private long LeftYinJiCount
	{
		get
		{
			return this.mLeftYinJiCount;
		}
		set
		{
			this.mLeftYinJiCount = value;
			this.mLblSumCount.Text = value.ToString();
		}
	}

	private int UsedYinJiCount
	{
		set
		{
			this.mLblUsedCount.Text = value.ToString();
		}
	}

	private void ShowYinJiInfo(int type)
	{
		if (this.DictTabYinJi.Count <= 0)
		{
			return;
		}
		List<YinJiData> list = this.DictTabYinJi[type];
		List<ChongShengYinJiShowItem> list2 = null;
		if (this.CacheItemsDict.TryGetValue(type, ref list2))
		{
			for (int i = 0; i < list2.Count; i++)
			{
				ChongShengYinJiShowItem chongShengYinJiShowItem = list2[i];
				chongShengYinJiShowItem.InitValue(list[i], list[0].Type);
				chongShengYinJiShowItem.ClickHandler = new Action<object, YinJiData>(this.OnClickHandler);
			}
		}
		else
		{
			List<ChongShengYinJiShowItem> list3 = new List<ChongShengYinJiShowItem>();
			for (int j = 0; j < list.Count; j++)
			{
				ChongShengYinJiShowItem chongShengYinJiShowItem2 = U3DUtils.NEW<ChongShengYinJiShowItem>();
				chongShengYinJiShowItem2.transform.SetParent((type != this.Left) ? this.mRightYinJiTrsfm[j] : this.mLeftYinJiTrsfm[j]);
				chongShengYinJiShowItem2.transform.localPosition = Vector3.zero;
				chongShengYinJiShowItem2.transform.localScale = Vector3.one;
				chongShengYinJiShowItem2.InitValue(list[j], list[0].Type);
				chongShengYinJiShowItem2.ClickHandler = new Action<object, YinJiData>(this.OnClickHandler);
				list3.Add(chongShengYinJiShowItem2);
			}
			if (!this.CacheItemsDict.ContainsKey(type))
			{
				this.CacheItemsDict.Add(type, list3);
			}
		}
	}

	private void OnClickHandler(object sender, YinJiData args)
	{
		this.mCurrentSelectYinJiData = args;
		this.ShowInfoPanel(this.mCurrentSelectYinJiData);
	}

	private void ShowInfoPanel(YinJiData yinji)
	{
		this.IsShowInfoPanel = true;
		this.mYinJiInfoPanel.InitValue(yinji, this.LeftYinJiCount);
	}

	private string TitleShuXingName
	{
		set
		{
			this.mLblTitle.Text = value;
		}
	}

	public void RespondLevelUp(string[] fields)
	{
		int num = Global.SafeConvertToInt32(fields[0]);
		if (num != 1)
		{
			Super.HintMainText(Global.GetLang(ChongShengYinJiData.GetErrorMsg(num)), 10, 3);
			return;
		}
		this.IsShowInfoPanel = false;
		int num2 = Global.SafeConvertToInt32(fields[1]);
		int id = Global.SafeConvertToInt32(fields[2]);
		int num3 = Global.SafeConvertToInt32(fields[3]);
		this.UsedYinJiCount = num3;
		this.LeftYinJiCount = this.LeftCount(num3);
		this.IsShowInfoPanel = false;
		if (this.DictTabYinJi.Count <= 0)
		{
			return;
		}
		List<YinJiData> list = this.DictTabYinJi[this.mCurrentTabId];
		int num4 = list.FindIndex((YinJiData result) => !result.IsMainYinJi && result.ID == this.mCurrentSelectYinJiData.ID && result.Type == this.mCurrentSelectYinJiData.Type);
		if (num4 < list.Count)
		{
			YinJiData yinJiData = list[num4];
			if (!yinJiData.IsFullLevel)
			{
				yinJiData.SetData(id, 1);
				this.mCurrentSelectYinJiData = yinJiData;
			}
		}
		YinJiData yinJiData2 = list[0];
		if (num2 != yinJiData2.ID)
		{
			yinJiData2.SetData(num2, 0);
		}
		this.ShowYinJiInfo(this.mCurrentTabId);
		this.UpdateShuXing();
	}

	private void UpdateShuXing1()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<YinJiData> list = this.DictTabYinJi[this.mCurrentTabId];
		ZhuYinJiCfgData zhuYinJiCfgByIdAndType = ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(list[0].ID, list[0].Type);
		if (zhuYinJiCfgByIdAndType == null)
		{
			Super.HintMainText(Global.GetLang("主印记不存在"), 10, 3);
			return;
		}
		this.TitleShuXingName = zhuYinJiCfgByIdAndType.Name;
		stringBuilder.Append(Global.GetString(new object[]
		{
			zhuYinJiCfgByIdAndType.Name,
			Global.GetLang("："),
			Global.GetLang("主印记")
		}));
		stringBuilder.Append(Environment.NewLine);
		string shuXingStr = ChongShengYinJiData.GetShuXingStr(zhuYinJiCfgByIdAndType.ShuXing, this.grayYellow);
		stringBuilder.Append(shuXingStr);
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetString(new object[]
		{
			zhuYinJiCfgByIdAndType.Name,
			Global.GetLang("："),
			Global.GetLang("辅印记")
		}));
		stringBuilder.Append(Environment.NewLine);
		for (int i = 1; i < list.Count; i++)
		{
			string shuXingStr2 = ChongShengYinJiData.GetShuXingStr(ChongShengYinJiData.GetZiYinJiCfgByIdAndType(list[i].ID, list[i].Type).ShuXing, this.grayYellow);
			if (!shuXingStr2.Contains(Global.GetLang("暂无属性")))
			{
				stringBuilder.Append(shuXingStr2);
			}
		}
		this.mLblShuXing.Text = stringBuilder.ToString();
	}

	private void UpdateShuXing()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<YinJiData> list = this.DictTabYinJi[this.mCurrentTabId];
		ZhuYinJiCfgData zhuYinJiCfgByIdAndType = ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(list[0].ID, list[0].Type);
		if (zhuYinJiCfgByIdAndType == null)
		{
			Super.HintMainText(Global.GetLang("主印记不存在"), 10, 3);
			return;
		}
		this.ItemCollection.Clear();
		this.TitleShuXingName = zhuYinJiCfgByIdAndType.Name;
		ChongShengYinJiPropData chongShengYinJiPropData = default(ChongShengYinJiPropData);
		chongShengYinJiPropData.Title = Global.GetString(new object[]
		{
			zhuYinJiCfgByIdAndType.Name,
			Global.GetLang("："),
			Global.GetLang("主印记")
		});
		List<ChongShengYinJiPropData> shuXingList = ChongShengYinJiData.GetShuXingList(zhuYinJiCfgByIdAndType.ShuXing, this.grayYellow);
		shuXingList.Insert(0, chongShengYinJiPropData);
		shuXingList.Insert(shuXingList.Count, default(ChongShengYinJiPropData));
		shuXingList.Insert(shuXingList.Count, default(ChongShengYinJiPropData));
		for (int i = 0; i < shuXingList.Count; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.mPropUI);
			gameObject.SetActive(true);
			NGUITools.AddChild2(this.mListBox.gameObject, gameObject);
			TextBlock component = gameObject.transform.FindChild("LblTitle").GetComponent<TextBlock>();
			component.Text = shuXingList[i].Title;
			TextBlock component2 = gameObject.transform.FindChild("LblValue").GetComponent<TextBlock>();
			component2.Text = shuXingList[i].Value;
			this.ItemCollection.Add(gameObject);
		}
		ChongShengYinJiPropData chongShengYinJiPropData2 = default(ChongShengYinJiPropData);
		chongShengYinJiPropData2.Title = Global.GetString(new object[]
		{
			zhuYinJiCfgByIdAndType.Name,
			Global.GetLang("："),
			Global.GetLang("辅印记")
		});
		bool flag = true;
		for (int j = 1; j < list.Count; j++)
		{
			List<ChongShengYinJiPropData> shuXingList2 = ChongShengYinJiData.GetShuXingList(ChongShengYinJiData.GetZiYinJiCfgByIdAndType(list[j].ID, list[j].Type).ShuXing, this.grayYellow);
			if (flag)
			{
				flag = false;
				shuXingList2.Insert(0, chongShengYinJiPropData2);
			}
			for (int k = 0; k < shuXingList2.Count; k++)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.mPropUI);
				gameObject2.SetActive(true);
				NGUITools.AddChild2(this.mListBox.gameObject, gameObject2);
				TextBlock component3 = gameObject2.transform.FindChild("LblTitle").GetComponent<TextBlock>();
				component3.Text = shuXingList2[k].Title;
				TextBlock component4 = gameObject2.transform.FindChild("LblValue").GetComponent<TextBlock>();
				component4.Text = shuXingList2[k].Value;
				this.ItemCollection.Add(gameObject2);
			}
		}
	}

	private void ShowHelpPart()
	{
		this.OpenHelpWindow();
	}

	private bool IsShowInfoPanel
	{
		set
		{
			if (this.mYinJiInfoPanel != null)
			{
				this.mYinJiInfoPanel.gameObject.SetActive(value);
			}
		}
	}

	private List<YinJiData> GetYinJiDatas(List<int> YinJiList)
	{
		List<YinJiData> list = new List<YinJiData>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < YinJiList.Count; i += 2)
		{
			list2.Add(YinJiList[i]);
		}
		List<int> list3 = new List<int>();
		for (int j = 1; j < YinJiList.Count; j += 2)
		{
			list3.Add(YinJiList[j]);
		}
		for (int k = 0; k < list2.Count; k++)
		{
			YinJiData yinJiData;
			if (k == 0)
			{
				int id = ChongShengYinJiData.GetZhuIDByTypeAndLevel(list2[k], list3[k]);
				yinJiData = new YinJiData(id, k);
			}
			else
			{
				int id = ChongShengYinJiData.GetZiIDByTypeAndLevel(list2[k], list3[k]);
				yinJiData = new YinJiData(id, k);
			}
			list.Add(yinJiData);
		}
		return list;
	}

	private void OpenHelpWindow()
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		this.m_helpPart.SetHelpInfo(ChongShengYinJiData.GetHelpData().list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	public void DestroySelf()
	{
		this.CacheItemsDict.Clear();
		this.DictTabYinJi.Clear();
		Object.Destroy(base.gameObject);
	}

	public DPSelectedItemEventHandler ChangeHandler;

	public TextBlock mLblTitle;

	public UISprite mBtnYinJi1;

	public UISprite mBtnYinJi2;

	public GButton mBtnChange;

	public GButton mBtnHelp;

	public TextBlock mLblSumTitle;

	public TextBlock mLblSumCount;

	public TextBlock mLblUsedTitle;

	public TextBlock mLblUsedCount;

	public TextBlock mLblShuXing;

	public Transform[] mLeftYinJiTrsfm;

	public Transform[] mRightYinJiTrsfm;

	public ShowNetImage mLeftImg;

	public ShowNetImage mRightImg;

	public UISprite mLeftBtnBg;

	public UISprite mRightBtnBg;

	private string[] mYinJi1Ids;

	private string[] mYinJi2Ids;

	private int mCurrentTabId = 1;

	public ChongShengYinJiInfoPanel mYinJiInfoPanel;

	private YinJiData mCurrentSelectYinJiData;

	private YinJiData mNextYinJiData;

	private Dictionary<int, List<YinJiData>> DictTabYinJi = new Dictionary<int, List<YinJiData>>();

	private string NormalBak = "NetImages/GameRes/Images/ChongShengYinJi/ChongShengYinJi_YinJiDi1.png.qj";

	private string HighLightBak = "NetImages/GameRes/Images/ChongShengYinJi/ChongShengYinJi_YinJiDi2.png.qj";

	private string[] Names = new string[]
	{
		"Zi_SSYJ1",
		"Zi_AYYJ1",
		"Zi_ZRYJ1",
		"Zi_HDYJ1",
		"Zi_MYYJ1"
	};

	private string[] DisableNames = new string[]
	{
		"Zi_SSYJ2",
		"Zi_AYYJ2",
		"Zi_ZRYJ2",
		"Zi_HDYJ2",
		"Zi_MYYJ2"
	};

	private string[] BtnBgs = new string[]
	{
		"AnNiu_Aa1",
		"AnNiu_Aa2"
	};

	public GameObject mLeftMask;

	public GameObject mRightMask;

	private int Left = 1;

	private int Right = 2;

	public Transform mEffect;

	public GameObject mPropUI;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private List<int> YinJi1List;

	private List<int> YinJi2List;

	private int mLeftZhuYinJiName;

	private int mRightZhuYinJi2Name;

	private long mLeftYinJiCount;

	private Dictionary<int, List<ChongShengYinJiShowItem>> CacheItemsDict = new Dictionary<int, List<ChongShengYinJiShowItem>>();

	private string grayYellow = "9d8667";

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;
}
