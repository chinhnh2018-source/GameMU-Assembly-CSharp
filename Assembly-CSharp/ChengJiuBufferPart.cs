using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ChengJiuBufferPart : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this.ConstTexts != null && this.ConstTexts.Length == 1)
		{
			this.ConstTexts[0].Text = Global.GetLang("加成效果:");
		}
		this.m_txtTitle.text = Global.GetLang("强化效果加成");
		this.m_txtBufferTips.text = Global.GetLang("生命上限");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
	}

	public void RefreshUI(int currentZhuoYueLevel, bool isChengJiu = true)
	{
		int[] array;
		if (isChengJiu)
		{
			array = Global.GetChengJiuBufferGoodsIDs();
		}
		else
		{
			array = Global.GetJunXianBufferGoodsIDs();
		}
		if (currentZhuoYueLevel > 0 && currentZhuoYueLevel <= array.Length)
		{
			int num = array[currentZhuoYueLevel - 1];
			string goodsNameByID = Global.GetGoodsNameByID(num, false);
			this.m_txtTitle.text = Global.GetLang(goodsNameByID);
			string goodsEquipPropsStringForBufferTips = Global.GetGoodsEquipPropsStringForBufferTips(num);
			this.m_txtBufferTips.text = Global.GetLang(goodsEquipPropsStringForBufferTips);
		}
	}

	public TextBlock m_txtBufferTips;

	public TextBlock m_txtTitle;

	public GButton CloseBtn;

	public TextBlock[] ConstTexts;

	public DPSelectedItemEventHandler DPSelectedItem;
}
