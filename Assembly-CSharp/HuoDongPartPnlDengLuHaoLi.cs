using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class HuoDongPartPnlDengLuHaoLi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnDengLuChouJiang.Text = Global.GetLang("领奖");
		this.m_LblLianXuDengLuInfo.text = string.Empty;
		this.m_LblLianXuDengLuNum.text = string.Empty;
		this.m_LblLianXuDengLuNum.pivot = 4;
		this.m_LblLianXuDengLuNum.transform.localPosition = new Vector3(0f, -233f, 0f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_ListDengLuHaoLiObC = this.m_ListDengLuHaoLi.ItemsSource;
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.SpriteQureyeEverydaySeriesLoginInfo();
		if (null != this.m_BtnDengLuChouJiang)
		{
			this.m_BtnDengLuChouJiang.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				int baoGuoSpaceCount = Global.GetBaoGuoSpaceCount();
				if (baoGuoSpaceCount < this.m_nZaiXianShiChang)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包空间不足，至少需要{0}个空格子"), new object[]
					{
						this.m_nZaiXianShiChang
					}), 0, -1, -1, 0);
					return;
				}
				this.m_nLastStep = Global.Data.MyHuoDongData.SeriesLoginGetAwardStep;
				Super.ShowNetWaiting(string.Empty);
				this.m_BtnDengLuChouJiang.isEnabled = false;
				GameInstance.Game.SpriteGetSeriesLoginAwardGiftCmd(1);
			};
		}
	}

	private void DengLuHaoLiChouJiang()
	{
		for (int i = 0; i < this.m_ListDengLuHaoLiObC.Count; i++)
		{
			GameObject at = this.m_ListDengLuHaoLiObC.GetAt(i);
			if (null != at)
			{
				DengluChoujiangListItem component = at.gameObject.GetComponent<DengluChoujiangListItem>();
				if (null != component && 3 < component.m_ListJiangPinObC.Count)
				{
					TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
					if (null != component2 && !component.m_bEnd)
					{
						component.m_bBegining = true;
						component2.enabled = true;
					}
				}
			}
		}
	}

	private GGoodIcon GetGoodsItemIcon(string strID, bool isDrag = false)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(strID), 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
			bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowGoodsTip(s);
		};
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private void SetGoodsIconImageGray(GGoodIcon icon)
	{
		if (null != icon)
		{
			UITexture component = icon.GoodImg.gameObject.gameObject.GetComponent<UITexture>();
			if (null != component)
			{
				component.shader = Shader.Find("Unlit/Gray");
				component.gameObject.transform.localPosition = new Vector3(0f, 0f, -0.1f);
				component.gameObject.SetActive(false);
				component.gameObject.SetActive(true);
			}
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
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
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
		}
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
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(num, 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, dummyGoodsDataMu);
		}
	}

	public void GetDengluChouJiangNum()
	{
		string[] array = Global.Data.MyHuoDongData.SeriesLoginAwardGoodsID.Split(new char[]
		{
			','
		});
		this.m_nZaiXianShiChang = Global.Data.roleData.SeriesLoginNum;
		if (null != this.m_LblLianXuDengLuInfo)
		{
			this.m_LblLianXuDengLuInfo.text = Global.GetColorStringForNGUIText(new object[]
			{
				"CC7432",
				string.Format(Global.GetLang("第{0}天"), this.m_nZaiXianShiChang)
			});
		}
		XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/MULoginNumGift.xml");
		if (isolateResXml == null)
		{
			return;
		}
		this.m_ListDengLuHaoLiObC.Clear();
		List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Item");
		int num = Global.Data.MyHuoDongData.SeriesLoginGetAwardStep;
		int num2 = this.m_nZaiXianShiChang;
		int num3 = 0;
		for (int i = 0; i < xelementList.Count; i++)
		{
			DengluChoujiangListItem dengluChoujiangListItem = U3DUtils.NEW<DengluChoujiangListItem>();
			dengluChoujiangListItem.m_ListJiangPinObC = dengluChoujiangListItem.m_ListJiangPin.ItemsSource;
			dengluChoujiangListItem.m_ListJiangPinObC.Clear();
			this.m_ListDengLuHaoLiObC.AddNoUpdate(dengluChoujiangListItem);
			XElement xelement = xelementList[i];
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowGoods");
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ID");
			string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "LoginTime");
			string[] array2 = xelementAttributeStr.Split(new char[]
			{
				','
			});
			int num4 = 0;
			if (0 < num)
			{
				dengluChoujiangListItem.m_bEnd = true;
				dengluChoujiangListItem.m_LiPinState.text = Global.GetLang("已经领取");
				dengluChoujiangListItem.m_SprLiPinState.gameObject.SetActive(true);
			}
			else if (0 >= num && 0 < num2)
			{
				num3++;
				dengluChoujiangListItem.m_bEnd = false;
				dengluChoujiangListItem.m_LiPinState.text = Global.GetLang("可抽取");
				dengluChoujiangListItem.m_LiPinState.gameObject.SetActive(false);
				dengluChoujiangListItem.m_SprKeChouJaing.gameObject.SetActive(true);
			}
			else
			{
				dengluChoujiangListItem.m_bEnd = false;
				int num5 = i + 1;
				dengluChoujiangListItem.m_LiPinState.text = string.Format(Global.GetLang("第{0}天"), num5);
				dengluChoujiangListItem.m_LiPinState.gameObject.SetActive(true);
				dengluChoujiangListItem.m_LiPinState.color = NGUIMath.HexToColorEx(7895160U);
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(array2[0], false);
				dengluChoujiangListItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
				dengluChoujiangListItem.m_ListJiangPinObC.DelayUpdate();
			}
			if (0 >= num && 0 < num2)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					if (num4 > 5)
					{
						break;
					}
					GGoodIcon goodsItemIcon2 = this.GetGoodsItemIcon(array2[j], false);
					dengluChoujiangListItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon2);
					num4++;
				}
				this.FixItem(dengluChoujiangListItem, xelementAttributeStr);
				dengluChoujiangListItem.m_ListJiangPinObC.DelayUpdate();
			}
			if (dengluChoujiangListItem.m_bEnd)
			{
				GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(Convert.ToInt32(array[i]), null);
				GGoodIcon goodsItemIcon3 = this.GetGoodsItemIcon(array[i], false);
				dengluChoujiangListItem.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon3);
				dengluChoujiangListItem.m_LiPinState.gameObject.SetActive(false);
				if (null != goodsItemIcon3)
				{
				}
				dengluChoujiangListItem.m_ListJiangPinObC.DelayUpdate();
			}
			num--;
			num2--;
		}
		if (0 >= num3 && null != this.m_BtnDengLuChouJiang)
		{
			this.m_BtnDengLuChouJiang.isEnabled = false;
			this.m_BtnDengLuChouJiang.Label.color = NGUIMath.HexToColorEx(7697781U);
		}
		this.m_ListDengLuHaoLiObC.DelayUpdate();
		if (null != this.m_LblLianXuDengLuNum)
		{
			if (0 < num3)
			{
				this.m_LblLianXuDengLuNum.text = string.Format(Global.GetLang("连续登陆{0}天，请点击{1}按钮领取{2}个奖励"), Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Convert.ToString(this.m_nZaiXianShiChang)
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Global.GetLang("【领奖】")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Convert.ToString(num3)
				}));
			}
			else
			{
				this.m_LblLianXuDengLuNum.text = string.Format(Global.GetLang("连续登陆{0}天，今日奖励已全部领取"), Global.GetColorStringForNGUIText(new object[]
				{
					"00ff00",
					Convert.ToString(this.m_nZaiXianShiChang)
				}));
			}
		}
	}

	private void FixItem(DengluChoujiangListItem item, string strGoods)
	{
		if (5 < item.m_ListJiangPinObC.Count)
		{
			for (int i = 0; i < item.m_ListJiangPinObC.Count; i++)
			{
				if (5 <= i)
				{
					item.m_ListJiangPinObC.RemoveAt(i);
				}
			}
			return;
		}
		if (5 > item.m_ListJiangPinObC.Count && 2 < item.m_ListJiangPinObC.Count)
		{
			string[] array = strGoods.Split(new char[]
			{
				','
			});
			for (int j = 0; j < array.Length; j++)
			{
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(array[j], false);
				item.m_ListJiangPinObC.AddNoUpdate(goodsItemIcon);
				if (5 <= item.m_ListJiangPinObC.Count)
				{
					break;
				}
			}
			return;
		}
	}

	protected virtual void OnEnable()
	{
		if (this.m_bIsRecvReady)
		{
			base.StopCoroutine("TimeProc");
			this.m_nLastTick = 1;
			base.StartCoroutine("TimeProc");
		}
	}

	private new void OnDestroy()
	{
		this.m_bIsRecvReady = false;
		this.m_nLastTick = 0;
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			if (this.m_bIsRecvReady && 5 < this.m_nLastTick)
			{
				this.DengLuHaoLiChouJiangStop();
			}
			this.m_nLastTick++;
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	private void DengLuHaoLiChouJiangStop()
	{
		if (0 < this.m_ListDengLuHaoLiObC.Count)
		{
			for (int i = 0; i < this.m_ListDengLuHaoLiObC.Count; i++)
			{
				GameObject at = this.m_ListDengLuHaoLiObC.GetAt(i);
				if (null != at)
				{
					DengluChoujiangListItem component = at.gameObject.GetComponent<DengluChoujiangListItem>();
					if (null != component && component.m_bBegining)
					{
						if (60f < component.m_ListJiangPin.gameObject.transform.localPosition.y && !component.m_bIsChangIcon)
						{
							component.m_bIsChangIcon = true;
							GameObject at2 = component.m_ListJiangPinObC.GetAt(0);
							this.ChangeIcon(at2, component.m_strJiangLiWuPin);
							GameObject at3 = component.m_ListJiangPinObC.GetAt(component.m_ListJiangPinObC.Count - 1);
							this.ChangeIcon(at3, component.m_strJiangLiWuPin);
						}
						TweenPosition component2 = component.m_ListJiangPin.gameObject.GetComponent<TweenPosition>();
						component2.duration += 0.2f;
						if (null != component2 && 30f > component.m_ListJiangPin.gameObject.transform.localPosition.y && !component.m_bEnd && 0 < component.m_ListJiangPinObC.Count)
						{
							component.m_bBegining = false;
							component.m_bEnd = true;
							SpringPosition component3 = component.m_ListJiangPin.gameObject.GetComponent<SpringPosition>();
							component3.target.x = 0f;
							component3.target.y = -4f;
							component3.enabled = true;
							component2.enabled = false;
							component2.duration = 0.2f;
							GameObject at4 = component.m_ListJiangPinObC.GetAt(0);
							component.m_LiPinState.gameObject.SetActive(false);
							component.m_SprKeChouJaing.gameObject.SetActive(false);
							component.m_SprLiPinState.gameObject.SetActive(true);
							if (null != at4)
							{
							}
							if (null != this.m_BtnDengLuChouJiang)
							{
								this.m_BtnDengLuChouJiang.isEnabled = false;
								this.m_BtnDengLuChouJiang.Label.color = NGUIMath.HexToColorEx(7697781U);
							}
							this.m_nChouJiangScuess++;
						}
					}
				}
			}
			if (this.m_nChouJiangScuess == this.m_nRecvConut)
			{
				GameInstance.Game.SpriteGetSeriesLoginAwardGiftCmd(2);
				this.m_nRecvConut = 0;
				this.m_nChouJiangScuess = -1;
				this.m_bIsRecvReady = false;
				this.m_nLastTick = 0;
				base.StopCoroutine("TimeProc");
			}
		}
	}

	private void ShowGetJiangLi(string strGooodsID)
	{
		string goodsNameByID = Global.GetGoodsNameByID(Convert.ToInt32(strGooodsID), false);
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang(string.Format(Global.GetLang("抽取到{0}"), goodsNameByID)), new object[0]), 0, -1, -1, 0);
	}

	private void ChangeIcon(GameObject GObject, string strRecvWuPin = "")
	{
		if (null != GObject)
		{
			GGoodIcon component = GObject.gameObject.GetComponent<GGoodIcon>();
			if (string.Empty != strRecvWuPin)
			{
				if (null != component)
				{
					component.BackgroundSprite0.gameObject.SetActive(false);
					component.BackgroundSprite1.gameObject.SetActive(false);
					component.BackgroundSprite2.gameObject.SetActive(false);
					component.TeXiao.gameObject.SetActive(false);
					component.BindingSprite.gameObject.SetActive(false);
					component.NoUseSprite.gameObject.SetActive(false);
					component.EndTimeSprite.gameObject.SetActive(false);
					component.ZhanLiSprite.gameObject.SetActive(false);
					component.GoodImg.gameObject.SetActive(false);
					component.ContentText.gameObject.SetActive(false);
					component.SecondText.gameObject.SetActive(false);
					BoxCollider componentInChildren = GObject.GetComponentInChildren<BoxCollider>();
					if (null != componentInChildren)
					{
						componentInChildren.enabled = false;
					}
				}
				GGoodIcon goodsItemIcon = this.GetGoodsItemIcon(strRecvWuPin, false);
				U3DUtils.AddChild(GObject.gameObject, goodsItemIcon.gameObject, true);
			}
			else
			{
				component.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					"Images/Goods/-1.png"
				}), false, 0);
			}
		}
	}

	public void DengLuHaoLiChouJiangWuPin(MUSocketConnectEventArgs e)
	{
		if (e != null)
		{
			int num = Convert.ToInt32(e.fields[0]);
			int num2 = Convert.ToInt32(e.fields[1]);
			int num3 = Convert.ToInt32(e.fields[2]);
			num3 -= this.m_nLastStep;
			int num4 = Convert.ToInt32(e.fields[3]);
			List<int> list = new List<int>();
			for (int i = 4; i < 11; i++)
			{
				list.Add(Convert.ToInt32(e.fields[i]));
			}
			this.m_nRecvConut = num3;
			this.DengLuHaoLiChouJiangSetRecvWuPin(num3, list);
			this.DengLuHaoLiChouJiang();
			base.StartCoroutine("TimeProc");
		}
	}

	private void DengLuHaoLiChouJiangSetRecvWuPin(int nJiangLiStep, List<int> listWuPin)
	{
		if (0 < this.m_ListDengLuHaoLiObC.Count)
		{
			for (int i = 0; i < nJiangLiStep; i++)
			{
				GameObject at = this.m_ListDengLuHaoLiObC.GetAt(this.m_nLastStep + i);
				if (null != at)
				{
					DengluChoujiangListItem component = at.gameObject.GetComponent<DengluChoujiangListItem>();
					if (null != component)
					{
						component.m_strJiangLiWuPin = Convert.ToString(listWuPin[i]);
					}
				}
			}
		}
		this.m_nLastStep = Global.Data.MyHuoDongData.SeriesLoginGetAwardStep;
		this.m_bIsRecvReady = true;
	}

	private new void Start()
	{
	}

	public bool m_bIsRecvReady;

	public int m_nZaiXianShiChang;

	public ListBox m_ListDengLuHaoLi = new ListBox();

	private ObservableCollection m_ListDengLuHaoLiObC;

	public GButton m_BtnDengLuChouJiang;

	public UILabel m_LblLianXuDengLuNum;

	public UILabel m_LblLianXuDengLuInfo;

	public int m_nLastStep;

	private int m_nLastTick;

	private int m_nRecvConut;

	private int m_nChouJiangScuess;
}
