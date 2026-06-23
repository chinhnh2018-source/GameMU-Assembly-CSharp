using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class UnMarryPanel : UserControl
{
	private void InitTextInPrefabs()
	{
		this.CheckBox.transform.localPosition = new Vector3(-145f, -187f, 0f);
		if (GameInstance.Game.CurrentSession.MarriageData != null)
		{
			this.HaveChecked = ((int)GameInstance.Game.CurrentSession.MarriageData.byAutoReject == 1);
			this.CheckBox.startsChecked = this.HaveChecked;
		}
		this.CheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.HaveChecked = this.CheckBox.isChecked;
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendCheckedMsg();
			PlayZone.GlobalPlayZone.CloseUnMarryWindow();
		};
		this.DefualtChecked = this.HaveChecked;
	}

	public void SendCheckedMsg()
	{
		if (this.DefualtChecked != this.HaveChecked)
		{
			GameInstance.Game.SpriteAutoRefuseMarry((!this.HaveChecked) ? 0 : 1);
			MUDebug.LogWarning<string>(new string[]
			{
				Global.GetLang("Check Change  Send Msg：") + this.HaveChecked
			});
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public GCheckBox CheckBox;

	public GButton CloseBtn;

	private bool HaveChecked;

	private bool DefualtChecked;
}
