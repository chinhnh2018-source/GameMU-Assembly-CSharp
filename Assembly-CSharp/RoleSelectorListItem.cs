using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class RoleSelectorListItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this._textCreateRole2.Text = Global.GetLang("创建角色");
		this._textCreateRole2.X = 10.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.FaceFrameImage.ImageURL = "NetImages/GameRes/Images/Plate/SelectRole_FaceFrame.png";
		this.DeleteRoleBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
	}

	public GTextBlockOutLine TextBlockName
	{
		get
		{
			return this._textBlockName;
		}
	}

	public GTextBlockOutLine TextBlockOccupation
	{
		get
		{
			return this._textBlockOccupation;
		}
	}

	public GTextBlockOutLine TextBlockLevel
	{
		get
		{
			return this._textBlockLevel;
		}
	}

	public bool ItemSelected
	{
		get
		{
			return this._ItemSelected;
		}
		set
		{
			this._ItemSelected = value;
			if (this._ItemSelected)
			{
				this.Bak.gameObject.SetActive(true);
				this.RolePhotoBorder.spriteName = this.rolePhotoBorderNames[1];
			}
			else
			{
				this.Bak.gameObject.SetActive(false);
				this.RolePhotoBorder.spriteName = this.rolePhotoBorderNames[0];
			}
			this.setItemText(this._ItemSelected);
		}
	}

	private void setItemText(bool isSelected)
	{
		if (isSelected)
		{
			this._textBlockName.textColor = 16708300U;
			this._textBlockLevel.textColor = 16777215U;
		}
		else
		{
			this._textBlockName.textColor = 7697781U;
			this._textBlockLevel.textColor = 7697781U;
		}
	}

	public int RoleID { get; set; }

	public int Sex { get; set; }

	public int Occupation
	{
		get
		{
			return this.mOccupation;
		}
		set
		{
			this.mOccupation = value;
			if (this.Occupation >= 0)
			{
				this.mBack.spriteName = string.Format("Touxiang_{0}", this.Occupation);
			}
		}
	}

	public long DeleteDeltaTicks { get; set; }

	public bool HintToCreate
	{
		get
		{
			return this._HintToCreate;
		}
		set
		{
			this._HintToCreate = value;
		}
	}

	public void ChangeImage(string path)
	{
		if (null != this.FaceImage)
		{
			this.FaceImage.URL = path;
		}
	}

	private new void Update()
	{
		if (this._ItemSelected)
		{
			if (this.DeleteDeltaTicks >= 0L)
			{
				if (this._ItemSelected)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2,
						AutoUseGold = false
					});
				}
			}
			else if (this._ItemSelected)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					AutoUseGold = true
				});
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject Bak;

	public UISprite RolePhotoBorder;

	public GTextBlockOutLine _textBlockName;

	public GTextBlockOutLine _textBlockOccupation;

	public GTextBlockOutLine _textBlockLevel;

	public TextBlock _textCreateRole;

	public TextBlock _textCreateRole2;

	public ShowNetImage FaceImage;

	public ShowNetImage FaceFrameImage;

	public GButton DeleteRoleBtn;

	public UISprite mBack;

	private string[] bakNames = new string[]
	{
		"tab_normal2",
		"tab_hover2"
	};

	private string[] rolePhotoBorderNames = new string[]
	{
		"rolePhotoBorder_normal",
		"rolePhotoBorder_hover"
	};

	private bool _ItemSelected;

	private int mOccupation = -1;

	private bool _HintToCreate;
}
