using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShiLiNoticeHint : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this._Bak.gameObject).onClick = new UIEventListener.VoidDelegate(this.GoToNoticePlace);
	}

	public void InitContent(KFCompNotice notice)
	{
		this.m_notice = notice;
		MUCompNotice noticeByID = ShiLiData.GetNoticeByID(notice.NoticeID);
		if (noticeByID == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (Global.IsInShiLiZhengBaMap())
		{
			if (noticeByID.IsCompOpen(NoticeShowType.TopMessage))
			{
				flag = true;
			}
			if (noticeByID.IsCompOpen(NoticeShowType.NoticeMessage))
			{
				flag2 = true;
			}
			if (noticeByID.IsCompOpen(NoticeShowType.ChatMessage))
			{
				flag3 = true;
			}
		}
		else
		{
			if (noticeByID.IsOriginalMapOpen(NoticeShowType.TopMessage))
			{
				flag = true;
			}
			if (noticeByID.IsOriginalMapOpen(NoticeShowType.NoticeMessage))
			{
				flag2 = true;
			}
			if (noticeByID.IsOriginalMapOpen(NoticeShowType.ChatMessage))
			{
				flag3 = true;
			}
		}
		if (flag)
		{
			string noticeString = this.GetNoticeString(notice, noticeByID, NoticeShowType.TopMessage);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.SysHintAndChatBox, noticeString, 0, -1, -1, 0);
		}
		if (flag2)
		{
			string noticeString2 = this.GetNoticeString(notice, noticeByID, NoticeShowType.NoticeMessage);
			this.ShowHintText(noticeString2, 3);
		}
		if (flag3)
		{
			string noticeString3 = this.GetNoticeString(notice, noticeByID, NoticeShowType.ChatMessage);
			GChat.AddSystemChatMessage(noticeString3, ChatTypeIndexes.System);
			PlayZone playZone = Super.GData.PlayZoneRoot as PlayZone;
			if (playZone.GameChatBox != null)
			{
				playZone.GameChatBox.RefreshChat(7);
			}
			if (playZone.GameChatBoxMini != null)
			{
				playZone.GameChatBoxMini.RefreshChat(7);
			}
		}
	}

	public string GetNoticeString(KFCompNotice notice, MUCompNotice compNotice, NoticeShowType type)
	{
		string text = string.Empty;
		if (compNotice != null)
		{
			string intro = compNotice.Intro;
			if (type == NoticeShowType.TopMessage)
			{
				text = intro.Replace("#0", string.Empty);
			}
			else if (type == NoticeShowType.ChatMessage)
			{
				int num = ShiLiData.AddNotice(notice);
				string text2 = string.Format("｛NoticeID_{0}_0｝", num);
				string text3 = string.Format("{0}{1}{2}", text2, Global.GetColorStringForNGUIText(new object[]
				{
					"e43ff0",
					Global.GetLang("立即前往")
				}), "｛-｝");
				text = intro.Replace("#0", text3);
			}
			else if (type == NoticeShowType.NoticeMessage)
			{
				text = intro.Replace("#0", Global.GetColorStringForNGUIText(new object[]
				{
					"e43ff0",
					Global.GetLang("立即前往")
				}));
			}
			text = text.Replace("#1", notice.Param1);
			text = text.Replace("#2", notice.Param2);
		}
		return text;
	}

	private void GoToNoticePlace(GameObject go)
	{
		this.ShowNotice(false);
		ShiLiData.GoToNoticePlace(this.m_notice);
	}

	private void LateUpdate()
	{
		if (this.TimeOut != 0f)
		{
			this.TimeOut -= Time.deltaTime;
			if (this.TimeOut < 0.5f && this.TimeOut > 0f)
			{
				this.noticePanel.alpha = this.TimeOut * 2f;
			}
			else if (this.TimeOut <= 0f)
			{
				this.TimeOut = 0f;
				this.lblContent.Text = string.Empty;
				this.ShowNotice(false);
			}
		}
	}

	public void ShowHintText(string text = "", int timeOut = 3)
	{
		if (timeOut > 0)
		{
			this.TimeOut = (float)timeOut + 0.5f;
		}
		else
		{
			this.TimeOut = 0f;
		}
		this.lblContent.Text = text;
		this.ShowNotice(true);
	}

	private void ShowNotice(bool show)
	{
		this.noticePanel.alpha = 1f;
		this.noticePanel.gameObject.SetActive(show);
	}

	public Action OnClose;

	public TextBlock lblContent;

	public UIPanel noticePanel;

	public UISprite _Bak;

	private KFCompNotice m_notice;

	private float TimeOut;
}
