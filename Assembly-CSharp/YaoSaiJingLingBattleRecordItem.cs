using System;
using HSGameEngine.GameEngine.Logic;

public class YaoSaiJingLingBattleRecordItem : UserControl
{
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

	public void InitRecordDataByServerData(YaoSaiBossFightLog data, int RankId)
	{
		this.RnakId = RankId;
		this.Name = data.OtherRname;
		this.InviteType = data.InviteType;
		this.DamageValue = data.FightLife;
	}

	private int RnakId
	{
		set
		{
			if (value <= 3)
			{
				NGUITools.SetActive(this.mLblRankIdImage.gameObject, true);
				NGUITools.SetActive(this.mLblRankId.gameObject, false);
				this.mLblRankIdImage.spriteName = value.ToString();
			}
			else
			{
				NGUITools.SetActive(this.mLblRankId.gameObject, true);
				NGUITools.SetActive(this.mLblRankIdImage.gameObject, false);
				this.mLblRankId.Text = value.ToString();
			}
		}
	}

	private new string Name
	{
		set
		{
			this.mLblName.Text = value;
		}
	}

	private int InviteType
	{
		set
		{
			this.mInviteType = (YaoSaiJingLingBattleRecordItem.EInviteType)value;
			switch (this.mInviteType)
			{
			case YaoSaiJingLingBattleRecordItem.EInviteType.MySelf:
				this.mLblInviteType.Text = Global.GetLang("自己");
				break;
			case YaoSaiJingLingBattleRecordItem.EInviteType.Friend:
				this.mLblInviteType.Text = Global.GetLang("好友");
				break;
			case YaoSaiJingLingBattleRecordItem.EInviteType.ZhanMeng:
				this.mLblInviteType.Text = Global.GetLang("战盟");
				break;
			}
		}
	}

	private int DamageValue
	{
		set
		{
			this.mLblDamageValue.Text = value.ToString();
		}
	}

	protected override void OnDestroy()
	{
	}

	public TextBlock mLblRankId;

	public UISprite mLblRankIdImage;

	public TextBlock mLblName;

	public TextBlock mLblInviteType;

	public TextBlock mLblDamageValue;

	private YaoSaiJingLingBattleRecordItem.EInviteType mInviteType;

	private enum EInviteType
	{
		MySelf,
		Friend,
		ZhanMeng
	}
}
