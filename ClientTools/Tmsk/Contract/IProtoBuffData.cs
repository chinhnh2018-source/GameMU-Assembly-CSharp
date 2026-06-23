using System;

namespace Tmsk.Contract
{
	public interface IProtoBuffData
	{
		int fromBytes(byte[] data, int offset, int count);

		byte[] toBytes();
	}
}
