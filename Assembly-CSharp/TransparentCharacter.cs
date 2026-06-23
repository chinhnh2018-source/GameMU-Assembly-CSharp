using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class TransparentCharacter : MonoBehaviour
{
	public static Vector3 ScreenCenter
	{
		get
		{
			if (TransparentCharacter.screenCenter == Vector3.zero)
			{
				TransparentCharacter.screenCenter = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
			}
			return TransparentCharacter.screenCenter;
		}
	}

	private void Start()
	{
		this.leaderInfo = base.GetComponent<LeaderInfo>();
		this.ignoreLayer = (1 << LayerMask.NameToLayer("building") | 1 << LayerMask.NameToLayer("Sprites"));
		this.camera = Global.MainCamera;
		if (this.originMats == null)
		{
			this.originMats = new List<Material>();
			Material[] sharedMaterials;
			if (base.GetComponent<Renderer>() != null)
			{
				sharedMaterials = base.GetComponent<Renderer>().sharedMaterials;
			}
			else
			{
				sharedMaterials = base.GetComponentInChildren<Renderer>().sharedMaterials;
			}
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				this.originMats.Add(sharedMaterials[i]);
			}
		}
		if (this.transparentMats == null)
		{
			this.transparentMats = new List<Material>();
			for (int j = 0; j < this.originMats.Count; j++)
			{
				Material material = new Material(Shader.Find("MU/OverlayTransparent"));
				material.mainTexture = this.originMats[j].mainTexture;
				this.transparentMats.Add(material);
			}
		}
		base.StartCoroutine(this.TestOcclution());
	}

	private IEnumerator TestOcclution()
	{
		while (base.enabled)
		{
			if (null != this.leaderInfo)
			{
				Ray r = new Ray(this.camera.transform.position, base.transform.position + new Vector3(0f, (!this.leaderInfo.LeaderIsOnHorse) ? 1.5f : 0f, 0f) - this.camera.transform.position);
				RaycastHit hitInfo;
				if (Physics.Raycast(r, ref hitInfo, 20f, this.ignoreLayer) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("building"))
				{
					if (!this.leaderInfo.TriggerByCancel && !this.blocked)
					{
						if (base.GetComponent<Renderer>() == null)
						{
							base.GetComponentInChildren<Renderer>().sharedMaterials = this.transparentMats.ToArray();
						}
						else
						{
							base.GetComponent<Renderer>().sharedMaterials = this.transparentMats.ToArray();
						}
						this.blocked = true;
					}
				}
				else if (this.blocked)
				{
					if (base.GetComponent<Renderer>() != null)
					{
						base.GetComponent<Renderer>().sharedMaterials = this.originMats.ToArray();
					}
					else
					{
						base.GetComponentInChildren<Renderer>().sharedMaterials = this.originMats.ToArray();
					}
					this.blocked = false;
				}
			}
			yield return new WaitForSeconds(0.3f);
		}
		yield break;
	}

	private List<Material> originMats;

	private List<Material> transparentMats;

	private bool blocked;

	private Camera camera;

	private LeaderInfo leaderInfo;

	private static Vector3 screenCenter = Vector3.zero;

	private LayerMask ignoreLayer;
}
