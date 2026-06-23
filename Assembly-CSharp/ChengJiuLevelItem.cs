using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

public class ChengJiuLevelItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_ConstWuGong.Text = Global.GetLang("物理攻击:");
		this.m_ConstMoGong.Text = Global.GetLang("魔法攻击:");
		this.m_ConstWuFang.Text = Global.GetLang("物理防御:");
		this.m_ConstMoFang.Text = Global.GetLang("魔法防御:");
		this.m_ConstShengMin.Text = Global.GetLang("生命上限:");
		this.m_ConstShanBi.Text = Global.GetLang("闪       避:");
		this.m_ConstShangHaiJianMian.Text = Global.GetLang("伤害吸收:");
		this.m_ConstWuGong.Pivot = 5;
		this.m_ConstMoGong.Pivot = 5;
		this.m_ConstWuFang.Pivot = 5;
		this.m_ConstMoFang.Pivot = 5;
		this.m_ConstShengMin.Pivot = 5;
		this.m_ConstShanBi.Pivot = 5;
		this.m_ConstShangHaiJianMian.Pivot = 5;
		this.m_ConstWuGong.X = -25.0;
		this.m_ConstMoGong.X = -25.0;
		this.m_ConstWuFang.X = -25.0;
		this.m_ConstMoFang.X = -25.0;
		this.m_ConstShengMin.X = -25.0;
		this.m_ConstShanBi.X = -25.0;
		this.m_ConstShangHaiJianMian.X = -25.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void refreshUI(int GoodId, int index)
	{
		this.CurrentIndex = index;
		this.m_TextureTitle.URL = "NetImages/GameRes/Images/ChengJiuLevel/cj_" + GoodId + ".png";
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(GoodId);
		this.m_WuGong.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[7], goodsEquipPropsDoubleList[8]);
		this.m_WuFang.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[3], goodsEquipPropsDoubleList[4]);
		this.m_MoGong.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[9], goodsEquipPropsDoubleList[10]);
		this.m_MoFang.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[5], goodsEquipPropsDoubleList[6]);
		this.m_ShanBi.Text = string.Format("{0}", goodsEquipPropsDoubleList[19]);
		this.m_ShengMin.Text = string.Format("{0}", goodsEquipPropsDoubleList[13]);
		this.m_ShangHaiJianMian.Text = string.Format("{0}%", goodsEquipPropsDoubleList[24] * 100.0);
		this.RefreshEnable();
	}

	public void RefreshEnable()
	{
		if (this.CurrentIndex + 1 > Global.GetChengJiuLevel(0))
		{
			this.SetEnable(false);
		}
		else
		{
			this.SetEnable(true);
		}
	}

	public void SetEnable(bool isEnable)
	{
		if (isEnable)
		{
			this.m_TextureTitle.ToGrayBitmap = false;
			this.m_TextureBak.ToGrayBitmap = false;
			this.m_WuGong.textColor = 14337966U;
			this.m_MoGong.textColor = 14337966U;
			this.m_MoFang.textColor = 14337966U;
			this.m_WuFang.textColor = 14337966U;
			this.m_ShengMin.textColor = 14337966U;
			this.m_ShanBi.textColor = 14337966U;
			this.m_ShangHaiJianMian.textColor = 14337966U;
			int count = this.LabelArr.Count;
			for (int i = 0; i < count; i++)
			{
				this.LabelArr[i].textColor = 4996647U;
			}
		}
		else
		{
			this.m_TextureTitle.ToGrayBitmap = true;
			this.m_TextureBak.ToGrayBitmap = true;
			this.m_WuGong.textColor = 12895428U;
			this.m_MoGong.textColor = 12895428U;
			this.m_MoFang.textColor = 12895428U;
			this.m_WuFang.textColor = 12895428U;
			this.m_ShengMin.textColor = 12895428U;
			this.m_ShanBi.textColor = 12895428U;
			this.m_ShangHaiJianMian.textColor = 12895428U;
			int count2 = this.LabelArr.Count;
			for (int j = 0; j < count2; j++)
			{
				this.LabelArr[j].textColor = 3750201U;
			}
		}
	}

	public ShowNetImage m_TextureTitle;

	public ShowNetImage m_TextureBak;

	public TextBlock m_WuGong;

	public TextBlock m_MoGong;

	public TextBlock m_WuFang;

	public TextBlock m_MoFang;

	public TextBlock m_ShengMin;

	public TextBlock m_ShanBi;

	public TextBlock m_ShangHaiJianMian;

	public List<TextBlock> LabelArr;

	public TextBlock m_ConstWuGong;

	public TextBlock m_ConstMoGong;

	public TextBlock m_ConstWuFang;

	public TextBlock m_ConstMoFang;

	public TextBlock m_ConstShengMin;

	public TextBlock m_ConstShanBi;

	public TextBlock m_ConstShangHaiJianMian;

	public int CurrentIndex = -1;
}
