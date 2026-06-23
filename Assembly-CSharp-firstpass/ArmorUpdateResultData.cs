using System;
using ProtoBuf;

[ProtoContract]
public class ArmorUpdateResultData
{
	public byte[] toBytes()
	{
		int num = 0;
		num += ProtoUtil.GetIntSize(this.Type, true, 1, true, 0);
		num += ProtoUtil.GetIntSize(this.Armor, true, 2, true, 0);
		num += ProtoUtil.GetIntSize(this.Exp, true, 3, true, 0);
		num += ProtoUtil.GetIntSize(this.Auto, true, 4, true, 0);
		num += ProtoUtil.GetIntSize(this.ZuanShi, true, 5, true, 0);
		num += ProtoUtil.GetIntSize(this.Result, true, 6, true, 0);
		byte[] array = new byte[num];
		int num2 = 0;
		ProtoUtil.IntMemberToBytes(array, 1, ref num2, this.Type, true, 0);
		ProtoUtil.IntMemberToBytes(array, 2, ref num2, this.Armor, true, 0);
		ProtoUtil.IntMemberToBytes(array, 3, ref num2, this.Exp, true, 0);
		ProtoUtil.IntMemberToBytes(array, 4, ref num2, this.Auto, true, 0);
		ProtoUtil.IntMemberToBytes(array, 5, ref num2, this.ZuanShi, true, 0);
		ProtoUtil.IntMemberToBytes(array, 6, ref num2, this.Result, true, 0);
		return array;
	}

	[ProtoMember(1)]
	public int Type;

	[ProtoMember(2)]
	public int Armor;

	[ProtoMember(3)]
	public int Exp;

	[ProtoMember(4)]
	public int Auto;

	[ProtoMember(5)]
	public int ZuanShi;

	[ProtoMember(6)]
	public int Result;
}
