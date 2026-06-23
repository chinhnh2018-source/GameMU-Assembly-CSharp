using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class PKLoversPartZhanBao : UserControl
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.ItemCollection = this.List.ItemsSource;
		this.back.URL = "NetImages/GameRes/Images/Plate/PKLoversZhanBao.jpg.qj";
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.SendZhanBaoMessge();
		this.LianSheng.transform.localPosition = new Vector3(-368f, -36f, -1f);
		this.DuanWeiJiFen.transform.localPosition = new Vector3(-368f, -66f, -1f);
	}

	public void InitAttr(CoupleArenaMainData mainData)
	{
		if (mainData == null)
		{
			return;
		}
		this.LeftRole.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(mainData.JingJiData.ManSelector.Occupation),
			mainData.JingJiData.ManSelector.RoleSex
		});
		this.LeftRoleName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"3681f3",
			mainData.JingJiData.ManSelector.RoleName
		});
		this.RightRole.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(mainData.JingJiData.WifeSelector.Occupation),
			mainData.JingJiData.WifeSelector.RoleSex
		});
		this.RightRoleName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"B02B94",
			mainData.JingJiData.WifeSelector.RoleName
		});
		this.DuanWei.text = string.Format("{0}", PKLoversPart.GetCoupleDuanWeiTypeDic()[mainData.JingJiData.DuanWeiType].Name);
		this.ShegnLv.text = string.Format("{0}%", (mainData.JingJiData.TotalFightTimes != 0) ? ((int)((double)mainData.JingJiData.WinFightTimes * 1.0 / (double)mainData.JingJiData.TotalFightTimes * 100.0)) : 0);
		this.LianSheng.text = mainData.JingJiData.LianShengTimes.ToString();
		this.DuanWeiJiFen.text = mainData.JingJiData.JiFen.ToString();
	}

	public void InitList(List<CoupleArenaZhanBaoItemData> PKLogData)
	{
		if (PKLogData == null)
		{
			return;
		}
		int i = 0;
		int count = PKLogData.Count;
		while (i < count)
		{
			string text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("你们挑战")
			});
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(PKLogData[i].TargetManZoneId, out ztBuffServerInfo))
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("{0}-{1},", ztBuffServerInfo.strServerName, PKLogData[i].TargetManRoleName)
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("s{0}-{1},", PKLogData[i].TargetManZoneId, PKLogData[i].TargetManRoleName)
				});
			}
			if (Global.GetNowServerIsZhuTiFu(PKLogData[i].TargetWifeZoneId, out ztBuffServerInfo))
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("{0}-{1}", ztBuffServerInfo.strServerName, PKLogData[i].TargetWifeRoleName)
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("s{0}-{1}", PKLogData[i].TargetWifeZoneId, PKLogData[i].TargetWifeRoleName)
				});
			}
			text += Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(",")
			});
			if (PKLogData[i].IsWin)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("{0} {1} ", Global.GetLang("获得段位积分"), PKLogData[i].GetJiFen)
				});
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"điểm"
				});
			}
			else
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					Global.GetLang("你们失败了")
				});
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang(", ")
				});
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("{0} {1} ", Global.GetLang("扣除段位积分"), PKLogData[i].GetJiFen)
				});
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"điểm"
				});
			}
			PKLoversPartZhanBaoList pkloversPartZhanBaoList = U3DUtils.NEW<PKLoversPartZhanBaoList>();
			pkloversPartZhanBaoList.Miaoshu = text;
			this.ItemCollection.AddNoUpdate(pkloversPartZhanBaoList);
			UIPanel component = pkloversPartZhanBaoList.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			i++;
		}
	}

	private void SendZhanBaoMessge()
	{
		GameInstance.Game.GetZhanBaoInfoForPKLovers();
		Super.ShowNetWaiting(null);
	}

	public GButton BtnClose;

	public ListBox List;

	public DPSelectedItemEventHandler CloseHandler;

	public ShowNetImage back;

	public ShowNetImage LeftRole;

	public ShowNetImage RightRole;

	public UILabel LeftRoleName;

	public UILabel RightRoleName;

	public UILabel DuanWei;

	public UILabel ShegnLv;

	public UILabel LianSheng;

	public UILabel DuanWeiJiFen;

	public UILabel RongYaoDianShu;

	private ObservableCollection _ItemCollection;
}
