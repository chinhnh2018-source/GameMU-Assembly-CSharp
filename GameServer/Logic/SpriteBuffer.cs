using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class SpriteBuffer
	{
		public void ResetForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		public void AddForeverBaseProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.BaseProps[index] = value;
			}
		}

		public void AddForeverExtProp(int index, double value)
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ExtProps[index] = value;
			}
		}

		public double[] getCopyBaseProp()
		{
			double[] baseProps = this._TempProp.BaseProps;
			double[] array = new double[baseProps.Length];
			for (int i = 0; i < baseProps.Length; i++)
			{
				array[i] = baseProps[i];
			}
			return array;
		}

		public double[] getCopyExtProp()
		{
			double[] extProps = this._TempProp.ExtProps;
			double[] array = new double[extProps.Length];
			for (int i = 0; i < extProps.Length; i++)
			{
				array[i] = extProps[i];
			}
			return array;
		}

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
				if (TimeUtil.NOW() * 10000L > this._TempProp.ExtPropsTick[index] || Math.Abs(value) >= Math.Abs(this._TempProp.ExtProps[index]))
				{
					this._TempProp.ExtProps[index] = value;
					this._TempProp.ExtPropsTick[index] = toTicks;
				}
			}
		}

		public void SetTempExtProp(int index, double value, long toTicks)
		{
			lock (this._TempProp)
			{
				this._TempProp.ExtProps[index] = value;
				this._TempProp.ExtPropsTick[index] = toTicks;
			}
		}

		public double GetBaseProp(int index)
		{
			double num = 0.0;
			lock (this._TempProp)
			{
				long num2 = TimeUtil.NOW() * 10000L;
				if (num2 - this._TempProp.BasePropsTick[index] < 0L)
				{
					num = this._TempProp.BaseProps[index];
				}
			}
			double result;
			lock (this._ForeverProp)
			{
				result = this._ForeverProp.BaseProps[index] + num;
			}
			return result;
		}

		public double GetExtProp(int index)
		{
			double num = 0.0;
			lock (this._TempProp)
			{
				long num2 = TimeUtil.NOW() * 10000L;
				if (num2 - this._TempProp.ExtPropsTick[index] < 0L)
				{
					num = this._TempProp.ExtProps[index];
				}
			}
			double result;
			lock (this._ForeverProp)
			{
				result = this._ForeverProp.ExtProps[index] + num;
			}
			return result;
		}

		public void ClearAllTempProps()
		{
			lock (this._TempProp)
			{
				this._TempProp.ResetProps();
			}
		}

		public void ClearAllForeverProps()
		{
			lock (this._ForeverProp)
			{
				this._ForeverProp.ResetProps();
			}
		}

		private BufferPropItem _ForeverProp = new BufferPropItem();

		private BufferPropItem _TempProp = new BufferPropItem();
	}
}
