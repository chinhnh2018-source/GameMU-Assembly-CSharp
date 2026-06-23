using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;

public class MUJieRipartLibao : UserControl
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
		this.lingquBtn.Text = Global.GetLang("领取");
		this.huodongStartime.Z = -0.10000000149011612;
		this.huodongEndtime.Z = -0.10000000149011612;
		this.lingquStarttime.Z = -0.10000000149011612;
		this.lingquEndtime.Z = -0.10000000149011612;
		this.descText.Z = -0.10000000149011612;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.goodlist.ItemsSource;
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.lingquBtn.isEnabled)
			{
				return;
			}
			GameInstance.Game.SpriteFetchActivityAward(9, 0);
		};
	}

	public string ThisXmlName
	{
		get
		{
			return this.thisXmlName;
		}
		set
		{
			this.thisXmlName = value;
			this.InitData(this.ThisXmlName);
			this.isInTime = Global.InLimitTimeRange(this.awardStartStr, this.awardEndStr);
			GameInstance.Game.SpriteQueryJieriDaLiBao();
		}
	}

	private void InitData(string strXML)
	{
		XElement xelement = XElement.Parse(this.ThisXmlName);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Activities");
		XElement xelement2 = xelementList[0];
		if (xelement2 == null)
		{
			return;
		}
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ActivityType");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "FromDate");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ToDate");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "AwardStartDate");
		string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "AwardEndDate");
		this.startTimeStr = xelementAttributeStr;
		this.endTimeStr = xelementAttributeStr2;
		this.awardStartStr = xelementAttributeStr3;
		this.awardEndStr = xelementAttributeStr4;
		this.huodongStartime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间："),
			"ffffff",
			this.startTimeStr
		});
		this.huodongEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.endTimeStr
		});
		this.lingquStarttime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("领取时间:"),
			"ffffff",
			this.awardStartStr
		});
		this.lingquEndtime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("    至    "),
			"ffffff",
			this.awardEndStr
		});
		List<XElement> xelementList2 = Global.GetXElementList(xelement, "GiftList");
		XElement xelement3 = xelementList2[0];
		if (xelement3 == null)
		{
			return;
		}
		string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement3, "Description");
		this.descText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动内容: "),
			"ffffff",
			xelementAttributeStr5
		});
		List<XElement> xelementList3 = Global.GetXElementList(xelement, "Award");
		XElement xelement4 = xelementList3[0];
		if (xelement4 == null)
		{
			return;
		}
		string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement4, "GoodsOne");
		string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement4, "GoodsTwo");
		string goodsIDs = string.Empty;
		if (!string.IsNullOrEmpty(xelementAttributeStr7))
		{
			goodsIDs = xelementAttributeStr6 + "@" + xelementAttributeStr7;
		}
		else
		{
			goodsIDs = xelementAttributeStr6;
		}
		this.loadGoodsList(goodsIDs);
		this.lingquBtn.gameObject.SetActive(false);
	}

	public void setCompletedInfo(int result)
	{
		if (result > 0)
		{
			this.lingquBtn.gameObject.SetActive(false);
			this.SprYilingqu.gameObject.SetActive(true);
		}
		else
		{
			this.lingquBtn.isEnabled = true;
			this.lingquBtn.gameObject.SetActive(true);
			this.SprYilingqu.gameObject.SetActive(false);
		}
	}

	private void loadGoodsList(string goodsIDs)
	{
		this.ItemCollection.Clear();
		if (!(string.Empty == goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.loadOtherJiangLiGoodsList(goodsIDs, false);
			}
			else
			{
				this.loadOtherJiangLiGoodsList(array[0], false);
				this.loadOtherJiangLiGoodsList(array[1], true);
			}
		}
	}

	private void loadOtherJiangLiGoodsList(string goodsStr, bool isOcc = false)
	{
		string text = StringUtil.trim(goodsStr);
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
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.addGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
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
			this.ItemCollection.Add(icon);
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			MUJierihuodongPart.SetGoodsIconBoxCollider(icon);
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

	public TextBlock huodongStartime;

	public TextBlock huodongEndtime;

	public TextBlock lingquStarttime;

	public TextBlock lingquEndtime;

	public ListBox goodlist;

	public TextBlock descText;

	public GButton lingquBtn;

	public UISprite SprYilingqu;

	private string startTimeStr;

	private string endTimeStr;

	private string awardStartStr;

	private string awardEndStr;

	private bool isInTime = true;

	private ObservableCollection _ItemCollection;

	private string thisXmlName = string.Empty;
}
