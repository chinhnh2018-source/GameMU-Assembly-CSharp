using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class NPCInfoVO
{
	public bool IsGatherNPC
	{
		get
		{
			return this.GatherCondition.Trim().Length >= 4;
		}
	}

	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.PicCode = Global.GetXElementAttributeInt(xml, "PicCode");
		this.ResName = Global.GetXElementAttributeStr(xml, "ResName");
		this.YouShou = Global.GetXElementAttributeStr(xml, "YouShou");
		this.ZuoShou = Global.GetXElementAttributeStr(xml, "ZuoShou");
		this.Wing = Global.GetXElementAttributeStr(xml, "Wing");
		this.FlyPet = Global.GetXElementAttributeStr(xml, "FlyPet");
		this.Scale = Global.GetXElementAttributeFloat(xml, "Scale");
		this.ShaderID = Global.GetXElementAttributeInt(xml, "ShaderID");
		this.GuaJieDian = Global.GetXElementAttributeStr(xml, "GuaJieDian");
		this.GuaJieTeXiao = Global.GetXElementAttributeStr(xml, "GuaJieTeXiao");
		this.Function = Global.GetXElementAttributeStr(xml, "Function");
		this.SName = Global.GetXElementAttributeStr(xml, "SName");
		this.MapCode = Global.GetXElementAttributeInt(xml, "MapCode");
		this.Talk = Global.GetXElementAttributeStr(xml, "Talk");
		this.SaleID = Global.GetXElementAttributeStr(xml, "SaleID");
		this.LuaScriptFile = Global.GetXElementAttributeStr(xml, "LuaScriptFile");
		this.Display = Global.GetXElementAttributeInt(xml, "Display");
		this.PlaySound = Global.GetXElementAttributeStr(xml, "PlaySound");
		this.Interval = Global.GetXElementAttributeFloat(xml, "Interval");
		this.TakeSound = Global.GetXElementAttributeStr(xml, "TakeSound");
		string[] array = Global.GetXElementAttributeStr(xml, "Collide").Split(new char[]
		{
			';'
		});
		if (array.Length == 2)
		{
			string[] array2 = array[0].Split(new char[]
			{
				','
			});
			this.CollideCenter = new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
			array2 = array[1].Split(new char[]
			{
				','
			});
			this.CollideSize = new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
			this.IsCollide = true;
		}
		array = Global.GetXElementAttributeStr(xml, "Obstacle").Split(new char[]
		{
			','
		});
		if (array.Length == 2)
		{
			this.ObstacleX = Convert.ToInt32(array[0]);
			this.ObstacleY = Convert.ToInt32(array[1]);
		}
		this.IsSafe = Global.GetXElementAttributeInt(xml, "IsSafe");
		this.GatherCondition = Global.GetXElementAttributeStr(xml, "GatherCondition");
	}

	public static Dictionary<string, int> PropertyIndexDict
	{
		get
		{
			if (NPCInfoVO._PropertyIndexDict == null)
			{
				NPCInfoVO._PropertyIndexDict = new Dictionary<string, int>();
				NPCInfoVO._PropertyIndexDict.Add("ID", 0);
				NPCInfoVO._PropertyIndexDict.Add("PicCode", 2);
				NPCInfoVO._PropertyIndexDict.Add("ResName", 3);
				NPCInfoVO._PropertyIndexDict.Add("YouShou", 4);
				NPCInfoVO._PropertyIndexDict.Add("ZuoShou", 5);
				NPCInfoVO._PropertyIndexDict.Add("Wing", 6);
				NPCInfoVO._PropertyIndexDict.Add("FlyPet", 7);
				NPCInfoVO._PropertyIndexDict.Add("Scale", 8);
				NPCInfoVO._PropertyIndexDict.Add("ShaderID", 9);
				NPCInfoVO._PropertyIndexDict.Add("GuaJieDian", 10);
				NPCInfoVO._PropertyIndexDict.Add("GuaJieTeXiao", 11);
				NPCInfoVO._PropertyIndexDict.Add("Function", 12);
				NPCInfoVO._PropertyIndexDict.Add("SName", 13);
				NPCInfoVO._PropertyIndexDict.Add("MapCode", 14);
				NPCInfoVO._PropertyIndexDict.Add("Talk", 15);
				NPCInfoVO._PropertyIndexDict.Add("SaleID", 19);
				NPCInfoVO._PropertyIndexDict.Add("LuaScriptFile", 20);
				NPCInfoVO._PropertyIndexDict.Add("Display", 22);
				NPCInfoVO._PropertyIndexDict.Add("PlaySound", 23);
				NPCInfoVO._PropertyIndexDict.Add("Interval", 24);
				NPCInfoVO._PropertyIndexDict.Add("TakeSound", 25);
				NPCInfoVO._PropertyIndexDict.Add("Collide", 26);
				NPCInfoVO._PropertyIndexDict.Add("Obstacle", 27);
				NPCInfoVO._PropertyIndexDict.Add("IsSafe", 28);
				NPCInfoVO._PropertyIndexDict.Add("GatherCondition", 29);
			}
			return NPCInfoVO._PropertyIndexDict;
		}
	}

	public void CopyFrom(TrdGoodVOPairs pairs)
	{
		if (TrdGoodVOPairs.IsCopySpeedUp())
		{
			pairs.StartGoodVOCopySpeedUp();
		}
		this.ID = pairs.PropertyAtOfInt("ID");
		this.PicCode = pairs.PropertyAtOfInt("PicCode");
		this.ResName = pairs.PropertyAtOfStr("ResName");
		this.YouShou = pairs.PropertyAtOfStr("YouShou");
		this.ZuoShou = pairs.PropertyAtOfStr("ZuoShou");
		this.Wing = pairs.PropertyAtOfStr("Wing");
		this.FlyPet = pairs.PropertyAtOfStr("FlyPet");
		this.Scale = float.Parse(pairs.PropertyAtOfStr("Scale"));
		this.ShaderID = pairs.PropertyAtOfInt("ShaderID");
		this.GuaJieDian = pairs.PropertyAtOfStr("GuaJieDian");
		this.GuaJieTeXiao = pairs.PropertyAtOfStr("GuaJieTeXiao");
		this.Function = pairs.PropertyAtOfStr("Function");
		this.SName = pairs.PropertyAtOfStr("SName");
		this.MapCode = pairs.PropertyAtOfInt("MapCode");
		this.Talk = pairs.PropertyAtOfStr("Talk");
		this.SaleID = pairs.PropertyAtOfStr("SaleID");
		this.LuaScriptFile = pairs.PropertyAtOfStr("LuaScriptFile");
		this.Display = pairs.PropertyAtOfInt("Display");
		this.PlaySound = pairs.PropertyAtOfStr("PlaySound");
		this.Interval = (float)ConvertExt.SafeConvertToDouble(pairs.PropertyAtOfStr("Interval"));
		this.TakeSound = pairs.PropertyAtOfStr("TakeSound");
		string[] array = pairs.PropertyAtOfStr("Collide").Split(new char[]
		{
			';'
		});
		if (array.Length == 3)
		{
			string[] array2 = array[0].Split(new char[]
			{
				','
			});
			this.CollideCenter = new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
			array2 = array[1].Split(new char[]
			{
				','
			});
			this.CollideSize = new Vector3(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
			int num = ConvertExt.SafeConvertToInt32(array[2]);
			this.IsCollide = (num == 1);
		}
		string[] array3 = pairs.PropertyAtOfStr("Obstacle").Split(new char[]
		{
			','
		});
		if (array3.Length == 2)
		{
			this.ObstacleX = Convert.ToInt32(array3[0]);
			this.ObstacleY = Convert.ToInt32(array3[1]);
		}
		this.IsSafe = pairs.PropertyAtOfInt("IsSafe");
		this.GatherCondition = pairs.PropertyAtOfStr("GatherCondition");
	}

	public List<byte> ToByteLst(NPCInfoVO preVO, NPCInfoVO curVO)
	{
		return new List<byte>();
	}

	public bool Equals(NPCInfoVO obj)
	{
		if (obj == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"dest is null"
			});
			return false;
		}
		return true;
	}

	public int ID;

	public int PicCode;

	public string ResName;

	public string YouShou;

	public string ZuoShou;

	public string Wing;

	public string FlyPet;

	public float Scale;

	public int ShaderID;

	public string GuaJieDian;

	public string GuaJieTeXiao;

	public string Function;

	public string SName;

	public int MapCode;

	public string Talk;

	public string SaleID;

	public string LuaScriptFile;

	public int Display;

	public string PlaySound;

	public float Interval;

	public string TakeSound;

	public bool IsCollide;

	public Vector3 CollideCenter = Vector3.zero;

	public Vector3 CollideSize = Vector3.zero;

	public int ObstacleX;

	public int ObstacleY;

	public int IsSafe = -1;

	public string GatherCondition;

	private static Dictionary<string, int> _PropertyIndexDict;
}
