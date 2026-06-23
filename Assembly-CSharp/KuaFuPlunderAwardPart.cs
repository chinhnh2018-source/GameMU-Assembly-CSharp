using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class KuaFuPlunderAwardPart : UserControl
{
	private void RuQinNoneAward()
	{
		NGUITools.SetActive(this.mLblRuQinNoneAward.gameObject, true);
		NGUITools.SetActive(this.mRuQinNoneAwardObj, false);
	}

	private void FangShouNoneAward()
	{
		NGUITools.SetActive(this.mLblFangShouNoneAward.gameObject, true);
		NGUITools.SetActive(this.mFangShouNoneAwardObj, false);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		NGUITools.SetActive(this.mLblRuQinNoneAward.gameObject, false);
		NGUITools.SetActive(this.mLblFangShouNoneAward.gameObject, false);
	}

	private void InitTextInPrefabs()
	{
		this.mBtnLingQu.Text = Global.GetLang("领取");
		this.mLblTitle.Text = Global.GetLang("领取奖励");
		this.mLblRuQinTitle.Text = Global.GetLang("入侵奖励");
		this.mLblFangShouTitle.Text = Global.GetLang("防守奖励");
		this.LUEDUOZIYUAN = Global.GetLang("掠夺资源：");
		this.FANGSHOUZIYUAN = Global.GetLang("防守资源：");
		this.GeRENJIFEN = Global.GetLang("个人积分：");
		this.mLblRuQinZiYuan.Text = this.LUEDUOZIYUAN + 0;
		this.mLblRuQinJiFen.Text = this.GeRENJIFEN + 0;
		this.mLblFangShouZiYuan.Text = this.FANGSHOUZIYUAN + 0;
		this.mLblFangShouJiFen.Text = this.GeRENJIFEN + 0;
		this.mLblRuQinNoneAward.Text = Global.GetLang("无奖励");
		this.mLblFangShouNoneAward.Text = Global.GetLang("无奖励");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnLingQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendGetKuaFuPlundeGetAward();
		};
	}

	private void InitValue()
	{
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
	}

	private void AddAwards(GameObject parent, long ExpValue, int jueXingValue, int bangJinValue)
	{
		List<GoodsData> showGoodsDataListByID = this.mCrusadeWarXml.GetShowGoodsDataListByID(1);
		if (showGoodsDataListByID != null)
		{
			for (int i = 0; i < showGoodsDataListByID.Count; i++)
			{
				GoodsData goodsData = showGoodsDataListByID[i];
				GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
				newGoodIcon.GoodsID = goodsData.GoodsID;
				newGoodIcon.Width = 70.0;
				newGoodIcon.Height = 70.0;
				newGoodIcon.ItemCategory = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				newGoodIcon.ItemObject = goodsData;
				newGoodIcon.isAutoSize = true;
				newGoodIcon.SecondText.transform.localPosition = new Vector3(newGoodIcon.SecondText.transform.localPosition.x, newGoodIcon.SecondText.transform.localPosition.y, -4f);
				newGoodIcon.BackSpriteName0 = "bagGrid4_bak";
				newGoodIcon.BackgroundSprite0.gameObject.transform.localScale = new Vector3(80f, 80f, 0f);
				newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsData.GoodsID), string.Empty)
				}), false, 0);
				newGoodIcon.Tip = Global.GetGoodsNameByID(goodsData.GoodsID, false);
				Super.InitGoodsGIcon(newGoodIcon, goodsData, true, IconTextTypes.Qianghua);
				if (i == 0)
				{
					if (ExpValue <= 0L)
					{
						NGUITools.SetActive(newGoodIcon.gameObject, false);
					}
					else
					{
						newGoodIcon.SecondText.Text = this.GetNumber(ExpValue);
					}
				}
				else if (i == 1)
				{
					if (bangJinValue <= 0)
					{
						NGUITools.SetActive(newGoodIcon.gameObject, false);
					}
					else
					{
						newGoodIcon.SecondText.Text = this.GetNumber(bangJinValue);
					}
				}
				else if (i == 2)
				{
					if (jueXingValue <= 0)
					{
						NGUITools.SetActive(newGoodIcon.gameObject, false);
					}
					else
					{
						newGoodIcon.SecondText.Text = this.GetNumber(jueXingValue);
					}
				}
				newGoodIcon.transform.SetParent(parent.transform, false);
				newGoodIcon.transform.localPosition = new Vector3((float)(-84 + 84 * i), 0f, 0f);
				newGoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			}
		}
	}

	private string GetNumber(long value)
	{
		if (value > 10000L)
		{
			return string.Format(Global.GetLang("{0}万"), (int)((double)value / 10000.0));
		}
		return value.ToString();
	}

	private string GetNumber(int value)
	{
		if (value > 10000)
		{
			return string.Format(Global.GetLang("{0}万"), (int)((double)value / 10000.0));
		}
		return value.ToString();
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

	public void RefreshDataByServer(List<KuaFuLueDuoAwardsData> result)
	{
		if (result == null || result.Count <= 0)
		{
			return;
		}
		if (result.Count == 1)
		{
			if (result[0].type == 2)
			{
				this.FangShouNoneAward();
				this.mLblRuQinZiYuan.Text = this.GetString(new object[]
				{
					this.LUEDUOZIYUAN,
					result[0].ZiYuan
				});
				this.mLblRuQinJiFen.Text = this.GetString(new object[]
				{
					this.GeRENJIFEN,
					result[0].JiFen
				});
				this.AddAwards(this.mRuQinAwardObj, result[0].Exp, result[0].JueXing, result[0].BindJinBi);
			}
			else if (result[0].type == 1)
			{
				this.RuQinNoneAward();
				this.mLblFangShouZiYuan.Text = this.GetString(new object[]
				{
					this.FANGSHOUZIYUAN,
					result[0].ZiYuan
				});
				this.mLblFangShouJiFen.Text = this.GetString(new object[]
				{
					this.GeRENJIFEN,
					result[0].JiFen
				});
				this.AddAwards(this.mFangShouAwardObj, result[0].Exp, result[0].JueXing, result[0].BindJinBi);
			}
		}
		else
		{
			for (int i = 0; i < result.Count; i++)
			{
				if (result[i].type == 2)
				{
					if (result[i].Exp == 0L || result[i].BindJinBi == 0)
					{
						this.RuQinNoneAward();
					}
					else
					{
						this.mLblRuQinZiYuan.Text = this.GetString(new object[]
						{
							this.LUEDUOZIYUAN,
							result[i].ZiYuan
						});
						this.mLblRuQinJiFen.Text = this.GetString(new object[]
						{
							this.GeRENJIFEN,
							result[i].JiFen
						});
						this.AddAwards(this.mRuQinAwardObj, result[i].Exp, result[i].JueXing, result[i].BindJinBi);
					}
				}
				else if (result[i].type == 1)
				{
					if (result[i].Exp == 0L || result[i].BindJinBi == 0)
					{
						this.FangShouNoneAward();
					}
					else
					{
						this.mLblFangShouZiYuan.Text = this.GetString(new object[]
						{
							this.FANGSHOUZIYUAN,
							result[i].ZiYuan
						});
						this.mLblFangShouJiFen.Text = this.GetString(new object[]
						{
							this.GeRENJIFEN,
							result[i].JiFen
						});
						this.AddAwards(this.mFangShouAwardObj, result[i].Exp, result[i].JueXing, result[i].BindJinBi);
					}
				}
			}
		}
	}

	public string GetString(params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < args.Length; i++)
		{
			stringBuilder.Append(args[i]);
		}
		return stringBuilder.ToString();
	}

	protected override void OnDestroy()
	{
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public GButton mBtnLingQu;

	public TextBlock mLblRuQinTitle;

	public TextBlock mLblRuQinZiYuan;

	public TextBlock mLblRuQinJiFen;

	public GameObject mRuQinAwardObj;

	public TextBlock mLblFangShouTitle;

	public TextBlock mLblFangShouZiYuan;

	public TextBlock mLblFangShouJiFen;

	public GameObject mFangShouAwardObj;

	private string LUEDUOZIYUAN = string.Empty;

	private string FANGSHOUZIYUAN = string.Empty;

	private string GeRENJIFEN = string.Empty;

	private CrusadeWarXml mCrusadeWarXml;

	public GameObject mRuQinNoneAwardObj;

	public TextBlock mLblRuQinNoneAward;

	public GameObject mFangShouNoneAwardObj;

	public TextBlock mLblFangShouNoneAward;
}
