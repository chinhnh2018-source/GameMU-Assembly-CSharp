using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZhiXianTaskPart : UserControl
{
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

	public ObservableCollection AwardItemCollection
	{
		get
		{
			return this._AwardItemCollection;
		}
		set
		{
			this._AwardItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this._Submit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mCurrentTaskData == null)
			{
				return;
			}
			this.ZhiXianBodyClick(this.mCurrentTaskData);
		};
	}

	private bool ZhiXianBodyClick(TaskData zhiXianTaskData)
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
			this.OnClose(this, null);
			return true;
		}
		if (zhiXianTaskData.zhiXianLinkID > 0 && !Super.JugeChengJiuComplete(taskXmlNodeByID))
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = zhiXianTaskData.zhiXianLinkID
			});
			this.OnClose(this, null);
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
		if (zhiXianTaskData.zhiXianTargetType == 10001)
		{
			GameInstance.Game.SpriteFindBiaoChe();
			this.OnClose(this, null);
			return true;
		}
		if (zhiXianTaskData.zhiXianTargetType == 10002)
		{
			this.OnClose(this, null);
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
				this.OnClose(this, null);
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
				this.OnClose(this, null);
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
				this.OnClose(this, null);
			}
		}
		else if (zhiXianTaskData.zhiXianTargetType == 3)
		{
			int num = (this.NpcID != 60900) ? 0 : 30;
			Global.Data.GameScene.AutoFindRoad(zhiXianMapCode, pos, 120 + num, ExtActionTypes.EXTACTION_NPCDLG);
			if (0 < this.CanTeleport && Super.CanTransport(zhiXianMapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport2(zhiXianTaskData.DoingTaskID);
				this.OnClose(this, null);
			}
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(zhiXianMapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
			if (0 < this.CanTeleport && Super.CanTransport(zhiXianMapCode, true, true))
			{
				GameInstance.Game.SpriteTaskTransport2(zhiXianTaskData.DoingTaskID);
				this.OnClose(this, null);
			}
		}
		return true;
	}

	private void InitValue()
	{
		PlayZone.GlobalPlayZone._ZhiXianTaskPart = this;
		this.ItemCollection = this.mItemListBox.ItemsSource;
		this.mItemListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject lastSelectedItem = this.mItemListBox.LastSelectedItem;
			if (lastSelectedItem != null)
			{
				ZhiXianTaskItem component = lastSelectedItem.GetComponent<ZhiXianTaskItem>();
				component.Selected = false;
			}
			GameObject selectedItem = this.mItemListBox.SelectedItem;
			if (null != selectedItem)
			{
				ZhiXianTaskItem component2 = selectedItem.GetComponent<ZhiXianTaskItem>();
				if (component2.mTaskData != null)
				{
					this.mCurrentTaskData = component2.mTaskData;
				}
				if (component2.taskVo != null)
				{
					component2.Selected = true;
					this._TaskTitle.Text = component2.taskVo.Title;
					int num = 0;
					string otherTaskContent = this.GetOtherTaskContent(component2.mTaskData, out num, -1, -1, true);
					this._TaskDesc.Text = otherTaskContent;
				}
			}
		};
	}

	public void InitPartData()
	{
		if (Global.Data.roleData.TaskDataList == null || Global.Data.roleData.TaskDataList.Count <= 0)
		{
			return;
		}
		List<TaskData> list = Global.Data.roleData.TaskDataList.FindAll((TaskData result) => result.TaskClass == 1);
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				TaskData taskData = list[i];
				GameObject gameObject = NGUITools.AddChild(this.mItemListBox.gameObject, this.mTaskItem);
				gameObject.SetActive(true);
				ZhiXianTaskItem component = gameObject.GetComponent<ZhiXianTaskItem>();
				component.Selected = false;
				int num = 0;
				string otherTaskContent = this.GetOtherTaskContent(taskData, out num, -1, -1, true);
				component.InitData(otherTaskContent, taskData);
				this.ItemCollection.AddNoUpdate(gameObject);
			}
			if (this.ItemCollection.Count > 0)
			{
				GameObject at = this.ItemCollection.GetAt(0);
				this.mItemListBox.SelectedIndex = 0;
				if (null != at)
				{
					ZhiXianTaskItem component2 = at.GetComponent<ZhiXianTaskItem>();
					if (component2.mTaskData != null)
					{
						this.mCurrentTaskData = component2.mTaskData;
					}
					if (component2.taskVo != null)
					{
						component2.Selected = true;
						this._TaskTitle.Text = component2.taskVo.Title;
						int num2 = 0;
						string otherTaskContent2 = this.GetOtherTaskContent(component2.mTaskData, out num2, -1, -1, true);
						this._TaskDesc.Text = otherTaskContent2;
					}
				}
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("暂无支线数据"), 10, 3);
		}
	}

	public void NotifySubmitResult(int ret, int npcID, int taskID)
	{
		Super.HintMainText(Global.GetLang("待处理"), 10, 3);
	}

	public void RefreshTask(int taskID = -1, bool newTask = false)
	{
		Super.HintMainText(Global.GetLang("待处理"), 10, 3);
	}

	public void RefreshAll(TaskAwardsData TaskAwards)
	{
		Super.HintMainText(Global.GetLang("待处理"), 10, 3);
		this.RefreshTaskAwardsData(TaskAwards);
	}

	public void RefreshTaskAwardsData(TaskAwardsData awardsData)
	{
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		TaskAwardsData taskAwardsData2 = new TaskAwardsData();
		this._AwardsList.ItemsSource.Clear();
		this._AwardsGoodsList.ItemsSource.Clear();
		taskAwardsData.Experienceaward = awardsData.Experienceaward;
		taskAwardsData.YinLiangaward = awardsData.YinLiangaward;
		taskAwardsData.Moneyaward = awardsData.Moneyaward;
		taskAwardsData.XingHunaward = awardsData.XingHunaward;
		taskAwardsData.BindYuanBaoaward = awardsData.BindYuanBaoaward;
		UIHelper.AddAwardData(this._AwardsList.ItemsSource, taskAwardsData, "CTextAwards2");
		taskAwardsData2.BindYuanBaoaward = (int)ConfigSystemParam.GetSystemParamIntByName("PriceTaskAward");
		this.AddAwardGoods(this._AwardsGoodsList.ItemsSource, awardsData);
	}

	protected void AddAwardGoods(ObservableCollection ItemCollection, TaskAwardsData taskAwards)
	{
		List<GoodsData> viewTaskInfoGoodsDataList = new List<GoodsData>();
		UIHelper.ParseAwardsItemList(taskAwards.TaskawardList, ref viewTaskInfoGoodsDataList);
		UIHelper.ParseAwardsItemList(taskAwards.OtherTaskawardList, ref viewTaskInfoGoodsDataList);
		Super.GData.ViewTaskInfoGoodsDataList = viewTaskInfoGoodsDataList;
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				UIHelper.AddGoodsIcon2(ItemCollection, Super.GData.ViewTaskInfoGoodsDataList[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	public string GetOtherTaskContent(TaskData taskData, out int completState, int taskType = -1, int triggerType = -1, bool BshowDialyTaskAertPatr = true)
	{
		completState = 0;
		if (taskData == null)
		{
			return null;
		}
		string result = null;
		if (taskData != null)
		{
			taskData.RoadOtherTaskId = taskData.DoingTaskID;
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskData.DoingTaskID);
			string title = taskXmlNodeByID.Title;
			int taskClass = taskXmlNodeByID.TaskClass;
			string taskClassName = Super.GetTaskClassName(taskClass);
			taskData.IsComplete = Super.JugeTaskGuanLianChengJiu(taskXmlNodeByID);
			bool flag = Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskData.DoingTaskVal1);
			bool flag2 = Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, taskData.DoingTaskVal2);
			string text = string.Empty;
			int limitLevel = taskXmlNodeByID.LimitLevel;
			int limitZhuanSheng = taskXmlNodeByID.LimitZhuanSheng;
			if (!UIHelper.AvalidLevel(limitLevel, limitZhuanSheng, false))
			{
				text += string.Format("[" + this.redColor + "]" + Global.GetLang("等级达到{0}后[-]\r\n"), UIHelper.FormatLevelLimit(limitLevel, limitZhuanSheng));
				taskData.IsLevelLimited = false;
			}
			else
			{
				taskData.IsLevelLimited = true;
			}
			bool flag3 = true;
			if (!flag || !flag2 || !taskData.IsComplete)
			{
				bool flag4 = false;
				int taketime = taskXmlNodeByID.Taketime;
				if (taketime > 0 && Global.GetCorrectLocalTime() - taskData.AddDateTime >= (long)(taketime * 1000))
				{
					flag4 = true;
					text += StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
					{
						title,
						Global.GetColorStringForNGUIText(new object[]
						{
							this.redColor,
							Global.GetLang("【失败】")
						})
					});
				}
				string pubStartTime = taskXmlNodeByID.PubStartTime;
				string pubEndTime = taskXmlNodeByID.PubEndTime;
				if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
				{
					double num = (double)Global.GetCorrectLocalTime();
					double num2 = (double)Global.SafeConvertToTicks(pubStartTime);
					double num3 = (double)Global.SafeConvertToTicks(pubEndTime);
					if (num < num2 || num > num3)
					{
						flag4 = true;
						text += StringUtil.substitute(Global.GetLang("{0}{1}"), new object[]
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
				if (flag4)
				{
					result = text;
					flag3 = false;
					completState = -1;
				}
			}
			if (flag3)
			{
				if (!flag || !flag2 || !taskData.IsComplete)
				{
					if (!flag)
					{
						text += this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID, 1);
					}
					else if (!flag2)
					{
						text += this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID, 2);
					}
					else if (!taskData.IsComplete)
					{
						text += this.GetZhiXianTaskInfoPartStr(taskData, taskXmlNodeByID, 0);
					}
					completState = 0;
				}
				else
				{
					text += Super.GetTaskDestNPCDesc(taskXmlNodeByID, true);
					if (text.Length > 0)
					{
						string taskDestNPCName = Super.GetTaskDestNPCName(taskXmlNodeByID);
						if (string.Empty != taskDestNPCName)
						{
							int num4 = -1;
							int num5 = -1;
							int num6 = -1;
							Super.GetTaskDestNPCID(taskXmlNodeByID, ref num4, ref num5, ref num6);
							this.SetTargetPos(num5, taskData.DoingTaskID, num6, num4, -1, -1, -1);
							taskData.zhiXianTargetType = num5;
							taskData.zhiXianNpcID = num6;
							taskData.zhiXianMapCode = num4;
						}
					}
					completState = 1;
				}
				result = text;
			}
		}
		return result;
	}

	private string GetZhiXianTaskInfoPartStr(TaskData taskData, TaskVO taskVO, int TargetID)
	{
		string text = string.Empty;
		string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskData.DoingTaskVal1, TargetID);
		if (taskData.IsLevelLimited)
		{
			text = Super.GetTaskTargetDesc(taskVO, TargetID, taskData.IsLevelLimited);
		}
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
					this.SetTargetPos(num2, taskData.DoingTaskID, num3, num, -1, -1, -1);
					taskData.zhiXianTargetType = num2;
					taskData.zhiXianNpcID = num3;
					taskData.zhiXianMapCode = num;
				}
				else
				{
					this.SetTargetPos(num2, taskData.DoingTaskID, num3, num, num4, num5, taskVO.LinkID);
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

	private void SetTargetPos(int targetType, int taskID, int targetID, int mapCode, int toPosX = -1, int toPosY = -1, int linkID = -1)
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

	protected void OnClose(object sender, EventArgs e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, null);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	protected override void OnDestroy()
	{
		PlayZone.GlobalPlayZone._ZhiXianTaskPart = this;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock _TaskTitle;

	public TextBlock _TaskDesc;

	public GButton _Submit;

	public ListBox _AwardsList;

	public ListBox _AwardsGoodsList;

	public ListBox _FinishAwardsList;

	public GameObject mTaskItem;

	public ListBox mItemListBox;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _AwardItemCollection;

	private TaskData mCurrentTaskData;

	private int TargetType = -1;

	public int TaskID = -1;

	private int NpcID = -1;

	public int MapCode = -1;

	public int ToPosX = -1;

	public int ToPosY = -1;

	public int linkID = -1;

	private int CanTeleport = -1;

	private string redColor = "ff0000";
}
