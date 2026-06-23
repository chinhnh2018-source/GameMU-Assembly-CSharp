using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class BufferPropsModule
	{
		public void Init(PropsCacheManager _propCacheManager)
		{
			this.propCacheManager = _propCacheManager;
		}

		private void UpdateTimedProps(TimedPropsData data, bool enable)
		{
			double num = 0.0;
			if (enable)
			{
				num = data.propsValue;
			}
			if (data.propsType == 1)
			{
				this.propCacheManager.SetExtPropsSingle(new object[]
				{
					PropsSystemTypes.BufferPropsManager,
					data.skillId,
					data.propsType,
					data.propsIndex,
					num
				});
			}
			else if (data.propsType == 0)
			{
				this.propCacheManager.SetBasePropsSingle(new object[]
				{
					PropsSystemTypes.BufferPropsManager,
					data.skillId,
					data.propsType,
					data.propsIndex,
					num
				});
			}
		}

		public void UpdateTimedPropsData(long nowTicks, long startTicks, int bufferTicks, int propsType, int propsIndex, double propsValue, int skillId, int tag)
		{
			long key = ((long)skillId << 32) + (long)((long)propsType << 24) + (long)propsIndex;
			lock (this.mutex)
			{
				TimedPropsData timedPropsData;
				if (!this.bufferDataDict.TryGetValue(key, out timedPropsData))
				{
					timedPropsData = new TimedPropsData(startTicks, bufferTicks, propsType, propsIndex, propsValue, tag, skillId);
					this.bufferDataDict[key] = timedPropsData;
				}
				else
				{
					timedPropsData.startTicks = startTicks;
					timedPropsData.bufferTicks = bufferTicks;
					timedPropsData.propsType = propsType;
					timedPropsData.propsIndex = propsIndex;
					timedPropsData.propsValue = propsValue;
					timedPropsData.tag = tag;
					timedPropsData.skillId = skillId;
					timedPropsData.endTicks = startTicks + (long)bufferTicks;
				}
				this.UpdateTimedProps(timedPropsData, true);
				this.TimerUpdateProps(nowTicks, true);
			}
		}

		public bool TimerUpdateProps(long nowTicks, bool force = false)
		{
			bool flag = false;
			if (null != this.propCacheManager)
			{
				lock (this.mutex)
				{
					if (!force && nowTicks < this.MinExpireTicks)
					{
						return false;
					}
					this.MinExpireTicks = nowTicks + 10000L;
					List<long> list = new List<long>();
					foreach (KeyValuePair<long, TimedPropsData> keyValuePair in this.bufferDataDict)
					{
						long endTicks = keyValuePair.Value.endTicks;
						if (endTicks < nowTicks)
						{
							list.Add(keyValuePair.Key);
							this.UpdateTimedProps(keyValuePair.Value, false);
							if (!flag)
							{
								flag = RoleAlgorithm.NeedNotifyClient((ExtPropIndexes)keyValuePair.Value.propsIndex);
							}
						}
						else if (endTicks < this.MinExpireTicks)
						{
							this.MinExpireTicks = endTicks;
						}
					}
					foreach (long key in list)
					{
						this.bufferDataDict.Remove(key);
					}
				}
			}
			return flag;
		}

		private const long MinCheckIntervalTicks = 10000L;

		private object mutex = new object();

		private long MinExpireTicks = 0L;

		public Dictionary<long, TimedPropsData> bufferDataDict = new Dictionary<long, TimedPropsData>();

		public PropsCacheManager propCacheManager = null;
	}
}
