using System;

public class ShenShiPartFuWenpeidaiitem : UserControl
{
	public bool setPeiDai
	{
		set
		{
			this.peidai.GetComponent<UITexture>().enabled = value;
		}
	}

	public bool XuanZhong
	{
		set
		{
			if (value)
			{
				this.guangquan.enabled = true;
			}
			else
			{
				this.guangquan.enabled = false;
			}
		}
	}

	public int goodsID
	{
		get
		{
			return this.goodid;
		}
		set
		{
			this.tubiao.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", value);
			this.goodid = value;
		}
	}

	public new int Count
	{
		set
		{
			this.count.text = value.ToString();
		}
	}

	public ShowNetImage tubiao;

	public UILabel count;

	public UISprite guangquan;

	public ShowNetImage peidai;

	private int goodid;
}
