using System;
using HSGameEngine.GameEngine.Logic;

public class RequestResultPart : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void SetTxt(string txt)
	{
		this.ResultAttr.text = string.Format(Global.GetLang("{0}拒绝了您的求婚"), txt);
	}

	public UILabel ResultAttr;
}
