using System;

namespace HTMLEngine.Core
{
	internal abstract class HtmlChunk : PoolableObject
	{
		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
		}
	}
}
