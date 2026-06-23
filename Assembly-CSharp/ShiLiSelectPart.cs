using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class ShiLiSelectPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.StartUITimer();
		this.InitNull();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		MUEventManager.RemoveEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
		this.StopTimer();
	}

	private void StartUITimer()
	{
		this.mUITimer = new DispatcherTimer("ShiLiSelectPart");
		this.mUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mUITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.mUITimer.Start();
	}

	private void UITimer_Tick(object sender, EventArgs args)
	{
		if (0 < this.mTimeActionList.Count)
		{
			for (int i = this.mTimeActionList.Count - 1; i >= 0; i--)
			{
				if (this.mTimeActionList[i] != null)
				{
					this.mTimeActionList[i].Invoke();
				}
				else
				{
					this.mTimeActionList.RemoveAt(i);
				}
			}
		}
	}

	private void StopTimer()
	{
		if (this.mUITimer != null)
		{
			this.mUITimer.Tick = null;
			this.mUITimer.Stop();
			this.mUITimer.Dispose();
			this.mUITimer = null;
		}
	}

	private void InitShiLiLiuDuo()
	{
		CompMineWarVO.TimePoint timePoint = new CompMineWarVO.TimePoint();
		MUForceCraft forceCraftByID = ShiLiData.GetForceCraftByID(1);
		string[] array = forceCraftByID.TimePoints.Split(new char[]
		{
			'-'
		});
		if (array != null && 0 < array.Length && !string.IsNullOrEmpty(array[0]))
		{
			string[] array2 = array[0].Split(new char[]
			{
				','
			});
			timePoint.Week = array2[0].SafeToInt32(0);
			string[] array3 = array2[1].Split(new char[]
			{
				':'
			});
			timePoint.SetTime1(array3[0].SafeToInt32(0), array3[1].SafeToInt32(0), array3[2].SafeToInt32(0));
		}
		if (array != null && 1 < array.Length && !string.IsNullOrEmpty(array[1]))
		{
			string[] array4 = array[1].Split(new char[]
			{
				':'
			});
			timePoint.SetTime2(array4[0].SafeToInt32(0), array4[1].SafeToInt32(0), array4[2].SafeToInt32(0));
		}
		ShiLiSelectItem item = null;
		if (this.itmes.ContainsKey(1))
		{
			item = this.itmes[1];
		}
		else
		{
			item = U3DUtils.NEW<ShiLiSelectItem>();
			item.name = "1";
			this.mObc.Insert(this.mObc.Count - 1, item);
			item.DraggablePanel = this._ViewDraggablePanel;
			this.mTimeActionList.Add(delegate()
			{
				item.RefreshTime();
			});
			this.itmes.Add(1, item);
		}
		item.SetActivityInf(1, (int)this.mCompBattleGameStates);
		item.SetActivityTime(timePoint);
	}

	private void InitShiLiMiDong()
	{
		CompMineWarVO.TimePoint activityTimeEX = IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineWarVOByID(1).ActivityTimeEX;
		ShiLiSelectItem item = null;
		if (this.itmes.ContainsKey(2))
		{
			item = this.itmes[2];
		}
		else
		{
			item = U3DUtils.NEW<ShiLiSelectItem>();
			this.mObc.Insert(this.mObc.Count - 1, item);
			item.name = "2";
			item.DraggablePanel = this._ViewDraggablePanel;
			this.mTimeActionList.Add(delegate()
			{
				item.RefreshTime();
			});
			this.itmes.Add(2, item);
		}
		item.SetActivityInf(2, (int)this.mCompMineGameStates);
		item.SetActivityTime(activityTimeEX);
	}

	private void InitNull()
	{
		ShiLiSelectItem shiLiSelectItem;
		if (this.itmes.ContainsKey(2))
		{
			shiLiSelectItem = this.itmes[2];
		}
		else
		{
			shiLiSelectItem = U3DUtils.NEW<ShiLiSelectItem>();
			this.mObc.Insert(this.mObc.Count - 1, shiLiSelectItem);
			shiLiSelectItem.name = "-1";
			shiLiSelectItem.DraggablePanel = this._ViewDraggablePanel;
			this.itmes.Add(-1, shiLiSelectItem);
		}
		shiLiSelectItem.InitNull(0);
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			MUEventManager.AddEventListener<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", new Action<CompBattleGameStates>(this.ServerGetBattleState));
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
				}
			};
			this.mObc = this._ViewListBox.ItemsSource;
			this._ViewListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectionChanged);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void ServerGetBattleState(CompBattleGameStates obj)
	{
		MUDebug.Log<string>(new string[]
		{
			"<color=yellow> CompBattleGameStates =  " + obj + "</color>"
		});
		this.mCompBattleGameStates = obj;
		this.InitShiLiLiuDuo();
	}

	private void SelectionChanged(object sender, MouseEvent e)
	{
		GameObject selectedItem = this._ViewListBox.SelectedItem;
		if (null != selectedItem && this.Hander != null)
		{
			this.Hander(null, new DPSelectedItemEventArgs
			{
				Type = 2,
				ID = selectedItem.name.SafeToInt32(0)
			});
		}
	}

	public void InitData()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.ShiLiGetCompBattleState();
		GameInstance.Game.SendShiLiGetCompMiDongStates();
	}

	public void NoticeGetStatesCallBask(int states)
	{
		this.mCompMineGameStates = (CompMineGameStates)states;
		MUDebug.Log<string>(new string[]
		{
			"<color=yellow> CompMineGameStates =  " + this.mCompBattleGameStates + "</color>"
		});
		this.InitShiLiMiDong();
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GButton[] _JianTou;

	[SerializeField]
	private UIDraggablePanel _ViewDraggablePanel;

	[SerializeField]
	private ListBox _ViewListBox;

	private DispatcherTimer mUITimer;

	private List<Action> mTimeActionList = new List<Action>();

	private ObservableCollection mObc;

	private CompMineGameStates mCompMineGameStates;

	private CompBattleGameStates mCompBattleGameStates;

	private Dictionary<int, ShiLiSelectItem> itmes = new Dictionary<int, ShiLiSelectItem>();
}
