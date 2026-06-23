using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TianFuTips : UserControl
{
	protected override void InitializeComponent()
	{
		this._SureBtn.Text = Global.GetLang("确定");
		this._AddSkillPoint.text = "0";
		this._AddBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			TianFuSystemPart.getShengYuDianShu();
			if (this._Point + 1 <= TianFuSystemPart.getShengYuDianShu())
			{
				this._Point++;
			}
			if (this._Point > this._PointMax)
			{
				this._Point = this._PointMax;
			}
			this.ChangePoint(this._Point);
		};
		this._CutBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._Point--;
			if (this._Point < 0)
			{
				this._Point = 0;
			}
			this.ChangePoint(this._Point);
		};
		this._SureBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SuerPoint();
		};
		this._CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseTips();
		};
		this.InitStr();
	}

	public string getTianFuPropertyXml(int type)
	{
		return string.Format("Config/TianFuProperty_{0}.Xml", type);
	}

	public void CloseTips()
	{
		base.gameObject.SetActive(false);
		this._Tips1.gameObject.SetActive(false);
		this._Tips3.gameObject.SetActive(false);
		this._Tips4.gameObject.SetActive(false);
	}

	private void InitStr()
	{
		this._CurrStr.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("<当前等级>")
		});
		this._LastStr.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("<下一等级>")
		});
		this.activityTiaoJian.text = Global.GetLang("{fac60d}激活需求{-}");
		this._SureBtn.Text = Global.GetLang("确定");
	}

	private new void Start()
	{
	}

	public void getSkillExplain(int lv)
	{
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(this.TianFuInfo, "Config"), "TianFuProperty", "ID", this._ID.ToString());
		XElement xel = xelementList[0];
		this._CurrEffect.text = this.setSkillExplain(xel, lv);
		if (this._CurrEffect.text == string.Empty)
		{
			this._CurrEffect.text = Global.GetLang("无效果");
		}
		this._LastSkill.text = this.setSkillExplain(xel, lv + 1);
		if (this._LastSkill.text == string.Empty)
		{
			this._LastSkill.text = Global.GetLang("无效果");
		}
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

	private void ChangePoint(int point)
	{
		if (point < 0)
		{
			return;
		}
		this.getSkillExplain(point + this.CurrLv);
		this.Name(this.ProName, point + this.CurrLv, this.MaxLv);
		this._AddSkillPoint.text = point.ToString();
	}

	private void SuerPoint()
	{
		if (this._Point > 0)
		{
			GameInstance.Game.TianFuJiaDian(this._ID, this._Point);
		}
	}

	public new void Name(string name, int curr, int max)
	{
		string text = string.Format("{0}({1}/{2})", name, curr, max);
		this._SkillName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			text
		});
	}

	public void TipsType(int Tips, TianFuItem TianFuItem)
	{
		this.MaxLv = TianFuItem._MaxLv;
		this.ProName = TianFuItem.Name;
		this.CurrLv = TianFuItem._CurrLv;
		this._ID = TianFuItem.ID;
		if (GameInstance.Game.isOtherFaceId)
		{
			this.TianFuInfo = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.otherPlayerTalentData.Occupation));
		}
		else
		{
			this.TianFuInfo = Global.GetGameResXml(this.getTianFuPropertyXml(Global.Data.roleData.Occupation));
		}
		this._Point = 0;
		this._PointMax = 0;
		this._PointMax = TianFuItem._MaxLv - TianFuItem._CurrLv;
		this.ChangePoint(this._Point);
		this._Tips1.SetActive(false);
		this._Tips3.SetActive(false);
		this._Tips4.SetActive(false);
		switch (Tips)
		{
		case 1:
			this.Tips1();
			break;
		case 2:
			this.Tips2();
			break;
		case 3:
			this.FullTips(TianFuItem._CurrSkillStr);
			this.Tips3();
			break;
		case 4:
			this.UnactivateTips(TianFuItem._LastSkillStr, TianFuItem._SkillTiaoJian_1, TianFuItem._SkillTiaoJian_2, TianFuItem._ItemType, TianFuItem._NeedTianFuLevel);
			this.Tips4();
			break;
		}
	}

	private void Tips1()
	{
		if (this._Tips1 != null)
		{
			this._Tips1.gameObject.SetActive(true);
			this._CurrEffect.gameObject.SetActive(true);
			this._LastSkill.gameObject.transform.localPosition = new Vector3(0f, -22f, -2f);
			if (GameInstance.Game.isOtherFaceId)
			{
				this.SetButtonFalse();
				this.TipsBackGround.gameObject.transform.localScale = this.tipsTmpScale;
			}
			else
			{
				this.TipsBackGround.gameObject.transform.localScale = this.tips1Scale;
			}
		}
	}

	private void Tips2()
	{
		if (this._Tips1 != null)
		{
			this._Tips1.gameObject.SetActive(true);
			if (GameInstance.Game.isOtherFaceId)
			{
				this.SetButtonFalse();
				this.TipsBackGround.gameObject.transform.localScale = this.tipsTmpScale;
			}
			else
			{
				this.TipsBackGround.gameObject.transform.localScale = this.tips1Scale;
			}
		}
	}

	private void Tips3()
	{
		if (GameInstance.Game.isOtherFaceId)
		{
			this.SetButtonFalse();
			this.TipsBackGround.gameObject.transform.localScale = this.tipsTmpScale;
		}
		else
		{
			this.TipsBackGround.gameObject.transform.localScale = this.tips3Scale;
		}
		this._Tips3.gameObject.SetActive(true);
	}

	private void Tips4()
	{
		if (GameInstance.Game.isOtherFaceId)
		{
			this.IsShowAcitvityTiaoJianLabel(false);
			this.TipsBackGround.gameObject.transform.localScale = this.tipsTmpScale;
		}
		else
		{
			this.TipsBackGround.gameObject.transform.localScale = this.tips4Scale;
		}
		this._Tips4.gameObject.SetActive(true);
	}

	public void ActivateTips(string str, string str1)
	{
		this._CurrEffect.text = str;
		this._LastSkill.text = str1;
	}

	public void ActivateTipsZ(string str)
	{
		this._CurrEffect.text = Global.GetLang("无效果");
		this._LastSkill.text = str;
	}

	public void UnactivateTips(string str, string info, string info1, int type, int needSkillLevel)
	{
		this.lastLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("<下一等级>")
		}) + "\n" + str;
		string text = string.Empty;
		string text2 = string.Empty;
		int num = Global.Data.roleData.MyTalentData.CountList[type];
		if (int.Parse(info1) != 0)
		{
			if (num >= int.Parse(info1))
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("需要{0}天赋达到{1}"), this.getTianFuType(type), info1)
				});
			}
			else
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0d00",
					string.Format(Global.GetLang("需要{0}天赋达到{1}"), this.getTianFuType(type), info1)
				});
			}
		}
		if (int.Parse(info) != -1)
		{
			if (this.TianFuTiaoJianSer(type, int.Parse(info)) != null && this.TianFuTiaoJianSer(type, int.Parse(info)).Level >= needSkillLevel)
			{
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("需要升级天赋{0}达到{1}"), this.getSkillName(int.Parse(info)), needSkillLevel)
				});
			}
			else
			{
				text2 = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0d00",
					string.Format(Global.GetLang("需要升级天赋{0}达到{1}"), this.getSkillName(int.Parse(info)), needSkillLevel)
				});
			}
		}
		this.tiaoJianLabel.text = text2 + "\n" + text;
	}

	public string getSkillName(int id)
	{
		if (id != -1)
		{
			XElement gameResXml = Global.GetGameResXml(TianFuSkillPro._OccupationXml);
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "TianFuProperty", "ID", id.ToString());
			XElement xelement = xelementList[0];
			return Global.GetXElementAttributeStr(xelement, "Name");
		}
		return string.Empty;
	}

	private string getTianFuType(int type)
	{
		switch (type)
		{
		case 1:
			return Global.GetLang("野蛮");
		case 2:
			return Global.GetLang("冷血");
		case 3:
			return Global.GetLang("无情");
		default:
			return string.Empty;
		}
	}

	private TalentEffectItem TianFuTiaoJianSer(int type, int id)
	{
		if (Global.Data.roleData.MyTalentData.EffectList == null)
		{
			return null;
		}
		List<TalentEffectItem> effectList = Global.Data.roleData.MyTalentData.EffectList;
		for (int i = 0; i < effectList.Count; i++)
		{
			if (effectList[i].ID == id)
			{
				return effectList[i];
			}
		}
		return null;
	}

	public void FullTips(string str)
	{
		this.maxLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("<当前等级>")
		}) + "\n\n" + str;
	}

	private void SetButtonFalse()
	{
		NGUITools.SetActive(this._SureBtn.gameObject, false);
		NGUITools.SetActive(this._CutBtn.gameObject, false);
		NGUITools.SetActive(this._AddBtn.gameObject, false);
		NGUITools.SetActive(this._LastSkill.gameObject, false);
		NGUITools.SetActive(this._LastStr.gameObject, false);
		NGUITools.SetActive(this._AddSkillPoint.gameObject, false);
		NGUITools.SetActive(this.PointBackGround.gameObject, false);
	}

	private void IsShowAcitvityTiaoJianLabel(bool isShow)
	{
		NGUITools.SetActive(this.tiaoJianLabel.gameObject, isShow);
		NGUITools.SetActive(this.activityTiaoJian.gameObject, isShow);
	}

	public UILabel _CurrStr;

	public UILabel _LastStr;

	public UILabel _SkillName;

	public UILabel _CurrEffect;

	public UILabel _LastSkill;

	public UISprite TipsBackGround;

	public UILabel _AddSkillPoint;

	public GButton _AddBtn;

	public GButton _CutBtn;

	public GButton _SureBtn;

	public GButton _CloseBtn;

	public UISprite PointBackGround;

	private int _Point;

	private int _PointMax;

	private Vector3 tips3Scale = new Vector3(270f, 200f, 1f);

	private Vector3 tips4Scale = new Vector3(270f, 240f, 1f);

	private Vector3 tips1Scale = new Vector3(270f, 314f, 1f);

	private Vector3 tipsTmpScale = new Vector3(270f, 180f, 1f);

	public GameObject _Tips1;

	public GameObject _Tips3;

	public GameObject _Tips4;

	public UILabel lastLabel;

	public UILabel tiaoJianLabel;

	public UILabel activityTiaoJian;

	public UILabel maxLabel;

	public int _ID;

	private XElement TianFuInfo;

	private int CurrLv;

	private string ProName;

	private int MaxLv;
}
