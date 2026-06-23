using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class ChongShengLianLuJiaGongItem : UserControl
{
	public int GoodId
	{
		get
		{
			return this.m_GoodId;
		}
		set
		{
			this.m_GoodId = value;
		}
	}

	public int Number
	{
		get
		{
			return this.number;
		}
		set
		{
			if (value > 0)
			{
				this.m_LabNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang(string.Format(Global.GetLang("数量:{0}"), value))
				});
				this.m_Sp0Number.gameObject.SetActive(false);
			}
			else
			{
				this.m_LabNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang(string.Format(Global.GetLang("数量:{0}"), value))
				});
				this.m_Sp0Number.gameObject.SetActive(true);
			}
			this.number = value;
		}
	}

	public string BaoShiName
	{
		get
		{
			return this.baoShiName;
		}
		set
		{
			this.m_LabName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
			this.baoShiName = value;
		}
	}

	public ChongShengBaoShiVO BaoShiData
	{
		get
		{
			return this.chongShengBaoShiVO;
		}
		set
		{
			this.chongShengBaoShiVO = value;
		}
	}

	public void SetDataChongSheng(ChongShengBaoShiVO voData)
	{
		this.GoodId = voData.BaoShiID;
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(voData.BaoShiID, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon = Global.CreateGoodsIcon(dummyGoodsDataMu, false, true);
		if (ggoodIcon.GetComponent<BoxCollider>() != null)
		{
			Object.Destroy(ggoodIcon.GetComponent<BoxCollider>());
		}
		ggoodIcon.transform.SetParent(this.m_GmaeParent.transform, false);
		this.BaoShiData = voData;
		this.BaoShiName = voData.Name;
		this.m_ListLabContent[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("攻击装备佩戴")
		});
		this.m_ListLabContent[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("防御装备佩戴")
		});
		string[] array = voData.ShuXing.Split(new char[]
		{
			'|'
		});
		ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[0].Split(new char[]
		{
			','
		})[0]);
		if (ConfigExtPropIndexes.GetPercentByWord(array[0].Split(new char[]
		{
			','
		})[0]))
		{
			this.m_ListLabContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord.Description, (float.Parse(array[0].Split(new char[]
				{
					','
				})[1]) * 100f).ToString("f1")))
			});
		}
		else
		{
			this.m_ListLabContent[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord.Description, array[0].Split(new char[]
				{
					','
				})[1]))
			});
		}
		ExtPropIndexesVO extPropIndexesVOByWord2 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array[1].Split(new char[]
		{
			','
		})[0]);
		if (ConfigExtPropIndexes.GetPercentByWord(array[1].Split(new char[]
		{
			','
		})[0]))
		{
			this.m_ListLabContent[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord2.Description, (float.Parse(array[1].Split(new char[]
				{
					','
				})[1]) * 100f).ToString("f1")))
			});
		}
		else
		{
			this.m_ListLabContent[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord2.Description, array[1].Split(new char[]
				{
					','
				})[1]))
			});
		}
	}

	public UILabel m_LabName;

	public UILabel m_LabNumber;

	public GameObject m_GmaeParent;

	public UISprite m_Sp0Number;

	public List<UILabel> m_ListLabContent;

	private int m_GoodId;

	private int number;

	private string baoShiName = string.Empty;

	private ChongShengBaoShiVO chongShengBaoShiVO;
}
