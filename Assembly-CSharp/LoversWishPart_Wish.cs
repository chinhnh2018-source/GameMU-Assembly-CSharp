using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class LoversWishPart_Wish : UserControl
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
		this.currentpaiming.pivot = 3;
		this.currentpaiming.transform.localPosition = new Vector3(160f, -236f, -1f);
		this.currentzhufuzhi.transform.localPosition = new Vector3(330f, -236f, -1f);
		this.HuaBian.transform.localPosition = new Vector3(-212f, 75f, -1f);
		this.zhufuzhi.transform.localPosition = new Vector3(300f, -236f, -1f);
		this.paihang.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("排行")
		});
		this.role.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("角色")
		});
		this.zhufu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("获得祝福")
		});
		this.paiming.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("当前排名:")
		});
		this.zhufuzhi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("祝福值：")
		});
		this.BtnZhuFu.Label.text = Global.GetLang("祝福他们");
		this.leftBackGround.URL = "NetImages/GameRes/Images/Wish/XinXing.jpg.qj";
		this.HuaBian.URL = "NetImages/GameRes/Images/Wish/MeiGui.png.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.ListItem.ItemsSource;
		this.ListItem.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this.InitListItem();
		this.BtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenRuleInterFace();
		};
		this.BtnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAwardInterFace();
		};
		this.BtnZhuFu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ManName != null && this.WomanName != null)
			{
				PlayZone.GlobalPlayZone.OpenLoversWishPartSendWishWindow(true, this.ManName, this.WomanName, this.DbCoupleId);
			}
			else
			{
				string lang = Global.GetLang("请选择祝福对象");
				Super.HintMainText(lang, 10, 3);
			}
		};
		GameInstance.Game.GetMainDataForCoupleWish();
		Super.ShowNetWaiting(null);
	}

	public void InitTab(CoupleWishMainData Data)
	{
		if (Data.RankList != null)
		{
			this.InitListItem(Data.RankList);
		}
		this.InitAttr(Data.MyCoupleRank, Data.MyCoupleBeWishNum);
	}

	private void CloseXuanZhong()
	{
		if (this.ItemCollection == null)
		{
			return;
		}
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			LoversWishPart_WishItem loversWishPart_WishItem = U3DUtils.AS<LoversWishPart_WishItem>(this.ItemCollection[i]);
			if (loversWishPart_WishItem != null)
			{
				loversWishPart_WishItem.IsShow = false;
			}
		}
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		this.CloseXuanZhong();
		LoversWishPart_WishItem loversWishPart_WishItem = U3DUtils.AS<LoversWishPart_WishItem>(this.ListItem.SelectedItem);
		loversWishPart_WishItem.IsShow = true;
		if (null == loversWishPart_WishItem)
		{
			return;
		}
		if (loversWishPart_WishItem.data == null)
		{
			return;
		}
		this.LoadRoleRes(loversWishPart_WishItem.data);
		this.ManName = Global.FormatRoleNameZoneid(loversWishPart_WishItem.data.Man.ZoneId, loversWishPart_WishItem.data.Man.RoleName, 1, 1);
		this.WomanName = Global.FormatRoleNameZoneid(loversWishPart_WishItem.data.Wife.ZoneId, loversWishPart_WishItem.data.Wife.RoleName, 1, 1);
		this.DbCoupleId = loversWishPart_WishItem.data.DbCoupleId;
	}

	public override void Destroy()
	{
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
		}
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
		}
		base.Destroy();
	}

	private void LoadRoleRes(CoupleWishCoupleData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.WifeSelector != null)
		{
			if (this.rightResLoader != null)
			{
				this.rightResLoader.Stop();
			}
			this.rightResLoader = UIHelper.LoadRoleRes(this.RightRoleModel, data.WifeSelector.SettingBitFlags, data.WifeSelector.Occupation, data.WifeSelector.SubOccupation, data.WifeSelector.RoleName, data.WifeSelector.GoodsDataList, null, data.WifeSelector.MyWingData, 1f, 0, null, false);
		}
		if (data.ManSelector != null)
		{
			if (this.leftResLoader != null)
			{
				this.leftResLoader.Stop();
			}
			this.leftResLoader = UIHelper.LoadRoleRes(this.LeftRoleModel, data.ManSelector.SettingBitFlags, data.ManSelector.Occupation, data.ManSelector.SubOccupation, data.ManSelector.RoleName, data.ManSelector.GoodsDataList, null, data.ManSelector.MyWingData, 1f, 0, null, false);
		}
	}

	private void InitAttr(int MyCoupleRank, int MyCoupleBeWishNum)
	{
		this.currentpaiming.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			(MyCoupleRank != 0) ? MyCoupleRank.ToString() : Global.GetLang("未上榜")
		});
		this.currentzhufuzhi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			MyCoupleBeWishNum
		});
	}

	private void InitListItem()
	{
		for (int i = 1; i < 21; i++)
		{
			LoversWishPart_WishItem loversWishPart_WishItem = U3DUtils.NEW<LoversWishPart_WishItem>();
			loversWishPart_WishItem.NoPeople.SetActive(true);
			loversWishPart_WishItem.Attr.SetActive(false);
			loversWishPart_WishItem.SetMiaoShu = ((i <= 10) ? string.Format(Global.GetLang("无人上榜，最低收到{0}祝福"), LoversWishPart.GetWishAwardDic()[i].MinWishNum) : Global.GetLang("无人上榜"));
			loversWishPart_WishItem.PaiMingNum = i;
			loversWishPart_WishItem.IsShow = false;
			this.ItemCollection.AddNoUpdate(loversWishPart_WishItem);
			UIPanel component = loversWishPart_WishItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void InitListItem(List<CoupleWishCoupleData> RankList)
	{
		for (int i = 0; i < RankList.Count; i++)
		{
			LoversWishPart_WishItem loversWishPart_WishItem = U3DUtils.AS<LoversWishPart_WishItem>(this.ListItem[RankList[i].Rank - 1]);
			loversWishPart_WishItem.NoPeople.SetActive(false);
			loversWishPart_WishItem.Attr.SetActive(true);
			loversWishPart_WishItem.SetManName = Global.FormatRoleNameZoneid(RankList[i].Man.ZoneId, RankList[i].Man.RoleName, 1, 1);
			loversWishPart_WishItem.SetWoManName = Global.FormatRoleNameZoneid(RankList[i].Wife.ZoneId, RankList[i].Wife.RoleName, 1, 1);
			loversWishPart_WishItem.PaiMingNum = RankList[i].Rank;
			loversWishPart_WishItem.data = RankList[i];
			loversWishPart_WishItem.ZhuFuNum = RankList[i].BeWishedNum;
			loversWishPart_WishItem.IsShow = false;
		}
		LoversWishPart_WishItem loversWishPart_WishItem2 = U3DUtils.AS<LoversWishPart_WishItem>(this.ListItem[0]);
		if (null == loversWishPart_WishItem2)
		{
			return;
		}
		loversWishPart_WishItem2.IsShow = true;
		if (loversWishPart_WishItem2.data != null)
		{
			this.LoadRoleRes(loversWishPart_WishItem2.data);
			this.ManName = Global.FormatRoleNameZoneid(loversWishPart_WishItem2.data.Man.ZoneId, loversWishPart_WishItem2.data.Man.RoleName, 1, 1);
			this.WomanName = Global.FormatRoleNameZoneid(loversWishPart_WishItem2.data.Wife.ZoneId, loversWishPart_WishItem2.data.Wife.RoleName, 1, 1);
			this.DbCoupleId = loversWishPart_WishItem2.data.DbCoupleId;
		}
	}

	private void OpenRuleInterFace()
	{
		if (this.LoversWishRuleWindow == null)
		{
			this.LoversWishRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.LoversWishRuleWindow.IsShowModal = true;
			this.LoversWishRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.LoversWishRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.LoversWishRuleWindow);
		}
		if (this.LoversWishRule == null)
		{
			this.LoversWishRule = U3DUtils.NEW<LoversWishPartRule>();
			this.LoversWishRule.CloseHandle = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRuleInterFace();
			};
		}
		this.LoversWishRuleWindow.SetContent(this.LoversWishRuleWindow.BodyPresenter, this.LoversWishRule, 0.0, 0.0, true);
	}

	private void CloseRuleInterFace()
	{
		if (null != this.LoversWishRule)
		{
			this.LoversWishRule.transform.parent = null;
			Object.Destroy(this.LoversWishRule.gameObject);
			this.LoversWishRule = null;
		}
		if (null != this.LoversWishRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.LoversWishRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.LoversWishRuleWindow, true);
			this.LoversWishRuleWindow = null;
		}
	}

	private void OpenAwardInterFace()
	{
		if (this.LoversWishAwardWindow == null)
		{
			this.LoversWishAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.LoversWishAwardWindow.IsShowModal = true;
			this.LoversWishAwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.LoversWishAwardWindow, Global.GetLang("奖励界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.LoversWishAwardWindow);
		}
		if (this.LoversWishAward == null)
		{
			this.LoversWishAward = U3DUtils.NEW<LoversWishPartAward>();
			this.LoversWishAward.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAwardInterFace();
			};
		}
		this.LoversWishAwardWindow.SetContent(this.LoversWishAwardWindow.BodyPresenter, this.LoversWishAward, 0.0, 0.0, true);
	}

	private void CloseAwardInterFace()
	{
		if (this.LoversWishAward)
		{
			this.LoversWishAward.transform.parent = null;
			Object.Destroy(this.LoversWishAward.gameObject);
			this.LoversWishAward = null;
		}
		if (this.LoversWishAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.LoversWishAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.LoversWishAwardWindow, true);
			this.LoversWishAwardWindow = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton BtnHelp;

	public GButton BtnAward;

	public GButton BtnZhuFu;

	public ListBox ListItem;

	public ShowNetImage leftBackGround;

	public ShowNetImage HuaBian;

	public UILabel paihang;

	public UILabel role;

	public UILabel zhufu;

	public UILabel paiming;

	public UILabel currentpaiming;

	public UILabel zhufuzhi;

	public UILabel currentzhufuzhi;

	public Modal3DShow LeftRoleModel;

	public Modal3DShow RightRoleModel;

	private int DbCoupleId;

	private string ManName;

	private string WomanName;

	private GChildWindow LoversWishRuleWindow;

	private LoversWishPartRule LoversWishRule;

	private GChildWindow LoversWishAwardWindow;

	private LoversWishPartAward LoversWishAward;

	private ObservableCollection _ItemCollection;

	private RoleResLoader rightResLoader;

	private RoleResLoader leftResLoader;
}
