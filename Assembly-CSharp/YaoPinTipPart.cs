using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class YaoPinTipPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitControlProc();
	}

	private void InitControlProc()
	{
		if (null != this.m_Check)
		{
			this.m_Check.Check = Global.g_bIsYaoPinTip;
			this.m_Check.CheckChanged = delegate(object s, BaseEventArgs e)
			{
				Global.g_bIsYaoPinTip = this.m_Check.Check;
			};
		}
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		if (null != this.m_BtnGo)
		{
			this.m_BtnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				Point npcpointByID = Global.GetNPCPointByID(1, 100);
				if (2 >= Global.Data.roleData.VIPLevel)
				{
					Global.Data.GameScene.AutoFindRoad(1, npcpointByID, 120, ExtActionTypes.EXTACTION_NPCDLG);
				}
				else
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 1
					});
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnClose;

	public GButton m_BtnGo;

	public GCheckBox m_Check;
}
