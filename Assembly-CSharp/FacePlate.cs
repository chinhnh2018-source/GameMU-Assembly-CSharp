using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class FacePlate : UserControl
{
	public MUVoiceManager MUVoice
	{
		get
		{
			return MUVoiceManager.GetInstance();
		}
	}

	public bool MicImgStatus
	{
		get
		{
			return this.imgSpeaker.spriteName == "mic_normal";
		}
		set
		{
			if (this.imgMic != null)
			{
				this.imgMic.spriteName = ((!value) ? "mic_close" : "mic_normal");
			}
		}
	}

	public bool SpeakerImgStatus
	{
		get
		{
			return this.imgSpeaker.spriteName == "speaker_normal";
		}
		set
		{
			if (this.imgSpeaker != null)
			{
				this.imgSpeaker.spriteName = ((!value) ? "speaker_gray" : "speaker_normal");
			}
		}
	}

	public bool ForbidMic
	{
		get
		{
			return this.mForbidMic;
		}
		set
		{
			this.mForbidMic = value;
			if (this.imgMic != null)
			{
				this.imgMic.spriteName = ((!value) ? "mic_normal" : "mic_gray");
			}
		}
	}

	protected override void InitializeComponent()
	{
		if (this.btnMic != null)
		{
			UIEventListener.Get(this.btnMic).onClick = delegate(GameObject s)
			{
				if (this.ForbidMic)
				{
					this.imgMic.spriteName = "mic_gray";
					Super.HintMainText(Global.GetLang("暂无权限开启麦克"), 10, 3);
					return;
				}
				this.MUVoice.MicStatus = !this.MUVoice.MicStatus;
				this.MicImgStatus = this.MUVoice.MicStatus;
			};
		}
		if (this.btnSpeaker != null)
		{
			UIEventListener.Get(this.btnSpeaker).onClick = delegate(GameObject s)
			{
				this.MUVoice.SpeakerStatus = !this.MUVoice.SpeakerStatus;
				this.SpeakerImgStatus = this.MUVoice.SpeakerStatus;
			};
		}
		NGUITools.SetActive(this.VoiceIconObj, false);
		this.MUVoice.RealTimeVoiceHandler = delegate(bool result)
		{
			NGUITools.SetActive(this.VoiceIconObj, result);
		};
		this.MUVoice.initMicHandler = delegate(bool result)
		{
			this.MicImgStatus = result;
			this.MUVoice.MicStatus = result;
		};
		this.MUVoice.ForbidMicCallBack = delegate(bool result)
		{
			this.ForbidMic = result;
		};
		this.MUVoice.initSpeakerHandler = delegate(bool result)
		{
			this.SpeakerImgStatus = result;
			this.MUVoice.SpeakerStatus = result;
		};
		UIEventListener.Get(this.RolePhotoIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.GongjiMode.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				GameInstance.Game.SpriteUpdatePKMode((Global.Data.roleData.PKMode != 2) ? 2 : 0);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.VipBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.g_bIsYaoQingCeShi)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("该功能暂未开放，敬请期待。"), -1, -1, -1, -1, false);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		ActivityTipManager.RegActivityTipItem(10000, delegate(int s, ActivityTipItem e)
		{
			this._VIPTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(32000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		this.BtnGameCenterGift.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenGameCenterGift();
			}
		};
		if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap)
		{
			this.IsShowGameCenterGift = false;
		}
		this.mDaTaoShaRelifeCount.Text = string.Empty;
		DaTaoShaDataManager.RelifeCountCallBack = delegate(int s)
		{
			if (s > 0)
			{
				this.mDaTaoShaRelifeCount.Text = Global.GetLang("复活X") + s;
			}
			else
			{
				this.mDaTaoShaRelifeCount.Text = string.Empty;
			}
		};
	}

	public bool IsShowGameCenterGift
	{
		set
		{
			if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap)
			{
				NGUITools.SetActive(this.BtnGameCenterGift.gameObject, false);
			}
			else
			{
				NGUITools.SetActive(this.BtnGameCenterGift.gameObject, value);
			}
		}
	}

	private void OnActivityStateChanged(int type, ActivityTipItem args)
	{
		if (!SceneUIClasses.RebornMap.IsTheScene())
		{
			if (type == 32000 && null != this.TipRole)
			{
				this.TipRole.SetActive(args.IsActive);
			}
		}
		else if (null != this.TipRole)
		{
			this.TipRole.SetActive(false);
		}
	}

	protected new void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(32000, null);
		ActivityTipManager.RegActivityTipItem(10000, null);
		base.OnDestroy();
	}

	public void ShowHelpAnim(int id, int state)
	{
		if (state > 0)
		{
			SystemHelpPart.SetMask(this.RolePhoto, default(Vector4));
		}
		else if (id != 0)
		{
			SystemHelpPart.HideMask();
		}
	}

	public double Left
	{
		get
		{
			return Canvas.GetLeft(this);
		}
		set
		{
			Canvas.SetLeft(this, value);
		}
	}

	public double Top
	{
		get
		{
			return Canvas.GetTop(this);
		}
		set
		{
			Canvas.SetTop(this, value);
		}
	}

	public int ZIndex
	{
		get
		{
			return (int)Canvas.GetZIndex(this);
		}
		set
		{
			Canvas.SetZIndex(this, (double)value);
		}
	}

	public double LifeTotalWidth
	{
		get
		{
			return 0.0;
		}
	}

	public double LifeWidth
	{
		set
		{
		}
	}

	public double MagicTotalWidth
	{
		get
		{
			return 0.0;
		}
	}

	public double MagicWidth
	{
		set
		{
		}
	}

	public void FacePlate_ctor(int type)
	{
	}

	public GIcon SetFaceSign(string value, int size, bool toGray = false)
	{
		return null;
	}

	private BitmapData GetFaceIcon(string iconName, int size)
	{
		BitmapData result = null;
		try
		{
			result = Global.GetSaleBitmapData(Global.GetLoginResImage(StringUtil.substitute("{0}.png", new object[]
			{
				iconName
			})), size, size);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			result = null;
		}
		return result;
	}

	public void ResetFaceIcon(string value, bool toGray = false, int size = 64)
	{
	}

	public void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
	}

	public bool GetImageFromCaching(string key)
	{
		return true;
	}

	public void DownloadNetFaceIcon(string value)
	{
	}

	private void ShowMenuItems()
	{
	}

	public bool BodyVisible
	{
		get
		{
			return this.Body.Visibility;
		}
		set
		{
			this.Body.Visibility = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public string VSName
	{
		get
		{
			return this._VSName;
		}
		set
		{
			this._VSName = value;
		}
	}

	public int SpriteType
	{
		get
		{
			return this._SpriteType;
		}
		set
		{
			this._SpriteType = value;
			this.ShowMenuItems();
		}
	}

	public string RolePhotoUrl
	{
		set
		{
			this.RolePhoto.URL = value;
		}
	}

	public string VLevel
	{
		set
		{
			this.RoleLevel.Text = value;
			if (MUVoiceManager.GetInstance().CanSendVoice(false) && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameChatBoxMini != null && !PlayZone.GlobalPlayZone.GameChatBoxMini.IsShowQuickVoice)
			{
				PlayZone.GlobalPlayZone.GameChatBoxMini.IsShowQuickVoice = true;
			}
		}
	}

	public string Zhandouli
	{
		set
		{
			this.NewValue = value.SafeToInt32(0);
			if (this.NewValue == this.RoleZhandouli.Text.SafeToInt32(0))
			{
				return;
			}
			UIHelper.DelayInvokeOnlyOnce(0.5f, delegate(object s, EventArgs e)
			{
				int oldZhanLi = 0;
				if (!string.IsNullOrEmpty(this.RoleZhandouli.Text))
				{
					oldZhanLi = ConvertExt.SafeConvertToInt32(this.RoleZhandouli.Text);
				}
				this.RoleZhandouli.Text = this.NewValue.ToString();
				SystemHelpPart.ZhanLiChangeTo(this.NewValue, oldZhanLi);
			});
		}
	}

	public double LifePercent
	{
		set
		{
			this.HPBar.Percent = value;
		}
	}

	public double MagicPercent
	{
		set
		{
			this.MPBar.Percent = value;
		}
	}

	public string LifeText
	{
		set
		{
			this.HPBar.ProgessText = value;
			this.m_LblRoleHP.text = value;
		}
	}

	public string MagicText
	{
		set
		{
			this.MPBar.ProgessText = value;
			this.m_LblRoleMP.text = value;
		}
	}

	public string LifeTip
	{
		get
		{
			return this._LifeTip;
		}
		set
		{
			this._LifeTip = value;
		}
	}

	public string MagicTip
	{
		get
		{
			return this._MagicTip;
		}
		set
		{
			this._MagicTip = value;
		}
	}

	public double BackWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	public object ItemObject
	{
		get
		{
			return this._ItemObject;
		}
		set
		{
			this._ItemObject = value;
		}
	}

	public Canvas RootCanvas
	{
		get
		{
			return this.Container;
		}
	}

	public bool TeamMode
	{
		get
		{
			return this._TeamMode;
		}
		set
		{
			this._TeamMode = value;
		}
	}

	public int VipLev
	{
		set
		{
			this.VipLeve.spriteName = string.Format("vip_{0}", value);
			if (value == 0)
			{
				this.vipSprite.spriteName = "vip_disable";
				this.VipBtn.normalSprite = "vip_disable";
			}
			else
			{
				this.vipSprite.spriteName = "vi_normal";
				this.VipBtn.normalSprite = "vi_normal";
			}
			this.VipBtn.Pressed = true;
		}
	}

	public int PKMode_v
	{
		set
		{
			this._PKMode = value;
			this.PKModeSprite.spriteName = this.GeSpriteNameByPkMode(this._PKMode);
			this.PKMode.MarkAsChanged();
		}
	}

	private string GeSpriteNameByPkMode(int pkMode)
	{
		int num = 0;
		switch (pkMode)
		{
		case 0:
			num = 1;
			break;
		case 1:
			num = 4;
			break;
		case 2:
			num = 3;
			break;
		case 3:
			num = 2;
			break;
		case 4:
			num = 5;
			break;
		case 7:
			num = 6;
			break;
		case 8:
			num = 8;
			break;
		}
		return string.Format("pkmode0{0}", num);
	}

	internal void RefreshLeaderArmor(float value)
	{
		this._RoleArmorProgressBarSP.fillAmount = 0.5f * value + 0.5f;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton RolePhotoIcon;

	public ShowNetImage RolePhoto;

	public TextBlock VIPLevel;

	public TextBlock RoleLevel;

	public UIButton GongjiMode;

	public SpriteSL Body;

	public GImgProgressBar HPBar;

	public GImgProgressBar MPBar;

	public TextBlock RoleZhandouli;

	public UILabel m_LblRoleHP;

	public UILabel m_LblRoleMP;

	public GButton VipBtn;

	public UISprite vipSprite;

	public UISprite VipLeve;

	public GButton BtnGameCenterGift;

	public Transform _VIPTipIcon;

	public UILabel PKMode;

	public UISprite PKModeSprite;

	public CAnimation _AnimClick;

	public GameObject TipRole;

	[SerializeField]
	private UISprite _RoleArmorProgressBarSP;

	public TextBlock mDaTaoShaRelifeCount;

	public GameObject VoiceIconObj;

	public GameObject btnMic;

	public UISprite imgMic;

	public GameObject btnSpeaker;

	public UISprite imgSpeaker;

	private bool mForbidMic;

	private int _RoleID;

	private string _VSName;

	private int _SpriteType;

	private int NewValue;

	private string _LifeTip = string.Empty;

	private string _MagicTip = string.Empty;

	private object _ItemObject;

	private bool _TeamMode;

	private int _PKMode;
}
