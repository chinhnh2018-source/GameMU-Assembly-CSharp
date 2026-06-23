using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class KuafuYongZheZhanChangPart : UserControl
{
	protected override void OnDestroy()
	{
		KuafuYongZheZhanChangPart.Instance = null;
	}

	private void InitTextInPrefabs()
	{
		this.m_UILabelBaoMingMsg.pivot = 5;
		this.m_UILabelBaoMingMsg.transform.localPosition = new Vector3(450f, -215f, -1f);
		this.m_labTime.Z = -0.10000000149011612;
		this.m_labTimeGroup.Text = string.Empty;
		this.staticText[0].text = Global.GetLang("活动时间：");
		this.staticText[1].text = Global.GetLang("十倍奖励：");
		this.m_BtnJoin.Text = Global.GetLang("立即报名");
		this.m_BtnEnter.Text = Global.GetLang("立即加入");
		this.m_labTime.Text = Global.GetLang("开启时间:");
		this.m_BtnLingJiang.Text = Global.GetLang("领取奖励");
		this.m_labRule.text = Global.GetLang("越南跨服勇者战场规则,一大串，看语言表配置");
	}

	protected override void InitializeComponent()
	{
		KuafuYongZheZhanChangPart.Instance = this;
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
			if (KuafuYongZheZhanChangPart.LocalState == KuafuActivityLocalState.SignUp && KuafuYongZheZhanChangPart._YongZheZhanGameStates == 1)
			{
				TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_JOIN.SendDataUseRoleID();
			}
			else if (KuafuYongZheZhanChangPart.LocalState == KuafuActivityLocalState.SignUp && KuafuBossPart._YongZheZhanGameStates == 5)
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
						if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 2 || KuafuYongZheZhanChangPart._YongZheZhanGameStates == 3)
						{
							TCPGameServerCmds tcpgameServerCmds2 = TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_ENTER;
							int yongZheZhanGameStates2 = KuafuYongZheZhanChangPart._YongZheZhanGameStates;
							tcpgameServerCmds2.SendData(yongZheZhanGameStates2.ToString());
						}
					}
				}, -1);
			}
			else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 2 || KuafuYongZheZhanChangPart._YongZheZhanGameStates == 3)
			{
				TCPGameServerCmds tcpgameServerCmds = TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_ENTER;
				int yongZheZhanGameStates = KuafuYongZheZhanChangPart._YongZheZhanGameStates;
				tcpgameServerCmds.SendData(yongZheZhanGameStates.ToString());
			}
		};
		this.m_BtnLingJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 4)
			{
				TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_AWARD.SendDataUseRoleID();
			}
		};
		this.m_BtnShiPin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 3
				});
			}
		};
		this.m_BtnRuleClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_rulePart.gameObject.SetActive(false);
		};
		this.InitTextInPrefabs();
		this.RewardOBC = this.m_RewardList.ItemsSource;
		TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_STATE.SendDataUseRoleID();
	}

	public void Action_CMD_SPR_YONGZHEZHANCHANG_JOIN(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			string errMsg = StdErrorCode.GetErrMsg(num, true, false);
			Super.HintMainText(errMsg, 10, 3);
		}
		else
		{
			TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_STATE.SendDataUseRoleID();
			KuafuYongZheZhanChangPart.SocreZiji = 0;
		}
	}

	public void ResetZhuangTai()
	{
		if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 1)
		{
			this.m_BtnJoin.gameObject.SetActive(true);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 2)
		{
			if (!KuafuYongZheZhanChangPart.isShowBaoMingSuccessHint)
			{
				Super.HintMainText(Global.GetLang("报名成功"), 10, 3);
				KuafuYongZheZhanChangPart.isShowBaoMingSuccessHint = true;
			}
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名成功,等待活动开启");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 3)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(true);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 4)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = false;
			this.m_BtnLingJiang.gameObject.SetActive(true);
		}
		else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == null)
		{
			this.m_BtnJoin.gameObject.SetActive(false);
			this.m_BtnEnter.gameObject.SetActive(false);
			this.m_UILabelBaoMingMsg.enabled = true;
			this.m_UILabelBaoMingMsg.text = Global.GetLang("报名时间未开始");
			this.m_BtnLingJiang.gameObject.SetActive(false);
		}
		else if (KuafuYongZheZhanChangPart._YongZheZhanGameStates == 5)
		{
			if (KuafuYongZheZhanChangPart.LocalState != KuafuActivityLocalState.SignUp)
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
			if (KuafuYongZheZhanChangPart.LocalState == KuafuActivityLocalState.Wait)
			{
				this.m_labTime.text = Global.GetLang("距活动开始时间 \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuafuYongZheZhanChangPart.LocalState == KuafuActivityLocalState.SignUp)
			{
				this.m_labTime.text = Global.GetLang("距报名结束时间  \n") + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else if (KuafuYongZheZhanChangPart.LocalState == KuafuActivityLocalState.Start)
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
			TCPGameServerCmds.CMD_SPR_YONGZHEZHANCHANG_STATE.SendDataUseRoleID();
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

	public void AddGoodsIcon(int goodsID)
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
		this.m_rulePart.gameObject.SetActive(true);
	}

	public static KuafuYongZheZhanChangPart Instance;

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

	public GButton m_BtnShiPin;

	public UILabel m_UILabelBaoMingMsg;

	private GKuafuActivityData.ItemData data;

	private KuafuActivityItemConfig kuafuConfig = new KuafuActivityItemConfig();

	private string[] groupTime;

	public static int SocreMengJun;

	public static int SocreJunTuan;

	public static int SocreZiji;

	private static bool isShowBaoMingSuccessHint;
}
