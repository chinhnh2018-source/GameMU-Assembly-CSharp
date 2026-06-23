using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MeiLanShuCaShiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_BtnSave.Text = Global.GetLang("保存");
		this.m_BtnClose.Text = Global.GetLang("取消");
		this.staticText.text = Global.GetLang("秘语属性");
		base.InitializeComponent();
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsShuXingChaKan)
			{
				TCPGameServerCmds.CMD_SPR_MERLIN_SECRET_ATTR_NOT_REPLACE.SendDataUseRoleID();
			}
			Object.Destroy(base.gameObject);
		};
		this.m_BtnSave.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			TCPGameServerCmds.CMD_SPR_MERLIN_SECRET_ATTR_REPLACE.SendDataUseRoleID();
		};
		this.m_BtnClose1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsShuXingChaKan)
			{
				TCPGameServerCmds.CMD_SPR_MERLIN_SECRET_ATTR_NOT_REPLACE.SendDataUseRoleID();
			}
			Object.Destroy(base.gameObject);
		};
		base.StartCoroutine(this.Innit());
		base.StartCoroutine(this.ShowShengyuShijian());
	}

	public MerlinGrowthSaveDBData DataBag
	{
		get
		{
			return this.dataBag;
		}
		set
		{
			this.dataBag = value;
		}
	}

	public IEnumerator Innit()
	{
		yield return new WaitForSeconds(0.01f);
		this.InnitData(this.DataBag);
		yield break;
	}

	private void InnitData(MerlinGrowthSaveDBData DataBag)
	{
		if (this.IsShuXingChaKan)
		{
			this.m_LableJianSu.text = Global.GetLang("{D9C7B1}减速几率: +{-}") + DataBag._ActiveAttr[2] + "%      ";
			this.m_LableZhongJi.text = Global.GetLang("{D9C7B1}重击几率: +{-}") + DataBag._ActiveAttr[3] + "%      ";
			this.m_LableDongJie.text = Global.GetLang("{D9C7B1}冻结几率: +{-}") + DataBag._ActiveAttr[0] + "%      ";
			this.m_LableMaBi.text = Global.GetLang("{D9C7B1}麻痹几率: +{-}") + DataBag._ActiveAttr[1] + "%      ";
			double num = DataBag._ActiveAttr[0] + DataBag._ActiveAttr[1] + DataBag._ActiveAttr[2] + DataBag._ActiveAttr[3];
			int num2 = 0;
			XElement gameResXml = Global.GetGameResXml("Config/MagicWord.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "MagicWord");
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Level");
				if (DataBag._Level == xelementAttributeInt)
				{
					num2 = Global.GetXElementAttributeInt(xelementList[i], "MaxNum");
				}
			}
			if (num < (double)num2)
			{
				this.m_LableZonghe.text = string.Concat(new object[]
				{
					Global.GetLang("{DFB10E}秘语总和:   {-}"),
					num,
					"%",
					Global.GetColorStringForNGUIText(new object[]
					{
						"F80200",
						string.Format(Global.GetLang("(上限{0}%)"), num2)
					})
				});
			}
			else
			{
				this.m_LableZonghe.text = string.Concat(new object[]
				{
					Global.GetLang("{DFB10E}秘语总和:   {-}"),
					num,
					"%",
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(Global.GetLang("(上限{0}%)"), num2)
					})
				});
			}
			foreach (Transform transform in this.JianTous)
			{
				transform.GetComponent<UISprite>().enabled = false;
			}
		}
		else
		{
			this.m_LableJianSu.text = string.Concat(new object[]
			{
				Global.GetLang("{D9C7B1}减速几率: +{-}"),
				DataBag._ActiveAttr[2],
				"%         ",
				DataBag._UnActiveAttr[2],
				"%"
			});
			this.m_LableZhongJi.text = string.Concat(new object[]
			{
				Global.GetLang("{D9C7B1}重击几率: +{-}"),
				DataBag._ActiveAttr[3],
				"%         ",
				DataBag._UnActiveAttr[3],
				"%"
			});
			this.m_LableDongJie.text = string.Concat(new object[]
			{
				Global.GetLang("{D9C7B1}冻结几率: +{-}"),
				DataBag._ActiveAttr[0],
				"%         ",
				DataBag._UnActiveAttr[0],
				"%"
			});
			this.m_LableMaBi.text = string.Concat(new object[]
			{
				Global.GetLang("{D9C7B1}麻痹几率: +{-}"),
				DataBag._ActiveAttr[1],
				"%         ",
				DataBag._UnActiveAttr[1],
				"%"
			});
			double num3 = DataBag._ActiveAttr[0] + DataBag._ActiveAttr[1] + DataBag._ActiveAttr[2] + DataBag._ActiveAttr[3];
			double num4 = DataBag._UnActiveAttr[0] + DataBag._UnActiveAttr[1] + DataBag._UnActiveAttr[2] + DataBag._UnActiveAttr[3];
			int num5 = 0;
			XElement gameResXml2 = Global.GetGameResXml("Config/MagicWord.xml");
			List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "MagicWord");
			for (int k = 0; k < xelementList2.Count; k++)
			{
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList2[k], "Level");
				if (DataBag._Level == xelementAttributeInt2)
				{
					num5 = Global.GetXElementAttributeInt(xelementList2[k], "MaxNum");
				}
			}
			if (num4 < (double)num5)
			{
				this.m_LableZonghe.text = Global.GetLang("{DFB10E}秘语总和:      {-}") + num3 + "%";
				this.m_LableZonghe1.text = num4 + "%" + Global.GetColorStringForNGUIText(new object[]
				{
					"F80200",
					string.Format(Global.GetLang("(上限{0}%)"), num5)
				});
			}
			else
			{
				this.m_LableZonghe.text = Global.GetLang("{DFB10E}秘语总和:      {-}") + num3 + "%";
				this.m_LableZonghe1.text = num4 + "%" + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("(上限{0}%)"), num5)
				});
			}
			UILabel[] array = new UILabel[]
			{
				this.m_LableDongJie,
				this.m_LableMaBi,
				this.m_LableJianSu,
				this.m_LableZhongJi
			};
			int num6 = 0;
			foreach (KeyValuePair<int, double> keyValuePair in DataBag._UnActiveAttr)
			{
				double num7 = keyValuePair.Value - Enumerable.ToList<double>(DataBag._ActiveAttr.Values)[num6];
				if (num7 > 0.0)
				{
					UILabel uilabel = array[num6];
					string text = uilabel.text;
					uilabel.text = string.Concat(new object[]
					{
						text,
						"{0EFF04} (",
						Math.Abs(num7),
						"%){-}"
					});
				}
				else if (num7 == 0.0)
				{
					UILabel uilabel2 = array[num6];
					uilabel2.text += "      {-}";
				}
				else if (num7 < 0.0)
				{
					UILabel uilabel3 = array[num6];
					string text = uilabel3.text;
					uilabel3.text = string.Concat(new object[]
					{
						text,
						"{FE0800} (",
						Math.Abs(num7),
						"%){-}"
					});
				}
				num6++;
			}
		}
	}

	public IEnumerator ShowShengyuShijian()
	{
		if (this.DataBag == null)
		{
			yield return null;
		}
		DateTime date = new DateTime(this.DataBag._ToTicks * 10000L);
		DateTime now = new DateTime(Global.GetCorrectLocalTime() * 10000L);
		if (this.DataBag._ToTicks != 0L && date > now)
		{
			string str_Hours = ((date - now).Hours + (date - now).Days * 24).ToString();
			string str_Minutes = (date - now).Minutes.ToString();
			string str_Seconds = (date - now).Seconds.ToString();
			if (str_Hours.Length == 1)
			{
				str_Hours = "0" + str_Hours;
			}
			if (str_Minutes.Length == 1)
			{
				str_Minutes = "0" + str_Minutes;
			}
			if (str_Seconds.Length == 1)
			{
				str_Seconds = "0" + str_Seconds;
			}
			this.m_LableShengYuShiJian.text = string.Concat(new string[]
			{
				Global.GetLang("剩余"),
				":{0EFF04}",
				str_Hours,
				":",
				str_Minutes,
				":",
				str_Seconds
			});
		}
		else
		{
			this.m_LableShengYuShiJian.text = string.Empty;
			for (int i = 0; i < this.DataBag._ActiveAttr.Count; i++)
			{
				this.DataBag._ActiveAttr[i] = 0.0;
			}
			this.InnitData(this.DataBag);
		}
		yield return new WaitForSeconds(1f);
		base.StartCoroutine(this.ShowShengyuShijian());
		yield break;
	}

	protected override void OnDestroy()
	{
	}

	public UILabel staticText;

	public GButton m_BtnClose;

	public GButton m_BtnClose1;

	public GButton m_BtnSave;

	public UILabel m_LableShengYuShiJian;

	public bool IsShuXingChaKan;

	public Transform[] JianTous;

	public UILabel m_LableJianSu;

	public UILabel m_LableZhongJi;

	public UILabel m_LableDongJie;

	public UILabel m_LableMaBi;

	public UILabel m_LableZonghe;

	public UILabel m_LableZonghe1;

	public Transform m_Attribut;

	private MerlinGrowthSaveDBData dataBag;
}
