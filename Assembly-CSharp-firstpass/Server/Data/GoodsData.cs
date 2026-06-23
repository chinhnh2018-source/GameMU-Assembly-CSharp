using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GoodsData
	{
		public object Clone1()
		{
			GoodsData result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Serializer.Serialize<GoodsData>(memoryStream, this);
				memoryStream.Position = 0L;
				result = Serializer.Deserialize<GoodsData>(memoryStream);
			}
			return result;
		}

		public GoodsData Clone(int goodsID)
		{
			return new GoodsData
			{
				AddPropIndex = this.AddPropIndex,
				AppendPropLev = this.AppendPropLev,
				BagIndex = this.BagIndex,
				Binding = this.Binding,
				BornIndex = this.BornIndex,
				ChangeLifeLevForEquip = this.ChangeLifeLevForEquip,
				ElementhrtsProps = this.ElementhrtsProps,
				Endtime = this.Endtime,
				ExcellenceInfo = this.ExcellenceInfo,
				Forge_level = this.Forge_level,
				GCount = this.GCount,
				GoodsID = goodsID,
				Id = -1,
				Jewellist = this.Jewellist,
				JuHunID = this.JuHunID,
				Lucky = this.Lucky,
				Props = this.Props,
				Quality = this.Quality,
				SaleMoney1 = this.SaleMoney1,
				SaleYinPiao = this.SaleYinPiao,
				SaleYuanBao = this.SaleYuanBao,
				Site = this.Site,
				Starttime = this.Starttime,
				Strong = this.Strong,
				Using = this.Using,
				WashProps = this.WashProps
			};
		}

		public GoodsData Clone()
		{
			return new GoodsData
			{
				AddPropIndex = this.AddPropIndex,
				AppendPropLev = this.AppendPropLev,
				BagIndex = this.BagIndex,
				Binding = this.Binding,
				BornIndex = this.BornIndex,
				ChangeLifeLevForEquip = this.ChangeLifeLevForEquip,
				ElementhrtsProps = this.ElementhrtsProps,
				Endtime = this.Endtime,
				ExcellenceInfo = this.ExcellenceInfo,
				Forge_level = this.Forge_level,
				GCount = this.GCount,
				GoodsID = this.GoodsID,
				Id = this.Id,
				Jewellist = this.Jewellist,
				JuHunID = this.JuHunID,
				Lucky = this.Lucky,
				Props = this.Props,
				Quality = this.Quality,
				SaleMoney1 = this.SaleMoney1,
				SaleYinPiao = this.SaleYinPiao,
				SaleYuanBao = this.SaleYuanBao,
				Site = this.Site,
				Starttime = this.Starttime,
				Strong = this.Strong,
				Using = this.Using,
				WashProps = this.WashProps
			};
		}

		[ProtoMember(1)]
		public int Id;

		[ProtoMember(2)]
		public int GoodsID;

		[ProtoMember(3)]
		public int Using;

		[ProtoMember(4)]
		public int Forge_level;

		[ProtoMember(5)]
		public string Starttime;

		[ProtoMember(6)]
		public string Endtime;

		[ProtoMember(7)]
		public int Site;

		[ProtoMember(8)]
		public int Quality;

		[ProtoMember(9)]
		public string Props;

		[ProtoMember(10)]
		public int GCount;

		[ProtoMember(11)]
		public int Binding;

		[ProtoMember(12)]
		public string Jewellist;

		[ProtoMember(13)]
		public int BagIndex;

		[ProtoMember(14)]
		public int SaleMoney1;

		[ProtoMember(15)]
		public int SaleYuanBao;

		[ProtoMember(16)]
		public int SaleYinPiao;

		[ProtoMember(17)]
		public int AddPropIndex;

		[ProtoMember(18)]
		public int BornIndex;

		[ProtoMember(19)]
		public int Lucky;

		[ProtoMember(20)]
		public int Strong;

		[ProtoMember(21)]
		public int ExcellenceInfo;

		[ProtoMember(22)]
		public int AppendPropLev;

		[ProtoMember(23)]
		public int ChangeLifeLevForEquip;

		[ProtoMember(24)]
		public List<int> WashProps;

		[ProtoMember(25)]
		public List<int> ElementhrtsProps;

		[ProtoMember(26)]
		public int JuHunID;
	}
}
