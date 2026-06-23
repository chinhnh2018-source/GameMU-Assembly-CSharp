using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

[Serializable]
public class DictionarySerializable<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
	public void WriteXml(XmlWriter write)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
		XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
		foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
		{
			write.WriteStartElement("SerializableDictionary");
			write.WriteStartElement("key");
			xmlSerializer.Serialize(write, keyValuePair.Key);
			write.WriteEndElement();
			write.WriteStartElement("value");
			xmlSerializer2.Serialize(write, keyValuePair.Value);
			write.WriteEndElement();
			write.WriteEndElement();
		}
	}

	public void ReadXml(XmlReader reader)
	{
		reader.Read();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(TKey));
		XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(TValue));
		while (reader.NodeType != 15)
		{
			reader.ReadStartElement("SerializableDictionary");
			reader.ReadStartElement("key");
			TKey tkey = (TKey)((object)xmlSerializer.Deserialize(reader));
			reader.ReadEndElement();
			reader.ReadStartElement("value");
			TValue tvalue = (TValue)((object)xmlSerializer2.Deserialize(reader));
			reader.ReadEndElement();
			reader.ReadEndElement();
			this.Add(tkey, tvalue);
			reader.MoveToContent();
		}
		reader.ReadEndElement();
	}

	public XmlSchema GetSchema()
	{
		return null;
	}
}
