using System;
using HSGameEngine.GameEngine.SilverLight;

public class TopListItemPlayers : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public string strSpriteName
	{
		get
		{
			return this.m_SpriteNum.name;
		}
		set
		{
			this.m_SpriteNum.name = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this.txtName.Text;
		}
		set
		{
			this.txtName.Text = value;
		}
	}

	public string Rank
	{
		get
		{
			return this.txtRank.Text;
		}
		set
		{
			this.txtRank.Text = value;
		}
	}

	public string Power
	{
		get
		{
			return this.txtPower.Text;
		}
		set
		{
			this.txtPower.Text = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
		}
	}

	public uint TxtColor
	{
		set
		{
			this.txtRank.textColor = value;
			this.txtName.textColor = value;
			this.txtPower.textColor = value;
		}
	}

	public uint BakColor
	{
		set
		{
			this.bak.Texture.color = NGUIMath.HexToColorEx(value);
		}
	}

	public int JieNum
	{
		get
		{
			return this._jieNum;
		}
		set
		{
			this._jieNum = value;
		}
	}

	public int LevelNum
	{
		get
		{
			return this._levelNum;
		}
		set
		{
			this._levelNum = value;
		}
	}

	public TextBlock txtRank;

	public TextBlock txtName;

	public TextBlock txtPower;

	public ShowNetImage bak;

	public UISprite m_SpriteHotBak;

	public UISprite m_SpriteNorMalBak;

	public UISprite m_SpriteNum;

	public int m_nZhiYe;

	private int _RoleID;

	private int _jieNum;

	private int _levelNum;
}
