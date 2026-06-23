using System;
using UnityEngine;

public class DengluChoujiangListItem : UserControl
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

	public bool m_bActive;

	public bool m_bChouJiang;

	public bool m_bBegining;

	public bool m_bEnd;

	public int m_nLastTick;

	public Vector3 m_BeginPos = Vector3.zero;

	public string m_strJiangLiWuPin = string.Empty;

	public UILabel m_LiPinState;

	public UISprite m_SprLiPinState;

	public UISprite m_SprKeChouJaing;

	public bool m_bIsChangIcon;
}
