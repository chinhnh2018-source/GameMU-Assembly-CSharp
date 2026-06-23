using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class GYuansuTips : UserControl
{
	private void InitTextInPrefabs()
	{
		this.PeidaiIcon.Text = Global.GetLang("佩戴");
		this.XiexiaIcon.Text = Global.GetLang("卸下");
		this.ShengjiIcon.Text = Global.GetLang("升级");
		this.txtZhandouliStatic.text = Global.GetLang("战斗力");
		this.txtLevelStatic.text = Global.GetLang("等级 ");
		this.txtJichuInfo.text = Global.GetLang("[基础属性]");
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
		this.PeidaiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Peidai, 0, HandTypes.None);
		};
		this.XiexiaIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Xiexia, 0, HandTypes.None);
		};
		this.ShengjiIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Jiagong, 0, HandTypes.None);
		};
	}

	public void RenderTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
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
		this.txtLevel.Text = string.Format(Global.GetLang("等级: {0}"), Global.GetYuansuGoodsDataLevel(goodsData));
		this.txtZhandouli.Text = Global.GetGoodsDataZhanLi(goodsData).ToString();
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
		bool flag = Global.CanUseGoods(goodsData.GoodsID, false, true);
		Super.InitYuansuGoodsGIcon(this.GoodIcon, goodsData);
		this.txtType.Text = Global.GetGoodsType(categoriy);
		int num = 50;
		int num2 = 0;
		this.txtInfo.Text = goodsXmlNodeByID.Description;
		if (string.IsNullOrEmpty(this.txtInfo.Text))
		{
			this.txtInfo.Visibility = false;
		}
		else
		{
			this.txtInfo.Visibility = true;
			num -= (int)this.txtInfo.ActualHeight + num2;
		}
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 810)
		{
			this.PropsBase.Visibility = false;
		}
		else
		{
			this.PropsBase.Visibility = true;
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
			this.txtBaseValue.Text = this.GetBaseAttributeStr(goodsData, goodsEquipPropsDoubleList, categoriy);
		}
		this.PropsList.repositionNow = true;
		this.uiCollider.updataNow = true;
		this.SetPropsPanel(goodsOwner, num);
	}

	private string GetBaseAttributeStr(GoodsData gd, double[] equipFields_1, int categoriy = -1)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int yuansuGoodsDataLevel = Global.GetYuansuGoodsDataLevel(gd);
		for (int i = 1; i <= 10; i += 2)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num = equipFields_1[i];
				if (i == 1)
				{
					if (equipFields_1[i] != 0.0)
					{
						text += string.Format("{0}: {1}%", text2, (int)equipFields_1[i]);
						text += "\n";
					}
				}
				else
				{
					int num2 = i;
					int num3 = i + 1;
					if (equipFields_1[num2] != 0.0 || equipFields_1[num3] != 0.0)
					{
						double num4 = equipFields_1[num2];
						double num5 = equipFields_1[num3];
						text += string.Format("{0}: {1}", text2, (int)(num5 * (double)yuansuGoodsDataLevel));
					}
				}
				text += "\n";
			}
		}
		for (int i = 11; i < 177; i++)
		{
			if (equipFields_1[i] != 0.0)
			{
				text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]);
				double num6 = equipFields_1[i];
				if (ExtPropIndexes.ExtPropIndexPercents[i] != 1)
				{
					if (ExtPropIndexes.ExtPropIndexPercents[i] == 0)
					{
						text += string.Format("{0}: {1}\r\n", text2, (int)(num6 * (double)yuansuGoodsDataLevel));
					}
				}
			}
		}
		return this.ProcessStr(text);
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	private void SetMenus(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		NGUITools.SetActiveChildren(this.Menus.gameObject, false);
		if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 810)
		{
			return;
		}
		if (goodsOwner == GoodsOwnerTypes.YuansuBag)
		{
			NGUITools.SetActive(this.PeidaiIcon.gameObject, goodsData.Site == 3000);
			NGUITools.SetActive(this.ShengjiIcon.gameObject, true);
			NGUITools.SetActive(this.XiexiaIcon.gameObject, goodsData.Site == 3001);
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
		if (type > TipsOperationTypes.Close && type < TipsOperationTypes.SwitchHand)
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

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	public UISprite Bak;

	public GButton CloseBtn;

	public CAnimation[] _HelpAnim;

	public SpriteSL Menus;

	public UITable MenusList;

	public GButton PeidaiIcon;

	public GButton XiexiaIcon;

	public GButton ShengjiIcon;

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
}
