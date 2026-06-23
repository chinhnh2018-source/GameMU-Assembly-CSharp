using System;
using HSGameEngine.GameEngine.Network;
using Server.Data;

public class TaoZhuangDesSettingInfo
{
	public TaoZhuangDesSettingInfo()
	{
		this.roleData = GameInstance.Game.CurrentSession.roleData;
	}

	public string shuXingNameColor = "cea46c";

	public string shuXingValuecolor = "f7f7de";

	public string notVisableColor = "786F6F";

	public string effectjiange = "\n\r    ";

	public string weaponEffectjiange = "\n\r            ";

	public bool beShowTaoZhuangName;

	public RoleData roleData;
}
