using System;

public class GRadarMapPoint : SpriteSL
{
	protected virtual void InitializeComponent()
	{
	}

	public string Img
	{
		get
		{
			return this.sprite.spriteName;
		}
		set
		{
			this.sprite.spriteName = value;
		}
	}

	public string Title
	{
		get
		{
			return this.title.text;
		}
		set
		{
			this.title.text = value;
		}
	}

	public double PointWidth { get; set; }

	public double PointHeight { get; set; }

	public int NPCTaskState { get; set; }

	public int RoleDirection { get; set; }

	public UILabel title;

	public UISprite sprite;

	public int id;
}
