using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class MUJieriZengsongFriendItem : UserControl
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

	public string RoleIcon
	{
		get
		{
			return this._roleIcon;
		}
		set
		{
			this._roleIcon = value;
			this.roleIcon.ImageURL = string.Format("NetImages/Face/{0}0_0.png", value);
			this.roleIcon.gameObject.SetActive(false);
			this.roleIcon.gameObject.SetActive(true);
		}
	}

	public ShowNetImage roleIcon;

	public TextBlock roleName;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _roleID;

	private string _roleName = string.Empty;

	private string _roleIcon = string.Empty;
}
