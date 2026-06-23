using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSGameEngine.GameEngine.U3D
{
	public static class MeshHelper
	{
		public static Mesh CreateMesh(Mesh oldMesh, int subIndex)
		{
			int[] indices = oldMesh.GetIndices(subIndex);
			Dictionary<int, int> dictionary = new Dictionary<int, int>(indices.Length / 3);
			Dictionary<int, int> dictionary2 = new Dictionary<int, int>(indices.Length / 3);
			int num = 0;
			for (int i = 0; i < indices.Length; i++)
			{
				if (!dictionary.ContainsKey(indices[i]))
				{
					dictionary.Add(indices[i], num);
					dictionary2.Add(num, indices[i]);
					num++;
				}
			}
			int[] array = new int[indices.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = dictionary[indices[j]];
			}
			Vector3[] vertices = oldMesh.vertices;
			Vector3[] normals = oldMesh.normals;
			Vector2[] uv = oldMesh.uv;
			BoneWeight[] boneWeights = oldMesh.boneWeights;
			Vector3[] array2 = new Vector3[num];
			Vector3[] array3 = new Vector3[num];
			Vector2[] array4 = new Vector2[num];
			BoneWeight[] array5 = new BoneWeight[num];
			for (int k = 0; k < num; k++)
			{
				int num2 = dictionary2[k];
				array2[k] = vertices[num2];
				array3[k] = normals[num2];
				array4[k] = uv[num2];
				array5[k] = boneWeights[num2];
				if (subIndex > 0)
				{
					BoneWeight[] array6 = array5;
					int num3 = k;
					array6[num3].boneIndex0 = array6[num3].boneIndex0 - oldMesh.bindposes.Length;
					BoneWeight[] array7 = array5;
					int num4 = k;
					array7[num4].boneIndex1 = array7[num4].boneIndex1 - oldMesh.bindposes.Length;
				}
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array2;
			mesh.uv = array4;
			mesh.triangles = array;
			mesh.normals = array3;
			mesh.boneWeights = array5;
			if (subIndex == 0)
			{
				mesh.bindposes = oldMesh.bindposes;
			}
			return mesh;
		}

		public static Vector3[] CopyVector3(Vector3[] newVectices, int length)
		{
			Vector3[] array = new Vector3[length];
			Array.Copy(newVectices, array, length);
			return array;
		}

		public static Vector2[] CopyVector2(Vector2[] newVectices, int length)
		{
			Vector2[] array = new Vector2[length];
			Array.Copy(newVectices, array, length);
			return array;
		}

		public static Mesh CreateMesh_(Mesh oldMesh, int subIndex)
		{
			Mesh mesh = new Mesh();
			int[] triangles = oldMesh.GetTriangles(subIndex);
			Dictionary<int, int> dictionary = new Dictionary<int, int>(oldMesh.vertices.Length);
			int num = 0;
			Vector3[] vertices = oldMesh.vertices;
			Vector3[] normals = oldMesh.normals;
			Vector2[] uv = oldMesh.uv;
			foreach (int num2 in triangles)
			{
				if (!dictionary.ContainsKey(num2))
				{
					MeshHelper.newVerticles[num] = vertices[num2];
					MeshHelper.newNormals[num] = normals[num2];
					MeshHelper.newUvs[num] = uv[num2];
					dictionary.Add(num2, num);
					num++;
				}
			}
			int[] array = new int[triangles.Length];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = dictionary[triangles[j]];
			}
			mesh.vertices = MeshHelper.CopyVector3(MeshHelper.newVerticles, num);
			mesh.uv = MeshHelper.CopyVector2(MeshHelper.newUvs, num);
			mesh.triangles = array;
			mesh.normals = MeshHelper.CopyVector3(MeshHelper.newNormals, num);
			return mesh;
		}

		public static void MergeMeshes_(string skeletonName, GameObject root, List<GameObject> objParts)
		{
			List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>(20);
			for (int i = 0; i < objParts.Count; i++)
			{
				list.AddRange(objParts[i].GetComponentsInChildren<SkinnedMeshRenderer>());
			}
			List<CombineInstance> list2 = null;
			for (int j = 0; j < list.Count; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = list[j];
				if (skinnedMeshRenderer.sharedMesh.subMeshCount > 1)
				{
					list2 = new List<CombineInstance>(5);
					break;
				}
			}
			List<CombineInstance> list3 = new List<CombineInstance>(5);
			List<Material> list4 = new List<Material>(20);
			List<Transform> list5 = new List<Transform>(1000);
			SkinnedMeshRenderer component = root.GetComponent<SkinnedMeshRenderer>();
			if (null == component)
			{
				root.AddComponent(typeof(SkinnedMeshRenderer));
				component = root.GetComponent<SkinnedMeshRenderer>();
			}
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>();
			Dictionary<string, int> dictionary = SkeletonBonesCachingDict.CalcBonesDict(skeletonName, root);
			for (int k = 0; k < list.Count; k++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer2 = list[k];
				list4.AddRange(skinnedMeshRenderer2.sharedMaterials);
				if (list2 != null)
				{
					if (skinnedMeshRenderer2.sharedMesh.subMeshCount > 1)
					{
						for (int l = 0; l < skinnedMeshRenderer2.sharedMesh.subMeshCount; l++)
						{
							CombineInstance combineInstance = default(CombineInstance);
							combineInstance.mesh = MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, l);
							combineInstance.subMeshIndex = 0;
							list2.Add(combineInstance);
						}
					}
					else
					{
						CombineInstance combineInstance = default(CombineInstance);
						combineInstance.mesh = skinnedMeshRenderer2.sharedMesh;
						combineInstance.subMeshIndex = 0;
						list2.Add(combineInstance);
					}
				}
				CombineInstance combineInstance2 = default(CombineInstance);
				combineInstance2.mesh = skinnedMeshRenderer2.sharedMesh;
				combineInstance2.subMeshIndex = 0;
				list3.Add(combineInstance2);
				for (int m = 0; m < skinnedMeshRenderer2.bones.Length; m++)
				{
					string name = skinnedMeshRenderer2.bones[m].name;
					int num = -1;
					if (dictionary.TryGetValue(name, ref num))
					{
						Transform transform = componentsInChildren[num];
						list5.Add(transform);
					}
				}
			}
			if (list2 != null)
			{
				component.sharedMesh = new Mesh();
				component.sharedMesh.CombineMeshes(list2.ToArray(), false, false);
			}
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(list3.ToArray(), false, false);
			if (list2 == null)
			{
				component.sharedMesh = mesh;
			}
			else
			{
				component.sharedMesh.bindposes = mesh.bindposes;
				component.sharedMesh.boneWeights = mesh.boneWeights;
				component.sharedMesh.tangents = mesh.tangents;
				component.sharedMesh.uv2 = mesh.uv2;
			}
			component.bones = list5.ToArray();
			component.sharedMaterials = list4.ToArray();
			MeshHelper.ResetFashionMaterials(objParts, component);
			for (int n = 0; n < objParts.Count; n++)
			{
				GameObject gameObject = objParts[n];
				Object.Destroy(gameObject);
			}
		}

		public static bool HaveSpecialEquip(List<GameObject> objParts)
		{
			bool result = false;
			for (int i = 0; i < objParts.Count; i++)
			{
				if ("ZS_foot_007(Clone)" == objParts[i].name || "GS_foot_005(Clone)" == objParts[i].name || "MJS_foot_006(Clone)" == objParts[i].name)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static void MergeMeshes(string skeletonName, GameObject root, List<GameObject> objParts)
		{
			if (MeshHelper.HaveSpecialEquip(objParts))
			{
				MeshHelper.MergeMeshes_(skeletonName, root, objParts);
				return;
			}
			List<SkinnedMeshRenderer> list = new List<SkinnedMeshRenderer>(objParts.Count + 1);
			for (int i = 0; i < objParts.Count; i++)
			{
				list.AddRange(objParts[i].GetComponentsInChildren<SkinnedMeshRenderer>(true));
			}
			SkinnedMeshRenderer skinnedMeshRenderer = root.GetComponent<SkinnedMeshRenderer>();
			if (!skinnedMeshRenderer)
			{
				skinnedMeshRenderer = root.AddComponent<SkinnedMeshRenderer>();
			}
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(true);
			Dictionary<string, int> dictionary = SkeletonBonesCachingDict.CalcBonesDict(skeletonName, root);
			List<CombineInstance> list2 = new List<CombineInstance>(objParts.Count + 1);
			List<Material> list3 = new List<Material>(objParts.Count + 1);
			List<Transform> list4 = new List<Transform>(componentsInChildren.Length);
			Mesh[] array = null;
			for (int j = 0; j < list.Count; j++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer2 = list[j];
				list3.AddRange(skinnedMeshRenderer2.sharedMaterials);
				if (skinnedMeshRenderer2.sharedMesh.subMeshCount == 2)
				{
					array = new Mesh[]
					{
						MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, 0),
						MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, 1)
					};
					List<CombineInstance> list5 = list2;
					CombineInstance combineInstance = default(CombineInstance);
					combineInstance.mesh = array[0];
					combineInstance.subMeshIndex = 0;
					list5.Add(combineInstance);
					List<CombineInstance> list6 = list2;
					CombineInstance combineInstance2 = default(CombineInstance);
					combineInstance2.mesh = array[1];
					combineInstance2.subMeshIndex = 0;
					list6.Add(combineInstance2);
				}
				else if (skinnedMeshRenderer2.sharedMesh.subMeshCount == 3)
				{
					array = new Mesh[]
					{
						MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, 0),
						MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, 1),
						MeshHelper.CreateMesh(skinnedMeshRenderer2.sharedMesh, 2)
					};
					List<CombineInstance> list7 = list2;
					CombineInstance combineInstance3 = default(CombineInstance);
					combineInstance3.mesh = array[0];
					combineInstance3.subMeshIndex = 0;
					list7.Add(combineInstance3);
					List<CombineInstance> list8 = list2;
					CombineInstance combineInstance4 = default(CombineInstance);
					combineInstance4.mesh = array[1];
					combineInstance4.subMeshIndex = 0;
					list8.Add(combineInstance4);
					List<CombineInstance> list9 = list2;
					CombineInstance combineInstance5 = default(CombineInstance);
					combineInstance5.mesh = array[2];
					combineInstance5.subMeshIndex = 0;
					list9.Add(combineInstance5);
				}
				else
				{
					List<CombineInstance> list10 = list2;
					CombineInstance combineInstance6 = default(CombineInstance);
					combineInstance6.mesh = skinnedMeshRenderer2.sharedMesh;
					combineInstance6.subMeshIndex = 0;
					list10.Add(combineInstance6);
				}
				for (int k = 0; k < skinnedMeshRenderer2.bones.Length; k++)
				{
					string name = skinnedMeshRenderer2.bones[k].name;
					int num = -1;
					if (dictionary.TryGetValue(name, ref num))
					{
						Transform transform = componentsInChildren[num];
						list4.Add(transform);
					}
				}
			}
			Mesh mesh = new Mesh();
			mesh.CombineMeshes(list2.ToArray(), false, false);
			skinnedMeshRenderer.sharedMesh = mesh;
			skinnedMeshRenderer.bones = list4.ToArray();
			skinnedMeshRenderer.materials = list3.ToArray();
			MeshHelper.ResetFashionMaterials(objParts, skinnedMeshRenderer);
			for (int l = 0; l < objParts.Count; l++)
			{
				GameObject gameObject = objParts[l];
				Object.Destroy(gameObject);
			}
			if (array != null)
			{
				for (int m = 0; m < array.Length; m++)
				{
					Object.Destroy(array[m]);
				}
			}
		}

		public static void ResetFashionMaterials(List<GameObject> objParts, SkinnedMeshRenderer r)
		{
			if (objParts.Count >= 5 && (objParts[0].name.Contains("_1000") || objParts[0].name.Contains("_1001")))
			{
				for (int i = 0; i < r.sharedMaterials.Length; i++)
				{
					r.sharedMaterials[i].shader = Shader.Find("Custom/Mobile/Diffuse");
				}
			}
		}

		private static Dictionary<string, Mesh[]> CachedSubmeshes = new Dictionary<string, Mesh[]>();

		public static Vector3[] newVerticles = new Vector3[4096];

		public static Vector3[] newNormals = new Vector3[4096];

		public static Vector2[] newUvs = new Vector2[4096];
	}
}
