using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class NPCDoingTaskPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.thisCtrl = this;
		this._Reward_Title.Text = Global.GetLang("任务奖励");
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		this._Submit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSubmit();
			this._Submit.EnableHint = false;
		};
		if (null != this._BossModal)
		{
			Vector3 localPosition = this._BossModal.transform.localPosition;
			localPosition.z = -500f;
			this._BossModal.transform.localPosition = localPosition;
		}
		this.m_RewardLeft.localPosition = new Vector3(-65f, 30f, 0f);
		this.m_RewardCenter.localPosition = new Vector3(0f, 30f, 0f);
		this.m_RewardRight.localPosition = new Vector3(65f, 30f, 0f);
		this.m_RewardCenter.GetComponent<UISprite>().MakePixelPerfect();
		this._Reward_List.Y = -12.0;
		this._TalkText.FontSize = 16;
		this._TalkText.MaxWidth = 545.0;
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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData(int taskID)
	{
	}

	protected void OnDisable()
	{
		if (null != this._Face)
		{
			this._Face.DestroyImmediateTexture();
		}
	}

	protected void SetState(bool completed, int targetType)
	{
		int num = this.TaskID;
		if (this.NeedZhuanSheng)
		{
			this._Submit.Text = Global.GetLang("立即转生");
		}
		else if (this.mTaskState == NPCDoingTaskPart.TaskStates.None)
		{
			this._Submit.Text = Global.GetLang("立即前往");
		}
		else
		{
			if (completed)
			{
				num = this.NextMainTaskID;
			}
			if (!completed)
			{
				this._Submit.Text = Global.GetLang("立即前往");
			}
			else
			{
				this._Submit.Text = Global.GetLang("领取奖励");
			}
		}
		this._Submit.EnableHint = true;
		this._Submit.isEnabled = (this.LevelLimited || this.NeedZhuanSheng);
	}

	protected void AddAwardGoods(ObservableCollection ItemCollection, TaskAwardsData taskAwards)
	{
		List<GoodsData> viewTaskInfoGoodsDataList = new List<GoodsData>();
		UIHelper.ParseTaskAwardsItemList(taskAwards.TaskawardList, ref viewTaskInfoGoodsDataList);
		UIHelper.ParseTaskAwardsItemList(taskAwards.OtherTaskawardList, ref viewTaskInfoGoodsDataList);
		Super.GData.ViewTaskInfoGoodsDataList = viewTaskInfoGoodsDataList;
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(ItemCollection, Super.GData.ViewTaskInfoGoodsDataList[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	public void RefreshTaskAwardsData(TaskAwardsData taskAwardsData)
	{
		UIHelper.AddAwardData(this._Reward_List.ItemsSource, taskAwardsData, "CTextAwards1");
		this.AddAwardGoods(this._Reward_Goods.ItemsSource, taskAwardsData);
	}

	public override void Destroy()
	{
		if (this.monsterNPCResLoader != null)
		{
			this.monsterNPCResLoader.Stop();
			this.monsterNPCResLoader = null;
		}
		base.Destroy();
	}

	public void GetNewData(int taskID, bool newTask = true)
	{
		this._Reward_Goods.ItemsSource.Clear();
		this._Reward_List.ItemsSource.Clear();
		int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(this.NpcExtensionID);
		this._Face.ShowImage(Global.GetNPCImageString(npcpicCodeByID));
		this._Title.Text = string.Empty;
		this._TalkText.Text = string.Empty;
		this.NextMainTaskID = -1;
		this.TaskID = taskID;
		this.LevelLimited = true;
		this._BossModal.Clear();
		this._GroupBoss.SetActive(false);
		this._GroupNormal.SetActive(true);
		if (this.TaskID != -1)
		{
			float scale;
			this.BossFuBenTask = Global.GetJuQingFuBenBossModalScale(taskID, out scale);
			this.TaskClass = Global.GetTaskClassByID(this.TaskID);
			this.NextMainTaskID = Super.GetNextMainTask(this.TaskID);
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(this.TaskID);
			if (taskXmlNodeByID != null)
			{
				this.LimitZhuanSheng = taskXmlNodeByID.LimitZhuanSheng;
				this.LimitLevel = taskXmlNodeByID.LimitLevel;
				this.NeedZhuanSheng = (this.LimitZhuanSheng > Global.Data.roleData.ChangeLifeCount);
				string text = string.Empty;
				if (!UIHelper.AvalidLevel(this.LimitLevel, this.LimitZhuanSheng, false))
				{
					this.LevelLimited = false;
					text = string.Format(Global.GetLang("等级达到{0}后\r\n"), UIHelper.FormatLevelLimit(this.LimitLevel, this.LimitZhuanSheng));
					GTextBlockOutLine talkText = this._TalkText;
					talkText.htmlText += text;
				}
				this._Title.Text = taskXmlNodeByID.Title;
				if (newTask)
				{
					this.mTaskState = NPCDoingTaskPart.TaskStates.None;
					GameInstance.Game.SpriteGetTaskAwards(this.TaskID);
					List<TalkTextNode> taskTalkTextInfo = Super.GetTaskTalkTextInfo(this.TaskID, "AcceptTalk");
					GTextBlockOutLine talkText2 = this._TalkText;
					talkText2.htmlText += Super.FormatTaskTalkText(taskTalkTextInfo);
					Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务接取UI"), taskID, -1, 1);
					this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 1);
					this.SetState(false, this.TargetType);
				}
				else
				{
					this.mTaskState = NPCDoingTaskPart.TaskStates.Accepted;
					TaskData taskDataByTaskID = Super.GetTaskDataByTaskID(this.TaskID);
					if (taskDataByTaskID == null)
					{
						return;
					}
					this.DbID = taskDataByTaskID.DbID;
					this.RefreshTaskAwardsData(taskDataByTaskID.TaskAwards);
					bool flag = Super.JugeTaskTargetComplete(taskXmlNodeByID, 1, taskDataByTaskID.DoingTaskVal1);
					bool flag2 = Super.JugeTaskTargetComplete(taskXmlNodeByID, 2, taskDataByTaskID.DoingTaskVal2);
					if (flag && flag2)
					{
						this.mTaskState = NPCDoingTaskPart.TaskStates.Complete;
						List<TalkTextNode> taskTalkTextInfo2 = Super.GetTaskTalkTextInfo(this.TaskID, "CompleteTalk");
						GTextBlockOutLine talkText3 = this._TalkText;
						talkText3.htmlText += Super.FormatTaskTalkText(taskTalkTextInfo2);
						Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("任务提交UI"), taskID, 2, 1);
						this.SetState(true, 0);
					}
					else
					{
						this.mTaskState = NPCDoingTaskPart.TaskStates.Accepted;
						List<TalkTextNode> taskTalkTextInfo3 = Super.GetTaskTalkTextInfo(this.TaskID, "DoingTalk");
						GTextBlockOutLine talkText4 = this._TalkText;
						talkText4.htmlText += Super.FormatTaskTalkText(taskTalkTextInfo3);
						if (!flag)
						{
							this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 1);
							this.TargetID = taskXmlNodeByID.TargetNPC1;
						}
						else
						{
							this.TargetType = Super.GetTaskTargetType(taskXmlNodeByID, 2);
							this.TargetID = taskXmlNodeByID.TargetNPC2;
						}
						if (this.BossFuBenTask && !flag)
						{
							this._GroupBoss.SetActive(true);
							this._GroupNormal.SetActive(false);
							this._BossDesc.Text = taskXmlNodeByID.DoingTalk;
							this.monsterNPCResLoader = UIHelper.LoadMonsterRes(this._BossModal, this.TargetID, scale);
							MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.TargetID);
							if (monsterXmlNodeByID != null)
							{
								this._BossName.Text = monsterXmlNodeByID.SName;
							}
							else
							{
								this._BossName.Text = null;
							}
						}
						else
						{
							this._GroupBoss.SetActive(false);
							this._GroupNormal.SetActive(true);
						}
						this.SetState(false, this.TargetType);
					}
				}
			}
		}
	}

	public void CleanUpChildWindows()
	{
		Super.RemoveSystemNaviBox(this.Container, Global.GetLang("任务提交UI"), null);
	}

	private void OnCompleteNow()
	{
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("YuanBaoCompleteTask");
		if (num > 0 && Global.Data.roleData.UserMoney >= num)
		{
			GameInstance.Game.SpriteYuanBaoCompleteTask(this.NpcID, this.TaskID);
			SystemHintWindow.AddTaskGuidHintText(this.TaskID);
		}
	}

	public static void TaskAutoRoad(int taskID)
	{
		Super.PrccessAutoTaskFindRoad(taskID, false, true, false, false);
	}

	private void OnSubmit()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, null);
		}
		if (this.TaskID == -1)
		{
			return;
		}
		if (this.NeedZhuanSheng)
		{
			PlayZone.GlobalPlayZone.ShowZhuanShengPart();
		}
		else if (this.mTaskState == NPCDoingTaskPart.TaskStates.None)
		{
			GameInstance.Game.SpriteNewTask(this.NpcID, this.TaskID);
			SystemHintWindow.AddTaskGuidHintText(this.TaskID);
		}
		else if (this.mTaskState == NPCDoingTaskPart.TaskStates.Accepted)
		{
			Super.PrccessAutoTaskFindRoad(this.TaskID, false, true, false, false);
		}
		else if (this.mTaskState == NPCDoingTaskPart.TaskStates.Complete)
		{
			GameInstance.Game.SpriteCompleteTask(this.NpcID, this.TaskID, this.DbID, this.NeedYuanBao);
			if (this.NextMainTaskID > 0)
			{
				Super.AutoAcceptTask(this.NextMainTaskID, "SourceNPC");
			}
		}
	}

	public bool FindDoingTaskID(int taskID)
	{
		if (Global.Data.roleData.TaskDataList == null)
		{
			return false;
		}
		for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
		{
			if (Global.Data.roleData.TaskDataList[i].DoingTaskID == taskID)
			{
				return true;
			}
		}
		return false;
	}

	public GButton _Close;

	public UISprite _Bak;

	public GButton _Submit;

	public ShowNetImage _Face;

	public ListBox _Reward_Goods;

	public ListBox _Reward_List;

	public TextBlock _Reward_Title;

	public TextBlock _Title;

	public GTextBlockOutLine _TalkText;

	public GameObject _GroupNormal;

	public GameObject _GroupBoss;

	public Transform m_RewardLeft;

	public Transform m_RewardCenter;

	public Transform m_RewardRight;

	public TextBlock _BossName;

	public TextBlock _BossDesc;

	public Modal3DShow _BossModal;

	private bool BossFuBenTask;

	private NPCDoingTaskPart.TaskStates mTaskState;

	private int TaskID = -1;

	private int DbID = -1;

	private int NeedYuanBao;

	private int TargetType = -1;

	private int TaskClass = -1;

	private int TargetID = -1;

	private int LimitZhuanSheng = -1;

	private int LimitLevel = -1;

	private bool NeedZhuanSheng;

	private SpriteSL thisCtrl;

	private int NextMainTaskID = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _NpcID;

	private int _NpcExtensionID;

	private bool LevelLimited = true;

	private MonsterNPCResLoader monsterNPCResLoader;

	internal enum TaskStates
	{
		None,
		Accepted,
		Faild,
		Complete
	}
}
