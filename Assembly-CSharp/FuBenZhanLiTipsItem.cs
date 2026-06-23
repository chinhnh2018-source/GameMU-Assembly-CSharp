using System;

public class FuBenZhanLiTipsItem : UserControl
{
	public string Img
	{
		set
		{
			this.img.URL = "NetImages/GameRes/Images/BianQiang/" + value + ".png";
		}
	}

	public ShowNetImage img;

	public TiShengZhanLiItemVO ItemVO;
}
