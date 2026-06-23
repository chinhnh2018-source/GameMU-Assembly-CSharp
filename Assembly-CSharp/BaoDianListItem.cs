using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class BaoDianListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnGoto.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				int actionType = this.ActionType;
				if (actionType != 0)
				{
					if (actionType == 1)
					{
						string[] array = this.ActionValue.Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							int id = int.Parse(array[0]);
							int index = int.Parse(array[1]);
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								IDType = this.ActionType,
								ID = id,
								Index = index
							});
						}
					}
				}
				else
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = this.ActionType,
						ID = int.Parse(this.ActionValue)
					});
				}
			}
		};
	}

	public int ActionType
	{
		get
		{
			return this._actionType;
		}
		set
		{
			this._actionType = value;
		}
	}

	public string ActionValue
	{
		get
		{
			return this._actionValue;
		}
		set
		{
			this._actionValue = value;
		}
	}

	public string NeedTitle
	{
		get
		{
			return this.lblNeedTitle.text;
		}
		set
		{
			this.lblNeedTitle.text = value;
		}
	}

	public string NeedDesc
	{
		get
		{
			return this.lblNeedDesc.text;
		}
		set
		{
			this.lblNeedDesc.text = value;
		}
	}

	public string ItemName
	{
		get
		{
			return this.lblItemName.text;
		}
		set
		{
			this.lblItemName.text = value;
		}
	}

	public string ItemDesc
	{
		get
		{
			return this.lblItemDesc.text;
		}
		set
		{
			this.lblItemDesc.text = value;
		}
	}

	public string iconTexture
	{
		get
		{
			return this.texIcon.spriteName;
		}
		set
		{
			if (value.Contains("."))
			{
				value = value.Split(new char[]
				{
					'.'
				})[0];
			}
			this.texIcon.spriteName = value;
		}
	}

	public int RecommondValue
	{
		get
		{
			return (int)this.texStar.transform.localScale.x / 21;
		}
		set
		{
			Vector3 localScale = this.texStar.transform.localScale;
			localScale.x = (float)(21 * value);
			this.texStar.transform.localScale = localScale;
		}
	}

	public bool CheckOk
	{
		set
		{
			this.btnGoto.gameObject.SetActive(value);
			this.group.SetActive(!value);
		}
	}

	public UILabel lblNeedTitle;

	public UILabel lblNeedDesc;

	public GameObject group;

	public GButton btnGoto;

	public UILabel lblItemName;

	public UILabel lblItemDesc;

	public UISprite texIcon;

	public UISprite texStar;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _actionType;

	private string _actionValue = string.Empty;
}
