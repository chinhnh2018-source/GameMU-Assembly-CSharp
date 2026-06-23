using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class GSoulGuardTips : UserControl
{
	private void InitTextInPrefabs()
	{
		this.unloadIcon.Text = Global.GetLang("卸下");
		this.replaceIcon.Text = Global.GetLang("替换");
		this.txtJichuInfo.text = Global.GetLang("[基础属性]");
		this.txtInfo.MaxWidth = 225.0;
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
		this.unloadIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Xiexia, 0, HandTypes.None);
		};
		this.replaceIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TipsCallBack(TipsOperationTypes.Replace, 0, HandTypes.None);
		};
	}

	public void RenderTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		this.SetText(goodsOwner, goodsData);
	}

	private void SetPropsPanel(GoodsOwnerTypes goodsOwner)
	{
		int num = 180;
		int num2 = 35;
		int num3 = 80;
		int num4 = (int)this.txtInfo.ActualHeight + 10 + num3;
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(num + num2 + num4), 1f);
		base.transform.localPosition = new Vector3(0f, (float)((int)((450f - this.Bak.transform.localScale.y) * -0.5f)), 0f);
		this.MenusList.transform.localPosition = new Vector3(this.Menus.transform.localPosition.x, this.Bak.transform.localPosition.y - this.Bak.transform.localScale.y + 20f, 0f);
	}

	private void SetText(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = string.Empty;
		text += goodsXmlNodeByID.Title;
		this.txtTitle.Text = string.Format("{0}", text);
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
		Super.InitGoodsGIcon(this.GoodIcon, goodsData, true, IconTextTypes.Qianghua);
		this.txtInfo.Text = goodsXmlNodeByID.Description;
		if (string.IsNullOrEmpty(this.txtInfo.Text))
		{
			this.txtInfo.Visibility = false;
		}
		else
		{
			this.txtInfo.Visibility = true;
		}
		this.PropsBase.Visibility = true;
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
		this.txtBaseValue.Text = Global.GetBaseAttributeStrFromPropertyList(goodsEquipPropsDoubleList, false, 0);
		this.PropsList.repositionNow = true;
		this.uiCollider.updataNow = true;
		this.SetPropsPanel(goodsOwner);
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
		if (type == TipsOperationTypes.Replace || type == TipsOperationTypes.Xiexia)
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

	public GButton unloadIcon;

	public GButton replaceIcon;

	public TextBlock txtTitle;

	public GGoodIcon GoodIcon;

	public Transform Body;

	public TextBlock txtInfo;

	public UITable PropsList;

	public UICollider uiCollider;

	public SpriteSL PropsBase;

	public TextBlock txtBaseValue;

	public TextBlock txtJichuInfo;
}
