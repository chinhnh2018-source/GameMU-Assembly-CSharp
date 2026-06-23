using System;

public class GoodsObtainVO
{
	public int[] BaoDianIDArray
	{
		get
		{
			if (this.mBaoDianIDArray == null && !string.IsNullOrEmpty(this.BaoDianID))
			{
				string[] array = this.BaoDianID.Split(new char[]
				{
					','
				});
				this.mBaoDianIDArray = new int[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					this.mBaoDianIDArray[i] = array[i].SafeToInt32(0);
				}
			}
			return this.mBaoDianIDArray;
		}
	}

	public int GoodsID;

	public string BaoDianID;

	public int CoinID;

	private int[] mBaoDianIDArray;

	public int ObtainSwitch;

	public string Name;
}
