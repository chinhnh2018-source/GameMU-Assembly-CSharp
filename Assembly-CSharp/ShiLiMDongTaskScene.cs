using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class ShiLiMDongTaskScene : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._InfLabel.text = string.Empty;
		this._BtnBattleInfBtn.Text = Global.GetLang("战场信息");
		this._BtnBattleInfBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.mPartType = ShiLiMDongTaskScene.PartType.BattleInf;
			this.RefreshBtns();
		};
		this._RankInfBtn.Text = Global.GetLang("资源排名");
		this._RankInfBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.mPartType = ShiLiMDongTaskScene.PartType.RankInf;
			this.RefreshBtns();
		};
		this._RankBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.ShowMiDongPaiHangWindow();
		};
		this.RefreshBtns();
		this.RefreshContent();
		this._InfScoreName.text = Global.GetLang("我的积分");
		this._InfScoreValue.text = "0";
	}

	public override void Update()
	{
		base.Update();
		if (this.m_nowScore != Global.Data.roleData.MoneyData[145])
		{
			this.m_nowScore = Global.Data.roleData.MoneyData[145];
			this._InfScoreValue.text = this.m_nowScore.ToString();
		}
	}

	private void RefreshBtns()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (this.mPartType == ShiLiMDongTaskScene.PartType.RankInf)
		{
			text = "mainTaskTab";
			text2 = "mainTaskBak_normal";
		}
		else
		{
			text = "mainTaskBak_normal";
			text2 = "mainTaskTab";
		}
		this._BtnBattleInfBtn.normalSprite = text2;
		this._BtnBattleInfBtn.hoverSprite = text2;
		this._BtnBattleInfBtn.pressedSprite = text2;
		this._BtnBattleInfBtn.Refresh();
		this._RankInfBtn.normalSprite = text;
		this._RankInfBtn.hoverSprite = text;
		this._RankInfBtn.pressedSprite = text;
		this._RankInfBtn.Refresh();
		this.RefreshContent();
	}

	private void RefreshContent()
	{
		if (this.mCompMineSideScore == null)
		{
			this._InfLabel.text = string.Empty;
		}
		else if (this.mPartType == ShiLiMDongTaskScene.PartType.RankInf)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.mCompMineSideScore.ResJiFenList != null && 0 < this.mCompMineSideScore.ResJiFenList.Count)
			{
				for (int i = 0; i < this.mCompMineSideScore.ResJiFenList.Count; i++)
				{
					CompMineResData compMineResData = this.mCompMineSideScore.ResJiFenList[i];
					if (i == 0)
					{
						stringBuilder.AppendLine(Global.GetColorStringForNGUIText(new object[]
						{
							this.GetCompColor(compMineResData.CompType),
							this.GetCompName(compMineResData.CompType) + Global.GetLang("：") + compMineResData.MineJiFen
						}));
					}
					else if (i == 1)
					{
						stringBuilder.AppendLine(Global.GetColorStringForNGUIText(new object[]
						{
							this.GetCompColor(compMineResData.CompType),
							this.GetCompName(compMineResData.CompType) + Global.GetLang("：") + compMineResData.MineJiFen
						}));
					}
					else if (i == 2)
					{
						stringBuilder.AppendLine(Global.GetColorStringForNGUIText(new object[]
						{
							this.GetCompColor(compMineResData.CompType),
							this.GetCompName(compMineResData.CompType) + Global.GetLang("：") + compMineResData.MineJiFen
						}));
					}
				}
			}
			this._InfLabel.text = stringBuilder.ToString();
		}
		else
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine(Global.GetColorStringForNGUIText(new object[]
			{
				"ddd2bd",
				string.Concat(new object[]
				{
					Global.GetLang("已出发矿车："),
					this.mCompMineSideScore.MineTruckGo,
					"/",
					IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineTruckCount()
				})
			}));
			stringBuilder2.AppendLine(Global.GetColorStringForNGUIText(new object[]
			{
				"2800ff",
				Global.GetLang("安全送达：") + this.mCompMineSideScore.SafeArrived
			}));
			stringBuilder2.AppendLine(Global.GetColorStringForNGUIText(new object[]
			{
				"ca9f13",
				Global.GetLang("当前矿车路程：") + this.mCompMineSideScore.MineTruckProcess + "%"
			}));
			this._InfLabel.text = stringBuilder2.ToString();
		}
	}

	private string GetCompName(int id)
	{
		if (id == 1)
		{
			return Global.GetLang("教团");
		}
		if (id == 2)
		{
			return Global.GetLang("同盟");
		}
		if (id == 3)
		{
			return Global.GetLang("协会");
		}
		return string.Empty;
	}

	private string GetCompColor(int id)
	{
		if (id == 1)
		{
			return "fffff0";
		}
		if (id == 2)
		{
			return "ca9f13";
		}
		if (id == 3)
		{
			return "3681f3";
		}
		return "fffff0";
	}

	public void NoticeRefreshData(CompMineSideScore data)
	{
		this.mCompMineSideScore = data;
		this.RefreshContent();
	}

	[SerializeField]
	private GButton _BtnBattleInfBtn;

	[SerializeField]
	private GButton _RankInfBtn;

	[SerializeField]
	private UILabel _InfLabel;

	[SerializeField]
	private GButton _RankBtn;

	[SerializeField]
	private UILabel _InfScoreName;

	[SerializeField]
	private UILabel _InfScoreValue;

	private ShiLiMDongTaskScene.PartType mPartType;

	private CompMineSideScore mCompMineSideScore;

	private long m_nowScore;

	public enum PartType
	{
		BattleInf,
		RankInf
	}
}
