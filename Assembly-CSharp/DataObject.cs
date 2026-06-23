using System;
using System.Collections.Generic;
using UnityEngine;

public class DataObject : MonoBehaviour
{
	public static DataObject Instance
	{
		get
		{
			return DataObject._Instance;
		}
	}

	private void Awake()
	{
		DataObject._Instance = this;
	}

	public void SetMonsterSpeed(int id, float speed)
	{
		if (this.MonsterSpeedDict.ContainsKey(id))
		{
			for (int i = 0; i < this.MonsterIDs.Count; i++)
			{
				if (id == this.MonsterIDs[i])
				{
					this.MonsterSpeeds[i] = speed;
				}
			}
		}
		else
		{
			this.MonsterSpeedDict[id] = speed;
			this.MonsterIDs.Add(id);
			this.MonsterSpeeds.Add(speed);
		}
	}

	public float GetMonsterSpeed(int id)
	{
		float result = 0.3f;
		if (!this.MonsterSpeedDict.TryGetValue(id, ref result))
		{
			return 0.3f;
		}
		return result;
	}

	public void Clear()
	{
		this.MonsterSpeedDict.Clear();
		this.MonsterIDs.Clear();
		this.MonsterSpeeds.Clear();
	}

	private static DataObject _Instance;

	private Dictionary<int, float> MonsterSpeedDict = new Dictionary<int, float>();

	public List<int> MonsterIDs = new List<int>();

	public List<float> MonsterSpeeds = new List<float>();
}
