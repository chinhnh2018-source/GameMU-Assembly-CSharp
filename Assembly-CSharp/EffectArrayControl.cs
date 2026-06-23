using System;
using UnityEngine;

public class EffectArrayControl : MonoBehaviour
{
	private void Awake()
	{
		if (this._EffectParticleArray != null && 0 < this._EffectParticleArray.Length)
		{
			int i = 0;
			int num = this._EffectParticleArray.Length;
			while (i < num)
			{
				ParticleSystem component = this._EffectParticleArray[i].GetComponent<ParticleSystem>();
				if (null != component && i < this._SceneEffectStartSize.Length)
				{
					component.startSize = this._SceneEffectStartSize[i];
				}
				i++;
			}
		}
	}

	public void ShowEffect(bool bShow)
	{
		if (this._EffectMeshArray != null && 0 < this._EffectMeshArray.Length)
		{
			int i = 0;
			int num = this._EffectMeshArray.Length;
			while (i < num)
			{
				if (null != this._EffectMeshArray[i])
				{
					this._EffectMeshArray[i].SetActive(bShow);
				}
				i++;
			}
		}
	}

	public void ShowtParticle(bool bShow, bool UI = false)
	{
		if (this._EffectParticleArray != null && 0 < this._EffectParticleArray.Length)
		{
			int i = 0;
			int num = this._EffectParticleArray.Length;
			while (i < num)
			{
				if (null != this._EffectParticleArray[i])
				{
					this._EffectParticleArray[i].SetActive(bShow);
					if (bShow)
					{
						if (!UI)
						{
							ParticleSystem component = this._EffectParticleArray[i].GetComponent<ParticleSystem>();
							if (null != component)
							{
							}
						}
						else
						{
							ParticleSystem component2 = this._EffectParticleArray[i].GetComponent<ParticleSystem>();
							if (null != component2)
							{
								component2.startSize = this._UIEffectStartSize * component2.startSize;
							}
						}
					}
				}
				i++;
			}
		}
	}

	public GameObject[] _EffectParticleArray;

	public float[] _SceneEffectStartSize;

	public float _UIEffectStartSize = 1f;

	public GameObject[] _EffectMeshArray;
}
