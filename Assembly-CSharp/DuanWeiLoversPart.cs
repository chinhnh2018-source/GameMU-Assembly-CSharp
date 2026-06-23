using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class DuanWeiLoversPart : UserControl
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
		this.PaiMingLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("我的排名：")
		});
		this.DuanWeiLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("段位：")
		});
		this.DuanWeiJiFenLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("段位积分：")
		});
		this.ZhanLiLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("战   力：")
		});
		this.DuanWeiJiFenLabPl.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("段位积分：")
		});
		this.BackGround.URL = "NetImages/GameRes/Images/Plate/PKLoversDuanWeiBang.jpg.qj";
		this.PaiMing.transform.localPosition = new Vector3(-20f, 230f, -1f);
		this.DuanWei.transform.localPosition = new Vector3(110f, 230f, -1f);
		this.DuanWei.transform.parent.localPosition = new Vector3(-70f, 0f, -1f);
		this.ZhanLi.transform.localPosition = new Vector3(336f, 16f, -1f);
		this.DuanWeiJiFenPl.transform.localPosition = new Vector3(336f, -31f, -1f);
		this.PaiMingLab.pivot = 3;
		this.PaiMingLab.transform.localPosition = new Vector3(-55f, 230f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.List.ItemsSource;
		this.List.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.listBox_MouseLeftButtonUp);
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.SendDuanWeiMessage();
	}

	public void AnalysisPaiHangDataBeFromServer(CoupleArenaPaiHangData paihangData)
	{
		if (paihangData == null || paihangData.PaiHang == null)
		{
			return;
		}
		this.LocalPaiHang = paihangData.PaiHang;
		int i = 0;
		int count = paihangData.PaiHang.Count;
		while (i < count)
		{
			DuanWeiLoversPartListItem duanWeiLoversPartListItem = U3DUtils.NEW<DuanWeiLoversPartListItem>();
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(paihangData.PaiHang[i].ManZoneId, out ztBuffServerInfo))
			{
				duanWeiLoversPartListItem.SetManName = string.Format("{0}-{1}", ztBuffServerInfo.strServerName, paihangData.PaiHang[i].ManSelector.RoleName);
			}
			else
			{
				duanWeiLoversPartListItem.SetManName = string.Format("s{0}-{1}", paihangData.PaiHang[i].ManZoneId, paihangData.PaiHang[i].ManSelector.RoleName);
			}
			if (Global.GetNowServerIsZhuTiFu(paihangData.PaiHang[i].WifeZoneId, out ztBuffServerInfo))
			{
				duanWeiLoversPartListItem.SetWoManName = string.Format("{0}-{1}", ztBuffServerInfo.strServerName, paihangData.PaiHang[i].WifeSelector.RoleName);
			}
			else
			{
				duanWeiLoversPartListItem.SetWoManName = string.Format("s{0}-{1}", paihangData.PaiHang[i].WifeZoneId, paihangData.PaiHang[i].WifeSelector.RoleName);
			}
			duanWeiLoversPartListItem.PaiMingNum = paihangData.PaiHang[i].Rank;
			duanWeiLoversPartListItem.num = i;
			if (i == 0)
			{
				duanWeiLoversPartListItem.IsXuanZhong = true;
			}
			else
			{
				duanWeiLoversPartListItem.IsXuanZhong = false;
			}
			this.ItemCollection.AddNoUpdate(duanWeiLoversPartListItem);
			UIPanel component = duanWeiLoversPartListItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			this.InitRoles();
			i++;
		}
	}

	private void SendDuanWeiMessage()
	{
		GameInstance.Game.GetPaiHangInfoForPKLovers();
		Super.ShowNetWaiting(null);
	}

	private void InitRoles()
	{
		if (this.LocalPaiHang != null)
		{
			this.ChangeRoles(this.LocalPaiHang[0]);
		}
	}

	private void ChangeRoles(CoupleArenaCoupleJingJiData data)
	{
		this.LoadRoleRes(data);
		this.SetRoleAttr(data);
	}

	public override void Destroy()
	{
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
			this.rightResLoader = null;
		}
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
			this.leftResLoader = null;
		}
		base.Destroy();
	}

	private void LoadRoleRes(CoupleArenaCoupleJingJiData data)
	{
		RoleData4Selector manSelector = data.ManSelector;
		RoleData4Selector wifeSelector = data.WifeSelector;
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
		}
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
		}
		this.rightResLoader = UIHelper.LoadRoleRes(this.RightRoleModel, wifeSelector.SettingBitFlags, wifeSelector.Occupation, wifeSelector.SubOccupation, wifeSelector.RoleName, wifeSelector.GoodsDataList, null, wifeSelector.MyWingData, 1f, 0, null, false);
		this.leftResLoader = UIHelper.LoadRoleRes(this.LeftRoleModel, manSelector.SettingBitFlags, manSelector.Occupation, manSelector.SubOccupation, manSelector.RoleName, manSelector.GoodsDataList, null, manSelector.MyWingData, 1f, 0, null, false);
	}

	private void SetRoleAttr(CoupleArenaCoupleJingJiData data)
	{
		this.ZhanLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			(long)(data.ManSelector.CombatForce + data.WifeSelector.CombatForce)
		});
		this.DuanWeiJiFenPl.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.JiFen
		});
		this.DuanWeiTex.URL = string.Format("NetImages/GameRes/Images/PKLovers/{0}-{1}.png.qj", (data.DuanWeiType < 5) ? data.DuanWeiType : 5, data.DuanWeiLevel);
		this.DuanWeiName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", PKLoversPart.GetCoupleDuanWeiTypeDic()[data.DuanWeiType].Name)
		});
	}

	private void listBox_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			DuanWeiLoversPartListItem duanWeiLoversPartListItem = U3DUtils.AS<DuanWeiLoversPartListItem>(this.ItemCollection[i].gameObject);
			duanWeiLoversPartListItem.IsXuanZhong = false;
		}
		DuanWeiLoversPartListItem duanWeiLoversPartListItem2 = U3DUtils.AS<DuanWeiLoversPartListItem>(this.List.SelectedItem);
		duanWeiLoversPartListItem2.IsXuanZhong = true;
		if (null == duanWeiLoversPartListItem2)
		{
			return;
		}
		this.ChangeRoles(this.LocalPaiHang[this.List.SelectedIndex]);
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Close;

	public UILabel PaiMing;

	public UILabel PaiMingLab;

	public UILabel DuanWei;

	public UILabel DuanWeiLab;

	public UILabel DuanWeiJiFen;

	public UILabel DuanWeiJiFenLab;

	public UILabel ZhanLi;

	public UILabel ZhanLiLab;

	public UILabel DuanWeiJiFenPl;

	public UILabel DuanWeiJiFenLabPl;

	public ShowNetImage BackGround;

	public ShowNetImage DuanWeiTex;

	public UILabel DuanWeiName;

	public ListBox List;

	public Modal3DShow LeftRoleModel;

	public Modal3DShow RightRoleModel;

	private ObservableCollection _ItemCollection;

	private List<CoupleArenaCoupleJingJiData> LocalPaiHang;

	private RoleResLoader rightResLoader;

	private RoleResLoader leftResLoader;
}
