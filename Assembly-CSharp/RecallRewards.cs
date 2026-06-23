using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class RecallRewards : RecallGoodsEx
{
	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
		this.signExtraAwardsBtn.Text = Global.GetLang("领取");
		this.inputStatement.text = Global.GetLang("(被推荐人需填此ID)");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.SetActivityContent();
		this.signExtraAwardsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SignInRecallExtraAwardRequest();
		};
		this.SetAvailableExtraRewards(0);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.SetMyRecallIDNew();
	}

	public override void OnDragFinished()
	{
		if (null == this.goodsList)
		{
			return;
		}
		int num = this.goodsList.Count();
		Vector3 vector = this.dragPanel.transform.localPosition - this.panelPosition;
		Vector3 size = RecallRewardItem.GetSize(base.GetType());
		int num2 = 0;
		if (vector.y > 0f)
		{
			num2 = Mathf.RoundToInt(vector.y / size.y);
		}
		if (num2 >= num - 1)
		{
			num2 = num - 1;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 == 0 || num2 == num - 1)
		{
			Vector3 vector2 = this.panelPosition + new Vector3(0f, size.y * (float)num2, 0f);
			SpringPanel.Begin(this.dragPanel.gameObject, vector2, 10f);
		}
	}

	public override bool InitRewards()
	{
		if (!base.InitRewards())
		{
			return false;
		}
		if (base.xmlList == null)
		{
			return false;
		}
		this._itemCollection.Clear();
		for (int i = 0; i < base.xmlList.Count; i++)
		{
			XElement xelement = base.xmlList[i];
			int num = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "ID"));
			int minRecruitNum = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "RecruitNum"));
			int minLevel = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinLevel"));
			int minVip = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinVip"));
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Description");
			if (this.mTab.RewardIDStateDic.ContainsKey(num))
			{
				RecallRewardItem recallRewardItem = base.CreateRecallRewardItem(true);
				recallRewardItem.index = i;
				recallRewardItem.xmlID = num;
				recallRewardItem.minRecruitNum = minRecruitNum;
				recallRewardItem.minLevel = minLevel;
				recallRewardItem.minVip = minVip;
				recallRewardItem.Description = xelementAttributeStr;
				recallRewardItem.label.text = xelementAttributeStr;
				recallRewardItem.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsID1") + "@" + Global.GetXElementAttributeStr(xelement, "GoodsID2");
			}
		}
		base._UpdateRewardsListStatus();
		this.SetExtraRewardIcon();
		this.GetRecallAwardRequest();
		return true;
	}

	private void SetMyRecallIDNew()
	{
		string text = string.Empty;
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			text = ServerBufferZhaoHui.Instance.userData.MyCode;
		}
		if (null != this.userID)
		{
			this.userID.text = text + Global.GetLang("  (被推荐人需填此ID)");
		}
		int num = 10 * text.Length;
		this.SetInputStatement((float)num);
		this.m_SubmitShare.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text2 = ConfigSystemParam.GetSystemParamByName("OldHttp", true);
			if (text2 == null || text2 == string.Empty)
			{
				text2 = "http://www.tmsk.cn/zhaohui/";
			}
			PlatSDKMgr.WXShareUrl(text2, string.Concat(new object[]
			{
				Global.GetLang("好久不见了，我在"),
				Global.Data.GameServerID,
				Global.GetLang("区，ID："),
				this.userID.text,
				Global.GetLang("现在回归即可领取丰厚好礼!")
			}));
		};
	}

	private void SetInputStatement(float x)
	{
		if (null == this.inputStatement)
		{
			return;
		}
		Vector3 localPosition = this.inputStatement.transform.localPosition;
		localPosition.x += x;
		this.inputStatement.transform.localPosition = localPosition;
	}

	private void SetActivityContent()
	{
		if (null != this.activityContent)
		{
			this.activityContent.text = this.HintMessage_ActivityCondition();
		}
	}

	private void SetAvailableExtraRewards(int extraRewards = 0)
	{
	}

	private void SetExtraRewardIcon()
	{
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("V4ZhaoHuiAward", true);
		if (string.IsNullOrEmpty(systemParamByName))
		{
			return;
		}
		GGoodIcon ggoodIcon = RecallRewardItem.LoadRewardItemGoodsIcon(systemParamByName, false, true, false);
		if (null != ggoodIcon && null != this.extraAwardIcon)
		{
			U3DUtils.AddChild(this.extraAwardIcon, ggoodIcon.gameObject, false);
		}
	}

	private string HintMessage_ActivityCondition()
	{
		string xmlName = "Config/PlayerRecall/HuoDongZhaoHui.xml";
		XElement isolateResXml = Global.GetIsolateResXml(xmlName);
		if (isolateResXml == null)
		{
			return null;
		}
		string empty = string.Empty;
		XElement xelement = Global.GetXElement(isolateResXml, "HuoDongZhaoHui", "ID", "1");
		if (xelement == null)
		{
			return null;
		}
		string text = PlayerRecall.TimeStringToDateStr(Global.GetXElementAttributeStr(xelement, "NotLoggedInBegin"));
		string text2 = PlayerRecall.TimeStringToDateStr(Global.GetXElementAttributeStr(xelement, "NotLoggedInFinish"));
		string text3 = "0";
		string text4 = "0";
		string[] array = Global.GetXElementAttributeStr(xelement, "Level").Split(new char[]
		{
			','
		});
		if (array != null && array.Length > 1)
		{
			text3 = array[0];
			text4 = array[1];
		}
		return string.Format(Global.GetLang("可召回{0}至{1}内未登录，{2}转{3}级以上用户"), new object[]
		{
			text,
			text2,
			text3,
			text4
		});
	}

	public void GetRecallAwardRequest()
	{
		ServerBufferZhaoHui.Instance.SendGetAwardList();
	}

	private void SignInRecallExtraAwardRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
	}

	public TextBlock userID;

	public TextBlock availableAwards;

	public GButton signExtraAwardsBtn;

	public GameObject extraAwardIcon;

	public UILabel activityContent;

	public UILabel inputStatement;

	public RecallRewards.OnRefereeCountsChangedDelegate onRefereeCountsChangedDelegate;

	public GButton m_SubmitShare;

	public delegate void OnRefereeCountsChangedDelegate(int totalRecallPlayer);
}
