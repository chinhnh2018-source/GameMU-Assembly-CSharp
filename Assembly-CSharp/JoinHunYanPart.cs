using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class JoinHunYanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Title1.text = Global.GetLang("宴会规模");
		this.Title2.text = Global.GetLang("举办时间");
		this.Title3.text = Global.GetLang("举办者");
		this.Title4.text = Global.GetLang("剩余次数");
		this.Title5.text = Global.GetLang("参与次数");
		this.Title1.textColor = Global.ParseStringColorToUint("#ffd460");
		this.Title2.textColor = Global.ParseStringColorToUint("#ffd460");
		this.Title3.textColor = Global.ParseStringColorToUint("#ffd460");
		this.Title4.textColor = Global.ParseStringColorToUint("#ffd460");
		this.Title5.textColor = Global.ParseStringColorToUint("#ffd460");
		this.Collection = this.ListBox.ItemsSource;
		this.ListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.OnSelectItem);
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.JoinTotalNum = Global.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("HunYanUseMaxNum", true));
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.GetHunYanListInfo(false);
	}

	public void SetHunYanList(Dictionary<int, MarryPartyData> datas)
	{
		this.haveJoinNum = 0;
		long num = Global.GetCorrectDateTime().Ticks / 10000L;
		List<HunYanItem> list = new List<HunYanItem>();
		List<HunYanItem> list2 = new List<HunYanItem>();
		foreach (MarryPartyData marryPartyData in datas.Values)
		{
			if (marryPartyData.StartTime <= num)
			{
				HunYanItem hunYanItem = U3DUtils.NEW<HunYanItem>();
				hunYanItem.SetData(marryPartyData);
				if (hunYanItem.CanUseNum > hunYanItem.HaveUseNum)
				{
					list.Add(hunYanItem);
				}
				else
				{
					list2.Add(hunYanItem);
				}
			}
		}
		list.AddRange(list2);
		for (int i = 0; i < list.Count; i++)
		{
			HunYanItem hunYanItem2 = list[i];
			this.Collection.AddNoUpdate(hunYanItem2);
			this.Collection.DelayUpdate();
			UIPanel component = hunYanItem2.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		list.Clear();
		list2.Clear();
		list = null;
		list2 = null;
		foreach (int num2 in Global.Data.HunYanJointTimes.Values)
		{
			this.haveJoinNum += num2;
		}
		this.haveJoinNum = Math.Min(this.haveJoinNum, this.JoinTotalNum);
		this.Title5.text = string.Format(Global.GetLang("参与次数（{0}/{1}）"), this.haveJoinNum, this.JoinTotalNum);
	}

	private void OnSelectItem(object sender, MouseEvent e)
	{
		if (this.ListBox.SelectedItem == null)
		{
			return;
		}
		HunYanItem component = this.ListBox.SelectedItem.GetComponent<HunYanItem>();
		if (this.haveJoinNum >= this.JoinTotalNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已酒足饭饱，吃不下任何食物了！"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (component.CanUseNum <= component.HaveUseNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此桌今日为您准备的食物已吃完，请去其它宴会看看吧！"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (component.HaveJoinTotalNum >= component.TotalNum)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("来晚了，已经被吃光了！"), new object[0]), 0, -1, -1, 0);
			return;
		}
		PlayZone.GlobalPlayZone.OpenHunYanPart(4, 1, component.Data, new DPSelectedItemEventHandler(this.CloseCallBack));
	}

	private void CloseCallBack(object target, DPSelectedItemEventArgs args)
	{
		this.Collection.Clear();
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.GetHunYanListInfo(false);
	}

	public TextBlock Title1;

	public TextBlock Title2;

	public TextBlock Title3;

	public TextBlock Title4;

	public TextBlock Title5;

	public UIDraggablePanel ScrollView;

	public GButton CloseBtn;

	public ListBox ListBox;

	private ObservableCollection Collection;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int JoinTotalNum;

	private int haveJoinNum;

	private class HunYanItemCmp : IComparer<HunYanItem>
	{
		public int Compare(HunYanItem x, HunYanItem y)
		{
			if ((x.CanUseNum == x.HaveUseNum && y.CanUseNum == y.HaveUseNum) || (x.CanUseNum != x.HaveUseNum && y.CanUseNum != y.HaveUseNum))
			{
				return y.Data.StartTime.CompareTo(x.Data.StartTime);
			}
			if (x.CanUseNum == x.HaveUseNum)
			{
				return 1;
			}
			if (y.CanUseNum == y.HaveUseNum)
			{
				return -1;
			}
			return 0;
		}
	}
}
