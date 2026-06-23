using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RecallGiftSet : RecallGoodsEx
{
	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
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
		XElement xelement = Global.GetXElement(base.xml, "GiftList");
		string text = Global.GetLang("成为回归用户即可领取(每个帐号只能领取1次)");
		if (xelement != null)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Title");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				text = xelementAttributeStr;
			}
		}
		if (null != this.activityInfo)
		{
			this.activityInfo.text = text;
		}
		if (base.xmlList == null)
		{
			return false;
		}
		this._itemCollection.Clear();
		for (int i = 0; i < base.xmlList.Count; i++)
		{
			xelement = base.xmlList[i];
			int xmlID = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "ID"));
			int minRecruitNum = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "RecruitNum"));
			int minLevel = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinLevel"));
			int num = Global.SafeConvertToInt32(Global.GetXElementAttributeStr(xelement, "MinVip"));
			RecallRewardItem recallRewardItem = base.CreateRecallRewardItem(true);
			recallRewardItem.index = i;
			recallRewardItem.xmlID = xmlID;
			recallRewardItem.minRecruitNum = minRecruitNum;
			recallRewardItem.minLevel = minLevel;
			recallRewardItem.minVip = num;
			recallRewardItem.label.text = StringUtil.substitute(Global.GetLang("VIP{0}回归奖励"), new object[]
			{
				num
			});
			recallRewardItem.GoodsIDs = Global.GetXElementAttributeStr(xelement, "GoodsID1") + "@" + Global.GetXElementAttributeStr(xelement, "GoodsID2");
		}
		base._UpdateRewardsListStatus();
		return true;
	}

	public void SetGiftSetStatus(int status)
	{
	}

	public void GetGiftSetInfo(int status)
	{
		this.SetGiftSetStatus(status);
	}

	public void SetPickupRecallGiftSetResult(int status)
	{
		string textMsg = string.Empty;
		switch (status + 7)
		{
		case 0:
			textMsg = PlayerRecall.HintMessage_Unserviceable();
			break;
		case 3:
			textMsg = string.Format(Global.GetLang("不能重复领取"), new object[0]);
			break;
		case 4:
			textMsg = string.Format(Global.GetLang("背包空间不足"), new object[0]);
			break;
		case 5:
			textMsg = string.Format(Global.GetLang("活动已结束"), new object[0]);
			break;
		case 6:
		case 9:
		case 10:
			textMsg = string.Format(Global.GetLang("领取失败"), new object[0]);
			break;
		case 8:
			this.signStatus.gameObject.SetActive(true);
			break;
		}
		if (status != 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	public void GetRecallGiftSetRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
	}

	private void PickupRecallGiftSetRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.PickupRecallGiftSet(Global.Data.UserID);
	}

	public UISprite signStatus;
}
