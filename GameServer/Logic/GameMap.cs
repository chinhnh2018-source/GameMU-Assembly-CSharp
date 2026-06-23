using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using HSGameEngine.Tools.AStarEx;

namespace GameServer.Logic
{
	public class GameMap
	{
		public int PKMode { get; set; }

		public int NotLostEquip { get; set; }

		public int IsolatedMap { get; set; }

		public int HoldNPC { get; set; }

		public int HoldMonster { get; set; }

		public int HoldRole { get; set; }

		public int RealiveMode { get; set; }

		public int RealiveTime { get; set; }

		public int DayLimitSecs { get; set; }

		public DateTimeRange[] LimitTimes { get; set; }

		public int[] LimitGoodsIDs { get; set; }

		public int[] LimitBufferIDs { get; set; }

		public int LimitAuotFight { get; set; }

		public int[] LimitMagicIDs { get; set; }

		public int MinZhuanSheng { get; set; }

		public int MinLevel { get; set; }

		public int MapCode { get; set; }

		public int MapPicCode { get; set; }

		public int MapWidth { get; set; }

		public int MapHeight { get; set; }

		public int MapGridWidth { get; set; }

		public int MapGridHeight { get; set; }

		public int MapGridColsNum { get; set; }

		public int MapGridRowsNum { get; set; }

		public int DefaultBirthPosX { get; set; }

		public int DefaultBirthPosY { get; set; }

		public int BirthRadius { get; set; }

		public NodeGrid MyNodeGrid
		{
			get
			{
				return this._NodeGrid;
			}
		}

		public AStar MyAStarFinder
		{
			get
			{
				return this._AStarFinder;
			}
		}

		public bool InSafeRegionList(Point grid)
		{
			return this.InSafeRegionList((int)grid.X, (int)grid.Y);
		}

		public bool InSafeRegionList(int gridX, int gridY)
		{
			return gridX >= 0 && gridY >= 0 && this.SafeRegionArray.GetUpperBound(0) > gridX && this.SafeRegionArray.GetUpperBound(1) > gridY && 1 == this.SafeRegionArray[gridX, gridY];
		}

		public void SetPartialSafeRegion(Point grid, int gridNum)
		{
			if (null != this.SafeRegionArray)
			{
				int num = Math.Max(0, (int)grid.X - gridNum);
				int num2 = Math.Max(0, (int)grid.Y - gridNum);
				int num3 = Math.Min(this.MapGridColsNum - 1, (int)grid.X + gridNum);
				int num4 = Math.Min(this.MapGridRowsNum - 1, (int)grid.Y + gridNum);
				for (int i = num; i <= num3; i++)
				{
					for (int j = num2; j <= num4; j++)
					{
						this.SafeRegionArray[i, j] = 1;
					}
				}
			}
		}

		public int GetAreaLuaID(Point grid)
		{
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					return this.AreaLuaList[i].ID;
				}
			}
			return -1;
		}

		public List<int> GetAreaLuaIDListByPoint(Point grid)
		{
			List<int> list = null;
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(this.AreaLuaList[i].ID);
				}
			}
			return list;
		}

		public GAreaLua GetAreaLuaByID(int areaLuaID)
		{
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (this.AreaLuaList[i].ID == areaLuaID)
				{
					return this.AreaLuaList[i];
				}
			}
			return null;
		}

		public List<GAreaLua> GetAreaLuaListByPoint(Point grid)
		{
			List<GAreaLua> list = null;
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					if (list == null)
					{
						list = new List<GAreaLua>();
					}
					list.Add(this.AreaLuaList[i]);
				}
			}
			return list;
		}

		public int CorrectWidthPointToGridPoint(int value)
		{
			return value / this.MapGridWidth * this.MapGridWidth + this.MapGridWidth / 2;
		}

		public int CorrectHeightPointToGridPoint(int value)
		{
			return value / this.MapGridHeight * this.MapGridHeight + this.MapGridHeight / 2;
		}

		public int CorrectPointToGrid(int value)
		{
			return value / this.MapGridWidth;
		}

		public void InitMap()
		{
			if (!this.InitGameMapBinary())
			{
				this.LoadObstruction();
				this.LoadAnQuanQuXml();
			}
			this.LoadMapTeleportDict();
			this.LoadPathFinderFast();
			this.LoadMapConfig();
			this.LoadAreaLua();
			this.InitEnterMapLuaFile();
		}

		public bool InitGameMapBinary()
		{
			string uri = string.Format("MapConfig/{0}/obs.bytes", this.MapPicCode);
			string path = Global.MapConfigResPath(uri);
			bool result;
			if (File.Exists(path))
			{
				byte[] array = File.ReadAllBytes(path);
				this.MapWidth = ((int)array[0] | (int)array[1] << 8 | (int)array[2] << 16 | (int)array[3] << 24);
				this.MapHeight = ((int)array[4] | (int)array[5] << 8 | (int)array[6] << 16 | (int)array[7] << 24);
				this.MapWidth *= 100;
				this.MapHeight *= 100;
				this.MapGridWidth = (this.MapGridHeight = 100);
				int num = (this.MapWidth - 1) / this.MapGridWidth + 1;
				int num2 = (this.MapHeight - 1) / this.MapGridHeight + 1;
				this.MapGridColsNum = num;
				this.MapGridRowsNum = num2;
				num = (int)Math.Ceiling(Math.Log((double)num, 2.0));
				num = (int)Math.Pow(2.0, (double)num);
				num2 = (int)Math.Ceiling(Math.Log((double)num2, 2.0));
				num2 = (int)Math.Pow(2.0, (double)num2);
				this._NodeGrid = new NodeGrid(num, num2);
				this.SafeRegionArray = new byte[this.MapGridColsNum, this.MapGridRowsNum];
				for (int i = 0; i < this.MapGridColsNum; i++)
				{
					for (int j = 0; j < this.MapGridRowsNum; j++)
					{
						byte b = array[i * this.MapGridColsNum + j + 9];
						this._NodeGrid.setWalkable(i, j, b != 0);
						if (255 == b)
						{
							this.SafeRegionArray[i, j] = 1;
						}
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void LoadMapConfig()
		{
			Trace.Assert(this.MapGridWidth > 0);
			Trace.Assert(this.MapGridHeight > 0);
			string text = string.Format("Map/{0}/MapConfig.xml", this.MapCode);
			XElement xelement = null;
			try
			{
				xelement = Global.GetResXml(text);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", text));
			}
			XElement safeXElement = Global.GetSafeXElement(xelement, "Settings");
			this.PKMode = (int)Global.GetSafeAttributeLong(safeXElement, "PKMode");
			this.NotLostEquip = (int)Global.GetSafeAttributeLong(safeXElement, "NotLostEquip");
			this.IsolatedMap = (int)Global.GetSafeAttributeLong(safeXElement, "IsolatedMap");
			this.HoldNPC = (int)Global.GetSafeAttributeLong(safeXElement, "HoldNPC");
			this.HoldMonster = (int)Global.GetSafeAttributeLong(safeXElement, "HoldMonster");
			this.HoldRole = (int)Global.GetSafeAttributeLong(safeXElement, "HoldRole");
			this.RealiveMode = (int)Global.GetSafeAttributeLong(safeXElement, "RealiveMode");
			this.RealiveTime = (int)Global.GetSafeAttributeLong(safeXElement, "RealiveTime");
			safeXElement = Global.GetSafeXElement(xelement, "Limits");
			this.DayLimitSecs = (int)Global.GetSafeAttributeLong(safeXElement, "DayLimitSecs");
			this.LimitTimes = Global.ParseDateTimeRangeStr(Global.GetSafeAttributeStr(safeXElement, "Times"));
			this.LimitGoodsIDs = Global.String2IntArray(Global.GetSafeAttributeStr(safeXElement, "GoodsIDs"), ',');
			this.LimitBufferIDs = Global.String2IntArray(Global.GetSafeAttributeStr(safeXElement, "BufferIDs"), ',');
			this.LimitAuotFight = (int)Global.GetSafeAttributeLong(safeXElement, "AutoFight");
			this.LimitMagicIDs = Global.String2IntArray(Global.GetSafeAttributeStr(safeXElement, "MagicIDs"), ',');
			this.MinZhuanSheng = (int)Global.GetSafeAttributeLong(safeXElement, "MinZhuanSheng");
			this.MinLevel = (int)Global.GetSafeAttributeLong(safeXElement, "MinLevel");
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				try
				{
					xelement = Global.GetXElement(Global.XmlInfo["ConfigSettings"], "Map", "Code", this.MapCode.ToString());
					if (null != xelement)
					{
						this.OnlyShowNPC = (Global.GetDefAttributeStr(xelement, "ElsePeople", "0") == "1");
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("{0} MapCode={1}", ex.Message.ToString(), this.MapCode));
				}
			}
		}

		private void LoadAreaLua()
		{
			Trace.Assert(this.MapGridWidth > 0);
			Trace.Assert(this.MapGridHeight > 0);
			string text = string.Format("Map/{0}/AreaLua.xml", this.MapCode);
			XElement xelement = null;
			try
			{
				xelement = Global.GetResXml(text);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", text));
			}
			IEnumerable<XElement> enumerable = xelement.Element("Areas").Elements();
			if (null != enumerable)
			{
				this.AreaLuaList = new List<GAreaLua>();
				foreach (XElement xelement2 in enumerable)
				{
					int id = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					int num = (int)Global.GetSafeAttributeLong(xelement2, "X");
					int num2 = (int)Global.GetSafeAttributeLong(xelement2, "Y");
					int num3 = (int)Global.GetSafeAttributeLong(xelement2, "Radius");
					string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "LuaScriptFile");
					int taskId = 0;
					AddtionType addtionType = AddtionType.NowTrigger;
					Dictionary<AreaEventType, List<int>> dictionary = new Dictionary<AreaEventType, List<int>>();
					if (xelement2.Attribute("Touch") != null)
					{
						string[] array = Global.GetSafeAttributeStr(xelement2, "Touch").Split(new char[]
						{
							','
						}, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length > 1)
						{
							addtionType = (AddtionType)Convert.ToInt32(array[0]);
							taskId = Convert.ToInt32(array[1]);
						}
						string[] array2 = Global.GetSafeAttributeStr(xelement2, "Event").Split(new char[]
						{
							'|'
						}, StringSplitOptions.RemoveEmptyEntries);
						foreach (string text2 in array2)
						{
							List<int> list = new List<int>();
							string[] array4 = text2.Split(new char[]
							{
								','
							}, StringSplitOptions.RemoveEmptyEntries);
							if (array4.Length > 1)
							{
								for (int j = 1; j < array4.Length; j++)
								{
									list.Add(Convert.ToInt32(array4[j]));
								}
								dictionary.Add((AreaEventType)Convert.ToInt32(array4[0]), list);
							}
						}
					}
					GAreaLua item = new GAreaLua
					{
						ID = id,
						CenterPoint = new Point((double)(num / this.MapGridWidth), (double)(num2 / this.MapGridHeight)),
						Radius = Math.Max(num3 / this.MapGridWidth, 1),
						LuaScriptFileName = safeAttributeStr,
						AddtionType = addtionType,
						TaskId = taskId,
						AreaEventDict = dictionary
					};
					this.AreaLuaList.Add(item);
				}
				xelement = null;
			}
		}

		private void LoadObstruction()
		{
			string text = string.Format("MapConfig/{0}/obs.xml", this.MapPicCode);
			XElement xelement = null;
			try
			{
				xelement = Global.GetMapConfigResXml(text);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", text));
			}
			this.MapGridWidth = GameManager.MapGridWidth;
			this.MapGridHeight = GameManager.MapGridHeight;
			int num = (this.MapWidth - 1) / this.MapGridWidth + 1;
			int num2 = (this.MapHeight - 1) / this.MapGridHeight + 1;
			this.MapGridColsNum = num;
			this.MapGridRowsNum = num2;
			num = (int)Math.Ceiling(Math.Log((double)num, 2.0));
			num = (int)Math.Pow(2.0, (double)num);
			num2 = (int)Math.Ceiling(Math.Log((double)num2, 2.0));
			num2 = (int)Math.Pow(2.0, (double)num2);
			if (!GameMap.NodeGridCacheDict.TryGetValue(this.MapPicCode, out this._NodeGrid))
			{
				this._NodeGrid = new NodeGrid(num, num2);
				GameMap.NodeGridCacheDict[this.MapPicCode] = this._NodeGrid;
				string value = xelement.Attribute("Value").Value;
				if (value != "")
				{
					string[] array = value.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Count<string>(); i++)
					{
						if (!(array[i].Trim() == ""))
						{
							string[] array2 = array[i].Split(new char[]
							{
								'_'
							});
							int num3 = Convert.ToInt32(array2[0]) / 2;
							int num4 = Convert.ToInt32(array2[1]) / 2;
							if (num3 < num && num4 < num2)
							{
								this._NodeGrid.setWalkable(num3, num4, false);
							}
						}
					}
				}
			}
		}

		private void LoadAnQuanQuXml()
		{
			string text = string.Format("MapConfig/{0}/anquanqu.xml", this.MapPicCode);
			XElement xelement = null;
			try
			{
				xelement = Global.GetMapConfigResXml(text);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", text));
			}
			if (!GameMap.AnQuanQuCacheDict.TryGetValue(this.MapPicCode, out this.SafeRegionArray))
			{
				this.SafeRegionArray = new byte[this.MapGridColsNum, this.MapGridRowsNum];
				GameMap.AnQuanQuCacheDict[this.MapPicCode] = this.SafeRegionArray;
				string value = xelement.Attribute("Value").Value;
				if (!string.IsNullOrEmpty(value))
				{
					string[] array = value.Split(new char[]
					{
						','
					});
					for (int i = 0; i < array.Count<string>(); i++)
					{
						if (!(array[i].Trim() == ""))
						{
							string[] array2 = array[i].Split(new char[]
							{
								'_'
							});
							int num = Convert.ToInt32(array2[0]) / 2;
							int num2 = Convert.ToInt32(array2[1]) / 2;
							if (num < this.MapGridColsNum && num2 < this.MapGridRowsNum)
							{
								this.SafeRegionArray[num, num2] = 1;
							}
						}
					}
				}
			}
		}

		private void LoadMapTeleportDict()
		{
			string text = string.Format("Map/{0}/teleports.xml", this.MapCode);
			XElement xelement = null;
			try
			{
				xelement = Global.GetResXml(text);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", text));
			}
			IEnumerable<XElement> enumerable = xelement.Element("Teleports").Elements();
			if (null != enumerable)
			{
				foreach (XElement xml in enumerable)
				{
					int num = (int)Global.GetSafeAttributeLong(xml, "Key");
					int toMapID = (int)Global.GetSafeAttributeLong(xml, "To");
					int toX = (int)Global.GetSafeAttributeLong(xml, "ToX");
					int toY = (int)Global.GetSafeAttributeLong(xml, "ToY");
					int x = (int)Global.GetSafeAttributeLong(xml, "X");
					int y = (int)Global.GetSafeAttributeLong(xml, "Y");
					int radius = (int)Global.GetSafeAttributeLong(xml, "Radius");
					MapTeleport value = new MapTeleport
					{
						Code = num,
						MapID = -1,
						X = x,
						Y = y,
						ToX = toX,
						ToY = toY,
						ToMapID = toMapID,
						Radius = radius
					};
					this.MapTeleportDict[num] = value;
				}
				xelement = null;
			}
		}

		private void LoadPathFinderFast()
		{
			this._AStarFinder = new AStar();
		}

		private void InitEnterMapLuaFile()
		{
			string mapLuaScriptFile = Global.GetMapLuaScriptFile(this.MapCode, "enterMap.lua");
			if (File.Exists(mapLuaScriptFile))
			{
				this.EnterMapLuaFile = mapLuaScriptFile;
			}
		}

		public bool CanMove(int gridX, int gridY)
		{
			return gridX * this.MapGridWidth < this.MapWidth && gridX >= 0 && gridY * this.MapGridHeight < this.MapHeight && gridY >= 0 && this.MyNodeGrid.isWalkable(gridX, gridY);
		}

		public bool OnlyShowNPC;

		public byte[,] SafeRegionArray = null;

		private List<GAreaLua> AreaLuaList;

		public Dictionary<int, MapTeleport> MapTeleportDict = new Dictionary<int, MapTeleport>();

		private NodeGrid _NodeGrid;

		private AStar _AStarFinder;

		public string EnterMapLuaFile = null;

		private static Dictionary<int, NodeGrid> NodeGridCacheDict = new Dictionary<int, NodeGrid>();

		private static Dictionary<int, byte[,]> AnQuanQuCacheDict = new Dictionary<int, byte[,]>();
	}
}
