using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class ZuduiFubenDamageBox : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.btnTitle.Label.text = Global.GetCurrentMapName();
		this.mapCode = Global.Data.roleData.MapCode;
		this.AddLabels();
		GameInstance.Game.SpriteGetCopyTeamDamageInfo();
		this.btnTitle.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowBody = !this.ShowBody;
			this._MainBossHPDirect.spriteName = ((!this.ShowBody) ? "mainBoxxHPUp" : "MainBossHPDown");
			this.group.SetActive(this.ShowBody);
		};
	}

	public void NotifyDamageValue(List<RoleDamage> damageList)
	{
		if (this.mapCode != Global.Data.roleData.MapCode)
		{
			this.btnTitle.Label.text = Global.GetCurrentMapName();
			this.mapCode = Global.Data.roleData.MapCode;
		}
		int i = 0;
		string text = string.Empty;
		foreach (RoleDamage roleDamage in damageList)
		{
			if (i >= this.roleDamageList.Count)
			{
				break;
			}
			if (!this.damageDataDict.ContainsKey(roleDamage.RoleID))
			{
				this.damageDataDict.Add(roleDamage.RoleID, new ZuduiFubenDamageBox.RoleDamageDataMini());
			}
			if (!string.IsNullOrEmpty(roleDamage.RoleName))
			{
				this.damageDataDict[roleDamage.RoleID].roleName = roleDamage.RoleName;
			}
			this.damageDataDict[roleDamage.RoleID].roleDamage = roleDamage.Damage;
			float num = (float)this.damageDataDict[roleDamage.RoleID].roleDamage;
			if (num >= 10000f)
			{
				num = Mathf.Round(num / 1000f) / 10f;
				text = string.Empty + num + "w";
			}
			else
			{
				text = string.Empty + num;
			}
			if (roleDamage.RoleID == Global.Data.roleData.RoleID)
			{
				this.roleDamageList[i].text = Global.GetLang("{ff0000}伤害：") + text + "{-}";
				this.roleNameList[i].text = "{ff0000}" + this.damageDataDict[roleDamage.RoleID].roleName + "{-}";
			}
			else
			{
				this.roleDamageList[i].text = Global.GetLang("伤害：") + text;
				this.roleNameList[i].text = this.damageDataDict[roleDamage.RoleID].roleName;
			}
			i++;
		}
		while (i < this.roleDamageList.Count)
		{
			this.roleDamageList[i].text = string.Empty;
			this.roleNameList[i].text = string.Empty;
			i++;
		}
	}

	private void AddLabels()
	{
		for (int i = 0; i < 5; i++)
		{
			UILabel uilabel = SpawnManager.Instantiate(this.lblPrefab) as UILabel;
			U3DUtils.AddChild(this.RoleNameGroup, uilabel.gameObject, true);
			uilabel.transform.localScale = new Vector3(16f, 16f, 0f);
			uilabel.text = string.Empty;
			this.roleNameList.Add(uilabel);
			uilabel.pivot = 3;
			uilabel.transform.localPosition = new Vector3(-30f, (float)(-(float)(i * 25)), -1f);
			uilabel = (SpawnManager.Instantiate(this.lblPrefab) as UILabel);
			U3DUtils.AddChild(this.RoleDamageGroup, uilabel.gameObject, true);
			uilabel.transform.localScale = new Vector3(16f, 16f, 0f);
			uilabel.text = string.Empty;
			this.roleDamageList.Add(uilabel);
			uilabel.pivot = 3;
			uilabel.transform.localPosition = new Vector3(-70f, (float)(-(float)(i * 25)), -1f);
		}
	}

	private Dictionary<int, ZuduiFubenDamageBox.RoleDamageDataMini> damageDataDict = new Dictionary<int, ZuduiFubenDamageBox.RoleDamageDataMini>();

	public GameObject group;

	public GameObject RoleNameGroup;

	public GameObject RoleDamageGroup;

	public GButton btnTitle;

	public UISprite _MainBossHPDirect;

	public UILabel lblPrefab;

	private List<UILabel> roleNameList = new List<UILabel>();

	private List<UILabel> roleDamageList = new List<UILabel>();

	private bool ShowBody = true;

	private int mapCode;

	private class RoleDamageDataMini
	{
		public string roleName = string.Empty;

		public long roleDamage;
	}
}
