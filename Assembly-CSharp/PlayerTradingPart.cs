using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class PlayerTradingPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.JiaoYiBtn.Text = Global.GetLang("交  易");
		this.SuoDingBtn.Text = Global.GetLang("锁  定");
		this.myName.Text = Global.GetLang("我的交易");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.myJBText.TextChanged = new EventHandler(this.Input_myJBText_EventHandler);
		this.myZSText.TextChanged = new EventHandler(this.Input_myZSText_EventHandler);
		this.ItemCollection1 = this.otherlistBox.ItemsSource;
		this.ItemCollection2 = this.mylistBox.ItemsSource;
		this.SuoDingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.LockExchange)
			{
				return;
			}
			if (this.myJBText.Text.Trim() == string.Empty)
			{
				this.myJBText.Text = "0";
			}
			if (Convert.ToInt32(this.myJBText.Text.Trim()) > 0)
			{
				GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 7, Convert.ToInt32(this.myJBText.Text.Trim()));
			}
			if (this.myZSText.Text.Trim() == string.Empty)
			{
				this.myZSText.Text = "0";
			}
			if (Convert.ToInt32(this.myZSText.Text.Trim()) > 0)
			{
				GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 8, Convert.ToInt32(this.myZSText.Text.Trim()));
			}
			this.LockExchange = true;
			this.SuoDingBtn.isEnabled = false;
			GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 9, Global.Data.ExchangeID);
		};
		this.JiaoYiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.JiaoYiBtn.enabled)
			{
				return;
			}
			if (this.IsAllLocked())
			{
				this.JiaoYiBtn.isEnabled = false;
				this.JiaoYiBtn.enabled = false;
				this.DoneExchange = true;
				GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 11, Global.Data.ExchangeID);
			}
			else
			{
				Super.HintMainText(Global.GetLang("请确认交易双方都已锁定物品，未锁定之前不能交易。"), 10, 3);
			}
		};
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = -1
				});
			}
		};
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = -1
			});
		}
	}

	public override void Destroy()
	{
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.LoadParcelPart(false);
		if (Global.Data.OtherRoles.ContainsKey(this.OtherRoleID))
		{
			RoleData value = Global.Data.OtherRoles.GetValue(this.OtherRoleID);
			this.otherPlayerName.Text = Global.FormatRoleName(value);
		}
	}

	private void LoadParcelPart(bool beReborn)
	{
		ParcelPart parcelPart = null;
		if (beReborn)
		{
			parcelPart = Super._ParcelRebornPart;
		}
		else
		{
			parcelPart = Super._ParcelPart;
		}
		if (parcelPart == null)
		{
			parcelPart = U3DUtils.NEW<ParcelPart>();
			parcelPart.iBaoGuoMode = 3;
			this.BaoGuoCanvas.Children.Add(parcelPart);
			if (beReborn)
			{
				parcelPart.name = "ParcelRebornPart";
				Super._ParcelRebornPart = parcelPart;
			}
			else
			{
				Super._ParcelPart = parcelPart;
			}
			parcelPart.IsRebornParcel = beReborn;
		}
		parcelPart.InitPartData();
		parcelPart.iBaoGuoMode = 3;
		parcelPart.Visibility = true;
		parcelPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == -1 && e.IDType == 3 && e.Tag != null)
			{
				GoodsData goodsData = e.Tag as GoodsData;
				if (!this.LockExchange)
				{
					if (this.ItemCollection2.Count < 6)
					{
						if (goodsData.Binding <= 0 && !Global.IsTimeLimitGoods(goodsData))
						{
							GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 5, goodsData.Id);
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("交易物品数量已达上限,无法放入!"), 10, 3);
					}
				}
			}
			if (e.ID == 3 && e.IDType == 3)
			{
				if (parcelPart.IsRebornParcel)
				{
					parcelPart.gameObject.SetActive(false);
					this.LoadParcelPart(false);
				}
				else
				{
					parcelPart.gameObject.SetActive(false);
					this.LoadParcelPart(true);
				}
			}
			return true;
		};
	}

	private bool IsAllLocked()
	{
		if (Global.Data.ExchangeDataItem == null)
		{
			return false;
		}
		if (Global.Data.ExchangeDataItem.LockDict == null)
		{
			return false;
		}
		int num = 0;
		if (Global.Data.ExchangeDataItem.LockDict.ContainsKey(Global.Data.roleData.RoleID) || num > 0)
		{
			num = Global.Data.ExchangeDataItem.LockDict.GetValue(Global.Data.roleData.RoleID);
			this.myMask.gameObject.SetActive(true);
			this.myJBText.enabled = false;
			this.myZSText.enabled = false;
		}
		bool flag = num > 0;
		int num2 = (Global.Data.roleData.RoleID != Global.Data.ExchangeDataItem.RequestRoleID) ? Global.Data.ExchangeDataItem.RequestRoleID : Global.Data.ExchangeDataItem.AgreeRoleID;
		num = 0;
		if (Global.Data.ExchangeDataItem.LockDict.ContainsKey(num2) || num > 0)
		{
			num = Global.Data.ExchangeDataItem.LockDict.GetValue(num2);
			this.otherMask.gameObject.SetActive(true);
		}
		return flag && num > 0;
	}

	private void AddOtherGoodsItem(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 64.0;
		icon.Height = 64.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty)
		}), false, 0);
		icon.TipType = 1;
		icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			gd.GoodsID,
			0,
			gd.Id,
			3
		});
		icon.ItemCategory = goodsXmlNodeByID.Categoriy;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 8;
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
		this.ItemCollection1.Add(icon);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.OtherRole, gd);
		};
	}

	private void AddMyGoodsItem(int index, GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 64.0;
		icon.Height = 64.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty)
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCategory = goodsXmlNodeByID.Categoriy;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 8;
		bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
		Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.LockExchange)
			{
				GoodsData goodData = icon.ItemObject as GoodsData;
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SelfPet, goodData);
			}
		};
		icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
			if (ev.IDType == 6 && !this.LockExchange)
			{
				GoodsData goodsData = icon.ItemObject as GoodsData;
				if (Global.CanAddGoods(goodsData.GoodsID, goodsData.GCount, goodsData.Binding, gd.Endtime, true))
				{
					GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 6, goodsData.Id);
				}
				else
				{
					Super.HintMainText(Global.GetLang("背包已满，请先清理出空闲位置后，再存放物品!"), 10, 3);
				}
			}
		};
		this.ItemCollection2.Add(icon);
	}

	private void ClearAllMyText()
	{
		for (int i = 0; i < 6; i++)
		{
		}
	}

	private void SetMyText(int index, string text)
	{
	}

	public void RefreshExchangeData()
	{
		if (Global.Data.ExchangeDataItem == null)
		{
			return;
		}
		this.ItemCollection2.Clear();
		if (Global.Data.ExchangeDataItem.GoodsDict != null && Global.Data.ExchangeDataItem.GoodsDict.ContainsKey(Global.Data.roleData.RoleID))
		{
			List<GoodsData> value = Global.Data.ExchangeDataItem.GoodsDict.GetValue(Global.Data.roleData.RoleID);
			if (value != null)
			{
				for (int i = 0; i < value.Count; i++)
				{
					this.AddMyGoodsItem(i, value[i]);
				}
			}
		}
		this.ItemCollection1.Clear();
		int num = (Global.Data.ExchangeDataItem.RequestRoleID != Global.Data.roleData.RoleID) ? Global.Data.ExchangeDataItem.RequestRoleID : Global.Data.ExchangeDataItem.AgreeRoleID;
		if (Global.Data.ExchangeDataItem.GoodsDict != null && Global.Data.ExchangeDataItem.GoodsDict.ContainsKey(num))
		{
			List<GoodsData> value = Global.Data.ExchangeDataItem.GoodsDict.GetValue(num);
			if (value != null)
			{
				for (int j = 0; j < value.Count; j++)
				{
					this.AddOtherGoodsItem(value[j]);
				}
			}
		}
		if (Global.Data.ExchangeDataItem.MoneyDict != null && Global.Data.ExchangeDataItem.MoneyDict.ContainsKey(num))
		{
			int value2 = Global.Data.ExchangeDataItem.MoneyDict.GetValue(num);
			this.otherJBText.Text = StringUtil.substitute("{0}", new object[]
			{
				value2
			});
		}
		if (Global.Data.ExchangeDataItem.YuanBaoDict != null && Global.Data.ExchangeDataItem.YuanBaoDict.ContainsKey(num))
		{
			int value2 = Global.Data.ExchangeDataItem.YuanBaoDict.GetValue(num);
			this.otherZSText.Text = StringUtil.substitute("{0}", new object[]
			{
				value2
			});
		}
		if (this.IsAllLocked() && !this.DoneExchange)
		{
			this.JiaoYiBtn.enabled = true;
		}
	}

	public void DragDrop(object sender, object e)
	{
		if (this.mylistBox != Super.GData.DragingListBox && null != Super.GData.DragingItem && Super.GData.DragingItem.BoxTypes == 1)
		{
			GoodsData goodsData = Super.GData.DragingItem.ItemObject as GoodsData;
			if (goodsData.Using <= 0 && !this.LockExchange)
			{
				if (this.ItemCollection2.Length < 6)
				{
					if (goodsData.Binding <= 0)
					{
						GameInstance.Game.SpriteGoodsExchange(this.OtherRoleID, 5, goodsData.Id);
					}
					else
					{
						Super.HintMainText(Global.GetLang("绑定物品不允许交易!"), 10, 3);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("交易物品数量已达上限,无法放入!"), 10, 3);
				}
			}
		}
	}

	private void Input_myJBText_EventHandler(object sender, EventArgs e)
	{
		if (this.LockExchange)
		{
			return;
		}
		string text = this.myJBText.Text.Trim();
		if (text == string.Empty)
		{
			this.myJBText.Text = "0";
			return;
		}
		string text2 = string.Empty;
		string text3 = text;
		for (int i = 0; i < text3.Length; i++)
		{
			char c = text3.get_Chars(i);
			if (char.IsDigit(c))
			{
				text2 += c;
			}
		}
		int num = 0;
		try
		{
			num = Convert.ToInt32(text2);
			if (num >= Global.Data.roleData.YinLiang)
			{
				num = Global.Data.roleData.YinLiang;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 0;
		}
		this.myJBText.Text = num.ToString();
	}

	private void Input_myZSText_EventHandler(object sender, EventArgs e)
	{
		if (this.LockExchange)
		{
			return;
		}
		string text = this.myZSText.Text.Trim();
		if (text == string.Empty)
		{
			this.myZSText.Text = "0";
			return;
		}
		string text2 = string.Empty;
		string text3 = text;
		for (int i = 0; i < text3.Length; i++)
		{
			char c = text3.get_Chars(i);
			if (char.IsDigit(c))
			{
				text2 += c;
			}
		}
		int num = 0;
		try
		{
			num = Convert.ToInt32(text2);
			if (num >= Global.Data.roleData.UserMoney)
			{
				num = Global.Data.roleData.UserMoney;
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
			num = 0;
		}
		this.myZSText.Text = num.ToString();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton SuoDingBtn;

	public GButton JiaoYiBtn;

	public GButton CloseBtn;

	public UISprite myMask;

	public UISprite otherMask;

	public ListBox mylistBox;

	public ListBox otherlistBox;

	public TextBlock otherPlayerName;

	public TextBlock otherJBText;

	public TextBlock otherZSText;

	public TextBox myJBText;

	public TextBox myZSText;

	public TextBlock myName;

	public Canvas BaoGuoCanvas;

	public int OtherRoleID = -1;

	public bool LockExchange;

	private bool DoneExchange;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	public ObservableCollection ItemCollection1;

	public ObservableCollection ItemCollection2;
}
