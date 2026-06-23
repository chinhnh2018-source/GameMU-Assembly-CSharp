using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhanMengLianSaiGuanZhanRank : UserControl
{
	public ObservableCollection JingCaiAwardItemCollection
	{
		get
		{
			return this._JingCaiAwardItemCollection;
		}
		set
		{
			this._JingCaiAwardItemCollection = value;
		}
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

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.mRanKDraggablePanel.onDragFinished = delegate()
		{
			int num = Mathf.RoundToInt(this.mRanKPanel.clipRange.y / (float)this.gridHeight) * this.gridHeight;
			SpringPanel.Begin(this.mRanKPanel.gameObject, new Vector3(0f, (float)(-(float)num - 37), 0f), 10f);
		};
	}

	private void InitTextInPrefabs()
	{
		this.mRankId.Text = Global.GetLang("排名");
		this.mZhanMengName.Text = Global.GetLang("战盟");
		this.mVictoryTimes.Text = Global.GetLang("胜场");
		this.mLblSum.Text = Global.GetLang("总计：");
		this.mBtnLingQu.Text = Global.GetLang("领取");
		this.mBtnYaZhu.Text = Global.GetLang("押注");
		this.LBL_RANK = Global.GetLang("联赛排名：");
		this.LBL_VECTORY_TIMES = Global.GetLang("胜利场次：");
		this.LBL_VECTORY_RATE = Global.GetLang("胜        率：");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendZhanMengLianSaiJingCaiAwardData();
		};
		UIEventListener.Get(this.mLeftJingCaiObj).onClick = delegate(GameObject s)
		{
			try
			{
				if (!this.hasChoosed)
				{
					if (!this.mYaImg.gameObject.activeSelf)
					{
						this.mYaImg.gameObject.SetActive(true);
					}
					this.mYaImg.transform.localPosition = new Vector3(this.mYaImgPosition[0].x, this.mYaImgPosition[0].y, this.mYaImg.transform.localPosition.z);
					this.mChoose = 1;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					ex
				});
			}
		};
		UIEventListener.Get(this.mRightJingCaiObj).onClick = delegate(GameObject s)
		{
			try
			{
				if (!this.hasChoosed)
				{
					if (!this.mYaImg.gameObject.activeSelf)
					{
						this.mYaImg.gameObject.SetActive(true);
					}
					this.mYaImg.transform.localPosition = new Vector3(this.mYaImgPosition[1].x, this.mYaImgPosition[1].y, this.mYaImg.transform.localPosition.z);
					this.mChoose = 2;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<Exception>(new Exception[]
				{
					ex
				});
			}
		};
		this.mBtnYaZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mChoose == 0)
			{
				Super.HintMainText(Global.GetLang("请先选择战盟"), 10, 3);
				return;
			}
			if (Global.Data != null && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= this.GetYaZhuCostJinBi())
			{
				GameInstance.Game.SendZhanMengLianSaiJingCaiData(this.bhid1, this.bhid2, this.mChoose);
			}
			else
			{
				Super.HintMainText(Global.GetLang("金币不足"), 10, 3);
			}
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.JingCaiAwardItemCollection = this.mJingCaiAwardListBox.ItemsSource;
	}

	public void InitRankData(List<BangHuiMatchRankInfo> datas)
	{
		this.InitRankUI();
		int count = datas.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanMengLianSaiGuanZhanRankItem zhanMengLianSaiGuanZhanRankItem = U3DUtils.NEW<ZhanMengLianSaiGuanZhanRankItem>();
			zhanMengLianSaiGuanZhanRankItem.InitRankData(datas[i], i);
			this.AddChild(zhanMengLianSaiGuanZhanRankItem, false);
		}
	}

	public void InitJingCaiAwardData(List<BangHuiMatchGuessInfo> datas)
	{
		this.InitJingCaiAwardUI();
		int count = datas.Count;
		for (int i = 0; i < count; i++)
		{
			ZhanMengLianSaiGuanZhanRankItem zhanMengLianSaiGuanZhanRankItem = U3DUtils.NEW<ZhanMengLianSaiGuanZhanRankItem>();
			zhanMengLianSaiGuanZhanRankItem.InitJingCaiData(datas[i], i);
			this.AddChild(zhanMengLianSaiGuanZhanRankItem, true);
		}
		int num = 0;
		for (int j = 0; j < count; j++)
		{
			num += datas[j].jifen;
		}
		this.mLblValue.Text = num.ToString();
	}

	public void InitJingCaiData(BangHuiMatchPKInfo data)
	{
		this.InitJingCaiUI();
		this.roundId = (int)data.round;
		try
		{
			this.bhid1 = data.bhid1;
			this.bhid2 = data.bhid2;
			this.mLeftLblName.Text = data.bhname1;
			this.mLeftLblRank.Text = this.GetString(new object[]
			{
				this.LBL_RANK,
				data.rank1.ToString()
			});
			this.mLeftLblVectoryTimes.Text = this.GetString(new object[]
			{
				this.LBL_VECTORY_TIMES,
				data.win1
			});
			this.mLeftLblVectoryRate.Text = this.GetString(new object[]
			{
				this.LBL_VECTORY_RATE,
				data.winpct1,
				"%"
			});
			this.mRightLblName.Text = data.bhname2;
			this.mRightLblRank.Text = this.GetString(new object[]
			{
				this.LBL_RANK,
				data.rank2.ToString()
			});
			this.mRightLblVectoryTimes.Text = this.GetString(new object[]
			{
				this.LBL_VECTORY_TIMES,
				data.win2
			});
			this.mRightLblVectoryRate.Text = this.GetString(new object[]
			{
				this.LBL_VECTORY_RATE,
				data.winpct2,
				"%"
			});
			this.mYaImg.gameObject.SetActive(false);
			this.mLblDescribe.Text = this.GetString(new object[]
			{
				Global.GetLang("押注战盟胜利可获得"),
				this.GetYaZhuAward(),
				Global.GetLang("竞猜积分")
			});
			this.mBtnLblHuoBiCount.Text = this.GetYaZhuCostJinBi().ToString();
		}
		catch (Exception ex)
		{
			MUDebug.LogError<Exception>(new Exception[]
			{
				ex
			});
		}
	}

	public void RefreshYaZhuStatusByServer(bool enabled)
	{
		this.hasChoosed = enabled;
		this.mBtnYaZhu.isEnabled = !enabled;
		this.mBtnYaZhu.GetComponent<BoxCollider>().enabled = !enabled;
		this.mBtnYaZhu.GetComponentInChildren<UISprite>().color = Color.gray;
	}

	public void RefreshLingQuAwardByServer(bool enabled)
	{
		this.mBtnLingQu.isEnabled = !enabled;
		this.mBtnLingQu.GetComponent<BoxCollider>().enabled = !enabled;
		this.mBtnLingQu.GetComponentInChildren<UISprite>().color = Color.gray;
	}

	private void InitRankUI()
	{
		try
		{
			NGUITools.SetActive(this.mRankObj, true);
			NGUITools.SetActive(this.mJingCaiAwardObj, false);
			NGUITools.SetActive(this.mJingCaiObj, false);
			NGUITools.SetActive(this.mTitle.gameObject, true);
			NGUITools.SetActive(this.mLblTitle.gameObject, false);
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"排行榜UI初始化有误：" + ex
			});
		}
	}

	private void InitJingCaiAwardUI()
	{
		try
		{
			NGUITools.SetActive(this.mRankObj, false);
			NGUITools.SetActive(this.mJingCaiAwardObj, true);
			NGUITools.SetActive(this.mJingCaiObj, false);
			NGUITools.SetActive(this.mTitle.gameObject, false);
			NGUITools.SetActive(this.mLblTitle.gameObject, true);
			this.mLblTitle.Text = Global.GetLang("竞猜奖励");
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"竞猜奖励UI初始化有误：" + ex
			});
		}
	}

	private void InitJingCaiUI()
	{
		try
		{
			NGUITools.SetActive(this.mRankObj, false);
			NGUITools.SetActive(this.mJingCaiAwardObj, false);
			NGUITools.SetActive(this.mJingCaiObj, true);
			NGUITools.SetActive(this.mTitle.gameObject, false);
			NGUITools.SetActive(this.mLblTitle.gameObject, true);
			this.mLblTitle.Text = Global.GetLang("竞猜");
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				"竞猜UI初始化有误：" + ex
			});
		}
	}

	private void AddChild(ZhanMengLianSaiGuanZhanRankItem item, bool isAward = false)
	{
		UIPanel component = item.transform.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		if (isAward)
		{
			U3DUtils.AddChild(this.mJingCaiAwardListBox.gameObject, item.gameObject, true);
			this.JingCaiAwardItemCollection.AddNoUpdate(item);
		}
		else
		{
			U3DUtils.AddChild(this.mListBox.gameObject, item.gameObject, true);
			this.ItemCollection.AddNoUpdate(item);
		}
	}

	private int GetYaZhuCostJinBi()
	{
		if (this.costJinBiList.Count <= 0)
		{
			this.ParseLeagueSustaionXML();
		}
		int num = this.roundId - 1;
		num = ((num >= 0) ? num : 0);
		if (num >= this.costJinBiList.Count)
		{
			int num2 = this.costJinBiList[this.costJinBiList.Count - 1];
		}
		return this.costJinBiList[num];
	}

	private void ParseLeagueSustaionXML()
	{
		if (this.costJinBiList.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/LeagueSustain.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueSustain");
			if (xelementList != null)
			{
				int count = xelementList.Count;
				for (int i = 0; i < count; i++)
				{
					this.costJinBiList.Add(Global.GetXElementAttributeInt(xelementList[i], "Cost"));
				}
			}
		}
		if (this.costJinBiList.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"LeagueSustain.XML 消耗金币配置有误！"
			});
		}
	}

	private int GetYaZhuAward()
	{
		if (this.awardList.Count <= 0)
		{
			this.ParseLeagueSustaionAwardXML();
		}
		int num = this.roundId - 1;
		num = ((num >= 0) ? num : 0);
		if (num >= this.awardList.Count)
		{
			int num2 = this.awardList[this.awardList.Count - 1];
		}
		return this.awardList[num];
	}

	private void ParseLeagueSustaionAwardXML()
	{
		if (this.awardList.Count > 0)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/LeagueSustain.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueSustain");
			if (xelementList != null)
			{
				int count = xelementList.Count;
				for (int i = 0; i < count; i++)
				{
					this.awardList.Add(Global.GetXElementAttributeInt(xelementList[i], "WinAward"));
				}
			}
		}
		if (this.awardList.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"LeagueSustain.XML awardList配置有误！"
			});
		}
	}

	public string GetString(params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < args.Length; i++)
		{
			stringBuilder.Append(args[i]);
		}
		return stringBuilder.ToString();
	}

	public override void Destroy()
	{
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mTitle = null;
		this.mRankObj = null;
		this.mRankId = null;
		this.mZhanMengName = null;
		this.mVictoryTimes = null;
		this.mRanKPanel = null;
		this.mRanKDraggablePanel = null;
		this.mJingCaiObj = null;
		this.mYaImg = null;
		this.mLeftJingCaiObj = null;
		this.mLeftLblName = null;
		this.mLeftLblRank = null;
		this.mLeftLblVectoryTimes = null;
		this.mLeftLblVectoryRate = null;
		this.mRightJingCaiObj = null;
		this.mRightLblName = null;
		this.mRightLblRank = null;
		this.mRightLblVectoryTimes = null;
		this.mRightLblVectoryRate = null;
		this.mBtnLblHuoBiCount = null;
		this.mLblDescribe = null;
		this.mBtnYaZhu = null;
		this.LBL_NAME = string.Empty;
		this.LBL_RANK = string.Empty;
		this.LBL_VECTORY_TIMES = string.Empty;
		this.LBL_VECTORY_RATE = string.Empty;
		this.mChoose = 0;
		this.mJingCaiAwardObj = null;
		this.mLblSum = null;
		this.mLblValue = null;
		this.mBtnLingQu = null;
		this.mListBox = null;
		this._ItemCollection = null;
	}

	private const string RANK_TITILE_NAME = "SuperLianSaiTitle01";

	private const string path = "Config/LeagueSustain.xml";

	private const string path1 = "Config/LeagueSustain.xml";

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public UISprite mTitle;

	public TextBlock mLblTitle;

	public GameObject mRankObj;

	public TextBlock mRankId;

	public TextBlock mZhanMengName;

	public TextBlock mVictoryTimes;

	public UIPanel mRanKPanel;

	public UIDraggablePanel mRanKDraggablePanel;

	public GameObject mJingCaiObj;

	public UISprite mYaImg;

	public GameObject mLeftJingCaiObj;

	public TextBlock mLeftLblName;

	public TextBlock mLeftLblRank;

	public TextBlock mLeftLblVectoryTimes;

	public TextBlock mLeftLblVectoryRate;

	public GameObject mRightJingCaiObj;

	public TextBlock mRightLblName;

	public TextBlock mRightLblRank;

	public TextBlock mRightLblVectoryTimes;

	public TextBlock mRightLblVectoryRate;

	public TextBlock mBtnLblHuoBiCount;

	public TextBlock mLblDescribe;

	public GButton mBtnYaZhu;

	private string LBL_NAME = string.Empty;

	private string LBL_RANK = string.Empty;

	private string LBL_VECTORY_TIMES = string.Empty;

	private string LBL_VECTORY_RATE = string.Empty;

	private Vector2[] mYaImgPosition = new Vector2[]
	{
		new Vector2(-42f, -70f),
		new Vector2(150f, -70f)
	};

	private int mChoose;

	private int roundId;

	private bool hasChoosed;

	public GameObject mJingCaiAwardObj;

	public TextBlock mLblSum;

	public TextBlock mLblValue;

	public GButton mBtnLingQu;

	public ListBox mJingCaiAwardListBox;

	private ObservableCollection _JingCaiAwardItemCollection;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private int gridHeight = 42;

	private int bhid1;

	private int bhid2;

	private List<int> costJinBiList = new List<int>();

	private List<int> awardList = new List<int>();
}
