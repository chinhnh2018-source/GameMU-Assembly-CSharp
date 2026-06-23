using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class OlympicsYesterdayRecordItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void SetValue(int index, OlympicsGuessData data)
	{
		this.content.Text = string.Format("{0}{1}{2}{3}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("竞猜")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				index + 1
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				":"
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				data.Content
			})
		});
		this.answer.Text = this.GetShowContent(data);
	}

	private string GetShowContent(OlympicsGuessData data)
	{
		string result;
		if (data.Select == -1)
		{
			result = string.Format("{0}{1}{2}{3}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("选择：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("未作答")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("(正确答案 ")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.GetAnswer(data.Answer, data) + ")"
				})
			});
		}
		else if (data.Select == data.Answer)
		{
			result = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("选择：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					this.GetAnswer(data.Select, data)
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("(正确答案，获得")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					data.Grade
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("积分)")
				})
			});
		}
		else
		{
			result = string.Format("{0}{1}{2}{3}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("选择：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.GetAnswer(data.Select, data)
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("(正确答案 ")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.GetAnswer(data.Answer, data) + ")"
				})
			});
		}
		return result;
	}

	private string GetAnswer(int _answer, OlympicsGuessData data)
	{
		switch (_answer + 1)
		{
		case 0:
			return Global.GetLang("未公布");
		case 2:
			return data.A;
		case 3:
			return data.B;
		case 4:
			return data.C;
		case 5:
			return data.D;
		}
		return null;
	}

	public TextBlock content;

	public TextBlock answer;
}
