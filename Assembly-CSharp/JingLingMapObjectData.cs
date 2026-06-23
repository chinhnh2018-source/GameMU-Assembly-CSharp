using System;
using System.Collections.Generic;
using UnityEngine;

public class JingLingMapObjectData
{
	public JingLingMapObjectData(int posIndex, int typeIndex, Vector3 pos, JingLingMapObj.EmBuildType buildType)
	{
		this.posIndex = posIndex;
		this.typeIndex = typeIndex;
		this.buildType = buildType;
		this.pos = pos;
	}

	public static List<JingLingMapObjectData> ObjectDataList
	{
		get
		{
			if (JingLingMapObjectData._objectDataList == null)
			{
				JingLingMapObjectData._objectDataList = new List<JingLingMapObjectData>();
				List<Vector3> list = new List<Vector3>();
				list.Add(new Vector3(28.2f, 50.5f, 26.1f));
				list.Add(new Vector3(35.4f, 50.5f, 26f));
				list.Add(new Vector3(24.3f, 50.5f, 17.4f));
				list.Add(new Vector3(21.9f, 50.7f, 22.1f));
				list.Add(new Vector3(28.2f, 50.5f, 18.5f));
				list.Add(new Vector3(20f, 50.6f, 28.3f));
				list.Add(new Vector3(33.4f, 50.5f, 21f));
				for (int i = 0; i < list.Count; i++)
				{
					if (i == 0)
					{
						JingLingMapObjectData._objectDataList.Add(new JingLingMapObjectData(i, i, list[i], JingLingMapObj.EmBuildType.Home));
					}
					else if (i == 1)
					{
						JingLingMapObjectData._objectDataList.Add(new JingLingMapObjectData(i, i, list[i], JingLingMapObj.EmBuildType.Boss));
					}
					else
					{
						JingLingMapObjectData._objectDataList.Add(new JingLingMapObjectData(i, i - 2, list[i], JingLingMapObj.EmBuildType.Task));
					}
				}
			}
			return JingLingMapObjectData._objectDataList;
		}
	}

	public static JingLingMapObjectData homeData
	{
		get
		{
			return JingLingMapObjectData.ObjectDataList[0];
		}
	}

	public static JingLingMapObjectData bossData
	{
		get
		{
			return JingLingMapObjectData.ObjectDataList[1];
		}
	}

	public static JingLingMapObjectData taskData(int taskIndex = 0)
	{
		for (int i = 0; i < JingLingMapObjectData.ObjectDataList.Count; i++)
		{
			JingLingMapObjectData jingLingMapObjectData = JingLingMapObjectData.ObjectDataList[i];
			if (jingLingMapObjectData.buildType == JingLingMapObj.EmBuildType.Task && jingLingMapObjectData.typeIndex == taskIndex)
			{
				return JingLingMapObjectData.ObjectDataList[i];
			}
		}
		return null;
	}

	public static int TaskCout
	{
		get
		{
			int num = 0;
			for (int i = 0; i < JingLingMapObjectData.ObjectDataList.Count; i++)
			{
				JingLingMapObjectData jingLingMapObjectData = JingLingMapObjectData.ObjectDataList[i];
				if (jingLingMapObjectData.buildType == JingLingMapObj.EmBuildType.Task)
				{
					num++;
				}
			}
			return num;
		}
	}

	public Vector3 pos;

	public int posIndex;

	public int typeIndex;

	public JingLingMapObj.EmBuildType buildType;

	private static List<JingLingMapObjectData> _objectDataList;
}
