using System;

namespace Assets.Plugins.Scripts.Server.Data
{
	public class EquipPropItem
	{
		public EquipPropItem()
		{
			this.ResetProps();
		}

		public double[] BaseProps
		{
			get
			{
				return this._BaseProps;
			}
		}

		public double[] ExtProps
		{
			get
			{
				return this._ExtProps;
			}
		}

		public void ResetProps()
		{
			for (int i = 0; i < 5; i++)
			{
				this._BaseProps[i] = 0.0;
			}
			for (int j = 0; j < 69; j++)
			{
				this._ExtProps[j] = 0.0;
			}
		}

		private double[] _BaseProps = new double[5];

		private double[] _ExtProps = new double[69];
	}
}
