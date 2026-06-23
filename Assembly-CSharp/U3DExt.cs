using System;
using UnityEngine;

public static class U3DExt
{
	public static T SafeGetComponent<T>(this GameObject gameObject) where T : Component
	{
		T t = (T)((object)null);
		if (null != gameObject)
		{
			t = gameObject.GetComponent<T>();
		}
		if (null == t)
		{
			t = (T)((object)null);
		}
		return t;
	}

	public static void xMakePerfect(this UISprite sprite)
	{
		Vector3 localPosition = sprite.transform.localPosition;
		sprite.MakePixelPerfect();
		sprite.transform.localPosition = new Vector3(localPosition.x, localPosition.y, localPosition.z);
	}

	public static void xMakePerfect(this ShowNetImage image)
	{
		image.Texture.MakePixelPerfect();
	}
}
