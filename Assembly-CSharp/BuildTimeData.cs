using System;

internal class BuildTimeData
{
	public BuildTimeData()
	{
		this.mHaveTime = false;
		this.mTimeArray = new int[6];
	}

	public bool mHaveTime;

	public int[] mTimeArray;
}
