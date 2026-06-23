using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LianLuJuHunPart : UserControl
{
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
		this.m_CheckBox.Text = Global.GetLang("始终使用\n神佑晶石");
		this.m_BindCheckBox.Text = Global.GetLang("优先使用绑定材料");
		this.m_JuHunBtn.Text = Global.GetLang("聚魂");
		this.m_JuHunDescribe.Text = Global.GetLang("11阶以上装备可以进行聚魂\n聚魂可对装备强化、追加、培养属性进行加成");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnClickEvent();
	}

	private void InitBtnClickEvent()
	{
		this.m_ClearEquipBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.InitAllValue();
			this.ClearTeXiao();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
			if (this.ShowPropertyCallBack != null)
			{
				this.ShowPropertyCallBack(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.m_JuHunBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartJuHun();
		};
		this.m_CheckBox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			if (!this.m_CheckBox.isChecked)
			{
				Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("不使用神佑晶石，聚魂失败等级降1级，确定不使用神佑晶石？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
			}
		};
		this.m_BindCheckBox.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.m_CaiLiaos.Clear();
			for (int i = 1; i < this.equipIcon.Length; i++)
			{
				this.equipIcon[i].Clear();
			}
			JuHunData juHunDataById = ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(this.m_JuHunID));
			int type = juHunDataById.Type;
			int level = juHunDataById.Level;
			this.ShowCaiLiao(type, level);
			this.IsShowShenYouShi(type, level);
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[5];
		this.equipIcon[0] = this.EquipNow;
		this.equipIcon[1] = this.Cailiao1;
		this.equipIcon[2] = this.Cailiao2;
		this.equipIcon[3] = this.Cailiao3;
		this.equipIcon[4] = this.Cailiao4;
		this.m_CaiLiaos = new List<LianLuJuHunPart.CaiLiaoStruct>();
		this.juHunGradeNames = ParseJuHunConfig.GetJuHunTypes();
		this.juHunDatas = ParseJuHunConfig.GetJuHunDatas();
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.m_TongQian.Text = "0";
		this.m_ChengGongLv.Text = "0%";
		this.IsShowClearEquipBtn(false);
		this.IsShowCheckBox(false);
		this.IsShowBindCheckBox(false);
		this.IsShowJuHunDescribe(true);
		this.UnebableJuHunBtn();
		this.m_CaiLiaos.Clear();
		this.NeedMoney = 0;
		this.isBindEquip = false;
		this.goodsId = 0;
	}

	private void ClearCurrentEquip()
	{
		if (this.EquipNow.Count() > 0)
		{
			this.EquipNow.Clear();
		}
	}

	private void StartJuHun()
	{
		bool flag = false;
		bool flag2 = false;
		int count = this.m_CaiLiaos.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				if (!this.m_CaiLiaos[i].IsEnough)
				{
					if (this.m_CaiLiaos[i].Index != 4)
					{
						if (Super.ShowGoodsGuide(this.m_CaiLiaos[i].ID, this.callback) == 1)
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法聚魂"), new object[]
							{
								Global.GetGoodsNameByID(this.m_CaiLiaos[i].ID, false)
							}), 19, -1, -1, this.m_CaiLiaos[i].ID);
						}
						flag = true;
						break;
					}
					if (this.m_CheckBox.isChecked)
					{
						if (Super.ShowGoodsGuide(this.m_CaiLiaos[i].ID, this.callback) == 1)
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法聚魂"), new object[]
							{
								Global.GetGoodsNameByID(this.m_CaiLiaos[i].ID, false)
							}), 19, -1, -1, this.m_CaiLiaos[i].ID);
						}
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			return;
		}
		if (this.NeedMoney > 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				this.m_TongQian.textColor = 16711680U;
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
			this.m_TongQian.textColor = 16777215U;
		}
		if (!this.isBindEquip && this.m_BindCheckBox.isChecked)
		{
			for (int j = 0; j < count; j++)
			{
				if (this.m_CaiLiaos[j].IsBind)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				string lang = Global.GetLang("存在绑定的材料，操作后您的装备将变为绑定，确认要执行该操作吗?");
				Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
				{
					if (e1.ID == 0)
					{
						this.ExecuteJuHunEquip();
					}
				}, buttons);
				return;
			}
		}
		this.ExecuteJuHunEquip();
	}

	private void ExecuteJuHunEquip()
	{
		Super.ShowNetWaiting(null);
		this.useShenShi = ((!this.m_CheckBox.isChecked || !this.m_IsUseShenShi) ? 0 : 1);
		this.useBindCaiLiao = ((!this.m_BindCheckBox.isChecked) ? 0 : 1);
		GameInstance.Game.SendJuHunToServer(this.goodsId, this.useShenShi, this.useBindCaiLiao);
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			this.m_CheckBox.isChecked = false;
		}
		else if (args.ID == 1)
		{
			this.m_CheckBox.isChecked = true;
		}
	}

	public void NotifyJuHunResult(int result, int equipId, int juHunId, int binding, bool isSuccess)
	{
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(equipId, null);
		this.AddEquip(goodsDataByDbID, true);
		if (juHunId % 10 == 1 && isSuccess)
		{
			this.ClearTeXiao();
		}
		if (juHunId == 0 || juHunId % 10 != 0 || !isSuccess)
		{
		}
		if (isSuccess)
		{
			this.AddSingleHunHuoTeXiao(ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(juHunId)).Type, ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(juHunId)).Level);
		}
		else if (juHunId > 0)
		{
			this.DeleteTeXiaoByLevel(ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(juHunId)).Level - 1);
		}
		else if (juHunId == 0)
		{
			this.ClearTeXiao();
		}
	}

	private void UnebableJuHunBtn()
	{
		if (this.EquipNow.Count() == 0)
		{
			this.m_JuHunBtn.isEnabled = false;
		}
	}

	private void EnableJuHunBtn()
	{
		if (this.EquipNow.Count() >= 0)
		{
			this.m_JuHunBtn.isEnabled = true;
		}
	}

	private void IsShowClearEquipBtn(bool isShow)
	{
		NGUITools.SetActive(this.m_ClearEquipBtn.gameObject, isShow);
	}

	private void IsShowJuHunDescribe(bool isShow)
	{
		NGUITools.SetActive(this.m_JuHunDescribe.gameObject, isShow);
	}

	private void IsShowCheckBox(bool isShow)
	{
		NGUITools.SetActive(this.m_CheckBox.gameObject, isShow);
	}

	private void IsShowBindCheckBox(bool isShow)
	{
		NGUITools.SetActive(this.m_BindCheckBox.gameObject, isShow);
	}

	public void AddEquip(GoodsData gd, bool result = true)
	{
		if (gd != null)
		{
			this.InitAllValue();
			if (gd.JuHunID < 0)
			{
				gd.JuHunID = 0;
			}
			this.m_JuHunID = this.GetJuHunID(gd.JuHunID + 1);
			int type = ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(this.m_JuHunID)).Type;
			int level = ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(this.m_JuHunID)).Level;
			this.isBindEquip = (gd.Binding == 1);
			this.goodsId = gd.Id;
			this.AddEquipIcon(gd, 0, false, 0);
			this.IsShowClearEquipBtn(true);
			this.EnableJuHunBtn();
			this.IsShowJuHunDescribe(false);
			this.IsShowBindCheckBox(true);
			this.ShowCaiLiao(type, level);
			this.IsShowShenYouShi(type, level);
			this.ShowBangJinAndChengGongLv(type, level);
			this.ShowJuHunProperty(this.m_JuHunID, gd);
		}
	}

	public void InitHunHuoTeXiao(GoodsData gd)
	{
		if (gd.JuHunID != 0)
		{
			this.ClearTeXiao();
			this.ShowHunHuo(ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(gd.JuHunID)).Type, ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(gd.JuHunID)).Level);
		}
	}

	private void ShowHunHuo(int type, int level)
	{
		if (type == 0 || level == 0)
		{
			return;
		}
		this.teXiaoPath = this.teXiaoForePath + this.GetTeXiaoName(type);
		for (int i = 0; i < level; i++)
		{
			this.LoadHunHuoPrefab(this.hunHuos[i].gameObject, this.hunHuoJiHuo);
			this.LoadHunHuoPrefab(this.hunHuos[i].gameObject, this.teXiaoPath);
		}
	}

	private void AddSingleHunHuoTeXiao(int type, int level)
	{
		if (type == 0 || level == 0)
		{
			return;
		}
		this.teXiaoPath = this.teXiaoForePath + this.GetTeXiaoName(type);
		this.LoadHunHuoPrefab(this.hunHuos[level - 1].gameObject, this.hunHuoJiHuo);
		this.LoadHunHuoPrefab(this.hunHuos[level - 1].gameObject, this.teXiaoPath);
	}

	private string GetTeXiaoName(int type)
	{
		string result;
		switch (type)
		{
		case 1:
		case 2:
			result = "huohuo_bai";
			break;
		case 3:
		case 4:
			result = "huohuo_lv";
			break;
		case 5:
		case 6:
			result = "huohuo_lan";
			break;
		case 7:
		case 8:
			result = "huohuo_zi";
			break;
		case 9:
		case 10:
			result = "huohuo_hong";
			break;
		default:
			result = "huohuo_bai";
			break;
		}
		return result;
	}

	private void LoadHunHuoPrefab(GameObject parent, string path)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load(path)) as GameObject;
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
	}

	private void ClearTeXiao()
	{
		for (int i = 0; i < this.hunHuos.Length; i++)
		{
			Transform transform = this.hunHuos[i].transform;
			int childCount = transform.childCount;
			if (childCount > 0)
			{
				for (int j = 0; j < childCount; j++)
				{
					Object.Destroy(transform.GetChild(j).gameObject);
				}
			}
		}
	}

	private void DeleteTeXiaoByLevel(int level)
	{
		for (int i = 0; i < this.hunHuos.Length; i++)
		{
			if (i > level)
			{
				Transform transform = this.hunHuos[i].transform;
				int childCount = transform.childCount;
				if (childCount > 0)
				{
					for (int j = 0; j < childCount; j++)
					{
						Object.Destroy(transform.GetChild(j).gameObject);
					}
				}
			}
		}
	}

	private void AddAllTeXiao(int juHunId)
	{
		for (int i = 0; i < this.hunHuos.Length; i++)
		{
			Transform transform = this.hunHuos[i].transform;
			this.teXiaoPath = this.teXiaoForePath + this.GetTeXiaoName(ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(juHunId)).Type);
			this.LoadHunHuoPrefab(this.hunHuos[i].gameObject, this.hunHuoJiHuo);
			this.LoadHunHuoPrefab(this.hunHuos[i].gameObject, this.teXiaoPath);
		}
	}

	private void IsShowShenYouShi(int type, int level)
	{
		JuHunData juHunDataByTypeAndLevel = ParseJuHunConfig.GetJuHunDataByTypeAndLevel(type, level);
		if (juHunDataByTypeAndLevel.shenShi != null)
		{
			this.IsShowCheckBox(true);
			string[] array = juHunDataByTypeAndLevel.shenShi.Split(new char[]
			{
				','
			});
			int caiLiaoID = int.Parse(array[0]);
			int iNeedNub = int.Parse(array[1]);
			this.AddCaiLiaoIcon(caiLiaoID, 4, iNeedNub, this.m_BindCheckBox.isChecked);
			this.m_IsUseShenShi = true;
		}
		else
		{
			this.m_IsUseShenShi = false;
			this.IsShowCheckBox(false);
		}
	}

	private void ShowCaiLiao(int type, int level)
	{
		JuHunData juHunDataByTypeAndLevel = ParseJuHunConfig.GetJuHunDataByTypeAndLevel(type, level);
		List<CaiLiaoData> caiLiaos = juHunDataByTypeAndLevel.caiLiaos;
		for (int i = 0; i < caiLiaos.Count; i++)
		{
			this.AddCaiLiaoIcon(caiLiaos[i].ID, i + 1, caiLiaos[i].Count, this.m_BindCheckBox.isChecked);
		}
	}

	private void ShowBangJinAndChengGongLv(int type, int level)
	{
		JuHunData juHunDataByTypeAndLevel = ParseJuHunConfig.GetJuHunDataByTypeAndLevel(type, level);
		this.NeedMoney = juHunDataByTypeAndLevel.CostBandJinBi;
		if (this.NeedMoney > 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				this.m_TongQian.textColor = 16711680U;
			}
			else
			{
				this.m_TongQian.textColor = 16777215U;
			}
		}
		this.m_TongQian.Text = juHunDataByTypeAndLevel.CostBandJinBi.ToString();
		this.m_ChengGongLv.Text = string.Format("{0}%", juHunDataByTypeAndLevel.SuccessProportion * 100f);
	}

	private void ShowJuHunProperty(int juHunID, GoodsData gd)
	{
		if (this.ShowPropertyCallBack != null)
		{
			this.ShowPropertyCallBack(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = juHunID,
				Data = gd,
				Title = Global.GetLang(this.juHunGradeNames[ParseJuHunConfig.GetJuHunDataById(juHunID).Type - 1])
			});
		}
	}

	private void AddEquipIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
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
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			if (index == 0)
			{
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(icon);
		}
	}

	public void AddCaiLiaoIcon(int caiLiaoID, int index, int iNeedNub, bool isBind)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(caiLiaoID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid3_bak";
			GoodsData gd = Global.GetIsBindGoodsDataByID(caiLiaoID, this.m_BindCheckBox.Check);
			if (gd == null)
			{
				gd = Global.GetDummyGoodsData(caiLiaoID);
			}
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.isAutoSize = true;
			icon.BackSpriteName0 = backSpriteName;
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				caiLiaoID,
				0,
				-1,
				-1
			});
			icon.ItemNum = iNeedNub;
			icon.ItemCode = caiLiaoID;
			icon.ItemObject = gd;
			icon.BoxTypes = 5;
			icon.Text = iNeedNub.ToString();
			icon.TextShadowColor = 4278190080U;
			icon.TextColor = 16777215U;
			icon.DisableTextColor = 8421504U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			icon.STextVisibility = false;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd = (icon.ItemObject as GoodsData);
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			int num = 0;
			this.GetEquipInfo(out num);
			int num2 = 0;
			bool flag = false;
			int num3 = Global.GetTotalGoodsCountByID(caiLiaoID);
			num2 += ConfigReplaceGoodVO.GetReplaceGoodCount(caiLiaoID, "JuHun", ref flag, (long)num);
			LianLuJuHunPart.CaiLiaoStruct caiLiaoStruct = default(LianLuJuHunPart.CaiLiaoStruct);
			caiLiaoStruct.ID = caiLiaoID;
			caiLiaoStruct.Index = index;
			caiLiaoStruct.IsBind = (flag || gd.Binding == 1);
			num3 += num2;
			icon.Text = string.Format("{0}/{1}", num3, iNeedNub);
			if (num3 >= iNeedNub)
			{
				icon.EnableIcon = true;
				icon.TextColor = 16777215U;
				caiLiaoStruct.IsEnough = true;
				caiLiaoStruct.Count = num3;
			}
			else
			{
				icon.EnableIcon = false;
				icon.TextColor = 16711680U;
				caiLiaoStruct.IsEnough = false;
			}
			icon.TextShadowColor = 4278190080U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			this.m_CaiLiaos.Add(caiLiaoStruct);
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(icon);
		}
	}

	private void GetEquipInfo(out int juHunType)
	{
		juHunType = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			return;
		}
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(this.goodsId, null);
		juHunType = ParseJuHunConfig.GetJuHunDataById(this.GetJuHunID(this.m_JuHunID)).Type;
	}

	private bool HasBindCaiLiaoCount(int caiLiaoId)
	{
		return Global.GetTotalBindingGoodsCountByID(caiLiaoId) > 0;
	}

	private int GetBindCaiLiaoCount(int caiLiaoId)
	{
		return Global.GetTotalBindingGoodsCountByID(caiLiaoId);
	}

	private bool HasNotBindCaiLiaoCount(int caiLiaoId)
	{
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(caiLiaoId);
		int totalBindingGoodsCountByID = Global.GetTotalBindingGoodsCountByID(caiLiaoId);
		return totalGoodsCountByID - totalBindingGoodsCountByID > 0;
	}

	private int GetNotBindCaiLiaoCount(int caiLiaoId)
	{
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(caiLiaoId);
		int totalBindingGoodsCountByID = Global.GetTotalBindingGoodsCountByID(caiLiaoId);
		return totalGoodsCountByID - totalBindingGoodsCountByID;
	}

	private int GetJuHunID(int id)
	{
		return (id >= 100) ? 100 : id;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public DPSelectedItemEventHandler ShowPropertyCallBack;

	public GButton m_JuHunBtn;

	public GButton m_ClearEquipBtn;

	public TextBlock m_JuHunDescribe;

	public TextBlock m_TongQian;

	public TextBlock m_ChengGongLv;

	public SpriteSL EquipNow;

	public SpriteSL Cailiao1;

	public SpriteSL Cailiao2;

	public SpriteSL Cailiao3;

	public SpriteSL Cailiao4;

	public GCheckBox m_CheckBox;

	public GCheckBox m_BindCheckBox;

	private List<string> juHunGradeNames;

	private List<JuHunData> juHunDatas;

	public SpriteSL[] hunHuos;

	private SpriteSL[] _equipIcon;

	private List<LianLuJuHunPart.CaiLiaoStruct> m_CaiLiaos;

	private int NeedMoney;

	private bool isBindEquip;

	private int goodsId;

	private int useShenShi = -1;

	private int useBindCaiLiao = -1;

	private int m_JuHunID;

	private bool m_IsUseShenShi;

	private string teXiaoPath;

	private string teXiaoForePath = "UITeXiao/Perfabs/hunhuo/";

	private string hunHuoJiHuo = "UITeXiao/Perfabs/hunhuo/hunhuo_jihuo";

	[StructLayout(0, Size = 1)]
	private struct CaiLiaoStruct
	{
		public int ID { get; set; }

		public int Index { get; set; }

		public bool IsEnough { get; set; }

		public bool IsBind { get; set; }

		public int Count { get; set; }
	}
}
