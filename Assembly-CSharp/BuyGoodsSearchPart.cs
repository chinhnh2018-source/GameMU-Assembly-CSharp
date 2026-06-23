using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class BuyGoodsSearchPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_CBWrite.Text = Global.GetLang("白色");
		this.m_CBGreen.Text = Global.GetLang("绿色");
		this.m_CBBlue.Text = Global.GetLang("蓝色");
		this.m_CBPurple.Text = Global.GetLang("紫色");
		this.m_BtnSearch.Text = Global.GetLang("搜索");
		this.TxtGoodsName.Text = Global.GetLang("物品名称");
		this.TxtEquipQuality.Text = Global.GetLang("装备品质");
		this.TxtEquipQuality.X = -95.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.m_BtnSearch.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = this.m_InputText.Text;
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				foreach (KeyValuePair<int, GoodVO> keyValuePair in ConfigGoods.GoodsXmlNodeDict)
				{
					if (keyValuePair.Value.Title.IndexOf(text) >= 0)
					{
						stringBuilder.Append(keyValuePair.Key);
						stringBuilder.Append("#");
					}
				}
				if (string.IsNullOrEmpty(stringBuilder.ToString().Trim()))
				{
					Super.HintMainText(Global.GetLang("输入查询的名称不正确"), 10, 3);
					return;
				}
			}
			int num = 0;
			if (this.m_CBWrite.Check)
			{
				num++;
			}
			if (this.m_CBGreen.Check)
			{
				num += 2;
			}
			if (this.m_CBBlue.Check)
			{
				num += 4;
			}
			if (this.m_CBPurple.Check)
			{
				num += 8;
			}
			if (string.IsNullOrEmpty(text.Trim()) && num == 0)
			{
				Super.HintMainText(Global.GetLang("请输入查询的名称或选择查询品质"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Quality = num,
				Title = stringBuilder.ToString()
			});
		};
	}

	public GButton m_CloseBtn;

	public GButton m_BtnSearch;

	public GCheckBox m_CBWrite;

	public GCheckBox m_CBGreen;

	public GCheckBox m_CBBlue;

	public GCheckBox m_CBPurple;

	public TextBox m_InputText;

	public TextBlock TxtGoodsName;

	public TextBlock TxtEquipQuality;

	public DPSelectedItemEventHandler DPSelectedItem;
}
