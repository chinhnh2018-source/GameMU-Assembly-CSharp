using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RoleSkeletonManager
{
	public static void PreLoadSkeleton()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		for (int i = 0; i < RoleSkeletonManager.skeletonName.Length; i++)
		{
			List<GameObject> list = null;
			if (RoleSkeletonManager.SkeletonDic.TryGetValue(RoleSkeletonManager.skeletonName[i], ref list))
			{
			}
			if (list == null || list.Count == 0)
			{
				if (list == null)
				{
					list = new List<GameObject>();
				}
				string text = new StringBuilder("Prefabs/Skeleton/").Append(RoleSkeletonManager.skeletonName[i]).ToString();
				GameObject gameObject = Resources.Load(text) as GameObject;
				list.Add(gameObject);
				RoleSkeletonManager.SkeletonDic[RoleSkeletonManager.skeletonName[i]] = list;
				GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
				gameObject2.AddComponent<SkinnedMeshRenderer>();
				ManagedSkeleton managedSkeleton = gameObject2.AddComponent<ManagedSkeleton>();
				Object.DontDestroyOnLoad(gameObject2);
				managedSkeleton.ID = RoleSkeletonManager.skeletonName[i];
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"Time:" + (Time.realtimeSinceStartup - realtimeSinceStartup)
		});
	}

	public static GameObject LoadSkeletonByName(string name)
	{
		List<GameObject> list = null;
		if (RoleSkeletonManager.SkeletonDic.TryGetValue(name, ref list))
		{
			if (list.Count > 1)
			{
				GameObject gameObject = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (null != component)
				{
					component.enabled = true;
				}
				return gameObject;
			}
			if (list.Count == 1)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(list[0]);
				gameObject2.AddComponent<SkinnedMeshRenderer>();
				ManagedSkeleton managedSkeleton = gameObject2.AddComponent<ManagedSkeleton>();
				managedSkeleton.ID = name;
				return gameObject2;
			}
		}
		if (list == null || list.Count == 0)
		{
			if (list == null)
			{
				list = new List<GameObject>();
			}
			string text = new StringBuilder("Prefabs/Skeleton/").Append(name).ToString();
			MUDebug.LogError<string>(new string[]
			{
				"path:" + text
			});
			GameObject gameObject3 = Resources.Load(text) as GameObject;
			list.Add(gameObject3);
			RoleSkeletonManager.SkeletonDic[name] = list;
			GameObject gameObject4 = Object.Instantiate<GameObject>(gameObject3);
			gameObject4.AddComponent<SkinnedMeshRenderer>();
			ManagedSkeleton managedSkeleton2 = gameObject4.AddComponent<ManagedSkeleton>();
			Object.DontDestroyOnLoad(gameObject4);
			managedSkeleton2.ID = name;
			return gameObject4;
		}
		return null;
	}

	public static void ManualReleaseSkeleton(string name, GameObject go)
	{
		List<GameObject> list = null;
		RoleSkeletonManager.SkeletonDic.TryGetValue(name, ref list);
		if (list == null || list.Count == 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"骨骼缓存出错 列表被意外释放了"
			});
		}
		else if (!list.Contains(go))
		{
			SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				component.sharedMesh = null;
				component.sharedMaterial = null;
				component.bones = null;
				component.enabled = false;
			}
			list.Add(go);
		}
	}

	public static void ManualDelete(string name, GameObject go)
	{
		List<GameObject> list = null;
		RoleSkeletonManager.SkeletonDic.TryGetValue(name, ref list);
		if (list == null || list.Count == 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"骨骼缓存出错 列表被意外释放了"
			});
		}
		else
		{
			list.Remove(go);
		}
	}

	private static Dictionary<string, List<GameObject>> SkeletonDic = new Dictionary<string, List<GameObject>>();

	private static string[] skeletonName = new string[]
	{
		"ZS_Skeleton"
	};
}
