using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class JingLingMapEvent
{
	public static PlayZone pz
	{
		get
		{
			return PlayZone.GlobalPlayZone;
		}
	}

	public static void ProcessEvent(EmJingMapEvent eEvent)
	{
		if (eEvent == EmJingMapEvent.SetMainUIForScene)
		{
			JingLingMap.inst.EnterMapScene();
		}
		else if (eEvent == EmJingMapEvent.MapChange)
		{
			JingLingMap.inst.MapChange();
		}
		else if (eEvent == EmJingMapEvent.LeaveJingLingMap)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
		}
		else if (eEvent != EmJingMapEvent.FromBuildMainPart_ClickBoss)
		{
			if (eEvent != EmJingMapEvent.FromBuildMainPart_ClickShiJieZhanYi)
			{
				if (eEvent == EmJingMapEvent.FromJingLingMap_ShenJi)
				{
					PlayZone.GlobalPlayZone.ShowSpiritTrackPartWindow();
				}
				else if (eEvent == EmJingMapEvent.FromJingLingMap_WorldBattle)
				{
					PlayZone.GlobalPlayZone.CloseBuildMain();
					SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
					if (mapSceneUIClass != JingLingMap.inst.JingLingFuBenClass)
					{
						GameInstance.Game.SpriteGotToMap(84000);
					}
				}
				else if (eEvent == EmJingMapEvent.FromJingLingMap_Prison)
				{
					PlayZone.GlobalPlayZone.ShowYaoSaiJianYuPartWindow();
				}
			}
		}
	}
}
