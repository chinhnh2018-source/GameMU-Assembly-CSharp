using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class PropsCacheItem
	{
		public PropsCacheItem(PropsCacheItem parent, int type)
		{
			this.ParentPropsCacheItem = parent;
			if (parent != null)
			{
				this.Path.AddRange(parent.Path);
				this.Path.Add(type);
			}
		}

		public string GetName()
		{
			string text = "";
			for (int i = 0; i < this.Path.Count; i++)
			{
				if (i == 0)
				{
					text = text + "\\" + (PropsSystemTypes)this.Path[i];
				}
				else
				{
					text = text + "\\" + this.Path[i];
				}
			}
			return text;
		}

		public void SetBaseProp(int index, double value)
		{
			PropsCacheItem parentPropsCacheItem = this.ParentPropsCacheItem;
			lock (this)
			{
				double num = value - this.BaseProps[index];
				if (num != 0.0)
				{
					bool flag2 = this.nowkg;
					this.BaseProps[index] = value;
					while (flag2)
					{
						parentPropsCacheItem.BaseProps[index] += num;
						if (null == parentPropsCacheItem.ParentPropsCacheItem)
						{
							parentPropsCacheItem.Age++;
							this.UpdateBasePropCache(index, parentPropsCacheItem);
							break;
						}
						flag2 = parentPropsCacheItem.nowkg;
						parentPropsCacheItem = parentPropsCacheItem.ParentPropsCacheItem;
					}
				}
			}
		}

		public void SetExtProp(int index, double value)
		{
			PropsCacheItem parentPropsCacheItem = this.ParentPropsCacheItem;
			lock (this)
			{
				double num = value - this.ExtProps[index];
				if (num != 0.0)
				{
					bool flag2 = this.nowkg;
					this.ExtProps[index] = value;
					while (flag2 && null != parentPropsCacheItem)
					{
						parentPropsCacheItem.ExtProps[index] += num;
						if (null == parentPropsCacheItem.ParentPropsCacheItem)
						{
							parentPropsCacheItem.Age++;
							this.UpdateExtPropCache(index, parentPropsCacheItem);
							break;
						}
						flag2 = parentPropsCacheItem.nowkg;
						parentPropsCacheItem = parentPropsCacheItem.ParentPropsCacheItem;
					}
				}
			}
		}

		public void SetNodeBool(bool kg)
		{
			PropsCacheItem parentPropsCacheItem = this.ParentPropsCacheItem;
			lock (this)
			{
				if (this.nowkg != kg)
				{
					if (this.nowkg)
					{
						for (int i = 0; i < 177; i++)
						{
							if (this.ExtProps[i] != 0.0)
							{
								parentPropsCacheItem.SetExtProp(i, parentPropsCacheItem.ExtProps[i] - this.ExtProps[i]);
							}
						}
						for (int i = 0; i < 4; i++)
						{
							if (this.BaseProps[i] != 0.0)
							{
								parentPropsCacheItem.SetBaseProp(i, parentPropsCacheItem.BaseProps[i] - this.BaseProps[i]);
							}
						}
					}
					else
					{
						for (int i = 0; i < 177; i++)
						{
							if (this.ExtProps[i] != 0.0)
							{
								parentPropsCacheItem.SetExtProp(i, parentPropsCacheItem.ExtProps[i] + this.ExtProps[i]);
							}
						}
						for (int i = 0; i < 4; i++)
						{
							if (this.BaseProps[i] != 0.0)
							{
								parentPropsCacheItem.SetBaseProp(i, parentPropsCacheItem.BaseProps[i] + this.BaseProps[i]);
							}
						}
					}
				}
				this.nowkg = kg;
			}
		}

		private void UpdateExtPropCache(int Prop, PropsCacheItem item)
		{
			item.ExtPropsCache[Prop] = RoleAlgorithm.GetExtProp(item.Client, Prop);
			if (null != RoleAlgorithm.ExtListArray[Prop])
			{
				int count = RoleAlgorithm.ExtListArray[Prop].Count;
				for (int i = 0; i < count; i++)
				{
					double extProp = RoleAlgorithm.GetExtProp(item.Client, (int)RoleAlgorithm.ExtListArray[Prop][i]);
					item.ExtPropsCache[(int)RoleAlgorithm.ExtListArray[Prop][i]] = extProp;
				}
			}
		}

		private void UpdateBasePropCache(int BaseProp, PropsCacheItem item)
		{
			if (null != RoleAlgorithm.BaseListArray[BaseProp])
			{
				int count = RoleAlgorithm.BaseListArray[BaseProp].Count;
				for (int i = 0; i < count; i++)
				{
					double extProp = RoleAlgorithm.GetExtProp(item.Client, (int)RoleAlgorithm.BaseListArray[BaseProp][i]);
					item.ExtPropsCache[(int)RoleAlgorithm.BaseListArray[BaseProp][i]] = extProp;
				}
			}
		}

		public void ResetProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this.BaseProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this.ExtProps[i] = 0.0;
			}
			Array.Clear(this.ExtPropsCache, 0, this.ExtPropsCache.Length);
		}

		public bool nowkg = true;

		public GameClient Client;

		public int Age = 0;

		public int LastAge = 0;

		public DelOnPropsChanged OnPropsChanged;

		public double[] BaseProps = new double[4];

		public double[] ExtProps = new double[177];

		public double[] ExtPropsCache = new double[177];

		public PropsCacheItem ParentPropsCacheItem;

		public Dictionary<int, PropsCacheItem> SubPropsItemDict = new Dictionary<int, PropsCacheItem>();

		public List<int> Path = new List<int>();
	}
}
