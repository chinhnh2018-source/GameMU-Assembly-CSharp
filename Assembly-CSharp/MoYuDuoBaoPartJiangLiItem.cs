using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

public class MoYuDuoBaoPartJiangLiItem : UserControl
{
	public ZorkDanAwardVO AwardVO
	{
		get
		{
			return this.m_AwardVO;
		}
		set
		{
			this.m_AwardVO = value;
			this.ResetInfo();
		}
	}

	private void InitTextInPrefabs()
	{
		this.lblDuanName.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	private void ResetInfo()
	{
		this.imgDuanWei.spriteName = "chengjiu" + this.m_AwardVO.ID;
		this.lblDuanName.text = this.m_AwardVO.RankLevel + Global.GetLang("段");
		List<string> rewardStr = new List<string>(this.m_AwardVO.SeasonReward);
		Global.LoadReward(rewardStr, this.gridReward, 70f, true);
	}

	public UISprite imgDuanWei;

	public UILabel lblDuanName;

	public UIGrid gridReward;

	private ZorkDanAwardVO m_AwardVO;
}
