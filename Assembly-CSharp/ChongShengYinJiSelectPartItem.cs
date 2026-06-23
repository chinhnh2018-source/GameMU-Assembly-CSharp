using System;
using System.Text;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiSelectPartItem : UserControl
{
	public string LblDetails
	{
		set
		{
			this.mLblDetails.Text = value;
		}
	}

	public int IDType { get; set; }

	protected override void InitializeComponent()
	{
		this.InitEvent();
	}

	private void InitEvent()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			if (this.ClickHandler != null)
			{
				this.ClickHandler(this, new DPSelectedItemEventArgs
				{
					Title = "Test ",
					ID = this.IDType
				});
			}
		};
	}

	public void InitValue(int ID)
	{
		this.IDType = ID;
		this.mNetImg.URL = this.TexturePath(this.GetYinJiName(this.IDType));
		this.mSprName.spriteName = this.Names[ID - 1];
	}

	private string TexturePath(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("NetImages/GameRes/Images/ChongShengYinJi/");
		stringBuilder.Append(name);
		return stringBuilder.ToString();
	}

	private string GetYinJiName(int index)
	{
		return index.ToString() + ".png.qj";
	}

	public bool Selecting
	{
		set
		{
			NGUITools.SetActive(this.mSprSelecting.gameObject, value);
		}
	}

	public bool Selected
	{
		set
		{
			NGUITools.SetActive(this.mSprSelecting.gameObject, value);
		}
	}

	public bool CancelSelect
	{
		set
		{
			NGUITools.SetActive(this.mSprSelecting.gameObject, !value);
		}
	}

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock mLblDetails;

	public UISprite mSprSelecting;

	public UISprite mSprName;

	public ShowNetImage mNetImg;

	private string[] Names = new string[]
	{
		"Zi_SSYinJi",
		"Zi_AYYinJi",
		"Zi_ZRYinJi",
		"Zi_HDYinJi",
		"Zi_MYYinJi"
	};
}
