using System;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class ShenHunPartSkill : UserControl
{
	public MUTransfigurationFashionEffect EffectInfo
	{
		get
		{
			return this.m_effectInfo;
		}
	}

	public MagicInfoVO SkillInfo
	{
		get
		{
			return this.m_skillInfo;
		}
	}

	public int SkillLevel
	{
		get
		{
			return this.m_skillLevel;
		}
	}

	public bool BeLock
	{
		get
		{
			return this.m_beLock;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSelectSkill);
	}

	private void OnSelectSkill(GameObject go)
	{
		if (this.OnClickSkill != null)
		{
			this.OnClickSkill.Invoke(this);
		}
	}

	public void InitSkill(int skillId, int skillLevel, int unLockLevel, int shiZhuangLevel = -1)
	{
		this.m_skillInfo = ConfigMagicInfos.GetMaigcInfoVOByCode(skillId);
		this.m_skillLevel = skillLevel;
		if (shiZhuangLevel > -1)
		{
			this.m_beLock = (unLockLevel > shiZhuangLevel);
		}
		else
		{
			this.m_beLock = (unLockLevel > skillLevel);
		}
		if (this.m_skillInfo == null)
		{
			return;
		}
		int num = this.m_skillInfo.MagicIcon;
		if (num < 0)
		{
			num = 0;
		}
		this.imgIcon.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
		{
			num
		});
		if (!this.m_beLock)
		{
			this.lblSkillLevel.text = "Lv" + skillLevel;
			this.imgSuo.gameObject.SetActive(false);
		}
		else
		{
			if (shiZhuangLevel > -1)
			{
				this.lblSkillLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("强化") + unLockLevel + Global.GetLang("级")
				});
			}
			else
			{
				this.lblSkillLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					unLockLevel + Global.GetLang("级解锁")
				});
			}
			this.imgSuo.gameObject.SetActive(true);
		}
	}

	public void InitShiZhuangEffect(int effectId, int unLockLevel, int shiZhuangLevel)
	{
		this.m_effectInfo = IConfigbase<ConfigShenHun>.Instance.GetFashionEffectByID(effectId);
		this.m_beLock = (unLockLevel > shiZhuangLevel);
		if (this.m_effectInfo == null)
		{
			return;
		}
		this.m_skillLevel = this.m_effectInfo.Level;
		int magicIcon = this.m_effectInfo.MagicIcon;
		this.imgIcon.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
		{
			magicIcon
		});
		if (!this.m_beLock)
		{
			this.lblSkillLevel.text = "Lv" + this.m_effectInfo.Level;
			this.imgSuo.gameObject.SetActive(false);
		}
		else
		{
			this.lblSkillLevel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("强化") + unLockLevel + Global.GetLang("级")
			});
			this.imgSuo.gameObject.SetActive(true);
		}
	}

	public Action<ShenHunPartSkill> OnClickSkill;

	public UILabel lblSkillLevel;

	public ShowNetImage imgIcon;

	public UISprite imgSuo;

	private MagicInfoVO m_skillInfo;

	private MUTransfigurationFashionEffect m_effectInfo;

	private int m_skillLevel;

	private bool m_beLock;
}
