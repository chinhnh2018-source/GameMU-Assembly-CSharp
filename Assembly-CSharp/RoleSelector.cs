using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RoleSelector : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.UserControl_Loaded(this);
		this.VectorGaiMing = this.m_BtnGaiMing.transform.localPosition;
		this.m_GaiMing.text = Global.GetLang("改名");
		this.EnterGameBtn.Text = Global.GetLang("进入游戏");
		if (this.Is3DBackground)
		{
			base.StartCoroutine<bool>(this.Init3DMap());
		}
		this.BtnShanChu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DeleteRoleBtn_MouseLeftButtonUp(null, null);
		};
		UIEventListener.Get(this.GoBackBtn.gameObject).onClick = delegate(GameObject s)
		{
			if (this.GoBackEvent != null)
			{
				this.GoBackEvent.Invoke(this, EventArgs.Empty);
			}
		};
		UIEventListener.Get(this.m_BtnGaiMing.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.GetLang("恢复角色") == this.m_GaiMing.text)
			{
				this.ShanChuJueSeUndo_MouseLeftButtonUp(s, null);
			}
			else
			{
				GaiMingPart gaiMingPart = U3DUtils.NEW<GaiMingPart>();
				gaiMingPart.transform.parent = base.transform;
				if (null != this.listBox.SelectedItem)
				{
					RoleSelectorListItem component = this.listBox.SelectedItem.GetComponent<RoleSelectorListItem>();
					if (null != component)
					{
						int roleID = component.RoleID;
						if (Global.HasSecondPassword == 1 && Global.NeedVerifySecondPassword == 1)
						{
							Super.ShowVerifySecondPasswordWindow(roleID);
						}
						gaiMingPart.roleid = roleID;
						gaiMingPart.zoneid = Global.Data.GameServerID;
						gaiMingPart.transform.localPosition = new Vector3(0f, 0f, -50f);
						gaiMingPart.transform.localScale = new Vector3(1f, 1f, 1f);
						gaiMingPart.listBox = this.listBox;
						GaiMingPart.Instance.GaiMingInfo(GaiMingPart._ChangeNameInfo);
					}
				}
			}
		};
		this.ShanChuJueSeCD.SetActive(false);
		if (Context.IsHaiwai)
		{
			this.BtnShanChu.Text = Global.GetLang("删除角色");
			this.EnterGameBtn.Text = Global.GetLang("进入游戏");
		}
	}

	private void ReleaseResLoader()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
			this.wingsResLoader = null;
		}
		if (this.shouHuChongResLoader != null)
		{
			this.shouHuChongResLoader.Stop();
			this.shouHuChongResLoader = null;
		}
		if (this.weaponResLoader != null)
		{
			this.weaponResLoader.Stop();
			this.weaponResLoader = null;
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
		if (null != this.ShowText)
		{
			this.ShowText.Destroy();
			this.ShowText = null;
		}
		if (null != RoleManager.DecoBackground)
		{
			Object.Destroy(RoleManager.DecoBackground);
			RoleManager.DecoBackground = null;
		}
		this.ReleaseResLoader();
		if (null != this.CurrentLoader)
		{
			this.CurrentLoader.Unload(true);
			this.CurrentLoader = null;
		}
	}

	private void UserControl_Loaded(UserControl sender)
	{
		this.ItemCollection = this.listBox.Items;
		this.listBox.ForceSelectChanged = true;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.DeleteRoleBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.DeleteRoleBtn_MouseLeftButtonUp);
		this.CreateRoleBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CreateRoleBtn_MouseLeftButtonUp);
		this.EnterGameBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EnterGameBtn_MouseLeftButtonUp);
		if (Global.Data != null && Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null)
		{
			this.ServerName.text = Global.Data.ServerData.LastServer.strServerName;
		}
		else
		{
			this.ServerName.text = Global.GetLang("全民奇迹");
		}
	}

	private void DeleteRoleBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (null == this.listBox.SelectedItem)
		{
			return;
		}
		int num = this.ListRoleIDs.IndexOf(U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem).RoleID);
		if (num < 0)
		{
			return;
		}
		string strName = this.ListRoleNames[num];
		this.m_DeleteRoleInfo = new RoleInfo();
		this.m_DeleteRoleInfo.nID = this.ListRoleIDs[num];
		this.m_DeleteRoleInfo.strName = strName;
		this.m_DeleteRoleInfo.nLevel = this.ListRoleLevels[num];
		this.m_DeleteRoleInfo.OccupType = this.ListRoleOccups[num];
		this.m_DeleteRoleInfo.nChangeLifeCount = this.ListRoleChangeLifeCount[num];
		if (Global.HasSecondPassword == 1 && Global.NeedVerifySecondPassword == 1)
		{
			Global.VerifySuccess = new Action(this.VerifySuccessDeleteRole);
		}
		this.VerifySecondPasswordInfo(this.ListRoleIDs[num], new Action(this.VerifySuccessDeleteRole));
	}

	private void VerifySuccessDeleteRole()
	{
		this.DeleteRoleBtnUp.Invoke(this, new EventArgs());
	}

	private void CreateRoleBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ListRoleIDs.Count >= 4)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("一个注册用户最多只能创建4个用户角色!"), -1, -1, -1, -1, false);
			return;
		}
		if (this.RolePanelChanged != null)
		{
			this.Hide3DObjects();
			this.RolePanelChanged.Invoke(this, EventArgs.Empty);
		}
	}

	private void ShanChuJueSeUndo_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		long num = 0L;
		if (null != this.listBox.SelectedItem)
		{
			num = U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem).DeleteDeltaTicks;
		}
		if (num <= 0L)
		{
			return;
		}
		int roleID = U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem).RoleID;
		GameInstance.Game.CancelUnRemoveRole(roleID);
	}

	private void EnterGameBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (null == this.listBox.SelectedItem)
		{
			return;
		}
		int num = this.ListRoleIDs.IndexOf(U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem).RoleID);
		if (num < 0)
		{
			return;
		}
		GameInstance.Game.CurrentSession.RoleID = this.ListRoleIDs[num];
		GameInstance.Game.CurrentSession.LocalRoleID = this.ListRoleIDs[num];
		GameInstance.Game.CurrentSession.RoleSex = this.ListRoleSexes[num];
		GameInstance.Game.CurrentSession.RoleName = this.ListRoleNames[num];
		if (Global.HasSecondPassword == 1 && Global.NeedVerifySecondPassword == 1)
		{
			Global.VerifySuccess = new Action(this.EnterGame);
		}
		this.VerifySecondPasswordInfo(GameInstance.Game.CurrentSession.RoleID, new Action(this.EnterGame));
	}

	private void VerifySecondPasswordInfo(int roleID, Action noVerifyCallBack)
	{
		if (Global.HasSecondPassword == 0)
		{
			if (noVerifyCallBack != null)
			{
				noVerifyCallBack.Invoke();
			}
		}
		else if (Global.HasSecondPassword == 1)
		{
			if (Global.NeedVerifySecondPassword == 0)
			{
				if (noVerifyCallBack != null)
				{
					noVerifyCallBack.Invoke();
				}
			}
			else
			{
				Super.ShowVerifySecondPasswordWindow(roleID);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"Global.HasSecondPassword值不正常:" + Global.HasSecondPassword
			});
		}
	}

	private void EnterGame()
	{
		Global.PlaySoundAudio("Audio/UI/dropbaoshi", false);
		Super.ShowNetWaiting(Global.GetLang("正在进入游戏..."));
		GameInstance.Game.InitPlayGame();
		Global.RoleCreate3DBakMapWWW = null;
		if (Global.RoleCreate3DBakMapLoader != null)
		{
			Global.RoleCreate3DBakMapLoader.Unload(true);
			Global.RoleCreate3DBakMapLoader = null;
		}
		if (Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null)
		{
			PlatformUserLogin.RecordLoginServerIDs(Global.Data.ServerData.LastServer);
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		RoleSelectorListItem roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem);
		if (this.itemTemp == roleSelectorListItem && 0 < this.ListRoleIDs.Count)
		{
			return;
		}
		this.itemTemp = roleSelectorListItem;
		if (null != roleSelectorListItem && roleSelectorListItem.HintToCreate)
		{
			if (null != this.listBox.LastSelectedItem)
			{
				roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.LastSelectedItem);
				if (null != roleSelectorListItem && !roleSelectorListItem.HintToCreate)
				{
					this.prepareToCreatePanel = true;
					this.listBox.DoSelectItem(this.listBox.LastSelectedItem);
					this.prepareToCreatePanel = false;
				}
			}
			this.CreateRoleBtn_MouseLeftButtonUp(null, null);
			return;
		}
		if (null != this.listBox.LastSelectedItem)
		{
			roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.LastSelectedItem);
			if (null != roleSelectorListItem && !roleSelectorListItem.HintToCreate)
			{
				roleSelectorListItem.ItemSelected = false;
			}
		}
		roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem);
		if (null == roleSelectorListItem)
		{
			return;
		}
		roleSelectorListItem.ItemSelected = true;
		roleSelectorListItem.ChangeImage(string.Format("NetImages/RS_Face/{0}.png", Global.CalcOriginalOccupationID(roleSelectorListItem.Occupation)));
		this.TextImg.ShowImage(StringUtil.substitute("NetImages/Roles/info_bg_{0}.png", new object[]
		{
			roleSelectorListItem.Occupation
		}));
		this.TextZhiYe.spriteName = StringUtil.substitute("info_{0}", new object[]
		{
			roleSelectorListItem.Occupation
		});
		if (roleSelectorListItem.Occupation == 0)
		{
			this.TextZhiYe.transform.localScale = new Vector3(62f, 33f, 1f);
		}
		else
		{
			this.TextZhiYe.transform.localScale = new Vector3(93f, 33f, 1f);
		}
		this.TextContent.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fff2d0",
			roleSelectorListItem.TextBlockName.Text
		});
		string text = Global.GetLang("全民奇迹");
		if (Global.Data != null && Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null)
		{
			text = Global.Data.ServerData.LastServer.strServerName;
		}
		this.TextTitle.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"bfb494",
			roleSelectorListItem.TextBlockLevel.Text + Environment.NewLine + text
		});
		if (!this.prepareToCreatePanel)
		{
			Super.ShowNetWaiting(Global.GetLang("正在获取角色数据..."));
			GameInstance.Game.SpriteGetUsingGoodsDataList(roleSelectorListItem.RoleID, true);
		}
	}

	public void AddRoleList(string s, bool canChangeIndex = true)
	{
		if (s == string.Empty)
		{
			return;
		}
		string[] array = s.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				'$'
			});
			if (array2.Length >= 5)
			{
				this.ListRoleIDs.Add(Convert.ToInt32(array2[0]));
				this.ListRoleSexes.Add(Convert.ToInt32(array2[1]));
				this.ListRoleOccups.Add(Convert.ToInt32(array2[2]));
				this.ListRoleNames.Add(array2[3]);
				this.ListRoleLevels.Add(Convert.ToInt32(array2[4]));
				this.ListRoleChangeLifeCount.Add(Convert.ToInt32(array2[5]));
				if (array2.Length >= 7)
				{
					string text = array2[6];
					if (string.IsNullOrEmpty(text))
					{
						this.ListRoleDeltimeTicks.Add(0L);
					}
					else
					{
						long num = (long)int.Parse(text);
						this.ListRoleDeltimeTicks.Add(num);
					}
				}
				else
				{
					this.ListRoleDeltimeTicks.Add(0L);
				}
			}
		}
		if (canChangeIndex)
		{
			this.ShowRoleListBox(-1);
		}
		this.StartTimer();
	}

	private DateTime SafeConvertDateTime(string str)
	{
		int[] array = new int[6];
		string[] array2 = str.Split(new char[]
		{
			' '
		});
		if (array2.Length == 2)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				if (i == 0)
				{
					string[] array3 = array2[i].Split(new char[]
					{
						'/'
					});
					if (array3.Length == 3)
					{
						for (int j = 0; j < array3.Length; j++)
						{
							array[j] = int.Parse(array3[j]);
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"服务器传的时间格式有误"
						});
					}
				}
				else if (i == 1)
				{
					string[] array4 = array2[i].Split(new char[]
					{
						'#'
					});
					if (array4.Length == 3)
					{
						for (int k = 0; k < array4.Length; k++)
						{
							array[k + 3] = int.Parse(array4[k]);
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"服务器传的时间格式有误"
						});
					}
				}
			}
		}
		string text = string.Format("{0}/{1}/{2} {3}:{4}:{5}", new object[]
		{
			array[0],
			array[1],
			array[2],
			array[3],
			array[4],
			array[5]
		});
		DateTime result;
		if (DateTime.TryParse(text, ref result))
		{
			return result;
		}
		return default(DateTime);
	}

	public void RemoveRole(string id)
	{
		if (id == string.Empty)
		{
			return;
		}
		if (id == "-20")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("你尚未验证二级密码"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-21")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("请离婚后再删除角色"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-4029")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("战队队长禁止删除角色"), -1, -1, -1, -1, false);
			return;
		}
		int num = Convert.ToInt32(id);
		for (int i = 0; i < this.listBox.Items.Count; i++)
		{
			RoleSelectorListItem roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.getChildAt(i));
			if (i < this.ListRoleDeltimeTicks.Count)
			{
				this.ListRoleDeltimeTicks[i] = roleSelectorListItem.DeleteDeltaTicks;
			}
		}
		for (int j = 0; j < this.ListRoleIDs.Count; j++)
		{
			if (num == this.ListRoleIDs[j])
			{
				this.ListRoleIDs.RemoveRange(j, 1);
				this.ListRoleSexes.RemoveRange(j, 1);
				this.ListRoleNames.RemoveRange(j, 1);
				this.ListRoleOccups.RemoveRange(j, 1);
				this.ListRoleLevels.RemoveRange(j, 1);
				this.ListRoleChangeLifeCount.RemoveRange(j, 1);
				this.ListRoleDeltimeTicks.RemoveRange(j, 1);
				if (j > 0)
				{
					j--;
				}
				this.ShowRoleListBox(j);
				break;
			}
		}
	}

	public void CancelUnRemoveRole(string id)
	{
		int num = Convert.ToInt32(id);
		for (int i = 0; i < this.ListRoleIDs.Count; i++)
		{
			if (num == this.ListRoleIDs[i])
			{
				RoleSelectorListItem roleById = this.GetRoleById(num);
				roleById.DeleteDeltaTicks = -1L;
				break;
			}
		}
	}

	public bool ZHSCreatorCondition()
	{
		int count = this.ListRoleChangeLifeCount.Count;
		if (0 < count)
		{
			for (int i = 0; i < count; i++)
			{
				if (3 <= this.ListRoleChangeLifeCount[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	public void UnRemoveRole(string id, string time)
	{
		if (id == string.Empty)
		{
			return;
		}
		if (id == "-20")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("你尚未验证二级密码"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-21")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("请离婚后再删除角色"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-22")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("占领领地的军团长禁止删除角色"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-23")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("军团战盟首领禁止删除角色"), -1, -1, -1, -1, false);
			return;
		}
		if (id == "-4029")
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("战队队长禁止删除角色"), -1, -1, -1, -1, false);
			return;
		}
		if (!string.IsNullOrEmpty(time))
		{
			int num = Convert.ToInt32(id);
			for (int i = 0; i < this.ListRoleIDs.Count; i++)
			{
				if (num == this.ListRoleIDs[i])
				{
					int num2 = 0;
					if (!int.TryParse(time, ref num2))
					{
						string systemParamByName = ConfigSystemParam.GetSystemParamByName("DeleteRoleNeedTime", true);
						num2 = Convert.ToInt32(systemParamByName);
					}
					RoleSelectorListItem roleById = this.GetRoleById(num);
					roleById.DeleteDeltaTicks = (long)num2;
					break;
				}
			}
		}
	}

	private RoleSelectorListItem GetRoleById(int id)
	{
		ObservableCollection items = this.listBox.Items;
		for (int i = 0; i < items.Count; i++)
		{
			RoleSelectorListItem roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(items.GetAt(i));
			if (roleSelectorListItem.RoleID == id)
			{
				return roleSelectorListItem;
			}
		}
		return null;
	}

	public int GetRolesCount()
	{
		return this.ListRoleIDs.Count;
	}

	public void StartGame()
	{
		if (this.StartGameByRole != null)
		{
			this.StartGameByRole.Invoke(this, EventArgs.Empty);
		}
	}

	public void DirectEnterGame(bool showWaiting = true)
	{
		GameInstance.Game.CurrentSession.RoleID = this.ListRoleIDs[this.ListRoleIDs.Count - 1];
		GameInstance.Game.CurrentSession.LocalRoleID = this.ListRoleIDs[this.ListRoleIDs.Count - 1];
		GameInstance.Game.CurrentSession.RoleSex = this.ListRoleSexes[this.ListRoleIDs.Count - 1];
		GameInstance.Game.CurrentSession.RoleName = this.ListRoleNames[this.ListRoleIDs.Count - 1];
		Global.ClearFashionAndTitleData();
		HuoDongCommonFlag.ClearStaticData();
		if (showWaiting)
		{
			Super.ShowNetWaiting(Global.GetLang("正在进入游戏..."));
		}
		GameInstance.Game.InitPlayGame();
		EventTracer.CreateRole();
		EventTracer.EnterGame("EnterGame");
		EventTracer.SetRoleId(GameInstance.Game.CurrentSession.RoleID);
	}

	public void GetRoleUsingGoodsDataList(RoleData4Selector roleData4Selector)
	{
		this.ClearLastRole();
		if (roleData4Selector == null || roleData4Selector.RoleID < 0)
		{
			return;
		}
		this.ClearLastRole();
		Super.ShowNetWaiting(Global.GetLang("加载角色..."));
		int fashionGoodsID = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
		this.LoadRoleRes(roleData4Selector.Occupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, roleData4Selector.MyWingData, fashionGoodsID, 0L, roleData4Selector.SubOccupation);
		Super.HideNetWaiting();
		for (int i = 0; i < this.ZhuanZhiBtns.Length; i++)
		{
			this.ZhuanZhiBtns[i].gameObject.SetActive(false);
		}
		if (roleData4Selector.OccupationList == null || roleData4Selector.OccupationList.Count <= 1)
		{
			return;
		}
		double num = 0.0;
		for (int j = 0; j < roleData4Selector.OccupationList.Count; j++)
		{
			this.ZhuanZhiBtns[j].gameObject.SetActive(true);
			this.ZhuanZhiBtns[j].occupation = roleData4Selector.OccupationList[j];
			float num2;
			if (roleData4Selector.OccupationList.Count <= 2)
			{
				num2 = (float)(-109.0 + 59.0 * (num + 1.0));
				num += 1.06;
			}
			else if (roleData4Selector.OccupationList.Count <= 3)
			{
				num2 = (float)(-109.0 + 59.0 * (num + 1.0));
				num += 1.06;
			}
			else
			{
				num2 = (float)(-109 + 59 * j);
			}
			this.ZhuanZhiBtns[j].transform.localPosition = new Vector3(num2, -220f, 0f);
			if (roleData4Selector.Occupation == roleData4Selector.OccupationList[j])
			{
				this.ZhuanZhiBtns[j].GetComponentInChildren<UISprite>().spriteName = "rolePhotoBorder_hover";
			}
			else
			{
				this.ZhuanZhiBtns[j].GetComponentInChildren<UISprite>().spriteName = "rolePhotoBorder_normal";
			}
			if (j == 0)
			{
				this.ZhuanZhiBtns[j].zhiye.spriteName = "zhuzhiye";
			}
			else
			{
				this.ZhuanZhiBtns[j].zhiye.spriteName = "fuzhiye";
			}
		}
	}

	private void ClearLastRole()
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
		}
		this.DestroyShouHuChong();
	}

	private void LoadRoleRes(int occupation, string roleName, List<GoodsData> goodsDataList, WingData MyWingData, int FashionWingGoodsID, long SettingBitFlags, int Suboccupation)
	{
		int myTimer = Global.GetMyTimer();
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		this.DestroyShouHuChong();
		string skeletonNameByOccupation = Global.GetSkeletonNameByOccupation(occupation);
		GameObject gameObject = U3DUtils.LoadSkeletonByName(skeletonNameByOccupation, false);
		myTimer = Global.GetMyTimer();
		if (null != gameObject)
		{
			this.oldRole = gameObject;
			string[] nakePartsList = Global.GetNakePartsList(occupation);
			GameObject parent = gameObject;
			RoleLoaderData roleLoaderData = new RoleLoaderData();
			roleLoaderData.parent = parent;
			roleLoaderData.ForceSyncLoad = false;
			roleLoaderData.SubOccupation = Suboccupation;
			roleLoaderData.GoodsDataList = new List<GoodsData>();
			List<GoodsData> list = new List<GoodsData>();
			if (goodsDataList != null)
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					int goodsID = goodsDataList[i].GoodsID;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
					if ((goodsXmlNodeByID.Categoriy >= 11 && goodsXmlNodeByID.Categoriy <= 21) || goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
					{
						list.Add(Global.GetFakeEquipGoodsData(goodsDataList[i].GoodsID, goodsDataList[i].Forge_level, goodsDataList[i].BagIndex));
					}
				}
				byte b = 0;
				int j = 0;
				int count = goodsDataList.Count;
				while (j < count)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList[j].GoodsID);
					if (categoriyByGoodsID == 24)
					{
						roleLoaderData.GoodsDataList.AddRange(Global.GetFashionEquipGoodsDataList(goodsDataList[j].GoodsID, occupation, goodsDataList[j].Forge_level));
						roleLoaderData.GoodsDataList.AddRange(list);
						b = 1;
						break;
					}
					j++;
				}
				if (b == 0)
				{
					roleLoaderData.GoodsDataList.AddRange(goodsDataList);
				}
				else
				{
					int k = 0;
					int count2 = goodsDataList.Count;
					while (k < count2)
					{
						int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsDataList[k].GoodsID);
						if (categoriyByGoodsID2 == 26 || categoriyByGoodsID2 == 25 || categoriyByGoodsID2 == 8)
						{
							roleLoaderData.GoodsDataList.Add(goodsDataList[k]);
						}
						k++;
					}
				}
			}
			roleLoaderData.SkeletonName = skeletonNameByOccupation;
			roleLoaderData.DefaultPartNames = nakePartsList;
			roleLoaderData.Occupation = occupation;
			roleLoaderData.wingData = MyWingData;
			roleLoaderData.FashionWingGoodsID = FashionWingGoodsID;
			roleLoaderData.LoadRebitrhEquit = 0;
			if (this.roleResLoader != null)
			{
				this.roleResLoader.Stop();
			}
			this.roleResLoader = new RoleResLoader(roleLoaderData, new OnLoadRoleComplete(this.RoleLoaderComplete));
		}
	}

	public void RoleLoaderComplete(RoleLoaderData loader, GameObject go)
	{
		Object.Destroy(this.oldRole);
		this.oldRole = null;
		this.DestroyShouHuChong();
		go.transform.localPosition = new Vector3(-0.0169144f, 0.1611748f, -6.948329f);
		go.transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
		GoodsData goodsData = null;
		string text = null;
		List<GoodsData> goodsDataList = loader.GoodsDataList;
		if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text))
		{
			Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text, loader.Occupation);
		}
		if (loader.FashionWingGoodsID > 0)
		{
			if (goodsData == null)
			{
				int @using = loader.wingData.Using;
				loader.wingData.Using = 1;
				if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text))
				{
					Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text, loader.Occupation);
				}
				loader.wingData.Using = @using;
			}
			if (goodsData != null)
			{
				goodsData.GoodsID = loader.FashionWingGoodsID;
				goodsData.Using = 1;
			}
		}
		if (goodsData != null)
		{
			this.wingsResLoader = new WingsResLoader(new WingsLoadData
			{
				data = goodsData,
				hangPointName = text,
				parent = go
			}, new OnWingsLoadComplete(this.WingsLoaderComplete));
		}
		ShouHuChongLoadData shouHuChongLoadData = new ShouHuChongLoadData();
		shouHuChongLoadData.parent = go;
		shouHuChongLoadData.Occupation = loader.Occupation;
		GoodsData goodsData2 = null;
		text = null;
		List<GoodsData> goodsDataList2 = loader.GoodsDataList;
		Global.ParseShouHuChongGoodsDataInfo(goodsDataList2, out goodsData2, out text);
		shouHuChongLoadData.data = goodsData2;
		shouHuChongLoadData.EmptyName = text;
		if (goodsData2 != null)
		{
			shouHuChongLoadData.Categoriy = (ItemCategories)Global.GetCategoriyByGoodsID(goodsData2.GoodsID);
		}
		shouHuChongLoadData.SpecialGameObjectsComplete = new AssetbundleLoaderComplete(this.ShouHuChongLoaderSpecialGameObjectsComplete);
		this.shouHuChongResLoader = new ShouHuChongResLoader(shouHuChongLoadData, new OnShouHuChongLoadComplete(this.ShouHuChongLoaderComplete));
		List<GoodsData> goodsDataList3 = loader.GoodsDataList;
		List<GoodsData> list = new List<GoodsData>();
		List<string> list2 = new List<string>();
		List<string> safeEmptyNamesList = new List<string>();
		int num = Global.CheckRoleOcc(loader.Occupation, loader.SubOccupation);
		Global.ParseWeaponGoodsDataInfo(goodsDataList3, list, list2, safeEmptyNamesList, num);
		this.weaponResLoader = new WeaponResLoader(new WeaponLoadData
		{
			parent = go,
			occupation = num,
			weaponList = list,
			hangPointList = list2
		}, new OnWeaponLoadComplete(this.WeaponLoaderComplete));
		this.oldRole = go;
		go.AddComponent<SpinWithFinger>();
		if (null != this.oldRole)
		{
			WeaponStates weaponState = Global.CalcWeaponState(list, null, num);
			string text2 = string.Empty;
			PlayRoleActions playRoleActions = this.oldRole.AddComponent<PlayRoleActions>();
			byte b = 0;
			List<GoodsData> list3 = new List<GoodsData>();
			for (int i = goodsDataList3.Count - 1; i >= 0; i--)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList3[i].GoodsID);
				if (categoriyByGoodsID == 25 && 0 < goodsDataList3[i].Using)
				{
					WuQiShiZhuangMoXingVO wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion = ConfigFashion.GetWuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion(goodsDataList3[i].GoodsID, num);
					if (wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion != null)
					{
						GoodsData goodsData3 = goodsDataList3[i].Clone();
						GoodsData goodsData4 = goodsDataList3[i].Clone();
						goodsData3.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Left;
						goodsData3.Id = -1;
						goodsData4.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Right;
						goodsData4.Id = -1;
						goodsData4.BagIndex = 1;
						if (0 < goodsData3.GoodsID)
						{
							list3.Add(goodsData3);
							b += 1;
						}
						if (0 < goodsData4.GoodsID)
						{
							list3.Add(goodsData4);
							b += 1;
						}
						break;
					}
				}
			}
			if (0 >= b)
			{
				for (int j = 0; j < goodsDataList3.Count; j++)
				{
					int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsDataList3[j].GoodsID);
					if (11 <= categoriyByGoodsID2 && 21 >= categoriyByGoodsID2)
					{
						list3.Add(goodsDataList3[j]);
					}
				}
			}
			text2 = Global.GetSpecialSkillActionName(loader.Occupation, text2, weaponState, out playRoleActions.StandName, list3);
			playRoleActions.AttackName = text2;
			playRoleActions.Occupation = loader.Occupation;
			string playingMusicFile = string.Empty;
			if (Global.CalcOriginalOccupationID(loader.Occupation) == 0)
			{
				playingMusicFile = "Audio/RoleSelect/zs.mp3";
			}
			else if (Global.CalcOriginalOccupationID(loader.Occupation) == 1)
			{
				playingMusicFile = "Audio/RoleSelect/fs.mp3";
			}
			else if (Global.CalcOriginalOccupationID(loader.Occupation) == 2)
			{
				playingMusicFile = "Audio/RoleSelect/gs.mp3";
			}
			else if (Global.CalcOriginalOccupationID(loader.Occupation) == 3)
			{
				if (Global.GetMJSType(loader.GoodsDataList, Global.CalcOriginalOccupationID(loader.Occupation), 0) == MJSSkillType.Magic_Sword)
				{
					playingMusicFile = "Audio/RoleSelect/mjs_F.mp3";
				}
				else
				{
					playingMusicFile = "Audio/RoleSelect/mjs_Z.mp3";
				}
			}
			else if (Global.CalcOriginalOccupationID(loader.Occupation) == 5)
			{
				playingMusicFile = "Audio/RoleSelect/zhs.mp3";
			}
			Global.PlaySoundAudio(playingMusicFile, false);
		}
	}

	public void WingsLoaderComplete(WingsLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		if (null == go)
		{
			return;
		}
		GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointName);
		if (null == gameObject)
		{
			Object.Destroy(go);
			return;
		}
		U3DUtils.AddChild(gameObject, go, true);
	}

	public void DestroyShouHuChong()
	{
		if (this.shouHuChongController != null)
		{
			this.shouHuChongController.Dispose();
			Object.Destroy(this.shouHuChongController);
			this.shouHuChongController = null;
		}
	}

	public void ShouHuChongLoaderComplete(ShouHuChongLoadData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		if (this.shouHuChongController != null)
		{
			this.shouHuChongController.Dispose();
			Object.Destroy(this.shouHuChongController);
			this.shouHuChongController = null;
		}
		this.shouHuChongController = go.AddComponent<ShouHuChongController>();
		this.shouHuChongController.LoaderURL = loader.LoaderURL;
		if (loader.ReplaceChildLayer)
		{
			U3DUtils.ReplaceLayerInChildren(go, loader.ToLayer, null);
		}
		this.shouHuChongController.InitController(go, loader.parent.transform);
		if (loader.parent.transform.parent != null)
		{
			U3DUtils.AddChild(loader.parent.transform.parent.gameObject, go, true);
		}
		go.transform.localScale = this.shouHuChongController.Target.localScale;
		Vector3 localScale = go.transform.localScale;
		PetFollow component = go.GetComponent<PetFollow>();
		if (component != null)
		{
			Transform transform = go.transform;
			if (loader.Categoriy == ItemCategories.ChongWu)
			{
				component.offsetX = localScale.x * -0.5f;
				component.offsetY = 0f;
				component.offsetZ = localScale.z * 0.5f;
			}
			else if (loader.Categoriy == ItemCategories.ShouHuChong)
			{
				component.offsetX = localScale.x * -0.5f;
				component.offsetY = localScale.x * 1.5f;
				component.offsetZ = localScale.z * 0.5f;
			}
			transform.localPosition = transform.localPosition + transform.forward * component.offsetX + transform.up * component.offsetY + transform.right * component.offsetZ;
		}
		component.ActionRange = 0.2f;
		component.stopRange = 0.1f;
		component.PetItemEvent = delegate(object s, PetEventArgs e)
		{
			if (e.StepType == 1)
			{
				if (this.shouHuChongController != null && this.shouHuChongController.Action != GPetActions.Walk)
				{
					this.shouHuChongController.Action = GPetActions.Walk;
				}
			}
			else if (e.StepType == 2 && this.shouHuChongController != null && this.shouHuChongController.Action == GPetActions.Walk)
			{
				this.shouHuChongController.Action = GPetActions.Stand;
			}
		};
		go.AddComponent<LoadRoleShaderAgain>();
	}

	public void ShouHuChongLoaderSpecialGameObjectsComplete(AssetbundleLoader loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		go.AddComponent<LoadRoleShaderAgain>();
	}

	public void WeaponLoaderComplete(WeaponLoadData loader, List<GameObject> gameObjectList)
	{
		if (gameObjectList == null || gameObjectList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < gameObjectList.Count; i++)
		{
			GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointList[i]);
			if (null == gameObject)
			{
				Object.Destroy(gameObjectList[i]);
			}
			else
			{
				U3DUtils.AddChild(gameObject, gameObjectList[i], true);
				UIHelper.SetReanderQueue(gameObjectList[i]);
			}
		}
	}

	public void ShowRoleListBox(int toSelectIndex = -1)
	{
		this.ItemCollection.Clear();
		if (0 >= this.ListRoleIDs.Count)
		{
			this.ClearLastRole();
			this.RoleNameTextBlock.Text = string.Empty;
		}
		if (this.ListRoleIDs.Count > 0)
		{
			int num = 0;
			while (num < this.ListRoleIDs.Count && num < 4)
			{
				RoleSelectorListItem roleSelectorListItem = U3DUtils.NEW<RoleSelectorListItem>();
				roleSelectorListItem.RoleID = this.ListRoleIDs[num];
				roleSelectorListItem.Sex = this.ListRoleSexes[num];
				roleSelectorListItem.Occupation = this.ListRoleOccups[num];
				roleSelectorListItem.DeleteDeltaTicks = this.ListRoleDeltimeTicks[num];
				roleSelectorListItem.HintToCreate = false;
				roleSelectorListItem.TextBlockName.Text = this.ListRoleNames[num];
				roleSelectorListItem.TextBlockLevel.Text = string.Format(Global.GetLang("{0}转{1}级"), this.ListRoleChangeLifeCount[num], this.ListRoleLevels[num]);
				roleSelectorListItem.ChangeImage(string.Format("NetImages/RS_Face/{0}.png", Global.CalcOriginalOccupationID(this.ListRoleOccups[num])));
				roleSelectorListItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 1)
					{
						this.DeleteRoleBtn_MouseLeftButtonUp(null, null);
					}
					else if (e.ID == 2 && this.BtnShanChu.gameObject.activeSelf != e.AutoUseGold)
					{
						if (!e.AutoUseGold)
						{
							this.m_BtnGaiMing.transform.localPosition = new Vector3((this.VectorGaiMing.x + this.BtnShanChu.transform.localPosition.x) / 2f, this.VectorGaiMing.y, this.VectorGaiMing.z);
						}
						else
						{
							this.m_BtnGaiMing.transform.localPosition = this.VectorGaiMing;
						}
						this.BtnShanChu.gameObject.SetActive(e.AutoUseGold);
					}
				};
				this.ItemCollection.AddNoUpdate(roleSelectorListItem);
				num++;
			}
		}
		for (int i = this.ListRoleIDs.Count; i < 4; i++)
		{
			RoleSelectorListItem roleSelectorListItem2 = U3DUtils.NEW<RoleSelectorListItem>();
			roleSelectorListItem2.RoleID = -1;
			roleSelectorListItem2.Sex = 0;
			roleSelectorListItem2.Occupation = -1;
			roleSelectorListItem2.HintToCreate = true;
			roleSelectorListItem2.TextBlockName.Text = string.Empty;
			roleSelectorListItem2.TextBlockOccupation.Text = string.Empty;
			roleSelectorListItem2.TextBlockLevel.Text = string.Empty;
			roleSelectorListItem2.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 1)
				{
					this.DeleteRoleBtn_MouseLeftButtonUp(null, null);
				}
				if (this.BtnShanChu.gameObject.activeSelf != e.AutoUseGold)
				{
					if (!e.AutoUseGold)
					{
						this.m_BtnGaiMing.transform.localPosition = new Vector3((this.VectorGaiMing.x + this.BtnShanChu.transform.localPosition.x) / 2f, this.VectorGaiMing.y, this.VectorGaiMing.z);
					}
					else
					{
						this.m_BtnGaiMing.transform.localPosition = this.VectorGaiMing;
					}
					this.BtnShanChu.gameObject.SetActive(e.AutoUseGold);
				}
			};
			this.ItemCollection.AddNoUpdate(roleSelectorListItem2);
		}
		this.ItemCollection.DelayUpdate();
		int selectedIndex = 0;
		if (toSelectIndex == -1)
		{
			if (null != this.listBox.SelectedItem)
			{
				selectedIndex = this.ItemCollection.Count - 1;
			}
		}
		else
		{
			selectedIndex = toSelectIndex;
		}
		if (this.ListRoleIDs.Count > 0)
		{
			this.listBox.SelectedIndex = selectedIndex;
		}
	}

	private IEnumerator Init3DMap()
	{
		if (null == Global.RoleSel3DBakMapLoader)
		{
			WWW www = Global.RoleSel3DBakMapWWW;
			if (www == null)
			{
				www = new WWW(PathUtils.WebPath("Map/xuanjue.unity3d"));
			}
			Global.RoleSel3DBakMapWWW = null;
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
			this.CurrentLoader = www.assetBundle;
			www.Dispose();
			www = null;
		}
		else
		{
			Global.RoleSel3DBakMapWWW = null;
			this.CurrentLoader = Global.RoleSel3DBakMapLoader;
		}
		string levelName = "xuanjue";
		AsyncOperation asyncOperation = Application.LoadLevelAsync(levelName);
		Global.IsInGameScene = false;
		yield return asyncOperation;
		CameraShake.Instance.enabled = false;
		CameraShake.Instance.OriginPosition = new Vector3(0f, 1.243f, -10.7f);
		Global.MainCamera.transform.localPosition = new Vector3(0f, 1.243f, -10.7f);
		Global.MainCamera.transform.localRotation = Quaternion.Euler(360f, 0f, 0f);
		Global.MainCamera.farClipPlane = 1000f;
		Global.MainCamera.fieldOfView = 45f;
		LoadingMap.ClearSpeicalMapEffect();
		RenderSettings.ambientLight = new Color(1f, 1f, 1f);
		PerformanceCtrl.ResetNormalSceneSettings();
		GameObject go = GameObject.Find("/CameraParams");
		if (null != go)
		{
			Camera componentCamera = go.GetComponent("Camera") as Camera;
			if (null != componentCamera)
			{
				Global.MainCamera.backgroundColor = componentCamera.backgroundColor;
				Global.MainCamera.farClipPlane = componentCamera.farClipPlane;
				AudioListener aul = componentCamera.GetComponent<AudioListener>();
				if (aul)
				{
					aul.enabled = false;
				}
			}
		}
		Global.MainCamera.backgroundColor = Color.black;
		yield break;
	}

	public void Hide3DObjects()
	{
		if (null != this.oldRole)
		{
			this.oldRole.gameObject.SetActive(false);
		}
		if (this.shouHuChongController != null)
		{
			this.shouHuChongController.gameObject.SetActive(false);
		}
	}

	public void Show3DObjects()
	{
		if (null != this.listBox.SelectedItem)
		{
			Super.ShowNetWaiting(Global.GetLang("正在获取角色数据..."));
			GameInstance.Game.SpriteGetUsingGoodsDataList(U3DUtils.AS<RoleSelectorListItem>(this.listBox.SelectedItem).RoleID, true);
		}
		CameraShake.Instance.enabled = false;
		CameraShake.Instance.OriginPosition = new Vector3(0f, 1.243f, -10.7f);
		Global.MainCamera.transform.localPosition = new Vector3(0f, 1.243f, -10.7f);
		Global.MainCamera.transform.localRotation = Quaternion.Euler(360f, 0f, 0f);
		Global.MainCamera.far = 1000f;
		Global.MainCamera.fieldOfView = 45f;
		RenderSettings.ambientLight = new Color(1f, 1f, 1f);
		GameObject gameObject = GameObject.Find("/CameraParams");
		if (null != gameObject)
		{
			Camera camera = gameObject.GetComponent("Camera") as Camera;
			if (null != camera)
			{
				Global.MainCamera.backgroundColor = camera.backgroundColor;
				Global.MainCamera.farClipPlane = camera.farClipPlane;
			}
		}
		Global.MainCamera.backgroundColor = Color.black;
	}

	private string FormatSecs(int secs)
	{
		return StringUtil.substitute(Global.GetLang("{0}小时{1}分钟{2}秒"), new object[]
		{
			Global.FormatStr("00", secs / 3600),
			Global.FormatStr("00", secs % 3600 / 60),
			Global.FormatStr("00", secs % 3600 % 60)
		});
	}

	private void Timer_Tick(object sender, object e)
	{
		this._UpdateShanChuJueSeCD();
	}

	private void StartTimer()
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = null;
			this.Timer.Stop();
			this.Timer = null;
		}
		this.Timer = new DispatcherTimer("RoleSelector_Timer");
		this.Timer.Interval = TimeSpan.FromSeconds(0.5);
		this.Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.Timer.Start();
	}

	private void StopTimer()
	{
		if (this.Timer != null)
		{
			this.Timer.Tick = null;
			this.Timer.Stop();
			this.Timer = null;
		}
		this.Visibility = false;
	}

	private void _UpdateShanChuJueSeCD()
	{
		if (null == this.listBox || null == this.listBox.SelectedItem)
		{
			this.ShanChuJueSeCD.SetActive(false);
			this.m_BtnGaiMing.gameObject.SetActive(true);
			this.EnterGameBtn.gameObject.SetActive(true);
			if (Global.GetLang("改  名") != this.m_GaiMing.text)
			{
				this.m_GaiMing.text = Global.GetLang("改  名");
			}
			return;
		}
		int num = (this.index != 1) ? 0 : 1;
		this.index = ((num != 0) ? 0 : 1);
		for (int i = 0; i < this.listBox.Items.Count; i++)
		{
			RoleSelectorListItem roleSelectorListItem = U3DUtils.AS<RoleSelectorListItem>(this.listBox.getChildAt(i));
			if (roleSelectorListItem.DeleteDeltaTicks > 0L)
			{
				roleSelectorListItem.DeleteDeltaTicks -= (long)num;
			}
			long deleteDeltaTicks = roleSelectorListItem.DeleteDeltaTicks;
			UITexture component = roleSelectorListItem.FaceImage.transform.GetComponent<UITexture>();
			string name = component.shader.name;
			if (deleteDeltaTicks < 0L)
			{
				if (this.listBox.SelectedItem.Equals(roleSelectorListItem.gameObject))
				{
					this.ShanChuJueSeCD.SetActive(false);
					this.m_BtnGaiMing.gameObject.SetActive(true);
					this.EnterGameBtn.gameObject.SetActive(true);
					if (Global.GetLang("改  名") != this.m_GaiMing.text)
					{
						this.m_GaiMing.text = Global.GetLang("改  名");
					}
				}
				if ("Unlit/Transparent Colored" != name)
				{
					component.shader = Shader.Find("Unlit/Transparent Colored");
				}
			}
			else if (deleteDeltaTicks > 0L)
			{
				if ("Unlit/Gray" != name)
				{
					component.shader = Shader.Find("Unlit/Gray");
				}
				if (this.listBox.SelectedItem.Equals(roleSelectorListItem.gameObject))
				{
					this.ShanChuJueSeCD.SetActive(true);
					this.m_BtnGaiMing.gameObject.SetActive(true);
					this.EnterGameBtn.gameObject.SetActive(false);
					long num2 = deleteDeltaTicks;
					int num3 = 86400;
					int num4 = 3600;
					int num5 = 60;
					int num6 = (int)(num2 % (long)num3 / (long)num4);
					int num7 = (int)(num2 % (long)num3 % (long)num4 / (long)num5);
					int num8 = (int)(num2 % (long)num3 % (long)num4 % (long)num5);
					this.ShanChuJueSeCDLabel.text = StringUtil.substitute(Global.GetLang("{0}小时{1}分{2}秒后删除"), new object[]
					{
						num6,
						num7,
						num8
					});
					if (Global.GetLang("恢复角色") != this.m_GaiMing.text)
					{
						this.m_GaiMing.text = Global.GetLang("恢复角色");
					}
				}
			}
			else
			{
				if ("Unlit/Gray" != name)
				{
					component.shader = Shader.Find("Unlit/Gray");
				}
				if (this.listBox.SelectedItem.Equals(roleSelectorListItem.gameObject))
				{
					this.ShanChuJueSeCDLabel.text = StringUtil.substitute(Global.GetLang("{0}小时{1}分{2}秒后删除"), new object[]
					{
						0,
						0,
						0
					});
					if (Global.GetLang("恢复角色") != this.m_GaiMing.text)
					{
						this.m_GaiMing.text = Global.GetLang("恢复角色");
					}
					this.ShanChuJueSeCD.SetActive(true);
					this.m_BtnGaiMing.gameObject.SetActive(true);
					this.EnterGameBtn.gameObject.SetActive(false);
				}
			}
		}
	}

	public GButton DeleteRoleBtn;

	public GButton CreateRoleBtn;

	public GButton EnterGameBtn;

	public ShowNetImage ShowText;

	public ListBox listBox;

	private ObservableCollection ItemCollection;

	public new ShowNetImage Background;

	public TextBlock RoleNameTextBlock;

	public RoleInfo m_DeleteRoleInfo;

	public TextBlock ServerName;

	public UIButton GoBackBtn;

	public UIButton m_BtnGaiMing;

	public GameObject ShanChuJueSeCD;

	public UILabel ShanChuJueSeCDLabel;

	public GButton ShanChuJueSeUndo;

	public UILabel m_GaiMing;

	public EventHandler GoBackEvent;

	public ZhuanZhiBtnItem[] ZhuanZhiBtns = new ZhuanZhiBtnItem[4];

	public ShowNetImage TextImg;

	public UISprite TextZhiYe;

	public TextBlock TextContent;

	public TextBlock TextTitle;

	public GButton BtnShanChu;

	private DispatcherTimer Timer;

	private Vector3 VectorGaiMing = default(Vector3);

	private bool Is3DBackground = true;

	private ShouHuChongController shouHuChongController;

	private RoleResLoader roleResLoader;

	private WingsResLoader wingsResLoader;

	private ShouHuChongResLoader shouHuChongResLoader;

	private WeaponResLoader weaponResLoader;

	public EventHandler DeleteRoleBtnUp;

	private bool prepareToCreatePanel;

	private RoleSelectorListItem itemTemp;

	public EventHandler RolePanelChanged;

	public EventHandler StartGameByRole;

	private List<int> ListRoleIDs = new List<int>();

	private List<int> ListRoleSexes = new List<int>();

	private List<int> ListRoleOccups = new List<int>();

	private List<string> ListRoleNames = new List<string>();

	private List<int> ListRoleLevels = new List<int>();

	private List<int> ListRoleChangeLifeCount = new List<int>();

	private List<long> ListRoleDeltimeTicks = new List<long>();

	private GameObject oldRole;

	private AssetBundle CurrentLoader;

	private Rect TerrainRect = new Rect(0f, 0f, 0f, 0f);

	private int index;
}
