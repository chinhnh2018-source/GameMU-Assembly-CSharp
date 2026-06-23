using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class LeaderTrigger : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Non-Visible");
		this._animation = base.transform.parent.GetComponent<Animation>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.name.StartsWith("Leader"))
		{
			GameObject gameObject = U3DUtils.FindGameObjectByName(Camera.main.gameObject, "XiaXue");
			if (null != gameObject)
			{
				gameObject.SetActive(false);
			}
			LeaderInfo component = other.GetComponent<LeaderInfo>();
			if (null != component)
			{
				component.TriggerByCancel = true;
				Global.Data.TriggerByCancel = true;
			}
			if (this.animClipsStart != null && this._animation != null)
			{
				this._animation.CrossFade(this.animClipsStart.name);
			}
			if (this.hideObjects != null)
			{
				foreach (GameObject gameObject2 in this.hideObjects)
				{
					gameObject2.SetActive(false);
				}
			}
			if (this.particles != null && !this.particles.isPlaying)
			{
				this.particles.Play();
			}
			if (this.hideObjectNames != null)
			{
				foreach (string text in this.hideObjectNames)
				{
					foreach (GameObject gameObject3 in Object.FindObjectsOfType(typeof(GameObject)))
					{
						if (gameObject3.name == text)
						{
							gameObject3.SetActive(false);
							this.hideObjsByNameList.Add(gameObject3);
						}
					}
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.name.StartsWith("Leader"))
		{
			GameObject gameObject = U3DUtils.FindGameObjectByName(Camera.main.gameObject, "XiaXue");
			if (null != gameObject)
			{
				gameObject.SetActive(true);
			}
			LeaderInfo component = other.GetComponent<LeaderInfo>();
			if (null != component)
			{
				component.TriggerByCancel = false;
				Global.Data.TriggerByCancel = false;
			}
			if (this.animClipsEnd != null && this._animation != null)
			{
				this._animation.CrossFade(this.animClipsEnd.name);
			}
			if (this.hideObjects != null)
			{
				foreach (GameObject gameObject2 in this.hideObjects)
				{
					gameObject2.SetActive(true);
				}
			}
			if (this.particles != null && this.particles.isPlaying)
			{
				this.particles.Stop();
			}
			if (this.hideObjsByNameList.Count > 0)
			{
				foreach (GameObject gameObject3 in this.hideObjsByNameList)
				{
					if (null != gameObject3)
					{
						gameObject3.SetActive(true);
					}
				}
			}
		}
	}

	public AnimationClip animClipsStart;

	public AnimationClip animClipsStay;

	public AnimationClip animClipsEnd;

	public GameObject[] hideObjects;

	public string[] hideObjectNames;

	public ParticleSystem particles;

	private List<GameObject> hideObjsByNameList = new List<GameObject>();

	private Animation _animation;
}
