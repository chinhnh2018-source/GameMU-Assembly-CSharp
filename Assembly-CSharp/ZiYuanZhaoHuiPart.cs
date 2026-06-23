using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZiYuanZhaoHuiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnOneTime.Text = Global.GetLang("一键找回");
		this.m_BtnPerfect.Text = Global.GetLang("完美找回");
		this.ConstHintText.Text = Global.GetLang("金币找回消耗金币找回75%的资源，钻石找回消耗钻石找回100%的资源\n一键找回消耗金币找回75%的资源，完美找回消耗钻石找回100%的资源");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_BtnOneTime)
		{
			this.m_BtnOneTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				OldResourceInfo totalResourceInfo = this.GetTotalResourceInfo(this.infoList);
				this.OpenZhaoHuiPopPart(totalResourceInfo, ZiYuanZhaoHuiPart.ZhaoHuiModel.TotalByGold, ZiYuanZhaoHuiPart.MoneyType.Gold, -1);
			};
		}
		if (null != this.m_BtnPerfect)
		{
			this.m_BtnPerfect.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				OldResourceInfo totalResourceInfo = this.GetTotalResourceInfo(this.infoList);
				this.OpenZhaoHuiPopPart(totalResourceInfo, ZiYuanZhaoHuiPart.ZhaoHuiModel.TotalByDiamond, ZiYuanZhaoHuiPart.MoneyType.Diamond, -1);
			};
		}
		this.m_GiftItemOBC = this.m_ListGiftItems.ItemsSource;
	}

	private OldResourceInfo GetTotalResourceInfo(List<OldResourceInfo> infoList)
	{
		OldResourceInfo oldResourceInfo = new OldResourceInfo();
		int count = infoList.Count;
		for (int i = 0; i < count; i++)
		{
			OldResourceInfo oldResourceInfo2 = infoList[i];
			oldResourceInfo.exp += oldResourceInfo2.exp;
			oldResourceInfo.bandmoney += oldResourceInfo2.bandmoney;
			oldResourceInfo.mojing += oldResourceInfo2.mojing;
			oldResourceInfo.shengwang += oldResourceInfo2.shengwang;
			oldResourceInfo.chengjiu += oldResourceInfo2.chengjiu;
			oldResourceInfo.zhangong += oldResourceInfo2.zhangong;
			oldResourceInfo.bandDiamond += oldResourceInfo2.bandDiamond;
			oldResourceInfo.xinghun += oldResourceInfo2.xinghun;
			oldResourceInfo.yuanSuFenMo += oldResourceInfo2.yuanSuFenMo;
		}
		return oldResourceInfo;
	}

	public void RequestServerData()
	{
		GameInstance.Game.QueryGetOldResourceInfo();
	}

	public void RefreshByServerData(List<OldResourceInfo> dataList)
	{
		if (dataList != null)
		{
			this.infoList = dataList;
			this.m_GiftItemOBC.Clear();
			int count = dataList.Count;
			for (int i = 0; i < count; i++)
			{
				ZiYuanZhaoHuiPartGiftItem ziYuanZhaoHuiPartGiftItem = U3DUtils.NEW<ZiYuanZhaoHuiPartGiftItem>();
				ziYuanZhaoHuiPartGiftItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
						OldResourceInfo resourceInfo2 = e.Data as OldResourceInfo;
						int id = e.ID;
						this.OpenZhaoHuiPopPart(resourceInfo2, ZiYuanZhaoHuiPart.ZhaoHuiModel.Single, ZiYuanZhaoHuiPart.MoneyType.Gold, id);
					}
					else if (e.IDType == 2)
					{
						OldResourceInfo resourceInfo2 = e.Data as OldResourceInfo;
						int id = e.ID;
						this.OpenZhaoHuiPopPart(resourceInfo2, ZiYuanZhaoHuiPart.ZhaoHuiModel.Single, ZiYuanZhaoHuiPart.MoneyType.Diamond, id);
					}
				};
				OldResourceInfo resourceInfo = dataList[i];
				ziYuanZhaoHuiPartGiftItem.Refresh(resourceInfo);
				this.m_GiftItemOBC.AddNoUpdate(ziYuanZhaoHuiPartGiftItem);
			}
			if (dataList.Count == 0)
			{
				this.SetEmptyState();
			}
			this.m_GiftItemOBC.DelayUpdate();
		}
		else
		{
			this.SetEmptyState();
		}
	}

	public void RefreshByGetResourceResult(int ID, int zhaoHuiModel)
	{
		if (zhaoHuiModel == 1 || zhaoHuiModel == 2)
		{
			if (this.infoList != null)
			{
				this.infoList.Clear();
			}
			this.m_GiftItemOBC.Clear();
			this.SetEmptyState();
		}
		else
		{
			int count = this.infoList.Count;
			int num = -1;
			for (int i = 0; i < count; i++)
			{
				OldResourceInfo oldResourceInfo = this.infoList[i];
				if (oldResourceInfo.type == ID)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this.infoList.RemoveAt(num);
				this.m_GiftItemOBC.RemoveAt(num);
				this.DragPanel.RestrictWithinBounds(true);
			}
			if (this.infoList.Count == 0)
			{
				this.SetEmptyState();
			}
		}
	}

	private void SetEmptyState()
	{
		this.m_bakEmpty.gameObject.SetActive(true);
		this.m_BtnOneTime.isEnabled = false;
		this.m_BtnPerfect.isEnabled = false;
	}

	private void OpenZhaoHuiPopPart(OldResourceInfo resourceInfo, ZiYuanZhaoHuiPart.ZhaoHuiModel zhaoHuiModel, ZiYuanZhaoHuiPart.MoneyType moneyType, int id = -1)
	{
		if (this.ZhaoHuiPopPart != null)
		{
			this.CloseZhaoHuiPopPart();
		}
		this.ZhaoHuiPopWindow = U3DUtils.NEW<GChildWindow>();
		this.ZhaoHuiPopWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.ZhaoHuiPopWindow, Global.GetLang(string.Empty));
		if (null == this.ZhaoHuiPopPart)
		{
			this.ZhaoHuiPopPart = U3DUtils.NEW<ZiYuanZhaoHuiPopPart>();
			this.ZhaoHuiPopPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseZhaoHuiPopPart();
				}
			};
			this.ZhaoHuiPopPart.Refresh(resourceInfo, zhaoHuiModel, moneyType, id);
			this.ZhaoHuiPopWindow.SetContent(this.ZhaoHuiPopWindow.BodyPresenter, this.ZhaoHuiPopPart, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, 0f, 0f);
			this.ZhaoHuiPopPart.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(this.ZhaoHuiPopWindow);
		}
	}

	private void CloseZhaoHuiPopPart()
	{
		if (null != this.ZhaoHuiPopWindow)
		{
			Super.GData.PlayZoneRoot.Children.Remove(this.ZhaoHuiPopWindow, true);
			Object.Destroy(this.ZhaoHuiPopPart.gameObject);
			this.ZhaoHuiPopPart = null;
			this.ZhaoHuiPopWindow = null;
		}
	}

	public GameObject m_PnlGift;

	public GButton m_BtnOneTime;

	public GButton m_BtnPerfect;

	public UISprite m_bakLight;

	public UISprite m_bakEmpty;

	public UIDraggablePanel DragPanel;

	public TextBlock ConstHintText;

	private ObservableCollection m_GiftItemOBC;

	public ListBox m_ListGiftItems = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<OldResourceInfo> infoList = new List<OldResourceInfo>();

	private GChildWindow ZhaoHuiPopWindow;

	private ZiYuanZhaoHuiPopPart ZhaoHuiPopPart;

	public enum MoneyType
	{
		Gold,
		Diamond
	}

	public enum ZhaoHuiModel
	{
		Single,
		TotalByGold,
		TotalByDiamond
	}
}
