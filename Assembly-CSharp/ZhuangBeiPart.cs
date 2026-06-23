using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class ZhuangBeiPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Bak.URL = "NetImages/GameRes/Images/Plate/roleZhuangbei_bak.jpg";
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.equipIcon[11] = this.wuqizuoIcon;
		this.equipIcon[36] = this.wuqiyouIcon;
		this.equipIcon[0] = this.toukuiIcon;
		this.equipIcon[1] = this.kaijiaIcon;
		this.equipIcon[2] = this.hushouIcon;
		this.equipIcon[3] = this.hutuiIcon;
		this.equipIcon[4] = this.xueziIcon;
		this.equipIcon[5] = this.xianglianIcon;
		this.equipIcon[6] = this.jiezhizuoIcon;
		this.equipIcon[31] = this.jiezhiyouIcon;
		this.equipIcon[7] = this.zuoJiIcon;
		this.equipIcon[8] = this.chibangIcon;
		this.equipIcon[9] = this.shouhuchongIcon;
		this.equipIcon[22] = this.hufuIcon;
		this.jueXingShiContainer[2] = this.xianglianIcon;
		this.jueXingShiContainer[3] = this.jiezhizuoIcon;
		this.jueXingShiContainer[4] = this.jiezhiyouIcon;
		this.jueXingShiContainer[5] = this.toukuiIcon;
		this.jueXingShiContainer[6] = this.kaijiaIcon;
		this.jueXingShiContainer[7] = this.hushouIcon;
		this.jueXingShiContainer[8] = this.hutuiIcon;
		this.jueXingShiContainer[9] = this.xueziIcon;
		this.jueXingShiContainer[21] = this.wuqizuoIcon;
		this.jueXingShiContainer[22] = this.wuqiyouIcon;
		this.equipIcon[23] = this.decorationIcon;
		this.equipIcon[24] = this.fashion;
		this.ShowShuXingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		this.qianghua.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenZhuangBeiTaoZhuangBufferPart();
		};
		this.zhuijia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenZhuangBeiZhuoYueBufferPart();
		};
		this.chjingjiu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentChengJiuLevel > 0)
			{
				this.OpenChengJiuBufferPart();
			}
			else
			{
				Super.HintMainText(Global.GetLang("成就等级不足，暂无成就加成"), 10, 3);
			}
		};
		this.junxia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurrentJunXianLevel > 0)
			{
				this.OpenJunXianBufferPart();
			}
			else
			{
				Super.HintMainText(Global.GetLang("军衔等级不足，暂无军衔加成"), 10, 3);
			}
		};
		this.ShowJueXingIcon();
	}

	public void InitPartData()
	{
		this.zhandouli.X = -40.0;
		this.zhandouli.Text = Global.Data.roleData.CombatForce.ToString();
		this.StartSetEquipIcon();
		this.chjingjiu.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/chengjiu_58x58.png";
		this.junxia.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/junxian_58x58.png";
		this.qianghua.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/qianghua_58x58.png";
		this.zhuijia.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/zhuijia_58x58.png";
		this.RefreshBufferUI();
	}

	public void RefreshBufferUI()
	{
		this.CurrentTaoZhuangIndex = Global.GetCurrentTaoZhuangIndex();
		if (this.CurrentTaoZhuangIndex != -1)
		{
			List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
			this.qianghua.SecondText.text = "+" + taoZhuangList[this.CurrentTaoZhuangIndex].Level.ToString();
			this.qianghua.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.qianghua.SecondText.text = string.Empty;
			this.qianghua.GoodImg.ToGrayBitmap = true;
		}
		this.CurrentZhuoYueIndex = Global.GetCurrentZhuoYueIndex();
		if (this.CurrentZhuoYueIndex != -1)
		{
			this.zhuijia.SecondText.text = (this.CurrentZhuoYueIndex + 1).ToString();
			this.zhuijia.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.zhuijia.SecondText.text = string.Empty;
			this.zhuijia.GoodImg.ToGrayBitmap = true;
		}
		if (Global.Data.ChengJiuData != null)
		{
			this.CurrentChengJiuLevel = Global.GetChengJiuLevel((int)Global.Data.ChengJiuData.ChengJiuPoints);
		}
		if (this.CurrentChengJiuLevel > 0)
		{
			this.chjingjiu.SecondText.text = this.CurrentChengJiuLevel.ToString();
			this.chjingjiu.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.chjingjiu.SecondText.text = string.Empty;
			this.chjingjiu.GoodImg.ToGrayBitmap = true;
		}
		this.CurrentJunXianLevel = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		if (this.CurrentJunXianLevel > 0)
		{
			this.junxia.SecondText.text = this.CurrentJunXianLevel.ToString();
			this.junxia.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.junxia.SecondText.text = string.Empty;
			this.junxia.GoodImg.ToGrayBitmap = true;
		}
	}

	private void StartSetEquipIcon()
	{
		if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
		{
			Global.GetUsingGoodsDataList();
		}
		this.usingGoodsList = Super.GData.RoleUsingGoodsDataList;
		if (Global.Data.equipPet != null)
		{
			for (int i = 0; i < Global.Data.equipPet.Count; i++)
			{
				if (Global.Data.equipPet[i].Using != 0)
				{
					this.usingGoodsList[Global.Data.equipPet[i].Id] = Global.Data.equipPet[i];
				}
			}
		}
		for (int j = 0; j < 25; j++)
		{
			if (j != 23 && j != 8)
			{
				this.SetEquipIcon(j);
			}
		}
		this.SetMarryRingIcon();
		this.SetHuiJiIcon();
	}

	public void CleanUpChildWindows()
	{
	}

	public void GetNewData()
	{
		this.UnFreezeAllHint();
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
	}

	private void OpenZhuangBeiTaoZhuangBufferPart()
	{
		this.TaoZhuangBuffWindow = U3DUtils.NEW<GChildWindow>();
		this.TaoZhuangBuffWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.TaoZhuangBuffWindow, Global.GetLang("装备套装"));
		this.TaoZhuangBuffWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseZhuangBeiTaoZhuangBufferPart();
			return true;
		};
		if (null == this.TaoZhuangPart)
		{
			this.TaoZhuangPart = U3DUtils.NEW<ZhuangBeiTaoZhuangBufferPart>();
			this.TaoZhuangPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseZhuangBeiTaoZhuangBufferPart();
				}
			};
			this.TaoZhuangPart.RefreshUI(this.CurrentTaoZhuangIndex);
			this.TaoZhuangBuffWindow.SetContent(this.TaoZhuangBuffWindow.BodyPresenter, this.TaoZhuangPart, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, -100f, 0f);
			this.TaoZhuangPart.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(this.TaoZhuangBuffWindow);
		}
	}

	private void CloseZhuangBeiTaoZhuangBufferPart()
	{
		if (null != this.TaoZhuangBuffWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.TaoZhuangBuffWindow);
			Object.Destroy(this.TaoZhuangPart.gameObject);
			this.TaoZhuangPart = null;
			this.TaoZhuangBuffWindow = null;
		}
	}

	private void OpenZhuangBeiZhuoYueBufferPart()
	{
		this.ZhuoYueBuffWindow = U3DUtils.NEW<GChildWindow>();
		this.ZhuoYueBuffWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.ZhuoYueBuffWindow, Global.GetLang("卓越效果"));
		this.ZhuoYueBuffWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseZhuangBeiZhuoYueBufferPart();
			return true;
		};
		if (null == this.ZhuoYuePart)
		{
			this.ZhuoYuePart = U3DUtils.NEW<ZhuangBeiZhuoYueBufferPart>();
			this.ZhuoYuePart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseZhuangBeiZhuoYueBufferPart();
				}
			};
			this.ZhuoYuePart.RefreshUI(this.CurrentZhuoYueIndex);
			this.ZhuoYueBuffWindow.SetContent(this.ZhuoYueBuffWindow.BodyPresenter, this.ZhuoYuePart, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, -100f, 0f);
			this.ZhuoYuePart.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(this.ZhuoYueBuffWindow);
		}
	}

	private void CloseZhuangBeiZhuoYueBufferPart()
	{
		if (null != this.ZhuoYueBuffWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.ZhuoYueBuffWindow);
			Object.Destroy(this.ZhuoYuePart.gameObject);
			this.ZhuoYuePart = null;
			this.ZhuoYueBuffWindow = null;
		}
	}

	private void OpenChengJiuBufferPart()
	{
		this.ChengJiuBuffWindow = U3DUtils.NEW<GChildWindow>();
		this.ChengJiuBuffWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.ChengJiuBuffWindow, Global.GetLang("成就增益"));
		this.ChengJiuBuffWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseChengJiuBufferPart();
			return true;
		};
		if (null == this.ChengJiuPart)
		{
			this.ChengJiuPart = U3DUtils.NEW<ChengJiuBufferPart>();
			this.ChengJiuPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseChengJiuBufferPart();
				}
			};
			this.ChengJiuPart.RefreshUI(this.CurrentChengJiuLevel, true);
			this.ChengJiuBuffWindow.SetContent(this.ChengJiuBuffWindow.BodyPresenter, this.ChengJiuPart, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, -100f, 0f);
			this.ChengJiuPart.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(this.ChengJiuBuffWindow);
		}
	}

	private void CloseChengJiuBufferPart()
	{
		if (null != this.ChengJiuBuffWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.ChengJiuBuffWindow);
			Object.Destroy(this.ChengJiuPart.gameObject);
			this.ChengJiuPart = null;
			this.ChengJiuBuffWindow = null;
		}
	}

	private void OpenJunXianBufferPart()
	{
		this.JunXianBuffWindow = U3DUtils.NEW<GChildWindow>();
		this.JunXianBuffWindow.ModalType = ChildWindowModalType.Translucent;
		Super.InitChildWindow(this.JunXianBuffWindow, Global.GetLang("军衔增益"));
		this.JunXianBuffWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
		{
			this.CloseJunXianBufferPart();
			return true;
		};
		if (null == this.JunXianPart)
		{
			this.JunXianPart = U3DUtils.NEW<ChengJiuBufferPart>();
			this.JunXianPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.CloseJunXianBufferPart();
				}
			};
			this.JunXianPart.RefreshUI(this.CurrentJunXianLevel, false);
			this.JunXianBuffWindow.SetContent(this.JunXianBuffWindow.BodyPresenter, this.JunXianPart, 0.0, 0.0, true);
			Vector3 localPosition;
			localPosition..ctor(0f, -100f, 0f);
			this.JunXianPart.transform.localPosition = localPosition;
			Super.GData.PlayZoneRoot.Children.Add(this.JunXianBuffWindow);
		}
	}

	private void CloseJunXianBufferPart()
	{
		if (null != this.JunXianBuffWindow)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot, this.JunXianBuffWindow);
			Object.Destroy(this.JunXianPart.gameObject);
			this.JunXianPart = null;
			this.JunXianBuffWindow = null;
		}
	}

	private void UnFreezeAllHint()
	{
	}

	private void SetEquipIcon(int equipCategory)
	{
		if (this.usingGoodsList == null)
		{
			return;
		}
		foreach (KeyValuePair<int, GoodsData> keyValuePair in this.usingGoodsList)
		{
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (equipCategory == categoriy)
			{
				GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
				icon.isAutoSize = true;
				icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
				{
					Super.GetIconCode(goodsXmlNodeByID)
				}), false, 0);
				icon.TipType = 1;
				icon.ItemCategory = goodsXmlNodeByID.Categoriy;
				icon.ItemCode = value.GoodsID;
				icon.ItemObject = value;
				icon.BoxTypes = 0;
				icon.TextSize = 20;
				icon.Tag = value.ExcellenceInfo;
				this.InitGoodIconSize(icon, icon.ItemCategory);
				if (value.ExcellenceInfo > 0 || categoriy == 22 || categoriy == 9 || categoriy == 10 || categoriy == 23 || categoriy == 24)
				{
					this.SetExcellenceStat(icon, categoriy);
				}
				this.SetEquipBorderBySuitID(icon, value);
				icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
				{
					if (ev.IDType == 2)
					{
						if (Global.Data.GameScene.IsDead())
						{
							return;
						}
						GoodsData goodsData = icon.ItemObject as GoodsData;
						int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
						if (goodsCatetoriy == 10 || goodsCatetoriy == 9)
						{
							if (goodsData.Using == 1)
							{
								goodsData.Using = 0;
								GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
							}
						}
						else if (Global.CanAddGoods(goodsData.GoodsID, goodsData.GCount, goodsData.Binding, goodsData.Endtime, false))
						{
							if (Global.Data.roleData.MapCode == 8 && Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 8)
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前地图中禁止卸载此装备！"), new object[0]), 0, -1, -1, 0);
								return;
							}
							if (goodsData.Using == 1)
							{
								goodsData.Using = 0;
								GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
							}
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再卸载装备..."), new object[0]), 1, -1, -1, 0);
						}
					}
					else if (ev.IDType == 16)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = -10
						});
						PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
						{
							ID = 1330
						});
					}
				};
				icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
				{
					this.SetBoxCollider(icon);
				};
				int handType = goodsXmlNodeByID.HandType;
				icon.transform.name = string.Format("{0}::{1}", (ItemCategories)goodsXmlNodeByID.Categoriy, value.BagIndex);
				this.SetZhuangBeiPeiDai(icon, equipCategory, handType, value.BagIndex);
			}
		}
	}

	private void SetHuiJiIcon()
	{
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.HuiJiHuTi))
		{
			return;
		}
		if (!Global.GetTerraceOpen("EmblemOpen"))
		{
			return;
		}
		int huiJiGoodsId = Global.GetHuiJiGoodsId(Global.SetEmbelemUpLevel);
		GoodsData goodsData = null;
		try
		{
			goodsData = Global.GetFakeEquipGoodsData(huiJiGoodsId, Global.SetEmbelemUpLevel, 0);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"徽记物品模拟创建失败:goodsData创建失败"
			});
		}
		if (goodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		if (this.huijiIcon.Count() <= 0)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.isAutoSize = true;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
			{
				Super.GetIconCode(goodsXmlNodeByID)
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = 0;
			icon.TextSize = 20;
			icon.Tag = goodsData.ExcellenceInfo;
			NGUITools.SetActive(icon.TeXiao.gameObject, true);
			icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("zhuoyueFlowLight_bag", true));
			this.SetExcellenceStat(icon, categoriy);
			icon.addEventListener("click", new MouseEventHandler(this.HuiJiClickBtn));
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
			};
			icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				this.SetBoxCollider(icon);
			};
			int handType = goodsXmlNodeByID.HandType;
			if (this.huijiIcon.Count() > 0)
			{
				this.huijiIcon.RemoveAt(0, true, true);
			}
			this.huijiIcon.Add(icon);
		}
	}

	private void SetMarryRingIcon()
	{
		if (Global.Data.MarryData == null || Global.Data.MarryData.nRingID == -1)
		{
			return;
		}
		GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(Global.Data.MarryData.nRingID, 0, 0);
		if (fakeEquipGoodsData == null)
		{
			return;
		}
		fakeEquipGoodsData.AppendPropLev = (int)Global.Data.MarryData.byGoodwillstar;
		fakeEquipGoodsData.Forge_level = (int)Global.Data.MarryData.byGoodwilllevel;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(fakeEquipGoodsData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		if ((int)Global.Data.MarryData.byMarrytype == -1)
		{
			if (this.equipIcon[goodsXmlNodeByID.Categoriy].Count() > 0)
			{
				this.equipIcon[goodsXmlNodeByID.Categoriy].RemoveAt(0, true, true);
			}
		}
		else
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.isAutoSize = true;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Equip/{0}.png", new object[]
			{
				Super.GetIconCode(goodsXmlNodeByID)
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = fakeEquipGoodsData.GoodsID;
			icon.ItemObject = fakeEquipGoodsData;
			icon.BoxTypes = 0;
			icon.TextSize = 20;
			icon.Tag = fakeEquipGoodsData.ExcellenceInfo;
			this.InitGoodIconSize(icon, icon.ItemCategory);
			if (fakeEquipGoodsData.ExcellenceInfo > 0 || categoriy == 22 || categoriy == 9 || categoriy == 10 || categoriy == 7)
			{
				this.SetExcellenceStat(icon, categoriy);
			}
			this.SetEquipBorderBySuitID(icon, fakeEquipGoodsData);
			icon.addEventListener("click", new MouseEventHandler(this.MarryRingClickBtn));
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
			};
			icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				this.SetBoxCollider(icon);
			};
			int handType = goodsXmlNodeByID.HandType;
			this.SetZhuangBeiPeiDai(icon, categoriy, handType, fakeEquipGoodsData.BagIndex);
		}
	}

	public void ShowJueXingIcon()
	{
		Dictionary<int, GGoodIcon>.Enumerator enumerator = this.jueXingShiContainer.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ShowNetImage showNetImage = null;
			Dictionary<int, ShowNetImage> dictionary = this.jueXingIcon;
			KeyValuePair<int, GGoodIcon> keyValuePair = enumerator.Current;
			if (!dictionary.TryGetValue(keyValuePair.Key, ref showNetImage))
			{
				GameObject gameObject = NGUITools.AddChild(this.jueXingIconContainer, this.jueXingShiTemplate.gameObject);
				Object @object = gameObject;
				object obj = "juexingshi";
				KeyValuePair<int, GGoodIcon> keyValuePair2 = enumerator.Current;
				@object.name = obj + (JueXingPositionType)keyValuePair2.Key;
				gameObject.transform.localScale = this.jueXingShiTemplate.transform.localScale;
				Transform transform = gameObject.transform;
				KeyValuePair<int, GGoodIcon> keyValuePair3 = enumerator.Current;
				transform.localPosition = keyValuePair3.Value.transform.localPosition;
				showNetImage = gameObject.GetComponent<ShowNetImage>();
				Dictionary<int, ShowNetImage> dictionary2 = this.jueXingIcon;
				KeyValuePair<int, GGoodIcon> keyValuePair4 = enumerator.Current;
				dictionary2[keyValuePair4.Key] = showNetImage;
			}
			showNetImage.gameObject.SetActive(false);
		}
		JueXingData.ResetSelfJueXingEquips();
		Dictionary<int, GoodsData> selfJueXingEquips = JueXingData.GetSelfJueXingEquips();
		List<int> equipedJiHuoList = JueXingData.GetEquipedJiHuoList(JueXingData.GetSelfJueXingData());
		for (int i = 0; i < equipedJiHuoList.Count; i++)
		{
			MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(equipedJiHuoList[i]);
			int num = jueXingShiInfoById.Position;
			GoodsData equipInfo = JueXingData.GetEquipInfo(selfJueXingEquips, (JueXingPositionType)num);
			if (num == 1)
			{
				this.jueXingIcon[21].URL = JueXingData.GetJueXingShiIconURL(jueXingShiInfoById);
				this.jueXingIcon[22].URL = JueXingData.GetJueXingShiIconURL(jueXingShiInfoById);
				if (equipInfo != null)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(equipInfo.GoodsID);
					if (JueXingData.GetEquipInfo(selfJueXingEquips, JueXingPositionType.WuQi) != JueXingData.GetEquipInfo(selfJueXingEquips, JueXingPositionType.WuQiZuo))
					{
						this.jueXingIcon[21].gameObject.SetActive(false);
						num = 22;
					}
					else
					{
						this.jueXingIcon[22].gameObject.SetActive(false);
						num = 21;
					}
				}
				else
				{
					num = 21;
				}
			}
			ShowNetImage showNetImage2 = this.jueXingIcon[num];
			showNetImage2.gameObject.SetActive(true);
			showNetImage2.URL = JueXingData.GetJueXingShiIconURL(jueXingShiInfoById);
			bool flag = JueXingData.IsCanEffect(equipInfo);
			showNetImage2.ToGrayBitmap = !flag;
		}
	}

	private void RefershJueXingShi(GGoodIcon usingIcon)
	{
		if (null == usingIcon.parent)
		{
			return;
		}
		GGoodIcon component = usingIcon.Parent.GetComponent<GGoodIcon>();
		if (component == null)
		{
			return;
		}
		Dictionary<int, GoodsData> selfJueXingEquips = JueXingData.GetSelfJueXingEquips();
		int num = -1;
		foreach (KeyValuePair<int, GGoodIcon> keyValuePair in this.jueXingShiContainer)
		{
			if (keyValuePair.Value == component)
			{
				Dictionary<int, GGoodIcon>.Enumerator enumerator;
				KeyValuePair<int, GGoodIcon> keyValuePair2 = enumerator.Current;
				num = keyValuePair2.Key;
				break;
			}
		}
		GoodsData goodsData = usingIcon.ItemObject as GoodsData;
		JueXingData.SetJueXingEquipPosition(goodsData, selfJueXingEquips, Global.Data.roleData);
		if (num == 1 || num == 22 || num == 21)
		{
			goodsData = JueXingData.GetEquipInfo(selfJueXingEquips, JueXingPositionType.WuQi);
			if (goodsData != JueXingData.GetEquipInfo(selfJueXingEquips, JueXingPositionType.WuQiZuo))
			{
				this.jueXingIcon[21].gameObject.SetActive(false);
				this.jueXingIcon[22].gameObject.SetActive(true);
				num = 22;
			}
			else
			{
				this.jueXingIcon[22].gameObject.SetActive(false);
				this.jueXingIcon[21].gameObject.SetActive(true);
				num = 21;
			}
		}
		ShowNetImage showNetImage = null;
		if (this.jueXingIcon.TryGetValue(num, ref showNetImage))
		{
			bool flag = JueXingData.IsCanEffect(goodsData);
			showNetImage.ToGrayBitmap = !flag;
		}
	}

	public void RemoveEquipIcon(GoodsData gd)
	{
		foreach (KeyValuePair<int, GGoodIcon> keyValuePair in this.equipIcon)
		{
			if (keyValuePair.Value != null && keyValuePair.Value.transform.childCount > 0)
			{
				GoodsData goodsData = keyValuePair.Value.getChildAt(0).GetComponent<GGoodIcon>().ItemObject as GoodsData;
				if (goodsData != null && goodsData.Id == gd.Id && this.equipIcon[keyValuePair.Key].Count() > 0)
				{
					this.equipIcon[keyValuePair.Key].Remove(0);
				}
			}
		}
		this.RefreshBufferUI();
		this.ShowJueXingIcon();
	}

	public void SetZhuangBeiPeiDai(GGoodIcon icon, int categoriy, int handType, int iBagIndex)
	{
		if (categoriy == 17 && Global.Data.roleData.Occupation == 3 && handType == 1)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(icon.ItemCode);
			if (goodsXmlNodeByID.ActionType == 1)
			{
				handType = 2;
			}
		}
		if (categoriy >= 11 && categoriy <= 21)
		{
			int num = 11;
			int num2 = 36;
			if (handType == 1)
			{
				if (this.equipIcon[num].Count() > 0)
				{
					this.equipIcon[num].RemoveAt(0, true, true);
				}
				this.equipIcon[num].Add(icon);
			}
			else if (handType == 0)
			{
				if (this.equipIcon[num2].Count() > 0)
				{
					this.equipIcon[num2].RemoveAt(0, true, true);
				}
				this.equipIcon[num2].Add(icon);
			}
			else if (handType == 2)
			{
				if (iBagIndex == 0)
				{
					if (this.equipIcon[num].Count() > 0)
					{
						this.equipIcon[num].RemoveAt(0, true, true);
					}
					this.equipIcon[num].Add(icon);
				}
				else if (iBagIndex == 1)
				{
					if (this.equipIcon[num2].Count() > 0)
					{
						this.equipIcon[num2].RemoveAt(0, true, true);
					}
					this.equipIcon[num2].Add(icon);
				}
			}
		}
		else if (categoriy == 6)
		{
			if (iBagIndex == 0)
			{
				if (this.equipIcon[categoriy].Count() > 0)
				{
					this.equipIcon[categoriy].RemoveAt(0, true, true);
				}
				this.equipIcon[categoriy].Add(icon);
			}
			else if (iBagIndex == 1)
			{
				if (this.equipIcon[25 + categoriy].Count() > 0)
				{
					this.equipIcon[25 + categoriy].RemoveAt(0, true, true);
				}
				this.equipIcon[25 + categoriy].Add(icon);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"戒子的数据出错  位置为非左手、右手"
				});
				if (this.equipIcon[25 + categoriy].Count() == 0)
				{
					this.equipIcon[25 + categoriy].Add(icon);
				}
				else if (this.equipIcon[categoriy].Count() == 0)
				{
					this.equipIcon[categoriy].Add(icon);
				}
				else
				{
					if (this.equipIcon[categoriy].Count() > 0)
					{
						this.equipIcon[categoriy].RemoveAt(0, true, true);
					}
					this.equipIcon[categoriy].Add(icon);
				}
			}
		}
		else if (categoriy == 9 || categoriy == 10)
		{
			if (this.equipIcon[9].Count() > 0)
			{
				this.equipIcon[9].RemoveAt(0, true, true);
			}
			this.equipIcon[9].Add(icon);
		}
		else if (25 > categoriy || 27 < categoriy)
		{
			if (40 > categoriy || 45 < categoriy)
			{
				if (this.equipIcon[categoriy].Count() > 0)
				{
					this.equipIcon[categoriy].RemoveAt(0, true, true);
				}
				this.equipIcon[categoriy].Add(icon);
			}
		}
		this.RefershJueXingShi(icon);
	}

	public void RefreshEquipGoods(GoodsData gd)
	{
		this.StartSetEquipIcon();
		this.RefreshBufferUI();
	}

	public void SetBoxCollider(GGoodIcon icon)
	{
		bool flag = Global.CanUseGoodsAttr(icon.ItemCode, false);
		if ((icon.ItemCategory >= 11 && icon.ItemCategory <= 36 && icon.ItemCategory != 22 && icon.ItemCategory != 23 && icon.ItemCategory != 24) || icon.ItemCategory == 1)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(83f, 129f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(83f, 129f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 0 || icon.ItemCategory == 2 || icon.ItemCategory == 3 || icon.ItemCategory == 4 || icon.ItemCategory == 22 || icon.ItemCategory == 7)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(80f, 80f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(80f, 80f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 5 || icon.ItemCategory == 9 || icon.ItemCategory == 6 || icon.ItemCategory == 31 || icon.ItemCategory == 23 || icon.ItemCategory == 24)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(53f, 53f, 0f);
			if (!flag)
			{
				icon.NoUseSprite.spriteName = "iconState_nouse2";
				icon.NoUseSprite.transform.transform.localScale = new Vector3(53f, 53f, 1f);
				icon.NoUseSprite.gameObject.SetActive(true);
			}
		}
		else if (icon.ItemCategory == 8)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(115f, 78f, 0f);
		}
		Super.InitEquipGIcon(icon, icon.ItemObject as GoodsData, true, IconTextTypes.Qianghua);
		icon.ContentText.Pivot = 2;
		icon.ContentText.X = (double)(icon.GetComponent<BoxCollider>().size.x / 2f);
		icon.ContentText.Y = (double)(icon.GetComponent<BoxCollider>().size.y / 2f);
	}

	public void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.SelfBagOnly = false;
		if (goodsData.Site == 6000)
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = 1480
			});
			PlayZone.GlobalPlayZone.CloseGamePayerRoleWindow();
		}
		else
		{
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SelfBag, goodsData);
		}
	}

	public void HuiJiClickBtn(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public void MarryRingClickBtn(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
	}

	public void InitGoodIconSize(GGoodIcon icon, int iCategoriy)
	{
		int num = 64;
		int num2 = 64;
		if ((iCategoriy >= 11 && iCategoriy <= 21) || iCategoriy == 1)
		{
			num = 83;
			num2 = 129;
		}
		else if (iCategoriy == 8)
		{
			num = 115;
			num2 = 78;
		}
		else if (iCategoriy == 6 || iCategoriy == 5 || iCategoriy == 9 || iCategoriy == 10 || iCategoriy == 23 || iCategoriy == 24)
		{
			num = 53;
			num2 = 53;
		}
		else if (iCategoriy == 7 || iCategoriy == 0 || iCategoriy == 2 || iCategoriy == 4 || iCategoriy == 3 || iCategoriy == 22)
		{
			num = 80;
			num2 = 80;
		}
		icon.Width = (double)num;
		icon.Height = (double)num2;
	}

	public void SetExcellenceStat(GGoodIcon icon, int iCategoriy)
	{
		if ((iCategoriy >= 11 && iCategoriy <= 21) || iCategoriy == 1)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(83f, 129f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xiongjia");
		}
		else if (iCategoriy == 8)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(115f, 78f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_chibang");
		}
		else if (iCategoriy == 6 || iCategoriy == 5 || iCategoriy == 9 || iCategoriy == 10 || iCategoriy == 23 || iCategoriy == 24)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(53f, 53f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xianglian");
		}
		else if (iCategoriy == 7 || iCategoriy == 0 || iCategoriy == 2 || iCategoriy == 4 || iCategoriy == 3 || iCategoriy == 22)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(80f, 80f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_toukui");
		}
		icon.BackgroundSprite1Visible = true;
	}

	private void SetExcellence(GGoodIcon icon, string zhuoyueTeXiaoPrefab)
	{
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		if (icon.ItemCategory == 10 || icon.ItemCategory == 9)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsData.ExcellenceInfo != 0)
			{
				icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
				icon.TeXiao.gameObject.SetActive(true);
			}
			else if (goodsXmlNodeByID.SuitID == 1)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
		}
		else if (goodsData.ExcellenceInfo > 0)
		{
			if (Global.GetZhuoyueAttributeCount(goodsData) >= 6)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) < 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) >= 3 && Global.GetZhuoyueAttributeCount(goodsData) < 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (Global.GetZhuoyueAttributeCount(goodsData) == 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
		}
		else if (icon.ItemCategory == 22 || icon.ItemCategory == 7 || icon.ItemCategory == 23 || icon.ItemCategory == 24)
		{
			int goodsQuality = Super.GetGoodsQuality(goodsData.GoodsID);
			if (goodsQuality == 1)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (goodsQuality == 2)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (goodsQuality == 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
			}
			else if (goodsQuality == 4)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (goodsQuality == 6)
			{
			}
		}
		icon.BindingSprite.gameObject.SetActive(goodsData.Binding > 0);
		Vector3 localScale = icon.BackgroundSprite1.transform.localScale;
		icon.BindingSprite.transform.localPosition = this.Pos(localScale, -(localScale.x / 2f - 12f), -(localScale.y / 2f - 12f), -0.03f);
	}

	public void SetEquipBorderBySuitID(GGoodIcon icon, GoodsData goodsData)
	{
		if (null == icon)
		{
			return;
		}
		if (goodsData == null)
		{
			return;
		}
		if (!Global.IsShengqi(goodsData))
		{
			return;
		}
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		Transform transform = icon.BackgroundSprite15.transform;
		if ((categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21) || categoriyByGoodsID == 1)
		{
			transform.localScale = new Vector3(93f, 139f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 8)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(125f, 88f);
			transform.localPosition = new Vector3(0.5f, 0f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 6 || categoriyByGoodsID == 5 || categoriyByGoodsID == 9 || categoriyByGoodsID == 10)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(63f, 63f);
			transform.localPosition = new Vector3(0.5f, 0.5f, transform.localPosition.z);
		}
		else if (categoriyByGoodsID == 7 || categoriyByGoodsID == 0 || categoriyByGoodsID == 2 || categoriyByGoodsID == 4 || categoriyByGoodsID == 3 || categoriyByGoodsID == 22)
		{
			icon.BackgroundSprite15.transform.localScale = new Vector3(90f, 90f);
		}
		Vector3 vector = icon.BackgroundSprite15.transform.localScale;
		vector += new Vector3(2f, 2f, 0f);
		icon.BackSpriteName15 = "iconStateGold";
		icon.BackgroundSprite15.transform.localScale = vector;
	}

	private Vector3 Pos(Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		return v;
	}

	public GGoodIcon zuoJiIcon;

	public GGoodIcon toukuiIcon;

	public GGoodIcon chibangIcon;

	public GGoodIcon wuqizuoIcon;

	public GGoodIcon wuqiyouIcon;

	public GGoodIcon xianglianIcon;

	public GGoodIcon kaijiaIcon;

	public GGoodIcon shouhuchongIcon;

	public GGoodIcon hushouIcon;

	public GGoodIcon jiezhizuoIcon;

	public GGoodIcon jiezhiyouIcon;

	public GGoodIcon hutuiIcon;

	public GGoodIcon xueziIcon;

	public GGoodIcon hufuIcon;

	public GGoodIcon decorationIcon;

	public GGoodIcon fashion;

	public GGoodIcon huijiIcon;

	public GGoodIcon chjingjiu;

	public GGoodIcon zhuijia;

	public GGoodIcon qianghua;

	public GGoodIcon junxia;

	public TextBlock zhanDouLitxt;

	public GButton ShowShuXingBtn;

	public TextBlock zhandouli;

	public Dictionary<int, GGoodIcon> equipIcon = new Dictionary<int, GGoodIcon>();

	private Dictionary<int, GoodsData> usingGoodsList = new Dictionary<int, GoodsData>();

	private bool FirstGetNewData = true;

	private int CurrentTaoZhuangIndex = -1;

	private int CurrentZhuoYueIndex = -1;

	private int CurrentChengJiuLevel = -1;

	private int CurrentJunXianLevel = -1;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public ShowNetImage Bak;

	public GameObject jueXingIconContainer;

	public ShowNetImage jueXingShiTemplate;

	private Dictionary<int, GGoodIcon> jueXingShiContainer = new Dictionary<int, GGoodIcon>();

	private Dictionary<int, ShowNetImage> jueXingIcon = new Dictionary<int, ShowNetImage>();

	private GChildWindow TaoZhuangBuffWindow;

	private ZhuangBeiTaoZhuangBufferPart TaoZhuangPart;

	private GChildWindow ZhuoYueBuffWindow;

	private ZhuangBeiZhuoYueBufferPart ZhuoYuePart;

	private GChildWindow ChengJiuBuffWindow;

	private ChengJiuBufferPart ChengJiuPart;

	private GChildWindow JunXianBuffWindow;

	private ChengJiuBufferPart JunXianPart;
}
