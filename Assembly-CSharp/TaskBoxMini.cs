using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TaskBoxMini : UserControl
{
	private GameObject MainTaskInfoObj
	{
		get
		{
			return this.MainTaskInfo.transform.parent.gameObject;
		}
	}

	private GameObject OtherTaskInfoObj
	{
		get
		{
			return this.OtherTaskInfo.transform.parent.gameObject;
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public bool IsShowLblTeamCompeteGuanZhan
	{
		set
		{
			NGUITools.SetActive(this.mLblTeamCompeteGuanZhan.gameObject, value);
		}
	}

	private void InitTextInPrefabs()
	{
		this.NextIcon.Text = Global.GetLang("下一层");
		this.staticText[0].text = Global.GetLang("人数");
		this.staticText[1].text = Global.GetLang("分数");
		this.staticText[2].text = Global.GetLang("阵营");
		this.mLblTeamCompeteGuanZhan.Text = Global.GetLang("战斗未结束退出战场将不能获得个人奖励");
		this.btnRenwu.Text = Global.GetLang("任\n务");
		this.btnTeam.Text = Global.GetLang("组\n队");
		this.taofaButton.Text = Global.GetLang("讨 伐");
		this.richangButton.Text = Global.GetLang("日 常");
		this.staticText[3].text = Global.GetLang("我的积分");
		this.staticText[4].text = Global.GetLang("我的积分");
	}

	private void ChangeText(int State)
	{
		this.OldTaskInfo.text = this.OtherTaskInfo.text;
		if (State == 0)
		{
			TweenPosition component = this.OldTaskInfo.gameObject.GetComponent<TweenPosition>();
			if (null != component)
			{
				component.gameObject.SetActive(true);
				component.from = TaskBoxMini.TextPosMiddle;
				component.to = TaskBoxMini.TextPosRight;
				component.Reset();
				component.Play(true);
			}
			component = this.OtherTaskInfo.gameObject.GetComponent<TweenPosition>();
			if (null != component)
			{
				component.gameObject.SetActive(true);
				component.from = TaskBoxMini.TextPosLeft;
				component.to = TaskBoxMini.TextPosMiddle;
				component.Reset();
				component.Play(true);
			}
		}
		else
		{
			TweenPosition component = this.OldTaskInfo.gameObject.GetComponent<TweenPosition>();
			if (null != component)
			{
				component.gameObject.SetActive(true);
				component.Reset();
				component.Play(true);
				component.from = TaskBoxMini.TextPosMiddle;
				component.to = TaskBoxMini.TextPosLeft;
			}
			component = this.OtherTaskInfo.gameObject.GetComponent<TweenPosition>();
			if (null != component)
			{
				component.gameObject.SetActive(true);
				component.Reset();
				component.Play(true);
				component.from = TaskBoxMini.TextPosRight;
				component.to = TaskBoxMini.TextPosMiddle;
			}
		}
	}

	private void SetHandEffectPos()
	{
		if (this.mTaskPanel.transform.localPosition.y > 20f)
		{
			base.StopCoroutine("ShowArrowDelay");
			this.AnimTaskHand.gameObject.SetActive(false);
		}
	}

	protected override void InitializeComponent()
	{
		this.AnimMainTaskRectParent = this.MainGoIcon.transform.parent.gameObject;
		this.AnimMainTaskRect.transform.SetParent(this.AnimMainTaskRectParent.transform);
		this.ShowSelectedTeXiao(this.AnimMainTaskRectParent);
		this.ItemCollection = this.mTaskListBox.ItemsSource;
		this.mTaskListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ClickOtherTaskItem);
		this.mTaskDraggablePanel.onDragIng = delegate()
		{
			this.ShowEffect(false);
		};
		this.mTaskDraggablePanel.onDragFinished = delegate()
		{
			this.ShowEffect(true);
			this.SetHandEffectPos();
		};
		this.SwitchIconBak = this.SwitchIcon.GetComponentInChildren<UISprite>();
		this.SwitchIcon1.transform.localPosition = new Vector3(180f, 11f, -0.5f);
		this.TitleBack.transform.localScale = new Vector3(205f, 30f, 1f);
		this.SceneInfosTitle.transform.localPosition = new Vector3(89f, 35f, -0.01f);
		this.InitTextInPrefabs();
		this.shiLiTaskWindow.OnSelectTask = new Action<TaskData>(this.OnClickShiLiTask);
		this.GoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		this.MainGoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ButtonClick(TaskClasses.Main, null, this.MainGoIcon.transform.parent.gameObject);
		};
		this.OtherGoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ButtonClick((TaskClasses)this._ActiveTaskClass, null, this.OtherGoIcon.transform.parent.gameObject);
		};
		this.btnTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).OnDealTeamBoxClick();
		};
		this.arrow.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.arrowDisplay = !this.arrowDisplay;
			this.taofaButton.gameObject.SetActive(this.arrowDisplay);
			this.richangButton.gameObject.SetActive(this.arrowDisplay);
			if (this._ActiveTaskClass == 9)
			{
				this.taofaButton.Pressed = true;
				this.richangButton.Pressed = false;
			}
			else
			{
				this.richangButton.Pressed = true;
				this.taofaButton.Pressed = false;
			}
			if (!this.arrow.Pressed)
			{
				this.arrow.Pressed = true;
			}
			else
			{
				this.arrow.Pressed = false;
			}
		};
		this.taofaButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._ActiveTaskClass = 9;
			this.RefreshTasks(-1, -1, true);
			this.taofaButton.gameObject.SetActive(false);
			this.richangButton.gameObject.SetActive(false);
			this.taofaButton.Pressed = false;
			this.richangButton.Pressed = false;
			this.arrow.Pressed = false;
			this.arrowDisplay = false;
		};
		this.richangButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._ActiveTaskClass = 8;
			this.RefreshTasks(-1, -1, true);
			this.taofaButton.gameObject.SetActive(false);
			this.richangButton.gameObject.SetActive(false);
			this.taofaButton.Pressed = false;
			this.richangButton.Pressed = false;
			this.arrow.Pressed = false;
			this.arrowDisplay = false;
		};
		UIEventListener.Get(this.OtherGoIcon.gameObject).onDrag = delegate(GameObject go, Vector2 delta)
		{
			if (this.dragEnd)
			{
				if (delta.x > 0f)
				{
					this.ChangeText(0);
				}
				else
				{
					this.ChangeText(1);
				}
				if (this._ActiveTaskClass == 9)
				{
					this._ActiveTaskClass = 8;
				}
				else
				{
					this._ActiveTaskClass = 9;
				}
				this.RefreshTasks(-1, -1, true);
				this.dragEnd = false;
			}
		};
		this.OtherGoIcon.OnPress = delegate(GameObject go, bool state)
		{
			this.dragEnd = true;
		};
		UIEventListener.Get(this.SwitchIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				this._RebirthBossTaskScene.ShowRankInf = !this._RebirthBossTaskScene.ShowRankInf;
			}
			else
			{
				this.BodyVisible = !this.BodyVisible;
			}
			Transform transform = this.SwitchIcon.transform;
			this.m_NormalSceneTaskBtnIsShow = this.BodyVisible;
			UISprite componentInChildren = this.SwitchIcon.gameObject.GetComponentInChildren<UISprite>();
			if (!this.BodyVisible)
			{
				componentInChildren.spriteName = "Taskarrow_02";
			}
			else
			{
				componentInChildren.spriteName = "Taskarrow1";
			}
		};
		UIEventListener.Get(this.SwitchIcon1.gameObject).onClick = delegate(GameObject s)
		{
			this.BodyVisible = !this.BodyVisible;
			Transform transform = this.SwitchIcon1.transform;
			this.m_OtherSceneTaskBtnIsShow = this.BodyVisible;
			if (!this.BodyVisible)
			{
				this.SwitchIconTrans1.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
				transform.localPosition = new Vector3(0f, transform.localPosition.y, transform.localPosition.z);
			}
			else
			{
				this.SwitchIconTrans1.localRotation = Quaternion.Euler(Vector3.zero);
				float num = 180f;
				if (Global.GetMapSceneUIClass() == SceneUIClasses.ZhengDuoZhiDi)
				{
					num = 275f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
				{
					num = 247f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
				{
					num = 225f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.WanMoXiaGu)
				{
					num = 234f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattleMiDong)
				{
					num = 200f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
				{
					num = 240f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu)
				{
					num = 300f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.JingYanFuBen || Global.GetMapSceneUIClass() == SceneUIClasses.JinBiFuBen)
				{
					num = 205f;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.ShiLian)
				{
					num = 300f;
				}
				transform.localPosition = new Vector3(num, transform.localPosition.y, transform.localPosition.z);
			}
		};
		this.NewTaskAnim.transform.localPosition = TaskBoxMini.AnimPosFrom;
		this.NextIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.StartUITimer();
		if (this.mTaskListBox.transform.childCount > 0)
		{
			for (int i = 0; i < this.mTaskListBox.transform.childCount; i++)
			{
				if (this.mTaskListBox.transform.GetChild(i).name == "MainTaskInfoObj")
				{
					this.ItemCollection.AddNoUpdate(this.mTaskListBox.transform.GetChild(i).gameObject);
				}
			}
		}
		this.ShowEffect(true);
		this.ShowOtherTaskItem(true, -1, null, -1, -1, true);
		if (!Global.IsInKuaFuTeamCompete())
		{
			this.IsShowLblTeamCompeteGuanZhan = false;
		}
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

	public bool AllVisible
	{
		get
		{
			return this.Visibility;
		}
		set
		{
			this.Visibility = value;
		}
	}

	public void FormatTaskInfo(string str)
	{
	}

	private void OnClickShiLiTask(TaskData taskData)
	{
		this.ShiLiTaskClick(taskData);
	}

	public void ShowAnim(bool isVisible)
	{
		if (isVisible)
		{
			this._Anim.gameObject.SetActive(true);
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.HideTaskBoxJianTou))
			{
				this.AnimTaskHand.gameObject.SetActive(false);
			}
			else
			{
				GameObject gameObject = null;
				if (this.ItemCollection.Count > 0)
				{
					for (int i = 0; i < this.ItemCollection.Count; i++)
					{
						if (this.ItemCollection.GetAt(i).name == "MainTaskInfoObj" && this.ItemCollection.GetAt(i).activeInHierarchy)
						{
							gameObject = this.ItemCollection.GetAt(i);
							break;
						}
					}
				}
				if (gameObject != null)
				{
				}
			}
		}
		else
		{
			this._Anim.gameObject.SetActive(false);
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (id == 1)
		{
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.HideTaskBoxJianTou))
			{
				GameObject gameObject = null;
				if (this.ItemCollection.Count > 0)
				{
					for (int i = 0; i < this.ItemCollection.Count; i++)
					{
						if (this.ItemCollection.GetAt(i).name == "MainTaskInfoObj" && this.ItemCollection.GetAt(i).activeInHierarchy)
						{
							gameObject = this.ItemCollection.GetAt(i);
							break;
						}
					}
				}
				if (gameObject != null)
				{
				}
			}
		}
		else if (id == 2)
		{
			GameObject gameObject2 = null;
			if (this.ItemCollection.Count > 0)
			{
				for (int j = 0; j < this.ItemCollection.Count; j++)
				{
					if (this.ItemCollection.GetAt(j).name == "MainTaskInfoObj" && this.ItemCollection.GetAt(j).activeInHierarchy)
					{
						gameObject2 = this.ItemCollection.GetAt(j);
						break;
					}
				}
			}
			if (gameObject2 != null)
			{
			}
		}
		else if (state > 0)
		{
			if (id == 0)
			{
				OtherTaskInfoItem otherTaskInfoItem = null;
				if (this.ItemCollection.Count > 0)
				{
					for (int k = 0; k < this.ItemCollection.Count; k++)
					{
						if (this.ItemCollection.GetAt(k).name == "DailyTaskItem" && this.ItemCollection.GetAt(k).activeInHierarchy)
						{
							otherTaskInfoItem = this.ItemCollection.GetAt(k).GetComponent<OtherTaskInfoItem>();
							break;
						}
					}
				}
				if (otherTaskInfoItem != null)
				{
					SystemHelpPart.SetMask(otherTaskInfoItem, default(Vector4));
				}
			}
			else if (id == 1000)
			{
				SystemHelpPart.SetMask(this.OtherGoIcon, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public void AutoAcceptTask(int taskID)
	{
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
		if (taskXmlNodeByID == null)
		{
			return;
		}
		int sourceNPC = taskXmlNodeByID.SourceNPC;
		if (sourceNPC <= 0)
		{
			return;
		}
		int npcID = 2130706432 + sourceNPC;
		GameInstance.Game.SpriteNewTask(npcID, taskID);
		SystemHintWindow.AddTaskGuidHintText(taskID);
	}

	public void AutoAcceptTasks(int oldLevel)
	{
		if (!this.LevelLimited)
		{
			this.RefreshTasks(-1, -1, true);
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AutoTaskIDs", ',');
		int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("AutoTaskLevels", ',');
		if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length <= 0)
		{
			return;
		}
		if (systemParamIntArrayByName2 == null || systemParamIntArrayByName2.Length <= 0)
		{
			return;
		}
		if (systemParamIntArrayByName2.Length != systemParamIntArrayByName.Length)
		{
			return;
		}
		for (int i = 0; i < systemParamIntArrayByName.Length; i++)
		{
			if (oldLevel < systemParamIntArrayByName2[i] && Global.Data.roleData.Level >= systemParamIntArrayByName2[i])
			{
				this.AutoAcceptTask(systemParamIntArrayByName[i]);
			}
		}
	}

	private void ResizeBak()
	{
		if (this.LevelLimited)
		{
			this.ShowHelpAnim(2, 1);
		}
		else
		{
			this.ShowHelpAnim(2, 0);
		}
		if (this.TaskID == 100 || this.TaskID == 1000 || this.TaskID == 1020)
		{
			this.ShowHelpAnim(1, 1);
		}
		else
		{
			this.ShowHelpAnim(1, 0);
		}
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.HideTaskBoxJianTou))
		{
			GameObject gameObject = null;
			if (this.ItemCollection.Count > 0)
			{
				for (int i = 0; i < this.ItemCollection.Count; i++)
				{
					if (this.ItemCollection.GetAt(i).name == "MainTaskInfoObj" && this.ItemCollection.GetAt(i).activeInHierarchy)
					{
						gameObject = this.ItemCollection.GetAt(i);
						break;
					}
				}
			}
			if (gameObject != null)
			{
			}
		}
	}

	public void RefreshTasks(int newTaskID = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		if ((triggerType == 1 || triggerType == 2) && this.mTaskDraggablePanel != null)
		{
			this.mTaskDraggablePanel.ResetPosition();
		}
		if (Global.Data.roleData != null && Global.Data.roleData.TaskDataList != null && Global.Data.roleData.TaskDataList.Count > 3)
		{
			this.mTaskDraggablePanel.scale = new Vector3(0f, 1f, 0f);
		}
		else
		{
			this.mTaskDraggablePanel.scale = new Vector3(0f, 0f, 0f);
		}
		TaskData taskData = null;
		TaskData taskData2 = null;
		TaskData taskData3 = null;
		if (newTaskID < 0)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				this.AddWaitingTask(TaskClasses.Main);
			}
			Super.FindHavingMainTask(0, out taskData);
			if (taskData == null)
			{
				this.AddWaitingTask(TaskClasses.Main);
			}
			this.DailyTask(out taskData2, newTaskID, triggerType, BshowDialyTaskAertPatr);
		}
		else
		{
			if (Global.Data.roleData != null && Global.Data.roleData.TaskDataList != null && Global.Data.roleData.TaskDataList.Count == 1)
			{
				if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.TaskClass == 0) != null)
				{
					this.DailyTask(out taskData2, newTaskID, triggerType, BshowDialyTaskAertPatr);
				}
			}
			TaskData taskDataByID = Global.GetTaskDataByID(newTaskID);
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(newTaskID);
			if (taskXmlNodeByID == null)
			{
				Debug.Log(Global.GetLang("出错 错误任务ID"));
				return;
			}
			if (taskXmlNodeByID.TaskClass == 0)
			{
				taskData = taskDataByID;
				if (taskData != null)
				{
					taskData.TaskClass = 0;
				}
			}
			else if (taskXmlNodeByID.TaskClass == 8)
			{
				taskData2 = taskDataByID;
				if (taskData2 != null)
				{
					taskData2.TaskClass = 8;
				}
			}
			else if (taskXmlNodeByID.TaskClass == 9)
			{
				taskData2 = taskDataByID;
				if (taskData2 != null)
				{
					taskData2.TaskClass = 9;
				}
			}
			else if (taskXmlNodeByID.TaskClass == 1)
			{
				taskData2 = taskDataByID;
				if (taskData2 != null)
				{
					taskData2.TaskClass = 1;
				}
			}
			else
			{
				if (!ShiLiData.BeShiLiTask(taskXmlNodeByID.TaskClass))
				{
					return;
				}
				taskData3 = taskDataByID;
				if (taskData3 != null)
				{
				}
			}
		}
		if (taskData != null)
		{
			this.TaskCompletedEX = false;
			this.RoadMainTaskID = taskData.DoingTaskID;
			TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			string title = taskXmlNodeByID2.Title;
			int taskClass = taskXmlNodeByID2.TaskClass;
			string taskClassName = Super.GetTaskClassName(taskClass);
			bool flag = Super.JugeTaskTargetComplete(taskXmlNodeByID2, 1, taskData.DoingTaskVal1);
			bool flag2 = Super.JugeTaskTargetComplete(taskXmlNodeByID2, 2, taskData.DoingTaskVal2);
			string text = string.Empty;
			int limitLevel = taskXmlNodeByID2.LimitLevel;
			int limitZhuanSheng = taskXmlNodeByID2.LimitZhuanSheng;
			if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"fd010c",
					string.Format(Global.GetLang("需要等级达到{0}\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng))
				});
				this.ShowEffect(false);
				this.LevelLimited = false;
			}
			else
			{
				this.LevelLimited = true;
			}
			bool flag3 = true;
			if (!flag || !flag2)
			{
				bool flag4 = false;
				int taketime = taskXmlNodeByID2.Taketime;
				if (taketime > 0 && Global.GetCorrectLocalTime() - taskData.AddDateTime >= (long)(taketime * 1000))
				{
					flag4 = true;
					text = StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
					{
						title,
						Global.GetColorStringForNGUIText(new object[]
						{
							"FF0000",
							Global.GetLang("【失败】")
						})
					});
				}
				string pubStartTime = taskXmlNodeByID2.PubStartTime;
				string pubEndTime = taskXmlNodeByID2.PubEndTime;
				if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
				{
					double num = (double)Global.GetCorrectLocalTime();
					double num2 = (double)Global.SafeConvertToTicks(pubStartTime);
					double num3 = (double)Global.SafeConvertToTicks(pubEndTime);
					if (num < num2 || num > num3)
					{
						flag4 = true;
						text = StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
						{
							title,
							Global.GetColorStringForNGUIText(new object[]
							{
								"FF0000",
								Global.GetLang("【失败】")
							})
						});
					}
				}
				if (flag4)
				{
					this.MainTaskInfo.Text = text;
					flag3 = false;
				}
			}
			if (flag3)
			{
				if (!flag)
				{
					string taskInfoPartStr = this.GetTaskInfoPartStr(taskData, taskXmlNodeByID2, 1);
					if (!this.LevelLimited && !string.IsNullOrEmpty(taskXmlNodeByID2.LevelYuGao))
					{
						text += taskXmlNodeByID2.LevelYuGao;
					}
					else
					{
						text += taskInfoPartStr;
					}
				}
				else if (!flag2)
				{
					string taskInfoPartStr2 = this.GetTaskInfoPartStr(taskData, taskXmlNodeByID2, 2);
					if (!this.LevelLimited && !string.IsNullOrEmpty(taskXmlNodeByID2.LevelYuGao))
					{
						text += taskXmlNodeByID2.LevelYuGao;
					}
					else
					{
						text += taskInfoPartStr2;
					}
				}
				else
				{
					this.TaskCompletedEX = true;
					this.TaskCompleted = true;
					if (triggerType == 1)
					{
						this.OnTaskOK(TaskClasses.Main);
					}
					text += Super.GetTaskDestNPCDesc(taskXmlNodeByID2, true);
					if (text.Length > 0)
					{
						string taskDestNPCName = Super.GetTaskDestNPCName(taskXmlNodeByID2);
						if (string.Empty != taskDestNPCName)
						{
							int mapCode = -1;
							int targetType = -1;
							int targetID = -1;
							Super.GetTaskDestNPCID(taskXmlNodeByID2, ref mapCode, ref targetType, ref targetID);
							this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, mapCode, -1, -1);
						}
					}
					if (Global.Data.roleData.IsFlashPlayer >= 1 && 101 <= taskData.DoingTaskID && taskData.DoingTaskID <= 105 && !PlayGameGuide.singleton.Visibility)
					{
						this.BodyClick(null);
					}
				}
				this.SwitchToTask(this.TaskID, text, triggerType, TaskClasses.Main);
				if (PlayZone.GlobalPlayZone != null)
				{
					PlayZone.GlobalPlayZone.ShowGongNeItem(taskData.DoingTaskID, false);
				}
				this.OldMainTaskInfoStr = this.MainTaskInfo.Text;
			}
		}
		if (taskData2 != null)
		{
			if (newTaskID == -1)
			{
				this.AddOtherTaskInfoItem(taskData2, newTaskID, taskData2.TaskClass, triggerType, BshowDialyTaskAertPatr);
			}
			TaskVO taskXmlNodeByID3 = ConfigTasks.GetTaskXmlNodeByID(newTaskID);
			if (taskXmlNodeByID3 != null)
			{
				this.AddOtherTaskInfoItem(taskData2, newTaskID, taskXmlNodeByID3.TaskClass, triggerType, BshowDialyTaskAertPatr);
			}
		}
		else
		{
			TaskVO taskXmlNodeByID4 = ConfigTasks.GetTaskXmlNodeByID(newTaskID);
			if (taskXmlNodeByID4 == null)
			{
				this.ShowOtherTaskItem(true, -1, null, -1, -1, true);
			}
			else
			{
				this.ShowOtherTaskItem(true, newTaskID, null, taskXmlNodeByID4.TaskClass, triggerType, true);
			}
		}
		if (Global.IsInShiLiZhengBaMap() && this.shiLiTaskWindow != null)
		{
			if (newTaskID < 0)
			{
				this.shiLiTaskWindow.RefreshData();
			}
			else if (taskData3 != null)
			{
				if (triggerType == 0)
				{
					this.shiLiTaskWindow.AddNewTask(taskData3);
				}
				else if (triggerType != -1 && Global.Data.roleData.TaskDataList.IndexOf(taskData3) < 0)
				{
					this.shiLiTaskWindow.RefreshData();
				}
				else
				{
					this.shiLiTaskWindow.RefreshTask(taskData3);
				}
			}
			else
			{
				this.shiLiTaskWindow.RefreshData();
			}
		}
		if (Global.IsInShiLiZhengBaBattleMap())
		{
		}
		this.ResizeBak();
	}

	private void DailyTask(out TaskData tmpTaskOtherData, int taskID = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		tmpTaskOtherData = null;
		TaskData taskData = null;
		string text;
		int num;
		int num2;
		if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.RiChangRenWu, false, out text) && Global.CanDoPaoHuanTask(8, out num, out num2))
		{
			if (num <= num2)
			{
				tmpTaskOtherData = taskData;
				Super.FindHavingMainTask(this._ActiveTaskClass, out taskData);
				if (taskData == null)
				{
					if (Global.Data.roleData != null && Global.Data.roleData.TaskDataList != null)
					{
						Global.Data.roleData.TaskDataList.RemoveAll((TaskData result) => result.TaskClass == 8 && result.SpecialID > 0);
						if (Global.Data.roleData.TaskDataList != null)
						{
							if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.DoingTaskID == -1 && result.TaskClass == 8) == null && this.DailyFakeTaskData != null)
							{
								Global.Data.roleData.TaskDataList.Add(this.DailyFakeTaskData);
								tmpTaskOtherData = this.DailyFakeTaskData;
							}
						}
					}
				}
				else
				{
					this.AddOtherTaskInfoItem(taskData, taskID, 8, triggerType, BshowDialyTaskAertPatr);
				}
			}
		}
		else if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.JingYanFuBen, false, out text) && Global.CanDoJingYanFuBen() && (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.RiChangRenWu, false, out text) || !Global.CanDoPaoHuanTask(8, out num, out num2)))
		{
			if (Global.Data.roleData != null && Global.Data.roleData.TaskDataList != null)
			{
				if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.TaskClass == 8 && result.DoingTaskID > 0 && !result.IsComplete) != null)
				{
					return;
				}
				Global.Data.roleData.TaskDataList.RemoveAll((TaskData result) => result.TaskClass == 8);
				if (Global.Data.roleData.TaskDataList != null)
				{
					if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.DoingTaskID == -1 && result.TaskClass == 8) == null)
					{
						TaskData jingYanFuBenTaskData = this.JingYanFuBenTaskData;
						if (jingYanFuBenTaskData != null)
						{
							Global.Data.roleData.TaskDataList.Add(jingYanFuBenTaskData);
							tmpTaskOtherData = jingYanFuBenTaskData;
						}
					}
				}
			}
		}
		else if (UIHelper.IsGongNengOpenedOrHint(GongNengIDs.MeiRiBiZuo, false, out text) && (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.RiChangRenWu, false, out text) || !Global.CanDoPaoHuanTask(8, out num, out num2)) && Global.Data.roleData != null && Global.Data.roleData.TaskDataList != null)
		{
			if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.TaskClass == 8 && result.DoingTaskID > 0 && !result.IsComplete) != null)
			{
				return;
			}
			Global.Data.roleData.TaskDataList.RemoveAll((TaskData result) => result.TaskClass == 8);
			if (Global.Data.roleData.TaskDataList != null)
			{
				if (Global.Data.roleData.TaskDataList.Find((TaskData result) => result.DoingTaskID == -1 && result.TaskClass == 8) == null)
				{
					TaskData meiRiBiZuoTaskData = this.MeiRiBiZuoTaskData;
					if (meiRiBiZuoTaskData != null)
					{
						Global.Data.roleData.TaskDataList.Add(meiRiBiZuoTaskData);
						tmpTaskOtherData = meiRiBiZuoTaskData;
					}
				}
			}
		}
	}

	private void TaoFaTask(int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
	}

	private TaskData DailyFakeTaskData
	{
		get
		{
			return new TaskData
			{
				DoingTaskID = -1,
				IsComplete = true,
				RoadOtherTaskId = 0,
				TaskClass = 8
			};
		}
	}

	private TaskData JingYanFuBenTaskData
	{
		get
		{
			return new TaskData
			{
				DoingTaskID = -1,
				IsComplete = true,
				RoadOtherTaskId = 0,
				specialRiChangTask = true,
				TaskClass = 8,
				zhiXianLinkID = 100,
				SpecialID = 1
			};
		}
	}

	private TaskData MeiRiBiZuoTaskData
	{
		get
		{
			return new TaskData
			{
				DoingTaskID = -1,
				IsComplete = true,
				RoadOtherTaskId = 0,
				specialRiChangTask = true,
				TaskClass = 8,
				zhiXianLinkID = 1523,
				SpecialID = 2
			};
		}
	}

	private TaskData TaoFaFakeTaskData
	{
		get
		{
			return new TaskData
			{
				DoingTaskID = -1,
				IsComplete = true,
				RoadOtherTaskId = 0,
				TaskClass = 9
			};
		}
	}

	private void AddOtherTaskInfoItem(TaskData otherTaskData, int taskID = -1, int taskType = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		this.ShowOtherTaskItem(true, taskID, otherTaskData, taskType, triggerType, BshowDialyTaskAertPatr);
	}

	private void SetTaskListTaskClass()
	{
		if (Global.Data.roleData.TaskDataList != null && Global.Data.roleData.TaskDataList.Count > 0)
		{
			List<TaskData> taskDataList = Global.Data.roleData.TaskDataList;
			if (taskDataList != null)
			{
				for (int i = 0; i < taskDataList.Count; i++)
				{
					TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskDataList[i].DoingTaskID);
					if (taskXmlNodeByID != null)
					{
						taskDataList[i].TaskClass = taskXmlNodeByID.TaskClass;
					}
				}
			}
		}
	}

	private string GetSpecialDailyTaskDes(int id)
	{
		string text = null;
		string text2 = null;
		if (id != 1)
		{
			if (id == 2)
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("每日必做")
				});
				text2 = Global.GetLang("点击前往每日必做，获得丰厚奖励");
			}
		}
		else
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("经验副本")
			});
			text2 = Global.GetLang("点击前往经验副本，获得大量经验");
		}
		return text + "\n" + text2;
	}

	public void ShowOtherTaskItem(bool init = false, int newTaskID = -1, TaskData otherTaskData = null, int taskType = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		if (taskType == 0)
		{
			return;
		}
		this.ClearTaskListBoxChildren();
		if (init)
		{
			this.SetTaskListTaskClass();
		}
		List<TaskData> taskListSort = this.GetTaskListSort(newTaskID, taskType, triggerType);
		bool flag = true;
		if (taskListSort != null && taskListSort.Count > 0)
		{
			this.DeleteFakeTaskData(taskListSort);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < taskListSort.Count; i++)
			{
				if (taskListSort[i].TaskClass != 0)
				{
					if (taskListSort[i].TaskClass != 8 || Global.CanDoPaoHuanTask(8, out num, out num2) || taskListSort[i].specialRiChangTask)
					{
						int completState = 0;
						string text;
						if (taskListSort[i].specialRiChangTask)
						{
							text = this.GetSpecialDailyTaskDes(taskListSort[i].SpecialID);
						}
						else
						{
							text = this.GetOtherTaskContent(taskListSort[i], out completState, taskType, triggerType, BshowDialyTaskAertPatr);
						}
						if (string.IsNullOrEmpty(text))
						{
							MUDebug.Log<string>(new string[]
							{
								ConfigTasks.GetTaskXmlNodeByID(taskListSort[i].DoingTaskID).Title + Global.GetLang("任务描述为空！")
							});
						}
						else
						{
							GameObject taskItem = this.GetTaskItem(i, ref flag);
							taskItem.SetActive(true);
							if (taskListSort[i].TaskClass == 8)
							{
								taskItem.transform.name = "DailyTaskItem";
							}
							else if (taskListSort[i].TaskClass == 9)
							{
								taskItem.transform.name = "TaoFaItem";
							}
							else if (taskListSort[i].TaskClass == 1)
							{
								taskItem.transform.name = "ZhiXianTaskItem";
							}
							OtherTaskInfoItem component = taskItem.GetComponent<OtherTaskInfoItem>();
							TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskListSort[i].DoingTaskID);
							component.InitTextContent(text, taskListSort[i]);
							component.CompletState = completState;
							if (flag)
							{
								this.ItemCollection.AddNoUpdate(taskItem);
							}
							this.mTaskListBox.repositionNow = true;
						}
					}
				}
			}
			this.mTaskListBox.repositionNow = true;
		}
	}

	private List<TaskData> GetTaskListSort(int taskId = -1, int taskType = -1, int triggerType = -1)
	{
		if (Global.Data.roleData.TaskDataList != null)
		{
			int count = Global.Data.roleData.TaskDataList.Count;
			for (int i = 0; i < count; i++)
			{
				TaskData taskData = Global.Data.roleData.TaskDataList[i];
				if (taskData.TaskClass != 0)
				{
					TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
					if (taskXmlNodeByID != null)
					{
						bool flag = Super.JugeTaskGuanLianChengJiu(taskXmlNodeByID);
						bool flag2 = Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskData.DoingTaskVal1);
						bool flag3 = Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, taskData.DoingTaskVal2);
						if (!flag || !flag2 || !flag3)
						{
							taskData.IsComplete = false;
						}
						else
						{
							taskData.IsComplete = true;
						}
					}
				}
			}
			if (taskId == -1 && taskType != 8)
			{
				return Global.Data.roleData.TaskDataList;
			}
			this.ClearAllList();
			this.sortMainTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 0);
			if (taskId > 0 && taskType == 1)
			{
				if (triggerType == 0)
				{
					this.changeZhiXianTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 1 && result.DoingTaskID == taskId && !result.IsComplete);
				}
				if (triggerType == 1)
				{
					this.changeZhiXianTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 1 && result.DoingTaskID == taskId && !result.IsComplete);
				}
			}
			this.sortZhiXianNotCompleteTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 1 && !result.IsComplete);
			if (this.changeZhiXianTask != null && this.changeZhiXianTask.Count > 0)
			{
				for (int j = 0; j < this.changeZhiXianTask.Count; j++)
				{
					if (j < this.sortZhiXianNotCompleteTask.Count && this.sortZhiXianNotCompleteTask.Contains(this.changeZhiXianTask[j]))
					{
						this.sortZhiXianNotCompleteTask.Remove(this.changeZhiXianTask[j]);
					}
				}
			}
			this.sortZhiXianCompleteTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 1 && result.IsComplete);
			this.sortDailyNotCompleteTask = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 8);
			this.sortList.AddRange(this.sortMainTask);
			this.sortList.AddRange(this.sortDailyNotCompleteTask);
			this.sortList.AddRange(this.sortZhiXianCompleteTask);
			if (this.changeZhiXianTask != null && this.changeZhiXianTask.Count > 0)
			{
				this.sortList.AddRange(this.changeZhiXianTask);
			}
			this.sortList.AddRange(this.sortZhiXianNotCompleteTask);
			if (this.sortList.Count == Global.Data.roleData.TaskDataList.Count)
			{
				List<TaskData> list = new List<TaskData>();
				for (int k = 0; k < this.sortList.Count; k++)
				{
					list.Add(this.sortList[k]);
				}
				Global.Data.roleData.TaskDataList = list;
			}
		}
		return this.sortList;
	}

	private void ClearAllList()
	{
		if (this.sortList != null)
		{
			this.sortList.Clear();
		}
		if (this.changeZhiXianTask != null)
		{
			this.changeZhiXianTask.Clear();
		}
		if (this.sortMainTask != null)
		{
			this.sortMainTask.Clear();
		}
		if (this.sortZhiXianNotCompleteTask != null)
		{
			this.sortZhiXianNotCompleteTask.Clear();
		}
		if (this.sortZhiXianCompleteTask != null)
		{
			this.sortZhiXianCompleteTask.Clear();
		}
		if (this.sortDailyNotCompleteTask != null)
		{
			this.sortDailyNotCompleteTask.Clear();
		}
	}

	private void ClickOtherTaskItem(object sender, MouseEvent e)
	{
		GameObject selectedItem = this.mTaskListBox.SelectedItem;
		if (null != selectedItem)
		{
			if (selectedItem.transform.name == "MainTaskInfoObj")
			{
				this.ButtonClick(TaskClasses.Main, null, selectedItem);
			}
			else
			{
				OtherTaskInfoItem component = selectedItem.GetComponent<OtherTaskInfoItem>();
				this.ButtonClick((TaskClasses)component.taskData.TaskClass, component.taskData, selectedItem);
			}
		}
	}

	public string GetOtherTaskContent(TaskData taskData, out int completState, int taskType = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		completState = 0;
		if (taskData.TaskClass == 8)
		{
			if (taskData.DoingTaskID <= 0)
			{
				TaskVO taskVO = Super.FindNextTask((TaskClasses)taskData.TaskClass);
				string taskSourceNPCDesc = Super.GetTaskSourceNPCDesc(taskVO);
				string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("日常任务")
				});
				return colorStringForNGUIText + "\n" + Global.GetLang("点击接取日常任务，获得经验奖励");
			}
			taskData.RoadOtherTaskId = taskData.DoingTaskID;
			taskData.IsComplete = false;
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			bool flag = Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskData.DoingTaskVal1);
			bool flag2 = Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, taskData.DoingTaskVal2);
			string text = string.Empty;
			if (!flag)
			{
				text += this.GetOtherTaskInfoPartStr(taskData, taskXmlNodeByID, 1);
			}
			else if (!flag2)
			{
				text += this.GetOtherTaskInfoPartStr(taskData, taskXmlNodeByID, 2);
			}
			else
			{
				taskData.IsComplete = true;
				if (triggerType == 1)
				{
					this.OnTaskOK(TaskClasses.DailyTask);
				}
				if (this._ActiveTaskClass == 8 && Global.Data.PlayGame && (!(null != PlayZone.GlobalPlayZone.TaskWindow) || !NGUITools.GetActive(PlayZone.GlobalPlayZone.TaskWindow.gameObject)) && BshowDialyTaskAertPatr && !Global.DialyTaskAlertIsUsing && "1" == ConfigSystemParam.GetSystemParamByName("RiChangOpen", true) && taskData.DoingTaskVal1 != 0)
				{
					PlayZone.GlobalPlayZone.ShowDialyTaskAlertPart(taskData);
				}
				text += Super.GetTaskDestNPCDesc(taskXmlNodeByID, true);
				if (taskData.TaskClass == 8)
				{
					this.SwitchToTask(taskData.DoingTaskID, text, triggerType, TaskClasses.DailyTask);
				}
			}
			return text;
		}
		else
		{
			if (taskData.TaskClass != 1)
			{
				return string.Empty;
			}
			if (taskData == null)
			{
				return null;
			}
			string result = null;
			if (taskData != null)
			{
				taskData.RoadOtherTaskId = taskData.DoingTaskID;
				TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
				string title = taskXmlNodeByID2.Title;
				int taskClass = taskXmlNodeByID2.TaskClass;
				string taskClassName = Super.GetTaskClassName(taskClass);
				taskData.IsComplete = Super.JugeTaskGuanLianChengJiu(taskXmlNodeByID2);
				bool flag3 = Super.JugeTaskTargetComplete(taskXmlNodeByID2, 1, taskData.DoingTaskVal1);
				bool flag4 = Super.JugeTaskTargetComplete(taskXmlNodeByID2, 2, taskData.DoingTaskVal2);
				string text2 = string.Empty;
				int limitLevel = taskXmlNodeByID2.LimitLevel;
				int limitZhuanSheng = taskXmlNodeByID2.LimitZhuanSheng;
				if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
				{
					text2 += Global.GetColorStringForNGUIText(new object[]
					{
						"fd010c",
						string.Format(Global.GetLang("需要等级达到{0}\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng))
					});
					taskData.IsLevelLimited = false;
				}
				else
				{
					taskData.IsLevelLimited = true;
				}
				bool flag5 = true;
				if (!flag3 || !flag4 || !taskData.IsComplete)
				{
					bool flag6 = false;
					int taketime = taskXmlNodeByID2.Taketime;
					if (taketime > 0 && Global.GetCorrectLocalTime() - taskData.AddDateTime >= (long)(taketime * 1000))
					{
						flag6 = true;
						text2 += StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
						{
							title,
							Global.GetColorStringForNGUIText(new object[]
							{
								this.redColor,
								Global.GetLang("【失败】")
							})
						});
					}
					string pubStartTime = taskXmlNodeByID2.PubStartTime;
					string pubEndTime = taskXmlNodeByID2.PubEndTime;
					if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
					{
						double num = (double)Global.GetCorrectLocalTime();
						double num2 = (double)Global.SafeConvertToTicks(pubStartTime);
						double num3 = (double)Global.SafeConvertToTicks(pubEndTime);
						if (num < num2 || num > num3)
						{
							flag6 = true;
							text2 += StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
							{
								title,
								Global.GetColorStringForNGUIText(new object[]
								{
									this.redColor,
									Global.GetLang("【失败】")
								})
							});
						}
					}
					if (flag6)
					{
						result = text2;
						flag5 = false;
						completState = -1;
					}
				}
				if (flag5)
				{
					if (!flag3 || !flag4 || !taskData.IsComplete)
					{
						if (!flag3)
						{
							string zhiXianTaskInfoPartStr = this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID2, 1);
							if (!taskData.IsLevelLimited && !string.IsNullOrEmpty(taskXmlNodeByID2.LevelYuGao))
							{
								text2 += taskXmlNodeByID2.LevelYuGao;
							}
							else
							{
								text2 += zhiXianTaskInfoPartStr;
							}
						}
						else if (!flag4)
						{
							string zhiXianTaskInfoPartStr2 = this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID2, 2);
							if (!taskData.IsLevelLimited && !string.IsNullOrEmpty(taskXmlNodeByID2.LevelYuGao))
							{
								text2 += taskXmlNodeByID2.LevelYuGao;
							}
							else
							{
								text2 += zhiXianTaskInfoPartStr2;
							}
						}
						else if (!taskData.IsComplete)
						{
							string zhiXianTaskInfoPartStr3 = this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID2, 0);
							if (!taskData.IsLevelLimited && !string.IsNullOrEmpty(taskXmlNodeByID2.LevelYuGao))
							{
								text2 += taskXmlNodeByID2.LevelYuGao;
							}
							else
							{
								text2 += zhiXianTaskInfoPartStr3;
							}
						}
						completState = 0;
					}
					else
					{
						taskData.IsComplete = true;
						text2 += Super.GetTaskDestNPCDesc(taskXmlNodeByID2, true);
						if (text2.Length > 0)
						{
							string taskDestNPCName = Super.GetTaskDestNPCName(taskXmlNodeByID2);
							if (string.Empty != taskDestNPCName)
							{
								int num4 = -1;
								int num5 = -1;
								int num6 = -1;
								Super.GetTaskDestNPCID(taskXmlNodeByID2, ref num4, ref num5, ref num6);
								this.SetZhiXianTargetPos(num5, taskData.DoingTaskID, num6, num4, -1, -1, -1);
								taskData.zhiXianTargetType = num5;
								taskData.zhiXianNpcID = num6;
								taskData.zhiXianMapCode = num4;
							}
							else
							{
								taskData.zhiXianNpcID = taskXmlNodeByID2.DestNPC;
							}
						}
						completState = 1;
					}
					result = text2;
				}
			}
			return result;
		}
	}

	private void ClearTaskListBoxChildren()
	{
		this.mCacheTaskItem.Clear();
		if (this.ItemCollection.Count > 0)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GameObject at = this.ItemCollection.GetAt(i);
				if (!(at.name == "MainTaskInfoObj"))
				{
					at.SetActive(false);
					this.mCacheTaskItem.Add(at);
				}
			}
		}
	}

	private GameObject GetTaskItem(int index, ref bool isAddToItemCollection)
	{
		if (this.mCacheTaskItem.Count <= 0 || index > this.mCacheTaskItem.Count - 1)
		{
			isAddToItemCollection = true;
			return NGUITools.AddChild(this.mTaskListBox.gameObject, this.mOtherTaskItemObj);
		}
		isAddToItemCollection = false;
		return this.mCacheTaskItem[index];
	}

	private void DeleteFakeTaskData(List<TaskData> taskDatas)
	{
		if (taskDatas == null || taskDatas.Count <= 0)
		{
			return;
		}
		TaskData taskData = taskDatas.Find((TaskData result) => result.DoingTaskID != -1 && result.TaskClass == 8);
		if (taskData != null)
		{
			TaskData taskData2 = taskDatas.Find((TaskData result) => result.DoingTaskID == -1 && result.TaskClass == 8);
			if (taskData2 != null)
			{
				taskDatas.Remove(taskData2);
			}
		}
	}

	public void PlayNewTaskAnim()
	{
		this.RefreshTasks(this.TaskID, 0, true);
	}

	private void SwitchTaskAnimEnd(UITweener tween)
	{
		this.OldTaskInfo.text = null;
		TweenPosition component = this.NewTaskAnim.gameObject.GetComponent<TweenPosition>();
		if (null != component)
		{
			component.gameObject.SetActive(false);
			component.Reset();
		}
		component = this.OldTaskInfo.gameObject.GetComponent<TweenPosition>();
		if (null != component)
		{
			component.gameObject.SetActive(false);
			this.NewTaskAnim.Reset();
			component.Reset();
		}
	}

	private void TaskOKAnimEnd(UITweener tween)
	{
		TweenAlpha component = this.TaskOKAnim.gameObject.GetComponent<TweenAlpha>();
		if (null != component)
		{
			component.gameObject.SetActive(false);
			component.Reset();
		}
		if (Global.Data.roleData.ChangeLifeCount < 3 && this.TaskCompleted)
		{
			this.ShowHelpAnim(1, 1);
		}
	}

	private void SwitchToTask(int taskID, string newTaskStr, int trigerType = -1, TaskClasses tasktype = TaskClasses.Main)
	{
		if (trigerType != 0)
		{
			if (tasktype == TaskClasses.Main)
			{
				this.MainTaskInfo.text = newTaskStr;
			}
			else
			{
				this.OtherTaskInfo.text = newTaskStr;
			}
		}
		else if (tasktype == TaskClasses.Main)
		{
			this.MainTaskInfo.text = newTaskStr;
			this.OnTaskNew(TaskClasses.Main);
		}
		else
		{
			this.OnTaskNew(TaskClasses.DailyTask);
		}
	}

	private void OnEmptyResetScroll(GameObject obj, string txt)
	{
		bool flag = false;
		if (string.IsNullOrEmpty(txt))
		{
			flag = true;
		}
		else if (txt.Trim().Length <= 16)
		{
			txt = txt.Substring(txt.IndexOf("["), txt.IndexOf("]") - txt.IndexOf("[") + 1);
			txt = txt.Substring(txt.IndexOf("["), txt.IndexOf("]") - txt.IndexOf("[") + 1);
			flag = string.IsNullOrEmpty(txt);
		}
		if (flag)
		{
			obj.SetActive(false);
		}
		else if (!obj.activeSelf)
		{
			obj.SetActive(true);
		}
	}

	private void OnTaskOK(TaskClasses Type)
	{
		if (Type == TaskClasses.Main)
		{
			this.TaskOKAnim.transform.localPosition = new Vector3(this.TaskOKAnim.transform.localPosition.x, -82f, this.TaskOKAnim.transform.localPosition.z);
		}
		else
		{
			this.TaskOKAnim.transform.localPosition = new Vector3(this.TaskOKAnim.transform.localPosition.x, -146f, this.TaskOKAnim.transform.localPosition.z);
		}
		if (!this.TaskOKAnim.gameObject.activeSelf)
		{
			TweenAlpha component = this.TaskOKAnim.gameObject.GetComponent<TweenAlpha>();
			if (null != component)
			{
				component.enabled = true;
				component.gameObject.SetActive(true);
				component.Reset();
				component.Play(true);
				this.TaskOKAnim.Reset();
			}
		}
	}

	private void OnTaskNew(TaskClasses Type)
	{
		if (Type == TaskClasses.Main)
		{
			this.NewTaskAnim.transform.localPosition = new Vector3(this.NewTaskAnim.transform.localPosition.x, -82f, this.NewTaskAnim.transform.localPosition.z);
		}
		else
		{
			this.NewTaskAnim.transform.localPosition = new Vector3(this.NewTaskAnim.transform.localPosition.x, -146f, this.NewTaskAnim.transform.localPosition.z);
		}
		if (!this.NewTaskAnim.gameObject.activeSelf)
		{
			this.NewTaskAnim.transform.localPosition = TaskBoxMini.AnimPosFrom;
			TweenPosition component = this.NewTaskAnim.gameObject.GetComponent<TweenPosition>();
			this.NewTaskAnim.enabled = true;
			if (null != component)
			{
				if (Type == TaskClasses.Main)
				{
					component.from = new Vector3(480f, -82f, 0f);
					component.to = new Vector3(480f, -82f, 0f);
				}
				else
				{
					component.from = new Vector3(480f, -146f, 0f);
					component.to = new Vector3(480f, -146f, 0f);
				}
				component.gameObject.SetActive(true);
				component.Reset();
				component.Play(true);
			}
		}
	}

	private Vector3 NewVector3Y(Vector3 v, float y)
	{
		v.y = y;
		return v;
	}

	private string GetTaskInfoPartStr(TaskData taskData, XElement taskXmlNode, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskXmlNode, taskData.DoingTaskVal1, TargetID);
		text = Super.GetTaskTargetDesc(taskXmlNode, TargetID);
		if (text.Length > 0)
		{
			string taskTargetName = Super.GetTaskTargetName(taskXmlNode, TargetID);
			if (string.Empty != taskTargetName)
			{
				int num = -1;
				int targetType = -1;
				int targetID = -1;
				int toPosX = -1;
				int toPosY = -1;
				int num2;
				int num3;
				Super.GetTaskTargetID(taskXmlNode, TargetID, out num2, out num, out targetType, out targetID, out num3, false, out toPosX, out toPosY);
				if (num != Global.Data.roleData.MapCode && Global.GetMapType(num) != MapTypes.Normal)
				{
					Super.GetTaskDestNPCID(taskXmlNode, ref num, ref targetType, ref targetID);
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, -1, -1);
				}
				else
				{
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, toPosX, toPosY);
				}
			}
			if (!string.IsNullOrEmpty(taskTargetNum))
			{
				text = StringUtil.substitute("{0}{1}", new object[]
				{
					text,
					Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						taskTargetNum
					})
				});
			}
		}
		return text;
	}

	private string GetZhiXianTaskInfoPartStr(TaskData taskData, TaskVO taskVO, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskData.DoingTaskVal1, TargetID);
		if (taskData.IsLevelLimited)
		{
			text = Super.GetTaskTargetDesc(taskVO, TargetID, taskData.IsLevelLimited);
		}
		taskData.zhiXianLinkID = taskVO.LinkID;
		if (text.Length > 0)
		{
			string taskTargetName = Super.GetTaskTargetName(taskVO, TargetID);
			if (string.Empty != taskTargetName || taskVO.TargetType1 == 20)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				int num5 = -1;
				int num6;
				int num7;
				Super.GetTaskTargetID(taskVO, TargetID, out num6, out num, out num2, out num3, out num7, false, out num4, out num5);
				if (num != Global.Data.roleData.MapCode && Global.GetMapType(num) != MapTypes.Normal)
				{
					Super.GetTaskDestNPCID(taskVO, ref num, ref num2, ref num3);
					this.SetZhiXianTargetPos(num2, taskData.DoingTaskID, num3, num, -1, -1, -1);
					taskData.zhiXianTargetType = num2;
					taskData.zhiXianNpcID = num3;
					taskData.zhiXianMapCode = num;
				}
				else
				{
					this.SetZhiXianTargetPos(num2, taskData.DoingTaskID, num3, num, num4, num5, taskVO.LinkID);
					taskData.zhiXianTargetType = num2;
					taskData.zhiXianNpcID = num3;
					taskData.zhiXianMapCode = num;
					taskData.zhiXianToPosX = num4;
					taskData.zhiXianToPosY = num5;
					taskData.zhiXianLinkID = taskVO.LinkID;
				}
			}
			if (!string.IsNullOrEmpty(taskTargetNum) && taskVO.TargetType1 != 0 && taskVO.TargetType1 != 20 && !Super.IsCurrentTaskGuanLianChengJiu(taskVO))
			{
				text = StringUtil.substitute("{0}{1}", new object[]
				{
					text,
					Global.GetColorStringForNGUIText(new object[]
					{
						this.redColor,
						taskTargetNum
					})
				});
			}
		}
		return text;
	}

	private void SetZhiXianTargetPos(int targetType, int taskID, int targetID, int mapCode, int toPosX = -1, int toPosY = -1, int linkID = -1)
	{
		if (this.TargetType == 10001)
		{
			return;
		}
		if (this.TargetType == 10002)
		{
			return;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return;
		}
		Point point;
		if (this.TargetType == 2)
		{
			point = Global.GetMonsterPointByID(this.MapCode, this.NpcID);
		}
		else if (this.TargetType == 3)
		{
			point = Global.GetNPCPointByID(this.MapCode, this.NpcID);
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else
		{
			point = new Point(-1, -1);
		}
		if (point.X >= 0 && point.Y >= 0)
		{
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else if (toPosX >= 0 && toPosY >= 0)
		{
			this.ToPosX = toPosX;
			this.ToPosY = toPosY;
		}
		Super.SetLeadTargetPos(this.MapCode, new Vector3((float)(this.ToPosX / 100), 0f, (float)(this.ToPosY / 100)));
	}

	private string GetTaskInfoPartStr(TaskData taskData, TaskVO taskVO, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskData.DoingTaskVal1, TargetID);
		text = Super.GetTaskTargetDesc(taskVO, TargetID, this.LevelLimited);
		if (text.Length > 0)
		{
			string taskTargetName = Super.GetTaskTargetName(taskVO, TargetID);
			if (string.Empty != taskTargetName)
			{
				int num = -1;
				int targetType = -1;
				int targetID = -1;
				int toPosX = -1;
				int toPosY = -1;
				int num2;
				int num3;
				Super.GetTaskTargetID(taskVO, TargetID, out num2, out num, out targetType, out targetID, out num3, false, out toPosX, out toPosY);
				if (num != Global.Data.roleData.MapCode && Global.GetMapType(num) != MapTypes.Normal)
				{
					Super.GetTaskDestNPCID(taskVO, ref num, ref targetType, ref targetID);
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, -1, -1);
				}
				else
				{
					this.SetTargetPos(targetType, taskData.DoingTaskID, targetID, num, toPosX, toPosY);
				}
			}
			if (!string.IsNullOrEmpty(taskTargetNum))
			{
				text = StringUtil.substitute("{0}{1}", new object[]
				{
					text,
					Global.GetColorStringForNGUIText(new object[]
					{
						"FF0000",
						taskTargetNum
					})
				});
			}
		}
		return text;
	}

	private string GetOtherTaskInfoPartStr(TaskData taskData, TaskVO taskVO, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskData.DoingTaskVal1, TargetID);
		text = Super.GetTaskTargetDesc(taskVO, TargetID, true);
		if (!string.IsNullOrEmpty(taskTargetNum))
		{
			text = StringUtil.substitute("{0}{1}", new object[]
			{
				text,
				Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					taskTargetNum
				})
			});
		}
		return text;
	}

	private bool AddWaitingTask(TaskClasses taskClass)
	{
		TaskVO taskVO = Super.FindNextTask(taskClass);
		if (taskVO == null)
		{
			return false;
		}
		if (taskClass == TaskClasses.Main)
		{
			string text = string.Empty;
			int limitLevel = taskVO.LimitLevel;
			int limitZhuanSheng = taskVO.LimitZhuanSheng;
			if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					string.Format(Global.GetLang("需要等级达到{0}\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng))
				});
				this.ShowEffect(false);
				this.LevelLimited = false;
			}
			else
			{
				this.LevelLimited = true;
			}
			this.MainTaskInfo.Text = Super.GetTaskSourceNPCDesc(taskVO);
			if (this.MainTaskInfo.Text.Length > 0)
			{
				string taskSourceNPCName = Super.GetTaskSourceNPCName(taskVO);
				if (string.Empty != taskSourceNPCName)
				{
					int mapCode = -1;
					int targetType = -1;
					int targetID = -1;
					int id = taskVO.ID;
					Super.GetTaskSourceNPCID(taskVO, out mapCode, out targetType, out targetID);
					this.SetTargetPos(targetType, id, targetID, mapCode, -1, -1);
				}
			}
		}
		else
		{
			this.OtherTaskInfo.Text = Super.GetTaskSourceNPCDesc(taskVO);
		}
		return true;
	}

	private int ItemsList_Sort(TaskData a, TaskData b)
	{
		int taskClassByID = Global.GetTaskClassByID(a.DoingTaskID);
		int taskClassByID2 = Global.GetTaskClassByID(b.DoingTaskID);
		return taskClassByID - taskClassByID2;
	}

	private void SetTargetPos(int targetType, int taskID, int targetID, int mapCode, int toPosX = -1, int toPosY = -1)
	{
		this.MapCode = mapCode;
		this.TargetType = targetType;
		this.NpcID = targetID;
		this.TaskID = taskID;
		this.ToPosX = toPosX;
		this.ToPosY = toPosY;
		if (this.TargetType == 10001)
		{
			return;
		}
		if (this.TargetType == 10002)
		{
			return;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return;
		}
		Point point;
		if (this.TargetType == 2)
		{
			point = Global.GetMonsterPointByID(this.MapCode, this.NpcID);
		}
		else if (this.TargetType == 3)
		{
			point = Global.GetNPCPointByID(this.MapCode, this.NpcID);
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else
		{
			point = new Point(-1, -1);
		}
		if (point.X >= 0 && point.Y >= 0)
		{
			this.ToPosX = point.X;
			this.ToPosY = point.Y;
		}
		else if (toPosX >= 0 && toPosY >= 0)
		{
			this.ToPosX = toPosX;
			this.ToPosY = toPosY;
		}
		Super.SetLeadTargetPos(this.MapCode, new Vector3((float)(this.ToPosX / 100), 0f, (float)(this.ToPosY / 100)));
	}

	private void ButtonClick(TaskClasses Type, TaskData taskData = null, GameObject selectObj = null)
	{
		SystemHelpMgr.Reset();
		if (Type == TaskClasses.Main)
		{
			this.BodyClick(null);
			this.ShowSelectedTeXiao(selectObj);
		}
		else if (Type == TaskClasses.ZhiXianTask)
		{
			this.ZhiXianBodyClick(taskData, null);
			this.ShowSelectedTeXiao(selectObj);
		}
		else
		{
			if (taskData.specialRiChangTask && taskData.zhiXianLinkID > 0)
			{
				if (Global.IsKuaFuMap(Global.Data.roleData.MapCode, false))
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					return;
				}
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = taskData.zhiXianLinkID
				});
			}
			else if (taskData.IsComplete && taskData.RoadOtherTaskId != -1)
			{
				if (taskData.TaskClass == 8)
				{
					GameInstance.Game.SpriteClickOnNPC(this.MapCode, 2130706551, 119);
				}
				else
				{
					GameInstance.Game.SpriteClickOnNPC(this.MapCode, 2130706552, 120);
				}
			}
			else if (taskData.RoadOtherTaskId != -1)
			{
				Super.PrccessAutoTaskFindRoad(taskData.RoadOtherTaskId, false, false, false, true);
			}
			else if (99999 < taskData.zhiXianLinkID)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = taskData.zhiXianLinkID
				});
			}
			this.ShowEffect(false);
		}
	}

	private bool ZhiXianBodyClick(TaskData zhiXianTaskData, GameObject btn = null)
	{
		Global.Data.CurrentClickZhiXianTaskID = 0;
		SystemHelpMgr.Reset();
		Global.Data.CurrentClickZhiXianTaskID = zhiXianTaskData.DoingTaskID;
		if (zhiXianTaskData.DoingTaskID == -1)
		{
			return true;
		}
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(zhiXianTaskData.DoingTaskID);
		if (Super.JugeChengJiuComplete(taskXmlNodeByID) && Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, zhiXianTaskData.DoingTaskVal1) && Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, zhiXianTaskData.DoingTaskVal2))
		{
			int roleID = 2130706432 + zhiXianTaskData.zhiXianNpcID;
			GameInstance.Game.SpriteClickOnNPC(this.MapCode, roleID, zhiXianTaskData.zhiXianNpcID);
			PlayZone.GlobalPlayZone.StopCaiji(true);
			return true;
		}
		if (zhiXianTaskData.zhiXianLinkID > 0 || !Super.JugeChengJiuComplete(taskXmlNodeByID))
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = zhiXianTaskData.zhiXianLinkID
			});
			return true;
		}
		if (!Super.JugeChengJiuComplete(taskXmlNodeByID))
		{
			return false;
		}
		if (!zhiXianTaskData.IsLevelLimited)
		{
			return false;
		}
		if (zhiXianTaskData.DoingTaskID < 0)
		{
			return false;
		}
		if (!this.Panels[0].gameObject.activeInHierarchy)
		{
		}
		this.ShowHelpAnim(1, 0);
		this.ShowEffect(false);
		if (zhiXianTaskData.zhiXianTargetType == 10001)
		{
			GameInstance.Game.SpriteFindBiaoChe();
			return true;
		}
		if (zhiXianTaskData.zhiXianTargetType == 10002)
		{
			return true;
		}
		if (zhiXianTaskData.zhiXianTargetType == -1 || zhiXianTaskData.zhiXianMapCode == -1)
		{
			return true;
		}
		int zhiXianMapCode = zhiXianTaskData.zhiXianMapCode;
		if (zhiXianTaskData.zhiXianTargetType == 3 && Global.Data.roleData.IsFlashPlayer >= 1 && zhiXianTaskData.DoingTaskID <= 105 && zhiXianTaskData.DoingTaskID > 100)
		{
			if (Global.Data.GameScene != null)
			{
				Global.Data.GameScene.ExternalCallNpcDialog(this.NpcID, 20000);
			}
			return true;
		}
		MUDebug.Log<string>(new string[]
		{
			Global.GetLang("自动寻路开始")
		});
		Global.Data.TargetNpcID = zhiXianTaskData.zhiXianNpcID;
		TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(zhiXianTaskData.DoingTaskID);
		TaskData taskDataByID = Global.GetTaskDataByID(zhiXianTaskData.DoingTaskID);
		Point pos;
		if (taskXmlNodeByID2.TargetType1 == 20)
		{
			if (taskDataByID != null && Super.JugeTaskComplete(taskXmlNodeByID2, taskDataByID.DoingTaskVal1, 0))
			{
				pos = Global.GetNPCPointByID(zhiXianMapCode, Global.Data.TargetNpcID);
				zhiXianTaskData.zhiXianTargetType = 3;
			}
			else
			{
				string[] array = taskXmlNodeByID2.TargetPos1.Split(new char[]
				{
					','
				});
				pos = new Point(int.Parse(array[0]), int.Parse(array[1]));
				zhiXianTaskData.zhiXianTargetType = -1;
			}
		}
		else if (zhiXianTaskData.zhiXianTargetType == 2)
		{
			pos = Global.GetMonsterPointByID(zhiXianMapCode, Global.Data.TargetNpcID);
		}
		else if (zhiXianTaskData.zhiXianTargetType == 3)
		{
			pos = Global.GetNPCPointByID(zhiXianMapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(this.ToPosX, this.ToPosY);
		}
		if (pos.X < 0 || (pos.Y < 0 && this.ToPosX >= 0 && this.ToPosY >= 0))
		{
			pos.X = this.ToPosX;
			pos.Y = this.ToPosY;
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		Global.Data.TargetNpcID = zhiXianTaskData.zhiXianNpcID;
		this.CanTeleport = Global.GetTaskTeleportsByID(taskXmlNodeByID2.ID);
		if (zhiXianTaskData.zhiXianTargetType == 2 && Global.Data.roleData.IsFlashPlayer == 0)
		{
			Global.Data.GameScene.AutoFindRoad(zhiXianMapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
			if (0 < this.CanTeleport && Super.CanTransport(zhiXianMapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(zhiXianTaskData.DoingTaskID);
			}
		}
		else if (zhiXianTaskData.zhiXianTargetType == 3)
		{
			int num = (this.NpcID != 60900) ? 0 : 30;
			Global.Data.GameScene.AutoFindRoad(zhiXianMapCode, pos, 120 + num, ExtActionTypes.EXTACTION_NPCDLG);
			if (0 < this.CanTeleport && Super.CanTransport(zhiXianMapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(zhiXianTaskData.DoingTaskID);
			}
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(zhiXianMapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
			if (0 < this.CanTeleport && Super.CanTransport(zhiXianMapCode, true, true))
			{
				GameInstance.Game.SpriteTaskTransport2(zhiXianTaskData.DoingTaskID);
			}
		}
		return true;
	}

	private void ShiLiTaskClick(TaskData taskData)
	{
		TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
		if (taskXmlNodeByID == null)
		{
			return;
		}
		if (Super.JugeTaskComplete(taskXmlNodeByID, taskData.DoingTaskVal1, taskData.DoingTaskVal2))
		{
			ShiLiData.OpenRenWuWindow();
			return;
		}
		if (taskXmlNodeByID.TargetType1 == 101 || taskXmlNodeByID.TargetType1 == 102)
		{
			return;
		}
		Super.PrccessAutoTaskFindRoad(taskData.RoadOtherTaskId, false, false, false, true);
	}

	private bool BodyClick(GameObject btn = null)
	{
		SystemHelpMgr.Reset();
		if (!this.LevelLimited)
		{
			return false;
		}
		if (this.TaskID < 0)
		{
			return false;
		}
		if (!this.Panels[0].gameObject.activeInHierarchy)
		{
		}
		this.ShowHelpAnim(1, 0);
		this.ShowEffect(false);
		SystemHelpMgr.OnAction(UIObjIDs.MainGameTaskBoxTaskDesc, HelpStateEvents.Clicked, -1);
		if (this.TargetType == 10001)
		{
			GameInstance.Game.SpriteFindBiaoChe();
			return true;
		}
		if (this.TargetType == 10002)
		{
			return true;
		}
		if (this.TargetType == -1 || this.MapCode == -1)
		{
			return true;
		}
		int mapCode = this.MapCode;
		if (Global.IsGoToKuaFuMap(mapCode))
		{
			PlayZone.GlobalPlayZone.OpenKuafuMapView(this.TargetType, -1, this.NpcID, mapCode, this.ToPosX, this.ToPosY, false, 0, 0, false, false);
			return true;
		}
		if (Global.GetMapType(mapCode) <= MapTypes.Normal || (long)mapCode == ConfigSystemParam.GetSystemParamIntByName("SpecialCopySceneToMonsterRealive"))
		{
			if (Global.IsSpecialNormalMapCode(this.MapCode))
			{
			}
		}
		this.CanTeleport = Global.GetTaskTeleportsByID(this.TaskID);
		if (this.TargetType == 3)
		{
			byte b = 0;
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (taskXmlNodeByID != null && 99999 < taskXmlNodeByID.LinkID && !this.TaskCompletedEX)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = taskXmlNodeByID.LinkID
				});
				b = 1;
			}
			if (b == 0 && Global.Data.roleData.IsFlashPlayer >= 1 && this.TaskID <= 105 && this.TaskID > 100)
			{
				if (Global.Data.GameScene != null)
				{
					Global.Data.GameScene.ExternalCallNpcDialog(this.NpcID, 20000);
				}
				return true;
			}
		}
		return this.FindRoad(mapCode);
	}

	private void ShowSelectedTeXiao(GameObject parent)
	{
	}

	private bool FindRoad(int mapCode)
	{
		Global.PlaySoundAudio("Audio/YinDao/NextPage.mp3", false);
		Global.Data.TargetNpcID = this.NpcID;
		Point pos;
		if (this.TargetType == 2)
		{
			pos = Global.GetMonsterPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else if (this.TargetType == 3)
		{
			pos = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}
		else
		{
			pos = new Point(this.ToPosX, this.ToPosY);
		}
		if (pos.X < 0 || (pos.Y < 0 && this.ToPosX >= 0 && this.ToPosY >= 0))
		{
			pos.X = this.ToPosX;
			pos.Y = this.ToPosY;
		}
		if (pos.X == -1 || pos.Y == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
			return true;
		}
		Global.Data.TargetNpcID = this.NpcID;
		if (this.TargetType == 2 && Global.Data.roleData.IsFlashPlayer == 0)
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		else if (this.TargetType == 3)
		{
			int num = (this.NpcID != 60900) ? 0 : 30;
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 120 + num, ExtActionTypes.EXTACTION_NPCDLG);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(mapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
			if (0 < this.CanTeleport && Super.CanTransport(mapCode, true, true))
			{
				GameInstance.Game.SpriteTaskTransport2(this.TaskID);
			}
		}
		return true;
	}

	private void OnEnable()
	{
		this.ShowEffect(true);
	}

	public void ShowEffect(bool show)
	{
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return;
		}
		if (show && !Global.IsAutoFighting() && Global.Data.AutoRoadItemsList == null)
		{
			if (!this.LevelLimited)
			{
				base.StopCoroutine("ShowArrowDelay");
				this.AnimTaskHand.gameObject.SetActive(false);
			}
			else if (base.gameObject.activeInHierarchy)
			{
				base.StopCoroutine("ShowArrowDelay");
				base.StartCoroutine("ShowArrowDelay", show);
			}
		}
		else
		{
			base.StopCoroutine("ShowArrowDelay");
			this.AnimTaskHand.gameObject.SetActive(false);
		}
	}

	private IEnumerator ShowArrowDelay(bool isShow)
	{
		yield return new WaitForSeconds(2f);
		this.AnimTaskHand.gameObject.SetActive(isShow);
		yield break;
	}

	public void EnterMapScene()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (this.CurrentMapCode == Global.Data.roleData.MapCode && this._SceneUIClass == mapSceneUIClass)
		{
			return;
		}
		Global.YongzhezhanchangLianShaInf = -1;
		this.CurrentMapCode = Global.Data.roleData.MapCode;
		this._SceneUIClass = mapSceneUIClass;
		float num = 198f;
		float num2 = 58f;
		this.TitleBack.transform.localScale = new Vector3(205f, 30f, 0f);
		this.SceneInfosTitle.transform.localPosition = new Vector3(89f, 35f, -0.01f);
		this.SwitchIcon1.transform.localPosition = new Vector3(180f, 11f, -0.5f);
		this.GoIcon.GetComponent<Collider>().enabled = true;
		this.SceneInfosTitle.Text = Global.GetLang("副本信息");
		this.fuBenTaskHand.SetActive(false);
		this.Bak.gameObject.SetActive(false);
		this.SwitchIcon.gameObject.SetActive(false);
		this.SwitchIcon1.gameObject.SetActive(false);
		this.Panels[0].gameObject.SetActive(true);
		this.Panels[1].gameObject.SetActive(false);
		this.Panels[2].gameObject.SetActive(false);
		this.Panels[3].gameObject.SetActive(false);
		this.Panels[4].gameObject.SetActive(false);
		this.Panels[5].gameObject.SetActive(false);
		this.Panels[6].gameObject.SetActive(false);
		this.Panels[7].gameObject.SetActive(false);
		this.ClearSceneTaskInfos(0, 6);
		this.ShowAnim(true);
		this.LabMoYuMingCi.text = string.Empty;
		this.LabMoYuName.text = string.Empty;
		this.LabMoYuShangHai.text = string.Empty;
		this.LabMyData.text = string.Empty;
		this.FubenProcessBar.gameObject.SetActive(false);
		switch (this._SceneUIClass)
		{
		case SceneUIClasses.NormalCopy:
		case SceneUIClasses.ShuiJingHuanJing:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 78f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang("活动信息");
			if (this._SceneUIClass == SceneUIClasses.NormalCopy)
			{
				this.Bak.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (PlayZone.GlobalPlayZone.GameFubenBoxMini != null)
					{
						PlayZone.GlobalPlayZone.GameFubenBoxMini.OnClickTxt();
					}
				};
			}
			goto IL_2564;
		case SceneUIClasses.TaskCopy:
		case SceneUIClasses.NewPlayerMap:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			this.Bak.gameObject.SetActive(false);
			this.SwitchIcon.gameObject.SetActive(true);
			this.SwitchIcon1.gameObject.SetActive(false);
			this.Panels[0].gameObject.SetActive(true);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(true);
			goto IL_2564;
		case SceneUIClasses.BloodCastle:
		case SceneUIClasses.Demon:
		case SceneUIClasses.Battle:
		case SceneUIClasses.KaLiMaTemple:
		case SceneUIClasses.LoveFuBen:
		case SceneUIClasses.OnePiece:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 63f;
			num = 228f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.JingYanFuBen:
		case SceneUIClasses.JinBiFuBen:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 63f;
			num = 228f;
			if (this.BodyVisible)
			{
				this.SwitchIcon1.transform.localPosition = new Vector3(205f, 11f, -0.5f);
			}
			else
			{
				this.SwitchIconTrans1.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
				this.SwitchIcon1.transform.localPosition = new Vector3(0f, 11f, -0.5f);
			}
			this.TitleBack.transform.localScale = new Vector3(228f, 30f, 1f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.EMoLaiXiCopy:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 63f;
			num = 201f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.PaTa:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 63f;
			num = 201f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(true);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.SetSceneTaskBtn(false, false);
			this.ClearSceneTaskInfos(0, 7);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.FamilyBoss:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 63f;
			num = 228f;
			this.Bak.gameObject.SetActive(false);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(false);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.LuolanFazhen:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 88f;
			num = 201f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			goto IL_2564;
		case SceneUIClasses.HuanYingSiYuan:
		case SceneUIClasses.TianTi:
		case SceneUIClasses.YongZheZhanChang:
		case SceneUIClasses.ElementWar:
		case SceneUIClasses.MoRiJudge:
		case SceneUIClasses.KuaFuBoss:
		case SceneUIClasses.CopyWolf:
		case SceneUIClasses.ZhongShenZhengBa:
		case SceneUIClasses.KuaFuWangZhe:
		case SceneUIClasses.WanMoXiaGu:
		case SceneUIClasses.KuaFuTeamCompete:
		case SceneUIClasses.KuaFuTeamCompeteZhengBa:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			if (Global.Data.roleData.MapCode == 13000)
			{
				num2 = 100f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(false);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ShowStatic(true);
				this.ClearSceneTaskInfos(0, 6);
				this.ShowAnim(false);
				this.SceneInfosTitle.Text = Global.GetLang("活动信息");
			}
			if (Global.Data.roleData.MapCode == 70000)
			{
				num2 = 113f;
				num = 228f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(true);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(2, 2);
				this.ShowAnim(false);
				this.ShowStatic(false);
				this.SceneInfos[0].GetComponent<TextBlock>()._CharMargin = new Vector2(1f, 5f);
				this.SetSceneTaskInfos(0, ZuduiFubenKuaFuPart.SetContentOfLbl(null, this.SceneInfos[0].text), new object[0]);
				this.SceneInfos[0].transform.localPosition = new Vector3(20.01926f, -43.17551f, 0f);
			}
			if (Global.Data.roleData.MapCode == 70100)
			{
				num2 = 113f;
				num = 201f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(true);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(2, 2);
				this.ShowAnim(false);
				this.ShowStatic(false);
			}
			if (Global.Data.roleData.MapCode == 70200)
			{
				num2 = 138f;
				num = 201f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(true);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(2, 2);
				this.ShowAnim(false);
				this.ShowStatic(false);
				this.FubenProcessBar.gameObject.SetActive(true);
			}
			if (Global.Data.roleData.MapCode == 70300)
			{
				num2 = 110f;
				num = 255f;
				this.TitleBack.transform.localScale = new Vector3(255f, 30f, 1f);
				this.SceneInfosTitle.transform.localPosition = new Vector3(114f, 35f, -0.01f);
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.SwitchIcon1.transform.localPosition = new Vector3(234f, 11f, -0.5f);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(true);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(2, 2);
				this.ShowAnim(false);
				this.ShowStatic(false);
			}
			if (Global.IsInKuafuHuodongYongZheZhanChang())
			{
				num2 = 100f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(false);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(0, 6);
				this.ShowAnim(false);
				this.SceneInfosTitle.Text = Global.GetLang("活动信息");
				num = 200f;
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("     教团       {0} "), 0), "fd010c"), new object[0]);
				this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("     盟军       {0} "), 0), "4997bc"), new object[0]);
				this.SetSceneTaskInfos(2, ColorCode.EncodingText(string.Format(Global.GetLang("     自己       {0} "), 0), "00ff00"), new object[0]);
			}
			if (Global.IsInKuaFuHuoDongWangZhe())
			{
				num2 = 100f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(false);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(0, 6);
				this.ShowAnim(false);
				this.SceneInfosTitle.Text = Global.GetLang("活动信息");
				num = 200f;
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("     教团       {0} "), 0), "fd010c"), new object[0]);
				this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("     盟军       {0} "), 0), "4997bc"), new object[0]);
				this.SetSceneTaskInfos(2, ColorCode.EncodingText(string.Format(Global.GetLang("     自己       {0} "), 0), "00ff00"), new object[0]);
			}
			if (Global.Data.roleData.MapCode >= 60000 && Global.Data.roleData.MapCode <= 60006)
			{
				num2 = 80f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(false);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(0, 6);
				this.ShowAnim(false);
				this.SceneInfosTitle.Text = Global.GetLang("活动信息");
				num = 200f;
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("     剩余BOSS：{0} "), 0), "fffffe"), new object[0]);
				this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("     剩余小怪：{0} "), 0), "fffffe"), new object[0]);
			}
			if (Global.IsInKuaFuTeamCompete())
			{
				num2 = 100f;
				this.Bak.gameObject.SetActive(true);
				this.SwitchIcon.gameObject.SetActive(false);
				this.SwitchIcon1.gameObject.SetActive(true);
				this.Panels[0].gameObject.SetActive(false);
				this.Panels[1].gameObject.SetActive(true);
				this.Panels[2].gameObject.SetActive(false);
				this.Panels[3].gameObject.SetActive(false);
				this.Panels[4].gameObject.SetActive(false);
				this.Panels[5].gameObject.SetActive(false);
				this.ClearSceneTaskInfos(0, 6);
				this.ShowAnim(false);
				this.SceneInfosTitle.Text = Global.GetLang("战场信息");
				num = 200f;
				this.mBakSprite.gameObject.SetActive(true);
				this.mBakSprite.transform.localScale = new Vector3(num, num2, 0f);
				this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("     我方存活人数：{0} "), 0), "fffffe"), new object[0]);
				this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("     对方存活人数：{0} "), 0), "fffffe"), new object[0]);
			}
			goto IL_2564;
		case SceneUIClasses.PKLovers:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 150f;
			num = 201f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang("战斗信息");
			this.SceneInfos[0].transform.localPosition = new Vector3(-8f, -45f, 0f);
			goto IL_2564;
		case SceneUIClasses.ZhengDuoZhiDi:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 150f;
			num = 296f;
			this.SwitchIcon1.transform.localPosition = new Vector3(275f, 11f, -0.5f);
			this.TitleBack.transform.localScale = new Vector3(296f, 30f, 1f);
			this.SceneInfosTitle.transform.localPosition = new Vector3(125f, 35f, -0.01f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			this.SceneInfosTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("争夺之地")
			});
			goto IL_2564;
		case SceneUIClasses.AKaLunXi:
		case SceneUIClasses.AKaLunDong:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 150f;
			num = 267f;
			this.SwitchIcon1.transform.localPosition = new Vector3(247f, 11f, -0.5f);
			this.TitleBack.transform.localScale = new Vector3(267f, 30f, 1f);
			this.SceneInfosTitle.transform.localPosition = new Vector3(125f, 35f, -0.01f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang("领地争夺");
			goto IL_2564;
		case SceneUIClasses.LingDiCaiJi:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 78f;
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang("领地采集");
			goto IL_2564;
		case SceneUIClasses.ZhanMengLianSaiBiSai:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 150f;
			num = 246f;
			if (this.BodyVisible)
			{
				this.SwitchIcon1.transform.localPosition = new Vector3(225f, 11f, -0.5f);
			}
			else
			{
				this.SwitchIconTrans1.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
				this.SwitchIcon1.transform.localPosition = new Vector3(0f, 11f, -0.5f);
			}
			this.TitleBack.transform.localScale = new Vector3(246f, 30f, 1f);
			this.SceneInfosTitle.transform.localPosition = new Vector3(105f, 35f, -0.01f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			this.SceneInfosTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战场情况")
			});
			goto IL_2564;
		case SceneUIClasses.KuaFuPlunderBattle:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 150f;
			num = 246f;
			if (this.BodyVisible)
			{
				this.SwitchIcon1.transform.localPosition = new Vector3(225f, 11f, -0.5f);
			}
			else
			{
				this.SwitchIconTrans1.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -180f));
				this.SwitchIcon1.transform.localPosition = new Vector3(0f, 11f, -0.5f);
			}
			this.TitleBack.transform.localScale = new Vector3(246f, 30f, 1f);
			this.SceneInfosTitle.transform.localPosition = new Vector3(105f, 35f, -0.01f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(true);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(2, 2);
			this.ShowAnim(false);
			this.SceneInfosTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战场信息")
			});
			goto IL_2564;
		case SceneUIClasses.Comp:
		{
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 78f;
			this.Bak.gameObject.SetActive(false);
			this.SwitchIcon.gameObject.SetActive(false);
			Transform transform = this.SwitchIcon1.transform;
			this.SwitchIcon1.gameObject.SetActive(true);
			transform.localPosition = new Vector3(200f, transform.localPosition.y, transform.localPosition.z);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(true);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang(string.Empty);
			GameInstance.Game.GetShiLiZhengBaScore();
			goto IL_2564;
		}
		case SceneUIClasses.MoYu:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 180f;
			num = 320f;
			this.SwitchIcon1.transform.localPosition = new Vector3(300f, 11f, -0.5f);
			this.TitleBack.transform.localScale = new Vector3(322f, 30f, 1f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.transform.localPosition = new Vector3(140f, 35f, -0.01f);
			this.SceneInfosTitle.Text = Global.GetLang("排行");
			this.SetMoYuNumber(-1);
			goto IL_2564;
		case SceneUIClasses.ShiLian:
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 180f;
			num = 320f;
			this.SwitchIcon1.transform.localPosition = new Vector3(300f, 11f, -0.5f);
			this.TitleBack.transform.localScale = new Vector3(322f, 30f, 1f);
			this.Bak.gameObject.SetActive(true);
			this.SwitchIcon.gameObject.SetActive(false);
			this.SwitchIcon1.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(true);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.transform.localPosition = new Vector3(140f, 35f, -0.01f);
			this.SceneInfosTitle.Text = Global.GetLang("排行");
			this.SceneInfos[0].text = string.Empty;
			this.SceneInfos[1].text = string.Empty;
			this.SceneInfos[2].text = string.Empty;
			goto IL_2564;
		case SceneUIClasses.CompBattle:
		{
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 78f;
			this.Bak.gameObject.SetActive(false);
			this.SwitchIcon.gameObject.SetActive(false);
			Transform transform2 = this.SwitchIcon1.transform;
			this.SwitchIcon1.gameObject.SetActive(true);
			transform2.localPosition = new Vector3(240f, transform2.localPosition.y, transform2.localPosition.z);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(true);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = Global.GetLang(string.Empty);
			GameInstance.Game.GetShiLiZhengBaScore();
			goto IL_2564;
		}
		case SceneUIClasses.CompBattleMiDong:
		{
			this.BodyVisible = this.m_OtherSceneTaskBtnIsShow;
			num2 = 78f;
			this.Bak.gameObject.SetActive(false);
			this.SwitchIcon.gameObject.SetActive(false);
			Transform transform3 = this.SwitchIcon1.transform;
			this.SwitchIcon1.gameObject.SetActive(true);
			transform3.localPosition = new Vector3(200f, transform3.localPosition.y, transform3.localPosition.z);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[1].gameObject.SetActive(false);
			this.Panels[2].gameObject.SetActive(false);
			this.Panels[3].gameObject.SetActive(false);
			this.Panels[4].gameObject.SetActive(false);
			this.Panels[5].gameObject.SetActive(false);
			this.Panels[6].gameObject.SetActive(true);
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SceneInfosTitle.Text = string.Empty;
			GameInstance.Game.GetShiLiZhengBaScore();
			goto IL_2564;
		}
		case SceneUIClasses.RebornMap:
			this.ClearSceneTaskInfos(0, 6);
			this.ShowAnim(false);
			this.SwitchIcon.gameObject.SetActive(true);
			this.Panels[0].gameObject.SetActive(false);
			this.Panels[7].gameObject.SetActive(true);
			if (null != this._RebirthBossTaskScene)
			{
				this._RebirthBossTaskScene.FirstTimeEnterMap = true;
				if (0 >= this._RebirthBossTaskScene.BoosLife)
				{
					this._RebirthBossTaskScene.FirstTimeEnterMap = false;
					this._RebirthBossTaskScene.ShowRankInf = false;
				}
				this._RebirthBossTaskScene.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(this._RebirthBossTaskScene, new DPSelectedItemEventArgs
						{
							ID = 0,
							Title = "RebirthBossTaskScene"
						});
					}
				};
			}
			goto IL_2564;
		}
		this.BodyVisible = this.m_NormalSceneTaskBtnIsShow;
		this.Bak.gameObject.SetActive(false);
		this.SwitchIcon.gameObject.SetActive(true);
		this.SwitchIcon1.gameObject.SetActive(false);
		this.Panels[0].gameObject.SetActive(true);
		this.Panels[1].gameObject.SetActive(false);
		this.Panels[2].gameObject.SetActive(false);
		this.Panels[3].gameObject.SetActive(false);
		this.Panels[4].gameObject.SetActive(false);
		this.Panels[5].gameObject.SetActive(false);
		this.ClearSceneTaskInfos(0, 6);
		this.ShowAnim(true);
		IL_2564:
		if (this.Panels[0].gameObject.activeSelf)
		{
			(Super.GData.PlayZoneRoot as PlayZone).SetTeamBoxShow(false);
		}
		else if (SceneUIClasses.RebornMap.IsTheScene())
		{
			(Super.GData.PlayZoneRoot as PlayZone).SetTeamBoxShow(false);
		}
		else
		{
			(Super.GData.PlayZoneRoot as PlayZone).SetTeamBoxShow(true);
		}
		this.Bak.Width = num;
		this.Bak.Height = num2;
		this.Bak.Refresh();
	}

	public void ClearSceneTaskInfos(int start, int count)
	{
		for (int i = start; i < start + count; i++)
		{
			this.SceneInfos[i].Text = string.Empty;
		}
	}

	public void WanMoXiaGuInfo(WanMoXiaGuScoreData data)
	{
		if (data == null)
		{
			this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("助阵坐骑个数：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				"0"
			})), new object[0]);
			this.SetSceneTaskInfos(1, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("助阵效果：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Empty
			})), new object[0]);
			this.SetSceneTaskInfos(2, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("BOSS剩余血量：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				"100%"
			})), new object[0]);
			return;
		}
		this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("助阵坐骑个数：")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.MonsterCount
		})), new object[0]);
		this.SetSceneTaskInfos(1, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("助阵效果：")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.Intro
		})), new object[0]);
		this.SetSceneTaskInfos(2, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("BOSS剩余血量：")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.BossLifePercent.ToString("p0")
		})), new object[0]);
		if (data.MonsterID > 0)
		{
			if (data.MonsterCount > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("添加Boss特效")
				});
				GameObject gameObject = GameObject.Find("Role_" + data.MonsterID);
				if (gameObject != null && data.Decorations != 0)
				{
					Transform transform = gameObject.transform.FindChild("WanMoXiaGuBossTeXiao");
					if (transform == null)
					{
						string bundleID = MuAssetManager.GetBundleID("Decoration", ConfigDecoration.GetDecorationVOByCode(data.Decorations).ResName);
						GameObject emptyLoader = U3DUtils.GetEmptyLoader("WanMoXiaGuBossTeXiao", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
						emptyLoader.transform.localPosition = new Vector3(0f, 0f, 0f);
						emptyLoader.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
						U3DUtils.AddChild(gameObject, emptyLoader, true);
					}
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("删除Boss特效")
				});
				GameObject gameObject2 = GameObject.Find("Role_" + data.MonsterID);
				if (gameObject2 != null)
				{
					Transform transform2 = gameObject2.transform.FindChild("WanMoXiaGuBossTeXiao");
					if (transform2 != null)
					{
						Object.Destroy(transform2.gameObject);
					}
				}
			}
		}
	}

	public void ZhanMengLianSaiCompetitionScore(BangHuiMatchScoreData data)
	{
		if (data == null)
		{
			data = new BangHuiMatchScoreData();
		}
		if (Global.IsCompetitionGuanKan)
		{
			this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("占领：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.BHName
			})), new object[0]);
			this.SetSceneTaskInfos(1, string.Format("{0}{1}{2}{3}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("教团：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.QiZhi1 + Global.GetLang("旗帜  ")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("分数：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.Score1
				})
			}), new object[0]);
			this.SetSceneTaskInfos(2, string.Format("{0}{1}{2}{3}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("盟军：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.QiZhi2 + Global.GetLang("旗帜  ")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("分数：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.Score2
				})
			}), new object[0]);
		}
		else
		{
			int battleWhichSide = Global.Data.roleData.BattleWhichSide;
			this.SetSceneTaskInfos(1, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("占领：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.BHName
			})), new object[0]);
			if (battleWhichSide == 1)
			{
				this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("神殿剩余敌人：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.PlayerNum2
				})), new object[0]);
				this.SetSceneTaskInfos(2, string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("本方：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.QiZhi1 + Global.GetLang("旗帜  ")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("分数：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.Score1
					})
				}), new object[0]);
				this.SetSceneTaskInfos(3, string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("敌方：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.QiZhi2 + Global.GetLang("旗帜  ")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("分数：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.Score2
					})
				}), new object[0]);
			}
			else if (battleWhichSide == 2)
			{
				this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("神殿剩余敌人：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					data.PlayerNum1
				})), new object[0]);
				this.SetSceneTaskInfos(2, string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("本方：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.QiZhi2 + Global.GetLang("旗帜  ")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("分数：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.Score2
					})
				}), new object[0]);
				this.SetSceneTaskInfos(3, string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("敌方：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.QiZhi1 + Global.GetLang("旗帜  ")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("分数：")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						data.Score1
					})
				}), new object[0]);
			}
		}
	}

	public void KuaFuPlunderBattleInfo(KuaFuLueDuoScoreData data)
	{
		int battleWhichSide = Global.Data.roleData.BattleWhichSide;
		if (battleWhichSide == 1)
		{
			this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("剩余资源：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.LeftZiYuan
			})), new object[0]);
			this.SetSceneTaskInfos(1, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("战场积分：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.SelfScore
			})), new object[0]);
			this.SetSceneTaskInfos(2, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("参与次数：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.Data.roleData.MoneyData[134]
			})), new object[0]);
		}
		else
		{
			this.SetSceneTaskInfos(0, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("剩余资源：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.LeftZiYuan
			})), new object[0]);
			this.SetSceneTaskInfos(1, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本方采集资源：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.LueDuoZiYuan
			})), new object[0]);
			this.SetSceneTaskInfos(2, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("战场积分：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.SelfScore
			})), new object[0]);
			this.SetSceneTaskInfos(3, string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("参与次数：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.Data.roleData.MoneyData[134]
			})), new object[0]);
		}
	}

	public void RefreshKuaFuTeamCompeteInfo(long info1, long info2)
	{
		int battleWhichSide = Global.Data.roleData.BattleWhichSide;
		if (battleWhichSide == 1)
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("  我方存活人数：{0} "), Global.GetString(new object[]
			{
				info1,
				"/",
				5
			})), "fffffe"), new object[0]);
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("  对方存活人数：{0} "), Global.GetString(new object[]
			{
				info2,
				"/",
				5
			})), "fffffe"), new object[0]);
		}
		else
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("  我方存活人数：{0} "), Global.GetString(new object[]
			{
				info2,
				"/",
				5
			})), "fffffe"), new object[0]);
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("  对方存活人数：{0} "), Global.GetString(new object[]
			{
				info1,
				"/",
				5
			})), "fffffe"), new object[0]);
		}
	}

	public void SetSceneTaskInfos(int index, string info, params object[] datas)
	{
		if (index >= 0 && index < this.SceneInfos.Length)
		{
			if (datas.Length == 0)
			{
				this.SceneInfos[index].Text = info;
			}
			else if (datas.Length == 1)
			{
				if (this.SceneInfos[index].Label.pivot == 3)
				{
					if (datas[0] is int && (int)datas[0] < 0)
					{
						this.SceneInfos[index].Text = string.Format("{0}{1}", info, "-");
					}
					else
					{
						this.SceneInfos[index].Text = string.Format("{0}{1}", info, datas[0]);
					}
				}
				else
				{
					this.SceneInfos[index].Text = string.Format("{1}{0}", info, datas[0]);
				}
			}
			else if (datas.Length == 2)
			{
				this.SceneInfos[index].Text = string.Format("{0}{1}/{2}", info, datas[0], datas[1]);
			}
			else
			{
				this.SceneInfos[index].Text = info;
				foreach (object obj in datas)
				{
					info = info + " " + obj;
				}
			}
		}
	}

	public void SetSceneTaskBtn(bool isVisible, bool isLeave = false)
	{
		int mapCode = Global.Data.roleData.MapCode;
		if (isLeave)
		{
			if (mapCode < Global.GetPataIndexRange()[1])
			{
				this.NextIcon.gameObject.SetActive(isVisible);
				this.SceneInfos[7].gameObject.SetActive(!isVisible);
				this.ShowHelpAnim(1, 1);
			}
		}
		else if (mapCode <= Global.GetPataIndexRange()[1])
		{
			this.NextIcon.gameObject.SetActive(isVisible);
			this.SceneInfos[7].gameObject.SetActive(!isVisible);
			this.ShowHelpAnim(1, 0);
		}
	}

	public void SetHuanyingScene(bool isWhich)
	{
		if (isWhich)
		{
			this.HuanyingBlue.gameObject.SetActive(true);
			this.HuanyingRed.gameObject.SetActive(false);
			this.Animator2.gameObject.SetActive(false);
			this.Animator2.gameObject.SetActive(true);
		}
		else
		{
			this.HuanyingBlue.gameObject.SetActive(false);
			this.HuanyingRed.gameObject.SetActive(true);
			this.Animator1.gameObject.SetActive(false);
			this.Animator1.gameObject.SetActive(true);
		}
		this.SceneInfos[1].transform.localPosition = new Vector3(20f, -25f, 0f);
		this.SceneInfos[2].transform.localPosition = new Vector3(20f, -50f, 0f);
	}

	private void ShowStatic(bool bl)
	{
		this.staticText[0].gameObject.SetActive(bl);
		this.staticText[1].gameObject.SetActive(bl);
		this.staticText[2].gameObject.SetActive(bl);
	}

	public void ShowYuansuEffect(int Wave)
	{
		switch (Wave)
		{
		case 2:
			this.Animator_1.gameObject.SetActive(false);
			this.Animator_1.gameObject.SetActive(true);
			break;
		case 3:
			this.Animator_2.gameObject.SetActive(false);
			this.Animator_2.gameObject.SetActive(true);
			break;
		case 4:
			this.Animator_3.gameObject.SetActive(false);
			this.Animator_3.gameObject.SetActive(true);
			break;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.StopTimer();
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("TaskBoxMini_Timer");
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
		switch (this._SceneUIClass)
		{
		case SceneUIClasses.CopyWolf:
			ZuduiFuBen_LangHunYaoSai.UpdateUI();
			break;
		}
	}

	public void BloodCastleSenceStateInfo(int index, int mapCode, int time, int[] extData)
	{
		if (index == 0)
		{
			this.Targets = new SceneTaskInfo[extData.Length];
			for (int i = 0; i < extData.Length; i++)
			{
				this.Targets[i] = new SceneTaskInfo();
				this.Targets[i].TargetNum = extData[i];
			}
			this.Targets[0].Desc = ColorCode.EncodingText(Global.GetLang("击杀吊桥怪物:"), "00ff00");
			this.Targets[1].Desc = ColorCode.EncodingText(Global.GetLang("击杀城门:"), "00ff00");
			this.Targets[2].Desc = ColorCode.EncodingText(Global.GetLang("击杀巫师:"), "00ff00");
			this.Targets[3].Desc = ColorCode.EncodingText(Global.GetLang("击碎灵棺:"), "00ff00");
			this.Targets[4].Desc = ColorCode.EncodingText(Global.GetLang("击碎雕像:"), "00ff00");
		}
	}

	public void BloodCastleSceneSelfScore(int mapCode, int score)
	{
		this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("个人积分: "), "00ff00"), new object[]
		{
			score
		});
	}

	public void BloodCastleSceneTaskStatus(int mapCode, int type, int num)
	{
		if (type >= 1 && this.Targets != null && type <= this.Targets.Length)
		{
			int num2 = type - 1;
			this.Targets[num2].TargetVal = num;
			this.SetSceneTaskInfos(0, this.Targets[num2].Desc, new object[]
			{
				num,
				this.Targets[num2].TargetNum
			});
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"Invalid type = " + type
			});
		}
	}

	public void DemonSceneTaskStatus(int remain, int score)
	{
		if (remain >= 0)
		{
			this.SetSceneTaskInfos(0, Global.GetLang("剩余波数: "), new object[]
			{
				remain
			});
		}
		if (score >= 0)
		{
			this.SetSceneTaskInfos(1, Global.GetLang("个人得分: "), new object[]
			{
				score
			});
		}
	}

	public void DecMonsterNum(SpriteNotifyEventArgs e)
	{
		if (this._SceneUIClass == SceneUIClasses.JingYanFuBen)
		{
			if (this.RemainMonsters > 0)
			{
				this.RemainMonsters--;
			}
			this.RefreshJingYanFuBenScoreInfo(-1, -1, -1);
		}
	}

	public void RefreshJingYanFuBenScoreInfo(int currentGroup = -1, int totalGroup = -1, int remainMonster = -1)
	{
		if (currentGroup >= 0 && totalGroup >= 0)
		{
			if (currentGroup != 1 && this.CurrentGroup < currentGroup)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, Global.GetLang("下一波怪物刷新"), 0, -1, -1, 0);
			}
			this.CurrentGroup = currentGroup;
			this.TotalGroup = totalGroup;
			this.RemainMonsters = remainMonster;
		}
		this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("当前波数: "), "c39550"), new object[]
		{
			this.CurrentGroup,
			this.TotalGroup
		});
		this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("剩余怪物: "), "c39550"), new object[]
		{
			this.RemainMonsters
		});
	}

	public void JinBiUpDateInfo(int num, int count)
	{
		this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("当前波数: "), "c39550"), new object[]
		{
			num,
			count
		});
	}

	public void JinBiSceneCountDown(int time)
	{
		this.CountDownTime = time;
		base.InvokeRepeating("JinBiUITimer_Tick", 1f, 1f);
	}

	private void JinBiUITimer_Tick()
	{
		this.CountDownTime--;
		if (this.CountDownTime <= 0 && this.fisrt)
		{
			this.fisrt = false;
			if (this._SceneUIClass == SceneUIClasses.JinBiFuBen)
			{
				SystemHelpPart.Countdown(3, null, false);
			}
		}
	}

	public void ShuiJingHuanJingSceneInfo(int num)
	{
		if (num >= 0)
		{
			TaskBoxMini.ShuiJingHuanJingNum = num;
			this.SetSceneTaskInfos(0, Global.GetLang("剩余次数: "), new object[]
			{
				num
			});
		}
	}

	public void ArmyCaiJiSceneInfo(int num)
	{
		if (num >= 0)
		{
			TaskBoxMini.ArmyCaiJiNum = num;
			this.SetSceneTaskInfos(0, Global.GetLang("剩余次数: "), new object[]
			{
				num
			});
		}
	}

	public void EmolaixiInfo(int RunNum, int Round, int TotalRound, int TotalMonster)
	{
		if (RunNum != this.runNum)
		{
			this.SetSceneTaskInfos(1, ColorCode.EncodingText(Global.GetLang("怪物逃走: "), "c39550"), new object[]
			{
				RunNum,
				TotalMonster
			});
		}
		if (Round != this.round && Round < TotalRound)
		{
			this.SetSceneTaskInfos(0, ColorCode.EncodingText(Global.GetLang("当前波数: "), "c39550"), new object[]
			{
				Round + 1,
				TotalRound
			});
			SystemHelpPart.ShowHintTextNoTarget(true, string.Format(Global.GetLang("第{0}波怪物已经出现，请尽快击杀"), Round + 1), 3);
		}
		this.runNum = RunNum;
		this.round = Round;
		this.totalRound = TotalRound;
	}

	public void SetLuolanFazhenInfosColor(string boss1, string boss2)
	{
		if (this.SceneInfos[0].text != null)
		{
			this.SceneInfos[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				boss1,
				this.SceneInfos[0].text
			});
		}
		if (this.SceneInfos[1].text != null)
		{
			this.SceneInfos[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				boss2,
				this.SceneInfos[1].text
			});
		}
	}

	public void SetProcessBarPercent(float fPercent)
	{
		if (this.FubenProcessBar != null)
		{
			this.FubenProcessBar.uiLabel.text = string.Format("{0}%", (int)fPercent);
			fPercent = Mathf.Min(1f, fPercent / 100f);
			this.FubenProcessBar.Percent = (double)fPercent;
		}
	}

	public void SetProcessBarForGroundImg(string spriteName)
	{
		if (this.FubenProcessBar != null)
		{
			this.FubenProcessBar.foreground.gameObject.GetComponent<UISprite>().spriteName = spriteName;
		}
	}

	public void SetBuffInfo(CoupleArenaBuffHoldData Data)
	{
		if (Data == null)
		{
			return;
		}
		string text = null;
		string text2 = null;
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(Data.ZhenAiHolderZoneId, out ztBuffServerInfo))
		{
			if (Data.IsZhenAiBuffValid)
			{
				text = string.Format("{0}-{1}", ztBuffServerInfo.strServerName, Data.ZhenAiHolderRname);
			}
		}
		else if (Data.IsZhenAiBuffValid)
		{
			text = string.Format("s{0}-{1}", Data.ZhenAiHolderZoneId, Data.ZhenAiHolderRname);
		}
		if (Global.GetNowServerIsZhuTiFu(Data.YongQiHolderZoneId, out ztBuffServerInfo))
		{
			if (Data.IsYongQiBuffValid)
			{
				text2 = string.Format("{0}-{1}", ztBuffServerInfo.strServerName, Data.YongQiHolderRname);
			}
		}
		else if (Data.IsYongQiBuffValid)
		{
			text2 = string.Format("s{0}-{1}", Data.YongQiHolderZoneId, Data.YongQiHolderRname);
		}
		if (this.SceneInfos[0] != null)
		{
			this.SceneInfos[0].text = string.Concat(new string[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("真爱祝福持有者：")
				}),
				"\r\n\r\n",
				string.Format("       {0}", text),
				"\r\n\r\n",
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("勇气祝福持有者：")
				}),
				"\r\n\r\n",
				string.Format("       {0}", text2)
			});
		}
	}

	public void SetZhengDuoInfo(ZhengDuoScoreInfo data)
	{
		if (data.ScoreRank == null)
		{
			return;
		}
		for (int i = 0; i < data.ScoreRank.Count; i++)
		{
			string text;
			if ((data.Step == 1 && Global.Data.roleData.RoleID == data.ScoreRank[i].Id) || (data.Step >= 2 && data.Step <= 5 && Global.Data.roleData.Faction == data.ScoreRank[i].Id))
			{
				text = "17e43e";
			}
			else
			{
				text = "FF0000";
			}
			string text2;
			if (data.ScoreRank[i].Hurt < 1000000L)
			{
				text2 = ((double)data.ScoreRank[i].Hurt / 10000.0).ToString("0.0");
			}
			else
			{
				text2 = (data.ScoreRank[i].Hurt / 10000L).ToString();
			}
			this.SceneInfos[i].text = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				string.Format(Global.GetLang("{0}   伤害：{1}W"), data.ScoreRank[i].Name, text2)
			});
		}
	}

	private int KarenScoreSort(KarenScoreData s, KarenScoreData e)
	{
		if (s.Score < e.Score)
		{
			return 1;
		}
		if (s.Score > e.Score)
		{
			return -1;
		}
		if (s.ticks < e.ticks)
		{
			return -1;
		}
		if (s.ticks > e.ticks)
		{
			return 1;
		}
		return 0;
	}

	public void SetZhengDuoInfo(List<KarenScoreData> data)
	{
		List<KarenScoreData> list = new List<KarenScoreData>();
		for (int i = 0; i < data.Count; i++)
		{
			if (!string.IsNullOrEmpty(data[i].Name))
			{
				list.Add(data[i]);
			}
		}
		list.Sort(new Comparison<KarenScoreData>(this.KarenScoreSort));
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].Name != null)
			{
				string text;
				if (Global.Data.roleData.JunTuanId == list[j].LegionID)
				{
					text = "17e43e";
				}
				else
				{
					text = "FF0000";
				}
				int num;
				if (list[j].ResourceList == null || list[j].ResourceList.Count <= 0)
				{
					num = 0;
				}
				else
				{
					num = list[j].ResourceList.Count;
				}
				this.SceneInfoName[j].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					list[j].Name
				});
				this.SceneInfoZiYuan[j].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					string.Format(Global.GetLang("{0}资源"), num)
				});
				this.SceneInfoBiFen[j].text = Global.GetColorStringForNGUIText(new object[]
				{
					text,
					list[j].Score
				});
			}
		}
	}

	public void SetMoYuAndShiLianInfo(BossLifeLog data)
	{
		this.SceneInfos[1].text = string.Empty;
		this.LabMyData.text = string.Empty;
		this.LabMoYuMingCi.text = string.Empty;
		this.LabMoYuName.text = string.Empty;
		this.LabMoYuShangHai.text = string.Empty;
		if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu)
		{
			this.SceneInfos[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("名次                  战盟名称                总伤害")
			});
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.ShiLian)
		{
			this.SceneInfos[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("名次                  队伍名称                总伤害")
			});
		}
		if (data.BHAttackRank != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			for (int i = 0; i < data.BHAttackRank.Count; i++)
			{
				if (data.BHAttackRank[i].BHID == Global.Data.roleData.Faction && Global.GetMapSceneUIClass() == SceneUIClasses.MoYu)
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang(text)
					});
					text = text + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						(i + 1).ToString()
					}) + Environment.NewLine;
					text2 = text2 + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						data.BHAttackRank[i].BHName
					}) + Environment.NewLine;
					if (data.BHAttackRank[i].BHInjure / 100000000L > 0L)
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(data.BHAttackRank[i].BHInjure / 100000000L).ToString() + Global.GetLang("亿")
						}) + Environment.NewLine;
					}
					else if (data.BHAttackRank[i].BHInjure / 10000L > 0L)
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(data.BHAttackRank[i].BHInjure / 10000L).ToString() + Global.GetLang("万")
						}) + Environment.NewLine;
					}
					else
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							data.BHAttackRank[i].BHInjure
						}) + Environment.NewLine;
					}
				}
				else if ((data.BHAttackRank[i].BHName.Equals(Global.GetTeamLeaderName()) || data.BHAttackRank[i].BHName.Equals(Global.Data.roleData.RoleName)) && Global.GetMapSceneUIClass() == SceneUIClasses.ShiLian)
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang(text)
					});
					text = text + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						(i + 1).ToString()
					}) + Environment.NewLine;
					text2 = text2 + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						data.BHAttackRank[i].BHName
					}) + Environment.NewLine;
					if (data.BHAttackRank[i].BHInjure / 100000000L > 0L)
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(data.BHAttackRank[i].BHInjure / 100000000L).ToString() + Global.GetLang("亿")
						}) + Environment.NewLine;
					}
					else if (data.BHAttackRank[i].BHInjure / 10000L > 0L)
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							(data.BHAttackRank[i].BHInjure / 10000L).ToString() + Global.GetLang("万")
						}) + Environment.NewLine;
					}
					else
					{
						text3 = text3 + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							data.BHAttackRank[i].BHInjure
						}) + Environment.NewLine;
					}
				}
				else
				{
					text = text + (i + 1).ToString() + Environment.NewLine;
					text2 = text2 + data.BHAttackRank[i].BHName + Environment.NewLine;
					if (data.BHAttackRank[i].BHInjure / 100000000L > 0L)
					{
						text3 = text3 + (data.BHAttackRank[i].BHInjure / 100000000L).ToString() + Global.GetLang("亿") + Environment.NewLine;
					}
					else if (data.BHAttackRank[i].BHInjure / 10000L > 0L)
					{
						text3 = text3 + (data.BHAttackRank[i].BHInjure / 10000L).ToString() + Global.GetLang("万") + Environment.NewLine;
					}
					else
					{
						text3 = text3 + data.BHAttackRank[i].BHInjure + Environment.NewLine;
					}
				}
			}
			this.LabMoYuMingCi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(text)
			});
			this.LabMoYuName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(text2)
			});
			this.LabMoYuShangHai.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang(text3)
			});
		}
		if (data.SelfBHAttack != null && (float)data.Injure > 0f)
		{
			float num = (float)data.SelfBHAttack.BHInjure / (float)data.Injure;
			string text4 = string.Empty;
			if (data.SelfBHAttack.BHInjure / 100000000L > 0L)
			{
				text4 = (data.SelfBHAttack.BHInjure / 100000000L).ToString() + Global.GetLang("亿");
			}
			else if (data.SelfBHAttack.BHInjure / 10000L > 0L)
			{
				text4 = (data.SelfBHAttack.BHInjure / 10000L).ToString() + Global.GetLang("万");
			}
			else
			{
				text4 = data.SelfBHAttack.BHInjure.ToString();
			}
			if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu)
			{
				this.LabMyData.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Concat(new string[]
					{
						Global.GetLang("我的战盟总伤害："),
						text4,
						"(",
						(num * 100f).ToString("F1"),
						"%)"
					})
				});
			}
			else if (Global.GetMapSceneUIClass() == SceneUIClasses.ShiLian)
			{
				this.LabMyData.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Concat(new string[]
					{
						Global.GetLang("我的队伍总伤害："),
						text4,
						"(",
						(num * 100f).ToString("F1"),
						"%)"
					})
				});
			}
		}
	}

	public void SetMoYuNumber(int number = -1)
	{
		if (number != -1)
		{
			this.MoYuCount = number;
		}
		this.SceneInfos[0].text = string.Empty;
		this.SceneInfos[1].text = string.Empty;
		this.LabMoYuMingCi.text = string.Empty;
		this.LabMoYuName.text = string.Empty;
		this.LabMoYuShangHai.text = string.Empty;
		this.LabMyData.text = string.Empty;
		this.SceneInfos[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			Global.GetLang("                 魔域龙穴BOSS剩余：") + this.MoYuCount
		});
	}

	public void ShowTeamTiShi()
	{
		this.imgHongDian.gameObject.SetActive(true);
	}

	public void HideTeamTiShi()
	{
		this.imgHongDian.gameObject.SetActive(false);
	}

	public void SetFubenArrow(bool beShow)
	{
		this.fuBenTaskHand.SetActive(beShow);
	}

	internal void NoticeMiDongSideScore(CompMineSideScore data)
	{
		if (data != null && null != this.mShiLiMDongTaskScene)
		{
			this.mShiLiMDongTaskScene.NoticeRefreshData(data);
		}
	}

	internal void NoticeRebornBossScore(RebornBossScoreData data)
	{
		if (null != this._RebirthBossTaskScene)
		{
			this._RebirthBossTaskScene.RefreshBossLife(data.LeftLifePct);
			if (this._RebirthBossTaskScene.FirstTimeEnterMap)
			{
				this._RebirthBossTaskScene.FirstTimeEnterMap = false;
				if (0 >= this._RebirthBossTaskScene.BoosLife)
				{
					this._RebirthBossTaskScene.ShowRankInf = false;
				}
				else
				{
					this._RebirthBossTaskScene.ShowRankInf = true;
				}
			}
			this._RebirthBossTaskScene.RefreshMyRankInf(string.Concat(new object[]
			{
				Global.GetLang("我的排行："),
				data.SelfRankNum,
				"                        ",
				data.SelfDamagePct,
				"%"
			}));
			string text = string.Empty;
			string text2 = string.Empty;
			int[] array = new int[5];
			if (data != null && data.rankList != null)
			{
				for (byte b = 0; b < 5; b += 1)
				{
					if ((int)b < data.rankList.Count)
					{
						text = text + data.rankList[(int)b].RoleName + "\n";
						text2 = text2 + data.rankList[(int)b].DamagePct + "%\n";
						array[(int)b] = data.rankList[(int)b].PtID;
					}
					else
					{
						text = text + Global.GetLang("暂无") + "\n";
					}
				}
			}
			else
			{
				for (byte b2 = 0; b2 < 5; b2 += 1)
				{
					text = text + Global.GetLang("暂无") + "\n";
				}
			}
			this._RebirthBossTaskScene.RefreshRankInfplatformIds(array);
			this._RebirthBossTaskScene.RefreshRankInf(text, text2);
			DateTime minValue = DateTime.MinValue;
			if (!string.IsNullOrEmpty(data.NextTime))
			{
				DateTime.TryParse(data.NextTime, ref minValue);
			}
			this._RebirthBossTaskScene.RefreshBossRefreshTime(minValue);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL Body;

	public GButton Bak;

	public UISprite mBakSprite;

	public GameObject _Anim;

	public GameObject AnimTaskHand;

	public GameObject AnimMainTaskRect;

	public GameObject AnimOtherTaskRect;

	public Transform AnimMainTaskRectTran;

	public Transform AnimOtherTaskRectTran;

	public UIPanel[] Panels;

	public TextBlock TaskInfo;

	public TextBlock MainTaskInfo;

	public TextBlock OtherTaskInfo;

	public TextBlock OldTaskInfo;

	public CAnimation NewTaskAnim;

	public CAnimation TaskOKAnim;

	public UIButton SwitchIcon;

	public Transform SwitchIconTrans;

	public UIButton SwitchIcon1;

	public Transform SwitchIconTrans1;

	public GButton GoIcon;

	public GButton MainGoIcon;

	public GButton OtherGoIcon;

	public GButton NextIcon;

	private UISprite SwitchIconBak;

	public UISprite HuanyingRed;

	public UISprite HuanyingBlue;

	public GameObject Animator1;

	public GameObject Animator2;

	public UILabel[] staticText;

	public GameObject Animator_1;

	public GameObject Animator_2;

	public GameObject Animator_3;

	public GImgProgressBar FubenProcessBar;

	[SerializeField]
	public TextBlock[] SceneInfos;

	public TextBlock SceneInfosTitle;

	public TextBlock LabMoYuMingCi;

	public TextBlock LabMoYuName;

	public TextBlock LabMoYuShangHai;

	public TextBlock LabMyData;

	public TextBlock[] SceneInfoName;

	public TextBlock[] SceneInfoZiYuan;

	public TextBlock[] SceneInfoBiFen;

	public GButton arrow;

	public GButton taofaButton;

	public GButton richangButton;

	public UISprite TitleBack;

	public TaskBoxShiLiMini shiLiTaskWindow;

	public TaskBoxShiLiBattleMini shiLiBattleWindow;

	public ShiLiMDongTaskScene mShiLiMDongTaskScene;

	public GButton btnTeam;

	public GameObject imgHongDian;

	public UILabel _ShiLiMiDongTaskInfLabel;

	public GameObject fuBenTaskHand;

	[SerializeField]
	private RebirthBossTaskScene _RebirthBossTaskScene;

	private string OldMainTaskInfoStr;

	private bool LevelLimited;

	private bool TaskCompleted;

	public bool TaskCompletedEX;

	private bool arrowDisplay;

	private bool dragEnd = true;

	private bool m_NormalSceneTaskBtnIsShow = true;

	private bool m_OtherSceneTaskBtnIsShow = true;

	public GameObject mOtherTaskItemObj;

	public UIPanel mTaskPanel;

	public UIDraggablePanel mTaskDraggablePanel;

	public ListBox mTaskListBox;

	private ObservableCollection _ItemCollection;

	private GameObject AnimMainTaskRectParent;

	public TextBlock mLblTeamCompeteGuanZhan;

	private int TargetType = -1;

	public int TaskID = -1;

	private int NpcID = -1;

	public int MapCode = -1;

	public int ToPosX = -1;

	public int ToPosY = -1;

	public int RoadMainTaskID = -1;

	public int RoadOtherTaskID = -1;

	private int CanTeleport = -1;

	private int MoYuCount;

	public GButton btnRenwu;

	private int _ActiveTaskClass = 8;

	private int CurrentMapCode = -1;

	private SceneUIClasses _SceneUIClass;

	private List<TaskData> sortList = new List<TaskData>();

	private List<TaskData> sortMainTask;

	private List<TaskData> changeZhiXianTask;

	private List<TaskData> sortZhiXianNotCompleteTask;

	private List<TaskData> sortZhiXianCompleteTask;

	private List<TaskData> sortDailyNotCompleteTask;

	private string redColor = "ff0000";

	private List<GameObject> mCacheTaskItem = new List<GameObject>();

	private static Vector3 TextPosLeft = new Vector3(-200f, -63f, 0f);

	private static Vector3 TextPosMiddle = new Vector3(-10f, -63f, 0f);

	private static Vector3 TextPosRight = new Vector3(190f, -63f, 0f);

	private static Vector3 AnimPosFrom = new Vector3(480f, -132f, 0f);

	private static Vector3 AnimPosTo = new Vector3(480f, -78f, 0f);

	private DispatcherTimer UITimer;

	private SceneTaskInfo[] Targets;

	public int RemainMonsters;

	public int CurrentGroup;

	public int TotalGroup;

	public int CountDownTime;

	private bool fisrt = true;

	public static int ShuiJingHuanJingNum = 0;

	public static int ArmyCaiJiNum = 0;

	private int runNum = -1;

	private int round = -1;

	private int totalRound = -1;
}
