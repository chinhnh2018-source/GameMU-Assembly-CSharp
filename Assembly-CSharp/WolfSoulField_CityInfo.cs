using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class WolfSoulField_CityInfo : UserControl
{
	public int Level
	{
		get
		{
			return this.level;
		}
		set
		{
			this.level = value;
			if (this.isWorldCity)
			{
				this.SetWroldCityInfo();
				this.SetGuanZhanInfo(this.level);
			}
			else
			{
				this.SetCityInfo(this.level);
			}
			this.SetInfo(this.level);
		}
	}

	public ObservableCollection ItemCollection1
	{
		get
		{
			return this._ItemCollection1;
		}
		set
		{
			this._ItemCollection1 = value;
		}
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

	public LangHunLingYuCityData LangHunlingyuCityData
	{
		get
		{
			return this.langhunlingyuCityData;
		}
		set
		{
			this.langhunlingyuCityData = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("城池信息")
		});
		this.awarddayInfo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("每日税收奖励")
		});
		this.awardInfo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("获胜奖励")
		});
		this.zhanling.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("占领者")
		});
		this.jingong.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("进攻方")
		});
		this.zhandoutime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("战斗时间")
		});
		this.BtnJoin.Label.text = Global.GetLang("参战");
		this.zhanling.transform.localPosition = new Vector3(155f, 189f, -3f);
		this.jingong.transform.localPosition = new Vector3(158f, 90f, -3f);
		this.zhandoutime.transform.localPosition = new Vector3(164f, -54f, -3f);
		this.BtnGuanZhan.Label.text = Global.GetLang("观战");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		NGUITools.SetActive(this.BtnGuanZhan.gameObject, false);
		this.ItemCollection1 = this.rewardDay.ItemsSource;
		this.ItemCollection2 = this.Reward.ItemsSource;
		if (this.isOtherCity)
		{
			this.BtnJoin.isEnabled = false;
		}
		else
		{
			this.BtnJoin.isEnabled = true;
		}
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
		this.BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isOtherCity)
			{
				Super.HintMainText(Global.GetLang("其他城池不可参战！"), 10, 3);
				return;
			}
			GameInstance.Game.EnterCity(this.cityID);
		};
		this.BtnGuanZhan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.LangHunlingyuCityData != null)
			{
				GameInstance.Game.EnterCity(this.LangHunlingyuCityData.CityId);
			}
		};
	}

	private void SetInfo(int cityLevel)
	{
		if (this.isShowzhandouTime)
		{
			this.GetFightTime(cityLevel);
		}
		this.City.URL = this.ChangeCityPic(cityLevel);
		this.judian.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format(Global.GetLang("{0}级据点"), cityLevel)
		});
		this.loadGoodsList(Global.GetCityInfo()[cityLevel].DayAward, true);
		this.loadGoodsList(Global.GetCityInfo()[cityLevel].Award, false);
	}

	private void SetCityInfo(int cityLevel)
	{
		this.zhanlingname.text = Global.GetLang("未知势力");
		this.jingongname.text = Global.GetLang("未知势力");
		this.BtnJoin.isEnabled = false;
		this.time.text = Global.GetLang("无战斗时间");
		if (WolfSoulField_Part.langhunlingyuRoleData == null)
		{
			return;
		}
		if (this.isOtherCity)
		{
			this.CityInfo(WolfSoulField_Part.langhunlingyuRoleData.OtherCityList, cityLevel, true);
		}
		else
		{
			this.CityInfo(WolfSoulField_Part.langhunlingyuRoleData.SelfCityList, cityLevel, false);
		}
	}

	private void CityInfo(List<LangHunLingYuCityData> CityList, int cityLevel, bool otherCity = false)
	{
		this.SetGuanZhanInfo(cityLevel);
		bool flag = false;
		if (CityList != null)
		{
			for (int i = 0; i < CityList.Count; i++)
			{
				if (cityLevel == CityList[i].CityLevel)
				{
					if (CityList[i].Owner != null)
					{
						this.zhanlingname.text = string.Empty;
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(CityList[i].Owner.ZoneID, out ztBuffServerInfo))
						{
							this.zhanlingname.text = Global.GetColorStringForNGUIText(new object[]
							{
								(Global.Data.roleData.Faction != CityList[i].Owner.BHID) ? "edddbf" : "17e43e",
								ztBuffServerInfo.strServerName + "-" + CityList[i].Owner.BHName
							});
						}
						else
						{
							this.zhanlingname.text = Global.GetColorStringForNGUIText(new object[]
							{
								(Global.Data.roleData.Faction != CityList[i].Owner.BHID) ? "edddbf" : "17e43e",
								string.Format(Global.GetLang("{0}区-{1}"), CityList[i].Owner.ZoneID, CityList[i].Owner.BHName)
							});
						}
						this.cityID = CityList[i].CityId;
						if (Global.Data.roleData.Faction == CityList[i].Owner.BHID)
						{
							flag = true;
						}
					}
					else
					{
						this.zhanlingname.text = Global.GetLang("无人占领");
					}
					if (CityList[i].AttackerList != null)
					{
						this.jingongname.text = string.Empty;
						for (int j = 0; j < CityList[i].AttackerList.Count; j++)
						{
							ZtBuffServerInfo ztBuffServerInfo2 = null;
							if (Global.GetNowServerIsZhuTiFu(CityList[i].AttackerList[j].ZoneID, out ztBuffServerInfo2))
							{
								UILabel uilabel = this.jingongname;
								uilabel.text += Global.GetColorStringForNGUIText(new object[]
								{
									(Global.Data.roleData.Faction != CityList[i].AttackerList[j].BHID) ? "edddbf" : "17e43e",
									ztBuffServerInfo2.strServerName + "-" + CityList[i].AttackerList[j].BHName + "\r\n\r\n"
								});
							}
							else
							{
								UILabel uilabel2 = this.jingongname;
								uilabel2.text += Global.GetColorStringForNGUIText(new object[]
								{
									(Global.Data.roleData.Faction != CityList[i].AttackerList[j].BHID) ? "edddbf" : "17e43e",
									string.Format(Global.GetLang("{0}区-{1}"), CityList[i].AttackerList[j].ZoneID, CityList[i].AttackerList[j].BHName) + "\r\n\r\n"
								});
							}
							if (Global.Data.roleData.Faction == CityList[i].AttackerList[j].BHID)
							{
								flag = true;
							}
						}
						this.cityID = CityList[i].CityId;
						this.isShowzhandouTime = true;
						this.BtnJoin.isEnabled = (!otherCity && flag && Global.IsWarTime(this.level));
					}
					else
					{
						this.jingongname.text = Global.GetLang("无人进攻");
					}
					return;
				}
			}
		}
	}

	private void SetGuanZhanInfo(int level)
	{
		if (level >= 7 && Global.Data != null && Global.Data.roleData.HideGM > 0)
		{
			NGUITools.SetActive(this.BtnGuanZhan.gameObject, true);
			this.BtnGuanZhan.isEnabled = (this.LangHunlingyuCityData != null);
		}
		else
		{
			NGUITools.SetActive(this.BtnGuanZhan.gameObject, false);
		}
	}

	private void SetWroldCityInfo()
	{
		this.BtnJoin.isEnabled = false;
		if (this.LangHunlingyuCityData == null)
		{
			this.zhanlingname.text = Global.GetLang("未知势力");
			this.jingongname.text = Global.GetLang("未知势力");
			return;
		}
		if (this.LangHunlingyuCityData.Owner != null)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.LangHunlingyuCityData.Owner.ZoneID, out ztBuffServerInfo))
			{
				this.zhanlingname.text = Global.GetColorStringForNGUIText(new object[]
				{
					"edddbf",
					ztBuffServerInfo.strServerName + "-" + this.LangHunlingyuCityData.Owner.BHName
				});
			}
			else
			{
				this.zhanlingname.text = Global.GetColorStringForNGUIText(new object[]
				{
					"edddbf",
					string.Format(Global.GetLang("{0}区-{1}"), this.LangHunlingyuCityData.Owner.ZoneID, this.LangHunlingyuCityData.Owner.BHName)
				});
			}
		}
		else
		{
			this.zhanlingname.text = Global.GetLang("未知势力");
		}
		if (this.LangHunlingyuCityData.AttackerList != null)
		{
			for (int i = 0; i < this.LangHunlingyuCityData.AttackerList.Count; i++)
			{
				ZtBuffServerInfo ztBuffServerInfo2 = null;
				if (Global.GetNowServerIsZhuTiFu(this.LangHunlingyuCityData.AttackerList[i].ZoneID, out ztBuffServerInfo2))
				{
					UILabel uilabel = this.jingongname;
					uilabel.text += Global.GetColorStringForNGUIText(new object[]
					{
						"edddbf",
						ztBuffServerInfo2.strServerName + "-" + this.LangHunlingyuCityData.AttackerList[i].BHName + "\r\n"
					});
				}
				else
				{
					UILabel uilabel2 = this.jingongname;
					uilabel2.text += Global.GetColorStringForNGUIText(new object[]
					{
						"edddbf",
						string.Format(Global.GetLang("{0}区-{1}"), this.LangHunlingyuCityData.AttackerList[i].ZoneID, this.LangHunlingyuCityData.AttackerList[i].BHName) + "\r\n"
					});
				}
			}
		}
		else
		{
			this.jingongname.text = Global.GetLang("未知势力");
		}
	}

	private string ChangeCityPic(int level)
	{
		string result = "NetImages/GameRes/Images/Plate/City_1.png";
		if (level == 1 || level == 2 || level == 3)
		{
			result = "NetImages/GameRes/Images/Plate/City_1.png";
		}
		else if (level == 4 || level == 5 || level == 6)
		{
			result = "NetImages/GameRes/Images/Plate/City_2.png";
		}
		else if (level == 7 || level == 8 || level == 9)
		{
			result = "NetImages/GameRes/Images/Plate/City_3.png";
		}
		else if (level == 10)
		{
			result = "NetImages/GameRes/Images/Plate/City_4.png";
		}
		return result;
	}

	private void GetFightTime(int CityLevel)
	{
		DateTime dateTime = Global.GetCorrectDateTime();
		DayOfWeek dayOfWeek = dateTime.DayOfWeek;
		int num = (!Global.IsDayWarTime(CityLevel)) ? (CityLevel + 1) : CityLevel;
		num = ((num <= 10) ? num : 10);
		string text;
		if (CityLevel == 10)
		{
			int num2 = Global.GetCityInfo()[CityLevel].AttackWeekDay.SafeToInt32(0);
			if (Convert.ToInt32(dayOfWeek) == num2)
			{
				if (Global.IsDayWarTime(CityLevel))
				{
					text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
				}
				else
				{
					dateTime = dateTime.AddDays(7.0);
					text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
				}
			}
			else if (Convert.ToInt32(dayOfWeek) < num2)
			{
				int num3 = num2 - Convert.ToInt32(dayOfWeek);
				dateTime = dateTime.AddDays((double)num3);
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
			else
			{
				int num3 = num2 - Convert.ToInt32(dayOfWeek) + 7;
				dateTime = dateTime.AddDays((double)num3);
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
		}
		else
		{
			int num4 = 0;
			string[] array = Global.GetCityInfo()[CityLevel].AttackWeekDay.Split(new char[]
			{
				','
			});
			string[] array2 = Global.GetCityInfo()[CityLevel].BaoMingIntro.Split(new char[]
			{
				'|'
			});
			int num5 = Global.GetCorrectDateTime().DayOfWeek;
			int num6 = num5;
			if (array.Length != array2.Length)
			{
				MUDebug.Log<string>(new string[]
				{
					"配置表：AttackWeekDay------BaoMingIntro不匹配"
				});
			}
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (num5 == array[j].SafeToInt32(0))
					{
						num4 = j;
						break;
					}
				}
				if (num5 == array[num4].SafeToInt32(0))
				{
					break;
				}
				num5++;
				if (num5 > 6)
				{
					num5 = 0;
				}
				dateTime = dateTime.AddDays(1.0);
			}
			text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			if (Global.IsDayWarTime(CityLevel))
			{
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
			else if (num5 == num6)
			{
				for (int k = 0; k < 7; k++)
				{
					dateTime = dateTime.AddDays(1.0);
					bool flag = false;
					for (int l = 0; l < array.Length; l++)
					{
						if (Convert.ToInt32(dateTime.DayOfWeek) == array[l].SafeToInt32(0))
						{
							text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		string attackTime = Global.GetCityInfo()[num].AttackTime;
		this.time.text = Global.GetColorStringForNGUIText(new object[]
		{
			"feedc5",
			text
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"3eb431",
			string.Format("\r\n{0}", attackTime)
		});
	}

	private void loadGoodsList(string Goods, bool change)
	{
		if (change)
		{
			this.ItemCollection1.Clear();
		}
		else
		{
			this.ItemCollection2.Clear();
		}
		string text = StringUtil.trim(Goods);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				this.addGoodsIcon(dummyGoodsDataMu, change, false);
			}
		}
		if (change)
		{
			this.ItemCollection1.DelayUpdate();
		}
		else
		{
			this.ItemCollection2.DelayUpdate();
		}
	}

	private void addGoodsIcon(GoodsData gd, bool change, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			if (change)
			{
				this.ItemCollection1.Add(icon);
			}
			else
			{
				this.ItemCollection2.Add(icon);
			}
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public GButton Close;

	public GButton BtnJoin;

	public GButton BtnGuanZhan;

	public ListBox rewardDay;

	public ListBox Reward;

	public ShowNetImage City;

	public UILabel Title;

	public UILabel awarddayInfo;

	public UILabel awardInfo;

	public UILabel zhanling;

	public UILabel zhanlingname;

	public UILabel jingong;

	public UILabel jingongname;

	public UILabel zhandoutime;

	public UILabel time;

	public UILabel judian;

	public DPSelectedItemEventHandler DPSelectItem;

	private bool isShowzhandouTime;

	private int cityID;

	private int level;

	public bool isWorldCity;

	public bool isOtherCity;

	private ObservableCollection _ItemCollection1;

	private ObservableCollection _ItemCollection2;

	private LangHunLingYuCityData langhunlingyuCityData;
}
