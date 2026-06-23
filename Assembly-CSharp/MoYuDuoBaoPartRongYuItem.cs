using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class MoYuDuoBaoPartRongYuItem : UserControl
{
	public ZorkAchievementVO ZorkAchievement
	{
		get
		{
			return this.m_zorkAchievement;
		}
		set
		{
			this.m_zorkAchievement = value;
			this.InitInfo(this.m_zorkAchievement);
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblName.text = Global.GetLang("魔域夺宝击杀BOSS");
		this.lblProgress.text = Global.GetLang("1/2");
		this.btnLingQu.Label.text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OnLingQu != null)
			{
				this.OnLingQu.Invoke(this);
			}
		};
	}

	private void InitInfo(ZorkAchievementVO info)
	{
		if (this.m_zorkAchievement.AchievementTarget == 7)
		{
			this.lblName.text = string.Format(this.m_zorkAchievement.AchievementTitle, this.m_zorkAchievement.TargetNum / 10000);
			this.lblProgress.text = 0 + "/" + this.m_zorkAchievement.TargetNum / 10000;
		}
		else
		{
			this.lblName.text = string.Format(this.m_zorkAchievement.AchievementTitle, this.m_zorkAchievement.TargetNum);
			this.lblProgress.text = 0 + "/" + this.m_zorkAchievement.TargetNum;
		}
		this.imgProgress.fillAmount = 0f;
		List<string> rewardStr = new List<string>(this.m_zorkAchievement.AchievementReward);
		Global.LoadReward(rewardStr, this.gridReward, 70f, true);
	}

	public void SetFinishNum(int num)
	{
		if (num > this.m_zorkAchievement.TargetNum)
		{
			num = this.m_zorkAchievement.TargetNum;
		}
		this.imgProgress.fillAmount = (float)num * 1f / (float)this.m_zorkAchievement.TargetNum;
		if (this.m_zorkAchievement.AchievementTarget == 7)
		{
			this.lblProgress.text = num / 10000 + "/" + this.m_zorkAchievement.TargetNum / 10000;
		}
		else
		{
			this.lblProgress.text = num + "/" + this.m_zorkAchievement.TargetNum;
		}
	}

	public void SetState(int state)
	{
		if (state == 1)
		{
			this.btnLingQu.Text = Global.GetLang("领取");
			this.btnLingQu.isEnabled = true;
		}
		else if (state == 2)
		{
			this.btnLingQu.Text = Global.GetLang("未完成");
			this.btnLingQu.isEnabled = false;
		}
		else if (state == 3)
		{
			this.btnLingQu.Text = Global.GetLang("已领取");
			this.btnLingQu.isEnabled = false;
		}
	}

	private string GetDuanWeiName(int id)
	{
		string result = string.Empty;
		switch (id)
		{
		case 1:
			result = Global.GetLang("王者");
			break;
		case 2:
			result = Global.GetLang("钻石");
			break;
		case 3:
			result = Global.GetLang("黄金");
			break;
		case 4:
			result = Global.GetLang("白银");
			break;
		case 5:
			result = Global.GetLang("青铜");
			break;
		}
		return result;
	}

	public Action<MoYuDuoBaoPartRongYuItem> OnLingQu;

	public UILabel lblName;

	public UILabel lblProgress;

	public GButton btnLingQu;

	public UISprite imgProgress;

	public UIGrid gridReward;

	private ZorkAchievementVO m_zorkAchievement;
}
