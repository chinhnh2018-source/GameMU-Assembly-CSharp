using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZoneLoader : TTMonoBehaviour
{
	public static ZoneLoader singleton
	{
		get
		{
			if (ZoneLoader.ms_Singleton == null)
			{
				GameObject gameObject = new GameObject("ZoneLoader", new Type[]
				{
					typeof(ZoneLoader)
				});
				ZoneLoader.ms_Singleton = (gameObject.GetComponent(typeof(ZoneLoader)) as ZoneLoader);
			}
			return ZoneLoader.ms_Singleton;
		}
	}

	private string GetBaseUrl()
	{
		return Global.WebPath(string.Format("Map/{0}/", this.MapName));
	}

	public string GetPrefix(string prefix, string postfix, int x, int y)
	{
		return string.Format("{0}{1}_{2}{3}", new object[]
		{
			prefix,
			y + 1,
			x + 1,
			postfix
		});
	}

	public string GetPrefix(int x, int y)
	{
		return this.GetPrefix(string.Format("{0}_BatchScene_", this.MapName), string.Empty, x, y);
	}

	private IEnumerator LoadZone(int x, int y)
	{
		if (this.m_IsLoading)
		{
			MUDebug.LogError<string>(new string[]
			{
				"Already loading zone"
			});
			yield break;
		}
		ZoneLoader.Zone zone = this.m_Zones[4 * y + x];
		if (!zone.m_IsLoadable)
		{
			yield break;
		}
		this.m_IsLoading = true;
		string levelName = this.GetPrefix(x, y);
		if (this.m_UseWWWCaching || this.m_UseWWW)
		{
			string fullUrl = this.GetBaseUrl() + levelName + ".unity3d";
			if (this.m_UseWWWCaching)
			{
				zone.m_ZoneDownload = WWW.LoadFromCacheOrDownload(fullUrl, this.m_CacheVersion);
			}
			else
			{
				zone.m_ZoneDownload = new WWW(fullUrl);
			}
			yield return zone.m_ZoneDownload;
			if (!string.IsNullOrEmpty(zone.m_ZoneDownload.error))
			{
				MUDebug.LogError<string>(new string[]
				{
					zone.m_ZoneDownload.error
				});
				zone.m_IsLoadable = false;
				this.m_IsLoading = false;
				zone.m_ZoneDownload.Dispose();
				zone.m_ZoneDownload = null;
				yield break;
			}
			zone.m_ZoneBundle = zone.m_ZoneDownload.assetBundle;
			zone.m_ZoneDownload.Dispose();
			zone.m_ZoneDownload = null;
			if (zone.m_ZoneBundle == null)
			{
				zone.m_IsLoadable = false;
				this.m_IsLoading = false;
				yield break;
			}
		}
		AsyncOperation async = Application.LoadLevelAdditiveAsync(levelName);
		yield return async;
		zone.m_Root = GameObject.Find(string.Format("MyScene_{0}_{1}", y + 1, x + 1));
		if (zone.m_Root != null)
		{
			Transform terain = zone.m_Root.transform.Find(string.Format("Terrain_Slice_{0}_{1}(Clone)", y + 1, x + 1));
			if (null == terain)
			{
				terain = zone.m_Root.transform.Find(string.Format("Terrain_Slice_{0}_{1}", y + 1, x + 1));
			}
			if (terain)
			{
				zone.m_Terrain = (terain.GetComponent(typeof(Terrain)) as Terrain);
				zone.m_Terrain.heightmapPixelError = 50f;
				zone.m_Terrain.basemapDistance = 100f;
				zone.m_Terrain.gameObject.layer = LayerMask.NameToLayer("Terrain");
			}
			zone.m_Loaded = true;
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				levelName + " could not be found after loading level"
			});
		}
		for (int yi = Mathf.Max(y - 1, 0); yi < Mathf.Min(y + 2, 4); yi++)
		{
			for (int xi = Mathf.Max(x - 1, 0); xi < Mathf.Min(x + 2, 4); xi++)
			{
				Terrain curTerrain = this.GetLoadedTerrain(xi, yi);
				if (curTerrain != null)
				{
					Terrain left = this.GetLoadedTerrain(xi - 1, yi);
					Terrain right = this.GetLoadedTerrain(xi + 1, yi);
					Terrain top = this.GetLoadedTerrain(xi, yi + 1);
					Terrain bottom = this.GetLoadedTerrain(xi, yi - 1);
					curTerrain.SetNeighbors(left, top, right, bottom);
				}
			}
		}
		this.m_IsLoading = false;
		yield break;
	}

	private IEnumerator UnloadZone(int x, int y)
	{
		this.m_IsUnloading = true;
		ZoneLoader.Zone zone = this.m_Zones[4 * y + x];
		zone.m_Loaded = false;
		if (zone.m_Root)
		{
			Object.Destroy(zone.m_Root);
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"Root for zone has already been unloaded:" + this.GetPrefix(x, y)
			});
		}
		zone.m_Terrain = null;
		zone.m_Root = null;
		yield return 0;
		if (this.m_UseWWWCaching || this.m_UseWWW)
		{
			zone.m_ZoneBundle.Unload(true);
			zone.m_ZoneBundle = null;
		}
		ZoneLoader.m_ZonesUnloadedSinceLastAssetUnload++;
		this.m_IsUnloading = false;
		yield break;
	}

	private void Awake()
	{
		this.m_Zones = new ZoneLoader.Zone[16];
		for (int i = 0; i < this.m_Zones.Length; i++)
		{
			this.m_Zones[i] = new ZoneLoader.Zone();
		}
		if (this.m_UseWWWCaching)
		{
		}
		ZoneLoader.ms_Singleton = this;
	}

	private float GetSqrDistance(Vector3 position, int x, int y)
	{
		float num = (float)x * this.m_GridSize;
		float num2 = (float)(x + 1) * this.m_GridSize;
		float num3 = (float)y * this.m_GridSize;
		float num4 = (float)(y + 1) * this.m_GridSize;
		float num5 = 0f;
		float num6 = 0f;
		if (position.x < num)
		{
			num5 = Mathf.Abs(num - position.x);
		}
		else if (position.x > num2)
		{
			num5 = Mathf.Abs(num2 - position.x);
		}
		if (position.z < num3)
		{
			num6 = Mathf.Abs(num3 - position.z);
		}
		else if (position.z > num4)
		{
			num6 = Mathf.Abs(num4 - position.z);
		}
		return num5 * num5 + num6 * num6;
	}

	private void GetClosestZone(Vector3 position, float closestDistance, out int closestX, out int closestY)
	{
		closestX = -1;
		closestY = -1;
		closestDistance *= closestDistance;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ZoneLoader.Zone zone = this.m_Zones[4 * i + j];
				if (!zone.m_Loaded && zone.m_IsLoadable)
				{
					float sqrDistance = this.GetSqrDistance(position, j, i);
					if (sqrDistance < closestDistance)
					{
						closestDistance = sqrDistance;
						closestX = j;
						closestY = i;
					}
				}
			}
		}
	}

	private void Update()
	{
		if (this.m_PlayerTransform)
		{
			this.m_PlayerPosition = this.m_PlayerTransform.position;
		}
		if (this.m_IsLoading || this.m_IsUnloading)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ZoneLoader.Zone zone = this.m_Zones[4 * i + j];
				if (zone != null)
				{
					if (zone.m_Loaded)
					{
						float sqrDistance = this.GetSqrDistance(this.m_PlayerPosition, j, i);
						if (sqrDistance > 900f)
						{
							base.StartCoroutine<bool>(this.UnloadZone(j, i));
							return;
						}
					}
				}
			}
		}
		int num;
		int y;
		this.GetClosestZone(this.m_PlayerPosition, 30f, out num, out y);
		if (num != -1)
		{
			base.StartCoroutine<bool>(this.LoadZone(num, y));
		}
	}

	private Terrain GetLoadedTerrain(int x, int y)
	{
		if (x >= 0 && x < 4 && y >= 0 && y < 4)
		{
			ZoneLoader.Zone zone = this.m_Zones[4 * y + x];
			if (zone.m_Loaded)
			{
				return zone.m_Terrain;
			}
		}
		return null;
	}

	public static void ReleaseSingleton()
	{
		if (null == ZoneLoader.ms_Singleton)
		{
			return;
		}
		ZoneLoader.ms_Singleton.ReleaseAssetBundles();
	}

	private void ReleaseAssetBundles()
	{
		if (this.m_Zones.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				ZoneLoader.Zone zone = this.m_Zones[4 * i + j];
				if (zone != null)
				{
					if (zone.m_Loaded)
					{
						zone.m_Loaded = false;
						zone.m_Terrain = null;
						zone.m_Root = null;
						if (null != zone.m_ZoneBundle)
						{
							zone.m_ZoneBundle.Unload(false);
							zone.m_ZoneBundle = null;
						}
					}
				}
			}
		}
		Object.Destroy(base.gameObject);
		ZoneLoader.ms_Singleton = null;
	}

	public bool CanMoveByGravity(float x, float z)
	{
		if (this.m_GridSize == 0f)
		{
			return false;
		}
		int num = (int)Math.Ceiling((double)(x / this.m_GridSize));
		int num2 = (int)Math.Ceiling((double)(z / this.m_GridSize));
		if (num < 0 || num2 < 0)
		{
			return false;
		}
		int num3 = this.m_Zones.Length;
		int num4 = 4 * (num2 - 1) + num - 1;
		if (num4 < 0 || num4 >= num3)
		{
			return false;
		}
		ZoneLoader.Zone zone = this.m_Zones[4 * (num2 - 1) + num - 1];
		return zone != null && zone.m_Loaded;
	}

	public Vector3 GetTerrainHeight(Vector3 pos)
	{
		if (this.m_GridSize == 0f)
		{
			return Vector3.zero;
		}
		int num = (int)Math.Ceiling((double)(pos.x / this.m_GridSize));
		int num2 = (int)Math.Ceiling((double)(pos.z / this.m_GridSize));
		if (num < 0 || num2 < 0)
		{
			return Vector3.zero;
		}
		int num3 = this.m_Zones.Length;
		int num4 = 4 * (num2 - 1) + num - 1;
		if (num4 < 0 || num4 >= num3)
		{
			return Vector3.zero;
		}
		ZoneLoader.Zone zone = this.m_Zones[4 * (num2 - 1) + num - 1];
		if (zone == null)
		{
			return Vector3.zero;
		}
		return new Vector3(pos.x, zone.m_Terrain.SampleHeight(pos), pos.z);
	}

	private const float m_LoadDistance = 30f;

	private const float m_UnloadDistance = 30f;

	public const int m_ZoneCount = 4;

	public float m_GridSize = 50f;

	public string MapName = "unkown";

	public static bool DisableSliceTerrain;

	private static ZoneLoader ms_Singleton;

	private bool m_IsLoading;

	private bool m_IsUnloading;

	private bool m_UseWWW = true;

	private bool m_UseWWWCaching;

	private int m_CacheVersion = 1;

	public Vector3 m_PlayerPosition = Vector2.zero;

	public Transform m_PlayerTransform;

	private static int m_ZonesUnloadedSinceLastAssetUnload;

	private ZoneLoader.Zone[] m_Zones = new ZoneLoader.Zone[16];

	public class Zone
	{
		public bool m_Loaded;

		public bool m_IsLoadable = true;

		public GameObject m_Root;

		public Terrain m_Terrain;

		public AssetBundle m_ZoneBundle;

		public WWW m_ZoneDownload;
	}
}
