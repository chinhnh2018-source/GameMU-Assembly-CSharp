using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class OlympicsGuessItem : UserControl
{
	protected override void InitializeComponent()
	{
		NGUITools.SetActive(this.clickBg.gameObject, false);
		this.CheckBoxClickEvent();
	}

	private void CheckBoxClickEvent()
	{
		this.optionA.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SetSelected(0);
		};
		this.optionB.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SetSelected(1);
		};
		this.optionC.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SetSelected(2);
		};
		this.optionD.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SetSelected(3);
		};
	}

	private void SetSelected(int index)
	{
		NGUITools.SetActive(this.clickBg.gameObject, true);
		for (int i = 0; i < this.options.Length; i++)
		{
			if (i == index)
			{
				this.options[i].isChecked = true;
				this.getSingleOption = (index + 1).ToString();
			}
			else
			{
				this.options[i].isChecked = false;
			}
		}
	}

	public string GetSingleOption
	{
		get
		{
			return this.getSingleOption;
		}
		set
		{
			this.getSingleOption = value;
		}
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
				Global.GetLang("：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"bdbdbd",
				data.Content
			})
		});
		this.score.Text = data.Grade.ToString();
		this.A.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.A
		});
		this.B.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.B
		});
		this.C.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.C
		});
		this.D.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.D
		});
		if (data.Select != -1)
		{
			this.SetSelected(data.Select - 1);
			this.SetCheckBoxsUnenable();
		}
		else
		{
			for (int i = 0; i < this.options.Length; i++)
			{
				this.options[i].isChecked = false;
			}
		}
	}

	public void SetCheckBoxsUnenable()
	{
		this.optionA.GetComponent<BoxCollider>().enabled = false;
		this.optionB.GetComponent<BoxCollider>().enabled = false;
		this.optionC.GetComponent<BoxCollider>().enabled = false;
		this.optionD.GetComponent<BoxCollider>().enabled = false;
	}

	public UISprite clickBg;

	public GCheckBox optionA;

	public GCheckBox optionB;

	public GCheckBox optionC;

	public GCheckBox optionD;

	public GCheckBox[] options;

	public TextBlock content;

	public TextBlock score;

	public TextBlock A;

	public TextBlock B;

	public TextBlock C;

	public TextBlock D;

	private string getSingleOption;
}
