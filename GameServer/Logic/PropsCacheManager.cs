using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Server.Tools;

namespace GameServer.Logic
{
	public class PropsCacheManager
	{
		public PropsCacheManager(GameClient client)
		{
			this.PropsCacheRoot.Client = client;
		}

		public int GetAge()
		{
			int age;
			lock (this.PropsCacheRoot)
			{
				age = this.PropsCacheRoot.Age;
			}
			return age;
		}

		public bool IsChanged()
		{
			lock (this.PropsCacheRoot)
			{
				if (this.PropsCacheRoot.LastAge != this.PropsCacheRoot.Age)
				{
					this.PropsCacheRoot.LastAge = this.PropsCacheRoot.Age;
					return true;
				}
			}
			return false;
		}

		public double[] getCopyBaseProp()
		{
			double[] baseProps = this.PropsCacheRoot.BaseProps;
			double[] array = new double[baseProps.Length];
			for (int i = 0; i < baseProps.Length; i++)
			{
				array[i] = baseProps[i];
			}
			return array;
		}

		public double[] getCopyExtProp()
		{
			double[] extProps = this.PropsCacheRoot.ExtProps;
			double[] array = new double[extProps.Length];
			for (int i = 0; i < extProps.Length; i++)
			{
				array[i] = extProps[i];
			}
			return array;
		}

		public void SetBaseProps(params object[] args)
		{
			PropsCacheItem propsCacheItem = this.PropsCacheRoot;
			PropsCacheItem propsCacheItem2 = null;
			double[] array = null;
			object obj = null;
			if (args.Length > 1)
			{
				obj = args[args.Length - 1];
				EquipPropItem equipPropItem = args[args.Length - 1] as EquipPropItem;
				if (null != equipPropItem)
				{
					array = equipPropItem.BaseProps;
				}
				else
				{
					array = (args[args.Length - 1] as double[]);
				}
			}
			if (null != array)
			{
				lock (this.PropsCacheRoot)
				{
					foreach (object obj2 in args)
					{
						if (obj2 == obj)
						{
							if (propsCacheItem2 != null)
							{
								Contract.Assert(propsCacheItem2.SubPropsItemDict.Count == 0, "only leaf node can set props!");
								int num = 0;
								while (num < 4 && num < array.Length)
								{
									propsCacheItem2.SetBaseProp(num, array[num]);
									num++;
								}
							}
							break;
						}
						if (!propsCacheItem.SubPropsItemDict.TryGetValue((int)obj2, out propsCacheItem2))
						{
							propsCacheItem2 = new PropsCacheItem(propsCacheItem, Convert.ToInt32(obj2));
							propsCacheItem.SubPropsItemDict.Add((int)obj2, propsCacheItem2);
						}
						propsCacheItem = propsCacheItem2;
					}
				}
			}
		}

		public void SetBasePropsSingle(params object[] args)
		{
			PropsCacheItem propsCacheItem = this.PropsCacheRoot;
			PropsCacheItem propsCacheItem2 = null;
			double value = 0.0;
			int num = -1;
			try
			{
				if (args.Length <= 2)
				{
					return;
				}
				num = (int)args[args.Length - 2];
				value = Convert.ToDouble(args[args.Length - 1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			if (num >= 0 && num < 4)
			{
				lock (this.PropsCacheRoot)
				{
					for (int i = 0; i < args.Length - 2; i++)
					{
						if (!propsCacheItem.SubPropsItemDict.TryGetValue((int)args[i], out propsCacheItem2))
						{
							propsCacheItem2 = new PropsCacheItem(propsCacheItem, Convert.ToInt32(args[i]));
							propsCacheItem.SubPropsItemDict.Add((int)args[i], propsCacheItem2);
						}
						propsCacheItem = propsCacheItem2;
					}
					if (propsCacheItem2 != null)
					{
						Contract.Assert(propsCacheItem2.SubPropsItemDict.Count == 0, "only leaf node can set props!");
						propsCacheItem2.SetBaseProp(num, value);
					}
				}
			}
		}

		public void SetExtProps(params object[] args)
		{
			PropsCacheItem propsCacheItem = this.PropsCacheRoot;
			PropsCacheItem propsCacheItem2 = null;
			double[] array = null;
			object obj = null;
			try
			{
				if (args.Length > 1)
				{
					obj = args[args.Length - 1];
					EquipPropItem equipPropItem = obj as EquipPropItem;
					if (null != equipPropItem)
					{
						array = equipPropItem.ExtProps;
					}
					else
					{
						array = (args[args.Length - 1] as double[]);
					}
				}
				lock (this.PropsCacheRoot)
				{
					foreach (object obj2 in args)
					{
						if (obj2 == obj)
						{
							if (propsCacheItem2 != null)
							{
								Contract.Assert(propsCacheItem2.SubPropsItemDict.Count == 0, "only leaf node can set props!");
								if (null != array)
								{
									int j = 0;
									while (j < 177 && j < array.Length)
									{
										propsCacheItem2.SetExtProp(j, array[j]);
										j++;
									}
								}
								else
								{
									for (int j = 0; j < 177; j++)
									{
										propsCacheItem2.SetExtProp(j, 0.0);
									}
								}
							}
							break;
						}
						if (!propsCacheItem.SubPropsItemDict.TryGetValue((int)obj2, out propsCacheItem2))
						{
							propsCacheItem2 = new PropsCacheItem(propsCacheItem, Convert.ToInt32(obj2));
							propsCacheItem.SubPropsItemDict.Add((int)obj2, propsCacheItem2);
						}
						propsCacheItem = propsCacheItem2;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void SetExtPropsSingle(params object[] args)
		{
			PropsCacheItem propsCacheItem = this.PropsCacheRoot;
			PropsCacheItem propsCacheItem2 = null;
			double value = 0.0;
			int num = -1;
			try
			{
				if (args.Length <= 2)
				{
					return;
				}
				num = (int)args[args.Length - 2];
				value = Convert.ToDouble(args[args.Length - 1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			if (num >= 0 && num < 177)
			{
				lock (this.PropsCacheRoot)
				{
					for (int i = 0; i < args.Length - 2; i++)
					{
						if (!propsCacheItem.SubPropsItemDict.TryGetValue((int)args[i], out propsCacheItem2))
						{
							propsCacheItem2 = new PropsCacheItem(propsCacheItem, Convert.ToInt32(args[i]));
							propsCacheItem.SubPropsItemDict.Add((int)args[i], propsCacheItem2);
						}
						propsCacheItem = propsCacheItem2;
					}
					if (propsCacheItem2 != null)
					{
						Contract.Assert(propsCacheItem2.SubPropsItemDict.Count == 0, "only leaf node can set props!");
						propsCacheItem2.SetExtProp(num, value);
					}
				}
			}
		}

		public List<PropsCacheItem> GetAllPropsCacheItems(PropsCacheItem parent = null)
		{
			List<PropsCacheItem> list = new List<PropsCacheItem>();
			if (null == parent)
			{
				parent = this.PropsCacheRoot;
			}
			lock (this.PropsCacheRoot)
			{
				if (parent.SubPropsItemDict.Count > 0)
				{
					foreach (PropsCacheItem parent2 in parent.SubPropsItemDict.Values)
					{
						list.AddRange(this.GetAllPropsCacheItems(parent2));
					}
				}
				else
				{
					list.Add(parent);
				}
			}
			return list;
		}

		public double GetBaseProp(int index)
		{
			double result = 0.0;
			lock (this.PropsCacheRoot)
			{
				result = this.PropsCacheRoot.BaseProps[index];
			}
			return result;
		}

		public double GetExtProp(int index)
		{
			double result = 0.0;
			lock (this.PropsCacheRoot)
			{
				result = this.PropsCacheRoot.ExtProps[index];
			}
			return result;
		}

		public void SetNodeState(params object[] args)
		{
			PropsCacheItem propsCacheItem = this.PropsCacheRoot;
			bool nodeBool;
			try
			{
				if (args.Length <= 1)
				{
					return;
				}
				nodeBool = (bool)args[args.Length - 1];
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			lock (this.PropsCacheRoot)
			{
				PropsCacheItem propsCacheItem2 = null;
				for (int i = 0; i < args.Length - 1; i++)
				{
					if (!propsCacheItem.SubPropsItemDict.TryGetValue((int)args[i], out propsCacheItem2))
					{
						propsCacheItem2 = new PropsCacheItem(propsCacheItem, Convert.ToInt32(args[i]));
						propsCacheItem.SubPropsItemDict.Add((int)args[i], propsCacheItem2);
					}
					propsCacheItem = propsCacheItem2;
				}
				if (propsCacheItem2 != null)
				{
					propsCacheItem2.SetNodeBool(nodeBool);
				}
			}
		}

		public double GetExtPropFinal(int index)
		{
			double result;
			lock (this.PropsCacheRoot)
			{
				result = this.PropsCacheRoot.ExtPropsCache[index];
			}
			return result;
		}

		private PropsCacheItem PropsCacheRoot = new PropsCacheItem(null, 0);

		public static readonly double[] ConstBaseProps = new double[4];

		public static readonly double[] ConstExtProps = new double[177];
	}
}
