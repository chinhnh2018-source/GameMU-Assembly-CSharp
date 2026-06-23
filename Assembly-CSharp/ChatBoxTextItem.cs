using System;
using UnityEngine;

public class ChatBoxTextItem : MonoBehaviour
{
	public string Text
	{
		get
		{
			return this.TextBlockEx.Text;
		}
		set
		{
			this.TextBlockEx.Text = value;
		}
	}

	public GTextBlockEx TextBlockEx;
}
