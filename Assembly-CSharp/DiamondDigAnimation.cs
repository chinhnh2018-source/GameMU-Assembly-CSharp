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

public class DiamondDigAnimation : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnNext.Text = Global.GetLang("再挖一次");
		this.btnOK.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.isStopped = false;
		this.iconList = new List<GGoodIcon>();
		this.btnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.isEnableExtract)
			{
				Super.HintMainText(Global.GetLang("挖掘尚未结束！"), 10, 3);
				this.isEnableExtract = true;
				return;
			}
			base.StartCoroutine<bool>(this.CloseAndRemoveWindow());
		};
		this.btnNext.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.isEnableExtract)
			{
				Super.HintMainText(Global.GetLang("挖掘尚未结束！"), 10, 3);
				return;
			}
			int num = (!this.digOnlyOnce) ? (10 * this.costs) : this.costs;
			if (this.costDiamond)
			{
				if (Global.GetRoleOwnNumByMoneyType(163) < num && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("YingGuanShiChouQu", num, true))
				{
					IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = num - Global.GetRoleOwnNumByMoneyType(163);
					string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
					GChildWindow messageBoxWindowXingYun = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					messageBoxWindowXingYun.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindowXingYun.MessageBoxReturn;
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindowXingYun);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
						}
						return true;
					};
					return;
				}
			}
			else
			{
				int num2 = 31;
				int num3 = Global.Data.roleData.RoleCommonUseIntPamams[num2];
				if (num > num3)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFluorescentPoint, this.callback, string.Empty, string.Empty);
					return;
				}
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = ((!this.digOnlyOnce) ? 10 : 1)
			});
			this.isAllAnimation = false;
		};
		this.AniContainer.SetActive(false);
		this.isAllAnimation = true;
		this.ButtonOKLabel.text = Global.GetLang("确定");
	}

	public IEnumerator CloseAndRemoveWindow()
	{
		this.isEnableExtract = false;
		int count = this.iconList.Count;
		if (count > 0)
		{
			GGoodIcon icon = null;
			float disposeTime = 1f;
			Vector3 disposePos = new Vector3(260f, -15f, 0f);
			for (int i = 0; i < count; i++)
			{
				icon = this.iconList[i];
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

	public bool useDiamondToDig
	{
		set
		{
			this.costDiamond = value;
			this.SetDigCostsIcon();
		}
	}

	public int digCosts
	{
		set
		{
			this.costs = value;
		}
	}

	public bool digOnce
	{
		set
		{
			this.digOnlyOnce = value;
			this.SetDigCostsField();
			this.SetDigButtonText();
		}
	}

	public void RefreshGoodsIcons(List<int> list_goods)
	{
		this.AniContainer.SetActive(false);
		this.AniContainer.SetActive(true);
		this.animator.SetBool("animationAll", this.isAllAnimation);
		this.RemoveIcons();
		this.isStopped = false;
		if (list_goods == null || list_goods.Count <= 0)
		{
			return;
		}
		List<GoodsData> list = new List<GoodsData>();
		for (int i = 0; i < list_goods.Count; i++)
		{
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(list_goods[i]);
			list.Add(dummyGoodsData);
		}
		base.StartCoroutine("AddGoodListIcon", list);
	}

	public void RefreshAddGoodIconsByGoodsStr(string goodsStr)
	{
		this.AniContainer.SetActive(false);
		this.AniContainer.SetActive(true);
		this.animator.SetBool("animationAll", this.isAllAnimation);
		this.RemoveIcons();
		this.isStopped = false;
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
		this.isEnableExtract = false;
		float animationTime = 1f;
		yield return new WaitForSeconds(animationTime);
		this.isStopped = false;
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
				if (this.isStopped)
				{
					yield break;
				}
				this.AddGoodIcon(goodsList[i], new Vector3(realX, realY, 0f));
				yield return new WaitForSeconds(0.3f);
			}
		}
		yield return new WaitForSeconds(1f);
		this.isEnableExtract = true;
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
		icon.TextColor = 15793920U;
		icon.ContentText.Text = "Lv" + Global.GetDiamondLevelByGoodsID(gd.GoodsID);
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
			GTipServiceEx.ShowTip(icon, TipTypes.FluorescentDiamondBagTip, GoodsOwnerTypes.FluorescentDiamondBag, gd);
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
		this.iconList.Add(icon);
	}

	public void RemoveIcons()
	{
		this.isStopped = true;
		base.StopCoroutine("AddGoodListIcon");
		int count = this.iconList.Count;
		for (int i = count - 1; i >= 0; i--)
		{
			Object.Destroy(this.iconList[i].gameObject);
		}
		this.iconList.Clear();
	}

	private void SetDigCostsField()
	{
		if (null != this.TextUnit)
		{
			int num = (!this.digOnlyOnce) ? (10 * this.costs) : this.costs;
			this.TextUnit.text = num.ToString();
		}
	}

	private void SetDigButtonText()
	{
		if (null != this.ButtonNextLabel)
		{
			string text = (!this.digOnlyOnce) ? Global.GetLang("再挖十次") : Global.GetLang("再挖一次");
			this.ButtonNextLabel.text = text;
		}
	}

	private void SetDigCostsIcon()
	{
		if (null == this.TextureUnit)
		{
			return;
		}
		if (this.costDiamond)
		{
			int count = (!this.digOnlyOnce) ? (10 * this.costs) : this.costs;
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshShowImg(this.TextureUnit, "YingGuanShiChouQu", count, "NetImages/GameRes/Images/Unit/xingyunzhixing.png");
		}
		else
		{
			this.TextureUnit.URL = "NetImages/GameRes/Images/Unit/fluorescentPowder.png";
		}
	}

	private const int timesUnit = 10;

	public ShowNetImage TextureUnit;

	public UILabel TextUnit;

	public UILabel ButtonNextLabel;

	public UILabel ButtonOKLabel;

	public GButton btnOK;

	public GButton btnClose;

	public GButton btnNext;

	public GameObject IconContainer;

	public GameObject AniContainer;

	public Animator animator;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler callback;

	private bool isStopped;

	private List<GGoodIcon> iconList;

	private bool isEnableExtract = true;

	private bool isAllAnimation = true;

	private bool costDiamond;

	private int costs;

	private bool digOnlyOnce = true;
}
