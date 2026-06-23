using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TianFuSkillPro : UserControl
{
	protected override void InitializeComponent()
	{
		this.YM_OBC = this.YM_ListBox.ItemsSource;
		this.JR_OBC = this.JR_ListBox.ItemsSource;
		this.JM_OBC = this.JM_ListBox.ItemsSource;
		this.YM_DPSelectedItem = new DPSelectedItemEventHandler(this.YM_BtnLIstence);
		this.JR_DPSelectedItem = new DPSelectedItemEventHandler(this.JR_BtnLIstence);
		this.JM_DPSelectedItem = new DPSelectedItemEventHandler(this.JM_BtnLIstence);
		UIEventListener.Get(this.YM_Image.gameObject).onClick = delegate(GameObject s)
		{
			this.TianFuExplain.gameObject.SetActive(true);
			this.TianFuExplain.setTianFuShuoMing(1);
		};
		UIEventListener.Get(this.JR_Image.gameObject).onClick = delegate(GameObject s)
		{
			this.TianFuExplain.gameObject.SetActive(true);
			this.TianFuExplain.setTianFuShuoMing(2);
		};
		UIEventListener.Get(this.JM_Image.gameObject).onClick = delegate(GameObject s)
		{
			this.TianFuExplain.gameObject.SetActive(true);
			this.TianFuExplain.setTianFuShuoMing(3);
		};
	}

	private new void Start()
	{
	}

	public void initGoodList()
	{
		this.InitTianFu();
		base.StartCoroutine<bool>(this.AddGoodListIcon());
	}

	public void InitTianFu()
	{
		TalentData talentData = Global.Data.roleData.MyTalentData;
		if (GameInstance.Game.isOtherFaceId)
		{
			talentData = Global.Data.otherPlayerTalentData;
		}
		if (talentData != null && talentData.CountList != null)
		{
			this.YM_Label.text = talentData.CountList[1].ToString();
			this.JR_Label.text = talentData.CountList[2].ToString();
			this.JM_Label.text = talentData.CountList[3].ToString();
		}
	}

	public string getTianFuPropertyXml(int type)
	{
		return string.Format("Config/TianFuProperty_{0}.Xml", type);
	}

	public IEnumerator AddGoodListIcon()
	{
		if (GameInstance.Game.isOtherFaceId)
		{
			TianFuSkillPro._OccupationXml = this.getTianFuPropertyXml(Global.Data.otherPlayerTalentData.Occupation);
		}
		else
		{
			TianFuSkillPro._OccupationXml = this.getTianFuPropertyXml(Global.Data.roleData.Occupation);
		}
		if (TianFuSkillPro._OccupationXml != string.Empty)
		{
			XElement tianFuInfo = null;
			if (GameInstance.Game.isOtherFaceId)
			{
				tianFuInfo = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.otherPlayerTalentData.Occupation));
			}
			else
			{
				tianFuInfo = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.roleData.Occupation));
			}
			List<XElement> tianFuIItem = Global.GetXElementList(Global.GetXElement(tianFuInfo, "Config"), "*");
			bool active = false;
			foreach (XElement xel in tianFuIItem)
			{
				string id = Global.GetXElementAttributeStr(xel, "ID");
				int type = int.Parse(Global.GetXElementAttributeStr(xel, "TianFuType"));
				string icon = Global.GetXElementAttributeStr(xel, "Icon");
				int maxLv = int.Parse(Global.GetXElementAttributeStr(xel, "LevelMax"));
				string name = Global.GetXElementAttributeStr(xel, "Name");
				int NeedTianFuLevel = int.Parse(Global.GetXElementAttributeStr(xel, "NeedTianFuLevel"));
				GameObject item = SpawnManager.Instantiate(this.m_SkillPro) as GameObject;
				TianFuItem tf = item.GetComponent<TianFuItem>();
				tf.SetIcon("NetImages/GameRes/Images/Skill/" + icon + ".png");
				item.SetActive(true);
				tf._NeedTianFu = int.Parse(Global.GetXElementAttributeStr(xel, "NeedTianFu"));
				if (type < 0)
				{
					tf._Visible.SetActive(false);
				}
				TalentEffectItem tei = this.TianFuTiaoJianSer(type, int.Parse(id));
				int currLv;
				if (tei == null)
				{
					currLv = 0;
					active = this.TianFuTiaoJian(id, tf);
					tf.ShowNetImage = !active;
				}
				else
				{
					currLv = tei.Level;
					active = true;
					tf.ShowNetImage = false;
					if (tf._NeedTianFu != -1)
					{
						tf._Huang.gameObject.SetActive(true);
						tf.setHuang = "huang";
					}
				}
				if (tf != null)
				{
					tf._Active = active;
					tf.ID = int.Parse(id);
					tf.Name = name;
					tf._ItemType = type;
					tf.setLabel(currLv, maxLv);
					tf._CurrSkillStr = string.Empty;
					tf._NeedTianFuLevel = NeedTianFuLevel;
					if (currLv != 0)
					{
						tf._CurrSkillStr = this.setSkillExplain(xel, currLv);
					}
					tf._LastSkillStr = this.setSkillExplain(xel, currLv + 1);
					tf._SkillTiaoJian_1 = string.Format(Global.GetLang("{0}"), int.Parse(Global.GetXElementAttributeStr(xel, "NeedTianFu")));
					tf._SkillTiaoJian_2 = string.Format(Global.GetLang("{0}"), Global.GetXElementAttributeStr(xel, "NeedInputPoint"));
				}
				this.initItem(item, tf);
				yield return null;
			}
		}
		yield break;
	}

	private string setSkillExplain(XElement xel, int currLv)
	{
		string xelementAttributeStr = Global.GetXElementAttributeStr(xel, "Description");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xel, "Remark");
		string[] array = xelementAttributeStr2.Split(new char[]
		{
			','
		});
		if (Global.GetXElementAttributeStr(xel, "Effect" + currLv) != string.Empty)
		{
			string[] array2 = Global.GetXElementAttributeStr(xel, "Effect" + currLv).Split(new char[]
			{
				'|'
			});
			string[] array3 = array2[0].Split(new char[]
			{
				','
			});
			float num = float.Parse(array3[int.Parse(array[0])]);
			if (int.Parse(array[1]) == 0)
			{
				num *= 100f;
			}
			return string.Format(xelementAttributeStr, num);
		}
		return string.Empty;
	}

	public void UpdateTianFu()
	{
		int click_TianFu_Type = this.Click_TianFu_Type;
		switch (click_TianFu_Type)
		{
		case 1:
			this.UpdateTianFuItem(this.YM_Dic);
			break;
		case 2:
			this.UpdateTianFuItem(this.JR_Dic);
			break;
		case 3:
			this.UpdateTianFuItem(this.JM_Dic);
			break;
		default:
			if (click_TianFu_Type == 100)
			{
				this.UpdateTianFuItem(this.YM_Dic);
				this.UpdateTianFuItem(this.JR_Dic);
				this.UpdateTianFuItem(this.JM_Dic);
			}
			break;
		}
	}

	private void UpdateTianFuItem(Dictionary<int, TianFuItem> dic)
	{
		XElement gameResXml;
		if (GameInstance.Game.isOtherFaceId)
		{
			gameResXml = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.otherPlayerTalentData.Occupation));
		}
		else
		{
			gameResXml = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.roleData.Occupation));
		}
		foreach (int num in dic.Keys)
		{
			TianFuItem tianFuItem = dic[num];
			if (tianFuItem._ItemType >= 0)
			{
				TalentEffectItem talentEffectItem = this.TianFuTiaoJianSer(tianFuItem._ItemType, num);
				if (talentEffectItem == null)
				{
					tianFuItem._CurrLv = 0;
					tianFuItem._Active = this.TianFuTiaoJian(num.ToString(), tianFuItem);
					tianFuItem.ShowNetImage = !tianFuItem._Active;
				}
				else
				{
					tianFuItem._CurrLv = talentEffectItem.Level;
					tianFuItem._Active = true;
					tianFuItem.ShowNetImage = false;
					if (tianFuItem._NeedTianFu != -1)
					{
						tianFuItem._Huang.gameObject.SetActive(true);
						tianFuItem.setHuang = "huang";
					}
				}
				List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuProperty", "ID", tianFuItem.ID.ToString());
				XElement xel = xelementList[0];
				if (tianFuItem._CurrLv != 0)
				{
					tianFuItem._CurrSkillStr = this.setSkillExplain(xel, tianFuItem._CurrLv);
				}
				tianFuItem._LastSkillStr = this.setSkillExplain(xel, tianFuItem._CurrLv + 1);
				tianFuItem.setLabel(tianFuItem._CurrLv, tianFuItem._MaxLv);
			}
		}
	}

	private bool TianFuTiaoJian(string id, TianFuItem tf)
	{
		XElement gameResXml = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.roleData.Occupation));
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuProperty", "ID", id);
		XElement xelement = xelementList[0];
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NeedTianFu");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "TianFuType");
		int num = int.Parse(Global.GetXElementAttributeStr(xelement, "NeedTianFuLevel"));
		if (xelementAttributeInt2 < 0)
		{
			return false;
		}
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "NeedInputPoint");
		int num2 = Global.Data.roleData.MyTalentData.CountList[xelementAttributeInt2];
		if (xelementAttributeInt == -1)
		{
			tf._Huang.gameObject.SetActive(false);
			if (num2 >= xelementAttributeInt3)
			{
				return true;
			}
		}
		else
		{
			tf._Huang.gameObject.SetActive(true);
			if (this.TianFuTiaoJianSer(xelementAttributeInt2, xelementAttributeInt) != null && num2 >= xelementAttributeInt3 && this.TianFuTiaoJianSer(xelementAttributeInt2, xelementAttributeInt).Level >= num)
			{
				tf.setHuang = "huang";
				return true;
			}
			tf.setHuang = "hui";
		}
		return false;
	}

	private TalentEffectItem TianFuTiaoJianSer(int type, int id)
	{
		List<TalentEffectItem> effectList;
		if (GameInstance.Game.isOtherFaceId)
		{
			effectList = Global.Data.otherPlayerTalentData.EffectList;
		}
		else
		{
			effectList = Global.Data.roleData.MyTalentData.EffectList;
		}
		if (type < 0 || effectList == null)
		{
			return null;
		}
		for (int i = 0; i < effectList.Count; i++)
		{
			if (effectList[i].ID == id)
			{
				return effectList[i];
			}
		}
		return null;
	}

	private void initItem(GameObject item, TianFuItem tf)
	{
		switch (Mathf.Abs(tf._ItemType))
		{
		case 1:
			this.YM_Dic.Add(tf.ID, tf);
			this.YM_OBC.Add(item);
			tf.DPSelectedItem = this.YM_DPSelectedItem;
			break;
		case 2:
			this.JR_Dic.Add(tf.ID, tf);
			this.JR_OBC.Add(item);
			tf.DPSelectedItem = this.JR_DPSelectedItem;
			break;
		case 3:
			this.JM_Dic.Add(tf.ID, tf);
			this.JM_OBC.Add(item);
			tf.DPSelectedItem = this.JM_DPSelectedItem;
			break;
		}
		if (item.GetComponent<UIPanel>() != null)
		{
			Object.Destroy(item.GetComponent<UIPanel>());
		}
	}

	public void YM_BtnLIstence(object sender, DPSelectedItemEventArgs args)
	{
		if (!this._XuanZhongKuang.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.SetActive(true);
		}
		TianFuItem tianFuItem = this.YM_Dic[args.ID];
		if (tianFuItem.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.transform.position = tianFuItem.gameObject.transform.position;
		}
		if (this.tft != null)
		{
			this.tft.gameObject.SetActive(true);
			this.tft.Name(tianFuItem.Name, tianFuItem._CurrLv, tianFuItem._MaxLv);
			this.tft.TipsType(tianFuItem.getTipsType(), tianFuItem);
			this.Click_TianFu_Type = tianFuItem._ItemType;
		}
	}

	public void JM_BtnLIstence(object sender, DPSelectedItemEventArgs args)
	{
		if (!this._XuanZhongKuang.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.SetActive(true);
		}
		TianFuItem tianFuItem = this.JM_Dic[args.ID];
		if (tianFuItem.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.transform.position = tianFuItem.gameObject.transform.position;
		}
		if (this.tft != null)
		{
			this.tft.gameObject.SetActive(true);
			this.tft.Name(tianFuItem.Name, tianFuItem._CurrLv, tianFuItem._MaxLv);
			this.tft.TipsType(tianFuItem.getTipsType(), tianFuItem);
			this.Click_TianFu_Type = tianFuItem._ItemType;
		}
	}

	public void JR_BtnLIstence(object sender, DPSelectedItemEventArgs args)
	{
		if (!this._XuanZhongKuang.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.SetActive(true);
		}
		TianFuItem tianFuItem = this.JR_Dic[args.ID];
		if (tianFuItem.gameObject.activeSelf)
		{
			this._XuanZhongKuang.gameObject.transform.position = tianFuItem.gameObject.transform.position;
		}
		if (this.tft != null)
		{
			this.tft.gameObject.SetActive(true);
			this.tft.Name(tianFuItem.Name, tianFuItem._CurrLv, tianFuItem._MaxLv);
			this.tft.TipsType(tianFuItem.getTipsType(), tianFuItem);
			this.Click_TianFu_Type = tianFuItem._ItemType;
		}
	}

	public ListBox YM_ListBox = new ListBox();

	public ListBox JR_ListBox = new ListBox();

	public ListBox JM_ListBox = new ListBox();

	private ObservableCollection YM_OBC;

	private ObservableCollection JR_OBC;

	private ObservableCollection JM_OBC;

	public GameObject m_SkillPro;

	public Dictionary<int, TianFuItem> YM_Dic = new Dictionary<int, TianFuItem>();

	public Dictionary<int, TianFuItem> JR_Dic = new Dictionary<int, TianFuItem>();

	public Dictionary<int, TianFuItem> JM_Dic = new Dictionary<int, TianFuItem>();

	public DPSelectedItemEventHandler YM_DPSelectedItem;

	public DPSelectedItemEventHandler JR_DPSelectedItem;

	public DPSelectedItemEventHandler JM_DPSelectedItem;

	public TianFuTips tft;

	public UISprite _XuanZhongKuang;

	public static string _OccupationXml;

	public UILabel YM_Label;

	public UILabel JR_Label;

	public UILabel JM_Label;

	public int Click_TianFu_Type;

	public UISprite YM_Image;

	public UISprite JR_Image;

	public UISprite JM_Image;

	public TianFuExplain TianFuExplain;
}
