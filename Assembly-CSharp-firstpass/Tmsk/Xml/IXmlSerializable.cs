using System;
using System.IO;

namespace Tmsk.Xml
{
	public interface IXmlSerializable
	{
		void WriteXml(StringWriter sw);

		void ReadXml(StringReader sr);
	}
}
