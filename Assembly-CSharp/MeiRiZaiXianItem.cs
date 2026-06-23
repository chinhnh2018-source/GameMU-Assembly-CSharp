using System;
using UnityEngine;

public class MeiRiZaiXianItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	private void InitControl()
	{
	}

	public ListBox m_ListJiangPin = new ListBox();

	public ObservableCollection m_ListJiangPinObC;

	public ShowNetImage m_ShowBakImage;

	public UISprite m_SpriteBak1;

	public UISprite m_SpriteBak2;

	public UILabel m_LblShowTime;

	public UILabel m_LblLiPinState;

	public int m_nMinTime;

	public GImgProgressBar m_ProgressBar;

	public bool m_bActive;

	public bool m_bChouJiang;

	public bool m_bBegining;

	public bool m_bEnd;

	public int m_nLastTick;

	public string m_strJiangLiWuPin = string.Empty;

	public Vector3 m_BeginPos = Vector3.zero;

	public UISprite m_SprProgressFull;

	public UISprite m_SprYiLingQu;

	public bool m_bIsChangeIcon;
}
