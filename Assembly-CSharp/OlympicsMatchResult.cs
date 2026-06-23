using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class OlympicsMatchResult : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnConfirm.Text = Global.GetLang("确定");
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.btnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void SetContent(int isWin, int grade, int type, int score)
	{
		if (isWin == 1)
		{
			this.title.Text = Global.GetLang("获胜");
			this.contentUp.Text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("胜利获得：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"bdbdbd",
				score
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"bdbdbd",
				Global.GetLang("积分")
			}));
			if (type == 1)
			{
				this.contentDown.Text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("最终环数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					grade
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					Global.GetLang("环")
				}));
			}
			else if (type == 2)
			{
				this.contentDown.Text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("进球个数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					grade
				}));
			}
		}
		else
		{
			this.title.Text = Global.GetLang("失败");
			this.contentUp.Text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("失败获得：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"bdbdbd",
				score
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"bdbdbd",
				Global.GetLang("积分")
			}));
			if (type == 1)
			{
				this.contentDown.Text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("最终环数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					grade
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					Global.GetLang("环")
				}));
			}
			else if (type == 2)
			{
				this.contentDown.Text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("进球个数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"bdbdbd",
					grade
				}));
			}
		}
	}

	public GButton btnClose;

	public GButton btnConfirm;

	public TextBlock title;

	public TextBlock contentUp;

	public TextBlock contentDown;

	public DPSelectedItemEventHandler Hander;
}
