using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZiYuanZhaoHuiPopPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnOK.Text = Global.GetLang("确定");
		this.m_BtnCancel.Text = Global.GetLang("取消");
		this.m_Description2.text = Global.GetLang("可找回资源:");
		this.MoneyTypeNetImage.transform.localPosition = new Vector3(-13f, -4f, 0f);
		this.m_ContentText.transform.localPosition = new Vector3(0f, -4f, 0f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this._ItemCollection = this.m_ListWuPin.ItemsSource;
		}
		if (null != this.m_BtnOK)
		{
			this.m_BtnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					if (this.m_MoneyType == ZiYuanZhaoHuiPart.MoneyType.Gold)
					{
						if (this.needCount > Global.Data.roleData.Money1 + Global.GetRoleOwnNumByMoneyType(8))
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
							return;
						}
					}
					else if (this.m_MoneyType == ZiYuanZhaoHuiPart.MoneyType.Diamond && this.needCount > Global.Data.roleData.UserMoney)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
						return;
					}
					GameInstance.Game.GetOldResource(this.ResourceInfo.type, (int)this.m_MoneyType, (int)this.m_ZhaoHuiModel);
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
		if (null != this.m_BtnCancel)
		{
			this.m_BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
		if (null != this.m_Close)
		{
			this.m_Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public void Refresh(OldResourceInfo resourceInfo, ZiYuanZhaoHuiPart.ZhaoHuiModel zhaoHuiModel, ZiYuanZhaoHuiPart.MoneyType moneyType, int id = -1)
	{
		if (resourceInfo != null)
		{
			this.ResourceInfo = resourceInfo;
			this.ID = id;
			this.m_MoneyType = moneyType;
			this.m_ZhaoHuiModel = zhaoHuiModel;
			this.LoadGiftItemList();
			if (moneyType == ZiYuanZhaoHuiPart.MoneyType.Gold)
			{
				this.MoneyTypeNetImage.URL = "NetImages/GameRes/Images/Unit/gold.png";
				this.MoneyTypeNetImage.ImageDownloaded = delegate(object s)
				{
					if (this.MoneyTypeNetImage.Texture != null)
					{
						this.MoneyTypeNetImage.Texture.MakePixelPerfect();
					}
				};
				this.m_Description.text = string.Format(Global.GetLang("找回{0}%的资源需消耗"), "75");
			}
			else if (moneyType == ZiYuanZhaoHuiPart.MoneyType.Diamond)
			{
				this.MoneyTypeNetImage.URL = "NetImages/GameRes/Images/Unit/diamond.png";
				this.MoneyTypeNetImage.ImageDownloaded = delegate(object s)
				{
					if (this.MoneyTypeNetImage.Texture != null)
					{
						this.MoneyTypeNetImage.Texture.MakePixelPerfect();
					}
				};
				this.m_Description.text = string.Format(Global.GetLang("找回{0}%的资源需消耗"), "100");
			}
			this.needCount = this.GetNeedCount(resourceInfo, moneyType);
			this.m_ContentText.text = this.needCount.ToString();
		}
	}

	public int GetNeedCount(OldResourceInfo resourceInfo, ZiYuanZhaoHuiPart.MoneyType moneyType)
	{
		int num = 0;
		int num2 = 0;
		if (moneyType == ZiYuanZhaoHuiPart.MoneyType.Gold)
		{
			num2 = 0;
		}
		else if (moneyType == ZiYuanZhaoHuiPart.MoneyType.Diamond)
		{
			num2 = 1;
		}
		if (resourceInfo.exp > 0)
		{
			double num3 = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuanShengExpXiShu")[Global.Data.roleData.ChangeLifeCount];
			num += (int)((double)resourceInfo.exp / (ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiExp")[num2] * num3));
		}
		if (resourceInfo.bandmoney > 0)
		{
			num += (int)((double)resourceInfo.bandmoney / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiBandGold")[num2]);
		}
		if (resourceInfo.mojing > 0)
		{
			num += (int)((double)resourceInfo.mojing / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiMoJing")[num2]);
		}
		if (resourceInfo.shengwang > 0)
		{
			num += (int)((double)resourceInfo.shengwang / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiShengWang")[num2]);
		}
		if (resourceInfo.chengjiu > 0)
		{
			num += (int)((double)resourceInfo.chengjiu / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiChengJiu")[num2]);
		}
		if (resourceInfo.zhangong > 0)
		{
			num += (int)((double)resourceInfo.zhangong / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiZhanGong")[num2]);
		}
		if (resourceInfo.bandDiamond > 0)
		{
			num += (int)((double)resourceInfo.bandDiamond / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiBindZuan")[num2]);
		}
		if (resourceInfo.xinghun > 0)
		{
			num += (int)((double)resourceInfo.xinghun / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiXingHun")[num2]);
		}
		if (resourceInfo.yuanSuFenMo > 0)
		{
			num += (int)((double)resourceInfo.yuanSuFenMo / ConfigSystemParam.GetSystemParamDoubleArrayByName("ZiYuanZhaoHuiYuanSuFenMo")[num2]);
		}
		return num;
	}

	public void LoadGiftItemList()
	{
		this.ItemCollection.Clear();
		int exp = this.ResourceInfo.exp;
		if (exp > 0)
		{
			this.AddGiftExceptGoodsIcon("ExpAward", exp);
		}
		int bandmoney = this.ResourceInfo.bandmoney;
		if (bandmoney > 0)
		{
			this.AddGiftExceptGoodsIcon("BandMoneyAward", bandmoney);
		}
		int mojing = this.ResourceInfo.mojing;
		if (mojing > 0)
		{
			this.AddGiftExceptGoodsIcon("MoJingAward", mojing);
		}
		int chengjiu = this.ResourceInfo.chengjiu;
		if (chengjiu > 0)
		{
			this.AddGiftExceptGoodsIcon("ChengJiuAward", chengjiu);
		}
		int shengwang = this.ResourceInfo.shengwang;
		if (shengwang > 0)
		{
			this.AddGiftExceptGoodsIcon("ShengWangAward", shengwang);
		}
		int zhangong = this.ResourceInfo.zhangong;
		if (zhangong > 0)
		{
			this.AddGiftExceptGoodsIcon("ZhanGongAward", zhangong);
		}
		int bandDiamond = this.ResourceInfo.bandDiamond;
		if (bandDiamond > 0)
		{
			this.AddGiftExceptGoodsIcon("BindZuanAward", bandDiamond);
		}
		int xinghun = this.ResourceInfo.xinghun;
		if (xinghun > 0)
		{
			this.AddGiftExceptGoodsIcon("XingHunAward", xinghun);
		}
		int yuanSuFenMo = this.ResourceInfo.yuanSuFenMo;
		if (yuanSuFenMo > 0)
		{
			this.AddGiftExceptGoodsIcon("YuanSuFenMo", yuanSuFenMo);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void AddGiftExceptGoodsIcon(string name, int count)
	{
		JinRiKeZuoGiftItemIcon icon = U3DUtils.NEW<JinRiKeZuoGiftItemIcon>();
		icon.NetImage.Width = 27.0;
		icon.NetImage.Height = 27.0;
		string url = string.Empty;
		if (name != null)
		{
			if (ZiYuanZhaoHuiPopPart.<>f__switch$mapC == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(9);
				dictionary.Add("ExpAward", 0);
				dictionary.Add("BandMoneyAward", 1);
				dictionary.Add("MoJingAward", 2);
				dictionary.Add("ChengJiuAward", 3);
				dictionary.Add("ShengWangAward", 4);
				dictionary.Add("ZhanGongAward", 5);
				dictionary.Add("BindZuanAward", 6);
				dictionary.Add("XingHunAward", 7);
				dictionary.Add("YuanSuFenMo", 8);
				ZiYuanZhaoHuiPopPart.<>f__switch$mapC = dictionary;
			}
			int num;
			if (ZiYuanZhaoHuiPopPart.<>f__switch$mapC.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					url = "NetImages/GameRes/Images/Unit/exp.png";
					icon.NetImage.Width = 43.0;
					icon.NetImage.Height = 43.0;
					break;
				case 1:
					url = "NetImages/GameRes/Images/Unit/bindmoney.png";
					break;
				case 2:
					url = "NetImages/GameRes/Images/Unit/mojing.png";
					break;
				case 3:
					url = "NetImages/GameRes/Images/Unit/chengjiu.png";
					break;
				case 4:
					url = "NetImages/GameRes/Images/Unit/shengwang.png";
					break;
				case 5:
					url = "NetImages/GameRes/Images/Unit/zhangong.png";
					break;
				case 6:
					url = "NetImages/GameRes/Images/Unit/binddiamond.png";
					break;
				case 7:
					url = "NetImages/GameRes/Images/Unit/xinghun.png";
					break;
				case 8:
					url = "NetImages/GameRes/Images/Unit/yuansu.png";
					break;
				}
			}
		}
		icon.NetImage.URL = url;
		icon.NetImage.ImageDownloaded = delegate(object s)
		{
			if (icon.NetImage.Texture != null)
			{
				icon.NetImage.Texture.MakePixelPerfect();
			}
		};
		if (this.m_MoneyType == ZiYuanZhaoHuiPart.MoneyType.Gold)
		{
			int num2 = (int)((float)count * 0.75f);
			icon.Text.text = num2.ToString();
		}
		else
		{
			icon.Text.text = count.ToString();
		}
		this.ItemCollection.AddNoUpdate(icon);
		icon.gameObject.AddComponent<UIDragPanelContents>();
	}

	public GButton m_Close;

	public GButton m_BtnOK;

	public GButton m_BtnCancel;

	public UILabel m_ContentText;

	public UILabel m_Description;

	public UILabel m_Description2;

	public ShowNetImage MoneyTypeNetImage;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ZiYuanZhaoHuiPart.MoneyType m_MoneyType;

	public ZiYuanZhaoHuiPart.ZhaoHuiModel m_ZhaoHuiModel;

	public int ID = -1;

	private OldResourceInfo ResourceInfo;

	private int needCount;
}
