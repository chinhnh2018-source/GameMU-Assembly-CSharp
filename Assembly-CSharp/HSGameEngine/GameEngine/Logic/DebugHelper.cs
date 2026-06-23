using System;
using System.Text;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public static class DebugHelper
	{
		public static string ShowAllObjects()
		{
			Object[] array = Object.FindObjectsOfType(typeof(GameObject));
			if (array == null)
			{
				return "0 - objects";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i] as GameObject;
				if (!(null != gameObject.transform.parent))
				{
					if (gameObject.layer != LayerMask.NameToLayer("MUUI"))
					{
						stringBuilder.AppendFormat("Object.Name={0}, Active={1}, Position={2}, Roation={3}, Scale={4}, ChildCount={5}\n", new object[]
						{
							gameObject.name,
							gameObject.activeSelf,
							gameObject.transform.localPosition,
							gameObject.transform.localRotation,
							gameObject.transform.localScale,
							gameObject.transform.childCount
						});
					}
				}
			}
			stringBuilder.AppendFormat("{0} - objects\n", array.Length);
			return stringBuilder.ToString();
		}
	}
}
