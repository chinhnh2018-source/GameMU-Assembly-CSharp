using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class luolan_part : UserControl
{
	private void InitTextInPrefabs()
	{
		this.chaKanJiangLi.Text = Global.GetLang("查看奖励");
		this.chaKanGuiZe.Text = Global.GetLang("查看规则");
		this.chengZhanShenQing.Text = Global.GetLang("城战申请");
		this.chengZhanJinRu.Text = Global.GetLang("参加城战");
		this.gongHui.text = Global.GetLang("占领公会：") + Global.GetLang("无战盟占领");
		this.leader.text = Global.GetLang("罗兰城主：") + Global.GetLang("无战盟占领");
		this.btnWingLoad.Text = Global.GetLang("佩戴");
		this.btnWingUnload.Text = Global.GetLang("卸下");
		this.lblRewardPrompt.text = Global.GetLang("罗兰城战占领战盟成员 \n 每日可领取以上奖励！");
		this.lblRewardPrompt.pivot = 3;
		this.lblRewardPrompt.transform.localPosition = new Vector3(-160f, -75f, 0f);
		this.lblRewardPrompt.lineWidth = 320;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ConfigInit();
		this.InitPromptInfo();
		this.chengZhanJinRu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.InChengzhanTime())
			{
				Super.HintMainText(Global.GetLang("当前不在城战开启时间内！"), 10, 3);
				return;
			}
			if (this.chengzhuInfo != null && this.shenqingList != null)
			{
				bool flag = false;
				bool flag2 = Global.Data.roleData.Faction == this.chengzhuInfo.BHID;
				foreach (LuoLanChengZhanRequestInfoEx luoLanChengZhanRequestInfoEx in this.shenqingList)
				{
					if (Global.Data.roleData.Faction == luoLanChengZhanRequestInfoEx.BHID)
					{
						flag = true;
						break;
					}
				}
				if (flag2 || flag)
				{
					GameInstance.Game.LuoLanChengZhan(0);
				}
				else
				{
					Super.HintMainText(Global.GetLang("您的战盟没有取得本次城战参加资格！"), 10, 3);
				}
			}
		};
		this.chaKanJiangLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.obj3DModel.SetActive(false);
			this.objJiangliPart.SetActive(true);
			if (this.goodsList == null || this.goodsList.Count <= 0)
			{
				string text = this.cfgGoodIds[0] + "," + this.cfgZhangong + ",0,0,0,0,0";
				this.goodsList.Add(this.initGood(text.Split(new char[]
				{
					','
				}), 0, this.objJiangliPart));
				text = this.cfgGoodIds[1] + "," + this.cfgDayExp + ",0,0,0,0,0";
				this.goodsList.Add(this.initGood(text.Split(new char[]
				{
					','
				}), 1, this.objJiangliPart));
				this.goodsList.Add(this.initGood(this.cfgGoods.Split(new char[]
				{
					','
				}), 2, this.objJiangliPart));
			}
			if (this.chengzhuInfo.BHID == Global.Data.roleData.Faction)
			{
				this.btnGetJiangli.gameObject.SetActive(true);
				this.lblRewardPrompt.gameObject.SetActive(false);
				if (this.chengzhuInfo.isGetReward)
				{
					this.btnGetJiangli.isEnabled = false;
					this.btnGetJiangli.Label.text = Global.GetLang("已领取");
				}
				else
				{
					this.btnGetJiangli.isEnabled = true;
					this.btnGetJiangli.Label.text = Global.GetLang("领取奖励");
				}
			}
			else
			{
				this.btnGetJiangli.gameObject.SetActive(false);
				this.lblRewardPrompt.gameObject.SetActive(true);
			}
		};
		this.btnCloseJiangli.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.obj3DModel.SetActive(true);
			this.objJiangliPart.SetActive(false);
		};
		this.btnGetJiangli.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetChengZhanDailyAwards();
		};
		this.btnWingLoad.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.MyWingData.Using == 0)
			{
				Super.HintMainText(Global.GetLang("需要佩戴翅膀才能佩戴罗兰羽翼！"), 10, 3);
				return;
			}
			GameInstance.Game.UploadLuoLanWing(this.cfgWingTabId, this.cfgWingId, 1);
			this.btnWingLoad.gameObject.SetActive(false);
			this.btnWingUnload.gameObject.SetActive(true);
		};
		this.btnWingUnload.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.UploadLuoLanWing(this.cfgWingTabId, this.cfgWingId, 2);
			this.btnWingLoad.gameObject.SetActive(true);
			this.btnWingUnload.gameObject.SetActive(false);
		};
		this.btnWingTips.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(Convert.ToInt32(this.cfgWingGoodsId));
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsData.GoodsID);
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(dummyGoodsData.GoodsID);
			GTipServiceEx.ShowTip(goodsXmlNodeByID.Title, UIHelper.GetBaseAttributeStr(dummyGoodsData, goodsEquipPropsDoubleList, -1), TipTypes.NormalText, true);
		};
		GameInstance.Game.GetLuoLanChengZhuInfo();
		GameInstance.Game.GetBangHuiLingDiItemData();
	}

	private void ConfigInit()
	{
		XElement gameResXml = Global.GetGameResXml("Config/SiegeWarfare.Xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
		this.cfgKaifuTime = Global.GetXElementAttributeStr(xelementList[0], "TimePoints");
		this.cfgKaifuDay = Global.GetXElementAttributeStr(xelementList[0], "WeekPoints").Split(new char[]
		{
			'|'
		});
		this.cfgEnrollTime = Global.GetXElementAttributeStr(xelementList[0], "EnrollTime");
		gameResXml = Global.GetGameResXml("Config/SiegeWarfareEveryDayAward.xml");
		xelementList = Global.GetXElementList(gameResXml, "DayAward");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Status");
			if (Global.Data.roleData.BHZhiWu == xelementAttributeInt)
			{
				this.cfgZhangong = Global.GetXElementAttributeStr(xelement, "DayZhanGong");
				this.cfgDayExp = Global.GetXElementAttributeStr(xelement, "DayExp");
				this.cfgGoods = Global.GetXElementAttributeStr(xelement, "DayGoods");
				this.cfgGoodIds = Global.GetXElementAttributeStr(xelement, "ImageID").Split(new char[]
				{
					','
				});
				break;
			}
		}
		gameResXml = Global.GetGameResXml("Config/Fashion.xml");
		xelementList = Global.GetXElementList(gameResXml, "Fashion");
		foreach (XElement xelement2 in xelementList)
		{
			this.cfgWingType = Global.GetXElementAttributeInt(xelement2, "Type");
			if (this.cfgWingType == 1)
			{
				this.cfgWingId = Global.GetXElementAttributeInt(xelement2, "ID");
				this.cfgWingGoodsId = Global.GetXElementAttributeStr(xelement2, "Goods");
				this.cfgWingName = Global.GetXElementAttributeStr(xelement2, "Name");
				this.cfgWingTabId = Global.GetXElementAttributeInt(xelement2, "Tab");
				break;
			}
		}
	}

	private void InitPromptInfo()
	{
		string[] array = new string[]
		{
			Global.GetLang("周一"),
			Global.GetLang("周二"),
			Global.GetLang("周三"),
			Global.GetLang("周四"),
			Global.GetLang("周五"),
			Global.GetLang("周六"),
			Global.GetLang("周日")
		};
		string text = string.Empty;
		for (int i = 0; i < this.cfgKaifuDay.Length; i++)
		{
			text += array[Convert.ToInt32(this.cfgKaifuDay[i]) - 1];
			if (i < this.cfgKaifuDay.Length - 1)
			{
				text += Global.GetLang("、");
			}
		}
		this.time.text = Global.GetLang("开战时间：") + text + " " + this.cfgKaifuTime;
	}

	public bool InChengzhanTime()
	{
		bool result = false;
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int num = Convert.ToInt32(correctDateTime.DayOfWeek);
		if (num == 0)
		{
			num = 7;
		}
		foreach (string strNum in this.cfgKaifuDay)
		{
			if (num == strNum.SafeToInt32(0))
			{
				DateTime dateTime = Convert.ToDateTime(correctDateTime.ToShortDateString() + " " + this.cfgKaifuTime);
				if (correctDateTime >= dateTime && correctDateTime < dateTime.AddSeconds((double)this.cfgEnrollTime.SafeToInt32(0)))
				{
					result = true;
				}
				break;
			}
		}
		return result;
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
	}

	public void RefreshChengzhuInfo(LuoLanChengZhuInfo chengzhuInfo)
	{
		if (chengzhuInfo != null)
		{
			this.chengzhuInfo = chengzhuInfo;
			this.gongHui.text = Global.GetLang("占领公会：") + ((chengzhuInfo.BHName != null && !(chengzhuInfo.BHName == string.Empty)) ? chengzhuInfo.BHName : Global.GetLang("无战盟占领"));
			if (chengzhuInfo.RoleInfoList.Count > 0)
			{
				RoleData4Selector roleData4Selector = chengzhuInfo.RoleInfoList[0];
				if (roleData4Selector != null)
				{
					this.leader.text = Global.GetLang("罗兰城主：") + roleData4Selector.RoleName;
					int fashionWingGoodsId = 0;
					if (roleData4Selector.RoleID == Global.Data.roleData.RoleID)
					{
						int num = 26;
						if (num < Global.Data.roleData.RoleCommonUseIntPamams.Count)
						{
							int fashionID = Global.Data.roleData.RoleCommonUseIntPamams[num];
							fashionWingGoodsId = Global.GetFashionGoodsID(fashionID);
						}
					}
					else
					{
						fashionWingGoodsId = Convert.ToInt32(this.cfgWingGoodsId);
					}
					if (this.roleResLoader != null)
					{
						this.roleResLoader.Stop();
					}
					this.roleResLoader = UIHelper.LoadRoleRes(this.Modal3D, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1.5f, fashionWingGoodsId, null, false);
					if (Global.Data.roleData.RoleID == roleData4Selector.RoleID)
					{
						if (Global.Data.roleData.RoleCommonUseIntPamams[26] == 1)
						{
							this.btnWingLoad.gameObject.SetActive(false);
							this.btnWingUnload.gameObject.SetActive(true);
						}
						else
						{
							this.btnWingLoad.gameObject.SetActive(true);
							this.btnWingUnload.gameObject.SetActive(false);
						}
					}
					else
					{
						this.btnWingLoad.gameObject.SetActive(false);
						this.btnWingUnload.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	public void Reload3DModal()
	{
		if (this.chengzhuInfo != null && this.chengzhuInfo.RoleInfoList.Count > 0)
		{
			RoleData4Selector roleData4Selector = this.chengzhuInfo.RoleInfoList[0];
			if (roleData4Selector != null)
			{
				this.leader.text = Global.GetLang("罗兰城主：") + roleData4Selector.RoleName;
				int fashionWingGoodsId = 0;
				if (roleData4Selector.RoleID == Global.Data.roleData.RoleID)
				{
					int num = 26;
					if (num < Global.Data.roleData.RoleCommonUseIntPamams.Count)
					{
						int fashionID = Global.Data.roleData.RoleCommonUseIntPamams[num];
						fashionWingGoodsId = Global.GetFashionGoodsID(fashionID);
					}
				}
				else
				{
					fashionWingGoodsId = Convert.ToInt32(this.cfgWingGoodsId);
				}
				this.roleResLoader = UIHelper.LoadRoleRes(this.Modal3D, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1.5f, fashionWingGoodsId, null, false);
			}
		}
	}

	public void RefreshChengzhanShenqingInfo(List<LuoLanChengZhanRequestInfoEx> listResponse)
	{
		this.shenqingList = listResponse;
	}

	public GGoodIcon initGood(string[] goods, int idx, GameObject parent)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2f);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.transform.localPosition = new Vector3(-100f + 105f * (float)idx, 15f, -1.5f);
			U3DUtils.AddChild(parent, ggoodIcon.gameObject, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon2 = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon2)
				{
					return;
				}
				GoodsData goodsData2 = ggoodIcon2.ItemObject as GoodsData;
				if (goodsData2 == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(ggoodIcon2, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData2);
			});
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
		}
		return ggoodIcon;
	}

	public void RefreshRewardResult(int result)
	{
		if (result == 0 || result == 1)
		{
			this.btnGetJiangli.Label.text = Global.GetLang("已领取");
			this.btnGetJiangli.isEnabled = false;
			this.chengzhuInfo.isGetReward = true;
		}
		else
		{
			string errMsg = StdErrorCode.GetErrMsg(result, false, false);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), errMsg, delegate(object s, DPSelectedItemEventArgs e)
			{
			}, new string[]
			{
				Global.GetLang("确定")
			});
		}
	}

	public LuoLanChengZhuInfo LuoLanChengZhuInfo
	{
		get
		{
			return this.chengzhuInfo;
		}
	}

	public Modal3DShow Modal3D;

	public UILabel gongHui;

	public UILabel leader;

	public UILabel time;

	public GButton chaKanJiangLi;

	public GButton chaKanGuiZe;

	public GButton chengZhanShenQing;

	public GButton chengZhanJinRu;

	public GButton yanHui;

	public GameObject obj3DModel;

	public GameObject objJiangliPart;

	public GButton btnCloseJiangli;

	public GButton btnGetJiangli;

	public GButton btnWingLoad;

	public GButton btnWingUnload;

	public GButton btnWingTips;

	public UILabel lblRewardPrompt;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<LuoLanChengZhanRequestInfoEx> shenqingList;

	private LuoLanChengZhuInfo chengzhuInfo;

	private string[] cfgKaifuDay;

	private string cfgEnrollTime;

	private string cfgKaifuTime;

	private string cfgZhangong;

	private string cfgDayExp;

	private string cfgGoods;

	private string[] cfgGoodIds;

	private List<GGoodIcon> goodsList = new List<GGoodIcon>();

	private int cfgWingTabId;

	private string cfgWingGoodsId;

	private string cfgWingName;

	private int cfgWingType;

	private int cfgWingId;

	private RoleResLoader roleResLoader;
}
