using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LoversWishPart_Record : UserControl
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

	private void InitTextInPrefabs()
	{
		this.rightBG.URL = "NetImages/GameRes/Images/Wish/TiaoXingGe.jpg.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.ListItem.ItemsSource;
		GameInstance.Game.GetWishRecordForCoupleWish();
		Super.ShowNetWaiting(null);
	}

	public void InitLeftAttr(CoupleWishMainData Data)
	{
		this.roleManPic.URL = ((Data.MyCoupleManSelector != null) ? StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(Data.MyCoupleManSelector.Occupation),
			Data.MyCoupleManSelector.RoleSex
		}) : string.Empty);
		this.roleWomanPic.URL = ((Data.MyCoupleWifeSelector != null) ? StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(Data.MyCoupleWifeSelector.Occupation),
			Data.MyCoupleWifeSelector.RoleSex
		}) : string.Empty);
		this.roleManName.text = ((Data.MyCoupleManSelector != null) ? Global.GetColorStringForNGUIText(new object[]
		{
			"3681f3",
			Data.MyCoupleManSelector.RoleName
		}) : string.Empty);
		this.roleWomanName.text = ((Data.MyCoupleWifeSelector != null) ? Global.GetColorStringForNGUIText(new object[]
		{
			"CD33AB",
			Data.MyCoupleWifeSelector.RoleName
		}) : string.Empty);
		this.paiming.text = ((Data.MyCoupleRank != 0) ? Data.MyCoupleRank.ToString() : Global.GetLang("无"));
		this.zhufuzhi.text = Data.MyCoupleBeWishNum.ToString();
	}

	public void InitList(List<CoupleWishWishRecordData> Data)
	{
		int i = 0;
		int count = Data.Count;
		while (i < count)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			bool flag = false;
			ZtBuffServerInfo ztBuffServerInfo = null;
			string colorStringForNGUIText;
			if (Global.GetNowServerIsZhuTiFu(Data[i].FromRole.ZoneId, out ztBuffServerInfo))
			{
				colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(Data[i].FromRole.RoleId != Global.Data.roleData.RoleID) ? Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, Data[i].FromRole.RoleName, 0) : Global.GetLang("你")
				});
			}
			else
			{
				colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(Data[i].FromRole.RoleId != Global.Data.roleData.RoleID) ? string.Format("S{0}.{1}", Data[i].FromRole.ZoneId, Data[i].FromRole.RoleName) : Global.GetLang("你")
				});
			}
			if (LoversWishPart.GetWishTypeDic().ContainsKey(Data[i].WishType))
			{
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					LoversWishPart.GetWishTypeDic()[Data[i].WishType].Name
				});
			}
			if (Data[i].WishTxt != null)
			{
				text3 = Data[i].WishTxt;
			}
			for (int j = 0; j < Data[i].TargetRoles.Count; j++)
			{
				ZtBuffServerInfo ztBuffServerInfo2 = null;
				if (Global.GetNowServerIsZhuTiFu(Data[i].TargetRoles[j].ZoneId, out ztBuffServerInfo2))
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						(Data[i].TargetRoles[j].RoleId != Global.Data.roleData.RoleID) ? Global.FormatRoleNameZhuTiFu(ztBuffServerInfo2.strServerName, Data[i].TargetRoles[j].RoleName, 0) : Global.GetLang("你")
					});
				}
				else
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						(Data[i].TargetRoles[j].RoleId != Global.Data.roleData.RoleID) ? string.Format("S{0}.{1}", Data[i].TargetRoles[j].ZoneId, Data[i].TargetRoles[j].RoleName) : Global.GetLang("你")
					});
				}
				if (Data[i].TargetRoles.Count > 1 && !flag)
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("和")
					});
					flag = true;
				}
			}
			string miaoshu = string.Concat(new string[]
			{
				colorStringForNGUIText,
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("向")
				}),
				text,
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang(",赠送")
				}),
				text2,
				Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("祝福")
				})
			});
			LoversWishPart_RecordItem loversWishPart_RecordItem = U3DUtils.NEW<LoversWishPart_RecordItem>();
			loversWishPart_RecordItem.Miaoshu = miaoshu;
			this.ItemCollection.AddNoUpdate(loversWishPart_RecordItem);
			UIPanel component = loversWishPart_RecordItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("{0}{1}", Global.GetLang("寄语："), text3)
				});
				LoversWishPart_RecordItem loversWishPart_RecordItem2 = U3DUtils.NEW<LoversWishPart_RecordItem>();
				loversWishPart_RecordItem2.Miaoshu = colorStringForNGUIText2;
				this.ItemCollection.AddNoUpdate(loversWishPart_RecordItem2);
				UIPanel component2 = loversWishPart_RecordItem2.GetComponent<UIPanel>();
				if (component2)
				{
					Object.Destroy(component2);
				}
			}
			i++;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox ListItem;

	public ShowNetImage rightBG;

	public ShowNetImage roleManPic;

	public ShowNetImage roleWomanPic;

	public UILabel paiming;

	public UILabel zhufuzhi;

	public UILabel roleManName;

	public UILabel roleWomanName;

	private ObservableCollection _ItemCollection;
}
