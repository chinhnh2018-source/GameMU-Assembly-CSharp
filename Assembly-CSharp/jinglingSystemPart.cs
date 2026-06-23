using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class jinglingSystemPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.jinglingButton.Text = Global.GetLang("精灵");
		this.yuansuButton.Text = Global.GetLang("元素之心");
		this.m_JingLingSkills.Text = Global.GetLang("精灵技能");
		this.m_SkillsGrasp.Text = Global.GetLang("技能领悟");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
		this.jinglingButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1, YuansuBagTypes.Normal);
		};
		this.yuansuButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2, YuansuBagTypes.Normal);
		};
		if (Global.GetJingLingSkillIsOpen())
		{
			NGUITools.SetActive(this.m_JingLingSkills, true);
			NGUITools.SetActive(this.m_SkillsGrasp, true);
			this.m_JingLingSkills.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetPart(3, YuansuBagTypes.Normal);
			};
			this.m_SkillsGrasp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetPart(4, YuansuBagTypes.Normal);
			};
		}
		else
		{
			NGUITools.SetActive(this.m_JingLingSkills, false);
			NGUITools.SetActive(this.m_SkillsGrasp, false);
		}
		this.InitPartData(1);
	}

	private void InitPartData(int type = 1)
	{
		this.SetPart(type, YuansuBagTypes.Normal);
	}

	public void RefreshSkillUpData()
	{
		if (null != this.jingling)
		{
			this.jingling.RefreshSkillUpData();
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	public void RefreshSkillAwarkData(List<int> data)
	{
		if (null != this.jingling)
		{
			this.jingling.RefreshSkillAwarkData(data);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	public void RefreshSkillAwakeCost(int cost)
	{
		if (null != this.jingling)
		{
			this.jingling.RefreshSkillAwakeCost(cost);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	public void ErrorHander(int state)
	{
		if (null != this.jingling)
		{
			this.jingling.ErrorHandle((EPetSkillState)state);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"界面逻辑出现漏洞！！！精灵信息界面没有出现"
			});
		}
	}

	public void SetPart(int type, YuansuBagTypes mode = YuansuBagTypes.Normal)
	{
		switch (type)
		{
		case 1:
			this.SetBtnStat(jinglingSystemPart.BtnType.JingLing);
			if (null == this.jingling)
			{
				this.jingling = U3DUtils.NEW<jinglingPart>();
				this.jingling.transform.parent = this.Panel;
				this.jingling.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.jingling.transform.localScale = new Vector3(1f, 1f, 1f);
				this.jingling.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = e.ID
					});
					if (e.ID == 1)
					{
						SystemHelpMgr.OnAction(UIObjIDs.JingLingPartDetailpart, HelpStateEvents.Clicked, 1);
					}
				};
			}
			SystemHelpMgr.OnAction(UIObjIDs.JingLingPart, HelpStateEvents.Actived, 1);
			break;
		case 2:
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuanSuHeart, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.YuanSuHeart, trigger, param, param2, true);
				return;
			}
			this.SetBtnStat(jinglingSystemPart.BtnType.YuanSuZhiXin);
			if (this.jinglingYuansuPart == null)
			{
				this.jinglingYuansuPart = U3DUtils.NEW<JinglingYuansuPart>();
				U3DUtils.AddChild(this.Panel.gameObject, this.jinglingYuansuPart.gameObject, true);
			}
			this.jinglingYuansuPart.InitPartData(mode);
			this.jinglingYuansuPart.CancelSelectIcon();
			break;
		}
		case 3:
			this.SetBtnStat(jinglingSystemPart.BtnType.JingLingSkills);
			if (null == this.jingling)
			{
				this.jingling = U3DUtils.NEW<jinglingPart>();
				this.jingling.transform.parent = this.Panel;
				this.jingling.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.jingling.transform.localScale = new Vector3(1f, 1f, 1f);
				this.jingling.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = e.ID
					});
				};
			}
			break;
		case 4:
			this.SetBtnStat(jinglingSystemPart.BtnType.SkillsGrasp);
			if (null == this.jingling)
			{
				this.jingling = U3DUtils.NEW<jinglingPart>();
				this.jingling.transform.parent = this.Panel;
				this.jingling.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.jingling.transform.localScale = new Vector3(1f, 1f, 1f);
				this.jingling.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = e.ID
					});
				};
			}
			break;
		}
		if (this.jingling != null)
		{
			this.jingling.gameObject.SetActive(type != 2);
			this.jingling.PartType = (jinglingSystemPart.BtnType)type;
		}
		if (this.jinglingYuansuPart != null)
		{
			this.jinglingYuansuPart.gameObject.SetActive(type == 2);
		}
	}

	private void SetBtnStat(jinglingSystemPart.BtnType type)
	{
		this.ChangebtnStat(this.jinglingButton, type == jinglingSystemPart.BtnType.JingLing);
		this.ChangebtnStat(this.yuansuButton, type == jinglingSystemPart.BtnType.YuanSuZhiXin);
		this.ChangebtnStat(this.m_JingLingSkills, type == jinglingSystemPart.BtnType.JingLingSkills);
		this.ChangebtnStat(this.m_SkillsGrasp, type == jinglingSystemPart.BtnType.SkillsGrasp);
	}

	private void ChangebtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	internal void ShowHelpAnim(int p, int param)
	{
		if (p == 603 && null != this.jingling)
		{
			SystemHelpPart.SetMask(this.jingling.HuntButton, default(Vector4));
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton jinglingButton;

	public GButton yuansuButton;

	public GButton close;

	public GButton m_JingLingSkills;

	public GButton m_SkillsGrasp;

	public Transform Panel;

	public jinglingPart jingling;

	public JinglingYuansuPart jinglingYuansuPart;

	public enum BtnType
	{
		JingLing = 1,
		YuanSuZhiXin,
		JingLingSkills,
		SkillsGrasp
	}
}
