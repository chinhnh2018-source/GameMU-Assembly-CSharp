using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

public class MUVipPropertyLevelItem : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.LabelArr != null && this.LabelArr.Count == 7)
		{
			this.LabelArr[0].Text = Global.GetLang("魔法防御:");
			this.LabelArr[1].Text = Global.GetLang("魔法攻击:");
			this.LabelArr[2].Text = Global.GetLang("闪       避:");
			this.LabelArr[3].Text = Global.GetLang("命       中:");
			this.LabelArr[4].Text = Global.GetLang("生命上限:");
			this.LabelArr[5].Text = Global.GetLang("物理防御:");
			this.LabelArr[6].Text = Global.GetLang("物理攻击:");
			for (int i = 0; i <= 6; i++)
			{
				this.LabelArr[i].Pivot = 5;
				this.LabelArr[i].X = -26.0;
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void refreshUI(int GoodId, int index)
	{
		this.CurrentIndex = index;
		this.m_TextureTitle.URL = "NetImages/GameRes/Images/VipLevel/" + index + ".png";
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(GoodId);
		this.m_WuGong.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[7], goodsEquipPropsDoubleList[8]);
		this.m_WuFang.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[3], goodsEquipPropsDoubleList[4]);
		this.m_MoGong.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[9], goodsEquipPropsDoubleList[10]);
		this.m_MoFang.Text = string.Format("{0}-{1}", goodsEquipPropsDoubleList[5], goodsEquipPropsDoubleList[6]);
		this.m_ShanBi.Text = string.Format("{0}", goodsEquipPropsDoubleList[19]);
		this.m_ShengMin.Text = string.Format("{0}", goodsEquipPropsDoubleList[13]);
		this.m_MingZhong.Text = string.Format("{0}", goodsEquipPropsDoubleList[18]);
		this.RefreshEnable();
	}

	public void RefreshEnable()
	{
		if (this.CurrentIndex > Global.GetVIPLeve())
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
			this.m_MingZhong.textColor = 14337966U;
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
			this.m_MingZhong.textColor = 12895428U;
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

	public TextBlock m_MingZhong;

	public List<TextBlock> LabelArr;

	public int CurrentIndex = -1;
}
