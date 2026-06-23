using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class PlayGameGuide : UserControl
{
	public static PlayGameGuide singleton
	{
		get
		{
			return PlayGameGuide.ms_Singleton;
		}
	}

	protected override void InitializeComponent()
	{
		this.BakTrans.localScale = Super.GetMainUISize();
		UIEventListener.Get(this.BakTrans.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowTalkText();
			Super.PlayYinDaoSound("NextPage.mp3", true, false);
		};
		float num = 960f;
		int num2 = Mathf.CeilToInt((float)Screen.height / ((float)Screen.width / num));
		Transform transform = this.TopBak.transform;
		transform.localScale = new Vector3(num, (float)this.BakHeight, 0f);
		transform.localPosition = new Vector3(-num / 2f, (float)(num2 / 2), 0f);
		Transform transform2 = this.TopBorder.transform;
		float y = transform2.localScale.y;
		transform2.localScale = new Vector3(num, y, 0f);
		transform2.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - (float)this.BakHeight);
		Transform transform3 = this.BottomBak.transform;
		transform3.localScale = new Vector3(num, (float)this.BakHeight, 0f);
		transform3.localPosition = new Vector3(transform.localPosition.x, (float)(-(float)num2 / 2), 0f);
		Transform transform4 = this.BottomBorder.transform;
		transform4.localPosition = new Vector3(transform3.localPosition.x, transform3.localPosition.y + (float)this.BakHeight);
		transform4.localScale = new Vector3(num, y, 0f);
		this.BottomSprite.transform.localPosition = transform4.localPosition + new Vector3((float)((int)(num / 2f)), 0f, 0f);
		Transform transform5 = this.NPCImage.gameObject.transform;
		transform5.localPosition = new Vector3(transform5.localPosition.x, transform4.localPosition.y - 43f, transform5.localPosition.z);
		this.NPCName.transform.localPosition = transform3.localPosition + new Vector3(20f, 75f, -10f);
		this.TalkText.transform.localPosition = transform3.localPosition + new Vector3(124f, 34f, -100f);
		this.Hand.transform.localPosition = new Vector3((float)((int)(num * 0.4f)), (float)(-(float)num2 / 2 + (int)((float)this.BakHeight * 0.6f)), -11f);
		this.InitCamerasSwitch();
		PlayGameGuide.ms_Singleton = this;
	}

	protected void OnDisable()
	{
		if (null != this.NPCImage)
		{
			this.NPCImage.DestroyImmediateTexture();
		}
	}

	public int TaskPlotID
	{
		get
		{
			return this._TaskPlotID;
		}
		set
		{
			this.TalkCount = 0;
			this._TaskPlotID = value;
			this.ParseTaskSlot();
		}
	}

	private void ParseTaskSlot()
	{
		this.NPCImage.Source = null;
		this.NPCImage.Visibility = false;
		this.RoleImage.Source = null;
		this.RoleImage.Visibility = false;
		this.NPCName.Text = string.Empty;
		this.TalkText.text = string.Empty;
		this._Mode = 0;
		this._TimeParameters = -1;
		this.TalkTextList = null;
		this.TalkTextIndex = 0;
		this._StartLuaFile = string.Empty;
		this._EndLuaFile = string.Empty;
		XElement taskPlotXmlItemByID = Global.GetTaskPlotXmlItemByID(this._TaskPlotID);
		if (taskPlotXmlItemByID == null)
		{
			return;
		}
		this._Mode = Global.GetXElementAttributeInt(taskPlotXmlItemByID, "TriggerCondition");
		this._TimeParameters = Global.GetXElementAttributeInt(taskPlotXmlItemByID, "TimeParameters");
		string xelementAttributeStr = Global.GetXElementAttributeStr(taskPlotXmlItemByID, "Content");
		this._StartLuaFile = Global.GetXElementAttributeStr(taskPlotXmlItemByID, "StartLuaFile");
		this._EndLuaFile = Global.GetXElementAttributeStr(taskPlotXmlItemByID, "EndLuaFile");
		this.PostWizardID = Global.GetXElementAttributeInt(taskPlotXmlItemByID, "PostWizardID");
		this.PostOpenID = Global.GetXElementAttributeInt(taskPlotXmlItemByID, "PostOpenID");
		this.TalkTextList = Super.ParseTalkTextInfo(xelementAttributeStr);
		this.ShowTalkText();
		if (!string.IsNullOrEmpty(this._StartLuaFile))
		{
			GameInstance.Game.SpriteRunTaskPlotLua(this._TaskPlotID, 0);
		}
		if (this.StartTalk != null)
		{
			this.StartTalk.Invoke(this, null);
		}
	}

	private void ShowTalkText()
	{
		this.SetHandVisible(false);
		Global.Data.GameScene.CancelAutoFindRoad(true);
		Global.Data.GameScene.CancelAutoFight(0, true);
		UICtrlBar.singleton.CancelAutoUseSkill();
		if (this.DisableClosePart)
		{
			return;
		}
		if (this.FieldOfView_TargetCamera != 0f)
		{
			this.CameraSwitch.TargetCamera.fieldOfView = this.FieldOfView_TargetCamera;
			this.CameraSwitch.TargetCamera.transform.localPosition = this.TargetCamera_Position;
			this.CameraSwitch.TargetCamera.transform.localRotation = this.TargetCamera_Quaternion;
			this.FieldOfView_TargetCamera = 0f;
		}
		if (this.StartTime != 0 && Global.GetMyTimer() - this.StartTime < 1000)
		{
			return;
		}
		this.TalkCount++;
		Global.DisableInput = true;
		if (this.TalkTextList == null || this.TalkTextIndex >= this.TalkTextList.Count)
		{
			Global.DisableInput = false;
			this.StopAppendText();
			this.HandleTalkEndEvents();
			if (!string.IsNullOrEmpty(this._EndLuaFile))
			{
				GameInstance.Game.SpriteRunTaskPlotLua(this._TaskPlotID, 1);
			}
			if (this.PostWizardID > 0)
			{
				SystemHelpMgr.ShowHint(this.PostWizardID, 0);
				this.PostWizardID = -1;
			}
			else if (this.PostOpenID > 0)
			{
				GongnengYugaoMgr.ReadyFlyingImgAnimation(PlayZone.GlobalPlayZone, this.PostOpenID);
			}
			else
			{
				this.ProcessAutoFindRoad();
			}
			if (this.CompleteTalk != null)
			{
				this.CompleteTalk.Invoke(this, null);
			}
			return;
		}
		TalkTextNode talkTextNode = this.TalkTextList[this.TalkTextIndex++];
		if (talkTextNode.NpcID <= 0)
		{
			this.NPCName.Text = ColorCode.EncodingText(Global.FormatRoleName(Global.Data.roleData), "fffffe");
		}
		else
		{
			this.NPCName.Text = ColorCode.EncodingText(ConfigNPCs.GetNPCNameByID(talkTextNode.NpcID), "00ff00");
		}
		this.NPCImage.gameObject.SetActive(talkTextNode.NpcID > 0);
		this.RoleImage.gameObject.SetActive(talkTextNode.NpcID <= 0);
		if (this.TaskPlotID == 106 || Global.Data.roleData.IsFlashPlayer == 0)
		{
			this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.ScenceMode, CamerasSwitch.CamerasType.MainCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		}
		else if (talkTextNode.NpcID > 0)
		{
			this.SwitchCamera(talkTextNode.NpcID);
			this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		}
		else
		{
			this.SwitchCamera(talkTextNode.NpcID);
			this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.SelfCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		}
		this.StartAppendText(talkTextNode.TalkText);
		if (this.NPCImage.gameObject.activeSelf)
		{
			int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(talkTextNode.NpcID);
			string url = string.Format("NetImages/NPCs/{0}.png", Global.FormatStr("000", npcpicCodeByID));
			this.NPCImage.URL = url;
		}
		if (this.RoleImage.gameObject.activeSelf)
		{
			string url2 = string.Format("RolePics/{0}{1}.png", Global.Data.roleData.Occupation, Global.Data.roleData.RoleSex);
			this.RoleImage.URL = url2;
		}
		this.HandleTalkCountEvents();
	}

	private void StartAppendText(string str)
	{
		this.TalkText.text = string.Empty;
		this._WaitingAppendText = str;
		this._WaitingAppendTextIndex = 0;
		this.StopAppendText();
		if (string.IsNullOrEmpty(this._WaitingAppendText))
		{
			return;
		}
		this.TalkText.Text = this._WaitingAppendText;
	}

	private void StopAppendText()
	{
		base.CancelInvoke("AppendText");
	}

	private void AppendText()
	{
		if (string.IsNullOrEmpty(this._WaitingAppendText))
		{
			return;
		}
		if (this._WaitingAppendTextIndex >= this._WaitingAppendText.Length)
		{
			if (!this.DisableClosePart)
			{
				this.SetHandVisible(true);
			}
			this.StopAppendText();
			return;
		}
		TextBlock talkText = this.TalkText;
		talkText.Text += this._WaitingAppendText.get_Chars(this._WaitingAppendTextIndex);
		this._WaitingAppendTextIndex++;
	}

	private void ProcessAutoFindRoad()
	{
		if (this._Mode == 6)
		{
			if (this._TimeParameters > 0 && this._TimeParameters != (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID"))
			{
				Super.PrccessAutoTaskFindRoad(this._TimeParameters, false, true, false, false);
			}
		}
		else if (this._Mode == 8 && this._TimeParameters > 0 && this._TimeParameters != (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID"))
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this._TimeParameters);
			Global.Data.GameScene.CancelAutoFight(0, true);
			int mapCode = -1;
			int npcType = -1;
			int num = -1;
			Super.GetTaskDestNPCID(taskXmlNodeByID, ref mapCode, ref npcType, ref num);
			if (Global.Data.roleData.IsFlashPlayer <= 0)
			{
				Super.AutoFindRoad(mapCode, npcType, num, (Global.GetTaskClassByID(this._TimeParameters) <= 0) ? 1 : 0);
			}
			else if (this._TimeParameters < 106)
			{
				Global.Data.GameScene.ExternalCallNpcDialog(num, 20000);
			}
		}
	}

	public void InitCamerasSwitch()
	{
		this.CameraSwitch = U3DUtils.NEW<CamerasSwitch>();
	}

	public void TestCameraAnimation(int npcID)
	{
		Vector3 zero = Vector3.zero;
		this._TargetSprite = Global.FindSprite("Role_1610612852");
		Transform targetTrans = Global.GetTargetTrans(this._TargetSprite, "TargetCamera", out zero, null);
		this.CameraSwitch.SetTargetTransform(targetTrans, zero, 0.8f, 0.75f, 3f, 0f);
		this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.PushMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.Track, 3f, 1f, false, false, null);
	}

	public void SwitchCamera(int npcID)
	{
		if (npcID > 0)
		{
			Vector3 zero = Vector3.zero;
			this._TargetSprite = Global.FindSprite(Global.GetNPCName(npcID));
			if (this._TargetSprite != null)
			{
				Transform targetTrans = Global.GetTargetTrans(this._TargetSprite, "TargetCamera", out zero, null);
				float extSubHeight = 0f;
				if (npcID == 60900)
				{
					extSubHeight = 0.5f;
				}
				this.CameraSwitch.SetTargetTransform(targetTrans, zero, 0.8f, 0.75f, 3.5f, extSubHeight);
			}
		}
		else
		{
			Vector3 zero2 = Vector3.zero;
			this._SelfSprite = Global.FindSprite("Leader");
			if (this._SelfSprite != null)
			{
				Transform targetTrans2 = Global.GetTargetTrans(this._SelfSprite, "SelfCamera", out zero2, null);
				this.CameraSwitch.SetSelfTransform(targetTrans2, zero2, 0.8f, 0.75f, 3.5f);
			}
		}
	}

	public void ReturnCamera()
	{
		this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.ScenceMode, CamerasSwitch.CamerasType.MainCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		if (this._SelfSprite != null)
		{
			this._SelfSprite.ModifyLayers("Sprites", null);
			this._SelfSprite = null;
		}
		if (this._TargetSprite != null)
		{
			this._TargetSprite.ModifyLayers("Sprites", null);
			this._TargetSprite = null;
		}
	}

	private void HandleTalkEndEvents()
	{
		if (this._TaskPlotID == 101)
		{
		}
	}

	private void HandleTalkCountEvents()
	{
		this.SetHandVisible(true);
		if (this._TaskPlotID == 100)
		{
			if (this.TalkCount == 3)
			{
				this.SetHandVisible(false);
				this.DisableClosePart = true;
				this.StartTime = Global.GetMyTimer();
				this.CameraSwitch.AddSpriteLayer(true);
				this.FieldOfView_TargetCamera = this.CameraSwitch.TargetCamera.fieldOfView;
				this.TargetCamera_Position = this.CameraSwitch.TargetCamera.transform.localPosition;
				this.TargetCamera_Quaternion = this.CameraSwitch.TargetCamera.transform.localRotation;
				this.CameraSwitch.TargetCamera.fieldOfView = 60f;
				this.CameraSwitch._AnimationMgr.ChangeAnimation("NewPlayer_CameraAmin_00", 1, false, null, 0f);
				this.CameraSwitch._AnimationMgr.EndAnimation = delegate(object s, EventArgs e)
				{
					UIHelper.DelayInvoke(1f, delegate(object s1, EventArgs e1)
					{
						this.DisableClosePart = false;
						this.SetHandVisible(true);
					});
				};
			}
		}
		else if (this._TaskPlotID == 101)
		{
			Global.CanGiveFakeEquips = true;
			Global.FakeEquipsIndex = 1;
			base.InvokeRepeating("ReplaceEquip", 0.5f, 0f);
		}
		else if (this._TaskPlotID != 103)
		{
			if (this._TaskPlotID != 105)
			{
				if (this._TaskPlotID == 106)
				{
					if (this.TalkCount == 1)
					{
						Global.CanGiveFakeEquips = true;
						Global.FakeEquipsIndex = 0;
						base.InvokeRepeating("ReplaceEquip", 0.4f, 0f);
					}
					else if (this.TalkCount != 2)
					{
						if (this.TalkCount == 4)
						{
							CameraShake instance = CameraShake.GetInstance(this.CameraSwitch.MainCamera);
							if (null != instance)
							{
								instance.ShakeForSeconds(4f, 0.01f, 0.02f, 0.02f);
							}
							Global.GetDecoration(85, GDecorationTypes.AutoRemove, new Point((int)(LeaderInfo.LeaderPos.x * 100f), (int)(LeaderInfo.LeaderPos.z * 100f)), true, null, -1, -1, true, false);
						}
						else if (this.TalkCount == 5)
						{
							Global.CanGiveFakeEquips = false;
							Global.FakeEquipsIndex = 0;
							base.InvokeRepeating("ReplaceEquip", 0.4f, 0f);
						}
						else if (this.TalkCount == 6)
						{
							Global.Data.GameScene.SetXueSeChengBaoTeleportActive(true);
							Global.Data.roleData.IsFlashPlayer = 0;
							Global.CanGiveFakeEquips = false;
							this.CameraSwitch.SetPositions(Vector3.zero, Vector3.zero, new Vector3(84.5f, 62f, 43f), new Vector3(99.5f, 50.5f, 58.97f));
							this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.None, 0.1f, 0.1f, false, true, null);
						}
					}
				}
			}
		}
	}

	private void ReplaceEquip()
	{
		Global.Data.GameScene.ServerChangeCode(null);
		base.CancelInvoke("ReplaceEquip");
	}

	private void ClearGameObjects()
	{
		for (int i = 0; i < this.DisableGOs.Count; i++)
		{
			this.DisableGOs[i].SetActive(false);
		}
		this.DisableGOs.Clear();
	}

	public void SetHandVisible(bool state)
	{
		if (state)
		{
			if (!this.Hand.gameObject.activeInHierarchy)
			{
				NGUITools.SetActive(this.Hand, state);
			}
		}
		else if (this.Hand.gameObject.activeInHierarchy)
		{
			NGUITools.SetActive(this.Hand, state);
		}
	}

	public EventHandler StartTalk;

	public EventHandler CompleteTalk;

	public Transform BakTrans;

	public UISprite TopBak;

	public UISprite TopBorder;

	public UISprite BottomBak;

	public UISprite BottomBorder;

	public UISprite BottomSprite;

	public ShowNetImage NPCImage;

	public ShowNetImage RoleImage;

	public TextBlock NPCName;

	public TextBlock TalkText;

	public GameObject Hand;

	private int BakHeight = 100;

	public TextBlock log;

	private float FieldOfView_TargetCamera;

	private Vector3 TargetCamera_Position;

	private Quaternion TargetCamera_Quaternion;

	public ExtCallBackHandler ExtCallBack;

	private static PlayGameGuide ms_Singleton;

	private int TalkCount;

	private int _TaskPlotID;

	private int _Mode;

	private int _TimeParameters = -1;

	private List<TalkTextNode> TalkTextList;

	private int TalkTextIndex;

	private string _StartLuaFile = string.Empty;

	private string _EndLuaFile = string.Empty;

	private int PostWizardID = -1;

	private int PostOpenID = -1;

	private string _WaitingAppendText;

	private int _WaitingAppendTextIndex;

	public CamerasSwitch CameraSwitch;

	private GSprite _SelfSprite;

	private GSprite _TargetSprite;

	private bool DisableClosePart;

	private int StartTime;

	private List<GameObject> DisableGOs = new List<GameObject>();
}
