using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Logic
{
	public class MapGridManager
	{
		public Dictionary<int, MapGrid> DictGrids
		{
			get
			{
				return this._DictGrids;
			}
		}

		public void InitAddMapGrid(int mapCode, int mapWidth, int mapHeight, int gridWidth, int gridHeight, GameMap gameMap)
		{
			MapGrid value = new MapGrid(mapCode, mapWidth, mapHeight, gridWidth, gridHeight, gameMap);
			lock (this._DictGrids)
			{
				this._DictGrids.Add(mapCode, value);
			}
		}

		public MapGrid GetMapGrid(int mapCode)
		{
			MapGrid result;
			lock (this._DictGrids)
			{
				MapGrid mapGrid;
				if (this._DictGrids.TryGetValue(mapCode, out mapGrid))
				{
					result = mapGrid;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public string GetAllMapClientCountForConsole()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<int, MapGrid> keyValuePair in this._DictGrids)
			{
				if (null != keyValuePair.Value)
				{
					int gridClientCountForConsole = keyValuePair.Value.GetGridClientCountForConsole();
					if (gridClientCountForConsole > 0)
					{
						stringBuilder.AppendFormat("{0}:{1}\n", keyValuePair.Key, gridClientCountForConsole);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private Dictionary<int, MapGrid> _DictGrids = new Dictionary<int, MapGrid>(100);
	}
}
