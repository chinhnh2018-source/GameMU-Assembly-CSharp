using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MembersInfoListItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this._LastOnLineTime.text = string.Empty;
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public string Level
	{
		get
		{
			return this.lblLevel.text;
		}
		set
		{
			this.lblLevel.text = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this.btnPersonImg.Label.text;
		}
		set
		{
			this.btnPersonImg.Label.text = value;
			float num = this.btnPersonImg.Label.transform.localScale.x * this.btnPersonImg.Label.relativeSize.x;
			float num2 = this.btnPersonImg.Label.transform.localScale.y * this.btnPersonImg.Label.relativeSize.y;
			this.RefeshOccSpPos();
		}
	}

	public int Occupation
	{
		set
		{
			this.mOccupation = value;
			this._OccSp.spriteName = "Occ" + this.mOccupation.ToString();
			this._OccSp.Update();
			this.RefeshOccSpPos();
		}
	}

	private void RefeshOccSpPos()
	{
		float num = this.btnPersonImg.Label.transform.localScale.x * this.btnPersonImg.Label.relativeSize.x / 2f;
		this._OccSp.transform.localPosition = new Vector3(this.btnPersonImg.transform.localPosition.x + num + 18f, this._OccSp.transform.localPosition.y, this._OccSp.transform.localPosition.z);
	}

	public string Occ
	{
		set
		{
			this.lblOcupation.text = string.Empty;
		}
	}

	public int BHZhiWu
	{
		get
		{
			return this._BHZhiWu;
		}
		set
		{
			this._BHZhiWu = value;
		}
	}

	public string Zw
	{
		get
		{
			return this.lblDuty.text;
		}
		set
		{
			this.lblDuty.text = value;
		}
	}

	public string MemberForce
	{
		get
		{
			return this.lblForce.text;
		}
		set
		{
			this.lblForce.text = value;
		}
	}

	public long LogOffTime
	{
		set
		{
			if (0L < value)
			{
				this.mLogOffTime = value;
				if (this._LastOnLineTime.color == NGUIMath.HexToColorEx(65280U))
				{
					this._LastOnLineTime.text = Global.GetLang("在线");
				}
				else
				{
					long num = (Global.GetCorrectLocalTime() - value) / 1000L;
					this._LastOnLineTime.text = Global.GetTimeStrBySecFilterZero((int)num, true, 2) + Global.GetLang("前");
				}
			}
		}
	}

	public int IsOnline
	{
		set
		{
			if (value == 1)
			{
				this.lblOcupation.color = NGUIMath.HexToColorEx(16777215U);
				this.lblLevel.color = NGUIMath.HexToColorEx(16777215U);
				this.lblDuty.color = NGUIMath.HexToColorEx(16751880U);
				this.lblForce.color = NGUIMath.HexToColorEx(16777215U);
				this.btnPersonImg.Label.color = NGUIMath.HexToColorEx(65280U);
				this._LastOnLineTime.color = NGUIMath.HexToColorEx(65280U);
				this._LastOnLineTime.text = Global.GetLang("在线");
			}
			else if (value == 0)
			{
				this.lblOcupation.color = NGUIMath.HexToColorEx(7697781U);
				this.lblLevel.color = NGUIMath.HexToColorEx(7697781U);
				this.lblDuty.color = NGUIMath.HexToColorEx(7697781U);
				this.lblForce.color = NGUIMath.HexToColorEx(7697781U);
				this.btnPersonImg.Label.color = NGUIMath.HexToColorEx(7697781U);
				this._LastOnLineTime.color = NGUIMath.HexToColorEx(7697781U);
				if (0L < this.mLogOffTime)
				{
					long num = (Global.GetCorrectLocalTime() - this.mLogOffTime) / 1000L;
					this._LastOnLineTime.text = Global.GetTimeStrBySecFilterZero((int)num, true, 2) + Global.GetLang("前");
				}
			}
		}
	}

	public void ChangeTextColor(bool onlineState)
	{
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			base.BackgroundAlpha = 0.2;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Auto;
			base.BackgroundAlpha = 0.01;
		}
	}

	public bool SelectedState
	{
		get
		{
			return this._SelectedState;
		}
		set
		{
			this._SelectedState = value;
			if (null != this.texSelected)
			{
				NGUITools.SetActiveSelf(this.texSelected.gameObject, this._SelectedState);
			}
			Vector2 lastTouchPosition = UICamera.lastTouchPosition;
			if (!this._SelectedState && this.menuPart != null)
			{
				this.menuPart.Visibility = false;
			}
		}
	}

	public int ZhiWuID { get; set; }

	public int Speech
	{
		get
		{
			return this.mEnableSpeech;
		}
		set
		{
			if (Global.Data != null && Global.Data.roleData.RoleID == this.RoleID && Global.Data.roleData.BHZhiWu == 1)
			{
				this.mEnableSpeech = 1;
				NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
			}
			else
			{
				this.mEnableSpeech = value;
				NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
			}
		}
	}

	public bool HideVoiceIcon
	{
		set
		{
			NGUITools.SetActive(this.mVoiceIcon.gameObject, false);
		}
	}

	public void RefreshPopupMenu(int zhiwuID)
	{
	}

	private bool InClickArea(Vector2 touchPos)
	{
		return touchPos.x / Global.Data.ScreenScaleX > 18f && touchPos.x / Global.Data.ScreenScaleX < 921f;
	}

	public void OnClick()
	{
		if (this.InClickArea(UICamera.lastTouchPosition))
		{
			this.CreateMenuWindow();
		}
	}

	public void CreateMenuWindow()
	{
		if (this.RoleID == Global.Data.roleData.RoleID)
		{
			return;
		}
		if (null != this.menuPart)
		{
			if (this.menuPart.Visibility)
			{
				this.menuPart.Visibility = false;
			}
			else
			{
				this.menuPart.Speech = this.Speech;
				this.ResetMenuPartPos();
				this.menuPart.Visibility = true;
			}
			return;
		}
		this.menuPart = U3DUtils.NEW<GTxtMenuPart>();
		this.menuPart.Width = 150.0;
		this.menuPart.ItemHeight = 35;
		if (this.ZhiWuID == 1)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
			this.menuPart.AddMenuItem(1, Global.GetLang("私 聊"));
			this.menuPart.AddMenuItem(2, Global.GetLang("踢 出"));
			this.menuPart.AddMenuItem(3, Global.GetLang("委 任"));
		}
		else if (this.ZhiWuID == 2)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
			this.menuPart.AddMenuItem(1, Global.GetLang("私 聊"));
			this.menuPart.AddMenuItem(2, Global.GetLang("踢 出"));
		}
		else if (this.ZhiWuID == 3 || this.ZhiWuID == 4)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
			this.menuPart.AddMenuItem(1, Global.GetLang("私 聊"));
			this.menuPart.AddMenuItem(2, Global.GetLang("踢 出"));
		}
		else if (this.ZhiWuID == 0)
		{
			this.menuPart.AddMenuItem(0, Global.GetLang("查 看"));
			this.menuPart.AddMenuItem(1, Global.GetLang("私 聊"));
		}
		if (Global.Data != null && Global.Data.roleData.BHZhiWu == 1)
		{
			this.menuPart.AddMenuItem(4, this.GetSpeechPermissionText(this.Speech));
		}
		this.menuPart.RenderMenu();
		this.menuPart.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != this.menuPart)
			{
				NGUITools.Destroy(this.menuPart.gameObject);
			}
			this.menuPart = null;
		};
		this.menuPart.MenuItemClick = delegate(object s, EventArgs e)
		{
			GTxtMenuItem gtxtMenuItem = s as GTxtMenuItem;
			if (null == gtxtMenuItem)
			{
				return;
			}
			if (gtxtMenuItem.MenuItemID >= 0 && gtxtMenuItem.MenuItemID <= 6)
			{
				this.ItemFunction(gtxtMenuItem.MenuItemID);
			}
			this.menuPart.Visibility = false;
		};
		this.menuPart.SelectIndex = -1;
		U3DUtils.AddChild(base.transform.gameObject, this.menuPart.gameObject, true);
		this.ResetMenuPartPos();
	}

	private string GetSpeechPermissionText(int type)
	{
		return null;
	}

	private void ResetMenuPartPos()
	{
		Vector3 vector = Vector3.zero;
		vector..ctor(UICamera.lastTouchPosition.x, UICamera.lastTouchPosition.y, -0.1f);
		if (UICamera.lastTouchPosition.y < 165f * Global.Data.ScreenScaleY)
		{
			vector..ctor(UICamera.lastTouchPosition.x, 165f * Global.Data.ScreenScaleY, -0.1f);
		}
		if (UICamera.lastTouchPosition.x > 775f * Global.Data.ScreenScaleX)
		{
			vector..ctor(775f * Global.Data.ScreenScaleX, vector.y, -0.1f);
		}
		if (this.menuPart != null)
		{
			this.menuPart.transform.position = UICamera.mainCamera.ScreenToWorldPoint(vector);
			vector = this.menuPart.transform.localPosition;
			vector.z = -1f;
			this.menuPart.transform.localPosition = vector;
		}
	}

	private void ItemFunction(int idx)
	{
		switch (idx)
		{
		case 0:
			if (this.RoleID != Global.Data.roleData.RoleID)
			{
				GameInstance.Game.SpriteGetOtherAttrib(this.RoleID);
			}
			break;
		case 1:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					Title = this.btnPersonImg.Label.text
				});
			}
			break;
		case 2:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			break;
		case 3:
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
			break;
		case 4:
			if (this.DPSelectedItem != null)
			{
				int type = (this.Speech != 2) ? 2 : 3;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 4,
					MyID = this.RoleID,
					Type = type
				});
			}
			break;
		}
	}

	public UILabel lblOcupation;

	public UILabel lblLevel;

	public UILabel lblDuty;

	public UILabel lblForce;

	public GButton btnPersonImg;

	public UISprite mVoiceIcon;

	[SerializeField]
	private UISprite _OccSp;

	[SerializeField]
	private UILabel _LastOnLineTime;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite texSelected;

	private Canvas Root;

	private int _RoleID;

	public UISprite texUnderLine;

	private int mOccupation;

	private int _BHZhiWu;

	private long mLogOffTime = -1L;

	private bool _SelectedState;

	private int mEnableSpeech = 3;

	public GTxtMenuPart menuPart;
}
