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

public class KuaFuWangZhePart : UserControl
{
	protected override void OnDestroy()
	{
		KuaFuWangZhePart.Instance = null;
	}

	private void InitTextInPrefabs()
	{
		this.staticText[0].text = Global.GetLang("活动时间：");
		this.staticText[1].text = Global.GetLang("十倍奖励：");
		this.m_BtnJoin.Text = Global.GetLang("立即报名");
		this.m_BtnEnter.Text = Global.GetLang("立即加入");
		this.m_labTime.Text = Global.GetLang("开启时间:");
		this.m_BtnLingJiang.Text = Global.GetLang("领取奖励");
		this.m_UILabelBaoMingMsg.lineWidth = 150;
		if (this.m_BtnHelp1.Text != null)
		{
			this.m_BtnHelp1.Text = Global.GetLang("详细规则");
		}
	}

	protected override void InitializeComponent()
	{
		KuaFuWangZhePart.Instance = this;
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
		};
		UIEventListener.Get(this.m_BtnHelp.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowRulePart();
		};
		this.m_BtnHelp1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowRulePart();
		};
		UIEventListener.Get(this.m_btnShop.gameObject).onClick = delegate(GameObject s)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ShowKuaFuWangZheMallWindow(false, 0, 0);
			}
		};
		UIEventListener.Get(this.m_btnShiPin.gameObject).onClick = delegate(GameObject s)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 2
				});
			}
		};
		this.m_BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (KuaFuWangZhePart.LocalState == KuafuActivityLocalState.SignUp)
			{
				this.Send_CMD_SPR_KINGOFBATTLE_JOIN();
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
						if (KuaFuWangZhePart._YongZheZhanGameStates == 2 || KuaFuWangZhePart._YongZheZhanGameStates == 3)
						{
							int yongZheZhanGameStates2 = KuaFuWangZhePart._YongZheZhanGameStates;
							KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_ENTER(yongZheZhanGameStates2.ToString());
						}
					}
				}, -1);
			}
			else if (KuaFuWangZhePart._YongZheZhanGameStates == 2 || KuaFuWangZhePart._YongZheZhanGameStates == 3)
			{
				int yongZheZhanGameStates = KuaFuWangZhePart._YongZheZhanGameStates;
				KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_ENTER(yongZheZhanGameStates.ToString());
			}
		};
		this.m_BtnLingJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (KuaFuWangZhePart._YongZheZhanGameStates == 4)
			{
				TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_AWARD.SendDataUseRoleID();
			}
		};
		this.m_BtnRuleClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_rulePart.gameObject.SetActive(false);
		};
		this.InitTextInPrefabs();
		this.RewardOBC = this.m_RewardList.ItemsSource;
		KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_STATE();
	}

	public void ResetZhuangTai()
	{
		if (KuaFuWangZhePart._YongZheZhanGameStates == 1)
		{
			this.m_BtnJoin.gameObject.SetActive(true);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
			KuaFuWangZhePart.isShowBaoMingSuccessHint = false;
		}
		else if (KuaFuWangZhePart._YongZheZhanGameStates == 2)
		{
			if (!KuaFuWangZhePart.isShowBaoMingSuccessHint)
			{
				Super.HintMainText(Global.GetLang("报名成功，本次活动开启后，可随时进入战场！"), 10, 3);
				KuaFuWangZhePart.isShowBaoMingSuccessHint = true;
			}
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名成功");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuaFuWangZhePart._YongZheZhanGameStates == 3)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(true);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuaFuWangZhePart._YongZheZhanGameStates == 4)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(true);
		}
		else if (KuaFuWangZhePart._YongZheZhanGameStates == null)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名时间未开始");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuaFuWangZhePart._YongZheZhanGameStates == 5)
		{
			if (KuaFuWangZhePart.LocalState != KuafuActivityLocalState.SignUp)
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
			if (KuaFuWangZhePart.LocalState == KuafuActivityLocalState.Wait)
			{
				this.m_labTime.text = Global.GetLang("距活动开始时间 \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuaFuWangZhePart.LocalState == KuafuActivityLocalState.SignUp)
			{
				this.m_labTime.text = Global.GetLang("距报名结束时间  \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuaFuWangZhePart.LocalState == KuafuActivityLocalState.Start)
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
			KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_STATE();
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
		this.m_labRule.Text = Global.GetLang(string.Empty);
		this.m_rulePart.gameObject.SetActive(true);
	}

	private static bool IsNetResultSuccess(MUSocketConnectEventArgs e)
	{
		return true;
	}

	private void Send_CMD_SPR_KINGOFBATTLE_JOIN()
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_JOIN.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_JOIN(MUSocketConnectEventArgs e)
	{
		if (KuaFuWangZhePart.Instance)
		{
			int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
			if (num < 0)
			{
				string errMsg = StdErrorCode.GetErrMsg(num, true, false);
				Super.HintMainText(errMsg, 10, 3);
			}
			else
			{
				KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_STATE();
				KuaFuWangZhePart.SocreZiji = 0;
			}
		}
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_ENTER(string s)
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_ENTER.SendData(s);
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_ENTER(MUSocketConnectEventArgs e)
	{
		KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_STATE();
		int intR = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (KuaFuWangZhePart.Instance == null && intR > 0)
		{
			PlayZone.GlobalPlayZone.ShowDaojishi(Global.GetLang("立即进入"), Global.GetLang("取消"), Global.GetLang("王者战场已开启,是否立即进入？"), string.Empty, false, delegate(object s, DPSelectedItemEventArgs eh)
			{
				if (eh.ID == 0)
				{
					PlayZone.GlobalPlayZone.CloseDaojishiWindow();
				}
				if (eh.ID == 1)
				{
					KuaFuWangZhePart.Send_CMD_SPR_KINGOFBATTLE_ENTER(intR.ToString());
				}
			});
		}
		else if (intR <= 0)
		{
			if (KuaFuWangZhePart._YongZheZhanGameStates == 3)
			{
				Super.HintMainText(Global.GetLang("当前地图无法传送至其他活动，请退出后再试！"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("活动未开始"), 10, 3);
			}
		}
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_AWARD()
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_AWARD.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_AWARD(MUSocketConnectEventArgs e)
	{
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_STATE()
	{
		TCPGameServerCmds.CMD_SPR_KINGOFBATTLE_STATE.SendDataUseRoleID();
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_STATE(MUSocketConnectEventArgs e)
	{
		int yongZheZhanGameStates = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		KuaFuWangZhePart._YongZheZhanGameStates = yongZheZhanGameStates;
		if (KuaFuWangZhePart.Instance != null)
		{
			KuaFuWangZhePart.Instance.ResetZhuangTai();
		}
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_SIDE_SCORE()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_SIDE_SCORE(MUSocketConnectEventArgs e)
	{
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_SELF_SCORE()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_SELF_SCORE(MUSocketConnectEventArgs e)
	{
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_LIANSHA()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_LIANSHA(MUSocketConnectEventArgs e)
	{
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_STOP_LIANSHA()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_STOP_LIANSHA(MUSocketConnectEventArgs e)
	{
	}

	private static void Send_CMD_SPR_KINGOFBATTLE_TELEPORT()
	{
	}

	public static void Action_CMD_SPR_KINGOFBATTLE_TELEPORT(MUSocketConnectEventArgs e)
	{
	}

	public static KuaFuWangZhePart.WangzheData wangzheData = new KuaFuWangZhePart.WangzheData();

	public static KuaFuWangZhePart Instance;

	public static KuafuActivityLocalState LocalState;

	public static YongZheZhanChangGameStates _YongZheZhanGameStates;

	public GButton m_BtnClose;

	public UIButton m_BtnHelp;

	public GButton m_BtnHelp1;

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

	public UIButton m_btnShop;

	public UIButton m_btnShiPin;

	private GKuafuActivityData.ItemData data;

	private KuafuActivityItemConfig kuafuConfig = new KuafuActivityItemConfig();

	private string[] groupTime;

	public static int SocreMengJun = 0;

	public static int SocreJunTuan = 0;

	public static int SocreZiji = 0;

	private static bool isShowBaoMingSuccessHint = false;

	public class WangzheData
	{
		public int nJiFen
		{
			get
			{
				return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.KingOfBattlePoint);
			}
		}

		public List<int> TeleportEnableList
		{
			get
			{
				return this.teleportEnableList;
			}
		}

		public bool IsTeleportEnable(int Key)
		{
			for (int i = 0; i < this.teleportEnableList.Count; i++)
			{
				if (this.teleportEnableList[i] == Key)
				{
					return true;
				}
			}
			return false;
		}

		private List<int> teleportEnableList = new List<int>();
	}
}
