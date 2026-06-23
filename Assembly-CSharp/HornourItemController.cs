using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class HornourItemController : MonoBehaviour
{
	public void Init()
	{
		for (int i = 0; i < this.hornourItems.Count; i++)
		{
			this.hornourItems[i].DPSelectedItem = this.DPSelectedItem;
		}
		this.progressBar.BodyHeight = 8.0;
		this.progressBar.BodyWidth = 470.0;
		this.progressBar.Percent = 0.0;
	}

	public void SetHornourItemsStatus()
	{
		PrestigeMedalData adendaData = Global.Data.adendaData;
		XElement gameResXml = Global.GetGameResXml("Config/ShengWangXunZhang.xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		int num = (adendaData.MedalID > 6) ? 6 : adendaData.MedalID;
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
			int num2 = xelementAttributeInt - 1;
			if (xelementAttributeInt < num)
			{
				this.hornourItems[num2].SetHornourItemSelectedStatus(true, false);
			}
			else if (xelementAttributeInt == num)
			{
				this.hornourItems[num2].SetHornourItemSelectedStatus(true, true);
			}
			else
			{
				this.hornourItems[num2].SetHornourItemSelectedStatus(false, false);
			}
		}
	}

	public void RefreshLevelProgress(bool start_over = false)
	{
		if (Global.Data.adendaData.MedalID == 1)
		{
			this.progressBar.Percent = 0.0;
			return;
		}
		if (start_over)
		{
			this.progressBar.Percent = (double)((float)(Global.Data.adendaData.MedalID - 1) / 5f);
			return;
		}
		this.progressBar.TweenPercent((double)((!start_over) ? ((float)(Global.Data.adendaData.MedalID - 2) / 5f) : 0f), (double)((float)(Global.Data.adendaData.MedalID - 1) / 5f), 1.0);
	}

	public void SetHornourItemSelectedStatus(int id)
	{
		XElement xelementByID = this.GetXElementByID(id);
		this.hornourItems[id - 1].SetHornourItemSelectedStatus(true, true);
	}

	public bool ItemActive(int id)
	{
		int num = id - 1;
		return this.hornourItems[num].isEnabled;
	}

	public GameObject GetLevelupAnimationPosition(int index)
	{
		int num = index - 1;
		if (num < 0 || this.hornourItems.Count <= num)
		{
			return null;
		}
		return this.hornourItems[num].gBtn.gameObject;
	}

	public void CreatePre(string str = "")
	{
		if (null == this.fontAddition)
		{
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.fontAddition);
		gameObject.transform.parent = this.fontAddition.transform.parent;
		gameObject.transform.localScale = new Vector3(28f, 28f, 0f);
		if (str != string.Empty)
		{
			gameObject.GetComponent<UILabel>().text = str;
		}
		gameObject.SetActive(true);
	}

	public void DesPre(object game)
	{
		TweenAlpha tweenAlpha = game as TweenAlpha;
		Object.Destroy(tweenAlpha.gameObject);
	}

	public XElement GetXElementByID(int id)
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShengWangXunZhang.xml");
		return Global.GetXElement(gameResXml, "ShengWangXunZhang", "ID", id.ToString());
	}

	public void SetItemSelected(int oldIndex, int index)
	{
		if (oldIndex == index)
		{
			return;
		}
		int num = oldIndex - 1;
		int num2 = index - 1;
		this.hornourItems[num].SetHornourItemSelected(false);
		this.hornourItems[num2].SetHornourItemSelected(true);
	}

	public List<HornourItem> hornourItems = new List<HornourItem>();

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject fontAddition;

	public GImgProgressBar progressBar;
}
