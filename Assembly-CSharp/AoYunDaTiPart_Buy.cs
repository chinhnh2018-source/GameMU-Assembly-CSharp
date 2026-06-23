using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AoYunDaTiPart_Buy : UserControl
{
	public bool isTianshi
	{
		set
		{
			if (value)
			{
				this.title.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("天使帮帮忙")
				});
				this.DanJiaLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.danjia[0]
				});
				this.XiaoHaoNum.text = ((Global.Data.roleData.UserMoney >= this.danjia[0] * this.buyNum) ? Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.danjia[0] * this.buyNum
				}) : Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					this.danjia[0] * this.buyNum
				}));
				this.goodsType = 0;
			}
			else
			{
				this.title.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("恶魔帮帮忙")
				});
				this.DanJiaLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.danjia[1]
				});
				this.XiaoHaoNum.text = ((Global.Data.roleData.UserMoney >= this.danjia[1] * this.buyNum) ? Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					this.danjia[1] * this.buyNum
				}) : Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					this.danjia[1] * this.buyNum
				}));
				this.goodsType = 1;
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.xianyoucishu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("现有次数：")
		});
		this.DanJia.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("单        价：")
		});
		this.goumai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("购买数量：")
		});
		this.XiaoHaoZuanShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("消耗钻石：")
		});
		this.goumaiNum.text = this.buyNum.ToString();
		this.BtnQuXiao.Label.text = Global.GetLang("取消");
		this.BtnSure.Label.text = Global.GetLang("确定");
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("QuestionItem", '|');
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			this.danjia[i] = int.Parse(array[0]);
			this.shangxian[i] = int.Parse(array[1]);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnQuXiao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnJian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.buyNum > 1)
			{
				this.buyNum--;
			}
			else
			{
				this.buyNum = 1;
			}
			this.goumaiNum.text = this.buyNum.ToString();
			int num = int.Parse(string.Format("{0}", Super.ClearStringColor(this.DanJiaLab.text))) * this.buyNum;
			this.XiaoHaoNum.text = ((Global.Data.roleData.UserMoney >= num) ? Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				num
			}) : Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				num
			}));
		};
		this.BtnJia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num;
			if (this.goodsType == 0)
			{
				num = this.shangxian[0] - int.Parse(this.xianyoucishuLab.text);
			}
			else
			{
				num = this.shangxian[1] - int.Parse(this.xianyoucishuLab.text);
			}
			if (this.buyNum < num)
			{
				this.buyNum++;
			}
			else
			{
				this.buyNum = num;
			}
			this.goumaiNum.text = this.buyNum.ToString();
			int num2 = int.Parse(string.Format("{0}", Super.ClearStringColor(this.DanJiaLab.text))) * this.buyNum;
			this.XiaoHaoNum.text = ((Global.Data.roleData.UserMoney >= num2) ? Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				num2
			}) : Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				num2
			}));
		};
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendAoYunBuyGoodsData(this.goodsType, this.buyNum);
			Super.ShowNetWaiting(null);
		};
		UIEventListener.Get(this.inputBak.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DPSelectedItemNum, null, 0, -100);
		};
		this.DPSelectedItemNum = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			int num;
			if (this.goodsType == 0)
			{
				num = this.shangxian[0] - int.Parse(this.xianyoucishuLab.text);
			}
			else
			{
				num = this.shangxian[1] - int.Parse(this.xianyoucishuLab.text);
			}
			int num2;
			if (id > num)
			{
				num2 = num;
			}
			else
			{
				num2 = id;
			}
			this.goumaiNum.text = num2.ToString();
			this.buyNum = num2;
			this.XiaoHaoNum.text = ((Global.Data.roleData.UserMoney >= int.Parse(Super.ClearStringColor(this.DanJiaLab.text)) * num2) ? Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				int.Parse(string.Format("{0}", Super.ClearStringColor(this.DanJiaLab.text))) * num2
			}) : Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				int.Parse(string.Format("{0}", Super.ClearStringColor(this.DanJiaLab.text))) * num2
			}));
		};
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnJian;

	public GButton BtnJia;

	public GButton BtnQuXiao;

	public GButton BtnSure;

	public UILabel title;

	public UILabel xianyoucishu;

	public UILabel xianyoucishuLab;

	public UILabel DanJia;

	public UILabel DanJiaLab;

	public UILabel goumai;

	public UILabel goumaiNum;

	public UILabel XiaoHaoZuanShi;

	public UILabel XiaoHaoNum;

	public UISprite inputBak;

	private DPSelectedItemEventHandler DPSelectedItemNum;

	private int[] danjia = new int[2];

	private int[] shangxian = new int[2];

	private int buyNum = 1;

	private int goodsType;
}
