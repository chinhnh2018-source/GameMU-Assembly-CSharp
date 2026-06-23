using System;
using UnityEngine;

namespace Xft
{
	public class RenderObject
	{
		public virtual void Initialize()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Reset()
		{
		}

		public virtual void SetEmitPosition(Vector3 pos)
		{
		}

		public EffectNode Node;
	}
}
