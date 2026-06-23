using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class MonsterInfoDisplay : MonoBehaviour
{
	public int MosterID
	{
		set
		{
			this.m_MosterID = value;
		}
	}

	public bool MonsterNameVisible
	{
		set
		{
			if (value)
			{
				this.ShowDisplayInfo();
			}
			else if (!Global.IsOwnZhaoHuanShou(this.m_MosterID))
			{
				this.HideDisplayInfo();
			}
			else
			{
				this.ShowDisplayInfo();
			}
			this._MonsterNameVisible = value;
			this.RefreshName();
		}
	}

	private void Start()
	{
		if (null == MonsterInfoDisplay.Prefab)
		{
			MonsterInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/MonsterInfo") as GameObject);
		}
		this.ShowDisplayInfo();
	}

	private void OnBecameVisible()
	{
		this.ShowDisplayInfo();
	}

	private void OnBecameInvisible()
	{
		this.HideDisplayInfo();
	}

	private void OnDestroy()
	{
		this.HideDisplayInfo();
	}

	private void ShowDisplayInfo()
	{
		if (null != this.NGUIChildObject)
		{
			return;
		}
		if (null == MonsterInfoDisplay.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		if (Global.IsZhaoHuanShou(this.m_MosterID))
		{
			if (Global.IsOwnZhaoHuanShou(this.m_MosterID))
			{
				this._MonsterameColor = Color.green;
			}
			else
			{
				this._MonsterameColor = Global.GetRoleNameColor(Global.Data.SystemMonsters[this.m_MosterID].MasterRoleID);
			}
		}
		this.NGUIChildObject = DisplayInfoManager.Instance.CreateMonsterInfoDisplay();
		this.NGUIChildObject.GetComponent<UIFollowTarget>().target = this.Target;
		this.MonsterName = this.NGUIChildObject.transform.Find("Label_MonsterName").gameObject.GetComponent<UILabel>();
		this.MonsterName.supportEncoding = false;
		this.MonsterName.color = this._MonsterameColor;
		this.MonsterName.text = this.MonsterNameText;
		this.MonsterName.gameObject.SetActive(Global.IsOwnZhaoHuanShou(this.m_MosterID) || this._MonsterNameVisible);
	}

	private void HideDisplayInfo()
	{
		this.m_MosterID = 0;
		if (null == this.NGUIChildObject)
		{
			return;
		}
		DisplayInfoManager.Instance.DeleteMonsterInfoDisplay(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.MonsterName = null;
	}

	private void RefreshName()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		this.MonsterName.supportEncoding = false;
		this.MonsterName.color = this._MonsterameColor;
		this.MonsterName.gameObject.SetActive(Global.IsOwnZhaoHuanShou(this.m_MosterID) || this._MonsterNameVisible);
	}

	private static GameObject Prefab;

	private int m_MosterID;

	public Transform Target;

	public string MonsterNameText;

	private Color _MonsterameColor = new Color(1f, 0.3882353f, 0f, 1f);

	private GameObject NGUIChildObject;

	private UILabel MonsterName;

	private bool _MonsterNameVisible;
}
