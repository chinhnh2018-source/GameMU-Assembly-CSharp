using System;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;

public class ChengJiuItem : UserControl
{
	public int itemState
	{
		set
		{
			if (value == 1)
			{
				this.lingquBtn.gameObject.SetActive(false);
				this.statSprite.spriteName = "weidacheng";
				this.statSprite.gameObject.SetActive(true);
			}
			else if (value == 2)
			{
				this.lingquBtn.gameObject.SetActive(true);
				this.statSprite.gameObject.SetActive(false);
			}
			else if (value == 3)
			{
				this.lingquBtn.gameObject.SetActive(false);
				this.statSprite.spriteName = "yilingqu";
				this.statSprite.gameObject.SetActive(true);
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.lingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteFetchChengJiuAward(this.chengJiuID);
		};
	}

	public TextBlock titleText;

	public TextBlock intText;

	public TextBlock jiangliCJText;

	public TextBlock jingliBDZuanshiText;

	public TextBlock jinduText;

	public GButton lingquBtn;

	public UISprite statSprite;

	public string chengJiuType;

	public int chengJiuID;

	public ShowNetImage Bak;

	public GImgProgressBar progressBar;
}
