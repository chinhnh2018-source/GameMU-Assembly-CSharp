using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiBattlePartPaiHang : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("积分排行");
		this.lblTitlePaiMing.text = Global.GetLang("排名");
		this.lblName.text = Global.GetLang("角色名称");
		this.lblJiFen.text = Global.GetLang("争霸积分");
		this.lblMyPaiHangWord.text = Global.GetLang("我的排行:");
		this.lblMyJiFenWord.text = Global.GetLang("我的积分:");
		this.lblMyPaiHangWord.pivot = 5;
		this.lblMyPaiHangWord.transform.localPosition = new Vector3(150f, this.lblMyPaiHangWord.transform.localPosition.y, this.lblMyPaiHangWord.transform.localPosition.z);
		this.lblMyJiFenWord.pivot = 5;
		this.lblMyJiFenWord.transform.localPosition = new Vector3(150f, this.lblMyJiFenWord.transform.localPosition.y, this.lblMyJiFenWord.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.SendGetRank();
	}

	private void InitRankInfo(CompBattleSelfScore ranks)
	{
		this.lblMyJiFen.text = Global.Data.roleData.MoneyData[143].ToString();
		if (ranks == null)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
			return;
		}
		if (ranks.RankNum <= 0)
		{
			this.lblMyPaiHang.text = Global.GetLang("无排名");
		}
		else
		{
			this.lblMyPaiHang.text = ranks.RankNum.ToString();
		}
		if (ranks.rankInfo2Client == null)
		{
			ranks.rankInfo2Client = new List<CompRankInfo>();
		}
		this.InitInfo(ranks.rankInfo2Client);
	}

	private void InitInfo(List<CompRankInfo> ranks)
	{
		int num = ranks.Count;
		if (num < 7)
		{
			num = 7;
		}
		for (int i = 0; i < num; i++)
		{
			ShiLiPartPaiHangItem shiLiPartPaiHangItem = U3DUtils.NEW<ShiLiPartPaiHangItem>();
			shiLiPartPaiHangItem.gameObject.name = "item" + i;
			shiLiPartPaiHangItem.transform.SetParent(this.grid.transform);
			shiLiPartPaiHangItem.transform.localScale = Vector3.one;
			shiLiPartPaiHangItem.transform.localPosition = Vector3.zero;
			if (i >= ranks.Count)
			{
				shiLiPartPaiHangItem.InitNull();
			}
			else
			{
				shiLiPartPaiHangItem.Init(i + 1, ranks[i].Param1, ranks[i].Value);
			}
		}
		this.grid.Reposition();
	}

	private List<CompRankInfo> GetTest(int num)
	{
		List<CompRankInfo> list = new List<CompRankInfo>();
		for (int i = 0; i < num; i++)
		{
			list.Add(new CompRankInfo
			{
				Key = i,
				Value = Random.Range(1, 99999),
				Param1 = Global.GetLang("这是一个测试") + i
			});
		}
		return list;
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
		MUEventManager.AddEventListener<CompBattleSelfScore>("CMD_SPR_COMP_BATTLE_SELF_SCORE", new Action<CompBattleSelfScore>(this.ServerGetRankReslut));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<CompBattleSelfScore>("CMD_SPR_COMP_BATTLE_SELF_SCORE", new Action<CompBattleSelfScore>(this.ServerGetRankReslut));
	}

	private void SendGetRank()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.ShiLiGetCompBattleSelfScore();
	}

	private void ServerGetRankReslut(CompBattleSelfScore ranks)
	{
		this.InitRankInfo(ranks);
	}

	private const int OnceUnm = 7;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTitlePaiMing;

	public UILabel lblName;

	public UILabel lblJiFen;

	public GButton btnClose;

	public UIGrid grid;

	public UILabel lblMyPaiHangWord;

	public UILabel lblMyJiFenWord;

	public UILabel lblMyPaiHang;

	public UILabel lblMyJiFen;
}
