using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class RoleCreator : UserControl
{
	public bool ZHSCanCreate
	{
		get
		{
			return this.m_ZHSCanCreate;
		}
		set
		{
			this.m_ZHSCanCreate = value;
			this.ShowCreateRoleBtn(this.ConditionOfRoleCreate(this.listBox.SelectedIndex + 1));
		}
	}

	private void InitTextInPrefabs()
	{
		if (this.ConstTexts != null && this.ConstTexts.Length == 1)
		{
			this.ConstTexts[0].Text = string.Empty;
		}
		this.CreateRoleBtn.Text = Global.GetLang("创建角色");
		this.dicContent.Add(0, Global.GetLang("剑士拥有过人的体力及华丽的剑术，在冒险时，因优异的机动力通常担任先锋。 "));
		this.dicContent.Add(1, Global.GetLang("魔法师掌控着寒冰与烈焰的魔力，可以利用大范围的魔法迅速消灭成片的魔物。"));
		this.dicContent.Add(2, Global.GetLang("弓箭手拥有美丽的外貌，擅长在远处狙击敌人，拥有一击必杀的爆发型技能。"));
		this.dicContent.Add(3, Global.GetLang("魔剑士是掌控黑暗力量的勇士，可选择修炼力量或魔法，更可佩戴剑士和魔法师的武器。"));
		this.dicContent.Add(5, Global.GetLang("召唤师可使用独特的诅咒系魔法，更可从魔界召唤出强大的幻兽为自己所用。"));
		this.textBox1.Text = Global.GetLang("你的角色名");
		this.textBox1.defaultText = Global.GetLang("你的角色名");
		this._RoleCreatCondition.Text = string.Format(Global.GetLang("拥有{0}转以上角色可直接创建"), 3);
		this._RoleCreatCondition1.Text = string.Format(Global.GetLang("游戏中使用【{0}】可创建"), Global.GetLang("招魂凭证"));
		this.staticText[0].text = Global.GetLang("魔法型");
		this.staticText[1].text = Global.GetLang("力量型");
		this.staticText[2].text = Global.GetLang("游戏内可更改主修方向");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.UserControl_Loaded(this);
		this.time = 0f;
		this.GBackBtnCanCheck = false;
		base.StartCoroutine<bool>(this.WaiteTwoS());
		Super.BackUpMainScene();
		if (this.Is3DBackground)
		{
			base.StartCoroutine<bool>(this.Init3DMap());
		}
		if (this.listBox.SelectedIndex != 3)
		{
			this.LiOrZhiChooseTransform.gameObject.SetActive(false);
		}
		this.RadioCheckZhi.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (!this.CanClick)
			{
				return;
			}
			this.LoadRoleRes2(3);
			this.ControlClick();
			if (this.RadioCheckZhi.Check)
			{
				this.m_MoJianCheckOnTexiao.transform.localPosition = new Vector3(this.RadioCheckZhi.transform.localPosition.x, 8f, -1f);
			}
		};
		this.RadioCheckLi.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (!this.CanClick)
			{
				return;
			}
			this.LoadRoleRes2(3);
			this.ControlClick();
			if (this.RadioCheckLi.Check)
			{
				this.m_MoJianCheckOnTexiao.transform.localPosition = new Vector3(this.RadioCheckLi.transform.localPosition.x, 8f, -1f);
			}
		};
	}

	public void SetWindowType(int nType)
	{
		if (nType == 1)
		{
			this.HasRoleInfo = false;
			string loginMode = Global.GetLoginMode();
			if ("0" == loginMode && null != this.GoBackBtn)
			{
				this.GoBackBtn.gameObject.SetActive(false);
			}
			this.RandomSelected(this.roleNum);
		}
		else if (nType == 2)
		{
			this.IsUseZHPZ = true;
			this.RandomSelected(3);
		}
		else
		{
			this.HasRoleInfo = true;
			this.RandomSelected(this.roleNum);
		}
	}

	public override void Destroy()
	{
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
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (this.monsertNPCLoader != null)
		{
			this.monsertNPCLoader.Stop();
			this.monsertNPCLoader = null;
		}
		if (null != this.CurrentLoader)
		{
			this.CurrentLoader.Unload(true);
			this.CurrentLoader = null;
		}
	}

	public RoleCreatorListItem CreatRole(int index)
	{
		RoleCreatorListItem roleCreatorListItem = null;
		switch (index)
		{
		case 0:
			roleCreatorListItem = U3DUtils.NEW<RoleCreatorListItem>();
			roleCreatorListItem.ChangeOccupationText(Global.GetLang("剑  士"));
			roleCreatorListItem.Occupation = 0;
			roleCreatorListItem.Sex = 0;
			roleCreatorListItem.ItemSelected = false;
			this.Items.Add(roleCreatorListItem.gameObject);
			break;
		case 1:
			roleCreatorListItem = U3DUtils.NEW<RoleCreatorListItem>();
			roleCreatorListItem.ChangeOccupationText(Global.GetLang("魔法师"));
			roleCreatorListItem.Occupation = 1;
			roleCreatorListItem.Sex = 0;
			roleCreatorListItem.ItemSelected = false;
			this.Items.Add(roleCreatorListItem.gameObject);
			break;
		case 2:
			roleCreatorListItem = U3DUtils.NEW<RoleCreatorListItem>();
			roleCreatorListItem.ChangeOccupationText(Global.GetLang("弓箭手"));
			roleCreatorListItem.Occupation = 2;
			roleCreatorListItem.Sex = 1;
			roleCreatorListItem.ItemSelected = false;
			this.Items.Add(roleCreatorListItem.gameObject);
			break;
		case 3:
			roleCreatorListItem = U3DUtils.NEW<RoleCreatorListItem>();
			roleCreatorListItem.ChangeOccupationText(Global.GetLang("魔剑士"));
			roleCreatorListItem.Occupation = 3;
			roleCreatorListItem.Sex = 0;
			roleCreatorListItem.ItemSelected = false;
			this.Items.Add(roleCreatorListItem.gameObject);
			break;
		case 5:
			roleCreatorListItem = U3DUtils.NEW<RoleCreatorListItem>();
			roleCreatorListItem.ChangeOccupationText(Global.GetLang("召唤师"));
			roleCreatorListItem.Occupation = 5;
			roleCreatorListItem.Sex = 1;
			roleCreatorListItem.ItemSelected = false;
			this.Items.Add(roleCreatorListItem.gameObject);
			break;
		}
		return roleCreatorListItem;
	}

	private void UserControl_Loaded(UserControl sender)
	{
		this.Items = this.listBox.Items;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.resetRoleCreatorList();
		Vector3 localPosition = this.listBox.transform.localPosition;
		localPosition.y += this.listBox.cellHeight;
		this.listBox.transform.localPosition = localPosition;
		UIEventListener.Get(this.GoBackBtn.gameObject).onClick = delegate(GameObject s)
		{
			if (this.GBackBtnCanCheck)
			{
				this.GoBack_MouseLeftButtonUp(null, null);
			}
		};
		this.CreateRoleBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CreateRoleNow_MouseLeftButtonUp);
		this.RandomNameBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.RandomName_MouseLeftButtonUp);
	}

	public void resetRoleCreatorList()
	{
		this.Items.Clear();
		this.roleNum = 0;
		this.lastTicks = 0L;
		if (this.isCreateSecondRole && Global.Data.roleData.OccupationList != null)
		{
			for (int i = 0; i < this.roleOccuption.Length; i++)
			{
				if (!Global.Data.roleData.OccupationList.Contains(this.roleOccuption[i]))
				{
					RoleCreatorListItem roleCreatorListItem = this.CreatRole(this.roleOccuption[i]);
					this.roleNum++;
				}
			}
			this.SetRandomPreName(Global.Data.roleData.RoleName);
			BoxCollider component = this.textBox1.GetComponent<BoxCollider>();
			component.enabled = false;
			this.RandomNameBtn.gameObject.SetActive(false);
			this.noChangeName = true;
			this.ZhuanZhiObj.SetActive(true);
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("PurchaseOccupation", ',');
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("PurchaseOccupationNum", true);
			if (int.Parse(systemParamByName) == systemParamIntArrayByName.Length && Global.Data.roleData.OccupationList.Count - 1 <= int.Parse(systemParamByName))
			{
				this.zuanshiNum = systemParamIntArrayByName[Global.Data.roleData.OccupationList.Count - 1];
			}
			this.XiaoHaoLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("消耗")
			});
			this.ZuanShiNumLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.zuanshiNum
			});
		}
		else
		{
			this.ZhuanZhiObj.SetActive(false);
			for (int j = 0; j < this.roleOccuption.Length; j++)
			{
				RoleCreatorListItem roleCreatorListItem = this.CreatRole(this.roleOccuption[j]);
				this.roleNum++;
			}
		}
	}

	public void SetRandomPreName(string name)
	{
		this.textBox1.activeColor = new Color(0.94f, 0.95f, 0.98f);
		this.textBox1.Text = name;
	}

	private bool ConditionOfRoleCreate(int Occupation)
	{
		switch (Occupation)
		{
		case 5:
			return this.isCreateSecondRole || this.IsUseZHPZ || this.m_ZHSCanCreate;
		}
		return true;
	}

	private void ShowCreateRoleBtn(bool BShow = true)
	{
		NGUITools.SetActive(this.CreateRoleBtn, BShow);
		NGUITools.SetActive(this._ConditionObj, !BShow);
	}

	private void CreateRoleNow_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.lastTicks < this.AnimationTime)
		{
			return;
		}
		if (this.noChangeName)
		{
			MouseLeftButtonUpEventHandler regOkCallBack = delegate(object s, MouseEvent args)
			{
				if (Global.Data.roleData.UserMoney >= this.zuanshiNum)
				{
					if (Global.IsOperateUnPermitInKuaFuMapCheck(false, true))
					{
						string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("跨服中不可转职")
						});
						Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), colorStringForNGUIText2, -1, -1, -1, -1, false);
						return;
					}
					this.CreatRoleNowQ();
				}
				else
				{
					string colorStringForNGUIText3 = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("钻石不足")
					});
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), colorStringForNGUIText3, -1, -1, -1, -1, false);
				}
			};
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("创建副职业需要花费"),
				"17e43e",
				string.Format(Global.GetLang("{0}钻石"), this.zuanshiNum),
				"dac7ae",
				Global.GetLang("\r\n副职业创建后不可"),
				"FF0000",
				Global.GetLang("删除"),
				"dac7ae",
				Global.GetLang("或"),
				"FF0000",
				Global.GetLang("修改")
			});
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 1, Global.GetLang("提示"), colorStringForNGUIText, regOkCallBack, null, -1, -1, -1, -1, false);
		}
		else
		{
			this.CreatRoleNowQ();
		}
		if (this.isCreateSecondRole)
		{
			GameObject gameObject = GameObject.Find("Leader");
			if (null != gameObject)
			{
				AudioListener component = gameObject.GetComponent<AudioListener>();
				if (null != component)
				{
					component.enabled = true;
				}
			}
			Global.AudioListener4UI.enabled = this.AudilListener4UIEnable;
		}
	}

	private void CreatRoleNowQ()
	{
		this.textBox1.Text = Global.StringReplaceAll(this.textBox1.Text, "'", string.Empty);
		this.textBox1.Text = Global.StringReplaceAll(this.textBox1.Text, "|", string.Empty);
		this.textBox1.Text = Global.StringReplaceAll(this.textBox1.Text, "$", string.Empty);
		this.textBox1.Text = Global.StringReplaceAll(this.textBox1.Text, ":", string.Empty);
		if (this.textBox1.Text.Contains("{") || this.textBox1.Text.Contains("}"))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		this.textBox1.Text = Global.ReplaceFilterFileds(this.textBox1.Text);
		if (string.IsNullOrEmpty(this.textBox1.Text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,请输入的您要创建的角色名称!"), -1, -1, -1, -1, false);
			return;
		}
		if (Global.GetLang("名字乘以五") == this.textBox1.Text)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,请输入的您要创建的角色名称!"), -1, -1, -1, -1, false);
			return;
		}
		byte b = Global.CheckRoleNameLenght(this.textBox1.Text);
		if (Global.NameLengthRange[0] == b)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,您输入的角色昵称不能少于") + b + Global.GetLang("个字，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (Global.NameLengthRange[1] == b)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("错误"), Global.GetLang("抱歉,您输入的角色昵称已超过") + b + Global.GetLang("个字，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (this.textBox1.Text.IndexOf(" ") != -1)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,昵称当中不允许包含空格，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		if (!Global.IsKoreanOrEn(this.textBox1.Text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		RoleCreatorListItem roleCreatorListItem = U3DUtils.AS<RoleCreatorListItem>(this.listBox.SelectedItem);
		if (null == roleCreatorListItem)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("请选择要创建的角色头像!"), -1, -1, -1, -1, false);
			return;
		}
		if (Global.IncludeReplaceFilterFileds(this.textBox1.Text))
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的昵称当中含有敏感词汇，请重新输入!"), -1, -1, -1, -1, false);
			return;
		}
		Global.PlaySoundAudio("Audio/UI/dropbaoshi", false);
		int num = 1;
		if (Global.Data != null)
		{
			num = Global.Data.GameServerID;
		}
		if (Global.GetLoginMode() != "0" && num != PlatformUserLogin.RecordSelectServerID)
		{
			Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,所选区不一致，请勿双开游戏! 可返回选区界面"), -1, -1, -1, -1, false);
			return;
		}
		Super.ShowNetWaiting(Global.GetLang("正在创建角色..."));
		int liOrZhi = 0;
		if (this.RadioCheckZhi.Check)
		{
			liOrZhi = 1;
		}
		int sex = roleCreatorListItem.Sex;
		if (roleCreatorListItem.Occupation == 3)
		{
			sex = 0;
		}
		if (this.IsUseZHPZ)
		{
			GameInstance.Game.SendCreateRoleInGame(sex, roleCreatorListItem.Occupation, string.Format("{0}${1}", Global.StringTrim(this.textBox1.Text), PlatSDKMgr.PlatName), num, liOrZhi);
		}
		else if (this.isCreateSecondRole)
		{
			GameInstance.Game.SendPurchaseData(roleCreatorListItem.Occupation);
			Global.secondRoleId = roleCreatorListItem.Occupation;
		}
		else
		{
			GameInstance.Game.CreateRole(sex, roleCreatorListItem.Occupation, string.Format("{0}${1}", Global.StringTrim(this.textBox1.Text), PlatSDKMgr.PlatName), num, liOrZhi);
		}
		PlatSDKMgr._lastCreateRole = Global.StringTrim(this.textBox1.Text);
		if (Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null)
		{
			PlatformUserLogin.RecordLoginServerIDs(Global.Data.ServerData.LastServer);
		}
		Global.MainCamera.cullingMask = this.cameraCullingMask;
	}

	private void GoBack_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.lastTicks < this.AnimationTime)
		{
			return;
		}
		if (this.RolePanelChanged != null)
		{
			if (this.isCreateSecondRole)
			{
				GameObject gameObject = GameObject.Find("Leader");
				if (null != gameObject)
				{
					AudioListener component = gameObject.GetComponent<AudioListener>();
					if (null != component)
					{
						component.enabled = true;
					}
				}
				Global.AudioListener4UI.enabled = this.AudilListener4UIEnable;
				GameObject gameObject2 = U3DUtils.FindGameObjectByName(null, "/Directional light");
				if (null != gameObject2)
				{
					U3DUtils.ModifyDirectLight(gameObject2, 1 << LayerMask.NameToLayer("Sprites") | 1 << LayerMask.NameToLayer("TargetCamera") | 1 << LayerMask.NameToLayer("SelfCamera"));
				}
			}
			this.Destroy3DObjects();
			this.RolePanelChanged.Invoke(this, EventArgs.Empty);
			Global.MainCamera.cullingMask = this.cameraCullingMask;
		}
	}

	private void RandomName_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (null != this.m_SpriteAnimation)
		{
			this.m_SpriteAnimation.gameObject.SetActive(true);
			this.m_SpriteAnimation.Reset();
			this.m_SpriteAnimation.isFinishedHiddle = true;
		}
		RoleCreatorListItem roleCreatorListItem = U3DUtils.AS<RoleCreatorListItem>(this.listBox.SelectedItem);
		if (null != roleCreatorListItem)
		{
			this.SetRandomPreName(Global.GetRandomCreatorRoleName(roleCreatorListItem.Sex));
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (correctLocalTime - this.lastTicks < this.AnimationTime)
		{
			return;
		}
		this.lastTicks = correctLocalTime;
		RoleCreatorListItem roleCreatorListItem = U3DUtils.AS<RoleCreatorListItem>(this.listBox.SelectedItem);
		if (null == roleCreatorListItem)
		{
			return;
		}
		if (this.IsUseZHPZ && roleCreatorListItem.Occupation != 5)
		{
			Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("提示"), Global.GetLang("只能创建召唤师职业！"), -1, -1, -1, -1, false);
			this.listBox.SelectedIndex = this.listBox.Items.Count - 1;
			return;
		}
		if (this.lastItem != null && this.lastItem != roleCreatorListItem)
		{
			this.lastItem.ItemSelected = false;
		}
		if (this.lastItem == roleCreatorListItem)
		{
			return;
		}
		if (roleCreatorListItem.Occupation == 3)
		{
			this.LiOrZhiChooseTransform.gameObject.SetActive(true);
		}
		else
		{
			this.LiOrZhiChooseTransform.gameObject.SetActive(false);
		}
		this.lastItem = roleCreatorListItem;
		this.lastItem.ItemSelected = true;
		this.TextImg.ShowImage(StringUtil.substitute("NetImages/Roles/info_bg_{0}.png", new object[]
		{
			this.lastItem.Occupation
		}));
		this.TextZhiYe.spriteName = StringUtil.substitute("info_{0}", new object[]
		{
			this.lastItem.Occupation
		});
		if (this.lastItem.Occupation == 0)
		{
			this.TextZhiYe.transform.localScale = new Vector3(62f, 33f, 1f);
		}
		else
		{
			this.TextZhiYe.transform.localScale = new Vector3(93f, 33f, 1f);
		}
		this.TextContent.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"bfb592",
			this.dicContent[this.lastItem.Occupation]
		});
		this.LoadRoleRes2(this.lastItem.Occupation);
		if (!this.noChangeName)
		{
			this.SetRandomPreName(Global.GetRandomCreatorRoleName(this.lastItem.Sex));
		}
		else
		{
			BoxCollider component = this.textBox1.GetComponent<BoxCollider>();
			component.enabled = false;
		}
		this.ShowCreateRoleBtn(this.ConditionOfRoleCreate(this.lastItem.Occupation));
	}

	private IEnumerator WaiteTwoS()
	{
		yield return new WaitForSeconds(1.49f);
		this.GBackBtnCanCheck = true;
		yield break;
	}

	private void LoadRoleRes2(int occupation)
	{
		this.time = 0f;
		this.GBackBtnCanCheck = false;
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		GameObject gameObject = new GameObject();
		if (null != gameObject)
		{
			this.oldRole = gameObject;
			GameObject gameObject2 = gameObject;
			string path = string.Empty;
			string resName = string.Empty;
			if (Global.CalcOriginalOccupationID(occupation) == 0)
			{
				resName = "ZS.unity3d";
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 1)
			{
				resName = "FS.unity3d";
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 2)
			{
				resName = "GS.unity3d";
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 3)
			{
				if (this.RadioCheckZhi.Check)
				{
					resName = "MJS_F.unity3d";
				}
				else
				{
					resName = "MJS_Z.unity3d";
				}
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 5)
			{
				resName = "ZHS.unity3d";
			}
			Transform transform = gameObject2.transform;
			path = string.Empty;
			int layer = LayerMask.NameToLayer("Water");
			GameObject gameObject3 = null;
			if (Global.CalcOriginalOccupationID(occupation) == 0)
			{
				path = MuAssetManager.GetBundleID("Decoration", "CJJS_ZS");
				gameObject3 = U3DUtils.GetEmptyLoader("Deco", path, false, null, null, -1, null, layer, 1f, true, false, null);
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 1)
			{
				path = MuAssetManager.GetBundleID("Decoration", "CJJS_FS");
				gameObject3 = U3DUtils.GetEmptyLoader("Deco", path, false, null, null, -1, null, layer, 1f, true, false, null);
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 2)
			{
				path = MuAssetManager.GetBundleID("Decoration", "CJJS_GS");
				gameObject3 = U3DUtils.GetEmptyLoader("Deco", path, false, null, null, -1, null, layer, 1f, true, false, null);
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 3)
			{
				path = MuAssetManager.GetBundleID("Decoration", "CJJS_MJS_Z");
				if (this.RadioCheckZhi.Check)
				{
					path = MuAssetManager.GetBundleID("Decoration", "CJJS_MJS_F");
				}
				gameObject3 = U3DUtils.GetEmptyLoader("Deco", path, false, null, null, -1, null, layer, 1f, true, false, null);
			}
			else if (Global.CalcOriginalOccupationID(occupation) == 5)
			{
				path = MuAssetManager.GetBundleID("Decoration", "CJJS_ZHS");
				gameObject3 = U3DUtils.GetEmptyLoader("Deco", path, false, null, null, -1, new AssetbundleLoaderComplete(this.RoleDecLoaderCompleteOK), layer, 1f, true, false, null);
			}
			EffectRoot effectRoot = gameObject3.AddComponent<EffectRoot>();
			gameObject3.layer = layer;
			this.monsertNPCLoader = new MonsterNPCResLoader(new MonsterNPCLoaderData
			{
				parent = gameObject,
				resName = resName,
				spriteType = GSpriteTypes.NPC,
				TagEx = occupation
			}, new OnLoadMonsterNPCComplete(this.RoleLoaderComplete2));
		}
	}

	private void RoleDecLoaderCompleteOK(AssetbundleLoader loader, GameObject go)
	{
		if (go != null && go.name.Contains("CJJS_ZHS"))
		{
			GameObject gameObject = U3DUtils.FindGameObjectByName(go, "Distort");
			if (gameObject)
			{
				gameObject.gameObject.SetActive(false);
			}
		}
	}

	public void RoleLoaderComplete2(MonsterNPCLoaderData loader, GameObject go)
	{
		if (base.IsActive)
		{
			base.StartCoroutine<bool>(this.WaiteTwoS());
		}
		if (null == go)
		{
			return;
		}
		Vector3 localPosition = this.oldRole.transform.localPosition;
		Object.Destroy(this.oldRole);
		this.oldRole = go;
		Transform transform = go.transform;
		int tagEx = loader.TagEx;
		Animation component = go.GetComponent<Animation>();
		if (component != null)
		{
			component.cullingType = 0;
		}
		PlayRoleActions playRoleActions = go.AddComponent<PlayRoleActions>();
		playRoleActions.AttackName = "Fly";
		playRoleActions.StandName = "Stand";
		string text = string.Empty;
		if (Global.CalcOriginalOccupationID(tagEx) == 0)
		{
			text = "Audio/RoleCreate/zs.mp3";
		}
		else if (Global.CalcOriginalOccupationID(tagEx) == 1)
		{
			text = "Audio/RoleCreate/fs.mp3";
		}
		else if (Global.CalcOriginalOccupationID(tagEx) == 2)
		{
			text = "Audio/RoleCreate/gs.mp3";
		}
		else if (Global.CalcOriginalOccupationID(tagEx) == 3)
		{
			if (this.RadioCheckZhi.Check)
			{
				text = "Audio/RoleCreate/mjs_F.mp3";
			}
			else
			{
				text = "Audio/RoleCreate/mjs_Z.mp3";
			}
		}
		else if (Global.CalcOriginalOccupationID(tagEx) == 5)
		{
			text = "Audio/RoleCreate/zhs.mp3";
		}
		if (this.isCreateSecondRole)
		{
			GameObject gameObject = GameObject.Find("Leader");
			if (null != gameObject)
			{
				AudioListener component2 = gameObject.GetComponent<AudioListener>();
				if (null != component2)
				{
					component2.enabled = false;
				}
			}
			this.AudilListener4UIEnable = Global.AudioListener4UI.enabled;
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				return;
			}
			if (null != MainGame._current.GlobalUIAudioSource)
			{
				Global.AudioListener4UI.enabled = true;
				MainGame._current.GlobalUIAudioSource.PlayAudio(text, false, false);
			}
		}
		else
		{
			Global.PlaySoundAudio(text, false);
		}
	}

	private void RandomSelected(int totalNum = 3)
	{
		totalNum = ((3 > totalNum) ? totalNum : 3);
		RandomAS randomAS = new RandomAS(0);
		int num = randomAS.Next(0, totalNum);
		if (this.IsUseZHPZ)
		{
			num = 4;
		}
		if (num != this.listBox.SelectedIndex)
		{
			this.listBox.SelectedIndex = num;
		}
		else
		{
			this.listBox.SelectedIndex = (num + 1) % totalNum;
		}
		this.ShowCreateRoleBtn(this.ConditionOfRoleCreate(this.listBox.SelectedIndex + 1));
	}

	private IEnumerator Init3DMap()
	{
		if (null == Global.RoleCreate3DBakMapLoader)
		{
			WWW www = Global.RoleCreate3DBakMapWWW;
			if (www == null)
			{
				www = new WWW(PathUtils.WebPath("Map/chuangjue.unity3d"));
			}
			Global.RoleCreate3DBakMapWWW = null;
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
			Global.RoleCreate3DBakMapWWW = null;
			this.CurrentLoader = Global.RoleCreate3DBakMapLoader;
		}
		string levelName = "chuangjue";
		AsyncOperation asyncOperation = Application.LoadLevelAdditiveAsync(levelName);
		Global.IsInGameScene = false;
		yield return asyncOperation;
		CameraShake.Instance.enabled = false;
		CameraShake.Instance.OriginPosition = new Vector3(-0.2159f, 99f, -62.724f);
		Global.MainCamera.transform.localPosition = new Vector3(-0.2159f, 99f, -62.724f);
		Global.MainCamera.transform.localRotation = Quaternion.Euler(351f, 0f, 0f);
		Global.MainCamera.far = 2000f;
		Global.MainCamera.fieldOfView = 36f;
		if (Global.MainCamera.cullingMask != 1 << LayerMask.NameToLayer("Water"))
		{
			this.cameraCullingMask = Global.MainCamera.cullingMask;
		}
		Global.MainCamera.cullingMask = 1 << LayerMask.NameToLayer("Water");
		RenderSettings.ambientLight = new Color(1f, 1f, 1f);
		GameObject directLight = U3DUtils.FindGameObjectByName(null, "/Directional light");
		if (null != directLight)
		{
			U3DUtils.ModifyDirectLight(directLight, -1);
		}
		GameObject go = GameObject.Find("/CameraParams");
		if (null != go)
		{
			Camera componentCamera = go.GetComponent("Camera") as Camera;
			if (null != componentCamera)
			{
				Global.MainCamera.backgroundColor = componentCamera.backgroundColor;
				Global.MainCamera.farClipPlane = componentCamera.farClipPlane;
			}
		}
		Global.MainCamera.backgroundColor = Color.black;
		yield break;
	}

	public void Destroy3DObjects()
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
	}

	private void ControlClick()
	{
		this.CanClick = false;
		this.RadioCheckLi.GetComponent<BoxCollider>().enabled = false;
		this.RadioCheckZhi.GetComponent<BoxCollider>().enabled = false;
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	protected void TickProc()
	{
		if (this.m_clickTime < 0f)
		{
			this.CanClick = true;
			this.RadioCheckZhi.GetComponent<BoxCollider>().enabled = true;
			this.RadioCheckLi.GetComponent<BoxCollider>().enabled = true;
			base.CancelInvoke("TickProc");
			this.m_clickTime = 1f;
		}
		this.m_clickTime -= 1f;
	}

	public override void Update()
	{
		base.Update();
		if (this.time < 5f)
		{
			this.time += Time.deltaTime;
		}
		else if (!this.GBackBtnCanCheck)
		{
			this.GBackBtnCanCheck = true;
		}
	}

	public void Show3DObjects()
	{
		if (null != this.oldRole)
		{
			this.oldRole.gameObject.SetActive(true);
			PlayRoleActions component = this.oldRole.GetComponent<PlayRoleActions>();
			if (component != null)
			{
				component.PlayAgain();
				component.PlayAnimationAgain();
			}
		}
		else
		{
			this.RandomSelected(3);
			if (this.listBox.SelectionChanged != null)
			{
				this.listBox.SelectionChanged(this.listBox, new MouseEvent("mouseUp", null));
			}
		}
		CameraShake.Instance.enabled = false;
		CameraShake.Instance.OriginPosition = new Vector3(-0.2159f, 99f, -62.724f);
		Global.MainCamera.transform.localPosition = new Vector3(-0.2159f, 99f, -62.724f);
		Global.MainCamera.transform.localRotation = Quaternion.Euler(351f, 0f, 0f);
		Global.MainCamera.far = 2000f;
		Global.MainCamera.fieldOfView = 36f;
		if (Global.MainCamera.cullingMask != 1 << LayerMask.NameToLayer("Water"))
		{
			this.cameraCullingMask = Global.MainCamera.cullingMask;
		}
		Global.MainCamera.cullingMask = 1 << LayerMask.NameToLayer("Water");
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

	public TextBox textBox1;

	public TextBlock HintError;

	public ShowNetImage ShowText;

	public ShowNetImage TextImg;

	public UISprite TextZhiYe;

	public TextBlock TextContent;

	public UIButton GoBackBtn;

	public GButton CreateRoleBtn;

	public GButton RandomNameBtn;

	public ListBox listBox;

	private ObservableCollection Items;

	public new ShowNetImage Background;

	public TextBlock[] ConstTexts;

	public Transform LiOrZhiChooseTransform;

	public GCheckBox RadioCheckZhi;

	public GCheckBox RadioCheckLi;

	public TextBlock[] staticText;

	public bool isCreateSecondRole;

	public GameObject m_GameObjTeXiao;

	public UISpriteAnimation m_SpriteAnimation;

	public GameObject m_MoJianCheckOnTexiao;

	public bool HasRoleInfo;

	private bool CanClick = true;

	private float m_clickTime = 1f;

	private bool Is3DBackground = true;

	private bool m_ZHSCanCreate;

	private int roleNum;

	private bool noChangeName;

	private int zuanshiNum;

	public TextBlock _RoleCreatCondition;

	public TextBlock _RoleCreatCondition1;

	public GameObject _ConditionObj;

	private bool IsUseZHPZ;

	public UILabel XiaoHaoLab;

	public UILabel ZuanShiNumLab;

	public GameObject ZhuanZhiObj;

	private Dictionary<int, string> dicContent = new Dictionary<int, string>();

	private MonsterNPCResLoader monsertNPCLoader;

	public EventHandler RolePanelChanged;

	private int[] roleOccuption = new int[]
	{
		0,
		1,
		2,
		3,
		5
	};

	private RoleCreatorListItem lastItem;

	private long lastTicks;

	private long AnimationTime = 2000L;

	private GameObject oldRole;

	private bool GBackBtnCanCheck;

	private bool AudilListener4UIEnable;

	private AssetBundle CurrentLoader;

	private int cameraCullingMask;

	private float time;
}
