using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using ProtoBuf;

[ProtoContract]
public class TarotKingData
{
	public bool KingIsOutTime()
	{
		if (this.AddtionDict == null)
		{
			return false;
		}
		if (0 >= this.AddtionDict.Count)
		{
			return false;
		}
		long num = Global.GetCorrectLocalTime() - this.StartTime;
		if (0L > num)
		{
			TimeSpan timeSpan;
			timeSpan..ctor(num);
			return timeSpan.TotalSeconds < (double)this.BufferSecs;
		}
		return false;
	}

	[ProtoMember(1)]
	public long StartTime;

	[ProtoMember(2)]
	public long BufferSecs;

	[ProtoMember(3)]
	public Dictionary<int, int> AddtionDict;
}
