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

public class KuafuJoinPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		UIEventListener.Get(this.BtnHelp1.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.ShowDaojishi(null, null, null, null, true, null);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, null);
		};
		UIEventListener.Get(this.BtnHelp.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowRulePart();
		};
		this.BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BtnJoin.Text == Global.GetLang("立即报名"))
			{
				if (Global.Data.CurrentCopyTeamData != null)
				{
					Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
					{
						if (e2.ID == 0)
						{
							GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
							GameInstance.Game.JoinHuanYingGroup();
						}
					}, -1);
				}
				else
				{
					GameInstance.Game.JoinHuanYingGroup();
				}
			}
			else
			{
				GameInstance.Game.QuitHuanYingGroup();
			}
		};
		this.BtnRuleClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.rulePart.gameObject.SetActive(false);
		};
		UIEventListener.Get(this.blackSprite.gameObject).onClick = delegate(GameObject s)
		{
			this.rulePart.gameObject.SetActive(false);
		};
		this.RewardOBC = this.RewardList.ItemsSource;
		GameInstance.Game.GetWaitNum();
		GameInstance.Game.GetHuanYingWinNum();
	}

	private void InitTextInPrefabs()
	{
		this.staticText[0].text = Global.GetLang("刷新时间：");
		this.staticText[1].text = Global.GetLang("三十倍奖励：");
		this.BtnJoin.Text = Global.GetLang("立即报名");
		this.labDengdai.text = Global.GetLang("本场等待人数：0/16");
		this.labTimeGroup.X = -320.0;
		this.labShibei.X = -365.0;
		this.labRule.X = -200.0;
	}

	public void InitData(KuafuActivityItem item)
	{
		if (item == null)
		{
			return;
		}
		this.blState = item.IsWaitingState;
		this.kuafuConfig = item.Data.configItem;
		this.groupTime = item.Data.configItem.time;
		this.GetTimes();
		this.labTimeGroup.text = this.GetGroup(this.groupTime, this.nGroup);
		this.loadGoodsList(item.Data.configItem.award);
	}

	private void GetTimes()
	{
		List<string> timePoints = this.kuafuConfig.timePoints;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime dateTime3;
		dateTime3..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.ToInt(timePoints[timePoints.Count - 2]), this.ToInt(timePoints[timePoints.Count - 1]), 0);
		for (int i = 0; i < timePoints.Count; i += 4)
		{
			dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.ToInt(timePoints[i]), this.ToInt(timePoints[i + 1]), 0);
			dateTime2..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, this.ToInt(timePoints[i + 2]), this.ToInt(timePoints[i + 3]), 0);
			if (correctDateTime < dateTime)
			{
				this.blState = true;
				this.nextDate = dateTime;
				this.nGroup = i / 4;
				break;
			}
			if (correctDateTime >= dateTime && correctDateTime <= dateTime2)
			{
				this.blState = false;
				this.nextDate = dateTime2;
				this.nGroup = i / 4;
				break;
			}
			if (correctDateTime > dateTime3)
			{
				this.nextDate = new DateTime(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day + 1, this.ToInt(timePoints[0]), this.ToInt(timePoints[1]), 0);
				this.nGroup = i / 4;
				this.blState = true;
				break;
			}
		}
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextDate.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (num2 <= 0)
			{
				base.CancelInvoke("TickProc");
				this.GetTimes();
				this.labTimeGroup.text = this.GetGroup(this.groupTime, this.nGroup);
				KuafuJoinPart.IsBaoMing = 0;
			}
			else if (this.blState)
			{
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("开启时间： 剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
			else
			{
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("结束时间： 剩余 {0}"), Global.GetTimeStrBySecEx((double)num2, true, -1))
				});
			}
		}
		else
		{
			base.CancelInvoke("TickProc");
			this.GetTimes();
			this.labTimeGroup.text = this.GetGroup(this.groupTime, this.nGroup);
			KuafuJoinPart.IsBaoMing = 0;
		}
	}

	private int ToInt(string str)
	{
		return Global.SafeConvertToInt32(str);
	}

	private string GetGroup(string[] str, int n)
	{
		string text = string.Empty;
		if (!str[1].Contains("|") && !str[2].Contains("|"))
		{
			str[1] = "|" + str[1];
			str[2] = "|" + str[2];
		}
		for (int i = 0; i < str.Length; i++)
		{
			if (n == i)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					str[i]
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					str[i]
				});
			}
		}
		return text;
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
		string text = "fac60d";
		string text2 = "dac7ae";
		string text3 = "17e43e";
		string text4 = "ff0000";
		this.labRule.text = string.Concat(new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				text,
				Global.GetLang("幻影寺院规则说明：")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("1、胜负规则")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("     先达到"),
				text3,
				Global.GetLang("1000积分"),
				text2,
				Global.GetLang("的阵营获胜")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("     本场次结束时积分高的阵营获胜")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("2、积分规则")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("     搬运圣杯："),
				text3,
				Global.GetLang("50积分")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("     击杀玩家："),
				text3,
				Global.GetLang("8积分")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("3、奖励规则")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text3,
				Global.GetLang("     胜利方"),
				text2,
				Global.GetLang("可获得"),
				text3,
				Global.GetLang("100%"),
				text2,
				Global.GetLang("的奖励")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text4,
				Global.GetLang("     失败方"),
				text2,
				Global.GetLang("可获得"),
				text3,
				Global.GetLang("50%"),
				text2,
				Global.GetLang("的奖励")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("4、30倍奖励")
			}),
			"\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				Global.GetLang("     每天"),
				text3,
				Global.GetLang("首次胜利"),
				text2,
				Global.GetLang("可获得"),
				text3,
				Global.GetLang("30倍奖励")
			})
		});
		this.rulePart.gameObject.SetActive(true);
	}

	public void JoinResult(int ret)
	{
		if (ret >= 0)
		{
			if (this.BtnJoin.Text == Global.GetLang("立即报名"))
			{
				this.BtnJoin.Text = Global.GetLang(string.Empty);
				this.BtnJoin.Text = Global.GetLang("取消报名");
				this.BtnJoin.pressedSprite = "tongyongBtn6_normal";
				this.BtnJoin.Pressed = true;
				this.labDengdai.text = string.Format(Global.GetLang("本场等待人数：{0}/16"), 1);
				KuafuJoinPart.IsBaoMing = Global.Data.roleData.RoleID;
			}
			else
			{
				this.BtnJoin.Text = Global.GetLang(string.Empty);
				this.BtnJoin.Text = Global.GetLang("立即报名");
				this.BtnJoin.normalSprite = "tongyongBtn3_normal";
				this.BtnJoin.Pressed = false;
				this.labDengdai.text = string.Format(Global.GetLang("本场等待人数：{0}/16"), 0);
				KuafuJoinPart.IsBaoMing = 0;
			}
		}
		else
		{
			string errMsg = StdErrorCode.GetErrMsg(ret, true, true);
			Super.HintMainText(errMsg, 10, 3);
			KuafuJoinPart.IsBaoMing = 0;
		}
	}

	public void GetWaitNum(int ret)
	{
		this.labDengdai.text = string.Format(Global.GetLang("本场等待人数：{0}/16"), (ret <= 0) ? 0 : ret);
		if (ret > 0)
		{
			this.BtnJoin.Text = Global.GetLang(string.Empty);
			this.BtnJoin.Text = Global.GetLang("取消报名");
			this.BtnJoin.pressedSprite = "tongyongBtn6_normal";
			this.BtnJoin.Pressed = true;
		}
		else
		{
			this.BtnJoin.Text = Global.GetLang(string.Empty);
			this.BtnJoin.Text = Global.GetLang("立即报名");
			this.BtnJoin.normalSprite = "tongyongBtn3_normal";
			this.BtnJoin.Pressed = false;
		}
	}

	public void GetWinNum(int ret)
	{
		int num = (ret <= 1) ? ret : 1;
		this.labShibei.text = string.Format("{0}/1", (num <= 0) ? 0 : num);
	}

	public void GetJoinResult(int ret)
	{
		if (ret >= 0)
		{
			PlayZone.GlobalPlayZone.CloseDaojishiWindow();
		}
		else
		{
			string errMsg = StdErrorCode.GetErrMsg(ret, true, true);
			Super.HintMainText(errMsg, 10, 3);
		}
	}

	public GButton BtnClose;

	public UIButton BtnHelp;

	public UIButton BtnHelp1;

	public TextBlock labTime;

	public TextBlock labTimeGroup;

	public TextBlock labShibei;

	public TextBlock labDengdai;

	public GButton BtnJoin;

	public ListBox RewardList;

	public GButton BtnRuleClose;

	public TextBlock labRule;

	public GameObject rulePart;

	public UISprite blackSprite;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection RewardOBC;

	public TextBlock[] staticText;

	private bool blState;

	private KuafuActivityItemConfig kuafuConfig = new KuafuActivityItemConfig();

	private int nGroup = 1;

	private string[] groupTime;

	private DateTime nextDate;

	public static int IsBaoMing;
}
