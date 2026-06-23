using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class GSwitchBox : UICheckbox
{
	private void OnClick()
	{
		try
		{
			if (base.enabled)
			{
				this.Check = !this.Check;
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
			if (base.isChecked)
			{
				this._Text.pivot = 5;
				this._Text.text = this.TextOn;
			}
			else
			{
				this._Text.pivot = 3;
				this._Text.text = this.TextOff;
			}
		}
	}

	public bool Visibility { get; set; }

	public string Text { get; set; }

	public string Alignment { get; set; }

	public object Tag { get; set; }

	public UILabel _Text;

	public string TextOn = Global.GetLang("在线挂机");

	public string TextOff = Global.GetLang("离线挂机");

	public BaseEventHandler2 CheckChanged;
}
