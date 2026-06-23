using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class HuodongPartChongzhiHuikuiPartFirstChongzhiPart : UserControl
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
		if (null != this.m_BtnLingQv)
		{
			this.m_BtnLingQv.Text = Global.GetLang("领取");
		}
		if (null != this.m_BtnChongZhi)
		{
			this.m_BtnChongZhi.Text = Global.GetLang("充值");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		GameInstance.Game.QueryPayActiveInfo(Global.Data.roleData.RoleID, 1);
		this.InitControl();
		if (null != this.GoodsList)
		{
			this.ItemCollection = this.GoodsList.ItemsSource;
		}
	}

	public void ReSetUIByCommd(string[] strArr)
	{
		int num = Convert.ToInt32(strArr[0]);
		if (num == 0)
		{
			this.m_BtnLingQv.gameObject.SetActive(false);
			this.m_BtnChongZhi.gameObject.SetActive(true);
		}
		if (num == 1)
		{
			this.m_BtnLingQv.isEnabled = true;
			this.m_BtnLingQv.gameObject.SetActive(true);
			this.m_BtnChongZhi.gameObject.SetActive(false);
			this.m_SprYiJingLingQv.gameObject.SetActive(false);
		}
		if (num == 2)
		{
			this.m_BtnChongZhi.gameObject.SetActive(false);
			this.m_BtnLingQv.gameObject.SetActive(false);
			this.m_SprYiJingLingQv.gameObject.SetActive(true);
		}
	}

	private void InitControl()
	{
		if (null != this.m_BtnLingQv)
		{
			this.m_BtnLingQv.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 1, 1);
			};
		}
		if (null != this.m_BtnChongZhi)
		{
			this.m_BtnChongZhi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			};
		}
	}

	public void InitPartData(string xmlPath)
	{
		XElement isolateResXml = Global.GetIsolateResXml(xmlPath);
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "GiftList");
		if (xelement == null)
		{
			return;
		}
		this.TxtHint.Text = Global.GetXElementAttributeStr(xelement, "Description");
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Award");
		xelement = xelementList[0];
		if (xelement == null)
		{
			return;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsTwo");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "GoodsOne");
		this.addGoodsIconList(xelementAttributeStr, true);
		this.addGoodsIconList(xelementAttributeStr2, false);
		this.showGoodsIconList();
	}

	private bool IsGoodsToOccupation(int nGoodsID)
	{
		if (0 >= nGoodsID)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		int toOccupation = goodsXmlNodeByID.ToOccupation;
		return toOccupation == -1 || !MUJieripartChongzhiKingItem.IsTongGuo(goodsXmlNodeByID.ID.ToString(), Global.Data.roleData.Occupation);
	}

	private void showGoodsIconList()
	{
		this.ItemCollection.Clear();
		this.GoodsIconCanvas.Clear();
		for (int i = 0; i < this.ItemsList.Count; i++)
		{
			this.ItemCollection.AddNoUpdate(this.ItemsList[i]);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void addGoodsIconList(string goodsStr, bool isOcc = false)
	{
		string[] array = goodsStr.Split(new char[]
		{
			'|'
		});
		if (isOcc)
		{
			int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				if (this.IsGoodsToOccupation(dummyGoodsDataMu.GoodsID))
				{
					this.addGoodsIcon(dummyGoodsDataMu, true, false);
				}
			}
		}
		else
		{
			for (int j = 0; j < array.Length; j++)
			{
				string[] array3 = array[j].Split(new char[]
				{
					','
				});
				if (array3.Length != 7)
				{
					return;
				}
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				this.addGoodsIcon(dummyGoodsDataMu, false, false);
			}
		}
	}

	private void addGoodsIcon(GoodsData gd, bool isOcc = false, bool grayShow = false)
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
			this.ItemsList.Add(icon);
			if (isOcc)
			{
				this.GoodsName.Text = goodsXmlNodeByID.Title;
				this.GoodsSummary.Text = goodsXmlNodeByID.Description;
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage Bak;

	public ShowNetImage HeadBak;

	public TextBlock TxtHint;

	public SpriteSL GoodsIconCanvas;

	public TextBlock GoodsName;

	public TextBlock GoodsSummary;

	public ListBox GoodsList;

	public GButton m_BtnLingQv;

	public GButton m_BtnChongZhi;

	public UISprite m_SprYiJingLingQv;

	private List<GGoodIcon> ItemsList = new List<GGoodIcon>();

	private ObservableCollection _ItemCollection;
}
