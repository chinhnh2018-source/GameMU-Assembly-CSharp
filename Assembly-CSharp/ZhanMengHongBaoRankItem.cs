using System;

public class ZhanMengHongBaoRankItem : UserControl
{
	public int LblRankID
	{
		set
		{
			this.mLblRankID.Text = value.ToString();
		}
	}

	public int SpriteRankID
	{
		set
		{
			this.mSpriteRankID.spriteName = value.ToString();
		}
	}

	public new string Name
	{
		set
		{
			this.mName.Text = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
	}

	private void InitValue()
	{
	}

	public void SetSpriteActivity(bool isShow)
	{
		NGUITools.SetActive(this.mSpriteRankID.gameObject, isShow);
		NGUITools.SetActive(this.mLblRankID.gameObject, !isShow);
	}

	protected override void OnDestroy()
	{
		this.mSpriteRankID = null;
		this.mLblRankID = null;
		this.mName = null;
	}

	public UISprite mSpriteRankID;

	public TextBlock mLblRankID;

	public TextBlock mName;
}
