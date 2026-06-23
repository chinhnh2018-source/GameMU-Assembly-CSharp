using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class RiChangHuoDongItem : UserControl
{
	public string ItemName
	{
		get
		{
			return this._ItemName.text;
		}
		set
		{
			this._ItemName.text = value;
		}
	}

	public int ShowType
	{
		set
		{
			if (value == 0)
			{
			}
		}
	}

	public string Text
	{
		set
		{
			this._Text.text = value;
		}
	}

	public string Time
	{
		set
		{
			this._Time.text = value;
		}
	}

	public bool IsGrayImage
	{
		set
		{
			if (this._GrayImage != value)
			{
				this._GrayImage = value;
				this._Preview.ToGrayBitmap = value;
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return this._IsEnabled;
		}
		set
		{
			if (this._IsEnabled != value)
			{
				this._IsEnabled = value;
				this.IsGrayImage = !value;
			}
		}
	}

	public bool SelectedState
	{
		set
		{
			this._SelectedState = value;
			this.SelectedRect.gameObject.SetActive(value);
			if (this._GrayIfInactive)
			{
				this.IsGrayImage = !value;
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._RewardLevel[0].transform.localPosition = new Vector3(-36f, -130f, -0.1f);
		this._RewardLevel[1].transform.localPosition = new Vector3(-36f, -166f, -0.1f);
		this._ItemName.Z = -0.1;
		this._RewardType[0].transform.localScale = new Vector3(20f, 20f, 1f);
		this._RewardType[1].transform.localScale = new Vector3(20f, 20f, 1f);
	}

	private string GetXmlNodeName()
	{
		if ("BloodCastleInfo.xml" == this.m_strConfigPath)
		{
			return "BloodCastleInfo";
		}
		if ("Demon.xml" == this.m_strConfigPath)
		{
			return "Emo";
		}
		if ("Battle.xml" == this.m_strConfigPath || "ArenaBattle.xml" == this.m_strConfigPath || "AngelTemple.xml" == this.m_strConfigPath)
		{
			return "Item";
		}
		if (null != this.m_LblShowText)
		{
			this.m_LblShowText.gameObject.SetActive(false);
		}
		return string.Empty;
	}

	private void ActivityTipEventHandler(int type, ActivityTipItem args)
	{
		this.ActivityActive = args.IsActive;
	}

	private bool GetBeginTimeLst()
	{
		if (string.Empty != this.m_strConfigPath)
		{
			string xmlName = "config/" + this.m_strConfigPath;
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return false;
			}
			string xmlNodeName = this.GetXmlNodeName();
			if (string.Empty == xmlNodeName)
			{
				return false;
			}
			if ("Item" == xmlNodeName)
			{
				if ("AngelTemple.xml" == this.m_strConfigPath)
				{
					ActivityTipManager.RegActivityTipItem(1007, new ActivityTipEventHandler(this.ActivityTipEventHandler));
				}
				if ("Battle.xml" == this.m_strConfigPath || "AngelTemple.xml" == this.m_strConfigPath)
				{
					List<XElement> xelementList = Global.GetXElementList(gameResXml, xmlNodeName);
					foreach (XElement xelement in xelementList)
					{
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
						int maxZhuanSheng = -1;
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinLevel");
						int maxLevel = -1;
						if (UIHelper.AvalidLevel(xelementAttributeInt2, maxLevel, xelementAttributeInt, maxZhuanSheng) == 0)
						{
							string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "TimePoints");
							this.m_LstBeginTime = UIHelper.ParserTimeArrayString2(xelementAttributeStr);
							this.m_nPrepareTime = Global.GetXElementAttributeInt(xelement, "WaitingEnterSecs");
							this.m_nPrepareTime += Global.GetXElementAttributeInt(xelement, "PrepareSecs");
							this.m_nPrepareTime += Global.GetXElementAttributeInt(xelement, "FightingSecs");
							return true;
						}
					}
				}
				else if ("ArenaBattle.xml" == this.m_strConfigPath)
				{
					List<XElement> xelementList2 = Global.GetXElementList(gameResXml, xmlNodeName);
					foreach (XElement xelement2 in xelementList2)
					{
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "MinZhuanSheng");
						int maxZhuanSheng2 = -1;
						int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "MinLevel");
						int maxLevel2 = -1;
						if (UIHelper.AvalidLevel(xelementAttributeInt4, maxLevel2, xelementAttributeInt3, maxZhuanSheng2) == 0)
						{
							string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "TimePoints");
							this.m_LstBeginTime = UIHelper.ParserTimeArrayString2(xelementAttributeStr2);
							this.m_nPrepareTime = Global.GetXElementAttributeInt(xelement2, "WaitingEnterSecs");
							this.m_nPrepareTime += Global.GetXElementAttributeInt(xelement2, "PrepareSecs");
							return true;
						}
					}
				}
				return false;
			}
			List<XElement> xelementList3 = Global.GetXElementList(gameResXml, xmlNodeName);
			foreach (XElement xelement3 in xelementList3)
			{
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement3, "MinChangeLife");
				int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement3, "MaxChangeLife");
				int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement3, "MinLevel");
				int xelementAttributeInt8 = Global.GetXElementAttributeInt(xelement3, "MaxLevel");
				if (UIHelper.AvalidLevel(xelementAttributeInt7, xelementAttributeInt8, xelementAttributeInt5, xelementAttributeInt6) == 0)
				{
					string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement3, "BeginTime");
					this.m_LstBeginTime = UIHelper.ParserTimeArrayString2(xelementAttributeStr3);
					this.m_nPrepareTime = Global.GetXElementAttributeInt(xelement3, "PrepareTime");
					return true;
				}
			}
			return false;
		}
		return false;
	}

	private new void OnDestroy()
	{
		if ("AngelTemple.xml" == this.m_strConfigPath)
		{
			ActivityTipManager.UnRegActivityTipItem(1007, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		}
	}

	public void Init()
	{
		Transform parent = base.transform.parent;
		ListBox listBox = NGUITools.FindInParents<ListBox>(base.gameObject);
		if (null != listBox)
		{
			parent = listBox.transform.parent;
		}
		UIDragObject uidragObject = base.gameObject.GetComponent<UIDragObject>();
		if (null == uidragObject)
		{
			uidragObject = base.gameObject.AddComponent<UIDragObject>();
		}
		if (this.ActivityCategory == ActivityCategorys.RiChangHuoDong)
		{
			uidragObject.scale = Vector3.right;
			uidragObject.target = base.transform.parent;
			uidragObject.restrictWithinPanel = true;
			this.GetBeginTimeLst();
		}
	}

	protected virtual void OnEnable()
	{
		if (this.ActivityCategory == ActivityCategorys.RiChangHuoDong)
		{
			if (this.m_LstBeginTime == null)
			{
				this.GetBeginTimeLst();
			}
			base.StartCoroutine<bool>(this.TimeProc());
		}
	}

	private string GetTimeFormatString(int state)
	{
		if (this.HuoDongType == RiChangHuoDongTypes.Battle)
		{
			if (state == 0)
			{
				return Global.GetLang("战斗结束倒计时");
			}
			return Global.GetLang("下轮活动倒计时");
		}
		else if (this.HuoDongType == RiChangHuoDongTypes.AngelTemple)
		{
			if (state == 0)
			{
				return Global.GetLang("活动结束倒计时");
			}
			return Global.GetLang("下轮活动倒计时");
		}
		else
		{
			if (state == 0)
			{
				return Global.GetLang("入场时间");
			}
			return Global.GetLang("开启时间");
		}
	}

	protected IEnumerator TimeProc()
	{
		int beginTime = 0;
		int nextTime = 0;
		for (;;)
		{
			DateTime now = Global.GetCorrectDateTime();
			int currentTime = now.Hour * 3600 + now.Minute * 60 + now.Second;
			if (currentTime < beginTime || nextTime <= currentTime)
			{
				if (this.m_LstBeginTime != null)
				{
					if (this.m_LstBeginTime.Count == 1)
					{
						beginTime = this.m_LstBeginTime[0];
						if (currentTime <= nextTime)
						{
							nextTime = this.m_LstBeginTime[0];
						}
						else
						{
							nextTime = this.m_LstBeginTime[0] + 86400;
						}
					}
					else if (this.m_LstBeginTime.Count >= 2)
					{
						beginTime = this.m_LstBeginTime[0];
						nextTime = this.m_LstBeginTime[this.m_LstBeginTime.Count - 1];
						if (currentTime >= beginTime && currentTime < nextTime)
						{
							for (int i = 0; i < this.m_LstBeginTime.Count - 1; i++)
							{
								if (currentTime >= this.m_LstBeginTime[i] && currentTime < this.m_LstBeginTime[i + 1])
								{
									beginTime = this.m_LstBeginTime[i];
									nextTime = this.m_LstBeginTime[i + 1];
									break;
								}
							}
						}
						else if (currentTime < beginTime)
						{
							beginTime = nextTime - 86400;
							nextTime = this.m_LstBeginTime[0];
						}
						else
						{
							beginTime = nextTime;
							nextTime = this.m_LstBeginTime[0] + 86400;
						}
					}
				}
			}
			string text = null;
			if (this.m_nPrepareTime > 0)
			{
				if ((this.ActivityActive || currentTime <= beginTime + 60) && currentTime >= beginTime && this.m_nPrepareTime + beginTime >= currentTime)
				{
					this.Text = this.GetTimeFormatString(0);
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Empty,
						"00ff00",
						Global.GetLang("剩余") + UIHelper.FormatSecs((long)(beginTime + this.m_nPrepareTime - currentTime), "-")
					});
					this.IsGrayImage = false;
				}
				else if (nextTime >= currentTime)
				{
					string timeFormatString = this.GetTimeFormatString(1);
					this.Text = timeFormatString;
					this.Text = timeFormatString;
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Empty,
						"fd010c",
						Global.GetLang("剩余") + UIHelper.FormatSecs((long)(nextTime - currentTime), string.Empty)
					});
					this.IsGrayImage = true;
				}
			}
			this._Time.text = text;
			if (string.Empty == text || this.m_LstBeginTime == null)
			{
				if (null != this.m_LblShowText)
				{
					this.m_LblShowText.gameObject.SetActive(false);
				}
			}
			else if (null != this.m_LblShowText)
			{
				this.m_LblShowText.gameObject.SetActive(true);
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
		}
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
		}
	}

	public ImageBrush BodyBackgournd
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public TextBlock _ItemName;

	public ShowNetImage _Preview;

	public TextBlock[] _RewardType;

	public GImgProgressBar[] _RewardLevel;

	public UISprite SelectedRect;

	public TextBlock _Text;

	public TextBlock _Time;

	public GameObject obj;

	public TextBlock NeedLeve;

	[NonSerialized]
	public string Level;

	[NonSerialized]
	public string CopyType = string.Empty;

	public Sprite sprite;

	[NonSerialized]
	public ActivityCategorys ActivityCategory = ActivityCategorys.RiChangHuoDong;

	[NonSerialized]
	public RiChangHuoDongTypes HuoDongType;

	[NonSerialized]
	public int CopyID;

	[NonSerialized]
	public int TabID;

	[NonSerialized]
	public int MapCode;

	[NonSerialized]
	public int DisplayID;

	[NonSerialized]
	public int MinLevel;

	[NonSerialized]
	public int MaxLevel;

	[NonSerialized]
	public int MinZhuanSheng;

	[NonSerialized]
	public int MaxZhuanSheng;

	[NonSerialized]
	public int MaxEnterNum;

	[NonSerialized]
	public int EnterGoods;

	[NonSerialized]
	public int GoodsNumber;

	[NonSerialized]
	public int MinSaoDangTime;

	[NonSerialized]
	public int NeedYuanBao;

	[NonSerialized]
	public int ZhanLi;

	[NonSerialized]
	public int EnterNum;

	[NonSerialized]
	public int FinishNum;

	[NonSerialized]
	public bool LevelAllow;

	[NonSerialized]
	public bool SaoDangAllow;

	[NonSerialized]
	public bool NumberAllow;

	[NonSerialized]
	public bool ZhanLiAllow;

	[NonSerialized]
	public string RewardExplains = string.Empty;

	[NonSerialized]
	public string RewardGoods = string.Empty;

	[NonSerialized]
	public string m_strConfigPath = string.Empty;

	[NonSerialized]
	public bool ActivityActive = true;

	private List<int> m_LstBeginTime;

	private int m_nPrepareTime;

	public UILabel m_LblShowText;

	private bool _GrayIfInactive;

	private bool _GrayImage;

	private bool _IsEnabled = true;

	private bool _SelectedState;
}
