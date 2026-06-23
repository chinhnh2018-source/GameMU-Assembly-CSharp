using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	[Serializable]
	public class FOVKick
	{
		public void Setup(Camera camera)
		{
			this.CheckStatus(camera);
			this.Camera = camera;
			this.originalFov = camera.fieldOfView;
		}

		private void CheckStatus(Camera camera)
		{
			if (camera == null)
			{
				throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
			}
			if (this.IncreaseCurve == null)
			{
				throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
			}
		}

		public void ChangeCamera(Camera camera)
		{
			this.Camera = camera;
		}

		public IEnumerator FOVKickUp()
		{
			float t = Mathf.Abs((this.Camera.fieldOfView - this.originalFov) / this.FOVIncrease);
			while (t < this.TimeToIncrease)
			{
				this.Camera.fieldOfView = this.originalFov + this.IncreaseCurve.Evaluate(t / this.TimeToIncrease) * this.FOVIncrease;
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			yield break;
		}

		public IEnumerator FOVKickDown()
		{
			float t = Mathf.Abs((this.Camera.fieldOfView - this.originalFov) / this.FOVIncrease);
			while (t > 0f)
			{
				this.Camera.fieldOfView = this.originalFov + this.IncreaseCurve.Evaluate(t / this.TimeToDecrease) * this.FOVIncrease;
				t -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			this.Camera.fieldOfView = this.originalFov;
			yield break;
		}

		public Camera Camera;

		[HideInInspector]
		public float originalFov;

		public float FOVIncrease = 3f;

		public float TimeToIncrease = 1f;

		public float TimeToDecrease = 1f;

		public AnimationCurve IncreaseCurve;
	}
}
