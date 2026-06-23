using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallShangJiaPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnCommit.Text = Global.GetLang("上架");
		this.m_LblJiage.text = Global.GetLang("输入收益钻石");
		this.m_LblNum.text = Global.GetLang("输入金币数");
		this.m_InputJiaGe.transform.localPosition = new Vector3(-70f, -47f, 0f);
		this.m_DiamondSprite.transform.localPosition = new Vector3(85f, -109f, 0f);
		this.m_LblNum.text = "0";
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnProc();
	}

	private void InputChange(GameObject gameobject, string text)
	{
		UILabel componentInChildren = gameobject.GetComponentInChildren<UILabel>();
		if (null != componentInChildren)
		{
			string text2 = componentInChildren.text;
			string[] array = text2.Split(new char[]
			{
				'|'
			});
			text2 = array[0];
			if (string.Empty == text2 || "|" == text2)
			{
				return;
			}
			int num = 0;
			if (!int.TryParse(text2, ref num))
			{
				return;
			}
			this.m_nSalePrice = (long)num;
		}
	}

	private void InputNum(GameObject gameobject, string text)
	{
		UILabel componentInChildren = gameobject.GetComponentInChildren<UILabel>();
		string text2 = componentInChildren.text;
		string[] array = text2.Split(new char[]
		{
			'|'
		});
		int num = 0;
		if (int.TryParse(array[0], ref num))
		{
			this.m_nSaleNum = (long)num;
			this.SetNumBtnState();
			return;
		}
	}

	public void SetBtnState(int nType)
	{
		this.m_nWindowType = nType;
		if (this.m_nWindowType == 1)
		{
			this.m_BtnCommit.Label.text = Global.GetLang("下架");
			this.m_LblTitle.text = Global.GetLang("物品下架");
			this.m_LblNum.text = Convert.ToString(this.m_GoodsData.GCount);
			this.m_LblJiage.text = Convert.ToString(this.m_GoodsData.SaleYuanBao);
		}
		else
		{
			this.m_LblTitle.text = Global.GetLang("物品上架");
		}
		if ((this.m_nWindowType != 0 && this.m_nWindowType != 1) || 1 >= this.m_GoodsData.GCount || this.m_nWindowType == 1)
		{
		}
		if (null != this.m_BtnCommit)
		{
			this.m_BtnCommit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.m_nWindowType == 0 || this.m_nWindowType == 2)
				{
					if (this.CheckCanAddGoods())
					{
						if (this.m_nWindowType == 0)
						{
							GameInstance.Game.SpriteSaleGoods(this.m_GoodsData.Id, -1, 1000, 0, 0, (int)this.m_nSaleNum);
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 3
							});
						}
						else if (this.m_nWindowType == 2)
						{
							GameInstance.Game.SpriteMarketSaleMoneyCmd2((int)this.m_nSaleNum, (int)this.m_nSalePrice);
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 3
							});
						}
					}
				}
				else
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.m_GoodsData.GoodsID);
					if (goodsXmlNodeByID != null && Global.IsRebornGood(goodsXmlNodeByID))
					{
						if (!Global.IsRebornBagFull())
						{
							this.DPSelectedItem(this, new DPSelectedItemEventArgs
							{
								ID = 4
							});
							GameInstance.Game.SpriteSaleGoods(this.m_GoodsData.Id, 15000, 0, 0, 0, -1);
							return;
						}
						this.ShowMsg(Global.GetLang("重生背包已满，清理出足够空间后再操作！"));
					}
					if (Global.IsBagFull())
					{
						this.ShowMsg(Global.GetLang("背包已满，清理出足够空间后再操作！"));
					}
					else
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 4
						});
						GameInstance.Game.SpriteSaleGoods(this.m_GoodsData.Id, 0, 0, 0, 0, -1);
					}
				}
			};
		}
	}

	public void SetNum(bool b)
	{
		string text = this.m_LblNum.text;
		if (string.Empty == text)
		{
			text = "0";
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		text = array[0];
		long num = Convert.ToInt64(text);
		if (b)
		{
			num += 1L;
			if (this.m_nWindowType == 2 && num >= (long)Global.Data.roleData.YinLiang)
			{
				num = (long)Global.Data.roleData.YinLiang;
			}
		}
		else
		{
			num -= 1L;
			if (0L >= num)
			{
				num = 0L;
			}
		}
		this.m_nSaleNum = num;
		this.m_LblNum.text = Convert.ToString(num);
		this.SetNumBtnState();
	}

	private void SetNumBtnState()
	{
		if (1L >= this.m_nSaleNum)
		{
		}
		if (this.m_nWindowType == 2)
		{
			if (this.m_nSaleNum >= (long)Global.Data.roleData.YinLiang)
			{
				this.m_LblNum.text = Convert.ToString(Global.Data.roleData.YinLiang);
				this.m_nSaleNum = Convert.ToInt64(Global.Data.roleData.YinLiang);
			}
		}
		else if (this.m_nSaleNum >= (long)this.m_GoodsData.GCount)
		{
			this.m_LblNum.text = Convert.ToString(this.m_GoodsData.GCount);
			this.m_nSaleNum = Convert.ToInt64(this.m_GoodsData.GCount);
		}
	}

	private void InputNumOnClick(GameObject obj)
	{
		this.ShuLiangDPS = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			if (this.m_nWindowType == 2)
			{
				if (id >= Global.Data.roleData.YinLiang)
				{
					this.m_LblNum.text = Convert.ToString(Global.Data.roleData.YinLiang);
					this.m_nSaleNum = Convert.ToInt64(Global.Data.roleData.YinLiang);
				}
			}
			else if (id >= this.m_GoodsData.GCount)
			{
				this.m_LblNum.text = Convert.ToString(this.m_GoodsData.GCount);
				this.m_nSaleNum = Convert.ToInt64(this.m_GoodsData.GCount);
			}
		};
		PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.ShuLiangDPS, this.m_LblNum, 0, -100);
	}

	private void InputJiaGeOnClick(GameObject obj)
	{
		this.JiaGeDPS = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			if (id < 2)
			{
				this.m_LblJiage.text = "2";
			}
		};
		PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.JiaGeDPS, this.m_LblJiage, 0, -100);
	}

	public void AddNum()
	{
		int num = 0;
		if (this.m_nWindowType == 2)
		{
			num = Global.Data.roleData.YinLiang;
		}
		else if (this.m_nWindowType == 0)
		{
			num = this.m_GoodsData.GCount;
		}
		int length = this.m_LblNum.ToString().Length;
		this.m_nSaleNum = Math.Min(this.m_nSaleNum + (long)((int)Math.Pow(10.0, (double)(length - 1))), (long)num);
		this.m_LblNum.text = this.m_nSaleNum.ToString();
	}

	public void SubNum()
	{
		int length = this.m_LblNum.ToString().Length;
		this.m_nSaleNum = Math.Max(this.m_nSaleNum - (long)((int)Math.Pow(10.0, (double)Math.Max(length - 2, 0))), 1L);
		this.m_LblNum.text = this.m_nSaleNum.ToString();
	}

	private void OnPressAdd(GameObject go, bool state)
	{
		if (state)
		{
			base.InvokeRepeating("AddSaleNum", 0.1f, 0.1f);
		}
		else
		{
			base.CancelInvoke("AddSaleNum");
		}
	}

	private void OnPressSub(GameObject go, bool state)
	{
		if (state)
		{
			base.InvokeRepeating("SubSaleNum", 0.1f, 0.1f);
		}
		else
		{
			base.CancelInvoke("SubSaleNum");
		}
	}

	private void AddSaleNum()
	{
		int num = 0;
		if (this.m_nWindowType == 2)
		{
			num = Global.Data.roleData.YinLiang;
		}
		else if (this.m_nWindowType == 0)
		{
			num = this.m_GoodsData.GCount;
		}
		int length = this.m_LblNum.text.Length;
		this.m_nSaleNum = Math.Min(this.m_nSaleNum + (long)((int)Math.Pow(10.0, (double)(length - 1))), (long)num);
		this.m_LblNum.text = this.m_nSaleNum.ToString();
	}

	private void SubSaleNum()
	{
		if (this.m_nWindowType == 2)
		{
			int num = Global.Data.roleData.YinLiang;
		}
		else if (this.m_nWindowType == 0)
		{
			int num = this.m_GoodsData.GCount;
		}
		int length = this.m_LblNum.text.Length;
		this.m_nSaleNum = Math.Max(this.m_nSaleNum - (long)((int)Math.Pow(10.0, (double)Math.Max(length - 2, 0))), 1L);
		this.m_LblNum.text = this.m_nSaleNum.ToString();
	}

	private void InitBtnProc()
	{
		UIEventListener.Get(this.m_InputJiaGe).onClick = new UIEventListener.VoidDelegate(this.InputJiaGeOnClick);
		UIEventListener.Get(this.m_InputNum).onClick = new UIEventListener.VoidDelegate(this.InputNumOnClick);
		this.m_BtnAdd.OnPress = new UIEventListener.BoolDelegate(this.OnPressAdd);
		this.m_BtnSub.OnPress = new UIEventListener.BoolDelegate(this.OnPressSub);
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		if (null != this.m_BtnSub)
		{
			this.m_BtnSub.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetNum(false);
			};
		}
		if (null != this.m_BtnAdd)
		{
			this.m_BtnAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.SetNum(true);
			};
		}
	}

	private int CheckNum(string strCheckText)
	{
		if (string.Empty == strCheckText)
		{
			return -1;
		}
		string[] array = strCheckText.Split(new char[]
		{
			'|'
		});
		int result = 0;
		if (int.TryParse(array[0], ref result))
		{
			return result;
		}
		return -1;
	}

	private bool CheckCanAddGoods()
	{
		this.m_nSalePrice = (long)this.CheckNum(this.m_LblJiage.text);
		this.m_nSaleNum = (long)this.CheckNum(this.m_LblNum.text);
		if (Global.Data.SaleGoodsDataList != null && Global.Data.SaleGoodsDataList.Count >= 16)
		{
			this.ShowMsg(Global.GetLang("摊位栏已满，无法上架物品。"));
			return false;
		}
		if (0L >= this.m_nSaleNum)
		{
			this.ShowMsg(Global.GetLang("请输入正确的货物数量"));
			return false;
		}
		if (this.m_nWindowType != 2 && this.m_nSaleNum > (long)this.m_GoodsData.GCount)
		{
			this.ShowMsg(Global.GetLang("输入货物数量超过实际数量"));
			return false;
		}
		if (0L >= this.m_nSalePrice)
		{
			this.ShowMsg(Global.GetLang("请输入正确的出售价格"));
			return false;
		}
		if (0 < this.m_GoodsData.Binding)
		{
			this.ShowMsg(Global.GetLang("请勿上架绑定物品"));
			return false;
		}
		if (this.m_nWindowType == 2 && this.m_nSaleNum > (long)Global.Data.roleData.YinLiang)
		{
			this.ShowMsg(Global.GetLang("上架金币数量超过自身金币携带数量"));
			return false;
		}
		return true;
	}

	private void ShowMsg(string strMsg)
	{
		string[] buttons = new string[]
		{
			Global.GetLang("确定")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang(strMsg), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID != 0)
		{
			if (args.ID == 1)
			{
			}
		}
	}

	public string GetGoodsColor(GoodsData goodsData)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(this.m_GoodsData.GoodsID);
		string result = string.Empty;
		if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
		{
			result = this.GetZhuangBeiNameColor(this.m_GoodsData);
		}
		else
		{
			result = this.GetXiaoHaoWuPinClolor(this.m_GoodsData);
		}
		return result;
	}

	private string GetZhuangBeiNameColor(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		string text = "FFFFFF";
		string text2 = string.Empty;
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
		{
			text = "00FF00";
		}
		else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
		{
			text = "0099FF";
		}
		else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
		{
			text = "FF08FF";
		}
		text2 += goodsXmlNodeByID.Title;
		return Global.GetColorStringForNGUIText(new object[]
		{
			text,
			text2
		});
	}

	private string GetXiaoHaoWuPinClolor(GoodsData goodsData)
	{
		string empty = string.Empty;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		return Global.GetColorStringForNGUIText(new object[]
		{
			goodsXmlNodeByID.GoodsColor,
			goodsXmlNodeByID.Title
		});
	}

	public GGoodIcon icon
	{
		get
		{
			return null;
		}
		set
		{
			U3DUtils.AddChild(this.m_GameObjIcon.gameObject, this.icon.gameObject, true);
		}
	}

	public string Title
	{
		set
		{
			if (null != this.m_LblWuPinName)
			{
				this.m_LblWuPinName.text = value;
			}
		}
	}

	public GoodsData goodsdata
	{
		get
		{
			return this.m_GoodsData;
		}
		set
		{
			if (value == null)
			{
				GoodsData dummyGoodsData = Global.GetDummyGoodsData(50200);
				this.m_GoodsData = dummyGoodsData;
			}
			else
			{
				this.m_GoodsData = value;
			}
			GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(this.m_GoodsData, false);
			if (null != goodsItemIcon && null != this.m_GameObjIcon)
			{
				U3DUtils.AddChild(this.m_GameObjIcon.gameObject, goodsItemIcon.gameObject, true);
				this.Title = this.GetGoodsColor(value);
			}
			this.SetNumBtnState();
		}
	}

	private GGoodIcon GetGoodsItemIcon(GoodsData goodsData, bool isDrag = false)
	{
		if (goodsData == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = goodsData.GoodsID;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(goodsData.GoodsID, false);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			ggoodIcon.SecondText.Text = string.Empty;
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowGoodsTip(s);
		};
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		if (null == ggoodIcon)
		{
			return;
		}
		string text = Convert.ToString(this.m_GoodsData.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnClose;

	public GButton m_BtnSub;

	public GButton m_BtnAdd;

	public GButton m_BtnCommit;

	public GameObject m_InputJiaGe;

	public GameObject m_InputNum;

	public UILabel m_LblJiage;

	public UILabel m_LblNum;

	public UILabel m_LblWuPinName;

	public UILabel m_LblTitle;

	public UILabel[] ConstTexts;

	public DPSelectedItemEventHandler JiaGeDPS;

	public DPSelectedItemEventHandler ShuLiangDPS;

	private long m_nSaleNum = 1L;

	private long m_nSalePrice = 1L;

	public int m_nWindowType;

	public GameObject m_GameObjIcon;

	public GoodsData m_GoodsData;

	public Transform m_DiamondSprite;
}
