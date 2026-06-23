using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeQuanActivityEventuallyAwardPart : UserControl, BaseTeQuanActivityPart
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void UpdataTransPos(Transform[] trans)
	{
		if (trans != null)
		{
			int num = trans.Length;
			if (0 < trans.Length)
			{
				if (num != 1)
				{
					int num2 = 0;
					if (num % 2 != 0)
					{
						num2 = -45;
					}
					for (int i = 0; i < num; i++)
					{
						Transform transform = trans[i];
						if (i % 2 == 0)
						{
							transform.transform.localPosition = new Vector3((float)(45 + i * 45 + num2), 0f, 0f);
						}
						else
						{
							transform.transform.localPosition = new Vector3((float)(0 - i * 45 + num2), 0f, 0f);
						}
					}
				}
			}
		}
	}

	private void InitGoods(int ID)
	{
		for (int i = 1; i < this._GoodsRoot.childCount; i++)
		{
			Transform child = this._GoodsRoot.GetChild(i);
			if (null != child)
			{
				Object.Destroy(child.gameObject);
			}
		}
		this._GoodsItemRoot.gameObject.SetActive(false);
		List<GoodsData> zhongJiJiangLiGoods = IConfigbase<ConfigTeQuan>.Instance.GetZhongJiJiangLiGoods(ID);
		Transform[] array = new Transform[zhongJiJiangLiGoods.Count];
		if (0 < zhongJiJiangLiGoods.Count)
		{
			for (int j = 0; j < zhongJiJiangLiGoods.Count; j++)
			{
				GGoodIcon ggoodIcon = this.AddGoodIcon(zhongJiJiangLiGoods[j]);
				GameObject gameObject = Object.Instantiate<GameObject>(this._GoodsItemRoot);
				gameObject.SetActive(true);
				array[j] = gameObject.transform;
				gameObject.transform.SetParent(this._GoodsRoot, false);
				ggoodIcon.transform.SetParent(gameObject.transform, false);
			}
		}
		this.UpdataTransPos(array);
	}

	public GGoodIcon AddGoodIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = 0;
		ggoodIcon.TextSize = 16;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.Tag = gd.ExcellenceInfo;
		ggoodIcon.SecondText.Text = gd.GCount.ToString();
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(ggoodIcon, gd, true, IconTextTypes.Qianghua);
		ggoodIcon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			ggoodIcon.TeXiao.gameObject.SetActive(true);
			GameObject gameObject = Resources.Load("UITeXiao/Qifu/GaoJiXuanZhuan") as GameObject;
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0.15f);
			U3DUtils.AddPrefab(ggoodIcon.TeXiao.gameObject, gameObject, true);
			gameObject = (Resources.Load("UITeXiao/Qifu/GaoJiShanGuang") as GameObject);
			U3DUtils.AddPrefab(ggoodIcon.TeXiao.gameObject, gameObject, true);
		}
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon && ggoodIcon.ItemObject != null)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData != null)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._GetAwardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (!this.PartOpen)
				{
					Super.HintMainText(Global.GetLang("当前活动暂未激活"), 10, 3);
					return;
				}
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendGetRoleTeQuanBuyOrGetAward(this.ID, this.mSpecPriorityActInfo.ActID, 1);
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshPart(SpecPriorityActInfo inf)
	{
		if (inf == null)
		{
			return;
		}
		TeQuanTiaoJianVO teQuanTiaoJianVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanTiaoJianVOByID(inf.ActID);
		this._ProgressLabel.text = Global.GetLang("活动期间累计充值达到:") + IConfigbase<ConfigTeQuan>.Instance.GetZhongJiJiangLiTiaoJianByID(inf.ActID);
		this._Progress1Label.text = Global.GetLang("当前充值：") + inf.ShowNum;
		if (inf != null)
		{
			this.mSpecPriorityActInfo = inf;
			this.InitGoods(inf.ActID);
			if (inf.State == -1)
			{
				this._GetAwardBtn.Text = Global.GetLang("未达成");
				this._GetAwardBtn.isEnabled = false;
			}
			else if (inf.State == 0)
			{
				this._GetAwardBtn.Text = Global.GetLang("领取");
				this._GetAwardBtn.isEnabled = true;
			}
			else if (inf.State == 1)
			{
				this._GetAwardBtn.Text = Global.GetLang("已领取");
				this._GetAwardBtn.isEnabled = false;
			}
		}
	}

	public int ID { get; set; }

	public bool PartOpen { get; set; }

	[SerializeField]
	private UILabel _ProgressLabel;

	[SerializeField]
	private UILabel _Progress1Label;

	[SerializeField]
	private Transform _GoodsRoot;

	[SerializeField]
	private GameObject _GoodsItemRoot;

	[SerializeField]
	private GButton _GetAwardBtn;

	private SpecPriorityActInfo mSpecPriorityActInfo;
}
