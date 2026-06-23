using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic
{
	public class MapGrid
	{
		public MapGrid(int mapCode, int mapWidth, int mapHeight, int mapGridWidth, int mapGridHeight, GameMap gameMap)
		{
			this.MapCode = mapCode;
			this.MapWidth = mapWidth;
			this.MapHeight = mapHeight;
			this._MapGridWidth = mapGridWidth;
			this._MapGridHeight = mapGridHeight;
			this._MapGridXNum = (this.MapWidth - 1) / this._MapGridWidth + 1;
			this._MapGridYNum = (this.MapHeight - 1) / this._MapGridHeight + 1;
			this._MapGridTotalNum = this._MapGridXNum * this._MapGridYNum;
			this._GameMap = gameMap;
			this.MyMapGridSpriteItem = new MapGridSpriteItem[this._MapGridTotalNum];
			for (int i = 0; i < this.MyMapGridSpriteItem.Length; i++)
			{
				this.MyMapGridSpriteItem[i].GridLock = new object();
				this.MyMapGridSpriteItem[i].ObjsList = new List<object>();
			}
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

		private int GetGridIndex(int gridX, int gridY)
		{
			return this._MapGridXNum * gridY + gridX;
		}

		private void ChangeMapGridsSpriteNum(int index, IObject obj, short addOrSubNum)
		{
			this.MyMapGridSpriteItem[index].ObjsListReadOnly = null;
			switch (obj.ObjectType)
			{
			case ObjectTypes.OT_CLIENT:
			{
				MapGridSpriteItem[] myMapGridSpriteItem = this.MyMapGridSpriteItem;
				myMapGridSpriteItem[index].RoleNum = myMapGridSpriteItem[index].RoleNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].RoleNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].RoleNum);
				break;
			}
			case ObjectTypes.OT_MONSTER:
			{
				MapGridSpriteItem[] myMapGridSpriteItem2 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem2[index].MonsterNum = myMapGridSpriteItem2[index].MonsterNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].MonsterNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].MonsterNum);
				break;
			}
			case ObjectTypes.OT_GOODSPACK:
			{
				MapGridSpriteItem[] myMapGridSpriteItem3 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem3[index].GoodsPackNum = myMapGridSpriteItem3[index].GoodsPackNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].GoodsPackNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].GoodsPackNum);
				break;
			}
			case ObjectTypes.OT_BIAOCHE:
			{
				MapGridSpriteItem[] myMapGridSpriteItem4 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem4[index].BiaoCheNum = myMapGridSpriteItem4[index].BiaoCheNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].BiaoCheNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].BiaoCheNum);
				break;
			}
			case ObjectTypes.OT_JUNQI:
			{
				MapGridSpriteItem[] myMapGridSpriteItem5 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem5[index].JunQiNum = myMapGridSpriteItem5[index].JunQiNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].JunQiNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].JunQiNum);
				break;
			}
			case ObjectTypes.OT_NPC:
			{
				MapGridSpriteItem[] myMapGridSpriteItem6 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem6[index].NPCNum = myMapGridSpriteItem6[index].NPCNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].NPCNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].NPCNum);
				break;
			}
			}
		}

		public int GetRoleNum(int gridX, int gridY)
		{
			int result = 0;
			int gridIndex = this.GetGridIndex(gridX, gridY);
			lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
			{
				result = (int)this.MyMapGridSpriteItem[gridIndex].RoleNum;
			}
			return result;
		}

		public void GetObjectsNum(int gridX, int gridY, out int totalNum, out int roleNum, out int monsterNum, out int nPCNum, out int biaoCheNum, out int junQiNum, out int goodsPackNum, out int decoNum)
		{
			int gridIndex = this.GetGridIndex(gridX, gridY);
			lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
			{
				totalNum = this.MyMapGridSpriteItem[gridIndex].ObjsList.Count;
				roleNum = (int)this.MyMapGridSpriteItem[gridIndex].RoleNum;
				monsterNum = (int)this.MyMapGridSpriteItem[gridIndex].MonsterNum;
				nPCNum = (int)this.MyMapGridSpriteItem[gridIndex].NPCNum;
				biaoCheNum = (int)this.MyMapGridSpriteItem[gridIndex].BiaoCheNum;
				junQiNum = (int)this.MyMapGridSpriteItem[gridIndex].JunQiNum;
				goodsPackNum = (int)this.MyMapGridSpriteItem[gridIndex].GoodsPackNum;
				decoNum = (int)this.MyMapGridSpriteItem[gridIndex].DecoNum;
			}
		}

		public bool MoveObjectEx(int oldGridX, int oldGridY, int newGridX, int newGridY, IObject obj)
		{
			int oldX = oldGridX * this._MapGridWidth;
			int oldY = oldGridY * this._MapGridHeight;
			int newX = newGridX * this._MapGridWidth;
			int newY = newGridY * this._MapGridHeight;
			return this.MoveObject(oldX, oldY, newX, newY, obj);
		}

		public bool MoveObject(int oldX, int oldY, int newX, int newY, IObject obj)
		{
			bool result;
			if (newX < 0 || newY < 0 || newX >= this.MapWidth || newY >= this.MapHeight)
			{
				LogManager.WriteLog(2, string.Format("坐标超出地图大小: MapCode={0}, newX={1}, newY={2}, Width={3}, Height={4}", new object[]
				{
					this.MapCode,
					newX,
					newY,
					this.MapWidth,
					this.MapHeight
				}), null, true);
				result = false;
			}
			else
			{
				int gridX = newX / this._MapGridWidth;
				int gridY = newY / this._MapGridHeight;
				int num = -1;
				lock (this.ModifyMutex)
				{
					this.HandleTracking(oldX, oldY, newX, newY, obj);
					lock (this._Obj2GridDict)
					{
						if (!this._Obj2GridDict.TryGetValue(obj, out num))
						{
							num = -1;
						}
					}
					int gridIndex = this.GetGridIndex(gridX, gridY);
					if (-1 != num && num == gridIndex)
					{
						return true;
					}
					if (-1 != num)
					{
						lock (this.MyMapGridSpriteItem[num].GridLock)
						{
							if (!this.MyMapGridSpriteItem[num].ObjsList.Remove(obj))
							{
								return false;
							}
							this.ChangeMapGridsSpriteNum(num, obj, -1);
						}
					}
					lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
					{
						this.MyMapGridSpriteItem[gridIndex].ObjsList.Add(obj);
						this.ChangeMapGridsSpriteNum(gridIndex, obj, 1);
					}
					lock (this._Obj2GridDict)
					{
						this._Obj2GridDict[obj] = gridIndex;
					}
				}
				result = true;
			}
			return result;
		}

		private void HandleTracking(int oldX, int oldY, int newX, int newY, IObject obj)
		{
			if (obj is GameClient)
			{
				lock (obj)
				{
					if ((obj as GameClient).ClientData.TrackingRoleIDList.Count != 0)
					{
						foreach (int roleID in (obj as GameClient).ClientData.TrackingRoleIDList)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
							if (gameClient != null && gameClient.ClientData.HideGM != 0)
							{
								lock (this._Obj2GridDict)
								{
									int num = -1;
									if (!this._Obj2GridDict.TryGetValue(gameClient, out num))
									{
										continue;
									}
								}
								gameClient.ClientData.PosX = (obj as GameClient).ClientData.PosX;
								gameClient.ClientData.PosY = (obj as GameClient).ClientData.PosY;
								this.MoveObject(oldX, oldY, newX, newY, gameClient);
							}
						}
					}
				}
			}
		}

		public bool RemoveObject(IObject obj)
		{
			lock (this.ModifyMutex)
			{
				int num = -1;
				lock (this._Obj2GridDict)
				{
					if (!this._Obj2GridDict.TryGetValue(obj, out num))
					{
						num = -1;
					}
					else
					{
						this._Obj2GridDict.Remove(obj);
					}
				}
				if (-1 == num)
				{
					return false;
				}
				lock (this.MyMapGridSpriteItem[num].GridLock)
				{
					if (this.MyMapGridSpriteItem[num].ObjsList.Remove(obj))
					{
						this.ChangeMapGridsSpriteNum(num, obj, -1);
					}
				}
			}
			return true;
		}

		public int GetGridClientCountForConsole()
		{
			int result;
			lock (this._Obj2GridDict)
			{
				result = this._Obj2GridDict.Count((KeyValuePair<object, int> x) => x.Key is GameClient);
			}
			return result;
		}

		public List<object> FindObjects(int gridX, int gridY)
		{
			int num = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (num < 0 || num >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> list = null;
				lock (this.MyMapGridSpriteItem[num].GridLock)
				{
					list = this.MyMapGridSpriteItem[num].ObjsList;
					if (list.Count == 0)
					{
						return null;
					}
					if (this.FlagOptimizeFindObjects)
					{
						if (null == this.MyMapGridSpriteItem[num].ObjsListReadOnly)
						{
							this.MyMapGridSpriteItem[num].ObjsListReadOnly = list.GetRange(0, list.Count);
						}
						return this.MyMapGridSpriteItem[num].ObjsListReadOnly;
					}
					list = list.GetRange(0, list.Count);
				}
				result = list;
			}
			return result;
		}

		public List<object> FindGoodsPackItems(int gridX, int gridY)
		{
			int num = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (num < 0 || num >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> list = null;
				lock (this.MyMapGridSpriteItem[num].GridLock)
				{
					if (this.MyMapGridSpriteItem[num].GoodsPackNum > 0)
					{
						list = this.MyMapGridSpriteItem[num].ObjsList.GetRange(0, this.MyMapGridSpriteItem[num].ObjsList.Count);
					}
				}
				result = list;
			}
			return result;
		}

		public List<object> FindGameClient(int gridX, int gridY)
		{
			int num = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (num < 0 || num >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> list = null;
				lock (this.MyMapGridSpriteItem[num].GridLock)
				{
					if (this.MyMapGridSpriteItem[num].RoleNum > 0)
					{
						list = this.MyMapGridSpriteItem[num].ObjsList.GetRange(0, this.MyMapGridSpriteItem[num].ObjsList.Count);
					}
				}
				result = list;
			}
			return result;
		}

		public List<object> FindObjects(int toX, int toY, int radius)
		{
			List<object> result;
			if (toX < 0 || toY < 0 || toX >= this.MapWidth || toY >= this.MapHeight)
			{
				result = null;
			}
			else
			{
				int num = toX / this._MapGridWidth;
				int num2 = toY / this._MapGridHeight;
				List<object> list = new List<object>();
				int num3 = (radius - 1) / this._MapGridWidth + 1;
				int num4 = (radius - 1) / this._MapGridHeight + 1;
				int num5 = num2 - num4;
				int num6 = num2 + num4;
				int num7 = num - num3;
				int num8 = num + num3;
				for (int i = num5; i <= num6; i++)
				{
					for (int j = num7; j <= num8; j++)
					{
						List<object> list2 = this.FindObjects(j, i);
						if (null != list2)
						{
							list.AddRange(list2);
						}
					}
				}
				result = list;
			}
			return result;
		}

		public bool CanMove(ObjectTypes objType, int gridX, int gridY, int holdGridNum, byte holdBitSet = 0)
		{
			bool result;
			if (objType == ObjectTypes.OT_BIAOCHE)
			{
				result = true;
			}
			else if (objType == ObjectTypes.OT_FAKEROLE)
			{
				result = true;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				this.GetObjectsNum(gridX, gridY, out num, out num2, out num3, out num4, out num5, out num6, out num7, out num8);
				if (num <= 0)
				{
					result = true;
				}
				else if (objType == ObjectTypes.OT_CLIENT)
				{
					bool flag = true;
					if (this._GameMap.HoldRole > 0 || 1 == (holdBitSet & 1))
					{
						if (num2 > holdGridNum)
						{
							flag = false;
						}
					}
					if (this._GameMap.HoldMonster > 0 || 2 == (holdBitSet & 2))
					{
						if (num3 > holdGridNum)
						{
							flag = false;
						}
					}
					if (this._GameMap.HoldNPC > 0 || 4 == (holdBitSet & 4))
					{
						if (num4 > holdGridNum || num6 > holdGridNum)
						{
							flag = false;
						}
					}
					result = flag;
				}
				else if (objType == ObjectTypes.OT_MONSTER)
				{
					bool flag = true;
					if (num2 > holdGridNum)
					{
						flag = false;
					}
					if (num3 > holdGridNum)
					{
						flag = false;
					}
					if (num4 > holdGridNum || num6 > holdGridNum)
					{
						flag = false;
					}
					result = flag;
				}
				else if (objType == ObjectTypes.OT_GOODSPACK)
				{
					bool flag = true;
					if (num7 > holdGridNum)
					{
						flag = false;
					}
					result = flag;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private object ModifyMutex = new object();

		public GameMap _GameMap = null;

		public bool FlagOptimizeFindObjects;

		private int MapCode;

		private int MapWidth;

		private int MapHeight;

		private int _MapGridWidth;

		private int _MapGridHeight;

		private int _MapGridXNum = 0;

		private int _MapGridYNum = 0;

		private int _MapGridTotalNum = 0;

		private Dictionary<object, int> _Obj2GridDict = new Dictionary<object, int>(1000);

		private MapGridSpriteItem[] MyMapGridSpriteItem = null;
	}
}
