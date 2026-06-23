using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MonsterCaiJiInfoDisplay : MonoBehaviour
{
	public int MosterID
	{
		set
		{
			this.m_MosterID = value;
			this.m_monsterData = Global.Data.SystemMonsters.GetValue(this.m_MosterID);
			if (this.m_monsterData != null)
			{
				this.m_totalGrowTime = ShiLiData.GetResourceGrowTime(this.m_monsterData.ExtensionID) * 1000;
				this.m_okTimeTicks = (long)this.m_totalGrowTime + this.m_monsterData.BirthTicks;
			}
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
			else
			{
				this.HideDisplayInfo();
			}
			this._MonsterNameVisible = value;
			this.RefreshName();
		}
	}

	private void Start()
	{
		if (null == MonsterCaiJiInfoDisplay.Prefab)
		{
			MonsterCaiJiInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/MonsterCaiJiInfo") as GameObject);
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
		if (null == MonsterCaiJiInfoDisplay.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		if (this.m_monsterData == null)
		{
			return;
		}
		if (this.m_monsterData.BirthTicks <= 0L)
		{
			this.SetCaiJiOK();
			return;
		}
		this.NGUIChildObject = NGUITools.AddChild(HUDTextRoot.go, MonsterCaiJiInfoDisplay.Prefab);
		this.NGUIChildObject.AddComponent<UIFollowTarget>().target = this.Target;
		this.lblTime = this.NGUIChildObject.transform.Find("Label_MonsterName").gameObject.GetComponent<UILabel>();
		this.imgValue = this.NGUIChildObject.transform.Find("imgValue").gameObject.GetComponent<UISprite>();
		base.StopAllCoroutines();
		base.StartCoroutine(this.TimeProc());
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			long ticks = Global.GetCorrectLocalTime();
			long leftTicks = this.m_okTimeTicks - ticks;
			if (leftTicks <= 0L)
			{
				break;
			}
			if (this.m_totalGrowTime != 0)
			{
				this.imgValue.fillAmount = (float)leftTicks * 1f / (float)this.m_totalGrowTime;
			}
			this.lblTime.text = this.GetTimeStrBySec(leftTicks / 1000L);
			if (this.imgValue.fillAmount <= 0f)
			{
				goto Block_2;
			}
			yield return new WaitForSeconds(0.5f);
		}
		this.SetCaiJiOK();
		this.HideDisplayInfo();
		yield break;
		Block_2:
		this.SetCaiJiOK();
		this.HideDisplayInfo();
		yield break;
		yield break;
	}

	private void SetCaiJiOK()
	{
		this.Target.transform.localScale = new Vector3(2f, 2f, 2f);
	}

	public string GetTimeStrBySec(long sec)
	{
		int num = 3600;
		int num2 = 60;
		if (num2 != 0 && num != 0)
		{
			int num3 = (int)(sec / (long)num);
			int num4 = (int)(sec % (long)num / (long)num2);
			int num5 = (int)(sec % (long)num2);
			return StringUtil.substitute("{0}:{1}:{2}", new object[]
			{
				num3.ToString("00"),
				num4.ToString("00"),
				num5.ToString("00")
			});
		}
		return "00:00:00";
	}

	private void HideDisplayInfo()
	{
		this.m_MosterID = 0;
		if (null == this.NGUIChildObject)
		{
			return;
		}
		base.StopAllCoroutines();
		Object.Destroy(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.lblTime = null;
	}

	private void RefreshName()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		this.lblTime.supportEncoding = false;
		this.lblTime.color = this._MonsterameColor;
		this.lblTime.gameObject.SetActive(Global.IsOwnZhaoHuanShou(this.m_MosterID) || this._MonsterNameVisible);
	}

	private static GameObject Prefab;

	private int m_MosterID;

	private MonsterData m_monsterData;

	private GameObject NGUIChildObject;

	private UILabel lblTime;

	private UISprite imgValue;

	private long m_okTimeTicks;

	private int m_totalGrowTime = 10000;

	private bool _MonsterNameVisible;

	public Transform Target;

	public string MonsterNameText;

	private Color _MonsterameColor = new Color(1f, 0.3882353f, 0f, 1f);
}
