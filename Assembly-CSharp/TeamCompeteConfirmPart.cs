using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteConfirmPart : UserControl
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
		base.InitializeComponent();
		this.ItemCollection = this.mListBox.Items;
		this.mConfirmTime = (int)ConfigSystemParam.GetSystemParamIntByName("TeamConfirmTime");
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("竞技确认");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.IsSearchEnemy && Global.Data != null && Global.Data.roleData.ZhanDuiZhiWu == 1)
			{
				GameInstance.Game.SendCancelSearchEnemyMsg();
			}
			this.ConfirmCallBack(0);
		};
		TeamCompeteDataManager.AllAcceptCallBack = delegate(bool haveRefuse, bool notPrepare, bool haveOffline)
		{
			if (haveRefuse || haveOffline)
			{
				this.IsShowCloseBtn = true;
				base.StopAllCoroutines();
			}
			else if (!notPrepare)
			{
				if (!haveRefuse && !notPrepare)
				{
					this.IsSearchEnemy = true;
					if (Global.Data != null && Global.Data.roleData.ZhanDuiZhiWu == 1)
					{
						this.IsShowCloseBtn = true;
					}
					this.LblTitle.Label.text = Global.GetLang("匹配中…");
					base.StopAllCoroutines();
					this.CountDownTimer(true, 60);
				}
			}
		};
	}

	private void InitValue()
	{
		this.IsShowCloseBtn = false;
		this.RefreshData();
	}

	public void RefreshData()
	{
		if (TeamCompeteDataManager.TianTi5v5PiPeiStateData != null)
		{
			this.LoadItems(TeamCompeteDataManager.TianTi5v5PiPeiStateData);
		}
	}

	private void CountDownTimer(bool IsSearching = false, int countDownTime = 30)
	{
		base.StartCoroutine(this.CountDown(countDownTime, delegate
		{
			this.IsShowCloseBtn = true;
			if (IsSearching)
			{
				this.ConfirmCallBack(0);
				TeamCompeteDataManager.PopupTimeOutWindow();
			}
			else
			{
				Super.HintMainText(Global.GetLang("准备超时"), 10, 3);
			}
			MUDebug.LogError<string>(new string[]
			{
				"倒计时结束"
			});
			this.StopCoroutine("CountDown");
		}));
	}

	private IEnumerator CountDown(int time, Action callBack = null)
	{
		for (int i = time; i >= 0; i--)
		{
			this.mLblCountDown.text = i.ToString();
			yield return new WaitForSeconds(1f);
		}
		if (callBack != null)
		{
			callBack.Invoke();
		}
		yield break;
	}

	private bool IsShowCloseBtn
	{
		set
		{
			NGUITools.SetActive(this.BtnClose.gameObject, value);
		}
	}

	public void ConfirmCallBack(int result)
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_SIGNUP_STATE", new Action<MUSocketConnectEventArgs>(this.RespondState));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_SIGNUP_STATE", new Action<MUSocketConnectEventArgs>(this.RespondState));
	}

	public void RequestState()
	{
		GameInstance.Game.RequestConfirmBattleTeamInfo();
	}

	public void RespondState(MUSocketConnectEventArgs e)
	{
		TianTi5v5PiPeiState tianTi5v5PiPeiState = DataHelper.BytesToObject<TianTi5v5PiPeiState>(e.bytesData, 0, e.bytesData.Length);
		if (tianTi5v5PiPeiState == null)
		{
			return;
		}
		this.LoadItems(tianTi5v5PiPeiState);
	}

	private void LoadItems(TianTi5v5PiPeiState data)
	{
		List<TianTi5v5PiPeiRoleState> roleList = data.RoleList;
		if (roleList == null || roleList.Count <= 0)
		{
			return;
		}
		base.StopAllCoroutines();
		this.CountDownTimer(false, this.GetConfirmCountDown(data.EndTicks));
		if (this.ItemCollection.Count > 0)
		{
			int count = this.ItemCollection.Count;
			for (int i = 0; i < count; i++)
			{
				TeamCompeteConfirmItemPart component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteConfirmItemPart>();
				component.InitValue(roleList[i]);
			}
		}
		else
		{
			for (int j = 0; j < roleList.Count; j++)
			{
				TeamCompeteConfirmItemPart teamCompeteConfirmItemPart = U3DUtils.NEW<TeamCompeteConfirmItemPart>();
				NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteConfirmItemPart.gameObject);
				teamCompeteConfirmItemPart.InitValue(roleList[j]);
				this.ItemCollection.Add(teamCompeteConfirmItemPart);
			}
		}
		if (this.HaveRefuseState())
		{
			this.IsShowCloseBtn = true;
		}
	}

	private int GetConfirmCountDown(long ticks)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		double num4 = (double)(ticks - Global.GetCorrectDateTime().Ticks / 10000L) / 1000.0;
		return (int)(num4 % (double)num % (double)num2 % (double)num3);
	}

	private bool HaveRefuseState()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			TeamCompeteConfirmItemPart component = this.ItemCollection.GetAt(i).GetComponent<TeamCompeteConfirmItemPart>();
			if (component.HaveRefuse)
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.IsSearchEnemy = false;
		TeamCompeteDataManager.AllAcceptCallBack = null;
		base.StopAllCoroutines();
	}

	public Action<int> ResultCallBack;

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public GButton BtnClose;

	public UILabel mLblCountDown;

	public UISprite mSprCountDown;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private int mConfirmTime;

	private bool IsSearchEnemy;
}
