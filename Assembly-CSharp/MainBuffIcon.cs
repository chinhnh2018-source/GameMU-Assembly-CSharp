using System;
using Server.Data;

public class MainBuffIcon : UserControl
{
	public BufferData BuffData
	{
		get
		{
			return this.m_buffData;
		}
		set
		{
			this.m_buffData = value;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void SetBuffImageCode(string url)
	{
		this.buffIcon.URL = url;
	}

	public ShowNetImage buffIcon;

	private BufferData m_buffData;
}
