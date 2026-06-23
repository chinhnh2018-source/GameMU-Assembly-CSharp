using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class LuolanchengzhanInfoBox : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnTitle.Text = Global.GetLang("罗兰城战");
		this.staticTxt[0].text = Global.GetLang("龙塔剩余敌人:");
		this.staticTxt[1].text = Global.GetLang("占领战盟:");
		this.lblEnemyLeft.transform.localPosition = new Vector3(-30f, -11f, 0f);
		this.lblFamilyName.transform.localPosition = new Vector3(-65f, -129f, 0f);
		for (int i = 0; i < this.lblName.Length; i++)
		{
			this.lblName[i].text = "Chiến Kỳ Roland: Không";
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnTitle.Label.text = Global.GetCurrentMapName();
		this.LoadQizhiConfig();
	}

	private void LoadQizhiConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/QiZuoConfig.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		this.dictQizhiInfo = new Dictionary<string, LuolanchengzhanInfoBox.QizhiCfgInfo>();
		foreach (XElement xelement in xelementList)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NPCID");
			if (!this.dictQizhiInfo.ContainsKey(xelementAttributeStr))
			{
				LuolanchengzhanInfoBox.QizhiCfgInfo qizhiCfgInfo = new LuolanchengzhanInfoBox.QizhiCfgInfo();
				qizhiCfgInfo.UseAuthority = Global.GetXElementAttributeStr(xelement, "UseAuthority");
				qizhiCfgInfo.PosX = Global.GetXElementAttributeStr(xelement, "PosX");
				qizhiCfgInfo.PosY = Global.GetXElementAttributeStr(xelement, "PosY");
				qizhiCfgInfo.BufferID = Global.GetXElementAttributeStr(xelement, "BufferID");
				qizhiCfgInfo.MapCode = Global.GetXElementAttributeStr(xelement, "MapCode");
				this.dictQizhiInfo.Add(xelementAttributeStr, qizhiCfgInfo);
			}
		}
	}

	private void SetEnemyLeft(string info)
	{
		this.lblEnemyLeft.text = info;
	}

	private void SetFamilyName(string info)
	{
		this.lblFamilyName.text = info;
	}

	public void RefreshEnemyCount(List<LuoLanChengZhanRoleCountData> list)
	{
		if (list != null)
		{
			int num = 0;
			foreach (LuoLanChengZhanRoleCountData luoLanChengZhanRoleCountData in list)
			{
				if (luoLanChengZhanRoleCountData.BHID != 0 && luoLanChengZhanRoleCountData.BHID != Global.Data.roleData.Faction)
				{
					num += luoLanChengZhanRoleCountData.RoleCount;
				}
			}
			this.SetEnemyLeft(string.Empty + num);
		}
		else
		{
			this.SetEnemyLeft("0");
		}
	}

	public void RefreshBuff(List<LuoLanChengZhanQiZhiBuffOwnerData> list)
	{
		int num = 0;
		if (list != null)
		{
			foreach (LuoLanChengZhanQiZhiBuffOwnerData luoLanChengZhanQiZhiBuffOwnerData in list)
			{
				if (this.dictQizhiInfo.ContainsKey(string.Empty + luoLanChengZhanQiZhiBuffOwnerData.NPCID) && num < this.lblName.Length)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(this.dictQizhiInfo[string.Empty + luoLanChengZhanQiZhiBuffOwnerData.NPCID].BufferID));
					if (luoLanChengZhanQiZhiBuffOwnerData.OwnerBHID != 0)
					{
						this.lblName[num].text = goodsXmlNodeByID.Title + ": " + luoLanChengZhanQiZhiBuffOwnerData.OwnerBHName;
					}
					else
					{
						this.lblName[num].text = goodsXmlNodeByID.Title + Global.GetLang(": 无");
					}
				}
				num++;
			}
		}
	}

	public void RefreshLongtaInfo(LuoLanChengZhanLongTaOwnerData longtaData)
	{
		if (longtaData != null)
		{
			if (longtaData.OwnerBHid != 0)
			{
				this.SetFamilyName(longtaData.OwnerBHName);
			}
			else
			{
				this.SetFamilyName(Global.GetLang("无"));
			}
		}
	}

	public GButton btnTitle;

	public UILabel lblEnemyLeft;

	public UILabel[] lblName;

	public UILabel[] lblBuff;

	public UILabel lblFamilyName;

	private Dictionary<string, LuolanchengzhanInfoBox.QizhiCfgInfo> dictQizhiInfo;

	public TextBlock[] staticTxt;

	private class QizhiCfgInfo
	{
		public string UseAuthority;

		public string PosX;

		public string PosY;

		public string BufferID;

		public string MapCode;
	}
}
