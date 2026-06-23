using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public struct KuafuActivityTabConfigLine
{
	public void InitConfig(XElement args)
	{
		if (args == null)
		{
			return;
		}
		this.ID = Global.GetXElementAttributeInt(args, "ID");
		this.Name = Global.GetXElementAttributeStr(args, "Name");
		this.Preview = Global.GetXElementAttributeStr(args, "Preview");
		this.RewardExplain = Global.GetXElementAttributeStr(args, "RewardExplain");
		this.GLXml = Global.GetXElementAttributeStr(args, "GLXml");
	}

	public bool IsYongZheZhanChang
	{
		get
		{
			return this.ID == 20002;
		}
	}

	public bool IsKuaFuBOSS
	{
		get
		{
			return this.ID == 20003;
		}
	}

	public bool IsZhuongShenZhengBa
	{
		get
		{
			return this.ID == 20004;
		}
	}

	public bool IsKuaFuWangZhe
	{
		get
		{
			return this.ID == 20006;
		}
	}

	public bool IsKuFuPlunder
	{
		get
		{
			return this.ID == 20009;
		}
	}

	public int nItemIndex
	{
		get
		{
			if (this.ID == 20000)
			{
				return 0;
			}
			if (this.ID == 20001)
			{
				return 1;
			}
			if (this.ID == 20002)
			{
				return 2;
			}
			if (this.ID == 20003)
			{
				return 3;
			}
			return 0;
		}
	}

	public int ID;

	public string Name;

	public string Preview;

	public string RewardExplain;

	public string GLXml;
}
