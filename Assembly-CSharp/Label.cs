using System;

public class Label : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public UILabel m_Label;

	public UILabel m_LblNextNodeProperty;

	public UISprite m_SprUp;

	public string m_strPropertyName = string.Empty;

	public string m_strNextPropertyValue = string.Empty;

	public string m_strPropertyValue = string.Empty;
}
