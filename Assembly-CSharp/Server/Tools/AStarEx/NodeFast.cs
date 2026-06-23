using System;
using System.Runtime.InteropServices;

namespace Server.Tools.AStarEx
{
	[StructLayout(0, Pack = 1)]
	public struct NodeFast
	{
		public float f;

		public float g;

		public float h;

		public int parentX;

		public int parentY;

		public byte ClosedStatus;
	}
}
