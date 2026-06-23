using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class DecorationVO
{
	public void CopyFrom(XElement xml)
	{
		this.Code = Global.GetXElementAttributeInt(xml, "Code");
		this.ResName = Global.GetXElementAttributeStr(xml, "ResName");
		this.Sound = Global.GetXElementAttributeStr(xml, "Sound");
		this.HangPos = Global.GetXElementAttributeInt(xml, "HangPos");
	}

	public int Code;

	public string ResName;

	public string Sound;

	public int HangPos;
}
