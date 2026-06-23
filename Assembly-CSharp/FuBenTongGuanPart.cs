using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FuBenTongGuanPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("领取");
		if (this.ConstTexts != null && this.ConstTexts.Length == 1)
		{
			this.ConstTexts[0].Text = Global.GetLang("(点击卡片抽取奖励):");
		}
		this._AwardExp.X = 170.0;
		this.ConstTexts[0].X = -290.0;
		this._chouJiangSpr.localScale = new Vector3(161.019f, 54.7152f, 1f);
		this._chouJiangSpr.localPosition = new Vector3(-385f, 6f, 0f);
		this._Time.Pivot = 3;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
		this._ChouJiang.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnChouJiang);
		this._ChouJiangList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ChouJiangListClick);
		this._Submit.isEnabled = false;
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			long second = (Global.GetCorrectLocalTime() - this.StartTicks) / 1000L;
			if (second > 0L)
			{
				if (this.DelayShow)
				{
					this.ShowMainPart(true);
				}
				if (this.TimeLimit > second)
				{
					this._Time.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit - second);
				}
				else
				{
					this.OnChouJiang(null, null);
				}
			}
			else if (second <= 0L)
			{
				this._WaitTime.text = string.Format(Global.GetLang("你将于{0}秒后离开副本"), ColorCode.EncodingText((int)(-(int)second), "fd010c"));
			}
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	public void InitPartData(FuBenTongGuanData fuBenTongGuanData, long startTick = 0L)
	{
		this.ChouJiang_Count = 0;
		this.ChouJiang_CountMax = 1;
		this.StartTicks = startTick;
		if (this.StartTicks > Global.GetCorrectLocalTime())
		{
			this.DelayShow = true;
			this.ShowMainPart(false);
		}
		else
		{
			this.DelayShow = false;
			this.ShowMainPart(true);
		}
		this.TimeLimit = 20L;
		this._Time.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit);
		this._WaitTime.text = null;
		if (this != null && base.IsActive)
		{
			base.StartCoroutine<bool>(this.TickProc());
		}
		this.FuBenID = fuBenTongGuanData.FuBenID;
		this.ItemsIDList = fuBenTongGuanData.GoodsIDList;
		this.moJingCount = fuBenTongGuanData.AwardMoJing;
		this.ChouJiang_CountMax = 1;
		FuBenData fuBenData = Global.GetFuBenData(this.FuBenID);
		CopyScoreDataInfo copyScoreDataInfo = Global.GetCopyScoreDataInfo(this.FuBenID, fuBenTongGuanData.TotalScore);
		if (copyScoreDataInfo != null)
		{
			this.Score4 = copyScoreDataInfo.ScoreName;
		}
		else
		{
			this.Score4 = (fuBenTongGuanData.TimeScore + fuBenTongGuanData.KillScore + fuBenTongGuanData.DieScore).ToString();
		}
		this._ScoreList.ItemsSource.Clear();
		if (fuBenTongGuanData.MaxTimeScore > 0)
		{
			FuBenScoreItem fuBenScoreItem = U3DUtils.NEW<FuBenScoreItem>();
			this._ScoreList.ItemsSource.AddNoUpdate(fuBenScoreItem);
			fuBenScoreItem.Name = Global.GetLang("通关时间:");
			fuBenScoreItem.Number = UIHelper.FormatSecs((long)fuBenTongGuanData.UsedSecs, "-");
			fuBenScoreItem.Score = string.Format("{0}/{1}", fuBenTongGuanData.TimeScore, fuBenTongGuanData.MaxTimeScore);
		}
		if (fuBenTongGuanData.MaxKillScore > 0)
		{
			FuBenScoreItem fuBenScoreItem2 = U3DUtils.NEW<FuBenScoreItem>();
			this._ScoreList.ItemsSource.AddNoUpdate(fuBenScoreItem2);
			fuBenScoreItem2.Name = Global.GetLang("击杀数量:");
			fuBenScoreItem2.Number = fuBenTongGuanData.KillNum.ToString();
			fuBenScoreItem2.Score = string.Format("{0}/{1}", fuBenTongGuanData.KillScore, fuBenTongGuanData.MaxKillScore);
		}
		if (fuBenTongGuanData.MaxDieScore > 0)
		{
			FuBenScoreItem fuBenScoreItem3 = U3DUtils.NEW<FuBenScoreItem>();
			this._ScoreList.ItemsSource.AddNoUpdate(fuBenScoreItem3);
			fuBenScoreItem3.Name = Global.GetLang("死亡次数:");
			fuBenScoreItem3.Number = fuBenTongGuanData.DieCount.ToString();
			fuBenScoreItem3.Score = string.Format("{0}/{1}", fuBenTongGuanData.DieScore, fuBenTongGuanData.MaxDieScore);
		}
		this._ScoreList.ItemsSource.DelayUpdate();
		this.Score4 = this.Score4.toLowerCase();
		this._ScoreAnim1.SpriteName = this.Score4;
		this._ScoreAnim2.SpriteName = this.Score4;
		UIHelper.DelayInvoke(1f, delegate(object s, EventArgs e)
		{
			if (null != this._ScoreAnim && this._ScoreAnim)
			{
				this._ScoreAnim.gameObject.SetActive(true);
			}
		});
		this._AwardExp.text = fuBenTongGuanData.AwardExp.ToString();
		this._AwardJinBi.text = fuBenTongGuanData.AwardJinBi.ToString();
		this.AwardsData = UIHelper.ParseFuBenAwards(this.FuBenID);
		if (this.AwardsData != null)
		{
			this._JiangLiGoodsList.ItemsSource.Clear();
			UIHelper.AddAwardGoods(this._JiangLiGoodsList.ItemsSource, this.AwardsData.TaskawardList, null);
		}
		this._ChouJiangList.ItemsSource.Clear();
		for (int i = 0; i < 4; i++)
		{
			GGoodsCard ggoodsCard = U3DUtils.NEW<GGoodsCard>();
			ggoodsCard.Width = 78.0;
			ggoodsCard.Height = 78.0;
			ggoodsCard.OuterWidth = 133.0;
			ggoodsCard.OuterHeight = 191.0;
			this._ChouJiangList.ItemsSource.AddNoUpdate(ggoodsCard);
		}
		this._ChouJiangList.ItemsSource.DelayUpdate();
		this.RefreshUI();
	}

	private void ChouJiangListClick(object sender, EventArgs args)
	{
		if (null == this._ChouJiangList.SelectedItem)
		{
			return;
		}
		GGoodsCard ggoodsCard = U3DUtils.AS<GGoodsCard>(this._ChouJiangList.SelectedItem);
		if (null != ggoodsCard)
		{
			if (ggoodsCard.IsShow)
			{
				return;
			}
			if (this.ChouJiang_Count >= this.ChouJiang_CountMax)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("没有剩余抽奖次数"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (!ggoodsCard.IsShow)
			{
				if (this.ItemsIDList != null && this.ItemsIDList.Count > 0)
				{
					int num = this.ItemsIDList[this.ChouJiang_Count];
					int goodsIconCodeByID = Global.GetGoodsIconCodeByID(num);
					ggoodsCard._GoodsImg.Width = 78.0;
					ggoodsCard._GoodsImg.Height = 78.0;
					ggoodsCard.BodyURL = Global.GetGoodsIconString(goodsIconCodeByID);
					string goodsNameByID = Global.GetGoodsNameByID(num, false);
					ggoodsCard.Name = goodsNameByID;
					ggoodsCard.GoodsID = num;
				}
				else if (this.moJingCount >= 0)
				{
					ggoodsCard._GoodsImg.Width = 78.0;
					ggoodsCard._GoodsImg.Height = 78.0;
					ggoodsCard.BodyURL = Global.GetGoodsIconString(5050);
					ggoodsCard.Name = string.Format("{0}{1}", this.moJingCount, Global.GetLang("魔晶"));
					ggoodsCard.GoodsID = 5050;
				}
				ggoodsCard.TurnCard();
				this.ChouJiang_Count++;
				GameInstance.Game.SpriteGetCopyMapAwardCmd();
				this._Submit.isEnabled = true;
			}
			else
			{
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(ggoodsCard.GoodsID, ggoodsCard.ForgeLevel, ggoodsCard.ZhuijiaLevel, ggoodsCard.ExcellenceInfo, ggoodsCard.Lucky, ggoodsCard.Binding, ggoodsCard.GoodsCount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
			}
		}
	}

	private void RefreshUI()
	{
		for (int i = 0; i < this.JiangPinCount; i++)
		{
			GGoodsCard ggoodsCard;
			if (i < this._ChouJiangList.ItemsSource.Count)
			{
				ggoodsCard = U3DUtils.AS<GGoodsCard>(this._ChouJiangList.ItemsSource[i]);
			}
			else
			{
				ggoodsCard = U3DUtils.NEW<GGoodsCard>();
				ggoodsCard.Width = 78.0;
				ggoodsCard.Height = 78.0;
				ggoodsCard.OuterWidth = 133.0;
				ggoodsCard.OuterHeight = 191.0;
				this._ChouJiangList.ItemsSource.AddNoUpdate(ggoodsCard);
			}
			string goodsIconString;
			if (i < this.ShowIDList.Count && this.ShowIDList[i] > 0)
			{
				goodsIconString = Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(this.ShowIDList[i]));
			}
			else
			{
				goodsIconString = Global.GetGoodsIconString(-1);
			}
			ggoodsCard.BodyURL = goodsIconString;
		}
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		while (this.ChouJiang_Count < this.ChouJiang_CountMax)
		{
			this.ChouJiang_Count++;
			GameInstance.Game.SpriteGetCopyMapAwardCmd();
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(null, null);
		}
	}

	private void OnChouJiang(object sender, MouseEvent e)
	{
		this.OnSubmit(sender, e);
	}

	public void NotifyResult(int ret, int roleID, int newChangeLiftID)
	{
		if (ret == 1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("*************"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void ShowMainPart(bool show)
	{
		if (show)
		{
			if (this.DelayShow)
			{
				this.DelayShow = false;
				this.StartTicks = Global.GetCorrectLocalTime();
			}
			this._MainWindow.SetActive(true);
			this._WaitWindow.SetActive(false);
		}
		else
		{
			this._MainWindow.SetActive(false);
			this._WaitWindow.SetActive(true);
		}
	}

	public TextBlock _Title;

	public ListBox _ScoreList;

	public ListBox _JiangLiList;

	public TextBlock _AwardExp;

	public TextBlock _AwardJinBi;

	public ListBox _JiangLiGoodsList;

	public GButton _Submit;

	public TextBlock _Score4;

	public Animation _ScoreAnim;

	public CTongGuanScore _ScoreAnim1;

	public CTongGuanScore _ScoreAnim2;

	public TextBlock _Time;

	public TextBlock _Fee;

	public GButton _ChouJiang;

	public ListBox _ChouJiangList;

	public TextBlock _WaitTime;

	public GameObject _WaitWindow;

	public GameObject _MainWindow;

	public int FuBenID = -1;

	public int MapCode = -1;

	public TextBlock[] ConstTexts;

	private int ChouJiang_Count;

	private int ChouJiang_CountMax;

	private float TickInterval = 0.25f;

	private long StartTicks;

	private long TimeLimit = 60L;

	private string Score4 = "B";

	private TaskAwardsData AwardsData;

	private int JiangPinCount = 4;

	private List<int> ShowIDList = new List<int>();

	private List<int> ItemsIDList = new List<int>();

	private int moJingCount;

	private bool DelayShow;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Transform _chouJiangSpr;

	internal enum UIStates
	{
		Normal,
		Hide
	}
}
