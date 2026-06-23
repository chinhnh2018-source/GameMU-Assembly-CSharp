using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ChiBangPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.goodsTsBtn.Text = Global.GetLang("道具提升");
		this.zuanshiTsBtn.Text = Global.GetLang("自动提升");
		this.goodsNum.Text = Global.GetLang("提升\n消耗:");
		this.goodsJJBtn.Text = Global.GetLang("道具进阶");
		this.zuanshiJJBtn.Text = Global.GetLang("自动进阶");
		this.jjgoodsNum.Text = Global.GetLang("进阶\n消耗:");
		this.dingJinText.Text = Global.GetLang("已达到顶阶");
		this.BtnTSdingji.Text = Global.GetLang("道具提升");
		this.statAnim.gameObject.SetActive(false);
		this.SetPeiDaiBtn();
		this.tsJingduProgressBar.Percent = 0.0;
		this.Dingji.gameObject.SetActive(false);
		this.jjgoodsNum.X = 450.0;
		this.jjZSName.X = 375.0;
		this.jjgoodsName.FontSize = 18;
		this.jjgoodsName.X = 6.0;
		this.jjCheckBox.transform.localPosition = new Vector3(-20f, -70f, 0f);
		this.zsNeedText.X = 335.0;
		this.BtnTSdingji.transform.localPosition = new Vector3(this.BtnTSdingji.transform.localPosition.x, this.BtnTSdingji.transform.localPosition.y, -0.1f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		this.tsLevProgBar.ItemWidth = 50f;
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.peidaiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			SystemHelpMgr.OnAction(UIObjIDs.ChiBangPartTiPeiDaiBtn, HelpStateEvents.Clicked, -1);
			if (Global.Data.roleData.MyWingData.Using == 1)
			{
				GoodsData goodsData = Global.GetRoleChiBangFashionList(true).Find((GoodsData f) => 1 == f.Using);
				if (goodsData != null)
				{
					GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, 0, 6000, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
			GameInstance.Game.SpriteWingoffonCmd();
		};
		if (!this.BtnLingyu.gameObject.activeSelf || !this.BtnZhuling.gameObject.activeSelf || !this.BtnZhuhun.gameObject.activeSelf)
		{
			if (GongnengYugaoMgr.IsIconOpened(50))
			{
				this.BtnLingyu.gameObject.gameObject.SetActive(true);
			}
			if (GongnengYugaoMgr.IsIconOpened(51))
			{
				this.BtnZhuling.gameObject.gameObject.SetActive(true);
			}
			if (GongnengYugaoMgr.IsIconOpened(52))
			{
				this.BtnZhuhun.gameObject.gameObject.SetActive(true);
			}
		}
		UIEventListener.Get(this.BtnLingyu.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowChibangLingyuWindow();
		};
		UIEventListener.Get(this.BtnZhuling.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowZhuLingWindows();
		};
		UIEventListener.Get(this.BtnZhuhun.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowZhuHunWindows();
		};
		this.goodsTsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartGoodsTiSheng();
		};
		this.zuanshiTsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.zuanshiTsBtn.Text == Global.GetLang("取消提升"))
			{
				this.StopAutoTiSheng();
			}
			else
			{
				this.zuanshiTsBtn.Text = Global.GetLang("取消提升");
				this.zuanshiTsBtn.normalSprite = "tongyongBtn6_normal";
				this.zuanshiTsBtn.Pressed = true;
				base.InvokeRepeating("AutoTiSheng_Tick", 1f, 0.5f);
			}
		};
		this.jjCheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.jjCheckBox.Check)
			{
				this.jjCheckBox.Check = false;
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ChiBangShengJie", this.zuanshiXiaoHao);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("选择后每次需要消耗{0}，确定执行吗？"), text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.jjCheckBox.Check = true;
					}
					else
					{
						this.jjCheckBox.Check = false;
					}
					return true;
				};
				return;
			}
		};
		this.tsCheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.tsCheckBox.Check)
			{
				this.tsCheckBox.Check = false;
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ChiBangShengXing", this.zuanshiXiaoHao);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("选择后每次需要消耗{0}，确定执行吗？"), text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.tsCheckBox.Check = true;
					}
					else
					{
						this.tsCheckBox.Check = false;
					}
					return true;
				};
				return;
			}
		};
		this.goodsJJBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartGoodsJinJie();
		};
		this.zuanshiJJBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.zuanshiJJBtn.Text == Global.GetLang("取消进阶"))
			{
				this.StopAutoJingJie();
			}
			else
			{
				this.zuanshiJJBtn.Text = Global.GetLang("取消进阶");
				this.zuanshiJJBtn.normalSprite = "tongyongBtn6_normal";
				this.zuanshiJJBtn.Pressed = true;
				base.InvokeRepeating("AutoJingJie_Tick", 1f, 0.5f);
			}
		};
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	private void StartGoodsTiSheng()
	{
		bool flag = false;
		IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ChiBangShengJie", this.zuanshiXiaoHao, false);
		int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(2016, "WingSuit", ref flag, (long)Global.Data.roleData.MyWingData.WingID);
		if (this.tsNeedGoodsNum > Global.GetTotalGoodsCountByID(2016) + replaceGoodCount)
		{
			this.StopAutoTiSheng();
			if (Super.ShowGoodsGuide(2016, this.callback) == 1)
			{
				Super.HintMainText(Global.GetLang("所需物品数量不够！"), 10, 3);
			}
			if (!this.tsCheckBox.Check)
			{
				this.checkboxAnim.SetActive(false);
				this.checkboxAnim.SetActive(true);
			}
		}
		else
		{
			this.itsType = 0;
			GameInstance.Game.SpriteWingUpStarCmd(0);
			SystemHelpMgr.OnAction(UIObjIDs.ChiBangPartTiShengBtn, HelpStateEvents.Clicked, -1);
		}
	}

	private void StartGoodsJinJie()
	{
		bool flag = false;
		int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(2017, "WingSuit", ref flag, (long)Global.Data.roleData.MyWingData.WingID);
		if (this.jjNeedGoodsNum > Global.GetTotalGoodsCountByID(2017) + replaceGoodCount)
		{
			this.StopAutoJingJie();
			if (Super.ShowGoodsGuide(2017, this.callback) == 1)
			{
				Super.HintMainText(Global.GetLang("所需物品数量不够！"), 10, 3);
			}
		}
		else
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ChiBangShengJie", this.zuanshiXiaoHao, false);
			GameInstance.Game.SpriteWingUpgradeCmd(0);
		}
	}

	public void InitPartData()
	{
		if (Global.Data.roleData.MyWingData == null)
		{
			return;
		}
		this.tempWingStarExp = Global.Data.roleData.MyWingData.StarExp;
		this.Wingxml = Global.GetGameResXml(string.Format("Config/Wing/Wing_{0}.xml", this.occupation));
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(this.Wingxml, "Config"), "*");
		this.ItemCollection.Clear();
		for (int i = 1; i <= xelementList.Count; i++)
		{
			XElement xelement = xelementList[i - 1];
			if (xelement != null)
			{
				ChiBangItem chiBangItem = U3DUtils.NEW<ChiBangItem>();
				this.ItemCollection.Add(chiBangItem);
				chiBangItem.icon.GoodImg.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", Global.GetGoodsIconCodeByID(Global.GetXElementAttributeInt(xelement, "GLGoods")));
				chiBangItem.gameObject.AddComponent<UIDragPanelContents>();
				chiBangItem.icon.Tag = i.ToString();
				if (i > Global.Data.roleData.MyWingData.WingID)
				{
					chiBangItem.icon.GoodImg.ToGrayBitmap = true;
				}
			}
		}
		if (Global.Data.roleData.MyWingData != null)
		{
			this.listBox.SelectedIndex = Global.Data.roleData.MyWingData.WingID - 1;
		}
		else
		{
			this.listBox.SelectedIndex = 0;
		}
		this.SetChiBang();
		this.ShowChengBangType();
		if (this.mClickHelpAnim)
		{
			SystemHelpPart.SetMask(this.goodsTsBtn, default(Vector4));
			this.mClickHelpAnim = false;
		}
	}

	public override void Destroy()
	{
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
			this.wingsResLoader = null;
		}
		base.Destroy();
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		ChiBangItem chiBangItem = U3DUtils.AS<ChiBangItem>(this.listBox.SelectedItem);
		if (null == chiBangItem)
		{
			return;
		}
		if (this.temp != null && this.temp != chiBangItem)
		{
			this.temp.stat.gameObject.SetActive(false);
		}
		this.temp = chiBangItem;
		this.temp.stat.gameObject.SetActive(true);
		this.InitChiBangInfo(chiBangItem.icon.Tag.ToString());
		XElement xelement = Global.GetXElement(this.Wingxml, "Level", "ID", chiBangItem.icon.Tag.ToString());
		if (xelement == null)
		{
			return;
		}
		if (null != this.ChiBangModal)
		{
			Object.Destroy(this.ChiBangModal.gameObject);
			this.ChiBangModal = null;
		}
		this.ChiBangModal = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(base.gameObject, this.ChiBangModal.gameObject, false);
		Transform transform = this.ChiBangModal.transform;
		transform.localPosition = new Vector3(-218f, 80f, -0.8f);
		transform.localRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		UIHelper.SetModalPosZ(this.ChiBangModal.transform);
		if (this.wingsResLoader != null)
		{
			this.wingsResLoader.Stop();
		}
		this.wingsResLoader = UIHelper.LoadGoodsRes(this.ChiBangModal, Global.GetXElementAttributeInt(xelement, "GLGoods"), 80f, 0.005f, 0, "Equip", true);
	}

	private void InitChiBangInfo(string chibangID)
	{
		if (Global.Data.roleData.MyWingData == null)
		{
			return;
		}
		bool isMinAttackBeZero = false;
		bool isMinMAttackBeZero = false;
		this.jieshuText.Text = chibangID;
		XElement xelement = Global.GetXElement(this.Wingxml, "Level", "ID", chibangID);
		if (xelement == null)
		{
			return;
		}
		this.title.Text = Global.GetXElementAttributeStr(xelement, "Name");
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinAttackV");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxAttackV");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinMAttackV");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxMAttackV");
		int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MinDefenseV");
		int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "MaxDefenseV");
		int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement, "MinMDefenseV");
		int xelementAttributeInt8 = Global.GetXElementAttributeInt(xelement, "MaxMDefenseV");
		int xelementAttributeInt9 = Global.GetXElementAttributeInt(xelement, "HitV");
		int xelementAttributeInt10 = Global.GetXElementAttributeInt(xelement, "Dodge");
		int xelementAttributeInt11 = Global.GetXElementAttributeInt(xelement, "MaxLifeV");
		this.intBasSubAttackInjurePercent = Global.GetXElementAttributeDouble(xelement, "SubAttackInjurePercent");
		this.intBasAddAttackInjurePercent = Global.GetXElementAttributeDouble(xelement, "AddAttackInjurePercent");
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		int num11 = 0;
		int num12 = 1;
		if (Global.Data.roleData.MyWingData.WingID < Convert.ToInt32(chibangID))
		{
			num12 = 10;
		}
		else if (Global.Data.roleData.MyWingData.WingID == Convert.ToInt32(chibangID))
		{
			num12 = Global.Data.roleData.MyWingData.ForgeLevel;
		}
		XElement wingStarXmlNode = this.GetWingStarXmlNode(chibangID, num12.ToString());
		if (wingStarXmlNode != null)
		{
			num = Global.GetXElementAttributeInt(wingStarXmlNode, "MinAttackV");
			num2 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxAttackV");
			num3 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinMAttackV");
			num4 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMAttackV");
			num5 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinDefenseV");
			num6 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxDefenseV");
			num7 = Global.GetXElementAttributeInt(wingStarXmlNode, "MinMDefenseV");
			num8 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMDefenseV");
			num9 = Global.GetXElementAttributeInt(wingStarXmlNode, "MaxLifeV");
			num10 = Global.GetXElementAttributeInt(wingStarXmlNode, "HitV");
			num11 = Global.GetXElementAttributeInt(wingStarXmlNode, "Dodge");
		}
		this.intTotalMinAttackV = xelementAttributeInt + num;
		this.intTotalMaxAttackV = xelementAttributeInt2 + num2;
		this.intTotalMinMAttackV = xelementAttributeInt3 + num3;
		this.intTotalMaxMAttackV = xelementAttributeInt4 + num4;
		this.intTotalMinDefenseV = xelementAttributeInt5 + num5;
		this.intTotalMaxDefenseV = xelementAttributeInt6 + num6;
		this.intTotalMinMDefenseV = xelementAttributeInt7 + num7;
		this.intTotalMaxMDefenseV = xelementAttributeInt8 + num8;
		this.intTotalMaxLifeV = xelementAttributeInt11 + num9;
		this.intTotalHitVV = xelementAttributeInt9 + num10;
		this.intTotalDodgeV = xelementAttributeInt10 + num11;
		if (xelementAttributeInt != 0)
		{
			this.wgongText.Text = string.Format(Global.GetLang("{{c39550}}物理攻击{{-}} {0}-{1}"), this.intTotalMinAttackV, this.intTotalMaxAttackV);
		}
		else
		{
			this.wgongText.gameObject.SetActive(false);
			this.bwgongText.transform.parent.gameObject.SetActive(false);
			isMinAttackBeZero = true;
		}
		if (xelementAttributeInt3 != 0)
		{
			this.mgongText.Text = string.Format(Global.GetLang("{{c39550}}魔法攻击{{-}} {0}-{1}"), this.intTotalMinMAttackV, this.intTotalMaxMAttackV);
		}
		else
		{
			this.mgongText.gameObject.SetActive(false);
			this.bmgongText.transform.parent.gameObject.SetActive(false);
			isMinMAttackBeZero = true;
		}
		this.wfangText.Text = string.Format(Global.GetLang("{{c39550}}物理防御{{-}} {0}-{1}"), this.intTotalMinDefenseV, this.intTotalMaxDefenseV);
		this.mfangText.Text = string.Format(Global.GetLang("{{c39550}}魔法防御{{-}} {0}-{1}"), this.intTotalMinMDefenseV, this.intTotalMaxMDefenseV);
		this.hitvText.Text = string.Format(Global.GetLang("{{c39550}}命       中{{-}} {0}"), this.intTotalHitVV);
		this.dodgeText.Text = string.Format(Global.GetLang("{{c39550}}闪       避{{-}} {0}"), this.intTotalDodgeV);
		this.shengmingText.Text = string.Format(Global.GetLang("{{c39550}}生命上限{{-}} {0}"), this.intTotalMaxLifeV);
		this.xishouText.Text = string.Format(Global.GetLang("{{c39550}}伤害吸收{{-}} {0}%"), this.intBasSubAttackInjurePercent * 100.0);
		this.jiachengText.Text = string.Format(Global.GetLang("{{c39550}}伤害加成{{-}} {0}%"), this.intBasAddAttackInjurePercent * 100.0);
		if (Global.Data.roleData.MyWingData.WingID.ToString() == chibangID)
		{
			this.peidaiBtn.gameObject.SetActive(true);
			this.duibiPanel.gameObject.SetActive(true);
			this.SetPeiDaiBtn();
			this.SetDuiBiInfo();
		}
		else
		{
			this.duibiPanel.gameObject.SetActive(false);
			this.peidaiBtn.gameObject.SetActive(false);
		}
		this.SetInfoPosition(isMinAttackBeZero, isMinMAttackBeZero);
	}

	private void SetInfoPosition(bool IsMinAttackBeZero, bool IsMinMAttackBeZero)
	{
		float topPosY = 140f;
		float deltaY = 25f;
		if (!IsMinAttackBeZero && IsMinMAttackBeZero)
		{
			this.SetGoArrayYPosition(topPosY, deltaY, false, new Transform[]
			{
				this.wgongText.transform,
				this.wfangText.transform,
				this.mfangText.transform,
				this.hitvText.transform,
				this.dodgeText.transform,
				this.shengmingText.transform,
				this.xishouText.transform,
				this.jiachengText.transform
			});
			this.SetGoArrayYPosition(topPosY, deltaY, true, new Transform[]
			{
				this.bwgongText.transform,
				this.bwfangText.transform,
				this.bmfangText.transform,
				this.bhitvText.transform,
				this.bdodgeText.transform,
				this.bshengmingText.transform,
				this.bxishouText.transform,
				this.bjiachengText.transform
			});
		}
		else if (IsMinAttackBeZero && !IsMinMAttackBeZero)
		{
			this.SetGoArrayYPosition(topPosY, deltaY, false, new Transform[]
			{
				this.mgongText.transform,
				this.wfangText.transform,
				this.mfangText.transform,
				this.hitvText.transform,
				this.dodgeText.transform,
				this.shengmingText.transform,
				this.xishouText.transform,
				this.jiachengText.transform
			});
			this.SetGoArrayYPosition(topPosY, deltaY, true, new Transform[]
			{
				this.bmgongText.transform,
				this.bwfangText.transform,
				this.bmfangText.transform,
				this.bhitvText.transform,
				this.bdodgeText.transform,
				this.bshengmingText.transform,
				this.bxishouText.transform,
				this.bjiachengText.transform
			});
		}
	}

	private void SetGoArrayYPosition(float topPosY, float deltaY, bool IsContainBrother, params Transform[] Gos)
	{
		for (int i = 0; i < Gos.Length; i++)
		{
			if (IsContainBrother)
			{
				for (int j = 0; j < Gos[i].parent.childCount; j++)
				{
					Transform child = Gos[i].parent.GetChild(j);
					child.localPosition = new Vector3(child.localPosition.x, topPosY - (float)i * deltaY, child.localPosition.z);
				}
			}
			else
			{
				Gos[i].localPosition = new Vector3(Gos[i].localPosition.x, topPosY - (float)i * deltaY, Gos[i].localPosition.z);
			}
		}
	}

	public void SetPeiDaiBtn()
	{
		if (Global.Data.roleData.MyWingData.Using == 1)
		{
			this.peidaiBtn.Label.text = Global.GetLang("卸下");
			this.peidaiBtn.normalSprite = this.BtnSpriteNames[1];
			this.peidaiBtn.target.spriteName = this.BtnSpriteNames[1];
		}
		else
		{
			this.peidaiBtn.Label.text = Global.GetLang("佩戴");
			this.peidaiBtn.normalSprite = this.BtnSpriteNames[0];
			this.peidaiBtn.target.spriteName = this.BtnSpriteNames[0];
		}
	}

	private XElement GetWingStarXmlNode(string id, string starid)
	{
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/WingStar_{0}.xml", this.occupation));
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "Wing", "ID", id);
		if (xelement == null)
		{
			return null;
		}
		xelement = Global.GetXElement(xelement, "Item", "Star", starid);
		if (xelement == null)
		{
			return null;
		}
		return xelement;
	}

	public void SetChiBang()
	{
		if (Global.Data.roleData.MyWingData.WingID == Convert.ToInt32(this.temp.icon.Tag.ToString()))
		{
			this.InitChiBangInfo(Global.Data.roleData.MyWingData.WingID.ToString());
		}
		WingData myWingData = Global.Data.roleData.MyWingData;
		if (myWingData == null)
		{
			return;
		}
		if (myWingData.ForgeLevel != 10)
		{
			this.objectJJ.gameObject.SetActive(false);
			this.objectTS.gameObject.SetActive(true);
			this.StopAutoJingJie();
			XElement wingStarXmlNode = this.GetWingStarXmlNode(myWingData.WingID.ToString(), (myWingData.ForgeLevel + 1).ToString());
			if (wingStarXmlNode != null)
			{
				this.tsJingduProgressBar.Percent = (double)myWingData.StarExp / Global.GetXElementAttributeDouble(wingStarXmlNode, "StarExp");
				this.tsjinduText.Text = string.Format("{0}/{1}", myWingData.StarExp, Global.GetXElementAttributeDouble(wingStarXmlNode, "StarExp"));
				if (this.itsType != -1)
				{
					if (this.hintPart == null)
					{
						Vector3 localPosition;
						localPosition..ctor(-220f, -78f, -3f);
						this.hintPart = U3DUtils.NEW<GetGoodsHintPart>();
						this.hintPart.transform.localPosition = localPosition;
						base.Children.Add(this.hintPart);
					}
					string attributeName = (this.itsType != 0) ? "ZuanShiExp" : "GoodsExp";
					if ((double)(myWingData.StarExp - this.tempWingStarExp) > Global.GetXElementAttributeDouble(wingStarXmlNode, attributeName))
					{
						this.hintPart.AddTextItem(1, string.Format(Global.GetLang("{{00ff00}}爆增 +{0}{{-}}"), myWingData.StarExp - this.tempWingStarExp));
						this.Tsanimation.spriteName = "WhiteLine";
					}
					else if (myWingData.StarExp - this.tempWingStarExp > 0)
					{
						this.hintPart.AddTextItem(1, string.Format(Global.GetLang("{{00ff00}}+{0}{{-}}"), myWingData.StarExp - this.tempWingStarExp));
						this.Tsanimation.spriteName = "WhiteEdge";
					}
					this.Anim.gameObject.SetActive(true);
					this.PlayStart(this.Anim, new ActiveAnimation.OnFinished(this.PlayFinished));
				}
				if (this.tsLevProgBar.Level < myWingData.ForgeLevel)
				{
					if (this.tsLevProgBar.Level > 0)
					{
						this.statAnim.gameObject.SetActive(true);
					}
					this.statAnim.transform.localPosition = new Vector3((float)(59 + this.tsLevProgBar.Level * 21), 40f, -1f);
					this.PlayStart(this.statAnim, new ActiveAnimation.OnFinished(this.PlayFinished));
				}
				this.tsLevProgBar.ItemWidth = 21f;
				this.tsLevProgBar.MaxLevel = 10;
				this.tsLevProgBar.Level = myWingData.ForgeLevel;
				string[] array = Global.GetXElementAttributeStr(wingStarXmlNode, "NeedGoods").Split(new char[]
				{
					','
				});
				this.tsNeedIcon.BackSpriteName0 = "bagGrid4_bak";
				this.tsNeedIcon.GoodImg.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", Global.GetGoodsIconCodeByID(Convert.ToInt32(array[0])));
				bool flag = false;
				int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(Convert.ToInt32(array[0]), "WingSuit", ref flag, (long)Global.Data.roleData.MyWingData.WingID);
				this.tsNeedIcon.SecondText.Text = string.Format("{0}/{1}", Global.GetTotalGoodsCountByID(Convert.ToInt32(array[0])) + replaceGoodCount, array[1]);
				this.goodsName.Text = string.Format(Global.GetLang("【{0}】"), Global.GetGoodsNameByID(Convert.ToInt32(array[0]), false));
				this.tsNeedGoodsNum = Convert.ToInt32(array[1]);
				this.zuanshiXiaoHao = Global.GetXElementAttributeStr(wingStarXmlNode, "NeedZuanShi").SafeToInt32(0);
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listSpDaiBiShengXing[0], "ChiBangShengXing", this.zuanshiXiaoHao, string.Empty);
				this.zsNeedText.Text = string.Format("{0}", Global.GetXElementAttributeStr(wingStarXmlNode, "NeedZuanShi"));
				if (IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("ChiBangShengXing", this.zuanshiXiaoHao) && this.tsCheckBox.Check)
				{
					this.StopAutoTiSheng();
				}
			}
		}
		else
		{
			XElement gameResXml = Global.GetGameResXml("Config/Wing/WingUp.xml");
			if (gameResXml == null)
			{
				return;
			}
			this.StopAutoTiSheng();
			XElement xelement = Global.GetXElement(gameResXml, "Star", "Level", (myWingData.WingID + 1).ToString());
			if (xelement != null)
			{
				double xelementAttributeDouble = Global.GetXElementAttributeDouble(xelement, "LuckyOne");
				this.jjJingduProgressBar.Percent = (double)myWingData.JinJieFailedNum / (110000.0 - xelementAttributeDouble + (110000.0 - xelementAttributeDouble) * 0.2);
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				string[] array2 = xelementAttributeStr.Split(new char[]
				{
					','
				});
				this.jjNeedIcon.BackSpriteName0 = "bagGrid4_bak";
				this.jjNeedIcon.GoodImg.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png", Global.GetGoodsIconCodeByID(Convert.ToInt32(array2[0])));
				bool flag2 = false;
				int replaceGoodCount2 = ConfigReplaceGoodVO.GetReplaceGoodCount(Convert.ToInt32(array2[0]), "WingSuit", ref flag2, (long)Global.Data.roleData.MyWingData.WingID);
				this.jjNeedIcon.SecondText.Text = string.Format("{0}/{1}", Global.GetTotalGoodsCountByID(Convert.ToInt32(array2[0])) + replaceGoodCount2, array2[1]);
				this.jjgoodsName.Text = string.Format(Global.GetLang("【{0}】"), Global.GetGoodsNameByID(Convert.ToInt32(array2[0]), false));
				this.jjNeedGoodsNum = Convert.ToInt32(array2[1]);
				this.zuanshiXiaoHao = Global.GetXElementAttributeStr(xelement, "NeedZuanShi").SafeToInt32(0);
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listSpDaiBiJinJie[0], "ChiBangShengJie", this.zuanshiXiaoHao, string.Empty);
				this.jjZSName.Text = string.Format("{0}", Global.GetXElementAttributeStr(xelement, "NeedZuanShi"));
				this.objectJJ.gameObject.SetActive(true);
				this.objectTS.gameObject.SetActive(false);
				NGUITools.SetActive(this.jjNeedIcon.BackgroundSprite0, true);
				if (IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("ChiBangShengJie", this.zuanshiXiaoHao) && this.jjCheckBox.Check)
				{
					this.StopAutoJingJie();
				}
			}
		}
		if (myWingData.WingID >= 12 && myWingData.ForgeLevel == 10)
		{
			this.objectTS.gameObject.SetActive(false);
			this.objectJJ.gameObject.SetActive(false);
			this.Dingji.gameObject.SetActive(true);
			this.BtnTSdingji.isEnabled = false;
		}
		else
		{
			this.Dingji.gameObject.SetActive(false);
		}
		this.tempWingStarExp = myWingData.StarExp;
		Global.ShowChiBangRedPoint();
	}

	public void JingJieChengGong()
	{
		this.listBox.SelectedIndex = Global.Data.roleData.MyWingData.WingID - 1;
		this.ShowChengBangType();
		this.temp.icon.GoodImg.ToGrayBitmap = false;
		this.StopAutoJingJie();
		Transform deco = U3DUtils.NEW<Transform>("Wing_JinJieChengGong");
		if (null != deco)
		{
			U3DUtils.AddChild(base.gameObject, deco.gameObject, false);
			deco.transform.localPosition = Vector3.back * 2f;
			UIHelper.DelayInvoke(1.5f, delegate(object s, EventArgs e)
			{
				Object.Destroy(deco.gameObject);
			});
		}
		Global.ShowChiBangRedPoint();
	}

	private void ShowChengBangType()
	{
		int wingID = Global.Data.roleData.MyWingData.WingID;
		if (wingID > 3 && wingID < 7)
		{
			this.UIDragPl.target.x = 349f;
			this.UIDragPl.target.y = 364f;
			this.UIDragPl.enabled = true;
		}
		else if (wingID > 6 && wingID < 9)
		{
			this.UIDragPl.target.x = 349f;
			this.UIDragPl.target.y = 695f;
			this.UIDragPl.enabled = true;
		}
		else if (wingID >= 9)
		{
			this.UIDragPl.target.x = 349f;
			this.UIDragPl.target.y = 1040f;
			this.UIDragPl.enabled = true;
		}
	}

	private void SetDuiBiInfo()
	{
		int wingID = Global.Data.roleData.MyWingData.WingID;
		int forgeLevel = Global.Data.roleData.MyWingData.ForgeLevel;
		if (forgeLevel < 10)
		{
			XElement wingStarXmlNode = this.GetWingStarXmlNode(wingID.ToString(), forgeLevel.ToString());
			XElement wingStarXmlNode2 = this.GetWingStarXmlNode(wingID.ToString(), (forgeLevel + 1).ToString());
			if (wingStarXmlNode != null)
			{
				if (Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxAttackV") != 0)
				{
					this.bwgongText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxAttackV") - Global.GetXElementAttributeInt(wingStarXmlNode, "MaxAttackV"));
				}
				if (Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMAttackV") != 0)
				{
					this.bmgongText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMAttackV") - Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMAttackV"));
				}
				this.bwfangText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxDefenseV") - Global.GetXElementAttributeInt(wingStarXmlNode, "MaxDefenseV"));
				this.bmfangText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMDefenseV") - Global.GetXElementAttributeInt(wingStarXmlNode, "MaxMDefenseV"));
				this.bhitvText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "HitV") - Global.GetXElementAttributeInt(wingStarXmlNode, "HitV"));
				this.bdodgeText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "Dodge") - Global.GetXElementAttributeInt(wingStarXmlNode, "Dodge"));
				this.bshengmingText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxLifeV") - Global.GetXElementAttributeInt(wingStarXmlNode, "MaxLifeV"));
			}
			else
			{
				if (Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxAttackV") != 0)
				{
					this.bwgongText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxAttackV"));
				}
				if (Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMAttackV") != 0)
				{
					this.bmgongText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMAttackV"));
				}
				this.bwfangText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxDefenseV"));
				this.bmfangText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxMDefenseV"));
				this.bhitvText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "HitV"));
				this.bdodgeText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "Dodge"));
				this.bshengmingText.Text = string.Format("{0}", Global.GetXElementAttributeInt(wingStarXmlNode2, "MaxLifeV"));
			}
			this.bxishouText.Text = string.Empty;
			this.bjiachengText.Text = string.Empty;
			this.sprite1.gameObject.SetActive(false);
			this.sprite2.gameObject.SetActive(false);
		}
		else
		{
			XElement xelement = Global.GetXElement(this.Wingxml, "Level", "ID", (wingID + 1).ToString());
			if (xelement == null)
			{
				this.duibiPanel.gameObject.SetActive(false);
				return;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MaxAttackV");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxMAttackV");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MaxDefenseV");
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxMDefenseV");
			int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "HitV");
			int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "Dodge");
			int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement, "MaxLifeV");
			double xelementAttributeDouble = Global.GetXElementAttributeDouble(xelement, "SubAttackInjurePercent");
			double xelementAttributeDouble2 = Global.GetXElementAttributeDouble(xelement, "AddAttackInjurePercent");
			if (xelementAttributeInt != 0)
			{
				this.bwgongText.Text = string.Format("{0}", xelementAttributeInt - this.intTotalMaxAttackV);
			}
			if (xelementAttributeInt2 != 0)
			{
				this.bmgongText.Text = string.Format("{0}", xelementAttributeInt2 - this.intTotalMaxMAttackV);
			}
			this.bwfangText.Text = string.Format("{0}", xelementAttributeInt3 - this.intTotalMaxDefenseV);
			this.bmfangText.Text = string.Format("{0}", xelementAttributeInt4 - this.intTotalMaxMDefenseV);
			this.bhitvText.Text = string.Format("{0}", xelementAttributeInt5 - this.intTotalHitVV);
			this.bdodgeText.Text = string.Format("{0}", xelementAttributeInt6 - this.intTotalDodgeV);
			this.bshengmingText.Text = string.Format("{0}", xelementAttributeInt7 - this.intTotalMaxLifeV);
			this.bxishouText.Text = string.Format("{0}%", (xelementAttributeDouble - this.intBasSubAttackInjurePercent) * 100.0);
			this.bjiachengText.Text = string.Format("{0}%", (xelementAttributeDouble2 - this.intBasAddAttackInjurePercent) * 100.0);
			this.sprite1.gameObject.SetActive(true);
			this.sprite2.gameObject.SetActive(true);
		}
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	private void AutoTiSheng_Tick()
	{
		if (this.tsCheckBox.Check)
		{
			bool flag = false;
			int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(2016, "WingSuit", ref flag, (long)Global.Data.roleData.MyWingData.WingID);
			if (this.tsNeedGoodsNum > Global.GetTotalGoodsCountByID(2016) + replaceGoodCount)
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ChiBangShengXing", this.zuanshiXiaoHao, false);
				this.itsType = 1;
				GameInstance.Game.SpriteWingUpStarCmd(1);
			}
			else
			{
				this.StartGoodsTiSheng();
			}
		}
		else
		{
			this.StartGoodsTiSheng();
		}
	}

	public void StopAutoTiSheng()
	{
		base.CancelInvoke("AutoTiSheng_Tick");
		this.zuanshiTsBtn.Text = Global.GetLang("自动提升");
		this.zuanshiTsBtn.normalSprite = "tongyongBtn3_normal";
		this.zuanshiTsBtn.Pressed = true;
	}

	private void AutoJingJie_Tick()
	{
		if (this.jjCheckBox.Check)
		{
			bool flag = false;
			int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(2017, "WingSuit", ref flag, (long)Global.Data.roleData.MyWingData.WingID);
			if (this.jjNeedGoodsNum > Global.GetTotalGoodsCountByID(2017) + replaceGoodCount)
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ChiBangShengJie", this.zuanshiXiaoHao, false);
				this.itsType = 1;
				GameInstance.Game.SpriteWingUpgradeCmd(1);
			}
			else
			{
				this.StartGoodsJinJie();
			}
		}
		else
		{
			this.StartGoodsJinJie();
		}
	}

	public void StopAutoJingJie()
	{
		base.CancelInvoke("AutoJingJie_Tick");
		this.zuanshiJJBtn.Text = Global.GetLang("自动进阶");
		this.zuanshiJJBtn.normalSprite = "tongyongBtn3_normal";
		this.zuanshiJJBtn.Pressed = true;
	}

	internal void ShowHelpAnim()
	{
		this.mClickHelpAnim = true;
	}

	public void ShowChibangLingyuWindow()
	{
		if (null != this.ChibangLingyuWindow)
		{
			if (this.ChibangLingyuWindow.Visibility)
			{
				this.CloseChibangLingyuWindow();
			}
			else
			{
				this.ChibangLingyuWindow.Visibility = true;
			}
			return;
		}
		this.ChibangLingyuWindow = U3DUtils.NEW<GChildWindow>();
		this.ChibangLingyuWindow.Modal = true;
		this.ChibangLingyuWindow.IsShowModal = true;
		this.InitChildWindow(this.ChibangLingyuWindow, Global.GetLang("翎羽"), false);
		this.ChibangLingyuWindow.ChildWindowClose = delegate(object s, EventArgs e)
		{
			this.CloseChibangLingyuWindow();
			return true;
		};
		this.ModalPart.Children.Add(this.ChibangLingyuWindow);
		this.chibangLingyuPart = U3DUtils.NEW<ChibangLingyuPart>();
		this.chibangLingyuPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0 || e.ID == 1)
			{
				this.CloseChibangLingyuWindow();
			}
		};
		this.ChibangLingyuWindow.SetContent(this.ChibangLingyuWindow.BodyPresenter, this.chibangLingyuPart, 0.0, 0.0, true);
	}

	public void ShowZhuHunWindows()
	{
		if (null != this.ZhuHunWindow)
		{
			this.CloseChildWindow(this.ZhuHunWindow);
			this.ZhuHunWindow = null;
			this.ZhuHunPart = null;
		}
		if (null == this.ZhuHunWindow && null == this.ZhuHunPart)
		{
			this.ZhuHunWindow = U3DUtils.NEW<GChildWindow>();
			this.ZhuHunWindow.IsShowModal = true;
			this.InitChildWindow(this.ZhuHunWindow, "_TaskWindow", false);
			this.Container.Children.Add(this.ZhuHunWindow);
			this.ZhuHunWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.ZhuHunWindow);
				this.ZhuHunWindow = null;
				this.ZhuHunPart = null;
				PlayZone.GlobalPlayZone._ZhuHunPart = null;
				return true;
			};
			this.ZhuHunWindow.ModalType = ChildWindowModalType.Translucent;
			Vector3 localPosition = this.ZhuHunWindow.transform.localPosition;
			localPosition.x = -411f;
			localPosition.y = -140f;
			this.ZhuHunWindow.transform.localPosition = localPosition;
			this.ZhuHunWindow.ModalBak.transform.localPosition = new Vector3(363f, 168f, 0.1f);
			this.ZhuHunPart = U3DUtils.NEW<Zhuhun_Part>();
			PlayZone.GlobalPlayZone._ZhuHunPart = this.ZhuHunPart;
			this.ZhuHunWindow.SetContent(this.ZhuHunWindow.BodyPresenter, this.ZhuHunPart, 0.0, 0.0, true);
			this.ZhuHunPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseChildWindow(this.ZhuHunWindow);
				this.ZhuHunWindow = null;
				this.ZhuHunPart = null;
				PlayZone.GlobalPlayZone._ZhuHunPart = null;
			};
		}
	}

	public void ShowZhuLingWindows()
	{
		if (null != this.ZhuLingWindow)
		{
			this.CloseChildWindow(this.ZhuLingWindow);
			this.ZhuLingWindow = null;
			this.ZhuLingPart = null;
		}
		if (null == this.ZhuLingWindow && null == this.ZhuLingPart)
		{
			this.ZhuLingWindow = U3DUtils.NEW<GChildWindow>();
			this.ZhuLingWindow.IsShowModal = true;
			this.InitChildWindow(this.ZhuLingWindow, "_TaskWindow", false);
			this.Container.Children.Add(this.ZhuLingWindow);
			this.ZhuLingWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.ZhuLingWindow);
				this.ZhuLingWindow = null;
				this.ZhuLingPart = null;
				PlayZone.GlobalPlayZone._ZhuLingPart = null;
				return true;
			};
			this.ZhuLingWindow.ModalType = ChildWindowModalType.Translucent;
			Vector3 localPosition = this.ZhuLingWindow.transform.localPosition;
			localPosition.x = -411f;
			localPosition.y = -140f;
			this.ZhuLingWindow.transform.localPosition = localPosition;
			this.ZhuLingWindow.ModalBak.transform.localPosition = new Vector3(363f, 168f, 0.1f);
			this.ZhuLingPart = U3DUtils.NEW<ZhuLing_Part>();
			PlayZone.GlobalPlayZone._ZhuLingPart = this.ZhuLingPart;
			this.ZhuLingWindow.SetContent(this.ZhuLingWindow.BodyPresenter, this.ZhuLingPart, 0.0, 0.0, true);
			this.ZhuLingPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseChildWindow(this.ZhuLingWindow);
				this.ZhuLingWindow = null;
				this.ZhuLingPart = null;
				PlayZone.GlobalPlayZone._ZhuLingPart = null;
			};
		}
	}

	public void CloseChibangLingyuWindow()
	{
		if (null != this.ChibangLingyuWindow)
		{
			this.chibangLingyuPart.CleanUpChildWindows();
			this.ChibangLingyuWindow.Visibility = false;
			this.CloseChildWindow(this.ChibangLingyuWindow);
			this.chibangLingyuPart = null;
		}
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		Super.CloseChildWindow(this.ModalPart, childWindow);
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
	}

	private void InitChildWindow1(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow1(childWindow, title);
	}

	private const int MaxWingLevel = 12;

	public SpriteSL ModalPart;

	private GChildWindow ZhuHunWindow;

	private Zhuhun_Part ZhuHunPart;

	private GChildWindow ZhuLingWindow;

	private ZhuLing_Part ZhuLingPart;

	public TextBlock title;

	public ListBox listBox;

	public SpringPanel UIDragPl;

	public TextBlock jieLevText;

	public TextBlock dingJinText;

	public GButton peidaiBtn;

	public TextBlock shuomingText;

	public TextBlock jieshuText;

	public UIButton BtnLingyu;

	public UIButton BtnZhuling;

	public UIButton BtnZhuhun;

	private Modal3DShow ChiBangModal;

	private ObservableCollection ItemCollection;

	private XElement Wingxml;

	private int tempWingStarExp;

	public Animation Anim;

	private int tsNeedGoodsNum;

	private int jjNeedGoodsNum;

	public DPSelectedItemEventHandler callback;

	public GameObject Dingji;

	public GButton BtnTSdingji;

	public TextBlock wgongText;

	public TextBlock mgongText;

	public TextBlock wfangText;

	public TextBlock mfangText;

	public TextBlock hitvText;

	public TextBlock dodgeText;

	public TextBlock shengmingText;

	public TextBlock xishouText;

	public TextBlock jiachengText;

	public GameObject duibiPanel;

	public TextBlock bwgongText;

	public TextBlock bmgongText;

	public TextBlock bwfangText;

	public TextBlock bmfangText;

	public TextBlock bhitvText;

	public TextBlock bdodgeText;

	public TextBlock bshengmingText;

	public TextBlock bxishouText;

	public TextBlock bjiachengText;

	public UISprite sprite1;

	public UISprite sprite2;

	public GameObject objectTS;

	public GButton goodsTsBtn;

	public GButton zuanshiTsBtn;

	public GImgProgressBar tsLevProgBar;

	public GImgProgressBar tsJingduProgressBar;

	public TextBlock tsjinduText;

	public TextBlock goodsName;

	public TextBlock goodsNum;

	public TextBlock zsNeedText;

	public GCheckBox tsCheckBox;

	public UISprite Tsanimation;

	public GGoodIcon tsNeedIcon;

	public Animation statAnim;

	public GameObject checkboxAnim;

	public GameObject objectJJ;

	public GButton goodsJJBtn;

	public GButton zuanshiJJBtn;

	public GImgProgressBar jjJingduProgressBar;

	public TextBlock jjjinBiText;

	public TextBlock jjgoodsName;

	public TextBlock jjgoodsNum;

	public TextBlock jjZSName;

	public GCheckBox jjCheckBox;

	public GGoodIcon jjNeedIcon;

	private GChildWindow messageBoxWindow;

	private int itsType = -1;

	private int occupation;

	private int intTotalMinAttackV;

	private int intTotalMaxAttackV;

	private int intTotalMinMAttackV;

	private int intTotalMaxMAttackV;

	private int intTotalMinDefenseV;

	private int intTotalMaxDefenseV;

	private int intTotalMinMDefenseV;

	private int intTotalMaxMDefenseV;

	private int intTotalMaxLifeV;

	private int intTotalHitVV;

	private int intTotalDodgeV;

	private double intBasSubAttackInjurePercent;

	private double intBasAddAttackInjurePercent;

	private int zuanshiXiaoHao;

	public List<UISprite> listSpDaiBiShengXing = new List<UISprite>();

	public List<UISprite> listSpDaiBiJinJie = new List<UISprite>();

	private ChiBangItem temp;

	private WingsResLoader wingsResLoader;

	private string[] BtnSpriteNames = new string[]
	{
		"tongyongBtn3_normal",
		"tongyongBtn6_normal"
	};

	private GetGoodsHintPart hintPart;

	private bool mClickHelpAnim;

	public GChildWindow ChibangLingyuWindow;

	public ChibangLingyuPart chibangLingyuPart;
}
