using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LianluPart : UserControl
{
	public LianluPart()
	{
		List<LianluTypes> list = new List<LianluTypes>();
		list.Add(LianluTypes.QianghuaChuancheng);
		list.Add(LianluTypes.ZhuijiaChuancheng);
		list.Add(LianluTypes.XilianChuancheng);
		list.Add(LianluTypes.JuHunChuancheng);
		list.Add(LianluTypes.FuMoChuanCheng);
		this.ChuanChengIndexList = list;
		this.ShengShangZhuangbeiPos = default(Vector3);
		this.BaoguoZhuangbeiPos = default(Vector3);
		this.m_ListBtns = new List<GButton>();
		this.ItemsList = new List<LianluEquipItem>();
		this.preTabIndex = -1;
		this.dicLianLuOpen = new Dictionary<int, bool>();
		this.categoriyIndexArr = new int[]
		{
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			8,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			23,
			24,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38
		};
		this.categoriyIndexDict = new Dictionary<int, int>();
		this.OpenPartCount = true;
		this.lianluAtlasName = new string[]
		{
			"lianluQianghuaOK",
			"lianluZhuijia",
			"lianluChuanchengOK",
			"lianluZhuanshengOK",
			"lianluZhuanshengBoliOK"
		};
		base..ctor();
	}

	public ObservableCollection ItemCollection2
	{
		get
		{
			return this._ItemCollection2;
		}
		set
		{
			this._ItemCollection2 = value;
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

	private void InitTextInPrefabs()
	{
		this.ZhuangbeiQianghua.Text = Global.GetLang("装备强化");
		this.ZhuangbeiZhuijia.Text = Global.GetLang("装备追加");
		this.QianghuaChuanchen.Text = Global.GetLang("装备传承");
		this.ZhuangbeiJinjie.Text = Global.GetLang("装备进阶");
		this.ShenqiZaizao.Text = Global.GetLang("神装再造");
		this.m_JuHunBtn.Text = Global.GetLang("神器聚魂");
		this.m_ChongWuChuanCheng.Text = Global.GetLang("精灵技能传承");
		this.ZhuangBeiPeiYang.Text = Global.GetLang("装备培养");
		this.m_ZhuangBeiFuMo.Text = Global.GetLang("装备附魔");
		this.ShenshangZhuangbei.Text = Global.GetLang("身上");
		this.BaoguoZhuangbei.Text = Global.GetLang("背包");
		this.m_CheckRole.Text = Global.GetLang("人物装备");
		this.m_CheckHorse.Text = Global.GetLang("坐骑装备");
		this.m_CheckRole.Check = true;
		this.m_CheckHorse.Check = false;
		this.ShuXing.Text = Global.GetLang("属性");
		this.m_CheckRole._Lable.lineWidth = 110;
		this.m_CheckHorse._Lable.lineWidth = 110;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.initCategoriyIndexDict();
		this.equipPanelPos = this.equipPanelTran.localPosition;
		this.equipPanelClipRange = this.equipPanel.clipRange;
		UIEventListener.Get(this.HelpBtn.gameObject).onClick = delegate(GameObject s)
		{
			if (null == this.lianluHelpPart)
			{
				this.lianluHelpPart = U3DUtils.NEW<LianluHelpPart>();
				this.lianluHelpPart.DPSelectedItem = delegate(object o, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 0)
					{
						this.lianluHelpPart.Visibility = false;
					}
				};
				this.SetLianluHelp(this.MenuIndex);
				this.LianluHelpWin.Add(this.lianluHelpPart);
				return;
			}
			this.SetLianluHelp(this.MenuIndex);
			this.lianluHelpPart.Visibility = true;
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StopAutoXiLian();
			if (this.MenuIndex == LianluTypes.ZhuangbeiXilian && !this.IsEnableContinueByLianlu(LianluZhuangbeiXilianPart.IsSave))
			{
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		this.MenuTabList.TabClick += delegate(GameObject s, int e)
		{
			if (this.MenuIndex == (LianluTypes)e)
			{
				return;
			}
			if (this.ChuanChengIndexList.Contains((LianluTypes)e) && this.lianluQianghuaChuanchenPart != null)
			{
				return;
			}
			this.zhuZhuangbeiData = null;
			this.fuZhuangbeiData = null;
			this.jinjieExceptIds = null;
			if (this.m_StarBool)
			{
				this.m_CheckRole.Check = true;
				this.m_CheckHorse.Check = false;
			}
			this.m_StarBool = true;
			this.MenuIndex = (LianluTypes)e;
			base.StartCoroutine<bool>(this.SelectIcon(this.MenuIndex));
			if (null != this.lianluHelpPart)
			{
				this.lianluHelpPart.Visibility = false;
			}
		};
		this.MenuTabList.PreChangeTabCallback = delegate(object s, DPSelectedItemEventArgs e)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (e.ID == 7 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuangBeiPeiYang, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZhuangBeiPeiYang, trigger, param, param2, true);
				return false;
			}
			if (e.ID == 1 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuiJia, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZhuiJia, trigger, param, param2, true);
				return false;
			}
			if (e.ID == 9 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZaiZao, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZaiZao, trigger, param, param2, true);
				return false;
			}
			if (e.ID == 11 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShenQiJuHun, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ShenQiJuHun, trigger, param, param2, true);
				return false;
			}
			if (this.MenuIndex == LianluTypes.ZhuangbeiXilian && !this.IsEnableContinueByLianlu(LianluZhuangbeiXilianPart.IsSave))
			{
				return false;
			}
			if (e.ID == 12 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.JingLingSystem, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.JingLingSystem, trigger, param, param2, true);
				return false;
			}
			return true;
		};
		this.EquipTabList.TabClick += delegate(GameObject s, int e)
		{
			this.SelectTab(e);
		};
		this.m_CheckRole.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.m_CheckRole.Check = true;
			this.m_CheckHorse.Check = false;
			this.RefreshEquipAndCailiaoList(1, this.zhuZhuangbeiData, this.fuZhuangbeiData, this.EquipTabList.TabIndex);
		};
		this.m_CheckHorse.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.m_CheckRole.Check = false;
			this.m_CheckHorse.Check = true;
			this.RefreshEquipAndCailiaoList(4, this.zhuZhuangbeiData, this.fuZhuangbeiData, this.EquipTabList.TabIndex);
		};
		if (null != this.ShenshangZhuangbei)
		{
			this.ShengShangZhuangbeiPos = this.ShenshangZhuangbei.gameObject.transform.localPosition;
		}
		if (null != this.BaoguoZhuangbei)
		{
			this.BaoguoZhuangbeiPos = this.BaoguoZhuangbei.gameObject.transform.localPosition;
		}
		this.InitActivityIcon();
		this.ZhuangbeiZhuijia.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.ZhuiJia, "chatTab_normal", null, null);
		this.ShenqiZaizao.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.ZaiZao, "chatTab_normal", null, null);
		this.m_JuHunBtn.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.ShenQiJuHun, "chatTab_normal", null, null);
		this.m_ChongWuChuanCheng.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.JingLingJiNengChuanCheng, "chatTab_normal", null, null);
		this.ZhuangBeiPeiYang.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.ZhuangBeiPeiYang, "chatTab_normal", null, null);
		this.m_ListBtns.Add(this.ZhuangbeiQianghua);
		this.m_ListBtns.Add(this.ZhuangbeiZhuijia);
		this.m_ListBtns.Add(this.ZhuangbeiJinjie);
		this.m_ListBtns.Add(this.ZhuangBeiPeiYang);
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100104) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FuMo, ref num, ref num2, ref num3))
		{
			this.m_ListBtns.Add(this.m_ZhuangBeiFuMo);
		}
		else
		{
			this.m_ZhuangBeiFuMo.gameObject.SetActive(false);
		}
		this.m_ListBtns.Add(this.QianghuaChuanchen);
		this.m_ListBtns.Add(this.ShenqiZaizao);
		this.m_ListBtns.Add(this.m_JuHunBtn);
		this.m_ListBtns.Add(this.m_ChongWuChuanCheng);
		Vector3 localPosition = this.ZhuangbeiQianghua.transform.localPosition;
		for (int i = 0; i < this.m_ListBtns.Count; i++)
		{
			if (this.m_ListBtns[i].gameObject.activeSelf)
			{
				this.m_ListBtns[i].transform.localPosition = localPosition;
				localPosition.y -= 60f;
				if (this.m_ListBtns[i].gameObject.GetComponent<UIDragPanelContents>() == null)
				{
					this.m_ListBtns[i].gameObject.AddComponent<UIDragPanelContents>();
				}
				this.m_ListBtns[i].gameObject.GetComponent<UIDragPanelContents>().draggablePanel = this.m_Draggable;
			}
		}
	}

	private void InitActivityIcon()
	{
		ActivityTipManager.RegActivityTipItem(31001, delegate(int s, ActivityTipItem e)
		{
			NGUITools.SetActive(this.tipQiangHuaIcon.gameObject, e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(31002, delegate(int s, ActivityTipItem e)
		{
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuiJia))
			{
				NGUITools.SetActive(this.tipZhuiJiaIcon.gameObject, e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(31003, delegate(int s, ActivityTipItem e)
		{
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuangBeiPeiYang))
			{
				NGUITools.SetActive(this.tipPeiYangIcon.gameObject, e.IsActive);
			}
		});
	}

	private new void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(31001, null);
		ActivityTipManager.UnRegActivityTipItem(31002, null);
		ActivityTipManager.UnRegActivityTipItem(31003, null);
	}

	public void InitPartGoodType(int type)
	{
		this.m_CheckRole.Check = true;
		this.m_CheckHorse.Check = false;
		if (type >= 40 && type <= 45)
		{
			this.m_CheckRole.Check = false;
			this.m_CheckHorse.Check = true;
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.ZhuangbeiZhuijia, default(Vector4));
			}
			else if (id == 1)
			{
				GameObject at = this.equipList.ItemsSource.GetAt(0);
				if (null != at)
				{
					SystemHelpPart.SetMask(at.transform, default(Vector4));
				}
				else
				{
					SystemHelpPart.HideMask();
				}
			}
			else if (id == 2)
			{
				Bounds mask;
				mask..ctor(new Vector3(-110f, -228f, 0f), new Vector3(118f, 48f, 0f));
				SystemHelpPart.SetMask(mask);
			}
			else if (id == 3)
			{
				SystemHelpPart.SetMask(this.CloseBtn, default(Vector4));
			}
			else if (id == 15)
			{
				SystemHelpPart.SetMask(this.QianghuaChuanchen, default(Vector4));
			}
			else if (id == 16)
			{
				if (null != this.lianluQianghuaChuanchenPart)
				{
					SystemHelpPart.SetMask(this.lianluQianghuaChuanchenPart.SubmitBtn, default(Vector4));
				}
			}
			else if (id == 17)
			{
				if (null != this.lianluQianghuaChuanchenPart)
				{
					SystemHelpPart.SetMask(this.lianluQianghuaChuanchenPart.RadioTongqian, default(Vector4));
				}
			}
			else if (id == 18)
			{
				SystemHelpPart.SetMask(this.ShenshangZhuangbei, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		SystemHelpMgr.OnAction(UIObjIDs.LianLuPart, HelpStateEvents.Inactived, -1);
	}

	public void InitPartSize(int width, int height)
	{
		this.ItemCollection = this.equipList.ItemsSource;
		this.equipList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.EquipIconMouseLeftButtonUp);
	}

	private void ClickHelpAnim()
	{
		if (SystemHelpMgr.ActiveHelpID > 0)
		{
			int activeHelpID = SystemHelpMgr.ActiveHelpID;
			switch (activeHelpID)
			{
			case 15:
			case 16:
				break;
			case 17:
				goto IL_4D;
			default:
				switch (activeHelpID)
				{
				case 30:
					this.ShowHelpAnim(0, 1);
					return;
				case 31:
					break;
				case 32:
					goto IL_4D;
				default:
					return;
				}
				break;
			}
			this.ShowHelpAnim(1, 1);
			return;
			IL_4D:
			this.ShowHelpAnim(2, 1);
		}
	}

	public void InitPartData(int pageID = 0)
	{
		this.SetBtnState(pageID, 0);
		this.ClickHelpAnim();
	}

	private void initPanelPos()
	{
		this.equipDraggablePanel.DisableSpring();
		this.equipPanelTran.localPosition = this.equipPanelPos;
		this.equipPanel.clipRange = this.equipPanelClipRange;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (this.HorseEquipMeltingOpen(this.MenuIndex) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese, ref num, ref num2, ref num3))
		{
			this.m_PanelHorse.gameObject.SetActive(true);
			this.equipPanel.transform.localPosition = new Vector3(292f, -42f, 0f);
			this.equipPanel.clipRange = new Vector4(0f, 14f, 320f, 360f);
		}
		else
		{
			this.m_PanelHorse.gameObject.SetActive(false);
		}
	}

	private void setBtnVisibleState(LianluTypes tabIndex)
	{
		this.DestroyShuXingPanel();
		if (tabIndex == LianluTypes.ZhuangbeiZhuansheng || tabIndex == LianluTypes.ZhuanshengBoli || tabIndex == LianluTypes.ZhuangbeiJinjie || tabIndex == LianluTypes.ShenqiZaizao)
		{
			NGUITools.SetActive(this.ShenshangZhuangbei, false);
			NGUITools.SetActive(this.BaoguoZhuangbei, true);
			NGUITools.SetActive(this.ShuXing, false);
			this.BaoguoZhuangbei.transform.localPosition = this.ShengShangZhuangbeiPos;
		}
		else if (tabIndex == LianluTypes.JuHun)
		{
			NGUITools.SetActive(this.BaoguoZhuangbei, false);
			NGUITools.SetActive(this.ShenshangZhuangbei, true);
			NGUITools.SetActive(this.ShuXing, false);
			this.ShenshangZhuangbei.transform.localPosition = this.ShengShangZhuangbeiPos;
		}
		else
		{
			NGUITools.SetActive(this.BaoguoZhuangbei, true);
			NGUITools.SetActive(this.ShenshangZhuangbei, true);
			NGUITools.SetActive(this.ShuXing, false);
			this.BaoguoZhuangbei.transform.localPosition = this.BaoguoZhuangbeiPos;
			this.ShenshangZhuangbei.transform.localPosition = this.ShengShangZhuangbeiPos;
		}
	}

	private void SetBtnState(int tabIndex, int type)
	{
		if (type == 0)
		{
			this.MenuTabList.TabIndex = tabIndex;
			int num = 0;
			for (int i = 0; i < this.m_ListBtns.Count; i++)
			{
				if (this.m_ListBtns[i].gameObject.activeSelf)
				{
					if (this.m_ListBtns[i].TagIndex == tabIndex)
					{
						if (num - 7 > 0)
						{
							this.m_SpringPanel.target.y = (float)((num - 7) * 60);
							this.m_SpringPanel.enabled = true;
						}
						break;
					}
					num++;
				}
			}
		}
		else if (type == 1)
		{
			this.EquipTabList.TabIndex = tabIndex;
		}
	}

	private IEnumerator SelectIcon(LianluTypes tabIndex)
	{
		if (tabIndex == LianluTypes.Qianghua)
		{
			yield return null;
		}
		this.SetLianluPart(tabIndex);
		if (tabIndex == LianluTypes.QianghuaChuancheng || tabIndex == LianluTypes.ZhuangbeiZhuansheng || tabIndex == LianluTypes.ZhuanshengBoli || tabIndex == LianluTypes.ZhuangbeiJinjie || tabIndex == LianluTypes.ShenqiZaizao)
		{
			this.SetBtnState(0, 1);
		}
		else
		{
			this.SetBtnState(1, 1);
		}
		this.setBtnVisibleState(tabIndex);
		this.SetHorseAndRole(tabIndex);
		SystemHelpMgr.OnAction(UIObjIDs.LianLuTabBtn, HelpStateEvents.Clicked, (int)tabIndex);
		yield break;
	}

	private void SelectTab(int tabIndex)
	{
		if (this.preTabIndex != tabIndex)
		{
			this.StopAutoXiLian();
		}
		this.preTabIndex = tabIndex;
		if (this.MenuTabList.TabIndex != 2)
		{
			this.LoadEquipList(tabIndex, this.jinjieExceptIds);
		}
		else
		{
			this.RefreshEquipAndCailiaoList(1, this.zhuZhuangbeiData, this.fuZhuangbeiData, this.EquipTabList.TabIndex);
		}
		SystemHelpMgr.OnAction(UIObjIDs.LianLuChuanChengSubmit, HelpStateEvents.Clicked, -1);
	}

	private void StopAutoXiLian()
	{
		if (null != this.lianluZhuangbeiXilianPart)
		{
			this.lianluZhuangbeiXilianPart.StopAutoTiShengWhenChangeItem();
			this.lianluZhuangbeiXilianPart.isContinueTiLian = true;
		}
	}

	private void SetHorseAndRole(LianluTypes showPage)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (this.HorseEquipMeltingOpen(showPage) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese, ref num, ref num2, ref num3))
		{
			this.m_PanelHorse.gameObject.SetActive(true);
			this.m_Scroll.background.transform.localScale = new Vector3(this.m_Scroll.background.transform.localScale.x, 300f, 1f);
			this.m_SpGunDongKuang.transform.localScale = new Vector3(this.m_SpGunDongKuang.transform.localScale.x, 360f, 1f);
			this.equipPanel.transform.localPosition = new Vector3(292f, -42f, 0f);
			this.equipPanel.clipRange = new Vector4(0f, 14f, 320f, 360f);
		}
		else
		{
			this.m_PanelHorse.gameObject.SetActive(false);
			this.m_Scroll.background.transform.localScale = new Vector3(this.m_Scroll.background.transform.localScale.x, 350f, 1f);
			this.m_SpGunDongKuang.transform.localScale = new Vector3(this.m_SpGunDongKuang.transform.localScale.x, 410f, 1f);
			this.equipPanel.transform.localPosition = new Vector3(292f, -42f, 0f);
			this.equipPanel.clipRange = new Vector4(0f, 0f, 320f, 388f);
		}
	}

	private void SetLianluPart(LianluTypes showPage)
	{
		this.partCanvas.Clear();
		switch (showPage)
		{
		case LianluTypes.Qianghua:
			if (null == this.lianluZhuangbeiQianghuaPart)
			{
				this.lianluZhuangbeiQianghuaPart = U3DUtils.NEW<LianluZhuangbeiQianghuaPart>();
				this.lianluZhuangbeiQianghuaPart.InitPartSize(0, 0);
				this.lianluZhuangbeiQianghuaPart.InitPartData();
				this.lianluZhuangbeiQianghuaPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluZhuangbeiQianghuaPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					this.SelectTab(this.EquipTabList.TabIndex);
				}
			};
			this.lianluZhuangbeiQianghuaPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuangbeiQianghuaPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuangbeiQianghuaPart);
			Canvas.SetLeft(this.lianluZhuangbeiQianghuaPart, 0);
			Canvas.SetTop(this.lianluZhuangbeiQianghuaPart, 0);
			break;
		case LianluTypes.Zhuijia:
			if (null == this.lianluZhuangbeiZhuijiaPart)
			{
				this.lianluZhuangbeiZhuijiaPart = U3DUtils.NEW<LianluZhuangbeiZhuijiaPart>();
				this.lianluZhuangbeiZhuijiaPart.InitPartSize(0, 0);
				this.lianluZhuangbeiZhuijiaPart.InitPartData();
				this.lianluZhuangbeiZhuijiaPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluZhuangbeiZhuijiaPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					this.SelectTab(this.EquipTabList.TabIndex);
				}
			};
			this.lianluZhuangbeiZhuijiaPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuangbeiZhuijiaPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuangbeiZhuijiaPart);
			Canvas.SetLeft(this.lianluZhuangbeiZhuijiaPart, 0);
			Canvas.SetTop(this.lianluZhuangbeiZhuijiaPart, 0);
			break;
		case LianluTypes.QianghuaChuancheng:
			if (null == this.lianluQianghuaChuanchenPart)
			{
				this.lianluQianghuaChuanchenPart = U3DUtils.NEW<LianluQianghuaChuanchenPart>();
				this.lianluQianghuaChuanchenPart.InitPartSize(0, 0);
				this.lianluQianghuaChuanchenPart.InitPartData();
				this.lianluQianghuaChuanchenPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluQianghuaChuanchenPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.LoadEquipList(this.EquipTabList.TabIndex, null);
				}
				else if (e.ID > 0)
				{
					if (e.ID == 1)
					{
						this.MenuIndex = LianluTypes.QianghuaChuancheng;
					}
					else if (e.ID == 2)
					{
						this.MenuIndex = LianluTypes.ZhuijiaChuancheng;
					}
					else if (e.ID == 3)
					{
						this.MenuIndex = LianluTypes.XilianChuancheng;
					}
					else if (e.ID == 4)
					{
						this.MenuIndex = LianluTypes.JuHunChuancheng;
					}
					else if (e.ID == 5)
					{
						this.MenuIndex = LianluTypes.FuMoChuanCheng;
					}
					this.equipPanelTran.localPosition = this.equipPanelPos;
					this.equipPanel.clipRange = this.equipPanelClipRange;
					this.RefreshEquipAndCailiaoList(e.FilterType, e.ZhuZhuangBei, e.FuZhuangBei, this.EquipTabList.TabIndex);
				}
			};
			this.lianluQianghuaChuanchenPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			if (this.zhuZhuangbeiData != null)
			{
				this.SetEffect(0, 0);
			}
			this.lianluQianghuaChuanchenPart.InitAllValue();
			this.partCanvas.Add(this.lianluQianghuaChuanchenPart);
			Canvas.SetLeft(this.lianluQianghuaChuanchenPart, 0);
			Canvas.SetTop(this.lianluQianghuaChuanchenPart, 0);
			break;
		case LianluTypes.ZhuangbeiZhuansheng:
			if (null == this.lianluZhuangbeiZhuanShengPart)
			{
				this.lianluZhuangbeiZhuanShengPart = U3DUtils.NEW<LianluZhuangbeiZhuanShengPart>();
				this.lianluZhuangbeiZhuanShengPart.InitPartSize(0, 0);
				this.lianluZhuangbeiZhuanShengPart.InitPartData();
				this.lianluZhuangbeiZhuanShengPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluZhuangbeiZhuanShengPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					this.SelectTab(this.EquipTabList.TabIndex);
				}
			};
			this.lianluZhuangbeiZhuanShengPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuangbeiZhuanShengPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuangbeiZhuanShengPart);
			Canvas.SetLeft(this.lianluZhuangbeiZhuanShengPart, 0);
			Canvas.SetTop(this.lianluZhuangbeiZhuanShengPart, 0);
			break;
		case LianluTypes.ZhuanshengBoli:
			if (null == this.lianluZhuanshengBoliPart)
			{
				this.lianluZhuanshengBoliPart = U3DUtils.NEW<LianluZhuanshengBoliPart>();
				this.lianluZhuanshengBoliPart.InitPartSize(0, 0);
				this.lianluZhuanshengBoliPart.InitPartData();
			}
			this.lianluZhuanshengBoliPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					this.SelectTab(this.EquipTabList.TabIndex);
				}
			};
			this.lianluZhuanshengBoliPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuanshengBoliPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuanshengBoliPart);
			Canvas.SetLeft(this.lianluZhuanshengBoliPart, 0);
			Canvas.SetTop(this.lianluZhuanshengBoliPart, 0);
			break;
		case LianluTypes.ZhuangbeiJinjie:
			if (null == this.lianluZhuangbeiJinjiePart)
			{
				this.lianluZhuangbeiJinjiePart = U3DUtils.NEW<LianluZhuangbeiJinjiePart>();
				this.lianluZhuangbeiJinjiePart.InitPartSize(0, 0);
				this.lianluZhuangbeiJinjiePart.InitPartData();
				this.lianluZhuangbeiJinjiePart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluZhuangbeiJinjiePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.LoadEquipList(0, e.EquipIDs);
				}
			};
			this.lianluZhuangbeiJinjiePart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuangbeiJinjiePart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuangbeiJinjiePart);
			Canvas.SetLeft(this.lianluZhuangbeiJinjiePart, 0);
			Canvas.SetTop(this.lianluZhuangbeiJinjiePart, 0);
			break;
		case LianluTypes.ZhuijiaChuancheng:
			if (null == this.lianluZhuijiaChuanchenPart)
			{
				this.lianluZhuijiaChuanchenPart = U3DUtils.NEW<LianluZhuijiaChuanchenPart>();
				this.lianluZhuijiaChuanchenPart.InitPartSize(0, 0);
				this.lianluZhuijiaChuanchenPart.InitPartData();
				this.lianluZhuijiaChuanchenPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.lianluZhuijiaChuanchenPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.LoadEquipList(this.EquipTabList.TabIndex, null);
				}
				else if (e.ID == 1)
				{
					this.equipPanelTran.localPosition = this.equipPanelPos;
					this.equipPanel.clipRange = this.equipPanelClipRange;
					this.RefreshEquipAndCailiaoList(e.FilterType, e.ZhuZhuangBei, e.FuZhuangBei, this.EquipTabList.TabIndex);
				}
			};
			this.lianluZhuijiaChuanchenPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			if (this.zhuZhuangbeiData != null)
			{
				this.SetEffect(0, 0);
			}
			this.lianluZhuijiaChuanchenPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuijiaChuanchenPart);
			Canvas.SetLeft(this.lianluZhuijiaChuanchenPart, 0);
			Canvas.SetTop(this.lianluZhuijiaChuanchenPart, 0);
			break;
		case LianluTypes.ZhuangbeiXilian:
			if (null == this.lianluZhuangbeiXilianPart)
			{
				this.lianluZhuangbeiXilianPart = U3DUtils.NEW<LianluZhuangbeiXilianPart>();
				this.lianluZhuangbeiXilianPart.InitPartSize(0, 0);
				this.lianluZhuangbeiXilianPart.InitPartData();
			}
			this.lianluZhuangbeiXilianPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.LoadEquipList(0, e.EquipIDs);
				}
			};
			this.lianluZhuangbeiXilianPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluZhuangbeiXilianPart.InitAllValue();
			this.partCanvas.Add(this.lianluZhuangbeiXilianPart);
			Canvas.SetLeft(this.lianluZhuangbeiXilianPart, 0);
			Canvas.SetTop(this.lianluZhuangbeiXilianPart, 0);
			break;
		case LianluTypes.ShenqiZaizao:
			if (null == this.lianluShenqiZaizaoPart)
			{
				this.lianluShenqiZaizaoPart = U3DUtils.NEW<LianluShenqiZaizaoPart>();
				this.lianluShenqiZaizaoPart.InitPartSize(0, 0);
				this.lianluShenqiZaizaoPart.InitPartData();
			}
			this.lianluShenqiZaizaoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 1)
				{
					this.SelectTab(this.EquipTabList.TabIndex);
				}
			};
			this.lianluShenqiZaizaoPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.lianluShenqiZaizaoPart.InitAllValue();
			this.partCanvas.Add(this.lianluShenqiZaizaoPart);
			Canvas.SetLeft(this.lianluShenqiZaizaoPart, 0);
			Canvas.SetTop(this.lianluShenqiZaizaoPart, 0);
			break;
		case LianluTypes.JuHun:
			if (null == this.juHunPart)
			{
				this.juHunPart = U3DUtils.NEW<LianLuJuHunPart>();
				this.juHunPart.InitPartData();
				this.juHunPart.InitAllValue();
				this.juHunPart.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = 0
					});
				};
			}
			this.juHunPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.RefreshEquipAndCailiaoList(e.FilterType, e.ZhuZhuangBei, e.FuZhuangBei, this.EquipTabList.TabIndex);
				}
			};
			this.juHunPart.ShowPropertyCallBack = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					NGUITools.SetActive(this.ShenshangZhuangbei, true);
					NGUITools.SetActive(this.ShuXing, false);
					this.DestroyShuXingPanel();
				}
				else if (e.ID == 1)
				{
					NGUITools.SetActive(this.ShenshangZhuangbei, false);
					NGUITools.SetActive(this.ShuXing, true);
					this.ItemCollection.Clear();
					this.equipPanelTran.localPosition = this.equipPanelPos;
					this.equipPanel.clipRange = this.equipPanelClipRange;
					if (null != this.shuXing)
					{
						this.shuXing.Visibility = true;
					}
					else
					{
						this.shuXing = U3DUtils.NEW<LianLuJuHunShuXingPart>();
						U3DUtils.AddChild(this.equipPanel.gameObject, this.shuXing.gameObject, true);
					}
					if (null != this.shuXing)
					{
						this.shuXing.InitValue(e.IDType, e.Data as GoodsData, e.Title);
					}
				}
			};
			this.juHunPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.partCanvas.Add(this.juHunPart);
			Canvas.SetLeft(this.juHunPart, 0);
			Canvas.SetTop(this.juHunPart, 0);
			break;
		case LianluTypes.ChongWuJiNengChuanCheng:
			if (this.jingLingJiNengChuanChengPart == null)
			{
				this.jingLingJiNengChuanChengPart = U3DUtils.NEW<JingLingJiNengChuanchengPart>();
				this.jingLingJiNengChuanChengPart.InitValue();
				this.jingLingJiNengChuanChengPart.InitPartSize();
			}
			this.jingLingJiNengChuanChengPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.RefreshEquipAndCailiaoList(e.FilterType, e.ZhuZhuangBei, e.FuZhuangBei, this.EquipTabList.TabIndex);
			};
			this.jingLingJiNengChuanChengPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.partCanvas.Add(this.jingLingJiNengChuanChengPart);
			break;
		case LianluTypes.ZhuangBeiFuMo:
			if (null == this.zhuanBeiFuMoPart)
			{
				this.zhuanBeiFuMoPart = U3DUtils.NEW<LianluZhuangbeiFuMoPart>();
			}
			this.zhuanBeiFuMoPart.DPEffectItem = delegate(object s, NotifyLianluEffectEventArgs e)
			{
				this.SetEffect(e.EffectID, e.PlayID);
			};
			this.partCanvas.Add(this.zhuanBeiFuMoPart);
			Canvas.SetLeft(this.zhuanBeiFuMoPart, 0);
			Canvas.SetTop(this.zhuanBeiFuMoPart, 0);
			break;
		}
	}

	private void DestroyShuXingPanel()
	{
		if (null != this.shuXing)
		{
			this.shuXing.Visibility = false;
			this.equipPanelTran.localPosition = this.equipPanelPos;
			this.equipPanel.clipRange = this.equipPanelClipRange;
		}
	}

	public bool HorseEquipMeltingOpen(LianluTypes type)
	{
		if (this.dicLianLuOpen.Count <= 0)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorseEquipMeltingOpen", ',');
			if (systemParamIntArrayByName.Length > 14)
			{
				for (int i = 0; i < systemParamIntArrayByName.Length; i++)
				{
					if (!this.dicLianLuOpen.ContainsKey(i))
					{
						this.dicLianLuOpen.Add(i, systemParamIntArrayByName[i] == 1);
					}
				}
			}
		}
		return this.dicLianLuOpen.ContainsKey((int)type) && this.dicLianLuOpen[(int)type];
	}

	public void RefreshEquipAndCailiaoList(int filterInt, GoodsData zhuZhuangbei, GoodsData fuZhuangbei, int isUsing)
	{
		this.initPanelPos();
		if (this.m_PanelHorse.gameObject.activeSelf && this.m_CheckHorse.Check && this.HorseEquipMeltingOpen(this.MenuIndex))
		{
			filterInt = 4;
		}
		if (this.MenuIndex == LianluTypes.ShenqiZaizao && this.lianluShenqiZaizaoPart != null)
		{
			if (filterInt == 1)
			{
				this.lianluShenqiZaizaoPart.QianghuaHintText.Text = Global.GetLang("10阶以上装备可通过再造获得进阶");
			}
			else if (filterInt == 4)
			{
				this.lianluShenqiZaizaoPart.QianghuaHintText.Text = Global.GetLang("7阶以上装备可通过再造获得进阶");
			}
		}
		if (filterInt == 1)
		{
			this.zhuZhuangbeiData = zhuZhuangbei;
			this.fuZhuangbeiData = fuZhuangbei;
			this.ItemsList.Clear();
			if (zhuZhuangbei != null && fuZhuangbei == null)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(zhuZhuangbei.GoodsID);
				int toOccupation = ConfigGoods.GetGoodsXmlNodeByID(zhuZhuangbei.GoodsID).ToOccupation;
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.GCount > 0)
					{
						if (goodsData.Using == isUsing)
						{
							if (this.MenuIndex == LianluTypes.QianghuaChuancheng || this.MenuIndex == LianluTypes.ZhuijiaChuancheng || this.MenuIndex == LianluTypes.XilianChuancheng || this.MenuIndex == LianluTypes.FuMoChuanCheng)
							{
								if (Global.GetGoodsOccByID(goodsData.GoodsID) == Global.GetGoodsOccByID(zhuZhuangbei.GoodsID))
								{
									if (this.MenuIndex == LianluTypes.QianghuaChuancheng)
									{
										if (goodsData.Forge_level >= zhuZhuangbei.Forge_level)
										{
											goto IL_3A3;
										}
									}
									else if (this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
									{
										if (goodsData.AppendPropLev >= zhuZhuangbei.AppendPropLev)
										{
											goto IL_3A3;
										}
									}
									else if (this.MenuIndex == LianluTypes.XilianChuancheng)
									{
										if (zhuZhuangbei.WashProps == null)
										{
											goto IL_3A3;
										}
										if (goodsData.WashProps == null)
										{
										}
										if (goodsData.ExcellenceInfo <= 0)
										{
											goto IL_3A3;
										}
										if (goodsData.Id == zhuZhuangbei.Id)
										{
											goto IL_3A3;
										}
										if (Global.IsToXilianPropsUpLimit(goodsData))
										{
											goto IL_3A3;
										}
									}
									else if (this.MenuIndex != LianluTypes.ChongWuJiNengChuanCheng)
									{
										if (this.MenuIndex == LianluTypes.FuMoChuanCheng && goodsData.Id == zhuZhuangbei.Id)
										{
											goto IL_3A3;
										}
									}
									int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
									if (categoriyByGoodsID >= 11 && categoriyByGoodsID < 21 && categoriyByGoodsID != 18)
									{
										if (categoriyByGoodsID2 >= 11 && categoriyByGoodsID2 < 21 && categoriyByGoodsID2 != 18)
										{
											int toOccupation2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).ToOccupation;
											if (toOccupation == toOccupation2)
											{
												this.AddIcon(Global.Data.roleData.GoodsDataList[i]);
											}
										}
									}
									else if (categoriyByGoodsID == categoriyByGoodsID2)
									{
										this.AddIcon(Global.Data.roleData.GoodsDataList[i]);
									}
								}
							}
							else if (this.MenuIndex == LianluTypes.JuHunChuancheng)
							{
								GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
								int suitID = goodsXmlNodeByID.SuitID;
								if (suitID >= 11)
								{
									if (goodsData.Id != zhuZhuangbei.Id)
									{
										int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
										if ((categoriyByGoodsID2 <= 6 || categoriyByGoodsID2 >= 10) && (categoriyByGoodsID2 <= 19 || categoriyByGoodsID2 == 21))
										{
											this.AddIcon(Global.Data.roleData.GoodsDataList[i]);
										}
									}
								}
							}
						}
					}
					IL_3A3:;
				}
				this.ShowPage();
			}
			else if (zhuZhuangbei == null && fuZhuangbei != null)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(fuZhuangbei.GoodsID);
				int toOccupation = ConfigGoods.GetGoodsXmlNodeByID(fuZhuangbei.GoodsID).ToOccupation;
				for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[j];
					if (goodsData.GCount > 0)
					{
						if (goodsData.Using == isUsing)
						{
							if (this.MenuIndex == LianluTypes.QianghuaChuancheng || this.MenuIndex == LianluTypes.ZhuijiaChuancheng || this.MenuIndex == LianluTypes.XilianChuancheng || this.MenuIndex == LianluTypes.FuMoChuanCheng)
							{
								if (Global.GetGoodsOccByID(goodsData.GoodsID) == Global.GetGoodsOccByID(fuZhuangbei.GoodsID))
								{
									if (this.MenuIndex == LianluTypes.QianghuaChuancheng)
									{
										if (goodsData.Forge_level <= fuZhuangbei.Forge_level)
										{
											goto IL_67E;
										}
									}
									else if (this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
									{
										if (goodsData.AppendPropLev <= fuZhuangbei.AppendPropLev)
										{
											goto IL_67E;
										}
									}
									else if (this.MenuIndex == LianluTypes.XilianChuancheng)
									{
										if (goodsData.WashProps == null)
										{
											goto IL_67E;
										}
										if (goodsData.Id == fuZhuangbei.Id)
										{
											goto IL_67E;
										}
									}
									else if (this.MenuIndex == LianluTypes.FuMoChuanCheng)
									{
										if (goodsData.ElementhrtsProps == null || goodsData.ElementhrtsProps.Count <= 0)
										{
											goto IL_67E;
										}
										if (goodsData.ElementhrtsProps[1] <= 0)
										{
											goto IL_67E;
										}
										if (goodsData.Id == fuZhuangbei.Id)
										{
											goto IL_67E;
										}
									}
									int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
									if (categoriyByGoodsID >= 11 && categoriyByGoodsID < 21 && categoriyByGoodsID != 18)
									{
										if (categoriyByGoodsID2 >= 11 && categoriyByGoodsID2 < 21 && categoriyByGoodsID2 != 18)
										{
											int toOccupation2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).ToOccupation;
											if (toOccupation == toOccupation2)
											{
												this.AddIcon(Global.Data.roleData.GoodsDataList[j]);
											}
										}
									}
									else if (categoriyByGoodsID == categoriyByGoodsID2)
									{
										this.AddIcon(Global.Data.roleData.GoodsDataList[j]);
									}
								}
							}
							else if (this.MenuIndex == LianluTypes.JuHunChuancheng)
							{
								GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
								int suitID2 = goodsXmlNodeByID2.SuitID;
								if (suitID2 >= 11)
								{
									if (goodsData.JuHunID > 0)
									{
										if (goodsData.Id != fuZhuangbei.Id)
										{
											this.AddIcon(Global.Data.roleData.GoodsDataList[j]);
										}
									}
								}
							}
						}
					}
					IL_67E:;
				}
				this.ShowPage();
			}
			else if (zhuZhuangbei == null && fuZhuangbei == null)
			{
				this.LoadEquipList(this.EquipTabList.TabIndex, null);
			}
		}
		else if (filterInt == 2)
		{
			if (fuZhuangbei != null)
			{
				this.fuZhuangbeiData = fuZhuangbei;
				this.ItemsList_Cailiao.Clear();
				int equipLevel = 0;
				if (this.MenuIndex == LianluTypes.QianghuaChuancheng)
				{
					equipLevel = fuZhuangbei.Forge_level;
				}
				else if (this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
				{
					equipLevel = fuZhuangbei.AppendPropLev;
				}
				for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[k];
					if (Global.IsShengyoufuGoodsID(goodsData.GoodsID) && Global.CheckShengyoufuIsHefa(goodsData.GoodsID, equipLevel))
					{
						this.AddCailiaoIcon(goodsData);
					}
				}
				this.ShowPage_Cailiao(this.CurrentSelectedPage_Cailiao);
			}
			else if (fuZhuangbei == null)
			{
				this.LoadCailiaoList(2);
			}
		}
		else if (filterInt == 3)
		{
			this.ItemsList.Clear();
			this.zhuZhuangbeiData = zhuZhuangbei;
			this.fuZhuangbeiData = fuZhuangbei;
			if (this.EquipTabList.TabIndex == 1)
			{
				for (int l = 0; l < Global.Data.equipPet.Count; l++)
				{
					GoodsData goodsData = Global.Data.equipPet[l];
					int num = this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level + 1);
					if (num > 0)
					{
						if (this.zhuZhuangbeiData != null)
						{
							int num2 = this.InitSkillsData(this.zhuZhuangbeiData.ElementhrtsProps, this.zhuZhuangbeiData.Forge_level + 1);
							if (num2 > num)
							{
								goto IL_8E6;
							}
							if (this.zhuZhuangbeiData.Id == goodsData.Id)
							{
								goto IL_8E6;
							}
						}
						if (this.fuZhuangbeiData != null)
						{
							if (this.zhuZhuangbeiData == null)
							{
								int num2 = this.InitSkillsData(this.fuZhuangbeiData.ElementhrtsProps, this.fuZhuangbeiData.Forge_level + 1);
								if (num2 < num)
								{
									goto IL_8E6;
								}
							}
							if (this.fuZhuangbeiData.Id == goodsData.Id)
							{
								goto IL_8E6;
							}
						}
						this.AddIcon(Global.Data.equipPet[l]);
					}
					IL_8E6:;
				}
			}
			else if (this.EquipTabList.TabIndex == 0)
			{
				for (int m = 0; m < Global.Data.roleData.GoodsDataList.Count; m++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[m];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (categoriyByGoodsID == 10 || categoriyByGoodsID == 9)
					{
						int num = this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level + 1);
						if (num > 0)
						{
							if (this.zhuZhuangbeiData != null)
							{
								int num2 = this.InitSkillsData(this.zhuZhuangbeiData.ElementhrtsProps, this.zhuZhuangbeiData.Forge_level + 1);
								if (num2 > num)
								{
									goto IL_A49;
								}
								if (this.zhuZhuangbeiData.Id == goodsData.Id)
								{
									goto IL_A49;
								}
							}
							if (this.fuZhuangbeiData != null)
							{
								if (this.zhuZhuangbeiData == null)
								{
									int num2 = this.InitSkillsData(this.fuZhuangbeiData.ElementhrtsProps, this.fuZhuangbeiData.Forge_level + 1);
									if (num2 < num)
									{
										goto IL_A49;
									}
								}
								if (this.fuZhuangbeiData.Id == goodsData.Id)
								{
									goto IL_A49;
								}
							}
							this.AddIcon(Global.Data.roleData.GoodsDataList[m]);
						}
					}
					IL_A49:;
				}
			}
			else if (zhuZhuangbei == null && fuZhuangbei == null)
			{
				this.LoadEquipList(this.EquipTabList.TabIndex, null);
			}
			this.ShowPage();
		}
		else if (filterInt == 4 && this.m_CheckHorse.Check && this.HorseEquipMeltingOpen(this.MenuIndex))
		{
			this.ItemsList.Clear();
			this.zhuZhuangbeiData = zhuZhuangbei;
			this.fuZhuangbeiData = fuZhuangbei;
			List<GoodsData> list = new List<GoodsData>();
			for (int n = 0; n < Global.Data.roleData.GoodsDataList.Count; n++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[n];
				if (goodsData.GCount > 0)
				{
					if (this.EquipTabList.TabIndex != 1 || goodsData.Using == 1)
					{
						if (this.EquipTabList.TabIndex != 0 || goodsData.Using == 0)
						{
							if (Global.HorseEquipOpen(goodsData.GoodsID))
							{
								list.Add(goodsData);
							}
						}
					}
				}
			}
			list.Sort(new Comparison<GoodsData>(this.SoryByCategoriyZuoQi));
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				GoodsData goodsData = list[num3];
				GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				if (zhuZhuangbei != null && fuZhuangbei == null)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(zhuZhuangbei.GoodsID);
					if (categoriyByGoodsID >= 40 && categoriyByGoodsID <= 45)
					{
						if (categoriyByGoodsID == goodsXmlNodeByID3.Categoriy)
						{
							if (this.MenuIndex == LianluTypes.QianghuaChuancheng || this.MenuIndex == LianluTypes.ZhuijiaChuancheng || this.MenuIndex == LianluTypes.XilianChuancheng)
							{
								if (this.MenuIndex == LianluTypes.QianghuaChuancheng)
								{
									if (goodsData.Forge_level >= zhuZhuangbei.Forge_level)
									{
										goto IL_E99;
									}
								}
								else if (this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
								{
									if (goodsData.AppendPropLev >= zhuZhuangbei.AppendPropLev)
									{
										goto IL_E99;
									}
								}
								else if (this.MenuIndex == LianluTypes.XilianChuancheng)
								{
									if (zhuZhuangbei.WashProps == null)
									{
										goto IL_E99;
									}
									if (goodsData.WashProps == null)
									{
									}
									if (goodsData.ExcellenceInfo <= 0)
									{
										goto IL_E99;
									}
									if (goodsData.Id == zhuZhuangbei.Id)
									{
										goto IL_E99;
									}
									if (Global.IsToXilianPropsUpLimit(goodsData))
									{
										goto IL_E99;
									}
								}
								this.AddIcon(goodsData);
							}
							else if (this.MenuIndex == LianluTypes.JuHunChuancheng)
							{
								GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
								int suitID3 = goodsXmlNodeByID4.SuitID;
								if (suitID3 >= 11)
								{
									if (goodsData.Id != zhuZhuangbei.Id)
									{
										this.AddIcon(goodsData);
									}
								}
							}
						}
					}
				}
				else if (zhuZhuangbei == null && fuZhuangbei != null)
				{
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(fuZhuangbei.GoodsID);
					if (categoriyByGoodsID < 40 || categoriyByGoodsID < 45)
					{
						return;
					}
					if (categoriyByGoodsID == goodsXmlNodeByID3.Categoriy)
					{
						if (this.MenuIndex == LianluTypes.QianghuaChuancheng || this.MenuIndex == LianluTypes.ZhuijiaChuancheng || this.MenuIndex == LianluTypes.XilianChuancheng)
						{
							if (this.MenuIndex == LianluTypes.QianghuaChuancheng)
							{
								if (goodsData.Forge_level <= fuZhuangbei.Forge_level)
								{
									goto IL_E99;
								}
							}
							else if (this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
							{
								if (goodsData.AppendPropLev <= fuZhuangbei.AppendPropLev)
								{
									goto IL_E99;
								}
							}
							else if (this.MenuIndex == LianluTypes.XilianChuancheng)
							{
								if (goodsData.WashProps == null)
								{
									goto IL_E99;
								}
								if (goodsData.Id == fuZhuangbei.Id)
								{
									goto IL_E99;
								}
							}
							this.AddIcon(goodsData);
						}
						else if (this.MenuIndex == LianluTypes.JuHunChuancheng)
						{
							GoodVO goodsXmlNodeByID5 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							int suitID4 = goodsXmlNodeByID5.SuitID;
							if (suitID4 >= 11)
							{
								if (goodsData.JuHunID > 0)
								{
									if (goodsData.Id != fuZhuangbei.Id)
									{
										this.AddIcon(goodsData);
									}
								}
							}
						}
					}
				}
				else if (zhuZhuangbei == null && fuZhuangbei == null)
				{
					this.LoadEquipList(this.EquipTabList.TabIndex, null);
					return;
				}
				IL_E99:;
			}
			this.ShowPage();
		}
	}

	private void LoadEquipList(int isUsing, int[] exceptIds = null)
	{
		this.initPanelPos();
		if (this.MenuIndex == LianluTypes.ChongWuJiNengChuanCheng)
		{
			this.ItemsList.Clear();
			if (this.EquipTabList.TabIndex == 1)
			{
				for (int i = 0; i < Global.Data.equipPet.Count; i++)
				{
					GoodsData goodsData = Global.Data.equipPet[i];
					int num = this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level + 1);
					if (num > 0)
					{
						if (this.zhuZhuangbeiData != null)
						{
							int num2 = this.InitSkillsData(this.zhuZhuangbeiData.ElementhrtsProps, this.zhuZhuangbeiData.Forge_level + 1);
							if (num2 > num)
							{
								goto IL_122;
							}
							if (this.zhuZhuangbeiData.Id == goodsData.Id)
							{
								goto IL_122;
							}
						}
						if (this.fuZhuangbeiData != null)
						{
							if (this.zhuZhuangbeiData == null)
							{
								int num2 = this.InitSkillsData(this.fuZhuangbeiData.ElementhrtsProps, this.fuZhuangbeiData.Forge_level + 1);
								if (num2 < num)
								{
									goto IL_122;
								}
							}
							if (this.fuZhuangbeiData.Id == goodsData.Id)
							{
								goto IL_122;
							}
						}
						this.AddIcon(goodsData);
					}
					IL_122:;
				}
			}
			else if (this.EquipTabList.TabIndex == 0)
			{
				for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[j];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (categoriyByGoodsID == 10 || categoriyByGoodsID == 9)
					{
						int num = this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level + 1);
						if (num > 0)
						{
							if (this.zhuZhuangbeiData != null)
							{
								int num2 = this.InitSkillsData(this.zhuZhuangbeiData.ElementhrtsProps, this.zhuZhuangbeiData.Forge_level + 1);
								if (num2 > num)
								{
									goto IL_268;
								}
								if (this.zhuZhuangbeiData.Id == goodsData.Id)
								{
									goto IL_268;
								}
							}
							if (this.fuZhuangbeiData != null)
							{
								if (this.zhuZhuangbeiData == null)
								{
									int num2 = this.InitSkillsData(this.fuZhuangbeiData.ElementhrtsProps, this.fuZhuangbeiData.Forge_level + 1);
									if (num2 < num)
									{
										goto IL_268;
									}
								}
								if (this.fuZhuangbeiData.Id == goodsData.Id)
								{
									goto IL_268;
								}
							}
							this.AddIcon(goodsData);
						}
					}
					IL_268:;
				}
			}
			this.ShowPage();
			return;
		}
		if (this.m_CheckHorse.Check && this.HorseEquipMeltingOpen(this.MenuIndex))
		{
			this.ItemsList.Clear();
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
				{
					GoodsData goodsData2 = Global.Data.roleData.GoodsDataList[k];
					if (this.EquipTabList.TabIndex != 1 || goodsData2.Using == 1)
					{
						if (this.EquipTabList.TabIndex != 0 || goodsData2.Using == 0)
						{
							if (Global.HorseEquipOpen(goodsData2.GoodsID))
							{
								list.Add(goodsData2);
							}
						}
					}
				}
			}
			list.Sort(new Comparison<GoodsData>(this.SoryByCategoriyZuoQi));
			int l = 0;
			while (l < list.Count)
			{
				GoodsData goodsData2 = list[l];
				int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData2.GoodsID);
				if (this.MenuIndex == LianluTypes.Qianghua)
				{
					if (goodsData2.Forge_level < Global.MaxForgeLevel)
					{
						goto IL_672;
					}
				}
				else if (this.MenuIndex == LianluTypes.Zhuijia)
				{
					if (goodsData2.AppendPropLev < 80)
					{
						goto IL_672;
					}
				}
				else if (this.MenuIndex == LianluTypes.ZhuangbeiZhuansheng)
				{
					if (goodsData2.ChangeLifeLevForEquip < 10)
					{
						goto IL_672;
					}
				}
				else if (this.MenuIndex == LianluTypes.ZhuanshengBoli)
				{
					if (goodsData2.ChangeLifeLevForEquip > 0)
					{
						goto IL_672;
					}
				}
				else if (this.MenuIndex == LianluTypes.ZhuangbeiJinjie)
				{
					int equipGoodsPropsByJinJie = Global.GetEquipGoodsPropsByJinJie(goodsData2.GoodsID);
					if (equipGoodsPropsByJinJie > 0)
					{
						if (exceptIds != null)
						{
							this.jinjieExceptIds = exceptIds;
							if (exceptIds.IndexOf(goodsData2.Id) != -1)
							{
								goto IL_67A;
							}
							int greaterThan = this.GetGreaterThan0(exceptIds);
							if (greaterThan != -1)
							{
								GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(exceptIds[greaterThan], null);
								if (goodsDataByDbID != null)
								{
									if (goodsDataByDbID.GoodsID != goodsData2.GoodsID)
									{
										goto IL_67A;
									}
									if (Global.GetColorByGoodsData(goodsDataByDbID) != Global.GetColorByGoodsData(goodsData2))
									{
										goto IL_67A;
									}
								}
							}
						}
						goto IL_672;
					}
				}
				else if (this.MenuIndex == LianluTypes.ZhuangbeiXilian)
				{
					int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData2);
					if (zhuoyueAttributeCount > 0)
					{
						goto IL_672;
					}
				}
				else
				{
					if (this.MenuIndex == LianluTypes.XilianChuancheng)
					{
						goto IL_672;
					}
					if (this.MenuIndex == LianluTypes.JuHunChuancheng)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
						int suitID = goodsXmlNodeByID.SuitID;
						if (suitID >= 11)
						{
							if (goodsData2.JuHunID > 0)
							{
								goto IL_672;
							}
						}
					}
					else if (this.MenuIndex == LianluTypes.FuMoChuanCheng)
					{
						if (goodsData2.ElementhrtsProps != null && goodsData2.ElementhrtsProps.Count > 0)
						{
							if (goodsData2.ElementhrtsProps[1] > 0)
							{
								goto IL_672;
							}
						}
					}
					else if (this.MenuIndex == LianluTypes.ShenqiZaizao)
					{
						int equipGoodsSuitID = Global.GetEquipGoodsSuitID(goodsData2.GoodsID);
						int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorseZaiZaoSuitID", ',');
						bool flag = true;
						int num3 = 0;
						if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length != 0)
						{
							while (flag && num3 < systemParamIntArrayByName.Length)
							{
								if (equipGoodsSuitID == systemParamIntArrayByName[num3++])
								{
									flag = false;
								}
							}
							if (!flag)
							{
								goto IL_672;
							}
						}
					}
					else
					{
						if (this.MenuIndex != LianluTypes.JuHun)
						{
							goto IL_672;
						}
						GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
						int suitID2 = goodsXmlNodeByID2.SuitID;
						if (suitID2 >= 11)
						{
							goto IL_672;
						}
					}
				}
				IL_67A:
				l++;
				continue;
				IL_672:
				this.AddIcon(goodsData2);
				goto IL_67A;
			}
			this.ShowPage();
			return;
		}
		if (Global.Data.roleData.GoodsDataList != null)
		{
			this.ItemsList.Clear();
			List<GoodsData> list2 = new List<GoodsData>();
			for (int m = 0; m < Global.Data.roleData.GoodsDataList.Count; m++)
			{
				GoodsData goodsData3 = Global.Data.roleData.GoodsDataList[m];
				if (goodsData3.GCount > 0)
				{
					int categoriyByGoodsID3 = Global.GetCategoriyByGoodsID(goodsData3.GoodsID);
					if (categoriyByGoodsID3 >= 0 && categoriyByGoodsID3 < 25)
					{
						categoriyByGoodsID3 = Global.GetCategoriyByGoodsID(goodsData3.GoodsID);
						if (categoriyByGoodsID3 != 9 && categoriyByGoodsID3 != 10 && categoriyByGoodsID3 != 7)
						{
							list2.Add(goodsData3);
						}
					}
				}
			}
			list2.Sort(new Comparison<GoodsData>(this.SoryByCategoriy));
			for (int n = 0; n < list2.Count; n++)
			{
				GoodsData goodsData3 = list2[n];
				int categoriyByGoodsID3 = Global.GetCategoriyByGoodsID(goodsData3.GoodsID);
				if ((categoriyByGoodsID3 <= 6 || categoriyByGoodsID3 >= 10) && (categoriyByGoodsID3 <= 19 || categoriyByGoodsID3 == 21))
				{
					if (this.MenuIndex == LianluTypes.Qianghua)
					{
						if (goodsData3.Forge_level >= Global.MaxForgeLevel)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.Zhuijia)
					{
						if (goodsData3.AppendPropLev >= 80)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.ZhuangbeiZhuansheng)
					{
						if (goodsData3.ChangeLifeLevForEquip >= 10)
						{
							goto IL_B32;
						}
						if (categoriyByGoodsID3 > 21 || categoriyByGoodsID3 == 9 || categoriyByGoodsID3 == 10 || categoriyByGoodsID3 == 7)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.ZhuanshengBoli)
					{
						if (goodsData3.ChangeLifeLevForEquip <= 0)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.ZhuangbeiJinjie)
					{
						int equipGoodsPropsByJinJie2 = Global.GetEquipGoodsPropsByJinJie(goodsData3.GoodsID);
						if (equipGoodsPropsByJinJie2 <= 0)
						{
							goto IL_B32;
						}
						if (exceptIds != null)
						{
							this.jinjieExceptIds = exceptIds;
							if (exceptIds.IndexOf(goodsData3.Id) != -1)
							{
								goto IL_B32;
							}
							int greaterThan2 = this.GetGreaterThan0(exceptIds);
							if (greaterThan2 != -1)
							{
								GoodsData goodsDataByDbID2 = Global.GetGoodsDataByDbID(exceptIds[greaterThan2], null);
								if (goodsDataByDbID2 != null)
								{
									if (goodsDataByDbID2.GoodsID != goodsData3.GoodsID)
									{
										goto IL_B32;
									}
									if (Global.GetColorByGoodsData(goodsDataByDbID2) != Global.GetColorByGoodsData(goodsData3))
									{
										goto IL_B32;
									}
								}
							}
						}
					}
					else if (this.MenuIndex == LianluTypes.ZhuangbeiXilian)
					{
						int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(goodsData3);
						if (zhuoyueAttributeCount2 <= 0)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.XilianChuancheng)
					{
						if (goodsData3.WashProps == null)
						{
						}
					}
					else if (this.MenuIndex == LianluTypes.JuHunChuancheng)
					{
						GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData3.GoodsID);
						int suitID3 = goodsXmlNodeByID3.SuitID;
						if (suitID3 < 11)
						{
							goto IL_B32;
						}
						if (goodsData3.JuHunID <= 0)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.FuMoChuanCheng)
					{
						if (goodsData3.ElementhrtsProps == null || goodsData3.ElementhrtsProps.Count <= 0)
						{
							goto IL_B32;
						}
						if (goodsData3.ElementhrtsProps[1] <= 0)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.ShenqiZaizao)
					{
						int equipGoodsSuitID2 = Global.GetEquipGoodsSuitID(goodsData3.GoodsID);
						int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("ZaiZaoSuitID", ',');
						bool flag2 = true;
						int num4 = 0;
						if (systemParamIntArrayByName2 == null || systemParamIntArrayByName2.Length == 0)
						{
							goto IL_B32;
						}
						while (flag2 && num4 < systemParamIntArrayByName2.Length)
						{
							if (equipGoodsSuitID2 == systemParamIntArrayByName2[num4++])
							{
								flag2 = false;
							}
						}
						if (flag2)
						{
							goto IL_B32;
						}
						if ((categoriyByGoodsID3 > 6 && categoriyByGoodsID3 < 11) || categoriyByGoodsID3 > 21)
						{
							goto IL_B32;
						}
					}
					else if (this.MenuIndex == LianluTypes.JuHun)
					{
						GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(goodsData3.GoodsID);
						int suitID4 = goodsXmlNodeByID4.SuitID;
						if ((goodsXmlNodeByID4.Categoriy > 6 && goodsXmlNodeByID4.Categoriy < 11) || goodsXmlNodeByID4.Categoriy == 20 || goodsXmlNodeByID4.Categoriy > 21)
						{
							goto IL_B32;
						}
						if (suitID4 < 11)
						{
							goto IL_B32;
						}
					}
					if (goodsData3.Using == isUsing)
					{
						this.AddIcon(list2[n]);
					}
				}
				IL_B32:;
			}
		}
		this.ShowPage();
	}

	public int GetGreaterThan0(int[] ids)
	{
		if (ids != null)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				if (ids[i] > 0)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private void initCategoriyIndexDict()
	{
		for (int i = 0; i < this.categoriyIndexArr.Length; i++)
		{
			if (!this.categoriyIndexDict.ContainsKey(this.categoriyIndexArr[i]))
			{
				this.categoriyIndexDict.Add(this.categoriyIndexArr[i], i);
			}
		}
	}

	private int SoryByCategoriyZuoQi(GoodsData a, GoodsData b)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(a.GoodsID);
		int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(b.GoodsID);
		return categoriyByGoodsID.CompareTo(categoriyByGoodsID2);
	}

	private int SoryByCategoriy(GoodsData a, GoodsData b)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(a.GoodsID);
		int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(b.GoodsID);
		return this.categoriyIndexDict[categoriyByGoodsID].CompareTo(this.categoriyIndexDict[categoriyByGoodsID2]);
	}

	private bool ChackSkillDataIsAllZore(List<int> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (data[0] != 0)
			{
				return false;
			}
		}
		return true;
	}

	private bool[] CheckJingLingLev(int Forge_level)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PatSkillCostLevel", '|');
		bool[] array = new bool[4];
		byte b = 0;
		while ((int)b < systemParamStringArrayByName.Length)
		{
			string[] array2 = systemParamStringArrayByName[(int)b].Split(new char[]
			{
				','
			});
			if (Forge_level >= Convert.ToInt32(array2[1]))
			{
				array[(int)b] = true;
			}
			b += 1;
		}
		return array;
	}

	public int InitSkillsData(List<int> data, int Forge_level = 0)
	{
		int num = 0;
		bool[] array = this.CheckJingLingLev(Forge_level);
		if (data == null || this.ChackSkillDataIsAllZore(data))
		{
			int num2 = 2;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i])
				{
					num++;
				}
				int num3 = num2 + 1;
				int num4 = num3 + 1;
				num2 = num4 + 1;
			}
		}
		else if (0 < data.Count)
		{
			int j = 0;
			int num5 = 2;
			int num6 = 0;
			while (j < data.Count)
			{
				if (array[num6])
				{
					num++;
				}
				j = num5 + 1;
				int num7 = j + 1;
				num5 = num7 + 1;
				num6++;
			}
		}
		return num;
	}

	private void AddIcon(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return;
		}
		LianluEquipItem equipItem = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			if ((categoriy >= 0 && categoriy < 25) || (categoriy >= 40 && categoriy <= 45) || Global.IsRebornEquip(categoriy))
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				int categoriy2 = goodsXmlNodeByID.Categoriy;
				try
				{
					equipItem = U3DUtils.NEW<LianluEquipItem>();
					equipItem.EquipIcon.Width = 78.0;
					equipItem.EquipIcon.Height = 78.0;
					equipItem.EquipIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
					equipItem.EquipIcon.ItemCategory = categoriy2;
					equipItem.EquipIcon.ItemCode = goodsData.GoodsID;
					equipItem.EquipIcon.ItemObject = goodsData;
					equipItem.EquipIcon.STextColor = 16381698U;
					equipItem.EquipIcon.STextShadowColor = 0U;
					equipItem.EquipIcon.STextHorizontalAlignment = global::Layout.Right;
					equipItem.EquipIcon.STextVerticalAlignment = global::Layout.Top;
					equipItem.EquipIcon.ContentText.transform.localPosition = new Vector3(equipItem.EquipIcon.ContentText.transform.localPosition.x, equipItem.EquipIcon.ContentText.transform.localPosition.y, -2f);
					if (this.MenuIndex == LianluTypes.JuHun || this.MenuIndex == LianluTypes.JuHunChuancheng)
					{
						equipItem.TxtEquipName.Text = UIHelper.FormatGoodsName(goodsData, false, true, true);
					}
					else if (this.MenuIndex == LianluTypes.ChongWuJiNengChuanCheng)
					{
						equipItem.TxtEquipName.Text = Global.GetColorStringForNGUIText(new object[]
						{
							"b266ff",
							Global.GetLang("技能") + goodsXmlNodeByID.Title + this.InitSkillsData(goodsData.ElementhrtsProps, goodsData.Forge_level + 1)
						});
					}
					else
					{
						equipItem.TxtEquipName.Text = UIHelper.FormatGoodsName(goodsData, false, true, false);
					}
					equipItem.EquipIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						if (!SystemHelpPart.IsMaskShowing())
						{
							GoodsData goodsData2 = equipItem.EquipIcon.ItemObject as GoodsData;
							GTipServiceEx.ShowTip(equipItem.EquipIcon, TipTypes.GoodsText, GoodsOwnerTypes.Lianlu, equipItem.EquipIcon.ItemObject as GoodsData);
						}
					};
					equipItem.EquipIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
					{
						if (e.IDType == 5)
						{
							this.equipList.DoSelectItem(equipItem.gameObject);
						}
					};
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
				bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
				if (this.MenuIndex == LianluTypes.Zhuijia || this.MenuIndex == LianluTypes.ZhuijiaChuancheng)
				{
					Super.InitGoodsGIcon(equipItem.EquipIcon, goodsData, canUse, IconTextTypes.Zhuijia);
				}
				else if (this.MenuIndex == LianluTypes.ZhuangbeiZhuansheng || this.MenuIndex == LianluTypes.ZhuanshengBoli)
				{
					Super.InitGoodsGIcon(equipItem.EquipIcon, goodsData, canUse, IconTextTypes.Zhuansheng);
				}
				else
				{
					Super.InitGoodsGIcon(equipItem.EquipIcon, goodsData, canUse, IconTextTypes.Qianghua);
				}
				if (this.MenuIndex == LianluTypes.ShenqiZaizao)
				{
					equipItem.UpgradeStat = Global.IsEnabledZaizao(goodsData);
				}
				if (Global.IsShengqi(goodsData))
				{
					Vector3 localPosition = equipItem.EquipIcon.GoodImg.transform.localPosition;
					localPosition.z = -0.1f;
					equipItem.EquipIcon.GoodImg.transform.localPosition = localPosition;
				}
				if (equipItem.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(equipItem.GetComponent<UIPanel>());
				}
				this.ItemsList.Add(equipItem);
			}
		}
	}

	private void EquipIconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.MenuIndex == LianluTypes.ZhuangbeiXilian && !this.IsEnableContinueByLianlu(LianluZhuangbeiXilianPart.IsSave))
		{
			return;
		}
		LianluEquipItem lianluEquipItem = U3DUtils.AS<LianluEquipItem>(this.equipList.SelectedItem);
		if (null == lianluEquipItem)
		{
			return;
		}
		if (this.temp != null && this.temp != lianluEquipItem)
		{
			this.temp.Bak.spriteName = "lianluEquipItem_bak";
		}
		if (this.temp == lianluEquipItem)
		{
			return;
		}
		this.temp = lianluEquipItem;
		this.temp.Bak.spriteName = "lianluEquipItem_bak2";
		GGoodIcon equipIcon = lianluEquipItem.EquipIcon;
		if (null == equipIcon)
		{
			return;
		}
		if (Global.IsShengqi(Global.GetGoodsDataByID(equipIcon.GoodsID)))
		{
			Vector3 localPosition = equipIcon.GoodImg.transform.localPosition;
			localPosition.z = -0.1f;
			equipIcon.GoodImg.transform.localPosition = localPosition;
		}
		switch (this.MenuTabList.TabIndex)
		{
		case 0:
			this.lianluZhuangbeiQianghuaPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 1:
			this.lianluZhuangbeiZhuijiaPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 2:
			this.lianluQianghuaChuanchenPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 3:
			this.lianluZhuangbeiZhuanShengPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 4:
			this.lianluZhuanshengBoliPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 5:
			if (this.lianluZhuangbeiJinjiePart.TrueForAll(0))
			{
				this.lianluZhuangbeiJinjiePart.AddEquipAuto(equipIcon.ItemObject as GoodsData);
			}
			else
			{
				this.lianluZhuangbeiJinjiePart.AddEquip(equipIcon.ItemObject as GoodsData);
			}
			break;
		case 6:
			this.lianluZhuijiaChuanchenPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 7:
			this.StopAutoXiLian();
			this.lianluZhuangbeiXilianPart.AddEquip(equipIcon.ItemObject as GoodsData, 1);
			break;
		case 9:
			this.lianluShenqiZaizaoPart.AddEquip(equipIcon.ItemObject as GoodsData, true);
			break;
		case 11:
			this.initPanelPos();
			this.juHunPart.AddEquip(equipIcon.ItemObject as GoodsData, true);
			this.juHunPart.InitHunHuoTeXiao(equipIcon.ItemObject as GoodsData);
			break;
		case 12:
			this.jingLingJiNengChuanChengPart.AddEquip(equipIcon.ItemObject as GoodsData);
			break;
		case 13:
			this.zhuanBeiFuMoPart.RefreshEquip(equipIcon.ItemObject as GoodsData);
			break;
		}
		Global.PlaySoundAudio("Audio/UI/fangru_wupin", false);
		SystemHelpMgr.OnAction(UIObjIDs.LianLuEquipItem, HelpStateEvents.Clicked, -1);
	}

	private void IconMouseLeftButtonUp_Cailiao(object sender, MouseEvent e)
	{
		GIcon gicon = sender as GIcon;
		if (null == gicon)
		{
			return;
		}
		GoodsData goodsData = gicon.ItemObject as GoodsData;
		switch (this.SelectedIcon.ItemCode)
		{
		case 0:
			this.lianluZhuangbeiQianghuaPart.AddGoodsIcon(goodsData.GoodsID, 2, 1);
			break;
		case 2:
			this.lianluQianghuaChuanchenPart.AddGoodsIcon(goodsData.GoodsID, 2, 1);
			break;
		case 5:
			this.lianluJinglianPart.AddGoodsIcon(goodsData.GoodsID, 5, 1);
			break;
		}
	}

	private void ShowPage()
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			if (null != this.ItemsList[i])
			{
				this.ItemCollection.AddNoUpdate(this.ItemsList[i]);
			}
		}
		this.ItemCollection.DelayUpdate();
		if (this.OpenPartCount)
		{
			this.OpenPartCount = false;
			this.ClickHelpAnim();
		}
	}

	private void LoadCailiaoList(int id)
	{
		int[] array = null;
		if (id == 0 || id == 5)
		{
			array = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLuckyGoodsIDs", ',');
		}
		else if (id == 2)
		{
			array = ConfigSystemParam.GetSystemParamIntArrayByName("ShenyoufuGoodsIDs", ',');
		}
		if (Global.Data.roleData.GoodsDataList != null)
		{
			this.ItemsList_Cailiao.RemoveRange(0, this.ItemsList_Cailiao.Count);
			if (array.Length != 0)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					if (Global.Data.roleData.GoodsDataList[i].GCount > 0)
					{
						if (Global.Data.roleData.GoodsDataList[i].Using <= 0)
						{
							if (Array.IndexOf<int>(array, Global.Data.roleData.GoodsDataList[i].GoodsID) != -1)
							{
								this.AddCailiaoIcon(Global.Data.roleData.GoodsDataList[i]);
							}
						}
					}
				}
			}
		}
		this.ShowPage_Cailiao(this.CurrentSelectedPage_Cailiao);
	}

	private void AddCailiaoIcon(GoodsData goodsData)
	{
		GGoodIcon ggoodIcon = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int categoriy = goodsXmlNodeByID.Categoriy;
			try
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 32.0;
				ggoodIcon.Height = 32.0;
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				ggoodIcon.TipType = 1;
				ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
				{
					goodsData.GoodsID,
					1,
					goodsData.Id,
					0
				});
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemCode = goodsData.GoodsID;
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.TextShadowColor = 4278190080U;
				ggoodIcon.BoxTypes = 1;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			ggoodIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp_Cailiao);
			this.ItemsList_Cailiao.Add(ggoodIcon);
		}
	}

	private void ShowPage_Cailiao(int pageIndex)
	{
		this.ItemCollection2.Clear();
		if (this.ItemsList_Cailiao.Count > this.CountOfSinglePage_Cailiao)
		{
			this.MaxPageCount_Cailiao = (this.ItemsList_Cailiao.Count - 1) / this.CountOfSinglePage_Cailiao + 1;
			this.PageHint_Cailiao.Text = StringUtil.substitute("{0}/{1}", new object[]
			{
				pageIndex + 1,
				this.MaxPageCount_Cailiao
			});
			int num = pageIndex * this.CountOfSinglePage_Cailiao;
			int num2 = num;
			while (num2 < this.ItemsList_Cailiao.Count && num2 < num + this.CountOfSinglePage_Cailiao)
			{
				this.ItemCollection2.AddNoUpdate(this.ItemsList_Cailiao[num2]);
				num2++;
			}
			this.ItemCollection2.DelayUpdate();
			if (pageIndex <= 0)
			{
				this.PrevPageIcon_Cailiao.EnableIcon = false;
			}
			else
			{
				this.PrevPageIcon_Cailiao.EnableIcon = true;
			}
			if (pageIndex >= this.MaxPageCount_Cailiao - 1)
			{
				this.NextPageIcon_Cailiao.EnableIcon = false;
			}
			else
			{
				this.NextPageIcon_Cailiao.EnableIcon = true;
			}
			this.PageHint_Cailiao.Visibility = true;
			this.NextPageIcon_Cailiao.Visibility = true;
			this.PrevPageIcon_Cailiao.Visibility = true;
		}
		else
		{
			for (int i = 0; i < this.ItemsList_Cailiao.Count; i++)
			{
				this.ItemCollection2.AddNoUpdate(this.ItemsList_Cailiao[i]);
			}
			this.ItemCollection2.DelayUpdate();
			this.PageHint_Cailiao.Visibility = false;
			this.NextPageIcon_Cailiao.Visibility = false;
			this.PrevPageIcon_Cailiao.Visibility = false;
		}
	}

	public void NextPage_Cailiao()
	{
		if (this.CurrentSelectedPage_Cailiao < this.MaxPageCount_Cailiao)
		{
			this.CurrentSelectedPage_Cailiao++;
			this.ShowPage_Cailiao(this.CurrentSelectedPage_Cailiao);
		}
	}

	public void PrevPage_Cailiao()
	{
		if (this.CurrentSelectedPage_Cailiao > 0)
		{
			this.CurrentSelectedPage_Cailiao--;
			this.ShowPage_Cailiao(this.CurrentSelectedPage_Cailiao);
		}
	}

	private void SetEffect(int effectID, int playID)
	{
		if (effectID == 1)
		{
			this.initEffectPos(this.MenuIndex, playID);
			this._LianluAnim[0].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[0], new ActiveAnimation.OnFinished(this.PlayFinished));
			if (this.MenuIndex == LianluTypes.Qianghua || this.MenuIndex == LianluTypes.Zhuijia || this.MenuIndex == LianluTypes.QianghuaChuancheng)
			{
				this._LianluAnim[2].gameObject.SetActive(true);
				this.PlayStart(this._LianluAnim[2], new ActiveAnimation.OnFinished(this.PlayFinished));
			}
			if (this.MenuIndex == LianluTypes.Qianghua)
			{
				this._LianluAnim[3].gameObject.SetActive(true);
				this.PlayStart(this._LianluAnim[3], new ActiveAnimation.OnFinished(this.PlayFinished));
			}
			else if (this.MenuIndex == LianluTypes.Zhuijia)
			{
				this._LianluAnim[4].gameObject.SetActive(true);
				this.PlayStart(this._LianluAnim[4], new ActiveAnimation.OnFinished(this.PlayFinished));
			}
			else if (this.MenuIndex == LianluTypes.ZhuangbeiJinjie)
			{
				this._HechengAnim[0].SetActive(false);
				this._HechengAnim[0].SetActive(true);
			}
			else if (this.MenuIndex == LianluTypes.ShenqiZaizao)
			{
				this.ShengqiZaizaoAnim.Clear();
				GameObject shenqiZaizaoPrefab = this.GetShenqiZaizaoPrefab();
				shenqiZaizaoPrefab.gameObject.SetActive(true);
				this.ShengqiZaizaoAnim.Add(shenqiZaizaoPrefab);
			}
		}
		else if (effectID == -1)
		{
			this._LianluAnim[1].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[1], new ActiveAnimation.OnFinished(this.PlayFinished));
		}
		else if (effectID == 5)
		{
			this._HechengAnim[1].SetActive(false);
			this._HechengAnim[1].SetActive(true);
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

	private void initEffectPos(LianluTypes tabID, int level)
	{
		int num = -148;
		if (Global.MaxForgeLevel == 20)
		{
			num = -203;
		}
		int num2 = 12;
		if (tabID == LianluTypes.Qianghua)
		{
			this._LianluAnim[2].transform.localPosition = new Vector3(-123f, 122f, 0f);
			this._LianluAnim[3].transform.localPosition = new Vector3((float)(num + (level - 1) * 21), (float)num2, 0f);
		}
		else if (tabID == LianluTypes.Zhuijia)
		{
			this._LianluAnim[2].transform.localPosition = new Vector3(-123f, 122f, 0f);
		}
		else if (tabID == LianluTypes.QianghuaChuancheng)
		{
			this._LianluAnim[2].transform.localPosition = new Vector3(125f, 120f, 0f);
		}
		else if (tabID == LianluTypes.ZhuangbeiJinjie)
		{
			this._LianluAnim[2].transform.localPosition = new Vector3(0f, 25f, 0f);
		}
		else if (tabID == LianluTypes.ShenqiZaizao)
		{
		}
	}

	public GameObject GetShenqiZaizaoPrefab()
	{
		if (this.ShenqiZaizaoPrefab == null)
		{
			this.ShenqiZaizaoPrefab = (Resources.Load(string.Format("UITeXiao/LV_11_HeCheng/Level_11_HeCheng_effect", new object[0])) as GameObject);
		}
		return SpawnManager.Instantiate(this.ShenqiZaizaoPrefab) as GameObject;
	}

	public void delEffect()
	{
	}

	public void PauseAllEffect(bool isPause)
	{
	}

	private void SetSprite(int menuIndex)
	{
	}

	public void RefreshGetAfter()
	{
	}

	private void SetLianluHelp(LianluTypes index)
	{
		if (index == LianluTypes.Qianghua)
		{
			this.lianluHelpPart.BackImgName = "lianluQianghuaHelp.png";
		}
		else if (index == LianluTypes.Zhuijia)
		{
			this.lianluHelpPart.BackImgName = "lianluZhuijiaHelp.png";
		}
		else if (index == LianluTypes.QianghuaChuancheng || index == LianluTypes.ZhuijiaChuancheng || index == LianluTypes.XilianChuancheng || index == LianluTypes.JuHunChuancheng || index == LianluTypes.FuMoChuanCheng)
		{
			this.lianluHelpPart.BackImgName = "lianluQianghuaChuanchengHelp.png";
		}
		else if (index == LianluTypes.ZhuangbeiZhuansheng)
		{
			this.lianluHelpPart.BackImgName = "lianluZhuanshengHelp.png";
		}
		else if (index == LianluTypes.ZhuanshengBoli)
		{
			this.lianluHelpPart.BackImgName = "lianluZhuanshengBoliHelp.png";
		}
		else if (index == LianluTypes.ZhuangbeiJinjie)
		{
			this.lianluHelpPart.BackImgName = "lianluZhuangbeiJinjieHelp.png";
		}
		else if (index == LianluTypes.ZhuangbeiXilian)
		{
			this.lianluHelpPart.BackImgName = "lianluZhuangbeiXilianHelp.png";
			this.StopAutoXiLian();
		}
		else if (index == LianluTypes.ShenqiZaizao)
		{
			this.lianluHelpPart.BackImgName = "lianluShenqiZaizaoHelp.png";
		}
		else if (index == LianluTypes.JuHun)
		{
			this.lianluHelpPart.BackImgName = "lianluShenqiJuHunHelp.png";
		}
		else if (index == LianluTypes.ChongWuJiNengChuanCheng)
		{
			this.lianluHelpPart.BackImgName = "JinglingJinengChuanchengHelp.png";
		}
		else if (index == LianluTypes.ZhuangBeiFuMo)
		{
			this.lianluHelpPart.BackImgName = "lianluFuMoHelp.png";
		}
		Transform transform = this.lianluHelpPart.Bak.transform;
		if (index == LianluTypes.QianghuaChuancheng || index == LianluTypes.ZhuijiaChuancheng || index == LianluTypes.XilianChuancheng || index == LianluTypes.JuHunChuancheng || index == LianluTypes.FuMoChuanCheng)
		{
			transform.localScale = new Vector3(transform.localScale.x, 622f, transform.localScale.z);
		}
		else
		{
			transform.localScale = new Vector3(transform.localScale.x, 438f, transform.localScale.z);
		}
	}

	private bool IsEnableContinueByLianlu(bool isSave)
	{
		return true;
	}

	private GIcon SelectedIcon;

	private TextBlock PageHint = new TextBlock();

	private int CountOfSinglePage_Cailiao = 8;

	private int CurrentSelectedPage_Cailiao;

	private int MaxPageCount_Cailiao;

	private GIcon NextPageIcon_Cailiao;

	private GIcon PrevPageIcon_Cailiao;

	private TextBlock PageHint_Cailiao = new TextBlock();

	private List<GGoodIcon> ItemsList_Cailiao = new List<GGoodIcon>();

	private ListBox cailiaoList = new ListBox();

	private ObservableCollection _ItemCollection2;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler CloseCallBack;

	public GButton CloseBtn;

	public UIButton HelpBtn;

	public SpriteSL LianluHelpWin;

	[HideInInspector]
	public LianluHelpPart lianluHelpPart;

	public UITab MenuTabList;

	public GButton ZhuangbeiQianghua;

	public GButton ZhuangbeiZhuijia;

	public GButton QianghuaChuanchen;

	public GButton ZhuangbeiJinjie;

	public GButton ShenqiZaizao;

	public GButton ZhuangBeiPeiYang;

	public GButton m_JuHunBtn;

	public GButton m_ChongWuChuanCheng;

	public GButton m_ZhuangBeiFuMo;

	public UIDraggablePanel m_Draggable;

	public SpringPanel m_SpringPanel;

	private LianluTypes MenuIndex = LianluTypes.None;

	private List<LianluTypes> ChuanChengIndexList;

	public Canvas partCanvas;

	private GoodsData zhuZhuangbeiData;

	private GoodsData fuZhuangbeiData;

	private int[] jinjieExceptIds;

	[HideInInspector]
	public LianluZhuangbeiQianghuaPart lianluZhuangbeiQianghuaPart;

	[HideInInspector]
	public LianluZhuangbeiZhuijiaPart lianluZhuangbeiZhuijiaPart;

	[HideInInspector]
	public LianluQianghuaChuanchenPart lianluQianghuaChuanchenPart;

	[HideInInspector]
	public LianluZhuijiaChuanchenPart lianluZhuijiaChuanchenPart;

	[HideInInspector]
	public LianluZhuangbeiZhuanShengPart lianluZhuangbeiZhuanShengPart;

	[HideInInspector]
	public LianluZhuanshengBoliPart lianluZhuanshengBoliPart;

	[HideInInspector]
	public LianluZhuangbeiJinjiePart lianluZhuangbeiJinjiePart;

	[HideInInspector]
	public LianluZhuangbeiXilianPart lianluZhuangbeiXilianPart;

	[HideInInspector]
	public LianluTianshengXilianPart lianluTianshengXilianPart;

	[HideInInspector]
	public LianluShengzhuangLianhuaPart lianluShengzhuangLianhuaPart;

	[HideInInspector]
	public LianluQianghuaFenliPart lianluQianghuaFenliPart;

	[HideInInspector]
	public LianluJinglianPart lianluJinglianPart;

	[HideInInspector]
	public LianluShenqiZaizaoPart lianluShenqiZaizaoPart;

	[HideInInspector]
	public LianLuJuHunPart juHunPart;

	[HideInInspector]
	public JingLingJiNengChuanchengPart jingLingJiNengChuanChengPart;

	[HideInInspector]
	public LianluZhuangbeiFuMoPart zhuanBeiFuMoPart;

	public UITab EquipTabList;

	public GButton ShenshangZhuangbei;

	public GButton BaoguoZhuangbei;

	public GButton ShuXing;

	private Vector3 ShengShangZhuangbeiPos;

	private Vector3 BaoguoZhuangbeiPos;

	public UIDraggablePanel equipDraggablePanel;

	public Transform equipPanelTran;

	public UIPanel equipPanel;

	private Vector3 equipPanelPos;

	private Vector4 equipPanelClipRange;

	public UIPanel m_PanelHorse;

	public GCheckBox m_CheckRole;

	public GCheckBox m_CheckHorse;

	public UISprite m_SpGunDongKuang;

	public UIScrollBar m_Scroll;

	private bool m_StarBool;

	private int m_PageCount;

	private List<GButton> m_ListBtns;

	public Animation[] _LianluAnim;

	public GameObject[] _HechengAnim;

	public SpriteSL ShengqiZaizaoAnim;

	public ListBox equipList;

	private List<LianluEquipItem> ItemsList;

	private LianLuJuHunShuXingPart shuXing;

	private ObservableCollection _ItemCollection;

	public GameObject tipQiangHuaIcon;

	public GameObject tipZhuiJiaIcon;

	public GameObject tipPeiYangIcon;

	private int preTabIndex;

	private Dictionary<int, bool> dicLianLuOpen;

	private int[] categoriyIndexArr;

	private Dictionary<int, int> categoriyIndexDict;

	private LianluEquipItem temp;

	private bool OpenPartCount;

	private GameObject ShenqiZaizaoPrefab;

	private string[] lianluAtlasName;
}
