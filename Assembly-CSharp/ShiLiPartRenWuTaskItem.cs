using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ShiLiPartRenWuTaskItem : UserControl
{
	public TaskData TaskData
	{
		get
		{
			return this.m_taskData;
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblWordNanDu.text = Global.GetLang("难度 :");
		this.lblWordNeiRong.text = Global.GetLang("任务 :");
		this.lblWordJiangLi.text = Global.GetLang("奖励 :");
		this.btnLingQu.Text = Global.GetLang("领取");
		this.lblNeiRong.maxLineCount = 0;
		this.lblWordNanDu.pivot = 5;
		this.lblWordNanDu.transform.localPosition = new Vector3(-60f, this.lblWordNanDu.transform.localPosition.y, this.lblWordNanDu.transform.localPosition.z);
		this.lblWordNeiRong.pivot = 5;
		this.lblWordNeiRong.transform.localPosition = new Vector3(-60f, this.lblWordNeiRong.transform.localPosition.y, this.lblWordNeiRong.transform.localPosition.z);
		this.lblWordJiangLi.pivot = 5;
		this.lblWordJiangLi.transform.localPosition = new Vector3(-60f, this.lblWordJiangLi.transform.localPosition.y, this.lblWordJiangLi.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnLingQuJingLi.Invoke(this.m_taskData);
		};
	}

	public void InitTask(TaskData task, TaskVO taskVO)
	{
		try
		{
			this.m_taskData = task;
			string text = Super.GetTaskTargetDesc(taskVO, 1, true);
			string taskTargetNum = Super.GetTaskTargetNum(taskVO, task.DoingTaskVal1, 1);
			string text2 = (taskVO.TaskClass != 0) ? string.Empty : Global.GetLang("[主]");
			string text3 = string.Empty;
			if (taskVO.TaskClass == 9)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff37f7",
					taskVO.Title
				});
			}
			else if (taskVO.TaskClass == 8)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					text2 + taskVO.Title
				});
			}
			else
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					text2 + taskVO.Title
				});
			}
			text = text.Replace(text3 + "\n", string.Empty);
			text = StringUtil.substitute("{0}{1}", new object[]
			{
				text,
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					taskTargetNum
				})
			});
			this.lblNeiRong.text = text;
			this.imgLingQu.gameObject.SetActive(false);
			bool isEnabled = Super.JugeTaskComplete(taskVO, task.DoingTaskVal1, task.DoingTaskVal2);
			this.btnLingQu.isEnabled = isEnabled;
		}
		catch
		{
			this.lblNeiRong.text = string.Empty;
		}
		this.lblTaskName.text = taskVO.Title;
		this.lblJunXian.text = taskVO.AwardCompFeast.ToString();
		this.lblGongXian.text = taskVO.AwardCompHonor.ToString();
		this.ShowStars(taskVO.Star);
		this._AwardsGoodsList.ItemsSource.Clear();
		this.AddAwardGoods(this._AwardsGoodsList.ItemsSource, task.TaskAwards);
	}

	public void InitCompleteTask(TaskVO taskVO)
	{
		try
		{
			this.m_taskData = null;
			string text = Super.GetTaskTargetDesc(taskVO, 1, true);
			string taskTargetNum = Super.GetTaskTargetNum(taskVO, taskVO.TargetNum1, 1);
			string text2 = (taskVO.TaskClass != 0) ? string.Empty : Global.GetLang("[主]");
			string text3 = string.Empty;
			if (taskVO.TaskClass == 9)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff37f7",
					taskVO.Title
				});
			}
			else if (taskVO.TaskClass == 8)
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					text2 + taskVO.Title
				});
			}
			else
			{
				text3 = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					text2 + taskVO.Title
				});
			}
			text = text.Replace(text3 + "\n", string.Empty);
			text = StringUtil.substitute("{0}{1}", new object[]
			{
				text,
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					taskTargetNum
				})
			});
			this.lblNeiRong.text = text;
			this.SetGotJingLi();
		}
		catch
		{
			this.lblNeiRong.text = string.Empty;
		}
		this.lblTaskName.text = taskVO.Title;
		this.lblJunXian.text = taskVO.AwardCompFeast.ToString();
		this.lblGongXian.text = taskVO.AwardCompHonor.ToString();
		this.ShowStars(taskVO.Star);
	}

	public void SetGotJingLi()
	{
		this.btnLingQu.gameObject.SetActive(false);
		this.imgLingQu.gameObject.SetActive(true);
	}

	protected void AddAwardGoods(ObservableCollection ItemCollection, TaskAwardsData taskAwards)
	{
		List<GoodsData> list = new List<GoodsData>();
		UIHelper.ParseAwardsItemList(taskAwards.TaskawardList, ref list);
		UIHelper.ParseAwardsItemList(taskAwards.OtherTaskawardList, ref list);
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				UIHelper.AddGoodsIcon2(ItemCollection, list[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	private void ShowStars(int num)
	{
		if (num < 0)
		{
			num = 0;
		}
		if (num > 5)
		{
			num = 5;
		}
		if (this.lstStars.Count < 5)
		{
			MUDebug.LogError<string>(new string[]
			{
				"星级列表个数错误"
			});
			return;
		}
		for (int i = 0; i < num; i++)
		{
			this.lstStars[i].SetActive(true);
		}
		for (int j = num; j < 5; j++)
		{
			this.lstStars[j].SetActive(false);
		}
	}

	public Action<TaskData> OnLingQuJingLi;

	public UILabel lblTaskName;

	public UILabel lblWordNanDu;

	public UILabel lblWordNeiRong;

	public UILabel lblNeiRong;

	public UILabel lblWordJiangLi;

	public UILabel lblJunXian;

	public UILabel lblGongXian;

	public GButton btnLingQu;

	public UISprite imgLingQu;

	public ListBox _AwardsGoodsList;

	public List<GameObject> lstStars;

	private TaskData m_taskData;
}
