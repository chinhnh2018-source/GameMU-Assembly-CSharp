using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class DestroyGoodsPack : MonoBehaviour
{
	public void OnTweenToTartetComplete()
	{
		if (this._NotifyDestroyGoods != null)
		{
			this._NotifyDestroyGoods();
		}
	}

	public Transform Trans
	{
		get
		{
			if (null == this._Trans)
			{
				this._Trans = base.transform;
			}
			return this._Trans;
		}
	}

	public void StartMove(int goodsId)
	{
		if (PlayZone.GlobalPlayZone.GameChatBoxMini != null && Global.MainCamera != null && UICamera.currentCamera)
		{
			Vector3 vector = Global.MainCamera.WorldToScreenPoint(base.transform.position);
			vector = UICamera.currentCamera.ScreenToWorldPoint(vector);
			PlayZone.GlobalPlayZone.GameChatBoxMini.PickUpGoods(goodsId, vector);
			this.OnTweenToTartetComplete();
			Object.Destroy(this);
		}
	}

	public NotifyDestroyGoodsHandler _NotifyDestroyGoods;

	public Vector3 SpritePos = Vector3.zero;

	public float TranslationTime = 1f;

	private Transform _Trans;
}
