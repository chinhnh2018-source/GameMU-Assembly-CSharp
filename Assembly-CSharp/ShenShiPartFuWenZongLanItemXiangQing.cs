using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShenShiPartFuWenZongLanItemXiangQing : UserControl
{
	public int goodsID
	{
		get
		{
			return this.goodsid;
		}
		set
		{
			this.GoodIcon.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", value);
			this.goodsid = value;
			this.zhizuocount = ShenShiPart.GetDicFuWen()[this.goodsID].PayNum;
			this.zhizuoNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.zhizuocount
			});
			int sendNum = ShenShiPart.GetDicFuWen()[this.goodsID].SendNum;
			this.fenjieNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				sendNum
			});
		}
	}

	public string GoodsName
	{
		set
		{
			this.goodsName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				value
			});
		}
	}

	public string GoodsAttr
	{
		set
		{
			this.goodsAttr.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				value
			});
		}
	}

	public new int Count
	{
		get
		{
			return this.count;
		}
		set
		{
			this.count = value;
			if (this.count >= 8)
			{
				this.BtnZhiZuo.isEnabled = false;
			}
			else
			{
				this.BtnZhiZuo.isEnabled = true;
			}
			if (value <= 0)
			{
				this.BtnFenJie.isEnabled = false;
			}
			else
			{
				this.BtnFenJie.isEnabled = true;
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnZhiZuo.Label.text = Global.GetLang("制作");
		this.BtnFenJie.Label.text = Global.GetLang("分解");
		this.title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("符文详情")
		});
		this.goodsName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("猛击符文")
		});
		this.zhizuoLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("制作消耗:")
		});
		this.fenjieLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("分解获得:")
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnZhiZuo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.roleData.RoleCommonUseIntPamams[49] >= this.zhizuocount)
			{
				GameInstance.Game.GetFuWenZhiZuo(this.goodsID);
				Super.ShowNetWaiting(null);
			}
			else
			{
				Super.HintMainText(Global.GetLang("所需符文之尘不足"), 10, 3);
			}
		};
		this.BtnFenJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.FenJieData != null && this.FenJieData.Count > 0)
			{
				this.FenJieData.Clear();
			}
			if (this.Count > 8)
			{
				string goodsID = string.Format("{0},{1}", this.goodsID, 1);
				GameInstance.Game.GetFuWenFenJie(goodsID);
				Super.ShowNetWaiting(null);
			}
			else
			{
				if (this.Count <= 0)
				{
					Super.HintMainText(Global.GetLang("无可分解符文"), 10, 3);
					return;
				}
				if (this.isPeiDai)
				{
					this.maxNum = 0;
					int i = 0;
					int num = Global.Data.MyFuWenTabData.Count;
					while (i < num)
					{
						int num2 = 0;
						int j = 0;
						int num3 = Global.Data.MyFuWenTabData[i].FuWenEquipList.Count;
						while (j < num3)
						{
							if (this.goodsID == Global.Data.MyFuWenTabData[i].FuWenEquipList[j])
							{
								num2++;
							}
							j++;
						}
						if (num2 > 0)
						{
							FuWenFenJieData fuWenFenJieData = new FuWenFenJieData();
							fuWenFenJieData.TabID = i;
							fuWenFenJieData.FuWenID = this.goodsID;
							fuWenFenJieData.FuWenNum = num2;
							this.FenJieData.Add(fuWenFenJieData);
						}
						i++;
					}
					if (this.FenJieData.Count > 0)
					{
						int k = 0;
						int num4 = this.FenJieData.Count;
						while (k < num4)
						{
							if (this.maxNum < this.FenJieData[k].FuWenNum)
							{
								this.maxNum = this.FenJieData[k].FuWenNum;
							}
							k++;
						}
						if (this.Count > this.maxNum)
						{
							string lang = Global.GetLang("分解后个数将小于8个,影响符文佩戴,确定要分解吗？");
							string[] buttons = new string[]
							{
								Global.GetLang("确定"),
								Global.GetLang("取消")
							};
							Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s2, DPSelectedItemEventArgs e2)
							{
								if (e2.ID == 0)
								{
									string goodsID2 = string.Format("{0},{1}", this.goodsID, 1);
									GameInstance.Game.GetFuWenFenJie(goodsID2);
									Super.ShowNetWaiting(null);
								}
							}, buttons);
						}
						else
						{
							string text = null;
							int l = 0;
							int num5 = this.FenJieData.Count;
							while (l < num5)
							{
								text = text + (this.FenJieData[l].TabID + 1).ToString() + Global.GetLang("、");
								l++;
							}
							text = text.Substring(0, text.Length - 1);
							string message = string.Format(Global.GetLang("该符文在符文页{0}中有使用，分解后将空置？"), text);
							string[] buttons2 = new string[]
							{
								Global.GetLang("确定"),
								Global.GetLang("取消")
							};
							Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
							{
								if (e2.ID == 0)
								{
									string goodsID2 = string.Format("{0},{1}", this.goodsID, 1);
									GameInstance.Game.GetFuWenFenJie(goodsID2);
									Super.ShowNetWaiting(null);
								}
							}, buttons2);
						}
					}
				}
				else
				{
					string lang2 = Global.GetLang("分解后个数将小于8个,影响符文佩戴,确定要分解吗？");
					string[] buttons3 = new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, delegate(object s2, DPSelectedItemEventArgs e2)
					{
						if (e2.ID == 0)
						{
							string goodsID2 = string.Format("{0},{1}", this.goodsID, 1);
							GameInstance.Game.GetFuWenFenJie(goodsID2);
							Super.ShowNetWaiting(null);
						}
					}, buttons3);
				}
			}
		};
	}

	public void RemoveFuWen()
	{
		if (this.FenJieData != null && this.FenJieData.Count > 0)
		{
			this.FenJieData.Clear();
		}
		GameInstance.Game.GetFuWenTabList();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnZhiZuo;

	public GButton BtnFenJie;

	public ShowNetImage GoodIcon;

	public UILabel title;

	public UILabel goodsName;

	public UILabel goodsAttr;

	public UILabel zhizuoLab;

	public UILabel fenjieLab;

	public UILabel zhizuoNum;

	public UILabel fenjieNum;

	private List<FuWenFenJieData> FenJieData = new List<FuWenFenJieData>();

	private int zhizuocount;

	public bool isPeiDai;

	private int maxNum;

	private int goodsid;

	public int count;
}
