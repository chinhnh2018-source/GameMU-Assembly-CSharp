using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class PropsCacheModule
	{
		public double GetExtPropsValue(int propIndex, Func<double> factoryFunc)
		{
			long currentTicksInexact = TimeUtil.CurrentTicksInexact;
			PropsValueFactory propsValueFactory;
			long age;
			lock (this.mutex)
			{
				if (!this.extPropValueDict.TryGetValue(propIndex, out propsValueFactory))
				{
					propsValueFactory = new PropsValueFactory
					{
						propIndex = propIndex,
						nextCalcTicks = currentTicksInexact,
						factoryFunc = factoryFunc
					};
					this.extPropValueDict[propIndex] = propsValueFactory;
				}
				if (propsValueFactory.nextCalcTicks > currentTicksInexact)
				{
					return propsValueFactory.propValue;
				}
				propsValueFactory.Age += 1L;
				age = propsValueFactory.Age;
			}
			double num = propsValueFactory.factoryFunc();
			double result;
			lock (this.mutex)
			{
				if (propsValueFactory.Age <= age)
				{
					propsValueFactory.nextCalcTicks = currentTicksInexact + GameManager.FlagRecalcRolePropsTicks;
					propsValueFactory.propValue = num;
				}
				result = num;
			}
			return result;
		}

		public double GetBasePropsValue(int propIndex, Func<double> factoryFunc, bool cache = true)
		{
			if (cache)
			{
				long currentTicksInexact = TimeUtil.CurrentTicksInexact;
				PropsValueFactory propsValueFactory;
				long age;
				lock (this.mutex)
				{
					if (!this.basePropsValueDict.TryGetValue(propIndex, out propsValueFactory))
					{
						propsValueFactory = new PropsValueFactory
						{
							propIndex = propIndex,
							nextCalcTicks = currentTicksInexact,
							factoryFunc = factoryFunc
						};
						this.basePropsValueDict[propIndex] = propsValueFactory;
					}
					if (propsValueFactory.nextCalcTicks > currentTicksInexact)
					{
						return propsValueFactory.propValue;
					}
					propsValueFactory.Age += 1L;
					age = propsValueFactory.Age;
				}
				double num = propsValueFactory.factoryFunc();
				lock (this.mutex)
				{
					if (propsValueFactory.Age == age)
					{
						propsValueFactory.nextCalcTicks = currentTicksInexact + GameManager.FlagRecalcRolePropsTicks;
						propsValueFactory.propValue = num;
					}
					return num;
				}
			}
			return factoryFunc();
		}

		public void ResetAllProps()
		{
			lock (this.mutex)
			{
				foreach (PropsValueFactory propsValueFactory in this.basePropsValueDict.Values)
				{
					propsValueFactory.nextCalcTicks = 0L;
				}
				foreach (PropsValueFactory propsValueFactory in this.extPropValueDict.Values)
				{
					propsValueFactory.nextCalcTicks = 0L;
				}
			}
		}

		private object mutex = new object();

		private Dictionary<int, PropsValueFactory> extPropValueDict = new Dictionary<int, PropsValueFactory>();

		private Dictionary<int, PropsValueFactory> basePropsValueDict = new Dictionary<int, PropsValueFactory>();
	}
}
