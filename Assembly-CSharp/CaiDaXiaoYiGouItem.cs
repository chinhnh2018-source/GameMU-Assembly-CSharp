using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CaiDaXiaoYiGouItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnJia.Text = Global.GetLang("加注");
		base.InitializeComponent();
		this.btnJia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerNumber(this, new DPSelectedItemEventArgs
			{
				Tag = this
			});
		};
	}

	public int Number
	{
		get
		{
			return this.m_Number;
		}
		set
		{
			this.m_Number = value;
			this.labNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				value
			});
		}
	}

	public DiceValueEnum ValueEnum
	{
		get
		{
			return this.m_ValueEnum;
		}
		set
		{
			this.m_ValueEnum = value;
			if (this.ValueEnum == DiceValueEnum.DiceMin)
			{
				this.spEnum.spriteName = "xiao_t";
			}
			else if (this.ValueEnum == DiceValueEnum.DiceMax)
			{
				this.spEnum.spriteName = "da_t";
			}
			else if (this.ValueEnum == DiceValueEnum.DiceLeopard)
			{
				this.spEnum.spriteName = "baozi_t";
				this.spEnum.transform.localScale = new Vector3(40f, 20f, 1f);
			}
		}
	}

	public UILabel labNumber;

	public UISprite spEnum;

	public GButton btnJia;

	public DPSelectedItemEventHandler handlerNumber;

	private int m_Number = -1;

	private DiceValueEnum m_ValueEnum = DiceValueEnum.DiceMin;
}
