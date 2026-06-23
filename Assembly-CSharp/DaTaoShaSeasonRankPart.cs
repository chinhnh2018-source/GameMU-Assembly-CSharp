using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class DaTaoShaSeasonRankPart : UserControl, IMUEventManagerHandler
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
		this.ItemCollection = this.mListBox.Items;
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.RequestRankInfo();
	}

	private void InitTextInPrefabs()
	{
		this.LblRank.Text = Global.GetLang("我的排名：");
		this.LblDes.Text = Global.GetLang(string.Empty);
		this.LblServer.Text = Global.GetLang("服务器：");
		this.LblZhanLi.Text = Global.GetLang("战    力：");
		this.LblScore.Text = Global.GetLang("段位积分：");
		this.LblMingCi.Text = Global.GetLang("名次");
		this.LblTeamNameDes.Text = Global.GetLang("战队名称");
		this.LblTeamLeader.Text = Global.GetLang("服务器");
		this.LblTeamDuanWeiDes.Text = Global.GetLang("段位积分");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
	}

	private void InitValue()
	{
		for (int i = 0; i < 5; i++)
		{
			this.LoadItem(0, null);
		}
	}

	private GameObject LoadItem(int index = 0, KFEscapeRankInfo ItemData = null)
	{
		if (ItemData == null)
		{
			return null;
		}
		int num = index + 1;
		GameObject gameObject = Object.Instantiate<GameObject>(this.mItem);
		gameObject.SetActive(true);
		Transform transform = gameObject.transform;
		UISprite component = transform.FindChild("SpriteRank").GetComponent<UISprite>();
		TextBlock component2 = transform.FindChild("LblRank").GetComponent<TextBlock>();
		if (num <= 3)
		{
			NGUITools.SetActive(component2.gameObject, false);
			component.spriteName = "no." + num;
		}
		else
		{
			NGUITools.SetActive(component.gameObject, false);
			component2.Text = Global.GetLang("第") + num + Global.GetLang("名");
		}
		TextBlock component3 = transform.FindChild("LblTeamName").GetComponent<TextBlock>();
		if (component3 != null)
		{
			component3.Text = ItemData.StrParam1;
		}
		TextBlock component4 = transform.FindChild("LblTeamLeader").GetComponent<TextBlock>();
		if (component4 != null)
		{
			component4.Text = Global.FormatRoleNameZoneid(ItemData.ZoneID, null, 0, 1);
		}
		TextBlock component5 = transform.FindChild("LblTeamDuanWei").GetComponent<TextBlock>();
		if (component5 != null)
		{
			component5.Text = ItemData.Value.ToString();
		}
		this.ItemCollection.Add(gameObject);
		UIPanel component6 = gameObject.GetComponent<UIPanel>();
		if (component6 != null)
		{
			Object.Destroy(component6);
		}
		return gameObject;
	}

	private int ServerName
	{
		set
		{
			if (value <= 0)
			{
				this.LblServer.Text = null;
			}
			else
			{
				this.LblServer.Text = Global.GetString(new object[]
				{
					this.mColor,
					Global.GetLang("服务器："),
					this.mWhiteColor,
					Global.FormatRoleNameZoneid(value, null, 0, 1)
				});
			}
		}
	}

	private string ZhanLi
	{
		set
		{
			this.LblZhanLi.Text = Global.GetString(new object[]
			{
				this.mColor,
				Global.GetLang("战    力："),
				this.mWhiteColor,
				value
			});
		}
	}

	private string DuanWeiScore
	{
		set
		{
			this.LblScore.Text = Global.GetString(new object[]
			{
				this.mColor,
				Global.GetLang("段位积分："),
				this.mWhiteColor,
				value
			});
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_RANK_INFO", new Action<MUSocketConnectEventArgs>(this.RespondRankInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ESCAPE_RANK_INFO", new Action<MUSocketConnectEventArgs>(this.RespondRankInfo));
	}

	public void RequestRankInfo()
	{
		GameInstance.Game.RequestDaTaoShaRankData();
	}

	public void RespondRankInfo(MUSocketConnectEventArgs e)
	{
		EscapeBattleRankInfo escapeBattleRankInfo = DataHelper.BytesToObject<EscapeBattleRankInfo>(e.bytesData, 0, e.bytesData.Length);
		if (escapeBattleRankInfo != null)
		{
			int selfRank = escapeBattleRankInfo.SelfRank;
			string text = string.Empty;
			if (selfRank <= 0)
			{
				text = Global.GetLang("未上榜");
			}
			else
			{
				text = selfRank.ToString();
			}
			this.LblRank.Text = Global.GetString(new object[]
			{
				this.mColor,
				Global.GetLang("我的排名："),
				text
			});
			this.LblDes.Text = escapeBattleRankInfo.myZhanDuiRankInfo.StrParam1;
			this.ServerName = escapeBattleRankInfo.myZhanDuiRankInfo.ZoneID;
			this.ZhanLi = escapeBattleRankInfo.myZhanDuiRankInfo.Param1.ToString();
			this.DuanWeiScore = escapeBattleRankInfo.myZhanDuiRankInfo.Value + string.Empty;
			string empty = string.Empty;
			this.mDuanWeiIcon.URL = this.GetDuanWeiIconPath(escapeBattleRankInfo.myZhanDuiRankInfo.Value, out empty);
			this.LblDuanWeiName.Text = Global.GetLang(empty);
			List<KFEscapeRankInfo> rankInfo2Client = escapeBattleRankInfo.rankInfo2Client;
			if (rankInfo2Client != null && rankInfo2Client.Count > 0)
			{
				for (int i = 0; i < rankInfo2Client.Count; i++)
				{
					this.LoadItem(i, rankInfo2Client[i]);
				}
			}
		}
		else
		{
			this.LblRank.Text = Global.GetString(new object[]
			{
				this.mColor,
				Global.GetLang("我的排名：无")
			});
			this.LblDes.Text = string.Empty;
			this.ServerName = 0;
			this.ZhanLi = string.Empty;
			this.DuanWeiScore = string.Empty;
		}
	}

	private string GetDuanWeiIconPath(int duanWeiValue, out string duanWeiName)
	{
		duanWeiName = string.Empty;
		int num = -1;
		int rankValue = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(1).RankValue;
		int rankValue2 = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(2).RankValue;
		int rankValue3 = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(3).RankValue;
		int rankValue4 = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(4).RankValue;
		int rankValue5 = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(5).RankValue;
		if (duanWeiValue < rankValue2)
		{
			num = 1;
		}
		else if (rankValue2 <= duanWeiValue && duanWeiValue < rankValue3)
		{
			num = 2;
		}
		else if (rankValue3 <= duanWeiValue && duanWeiValue < rankValue4)
		{
			num = 3;
		}
		else if (rankValue4 <= duanWeiValue && duanWeiValue < rankValue5)
		{
			num = 4;
		}
		else if (duanWeiValue >= rankValue5)
		{
			num = 5;
		}
		string result = string.Empty;
		switch (num)
		{
		case 1:
			result = "NetImages/GameRes/Images/TeamCompete/qingtong_b.png.qj";
			duanWeiName = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(1).RankLevelName;
			break;
		case 2:
			result = "NetImages/GameRes/Images/TeamCompete/baiyin_b.png.qj";
			duanWeiName = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(2).RankLevelName;
			break;
		case 3:
			result = "NetImages/GameRes/Images/TeamCompete/huangjin_b.png.qj";
			duanWeiName = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(3).RankLevelName;
			break;
		case 4:
			result = "NetImages/GameRes/Images/TeamCompete/zhuanshi_b.png.qj";
			duanWeiName = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(4).RankLevelName;
			break;
		case 5:
			result = "NetImages/GameRes/Images/TeamCompete/wangzheng_b.png.qj";
			duanWeiName = IConfigbase<ConfigDaTaoSha>.Instance.GeEscapeDanListVoDataById(5).RankLevelName;
			break;
		}
		return result;
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblRank;

	public TextBlock LblDes;

	public TextBlock LblServer;

	public TextBlock LblZhanLi;

	public TextBlock LblScore;

	public TextBlock LblMingCi;

	public TextBlock LblTeamNameDes;

	public TextBlock LblTeamLeader;

	public TextBlock LblTeamDuanWeiDes;

	public GButton BtnClose;

	public TextBlock LblDuanWeiName;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public ShowNetImage mDuanWeiIcon;

	public GameObject mItem;

	private string mColor = "{DDD4B3}";

	private string mWhiteColor = "{f0f0f0}";
}
