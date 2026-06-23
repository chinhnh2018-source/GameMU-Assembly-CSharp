using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuoDongInfoBox : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitPartData();
		base.InitializeComponent();
		this._mainBossHPHead.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowBody = !this.ShowBody;
			this._MainBossHPDirect.spriteName = ((!this.ShowBody) ? "mainBoxxHPUp" : "MainBossHPDown");
			this._Body.SetActive(this.ShowBody);
		};
	}

	public void InitPartData()
	{
		this._mainBossHPHead.Text = "{F38D37}" + Global.GetLang("PK之王");
	}

	public void SetSceneTaskInfos(int index, string info, params object[] datas)
	{
		if (index >= 0 && index < this.SceneInfos.Length)
		{
			if (datas.Length == 0)
			{
				this.SceneInfos[index].Text = info;
			}
			else if (datas.Length == 1)
			{
				if (this.SceneInfos[index].Label.pivot == 3)
				{
					if (datas[0] is int && (int)datas[0] < 0)
					{
						this.SceneInfos[index].Text = string.Format("{0}{1}", info, "-");
					}
					else
					{
						this.SceneInfos[index].Text = string.Format("{0}{1}", info, datas[0]);
					}
				}
				else
				{
					this.SceneInfos[index].Text = string.Format("{1}{0}", info, datas[0]);
				}
			}
			else if (datas.Length == 2)
			{
				this.SceneInfos[index].Text = string.Format("{0}{1}/{2}", info, datas[0], datas[1]);
			}
			else
			{
				this.SceneInfos[index].Text = info;
				foreach (object obj in datas)
				{
					info = info + " " + obj;
				}
			}
		}
	}

	public void SetInfoVisiable(int index, bool visiable)
	{
		this.SceneInfos[index].Visibility = visiable;
	}

	public GButton _mainBossHPHead;

	public UISprite _MainBossHPDirect;

	public GameObject _Body;

	public TextBlock[] SceneInfos;

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool ShowBody = true;
}
