using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiPartPaiHang : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("本周势力军衔排行");
		this.lblTitlePaiMing.text = Global.GetLang("排名");
		this.lblName.text = Global.GetLang("角色名");
		this.lblJunXian.text = Global.GetLang("势力军衔");
		this.lblJunXian.lineWidth = 105;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.selfItem.gameObject.SetActive(false);
	}

	public void GetRankInfo(RankType type)
	{
		if (type == RankType.LastWeak)
		{
			this.lblTitle.text = Global.GetLang("上周势力军衔排行");
		}
		else
		{
			this.lblTitle.text = Global.GetLang("本周势力军衔排行");
		}
		this.SendGetRank(type);
	}

	private void InitInfo(List<CompRankInfo> ranks)
	{
		int paiMing = -1;
		int num = ranks.Count - 1;
		if (num < 7)
		{
		}
		for (int i = 0; i < ranks.Count - 1; i++)
		{
			ShiLiPartPaiHangItem shiLiPartPaiHangItem = U3DUtils.NEW<ShiLiPartPaiHangItem>();
			shiLiPartPaiHangItem.gameObject.name = "item" + i;
			shiLiPartPaiHangItem.transform.SetParent(this.grid.transform);
			shiLiPartPaiHangItem.transform.localScale = Vector3.one;
			shiLiPartPaiHangItem.transform.localPosition = Vector3.zero;
			if (i >= ranks.Count - 1)
			{
				shiLiPartPaiHangItem.InitNull();
			}
			else
			{
				shiLiPartPaiHangItem.Init(i + 1, ranks[i].Param1, ranks[i].Value);
				if (GameInstance.Game.CurrentSession.roleData != null)
				{
					if (ranks[i].Key == GameInstance.Game.CurrentSession.roleData.RoleID)
					{
						paiMing = i + 1;
					}
				}
			}
		}
		this.grid.Reposition();
		this.selfItem.gameObject.SetActive(true);
		int num2 = ranks.Count - 1;
		this.selfItem.Init(paiMing, Global.GetLang("我的排名"), ranks[num2].Value);
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

	private void InitTest(int num)
	{
		for (int i = 0; i < num; i++)
		{
			ShiLiPartPaiHangItem shiLiPartPaiHangItem = U3DUtils.NEW<ShiLiPartPaiHangItem>();
			shiLiPartPaiHangItem.gameObject.name = "item" + i;
			shiLiPartPaiHangItem.transform.SetParent(this.grid.transform);
			shiLiPartPaiHangItem.transform.localScale = Vector3.one;
			shiLiPartPaiHangItem.transform.localPosition = Vector3.zero;
			shiLiPartPaiHangItem.Init(i + 1, Global.GetLang("这是一个测试"), Random.Range(1, 99999));
		}
		this.grid.Reposition();
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
		MUEventManager.AddEventListener<List<CompRankInfo>>("CMD_SPR_COMP_RANK_INFO", new Action<List<CompRankInfo>>(this.ServerGetRankReslut));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<List<CompRankInfo>>("CMD_SPR_COMP_RANK_INFO", new Action<List<CompRankInfo>>(this.ServerGetRankReslut));
	}

	private void SendGetRank(RankType type)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.ShiLiGetRank((int)type);
	}

	private void ServerGetRankReslut(List<CompRankInfo> ranks)
	{
		this.InitInfo(ranks);
	}

	private const int OnceUnm = 7;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblTitlePaiMing;

	public UILabel lblName;

	public UILabel lblJunXian;

	public GButton btnClose;

	public UIGrid grid;

	public ShiLiPartPaiHangItem selfItem;
}
