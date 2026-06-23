using System;

public class HongBaoQuanMinItem : UserControl
{
	public void NextHongBao()
	{
		NGUITools.SetActive(this.m_SpBack.gameObject, true);
		NGUITools.SetActive(this.m_SpXiaJianTou.gameObject, true);
	}

	public bool EndBool
	{
		get
		{
			return this.m_EndBool;
		}
		set
		{
			this.m_EndBool = value;
			if (value)
			{
				this.m_Img.URL = this.PATH + "HongBaoQuanMinItem_Kai.png";
			}
			else
			{
				this.m_Img.URL = this.PATH + "HongBaoQuanMinItem.png";
			}
		}
	}

	public ShowNetImage m_Img;

	public UILabel m_Time;

	public UISprite m_SpBack;

	public UISprite m_SpXiaJianTou;

	private string PATH = "NetImages/GameRes/Images/HongBao/";

	private bool m_EndBool;
}
