using System;
using HSGameEngine.GameEngine.Logic;

public class YaoSaiXinXiPartFuLuItem : UserControl
{
	public string SetTouXiang
	{
		set
		{
			this.TouXiang.URL = value;
		}
	}

	public string SetName
	{
		set
		{
			this.Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				value
			});
		}
	}

	public string SetSever
	{
		set
		{
			this.Sever.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				value
			});
		}
	}

	public ShowNetImage TouXiang;

	public new UILabel Name;

	public UILabel Sever;
}
