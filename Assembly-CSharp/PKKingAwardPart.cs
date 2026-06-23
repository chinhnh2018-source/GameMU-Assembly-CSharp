using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PKKingAwardPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("确定");
		this._Title.Y = 95.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.Close);
		this._Submit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
	}

	protected void Close(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void InitPartData(string title, bool success, int score, int chengJiu, int expr)
	{
		this._Title.Text = title;
		this._Score.Text = string.Format(Global.GetLang("个人得分: {0}"), ColorCode.EncodingText(score, "fffffe"));
		this._AwardExpr.Text = string.Format(Global.GetLang("奖励经验: {0}"), ColorCode.EncodingText(expr, "fffffe"));
		this._AwardJinBi.Text = string.Format(Global.GetLang("奖励成就: {0}"), ColorCode.EncodingText(chengJiu, "fffffe"));
	}

	public GButton _Close;

	public ShowNetImage _Bak;

	public GButton _Submit;

	public TextBlock _Title;

	public TextBlock _Score;

	public TextBlock _AwardExpr;

	public TextBlock _AwardJinBi;

	public DPSelectedItemEventHandler DPSelectedItem;
}
