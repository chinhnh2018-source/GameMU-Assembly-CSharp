using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class MonsterVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.PicCode = Global.GetXElementAttributeInt(xml, "PicCode");
		this.ResName = Global.GetXElementAttributeStr(xml, "ResName");
		this.YouShou = Global.GetXElementAttributeStr(xml, "YouShou");
		this.ZuoShou = Global.GetXElementAttributeStr(xml, "ZuoShou");
		this.Scale = Global.GetXElementAttributeFloat(xml, "Scale");
		this.ShaderID = Global.GetXElementAttributeInt(xml, "ShaderID");
		this.GuaJieDian = Global.GetXElementAttributeStr(xml, "GuaJieDian");
		this.GuaJieTeXiao = Global.GetXElementAttributeStr(xml, "GuaJieTeXiao");
		this.SName = Global.GetXElementAttributeStr(xml, "SName");
		this.MapCode = Global.GetXElementAttributeInt(xml, "MapCode");
		this.Talk = Global.GetXElementAttributeStr(xml, "Talk");
		this.MaxLife = Global.GetXElementAttributeInt(xml, "MaxLife");
		this.SeedRange = Global.GetXElementAttributeInt(xml, "SeedRange");
		this.MonsterType = Global.GetXElementAttributeInt(xml, "MonsterType");
		this.XueTiaoType = Global.GetXElementAttributeIntArray(xml, "XueTiaoType", "*");
		this.Display = Global.GetXElementAttributeInt(xml, "Display");
		this.SkillIDs = Global.GetXElementAttributeStr(xml, "SkillIDs");
		this.PlaySound = Global.GetXElementAttributeStr(xml, "PlaySound");
		this.AttackSound = Global.GetXElementAttributeStr(xml, "AttackSound");
		this.HitSound = Global.GetXElementAttributeStr(xml, "HitSound");
		this.DieSound = Global.GetXElementAttributeStr(xml, "DieSound");
		this.ComeAnimation = Global.GetXElementAttributeInt(xml, "ComeAnimation");
		this.DieAnimation = Global.GetXElementAttributeInt(xml, "DieAnimation");
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
	}

	public static Dictionary<string, int> PropertyIndexDict
	{
		get
		{
			if (MonsterVO._PropertyIndexDict == null)
			{
				MonsterVO._PropertyIndexDict = new Dictionary<string, int>();
				MonsterVO._PropertyIndexDict.Add("ID", 0);
				MonsterVO._PropertyIndexDict.Add("PicCode", 2);
				MonsterVO._PropertyIndexDict.Add("ResName", 3);
				MonsterVO._PropertyIndexDict.Add("YouShou", 4);
				MonsterVO._PropertyIndexDict.Add("ZuoShou", 5);
				MonsterVO._PropertyIndexDict.Add("Scale", 6);
				MonsterVO._PropertyIndexDict.Add("ShaderID", 7);
				MonsterVO._PropertyIndexDict.Add("GuaJieDian", 8);
				MonsterVO._PropertyIndexDict.Add("GuaJieTeXiao", 9);
				MonsterVO._PropertyIndexDict.Add("SName", 12);
				MonsterVO._PropertyIndexDict.Add("MapCode", 13);
				MonsterVO._PropertyIndexDict.Add("Talk", 14);
				MonsterVO._PropertyIndexDict.Add("MaxLife", 25);
				MonsterVO._PropertyIndexDict.Add("SeedRange", 41);
				MonsterVO._PropertyIndexDict.Add("MonsterType", 43);
				MonsterVO._PropertyIndexDict.Add("XueTiaoType", 44);
				MonsterVO._PropertyIndexDict.Add("Display", 49);
				MonsterVO._PropertyIndexDict.Add("SkillIDs", 58);
				MonsterVO._PropertyIndexDict.Add("PlaySound", 62);
				MonsterVO._PropertyIndexDict.Add("AttackSound", 63);
				MonsterVO._PropertyIndexDict.Add("HitSound", 64);
				MonsterVO._PropertyIndexDict.Add("DieSound", 65);
				MonsterVO._PropertyIndexDict.Add("ComeAnimation", 59);
				MonsterVO._PropertyIndexDict.Add("DieAnimation", 60);
				MonsterVO._PropertyIndexDict.Add("Collide", 66);
			}
			return MonsterVO._PropertyIndexDict;
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
		this.Scale = ConvertExt.SafeConvertToFloat(pairs.PropertyAtOfStr("Scale"), 0f);
		this.ShaderID = pairs.PropertyAtOfInt("ShaderID");
		this.GuaJieDian = pairs.PropertyAtOfStr("GuaJieDian");
		this.GuaJieTeXiao = pairs.PropertyAtOfStr("GuaJieTeXiao");
		this.SName = pairs.PropertyAtOfStr("SName");
		this.MapCode = pairs.PropertyAtOfInt("MapCode");
		this.Talk = pairs.PropertyAtOfStr("Talk");
		this.MaxLife = pairs.PropertyAtOfInt("MaxLife");
		this.SeedRange = pairs.PropertyAtOfInt("ID");
		this.SeedRange = pairs.PropertyAtOfInt("SeedRange");
		this.MonsterType = pairs.PropertyAtOfInt("MonsterType");
		this.XueTiaoType = ConvertExt.String2IntArray(pairs.PropertyAtOfStr("XueTiaoType"), ',');
		this.Display = pairs.PropertyAtOfInt("Display");
		this.SkillIDs = pairs.PropertyAtOfStr("SkillIDs");
		this.PlaySound = pairs.PropertyAtOfStr("PlaySound");
		this.AttackSound = pairs.PropertyAtOfStr("AttackSound");
		this.HitSound = pairs.PropertyAtOfStr("HitSound");
		this.DieSound = pairs.PropertyAtOfStr("DieSound");
		this.ComeAnimation = pairs.PropertyAtOfInt("ComeAnimation");
		this.DieAnimation = pairs.PropertyAtOfInt("DieAnimation");
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
	}

	public List<byte> ToByteLst(MonsterVO preVO, MonsterVO curVO)
	{
		return new List<byte>();
	}

	public bool Equals(MonsterVO obj)
	{
		return obj != null;
	}

	public int ID;

	public int PicCode;

	public string ResName;

	public string YouShou;

	public string ZuoShou;

	public float Scale;

	public int ShaderID;

	public string GuaJieDian;

	public string GuaJieTeXiao;

	public string SName;

	public int MapCode;

	public string Talk;

	public int MaxLife;

	public int SeedRange;

	public int MonsterType;

	public int[] XueTiaoType;

	public int Display;

	public string SkillIDs;

	public string PlaySound;

	public string AttackSound;

	public string HitSound;

	public string DieSound;

	public int ComeAnimation;

	public int DieAnimation;

	public Vector3 CollideCenter = Vector3.zero;

	public Vector3 CollideSize = Vector3.zero;

	public bool IsCollide;

	private static Dictionary<string, int> _PropertyIndexDict;
}
