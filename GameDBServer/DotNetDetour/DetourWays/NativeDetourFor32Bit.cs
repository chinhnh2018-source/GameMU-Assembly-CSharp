using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetDetour.DetourWays
{
	public class NativeDetourFor32Bit : IDetour
	{
		public unsafe virtual void Patch(MethodInfo src, MethodInfo dest, MethodInfo ori)
		{
			RuntimeTypeHandle[] instantiation = (from t in src.DeclaringType.GetGenericArguments()
			select t.TypeHandle).ToArray<RuntimeTypeHandle>();
			RuntimeHelpers.PrepareMethod(src.MethodHandle, instantiation);
			this.srcPtr = (byte*)src.MethodHandle.GetFunctionPointer().ToPointer();
			byte* ptr = (byte*)dest.MethodHandle.GetFunctionPointer().ToPointer();
			if (ori != null)
			{
				this.CreateOriginalMethod(ori);
			}
			fixed (byte* ptr2 = this.newInstrs)
			{
				*(int*)((byte*)ptr2 + 1) = (int)(ptr - this.srcPtr - 5U);
			}
			this.Patch();
		}

		protected unsafe virtual void Patch()
		{
			uint num;
			NativeAPI.VirtualProtect((IntPtr)((void*)this.srcPtr), 5U, Protection.PAGE_EXECUTE_READWRITE, out num);
			for (int i = 0; i < this.newInstrs.Length; i++)
			{
				this.srcPtr[i] = this.newInstrs[i];
			}
		}

		protected unsafe virtual void CreateOriginalMethod(MethodInfo method)
		{
			uint num = LDasm.SizeofMin5Byte((void*)this.srcPtr);
			int num2 = (int)(num + 5U);
			byte[] array = new byte[num2];
			IntPtr intPtr = Marshal.AllocHGlobal(num2);
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num))
			{
				array[num3] = this.srcPtr[num3];
				num3++;
			}
			array[(int)((UIntPtr)num)] = 233;
			fixed (byte* ptr = &array[(int)((UIntPtr)(num + 1U))])
			{
				*(int*)ptr = (int)(this.srcPtr - (uint)((int)intPtr) - 5U);
			}
			Marshal.Copy(array, 0, intPtr, num2);
			uint num4;
			NativeAPI.VirtualProtect(intPtr, (uint)num2, Protection.PAGE_EXECUTE_READWRITE, out num4);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
			*(int*)((byte*)method.MethodHandle.Value.ToPointer() + 8) = (int)intPtr;
		}

		protected byte[] newInstrs = new byte[]
		{
			233,
			144,
			144,
			144,
			144
		};

		protected unsafe byte* srcPtr;
	}
}
