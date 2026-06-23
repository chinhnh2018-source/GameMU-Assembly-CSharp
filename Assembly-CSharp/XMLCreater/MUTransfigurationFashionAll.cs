using System;
using System.Collections.Generic;
using Tmsk.Xml;

namespace XMLCreater
{
	public class MUTransfigurationFashionAll
	{
		public MUTransfigurationFashionAll()
		{
		}

		public MUTransfigurationFashionAll(XElement xe)
		{
			this.m_TransfigurationFashions = new List<MUTransfigurationFashion>();
			IEnumerable<XElement> enumerable = xe.Elements("TransfigurationFashion");
			if (enumerable != null)
			{
				foreach (XElement xe2 in enumerable)
				{
					MUTransfigurationFashion mutransfigurationFashion = new MUTransfigurationFashion(xe2);
					this.m_TransfigurationFashions.Add(mutransfigurationFashion);
				}
			}
		}

		public List<MUTransfigurationFashion> TransfigurationFashions
		{
			get
			{
				return this.m_TransfigurationFashions;
			}
			set
			{
				this.m_TransfigurationFashions = value;
			}
		}

		public MUTransfigurationFashion GetTransfigurationFashionByID(int id)
		{
			if (this.m_TransfigurationFashions == null)
			{
				return null;
			}
			return this.m_TransfigurationFashions.Find((MUTransfigurationFashion info) => info.ID == id);
		}

		public MUTransfigurationFashion GetTransfigurationFashion(int goodsId, int level)
		{
			if (this.m_TransfigurationFashions == null)
			{
				return null;
			}
			return this.m_TransfigurationFashions.Find((MUTransfigurationFashion info) => info.GoodsID == goodsId && info.level == level);
		}

		public List<MUTransfigurationFashion> GetTransfigurationFashionByGoodsId(int goodsId)
		{
			List<MUTransfigurationFashion> list = new List<MUTransfigurationFashion>();
			if (this.m_TransfigurationFashions == null)
			{
				return list;
			}
			for (int i = 0; i < this.m_TransfigurationFashions.Count; i++)
			{
				if (this.m_TransfigurationFashions[i].GoodsID == goodsId)
				{
					list.Add(this.m_TransfigurationFashions[i]);
				}
			}
			return list;
		}

		private List<MUTransfigurationFashion> m_TransfigurationFashions;
	}
}
