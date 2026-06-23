using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SpiritTrackItemPart : UserControl
{
	public string Icon
	{
		set
		{
			this.TuBiao.URL = string.Format("NetImages/GameRes/Images/ShenJi/{0}.png.qj", value);
		}
	}

	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public int Level
	{
		get
		{
			return this.level;
		}
		set
		{
			this.level = value;
		}
	}

	public int Perv
	{
		get
		{
			return this.perv;
		}
		set
		{
			this.perv = value;
		}
	}

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	public float Size
	{
		get
		{
			return this.size;
		}
		set
		{
			this.size = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.TuBiao.gameObject).onClick = delegate(GameObject e)
		{
			this.ItemClick(this, new DPSelectedItemEventArgs
			{
				ID = this.id,
				Level = this.level,
				Index = this.index,
				Type = this.perv
			});
		};
	}

	public ShowNetImage TuBiao;

	public UISprite BG;

	public DPSelectedItemEventHandler ItemClick;

	private int id;

	private int level;

	private int perv;

	private int index;

	private float size;
}
