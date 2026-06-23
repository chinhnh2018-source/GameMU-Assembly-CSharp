using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiMiDongPaiHangPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("积分排名");
		this.lblTitlePaiMing.text = Global.GetLang("排名");
		this.lblName.text = Global.GetLang("角色名");
		this.lblJunXian.text = Global.GetLang("战场积分");
		this._MyRankLabel.pivot = 5;
		this._MyRankLabel.transform.localPosition = new Vector3(30f, 0f, this._MyRankLabel.transform.localPosition.z);
		this._MyScoreLabel.pivot = 5;
		this._MyScoreLabel.transform.localPosition = new Vector3(30f, -20f, this._MyScoreLabel.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Handler(this, new DPSelectedItemEventArgs());
		};
		this.selfItem.gameObject.SetActive(false);
		this.GetRankInfo();
	}

	public override void Update()
	{
		base.Update();
		this.mTime -= Time.deltaTime;
		if (0f >= this.mTime)
		{
			this.mTime = 10f;
		}
	}

	private void OnEnable()
	{
		this.mTime = 10f;
	}

	public void GetRankInfo()
	{
		this.lblTitle.text = Global.GetLang("积分排名");
		this.SendGetRank();
	}

	public void NoticeGetDataCallBack(CompMineSelfScore rankData)
	{
		if (null != this._NoDataLabel)
		{
			this._NoDataLabel.text = string.Empty;
		}
		int num = 0;
		List<CompRankInfo> list = new List<CompRankInfo>();
		if (rankData != null)
		{
			list = rankData.rankInfo2Client;
			num = list.Count;
		}
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
			if (i >= list.Count)
			{
				shiLiPartPaiHangItem.InitNull();
			}
			else
			{
				shiLiPartPaiHangItem.Select = false;
				shiLiPartPaiHangItem.Init(i + 1, list[i].Param1, list[i].Value);
				if (Global.Data != null)
				{
					if (list[i].Key == Global.Data.RoleID)
					{
						int num2 = i + 1;
						shiLiPartPaiHangItem.Select = true;
					}
				}
			}
		}
		this.grid.Reposition();
		string text = Global.GetLang("暂无");
		if (rankData != null)
		{
			text = ((rankData.RankNum != 0) ? rankData.RankNum.ToString() : Global.GetLang("暂无"));
		}
		this._MyRankLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ca9f43",
			Global.GetLang("我的排名：") + text
		});
		this._MyScoreLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ca9f43",
			Global.GetLang("我的积分：") + Global.Data.roleData.MoneyData[145].ToString()
		});
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

	private void SendGetRank()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendShiLiMiDongGetSelfScore();
	}

	private void ServerGetRankReslut(List<CompRankInfo> ranks)
	{
	}

	private const int OnceUnm = 7;

	public DPSelectedItemEventHandler Handler;

	public UILabel lblTitle;

	public UILabel lblTitlePaiMing;

	public UILabel lblName;

	public UILabel lblJunXian;

	public GButton btnClose;

	public UIGrid grid;

	[SerializeField]
	private UILabel _MyRankLabel;

	[SerializeField]
	private UILabel _MyScoreLabel;

	public ShiLiPartPaiHangItem selfItem;

	private float mTime = 10f;

	private UILabel _NoDataLabel;

	public enum RankType
	{

	}
}
