using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class LangHunLingYuInfoBox : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnTitle.Label.text = Global.GetLang("圣域争霸");
		this.staticTxt[0].text = Global.GetLang("龙塔剩余敌人:");
		this.staticTxt[1].text = Global.GetLang("占领战盟:");
		this.lblFamilyName.transform.localPosition = new Vector3(-100f, -118f, 0f);
		for (int i = 0; i < this.lblName.Length; i++)
		{
			this.lblName[i].text = "Cờ: Không";
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.LoadQizhiConfig();
		if (Global.CanGuanZhan())
		{
			NGUITools.SetActive(this.staticTxt[0].gameObject, false);
			this.mTrsfRoleNameGroup.localPosition = new Vector3(-80f, 104f, 0f);
			this.mTrsfRoleDamageGroup.localPosition = new Vector3(80f, 104f, 0f);
		}
		else
		{
			NGUITools.SetActive(this.staticTxt[0].gameObject, true);
			this.mTrsfRoleNameGroup.localPosition = new Vector3(-80f, 68f, 0f);
			this.mTrsfRoleDamageGroup.localPosition = new Vector3(80f, 68f, 0f);
		}
	}

	private void LoadQizhiConfig()
	{
		XElement gameResXml = Global.GetGameResXml("Config/CityWarQiZuo.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		this.dictQizhiInfo = new Dictionary<string, LangHunLingYuInfoBox.QizhiCfgInfo>();
		foreach (XElement xelement in xelementList)
		{
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NPCID");
			if (!this.dictQizhiInfo.ContainsKey(xelementAttributeStr))
			{
				LangHunLingYuInfoBox.QizhiCfgInfo qizhiCfgInfo = new LangHunLingYuInfoBox.QizhiCfgInfo();
				qizhiCfgInfo.PosX = Global.GetXElementAttributeStr(xelement, "PosX");
				qizhiCfgInfo.PosY = Global.GetXElementAttributeStr(xelement, "PosY");
				qizhiCfgInfo.BufferID = Global.GetXElementAttributeStr(xelement, "BufferID");
				this.dictQizhiInfo.Add(xelementAttributeStr, qizhiCfgInfo);
			}
		}
	}

	private void SetEnemyLeft(string info)
	{
		if (Global.CanGuanZhan())
		{
			this.lblEnemyLeft.text = string.Empty;
		}
		else
		{
			this.lblEnemyLeft.text = info;
		}
	}

	private void SetFamilyName(string info)
	{
		this.lblFamilyName.text = info;
	}

	public void RefreshEnemyCount(List<BangHuiRoleCountData> list)
	{
		if (list != null)
		{
			int num = 0;
			foreach (BangHuiRoleCountData bangHuiRoleCountData in list)
			{
				if (bangHuiRoleCountData.BHID != 0 && bangHuiRoleCountData.BHID != Global.Data.roleData.Faction)
				{
					num += bangHuiRoleCountData.RoleCount;
				}
			}
			this.SetEnemyLeft(string.Empty + num);
		}
		else
		{
			this.SetEnemyLeft("0");
		}
	}

	public void RefreshBuff(List<LangHunLingYuQiZhiBuffOwnerData> list)
	{
		int num = 0;
		if (list != null)
		{
			foreach (LangHunLingYuQiZhiBuffOwnerData langHunLingYuQiZhiBuffOwnerData in list)
			{
				if (this.dictQizhiInfo.ContainsKey(string.Empty + langHunLingYuQiZhiBuffOwnerData.NPCID) && num < this.lblName.Length)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(this.dictQizhiInfo[string.Empty + langHunLingYuQiZhiBuffOwnerData.NPCID].BufferID));
					if (langHunLingYuQiZhiBuffOwnerData.OwnerBHID != 0)
					{
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(langHunLingYuQiZhiBuffOwnerData.OwnerBHZoneId, out ztBuffServerInfo))
						{
							this.lblName[num].text = string.Concat(new string[]
							{
								goodsXmlNodeByID.Title,
								": ",
								ztBuffServerInfo.strServerName,
								"-",
								langHunLingYuQiZhiBuffOwnerData.OwnerBHName
							});
						}
						else
						{
							this.lblName[num].text = string.Concat(new object[]
							{
								goodsXmlNodeByID.Title,
								": [",
								Global.GetLang("区"),
								langHunLingYuQiZhiBuffOwnerData.OwnerBHZoneId,
								"] ",
								langHunLingYuQiZhiBuffOwnerData.OwnerBHName
							});
						}
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

	public void RefreshLongtaInfo(LangHunLingYuLongTaOwnerData longtaData)
	{
		if (longtaData != null)
		{
			if (longtaData.OwnerBHid != 0)
			{
				ZtBuffServerInfo ztBuffServerInfo = null;
				if (Global.GetNowServerIsZhuTiFu(longtaData.OwnerBHZoneId, out ztBuffServerInfo))
				{
					string familyName = ztBuffServerInfo.strServerName + "-" + longtaData.OwnerBHName;
					this.SetFamilyName(familyName);
				}
				else
				{
					string familyName2 = string.Concat(new object[]
					{
						"[",
						Global.GetLang("区"),
						longtaData.OwnerBHZoneId,
						"] ",
						longtaData.OwnerBHName
					});
					this.SetFamilyName(familyName2);
				}
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

	public Transform mTrsfRoleNameGroup;

	public Transform mTrsfRoleDamageGroup;

	private Dictionary<string, LangHunLingYuInfoBox.QizhiCfgInfo> dictQizhiInfo;

	public TextBlock[] staticTxt;

	private class QizhiCfgInfo
	{
		public string PosX;

		public string PosY;

		public string BufferID;
	}
}
