using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class GFluorescentDiamondTips : UserControl
{
	private void InitTextInPrefabs()
	{
		this.txtType.Pivot = 3;
		this.unloadBtn.Text = Global.GetLang("卸下");
		this.pulverizationBtn.Text = Global.GetLang("分解");
		this.levelupBtn.Text = Global.GetLang("升级");
		this.txtZhandouliStatic.text = Global.GetLang("战斗力");
		this.txtLevelStatic.text = Global.GetLang("等级 ");
		this.txtJichuInfo.text = Global.GetLang("[基础属性]");
		this.pulverizedPowderNum.Text = Global.GetLang("分解：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (null == this.uiCollider)
		{
			this.uiCollider = this.PropsList.gameObject.GetComponent<UICollider>();
		}
		this.GoodIcon.Width = 78.0;
		this.GoodIcon.Height = 78.0;
		this.GoodIcon.isAutoSize = false;
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Close, 0, HandTypes.None);
		};
		this.unloadBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Xiexia, 0, HandTypes.None);
		};
		this.levelupBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Jiagong, 0, HandTypes.None);
		};
		this.pulverizationBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Pulverize, 0, HandTypes.None);
		};
	}

	public void RenderTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		GGoodTips.LastGoodsID = goodsData.GoodsID;
		this.SetText(goodsOwner, goodsData);
		this.SetMenus(goodsOwner, goodsData);
	}

	private void SetPropsPanel(GoodsOwnerTypes goodsOwner, int posY)
	{
		int num = 180;
		int num2 = 36;
		int num3 = 35;
		int num4 = 80;
		int num5 = (int)this.txtInfo.ActualHeight + 10 + num4;
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(num + num3 + num5 + num2), 1f);
		Transform transform = this.PropsList.gameObject.transform;
		transform.localPosition = new Vector3(transform.localPosition.x, (float)(posY - 5), transform.localPosition.z);
		base.transform.localPosition = new Vector3(0f, (float)((int)((450f - this.Bak.transform.localScale.y) * -0.5f)), 0f);
		this.MenusList.transform.localPosition = new Vector3(this.Menus.transform.localPosition.x, this.Bak.transform.localPosition.y - this.Bak.transform.localScale.y + 20f, 0f);
	}

	private void SetText(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = string.Empty;
		text += goodsXmlNodeByID.Title;
		this.txtTitle.Text = string.Format("{0}", text);
		int num = 0;
		int num2 = Global.GetDiamondLevelByGoodsID(goodsData.GoodsID);
		if (goodsOwner == GoodsOwnerTypes.SoulCometStoneBag)
		{
			num2 = Global.GetSoulCometStoneLevel(goodsData, out num);
		}
		this.txtLevel.Text = string.Format(Global.GetLang("等级: {0}"), num2);
		double goodsDataZhanLi = Global.GetGoodsDataZhanLi(goodsData);
		double num3 = goodsDataZhanLi;
		if (goodsOwner == GoodsOwnerTypes.SoulCometStoneBag)
		{
			num3 *= (double)num2;
		}
		this.txtZhandouli.Text = num3.ToString();
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite1.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite2.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BindingSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.EndTimeSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.NoUseSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.TeXiao.gameObject, false);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string value = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodsXmlNodeByID)
		});
		this.GoodIcon.BackSpriteName0 = "bagGrid2_bak";
		this.GoodIcon.BodyURL = new ImageURL(value, false, 0);
		this.GoodIcon.ItemCategory = categoriy;
		this.GoodIcon.ItemCode = goodsData.GoodsID;
		this.GoodIcon.ItemObject = goodsData;
		this.GoodIcon.TextColor = 15793920U;
		this.GoodIcon.ContentText.Text = "Lv" + num2;
		if (num2 >= 2 && num2 <= 3)
		{
			this.GoodIcon.BackSpriteName1 = "iconState_zuoyue";
		}
		if (num2 >= 4 && num2 <= 5)
		{
			this.GoodIcon.BackSpriteName1 = "iconState_zuoyue1";
		}
		if (num2 >= 6 && num2 <= 7)
		{
			this.GoodIcon.BackSpriteName1 = "iconState_zuoyue2";
		}
		if (num2 >= 8 && num2 <= 12)
		{
			this.GoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			this.GoodIcon.TeXiao.gameObject.SetActive(true);
		}
		if (goodsOwner == GoodsOwnerTypes.SoulCometStoneBag)
		{
			int equipGoodsSuitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
			if (equipGoodsSuitID == 1)
			{
				this.GoodIcon.BackSpriteName1 = "iconState_zuoyue";
			}
			if (equipGoodsSuitID == 2)
			{
				this.GoodIcon.BackSpriteName1 = "iconState_zuoyue1";
			}
			if (equipGoodsSuitID == 3)
			{
				this.GoodIcon.BackSpriteName1 = "iconState_zuoyue2";
			}
			if (equipGoodsSuitID >= 4 && equipGoodsSuitID <= 10)
			{
				this.GoodIcon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
				this.GoodIcon.TeXiao.gameObject.SetActive(true);
			}
		}
		bool flag = Global.CanUseGoods(goodsData.GoodsID, false, true);
		Super.InitGoodsGIcon(this.GoodIcon, goodsData, true, IconTextTypes.Qianghua);
		this.txtType.Text = Global.GetGoodsType(categoriy);
		int num4 = 50;
		int num5 = 0;
		this.txtInfo.Text = goodsXmlNodeByID.Description;
		if (string.IsNullOrEmpty(this.txtInfo.Text))
		{
			this.txtInfo.Visibility = false;
		}
		else
		{
			this.txtInfo.Visibility = true;
			num4 -= (int)this.txtInfo.ActualHeight + num5;
		}
		this.PropsBase.Visibility = (Global.GetCategoriyByGoodsID(goodsData.GoodsID) != 910);
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		this.txtBaseValue.Text = GFluorescentDiamondTips.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, (goodsOwner != GoodsOwnerTypes.SoulCometStoneBag) ? 1 : num2, 0);
		this.PropsList.repositionNow = true;
		this.uiCollider.updataNow = true;
		float num6 = 55f;
		float num7 = this.txtBaseValue.transform.localPosition.y - num6;
		double num8 = (!string.IsNullOrEmpty(this.txtBaseValue.Text)) ? ((double)num7 - this.txtBaseValue.ActualHeight) : ((double)num7);
		this.Foot.localPosition = new Vector3(this.Foot.localPosition.x, (float)num8, 0f);
		if (goodsOwner == GoodsOwnerTypes.FluorescentDiamondBag)
		{
			string text2 = "e3b36c";
			string lang = Global.GetLang("分解：");
			this.pulverizedPowderNum.Text = Global.GetColorStringForNGUIText(new object[]
			{
				text2,
				lang
			}) + ((goodsXmlNodeByID == null) ? 0 : goodsXmlNodeByID.PulverizedFluorescentPowderNum);
			this.fluorescentPowderIcon.spriteName = "yingguangfenmo";
			this.fluorescentPowderIcon.transform.localPosition = new Vector3(this.pulverizedPowderNum.transform.localPosition.x + (float)((int)this.pulverizedPowderNum.ActualWidth) + 10f, -15f, 0f);
		}
		this.Foot.gameObject.SetActive(goodsOwner == GoodsOwnerTypes.FluorescentDiamondBag);
		this.SetPropsPanel(goodsOwner, num4);
	}

	private void SetMenus(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		NGUITools.SetActiveChildren(this.Menus.gameObject, false);
		if (goodsOwner == GoodsOwnerTypes.FluorescentDiamondBag)
		{
			NGUITools.SetActive(this.unloadBtn.gameObject, goodsData.Site == 7001);
			NGUITools.SetActive(this.levelupBtn.gameObject, goodsData.Site == 7001 || goodsData.Site == 7000);
			NGUITools.SetActive(this.pulverizationBtn.gameObject, goodsData.Site == 7000);
			this.MenusList.repositionNow = true;
		}
		else if (goodsOwner == GoodsOwnerTypes.SoulCometStoneBag && Global.GetCategoriyByGoodsID(goodsData.GoodsID) != 910)
		{
			NGUITools.SetActive(this.unloadBtn.gameObject, goodsData.Site == 8001);
			NGUITools.SetActive(this.levelupBtn.gameObject, goodsData.Site == 8000 || goodsData.Site == 8001);
			this.MenusList.repositionNow = true;
		}
	}

	private void TipsCallBack(TipsOperationTypes type, int id = 0, HandTypes handTypes = HandTypes.None)
	{
		bool flag = false;
		if (handTypes == HandTypes.None)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
			flag = true;
		}
		if (type == TipsOperationTypes.Jiagong || type == TipsOperationTypes.Xiexia || type == TipsOperationTypes.Pulverize)
		{
			GTipServiceEx.TipsSender.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = (int)type,
				ID = id
			});
		}
		if (flag)
		{
			GTipServiceEx.ClearChildWindow();
		}
	}

	public static string GetBaseAttributeStrFromPropertyList(double[] equipFields, int level = 1, int fillCount = 0)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = "e3b36c";
		string text4 = string.Empty;
		string text5 = " ";
		for (int i = 0; i < fillCount; i++)
		{
			text4 += text5;
		}
		for (int j = 1; j <= 10; j += 2)
		{
			if (equipFields[j] != 0.0)
			{
				text2 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					text2 + ": " + text4
				});
				double num = equipFields[j];
				if (j == 1)
				{
					if (equipFields[j] != 0.0)
					{
						text += string.Format("{0}{1}%", text2, (int)num);
						text += "\n";
					}
				}
				else
				{
					int num2 = j;
					int num3 = j + 1;
					if (equipFields[num2] != 0.0 || equipFields[num3] != 0.0)
					{
						double num4 = equipFields[num2];
						double num5 = equipFields[num3];
						text += string.Format("{0}{1}", text2, (int)(num5 * (double)level));
					}
				}
				text += "\n";
			}
		}
		for (int j = 11; j < 177; j++)
		{
			if (equipFields[j] != 0.0)
			{
				text2 = ExtPropIndexes.ExtPropIndexChineseNames[j];
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					text2 + ": " + text4
				});
				double num6 = equipFields[j];
				if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
				{
					double num7 = num6 * (double)level * 100.0;
					text += string.Format("{0}{1}%", text2, Math.Round(num7, 2));
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
				{
					text += string.Format("{0}{1}", text2, (int)(num6 * (double)level));
				}
				text += "\n";
			}
		}
		return Global.ProcessStr(text);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public UISprite Bak;

	public GButton CloseBtn;

	public CAnimation[] _HelpAnim;

	public SpriteSL Menus;

	public UITable MenusList;

	public GButton unloadBtn;

	public GButton levelupBtn;

	public GButton pulverizationBtn;

	public TextBlock txtTitle;

	public TextBlock txtZhandouli;

	public TextBlock txtType;

	public GGoodIcon GoodIcon;

	public Transform Body;

	public TextBlock txtLevel;

	public TextBlock txtInfo;

	public UITable PropsList;

	public UICollider uiCollider;

	public SpriteSL PropsBase;

	public TextBlock txtBaseValue;

	public TextBlock txtZhandouliStatic;

	public TextBlock txtLevelStatic;

	public TextBlock txtJichuInfo;

	public Transform Foot;

	public TextBlock pulverizedPowderNum;

	public UISprite fluorescentPowderIcon;
}
