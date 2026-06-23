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

public class ShenShiPartFuWenChouQuGifts : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnOK.Text = Global.GetLang("确定");
		this.btnNext.Text = Global.GetLang("再抽一次");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.listDaiBi.Add(this.TextureUnit);
		this.IsStopped = false;
		this.IconList = new List<GGoodIcon>();
		this.btnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsEnableExtract)
			{
				Super.HintMainText(Global.GetLang("抽取尚未结束！"), 10, 3);
				this.IsEnableExtract = true;
				return;
			}
			base.StartCoroutine<bool>(this.CloseAndRemoveWindow());
		};
		this.btnNext.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.IsEnableExtract)
			{
				Super.HintMainText(Global.GetLang("抽取尚未结束！"), 10, 3);
				return;
			}
			if (this.IsQiFuTypeOne)
			{
				int goodsID = (int)ConfigSystemParam.GetSystemParamIntByName("JinDanFuWen");
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				if (this.values[0] > Global.GetRoleOwnNumByMoneyType(163) && totalGoodsCountByID <= 0 && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("FuWenChouQu", this.values[0], true))
				{
					IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = this.values[0] - Global.GetRoleOwnNumByMoneyType(163);
					string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
						}
						return true;
					};
					return;
				}
				if (totalGoodsCountByID <= 0 && Global.GetZuanShi(ZuanShiPartClass.ShenShiChouQu))
				{
					string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "FuWenChouQu", this.values[0]);
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.values[0], text)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.ShenShiChouQu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.Data.IsDoingZaJinDan = true;
							Super.ShowNetWaiting(null);
							GameInstance.Game.GetFuWenChouQu(1);
						}
						return true;
					};
					return;
				}
				Global.Data.IsDoingZaJinDan = true;
				Super.ShowNetWaiting(null);
				GameInstance.Game.GetFuWenChouQu(1);
			}
			else
			{
				if (this.values[1] > Global.GetRoleOwnNumByMoneyType(163) && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("FuWenChouQu", this.values[1], true))
				{
					int buyZuanShi = this.values[1] - Global.GetRoleOwnNumByMoneyType(163);
					string lang2 = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(lang2, buyZuanShi)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendBoCaiDaiBi(buyZuanShi, 2);
						}
						return true;
					};
					return;
				}
				if (Global.GetZuanShi(ZuanShiPartClass.ShenShiChouQu))
				{
					string text2 = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "FuWenChouQu", this.values[1]);
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.values[1], text2)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.ShenShiChouQu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.Data.IsDoingZaJinDan = true;
							Super.ShowNetWaiting(null);
							GameInstance.Game.GetFuWenChouQu(2);
						}
						return true;
					};
					return;
				}
				Global.Data.IsDoingZaJinDan = true;
				Super.ShowNetWaiting(null);
				GameInstance.Game.GetFuWenChouQu(2);
			}
		};
		this.AniContainer.SetActive(false);
	}

	public IEnumerator CloseAndRemoveWindow()
	{
		this.IsEnableExtract = false;
		int count = this.IconList.Count;
		if (count > 0)
		{
			GGoodIcon icon = null;
			float disposeTime = 1f;
			Vector3 disposePos = new Vector3(325f, 215f, 0f);
			for (int i = 0; i < count; i++)
			{
				icon = this.IconList[i];
				iTween.MoveTo(icon.gameObject, iTween.Hash(new object[]
				{
					"position",
					disposePos,
					"time",
					disposeTime,
					"islocal",
					true
				}));
				iTween.ScaleTo(icon.gameObject, Vector3.zero, disposeTime);
			}
			yield return new WaitForSeconds(disposeTime + 0.1f);
		}
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 0
		});
		yield break;
	}

	public void RefreshUIUnit(bool oneTime)
	{
		this.IsQiFuTypeOne = oneTime;
		if (oneTime)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("JinDanFuWen");
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num);
			if (totalGoodsCountByID > 0)
			{
				this.TextUnit.text = Global.GetGoodsNameByID(num, false);
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
				if (goodsXmlNodeByID != null)
				{
					string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
					this.TextureUnit.transform.localPosition = new Vector3(58f, 0f, -1f);
					this.TextureUnit.URL = StringUtil.substitute("NetImages/GameRes/{0}", new object[]
					{
						goodsImageURLFromIconCode
					});
				}
				this.ButtonNextLabel.text = Global.GetLang("再抽一次");
			}
			else
			{
				this.TextUnit.text = this.values[0].ToString();
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.listDaiBi[0], "FuWenChouQu", this.values[0], "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
				this.ButtonNextLabel.text = Global.GetLang("再抽一次");
			}
		}
		else
		{
			this.TextUnit.text = this.values[1].ToString();
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.listDaiBi[0], "FuWenChouQu", this.values[1], "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
			this.ButtonNextLabel.text = Global.GetLang("再抽十次");
		}
	}

	public void RefreshAddGoodIcons(string goodsStr)
	{
		this.AniContainer.SetActive(false);
		this.AniContainer.SetActive(true);
		this.RemoveIcons();
		this.IsStopped = false;
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				int goodsID = Convert.ToInt32(array2[0]);
				int gcount = Convert.ToInt32(array2[1]);
				int binding = Convert.ToInt32(array2[2]);
				int forgeLevel = Convert.ToInt32(array2[3]);
				int zhuijiaLevel = Convert.ToInt32(array2[4]);
				int lucky = Convert.ToInt32(array2[5]);
				int zhuoyueIndex = Convert.ToInt32(array2[6]);
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				list.Add(dummyGoodsDataMu);
			}
		}
		base.StartCoroutine("AddGoodListIcon", list);
	}

	public IEnumerator AddGoodListIcon(List<GoodsData> goodsList)
	{
		this.IsEnableExtract = false;
		yield return new WaitForSeconds(2f);
		this.IsStopped = false;
		int goodsCount = goodsList.Count;
		if (goodsCount == 1)
		{
			this.AddGoodIcon(goodsList[0], new Vector3(0f, 0f, 0f));
		}
		else
		{
			int beginX = -220;
			float interval = 110f;
			float realY = 20f;
			float realX = 0f;
			for (int i = 0; i < goodsCount; i++)
			{
				if (i >= 5)
				{
					realY = -80f;
					realX = (float)beginX + interval * (float)(i - 5);
				}
				else
				{
					realX = (float)beginX + interval * (float)i;
				}
				if (this.IsStopped)
				{
					yield break;
				}
				this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
				yield return new WaitForSeconds(0.3f);
			}
		}
		yield return new WaitForSeconds(1f);
		this.IsEnableExtract = true;
		SystemHelpMgr.OnAction(UIObjIDs.QiFuPartBtn01, HelpStateEvents.Clicked, 1);
		yield break;
	}

	public void AddGoodIcon(GoodsData gd, Vector3 localPos)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			icon.TeXiao.gameObject.SetActive(true);
			GameObject gameObject = Resources.Load("UITeXiao/Qifu/GaoJiXuanZhuan") as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.15f);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
			gameObject = (Resources.Load("UITeXiao/Qifu/GaoJiShanGuang") as GameObject);
			U3DUtils.AddPrefab(icon.TeXiao.gameObject, gameObject, true);
		}
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
		};
		U3DUtils.AddChild(this.IconContainer, icon.gameObject, true);
		icon.transform.localScale = new Vector3(0f, 0f, 0f);
		icon.gameObject.transform.localPosition = new Vector3(0f, 200f, 0f);
		icon.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
		iTween.MoveTo(icon.gameObject, iTween.Hash(new object[]
		{
			"position",
			localPos,
			"time",
			1.2f,
			"islocal",
			true
		}));
		iTween.RotateTo(icon.gameObject, new Vector3(0f, 0f, 0f), 1.2f);
		iTween.ScaleTo(icon.gameObject, new Vector3(1f, 1f, 1f), 1.2f);
		this.IconList.Add(icon);
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
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			GTipServiceEx.SelfBagOnly = false;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
		}
	}

	public void RemoveIcons()
	{
		this.IsStopped = true;
		base.StopCoroutine("AddGoodListIcon");
		int count = this.IconList.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			Object.Destroy(this.IconList[i].gameObject);
		}
		this.IconList.Clear();
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.btnOK, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public ShowNetImage TextureUnit;

	public UILabel TextUnit;

	public UILabel ButtonNextLabel;

	public GButton btnOK;

	public GButton btnNext;

	public GameObject IconContainer;

	public GameObject AniContainer;

	public DPSelectedItemEventHandler DPSelectedItem;

	private bool IsStopped;

	private List<GGoodIcon> IconList;

	private bool IsQiFuTypeOne = true;

	private bool IsEnableExtract = true;

	public List<ShowNetImage> listDaiBi = new List<ShowNetImage>();

	private int[] values = ConfigSystemParam.GetSystemParamIntArrayByName("FuWenPay", ',');
}
