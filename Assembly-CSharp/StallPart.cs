using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class StallPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.SetBottomBtnState((int)Global.g_StallStateType);
		this.InitControl();
		this.InitBagPart();
		GameInstance.Game.SpriteSelfSaleGoodsList();
		if (Global.g_StallStateType == StallStateType.StallOutline)
		{
			GameInstance.Game.SpriteGetLiXianBaiTanSecsCmd();
		}
	}

	public void SetBtsState()
	{
	}

	public void SetBottomBtnState(int nType)
	{
		switch (nType)
		{
		case 0:
			this.m_GameObjOutLine.gameObject.SetActive(false);
			this.m_GameObjBaiTanQian.gameObject.SetActive(true);
			this.m_GameObjShouTan.gameObject.SetActive(false);
			break;
		case 1:
			this.m_GameObjOutLine.gameObject.SetActive(false);
			this.m_GameObjBaiTanQian.gameObject.SetActive(false);
			this.m_GameObjShouTan.gameObject.SetActive(true);
			break;
		case 2:
			this.m_GameObjOutLine.gameObject.SetActive(true);
			this.m_GameObjBaiTanQian.gameObject.SetActive(false);
			this.m_GameObjShouTan.gameObject.SetActive(true);
			base.StartCoroutine("TimeProc");
			break;
		}
	}

	protected virtual void OnEnable()
	{
		if (Global.g_StallStateType == StallStateType.StallOutline)
		{
			base.StartCoroutine("TimeProc");
		}
	}

	public void SetTotalTime(int nSec)
	{
		if (nSec > 43200)
		{
			nSec = 43200;
		}
		this.m_nOutLineTotalTime = nSec;
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			this.m_nOutLineTotalTime--;
			if (null != this.m_LblOutLineTime)
			{
				this.m_LblOutLineTime.text = UIHelper.FormatSecsShort((long)this.m_nOutLineTotalTime, "-");
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public void BeginStall()
	{
	}

	public void EndStall()
	{
		Global.g_StallStateType = StallStateType.StallNull;
		base.StopCoroutine("TimeProc");
		this.SetBottomBtnState(0);
	}

	private void InitControl()
	{
		this.m_ListStallItemObC = this.m_ListStallItem.ItemsSource;
		this.m_ListStallItem.SelectionChanged = delegate(object s, MouseEvent e)
		{
			if (null != this.m_ListStallItem.SelectedItem)
			{
				StallItem component = this.m_ListStallItem.SelectedItem.gameObject.GetComponent<StallItem>();
				if (null != component && component.m_GoodsData != null)
				{
					PlayZone.GlobalPlayZone.OpenWuPinShangJiaWindow(component.m_GoodsData, 1);
				}
			}
		};
		if (null != this.m_LblStallName)
		{
			this.m_LblStallName.text = string.Format(Global.GetLang("{0}的货摊"), Global.Data.roleData.RoleName);
		}
		if (null != this.m_BtnShouTan)
		{
			this.m_BtnShouTan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.SpriteOpenMarketCmd(string.Empty, 0);
			};
		}
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
		if (null != this.m_BtnLog)
		{
			this.m_BtnLog.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			};
		}
		if (null != this.m_BtnOutLine)
		{
			this.m_BtnOutLine.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				WordsFilterMgr.ExecWordsFilter(this.m_LblStallName.text, delegate(object content, ExecWordsFilterEventArgs result)
				{
					if (result.ret > 0)
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
						{
							result.ret,
							result.msg
						}), 10, 3);
						return;
					}
					if (result.is_dirty > 0)
					{
						Super.HintMainText(Global.GetLang("摊位名称不能包含国家规定禁止的词汇!"), 10, 3);
						return;
					}
					if (10 < this.m_LblStallName.text.Length)
					{
						Super.HintMainText(Global.GetLang("摊位名称不能超过10的字符!"), 10, 3);
						return;
					}
				});
				int num = (int)ConfigSystemParam.GetSystemParamIntByName("VIPLiXianBaiTan");
				if (Global.GetVIPLeve() < num)
				{
					Super.HintMainText(string.Format(Global.GetLang("VIP达到{0}级才能离线摆摊！"), num), 10, 3);
					return;
				}
				if (this.AllowBaiTan())
				{
					Global.g_StallStateType = StallStateType.StallOutline;
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 2
					});
				}
			};
		}
		if (null != this.m_BtnOnLine)
		{
			this.m_BtnOnLine.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				WordsFilterMgr.ExecWordsFilter(this.m_LblStallName.text, delegate(object content, ExecWordsFilterEventArgs result)
				{
					if (result.ret > 0)
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
						{
							result.ret,
							result.msg
						}), 10, 3);
						return;
					}
					if (result.is_dirty > 0)
					{
						Super.HintMainText(Global.GetLang("摊位名称不能包含国家规定禁止的词汇!"), 10, 3);
						return;
					}
					if (10 < this.m_LblStallName.text.Length)
					{
						Super.HintMainText(Global.GetLang("摊位名称不能超过10的字符!"), 10, 3);
						return;
					}
				});
				if (this.AllowBaiTan())
				{
					this.SetBottomBtnState(1);
					Global.g_StallStateType = StallStateType.StallOnline;
					GameInstance.Game.SpriteOpenMarketCmd(this.m_LblStallName.text, 0);
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 3
					});
				}
			};
		}
	}

	private bool AllowBaiTan()
	{
		if (0 >= this.m_ListStallItemObC.Count)
		{
			this.ShowMsg(Global.GetLang("至少要有一个上架商品"));
			return false;
		}
		return true;
	}

	public void AddStallItem(GoodsData goodsdata, int nNum)
	{
		if (goodsdata == null)
		{
			return;
		}
		StallItem stallItem = U3DUtils.NEW<StallItem>();
		stallItem.goodsdata = goodsdata;
		if (Global.Data.SaleGoodsDataList != null)
		{
			this.m_ListStallItem.Replace(Global.Data.SaleGoodsDataList.Count, stallItem.gameObject);
		}
		else
		{
			this.m_ListStallItem.Replace(0, stallItem.gameObject);
		}
	}

	public void DeleteStallItem(GoodsData goodsData, int nPrice, int nNum)
	{
		if (goodsData == null)
		{
			return;
		}
		for (int i = 0; i < this.m_ListStallItemObC.Count; i++)
		{
			StallItem component = this.m_ListStallItemObC[i].gameObject.GetComponent<StallItem>();
			if (component.m_GoodsData.Id == goodsData.Id)
			{
				this.m_ListStallItemObC.RemoveAt(i);
				break;
			}
		}
		this.m_ListStallItemObC.DelayUpdate();
	}

	private void ShowMsg(string strMsg)
	{
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
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

	public void InitStallItem()
	{
		this.m_ListStallItemObC.Clear();
		if (Global.Data.SaleGoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.SaleGoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.SaleGoodsDataList[i];
				StallItem stallItem = U3DUtils.NEW<StallItem>();
				stallItem.goodsdata = Global.Data.SaleGoodsDataList[i];
				this.m_ListStallItemObC.AddNoUpdate(stallItem);
			}
			for (int j = 0; j < 16 - Global.Data.SaleGoodsDataList.Count; j++)
			{
				StallItem stallItem2 = U3DUtils.NEW<StallItem>();
				this.m_ListStallItemObC.AddNoUpdate(stallItem2);
				stallItem2.isEmpty = true;
			}
		}
		else
		{
			for (int k = 0; k < 16; k++)
			{
				StallItem stallItem3 = U3DUtils.NEW<StallItem>();
				this.m_ListStallItemObC.AddNoUpdate(stallItem3);
				stallItem3.isEmpty = true;
			}
		}
	}

	private void InitBagPart()
	{
		if (null != this.m_GameObjBag)
		{
			if (Super._ParcelPart != null)
			{
				Super._ParcelPart.iBaoGuoMode = 7;
				U3DUtils.AddChild(this.m_GameObjBag.gameObject, Super._ParcelPart.gameObject, true);
			}
			else
			{
				ParcelPart parcelPart = U3DUtils.NEW<ParcelPart>();
				Super._ParcelPart = parcelPart;
				Super._ParcelPart.iBaoGuoMode = 7;
				Super._ParcelPart.InitPartData();
				U3DUtils.AddChild(this.m_GameObjBag.gameObject, Super._ParcelPart.gameObject, true);
			}
			if (null != Super._ParcelPart)
			{
				Super._ParcelPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 7)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 502
						});
					}
					return true;
				};
			}
		}
	}

	private List<GoodsData> GetBeiBaoWuPin()
	{
		List<GoodsData> list = new List<GoodsData>();
		if (list == null || Global.Data.roleData.GoodsDataList == null)
		{
			return list;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.GoodsDataList[i].Using == 0)
			{
				list.Add(Global.Data.roleData.GoodsDataList[i]);
			}
		}
		return list;
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
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
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
		string text = Convert.ToString(ggoodIcon.GoodsID);
		if (string.Empty == text)
		{
			return;
		}
		int num = Convert.ToInt32(text);
		if (1 < num)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, 5, 6, 10000, 1, 0, 1, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton m_BtnClose;

	public GButton m_BtnOutLine;

	public GButton m_BtnOnLine;

	public GButton m_BtnLog;

	public GButton m_BtnShouTan;

	public UILabel m_LblStallName;

	public UILabel m_LblOutLineTime;

	public GameObject m_GameObjBag;

	public GameObject m_GameObjBaiTanQian;

	public GameObject m_GameObjOutLine;

	public GameObject m_GameObjShouTan;

	public ParcelPart m_ParcelPart;

	public ListBox m_ListStallItem = new ListBox();

	private ObservableCollection m_ListStallItemObC;

	public ListBox m_ListIcon = new ListBox();

	public int m_nOutLineTotalTime;
}
