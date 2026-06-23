using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class YaoSaiJianYuPartJiLu : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.OBCItems = this.Items.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.SendPrisonHuDongJiLuData();
		Super.ShowNetWaiting(null);
	}

	public void ServerToClientData(List<HuDongState> data)
	{
		for (int i = data.Count - 1; i >= 0; i--)
		{
			YaoSaiJianYuPartJiLuItem yaoSaiJianYuPartJiLuItem = U3DUtils.NEW<YaoSaiJianYuPartJiLuItem>();
			switch (data[i].ID)
			{
			case 1:
			case 2:
			case 9:
			case 10:
			case 13:
			case 14:
			case 18:
			case 19:
			case 22:
			case 23:
			{
				string intro = YaoSaiJianYuPartJiLu.GetDicManorIntro()[data[i].ID].Intro;
				yaoSaiJianYuPartJiLuItem.MiaoShu.text = intro.Replace("{0}", data[i].Name1);
				break;
			}
			case 3:
			case 4:
			case 5:
			{
				string intro2 = YaoSaiJianYuPartJiLu.GetDicManorIntro()[data[i].ID].Intro;
				string awardType = this.GetAwardType(data[i].JiangLiType, data[i].JiangLiCount);
				yaoSaiJianYuPartJiLuItem.MiaoShu.text = intro2.Replace("{0}", awardType);
				break;
			}
			case 6:
			case 7:
			case 8:
			case 11:
			case 12:
				yaoSaiJianYuPartJiLuItem.MiaoShu.text = string.Format("{0}", YaoSaiJianYuPartJiLu.GetDicManorIntro()[data[i].ID].Intro);
				break;
			case 15:
			case 16:
			case 17:
			{
				string awardType2 = this.GetAwardType(data[i].JiangLiType, data[i].JiangLiCount);
				string text = YaoSaiJianYuPartJiLu.GetDicManorIntro()[data[i].ID].Intro;
				text = text.Replace("{0}", data[i].Name1);
				text = text.Replace("{1}", awardType2);
				yaoSaiJianYuPartJiLuItem.MiaoShu.text = text;
				break;
			}
			case 20:
			case 21:
			case 24:
			case 25:
			{
				string text2 = YaoSaiJianYuPartJiLu.GetDicManorIntro()[data[i].ID].Intro;
				text2 = text2.Replace("{0}", data[i].Name1);
				text2 = text2.Replace("{1}", data[i].Name2);
				yaoSaiJianYuPartJiLuItem.MiaoShu.text = text2;
				break;
			}
			}
			UIPanel component = yaoSaiJianYuPartJiLuItem.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.OBCItems.AddNoUpdate(yaoSaiJianYuPartJiLuItem);
		}
	}

	private string GetAwardType(int type, int count)
	{
		string result = string.Empty;
		if (type == 3)
		{
			result = string.Format(Global.GetLang("{0}成就"), count);
		}
		else if (type == 1)
		{
			result = string.Format(Global.GetLang("{0}魔晶"), count);
		}
		else if (type == 4)
		{
			result = string.Format(Global.GetLang("{0}声望"), count);
		}
		else if (type == 2)
		{
			result = string.Format(Global.GetLang("{0}星魂"), count);
		}
		else if (type == 5)
		{
			result = string.Format(Global.GetLang("{0}战功"), count);
		}
		return result;
	}

	public static Dictionary<int, ManorIntro> GetDicManorIntro()
	{
		if (YaoSaiJianYuPartJiLu.dicManorIntro.Count > 0)
		{
			return YaoSaiJianYuPartJiLu.dicManorIntro;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ManorIntro.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ManorIntro");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ManorIntro manorIntro = new ManorIntro();
			manorIntro.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			manorIntro.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
			if (!YaoSaiJianYuPartJiLu.dicManorIntro.ContainsKey(manorIntro.ID))
			{
				YaoSaiJianYuPartJiLu.dicManorIntro.Add(manorIntro.ID, manorIntro);
			}
			i++;
		}
		return YaoSaiJianYuPartJiLu.dicManorIntro;
	}

	public static void ClearXMLData()
	{
		if (0 < YaoSaiJianYuPartJiLu.dicManorIntro.Count)
		{
			YaoSaiJianYuPartJiLu.dicManorIntro.Clear();
		}
	}

	public GButton BtnClose;

	public ListBox Items;

	public ObservableCollection OBCItems;

	public DPSelectedItemEventHandler CloseHandler;

	private static Dictionary<int, ManorIntro> dicManorIntro = new Dictionary<int, ManorIntro>();
}
