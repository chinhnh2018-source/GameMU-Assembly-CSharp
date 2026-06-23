using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LianluZhuangbeiQianghuaPart : UserControl
{
	public void InitTextInPrefabs()
	{
		JieRiFanBeiItemData jieRiFanBeiItemData = null;
		if (!Global.JieriFanbeiInfo.TryGetValue(104, ref jieRiFanBeiItemData))
		{
			return;
		}
		string extArg = jieRiFanBeiItemData.ExtArg1;
		string extArg2 = jieRiFanBeiItemData.ExtArg2;
		if (string.IsNullOrEmpty(extArg) || extArg.Split(new char[]
		{
			','
		}).Length == 1)
		{
			this.QianghuaHintText.Text = Global.GetLang("装备强化后会附有华丽的流光效果");
		}
		else
		{
			string[] array = extArg.Split(new char[]
			{
				','
			});
			DateTime dataTime = DateTime.Parse(array[1]);
			string text = dataTime.toString("yyyy-MM-dd");
			DateTime dataTime2 = DateTime.Parse(array[2]);
			string text2 = dataTime2.toString("yyyy-MM-dd");
			if (extArg2.Equals("1"))
			{
				this.QianghuaHintText.Text = string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("至")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text2
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("强化上限开放至20")
					})
				}) + "\n" + Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("强化+15升级至 +16失败不掉级")
				});
			}
			else
			{
				this.QianghuaHintText.Text = string.Format("{0}{1}{2}{3}", new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("至")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text2
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						Global.GetLang("强化上限开放至20")
					})
				});
			}
		}
		this.CheckBox.Text = Global.GetLang("始终使用\n神佑晶石");
		this.CheckBoxBind.Text = Global.GetLang("优先使用绑定材料");
		this.SubmitBtn.Text = Global.GetLang("强化");
	}

	protected override void InitializeComponent()
	{
		GameInstance.Game.GetJieriFanbeiInfo();
		this.CheckBox.Text = Global.GetLang("始终使用\n神佑晶石");
		this.CheckBoxBind.Text = Global.GetLang("优先使用绑定材料");
		this.SubmitBtn.Text = Global.GetLang("强化");
		this.QianghuaHintText.Text = Global.GetLang("装备强化后会附有华丽的流光效果");
		this.CheckBox.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			if (!this.CheckBox.isChecked)
			{
				Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("不使用神佑晶石，强化失败等级降1级，确定不使用神佑晶石？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
			}
		};
		this.CheckBoxBind.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.equipIcon[0].Length() > 0)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
				this.AddGoodsList(goodsData.Forge_level);
			}
		};
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			this.CheckBox.isChecked = false;
		}
		else if (args.ID == 1)
		{
			this.CheckBox.isChecked = true;
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

	public new string BodyBackgroundURL
	{
		set
		{
			this.Bak.URL = Super.GetWindowsBakImageURLFromName(value);
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.QianghuaProgressBar.ItemWidth = 21f;
		if (Global.MaxForgeLevel == 20)
		{
			this.QianghuaProgressBar.MaxLevel = 20;
			this.QianghuaProgressBar.transform.localPosition = new Vector3(-145f, 70f, 0f);
		}
		else
		{
			this.QianghuaProgressBar.MaxLevel = 15;
			this.QianghuaProgressBar.transform.localPosition = new Vector3(-90f, 70f, 0f);
		}
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartForgeEquip();
		};
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
		this.EquipText.Text = string.Empty;
		this.QianghuaProgressBar.Level = 0;
		NGUITools.SetActive(this.CheckBox.gameObject, false);
		NGUITools.SetActive(this.CheckBoxBind.gameObject, false);
		NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 6)
		{
			this.equipIcon[index].Clear();
		}
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			int forgeLevel = Math.Min(gd.Forge_level + 1, Global.MaxForgeLevel);
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(gd.GoodsID, forgeLevel, gd.AppendPropLev, gd.ExcellenceInfo, gd.Lucky, gd.Binding, gd.GCount, 0, gd.WashProps, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			this.AddEquipGoodsIcon(dummyGoodsDataMu, 1, false, 0);
			this.QianghuaProgressBar.Level = gd.Forge_level;
			NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
			if (gd.Forge_level >= Global.MaxForgeLevel)
			{
				return;
			}
			this.AddGoodsList(gd.Forge_level);
			this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(dummyGoodsDataMu) - Global.GetGoodsDataZhanLi(gd));
			int num = Global.GetForgeNextLevelYinLiang(gd);
			num = Global.RecalcNeedYinLiang(num);
			this.TongqianText.Text = num.ToString();
			object roleData = Global.Data.roleData;
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < num)
			{
				this.TongqianText.textColor = 16711680U;
			}
			else
			{
				this.TongqianText.textColor = 16777215U;
			}
			int forgePercent = Global.GetForgePercent(gd, 0);
			int num2 = 0;
			if (forgePercent < 100)
			{
				num2 = Global.GetSystemParamVipLeveValue("VIPQiangHuaAdd");
			}
			if (num2 > 0)
			{
				this.ChenggonglvText.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffffff",
					string.Format("{0}% ", forgePercent),
					"00ff00",
					string.Format("+{0}%", num2)
				});
			}
			else
			{
				this.ChenggonglvText.Text = string.Format("{0}%", forgePercent);
			}
		}
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

	private void AddGoodsList(int forgeLevel)
	{
		int num = forgeLevel + 1;
		int[] forgeNeedGoodsList = Global.GetForgeNeedGoodsList(num);
		int[] forgeNeedGoodsNumList = Global.GetForgeNeedGoodsNumList(num);
		if (forgeNeedGoodsList != null)
		{
			for (int i = 0; i < forgeNeedGoodsList.Length; i++)
			{
				this.AddGoodsIcon(forgeNeedGoodsList[i], i + 2, forgeNeedGoodsNumList[i]);
			}
		}
		if (num > 6)
		{
			int goodsID = -1;
			int iNeedNub = 0;
			Global.GetBaoHuGoodsIDs(num, out goodsID, out iNeedNub);
			this.AddGoodsIcon(goodsID, 5, iNeedNub);
			NGUITools.SetActive(this.CheckBox.gameObject, true);
		}
		else
		{
			NGUITools.SetActive(this.CheckBox.gameObject, false);
		}
	}

	private void GetEquipInfo(out int suitID, out int qianghuangLevel)
	{
		suitID = 0;
		qianghuangLevel = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		suitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
		qianghuangLevel = goodsData.Forge_level;
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
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd = (icon.ItemObject as GoodsData);
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			int num = Global.GetTotalGoodsCountByID(goodsID);
			int num2 = 0;
			int num3 = 0;
			this.GetEquipInfo(out num2, out num3);
			int num4 = 0;
			bool flag = false;
			num4 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "EquipSuit", ref flag, (long)num2);
			num4 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "QiangHuaLevel", ref flag, (long)num3);
			num += num4;
			icon.Text = string.Format("{0}/{1}", num, iNeedNub);
			if (num >= iNeedNub)
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

	private void StartForgeEquip()
	{
		this.ForgeDbID = -1;
		this.RockGoodsID = -1;
		int num = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要强化的装备"), new object[0]), 0, -1, -1, 0);
			SystemHelpMgr.OnAction(UIObjIDs.LianLuQiangHuaSubmit, HelpStateEvents.Clicked, -1);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		this.ForgeDbID = goodsData.Id;
		SystemHelpMgr.OnAction(UIObjIDs.LianLuQiangHuaSubmit, HelpStateEvents.Clicked, -1);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int binding = goodsData.Binding;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		string title = goodsXmlNodeByID.Title;
		if (goodsData.Forge_level >= Global.MaxForgeLevel)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别"), new object[]
			{
				title
			}), 0, -1, -1, 0);
			return;
		}
		this.NeedYinLiang = Global.GetForgeNextLevelYinLiang(goodsData);
		this.NeedYinLiang = Global.RecalcNeedYinLiang(this.NeedYinLiang);
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedYinLiang)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			return;
		}
		for (int i = 2; i < this.equipIcon.Length; i++)
		{
			if (this.equipIcon[i].Length() > 0)
			{
				if (i != 5 || this.CheckBox.Check)
				{
					GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.equipIcon[i][0]);
					goodsData = (ggoodIcon.ItemObject as GoodsData);
					int goodsID = goodsData.GoodsID;
					int itemNum = ggoodIcon.ItemNum;
					int num2 = Global.GetTotalGoodsCountByID(goodsID);
					int num3 = 0;
					int num4 = 0;
					this.GetEquipInfo(out num3, out num4);
					int num5 = 0;
					bool flag = false;
					num5 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "EquipSuit", ref flag, (long)num3);
					if (flag)
					{
						num = 1;
					}
					num5 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "QiangHuaLevel", ref flag, (long)num4);
					if (flag)
					{
						num = 1;
					}
					num2 += num5;
					if (num2 < itemNum)
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
					if (i == 5)
					{
						this.RockGoodsID = goodsID;
					}
					if (goodsData != null && goodsData.Binding == 1)
					{
						num = goodsData.Binding;
					}
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
		GameInstance.Game.SpriteForgeGoodsNew(this.ForgeDbID, this.RockGoodsID, this.XYfuGoodsID, 1, (!this.CheckBoxBind.Check) ? 0 : 1);
	}

	public void NotifyForgeResult(GoodsData goodsData, int result, int dbID, int forgeLevel, bool down, int binding)
	{
		this.CloseModalDialog();
		Global.Data.GameScene.RemoveForgeDecoration();
		string text = string.Empty;
		if (goodsData != null)
		{
			text = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		}
		if (result < 1)
		{
			if (result == 0)
			{
				if (down)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("强化失败，装备降级"), new object[0]), 0, -1, -1, 0);
				}
			}
			else if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有找到要强化的装备"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -8)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("{0}不能强化"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (result == -9998)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备{0}不在身上或背包中"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			}
			else if (result == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("强化【{0}】时发生错误"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else if (result == -500)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("强化失败，装备降级"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -22)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedShenyoujingshi, this.callback, string.Empty, string.Empty);
			}
			else if (result == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别，无法强化"), new object[]
				{
					text
				}), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("强化【{0}】时发生错误:{1}"), new object[]
				{
					text,
					result
				}), 0, -1, -1, 0);
			}
			if (result != -4)
			{
				Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = -1
				});
			}
		}
		else
		{
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1,
				PlayID = goodsData.Forge_level
			});
		}
		this.AddEquip(goodsData);
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

	private ListBox Equip = new ListBox();

	private ListBox Rock = new ListBox();

	private ListBox XYfu = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public TextBlock EquipText;

	public TextBlock TongqianText;

	public TextBlock ChenggonglvText;

	public TextBlock QianghuaHintText;

	public TextBlock ZhandouliText;

	public GImgProgressBar QianghuaProgressBar;

	public GCheckBox CheckBox;

	public GCheckBox CheckBoxBind;

	public SpriteSL EquipNow;

	public SpriteSL EquipMax;

	public SpriteSL Cailiao1;

	public SpriteSL Cailiao2;

	public SpriteSL Cailiao3;

	public SpriteSL Cailiao4;

	private int ForgeDbID = -1;

	private int RockGoodsID = -1;

	private int NeedYinLiang = -1;

	private int XYfuGoodsID = -1;

	private int[,] EquipPos = new int[,]
	{
		{
			-76,
			99
		},
		{
			62,
			99
		}
	};

	private int[,] CailiaoPos = new int[,]
	{
		{
			-132,
			-50
		},
		{
			-64,
			-50
		},
		{
			4,
			-50
		},
		{
			72,
			-50
		}
	};

	private string name = "ForgeMaxOpen";

	private string baohu = "ForgeProtectOpen";

	private SpriteSL[] _equipIcon;
}
