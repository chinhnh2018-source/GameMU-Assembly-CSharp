using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class BattleScoreBox : UserControl
{
	public int KillEnemiesNum
	{
		get
		{
			return this._KillEnemiesNum;
		}
		set
		{
			this._KillEnemiesNum = value;
		}
	}

	public int TopScore
	{
		get
		{
			return this._TopScore;
		}
		set
		{
			this._TopScore = value;
		}
	}

	public int SuiEnemiesNum
	{
		get
		{
			return this._SuiEnemiesNum;
		}
		set
		{
			this._SuiEnemiesNum = value;
		}
	}

	public int TangEnemiesNum
	{
		get
		{
			return this._TangEnemiesNum;
		}
		set
		{
			this._TangEnemiesNum = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		GameInstance.Game.SpriteGetCopyTeamDamageInfo();
		this.btnTitle.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowBody = !this.ShowBody;
			this._MainBossHPDirect.spriteName = ((!this.ShowBody) ? "mainBoxxHPUp" : "MainBossHPDown");
			this.group.SetActive(this.ShowBody);
		};
		this.RefreshSceneScoreInfo();
		if (Super.GData.BattleScoreList != null && Global.GetCorrectLocalTime() < Super.GData.LastUpdateBattleScoreListTicks + 30000L)
		{
			this.NotifyDamageValue(Super.GData.BattleScoreList);
		}
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(8000, StringUtil.substitute("Monsters.Xml", new object[0]));
		XElement xelement = Global.GetXElement(gameMapSettingsXml, "Monster", "Code", "800002");
		if (xelement != null)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "TimePoints");
			DateTime dateTime;
			if (DateTime.TryParse(xelementAttributeStr, ref dateTime))
			{
				this.BossBirthTicks = dateTime.Ticks / 10000L;
			}
		}
	}

	public void SetSceneInfo(int index, string name, double score, string color)
	{
		if (index >= 0 && index < this._Names.Length)
		{
			if (!string.IsNullOrEmpty(name))
			{
				this._Names[index].Text = ColorCode.EncodingText(name, color);
				this._Scores[index].Text = ColorCode.EncodingText(score.ToString(), color);
			}
			else
			{
				this._Names[index].Text = null;
				this._Scores[index].Text = null;
			}
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

	private void OnEnable()
	{
		base.StartCoroutine_Auto(this.TickProc());
	}

	public IEnumerator TickProc()
	{
		while (!this.ShowBossBirthCountDown)
		{
			int time = (int)(this.BossBirthTicks - Global.GetCorrectLocalTime()) / 1000;
			if (time > 0 && time <= 3)
			{
				this.ShowBossBirthCountDown = true;
				SystemHelpPart.Countdown(time, delegate(object s, EventArgs e)
				{
					SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("BOSS出现"), 3);
				}, false);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void RefreshSceneScoreInfo()
	{
		this.SetSceneTaskInfos(5, ColorCode.EncodingText(Global.GetLang("教团 "), "fd010c"), new object[]
		{
			this._SuiEnemiesNum
		});
		this.SetSceneTaskInfos(6, ColorCode.EncodingText(Global.GetLang(" 盟军"), "4997bc"), new object[]
		{
			this._TangEnemiesNum
		});
		this.SetSceneInfo(5, Global.GetLang("我的得分"), (double)this.KillEnemiesNum, (Global.Data.roleData.BattleWhichSide != 1) ? "4997bc" : "fd010c");
	}

	public void NotifyDamageValue(List<RoleDamage> damageList)
	{
		int i = 0;
		foreach (RoleDamage roleDamage in damageList)
		{
			if (i >= 5)
			{
				break;
			}
			if (!this.damageDataDict.ContainsKey(roleDamage.RoleID))
			{
				this.damageDataDict.Add(roleDamage.RoleID, new BattleScoreBox.RoleDamageDataMini());
			}
			if (!string.IsNullOrEmpty(roleDamage.RoleName))
			{
				this.damageDataDict[roleDamage.RoleID].roleName = roleDamage.RoleName;
			}
			this.damageDataDict[roleDamage.RoleID].roleDamage = roleDamage.Damage;
			if (roleDamage.FlagList != null && roleDamage.FlagList.Count > 0 && roleDamage.FlagList[0] == 1)
			{
				this.SetSceneInfo(i, this.damageDataDict[roleDamage.RoleID].roleName, (double)this.damageDataDict[roleDamage.RoleID].roleDamage, "fd010c");
			}
			else
			{
				this.SetSceneInfo(i, this.damageDataDict[roleDamage.RoleID].roleName, (double)this.damageDataDict[roleDamage.RoleID].roleDamage, "4997bc");
			}
			i++;
		}
		while (i < 5)
		{
			this.SetSceneInfo(i, null, 0.0, "fd010c");
			i++;
		}
	}

	public const int BattleMapCode = 8000;

	private Dictionary<int, BattleScoreBox.RoleDamageDataMini> damageDataDict = new Dictionary<int, BattleScoreBox.RoleDamageDataMini>();

	public GameObject group;

	public GButton btnTitle;

	public UISprite _MainBossHPDirect;

	[SerializeField]
	public TextBlock[] SceneInfos;

	public TextBlock[] _Orders;

	public TextBlock[] _Names;

	public TextBlock[] _Scores;

	private bool ShowBody = true;

	public long BossBirthTicks;

	private bool ShowBossBirthCountDown;

	private int _KillEnemiesNum;

	private int _TopScore;

	private int _SuiEnemiesNum;

	private int _TangEnemiesNum;

	public int SideRoleNum0;

	public int SideRoleNum1;

	private class RoleDamageDataMini
	{
		public string roleName = string.Empty;

		public long roleDamage;
	}
}
