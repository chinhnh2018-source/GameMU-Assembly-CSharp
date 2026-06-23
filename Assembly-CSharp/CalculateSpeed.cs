using System;
using UnityEngine;

public class CalculateSpeed : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 10f, 50f, 30f), "Fire"))
		{
			this.Fire();
		}
	}

	private void Fire()
	{
		if (this.target != null)
		{
			float num = base.transform.eulerAngles.x;
			base.transform.rotation = Quaternion.LookRotation(this.target.transform.position - base.transform.position);
			num += base.transform.eulerAngles.x;
			base.transform.Rotate(330f, 0f, 0f);
			ParticleSystem component = base.transform.gameObject.GetComponent<ParticleSystem>();
			float num2 = 0.5235988f;
			float num3 = Mathf.Abs(Vector3.Distance(base.transform.position, this.target.transform.position));
			float num4 = (float)((double)num3 * 9.8 * (double)component.gravityModifier / (double)(2f * Mathf.Sin(num2) * Mathf.Cos(num2)));
			float startSpeed = Mathf.Sqrt(num4);
			component.startSpeed = startSpeed;
			component.Play();
		}
	}

	public GameObject target;
}
