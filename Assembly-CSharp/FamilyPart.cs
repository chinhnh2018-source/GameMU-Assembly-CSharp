using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class FamilyPart : UserControl
{
	public int TodayZhangongForGold
	{
		get
		{
			return this._todayZhangongForGold;
		}
		set
		{
			this._todayZhangongForGold = value;
		}
	}

	public int TodayZhangongForDiamond
	{
		get
		{
			return this._todayZhangongForDiamond;
		}
		set
		{
			this._todayZhangongForDiamond = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.btnMemberInfo.Text = Global.GetLang("成员信息");
		this.btnDonate.Text = Global.GetLang("战盟捐赠");
		this.btnBuild.Text = Global.GetLang("战盟建设");
		this.btnLeagueEvent.Text = Global.GetLang("战盟事件");
		this.btnLeagueList.Text = Global.GetLang("战盟列表");
		this.btnQuite.Text = Global.GetLang("退出战盟");
		this.btnRename.Text = Global.GetLang("战盟改名");
		this.btnLeagueActivity.Text = Global.GetLang("战盟活动");
		this.btnLeagueShop.Text = Global.GetLang("战功商店");
		this.btnZhanMengShenDian.Text = Global.GetLang("战盟神殿");
		this.btnZhanMengWaiJiao.Text = Global.GetLang("战盟外交");
		this.btnHongBao.Text = Global.GetLang("战盟红包");
		this.btnSubmit.Text = Global.GetLang("确定");
		this.inputNotice.text = string.Empty;
		this.inputNotice.defaultText = Global.GetLang("点击输入");
		this.btnRename.gameObject.SetActive(false);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Root = this.Container;
		this.thisCtrl = this;
		ActivityTipManager.RegActivityTipItem(15001, delegate(int s, ActivityTipItem e)
		{
			this.isShowZhanMengBossTip = e.IsActive;
			if (this.isShowLangHunTip)
			{
				this.TipIconFamily.gameObject.SetActive(true);
			}
			else
			{
				this.TipIconFamily.gameObject.SetActive(this.isShowZhanMengBossTip);
			}
		});
		ActivityTipManager.RegActivityTipItem(15002, delegate(int s, ActivityTipItem e)
		{
			this.isShowLangHunTip = e.IsActive;
			if (this.isShowZhanMengBossTip)
			{
				this.TipIconFamily.gameObject.SetActive(true);
			}
			else
			{
				this.TipIconFamily.gameObject.SetActive(this.isShowLangHunTip);
			}
		});
		ActivityTipManager.RegActivityTipItem(15004, delegate(int s, ActivityTipItem e)
		{
			this.isShowLangHunTip = e.IsActive;
			if (this.isShowZhanMengBossTip)
			{
				this.TipIconFamily.gameObject.SetActive(true);
			}
			else
			{
				this.TipIconFamily.gameObject.SetActive(this.isShowLangHunTip);
			}
		});
	}

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

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.btnLeagueList.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnLeagueList))
			{
				return;
			}
			this.CreateFamilyListPart();
		};
		this.btnLeagueEvent.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnLeagueEvent))
			{
				return;
			}
			this.CreateLeagueEventPart();
		};
		this.btnMemberInfo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnMemberInfo))
			{
				return;
			}
			if (this.MyBangHuiDetailData == null)
			{
				return;
			}
			if (this.MyBangHuiDetailData.BHID != Global.Data.roleData.Faction)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法查看【{0}】的成员列表!"), new object[]
				{
					Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
				}), 10, 3);
				return;
			}
			this.ShowMembersInfoWindow();
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -10
				});
			}
		};
		this.btnShowEdit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData.RoleID, this.MyBangHuiDetailData) <= 0)
			{
				Super.HintMainText(Global.GetLang("普通成员无权修改战盟公告!"), 10, 3);
				return;
			}
			if (this.MyBangHuiDetailData.BHBulletin == null || this.MyBangHuiDetailData.BHBulletin.Length == 0)
			{
				this.keyboard = TouchScreenKeyboard.Open(string.Empty, 1);
			}
			else
			{
				this.keyboard = TouchScreenKeyboard.Open(this.lblNotice.text, 1);
			}
			this.isNoticeShow = false;
		};
		this.btnSubmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IncludeReplaceFilterFileds(this.inputNotice.text))
			{
				Super.HintMainText(Global.GetLang("您输入的内容含有敏感词汇，请重新输入!"), 10, 3);
				return;
			}
			if (this.inputNotice.text.Length > 100)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
				this.inputNotice.selected = true;
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.inputNotice.text, delegate(object content, ExecWordsFilterEventArgs result)
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
					Super.HintMainText(Global.GetLang("战盟公告不能包含国家规定禁止的词汇!"), 10, 3);
					this.inputNotice.selected = true;
					return;
				}
				this.inputNotice.text = result.msg;
				this.inputNotice.text = Global.StringReplaceAll(this.inputNotice.text, "'", string.Empty);
				this.inputNotice.text = Global.StringReplaceAll(this.inputNotice.text, "|", string.Empty);
				this.inputNotice.text = Global.StringReplaceAll(this.inputNotice.text, "$", string.Empty);
				this.inputNotice.text = Global.StringReplaceAll(this.inputNotice.text, ":", string.Empty);
				this.inputNotice.text = Global.StringReplaceAll(this.inputNotice.text, Global.GetLang("："), string.Empty);
				GameInstance.Game.SpriteUpdateBangHuiBulletinMsg(this.MyBangHuiDetailData.BHID, this.inputNotice.text);
				this.MyBangHuiDetailData.BHBulletin = this.inputNotice.text;
				this.lblNotice.text = this.inputNotice.text;
				this.lblNotice.color = Color.white;
				this.NoticeEditWin.SetActive(false);
			});
		};
		this.btnCloseEditWin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NoticeEditWin.SetActive(false);
		};
		this.btnDonate.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnDonate))
			{
				return;
			}
			this.CreateFamilyDonatePart();
		};
		this.btnBuild.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnBuild))
			{
				return;
			}
			this.CreateFamilyBuildPart();
		};
		this.btnQuite.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.Faction == this.MyBangHuiDetailData.BHID)
			{
				if (Global.IsBangHuiLeader(Global.Data.roleData, this.MyBangHuiDetailData))
				{
					Super.HintMainText(Global.GetLang("首领无法退出战盟!"), 10, 3);
					return;
				}
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				string message = StringUtil.substitute(Global.GetLang("您确认要退出【{0}】战盟吗？"), new object[]
				{
					this.MyBangHuiDetailData.BHName
				});
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, new DPSelectedItemEventHandler(this.DPSelectItemHandlerQuitBH), buttons);
			}
		};
		this.btnRename.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			FamilyGaiMingPart familyGaiMingPart = U3DUtils.NEW<FamilyGaiMingPart>();
			familyGaiMingPart.transform.parent = base.gameObject.transform;
			familyGaiMingPart.transform.localPosition = new Vector3(0f, 0f, -50f);
			familyGaiMingPart.transform.localScale = new Vector3(1f, 1f, 1f);
		};
		this.btnLeagueActivity.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnLeagueActivity))
			{
				return;
			}
			this.ShowActivityWindow();
		};
		this.btnLeagueShop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 10
				});
			}
		};
		this.btnZhanMengShenDian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 11
				});
			}
		};
		this.btnZhanMengWaiJiao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsChildWindowOpenEnable(this.btnZhanMengWaiJiao))
			{
				return;
			}
			this.CreateWaiJiaoPart();
		};
		this.btnHongBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHongBao();
		};
	}

	public void InitPartData()
	{
	}

	private void ShowMasterList()
	{
		this.ItemCollection.Clear();
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		if (this.MyBangHuiDetailData.MgrItemList == null)
		{
			return;
		}
		this.MyBangHuiDetailData.MgrItemList.Sort(new Comparison<BangHuiMgrItemData>(this.MyBangHuiDetailData_MgrItemList_Sort));
		for (int i = 0; i < this.MyBangHuiDetailData.MgrItemList.Count; i++)
		{
			FamilysManagersListItem familysManagersListItem = U3DUtils.NEW<FamilysManagersListItem>();
			familysManagersListItem.Width = 353.0;
			familysManagersListItem.Height = 19.0;
			familysManagersListItem.RoleID = this.MyBangHuiDetailData.MgrItemList[i].RoleID;
			familysManagersListItem.RoleName = Global.FormatRoleName(this.MyBangHuiDetailData.MgrItemList[i].ZoneID, this.MyBangHuiDetailData.MgrItemList[i].RoleName);
			familysManagersListItem.BHZhiWu = this.MyBangHuiDetailData.MgrItemList[i].BHZhiwu;
			familysManagersListItem.Level = this.MyBangHuiDetailData.MgrItemList[i].Level.ToString();
			familysManagersListItem.Occ = Global.GetOccupationStr(this.MyBangHuiDetailData.MgrItemList[i].Occupation);
			familysManagersListItem.Zw = Global.GetBHZhiWu(this.MyBangHuiDetailData.MgrItemList[i].BHZhiwu);
			familysManagersListItem.NickName = this.MyBangHuiDetailData.MgrItemList[i].ChengHao;
			familysManagersListItem.Bg = this.MyBangHuiDetailData.MgrItemList[i].BangGong.ToString();
			this.ItemCollection.AddNoUpdate(familysManagersListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	public int MyBangHuiDetailData_MgrItemList_Sort(BangHuiMgrItemData a, BangHuiMgrItemData b)
	{
		return a.BHZhiwu - b.BHZhiwu;
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (this.listBox.SelectedIndex < 0)
		{
			this.UnSelectItem();
			return;
		}
		if (null != this.SelectedListItem)
		{
			this.SelectedListItem.BodyBackground = null;
		}
		this.SelectedListItem = U3DUtils.AS<FamilysManagersListItem>(this.listBox.SelectedItem);
		if (null == this.SelectedListItem)
		{
			this.UnSelectItem();
			return;
		}
		this.SelectedListItem.BodyBackground = this.SelectedListItemBakImg;
		this.SelectedListItem.BodyWidth = 349.0;
		this.SelectedListItem.BodyHeight = 19.0;
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void CloseJgssWindow()
	{
		this.CloseModalDialog();
		this.CloseChildWindow(this.JgssWindow);
		this.JgssWindow = null;
		this.jgssPart.CleanUpChildWindows();
		this.jgssPart = null;
	}

	private void ShowJgssWindow()
	{
		if (null != this.JgssWindow)
		{
			this.CloseJgssWindow();
			return;
		}
		this.ShowModalDialog();
		this.JgssWindow = U3DUtils.NEW<GChildWindow>();
		this.JgssWindow.Left = (double)Super.GetChildLeft(630, 308);
		this.JgssWindow.Top = (double)Super.GetChildTop(398, 382);
		this.JgssWindow.HeadLeft = 0.0;
		this.JgssWindow.HeadTop = 0.0;
		this.JgssWindow.HeadWidth = 308.0;
		this.JgssWindow.HeadHeight = 46.0;
		this.JgssWindow.BodyLeft = 0.0;
		this.JgssWindow.BodyTop = 46.0;
		this.JgssWindow.BodyWidth = 308.0;
		this.JgssWindow.BodyHeight = 336.0;
		this.InitChildWindow1(this.JgssWindow, Global.GetLang("军贡税收"));
		this.JgssWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseJgssWindow();
			return true;
		};
		Canvas.SetZIndex(this.JgssWindow, 9001.0);
		this.Root.Children.Add(this.JgssWindow);
		this.jgssPart = U3DUtils.NEW<JgssPart>();
		this.jgssPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_slss_bak.png"), false, 0);
		this.jgssPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseJgssWindow();
		};
		this.jgssPart.InitPartSize((int)this.JgssWindow.BodyWidth - 18, (int)this.JgssWindow.BodyHeight - 9);
		this.jgssPart.InitPartData(this.MyBangHuiDetailData);
		this.JgssWindow.SetContent(this.JgssWindow.BodyPresenter, this.jgssPart, 9.0, 0.0, true);
	}

	private void ChangeInfo()
	{
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		if (null != this.ChangeInfoWindow)
		{
			this.CloseChildWindow(this.ChangeInfoWindow);
			this.ChangeInfoWindow = null;
			return;
		}
		this.ShowModalDialog();
		this.ChangeInfoWindow = U3DUtils.NEW<GChildWindow>();
		this.ChangeInfoWindow.Left = (double)Super.GetChildLeft(630, 308);
		this.ChangeInfoWindow.Top = (double)Super.GetChildTop(398, 266);
		this.ChangeInfoWindow.HeadLeft = 0.0;
		this.ChangeInfoWindow.HeadTop = 0.0;
		this.ChangeInfoWindow.HeadWidth = 308.0;
		this.ChangeInfoWindow.HeadHeight = 46.0;
		this.ChangeInfoWindow.BodyLeft = 0.0;
		this.ChangeInfoWindow.BodyTop = 46.0;
		this.ChangeInfoWindow.BodyWidth = 308.0;
		this.ChangeInfoWindow.BodyHeight = 220.0;
		this.InitChildWindow1(this.ChangeInfoWindow, Global.GetLang("修改公告"));
		this.ChangeInfoWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.ChangeInfoWindow = null;
			return true;
		};
		Canvas.SetZIndex(this.ChangeInfoWindow, 9001.0);
		this.Root.Children.Add(this.ChangeInfoWindow);
		ChangeInfoPart changeInfoPart = U3DUtils.NEW<ChangeInfoPart>();
		changeInfoPart.bhid = this.MyBangHuiDetailData.BHID;
		changeInfoPart.BulletinMsg = this.txtFamilyDes.Text;
		changeInfoPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_xggg.png"), false, 0);
		changeInfoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 1)
			{
				if (this.MyBangHuiDetailData != null)
				{
					this.MyBangHuiDetailData.BHBulletin = changeInfoPart.BulletinMsg;
				}
				this.txtFamilyDes.Text = changeInfoPart.BulletinMsg;
			}
			this.ChangeInfoWindow.NotifyClose(0);
		};
		changeInfoPart.InitPartSize((int)this.ChangeInfoWindow.BodyWidth - 18, (int)this.ChangeInfoWindow.BodyHeight - 9);
		changeInfoPart.InitPartData(this.MyBangHuiDetailData);
		this.ChangeInfoWindow.SetContent(this.ChangeInfoWindow.BodyPresenter, changeInfoPart, 9.0, 0.0, true);
	}

	private void ShowBG()
	{
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		if (this.MyBangHuiDetailData.BHID != Global.Data.roleData.Faction)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("非【{0}】战盟成员, 无法查看【{0}】的战盟军贡!"), new object[]
			{
				Global.FormatBangHuiName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName)
			}), 10, 3);
			return;
		}
		if (null != this.BGWindow)
		{
			this.CloseChildWindow(this.BGWindow);
			this.BGWindow = null;
			this.bgPart.CleanUpChildWindows();
			this.bgPart = null;
			return;
		}
		this.ShowModalDialog();
		this.BGWindow = U3DUtils.NEW<GChildWindow>();
		this.BGWindow.Left = 0.0;
		this.BGWindow.Top = 0.0;
		this.BGWindow.HeadLeft = 0.0;
		this.BGWindow.HeadTop = 0.0;
		this.BGWindow.HeadWidth = 648.0;
		this.BGWindow.HeadHeight = 64.0;
		this.BGWindow.BodyLeft = 0.0;
		this.BGWindow.BodyTop = 64.0;
		this.BGWindow.BodyWidth = 648.0;
		this.BGWindow.BodyHeight = 407.0;
		this.BGWindow.ImgTitle = "NetImages/GameRes/Images/WinTitle/BanghuiRongyao.png";
		this.InitChildWindow(this.BGWindow, Global.GetLang("战盟荣耀"));
		this.BGWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseModalDialog();
			this.CloseChildWindow(s as GChildWindow);
			this.BGWindow = null;
			this.bgPart.CleanUpChildWindows();
			this.bgPart = null;
			return true;
		};
		Canvas.SetZIndex(this.BGWindow, 9001.0);
		this.Root.Children.Add(this.BGWindow);
		this.bgPart = U3DUtils.NEW<BGPart>();
		this.bgPart.BodyBackgroundURL = new ImageURL(Global.GetGameResImageURL("Images/Plate/bh_gx_bak.png"), false, 0);
		this.bgPart.InitPartSize((int)this.BGWindow.BodyWidth - 18, (int)this.BGWindow.BodyHeight - 9);
		this.bgPart.InitPartData(this.MyBangHuiDetailData);
		this.BGWindow.SetContent(this.BGWindow.BodyPresenter, this.bgPart, 9.0, 0.0, true);
	}

	public void SortMembersByVoicePriority(GVoicePriorityData data)
	{
		if (this.membersInfoPart != null)
		{
			this.membersInfoPart.SortMembersByVoicePriority(data);
		}
	}

	private void ShowMembersInfoWindow()
	{
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		this.membersInfoPart = U3DUtils.NEW<MembersInfoPart>();
		this.membersInfoPart.transform.parent = base.transform;
		this.membersInfoPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.membersInfoPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.membersInfoPart.InitPartSize(0, 0);
		this.membersInfoPart.InitPartData(this.MyBangHuiDetailData);
		this.membersInfoPart.ShowChatBox = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(s, e);
			}
		};
		this.membersInfoPart.CloseMemberWindow = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.membersInfoPart.transform.parent = null;
			Object.Destroy(this.membersInfoPart.gameObject);
			this.membersInfoPart = null;
			Resources.UnloadUnusedAssets();
		};
	}

	public int GetBHID()
	{
		return this.BHID;
	}

	public void GetNewData(int bhid)
	{
		this.BHID = bhid;
		this.MyBangHuiDetailData = null;
		this.lblLeagueName.text = string.Empty;
		this.lblLeaderName.text = string.Empty;
		this.lblMemberCount.text = string.Empty;
		this.lblNotice.text = string.Empty;
		GameInstance.Game.SpriteQueryBangHuiDetail(this.BHID);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (this.Deco != null)
		{
			Global.RemoveObject(this.Deco, true);
			this.Deco = null;
		}
		Super.CleanUpAllChildWindows(this.Root);
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		int num = this.ChildWindowList.IndexOf(childWindow, 0);
		if (num >= 0)
		{
			this.ChildWindowList.RemoveRange(num, 1);
		}
		Super.CloseChildWindow(this.Root, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow1(childWindow, title);
		this.ChildWindowList.Add(childWindow);
	}

	public void ShowModalDialog()
	{
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Root.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Root, noBorderWindow);
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private string GetIconID(int index, string[] IconIDs)
	{
		if (IconIDs == null)
		{
			return "0";
		}
		if (index < 0 || index >= IconIDs.Length)
		{
			return "0";
		}
		return IconIDs[index];
	}

	public void AddBangQiIcon()
	{
		this.FlagIcoImg.Children.Clear();
		if (this.MyBangHuiDetailData == null)
		{
			return;
		}
		string value = StringUtil.substitute("Images/Flags/00{0}.png", new object[]
		{
			this.MyBangHuiDetailData.QiLevel
		});
		string tip = string.Empty;
		if (this.MyBangHuiDetailData.QiLevel == 1)
		{
			tip = Global.GetLang("一级战旗可为您加成属性最大物理攻击+20,最大魔法攻击+20,最大道术攻击+20，最大生命上限+100");
		}
		else if (this.MyBangHuiDetailData.QiLevel == 2)
		{
			tip = Global.GetLang("二级战旗可以为您加成属性：攻+3%，防+3%，生命上限+10% 战旗战胜利战盟成员可享受世界练级经验加成60% 【战旗的等级越高加成的各项属性越高】");
		}
		else if (this.MyBangHuiDetailData.QiLevel == 3)
		{
			tip = Global.GetLang("三级战旗可以为您加成属性：攻+6%，防+6%，生命上限+15% 战旗战胜利战盟成员可享受世界练级经验加成90% 【战旗的等级越高加成的各项属性越高】");
		}
		else if (this.MyBangHuiDetailData.QiLevel == 4)
		{
			tip = Global.GetLang("四级战旗可以为您加成属性：攻+10%，防+10%，生命上限+20% 战旗战胜利战盟成员可享受世界练级经验加成120% 【战旗的等级越高加成的各项属性越高】");
		}
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 32.0;
		gicon.Height = 32.0;
		gicon.BodyURL = new ImageURL(value, false, 0);
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/32_Hover.png"));
		gicon.TipType = 0;
		gicon.Tip = tip;
		gicon.ItemCode = 0;
		gicon.ItemObject = null;
		gicon.BoxTypes = -1;
		this.FlagIcoImg.Children.Add(gicon);
	}

	public void NotifyBangHuiDetailData(BangHuiDetailData bangHuiDetailData)
	{
		this.MyBangHuiDetailData = bangHuiDetailData;
		if (this.MyBangHuiDetailData == null)
		{
			Super.HintMainText(Global.GetLang("要查看的战盟已经不存在！"), 10, 3);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = 0
				});
			}
			return;
		}
		if (this.MyBangHuiDetailData != null)
		{
			string text = string.Empty;
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.MyBangHuiDetailData.ZoneID, out ztBuffServerInfo) && !Global.IsKuaFuMap(Global.Data.roleData.MapCode, true))
			{
				text = ztBuffServerInfo.strServerName + string.Empty;
			}
			if (string.IsNullOrEmpty(text))
			{
				this.lblLeagueName.text = string.Format("S{0}.{1}", this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BHName);
			}
			else
			{
				this.lblLeagueName.text = string.Format("{0}.{1}", text, this.MyBangHuiDetailData.BHName);
			}
			this.lblLeaderName.text = Global.FormatRoleName(this.MyBangHuiDetailData.ZoneID, this.MyBangHuiDetailData.BZRoleName);
			this.lblMemberCount.text = this.MyBangHuiDetailData.TotalNum.ToString() + "/50";
			if (Global.Data != null)
			{
				Global.Data.mCurrentZhanMengMemberNum = this.MyBangHuiDetailData.TotalNum;
			}
			if (this.MyBangHuiDetailData.BHBulletin == null || this.MyBangHuiDetailData.BHBulletin.Length == 0)
			{
				this.lblNotice.text = Global.GetLang("点击修改公告！");
				this.lblNotice.color = Color.gray;
			}
			else
			{
				this.lblNotice.color = Color.white;
				this.lblNotice.text = this.MyBangHuiDetailData.BHBulletin;
			}
			Global.zhanmengZiJin = (long)this.MyBangHuiDetailData.TotalMoney;
			Global.zhanmengLevel = this.MyBangHuiDetailData.QiLevel;
			this.lblLeagueMoney.text = string.Empty + this.MyBangHuiDetailData.TotalMoney;
			this.TodayZhangongForGold = this.MyBangHuiDetailData.TodayZhanGongForGold;
			this.TodayZhangongForDiamond = this.MyBangHuiDetailData.TodayZhanGongForDiamond;
			int zhanMengWeiHuXiaoHao = Global.ZhanMengWeiHuXiaoHao;
			this.lblMaintain.text = string.Empty + zhanMengWeiHuXiaoHao;
			if (this.MyBangHuiDetailData.TotalMoney < zhanMengWeiHuXiaoHao && !PlayZone.GlobalPlayZone.IsExistZhanMengHongBao())
			{
				string lang = Global.GetLang("战盟资金过少，为避免解散战盟，请尽快捐赠!");
				string[] buttons = new string[]
				{
					Global.GetLang("确定")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
				{
				}, buttons);
			}
			if (this.MyBangHuiDetailData.CanModNameTimes > 0 && Global.Data.RoleName.Equals(this.MyBangHuiDetailData.BZRoleName))
			{
				this.btnRename.gameObject.SetActive(true);
			}
			else
			{
				this.btnRename.gameObject.SetActive(false);
			}
			this.IsShowZhanMengWaiJiao();
		}
		if (this.NeedToShowSubWindow >= 0)
		{
			int needToShowSubWindow = this.NeedToShowSubWindow;
			if (needToShowSubWindow == 0)
			{
				this.CreateFamilyBuildPart();
			}
			this.NeedToShowSubWindow = -1;
		}
		if (this.membersInfoPart != null)
		{
			this.membersInfoPart.InitPartData(this.MyBangHuiDetailData);
		}
	}

	public void NotifyBangHuiMemberDataList(List<BangHuiMemberData> bangHuiMemberDataList)
	{
		if (null != this.membersInfoPart)
		{
			this.membersInfoPart.NotifyBangHuiMemberDataList(bangHuiMemberDataList);
		}
	}

	public void NotifyBangHuiMemberZhiWu(int retCode, int bhid, int otherRoleID, int zhiWu)
	{
		if (null != this.membersInfoPart)
		{
			this.membersInfoPart.NotifyBangHuiMemberZhiWu(retCode, bhid, otherRoleID, zhiWu);
		}
	}

	public void NotifyBangHuiMemberChengHao(int retCode, int bhid, int otherRoleID, string chengHao)
	{
		if (retCode >= 0)
		{
			if (this.MyBangHuiDetailData != null && this.MyBangHuiDetailData.MgrItemList != null)
			{
				for (int i = 0; i < this.MyBangHuiDetailData.MgrItemList.Count; i++)
				{
					if (this.MyBangHuiDetailData.MgrItemList[i].RoleID == otherRoleID)
					{
						this.MyBangHuiDetailData.MgrItemList[i].ChengHao = chengHao;
						break;
					}
				}
			}
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				FamilysManagersListItem familysManagersListItem = U3DUtils.AS<FamilysManagersListItem>(this.ItemCollection[j]);
				if (familysManagersListItem.RoleID == otherRoleID)
				{
					familysManagersListItem.NickName = chengHao;
					break;
				}
			}
		}
		if (null != this.membersInfoPart)
		{
			this.membersInfoPart.NotifyBangHuiMemberChengHao(retCode, bhid, otherRoleID, chengHao);
		}
	}

	public void NotifyRemoveBangHuiMember(int retCode, int bhid, int otherRoleID)
	{
		if (null != this.membersInfoPart)
		{
			this.membersInfoPart.NotifyRemoveBangHuiMember(retCode, bhid, otherRoleID);
		}
	}

	public void RefreshSearchData(List<SearchRoleData> searchRoleDataList)
	{
		if (searchRoleDataList != null)
		{
			if (null != this.membersInfoPart)
			{
				this.membersInfoPart.RefreshSearchData(searchRoleDataList);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("玩家未找到!"), 10, 3);
		}
	}

	public void RefreshZhanMengLianSaiState(string[] Files)
	{
		if (null != this.m_familyActivity && Files != null && 1 < Files.Length)
		{
			int type = Files[0].SafeToInt32(0);
			BangHuiMatchGameStates state = Files[1].SafeToInt32(0);
			BangHuiMatchType oldBangHuiMatchType = Files[2].SafeToInt32(0);
			int rank = Files[3].SafeToInt32(0);
			this.m_familyActivity.RefreshZhanMengLianSaiState(type, state, oldBangHuiMatchType, rank, Files[4].SafeToInt32(0));
		}
	}

	public void NotifyBangGongHist(BangHuiBagData bangHuiBagData)
	{
		if (null != this.bgPart)
		{
			this.bgPart.NotifyBangGongHist(bangHuiBagData);
		}
		if (this.familyBuildPart != null)
		{
			this.familyBuildPart.NotifyGoodsData(bangHuiBagData);
		}
	}

	public void NotifyLianSaiJion(string ret)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeJionCallBack(ret);
		}
	}

	public void NotifyGetLianSaiANALYSISCallBack(List<int> dataList)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NotifyGetLianSaiANALYSISCallBack(dataList);
		}
	}

	public void RefreshBangGong()
	{
		if (null != this.bgPart)
		{
			this.bgPart.RefreshBangGong();
		}
	}

	public void NoticeGetSaiJiGetBHMatch_GetAwardCallBack()
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeGetSaiJiGetBHMatch_GetAwardCallBack();
		}
	}

	public void NotifyDonateTongQianResult(int retCode, int roleID, int bhid, int bhExtraMoney, int personZhangong)
	{
		this.MyBangHuiDetailData.TotalMoney += bhExtraMoney;
		if (null != this.familyDonatePart)
		{
			this.familyDonatePart.RefreshDonateData(retCode, roleID, bhid, bhExtraMoney, personZhangong);
		}
	}

	public void NoticeGetShiPinDataCallBack(Dictionary<int, OrnamentData> data)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeGetShiPinDataCallBack(data);
		}
	}

	public void NoticeGetSaiJiGetRankInfMiniCallBack(List<BangHuiMatchRankInfo> Datalist)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeGetSaiJiGetRankInfMiniCallBack(Datalist);
		}
	}

	public void NotifyDonateGoodsResult(int retCode, int roleID, int bhid)
	{
		if (this.familyDonatePart != null)
		{
			this.familyDonatePart.NotifyDonateGoodsResult(retCode, roleID, bhid);
		}
	}

	public void NotifyBangHuiLingDiInfosDict(Dictionary<int, BangHuiLingDiInfoData> bangHuiLingDiInfosDict)
	{
		if (null != this.jgssPart)
		{
			this.jgssPart.NotifyBangHuiLingDiInfosDict(bangHuiLingDiInfosDict);
		}
	}

	public void NotifySetLingDiTaxResult(int retCode, int roleID, int bhid, int lingDiID, int newLingDiTax)
	{
		if (null != this.jgssPart)
		{
			this.jgssPart.NotifySetLingDiTaxResult(retCode, roleID, bhid, lingDiID, newLingDiTax);
		}
	}

	public void NotifyTakeTaxMoneyResult(int retCode, int roleID, int bhid, int lingDiID, int takeTaxMoney)
	{
		if (null != this.jgssPart)
		{
			this.jgssPart.NotifyTakeTaxMoneyResult(retCode, roleID, bhid, lingDiID, takeTaxMoney);
		}
	}

	public void NotifyBangHuiListData(BangHuiListData bangHuiListData)
	{
		if (null != this.familyListPart)
		{
			this.familyListPart.NotifyBangHuiListData(bangHuiListData);
		}
	}

	public void RefreshEvent(List<ZhanMengShiJianData> list)
	{
		if (null != this.leagueEventPart)
		{
			this.leagueEventPart.refresh(list);
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

	public void RefreshFamilyList(int RoleID)
	{
		this.familyListPart.luolanChengZhuID = RoleID;
		this.familyListPart.GetNewData();
	}

	private void DestoryFamilyListPart()
	{
		this.familyListPart.transform.parent = null;
		Object.Destroy(this.familyListPart.gameObject);
		this.familyListPart = null;
		Resources.UnloadUnusedAssets();
	}

	private void CreateFamilyDonatePart()
	{
		this.familyDonatePart = U3DUtils.NEW<FamilyDonate>();
		this.familyDonatePart.transform.parent = base.transform;
		this.familyDonatePart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.familyDonatePart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.familyDonatePart.GetLeagueDonateData();
		this.familyDonatePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == -10)
			{
				this.DestoryFamilyDonatePart();
			}
			else if (e.Type == 1)
			{
				this.lblLeagueMoney.text = string.Empty + e.ID;
				Global.zhanmengZiJin = (long)e.ID;
			}
			else if (e.Type == 2)
			{
				this.TodayZhangongForGold = e.ID;
			}
			else if (e.Type == 3)
			{
				this.TodayZhangongForDiamond = e.ID;
			}
		};
		this.familyDonatePart.SetDonateInfo(this.lblLeagueMoney.text, this.TodayZhangongForGold, this.TodayZhangongForDiamond);
	}

	private void DestoryFamilyDonatePart()
	{
		this.familyDonatePart.transform.parent = null;
		Object.Destroy(this.familyDonatePart.gameObject);
		this.familyDonatePart = null;
		Resources.UnloadUnusedAssets();
	}

	private void CreateFamilyBuildPart()
	{
		if (this.MyBangHuiDetailData != null)
		{
			this.familyBuildPart = U3DUtils.NEW<FamilyBuild>();
			this.familyBuildPart.transform.parent = base.transform;
			this.familyBuildPart.transform.localPosition = new Vector3(0f, 0f, -1f);
			this.familyBuildPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.familyBuildPart.SetRoleInfo(Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData));
			this.familyBuildPart.SetBuildingInfo(this.MyBangHuiDetailData.QiLevel, this.MyBangHuiDetailData.JiTan, this.MyBangHuiDetailData.JunXie, this.MyBangHuiDetailData.GuangHuan, this.MyBangHuiDetailData.TotalMoney);
			this.familyBuildPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.DestoryFamilyBuildPart();
			};
			GameInstance.Game.SpriteGetBangGongHist(this.MyBangHuiDetailData.BHID);
		}
	}

	private void CreateWaiJiaoPart()
	{
		this.zhanMengWaiJiaoPart = U3DUtils.NEW<ZhanMengWaiJiaoPart>();
		this.zhanMengWaiJiaoPart.transform.parent = base.transform;
		this.zhanMengWaiJiaoPart.transform.localPosition = new Vector3(0f, 0f, -3f);
		this.zhanMengWaiJiaoPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.zhanMengWaiJiaoPart.InitTabPanel();
		this.zhanMengWaiJiaoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.zhanMengWaiJiaoPart = null;
			Resources.UnloadUnusedAssets();
		};
	}

	private void OpenHongBao()
	{
		PlayZone.GlobalPlayZone.OpenZhanMengHongBaoWindow(true);
	}

	private void DestoryFamilyBuildPart()
	{
		this.familyBuildPart.transform.parent = null;
		Object.Destroy(this.familyBuildPart.gameObject);
		this.familyBuildPart = null;
		Resources.UnloadUnusedAssets();
	}

	private void CreateLeagueEventPart()
	{
		this.leagueEventPart = U3DUtils.NEW<LeagueEventPart>();
		this.leagueEventPart.transform.parent = base.transform;
		this.leagueEventPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.leagueEventPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.leagueEventPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.leagueEventPart = null;
			Resources.UnloadUnusedAssets();
		};
	}

	public void ShowSubWindow(int idx)
	{
		this.NeedToShowSubWindow = idx;
	}

	public void NotifyLianSaiRankCallBack(List<BangHuiMatchRankInfo> data)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NotifyLianSaiRankCallBack(data);
		}
	}

	public void NoticeGetSaiJiAwardCallBack(int ret)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeGetSaiJiAwardCallBack(ret);
		}
	}

	public void NoticeGetSaiJiGetBHMatch_AwardCallBack(BangHuiMatchAwardsData data)
	{
		if (null != this.m_familyActivity)
		{
			this.m_familyActivity.NoticeGetSaiJiGetBHMatch_AwardCallBack(data);
		}
	}

	public void NotifyBuildingLevelUpSucess(int buildType, int newLevel, int subMoney)
	{
		if (newLevel < 0)
		{
			string message = string.Empty;
			switch (newLevel + 3)
			{
			case 0:
				message = Global.GetLang("配置项错误！");
				break;
			default:
				if (newLevel != -9368)
				{
					if (newLevel != -1210)
					{
						if (newLevel != -1120)
						{
							if (newLevel != -1110)
							{
								if (newLevel != -1005)
								{
									if (newLevel != -1000)
									{
										message = Global.GetLang("其他错误，错误码[") + newLevel + "]";
									}
									else
									{
										message = Global.GetLang("该战盟不存在！");
									}
								}
								else
								{
									message = Global.GetLang("战盟建筑级别错误！");
								}
							}
							else
							{
								message = Global.GetLang("战盟资金不足，无法进行操作！");
							}
						}
						else
						{
							message = Global.GetLang("战盟资金过低，为避免战盟解散，导致剩余资金低于") + Global.ZhanMengChuShiZiJin + Global.GetLang("的操作不能执行！");
						}
					}
					else
					{
						message = Global.GetLang("升级所需要的道具不足。");
					}
				}
				else
				{
					message = Global.GetLang("只有盟主能进行升级操作！");
				}
				break;
			case 2:
				message = Global.GetLang("非战盟成员！");
				break;
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s, DPSelectedItemEventArgs e)
			{
			}, buttons);
		}
		else
		{
			switch (buildType)
			{
			case 1:
				this.MyBangHuiDetailData.QiLevel = newLevel;
				Super.HintMainText(Global.GetLang("战盟战旗升级成功！"), 10, 3);
				break;
			case 2:
				this.MyBangHuiDetailData.JiTan = newLevel;
				Super.HintMainText(Global.GetLang("战盟祭坛升级成功！"), 10, 3);
				break;
			case 3:
				this.MyBangHuiDetailData.JunXie = newLevel;
				Super.HintMainText(Global.GetLang("战盟军械升级成功！"), 10, 3);
				break;
			case 4:
				this.MyBangHuiDetailData.GuangHuan = newLevel;
				Super.HintMainText(Global.GetLang("战盟光环升级成功！"), 10, 3);
				break;
			}
			this.MyBangHuiDetailData.TotalMoney -= subMoney;
			this.lblLeagueMoney.text = string.Empty + this.MyBangHuiDetailData.TotalMoney;
			Global.zhanmengZiJin = (long)this.MyBangHuiDetailData.TotalMoney;
			this.familyBuildPart.NotifyBuildingLevelUpSucess(buildType, newLevel, this.MyBangHuiDetailData.TotalMoney);
			GameInstance.Game.SpriteGetBangGongHist(this.MyBangHuiDetailData.BHID);
		}
	}

	public void NotifyBuildingBuffGetSucess(int retcode)
	{
		if (retcode <= 0)
		{
			string message = string.Empty;
			switch (retcode + 3)
			{
			case 0:
				message = Global.GetLang("系统错误！");
				break;
			default:
				if (retcode != -1110)
				{
					if (retcode != -1000)
					{
						message = Global.GetLang("其他错误，错误码[") + retcode + "]";
					}
					else
					{
						message = Global.GetLang("操作数据库失败！");
					}
				}
				else
				{
					message = Global.GetLang("战功值不足！");
				}
				break;
			case 2:
				message = Global.GetLang("非战盟成员！");
				break;
			case 3:
				message = Global.GetLang("指令参数错误");
				break;
			}
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s, DPSelectedItemEventArgs e)
			{
			}, buttons);
		}
		else
		{
			this.familyBuildPart.NotifyGetBuffSucess();
		}
	}

	public void DPSelectItemHandlerQuitBH(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			GameInstance.Game.SpriteQuitFromBangHui(Global.Data.roleData.Faction);
		}
	}

	public void IsShowZhanMengWaiJiao()
	{
		if ((Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction) || Global.IsInBangHui(Global.Data.roleData.Faction)) && Global.zhanmengLevel >= 5)
		{
			if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
			{
				ActivityTipManager.RegActivityTipItem(14111, delegate(int s, ActivityTipItem e)
				{
					this.TipIconZhanMeng.gameObject.SetActive(e.IsActive);
				});
			}
			this.btnZhanMengWaiJiao.gameObject.SetActive(true);
			this.btnQuite.transform.localPosition = new Vector3(-54f, -215f, -0.1f);
		}
		else
		{
			this.btnQuite.transform.localPosition = new Vector3(409f, -155f, -0.1f);
			this.btnZhanMengWaiJiao.gameObject.SetActive(false);
		}
	}

	public void ShowActivityWindow()
	{
		this.m_familyActivity = U3DUtils.NEW<FamilyActivity>();
		this.m_familyActivity.transform.parent = base.transform;
		this.m_familyActivity.transform.localPosition = new Vector3(0f, 0f, -1.2f);
		this.m_familyActivity.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_familyActivity.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseActivityWindow();
			return true;
		};
		this.m_familyActivity.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 1)
			{
				this.CloseActivityWindow();
			}
			else if (e.ID == 10)
			{
				if (null != this.m_familyActivity)
				{
					this.m_familyActivity.gameObject.SetActive(false);
				}
				if (null == this.shenQingPart)
				{
					this.shenQingPart = U3DUtils.NEW<shenqingchengzhan>();
					this.shenQingPart.transform.parent = base.transform;
					this.shenQingPart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						if (null != this.m_familyActivity)
						{
							this.m_familyActivity.gameObject.SetActive(true);
						}
						Object.Destroy(this.shenQingPart.gameObject);
						this.shenQingPart = null;
					};
					this.shenQingPart.transform.localScale = new Vector3(1f, 1f, 1f);
					this.shenQingPart.transform.localPosition = new Vector3(0f, 0f, -1f);
					this.shenQingPart.SetZhanmengInfo(this.MyBangHuiDetailData.BHID, this.MyBangHuiDetailData.TotalMoney, this.m_ChengZhuInfo.BHName, this.m_ChengZhuInfo.BHID);
				}
			}
			else if (e.ID == 13)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 13
					});
				}
			}
			else if (e.ID == 12)
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				XElement gameResXml = Global.GetGameResXml("Config/GleeFeastAward.Xml");
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "YanHui");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[0], "ProhibitedTime");
				string[] array = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				bool flag = true;
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					int dayOfWeek = correctDateTime.DayOfWeek;
					if (Convert.ToInt32(array2[0]) == dayOfWeek)
					{
						DateTime dateTime = Convert.ToDateTime(array2[1]);
						DateTime dateTime2 = Convert.ToDateTime(array2[2]);
						if (correctDateTime.TimeOfDay < dateTime2.TimeOfDay && correctDateTime.TimeOfDay > dateTime.TimeOfDay)
						{
							flag = false;
						}
					}
				}
				if (!flag)
				{
					Super.HintMainText(Global.GetLang("当前时段不能举办宴会"), 10, 3);
					return;
				}
				if (null != this.m_familyActivity)
				{
					this.m_familyActivity.gameObject.SetActive(false);
				}
				if (null == this.yanHuiPart)
				{
					this.yanHuiPart = U3DUtils.NEW<YanHuiPart>();
					PlayZone.GlobalPlayZone.yanhuiPart = this.yanHuiPart;
					this.yanHuiPart.transform.parent = base.transform;
					this.yanHuiPart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						if (null != this.m_familyActivity)
						{
							this.m_familyActivity.gameObject.SetActive(true);
						}
						Object.Destroy(this.yanHuiPart.gameObject);
						PlayZone.GlobalPlayZone.yanhuiPart = null;
						this.yanHuiPart = null;
						if (null != this.yanHuiLingQuPart)
						{
							Object.Destroy(this.yanHuiLingQuPart.gameObject);
							PlayZone.GlobalPlayZone.yanhuiLingQuPart = null;
							this.yanHuiLingQuPart = null;
						}
					};
					this.yanHuiPart.transform.localScale = new Vector3(1f, 1f, 1f);
					this.yanHuiPart.transform.localPosition = new Vector3(0f, 0f, -5f);
					this.yanHuiPart.gameObject.SetActive(false);
				}
				else
				{
					this.yanHuiPart.gameObject.SetActive(false);
				}
				if (null == this.yanHuiLingQuPart)
				{
					this.yanHuiLingQuPart = U3DUtils.NEW<YanHuiLingQuPart>();
					PlayZone.GlobalPlayZone.yanhuiLingQuPart = this.yanHuiLingQuPart;
					this.yanHuiLingQuPart.transform.parent = base.transform;
					this.yanHuiLingQuPart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						if (null != this.m_familyActivity)
						{
							this.m_familyActivity.gameObject.SetActive(true);
						}
						Object.Destroy(this.yanHuiLingQuPart.gameObject);
						PlayZone.GlobalPlayZone.yanhuiLingQuPart = null;
						this.yanHuiLingQuPart = null;
						if (null != this.yanHuiPart)
						{
							Object.Destroy(this.yanHuiPart.gameObject);
							PlayZone.GlobalPlayZone.yanhuiPart = null;
							this.yanHuiPart = null;
						}
					};
					this.yanHuiLingQuPart.transform.localScale = new Vector3(1f, 1f, 1f);
					this.yanHuiLingQuPart.transform.localPosition = new Vector3(0f, 0f, -5f);
					this.yanHuiLingQuPart.gameObject.SetActive(false);
				}
				else
				{
					this.yanHuiLingQuPart.gameObject.SetActive(false);
				}
				GameInstance.Game.ApplyYanHui(1, 1);
			}
		};
	}

	private void CloseActivityWindow()
	{
		this.m_familyActivity.transform.parent = null;
		Object.Destroy(this.m_familyActivity.gameObject);
		this.m_familyActivity = null;
		Resources.UnloadUnusedAssets();
	}

	public new void Update()
	{
		if (this.keyboard != null && this.keyboard.done && !this.isNoticeShow)
		{
			if (this.keyboard.text.Length > 100 && !this.isNoticeShow)
			{
				Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
				this.isNoticeShow = true;
				return;
			}
			WordsFilterMgr.ExecWordsFilter(this.keyboard.text, delegate(object content, ExecWordsFilterEventArgs result)
			{
				if (result.ret > 0 && !this.isNoticeShow)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
					{
						result.ret,
						result.msg
					}), 10, 3);
					this.isNoticeShow = true;
					return;
				}
				if (result.is_dirty > 0 && !this.isNoticeShow)
				{
					Super.HintMainText(Global.GetLang("战盟公告不能包含国家规定禁止的词汇!"), 10, 3);
					this.isNoticeShow = true;
					return;
				}
				string text = result.msg;
				text = Global.StringReplaceAll(text, "'", string.Empty);
				text = Global.StringReplaceAll(text, "|", string.Empty);
				text = Global.StringReplaceAll(text, "$", string.Empty);
				text = Global.StringReplaceAll(text, ":", string.Empty);
				GameInstance.Game.SpriteUpdateBangHuiBulletinMsg(this.MyBangHuiDetailData.BHID, text);
				this.MyBangHuiDetailData.BHBulletin = text;
				this.lblNotice.color = Color.white;
				this.lblNotice.text = text;
			});
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(15001, null);
		ActivityTipManager.RegActivityTipItem(15002, null);
		ActivityTipManager.RegActivityTipItem(15004, null);
		ActivityTipManager.RegActivityTipItem(14111, null);
	}

	public void RefreshChengzhanShenqingPart(List<LuoLanChengZhanRequestInfoEx> listResponse)
	{
		if (this.shenQingPart != null)
		{
			this.shenQingPart.DataRefresh(listResponse);
		}
		if (this.m_familyActivity != null)
		{
			this.m_familyActivity.RefreshChengzhanShenqingInfo(listResponse);
		}
	}

	public void RefreshChengzhuInfo(LuoLanChengZhuInfo chengzhuInfo)
	{
		this.m_ChengZhuInfo = chengzhuInfo;
		if (this.m_familyActivity != null)
		{
			this.m_familyActivity.RefreshChengzhuInfo(chengzhuInfo);
		}
	}

	public void Reload3DModal()
	{
		if (this.m_familyActivity != null)
		{
			this.m_familyActivity.Reload3DModal();
		}
	}

	public void RefreshZhanmengZijin(int ZhanmengId, int ZhanmengZijin)
	{
		if (Global.Data.roleData.Faction == ZhanmengId)
		{
			this.lblLeagueMoney.text = string.Empty + ZhanmengZijin;
			Global.zhanmengZiJin = (long)this.MyBangHuiDetailData.TotalMoney;
		}
		if (this.shenQingPart != null)
		{
			this.MyBangHuiDetailData.TotalMoney = ZhanmengZijin;
			this.shenQingPart.SetZhanmengInfo(ZhanmengId, ZhanmengZijin, this.m_ChengZhuInfo.BHName, this.m_ChengZhuInfo.BHID);
		}
	}

	public void RefreshChenzhanBidResult(int result)
	{
		if (this.shenQingPart != null)
		{
			this.shenQingPart.RefreshBidResult(result);
		}
	}

	public void RefreshRewardResult(int result)
	{
		if (this.m_familyActivity != null)
		{
			this.m_familyActivity.RefreshRewardResult(result);
		}
	}

	public void Action_CMD_NTF_BANGHUI_CHANGE_NAME(MUSocketConnectEventArgs e)
	{
		string text = e.fields[0];
		string text2 = e.fields[1];
		this.lblLeagueName.text = text2;
		this.btnRename.gameObject.SetActive(false);
	}

	private bool IsChildWindowOpenEnable(GButton fromBtn)
	{
		bool flag = true;
		if (!(fromBtn == this.btnMemberInfo))
		{
			if (fromBtn == this.btnDonate)
			{
				flag = (flag ? (!(this.membersInfoPart != null) && flag) : flag);
				flag = (flag ? (!(this.familyDonatePart != null) && flag) : flag);
				flag = (flag ? (!(this.familyBuildPart != null) && flag) : flag);
				flag = (flag ? (!(this.leagueEventPart != null) && flag) : flag);
				flag = (flag ? (!(this.familyListPart != null) && flag) : flag);
				flag = (flag ? (!(this.m_familyActivity != null) && flag) : flag);
			}
			else if (!(fromBtn == this.btnBuild))
			{
				if (!(fromBtn == this.btnLeagueEvent))
				{
					if (!(fromBtn == this.btnLeagueList))
					{
						if (fromBtn == this.btnLeagueActivity)
						{
							flag = (flag ? (!(this.membersInfoPart != null) && flag) : flag);
							flag = (flag ? (!(this.familyDonatePart != null) && flag) : flag);
							flag = (flag ? (!(this.familyBuildPart != null) && flag) : flag);
							flag = (flag ? (!(this.leagueEventPart != null) && flag) : flag);
							flag = (flag ? (!(this.familyListPart != null) && flag) : flag);
							flag = (flag ? (!(this.m_familyActivity != null) && flag) : flag);
						}
						else if (!(fromBtn == this.btnLeagueShop))
						{
							if (fromBtn == this.btnQuite)
							{
							}
						}
					}
				}
			}
		}
		return flag;
	}

	private void ClearChildWindowReference()
	{
		this.membersInfoPart = null;
		this.familyDonatePart = null;
		this.familyBuildPart = null;
		this.leagueEventPart = null;
		this.familyListPart = null;
		this.m_familyActivity = null;
	}

	public UILabel lblLeagueName;

	public UILabel lblLeaderName;

	public UILabel lblMemberCount;

	public UILabel lblLeagueMoney;

	public UILabel lblMaintain;

	public UILabel lblNotice;

	public GameObject TipIconFamily;

	public GameObject TipIconZhanMeng;

	public GButton btnMemberInfo;

	public GButton btnDonate;

	public GButton btnBuild;

	public GButton btnLeagueEvent;

	public GButton btnLeagueList;

	public GButton btnLeagueActivity;

	public GButton btnLeagueShop;

	public GButton btnQuite;

	public GButton btnClose;

	public GButton btnRename;

	public GButton btnZhanMengShenDian;

	public GButton btnZhanMengWaiJiao;

	public GButton btnHongBao;

	public GameObject TipIconHongBao;

	public GButton btnShowEdit;

	public GButton btnSubmit;

	public GButton btnCloseEditWin;

	public UIInput inputNotice;

	public GameObject NoticeEditWin;

	public GChildWindow BGWindow;

	public GChildWindow MembersInfoWindow;

	public GChildWindow ChangeInfoWindow;

	public GChildWindow JgssWindow;

	private LoadingWindow LoadingWin;

	private GDecoration Deco;

	private FamilysManagersListItem SelectedListItem;

	private Canvas Root;

	private shenqingchengzhan shenQingPart;

	private YanHuiPart yanHuiPart;

	private YanHuiLingQuPart yanHuiLingQuPart;

	public ListBox listBox;

	private TouchScreenKeyboard keyboard;

	private Canvas FlagIcoImg;

	private GTextBlockOutLine txtFamilyDes;

	private SpriteSL thisCtrl;

	private FamilyListPart familyListPart;

	private FamilyDonate familyDonatePart;

	private FamilyBuild familyBuildPart;

	private LeagueEventPart leagueEventPart;

	public ZhanMengWaiJiaoPart zhanMengWaiJiaoPart;

	public FamilyActivity m_familyActivity;

	private LuoLanChengZhuInfo m_ChengZhuInfo;

	private int NeedToShowSubWindow = -1;

	private int _todayZhangongForGold;

	private int _todayZhangongForDiamond;

	private bool isNoticeShow;

	private bool isShowZhanMengBossTip;

	private bool isShowLangHunTip;

	private ImageBrush SelectedListItemBakImg;

	private JgssPart jgssPart;

	private BGPart bgPart;

	private MembersInfoPart membersInfoPart;

	private int BHID;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private Canvas PlaceHolder;

	public BangHuiDetailData MyBangHuiDetailData;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
