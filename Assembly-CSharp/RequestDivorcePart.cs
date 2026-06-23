using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class RequestDivorcePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitPrefabs();
	}

	private void InitPrefabs()
	{
		this.ApplyBtn.Text = Global.GetLang("申请离婚");
		this.CloseBtn.Text = Global.GetLang("再想一想");
		this.AttrLabel.text = Global.GetLang("注：离婚以后定情信物将暂时封存，不再为您添加属性");
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("DivorceJinBiCost", true);
		this.FreeNeedNum.text = systemParamByName;
		string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("DivorceZuanShiCost", true);
		this.ForceNeedNum.text = systemParamByName2;
		this.NeedGoldNum = Global.SafeConvertToInt32(systemParamByName);
		this.NeedDiamondNum = Global.SafeConvertToInt32(systemParamByName2);
		this.ApplyBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.PreIndex == 1)
			{
				if (Global.GetRoleOwnNumByMoneyType(8) + Global.GetRoleOwnNumByMoneyType(1) < this.NeedGoldNum)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
					return;
				}
				Super.ShowNetWaiting(string.Empty);
				GameInstance.Game.SpriteRequestDivorce(1);
			}
			else
			{
				if (Global.GetRoleOwnNumByMoneyType(40) < this.NeedDiamondNum)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, new DPSelectedItemEventHandler(this.CloseThisWindow), string.Empty, string.Empty);
					return;
				}
				GameInstance.Game.SpriteRequestDivorce(0);
			}
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseRequestDivorceWindow();
			PlayZone.GlobalPlayZone.OpenMarryLoveTockenPart();
		};
		this.FreeDivorceBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectIndex(1);
		};
		this.ForceDivorceBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetSelectIndex(2);
		};
	}

	public void SetSelectIndex(int index)
	{
		if (this.PreIndex != index)
		{
			if (index == 1)
			{
				this.FreeDivorceBtn.normalSprite = "selectkuang";
				this.FreeDivorceBtn.Refresh();
				this.ForceDivorceBtn.normalSprite = "maskkuang";
				this.ForceDivorceBtn.Refresh();
			}
			else
			{
				this.FreeDivorceBtn.normalSprite = "maskkuang";
				this.FreeDivorceBtn.Refresh();
				this.ForceDivorceBtn.normalSprite = "selectkuang";
				this.ForceDivorceBtn.Refresh();
			}
			this.PreIndex = index;
		}
	}

	private void CloseThisWindow(object sender, DPSelectedItemEventArgs args)
	{
		PlayZone.GlobalPlayZone.CloseRequestDivorceWindow();
	}

	public GButton FreeDivorceBtn;

	public GButton ForceDivorceBtn;

	public GButton ApplyBtn;

	public GButton CloseBtn;

	public UILabel AttrLabel;

	public UILabel FreeNeedNum;

	public UILabel ForceNeedNum;

	private int NeedGoldNum;

	private int NeedDiamondNum;

	private int PreIndex;
}
