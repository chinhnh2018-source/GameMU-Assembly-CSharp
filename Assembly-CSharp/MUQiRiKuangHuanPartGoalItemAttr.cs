using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class MUQiRiKuangHuanPartGoalItemAttr : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this.m_ListGiftItem)
		{
			this.m_ListGiftItemObC = this.m_ListGiftItem.ItemsSource;
		}
	}

	public void ResetPanelGiftsPos(int x, int y, int a, int b, int m)
	{
		this.PanelGifts.transform.localPosition = Vector3.zero;
		this.PanelGifts.clipRange = new Vector4((float)x, (float)y, (float)a, (float)b);
		this.ScorllBak.transform.localScale = new Vector3(20f, (float)m, 1f);
	}

	public void GetGoalTypeInfo(int goaltype)
	{
		if (Global.dic_SevenDayGoal.Keys.Count <= 0)
		{
			return;
		}
		int qiRiKuanghuanDaysNum = Global.GetQiRiKuanghuanDaysNum();
		foreach (SevendayGoal sevendayGoal in Global.dic_SevenDayGoal.Values)
		{
			if (goaltype == sevendayGoal.GoalType)
			{
				MUQiRiGoalItemList muqiRiGoalItemList = U3DUtils.NEW<MUQiRiGoalItemList>();
				muqiRiGoalItemList.ID = sevendayGoal.ID;
				muqiRiGoalItemList.ShowNum = sevendayGoal.ShowNum;
				muqiRiGoalItemList.TypeGoal = sevendayGoal.TypeGoal;
				muqiRiGoalItemList.describe = sevendayGoal.Describe;
				string award = sevendayGoal.Award;
				string[] array = sevendayGoal.Award.Split(new char[]
				{
					'|'
				});
				muqiRiGoalItemList.goodscount = array.Length;
				muqiRiGoalItemList.GoodsList = award;
				this.m_ListGiftItemObC.Add(muqiRiGoalItemList);
				this.CurrentItems.Add(muqiRiGoalItemList);
			}
		}
	}

	public void SetBtnState(SevenDayActQueryData sevendayactquerydata)
	{
		if (sevendayactquerydata == null)
		{
			return;
		}
		SevenDayItemData sevenDayItemData = null;
		int qiRiKuanghuanDaysNum = Global.GetQiRiKuanghuanDaysNum();
		foreach (MUQiRiGoalItemList muqiRiGoalItemList in this.CurrentItems)
		{
			if (sevendayactquerydata.ItemDict == null)
			{
				muqiRiGoalItemList.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
			else if (sevendayactquerydata.ItemDict.TryGetValue(muqiRiGoalItemList.ID, ref sevenDayItemData))
			{
				muqiRiGoalItemList.HadNum = sevenDayItemData.Params1;
				bool flag;
				bool flag2;
				this.GetGoalItemState(muqiRiGoalItemList.ID, sevenDayItemData, out flag, out flag2);
				if (flag)
				{
					muqiRiGoalItemList.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
				else
				{
					muqiRiGoalItemList.AwardGiftGainState = ((!flag2) ? JieriAwardGiftGainState.CanNotGain : JieriAwardGiftGainState.CanGain);
				}
			}
			else
			{
				muqiRiGoalItemList.HadNum = 0;
				muqiRiGoalItemList.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			}
		}
	}

	private void GetGoalItemState(int goalId, SevenDayItemData itemData, out bool hadGet, out bool canGet)
	{
		hadGet = false;
		canGet = false;
		if (itemData.AwardFlag == 1)
		{
			hadGet = true;
			canGet = false;
			return;
		}
		int num = this.GetFunctionTypeByGoalId(goalId);
		int num2;
		int num3;
		int num4;
		this.GetTypeGoal(goalId, out num2, out num3, out num4);
		if ((num >= 6 && num <= 7) || (num >= 9 && num <= 16))
		{
			num = 2;
		}
		switch (num)
		{
		case 1:
		case 39:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 > num2)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			if (itemData.Params1 == num2 && itemData.Params2 >= num3)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		case 2:
		case 3:
		case 4:
		case 22:
		case 23:
		case 24:
		case 26:
		case 28:
		case 29:
		case 31:
		case 32:
		case 34:
		case 35:
		case 37:
		case 38:
		case 40:
		case 43:
		case 44:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 >= num2)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		case 5:
		case 17:
		case 19:
		case 20:
		case 21:
		case 25:
		case 27:
		case 33:
		case 36:
		case 42:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 >= num3)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		case 8:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 > -1 && itemData.Params1 <= num2)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		case 18:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 >= num4)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		case 30:
		case 41:
			if (itemData.AwardFlag == 1)
			{
				hadGet = true;
				canGet = false;
				return;
			}
			if (itemData.Params1 == 1)
			{
				hadGet = false;
				canGet = true;
				return;
			}
			break;
		}
	}

	private void GetTypeGoal(int goalId, out int params1, out int params2, out int params3)
	{
		params1 = 0;
		params2 = 0;
		params3 = 0;
		string text = string.Empty;
		SevendayGoal sevendayGoal = null;
		if (Global.dic_SevenDayGoal.TryGetValue(goalId, ref sevendayGoal))
		{
			text = sevendayGoal.TypeGoal;
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array.Length == 1)
			{
				params1 = int.Parse(array[0]);
				return;
			}
			if (array.Length == 2)
			{
				params1 = int.Parse(array[0]);
				params2 = int.Parse(array[1]);
				return;
			}
			if (array.Length == 3)
			{
				params1 = int.Parse(array[0]);
				params2 = int.Parse(array[1]);
				params3 = int.Parse(array[2]);
			}
		}
	}

	private int GetFunctionTypeByGoalId(int goalId)
	{
		SevendayGoal sevendayGoal = null;
		if (Global.dic_SevenDayGoal.TryGetValue(goalId, ref sevendayGoal))
		{
			return sevendayGoal.FunctionType;
		}
		return 0;
	}

	public void setCompletedInfo(int ID)
	{
		foreach (MUQiRiGoalItemList muqiRiGoalItemList in this.CurrentItems)
		{
			if (ID == muqiRiGoalItemList.ID)
			{
				muqiRiGoalItemList.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListGiftItem = new ListBox();

	public UIPanel PanelGifts;

	public GameObject ScorllBak;

	private ObservableCollection m_ListGiftItemObC;

	private List<MUQiRiGoalItemList> CurrentItems = new List<MUQiRiGoalItemList>();
}
