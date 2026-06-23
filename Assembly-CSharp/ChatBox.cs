using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChatBox : UserControl
{
	private float VoiceChangeIntro
	{
		get
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("VoiceChangeIntro", true);
			if (string.IsNullOrEmpty(systemParamByName))
			{
				this._VoiceChangeIntro = 4f;
			}
			else
			{
				this._VoiceChangeIntro = (float)Global.SafeConvertToInt32(systemParamByName);
			}
			return this._VoiceChangeIntro + this._VoiceChangeIntroDelta;
		}
	}

	public int ChatChannelIndex
	{
		get
		{
			return this._ChatChannelIndex;
		}
		private set
		{
			this._ChatChannelIndex = value;
		}
	}

	[HideInInspector]
	public int SendChatChannelID
	{
		get
		{
			return this.mSendChatChannelID;
		}
		set
		{
			this.mSendChatChannelID = value;
			this.RefreshUIState();
		}
	}

	public MUVoiceManager MUVoice
	{
		get
		{
			return MUVoiceManager.GetInstance();
		}
	}

	private void InitTextInPrefabs()
	{
		this.mOBC = this.TabBtnListBox.ItemsSource;
		this.TabBtnListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TabBtnListBoxSelectionChanged);
		this.m_chatTypeIndexDic.Clear();
		List<GameObject> list = new List<GameObject>();
		for (byte b = 0; b < 12; b += 1)
		{
			GameObject gameObject = SpawnManager.Instantiate(this.BtnItem) as GameObject;
			list.Add(gameObject);
			gameObject.name = b.ToString();
			BtnHander btnHander = new BtnHander(gameObject);
			btnHander.ChatChannelIndexes = (ChatChannelIndexes)b;
			this.mChatTypeLst.Add(btnHander);
			UIDragPanelContents uidragPanelContents = gameObject.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = gameObject.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = this.DranggabelPanel;
		}
		int num = 0;
		this.mOBC.Add(list[0]);
		this.m_chatTypeIndexDic[0] = num;
		num++;
		this.mOBC.Add(list[9]);
		this.m_chatTypeIndexDic[9] = num;
		num++;
		for (int i = 1; i < 12; i++)
		{
			if (i != 9)
			{
				this.m_chatTypeIndexDic[i] = num;
				num++;
				this.mOBC.Add(list[i]);
			}
		}
		list.Clear();
		this.SendBtn.Text = Global.GetLang("发送");
		this.ChatTextBox.label.text = Global.GetLang("请输入聊天信息");
		this.ChatTextBox.defaultText = Global.GetLang("请输入聊天信息");
		this.SendVoiceSP.spriteName = "Speek0";
		this.btnChannel.Text = Global.GetLang("频道");
		this.btnPrivate.Text = Global.GetLang("私聊");
		this.btnChannel.Label.lineWidth = 40;
		this.btnChannel.Label.pivot = 4;
		this.btnChannel.Label.transform.localPosition = new Vector3(0f, 0f, -1f);
		this.btnPrivate.Label.lineWidth = 40;
		this.btnPrivate.Label.pivot = 4;
		this.btnPrivate.Label.transform.localPosition = new Vector3(0f, 0f, -1f);
		if (this.staticText.Length == 2)
		{
			if (this.staticText[0] != null)
			{
				this.staticText[0].text = Global.GetLang("松开手指取消发送");
			}
			if (this.staticText[1] != null)
			{
				this.staticText[1].text = Global.GetLang("手指上划取消发送");
			}
		}
		this.m_BtnFriend.transform.localPosition = new Vector3(-200f, 0f, 0f);
		this.m_BtnWuPin.transform.localPosition = new Vector3(-120f, 0f, 0f);
		this.VoiceOrWordBtn.gameObject.SetActive(false);
		this.SendVoiceBtn.gameObject.SetActive(false);
	}

	private ChatChannelIndexes GetSelectIndex()
	{
		GameObject selectedItem = this.TabBtnListBox.SelectedItem;
		if (null != selectedItem)
		{
			return (ChatChannelIndexes)selectedItem.name.SafeToInt32(0);
		}
		return ChatChannelIndexes.All;
	}

	private void TabBtnListBoxSelectionChanged(object sender, MouseEvent e)
	{
		if (0 <= this.TabBtnListBox.SelectedIndex)
		{
			int selectIndex = (int)this.GetSelectIndex();
			if (selectIndex < this.mChatTypeLst.Count)
			{
				BtnHander btnHander = this.mChatTypeLst[selectIndex];
				if (btnHander != null)
				{
					if (this.mLastselectBtnHander != null)
					{
						this.mLastselectBtnHander.Bselect = false;
						if (this.mLastselectBtnHander != btnHander)
						{
							this.mLastselectBtnHander = btnHander;
						}
					}
					else
					{
						this.mLastselectBtnHander = btnHander;
					}
					btnHander.Bselect = true;
					if (Global.InKuafuHuodongSingle())
					{
						this.mChatTypeLst[6].TextColor = NGUIMath.HexToColorEx(7697781U);
						this.SetChannelIndex(7);
					}
					else if (Global.IsInKuafuHuodongZhenYing())
					{
						this.mChatTypeLst[6].TextColor = NGUIMath.HexToColorEx(7697781U);
						this.SetChannelIndex(6);
					}
					else if (Global.InKuafuFuben())
					{
						this.mChatTypeLst[6].TextColor = NGUIMath.HexToColorEx(7697781U);
						this.SetChannelIndex(7);
					}
					else
					{
						this.mChatTypeLst[this.ChatChannelIndex].TextColor = NGUIMath.HexToColorEx(7697781U);
						this.mChatTypeLst[selectIndex].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(btnHander.Text)]);
						this.SetChannelIndex(selectIndex);
					}
					this.RefreshChat(7);
					this.danMuButton.SetDanMuType((ChatChannelIndexes)this.ChatChannelIndex);
				}
			}
		}
	}

	private int GetColorIndex(string str)
	{
		if (str == this.strType[0])
		{
			return 0;
		}
		if (str == this.strType[8])
		{
			return 7;
		}
		if (str == this.strType[2])
		{
			return 2;
		}
		if (str == this.strType[7])
		{
			return 6;
		}
		if (str == this.strType[4])
		{
			return 4;
		}
		if (str == this.strType[5])
		{
			return 5;
		}
		if (str == this.strType[3])
		{
			return 3;
		}
		if (str == this.strType[1])
		{
			return 1;
		}
		if (str == this.strType[6])
		{
			return 6;
		}
		if (str == this.strType[9])
		{
			return 2;
		}
		if (!Global.isHaiWai && str == this.strType[10])
		{
			return 0;
		}
		return 0;
	}

	private void RefreshLastChatChannel(ChatChannelIndexes index, ChatChannelIndexes ChatIndex)
	{
		ChatChannelIndexes chatChannelIndexes = ChatIndex;
		if (0 <= ChatBox.LastChatChannel && ChatBox.LastChatChannel < this.mChatTypeLst.Count)
		{
			if (this.mChatTypeLst[ChatBox.LastChatChannel].BActive)
			{
				chatChannelIndexes = (ChatChannelIndexes)ChatBox.LastChatChannel;
			}
			this.SetSendMode((int)chatChannelIndexes);
			this.SetChannelIndex((int)chatChannelIndexes);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[(int)chatChannelIndexes];
		}
		else
		{
			this.mChatTypeLst[0].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[0])]);
			this.mChatTypeLst[0].Bselect = true;
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
			for (int i = 1; i < 12; i++)
			{
				if (i != 4)
				{
					if (i < this.mChatTypeLst.Count && this.mChatTypeLst[i].BActive)
					{
						chatChannelIndexes = (ChatChannelIndexes)i;
						break;
					}
				}
			}
			this.SetSendMode((int)chatChannelIndexes);
			this.SetChannelIndex((int)index);
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.privateController.OnChatRoleSelect = new Action<ChatBoxPrivateBtnItem>(this.OnChatRoleSelect);
		ActivityTipManager.RegActivityTipItem(90001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		this.privateController.UpdatePrvateTip();
		this.ProssLength = 1f / this.MUVoice.RecordVoiceMaxLength_sec;
		this.LuYin.SetActive(false);
		this.CancelYuYin.SetActive(false);
		this.Slider.transform.parent.gameObject.SetActive(false);
		this.mChatTypeLst[6].BActive = false;
		this.mChatTypeLst[7].BActive = false;
		if (Global.g_bIsYaoQingCeShi)
		{
		}
		this.uiCollider = this.TextList.gameObject.GetComponent<UICollider>();
		this.TextListPanelTrans = this.TextListPanel.transform;
		this.TextDragPanel = this.TextListPanel.gameObject.GetComponent<UIDraggablePanel>();
		this.SpeakerInstance.TimeOutDelegateInstance = delegate()
		{
			this.CanSendVioce = true;
			this.VoiceTimeLength = 0;
			base.CancelInvoke("Moveing");
			base.CancelInvoke("VoiceSimple");
			this.ResetMusicAndAudio();
			this.SendVoiceSP.spriteName = "Speek0";
			this.MicrophoneContainer.SetActive(false);
			this.ProgessT.spriteName = "luyin_bar_green";
			this.Slider.sliderValue = 0f;
			this.SpeakerInstance.EndRecord();
			List<byte> encodeBytes = this.SpeakerInstance.EncodeBytes;
			byte[] array = this.SpeakerInstance.ReceiveAudioByte(encodeBytes.ToArray());
			string voice = Convert.ToBase64String(array);
			GChat.IsInLuYinAndSending = true;
			GChat.IsInvokeSendOver = false;
			base.Invoke("SendVioceToWordMesg", this.VoiceChangeIntro);
			QMQJJava.RecordToWord(voice);
		};
		this.SpeakerInstance.TimeEachSecondInstance = delegate(int passTime)
		{
			this.VoiceTimeLength = passTime;
			this.refreshProgress();
		};
		this.mChatTypeLst[9].BActive = false;
		this.mChatTypeLst[4].BActive = false;
		this.mChatTypeLst[10].BActive = false;
		this.mChatTypeLst[11].BActive = false;
		this.btnPrivate.gameObject.SetActive(true);
		this.ChatTextBox.encoding = true;
		if (Global.InKuafuHuodongSingle())
		{
			this.mChatTypeLst[7].Bselect = true;
			this.mChatTypeLst[7].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[7])]);
			this.SetSendMode(7);
			this.SetChannelIndex(7);
			this.mChatTypeLst[0].BActive = false;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = true;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[7];
		}
		else if (Global.IsInKuafuHuodongZhenYing())
		{
			this.mChatTypeLst[6].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[6])]);
			this.mChatTypeLst[6].Bselect = true;
			this.SetSendMode(6);
			this.SetChannelIndex(6);
			this.mChatTypeLst[0].BActive = false;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = true;
			this.mChatTypeLst[7].BActive = false;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[6];
		}
		else if (Global.InKuafuFuben())
		{
			this.mChatTypeLst[7].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[7])]);
			this.mChatTypeLst[7].Bselect = true;
			this.SetSendMode(7);
			this.SetChannelIndex(7);
			this.mChatTypeLst[0].BActive = false;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = true;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[7];
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap)
		{
			this.mChatTypeLst[0].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[0])]);
			this.mChatTypeLst[0].Bselect = true;
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = true;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = true;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(true);
			this.mChatTypeLst[5].BActive = true;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
			this.RefreshLastChatChannel(ChatChannelIndexes.All, ChatChannelIndexes.World);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
		{
			this.mChatTypeLst[0].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[0])]);
			this.mChatTypeLst[0].Bselect = true;
			this.SetSendMode(0);
			this.SetChannelIndex(0);
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = true;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || Global.GetMapSceneUIClass() == SceneUIClasses.ZhengDuoZhiDi)
		{
			this.mChatTypeLst[2].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[2])]);
			this.mChatTypeLst[2].Bselect = true;
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[2];
			this.SetSendMode(2);
			this.SetChannelIndex(2);
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai)
		{
			this.mChatTypeLst[0].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[0])]);
			this.mChatTypeLst[0].Bselect = true;
			this.SetSendMode(2);
			this.SetChannelIndex(2);
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = true;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = false;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
		{
			this.mChatTypeLst[0].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[0])]);
			this.mChatTypeLst[0].Bselect = true;
			this.SetSendMode(0);
			this.SetChannelIndex(0);
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = true;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = false;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattleMiDong)
		{
			this.mChatTypeLst[9].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[9])]);
			this.mChatTypeLst[9].Bselect = true;
			this.SetSendMode(9);
			this.SetChannelIndex(9);
			this.mChatTypeLst[0].BActive = false;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = false;
			this.mChatTypeLst[9].BActive = true;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[9];
		}
		else if (SceneUIClasses.RebornMap.IsTheScene())
		{
			this.mChatTypeLst[10].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[10])]);
			this.SetSendMode(0);
			this.SetChannelIndex(0);
			this.mChatTypeLst[10].BActive = !Global.isHaiWai;
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = true;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(true);
			this.mChatTypeLst[5].BActive = true;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = false;
			this.mChatTypeLst[9].BActive = false;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(true);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[0];
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.DaTaoShaPrepare || Global.GetMapSceneUIClass() == SceneUIClasses.DaTaoSha || Global.GetMapSceneUIClass() == SceneUIClasses.MoYuDuoBao)
		{
			this.mChatTypeLst[11].TextColor = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[11])]);
			this.SetSendMode(11);
			this.SetChannelIndex(11);
			this.mChatTypeLst[10].BActive = false;
			this.mChatTypeLst[0].BActive = true;
			this.mChatTypeLst[1].BActive = false;
			this.mChatTypeLst[2].BActive = false;
			this.mChatTypeLst[3].BActive = false;
			this.mChatTypeLst[4].BActive = false;
			this.btnPrivate.gameObject.SetActive(false);
			this.mChatTypeLst[5].BActive = false;
			this.mChatTypeLst[6].BActive = false;
			this.mChatTypeLst[7].BActive = false;
			this.mChatTypeLst[8].BActive = false;
			this.mChatTypeLst[9].BActive = false;
			this.mChatTypeLst[11].BActive = true;
			this.m_BtnFriend.gameObject.SetActive(false);
			this.m_BtnWuPin.gameObject.SetActive(false);
			this.VoiceContainer.gameObject.SetActive(false);
			this.VoiceOrWordBtn.gameObject.SetActive(false);
			this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[11];
		}
		else
		{
			this.RefreshLastChatChannel(ChatChannelIndexes.All, ChatChannelIndexes.World);
		}
		if (Global.RoleHaveArmyGroup())
		{
			if (Global.IsInKuafuHuodongZhenYing() || Global.InKuafuHuodongSingle() || Global.InKuafuFuben() || Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle || Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattleMiDong || Global.GetMapSceneUIClass() == SceneUIClasses.MoYuDuoBao || Global.GetMapSceneUIClass() == SceneUIClasses.DaTaoShaPrepare || Global.GetMapSceneUIClass() == SceneUIClasses.DaTaoSha)
			{
				this.mChatTypeLst[8].BActive = false;
			}
			else
			{
				this.mChatTypeLst[8].BActive = true;
			}
		}
		else
		{
			this.mChatTypeLst[8].BActive = false;
		}
		if (Global.RoleHaveComp() && Global.IsInShiLiZhengBaMap())
		{
			this.mChatTypeLst[9].BActive = true;
		}
		this.TabBtnListBox.repositionNow = true;
		this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
		this.RefreshUIState();
		this.btnChannel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectType(ChatSelectType.Channel);
		};
		this.btnPrivate.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenPrivateChat(string.Empty);
		};
		UIEventListener.Get(this.Bak.gameObject).onClick = delegate(GameObject s)
		{
			if (null != this.menuPart)
			{
				this.menuPart.Visibility = false;
			}
		};
		UIEventListener.Get(this.CloseBtn).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this._ChatWinCloseBtn).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.ChannelIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		this.SendBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendMessage();
		};
		if (this.ChatSymbolBtn != null)
		{
			UIEventListener.Get(this.ChatSymbolBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.SendChatChannelID == 5)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("当前频道不能发言"), new object[0]), 10, 3);
					return;
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			};
		}
		if (null != this.m_BtnFriend)
		{
			UIEventListener.Get(this.m_BtnFriend.gameObject).onClick = delegate(GameObject s)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			};
		}
		if (null != this.m_BtnWuPin)
		{
			UIEventListener.Get(this.m_BtnWuPin.gameObject).onClick = delegate(GameObject s)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3
				});
				if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
				{
					this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
					this.RefreshUIState();
				}
			};
		}
		if (this.SendVoiceBtn != null)
		{
			UIEventListener.Get(this.SendVoiceBtn.gameObject).onDrag = delegate(GameObject gameObject, Vector2 delta)
			{
				if (!this.MUVoice.CanSendVoice(true))
				{
					return;
				}
				this.LogOnScreen("UICamera.currentTouch.totalDelta.y == " + UICamera.currentTouch.totalDelta.y);
				if (UICamera.currentTouch.totalDelta.y >= this.DragLength)
				{
					this.isDragSend = false;
					this.HideVoiceMark();
				}
				else
				{
					this.isDragSend = true;
					this.ShowVoiceMark();
				}
			};
			this.SendVoiceBtn.OnPress = delegate(GameObject s, bool state)
			{
				if (Global.CanGuanZhan())
				{
					return;
				}
				if (!this.MUVoice.IsMuVoiceOpen("VoiceOpen"))
				{
					Super.HintMainText(Global.GetLang("语音暂未开放"), 10, 3);
					return;
				}
				if (CheckSFAndWorker.checkNet() && !this.MUVoice.IsSimulatorOpenVoice())
				{
					Super.HintMainText(Global.GetLang("模拟器环境不能使用语音"), 10, 3);
					return;
				}
				if (this.MUVoice.CanSendVoice(true))
				{
					if (!this.MUVoice.MicEnable())
					{
						Super.HintMainText(Global.GetLang("麦克风未开启"), 10, 3);
						return;
					}
					if (this.ChatChannelIndex == 2)
					{
						if (Global.Data.roleData.Faction == 0 && Global.Data.roleData.BHName == string.Empty)
						{
							if (!state)
							{
								this.SetVioce();
							}
							else
							{
								Super.HintMainText(Global.GetLang("加入帮会才可以使用帮会语音"), 10, 3);
							}
							return;
						}
					}
					else if (this.ChatChannelIndex == 3)
					{
						if (Global.Data.roleData.TeamID <= 0)
						{
							if (!state)
							{
								this.SetVioce();
							}
							else
							{
								Super.HintMainText(Global.GetLang("组建队伍后，才能给队友发送消息..."), 10, 3);
							}
							return;
						}
					}
					else if (this.ChatChannelIndex == 5)
					{
						Super.HintMainText(Global.GetLang("系统频道不能发送语音..."), 10, 3);
						return;
					}
					if (state)
					{
						if (Time.time - this.witeTime < 3f)
						{
							this.witeTimeIs = false;
							return;
						}
						this.CanSendVioce = false;
						this.LogOnScreen(string.Concat(new object[]
						{
							Global.GetLang("刚按下  "),
							this.CanSendVioce,
							"    ",
							GChat.IsInvokeSendOver,
							"  ",
							GChat.IsInvokeSendOver
						}));
						GChat.ILuYinSecondCout = 0;
						this.isLuYinNow = true;
						this.witeTimeIs = true;
						this.witeTime = Time.time;
						this.isDragSend = true;
						this.ShowVoiceMark();
						this.VoiceTimeLength = 0;
						this.ProgessT.spriteName = "luyin_bar_green";
						this.SetMusicAndAudio();
						this.SendVoiceSP.spriteName = "Speek1";
						this.MicrophoneContainer.SetActive(true);
						this.MUVoice.ChangeMode = 2;
						this.MUVoice.ChatChanel = this.GetChannelType(this.SendChatChannelID);
						if (!this.MUVoice.StartRecord())
						{
							return;
						}
						this.MUVoice.SpeechToSomeoneName = this.ChatTextBox.text;
						this.Slider.sliderValue = 0f;
						base.InvokeRepeating("Moveing", 0f, 1f);
						base.InvokeRepeating("VoiceSimple", 0f, 0.03f);
					}
					else
					{
						this.ResetMusicAndAudio();
						this.SendVoiceSP.spriteName = "Speek0";
						this.MicrophoneContainer.SetActive(false);
						if (!this.witeTimeIs)
						{
							return;
						}
						if (!this.isDragSend)
						{
							this.MUVoice.IsRecording = false;
							this.MUVoice.CancelRecord();
						}
						else
						{
							this.MUVoice.StopAndUploadRecord();
						}
						if (this.MUVoice.IsInRoom)
						{
							this.MUVoice.ChangeMode = 0;
						}
						this.VoiceTimeLength = 0;
						base.CancelInvoke("Moveing");
						base.CancelInvoke("VoiceSimple");
					}
				}
			};
		}
		if (this.VoiceOrWordBtn != null)
		{
			UIEventListener.Get(this.VoiceOrWordBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.SendChatChannelID == 5)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("当前频道不能发言"), new object[0]), 10, 3);
					return;
				}
				if (this.MUVoice.IsRecording)
				{
					Super.HintMainText(Global.GetLang("正在录音，不能切换到文字状态"), 10, 3);
					return;
				}
				if (this.SendChatChannelID == 4 && this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.Word)
				{
					string currentRoleName = this.privateController.GetCurrentRoleName();
					if (!(string.Empty != currentRoleName))
					{
						Super.HintMainText(Global.GetLang("请选择私聊对象"), 10, 3);
						return;
					}
					this.ChatTextBox.text = string.Format("/{0} ", currentRoleName);
				}
				switch (this.CurVoiceOrWordState)
				{
				case ChatBox.VoiceOrWordState.VoiceNormal:
					this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
					this.RefreshUIState();
					break;
				case ChatBox.VoiceOrWordState.Word:
					this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.VoiceNormal;
					this.RefreshUIState();
					break;
				}
			};
		}
		if (Global.Data.FriendDataList == null)
		{
			GameInstance.Game.SpriteGetFriends();
		}
		this.StartUITimer();
		GChat.ClearAllVoiceState();
	}

	private int GetChannelType(int channelId)
	{
		int result = 2;
		switch (channelId)
		{
		case 0:
			result = 2;
			break;
		case 1:
			result = 2;
			break;
		case 2:
			result = 3;
			break;
		case 3:
			result = 4;
			break;
		case 4:
			result = 5;
			break;
		case 6:
			result = 7;
			break;
		case 7:
			result = 8;
			break;
		case 8:
			result = 9;
			break;
		case 9:
			result = 10;
			break;
		}
		return result;
	}

	public void OnDispose()
	{
		this.StopTimer();
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
		this.privateController.OnDispose();
	}

	public override void Destroy()
	{
		base.Destroy();
		ActivityTipManager.UnRegActivityTipItem(90001, new ActivityTipEventHandler(this.OnActivityStateChanged));
	}

	private void SetMusicAndAudio()
	{
		this.CloseGameMusic = Global.Data.SysSetting.CloseGameMusic;
		this.CloseGameAudio = Global.Data.SysSetting.CloseGameAudio;
		if (!this.CloseGameMusic)
		{
			Global.Data.SysSetting.CloseGameMusic = true;
			Global.BackgroundAudio43D.StopPlay();
		}
		if (!this.CloseGameAudio)
		{
			Global.Data.SysSetting.CloseGameAudio = true;
		}
	}

	private void ResetMusicAndAudio()
	{
		if (!this.CloseGameMusic)
		{
			Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, false), true, false);
			Global.Data.SysSetting.CloseGameMusic = false;
		}
		if (!this.CloseGameAudio)
		{
			Global.Data.SysSetting.CloseGameAudio = false;
		}
	}

	private void SendHttpVocieMessage(byte[] encodeBytes, string VoiceToWordStr = "")
	{
		string boxText = this.ChatTextBox.text;
		if (!boxText.EndsWith(" ") && this.SendChatChannelID == 4 && this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
		{
			boxText += " ";
		}
		float TimeLength = this.GetClipData(encodeBytes);
		if (!this.SendVocieMessage(string.Empty, boxText + " ", false, TimeLength, VoiceToWordStr))
		{
			return;
		}
		ClientPostAudioChatData clientPostAudioChatData = new ClientPostAudioChatData();
		ServerPostAudioChatData responseData = null;
		if (this.SendChatChannelID == 4)
		{
			clientPostAudioChatData.iTargetNum = 2;
		}
		else
		{
			clientPostAudioChatData.iTargetNum = 0;
		}
		clientPostAudioChatData.arrAudioChat = encodeBytes;
		clientPostAudioChatData.lTime = Global.GetTimeStamp();
		clientPostAudioChatData.strMD5 = MD5Helper.get_md5_string("HWjKO26fEJvZ27f8v0Qu9EGZ3k3phFO4NCt8A" + clientPostAudioChatData.iTargetNum + clientPostAudioChatData.lTime);
		VoiceRequestParam requestParam = new VoiceRequestParam();
		float newTime = Time.realtimeSinceStartup;
		requestParam.Length = TimeLength;
		TestGUIData.VoiceLength = TimeLength;
		string curChatVoiceServerURL = Global.ChatVoiceServerURL;
		requestParam.url = curChatVoiceServerURL + "PostAudioChat.aspx";
		this.LogOnScreen("requestParam.url=" + requestParam.url);
		requestParam.callback = delegate(WWW w, object obj)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					this.LogOnScreen("PostAudioChat.aspx response returnBytes length = " + bytes.Length);
					try
					{
						responseData = DataHelper.BytesToObject<ServerPostAudioChatData>(bytes, 0, bytes.Length);
					}
					catch (Exception ex)
					{
						MUDebug.LogError<Exception>(new Exception[]
						{
							ex
						});
						return;
					}
					TestGUIData.TransmitLength = Time.realtimeSinceStartup - newTime;
					if (responseData != null)
					{
						this.LogOnScreen(string.Concat(new object[]
						{
							"PostAudioChat.aspx response responseData.iAudioChatOrder = ",
							responseData.iAudioChatOrder,
							" TimeLength ==",
							TimeLength
						}));
						if (!VoiceToWordStr.Equals(string.Empty))
						{
							this.SendVocieMessage(string.Concat(new object[]
							{
								curChatVoiceServerURL,
								responseData.iAudioChatOrder,
								"#",
								requestParam.Length
							}), boxText, true, 0f, VoiceToWordStr);
						}
						else
						{
							this.SendVocieMessage(string.Concat(new object[]
							{
								curChatVoiceServerURL,
								responseData.iAudioChatOrder,
								"#",
								requestParam.Length
							}), boxText, true, 0f, string.Empty);
						}
					}
				}
			}
			else
			{
				this.SendVocieMessage("SendError", string.Empty, true, 0f, string.Empty);
				this.LogOnScreen(Global.GetLang("语音发送失败！"));
			}
		};
		requestParam.postData = DataHelper.ObjectToBytes<ClientPostAudioChatData>(clientPostAudioChatData);
		requestParam.timeout = 20;
		HttpSyncChatVoiceMgr.GetInstance().AddOneTask(requestParam);
	}

	public void RefreshTable()
	{
		this.TextList.UpdataNow();
		this.uiCollider.UpdataCollider();
		if (this.uiCollider.box != null)
		{
			Vector3 center = this.uiCollider.box.center;
			this.uiCollider.box.center = new Vector3(190f, center.y, -0.1f);
			Vector3 size = this.uiCollider.box.size;
			this.uiCollider.box.size = new Vector3(380f, size.y, size.z);
		}
		this.RefreshTextPanelPos();
	}

	public void RefreshUIState()
	{
		if (this.SendChatChannelID == 0 || this.SendChatChannelID == 1 || this.SendChatChannelID == 8)
		{
			if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.Word || this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceDisable)
			{
				this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
				this.VoiceContainer.SetActive(false);
				this.SendBtn.gameObject.SetActive(true);
				this.ChatSymbolBtn.gameObject.SetActive(true);
				this.ChatTextBox.gameObject.SetActive(true);
				this.ChatTextBox.enabled = true;
			}
			else if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
			{
				this.VoiceContainer.SetActive(true);
				this.SendVoiceBtn.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.SendBtn.gameObject.SetActive(false);
				this.ChatSymbolBtn.gameObject.SetActive(false);
				this.ChatTextBox.gameObject.SetActive(false);
				this.ChatTextBox.text = string.Empty;
			}
		}
		else if (this.SendChatChannelID == 2 || this.SendChatChannelID == 3 || this.SendChatChannelID == 1 || this.SendChatChannelID == 6 || this.SendChatChannelID == 9 || this.SendChatChannelID == 11)
		{
			if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.Word || this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceDisable)
			{
				this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
				this.VoiceContainer.SetActive(false);
				this.SendBtn.gameObject.SetActive(true);
				this.ChatSymbolBtn.gameObject.SetActive(true);
				this.ChatTextBox.gameObject.SetActive(true);
				this.ChatTextBox.enabled = true;
			}
			else if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
			{
				this.VoiceContainer.SetActive(true);
				this.SendVoiceBtn.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.SendBtn.gameObject.SetActive(false);
				this.ChatSymbolBtn.gameObject.SetActive(false);
				this.ChatTextBox.gameObject.SetActive(false);
				this.ChatTextBox.text = string.Empty;
			}
		}
		else if (this.SendChatChannelID == 4)
		{
			if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.Word || this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceDisable)
			{
				this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
				this.VoiceContainer.SetActive(false);
				this.SendBtn.gameObject.SetActive(true);
				this.ChatSymbolBtn.gameObject.SetActive(true);
				this.ChatTextBox.gameObject.SetActive(true);
				this.ChatTextBox.enabled = true;
			}
			else if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
			{
				this.VoiceContainer.SetActive(true);
				this.SendVoiceBtn.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.SendBtn.gameObject.SetActive(false);
				this.ChatSymbolBtn.gameObject.SetActive(false);
				this.ChatTextBox.gameObject.SetActive(false);
				this.ChatTextBox.enabled = false;
			}
		}
		else if (this.SendChatChannelID == 5)
		{
			this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
			this.VoiceContainer.SetActive(false);
			this.SendBtn.gameObject.SetActive(true);
			this.ChatSymbolBtn.gameObject.SetActive(true);
			this.ChatTextBox.gameObject.SetActive(true);
			this.ChatTextBox.enabled = false;
		}
		else if (!Global.isHaiWai && this.SendChatChannelID == 10)
		{
			if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.Word || this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceDisable)
			{
				this.CurVoiceOrWordState = ChatBox.VoiceOrWordState.Word;
				this.VoiceContainer.SetActive(false);
				this.SendBtn.gameObject.SetActive(true);
				this.ChatSymbolBtn.gameObject.SetActive(true);
				this.ChatTextBox.gameObject.SetActive(true);
				this.ChatTextBox.enabled = true;
			}
			else if (this.CurVoiceOrWordState == ChatBox.VoiceOrWordState.VoiceNormal)
			{
				this.VoiceContainer.SetActive(true);
				this.SendVoiceBtn.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.SendBtn.gameObject.SetActive(false);
				this.ChatSymbolBtn.gameObject.SetActive(false);
				this.ChatTextBox.gameObject.SetActive(false);
				this.ChatTextBox.text = string.Empty;
			}
		}
	}

	private ChatBox.VoiceOrWordState CurVoiceOrWordState
	{
		get
		{
			return this.m_VoiceOrWorldState;
		}
		set
		{
			this.m_VoiceOrWorldState = value;
			switch (this.m_VoiceOrWorldState)
			{
			case ChatBox.VoiceOrWordState.VoiceNormal:
				this.VoiceOrWordBtnBackground.spriteName = "keyboard_noraml";
				break;
			case ChatBox.VoiceOrWordState.VoiceDisable:
				this.VoiceOrWordBtnBackground.spriteName = "voice_disable";
				break;
			case ChatBox.VoiceOrWordState.Word:
				this.VoiceOrWordBtnBackground.spriteName = "voice_noraml";
				break;
			}
		}
	}

	private IEnumerator RefreshPos()
	{
		yield return new WaitForEndOfFrame();
		this.RefreshTable();
		yield break;
	}

	protected override void OnDestroy()
	{
		foreach (KeyValuePair<int, int> keyValuePair in this.m_chatTypeIndexDic)
		{
			if (keyValuePair.Value == this.TabBtnListBox.SelectedIndex)
			{
				Dictionary<int, int>.Enumerator enumerator;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				ChatBox.LastChatChannel = keyValuePair2.Key;
				break;
			}
		}
		base.OnDestroy();
	}

	private void ShowEmotionContainer()
	{
	}

	public void OpenChatBoxChannel(ChatChannelIndexes channelIndex)
	{
		this.TabBtnListBox.SelectedIndex = this.m_chatTypeIndexDic[(int)channelIndex];
		this.SetChannelIndex(this.TabBtnListBox.SelectedIndex);
		this.LogOnScreen("OpenChatBoxChannel");
	}

	public bool IsChatTextBoxFocus()
	{
		return false;
	}

	private Canvas GetListPannel()
	{
		return null;
	}

	private void ResetIcons(GButton icon)
	{
	}

	private void TabClick(GameObject obj, int index)
	{
	}

	public void InitPartSize()
	{
	}

	public void InitPartData()
	{
	}

	public string GetLastPrivateRoleName(bool toFocus)
	{
		if (!toFocus)
		{
			return string.Empty;
		}
		if (this.SendChatChannelID != 4)
		{
			return string.Empty;
		}
		if (string.IsNullOrEmpty(this.LastSendRoleName))
		{
			return string.Empty;
		}
		if (this.LastSendRoleName.get_Chars(0) == '/')
		{
			this.LastSendRoleName = this.LastSendRoleName.Substring(1);
			return "/";
		}
		return "/" + this.LastSendRoleName + " ";
	}

	public void ShowInputBox(string text = null, bool toFocus = true)
	{
	}

	public void SetSendMode(int id)
	{
		this.SendChatChannelID = id;
		if (id == 0)
		{
			this.ChannelIcon.Text = Global.GetLang("附近");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(16777215U);
		}
		else if (id == 1)
		{
			this.ChannelIcon.Text = Global.GetLang("世界");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[1]);
		}
		else if (id == 2)
		{
			this.ChannelIcon.Text = Global.GetLang("战盟");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[2]);
		}
		else if (id == 3)
		{
			this.ChannelIcon.Text = Global.GetLang("队伍");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[3]);
		}
		else if (id == 4)
		{
			this.ChannelIcon.Text = Global.GetLang("私聊");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[4]);
		}
		else if (id == 6)
		{
			this.ChannelIcon.Text = Global.GetLang("阵营");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[5]);
		}
		else if (id == 7)
		{
			this.ChannelIcon.Text = Global.GetLang("副本");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[5]);
		}
		else if (id == 8)
		{
			this.ChannelIcon.Text = Global.GetLang("军团");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[8])]);
		}
		else if (id == 9)
		{
			this.ChannelIcon.Text = Global.GetLang("势力");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[9])]);
		}
		else if (!Global.isHaiWai && id == 10)
		{
			this.ChannelIcon.Text = Global.GetLang("平台");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[9])]);
		}
		else if (id == 11)
		{
			this.ChannelIcon.Text = Global.GetLang("战队");
			this.ChannelIcon.Label.color = NGUIMath.HexToColorEx(this.TabListColor[this.GetColorIndex(this.strType[11])]);
		}
		if (id == 4)
		{
		}
	}

	public void SetChannelIndex(int id)
	{
		this.ChatChannelIndex = id;
		this.SendChatChannelID = id;
		ChatBoxPrivateController.ChatChannelIndexes = id;
	}

	public void NextChannelText()
	{
		this.SendChatChannelID = (this.SendChatChannelID + 1) % 5;
		this.SetSendMode(this.SendChatChannelID);
	}

	public void PrevChannelText()
	{
		this.SendChatChannelID--;
		if (this.SendChatChannelID < 0)
		{
			this.SendChatChannelID = 4;
		}
		this.SetSendMode(this.SendChatChannelID);
	}

	private void FlashIcon(int chatTypeIndexe)
	{
		if (this.ChatChannelIndex != 2 && chatTypeIndexe == 3 && this.mChatTypeLst[2] != null)
		{
			this.mChatTypeLst[2].Bselect = true;
		}
		if (this.ChatChannelIndex != 3 && chatTypeIndexe == 4 && this.mChatTypeLst[3] != null)
		{
			this.mChatTypeLst[3].Bselect = true;
		}
		if (this.ChatChannelIndex != 4 && chatTypeIndexe == 5 && this.mChatTypeLst[4] != null && !Global.Data.SysSetting.RefusePrivateChat)
		{
			this.mChatTypeLst[4].Bselect = true;
		}
		if (this.ChatChannelIndex != 8 && chatTypeIndexe == 9 && this.mChatTypeLst[8] != null)
		{
			this.mChatTypeLst[8].Bselect = true;
		}
		if (this.ChatChannelIndex != 9 && chatTypeIndexe == 10 && this.mChatTypeLst[9] != null)
		{
			this.mChatTypeLst[9].Bselect = true;
		}
	}

	private void ChatTextBoxMouseLeftButtonDown(MouseEvent e)
	{
	}

	public bool TextClick(object sender, BaseEventArgs e)
	{
		GTextBlockExItem gtextBlockExItem = sender as GTextBlockExItem;
		if (!(e.Tag is SpecialTextItem))
		{
			if (this.TextMouseLeftButtonDown != null)
			{
				this.TextMouseLeftButtonDown.Invoke(this, e.e as MouseEvent);
			}
			return true;
		}
		string text = (e.Tag as SpecialTextItem).Text;
		if (!(string.Empty != text))
		{
			return false;
		}
		if (!Super.CanShowFormatedRoleMenu(text))
		{
			if (this.TextMouseLeftButtonDown != null)
			{
				this.TextMouseLeftButtonDown.Invoke(this, e.e as MouseEvent);
			}
			return true;
		}
		if (this.FormatTextItemClick != null)
		{
			this.FormatTextItemClick.Invoke(this, e.e as MouseEvent);
		}
		return true;
	}

	public Rect GetBoundsRect()
	{
		Rect result = default(Rect);
		result.x = 0f;
		result.y = 0f;
		result.width = (float)this.Width;
		result.height = (float)this.Height;
		return result;
	}

	private string FormatChatText(ChatTextItem chatTextItem, GTextBlockEx textBlockEx)
	{
		string text = chatTextItem.TextMsg;
		string text2 = string.Empty;
		string text3 = text.Substring(0, Math.Min(5, text.Length));
		if (text3 == "[VIP]")
		{
			text3 = textBlockEx.GetFormatedStr(text3, U3DUtils.ConvertToNguiColor(4294944000U));
			text = text.Substring(5, text.Length - 5);
		}
		else
		{
			text3 = string.Empty;
		}
		if (chatTextItem.ChatIndex == ChatTypeIndexes.System)
		{
			textBlockEx.textColor = this.TabListColor[5];
			text2 = Global.GetLang("[系统]：");
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.Bulletin)
		{
			textBlockEx.textColor = this.TabListColor[0];
			text2 = Global.GetLang("[公告]：");
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.Map)
		{
			textBlockEx.textColor = 16777215U;
			text2 = StringUtil.substitute(Global.GetLang("[附近]{0}{1}："), new object[]
			{
				text3,
				textBlockEx.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID)
			});
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.World)
		{
			textBlockEx.textColor = this.TabListColor[1];
			text2 = StringUtil.substitute(Global.GetLang("[世界]{0}{1}："), new object[]
			{
				text3,
				textBlockEx.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID)
			});
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.Faction)
		{
			textBlockEx.textColor = this.TabListColor[2];
			text2 = StringUtil.substitute(Global.GetLang("[战盟]{0}{1}："), new object[]
			{
				text3,
				textBlockEx.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID)
			});
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.Team)
		{
			textBlockEx.textColor = this.TabListColor[3];
			text2 = StringUtil.substitute(Global.GetLang("[队伍]{0}{1}："), new object[]
			{
				text3,
				textBlockEx.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID)
			});
		}
		else if (chatTextItem.ChatIndex == ChatTypeIndexes.Private && chatTextItem.Status == 0)
		{
			textBlockEx.textColor = this.TabListColor[4];
			if (chatTextItem.FromRoleID == Global.Data.roleData.RoleID)
			{
				text2 = StringUtil.substitute(Global.GetLang("[私聊]{0}对{1}{2}说："), new object[]
				{
					textBlockEx.GetFormatedRolename(Global.GetLang("你"), chatTextItem.FromRoleID),
					text3,
					chatTextItem.ToRoleName
				});
			}
			else
			{
				text2 = StringUtil.substitute(Global.GetLang("[私聊]{0}{1}对你说："), new object[]
				{
					textBlockEx.GetFormatedRolename(chatTextItem.FromRoleName, chatTextItem.FromRoleID),
					text3
				});
			}
		}
		int offset = text2.IndexOf(text);
		text = textBlockEx.GetFormatedTextContent(text, offset, chatTextItem.FromRoleID, true);
		return text2 + text;
	}

	public void ShowLastSendText()
	{
		if (this.LastSendText.Count > 0)
		{
			this.lastIndex++;
			if (this.LastSendText.Count - this.lastIndex < 0)
			{
				this.lastIndex--;
				return;
			}
			this.ChatTextBox.Text = this.LastSendText[this.LastSendText.Count - this.lastIndex];
			string goodsText = this.LastSendGoodsText[this.LastSendGoodsText.Count - this.lastIndex];
			int offset = this.LastSendGoodsOffset[this.LastSendGoodsOffset.Count - this.lastIndex];
			this.ChatTextBox.SetGoodsText(goodsText, offset);
		}
	}

	public void ShowNextSendText()
	{
		if (this.LastSendText.Count > 0)
		{
			this.lastIndex--;
			if (this.LastSendText.Count - this.lastIndex >= this.LastSendText.Count)
			{
				this.lastIndex++;
				return;
			}
			this.ChatTextBox.Text = this.LastSendText[this.LastSendText.Count - this.lastIndex];
			string goodsText = this.LastSendGoodsText[this.LastSendGoodsText.Count - this.lastIndex];
			int offset = this.LastSendGoodsOffset[this.LastSendGoodsOffset.Count - this.lastIndex];
			this.ChatTextBox.SetGoodsText(goodsText, offset);
		}
	}

	public void AddSystemMessage(string msg)
	{
		GChat.AddSystemChatMessage(msg, ChatTypeIndexes.System);
		this.RefreshChat(7);
	}

	public void RefreshChat(int chatTypeIndexe = 7)
	{
		if (this.m_selectType == ChatSelectType.Private)
		{
			if (chatTypeIndexe == 5)
			{
				this.privateController.UpdateNewMessage();
			}
		}
		else
		{
			this.ShowChatContentEx();
			this.FlashIcon(chatTypeIndexe);
		}
		if (chatTypeIndexe == 5)
		{
			this.privateController.UpdatePrvateTip();
		}
	}

	private List<ChatTextItem> GetChatTextItemList(int ChatChannelIndex_)
	{
		List<ChatTextItem> list = null;
		if (ChatChannelIndex_ == 0)
		{
			list = GChat.AllChatTextList;
		}
		else if (ChatChannelIndex_ == 1)
		{
			list = GChat.WorldChatTextList;
		}
		else if (ChatChannelIndex_ == 2)
		{
			list = GChat.FactionChatTextList;
		}
		else if (ChatChannelIndex_ == 3)
		{
			list = GChat.TeamChatTextList;
		}
		else if (ChatChannelIndex_ == 4)
		{
			list = GChat.PrivateChatTextList;
		}
		else if (ChatChannelIndex_ == 5)
		{
			list = GChat.SystemChatTextList;
		}
		else if (ChatChannelIndex_ == 6)
		{
			list = GChat.ZhenYingChatTextList;
		}
		else if (ChatChannelIndex_ == 7)
		{
			list = GChat.FuBenChatTextList;
		}
		else if (ChatChannelIndex_ == 8)
		{
			list = GChat.ArmyGroupChatTextList;
		}
		else if (ChatChannelIndex_ == 9)
		{
			list = GChat.CompGroupChatTextList;
		}
		else if (!Global.isHaiWai && ChatChannelIndex_ == 10)
		{
			list = GChat.PlatformChatTextList;
		}
		else if (ChatChannelIndex_ == 11)
		{
			list = GChat.ZhanDuiChatTextList;
		}
		if (list == null)
		{
			list = new List<ChatTextItem>();
		}
		return list;
	}

	private void ShowChatContentEx()
	{
		List<ChatTextItem> chatTextItemList = this.GetChatTextItemList(this.ChatChannelIndex);
		if (0 >= this.mChatContentNeedRefreshQueue.Count)
		{
			for (int i = 0; i < chatTextItemList.Count; i++)
			{
				this.mChatContentNeedRefreshQueue.Enqueue(chatTextItemList[i]);
			}
			base.StartCoroutine<bool>(this.ShowChatContent(this.TextList, this.ShownVoiceItemExList));
		}
		else
		{
			ChatTextItem chatTextItem = Enumerable.Last<ChatTextItem>(this.mChatContentNeedRefreshQueue);
			this.mRefreshChatContent = 1;
			for (int j = 0; j < chatTextItemList.Count; j++)
			{
				if (chatTextItemList[j].ReceiveID > chatTextItem.ReceiveID)
				{
					this.mChatContentNeedRefreshQueue.Enqueue(chatTextItemList[j]);
				}
			}
			this.mRefreshChatContent = 2;
		}
		for (int k = 0; k < this.mChatTypeLst.Count; k++)
		{
			byte b = 0;
			if (this.mChatTypeLst[k].BActive && this.ChatChannelIndex != (int)this.mChatTypeLst[k].ChatChannelIndexes)
			{
				List<ChatTextItem> chatTextItemList2 = this.GetChatTextItemList((int)this.mChatTypeLst[k].ChatChannelIndexes);
				int num = chatTextItemList2.FindIndex((ChatTextItem e) => e.IsRead == 0);
				if (0 <= num)
				{
					b = 1;
				}
			}
			this.mChatTypeLst[k].ActiveTiShi = (1 == b);
		}
	}

	public IEnumerator ShowChatContent(UITable TextList, List<ChatBoxVoiceItemEx> ShownVoiceItemExList)
	{
		if (0 >= this.mChatContentNeedRefreshQueue.Count)
		{
			TextList.Clear();
			yield break;
		}
		GChat.CheckAndHandleMessage();
		List<GameObject> needDeleteList = null;
		List<ChatBoxTextItemEx> shownTextItemExList = null;
		List<ChatTextItem> shownItemList = null;
		this.CheckNeedDelete(out needDeleteList, out shownTextItemExList, out ShownVoiceItemExList, out shownItemList);
		int waiteCount = this.mChatContentNeedRefreshQueue.Count;
		int CallBackCount = this.mChatContentNeedRefreshQueue.Count;
		int i = 0;
		while (0 < this.mChatContentNeedRefreshQueue.Count)
		{
			if (0 < this.mRefreshChatContent)
			{
				if (1 < this.mRefreshChatContent)
				{
					this.CheckNeedDelete(out needDeleteList, out shownTextItemExList, out ShownVoiceItemExList, out shownItemList);
					this.mRefreshChatContent = 0;
				}
				yield return null;
			}
			ChatTextItem item = Enumerable.First<ChatTextItem>(this.mChatContentNeedRefreshQueue);
			this.mChatContentNeedRefreshQueue.Dequeue();
			item.IsRead = 1;
			if (item.ChatType == ChatType.TextOrSymbol)
			{
				int index = ChatBoxPrivateController.FindTextItem(shownTextItemExList, item);
				if (index != -1)
				{
					ChatBoxPrivateController.HandleTextOrVoice(TextList, item, i, true, shownTextItemExList[index], null, delegate(object e, DPSelectedItemEventArgs s)
					{
						CallBackCount--;
					});
				}
				else
				{
					ChatBoxPrivateController.HandleTextOrVoice(TextList, item, i, false, null, null, delegate(object e, DPSelectedItemEventArgs s)
					{
						CallBackCount--;
					});
				}
			}
			else
			{
				int index2 = ChatBoxPrivateController.FindVoiceItem(ShownVoiceItemExList, item);
				if (index2 != -1)
				{
					ChatBoxPrivateController.HandleTextOrVoice(TextList, item, i, true, null, ShownVoiceItemExList[index2], delegate(object e, DPSelectedItemEventArgs s)
					{
						CallBackCount--;
					});
				}
				else
				{
					ChatBoxPrivateController.HandleTextOrVoice(TextList, item, i, false, null, null, delegate(object e, DPSelectedItemEventArgs s)
					{
						CallBackCount--;
					});
				}
			}
			i++;
			if (i > 50)
			{
				yield return null;
			}
		}
		int count = needDeleteList.Count;
		for (int j = count - 1; j >= 0; j--)
		{
			NGUITools.Destroy(needDeleteList[j]);
		}
		needDeleteList.Clear();
		needDeleteList = null;
		shownTextItemExList.Clear();
		shownTextItemExList = null;
		if (1 < CallBackCount && --waiteCount > 0)
		{
			yield return null;
		}
		base.StartCoroutine<bool>(this.RefreshPos());
		yield break;
	}

	private void CheckNeedDelete(out List<GameObject> needDeleteList, out List<ChatBoxTextItemEx> shownTextItemExList, out List<ChatBoxVoiceItemEx> ShownVoiceItemExList, out List<ChatTextItem> shownItemList)
	{
		Transform transform = this.TextList.transform;
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child && child.gameObject)
			{
				list.Add(child);
			}
		}
		needDeleteList = new List<GameObject>();
		shownTextItemExList = new List<ChatBoxTextItemEx>();
		ShownVoiceItemExList = new List<ChatBoxVoiceItemEx>();
		shownItemList = new List<ChatTextItem>();
		shownTextItemExList.Clear();
		needDeleteList.Clear();
		ShownVoiceItemExList.Clear();
		shownItemList.Clear();
		int count = list.Count;
		for (int j = 0; j < count; j++)
		{
			if (!(list[j] == null))
			{
				ChatBoxTextItemEx component = list[j].GetComponent<ChatBoxTextItemEx>();
				if (component != null)
				{
					if (!this.mChatContentNeedRefreshQueue.Contains(component.ChatItem))
					{
						needDeleteList.Add(component.gameObject);
					}
					shownTextItemExList.Add(component);
					shownItemList.Add(component.ChatItem);
				}
				else
				{
					ChatBoxVoiceItemEx component2 = list[j].GetComponent<ChatBoxVoiceItemEx>();
					if (component2 != null)
					{
						if (!this.mChatContentNeedRefreshQueue.Contains(component2.ChatItem))
						{
							needDeleteList.Add(component2.gameObject);
						}
						ShownVoiceItemExList.Add(component2);
						shownItemList.Add(component2.ChatItem);
					}
				}
			}
		}
	}

	private void AddVoiceItemFromClient(ChatTextItem chatItem, int index)
	{
		string text = "j" + index;
		ChatBoxVoiceItemEx chatBoxVoiceItemEx = U3DUtils.NEW<ChatBoxVoiceItemEx>();
		chatBoxVoiceItemEx.Text = GChat.FormatChatVoiceText(chatItem, true, false);
		chatBoxVoiceItemEx.Speaker = this.SpeakerInstance;
		chatBoxVoiceItemEx.ChatChannelIndexes = (ChatChannelIndexes)ChatBoxPrivateController.ChatChannelIndexes;
		chatBoxVoiceItemEx.ChatItem = chatItem;
		chatBoxVoiceItemEx.CurVoiceItemState = VoiceItemState.Sending_Data;
		chatBoxVoiceItemEx.VioceToWordText = chatItem.VioceToWordStr;
		chatBoxVoiceItemEx.gameObject.name = text + "ChatBoxVoiceItemEx";
		U3DUtils.AddChild(this.TextList.gameObject, chatBoxVoiceItemEx.gameObject, true);
		this.RefreshTable();
	}

	private void AddVoice(List<byte> encodeBytes)
	{
	}

	private void RefreshTextPanelPos()
	{
		int num = (int)this.uiCollider.BoxSize.y;
		Vector4 clipRange = this.TextListPanel.clipRange;
		int num2 = num - this.LastTxtHeight;
		Vector3 localPosition = this.TextListPanelTrans.localPosition;
		Vector3 vector;
		vector..ctor(localPosition.x, localPosition.y + Math.Max((float)num - clipRange.w, 0f), localPosition.z);
		if ((float)this.LastTxtHeight > clipRange.w)
		{
			vector..ctor(localPosition.x, localPosition.y + (float)num2, localPosition.z);
		}
		this.LastTxtHeight = num;
		this.TextDragPanel.verticalScrollBar.scrollValue = 1f;
	}

	private int GetMaxSendWordsNum()
	{
		if (Global.Data.GMAuth > 0)
		{
			return 1024;
		}
		return 200;
	}

	public bool CanISendMessage()
	{
		int minLevelForSendWorldMessage = Global.GetMinLevelForSendWorldMessage();
		if (Global.Data.roleData.Level < minLevelForSendWorldMessage)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("到{0}级才能发送附近聊天信息"), new object[]
			{
				minLevelForSendWorldMessage
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		return true;
	}

	public bool CanISendArmyGroupMessage()
	{
		if (!Global.RoleHaveArmyGroup())
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您尚未加入军团，不能使用军团频道"), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		int timer = U3DUtils.GetTimer();
		ArmyGroupLegionsVO roleArmyGroupLimitsVO = ConfigArmyGroupLegions.GetRoleArmyGroupLimitsVO(Global.Data.roleData.JunTuanZhiWu);
		if (roleArmyGroupLimitsVO == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("普通成员无法使用军团频道"), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		if ("-1" == roleArmyGroupLimitsVO.TalkLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("普通成员无法使用军团频道"), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		int num = roleArmyGroupLimitsVO.TalkLevel.Split(new char[]
		{
			','
		})[0].SafeToInt32(0);
		int num2 = roleArmyGroupLimitsVO.TalkLevel.Split(new char[]
		{
			','
		})[1].SafeToInt32(0);
		if (Global.Data.roleData.ChangeLifeCount < num || Global.Data.roleData.Level < num2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}需达到{1}转{2}级才能使用军团频道"), new object[]
			{
				ConfigArmyGroupLegions.GetZhiWuNameByID(Global.Data.roleData.JunTuanZhiWu),
				num,
				num2
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		int num3 = (int)roleArmyGroupLimitsVO.TalkCD;
		int num4 = 0;
		if (this.LastSendArmyGroupMsgTicks != 0)
		{
			num4 = num3 - (timer - this.LastSendArmyGroupMsgTicks) / 1000;
		}
		if (num4 > 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}秒之后才能发送军团聊天信息"), new object[]
			{
				num4
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		return true;
	}

	public bool CanISendWorldMessage()
	{
		int minLevelForSendWorldMessage = Global.GetMinLevelForSendWorldMessage();
		if (Global.Data.roleData.Level < minLevelForSendWorldMessage)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("到{0}级才能发送世界聊天信息"), new object[]
			{
				minLevelForSendWorldMessage
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		int timer = U3DUtils.GetTimer();
		int minIntervalSecsForSendWorldMessage = Global.GetMinIntervalSecsForSendWorldMessage();
		int num = 0;
		if (this.LastSendWorldMsgTicks != 0)
		{
			num = minIntervalSecsForSendWorldMessage - (timer - this.LastSendWorldMsgTicks) / 1000;
		}
		if (num > 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}秒之后才能发送世界聊天信息"), new object[]
			{
				num
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		return true;
	}

	public bool SendMessage()
	{
		WordsFilterMgr.ExecWordsFilter(this.ChatTextBox.Text, delegate(object content, ExecWordsFilterEventArgs result)
		{
			if (result.ret > 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
				{
					result.ret,
					result.msg
				}), 0, -1, -1, 0);
				return;
			}
			if (result.is_dirty > 0)
			{
			}
			if (Global.Data.GMAuth <= 0)
			{
				this.ChatTextBox.Text = result.msg;
			}
			ChatTextItem chatTextItem = null;
			this.SendFilterMessage(ChatType.TextOrSymbol, Global.StringTrim(this.ChatTextBox.Text.Trim()), ref chatTextItem, true, string.Empty);
			if (this.SendChatChannelID == 4)
			{
				string currentRoleName = this.privateController.GetCurrentRoleName();
				if (string.Empty != currentRoleName)
				{
					this.ChatTextBox.text = string.Format("/{0} ", currentRoleName);
				}
			}
		});
		this.lastIndex = 0;
		return true;
	}

	private bool SendVocieMessage(string voiceURL, string boxText, bool sendToServer = true, float TimeLabel = 0f, string VioceToWordStr = "")
	{
		GChat.IsInLuYinAndSending = false;
		ChatTextItem chatTextItem = null;
		if (!sendToServer && (boxText == null || boxText == string.Empty))
		{
			boxText = "client";
		}
		bool flag = this.SendFilterMessage(ChatType.Voice, boxText + voiceURL, ref chatTextItem, sendToServer, VioceToWordStr);
		if (this.SendChatChannelID == 4)
		{
			string currentRoleName = this.privateController.GetCurrentRoleName();
			if (string.Empty != currentRoleName)
			{
				this.ChatTextBox.text = string.Format("/{0} ", currentRoleName);
			}
		}
		if (!flag || sendToServer || chatTextItem != null)
		{
		}
		this.lastIndex = 0;
		return flag;
	}

	private int GetFileterStringLenght(string str)
	{
		if (0 >= str.Length)
		{
			return -1;
		}
		bool flag = false;
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			if (str.get_Chars(i) == '｛' || str.get_Chars(i) == '{')
			{
				flag = true;
			}
			if (str.get_Chars(i) == '｝' || str.get_Chars(i) == '}')
			{
				num++;
				flag = false;
			}
			else if (flag)
			{
				num++;
			}
		}
		int length = str.Length;
		return length - num;
	}

	private bool SendFilterMessage(ChatType chatType, string text, ref ChatTextItem textItem, bool sendToServer = true, string VioceToWordstr = "")
	{
		if (text.CompareTo("-sysinfo") == 0)
		{
			GChat.AddSystemChatMessage(Global.GetLang("--本机的硬件信息:"), ChatTypeIndexes.System);
			GChat.AddSystemChatMessage(SystemInformation.GetSystemInfo(), ChatTypeIndexes.System);
			this.RefreshChat(7);
			return true;
		}
		if (text.CompareTo("-ol") == 0)
		{
			GChat.AddSystemChatMessage(Global.GetLang("--场景对象列表:"), ChatTypeIndexes.System);
			GChat.AddSystemChatMessage(DebugHelper.ShowAllObjects(), ChatTypeIndexes.System);
			this.RefreshChat(7);
			return true;
		}
		if (text.CompareTo("-checkversionandupdate") == 0)
		{
			GChat.AddSystemChatMessage(Global.GetLang("更新功能开启状态: " + MainGame.IsUpdateEnabled), ChatTypeIndexes.System);
			if (Global.NetVersionXML != null)
			{
			}
			this.RefreshChat(7);
			return true;
		}
		if (text.CompareTo("-cl") == 0)
		{
			Console.MyConsole.show = !Console.MyConsole.show;
			return true;
		}
		if (text.IndexOf("-en ") == 0)
		{
			string text2 = text.Substring(4);
			GameObject gameObject = U3DUtils.FindGameObjectByName(null, "/" + text2);
			if (null != gameObject)
			{
				GChat.AddSystemChatMessage(Global.GetLang("--显示对象:") + text2, ChatTypeIndexes.System);
				gameObject.SetActive(true);
			}
			return true;
		}
		if (text.IndexOf("-di ") == 0)
		{
			string text3 = text.Substring(4);
			GameObject gameObject2 = U3DUtils.FindGameObjectByName(null, "/" + text3);
			if (null != gameObject2)
			{
				GChat.AddSystemChatMessage(Global.GetLang("--隐藏对象:") + text3, ChatTypeIndexes.System);
				gameObject2.SetActive(false);
			}
			return true;
		}
		if (text.IndexOf("-en2 ") == 0)
		{
			string text4 = text.Substring(4);
			int num = text4.IndexOf("#");
			if (num != -1)
			{
				text4 = text4.Substring(0, num);
				string text5 = text4.Substring(num + 1);
				GameObject gameObject3 = U3DUtils.FindGameObjectByName(null, "/" + text4);
				if (null != gameObject3)
				{
					GChat.AddSystemChatMessage(Global.GetLang("--启动对象组件:") + text4 + "#" + text5, ChatTypeIndexes.System);
					MonoBehaviour monoBehaviour = gameObject3.GetComponent(text5) as MonoBehaviour;
					if (null != monoBehaviour)
					{
						monoBehaviour.enabled = true;
					}
				}
			}
			return true;
		}
		if (text.IndexOf("-di2 ") == 0)
		{
			string text6 = text.Substring(4);
			int num2 = text6.IndexOf("#");
			if (num2 != -1)
			{
				text6 = text6.Substring(0, num2);
				string text7 = text6.Substring(num2 + 1);
				GameObject gameObject4 = U3DUtils.FindGameObjectByName(null, "/" + text6);
				if (null != gameObject4)
				{
					GChat.AddSystemChatMessage(Global.GetLang("--禁止对象组件:") + text6 + "#" + text7, ChatTypeIndexes.System);
					MonoBehaviour monoBehaviour2 = gameObject4.GetComponent(text7) as MonoBehaviour;
					if (null != monoBehaviour2)
					{
						monoBehaviour2.enabled = false;
					}
				}
			}
			return true;
		}
		if (text.IndexOf("-clrpay") == 0)
		{
			OrderIDsStorage.Clear();
		}
		else if (text.IndexOf("-setserverid ") == 0)
		{
			int num3 = text.Substring("-setserverid ".Length).Trim().SafeToInt32(0);
			if (num3 > 0)
			{
				PlayerPrefs.SetInt("NewLastServerInfoID", num3);
				return true;
			}
		}
		if (text.CompareTo("-tformat") == 0)
		{
			GChat.AddSystemChatMessage(Global.GetLang("--本机的支持材质信息:"), ChatTypeIndexes.System);
			GChat.AddSystemChatMessage(SystemInformation.GetSupportTextureFormats(), ChatTypeIndexes.System);
			this.RefreshChat(7);
			return true;
		}
		if (text.CompareTo("-lwjinfo") == 0)
		{
			GChat.AddSystemChatMessage(Global.GetLang("--我的老玩家召回信息:"), ChatTypeIndexes.System);
			GChat.AddSystemChatMessage(RecallGoodsEx.GetRecallInfo(), ChatTypeIndexes.System);
			this.RefreshChat(7);
			return true;
		}
		if (text.IndexOf("-newvoice") == 0)
		{
			if (text.StartsWith("-newvoiceshowtime"))
			{
				text = string.Concat(new object[]
				{
					"-newvoice showtime=",
					this.VoiceChangeIntro,
					" delta=",
					this._VoiceChangeIntroDelta
				});
			}
			else if (text.StartsWith("-newvoicesettime"))
			{
				string str = text.Remove(0, "-newvoicesettime".Length);
				int num4 = ConvertExt.SafeConvertToInt32(str);
				this._VoiceChangeIntroDelta = (float)num4 - this._VoiceChangeIntro;
				text = string.Concat(new object[]
				{
					"-newvoice settime=",
					this.VoiceChangeIntro,
					" delta=",
					this._VoiceChangeIntroDelta
				});
			}
			float num5 = (float)(text.Length / 2);
			VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			voiceRequestParam.Length = num5;
			TestGUIData.VoiceLength = num5;
			string chatVoiceServerURL = Global.ChatVoiceServerURL;
			voiceRequestParam.url = chatVoiceServerURL + "PostAudioChat.aspx";
			string text8 = text.Remove(0, "-newvoice".Length);
			text8 = Global.ReplaceVioceToWordFilterFileds(text8);
			string boxText = " ";
			ServerPostAudioChatData serverPostAudioChatData = new ServerPostAudioChatData();
			string text9 = "http://192.168.0.100/PrecompiledApp/AudioChat/PostAudioChat.aspx";
			this.SendVocieMessage(string.Concat(new object[]
			{
				text9,
				serverPostAudioChatData.iAudioChatOrder,
				"#",
				voiceRequestParam.Length
			}), boxText, true, 0f, text8);
			return true;
		}
		if (text.IndexOf("-dshowbuild") == 0)
		{
			if (text.IndexOf("-dshowbuild0") == 0)
			{
				JingLingMap.debugShowBuild = false;
			}
			else
			{
				JingLingMap.debugShowBuild = true;
			}
		}
		else if (text.IndexOf("-dshowfps") == 0)
		{
			if (text.IndexOf("-dshowfps0") == 0)
			{
				FPSCounter.showFps = false;
			}
			else
			{
				FPSCounter.showFps = true;
			}
		}
		else if (text.IndexOf("-dshowshadow") == 0)
		{
			if (text.IndexOf("-dshowshadow0") == 0)
			{
				JingLingMap.debugShowShadow = false;
			}
			else
			{
				JingLingMap.debugShowShadow = true;
			}
		}
		if (this.SendChatChannelID == 5)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("当前频道不能发言"), new object[0]), 10, 3);
			this.ChatTextBox.Text = string.Empty;
			return true;
		}
		if (text == string.Empty)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("不能发送空内容"), new object[0]), 10, 3);
			this.ChatTextBox.Text = string.Empty;
			return true;
		}
		if (text.get_Chars(0) != '/' && (this.SendChatChannelID == 4 || this.SendChatChannelID == 5))
		{
			GChat.AddSystemChatMessage(Global.GetLang("请输入要私聊的角色名称"), ChatTypeIndexes.System);
			Super.HintMainText(Global.GetLang("请输入要私聊的角色名称"), 10, 3);
			this.RefreshChat(7);
			return false;
		}
		if (text.Length < 1)
		{
			text = text.ToLower();
			if (text.get_Chars(0) == '/' && text.get_Chars(1) == 's')
			{
				this.SetSendMode(0);
			}
			else if (text.get_Chars(0) == '/' && text.get_Chars(1) == 'w')
			{
				this.SetSendMode(1);
			}
			else if (text.get_Chars(0) == '/' && text.get_Chars(1) == 'g')
			{
				this.SetSendMode(2);
			}
			else if (text.get_Chars(0) == '/' && text.get_Chars(1) == 't')
			{
				this.SetSendMode(3);
			}
			else if (text.get_Chars(0) == '/' && text.get_Chars(1) == 'j')
			{
				this.SetSendMode(8);
			}
			else if (text.get_Chars(0) == '/' && text.get_Chars(1) == 'c')
			{
				this.SetSendMode(9);
			}
			else if (text.get_Chars(0) == '/')
			{
				this.SetSendMode(4);
				GChat.AddSystemChatMessage(Global.GetLang("请输入要私聊的角色名称"), ChatTypeIndexes.System);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("请输入要私聊的角色名称"), new object[0]), 0, -1, -1, 0);
				this.RefreshChat(7);
			}
			this.ChatTextBox.Text = string.Empty;
			return false;
		}
		int num6 = text.Length;
		if (num6 > this.GetMaxSendWordsNum())
		{
			num6 = this.GetFileterStringLenght(text);
		}
		int num7 = text.IndexOf("<$goods$>");
		if (num7 != -1)
		{
			num6 = num7;
		}
		if (num6 > this.GetMaxSendWordsNum() && chatType != ChatType.Voice)
		{
			GChat.AddSystemChatMessage(StringUtil.substitute(Global.GetLang("发送的聊天信息不能超过{0}个字"), new object[]
			{
				this.GetMaxSendWordsNum()
			}), ChatTypeIndexes.System);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("发送的聊天信息不能超过{0}个字"), new object[]
			{
				this.GetMaxSendWordsNum()
			}), 0, -1, -1, 0);
			this.RefreshChat(7);
			return false;
		}
		int num8 = 0;
		string text10 = string.Empty;
		string text11 = string.Empty;
		if (text.get_Chars(0) != '/' || text.Length < 3)
		{
			if (this.SendChatChannelID == 0)
			{
				text = "/s " + text;
			}
			else if (this.SendChatChannelID == 1)
			{
				text = "/w " + text;
			}
			else if (this.SendChatChannelID == 2)
			{
				text = "/g " + text;
			}
			else if (this.SendChatChannelID == 3)
			{
				text = "/t " + text;
			}
			else if (this.SendChatChannelID == 6)
			{
				text = "/z " + text;
			}
			else if (this.SendChatChannelID == 7)
			{
				text = "/f " + text;
			}
			else if (this.SendChatChannelID == 8)
			{
				text = "/j " + text;
			}
			else if (this.SendChatChannelID == 9)
			{
				text = "/c " + text;
			}
			else if (!Global.isHaiWai && this.SendChatChannelID == 10)
			{
				text = "/pt " + text;
			}
			else if (this.SendChatChannelID == 11)
			{
				text = "/d " + text;
			}
		}
		string text12 = text.Substring(0, 3);
		text12 = text12.ToLower();
		int num9;
		if (text12 == "/s ")
		{
			this.SetSendMode(0);
			if (!this.CanISendMessage())
			{
				return false;
			}
			if (!this.CanISendMapMessage())
			{
				Super.HintMainText(Global.GetLang("该场景当前频道不能发言"), 10, 3);
				return false;
			}
			if (Global.IsInKuaFuPlunderBattleMap())
			{
				num9 = 8;
			}
			else
			{
				num9 = 1;
			}
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/w ")
		{
			this.SetSendMode(1);
			if (!this.CanISendWorldMessage())
			{
				return false;
			}
			if (sendToServer)
			{
				this.LastSendWorldMsgTicks = U3DUtils.GetTimer();
			}
			num9 = 2;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/g ")
		{
			this.SetSendMode(2);
			if (Global.Data.roleData.Faction <= 0)
			{
				GChat.AddSystemChatMessage(StringUtil.substitute(Global.GetLang("您尚未加入任何战盟，不能使用战盟频道聊天"), new object[0]), ChatTypeIndexes.System);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("您尚未加入任何战盟，不能使用战盟频道聊天"), new object[0]), 0, -1, -1, 0);
				this.RefreshChat(7);
				this.ChatTextBox.Text = string.Empty;
				return false;
			}
			num9 = 3;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/t ")
		{
			this.SetSendMode(3);
			if (Global.Data.roleData.TeamID <= 0)
			{
				GChat.AddSystemChatMessage(StringUtil.substitute(Global.GetLang("组建队伍后，才能发送给队友发送消息..."), new object[0]), ChatTypeIndexes.System);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("组建队伍后，才能发送给队友发送消息..."), new object[0]), 0, -1, -1, 0);
				this.RefreshChat(7);
				this.ChatTextBox.Text = string.Empty;
				return false;
			}
			num9 = 4;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/z ")
		{
			this.SetSendMode(6);
			num9 = 7;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/f ")
		{
			this.SetSendMode(7);
			num9 = 8;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/j ")
		{
			this.SetSendMode(8);
			if (!this.CanISendArmyGroupMessage())
			{
				return false;
			}
			if (sendToServer)
			{
				this.LastSendArmyGroupMsgTicks = U3DUtils.GetTimer();
			}
			num9 = 9;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/c ")
		{
			this.SetSendMode(9);
			num9 = 10;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (!Global.isHaiWai && text12 + " " == "/pt ")
		{
			this.SetSendMode(10);
			num9 = 11;
			text10 = text.Substring(3, text.Length - 3);
		}
		else if (text12 == "/d ")
		{
			this.SetSendMode(11);
			num9 = 12;
			text10 = text.Substring(3, text.Length - 3);
		}
		else
		{
			this.SetSendMode(4);
			num9 = 5;
			int num10 = text.IndexOf(" ", 0);
			if (num10 != -1)
			{
				text11 = text.Substring(1, num10 - 1);
				text10 = text.Substring(num10 + 1, text.Length - num10 - 1);
				num8 = -(text11.Length + 1);
				num8--;
			}
			else if (chatType == ChatType.TextOrSymbol)
			{
				return false;
			}
		}
		if (text11.Length > 21)
		{
			GChat.AddSystemChatMessage(Global.GetLang("私聊时需要输入正确的角色名称"), ChatTypeIndexes.System);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("私聊时需要输入正确的角色名称"), new object[0]), 0, -1, -1, 0);
			this.RefreshChat(7);
			return false;
		}
		text11 = Global.StringReplaceAll(text11, ":", "：");
		text10 = Global.StringReplaceAll(text10, ":", "：");
		if (text10.Length <= 0 || text10.get_Chars(0) != '-')
		{
			double num11 = (double)Global.GetCorrectLocalTime();
			if (num11 - this.LastSendMessageTicks <= 800.0)
			{
				GChat.AddSystemChatMessage(Global.GetLang("发言过快，请稍后再发"), ChatTypeIndexes.System);
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("发言过快，请稍后再发"), new object[0]), 0, -1, -1, 0);
				this.RefreshChat(7);
				return false;
			}
			this.LastSendMessageText = text10;
			if (sendToServer)
			{
				this.LastSendMessageTicks = (double)Global.GetCorrectLocalTime();
			}
			text10 = Global.ReplaceFilterFileds(text10);
			text10 = Global.StringReplaceAll(text10, "【", "『");
			text10 = Global.StringReplaceAll(text10, "】", "』");
			if (Global.CurrentListData != null)
			{
				int lineID = Global.CurrentListData.LineID;
			}
			string text13 = string.Empty;
			if (Global.ProcessMonthVIP() > 0.0)
			{
				text13 = "[VIP]";
			}
			text10 = StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
			{
				text13,
				text10
			});
		}
		this.LastSendText.Add(this.ChatTextBox.Text);
		string appendGoodsText = this.ChatTextBox.getAppendGoodsText(num8);
		this.LastSendGoodsText.Add(appendGoodsText);
		this.LastSendGoodsOffset.Add(num8);
		if (this.LastSendText.Count >= 50)
		{
			this.LastSendText.RemoveAt(0);
			this.LastSendGoodsText.RemoveAt(0);
			this.LastSendGoodsOffset.RemoveAt(0);
		}
		this.ChatTextBox.Text = string.Empty;
		if (num9 == 5 && string.Empty == text11)
		{
			this.LastSendMessageText = string.Empty;
			this.LastSendMessageTicks = 0.0;
			GChat.AddSystemChatMessage(Global.GetLang("私聊时需要输入正确的角色名称"), ChatTypeIndexes.System);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("私聊时需要输入正确的角色名称"), new object[0]), 0, -1, -1, 0);
			this.RefreshChat(7);
			return false;
		}
		if (string.Empty == text10)
		{
			return false;
		}
		if (sendToServer)
		{
			if (!GChat.CheckGoodFormatedContent(text10 + appendGoodsText))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("发送的物品信息不正确"), new object[0]), 0, -1, -1, 0);
				this.ChatTextBox.Text = string.Empty;
				return false;
			}
			if (this.CheckTextColor(text10))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.OnlyErr, StringUtil.substitute(Global.GetLang("发送信息含有敏感词汇"), new object[0]), 0, -1, -1, 0);
				this.ChatTextBox.Text = string.Empty;
				return false;
			}
			if (chatType == ChatType.Voice)
			{
				string text14 = string.Empty;
				ZtBuffServerInfo ztBuffServerInfo = null;
				if (Global.GetNowServerIsZhuTiFu(Global.Data.roleData.ZoneID, out ztBuffServerInfo) && Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
				{
					if (num9 == 10)
					{
						text14 = Global.FormatSeflRoleNameShiLiZoon();
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						text14 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, Global.Data.roleData.RoleName, 0);
					}
					else
					{
						text14 = Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, Global.Data.roleData.RoleName, 0);
					}
				}
				if (string.Empty.Equals(text14))
				{
					if (num9 == 9)
					{
						text14 = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
					}
					else if (num9 == 10)
					{
						text14 = Global.FormatSeflRoleNameShiLiZoon();
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						text14 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
					}
					else
					{
						text14 = Global.FormatRoleName(Global.Data.roleData);
					}
				}
				if (VioceToWordstr.Equals(string.Empty))
				{
					GameInstance.Game.SpriteSendChat(num9, text14, text11, text10 + appendGoodsText, chatType, 0);
				}
				else
				{
					GameInstance.Game.SpriteSendChat(num9, text14, text11, text10 + appendGoodsText + "@" + VioceToWordstr, chatType, 0);
				}
			}
			else
			{
				string text15 = string.Empty;
				ZtBuffServerInfo ztBuffServerInfo2 = null;
				if (Global.GetNowServerIsZhuTiFu(Global.Data.roleData.ZoneID, out ztBuffServerInfo2) && Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !string.IsNullOrEmpty(ztBuffServerInfo2.strServerName))
				{
					if (num9 == 10)
					{
						text15 = Global.FormatSeflRoleNameShiLiZoon();
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						text15 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo2.strServerName, Global.Data.roleData.RoleName, 0);
					}
					else
					{
						text15 = Global.FormatRoleNameZhuTiFu(ztBuffServerInfo2.strServerName, Global.Data.roleData.RoleName, 0);
					}
				}
				if (string.Empty.Equals(text15))
				{
					if (num9 == 9)
					{
						text15 = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
					}
					else if (num9 == 10)
					{
						text15 = Global.FormatSeflRoleNameShiLiZoon();
					}
					else if (num9 == 5)
					{
						if (Global.IsKuaFuMap(Global.Data.roleData.MapCode, false) || Global.IsInShiLiZhengBaMap())
						{
							text15 = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
						}
						else if (SceneUIClasses.RebornMap.IsTheScene())
						{
							text15 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
						}
						else
						{
							text15 = Global.FormatRoleName(Global.Data.roleData);
						}
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						text15 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
					}
					else
					{
						text15 = Global.FormatRoleName(Global.Data.roleData);
					}
				}
				GameInstance.Game.SpriteSendChat(num9, text15, text11, text10 + appendGoodsText, chatType, 0);
			}
		}
		else
		{
			string text16 = string.Empty;
			ZtBuffServerInfo ztBuffServerInfo3 = null;
			if (Global.GetNowServerIsZhuTiFu(Global.Data.roleData.ZoneID, out ztBuffServerInfo3) && Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !string.IsNullOrEmpty(ztBuffServerInfo3.strServerName))
			{
				text16 = Global.FormatRoleNameZhuTiFu(ztBuffServerInfo3.strServerName, Global.Data.roleData.RoleName, 1);
			}
			if (string.Empty.Equals(text16))
			{
				if (num9 == 9)
				{
					text16 = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
				}
				else if (num9 == 10)
				{
					text16 = Global.FormatSeflRoleNameShiLiZoon();
				}
				else if (SceneUIClasses.RebornMap.IsTheScene())
				{
					text16 = IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Global.Data.roleData.PTID, true) + Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
				}
				else
				{
					text16 = Global.FormatRoleName(Global.Data.roleData);
				}
			}
			textItem = new ChatTextItem
			{
				FromRoleID = Global.Data.RoleID,
				FromRoleName = text16,
				ToRoleName = text11,
				TextMsg = string.Empty,
				Ticks = 0L,
				ChatIndex = (ChatTypeIndexes)num9,
				ChatType = chatType,
				PTID = Global.Data.roleData.PTID
			};
		}
		if (this.SendChatChannelID >= 4)
		{
			this.LastSendRoleName = text11;
		}
		return true;
	}

	private bool CanISendMapMessage()
	{
		return Global.GetMapSceneUIClass() != SceneUIClasses.KuaFuMap && Global.GetMapSceneUIClass() != SceneUIClasses.LangHunLingYu && Global.GetMapSceneUIClass() != SceneUIClasses.ZhengDuoZhiDi && (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() != SceneUIClasses.ZhanMengLianSaiBiSai);
	}

	private bool CheckTextColor(string text)
	{
		if (this.colorlst == null)
		{
			this.colorlst = ConfigSystemParam.GetSystemParamStringArrayByName("TalkColor", ',');
			for (int i = 0; i < this.colorlst.Length; i++)
			{
				this.colorlst[i] = "{" + this.colorlst[i] + "}";
			}
		}
		string text2 = string.Empty;
		string text3 = string.Empty;
		bool flag = false;
		bool flag2 = false;
		for (int j = 0; j < text.Length; j++)
		{
			if ("{" == text.get_Chars(j).ToString())
			{
				flag = true;
			}
			if (flag)
			{
				text2 += text.get_Chars(j);
			}
			if ("}" == text.get_Chars(j).ToString())
			{
				flag = false;
				text2 += "|";
			}
			if ("｛" == text.get_Chars(j).ToString())
			{
				flag2 = true;
			}
			if (flag2)
			{
				text3 += text.get_Chars(j);
			}
			if ("｝" == text.get_Chars(j).ToString())
			{
				flag2 = false;
				text3 += "|";
			}
		}
		string[] array = text2.Split(new char[]
		{
			'|'
		});
		bool flag3 = false;
		bool flag4 = false;
		if (array.Length > 1)
		{
			for (int k = 0; k < array.Length; k++)
			{
				if (!("{-}" == array[k].ToString()) && !(string.Empty == array[k].ToString()))
				{
					if (array[k].ToString() == "{ffffff}" || array[k].ToString() == "{FFFFFF}" || array[k].Length != 8)
					{
						flag3 = true;
						flag4 = true;
						break;
					}
					flag3 = true;
					for (int l = 0; l < this.colorlst.Length; l++)
					{
						if (this.colorlst[l].ToString() == array[k].ToString())
						{
							flag4 = true;
							break;
						}
					}
				}
			}
		}
		if (flag4)
		{
			int num = 0;
			int num2 = 0;
			foreach (string text4 in array)
			{
				if (text4.Length > 0 && "}" != text4.get_Chars(text4.Length - 1).ToString())
				{
					this.LogOnScreen(Global.GetLang("格式不对 - Color - }"));
					return true;
				}
				if (string.Empty != text4)
				{
					num++;
				}
			}
			if (num % 2 != 0)
			{
				this.LogOnScreen(Global.GetLang("格式不对 - Color"));
				return true;
			}
			foreach (string text5 in text3.Split(new char[]
			{
				'|'
			}))
			{
				if (0 < text5.Length && "｝" != text5.get_Chars(text5.Length - 1).ToString())
				{
					this.LogOnScreen(Global.GetLang("格式不对 -Content -  ｝"));
					return true;
				}
				if (string.Empty != text5)
				{
					num2++;
				}
			}
			if (num2 % 2 != 0)
			{
				this.LogOnScreen(Global.GetLang("格式不对 -Content "));
				return true;
			}
		}
		if (flag3)
		{
			if (string.IsNullOrEmpty(text3))
			{
				this.LogOnScreen(Global.GetLang("没有装备"));
				return true;
			}
			string[] array3 = text3.Split(new char[]
			{
				'|'
			});
			if (array3.Length % 2 == 0)
			{
				this.LogOnScreen(Global.GetLang("没有装备"));
				return true;
			}
			for (int num3 = 0; num3 < array3.Length - 1; num3 += 2)
			{
				string text6 = array3[num3];
				string text7 = array3[num3 + 1];
				if (text6.Split(new char[]
				{
					'_'
				}).Length != 3)
				{
					return true;
				}
				if (text7 != "｛-｝")
				{
					return true;
				}
			}
		}
		return false;
	}

	public void RenderTip()
	{
		if (this.ShowTipState)
		{
			return;
		}
		if (this.HookGoodsDbID != -1 && this.HookRoleID != -1 && (double)Global.GetCorrectLocalTime() - this.MouseEnterTicks >= 300.0)
		{
			this.ShowTipState = true;
			GameInstance.Game.SpriteGetGoodsByDbID(this.HookRoleID, this.HookGoodsDbID);
		}
	}

	public void ShowGoodsTip(GoodsData goodsData)
	{
		if (!this.ShowTipState)
		{
			return;
		}
		if (goodsData == null)
		{
			return;
		}
		if (goodsData.Id != this.HookGoodsDbID)
		{
			return;
		}
		Super.GData.CurrentChatGoodsData = goodsData;
		string tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			goodsData.GoodsID,
			0,
			goodsData.Id,
			19
		});
		GTipService.NotifyTip(this, new NotifyTipEventArgs
		{
			MouseState = true,
			TipType = TipTypes.GoodsText,
			Tip = tip,
			MouseEvent = this.MouseEnterE
		});
	}

	public void ShowChatChannelMenuWindow(Transform targetTrans, int targetWidth, int targetHeight, int index = 0)
	{
		if (null != this.menuPart)
		{
			if (this.menuPart.Visibility)
			{
				this.menuPart.Visibility = false;
			}
			else
			{
				this.menuPart.Visibility = true;
			}
			return;
		}
		this.menuPart = U3DUtils.NEW<GTxtMenuPart>();
		this.menuPart.Width = 150.0;
		this.menuPart.ItemHeight = 35;
		if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap)
		{
			this.menuPart.AddMenuItem(1, Global.GetLang("世界 /w"));
			this.menuPart.AddMenuItem(3, Global.GetLang("队伍 /t"));
			this.menuPart.AddMenuItem(4, Global.GetLang("私聊 /"));
			if (Global.RoleHaveArmyGroup())
			{
				this.menuPart.AddMenuItem(5, Global.GetLang("军团 /j"));
			}
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || Global.GetMapSceneUIClass() == SceneUIClasses.ZhengDuoZhiDi)
		{
			this.menuPart.AddMenuItem(2, Global.GetLang("战盟 /g"));
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("附近 /s"));
			if (Global.RoleHaveArmyGroup())
			{
				this.menuPart.AddMenuItem(5, Global.GetLang("军团 /j"));
			}
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai)
		{
			this.menuPart.AddMenuItem(2, Global.GetLang("战盟 /g"));
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("附近 /s"));
			this.menuPart.AddMenuItem(2, Global.GetLang("战盟 /g"));
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattleMiDong)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("附近 /s"));
			this.menuPart.AddMenuItem(6, Global.GetLang("势力 /c"));
		}
		else
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("附近 /s"));
			this.menuPart.AddMenuItem(1, Global.GetLang("世界 /w"));
			this.menuPart.AddMenuItem(2, Global.GetLang("战盟 /g"));
			this.menuPart.AddMenuItem(3, Global.GetLang("队伍 /t"));
			this.menuPart.AddMenuItem(4, Global.GetLang("私聊 /"));
			if (Global.RoleHaveArmyGroup())
			{
				this.menuPart.AddMenuItem(5, Global.GetLang("军团 /j"));
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.Comp)
			{
				this.menuPart.AddMenuItem(6, Global.GetLang("势力 /c"));
			}
		}
		this.menuPart.RenderMenu();
		this.menuPart.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != this.menuPart)
			{
				NGUITools.Destroy(this.menuPart.gameObject);
			}
			this.menuPart = null;
		};
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GTxtMenuItem gtxtMenuItem = s as GTxtMenuItem;
			if (null == gtxtMenuItem)
			{
				return;
			}
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 6)
			{
				if (gtxtMenuItem.MenuItemText == Global.GetLang("军团 /j"))
				{
					this.SetSendMode(8);
				}
				else if (gtxtMenuItem.MenuItemText == Global.GetLang("势力 /c"))
				{
					this.SetSendMode(9);
				}
				else
				{
					this.SetSendMode(gtxtMenuItem.MenuItemID);
				}
			}
			this.RefreshUIState();
			this.menuPart.Visibility = false;
		};
		int num = (int)this.menuPart.Bak.transform.localScale.y;
		int num2 = (int)((double)(targetTrans.localPosition.x - (float)(targetWidth / 2)) - (this.menuPart.Width - (double)targetWidth) / 2.0);
		int num3 = (int)(targetTrans.localPosition.y + (float)(targetHeight / 2) + (float)num);
		U3DUtils.AddChild(targetTrans.parent.gameObject, this.menuPart.gameObject, true);
		this.menuPart.transform.localPosition = new Vector3((float)num2, (float)num3, -1.5f);
		if (index == 8)
		{
			index = 5;
		}
		this.menuPart.SelectIndex = index;
	}

	public void ShowVoiceMark()
	{
		if (!this.LuYin.activeSelf)
		{
			this.LuYin.SetActive(true);
		}
		if (this.CancelYuYin.activeSelf)
		{
			this.CancelYuYin.SetActive(false);
		}
		if (this.LuYin.activeSelf || this.CancelYuYin.activeSelf)
		{
			this.Slider.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			this.Slider.transform.parent.gameObject.SetActive(false);
		}
	}

	public void HideVoiceMark()
	{
		if (this.LuYin.activeSelf)
		{
			this.LuYin.SetActive(false);
		}
		if (!this.CancelYuYin.activeSelf)
		{
			this.CancelYuYin.SetActive(true);
		}
		if (this.LuYin.activeSelf || this.CancelYuYin.activeSelf)
		{
			this.Slider.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			this.Slider.transform.parent.gameObject.SetActive(false);
		}
	}

	public void VioceToWord(string str)
	{
		this.LogOnScreen(string.Concat(new object[]
		{
			Global.GetLang("@语音转文字回调接受 str == "),
			str,
			Global.GetLang("@语音转文字回调接受 GChat.IsInvokeSendOver == "),
			GChat.IsInvokeSendOver,
			Global.GetLang("@语音转文字回调接受 GChat.IsInLuYinAndSending == "),
			GChat.IsInLuYinAndSending,
			Global.GetLang("@语音转文字回调接受 CanSendVioce == "),
			this.CanSendVioce
		}));
		GChat.IsInLuYinAndSending = false;
		if (GChat.IsInvokeSendOver)
		{
			GChat.IsSendOver = false;
			return;
		}
		base.CancelInvoke("SendVioceToWordMesg");
		GChat.IsSendOver = true;
		List<byte> encodeBytes = this.SpeakerInstance.EncodeBytes;
		if (this.CanSendVioce && encodeBytes != null)
		{
			if (str.Equals(string.Empty))
			{
				this.SendHttpVocieMessage(encodeBytes.ToArray(), Global.GetLang(" "));
			}
			else
			{
				str = Global.ReplaceVioceToWordFilterFileds(str);
				this.SendHttpVocieMessage(encodeBytes.ToArray(), str);
			}
		}
	}

	private void SetVioce()
	{
		this.VoiceTimeLength = 0;
		base.CancelInvoke("Moveing");
		base.CancelInvoke("VoiceSimple");
	}

	private void VoiceSimple()
	{
		this.VoiceSize();
	}

	public void Moveing()
	{
		this.Slider.sliderValue += 0.01666667f;
	}

	public void VoiceSize()
	{
		int volume = this.MUVoice.GetVolume();
		if (volume <= 800)
		{
			this.ShowVoiceSize(1);
		}
		else if (volume <= 1600)
		{
			this.ShowVoiceSize(2);
		}
		else if (volume <= 2400)
		{
			this.ShowVoiceSize(3);
		}
		else if (volume <= 3200)
		{
			this.ShowVoiceSize(4);
		}
		else if (volume <= 4000)
		{
			this.ShowVoiceSize(5);
		}
		else
		{
			this.ShowVoiceSize(6);
		}
	}

	private void ShowVoiceSize(int size = 0)
	{
		int count = this.ListVoiceSize.Count;
		for (int i = 0; i < count; i++)
		{
			if (i < size)
			{
				this.ListVoiceSize[i].gameObject.SetActive(true);
			}
			else if (this.ListVoiceSize[i].gameObject.activeSelf)
			{
				this.ListVoiceSize[i].gameObject.SetActive(false);
			}
		}
	}

	private float GetClipData(byte[] encodeData)
	{
		List<float> list = null;
		float result = -1f;
		if (list == null)
		{
			this.EncodeData = encodeData;
			if (this.EncodeData != null)
			{
				List<AudioClip> decodeBytes = this.SpeakerInstance.GetDecodeBytes(this.EncodeData);
				list = USpeakerOut.GetClipDataLength(decodeBytes, ref result, this.SpeakerInstance.audioFrequency);
			}
		}
		return result;
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("ChatBox_Timer");
			this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (GChat.IsZhengZaiLuYin)
		{
			this.VoiceTimeLength++;
			this.refreshProgress();
			GChat.ILuYinSecondCout++;
		}
		else
		{
			GChat.ILuYinSecondCout = -1;
		}
	}

	private void refreshProgress()
	{
		float num = (float)this.VoiceTimeLength / this.MUVoice.RecordVoiceMaxLength_sec;
		if (num >= 0.8f)
		{
			this.ProgessT.spriteName = "luyin_bar_red";
		}
		else
		{
			this.ProgessT.spriteName = "luyin_bar_green";
		}
	}

	private void LogOnScreen(string log)
	{
	}

	public void ClearAllVoiceState()
	{
	}

	private void SetButtonSprite(GButton btn, string spriteName)
	{
		btn.target.spriteName = spriteName;
		btn.normalSprite = spriteName;
		btn.hoverSprite = spriteName;
		btn.pressedSprite = spriteName;
	}

	private void SetButtonSelectState(GButton btn, bool beSelected)
	{
		btn.Width = (float)((!beSelected) ? 49 : 53);
		btn.Height = (float)((!beSelected) ? 85 : 127);
		Vector3 localPosition = btn.transform.localPosition;
		btn.transform.localPosition = new Vector3((float)((!beSelected) ? -47 : -43), localPosition.y, localPosition.z);
		btn.target.depth = ((!beSelected) ? 2 : 3);
		string spriteName = (!beSelected) ? "AnNiu_CeMian02" : "AnNiu_CeMian01";
		this.SetButtonSprite(btn, spriteName);
		btn.Refresh();
	}

	private void SetSelectType(ChatSelectType type)
	{
		if (this.m_selectType == type)
		{
			return;
		}
		this.m_selectType = type;
		this.SetButtonSelectState(this.btnChannel, this.m_selectType == ChatSelectType.Channel);
		this.SetButtonSelectState(this.btnPrivate, this.m_selectType == ChatSelectType.Private);
		this.goChannelContainer.SetActive(this.m_selectType == ChatSelectType.Channel);
		this.goPrivateContainer.SetActive(this.m_selectType == ChatSelectType.Private);
		if (this.m_selectType == ChatSelectType.Channel)
		{
			this.RefreshUIState();
		}
		else
		{
			this.ChatTextBox.enabled = true;
		}
		Color white = Color.white;
		Color color = NGUIMath.HexToColorEx(8421505U);
		this.btnChannel.TextColor = ((this.m_selectType != ChatSelectType.Channel) ? color : white);
		this.btnPrivate.TextColor = ((this.m_selectType != ChatSelectType.Private) ? color : white);
		if (type == ChatSelectType.Private)
		{
			this.m_formerChannelIndex = this.ChatChannelIndex;
			this.SetChannelIndex(4);
		}
		else
		{
			this.SetChannelIndex(this.m_formerChannelIndex);
			this.ChatTextBox.text = string.Empty;
			this.ShowChatContentEx();
		}
	}

	public void OpenPrivateChat(string roleName)
	{
		this.SetSelectType(ChatSelectType.Private);
		this.privateController.SetTalkRole(roleName);
	}

	private void OnChatRoleSelect(ChatBoxPrivateBtnItem item)
	{
		if (item == null)
		{
			this.ChatTextBox.text = string.Format(string.Empty, new object[0]);
		}
		else
		{
			this.ChatTextBox.text = string.Format("/{0} ", item.RoleInfo.RoleName);
		}
	}

	private void OnActivityStateChanged(int type, ActivityTipItem args)
	{
		if (type == 90001)
		{
			this.imgPrivateTip.SetActive(args.IsActive);
		}
	}

	public static int LastChatChannel = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage Bak;

	public GameObject CloseBtn;

	public GButton SendBtn;

	public UIButton ChatSymbolBtn;

	public UIButton m_BtnFriend;

	public UIButton m_BtnWuPin;

	public UILabel m_MicrophoneLabel;

	public GameObject MicrophoneContainer;

	public GButton ChannelIcon;

	public string m_strPraviteName = string.Empty;

	public ListBox TabBtnListBox;

	public UIDraggablePanel DranggabelPanel;

	public GameObject BtnItem;

	public UIPanel TextListPanel;

	public UITable TextList;

	private Transform TextListPanelTrans;

	public UIDraggablePanel TextDragPanel;

	public TextBox ChatTextBox;

	public GameObject VoiceContainer;

	public UIButton VoiceOrWordBtn;

	public GButton SendVoiceBtn;

	public UISprite SendVoiceSP;

	public UISprite VoiceOrWordBtnBackground;

	public EventHandler FormatTextItemClick;

	public USpeaker SpeakerInstance;

	public GameObject LuYin;

	public GameObject CancelYuYin;

	public UISprite ProgessT;

	public UISlider Slider;

	public UILabel BiaoYuLabel;

	public List<UISprite> ListVoiceSize = new List<UISprite>();

	public GButton btnChannel;

	public GButton btnPrivate;

	public GameObject imgPrivateTip;

	public GameObject goChannelContainer;

	public GameObject goPrivateContainer;

	public ChatBoxPrivateController privateController;

	[SerializeField]
	private GameObject _ChatWinCloseBtn;

	private bool isLuYinNow;

	private UICollider uiCollider;

	private List<ChatBoxVoiceItemEx> ShownVoiceItemExList = new List<ChatBoxVoiceItemEx>();

	private float ProssLength;

	private int VoiceTimeLength;

	private float DragLength = 60f;

	private bool isDragSend = true;

	private bool CanSendVioce;

	private float witeTime;

	private bool witeTimeIs = true;

	private float _VoiceChangeIntro = 2f;

	private float _VoiceChangeIntroDelta;

	private int m_formerChannelIndex;

	private int _ChatChannelIndex;

	private int mSendChatChannelID;

	private List<string> LastSendText = new List<string>();

	private List<string> LastSendGoodsText = new List<string>();

	private List<int> LastSendGoodsOffset = new List<int>();

	private string LastSendMessageText = string.Empty;

	private double LastSendMessageTicks;

	private string LastSendRoleName = "/";

	private bool CloseGameMusic;

	private bool CloseGameAudio;

	private uint[] TabListColor = new uint[]
	{
		14599836U,
		16381698U,
		10079487U,
		65280U,
		14680314U,
		16751880U,
		14599836U,
		16750848U
	};

	private Dictionary<int, byte[]> SelfVoiceDict = new Dictionary<int, byte[]>();

	private Dictionary<int, int> m_chatTypeIndexDic = new Dictionary<int, int>();

	private ObservableCollection mOBC;

	private List<BtnHander> mChatTypeLst = new List<BtnHander>();

	private string[] strType = new string[]
	{
		Global.GetLang("综合"),
		Global.GetLang("世界"),
		Global.GetLang("战盟"),
		Global.GetLang("队伍"),
		Global.GetLang("私聊"),
		Global.GetLang("系统"),
		Global.GetLang("阵营"),
		Global.GetLang("副本"),
		Global.GetLang("军团"),
		Global.GetLang("势力"),
		Global.GetLang("平台"),
		Global.GetLang("战队")
	};

	public ChatBoxDanMuButton danMuButton;

	private ChatSelectType m_selectType;

	public UILabel[] staticText;

	private BtnHander mLastselectBtnHander;

	private ChatBox.VoiceOrWordState m_VoiceOrWorldState;

	public EventHandler ChatChannelSendMouseLeftButtonUp;

	public EventHandler ChatChannelMouseLeftButtonUp;

	public EventHandler TextMouseLeftButtonDown;

	private int lastIndex;

	private byte mRefreshChatContent;

	private Queue<ChatTextItem> mChatContentNeedRefreshQueue = new Queue<ChatTextItem>();

	private int LastTxtHeight;

	private int LastSendArmyGroupMsgTicks;

	private int LastSendWorldMsgTicks;

	private int tempIndex;

	private string[] colorlst;

	private int HookRoleID = -1;

	private int HookGoodsDbID = -1;

	private MouseEvent MouseEnterE;

	private double MouseEnterTicks;

	private bool ShowTipState;

	private GTxtMenuPart menuPart;

	private byte[] EncodeData;

	private DispatcherTimer UITimer;

	private enum VoiceOrWordState
	{
		VoiceNormal,
		VoiceDisable,
		Word
	}
}
