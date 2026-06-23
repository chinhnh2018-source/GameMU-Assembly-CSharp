using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ChongShengLianluPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.BtnOnBody.Label.text = Global.GetLang("身上");
		this.BtnKnapsack.Label.text = Global.GetLang("背包");
		this.m_ObserEquip = this.m_ListBoxEquip.ItemsSource;
		this.m_SpringVec = this.m_SpringEquip.transform.localPosition;
		this.m_VecHelpOpen = this.m_BtnHelpOpen.transform.localPosition;
		this.InitBtns();
		this.InitOnClick();
	}

	private void SetOnlyBeiBao(bool beOnlyBeiBao)
	{
		GButton gbutton = this.m_TabEquip[1];
		GButton gbutton2 = this.m_TabEquip[0];
		if (gbutton != null && gbutton2 != null)
		{
			if (beOnlyBeiBao)
			{
				gbutton.gameObject.SetActive(false);
				gbutton2.gameObject.SetActive(true);
				Vector3 localPosition = gbutton2.gameObject.transform.localPosition;
				localPosition.x = 0f;
				gbutton2.gameObject.transform.localPosition = localPosition;
			}
			else
			{
				gbutton.gameObject.SetActive(true);
				gbutton2.gameObject.SetActive(true);
				Vector3 localPosition2 = gbutton2.gameObject.transform.localPosition;
				localPosition2.x = 0f;
				gbutton.gameObject.transform.localPosition = localPosition2;
				localPosition2.x = 115f;
				gbutton2.gameObject.transform.localPosition = localPosition2;
			}
		}
	}

	private void InitOnClick()
	{
		this.m_TabEquip.TabClick += delegate(GameObject s, int e)
		{
			this.SetTab(e);
		};
		this.m_TabBtns.TabClick += delegate(GameObject s, int e)
		{
			this.SetLeftBtn(e);
		};
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		};
		this.m_BtnHelpOpen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow();
		};
		this.m_ListBoxEquip.SelectionChanged = new MouseLeftButtonUpEventHandler(this.EquipOnClick);
	}

	private void SetTab(int index)
	{
		this.m_IndexEquip = index;
		this.LoadEquip(index);
	}

	private void LoadEquip(int index)
	{
		List<GoodsData> list = new List<GoodsData>();
		if (Global.Data.roleData.RebornGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.RebornGoodsDataList[i];
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				if (goodsXmlNodeByID == null)
				{
					MUDebug.Log<string>(new string[]
					{
						Global.GetLang("道具") + goodsData.GoodsID + Global.GetLang("缺少Goods表配置")
					});
				}
				else if (goodsXmlNodeByID.RebornEquip != 0)
				{
					if (Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
					{
						if (index == 0)
						{
							if (goodsData.Using == 1)
							{
								goto IL_178;
							}
						}
						else
						{
							if (index != 1)
							{
								goto IL_178;
							}
							if (goodsData.Using == 0)
							{
								goto IL_178;
							}
						}
						if (this.m_TabBtns.TabIndex == 1)
						{
							if (this.GetMaxDaKong(goodsData))
							{
								goto IL_178;
							}
						}
						else if (this.m_TabBtns.TabIndex != 2)
						{
							if (this.m_TabBtns.TabIndex == 3)
							{
								if (!this.GetOnDaKong(goodsData))
								{
									goto IL_178;
								}
							}
							else if (this.m_TabBtns.TabIndex == 4 && IConfigbase<ConfigReborn>.Instance.GetRebornEquipEvolutionByGoodsID(goodsData.GoodsID) == null)
							{
								goto IL_178;
							}
						}
						list.Add(goodsData);
					}
				}
				IL_178:;
			}
		}
		this.m_ObserEquip.Clear();
		for (int j = 0; j < list.Count; j++)
		{
			ChongShengLianluEquipItem chongShengLianluEquipItem = U3DUtils.NEW<ChongShengLianluEquipItem>();
			chongShengLianluEquipItem.SetData(list[j]);
			this.m_ObserEquip.AddNoUpdate(chongShengLianluEquipItem);
			chongShengLianluEquipItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.RefreshEquip(e.ZhuZhuangBei);
			};
			if (chongShengLianluEquipItem.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(chongShengLianluEquipItem.GetComponent<UIPanel>());
			}
		}
		this.m_SpringEquip.target = this.m_SpringVec;
		this.m_SpringEquip.enabled = true;
	}

	private void InitBtns()
	{
		List<ChongShenLianLuTypesData> list = new List<ChongShenLianLuTypesData>();
		list.Add(new ChongShenLianLuTypesData(ChongShenLianLuTypes.ZhuangBeiDaKong, Global.GetLang("装备打孔")));
		list.Add(new ChongShenLianLuTypesData(ChongShenLianLuTypes.BaoShiJiaGong, Global.GetLang("宝石加工")));
		list.Add(new ChongShenLianLuTypesData(ChongShenLianLuTypes.BaoShiXiangQian, Global.GetLang("宝石镶嵌")));
		list.Add(new ChongShenLianLuTypesData(ChongShenLianLuTypes.JinJie, Global.GetLang("装备进阶")));
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100116) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.CuiLian, ref num, ref num2, ref num3))
		{
			list.Add(new ChongShenLianLuTypesData(ChongShenLianLuTypes.CuiLian, Global.GetLang("装备灌注")));
		}
		for (int i = 0; i < list.Count; i++)
		{
			GButton component = U3DUtils.Clone(this.m_BtnLeft.transform.parent.gameObject, this.m_BtnLeft.gameObject).GetComponent<GButton>();
			Vector3 localPosition = component.transform.localPosition;
			localPosition.y -= (float)(i * 45);
			component.transform.localPosition = localPosition;
			component.Text = Global.GetLang(list[i].Name);
			if (component != null)
			{
				component.gameObject.SetActive(true);
				component.TagIndex = (int)list[i].typs;
			}
		}
	}

	private void RefreshEquip(GoodsData gd)
	{
		if (gd == null)
		{
			for (int i = 0; i < this.m_ObserEquip.Count; i++)
			{
				ChongShengLianluEquipItem component = this.m_ObserEquip.GetAt(i).GetComponent<ChongShengLianluEquipItem>();
				if (component != null)
				{
					component.OnClickItem = false;
				}
			}
			return;
		}
		for (int j = 0; j < this.m_ObserEquip.Count; j++)
		{
			ChongShengLianluEquipItem component2 = this.m_ObserEquip.GetAt(j).GetComponent<ChongShengLianluEquipItem>();
			if (component2 != null)
			{
				if (component2.GoodsData.Id == gd.Id)
				{
					component2.OnClickItem = true;
				}
				else
				{
					component2.OnClickItem = false;
				}
			}
		}
		if (this.m_TabBtns.TabIndex == 1)
		{
			this.m_ChongShengLianLuDaKongPart.AddEqip(gd);
		}
		else if (this.m_TabBtns.TabIndex == 3)
		{
			this.m_ChongShengLianLuXiangQianPart.AddEqip(gd);
		}
		else if (this.m_TabBtns.TabIndex == 4)
		{
			this.lianluChongShengJinJiePart.AddEquip(gd);
		}
	}

	private void EquipOnClick(object sender, MouseEvent e)
	{
		if (this.m_ListBoxEquip.SelectedItem == null)
		{
			return;
		}
		ChongShengLianluEquipItem component = this.m_ListBoxEquip.SelectedItem.GetComponent<ChongShengLianluEquipItem>();
		if (component == null)
		{
			return;
		}
		this.RefreshEquip(component.GoodsData);
	}

	public void StartPage(ChongShenLianLuTypes type)
	{
		this.m_TabBtns.TabIndex = (int)type;
	}

	public void SetLeftBtn(int index)
	{
		if (this.m_IndexBtn == index)
		{
			return;
		}
		this.m_IndexBtn = index;
		if (this.m_GamePanel != null)
		{
			Object.Destroy(this.m_GamePanel);
			this.m_GamePanel = null;
		}
		this.m_TabEquip.gameObject.SetActive(true);
		this.m_SpShuXian.gameObject.SetActive(true);
		this.m_SpHenXian.gameObject.SetActive(true);
		this.m_ImgDiCenter.gameObject.SetActive(true);
		this.m_ListBoxEquip.gameObject.SetActive(true);
		this.m_BtnHelpOpen.gameObject.SetActive(true);
		this.m_BtnHelpOpen.transform.localPosition = this.m_VecHelpOpen;
		if (index == 1)
		{
			this.SetOnlyBeiBao(false);
			this.m_ChongShengLianLuDaKongPart = U3DUtils.NEW<ChongShengLianLuDaKongPart>();
			this.m_ChongShengLianLuDaKongPart.transform.SetParent(this.m_GamePanelParent.transform, false);
			this.m_GamePanel = this.m_ChongShengLianLuDaKongPart.gameObject;
			this.m_ChongShengLianLuDaKongPart.m_BtnShanChu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.m_ChongShengLianLuDaKongPart.AddEqip(null);
				this.RefreshEquip(null);
			};
			this.m_ChongShengLianLuDaKongPart.dpsRefreshEquip = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.RefreshEquipList(e.ZhuZhuangBei);
			};
		}
		else if (index == 2)
		{
			this.SetOnlyBeiBao(false);
			this.m_ChongShengLianLuJiaGongPart = U3DUtils.NEW<ChongShengLianLuJiaGongPart>();
			this.m_ChongShengLianLuJiaGongPart.transform.SetParent(this.m_GamePanelParent.transform, false);
			this.m_GamePanel = this.m_ChongShengLianLuJiaGongPart.gameObject;
			this.m_TabEquip.gameObject.SetActive(false);
			this.m_ListBoxEquip.gameObject.SetActive(false);
			this.m_SpShuXian.gameObject.SetActive(false);
			this.m_ImgDiCenter.gameObject.SetActive(false);
			this.m_BtnHelpOpen.transform.localPosition = new Vector3(150f, 146f, -1f);
			this.m_ChongShengLianLuJiaGongPart.m_DPSelectedItemEventHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.m_BtnHelpOpen.gameObject.SetActive(true);
				}
				else if (e.IDType == 1)
				{
					this.m_BtnHelpOpen.gameObject.SetActive(false);
				}
			};
			this.m_ChongShengLianLuJiaGongPart.m_TabBtnType.TabIndex = 0;
		}
		else if (index == 3)
		{
			this.SetOnlyBeiBao(false);
			this.m_ChongShengLianLuXiangQianPart = U3DUtils.NEW<ChongShengLianLuXiangQianPart>();
			this.m_ChongShengLianLuXiangQianPart.transform.SetParent(this.m_GamePanelParent.transform, false);
			this.m_ChongShengLianLuXiangQianPart.dpsRefreshEquip = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.RefreshEquipList(e.ZhuZhuangBei);
			};
			this.m_GamePanel = this.m_ChongShengLianLuXiangQianPart.gameObject;
		}
		else
		{
			if (index == 4)
			{
				this.SetOnlyBeiBao(true);
				this.lianluChongShengJinJiePart = U3DUtils.NEW<LianluChongShengJinJiePart>();
				this.lianluChongShengJinJiePart.InitPartSize(0, 0);
				this.lianluChongShengJinJiePart.InitPartData();
				this.lianluChongShengJinJiePart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
				{
					if (e.EffectID == 1)
					{
						this.LoadEquip(0);
					}
					this.SetEffect(e.EffectID == 1);
				};
				this.lianluChongShengJinJiePart.InitAllValue(true);
				this.lianluChongShengJinJiePart.transform.SetParent(this.m_GamePanelParent.transform, false);
				this.m_GamePanel = this.lianluChongShengJinJiePart.gameObject;
				this.m_TabEquip.TabIndex = 0;
				return;
			}
			if (index == 5)
			{
				this.SetOnlyBeiBao(false);
				this.m_SpShuXian.gameObject.SetActive(false);
				this.m_SpHenXian.gameObject.SetActive(false);
				this.m_ChongShengEquipCuiLianPart = U3DUtils.NEW<ChongShengEquipCuiLianPart>();
				this.m_ChongShengEquipCuiLianPart.transform.SetParent(this.m_GamePanelParent.transform, false);
				this.m_GamePanel = this.m_ChongShengEquipCuiLianPart.gameObject;
				this.m_TabEquip.gameObject.SetActive(false);
				this.m_ListBoxEquip.gameObject.SetActive(false);
				this.m_BtnHelpOpen.transform.localPosition = new Vector3(-140f, 210f, -1f);
			}
		}
		this.m_TabEquip.TabIndex = 1;
	}

	private void OpenHelpWindow()
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		string text = string.Empty;
		if (this.m_TabBtns.TabIndex == 1)
		{
			text = "Config/ZhuangBeiDaKongJiIntro.xml";
		}
		else if (this.m_TabBtns.TabIndex == 2)
		{
			text = "Config/BaoShiJiaGongJiIntro.xml";
		}
		else if (this.m_TabBtns.TabIndex == 3)
		{
			text = "Config/BaoShiXiangQianJiIntro.xml";
		}
		else if (this.m_TabBtns.TabIndex == 4)
		{
			text = "Config/RebornEquipEvolutionIntro.xml";
		}
		else
		{
			if (this.m_TabBtns.TabIndex != 5)
			{
				return;
			}
			this.m_helpPart.mChildTransform.localPosition = new Vector3(160f, 0f, 0f);
			text = "Config/EquipQuenchingIntro.xml";
		}
		ChangeableRulePart.RuleXml ruleXml = null;
		if (ruleXml == null)
		{
			XElement gameResXml = Global.GetGameResXml(text);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format("加载{0}出现错误", text)
				});
			}
			ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		}
		this.m_helpPart.SetHelpInfo(ruleXml.list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	public void RefreshEquipList(GoodsData gd = null)
	{
		if (gd == null)
		{
			this.SetTab(this.m_TabEquip.TabIndex);
		}
		else
		{
			for (int i = 0; i < this.m_ObserEquip.Count; i++)
			{
				ChongShengLianluEquipItem component = this.m_ObserEquip.GetAt(i).GetComponent<ChongShengLianluEquipItem>();
				if (!(component == null))
				{
					if (component.GoodsData.Id == gd.Id)
					{
						if (this.m_TabBtns.TabIndex == 1 && this.GetMaxDaKong(gd))
						{
							this.SetTab(this.m_TabEquip.TabIndex);
							break;
						}
						component.SetData(gd);
					}
				}
			}
		}
	}

	private bool GetMaxDaKong(GoodsData gd)
	{
		bool flag = true;
		if (gd == null)
		{
			return false;
		}
		if (string.IsNullOrEmpty(gd.Props))
		{
			return false;
		}
		string[] array = gd.Props.Split(new char[]
		{
			'|'
		});
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		int daKongPingZhi = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongPingZhi(Global.GetZhuoyueAttributeCount(gd));
		ZhuangBeiDaKongVO daKongDataByJieShu = IConfigbase<ConfigChongShengZhuangBei>.Instance.GetDaKongDataByJieShu(goodsXmlNodeByID.SuitID, daKongPingZhi);
		int num = 0;
		string[] array2 = daKongDataByJieShu.GaiLv.Split(new char[]
		{
			'|'
		});
		int[] array3 = new int[array2.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			if (!string.IsNullOrEmpty(array2[i]) && float.Parse(array2[i].Split(new char[]
			{
				','
			})[1]) > 0f)
			{
				array3[i] = array2[i].Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
			}
		}
		int num2 = Mathf.Max(array3);
		for (int j = 0; j < array.Length; j++)
		{
			if (!string.IsNullOrEmpty(array[j]))
			{
				int num3 = array[j].Split(new char[]
				{
					'_'
				})[0].SafeToInt32(0);
				if (num3 != -1)
				{
					num++;
					int num4 = array[j].Split(new char[]
					{
						'_'
					})[1].SafeToInt32(0);
					if (num4 < num2)
					{
						return false;
					}
				}
			}
		}
		return num >= daKongDataByJieShu.DaKongShuLiang && flag;
	}

	private bool GetOnDaKong(GoodsData gd)
	{
		if (gd == null)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID.Categoriy == 37)
		{
			return true;
		}
		if (string.IsNullOrEmpty(gd.Props))
		{
			return false;
		}
		string[] array = gd.Props.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				int num = array[i].Split(new char[]
				{
					'_'
				})[0].SafeToInt32(0);
				if (num != -1)
				{
					int num2 = array[i].Split(new char[]
					{
						'_'
					})[1].SafeToInt32(0);
					if (num2 > 0)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void SetEffect(bool retBool)
	{
		if (retBool)
		{
			this.m_AnimationChengGong.gameObject.SetActive(true);
			this.PlayStart(this.m_AnimationChengGong, new ActiveAnimation.OnFinished(this.PlayFinished));
		}
		else
		{
			this.m_AnimationShiBai.gameObject.SetActive(true);
			this.PlayStart(this.m_AnimationShiBai, new ActiveAnimation.OnFinished(this.PlayFinished));
		}
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		if (anim.isPlaying)
		{
			anim.Stop();
		}
		if (this.m_TabBtns.TabIndex == 5)
		{
			anim.transform.localPosition = new Vector3(160f, 0f, -3f);
		}
		else
		{
			anim.transform.localPosition = Vector3.back;
		}
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

	public void Refresh(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.CmdID == 2050)
		{
			int num = e.fields[0].SafeToInt32(0);
			if (num == 1)
			{
				this.SetEffect(true);
				string props = e.fields[1];
				int binDing = e.fields[2].SafeToInt32(0);
				if (this.m_ChongShengLianLuDaKongPart != null)
				{
					this.m_ChongShengLianLuDaKongPart.Refresh(props, binDing);
				}
			}
			else
			{
				Super.HintMainText(RebornStornOpcodeError.ErrorChongShengLianLu(num), 10, 3);
			}
		}
		else if (e.CmdID == 2054 || e.CmdID == 2055)
		{
			int num2 = e.fields[0].SafeToInt32(0);
			if (num2 == 20)
			{
				this.SetEffect(true);
				string props2 = e.fields[1];
				int binDing2 = -1;
				if (e.fields.Length > 2)
				{
					binDing2 = e.fields[2].SafeToInt32(0);
				}
				if (this.m_ChongShengLianLuXiangQianPart != null)
				{
					this.m_ChongShengLianLuXiangQianPart.Refresh(props2, binDing2);
				}
			}
			else
			{
				Super.HintMainText(RebornStornOpcodeError.ErrorChongShengLianLu(num2), 10, 3);
			}
		}
		else if (e.CmdID == 2056)
		{
			int num3 = e.fields[0].SafeToInt32(0);
			if (num3 == 54)
			{
				this.SetEffect(true);
				if (this.m_ChongShengLianLuJiaGongPart != null)
				{
					this.m_ChongShengLianLuJiaGongPart.RefreshChongSheng();
				}
			}
			else
			{
				Super.HintMainText(RebornStornOpcodeError.ErrorChongShengLianLu(num3), 10, 3);
			}
		}
		else if (e.CmdID == 2058)
		{
			int num4 = e.fields[0].SafeToInt32(0);
			if (num4 == 55)
			{
				this.SetEffect(true);
				if (this.m_ChongShengLianLuJiaGongPart != null)
				{
					this.m_ChongShengLianLuJiaGongPart.RefreshChongSheng();
				}
			}
			else
			{
				Super.HintMainText(RebornStornOpcodeError.ErrorChongShengLianLu(num4), 10, 3);
			}
		}
		else if (e.CmdID == 2057)
		{
			int num5 = e.fields[0].SafeToInt32(0);
			if (num5 == 70)
			{
				this.SetEffect(true);
				if (this.m_ChongShengLianLuJiaGongPart != null)
				{
					this.m_ChongShengLianLuJiaGongPart.RefreshXuanCai();
				}
			}
			else
			{
				Super.HintMainText(RebornStornOpcodeError.ErrorChongShengLianLu(num5), 10, 3);
			}
		}
		else if (e.CmdID == 2095)
		{
			int num6 = e.fields[0].SafeToInt32(0);
			if (num6 == 1)
			{
				int able = e.fields[1].SafeToInt32(0);
				this.SetEffect(true);
				if (this.m_ChongShengEquipCuiLianPart != null)
				{
					this.m_ChongShengEquipCuiLianPart.RefreshGuanZhu(able);
				}
			}
			else
			{
				Super.HintMainText(IConfigbase<ConfigChongShengEquipCuiLian>.Instance.ErrorCuiLianString((RebornPerfusionOpcode)num6), 10, 3);
			}
		}
		else if (e.CmdID == 2096)
		{
			int num7 = e.fields[0].SafeToInt32(0);
			if (num7 == 1)
			{
				int level = e.fields[1].SafeToInt32(0);
				int able2 = e.fields[2].SafeToInt32(0);
				this.SetEffect(true);
				if (this.m_ChongShengEquipCuiLianPart != null)
				{
					this.m_ChongShengEquipCuiLianPart.RefreshCuiLian(able2, level);
				}
			}
			else if (num7 == 10)
			{
				int level2 = e.fields[1].SafeToInt32(0);
				int able3 = e.fields[2].SafeToInt32(0);
				this.SetEffect(false);
				if (this.m_ChongShengEquipCuiLianPart != null)
				{
					this.m_ChongShengEquipCuiLianPart.RefreshCuiLian(able3, level2);
				}
			}
			else
			{
				Super.HintMainText(IConfigbase<ConfigChongShengEquipCuiLian>.Instance.ErrorCuiLianString((RebornPerfusionOpcode)num7), 10, 3);
			}
		}
	}

	[SerializeField]
	private GButton m_BtnClose;

	[SerializeField]
	private UITab m_TabEquip;

	[SerializeField]
	private ListBox m_ListBoxEquip;

	[SerializeField]
	private SpringPanel m_SpringEquip;

	[SerializeField]
	private UITab m_TabBtns;

	[SerializeField]
	private GButton m_BtnLeft;

	[SerializeField]
	private GameObject m_GamePanelParent;

	[SerializeField]
	private GButton ShenshangZhuangbei;

	[SerializeField]
	private GButton BaoguoZhuangbei;

	[SerializeField]
	private GButton m_BtnHelpOpen;

	[SerializeField]
	private UISprite m_SpShuXian;

	[SerializeField]
	private UISprite m_SpHenXian;

	[SerializeField]
	private ShowNetImage m_ImgDiCenter;

	[SerializeField]
	private Animation m_AnimationChengGong;

	[SerializeField]
	private Animation m_AnimationShiBai;

	private int m_IndexEquip;

	private Vector3 m_VecHelpOpen;

	private GameObject m_GamePanel;

	private ObservableCollection m_ObserEquip;

	private ChongShengLianLuDaKongPart m_ChongShengLianLuDaKongPart;

	private ChongShengLianLuJiaGongPart m_ChongShengLianLuJiaGongPart;

	private ChongShengLianLuXiangQianPart m_ChongShengLianLuXiangQianPart;

	public LianluChongShengJinJiePart lianluChongShengJinJiePart;

	public ChongShengEquipCuiLianPart m_ChongShengEquipCuiLianPart;

	private Vector3 m_SpringVec;

	private int m_IndexBtn = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	[SerializeField]
	private GButton BtnOnBody;

	[SerializeField]
	private GButton BtnKnapsack;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;
}
