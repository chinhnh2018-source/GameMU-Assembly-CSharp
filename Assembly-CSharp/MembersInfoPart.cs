using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MembersInfoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnQuite.Text = Global.GetLang("解散战盟");
		this.btnDismiss.Text = this.btnQuite.Text;
		this.btnAddMember.Text = Global.GetLang("添加成员");
		this.chkAutoVerify.Text = Global.GetLang("自动收人");
		this.btnAllMember.Text = Global.GetLang("所有成员");
		this.btnOnline.Text = Global.GetLang("在线成员");
		this.btnOffline.Text = Global.GetLang("离线成员");
		this.lblFilter.text = Global.GetLang("所有成员");
		this.btnSubmitMemberInfo.Text = Global.GetLang("添加");
		this.ConstHint.Text = Global.GetLang("请输入角色名称：");
		this.ConstTitle.Text = Global.GetLang("添加成员");
		this.inputMemberName.text = Global.GetLang("添加成员");
		this.inputMemberName.defaultText = Global.GetLang("添加成员");
		this.mLblSpeechMembers.Text = Global.GetLang("允许语音：0/2");
		NGUITools.SetActive(this.mLblSpeechMembers.gameObject, false);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitVoiceMembers();
		this.Root = this.Container;
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
	}

	private void InitVoiceMembers()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("VoicePowerNum", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length >= 2)
		{
			this.sumSpeechCount = systemParamIntArrayByName[0];
		}
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

	public void SortMembersByVoicePriority(GVoicePriorityData data)
	{
		this.mGVoicePriorityData = null;
		GameInstance.Game.SpriteGetBangHuiMemberDataList(this.MyBangHuiDetailData.BHID);
		if (data == null)
		{
			return;
		}
		if (data.Type == 1)
		{
			this.mGVoicePriorityData = data;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.chkAutoVerify.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (!Global.IsBangHuiLeader(Global.Data.roleData, this.MyBangHuiDetailData))
			{
				Super.HintMainText(Global.GetLang("只有首领才能修改此设置"), 10, 3);
				this.chkAutoVerify.Check = !this.chkAutoVerify.Check;
				return;
			}
			this.MyBangHuiDetailData.IsVerify = ((!this.chkAutoVerify.Check) ? 1 : 0);
			GameInstance.Game.SpriteUpdateBHVerify(this.MyBangHuiDetailData.BHID, this.MyBangHuiDetailData.IsVerify);
		};
		this.btnAddMember.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.MyBangHuiMemberDataList.Count >= 50)
			{
				string message = StringUtil.substitute(Global.GetLang("战盟成员人数已经达到上限，无法再添加成员!"), new object[0]);
				string[] buttons = new string[]
				{
					Global.GetLang("确定")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
				{
				}, buttons);
			}
			else
			{
				this.ShowAddMemberWin();
			}
		};
		this.btnDismiss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsBangHuiLeader(Global.Data.roleData, this.MyBangHuiDetailData))
			{
				string message = StringUtil.substitute(Global.GetLang("您确认要解散【{0}】战盟吗？战盟解散后，将不可恢复, 请谨慎操作!"), new object[]
				{
					this.MyBangHuiDetailData.BHName
				});
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, new DPSelectedItemEventHandler(this.DPSelectItemHandlerDismissBH), buttons);
			}
			else
			{
				Super.HintMainText(Global.GetLang("只有首领能执行解散战盟的操作!"), 10, 3);
			}
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
		this.btnAllMember.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectedMenuItemID = 0;
			this.lblFilter.text = Global.GetLang("所有成员");
			this.FilterMemberDataList(this.SelectedMenuItemID);
			this.RefreshList();
			this.scrollBar.scrollValue = 0f;
		};
		this.btnOnline.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectedMenuItemID = 1;
			this.lblFilter.text = Global.GetLang("在线成员");
			this.FilterMemberDataList(this.SelectedMenuItemID);
			this.RefreshList();
			this.scrollBar.scrollValue = 0f;
		};
		this.btnOffline.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SelectedMenuItemID = 2;
			this.lblFilter.text = Global.GetLang("离线成员");
			this.FilterMemberDataList(this.SelectedMenuItemID);
			this.RefreshList();
			this.scrollBar.scrollValue = 0f;
		};
		this.btnSubmitMemberInfo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = Global.StringTrim(this.inputMemberName.text);
			if (!string.IsNullOrEmpty(text))
			{
				if (text.get_Chars(0) == '[' && text.IndexOf(Global.GetLang("区]")) != -1)
				{
					try
					{
						text = text.Substring(text.IndexOf(']') + 1);
					}
					catch (Exception ex)
					{
						MUDebug.LogException(ex);
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					this.SearchPlayers(text);
				}
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseMemberWindow != null)
			{
				this.CloseMemberWindow(this, null);
			}
		};
		this.btnCloseAddMemberWin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.addMemberWin.SetActive(false);
		};
	}

	private int ItemsList_Sort(BangHuiMemberData a, BangHuiMemberData b)
	{
		if (Convert.ToBoolean((string)this.JnSortIcon.Tag))
		{
			return b.SkillLearnedNum - a.SkillLearnedNum;
		}
		return a.SkillLearnedNum - b.SkillLearnedNum;
	}

	private int ItemsList_Sort1(BangHuiMemberData a, BangHuiMemberData b)
	{
		if (Convert.ToBoolean((string)this.JmSortIcon.Tag))
		{
			return b.XueWeiNum - a.XueWeiNum;
		}
		return a.XueWeiNum - b.XueWeiNum;
	}

	private int ItemsList_Sort3(BangHuiMemberData a, BangHuiMemberData b)
	{
		if (Convert.ToBoolean(this.LevelSortIcon.Tag.ToString()))
		{
			return b.Level - a.Level;
		}
		return a.Level - b.Level;
	}

	public void InitPartData(BangHuiDetailData bangHuiDetailData)
	{
		this.RefreshList();
		this.MyBangHuiDetailData = bangHuiDetailData;
		this.chkAutoVerify.Check = (this.MyBangHuiDetailData.IsVerify <= 0);
		this.chkAutoVerify.gameObject.SetActive(false);
		this.btnAddMember.gameObject.SetActive(false);
		this.btnDismiss.gameObject.SetActive(false);
		int bangHuiZhiWuByRoleID = Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData);
		if (bangHuiZhiWuByRoleID == 0)
		{
			this.chkAutoVerify.gameObject.SetActive(false);
			this.btnAddMember.gameObject.SetActive(false);
			this.btnDismiss.gameObject.SetActive(false);
		}
		else if (bangHuiZhiWuByRoleID == 1)
		{
			this.chkAutoVerify.gameObject.SetActive(true);
			this.btnAddMember.gameObject.SetActive(true);
			this.btnDismiss.gameObject.SetActive(true);
			this.btnAddMember.transform.localPosition = new Vector3(201f, -229f, 0f);
		}
		else if (bangHuiZhiWuByRoleID == 2 || bangHuiZhiWuByRoleID == 3 || bangHuiZhiWuByRoleID == 4)
		{
			this.chkAutoVerify.gameObject.SetActive(true);
			this.btnAddMember.gameObject.SetActive(true);
			this.btnDismiss.gameObject.SetActive(false);
			this.btnAddMember.transform.localPosition = new Vector3(341f, -229f, 0f);
		}
		GameInstance.Game.SendGetRealTimePriority(1);
	}

	private void RefreshList()
	{
		this.ShowPage(this.CurrentSelectedPage);
		base.StartCoroutine<bool>(this.ResetPos());
	}

	private IEnumerator ResetPos()
	{
		yield return new WaitForEndOfFrame();
		GameObject go = this.listBox.transform.parent.transform.gameObject;
		SpringPanel.Begin(go, new Vector3(-10f, -11f, 0f), 20f).onFinished = delegate()
		{
			this.listBox.transform.parent.GetComponent<UIPanel>().clipRange = new Vector4(0f, 0f, 910f, 346f);
		};
		yield break;
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

	private void SetBtnState(bool state)
	{
	}

	private void listBox_MouseLeftButtonDown(object sender, MouseEvent e)
	{
	}

	public void ShowAddMemberWin()
	{
		if (this.MyBangHuiDetailData != null && !this.addMemberWin.activeInHierarchy)
		{
			this.addMemberWin.SetActive(true);
		}
	}

	public void ChangeNickName()
	{
	}

	public void ChangeZw()
	{
		this.changeZwPart = U3DUtils.NEW<ChangeZwPart>();
		this.changeZwPart.transform.parent = base.transform;
		this.changeZwPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.changeZwPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.changeZwPart.RoleName = this.SelectedListItem.RoleName;
		this.changeZwPart.RoleID = this.SelectedListItem.RoleID;
		this.changeZwPart.RefreshPopupMenuByZhiwuID(Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData));
		this.changeZwPart.InitPartData(this.MyBangHuiDetailData);
		this.changeZwPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.changeZwPart != null)
			{
				this.changeZwPart.transform.parent = null;
				Object.Destroy(this.changeZwPart.gameObject);
			}
		};
	}

	private void SpeechPermission(int setedID, int type)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.ItemCollection != null && this.ItemCollection.Count > 0)
		{
			int count = this.ItemCollection.Count;
			for (int i = 0; i < count; i++)
			{
				MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[i]);
				if (membersInfoListItem.BHZhiWu != 1)
				{
					if (type != 3 || membersInfoListItem.RoleID != setedID)
					{
						if (membersInfoListItem.Speech < 3)
						{
							stringBuilder.Append(membersInfoListItem.RoleID);
							stringBuilder.Append(",");
						}
					}
				}
			}
		}
		if (type == 3)
		{
			stringBuilder.ToString().TrimEnd(new char[]
			{
				','
			});
		}
		else
		{
			stringBuilder.Append(setedID);
		}
		GameInstance.Game.SendSetRealTimePriority(1, stringBuilder.ToString());
	}

	private void DeleMember()
	{
		if (null == this.SelectedListItem)
		{
			Super.HintMainText(Global.GetLang("请先选中要开除的战盟成员!"), 10, 3);
			return;
		}
		int bangHuiZhiWuByRoleID = Global.GetBangHuiZhiWuByRoleID(this.SelectedListItem.RoleID, this.MyBangHuiDetailData);
		if (bangHuiZhiWuByRoleID == 1)
		{
			Super.HintMainText(Global.GetLang("首领无法被开除出战盟!"), 10, 3);
			return;
		}
		int bangHuiZhiWuByRoleID2 = Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData);
		if (bangHuiZhiWuByRoleID2 > 0)
		{
			if (bangHuiZhiWuByRoleID2 > 1 && bangHuiZhiWuByRoleID > 1)
			{
				Super.HintMainText(Global.GetLang("只有首领才能开除战盟的管理成员!"), 10, 3);
				return;
			}
			string message = StringUtil.substitute(Global.GetLang("您确认要开除【{0}】吗？"), new object[]
			{
				this.SelectedListItem.RoleName
			});
			GChildWindow messageBoxWindow = Super.ShowMessageBox(this.Container, 1, Global.GetLang("提示"), message, ((int)this.Container.Width - 253) / 2, ((int)this.Container.Height - 171) / 2, (int)this.Container.Width, (int)this.Container.Height, 0.01, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(this.Container, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SpriteRemoveBHMember(Global.Data.roleData.Faction, this.SelectedListItem.RoleID, this.SelectedListItem.RoleName);
				}
				return true;
			};
		}
		else
		{
			Super.HintMainText(Global.GetLang("普通成员无法执行开除成员的操作!"), 10, 3);
		}
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

	private void CloseChildWindow(GChildWindow childWindow)
	{
		this.ChildWindowList.Remove(childWindow);
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

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (null != this.listBox.LastSelectedItem && this.listBox.LastSelectedItem != this.listBox.SelectedItem)
		{
			MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.listBox.LastSelectedItem);
			if (null != membersInfoListItem)
			{
				membersInfoListItem.SelectedState = false;
			}
		}
		if (null != this.listBox.SelectedItem)
		{
			MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.listBox.SelectedItem);
			if (!membersInfoListItem.SelectedState || this.listBox.LastSelectedItem != this.listBox.SelectedItem)
			{
				membersInfoListItem.SelectedState = true;
				this.SelectedListItem = membersInfoListItem;
			}
		}
	}

	private void SelectListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem();
		}
	}

	private void UnSelectItem()
	{
		this.SelectedListItem = null;
	}

	private void ShowVerifyList()
	{
	}

	private void ShowPage(int pageIndex)
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			MembersInfoListItem membersInfoListItem = U3DUtils.NEW<MembersInfoListItem>();
			membersInfoListItem.RoleID = this.ItemsList[i].RoleID;
			membersInfoListItem.RoleName = Global.FormatRoleName(this.ItemsList[i].ZoneID, this.ItemsList[i].RoleName);
			membersInfoListItem.BHZhiWu = this.ItemsList[i].BHZhiwu;
			membersInfoListItem.Level = string.Concat(new string[]
			{
				"LV:",
				this.ItemsList[i].Level.ToString(),
				"[",
				this.ItemsList[i].BangHuiMemberChangeLifeLev.ToString(),
				Global.GetLang("转]")
			});
			membersInfoListItem.Occupation = this.ItemsList[i].Occupation;
			membersInfoListItem.Occ = Global.GetOccupationStr(this.ItemsList[i].Occupation);
			membersInfoListItem.Zw = Global.GetBHZhiWu(this.ItemsList[i].BHZhiwu);
			membersInfoListItem.MemberForce = string.Empty + this.ItemsList[i].BangHuiMemberCombatForce;
			membersInfoListItem.LogOffTime = this.ItemsList[i].LogOffTime;
			membersInfoListItem.ZhiWuID = Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData);
			membersInfoListItem.IsOnline = this.ItemsList[i].OnlineState;
			if (membersInfoListItem.BHZhiWu == 1)
			{
				membersInfoListItem.Speech = 1;
				this.RefreshSpeechMembers(1);
			}
			else
			{
				membersInfoListItem.Speech = 3;
				membersInfoListItem.HideVoiceIcon = true;
			}
			membersInfoListItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.ChangeZw();
				}
				else if (e.ID == 1)
				{
					this.DeleMember();
				}
				else if (e.ID == 2)
				{
					if (this.ShowChatBox != null)
					{
						this.ShowChatBox(s, e);
						base.transform.parent = null;
						Object.Destroy(base.gameObject);
					}
				}
				else if (e.ID == 4)
				{
					this.SpeechPermission(e.MyID, e.Type);
				}
			};
			UIPanel component = membersInfoListItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.ItemCollection.AddNoUpdate(membersInfoListItem);
		}
		this.ItemCollection.DelayUpdate();
		this.RefreshMembersStatusByGVoicePriorityData();
	}

	private void RefreshSpeechMembers(int count)
	{
		this.mLblSpeechMembers.Text = string.Format("{0}{1}/{2}", Global.GetLang("允许语音："), count, this.sumSpeechCount);
	}

	private void RefreshMembersStatusByGVoicePriorityData()
	{
		if (this.mGVoicePriorityData == null)
		{
			return;
		}
		string[] array = null;
		if (!string.IsNullOrEmpty(this.mGVoicePriorityData.RoleIdList))
		{
			array = this.mGVoicePriorityData.RoleIdList.Split(new char[]
			{
				','
			});
		}
		if (this.ItemCollection != null && this.ItemCollection.Count > 0)
		{
			int count = this.ItemCollection.Count;
			int num = 1;
			for (int i = 0; i < count; i++)
			{
				MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[i]);
				if (membersInfoListItem.BHZhiWu == 1)
				{
					membersInfoListItem.Speech = 1;
				}
				else
				{
					membersInfoListItem.Speech = 3;
					membersInfoListItem.HideVoiceIcon = true;
				}
			}
			for (int j = 0; j < count; j++)
			{
				if (array != null && array.Length > 0)
				{
					for (int k = 0; k < array.Length; k++)
					{
						MembersInfoListItem membersInfoListItem2 = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[j]);
						if (membersInfoListItem2.BHZhiWu != 1)
						{
							if (membersInfoListItem2.RoleID == ConvertExt.SafeConvertToInt32(array[k]))
							{
								membersInfoListItem2.Speech = 2;
								num++;
							}
						}
					}
				}
			}
			this.RefreshSpeechMembers(num);
		}
	}

	private void NextPage()
	{
		if (this.CurrentSelectedPage < this.MaxPageCount - 1)
		{
			this.CurrentSelectedPage++;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void PrevPage()
	{
		if (this.CurrentSelectedPage > 0)
		{
			this.CurrentSelectedPage--;
			this.ShowPage(this.CurrentSelectedPage);
		}
	}

	private void FirstPage()
	{
		this.CurrentSelectedPage = 0;
		this.ShowPage(this.CurrentSelectedPage);
	}

	private void EndPage()
	{
		this.CurrentSelectedPage = this.MaxPageCount - 1;
		this.ShowPage(this.CurrentSelectedPage);
	}

	private void HideWindow()
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	public void ShowMenuWindow(int x, int y, int[] ids, string[] names, int menus_id)
	{
		if (null != this.MenuWindow)
		{
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			this.menuPart = null;
		}
		this.MenuWindow = U3DUtils.NEW<NoBorderWindow>();
		this.MenuWindow.Left = (double)x;
		this.MenuWindow.Top = (double)y;
		this.InitNoBorderWindow(this.MenuWindow);
		this.Root.Children.Add(this.MenuWindow);
		this.menuPart = U3DUtils.NEW<GMenuPart>();
		this.menuPart.InitPartSize((int)this.MenuWindow.BodyWidth - 4, (int)this.MenuWindow.BodyHeight - 4);
		string imageFileName = "Images/Plate/menu_item_unselected.png";
		for (int i = 0; i < ids.Length; i++)
		{
			this.menuPart.AddMenuItem(ids[i], imageFileName, names[i], null);
		}
		this.menuPart.RenderMenu(21);
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GMenuItem gmenuItem = s as GMenuItem;
			if (null == gmenuItem)
			{
				return;
			}
			this.CloseNoBorderWindow(this.MenuWindow);
			this.MenuWindow = null;
			if (menus_id == 0)
			{
				this.ProcessDropDownMenuClick(gmenuItem.MenuItemID);
			}
			else if (menus_id == 1)
			{
				this.ProcessPropMenuClick(gmenuItem.MenuItemID);
			}
		};
		this.MenuWindow.SetContent(this.MenuWindow.BodyPresenter, this.menuPart, 2.0, 2.0);
	}

	private void ProcessDropDownMenuClick(int id)
	{
		this.SelectedMenuItemID = id;
		this.txtShowItem.EditText = this.MenusItemsNames[id];
		this.FilterMemberDataList(this.SelectedMenuItemID);
		this.RefreshList();
	}

	private void ProcessPropMenuClick(int id)
	{
		if (id == 0)
		{
			GameInstance.Game.SpriteQueryIDByName(this.SelectedListItem.RoleName, 0);
		}
		else if (id == 1)
		{
			if (Global.Data.roleData.RoleID != this.SelectedListItem.RoleID)
			{
				GameInstance.Game.SpriteAddFriend(this.SelectedListItem.RoleID, this.SelectedListItem.RoleName, 0);
			}
		}
		else if (id == 2)
		{
			this.ChangeNickName();
		}
		else if (id == 3)
		{
			if (Global.Data.roleData.RoleID != this.SelectedListItem.RoleID)
			{
				this.ChangeZw();
			}
			else
			{
				Super.HintMainText(Global.GetLang("首领无法修改自己的职务!"), 10, 3);
			}
		}
		else if (id != 4)
		{
			if (id == 5)
			{
			}
		}
	}

	private void FilterMemberDataList(int type)
	{
		this.ItemsList.Clear();
		if (this.MyBangHuiMemberDataList == null)
		{
			return;
		}
		this.MemberListSort();
		for (int i = 0; i < this.MyBangHuiMemberDataList.Count; i++)
		{
			if (type == 0)
			{
				this.ItemsList.Add(this.MyBangHuiMemberDataList[i]);
			}
			else if (type == 1)
			{
				if (this.MyBangHuiMemberDataList[i].OnlineState == 1)
				{
					this.ItemsList.Add(this.MyBangHuiMemberDataList[i]);
				}
			}
			else if (type == 2 && this.MyBangHuiMemberDataList[i].OnlineState == 0)
			{
				this.ItemsList.Add(this.MyBangHuiMemberDataList[i]);
			}
		}
		if (this.mGVoicePriorityData != null)
		{
			string roleIdList = this.mGVoicePriorityData.RoleIdList;
			if (!string.IsNullOrEmpty(roleIdList))
			{
				int roleId = ConvertExt.SafeConvertToInt32(roleIdList);
				BangHuiMemberData bangHuiMemberData = this.ItemsList.Find((BangHuiMemberData result) => result.RoleID == roleId);
				if (bangHuiMemberData != null)
				{
					BangHuiMemberData bangHuiMemberData2 = this.CloneData(bangHuiMemberData);
					int num = this.ItemsList.FindIndex((BangHuiMemberData result) => result.RoleID == roleId);
					this.ItemsList.RemoveAt(num);
					this.ItemsList.Insert(1, bangHuiMemberData2);
				}
			}
		}
	}

	private BangHuiMemberData CloneData(BangHuiMemberData data)
	{
		return new BangHuiMemberData
		{
			ZoneID = data.ZoneID,
			RoleID = data.RoleID,
			RoleName = data.RoleName,
			Occupation = data.Occupation,
			BHZhiwu = data.BHZhiwu,
			ChengHao = data.ChengHao,
			BangGong = data.BangGong,
			Level = data.Level,
			XueWeiNum = data.XueWeiNum,
			SkillLearnedNum = data.SkillLearnedNum,
			OnlineState = data.OnlineState,
			BangHuiMemberCombatForce = data.BangHuiMemberCombatForce,
			BangHuiMemberChangeLifeLev = data.BangHuiMemberChangeLifeLev,
			JunTuanZhiWu = data.JunTuanZhiWu,
			YaoSaiBossState = data.YaoSaiBossState,
			YaoSaiJianYuState = data.YaoSaiJianYuState
		};
	}

	public void NotifyBangHuiMemberDataList(List<BangHuiMemberData> bangHuiMemberDataList)
	{
		this.MyBangHuiMemberDataList = bangHuiMemberDataList;
		this.FilterMemberDataList(this.SelectedMenuItemID);
		this.RefreshList();
	}

	private int ItemsList_Sort4(BangHuiMemberData a, BangHuiMemberData b)
	{
		if (Convert.ToBoolean(this.LevelSortIcon.Tag.ToString()))
		{
			return b.Level - a.Level;
		}
		return a.Level - b.Level;
	}

	public void NotifyBangHuiMemberZhiWu(int retCode, int bhid, int otherRoleID, int zhiWu)
	{
		GameInstance.Game.SpriteQueryBangHuiDetail(bhid);
		if (retCode < 0)
		{
			if (retCode == -1050)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("对方的角色等级小于【{0}】, 无法担任首领"), new object[]
				{
					Global.CreateBangHuiNeedLevel
				}), 10, 3);
			}
			else if (retCode != -201)
			{
				if (retCode == -1006)
				{
					Super.HintMainText(Global.GetLang("罗兰城主持有期间不能委任其他成员为战盟首领"), 10, 3);
				}
				else if (retCode == -1007)
				{
					Super.HintMainText(Global.GetLang("圣域领主持有期间不能委任其他成员为战盟首领"), 10, 3);
				}
				else if (retCode == -1032)
				{
					Super.HintMainText(Global.GetLang("军团拥有领地,不能改变领主"), 10, 3);
				}
				else
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("修改成员的职务时发生错误: {0}"), new object[]
					{
						retCode
					}), 10, 3);
				}
			}
			return;
		}
		if (this.MyBangHuiMemberDataList != null)
		{
			if (zhiWu > 0)
			{
				for (int i = 0; i < this.MyBangHuiMemberDataList.Count; i++)
				{
					if (zhiWu == this.MyBangHuiMemberDataList[i].BHZhiwu)
					{
						this.MyBangHuiMemberDataList[i].BHZhiwu = 0;
						break;
					}
				}
			}
			for (int j = 0; j < this.MyBangHuiMemberDataList.Count; j++)
			{
				if (otherRoleID == this.MyBangHuiMemberDataList[j].RoleID)
				{
					this.MyBangHuiMemberDataList[j].BHZhiwu = zhiWu;
					break;
				}
			}
		}
		if (zhiWu > 0)
		{
			for (int k = 0; k < this.ItemsList.Count; k++)
			{
				if (zhiWu == this.ItemsList[k].BHZhiwu)
				{
					this.ItemsList[k].BHZhiwu = 0;
					break;
				}
			}
		}
		for (int l = 0; l < this.ItemsList.Count; l++)
		{
			if (otherRoleID == this.ItemsList[l].RoleID)
			{
				this.ItemsList[l].BHZhiwu = zhiWu;
				break;
			}
		}
		if (zhiWu > 0)
		{
			for (int m = 0; m < this.ItemCollection.Count; m++)
			{
				MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[m]);
				if (zhiWu == membersInfoListItem.BHZhiWu)
				{
					membersInfoListItem.BHZhiWu = 0;
					membersInfoListItem.Zw = Global.GetBHZhiWu(0);
					break;
				}
			}
		}
		for (int n = 0; n < this.ItemCollection.Count; n++)
		{
			MembersInfoListItem membersInfoListItem2 = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[n]);
			if (otherRoleID == membersInfoListItem2.RoleID)
			{
				membersInfoListItem2.BHZhiWu = zhiWu;
				membersInfoListItem2.Zw = Global.GetBHZhiWu(zhiWu);
				break;
			}
		}
		this.FilterMemberDataList(this.SelectedMenuItemID);
		this.RefreshList();
	}

	public void NotifyBangHuiMemberChengHao(int retCode, int bhid, int otherRoleID, string chengHao)
	{
		if (retCode < 0)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("修改成员的称号时发生错误: {0}"), new object[]
			{
				retCode
			}), 10, 3);
			return;
		}
		if (this.MyBangHuiMemberDataList != null)
		{
			for (int i = 0; i < this.MyBangHuiMemberDataList.Count; i++)
			{
				if (otherRoleID == this.MyBangHuiMemberDataList[i].RoleID)
				{
					this.MyBangHuiMemberDataList[i].ChengHao = chengHao;
					break;
				}
			}
		}
		for (int j = 0; j < this.ItemsList.Count; j++)
		{
			if (otherRoleID == this.ItemsList[j].RoleID)
			{
				this.ItemsList[j].ChengHao = chengHao;
				break;
			}
		}
		for (int k = 0; k < this.ItemCollection.Count; k++)
		{
			MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[k]);
			if (otherRoleID == membersInfoListItem.RoleID)
			{
				break;
			}
		}
	}

	public void NotifyRemoveBangHuiMember(int retCode, int bhid, int otherRoleID)
	{
		if (retCode < 0)
		{
			if (retCode == -1001)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("开除成员时发生错误, 战盟已经不存在"), new object[0]), 10, 3);
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("开除成员时发生错误: {0}"), new object[]
				{
					retCode
				}), 10, 3);
			}
			return;
		}
		if (this.MyBangHuiMemberDataList != null)
		{
			for (int i = 0; i < this.MyBangHuiMemberDataList.Count; i++)
			{
				if (otherRoleID == this.MyBangHuiMemberDataList[i].RoleID)
				{
					this.MyBangHuiMemberDataList.RemoveRange(i, 1);
					break;
				}
			}
		}
		for (int j = 0; j < this.ItemsList.Count; j++)
		{
			if (otherRoleID == this.ItemsList[j].RoleID)
			{
				this.ItemsList.RemoveRange(j, 1);
				break;
			}
		}
		for (int k = 0; k < this.ItemCollection.Count; k++)
		{
			MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.ItemCollection[k]);
			if (otherRoleID == membersInfoListItem.RoleID)
			{
				this.ItemCollection.RemoveAt(k);
				break;
			}
		}
	}

	public void RefreshSearchData(List<SearchRoleData> searchRoleDataList)
	{
		this.AddMember(searchRoleDataList);
	}

	private void SearchPlayers(string key)
	{
		if (key.Length < 2)
		{
			Super.HintMainText(Global.GetLang("要搜索的关键字不能少于2个字"), 10, 3);
			return;
		}
		if (key.Length > 10)
		{
			Super.HintMainText(Global.GetLang("要搜索的关键字不能超过7个字"), 10, 3);
			return;
		}
		string text = Global.StringReplaceAll(key, "'", string.Empty);
		text = Global.StringReplaceAll(text, "|", string.Empty);
		text = Global.StringReplaceAll(text, "$", string.Empty);
		text = Global.StringReplaceAll(text, ":", string.Empty);
		Global.SendEvent("702", Global.GetLang("添加成员次数"));
		GameInstance.Game.SpriteSearchRolesFromDB(text, 0);
	}

	private void AddMember(List<SearchRoleData> searchRoleDataList)
	{
		if (searchRoleDataList.Count > 0)
		{
			int num = 0;
			for (int i = 0; i < searchRoleDataList.Count; i++)
			{
				if (searchRoleDataList[i].RoleName == this.inputMemberName.text)
				{
					num = i;
					break;
				}
			}
			if (searchRoleDataList[num].RoleID != Global.Data.roleData.RoleID)
			{
				if (searchRoleDataList[num].Level < Global.JoinBangHuiNeedLevel)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("【{0}】已经是其他战盟的成员，无法再邀请!"), new object[]
					{
						searchRoleDataList[num].RoleName
					}), 10, 3);
					return;
				}
				if (searchRoleDataList[num].Level < Global.JoinBangHuiNeedLevel)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("角色等级小于【{0}】的无法成为战盟成员"), new object[]
					{
						Global.JoinBangHuiNeedLevel
					}), 10, 3);
					return;
				}
				if (Global.GetBangHuiZhiWuByRoleID(Global.Data.roleData, this.MyBangHuiDetailData) > 0)
				{
					GameInstance.Game.SpriteAddBHMember(Global.Data.roleData.Faction, searchRoleDataList[num].RoleID, searchRoleDataList[num].RoleName, 1);
					this.addMemberWin.SetActive(false);
				}
				else
				{
					Super.HintMainText(Global.GetLang("普通成员无法执行添加成员的操作!"), 10, 3);
				}
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("用户不存在!"), 10, 3);
		}
	}

	private void MemberListSort()
	{
		if (this.MyBangHuiMemberDataList != null)
		{
			List<BangHuiMemberData> list = new List<BangHuiMemberData>();
			for (int i = 0; i < this.MyBangHuiMemberDataList.Count; i++)
			{
				if (this.MyBangHuiMemberDataList[i].BHZhiwu != 0)
				{
					list.Add(this.MyBangHuiMemberDataList[i]);
					this.MyBangHuiMemberDataList.RemoveAt(i--);
				}
			}
			list.Sort((BangHuiMemberData a, BangHuiMemberData b) => a.BHZhiwu - b.BHZhiwu);
			this.MyBangHuiMemberDataList.Sort((BangHuiMemberData a, BangHuiMemberData b) => b.Level - a.Level);
			this.MyBangHuiMemberDataList.Sort(delegate(BangHuiMemberData a, BangHuiMemberData b)
			{
				if (a.BangHuiMemberChangeLifeLev == b.BangHuiMemberChangeLifeLev)
				{
					return b.Level - a.Level;
				}
				return b.BangHuiMemberChangeLifeLev - a.BangHuiMemberChangeLifeLev;
			});
			this.MyBangHuiMemberDataList.InsertRange(0, list);
		}
	}

	public void OnClick()
	{
		MembersInfoListItem membersInfoListItem = U3DUtils.AS<MembersInfoListItem>(this.listBox.SelectedItem);
		if (null != membersInfoListItem)
		{
			membersInfoListItem.SelectedState = false;
		}
	}

	public void DPSelectItemHandlerQuitBH(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			GameInstance.Game.SpriteQuitFromBangHui(Global.Data.roleData.Faction);
		}
	}

	public void DPSelectItemHandlerDismissBH(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			Global.SendEvent("703", Global.GetLang("解散战盟次数"));
			GameInstance.Game.SpriteDestroyBangHui(Global.Data.roleData.Faction);
			if (this.CloseMemberWindow != null)
			{
				this.CloseMemberWindow(this, null);
			}
		}
	}

	public GButton btnQuite;

	public GButton btnAddMember;

	public GButton btnDismiss;

	public GButton btnClose;

	public GCheckBox chkAutoVerify;

	public ListBox listBox;

	public GButton btnAllMember;

	public GButton btnOnline;

	public GButton btnOffline;

	public UILabel lblFilter;

	public GameObject addMemberWin;

	public GButton btnSubmitMemberInfo;

	public GButton btnCloseAddMemberWin;

	public UIInput inputMemberName;

	public TextBlock ConstHint;

	public TextBlock ConstTitle;

	private ChangeZwPart changeZwPart;

	public UIScrollBar scrollBar;

	private int CurrentSelectedPage;

	private int MaxPageCount;

	private GIcon JnSortIcon;

	private GIcon JmSortIcon;

	private GIcon LevelSortIcon;

	private GTextBlock txtShowItem;

	private int SelectedMenuItemID;

	private GMenuPart menuPart;

	private NoBorderWindow MenuWindow;

	private int[] MenuItemsIDs = new int[]
	{
		default(int),
		1,
		2
	};

	private string[] MenusItemsNames = new string[]
	{
		Global.GetLang("所有成员"),
		Global.GetLang("在线成员"),
		Global.GetLang("离线成员")
	};

	private int[] PlayerListMenuItemIDs = new int[]
	{
		0,
		1,
		2,
		3,
		4,
		5
	};

	private string[] PlayerListMenuItemNames = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("加为好友"),
		Global.GetLang("更改称号"),
		Global.GetLang("更改职务"),
		Global.GetLang("私聊"),
		Global.GetLang("发送邮件")
	};

	private int[] PlayerListMenuItemIDs_1 = new int[]
	{
		0,
		1,
		2,
		4,
		5
	};

	private string[] PlayerListMenuItemNames_1 = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("加为好友"),
		Global.GetLang("更改称号"),
		Global.GetLang("私聊"),
		Global.GetLang("发送邮件")
	};

	private int[] PlayerListMenuItemIDs_2 = new int[]
	{
		default(int),
		1,
		4,
		5
	};

	private string[] PlayerListMenuItemNames_2 = new string[]
	{
		Global.GetLang("查看装备"),
		Global.GetLang("加为好友"),
		Global.GetLang("私聊"),
		Global.GetLang("发送邮件")
	};

	private LoadingWindow LoadingWin;

	private List<BangHuiMemberData> ItemsList = new List<BangHuiMemberData>();

	private BangHuiDetailData MyBangHuiDetailData;

	private Canvas PlaceHolder;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	private MembersInfoListItem SelectedListItem;

	private Canvas Root;

	public DPSelectedItemEventHandler ShowChatBox;

	public DPSelectedItemEventHandler CloseMemberWindow;

	public TextBlock mLblSpeechMembers;

	public int sumSpeechCount = 2;

	private List<BangHuiMemberData> MyBangHuiMemberDataList;

	private ObservableCollection _ItemCollection;

	private GVoicePriorityData mGVoicePriorityData;
}
