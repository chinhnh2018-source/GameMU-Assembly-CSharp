using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class RiChangGuZhanChangPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Enter.Text = Global.GetLang("立即进入");
		this._Content.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战场中配合使用【经验印章】可提高经验收益！")
		});
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(4041, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		Global.AddGoodsIcon(dummyGoodsDataMu, this._GameGoods, false, GoodsOwnerTypes.SysGifts);
		this._Enter.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnEnter);
		GameInstance.Game.GetJieriFanbeiInfo();
	}

	public void InitFanbei()
	{
		if (Global.isFanbei(4))
		{
			this.LeftSecs = this.guMuTime;
			this._Time.Text = string.Format(Global.GetLang("今日剩余时间:{0}"), UIHelper.FormatSecs(this.LeftSecs, "-"));
			FanbeiPrefab fanbeiPrefab = U3DUtils.NEW<FanbeiPrefab>();
			fanbeiPrefab.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/TimeDouble.png";
			this.obj.Add(fanbeiPrefab);
			this.obj.gameObject.SetActive(true);
		}
	}

	public void InitPartData(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/HuoDongTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		if (category == ActivityCategorys.GuZhanChang)
		{
			List<int> guMuMapList = Global.GetGuMuMapList();
			if (guMuMapList.Count > 0)
			{
				this.guMuTime = Global.GetMapLimitTime(guMuMapList[0]);
				this.LeftSecs = this.guMuTime;
				this._Time.Text = string.Format(Global.GetLang("今日剩余时间:{0}"), UIHelper.FormatSecs(this.LeftSecs, "-"));
			}
			XElement xelement = Global.GetXElement(gameResXml, "HuoDong", "ID", 7.ToString());
			if (xelement != null)
			{
				this._Preview.URL = Super.GetTaskImageString2(Global.GetXElementAttributeStr(xelement, "Preview"));
			}
		}
	}

	public void OnEnter(object sender, MouseEvent e)
	{
		if (this.LeftSecs <= 0L)
		{
			Super.HintMainText(Global.GetLang("今日已无进入古战场时间"), 10, 3);
			return;
		}
		if (Global.Data.CurrentCopyTeamData != null)
		{
			Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
			{
				if (e2.ID == 0)
				{
					GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
					GameInstance.Game.SpriteRunNPCScript(-1, 800);
				}
			}, -1);
		}
		else
		{
			GameInstance.Game.SpriteRunNPCScript(-1, 800);
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

	public ShowNetImage _Bak;

	public GButton _Enter;

	public TextBlock _Time;

	public ShowNetImage _Preview;

	public UILabel _Content;

	public GameObject _GameGoods;

	public SpriteSL obj;

	private long LeftSecs;

	private long guMuTime;
}
