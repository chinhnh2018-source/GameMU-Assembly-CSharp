using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;

public class MUVipBuffPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.chongzhiBtn.Text = Global.GetLang("充值");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_ItemOBC = this.m_ListItems.ItemsSource;
		this.vipLeveText.Text = Global.GetVIPLeve().ToString();
		this.chongzhiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 10
				});
			}
		};
		this.VipGoodsIDArr = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuanhuangBufferGoodsIDs", ',');
		this.InitItemList();
	}

	private void InitItemList()
	{
		this.m_ItemOBC.Clear();
		this.ItemList.Clear();
		for (int i = 1; i < this.VipGoodsIDArr.Length; i++)
		{
			MUVipPropertyLevelItem muvipPropertyLevelItem = U3DUtils.NEW<MUVipPropertyLevelItem>();
			this.m_ItemOBC.Add(muvipPropertyLevelItem);
			this.ItemList.Add(muvipPropertyLevelItem);
			muvipPropertyLevelItem.refreshUI(this.VipGoodsIDArr[i], i);
		}
	}

	public void RefreshItemEnableState()
	{
		int count = this.ItemList.Count;
		for (int i = 0; i < count; i++)
		{
			this.ItemList[i].RefreshEnable();
		}
	}

	public void SetnVipExp(int exp, int flag)
	{
		int num = Global.GetVIPLeve() + 1;
		if (num > MUVipPart.MaxVipLevel)
		{
			num = MUVipPart.MaxVipLevel;
		}
		long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("ZuanshiVIPExp");
		XElement xelement = Global.GetXElement(this.xml, "Item", "VIPLevel", Convert.ToString(num));
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedExp");
		int num2 = (int)((long)xelementAttributeInt * systemParamIntByName);
		this.jinduText.Text = string.Format("{0}/{1}", (exp <= xelementAttributeInt) ? exp : xelementAttributeInt, xelementAttributeInt);
		this.levelUpinfoText.Text = string.Format(Global.GetLang("累计充值{{00FF00}}{0}钻石{{-}}，即可升级到{{00FF00}}VIP{1}{{-}}"), num2, num);
		this.LevProgBar.Percent = ((exp <= xelementAttributeInt) ? ((double)exp / (double)xelementAttributeInt) : 1.0);
		if (Global.GetVIPLeve() == 0)
		{
			this.InitVIPZongLanPartData(0);
		}
		else
		{
			this.InitVIPZongLanPartData(Global.GetVIPLeve());
		}
	}

	private void InitVIPZongLanPartData(int leve)
	{
		int num = leve;
		if (num == 0)
		{
			num = 1;
		}
		else if (num >= 12)
		{
			num = 12;
		}
		this.vipLeveText2.Text = num.ToString();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock vipLeveText;

	public TextBlock vipLeveText2;

	public TextBlock levelUpinfoText;

	public TextBlock jinduText;

	public GImgProgressBar LevProgBar;

	public GButton chongzhiBtn;

	public XElement xml;

	private ObservableCollection m_ItemOBC;

	public ListBox m_ListItems = new ListBox();

	public List<MUVipPropertyLevelItem> ItemList = new List<MUVipPropertyLevelItem>();

	private int[] VipGoodsIDArr;
}
