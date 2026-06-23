using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class MonsterBuffer
	{
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempBasePropsDict)
			{
				this._TempBasePropsDict[index] = new TempPropItem
				{
					PropValue = value,
					ToTicks = toTicks
				};
			}
		}

		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempExtPropsDict)
			{
				this._TempExtPropsDict[index] = new TempPropItem
				{
					PropValue = value,
					ToTicks = toTicks
				};
			}
		}

		public void ClearTempBaseProp()
		{
			lock (this._TempBasePropsDict)
			{
				this._TempBasePropsDict.Clear();
			}
		}

		public void ClearTempExtProp()
		{
			lock (this._TempExtPropsDict)
			{
				this._TempExtPropsDict.Clear();
			}
		}

		public void Init()
		{
			this.ClearTempBaseProp();
			this.ClearTempExtProp();
		}

		public double GetBaseProp(int index)
		{
			long num = TimeUtil.NOW() * 10000L;
			double result = 0.0;
			TempPropItem tempPropItem = null;
			lock (this._TempBasePropsDict)
			{
				if (!this._TempBasePropsDict.TryGetValue(index, out tempPropItem))
				{
					return result;
				}
				if (num - tempPropItem.ToTicks < 0L)
				{
					result = tempPropItem.PropValue;
				}
				else
				{
					this._TempBasePropsDict.Remove(index);
				}
			}
			return result;
		}

		public double GetExtProp(int index)
		{
			long num = TimeUtil.NOW() * 10000L;
			double result = 0.0;
			TempPropItem tempPropItem = null;
			lock (this._TempExtPropsDict)
			{
				if (!this._TempExtPropsDict.TryGetValue(index, out tempPropItem))
				{
					return result;
				}
				if (num - tempPropItem.ToTicks < 0L)
				{
					result = tempPropItem.PropValue;
				}
				else
				{
					this._TempExtPropsDict.Remove(index);
				}
			}
			return result;
		}

		private Dictionary<int, TempPropItem> _TempBasePropsDict = new Dictionary<int, TempPropItem>();

		private Dictionary<int, TempPropItem> _TempExtPropsDict = new Dictionary<int, TempPropItem>();
	}
}
