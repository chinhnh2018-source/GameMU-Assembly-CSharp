using System;
using System.Collections;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RiChangPaTaPassPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ObservableCollection ItemCollectionAward
	{
		get
		{
			return this._ItemCollectionAward;
		}
		set
		{
			this._ItemCollectionAward = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnOk.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this._GoodCollection = this.mGoodList.ItemsSource;
		this.ItemCollection = this.ItemList.ItemsSource;
		this.ItemCollectionAward = this.AwardList.ItemsSource;
		this.BtnOk.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
		this.TxtName.Text = string.Empty;
	}

	public void InitPartData(FuBenTongGuanData fuBenTongGuanData, long startTick = 0L, int state = 0)
	{
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
		this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit);
		base.StartCoroutine<bool>(this.TickProc());
		this.ShowAnim((RiChangPaTaPassPart.PassState)state);
		this.refreshData(fuBenTongGuanData);
		if (this.IsHavePataNextIndex())
		{
			this.BtnOk.Text = Global.GetLang("进入下一层");
			this.BtnOk.TagIndex = 201;
		}
		else
		{
			this.BtnOk.Text = Global.GetLang("确定");
			this.BtnOk.TagIndex = 200;
		}
	}

	private void refreshData(FuBenTongGuanData fuBenTongGuanData)
	{
		int fuBenID = fuBenTongGuanData.FuBenID;
		int mapCode = fuBenTongGuanData.MapCode;
		int awardZhanGong = fuBenTongGuanData.AwardZhanGong;
		double awardRate = fuBenTongGuanData.AwardRate;
		if (Global.GetMapSceneUIClass() == SceneUIClasses.FamilyBoss && fuBenTongGuanData.AwardZhanGong == 0)
		{
			this.TxtTiShi.text = Global.GetLang("本周此BOSS通关奖励已领取");
			return;
		}
		this.TxtTiShi.text = string.Empty;
		XElement fuBenMapElement = Global.GetFuBenMapElement(fuBenID, mapCode);
		if (fuBenMapElement == null)
		{
			return;
		}
		int num = Global.GetXElementAttributeInt(fuBenMapElement, "XingHunaward") + Global.GetXElementAttributeInt(fuBenMapElement, "FirstXingHun");
		int num2 = Global.GetXElementAttributeInt(fuBenMapElement, "YuanSuFenMoaward");
		int num3 = fuBenTongGuanData.AwardExp;
		int num4 = fuBenTongGuanData.AwardJinBi;
		num3 = ((num3 > 0) ? num3 : 0);
		num4 = ((num4 > 0) ? num4 : 0);
		num = ((num >= 0) ? num : 0);
		int num5 = (awardZhanGong >= 0) ? awardZhanGong : 0;
		num2 = ((num2 >= 0) ? num2 : 0);
		this.ItemCollection.Clear();
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		if (fuBenTongGuanData.TreasureEventID != 0)
		{
			XElement cangBaoEventNode = CangBaoEventXml.GetCangBaoEventNode(fuBenTongGuanData.TreasureEventID);
			if (cangBaoEventNode != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(cangBaoEventNode, "NewGoods");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(cangBaoEventNode, "NewValue");
				if (!string.IsNullOrEmpty(xelementAttributeStr))
				{
					string[] array = xelementAttributeStr.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							UIHelper.AddAwardGoods(this.ItemCollection, array[i]);
						}
					}
				}
				if (!string.IsNullOrEmpty(xelementAttributeStr2))
				{
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < array2.Length; j++)
					{
						string[] array3 = array2[j].Split(new char[]
						{
							','
						});
						UIHelper.AddAwardData(this.AwardList.ItemsSource, (int.Parse(array3[0]) != 111) ? MoneyTypes.BaoZangJiFen : MoneyTypes.BaoZangXueZuan, int.Parse(array3[1]), "CTextAwards2");
					}
				}
			}
		}
		taskAwardsData.Experienceaward = (long)((int)((double)num3 * awardRate));
		taskAwardsData.Moneyaward = (int)((double)num4 * awardRate);
		taskAwardsData.XingHunaward = (int)((double)num * awardRate);
		taskAwardsData.JunGongaward = (int)((double)num5 * awardRate);
		taskAwardsData.FenMoAward = (int)((double)num2 * awardRate);
		UIHelper.AddAwardData(this.AwardList.ItemsSource, taskAwardsData, "CTextAwards2");
		UIHelper.AddAwardGoods(this.ItemCollection, Global.GetXElementAttributeStr(fuBenMapElement, "FirstGoodsID"));
		UIHelper.AddAwardGoods(this.ItemCollection, Global.GetXElementAttributeStr(fuBenMapElement, "GoodsIDs"));
		Transform transform = this.ItemList.transform;
		transform.localPosition = this.PosX(transform.localPosition, (float)(Mathf.Max(this.ItemList.Count() - 1, 0) * -39));
	}

	public void InitKuaFuPassData(HuanYingSiYuanAwardsData huanYingSiYuanAwardsData, long startTick)
	{
		if (huanYingSiYuanAwardsData == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		int successSide = huanYingSiYuanAwardsData.SuccessSide;
		long exp = huanYingSiYuanAwardsData.Exp;
		int shengWang = huanYingSiYuanAwardsData.ShengWang;
		int chengJiuAward = huanYingSiYuanAwardsData.ChengJiuAward;
		int awardsRate = huanYingSiYuanAwardsData.AwardsRate;
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
		this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit);
		base.StartCoroutine<bool>(this.TickProc());
		int state = (Global.Data.roleData.BattleWhichSide != successSide) ? 1 : 0;
		this.ShowAnim((RiChangPaTaPassPart.PassState)state);
		TaskAwardsData taskAwardsData = new TaskAwardsData();
		taskAwardsData.Experienceaward = exp;
		taskAwardsData.ShengwangAward = shengWang;
		taskAwardsData.RongYuaward = chengJiuAward;
		UIHelper.AddAwardDataRate(this.AwardList.ItemsSource, taskAwardsData, awardsRate, "CTextAwards2");
		Transform transform = this.ItemList.transform;
		transform.localPosition = this.PosX(transform.localPosition, (float)(Mathf.Max(this.ItemList.Count() - 1, 0) * -39));
		Super.LoadGoodsList(huanYingSiYuanAwardsData.AwardGoods, this._GoodCollection);
		if (this._GoodCollection.Count > 1)
		{
			Vector3 localPosition = this.mGoodList.transform.localPosition;
			localPosition.x -= (float)(this._GoodCollection.Count - 1) * (this.mGoodList.cellWidth / 2f);
			this.mGoodList.transform.localPosition = localPosition;
		}
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
					this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit - second);
				}
				else
				{
					this.OnSubmit(null, null);
				}
			}
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
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
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void ShowAnim(RiChangPaTaPassPart.PassState state)
	{
		this.Anim[0].gameObject.SetActive(false);
		this.Anim[1].gameObject.SetActive(false);
		if (state == RiChangPaTaPassPart.PassState.Success)
		{
			this.Anim[0].gameObject.SetActive(true);
		}
		else if (state == RiChangPaTaPassPart.PassState.Fail)
		{
			this.Anim[1].gameObject.SetActive(true);
			this.ItemList.gameObject.SetActive(false);
			this.TxtName.gameObject.SetActive(false);
		}
	}

	private Vector3 PosX(Vector3 v, float x)
	{
		v.x = x;
		return v;
	}

	public bool IsHavePataNextIndex()
	{
		int num = Global.Data.roleData.MapCode;
		num++;
		return num >= Global.GetPataIndexRange()[0] && num <= Global.GetPataIndexRange()[1];
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(null, new DPSelectedItemEventArgs
			{
				ID = this.BtnOk.TagIndex
			});
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox ItemList;

	public ListBox AwardList;

	public GButton BtnOk;

	public TextBlock TxtTime;

	public TextBlock TxtName;

	public UILabel TxtTiShi;

	public Animator[] Anim;

	public ListBox mGoodList;

	private float TickInterval = 0.25f;

	private long StartTicks;

	private long TimeLimit = 60L;

	private bool DelayShow;

	private ObservableCollection _GoodCollection;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollectionAward;

	private enum PassState
	{
		Success,
		Fail
	}
}
