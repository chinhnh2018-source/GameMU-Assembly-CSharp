using System;
using System.IO;
using UnityEngine;

namespace UniLua
{
	public class LuaFile
	{
		public static void SetPathHook(PathHook hook)
		{
			LuaFile.pathhook = hook;
		}

		public static FileLoadInfo OpenFile(string filename)
		{
			string text = LuaFile.pathhook(filename);
			return new FileLoadInfo(File.Open(text, 3, 1, 3));
		}

		public static bool Readable(string filename)
		{
			string text = LuaFile.pathhook(filename);
			bool result;
			try
			{
				using (File.Open(text, 3, 1, 3))
				{
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static PathHook pathhook = (string s) => Path.Combine(Path.Combine(Application.streamingAssetsPath, "LuaRoot"), s);
	}
}
