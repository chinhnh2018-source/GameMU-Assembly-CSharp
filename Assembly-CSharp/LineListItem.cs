using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class LineListItem : GBasePart
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitControls();
	}

	public override void Destroy()
	{
		this.CleanUpChildWindows();
	}

	protected override void InitControls()
	{
		this.Container.Children.Add(this.BakRect1);
		this.BakRect1.Width = 159.0;
		this.BakRect1.Height = 36.0;
		this.BakRect1.Cursor = Cursors.Hand;
		Canvas.SetLeft(this.BakRect1, 0);
		Canvas.SetTop(this.BakRect1, 0);
		this.BakRect1.Visibility = true;
		this.Container.Children.Add(this.BakRect2);
		this.BakRect2.Width = 159.0;
		this.BakRect2.Height = 36.0;
		this.BakRect2.Cursor = Cursors.Hand;
		Canvas.SetLeft(this.BakRect2, 0);
		Canvas.SetTop(this.BakRect2, 0);
		this.BakRect2.Visibility = false;
		this._textBlock = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		this._textBlock.FontSize = FontSizeMgr.LineItemTextFontSize;
		this._textBlock.Cursor = Cursors.Hand;
		Canvas.SetLeft(this._textBlock, 33);
		Canvas.SetTop(this._textBlock, 12);
		this.Container.Children.Add(this._textBlock);
		this.BakRect1.doubleClickEnabled = true;
		this.BakRect2.doubleClickEnabled = true;
		this._textBlock.doubleClickEnabled = true;
		this._textBlock.IsHitTestVisible = false;
		base.addEventListener("ROLL_OVER", new MouseEventHandler(this.UserControl_MouseEnter));
		base.addEventListener("ROLL_OUT", new MouseEventHandler(this.UserControl_MouseLeave));
	}

	public override void CleanUpChildWindows()
	{
		this.removeEventListener("ROLL_OVER", new MouseEventHandler(this.UserControl_MouseEnter));
		this.removeEventListener("ROLL_OUT", new MouseEventHandler(this.UserControl_MouseLeave));
	}

	public GTextBlockOutLine LineTextBlock
	{
		get
		{
			return this._textBlock;
		}
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (this._SelectedState)
			{
				this.BakRect1.Visibility = false;
				this.BakRect2.Visibility = true;
			}
			else
			{
				this.BakRect1.Visibility = true;
				this.BakRect2.Visibility = false;
			}
		}
	}

	public string Tip
	{
		get
		{
			return this._Tip;
		}
		set
		{
			this._Tip = value;
		}
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		this.BakRect1.Visibility = false;
		this.BakRect2.Visibility = true;
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (!this.SelectedState)
		{
			this.BakRect1.Visibility = true;
			this.BakRect2.Visibility = false;
		}
	}

	private void removeEventListener(string eventName, MouseEventHandler handler)
	{
	}

	private RectangleSL BakRect1 = new RectangleSL();

	private RectangleSL BakRect2 = new RectangleSL();

	private GTextBlockOutLine _textBlock;

	private bool _SelectedState;

	private string _Tip;
}
