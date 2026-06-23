using System;
using UnityEngine;

public class LoadShaderAgain : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Non-Barrier");
		Renderer[] components = base.gameObject.GetComponents<Renderer>();
		for (int i = 0; i < components.Length; i++)
		{
			for (int j = 0; j < components[i].sharedMaterials.Length; j++)
			{
				string name = components[i].sharedMaterials[j].shader.name;
				components[i].sharedMaterials[j].shader = Shader.Find(name);
				name = components[i].sharedMaterials[j].shader.name;
				if (name.EndsWith("ErrorShader"))
				{
				}
			}
		}
	}
}
