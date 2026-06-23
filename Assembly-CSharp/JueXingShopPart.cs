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

public class JueXingShopPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.NoticeRefreshRoleMoney();
		this.mCrusadeStoreXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeStoreXmlInstance();
		GameInstance.Game.SendGetKuFuPlubdeJueXingShopData();
	}

	protected override void OnDestroy()
	{
		this.StopTimeTicks();
		base.OnDestroy();
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposadeCrusadeStoreXml();
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer != null)
		{
			this.mDispatcherTimer.Stop();
			this.mDispatcherTimer.Dispose();
			this.mDispatcherTimer = null;
		}
	}

	private void StartTimeTicks()
	{
		this.StopTimeTicks();
		this.mDispatcherTimer = null;
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderRankPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		if (DateTime.MinValue != this.mLastRefreshTime)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			int num = (int)this.mCrusadeStoreCD - (int)(correctDateTime - this.mLastRefreshTime).TotalSeconds;
			if (num <= 0)
			{
				this.mTimeLable.text = Global.GetLang("商品刷新免费");
			}
			else
			{
				this.mTimeLable.text = Global.GetLang("商品刷新倒计时：") + Environment.NewLine + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					this.ParseDateTimeToChanese(num)
				});
			}
		}
		else
		{
			this.mTimeLable.text = string.Empty;
		}
	}

	private string ParseDateTimeToChanese(int seconds)
	{
		int[] array = new int[]
		{
			default(int),
			default(int),
			default(int),
			seconds % 60
		};
		array[2] = (seconds - array[3]) / 60 % 60;
		array[1] = (seconds - array[2] * 60 - array[3]) / 3600 % 24;
		array[0] = (seconds - array[1] * 3600 - array[2] * 60 - array[3]) / 86400;
		string text = string.Empty;
		byte b = 0;
		string[] array2 = new string[]
		{
			Global.GetLang("天"),
			Global.GetLang("小时"),
			Global.GetLang("分钟"),
			Global.GetLang("秒")
		};
		byte b2 = 0;
		while ((int)b2 < array.Length)
		{
			if (array[(int)b2] > 0)
			{
				text = text + array[(int)b2].ToString() + array2[(int)b2];
				b = 1;
			}
			else if (b == 1)
			{
				text = text + array[(int)b2].ToString() + array2[(int)b2];
			}
			b2 += 1;
		}
		return text;
	}

	private MallItem GetMallItem(int Index)
	{
		MallItem mallItem = null;
		GameObject at = this.mOBCMall.GetAt(Index);
		if (null != at)
		{
			mallItem = at.GetComponent<MallItem>();
			if (null == mallItem)
			{
				Object.Destroy(at);
			}
		}
		if (null == mallItem)
		{
			mallItem = U3DUtils.NEW<MallItem>();
			this.mOBCMall.Add(mallItem);
		}
		mallItem.gameObject.SetActive(true);
		return mallItem;
	}

	private IEnumerator RefreshView()
	{
		int index = 0;
		Dictionary<int, CrusadeStoreVO>.Enumerator itr = this.mCrusadeStoreXml.GetEnumerator();
		while (itr.MoveNext())
		{
			KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData = null;
			if (this.mKuaFuLueDuoStoreData != null && this.mKuaFuLueDuoStoreData.SaleList != null && 0 < this.mKuaFuLueDuoStoreData.SaleList.Count)
			{
				kuaFuLueDuoStoreSaleData = this.mKuaFuLueDuoStoreData.SaleList.Find(delegate(KuaFuLueDuoStoreSaleData e)
				{
					int id = e.ID;
					KeyValuePair<int, CrusadeStoreVO> keyValuePair8 = itr.Current;
					return id == keyValuePair8.Value.ID;
				});
			}
			if (kuaFuLueDuoStoreSaleData != null)
			{
				int index2;
				index = (index2 = index) + 1;
				MallItem item = this.GetMallItem(index2);
				MallItem mallItem = item;
				KeyValuePair<int, CrusadeStoreVO> keyValuePair = itr.Current;
				mallItem.GoodsDataInfo = keyValuePair.Value.Good;
				MallItem mallItem2 = item;
				KeyValuePair<int, CrusadeStoreVO> keyValuePair2 = itr.Current;
				mallItem2.GoodsName = Global.GetGoodsNameByID(keyValuePair2.Value.Good.GoodsID, true);
				int num2 = 0;
				KeyValuePair<int, CrusadeStoreVO> keyValuePair3 = itr.Current;
				if (num2 < keyValuePair3.Value.ZuanShiNum)
				{
					item.goodsOwnerTypes = GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond;
					MallItem mallItem3 = item;
					KeyValuePair<int, CrusadeStoreVO> keyValuePair4 = itr.Current;
					mallItem3.GoodsPrice = keyValuePair4.Value.ZuanShiNum.ToString();
				}
				else
				{
					item.goodsOwnerTypes = GoodsOwnerTypes.KuaFuPlunderJueXingShop;
					MallItem mallItem4 = item;
					KeyValuePair<int, CrusadeStoreVO> keyValuePair5 = itr.Current;
					mallItem4.GoodsPrice = keyValuePair5.Value.JueXingNum.ToString();
				}
				KeyValuePair<int, CrusadeStoreVO> keyValuePair6 = itr.Current;
				int num = keyValuePair6.Value.SinglePurchase - kuaFuLueDuoStoreSaleData.Purchase;
				if (0 > num)
				{
					num = 0;
				}
				item.wangzheXianGou.gameObject.SetActive(true);
				item.wangzheXianGouLabel.text = num.ToString();
				item.ItemID = kuaFuLueDuoStoreSaleData.ID;
				MallItem mallItem5 = item;
				KeyValuePair<int, CrusadeStoreVO> keyValuePair7 = itr.Current;
				mallItem5.MallGoodsID = keyValuePair7.Value.Good.GoodsID;
				item.ItemType = 0;
				item.InitGoodsIcon();
				item.ItemIcon.gameObject.transform.localPosition = new Vector3(item.ItemIcon.gameObject.transform.localPosition.x, item.ItemIcon.gameObject.transform.localPosition.y, -1f);
				item.MouseLeftButtonUpEX = delegate(object ls, MouseEvent ml)
				{
					this.SelectID = ml.IDType;
				};
				item.ItemIcon.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e1)
				{
					int num3 = 0;
					if (e1 != null)
					{
						num3 = e1.MyID;
					}
					MallItem mallItem6 = null;
					for (int i = 0; i < this.mOBCMall.Count; i++)
					{
						GameObject at = this.mOBCMall.GetAt(i);
						if (null != at)
						{
							mallItem6 = at.GetComponent<MallItem>();
							if (null != mallItem6 && mallItem6.MallGoodsID == num3)
							{
								break;
							}
						}
					}
					if (this.SelectID != -1)
					{
						for (int j = 0; j < this.mOBCMall.Count; j++)
						{
							GameObject at2 = this.mOBCMall.GetAt(j);
							if (null != at2)
							{
								mallItem6 = at2.GetComponent<MallItem>();
								if (null != mallItem6 && mallItem6.ItemID == this.SelectID)
								{
									break;
								}
							}
						}
					}
					if (null == mallItem6)
					{
						mallItem6 = U3DUtils.AS<MallItem>(this.mMallRooeList.SelectedItem);
					}
					if (null != mallItem6 && e1.IDType == 8)
					{
						if (mallItem6.GoodsOwnerTypesEX == GoodsOwnerTypes.KuaFuPlunderJueXingShop_Diamond)
						{
							if (Global.Data.roleData.Gold < int.Parse(mallItem6.GoodsPrice) * e1.ID)
							{
								Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
								return;
							}
						}
						else if (Global.Data.roleData.MoneyData[132] < (long)(int.Parse(mallItem6.GoodsPrice) * e1.ID))
						{
							Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
							return;
						}
						this.StartGouMai(mallItem6.MallGoodsID, mallItem6.ItemID, e1.ID, mallItem6.ItemType != 0);
					}
				};
				yield return null;
			}
		}
		yield break;
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTimeLable.Margin = new Vector2(4f, 8f);
			this.mTimeLable.text = string.Empty;
			this.mRefreshBtn.Label.text = Global.GetLang("刷新");
			this.mRefreshPicIconLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("消耗：")
			});
			this.mRefreshPicLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				ConfigSystemParam.GetSystemParamByName("CrusadeStorePrice", true)
			});
			this.mCrusadeStoreCD = ConfigSystemParam.GetSystemParamIntByName("CrusadeStoreCD");
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
			this.mTitleSp.spriteName = "JueXingShangCheng";
			this.mShopGrilImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/ShopGril.png";
			this.mShopGrilImage.ImageDownloaded = delegate(object g)
			{
				this.mShopGrilImage.transform.localScale = new Vector3((float)this.mShopGrilImage.ItsSizeWidth, (float)this.mShopGrilImage.ItsSizeHeight, 0f);
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

	private void InitHandler()
	{
		try
		{
			this.mOBCMall = this.mMallRooeList.ItemsSource;
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mMallRooeList.MouseLeftButtonDownEx = new MouseLeftButtonUpEventHandler(this.ItemSelectChange);
			string[] array = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			this.mRefreshBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				int num = (int)this.mCrusadeStoreCD - (correctDateTime - this.mLastRefreshTime).Seconds;
				if (num <= 0)
				{
					GameInstance.Game.SendKuaFuPlunderJUeXingShopRefresh();
				}
				else
				{
					if (Global.Data.roleData.UserMoney < ConfigSystemParam.GetSystemParamByName("CrusadeStorePrice", true).SafeToInt32(0))
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
						return;
					}
					Super.ZuanShiShowMessageBox(Global.GetLang("提示"), Global.GetLang("确定花费      ") + ConfigSystemParam.GetSystemParamByName("CrusadeStorePrice", true) + Global.GetLang("刷新觉醒商城\n中物品？"), 11, delegate(object g, DPSelectedItemEventArgs j)
					{
						if (j != null && j.ID == 0)
						{
							GameInstance.Game.SendKuaFuPlunderJUeXingShopRefresh();
						}
					}, MessBoxIsHintTypes.None, -130f, string.Empty, 0);
				}
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

	private void ItemSelectChange(object sender, MouseEvent e)
	{
		if (e != null && null != e.target)
		{
			MallItem component = e.target.GetComponent<MallItem>();
			if (null != component)
			{
				component.ItemIcon.MouseLeftButtonUp(sender, e);
			}
		}
	}

	private void StartGouMai(int buyGoodsID, int itemID, int num, bool isXianGou = false)
	{
		if (Global.IsBagFull())
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再购买！"), new object[0]), 10, 3);
			return;
		}
		GameInstance.Game.SendKuaFuPlunderJUeXingShopBuy(itemID, num);
	}

	public void NoticeGetShopDataCallBack(KuaFuLueDuoStoreData data)
	{
		if (data != null)
		{
			this.mLastRefreshTime = data.LastRefTime;
		}
		this.mKuaFuLueDuoStoreData = data;
		base.StartCoroutine<bool>(this.RefreshView());
	}

	public void NoticeRefreshRoleMoney()
	{
		this.mRoleMoney.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.Data.roleData.UserMoney
		});
		this.mRoleJueXingMoney.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.Data.roleData.MoneyData[132]
		});
		this.StartTimeTicks();
	}

	public void NoticeBuyGoodsCallBack(int ret)
	{
		if (ret == 0)
		{
			GameInstance.Game.SendGetKuFuPlubdeJueXingShopData();
		}
		else if (ret == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("购买的物品已经从商城中下架了"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -2)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else if (ret == -3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请清理出空格后再购买"), new object[0]), 1, -1, -1, 0);
		}
		else if (ret == -20003)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("该商品已经出售完毕"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -20004)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("购买个数超过总剩余个数"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -20005)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("抢购限额已达上限，无法购买"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -20006)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("购买个数超过购买限额剩余个数"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -20)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("购买物品不存在"), new object[0]), 0, -1, -1, 0);
		}
		else if (ret == -36)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("限购次数不足"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("从商城购买物品失败:{0}"), new object[]
			{
				ret
			}), 0, -1, -1, 0);
		}
	}

	private const long mTicksPerMillisecond = 10000L;

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private UISprite mTitleSp;

	[SerializeField]
	private ShowNetImage mShopGrilImage;

	[SerializeField]
	private UILabel mTimeLable;

	[SerializeField]
	private UILabel mRefreshPicIconLabel;

	[SerializeField]
	private UILabel mRefreshPicLabel;

	[SerializeField]
	private GButton mRefreshBtn;

	[SerializeField]
	private UILabel mRoleMoney;

	[SerializeField]
	private UILabel mRoleJueXingMoney;

	[SerializeField]
	private ListBox mMallRooeList;

	private DispatcherTimer mDispatcherTimer;

	private long mCrusadeStoreCD;

	private CrusadeStoreXml mCrusadeStoreXml;

	private ObservableCollection mOBCMall;

	private KuaFuLueDuoStoreData mKuaFuLueDuoStoreData;

	private DateTime mLastRefreshTime = DateTime.MinValue;

	private int SelectID = -1;

	public DPSelectedItemEventHandler Hander;
}
