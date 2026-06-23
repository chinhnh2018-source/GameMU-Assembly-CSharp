using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HongBaoDetailsPart : UserControl
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

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mLblSum.Text = Global.GetLang("总额：");
		this.m_BtnOpenFaHongBaoUI.Text = Global.GetLang("发红包");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.m_BtnOpenFaHongBaoUI.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteQueryBangHuiDetail(Global.Data.roleData.Faction);
			PlayZone.GlobalPlayZone.OpenZhanMengHongBaoWindow(true);
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mList.ItemsSource;
	}

	public void NotifyInitDataByServerData(HongBaoDeatilsData data)
	{
		if (data == null)
		{
			return;
		}
		this.hongBaoStatus = data.hongBaoStatus;
		this.hongBaoType = data.type;
		int num = this.hongBaoType;
		if (num != 0)
		{
			if (num == 1)
			{
				string text = string.Format("{0}{1}", data.sender, Global.GetLang("的手气红包"));
				this.mLblName.Text = text;
			}
		}
		else
		{
			string text2 = string.Format("{0}{1}", data.sender, Global.GetLang("的普通红包"));
			this.mLblName.Text = text2;
		}
		bool flag = false;
		this.sendTime = data.sendTime.ToString();
		NGUITools.SetActive(this.mTitleObj, false);
		NGUITools.SetActive(this.mLblStatus.gameObject, false);
		if (data.leftCount == 0 && this.hongBaoType == 1)
		{
			flag = true;
		}
		switch (this.hongBaoStatus)
		{
		case 0:
			NGUITools.SetActive(this.mLblStatus.gameObject, true);
			this.mLblStatus.Text = this.sendTime;
			break;
		case 1:
			NGUITools.SetActive(this.mTitleObj, true);
			this.mLblDiamondNum.Text = data.diamondNum.ToString();
			break;
		case 2:
			NGUITools.SetActive(this.mLblStatus.gameObject, true);
			if (Global.Data.mIsChanKanHongBao)
			{
				this.mLblStatus.Text = data.sendTime.ToString("yyyy-MM-dd");
			}
			else
			{
				this.mLblStatus.Text = Global.GetLang("红包已过期！");
			}
			break;
		case 3:
			NGUITools.SetActive(this.mLblStatus.gameObject, true);
			if (Global.Data.mIsChanKanHongBao)
			{
				this.mLblStatus.Text = data.sendTime.ToString("yyyy-MM-dd");
			}
			else
			{
				this.mLblStatus.Text = Global.GetLang("红包已被抢光！");
			}
			break;
		}
		this.mLblSumDaimondNum.Text = data.sumDiamondNum.ToString();
		string text3 = string.Format("{0}{1}{2}", Global.GetLang("剩余"), data.leftCount, Global.GetLang("个"));
		this.mLblSumDaimondEnd.Text = text3;
		if (data.infos == null || data.infos.Count <= 0)
		{
			return;
		}
		this.ItemCollection.Clear();
		List<SingleHongBaoRankInfo> infos = data.infos;
		if (flag)
		{
			infos.Sort(delegate(SingleHongBaoRankInfo d1, SingleHongBaoRankInfo d2)
			{
				if (d1.diamondNum > d2.diamondNum)
				{
					return -1;
				}
				if (d1.diamondNum != d2.diamondNum)
				{
					return 1;
				}
				if (d1.recvTime.CompareTo(d2.recvTime) == -1)
				{
					return -1;
				}
				return 0;
			});
			infos[0].zuiJia = 1;
			infos.Sort(delegate(SingleHongBaoRankInfo d1, SingleHongBaoRankInfo d2)
			{
				if (d1.recvTime.CompareTo(d2.recvTime) == 1)
				{
					return -1;
				}
				if (d1.recvTime.CompareTo(d2.recvTime) == 0)
				{
					return 0;
				}
				return 1;
			});
		}
		for (int i = 0; i < infos.Count; i++)
		{
			HongBaoDetailsItem hongBaoDetailsItem = U3DUtils.NEW<HongBaoDetailsItem>();
			hongBaoDetailsItem.NotifyInitDataByServerData(infos[i]);
			UIPanel component = hongBaoDetailsItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.ItemCollection.AddNoUpdate(hongBaoDetailsItem);
		}
	}

	protected override void OnDestroy()
	{
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mLblName = null;
		this.mLblStatus = null;
		this.mTitleObj = null;
		this.mLblDiamondNum = null;
		this.mLblSum = null;
		this.mLblSumDaimondNum = null;
		this.mLblSumDaimondEnd = null;
		this.mList = null;
		this.m_BtnOpenFaHongBaoUI = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblName;

	public TextBlock mLblStatus;

	public GameObject mTitleObj;

	public TextBlock mLblDiamondNum;

	public TextBlock mLblSum;

	public TextBlock mLblSumDaimondNum;

	public TextBlock mLblSumDaimondEnd;

	public ListBox mList;

	public GButton m_BtnOpenFaHongBaoUI;

	private ObservableCollection _ItemCollection;

	private int hongBaoStatus;

	private int hongBaoType;

	private string sendTime;
}
