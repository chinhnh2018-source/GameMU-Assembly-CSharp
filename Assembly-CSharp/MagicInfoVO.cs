using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class MagicInfoVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.NextMagicID = Global.GetXElementAttributeInt(xml, "NextMagicID");
		this.ParentMagicID = Global.GetXElementAttributeInt(xml, "ParentMagicID");
		this.SkillType = Global.GetXElementAttributeInt(xml, "SkillType");
		this.Queue = Global.GetXElementAttributeInt(xml, "Queue");
		this.InjureType = Global.GetXElementAttributeInt(xml, "InjureType");
		this.DelayDecoToMap = Global.GetXElementAttributeInt(xml, "DelayDecoToMap");
		this.SkillAction = Global.GetXElementAttributeStr(xml, "SkillAction");
		this.ToOcuupation = Global.GetXElementAttributeInt(xml, "ToOcuupation");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.LearnCondition = Global.GetXElementAttributeStr(xml, "LearnCondition");
		this.LearnTask = Global.GetXElementAttributeStr(xml, "LearnTask");
		this.HasDirection = Global.GetXElementAttributeInt(xml, "HasDirection");
		this.MagicType = Global.GetXElementAttributeInt(xml, "MagicType");
		this.DamageType = Global.GetXElementAttributeInt(xml, "DamageType");
		this.Bangding = Global.GetXElementAttributeInt(xml, "Bangding");
		this.MagicsBook = Global.GetXElementAttributeInt(xml, "MagicsBook");
		this.UseType = Global.GetXElementAttributeInt(xml, "UseType");
		this.FanWeiDescription = Global.GetXElementAttributeStr(xml, "FanWeiDescription");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.CDTime = Global.GetXElementAttributeInt(xml, "CDTime");
		this.PubCDTime = Global.GetXElementAttributeInt(xml, "PubCDTime");
		this.AttackInterval = Global.GetXElementAttributeInt(xml, "AttackInterval");
		this.BaseMagic = Global.GetXElementAttributeInt(xml, "BaseMagic");
		this.AttackDistance = Global.GetXElementAttributeStr(xml, "AttackDistance");
		this.ScanType = Global.GetXElementAttributeStr(xml, "ScanType");
		this.TargetPos = Global.GetXElementAttributeInt(xml, "TargetPos");
		this.TargetType = Global.GetXElementAttributeInt(xml, "TargetType");
		this.MagicScripts = Global.GetXElementAttributeStr(xml, "MagicScripts");
		this.MagicScripts2 = Global.GetXElementAttributeStr(xml, "MagicScripts2");
		this.ManyTimeDmage = Global.GetXElementAttributeStr(xml, "ManyTimeDmage");
		this.MaxNum = Global.GetXElementAttributeInt(xml, "MaxNum");
		this.MagicIcon = Global.GetXElementAttributeInt(xml, "MagicIcon");
		this.AutoStart = Global.GetXElementAttributeInt(xml, "AutoStart");
		this.MagicTime = Global.GetXElementAttributeStr(xml, "MagicTime");
		this.MagicColor = Global.GetXElementAttributeStr(xml, "MagicColor");
		this.ActionType = Global.GetXElementAttributeInt(xml, "ActionType");
		this.MagicCode = Global.GetXElementAttributeStr(xml, "MagicCode");
		this.FlyDecoration = Global.GetXElementAttributeStr(xml, "FlyDecoration");
		this.TargetDecoration = Global.GetXElementAttributeStr(xml, "TargetDecoration");
		this.DelayDecoration = Global.GetXElementAttributeStr(xml, "DelayDecoration");
		this.TargetPlayingType = Global.GetXElementAttributeInt(xml, "TargetPlayingType");
		this.MusicWeapon = Global.GetXElementAttributeStr(xml, "MusicWeapon");
		this.MusicNoWeapon = Global.GetXElementAttributeStr(xml, "MusicNoWeapon");
		this.ActionIndex = Global.GetXElementAttributeInt(xml, "ActionIndex");
		this.MoveType = Global.GetXElementAttributeInt(xml, "MoveType");
		this.HorseSkill = Global.GetXElementAttributeInt(xml, "HorseSkill");
	}

	public int ID;

	public int NextMagicID;

	public int ParentMagicID;

	public int SkillType;

	public int Queue;

	public int InjureType;

	public int DelayDecoToMap;

	public string SkillAction;

	public int ToOcuupation;

	public string Name;

	public string LearnCondition;

	public string LearnTask;

	public int HasDirection;

	public int MagicType;

	public int DamageType;

	public int Bangding;

	public int MagicsBook;

	public int UseType;

	public string FanWeiDescription;

	public string Description;

	public int CDTime;

	public int PubCDTime;

	public int AttackInterval;

	public int BaseMagic;

	public string AttackDistance;

	public string ScanType;

	public int TargetPos;

	public int TargetType;

	public string MagicScripts;

	public string MagicScripts2;

	public string ManyTimeDmage;

	public int MaxNum;

	public int MagicIcon;

	public int AutoStart;

	public string MagicTime;

	public string MagicColor;

	public int ActionType;

	public string MagicCode;

	public string FlyDecoration;

	public string TargetDecoration;

	public string DelayDecoration;

	public int TargetPlayingType;

	public string MusicWeapon;

	public string MusicNoWeapon;

	public int ActionIndex;

	public int MoveType;

	public int HorseSkill;
}
