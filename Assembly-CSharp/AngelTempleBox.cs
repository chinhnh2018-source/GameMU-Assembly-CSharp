using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AngelTempleBox : UserControl
{
	private void InitTextInPrefabs()
	{
		if (this._Orders != null && this._Orders.Length > 5)
		{
			this._Orders[5].Text = Global.GetLang("无");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		base.InitializeComponent();
		this._mainBossHPHead.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowBody = !this.ShowBody;
			this._MainBossHPDirect.spriteName = ((!this.ShowBody) ? "mainBoxxHPUp" : "MainBossHPDown");
			this._Body.SetActive(this.ShowBody);
		};
		this._mainBossHPHead.Text = Global.GetLang("BOSS剩余生命:");
	}

	public void SetSceneInfo(int index, string name, double score)
	{
		if (index >= 0 && index < this._Names.Length)
		{
			this._Names[index].Text = name;
			this._Scores[index].Text = string.Format("{0}%", score * 100.0);
		}
	}

	public void SetInfoVisiable(int index, bool visiable)
	{
		this._Orders[index].Visibility = visiable;
		this._Names[index].Visibility = visiable;
		this._Scores[index].Visibility = visiable;
	}

	public void SetBossLife(double score)
	{
		this._BossLife.Text = string.Format("{0}%", score * 100.0);
	}

	public GButton _mainBossHPHead;

	public UISprite _MainBossHPDirect;

	public GameObject _Body;

	public TextBlock _BossLife;

	public TextBlock[] _Orders;

	public TextBlock[] _Names;

	public TextBlock[] _Scores;

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool ShowBody = true;
}
