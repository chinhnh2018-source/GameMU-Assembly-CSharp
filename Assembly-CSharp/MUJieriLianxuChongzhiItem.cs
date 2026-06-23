using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MUJieriLianxuChongzhiItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.staticText.text = Global.GetLang("每日充值");
		this.itemCollection = this.goodsList.ItemsSource;
		this.staticText.Pivot = 3;
		this.staticText.X = -17.0;
	}

	public string ZuanshiNum
	{
		get
		{
			return this._zuanshiNum;
		}
		set
		{
			this._zuanshiNum = value;
			this.NeedZuanshiNum.text = this.ZuanshiNum;
		}
	}

	public void InitItem1(Dictionary<int, string> dic, int group)
	{
		foreach (int num in dic.Keys)
		{
			MUJieriLianxuChongzhiItemGoodsItem mujieriLianxuChongzhiItemGoodsItem = U3DUtils.NEW<MUJieriLianxuChongzhiItemGoodsItem>();
			string empty = string.Empty;
			if (dic.TryGetValue(num, ref empty))
			{
				mujieriLianxuChongzhiItemGoodsItem.NeedDays = num.ToString();
				mujieriLianxuChongzhiItemGoodsItem.Id = group;
				string[] array = empty.Split(new char[]
				{
					'|'
				});
				bool flag = array.Length == 1;
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length != 7)
					{
						return;
					}
					int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
					if (!MUJieripartChongzhiKingItem.IsTongGuo(array2[0], roleOcc) || flag)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(array2[0].SafeToInt32(0));
						if (goodsXmlNodeByID != null)
						{
							string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
							string backSpriteName = "bagGrid4_bak";
							GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(array2[0].SafeToInt32(0), array2[3].SafeToInt32(0), array2[4].SafeToInt32(0), array2[6].SafeToInt32(0), array2[5].SafeToInt32(0), array2[2].SafeToInt32(0), array2[1].SafeToInt32(0), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
							GGoodIcon icon = mujieriLianxuChongzhiItemGoodsItem.GoodIcon;
							icon.Width = 78.0;
							icon.Height = 78.0;
							icon.isAutoSize = true;
							icon.BackSpriteName0 = backSpriteName;
							icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
							icon.TipType = 1;
							icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
							{
								array2[0].SafeToInt32(0),
								0,
								-1,
								-1
							});
							icon.ItemCode = array2[0].SafeToInt32(0);
							icon.ItemObject = dummyGoodsDataMu;
							icon.BoxTypes = 5;
							icon.TextShadowColor = 4278190080U;
							icon.TextColor = 16777215U;
							icon.DisableTextColor = 8421504U;
							bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
							Super.InitGoodsGIcon(icon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
							icon.gameObject.AddComponent<UIDragPanelContents>();
							icon.addEventListener("click", delegate(MouseEvent e)
							{
								GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
								if (null == ggoodIcon)
								{
									return;
								}
								GoodsData goodsData = icon.ItemObject as GoodsData;
								if (goodsData == null)
								{
									return;
								}
								GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
							});
						}
					}
				}
			}
			mujieriLianxuChongzhiItemGoodsItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
			mujieriLianxuChongzhiItemGoodsItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				GameInstance.Game.GetJieriChongzhiRewardCmd(e.ID, e.Index);
			};
			this.itemCollection.Add(mujieriLianxuChongzhiItemGoodsItem);
			UIPanel component = mujieriLianxuChongzhiItemGoodsItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			if (!this.DicIDtoItem.ContainsKey(num))
			{
				this.DicIDtoItem.Add(num, mujieriLianxuChongzhiItemGoodsItem);
			}
		}
	}

	public void InitItem(string goods, int id)
	{
		string[] array = goods.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length != 0)
			{
				MUJieriLianxuChongzhiItemGoodsItem mujieriLianxuChongzhiItemGoodsItem = U3DUtils.NEW<MUJieriLianxuChongzhiItemGoodsItem>();
				mujieriLianxuChongzhiItemGoodsItem.NeedDays = array2[0];
				mujieriLianxuChongzhiItemGoodsItem.Id = id;
				int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
				for (int j = 1; j < array2.Length; j++)
				{
					if (!MUJieripartChongzhiKingItem.IsTongGuo(array2[j], roleOcc))
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(array2[j].SafeToInt32(0));
						if (goodsXmlNodeByID != null)
						{
							string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
							string backSpriteName = "bagGrid4_bak";
							GoodsData dummyGoodsData = Global.GetDummyGoodsData(array2[j].SafeToInt32(0));
							dummyGoodsData.Binding = 1;
							GGoodIcon icon = mujieriLianxuChongzhiItemGoodsItem.GoodIcon;
							icon.Width = 78.0;
							icon.Height = 78.0;
							icon.isAutoSize = true;
							icon.BackSpriteName0 = backSpriteName;
							icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
							icon.TipType = 1;
							icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
							{
								array2[j].SafeToInt32(0),
								0,
								-1,
								-1
							});
							icon.ItemCode = array2[j].SafeToInt32(0);
							icon.ItemObject = dummyGoodsData;
							icon.BoxTypes = 5;
							icon.TextShadowColor = 4278190080U;
							icon.TextColor = 16777215U;
							icon.DisableTextColor = 8421504U;
							icon.TextHorizontalAlignment = global::Layout.Right;
							icon.TextVerticalAlignment = global::Layout.Bottom;
							icon.STextVisibility = false;
							bool canUse = Global.CanUseGoods(dummyGoodsData.GoodsID, false, true);
							Super.InitGoodsGIcon(icon, dummyGoodsData, canUse, IconTextTypes.Qianghua);
							icon.gameObject.AddComponent<UIDragPanelContents>();
							icon.addEventListener("click", delegate(MouseEvent e)
							{
								GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
								if (null == ggoodIcon)
								{
									return;
								}
								GoodsData goodsData = icon.ItemObject as GoodsData;
								if (goodsData == null)
								{
									return;
								}
								GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
							});
						}
					}
				}
				mujieriLianxuChongzhiItemGoodsItem.AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;
				mujieriLianxuChongzhiItemGoodsItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					GameInstance.Game.GetJieriChongzhiRewardCmd(e.ID, e.Index);
				};
				this.itemCollection.Add(mujieriLianxuChongzhiItemGoodsItem);
				UIPanel component = mujieriLianxuChongzhiItemGoodsItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
				if (!this.DicIDtoItem.ContainsKey(array2[0].SafeToInt32(0)))
				{
					this.DicIDtoItem.Add(array2[0].SafeToInt32(0), mujieriLianxuChongzhiItemGoodsItem);
				}
			}
		}
	}

	public void SetInitState(int day, int flag)
	{
		MUJieriLianxuChongzhiItemGoodsItem mujieriLianxuChongzhiItemGoodsItem = null;
		foreach (int num in this.DicIDtoItem.Keys)
		{
			if (day >= num && this.DicIDtoItem.TryGetValue(num, ref mujieriLianxuChongzhiItemGoodsItem))
			{
				if (Global.GetIntSomeBit(flag, num) == 0)
				{
					mujieriLianxuChongzhiItemGoodsItem.AwardGiftGainState = JieriAwardGiftGainState.CanGain;
				}
				else
				{
					mujieriLianxuChongzhiItemGoodsItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
				}
			}
		}
	}

	public void SetLianxuChongzhiState(int day)
	{
		MUJieriLianxuChongzhiItemGoodsItem mujieriLianxuChongzhiItemGoodsItem = null;
		if (this.DicIDtoItem.TryGetValue(day, ref mujieriLianxuChongzhiItemGoodsItem))
		{
			mujieriLianxuChongzhiItemGoodsItem.AwardGiftGainState = JieriAwardGiftGainState.Gained;
			Super.HintMainText(Global.GetLang("领取成功！"), 10, 3);
		}
	}

	public TextBlock NeedZuanshiNum;

	public ListBox goodsList;

	public TextBlock staticText;

	public ObservableCollection itemCollection;

	private Dictionary<int, MUJieriLianxuChongzhiItemGoodsItem> DicIDtoItem = new Dictionary<int, MUJieriLianxuChongzhiItemGoodsItem>();

	private string _zuanshiNum = string.Empty;
}
