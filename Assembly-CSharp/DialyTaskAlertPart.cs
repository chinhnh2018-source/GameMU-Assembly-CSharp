using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class DialyTaskAlertPart : UserControl
{
	public static bool CheckTask(int TaskDbID)
	{
		string text = string.Format("{0}{1}", Global.Data.roleData.RoleID, " DialyTaskTaskDbID");
		if (PlayerPrefs.HasKey(text))
		{
			DialyTaskAlertPart.TaskKeyInF = PlayerPrefs.GetString(text);
		}
		if (string.IsNullOrEmpty(DialyTaskAlertPart.TaskKeyInF))
		{
			DialyTaskAlertPart.TaskKeyInF = string.Format("{0}", TaskDbID);
			PlayerPrefs.SetString(text, DialyTaskAlertPart.TaskKeyInF);
			return true;
		}
		if (DialyTaskAlertPart.TaskKeyInF == TaskDbID.ToString())
		{
			return false;
		}
		DialyTaskAlertPart.TaskKeyInF = string.Format("{0}", TaskDbID);
		PlayerPrefs.SetString(text, DialyTaskAlertPart.TaskKeyInF);
		return true;
	}

	public TaskData TaskData
	{
		set
		{
			this.m_TaskData = value;
			if (this.m_TaskData != null)
			{
				this.m_TaskVO = ConfigTasks.GetTaskXmlNodeByID(this.m_TaskData.DoingTaskID);
				DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(this.m_TaskVO.TaskClass);
				int maxDailyTaskNum = Global.GetMaxDailyTaskNum(this.m_TaskVO.TaskClass, dailyTaskData);
				if (dailyTaskData == null)
				{
					this.m_RecNum = 0;
				}
				else
				{
					this.m_RecNum = Math.Min(dailyTaskData.RecNum + 1, maxDailyTaskNum);
				}
				Super.GetTaskDestNPCID(this.m_TaskVO, ref this.mapCode, ref this.npcType, ref this.npcID);
				this.HaveTime = (maxDailyTaskNum > this.m_RecNum);
				this.SetStar(this.m_TaskData.StarLevel);
				TaskStarInfo taskStarInfo = Global.GetTaskStarInfo(this.m_TaskData.StarLevel);
				this.SetEXP((int)((float)this.m_TaskData.TaskAwards.Experienceaward * taskStarInfo.ExpModule));
				if (this._AutoDoubelCheckBox.CheckChanged != null)
				{
					this._AutoDoubelCheckBox.CheckChanged(null, null);
				}
				if (this._AutoFullCheckBox.CheckChanged != null)
				{
					this._AutoFullCheckBox.CheckChanged(null, null);
				}
			}
		}
	}

	public int RecNum
	{
		get
		{
			return this.m_RecNum;
		}
	}

	public bool HaveTime
	{
		set
		{
			this.m_HaveTime = value;
			base.CancelInvoke();
			if (this.m_HaveTime)
			{
				base.InvokeRepeating("TimeGoO", 1f, 1f);
			}
			else
			{
				this._Timelabel.Text = string.Empty;
			}
			this._GetBtn.Text = ((!this.m_HaveTime) ? Global.GetLang("领取") : Global.GetLang("领取并继续"));
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitData();
		this.IntiText();
		this.InitHander();
		this.InitStar();
		this.RefreshTime();
		if (this.m_HaveTime)
		{
			base.CancelInvoke();
			base.InvokeRepeating("TimeGoO", 1f, 1f);
		}
	}

	private void InitHander()
	{
		this._AutoDoubelCheckBox.CheckChanged = delegate(object e, BaseEventArgs s)
		{
			this.m_AutoDoubleExpIsClick = this._AutoDoubelCheckBox.Check;
			PlayerPrefs.SetInt(this.m_AutoDoubleExpKey, (!this.m_AutoDoubleExpIsClick) ? 0 : 1);
			TaskStarInfo taskStarInfo = Global.GetTaskStarInfo((!this.m_AutoFullStarIsClick) ? this.m_TaskData.StarLevel : 5);
			if (this.m_AutoDoubleExpIsClick)
			{
				if (this.m_TaskData != null)
				{
					this.SetEXP((int)((float)this.m_TaskData.TaskAwards.Experienceaward * taskStarInfo.ExpModule * 2f));
				}
			}
			else
			{
				this.SetEXP((int)((float)this.m_TaskData.TaskAwards.Experienceaward * taskStarInfo.ExpModule));
			}
		};
		this._AutoFullCheckBox.CheckChanged = delegate(object e, BaseEventArgs s)
		{
			this.m_AutoFullStarIsClick = this._AutoFullCheckBox.Check;
			PlayerPrefs.SetInt(this.m_AutoFullStarKey, (!this.m_AutoFullStarIsClick) ? 0 : 1);
			TaskStarInfo taskStarInfo = Global.GetTaskStarInfo((!this.m_AutoFullStarIsClick) ? this.m_TaskData.StarLevel : 5);
			if (this.m_TaskData != null)
			{
				this.SetEXP((int)((float)this.m_TaskData.TaskAwards.Experienceaward * taskStarInfo.ExpModule * (float)((!this.m_AutoDoubleExpIsClick) ? 1 : 2)));
			}
			this.SetStar(taskStarInfo.StarLevel);
		};
		this._GetBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.SendTask();
			if (this.Hander != null && Global.GetLang("领取") == this._GetBtn.Text)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this._GOBackBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void SendTask()
	{
		this.ShuaXing();
		int useYuanBao = 0;
		if (this.m_AutoDoubleExpIsClick && this.m_CompleteEveryTaskNeedYuanBao <= Global.Data.roleData.UserMoney)
		{
			useYuanBao = 2;
		}
		Global.SendEvent("1901", Global.GetLang("完成日常任务次数"));
		GameInstance.Game.SpriteCompleteTask(this.npcID, this.m_TaskData.DoingTaskID, this.m_TaskData.DbID, useYuanBao);
		Super.ShowNetWaiting(null);
	}

	private void ShuaXing()
	{
		if (5 > this.m_TaskData.StarLevel && this.m_AutoFullStarIsClick && this.m_TaskStarInfosNeedJinBi <= Global.Data.roleData.Money1 + Global.Data.roleData.Money2)
		{
			Global.SendEvent("1900", Global.GetLang("日常任务刷星次数"));
			GameInstance.Game.SpriteRiChangTaskShuaXing(this.m_TaskData.DoingTaskID, this.m_TaskData.DbID);
		}
	}

	private void InitData()
	{
		this.m_AutoDoubleExpKey = Global.Data.roleData.RoleID + "AutoDoubleExp";
		this.m_AutoFullStarKey = Global.Data.roleData.RoleID + "AutoFullStar";
		if (PlayerPrefs.HasKey(this.m_AutoDoubleExpKey))
		{
			this.m_AutoDoubleExpIsClick = (PlayerPrefs.GetInt(this.m_AutoDoubleExpKey) != 0);
		}
		else
		{
			PlayerPrefs.SetInt(this.m_AutoDoubleExpKey, (!this.m_AutoDoubleExpIsClick) ? 0 : 1);
		}
		if (PlayerPrefs.HasKey(this.m_AutoFullStarKey))
		{
			this.m_AutoFullStarIsClick = (PlayerPrefs.GetInt(this.m_AutoFullStarKey) != 0);
		}
		else
		{
			PlayerPrefs.SetInt(this.m_AutoFullStarKey, (!this.m_AutoFullStarIsClick) ? 0 : 1);
		}
		this._AutoFullCheckBox.isChecked = this.m_AutoFullStarIsClick;
		this._AutoDoubelCheckBox.isChecked = this.m_AutoDoubleExpIsClick;
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("RiChangRenWuAutoGet", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			this.m_Time = Global.SafeConvertToInt32(systemParamByName);
		}
		this.m_TaskStarInfosNeedJinBi = (int)ConfigSystemParam.GetSystemParamIntByName("TaskStarInfosNeedJinBi");
		this.m_CompleteEveryTaskNeedYuanBao = (int)ConfigSystemParam.GetSystemParamIntByName("DoubleExp");
	}

	private void IntiText()
	{
		try
		{
			this._TitleLabel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("任务追踪")
			});
			this._Explain.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"d02929",
				Global.GetLang("注：金币或元宝不足时，自动领取正常奖励")
			});
			this._StarText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("任务星级：")
			});
			this._AwardText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("任务奖励：")
			});
			this._AutoFullText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("自动满星")
			});
			this._AutoDoubelText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("自动双倍")
			});
			this._AutoFullMoneyLabel.Text = "0";
			this._AutoDoubelMoneyLabel.Text = "0";
			this._GetBtn.Text = ((!this.m_HaveTime) ? Global.GetLang("领取") : Global.GetLang("领取并继续"));
			this._GOBackBtn.Text = Global.GetLang("稍后");
			this._AutoDoubelMoneyLabel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				(this.m_CompleteEveryTaskNeedYuanBao > Global.Data.roleData.UserMoney) ? "d02929" : "fdf7dd",
				this.m_CompleteEveryTaskNeedYuanBao.ToString()
			});
			this._AutoDoubelMoneySp.spriteName = "Diamond";
			this._AutoFullMoneyLabel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				((long)this.m_TaskStarInfosNeedJinBi > (long)Global.Data.roleData.Money1 + (long)Global.Data.roleData.Money2) ? "d02929" : "fdf7dd",
				this.m_TaskStarInfosNeedJinBi.ToString()
			});
			this._AutoFullMoneySp.spriteName = "TaskGold";
			this._AutoFullText.Pivot = 5;
			this._AutoFullText.X = -55.0;
			this._AutoDoubelText.Pivot = 5;
			this._AutoDoubelText.X = -50.0;
			this._AutoFullCheckBox.transform.localPosition = new Vector3(-195f, 0f, 0f);
			this._AutoDoubelCheckBox.transform.localPosition = new Vector3(-195f, 0f, 0f);
			this._Explain.MaxWidth = 395.0;
			this._Explain.Pivot = 3;
			this._Explain.X = -190.0;
			this._Timelabel.MaxWidth = 400.0;
			this._Timelabel.Pivot = 3;
			this._Timelabel.X = -190.0;
			this._GetBtn.Label.lineWidth = 100;
			this._AwardText.Pivot = 5;
			this._AwardText.X = -30.0;
			this._AwardTypeSP.transform.localPosition = Vector3.zero;
			this._AwardValue.Pivot = 3;
			this._AwardValue.X = 30.0;
		}
		catch (Exception ex)
		{
		}
	}

	private void InitStar()
	{
		this.m_StarOBC = this._StarListBox.ItemsSource;
		this._StarListBox.cellWidth = 32f;
		for (byte b = 0; b < 5; b += 1)
		{
			UISprite uisprite = Object.Instantiate<UISprite>(this._StarSp);
			if (null != uisprite)
			{
				this.m_StarOBC.AddNoUpdate(uisprite);
				uisprite.transform.localScale = Vector3.one * 24f;
				this.m_StarLst.Add(uisprite);
			}
		}
		this._StarListBox.transform.localPosition = new Vector3(-48f, 0f, 0f);
		NGUITools.SetActive(this._StarSp, false);
	}

	private void SetStar(int star)
	{
		byte b = 0;
		while ((int)b < this.m_StarLst.Count)
		{
			this.m_StarLst[(int)b].spriteName = (((int)b >= star) ? "star1" : "star0");
			b += 1;
		}
	}

	private void SetEXP(int exp)
	{
		this._AwardTypeSP.spriteName = "exp";
		this._AwardValue.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("+{0}", exp)
		});
	}

	private void RefreshTime()
	{
		int num = Mathf.FloorToInt((float)(this.m_Time / 60));
		int num2 = this.m_Time % 60;
		string text;
		if (60 < this.m_Time)
		{
			text = ((9 <= num) ? ((10 <= num2) ? string.Format("{0}:{1}", num, num2) : string.Format("{0}:0{1}", num, num2)) : ((10 <= num2) ? string.Format("0{0}:{1}", num, num2) : string.Format("0{0}:0{1}", num, num2)));
		}
		else
		{
			text = string.Format("{0}", (10 <= num2) ? string.Format("0{0}:{1}", num, num2) : string.Format("0{0}:0{1}", num, num2));
		}
		this._Timelabel.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format("{0}{1}", text, Global.GetLang("后自动领奖励并拾取下一环任务"))
		});
	}

	private void TimeGoO()
	{
		this.m_Time--;
		if (this.m_Time >= 0)
		{
			this.RefreshTime();
		}
		else
		{
			this.TimeOut();
		}
	}

	private void TimeOut()
	{
		base.CancelInvoke();
		if (this.m_HaveTime)
		{
			this.SendTask();
		}
	}

	public static string TaskKeyInF = string.Empty;

	public DPSelectedItemEventHandler Hander;

	public GButton _CloseBtn;

	public UISprite _StarSp;

	public ListBox _StarListBox;

	public TextBlock _StarText;

	public TextBlock _AwardText;

	public UISprite _AwardTypeSP;

	public TextBlock _AwardValue;

	public GCheckBox _AutoFullCheckBox;

	public TextBlock _AutoFullText;

	public UISprite _AutoFullMoneySp;

	public TextBlock _AutoFullMoneyLabel;

	public GCheckBox _AutoDoubelCheckBox;

	public TextBlock _AutoDoubelText;

	public UISprite _AutoDoubelMoneySp;

	public TextBlock _AutoDoubelMoneyLabel;

	public TextBlock _TitleLabel;

	public GButton _GOBackBtn;

	public GButton _GetBtn;

	public TextBlock _Timelabel;

	public TextBlock _Explain;

	private List<UISprite> m_StarLst = new List<UISprite>();

	private ObservableCollection m_StarOBC;

	private bool m_AutoDoubleExpIsClick;

	private bool m_AutoFullStarIsClick = true;

	private string m_AutoDoubleExpKey = string.Empty;

	private string m_AutoFullStarKey = string.Empty;

	private int m_Time = 10;

	private bool m_HaveTime = true;

	private int m_RecNum;

	private TaskData m_TaskData;

	private int mapCode = -1;

	private int npcType = -1;

	private int npcID = -1;

	private TaskVO m_TaskVO;

	private int m_TaskStarInfosNeedJinBi;

	private int m_CompleteEveryTaskNeedYuanBao;
}
