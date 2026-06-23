using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class WolfSoulField_World : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/Plate/wordmap.jpg";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		GameInstance.Game.ApplyPlayerWorldData();
		this.SetCityLevelAndID();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
	}

	private void SetCityLevelAndID()
	{
		for (int i = 0; i < this.CityName.Length; i++)
		{
			this.CityName[i].Level = this.level[i];
			this.CityName[i].ID = i + 1;
			this.CityName[i].CityName = Global.GetLang("无人占领");
		}
	}

	public void SetCityName(LangHunLingYuWorldData langhunlingyuworldData)
	{
		if (langhunlingyuworldData == null)
		{
			return;
		}
		if (langhunlingyuworldData.CityList == null)
		{
			return;
		}
		for (int i = 0; i < langhunlingyuworldData.CityList.Count; i++)
		{
			for (int j = 0; j < this.CityName.Length; j++)
			{
				if (langhunlingyuworldData.CityList[i].CityId == this.CityName[j].ID)
				{
					if (langhunlingyuworldData.CityList[i].Owner != null)
					{
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(langhunlingyuworldData.CityList[i].Owner.ZoneID, out ztBuffServerInfo))
						{
							this.CityName[j].CityName = ztBuffServerInfo.strServerName + "\r\n" + langhunlingyuworldData.CityList[i].Owner.BHName;
						}
						else
						{
							this.CityName[j].CityName = string.Concat(new object[]
							{
								Global.GetLang("区"),
								langhunlingyuworldData.CityList[i].Owner.ZoneID,
								"\r\n",
								langhunlingyuworldData.CityList[i].Owner.BHName
							});
						}
					}
					this.CityName[j].LangHunlingyuCityData = langhunlingyuworldData.CityList[i];
				}
			}
		}
	}

	public GButton Close;

	public ShowNetImage Bak;

	public MyWorldCity[] CityName;

	public DPSelectedItemEventHandler DPSelectItem;

	private int[] level = new int[]
	{
		10,
		9,
		9,
		8,
		8,
		8,
		8,
		7,
		7,
		7,
		7,
		7,
		7,
		7,
		7
	};
}
