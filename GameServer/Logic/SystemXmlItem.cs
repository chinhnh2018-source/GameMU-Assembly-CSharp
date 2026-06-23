using System;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class SystemXmlItem
	{
		public XElement XMLNode { get; set; }

		public string GetStringValue(string name)
		{
			string result = "";
			try
			{
				result = (string)this.XMLNode.Attribute(name);
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, xelementNodePath), null, true);
			}
			return result;
		}

		public int GetIntValue(string name, int defaultValue = -1)
		{
			int result = defaultValue;
			try
			{
				string text = (string)this.XMLNode.Attribute(name);
				if (text != null && text != "")
				{
					result = (int)Convert.ToDouble(text);
				}
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, xelementNodePath), null, true);
			}
			return result;
		}

		public long GetLongValue(string name)
		{
			long result = -1L;
			try
			{
				string text = (string)this.XMLNode.Attribute(name);
				if (text != null && text != "")
				{
					result = Convert.ToInt64(text);
				}
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, xelementNodePath), null, true);
			}
			return result;
		}

		public double GetDoubleValue(string name)
		{
			double result = 0.0;
			try
			{
				string text = (string)this.XMLNode.Attribute(name);
				if (text != null && text != "")
				{
					result = Convert.ToDouble(text);
				}
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1}", name, xelementNodePath), null, true);
			}
			return result;
		}

		public int[] GetIntArrayValue(string name, char split = ',')
		{
			int[] array = null;
			try
			{
				string text = (string)this.XMLNode.Attribute(name);
				if (text != null && text != "")
				{
					string[] array2 = text.Split(new char[]
					{
						split
					});
					if (array2.Length > 0)
					{
						array = new int[array2.Length];
						for (int i = 0; i < array2.Length; i++)
						{
							array[i] = Convert.ToInt32(array2[i]);
						}
					}
				}
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1},采用整形数组返回", name, xelementNodePath), null, true);
			}
			return array;
		}

		public double[] GetDoubleArrayValue(string name, char split = ',')
		{
			double[] array = null;
			try
			{
				string text = (string)this.XMLNode.Attribute(name);
				if (text != null && text != "")
				{
					string[] array2 = text.Split(new char[]
					{
						split
					});
					if (array2.Length > 0)
					{
						array = new double[array2.Length];
						for (int i = 0; i < array2.Length; i++)
						{
							array[i] = Convert.ToDouble(array2[i]);
						}
					}
				}
			}
			catch (Exception)
			{
				string xelementNodePath = Global.GetXElementNodePath(this.XMLNode);
				LogManager.WriteLog(1, string.Format("解析XMLNode 中的属性值: {0}, 失败, XML节点路径: {1},采用整形数组返回", name, xelementNodePath), null, true);
			}
			return array;
		}
	}
}
