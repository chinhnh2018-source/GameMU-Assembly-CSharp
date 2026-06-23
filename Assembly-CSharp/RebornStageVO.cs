using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class RebornStageVO
{
	public RebornStageVO(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.NeedZhuanSheng = Global.GetXElementAttributeStr(xml, "NeedZhuanSheng");
		this.NeedRebornLevel = Global.GetXElementAttributeInt(xml, "NeedRebornLevel");
		this.NeedZhanLi = Global.GetXElementAttributeInt(xml, "NeedZhanLi");
		this.NeedMaxWing = Global.GetXElementAttributeStr(xml, "NeedMaxWing");
		this.NeedChengJie = Global.GetXElementAttributeInt(xml, "NeedChengJie");
		this.NeedShengWang = Global.GetXElementAttributeInt(xml, "NeedShengWang");
		this.NeedMagicBook = Global.GetXElementAttributeStr(xml, "NeedMagicBook");
		this.MaxRebornLevel = Global.GetXElementAttributeInt(xml, "MaxRebornLevel");
		this.RebornDian = Global.GetXElementAttributeInt(xml, "RebornDian");
		this.ExtProp = Global.GetXElementAttributeStr(xml, "ExtProp");
		this.AwardGoods = Global.GetXElementAttributeStr(xml, "AwardGoods");
		this.Show = Global.GetXElementAttributeStr(xml, "Show");
		this.NeedIntro = Global.GetXElementAttributeStr(xml, "NeedIntro");
		this.GetIntro = Global.GetXElementAttributeStr(xml, "GetIntro");
		this.Wing = Global.GetXElementAttributeInt(xml, "Wing");
	}

	public List<GoodsData> ShowModalGoodsDataList
	{
		get
		{
			if (this.mShowModalGoodsID == null)
			{
				this.mShowModalGoodsID = new List<GoodsData>();
				string[] array = this.Show.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(array2[0].SafeToInt32(0), array2[3].SafeToInt32(0), array2[4].SafeToInt32(0), array2[6].SafeToInt32(0), array2[5].SafeToInt32(0), array2[2].SafeToInt32(0), array2[1].SafeToInt32(0), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
						this.mShowModalGoodsID.Add(dummyGoodsDataMu);
						dummyGoodsDataMu.Using = 1;
					}
				}
			}
			return this.mShowModalGoodsID;
		}
	}

	public List<GoodsData> AwardShowGoodsGoodsDataList
	{
		get
		{
			if (this.mAwardGoodsGoodsID == null)
			{
				this.mAwardGoodsGoodsID = new List<GoodsData>();
				string[] array = this.AwardGoods.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(array2[0].SafeToInt32(0), array2[3].SafeToInt32(0), array2[4].SafeToInt32(0), array2[6].SafeToInt32(0), array2[5].SafeToInt32(0), array2[2].SafeToInt32(0), array2[1].SafeToInt32(0), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
						this.mAwardGoodsGoodsID.Add(dummyGoodsDataMu);
					}
				}
			}
			return this.mAwardGoodsGoodsID;
		}
	}

	public int[] NeedZhuanShengInf
	{
		get
		{
			int[] array = new int[2];
			if (!string.IsNullOrEmpty(this.NeedZhuanSheng))
			{
				if ("-1".Equals(this.NeedZhuanSheng))
				{
					array[0] = -1;
					array[1] = -1;
				}
				else
				{
					string[] array2 = this.NeedZhuanSheng.Split(new char[]
					{
						'|'
					});
					array[0] = array2[0].SafeToInt32(0);
					array[1] = array2[1].SafeToInt32(0);
				}
			}
			return array;
		}
	}

	public int[] NeedMagicBookInf
	{
		get
		{
			int[] array = new int[2];
			if (!string.IsNullOrEmpty(this.NeedMagicBook))
			{
				if ("-1".Equals(this.NeedMagicBook))
				{
					array[0] = -1;
					array[1] = -1;
				}
				else
				{
					string[] array2 = this.NeedMagicBook.Split(new char[]
					{
						'|'
					});
					array[0] = array2[0].SafeToInt32(0);
					array[1] = array2[1].SafeToInt32(0);
				}
			}
			return array;
		}
	}

	public int[] NeedMaxWingInf
	{
		get
		{
			int[] array = new int[]
			{
				-1,
				-1,
				-1,
				-1,
				-1
			};
			if (!string.IsNullOrEmpty(this.NeedMaxWing) && !"-1".Equals(this.NeedMaxWing))
			{
				string[] array2 = this.NeedMaxWing.Split(new char[]
				{
					'|'
				});
				array[0] = array2[0].SafeToInt32(0);
				array[1] = array2[1].SafeToInt32(0);
				array[2] = array2[2].SafeToInt32(0);
				array[3] = array2[3].SafeToInt32(0);
				array[4] = array2[4].SafeToInt32(0);
			}
			return array;
		}
	}

	public int ID;

	public string NeedZhuanSheng;

	public int NeedRebornLevel;

	public int NeedZhanLi;

	public string NeedMaxWing;

	public int NeedChengJie;

	public int NeedShengWang;

	public string NeedMagicBook;

	public int MaxRebornLevel;

	public int RebornDian;

	public int Wing;

	public string ExtProp;

	public string AwardGoods;

	public string Show;

	public string NeedIntro;

	public string GetIntro;

	private List<GoodsData> mShowModalGoodsID;

	private List<GoodsData> mAwardGoodsGoodsID;
}
