using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class CompMineTruckVO
{
	public void CopyForm(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.MonstersID = Global.GetXElementAttributeInt(xml, "MonstersID");
		this.TimePoints = Global.GetXElementAttributeStr(xml, "TimePoints");
		this.Speed = Global.GetXElementAttributeFloat(xml, "Speed");
		this.MaxHurt = Global.GetXElementAttributeInt(xml, "MaxHurt");
		this.AddLIfe = Global.GetXElementAttributeStr(xml, "AddLIfe");
		this.FinishNum = Global.GetXElementAttributeStr(xml, "FinishNum");
		this.BrokenNum = Global.GetXElementAttributeStr(xml, "BrokenNum");
		this.CompNum = Global.GetXElementAttributeInt(xml, "CompNum");
		this.CompMineNum = Global.GetXElementAttributeInt(xml, "CompMineNum");
	}

	public List<int> AddLIfeList
	{
		get
		{
			if (0 >= this.mAddLIfeList.Count && !string.IsNullOrEmpty(this.AddLIfe))
			{
				string[] array = this.AddLIfe.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						this.mAddLIfeList.Add(array[i].SafeToInt32(0));
					}
				}
			}
			return this.mAddLIfeList;
		}
	}

	public List<float> FinishNumList
	{
		get
		{
			if (0 >= this.mFinishNumList.Count && !string.IsNullOrEmpty(this.FinishNum))
			{
				string[] array = this.FinishNum.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						float num = 0f;
						float.TryParse(array[i], ref num);
						this.mFinishNumList.Add(num);
					}
				}
			}
			return this.mFinishNumList;
		}
	}

	public List<float> BrokenNumList
	{
		get
		{
			if (0 >= this.mBrokenNumList.Count && !string.IsNullOrEmpty(this.BrokenNum))
			{
				string[] array = this.BrokenNum.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						float num = 0f;
						float.TryParse(array[i], ref num);
						this.mBrokenNumList.Add(num);
					}
				}
			}
			return this.mBrokenNumList;
		}
	}

	public int ID;

	public int MonstersID;

	public string TimePoints;

	public float Speed;

	public int MaxHurt;

	public string AddLIfe;

	public string FinishNum;

	public string BrokenNum;

	public int CompNum;

	public int CompMineNum;

	private List<int> mAddLIfeList = new List<int>();

	private List<float> mFinishNumList = new List<float>();

	private List<float> mBrokenNumList = new List<float>();
}
