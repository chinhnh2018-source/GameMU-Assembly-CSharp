using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ChongShengEquipCuiLianPart : UserControl
{
	private List<Vector3> ListVecEquip
	{
		get
		{
			if (this.m_ListVecEquip.Count <= 0)
			{
				this.m_ListVecEquip.Add(new Vector3(-242f, 182f, 0f));
				this.m_ListVecEquip.Add(new Vector3(-284f, 83f, 0f));
				this.m_ListVecEquip.Add(new Vector3(-303f, -19f, 0f));
				this.m_ListVecEquip.Add(new Vector3(-284f, -123f, 0f));
				this.m_ListVecEquip.Add(new Vector3(-235f, -212f, 0f));
				this.m_ListVecEquip.Add(new Vector3(360f, 182f, 0f));
				this.m_ListVecEquip.Add(new Vector3(398f, 83f, 0f));
				this.m_ListVecEquip.Add(new Vector3(414f, -19f, 0f));
				this.m_ListVecEquip.Add(new Vector3(398f, -123f, 0f));
				this.m_ListVecEquip.Add(new Vector3(360f, -212f, 0f));
			}
			return this.m_ListVecEquip;
		}
	}

	protected override void InitializeComponent()
	{
		this.vecSpring = this.springTextContent.target;
		this.AddList();
		this.InitPrefabText();
		this.btnOnClick();
	}

	protected override void OnDestroy()
	{
	}

	private void InitPrefabText()
	{
		this.btnGuanZhu.Text = Global.GetLang("灌注");
		this.labXiaoHaoTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("灌注消耗：")
		});
		this.labMaxLevel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("当前已满级")
		});
	}

	private void btnOnClick()
	{
		this.btnCuiLian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendCuiLian();
		};
		this.btnGuanZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendGuanZhu();
		};
		this.btnAddEquip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_EquipIcon == null)
			{
				PlayZone.GlobalPlayZone.ShowGamePayerRoleWindow(GamePayerRolePart_PartID.GamePayerRolePart_BeiBaoChongSheng);
			}
		};
	}

	public void RedreshEquipKey()
	{
		this.SelectionChanged(this.m_OnKey);
	}

	private void AddList()
	{
		ObservableCollection itemsSource = this.listBox.ItemsSource;
		for (int i = 1; i < 11; i++)
		{
			ChongShengEquipCuiLianItem item = U3DUtils.NEW<ChongShengEquipCuiLianItem>();
			item.EquipCuiLianKey = (EquipCuiLian)i;
			item.transform.SetParent(this.listBox.transform, false);
			item.transform.localPosition = this.ListVecEquip[i - 1];
			this.m_ListEquip.Add(item);
			item.GetComponent<GButton>().MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.SelectionChanged(item.EquipCuiLianKey);
			};
		}
		this.SelectionChanged(EquipCuiLian.touKui);
	}

	private void SelectionChanged(EquipCuiLian equip)
	{
		this.m_OnKey = equip;
		ChongShengEquipCuiLianItem chongShengEquipCuiLianItem = null;
		for (int i = 0; i < this.m_ListEquip.Count; i++)
		{
			if (this.m_ListEquip[i].EquipCuiLianKey == equip)
			{
				chongShengEquipCuiLianItem = this.m_ListEquip[i];
				chongShengEquipCuiLianItem.OnClick = true;
			}
			if (this.m_OnKey > EquipCuiLian.none && this.m_OnKey != this.m_ListEquip[i].EquipCuiLianKey)
			{
				this.m_ListEquip[i].OnClick = false;
			}
		}
		if (chongShengEquipCuiLianItem == null)
		{
			return;
		}
		this.m_ChengGongLv = 0;
		RebornEquipData levelByEquipKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetLevelByEquipKey(this.m_OnKey, Global.Data.RoleID);
		int level = 0;
		if (levelByEquipKey != null)
		{
			level = levelByEquipKey.Level;
			this.m_ChengGongLv = levelByEquipKey.Able;
		}
		this.labChengGongLv.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(string.Format(Global.GetLang("成功率：{0}%"), this.m_ChengGongLv))
		});
		if (this.gameTeXiao.activeSelf)
		{
			this.gameTeXiao.GetComponentInChildren<Renderer>().material.SetFloat("_Height", (float)this.m_ChengGongLv / 100f);
		}
		EquipQuenchingVO equipQuenchingVODataByLevelAndKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipQuenchingVODataByLevelAndKey(this.m_OnKey, level);
		GoodsData gd = null;
		if (Global.Data.roleData.RebornGoodsDataList != null)
		{
			for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.RebornGoodsDataList[j].GoodsID);
				if (goodsXmlNodeByID.Categoriy >= 30 && goodsXmlNodeByID.Categoriy <= 38)
				{
					EquipCuiLianXmlData equipByCategoriy = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipByCategoriy((ItemCategories)goodsXmlNodeByID.Categoriy, (HandTypes)Global.Data.roleData.RebornGoodsDataList[j].BagIndex);
					if (equipByCategoriy != null && equipByCategoriy.CateCuiLian == chongShengEquipCuiLianItem.EquipCuiLianKey && Global.Data.roleData.RebornGoodsDataList[j].Using == 1)
					{
						gd = Global.Data.roleData.RebornGoodsDataList[j];
					}
				}
			}
		}
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("EquipQuenchingFreeNum") - Global.GetRoleOwnNumByMoneyType(161);
		if (num > 0)
		{
			this.labMianFeiNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("今日免费{0}次"), num))
			});
		}
		else
		{
			this.labMianFeiNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("今日免费{0}次"), 0))
			});
		}
		this.Refresh(equipQuenchingVODataByLevelAndKey, gd);
	}

	private void Refresh(EquipQuenchingVO xmlVO = null, GoodsData gd = null)
	{
		this.labContent.text = string.Empty;
		this.labContentNext.text = string.Empty;
		this.labContentOnoeOrMax.text = string.Empty;
		this.labContentOnoeOrMax.transform.localPosition = Vector3.back;
		this.springTextContent.target = this.vecSpring;
		this.springTextContent.enabled = true;
		if (this.m_EquipIcon != null)
		{
			Object.Destroy(this.m_EquipIcon.gameObject);
			this.m_EquipIcon = null;
		}
		if (gd == null)
		{
			this.btnCuiLian.isEnabled = false;
			this.btnGuanZhu.isEnabled = false;
			this.labChengGongLv.gameObject.SetActive(false);
			this.labXiaoHaoTitle.gameObject.SetActive(false);
			this.gameIconParent.gameObject.SetActive(false);
			this.btnAddEquip.gameObject.SetActive(true);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("您未佩戴装备,请将重生装备放入装备槽") + Environment.NewLine
			}));
			this.labContentOnoeOrMax.text = stringBuilder.ToString();
			this.btnCuiLian.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("淬炼")
			});
			this.labMaxLevel.gameObject.SetActive(false);
			return;
		}
		if (this.m_ChengGongLv < (int)(xmlVO.LelInterval * 100f))
		{
			this.btnCuiLian.isEnabled = false;
		}
		else
		{
			this.btnCuiLian.isEnabled = true;
		}
		this.btnGuanZhu.isEnabled = true;
		this.labChengGongLv.gameObject.SetActive(true);
		this.labXiaoHaoTitle.gameObject.SetActive(true);
		this.gameIconParent.gameObject.SetActive(true);
		this.btnAddEquip.gameObject.SetActive(false);
		if (gd != null)
		{
			this.m_EquipIcon = Global.CreateGoodsIcon(gd, false, true);
			this.m_EquipIcon.transform.SetParent(this.gameEquipParent.transform, false);
			this.m_EquipIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(this.m_EquipIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
		}
		string[] array = xmlVO.EverLelAttribute.Split(new char[]
		{
			'|'
		});
		string equipName = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipName((EquipCuiLian)xmlVO.PerfusionName);
		if (this.m_CaiLiaoIcon != null)
		{
			Object.Destroy(this.m_CaiLiaoIcon.gameObject);
			this.m_CaiLiaoIcon = null;
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		if (xmlVO.PerfusionLevel >= 0 && xmlVO.PerfusionLevel < IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetMaxLevelByEquipKey((EquipCuiLian)xmlVO.PerfusionName))
		{
			if (xmlVO.PerfusionLevel <= 0)
			{
				stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("装备灌注部位未激活")
				}));
				this.btnCuiLian.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("激活")
				});
				this.labContentOnoeOrMax.text = stringBuilder2.ToString();
			}
			else
			{
				this.btnCuiLian.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang("淬炼")
				});
				stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}{1}", Global.GetLang("当前等级："), xmlVO.PerfusionLevel) + Environment.NewLine
				}));
				stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}{1}", Global.GetLang("灌注部位："), equipName) + Environment.NewLine
				}));
				for (int i = 0; i < array.Length; i++)
				{
					string word = array[i].Split(new char[]
					{
						','
					})[0];
					ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(word);
					if (ConfigExtPropIndexes.GetPercentByWord(word))
					{
						stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord.Description, (float.Parse(array[i].Split(new char[]
							{
								','
							})[1]) * 100f).ToString("f2")) + Environment.NewLine)
						}));
					}
					else
					{
						stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord.Description, array[i].Split(new char[]
							{
								','
							})[1]) + Environment.NewLine)
						}));
					}
				}
				this.labContent.text = stringBuilder2.ToString();
			}
			if (!string.IsNullOrEmpty(xmlVO.LossItem) && !xmlVO.LossItem.Equals("-1"))
			{
				int goodsID = xmlVO.LossItem.Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				int num = xmlVO.LossItem.Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
				GoodsData CreatGd = Global.GetDummyGoodsData(goodsID);
				this.m_CaiLiaoIcon = Global.CreateGoodsIcon(CreatGd, false, true);
				this.m_CaiLiaoIcon.transform.SetParent(this.gameIconParent.transform, false);
				this.m_CaiLiaoIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					GTipServiceEx.ShowTip(this.m_CaiLiaoIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, CreatGd);
				};
				int num2 = this.GetGdNumber(goodsID);
				if (num2 > 9999)
				{
					num2 = 9999;
				}
				this.m_CaiLiaoIcon.SecondText.Label.supportEncoding = true;
				if (num2 >= num)
				{
					this.m_CaiLiaoIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}/{1}", num2, num)
					});
				}
				else
				{
					this.m_CaiLiaoIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", num2, num)
					});
				}
				this.m_CaiLiaoIcon.SecondText.Label.pivot = 3;
				this.m_CaiLiaoIcon.SecondText.Label.transform.localPosition = new Vector3(60f, -7f, -2f);
				this.m_CaiLiaoIcon.SecondText.Label.transform.localScale = new Vector3(25f, 25f, 1f);
			}
			this.labMaxLevel.gameObject.SetActive(false);
			this.labXiaoHaoTitle.gameObject.SetActive(true);
			this.labChengGongLv.gameObject.SetActive(true);
		}
		else if (xmlVO.PerfusionLevel >= IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetMaxLevelByEquipKey((EquipCuiLian)xmlVO.PerfusionName))
		{
			this.btnCuiLian.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("淬炼")
			});
			stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("{0}{1}", Global.GetLang("当前等级："), xmlVO.PerfusionLevel) + Environment.NewLine
			}));
			stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("{0}{1}", Global.GetLang("灌注部位："), equipName) + Environment.NewLine
			}));
			for (int j = 0; j < array.Length; j++)
			{
				string word2 = array[j].Split(new char[]
				{
					','
				})[0];
				ExtPropIndexesVO extPropIndexesVOByWord2 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(word2);
				if (ConfigExtPropIndexes.GetPercentByWord(word2))
				{
					stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord2.Description, (float.Parse(array[j].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f2")) + Environment.NewLine)
					}));
				}
				else
				{
					stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord2.Description, array[j].Split(new char[]
						{
							','
						})[1]) + Environment.NewLine)
					}));
				}
			}
			this.labContentOnoeOrMax.text = stringBuilder2.ToString();
			this.labContentOnoeOrMax.transform.localPosition = new Vector3(0f, 47f, -1f);
			this.labMaxLevel.gameObject.SetActive(true);
			this.labXiaoHaoTitle.gameObject.SetActive(false);
			this.labChengGongLv.gameObject.SetActive(false);
		}
		StringBuilder stringBuilder3 = new StringBuilder();
		if (xmlVO.PerfusionLevel > 0 && xmlVO.PerfusionLevel + 1 <= IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetMaxLevelByEquipKey((EquipCuiLian)xmlVO.PerfusionName))
		{
			EquipQuenchingVO equipQuenchingVODataByLevelAndKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipQuenchingVODataByLevelAndKey(this.m_OnKey, xmlVO.PerfusionLevel + 1);
			stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format("{0}{1}", Global.GetLang("下一等级："), equipQuenchingVODataByLevelAndKey.PerfusionLevel) + Environment.NewLine
			}));
			stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				string.Format("{0}{1}", Global.GetLang("灌注部位："), equipName) + Environment.NewLine
			}));
			string[] array2 = equipQuenchingVODataByLevelAndKey.EverLelAttribute.Split(new char[]
			{
				'|'
			});
			for (int k = 0; k < array2.Length; k++)
			{
				string word3 = array2[k].Split(new char[]
				{
					','
				})[0];
				ExtPropIndexesVO extPropIndexesVOByWord3 = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(word3);
				if (ConfigExtPropIndexes.GetPercentByWord(word3))
				{
					stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						Global.GetLang(string.Format("{0}:{1}%", extPropIndexesVOByWord3.Description, (float.Parse(array2[k].Split(new char[]
						{
							','
						})[1]) * 100f).ToString("f2")) + Environment.NewLine)
					}));
				}
				else
				{
					stringBuilder3.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						Global.GetLang(string.Format("{0}:{1}", extPropIndexesVOByWord3.Description, array2[k].Split(new char[]
						{
							','
						})[1]) + Environment.NewLine)
					}));
				}
			}
		}
		this.labContentNext.text = stringBuilder3.ToString();
	}

	private int GetGdNumber(int goodsID)
	{
		if (Global.Data.roleData.RebornGoodsDataList == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.RebornGoodsDataList[i].GoodsID == goodsID)
			{
				if (!Global.IsGoodsTimeOver(Global.Data.roleData.RebornGoodsDataList[i]))
				{
					num += Global.Data.roleData.RebornGoodsDataList[i].GCount;
				}
			}
		}
		return num;
	}

	private void SendGuanZhu()
	{
		if (!(this.m_EquipIcon.ItemObject is GoodsData))
		{
			return;
		}
		int num = 0;
		RebornEquipData levelByEquipKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetLevelByEquipKey(this.m_OnKey, Global.Data.RoleID);
		if (levelByEquipKey != null)
		{
			num = levelByEquipKey.Level;
		}
		if (num >= IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetMaxLevelByEquipKey(this.m_OnKey))
		{
			Super.HintMainText(Global.GetLang("当前装备淬炼已经达到满级"), 10, 3);
			return;
		}
		int num2 = (int)ConfigSystemParam.GetSystemParamIntByName("EquipQuenchingFreeNum");
		int num3 = num2 - Global.GetRoleOwnNumByMoneyType(161);
		if (num3 <= 0)
		{
			EquipQuenchingVO equipQuenchingVODataByLevelAndKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetEquipQuenchingVODataByLevelAndKey(this.m_OnKey, num);
			if (equipQuenchingVODataByLevelAndKey == null)
			{
				return;
			}
			int goodsID = equipQuenchingVODataByLevelAndKey.LossItem.Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			int gdNumber = this.GetGdNumber(goodsID);
			int num4 = equipQuenchingVODataByLevelAndKey.LossItem.Split(new char[]
			{
				','
			})[1].SafeToInt32(0);
			if (gdNumber < num4)
			{
				Super.ShowGoodsGuideForGoodsTips(goodsID, null);
				return;
			}
		}
		GameInstance.Game.SendGuanZhu((int)this.m_OnKey);
	}

	private void SendCuiLian()
	{
		if (!(this.m_EquipIcon.ItemObject is GoodsData))
		{
			return;
		}
		int num = 0;
		RebornEquipData levelByEquipKey = IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetLevelByEquipKey(this.m_OnKey, Global.Data.RoleID);
		if (levelByEquipKey != null)
		{
			num = levelByEquipKey.Level;
		}
		if (num >= IConfigbase<ConfigChongShengEquipCuiLian>.Instance.GetMaxLevelByEquipKey(this.m_OnKey))
		{
			Super.HintMainText(Global.GetLang("当前装备淬炼已经达到满级"), 10, 3);
			return;
		}
		GameInstance.Game.SendCuiLian((int)this.m_OnKey);
	}

	public void RefreshGuanZhu(int able)
	{
		if (Global.Data.roleData.RebornEquipHoleData == null)
		{
			Global.Data.roleData.RebornEquipHoleData = new Dictionary<int, RebornEquipData>();
		}
		if (Global.Data.roleData.RebornEquipHoleData.ContainsKey((int)this.m_OnKey))
		{
			Global.Data.roleData.RebornEquipHoleData[(int)this.m_OnKey].Able = able;
		}
		else
		{
			RebornEquipData rebornEquipData = new RebornEquipData();
			rebornEquipData.Able = able;
			rebornEquipData.HoleID = (int)this.m_OnKey;
			rebornEquipData.Level = 0;
			rebornEquipData.RoleID = Global.Data.RoleID;
			Global.Data.roleData.RebornEquipHoleData.Add((int)this.m_OnKey, rebornEquipData);
		}
		this.SelectionChanged(this.m_OnKey);
	}

	public void RefreshCuiLian(int able, int level)
	{
		if (Global.Data.roleData.RebornEquipHoleData.ContainsKey((int)this.m_OnKey))
		{
			Global.Data.roleData.RebornEquipHoleData[(int)this.m_OnKey].Level = level;
			Global.Data.roleData.RebornEquipHoleData[(int)this.m_OnKey].Able = able;
		}
		this.SelectionChanged(this.m_OnKey);
	}

	public UILabel labContent;

	public UILabel labContentNext;

	public UILabel labContentOnoeOrMax;

	public UILabel labXiaoHaoTitle;

	public UILabel labChengGongLv;

	public UILabel labMianFeiNumber;

	public UILabel labMaxLevel;

	public GButton btnGuanZhu;

	public GButton btnAddEquip;

	public GButton btnCuiLian;

	public ListBox listBox;

	public GameObject gameIconParent;

	public GameObject gameEquipParent;

	public GameObject gameTeXiao;

	private EquipCuiLian m_OnKey = EquipCuiLian.touKui;

	private GGoodIcon m_CaiLiaoIcon;

	private GGoodIcon m_EquipIcon;

	public SpringPanel springTextContent;

	private Vector3 vecSpring;

	private int m_ChengGongLv;

	private List<Vector3> m_ListVecEquip = new List<Vector3>();

	private List<ChongShengEquipCuiLianItem> m_ListEquip = new List<ChongShengEquipCuiLianItem>();
}
