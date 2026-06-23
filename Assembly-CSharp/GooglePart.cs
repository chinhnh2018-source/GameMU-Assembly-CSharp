using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GooglePart : UserControl
{
	protected override void InitializeComponent()
	{
		GameInstance.Game.SpriteQureyWanmotaInfo();
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.ChengjiuIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		UIEventListener.Get(this.CaifuIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		UIEventListener.Get(this.WanmotaIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3,
				Index = this.WanMoCeng
			});
		};
	}

	public void GetPaihangData(PaiHangData paiHangData)
	{
		if (paiHangData == null)
		{
			this.WanMoPaihang = -1;
		}
		if (paiHangData.PaiHangList == null)
		{
			this.WanMoPaihang = -1;
		}
		for (int i = 0; i < paiHangData.PaiHangList.Count; i++)
		{
			if (paiHangData.PaiHangList[i].RoleID == Global.Data.roleData.RoleID)
			{
				this.WanMoPaihang = i + 1;
			}
		}
	}

	public void NotifyTiaozhanData(int result, int currentLayer, int maxLayer, int saodangNum, int saodangIndex, int isReboot)
	{
		if (result == 0)
		{
			this.WanMoCeng = currentLayer - 1;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton ChengjiuIcon;

	public UIButton CaifuIcon;

	public UIButton WanmotaIcon;

	public GButton closeBtn;

	private int WanMoPaihang = -1;

	private bool HasData;

	private int WanMoCeng;
}
