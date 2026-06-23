using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class MoYuDuoBaoPartRongYu : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnGeRen.Label.text = Global.GetLang("个人成就");
		this.btnZhanDui.Label.text = Global.GetLang("战队成就");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(this, null);
			}
		};
		this.btnGeRen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectContent(MoYuRongYuType.GeRenRongYu);
		};
		this.btnZhanDui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectContent(MoYuRongYuType.ZhanDuiRongYu);
		};
		this.OnSelectContent(MoYuRongYuType.GeRenRongYu);
		this.GetRongYuInfo();
	}

	private void OnSelectContent(MoYuRongYuType type)
	{
		if (this.m_type == type && this.m_dicItems.Count > 0)
		{
			return;
		}
		this.m_type = type;
		this.goGeRenContent.SetActive(this.m_type == MoYuRongYuType.GeRenRongYu);
		this.SetButtonState(this.btnGeRen, this.m_type == MoYuRongYuType.GeRenRongYu);
		this.goZhanDuiContent.SetActive(this.m_type == MoYuRongYuType.ZhanDuiRongYu);
		this.SetButtonState(this.btnZhanDui, this.m_type == MoYuRongYuType.ZhanDuiRongYu);
		if (this.m_type == MoYuRongYuType.GeRenRongYu)
		{
			if (this.gridGeRenRongYu.transform.childCount == 0 && this.m_rongYuServerData != null)
			{
				this.LoadGeRenContent();
			}
		}
		else if (this.m_type != MoYuRongYuType.ZhanDuiRongYu || this.gridZhanDuiRongYu.transform.childCount != 0 || this.m_rongYuServerData != null)
		{
		}
	}

	private void LoadGeRenContent()
	{
		List<ZorkAchievementVO> lstZorkAchievementVOCfg = IConfigbase<ConfigMoYuDuoBao>.Instance.LstZorkAchievementVOCfg;
		for (int i = 0; i < lstZorkAchievementVOCfg.Count; i++)
		{
			ZorkAchievementVO achievementVO = lstZorkAchievementVOCfg[i];
			MoYuDuoBaoPartRongYuItem moYuDuoBaoPartRongYuItem = this.LoadItem(achievementVO, this.gridGeRenRongYu.transform);
		}
		this.gridGeRenRongYu.Reposition();
	}

	private MoYuDuoBaoPartRongYuItem LoadItem(ZorkAchievementVO achievementVO, Transform parent)
	{
		MoYuDuoBaoPartRongYuItem moYuDuoBaoPartRongYuItem = U3DUtils.NEW<MoYuDuoBaoPartRongYuItem>();
		moYuDuoBaoPartRongYuItem.transform.SetParent(parent);
		moYuDuoBaoPartRongYuItem.transform.localPosition = Vector3.zero;
		moYuDuoBaoPartRongYuItem.transform.localScale = Vector3.one;
		moYuDuoBaoPartRongYuItem.ZorkAchievement = achievementVO;
		moYuDuoBaoPartRongYuItem.SetFinishNum(this.GetTagFinishNum(achievementVO.AchievementTarget));
		int num = 2;
		if (!this.IsChengJiuFinish(achievementVO))
		{
			num = 2;
		}
		else if (this.IsChengJiuFinish(achievementVO) && !this.IsRewardLingQu(achievementVO.ID))
		{
			num = 1;
		}
		else if (this.IsChengJiuFinish(achievementVO) && this.IsRewardLingQu(achievementVO.ID))
		{
			num = 3;
		}
		moYuDuoBaoPartRongYuItem.SetState(num);
		moYuDuoBaoPartRongYuItem.OnLingQu = new Action<MoYuDuoBaoPartRongYuItem>(this.LingQuJiangLi);
		moYuDuoBaoPartRongYuItem.gameObject.name = string.Concat(new object[]
		{
			num,
			string.Empty,
			1000 + achievementVO.ID,
			"RongYuItem"
		});
		this.m_dicItems[achievementVO.ID] = moYuDuoBaoPartRongYuItem;
		return moYuDuoBaoPartRongYuItem;
	}

	private void LingQuJiangLi(MoYuDuoBaoPartRongYuItem item)
	{
		this.SendDuoGetAward(item);
	}

	private void SetButtonState(GButton button, bool beSelected)
	{
		string text = string.Empty;
		if (button == this.btnGeRen)
		{
			text = Global.GetLang("个人成就");
		}
		else if (button == this.btnZhanDui)
		{
			text = Global.GetLang("战队成就");
		}
		string text2 = (!beSelected) ? "808081" : "fac60d";
		string text3 = (!beSelected) ? "teamTab_normal" : "teamTab_hover";
		button.Text = Global.GetColorStringForNGUIText(new object[]
		{
			text2,
			text
		});
		button.normalSprite = text3;
		button.target.spriteName = text3;
		button.hoverSprite = text3;
		button.pressedSprite = "teamTab_hover";
	}

	private bool IsChengJiuFinish(ZorkAchievementVO achievement)
	{
		int tagFinishNum = this.GetTagFinishNum(achievement.AchievementTarget);
		return tagFinishNum >= achievement.TargetNum;
	}

	private int GetTagFinishNum(int tagType)
	{
		int result = 0;
		if (this.m_rongYuServerData != null && this.m_rongYuServerData.listAnalysisData != null)
		{
			int num = tagType - 1;
			if (this.m_rongYuServerData.listAnalysisData.Count > num)
			{
				result = this.m_rongYuServerData.listAnalysisData[num];
			}
		}
		return result;
	}

	private bool IsRewardLingQu(int tagId)
	{
		bool result = false;
		if (this.m_rongYuServerData != null && this.m_rongYuServerData.ArchievementAwardDict != null)
		{
			int num = 0;
			if (this.m_rongYuServerData.ArchievementAwardDict.TryGetValue(tagId, ref num))
			{
				result = (num == 1);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"获取成就领取状态错误"
				});
			}
		}
		return result;
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<ZorkBattleBaseData>("CMD_SPR_ZORK_BASE_DATA", new Action<ZorkBattleBaseData>(this.GetRongYuInfo));
		MUEventManager.AddEventListener<int>("CMD_SPR_ZORK_AWARD_GET", new Action<int>(this.GetAwardResult));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleBaseData>("CMD_SPR_ZORK_BASE_DATA", new Action<ZorkBattleBaseData>(this.GetRongYuInfo));
		MUEventManager.RemoveEventListener<int>("CMD_SPR_ZORK_AWARD_GET", new Action<int>(this.GetAwardResult));
	}

	private void GetRongYuInfo()
	{
		GameInstance.Game.SendDuoBaoGetBaseInfo();
	}

	private void GetRongYuInfo(ZorkBattleBaseData data)
	{
		this.m_rongYuServerData = data;
		this.OnSelectContent(this.m_type);
	}

	private void SendDuoGetAward(MoYuDuoBaoPartRongYuItem item)
	{
		this.m_lingQu = item;
		GameInstance.Game.SendDuoGetAward(this.m_lingQu.ZorkAchievement.ID);
	}

	private void GetAwardResult(int ret)
	{
		this.m_lingQu.SetState(3);
	}

	public GButton btnClose;

	public GButton btnGeRen;

	public GButton btnZhanDui;

	public GameObject goGeRenContent;

	public GameObject goZhanDuiContent;

	public UIGrid gridGeRenRongYu;

	public UIGrid gridZhanDuiRongYu;

	private MoYuRongYuType m_type;

	private ZorkBattleBaseData m_rongYuServerData;

	private Dictionary<int, MoYuDuoBaoPartRongYuItem> m_dicItems = new Dictionary<int, MoYuDuoBaoPartRongYuItem>();

	private MoYuDuoBaoPartRongYuItem m_lingQu;
}
