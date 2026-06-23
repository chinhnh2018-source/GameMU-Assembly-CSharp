using System;
using System.Xml.Serialization;

public class AssetState
{
	[XmlAttribute]
	public string url;

	[XmlAttribute]
	public AssetAction action;

	[XmlAttribute]
	public string assetName;

	[XmlAttribute]
	public float timeStamp;
}
