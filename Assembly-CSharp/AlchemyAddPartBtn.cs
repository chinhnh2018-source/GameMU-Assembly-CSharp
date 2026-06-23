using System;
using HSGameEngine.GameFramework.Logic;

public class AlchemyAddPartBtn : UserControl
{
	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
			if (this.type < 100)
			{
				this.GoodType.URL = string.Format("NetImages/GameRes/Images/Plate/AlchemyHuoBi/{0}.png.qj", value);
			}
			else
			{
				this.GoodType.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", value);
			}
		}
	}

	public string BtnName
	{
		set
		{
			this.lable.text = value;
		}
	}

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	public bool IsEnabled
	{
		set
		{
			if (this.GoodType)
			{
				this.GoodType.ToGrayBitmap = !value;
			}
		}
	}

	public DPSelectedItemEventHandler DPSHandler;

	public ShowNetImage GoodType;

	public UILabel lable;

	public UISprite bak;

	private int type;

	private int index;
}
