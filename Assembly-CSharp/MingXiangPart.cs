using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MingXiangPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this._Desc1.Text = Global.GetLang("(离线同样可以享受丰厚的冥想收益)");
		this.mMXLbls[0].Text = Global.GetLang("冥想时间：");
		this.mLevelLbls[0].Text = Global.GetLang("等级提升：");
		this.mExpLbls[0].Text = Global.GetLang("经验收益：");
		this.mHuoBiLbls[0].Text = Global.GetLang("货币收益：");
		this.mLblDaoJuTitle.Text = Global.GetLang("道具收益：");
		this.mBtn1.Text = Global.GetLang("领取");
		this.staticText.text = Global.GetLang("冥想修炼");
		this._MingXiangTime.X = -176.0;
		this._MingXiangShouYi.X = -186.0;
		this._Desc2.X = 195.0;
		this._TimeScroll.transform.localPosition = new Vector3(-95f, 104f, -0.02f);
		this._LingQu2.Label.transform.localPosition = new Vector3(12f, -1f, -0.01f);
		this._LingQu4.Label.transform.localPosition = new Vector3(12f, -2f, -0.01f);
		this.mMXLbls[1].Pivot = 3;
		this.mMXLbls[1].X = -12.0;
		this.mMXLbls[2].Pivot = 5;
		this.mMXLbls[2].X = 220.0;
		this.mLevelLbls[1].X = -100.0;
		this.checkBox1.Label.text = Global.GetLang("1倍收益");
		this.checkBox2.Label.text = Global.GetLang("2倍收益");
		this.checkBox4.Label.text = Global.GetLang("4倍收益");
		this.mBtn2.Label.text = Global.GetLang("领取");
	}

	private void InitValue()
	{
		this.IsShowVipLimitFlag = false;
		this.ItemCollection = this.mAwardList.ItemsSource;
		if (this.dragPabel != null)
		{
			this.dragPabel.onDragFinished = delegate()
			{
				if ((double)this.dragPabel.verticalScrollBar.scrollValue > 0.9)
				{
					this.ShowGoodIcon();
				}
			};
		}
	}

	private void InitClickEvent()
	{
		this.mBtn1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.LingQu_OnClick(0);
		};
		this.mBtn2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mBeiLv == 2)
			{
				this.LingQu_OnClick(1);
			}
			else if (this.mBeiLv == 4)
			{
				this.LingQu_OnClick(2);
			}
		};
		this.mTab.TabClick += delegate(GameObject s, int e)
		{
			this.SelectTab(e);
		};
	}

	public void LingQu_OnClick(int multi)
	{
		if (multi == 1)
		{
			long num = (long)((int)ConfigSystemParam.GetSystemParamIntByName("VIPMingXiang2Times"));
			if (this.UseGold > (long)Global.Data.roleData.Money1 + (long)Global.Data.roleData.YinLiang)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				return;
			}
		}
		else if (multi == 2)
		{
			long num2 = (long)((int)ConfigSystemParam.GetSystemParamIntByName("VIPMingXiang4Times"));
			if (this.UseZuanShi > (long)Global.Data.roleData.UserMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("MingXiangDuoBei", (int)this.UseZuanShi, false))
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			if (this.UseZuanShi > 0L)
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "MingXiangDuoBei", (int)this.UseZuanShi);
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.UseZuanShi, text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SpriteGetMeditateExpCmd(multi);
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 0
							});
						}
					}
					return true;
				};
				return;
			}
		}
		GameInstance.Game.SpriteGetMeditateExpCmd(multi);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
	}

	private new void Start()
	{
		this.SelectTab(1);
	}

	public void SelectTab(int index)
	{
		switch (index)
		{
		case 1:
			this.One();
			break;
		case 2:
			this.Two();
			break;
		case 4:
			this.Four();
			break;
		}
	}

	private void One()
	{
		this.BtnClick(this.mTab[1]);
		this.SetJiangLiValue(1, "X1");
	}

	private void Two()
	{
		this.BtnClick(this.mTab[2]);
		this.SetJiangLiValue(2, "X2");
	}

	private void Four()
	{
		this.BtnClick(this.mTab[4]);
		this.SetJiangLiValue(4, "X4");
	}

	private void BtnClick(GButton button)
	{
		if (null != this.tmpButton)
		{
			if (this.tmpButton == button)
			{
				button.Label.color = NGUIMath.HexToColorEx(16434701U);
				return;
			}
			button.normalSprite = "danxuan1";
			button.hoverSprite = "danxuan1";
			button.pressedSprite = "danxuan1";
			button.disabledSprite = "danxuan1";
			button.Pressed = true;
			button.Label.color = NGUIMath.HexToColorEx(16434701U);
			this.tmpButton.normalSprite = "danxuan2";
			this.tmpButton.hoverSprite = "danxuan2";
			this.tmpButton.pressedSprite = "danxuan2";
			this.tmpButton.disabledSprite = "danxuan2";
			this.tmpButton.Pressed = false;
			this.tmpButton.Refresh();
			this.tmpButton.Label.color = NGUIMath.HexToColorEx(12434877U);
			this.tmpButton = button;
			this.tmpButton.Refresh();
			this.tmpButton.normalSprite = "danxuan1";
			this.tmpButton.hoverSprite = "danxuan1";
			this.tmpButton.pressedSprite = "danxuan1";
			this.tmpButton.disabledSprite = "danxuan1";
		}
		else
		{
			button.Label.color = NGUIMath.HexToColorEx(16434701U);
			button.normalSprite = "danxuan1";
			button.hoverSprite = "danxuan1";
			button.pressedSprite = "danxuan1";
			button.disabledSprite = "danxuan1";
			button.Pressed = true;
			this.tmpButton = button;
			this.tmpButton.normalSprite = "danxuan1";
			this.tmpButton.hoverSprite = "danxuan1";
			this.tmpButton.pressedSprite = "danxuan1";
			this.tmpButton.disabledSprite = "danxuan1";
		}
	}

	private void SetJiangLiValue(int beiLv, string lblValue)
	{
		this.JiangLiNum = beiLv;
		this.mExpLbls[2].Text = lblValue;
		this.mHuoBiLbls[2].Text = lblValue;
		int uplv = this.GetUPLV(this.Exprence * (long)this.JiangLiNum);
		this.mLevelLbls[2].text = ((Global.Data.roleData.ChangeLifeCount <= 0) ? string.Format(Global.GetLang("{0}级"), uplv) : string.Format(Global.GetLang("{0}级[{1}转]"), uplv, Global.Data.roleData.ChangeLifeCount));
		this.mBeiLv = beiLv;
		if (beiLv >= 2)
		{
			int num;
			if (beiLv == 2)
			{
				num = (int)ConfigSystemParam.GetSystemParamIntByName("VIPMingXiang2Times");
			}
			else
			{
				num = (int)ConfigSystemParam.GetSystemParamIntByName("VIPMingXiang4Times");
			}
			this.IsShowVipLimitFlag = (Global.Data.roleData.VIPLevel < num);
			if (this.IsShowVipLimitFlag)
			{
				this.IsShowBtn(2, true);
				this.VIPLimitContent = num;
			}
			else
			{
				this.IsShowBtn(2, false);
			}
		}
		else
		{
			this.IsShowBtn(1, false);
			this.isShowVipLimitFlag = false;
			NGUITools.SetActive(this.mLblVIPFlag, this.isShowVipLimitFlag);
		}
		if (beiLv == 2 || beiLv == 4)
		{
			this.currentBeiLv = beiLv;
			this.UpdateMingXiangInfo(-1, -1, 0L);
		}
	}

	private void IsShowBtn(int flag, bool isForce = false)
	{
		if (!isForce)
		{
			NGUITools.SetActive(this.mBtn1, flag == 1);
			NGUITools.SetActive(this.mBtn2Obj, flag == 2);
		}
		else
		{
			NGUITools.SetActive(this.mBtn1, false);
			NGUITools.SetActive(this.mBtn2Obj, false);
		}
	}

	private bool IsShowVipLimitFlag
	{
		get
		{
			return this.isShowVipLimitFlag;
		}
		set
		{
			this.isShowVipLimitFlag = value;
			NGUITools.SetActive(this.mLblVIPFlag, value);
		}
	}

	private int VIPLimitContent
	{
		set
		{
			this.mLblVIPFlag.Text = "VIP" + value + Global.GetLang("才可领取");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitValue();
		this.InitClickEvent();
		double[] systemParamDoubleArrayByName = ConfigSystemParam.GetSystemParamDoubleArrayByName("MingXiangLingQu");
		if (systemParamDoubleArrayByName != null && systemParamDoubleArrayByName.Length >= 2)
		{
			this.UseGoldPerMinite = systemParamDoubleArrayByName[0];
			this.UseZuanShiPerMinite = systemParamDoubleArrayByName[1];
		}
		this._Start.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.SaveSettings();
			this.StartMingXiang();
		};
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.SaveSettings();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = 0
				});
			}
		};
	}

	private void StartMingXiang()
	{
		if (this.DPSelectedItem != null)
		{
			GameInstance.Game.SpriteStartMeditateCmd((Global.Data.MeditateState > 0) ? 0 : 1);
			this._Start.Text = ((!Global.IsMingXiang()) ? Global.GetLang("开始冥想") : Global.GetLang("停止冥想"));
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		}
		SystemHelpMgr.OnAction(UIObjIDs.MingXiangPartStart, HelpStateEvents.Clicked, 1);
	}

	public void SaveSettings()
	{
		if (Global.IsAutoFighting())
		{
			Global.Data.GameScene.CancelAutoFight(0, true);
			Global.Data.GameScene.CancelAutoFindRoad(true);
		}
	}

	private void OnTimeChanged(float percent)
	{
	}

	public void UpdateData(bool save = false)
	{
	}

	private void ChongZhiHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0 && this.DPSelectedItem != null)
		{
			if (Global.g_bIsYaoQingCeShi)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("该功能暂未开放，敬请期待。"), -1, -1, -1, -1, false);
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		}
	}

	public void LingQu(int multi)
	{
	}

	public void UpdateMingXiangInfo(int secs1 = -1, int secs2 = -1, long newexpr = 0L)
	{
		if (secs1 >= 0 || secs2 >= 0)
		{
			this.Seconeds = secs1 + secs2;
			int num;
			long mingXiangExpr = Global.GetMingXiangExpr(out num);
			this.Exprence = (long)(secs1 / 60) * mingXiangExpr;
			this.Exprence += (long)(secs2 / 60) * mingXiangExpr;
			this.XingHun = (long)(secs1 / 60 * num);
			this.XingHun += (long)(secs2 / 60 * num);
		}
		this.mExpLbls[1].Text = this.Exprence.ToString();
		this.mHuoBiLbls[1].Text = this.XingHun.ToString();
		if (this.currentBeiLv == 2)
		{
			this.mIconHuoBi.spriteName = "gold";
			this.UseGold = (long)Mathf.CeilToInt((float)(this.Seconeds / 60) * (float)this.UseGoldPerMinite);
			this.mLblCostDiamond.Text = this.UseGold.ToString();
			this.SetLabelColor(this.UseGold, 1);
		}
		else if (this.currentBeiLv == 4)
		{
			this.UseZuanShi = (long)Mathf.CeilToInt((float)(this.Seconeds / 60) * (float)this.UseZuanShiPerMinite);
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.mIconHuoBi, "MingXiangDuoBei", (int)this.UseZuanShi, string.Empty);
			this.mLblCostDiamond.Text = this.UseZuanShi.ToString();
			this.SetLabelColor(this.UseZuanShi, 2);
		}
		this.mLevelLbls[1].text = ((Global.Data.roleData.ChangeLifeCount <= 0) ? string.Format(Global.GetLang("{0}级"), Global.Data.roleData.Level) : string.Format(Global.GetLang("{0}级[{1}转]"), Global.Data.roleData.Level, Global.Data.roleData.ChangeLifeCount));
		int uplv = this.GetUPLV(this.Exprence * (long)this.JiangLiNum);
		this.mLevelLbls[2].text = ((Global.Data.roleData.ChangeLifeCount <= 0) ? string.Format(Global.GetLang("{0}级"), uplv) : string.Format(Global.GetLang("{0}级[{1}转]"), uplv, Global.Data.roleData.ChangeLifeCount));
		this._TimeScroll.Percent = (double)((float)this.Seconeds / 3600f / this.TimeMax);
		this._TimeScroll.ProgessText = UIHelper.FormatSecsShort((long)(this.Seconeds / 60 * 60), Global.GetLang("0分钟"));
		string text = UIHelper.FormatSecsShort((long)this.Seconeds, Global.GetLang("0分钟"));
		if (this.Seconeds >= 43200)
		{
			this.mMXLbls[1].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("12小时")
			});
			this.mMXLbls[2].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("12小时"),
				"17e43e",
				Global.GetLang("(已满)")
			});
		}
		else
		{
			this.mMXLbls[1].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				text
			});
			this.mMXLbls[2].Text = string.Empty;
		}
	}

	private void SetLabelColor(long huoBi, int type)
	{
		this.mLblCostDiamond.textColor = 15790320U;
		if (type == 1)
		{
			if (huoBi > (long)Global.Data.roleData.Money1 + (long)Global.Data.roleData.YinLiang)
			{
				this.mLblCostDiamond.textColor = 16711680U;
			}
		}
		else if (type == 2 && this.UseZuanShi > (long)Global.Data.roleData.UserMoney)
		{
			this.mLblCostDiamond.textColor = 16711680U;
		}
	}

	private int GetUPLV(long exp)
	{
		double num = 0.0;
		if (Global.Data.roleData.Level <= Global.Data.LevelUpExperienceList.Length - 1)
		{
			bool flag = true;
			int num2 = 0;
			double num3 = 1.0;
			if (Global.Data.roleData.ChangeLifeCount > 0)
			{
				Global.Data.LevelUpExpProportionList.TryGetValue(Global.Data.roleData.ChangeLifeCount, ref num3);
			}
			while (flag)
			{
				int num4 = Math.Min(Global.Data.roleData.Level + num2, Global.Data.LevelUpExperienceList.Length - 1);
				double num5 = (double)Global.Data.LevelUpExperienceList[num4];
				num5 *= num3;
				num += num5;
				if (num >= (double)(exp + Global.Data.roleData.Experience))
				{
					break;
				}
				if (Global.Data.roleData.Level >= 100)
				{
					break;
				}
				if (num2 >= 100)
				{
					break;
				}
				num2++;
			}
			return Math.Min(Global.Data.roleData.Level + num2, 100);
		}
		return 100;
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
		SystemHelpMgr.OnAction(UIObjIDs.MingXiangPart, HelpStateEvents.Inactived, 1);
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			this.UpdateMingXiangInfo(-1, -1, 0L);
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	public void InitPartSize()
	{
		this._Start.Text = ((!Global.IsMingXiang()) ? Global.GetLang("开始冥想") : Global.GetLang("停止冥想"));
		GameInstance.Game.SpriteGetMeditateTimeInfoCmd();
		SystemHelpMgr.OnAction(UIObjIDs.MingXiangPart, HelpStateEvents.Actived, 1);
	}

	public static void NotifyResult(int type, string message, params string[] param0)
	{
		switch (type)
		{
		case 0:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您被【{0}】击杀，请立刻回到游戏中复仇！"), new object[]
			{
				message
			}), 0, -1, -1, 0);
			break;
		case 1:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("离线挂机时间已达到最大，请您尽快领取收益！"), new object[0]), 0, -1, -1, 0);
			break;
		case 2:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("离线挂机经验已达到最大，请您尽快领取收益！"), new object[0]), 0, -1, -1, 0);
			break;
		case 3:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您的背包已满，为防止您损失收益，请您尽快处理！"), new object[0]), 0, -1, -1, 0);
			break;
		case 4:
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("使用离线挂机，可让您在离线时继续获得收益！"), new object[0]), 0, -1, -1, 0);
			break;
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this._Start, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this._Close, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public void RefreshGoodsData(List<GoodsData> gdlst)
	{
		this.goodsDataList = gdlst;
		this.ShowGoodIcon();
	}

	private void ShowGoodIcon()
	{
		if (this.goodsDataList == null || this.goodsDataList.Count <= 0)
		{
			return;
		}
		int num = (this.mAwardList.Count() + 10 <= this.goodsDataList.Count) ? (this.mAwardList.Count() + 10) : this.goodsDataList.Count;
		for (int i = this.mAwardList.Count(); i < num; i++)
		{
			if (i <= num - 1)
			{
				this.AddGoodIcon(this.goodsDataList[i]);
			}
		}
	}

	private void AddGoodIcon(GoodsData gd)
	{
		GGoodIcon icon = UIHelper.AddGoodsIcon(this.ItemCollection, gd, null, false, "bagGrid4_bak");
		if (gd.GoodsID <= 0)
		{
			icon.MouseLeftButtonUp = null;
		}
		UIDragPanelContents component = icon.transform.GetComponent<UIDragPanelContents>();
		if (component == null)
		{
			icon.gameObject.AddComponent<UIDragPanelContents>();
		}
		icon.MouseLeftButtonUp = null;
		icon.addEventListener("click", delegate(MouseEvent e)
		{
			GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
			if (null == ggoodIcon)
			{
				return;
			}
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		});
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	private const int countPerPage = 10;

	private LocalAutoFightData CopyAutoFightData;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton _Start;

	private int Multi;

	public GButton _LingQu1;

	public GButton _LingQu2;

	public GButton _LingQu4;

	public ListBox _Awardslist;

	public ListBox _NeedList;

	public TextBlock _Desc2;

	public TextBlock _MingXiangShouYi;

	public TextBlock _MingXiangTime;

	public GImgProgressBar _TimeScroll;

	public TextBlock _ExprScroll;

	public TextBlock _GoldUsed;

	private float TimeMax = 12f;

	private long UseGold;

	private long UseZuanShi;

	public TextBlock _Desc1;

	public GButton _Close;

	public TextBlock[] mMXLbls;

	public TextBlock[] mLevelLbls;

	public TextBlock[] mExpLbls;

	public TextBlock[] mHuoBiLbls;

	public TextBlock mLblDaoJuTitle;

	public ListBox mAwardList;

	private ObservableCollection _ItemCollection;

	public GButton mBtn1;

	public GameObject mBtn2Obj;

	public GButton mBtn2;

	public TextBlock mLblCostDiamond;

	public TextBlock mLblVIPFlag;

	public UITab mTab;

	private int mBeiLv = -1;

	public UISprite mIconHuoBi;

	public UIDraggablePanel dragPabel;

	private GChildWindow messageBoxWindow;

	public TextBlock staticText;

	public GButton checkBox1;

	public GButton checkBox2;

	public GButton checkBox4;

	private int currentBeiLv = -1;

	private GButton tmpButton;

	private bool isShowVipLimitFlag;

	private double UseGoldPerMinite;

	private double UseZuanShiPerMinite;

	private int Seconeds;

	private long Exprence;

	private long XingHun;

	private int JiangLiNum = 1;

	private float TickInterval = 30f;

	private List<GoodsData> goodsDataList;
}
