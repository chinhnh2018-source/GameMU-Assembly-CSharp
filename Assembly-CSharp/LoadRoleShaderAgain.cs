using System;
using UnityEngine;

public class LoadRoleShaderAgain : MonoBehaviour
{
	private void Start()
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].gameObject.name.CompareTo("yingzi(Clone)") != 0)
			{
				for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
				{
					if (componentsInChildren[i].sharedMaterials[j] != null)
					{
						string name = componentsInChildren[i].sharedMaterials[j].shader.name;
						int renderQueue = componentsInChildren[i].sharedMaterials[j].renderQueue;
						componentsInChildren[i].sharedMaterials[j].shader = Shader.Find(name);
						componentsInChildren[i].sharedMaterials[j].renderQueue = ((renderQueue != 2000) ? renderQueue : -1);
						name = componentsInChildren[i].sharedMaterials[j].shader.name;
						if (name.EndsWith("ErrorShader"))
						{
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"sharedMaterial is null : " + componentsInChildren[i].name
						});
					}
				}
			}
		}
	}

	private void Update()
	{
		Object.Destroy(this);
	}
}
