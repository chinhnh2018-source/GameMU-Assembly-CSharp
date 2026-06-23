using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChatBoxPrivateController : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.uiCollider = this.tableChat.gameObject.GetComponent<UICollider>();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnCloseCurrent();
		};
	}

	private void ResetBtns()
	{
		if (this.m_lstRoleItem.Count > this.m_lstRoleInfos.Count)
		{
			for (int i = this.m_lstRoleInfos.Count; i < this.m_lstRoleItem.Count; i++)
			{
				this.m_lstRoleItem[i].gameObject.SetActive(false);
			}
		}
		for (int j = 0; j < this.m_lstRoleInfos.Count; j++)
		{
			ChatBoxPrivateBtnItem chatBoxPrivateBtnItem;
			if (j < this.m_lstRoleItem.Count)
			{
				chatBoxPrivateBtnItem = this.m_lstRoleItem[j];
			}
			else
			{
				chatBoxPrivateBtnItem = U3DUtils.NEW<ChatBoxPrivateBtnItem>();
				chatBoxPrivateBtnItem.transform.SetParent(this.privateBtnContainer.transform);
				chatBoxPrivateBtnItem.transform.localScale = Vector3.one;
				this.m_lstRoleItem.Add(chatBoxPrivateBtnItem);
				chatBoxPrivateBtnItem.Draggablepanel = this._UIDraggablePanel;
				chatBoxPrivateBtnItem.OnSelectItem = new Action<ChatBoxPrivateBtnItem>(this.OnSelectBtnItem);
			}
			chatBoxPrivateBtnItem.SetSelectState(false);
			chatBoxPrivateBtnItem.RoleInfo = this.m_lstRoleInfos[j];
			chatBoxPrivateBtnItem.gameObject.transform.localPosition = new Vector3(0f, (float)(0 - 42 * j), 0f);
			chatBoxPrivateBtnItem.gameObject.SetActive(true);
		}
	}

	public void OnDispose()
	{
		if (this.ShownVoiceItemExList != null && this.ShownVoiceItemExList.Count > 0)
		{
			int count = this.ShownVoiceItemExList.Count;
			for (int i = 0; i < count; i++)
			{
				this.ShownVoiceItemExList[i].OnDispose();
			}
			this.ShownVoiceItemExList.Clear();
		}
		this.ShownVoiceItemExList = null;
	}

	private void OnCloseCurrent()
	{
		if (this.m_currentSelectItem == null)
		{
			return;
		}
		string roleName = this.m_currentSelectItem.RoleInfo.RoleName;
		for (int i = GChat.PrivateChatTextList.Count - 1; i > -1; i--)
		{
			string empty = string.Empty;
			this.GetRoleInfo(GChat.PrivateChatTextList[i], ref empty);
			if (empty == roleName)
			{
				GChat.PrivateChatTextList.Remove(GChat.PrivateChatTextList[i]);
			}
		}
		this.m_currentSelectItem = null;
		this.SetTalkRole(string.Empty);
	}

	private void OnSelectBtnItem(ChatBoxPrivateBtnItem item)
	{
		if (item == null)
		{
			this.lblUserName.text = string.Empty;
			this.btnClose.gameObject.SetActive(false);
			ChatBoxPrivateController.ShowChatContent(this.tableChat, this.ShownVoiceItemExList, new List<ChatTextItem>());
			if (this.OnChatRoleSelect != null)
			{
				this.OnChatRoleSelect.Invoke(null);
			}
			return;
		}
		if (item == this.m_currentSelectItem)
		{
			return;
		}
		if (this.m_currentSelectItem != null)
		{
			this.m_currentSelectItem.SetSelectState(false);
		}
		this.m_currentSelectItem = item;
		this.m_currentSelectItem.SetSelectState(true);
		this.lblUserName.text = string.Format(Global.GetLang("与{0}私聊"), Global.GetColorStringForNGUIText(new object[]
		{
			"f7d66b",
			item.RoleInfo.RoleName
		}));
		this.btnClose.gameObject.SetActive(true);
		this.ShowRoleChatcontents(this.m_currentSelectItem);
		if (this.OnChatRoleSelect != null)
		{
			this.OnChatRoleSelect.Invoke(this.m_currentSelectItem);
		}
		this.UpdatePrvateTip();
	}

	private void ShowRoleChatcontents(ChatBoxPrivateBtnItem item)
	{
		List<ChatTextItem> list = new List<ChatTextItem>();
		for (int i = 0; i < GChat.PrivateChatTextList.Count; i++)
		{
			string empty = string.Empty;
			this.GetRoleInfo(GChat.PrivateChatTextList[i], ref empty);
			if (empty == item.RoleInfo.RoleName)
			{
				if (GChat.PrivateChatTextList[i].IsRead != 1)
				{
					GChat.PrivateChatTextList[i].IsRead = 1;
				}
				list.Add(GChat.PrivateChatTextList[i]);
			}
		}
		item.RoleInfo.UnReadMessageNum = 0;
		item.RefershTipNum();
		ChatBoxPrivateController.ShowChatContent(this.tableChat, this.ShownVoiceItemExList, list);
		this.m_tableNeedToUpdate = true;
	}

	public void SetTalkRole(string roleName = "")
	{
		this.m_lstRoleInfos.Clear();
		PrivateRoleInfo privateRoleInfo = null;
		string text = string.Empty;
		if (roleName != string.Empty)
		{
			privateRoleInfo = new PrivateRoleInfo();
			privateRoleInfo.RoleName = roleName;
			privateRoleInfo.UnReadMessageNum = 0;
			this.m_lstRoleInfos.Add(privateRoleInfo);
			this.m_currentSelectItem = null;
		}
		else if (this.m_currentSelectItem != null)
		{
			text = this.m_currentSelectItem.RoleInfo.RoleName;
		}
		List<PrivateRoleInfo> list = this.ReLoadChatRoleInfo(privateRoleInfo);
		this.m_lstRoleInfos.AddRange(list);
		this.ResetBtns();
		if (this.m_lstRoleInfos.Count <= 0)
		{
			this.OnSelectBtnItem(null);
		}
		else
		{
			ChatBoxPrivateBtnItem item = this.m_lstRoleItem[0];
			if (this.m_currentSelectItem != null)
			{
				for (int i = 0; i < this.m_lstRoleInfos.Count; i++)
				{
					if (this.m_lstRoleItem[i].RoleInfo.RoleName == text)
					{
						item = this.m_lstRoleItem[i];
						break;
					}
				}
			}
			this.m_currentSelectItem = null;
			this.OnSelectBtnItem(item);
		}
	}

	private List<PrivateRoleInfo> ReLoadChatRoleInfo(PrivateRoleInfo first = null)
	{
		List<PrivateRoleInfo> list = new List<PrivateRoleInfo>();
		for (int i = GChat.PrivateChatTextList.Count - 1; i >= 0; i--)
		{
			ChatTextItem item = GChat.PrivateChatTextList[i];
			string empty = string.Empty;
			this.GetRoleInfo(item, ref empty);
			if (first == null || !(empty == first.RoleName))
			{
				this.UpdateRoleList(ref list, item);
			}
		}
		this.SortRoleInfo(list);
		return list;
	}

	private void UpdateRoleList(ref List<PrivateRoleInfo> lstRoles, ChatTextItem item)
	{
		string empty = string.Empty;
		this.GetRoleInfo(item, ref empty);
		PrivateRoleInfo privateRoleInfo = this.GetRoleInfoByNameAndZone(empty, lstRoles);
		if (privateRoleInfo == null)
		{
			privateRoleInfo = new PrivateRoleInfo();
			privateRoleInfo.LastChatTicks = item.Ticks;
			privateRoleInfo.RoleName = empty;
			privateRoleInfo.UnReadMessageNum = 0;
			lstRoles.Add(privateRoleInfo);
		}
		if (item.FromRoleID != Global.Data.roleData.RoleID && item.IsRead != 1)
		{
			privateRoleInfo.UnReadMessageNum++;
		}
	}

	private void GetRoleInfo(ChatTextItem item, ref string roleName)
	{
		if (item.FromRoleID == Global.Data.roleData.RoleID)
		{
			roleName = item.ToRoleName;
		}
		else
		{
			roleName = item.FromRoleName;
		}
	}

	private void SortRoleInfo(List<PrivateRoleInfo> lstRoles)
	{
		lstRoles.Sort(delegate(PrivateRoleInfo role1, PrivateRoleInfo role2)
		{
			if ((role1.UnReadMessageNum > 0 && role2.UnReadMessageNum > 0) || (role1.UnReadMessageNum <= 0 && role2.UnReadMessageNum <= 0))
			{
				if (role1.LastChatTicks > role2.LastChatTicks)
				{
					return -1;
				}
				if (role1.LastChatTicks < role2.LastChatTicks)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (role1.UnReadMessageNum > 0 && role2.UnReadMessageNum <= 0)
				{
					return -1;
				}
				if (role1.UnReadMessageNum <= 0 && role2.UnReadMessageNum > 0)
				{
					return 1;
				}
				return 0;
			}
		});
	}

	private PrivateRoleInfo GetRoleInfoByNameAndZone(string name, List<PrivateRoleInfo> lstRoles)
	{
		return lstRoles.Find((PrivateRoleInfo info) => info.RoleName == name);
	}

	public void UpdateNewMessage()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.m_currentSelectItem == null)
		{
			this.SetTalkRole(string.Empty);
			return;
		}
		ChatTextItem chatTextItem = GChat.PrivateChatTextList[GChat.PrivateChatTextList.Count - 1];
		string empty = string.Empty;
		this.GetRoleInfo(chatTextItem, ref empty);
		if (empty == this.m_currentSelectItem.RoleInfo.RoleName)
		{
			chatTextItem.IsRead = 1;
			this.ShowRoleChatcontents(this.m_currentSelectItem);
		}
		else
		{
			string roleName = this.m_currentSelectItem.RoleInfo.RoleName;
			bool flag = false;
			for (int i = 0; i < this.m_lstRoleInfos.Count; i++)
			{
				if (this.m_lstRoleInfos[i].RoleName == empty)
				{
					this.m_lstRoleInfos[i].UnReadMessageNum++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				PrivateRoleInfo privateRoleInfo = new PrivateRoleInfo();
				privateRoleInfo.RoleName = empty;
				privateRoleInfo.UnReadMessageNum = 1;
				this.m_lstRoleInfos.Add(privateRoleInfo);
			}
			this.SortRoleInfo(this.m_lstRoleInfos);
			this.ResetBtns();
			for (int j = 0; j < this.m_lstRoleInfos.Count; j++)
			{
				if (this.m_lstRoleItem[j].RoleInfo.RoleName == roleName)
				{
					this.m_currentSelectItem = this.m_lstRoleItem[j];
					this.m_currentSelectItem.SetSelectState(true);
					break;
				}
			}
		}
	}

	public void UpdatePrvateTip()
	{
		int num = 0;
		for (int i = 0; i < GChat.PrivateChatTextList.Count; i++)
		{
			ChatTextItem chatTextItem = GChat.PrivateChatTextList[i];
			if (chatTextItem.FromRoleID != Global.Data.roleData.RoleID)
			{
				if (GChat.PrivateChatTextList[i].IsRead != 1)
				{
					num++;
				}
			}
		}
		ActivityTipManager.SetActivityTipItemActive(90001, num > 0);
	}

	private void LateUpdate()
	{
		if (this.m_tableNeedToUpdate)
		{
			this.m_lateUpdateCount++;
			if (this.m_lateUpdateCount >= 2)
			{
				this.RefreshTable();
				this.m_tableNeedToUpdate = false;
				this.m_lateUpdateCount = 0;
			}
		}
	}

	public void RefreshTable()
	{
		this.tableChat.UpdataNow();
		this.uiCollider.UpdataCollider();
		if (this.uiCollider.box != null)
		{
			Vector3 center = this.uiCollider.box.center;
			this.uiCollider.box.center = new Vector3(190f, center.y, -0.1f);
			Vector3 size = this.uiCollider.box.size;
			this.uiCollider.box.size = new Vector3(380f, size.y, size.z);
		}
		this.textDragPanel.verticalScrollBar.scrollValue = 1f;
	}

	public string GetCurrentRoleName()
	{
		string result = string.Empty;
		if (this.m_currentSelectItem != null)
		{
			result = this.m_currentSelectItem.RoleInfo.RoleName;
		}
		return result;
	}

	public static void ShowChatContent(UITable TextList, List<ChatBoxVoiceItemEx> ShownVoiceItemExList, List<ChatTextItem> chatTextItemList)
	{
		if (chatTextItemList == null)
		{
			TextList.Clear();
			return;
		}
		GChat.CheckAndHandleMessage();
		List<ChatTextItem> list = new List<ChatTextItem>();
		List<GameObject> list2 = new List<GameObject>();
		List<ChatTextItem> list3 = new List<ChatTextItem>();
		List<ChatBoxTextItemEx> list4 = new List<ChatBoxTextItemEx>();
		if (ShownVoiceItemExList == null)
		{
			ShownVoiceItemExList = new List<ChatBoxVoiceItemEx>();
		}
		ShownVoiceItemExList.Clear();
		List<ChatTextItem> list5 = new List<ChatTextItem>();
		List<Transform> children = TextList.children;
		int count = children.Count;
		for (int i = 0; i < count; i++)
		{
			if (!(children[i] == null))
			{
				ChatBoxTextItemEx component = children[i].GetComponent<ChatBoxTextItemEx>();
				if (component != null)
				{
					if (chatTextItemList.Contains(component.ChatItem))
					{
						list3.Add(component.ChatItem);
					}
					else
					{
						list2.Add(component.gameObject);
					}
					list4.Add(component);
					list5.Add(component.ChatItem);
				}
				else
				{
					ChatBoxVoiceItemEx component2 = children[i].GetComponent<ChatBoxVoiceItemEx>();
					if (component2 != null)
					{
						if (chatTextItemList.Contains(component2.ChatItem))
						{
							list3.Add(component2.ChatItem);
						}
						else
						{
							list2.Add(component2.gameObject);
						}
						ShownVoiceItemExList.Add(component2);
						list5.Add(component2.ChatItem);
					}
				}
			}
		}
		count = chatTextItemList.Count;
		for (int j = 0; j < count; j++)
		{
			ChatTextItem chatTextItem = chatTextItemList[j];
			if (!list5.Contains(chatTextItem))
			{
				ChatBoxPrivateController.HandleTextOrVoice(TextList, chatTextItemList[j], j, false, null, null, null);
			}
			else
			{
				int num = ChatBoxPrivateController.FindTextItem(list4, chatTextItem);
				if (num != -1)
				{
					ChatBoxPrivateController.HandleTextOrVoice(TextList, chatTextItemList[j], j, true, list4[num], null, null);
				}
				else
				{
					num = ChatBoxPrivateController.FindVoiceItem(ShownVoiceItemExList, chatTextItem);
					if (num != -1)
					{
						ChatBoxPrivateController.HandleTextOrVoice(TextList, chatTextItemList[j], j, true, null, ShownVoiceItemExList[num], null);
					}
				}
			}
		}
		count = list2.Count;
		for (int k = count - 1; k >= 0; k--)
		{
			NGUITools.Destroy(list2[k]);
		}
		list.Clear();
		list2.Clear();
		list3.Clear();
		list4.Clear();
	}

	public static int FindVoiceItem(List<ChatBoxVoiceItemEx> itemList, ChatTextItem textItem)
	{
		int result = -1;
		int count = itemList.Count;
		for (int i = 0; i < count; i++)
		{
			if (itemList[i].ChatItem == textItem)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public static int FindTextItem(List<ChatBoxTextItemEx> itemList, ChatTextItem textItem)
	{
		int result = -1;
		int count = itemList.Count;
		for (int i = 0; i < count; i++)
		{
			if (itemList[i].ChatItem.ReceiveID == textItem.ReceiveID)
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public static void HandleTextOrVoice(UITable TextList, ChatTextItem chatItem, int index, bool bExist = false, ChatBoxTextItemEx textItemTemplet = null, ChatBoxVoiceItemEx voiceItem = null, DPSelectedItemEventHandler hander = null)
	{
		string text = ChatBoxPrivateController.NamePrefix[index / 10] + index;
		if (chatItem.ChatType == ChatType.TextOrSymbol)
		{
			if (bExist && textItemTemplet != null)
			{
				textItemTemplet.ChatChannelIndexes = (ChatChannelIndexes)ChatBoxPrivateController.ChatChannelIndexes;
				textItemTemplet.ShowFace = true;
				textItemTemplet.hander = hander;
				textItemTemplet.ChatItem = chatItem;
				textItemTemplet.gameObject.name = text + "ChatBoxTextItemEx";
			}
			else
			{
				textItemTemplet = U3DUtils.NEW<ChatBoxTextItemEx>();
				textItemTemplet.ChatChannelIndexes = (ChatChannelIndexes)ChatBoxPrivateController.ChatChannelIndexes;
				textItemTemplet.hander = hander;
				textItemTemplet.ShowFace = true;
				textItemTemplet.ChatItem = chatItem;
				textItemTemplet.UserText = GChat.FormatChatUserText(chatItem, false, false);
				textItemTemplet.ContentText = GChat.FormatChatContentText(chatItem, textItemTemplet.NGUIHTMLContent.maxLineWidth, true);
				textItemTemplet.gameObject.name = text + "ChatBoxTextItemEx";
				U3DUtils.AddChild(TextList.gameObject, textItemTemplet.gameObject, true);
			}
			if (null != textItemTemplet)
			{
				textItemTemplet.Refresh();
			}
		}
		else if (chatItem.ChatType == ChatType.Voice)
		{
			string textMsg = chatItem.TextMsg;
			string[] array = textMsg.Split(new char[]
			{
				'@'
			});
			string[] array2 = array[0].Split(new char[]
			{
				'#'
			});
			string text2 = array2[0];
			if (array2.Length >= 2)
			{
				chatItem.ClipLength = float.Parse(array2[1]);
			}
			string text3 = string.Empty;
			if (array.Length >= 2)
			{
				text3 = array[1];
			}
			chatItem.TextMsg = text2 + "@" + text3;
			if (!bExist || voiceItem == null)
			{
				voiceItem = U3DUtils.NEW<ChatBoxVoiceItemEx>("ChatBoxVoiceItemEx");
				voiceItem.Text = GChat.FormatChatVoiceText(chatItem, false, false);
				if (chatItem.FromRoleID == Global.Data.RoleID)
				{
					chatItem.IsRead = 1;
					voiceItem.SelfVoice = true;
				}
				else
				{
					voiceItem.SelfVoice = false;
				}
				voiceItem.voiceFileId = text2;
				voiceItem.gameObject.name = text + "ChatBoxVoiceItemEx";
				U3DUtils.AddChild(TextList.gameObject, voiceItem.gameObject, true);
			}
			voiceItem.ChatItem = chatItem;
			if (bExist)
			{
				voiceItem.CurVoiceItemState = VoiceItemState.EMPTY;
				voiceItem.gameObject.name = text + "ChatBoxVoiceItemEx";
			}
			else if (chatItem.EncodeVoiceBytes != null && chatItem.EncodeVoiceBytes.Length > 0)
			{
				voiceItem.SetData(chatItem.EncodeVoiceBytes);
			}
			else
			{
				voiceItem.CurVoiceItemState = VoiceItemState.EMPTY;
			}
			voiceItem.SetRedPoint(chatItem.IsRead);
			voiceItem.VioceToWordText = text3;
			voiceItem.ErrorLabel.text = string.Empty;
			if (chatItem.TextMsg.StartsWith("SendError"))
			{
				voiceItem.ErrorLabel.text = Global.GetLang("发送失败");
				chatItem.IsRead = 1;
				voiceItem.SendVoiceSuc = false;
				if (voiceItem.IsSelfInfo)
				{
					voiceItem.FailSprite.gameObject.SetActive(true);
				}
				else
				{
					voiceItem.ErrorLabel.gameObject.SetActive(false);
					voiceItem.FailSprite.gameObject.SetActive(false);
				}
			}
			if (chatItem.TextMsg.StartsWith("SendErrorNotOnLine"))
			{
				voiceItem.ErrorLabel.text = Global.GetLang("对方不在线");
				chatItem.IsRead = 1;
				voiceItem.SendVoiceSuc = false;
				if (voiceItem.IsSelfInfo)
				{
					voiceItem.ErrorLabel.gameObject.SetActive(true);
					voiceItem.FailSprite.gameObject.SetActive(true);
				}
				else
				{
					voiceItem.ErrorLabel.gameObject.SetActive(false);
					voiceItem.FailSprite.gameObject.SetActive(false);
				}
			}
			if (chatItem.TextMsg.StartsWith("SendErrorNoUser"))
			{
				voiceItem.ErrorLabel.text = Global.GetLang("对方不存在");
				chatItem.IsRead = 1;
				voiceItem.SendVoiceSuc = false;
				if (voiceItem.IsSelfInfo)
				{
					voiceItem.ErrorLabel.gameObject.SetActive(true);
					voiceItem.FailSprite.gameObject.SetActive(true);
				}
				else
				{
					voiceItem.ErrorLabel.gameObject.SetActive(false);
					voiceItem.FailSprite.gameObject.SetActive(false);
				}
			}
		}
	}

	private const int ItemHeight = 42;

	public GameObject privateBtnContainer;

	public UILabel lblUserName;

	public UIDraggablePanel _UIDraggablePanel;

	public UIDraggablePanel textDragPanel;

	public UITable tableChat;

	public GButton btnClose;

	private bool m_tableNeedToUpdate;

	private int m_lateUpdateCount;

	private List<PrivateRoleInfo> m_lstRoleInfos = new List<PrivateRoleInfo>();

	private List<ChatBoxPrivateBtnItem> m_lstRoleItem = new List<ChatBoxPrivateBtnItem>();

	private ChatBoxPrivateBtnItem m_currentSelectItem;

	private UICollider uiCollider;

	private List<ChatBoxVoiceItemEx> ShownVoiceItemExList = new List<ChatBoxVoiceItemEx>();

	public Action<ChatBoxPrivateBtnItem> OnChatRoleSelect;

	public static int ChatChannelIndexes = 0;

	private static string[] NamePrefix = new string[]
	{
		"a",
		"b",
		"c",
		"d",
		"e",
		"f",
		"g",
		"h",
		"i",
		"j"
	};
}
