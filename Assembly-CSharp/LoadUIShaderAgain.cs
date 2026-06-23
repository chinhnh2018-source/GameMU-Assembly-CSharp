using System;
using UnityEngine;

public class LoadUIShaderAgain : MonoBehaviour
{
	private void Start()
	{
		Renderer[] componentsInChildren = base.gameObject.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
			{
				string name = componentsInChildren[i].sharedMaterials[j].shader.name;
				Shader shader = Shader.Find(name + "ForUI");
				if (shader != null)
				{
					componentsInChildren[i].sharedMaterials[j].shader = shader;
				}
				else
				{
					componentsInChildren[i].sharedMaterials[j].shader = Shader.Find(name);
				}
				name = componentsInChildren[i].sharedMaterials[j].shader.name;
				if (name.EndsWith("ErrorShader"))
				{
				}
			}
		}
	}

	private void Update()
	{
		Object.Destroy(this);
	}
}
