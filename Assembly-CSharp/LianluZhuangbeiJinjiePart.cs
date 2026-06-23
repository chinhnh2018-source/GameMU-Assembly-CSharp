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

public class LianluZhuangbeiJinjiePart : UserControl
{
	public GCheckBox[] radioIcon
	{
		get
		{
			return this._radioIcon;
		}
		set
		{
			this._radioIcon = value;
		}
	}

	public GButton[] clearIcon
	{
		get
		{
			return this._clearIcon;
		}
		set
		{
			this._clearIcon = value;
		}
	}

	public SpriteSL[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("进阶");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartUp();
		};
		this.Clear0.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Clear1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(1);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Clear2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(2);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
		this.Clear3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(3);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = this.EquipWeiArr
			});
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[4];
		this.equipIcon[0] = this.Equip0;
		this.equipIcon[1] = this.Equip1;
		this.equipIcon[2] = this.Equip2;
		this.equipIcon[3] = this.Equip3;
		this.clearIcon = new GButton[4];
		this.clearIcon[0] = this.Clear0;
		this.clearIcon[1] = this.Clear1;
		this.clearIcon[2] = this.Clear2;
		this.clearIcon[3] = this.Clear3;
		this.InitAllValue();
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
			this.SetEquipWei(i, 0);
		}
		this.TongqianText.Text = "0";
		for (int j = 0; j < this.clearIcon.Length; j++)
		{
			NGUITools.SetActive(this.clearIcon[j].gameObject, false);
		}
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 3)
		{
			this.equipIcon[index].Clear();
			this.equipIcon[3].Clear();
			NGUITools.SetActive(this.clearIcon[index].gameObject, false);
			this.SetEquipWei(index, 0);
		}
		else if (index == 3)
		{
			this.equipIcon[3].Clear();
			for (int i = 0; i < 3; i++)
			{
				this.equipIcon[i].Clear();
				NGUITools.SetActive(this.clearIcon[i].gameObject, false);
				this.SetEquipWei(i, 0);
			}
			NGUITools.SetActive(this.clearIcon[3].gameObject, false);
		}
		if (Array.TrueForAll<int>(this.EquipWeiArr, (int e) => e <= 0))
		{
			this.InitAllValue();
		}
	}

	private void SetXiaohao(XElement xml, GoodsData goodsData)
	{
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		this.NeedTongqian = Global.GetXElementAttributeInt(xml, "NeedMoJing");
		this.NeedTongqian *= 1 + (int)Math.Ceiling((double)zhuoyueAttributeCount / 2.0);
		this.TongqianText.Text = this.NeedTongqian.ToString();
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < this.NeedTongqian)
		{
			this.TongqianText.textColor = 16711680U;
		}
		else
		{
			this.TongqianText.textColor = 16777215U;
		}
	}

	public int SearchItem()
	{
		if (this.EquipWeiArr != null)
		{
			for (int i = 0; i < this.EquipWeiArr.Length; i++)
			{
				if (this.EquipWeiArr[i] > 0)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public int GetEquipKongwei(int value = 0)
	{
		return this.EquipWeiArr.IndexOf(value);
	}

	public bool TrueForAll(int value)
	{
		return Array.TrueForAll<int>(this.EquipWeiArr, (int e) => e == value);
	}

	public void SetEquipWei(int index, int value)
	{
		if (index < 0 || index >= this.EquipWeiArr.Length)
		{
			return;
		}
		this.EquipWeiArr[index] = value;
	}

	public int IsEquipOK(bool isReal = false)
	{
		if (this.equipIcon[0].Length() > 0 && this.equipIcon[1].Length() > 0 && this.equipIcon[2].Length() > 0)
		{
			if (!isReal)
			{
				return 1;
			}
			if (this.GetEquipKongwei(-1) != -1)
			{
				return -1;
			}
			return 1;
		}
		else
		{
			if (this.equipIcon[0].Length() <= 0 && this.equipIcon[1].Length() <= 0 && this.equipIcon[2].Length() <= 0)
			{
				return 0;
			}
			return -1;
		}
	}

	public int IsBindOfNextEquip()
	{
		for (int i = 0; i < this.equipIcon.Length - 1; i++)
		{
			if (this.equipIcon[i].Length() > 0 && (U3DUtils.AS<GGoodIcon>(this.equipIcon[i][0]).ItemObject as GoodsData).Binding == 1)
			{
				return 1;
			}
		}
		return 0;
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			int num = this.IsEquipOK(false);
			if (num == 1)
			{
				return;
			}
			if (num == 0)
			{
			}
			int equipKongwei = this.GetEquipKongwei(0);
			if (equipKongwei != -1)
			{
				this.AddEquipGoodsIcon(gd, equipKongwei, false, 0);
				this.SetEquipWei(equipKongwei, gd.Id);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					EquipIDs = this.EquipWeiArr
				});
			}
			if (this.IsEquipOK(false) == 1)
			{
				GoodsData goodsData = (this.equipIcon[0].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
				GoodsData gd2 = (this.equipIcon[1].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
				GoodsData gd3 = (this.equipIcon[2].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[2][0]).ItemObject as GoodsData);
				if (goodsData != null)
				{
					GoodsData nextQualityEquipGoodsData = this.GetNextQualityEquipGoodsData(goodsData, this.IsBindOfNextEquip(), gd2, gd3);
					if (nextQualityEquipGoodsData != null)
					{
						this.AddEquipGoodsIcon(nextQualityEquipGoodsData, 3, false, 0);
					}
				}
			}
		}
	}

	public void AddEquipAuto(GoodsData gd)
	{
		if (gd != null)
		{
			int num = this.IsEquipOK(false);
			if (num == 1)
			{
				return;
			}
			if (num == 0)
			{
				bool flag = true;
				for (int i = 0; i < this.EquipWeiArr.Length; i++)
				{
					int equipKongwei = this.GetEquipKongwei(0);
					if (equipKongwei != -1)
					{
						if (flag)
						{
							this.AddEquipGoodsIcon(gd, equipKongwei, false, 0);
							this.SetEquipWei(equipKongwei, gd.Id);
							flag = false;
						}
						else
						{
							GoodsData goodsData = this.GetGoodsDataByID(gd, this.EquipWeiArr, 0);
							if (goodsData == null)
							{
								goodsData = Global.GetDummyGoodsDataMu(gd.GoodsID, 0, 0, gd.ExcellenceInfo, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
								this.AddEquipGoodsIcon(goodsData, equipKongwei, true, 0);
								this.SetEquipWei(equipKongwei, goodsData.Id);
							}
							else
							{
								this.AddEquipGoodsIcon(goodsData, equipKongwei, false, 0);
								this.SetEquipWei(equipKongwei, goodsData.Id);
							}
						}
					}
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0,
					EquipIDs = this.EquipWeiArr
				});
				num = this.IsEquipOK(true);
				if (num == -1 || num == 1)
				{
					GoodsData goodsData2 = (this.equipIcon[0].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
					GoodsData gd2 = (this.equipIcon[1].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
					GoodsData gd3 = (this.equipIcon[2].Length() <= 0) ? null : (U3DUtils.AS<GGoodIcon>(this.equipIcon[2][0]).ItemObject as GoodsData);
					if (goodsData2 != null)
					{
						GoodsData nextQualityEquipGoodsData = this.GetNextQualityEquipGoodsData(goodsData2, this.IsBindOfNextEquip(), gd2, gd3);
						if (nextQualityEquipGoodsData != null)
						{
							this.AddEquipGoodsIcon(nextQualityEquipGoodsData, 3, num == -1, 0);
						}
					}
				}
			}
		}
	}

	private GoodsData GetNextQualityEquipGoodsData(GoodsData goodsData, int isBind, GoodsData gd1, GoodsData gd2)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		int num = goodsXmlNodeByID.Categoriy;
		int suitID = goodsXmlNodeByID.SuitID;
		if ((num >= 11 && num <= 19) || num == 21)
		{
			num = 10000;
		}
		XElement equipUpXmlNode = Global.GetEquipUpXmlNode(num, suitID + 1);
		if (equipUpXmlNode == null)
		{
			return null;
		}
		this.SetXiaohao(equipUpXmlNode, goodsData);
		goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsXmlNodeByID.JinJie);
		if (equipUpXmlNode == null)
		{
			return null;
		}
		int id = goodsXmlNodeByID.ID;
		GoodsData goodsData2 = new GoodsData();
		goodsData2.BagIndex = goodsData.BagIndex;
		goodsData2.Binding = isBind;
		goodsData2.Endtime = goodsData.Endtime;
		goodsData2.Forge_level = goodsData.Forge_level;
		goodsData2.AppendPropLev = goodsData.AppendPropLev;
		goodsData2.GCount = goodsData.GCount;
		goodsData2.GoodsID = id;
		goodsData2.Id = 10000;
		goodsData2.Jewellist = goodsData.Jewellist;
		goodsData2.Props = goodsData.Props;
		goodsData2.Quality = goodsData.Quality;
		goodsData2.SaleMoney1 = goodsData.SaleMoney1;
		goodsData2.SaleYinPiao = goodsData.SaleYinPiao;
		goodsData2.SaleYuanBao = goodsData.SaleYuanBao;
		goodsData2.Site = goodsData.Site;
		goodsData2.Starttime = goodsData.Starttime;
		goodsData2.Using = goodsData.Using;
		goodsData2.AddPropIndex = goodsData.AddPropIndex;
		goodsData2.BornIndex = goodsData.BornIndex;
		goodsData2.Lucky = goodsData.Lucky;
		goodsData2.ExcellenceInfo = goodsData.ExcellenceInfo;
		goodsData2.Strong = goodsData.Strong;
		goodsData2.WashProps = goodsData.WashProps;
		goodsData2.ElementhrtsProps = goodsData.ElementhrtsProps;
		if (gd1 != null)
		{
			goodsData2.Forge_level = ((gd1.Forge_level <= goodsData2.Forge_level) ? goodsData2.Forge_level : gd1.Forge_level);
			goodsData2.AppendPropLev = ((gd1.AppendPropLev <= goodsData2.AppendPropLev) ? goodsData2.AppendPropLev : gd1.AppendPropLev);
			if (goodsData2.Lucky <= 0)
			{
				goodsData2.Lucky = gd1.Lucky;
			}
		}
		if (gd2 != null)
		{
			goodsData2.Forge_level = ((gd2.Forge_level <= goodsData2.Forge_level) ? goodsData2.Forge_level : gd2.Forge_level);
			goodsData2.AppendPropLev = ((gd2.AppendPropLev <= goodsData2.AppendPropLev) ? goodsData2.AppendPropLev : gd2.AppendPropLev);
			if (goodsData2.Lucky <= 0)
			{
				goodsData2.Lucky = gd2.Lucky;
			}
		}
		return goodsData2;
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = 12;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (gd.Id != -1)
				{
					GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
				}
			};
			if (grayShow)
			{
				icon.GoodImg.ToGrayBitmap = true;
			}
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			bool flag = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, flag, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(icon);
			if (index >= 0 && index < 3)
			{
				if (gd.Id > 0)
				{
					NGUITools.SetActive(this.clearIcon[index].gameObject, true);
				}
				else
				{
					icon.BackgroundSprite1Visible = false;
					icon.NoUseSpriteVisible = false;
				}
			}
			else if (index == 3)
			{
				icon.NoUseSpriteVisible = (!grayShow && !flag);
				NGUITools.SetActive(this.clearIcon[index].gameObject, true);
			}
		}
	}

	public void StartUp()
	{
		int equipKongwei = this.GetEquipKongwei(0);
		if (equipKongwei != -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("需要放入三件相同名称、相同品质的装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		List<int> list = new List<int>();
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("主装不在背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		list.Add(Global.GetZhuoyueAttributeCount(goodsData));
		int id = goodsData.Id;
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("辅装不在背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		list.Add(Global.GetZhuoyueAttributeCount(goodsData));
		int id2 = goodsData.Id;
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[2][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("辅装不在背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		list.Add(Global.GetZhuoyueAttributeCount(goodsData));
		int id3 = goodsData.Id;
		if (this.equipIcon[3].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备已经进阶到了最高级"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < this.NeedTongqian)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMojing, this.callback, string.Empty, string.Empty);
			return;
		}
		this.ExecuteJinjie(id, id2, id3);
	}

	private void ExecuteJinjie(int id1, int id2, int id3)
	{
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(id1, null);
		GoodsData goodsDataByDbID2 = Global.GetGoodsDataByDbID(id2, null);
		GoodsData goodsDataByDbID3 = Global.GetGoodsDataByDbID(id3, null);
		bool flag = false;
		string[] str = null;
		if (goodsDataByDbID == null || goodsDataByDbID2 == null || goodsDataByDbID3 == null)
		{
			flag = true;
		}
		else
		{
			string[] array = new string[]
			{
				this.GetZhuoyueAttributeStr(goodsDataByDbID),
				this.GetZhuoyueAttributeStr(goodsDataByDbID2),
				this.GetZhuoyueAttributeStr(goodsDataByDbID3)
			};
			int num = 0;
			byte b = 0;
			while ((int)b < array.Length)
			{
				if (string.IsNullOrEmpty(array[(int)b]))
				{
					num++;
				}
				b += 1;
			}
			if (num == 3)
			{
				flag = true;
			}
			str = array;
		}
		if (flag)
		{
			Super.ShowMessageBox(Global.GetLang("提示"), string.Format(Global.GetLang("确定要将放入的三件装备进阶为一件更高级的装备?{0}装备成功进阶后如有多件带有附魔属性，将会随机一件装备的随机附魔属性"), Environment.NewLine), 1, delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					this.DPEffectItem(this, new NotifyLianluEffectEventArgs
					{
						EffectID = 5
					});
					this.ShowModalDialog();
					GameInstance.Game.SpriteMuEquipUpgradeCmd(id1, id2, id3, 0);
				}
			}, MessBoxIsHintTypes.None);
		}
		else
		{
			this.ShowPropChosePart(str, id1, id2, id3);
		}
	}

	private void ShowPropChosePart(string[] str, int id1, int id2, int id3)
	{
		if (null == this.m_PropChosePartWind)
		{
			this.m_PropChosePartWind = U3DUtils.NEW<GChildWindow>();
			this.m_PropChosePartWind.ModalType = ChildWindowModalType.Translucent2;
			this.Container.Add(this.m_PropChosePartWind);
			this.m_PropChosePartWind.transform.localPosition = new Vector3(0f, 0f, -4f);
		}
		if (null != this.m_PropChosePart)
		{
			Object.Destroy(this.m_PropChosePart.gameObject);
		}
		this.m_PropChosePart = U3DUtils.NEW<PropChosePart>();
		this.m_PropChosePart.SetInf(str);
		this.m_PropChosePart.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.IDType == 1)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 5
				});
				this.ShowModalDialog();
				if (s.ID == 0)
				{
					GameInstance.Game.SpriteMuEquipUpgradeCmd(id1, id2, id3, id1);
				}
				else if (s.ID == 1)
				{
					GameInstance.Game.SpriteMuEquipUpgradeCmd(id2, id1, id3, id2);
				}
				else if (s.ID == 2)
				{
					GameInstance.Game.SpriteMuEquipUpgradeCmd(id3, id2, id1, id3);
				}
			}
			this.ClosePropChosePart();
		};
		this.m_PropChosePartWind.Body.Add(this.m_PropChosePart);
	}

	private void ClosePropChosePart()
	{
		Object.Destroy(this.m_PropChosePartWind.gameObject);
		Object.Destroy(this.m_PropChosePart.gameObject);
	}

	private string GetZhuoyueAttributeStr(GoodsData gd)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		int num = 32;
		for (int i = 0; i < num; i++)
		{
			if (Global.GetIntSomeBit(gd.ExcellenceInfo, i) == 1)
			{
				text2 = this.GetZhuoyuePropStr(i);
				if (!string.IsNullOrEmpty(text2))
				{
					text += text2;
					text += "\n";
				}
			}
		}
		if (gd.ElementhrtsProps != null && gd.ElementhrtsProps.Count > 0 && gd.ElementhrtsProps[1] > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("          装备附魔属性")
			}) + Environment.NewLine);
			for (int j = 0; j < gd.ElementhrtsProps.Count; j += 2)
			{
				if (!ConfigExtPropIndexes.GetPercentByID(gd.ElementhrtsProps[j]))
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(gd.ElementhrtsProps[j], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						gd.ElementhrtsProps[j + 1] / 1000
					}));
				}
				else
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(gd.ElementhrtsProps[j], false) + ":"
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						gd.ElementhrtsProps[j + 1] / 10 + "%"
					}));
				}
				if (j < gd.ElementhrtsProps.Count - 2)
				{
					stringBuilder.Append(Environment.NewLine);
				}
			}
			text += stringBuilder.ToString();
		}
		return this.ProcessStr(text);
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	public string GetZhuoyuePropStr(int flag)
	{
		string result = string.Empty;
		string text = string.Empty;
		string text2 = ZhuoyuePropIndexes.ZhuoyuePropIndexChineseNames[flag];
		if (flag == 1 || flag == 2 || flag == 9)
		{
			text = string.Format(Global.GetLang("人物等级/{0}"), ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag]);
		}
		else
		{
			text = ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag].ToString();
		}
		if (ZhuoyuePropIndexes.ZhuoyuePropIndexPercents[flag] == 1)
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("{0}:", text2)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(" +{0}%", text)
			});
		}
		else
		{
			result = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("{0}:", text2)
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format(" +{0}", text)
			});
		}
		return result;
	}

	public void NotifyResult(int dbID)
	{
		this.CloseModalDialog();
		if (dbID < 0)
		{
			if (dbID == -1000 || dbID == -5555)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("进阶失败！"), new object[0]), 0, -1, -1, 0);
			}
			else if (dbID == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在背包中，无法进阶"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备进阶时发生错误:{0}"), new object[]
				{
					dbID
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				Flag = 0
			});
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
			return;
		}
		this.InitAllValue();
		Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		this.DPEffectItem(this, new NotifyLianluEffectEventArgs
		{
			EffectID = 1
		});
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备进阶成功!"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1,
				Flag = 1
			});
		}
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		if (goodsDataByDbID == null)
		{
			this.InitAllValue();
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				EquipIDs = null
			});
			return;
		}
		this.AddEquipGoodsIcon(goodsDataByDbID, 3, false, 0);
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public GoodsData GetGoodsDataByID(GoodsData goodsData, int[] exceptDBIDs, int isUseing = 0)
	{
		if (Global.Data.roleData.GoodsDataList == null)
		{
			return null;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData2 = Global.Data.roleData.GoodsDataList[i];
			if (goodsData2.Using == isUseing)
			{
				if (goodsData2.GoodsID == goodsData.GoodsID)
				{
					if (exceptDBIDs == null || exceptDBIDs.IndexOf(goodsData2.Id) == -1)
					{
						if (Global.GetColorByGoodsData(goodsData) == Global.GetColorByGoodsData(goodsData2))
						{
							return goodsData2;
						}
					}
				}
			}
		}
		return null;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public GButton SubmitBtn;

	public TextBlock TongqianText;

	public SpriteSL Equip0;

	public SpriteSL Equip1;

	public SpriteSL Equip2;

	public SpriteSL Equip3;

	public GButton Clear0;

	public GButton Clear1;

	public GButton Clear2;

	public GButton Clear3;

	private int[] EquipWeiArr = new int[3];

	private int NeedTongqian;

	private GCheckBox[] _radioIcon;

	private GButton[] _clearIcon;

	private SpriteSL[] _equipIcon;

	private PropChosePart m_PropChosePart;

	private GChildWindow m_PropChosePartWind;
}
