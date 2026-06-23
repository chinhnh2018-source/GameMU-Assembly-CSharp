using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameEngine.U3D;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class U3DUtils
	{
		public static T AS<T>(GameObject go) where T : class
		{
			if (null == go)
			{
				return (T)((object)null);
			}
			return go.GetComponent(typeof(T)) as T;
		}

		public static T NEW<T>() where T : class
		{
			string name = typeof(T).ToString();
			GameObject gameObject = U3DUtils.UIResources(name);
			if (null == gameObject)
			{
				return (T)((object)null);
			}
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			return gameObject.GetComponent(typeof(T)) as T;
		}

		public static UIAtlas LoadAtlas(string path)
		{
			Object[] array = Resources.LoadAll(path, typeof(UIAtlas));
			if (array != null && array.Length > 0)
			{
				return array[0] as UIAtlas;
			}
			return null;
		}

		public static T NEW<T>(string name) where T : class
		{
			GameObject gameObject = U3DUtils.UIResources(name);
			if (null == gameObject)
			{
				return (T)((object)null);
			}
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			return gameObject.GetComponent(typeof(T)) as T;
		}

		public static GameObject UIResources(string name)
		{
			string text = "Prefabs/UI/" + name;
			GameObject result;
			try
			{
				Object @object = Resources.Load(text);
				result = (@object as GameObject);
			}
			catch (Exception ex)
			{
				MUDebug.LogError<string>(new string[]
				{
					ex.ToString()
				});
				result = null;
			}
			return result;
		}

		public static bool WWWError(WWW www)
		{
			return www.error != null;
		}

		public static T GetComponentByPrefabName<T>(string prefabName) where T : class
		{
			GameObject gameObject = U3DUtils.UIResources(prefabName);
			if (null == gameObject)
			{
				return (T)((object)null);
			}
			gameObject = (SpawnManager.Instantiate(gameObject) as GameObject);
			return gameObject.GetComponent(typeof(T)) as T;
		}

		public static List<T> GetComponentList<T>(Transform tra) where T : Component
		{
			List<T> list = new List<T>();
			foreach (object obj in tra)
			{
				Transform transform = (Transform)obj;
				list.Add(transform.GetComponentInChildren<T>());
			}
			return list;
		}

		public static string ConvertToNguiColor(uint color)
		{
			string text = Convert.ToString((long)((ulong)color), 16);
			return text.Substring(text.Length - 6);
		}

		public static void ClearAll3DObjects(bool ignoreMainCamera = true, bool ignoreUILayer = true)
		{
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			if (array != null)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				if (ignoreUILayer)
				{
					num = LayerMask.NameToLayer("MUUI");
					num2 = LayerMask.NameToLayer("GUI");
					num3 = LayerMask.NameToLayer("Lights");
				}
				for (int i = 0; i < array.Length; i++)
				{
					if (!ignoreMainCamera || !((GameObject)array[i]).CompareTag("MainCamera"))
					{
						if (null == ((GameObject)array[i]).transform.parent && ((GameObject)array[i]).layer != num && ((GameObject)array[i]).layer != num2 && ((GameObject)array[i]).layer != num3)
						{
							Object.Destroy(array[i]);
						}
					}
				}
			}
		}

		public static void AddChild(GameObject parent, GameObject child, bool takeOldTransform)
		{
			if (child != null && parent != null)
			{
				Transform transform = child.transform;
				Vector3 localPosition = (!takeOldTransform) ? Vector3.zero : transform.localPosition;
				Quaternion localRotation = (!takeOldTransform) ? Quaternion.identity : transform.localRotation;
				Vector3 localScale = (!takeOldTransform) ? Vector3.one : transform.localScale;
				transform.parent = parent.transform;
				transform.localPosition = localPosition;
				transform.localRotation = localRotation;
				transform.localScale = localScale;
				child.layer = parent.layer;
			}
		}

		public static void AddPrefab(GameObject parent, GameObject prefab, bool takeOldTransform)
		{
			GameObject child = SpawnManager.Instantiate(prefab) as GameObject;
			U3DUtils.AddChild(parent, child, takeOldTransform);
		}

		public static GameObject FindGameObjectByName(GameObject root, string theName)
		{
			if (null == root)
			{
				if ("wing" == theName || theName.Length == 4)
				{
					MUDebug.LogError<string>(new string[]
					{
						"root = NULL =====加载翅膀的父类为空======theName= " + theName
					});
				}
				return GameObject.Find(theName);
			}
			Transform transform = root.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				if (transform.GetChild(i).gameObject.name == theName)
				{
					return transform.GetChild(i).gameObject;
				}
			}
			for (int j = 0; j < childCount; j++)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(transform.GetChild(j).gameObject, theName);
				if (null != gameObject)
				{
					return gameObject;
				}
			}
			return null;
		}

		public static int GetTimer()
		{
			long num = U3DUtils.DateTime2010.Ticks / 10000L;
			long num2 = DateTime.Now.Ticks / 10000L - num;
			return (int)num2;
		}

		public static GameObject GetEmptyLoader(string name, string path, bool toReplaceEffectShader = false, AssetbundleLoaderComplete Complete = null, string ownerName = null, int triggerType = -1, AssetbundleLoaderComplete loadOK = null, int layer = -1, float particlescale = 1f, bool forceSyncLoad = true, bool isCache = false, AssetbundleLoaderComplete LoadFail = null)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			AssetbundleLoader assetbundleLoader = gameObject.AddComponent<AssetbundleLoader>();
			assetbundleLoader.BundleID = path;
			assetbundleLoader.ToReplaceEffectShader = toReplaceEffectShader;
			assetbundleLoader.Complete = Complete;
			assetbundleLoader.OwnerName = ownerName;
			assetbundleLoader.TriggerType = triggerType;
			assetbundleLoader.LoadOK = loadOK;
			assetbundleLoader.LoadFail = LoadFail;
			assetbundleLoader.ToLayer = layer;
			assetbundleLoader.ParticleScale = particlescale;
			assetbundleLoader.ForceSyncfLoad = forceSyncLoad;
			assetbundleLoader.IsCache = isCache;
			return gameObject;
		}

		public static GameObject LoadDecoration(string name, string resName, float posX, float posY, float poxZ, string ownerName, int triggerType, AssetbundleLoaderComplete loadOK, int layer, bool forceSyncLoad = true, bool isCache = false)
		{
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(name, MuAssetManager.GetBundleID("Decoration", resName), false, null, ownerName, triggerType, loadOK, layer, 1f, forceSyncLoad, isCache, null);
			emptyLoader.transform.localPosition = new Vector3(posX, posY, poxZ);
			return emptyLoader;
		}

		public static GameObject LoadWeapon(string name, bool trail)
		{
			return null;
		}

		public static GameObject GetWeapon(string name, Transform root, string targetPosName, bool trail)
		{
			return null;
		}

		public static GameObject LoadGoodsPack(string name, string path, float posX, float posY, float poxZ, AssetbundleLoaderComplete Complete = null)
		{
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(name, path, false, Complete, null, -1, null, LayerMask.NameToLayer("Sprites"), 1f, true, false, null);
			emptyLoader.transform.localPosition = new Vector3(posX, posY, poxZ);
			emptyLoader.layer = LayerMask.NameToLayer("Sprites");
			return emptyLoader;
		}

		public static GameObject LoadTeleport(string text, string name, string resName, float posX, float posY, float poxZ, AssetbundleLoaderComplete LoadOK = null, AssetbundleLoaderComplete LoadFail = null)
		{
			string bundleID = MuAssetManager.GetBundleID("Decoration", resName);
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(name, bundleID, true, null, null, -1, LoadOK, -1, 1f, true, false, LoadFail);
			emptyLoader.transform.localPosition = new Vector3(posX, posY, poxZ);
			TeleportInfoDisplay teleportInfoDisplay = emptyLoader.AddComponent<TeleportInfoDisplay>();
			teleportInfoDisplay.Target = emptyLoader.transform;
			teleportInfoDisplay.TeleNameText = text;
			return emptyLoader;
		}

		public static GameObject GetStallShow(int code, int owerRoleID, string showText, Point pos)
		{
			return null;
		}

		public static GameObject LoadNudeModelByName(string name)
		{
			string text = string.Format("Prefabs/NudeModel/{0}", name);
			GameObject gameObject = null;
			if (!U3DUtils.NudeModalAndSkeletonCachingDict.TryGetValue(text, ref gameObject))
			{
				gameObject = (Resources.Load(text) as GameObject);
				if (gameObject != null)
				{
					U3DUtils.NudeModalAndSkeletonCachingDict.Add(text, gameObject);
				}
			}
			return SpawnManager.Instantiate(gameObject) as GameObject;
		}

		public static GameObject LoadSkeletonByName(string name, bool onlyCaching = false)
		{
			string text = string.Format("Prefabs/Skeleton/{0}", name);
			GameObject gameObject = null;
			if (Global.IsOpenABLoadAnim && Context.IsHaiwai)
			{
				if (U3DUtils.NudeModalAndSkeletonCachingDict.ContainsKey(name) && U3DUtils.NudeModalAndSkeletonCachingDict[name] != null)
				{
					gameObject = U3DUtils.NudeModalAndSkeletonCachingDict[name];
				}
			}
			else if (!U3DUtils.NudeModalAndSkeletonCachingDict.TryGetValue(text, ref gameObject))
			{
				gameObject = (Resources.Load(text) as GameObject);
				if (gameObject != null)
				{
					AnimationManager.InitAnimData(Global.GetOccupationBySkeletonName(name), gameObject);
					U3DUtils.NudeModalAndSkeletonCachingDict.Add(text, gameObject);
				}
			}
			if (onlyCaching)
			{
				return null;
			}
			return SpawnManager.Instantiate(gameObject) as GameObject;
		}

		public static GameObject MergeRoleObject(string skeletonName, GameObject skeleton, List<GameObject> partsList, Vector3 ccCenter, float ccRadius, float ccHeight)
		{
			skeleton.AddComponent<SkinnedMeshRenderer>();
			MeshHelper.MergeMeshes(skeletonName, skeleton, partsList);
			return skeleton;
		}

		public static void AddPlayerController(GameObject skeleton = null)
		{
			CameraController cameraController = null;
			if (skeleton == null)
			{
				IObject @object = ObjectsManager.FindSprite("Leader");
				if (@object is GSprite)
				{
					GSprite gsprite = @object as GSprite;
					skeleton = gsprite.The3DGameObject;
				}
				if (null == skeleton)
				{
					skeleton = GameObject.Find("Leader");
				}
				cameraController = skeleton.GetComponent<CameraController>();
				if (cameraController && cameraController.Cam == null)
				{
					cameraController.Cam = Global.MainCamera.gameObject;
					return;
				}
			}
			if (cameraController == null)
			{
				cameraController = (skeleton.AddComponent(typeof(CameraController)) as CameraController);
				cameraController.Cam = Global.MainCamera.gameObject;
				skeleton.AddComponent(typeof(LeaderInfo));
			}
		}

		public static void AddCameraController(GameObject skeleton)
		{
			CameraControllerDrag cameraControllerDrag = null;
			if (cameraControllerDrag == null)
			{
				cameraControllerDrag = (skeleton.GetComponent(typeof(CameraControllerDrag)) as CameraControllerDrag);
				if (null == cameraControllerDrag)
				{
					cameraControllerDrag = (skeleton.AddComponent(typeof(CameraControllerDrag)) as CameraControllerDrag);
				}
				cameraControllerDrag.Cam = Global.MainCamera.gameObject;
			}
		}

		public static GameObject LoadDefaultObject()
		{
			GameObject original = Resources.Load(string.Format("Prefabs/guaiwu/guaiwu", new object[0])) as GameObject;
			return SpawnManager.Instantiate(original) as GameObject;
		}

		public static WingsLingyuResLoader LoadJunQi(GameObject go, string resName, OnWingsLingYuLoadComplete notifyComplete)
		{
			return new WingsLingyuResLoader(new WingsLingYuLoadData
			{
				parent = go,
				path = "Monster",
				resName = resName
			}, notifyComplete);
		}

		public static Vector3 GetGroundPosition2(float x, float z, float y = 0f)
		{
			Vector3 vector;
			vector..ctor(x, 100f, z);
			Vector3 vector2;
			vector2..ctor(x, 0f, z);
			RaycastHit raycastHit = default(RaycastHit);
			int num = 0;
			for (int i = 8; i < 20; i++)
			{
				num |= 1 << i;
			}
			if (Physics.Linecast(vector, vector2, ref raycastHit, num))
			{
				return raycastHit.point;
			}
			return new Vector3(x, y, z);
		}

		public static Vector3 GetGroundPosition3(float x, float z, float y = 0f, int layerMask = -1)
		{
			Vector3 terrainHeight;
			terrainHeight..ctor(x, 0f, z);
			if (!ZoneLoader.DisableSliceTerrain)
			{
				if (!ZoneLoader.singleton.CanMoveByGravity(x, z))
				{
					return new Vector3(x, y, z);
				}
				terrainHeight = ZoneLoader.singleton.GetTerrainHeight(new Vector3(x, y, z));
			}
			Vector3 vector;
			vector..ctor(x, 65f, z);
			Vector3 vector2 = terrainHeight;
			RaycastHit raycastHit = default(RaycastHit);
			if (Physics.Linecast(vector, vector2, ref raycastHit, layerMask))
			{
				return new Vector3(raycastHit.point.x, Math.Max(terrainHeight.y, raycastHit.point.y), raycastHit.point.z);
			}
			if ((double)terrainHeight.y != 0.0)
			{
				return terrainHeight;
			}
			return new Vector3(x, y, z);
		}

		public static GameObject HitTest(Vector3 position, int layer = -1)
		{
			Ray ray = Camera.main.ScreenPointToRay(position);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, ref raycastHit, float.PositiveInfinity, layer))
			{
				return raycastHit.collider.gameObject;
			}
			return null;
		}

		public static GSpriteTypes GetGameObjectTypes(GameObject go)
		{
			if (go.CompareTag("Leader"))
			{
				return GSpriteTypes.Leader;
			}
			if (go.CompareTag("Other"))
			{
				return GSpriteTypes.Other;
			}
			if (go.CompareTag("Monster"))
			{
				return GSpriteTypes.Monster;
			}
			if (go.CompareTag("NPC"))
			{
				return GSpriteTypes.NPC;
			}
			if (go.CompareTag("Pet"))
			{
				return GSpriteTypes.Pet;
			}
			if (go.CompareTag("BiaoChe"))
			{
				return GSpriteTypes.BiaoChe;
			}
			if (go.CompareTag("JunQi"))
			{
				return GSpriteTypes.JunQi;
			}
			if (go.CompareTag("FakeRole"))
			{
				return GSpriteTypes.FakeRole;
			}
			return GSpriteTypes.None;
		}

		public static IObject GetGameObjectOwnerObject(GameObject go)
		{
			OwnerTypeManager component = go.GetComponent<OwnerTypeManager>();
			if (null == component)
			{
				return null;
			}
			return component.OwnerObject;
		}

		public static void ReplaceShader(GameObject go, string[] oldNames, string[] newNames)
		{
			Transform transform = go.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				ParticleSystem component = child.gameObject.GetComponent<ParticleSystem>();
				if (null == component || null == component.GetComponent<Renderer>() || null == component.GetComponent<Renderer>().sharedMaterial || null == component.GetComponent<Renderer>().sharedMaterial.shader)
				{
					U3DUtils.ReplaceShader(child.gameObject, oldNames, newNames);
				}
				else
				{
					for (int j = 0; j < oldNames.Length; j++)
					{
						if (component.GetComponent<Renderer>().sharedMaterial.shader.name.IndexOf(oldNames[j]) >= 0)
						{
							Shader shader = Shader.Find(newNames[j]);
							component.GetComponent<Renderer>().sharedMaterial.shader = shader;
						}
					}
					U3DUtils.ReplaceShader(child.gameObject, oldNames, newNames);
				}
			}
		}

		public static void ReplaceEffectShader(GameObject go)
		{
			U3DUtils.ReplaceShader(go, U3DUtils.EffectShaderOldNames, U3DUtils.EffectShaderNewNames);
		}

		public static void LoadRoleShaderAgain(GameObject go)
		{
		}

		public static Material[] GetMaterials(GameObject go)
		{
			Renderer renderer = go.GetComponent<SkinnedMeshRenderer>();
			if (null == renderer)
			{
				renderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
				if (null == renderer)
				{
					renderer = go.GetComponentInChildren<MeshRenderer>();
				}
			}
			return renderer.sharedMaterials;
		}

		public static void SetToReplaceMaterialsDict(GameObject go, Dictionary<string, int> dict, int shaderID)
		{
			Material[] materials = U3DUtils.GetMaterials(go);
			if (materials != null)
			{
				for (int i = 0; i < materials.Length; i++)
				{
					if (!(null == materials[i].mainTexture))
					{
						dict[materials[i].mainTexture.name] = shaderID;
					}
				}
			}
		}

		private static int GetMaterialShaderID(Material material, Dictionary<string, int> toReplaceDict, int shaderID)
		{
			if (toReplaceDict == null)
			{
				return shaderID;
			}
			if (null == material.mainTexture)
			{
				return shaderID;
			}
			if (!toReplaceDict.TryGetValue(material.mainTexture.name, ref shaderID))
			{
				return shaderID;
			}
			return shaderID;
		}

		public static void ReplaceMaterials(GameObject go, int shaderID, bool isWeaponLoader = false)
		{
			if (go == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"ReplaceMaterials对象为空,不应该为空"
				});
				return;
			}
			Renderer[] array = null;
			if (shaderID <= 0)
			{
				array = go.GetComponentsInChildren<Renderer>();
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						Material[] sharedMaterials = array[i].sharedMaterials;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							Texture texture = sharedMaterials[j].GetTexture("_MainTex");
							if (texture.name.IndexOf("_alpha") >= 0)
							{
								sharedMaterials[j].shader = Shader.Find("Custom/Mobile/Diffuse");
							}
							else
							{
								sharedMaterials[j].shader = Shader.Find(array[i].sharedMaterial.shader.name);
							}
						}
					}
				}
				return;
			}
			if (isWeaponLoader)
			{
				if (go.transform.childCount > 0)
				{
					array = new Renderer[]
					{
						go.transform.GetChild(0).GetComponent<Renderer>()
					};
				}
			}
			else
			{
				array = go.GetComponentsInChildren<Renderer>();
			}
			if (array != null)
			{
				for (int k = 0; k < array.Length; k++)
				{
					Renderer renderer = array[k];
					if (!(null == renderer))
					{
						if (!(renderer is ParticleRenderer) && !(renderer is ParticleSystemRenderer))
						{
							Material[] array2 = new Material[renderer.sharedMaterials.Length];
							for (int l = 0; l < renderer.sharedMaterials.Length; l++)
							{
								if (renderer.sharedMaterials[l].shader.name.StartsWith("Custom/Mobile/Particles/Additive Culled"))
								{
									array2[l] = renderer.sharedMaterials[l];
								}
								else
								{
									Texture texture2 = renderer.sharedMaterials[l].GetTexture("_MainTex");
									Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[l], shaderID, texture2.name.IndexOf("_alpha") >= 0);
									if (null == materialReflByShaderID)
									{
										array2[l] = materialReflByShaderID;
									}
									else
									{
										materialReflByShaderID.SetTexture("_MainTex", texture2);
										array2[l] = materialReflByShaderID;
									}
								}
							}
							array[k].materials = array2;
						}
					}
				}
			}
		}

		public static void ReplaceMaterials4Monster(GameObject go, int shaderID, bool isWeaponLoader = false)
		{
			if (go == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"ReplaceMaterials对象为空,不应该为空"
				});
				return;
			}
			Renderer[] array = null;
			if (shaderID <= 0)
			{
				array = go.GetComponentsInChildren<Renderer>();
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						Material[] sharedMaterials = array[i].sharedMaterials;
						for (int j = 0; j < sharedMaterials.Length; j++)
						{
							Texture texture = sharedMaterials[j].GetTexture("_MainTex");
							if (texture.name.IndexOf("_alpha") >= 0)
							{
								sharedMaterials[j].shader = Shader.Find("Custom/Mobile/Diffuse");
							}
							else
							{
								sharedMaterials[j].shader = Shader.Find(array[i].sharedMaterial.shader.name);
							}
						}
					}
				}
				return;
			}
			if (isWeaponLoader)
			{
				if (go.transform.childCount > 0)
				{
					array = new Renderer[]
					{
						go.transform.GetChild(0).GetComponent<Renderer>()
					};
				}
			}
			else
			{
				array = go.GetComponentsInChildren<Renderer>();
			}
			if (array != null)
			{
				for (int k = 0; k < array.Length; k++)
				{
					Renderer renderer = array[k];
					if (!(null == renderer))
					{
						if (!(renderer is ParticleRenderer) && !(renderer is ParticleSystemRenderer))
						{
							if (renderer is MeshRenderer)
							{
								MeshFilter component = renderer.gameObject.GetComponent<MeshFilter>();
								if (component != null)
								{
									if (!component.name.StartsWith("Weapon"))
									{
										goto IL_269;
									}
								}
							}
							Material[] array2 = new Material[renderer.sharedMaterials.Length];
							for (int l = 0; l < renderer.sharedMaterials.Length; l++)
							{
								if (renderer.sharedMaterials[l].shader.name.StartsWith("Custom/Mobile/Particles/Additive Culled"))
								{
									array2[l] = renderer.sharedMaterials[l];
								}
								else
								{
									Texture texture2 = renderer.sharedMaterials[l].GetTexture("_MainTex");
									Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[l], shaderID, texture2.name.IndexOf("_alpha") >= 0);
									if (null == materialReflByShaderID)
									{
										array2[l] = materialReflByShaderID;
									}
									else
									{
										materialReflByShaderID.SetTexture("_MainTex", texture2);
										array2[l] = materialReflByShaderID;
									}
								}
							}
							array[k].materials = array2;
						}
					}
					IL_269:;
				}
			}
		}

		public static void ReplaceMaterials(GameObject go, Dictionary<string, int> toReplaceDict = null)
		{
			if (go == null)
			{
				return;
			}
			Renderer renderer = go.GetComponent<SkinnedMeshRenderer>();
			if (null == renderer)
			{
				renderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
				if (null == renderer)
				{
					renderer = go.GetComponentInChildren<MeshRenderer>();
				}
			}
			Material[] array = new Material[renderer.sharedMaterials.Length];
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				Texture texture = renderer.sharedMaterials[i].GetTexture("_MainTex");
				int materialShaderID = U3DUtils.GetMaterialShaderID(renderer.sharedMaterials[i], toReplaceDict, 0);
				if (materialShaderID <= 0)
				{
					array[i] = renderer.sharedMaterials[i];
					string name = array[i].shader.name;
					array[i].shader = Shader.Find(name);
				}
				else
				{
					Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[i], materialShaderID, texture.name.IndexOf("_alpha") >= 0);
					if (null == materialReflByShaderID)
					{
						array[i] = renderer.sharedMaterials[i];
						string name2 = array[i].shader.name;
						array[i].shader = Shader.Find(name2);
					}
					else
					{
						materialReflByShaderID.SetTexture("_MainTex", texture);
						array[i] = materialReflByShaderID;
					}
				}
			}
			renderer.materials = array;
		}

		public static void ReplaceAlphaMaterials(GameObject go)
		{
			Renderer renderer = go.GetComponent<SkinnedMeshRenderer>();
			if (null == renderer)
			{
				renderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
				if (null == renderer)
				{
					renderer = go.GetComponentInChildren<MeshRenderer>();
				}
			}
			Material material = Resources.Load("Materials/NormalAlpha") as Material;
			if (null == material)
			{
				return;
			}
			Material[] array = new Material[renderer.sharedMaterials.Length];
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				Texture texture = renderer.sharedMaterials[i].GetTexture("_MainTex");
				if (texture.name.IndexOf("_alpha") < 0)
				{
					array[i] = renderer.sharedMaterials[i];
				}
				else
				{
					Material material2 = SpawnManager.Instantiate(material) as Material;
					material2.SetTexture("_MainTex", texture);
					array[i] = material2;
				}
			}
			renderer.materials = array;
		}

		public static Material GetMaterialReflByShaderID(Material origin, int shaderID, bool isAlpha)
		{
			Material result;
			if (2000 <= shaderID && 3000 > shaderID)
			{
				result = MaterialManager.GetFashionMaterialReflByShaderID(origin, shaderID, isAlpha);
			}
			else
			{
				result = MaterialManager.GetMaterialReflByShaderID(shaderID, isAlpha);
			}
			return result;
		}

		public static void SetZhuoYueDict(GameObject go, Dictionary<string, int> dict, int excellenceInfo)
		{
			Material[] materials = U3DUtils.GetMaterials(go);
			if (materials != null)
			{
				for (int i = 0; i < materials.Length; i++)
				{
					if (!(null == materials[i]))
					{
						if (!(null == materials[i].mainTexture))
						{
							dict[materials[i].mainTexture.name] = excellenceInfo;
						}
					}
				}
			}
		}

		public static void AddZhuoYueMaterials(GameObject go, ChangeColor changeColor, Dictionary<string, int> toReplaceDict = null)
		{
			if (go == null)
			{
				return;
			}
			Renderer renderer = go.GetComponent<SkinnedMeshRenderer>();
			if (null == renderer)
			{
				renderer = go.GetComponentInChildren<SkinnedMeshRenderer>();
				if (null == renderer)
				{
					renderer = go.GetComponentInChildren<MeshRenderer>();
				}
			}
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				int num = 0;
				if (toReplaceDict.TryGetValue(renderer.sharedMaterials[i].mainTexture.name, ref num))
				{
					if (num > 0)
					{
						changeColor.AddMaterial(renderer.sharedMaterials[i]);
					}
				}
			}
		}

		public static void EnableCollider(GameObject go, bool active = true)
		{
			Collider[] componentsInChildren = go.GetComponentsInChildren<Collider>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = active;
			}
		}

		public static GameObject Clone(GameObject parent, GameObject go)
		{
			if (go != null && parent != null)
			{
				GameObject gameObject = SpawnManager.Instantiate(go) as GameObject;
				Transform transform = gameObject.transform;
				transform.parent = parent.transform;
				transform.localPosition = go.transform.localPosition;
				transform.localRotation = go.transform.localRotation;
				transform.localScale = go.transform.localScale;
				gameObject.layer = go.layer;
				return gameObject;
			}
			return null;
		}

		public static bool ComponentIsEnabled(MonoBehaviour comp)
		{
			return null != comp && comp.enabled && comp.gameObject.activeInHierarchy;
		}

		public static void ModifyAnimationSpeed(GameObject go, float speed)
		{
			Animation component = go.GetComponent<Animation>();
			if (component != null)
			{
				foreach (object obj in component)
				{
					AnimationState animationState = (AnimationState)obj;
					animationState.speed = speed;
				}
			}
			Animator component2 = go.GetComponent<Animator>();
			if (component2 != null)
			{
				component2.speed = speed;
			}
		}

		public static float GetAnimationSpeed(GameObject go)
		{
			if (null == go.GetComponent<Animation>())
			{
				return 1f;
			}
			Animation component = go.GetComponent<Animation>();
			using (IEnumerator enumerator = component.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AnimationState animationState = (AnimationState)enumerator.Current;
					return animationState.speed;
				}
			}
			return 1f;
		}

		public static void ReplaceLayerInChildren(GameObject go, int layer, string[] ignoreList = null)
		{
			if (null == go)
			{
				return;
			}
			Transform transform = go.transform;
			if (null == transform)
			{
				return;
			}
			go.layer = layer;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = transform.GetChild(i).gameObject;
				if (!(gameObject == null) && gameObject.layer != 1)
				{
					gameObject.layer = layer;
					U3DUtils.ReplaceLayerInChildren(gameObject, layer, ignoreList);
				}
			}
		}

		public static void DestoryAllChild(GameObject go)
		{
			if (null == go)
			{
				return;
			}
			Transform transform = go.transform;
			if (null == transform)
			{
				return;
			}
			int childCount = transform.childCount;
			for (int i = childCount - 1; i >= 0; i--)
			{
				Object.Destroy(transform.GetChild(i).gameObject);
			}
		}

		public static void ModifyDirectLight(GameObject directLight, int cullingMask)
		{
			Light component = directLight.GetComponent<Light>();
			if (null == component)
			{
				return;
			}
			component.cullingMask = cullingMask;
		}

		public static void SetEffectManagerParams(GameObject go, string ownerName, int triggerType)
		{
			EffectManager[] componentsInChildren = go.GetComponentsInChildren<EffectManager>();
			if (componentsInChildren == null)
			{
				return;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].OwnerName = ownerName;
				componentsInChildren[i].TriggerType = triggerType;
			}
		}

		public static Vector3 GetMeshSize(Transform trans)
		{
			Renderer componentInChildren = trans.GetComponentInChildren<Renderer>();
			if (null != componentInChildren)
			{
				return componentInChildren.bounds.center + new Vector3(0f, componentInChildren.bounds.extents.y, 0f) - trans.position;
			}
			MeshFilter componentInChildren2 = trans.GetComponentInChildren<MeshFilter>();
			if (null != componentInChildren2)
			{
				return new Vector3(0f, componentInChildren2.mesh.bounds.extents.y, 0f);
			}
			return Vector3.zero;
		}

		public static void AddBoxCollider(GameObject go, Vector3 center, Vector3 size, bool isTrigger = false)
		{
			BoxCollider boxCollider = go.GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = go.AddComponent<BoxCollider>();
			}
			boxCollider.center = center;
			boxCollider.size = size;
			boxCollider.isTrigger = isTrigger;
		}

		public static Component CopyComponent(Component original, Component destination, string[] fieldNames)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			for (int i = 0; i < fieldNames.Length; i++)
			{
				dictionary[fieldNames[i]] = true;
			}
			Type type = original.GetType();
			foreach (FieldInfo fieldInfo in type.GetFields())
			{
				if (dictionary.ContainsKey(fieldInfo.Name))
				{
					fieldInfo.SetValue(destination, fieldInfo.GetValue(original));
				}
			}
			return destination;
		}

		public static void ResetLayer(GameObject obj, string layerName)
		{
			Transform[] componentsInChildren = obj.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer(layerName);
			}
			obj.transform.localPosition = new Vector3(0f, 0f, -190f);
		}

		public static void ChangePerformanceTypeShader(Transform lader, PerformanceTypes state)
		{
		}

		public static GameObject GetEmptyGameObjcetInScene(Transform parent = null)
		{
			GameObject gameObject = new GameObject();
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
			Object.Destroy(gameObject);
			if (null != parent)
			{
				gameObject2.transform.SetParent(parent, false);
			}
			return gameObject2;
		}

		public static IEnumerator LoadSekletonPrefab(string name)
		{
			string path = PathUtils.WebPath(string.Format("Skeleton/{0}.unity3d", name));
			WWW www = WWW.LoadFromCacheOrDownload(path, Global.SekletonVersion);
			yield return www;
			GameObject obj = null;
			if (www.isDone && string.IsNullOrEmpty(www.error))
			{
				AssetBundleRequest abr = www.assetBundle.LoadAssetAsync(name, typeof(Object));
				yield return abr;
				obj = (abr.asset as GameObject);
				if (obj != null)
				{
					if (U3DUtils.NudeModalAndSkeletonCachingDict.ContainsKey(name))
					{
						U3DUtils.NudeModalAndSkeletonCachingDict[name] = obj;
					}
					else
					{
						U3DUtils.NudeModalAndSkeletonCachingDict.Add(name, obj);
					}
				}
			}
			yield break;
		}

		private static DateTime DateTime2010 = new DateTime(2010, 1, 1, 8, 0, 0);

		private static Dictionary<string, GameObject> NudeModalAndSkeletonCachingDict = new Dictionary<string, GameObject>();

		private static string[] EffectShaderOldNames = new string[]
		{
			"Xffect/heat_distortion"
		};

		private static string[] EffectShaderNewNames = new string[]
		{
			"Xffect/heat_distortion"
		};

		private static Transform m_Lader_YingZi = null;
	}
}
