using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class AssetbundleLoader2 : TTMonoBehaviour
{
	public static void SetParent(Transform parent, Transform child)
	{
		Vector3 position = child.position;
		Vector3 localScale = child.localScale;
		Quaternion rotation = child.rotation;
		child.parent = parent;
		child.localScale = localScale;
		child.localPosition = position;
		child.localRotation = rotation;
		child.gameObject.SetActive(true);
	}

	public AssetbundleLoader2 Init(string url, int version)
	{
		this.Url = url;
		this.Version = version;
		return this;
	}

	public static GameObject LocalLoadAssetbundle(string url, Type AssetType, string AssetName, Transform parent)
	{
		GameObject result = null;
		if (string.IsNullOrEmpty(url))
		{
			return null;
		}
		string wwwpath = AssetbundleLoader2.GetWWWPath(AssetbundleLoader2.GetAssetPath(url));
		using (WWW www = new WWW(wwwpath))
		{
			if (!string.IsNullOrEmpty(www.error))
			{
				return null;
			}
			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				throw new Exception("it's surprised to me");
			}
			GameObject gameObject;
			if (!string.IsNullOrEmpty(AssetName))
			{
				gameObject = (www.assetBundle.LoadAsset(AssetName, AssetType) as GameObject);
				gameObject = Object.Instantiate<GameObject>(gameObject);
			}
			else
			{
				gameObject = (Object.Instantiate(www.assetBundle.mainAsset) as GameObject);
			}
			if (null != gameObject)
			{
				AssetbundleLoader2.SetParent(parent, gameObject.transform);
				gameObject.SetActive(true);
			}
			www.assetBundle.Unload(false);
			if (www.assetBundle)
			{
				www.assetBundle.Unload(false);
			}
			www.Dispose();
		}
		return result;
	}

	private void Start()
	{
		base.StartCoroutine<bool>(this.MyStart());
	}

	private IEnumerator MyStart()
	{
		if (string.IsNullOrEmpty(this.Url))
		{
			yield break;
		}
		using (WWW www = new WWW(this.Url))
		{
			yield return www;
			if (!string.IsNullOrEmpty(www.error))
			{
				yield break;
			}
			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				throw new Exception("it's surprised to me");
			}
			GameObject obj = null;
			if (!string.IsNullOrEmpty(this.AssetName))
			{
				obj = (www.assetBundle.LoadAsset(this.AssetName, this.AssetType) as GameObject);
				obj = Object.Instantiate<GameObject>(obj);
			}
			else
			{
				obj = (Object.Instantiate(www.assetBundle.mainAsset) as GameObject);
			}
			if (null != obj)
			{
				AssetbundleLoader2.SetParent(base.transform, obj.transform);
				obj.layer = base.gameObject.layer;
				obj.SetActive(true);
			}
			www.assetBundle.Unload(false);
			if (www.assetBundle)
			{
				www.assetBundle.Unload(false);
			}
			www.Dispose();
		}
		yield break;
	}

	public static string GetAssetPath(string path)
	{
		return PathUtils.WebPath(path);
	}

	public static string GetPersistentPath(string path)
	{
		return Application.persistentDataPath + "/" + path;
	}

	public static string GetWWWPath(string path)
	{
		if (path.StartsWith("http://") || path.StartsWith("ftp://") || path.StartsWith("https://") || path.StartsWith("file://") || path.StartsWith("jar:file://"))
		{
			return path;
		}
		if (Application.platform == 11)
		{
			return path.Insert(0, "jar:file://");
		}
		if (Application.platform != 5 && Application.platform != 3)
		{
			return path.Insert(0, "file://");
		}
		return path;
	}

	public string Url;

	public int Version;

	public string AssetName;

	public Type AssetType;
}
