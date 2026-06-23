using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LianluZhuangbeiFuMoPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitText();
		this.RefreshShuXing();
		this.m_BtnFuMo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartFuMo();
		};
		this.m_BtnBaoCun.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GoodsData goodsData = this.icon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GameInstance.Game.SenFuMoZhuangBeiBaoCun(goodsData.Id);
		};
		this.m_PanelYuLanParent.gameObject.SetActive(false);
		this.AddShuXingYueLan();
		this.m_BtnYuLan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelYuLanParent.gameObject.SetActive(true);
		};
		this.m_BtnYuLanReturn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_PanelYuLanParent.gameObject.SetActive(false);
		};
	}

	private void InitText()
	{
		this.m_LabTitle.gameObject.SetActive(false);
		this.m_BtnBaoCun.gameObject.SetActive(false);
		this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("装备附魔可为装备附加多种属性")
		});
		this.m_LabTitleYuLan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("附魔属性预览")
		});
		this.m_BtnBaoCun.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("保存")
		});
		this.m_BtnFuMo.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("附魔")
		});
		this.m_LabXiaoHaoTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("消耗：")
		});
		this.m_LabHuoBi.text = Global.GetRoleOwnNumByMoneyType(146).ToString();
		if ((long)Global.GetRoleOwnNumByMoneyType(146) < ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost"))
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f00000",
				ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost").ToString()
			});
		}
		else
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost").ToString()
			});
		}
	}

	public void RefreshFuMo()
	{
		if (this.DPEffectItem != null)
		{
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1
			});
		}
		this.m_BtnBaoCun.gameObject.SetActive(false);
		GoodsData goodsData = this.icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Id == goodsData.Id)
				{
					goodsData.ElementhrtsProps = this.strsFuMo;
				}
			}
		}
		this.AddIcon(goodsData);
	}

	public void RefreshFuMo(FuMoCachedTemplate data)
	{
		if ((long)Global.GetRoleOwnNumByMoneyType(146) < ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost"))
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f00000",
				ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost").ToString()
			});
		}
		else
		{
			this.m_LabXiaoHaoNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				ConfigSystemParam.GetSystemParamIntByName("EnchantmentCost").ToString()
			});
		}
		this.m_LabHuoBi.text = Global.GetRoleOwnNumByMoneyType(146).ToString();
		if (data != null)
		{
			this.strsFuMo = data.AttrTypeValue;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.strsFuMo.Count; i += 2)
			{
				if (!ConfigExtPropIndexes.GetPercentByID(this.strsFuMo[i]))
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(this.strsFuMo[i], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						this.strsFuMo[i + 1] / 1000
					}));
				}
				else
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(this.strsFuMo[i], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						this.strsFuMo[i + 1] / 10 + "%"
					}));
				}
				if (i < this.strsFuMo.Count - 2)
				{
					stringBuilder.Append(Environment.NewLine);
				}
			}
			this.AddRightBack(data.AttrTypeValue);
			GoodsData goodsData = this.icon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			if (goodsData.ElementhrtsProps == null || goodsData.ElementhrtsProps.Count <= 0)
			{
				this.RefreshFuMo();
				return;
			}
			if (goodsData.ElementhrtsProps[1] <= 0)
			{
				this.RefreshFuMo();
				return;
			}
			this.m_BtnBaoCun.gameObject.SetActive(true);
			this.m_LabTitle.gameObject.SetActive(false);
			this.m_ShowLeft.gameObject.SetActive(true);
			if (string.IsNullOrEmpty(this.m_LabOld.text))
			{
				this.m_ShowLeft.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
			}
			else
			{
				this.m_ShowLeft.URL = "NetImages/GameRes/Images/FriendFuMo/yizhi_bg.png";
			}
			this.m_ShowRight.gameObject.SetActive(true);
			this.m_ShowRight.URL = "NetImages/GameRes/Images/FriendFuMo/yizhi_bg.png";
			this.m_ShowJianTou.gameObject.SetActive(true);
			this.m_ShowJianTou.URL = "NetImages/GameRes/Images/FriendFuMo/diwen_l.png";
			this.m_LabNew.text = stringBuilder.ToString();
			this.m_SpXianRight.gameObject.SetActive(true);
		}
	}

	public void RefreshEquip(GoodsData gd)
	{
		this.m_BtnBaoCun.gameObject.SetActive(false);
		this.AddIcon(gd);
		this.RefreshShuXing();
	}

	private void AddIcon(GoodsData gd)
	{
		this.m_LabTitle.gameObject.SetActive(false);
		if (this.icon != null)
		{
			Object.Destroy(this.icon.gameObject);
			this.icon = null;
		}
		this.icon = Global.CreateGoodsIcon(gd, false, true);
		this.icon.transform.SetParent(this.m_GameGoodsParent.transform, false);
		this.m_LabOld.text = string.Empty;
		this.m_LabNew.text = string.Empty;
		this.m_SpXianRight.gameObject.SetActive(false);
		this.m_SpRight.gameObject.SetActive(false);
		if (gd.ElementhrtsProps != null && gd.ElementhrtsProps.Count > 0)
		{
			if (gd.ElementhrtsProps[1] <= 0)
			{
				this.m_LabTitle.gameObject.SetActive(false);
				this.m_ShowLeft.gameObject.SetActive(true);
				this.m_ShowLeft.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
				this.m_ShowRight.gameObject.SetActive(true);
				this.m_ShowRight.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
				this.m_ShowJianTou.gameObject.SetActive(true);
				this.m_ShowJianTou.URL = "NetImages/GameRes/Images/FriendFuMo/diwen_lg.png";
				return;
			}
			this.m_LabTitle.gameObject.SetActive(false);
			this.m_ShowLeft.gameObject.SetActive(true);
			this.m_ShowLeft.URL = "NetImages/GameRes/Images/FriendFuMo/yizhi_bg.png";
			this.m_ShowRight.gameObject.SetActive(true);
			this.m_ShowRight.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
			this.m_ShowJianTou.gameObject.SetActive(true);
			this.m_ShowJianTou.URL = "NetImages/GameRes/Images/FriendFuMo/diwen_lg.png";
			List<int> elementhrtsProps = gd.ElementhrtsProps;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < elementhrtsProps.Count; i += 2)
			{
				if (!ConfigExtPropIndexes.GetPercentByID(elementhrtsProps[i]))
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(elementhrtsProps[i], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						elementhrtsProps[i + 1] / 1000
					}));
				}
				else
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(elementhrtsProps[i], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						elementhrtsProps[i + 1] / 10 + "%"
					}));
				}
				if (i < elementhrtsProps.Count - 2)
				{
					stringBuilder.Append(Environment.NewLine);
				}
			}
			this.m_LabOld.text = stringBuilder.ToString();
		}
		else
		{
			this.m_LabTitle.gameObject.SetActive(false);
			this.m_ShowLeft.gameObject.SetActive(true);
			this.m_ShowLeft.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
			this.m_ShowRight.gameObject.SetActive(true);
			this.m_ShowRight.URL = "NetImages/GameRes/Images/FriendFuMo/weizhi_bg.png";
			this.m_ShowJianTou.gameObject.SetActive(true);
			this.m_ShowJianTou.URL = "NetImages/GameRes/Images/FriendFuMo/diwen_lg.png";
		}
		this.AddLeftBack(gd.ElementhrtsProps);
	}

	private void AddLeftBack(List<int> list)
	{
		if (list == null || list.Count <= 0)
		{
			this.m_SpMax.gameObject.SetActive(false);
			this.m_SpXian.gameObject.SetActive(false);
			return;
		}
		if (list[1] <= 0)
		{
			this.m_SpMax.gameObject.SetActive(false);
			this.m_SpXian.gameObject.SetActive(false);
			return;
		}
		this.m_SpMax.gameObject.SetActive(true);
		this.m_SpXian.gameObject.SetActive(true);
		int num = list.Count / 2;
		Vector3 localPosition = this.m_SpMax.transform.localPosition;
		localPosition.y = (float)(3 + (num - 1) * 13);
		this.m_SpMax.transform.localPosition = localPosition;
		for (int i = 0; i < this.m_ListLeftSpBack.Count; i++)
		{
			if (i > num - 1)
			{
				this.m_ListLeftSpBack[i].gameObject.SetActive(false);
			}
			else if (this.m_DicMax.ContainsKey(list[i * 2]))
			{
				if (list[i * 2 + 1] >= this.m_DicMax[list[i * 2]])
				{
					this.m_ListLeftSpBack[i].gameObject.SetActive(true);
				}
				else
				{
					this.m_ListLeftSpBack[i].gameObject.SetActive(false);
				}
			}
			else
			{
				this.m_ListLeftSpBack[i].gameObject.SetActive(false);
			}
		}
		Vector3 localPosition2 = this.m_SpXian.transform.localPosition;
		localPosition2.y = (float)(-7 + (num - 1) * 13);
		this.m_SpXian.transform.localPosition = localPosition2;
	}

	private void AddRightBack(List<int> list)
	{
		if (list == null || list.Count <= 0)
		{
			this.m_SpRight.gameObject.SetActive(false);
			this.m_SpXianRight.gameObject.SetActive(false);
			return;
		}
		if (list[1] <= 0)
		{
			this.m_SpRight.gameObject.SetActive(false);
			this.m_SpXianRight.gameObject.SetActive(false);
			return;
		}
		this.m_SpRight.gameObject.SetActive(true);
		this.m_SpXianRight.gameObject.SetActive(true);
		int num = list.Count / 2;
		Vector3 localPosition = this.m_SpRight.transform.localPosition;
		localPosition.y = (float)(3 + (num - 1) * 13);
		this.m_SpRight.transform.localPosition = localPosition;
		for (int i = 0; i < this.m_ListRightSpBack.Count; i++)
		{
			if (i > num - 1)
			{
				this.m_ListRightSpBack[i].gameObject.SetActive(false);
			}
			else if (this.m_DicMax.ContainsKey(list[i * 2]))
			{
				if (list[i * 2 + 1] >= this.m_DicMax[list[i * 2]])
				{
					this.m_ListRightSpBack[i].gameObject.SetActive(true);
				}
				else
				{
					this.m_ListRightSpBack[i].gameObject.SetActive(false);
				}
			}
			else
			{
				this.m_ListRightSpBack[i].gameObject.SetActive(false);
			}
		}
		Vector3 localPosition2 = this.m_SpXianRight.transform.localPosition;
		localPosition2.y = (float)(-7 + (num - 1) * 13);
		this.m_SpXianRight.transform.localPosition = localPosition2;
	}

	private void RefreshShuXing()
	{
		if (this.icon == null)
		{
			this.m_LabTitle.gameObject.SetActive(true);
			this.m_ShowLeft.gameObject.SetActive(false);
			this.m_ShowRight.gameObject.SetActive(false);
			this.m_ShowJianTou.gameObject.SetActive(false);
		}
		else
		{
			this.m_LabTitle.gameObject.SetActive(false);
			this.m_ShowLeft.gameObject.SetActive(true);
			this.m_ShowRight.gameObject.SetActive(true);
			this.m_ShowJianTou.gameObject.SetActive(true);
		}
	}

	private void StartFuMo()
	{
		if (this.icon == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("附魔装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = this.icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GameInstance.Game.SenFuMoZhuangBei(goodsData.Id);
	}

	private void AddShuXingYueLan()
	{
		this.m_DicMax.Clear();
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		XElement gameResXml = Global.GetGameResXml("Config/EquipEnchantmentRandom.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "EquipEnchantmentRandom");
		for (int i = 0; i < xelementList.Count; i++)
		{
			stringBuilder.Append(Global.GetXElementAttributeStr(xelementList[i], "Name") + Global.GetLang("：") + Environment.NewLine);
			stringBuilder2.Append(Global.GetXElementAttributeStr(xelementList[i], "Shuxingyulan") + Environment.NewLine);
			string[] array = Global.GetXElementAttributeStr(xelementList[i], "Parameter").Split(new char[]
			{
				'|'
			});
			int[] array2 = new int[array.Length];
			int id = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(Global.GetXElementAttributeStr(xelementList[i], "Type")).ID;
			for (int j = 0; j < array2.Length; j++)
			{
				if (ConfigExtPropIndexes.GetPercentByID(id))
				{
					array2[j] = (int)(float.Parse(array[j].Split(new char[]
					{
						','
					})[0]) * 1000f);
				}
				else
				{
					array2[j] = array[j].Split(new char[]
					{
						','
					})[0].SafeToInt32(0) * 1000;
				}
			}
			if (this.m_DicMax.ContainsKey(id))
			{
				this.m_DicMax[id] = Mathf.Max(array2);
			}
			else
			{
				this.m_DicMax.Add(id, Mathf.Max(array2));
			}
		}
		this.m_LabYuLanName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(stringBuilder.ToString())
		});
		this.m_LabYuLanContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(stringBuilder2.ToString())
		});
		XElement gameResXml2 = Global.GetGameResXml("Config/EquipEnchantmentExtra.xml");
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "EquipEnchantmentExtra");
		for (int k = 0; k < xelementList2.Count; k++)
		{
			string[] array3 = Global.GetXElementAttributeStr(xelementList2[k], "Parameter").Split(new char[]
			{
				'|'
			});
			int[] array4 = new int[array3.Length];
			int id2 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(Global.GetXElementAttributeStr(xelementList2[k], "Type")).ID;
			for (int l = 0; l < array4.Length; l++)
			{
				if (ConfigExtPropIndexes.GetPercentByID(id2))
				{
					array4[l] = (int)(float.Parse(array3[l].Split(new char[]
					{
						','
					})[0]) * 1000f);
				}
				else
				{
					array4[l] = array3[l].Split(new char[]
					{
						','
					})[0].SafeToInt32(0) * 1000;
				}
			}
			if (this.m_DicMax.ContainsKey(id2))
			{
				this.m_DicMax[id2] = Mathf.Max(array4);
			}
			else
			{
				this.m_DicMax.Add(id2, Mathf.Max(array4));
			}
		}
	}

	[SerializeField]
	private GameObject m_GameGoodsParent;

	[SerializeField]
	private UILabel m_LabOld;

	[SerializeField]
	private UILabel m_LabNew;

	[SerializeField]
	private UILabel m_LabTitle;

	[SerializeField]
	private UILabel m_LabTitleYuLan;

	[SerializeField]
	private UILabel m_LabHuoBi;

	[SerializeField]
	private UILabel m_LabXiaoHaoNumber;

	[SerializeField]
	private UILabel m_LabXiaoHaoTitle;

	[SerializeField]
	private GButton m_BtnBaoCun;

	[SerializeField]
	private GButton m_BtnFuMo;

	[SerializeField]
	private ShowNetImage m_ShowLeft;

	[SerializeField]
	private ShowNetImage m_ShowRight;

	[SerializeField]
	private ShowNetImage m_ShowJianTou;

	[SerializeField]
	private UILabel m_LabYuLanName;

	[SerializeField]
	private UILabel m_LabYuLanContent;

	[SerializeField]
	private UIPanel m_PanelYuLanParent;

	[SerializeField]
	private GButton m_BtnYuLan;

	[SerializeField]
	private GButton m_BtnYuLanReturn;

	[SerializeField]
	private GameObject m_SpMax;

	[SerializeField]
	private GameObject m_SpRight;

	[SerializeField]
	private GameObject m_SpXian;

	[SerializeField]
	private GameObject m_SpXianRight;

	public LianluEffectEventHandler DPEffectItem;

	private GGoodIcon icon;

	private List<int> strsFuMo;

	public List<GameObject> m_ListLeftSpBack = new List<GameObject>();

	public List<GameObject> m_ListRightSpBack = new List<GameObject>();

	private Dictionary<int, int> m_DicMax = new Dictionary<int, int>();
}
