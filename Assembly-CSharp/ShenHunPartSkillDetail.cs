using System;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class ShenHunPartSkillDetail : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblNowDesWord.text = Global.GetLang("描述：");
		this.lblNextDesWord.text = Global.GetLang("下级预览：");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.btnCancal).onClick = new UIEventListener.VoidDelegate(this.OnClose);
	}

	private void OnClose(GameObject go)
	{
		base.gameObject.SetActive(false);
	}

	public void InitSkill(MagicInfoVO skill, int level)
	{
		int topBianShenLevel = IConfigbase<ConfigShenHun>.Instance.GetTopBianShenLevel();
		this.lblNextDesWord.text = Global.GetLang("下级预览：");
		this.lblSkillName.text = skill.Name;
		this.lblLv.text = "Lv" + level;
		string description = skill.Description;
		string magicScripts = skill.MagicScripts;
		string[] array = magicScripts.Split(new char[]
		{
			'|'
		});
		string[] magicScriptsValue = this.GetMagicScriptsValue(array[0]);
		if (magicScriptsValue.Length >= 3)
		{
			this.lblNowDes.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), Convert.ToDouble(magicScriptsValue[2]), level), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), Convert.ToInt32(magicScriptsValue[1]), level));
			this.lblNextDes.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), Convert.ToDouble(magicScriptsValue[2]), level + 1), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), Convert.ToInt32(magicScriptsValue[1]), level + 1));
		}
		else
		{
			this.lblNowDes.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), level), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), level));
			this.lblNextDes.Text = string.Format(description, this.GetProportion(Convert.ToDouble(magicScriptsValue[0]), level + 1), this.GetGuDingzhi(Convert.ToInt32(magicScriptsValue[1]), level + 1));
		}
		if (level >= topBianShenLevel)
		{
			this.lblNextDesWord.text = Global.GetLang("已到最高等级");
			this.lblNextDes.text = string.Empty;
		}
		int num = skill.MagicIcon;
		if (num < 0)
		{
			num = 0;
		}
		this.skillIcon.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
		{
			num
		});
	}

	public void InitEffect(MUTransfigurationFashionEffect effect, MUTransfigurationFashionEffect nextEffect, int nextlevel)
	{
		this.lblSkillName.text = effect.Name;
		this.lblLv.text = string.Empty;
		string description = effect.Description;
		this.lblNowDes.Text = description;
		if (nextEffect == null)
		{
			this.lblNextDesWord.text = Global.GetLang("已到最高等级");
			this.lblNextDes.Text = string.Empty;
		}
		else
		{
			this.lblNextDesWord.text = string.Format(Global.GetLang("时装强化{0}级获得下1级效果"), nextlevel);
			this.lblNextDes.Text = nextEffect.Description;
		}
		int magicIcon = effect.MagicIcon;
		this.skillIcon.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
		{
			magicIcon
		});
	}

	private string[] GetMagicScriptsValue(string str)
	{
		if (str == string.Empty)
		{
			return null;
		}
		if (str.Contains("MU_FIRE_WALL_Y") || str.Contains("ZHONGDU"))
		{
			int num = str.IndexOf('(') + 1;
			int num2 = str.IndexOf(')') - num;
			string text = str.Substring(num, num2);
			string[] array = text.Split(new char[]
			{
				','
			});
			return new string[]
			{
				array[2],
				array[3]
			};
		}
		int num3 = str.IndexOf('(') + 1;
		int num4 = str.IndexOf(')') - num3;
		return str.Substring(num3, num4).Split(new char[]
		{
			','
		});
	}

	private string GetGuDingzhi(int jichubi, int sklev)
	{
		return string.Format("{{00ff00}}{0}{{-}}", (jichubi + jichubi * sklev).ToString());
	}

	private string GetGuDingzhi(int jichubi, int step, int sklev)
	{
		return string.Format("{{00ff00}}{0}{{-}}", (jichubi + step * sklev).ToString());
	}

	private string GetProportion(double jichubi, int sklev)
	{
		double num = jichubi + jichubi / 200.0 * (double)sklev;
		return string.Format("{{00ff00}}{0}%{{-}}", (num * 100.0).ToString("0.00"));
	}

	private string GetProportion(double jichubi, double step, int sklev)
	{
		double num = jichubi + step * (double)sklev;
		return string.Format("{{00ff00}}{0}%{{-}}", (num * 100.0).ToString("0.00"));
	}

	public UILabel lblSkillName;

	public UILabel lblLv;

	public UILabel lblNowDesWord;

	public TextBlock lblNowDes;

	public UILabel lblNextDesWord;

	public TextBlock lblNextDes;

	public ShowNetImage skillIcon;

	public GameObject btnCancal;
}
