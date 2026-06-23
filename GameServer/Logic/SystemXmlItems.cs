using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	public class SystemXmlItems
	{
		public Dictionary<int, SystemXmlItem> SystemXmlItemDict
		{
			get
			{
				return this._SystemXmlItemDict;
			}
		}

		public int MaxKey { get; private set; }

		private Dictionary<int, SystemXmlItem> _LoadFromXMlFile(string fileName, string rootName, string keyName, int resType)
		{
			XElement xelement = null;
			try
			{
				string text = "";
				if (0 == resType)
				{
					text = Global.GameResPath(fileName);
				}
				else if (1 == resType)
				{
					text = Global.IsolateResPath(fileName);
				}
				text = WorldLevelManager.getInstance().GetJieRiConfigFileName(text);
				xelement = XElement.Load(text);
				if (null == xelement)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				this.FileName = fileName;
				this.RootName = rootName;
				this.KeyName = keyName;
				this.ResType = resType;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。\r\n{1}", fileName, ex.ToString()));
			}
			IEnumerable<XElement> enumerable;
			if ("" == rootName)
			{
				enumerable = xelement.Elements();
			}
			else
			{
				enumerable = xelement.Elements(rootName).Elements<XElement>();
			}
			Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
			foreach (XElement xelement2 in enumerable)
			{
				SystemXmlItem value = new SystemXmlItem
				{
					XMLNode = xelement2
				};
				int num = (int)Global.GetSafeAttributeLong(xelement2, keyName);
				dictionary[num] = value;
				if (num > this.MaxKey)
				{
					this.MaxKey = num;
				}
			}
			this.FirstLoadOK = true;
			return dictionary;
		}

		public void LoadFromXMlFile(string fileName, string rootName, string keyName, int resType = 0)
		{
			this._SystemXmlItemDict = this._LoadFromXMlFile(fileName, rootName, keyName, resType);
		}

		public int ReloadLoadFromXMlFile()
		{
			int result;
			if (!this.FirstLoadOK)
			{
				result = -2;
			}
			else
			{
				try
				{
					this._SystemXmlItemDict = this._LoadFromXMlFile(this.FileName, this.RootName, this.KeyName, this.ResType);
				}
				catch (Exception)
				{
					return -1;
				}
				result = 0;
			}
			return result;
		}

		private Dictionary<int, SystemXmlItem> _SystemXmlItemDict = null;

		private bool FirstLoadOK = false;

		private string FileName = "";

		private string RootName = "";

		private string KeyName = "";

		private int ResType = 0;
	}
}
