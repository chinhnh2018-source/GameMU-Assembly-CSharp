using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FortressTaskPetItem : UserControl
{
	public float Percent
	{
		get
		{
			return this.mPercent;
		}
		set
		{
			this.mPercent = value;
			this.LabelPercent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("成功率：") + this.mPercent.ToString("0") + "%"
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitXml();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private void InitXml()
	{
	}

	private void InitPrefabText()
	{
		try
		{
			this.LabelPetLevel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("等级:") + "1"
			});
			this.LabelAttribute.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("卓越属性条数: ") + "0"
			});
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	private GGoodIcon GetGoodIcon(GoodsData data)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		}
		return ggoodIcon;
	}

	public void RefreshPetInf(GoodsData gd, PetMissionVO petMission, byte bState = 0)
	{
		if (gd != null)
		{
			try
			{
				this.mGoodsData = gd;
				GGoodIcon ggoodIcon = null;
				Transform transform = this.ObjIcon.transform.FindChild("PetIocn");
				if (null != transform)
				{
					ggoodIcon = transform.GetComponent<GGoodIcon>();
					if (null == ggoodIcon)
					{
						Object.DestroyImmediate(transform.gameObject);
					}
				}
				if (null == ggoodIcon)
				{
					ggoodIcon = this.GetGoodIcon(gd);
					ggoodIcon.transform.name = "PetIocn";
				}
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.mGoodsData.GoodsID);
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
				ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
				NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
				ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
				ggoodIcon.BackgroundSprite1.transform.localPosition = new Vector3(ggoodIcon.BackgroundSprite1.transform.localPosition.x, ggoodIcon.BackgroundSprite1.transform.localPosition.y, -0.001f);
				ggoodIcon.BackSpriteName1 = string.Empty;
				Super.InitGoodsGIcon(ggoodIcon, this.mGoodsData, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
				int goodsQuality = Super.GetGoodsQuality(this.mGoodsData.GoodsID);
				if (goodsQuality == 4 || goodsQuality == 6)
				{
					ggoodIcon.TeXiao.gameObject.SetActive(true);
				}
				else
				{
					ggoodIcon.TeXiao.gameObject.SetActive(false);
				}
				ggoodIcon.transform.SetParent(this.ObjIcon.transform, false);
				UIPanel component = ggoodIcon.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
				ggoodIcon.petLevel.text = string.Empty;
				if (bState == 0)
				{
					if (gd.Site == 10001)
					{
						this.PetState = 1;
					}
					else
					{
						this.PetState = 0;
					}
				}
				this.LabelPetLevel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("等级:") + (gd.Forge_level + 1).ToString()
				});
				this.LabelPetName.text = Global.GetGoodsNameByID(gd.GoodsID, true);
				int num = 0;
				List<string> zhuoYueAttribute = Global.GetZhuoYueAttribute(gd);
				if (zhuoYueAttribute != null)
				{
					num = zhuoYueAttribute.Count;
				}
				this.LabelAttribute.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("卓越属性条数: ") + num.ToString()
				});
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}
	}

	public byte PetState
	{
		get
		{
			return this.mPetState;
		}
		set
		{
			this.mPetState = value;
			if (value == 0)
			{
				this.LabelTaskState.text = string.Empty;
			}
			else if (value == 1)
			{
				this.LabelTaskState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("任务中")
				});
			}
			else if (value == 2)
			{
				this.LabelTaskState.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("已上阵")
				});
			}
			Transform transform = this.ObjIcon.transform.FindChild("PetIocn");
			if (null != transform)
			{
				GGoodIcon component = transform.GetComponent<GGoodIcon>();
				if (null != component)
				{
					if (value == 1 || value == 2)
					{
						component.NoUseSpriteVisible = true;
						component.NoUseSprite.spriteName = "iconState_nouse3";
						component.NoUseSprite.alpha = 0.65f;
						component.NoUseSprite.transform.localPosition = new Vector3(component.NoUseSprite.transform.localPosition.x, component.NoUseSprite.transform.localPosition.y, -0.1f);
					}
					else
					{
						component.NoUseSpriteVisible = false;
					}
				}
			}
		}
	}

	public GoodsData GGoodsData
	{
		get
		{
			return this.mGoodsData;
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this.mDragPanelContents)
			{
				this.mDragPanelContents = base.GetComponent<UIDragPanelContents>();
				if (null == this.mDragPanelContents)
				{
					this.mDragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
				}
			}
			this.mDragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	[SerializeField]
	private GameObject ObjIcon;

	[SerializeField]
	private UILabel LabelTaskState;

	[SerializeField]
	private UILabel LabelPetName;

	[SerializeField]
	private UILabel LabelPetLevel;

	[SerializeField]
	private UILabel LabelAttribute;

	[SerializeField]
	private UILabel LabelPercent;

	private UIDragPanelContents mDragPanelContents;

	private GoodsData mGoodsData;

	private float mPercent;

	private byte mPetState;
}
