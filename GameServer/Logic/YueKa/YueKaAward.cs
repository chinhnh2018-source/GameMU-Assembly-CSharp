using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic.YueKa
{
	internal class YueKaAward
	{
		public void Init(XElement xml)
		{
			this.Day = (int)Global.GetSafeAttributeLong(xml, "Day");
			this.BindZuanShi = (int)Global.GetSafeAttributeLong(xml, "BandZuanShiAward");
			this._InitGoods(this.AllGoodsList, Global.GetSafeAttributeStr(xml, "GoodsOne"));
			this._InitGoods(this.OccGoodsList, Global.GetSafeAttributeStr(xml, "GoodsTwo"));
		}

		public List<GoodsData> GetGoodsByOcc(int occ)
		{
			List<GoodsData> list = new List<GoodsData>();
			foreach (Tuple<int, int, int, int, int, int, int> tuple in this.AllGoodsList)
			{
				GoodsData item = this._ParseGoodsFromDetail(tuple);
				list.Add(item);
			}
			foreach (Tuple<int, int, int, int, int, int, int> tuple in this.OccGoodsList)
			{
				if (Global.IsRoleOccupationMatchGoods(occ, tuple.Item1))
				{
					GoodsData item = this._ParseGoodsFromDetail(tuple);
					list.Add(item);
				}
			}
			return list;
		}

		private GoodsData _ParseGoodsFromDetail(Tuple<int, int, int, int, int, int, int> detail)
		{
			return new GoodsData
			{
				Id = -1,
				GoodsID = detail.Item1,
				Using = 0,
				Forge_level = detail.Item4,
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = 0,
				Quality = 0,
				Props = "",
				GCount = detail.Item2,
				Binding = detail.Item3,
				Jewellist = "",
				BagIndex = 0,
				AddPropIndex = 0,
				BornIndex = 0,
				Lucky = detail.Item6,
				Strong = 0,
				ExcellenceInfo = detail.Item7,
				AppendPropLev = detail.Item5,
				ChangeLifeLevForEquip = 0
			};
		}

		private void _InitGoods(List<Tuple<int, int, int, int, int, int, int>> lst, string goods)
		{
			if (!string.IsNullOrEmpty(goods))
			{
				string[] array = goods.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 7)
					{
						int item = Convert.ToInt32(array3[0]);
						int item2 = Convert.ToInt32(array3[1]);
						int item3 = Convert.ToInt32(array3[2]);
						int item4 = Convert.ToInt32(array3[3]);
						int item5 = Convert.ToInt32(array3[4]);
						int item6 = Convert.ToInt32(array3[5]);
						int item7 = Convert.ToInt32(array3[6]);
						lst.Add(new Tuple<int, int, int, int, int, int, int>(item, item2, item3, item4, item5, item6, item7));
					}
				}
			}
		}

		public int Day = 0;

		public int BindZuanShi = 0;

		public List<Tuple<int, int, int, int, int, int, int>> AllGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();

		public List<Tuple<int, int, int, int, int, int, int>> OccGoodsList = new List<Tuple<int, int, int, int, int, int, int>>();
	}
}
