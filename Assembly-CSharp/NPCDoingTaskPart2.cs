using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class NPCDoingTaskPart2 : UserControl
{
	protected override void InitializeComponent()
	{
		this.BakTrans.localScale = Super.GetMainUISize();
		UIEventListener.Get(this.BakTrans.gameObject).onClick = delegate(GameObject s)
		{
			this.OnSubmit();
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
		this.Hand.transform.localPosition = new Vector3((float)((int)(num * 0.4f)), (float)(-(float)num2 / 2 + (int)((float)this.BakHeight * 0.6f)), -11f);
		this.NPCName.transform.localPosition = transform3.localPosition + new Vector3(20f, 75f, -10f);
		this.TalkText.transform.localPosition = transform3.localPosition + new Vector3(124f, 34f, -100f);
	}

	public int NpcID
	{
		get
		{
			return this._NpcID;
		}
		set
		{
			this._NpcID = value;
		}
	}

	public int NpcExtensionID
	{
		get
		{
			return this._NpcExtensionID;
		}
		set
		{
			this._NpcExtensionID = value;
		}
	}

	protected void OnDisable()
	{
		if (null != this.NPCImage)
		{
			this.NPCImage.DestroyImmediateTexture();
		}
	}

	private void OnSubmit()
	{
		if (this.DisableClosePart)
		{
			return;
		}
		if (this.StartTime != 0 && Global.GetMyTimer() - this.StartTime < 1000)
		{
			return;
		}
		Global.DisableInput = false;
		if (this.FieldOfView_TargetCamera != 0f)
		{
			this.CameraSwitch.TargetCamera.fieldOfView = this.FieldOfView_TargetCamera;
			this.CameraSwitch.TargetCamera.transform.localPosition = this.TargetCamera_Position;
			this.CameraSwitch.TargetCamera.transform.localRotation = this.TargetCamera_Quaternion;
			this.FieldOfView_TargetCamera = 0f;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, null);
		}
		if (this.TaskID == -1)
		{
			return;
		}
		if (this.mTaskState == NPCDoingTaskPart2.TaskStates.None)
		{
			GameInstance.Game.SpriteNewTask(this.NpcID, this.TaskID);
			SystemHintWindow.AddTaskGuidHintText(this.TaskID);
		}
		else if (this.mTaskState == NPCDoingTaskPart2.TaskStates.Accepted)
		{
			if (this.TargetType == 1)
			{
				GameInstance.Game.SpriteTransferSomething(this.TaskID);
			}
			else if (this.TargetType == 2)
			{
				Super.PrccessAutoTaskFindRoad(this.TaskID, false, true, false, false);
			}
			NPCDoingTaskPart2.TaskAutoRoad(this.TaskID);
		}
		else if (this.mTaskState == NPCDoingTaskPart2.TaskStates.Complete && this.TaskID != -1)
		{
			GameInstance.Game.SpriteCompleteTask(this.NpcID, this.TaskID, this.DbID, this.NeedYuanBao);
			if (this.NextMainTaskID > 0)
			{
				Super.AutoAcceptTask(this.NextMainTaskID, "SourceNPC");
			}
			this.HandleTalkEndEvents();
		}
	}

	public static void TaskAutoRoad(int taskID)
	{
		Super.PrccessAutoTaskFindRoad(taskID, false, true, false, false);
	}

	public void GetNewData(int taskID, bool newTask = true)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		this.TalkText.Text = string.Empty;
		this.NPCName.Text = string.Empty;
		this.NextMainTaskID = -1;
		this.TaskID = taskID;
		bool flag = false;
		this.TalkNpcID = -1;
		this.DisableClosePart = false;
		this.StartTime = 0;
		if (this.TaskID != -1)
		{
			this.TaskClass = Global.GetTaskClassByID(this.TaskID);
			this.NextMainTaskID = Super.GetNextMainTask(this.TaskID);
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (taskXmlNodeByID != null)
			{
				if (newTask)
				{
					this.mTaskState = NPCDoingTaskPart2.TaskStates.None;
					GameInstance.Game.SpriteGetTaskAwards(this.TaskID);
					List<TalkTextNode> taskTalkTextInfo = Super.GetTaskTalkTextInfo(this.TaskID, "AcceptTalk");
					this.TalkNpcID = this.SetTaskTalkText(taskTalkTextInfo, ref empty, ref empty2);
					this.NPCName.Text = empty;
					this.StartAppendText(empty2);
					Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务接取UI"), taskID, -1, 1);
					this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 1);
					this.SetState(false, this.TargetType);
				}
				else
				{
					this.mTaskState = NPCDoingTaskPart2.TaskStates.Accepted;
					TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(this.TaskID);
					if (taskDataByTaskID == null)
					{
						return;
					}
					this.DbID = taskDataByTaskID.DbID;
					if (Super.JugeTaskComplete(taskXmlNodeByID, Super.GetTaskDataByTaskID(taskID).DoingTaskVal1, Super.GetTaskDataByTaskID(taskID).DoingTaskVal2))
					{
						this.mTaskState = NPCDoingTaskPart2.TaskStates.Complete;
						List<TalkTextNode> taskTalkTextInfo2 = Super.GetTaskTalkTextInfo(this.TaskID, "CompleteTalk");
						this.TalkNpcID = this.SetTaskTalkText(taskTalkTextInfo2, ref empty, ref empty2);
						this.NPCName.Text = empty;
						this.StartAppendText(empty2);
						Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务提交UI"), taskID, 2, 1);
						this.SetState(true, 0);
						flag = true;
					}
					else
					{
						this.mTaskState = NPCDoingTaskPart2.TaskStates.Accepted;
						List<TalkTextNode> taskTalkTextInfo3 = Super.GetTaskTalkTextInfo(this.TaskID, "DoingTalk");
						this.TalkNpcID = this.SetTaskTalkText(taskTalkTextInfo3, ref empty, ref empty2);
						this.NPCName.Text = empty;
						this.StartAppendText(empty2);
						bool completed;
						if (!Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskDataByTaskID.DoingTaskVal1))
						{
							this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 1);
							completed = true;
						}
						else
						{
							this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 2);
							completed = false;
						}
						this.SetState(completed, this.TargetType);
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (this.TalkNpcID > 0 && this.TaskID < 103)
		{
			this.SwitchCamera(this.NpcID - 2130706432);
		}
		else
		{
			this.SwitchCamera(-1);
		}
		this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		this.HandleShowEvents(flag);
		Global.DisableInput = true;
	}

	protected void SetState(bool completed, int targetType)
	{
		int num = this.TaskID;
		if (completed)
		{
			num = this.NextMainTaskID;
		}
	}

	public int SetTaskTalkText(List<TalkTextNode> talkList, ref string name, ref string talkText)
	{
		int num = -1;
		for (int i = 0; i < talkList.Count; i++)
		{
			num = talkList[i].NpcID;
			if (num <= 0)
			{
				name = ColorCode.EncodingText(Global.FormatRoleName(Global.Data.roleData), "fffffe");
			}
			else
			{
				name = ColorCode.EncodingText(ConfigNPCs.GetNPCNameByID(num), "00ff00");
			}
			talkText = talkList[i].TalkText;
			this.NPCImage.gameObject.SetActive(num > 0);
			this.RoleImage.gameObject.SetActive(num <= 0);
			if (this.NPCImage.gameObject.activeSelf)
			{
				int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(num);
				string url = string.Format("NetImages/NPCs/{0}.png", Global.FormatStr("000", npcpicCodeByID));
				this.NPCImage.URL = url;
			}
			if (this.RoleImage.gameObject.activeSelf)
			{
				string url2 = string.Format("RolePics/{0}{1}.png", Global.Data.roleData.Occupation, Global.Data.roleData.RoleSex);
				this.RoleImage.URL = url2;
			}
		}
		return num;
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
			this.StopAppendText();
			return;
		}
		TextBlock talkText = this.TalkText;
		talkText.Text += this._WaitingAppendText.get_Chars(this._WaitingAppendTextIndex);
		this._WaitingAppendTextIndex++;
	}

	public void SwitchCamera(int npcID)
	{
		Vector3 zero = Vector3.zero;
		this._TargetSprite = ((npcID <= 0) ? Global.FindSprite("Leader") : Global.FindSprite(Global.GetNPCName(npcID)));
		Transform targetTrans = Global.GetTargetTrans(this._TargetSprite, "TargetCamera", out zero, null);
		if (null == targetTrans)
		{
			return;
		}
		float extSubHeight = 0f;
		if (npcID == 60900)
		{
			extSubHeight = 0.5f;
		}
		this.CameraSwitch.SetTargetTransform(targetTrans, zero, 0.8f, 0.75f, 3f, extSubHeight);
		GSprite gsprite = Global.FindSprite("Leader");
		if (gsprite != null)
		{
			gsprite.ModifyLayers("TargetCamera", null);
		}
	}

	public void ReturnCamera()
	{
		this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.ScenceMode, CamerasSwitch.CamerasType.MainCamera, CamerasSwitch.TweenOfAfterPushMode.None, 3f, 1f, false, false, null);
		if (this._TargetSprite != null)
		{
			this._TargetSprite.ModifyLayers("Sprites", null);
			this._TargetSprite = null;
		}
		GSprite gsprite = Global.FindSprite("Leader");
		if (gsprite != null)
		{
			gsprite.ModifyLayers("Sprites", null);
		}
	}

	private void HandleTalkEndEvents()
	{
	}

	private void HandleShowEvents(bool completed)
	{
		if (this.TaskID == 100)
		{
			CameraShake.DisableCameraShake = true;
			UICtrlBar.singleton.CancelAutoUseSkill();
		}
		else if (this.TaskID == 101)
		{
			this.DisableClosePart = true;
			this.StartTime = Global.GetMyTimer();
			this.SetHandVisible(false);
			Super.ActiveModalLayer(true);
			GameObject gameObject = Global.Data.GameScene.AddSpriteEffect(Global.FindSprite("Leader"), null);
			if (null != gameObject)
			{
				GSprite gsprite = Global.FindSprite("Leader");
				if (gsprite != null)
				{
					base.InvokeRepeating("ReplaceEquips", 3f, 100f);
				}
			}
			this.ReturnCamera();
		}
		else if (this.TaskID == 102)
		{
			SystemHelpMgr.SetTargetGridEventHandler(80, 62, 2, 3, delegate(object s, EventArgs e)
			{
				int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
				GongnengYugaoMgr.ReadyFlyingImgAnimation(PlayZone.GlobalPlayZone, 1000 + num * 100 + 1);
			});
			UICtrlBar.singleton.CancelAutoUseSkill();
			this.DisableClosePart = true;
			this.StartTime = Global.GetMyTimer();
			this.SetHandVisible(false);
			GameObject gameObject2 = Global.Data.GameScene.AddDiaoQiaoEffect(delegate(object s, EventArgs e)
			{
				Global.Data.GameScene.AddYinDaoZhiYin(new Vector2(51f, 62.5f), new Vector2(80f, 62.5f));
				this.DisableClosePart = false;
				this.SetHandVisible(true);
			});
			Global.Data.GameScene.SetGuangMuState(21, 0, false, -1);
			Global.Data.GameScene.SetGuangMuState(22, 2, false, -1);
			if (null != gameObject2)
			{
				this.CameraSwitch.SetPositions(Vector3.zero, Vector3.zero, new Vector3(62f, 62f, 47f), new Vector3(76.94f, 50.5f, 62.97f));
				this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.None, 0.1f, 0.1f, false, false, null);
			}
		}
		else if (this.TaskID == 103)
		{
			Global.Data.GameScene.AddYinDaoZhiYin(new Vector2(80f, 62.5f), new Vector2(98f, 62.5f));
			UICtrlBar.singleton.CancelAutoUseSkill();
			this.StartTime = Global.GetMyTimer();
			this.CameraSwitch.SetPositions(Vector3.zero, Vector3.zero, new Vector3(74f, 62f, 54f), new Vector3(83f, 50.5f, 63f));
			this.CameraSwitch.SwitchCamera(CamerasSwitch.CamerasMode.NpcMode, CamerasSwitch.CamerasType.TargetCamera, CamerasSwitch.TweenOfAfterPushMode.None, 0.1f, 0.1f, false, true, null);
			SystemHelpMgr.SetTargetGridEventHandler(98, 62, 5, 5, delegate(object s, EventArgs e)
			{
				int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
				GongnengYugaoMgr.ReadyFlyingImgAnimation(PlayZone.GlobalPlayZone, 1000 + num * 100 + 2);
			});
			Global.Data.GameScene.SetGuangMuState(3, 0, false, -1);
		}
		else if (this.TaskID == 104)
		{
			UICtrlBar.singleton.CancelAutoUseSkill();
			UICtrlBar.singleton.ShowHelpAnim(-1, 0);
			this.DisableClosePart = true;
			this.StartTime = Global.GetMyTimer();
			this.SetHandVisible(false);
			GameObject gameObject3 = U3DUtils.FindGameObjectByName(null, "/Sprites/19_xscb_common_shuijinguancai_001");
			if (null != gameObject3)
			{
				this.DisableClosePart = false;
				this.SetHandVisible(true);
				Global.Data.GameScene.AddShiGuanEffect(null);
			}
		}
		else if (this.TaskID == 105)
		{
			UICtrlBar.singleton.CancelAutoUseSkill();
		}
		else if (this.TaskID == 106)
		{
			Global.Data.GameScene.AddXueSeChengBaoZuDang0();
			Global.Data.GameScene.RemoveYinDaoZhiYin();
			CameraShake.DisableCameraShake = false;
		}
	}

	private void ReplaceEquips()
	{
		GSprite gsprite = Global.FindSprite("Leader");
		if (gsprite != null)
		{
			Global.CanGiveFakeEquips = true;
			Global.FakeEquipsIndex = 0;
			Global.Data.GameScene.ServerChangeCode(null);
			gsprite = Global.FindSprite("Leader");
			if (gsprite != null)
			{
				gsprite.ModifyLayers("TargetCamera", null);
			}
			UICtrlBar.singleton.AddTempSkill(0, 1);
		}
		base.CancelInvoke("ReplaceEquips");
		this.DisableClosePart = false;
		this.SetHandVisible(true);
		Super.ActiveModalLayer(false);
		this.OnSubmit();
		this.DelayInvoke("ShowTaskDiagram101", 3f);
	}

	private void DelayInvoke(string functionName, float delay)
	{
		base.InvokeRepeating(functionName, delay, 0f);
	}

	private void ShowTaskDiagram101()
	{
		base.CancelInvoke("ShowTaskDiagram101");
		SystemHelpPart.ShowDiagram(Global.GetGameResImageString("Diagram000.jpg"), 101, true);
	}

	private void DiaoQiaoBaoZha()
	{
		GameObject gameObject = Global.Data.GameScene.AddDiaoQiaoBaoZhaEffect(null);
		GameObject gameObject2 = U3DUtils.FindGameObjectByName(null, "/Sence/Model/animation/19_xscb_common_diaoqiao_001");
		if (null != gameObject2)
		{
			gameObject2.SetActive(false);
		}
		base.CancelInvoke("DiaoQiaoBaoZha");
		this.DisableClosePart = false;
		this.SetHandVisible(true);
	}

	public void BurstDecorationDestroyNotify(object sender, EventArgs e)
	{
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

	private NPCDoingTaskPart2.TaskStates mTaskState;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int TaskID = -1;

	private int TaskClass = -1;

	private int TargetType = -1;

	private int NextMainTaskID = -1;

	private int DbID = -1;

	private int NeedYuanBao;

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

	private float FieldOfView_TargetCamera;

	private Vector3 TargetCamera_Position;

	private Quaternion TargetCamera_Quaternion;

	private int _NpcID;

	private int _NpcExtensionID;

	private int TalkNpcID = -1;

	private string _WaitingAppendText;

	private int _WaitingAppendTextIndex;

	public CamerasSwitch CameraSwitch;

	private GSprite _TargetSprite;

	private bool DisableClosePart;

	private int StartTime;

	internal enum TaskStates
	{
		None,
		Accepted,
		Faild,
		Complete
	}
}
