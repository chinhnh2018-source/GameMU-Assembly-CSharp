using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class RebirthBossTaskScene : MonoBehaviour
{
	private void Awake()
	{
		this._RankValue.Margin = new Vector2(0f, 17f);
		this._DamagePctLabel.Margin = new Vector2(0f, 17f);
	}

	public int BoosLife
	{
		get
		{
			return this.mBoosLife;
		}
	}

	public void RefreshBossLife(int value)
	{
		this.mBoosLife = value;
		this.RefeshBossPic();
		if (null != this._BossLifeValue)
		{
			this._BossLifeValue.text = (value + "%").ToString();
		}
		if (null != this._BossInfBtn)
		{
			UIEventListener.Get(this._BossInfBtn.gameObject).onClick = delegate(GameObject g)
			{
				if (this.Hander != null)
				{
					this.Hander(null, null);
				}
			};
		}
	}

	public void RefreshRankInf(string value, string DamagePctValue)
	{
		if (null != this._RankValue)
		{
			this._RankValue.text = value;
		}
		if (null != this._DamagePctLabel)
		{
			this._DamagePctLabel.text = DamagePctValue;
		}
	}

	public void RefreshMyRankInf(string value)
	{
		if (null != this._MyRankInf)
		{
			this._MyRankInf.text = value;
		}
	}

	private void RefeshBossPic()
	{
		if (0 >= this.mBoosLife)
		{
			this._BossInfBtn.spriteName = "boss_kuang_hui";
			this._BossInfIcon.spriteName = "boss_touxiang_hui";
		}
		else
		{
			this._BossInfBtn.spriteName = "boss_kuang";
			this._BossInfIcon.spriteName = "boss_touxiang";
		}
	}

	private void RefresTime()
	{
		if (this.mNextrefreshTime != DateTime.MinValue)
		{
			if (this.mNextrefreshTime > Global.GetCorrectDateTime())
			{
				TimeSpan timeSpan;
				timeSpan..ctor(this.mNextrefreshTime.Ticks - Global.GetCorrectDateTime().Ticks);
				this._BossInfLabel.text = Global.GetTimeStrBySecFilterZero((int)timeSpan.TotalSeconds, true, 2);
			}
			else
			{
				this._BossInfLabel.text = Global.GetLang("已刷新");
			}
		}
		else
		{
			this._BossInfLabel.text = Global.GetLang("已刷新");
		}
	}

	internal void RefreshBossRefreshTime(DateTime BossRefreshTime)
	{
		if (DateTime.MinValue == BossRefreshTime)
		{
			if (null != this._BossInfLabel)
			{
				this._BossInfLabel.text = Global.GetLang("已刷新");
			}
		}
		else
		{
			this.mNextrefreshTime = BossRefreshTime;
			if (null != this._BossInfLabel)
			{
				this._BossInfLabel.text = Global.GetLang(string.Empty);
			}
			this.RefresTime();
		}
	}

	private void Update()
	{
		this.RefresTime();
	}

	private string GetPlatformIconName(int platform)
	{
		return string.Empty;
	}

	internal void RefreshRankInfplatformIds(int[] platformIds)
	{
		byte b = 0;
		while ((int)b < platformIds.Length)
		{
			string platformIconName = this.GetPlatformIconName(platformIds[(int)b]);
			if (string.IsNullOrEmpty(platformIconName))
			{
				if (this._Platforms[(int)b].enabled)
				{
					this._Platforms[(int)b].enabled = false;
				}
			}
			else
			{
				if (!this._Platforms[(int)b].enabled)
				{
					this._Platforms[(int)b].enabled = !Context.IsHaiwai;
				}
				if (!platformIconName.Equals(this._Platforms[(int)b].spriteName))
				{
					this._Platforms[(int)b].spriteName = platformIconName;
				}
			}
			b += 1;
		}
	}

	public bool ShowRankInf
	{
		get
		{
			return NGUITools.GetActive(this._LiftRoot);
		}
		set
		{
			this._LiftRoot.SetActive(value);
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel _BossLifeValue;

	[SerializeField]
	private UILabel _RankValue;

	[SerializeField]
	private UILabel _MyRankInf;

	[SerializeField]
	private GameObject _ShowInfBtn;

	[SerializeField]
	private UISprite _BossInfBtn;

	[SerializeField]
	private UILabel _BossInfLabel;

	[SerializeField]
	private UISprite _BossInfIcon;

	[SerializeField]
	private UILabel _DamagePctLabel;

	[SerializeField]
	private UISprite[] _Platforms;

	[SerializeField]
	private GameObject _LiftRoot;

	public bool FirstTimeEnterMap = true;

	private int mBoosLife;

	private DateTime mNextrefreshTime = DateTime.MinValue;
}
