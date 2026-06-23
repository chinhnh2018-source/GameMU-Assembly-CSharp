using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ActivityHint : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(this._Bak.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) && PlayZone.GlobalPlayZone.GameFubenBoxMini != null)
			{
				PlayZone.GlobalPlayZone.GameFubenBoxMini.ShowMessageText();
				return;
			}
			if (Global.IsOperateUnPermitInKuaFuMapCheck(true, false))
			{
				return;
			}
			int id = -1;
			if (this.ID == 1)
			{
				id = 301;
			}
			else if (this.ID == 2)
			{
				id = 302;
			}
			else if (this.ID == 3)
			{
				id = 1001;
			}
			else if (this.ID == 4)
			{
				id = 105;
			}
			else if (this.ID == 5)
			{
				id = 1002;
			}
			else if (this.ID == 8 || this.ID == 16)
			{
				int trigger = 0;
				int param = 0;
				int param2 = 0;
				if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShuiJingHuanJing, ref trigger, ref param, ref param2))
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.ShuiJingHuanJing, trigger, param, param2, true);
				}
				else
				{
					id = 1300;
				}
			}
			else if (this.ID == 9)
			{
				if (string.IsNullOrEmpty(Global.Data.roleData.BHName))
				{
					Super.HintMainText(Global.GetLang("需要加入任意战盟后才可参与"), 10, 3);
				}
				else
				{
					id = 208;
				}
			}
			else
			{
				if (this.ID == 10)
				{
					string[] buttons = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					string lang = Global.GetLang("大爷~罗兰城主携软妹子邀请您参加罗兰城战庆功宴，要不要来参加？\n点击[确定]将传送到勇者大陆的宴会地点。");
					Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
					{
						if (e1.ID == 0)
						{
							XElement gameResXml = Global.GetGameResXml("Config/GleeFeastAward.Xml");
							List<XElement> xelementList = Global.GetXElementList(gameResXml, "YanHui");
							GameInstance.Game.SpriteTaskTransport(Global.GetXElementAttributeInt(xelementList[0], "MapCode"), Global.GetXElementAttributeInt(xelementList[0], "X"), Global.GetXElementAttributeInt(xelementList[0], "Y"), 0);
						}
					}, buttons);
					return;
				}
				if (this.ID == 11)
				{
					id = 1370;
				}
				else if (this.ID == 12 || this.ID == 13)
				{
					id = 1370;
				}
				else if (this.ID == 14 || this.ID == 15)
				{
					id = 1370;
				}
				else if (this.ID == 17)
				{
					id = 1450;
				}
				else if (this.ID == 18 || this.ID == 19)
				{
					id = 1370;
				}
				else if (this.ID == 23 || this.ID == 24)
				{
					id = 1535;
				}
			}
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = id
			});
		};
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(this.Timer_Tick());
	}

	protected IEnumerator Timer_Tick()
	{
		for (;;)
		{
			if (this._ActivityTime != null && !string.IsNullOrEmpty(this.PartenString))
			{
				TimeSpan ts = this._ActivityTime.DateTimeRight - Global.GetCorrectDateTime();
				if (ts.TotalSeconds >= 0.0)
				{
					this._Text.Text = this.PartenString + UIHelper.FormatSecs1((long)ts.TotalSeconds, string.Empty);
					if (!this._Bak.gameObject.activeSelf)
					{
						this._Bak.gameObject.SetActive(true);
					}
				}
				else
				{
					this._Text.Text = string.Empty;
					if (this._Bak.gameObject.activeSelf)
					{
						this._Bak.gameObject.SetActive(false);
					}
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
		yield break;
	}

	public void UpdateActivityInfo(int id, string title, ActivityTime activityTime)
	{
		this.OpenActicityTipWindow(id);
		this.ID = id;
		this.Title = title;
		this._ActivityTime = activityTime;
		TimeSpan timeSpan = this._ActivityTime.DateTimeRight - this._ActivityTime.DateTimeLeft;
		if (id == 17)
		{
			this.PartenString = string.Format(Global.GetLang("[{0}]{{83f569}}  活动倒计时  "), title);
			return;
		}
		if (id == 8 || id == 16)
		{
			this.PartenString = string.Format(Global.GetLang("[{0}]{{83f569}}  双倍剩余时间  "), title);
		}
		else if (timeSpan.Minutes <= 5)
		{
			this.PartenString = string.Format(Global.GetLang("[{0}]{{83f569}}  入场倒计时  "), title);
		}
		else
		{
			this.PartenString = string.Format(Global.GetLang("[{0}]{{83f569}}  活动倒计时  "), title);
		}
	}

	private void OpenActicityTipWindow(int id)
	{
		if (this.dialogBoxWindow != null || this.isOpened)
		{
			return;
		}
		string message = null;
		string name = null;
		int linkID = this.GetWindowLinkID(id, out message, out name);
		if (linkID > 0)
		{
			this.isOpened = true;
			this.dialogBoxWindow = Super.ShowDialogBox(this.Container, 1, message, 0, 0, 20000, name, "确定", "取消");
			this.dialogBoxWindow.transform.localPosition = new Vector3(this.dialogBoxWindow.transform.localPosition.x, 200f, this.dialogBoxWindow.transform.localPosition.z);
			this.dialogBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int dialogBoxReturn = (s1 as NoTitleWindow).DialogBoxReturn;
				Super.CloseNoTitleWindow(Super.MainWindowRoot, s1 as NoTitleWindow);
				if (dialogBoxReturn == 0)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = linkID
					});
				}
				this.dialogBoxWindow = null;
				return true;
			};
		}
	}

	private int GetWindowLinkID(int ID, out string describe, out string title)
	{
		describe = null;
		title = null;
		XElement gameResXml = Global.GetGameResXml("Config/Activity/ActivityTip.Xml");
		XElement xelement = Global.GetXElement(gameResXml, "Tip", "ID", ID.ToString());
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "List");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Hint");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Intro");
		if (xelementAttributeInt2 == 1)
		{
			title = xelementAttributeStr;
			describe = xelementAttributeStr2;
			return xelementAttributeInt;
		}
		return -1;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int ID;

	private string Title;

	private string PartenString = string.Empty;

	private ActivityTime _ActivityTime;

	public TextBlock _Text;

	public UISprite _Bak;

	private NoTitleWindow dialogBoxWindow;

	private bool isOpened;
}
