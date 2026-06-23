using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class FamilyRequestItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnAgree.Text = Global.GetLang("同意");
		this.btnRefuse.Text = Global.GetLang("拒绝");
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.thisCtrl = this;
		this.InitTextInPrefabs();
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public double BodyWidth
	{
		get
		{
			return this.Container.Width;
		}
		set
		{
			this.Container.Width = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this.Container.Height;
		}
		set
		{
			this.Container.Height = value;
		}
	}

	public BangHuiUIItem CurrentBangHuiUIItem
	{
		get
		{
			return this._CurrentBangHuiUIItem;
		}
		set
		{
			this._CurrentBangHuiUIItem = value;
			if (value.Level > 0)
			{
				this.lblName.text = value.OtherRoleName;
				this.lblLever.text = string.Concat(new object[]
				{
					"LV:",
					value.Level,
					"[",
					value.CLevel,
					Global.GetLang("转]")
				});
			}
			else
			{
				this.lblName.text = value.BHName;
				this.lblLever.text = value.OtherRoleName;
			}
			this.lblOccu.text = Global.GetOccupationStr(value.Occupation);
		}
	}

	public int ID
	{
		get
		{
			return this._ID;
		}
		set
		{
			this._ID = value;
		}
	}

	public void InitButtons()
	{
		this.btnAgree.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentBangHuiUIItem.BangHuiCmd == 1)
			{
				GameInstance.Game.SpriteAddBHMember(this.CurrentBangHuiUIItem.BHID, this.CurrentBangHuiUIItem.OtherRoleID, this.CurrentBangHuiUIItem.OtherRoleName, 0);
			}
			else
			{
				GameInstance.Game.SpriteAgreeToBHMember(this.CurrentBangHuiUIItem.OtherRoleID, this.CurrentBangHuiUIItem.BHID, this.CurrentBangHuiUIItem.BHName, 1);
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this.ID
				});
			}
		};
		this.btnRefuse.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentBangHuiUIItem.BangHuiCmd == 1)
			{
				GameInstance.Game.SpriteRefuseApplyToBHMember(this.CurrentBangHuiUIItem.BHID, this.CurrentBangHuiUIItem.OtherRoleID);
			}
			else
			{
				GameInstance.Game.SpriteAgreeToBHMember(this.CurrentBangHuiUIItem.OtherRoleID, this.CurrentBangHuiUIItem.BHID, this.CurrentBangHuiUIItem.BHName, 0);
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this.ID
				});
			}
		};
	}

	public GButton btnAgree;

	public GButton btnRefuse;

	public UILabel lblName;

	public UILabel lblLever;

	public UILabel lblOccu;

	public ShowNetImage texPersonImg;

	private Canvas Root;

	public GTextBlockOutLine gtbPlayerName;

	public GTextBlockOutLine gtbLevel;

	public SpriteSL thisCtrl;

	private BangHuiUIItem _CurrentBangHuiUIItem;

	private int _ID;

	public DPSelectedItemEventHandler DPSelectedItem;
}
