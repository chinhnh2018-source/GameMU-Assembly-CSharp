using System;

public class PlayGameContolEvent
{
	public PlayGameContolEvent(string type, object myArg)
	{
		this._type = type;
		this.argu = myArg;
	}

	public string type
	{
		get
		{
			return this._type;
		}
		set
		{
			this._type = value;
		}
	}

	public PlayGameContolEvent clone()
	{
		return new PlayGameContolEvent(this._type, this.argu);
	}

	public object argu = string.Empty;

	private string _type;
}
