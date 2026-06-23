using System;

namespace UniLua
{
	public struct NameFuncPair
	{
		public NameFuncPair(string name, CSharpFunctionDelegate func)
		{
			this.Name = name;
			this.Func = func;
		}

		public string Name;

		public CSharpFunctionDelegate Func;
	}
}
