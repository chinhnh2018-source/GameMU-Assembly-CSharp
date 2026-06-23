using System;
using System.Collections.Generic;
using Tmsk.Xml;

public class ShengYinShengJiVO
{
	public ShengYinShengJiVO()
	{
	}

	public ShengYinShengJiVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.DaoJuID = xe.GetAttributeInt("DaoJuID", -1);
		this.LeiXing = xe.GetAttributeInt("LeiXing", -1);
		this.BuWei = xe.GetAttributeInt("BuWei", -1);
		this.PinZhi = xe.GetAttributeInt("PinZhi", -1);
		this.DengJi = xe.GetAttributeInt("DengJi", -1);
		this.ShengJiJingYan = xe.GetAttributeInt("ShengJiJingYan", -1);
		this.TunShiJingYan = xe.GetAttributeInt("TunShiJingYan", -1);
		this.ZhuoYueShuXingTiaoShu = xe.GetAttributeInt("ZhuoYueShuXingTiaoShu", -1);
		this.JiChuShuXing = xe.GetAttributeStr("JiChuShuXing");
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public int DaoJuID { get; set; }

	public int LeiXing { get; set; }

	public int BuWei { get; set; }

	public int PinZhi { get; set; }

	public int DengJi { get; set; }

	public int ShengJiJingYan { get; set; }

	public int TunShiJingYan { get; set; }

	public int ZhuoYueShuXingTiaoShu { get; set; }

	public string JiChuShuXing { get; set; }

	public Dictionary<string, double> JiChuProp
	{
		get
		{
			if (0 >= this.mJiChuProp.Count && !string.IsNullOrEmpty(this.JiChuShuXing))
			{
				string[] array = this.JiChuShuXing.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							this.mJiChuProp[array2[0]] = double.Parse(array2[1]);
						}
					}
				}
			}
			return this.mJiChuProp;
		}
	}

	private Dictionary<string, double> mJiChuProp = new Dictionary<string, double>();
}
