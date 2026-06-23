using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class ObjectItem
	{
		public int ID;

		public Type Type;

		public ISystemHelpPart Target;

		public int Value;

		public bool Enabled;

		public bool Actived;

		public bool Clicked;

		public bool Destoryed;

		public bool ShowHelp;

		public long LastTicks;

		public bool Changed;
	}
}
