using System;

public class GoodsMovingEvent
{
	public GoodsMovingEvent(string type, object tag, bool bubbles = false, bool cancelable = false)
	{
		this._Tag = tag;
	}

	public object Tag
	{
		get
		{
			return this._Tag;
		}
		set
		{
			this._Tag = value;
		}
	}

	public static string GoodsMovingEnd = "GoodsMovingEnd";

	private object _Tag;
}
