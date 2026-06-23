using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class LoversWishPart_SendWish : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ZhuFuZhi.pivot = 4;
		this.ZhuFuZhi.transform.localPosition = new Vector3(100f, -87f, -1f);
		this.InputBoxJiYu.maxChars = 42;
		this.Normal.transform.localPosition = new Vector3(-25f, this.Normal.transform.localPosition.y, this.Normal.transform.localPosition.z);
		this.BtnXinXinXiangYin.Label.text = Global.GetLang("心心相印");
		this.BtnBaiNianHaoHe.Label.text = Global.GetLang("百年好合");
		this.BtnTianChangDiJiu.Label.text = Global.GetLang("天长地久");
		this.BtnYiShengYiShi.Label.text = Global.GetLang("一生一世");
		this.BtnZengSong.Label.text = Global.GetLang("赠送");
		this.BtnChange.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("换")
		});
		this.XiaoHaoLab1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("消耗：")
		});
		this.XiaohaoLab2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("消耗：")
		});
		this.XiaohaoLab3.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("消耗：")
		});
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("送出祝福")
		});
	}

	private void XuanZhongChange(int Index)
	{
		for (int i = 0; i < this.XuanZhong.Length; i++)
		{
			if (i == Index)
			{
				this.XuanZhong[i].SetActive(true);
			}
			else
			{
				this.XuanZhong[i].SetActive(false);
			}
		}
	}

	private void FirstGouXuan()
	{
		this.chkModeZhuFu.Check = true;
		this.WishCostType = 1;
		this.chkModeDiamond.Check = false;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.FirstGouXuan();
		this.chkModeZhuFu.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.chkModeZhuFu.Check)
			{
				this.WishCostType = 1;
				this.chkModeDiamond.Check = false;
			}
			else
			{
				this.WishCostType = 2;
				this.chkModeDiamond.Check = true;
			}
		};
		this.chkModeDiamond.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (this.chkModeDiamond.Check)
			{
				this.WishCostType = 2;
				this.chkModeZhuFu.Check = false;
			}
			else
			{
				this.WishCostType = 1;
				this.chkModeZhuFu.Check = true;
			}
		};
		this.BtnXinXinXiangYin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NormalExpend(1);
			this.XuanZhongChange(0);
		};
		this.BtnBaiNianHaoHe.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NormalExpend(2);
			this.XuanZhongChange(1);
		};
		this.BtnTianChangDiJiu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.NormalExpend(3);
			this.XuanZhongChange(2);
		};
		this.BtnYiShengYiShi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendLanguage(4);
			this.XuanZhongChange(3);
		};
		this.BtnZengSong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendMessage();
		};
		this.BtnXiaLa.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowFriendWindow();
		};
		this.BtnChange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.BenDiZhuFu(null);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs());
		};
	}

	public void KuaFuZhuFu(string rolemanname, string rolewomanname, int dbcoupleId)
	{
		this.IsKuaFu = true;
		this.DBCoupleId = dbcoupleId;
		this.KuaFu2BenDi();
		this.KuaFuZhuFuText(rolemanname, rolewomanname);
		this.NormalExpend(1);
		this.XuanZhongChange(0);
	}

	public void BenDiZhuFu(string rolename = null)
	{
		this.IsKuaFu = false;
		this.KuaFu2BenDi();
		this.BenDiZhuFuText(rolename);
		this.NormalExpend(1);
		this.XuanZhongChange(0);
	}

	private void SendMessage()
	{
		CoupleWishWishReqData coupleWishWishReqData = new CoupleWishWishReqData();
		coupleWishWishReqData.IsWishRankRole = this.IsKuaFu;
		if (this.IsKuaFu)
		{
			coupleWishWishReqData.ToRankCoupleId = this.DBCoupleId;
			coupleWishWishReqData.ToLocalRoleName = string.Empty;
		}
		else
		{
			coupleWishWishReqData.ToRankCoupleId = 0;
			coupleWishWishReqData.ToLocalRoleName = this.InputBox.label.text;
		}
		coupleWishWishReqData.WishType = this.WishType;
		if (this.WishType == 4)
		{
			string text = this.InputBoxJiYu.label.text;
			if (Global.IncludeReplaceFilterFileds(text))
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的寄语当中含有敏感词汇，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			text = Global.StringReplaceAll(text, "'", string.Empty);
			text = Global.StringReplaceAll(text, "|", string.Empty);
			text = Global.StringReplaceAll(text, "$", string.Empty);
			text = Global.StringReplaceAll(text, ":", string.Empty);
			text = Global.ReplaceFilterFileds(text);
			if (text.Contains("{") || text.Contains("}"))
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("抱歉,您的寄语当中含有非法字符，请重新输入!"), -1, -1, -1, -1, false);
				return;
			}
			if (this.InputBoxJiYu.label.text.Equals(Global.GetLang("最多15字")))
			{
				text = string.Empty;
			}
			coupleWishWishReqData.CostType = 2;
			coupleWishWishReqData.WishTxt = text;
		}
		else
		{
			coupleWishWishReqData.CostType = this.WishCostType;
			coupleWishWishReqData.WishTxt = string.Empty;
		}
		GameInstance.Game.WishOtherRoleForCoupleWish(coupleWishWishReqData);
		Super.ShowNetWaiting(null);
	}

	private void KuaFuZhuFuText(string rolemanname, string rolewomanname)
	{
		this.KuaFuRoleManName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			rolemanname
		});
		this.KuaFuRoleWomanName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			rolewomanname
		});
	}

	private void BenDiZhuFuText(string rolename)
	{
		this.InputBox.label.text = rolename;
	}

	private void NormalExpend(int type)
	{
		this.WishType = type;
		this.Normal2YiSheng(true);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(int.Parse(LoversWishPart.GetWishTypeDic()[type].ItemNum.Split(new char[]
		{
			','
		})[0]));
		if (Global.GetTotalGoodsCountByID(2130) < int.Parse(LoversWishPart.GetWishTypeDic()[type].ItemNum.Split(new char[]
		{
			','
		})[1]))
		{
			this.XiaoHaoZhuFu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				string.Format(Global.GetLang("【{0}】 X {1}"), goodsXmlNodeByID.Title, LoversWishPart.GetWishTypeDic()[type].ItemNum.Split(new char[]
				{
					','
				})[1])
			});
		}
		else
		{
			this.XiaoHaoZhuFu.text = string.Format(Global.GetLang("【{0}】X{1}"), goodsXmlNodeByID.Title, LoversWishPart.GetWishTypeDic()[type].ItemNum.Split(new char[]
			{
				','
			})[1]);
		}
		if (Global.Data.roleData.UserMoney < LoversWishPart.GetWishTypeDic()[type].ZhuanShi)
		{
			this.XiaoHaoDiamond.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				LoversWishPart.GetWishTypeDic()[type].ZhuanShi
			});
		}
		else
		{
			this.XiaoHaoDiamond.text = LoversWishPart.GetWishTypeDic()[type].ZhuanShi.ToString();
		}
		this.ZhuFuZhi.text = string.Format(Global.GetLang("对方增加{0}祝福值"), LoversWishPart.GetWishTypeDic()[type].WishNum);
	}

	private void SendLanguage(int type)
	{
		this.WishType = type;
		this.Normal2YiSheng(false);
		this.InputBoxJiYu.label.text = Global.GetLang("最多15字");
		this.InputBoxJiYu.label.color = Color.gray;
		if (Global.Data.roleData.UserMoney < LoversWishPart.GetWishTypeDic()[type].ZhuanShi)
		{
			this.XiaoHaoNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				LoversWishPart.GetWishTypeDic()[type].ZhuanShi
			});
		}
		else
		{
			this.XiaoHaoNum.text = LoversWishPart.GetWishTypeDic()[type].ZhuanShi.ToString();
		}
		this.ZhuFuZhi.text = string.Format(Global.GetLang("对方增加{0}祝福值"), LoversWishPart.GetWishTypeDic()[type].WishNum);
	}

	private void KuaFu2BenDi()
	{
		this.KuaFu.SetActive(this.IsKuaFu);
		this.BenDi.SetActive(!this.IsKuaFu);
	}

	private void Normal2YiSheng(bool IsNormal)
	{
		this.Normal.SetActive(IsNormal);
		this.YiSheng.SetActive(!IsNormal);
	}

	private void ShowFriendWindow()
	{
		this.CloseFriendWindow();
		if (this.FriendWindow == null && this._FriendPart == null)
		{
			this.FriendWindow = U3DUtils.NEW<GChildWindow>();
			this.FriendWindow.IsShowModal = true;
			this.InitChildWindow(this.FriendWindow, "jieriZengsongFriend", false);
			this.Container.Add(this.FriendWindow);
			this.FriendWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.FriendWindow);
				return true;
			};
			this.FriendWindow.ChildWindowModalBakClick = delegate(object s, EventArgs e)
			{
				this.CloseChildWindow(this.FriendWindow);
				return true;
			};
			this.FriendWindow.ModalType = ChildWindowModalType.TransBak;
			this._FriendPart = U3DUtils.NEW<LovserWishpartFriendPart>();
			this._FriendPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.InputBox.text = e.Title;
				this.CloseFriendWindow();
			};
			this.FriendWindow.SetContent(this.FriendWindow.BodyPresenter, this._FriendPart, 0.0, 0.0, true);
		}
	}

	private void CloseFriendWindow()
	{
		if (this.FriendWindow != null)
		{
			this.CloseChildWindow(this.FriendWindow);
			this.FriendWindow = null;
			this._FriendPart = null;
		}
	}

	private void InitChildWindow(GChildWindow childWindow, string title, bool limitRange = false)
	{
		Super.InitChildWindow(childWindow, title);
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
		Super.CloseChildWindow(this.Container, childWindow);
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton BtnXinXinXiangYin;

	public GButton BtnBaiNianHaoHe;

	public GButton BtnClose;

	public GButton BtnTianChangDiJiu;

	public GButton BtnYiShengYiShi;

	public GButton BtnZengSong;

	public GButton BtnXiaLa;

	public GButton BtnChange;

	public GCheckBox chkModeZhuFu;

	public GCheckBox chkModeDiamond;

	public GameObject BenDi;

	public GameObject KuaFu;

	public GameObject Normal;

	public GameObject YiSheng;

	public GameObject[] XuanZhong;

	public UILabel KuaFuRoleManName;

	public UILabel KuaFuRoleWomanName;

	public UILabel XiaoHaoZhuFu;

	public UILabel XiaoHaoDiamond;

	public UILabel XiaoHaoLab1;

	public UILabel XiaohaoLab2;

	public UILabel XiaohaoLab3;

	public UILabel ZhuFuZhi;

	public UILabel XiaoHaoNum;

	public UILabel Title;

	public TextBox InputBox;

	public TextBox InputBoxJiYu;

	private GChildWindow FriendWindow;

	public LovserWishpartFriendPart _FriendPart;

	private bool IsKuaFu;

	private int DBCoupleId;

	private int WishType;

	private int WishCostType;
}
