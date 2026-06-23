using System;
using System.Collections.Generic;
using UnityEngine;

public static class JingLingPos
{
	public static Vector3 taskpos(int missonType, int index)
	{
		if (missonType == 1)
		{
			return JingLingPos.data(index)[0];
		}
		if (missonType == 2)
		{
			return JingLingPos.data(index)[1];
		}
		if (missonType == 3)
		{
			return JingLingPos.data(index)[2];
		}
		if (missonType == 4)
		{
			return JingLingPos.data(index)[3];
		}
		if (missonType == 5)
		{
			return JingLingPos.data(index)[4];
		}
		return Vector3.zero;
	}

	public static List<Vector3> data(int index)
	{
		if (index == 0)
		{
			List<Vector3> list = new List<Vector3>();
			list.Add(new Vector3(24.3f, 50.5f, 17.4f));
			list.Add(new Vector3(21.9f, 50.79f, 22.1f));
			list.Add(new Vector3(28.2f, 50.5f, 18.5f));
			list.Add(new Vector3(20f, 50.6f, 28.3f));
			list.Add(new Vector3(33.4f, 50.5f, 21f));
			return list;
		}
		if (index == 1)
		{
			List<Vector3> list2 = new List<Vector3>();
			list2.Add(new Vector3(0f, 39f, 0f));
			list2.Add(new Vector3(0f, 252f, 0f));
			list2.Add(new Vector3(0f, 349f, 0f));
			list2.Add(new Vector3(0f, 315f, 0f));
			list2.Add(new Vector3(0f, 15f, 0f));
			return list2;
		}
		if (index == 2)
		{
			List<Vector3> list3 = new List<Vector3>();
			list3.Add(new Vector3(0.8f, 0.8f, 0.8f));
			list3.Add(new Vector3(0.8f, 0.8f, 0.8f));
			list3.Add(new Vector3(0.8f, 0.8f, 0.8f));
			list3.Add(new Vector3(1f, 1f, 1f));
			list3.Add(new Vector3(0.8f, 0.8f, 0.8f));
			return list3;
		}
		return null;
	}
}
