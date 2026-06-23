using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class JiFenHuiShouPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.TypeText.Text = Global.GetLang("魔晶收益:");
		this.Items = this.goodsListBox.Items;
		this.FangRuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (ObjectClickGetingMgr.IsType(18))
			{
				ObjectClickGetingMgr.CancelClickGetThing(18);
				this.FangRuBtn.Text = Global.GetLang("批量操作");
				this.selectImage.Visibility = false;
				this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
				Super._ParcelPart.isShowTips = true;
			}
			else
			{
				this.FangRuBtn.Text = Global.GetLang("取消操作");
				ObjectClickGetingMgr.StartClickGetThing(18, e);
				this.selectImage.Visibility = true;
				this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
				Super._ParcelPart.isShowTips = false;
			}
		};
		this.ChuShouBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Items.Count <= 0)
			{
				return;
			}
			string text = string.Empty;
			for (int i = 0; i < this.Items.Count; i++)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
				if (goodsData != null)
				{
					if (text.Length > 0)
					{
						text += ",";
					}
					text += goodsData.Id;
				}
			}
			if (text != string.Empty)
			{
				GameInstance.Game.SpriteOneKeyQuickSaleOut(2, text);
				this.Items.Clear();
				Super.goodDBIdDict.Clear();
				this.JiFenNumText.Text = "0";
			}
			SystemHelpMgr.OnAction(UIObjIDs.JiFenHuishowSubmit, HelpStateEvents.Clicked, -1);
		};
		this.returnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				Super.goodDBIdDict.Clear();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				GGoodIcon ggoodIcon = Super._ParcelPart.LocationGoodsIcon(state, true);
				if (null != ggoodIcon)
				{
					SystemHelpPart.SetMask(ggoodIcon, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
				}
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.ChuShouBtn, default(Vector4));
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.returnBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		SystemHelpMgr.OnAction(UIObjIDs.JiFenHuishowPart, HelpStateEvents.Inactived, -1);
	}

	public void InitPartSize(int width, int height)
	{
		this.goodsListBox.AutoCollider = true;
	}

	public void InitPartData()
	{
		if (Super._ParcelPart == null)
		{
			ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
			parcelPart.iBaoGuoMode = 5;
			parcelPart.InitPartData();
			Super._ParcelPart = parcelPart;
		}
		Super._ParcelPart.iBaoGuoMode = 5;
		Super._ParcelPart.Visibility = true;
		this.baoGuoCanvas.Children.Add(Super._ParcelPart);
		Super._ParcelPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == -1 && e.IDType == 5 && e.Tag != null)
			{
				this.AddHuiShouGoods(e.Tag as GoodsData);
			}
			return true;
		};
		this.SetPrice();
		ObjectClickGetingMgr.StartClickGetThing(18, null);
		this.FangRuBtn.Text = Global.GetLang("取消操作");
		this.selectImage.Visibility = true;
		this.selectBaoGuoImage.Visibility = this.selectImage.Visibility;
		Super._ParcelPart.isShowTips = false;
		SystemHelpMgr.OnAction(UIObjIDs.JiFenHuishowPart, HelpStateEvents.Actived, -1);
	}

	public void CleanUpChildWindows()
	{
	}

	public void GetNewData()
	{
	}

	private void SetPrice()
	{
		this.BangdingTongqianNum = 0;
		for (int i = 0; i < this.goodsListBox.Count(); i++)
		{
			GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
			this.BangdingTongqianNum += Global.GetGoodsSaleToNpJiFen(goodsData);
		}
		this.JiFenNumText.Text = this.BangdingTongqianNum.ToString();
	}

	private void InitDict()
	{
		for (int i = 0; i < this.YaopinGoodsID.Length; i++)
		{
			if (!this.GoodsIDDict.ContainsKey(this.YaopinGoodsID[i]))
			{
				this.GoodsIDDict[this.YaopinGoodsID[i]] = 0;
			}
		}
	}

	private void LoadAllWhiteEquip()
	{
		this.Items.Clear();
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (goodsData.GCount > 0)
				{
					if (goodsData.Using <= 0)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
						if (goodsXmlNodeByID != null)
						{
							if (goodsXmlNodeByID.ChangeJinYuan != 0)
							{
								if (goodsXmlNodeByID.Categoriy >= 25 || goodsData.ExcellenceInfo > 0)
								{
									if (Global.GetGoodsSaleToNpJiFen(goodsData) > 0)
									{
										if (Super.CanSaleOutGoods(goodsData))
										{
											if (this.Items.Count >= 24)
											{
												break;
											}
											this.AddGGoodIcon(goodsData);
											if (this.Items.Count > 100)
											{
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		this.Items.DelayUpdate();
	}

	private void AddGGoodIcon(GoodsData goodsData)
	{
		GGoodIcon icon = null;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			SystemHelpMgr.OnAction(UIObjIDs.JiFenHuishowPutGoods, HelpStateEvents.Clicked, goodsData.GoodsID);
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			int categoriy = goodsXmlNodeByID.Categoriy;
			try
			{
				icon = U3DUtils.NEW<GGoodIcon>();
				icon.Width = 78.0;
				icon.Height = 78.0;
				icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				icon.TipType = 1;
				icon.ItemCategory = categoriy;
				icon.ItemCode = goodsData.GoodsID;
				icon.ItemObject = goodsData;
				icon.TextShadowColor = 4278190080U;
				icon.BoxTypes = 1;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			this.Items.Add(icon.gameObject);
			if (!Super.goodDBIdDict.ContainsKey(goodsData.Id))
			{
				Super.goodDBIdDict.Add(goodsData.Id, goodsData.GoodsID);
			}
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, goodsData, canUse, IconTextTypes.Qianghua);
			Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), true);
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.selectImage.Visibility)
				{
					Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
					this.Items.Remove(icon.gameObject);
					this.SetPrice();
					if (Super.goodDBIdDict.ContainsKey(goodsData.Id))
					{
						Super.goodDBIdDict.Remove((icon.ItemObject as GoodsData).Id);
					}
				}
				else
				{
					GoodsData goodData = icon.ItemObject as GoodsData;
					GTipServiceEx.SelfBagOnly = false;
					GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SelfPet, goodData);
				}
			};
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 6)
				{
					Super.SetBgGIconShouStat(Super._ParcelPart.FindGoodsIconByDbID((icon.ItemObject as GoodsData).Id), false);
					this.Items.Remove(icon.gameObject);
					this.SetPrice();
					if (Super.goodDBIdDict.ContainsKey(goodsData.Id))
					{
						Super.goodDBIdDict.Remove((icon.ItemObject as GoodsData).Id);
					}
				}
			};
		}
	}

	private void AddHuiShouGoods(GoodsData gd)
	{
		if (gd != null)
		{
			if (gd.ExcellenceInfo <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (Global.GetGoodsSaleToNpJiFen(gd) <= 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (this.FindGoodDataByID(gd.Id))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("物品已存在"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (this.goodsListBox.Count() >= 24)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("回收空间已经满，请先回收后，再进行添加操作！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			if (goodsXmlNodeByID.ChangeJinYuan == 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (goodsXmlNodeByID.Categoriy >= 25)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该物品无法进行回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (gd.ExcellenceInfo == 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("只有卓越类装备才能回收！"), new object[0]), 0, -1, -1, 0);
				return;
			}
			if (((goodsXmlNodeByID.SuitID >= 5 && Global.GetZhuoyueAttributeCount(gd) >= 2) || (goodsXmlNodeByID.SuitID < 5 && Global.GetZhuoyueAttributeCount(gd) >= 4) || gd.Forge_level >= 7 || gd.AppendPropLev >= 20) && !this.spList.Contains(gd.GoodsID))
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("此装备比较贵重，是否确认回收？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(this.Container, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.AddGGoodIcon(gd);
						this.Items.DelayUpdate();
						this.SetPrice();
					}
					return true;
				};
			}
			else
			{
				this.AddGGoodIcon(gd);
				this.Items.DelayUpdate();
				this.SetPrice();
			}
		}
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		ClickGetThingEventArgs clickGetThingEventArgs = evt.Tag as ClickGetThingEventArgs;
		if (clickGetThingEventArgs.ClickGetThingType != 18)
		{
			return;
		}
		if (clickGetThingEventArgs.Cancel)
		{
			return;
		}
		object sender = clickGetThingEventArgs.sender;
		GGoodIcon ggoodIcon = sender as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData != null)
		{
			if (!Super.CanSaleOutGoods(goodsData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("此物品不可出售"), new object[0]), 0, -1, -1, 0);
			}
			else if (!this.FindGoodDataByID(goodsData.Id))
			{
				if (this.Items.Count < 24)
				{
					this.AddGGoodIcon(goodsData);
					this.Items.DelayUpdate();
					this.SetPrice();
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("一键出售的空间已经满，请先出售后，再进行添加操作"), new object[0]), 0, -1, -1, 0);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("物品已存在"), new object[0]), 0, -1, -1, 0);
			}
		}
		clickGetThingEventArgs.NextClick = true;
	}

	private bool FindGoodDataByID(int id)
	{
		if (this.goodsListBox.Count() <= 0)
		{
			return false;
		}
		for (int i = 0; i < this.goodsListBox.Count(); i++)
		{
			GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.Items[i]).ItemObject as GoodsData;
			if (goodsData != null)
			{
				if (goodsData.Id == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private int BangdingTongqianNum;

	public TextBlock JiFenNumText;

	public TextBlock TypeText;

	public ListBox goodsListBox;

	public GButton FangRuBtn;

	public GButton returnBtn;

	public GButton ChuShouBtn;

	public SpriteSL selectImage;

	public SpriteSL selectBaoGuoImage;

	private ObservableCollection Items;

	public Canvas baoGuoCanvas;

	private int[] YaopinGoodsID = new int[]
	{
		20000,
		20001,
		20002,
		20003,
		20004,
		20005,
		20006,
		20007,
		20010,
		20011,
		20012,
		20013,
		20014,
		20015,
		20016,
		20017,
		20020,
		20021,
		20022,
		20023,
		1000040,
		1100040,
		1200040
	};

	private Dictionary<int, int> GoodsIDDict = new Dictionary<int, int>();

	private List<int> spList = new List<int>();
}
