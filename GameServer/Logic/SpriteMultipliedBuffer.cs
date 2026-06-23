using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class SpriteMultipliedBuffer
	{
		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		public void ClearAllTempExtProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		public double GetExtProp(int index, double baseValue)
		{
			double num = 0.0;
			lock (this._TempProp)
			{
				long num2 = TimeUtil.NOW() * 10000L;
				if (this._TempProp.ExtPropsTick[index] <= 0L || num2 - this._TempProp.ExtPropsTick[index] < 0L)
				{
					num = this._TempProp.ExtProps[index];
				}
			}
			return (1.0 + num) * baseValue;
		}

		public double GetExtProp(int index)
		{
			double result = 0.0;
			lock (this._TempProp)
			{
				long num = TimeUtil.NOW() * 10000L;
				if (this._TempProp.ExtPropsTick[index] <= 0L || num - this._TempProp.ExtPropsTick[index] < 0L)
				{
					result = this._TempProp.ExtProps[index];
				}
			}
			return result;
		}

		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
