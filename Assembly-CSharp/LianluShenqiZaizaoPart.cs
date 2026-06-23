using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LianluShenqiZaizaoPart : UserControl
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
		this.QianghuaHintText.Text = Global.GetLang("10阶以上装备可通过再造获得进阶");
		this.CheckBoxBind.Text = Global.GetLang("优先使用绑定材料");
		this.SubmitBtn.Text = Global.GetLang("再造");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartZaizaoEquip();
		};
		this.CheckBoxBind.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.equipIcon[0].Length() > 0)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				ShenqiZaizaoXmlData shenqiZaizaoXmlData = Global.GetShenqiZaizaoXmlData(goodsData);
				if (shenqiZaizaoXmlData == null)
				{
					return;
				}
				this.AddGoodsList(shenqiZaizaoXmlData.NeedGoods);
			}
		};
	}

	public void InitPartSize(int width, int height)
	{
		this.BakTrans.localScale = new Vector3(462f, 458f, this.BakTrans.localScale.z);
		this.BakTrans.localPosition = new Vector3(-98f, -28f, this.BakTrans.localPosition.z);
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[6];
		this.equipIcon[0] = this.EquipNow;
		this.equipIcon[1] = this.EquipMax;
		this.equipIcon[2] = this.Cailiao3;
		this.equipIcon[3] = this.Cailiao1;
		this.equipIcon[4] = this.Cailiao2;
		this.equipIcon[5] = this.Cailiao4;
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.ZhandouliText.Text = string.Empty;
		this.TongqianText.Text = "0";
		this.ChenggonglvText.Text = "0%";
		NGUITools.SetActive(this.CheckBoxBind.gameObject, false);
		NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
		this.QianghuaProgressBar.ProgessText = string.Format(Global.GetLang("再造点数：{0}/{1}"), 0, 0);
		this.QianghuaProgressBar.Percent = 0.0;
		this.Bak_black.gameObject.transform.localScale = this.totalVer3;
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 6)
		{
			this.equipIcon[index].Clear();
		}
	}

	public void AddEquip(GoodsData gd, bool result = true)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			GoodsData nextQualityEquipGoodsData = this.GetNextQualityEquipGoodsData(gd, result);
			if (nextQualityEquipGoodsData != null)
			{
				this.AddEquipGoodsIcon(nextQualityEquipGoodsData, 1, false, 0);
			}
			NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
			this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(nextQualityEquipGoodsData) - Global.GetGoodsDataZhanLi(gd));
		}
	}

	private GoodsData GetNextQualityEquipGoodsData(GoodsData goodsData, bool result)
	{
		if (ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID) == null)
		{
			return null;
		}
		ShenqiZaizaoXmlData shenqiZaizaoXmlData = Global.GetShenqiZaizaoXmlData(goodsData);
		if (shenqiZaizaoXmlData == null)
		{
			return null;
		}
		this.SetXiaohao(shenqiZaizaoXmlData, goodsData, result);
		int newEquitID = shenqiZaizaoXmlData.NewEquitID;
		if (ConfigGoods.GetGoodsXmlNodeByID(newEquitID) == null)
		{
			return null;
		}
		int goodsID = newEquitID;
		return new GoodsData
		{
			BagIndex = goodsData.BagIndex,
			Binding = goodsData.Binding,
			Endtime = goodsData.Endtime,
			Forge_level = goodsData.Forge_level,
			AppendPropLev = goodsData.AppendPropLev,
			GCount = goodsData.GCount,
			GoodsID = goodsID,
			Id = 10000,
			Jewellist = goodsData.Jewellist,
			Props = goodsData.Props,
			Quality = goodsData.Quality,
			SaleMoney1 = goodsData.SaleMoney1,
			SaleYinPiao = goodsData.SaleYinPiao,
			SaleYuanBao = goodsData.SaleYuanBao,
			Site = goodsData.Site,
			Starttime = goodsData.Starttime,
			Using = goodsData.Using,
			AddPropIndex = goodsData.AddPropIndex,
			BornIndex = goodsData.BornIndex,
			Lucky = goodsData.Lucky,
			ExcellenceInfo = goodsData.ExcellenceInfo,
			Strong = goodsData.Strong,
			WashProps = goodsData.WashProps,
			JuHunID = goodsData.JuHunID,
			ElementhrtsProps = goodsData.ElementhrtsProps
		};
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
				NGUITools.SetActive(this.CheckBoxBind.gameObject, true);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(icon);
		}
	}

	private void AddGoodsList(string goodsStr)
	{
		if (string.IsNullOrEmpty(goodsStr))
		{
			return;
		}
		string[] array = goodsStr.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2 != null && array2.Length >= 2)
			{
				this.AddGoodsIcon(array2[0].SafeToInt32(0), i + 2, array2[1].SafeToInt32(0));
			}
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid3_bak";
			GoodsData gd = Global.GetIsBindGoodsDataByID(goodsID, this.CheckBoxBind.Check);
			if (gd == null)
			{
				gd = Global.GetDummyGoodsData(goodsID);
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
				goodsID,
				0,
				-1,
				-1
			});
			icon.ItemNum = iNeedNub;
			icon.ItemCode = goodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = 5;
			icon.Text = iNeedNub.ToString();
			icon.TextShadowColor = 4278190080U;
			icon.TextColor = 16777215U;
			icon.DisableTextColor = 8421504U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			icon.STextVisibility = false;
			icon.GoodImg.transform.localPosition = new Vector3(0f, 0f, -1f);
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd = (icon.ItemObject as GoodsData);
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			icon.Text = string.Format("{0}/{1}", totalGoodsCountByID, iNeedNub);
			if (totalGoodsCountByID >= iNeedNub)
			{
				icon.EnableIcon = true;
				icon.TextColor = 16777215U;
			}
			else
			{
				icon.EnableIcon = false;
				icon.TextColor = 16711680U;
			}
			icon.TextShadowColor = 4278190080U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(icon);
		}
	}

	private void SetXiaohao(ShenqiZaizaoXmlData data, GoodsData goodsData, bool result)
	{
		this.NeedTongqian = data.NeedBandJinBi;
		this.TongqianText.Text = this.NeedTongqian.ToString();
		this.QianghuaProgressBar.ProgessText = string.Format(Global.GetLang("再造点数：{0}/{1}"), Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian), data.NeedZaiSheng);
		this.QianghuaProgressBar.Percent = (double)Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian) / (double)data.NeedZaiSheng;
		double num = (double)Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian) / (double)data.NeedZaiSheng;
		if (num >= 1.0)
		{
			num = 1.0;
		}
		this.newLength = Mathf.Round(float.Parse(((double)this.totalLength * (1.0 - num)).ToString()));
		this.usedTime = ((!result) ? 1f : 0f);
		this.isDoing = true;
		double successRate = data.SuccessRate;
		this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
		{
			successRate * 100.0
		});
		string needGoods = data.NeedGoods;
		this.AddGoodsList(needGoods);
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqian)
		{
			this.TongqianText.textColor = 16711680U;
		}
		else
		{
			this.TongqianText.textColor = 16777215U;
		}
	}

	private void ChangeLength()
	{
		this.usedTime += Time.deltaTime;
		this.Bak_black.gameObject.transform.localScale = Vector3.Lerp(this.totalVer3, new Vector3((this.newLength <= 0f) ? 1f : this.newLength, 30f, 1f), this.usedTime / this.totalTime);
		if (this.usedTime >= this.totalTime)
		{
			this.isDoing = false;
		}
	}

	private new void Update()
	{
		if (this.isDoing)
		{
			this.ChangeLength();
		}
	}

	private void StartZaizaoEquip()
	{
		int num = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入再造的装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		this.ForgeDbID = goodsData.Id;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int binding = goodsData.Binding;
		ShenqiZaizaoXmlData shenqiZaizaoXmlData = Global.GetShenqiZaizaoXmlData(goodsData);
		if (shenqiZaizaoXmlData != null)
		{
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian);
			if (roleCommonUseParamsValue < shenqiZaizaoXmlData.NeedZaiSheng)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZaizaodian, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqian)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			return;
		}
		for (int i = 2; i < this.equipIcon.Length; i++)
		{
			if (this.equipIcon[i].Length() > 0)
			{
				GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.equipIcon[i][0]);
				goodsData = (ggoodIcon.ItemObject as GoodsData);
				int goodsID = goodsData.GoodsID;
				int itemNum = ggoodIcon.ItemNum;
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (goodsData == null || totalGoodsCountByID < itemNum)
				{
					if (Super.ShowGoodsGuide(goodsID, this.callback) == 1)
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法强化"), new object[]
						{
							Global.GetGoodsNameByID(goodsID, false)
						}), 19, -1, -1, goodsID);
					}
					return;
				}
				if (goodsData.Binding == 1)
				{
					num = goodsData.Binding;
				}
			}
		}
		if (binding == 0 && num == 1)
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
					this.ExecuteForgeEquip();
				}
			}, buttons);
			return;
		}
		this.ExecuteForgeEquip();
	}

	private void ExecuteForgeEquip()
	{
		this.ShowModalDialog();
		GameInstance.Game.SpriteShenqiZaizao(this.ForgeDbID, (!this.CheckBoxBind.Check) ? 0 : 1);
	}

	public void NotifyResult(int result, int dbID, int binding)
	{
		this.CloseModalDialog();
		GoodsData goodsData = Global.GetGoodsDataByDbID(dbID, null);
		string text = string.Empty;
		if (goodsData != null)
		{
			text = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		}
		if (result < 1)
		{
			if (result == 0)
			{
				if (goodsData == null && this.equipIcon[0].Length() > 0)
				{
					goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
					goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
					this.AddEquip(goodsData, false);
				}
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备无法再造"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("再造点不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("材料不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("再造【{0}】时发生错误:{1}"), new object[]
				{
					text,
					result
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
		}
		else
		{
			if (goodsData != null)
			{
				this.InitAllValue();
				this.AddEquipGoodsIcon(goodsData, 1, false, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1,
				PlayID = goodsData.Forge_level
			});
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
			});
		}
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public GButton SubmitBtn;

	public TextBlock TongqianText;

	public TextBlock ChenggonglvText;

	public TextBlock QianghuaHintText;

	public TextBlock ZhandouliText;

	public GCheckBox CheckBoxBind;

	public GImgProgressBar QianghuaProgressBar;

	public SpriteSL EquipNow;

	public SpriteSL EquipMax;

	public SpriteSL Cailiao1;

	public SpriteSL Cailiao2;

	public SpriteSL Cailiao3;

	public SpriteSL Cailiao4;

	public Transform BakTrans;

	public UISprite Bak_black;

	private float totalLength = 423f;

	private Vector3 totalVer3 = new Vector3(423f, 30f, 1f);

	private float usedTime;

	private float totalTime = 0.5f;

	private bool isDoing;

	private float newLength;

	private int ForgeDbID = -1;

	private int NeedTongqian;

	private SpriteSL[] _equipIcon;
}
