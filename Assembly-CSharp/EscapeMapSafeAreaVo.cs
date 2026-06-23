using System;
using Tmsk.Xml;

public class EscapeMapSafeAreaVo
{
	public EscapeMapSafeAreaVo(XElement element)
	{
		this.ID = element.GetAttributeInt("ID", -1);
		this.RefreshStage = element.GetAttributeStr("RefreshStage");
		this.TimeStage = element.GetAttributeInt("TimeStage", -1);
		this.StartSafePoint = element.GetAttributeStr("StartSafePoint");
		this.SafeRadius = element.GetAttributeInt("SafeRadius", -1);
		this.GodFireHitTime = element.GetAttributeInt("GodFireHitTime", -1);
		this.GodFireHitPercent = element.GetAttributeInt("GodFireHitPercent", -1);
		this.GodFireHitHp = element.GetAttributeInt("GodFireHitHp", -1);
		this.GodEffictId = element.GetAttributeInt("GodEffictId", -1);
	}

	public int ID;

	public string RefreshStage;

	public int TimeStage;

	public string StartSafePoint;

	public int SafeRadius;

	public int GodFireHitTime;

	public int GodFireHitPercent;

	public int GodFireHitHp;

	public int GodEffictId;
}
