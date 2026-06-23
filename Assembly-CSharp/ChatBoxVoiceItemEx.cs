using System;
using System.Collections.Generic;
using System.Globalization;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChatBoxVoiceItemEx : UserControl
{
	public MUVoiceManager MUVoice
	{
		get
		{
			return MUVoiceManager.GetInstance();
		}
	}

	private void PlayVoice()
	{
		if (!this.MUVoice.CanSendVoice(true))
		{
			return;
		}
		this.MUVoice.ChangeMode = 2;
		if (!string.IsNullOrEmpty(this.voiceFileId))
		{
			if (this.MUVoice != null && this.MUVoice.PlayOffLineSound(this.voiceFileId))
			{
				this.m_CurVoiceItemState = VoiceItemState.Normal_Playing;
				this.SetMusicAndAudio();
				this.MUVoice.VoicePlayCompleteCallBack = delegate(bool result)
				{
					this.m_CurVoiceItemState = VoiceItemState.Normal_Stop;
				};
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("找不到语音文件"), 10, 3);
		}
	}

	private void StopVoice()
	{
		if (this.MUVoice != null && this.MUVoice.StopPlayingOffLineSound())
		{
			this.m_CurVoiceItemState = VoiceItemState.Normal_Stop;
			this.ResetMusicAndAudio();
		}
	}

	protected override void InitializeComponent()
	{
		UIEventListener.Get(this.SelfVoiceSprite.gameObject).onClick = delegate(GameObject s)
		{
			this.LogOnScreen("do action onClick");
			switch (this.m_CurVoiceItemState)
			{
			case VoiceItemState.EMPTY:
			case VoiceItemState.Failed:
				this.PlayVoice();
				break;
			case VoiceItemState.Normal_Stop:
				this.PlayVoice();
				break;
			case VoiceItemState.Normal_Playing:
				this.StopVoice();
				break;
			case VoiceItemState.Getting_Data:
			case VoiceItemState.Sending_Data:
				MUDebug.LogError<string>(new string[]
				{
					"Self 为什么不播放？？？"
				});
				break;
			}
		};
		UIEventListener.Get(this.OtherVoiceSprite.gameObject).onClick = delegate(GameObject s)
		{
			this.LogOnScreen("do action onClick");
			this.LogOnScreen(Global.GetLang("点击 手动播放音乐 "));
			this.ChatItem.IsRead = 1;
			this.SetRedPoint(this.ChatItem.IsRead);
			switch (this.m_CurVoiceItemState)
			{
			case VoiceItemState.EMPTY:
			case VoiceItemState.Failed:
				this.PlayVoice();
				break;
			case VoiceItemState.Normal_Stop:
				this.PlayVoice();
				break;
			case VoiceItemState.Normal_Playing:
				this.StopVoice();
				break;
			case VoiceItemState.Getting_Data:
			case VoiceItemState.Sending_Data:
				MUDebug.LogError<string>(new string[]
				{
					"Ohter 为什么不播放？？？"
				});
				break;
			}
		};
		if (this.WaitingSprite != null)
		{
			this.WaitingSprite.gameObject.SetActive(false);
		}
		if (this.PlayingSpriteAnimation != null)
		{
			this.PlayingSpriteAnimation.Stop();
		}
	}

	public void RequestHTTPAudio()
	{
	}

	public ChatChannelIndexes ChatChannelIndexes { private get; set; }

	private void RefreshFace()
	{
		this._FaceRootObj.SetActive(true);
		if (this.ChatItem != null)
		{
			this._FaceSp.enabled = true;
			if (this.ChatItem.ChatIndex == ChatTypeIndexes.System)
			{
				this._FaceSp.spriteName = "99_9";
			}
			else
			{
				this._FaceSp.spriteName = this.ChatItem.occupation + "0_0";
			}
		}
		if (this.ChatItem.FromRoleID == Global.Data.RoleID)
		{
			this.NGUIHTMLUserFace.transform.localPosition = new Vector3(390f, -37f, -0.001f);
		}
		else
		{
			this.NGUIHTMLUserFace.transform.localPosition = new Vector3(20f, -54f, -0.001f);
		}
		if (this.ChatChannelIndexes == ChatChannelIndexes.All)
		{
			if (this.ChatItem.ChatIndex < (ChatTypeIndexes)this.ChatIndexSpName.Length)
			{
				this._FaceBg.gameObject.SetActive(true);
				this._FaceBg.spriteName = this.ChatIndexSpName[(int)this.ChatItem.ChatIndex];
			}
			else
			{
				this._FaceBg.gameObject.SetActive(false);
			}
		}
		else
		{
			this._FaceBg.gameObject.SetActive(false);
		}
	}

	public ChatTextItem ChatItem
	{
		get
		{
			return this.ChatTextItemInstance;
		}
		set
		{
			if (value != null)
			{
				if (value.FromRoleID == Global.Data.RoleID)
				{
					this.SetIsSelf(true);
				}
				else
				{
					this.SetIsSelf(false);
				}
				this.SetClipLength(value.ClipLength);
			}
			this.ChatTextItemInstance = value;
			this.RefreshFace();
		}
	}

	public void SetRedPoint(int IsRead)
	{
		if (IsRead == 0)
		{
			if (!this.RedPoint.gameObject.activeSelf)
			{
				this.RedPoint.gameObject.SetActive(true);
			}
		}
		else if (IsRead == 1 && this.RedPoint.gameObject.activeSelf)
		{
			this.RedPoint.gameObject.SetActive(false);
		}
	}

	public USpeaker Speaker { get; set; }

	public VoiceItemState CurVoiceItemState
	{
		get
		{
			return this.m_CurVoiceItemState;
		}
		set
		{
			if (this && !base.enabled)
			{
				return;
			}
			this.m_CurVoiceItemState = value;
			if (this.FailSprite != null)
			{
				this.FailSprite.gameObject.SetActive(false);
			}
			if (this.WaitingSprite != null)
			{
				this.WaitingSprite.gameObject.SetActive(false);
			}
			switch (this.m_CurVoiceItemState)
			{
			case VoiceItemState.EMPTY:
				if (this.TimeLabel != null)
				{
					this.TimeLabel.text = string.Empty;
				}
				break;
			case VoiceItemState.Getting_Data:
				if (this.WaitingSprite != null)
				{
					this.WaitingSprite.gameObject.SetActive(true);
				}
				if (this.TimeLabel != null)
				{
					this.TimeLabel.text = string.Empty;
				}
				break;
			case VoiceItemState.Failed:
				if (this.FailSprite != null)
				{
					this.FailSprite.gameObject.SetActive(true);
				}
				if (this.TimeLabel != null)
				{
					this.TimeLabel.text = string.Empty;
				}
				break;
			}
		}
	}

	public void PlayRecord()
	{
	}

	public void Stop()
	{
	}

	public void SetAnimSpr()
	{
		if (this.PlayingSprite != null)
		{
			if (this.IsSelfInfo)
			{
				this.PlayingSprite.spriteName = "Talk_ChatSignalB_01";
			}
			else
			{
				this.PlayingSprite.spriteName = "Talk_ChatSignalA_01";
			}
			this.PlayingSprite.MakePixelPerfect();
		}
	}

	private void SetIsSelf(bool isSelf)
	{
		this.IsSelfInfo = isSelf;
		if (isSelf)
		{
			this.PlayingSprite = this.SelfPlayingSprite;
			this.PlayingSpriteAnimation = this.SelfPlayingSpriteAnimation;
			this.PlayingSprite.gameObject.SetActive(true);
			this.PlayingSpriteAnimation.Stop();
			this.OtherPlayingSprite.gameObject.SetActive(false);
			this.OtherPlayingSpriteAnimation.Stop();
			this.OtherVoiceSprite.gameObject.SetActive(false);
		}
		else
		{
			this.PlayingSprite = this.OtherPlayingSprite;
			this.PlayingSpriteAnimation = this.OtherPlayingSpriteAnimation;
			this.PlayingSprite.gameObject.SetActive(true);
			this.PlayingSpriteAnimation.Stop();
			this.SelfPlayingSprite.gameObject.SetActive(false);
			this.SelfPlayingSpriteAnimation.Stop();
			this.SelfVoiceSprite.gameObject.SetActive(false);
		}
	}

	private void GetAudioChatData()
	{
		this.LogOnScreen(string.Format("GetAudioChatData ChatItem.TextMsg=", this.ChatItem.TextMsg));
		this.ChatItem.TextMsg = this.ChatItem.TextMsg.Trim();
		this.LogOnScreen(string.Format("GetAudioChatData ChatItem.TextMsg=", this.ChatItem.TextMsg));
		string text = this.ChatItem.TextMsg.Split(new char[]
		{
			'@'
		})[0];
		text = Global.StringReplaceAll(text, Global.GetLang("："), ":");
		int num = text.LastIndexOf("/");
		string text2 = text.Substring(0, num + 1);
		string iAudioChatOrder = text.Substring(num + 1, text.Length - num - 1);
		this.LogOnScreen(string.Format("wholeURL{0}", text));
		ClientGetAudioChatData clientGetAudioChatData = new ClientGetAudioChatData();
		ServerGetAudioChatData responseData = null;
		clientGetAudioChatData.iAudioChatOrder = iAudioChatOrder;
		clientGetAudioChatData.lTime = Global.GetTimeStamp();
		clientGetAudioChatData.strMD5 = MD5Helper.get_md5_string("HWjKO26fEJvZ27f8v0Qu9EGZ3k3phFO4NCt8A" + clientGetAudioChatData.iAudioChatOrder + clientGetAudioChatData.lTime);
		this.LogOnScreen(string.Format(" GetAudioChat iAudioChatOrder{0}", clientGetAudioChatData.iAudioChatOrder));
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = text2 + "GetAudioChat.aspx";
		this.LogOnScreen(string.Format("GetAudioChat send requestParam.url={0}", voiceRequestParam.url));
		voiceRequestParam.callback = delegate(WWW w, object obj)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					responseData = DataHelper.BytesToObject<ServerGetAudioChatData>(bytes, 0, bytes.Length);
					this.LogOnScreen(string.Format("GetAudioChat returnBytes length:{0}", bytes.Length));
					if (responseData != null)
					{
						this.ChatItem.EncodeVoiceBytes = responseData.arrAudioChat;
						this.ChatItem.IsRead = 1;
						this.SetRedPoint(this.ChatItem.IsRead);
						if (this && this.enabled)
						{
							this.SetData(responseData.arrAudioChat);
							this.PlayRecord();
							this.SetClipLength(this.ChatItem.ClipLength);
						}
					}
				}
			}
			else if (this && this.enabled)
			{
				this.CurVoiceItemState = VoiceItemState.Failed;
				this.ChatItem.IsRead = 1;
				this.SetRedPoint(this.ChatItem.IsRead);
				if (this.SendVoiceSuc)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("语音已过期"), new object[0]), 10, 3);
				}
			}
		};
		voiceRequestParam.postData = DataHelper.ObjectToBytes<ClientGetAudioChatData>(clientGetAudioChatData);
		voiceRequestParam.timeout = 20;
		HttpSyncChatVoiceMgr.GetInstance().AddOneTask(voiceRequestParam);
	}

	public void RefreshEmptyData()
	{
		if (this.WaitingSprite != null)
		{
			this.WaitingSprite.gameObject.SetActive(true);
		}
		if (this.TimeLabel != null)
		{
			this.TimeLabel.text = string.Empty;
		}
	}

	public void SetData(byte[] encodeBytes)
	{
		this.EncodeData = encodeBytes;
		if (this.WaitingSprite != null)
		{
			this.WaitingSprite.gameObject.SetActive(false);
		}
		this.CurVoiceItemState = VoiceItemState.Normal_Stop;
	}

	private List<float> GetClipData()
	{
		if (this.ClipData == null && this.EncodeData != null)
		{
			float clipLength = -1f;
			List<AudioClip> decodeBytes = this.Speaker.GetDecodeBytes(this.EncodeData);
			this.ClipData = this.SpeakerOut.GetClipData(decodeBytes, ref clipLength, this.Speaker.audioFrequency);
			if (this.ChatItem != null && this.ChatItem.ClipLength < 0f)
			{
				this.ChatItem.ClipLength = clipLength;
			}
		}
		return this.ClipData;
	}

	private void SetClipLength(float clipLength)
	{
		this.ClipLength = clipLength;
		if (clipLength <= 0f)
		{
			clipLength = 0f;
		}
		this.resetBubbleSize();
	}

	public string Text
	{
		get
		{
			return this.NGUIHTMLInstance.html;
		}
		set
		{
			string text = value.Replace('[', '［');
			text = text.Replace(']', '］');
			this.NGUIHTMLInstance.IsVoiceInfo = true;
			this.NGUIHTMLInstance.html = text;
			int num = 0;
			float num2 = this.Label.font.CalculatePrintedSize(NGUIHTML.GetPureText(text), this.Label.supportEncoding, this.Label.symbolStyle, ref num, default(Vector2)).x * this.Label.transform.localScale.x;
			this.SpriteContainer.transform.localPosition = new Vector3(60f, -57f, 0f);
		}
	}

	public void OnDispose()
	{
		if (this && !base.enabled)
		{
			return;
		}
		if (this.IsPlayingVoice)
		{
			this.Stop();
		}
		this.Speaker = null;
		this.EncodeData = null;
		this.ClipData = null;
	}

	private void SetMusicAndAudio()
	{
		this.CloseGameMusic = Global.Data.SysSetting.CloseGameMusic;
		this.CloseGameAudio = Global.Data.SysSetting.CloseGameAudio;
		this.MUVoice.CloseGameMusic = this.CloseGameMusic;
		this.MUVoice.CloseGameAudio = this.CloseGameAudio;
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

	public string VioceToWordText
	{
		get
		{
			return this.textValue;
		}
		set
		{
			if (this.VioceToWordLabel == null || this.bak == null)
			{
				return;
			}
			if (value.Equals(string.Empty))
			{
				this.VioceToWordLabel.gameObject.SetActive(false);
			}
			else
			{
				this.VioceToWordLabel.gameObject.SetActive(true);
			}
			this.textValue = value;
			this.resetBubbleSize();
		}
	}

	private void resetBubbleSize()
	{
		this.VioceToWordLabel.text = string.Empty;
		this.TimeLabel.text = string.Empty;
		float num = 160f;
		float num2 = 0f;
		float num3 = 45f;
		if (!string.IsNullOrEmpty(this.textValue))
		{
			string text = this.textValue;
			int num4 = 0;
			float num5 = this.Label.font.CalculatePrintedSize(text, this.Label.supportEncoding, this.Label.symbolStyle, ref num4, default(Vector2)).x * this.Label.transform.localScale.x;
			if (num5 > num)
			{
				string text2 = string.Empty;
				string text3 = string.Empty;
				for (int i = 1; i <= text.Length; i++)
				{
					text3 = text.Substring(0, i);
					float num6 = this.Label.font.CalculatePrintedSize(text3, this.Label.supportEncoding, this.Label.symbolStyle, ref num4, default(Vector2)).x * this.Label.transform.localScale.x;
					if (num6 > num)
					{
						break;
					}
				}
				string text4 = string.Empty;
				text = text.Remove(0, text3.Length);
				for (int j = 1; j <= text.Length; j++)
				{
					text4 = text.Substring(0, j);
					float num6 = this.Label.font.CalculatePrintedSize(text4, this.Label.supportEncoding, this.Label.symbolStyle, ref num4, default(Vector2)).x * this.Label.transform.localScale.x;
					if (num6 > num - 50f)
					{
						text2 = "...";
						break;
					}
				}
				string text5 = text3 + "\r\n" + text4 + text2;
				this.VioceToWordLabel.text = text5;
				num2 = num;
				num3 = 65f;
			}
			else
			{
				string text6 = string.Empty;
				float num7 = 0f;
				string text7 = string.Empty;
				for (int k = 1; k <= text.Length; k++)
				{
					text7 = text.Substring(0, k);
					num7 = this.Label.font.CalculatePrintedSize(text7, this.Label.supportEncoding, this.Label.symbolStyle, ref num4, default(Vector2)).x * this.Label.transform.localScale.x;
					if (num7 > num - 50f)
					{
						text6 = "...";
						break;
					}
				}
				string text8 = text7 + text6;
				this.VioceToWordLabel.text = text8;
				num2 = num7 + 20f;
				num3 = 45f;
			}
		}
		else if (this.ClipLength >= 0f)
		{
			float num8 = this.ClipLength / 10f;
			float num9 = this.SpriteScale.x * num8;
			num2 = num9;
		}
		float num10 = num2 + 75f;
		this.SelfVoiceSprite.transform.localScale = new Vector3(num10, num3, this.SpriteScale.z);
		this.OtherVoiceSprite.transform.localScale = new Vector3(num10, num3, this.SpriteScale.z);
		if (this.ClipLength > 0f)
		{
			this.TimeLabel.text = this.ClipLength.ToString("0.0", CultureInfo.InvariantCulture) + '”';
		}
		else
		{
			this.TimeLabel.text = string.Empty;
		}
		this.TimeLabel.transform.localPosition = new Vector3(num10 + 5f, 0f, 0f);
		this.RedPoint.transform.localPosition = new Vector3(num10 + 5f, 17f, 0f);
		if (this.FailSprite)
		{
			this.FailSprite.transform.localPosition = new Vector3(num10 + 20f, 0f, 0f);
		}
		if (this.ErrorLabel)
		{
			this.ErrorLabel.transform.localPosition = new Vector3(num10 + 40f, 0f, 0f);
		}
		if (this.IsSelfInfo)
		{
			BoxCollider component = this.SelfVoiceSprite.gameObject.GetComponent<BoxCollider>();
			Vector3 center = component.center;
			center.x = 0.5f;
			component.center = center;
			Vector3 localPosition = this._VoiceContentObj.transform.localPosition;
			localPosition.x = 50f + (num - num2);
			this._VoiceContentObj.transform.localPosition = localPosition;
			this.TimeLabel.transform.localPosition = new Vector3(localPosition.x - 55f, 0f, 0f);
		}
	}

	private void LogOnScreen(string log)
	{
	}

	private const int with1 = 390;

	private const int with2 = 20;

	[SerializeField]
	private UISprite _FaceBg;

	[SerializeField]
	private UISprite _FaceBg1;

	[SerializeField]
	private NGUIHTML NGUIHTMLUserFace;

	[SerializeField]
	private UISprite _FaceSp;

	[SerializeField]
	private GameObject _FaceRootObj;

	[SerializeField]
	private GameObject _VoiceContentObj;

	public USpeakerOut SpeakerOut;

	public NGUIHTML NGUIHTMLInstance;

	private UISprite PlayingSprite;

	public UISpriteAnimation PlayingSpriteAnimation;

	public UISprite WaitingSprite;

	public UISprite FailSprite;

	public GameObject SpriteContainer;

	public UILabel Label;

	public UILabel TimeLabel;

	public UISprite SelfVoiceSprite;

	public UISprite SelfPlayingSprite;

	public UISpriteAnimation SelfPlayingSpriteAnimation;

	public UISprite OtherVoiceSprite;

	public UISprite OtherPlayingSprite;

	public UISpriteAnimation OtherPlayingSpriteAnimation;

	private float ClipLength;

	private List<float> ClipData;

	private byte[] EncodeData;

	public bool IsPlayingVoice;

	private VoiceItemState m_CurVoiceItemState;

	private Vector3 SpriteScale = new Vector3(100f, 45f, 0f);

	public bool IsSelfInfo;

	public ShowNetImage showNetImage;

	public UISprite RedPoint;

	public UILabel ErrorLabel;

	public bool SelfVoice = true;

	public bool SendVoiceSuc = true;

	public NGUIHTML TextNGUIHTMLInstance;

	public UISprite bak;

	public UILabel VioceToWordLabel;

	public UILabel testLabel;

	public string voiceFileId;

	public float voiceSeconds;

	public string voiceSpeechToText;

	private string[] ChatIndexSpName = new string[]
	{
		"xitong",
		"fujin",
		"shijie",
		"zhanmeng",
		"duiwu",
		"siliao",
		"gonggao",
		"zhenying",
		"fuben",
		"juntuan",
		"shili"
	};

	private ChatTextItem ChatTextItemInstance;

	private bool CloseGameMusic;

	private bool CloseGameAudio;

	private string textValue = string.Empty;
}
