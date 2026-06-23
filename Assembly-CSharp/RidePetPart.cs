using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RidePetPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		this.RefreshBeiBaoNumber();
		this.InitOclick();
		this.SenData();
		this.RefreshRoleMoney();
	}

	private void InitText()
	{
		this.m_BtnZuoQi.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("坐骑")
		});
		this.m_BtnZhuangBei.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("装备")
		});
		this.m_BtnTuJian.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("图鉴")
		});
		this.m_BtnFashion.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("时装")
		});
		this.m_LabCaiLiao.text = "0";
	}

	private void InitOclick()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.CloseRidePetPartWindow();
		};
		bool flag = false;
		foreach (KeyValuePair<int, bool> keyValuePair in Global.DicHorseEquipOpen)
		{
			if (keyValuePair.Value)
			{
				flag = true;
				break;
			}
		}
		this.m_BtnZuoQi.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.BtnOclick(RidePetUIType.ZuoQi);
		};
		if (flag)
		{
			this.m_BtnZhuangBei.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.BtnOclick(RidePetUIType.ZhuangBei);
			};
			this.m_BtnZhuangBei.gameObject.SetActive(true);
			this.m_BtnZhuangBei.transform.localPosition = new Vector3(199f, -26f, 0f);
			this.m_BtnTuJian.transform.localPosition = new Vector3(333f, -26f, 0f);
		}
		else
		{
			this.m_BtnZhuangBei.gameObject.SetActive(false);
			this.m_BtnTuJian.transform.localPosition = new Vector3(199f, -26f, 0f);
		}
		this.m_BtnTuJian.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.BtnOclick(RidePetUIType.TuJian);
		};
		this.m_BtnFashion.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.BtnOclick(RidePetUIType.Fashion);
		};
		this.m_BtnWindowColse.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_GameWindow.SetActive(false);
		};
	}

	public void InitValue(RidePetUIType page)
	{
		if (Global.Data.roleData.MountEquipList != null && 0 < Global.Data.roleData.MountEquipList.Count)
		{
			this.BtnOclick(page);
		}
		else if (0 < Global.GetRoleHorseEquipGoodsDataList(Global.Data.RoleID, 1).Count)
		{
			this.BtnOclick(page);
		}
		else if (0 < Global.GetRoleHorseFashionList(0).Count)
		{
			this.BtnOclick(page);
		}
		else
		{
			this.BtnOclick(RidePetUIType.TuJian);
		}
	}

	private void HideAllView()
	{
		if (null != this.m_RidePetZuoQiPart)
		{
			this.m_RidePetZuoQiPart.gameObject.SetActive(false);
		}
		if (null != this.m_RidePetTuJianPart)
		{
			this.m_RidePetTuJianPart.gameObject.SetActive(false);
		}
		if (null != this.m_RideZhuangBeiPart)
		{
			this.m_RideZhuangBeiPart.gameObject.SetActive(false);
		}
		if (null != this.mRideFashionPart)
		{
			this.mRideFashionPart.gameObject.SetActive(false);
		}
	}

	private void BtnOclick(RidePetUIType type)
	{
		this.m_BtnZuoQi.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("坐骑")
		});
		this.m_BtnZuoQi.transform.localPosition = new Vector3(this.m_BtnZuoQi.transform.localPosition.x, this.m_BtnZuoQi.transform.localPosition.y, 0f);
		this.m_BtnZuoQi.normalSprite = "teamTab_normal";
		this.m_BtnZuoQi.hoverSprite = "teamTab_hover";
		this.m_BtnZuoQi.target.spriteName = "teamTab_normal";
		this.m_BtnZhuangBei.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("装备")
		});
		this.m_BtnZhuangBei.transform.localPosition = new Vector3(this.m_BtnZhuangBei.transform.localPosition.x, this.m_BtnZhuangBei.transform.localPosition.y, 0f);
		this.m_BtnZhuangBei.normalSprite = "teamTab_normal";
		this.m_BtnZhuangBei.target.spriteName = "teamTab_normal";
		this.m_BtnZhuangBei.hoverSprite = "teamTab_hover";
		this.m_BtnTuJian.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("图鉴")
		});
		this.m_BtnTuJian.transform.localPosition = new Vector3(this.m_BtnTuJian.transform.localPosition.x, this.m_BtnTuJian.transform.localPosition.y, 0f);
		this.m_BtnTuJian.normalSprite = "teamTab_normal";
		this.m_BtnTuJian.target.spriteName = "teamTab_normal";
		this.m_BtnTuJian.hoverSprite = "teamTab_hover";
		if (ConfigSystemParam.GetSystemParamIntByName("HorseFashionOpen") == 1L)
		{
			this.m_BtnFashion.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.GetLang("时装")
			});
			this.m_BtnFashion.transform.localPosition = new Vector3(this.m_BtnFashion.transform.localPosition.x, this.m_BtnFashion.transform.localPosition.y, 0f);
			this.m_BtnFashion.normalSprite = "teamTab_normal";
			this.m_BtnFashion.target.spriteName = "teamTab_normal";
			this.m_BtnFashion.hoverSprite = "teamTab_hover";
		}
		else
		{
			this.m_BtnFashion.gameObject.SetActive(false);
		}
		if (type == RidePetUIType.ZuoQi)
		{
			this.HideAllView();
			this.m_BtnZuoQi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("坐骑")
			});
			this.m_BtnZuoQi.transform.localPosition = new Vector3(this.m_BtnZuoQi.transform.localPosition.x, this.m_BtnZuoQi.transform.localPosition.y, -0.3f);
			this.m_BtnZuoQi.normalSprite = "teamTab_hover";
			this.m_BtnZuoQi.target.spriteName = "teamTab_hover";
			if (this.m_RidePetZuoQiPart == null)
			{
				this.m_RidePetZuoQiPart = U3DUtils.NEW<RidePetZuoQiPart>();
				this.m_RidePetZuoQiPart.transform.SetParent(this.m_GameParent.transform, false);
				this.m_RidePetZuoQiPart.RefreshData();
				this.m_RidePetZuoQiPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (s.ID == 10 && this.Hander != null)
					{
						this.Hander(e, s);
					}
				};
			}
			else
			{
				this.m_RidePetZuoQiPart.gameObject.SetActive(true);
				this.m_RidePetZuoQiPart.RefreshData();
			}
		}
		else if (type == RidePetUIType.ZhuangBei)
		{
			this.HideAllView();
			this.m_BtnZhuangBei.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("装备")
			});
			this.m_BtnZhuangBei.transform.localPosition = new Vector3(this.m_BtnZhuangBei.transform.localPosition.x, this.m_BtnZhuangBei.transform.localPosition.y, -0.3f);
			this.m_BtnZhuangBei.normalSprite = "teamTab_hover";
			this.m_BtnZhuangBei.target.spriteName = "teamTab_hover";
			if (this.m_RideZhuangBeiPart == null)
			{
				this.m_RideZhuangBeiPart = U3DUtils.NEW<RideZhuangBeiPart>();
				this.m_RideZhuangBeiPart.transform.SetParent(this.m_GameParent.transform, false);
				this.m_RideZhuangBeiPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (this.Hander != null)
					{
						this.Hander(null, new DPSelectedItemEventArgs
						{
							ID = s.ID
						});
					}
				};
			}
			else
			{
				this.m_RideZhuangBeiPart.gameObject.SetActive(true);
				this.m_RideZhuangBeiPart.RefreshionData();
			}
		}
		else if (type == RidePetUIType.TuJian)
		{
			this.HideAllView();
			this.m_BtnTuJian.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("图鉴")
			});
			this.m_BtnTuJian.transform.localPosition = new Vector3(this.m_BtnTuJian.transform.localPosition.x, this.m_BtnTuJian.transform.localPosition.y, -0.3f);
			this.m_BtnTuJian.normalSprite = "teamTab_hover";
			this.m_BtnTuJian.target.spriteName = "teamTab_hover";
			if (this.m_RidePetTuJianPart == null)
			{
				this.m_RidePetTuJianPart = U3DUtils.NEW<RidePetTuJianPart>();
				this.m_RidePetTuJianPart.transform.SetParent(this.m_GameParent.transform, false);
				this.m_RidePetTuJianPart.m_Handler = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.Type == 2)
					{
						this.OpenWindow(e.Title, e.CountdownInfo);
					}
					else if (e.Type == 1 && this.Hander != null)
					{
						this.Hander(this, new DPSelectedItemEventArgs
						{
							ID = 10
						});
					}
				};
			}
			else
			{
				this.m_RidePetTuJianPart.gameObject.SetActive(true);
			}
		}
		else if (type == RidePetUIType.Fashion)
		{
			if (0 >= Global.GetRoleHorseFashionList(0).Count)
			{
				Super.HintMainText(Global.GetLang("此功能暂未开放"), 10, 3);
				return;
			}
			this.HideAllView();
			if (ConfigSystemParam.GetSystemParamIntByName("HorseFashionOpen") == 1L)
			{
				this.m_BtnFashion.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("时装")
				});
				this.m_BtnFashion.transform.localPosition = new Vector3(this.m_BtnFashion.transform.localPosition.x, this.m_BtnFashion.transform.localPosition.y, -0.3f);
				this.m_BtnFashion.normalSprite = "teamTab_hover";
				this.m_BtnFashion.target.spriteName = "teamTab_hover";
				if (this.mRideFashionPart == null)
				{
					this.mRideFashionPart = U3DUtils.NEW<RideFashionPart>();
					this.mRideFashionPart.transform.SetParent(this.m_GameParent.transform, false);
				}
				else
				{
					this.mRideFashionPart.gameObject.SetActive(true);
				}
			}
			else
			{
				this.m_BtnFashion.gameObject.SetActive(false);
			}
		}
	}

	public void RefreshBeiBaoNumber()
	{
		if (this.m_RidePetTuJianPart != null)
		{
			if (Global.Data.roleData.MountStoreList != null && 0 < Global.Data.roleData.MountStoreList.Count)
			{
				this.m_RidePetTuJianPart.m_BtnBeiBaoHongDian.gameObject.SetActive(true);
				this.m_RidePetTuJianPart.m_BtnBeiBao.Text = Global.Data.roleData.MountStoreList.Count.ToString();
			}
			else
			{
				this.m_RidePetTuJianPart.m_BtnBeiBaoHongDian.gameObject.SetActive(false);
			}
		}
		if (this.m_RidePetZuoQiPart != null)
		{
			if (Global.Data.roleData.MountStoreList != null && 0 < Global.Data.roleData.MountStoreList.Count)
			{
				this.m_RidePetZuoQiPart.mBtnBeiBaoHongDian.gameObject.SetActive(true);
				this.m_RidePetZuoQiPart.mBtnBeiBao.Text = Global.Data.roleData.MountStoreList.Count.ToString();
			}
			else
			{
				this.m_RidePetZuoQiPart.mBtnBeiBaoHongDian.gameObject.SetActive(false);
			}
		}
	}

	public void OpenWindow(string title, string content)
	{
		this.m_GameWindow.SetActive(true);
		this.m_LabWindowTitle.text = title;
		this.m_LabWinodwContent.text = content;
	}

	private void SenData()
	{
	}

	private void GetData(ZuoQiMainData mainData)
	{
		this.data = mainData;
		if (this.data == null || this.data.MountList.Count == 0)
		{
			this.BtnOclick(RidePetUIType.TuJian);
		}
	}

	public void RefreshRoleMoney()
	{
		this.m_LabCaiLiao.text = Global.GetRoleOwnNumByMoneyType(139).ToString();
	}

	internal void RefreshHorseUseing()
	{
		if (null != this.m_RidePetZuoQiPart)
		{
			this.m_RidePetZuoQiPart.RefreshHorseUseing();
		}
	}

	internal void NotcivGetMainDataCallBack(ZuoQiMainData data)
	{
		if (null != this.m_RidePetZuoQiPart && this.m_RidePetZuoQiPart.gameObject.activeSelf)
		{
			this.m_RidePetZuoQiPart.NotcivGetMainDataCallBack(data);
		}
		if (null != this.m_RidePetTuJianPart && this.m_RidePetTuJianPart.gameObject.activeSelf && data != null)
		{
			this.m_RidePetTuJianPart.NoticeRideGetTuJianDataCallBack(data.MountList);
		}
	}

	internal void NoticeRideUpStarCallBack(string[] p)
	{
		if (p != null)
		{
			int num = p[0].SafeToInt32(0);
			if (num != 0)
			{
				Super.HintMainText(Global.GetLang(ZuoQiActionResultTypeErr.GetErrMsg(num, false, false)), 10, 3);
			}
			else
			{
				GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, p[1].SafeToInt32(0), 1);
				roleHorseGoodsDataInBeiZhanBaoGuoByDbId.Forge_level = p[2].SafeToInt32(0);
				roleHorseGoodsDataInBeiZhanBaoGuoByDbId.Binding = p[3].SafeToInt32(0);
				if (null != this.m_RidePetZuoQiPart)
				{
					this.m_RidePetZuoQiPart.NoticeRideUpStarCallBack(true);
				}
			}
		}
	}

	internal void NoticeRideUpLevelCallBack(string[] p)
	{
		int num = p[0].SafeToInt32(0);
		if (num != 0)
		{
			Super.HintMainText(Global.GetLang(ZuoQiActionResultTypeErr.GetErrMsg(num, false, false)), 10, 3);
		}
		else if (null != this.m_RidePetZuoQiPart)
		{
			this.m_RidePetZuoQiPart.NoticeRideUpLevelCallBack(p[1].SafeToInt32(0), true);
		}
	}

	internal void NoticRefreshEquip(GoodsData data)
	{
		if (null != this.m_RideZhuangBeiPart)
		{
			this.m_RideZhuangBeiPart.NoticRefreshEquip(data);
		}
	}

	internal void NOticeRideUnLoadCallBack(GoodsData goodsData)
	{
		if (null != this.m_RidePetZuoQiPart)
		{
			this.m_RidePetZuoQiPart.NOticeRideUnLoadCallBack(goodsData);
		}
	}

	internal void NoticeRideGetTuJianDataCallBack(List<MountData> data)
	{
		if (null != this.m_RidePetTuJianPart)
		{
			this.m_RidePetTuJianPart.NoticeRideGetTuJianDataCallBack(data);
		}
	}

	internal void NoticeUpFashionCallBack(string[] p)
	{
		if (null != this.mRideFashionPart)
		{
			this.mRideFashionPart.NoticeUpFashionCallBack(p);
		}
	}

	internal void NoticeWearFashionCallBask(SCModGoods ModGoods)
	{
		if (null != this.mRideFashionPart)
		{
			this.mRideFashionPart.NoticeWearFashionCallBask(ModGoods);
		}
	}

	public DPSelectedItemEventHandler Hander;

	public GButton m_BtnClose;

	public GButton m_BtnZuoQi;

	public GButton m_BtnZhuangBei;

	public GButton m_BtnTuJian;

	public GButton m_BtnFashion;

	public GameObject m_GameParent;

	public GameObject m_GameWindow;

	public UILabel m_LabWindowTitle;

	public UILabel m_LabWinodwContent;

	public GButton m_BtnWindowColse;

	public UILabel m_LabCaiLiao;

	private RideFashionPart mRideFashionPart;

	private RidePetTuJianPart m_RidePetTuJianPart;

	private RidePetZuoQiPart m_RidePetZuoQiPart;

	private RideZhuangBeiPart m_RideZhuangBeiPart;

	private ZuoQiMainData data;
}
