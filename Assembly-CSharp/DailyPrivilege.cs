using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DailyPrivilege : UserControl
{
	private void InitTextInPrefabs()
	{
		this.signBtn.Text = Global.GetLang("一键领取");
		this.checkAll.text = Global.GetLang("全选");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.checkBox)
		{
			this.checkBox.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SetCheckAllState();
			};
		}
		if (null != this.signBtn)
		{
			this.signBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SignSelectItem();
			};
		}
	}

	public void Init()
	{
		this.GetDailyPrivilegeInfo();
	}

	private void SignSelectItem()
	{
		if (!this.checkBox.isChecked && (null == this.dailyItemList.SelectedItem || this.dailyItemList.SelectedIndex < 0))
		{
			return;
		}
		int signAll = (!this.checkBox.isChecked) ? 0 : 1;
		int signID = 0;
		GameObject selectedItem = this.dailyItemList.SelectedItem;
		if (null != selectedItem)
		{
			DailyPrivilegeItem component = selectedItem.GetComponent<DailyPrivilegeItem>();
			if (null != component)
			{
				signID = component.dailyPrivilegeID;
			}
		}
		this.SignDailyPrivilege(signAll, signID);
	}

	private void SetCheckAllState()
	{
		if (null == this.checkBox)
		{
			return;
		}
		bool isChecked = this.checkBox.isChecked;
		this.SetHighlightItem(isChecked, !isChecked);
	}

	private void SetHighlightItem(bool selectAll = false, bool state_unselect = false)
	{
		ObservableCollection itemsSource = this.dailyItemList.ItemsSource;
		this.dailyItemList.SelectedIndex = -1;
		bool flag = false;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GameObject at = itemsSource.GetAt(i);
			if (!(null == at))
			{
				DailyPrivilegeItem component = at.GetComponent<DailyPrivilegeItem>();
				if (!(null == component))
				{
					component.highlight = false;
					if (selectAll)
					{
						component.highlight = !state_unselect;
					}
					else if (!flag && component.UnFinished())
					{
						flag = true;
						component.highlight = true;
						this.dailyItemList.SelectedIndex = i;
					}
				}
			}
		}
	}

	public void SetDailyPrivileges(int state, string dailyInfo)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"SetDailyPrivileges, state: ",
				state,
				", dailyInfo: ",
				dailyInfo
			})
		});
		string msg = string.Empty;
		switch (state + 1)
		{
		case 0:
			msg = Global.GetLang("获取专享信息失败");
			break;
		default:
			if (state == -11)
			{
				msg = Global.GetLang("需要完成5转1级的【坎特鲁足迹】才可开启每日专享");
			}
			break;
		case 2:
			this.SetDailyPrivileges(dailyInfo);
			return;
		}
		if (state != 1)
		{
			Super.HintMainText(msg, 10, 3);
		}
	}

	public void SetDailyPrivilegeListState(int state, string signInfo)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"SetDailyPrivilegeListState, state: ",
				state,
				", signInfo: ",
				signInfo
			})
		});
		string msg = string.Empty;
		switch (state + 11)
		{
		case 0:
			msg = Global.GetLang("需要完成5转1级的【坎特鲁足迹】才可开启每日专享");
			break;
		case 3:
		case 4:
			msg = Global.GetLang("领取失败");
			break;
		case 5:
			msg = Global.GetLang("当前地图无法领取");
			break;
		case 6:
			msg = Global.GetLang("奖励已全部领取");
			break;
		case 7:
			msg = Global.GetLang("奖励已领取");
			break;
		case 8:
			msg = Global.GetLang("没有该项奖励");
			break;
		case 9:
			msg = Global.GetLang("背包空间不足");
			break;
		case 10:
			msg = Global.GetLang("领取失败");
			break;
		case 12:
			this.SetDailyPrivilegeListState(signInfo);
			return;
		}
		if (state != 1)
		{
			Super.HintMainText(msg, 10, 3);
		}
	}

	private void SetDailyPrivileges(string dailyInfo)
	{
		if (string.IsNullOrEmpty(dailyInfo))
		{
			return;
		}
		string[] array = dailyInfo.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.dailyItemList.ItemsSource;
		itemsSource.Clear();
		this.dailyItemList.SelectedIndex = -1;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'*'
			});
			if (array2.Length == 2)
			{
				DailyPrivilegeItem dailyPrivilegeItem = U3DUtils.NEW<DailyPrivilegeItem>();
				dailyPrivilegeItem.dailyPrivilegeID = Global.SafeConvertToInt32(array2[0]);
				dailyPrivilegeItem.completeCount = Global.SafeConvertToInt32(array2[1]);
				itemsSource.AddNoUpdate(dailyPrivilegeItem);
				if (!flag && dailyPrivilegeItem.UnFinished())
				{
					flag = true;
					dailyPrivilegeItem.highlight = true;
					this.dailyItemList.SelectedIndex = itemsSource.Count - 1;
				}
			}
		}
		this.dailyItemList.SelectionChanged = delegate(object s, MouseEvent e)
		{
			if (this.checkBox.isChecked)
			{
				return;
			}
			ListBox listBox = (ListBox)s;
			if (null != listBox.LastSelectedItem)
			{
				DailyPrivilegeItem component = listBox.LastSelectedItem.GetComponent<DailyPrivilegeItem>();
				if (null != component)
				{
					component.highlight = false;
				}
			}
			if (null != listBox.SelectedItem)
			{
				DailyPrivilegeItem component2 = listBox.SelectedItem.GetComponent<DailyPrivilegeItem>();
				if (null != component2)
				{
					component2.highlight = true;
				}
			}
		};
	}

	private void SetDailyPrivilegeListState(string signInfo)
	{
		if (string.IsNullOrEmpty(signInfo))
		{
			return;
		}
		string[] array = signInfo.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.dailyItemList.ItemsSource;
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'*'
			});
			if (array2.Length == 2)
			{
				int num = Global.SafeConvertToInt32(array2[0]);
				int completeCount = Global.SafeConvertToInt32(array2[1]);
				for (int j = 0; j < itemsSource.Count; j++)
				{
					GameObject at = itemsSource.GetAt(j);
					if (!(null == at))
					{
						DailyPrivilegeItem component = at.GetComponent<DailyPrivilegeItem>();
						if (null != component && component.dailyPrivilegeID == num)
						{
							component.completeCount = completeCount;
							break;
						}
					}
				}
			}
		}
		this.SetHighlightItem(false, false);
		if (array.Length == this.dailyItemList.ItemsSource.Count)
		{
			this.checkBox.isChecked = false;
		}
	}

	private void GetDailyPrivilegeInfo()
	{
		GameInstance.Game.GetDailyPrivilegeInfo();
	}

	private void SignDailyPrivilege(int signAll, int signID)
	{
		GameInstance.Game.SignDailyPrivilege(signAll, signID);
	}

	public GButton signBtn;

	public UILabel checkAll;

	public ListBox dailyItemList;

	public GCheckBox checkBox;
}
