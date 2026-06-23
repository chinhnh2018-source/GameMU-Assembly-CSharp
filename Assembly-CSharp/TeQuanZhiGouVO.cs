using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class TeQuanZhiGouVO
{
	public int ZhiGouID
	{
		get
		{
			if (this.mZhiGouID == -1)
			{
				if (!string.IsNullOrEmpty(this.ZhiGouJiaGe))
				{
					string[] array = this.ZhiGouJiaGe.Split(new char[]
					{
						'|'
					});
					if (array.Length == 3)
					{
						this.mZhiGouID = array[2].SafeToInt32(0);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>ZhiGouJiaGe 未按照约定的格式配 </color>"
						});
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=yellow>ZhiGouJiaGe = null </color>"
					});
				}
			}
			return this.mZhiGouID;
		}
	}

	public int ChongZhiID
	{
		get
		{
			if (this.mChongZhiID == -1)
			{
				if (!string.IsNullOrEmpty(this.ZhiGouJiaGe))
				{
					string[] array = this.ZhiGouJiaGe.Split(new char[]
					{
						'|'
					});
					if (array.Length == 3)
					{
						this.mChongZhiID = array[1].SafeToInt32(0);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>ZhiGouJiaGe 未按照约定的格式配 </color>"
						});
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=yellow>ZhiGouJiaGe = null </color>"
					});
				}
			}
			return this.mChongZhiID;
		}
	}

	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.TeQuanID = Global.GetXElementAttributeInt(xml, "TeQuanID");
		this.ZhiGouJiaGe = Global.GetXElementAttributeStr(xml, "ZhiGouJiaGe");
		this.FenZhiYeJiangLi = Global.GetXElementAttributeStr(xml, "FenZhiYeJiangLi");
		this.XianShiJiangLi = Global.GetXElementAttributeStr(xml, "XianShiJiangLi");
		this.DaoJuJiangLi = Global.GetXElementAttributeStr(xml, "DaoJuJiangLi");
		this.CanShu = Global.GetXElementAttributeStr(xml, "CanShu");
		this.GouMaiCiShu = Global.GetXElementAttributeStr(xml, "GouMaiCiShu");
		this.ZhiGouPinZhi = Global.GetXElementAttributeInt(xml, "ZhiGouPinZhi");
		this.initWuPin();
	}

	private void initWuPin()
	{
		if (0 >= this.mWuPin.size && !string.IsNullOrEmpty(this.DaoJuJiangLi))
		{
			string[] array = this.DaoJuJiangLi.Split(new char[]
			{
				'|'
			});
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array[i], 0);
					this.mWuPin.Add(goodsDataByStr);
				}
			}
		}
	}

	public BetterList<GoodsData> WuPin
	{
		get
		{
			this.initWuPin();
			return this.mWuPin;
		}
	}

	public int ID;

	public int TeQuanID;

	public string ZhiGouJiaGe;

	private int mZhiGouID = -1;

	private int mChongZhiID = -1;

	public string DaoJuJiangLi;

	public string FenZhiYeJiangLi;

	public string XianShiJiangLi;

	public string CanShu;

	public string GouMaiCiShu;

	public int ZhiGouPinZhi;

	private BetterList<GoodsData> mWuPin = new BetterList<GoodsData>();
}
