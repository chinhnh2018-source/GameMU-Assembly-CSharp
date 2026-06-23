using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;
using XMLCreater;

public class OtherAttributePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.HaoYouBtn.Text = Global.GetLang("加为好友");
		this.SiLiaoBtn.Text = Global.GetLang("私人聊天");
		this.HeiMingDanBtn.Text = Global.GetLang("加入屏蔽");
		this.ZuDuiBtn.Text = Global.GetLang("组      队");
		this.ChouRenBtn.Text = Global.GetLang("加为仇人");
		this.JiaoYiBtn.Text = Global.GetLang("交      易");
		this.Level.Y = 97.0;
		this.FamilyName.Y = 65.0;
		this.OtherRoleName.Y = 30.0;
		this.RoleIDText.X = -19.0;
		this.TeamInviteBtn.Text = Global.GetLang("战队邀请");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.chjingjiu.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/chengjiu_58x58.png";
		this.junxian.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/junxian_58x58.png";
		this.qianghua.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/qianghua_58x58.png";
		this.zhuijia.GoodImg.URL = "NetImages/GameRes/Images/Hybrid/zhuijia_58x58.png";
		this.ReturnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.JiaoYiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Type = 6
				});
			}
		};
		if (null != this.SiLiaoBtn)
		{
			this.SiLiaoBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			};
		}
		if (null != this.HaoYouBtn)
		{
			this.HaoYouBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3,
					Type = 0,
					Tag = this.SName.Text
				});
			};
		}
		if (null != this.HeiMingDanBtn)
		{
			this.HeiMingDanBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3,
					Type = 1,
					Tag = this.SName.Text
				});
			};
		}
		if (null != this.ChouRenBtn)
		{
			this.ChouRenBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3,
					Type = 2,
					Tag = this.SName.Text
				});
			};
		}
		if (null != this.ZuDuiBtn)
		{
			this.ZuDuiBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 5,
					Tag = this.SName.Text
				});
			};
		}
		if (!ConfigVersionSystemOpen.IsVersionSystemOpen(100108) || !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompete))
		{
			this.TeamInviteBtn.gameObject.SetActive(false);
		}
		else
		{
			this.TeamInviteBtn.gameObject.SetActive(true);
			this.TeamInviteBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
					return;
				}
				if (Super.GData.OtherRoleData != null)
				{
					TeamCompeteDataManager.SendInviteTeanMemberMsg(Super.GData.OtherRoleData.RoleID);
				}
			};
		}
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
		this.equipIcon[23] = this.dectionIcon;
		this.equipIcon[24] = this.fashion;
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
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public GGoodIcon FindEquipIcon(int equipCategory, int bagIndex = -1)
	{
		if (Super.GData.OtherRoleData.GoodsDataList == null)
		{
			return null;
		}
		for (int i = 0; i < Super.GData.OtherRoleData.GoodsDataList.Count; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Super.GData.OtherRoleData.GoodsDataList[i].GoodsID);
			if (goodsXmlNodeByID != null)
			{
				if (bagIndex < 0 || Super.GData.OtherRoleData.GoodsDataList[i].BagIndex == bagIndex)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (equipCategory == categoriy && Super.GData.OtherRoleData.GoodsDataList[i].Using > 0)
					{
						GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
						ggoodIcon.isAutoSize = true;
						ggoodIcon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty), false, 0);
						ggoodIcon.TipType = 1;
						ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
						{
							goodsXmlNodeByID.ID,
							1,
							Super.GData.OtherRoleData.GoodsDataList[i].Id,
							1
						});
						ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
						ggoodIcon.ItemCode = Super.GData.OtherRoleData.GoodsDataList[i].GoodsID;
						ggoodIcon.ItemObject = Super.GData.OtherRoleData.GoodsDataList[i];
						ggoodIcon.BoxTypes = 0;
						ggoodIcon.TextSize = 20;
						ggoodIcon.TextShadowColor = 4278190080U;
						Super.InitEquipGIcon(ggoodIcon, Super.GData.OtherRoleData.GoodsDataList[i], true, IconTextTypes.Qianghua);
						return ggoodIcon;
					}
				}
			}
		}
		return null;
	}

	private void SetJingMaiProgress()
	{
	}

	public void SetLearnedSkillProgress()
	{
	}

	public void SetLifeMagicValues()
	{
	}

	public void SetInterPowerValue()
	{
	}

	public void SetPKValues()
	{
	}

	public void SetExpProgress()
	{
	}

	public void InitPartData()
	{
	}

	public void GetNewData()
	{
		if (this.equipIcon[11].Count() > 0)
		{
			this.equipIcon[11].RemoveAt(0, true, true);
		}
		if (this.equipIcon[36].Count() > 0)
		{
			this.equipIcon[36].RemoveAt(0, true, true);
		}
		if (this.equipIcon[0].Count() > 0)
		{
			this.equipIcon[0].RemoveAt(0, true, true);
		}
		if (this.equipIcon[1].Count() > 0)
		{
			this.equipIcon[1].RemoveAt(0, true, true);
		}
		if (this.equipIcon[2].Count() > 0)
		{
			this.equipIcon[2].RemoveAt(0, true, true);
		}
		if (this.equipIcon[3].Count() > 0)
		{
			this.equipIcon[3].RemoveAt(0, true, true);
		}
		if (this.equipIcon[4].Count() > 0)
		{
			this.equipIcon[4].RemoveAt(0, true, true);
		}
		if (this.equipIcon[5].Count() > 0)
		{
			this.equipIcon[5].RemoveAt(0, true, true);
		}
		if (this.equipIcon[6].Count() > 0)
		{
			this.equipIcon[6].RemoveAt(0, true, true);
		}
		if (this.equipIcon[31].Count() > 0)
		{
			this.equipIcon[31].RemoveAt(0, true, true);
		}
		if (this.equipIcon[7].Count() > 0)
		{
			this.equipIcon[7].RemoveAt(0, true, true);
		}
		if (this.equipIcon[8].Count() > 0)
		{
			this.equipIcon[8].RemoveAt(0, true, true);
		}
		if (this.equipIcon[9].Count() > 0)
		{
			this.equipIcon[9].RemoveAt(0, true, true);
		}
		if (this.equipIcon[22].Count() > 0)
		{
			this.equipIcon[22].RemoveAt(0, true, true);
		}
		if (this.equipIcon[23].Count() > 0)
		{
			this.equipIcon[23].RemoveAt(0, true, true);
		}
		if (this.equipIcon[24].Count() > 0)
		{
			this.equipIcon[24].RemoveAt(0, true, true);
		}
		this.GetUsingGoodsList();
		for (int i = 0; i < 25; i++)
		{
			this.SetEquipIcon(i);
		}
		this.ShowPartData(this.DataFields);
		this.SetHuiJiIcon();
		this.RefreshBufferUI();
		this.ShowJueXingIcon();
	}

	private void SetHuiJiIcon()
	{
		if (Super.GData.OtherRoleData.HuiJiData == null)
		{
			return;
		}
		if (Super.GData.OtherRoleData.HuiJiData.huiji < 1)
		{
			return;
		}
		if (!Global.GetTerraceOpen("EmblemOpen"))
		{
			return;
		}
		int num = 0;
		if (Global.m_DicStar.Count <= 0)
		{
			Global.m_DicStar = Global.AddDicEmblemStart();
		}
		if (Global.m_DicStar.ContainsKey(Super.GData.OtherRoleData.HuiJiData.huiji))
		{
			num = Global.m_DicStar[Super.GData.OtherRoleData.HuiJiData.huiji].EmblemLevel;
		}
		int huiJiGoodsId = Global.GetHuiJiGoodsId(num);
		GoodsData goodsData = null;
		try
		{
			goodsData = Global.GetFakeEquipGoodsData(huiJiGoodsId, num, 0);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"徽记物品模拟创建失败"
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
		Dictionary<int, GoodsData> jueXingEquips = JueXingData.GetJueXingEquips(Super.GData.OtherRoleData);
		List<int> equipedJiHuoList = JueXingData.GetEquipedJiHuoList(Super.GData.OtherRoleData.JueXingData);
		for (int i = 0; i < equipedJiHuoList.Count; i++)
		{
			MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(equipedJiHuoList[i]);
			int num = jueXingShiInfoById.Position;
			GoodsData equipInfo = JueXingData.GetEquipInfo(jueXingEquips, (JueXingPositionType)num);
			if (num == 1)
			{
				this.jueXingIcon[21].URL = JueXingData.GetJueXingShiIconURL(jueXingShiInfoById);
				this.jueXingIcon[22].URL = JueXingData.GetJueXingShiIconURL(jueXingShiInfoById);
				if (equipInfo != null)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(equipInfo.GoodsID);
					if (equipInfo != JueXingData.GetEquipInfo(jueXingEquips, JueXingPositionType.WuQiZuo))
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

	private void FreezeAllHint()
	{
	}

	public void CleanUpChildWindows()
	{
		this.FreezeAllHint();
		Super.CleanUpAllChildWindows(this.Container);
	}

	private int GetMapCodeByLingDiID(int lingDiID, int[] lingDiIDs2MapCodes)
	{
		if (lingDiIDs2MapCodes == null)
		{
			return 0;
		}
		if (lingDiID < 0 || lingDiID >= lingDiIDs2MapCodes.Length)
		{
			return 0;
		}
		return lingDiIDs2MapCodes[lingDiID - 1];
	}

	public void ShowPartData(string[] fields)
	{
		if (fields == null)
		{
			return;
		}
		this.TouXingImage.URL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(Super.GData.OtherRoleData.Occupation),
			Super.GData.OtherRoleData.RoleSex
		});
		this.VipLevel.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.GetOtherRoleVIPLeve(Super.GData.OtherRoleData)
		});
		this.SName.Text = Global.FormatRoleName(Super.GData.OtherRoleData);
		string bhname = Super.GData.OtherRoleData.BHName;
		this.FamilyName.Text = ((!string.IsNullOrEmpty(bhname)) ? bhname : Global.GetLang("无"));
		this.Level.Text = StringUtil.substitute(Global.GetLang("{0}转{1}级"), new object[]
		{
			fields[21],
			Super.GData.OtherRoleData.Level
		});
		this.RoleIDText.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			StringUtil.substitute(Global.GetLang("ID:{0}"), new object[]
			{
				(long)Super.GData.OtherRoleData.RoleID ^ (long)((ulong)-936551821)
			})
		});
		this.zhanDouLitxt.Text = fields[22];
		this.ShowOtherName(Super.GData.OtherRoleData.OtherName);
	}

	public void ShowOtherName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			this.OtherRoleName.text = Global.GetLang("无");
		}
		else
		{
			this.OtherRoleName.text = name;
		}
	}

	private List<GoodsData> GetUsingGoodsList()
	{
		this.usingGoodsList.Clear();
		if (Super.GData.OtherRoleData.GoodsDataList == null)
		{
			return this.usingGoodsList;
		}
		for (int i = 0; i < Super.GData.OtherRoleData.GoodsDataList.Count; i++)
		{
			if (Super.GData.OtherRoleData.GoodsDataList[i].Using == 1 && Super.GData.OtherRoleData.GoodsDataList[i].Site == 0)
			{
				this.usingGoodsList.Add(Super.GData.OtherRoleData.GoodsDataList[i]);
			}
			else if (Super.GData.OtherRoleData.GoodsDataList[i].Using == 1 && Super.GData.OtherRoleData.GoodsDataList[i].Site == 5000)
			{
				this.usingGoodsList.Add(Super.GData.OtherRoleData.GoodsDataList[i]);
			}
			else if (Super.GData.OtherRoleData.GoodsDataList[i].Using == 1 && Super.GData.OtherRoleData.GoodsDataList[i].Site == 6000)
			{
				this.usingGoodsList.Add(Super.GData.OtherRoleData.GoodsDataList[i]);
			}
		}
		return this.usingGoodsList;
	}

	private void SetEquipIcon(int equipCategory)
	{
		if (this.usingGoodsList == null)
		{
			return;
		}
		for (int i = 0; i < this.usingGoodsList.Count; i++)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.usingGoodsList[i].GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy != 23 && categoriy != 8)
			{
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
					icon.ItemCode = this.usingGoodsList[i].GoodsID;
					icon.ItemObject = this.usingGoodsList[i];
					icon.BoxTypes = 0;
					icon.TextSize = 20;
					icon.TextShadowColor = 4278190080U;
					this.InitGoodIconSize(icon, icon.ItemCategory);
					icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GoodsData goodsData = icon.ItemObject as GoodsData;
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						GTipServiceEx.SelfBagOnly = false;
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, (categoriyByGoodsID != 24) ? GoodsOwnerTypes.OtherRole : GoodsOwnerTypes.LookOther, goodsData);
					};
					icon.DPImageDownloadedItem = delegate(object s, DPSelectedItemEventArgs ev)
					{
						this.SetBoxCollider(icon);
					};
					if (this.usingGoodsList[i].ExcellenceInfo > 0 || categoriy == 22 || categoriy == 9 || categoriy == 10 || categoriy == 7 || categoriy == 24 || categoriy == 23)
					{
						this.SetExcellenceStat(icon, categoriy);
					}
					this.SetEquipBorderBySuitID(icon, this.usingGoodsList[i]);
					int actionType = goodsXmlNodeByID.ActionType;
					int handType = goodsXmlNodeByID.HandType;
					this.SetZhuangBeiPeiDai(icon, categoriy, handType, this.usingGoodsList[i].BagIndex);
				}
			}
		}
	}

	public void SetZhuangBeiPeiDai(GGoodIcon icon, int categoriy, int handType, int iBagIndex)
	{
		if (categoriy == 17 && Super.GData.OtherRoleData.Occupation == 3 && handType == 1)
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
				else
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
		else
		{
			if (this.equipIcon[categoriy].Count() > 0)
			{
				this.equipIcon[categoriy].RemoveAt(0, true, true);
			}
			this.equipIcon[categoriy].Add(icon);
		}
	}

	public void SetBoxCollider(GGoodIcon icon)
	{
		if ((icon.ItemCategory >= 11 && icon.ItemCategory <= 36 && icon.ItemCategory != 23 && icon.ItemCategory != 24) || icon.ItemCategory == 1)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(83f, 129f, 0f);
		}
		else if (icon.ItemCategory == 0 || icon.ItemCategory == 2 || icon.ItemCategory == 3 || icon.ItemCategory == 4 || icon.ItemCategory == 22 || icon.ItemCategory == 7)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(80f, 80f, 0f);
		}
		else if (icon.ItemCategory == 5 || icon.ItemCategory == 9 || icon.ItemCategory == 31 || icon.ItemCategory == 24 || icon.ItemCategory == 23)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(53f, 53f, 0f);
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

	private void ShowDuiBiWindow()
	{
		if (null != OtherAttributePart.DuiBiWindow)
		{
			if (OtherAttributePart.DuiBiWindow.Visibility)
			{
				OtherAttributePart.DuiBiWindow.Visibility = false;
			}
			else
			{
				OtherAttributePart.DuiBiWindow.Visibility = true;
			}
			return;
		}
		OtherAttributePart.DuiBiWindow = U3DUtils.NEW<GChildWindow>();
		OtherAttributePart.DuiBiWindow.Modal = true;
		OtherAttributePart.InitChildWindow(OtherAttributePart.DuiBiWindow, "DuiBiWindow");
		this.Container.Children.Add(OtherAttributePart.DuiBiWindow);
		ShuXingDuiBiPart shuXingDuiBiPart = U3DUtils.NEW<ShuXingDuiBiPart>();
		shuXingDuiBiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.ShowDuiBiWindow();
			return true;
		};
		OtherAttributePart.DuiBiWindow.SetContent(OtherAttributePart.DuiBiWindow.BodyPresenter, shuXingDuiBiPart, 0.0, 0.0, true);
	}

	protected static void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
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
		else if (icon.ItemCategory == 22 || icon.ItemCategory == 9 || icon.ItemCategory == 7 || icon.ItemCategory == 23 || icon.ItemCategory == 24)
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

	private Vector3 Pos(Vector3 v, float x, float y, float z)
	{
		v.x = x;
		v.y = y;
		v.z = z;
		return v;
	}

	public void RefreshBufferUI()
	{
		this.CurrentTaoZhuangIndex = this.GetCurrentTaoZhuangIndex();
		if (this.CurrentTaoZhuangIndex != -1)
		{
			List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
			this.qianghua.SecondText.text = taoZhuangList[this.CurrentTaoZhuangIndex].Level.ToString();
			this.qianghua.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.qianghua.GoodImg.ToGrayBitmap = true;
			this.qianghua.SecondText.text = string.Empty;
		}
		this.CurrentZhuoYueIndex = this.GetCurrentZhuoYueIndex();
		if (this.CurrentZhuoYueIndex != -1)
		{
			this.zhuijia.SecondText.text = (this.CurrentZhuoYueIndex + 1).ToString();
			this.zhuijia.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.zhuijia.GoodImg.ToGrayBitmap = true;
			this.zhuijia.SecondText.text = string.Empty;
		}
		if (Super.GData.OtherRoleData != null)
		{
			this.CurrentChengJiuLevel = Global.GetRoleCommonUseParamsValueForOtherRole2(Super.GData.OtherRoleData, RoleCommonUseIntParamsIndexs.ChengJiuLevel);
		}
		if (this.CurrentChengJiuLevel > 0)
		{
			this.chjingjiu.GoodImg.ToGrayBitmap = false;
			this.chjingjiu.SecondText.text = this.CurrentChengJiuLevel.ToString();
		}
		else
		{
			this.chjingjiu.GoodImg.ToGrayBitmap = true;
			this.chjingjiu.ContentText.text = string.Empty;
		}
		this.CurrentJunXianLevel = Global.GetRoleCommonUseParamsValueForOtherRole2(Super.GData.OtherRoleData, RoleCommonUseIntParamsIndexs.ShengWangLevel);
		if (this.CurrentJunXianLevel > 0)
		{
			this.junxian.SecondText.text = this.CurrentJunXianLevel.ToString();
			this.junxian.GoodImg.ToGrayBitmap = false;
		}
		else
		{
			this.junxian.SecondText.text = string.Empty;
			this.junxian.GoodImg.ToGrayBitmap = true;
		}
	}

	public int GetCurrentTaoZhuangIndex()
	{
		List<GoodsData> list = this.GetUsingGoodsList();
		List<int> list2 = new List<int>();
		if (list == null)
		{
			return -1;
		}
		if (list.Count <= 0)
		{
			return -1;
		}
		for (int i = 0; i < list.Count; i++)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(list[i].GoodsID);
			if (list[i].Strong < (int)goodsEquipPropsDoubleList[0])
			{
				int num = (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue) - list[i].Strong / Global.MaxNotifyEquipStrongValue;
			}
			list2.Add(list[i].Forge_level);
		}
		int result = -1;
		if (list2.Count >= 8)
		{
			list2.Sort();
			int num2 = list2[list2.Count - 8];
			List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
			int count = taoZhuangList.Count;
			if (num2 < taoZhuangList[0].Level)
			{
				result = -1;
			}
			else if (num2 >= taoZhuangList[count - 1].Level)
			{
				result = count - 1;
			}
			else
			{
				for (int j = 0; j < count - 1; j++)
				{
					if (num2 >= taoZhuangList[j].Level && num2 < taoZhuangList[j + 1].Level)
					{
						result = j;
						break;
					}
				}
			}
		}
		return result;
	}

	public int GetCurrentZhuoYueIndex()
	{
		List<GoodsData> list = this.GetUsingGoodsList();
		if (list == null)
		{
			return -1;
		}
		if (list.Count <= 0)
		{
			return -1;
		}
		List<int> list2 = new List<int>();
		for (int i = 0; i < list.Count; i++)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(list[i].GoodsID);
			if (list[i].Strong < (int)goodsEquipPropsDoubleList[0])
			{
				int num = (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue) - list[i].Strong / Global.MaxNotifyEquipStrongValue;
			}
			list2.Add(list[i].ExcellenceInfo);
		}
		int result = -1;
		if (list2.Count >= 8)
		{
			list2.Sort();
			int excellenceInfo = list2[list2.Count - 8];
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(excellenceInfo);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				result = 0;
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				result = 1;
			}
			else if (zhuoyueAttributeCount >= 5)
			{
				result = 2;
			}
		}
		return result;
	}

	private void SetEquipBorderBySuitID(GGoodIcon icon, GoodsData goodsData)
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
		else if (categoriyByGoodsID == 6 || categoriyByGoodsID == 5 || categoriyByGoodsID == 9 || categoriyByGoodsID == 10 || categoriyByGoodsID == 23 || categoriyByGoodsID == 24)
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

	public TextBlock SName;

	public TextBlock Level;

	public TextBlock OtherRoleName;

	public TextBlock FamilyName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public TextBlock VipLevel;

	public TextBlock zhanDouLitxt;

	public TextBlock Title;

	public TextBlock RoleIDText;

	public GButton HaoYouBtn;

	public GButton SiLiaoBtn;

	public GButton HeiMingDanBtn;

	public GButton ZuDuiBtn;

	public GButton ChouRenBtn;

	public GButton JiaoYiBtn;

	public GButton ReturnBtn;

	public GButton TeamInviteBtn;

	public Dictionary<int, GGoodIcon> equipIcon = new Dictionary<int, GGoodIcon>();

	public DPSelectedItemBoolEventHandler DPSelectedItem;

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

	public GGoodIcon dectionIcon;

	public GGoodIcon fashion;

	public GGoodIcon huijiIcon;

	public ShowNetImage TouXingImage;

	private static GChildWindow DuiBiWindow;

	private List<GoodsData> usingGoodsList = new List<GoodsData>();

	public string[] DataFields;

	public GGoodIcon chjingjiu;

	public GGoodIcon zhuijia;

	public GGoodIcon qianghua;

	public GGoodIcon junxian;

	private int CurrentTaoZhuangIndex = -1;

	private int CurrentZhuoYueIndex = -1;

	private int CurrentChengJiuLevel = -1;

	private int CurrentJunXianLevel = -1;

	public GameObject jueXingIconContainer;

	public ShowNetImage jueXingShiTemplate;

	private Dictionary<int, GGoodIcon> jueXingShiContainer = new Dictionary<int, GGoodIcon>();

	private Dictionary<int, ShowNetImage> jueXingIcon = new Dictionary<int, ShowNetImage>();
}
