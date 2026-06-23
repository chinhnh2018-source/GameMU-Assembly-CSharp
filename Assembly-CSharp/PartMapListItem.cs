using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class PartMapListItem : UserControl
{
	public string NameText
	{
		get
		{
			return this.lblName.text;
		}
		set
		{
			this.lblName.text = value;
		}
	}

	public new string Tag
	{
		get
		{
			return this._tag;
		}
		set
		{
			this._tag = value;
		}
	}

	protected override void InitializeComponent()
	{
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
			if (null != this.texSelectedBak)
			{
				NGUITools.SetActiveSelf(this.texSelectedBak.gameObject, this._SelectedState);
			}
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Width;
		}
		set
		{
			this.Width = value;
			this.SelectedRect.Width = value - 2.0;
			this.BakPanel.Width = value;
		}
	}

	public Brush TextColor
	{
		set
		{
			this._NameText.TextFontColor = (value as SolidColorBrush);
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Height;
		}
		set
		{
			this.Height = value;
			this.SelectedRect.Height = value - 2.0;
			this.BakPanel.Height = value;
		}
	}

	public string Tip
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public int NpcID
	{
		get
		{
			return this._NpcID;
		}
		set
		{
			this._NpcID = value;
		}
	}

	private void UserControl_MouseLeftButtonUp(MouseEvent e)
	{
	}

	private void UserControl_MouseEnter(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Hand;
			this.SelectedRect.Visibility = true;
		}
	}

	private void UserControl_MouseLeave(MouseEvent e)
	{
		if (Global.Data.GameCursorImageID < 100)
		{
			base.Cursor = Cursors.Auto;
			this.SelectedRect.Visibility = false;
		}
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		string nameText = this.NameText;
		if (nameText == null)
		{
			return;
		}
		string[] array = nameText.Split(new char[]
		{
			','
		});
		if (array.Length != 5)
		{
			return;
		}
		int num = Convert.ToInt32(array[4]);
		int num2 = Convert.ToInt32(array[3]);
		if (num2 == 10001)
		{
			return;
		}
		if (num2 == -1 || num == -1)
		{
			return;
		}
		Point pos;
		if (num2 == 2)
		{
			Global.Data.TargetNpcID = this.NpcID;
			pos = Global.GetMonsterPointByID(num, Global.Data.TargetNpcID);
			Global.Data.GameScene.AutoFindRoad(num, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 11,
					IDType = -10
				});
			}
		}
		else
		{
			if (num2 != 3)
			{
				GameInstance.Game.SpriteGotToMap(num);
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 11,
						IDType = -10
					});
				}
				return;
			}
			Global.Data.TargetNpcID = this.NpcID;
			pos = Global.GetNPCPointByID(num, Global.Data.TargetNpcID);
			Global.Data.GameScene.AutoFindRoad(num, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 11,
					IDType = -10
				});
			}
		}
		if (Super.CanTransport(num, true, true))
		{
			GameInstance.Game.SpriteTaskTransport(num, pos.X, pos.Y, 0);
		}
	}

	public UILabel lblName;

	public UISprite texSelectedBak;

	private string _tag;

	public GTextBlockOutLine _NameText;

	public ShowNetImage SelectedRect;

	private bool _SelectedState;

	public ShowNetImage bak;

	private Canvas BakPanel;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _NpcID;
}
