using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class PlayerRecall : UserControl
{
	protected override void InitializeComponent()
	{
		this.recallItemTable.IsPosYFixed = true;
		this.InitRecallItems();
		GameInstance.Game.SenRetureXml();
	}

	public void InitRecallData()
	{
		this.GetRecallDataRequest();
	}

	private void InitRecallItems()
	{
		if (this.recallItems == null)
		{
			this.recallItems = new PlayerRecallItem[4];
		}
		for (int i = 0; i < 4; i++)
		{
			if (null == this.recallItems[i])
			{
				this.recallItems[i] = U3DUtils.NEW<PlayerRecallItem>();
				this.recallItems[i].background.URL = Global.GetGameResImageString(string.Format("PlayerRecall/recallItem_background_{0}.jpg", i + 1));
				this.recallItems[i].itemIndex = i;
			}
			this.recallItems[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (!this.isTween)
				{
					this.InitRecallItemAtIndex(e.ID);
				}
			};
			U3DUtils.AddChild(this.recallItemTable.gameObject, this.recallItems[i].gameObject, false);
			this.recallItems[i].activityTipIcon.SetActive(false);
			if (i == 0)
			{
				if (HuoDongCommonFlag.GetLaoWanJiaActivityTipTypesState(ActivityTipTypes.Recall_SignIn))
				{
					this.recallItems[i].activityTipIcon.SetActive(true);
				}
				ActivityTipManager.RegActivityTipItem(14103, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			}
			else if (i == 1)
			{
				if (HuoDongCommonFlag.GetLaoWanJiaActivityTipTypesState(ActivityTipTypes.Recall_GiftSet))
				{
					this.recallItems[i].activityTipIcon.SetActive(true);
				}
				ActivityTipManager.RegActivityTipItem(14102, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			}
			else if (i == 2)
			{
				if (HuoDongCommonFlag.GetLaoWanJiaActivityTipTypesState(ActivityTipTypes.SpecPActAward))
				{
					this.recallItems[i].activityTipIcon.SetActive(true);
				}
				ActivityTipManager.RegActivityTipItem(14115, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			}
		}
	}

	private void SetRecallItemInfo(int index, string info)
	{
		if (index < 0 || index >= this.recallItems.Length)
		{
			return;
		}
		this.recallItems[index].SetRecallItemInfo(info);
	}

	public void OnRefereeCountsChanged(int totalRecallPlayer)
	{
		if (this.recallItems == null || this.recallItems.Length <= 0)
		{
			return;
		}
		this.totalRecallPlayers = totalRecallPlayer;
		this.SetRecallItemInfo(0, totalRecallPlayer.ToString());
	}

	private void ActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (args == null)
		{
			return;
		}
		if (this.recallItems == null || this.recallItems.Length <= 0)
		{
			return;
		}
		HuoDongCommonFlag.SetLaoWanJiaActivityTipTypesState((ActivityTipTypes)type, args.IsActive);
		int num = -1;
		if (type == 14103)
		{
			num = 0;
			if (!args.IsActive || null != this.signIn)
			{
			}
		}
		else if (type == 14102)
		{
			num = 1;
			if (args.IsActive && null != this.giftSet)
			{
				this.giftSet.GetRecallGiftSetRequest();
			}
		}
		else if (type == 14115)
		{
			num = 2;
			if (!args.IsActive || null != this.leiChong)
			{
			}
		}
		bool laoWanJiaActivityTipTypesState = HuoDongCommonFlag.GetLaoWanJiaActivityTipTypesState((ActivityTipTypes)type);
		this.recallItems[num].activityTipIcon.SetActive(laoWanJiaActivityTipTypesState);
		this.recallItems[num].detailLabel.gameObject.SetActive(!laoWanJiaActivityTipTypesState);
		if (!laoWanJiaActivityTipTypesState && num == this.selectedIndex)
		{
			this.recallItems[num].detailLabel.gameObject.SetActive(true);
			this.recallItems[num].detailLabel.text = Global.GetLang("点击收起");
		}
	}

	private new void OnDestroy()
	{
		if (this.recallItems != null)
		{
			for (int i = 0; i < 4; i++)
			{
				if (null != this.recallItems[i])
				{
					Object.Destroy(this.recallItems[i].gameObject);
					this.recallItems[i] = null;
				}
			}
		}
		this.recallItems = null;
		if (null != this.giftSet)
		{
			Object.Destroy(this.giftSet.gameObject);
			this.giftSet = null;
		}
		if (null != this.shop)
		{
			Object.Destroy(this.shop.gameObject);
			this.shop = null;
		}
		if (null != this.signIn)
		{
			Object.Destroy(this.signIn.gameObject);
			this.signIn = null;
		}
		ActivityTipManager.RegActivityTipItem(14103, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14102, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14104, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14115, new ActivityTipEventHandler(this.ActivityTipEventHandler));
	}

	private void SetRecallItemToggleState(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.recallItems[this.selectedIndex].toggleState = !this.recallItems[this.selectedIndex].toggleState;
					return;
				}
				if (this.recallItems[this.selectedIndex].toggleState)
				{
					this.recallItems[this.selectedIndex].toggleState = false;
				}
			}
			this.selectedIndex = index;
			this.recallItems[this.selectedIndex].toggleState = true;
		}
	}

	private void TweenPositionRecallItemAtIndex(int index)
	{
		this.toggleState = !this.toggleState;
		if (index == this.selectedIndex)
		{
			Vector3 vector = this.fromPos;
			this.fromPos = this.toPos;
			this.toPos = vector;
			this.isTween = true;
			this.tweenPosition.Play(this.toggleState);
		}
		else
		{
			float num = -((float)index * (161f + this.recallItemTable.padding.x * 2f));
			this.fromPos = this.toPos;
			this.toPos = new Vector3(num, this.toPos.y, this.toPos.z);
			this.tweenPosition.from = this.fromPos;
			this.tweenPosition.to = this.toPos;
			this.isTween = true;
			this.tweenPosition.Play(this.toggleState);
		}
		this.tweenPosition.onFinished = new UITweener.OnFinished(this.OnTweenPositionFinished);
	}

	private void OnTweenPositionFinished(UITweener tween)
	{
		this.isTween = false;
		for (int i = 0; i < this.recallItems.Length; i++)
		{
			PlayerRecallItem playerRecallItem = this.recallItems[i];
			playerRecallItem.transform.localPosition = NGUITools.Round(playerRecallItem.transform.localPosition);
			playerRecallItem.position = NGUITools.Round(playerRecallItem.transform.localPosition);
		}
	}

	private void InitRecallItemAtIndex(int index)
	{
		switch (index)
		{
		case 0:
			if (null == this.signIn)
			{
				this.signIn = U3DUtils.NEW<RecallSignIn>();
				this.recallItems[index].content.Add(this.signIn);
			}
			this.signIn.OnActive();
			break;
		case 1:
			if (null == this.giftSet)
			{
				this.giftSet = U3DUtils.NEW<RecallGiftSet>();
				this.recallItems[index].content.Add(this.giftSet);
			}
			this.giftSet.OnActive();
			break;
		case 2:
			if (null == this.leiChong)
			{
				this.leiChong = U3DUtils.NEW<RecallLeiChong>();
				this.recallItems[index].content.Add(this.leiChong);
			}
			this.leiChong.OnActive();
			break;
		case 3:
			if (null == this.shop)
			{
				this.shop = U3DUtils.NEW<RecallShop>();
				this.recallItems[index].content.Add(this.shop);
			}
			this.shop.OnActive();
			break;
		}
		this.TweenPositionRecallItemAtIndex(index);
		this.recallItems[index].detailLabel.text = Global.GetLang("点击收起");
		if (index != this.selectedIndex && this.selectedIndex != -1)
		{
			this.recallItems[this.selectedIndex].detailLabel.text = Global.GetLang("点击查看");
		}
		else if (index == this.selectedIndex && this.selectedIndex != -1)
		{
			if (this.recallItems[index].toggleState)
			{
				this.recallItems[index].detailLabel.text = Global.GetLang("点击查看");
			}
			else
			{
				this.recallItems[index].detailLabel.text = Global.GetLang("点击收起");
			}
		}
		if (index == 3)
		{
			this.shop.m_Scroll.scrollValue = 0f;
		}
		this.SetRecallItemToggleState(index);
	}

	public static string HintMessage_Unserviceable()
	{
		string empty = string.Empty;
		XElement xelement = null;
		if (RecallGoodsEx.m_UserReturnXmlData != null)
		{
			for (int i = 0; i < RecallGoodsEx.m_UserReturnXmlData.XmlNameList.Count; i++)
			{
				if (RecallGoodsEx.m_UserReturnXmlData.XmlNameList[i].Equals("HuoDongZhaoHui.xml"))
				{
					xelement = XElement.Parse(RecallGoodsEx.m_UserReturnXmlData.XmlList[i]);
				}
			}
		}
		if (xelement == null)
		{
			return empty;
		}
		XElement xelement2 = Global.GetXElement(xelement, "HuoDongZhaoHui", "ID", "1");
		if (xelement2 == null)
		{
			return empty;
		}
		string text = "0";
		string text2 = "0";
		string text3 = PlayerRecall.TimeStringToDateStr(Global.GetXElementAttributeStr(xelement2, "NotLoggedInBegin"));
		string text4 = PlayerRecall.TimeStringToDateStr(Global.GetXElementAttributeStr(xelement2, "NotLoggedInFinish"));
		string[] array = Global.GetXElementAttributeStr(xelement2, "Level").Split(new char[]
		{
			','
		});
		if (array != null && array.Length > 1)
		{
			text = array[0];
			text2 = array[1];
		}
		return string.Format(Global.GetLang("在{0}至{1}内未登录，{2}转{3}级以上用户才可被召回"), new object[]
		{
			text3,
			text4,
			text,
			text2
		});
	}

	public static string TimeStringToDateStr(string time)
	{
		if (string.IsNullOrEmpty(time))
		{
			return string.Empty;
		}
		DateTime dateTime;
		DateTime.TryParse(time, ref dateTime);
		return dateTime.ToString(Global.GetLang("yyyy年M月d日"));
	}

	public static string TimeStringToDateStrMD(string time)
	{
		if (string.IsNullOrEmpty(time))
		{
			return string.Empty;
		}
		DateTime dateTime;
		DateTime.TryParse(time, ref dateTime);
		return dateTime.ToString(Global.GetLang("M月d日"));
	}

	private void GetRecallDataRequest()
	{
		Super.ShowNetWaiting(null);
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.GetReturnData();
	}

	public void DidReceiveData(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.CmdID == 900)
		{
			UserReturnData userReturnData = DataHelper.BytesToObject<UserReturnData>(e.bytesData, 0, e.bytesData.Length);
			if (userReturnData == null)
			{
				return;
			}
			ServerBufferZhaoHui.Instance.userData = userReturnData;
			this._updateAllUI();
		}
		else if (e.CmdID == 901)
		{
			EReturnState ereturnState = DataHelper.BytesToObject<EReturnState>(e.bytesData, 0, e.bytesData.Length);
			PlayerRecall.ErrorProcessEReturnState(ereturnState);
			if (ereturnState == -3)
			{
				return;
			}
			ServerBufferZhaoHui.Instance.eReturnState = ereturnState;
		}
		else if (e.CmdID == 902)
		{
			if (e.fields == null || e.fields.Length < 3)
			{
				return;
			}
			ServerBufferZhaoHui.Instance.eReturnAwardState = Global.SafeConvertToInt32(e.fields[0]);
			ServerBufferZhaoHui.Instance.eReturnAwardType = Global.SafeConvertToInt32(e.fields[1]);
			ServerBufferZhaoHui.Instance.eReturnAwardData = e.fields[2];
			PlayerRecall.ErrorProcessEReturnAwardState(ServerBufferZhaoHui.Instance.eReturnAwardState);
			if (ServerBufferZhaoHui.Instance.eReturnAwardState == 1 || ServerBufferZhaoHui.Instance.eReturnAwardState == null)
			{
				int eReturnAwardType = ServerBufferZhaoHui.Instance.eReturnAwardType;
				if (ServerBufferZhaoHui.Instance.userData.AwardDic.ContainsKey(eReturnAwardType))
				{
					string[] array = ServerBufferZhaoHui.Instance.eReturnAwardData.Split(new char[]
					{
						'*'
					});
					int[] array2 = new int[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = Convert.ToInt32(array[i]);
					}
					ServerBufferZhaoHui.Instance.userData.AwardDic[eReturnAwardType] = array2;
				}
			}
			GameInstance.Game.GetReturnData();
			this._updateAllUI();
		}
	}

	public void _updateAllUI()
	{
		if (this.leiChong != null)
		{
			this.leiChong.UpdateUIOnServerDataChanged();
		}
		if (this.giftSet != null)
		{
			this.giftSet.UpdateUIOnServerDataChanged();
		}
		if (this.signIn != null)
		{
			this.signIn.UpdateUIOnServerDataChanged();
		}
		if (this.shop != null)
		{
			this.shop.UpdateUIOnServerDataChanged();
		}
	}

	public static int ErrorProcessEReturnAwardState(EReturnAwardState ret)
	{
		string text = string.Empty;
		switch (ret + 9)
		{
		case 0:
			text = string.Format(Global.GetLang(" 超过购买上限"), new object[0]);
			break;
		case 1:
			text = string.Format(Global.GetLang(" vip等级不足"), new object[0]);
			break;
		case 2:
			text = string.Format(Global.GetLang(" 活动未开启"), new object[0]);
			break;
		case 3:
			text = string.Format(Global.GetLang(" 钻石不足"), new object[0]);
			break;
		case 4:
			text = string.Format(Global.GetLang(" 背包已满"), new object[0]);
			break;
		case 5:
			text = string.Format(Global.GetLang(" 已经领取"), new object[0]);
			break;
		case 6:
			text = string.Format(Global.GetLang(" 不是召回人"), new object[0]);
			break;
		case 7:
			text = string.Format(Global.GetLang("不是回归角色"), new object[0]);
			break;
		case 8:
			text = string.Format(Global.GetLang("失败"), new object[0]);
			break;
		case 9:
			text = string.Format(Global.GetLang(string.Empty), new object[0]);
			break;
		case 10:
			text = string.Format(Global.GetLang("成功"), new object[0]);
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, text, 0, -1, -1, 0);
		}
		return ret;
	}

	public static int ErrorProcessEReturnState(EReturnState ret)
	{
		string text = string.Empty;
		switch (ret + 14)
		{
		case 0:
			text = string.Format(Global.GetLang("超过购买上限"), new object[0]);
			break;
		case 1:
			text = string.Format(Global.GetLang("等级不足"), new object[0]);
			break;
		case 2:
			text = string.Format(Global.GetLang("vip等级不够"), new object[0]);
			break;
		case 3:
			text = string.Format(Global.GetLang("平台不同"), new object[0]);
			break;
		case 4:
			text = string.Format(Global.GetLang("活动未开启"), new object[0]);
			break;
		case 5:
			text = string.Format(Global.GetLang("召回人不存在"), new object[0]);
			break;
		case 6:
			text = string.Format(Global.GetLang("推荐人不能是自己"), new object[0]);
			break;
		case 7:
			text = string.Format(Global.GetLang("不符合召回条件"), new object[0]);
			break;
		case 8:
			text = string.Format(Global.GetLang("验证中"), new object[0]);
			break;
		case 9:
			text = string.Format(Global.GetLang("已经被召回"), new object[0]);
			break;
		case 10:
			text = string.Format(Global.GetLang("超时"), new object[0]);
			break;
		case 11:
			text = string.Format(Global.GetLang("验证失败"), new object[0]);
			break;
		case 12:
			text = string.Format(Global.GetLang("校验失败"), new object[0]);
			break;
		case 13:
			text = string.Format(Global.GetLang("校验失败，资格不够"), new object[0]);
			break;
		case 14:
			break;
		case 15:
			text = string.Format(Global.GetLang("等待验证资格"), new object[0]);
			break;
		case 16:
			text = string.Format(Global.GetLang("验证成功"), new object[0]);
			break;
		case 17:
			text = string.Format(Global.GetLang("等待验证召回码"), new object[0]);
			break;
		case 18:
			text = string.Format(Global.GetLang("验证成功"), new object[0]);
			break;
		default:
			switch (ret + 52)
			{
			case 0:
				text = string.Format(Global.GetLang("该帐号下已经有其他角色被召回"), new object[0]);
				break;
			case 1:
				text = string.Format(Global.GetLang("未通过资格老玩家验证"), new object[0]);
				break;
			case 2:
				text = string.Format(Global.GetLang("未通过老玩家推荐人验证"), new object[0]);
				break;
			default:
				if (ret != -100)
				{
					if (ret != -99)
					{
					}
				}
				else
				{
					text = string.Format(Global.GetLang("数据重复"), new object[0]);
				}
				break;
			}
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, text, 0, -1, -1, 0);
		}
		return ret;
	}

	private const int itemCount = 4;

	private RecallGiftSet giftSet;

	private RecallLeiChong leiChong;

	private RecallSignIn signIn;

	private RecallShop shop;

	public UITable recallItemTable;

	public TweenPosition tweenPosition;

	private PlayerRecallItem[] recallItems;

	private int selectedIndex = -1;

	private bool isTween;

	private int totalRecallPlayers;

	private Vector3 fromPos = Vector3.zero;

	private Vector3 toPos = Vector3.zero;

	private bool toggleState;
}
