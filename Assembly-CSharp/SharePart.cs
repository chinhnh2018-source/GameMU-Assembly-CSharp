using System;
using System.Collections;
using System.IO;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class SharePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnLingqu.Text = Global.GetLang("领取");
		this.TextHint.Text = Global.GetLang("每日首次分享奖励: ");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnShare.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.btnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetShareReward();
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -10
				});
			}
		};
	}

	public void InitPartData()
	{
		GameInstance.Game.GetShareStat();
		this.InitRewardData();
	}

	public void InitRewardData()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ShareAward", '|');
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] goods = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			this.initGood(goods, i);
		}
	}

	public void initGood(string[] goods, int idx)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
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
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
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
				GTipServiceEx.ResetTipsLayer(goodsData2, LayerMask.LayerToName(base.gameObject.layer));
			});
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
			ggoodIcon.transform.localPosition = new Vector3(-146f + 76f * (float)idx, -68f, -1f);
		}
	}

	private IEnumerator TakeShot()
	{
		string picName = "MU_" + DateTime.Now.ToString("yyMMddHHmmss") + ".png";
		Application.CaptureScreenshot(picName);
		string fileName = Application.persistentDataPath + "/" + picName;
		while (!File.Exists(fileName))
		{
			yield return new WaitForSeconds(0.1f);
		}
		PlatSDKMgr.WXShareImage(fileName);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		}
		yield return new WaitForEndOfFrame();
		yield break;
	}

	public void UpdateBtnStatus(string btnStatus)
	{
		if (btnStatus == "0")
		{
			this.btnLingqu.isEnabled = false;
		}
		else if (btnStatus == "1")
		{
			this.btnLingqu.isEnabled = true;
		}
		else if (btnStatus == "2")
		{
			this.btnLingqu.isEnabled = false;
			this.btnLingqu.Label.text = Global.GetLang("已领取");
		}
	}

	public void OnGetReward(string status)
	{
		GameInstance.Game.GetShareStat();
	}

	public void ResetLayer(GameObject obj, string layerName)
	{
		Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer(layerName);
		}
		obj.transform.localPosition = new Vector3(0f, 0f, -185f);
	}

	public GButton btnShare;

	public GButton btnLingqu;

	public GButton btnClose;

	public TextBlock TextHint;

	public DPSelectedItemEventHandler DPSelectedItem;
}
