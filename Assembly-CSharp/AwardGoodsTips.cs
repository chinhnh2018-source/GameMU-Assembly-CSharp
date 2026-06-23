using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class AwardGoodsTips : UserControl
{
	protected override void InitializeComponent()
	{
		this.staticText.Text = Global.GetLang("点击任意位置关闭界面");
		this.IconList = new List<GGoodIcon>();
		UIEventListener.Get(this.bakClose.gameObject).onClick = delegate(GameObject s)
		{
			this.CloseWindow();
		};
	}

	private void CloseWindow()
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(null, new DPSelectedItemEventArgs());
		}
	}

	protected override void OnDestroy()
	{
		base.StopCoroutine("AddGoodListIcon");
	}

	public void ShowGoodsIcon(List<GoodsData> goodsList)
	{
		if (goodsList == null || goodsList.Count <= 0)
		{
			return;
		}
		base.StartCoroutine("AddGoodListIcon", goodsList);
	}

	private IEnumerator AddGoodListIcon(List<GoodsData> goodsList)
	{
		yield return new WaitForSeconds(0.1f);
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
			for (int i = 0; i < 10; i++)
			{
				if (i >= 5)
				{
					realY = -80f;
					realX = (float)beginX + interval * (float)(i - 5);
				}
				else
				{
					realX = (float)beginX + interval * (float)i;
				}
				if (i < goodsCount)
				{
					this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		int leftCount = 10;
		if (leftCount >= goodsList.Count)
		{
			this.isStop = true;
		}
		if (this.isStop)
		{
			yield break;
		}
		yield return new WaitForSeconds(0.3f);
		for (int j = 0; j < this.IconContainer.transform.childCount; j++)
		{
			Object.Destroy(this.IconContainer.transform.GetChild(j).gameObject);
		}
		yield return new WaitForSeconds(0.1f);
		if (leftCount <= goodsList.Count)
		{
			goodsList.RemoveRange(0, leftCount);
			if (leftCount >= goodsList.Count)
			{
				this.isStop = true;
			}
			base.StartCoroutine("AddGoodListIcon", goodsList);
		}
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
		icon.SecondText.Text = gd.GCount.ToString();
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		icon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
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
		icon.gameObject.transform.localPosition = new Vector3(0f, 100f, 0f);
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

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite bakClose;

	public GameObject IconContainer;

	private List<GGoodIcon> IconList;

	public TextBlock staticText;

	private bool isStop;
}
