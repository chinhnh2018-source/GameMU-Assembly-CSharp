using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class KuafuBossPart : UserControl
{
	protected override void OnDestroy()
	{
		KuafuBossPart.Instance = null;
	}

	private void InitTextInPrefabs()
	{
		this.staticText[0].text = Global.GetLang("活动时间：");
		this.staticText[1].text = Global.GetLang("十倍奖励：");
		this.m_BtnJoin.Text = Global.GetLang("立即报名");
		this.m_BtnEnter.Text = Global.GetLang("立即加入");
		this.m_labTime.Text = Global.GetLang("开启时间:");
		this.m_UILabelBaoMingMsg.transform.localPosition = new Vector3(320f, -214f, -1f);
	}

	protected override void InitializeComponent()
	{
		KuafuBossPart.Instance = this;
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
		};
		UIEventListener.Get(this.m_BtnHelp.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowRulePart();
		};
		this.m_BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (KuafuBossPart.LocalState == KuafuActivityLocalState.SignUp && KuafuBossPart._YongZheZhanGameStates == 1)
			{
				this.Send_CMD_SPR_KUAFU_BOSS_JOIN();
			}
			else if (KuafuBossPart.LocalState == KuafuActivityLocalState.SignUp && KuafuBossPart._YongZheZhanGameStates == 5)
			{
				Super.HintMainText(Global.GetLang("已报名，无需再报名"), 10, 3);
			}
		};
		this.m_BtnEnter.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						if (KuafuBossPart._YongZheZhanGameStates == 2 || KuafuBossPart._YongZheZhanGameStates == 3)
						{
							int yongZheZhanGameStates2 = KuafuBossPart._YongZheZhanGameStates;
							KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_ENTER(yongZheZhanGameStates2.ToString());
						}
					}
				}, -1);
			}
			else if (KuafuBossPart._YongZheZhanGameStates == 2 || KuafuBossPart._YongZheZhanGameStates == 3)
			{
				int yongZheZhanGameStates = KuafuBossPart._YongZheZhanGameStates;
				KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_ENTER(yongZheZhanGameStates.ToString());
			}
		};
		this.m_BtnLingJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (KuafuBossPart._YongZheZhanGameStates == 4)
			{
			}
		};
		this.m_BtnRuleClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_rulePart.gameObject.SetActive(false);
		};
		this.InitTextInPrefabs();
		this.RewardOBC = this.m_RewardList.ItemsSource;
		KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_STATE();
	}

	public void ResetZhuangTai()
	{
		if (KuafuBossPart._YongZheZhanGameStates == 1)
		{
			this.m_BtnJoin.gameObject.SetActive(true);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuBossPart._YongZheZhanGameStates == 2)
		{
			if (!KuafuBossPart.isShowBaoMingSuccessHint)
			{
				Super.HintMainText(Global.GetLang("报名成功"), 10, 3);
				KuafuBossPart.isShowBaoMingSuccessHint = true;
			}
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名成功,等待活动开启");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuBossPart._YongZheZhanGameStates == 3)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(true);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuBossPart._YongZheZhanGameStates == 4)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(true);
		}
		else if (KuafuBossPart._YongZheZhanGameStates == null)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名时间未开始");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuBossPart._YongZheZhanGameStates == 5)
		{
			if (KuafuBossPart.LocalState != KuafuActivityLocalState.SignUp)
			{
				this.m_BtnJoin.gameObject.SetActive(false);
				this.m_UILabelBaoMingMsg.enabled = true;
				this.m_UILabelBaoMingMsg.text = Global.GetLang("报名时间已结束");
			}
			else
			{
				this.m_BtnJoin.gameObject.SetActive(true);
				this.m_UILabelBaoMingMsg.enabled = false;
			}
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
	}

	public void InitData(KuafuActivityItem item)
	{
		if (item == null)
		{
			return;
		}
		this.data = item.Data;
		this.groupTime = this.data.configItem.time;
		this.StartTickInvoke();
		this.m_labTimeGroup.text = this.data.GetGroup(this.groupTime, 0);
		this.loadGoodsList(this.data.configItem.award);
	}

	private DateTime nextDate
	{
		get
		{
			return this.data.nextDate;
		}
	}

	private void StartTickInvoke()
	{
		this.data.RefreshNextDate();
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextDate.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (KuafuBossPart.LocalState == KuafuActivityLocalState.Wait)
			{
				this.m_labTime.text = Global.GetLang("距活动开始时间 \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuafuBossPart.LocalState == KuafuActivityLocalState.SignUp)
			{
				this.m_labTime.text = Global.GetLang("距报名结束时间  \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuafuBossPart.LocalState == KuafuActivityLocalState.Start)
			{
				this.m_labTime.text = Global.GetLang("距活动结束时间  \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else
			{
				this.m_labTime.text = Global.GetLang("距报名开始时间  \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"fd010c",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
		}
		else
		{
			KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_STATE();
			base.CancelInvoke("TickProc");
			this.StartTickInvoke();
			this.m_labTimeGroup.text = this.data.GetGroup(this.groupTime, 0);
		}
	}

	private int ToInt(string str)
	{
		return Global.SafeConvertToInt32(str);
	}

	private void loadGoodsList(string[] goodsID)
	{
		this.RewardOBC.Clear();
		for (int i = 0; i < goodsID.Length; i++)
		{
			this.AddGoodsIcon(Global.SafeConvertToInt32(goodsID[i]));
		}
	}

	private void AddGoodsIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = dummyGoodsData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.STextVisibility = false;
			bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			this.RewardOBC.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	private void ShowRulePart()
	{
		this.m_labRule.Y = 0.0;
		this.m_labRule.Text = string.Concat(new string[]
		{
			Global.GetLang("{C7C605}报名规则{-}"),
			"\r\n\r\n",
			Global.GetLang("1、每周六，周日14:00-14:25|18:00-18:25可报名"),
			"\r\n\r\n",
			Global.GetLang("{C7C605}活动规则{-}"),
			"\r\n\r\n",
			Global.GetLang("1、活动开始后，地图中会刷新5个{C7C605}BOOS{-}和大量{C7C605}小怪{-}"),
			"\r\n\r\n",
			Global.GetLang("2、复活会{C7C605}随即分配{-}到一个{C7C605}复活点{-}"),
			"\r\n\r\n",
			Global.GetLang("3、地图内的{C7C605}小怪掉落无归属{-}，可以随意拾取")
		});
		this.m_rulePart.gameObject.SetActive(true);
	}

	private static bool IsNetResultSuccess(MUSocketConnectEventArgs e)
	{
		return true;
	}

	private void Send_CMD_SPR_KUAFU_BOSS_JOIN()
	{
		TCPGameServerCmds.CMD_SPR_KUAFU_BOSS_JOIN.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KUAFU_BOSS_JOIN(MUSocketConnectEventArgs e)
	{
		if (KuafuBossPart.Instance)
		{
			int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
			if (num < 0)
			{
				string errMsg = StdErrorCode.GetErrMsg(num, true, false);
				Super.HintMainText(errMsg, 10, 3);
			}
			else
			{
				KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_STATE();
				KuafuBossPart.SocreZiji = 0;
			}
		}
	}

	private static void Send_CMD_SPR_KUAFU_BOSS_ENTER(string s)
	{
		TCPGameServerCmds.CMD_SPR_KUAFU_BOSS_ENTER.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KUAFU_BOSS_ENTER(MUSocketConnectEventArgs e)
	{
		KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_STATE();
		int intR = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		MUDebug.LogError<int>(new int[]
		{
			intR
		});
		if (KuafuBossPart.Instance == null && intR > 0)
		{
			PlayZone.GlobalPlayZone.ShowDaojishi(Global.GetLang("立即进入"), Global.GetLang("取消"), Global.GetLang("魔炼之地已开启,是否立即进入？"), string.Empty, false, delegate(object s, DPSelectedItemEventArgs eh)
			{
				if (eh.ID == 0)
				{
					PlayZone.GlobalPlayZone.CloseDaojishiWindow();
				}
				if (eh.ID == 1)
				{
					KuafuBossPart.Send_CMD_SPR_KUAFU_BOSS_ENTER(intR.ToString());
				}
			});
		}
		else if (intR <= 0)
		{
			Super.HintMainText(Global.GetLang("当前地图无法传送至其他活动，请退出后再试！"), 10, 3);
		}
	}

	private static void Send_CMD_SPR_KUAFU_BOSS_STATE()
	{
		TCPGameServerCmds.CMD_SPR_KUAFU_BOSS_STATE.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KUAFU_BOSS_STATE(MUSocketConnectEventArgs e)
	{
		int yongZheZhanGameStates = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		KuafuBossPart._YongZheZhanGameStates = yongZheZhanGameStates;
		if (KuafuBossPart.Instance != null)
		{
			KuafuBossPart.Instance.ResetZhuangTai();
		}
	}

	private static void Send_CMD_SPR_KUAFU_BOSS_DATA()
	{
	}

	public static void Action_CMD_SPR_KUAFU_BOSS_DATA(MUSocketConnectEventArgs e)
	{
		KuaFuBossSceneStateData kuaFuBossSceneStateData = DataHelper.BytesToObject<KuaFuBossSceneStateData>(e.bytesData, 0, e.bytesData.Length);
		if (PlayZone.GlobalPlayZone.GameFubenBoxMini != null && kuaFuBossSceneStateData != null)
		{
			PlayZone.GlobalPlayZone.SetSceneTaskInfos(0, ColorCode.EncodingText(string.Format(Global.GetLang("     剩余BOSS：{0} "), kuaFuBossSceneStateData.BossNum), "fffffe"), new object[0]);
			PlayZone.GlobalPlayZone.SetSceneTaskInfos(1, ColorCode.EncodingText(string.Format(Global.GetLang("     剩余小怪：{0} "), kuaFuBossSceneStateData.MonsterNum), "fffffe"), new object[0]);
		}
	}

	public static KuafuBossPart Instance;

	public static KuafuActivityLocalState LocalState;

	public static YongZheZhanChangGameStates _YongZheZhanGameStates;

	public GButton m_BtnClose;

	public UIButton m_BtnHelp;

	public UIButton m_BtnHelp1;

	public TextBlock m_labTime;

	public TextBlock m_labTimeGroup;

	public TextBlock m_labShibei;

	public TextBlock m_labDengdai;

	public GButton m_BtnJoin;

	public ListBox m_RewardList;

	public GButton m_BtnRuleClose;

	public TextBlock m_labRule;

	public Transform m_rulePart;

	public UISprite m_blackSprite;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection RewardOBC;

	public TextBlock[] staticText;

	public GButton m_BtnEnter;

	public GButton m_BtnLingJiang;

	public UILabel m_UILabelBaoMingMsg;

	private GKuafuActivityData.ItemData data;

	private KuafuActivityItemConfig kuafuConfig = new KuafuActivityItemConfig();

	private string[] groupTime;

	public static int SocreMengJun;

	public static int SocreJunTuan;

	public static int SocreZiji;

	private static bool isShowBaoMingSuccessHint;
}
