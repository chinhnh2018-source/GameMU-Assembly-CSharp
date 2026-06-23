using System;

namespace UniLua
{
	public class StkId
	{
		public int Index { get; private set; }

		public void SetList(StkId[] list)
		{
			this.List = list;
		}

		public void SetIndex(int index)
		{
			this.Index = index;
		}

		public static StkId inc(ref StkId val)
		{
			StkId result = val;
			val = val.List[val.Index + 1];
			return result;
		}

		public override string ToString()
		{
			string text;
			if (this.V.TtIsString())
			{
				text = this.V.SValue().Replace("\n", "»");
			}
			else
			{
				text = "...";
			}
			return string.Format("StkId - {0} - {1}", LuaState.TypeName((LuaType)this.V.Tt), text);
		}

		public TValue V;

		private StkId[] List;
	}
}
