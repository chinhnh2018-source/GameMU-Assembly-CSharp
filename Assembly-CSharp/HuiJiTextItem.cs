using System;

public class HuiJiTextItem : UserControl
{
	public void SetText(string title, string addNumber, bool ImgBool = false)
	{
		this.m_LabTitle.text = title;
		if (ImgBool)
		{
			this.m_LabAddNumber.text = addNumber;
			this.m_Img.gameObject.SetActive(true);
		}
		else
		{
			this.m_LabAddNumber.gameObject.SetActive(false);
			this.m_Img.gameObject.SetActive(false);
		}
	}

	public UILabel m_LabTitle;

	public UILabel m_LabAddNumber;

	public UISprite m_Img;
}
