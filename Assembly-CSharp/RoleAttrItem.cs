using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class RoleAttrItem : UserControl
{
	public int addValue
	{
		get
		{
			return this._addValue;
		}
		set
		{
			this._addValue = value;
			if (this._addValue <= 0)
			{
				this.AttrAddV.gameObject.SetActive(false);
				this.SubBtn.gameObject.SetActive(false);
				this.inputBak.gameObject.SetActive(false);
			}
			else
			{
				this.AttrAddV.Text = string.Format("+{0}", this._addValue);
				this.AttrAddV.gameObject.SetActive(true);
				this.SubBtn.gameObject.SetActive(true);
				this.inputBak.gameObject.SetActive(true);
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.SubBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.AddBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		UIEventListener.Get(this.inputBak.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DPSelectedItemNum, null, 200, -100);
		};
		this.DPSelectedItemNum = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = id
				});
			}
		};
		UIEventListener.Get(this.Bak.gameObject).onPress = delegate(GameObject s, bool b)
		{
			if (b)
			{
				this.DPSelectedItemTips(this, new DPSelectedItemEventArgs
				{
					ID = this.PropIndexes
				});
			}
			else
			{
				this.DPSelectedItemTips(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
	}

	public bool ImgVisible
	{
		set
		{
			this.Img.gameObject.SetActive(value);
			this.Bak.gameObject.SetActive(!value);
		}
	}

	public UISprite icon;

	public UISprite inputBak;

	public GButton SubBtn;

	public GButton AddBtn;

	public TextBlock Title;

	public TextBlock AttrV;

	public TextBlock AttrAddV;

	public int PropIndexes;

	public GImgProgressBar ProgressBar;

	public TextBlock ProgressText;

	public UISprite Bak;

	public ShowNetImage Img;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	private DPSelectedItemEventHandler DPSelectedItemNum;

	public DPSelectedItemBoolEventHandler DPSelectedItemTips;

	private int _addValue;
}
