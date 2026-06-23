using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class JiYuanJuanXianPartItem : UserControl
{
	public int EraStage
	{
		get
		{
			return this.m_EraStage;
		}
		set
		{
			this.m_EraStage = value;
		}
	}

	public int EraStateProcess
	{
		get
		{
			return this.m_EraStateProcess;
		}
		set
		{
			this.m_EraStateProcess = value;
			this.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ecb36c",
				Global.GetLang("进度：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				value.ToString() + "%"
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
		this.m_VecPnl = this.m_Panel.transform.localPosition;
		this.m_VecClipPnl = this.m_Panel.clipRange;
		this.m_LabWeiWanCheng.lineWidth = 165;
		this.m_LabWeiWanCheng.pivot = 3;
	}

	public UISprite m_SpGanTanHao;

	public ShowNetImage m_ShowImg;

	public UILabel m_LabTitle;

	public UILabel m_LabWeiWanCheng;

	public UILabel m_LabJinDu;

	public GButton m_BtnJuanXian;

	public ListBox m_ListBox;

	public GButton m_BtnAll;

	public TweenScale m_Tween;

	public GameObject m_Bck;

	public UIPanel m_Panel;

	public int m_EraStage;

	public int m_EraStateProcess;

	public string goodsids;

	public ObservableCollection m_ObservableCollection;

	public Vector3 m_VecPnl = default(Vector3);

	public Vector4 m_VecClipPnl = default(Vector4);
}
