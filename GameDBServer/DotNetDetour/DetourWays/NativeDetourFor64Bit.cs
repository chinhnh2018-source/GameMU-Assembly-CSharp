using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetDetour.DetourWays
{
	public class NativeDetourFor64Bit : NativeDetourFor32Bit
	{
		protected unsafe override void CreateOriginalMethod(MethodInfo method)
		{
			uint num = LDasm.SizeofMin5Byte((void*)this.srcPtr);
			byte[] array = new byte[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = this.srcPtr[num2];
				num2++;
			}
			fixed (byte* ptr = &this.jmp_inst[3])
			{
				*(long*)ptr = this.srcPtr + num;
			}
			int num3 = array.Length + this.jmp_inst.Length;
			IntPtr intPtr = Marshal.AllocHGlobal(num3);
			Marshal.Copy(array, 0, intPtr, array.Length);
			Marshal.Copy(this.jmp_inst, 0, intPtr + array.Length, this.jmp_inst.Length);
			uint num4;
			NativeAPI.VirtualProtect(intPtr, (uint)num3, Protection.PAGE_EXECUTE_READWRITE, out num4);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
			*(long*)((byte*)method.MethodHandle.Value.ToPointer() + 8) = (long)intPtr;
		}

		private byte[] jmp_inst = new byte[]
		{
			80,
			72,
			184,
			144,
			144,
			144,
			144,
			144,
			144,
			144,
			144,
			80,
			72,
			139,
			68,
			36,
			8,
			194,
			8,
			0
		};
	}
}
