using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class CangbaoMiJingBoxPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
	}

	public override void Destroy()
	{
		base.Destroy();
		this.goodsList.Clear();
	}

	private void InitPrefabText()
	{
		if (this.m_SureBtn != null && this.m_SureBtn.Text != null)
		{
			this.m_SureBtn.Text = Global.GetLang("确定");
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_SureBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.handler(e, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void RemoveIcons()
	{
		this.IsStopped = true;
		base.StopCoroutine("AddGoodListIcon");
		int count = this.IconList.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			Object.Destroy(this.IconList[i].gameObject);
		}
		this.IconList.Clear();
	}

	private void RefreshAddGoodIcons(string goodsStr)
	{
		NGUITools.SetActive(this.m_AnimatorContainer, false);
		NGUITools.SetActive(this.m_AnimatorContainer, true);
		this.RemoveIcons();
		this.IsStopped = false;
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
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				int goodsID = Convert.ToInt32(array2[0]);
				int gcount = Convert.ToInt32(array2[1]);
				int binding = Convert.ToInt32(array2[2]);
				int forgeLevel = Convert.ToInt32(array2[3]);
				int zhuijiaLevel = Convert.ToInt32(array2[4]);
				int lucky = Convert.ToInt32(array2[5]);
				int zhuoyueIndex = Convert.ToInt32(array2[6]);
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				this.goodsList.Add(dummyGoodsDataMu);
			}
		}
		base.StartCoroutine("AddGoodListIcon", this.goodsList);
	}

	private IEnumerator AddGoodListIcon(List<GoodsData> goodsList)
	{
		yield return new WaitForSeconds(1f);
		this.IsStopped = false;
		int goodsCount = goodsList.Count;
		if (goodsCount == 1)
		{
			this.AddGoodIcon(goodsList[0], new Vector3(0f, 0f, 0f));
		}
		else
		{
			int beginX = -220;
			float interval = 110f;
			float realY = 20f;
			float realX = 0f;
			for (int i = 0; i < goodsCount; i++)
			{
				realY = ((i < 5) ? 20f : -80f);
				realX = ((i < 5) ? ((i % 2 != 0) ? (0f + interval * (float)(i + 1) / 2f) : (interval * 0.5f - interval * (float)(i + 1) / 2f)) : ((float)beginX + interval * (float)(i - 5)));
				if (5 > goodsCount)
				{
					realX += ((goodsCount % 2 != 0) ? 0f : (-interval * 0.5f));
				}
				else
				{
					realX = realX;
				}
				if (this.IsStopped)
				{
					yield break;
				}
				this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
				yield return new WaitForSeconds(0.3f);
			}
		}
		yield return new WaitForSeconds(1f);
		SystemHelpMgr.OnAction(UIObjIDs.QiFuPartBtn01, HelpStateEvents.Clicked, 1);
		yield break;
	}

	public void AddGoodIcon(GoodsData gd, Vector3 localPos)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			icon.TeXiao.gameObject.SetActive(true);
			GameObject gameObject = Resources.Load("UITeXiao/Qifu/GaoJiXuanZhuan") as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.15f);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
			gameObject = (Resources.Load("UITeXiao/Qifu/GaoJiShanGuang") as GameObject);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
		}
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
		};
		U3DUtils.AddChild(this.IconContainer, icon.gameObject, true);
		icon.transform.localScale = new Vector3(0f, 0f, 0f);
		icon.gameObject.transform.localPosition = new Vector3(0f, 200f, 0f);
		icon.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
		iTween.MoveTo(icon.gameObject, iTween.Hash(new object[]
		{
			"position",
			localPos,
			"time",
			1.2f,
			"islocal",
			true
		}));
		iTween.RotateTo(icon.gameObject, new Vector3(0f, 0f, 0f), 1.2f);
		iTween.ScaleTo(icon.gameObject, new Vector3(1f, 1f, 1f), 1.2f);
		this.IconList.Add(icon);
	}

	public void AddGoods(List<Data_CangBao_Box> list)
	{
		int num = 0;
		this.m_Goods = list;
		string empty = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < this.m_Goods.Count; i++)
		{
			if (this.m_Goods[i].Type == 1)
			{
				text += ((num++ <= 0) ? this.m_Goods[i].Goods : ("|" + this.m_Goods[i].Goods));
			}
			else if (this.m_Goods[i].Type == 2)
			{
				text += ((num++ <= 0) ? ("8029," + this.m_Goods[i].Goods + ",0,0,0,0,0") : ("|8029," + this.m_Goods[i].Goods + ",0,0,0,0,0"));
			}
			else if (this.m_Goods[i].Type == 3)
			{
				text += ((num++ <= 0) ? ("8028," + this.m_Goods[i].Goods + ",0,0,0,0,0") : ("|8028," + this.m_Goods[i].Goods + ",0,0,0,0,0"));
			}
		}
		this.m_ContentLabel.text = string.Empty;
		this.RefreshAddGoodIcons(text);
	}

	public GameObject m_AnimatorContainer;

	public GButton m_SureBtn;

	public GameObject IconContainer;

	public UILabel m_ContentLabel;

	private bool IsStopped;

	private List<GGoodIcon> IconList = new List<GGoodIcon>();

	private List<Data_CangBao_Box> m_Goods = new List<Data_CangBao_Box>();

	private string m_space = Global.GetLang("        ");

	private List<GoodsData> goodsList = new List<GoodsData>();

	public DPSelectedItemEventHandler handler;

	public enum BoxGoodsType
	{
		Goods = 1,
		TreasureJiFen,
		TreasureXueZuan
	}
}
