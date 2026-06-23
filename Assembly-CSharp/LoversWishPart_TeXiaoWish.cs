using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class LoversWishPart_TeXiaoWish : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public string Miaoshu
	{
		set
		{
			this.ZhanBao.text = value;
			string path = "UITeXiao/Perfabs/hunyin/hunyin_zhufu";
			this.TObj = this.LoadTeXiao(path, base.gameObject.transform);
			DelayDestroy delayDestroy = base.gameObject.AddComponent<DelayDestroy>();
			delayDestroy.delayTime = 4f;
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.LoversWishPart_TeXiaoWishWindow != null)
			{
				DelayDestroy delayDestroy2 = PlayZone.GlobalPlayZone.LoversWishPart_TeXiaoWishWindow.gameObject.AddComponent<DelayDestroy>();
				delayDestroy2.delayTime = 4f;
			}
		}
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

	public UILabel ZhanBao;

	private GameObject TObj;
}
