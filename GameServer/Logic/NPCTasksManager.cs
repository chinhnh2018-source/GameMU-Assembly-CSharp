using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class NPCTasksManager
	{
		public Dictionary<int, List<int>> SourceNPCTasksDict
		{
			get
			{
				return this._SourceNPCTasksDict;
			}
		}

		private void AddSourceNPCTask(int npcID, int taskID, Dictionary<int, List<int>> sourceNPCTasksDict)
		{
			List<int> list = null;
			if (!sourceNPCTasksDict.TryGetValue(npcID, out list))
			{
				list = new List<int>();
				sourceNPCTasksDict[npcID] = list;
			}
			if (-1 == list.IndexOf(taskID))
			{
				list.Add(taskID);
			}
		}

		public Dictionary<int, List<int>> DestNPCTasksDict
		{
			get
			{
				return this._DestNPCTasksDict;
			}
		}

		private void AddDestNPCTask(int npcID, int taskID, Dictionary<int, List<int>> destNPCTasksDict)
		{
			List<int> list = null;
			if (!destNPCTasksDict.TryGetValue(npcID, out list))
			{
				list = new List<int>();
				destNPCTasksDict[npcID] = list;
			}
			if (-1 == list.IndexOf(taskID))
			{
				list.Add(taskID);
			}
		}

		public void LoadNPCTasks(SystemXmlItems systemTasks)
		{
			Dictionary<int, List<int>> sourceNPCTasksDict = new Dictionary<int, List<int>>();
			Dictionary<int, List<int>> destNPCTasksDict = new Dictionary<int, List<int>>();
			foreach (int key in systemTasks.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemTasks.SystemXmlItemDict[key];
				this.AddSourceNPCTask(systemXmlItem.GetIntValue("SourceNPC", -1), systemXmlItem.GetIntValue("ID", -1), sourceNPCTasksDict);
				this.AddDestNPCTask(systemXmlItem.GetIntValue("DestNPC", -1), systemXmlItem.GetIntValue("ID", -1), destNPCTasksDict);
			}
			this._SourceNPCTasksDict = sourceNPCTasksDict;
			this._DestNPCTasksDict = destNPCTasksDict;
		}

		private Dictionary<int, List<int>> _SourceNPCTasksDict = null;

		private Dictionary<int, List<int>> _DestNPCTasksDict = null;
	}
}
