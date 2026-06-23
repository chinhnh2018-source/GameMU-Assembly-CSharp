using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSGameEngine.GameEngine.U3D
{
	public class SkeletonBonesCachingDict
	{
		public static Dictionary<string, int> CalcBonesDict(string skeletonName, GameObject skeletonObj)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (SkeletonBonesCachingDict.SkeletonBonesDict.TryGetValue(skeletonName, ref dictionary))
			{
				return dictionary;
			}
			dictionary = new Dictionary<string, int>();
			Transform[] componentsInChildren = skeletonObj.GetComponentsInChildren<Transform>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Transform transform = componentsInChildren[i];
				dictionary[transform.name] = i;
			}
			SkeletonBonesCachingDict.SkeletonBonesDict[skeletonName] = dictionary;
			return dictionary;
		}

		private static Dictionary<string, Dictionary<string, int>> SkeletonBonesDict = new Dictionary<string, Dictionary<string, int>>();
	}
}
