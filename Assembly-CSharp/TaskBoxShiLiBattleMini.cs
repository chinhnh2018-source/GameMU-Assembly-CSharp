using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class TaskBoxShiLiBattleMini : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnShiLi.Text = Global.GetLang("势力信息");
		this.btnZhuJiang.Text = Global.GetLang("据点信息");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnShiLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.btnShiLi.target.alpha = 1f;
			this.btnShiLi.Label.alpha = 1f;
			this.btnZhuJiang.target.alpha = 0.4f;
			this.btnZhuJiang.Label.alpha = 0.4f;
			this.zhuJiangRoot.gameObject.SetActive(true);
			this.juDianRoot.gameObject.SetActive(false);
		};
		this.btnZhuJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.btnShiLi.target.alpha = 0.4f;
			this.btnShiLi.Label.alpha = 0.4f;
			this.btnZhuJiang.target.alpha = 1f;
			this.btnZhuJiang.Label.alpha = 1f;
			this.zhuJiangRoot.gameObject.SetActive(false);
			this.juDianRoot.gameObject.SetActive(true);
		};
		this.btnPaiHang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnOpenPaiHangWindow();
		};
		this.InitInfo();
	}

	private void LoadTest()
	{
		List<CompBattleSideScore> list = new List<CompBattleSideScore>();
		CompBattleSideScore compBattleSideScore = new CompBattleSideScore();
		compBattleSideScore.CompType = 1;
		compBattleSideScore.ZhuJiangNum = 3;
		compBattleSideScore.StrongholdSet.Add(1);
		compBattleSideScore.StrongholdSet.Add(2);
		CompBattleSideScore compBattleSideScore2 = new CompBattleSideScore();
		compBattleSideScore2.CompType = 2;
		compBattleSideScore2.ZhuJiangNum = 2;
		compBattleSideScore2.StrongholdSet.Add(5);
		compBattleSideScore.StrongholdSet.Add(6);
		CompBattleSideScore compBattleSideScore3 = new CompBattleSideScore();
		compBattleSideScore3.CompType = 3;
		compBattleSideScore3.ZhuJiangNum = 2;
		compBattleSideScore3.StrongholdSet.Add(3);
		compBattleSideScore.StrongholdSet.Add(4);
		list.Add(compBattleSideScore);
		list.Add(compBattleSideScore2);
		list.Add(compBattleSideScore3);
		this.UpdateInfo(list);
	}

	private void InitInfo()
	{
		if (ShiLiData.GetBattleCity() == null)
		{
			return;
		}
		List<MUForceStronghold> forceStrongholdsByMapCode = ShiLiData.GetAllForceStronghold().GetForceStrongholdsByMapCode(ShiLiData.GetBattleCity().MapCode);
		for (int i = 0; i < forceStrongholdsByMapCode.Count; i++)
		{
			TaskBoxJuDianItem taskBoxJuDianItem = U3DUtils.NEW<TaskBoxJuDianItem>();
			taskBoxJuDianItem.gameObject.name = "juDianItem" + i;
			taskBoxJuDianItem.transform.SetParent(this.juDianItemContainer.transform);
			taskBoxJuDianItem.transform.localScale = Vector3.one;
			taskBoxJuDianItem.transform.localPosition = new Vector3(0f, (float)(0 - 25 * i), 0f);
			taskBoxJuDianItem.SetJuDianInfo(forceStrongholdsByMapCode[i], ShiLiType.None);
			this.m_lstJuDian.Add(taskBoxJuDianItem);
		}
		for (int j = 0; j < 3; j++)
		{
			TaskBoxShiLiItem taskBoxShiLiItem = U3DUtils.NEW<TaskBoxShiLiItem>();
			taskBoxShiLiItem.gameObject.name = "ShiLiItem" + j;
			taskBoxShiLiItem.transform.SetParent(this.shiLiItemContainer.transform);
			taskBoxShiLiItem.transform.localScale = Vector3.one;
			taskBoxShiLiItem.transform.localPosition = new Vector3(0f, (float)(0 - 50 * j), 0f);
			taskBoxShiLiItem.SetInfo((ShiLiType)j, 0f, 0);
			this.m_lstShiLi.Add(taskBoxShiLiItem);
		}
		this.zhuJiangRoot.gameObject.SetActive(true);
		this.juDianRoot.gameObject.SetActive(false);
	}

	private void UpdateInfo(List<CompBattleSideScore> scoreInfo)
	{
		List<TaskBoxShiLiBattleMini.SideScoreInfo> list = new List<TaskBoxShiLiBattleMini.SideScoreInfo>();
		List<int> list2 = new List<int>();
		for (int i = 0; i < scoreInfo.Count; i++)
		{
			TaskBoxShiLiBattleMini.SideScoreInfo sideScoreInfo = new TaskBoxShiLiBattleMini.SideScoreInfo();
			sideScoreInfo.compType = (ShiLiType)scoreInfo[i].CompType;
			sideScoreInfo.zhuJiangNum = scoreInfo[i].ZhuJiangNum;
			sideScoreInfo.zhanBi = 0f;
			if (scoreInfo[i].StrongholdSet != null)
			{
				foreach (int num in scoreInfo[i].StrongholdSet)
				{
					TaskBoxJuDianItem juDianById = this.GetJuDianById(num);
					if (juDianById != null)
					{
						list2.Add(num);
						juDianById.SetShiLi(sideScoreInfo.compType);
						sideScoreInfo.zhanBi += juDianById.JuDianInfo.Rate;
					}
				}
				list.Add(sideScoreInfo);
			}
		}
		for (int j = 0; j < this.m_lstJuDian.Count; j++)
		{
			if (list2.IndexOf(this.m_lstJuDian[j].JuDianInfo.ID) < 0)
			{
				this.m_lstJuDian[j].SetShiLi(ShiLiType.None);
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			this.m_lstShiLi[k].SetInfo(list[k].compType, list[k].zhanBi, list[k].zhuJiangNum);
		}
	}

	private void ReSortShiLiInfo(List<TaskBoxShiLiBattleMini.SideScoreInfo> lstScore)
	{
		lstScore.Sort(delegate(TaskBoxShiLiBattleMini.SideScoreInfo s1, TaskBoxShiLiBattleMini.SideScoreInfo s2)
		{
			float num = s2.zhanBi - s1.zhanBi;
			if (num > 0f)
			{
				return 1;
			}
			if (num < 0f)
			{
				return -1;
			}
			return 0;
		});
	}

	private TaskBoxJuDianItem GetJuDianById(int juDianId)
	{
		return this.m_lstJuDian.Find((TaskBoxJuDianItem info) => info.GetJuDianId() == juDianId);
	}

	private new void Update()
	{
		if (this.m_nowScore != Global.Data.roleData.MoneyData[143])
		{
			this.m_nowScore = Global.Data.roleData.MoneyData[143];
			this.lblScore.text = this.m_nowScore.ToString();
		}
	}

	private void OnOpenPaiHangWindow()
	{
		if (this.m_paiHangWindow == null)
		{
			this.m_paiHangWindow = U3DUtils.NEW<GChildWindow>();
			this.m_paiHangWindow.IsShowModal = true;
			this.m_paiHangWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_paiHangWindow, Global.GetLang("排行"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_paiHangWindow);
		}
		if (this.m_paiHangPart == null)
		{
			this.m_paiHangPart = U3DUtils.NEW<ShiLiBattlePartPaiHang>();
			this.m_paiHangPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ClosePaiHangWindow();
			};
		}
		this.m_paiHangWindow.SetContent(this.m_paiHangWindow.BodyPresenter, this.m_paiHangPart, 0.0, 0.0, true);
	}

	private void ClosePaiHangWindow()
	{
		if (null != this.m_paiHangPart)
		{
			this.m_paiHangPart.transform.parent = null;
			Object.Destroy(this.m_paiHangPart.gameObject);
			this.m_paiHangPart = null;
		}
		if (null != this.m_paiHangWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_paiHangWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_paiHangWindow, true);
			this.m_paiHangWindow = null;
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
		MUEventManager.AddEventListener<List<CompBattleSideScore>>("CMD_SPR_COMP_BATTLE_SIDE_SCORE", new Action<List<CompBattleSideScore>>(this.ServerGetBattleSideScore));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<List<CompBattleSideScore>>("CMD_SPR_COMP_BATTLE_SIDE_SCORE", new Action<List<CompBattleSideScore>>(this.ServerGetBattleSideScore));
	}

	private void ServerGetBattleSideScore(List<CompBattleSideScore> scoreInfo)
	{
		this.UpdateInfo(scoreInfo);
	}

	private const int juDianCellHeight = 25;

	private const int shiLiCellHeight = 50;

	public GButton btnShiLi;

	public GButton btnZhuJiang;

	public GameObject juDianRoot;

	public GameObject zhuJiangRoot;

	public GameObject juDianItemContainer;

	public GameObject shiLiItemContainer;

	public UILabel lblScore;

	public GButton btnPaiHang;

	private long m_nowScore;

	private List<TaskBoxJuDianItem> m_lstJuDian = new List<TaskBoxJuDianItem>();

	private List<TaskBoxShiLiItem> m_lstShiLi = new List<TaskBoxShiLiItem>();

	protected GChildWindow m_paiHangWindow;

	protected ShiLiBattlePartPaiHang m_paiHangPart;

	public class SideScoreInfo
	{
		public ShiLiType compType;

		public int zhuJiangNum;

		public float zhanBi;
	}
}
