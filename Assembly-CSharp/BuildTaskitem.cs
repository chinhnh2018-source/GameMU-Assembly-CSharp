using System;
using HSGameEngine.GameEngine.Logic;

public class BuildTaskitem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this._WanChengJIangliLabel)
		{
			this._WanChengJIangliLabel.text = BuildFintColor.Yellow + Global.GetLang("完成奖励") + BuildFintColor.End;
		}
		this._ZhiXingBtn.Label.text = Global.GetLang("执行");
	}

	public void SetStar(byte Quality)
	{
		for (byte b = 0; b < 4; b += 1)
		{
			if (b < Quality)
			{
				this._TaskStars[(int)b].spriteName = "Star0";
			}
			else
			{
				this._TaskStars[(int)b].spriteName = "Star1";
			}
		}
	}

	public void SetAwarsSign(int Title)
	{
		string spriteName = "BuildTaskExp";
		string spriteName2 = "TaskGold";
		this._AwardSign[0].spriteName = spriteName;
		this._AwardSign[1].spriteName = BuildMainPart.GetLdHintTitileName()[Title - 1];
		this._AwardSign[2].spriteName = spriteName2;
	}

	public void SetAwardLabel(BuildAwardInF Award, double sum, double ExpNum, int needtime, string color, int NextLevNeedExp, bool bMaxLev = false, string SignContent = "完成后可以让建筑升级")
	{
		this._AwardLabel[0].text = Global.GetLang(string.Concat(new string[]
		{
			(Award.Exp * needtime).ToString(),
			color,
			"  X",
			this.WipeOffStrLast0(ExpNum.ToString()),
			"{-}"
		}));
		this._AwardLabel[1].text = Global.GetLang(string.Concat(new string[]
		{
			(Award.BuildAward_A * (float)needtime).ToString(),
			color,
			"  X",
			this.WipeOffStrLast0((sum - ExpNum).ToString()),
			"{-}"
		}));
		this._AwardLabel[2].text = Global.GetLang(string.Concat(new string[]
		{
			(Award.Money * needtime).ToString(),
			color,
			"  X",
			this.WipeOffStrLast0((sum - ExpNum).ToString()),
			"{-}"
		}));
		if (bMaxLev)
		{
			NGUITools.SetActive(this._Signlabel, false);
		}
		else if ((double)NextLevNeedExp < (double)Award.Exp * ExpNum * (double)needtime)
		{
			NGUITools.SetActive(this._Signlabel, true);
			this._Signlabel.text = "{2cd802}" + Global.GetLang(SignContent) + "{-}";
		}
		else
		{
			NGUITools.SetActive(this._Signlabel, false);
		}
	}

	private string WipeOffStrLast0(string str)
	{
		for (int i = str.Length - 1; i >= 0; i--)
		{
			if ('\0' < str.get_Chars(i))
			{
				break;
			}
			str.Remove(i);
		}
		return str;
	}

	public int TaskID
	{
		get
		{
			return this._TaskID;
		}
		set
		{
			this._TaskID = value;
		}
	}

	public UILabel _TaskName;

	public UISprite[] _TaskStars;

	public UISprite[] _AwardSign;

	public UILabel[] _AwardLabel;

	public UILabel _Signlabel;

	public UILabel _NeedTime;

	public GButton _ZhiXingBtn;

	public UILabel _WanChengJIangliLabel;

	private int _TaskID;
}
