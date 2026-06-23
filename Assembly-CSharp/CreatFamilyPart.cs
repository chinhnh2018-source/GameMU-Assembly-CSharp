using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class CreatFamilyPart : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnCreate.Text = Global.GetLang("创建战盟");
		this.btnGoback.Text = Global.GetLang("其他战盟");
		this.cbAutoToLeague.Text = Global.GetLang("自动接受加入战盟邀请");
		this.inputLeagueName.defaultText = Global.GetLang("点击输入");
		this.lblNeedEquip.transform.localPosition = new Vector3(280f, 43f, 0f);
		this.lblNeedLevel.transform.localPosition = new Vector3(280f, 103f, 0f);
		this.lblNeedMoney.transform.localPosition = new Vector3(125f, 133f, 0f);
		this.inputLeagueName.text = Global.GetLang("点击输入");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.thisCtrl = this;
		string text = "{FFFFFF}";
		if (Global.Data.roleData.Money1 < Global.CreateBangHuiNeedTongQian && Global.Data.roleData.YinLiang < Global.CreateBangHuiNeedTongQian - Global.Data.roleData.Money1)
		{
			text = "{FF0000}";
			this.MoneyOk = false;
		}
		this.lblNeedMoney.text = string.Concat(new object[]
		{
			text,
			Global.CreateBangHuiNeedTongQian / 10000,
			Global.GetLang("万"),
			"{-}"
		});
		text = "{FFFFFF}";
		int createBangHuiNeedDaoju = Global.CreateBangHuiNeedDaoju;
		if (createBangHuiNeedDaoju != -1 && Global.GetTotalGoodsCountByID(createBangHuiNeedDaoju) <= 0)
		{
			text = "{FF0000}";
			this.GoodsOk = false;
		}
		this.lblNeedEquip.text = string.Concat(new string[]
		{
			text,
			Global.GetLang("【"),
			Global.GetGoodsNameByID(Global.CreateBangHuiNeedDaoju, false),
			Global.GetLang("】"),
			"{-}"
		});
		text = "{FFFFFF}";
		int createBangHuiZhuansheng = Global.CreateBangHuiZhuansheng;
		if (Global.Data.roleData.ChangeLifeCount < createBangHuiZhuansheng)
		{
			text = "{FF0000}";
			this.LevelOk = false;
		}
		else if (Global.Data.roleData.ChangeLifeCount == createBangHuiZhuansheng && Global.Data.roleData.Level < Global.CreateBangHuiNeedLevel)
		{
			text = "{FF0000}";
			this.LevelOk = false;
		}
		this.lblNeedLevel.text = string.Concat(new object[]
		{
			text,
			Global.GetLang("【"),
			Global.CreateBangHuiZhuansheng,
			Global.GetLang("转"),
			Global.GetLang("级"),
			Global.CreateBangHuiNeedLevel,
			Global.GetLang("】"),
			"{-}"
		});
	}

	public void InitPartSize(int width, int height)
	{
		this.btnCreate.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.inputLeagueName.text = this.inputLeagueName.text.Trim();
			this.inputLeagueName.text = Global.StringReplaceAll(this.inputLeagueName.text, "'", string.Empty);
			this.inputLeagueName.text = Global.StringReplaceAll(this.inputLeagueName.text, "|", string.Empty);
			this.inputLeagueName.text = Global.StringReplaceAll(this.inputLeagueName.text, "$", string.Empty);
			this.inputLeagueName.text = Global.StringReplaceAll(this.inputLeagueName.text, ":", string.Empty);
			this.inputLeagueName.text = Global.StringReplaceAll(this.inputLeagueName.text, Global.GetLang("："), string.Empty);
			if (!Global.IsKoreanOrEn(this.inputLeagueName.text))
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			if (Global.IncludeReplaceFilterFileds(this.inputLeagueName.text))
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("战盟名称当中含有敏感词汇，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			if (Global.IsHavingBangHui())
			{
				Super.HintMainText(Global.GetLang("你已经在战盟中，无法再创建新的战盟!"), 10, 3);
				return;
			}
			if (this.inputLeagueName.text == Global.GetLang("战盟名称七个字"))
			{
				Super.HintMainText(Global.GetLang("请输入要创建的战盟的名称!"), 10, 3);
				return;
			}
			byte b = Global.CheckRoleNameLenght(this.inputLeagueName.text);
			if (Global.NameLengthRange[0] == b)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟名称不能少于") + b + Global.GetLang("个汉字，请重新输入!"), 10, 3);
				return;
			}
			if (Global.NameLengthRange[1] == b)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟名称已超过") + b + Global.GetLang("个汉字，请重新输入！"), 10, 3);
				return;
			}
			if (this.inputLeagueName.text.Length > 100)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
				return;
			}
			if (!this.MoneyOk)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				return;
			}
			if (!this.GoodsOk)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟需要一个【{0}】，你的背包中没有该物品，无法创建!"), new object[]
				{
					Global.GetGoodsNameByID(Global.CreateBangHuiNeedDaoju, false)
				}), 10, 3);
				return;
			}
			if (!this.LevelOk)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟需要角色等级达到【{0}】"), new object[]
				{
					Global.CreateBangHuiNeedLevel
				}), 10, 3);
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.inputLeagueName.text, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 10, 3);
					return;
				}
				if (result.is_dirty > 0)
				{
					Super.HintMainText(Global.GetLang("战盟名称和战盟公告不能包含国家规定禁止的词汇!"), 10, 3);
					return;
				}
				Global.SendEvent("700", Global.GetLang("创建战盟次数"));
				GameInstance.Game.SpriteCreateBangHui(string.Format("{0}${1}", Global.StringTrim(this.inputLeagueName.text), PlatSDKMgr.PlatName), string.Empty);
			});
		};
		this.btnGoback.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CreateFamilyListPart();
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.cbAutoToLeague.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			Global.Data.roleData.BHVerify = ((!this.cbAutoToLeague.Check) ? 1 : 0);
			GameInstance.Game.SpriteBeInvitedByBHVerify(Global.Data.roleData.BHVerify);
		};
		this.cbAutoToLeague.Check = (Global.Data.roleData.BHVerify <= 0);
		UIEventListener.Get(this.inputLeagueName.gameObject).onClick = delegate(GameObject s)
		{
			if (this.inputLeagueName.text == Global.GetLang("点击输入"))
			{
				this.inputLeagueName.text = string.Empty;
			}
		};
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	public void CleanUpLoadingWindow()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	public void InitPartData()
	{
	}

	private void ShowFamilyListWindow()
	{
		if (null != this.FamilyListPartWindow)
		{
			this.CloseFamilyListWindow();
			return;
		}
		this.FamilyListPartWindow = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow1(this.FamilyListPartWindow, Global.GetLang("创建列表"));
		this.Container.Children.Add(this.FamilyListPartWindow);
		this.familyListPart = U3DUtils.NEW<FamilyListPart>();
		this.familyListPart.InitPartSize((int)this.FamilyListPartWindow.BodyWidth - 18, (int)this.FamilyListPartWindow.BodyHeight - 9);
		this.familyListPart.InitPartData();
		this.FamilyListPartWindow.SetContent(this.FamilyListPartWindow.BodyPresenter, this.familyListPart, 9.0, 0.0, true);
		this.familyListPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseFamilyListWindow();
		};
		GameInstance.Game.GetLuoLanChengZhuRoleInfo();
	}

	public void RefreshFamilyList(int RoleID)
	{
		this.familyListPart.luolanChengZhuID = RoleID;
		this.familyListPart.GetNewData();
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseFamilyListWindow()
	{
		if (null != this.FamilyListPartWindow)
		{
			this.CloseModalDialog();
			this.familyListPart.CleanUpChildWindows();
			this.familyListPart = null;
			Super.CloseChildWindow(this.Container, this.FamilyListPartWindow);
			this.FamilyListPartWindow = null;
		}
	}

	private void CreateFamilyListPart()
	{
		this.familyListPart = U3DUtils.NEW<FamilyListPart>();
		this.familyListPart.transform.parent = base.transform;
		this.familyListPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.familyListPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.familyListPart.InitPartSize(0, 0);
		GameInstance.Game.GetLuoLanChengZhuRoleInfo();
		this.familyListPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.DestoryFamilyListPart();
		};
		this.familyListPart.ShowChatBoxCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			PlayZone.GlobalPlayZone.ShowChatBoxInFamilyList(4, e);
		};
	}

	private void DestoryFamilyListPart()
	{
		if (this.familyListPart != null)
		{
			this.familyListPart.transform.parent = null;
			Object.Destroy(this.familyListPart.gameObject);
			this.familyListPart = null;
		}
	}

	private void ShowFamilyListPart()
	{
		if (this.familyListPart == null)
		{
			this.CreateFamilyListPart();
		}
		this.familyListPart.gameObject.SetActive(true);
	}

	public void NotifyBangHuiListData(BangHuiListData bangHuiListData)
	{
		if (null != this.familyListPart)
		{
			this.familyListPart.NotifyBangHuiListData(bangHuiListData);
		}
	}

	public void NotifyCreateBangHuiResult(int retCode, int roleID, int bangHuiID)
	{
		if (retCode < 0)
		{
			if (retCode == -2)
			{
				Super.HintMainText(Global.GetLang("创建战盟失败, 服务器暂时禁止创建战盟,请稍候再试"), 10, 3);
			}
			else if (retCode == -3)
			{
				Super.HintMainText(Global.GetLang("创建战盟失败, 名字包含特殊字符, 请换个名称后创建"), 10, 3);
			}
			else if (retCode == -4)
			{
				Super.HintMainText(Global.GetLang("创建战盟失败, 名称已经存在, 请换个名称后创建"), 10, 3);
			}
			else if (retCode == -1011 || retCode == -1031)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("要创建的战盟名称已经存在，请输入其他名称重新创建"), new object[0]), 10, 3);
			}
			else if (retCode == -1010)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("创建失败，已经是战盟成员"), new object[0]), 10, 3);
			}
			else if (retCode == -10)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("创建战盟时发生未知错误: {0}"), new object[]
				{
					retCode
				}), 10, 3);
			}
			return;
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 1
			});
		}
	}

	public void NotifyJoinBangHuiSuccess(int retCode)
	{
		if (retCode < 0)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("加入战盟时发生未知错误: {0}"), new object[]
			{
				retCode
			}), 10, 3);
			return;
		}
		this.DestoryFamilyListPart();
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
	}

	public GButton btnCreate;

	public GButton btnGoback;

	public GButton btnClose;

	public UIInput inputLeagueName;

	public UILabel lblNeedMoney;

	public UILabel lblNeedEquip;

	public UILabel lblNeedLevel;

	private Canvas PlaceHolder;

	private GChildWindow FamilyListPartWindow;

	private FamilyListPart familyListPart;

	public DPSelectedItemEventHandler DPSelectedItem;

	private Canvas Root;

	public GCheckBox cbAutoToLeague;

	private LoadingWindow LoadingWin;

	private SpriteSL thisCtrl;

	private bool MoneyOk = true;

	private bool GoodsOk = true;

	private bool LevelOk = true;
}
