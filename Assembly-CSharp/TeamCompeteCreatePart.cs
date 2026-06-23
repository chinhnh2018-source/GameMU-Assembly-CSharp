using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteCreatePart : UserControl, IMUEventManagerHandler
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.Lbl.Text = Global.GetLang("请输入战队名称:");
		this.LblJinBiDes.Text = Global.GetLang("需要         钻石");
		this.LblLevelDes.Text = Global.GetLang("确认自己的等级在                    以上");
		this.LblNameDes.Text = Global.GetLang("输入一个喜欢的战队名称");
		this.LblCreateDes.Text = Global.GetLang("点击创建战队按钮");
		this.LblDeccribe.Text = Global.GetLang("自动接受加入战队邀请");
		this.BtnCreateTeam.Label.text = Global.GetLang("创建战队");
		this.BtnOtherTeam.Label.text = Global.GetLang("其他战队");
		this.LblJinBiValue.Text = ConfigSystemParam.GetSystemParamIntByName("TeamNeedZuan").ToString();
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TeamLevelLimit", ',');
		this.LblLevelValue.Text = Global.GetString(new object[]
		{
			systemParamIntArrayByName[0],
			Global.GetLang("转"),
			systemParamIntArrayByName[1],
			Global.GetLang("级")
		});
		this.LblLevelValue.X = 205.0;
		this.mCfgNameLengthLimit = ConfigSystemParam.GetSystemParamIntArrayByName("TeamBattleNameRange", ',');
		if (this.mCfgNameLengthLimit.Length <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"SystemParam---TeamBattleNameRange---有误！"
			});
		}
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnCreateTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (string.IsNullOrEmpty(this.mInputName.Text))
			{
				Super.HintMainText(Global.GetLang("战队名不能为空"), 10, 3);
				return;
			}
			if (!this.RequestCreateTeamMsg(this.mInputName.Text))
			{
				return;
			}
			if (!TeamCompeteDataManager.IsLevelEnough)
			{
				Super.HintMainText(Global.GetLang("等级不够"), 10, 3);
				return;
			}
			if (!TeamCompeteDataManager.IsDiamondEnough)
			{
				Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
				return;
			}
			if (!TeamCompeteDataManager.IsEnoughLenth(this.mInputName.Text))
			{
				return;
			}
			GameInstance.Game.SendCreateTeamMsg(this.mInputName.Text, null);
		};
		this.BtnOtherTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RequestOtherTeamInfoMsg();
		};
		NGUITools.SetActive(this.SprtSelected.gameObject, this.IsAutoAcceptOther);
		UIEventListener.Get(this.SprtCheckBox.gameObject).onClick = delegate(GameObject s)
		{
			this.IsAutoAcceptOther = !this.IsAutoAcceptOther;
			NGUITools.SetActive(this.SprtSelected.gameObject, this.IsAutoAcceptOther);
		};
	}

	public bool RequestCreateTeamMsg(string name)
	{
		bool ret = true;
		if (0 < name.Length && name.Length < this.mCfgNameLengthLimit[this.min])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称不能少于{0}个字符，请重新输入！"), this.mCfgNameLengthLimit[this.min]), 10, 3);
			ret = false;
		}
		if (name.Length > this.mCfgNameLengthLimit[this.max])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称已超过{0}个字符，请重新输入！"), this.mCfgNameLengthLimit[this.max]), 10, 3);
			ret = false;
		}
		WordsFilterMgr.ExecWordsFilter(name, delegate(object content, ExecWordsFilterEventArgs result)
		{
			if (result.ret > 0)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
				{
					result.ret,
					result.msg
				}), 10, 3);
				ret = false;
			}
			if (result.is_dirty > 0)
			{
				Super.HintMainText(Global.GetLang("战队名称不能包含国家规定禁止的词汇!"), 10, 3);
				ret = false;
			}
		});
		return ret;
	}

	private void InitValue()
	{
	}

	public void OpenTeamCompeteCreateTeamPart(List<TianTi5v5ZhanDuiMiniData> data)
	{
		if (this.mTeamCompeteCreateTeamPartWind != null || this.mTeamCompeteCreateTeamPart != null)
		{
			this.CloseTeamCompeteCreateTeamPart();
		}
		this.mTeamCompeteCreateTeamPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteCreateTeamPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteCreateTeamPartWind.Modal = true;
		this.mTeamCompeteCreateTeamPartWind.IsShowModal = true;
		Super.InitChildWindow(this.mTeamCompeteCreateTeamPartWind, "mTeamCompeteCreateTeamPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteCreateTeamPartWind);
		this.mTeamCompeteCreateTeamPart = U3DUtils.NEW<TeamCompeteCreateTeamPart>();
		this.mTeamCompeteCreateTeamPart.LoadItems(data);
		this.mTeamCompeteCreateTeamPartWind.Body.Add(this.mTeamCompeteCreateTeamPart);
		this.mTeamCompeteCreateTeamPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteCreateTeamPart();
		};
	}

	private void CloseTeamCompeteCreateTeamPart()
	{
		if (null != this.mTeamCompeteCreateTeamPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteCreateTeamPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteCreateTeamPartWind, true);
			this.mTeamCompeteCreateTeamPartWind = null;
		}
		if (null != this.mTeamCompeteCreateTeamPart)
		{
			this.mTeamCompeteCreateTeamPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteCreateTeamPart.gameObject);
			this.mTeamCompeteCreateTeamPart = null;
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GETZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondOtherTeamInfo));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CREATE_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondCreateTeam));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CREATE_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondCreateTeam));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_GETZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondOtherTeamInfo));
	}

	public void RequestOtherTeamInfoMsg()
	{
		GameInstance.Game.RequestOtherTeamInfoMsg();
	}

	public void RespondOtherTeamInfo(MUSocketConnectEventArgs e)
	{
		List<TianTi5v5ZhanDuiMiniData> list = DataHelper.BytesToObject<List<TianTi5v5ZhanDuiMiniData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("暂无战队"), 10, 3);
			return;
		}
		this.CloseUI();
		this.OpenTeamCompeteCreateTeamPart(list);
	}

	public void RespondCreateTeam(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num >= 0)
		{
			this.CloseUI();
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenTeamCompeteMainPart();
			}
			return;
		}
		if (num == -4027)
		{
			Super.HintMainText(Global.GetLang("创建战队失败，名字包含特殊字符，请重新输入！"), 10, 3);
			return;
		}
		if (num == -4023)
		{
			Super.HintMainText(Global.GetLang("创建战队名称已存在，请输入其他名称重新创建"), 10, 3);
			return;
		}
		Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblJinBiDes;

	public TextBlock LblJinBiValue;

	public TextBlock LblLevelDes;

	public TextBlock LblLevelValue;

	public TextBlock LblNameDes;

	public TextBlock LblWhat;

	public TextBlock LblCreateDes;

	public TextBlock LblDeccribe;

	public GButton BtnClose;

	public GButton BtnCreateTeam;

	public GButton BtnOtherTeam;

	public UISprite SprtSelected;

	public UISprite SprtCheckBox;

	public TextBlock Lbl;

	public TextBox mInputName;

	private bool IsAutoAcceptOther = true;

	private int[] mCfgNameLengthLimit;

	private int min;

	private int max = 1;

	protected GChildWindow mTeamCompeteCreateTeamPartWind;

	protected TeamCompeteCreateTeamPart mTeamCompeteCreateTeamPart;
}
