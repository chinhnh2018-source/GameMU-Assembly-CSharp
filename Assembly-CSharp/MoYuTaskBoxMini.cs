using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class MoYuTaskBoxMini : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblLeftWord.text = Global.GetLang("剩余:");
		this.btnDuiWu.Label.text = Global.GetLang("队伍");
		this.btnBoss.Label.text = Global.GetLang("头领伤害");
		this.btnDuiWu.Label.lineWidth = 0;
		this.btnBoss.Label.lineWidth = 0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.IsInDaTaoSha();
		this.m_bossId = this.GetBossId();
		this.btnDuiWu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(MoYuTaskBoxSelectType.ZhanDuiSelect);
		};
		this.btnBoss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(MoYuTaskBoxSelectType.BossSelect);
		};
		this.OnSelectType(MoYuTaskBoxSelectType.ZhanDuiSelect);
		this.SetContentShow(true);
		UIEventListener.Get(this.goSwitch).onClick = new UIEventListener.VoidDelegate(this.OnSwicth);
	}

	private void OnSwicth(GameObject go)
	{
		this.goContent.SetActive(!this.m_beShow);
		this.SetContentShow(!this.m_beShow);
	}

	private void SetContentShow(bool beShow)
	{
		this.m_beShow = beShow;
		if (beShow)
		{
			this.sprSwitch.spriteName = "Taskarrow1";
		}
		else
		{
			this.sprSwitch.spriteName = "Taskarrow_02";
		}
	}

	private void ShowAnimation(bool beShow)
	{
		this.m_beCanClick = false;
		if (beShow)
		{
			TweenPosition.Begin(this.goContent, 0.3f, new Vector3(0f, 55f, -0.2f), new Vector3(this.m_offX, 55f, -0.2f));
		}
		else
		{
			TweenPosition.Begin(this.goContent, 0.3f, new Vector3(this.m_offX, 55f, -0.2f), new Vector3(0f, 55f, -0.2f));
		}
		base.StartCoroutine<bool>(this.DealDoTime());
	}

	private IEnumerator DealDoTime()
	{
		yield return new WaitForSeconds(0.3f);
		this.m_beCanClick = true;
		yield break;
	}

	private void OnSelectType(MoYuTaskBoxSelectType type)
	{
		if (this.m_type == type)
		{
			return;
		}
		this.m_type = type;
		this.SetButtonState(this.btnDuiWu, this.m_type == MoYuTaskBoxSelectType.ZhanDuiSelect);
		this.SetButtonState(this.btnBoss, this.m_type == MoYuTaskBoxSelectType.BossSelect);
		this.goZhanDuiContent.SetActive(this.m_type == MoYuTaskBoxSelectType.ZhanDuiSelect);
		this.goBossContent.SetActive(this.m_type == MoYuTaskBoxSelectType.BossSelect);
		if (this.m_info == null)
		{
			return;
		}
		if (this.m_type == MoYuTaskBoxSelectType.ZhanDuiSelect)
		{
			this.RefershZhanDuiInfo();
		}
		else if (this.m_type == MoYuTaskBoxSelectType.BossSelect)
		{
			this.LoadBossInfo();
		}
	}

	private void LoadBossInfo()
	{
		this.m_bossAppearLeftTime = this.GetBossAppearTime();
		if (this.m_bossAppearLeftTime <= 0L)
		{
			this.goBossWillApear.SetActive(false);
			this.goBossDamage.SetActive(true);
		}
		else
		{
			this.goBossWillApear.SetActive(true);
			this.goBossDamage.SetActive(false);
			this.lblLeftTime.text = this.GetTimeStrBySecEx(this.m_bossAppearLeftTime);
		}
	}

	private void UpdateTimeInfo()
	{
		this.m_bossAppearLeftTime = this.GetBossAppearTime();
		if (this.m_lastLeftTime > 0L && this.m_bossAppearLeftTime <= 0L)
		{
			this.goBossWillApear.SetActive(false);
			this.goBossDamage.SetActive(true);
		}
		else
		{
			this.lblLeftTime.text = this.GetTimeStrBySecEx(this.m_bossAppearLeftTime);
		}
		this.m_lastLeftTime = this.m_bossAppearLeftTime;
	}

	private void RefershZhanDuiInfo()
	{
		bool flag = false;
		for (int i = 0; i < this.m_info.ZorkBattleRoleList.Count; i++)
		{
			ZorkBattleRoleInfo zorkBattleRoleInfo = this.m_info.ZorkBattleRoleList[i];
			MoYuTaskUserItem moYuTaskUserItem = null;
			if (this.m_dicRoles.TryGetValue(zorkBattleRoleInfo.RoleID, ref moYuTaskUserItem))
			{
				moYuTaskUserItem.RoleInfo = zorkBattleRoleInfo;
				if (!moYuTaskUserItem.gameObject.activeSelf)
				{
					flag = true;
					moYuTaskUserItem.gameObject.SetActive(true);
				}
			}
			else
			{
				flag = true;
				moYuTaskUserItem = this.AddUserItem(zorkBattleRoleInfo);
			}
		}
		for (int j = 0; j < this.m_lstRoles.Count; j++)
		{
			int roleId = this.m_lstRoles[j].RoleInfo.RoleID;
			if (this.m_info.ZorkBattleRoleList.Find((ZorkBattleRoleInfo info) => info.RoleID == roleId) == null)
			{
				MoYuTaskUserItem moYuTaskUserItem2 = this.m_dicRoles[roleId];
				moYuTaskUserItem2.gameObject.SetActive(false);
				flag = true;
			}
		}
		if (flag)
		{
			this.gridDuiWu.Reposition();
		}
	}

	private MoYuTaskUserItem AddUserItem(ZorkBattleRoleInfo info)
	{
		MoYuTaskUserItem moYuTaskUserItem = U3DUtils.NEW<MoYuTaskUserItem>();
		moYuTaskUserItem.transform.SetParent(this.gridDuiWu.transform);
		moYuTaskUserItem.transform.localPosition = Vector3.zero;
		moYuTaskUserItem.transform.localScale = Vector3.one;
		moYuTaskUserItem.RoleInfo = info;
		this.m_dicRoles[info.RoleID] = moYuTaskUserItem;
		this.m_lstRoles.Add(moYuTaskUserItem);
		return moYuTaskUserItem;
	}

	private void SetButtonState(GButton btn, bool beSelected)
	{
		if (beSelected)
		{
			Global.SetButtonSprite(btn, "AnNiu_GuanZhanXuanZhong");
		}
		else
		{
			Global.SetButtonSprite(btn, "AnNiu_GuanZhanAn");
		}
	}

	private string GetTimeStrBySecEx(long sec)
	{
		int num = 3600;
		if (sec > (long)num)
		{
			MUDebug.LogError<string>(new string[]
			{
				"请检查时间是否正确"
			});
		}
		long num2 = sec / 60L;
		long num3 = sec % 60L;
		if (num2 > 0L)
		{
			return string.Concat(new object[]
			{
				num2,
				Global.GetLang("分"),
				num3,
				Global.GetLang("秒")
			});
		}
		return num3 + Global.GetLang("秒");
	}

	private long GetBossAppearTime()
	{
		if (this.m_info == null)
		{
			return 0L;
		}
		Dictionary<int, string> mosterNextTimeDict = this.m_info.MosterNextTimeDict;
		string empty = string.Empty;
		if (mosterNextTimeDict.TryGetValue(this.m_bossId, ref empty))
		{
			if (empty == string.Empty)
			{
				return 0L;
			}
			DateTime dateTime = default(DateTime);
			if (DateTime.TryParse(empty, ref dateTime))
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				return (long)(dateTime - correctDateTime).TotalSeconds;
			}
		}
		return 0L;
	}

	private int GetBossId()
	{
		foreach (KeyValuePair<int, ZorkSceneVO> keyValuePair in IConfigbase<ConfigMoYuDuoBao>.Instance.DictZorkSceneVOCfg)
		{
			ZorkSceneVO value = keyValuePair.Value;
			if (value.ArmyType == 3)
			{
				return value.BuffAreID;
			}
		}
		return 0;
	}

	private void UpdateInfo(ZorkBattleSideScore info)
	{
		this.m_info = info;
		if (info.ZorkBattleTeamList.Count > 0)
		{
			this.lblZhanDuiName1.text = info.ZorkBattleTeamList[0].TeamName;
			this.lblDamage1.text = info.ZorkBattleTeamList[0].BossInjurePct + "%";
			this.ZhanDui1.SetActive(true);
		}
		else
		{
			this.ZhanDui1.SetActive(false);
		}
		if (info.ZorkBattleTeamList.Count > 1)
		{
			this.lblZhanDuiName2.text = info.ZorkBattleTeamList[1].TeamName;
			this.lblDamage2.text = info.ZorkBattleTeamList[1].BossInjurePct + "%";
			this.ZhanDui2.SetActive(true);
		}
		else
		{
			this.ZhanDui2.SetActive(false);
		}
		if (info.ZorkBattleTeamList.Count > 2)
		{
			this.lblZhanDuiName3.text = info.ZorkBattleTeamList[2].TeamName;
			this.lblDamage3.text = info.ZorkBattleTeamList[2].BossInjurePct + "%";
			this.ZhanDui3.SetActive(true);
		}
		else
		{
			this.ZhanDui3.SetActive(false);
		}
		PlayZone.GlobalPlayZone.SetBossBuff(info.BossBuffID);
		this.RefershZhanDuiInfo();
		if (this.TeamCompeteUITimer == null)
		{
			this.StartDuoBaoCountDown();
		}
	}

	private void StartDuoBaoCountDown()
	{
		this.StopTeamCompeteTimer();
		this.TeamCompeteUITimer = new DispatcherTimer("MoYuDuoBaoMainUITimer");
		this.TeamCompeteUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.TeamCompeteUITimer.Tick = new DispatcherTimerEventHandler(this.TeamCompeteUITimer_Tick);
		this.TeamCompeteUITimer.Start();
	}

	protected void TeamCompeteUITimer_Tick(object sender, object e)
	{
		this.UpdateTimeInfo();
	}

	private void StopTeamCompeteTimer()
	{
		if (this.TeamCompeteUITimer != null)
		{
			this.TeamCompeteUITimer.Tick = null;
			this.TeamCompeteUITimer.Stop();
			this.TeamCompeteUITimer = null;
		}
	}

	protected new virtual void OnDestroy()
	{
		base.OnDestroy();
		this.StopTeamCompeteTimer();
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
		MUEventManager.AddEventListener<ZorkBattleSideScore>("CMD_SPR_ZORK_SIDE_SCORE", new Action<ZorkBattleSideScore>(this.GetInfo));
		MUEventManager.AddEventListener<ZorkBattleAwardsData>("CMD_SPR_ZORK_AWARD", new Action<ZorkBattleAwardsData>(this.AwardGet));
		MUEventManager.AddEventListener<List<EscapeBattleJoinRoleInfo>>("CMD_SPR_ESCAPE_JOIN_INFO", new Action<List<EscapeBattleJoinRoleInfo>>(this.RespondDaTaoShaPrepareSceneInfo));
		MUEventManager.AddEventListener<EscapeBattleSideScore>("CMD_SPR_ESCAPE_SIDE_SCORE", new Action<EscapeBattleSideScore>(this.RespondDaTaoShaBattleSceneInfo));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<ZorkBattleSideScore>("CMD_SPR_ZORK_SIDE_SCORE", new Action<ZorkBattleSideScore>(this.GetInfo));
		MUEventManager.RemoveEventListener<ZorkBattleAwardsData>("CMD_SPR_ZORK_AWARD", new Action<ZorkBattleAwardsData>(this.AwardGet));
		MUEventManager.RemoveEventListener<List<EscapeBattleJoinRoleInfo>>("CMD_SPR_ESCAPE_JOIN_INFO", new Action<List<EscapeBattleJoinRoleInfo>>(this.RespondDaTaoShaPrepareSceneInfo));
		MUEventManager.RemoveEventListener<EscapeBattleSideScore>("CMD_SPR_ESCAPE_SIDE_SCORE", new Action<EscapeBattleSideScore>(this.RespondDaTaoShaBattleSceneInfo));
	}

	private void GetRongYuInfo()
	{
		GameInstance.Game.SendDuoBaoGetBaseInfo();
	}

	private void GetInfo(ZorkBattleSideScore data)
	{
		this.UpdateInfo(data);
	}

	private void AwardGet(ZorkBattleAwardsData data)
	{
		MoYuZhanDouEnd moYuZhanDouEnd = base.OpenWindow<MoYuZhanDouEnd>(true);
		ZorkDanAwardVO zorkDanAwardVODataById = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkDanAwardVODataById(data.AwardID);
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> list2 = new List<GoodsData>();
		string[] array;
		if (data.Success == 0)
		{
			array = zorkDanAwardVODataById.LoseRankReward;
		}
		else if (data.TeamWinNum == 1)
		{
			array = zorkDanAwardVODataById.FirstBattleReward;
		}
		else
		{
			array = zorkDanAwardVODataById.WinRankReward;
		}
		for (int i = 0; i < array.Length; i++)
		{
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(array[i]);
			list.Add(dummyGoodsData);
		}
		if (data.BossAwardGoodsDataList != null)
		{
			for (int j = 0; j < data.BossAwardGoodsDataList.Count; j++)
			{
				GoodsData goodsData = this.DumpToGoodsData(data.BossAwardGoodsDataList[j]);
				list2.Add(goodsData);
			}
		}
		moYuZhanDouEnd.SetInfo(data.TeamJiFen, data.Success == 1, data.SelfJiFen, list, list2, false);
	}

	private GoodsData DumpToGoodsData(AwardsItemData awardsItemData)
	{
		return new GoodsData
		{
			Id = 0,
			GoodsID = awardsItemData.GoodsID,
			Using = 0,
			Forge_level = awardsItemData.Level,
			Starttime = "1900-01-01 12:00:00",
			Endtime = awardsItemData.EndTime,
			Site = 0,
			Quality = awardsItemData.Quality,
			Props = string.Empty,
			GCount = awardsItemData.GoodsNum,
			Binding = awardsItemData.Binding,
			Jewellist = string.Empty,
			BagIndex = 0,
			AddPropIndex = 0,
			BornIndex = 0,
			Lucky = 0,
			Strong = 0,
			ExcellenceInfo = awardsItemData.ExcellencePorpValue,
			AppendPropLev = awardsItemData.IsHaveLuckyProp
		};
	}

	[ContextMenu("打开魔神秘宝")]
	private void ShouMoShenMiBao()
	{
		this.AwardGet(new ZorkBattleAwardsData
		{
			AwardID = 5,
			BossAwardGoodsDataList = null,
			RankNum = 2,
			SelfJiFen = 20,
			Success = 1,
			TeamJiFen = 120,
			TeamWinNum = 1
		});
	}

	private void IsInDaTaoSha()
	{
		if (Global.IsInDaTaoSha() || Global.IsInDaTaoShaPrepare())
		{
			this.m_offX = -220f;
			NGUITools.SetActive(this.bg, false);
			NGUITools.SetActive(this.top, false);
			NGUITools.SetActive(this.goBossContent, false);
			if (Global.IsInDaTaoShaPrepare())
			{
				this.eStatus = EDaTaoShaStatus.Prepare;
				NGUITools.SetActive(this.DaTaoShaObj, true);
				NGUITools.SetActive(this.DaTaoShaPrepareInfo, true);
				NGUITools.SetActive(this.goZhanDuiContent, false);
			}
			else if (Global.IsInDaTaoSha())
			{
				this.eStatus = EDaTaoShaStatus.Safe;
				NGUITools.SetActive(this.goZhanDuiContent, true);
				NGUITools.SetActive(this.DaTaoShaObj, false);
				NGUITools.SetActive(this.switchIcon, false);
				if (this.daTaoShaItems.Count <= 0)
				{
					for (int i = 0; i < 4; i++)
					{
						MoYuTaskUserItem moYuTaskUserItem = this.AddDaTaoShaUserItem();
						NGUITools.SetActive(moYuTaskUserItem.gameObject, false);
					}
				}
			}
		}
		else
		{
			this.m_offX = -150f;
			NGUITools.SetActive(this.DaTaoShaObj, false);
		}
	}

	private void InitDaTaoShaPrepareInfo(List<EscapeBattleJoinRoleInfo> datas)
	{
		this.mDaTaoShaPrepareInfo.InitValue(datas);
	}

	private MoYuTaskUserItem AddDaTaoShaUserItem()
	{
		MoYuTaskUserItem moYuTaskUserItem = U3DUtils.NEW<MoYuTaskUserItem>();
		moYuTaskUserItem.transform.SetParent(this.gridDuiWu.transform);
		moYuTaskUserItem.transform.localPosition = new Vector3(-40f, 0f, 0f);
		moYuTaskUserItem.transform.localScale = Vector3.one;
		this.daTaoShaItems.Add(moYuTaskUserItem);
		return moYuTaskUserItem;
	}

	public void RespondDaTaoShaPrepareSceneInfo(List<EscapeBattleJoinRoleInfo> datas)
	{
		this.InitDaTaoShaPrepareInfo(DaTaoShaDataManager.CacheEscapeBattleJoinRoleInfo);
	}

	public void RespondDaTaoShaBattleSceneInfo(EscapeBattleSideScore data)
	{
		bool flag = false;
		List<EscapeBattleRoleInfo> battleRoleList = data.BattleRoleList;
		if (battleRoleList != null && battleRoleList.Count > 0)
		{
			if (this.lastBattleMembers != battleRoleList.Count)
			{
				flag = true;
			}
			if (this.daTaoShaItems != null && this.daTaoShaItems.Count > 0)
			{
				int count = battleRoleList.Count;
				if (count > 0)
				{
					for (int i = 0; i < this.daTaoShaItems.Count; i++)
					{
						if (i + 1 > count)
						{
							this.daTaoShaItems[i].gameObject.SetActive(false);
						}
					}
				}
			}
			for (int j = 0; j < battleRoleList.Count; j++)
			{
				if (battleRoleList.Count > this.daTaoShaItems.Count)
				{
					MUDebug.LogError<string>(new string[]
					{
						"大逃杀战斗场景，左侧人数，服务器传的人数大于4"
					});
				}
				if (battleRoleList.Count <= this.daTaoShaItems.Count)
				{
					MoYuTaskUserItem moYuTaskUserItem = this.daTaoShaItems[j];
					NGUITools.SetActive(moYuTaskUserItem.gameObject, true);
					moYuTaskUserItem.InitDaTaoShaData(battleRoleList[j]);
					moYuTaskUserItem.transform.localPosition = new Vector3(-40f, moYuTaskUserItem.transform.localPosition.y, moYuTaskUserItem.transform.localPosition.z);
				}
			}
			if (flag)
			{
				this.gridDuiWu.Reposition();
			}
			for (int k = 0; k < this.daTaoShaItems.Count; k++)
			{
				MoYuTaskUserItem moYuTaskUserItem2 = this.daTaoShaItems[k];
				moYuTaskUserItem2.transform.localPosition = new Vector3(-40f, moYuTaskUserItem2.transform.localPosition.y, moYuTaskUserItem2.transform.localPosition.z);
			}
			this.lastBattleMembers = battleRoleList.Count;
		}
	}

	private void HideItem()
	{
		if (this.daTaoShaItems != null && this.daTaoShaItems.Count > 0)
		{
			for (int i = 0; i < this.daTaoShaItems.Count; i++)
			{
				NGUITools.SetActive(this.daTaoShaItems[i].gameObject, false);
			}
		}
	}

	public UILabel lblLeftWord;

	public UILabel lblLeftTime;

	public GameObject ZhanDui1;

	public UILabel lblDamage1;

	public UILabel lblZhanDuiName1;

	public GameObject ZhanDui2;

	public UILabel lblDamage2;

	public UILabel lblZhanDuiName2;

	public GameObject ZhanDui3;

	public UILabel lblZhanDuiName3;

	public UILabel lblDamage3;

	public GButton btnDuiWu;

	public GButton btnBoss;

	public GameObject goZhanDuiContent;

	public GameObject goBossContent;

	public UIGrid gridDuiWu;

	public GameObject goBossWillApear;

	public GameObject goBossDamage;

	public GameObject DaTaoShaObj;

	public GameObject DaTaoShaPrepareInfo;

	public GameObject DaTaoShaScoreInfo;

	public GameObject goSwitch;

	public GameObject goContent;

	public UISprite sprSwitch;

	private MoYuTaskBoxSelectType m_type;

	private long m_bossAppearLeftTime;

	private long m_lastLeftTime;

	private ZorkBattleSideScore m_info;

	private Dictionary<int, MoYuTaskUserItem> m_dicRoles = new Dictionary<int, MoYuTaskUserItem>();

	private List<MoYuTaskUserItem> m_lstRoles = new List<MoYuTaskUserItem>();

	private int m_bossId;

	private bool m_beShow = true;

	private bool m_beCanClick = true;

	private float m_offX = -150f;

	private DispatcherTimer TeamCompeteUITimer;

	public GameObject bg;

	public GameObject top;

	public GameObject switchIcon;

	public EDaTaoShaStatus eStatus;

	public DaTaoShaScenePrepareInfo mDaTaoShaPrepareInfo;

	private List<MoYuTaskUserItem> daTaoShaItems = new List<MoYuTaskUserItem>();

	private int lastBattleMembers;
}
