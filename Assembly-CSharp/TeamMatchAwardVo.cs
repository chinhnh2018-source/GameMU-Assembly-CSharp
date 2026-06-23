using System;
using Tmsk.Xml;

public class TeamMatchAwardVo
{
	public TeamMatchAwardVo(XElement element)
	{
		this.ID = element.GetAttributeInt("ID", -1);
		this.TeamPoint = element.GetAttributeInt("TeamPoint", -1);
	}

	public int ID;

	public string Name;

	public int Rank;

	public string Award;

	public int TeamPoint;
}
