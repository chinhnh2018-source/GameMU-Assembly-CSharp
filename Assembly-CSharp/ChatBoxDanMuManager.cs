using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChatBoxDanMuManager : UserControl
{
	public static bool BeDanMuOpen(DanMuType type)
	{
		if (type == DanMuType.Team)
		{
			return PlayerPrefs.GetInt("TeamDanMu", 1) == 1;
		}
		return type == DanMuType.ZhanMeng && PlayerPrefs.GetInt("ZhanMengDanMu", 1) == 1;
	}

	public static void SetDanMuOpen(DanMuType type, bool beOpen)
	{
		if (type == DanMuType.Team)
		{
			PlayerPrefs.SetInt("TeamDanMu", (!beOpen) ? 0 : 1);
		}
		else if (type == DanMuType.ZhanMeng)
		{
			PlayerPrefs.SetInt("ZhanMengDanMu", (!beOpen) ? 0 : 1);
		}
		if (!beOpen)
		{
			PlayZone playZone = Super.GData.PlayZoneRoot as PlayZone;
			if (playZone.DanMuManager != null)
			{
				playZone.DanMuManager.CloseDanMuByType(type);
			}
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		for (int i = 0; i < this.lstCenterContainer.Count; i++)
		{
			this.m_lstFreeCenterContainer.Add(this.lstCenterContainer[i]);
		}
		for (int j = 0; j < this.lstOffContainer.Count; j++)
		{
			this.m_lstFreeOffContainer.Add(this.lstOffContainer[j]);
		}
	}

	private GameObject GetCenterDanMuShowContainer()
	{
		if (this.m_lstFreeCenterContainer.Count <= 0)
		{
			return null;
		}
		int num = Random.Range(0, this.m_lstFreeCenterContainer.Count);
		GameObject gameObject = this.m_lstFreeCenterContainer[num];
		this.m_lstFreeCenterContainer.Remove(gameObject);
		return gameObject;
	}

	private GameObject GetOffDanMuShowContainer()
	{
		if (this.m_lstFreeOffContainer.Count <= 0)
		{
			return null;
		}
		int num = Random.Range(0, this.m_lstFreeOffContainer.Count);
		GameObject gameObject = this.m_lstFreeOffContainer[num];
		this.m_lstFreeOffContainer.Remove(gameObject);
		return gameObject;
	}

	private GameObject GetDanMuShowContainer()
	{
		GameObject gameObject = this.GetCenterDanMuShowContainer();
		if (gameObject == null)
		{
			gameObject = this.GetOffDanMuShowContainer();
		}
		if (gameObject == null)
		{
			int num = Random.Range(0, this.lstCenterContainer.Count);
			gameObject = this.lstCenterContainer[num];
		}
		return gameObject;
	}

	private ChatBoxDanMuItem GetFreeItem()
	{
		if (this.m_lstFreeItem.Count == 0)
		{
			return null;
		}
		ChatBoxDanMuItem result = this.m_lstFreeItem[0];
		this.m_lstFreeItem.RemoveAt(0);
		return result;
	}

	private ChatBoxDanMuItem GetDanMuItem()
	{
		ChatBoxDanMuItem chatBoxDanMuItem = this.GetFreeItem();
		if (chatBoxDanMuItem == null)
		{
			chatBoxDanMuItem = U3DUtils.NEW<ChatBoxDanMuItem>();
		}
		this.m_lstShownItem.Add(chatBoxDanMuItem);
		chatBoxDanMuItem.OnMoveStep1Finish = new Action<ChatBoxDanMuItem>(this.OnMoveStep1Finish);
		chatBoxDanMuItem.OnMoveStep2Finish = new Action<ChatBoxDanMuItem>(this.OnMoveStep2Finish);
		chatBoxDanMuItem.transform.gameObject.SetActive(true);
		return chatBoxDanMuItem;
	}

	private void OnMoveStep1Finish(ChatBoxDanMuItem item)
	{
		if (this.lstCenterContainer.IndexOf(item.transform.parent.gameObject) >= 0)
		{
			if (this.m_lstFreeCenterContainer.IndexOf(item.transform.parent.gameObject) < 0)
			{
				this.m_lstFreeCenterContainer.Add(item.transform.parent.gameObject);
			}
		}
		else if (this.m_lstFreeOffContainer.IndexOf(item.transform.parent.gameObject) < 0)
		{
			this.m_lstFreeOffContainer.Add(item.transform.parent.gameObject);
		}
	}

	private void OnMoveStep2Finish(ChatBoxDanMuItem item)
	{
		this.m_lstShownItem.Remove(item);
		if (this.m_lstFreeItem.IndexOf(item) < 0)
		{
			this.m_lstFreeItem.Add(item);
		}
		item.transform.gameObject.SetActive(false);
	}

	private void ShowDanMu(string content, DanMuType type)
	{
		GameObject danMuShowContainer = this.GetDanMuShowContainer();
		ChatBoxDanMuItem danMuItem = this.GetDanMuItem();
		danMuItem.DanMuType = type;
		danMuItem.transform.SetParent(danMuShowContainer.transform);
		danMuItem.transform.localPosition = Vector3.zero;
		danMuItem.transform.localRotation = Quaternion.identity;
		danMuItem.transform.localScale = Vector3.one;
		danMuItem.MoveText(content, this.DanMuScreenWidth, Vector3.zero, this.MoveSpeed, 0.2f);
	}

	public void AddDanMuItem(ChatTextItem item)
	{
		if (item.ChatType == ChatType.Voice)
		{
			return;
		}
		DanMuType danMuType = DanMuType.None;
		if (item.ChatIndex == ChatTypeIndexes.Team)
		{
			danMuType = DanMuType.Team;
		}
		else if (item.ChatIndex == ChatTypeIndexes.Faction)
		{
			danMuType = DanMuType.ZhanMeng;
		}
		if ((danMuType == DanMuType.Team && ChatBoxDanMuManager.BeDanMuOpen(DanMuType.Team)) || (danMuType == DanMuType.ZhanMeng && ChatBoxDanMuManager.BeDanMuOpen(DanMuType.ZhanMeng)))
		{
			string text = GChat.FormatChatText(item, false, false);
			string pureText = NGUIHTML.GetPureText(text);
			this.ShowDanMu(pureText, danMuType);
		}
	}

	public void CloseDanMuByType(DanMuType type)
	{
		List<ChatBoxDanMuItem> list = new List<ChatBoxDanMuItem>();
		for (int i = 0; i < this.m_lstShownItem.Count; i++)
		{
			ChatBoxDanMuItem chatBoxDanMuItem = this.m_lstShownItem[i];
			if (chatBoxDanMuItem.DanMuType == type)
			{
				list.Add(chatBoxDanMuItem);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			this.OnMoveStep1Finish(list[j]);
			this.OnMoveStep2Finish(list[j]);
		}
	}

	private const string TeamDanMu = "TeamDanMu";

	private const string ZhanMengDanMu = "ZhanMengDanMu";

	public float DanMuScreenWidth;

	public float MoveSpeed = 100f;

	private List<ChatBoxDanMuItem> m_lstShownItem = new List<ChatBoxDanMuItem>();

	private List<ChatBoxDanMuItem> m_lstFreeItem = new List<ChatBoxDanMuItem>();

	public List<GameObject> lstCenterContainer;

	public List<GameObject> lstOffContainer;

	private List<GameObject> m_lstFreeCenterContainer = new List<GameObject>();

	private List<GameObject> m_lstFreeOffContainer = new List<GameObject>();
}
