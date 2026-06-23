using System;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInfoManager
{
	public static DisplayInfoManager Instance
	{
		get
		{
			if (DisplayInfoManager.m_Instance == null)
			{
				DisplayInfoManager.m_Instance = new DisplayInfoManager();
				DisplayInfoManager.m_Instance.InitInfo();
			}
			return DisplayInfoManager.m_Instance;
		}
	}

	private GameObject GetNpcPreferb()
	{
		if (DisplayInfoManager.m_npcPrefab == null)
		{
			DisplayInfoManager.m_npcPrefab = (Resources.Load("Prefabs/FollowInfo/NPCInfo") as GameObject);
		}
		return DisplayInfoManager.m_npcPrefab;
	}

	private GameObject GetMonsterPreferb()
	{
		if (DisplayInfoManager.m_monsterPrefab == null)
		{
			DisplayInfoManager.m_monsterPrefab = (Resources.Load("Prefabs/FollowInfo/MonsterInfo") as GameObject);
		}
		return DisplayInfoManager.m_monsterPrefab;
	}

	private GameObject GetRolePreferb()
	{
		if (DisplayInfoManager.m_rolePrefab == null)
		{
			DisplayInfoManager.m_rolePrefab = (Resources.Load("Prefabs/FollowInfo/RoleInfo") as GameObject);
		}
		return DisplayInfoManager.m_rolePrefab;
	}

	private void InitInfo()
	{
		for (int i = 0; i < 20; i++)
		{
			this.AddNewNPCDisplay();
		}
		for (int j = 0; j < 30; j++)
		{
			this.AddNewMonsterDisplay();
		}
		for (int k = 0; k < 20; k++)
		{
			this.AddNewRoleDisplay();
		}
	}

	private void AddNewNPCDisplay()
	{
		GameObject gameObject = NGUITools.AddChild(HUDTextRoot.go, this.GetNpcPreferb());
		gameObject.AddComponent<UIFollowTarget>();
		gameObject.SetActive(false);
		this.m_NPCInfoCache.Add(gameObject);
		gameObject.name = "NPCInfoDisplay" + this.m_NPCInfoCache.Count;
	}

	private void AddNewMonsterDisplay()
	{
		GameObject gameObject = NGUITools.AddChild(HUDTextRoot.go, this.GetMonsterPreferb());
		gameObject.AddComponent<UIFollowTarget>();
		gameObject.SetActive(false);
		this.m_MonsterInfoCache.Add(gameObject);
		gameObject.name = "MonsterInfoDisplay" + this.m_MonsterInfoCache.Count;
	}

	private void AddNewRoleDisplay()
	{
		GameObject gameObject = NGUITools.AddChild(HUDTextRoot.go, this.GetRolePreferb());
		gameObject.SetActive(false);
		this.m_RoleInfoCache.Add(gameObject);
		gameObject.name = "RoleInfoDisplay" + this.m_RoleInfoCache.Count;
	}

	public GameObject CreateNPCInfoDisplay()
	{
		int num = -1;
		for (int i = 0; i < this.m_NPCInfoCache.Count; i++)
		{
			if (!this.m_NPCInfoCache[i].activeSelf)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			this.m_NPCInfoCache[num].SetActive(true);
			return this.m_NPCInfoCache[num];
		}
		this.AddNewNPCDisplay();
		num = this.m_NPCInfoCache.Count - 1;
		this.m_NPCInfoCache[num].SetActive(true);
		return this.m_NPCInfoCache[num];
	}

	public void DeleteNPCInfoDisplay(GameObject go)
	{
		int num = this.m_NPCInfoCache.IndexOf(go);
		if (num >= 0)
		{
			go.SetActive(false);
		}
		else
		{
			Object.Destroy(go);
		}
	}

	public GameObject CreateMonsterInfoDisplay()
	{
		int num = -1;
		for (int i = 0; i < this.m_MonsterInfoCache.Count; i++)
		{
			if (!this.m_MonsterInfoCache[i].activeSelf)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			this.m_MonsterInfoCache[num].SetActive(true);
			return this.m_MonsterInfoCache[num];
		}
		this.AddNewMonsterDisplay();
		num = this.m_MonsterInfoCache.Count - 1;
		this.m_MonsterInfoCache[num].SetActive(true);
		return this.m_MonsterInfoCache[num];
	}

	public void DeleteMonsterInfoDisplay(GameObject go)
	{
		int num = this.m_MonsterInfoCache.IndexOf(go);
		if (num >= 0)
		{
			go.SetActive(false);
		}
		else
		{
			Object.Destroy(go);
		}
	}

	public GameObject CreateRoleInfoDisplay()
	{
		int num = -1;
		for (int i = 0; i < this.m_RoleInfoCache.Count; i++)
		{
			if (!this.m_RoleInfoCache[i].activeSelf)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			this.m_RoleInfoCache[num].SetActive(true);
			return this.m_RoleInfoCache[num];
		}
		this.AddNewRoleDisplay();
		num = this.m_RoleInfoCache.Count - 1;
		this.m_RoleInfoCache[num].SetActive(true);
		return this.m_RoleInfoCache[num];
	}

	public void DeleteRoleInfoDisplay(GameObject go)
	{
		int num = this.m_RoleInfoCache.IndexOf(go);
		if (num >= 0)
		{
			UIFollowTarget component = go.GetComponent<UIFollowTarget>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			go.SetActive(false);
		}
		else
		{
			Object.Destroy(go);
		}
	}

	private const int MaxNPCDisplayNum = 20;

	private const int MaxMonsterDisplayNum = 30;

	private const int MaxRoleDisplayNum = 20;

	private static GameObject m_npcPrefab;

	private static GameObject m_monsterPrefab;

	private static GameObject m_rolePrefab;

	private List<GameObject> m_NPCInfoCache = new List<GameObject>();

	private List<GameObject> m_MonsterInfoCache = new List<GameObject>();

	private List<GameObject> m_RoleInfoCache = new List<GameObject>();

	private static DisplayInfoManager m_Instance;
}
