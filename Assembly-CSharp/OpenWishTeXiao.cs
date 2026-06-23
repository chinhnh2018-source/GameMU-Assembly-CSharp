using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class OpenWishTeXiao : UserControl
{
	private new void Start()
	{
		string path = "UITeXiao/Perfabs/hunyin/hunyin_zhufu";
		this.TObj = this.LoadTeXiao(path, null);
		base.StartCoroutine("DelayDestroy", this.TObj);
	}

	public IEnumerator DelayDestroy(GameObject target)
	{
		yield return new WaitForSeconds(4f);
		PlayZone.GlobalPlayZone.CloseLoversWishWindow();
		Object.Destroy(target);
		Object.Destroy(base.gameObject);
		yield break;
	}

	private new void Destroy()
	{
		base.StopCoroutine("DelayDestroy");
		if (this.TObj != null)
		{
			Object.Destroy(this.TObj);
		}
		Object.Destroy(base.gameObject);
	}

	private GameObject LoadTeXiao(string path, Transform parent = null)
	{
		Object @object = Resources.Load(path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			return gameObject;
		}
		return null;
	}

	private GameObject TObj;
}
