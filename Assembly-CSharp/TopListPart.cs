using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TopListPart : UserControl
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

	public bool m_bIsModalSate
	{
		get
		{
			return this._bIsModalState;
		}
		set
		{
			if (null != this.m_BossModal)
			{
				this.m_BossModal.transform.localScale = ((!value) ? Vector3.zero : new Vector3(1f, 1f, 1f));
			}
		}
	}

	public new string BodyBackgroundURL
	{
		set
		{
			this.Bak.URL = Super.GetWindowsBakImageURLFromName(value);
		}
	}

	private void InitTextInPrefabs()
	{
		this.txtShengyuChongbaiNum.X = 365.0;
		this.ConstTabBtnZhanli.Text = Global.GetLang("战力");
		this.ConstTabBtnLevel.Text = Global.GetLang("等级");
		this.ConstTabBtnCaifu.Text = Global.GetLang("财富");
		this.ConstTabBtnJingjichangZhanli.Text = Global.GetLang("竞技场");
		this.ConstTabBtnPata.Text = Global.GetLang("万魔塔");
		this.ConstTabBtnChibang.Text = Global.GetLang("翅膀");
		this.ConstLblName.text = Global.GetLang("角色");
		this.ConstLblTop.text = Global.GetLang("排行");
		this.m_BtnChongBai.Text = Global.GetLang("崇拜");
		this.LookPlayerInfoIcon.Text = Global.GetLang("查看");
		this.ConstLblShengyu.text = Global.GetLang("已崇拜次数:");
		this.ConstTabBtnXinWu.Text = Global.GetLang("信物");
		this.ConstTabBtnMeiLinMoFaShu.Text = Global.GetLang("梅林之书");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		int num = 10;
		string text = string.Format("{0}/{1}", Global.Data.roleData.AdorationCount, num);
		if (null != this.txtShengyuChongbaiNum)
		{
			this.txtShengyuChongbaiNum.text = text;
		}
		this.m_LblLastClick = null;
		this.ItemCollection = this.lbTopPlayer.ItemsSource;
		this.lbTopPlayer.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbTopPlayer_SelectionChanged);
		this.lbTopPlayer.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.lbTopPlayer_MouseLeftButtonUp);
		this.TabList.TabClick += delegate(GameObject s, int e)
		{
			if (this.m_nLastClickBtn == e)
			{
				return;
			}
			if (this.PaihangID[e] == 13)
			{
				this.isJingJiChang = true;
			}
			else
			{
				this.isJingJiChang = false;
			}
			this.ShowTypeList(this.PaihangID[e]);
			this.m_nLastClickBtn = e;
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.LookPlayerInfoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = this.m_SelectCurrentRoleID
			});
		};
		if (null != this.m_BtnChongBai)
		{
			this.m_BtnChongBai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				SystemHelpMgr.OnAction(UIObjIDs.PaiHangPartMoBai, HelpStateEvents.Clicked, 1);
				Global.SendEvent("800", Global.GetLang("排行榜崇拜次数"));
				GameInstance.Game.SpriteAdmiredOperation(this.m_SelectCurrentRoleID);
			};
		}
		UIHelper.SetModalPosZ(this.m_BossModal.transform);
		UIHelper.SetModalPosZ(this.m_ShouHuTianShi.transform);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.m_BtnChongBai, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public void ShowText(int nRoleSelfID, int nRoleTargetID, int nSelfAdmireCount, int nTargetAdmiredCount)
	{
		Global.Data.roleData.AdorationCount = nSelfAdmireCount;
		int num = 10;
		string text = string.Format("{0}/{1}", Global.Data.roleData.AdorationCount, num);
		if (null != this.txtShengyuChongbaiNum)
		{
			this.txtShengyuChongbaiNum.text = text;
		}
		PaiHangItemData paiHangItemData = this.CurrentPaiHangData.PaiHangList.Find((PaiHangItemData x) => x.RoleID == nRoleTargetID);
		if (paiHangItemData != null)
		{
			int num2 = paiHangItemData.Val3 + 1;
			RoleData roleData = Global.FindRoleDataByName(StringUtil.substitute("Role_{0}", new object[]
			{
				nRoleTargetID
			}));
			if (roleData != null && roleData.AdmiredCount > num2)
			{
				num2 = Math.Max(num2, roleData.AdmiredCount + 1);
				roleData.AdmiredCount = num2;
			}
			paiHangItemData.Val3 = num2;
		}
		if (null != this.txtBeiChongbaiNum)
		{
			this.txtBeiChongbaiNum.Label.text = Convert.ToString(nTargetAdmiredCount);
		}
		if (!(null != this.txtBeiChongbaiNum) || nRoleTargetID == this.CurrentPaiHangData.PaiHangList[this.lbTopPlayer.SelectedIndex].RoleID)
		{
		}
	}

	private void LoadTocken(int ringId)
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (null != this.m_BossModal)
		{
			this.m_BossModal.Clear();
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(ringId);
		this.m_LblWingName.text = goodsXmlNodeByID.Title;
		Modal3DShow bossModal = this.m_BossModal;
		U3DUtils.AddChild(base.gameObject, bossModal.gameObject, false);
		Transform transform = bossModal.gameObject.transform;
		transform.localPosition = new Vector3(295f, -30f, -500f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(bossModal.transform);
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
		}
		this.wingsResLoader = UIHelper.LoadGoodsRes(bossModal, ringId, 1f, 0.005f, 0, "UIModel", false);
	}

	private void LoadMagicBook(int jieNum, float scale = 1f)
	{
		XElement gameResXml = Global.GetGameResXml("Config/MagicBook.xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		string text = string.Empty;
		string text2 = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			text2 = Global.GetXElementAttributeStr(xelement, "Level");
			string text3 = jieNum.ToString();
			if (text2 == text3)
			{
				this.m_LblWingName.text = string.Format(Global.GetLang("梅林之书 {0} 阶"), text2);
				text = Global.GetXElementAttributeStr(xelement, "Mod");
			}
		}
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (null != this.m_BossModal)
		{
			this.m_BossModal.Clear();
		}
		string modelNameByID = Global.GetModelNameByID(Convert.ToInt32(text));
		Modal3DShow bossModal = this.m_BossModal;
		U3DUtils.AddChild(base.gameObject, bossModal.gameObject, false);
		Transform transform = this.m_BossModal.transform;
		transform.localPosition = new Vector3(295f, 20f, -500f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		UIHelper.SetModalPosZ(this.m_BossModal.transform);
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
		}
		this.resourceLoader = UIHelper.LoadModelResource(this.m_BossModal, Convert.ToInt32(text), scale, delegate(object s, DPSelectedItemEventArgs e)
		{
			Animation componentInChildren = this.m_BossModal.transform.GetChild(0).GetComponentInChildren<Animation>();
			this.StartCoroutine(this.PlayAni(componentInChildren, this.m_Modal3DShowTeXiao, jieNum));
		});
	}

	public IEnumerator PlayAni(Animation _Animation, Modal3DShow m_Modal3DShowTeXiao, int level)
	{
		if (!_Animation)
		{
			yield break;
		}
		if (_Animation.IsPlaying("Stand"))
		{
			_Animation.Play("Relax");
			AnimationState _AnimationClip = _Animation["Relax"];
			_AnimationClip.wrapMode = 2;
			DecorationVO cachingDecoConfig = ConfigDecoration.GetDecorationVOByCode(9000 + (int)((float)level / 2f - 0.5f));
			string resName = cachingDecoConfig.ResName;
			GDecoration deco = new GDecoration(resName);
			deco.Layer = LayerMask.NameToLayer("MUUI");
			deco.Position3D = m_Modal3DShowTeXiao.transform.position;
			deco.Start();
			deco.Parent = m_Modal3DShowTeXiao.transform;
			yield return new WaitForSeconds(_AnimationClip.length);
			base.StartCoroutine(this.PlayAni(_Animation, m_Modal3DShowTeXiao, level));
			yield return new WaitForSeconds(2f);
			Object.Destroy(deco.The3DGameObject);
		}
		else
		{
			_Animation.Play("Stand");
			AnimationState _AnimationClip2 = _Animation["Stand"];
			_AnimationClip2.wrapMode = 2;
			yield return new WaitForSeconds(_AnimationClip2.length * 2f);
			base.StartCoroutine(this.PlayAni(_Animation, m_Modal3DShowTeXiao, level));
		}
		yield break;
	}

	private void GetWingData(int zhuanNum, int nZhiYe = 0)
	{
		int num = Global.CalcOriginalOccupationID(nZhiYe);
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/Wing_{0}.xml", num));
		Debug.Log("Wingxml -- " + gameResXml);
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		string text = zhuanNum.ToString();
		string text2 = string.Empty;
		string text3 = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ID");
			if (xelementAttributeStr == text)
			{
				text2 = Global.GetXElementAttributeStr(xelement, "GLGoods");
				text3 = Global.GetXElementAttributeStr(xelement, "Name");
			}
		}
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (null != this.m_BossModal)
		{
			this.m_BossModal.Clear();
		}
		if (null != this.m_LblWingName)
		{
			this.m_LblWingName.text = text3;
		}
		if (string.Empty != text2)
		{
			if (this.wingsResLoaderBoss != null)
			{
				this.wingsResLoaderBoss.Stop();
			}
			this.wingsResLoaderBoss = UIHelper.LoadGoodsRes(this.m_BossModal, Convert.ToInt32(text2), 80f, 0.005f, 0, "Equip", true);
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData(int nType = 0)
	{
		this.TabList.TabIndex = nType;
		this.m_LblState.text = this.StrTopTypes[this.TabList.TabIndex];
	}

	public void InitRoleEquip()
	{
	}

	public void SetBonusIcon()
	{
	}

	public void ResetGetNewData()
	{
		this._PaiHangDataDict.Clear();
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void GetPaiHangData(string name)
	{
	}

	private int GetPaiHangDataCount()
	{
		if (this.CurrentPaiHangData == null)
		{
			return 0;
		}
		if (this.CurrentPaiHangData.PaiHangList == null)
		{
			return 0;
		}
		return this.CurrentPaiHangData.PaiHangList.Count;
	}

	private int GetMySelfPaiHangBangIndex()
	{
		if (this.CurrentPaiHangData == null)
		{
			return -1;
		}
		if (this.CurrentPaiHangData.PaiHangList == null)
		{
			return -1;
		}
		for (int i = 0; i < this.CurrentPaiHangData.PaiHangList.Count; i++)
		{
			if (this.CurrentPaiHangData.PaiHangList[i].RoleID == Global.Data.roleData.RoleID)
			{
				return i + 1;
			}
		}
		return -1;
	}

	protected IEnumerator InitListProc(int nPaiHangBangID)
	{
		this.ShowTypeList(nPaiHangBangID);
		yield return null;
		yield break;
	}

	private void ShowTypeList(int paiHangID)
	{
		this.m_nCurrentSelectPaiHangID = paiHangID;
		if (!this.GetListByTypeName(paiHangID))
		{
			return;
		}
		base.StartCoroutine(this.ShowTopPlayerListProc(paiHangID));
		if (null != this.m_LblState)
		{
			this.m_LblState.text = this.StrTopTypes[this.TabList.TabIndex];
		}
		int mySelfPaiHangBangIndex = this.GetMySelfPaiHangBangIndex();
		if (mySelfPaiHangBangIndex == -1)
		{
			this.ctMyNo.Text = Global.GetLang("未上榜");
		}
		else
		{
			string text = string.Format("{0}", mySelfPaiHangBangIndex.ToString());
			this.ctMyNo.Text = text;
		}
		if (null != this.m_ScrollBar)
		{
			this.m_ScrollBar.scrollValue = 0f;
		}
		this.m_nTime2 = U3DUtils.GetTimer();
		string text2 = string.Format(Global.GetLang("排行榜加载消耗{0}毫秒"), this.m_nTime2 - this.m_nTime3);
	}

	protected IEnumerator ShowTopPlayerListProc(int nPaiHangBangID)
	{
		this.ItemCollection.Clear();
		if (this.CurrentPaiHangData.PaiHangList == null)
		{
			yield return null;
		}
		if (this.CurrentPaiHangData != null && this.CurrentPaiHangData.PaiHangList != null)
		{
			for (int i = 0; i < this.CurrentPaiHangData.PaiHangList.Count; i++)
			{
				if (0 < this.CurrentPaiHangData.PaiHangList[i].RoleID)
				{
					TopListItemPlayers item = U3DUtils.NEW<TopListItemPlayers>();
					item.Height = 31.0;
					item.Rank = (i + 1).ToString();
					item.RoleID = this.CurrentPaiHangData.PaiHangList[i].RoleID;
					item.RoleName = this.CurrentPaiHangData.PaiHangList[i].RoleName;
					item.m_nZhiYe = this.CurrentPaiHangData.PaiHangList[i].Val3;
					Debug.Log("ID = = = " + item.m_nZhiYe);
					if (3 > i)
					{
						int nSpriteName = i + 1;
						item.m_SpriteNum.spriteName = Convert.ToString(nSpriteName);
						item.m_SpriteNum.gameObject.SetActive(true);
						item.txtRank.Label.gameObject.SetActive(false);
					}
					string strPower = string.Empty;
					if (nPaiHangBangID == 5)
					{
						strPower = string.Format(Global.GetLang("{0}转{1}级"), this.CurrentPaiHangData.PaiHangList[i].Val2.ToString(), this.CurrentPaiHangData.PaiHangList[i].Val1.ToString());
						item.Power = strPower;
					}
					else if (nPaiHangBangID == 13)
					{
						item.Power = this.CurrentPaiHangData.PaiHangList[i].Val2.ToString();
					}
					else if (nPaiHangBangID == 15)
					{
						strPower = string.Format(Global.GetLang("{0}阶{1}星"), this.CurrentPaiHangData.PaiHangList[i].Val1.ToString(), this.CurrentPaiHangData.PaiHangList[i].Val2.ToString());
						item.Power = strPower;
						item.JieNum = this.CurrentPaiHangData.PaiHangList[i].Val1;
						item.LevelNum = this.CurrentPaiHangData.PaiHangList[i].Val2;
					}
					else if (nPaiHangBangID == 16)
					{
						strPower = string.Format(Global.GetLang("{0}阶{1}星"), this.CurrentPaiHangData.PaiHangList[i].Val1.ToString(), this.CurrentPaiHangData.PaiHangList[i].Val2.ToString());
						item.Power = strPower;
					}
					else if (nPaiHangBangID == 17)
					{
						strPower = string.Format(Global.GetLang("{0}阶{1}星"), this.CurrentPaiHangData.PaiHangList[i].Val1.ToString(), this.CurrentPaiHangData.PaiHangList[i].Val2.ToString());
						item.Power = strPower;
						item.JieNum = this.CurrentPaiHangData.PaiHangList[i].Val1;
					}
					else
					{
						item.Power = this.CurrentPaiHangData.PaiHangList[i].Val1.ToString();
					}
					item.m_SpriteHotBak.gameObject.SetActive(false);
					item.m_SpriteNorMalBak.gameObject.SetActive(true);
					this.ItemCollection.AddNoUpdate(item);
				}
			}
			this.ItemCollection.DelayUpdate();
		}
		int oldSelectedIndex = 0;
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = this.lbTopPlayer.SelectedIndex;
		}
		if (null != this.txtBeiChongbaiNum && this.CurrentPaiHangData.PaiHangList != null && 0 > oldSelectedIndex)
		{
			oldSelectedIndex = 0;
		}
		this.SelectPlayerListBox(oldSelectedIndex);
		if (null != this.m_ScrollBar)
		{
			this.m_ScrollBar.scrollValue = 0f;
		}
		SystemHelpMgr.OnAction(UIObjIDs.PaiHangPart, HelpStateEvents.Actived, 1);
		yield return null;
		yield break;
	}

	private void ShowTopPlayerList(int nPaiHangBangID)
	{
		this.ItemCollection.Clear();
		if (this.CurrentPaiHangData.PaiHangList == null)
		{
			return;
		}
		if (this.CurrentPaiHangData != null && this.CurrentPaiHangData.PaiHangList != null)
		{
			for (int i = 0; i < this.CurrentPaiHangData.PaiHangList.Count; i++)
			{
				if (0 < this.CurrentPaiHangData.PaiHangList[i].RoleID)
				{
					TopListItemPlayers topListItemPlayers = U3DUtils.NEW<TopListItemPlayers>();
					topListItemPlayers.Height = 31.0;
					topListItemPlayers.Rank = (i + 1).ToString();
					topListItemPlayers.RoleID = this.CurrentPaiHangData.PaiHangList[i].RoleID;
					topListItemPlayers.RoleName = this.CurrentPaiHangData.PaiHangList[i].RoleName;
					if (3 > i)
					{
						int num = i + 1;
						topListItemPlayers.m_SpriteNum.spriteName = Convert.ToString(num);
						topListItemPlayers.m_SpriteNum.gameObject.SetActive(true);
						topListItemPlayers.txtRank.Label.gameObject.SetActive(false);
					}
					if (nPaiHangBangID == 5)
					{
						string power = string.Format(Global.GetLang("{0}转{1}级"), this.CurrentPaiHangData.PaiHangList[i].Val2.ToString(), this.CurrentPaiHangData.PaiHangList[i].Val1.ToString());
						topListItemPlayers.Power = power;
					}
					else if (nPaiHangBangID == 13)
					{
						topListItemPlayers.Power = this.CurrentPaiHangData.PaiHangList[i].Val2.ToString();
					}
					else
					{
						topListItemPlayers.Power = this.CurrentPaiHangData.PaiHangList[i].Val1.ToString();
					}
					topListItemPlayers.m_SpriteHotBak.gameObject.SetActive(false);
					topListItemPlayers.m_SpriteNorMalBak.gameObject.SetActive(true);
					this.ItemCollection.AddNoUpdate(topListItemPlayers);
				}
			}
			this.ItemCollection.DelayUpdate();
		}
		int num2 = 0;
		if (this.ItemCollection.Count > 0)
		{
			num2 = this.lbTopPlayer.SelectedIndex;
		}
		if (null != this.txtBeiChongbaiNum && this.CurrentPaiHangData.PaiHangList != null && 0 > num2)
		{
			num2 = 0;
		}
		this.SelectPlayerListBox(num2);
		if (null != this.m_ScrollBar)
		{
			this.m_ScrollBar.scrollValue = 0f;
		}
		SystemHelpMgr.OnAction(UIObjIDs.PaiHangPart, HelpStateEvents.Actived, 1);
	}

	private void lbTopMenu_SelectionChanged(object sender, object e)
	{
	}

	private void lbTopMenu_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void lbTopPlayer_MouseLeftButtonUp(object sender, MouseEvent e)
	{
	}

	private void lbTopPlayer_SelectionChanged(object sender, object e)
	{
		ListBox listBox = sender as ListBox;
		if (null != listBox)
		{
			GameObject itemByIndex = listBox.GetItemByIndex(listBox.SelectedIndex);
			if (null != itemByIndex)
			{
				TopListItemPlayers component = itemByIndex.GetComponent<TopListItemPlayers>();
				if (component == this.m_ShangYiciXuanZhong)
				{
					return;
				}
				if (null != component)
				{
					string text = string.Empty;
					text = string.Format("ID  {0},{1}", component.RoleID, component.RoleName);
					component.m_SpriteHotBak.gameObject.SetActive(true);
					if (null != this.m_ShangYiciXuanZhong && this.m_ShangYiciXuanZhong != component)
					{
						this.m_ShangYiciXuanZhong.m_SpriteHotBak.gameObject.SetActive(false);
					}
					this.m_ShangYiciXuanZhong = component;
					this.m_SelectCurrentRoleID = component.RoleID;
					if (this.m_nCurrentSelectPaiHangID == 15 || this.m_nCurrentSelectPaiHangID == 16 || this.m_nCurrentSelectPaiHangID == 17)
					{
						this.LookPlayerInfoIcon.gameObject.SetActive(false);
						this.m_ObjWing21.gameObject.SetActive(true);
						if (this.m_nCurrentSelectPaiHangID == 15)
						{
							this.GetWingData(component.JieNum, component.m_nZhiYe);
							this.m_BossModal.gameObject.transform.localPosition = new Vector3(this.m_BossModal.gameObject.transform.localPosition.x, 20f, this.m_BossModal.gameObject.transform.localPosition.z);
							Debug.Log(Global.GetLang("  m_BossModal 位置 ") + this.m_BossModal.gameObject.transform.localPosition);
						}
						if (this.m_nCurrentSelectPaiHangID == 16)
						{
							Debug.Log("ID xinwu ---- " + component.m_nZhiYe);
							this.LoadTocken(component.m_nZhiYe);
						}
						if (this.m_nCurrentSelectPaiHangID == 17)
						{
							this.LoadMagicBook(component.JieNum, 1f);
						}
					}
					else
					{
						this.m_BossModal.gameObject.transform.localPosition = new Vector3(this.m_BossModal.gameObject.transform.localPosition.x, -80f, this.m_BossModal.gameObject.transform.localPosition.z);
						this.LookPlayerInfoIcon.gameObject.SetActive(true);
						this.m_ObjWing21.gameObject.SetActive(false);
						Debug.Log(Global.GetLang("  m_BossModal 位置2 ") + this.m_BossModal.gameObject.transform.localPosition);
					}
					Super.ShowNetWaiting(string.Empty);
					if (this.isJingJiChang)
					{
						GameInstance.Game.GetJingJiChangRoleLooks(component.RoleID);
					}
					else
					{
						GameInstance.Game.SpriteGetUsingGoodsDataList(component.RoleID, false);
					}
				}
			}
		}
	}

	private void UnSelectItem(int i)
	{
	}

	private void SelectPlayerListBox(int oldSelectedIndex)
	{
		if (this.ItemCollection.Count > 0)
		{
			oldSelectedIndex = Global.GMin(oldSelectedIndex, this.ItemCollection.Count);
			int num = oldSelectedIndex;
			if (num < 0)
			{
				num = 0;
			}
			this.lbTopPlayer.SelectedIndex = num;
		}
		else
		{
			this.UnSelectItem(0);
		}
	}

	private GIcon FindEquipIcon(int equipCategory, int bagIndex = -1)
	{
		return null;
	}

	public void RefreshFrontShow()
	{
	}

	public void ClearAllDownloader()
	{
	}

	public void DownLoaderComplete1(object sender, DownloadEventArgs e)
	{
	}

	public bool GetImageFromCaching(string key, Image image)
	{
		return false;
	}

	public void DownloadNetImage(string value, Image image)
	{
	}

	private void InitNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.InitNoBorderWindow(noBorderWindow);
	}

	private void CloseNoBorderWindow(NoBorderWindow noBorderWindow)
	{
		Super.CloseNoBorderWindow(this.Container, noBorderWindow);
	}

	private void HideWindow()
	{
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
		this.HideWindow();
	}

	public void ShowMenuWindow(int px, int py, int[] ids, string[] names)
	{
	}

	private void ProcessMenuClick(int id)
	{
	}

	private void lbTopPlayer_MouseLeftButtonDown(object sender, MouseEvent e)
	{
	}

	private bool GetListByTypeName(int paiHangID)
	{
		PaiHangData currentPaiHangData = new PaiHangData();
		if (!this._PaiHangDataDict.ContainsKey(paiHangID))
		{
			Super.ShowNetWaiting(Global.GetLang("加载中..."));
			GameInstance.Game.SpriteGetPaiHang(paiHangID);
			return false;
		}
		currentPaiHangData = this._PaiHangDataDict.GetValue(paiHangID);
		this.CurrentPaiHangData = currentPaiHangData;
		return true;
	}

	public void NotifyPaiHangData(PaiHangData paiHangData)
	{
		Super.HideNetWaiting();
		this._PaiHangDataDict[paiHangData.PaiHangType] = paiHangData;
		this.ShowTypeList(paiHangData.PaiHangType);
		if (null != this.m_ScrollBar)
		{
			this.m_ScrollBar.scrollValue = 0f;
		}
	}

	public void NotifyPaiHangRoleData(RoleData roleData)
	{
	}

	public void NotifyFuBenHistDataDict(Dictionary<int, FuBenHistData> fuBenHistDataDict)
	{
	}

	public void ShowModalDialog()
	{
	}

	public void CloseModalDialog()
	{
	}

	private bool CloseMostTopChildWindow()
	{
		return false;
	}

	private GChildWindow GetMostTopChildWindow()
	{
		return null;
	}

	private bool MouseInAnyChildWindow(MouseEvent e)
	{
		return false;
	}

	private void RemoveDecoObject()
	{
	}

	public void SetMyBuffGicon()
	{
	}

	public void SetBuffGicon()
	{
	}

	private GIcon Geticon(string Type, string GoodsCod)
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.BodyURL = new ImageURL(StringUtil.substitute("Images/RoleAttr/{0}/{1}.png", new object[]
		{
			Type,
			GoodsCod
		}), false, 0);
		return gicon;
	}

	public void AddEffect()
	{
	}

	protected IEnumerator Init3DProc(RoleData4Selector roleData4Selector)
	{
		this.LoadRoleRes(roleData4Selector);
		yield return null;
		yield break;
	}

	public void GetRoleUsingGoodsDataList(RoleData4Selector roleData4Selector)
	{
		if (this.m_nCurrentSelectPaiHangID == 15)
		{
			this.txtBeiChongbaiNum.Label.text = Convert.ToString(roleData4Selector.AdmiredCount);
			return;
		}
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (roleData4Selector == null)
		{
			return;
		}
		if (this.m_nCurrentSelectPaiHangID == 16)
		{
			this.txtBeiChongbaiNum.Label.text = Convert.ToString(roleData4Selector.AdmiredCount);
			return;
		}
		if (this.m_nCurrentSelectPaiHangID == 17)
		{
			this.txtBeiChongbaiNum.Label.text = Convert.ToString(roleData4Selector.AdmiredCount);
			return;
		}
		this.txtBeiChongbaiNum.Label.text = Convert.ToString(roleData4Selector.AdmiredCount);
		base.StartCoroutine(this.Init3DProc(roleData4Selector));
	}

	public override void Destroy()
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
		if (this.wingsResLoaderBoss != null)
		{
			this.wingsResLoaderBoss.Stop();
			this.wingsResLoaderBoss = null;
		}
		if (this.resourceLoader != null)
		{
			this.resourceLoader.Stop();
			this.resourceLoader = null;
		}
		base.Destroy();
	}

	private void LoadRoleRes(RoleData4Selector roleData4Selector)
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		int fashionGoodsID = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
		if (null != this.m_BossModal)
		{
			this.m_BossModal.Clear();
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		this.roleResLoader = UIHelper.LoadRoleRes(this.m_BossModal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1.7f, fashionGoodsID, null, false);
	}

	public ShowNetImage Bak;

	public GButton CloseBtn;

	public GButton LookPlayerInfoIcon;

	public GButton m_BtnChongBai;

	public int m_SelectCurrentRoleID;

	public UITab TabList;

	public UILabel m_LblLastClick;

	public Color m_OldColor = Color.white;

	public UIScrollBar m_ScrollBar;

	public UILabel m_LblState;

	public TextBlock ctTitle;

	public UITexture m_Title2;

	public TextBlock ctMyNo;

	public TextBlock txtBeiChongbaiNum;

	public TextBlock txtShengyuChongbaiNum;

	public UILabel m_LblWingName;

	public ListBox lbTopPlayer;

	public GButton ConstTabBtnZhanli;

	public GButton ConstTabBtnLevel;

	public GButton ConstTabBtnCaifu;

	public GButton ConstTabBtnJingjichangZhanli;

	public GButton ConstTabBtnPata;

	public GButton ConstTabBtnChibang;

	public GButton ConstTabBtnXinWu;

	public GButton ConstTabBtnMeiLinMoFaShu;

	public UILabel ConstLblName;

	public UILabel ConstLblTop;

	public UILabel ConstLblShengyu;

	private string[] StrTopTypes = new string[]
	{
		Global.GetLang("战力"),
		Global.GetLang("等级"),
		Global.GetLang("财富"),
		Global.GetLang("竞技场战力"),
		Global.GetLang("万魔塔"),
		Global.GetLang("翅膀"),
		Global.GetLang("信物"),
		Global.GetLang("梅林之书")
	};

	private int[] PaihangID = new int[]
	{
		12,
		5,
		6,
		13,
		14,
		15,
		16,
		17
	};

	public int m_nCurrentSelectPaiHangID = 12;

	public Modal3DShow m_BossModal;

	public Modal3DShow m_ShouHuTianShi;

	public Modal3DShow m_Modal3DShowTeXiao;

	public GameObject m_RenWuModal;

	public GameObject m_ShouHuChongWu;

	public GameObject m_ChiBang;

	public GameObject m_ObjWing21;

	public List<GameObject> m_WuQiList;

	public TopListItemPlayers m_LastSelectItem;

	private PaiHangData CurrentPaiHangData;

	private Dictionary<int, PaiHangData> _PaiHangDataDict = new Dictionary<int, PaiHangData>();

	private LoadingWindow LoadingWin;

	public DPSelectedItemEventHandler DPSelectedItem;

	private TopListItemPlayers m_ShangYiciXuanZhong;

	private int m_nLastClickBtn = -1;

	private bool isJingJiChang;

	private ObservableCollection _ItemCollection;

	private bool _bIsModalState = true;

	private int m_nTime2;

	private int m_nTime3;

	private WingsResLoader wingsResLoader;

	private ResourceResLoader resourceLoader;

	private WingsResLoader wingsResLoaderBoss;

	private GameObject oldRole;

	private RoleResLoader roleResLoader;
}
