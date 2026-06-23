using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class ZhanMengWaiJiaoRequestItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnCancel.Text = Global.GetLang("拒绝");
		this.btnAgree.Text = Global.GetLang("同意");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = this.zhanMengId,
					Flag = 2
				});
			}
		};
		this.btnAgree.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = this.zhanMengId,
					Flag = 1
				});
			}
		};
	}

	public void SetValue(AllyData data)
	{
		if (data == null)
		{
			return;
		}
		this.zhanMengId = data.UnionID;
		this.txtZhanMengName.Text = Global.FormatRoleNameZoneid(data.UnionZoneID, data.UnionName, 1, 1);
		this.txtLevel.Text = string.Format("{0}{1}", "Lv", data.UnionLevel);
		this.txtLeaderName.Text = string.Format("{0}{1}{2}", "{18bf35}", data.LeaderName, "{-}");
		this.txtPeopleAmount.Text = string.Format("{0}{1}", data.UnionNum, "/50");
	}

	public GButton btnCancel;

	public GButton btnAgree;

	public TextBlock txtZhanMengName;

	public TextBlock txtLevel;

	public TextBlock txtLeaderName;

	public TextBlock txtPeopleAmount;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int zhanMengId;
}
