using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public class RiChangShuiJingHuanJingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Enter.Text = Global.GetLang("立即进入");
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnter);
		this.TxtHint.Pivot = 0;
		this.TxtHint.X = -320.0;
		this.TxtHint.Y = -155.0;
		this.TxtHint.MaxWidth = 777.0;
	}

	public void InitPartData(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/HuoDongTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		if (category == ActivityCategorys.ShuiJingHuanJing)
		{
			XElement xelement = Global.GetXElement(gameResXml, "HuoDong", "ID", 8.ToString());
			if (xelement != null)
			{
				this._Preview.URL = Super.GetTaskImageString2(Global.GetXElementAttributeStr(xelement, "Preview"));
			}
			this.DoubleAwardTime = ConfigSystemParam.GetSystemParamStringArrayByName("MuKuangDoubleAward", '|');
			string text = string.Empty;
			string text2 = "1";
			for (int i = 0; i < this.DoubleAwardTime.Length; i++)
			{
				string[] array = this.DoubleAwardTime[i].Split(new char[]
				{
					','
				});
				if (array != null && array.Length == 3)
				{
					text2 = array[2];
					text += string.Format("{0}{1}-{2}", (i != 0) ? Global.GetLang("、") : string.Empty, array[0], array[1]);
				}
			}
			text += string.Format(Global.GetLang("采集可获得{0}倍收益！时间内会出现稀有的至尊水晶！"), text2);
			if (this.DoubleAwardTime != null && this.DoubleAwardTime.Length == 2)
			{
				this.MaxNum = ConfigSystemParam.GetSystemParamByName("MuKuangNum", true).SafeToInt32(0);
				GameInstance.Game.SpriteQueryCaijiLastNum(0);
				this.NotifyNum(1, 0);
			}
			this.TxtHint.Text = text;
		}
	}

	public void OnEnter(object sender, MouseEvent e)
	{
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShuiJingHuanJing))
		{
			return;
		}
		if (Global.Data.CurrentCopyTeamData != null)
		{
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
					GameInstance.Game.SpriteRunNPCScript(-1, 2000);
				}
			}, -1);
		}
		else
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 2000);
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
	}

	protected override void OnDestroy()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (null != playZone)
		{
			playZone._RiChangVIPHuoDongPart = null;
		}
		base.OnDestroy();
	}

	public void NotifyNum(int result, int num)
	{
		if (result < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("获取剩余次数时发生错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
			return;
		}
		if (result == 0)
		{
			this.Num = num;
			this.TxtNum.Text = string.Format(Global.GetLang("今日采集剩余次数：{0}"), this.Num);
		}
	}

	private const int MAPCODE_SHUIJINGHUANJING = 12000;

	public ShowNetImage _Bak;

	public GButton _Enter;

	public TextBlock TxtNum;

	public TextBlock TxtHint;

	public ShowNetImage _Preview;

	private string[] DoubleAwardTime;

	private int Num;

	private int MaxNum;
}
