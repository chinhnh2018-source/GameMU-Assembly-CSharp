using System;
using HSGameEngine.GameEngine.Logic;
using Server.Tools;

public class ZhuanZhiBtnItem : UserControl
{
	public int occupation
	{
		get
		{
			return this._occupation;
		}
		set
		{
			this._occupation = value;
			if (value == 3)
			{
				this.sex = 1;
			}
			this.bak.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
			{
				Global.CalcOriginalOccupationID(value),
				this.sex
			});
		}
	}

	public ShowNetImage bak;

	public UILabel label;

	public UISprite zhiye;

	private int sex;

	private int _occupation = -1;
}
