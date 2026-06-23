using System;
using System.Collections.Generic;

public class TrdGoodVOPairs
{
	public TrdGoodVOPairs(Dictionary<string, int> voPropertyIndexDict)
	{
		this._voPropertyIndexDict = voPropertyIndexDict;
		for (int i = 0; i < this.PairValueLst.Length; i++)
		{
			this.PairValueLst[i].sValue = string.Empty;
		}
		for (int j = 0; j < this.SpeedUpAutoIndexLst.Length; j++)
		{
			this.SpeedUpAutoIndexLst[j] = -1;
		}
		this.nSpeedUpAutoIndex = 0;
	}

	public void StartGoodVOCopySpeedUp()
	{
		this.nSpeedUpAutoIndex = -1;
	}

	private int AutoIndexValue
	{
		get
		{
			return this.SpeedUpAutoIndexLst[this.nSpeedUpAutoIndex];
		}
		set
		{
			this.SpeedUpAutoIndexLst[this.nSpeedUpAutoIndex] = value;
		}
	}

	public int PropertyAtOfInt(string strName)
	{
		this.nSpeedUpAutoIndex++;
		int nValue;
		if (this.AutoIndexValue == -1)
		{
			TrdGoodVOPairs.pairValue[] pairValueLst = this.PairValueLst;
			int num = this._voPropertyIndexDict[strName];
			this.AutoIndexValue = num;
			nValue = pairValueLst[num].nValue;
		}
		else
		{
			nValue = this.PairValueLst[this.AutoIndexValue].nValue;
		}
		return nValue;
	}

	public string PropertyAtOfStr(string strName)
	{
		this.nSpeedUpAutoIndex++;
		string sValue;
		if (this.AutoIndexValue == -1)
		{
			TrdGoodVOPairs.pairValue[] pairValueLst = this.PairValueLst;
			int num = this._voPropertyIndexDict[strName];
			this.AutoIndexValue = num;
			sValue = pairValueLst[num].sValue;
		}
		else
		{
			sValue = this.PairValueLst[this.AutoIndexValue].sValue;
		}
		return sValue;
	}

	public static bool IsCopySpeedUp()
	{
		return true;
	}

	private const int MAX_PAIR = 100;

	public TrdGoodVOPairs.pairValue[] PairValueLst = new TrdGoodVOPairs.pairValue[100];

	private Dictionary<string, int> _voPropertyIndexDict;

	private int nSpeedUpAutoIndex;

	private int[] SpeedUpAutoIndexLst = new int[100];

	public struct pairValue
	{
		public int nValue;

		public string sValue;
	}
}
