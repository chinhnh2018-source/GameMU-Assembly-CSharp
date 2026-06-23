using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class NumberKeyboardPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
		};
		this.BtnDelete.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DeleteText();
		};
		this.BtnDelete.OnPress = delegate(GameObject s, bool b)
		{
			if (b)
			{
				base.InvokeRepeating("DeleteText", 0.1f, 0.1f);
			}
			else
			{
				base.CancelInvoke("DeleteText");
			}
		};
		this.BtnEnter.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				ID = ConvertExt.SafeConvertToInt32(this.LabelNum.text)
			});
		};
		this.BtnZero.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("0");
		};
		this.BtnOne.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("1");
		};
		this.BtnTwo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("2");
		};
		this.BtnThree.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("3");
		};
		this.BtnFour.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("4");
		};
		this.BtnFive.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("5");
		};
		this.BtnSix.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("6");
		};
		this.BtnSeven.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("7");
		};
		this.BtnEight.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("8");
		};
		this.BtnNine.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetText("9");
		};
	}

	public void RefreshUI(string value)
	{
		this.LabelNum.text = value;
	}

	private void SetText(string value)
	{
		if (this.LabelNum.text == null)
		{
			this.LabelNum.text = value;
		}
		else
		{
			this.LabelNum.text = ConvertExt.SafeConvertToInt32(this.LabelNum.text + value).ToString();
		}
	}

	private void DeleteText()
	{
		if (this.LabelNum.text == null)
		{
			this.LabelNum.text = "0";
		}
		else
		{
			string text = this.LabelNum.text;
			if (text.Length > 1)
			{
				this.LabelNum.text = ConvertExt.SafeConvertToInt32(text.Substring(0, text.Length - 1)).ToString();
			}
			else
			{
				this.LabelNum.text = "0";
			}
		}
	}

	public GButton BtnDelete;

	public GButton BtnEnter;

	public GButton BtnZero;

	public GButton BtnOne;

	public GButton BtnTwo;

	public GButton BtnThree;

	public GButton BtnFour;

	public GButton BtnFive;

	public GButton BtnSix;

	public GButton BtnSeven;

	public GButton BtnEight;

	public GButton BtnNine;

	public GButton CloseBtn;

	public UILabel LabelNum;

	public DPSelectedItemEventHandler DPSelectedItem;
}
