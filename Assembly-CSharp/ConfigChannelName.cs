using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigChannelName : IConfigbase<ConfigChannelName>, ConfigBase
{
	public ConfigChannelName()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
		this.mChannelNameList.Clear();
	}

	private void InitChannelName()
	{
		if (0 >= this.mChannelNameList.size)
		{
			XElement gameResXml = Global.GetGameResXml("GameRes/Config/ChannelName.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "ChannelName");
				for (int i = 0; i < xelementList.Count; i++)
				{
					ChannelNameVO channelNameVO = new ChannelNameVO();
					channelNameVO.CopyForm(xelementList[i]);
					this.mChannelNameList.Add(channelNameVO);
				}
			}
		}
	}

	public string GetPTNameByPTID(int PTID, bool AddPoint = true)
	{
		if (Global.isHaiWai)
		{
			return string.Empty;
		}
		this.InitChannelName();
		for (int i = 0; i < this.mChannelNameList.size; i++)
		{
			if (this.mChannelNameList[i] != null && PTID == this.mChannelNameList[i].PTID)
			{
				return this.mChannelNameList[i].PTName + ((!AddPoint) ? string.Empty : "•");
			}
		}
		return string.Empty;
	}

	private const string path = "GameRes/Config/ChannelName.xml";

	private BetterList<ChannelNameVO> mChannelNameList = new BetterList<ChannelNameVO>();
}
