using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class LovserWishpartFriendPartItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public int RoleID
	{
		get
		{
			return this._roleID;
		}
		set
		{
			this._roleID = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this._roleName;
		}
		set
		{
			this._roleName = value;
			this.roleName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this._roleName
			});
		}
	}

	public TextBlock roleName;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _roleID;

	private string _roleName = string.Empty;
}
