using System;
using HSGameEngine.GameEngine.Logic;

public class LingYuItem : UserControl
{
	public string Lingyuname
	{
		get
		{
			return this.lingyuname;
		}
		set
		{
			this.lingyuname = value;
		}
	}

	public int Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
		}
	}

	public string Mode
	{
		get
		{
			return this.mode;
		}
		set
		{
			this.mode = value;
			this.strConfig = this.Mode.Split(new char[]
			{
				','
			});
		}
	}

	public string dealStr(int suit)
	{
		string result = string.Empty;
		if (this.strConfig != null)
		{
			for (int i = 0; i < this.strConfig.Length - 2; i += 3)
			{
				if (suit >= Global.SafeConvertToInt32(this.strConfig[i]) && suit <= Global.SafeConvertToInt32(this.strConfig[i + 1]))
				{
					result = this.strConfig[i + 2];
					break;
				}
			}
		}
		return result;
	}

	public UISprite stat;

	public GGoodIcon icon;

	private string[] strConfig;

	private string lingyuname = string.Empty;

	public float[] attribute = new float[5];

	private int type = 1;

	private string mode = string.Empty;
}
