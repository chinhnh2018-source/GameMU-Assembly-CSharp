using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HTMLEngine;
using HTMLEngine.NGUI;
using UnityEngine;

public class ChatBoxMini : UserControl
{
	public bool IsShowQuickVoiceInGame
	{
		set
		{
			NGUITools.SetActive(this.mQuickVoice.gameObject, false);
		}
	}

	public MUVoiceManager MUVoice
	{
		get
		{
			return MUVoiceManager.GetInstance();
		}
	}

	private bool IsOpenRecordVoiceFlag
	{
		set
		{
			if (value)
			{
				NGUITools.SetActive(this.mVoiceMask, true);
				NGUITools.SetActive(this.mProgressObj, true);
				NGUITools.SetActive(this.mRecordingObj, true);
			}
			else
			{
				NGUITools.SetActive(this.mVoiceMask, false);
				NGUITools.SetActive(this.mProgressObj, false);
				NGUITools.SetActive(this.mRecordingObj, false);
			}
		}
	}

	private bool IsShowRecordVoiceFlag
	{
		set
		{
			if (value)
			{
				NGUITools.SetActive(this.mRecordingObj, true);
				NGUITools.SetActive(this.mCancelRecordObj, false);
			}
			else
			{
				NGUITools.SetActive(this.mRecordingObj, false);
				NGUITools.SetActive(this.mCancelRecordObj, true);
			}
		}
	}

	private void InitQuickVoiceImgAndLabel()
	{
		this.voiceImg = this.mQuickVoice.GetComponentInChildren<UISprite>();
		if (!this.MUVoice.CanSendVoice(false))
		{
			this.voiceImg.spriteName = this.voiceImgs_gray[0];
		}
		else
		{
			this.voiceImg.spriteName = this.voiceImgs[0];
		}
	}

	public bool IsShowQuickVoice
	{
		get
		{
			return this.mIsShowQuickVoice;
		}
		set
		{
			this.mIsShowQuickVoice = false;
		}
	}

	public bool IsTeamVoiceIconIndex
	{
		get
		{
			return this.GetCurrentVoiceIndex() == 1;
		}
	}

	public bool IsZhanMengVoiceIconIndex
	{
		get
		{
			return this.GetCurrentVoiceIndex() == 2;
		}
	}

	public bool IsJunTuanVoiceIconIndex
	{
		get
		{
			return this.GetCurrentVoiceIndex() == 3;
		}
	}

	private int GetCurrentVoiceIndex()
	{
		if (this.mVoiceImgClickTimes >= this.voiceImgs.Length)
		{
			return 0;
		}
		return this.mVoiceImgClickTimes;
	}

	public bool HasNewMessage
	{
		get
		{
			return HintQueueIcon.HintNewPrivateMsg;
		}
		set
		{
			HintQueueIcon.HintNewPrivateMsg = value;
			if (null != this.NewChartAnim)
			{
				if (HintQueueIcon.HintNewPrivateMsg)
				{
					this.NewChartAnim.gameObject.SetActive(true);
					this.NewChartAnim.loop = true;
					this.NewChartAnim.Reset();
				}
				else
				{
					this.NewChartAnim.Stop();
					this.NewChartAnim.loop = false;
					this.NewChartAnim.gameObject.SetActive(false);
				}
			}
			if (this._NewChartAnimScale != null)
			{
				this._NewChartAnimScale.Reset();
				this._NewChartAnimScale.enabled = HintQueueIcon.HintNewPrivateMsg;
			}
		}
	}

	public bool IsShowGuaJiIcon
	{
		get
		{
			return this.mIsShowGuaJiIcon;
		}
		set
		{
			this.mIsShowGuaJiIcon = value;
			NGUITools.SetActive(this.GuajiIcon.gameObject, value);
		}
	}

	public bool IsShowGuWuIcon
	{
		get
		{
			return this.mIsShowGuWuIcon;
		}
		set
		{
			this.mIsShowGuWuIcon = value;
			NGUITools.SetActive(this.MainAngelTempelGuWuIcon.gameObject, value);
		}
	}

	protected override void InitializeComponent()
	{
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
		this.dataoshaTip.text = Global.GetLang("进入杀戮阶段战场中出现天神怒焰!");
		this.ProssLength = 1f / this.MUVoice.RecordVoiceMaxLength_sec;
		NGUITools.SetActive(this.mQuickVoice.gameObject, false);
		this.InteractionIcon.gameObject.SetActive(false);
		ChatBoxMini.NGUIDeviceInstance = new NGUIDevice();
		HtEngine.RegisterDevice(ChatBoxMini.NGUIDeviceInstance);
		ChatSymbolConfig.Singleton.PrepareChatSymbolData();
		this.uiCollider = this.TextList.gameObject.GetComponent<UICollider>();
		this.TextListPanelTrans = this.TextListPanel.transform;
		UIEventListener.Get(this.ChatBoxIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		UIEventListener.Get(this.GuajiIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.IsInZhanMengLianSaiCompetetionMap())
			{
				Super.HintMainText(Global.GetLang("跨服副本中，无法使用此功能"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 2
			});
			SystemHelpMgr.OnAction(UIObjIDs.MainIconStartAutoFight, HelpStateEvents.Clicked, -1);
		};
		UIEventListener.Get(this.BaoGuoIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 3
			});
		};
		UIEventListener.Get(this.ArmyTeQuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 4
			});
		};
		if (this.mQuickVoice != null)
		{
			this.QuickRecord();
		}
		this.ResetAutoFightingUI();
		ActivityTipManager.RegActivityTipItem(30000, delegate(int s, ActivityTipItem e)
		{
			NGUITools.SetActive(this.tipIcon.gameObject, e.IsActive);
		});
		DaTaoShaDataManager.ShowShaLuTips = delegate()
		{
			DaTaoShaDataManager.ShowShaLuTips = null;
			base.StartCoroutine<bool>(this.DaTaoShaCountDown(3));
		};
	}

	private IEnumerator DaTaoShaCountDown(int times)
	{
		NGUITools.SetActive(this.DaTaoShaHintObj, true);
		yield return new WaitForSeconds((float)times);
		NGUITools.SetActive(this.DaTaoShaHintObj, false);
		yield break;
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(30000, null);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 700 || id == 710)
			{
				SystemHelpPart.SetMask(this.BaoGuoIcon, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void QuickRecord()
	{
		UIEventListener.Get(this.mQuickVoice.gameObject).onPress = delegate(GameObject s, bool e)
		{
			if (Global.CanGuanZhan())
			{
				return;
			}
			if (!this.MUVoice.IsMuVoiceOpen("VoiceOpen"))
			{
				return;
			}
			if (!CheckSFAndWorker.checkNet())
			{
				MUDebug.Log<string>(new string[]
				{
					"不是模拟器环境"
				});
			}
			if (CheckSFAndWorker.checkNet() && !this.MUVoice.IsSimulatorOpenVoice())
			{
				Super.HintMainText(Global.GetLang("模拟器环境不能使用语音"), 10, 3);
				return;
			}
			this.isRecordPress = e;
			if (!this.isRecordPress)
			{
				if (!this.canSendVoice)
				{
					this.canSendVoice = true;
				}
				else
				{
					if (!this.isDragSend)
					{
						NGUITools.SetActive(this.mRecordingObj, false);
						NGUITools.SetActive(this.mCancelRecordObj, false);
						this.IsOpenRecordVoiceFlag = false;
						this.StopMoveRecordSlider();
					}
					if (this.isLongPress)
					{
						this.isLongPress = false;
						this.PressSeconds = 1.5f;
						if (!this.isDragSend)
						{
							this.ResetMusicAndAudio();
							this.MUVoice.IsRecording = false;
							NGUITools.SetActive(this.mRecordingObj, false);
							NGUITools.SetActive(this.mCancelRecordObj, false);
							this.IsOpenRecordVoiceFlag = false;
							this.StopMoveRecordSlider();
							this.MUVoice.CancelRecord();
						}
						else
						{
							this.ResetMusicAndAudio();
							this.MUVoice.StopAndUploadRecord();
							MUDebug.LogError<string>(new string[]
							{
								"===================================发送了几次？"
							});
							this.IsOpenRecordVoiceFlag = false;
							this.StopMoveRecordSlider();
							if (this.MUVoice.IsInRoom)
							{
								this.MUVoice.ChangeMode = 0;
							}
						}
					}
					else
					{
						this.mVoiceImgClickTimes++;
						if (this.voiceImg != null)
						{
							if (this.mVoiceImgClickTimes >= this.voiceImgs.Length)
							{
								this.mVoiceImgClickTimes = 0;
							}
							this.voiceImg.spriteName = this.voiceImgs[this.mVoiceImgClickTimes];
							if (!this.MUVoice.CanSendVoice(true) || !this.CanSendVoice(true))
							{
								this.voiceImg.spriteName = this.voiceImgs_gray[this.mVoiceImgClickTimes];
							}
						}
					}
				}
			}
		};
		UIEventListener.Get(this.mQuickVoice.gameObject).onDrag = delegate(GameObject s, Vector2 e)
		{
			if (this.isLongPress)
			{
				if (UICamera.currentTouch.totalDelta.y >= this.DragLength)
				{
					this.isDragSend = false;
					this.IsShowRecordVoiceFlag = false;
				}
				else
				{
					this.isDragSend = true;
					this.IsShowRecordVoiceFlag = true;
				}
			}
		};
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("GVoice_ChatBoxMini_Timer");
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
		this.VoiceTimeLength++;
		this.refreshProgress();
	}

	private void refreshProgress()
	{
		float num = (float)this.VoiceTimeLength / this.MUVoice.RecordVoiceMaxLength_sec;
		if (num >= 0.8f)
		{
			this.mProgressSprite.spriteName = "luyin_bar_red";
		}
		else
		{
			this.mProgressSprite.spriteName = "luyin_bar_green";
		}
		if (num >= 1f)
		{
		}
	}

	private ChatTypeIndexes GetChatChannel(int index)
	{
		ChatTypeIndexes result = ChatTypeIndexes.World;
		switch (index)
		{
		case 0:
			result = ChatTypeIndexes.World;
			break;
		case 1:
			result = ChatTypeIndexes.Team;
			break;
		case 2:
			result = ChatTypeIndexes.Faction;
			break;
		case 3:
			result = ChatTypeIndexes.ArmyGroup;
			break;
		default:
			MUDebug.LogError<string>(new string[]
			{
				"快速语音类型错误！"
			});
			break;
		}
		return result;
	}

	private new void Update()
	{
		if (Time.time >= this.mNextRefreshTime)
		{
			this.RefreshChatbox();
		}
		if (this.isRecordPress)
		{
			this.PressSeconds -= Time.deltaTime;
			if (this.PressSeconds <= 0f)
			{
				this.PressSeconds = 1.5f;
				this.isRecordPress = false;
				if (!this.MUVoice.CanSendVoice(true))
				{
					this.canSendVoice = false;
					return;
				}
				if (!this.CanSendVoice(true))
				{
					this.canSendVoice = false;
					return;
				}
				if (this.canSendVoice)
				{
					if (this.MUVoice.MicEnable())
					{
						this.SetMusicAndAudio();
						this.MUVoice.ChangeMode = 2;
						this.IsOpenRecordVoiceFlag = true;
						this.isLongPress = true;
						this.StartMoveRecordSlider();
						this.isDragSend = true;
						this.MUVoice.ChatChanel = (int)this.GetChatChannel(this.mVoiceImgClickTimes);
						NGUITools.SetActive(this.mCancelRecordObj, false);
						if (!this.MUVoice.StartRecord())
						{
							return;
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("麦克风未开启"), 10, 3);
					}
				}
			}
		}
	}

	private bool CanSendVoice(bool isShowTip = true)
	{
		if (this.GetChatChannel(this.mVoiceImgClickTimes) == ChatTypeIndexes.Faction)
		{
			if (Global.Data.roleData.Faction == 0 && Global.Data.roleData.BHName == string.Empty)
			{
				if (isShowTip)
				{
					Super.HintMainText(Global.GetLang("加入战盟后才可以使用帮会语音消息..."), 10, 3);
				}
				return false;
			}
		}
		else if (this.GetChatChannel(this.mVoiceImgClickTimes) == ChatTypeIndexes.Team)
		{
			if (Global.Data.roleData.TeamID <= 0)
			{
				if (isShowTip)
				{
					Super.HintMainText(Global.GetLang("组建队伍后，才能给队友发送语音消息..."), 10, 3);
				}
				return false;
			}
		}
		else if (this.GetChatChannel(this.mVoiceImgClickTimes) == ChatTypeIndexes.ArmyGroup && !Global.RoleHaveArmyGroup())
		{
			if (isShowTip)
			{
				Super.HintMainText(Global.GetLang("加入军团后，才可以发送语音消息"), 10, 3);
			}
			return false;
		}
		return true;
	}

	private void StartMoveRecordSlider()
	{
		this.VoiceTimeLength = 0;
		this.mSlider.sliderValue = 0f;
		this.mProgressSprite.spriteName = "luyin_bar_green";
		this.StartUITimer();
		base.InvokeRepeating("Moveing", 0f, 1f);
		base.InvokeRepeating("VoiceSimple", 0f, 0.03f);
	}

	private void VoiceSimple()
	{
		this.VoiceSize();
	}

	public void Moveing()
	{
		this.mSlider.sliderValue += 0.01666667f;
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

	private void StopMoveRecordSlider()
	{
		this.VoiceTimeLength = 0;
		this.mSlider.sliderValue = 0f;
		this.StopTimer();
		base.CancelInvoke("Moveing");
		base.CancelInvoke("VoiceSimple");
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

	public void ResetAutoFightingUI()
	{
		if (Global.IsAutoFighting())
		{
			this.GuajiSprite.spriteName = "mainGuajiEnd";
		}
		else
		{
			this.GuajiSprite.spriteName = "mainGuaji";
		}
	}

	public void ChatBoxSelectChannel(object sender, DPSelectedItemEventArgs args)
	{
	}

	public void RefreshChat(int chatTypeIndexe = 7)
	{
	}

	private void RefreshChatbox()
	{
		this.mNextRefreshTime = Time.time + this.refreshInterval;
		ChatTextItem chatTextItem;
		if (GChat.AllChatTextList == null)
		{
			chatTextItem = null;
		}
		else if (GChat.AllChatTextList.Count == 0)
		{
			chatTextItem = null;
		}
		else
		{
			chatTextItem = GChat.AllChatTextList[GChat.AllChatTextList.Count - 1];
		}
		if (this.m_lastShowChateItem != chatTextItem)
		{
			this.m_lastShowChateItem = chatTextItem;
			this.ShowChatContent();
		}
	}

	private void ShowChatContent()
	{
		List<ChatTextItem> allChatTextList = GChat.AllChatTextList;
		if (allChatTextList == null)
		{
			return;
		}
		this.TextList.Clear();
		if (allChatTextList.Count <= 0)
		{
			return;
		}
		if (allChatTextList.Count > 3)
		{
			for (int i = allChatTextList.Count - 3; i < allChatTextList.Count; i++)
			{
				this.addText(allChatTextList[i]);
			}
		}
		else
		{
			for (int j = 0; j < allChatTextList.Count; j++)
			{
				this.addText(allChatTextList[j]);
			}
		}
		this.TableNeedToUpdate = true;
	}

	public void RefreshTable()
	{
		this.TextList.UpdataNow();
		Vector3 localPosition = this.TextList.gameObject.transform.localPosition;
		this.TextList.gameObject.transform.localPosition = new Vector3(localPosition.x, 36f, localPosition.z);
	}

	private void LateUpdate()
	{
		if (this.TableNeedToUpdate)
		{
			this.LateUpdateCount++;
			if (this.LateUpdateCount >= 2)
			{
				this.RefreshTable();
				this.TableNeedToUpdate = false;
				this.LateUpdateCount = 0;
			}
		}
	}

	private void addText(ChatTextItem chatItem)
	{
		ChatBoxTextItemEx chatBoxTextItemEx = U3DUtils.NEW<ChatBoxTextItemEx>();
		chatBoxTextItemEx.ShowFace = false;
		chatBoxTextItemEx.NGUIHTMLUserName.maxLineWidth = 360;
		string userText = string.Empty;
		if (chatItem.ChatType == ChatType.TextOrSymbol)
		{
			userText = GChat.FormatChatText(chatItem, false, true);
		}
		else
		{
			userText = GChat.FormatChatVoiceText(chatItem, true, true) + Global.GetLang("[语音]");
		}
		chatBoxTextItemEx.UserText = userText;
		chatBoxTextItemEx.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
		U3DUtils.AddChild(this.TextList.gameObject, chatBoxTextItemEx.gameObject, true);
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
		if (localPosition.y >= (float)this.LastTxtHeight - clipRange.w)
		{
			this.TextListPanelTrans.localPosition = vector;
			Vector3 vector2 = vector - localPosition;
			clipRange.x -= vector2.x;
			clipRange.y -= vector2.y;
			this.TextListPanel.clipRange = clipRange;
		}
		this.LastTxtHeight = num;
	}

	public void PickUpGoods(int goodsId, Vector3 position)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
		if (goodsXmlNodeByID == null || goodsXmlNodeByID.Categoriy == 501)
		{
			return;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GameObject gameObject = Object.Instantiate<GameObject>(this.NetImgPickUpGoods.gameObject);
		gameObject.SetActive(true);
		gameObject.transform.parent = this.BaoGuoIcon.transform;
		gameObject.transform.position = new Vector3(position.x, position.y, -10f);
		gameObject.name = "GoodsID:" + goodsId;
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -15f);
		ShowNetImage component = gameObject.GetComponent<ShowNetImage>();
		component.URL = "NetImages/GameRes/" + goodsImageURLFromIconCode;
		component.xMakePerfect();
		FlyToBag component2 = gameObject.GetComponent<FlyToBag>();
		component2.StartMove();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton ChatBoxIcon;

	public UIButton InteractionIcon;

	public UIButton ArmyTeQuanIcon;

	public UIButton GuajiIcon;

	public GButton MainAngelTempelGuWuIcon;

	public UIButton BaoGuoIcon;

	public UISprite GuajiSprite;

	public UIPanel TextListPanel;

	public UITable TextList;

	private Transform TextListPanelTrans;

	private UICollider uiCollider;

	public GameObject tipIcon;

	public UIButton mQuickVoice;

	public GameObject mVoiceMask;

	public GameObject mProgressObj;

	public UISprite mProgressSprite;

	public UISlider mSlider;

	private float ProssLength;

	public GameObject mRecordingObj;

	public GameObject mCancelRecordObj;

	private float DragLength = 60f;

	private bool isDragSend = true;

	private bool isRecordPress;

	private UISprite voiceImg;

	private bool isLongPress;

	private float PressSeconds = 1.5f;

	private int mVoiceImgClickTimes;

	private string[] voiceImgs = new string[]
	{
		"voiceWorld_normal",
		"voiceTeam_normal",
		"voiceMeng_normal",
		"voiceTuan_normal"
	};

	private string[] voiceImgs_gray = new string[]
	{
		"voiceWorld_gray",
		"voiceTeam_gray",
		"voiceMeng_gray",
		"voiceTuan_gray"
	};

	private bool mIsShowQuickVoice;

	public UISpriteAnimation NewChartAnim;

	public TweenScale _NewChartAnimScale;

	public DPSelectedItemEventHandler DPSelectItem;

	public static NGUIDevice NGUIDeviceInstance;

	private ChatTextItem m_lastShowChateItem;

	public float refreshInterval = 1f;

	public float mNextRefreshTime;

	private bool TableNeedToUpdate;

	private int LateUpdateCount;

	public ShowNetImage NetImgPickUpGoods;

	private bool mIsShowGuaJiIcon = true;

	private bool mIsShowGuWuIcon = true;

	public UILabel[] staticText;

	public UILabel dataoshaTip;

	public GameObject DaTaoShaHintObj;

	private int VoiceTimeLength;

	private DispatcherTimer UITimer;

	private bool canSendVoice = true;

	public List<UISprite> ListVoiceSize = new List<UISprite>();

	private bool CloseGameMusic;

	private bool CloseGameAudio;

	private int LastTxtHeight;

	private uint[] TabListColor = new uint[]
	{
		14599836U,
		16381698U,
		10079487U,
		65280U,
		14680314U,
		16751880U
	};
}
