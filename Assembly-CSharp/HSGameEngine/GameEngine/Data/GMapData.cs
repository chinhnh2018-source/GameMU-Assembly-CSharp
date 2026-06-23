using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameEngine.Data
{
	public class GMapData
	{
		public GMapData()
		{
			this.GameObjectList = new List<GMapItem>();
			this.SectionList = new List<Point2Section>();
			this.MapSections = new Dictionary<string, GMapSection>();
			this.GuangMuDict = new Dictionary<int, GGuangMuData>();
		}

		public int PKMode { get; set; }

		public int NotLostEquip { get; set; }

		public int IsolatedMap { get; set; }

		public int HoldRole { get; set; }

		public int HoldMonster { get; set; }

		public int HoldNPC { get; set; }

		public List<int> LimitMagicIDs { get; set; }

		public List<int> LimitGoodsIDs { get; set; }

		public int MinZhuanSheng { get; set; }

		public int MinLevel { get; set; }

		public int MapWidth { get; set; }

		public int MapHeight { get; set; }

		public int SectionWidth { get; set; }

		public int SectionHeight { get; set; }

		public int SectionXNum { get; set; }

		public int SectionZNum { get; set; }

		public int GridSizeX = 50;

		public int GridSizeY = 50;

		public int GridSizeXNum;

		public int GridSizeYNum;

		public byte[,] fixedObstruction;

		public byte[,] TerrainWithTeleports;

		public MapGrid _MapGrid;

		public List<GMapItem> GameObjectList;

		public List<Point2Section> SectionList;

		public Dictionary<string, GMapSection> MapSections;

		public Dictionary<int, GGuangMuData> GuangMuDict;
	}
}
