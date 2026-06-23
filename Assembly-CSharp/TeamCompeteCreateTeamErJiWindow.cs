using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class TeamCompeteCreateTeamErJiWindow : UserControl
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
		this.LblTitle.Text = Global.GetLang("创建战队");
		this.LblContent.Text = Global.GetLang("是否对该队员进行？");
		this.LblName.Text = Global.GetLang("战队名称");
		this.LblXuanYan.Text = Global.GetLang("战队宣言");
		this.LblCostDiamond.Text = Global.GetLang("New Label");
		this.BtnCancel.Label.text = Global.GetLang("取消");
		this.BtnConfirm.Label.text = Global.GetLang("创建");
	}

	private void InitEvent()
	{
		this.BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
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
			WordsFilterMgr.ExecWordsFilter(this.mInputXuanYan.Text, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 10, 3);
					return;
				}
				if (result.is_dirty > 0)
				{
					Super.HintMainText(Global.GetLang("战队宣言不能包含国家规定禁止的词汇!"), 10, 3);
					return;
				}
			});
			GameInstance.Game.SendCreateTeamMsg(this.mInputName.Text, this.mInputXuanYan.Text);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	private void InitValue()
	{
		this.LblCostDiamond.Text = Global.GetLang("创建费用：") + ConfigSystemParam.GetSystemParamIntByName("TeamNeedZuan") + Global.GetLang("钻石");
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
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CREATE_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondCreateTeam));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_CREATE_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondCreateTeam));
	}

	public void RespondCreateTeam(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(num, false, false)), 10, 3);
			return;
		}
		if (this.ClickHandler != null)
		{
			this.ClickHandler(null, null);
		}
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.OpenTeamCompeteMainPart();
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblContent;

	public TextBlock LblName;

	public TextBlock LblXuanYan;

	public TextBlock LblCostDiamond;

	public GButton BtnCancel;

	public GButton BtnConfirm;

	public GButton BtnClose;

	public TextBox mInputName;

	public TextBox mInputXuanYan;
}
