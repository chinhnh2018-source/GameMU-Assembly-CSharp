using System;
using Server.Data;

public class Players : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public new UILabel Name;

	public TianTiPaiHangRoleData RoleData;

	public int Group;

	public int State;

	public int Grade;
}
