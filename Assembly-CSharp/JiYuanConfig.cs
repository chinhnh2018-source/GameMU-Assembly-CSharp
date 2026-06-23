using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class JiYuanConfig
{
	public JiYuanConfig()
	{
		this.AddXmlEraDrop();
		this.AddXmlEraTask();
		this.AddXmlEraReward();
		this.AddXmlEraUI();
		this.AddXmlEraContribution();
		this.AddXmlEraIntro();
	}

	private void AddXmlEraDrop()
	{
		XElement gameResXml = Global.GetGameResXml(this.ERADROP_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EraDrop");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraDrop eraDrop = new EraDrop();
			eraDrop.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eraDrop.EraID = Global.GetXElementAttributeInt(xelementList[i], "EraID");
			eraDrop.StartTime = Global.GetXElementAttributeStr(xelementList[i], "StartTime");
			eraDrop.EndTime = Global.GetXElementAttributeStr(xelementList[i], "EndTime");
			eraDrop.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			eraDrop.MapID = Global.GetXElementAttributeStr(xelementList[i], "MapID");
			eraDrop.MonsterType = Global.GetXElementAttributeInt(xelementList[i], "MonsterType");
			eraDrop.BuffID = Global.GetXElementAttributeInt(xelementList[i], "BuffID");
			eraDrop.DropID = Global.GetXElementAttributeInt(xelementList[i], "DropID");
			if (!this.m_DicEraDrop.ContainsKey(eraDrop.ID))
			{
				this.m_DicEraDrop.Add(eraDrop.ID, eraDrop);
			}
		}
	}

	private void AddXmlEraTask()
	{
		this.m_DicEraTask.Clear();
		XElement gameResXml = Global.GetGameResXml(this.ERATASK_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EraTask");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraTask eraTask = new EraTask();
			eraTask.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eraTask.EraID = Global.GetXElementAttributeInt(xelementList[i], "EraID");
			eraTask.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			eraTask.EraStage = Global.GetXElementAttributeInt(xelementList[i], "EraStage");
			eraTask.StageName = Global.GetXElementAttributeStr(xelementList[i], "StageName");
			eraTask.StageDescription = Global.GetXElementAttributeStr(xelementList[i], "StageDescription");
			eraTask.TaskName = Global.GetXElementAttributeStr(xelementList[i], "TaskName");
			eraTask.Description = Global.GetXElementAttributeStr(xelementList[i], "Description");
			eraTask.CompletionCondition = Global.GetXElementAttributeStr(xelementList[i], "CompletionCondition");
			eraTask.Reward = Global.GetXElementAttributeStr(xelementList[i], "Reward");
			if (!this.m_DicEraTask.ContainsKey(eraTask.ID))
			{
				this.m_DicEraTask.Add(eraTask.ID, eraTask);
			}
		}
	}

	private void AddXmlEraReward()
	{
		XElement gameResXml = Global.GetGameResXml(this.ERAREWARD_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EraReward");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraReward eraReward = new EraReward();
			eraReward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eraReward.EraID = Global.GetXElementAttributeInt(xelementList[i], "EraID");
			eraReward.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			eraReward.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			eraReward.StartTime = Global.GetXElementAttributeStr(xelementList[i], "StartTime");
			eraReward.EndTime = Global.GetXElementAttributeStr(xelementList[i], "EndTime");
			eraReward.RewardName = Global.GetXElementAttributeStr(xelementList[i], "RewardName");
			eraReward.EraRanking = Global.GetXElementAttributeInt(xelementList[i], "EraRanking");
			eraReward.Progress = Global.GetXElementAttributeInt(xelementList[i], "Progress");
			eraReward.Contribution = Global.GetXElementAttributeInt(xelementList[i], "Contribution");
			eraReward.LeaderReward = Global.GetXElementAttributeStr(xelementList[i], "LeaderReward");
			eraReward.MasterReward = Global.GetXElementAttributeStr(xelementList[i], "MasterReward");
			eraReward.Reward = Global.GetXElementAttributeStr(xelementList[i], "Reward");
			if (!this.m_DicEraReward.ContainsKey(eraReward.ID))
			{
				this.m_DicEraReward.Add(eraReward.ID, eraReward);
			}
		}
	}

	private void AddXmlEraUI()
	{
		XElement gameResXml = Global.GetGameResXml(this.ERAUI_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EraUI");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraUI eraUI = new EraUI();
			eraUI.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eraUI.EraID = Global.GetXElementAttributeInt(xelementList[i], "EraID");
			eraUI.StartTime = Global.GetXElementAttributeStr(xelementList[i], "StartTime");
			eraUI.EndTime = Global.GetXElementAttributeStr(xelementList[i], "EndTime");
			eraUI.ThemeBackground = Global.GetXElementAttributeStr(xelementList[i], "ThemeBackground");
			eraUI.Logo = Global.GetXElementAttributeStr(xelementList[i], "Logo");
			eraUI.AwardBackground = Global.GetXElementAttributeStr(xelementList[i], "AwardBackground");
			eraUI.AwardButton1 = Global.GetXElementAttributeStr(xelementList[i], "AwardButton1");
			eraUI.AwardButton2 = Global.GetXElementAttributeStr(xelementList[i], "AwardButton2");
			eraUI.ProgressBar1Icon1 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar1Icon1");
			eraUI.ProgressBar1Icon2 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar1Icon2");
			eraUI.ProgressBar1Icon3 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar1Icon3");
			eraUI.ProgressBar2Icon1 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar2Icon1");
			eraUI.ProgressBar2Icon2 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar2Icon2");
			eraUI.ProgressBar2Icon3 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar2Icon3");
			eraUI.ProgressBar3Icon1 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar3Icon1");
			eraUI.ProgressBar3Icon2 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar3Icon2");
			eraUI.ProgressBar3Icon3 = Global.GetXElementAttributeStr(xelementList[i], "ProgressBar3Icon3");
			eraUI.OwnProgressBarColor = Global.GetXElementAttributeStr(xelementList[i], "OwnProgressBarColor");
			eraUI.FastestProgressBarColor = Global.GetXElementAttributeStr(xelementList[i], "FastestProgressBarColor");
			eraUI.DonationScheduleColor = Global.GetXElementAttributeStr(xelementList[i], "DonationScheduleColor");
			eraUI.ProgressIcon = Global.GetXElementAttributeStr(xelementList[i], "ProgressIcon");
			if (!this.m_DicEraUI.ContainsKey(eraUI.ID))
			{
				this.m_DicEraUI.Add(eraUI.ID, eraUI);
			}
		}
	}

	private void AddXmlEraContribution()
	{
		this.m_DicEraContribution.Clear();
		XElement gameResXml = Global.GetGameResXml(this.ERACONTRIBUTION_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EraContribution");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraContribution eraContribution = new EraContribution();
			eraContribution.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			eraContribution.EraID = Global.GetXElementAttributeInt(xelementList[i], "EraID");
			eraContribution.ProgressID = Global.GetXElementAttributeInt(xelementList[i], "ProgressID");
			eraContribution.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
			eraContribution.Contribution = Global.GetXElementAttributeInt(xelementList[i], "Contribution");
			eraContribution.MonsterID = Global.GetXElementAttributeStr(xelementList[i], "MonsterID");
			if (!this.m_DicEraContribution.ContainsKey(eraContribution.ID))
			{
				this.m_DicEraContribution.Add(eraContribution.ID, eraContribution);
			}
		}
	}

	private void AddXmlEraIntro()
	{
		this.m_StrShuoMing = string.Empty;
		XElement gameResXml = Global.GetGameResXml(this.ERAINTOR_PATH);
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
		for (int i = 0; i < xelementList.Count; i++)
		{
			EraTask eraTask = new EraTask();
			if (Global.GetXElementAttributeInt(xelementList[i], "Bold") == 1)
			{
				this.m_StrTitle = Global.GetXElementAttributeStr(xelementList[i], "Intro");
			}
			else if (Global.GetXElementAttributeInt(xelementList[i], "Bold") == 0)
			{
				this.m_StrShuoMing = this.m_StrShuoMing + Global.GetXElementAttributeStr(xelementList[i], "Intro") + Environment.NewLine;
			}
		}
	}

	public void InitValue()
	{
		this.AddXmlEraDrop();
		this.AddXmlEraTask();
		this.AddXmlEraReward();
		this.AddXmlEraUI();
		this.AddXmlEraContribution();
	}

	public string StrTitle
	{
		get
		{
			return this.m_StrTitle;
		}
	}

	public string StrShuoMing
	{
		get
		{
			return this.m_StrShuoMing;
		}
	}

	public Dictionary<int, EraDrop> DicEraDrop
	{
		get
		{
			return this.m_DicEraDrop;
		}
	}

	public Dictionary<int, EraTask> DicEraTask
	{
		get
		{
			return this.m_DicEraTask;
		}
	}

	public Dictionary<int, EraReward> DicEraReward
	{
		get
		{
			return this.m_DicEraReward;
		}
	}

	public EraUI EraUI
	{
		get
		{
			if (this.m_DicEraUI.ContainsKey(this.JiYuanKey))
			{
				return this.m_DicEraUI[this.JiYuanKey];
			}
			return null;
		}
	}

	public Dictionary<int, EraContribution> DicEraContribution
	{
		get
		{
			return this.m_DicEraContribution;
		}
	}

	public string StageDescription
	{
		get
		{
			foreach (KeyValuePair<int, EraTask> keyValuePair in this.m_DicEraTask)
			{
				if (keyValuePair.Value.EraID == this.m_JiYuanKey)
				{
					Dictionary<int, EraTask>.Enumerator enumerator;
					KeyValuePair<int, EraTask> keyValuePair2 = enumerator.Current;
					return keyValuePair2.Value.StageDescription;
				}
			}
			return string.Empty;
		}
	}

	public string[] EraRefreshTime
	{
		get
		{
			this.strTimes = ConfigSystemParam.GetSystemParamStringArrayByName("EraRefreshTime", ',');
			return this.strTimes;
		}
	}

	public Dictionary<int, EraReward> DicEraRewardJieDuan
	{
		get
		{
			if (this.m_DicEraRewardJieDuan.Count <= 0)
			{
				foreach (KeyValuePair<int, EraReward> keyValuePair in this.m_DicEraReward)
				{
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						Dictionary<int, EraReward>.Enumerator enumerator;
						KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value.Type == 1)
						{
							Dictionary<int, EraReward> dicEraRewardJieDuan = this.m_DicEraRewardJieDuan;
							KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
							if (!dicEraRewardJieDuan.ContainsKey(keyValuePair3.Value.Progress))
							{
								Dictionary<int, EraReward> dicEraRewardJieDuan2 = this.m_DicEraRewardJieDuan;
								KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
								int progress = keyValuePair4.Value.Progress;
								KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
								dicEraRewardJieDuan2.Add(progress, keyValuePair5.Value);
							}
						}
					}
				}
			}
			return this.m_DicEraRewardJieDuan;
		}
	}

	public Dictionary<int, EraReward> DicEraRewardGongXian
	{
		get
		{
			if (this.m_DicEraRewardGongXian.Count <= 0)
			{
				foreach (KeyValuePair<int, EraReward> keyValuePair in this.m_DicEraReward)
				{
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						Dictionary<int, EraReward>.Enumerator enumerator;
						KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value.Type == 3)
						{
							Dictionary<int, EraReward> dicEraRewardGongXian = this.m_DicEraRewardGongXian;
							KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
							if (!dicEraRewardGongXian.ContainsKey(keyValuePair3.Value.Contribution))
							{
								Dictionary<int, EraReward> dicEraRewardGongXian2 = this.m_DicEraRewardGongXian;
								KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
								int contribution = keyValuePair4.Value.Contribution;
								KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
								dicEraRewardGongXian2.Add(contribution, keyValuePair5.Value);
							}
						}
					}
				}
			}
			return this.m_DicEraRewardGongXian;
		}
	}

	public Dictionary<int, EraReward> DicEraRewardPaiHang
	{
		get
		{
			if (this.m_DicEraRewardPaiHang.Count <= 0)
			{
				foreach (KeyValuePair<int, EraReward> keyValuePair in this.m_DicEraReward)
				{
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						Dictionary<int, EraReward>.Enumerator enumerator;
						KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value.Type == 2)
						{
							Dictionary<int, EraReward> dicEraRewardPaiHang = this.m_DicEraRewardPaiHang;
							KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
							if (!dicEraRewardPaiHang.ContainsKey(keyValuePair3.Value.EraRanking))
							{
								Dictionary<int, EraReward> dicEraRewardPaiHang2 = this.m_DicEraRewardPaiHang;
								KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
								int eraRanking = keyValuePair4.Value.EraRanking;
								KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
								dicEraRewardPaiHang2.Add(eraRanking, keyValuePair5.Value);
								KeyValuePair<int, EraReward> keyValuePair6 = enumerator.Current;
								this.m_PaiHangBangTime = DateTime.Parse(keyValuePair6.Value.StartTime).Ticks / 10000L;
							}
						}
					}
				}
			}
			return this.m_DicEraRewardPaiHang;
		}
	}

	public long PaiHangBangTime
	{
		get
		{
			if (this.m_PaiHangBangTime <= 0L)
			{
				foreach (KeyValuePair<int, EraReward> keyValuePair in this.m_DicEraReward)
				{
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						Dictionary<int, EraReward>.Enumerator enumerator;
						KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
						if (keyValuePair2.Value.Type == 2)
						{
							Dictionary<int, EraReward> dicEraRewardPaiHang = this.m_DicEraRewardPaiHang;
							KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
							if (!dicEraRewardPaiHang.ContainsKey(keyValuePair3.Value.EraRanking))
							{
								Dictionary<int, EraReward> dicEraRewardPaiHang2 = this.m_DicEraRewardPaiHang;
								KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
								int eraRanking = keyValuePair4.Value.EraRanking;
								KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
								dicEraRewardPaiHang2.Add(eraRanking, keyValuePair5.Value);
								KeyValuePair<int, EraReward> keyValuePair6 = enumerator.Current;
								this.m_PaiHangBangTime = DateTime.Parse(keyValuePair6.Value.StartTime).Ticks / 10000L;
							}
						}
					}
				}
			}
			return this.m_PaiHangBangTime;
		}
	}

	public Dictionary<int, EraTask> DicEraRewardJuanXian
	{
		get
		{
			if (this.m_DicEraRewardJuanXian.Count <= 0)
			{
				Dictionary<int, EraTask>.Enumerator enumerator = this.m_DicEraTask.GetEnumerator();
				string text = string.Empty;
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, EraTask> keyValuePair = enumerator.Current;
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						Dictionary<int, EraTask> dicEraRewardJuanXian = this.m_DicEraRewardJuanXian;
						KeyValuePair<int, EraTask> keyValuePair2 = enumerator.Current;
						if (!dicEraRewardJuanXian.ContainsKey(keyValuePair2.Value.EraStage))
						{
							Dictionary<int, EraTask> dicEraRewardJuanXian2 = this.m_DicEraRewardJuanXian;
							KeyValuePair<int, EraTask> keyValuePair3 = enumerator.Current;
							int eraStage = keyValuePair3.Value.EraStage;
							KeyValuePair<int, EraTask> keyValuePair4 = enumerator.Current;
							dicEraRewardJuanXian2.Add(eraStage, keyValuePair4.Value);
							KeyValuePair<int, EraTask> keyValuePair5 = enumerator.Current;
							text = keyValuePair5.Value.CompletionCondition;
						}
						else
						{
							string text2 = text;
							string text3 = "|";
							KeyValuePair<int, EraTask> keyValuePair6 = enumerator.Current;
							text = text2 + text3 + keyValuePair6.Value.CompletionCondition;
							Dictionary<int, EraTask> dicEraRewardJuanXian3 = this.m_DicEraRewardJuanXian;
							KeyValuePair<int, EraTask> keyValuePair7 = enumerator.Current;
							dicEraRewardJuanXian3[keyValuePair7.Value.EraStage].CompletionCondition = text;
						}
					}
				}
				this.AddXmlEraTask();
			}
			return this.m_DicEraRewardJuanXian;
		}
	}

	public int JiYuanKey
	{
		get
		{
			if (this.m_JiYuanKey == -1)
			{
				Dictionary<int, EraUI>.Enumerator enumerator = this.m_DicEraUI.GetEnumerator();
				while (enumerator.MoveNext())
				{
					long correctLocalTime = Global.GetCorrectLocalTime();
					KeyValuePair<int, EraUI> keyValuePair = enumerator.Current;
					long num = DateTime.Parse(keyValuePair.Value.StartTime).Ticks / 10000L;
					KeyValuePair<int, EraUI> keyValuePair2 = enumerator.Current;
					long num2 = DateTime.Parse(keyValuePair2.Value.EndTime).Ticks / 10000L;
					if (correctLocalTime >= num && correctLocalTime <= num2)
					{
						KeyValuePair<int, EraUI> keyValuePair3 = enumerator.Current;
						this.m_JiYuanKey = keyValuePair3.Value.EraID;
					}
				}
				if (this.m_JiYuanKey == -1)
				{
				}
			}
			return this.m_JiYuanKey;
		}
	}

	public List<EraTask> ListEraTask
	{
		get
		{
			this.m_ListEraTask.Clear();
			Dictionary<int, EraTask>.Enumerator enumerator = this.m_DicEraTask.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (this.data != null)
				{
					KeyValuePair<int, EraTask> keyValuePair = enumerator.Current;
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						int eraStage = (int)this.data.EraStage;
						KeyValuePair<int, EraTask> keyValuePair2 = enumerator.Current;
						if (eraStage == keyValuePair2.Value.EraStage)
						{
							List<EraTask> listEraTask = this.m_ListEraTask;
							KeyValuePair<int, EraTask> keyValuePair3 = enumerator.Current;
							listEraTask.Add(keyValuePair3.Value);
						}
					}
				}
			}
			return this.m_ListEraTask;
		}
	}

	public int GongXianMax
	{
		get
		{
			if (this.m_GongXianMax <= 0)
			{
				foreach (KeyValuePair<int, EraReward> keyValuePair in this.DicEraRewardGongXian)
				{
					if (keyValuePair.Value.EraID == this.JiYuanKey)
					{
						int gongXianMax = this.m_GongXianMax;
						Dictionary<int, EraReward>.Enumerator enumerator;
						KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
						if (gongXianMax < keyValuePair2.Value.Contribution)
						{
							KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
							this.m_GongXianMax = keyValuePair3.Value.Contribution;
						}
					}
				}
			}
			return this.m_GongXianMax;
		}
	}

	private string ERADROP_PATH = "Config/EraDrop";

	private string ERATASK_PATH = "Config/EraTask";

	private string ERAREWARD_PATH = "Config/EraReward";

	private string ERAUI_PATH = "Config/EraUI";

	private string ERACONTRIBUTION_PATH = "Config/EraContribution";

	private string ERAINTOR_PATH = "Config/EraIntro";

	private Dictionary<int, EraDrop> m_DicEraDrop = new Dictionary<int, EraDrop>();

	private Dictionary<int, EraTask> m_DicEraTask = new Dictionary<int, EraTask>();

	private Dictionary<int, EraReward> m_DicEraReward = new Dictionary<int, EraReward>();

	private Dictionary<int, EraUI> m_DicEraUI = new Dictionary<int, EraUI>();

	private Dictionary<int, EraContribution> m_DicEraContribution = new Dictionary<int, EraContribution>();

	private Dictionary<int, EraReward> m_DicEraRewardJieDuan = new Dictionary<int, EraReward>();

	private Dictionary<int, EraReward> m_DicEraRewardPaiHang = new Dictionary<int, EraReward>();

	private Dictionary<int, EraReward> m_DicEraRewardGongXian = new Dictionary<int, EraReward>();

	private Dictionary<int, EraTask> m_DicEraRewardJuanXian = new Dictionary<int, EraTask>();

	private List<EraTask> m_ListEraTask = new List<EraTask>();

	public EraData data = new EraData();

	private int m_JiYuanKey = -1;

	private int m_GongXianMax = -1;

	private string[] strTimes = new string[2];

	private long m_PaiHangBangTime;

	private string m_StrShuoMing = string.Empty;

	private string m_StrTitle = string.Empty;
}
