using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class MoYuDuoBaoPartRank : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitleZhanLi1.text = Global.GetLang("排名");
		this.lblTitleZhanLi2.text = Global.GetLang("战队名称");
		this.lblTitleZhanLi3.text = Global.GetLang("战队段位");
		this.lblTitleZhanLi4.text = Global.GetLang("积分");
		this.lblTitleMVP1.text = Global.GetLang("排名");
		this.lblTitleMVP2.text = Global.GetLang("玩家名称");
		this.lblTitleMVP3.text = Global.GetLang("重生等级");
		this.lblTitleMVP4.text = Global.GetLang("击杀数量");
		this.btnRankZhanLi.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("战队排名")
		});
		this.btnRankMVP.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("个人击杀排名")
		});
		this.lblZhanLiWu.text = Global.GetLang("暂无数据");
		this.lblMVPWu.text = Global.GetLang("暂无数据");
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
		this.btnRankZhanLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectRankType(MoYuDuoBaoRankType.ZhanLiType);
		};
		this.btnRankMVP.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectRankType(MoYuDuoBaoRankType.MVPType);
		};
		this.OnSelectRankType(MoYuDuoBaoRankType.ZhanLiType);
		this.GetRankInfo();
	}

	private void OnSelectRankType(MoYuDuoBaoRankType type)
	{
		if (this.m_type == type && this.m_rankInfo != null)
		{
			return;
		}
		this.SetButtonState(this.btnRankZhanLi, type == MoYuDuoBaoRankType.ZhanLiType);
		this.SetButtonState(this.btnRankMVP, type == MoYuDuoBaoRankType.MVPType);
		if (type == MoYuDuoBaoRankType.ZhanLiType)
		{
			this.goZhanLiContent.SetActive(true);
			this.goMVPContent.SetActive(false);
			if (this.m_rankInfo == null)
			{
				return;
			}
			this.m_type = type;
			this.StartLoadZhanLiRank();
		}
		else if (type == MoYuDuoBaoRankType.MVPType)
		{
			this.goZhanLiContent.SetActive(false);
			this.goMVPContent.SetActive(true);
			if (this.m_rankInfo == null)
			{
				return;
			}
			this.m_type = type;
			this.StartLoadMVPRank();
		}
	}

	private void SetButtonState(GButton button, bool beSelected)
	{
		string text = string.Empty;
		if (button == this.btnRankZhanLi)
		{
			text = Global.GetLang("战队排名");
		}
		else if (button == this.btnRankMVP)
		{
			text = Global.GetLang("个人击杀排名");
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

	private void StartLoadZhanLiRank()
	{
		if (this.gridZhanLi.transform.childCount > 0)
		{
			return;
		}
		List<KFZorkRankInfo> list = null;
		if (this.m_rankInfo.rankInfo2Client != null)
		{
			if (this.m_rankInfo.rankInfo2Client.TryGetValue(0, ref list))
			{
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						int rank = i + 1;
						string strParam = list[i].StrParam1;
						int param = list[i].Param1;
						string rankLevel = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkDanAwardVODataByScore(list[i].Value).RankLevel;
						string content = list[i].Value.ToString();
						MoYuDuoBaoPartRankItem moYuDuoBaoPartRankItem = this.LoadItem(this.gridZhanLi.transform, rank, strParam, rankLevel, content);
					}
					this.gridZhanLi.Reposition();
					if (list.Count == 0)
					{
						this.lblZhanLiWu.gameObject.SetActive(true);
					}
					else
					{
						this.lblZhanLiWu.gameObject.SetActive(false);
					}
				}
				else
				{
					this.lblZhanLiWu.gameObject.SetActive(true);
				}
			}
			else
			{
				this.lblZhanLiWu.gameObject.SetActive(true);
			}
		}
		else
		{
			this.lblZhanLiWu.gameObject.SetActive(true);
		}
	}

	private void StartLoadMVPRank()
	{
		if (this.gridMVP.transform.childCount > 0)
		{
			return;
		}
		List<KFZorkRankInfo> list = null;
		if (this.m_rankInfo.rankInfo2Client != null)
		{
			if (this.m_rankInfo.rankInfo2Client.TryGetValue(1, ref list))
			{
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						int rank = i + 1;
						string strParam = list[i].StrParam1;
						int param = list[i].Param1;
						string content = string.Format(Global.GetLang("重生{0}级"), param);
						string content2 = list[i].Value.ToString();
						MoYuDuoBaoPartRankItem moYuDuoBaoPartRankItem = this.LoadItem(this.gridMVP.transform, rank, strParam, content, content2);
					}
					this.gridMVP.Reposition();
					if (list.Count == 0)
					{
						this.lblMVPWu.gameObject.SetActive(true);
					}
					else
					{
						this.lblMVPWu.gameObject.SetActive(false);
					}
				}
				else
				{
					this.lblMVPWu.gameObject.SetActive(true);
				}
			}
			else
			{
				this.lblMVPWu.gameObject.SetActive(true);
			}
		}
		else
		{
			this.lblMVPWu.gameObject.SetActive(true);
		}
	}

	private MoYuDuoBaoPartRankItem LoadItem(Transform parent, int rank, string content2, string content3, string content4)
	{
		MoYuDuoBaoPartRankItem moYuDuoBaoPartRankItem = U3DUtils.NEW<MoYuDuoBaoPartRankItem>();
		moYuDuoBaoPartRankItem.transform.SetParent(parent);
		moYuDuoBaoPartRankItem.InitInfo(rank, content2, content3, content4);
		moYuDuoBaoPartRankItem.transform.localPosition = Vector3.zero;
		moYuDuoBaoPartRankItem.transform.localScale = Vector3.one;
		return moYuDuoBaoPartRankItem;
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
		MUEventManager.AddEventListener<ZorkBattleRankInfo>("CMD_SPR_ZORK_RANK_INFO", new Action<ZorkBattleRankInfo>(this.ServerGetRankInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleRankInfo>("CMD_SPR_ZORK_RANK_INFO", new Action<ZorkBattleRankInfo>(this.ServerGetRankInfo));
	}

	private void GetRankInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendDuoRankInfo();
	}

	private void ServerGetRankInfo(ZorkBattleRankInfo data)
	{
		this.m_rankInfo = data;
		this.OnSelectRankType(MoYuDuoBaoRankType.ZhanLiType);
	}

	public UILabel lblTitleZhanLi1;

	public UILabel lblTitleZhanLi2;

	public UILabel lblTitleZhanLi3;

	public UILabel lblTitleZhanLi4;

	public UILabel lblTitleMVP1;

	public UILabel lblTitleMVP2;

	public UILabel lblTitleMVP3;

	public UILabel lblTitleMVP4;

	public GButton btnClose;

	public GButton btnRankZhanLi;

	public GButton btnRankMVP;

	public UILabel lblZhanLiWu;

	public UILabel lblMVPWu;

	public GameObject goZhanLiContent;

	public GameObject goMVPContent;

	public UIGrid gridZhanLi;

	public UIGrid gridMVP;

	private MoYuDuoBaoRankType m_type;

	private ZorkBattleRankInfo m_rankInfo;
}
