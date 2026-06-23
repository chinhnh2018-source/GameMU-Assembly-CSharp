using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LangHunLingYuResultPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnFinish.Text = Global.GetLang("离开");
	}

	protected override void InitializeComponent()
	{
		NGUITools.SetActive(this.mLblWinnerNameInGuanZhan.gameObject, false);
		this.InitTextInPrefabs();
		this.btnFinish.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		};
		this.countDown = Global.GetLuolanchengzhanClearSecs();
		base.InvokeRepeating("TimeProc", 0f, 1f);
	}

	public void Init(LangHunLingYuAwardsData item)
	{
		if (item == null)
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
			return;
		}
		if (item.Success == 1)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		if (Global.CanGuanZhan())
		{
			NGUITools.SetActive(this.mLblWinnerNameInGuanZhan.gameObject, true);
			this.mLblWinnerNameInGuanZhan.Text = item.successBhName + Global.GetLang("战盟成功占领圣域");
		}
		else
		{
			NGUITools.SetActive(this.mLblWinnerNameInGuanZhan.gameObject, false);
			this.AddAwardGoods(this.List.ItemsSource, item);
		}
	}

	protected void AddAwardGoods(ObservableCollection ItemCollection, LangHunLingYuAwardsData item)
	{
		List<GoodsData> viewTaskInfoGoodsDataList = new List<GoodsData>();
		UIHelper.ParseAwardsItemList(item.AwardsItemDataList, ref viewTaskInfoGoodsDataList);
		Super.GData.ViewTaskInfoGoodsDataList = viewTaskInfoGoodsDataList;
		if (Super.GData.ViewTaskInfoGoodsDataList != null && Super.GData.ViewTaskInfoGoodsDataList.Count > 0)
		{
			for (int i = 0; i < Super.GData.ViewTaskInfoGoodsDataList.Count; i++)
			{
				UIHelper.AddGoodsIcon2(ItemCollection, Super.GData.ViewTaskInfoGoodsDataList[i], null, false, "bagGrid4_bak");
			}
			ItemCollection.DelayUpdate();
		}
	}

	protected void TimeProc()
	{
		if (this.countDown < 0)
		{
			this.lblTime.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
		this.lblTime.text = StringUtil.substitute("{0}" + Global.GetLang("秒后关闭"), new object[]
		{
			this.countDown
		});
		this.countDown--;
	}

	public UILabel lblTime;

	public GButton btnFinish;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int countDown = 10;

	public Animator AnimWin;

	public Animator AnimLose;

	public ListBox List;

	public TextBlock mLblWinnerNameInGuanZhan;
}
