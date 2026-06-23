using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TaskVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TaskClass = Global.GetXElementAttributeInt(xml, "TaskClass");
		this.Title = Global.GetXElementAttributeStr(xml, "Title");
		this.GuideText = Global.GetXElementAttributeStr(xml, "GuideText");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.Description2 = Global.GetXElementAttributeStr(xml, "Description2");
		this.PrevTask = Global.GetXElementAttributeInt(xml, "PrevTask");
		this.NextTask = Global.GetXElementAttributeInt(xml, "NextTask");
		this.MinZhuanSheng = Global.GetXElementAttributeInt(xml, "MinZhuanSheng");
		this.MinLevel = Global.GetXElementAttributeInt(xml, "MinLevel");
		this.MaxLevel = Global.GetXElementAttributeInt(xml, "MaxLevel");
		this.MaxRedoing = Global.GetXElementAttributeInt(xml, "MaxRedoing");
		this.Taketime = Global.GetXElementAttributeInt(xml, "Taketime");
		this.LimitZhuanSheng = Global.GetXElementAttributeInt(xml, "LimitZhuanSheng");
		this.LimitLevel = Global.GetXElementAttributeInt(xml, "LimitLevel");
		this.AcceptTalk = Global.GetXElementAttributeStr(xml, "AcceptTalk");
		this.DoingTalk = Global.GetXElementAttributeStr(xml, "DoingTalk");
		this.CompleteTalk = Global.GetXElementAttributeStr(xml, "CompleteTalk");
		this.SourceNPC = Global.GetXElementAttributeInt(xml, "SourceNPC");
		this.DestNPC = Global.GetXElementAttributeInt(xml, "DestNPC");
		this.TargetType1 = Global.GetXElementAttributeInt(xml, "TargetType1");
		this.TargetNPC1 = Global.GetXElementAttributeInt(xml, "TargetNPC1");
		this.PropsName1 = Global.GetXElementAttributeStr(xml, "PropsName1");
		this.TargetNum1 = Global.GetXElementAttributeInt(xml, "TargetNum1");
		this.TargetMapCode1 = Global.GetXElementAttributeInt(xml, "TargetMapCode1");
		this.TargetPos1 = Global.GetXElementAttributeStr(xml, "TargetPos1");
		this.TargetType2 = Global.GetXElementAttributeInt(xml, "TargetType2");
		this.TargetNPC2 = Global.GetXElementAttributeInt(xml, "TargetNPC2");
		this.PropsName2 = Global.GetXElementAttributeStr(xml, "PropsName2");
		this.TargetNum2 = Global.GetXElementAttributeInt(xml, "TargetNum2");
		this.TargetMapCode2 = Global.GetXElementAttributeInt(xml, "TargetMapCode2");
		this.TargetPos2 = Global.GetXElementAttributeStr(xml, "TargetPos2");
		this.PubStartTime = Global.GetXElementAttributeStr(xml, "PubStartTime");
		this.PubEndTime = Global.GetXElementAttributeStr(xml, "PubEndTime");
		this.Teleports = Global.GetXElementAttributeInt(xml, "Teleports");
		this.YuanBaoComplete = Global.GetXElementAttributeInt(xml, "YuanBaoComplete");
		this.TaskGuild = Global.GetXElementAttributeStr(xml, "TaskGuild");
		this.TaskTiJiao = Global.GetXElementAttributeStr(xml, "TaskTiJiao");
		this.ZiDongTask = Global.GetXElementAttributeStr(xml, "ZiDongTask");
		this.ChenJiuID = Global.GetXElementAttributeInt(xml, "ChenJiuID");
		this.LinkID = Global.GetXElementAttributeInt(xml, "LinkID");
		this.NeedTargetNum = Global.GetXElementAttributeInt(xml, "NeedTargetNum");
		this.FallGoods = Global.GetXElementAttributeStr(xml, "FallGoods");
		this.Star = Global.GetXElementAttributeInt(xml, "Star");
		this.CompID = Global.GetXElementAttributeInt(xml, "CompID");
		this.AwardCompHonor = Global.GetXElementAttributeInt(xml, "AwardCompHonor");
		this.AwardCompFeast = Global.GetXElementAttributeInt(xml, "AwardCompFeast");
		this.LevelYuGao = Global.GetXElementAttributeStr(xml, "LevelYuGao");
	}

	public static Dictionary<string, int> PropertyIndexDict
	{
		get
		{
			if (TaskVO._PropertyIndexDict == null)
			{
				TaskVO._PropertyIndexDict = new Dictionary<string, int>();
				TaskVO._PropertyIndexDict.Add("ID", 0);
				TaskVO._PropertyIndexDict.Add("TaskClass", 1);
				TaskVO._PropertyIndexDict.Add("Title", 2);
				TaskVO._PropertyIndexDict.Add("GuideText", 3);
				TaskVO._PropertyIndexDict.Add("Description", 4);
				TaskVO._PropertyIndexDict.Add("Description2", 5);
				TaskVO._PropertyIndexDict.Add("PrevTask", 6);
				TaskVO._PropertyIndexDict.Add("NextTask", 7);
				TaskVO._PropertyIndexDict.Add("MinZhuanSheng", 8);
				TaskVO._PropertyIndexDict.Add("MinLevel", 9);
				TaskVO._PropertyIndexDict.Add("MaxLevel", 10);
				TaskVO._PropertyIndexDict.Add("MaxRedoing", 11);
				TaskVO._PropertyIndexDict.Add("Taketime", 12);
				TaskVO._PropertyIndexDict.Add("LimitZhuanSheng", 13);
				TaskVO._PropertyIndexDict.Add("LimitLevel", 14);
				TaskVO._PropertyIndexDict.Add("AcceptTalk", 15);
				TaskVO._PropertyIndexDict.Add("DoingTalk", 16);
				TaskVO._PropertyIndexDict.Add("CompleteTalk", 18);
				TaskVO._PropertyIndexDict.Add("SourceNPC", 19);
				TaskVO._PropertyIndexDict.Add("DestNPC", 20);
				TaskVO._PropertyIndexDict.Add("TargetType1", 21);
				TaskVO._PropertyIndexDict.Add("TargetNPC1", 23);
				TaskVO._PropertyIndexDict.Add("PropsName1", 24);
				TaskVO._PropertyIndexDict.Add("TargetNum1", 25);
				TaskVO._PropertyIndexDict.Add("TargetMapCode1", 26);
				TaskVO._PropertyIndexDict.Add("TargetPos1", 28);
				TaskVO._PropertyIndexDict.Add("TargetType2", 29);
				TaskVO._PropertyIndexDict.Add("TargetNPC2", 30);
				TaskVO._PropertyIndexDict.Add("PropsName2", 31);
				TaskVO._PropertyIndexDict.Add("TargetNum2", 36);
				TaskVO._PropertyIndexDict.Add("TargetMapCode2", 37);
				TaskVO._PropertyIndexDict.Add("TargetPos2", 38);
				TaskVO._PropertyIndexDict.Add("PubStartTime", 41);
				TaskVO._PropertyIndexDict.Add("PubEndTime", 42);
				TaskVO._PropertyIndexDict.Add("Teleports", 44);
				TaskVO._PropertyIndexDict.Add("YuanBaoComplete", 45);
				TaskVO._PropertyIndexDict.Add("TaskZhangJieID", 46);
				TaskVO._PropertyIndexDict.Add("TaskIndexOfZhangJie", 47);
				TaskVO._PropertyIndexDict.Add("TaskGuild", 60);
				TaskVO._PropertyIndexDict.Add("TaskTiJiao", 61);
				TaskVO._PropertyIndexDict.Add("ZiDongTask", 62);
				TaskVO._PropertyIndexDict.Add("ChenJiuID", 63);
				TaskVO._PropertyIndexDict.Add("LinkID", 64);
				TaskVO._PropertyIndexDict.Add("NeedTargetNum", 65);
				TaskVO._PropertyIndexDict.Add("FallGoods", 66);
				TaskVO._PropertyIndexDict.Add("Star", 67);
				TaskVO._PropertyIndexDict.Add("CompID", 68);
				TaskVO._PropertyIndexDict.Add("AwardCompHonor", 69);
				TaskVO._PropertyIndexDict.Add("AwardCompFeast", 70);
				TaskVO._PropertyIndexDict.Add("LevelYuGao", 71);
			}
			return TaskVO._PropertyIndexDict;
		}
	}

	public void CopyFrom(TrdGoodVOPairs pairs)
	{
		if (TrdGoodVOPairs.IsCopySpeedUp())
		{
			pairs.StartGoodVOCopySpeedUp();
		}
		this.ID = pairs.PropertyAtOfInt("ID");
		this.TaskClass = pairs.PropertyAtOfInt("TaskClass");
		this.Title = pairs.PropertyAtOfStr("Title");
		this.GuideText = pairs.PropertyAtOfStr("GuideText");
		this.Description = pairs.PropertyAtOfStr("Description");
		this.Description2 = pairs.PropertyAtOfStr("Description2");
		this.PrevTask = pairs.PropertyAtOfInt("PrevTask");
		this.NextTask = pairs.PropertyAtOfInt("NextTask");
		this.MinZhuanSheng = pairs.PropertyAtOfInt("MinZhuanSheng");
		this.MinLevel = pairs.PropertyAtOfInt("MinLevel");
		this.MaxLevel = pairs.PropertyAtOfInt("MaxLevel");
		this.MaxRedoing = pairs.PropertyAtOfInt("MaxRedoing");
		this.Taketime = pairs.PropertyAtOfInt("Taketime");
		this.LimitZhuanSheng = pairs.PropertyAtOfInt("LimitZhuanSheng");
		this.LimitLevel = pairs.PropertyAtOfInt("LimitLevel");
		this.AcceptTalk = pairs.PropertyAtOfStr("AcceptTalk");
		this.DoingTalk = pairs.PropertyAtOfStr("DoingTalk");
		this.CompleteTalk = pairs.PropertyAtOfStr("CompleteTalk");
		this.SourceNPC = pairs.PropertyAtOfInt("SourceNPC");
		this.DestNPC = pairs.PropertyAtOfInt("DestNPC");
		this.TargetType1 = pairs.PropertyAtOfInt("TargetType1");
		this.TargetNPC1 = pairs.PropertyAtOfInt("TargetNPC1");
		this.PropsName1 = pairs.PropertyAtOfStr("PropsName1");
		this.TargetNum1 = pairs.PropertyAtOfInt("TargetNum1");
		this.TargetMapCode1 = pairs.PropertyAtOfInt("TargetMapCode1");
		this.TargetPos1 = pairs.PropertyAtOfStr("TargetPos1");
		this.TargetType2 = pairs.PropertyAtOfInt("TargetType2");
		this.TargetNPC2 = pairs.PropertyAtOfInt("TargetNPC2");
		this.PropsName2 = pairs.PropertyAtOfStr("PropsName2");
		this.TargetNum2 = pairs.PropertyAtOfInt("TargetNum2");
		this.TargetMapCode2 = pairs.PropertyAtOfInt("TargetMapCode2");
		this.TargetPos2 = pairs.PropertyAtOfStr("TargetPos2");
		this.PubStartTime = pairs.PropertyAtOfStr("PubStartTime");
		this.PubEndTime = pairs.PropertyAtOfStr("PubEndTime");
		this.Teleports = pairs.PropertyAtOfInt("Teleports");
		this.YuanBaoComplete = pairs.PropertyAtOfInt("YuanBaoComplete");
		this.TaskZhangJieID = pairs.PropertyAtOfInt("TaskZhangJieID");
		this.TaskIndexOfZhangJie = pairs.PropertyAtOfInt("TaskIndexOfZhangJie");
		this.TaskGuild = pairs.PropertyAtOfStr("TaskGuild");
		this.TaskTiJiao = pairs.PropertyAtOfStr("TaskTiJiao");
		this.ZiDongTask = pairs.PropertyAtOfStr("ZiDongTask");
		this.ChenJiuID = pairs.PropertyAtOfInt("ChenJiuID");
		this.LinkID = pairs.PropertyAtOfInt("LinkID");
		this.NeedTargetNum = pairs.PropertyAtOfInt("NeedTargetNum");
		this.FallGoods = pairs.PropertyAtOfStr("FallGoods");
		this.Star = pairs.PropertyAtOfInt("Star");
		this.CompID = pairs.PropertyAtOfInt("CompID");
		this.AwardCompHonor = pairs.PropertyAtOfInt("AwardCompHonor");
		this.AwardCompFeast = pairs.PropertyAtOfInt("AwardCompFeast");
		this.LevelYuGao = pairs.PropertyAtOfStr("LevelYuGao");
	}

	public List<byte> ToByteLst(TaskVO preVO, TaskVO curVO)
	{
		return new List<byte>();
	}

	public bool Equals(TaskVO obj)
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

	public int TaskClass;

	public string Title;

	public string GuideText;

	public string Description;

	public string Description2;

	public int PrevTask;

	public int NextTask;

	public int MinZhuanSheng;

	public int MinLevel;

	public int MaxLevel;

	public int MaxRedoing;

	public int Taketime;

	public int LimitZhuanSheng;

	public int LimitLevel;

	public string AcceptTalk;

	public string DoingTalk;

	public string CompleteTalk;

	public int SourceNPC;

	public int DestNPC;

	public int TargetType1;

	public int TargetNPC1;

	public string PropsName1;

	public int TargetNum1;

	public int TargetMapCode1;

	public string TargetPos1;

	public int TargetType2;

	public int TargetNPC2;

	public string PropsName2;

	public int TargetNum2;

	public int TargetMapCode2;

	public string TargetPos2;

	public string PubStartTime;

	public string PubEndTime;

	public int Teleports;

	public int YuanBaoComplete;

	public int TaskZhangJieID;

	public int TaskIndexOfZhangJie;

	public string TaskGuild;

	public string TaskTiJiao;

	public string ZiDongTask;

	public int ChenJiuID;

	public int LinkID;

	public int NeedTargetNum;

	public string FallGoods;

	public int Star;

	public int CompID;

	public int AwardCompHonor;

	public int AwardCompFeast;

	public string LevelYuGao;

	private static Dictionary<string, int> _PropertyIndexDict;
}
