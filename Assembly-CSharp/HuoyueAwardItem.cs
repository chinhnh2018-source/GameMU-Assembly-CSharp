using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class HuoyueAwardItem : UserControl
{
	public int ID
	{
		get
		{
			return this.m_AwardID;
		}
		set
		{
			this.m_AwardID = value;
		}
	}

	public int Need
	{
		get
		{
			return this.m_Need;
		}
		set
		{
			this.m_Need = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.GetButton.Text = Global.GetLang("领取");
		this.m_States[1].GetComponent<GButton>().MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.goodsCount > Global.GetBaoGuoSpaceCount())
			{
				Super.HintMainText(Global.GetLang("领取失败，背包空间不足！"), 10, 3);
			}
			else
			{
				GameInstance.Game.SpriteGetDailyActiveAwardCmd(this.ID);
			}
		};
	}

	public void init(string[] gods)
	{
		this.goodsCount = gods.Length;
		for (int i = 0; i < this.goodsCount; i++)
		{
			string[] array = gods[i].Split(new char[]
			{
				','
			});
			int num = Convert.ToInt32(array[0]);
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
			if (goodsXmlNodeByID != null)
			{
				GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 78.0;
				ggoodIcon.Height = 78.0;
				GoodsData goodsData = new GoodsData();
				goodsData.GoodsID = num;
				goodsData.GCount = Convert.ToInt32(array[1]);
				goodsData.Binding = Convert.ToInt32(array[2]);
				goodsData.Forge_level = Convert.ToInt32(array[3]);
				goodsData.AppendPropLev = Convert.ToInt32(array[4]);
				goodsData.Lucky = Convert.ToInt32(array[5]);
				goodsData.ExcellenceInfo = Convert.ToInt32(array[6]);
				ggoodIcon.ItemObject = goodsData;
				ggoodIcon.ItemCode = num;
				ggoodIcon.TipType = 1;
				ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				if (i == 0)
				{
					ggoodIcon.transform.localPosition = new Vector3(-107f, 0f, -1f);
					ggoodIcon.GoodImg.gameObject.transform.localScale = new Vector3(64f, 64f, 0f);
				}
				else
				{
					ggoodIcon.transform.localPosition = new Vector3(this.m_Icon2Sprite.transform.localPosition.x, 0f, -1f);
				}
				this.m_Icon2Sprite.transform.localScale = new Vector3(78f, 78f, 1f);
				U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
				Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
				ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.ShowGoodsTip(s);
				};
				if (i == 1)
				{
					this.m_Icon2Sprite.transform.gameObject.SetActive(true);
				}
			}
		}
	}

	public void setStates(int states)
	{
		for (int i = 0; i < this.m_States.Length; i++)
		{
			if (i == states)
			{
				this.m_States[i].SetActive(true);
			}
			else
			{
				this.m_States[i].SetActive(false);
			}
		}
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	public UISprite m_Icon2Sprite;

	public GameObject[] m_States;

	public GButton GetButton;

	private int m_AwardID;

	private int m_Need;

	private int goodsCount;
}
