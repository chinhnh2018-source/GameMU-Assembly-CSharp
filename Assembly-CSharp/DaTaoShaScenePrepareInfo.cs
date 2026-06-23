using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class DaTaoShaScenePrepareInfo : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.mLblCountDown.Text = string.Empty;
		this.mBtnInvite.spriteName = this.normalName;
		this.cfgCountDown = (int)ConfigSystemParam.GetSystemParamIntByName("MailTime");
		UIEventListener.Get(this.mBtnInvite.gameObject).onClick = delegate(GameObject s)
		{
			if (this.memberCount >= 5)
			{
				return;
			}
			if (this.IsCountingDown)
			{
				return;
			}
			this.countTimes = this.cfgCountDown;
			this.IsCountingDown = true;
			GameInstance.Game.SendInviteDaTaoShaTeamMembers();
			DateTime dateTime = DateTime.Now.AddSeconds((double)this.countTimes);
			this.remainderTicks = (long)(dateTime - DateTime.Now).TotalSeconds;
			this.InviteActivityCountDown();
			this.mBtnInvite.spriteName = this.disableName;
			this.mLblCountDown.Text = this.countTimes.ToString();
		};
		if (Global.Data != null && Global.Data.roleData != null)
		{
			if (Global.Data.roleData.ZhanDuiZhiWu == 1)
			{
				this.mBtnInvite.gameObject.SetActive(true);
				this.mLblCountDown.gameObject.SetActive(true);
			}
			else
			{
				this.mBtnInvite.gameObject.SetActive(false);
				this.mLblCountDown.gameObject.SetActive(false);
			}
		}
	}

	private void InviteActivityCountDown()
	{
		this.InviteActivityUITimer = new DispatcherTimer("DaTaoShaInvitesUITimer");
		this.InviteActivityUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.InviteActivityUITimer.Tick = new DispatcherTimerEventHandler(this.InviteActivityUITimer_Tick);
		this.InviteActivityUITimer.Start();
	}

	protected void InviteActivityUITimer_Tick(object sender, object e)
	{
		if (this.remainderTicks > 0L)
		{
			this.mLblCountDown.Text = this.GetTimeStrBySecEx(this.remainderTicks);
			this.remainderTicks -= 1L;
		}
		else
		{
			this.StopInviteActivityTimer();
			this.countTimes = this.cfgCountDown;
			this.mLblCountDown.Text = string.Empty;
			this.mBtnInvite.spriteName = this.normalName;
			this.remainderTicks = 0L;
		}
	}

	private string GetTimeStrBySecEx(long sec)
	{
		int num = 3600;
		if (sec > (long)num)
		{
			MUDebug.LogError<string>(new string[]
			{
				"请检查时间是否正确"
			});
		}
		long num2 = sec / 60L;
		long num3 = sec % 60L;
		if (num2 > 0L)
		{
			return string.Concat(new object[]
			{
				num2,
				Global.GetLang("分"),
				num3,
				Global.GetLang("秒")
			});
		}
		return num3.ToString();
	}

	private void StopInviteActivityTimer()
	{
		this.IsCountingDown = false;
		if (this.InviteActivityUITimer != null)
		{
			this.InviteActivityUITimer.Tick = null;
			this.InviteActivityUITimer.Stop();
			this.InviteActivityUITimer.Dispose();
			this.InviteActivityUITimer = null;
		}
	}

	private void InitTextInPrefabs()
	{
		this.mLblTitle1.Text = Global.GetLang("名称");
		this.mLblTitle2.Text = Global.GetLang("战队信息");
		this.mLblMemberCount.Text = Global.GetString(new object[]
		{
			Global.GetLang("已进入人数: "),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				"1/5"
			})
		});
	}

	public void InitValue(List<EscapeBattleJoinRoleInfo> datas)
	{
		if (datas != null && datas.Count > 0)
		{
			List<EscapeBattleJoinRoleInfo> list = datas.FindAll((EscapeBattleJoinRoleInfo s) => s.Join);
			if (list != null && list.Count > 0)
			{
				this.memberCount = list.Count;
			}
			else
			{
				this.memberCount = 1;
			}
			this.mLblMemberCount.Text = Global.GetString(new object[]
			{
				Global.GetLang("已进入人数: "),
				(this.memberCount >= 3) ? (this.memberCount + "/5") : Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.memberCount + "/5"
				})
			});
			if (this.items.Count <= 0)
			{
				for (int i = 0; i < 4; i++)
				{
					DaTaoShaScenePrepareInfo.ItemInfo itemInfo = new DaTaoShaScenePrepareInfo.ItemInfo();
					GameObject gameObject = Object.Instantiate<GameObject>(this.ItemObj);
					gameObject.transform.SetParent(this.ItemObj.transform.parent);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					itemInfo.Obj = gameObject;
					itemInfo.SetLblColorByIndex = i + 1;
					this.items.Add(itemInfo);
				}
			}
			if (this.items.Count > 0)
			{
				int num = 0;
				int selfId = 0;
				if (Global.Data != null && Global.Data.roleData != null)
				{
					selfId = Global.Data.roleData.RoleID;
				}
				List<EscapeBattleJoinRoleInfo> list2 = datas.FindAll((EscapeBattleJoinRoleInfo s) => s.RoleID != selfId);
				for (int j = 0; j < list2.Count; j++)
				{
					if (!(this.items[j].Obj == null))
					{
						this.items[j].Obj.SetActive(true);
						DaTaoShaScenePrepareInfo.ItemInfo itemInfo2 = this.items[j];
						itemInfo2.Name = list2[j].Name;
						itemInfo2.Status = ((!list2[j].Join) ? Global.GetLang("未加入") : Global.GetLang("已加入"));
						itemInfo2.SetLabelColor((!list2[j].Join) ? Color.white : Color.green);
						itemInfo2.SetY = num;
						num++;
					}
				}
			}
			return;
		}
		this.memberCount = 1;
		this.mLblMemberCount.Text = Global.GetString(new object[]
		{
			Global.GetLang("已进入人数: "),
			(this.memberCount >= 3) ? (this.memberCount + "/5") : Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				this.memberCount + "/5"
			})
		});
	}

	public override void Destroy()
	{
		base.Destroy();
		this.StopInviteActivityTimer();
	}

	public TextBlock mLblMemberCount;

	public TextBlock mLblCountDown;

	public UISprite mBtnInvite;

	public TextBlock mLblTitle1;

	public TextBlock mLblTitle2;

	public GameObject ItemObj;

	private List<DaTaoShaScenePrepareInfo.ItemInfo> items = new List<DaTaoShaScenePrepareInfo.ItemInfo>();

	private string normalName = "anniu_fasongyaoq";

	private string disableName = "anniu_fasongyaoq02";

	private int cfgCountDown;

	private bool IsCountingDown;

	private int countTimes = 10;

	private DispatcherTimer InviteActivityUITimer;

	private long remainderTicks;

	private int memberCount = 1;

	private class ItemInfo
	{
		public GameObject Obj
		{
			get
			{
				return this.obj;
			}
			set
			{
				if (value != null)
				{
					this.obj = value;
					this.mLblName = value.transform.FindChild("LblName").GetComponent<TextBlock>();
					this.mLblStatus = value.transform.FindChild("LblStatus").GetComponent<TextBlock>();
				}
			}
		}

		public int SetY
		{
			set
			{
				if (this.Obj != null)
				{
					this.Obj.transform.localPosition = new Vector3(this.Obj.transform.localPosition.x, (float)(-50 + value * -22), -1f);
				}
			}
		}

		public int SetLblColorByIndex
		{
			set
			{
				switch (value)
				{
				case 1:
					this.mLblName.textColor = Global.ParseStringColorToUint("#feffce");
					break;
				case 2:
					this.mLblName.textColor = Global.ParseStringColorToUint("#8bf596");
					break;
				case 3:
					this.mLblName.textColor = Global.ParseStringColorToUint("#89e3ff");
					break;
				case 4:
					this.mLblName.textColor = Global.ParseStringColorToUint("#b266ff");
					break;
				}
				this.mLblStatus.textColor = Global.ParseStringColorToUint("#e1cd92");
			}
		}

		public string Name
		{
			set
			{
				this.mLblName.Text = value;
			}
		}

		public string Status
		{
			set
			{
				this.mLblStatus.Text = value;
			}
		}

		public void SetLabelColor(Color c)
		{
			this.mLblStatus.Label.color = c;
		}

		public void Clear()
		{
			this.Name = string.Empty;
			this.Status = string.Empty;
		}

		public void IsShowObj(bool isShow)
		{
			NGUITools.SetActive(this.obj, isShow);
		}

		private TextBlock mLblName;

		private TextBlock mLblStatus;

		private GameObject obj;
	}
}
