using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class KuaFuPlunderEventProcessor
{
	public static void ProcessEvent(EmKuaFuPlunderEvent eEvent)
	{
		if (eEvent == EmKuaFuPlunderEvent.EnterPlunderMap)
		{
			KuaFuPlunderMap.GetInstance().EnterMapScene();
		}
		else if (eEvent == EmKuaFuPlunderEvent.LeavePlunderMap)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
		}
		else if (eEvent == EmKuaFuPlunderEvent.MapChange)
		{
			KuaFuPlunderMap.GetInstance().MapChange();
		}
	}
}
