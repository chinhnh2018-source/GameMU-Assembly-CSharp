using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.GoodsPack;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

namespace HSGameEngine.GameEngine.Data
{
	public class MapGrid
	{
		public MapGrid(int mapCode, int mapWidth, int mapHeight, int mapGridWidth, int mapGridHeight, GMapData mapData)
		{
			this.MapCode = mapCode;
			this.MapWidth = mapWidth;
			this.MapHeight = mapHeight;
			this._MapGridWidth = mapGridWidth;
			this._MapGridHeight = mapGridHeight;
			if (this._MapGridWidth != 0 && this._MapGridHeight != 0)
			{
				this._MapGridXNum = (this.MapWidth - 1) / this._MapGridWidth + 1;
				this._MapGridYNum = (this.MapHeight - 1) / this._MapGridHeight + 1;
			}
			this._MapData = mapData;
		}

		public int MapGridWidth
		{
			get
			{
				return this._MapGridWidth;
			}
		}

		public int MapGridHeight
		{
			get
			{
				return this._MapGridHeight;
			}
		}

		public int MapGridXNum
		{
			get
			{
				return this._MapGridXNum;
			}
		}

		public int MapGridYNum
		{
			get
			{
				return this._MapGridYNum;
			}
		}

		private void ChangeMapGridsSpriteNum(string key, object obj, int addOrSubNum)
		{
			MapGridSpriteNum mapGridSpriteNum;
			if (!this._MapGridsSpriteNum.ContainsKey(key))
			{
				mapGridSpriteNum = new MapGridSpriteNum();
				this._MapGridsSpriteNum[key] = mapGridSpriteNum;
			}
			else
			{
				mapGridSpriteNum = this._MapGridsSpriteNum[key];
			}
			mapGridSpriteNum.TotalNum += addOrSubNum;
			mapGridSpriteNum.TotalNum = Global.GMax(0, mapGridSpriteNum.TotalNum);
			if (obj is GSprite)
			{
				if ((obj as GSprite).SpriteType == GSpriteTypes.Other || (obj as GSprite).SpriteType == GSpriteTypes.Leader)
				{
					mapGridSpriteNum.RoleNum += addOrSubNum;
					mapGridSpriteNum.RoleNum = Global.GMax(0, mapGridSpriteNum.RoleNum);
				}
				else if ((obj as GSprite).SpriteType == GSpriteTypes.Monster)
				{
					mapGridSpriteNum.MonsterNum += addOrSubNum;
					mapGridSpriteNum.MonsterNum = Global.GMax(0, mapGridSpriteNum.MonsterNum);
				}
				else if ((obj as GSprite).SpriteType == GSpriteTypes.NPC)
				{
					mapGridSpriteNum.NPCNum += addOrSubNum;
					mapGridSpriteNum.NPCNum = Global.GMax(0, mapGridSpriteNum.NPCNum);
				}
				else if ((obj as GSprite).SpriteType == GSpriteTypes.BiaoChe)
				{
					mapGridSpriteNum.BiaoCheNum += addOrSubNum;
					mapGridSpriteNum.BiaoCheNum = Global.GMax(0, mapGridSpriteNum.BiaoCheNum);
				}
				else if ((obj as GSprite).SpriteType == GSpriteTypes.JunQi)
				{
					mapGridSpriteNum.JunQiNum += addOrSubNum;
					mapGridSpriteNum.JunQiNum = Global.GMax(0, mapGridSpriteNum.JunQiNum);
				}
			}
			else if (obj is GGoodsPack)
			{
				mapGridSpriteNum.GoodsPackNum += addOrSubNum;
				mapGridSpriteNum.GoodsPackNum = Global.GMax(0, mapGridSpriteNum.GoodsPackNum);
			}
		}

		public MapGridSpriteNum GetMapGridSpriteNum(int gridX, int gridY)
		{
			string text = gridX.ToString() + "_" + gridY.ToString();
			MapGridSpriteNum result = null;
			this._MapGridsSpriteNum.TryGetValue(text, ref result);
			return result;
		}

		public int GetRoleNum(int gridX, int gridY)
		{
			MapGridSpriteNum mapGridSpriteNum = this.GetMapGridSpriteNum(gridX, gridY);
			if (mapGridSpriteNum == null)
			{
				return 0;
			}
			return mapGridSpriteNum.RoleNum;
		}

		public bool MoveObjectEx(int oldGridX, int oldGridY, int newGridX, int newGridY, object obj)
		{
			int oldX = oldGridX * this._MapGridWidth;
			int oldY = oldGridY * this._MapGridHeight;
			int newX = newGridX * this._MapGridWidth;
			int newY = newGridY * this._MapGridHeight;
			SystemHelpMgr.OnGridChanged(newGridX, newGridY);
			return this.MoveObject(oldX, oldY, newX, newY, obj);
		}

		public bool MoveObject(int oldX, int oldY, int newX, int newY, object obj)
		{
			if (newX < 0 || newY < 0 || newX >= this.MapWidth || newY >= this.MapHeight)
			{
				return false;
			}
			if (this._MapGridWidth != 0 && this._MapGridHeight != 0)
			{
				newX /= this._MapGridWidth;
				newY /= this._MapGridHeight;
			}
			string text = null;
			this._Obj2GridDict.TryGetValue(obj, ref text);
			string text2 = StringUtil.substitute("{0}_{1}", new object[]
			{
				newX,
				newY
			});
			List<object> list = null;
			if (text != null)
			{
				this._MapGrids.TryGetValue(text, ref list);
				if (list != null)
				{
					int num = list.IndexOf(obj);
					list.RemoveAt(num);
					this.ChangeMapGridsSpriteNum(text, obj, -1);
				}
			}
			this._MapGrids.TryGetValue(text2, ref list);
			if (list != null)
			{
				list.Add(obj);
				this.ChangeMapGridsSpriteNum(text2, obj, 1);
			}
			else
			{
				list = new List<object>();
				list.Add(obj);
				this._MapGrids[text2] = list;
				this.ChangeMapGridsSpriteNum(text2, obj, 1);
			}
			this._Obj2GridDict[obj] = text2;
			return true;
		}

		public void RemoveObject(object obj)
		{
			string text = null;
			if (!this._Obj2GridDict.TryGetValue(obj, ref text))
			{
				return;
			}
			this._Obj2GridDict.Remove(obj);
			if (text == null)
			{
				return;
			}
			List<object> list = null;
			if (!this._MapGrids.TryGetValue(text, ref list))
			{
				return;
			}
			if (list != null)
			{
				int num = list.IndexOf(obj);
				list.RemoveAt(num);
				this.ChangeMapGridsSpriteNum(text, obj, -1);
			}
		}

		public List<object> FindObjects(int gridX, int gridY)
		{
			if (gridX < 0 || gridY < 0 || gridX >= this._MapGridXNum || gridY >= this._MapGridYNum)
			{
				return null;
			}
			string text = StringUtil.substitute("{0}_{1}", new object[]
			{
				gridX,
				gridY
			});
			List<object> list = null;
			this._MapGrids.TryGetValue(text, ref list);
			List<object> list2 = null;
			if (list != null)
			{
				list2 = new List<object>();
				list2.AddRange(list);
			}
			return list2;
		}

		public List<object> FindObjects(int toX, int toY, int radius)
		{
			if (toX < 0 || toY < 0 || toX >= this.MapWidth || toY >= this.MapHeight)
			{
				return null;
			}
			if (this._MapGridWidth == 0 || this._MapGridHeight == 0)
			{
				return null;
			}
			toX /= this._MapGridWidth;
			toY /= this._MapGridHeight;
			List<object> list = new List<object>();
			int num = radius / this._MapGridWidth / 2;
			num = Global.GMax(1, num);
			int num2 = radius / this._MapGridHeight / 2;
			num2 = Global.GMax(1, num2);
			for (int i = toY - num2; i <= toY + num2; i++)
			{
				for (int j = toX - num; j <= toX + num; j++)
				{
					List<object> list2 = this.FindObjects(j, i);
					if (list2 != null)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							list.Add(list2[k]);
						}
					}
				}
			}
			return list;
		}

		public bool CanMove(int objType, int gridX, int gridY, int holdGridNum, byte holdBitSet = 0)
		{
			if (objType == 5)
			{
				return true;
			}
			MapGridSpriteNum mapGridSpriteNum = this.GetMapGridSpriteNum(gridX, gridY);
			if (mapGridSpriteNum == null)
			{
				return true;
			}
			if (objType == 0 || objType == 1)
			{
				bool result = true;
				if (this._MapData.HoldRole > 0 && (mapGridSpriteNum.RoleNum > holdGridNum || (holdBitSet & 1) == 1))
				{
					result = false;
				}
				if (this._MapData.HoldMonster > 0 && (mapGridSpriteNum.MonsterNum > holdGridNum || (holdBitSet & 2) == 2))
				{
					result = false;
				}
				if (this._MapData.HoldNPC > 0 && (mapGridSpriteNum.NPCNum > holdGridNum || mapGridSpriteNum.JunQiNum > holdGridNum || (holdBitSet & 4) == 4))
				{
					result = false;
				}
				return result;
			}
			if (objType == 1)
			{
				bool result2 = true;
				if (mapGridSpriteNum.RoleNum > holdGridNum)
				{
					result2 = false;
				}
				if (mapGridSpriteNum.MonsterNum > holdGridNum)
				{
					result2 = false;
				}
				if (mapGridSpriteNum.NPCNum > holdGridNum || mapGridSpriteNum.JunQiNum > holdGridNum)
				{
					result2 = false;
				}
				return result2;
			}
			if (objType == 2)
			{
				bool result3 = true;
				if (mapGridSpriteNum.GoodsPackNum > holdGridNum)
				{
					result3 = false;
				}
				return result3;
			}
			return false;
		}

		private GMapData _MapData;

		private int MapCode;

		private int MapWidth;

		private int MapHeight;

		private int _MapGridWidth;

		private int _MapGridHeight;

		private int _MapGridXNum;

		private int _MapGridYNum;

		private Dictionary<object, string> _Obj2GridDict = new Dictionary<object, string>(1000);

		private Dictionary<string, List<object>> _MapGrids = new Dictionary<string, List<object>>(1000);

		private Dictionary<string, MapGridSpriteNum> _MapGridsSpriteNum = new Dictionary<string, MapGridSpriteNum>(1000);
	}
}
