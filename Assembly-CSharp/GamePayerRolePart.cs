using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GamePayerRolePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.renwuBtn.Text = Global.GetLang("属 性");
		this.btnHuobi.Text = Global.GetLang("货 币");
		this.baoguoBtn.Text = Global.GetLang("背 包");
		this.baoguoChongShengBtn.Text = Global.GetLang("重生背包");
		this.jinengBtn.Text = Global.GetLang("技 能");
		this.shizhuangBtn.Text = Global.GetLang("称 号");
		this.chibangBtn.Text = Global.GetLang("翅 膀");
		this.m_BtnXingHun.Text = Global.GetLang("星 魂");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.renwuBtn)
		{
			this.renwuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_ShuXing))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_ShuXing, true))
				{
					return;
				}
				this.needChangeLayer = true;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_ShuXing);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_ShuXing;
			};
		}
		if (null != this.baoguoBtn)
		{
			this.baoguoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_BeiBao;
			};
		}
		if (null != this.baoguoChongShengBtn)
		{
			if (!ChongShengData.IsChongShengBgOpen())
			{
				this.baoguoChongShengBtn.gameObject.SetActive(false);
			}
			this.baoguoChongShengBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng;
			};
		}
		if (null != this.jinengBtn)
		{
			this.jinengBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_JiNeng))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_JiNeng, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_JiNeng);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_JiNeng;
			};
		}
		if (null != this.chibangBtn)
		{
			this.chibangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_ChiBang))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_ChiBang, true))
				{
					return;
				}
				this.needChangeLayer = true;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_ChiBang);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_ChiBang;
			};
		}
		if (null != this.m_BtnXingHun)
		{
			this.m_BtnXingHun.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_XingHun))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_XingHun, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_XingHun);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_XingHun;
			};
		}
		if (null != this.btnHuobi)
		{
			this.btnHuobi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_HuoBi))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_HuoBi, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_HuoBi);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_HuoBi;
			};
		}
		if (null != this.shizhuangBtn)
		{
			this.initPosition = this.shizhuangBtn.transform.localPosition;
			this.shizhuangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.BtnIndex(GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title))
				{
					return;
				}
				if (!this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title, true))
				{
					return;
				}
				this.needChangeLayer = false;
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title);
				this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title;
			};
		}
		this.returnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Inactived, -1);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		if (PlatSDKMgr.PlatName == "I4" || PlatSDKMgr.PlatName == "PP")
		{
			if (this.btnShare.gameObject.activeSelf)
			{
				this.btnShare.gameObject.SetActive(false);
			}
		}
		else
		{
			this.btnShare.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1,
						IDType = ((!this.needChangeLayer) ? 0 : 1)
					});
				}
			};
		}
		this.btnShare.gameObject.SetActive(PlatSDKMgr.NeedShowShareButton());
		ActivityTipManager.RegActivityTipItem(30000, delegate(int s, ActivityTipItem e)
		{
			if (this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_BeiBao, false))
			{
				NGUITools.SetActive(this.tipIcon.gameObject, e.IsActive);
			}
		});
		GameInstance.Game.SpriteGetSkillInfoCmd();
		ActivityTipManager.RegActivityTipItem(32001, delegate(int s, ActivityTipItem e)
		{
			if (this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_JiNeng, false))
			{
				NGUITools.SetActive(this.tipJiNengIcon.gameObject, e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(32002, delegate(int s, ActivityTipItem e)
		{
			if (this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_ChiBang, false) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartChiBang))
			{
				NGUITools.SetActive(this.tipJiChiBangIcon.gameObject, e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(32003, delegate(int s, ActivityTipItem e)
		{
			if (this.CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID.GamePayerRolePart_XingHun, false) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartXingZuo))
			{
				NGUITools.SetActive(this.tipJiXingHunIcon.gameObject, e.IsActive);
			}
		});
		if (Context.IsHaiwai)
		{
			this.btnShare.gameObject.SetActive(false);
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(30000, null);
		ActivityTipManager.UnRegActivityTipItem(32001, null);
		ActivityTipManager.UnRegActivityTipItem(32002, null);
		ActivityTipManager.UnRegActivityTipItem(32003, null);
	}

	private bool CheckTheMapCanUsePartInRebornMap(GamePayerRolePart_PartID type, bool Tip = true)
	{
		if (SceneUIClasses.RebornMap.IsTheScene() && type != GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng)
		{
			if (Tip)
			{
				Super.HintMainText(Global.GetLang("当前地图无法使用此功能"), 10, 3);
			}
			return false;
		}
		return true;
	}

	private void SetPart(GamePayerRolePart_PartID type)
	{
		this.SelectChildWindows(type);
		switch (type)
		{
		case GamePayerRolePart_PartID.GamePayerRolePart_ShuXing:
			this.ShowRenwu();
			this.SetBtnStat(this.renwuBtn);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_HuoBi:
			this.ShowHuobiPart();
			this.SetBtnStat(this.btnHuobi);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_BeiBao:
			base.StartCoroutine<bool>(this.ShowBaoGuo(false, 0));
			this.SetBtnStat(this.baoguoBtn);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_JiNeng:
			this.ShowJiNeng();
			this.SetBtnStat(this.jinengBtn);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_ChiBang:
			this.ShowChiBang();
			this.SetBtnStat(this.chibangBtn);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_XingHun:
			this.ShowXingHun();
			this.SetBtnStat(this.m_BtnXingHun);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title:
			this.ShowShiZhuang();
			this.SetBtnStat(this.shizhuangBtn);
			this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title;
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_ChuShou:
			base.StartCoroutine<bool>(this.ShowBaoGuo(false, 0));
			this.SetBtnStat(this.baoguoBtn);
			this.m_nBtnIndex = GamePayerRolePart_PartID.GamePayerRolePart_BeiBao;
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_JingLIngHuiShou:
			base.StartCoroutine<bool>(this.ShowBaoGuo(true, 1));
			this.SetBtnStat(this.baoguoBtn);
			SystemHelpMgr.OnAction(UIObjIDs.GamePayerRolePart, HelpStateEvents.Actived, -1);
			break;
		case GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng:
			base.StartCoroutine<bool>(this.ShowChongShengBaoGuo(false, 0));
			this.SetBtnStat(this.baoguoChongShengBtn);
			break;
		}
	}

	public void SelectChildWindows(GamePayerRolePart_PartID Index)
	{
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_BeiBao && Index != GamePayerRolePart_PartID.GamePayerRolePart_ChuShou)
		{
			if (null != this.zhuangbeiPart)
			{
				this.zhuangbeiPart.Visibility = false;
			}
			if (null != this.parcelPart)
			{
				this.parcelPart.Visibility = false;
			}
			if (null != this.chushou)
			{
				this.chushou.ClearBgShouStat();
				this.chushou.Visibility = false;
			}
			if (Super._ParcelPart != null)
			{
				Super._ParcelPart.Visibility = false;
			}
			if (Super._ParcelRebornPart != null)
			{
				Super._ParcelRebornPart.Visibility = false;
			}
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng)
		{
			if (null != this.zhuangbeiChongShengPart)
			{
				this.zhuangbeiChongShengPart.Visibility = false;
			}
			if (null != this.parcelChongShengPart)
			{
				this.parcelChongShengPart.Visibility = false;
			}
			if (null != this.chongShengChuShou)
			{
				this.chongShengChuShou.ClearBgShouStat();
				this.chongShengChuShou.Visibility = false;
			}
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_ChiBang && null != this.chibangPart)
		{
			this.chibangPart.Visibility = false;
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_HuoBi && null != this.huobiPart)
		{
			this.huobiPart.Visibility = false;
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_JiNeng && null != this.skillPart)
		{
			this.skillPart.Visibility = false;
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_ShiZhuang_Title && null != this.shizhuangPart)
		{
			this.shizhuangPart.Visibility = false;
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_ShuXing && null != this.roleAttributePart)
		{
			this.roleAttributePart.Visibility = false;
		}
		if (Index != GamePayerRolePart_PartID.GamePayerRolePart_XingHun && null != this.m_XingHunPart)
		{
			this.m_XingHunPart.Visibility = false;
		}
	}

	private bool BtnIndex(GamePayerRolePart_PartID nIndex)
	{
		return nIndex == this.m_nBtnIndex;
	}

	public void InitPartData(GamePayerRolePart_PartID type = GamePayerRolePart_PartID.GamePayerRolePart_ShuXing)
	{
		bool flag = GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartChiBang);
		this.chibangBtn.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.GamePayerRolePartChiBang, "roleTab_normal", null, null);
		bool flag2 = GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartXingZuo);
		this.m_BtnXingHun.gameObject.AddComponent<TabButtonOpen>().SetTabState(GongNengIDs.GamePayerRolePartXingZuo, "roleTab_normal", null, null);
		this.renwuBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.baoguoBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.baoguoChongShengBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.jinengBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.chibangBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_BtnXingHun.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.btnHuobi.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.shizhuangBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.shizhuangBtn.gameObject.SetActive(false);
		this.gridBtns.Reposition();
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("ShiZhuangXianShi", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			int num = systemParamByName.SafeToInt32(0);
			if (num == 1)
			{
				this.GetTitleList();
			}
		}
		Vector3 localPosition = this.shizhuangBtn.transform.localPosition;
		this._ActivityTipIcons[6].transform.localPosition = new Vector3(this._ActivityTipIcons[6].transform.localPosition.x, localPosition.y, 0f);
		this.SetPart(type);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				GGoodIcon target;
				if (null != Super._ParcelPart && null != (target = Super._ParcelPart.LocationGoodsIcon(7004, true)))
				{
					SystemHelpPart.SetMask(target, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
				}
			}
			else if (id == 198)
			{
				if (null != this.chibangBtn)
				{
					SystemHelpPart.SetMask(this.chibangBtn, default(Vector4));
				}
			}
			else if (id == 402)
			{
				if (null != this.m_BtnXingHun)
				{
					SystemHelpPart.SetMask(this.m_BtnXingHun, default(Vector4));
				}
			}
			else if (id == 1000)
			{
				if (null != this.chibangPart)
				{
					this.chibangPart.ShowHelpAnim();
				}
			}
			else if (id == 1001)
			{
				if (null != this.chibangPart)
				{
					SystemHelpPart.SetMask(this.chibangPart.peidaiBtn, default(Vector4));
				}
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.returnBtn, default(Vector4));
			}
			else if (id == 705)
			{
				if (null != this.chushou)
				{
					SystemHelpPart.SetMask(this.chushou.zicheckBox, default(Vector4));
				}
			}
			else if (id == 706)
			{
				if (null != this.chushou)
				{
					SystemHelpPart.SetMask(this.chushou.baicheckBox, default(Vector4));
				}
			}
			else if (id == 707)
			{
				if (null != this.chushou)
				{
					SystemHelpPart.SetMask(this.chushou.ChuShouBtn, default(Vector4));
				}
			}
			else if (id == 715 && null != this.chushou)
			{
				SystemHelpPart.SetMask(this.chushou.baicheckBox, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void ShowXingHun()
	{
		this.SetBtnStat(this.m_BtnXingHun);
		if (null == this.m_XingHunPart)
		{
			this.m_XingHunPart = U3DUtils.NEW<XingHunPart>();
			this.ShowObjCanvs1.Children.Add(this.m_XingHunPart);
		}
		if (null != this.m_XingHunPart)
		{
			this.m_XingHunPart.gameObject.SetActive(true);
		}
	}

	private void ShowRenwu()
	{
		if (this.roleAttributePart == null)
		{
			this.roleAttributePart = U3DUtils.NEW<RoleAttributePart>();
			this.roleAttributePart.InitPartData(true);
			this.roleAttributePart.GetNewData();
			this.roleAttributePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 1
					});
				}
			};
			this.ShowObjCanvs1.Children.Add(this.roleAttributePart);
		}
		if (!this.roleAttributePart.Visibility)
		{
			this.roleAttributePart.Visibility = true;
		}
	}

	private IEnumerator ShowBaoGuo(bool isshowchushou = false, int HuiShouType = 0)
	{
		this.LoadZhuangBei();
		this.zhuangbeiPart.InitPartData();
		yield return null;
		base.StartCoroutine<bool>(this.LoadBaoGuo(isshowchushou, HuiShouType));
		if (isshowchushou)
		{
		}
		yield break;
	}

	private IEnumerator ShowChongShengBaoGuo(bool isshowchushou = false, int HuiShouType = 0)
	{
		this.LoadChongShengZhuangBei();
		this.zhuangbeiChongShengPart.InitPartData();
		yield return null;
		base.StartCoroutine<bool>(this.LoadChongShengBaoGuo(isshowchushou, HuiShouType));
		if (isshowchushou)
		{
		}
		yield break;
	}

	private void ShowJiNeng()
	{
		if (this.skillPart == null)
		{
			this.skillPart = U3DUtils.NEW<SkillPart>();
			this.skillPart.InitPartData();
			this.ShowObjCanvs1.Children.Add(this.skillPart);
		}
		if (!this.skillPart.Visibility)
		{
			this.skillPart.Visibility = true;
			this.skillPart.InitPartData();
		}
		this.skillPart.Anim[0].SetActive(false);
	}

	private void ShowHuobiPart()
	{
		this.SetBtnStat(this.btnHuobi);
		if (this.huobiPart == null)
		{
			this.huobiPart = U3DUtils.NEW<HuobiPart>();
			this.huobiPart.InitPartData();
			this.huobiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(s, e);
				}
			};
		}
		if (!this.huobiPart.Visibility)
		{
			this.huobiPart.Visibility = true;
		}
		this.ShowObjCanvs1.Children.Add(this.huobiPart);
	}

	private void ShowChiBang()
	{
		if (this.chibangPart == null)
		{
			this.chibangPart = U3DUtils.NEW<ChiBangPart>();
			this.chibangPart.callback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1
					});
				}
			};
		}
		if (!this.chibangPart.Visibility)
		{
			this.chibangPart.Visibility = true;
		}
		this.ShowObjCanvs1.Children.Add(this.chibangPart);
		SystemHelpMgr.OnAction(UIObjIDs.ChiBangPart, HelpStateEvents.Actived, -1);
		this.chibangPart.checkboxAnim.SetActive(false);
		GameInstance.Game.SpriteQueryWingData();
	}

	private void ShowShiZhuang()
	{
		if (this.shizhuangPart == null)
		{
			this.shizhuangPart = U3DUtils.NEW<RoleShiZhuangPart>();
			this.shizhuangPart.InitRoleDressData();
		}
		if (!this.shizhuangPart.Visibility)
		{
			this.shizhuangPart.Visibility = true;
		}
		this.ShowObjCanvs1.Children.Add(this.shizhuangPart);
	}

	private void GetTitleList()
	{
		GameInstance.Game.GetGetTeShuChengHaoData();
	}

	public void SetFashionTitleState(List<GoodsData> list_goods)
	{
		this.shizhuangBtn.gameObject.SetActive(this.IsTitleButtonEnabled(list_goods));
		this.gridBtns.Reposition();
		if (this.IsTitleButtonEnabled(list_goods))
		{
			if (Global.Data.teshuTitileTipOne == 0)
			{
				if (Global.Data.teshuTitileBufferLst.Count > 0 && Global.Data.BufferDataId <= 0)
				{
					Global.Data.teshuTitileTipOne = 1;
				}
				else
				{
					Global.Data.teshuTitileTipOne = 2;
				}
			}
			if (Global.Data.teshuTitileBufferLst.Count > 0 && Global.Data.BufferDataId <= 0 && Global.Data.teshuTitileTipOne == 1)
			{
				this._ActivityTipIcons[6].SetActive(true);
				Vector3 localPosition = this.shizhuangBtn.transform.localPosition;
				this._ActivityTipIcons[6].transform.localPosition = new Vector3(this._ActivityTipIcons[6].transform.localPosition.x, localPosition.y, 0f);
			}
		}
	}

	private bool IsTitleButtonEnabled(List<GoodsData> list_goods)
	{
		if (Global.Data.teshuTitileBufferLst.Count > 0)
		{
			return true;
		}
		if (list_goods == null || list_goods.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < list_goods.Count; i++)
		{
			GoodsData goodsData = list_goods[i];
			int dressTabIDByGoodsID = Global.GetDressTabIDByGoodsID(goodsData.GoodsID);
			if (dressTabIDByGoodsID == 2)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator LoadBaoGuo(bool isshowchushou, int chuShouType = 0)
	{
		if (Super._ParcelPart != null)
		{
			Super._ParcelPart.iBaoGuoMode = 0;
			Super._ParcelPart.Visibility = true;
			Super._ParcelPart.Huishou(true);
			Super._ParcelPart.JingLingHuiShou(true);
			if (!Super._ParcelPart.IsItemFinished)
			{
				Super._ParcelPart.CheckInitedPartData();
			}
		}
		else
		{
			this.parcelPart = U3DUtils.NEW<ParcelPart>();
			Super._ParcelPart = this.parcelPart;
			Super._ParcelPart.iBaoGuoMode = 0;
			this.ShowObjCanvs2.Children.Add(Super._ParcelPart);
			yield return null;
			Super._ParcelPart.InitPartData();
		}
		SystemHelpMgr.OnAction(UIObjIDs.BagPart, HelpStateEvents.Clicked, 2);
		Super._ParcelPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("整理背包功能开发中..."), 0, -1, -1, 0);
			}
			else if (e.ID == 10)
			{
				this.LoadYiJianChuShou(HuiShouType.Equipment);
			}
			else if (e.ID == 11)
			{
				this.LoadYiJianChuShou(HuiShouType.Pet);
			}
			else if (e.ID == 101)
			{
				this.LoadZhuangBei();
			}
			else if (e.ID == 102)
			{
				this.LoadYiJianChuShou(HuiShouType.horse);
			}
			else if (e.ID == -1 && e.IDType == 1)
			{
				if (e.Tag != null)
				{
					GoodsData gd = e.Tag as GoodsData;
					if (this.chushou != null)
					{
						this.chushou.AddChuShouGoods(gd);
					}
				}
			}
			else if (e.ID == -1 && e.IDType == 5 && e.Tag != null)
			{
				GoodsData goodsData = e.Tag as GoodsData;
				int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				if (this.chushou.huiShouType == HuiShouType.Equipment)
				{
					if (goodsCatetoriy == 9 || goodsCatetoriy == 10 || (40 <= goodsCatetoriy && 45 >= goodsCatetoriy) || goodsCatetoriy == 340)
					{
						Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
						return true;
					}
				}
				else
				{
					if (this.chushou.huiShouType == HuiShouType.Pet && goodsCatetoriy != 9 && goodsCatetoriy != 10)
					{
						Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
						return true;
					}
					if (this.chushou.huiShouType == HuiShouType.horse && goodsCatetoriy != 340)
					{
						if (40 > goodsCatetoriy || 45 < goodsCatetoriy)
						{
							Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
							return true;
						}
					}
				}
				if (this.chushou != null)
				{
					this.chushou.AddHuiShouGoods(goodsData);
				}
			}
			return true;
		};
		Super._ParcelPart.GuanLianItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			int idtype = e.IDType;
			if (idtype == 190)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowLianluWindow(0, 0);
			}
			else if (idtype == 250)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowLianluWindow(1, 0);
			}
			else if (idtype == 230)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowZaDanWindow(false);
			}
			else if (idtype == 260)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowSyntesizeWindow(false, 101);
			}
			else if (idtype == 270)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowSyntesizeWindow(false, 1000);
			}
			else if (idtype == 280)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowSyntesizeWindow(false, 201);
			}
			else if (idtype == 281)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowSyntesizeWindow(false, 401);
			}
			else if (idtype == 290)
			{
				(Super.GData.GlobalPlayZone as PlayZone).OpenTuJian();
			}
			else if (idtype == 300)
			{
				this.SetPart(GamePayerRolePart_PartID.GamePayerRolePart_ChiBang);
			}
			else if (idtype == 310)
			{
				(Super.GData.GlobalPlayZone as PlayZone).ShowLianluWindow(9, 0);
			}
			else if (idtype == 320)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1351
				});
			}
			else if (idtype == 321)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1352
				});
			}
		};
		if (isshowchushou)
		{
			if (chuShouType == 0)
			{
				Super._ParcelPart.Huishou(false);
			}
			else
			{
				Super._ParcelPart.JingLingHuiShou(false);
			}
		}
		yield break;
	}

	private IEnumerator LoadChongShengBaoGuo(bool isshowchushou, int chuShouType = 0)
	{
		if (Super._ParcelRebornPart != null)
		{
			Super._ParcelRebornPart.iBaoGuoMode = 0;
			Super._ParcelRebornPart.Visibility = true;
			Super._ParcelRebornPart.Huishou(true);
			Super._ParcelRebornPart.JingLingHuiShou(true);
			if (!Super._ParcelRebornPart.IsItemFinished)
			{
				Super._ParcelRebornPart.CheckInitedPartData();
			}
		}
		else
		{
			this.parcelChongShengPart = U3DUtils.NEW<ParcelPart>();
			this.parcelChongShengPart.IsRebornParcel = true;
			Super._ParcelRebornPart = this.parcelChongShengPart;
			this.parcelChongShengPart.iBaoGuoMode = 0;
			this.ShowObjCanvs2.Children.Add(this.parcelChongShengPart);
			yield return null;
			this.parcelChongShengPart.InitPartData();
		}
		SystemHelpMgr.OnAction(UIObjIDs.BagPart, HelpStateEvents.Clicked, 2);
		this.parcelChongShengPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("整理背包功能开发中..."), 0, -1, -1, 0);
			}
			else if (e.ID == 10)
			{
				this.LoadChongShengChuShou(HuiShouType.EquipmentChongSheng);
			}
			else if (e.ID == 101)
			{
				this.LoadChongShengZhuangBei();
			}
			else if (e.ID == -1 && e.IDType == 5 && e.Tag != null)
			{
				GoodsData goodsData = e.Tag as GoodsData;
				int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				if (this.chongShengChuShou.huiShouType == HuiShouType.EquipmentChongSheng)
				{
					if (!Global.IsRebornEquip(goodsCatetoriy))
					{
						Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
						return true;
					}
				}
				else if (this.chongShengChuShou.huiShouType == HuiShouType.BaoShiChongSheng)
				{
					if (!Global.IsRebornBaoShi(goodsCatetoriy))
					{
						Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
						return true;
					}
				}
				else if (this.chongShengChuShou.huiShouType == HuiShouType.XuanCaiChongSheng && !Global.IsRebornXuanCai(goodsCatetoriy))
				{
					Super.HintMainText(Global.GetLang("该物品类型不能放入"), 10, 3);
					return true;
				}
				if (this.chongShengChuShou != null)
				{
					this.chongShengChuShou.AddChuShouGoods(goodsData);
				}
			}
			return true;
		};
		if (isshowchushou)
		{
			if (chuShouType == 0)
			{
				Super._ParcelRebornPart.Huishou(false);
			}
			else
			{
				Super._ParcelRebornPart.JingLingHuiShou(false);
			}
		}
		yield break;
	}

	private void LoadZhuangBei()
	{
		if (this.chushou != null)
		{
			this.chushou.ClearBgShouStat();
			this.chushou.Visibility = false;
		}
		if (this.zhuangbeiPart == null)
		{
			this.zhuangbeiPart = U3DUtils.NEW<ZhuangBeiPart>();
			this.zhuangbeiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -10)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1
					});
				}
				return true;
			};
			this.ShowObjCanvs1.Children.Add(this.zhuangbeiPart);
		}
		if (!this.zhuangbeiPart.Visibility)
		{
			this.zhuangbeiPart.Visibility = true;
		}
	}

	private void LoadChongShengZhuangBei()
	{
		if (this.chongShengChuShou != null)
		{
			this.chongShengChuShou.ClearBgShouStat();
			this.chongShengChuShou.Visibility = false;
		}
		if (this.zhuangbeiChongShengPart == null)
		{
			this.zhuangbeiChongShengPart = U3DUtils.NEW<ZhuangBeiChongShengPart>();
			this.zhuangbeiChongShengPart.InitPartData();
			this.zhuangbeiChongShengPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -10)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1
					});
				}
				return true;
			};
			this.ShowObjCanvs1.Children.Add(this.zhuangbeiChongShengPart);
		}
		if (!this.zhuangbeiChongShengPart.Visibility)
		{
			this.zhuangbeiChongShengPart.Visibility = true;
		}
	}

	private void LoadYiJianChuShou(HuiShouType huiShouType)
	{
		if (this.zhuangbeiPart != null)
		{
			this.zhuangbeiPart.Visibility = false;
		}
		if (this.chushou == null)
		{
			this.chushou = U3DUtils.NEW<YijianChushouPart>();
			this.chushou.huiShouType = huiShouType;
			this.chushou.InitPartData();
			this.chushou.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -1)
				{
					base.StartCoroutine<bool>(this.ShowBaoGuo(false, 0));
				}
				else if (e.ID == -2)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 2
					});
				}
				else if (e.ID == -3)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 3
					});
				}
				else if (e.ID == -4)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 4
					});
				}
			};
			this.ShowObjCanvs1.Children.Add(this.chushou);
		}
		if (!this.chushou.Visibility)
		{
			this.chushou.Visibility = true;
			this.chushou.huiShouType = huiShouType;
			this.chushou.InitPartData();
			this.chushou.HuiShouBtnOnClick(huiShouType, false);
		}
	}

	private void LoadChongShengChuShou(HuiShouType huiShouType)
	{
		if (this.zhuangbeiChongShengPart != null)
		{
			this.zhuangbeiChongShengPart.Visibility = false;
		}
		if (this.chongShengChuShou == null)
		{
			this.chongShengChuShou = U3DUtils.NEW<ChongShengChushouPart>();
			this.chongShengChuShou.parcelChongShengPart = this.parcelChongShengPart;
			this.chongShengChuShou.huiShouType = huiShouType;
			this.chongShengChuShou.InitPartData();
			this.chongShengChuShou.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -1)
				{
					base.StartCoroutine<bool>(this.ShowChongShengBaoGuo(false, 0));
				}
				else if (e.ID == -2)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 2
					});
				}
				else if (e.ID == -3)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 3
					});
				}
				else if (e.ID == -4)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = -1,
						IDType = 4
					});
				}
			};
			this.ShowObjCanvs1.Children.Add(this.chongShengChuShou);
		}
		if (!this.chongShengChuShou.Visibility)
		{
			this.chongShengChuShou.Visibility = true;
			this.chongShengChuShou.huiShouType = huiShouType;
			this.chongShengChuShou.InitPartData();
			this.chongShengChuShou.HuiShouBtnOnClick(huiShouType, false);
		}
	}

	private void ClearCanvas1()
	{
		GameObject gameObject = null;
		this.objList.TryGetValue(1, ref gameObject);
		if (gameObject != null)
		{
			this.ShowObjCanvs1.Children.Remove(gameObject.gameObject, true);
			this.objList.Remove(1);
		}
	}

	private void ClearCanvas2()
	{
		GameObject gameObject = null;
		this.objList.TryGetValue(2, ref gameObject);
		if (gameObject != null)
		{
			this.ShowObjCanvs2.Children.Remove(gameObject.gameObject, true);
			this.objList.Remove(2);
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16766048U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16766048U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	public void CloseXingHun()
	{
		if (null != this.m_XingHunPart)
		{
			this.ShowObjCanvs1.Children.Remove(this.m_XingHunPart, true);
			this.m_XingHunPart = null;
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
	}

	public void CleanUpChildWindows()
	{
	}

	public Canvas ShowObjCanvs1;

	public Canvas ShowObjCanvs2;

	public UIGrid gridBtns;

	public GButton renwuBtn;

	public GButton baoguoBtn;

	public GButton baoguoChongShengBtn;

	public GButton jinengBtn;

	public GButton chibangBtn;

	public GButton m_BtnXingHun;

	public GButton btnShare;

	public GButton btnHuobi;

	public GButton shizhuangBtn;

	public GButton returnBtn;

	public GameObject[] _ActivityTipIcons;

	public RoleAttributePart roleAttributePart;

	public HuobiPart huobiPart;

	public ChiBangPart chibangPart;

	public ZhuangBeiPart zhuangbeiPart;

	public ZhuangBeiChongShengPart zhuangbeiChongShengPart;

	public ParcelPart parcelPart;

	public ParcelPart parcelChongShengPart;

	public YijianChushouPart chushou;

	public ChongShengChushouPart chongShengChuShou;

	public SkillPart skillPart;

	public XingHunPart m_XingHunPart;

	public RoleShiZhuangPart shizhuangPart;

	private GButton tempBtn;

	private Dictionary<int, GameObject> objList = new Dictionary<int, GameObject>();

	private bool needChangeLayer;

	private GamePayerRolePart_PartID m_nBtnIndex;

	public TextBlock titleTxt;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public GameObject tipIcon;

	public GameObject tipJiNengIcon;

	public GameObject tipJiChiBangIcon;

	public GameObject tipJiXingHunIcon;

	private Vector3 initPosition = Vector3.zero;
}
