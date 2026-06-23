using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CrusadeWorldVO
{
	public void CopyForm(XElement xle)
	{
		this.ID = Global.GetXElementAttributeInt(xle, "ID");
		this.X = Global.GetXElementAttributeFloat(xle, "X");
		this.Y = Global.GetXElementAttributeFloat(xle, "Y");
		this.Dir = Global.GetXElementAttributeInt(xle, "Dir");
	}

	public int ID;

	public float Y;

	public float X;

	public int Dir;
}
