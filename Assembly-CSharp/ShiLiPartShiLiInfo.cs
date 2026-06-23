using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using UnityEngine;
using XMLCreater;

public class ShiLiPartShiLiInfo : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblChanChuTitle.text = Global.GetLang("势力产出");
		this.lblJiRi.text = Global.GetLang("【今日产出】");
		this.lblJinShuiJing.text = Global.GetLang("迷踪水晶:");
		this.lblJinBoss.text = Global.GetLang("势力BOSS:");
		this.lblZuoRi.text = Global.GetLang("【昨日产出】");
		this.lblZuoShuiJing.text = Global.GetLang("迷踪水晶:");
		this.lblZuoBoss.text = Global.GetLang("势力BOSS:");
		this.lblZuoBeiDuo.text = Global.GetLang("被夺资源:");
		this.lblZuoChouDi.text = Global.GetLang("仇敌:");
		this.lblZuoChouDiName.text = Global.GetLang("【织梦协会】");
		this.lblBossTitle.text = Global.GetLang("势力BOSS");
		this.lblBossInfo.text = Global.GetLang("昨日势力BOSS已被【自由同盟】击杀");
		this.lblBossChuXian.text = Global.GetLang("BOSS刷新时间 16:30");
		this.btnGo.Text = Global.GetLang("前往");
		this.lblJinBossNum.pivot = 3;
		this.lblJinBossNum.transform.localPosition = new Vector3(230f, -32f, -1f);
		this.lblZuoBossNum.pivot = 3;
		this.lblZuoBossNum.transform.localPosition = new Vector3(230f, -32f, -1f);
		this.lblZuoBeiDuoNum.pivot = 3;
		this.lblZuoBeiDuoNum.transform.localPosition = new Vector3(95f, -60f, -1f);
		this.lblZuoChouDi.pivot = 3;
		this.lblZuoChouDi.transform.localPosition = new Vector3(-38f, -90f, -1f);
		this.lblZuoChouDiName.pivot = 3;
		this.lblZuoChouDiName.transform.localPosition = new Vector3(32f, -90f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			ShiLiData.OpenHelpWindow(ShiLiHelpType.HelpCompIntroMap);
		};
		this.btnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.GoToBoss();
		};
		this.InitInfo();
	}

	public void InitInfo()
	{
		this.lblBossChuXian.text = Global.GetLang("BOSS刷新时间") + ShiLiData.GetBossAppearTime();
		KFCompData kfCompData = ShiLiData.GetSelfCompData().kfCompData;
		this.lblJinShuiJingNum.text = kfCompData.Crystal.ToString();
		this.lblJinBossNum.text = kfCompData.Boss.ToString();
		this.lblZuoShuiJingNum.text = kfCompData.YestdCrystal.ToString();
		this.lblZuoBossNum.text = kfCompData.YestdBoss.ToString();
		int id = 1;
		int seftPlunderRes = this.GetSeftPlunderRes(ShiLiData.GetSelfCompData().kfCompData, ref id);
		this.lblZuoBeiDuoNum.text = seftPlunderRes.ToString();
		MUComp mucompById = ShiLiData.GetMUCompById(id);
		if (mucompById != null)
		{
			this.lblZuoChouDiName.text = Global.GetLang("【") + mucompById.CompName + Global.GetLang("】");
		}
		else
		{
			this.lblZuoChouDiName.text = Global.GetLang("【") + Global.GetLang("暂无敌对") + Global.GetLang("】");
		}
		MUComp mucompById2 = ShiLiData.GetMUCompById(kfCompData.YestdBossKillCompType);
		if (mucompById2 == null)
		{
			this.lblBossInfo.text = Global.GetLang("昨日势力BOSS未被击杀");
		}
		else
		{
			this.lblBossInfo.text = string.Format(Global.GetLang("昨日势力BOSS已被【{0}】击杀"), mucompById2.CompName);
		}
		if (ShiLiData.GetSelfCompData().BoomValueList.Count != 3)
		{
			MUDebug.LogError<string>(new string[]
			{
				"繁荣度数目出错"
			});
			return;
		}
		int num = -1;
		for (int i = 0; i < ShiLiData.GetSelfCompData().BoomValueList.Count; i++)
		{
			if (ShiLiData.GetSelfCompData().BoomValueList[i] > num)
			{
				num = ShiLiData.GetSelfCompData().BoomValueList[i];
			}
		}
		this.m_lstPosition.Clear();
		this.m_lstPosition.Add(this.jiaoTuanItem.transform.localPosition);
		this.m_lstPosition.Add(this.tongMengItem.transform.localPosition);
		this.m_lstPosition.Add(this.xieHuiItem.transform.localPosition);
		this.jiaoTuanItem.Init(ShiLiType.ShenShengJiaoTuan, ShiLiData.GetSelfCompData().BoomValueList[0], num);
		this.tongMengItem.Init(ShiLiType.ZiYouTongMeng, ShiLiData.GetSelfCompData().BoomValueList[1], num);
		this.xieHuiItem.Init(ShiLiType.ZhiMengXieHui, ShiLiData.GetSelfCompData().BoomValueList[2], num);
		this.RePosition();
	}

	private int GetSeftPlunderRes(KFCompData compData, ref int enemyType)
	{
		if (compData.PlunderResList == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"PlunderResList is null"
			});
			enemyType = 1;
			return 0;
		}
		if (compData.PlunderResList.Count != 3)
		{
			MUDebug.LogError<string>(new string[]
			{
				"PlunderResList count error"
			});
			enemyType = 1;
			return 0;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < compData.PlunderResList.Count; i++)
		{
			int num3 = i + 1;
			if (compData.CompType != num3)
			{
				num += compData.PlunderResList[i];
				if (compData.PlunderResList[i] > num2)
				{
					enemyType = num3;
					num2 = compData.PlunderResList[i];
				}
			}
		}
		if (num2 <= 0)
		{
			enemyType = -1;
		}
		return num;
	}

	private void RePosition()
	{
		List<ShiLiPartShiLiInfoItem> list = new List<ShiLiPartShiLiInfoItem>();
		ShiLiType compType = (ShiLiType)GameInstance.Game.CurrentSession.roleData.CompType;
		if (compType == ShiLiType.ZiYouTongMeng)
		{
			list.Add(this.jiaoTuanItem);
			list.Add(this.tongMengItem);
			list.Add(this.xieHuiItem);
		}
		else if (compType == ShiLiType.ShenShengJiaoTuan)
		{
			list.Add(this.tongMengItem);
			list.Add(this.jiaoTuanItem);
			list.Add(this.xieHuiItem);
		}
		else if (compType == ShiLiType.ZhiMengXieHui)
		{
			list.Add(this.tongMengItem);
			list.Add(this.xieHuiItem);
			list.Add(this.jiaoTuanItem);
		}
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].transform.localPosition = this.m_lstPosition[i];
			}
		}
	}

	private void GoToBoss()
	{
		MUComp mucompById = ShiLiData.GetMUCompById(GameInstance.Game.CurrentSession.roleData.CompType);
		if (mucompById != null)
		{
			int monstersID = mucompById.MonstersID;
			int mapCode = mucompById.MapCode;
			Point monsterPointByID = Global.GetMonsterPointByID(mapCode, monstersID);
			if (monsterPointByID.X == -1 || monsterPointByID.Y == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("路径信息格式错误 ,无法自动寻路"), new object[0]), 0, -1, -1, 0);
				return;
			}
			ShiLiData.CloseShiLiWindow();
			if (Global.IsInShiLiZhengBaMap())
			{
				Global.Data.GameScene.AutoFindRoad(mapCode, monsterPointByID, 0, ExtActionTypes.EXTACTION_NONE);
			}
			else
			{
				GameInstance.Game.EnterCompMap(mapCode, monsterPointByID.X, monsterPointByID.Y, 0, monstersID);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到势力配置"
			});
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblChanChuTitle;

	public UILabel lblJiRi;

	public UILabel lblJinShuiJing;

	public UILabel lblJinShuiJingNum;

	public UILabel lblJinBoss;

	public UILabel lblJinBossNum;

	public UILabel lblZuoRi;

	public UILabel lblZuoShuiJing;

	public UILabel lblZuoShuiJingNum;

	public UILabel lblZuoBoss;

	public UILabel lblZuoBossNum;

	public UILabel lblZuoBeiDuo;

	public UILabel lblZuoBeiDuoNum;

	public UILabel lblZuoChouDi;

	public UILabel lblZuoChouDiName;

	public UILabel lblBossTitle;

	public UILabel lblBossInfo;

	public UILabel lblBossChuXian;

	public GButton btnClose;

	public GButton btnHelp;

	public GButton btnGo;

	public ShiLiPartShiLiInfoItem jiaoTuanItem;

	public ShiLiPartShiLiInfoItem tongMengItem;

	public ShiLiPartShiLiInfoItem xieHuiItem;

	private List<Vector3> m_lstPosition = new List<Vector3>();
}
