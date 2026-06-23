using System;

namespace Tmsk.Xml
{
	public class XmlDeclaration : XmlNode
	{
		public XmlDeclaration(string ver, string enc, string standa_alone)
		{
			this.version = ver;
			this.encoding = enc;
			this.standalone = standa_alone;
		}

		public override string ToString()
		{
			return string.Format("<?xml version=\"{0}\" encoding=\"{1}\" {2} ?>\r\n", this.version, this.encoding, this.standalone);
		}

		private string version = "1.0";

		private string encoding = "utf-8";

		private string standalone = string.Empty;
	}
}
