using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class OtherRebornAttributePart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.HaoYouBtn.Text = Global.GetLang("加为好友");
		this.SiLiaoBtn.Text = Global.GetLang("私人聊天");
		this.HeiMingDanBtn.Text = Global.GetLang("加入屏蔽");
		this.ZuDuiBtn.Text = Global.GetLang("组      队");
		this.ChouRenBtn.Text = Global.GetLang("加为仇人");
		this.JiaoYiBtn.Text = Global.GetLang("交      易");
		this.TeamInviteBtn.Text = Global.GetLang("战队邀请");
		this.LevelWord.Text = Global.GetLang("重生等级:");
		this.FamilyNameWord.Text = Global.GetLang("战盟:");
		this.QuFuNameWord.Text = Global.GetLang("区服:");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
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
		this.equipIcon[37] = this.wuqizuoIcon;
		this.equipIcon[38] = this.wuqiyouIcon;
		this.equipIcon[30] = this.toukuiIcon;
		this.equipIcon[31] = this.kaijiaIcon;
		this.equipIcon[32] = this.hushouIcon;
		this.equipIcon[33] = this.hutuiIcon;
		this.equipIcon[34] = this.xueziIcon;
		this.equipIcon[35] = this.xianglianIcon;
		this.equipIcon[36] = this.jiezhizuoIcon;
		this.equipIcon[136] = this.jiezhiyouIcon;
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

	public void InitPartData()
	{
	}

	public void GetNewData()
	{
		this.GetUsingGoodsList();
		for (int i = 30; i <= 38; i++)
		{
			this.SetEquipIcon(i);
		}
		this.SetEquipIcon(136);
		this.ShowPartData(this.DataFields);
	}

	public void CleanUpChildWindows()
	{
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
		this.SName.Text = Global.FormatRoleNameZoneid(Super.GData.OtherRoleData.ZoneID, string.Empty, 0, 1);
		string bhname = Super.GData.OtherRoleData.BHName;
		this.FamilyName.Text = ((!string.IsNullOrEmpty(bhname)) ? bhname : Global.GetLang("无"));
		this.Level.Text = StringUtil.substitute(Global.GetLang("重生{0}阶{1}级"), new object[]
		{
			Super.GData.OtherRoleData.RebornCount,
			Super.GData.OtherRoleData.RebornLevel
		});
		this.RoleIDText.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			StringUtil.substitute(Global.GetLang("ID:{0}"), new object[]
			{
				(long)Super.GData.OtherRoleData.RoleID ^ (long)((ulong)-936551821)
			})
		});
		this.zhanDouLitxt.Text = fields[29];
		this.ShowQuFurName(IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(Super.GData.OtherRoleData.PTID, true) + Global.GetZoneName(Super.GData.OtherRoleData.ZoneID));
	}

	public void ShowQuFurName(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			this.QuFuName.text = "无";
		}
		else
		{
			this.QuFuName.text = name;
		}
	}

	private List<GoodsData> GetUsingGoodsList()
	{
		this.usingGoodsList.Clear();
		if (Super.GData.OtherRoleData.RebornGoodsDataList == null)
		{
			return this.usingGoodsList;
		}
		for (int i = 0; i < Super.GData.OtherRoleData.RebornGoodsDataList.Count; i++)
		{
			if (Super.GData.OtherRoleData.RebornGoodsDataList[i].Using == 1 && Super.GData.OtherRoleData.RebornGoodsDataList[i].Site == 15000)
			{
				this.usingGoodsList.Add(Super.GData.OtherRoleData.RebornGoodsDataList[i]);
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
				this.SetExcellenceStat(icon, categoriy);
				int actionType = goodsXmlNodeByID.ActionType;
				int handType = goodsXmlNodeByID.HandType;
				this.SetZhuangBeiPeiDai(icon, categoriy, handType, this.usingGoodsList[i].BagIndex);
			}
		}
	}

	public void SetZhuangBeiPeiDai(GGoodIcon icon, int categoriy, int handType, int iBagIndex)
	{
		if (categoriy == 36)
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
				if (this.equipIcon[136].Count() > 0)
				{
					this.equipIcon[136].RemoveAt(0, true, true);
				}
				this.equipIcon[136].Add(icon);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"戒子的数据出错  位置为非左手、右手"
				});
				if (this.equipIcon[136].Count() == 0)
				{
					this.equipIcon[136].Add(icon);
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
		if (icon.ItemCategory == 37 || icon.ItemCategory == 38 || icon.ItemCategory == 31)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(83f, 129f, 0f);
		}
		else if (icon.ItemCategory == 30 || icon.ItemCategory == 32 || icon.ItemCategory == 34 || icon.ItemCategory == 33)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(80f, 80f, 0f);
		}
		else if (icon.ItemCategory == 36 || icon.ItemCategory == 35)
		{
			icon.GetComponent<BoxCollider>().size = new Vector3(53f, 53f, 0f);
		}
		Super.InitEquipGIcon(icon, icon.ItemObject as GoodsData, true, IconTextTypes.Qianghua);
		icon.ContentText.Pivot = 2;
		icon.ContentText.X = (double)(icon.GetComponent<BoxCollider>().size.x / 2f);
		icon.ContentText.Y = (double)(icon.GetComponent<BoxCollider>().size.y / 2f);
	}

	private void ShowDuiBiWindow()
	{
		if (null != OtherRebornAttributePart.DuiBiWindow)
		{
			if (OtherRebornAttributePart.DuiBiWindow.Visibility)
			{
				OtherRebornAttributePart.DuiBiWindow.Visibility = false;
			}
			else
			{
				OtherRebornAttributePart.DuiBiWindow.Visibility = true;
			}
			return;
		}
		OtherRebornAttributePart.DuiBiWindow = U3DUtils.NEW<GChildWindow>();
		OtherRebornAttributePart.DuiBiWindow.Modal = true;
		OtherRebornAttributePart.InitChildWindow(OtherRebornAttributePart.DuiBiWindow, "DuiBiWindow");
		this.Container.Children.Add(OtherRebornAttributePart.DuiBiWindow);
		ShuXingDuiBiPart shuXingDuiBiPart = U3DUtils.NEW<ShuXingDuiBiPart>();
		shuXingDuiBiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.ShowDuiBiWindow();
			return true;
		};
		OtherRebornAttributePart.DuiBiWindow.SetContent(OtherRebornAttributePart.DuiBiWindow.BodyPresenter, shuXingDuiBiPart, 0.0, 0.0, true);
	}

	protected static void InitChildWindow(GChildWindow childWindow, string title)
	{
		Super.InitChildWindow(childWindow, title);
	}

	public void InitGoodIconSize(GGoodIcon icon, int iCategoriy)
	{
		int num = 64;
		int num2 = 64;
		if (iCategoriy == 37 || iCategoriy == 38 || iCategoriy == 31)
		{
			num = 83;
			num2 = 129;
		}
		else if (iCategoriy == 36 || iCategoriy == 35)
		{
			num = 53;
			num2 = 53;
		}
		else if (iCategoriy == 30 || iCategoriy == 32 || iCategoriy == 34 || iCategoriy == 33)
		{
			num = 80;
			num2 = 80;
		}
		icon.Width = (double)num;
		icon.Height = (double)num2;
	}

	public void SetExcellenceStat(GGoodIcon icon, int iCategoriy)
	{
		if (iCategoriy == 37 || iCategoriy == 38 || iCategoriy == 31)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(83f, 129f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xiongjia");
		}
		else if (iCategoriy == 36 || iCategoriy == 35)
		{
			icon.BackgroundSprite1.transform.localScale = new Vector3(53f, 53f);
			this.SetExcellence(icon, "ZhuoyueFlowLight_xianglian");
		}
		else if (iCategoriy == 30 || iCategoriy == 32 || iCategoriy == 34 || iCategoriy == 33)
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
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (zhuoyueAttributeCount > 0)
		{
			if (zhuoyueAttributeCount >= 6)
			{
				if (icon.TeXiao._Sprite != null)
				{
					icon.TeXiao._Sprite.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString(zhuoyueTeXiaoPrefab, true));
					icon.TeXiao.gameObject.SetActive(true);
				}
			}
			else if (zhuoyueAttributeCount < 3)
			{
				icon.BackSpriteName1 = "iconState_zuoyue";
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount < 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue1";
			}
			else if (zhuoyueAttributeCount == 5)
			{
				icon.BackSpriteName1 = "iconState_zuoyue2";
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

	private const int JieZhiYou = 136;

	public TextBlock SName;

	public TextBlock Level;

	public TextBlock QuFuName;

	public TextBlock LevelWord;

	public TextBlock QuFuNameWord;

	public TextBlock FamilyName = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public TextBlock FamilyNameWord = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public TextBlock VipLevel;

	public TextBlock zhanDouLitxt;

	public TextBlock Title;

	public TextBlock RoleIDText;

	public GButton ReturnBtn;

	public GButton HaoYouBtn;

	public GButton SiLiaoBtn;

	public GButton HeiMingDanBtn;

	public GButton ZuDuiBtn;

	public GButton ChouRenBtn;

	public GButton JiaoYiBtn;

	public GButton TeamInviteBtn;

	public Dictionary<int, SpriteSL> equipIcon = new Dictionary<int, SpriteSL>();

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	public SpriteSL toukuiIcon;

	public SpriteSL wuqizuoIcon;

	public SpriteSL wuqiyouIcon;

	public SpriteSL xianglianIcon;

	public SpriteSL kaijiaIcon;

	public SpriteSL hushouIcon;

	public SpriteSL jiezhizuoIcon;

	public SpriteSL jiezhiyouIcon;

	public SpriteSL hutuiIcon;

	public SpriteSL xueziIcon;

	public ShowNetImage TouXingImage;

	private static GChildWindow DuiBiWindow;

	private List<GoodsData> usingGoodsList = new List<GoodsData>();

	public string[] DataFields;

	private int CurrentTaoZhuangIndex = -1;

	private int CurrentZhuoYueIndex = -1;

	private int CurrentChengJiuLevel = -1;

	private int CurrentJunXianLevel = -1;
}
