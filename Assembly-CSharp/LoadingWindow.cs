using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;

public class LoadingWindow : GBasePart
{
	public LoadingWindow(int decoCode, double width, double height, string hintText = "", double opacity = 0.01)
	{
		this.Width = width;
		this.Height = height;
		this.Container.Width = width;
		this.Container.Height = height;
		this.Deco = Global.GetDecoration(decoCode, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
		this.Deco.cx = (int)Math.Floor(this.Width - this.Deco.BodyWidth) / 2 + 35;
		this.Deco.cy = (int)Math.Floor(this.Height - this.Deco.BodyHeight) / 2 - (int)(this.Height / 5.0);
		if (hintText != null)
		{
			this.HintTextBlock = new HSTextField(hintText, 65280, -1, 0, -1, -1);
			this.HintTextBlock.name = "Hint";
			this.HintTextBlock.X = Math.Floor((double)this.Deco.cx - this.HintTextBlock.textWidth / 2.0);
			this.HintTextBlock.Y = (double)(this.Deco.cy + 20);
			this.Container.Children.Add(this.HintTextBlock);
		}
	}

	public string HintText
	{
		get
		{
			return (!(null == this.HintTextBlock)) ? this.HintTextBlock.text : string.Empty;
		}
		set
		{
			if (null != this.HintTextBlock)
			{
				this.HintTextBlock.text = value;
			}
		}
	}

	public override void Destroy()
	{
		if (this.Deco != null)
		{
			Global.RemoveObject(this.Deco, true);
			this.Deco = null;
		}
		if (null != this.HintTextBlock)
		{
			this.Container.Children.Remove(this.HintTextBlock, true);
			this.HintTextBlock = null;
		}
	}

	private GDecoration Deco;

	private HSTextField HintTextBlock;
}
