using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUQiRiGoalPart : UserControl
{
	private void InitTextInPrefabs()
	{
		int i = 0;
		int num = this.dayLabel.Length;
		while (i < num)
		{
			this.dayLabel[i].text = string.Format(Global.GetLang("第{0}天"), i + 1);
			i++;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		Global.LoadSevenDayGoalxml();
		this.day = Global.GetQiRiKuanghuanDaysNum();
		this.ShowPage(this.day);
		this.BtnOneDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(1);
		};
		this.BtnTwoDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(2);
		};
		this.BtnThreeDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(3);
		};
		this.BtnFourDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(4);
		};
		this.BtnFiveDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(5);
		};
		this.BtnSixDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(6);
		};
		this.BtnSevenDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowPage(7);
		};
		ActivityTipManager.RegActivityTipItem(17005, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnOneDayTip)
			{
				this.BtnOneDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17006, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnTwoDayTip)
			{
				this.BtnTwoDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17007, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnThreeDayTip)
			{
				this.BtnThreeDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17008, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnFourDayTip)
			{
				this.BtnFourDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17009, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnFiveDayTip)
			{
				this.BtnFiveDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17010, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnSixDayTip)
			{
				this.BtnSixDayTip.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17011, delegate(int s, ActivityTipItem e)
		{
			if (null != this.BtnSevenDayTip)
			{
				this.BtnSevenDayTip.gameObject.SetActive(e.IsActive);
			}
		});
	}

	private void ShowPage(int index)
	{
		if (index > this.day)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未达到开启天数!"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.isCurDay == index)
		{
			return;
		}
		this.SprPnlContent.Clear();
		GameInstance.Game.DemandQiRiKuangHuanInfo(3);
		this.isCurDay = index;
		switch (index)
		{
		case 1:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 2:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 3:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 4:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 5:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 6:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		case 7:
			this.SetBtnState(index);
			this.OpenDayPage(index);
			break;
		}
	}

	private void SetBtnState(int select)
	{
		this.SetBtnStat(this.BtnOneDay, select == 1);
		this.SetBtnStat(this.BtnTwoDay, select == 2);
		this.SetBtnStat(this.BtnThreeDay, select == 3);
		this.SetBtnStat(this.BtnFourDay, select == 4);
		this.SetBtnStat(this.BtnFiveDay, select == 5);
		this.SetBtnStat(this.BtnSixDay, select == 6);
		this.SetBtnStat(this.BtnSevenDay, select == 7);
	}

	private void OpenDayPage(int index)
	{
		this.muQiRiKuangHuanPartGoal = null;
		this.muQiRiKuangHuanPartGoal = U3DUtils.NEW<MUQiRiKuangHuanPartGoal>();
		this.muQiRiKuangHuanPartGoal.Day = index;
		U3DUtils.AddChild(this.PnlContent.gameObject, this.muQiRiKuangHuanPartGoal.gameObject, true);
	}

	public void InitState(SevenDayActQueryData data)
	{
		this.muQiRiKuangHuanPartGoal.SetBtnState(data);
	}

	public void SetBtnStateInfo(int ID)
	{
		this.muQiRiKuangHuanPartGoal.SetBtnState(ID);
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	public GameObject PnlContent;

	public SpriteSL SprPnlContent;

	public GButton BtnOneDay;

	public GButton BtnTwoDay;

	public GButton BtnThreeDay;

	public GButton BtnFourDay;

	public GButton BtnFiveDay;

	public GButton BtnSixDay;

	public GButton BtnSevenDay;

	public GameObject BtnOneDayTip;

	public GameObject BtnTwoDayTip;

	public GameObject BtnThreeDayTip;

	public GameObject BtnFourDayTip;

	public GameObject BtnFiveDayTip;

	public GameObject BtnSixDayTip;

	public GameObject BtnSevenDayTip;

	public UILabel[] dayLabel;

	public DPSelectedItemEventHandler DPSelectedItem;

	public MUQiRiKuangHuanPartGoal muQiRiKuangHuanPartGoal;

	private int day;

	private int isCurDay;
}
