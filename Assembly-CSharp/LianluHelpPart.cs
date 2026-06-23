using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LianluHelpPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.HelpPanelPos = this.HelpPanelTran.localPosition;
		this.HelpPanelClipRange = this.HelpPanel.clipRange;
		UIEventListener.Get(this.CloseBtn.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
	}

	private void OnEnable()
	{
		this.InitPanelPos();
	}

	public string BackImgName
	{
		set
		{
			if (this._BackImgName != value)
			{
				this._BackImgName = value;
				this.Bak.URL = string.Format("NetImages/GameRes/Images/Plate/{0}", value);
			}
		}
	}

	private void InitPanelPos()
	{
		this.HelpDraggablePanel.DisableSpring();
		this.HelpPanelTran.localPosition = this.HelpPanelPos;
		this.HelpPanel.clipRange = this.HelpPanelClipRange;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton CloseBtn;

	public ShowNetImage Bak;

	public Transform HelpPanelTran;

	public UIPanel HelpPanel;

	public UIDraggablePanel HelpDraggablePanel;

	private Vector3 HelpPanelPos;

	private Vector4 HelpPanelClipRange;

	private string _BackImgName;
}
