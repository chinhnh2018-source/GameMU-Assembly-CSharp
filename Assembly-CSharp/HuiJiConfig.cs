using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class HuiJiConfig
{
	private ResourceResLoader AddDicUIModel(int modelID, GameObject show3DModel)
	{
		XElement gameResXml = Global.GetGameResXml("Config/UIModel.xml");
		if (gameResXml == null)
		{
			return null;
		}
		if (Global.GetXElement(gameResXml, "Mod", "ID", modelID.ToString()) == null)
		{
			return null;
		}
		Modal3DShow modal3DShow = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(show3DModel, modal3DShow.gameObject, false);
		return UIHelper.LoadModelResource(modal3DShow, modelID, 1f, this.LoadCompleteCallBack = delegate(object e, DPSelectedItemEventArgs s)
		{
			Modal3DShow componentInChildren = show3DModel.GetComponentInChildren<Modal3DShow>();
			if (componentInChildren != null)
			{
				SkinnedMeshRenderer[] componentsInChildren = componentInChildren.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (componentsInChildren == null || componentsInChildren.Length <= 0)
				{
					return;
				}
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if ("Artist/PlayerCharacter".Equals(componentsInChildren[i].sharedMaterial.shader.name))
					{
						componentsInChildren[i].sharedMaterial.shader = Shader.Find("Artist/PlayerCharacterForUI");
					}
				}
			}
		});
	}

	private void AddDicEmblemUp()
	{
		XElement gameResXml = Global.GetGameResXml(this.EMBLEMUPPATH);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EmblemUp");
		if (xelementList == null)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			EmblemUp emblemUp = new EmblemUp();
			emblemUp.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			emblemUp.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			emblemUp.EmblemLevel = Global.GetXElementAttributeInt(xelementList[i], "EmblemLevel");
			emblemUp.ModID = Global.GetXElementAttributeInt(xelementList[i], "ModID");
			emblemUp.EmblemIcon = Global.GetXElementAttributeStr(xelementList[i], "EmblemIcon");
			emblemUp.LuckyOne = Global.GetXElementAttributeInt(xelementList[i], "LuckyOne");
			emblemUp.LuckyTwo = Global.GetXElementAttributeInt(xelementList[i], "LuckyTwo");
			emblemUp.LuckyTwoRate = Global.GetXElementAttributeStr(xelementList[i], "LuckyTwoRate");
			emblemUp.Instructions = Global.GetXElementAttributeStr(xelementList[i], "Instructions");
			emblemUp.DurationTime = Global.GetXElementAttributeFloat(xelementList[i], "DurationTime");
			emblemUp.CDTime = Global.GetXElementAttributeFloat(xelementList[i], "CDTime");
			emblemUp.SubAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "SubAttackInjurePercent");
			emblemUp.SPAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "SPAttackInjurePercent");
			emblemUp.AttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "AttackInjurePercent");
			emblemUp.ElementAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "ElementAttackInjurePercent");
			emblemUp.LifeV = Global.GetXElementAttributeInt(xelementList[i], "LifeV");
			emblemUp.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
			emblemUp.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
			emblemUp.DecreaseInjureValue = Global.GetXElementAttributeInt(xelementList[i], "DecreaseInjureValue");
			emblemUp.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
			emblemUp.NeedDiamond = Global.GetXElementAttributeInt(xelementList[i], "NeedDiamond");
			emblemUp.Decorations = Global.GetXElementAttributeStr(xelementList[i], "Decorations");
			if (!this.m_DicEmblemData.ContainsKey(emblemUp.EmblemLevel))
			{
				EmblemData emblemData = new EmblemData();
				emblemData.emblemUp = emblemUp;
				this.m_DicEmblemData.Add(emblemUp.EmblemLevel, emblemData);
			}
			else
			{
				this.m_DicEmblemData[emblemUp.EmblemLevel].emblemUp = emblemUp;
			}
		}
	}

	private void AddDicEmblemStart()
	{
		Dictionary<int, EmblemStarXml> dictionary = new Dictionary<int, EmblemStarXml>();
		XElement gameResXml = Global.GetGameResXml(this.EMBLEMSTARTPATH);
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EmblemStar");
		if (xelementList == null)
		{
			return;
		}
		for (int i = 0; i < xelementList.Count; i++)
		{
			EmblemStarXml emblemStarXml = new EmblemStarXml();
			emblemStarXml.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			emblemStarXml.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			emblemStarXml.EmblemLevel = Global.GetXElementAttributeInt(xelementList[i], "EmblemLevel");
			emblemStarXml.EmblemStar = Global.GetXElementAttributeInt(xelementList[i], "EmblemStar");
			emblemStarXml.ModID = Global.GetXElementAttributeInt(xelementList[i], "ModID");
			emblemStarXml.LifeV = Global.GetXElementAttributeInt(xelementList[i], "LifeV");
			emblemStarXml.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
			emblemStarXml.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
			emblemStarXml.DecreaseInjureValue = Global.GetXElementAttributeInt(xelementList[i], "DecreaseInjureValue");
			emblemStarXml.StarExp = Global.GetXElementAttributeInt(xelementList[i], "StarExp");
			emblemStarXml.GoodsExp = Global.GetXElementAttributeInt(xelementList[i], "GoodsExp");
			emblemStarXml.ZuanShiExp = Global.GetXElementAttributeInt(xelementList[i], "ZuanShiExp");
			emblemStarXml.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
			emblemStarXml.NeedDiamond = Global.GetXElementAttributeInt(xelementList[i], "NeedDiamond");
			if (!this.m_DicStar.ContainsKey(emblemStarXml.ID))
			{
				this.m_DicStar[emblemStarXml.ID] = emblemStarXml;
			}
			else
			{
				this.m_DicStar.Add(emblemStarXml.ID, emblemStarXml);
			}
			if (!this.m_DicEmblemData.ContainsKey(emblemStarXml.EmblemLevel))
			{
				EmblemData emblemData = new EmblemData();
				emblemData.emblemStartDic = new Dictionary<int, EmblemStarXml>();
				this.m_DicEmblemData.Add(emblemStarXml.EmblemLevel, emblemData);
			}
			else
			{
				if (this.m_DicEmblemData[emblemStarXml.EmblemLevel].emblemStartDic == null)
				{
					this.m_DicEmblemData[emblemStarXml.EmblemLevel].emblemStartDic = new Dictionary<int, EmblemStarXml>();
				}
				if (!this.m_DicEmblemData[emblemStarXml.EmblemLevel].emblemStartDic.ContainsKey(emblemStarXml.EmblemStar))
				{
					this.m_DicEmblemData[emblemStarXml.EmblemLevel].emblemStartDic.Add(emblemStarXml.EmblemStar, emblemStarXml);
				}
			}
		}
	}

	public void Init()
	{
		this.AddDicEmblemUp();
		this.AddDicEmblemStart();
	}

	public EmblemUp GetEmblemUp(int upLevel)
	{
		if (!this.m_DicEmblemData.ContainsKey(upLevel))
		{
			return null;
		}
		return this.m_DicEmblemData[upLevel].emblemUp;
	}

	public void EmblemUpAdd(int upLevel, out float SubAttackInjurePercent, out float DurationTime, out float CDTime)
	{
		SubAttackInjurePercent = -1f;
		DurationTime = -1f;
		CDTime = -1f;
		if (!this.m_DicEmblemData.ContainsKey(upLevel) || this.m_DicEmblemData.ContainsKey(upLevel + 1))
		{
			return;
		}
		SubAttackInjurePercent = this.m_DicEmblemData[upLevel + 1].emblemUp.SubAttackInjurePercent - this.m_DicEmblemData[upLevel].emblemUp.SubAttackInjurePercent;
		DurationTime = this.m_DicEmblemData[upLevel + 1].emblemUp.DurationTime - this.m_DicEmblemData[upLevel].emblemUp.DurationTime;
		CDTime = this.m_DicEmblemData[upLevel + 1].emblemUp.CDTime - this.m_DicEmblemData[upLevel].emblemUp.CDTime;
	}

	public EmblemStarXml EmblemStar(int upLevel, int startLevel)
	{
		if (!this.m_DicEmblemData.ContainsKey(upLevel))
		{
			return null;
		}
		if (this.m_DicEmblemData[upLevel].emblemStartDic.ContainsKey(startLevel))
		{
			return this.m_DicEmblemData[upLevel].emblemStartDic[startLevel];
		}
		return null;
	}

	public void EmblemStarAdd(HuiJiStateType type, int upLevel, int startLevel, out int LifeV, out int AddAttack, out int AddDefense, out int LifeSteal)
	{
		LifeV = 0;
		AddAttack = 0;
		AddDefense = 0;
		LifeSteal = 0;
		if (type == HuiJiStateType.UpLevel)
		{
			LifeV = this.m_DicEmblemData[upLevel + 1].emblemUp.LifeV - this.m_DicEmblemData[upLevel].emblemUp.LifeV;
			AddAttack = this.m_DicEmblemData[upLevel + 1].emblemUp.AddAttack - this.m_DicEmblemData[upLevel].emblemUp.AddAttack;
			AddDefense = this.m_DicEmblemData[upLevel + 1].emblemUp.AddDefense - this.m_DicEmblemData[upLevel].emblemUp.AddDefense;
			LifeSteal = this.m_DicEmblemData[upLevel + 1].emblemUp.DecreaseInjureValue - this.m_DicEmblemData[upLevel].emblemUp.DecreaseInjureValue;
		}
		else if (type != HuiJiStateType.MaxLevel && type != HuiJiStateType.None)
		{
			LifeV = this.m_DicEmblemData[upLevel].emblemStartDic[startLevel + 1].LifeV - this.m_DicEmblemData[upLevel].emblemStartDic[startLevel].LifeV;
			AddAttack = this.m_DicEmblemData[upLevel].emblemStartDic[startLevel + 1].AddAttack - this.m_DicEmblemData[upLevel].emblemStartDic[startLevel].AddAttack;
			AddDefense = this.m_DicEmblemData[upLevel].emblemStartDic[startLevel + 1].AddDefense - this.m_DicEmblemData[upLevel].emblemStartDic[startLevel].AddDefense;
			LifeSteal = this.m_DicEmblemData[upLevel].emblemStartDic[startLevel + 1].DecreaseInjureValue - this.m_DicEmblemData[upLevel].emblemStartDic[startLevel].DecreaseInjureValue;
			return;
		}
	}

	public EmblemUp EmblemUpAdd(HuiJiStateType type, int upLevel)
	{
		EmblemUp emblemUp = new EmblemUp();
		if (type == HuiJiStateType.UpLevel)
		{
			emblemUp.SubAttackInjurePercent = this.m_DicEmblemData[upLevel + 1].emblemUp.SubAttackInjurePercent - this.m_DicEmblemData[upLevel].emblemUp.SubAttackInjurePercent;
			emblemUp.SPAttackInjurePercent = this.m_DicEmblemData[upLevel + 1].emblemUp.SPAttackInjurePercent - this.m_DicEmblemData[upLevel].emblemUp.SPAttackInjurePercent;
			emblemUp.AttackInjurePercent = this.m_DicEmblemData[upLevel + 1].emblemUp.AttackInjurePercent - this.m_DicEmblemData[upLevel].emblemUp.AttackInjurePercent;
			emblemUp.ElementAttackInjurePercent = this.m_DicEmblemData[upLevel + 1].emblemUp.ElementAttackInjurePercent - this.m_DicEmblemData[upLevel].emblemUp.ElementAttackInjurePercent;
			emblemUp.DurationTime = this.m_DicEmblemData[upLevel + 1].emblemUp.DurationTime - this.m_DicEmblemData[upLevel].emblemUp.DurationTime;
			emblemUp.CDTime = this.m_DicEmblemData[upLevel + 1].emblemUp.CDTime - this.m_DicEmblemData[upLevel].emblemUp.CDTime;
		}
		return emblemUp;
	}

	public ResourceResLoader DicUIModel(int key, GameObject show3DModel)
	{
		Modal3DShow componentInChildren = show3DModel.GetComponentInChildren<Modal3DShow>();
		if (componentInChildren != null)
		{
			componentInChildren.Clear();
			Object.DestroyObject(componentInChildren.gameObject);
		}
		return this.AddDicUIModel(key, show3DModel);
	}

	public HuiJiStateType GetStateType(int upLevel, int star)
	{
		if (!this.m_DicEmblemData.ContainsKey(upLevel))
		{
			return HuiJiStateType.None;
		}
		int num = -1;
		EmblemStarXml emblemStarXml = this.HuiJiStar(Global.Data.roleData.HuiJiData.huiji);
		if (emblemStarXml != null)
		{
			num = emblemStarXml.EmblemLevel;
		}
		int[] array = new int[this.m_DicEmblemData.Count];
		int[] array2 = new int[this.m_DicEmblemData[upLevel].emblemStartDic.Count];
		int num2 = 0;
		Dictionary<int, EmblemData>.Enumerator enumerator = this.m_DicEmblemData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int[] array3 = array;
			int num3 = num2;
			KeyValuePair<int, EmblemData> keyValuePair = enumerator.Current;
			array3[num3] = keyValuePair.Value.emblemUp.EmblemLevel;
			num2++;
		}
		Dictionary<int, EmblemStarXml>.Enumerator enumerator2 = this.m_DicEmblemData[upLevel].emblemStartDic.GetEnumerator();
		int num4 = 0;
		while (enumerator2.MoveNext())
		{
			int[] array4 = array2;
			int num5 = num4;
			KeyValuePair<int, EmblemStarXml> keyValuePair2 = enumerator2.Current;
			array4[num5] = keyValuePair2.Value.EmblemStar;
			num4++;
		}
		if (Mathf.Max(array2) > star && num >= upLevel)
		{
			return HuiJiStateType.StartLevel;
		}
		if (Mathf.Max(array2) <= star && Mathf.Max(array) > upLevel && num >= upLevel)
		{
			return HuiJiStateType.UpLevel;
		}
		if (Mathf.Max(array2) <= star && Mathf.Max(array) <= upLevel && num >= upLevel)
		{
			return HuiJiStateType.MaxLevel;
		}
		return HuiJiStateType.None;
	}

	public int GetMaxStarLevel(int upLevel)
	{
		if (this.m_DicEmblemData.ContainsKey(upLevel) && this.m_DicEmblemData[upLevel].emblemStartDic != null && this.m_DicEmblemData[upLevel].emblemStartDic.Count > 0)
		{
			int num = 0;
			foreach (KeyValuePair<int, EmblemStarXml> keyValuePair in this.m_DicEmblemData[upLevel].emblemStartDic)
			{
				if (keyValuePair.Value.EmblemStar > num)
				{
					Dictionary<int, EmblemStarXml>.Enumerator enumerator;
					KeyValuePair<int, EmblemStarXml> keyValuePair2 = enumerator.Current;
					num = keyValuePair2.Value.EmblemStar;
				}
			}
			return num;
		}
		return 10;
	}

	public int GetMinStarLevel(int upLevel)
	{
		if (this.m_DicEmblemData.ContainsKey(upLevel) && this.m_DicEmblemData[upLevel].emblemStartDic != null && this.m_DicEmblemData[upLevel].emblemStartDic.Count > 0)
		{
			int num = 0;
			foreach (KeyValuePair<int, EmblemStarXml> keyValuePair in this.m_DicEmblemData[upLevel].emblemStartDic)
			{
				if (keyValuePair.Value.EmblemStar < num)
				{
					Dictionary<int, EmblemStarXml>.Enumerator enumerator;
					KeyValuePair<int, EmblemStarXml> keyValuePair2 = enumerator.Current;
					num = keyValuePair2.Value.EmblemStar;
				}
			}
			return num;
		}
		return 0;
	}

	public int MinUpLevel
	{
		get
		{
			if (this.m_MinUpLevel <= 0)
			{
				int num = 10;
				foreach (KeyValuePair<int, EmblemData> keyValuePair in this.m_DicEmblemData)
				{
					if (keyValuePair.Value.emblemUp.EmblemLevel < num)
					{
						Dictionary<int, EmblemData>.Enumerator enumerator;
						KeyValuePair<int, EmblemData> keyValuePair2 = enumerator.Current;
						num = keyValuePair2.Value.emblemUp.EmblemLevel;
					}
				}
				this.m_MinUpLevel = num;
			}
			return this.m_MinUpLevel;
		}
	}

	public int MaxUpLevel
	{
		get
		{
			if (this.m_MaxUpLevel == -1)
			{
				foreach (KeyValuePair<int, EmblemData> keyValuePair in this.m_DicEmblemData)
				{
					if (keyValuePair.Value.emblemUp.EmblemLevel > this.m_MaxUpLevel)
					{
						Dictionary<int, EmblemData>.Enumerator enumerator;
						KeyValuePair<int, EmblemData> keyValuePair2 = enumerator.Current;
						this.m_MaxUpLevel = keyValuePair2.Value.emblemUp.EmblemLevel;
					}
				}
			}
			return this.m_MaxUpLevel;
		}
	}

	public EmblemStarXml HuiJiStar(int ID)
	{
		if (this.m_DicStar.ContainsKey(ID))
		{
			return this.m_DicStar[ID];
		}
		return null;
	}

	private string EMBLEMSTARTPATH = "Config/EmblemStar.xml";

	private string EMBLEMUPPATH = "Config/EmblemUp.xml";

	private Dictionary<int, EmblemData> m_DicEmblemData = new Dictionary<int, EmblemData>();

	private Dictionary<int, EmblemStarXml> m_DicStar = new Dictionary<int, EmblemStarXml>();

	private int m_MinUpLevel = -1;

	private int m_MaxUpLevel = -1;

	private DPSelectedItemEventHandler LoadCompleteCallBack;

	public bool m_ModelAddBool;
}
