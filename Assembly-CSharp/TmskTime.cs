using System;

internal class TmskTime
{
	private TmskTime()
	{
	}

	public static void Init(bool strictChk)
	{
		TmskTime._tmskTime.InnerInit(strictChk);
	}

	public static void Update(float deltaTime)
	{
		TmskTime._tmskTime.InnerUpdate(deltaTime);
	}

	public static long DeltaMills()
	{
		return TmskTime._tmskTime.InnerDeltaMills();
	}

	public static long CurMills()
	{
		return TmskTime._tmskTime.InnerCurMills();
	}

	public static long FrameCount()
	{
		return TmskTime._tmskTime.InnerFrameCount();
	}

	public void InnerInit(bool strictChk)
	{
		this._strictChk = strictChk;
		this._curTimeMills = DateTime.Now.Ticks / 10000L;
	}

	public void InnerUpdate(float deltaTime)
	{
		this._updateTick += 1L;
		long num = (long)(deltaTime * 1000f);
		if (this._strictChk)
		{
			this._deltaMills = ((num <= 22L) ? num : 22L);
		}
		else
		{
			this._deltaMills = num;
		}
		this._curTimeMills += this._deltaMills;
	}

	public long InnerDeltaMills()
	{
		return this._deltaMills;
	}

	public long InnerCurMills()
	{
		return this._curTimeMills;
	}

	public long InnerFrameCount()
	{
		return this._updateTick;
	}

	private static TmskTime _tmskTime = new TmskTime();

	private bool _strictChk;

	private long _updateTick;

	private long _curTimeMills;

	private long _deltaMills;
}
