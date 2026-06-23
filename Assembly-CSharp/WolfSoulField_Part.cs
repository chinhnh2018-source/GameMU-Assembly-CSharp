using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class WolfSoulField_Part : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Rule.Label.text = Global.GetLang("规则");
		this.Award.Label.text = Global.GetLang("奖励");
		this.Attack.Label.text = Global.GetLang("进攻");
		this.WorldMap.Label.text = Global.GetLang("圣域排行");
		this.TextureMap.URL = "NetImages/GameRes/Images/Plate/shengzhanmap.jpg";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetCityState();
		this.SetCityLevel();
		this.Rule.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelecetItem(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		};
		this.Award.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelecetItem(this, new DPSelectedItemEventArgs
			{
				ID = -2
			});
		};
		this.Attack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				Super.HintMainText(Global.GetLang("非战盟首领不可参加报名！"), 10, 3);
				return;
			}
			if (this.isMaxLevelCity)
			{
				Super.HintMainText(Global.GetLang("已占领最高等级城池，无法继续进攻!"), 10, 3);
				return;
			}
			this.DPSelecetItem(this, new DPSelectedItemEventArgs
			{
				ID = -3
			});
		};
		this.WorldMap.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelecetItem(this, new DPSelectedItemEventArgs
			{
				ID = -4
			});
		};
		GameInstance.Game.ApplyPlayerData();
		Super.ShowNetWaiting(null);
		base.InvokeRepeating("UpdateInfo", 30f, 30f);
		ActivityTipManager.RegActivityTipItem(15002, delegate(int s, ActivityTipItem e)
		{
			this.AwardTipsICon.SetActive(e.IsActive);
		});
	}

	public override void Destroy()
	{
		base.Destroy();
		if (null != this.m_TeXiaoObj)
		{
			NGUITools.Destroy(this.m_TeXiaoObj);
		}
	}

	private void SetCityState()
	{
		for (int i = 0; i < this.City.Length - 1; i++)
		{
			this.City[i].Cloud.transform.gameObject.SetActive(true);
			this.City[i].State.transform.gameObject.SetActive(false);
			this.City[i].Name.transform.gameObject.SetActive(false);
		}
		this.City[this.City.Length - 1].Cloud.transform.gameObject.SetActive(false);
		this.City[this.City.Length - 1].State.transform.gameObject.SetActive(false);
		this.City[this.City.Length - 1].Name.transform.gameObject.SetActive(true);
		this.City[this.City.Length - 1].Name.text = Global.GetLang("无人占领");
	}

	private void SetCityLevel()
	{
		for (int i = 0; i < this.City.Length; i++)
		{
			this.City[i].Level = Global.GetCityInfo()[i + 1].CityLevel;
		}
	}

	private void UpdateInfo()
	{
		this.SetCityState(WolfSoulField_Part.langhunlingyuRoleData);
	}

	public void SetCityState(LangHunLingYuRoleData langhunlingyuRoleData)
	{
		this.Award.isEnabled = false;
		if (langhunlingyuRoleData == null)
		{
			return;
		}
		this.SetCityInfo(langhunlingyuRoleData);
		if (langhunlingyuRoleData.GetDayAwardsState != null && langhunlingyuRoleData.GetDayAwardsState.Count > 0)
		{
			this.Award.isEnabled = true;
		}
	}

	private void SetCityInfo(LangHunLingYuRoleData langhunlingyuRoleData)
	{
		if (langhunlingyuRoleData.SelfCityList == null)
		{
			return;
		}
		for (int i = 0; i < this.City.Length; i++)
		{
			for (int j = 0; j < langhunlingyuRoleData.SelfCityList.Count; j++)
			{
				if (this.City[i].Level == langhunlingyuRoleData.SelfCityList[j].CityLevel)
				{
					this.CityState(langhunlingyuRoleData.SelfCityList[j], i, false);
					this.isMaxLevelCity = this.IsMaxLevel(langhunlingyuRoleData.SelfCityList[j]);
				}
			}
		}
	}

	private bool IsCurrentCityLevelOverOtherCityLevel(LangHunLingYuRoleData langhunlingyuRoleData)
	{
		if (langhunlingyuRoleData.SelfCityList == null)
		{
			return false;
		}
		for (int i = 0; i < langhunlingyuRoleData.SelfCityList.Count; i++)
		{
			if (langhunlingyuRoleData.SelfCityList[i].Owner != null)
			{
				if (langhunlingyuRoleData.SelfCityList[i].Owner.BHID == Global.Data.roleData.Faction && langhunlingyuRoleData.SelfCityList[i].CityLevel >= 6)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool IsMaxLevel(LangHunLingYuCityData SelfCity)
	{
		return SelfCity.Owner != null && (SelfCity.CityLevel == 10 && SelfCity.Owner.BHID == Global.Data.roleData.Faction);
	}

	private List<int> IsContainTheLevel()
	{
		if (WolfSoulField_Part.langhunlingyuRoleData != null && WolfSoulField_Part.langhunlingyuRoleData.SelfCityList != null)
		{
			for (int i = 0; i < WolfSoulField_Part.langhunlingyuRoleData.SelfCityList.Count; i++)
			{
				if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner != null)
				{
					if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner.BHID == Global.Data.roleData.Faction)
					{
						this.Owner.Add(WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].CityLevel);
					}
				}
			}
			return this.Owner;
		}
		return this.Owner = null;
	}

	private void SetOtherCityInfo(LangHunLingYuRoleData langhunlingyuRoleData)
	{
	}

	private void OtherCityState(LangHunLingYuCityData CityList, int i, bool isShowCloud = true, bool isOverCityLevel = false)
	{
	}

	private void CityState(LangHunLingYuCityData CityList, int i, bool isShowName = false)
	{
		string str_StateName = string.Empty;
		string str_StateName2 = string.Empty;
		string chineseText = (CityList.CityLevel != 10) ? string.Empty : "无人占领";
		bool active = false;
		bool active2 = CityList.CityLevel == 10;
		bool active3 = CityList.CityLevel != 10;
		if (CityList.CityLevel == 10 && CityList.Owner != null && Global.Data.roleData.Faction != CityList.Owner.BHID && !this.IsAttackerListContainsMe(CityList))
		{
			if (CityList.CityLevel == 10)
			{
				str_StateName = ((CityList.AttackerList == null) ? "None" : ((!Global.IsWarTime(CityList.CityLevel)) ? "None" : "fire"));
				active = true;
				active3 = false;
				chineseText = Global.GetLang("区") + CityList.Owner.ZoneID.ToString() + "\r\n" + CityList.Owner.BHName;
			}
		}
		else if (CityList.Owner != null && Global.Data.roleData.Faction == CityList.Owner.BHID)
		{
			str_StateName = ((CityList.AttackerList == null) ? "None" : "defend");
			str_StateName2 = ((!Global.IsWarTime(CityList.CityLevel)) ? "None" : ((CityList.AttackerList == null) ? "None" : "fire"));
			active = true;
			active3 = false;
			if (CityList.CityLevel == 10)
			{
				chineseText = Global.GetLang("区") + CityList.Owner.ZoneID.ToString() + "\r\n" + CityList.Owner.BHName;
			}
		}
		else if (CityList.AttackerList != null)
		{
			for (int j = 0; j < CityList.AttackerList.Count; j++)
			{
				if (Global.Data.roleData.Faction == CityList.AttackerList[j].BHID)
				{
					str_StateName = "pk";
					str_StateName2 = ((!Global.IsWarTime(CityList.CityLevel)) ? "None" : "fire");
					active = true;
					active3 = false;
					if (CityList.CityLevel == 10 && CityList.Owner != null)
					{
						chineseText = CityList.Owner.ZoneID.ToString() + Global.GetLang("区") + "\r\n" + CityList.Owner.BHName;
					}
				}
			}
		}
		this.City[i].Name.text = Global.GetLang(chineseText);
		this.City[i].Cloud.transform.gameObject.SetActive(active3);
		this.City[i].State.transform.gameObject.SetActive(active);
		this.City[i].Name.transform.gameObject.SetActive(active2);
		this.City[i].State.enabled = false;
		this.StateTeXiao(str_StateName, this.City[i].State.transform);
		this.StateTeXiao(str_StateName2, this.City[i].Fire.transform);
	}

	private void StateTeXiao(string str_StateName, Transform parent)
	{
		parent.localScale = Vector3.one;
		string text = "UITeXiao/Perfabs/langhunshengyu/";
		string text2 = string.Empty;
		if ("defend" == str_StateName)
		{
			text2 = "lhsy_fangyu";
		}
		else if ("fire" == str_StateName)
		{
			text2 = "lhsy_zhandouzhong";
		}
		else if ("pk" == str_StateName)
		{
			text2 = "lhsy_gongji";
		}
		else
		{
			text2 = string.Empty;
		}
		if (parent.childCount != 0)
		{
			this.ClearChild(parent.gameObject);
		}
		if (!string.IsNullOrEmpty(text2))
		{
			this.m_TeXiaoObj = this.LoadTeXiaoObj(text + text2, parent);
		}
		if ("fire" == str_StateName)
		{
			this.m_TeXiaoObj.transform.localPosition = new Vector3(0f, 0f, -100f);
		}
	}

	private void ClearChild(GameObject obj)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			GameObject gameObject = obj.transform.GetChild(i).gameObject;
			if (null != gameObject)
			{
				NGUITools.Destroy(gameObject);
			}
		}
	}

	private GameObject LoadTeXiaoObj(string Path, Transform parent)
	{
		Object @object = Resources.Load(Path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			return gameObject;
		}
		return null;
	}

	private bool IsAttackerListContainsMe(LangHunLingYuCityData CityList)
	{
		if (CityList.AttackerList == null)
		{
			return false;
		}
		for (int i = 0; i < CityList.AttackerList.Count; i++)
		{
			if (Global.Data.roleData.Faction == CityList.AttackerList[i].BHID)
			{
				return true;
			}
		}
		return false;
	}

	private float FixedTimeUpdate()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime.TryParse(string.Format("{0}/{1}/{2} 20:00:00", correctDateTime.Year, correctDateTime.Month, correctDateTime.Day), ref dateTime);
		DateTime.TryParse(string.Format("{0}/{1}/{2} 20:30:00", correctDateTime.Year, correctDateTime.Month, correctDateTime.Day), ref dateTime2);
		float num = (float)(correctDateTime.Ticks / 10000000L);
		float num2 = (float)(dateTime.Ticks / 10000000L);
		float num3 = (float)(dateTime2.Ticks / 10000000L);
		if (num > num3)
		{
			return 3600f;
		}
		if (num < num2)
		{
			return num2 - num;
		}
		return num3 - num;
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("UpdateInfo");
		WolfSoulField_Part.langhunlingyuRoleData = null;
		ActivityTipManager.RegActivityTipItem(15002, null);
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler DPSelecetItem;

	public GButton Rule;

	public GButton Award;

	public GButton Attack;

	public GButton WorldMap;

	public GameObject AwardTipsICon;

	public ShowNetImage TextureMap;

	public MyCity[] City;

	public static LangHunLingYuRoleData langhunlingyuRoleData;

	private int[] otherCityLevel = new int[]
	{
		1,
		3,
		5,
		6
	};

	private bool isMaxLevelCity;

	private List<int> Owner = new List<int>();

	private GameObject m_TeXiaoObj;
}
