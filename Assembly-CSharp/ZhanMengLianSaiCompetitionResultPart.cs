using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZhanMengLianSaiCompetitionResultPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.m_ConfirmBtn.Text = Global.GetLang("确定");
		if (this.ConstTexts != null && this.ConstTexts.Length == 2)
		{
			this.ConstTexts[0].Text = Global.GetLang("经验:");
			this.ConstTexts[1].Text = Global.GetLang("绑金:");
		}
		this.mGuanZhanLabel.Text = Global.GetLang("获胜战盟：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		this.m_ConfirmBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			MUDebug.Log<string>(new string[]
			{
				"越南调试用：离开场景。调用Leve()"
			});
			this.Leave();
		};
		base.InvokeRepeating("TimeProc", 0f, 1f);
		NGUITools.SetActive(this.mMvp.gameObject, false);
		this.ItemCollection = this.mListBox.ItemsSource;
	}

	public void Initdata(BangHuiMatchAwardsData data)
	{
		if (Global.CanGuanZhan())
		{
			this.mLianSaiResult.SetActive(false);
			this.mGuanZhanResult.SetActive(true);
			this.AnimWin.gameObject.SetActive(true);
			this.mGuanZhanLabelValue.Text = data.SuccessBHName;
		}
		else
		{
			this.mLianSaiResult.SetActive(true);
			this.mGuanZhanResult.SetActive(false);
			this.mBangJinValueLabel.Text = string.Empty + data.BindJinBi;
			this.mJingYanValueLabel.Text = string.Empty + data.Exp;
			if (data.Success == 0)
			{
				this.AnimLose.gameObject.SetActive(true);
			}
			else if (data.Success == 1)
			{
				this.AnimWin.gameObject.SetActive(true);
			}
			this.AddAward(data.AwardsItemDataList);
		}
		Global.PlaySoundAudio("Audio/UI/JingJiChangSuccess", false);
		if (string.IsNullOrEmpty(data.MvpRoleName))
		{
			NGUITools.SetActive(this.mName.gameObject, false);
			NGUITools.SetActive(this.mTouXiang.transform.parent.gameObject, false);
		}
		else
		{
			NGUITools.SetActive(this.mMvp.gameObject, true);
			this.mName.Text = data.MvpRoleName;
			string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(data.MvpOccupation),
				data.MvpRoleSex
			});
			this.mTouXiang.URL = url;
		}
	}

	private void AddAward(List<AwardsItemData> dataList)
	{
		if (dataList == null || dataList.Count <= 0)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		UIHelper.ParseAwardsItemList(dataList, ref list);
		for (int i = 0; i < list.Count; i++)
		{
			UIHelper.AddGoodsIcon(this.ItemCollection, list[i], null, false, "bagGrid4_bak");
		}
	}

	protected void TimeProc()
	{
		if (this.countDown < 0)
		{
			this.m_TimeLabel.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
			this.Leave();
		}
		this.countDown--;
	}

	private void Leave()
	{
		if (!Global.CanGuanZhan())
		{
			MUDebug.Log<string>(new string[]
			{
				"越南调试用：无权限观战调用SendZhanMengLianSaiGetAward()"
			});
			GameInstance.Game.SendZhanMengLianSaiGetAward();
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
	}

	public override void Destroy()
	{
		base.CancelInvoke("TimeProc");
		base.Destroy();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_ConfirmBtn;

	public Animator AnimWin;

	public Animator AnimLose;

	public TextBlock mJingYanValueLabel;

	public TextBlock mBangJinValueLabel;

	public TextBlock[] ConstTexts;

	public TextBlock mName;

	public ShowNetImage mTouXiang;

	public TextBlock m_TimeLabel;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private int countDown = 10;

	public UISprite mMvp;

	public GameObject mLianSaiResult;

	public GameObject mGuanZhanResult;

	public TextBlock mGuanZhanLabel;

	public TextBlock mGuanZhanLabelValue;

	public Transform bak;
}
