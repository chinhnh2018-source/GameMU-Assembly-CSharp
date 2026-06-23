using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using Server.Tools;

public class MonsterDeathState : SpriteStateBase
{
	public MonsterDeathState(GSprite monster) : base(monster)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Death;
		}
	}

	public override void EnterState()
	{
		this.mSprite.AnimatorComponent.SetBool("Die", true);
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.mSprite.ExtensionID);
		if (monsterXmlNodeByID != null)
		{
			if (monsterXmlNodeByID.DieAnimation > 0)
			{
				GDecoration decoration = Global.GetDecoration(monsterXmlNodeByID.DieAnimation, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, -1, true, false);
				if (decoration != null)
				{
					return;
				}
			}
			string dieSound = monsterXmlNodeByID.DieSound;
			if (!string.IsNullOrEmpty(dieSound))
			{
				string playingMusicFile = StringUtil.substitute("Audio/Monster/{0}", new object[]
				{
					dieSound
				});
				this.mSprite.PlaySpriteSound(playingMusicFile, false);
			}
		}
		if (Global.IsBloodCastleChengMen(this.mSprite.ExtensionID))
		{
			int code = 79;
			GDecoration decoration2 = Global.GetDecoration(code, GDecorationTypes.Loop, this.mSprite.Coordinate, true, null, -1, -1, true, false);
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ExitState()
	{
		this.mSprite.AnimatorComponent.SetBool("Die", false);
		if (!this.mSprite.IsDeath)
		{
			this.mSprite.IsDeath = true;
			this.mSprite.deathDelay = Global.GetMyTimer();
		}
		base.ExitState();
	}
}
