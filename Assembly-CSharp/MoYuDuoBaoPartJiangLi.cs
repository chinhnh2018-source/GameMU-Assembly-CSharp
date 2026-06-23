using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class MoYuDuoBaoPartJiangLi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblDuanWei.text = Global.GetLang(string.Empty);
		this.lblDesWord.text = Global.GetLang("奖励说明 :");
		this.lblDesContent.text = Global.GetLang("每日魔域夺宝活动开启后，根据战队段位完成首场战斗胜利，即可领取首胜段位奖励，每个赛季结束后可以领取赛季段位奖励，段位越高奖励越好。");
		this.lblRewardWord.text = Global.GetLang("首胜段位奖励 :");
		this.lblYuLan.text = Global.GetLang("赛季段位奖励预览");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.initInfo();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(this, null);
			}
		};
		this.GetRongYuInfo();
	}

	private void initInfo()
	{
		Dictionary<int, ZorkDanAwardVO> dictZorkDanAwardVOCfg = IConfigbase<ConfigMoYuDuoBao>.Instance.DictZorkDanAwardVOCfg;
		for (int i = 5; i > 0; i--)
		{
			MoYuDuoBaoPartJiangLiItem moYuDuoBaoPartJiangLiItem = U3DUtils.NEW<MoYuDuoBaoPartJiangLiItem>();
			moYuDuoBaoPartJiangLiItem.transform.SetParent(this.gridDuanWeiItem.transform);
			moYuDuoBaoPartJiangLiItem.transform.localPosition = Vector3.zero;
			moYuDuoBaoPartJiangLiItem.transform.localScale = Vector3.one;
			moYuDuoBaoPartJiangLiItem.AwardVO = dictZorkDanAwardVOCfg[i];
		}
		this.gridDuanWeiItem.Reposition();
	}

	private void initSelfDuanWei()
	{
		if (this.n_DanAwardVO == null)
		{
			Debug.LogError(Global.GetLang("段位错误"));
		}
		else
		{
			this.lblDuanWei.text = Global.GetLang("当前段位：") + this.n_DanAwardVO.RankLevel + Global.GetLang("段");
			List<string> rewardStr = new List<string>(this.n_DanAwardVO.FirstBattleReward);
			Global.LoadReward(rewardStr, this.gridReward, 70f, true);
		}
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
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleBaseData>("CMD_SPR_ZORK_BASE_DATA", new Action<ZorkBattleBaseData>(this.GetRongYuInfo));
	}

	private void GetRongYuInfo()
	{
		GameInstance.Game.SendDuoBaoGetBaseInfo();
	}

	private void GetRongYuInfo(ZorkBattleBaseData data)
	{
		int num = 1;
		if (data != null)
		{
			num = data.TeamDuanWei;
		}
		if (num <= 0)
		{
			num = 1;
		}
		this.n_DanAwardVO = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkDanAwardVODataById(num);
		this.initSelfDuanWei();
	}

	public UILabel lblDuanWei;

	public UILabel lblDesWord;

	public UILabel lblDesContent;

	public UILabel lblRewardWord;

	public UILabel lblYuLan;

	public GButton btnClose;

	public UIGrid gridDuanWeiItem;

	public UIGrid gridReward;

	private ZorkDanAwardVO n_DanAwardVO;
}
