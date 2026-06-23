using System;

namespace GameServer.Logic
{
	public class SpriteOnceBuffer
	{
		public void AddTempBaseProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.BaseProps[index] = value;
				this._TempProp.BasePropsTick[index] = toTicks;
			}
		}

		public void AddTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		public double GetBaseProp(int index)
		{
			double result = 0.0;
			lock (this._TempProp)
			{
				result = this._TempProp.BaseProps[index];
				this._TempProp.BaseProps[index] = 0.0;
			}
			return result;
		}

		public double GetExtProp(int index)
		{
			double result = 0.0;
			lock (this._TempProp)
			{
				result = this._TempProp.ExtProps[index];
				this._TempProp.ExtProps[index] = 0.0;
			}
			return result;
		}

		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
