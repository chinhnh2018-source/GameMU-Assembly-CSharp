using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GCheckBox : UICheckbox
{
	private void OnClick()
	{
		try
		{
			if (base.enabled)
			{
				base.isChecked = !base.isChecked;
			}
			if (this.CheckChanged != null)
			{
				this.CheckChanged(this, BaseEventArgs.Empty);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public bool Check
	{
		get
		{
			return base.isChecked;
		}
		set
		{
			base.isChecked = value;
		}
	}

	public string Text
	{
		set
		{
			if (this._Lable != null)
			{
				this._Lable.text = value;
			}
			else if (base.GetComponentInChildren<UILabel>())
			{
				base.GetComponentInChildren<UILabel>().text = value;
			}
		}
	}

	public bool Visibility { get; set; }

	public bool Onlydouble { get; set; }

	public ImageBrush BodySource { get; set; }

	public ImageBrush NewSource { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public string Name { get; set; }

	public SolidColorBrush TextColor { get; set; }

	public string Content { get; set; }

	public SolidColorBrush Foreground { get; set; }

	public string HorizontalAlignment { get; set; }

	public string TextHorizontalAlignment { get; set; }

	public bool DisableTextCheck { get; set; }

	public object Tag { get; set; }

	public BaseEventHandler2 CheckChanged;
}
