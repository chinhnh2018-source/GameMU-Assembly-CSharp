using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class TextBox : UIInput
{
	public GameObject cachedGameObject
	{
		get
		{
			if (this.mGo == null)
			{
				this.mGo = base.gameObject;
			}
			return this.mGo;
		}
	}

	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	public void Focus()
	{
		base.selected = true;
	}

	public void NotFocus()
	{
		base.selected = false;
	}

	protected override void Update()
	{
		if (Input.GetKeyDown(9))
		{
			if (Time.time < this.LastEditKeyTime + 0.5f)
			{
				return;
			}
			this.LastEditKeyTime = Time.time;
			this.text += ClipboardHelper.clipBoard;
		}
		base.Update();
		if (TextBox.LastSelectedGo != this.cachedGameObject)
		{
			if (UICamera.selectedObject == this.cachedGameObject)
			{
				TextBox.LastSelectedGo = this.cachedGameObject;
				if (this.GotFocus != null)
				{
					this.GotFocus.Invoke(this, EventArgs.Empty);
				}
			}
		}
		else if (TextBox.LastSelectedGo == this.cachedGameObject && UICamera.selectedObject != this.cachedGameObject)
		{
			TextBox.LastSelectedGo = null;
			if (this.LostFocus != null)
			{
				this.LostFocus.Invoke(this, EventArgs.Empty);
			}
		}
	}

	private void OnInputChanged()
	{
		if (this.TextChanged != null)
		{
			this.TextChanged.Invoke(this, EventArgs.Empty);
		}
	}

	private void OnSubmit()
	{
		if (this.TextBoxChanged != null)
		{
			this.TextBoxChanged.Invoke(this, EventArgs.Empty);
		}
	}

	public string Text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
		}
	}

	public int FontSize { get; set; }

	public double Height { get; set; }

	public double Width { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public uint textColor { get; set; }

	public Thickness Padding { get; set; }

	public SolidColorBrush Background { get; set; }

	public SolidColorBrush CaretBrush { get; set; }

	public SolidColorBrush Foreground { get; set; }

	public bool mouseEnabled { get; set; }

	public bool selectable { get; set; }

	public bool TextWrapping { get; set; }

	public bool AcceptsReturn { get; set; }

	public bool background { get; set; }

	public bool border { get; set; }

	public int HorizontalScrollBarVisibility { get; set; }

	public int VerticalScrollBarVisibility { get; set; }

	public string restrict { get; set; }

	public int caretIndex { get; set; }

	public int selectionBeginIndex { get; set; }

	public int selectionEndIndex { get; set; }

	public FontFamilySL FontFamily { get; set; }

	public int getCharIndexAtPoint(double x, double y)
	{
		return 0;
	}

	public void setSelection(int start, int end)
	{
	}

	public void setTextFormat(TextFormat format)
	{
	}

	public bool TextFontWrapping { get; set; }

	public uint borderColor { get; set; }

	public uint backgroundColor { get; set; }

	public bool Visibility { get; set; }

	public void SetGoodsText(string goodsText, int offset)
	{
	}

	public string getAppendGoodsText(int addIndex = 0)
	{
		return string.Empty;
	}

	public void addText(string str, object tag, uint color, bool underLine = true)
	{
	}

	public EventHandler GotFocus;

	public EventHandler LostFocus;

	public EventHandler TextChanged;

	public EventHandler TextBoxChanged;

	public GameObject SendObg;

	private GameObject mGo;

	private Transform mTrans;

	private float LastEditKeyTime;

	private static GameObject LastSelectedGo;

	public int textHeight;

	private List<string> tagArray = new List<string>();
}
