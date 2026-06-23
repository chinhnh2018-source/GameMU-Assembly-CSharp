using System;

namespace Tmsk.Xml
{
	internal class XParserContext
	{
		public string GetString(char[] key, int start, int len)
		{
			if (len > 36)
			{
				return null;
			}
			return this._nameTable.Get(key, start, len);
		}

		public string AddString(char[] key, int start, int len)
		{
			if (len > 36)
			{
				return null;
			}
			return this._nameTable.Add(key, start, len);
		}

		public string AddString(string key)
		{
			if (key.Length > 36)
			{
				return key;
			}
			return this._nameTable.Add(key);
		}

		private XNameTable _nameTable = new XNameTable();
	}
}
