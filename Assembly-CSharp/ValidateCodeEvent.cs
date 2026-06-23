using System;

public class ValidateCodeEvent
{
	public ValidateCodeEvent(string type, bool bubbles = false, bool cancelable = false)
	{
	}

	public string code
	{
		get
		{
			return string.Empty;
		}
	}

	public const string CHANGE = "";
}
