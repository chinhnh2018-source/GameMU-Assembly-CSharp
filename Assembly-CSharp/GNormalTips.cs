using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GNormalTips : UserControl
{
	protected override void InitializeComponent()
	{
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
	}

	public void RenderTips(string titleStr, string contentStr)
	{
		this.SetText(titleStr, contentStr);
		this.SetPropsPanel();
	}

	private void SetText(string titleStr, string contentStr)
	{
		this.txtTitle.Text = titleStr;
		this.txtResult.Text = contentStr;
	}

	private void SetPropsPanel()
	{
		int num = 50;
		int num2 = Mathf.Max((int)this.txtResult.ActualHeight, 220);
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(num2 + num), 1f);
		base.transform.localPosition = new Vector3(0f, (float)((int)((450f - this.Bak.transform.localScale.y) * -0.5f)), 0f);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite Bak;

	public TextBlock txtTitle;

	public TextBlock txtResult;

	public GButton CloseBtn;
}
