using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ChatBoxDanMuButton : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblOpenOn.text = Global.GetLang("弹");
		this.lblOpenOff.text = Global.GetLang("关");
		this.lblCloseOn.text = Global.GetLang("弹");
		this.lblCloseOff.text = Global.GetLang("关");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.btnClick).onClick = new UIEventListener.VoidDelegate(this.OnSelectDanMu);
		this.SetOpenState(this.m_beDanMuOn);
	}

	private void OnSelectDanMu(GameObject go)
	{
		this.m_beDanMuOn = !this.m_beDanMuOn;
		this.SetOpenState(this.m_beDanMuOn);
		ChatBoxDanMuManager.SetDanMuOpen(this.m_type, this.m_beDanMuOn);
	}

	private void SetOpenState(bool beOpen)
	{
		this.objDanMuOpen.SetActive(beOpen);
		this.objDanMuClose.SetActive(!beOpen);
		this.m_beDanMuOn = beOpen;
	}

	public void SetDanMuType(ChatChannelIndexes index)
	{
		if (index == ChatChannelIndexes.Team)
		{
			this.m_type = DanMuType.Team;
			this.SetOpenState(ChatBoxDanMuManager.BeDanMuOpen(this.m_type));
			base.gameObject.SetActive(true);
		}
		else if (index == ChatChannelIndexes.Faction)
		{
			this.m_type = DanMuType.ZhanMeng;
			this.SetOpenState(ChatBoxDanMuManager.BeDanMuOpen(this.m_type));
			base.gameObject.SetActive(true);
		}
		else
		{
			this.m_type = DanMuType.None;
			base.gameObject.SetActive(false);
		}
	}

	public UILabel lblOpenOn;

	public UILabel lblOpenOff;

	public UILabel lblCloseOn;

	public UILabel lblCloseOff;

	public GameObject objDanMuOpen;

	public GameObject objDanMuClose;

	public GameObject btnClick;

	private bool m_beDanMuOn;

	private DanMuType m_type;
}
