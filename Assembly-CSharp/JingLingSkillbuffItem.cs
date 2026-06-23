using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JingLingSkillbuffItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
		this.m_TimeMask.alpha = 0.5f;
	}

	public override void Update()
	{
		base.Update();
		if (this.m_LifeTime >= 0f)
		{
			this.m_LifeTime -= Time.deltaTime;
			this.m_TimeMask.fillAmount = 1f - this.m_LifeTime / this.m_LifeTimeLong;
		}
		else
		{
			this.hander(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
			Object.Destroy(base.gameObject);
		}
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public void Init(int skillId)
	{
		this.m_SkillID = skillId;
		this.m_LifeTime = Global.GetSkillCDTime(skillId);
		this.m_LifeTimeLong = this.m_LifeTime;
		this.m_Sign.URL = Global.GetJingLingSkillSignURL(skillId);
		int skillMagicColor = Global.GetSkillMagicColor(skillId);
		if (skillMagicColor == 1)
		{
			this.m_StepSp.spriteName = "iconState_zuoyue";
		}
		else if (skillMagicColor == 2)
		{
			this.m_StepSp.spriteName = "iconState_zuoyue1";
		}
		else if (skillMagicColor == 3)
		{
			this.m_StepSp.spriteName = "iconState_zuoyue2";
		}
	}

	public int SkillId
	{
		get
		{
			return this.m_SkillID;
		}
		set
		{
			this.m_SkillID = value;
		}
	}

	public float SignTime
	{
		set
		{
			this.m_LifeTime = value;
		}
	}

	public UISprite m_StepSp;

	public ShowNetImage m_Sign;

	public UISprite m_TimeMask;

	private float m_LifeTime;

	private float m_LifeTimeLong;

	private int m_SkillID;

	public DPSelectedItemEventHandler hander;
}
