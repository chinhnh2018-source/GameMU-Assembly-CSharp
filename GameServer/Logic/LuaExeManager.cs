using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using Microsoft.CSharp.RuntimeBinder;
using Neo.IronLua;

namespace GameServer.Logic
{
	public class LuaExeManager
	{
		public static LuaExeManager getInstance()
		{
			return LuaExeManager.instance;
		}

		public void InitLuaEnv()
		{
			this.gEnv = this.lua.CreateEnvironment();
			LuaExeManager.timerCheckDict = new Timer(100000.0);
			LuaExeManager.timerCheckDict.Elapsed += this.CheckDictLuaInfo;
			LuaExeManager.timerCheckDict.Interval = 100000.0;
			LuaExeManager.timerCheckDict.Enabled = true;
		}

		private void CheckDictLuaInfo(object source, ElapsedEventArgs e)
		{
			lock (this.dictLuaCache)
			{
				using (Dictionary<string, LuaExeInfo>.Enumerator enumerator = this.dictLuaCache.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, LuaExeInfo> kvLuaInfo = enumerator.Current;
						KeyValuePair<string, LuaExeInfo> kvLuaInfo3 = kvLuaInfo;
						DateTime lastWriteTime = File.GetLastWriteTime(kvLuaInfo3.Key);
						DateTime t = lastWriteTime;
						kvLuaInfo3 = kvLuaInfo;
						if (t > kvLuaInfo3.Value.dateLastWrite)
						{
							Func<string> func = delegate()
							{
								KeyValuePair<string, LuaExeInfo> kvLuaInfo2 = kvLuaInfo;
								return File.ReadAllText(kvLuaInfo2.Key);
							};
							kvLuaInfo3 = kvLuaInfo;
							string fileName = Path.GetFileName(kvLuaInfo3.Key);
							LuaChunk luaChunk = this.lua.CompileChunk(func(), fileName, false, new KeyValuePair<string, Type>[0]);
							kvLuaInfo3 = kvLuaInfo;
							kvLuaInfo3.Value.dateLastWrite = lastWriteTime;
							kvLuaInfo3 = kvLuaInfo;
							kvLuaInfo3.Value.luaChunk = luaChunk;
						}
					}
				}
			}
		}

		public LuaGlobal ExeLua(string strLuaPath)
		{
			LuaExeInfo luaExeInfo = null;
			string strFullPath = Path.GetFullPath(strLuaPath);
			lock (this.dictLuaCache)
			{
				if (!this.dictLuaCache.TryGetValue(strFullPath, out luaExeInfo))
				{
					Func<string> func = () => File.ReadAllText(strFullPath);
					string fileName = Path.GetFileName(strFullPath);
					LuaChunk luaChunk = this.lua.CompileChunk(func(), fileName, false, new KeyValuePair<string, Type>[0]);
					luaExeInfo = new LuaExeInfo();
					luaExeInfo.dateLastWrite = File.GetLastWriteTime(strFullPath);
					luaExeInfo.luaChunk = luaChunk;
					this.dictLuaCache.Add(strFullPath, luaExeInfo);
				}
				this.gEnv.DoChunk(luaExeInfo.luaChunk, new object[0]);
			}
			return this.gEnv;
		}

		public LuaResult ExecLuaFunction(LuaManager luaManager, LuaGlobal g, string strLuaFunction, GameClient client)
		{
			LuaResult result;
			lock (this.dictLuaCache)
			{
				if (LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitec == null)
				{
					LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitec = CallSite<Func<CallSite, object, LuaResult>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof(LuaResult), typeof(LuaExeManager)));
				}
				Func<CallSite, object, LuaResult> target = LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitec.Target;
				CallSite <>p__Sitec = LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitec;
				if (LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sited == null)
				{
					LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sited = CallSite<Func<CallSite, object, LuaManager, GameClient, object, object>>.Create(Binder.Invoke(CSharpBinderFlags.None, typeof(LuaExeManager), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null)
					}));
				}
				Func<CallSite, object, LuaManager, GameClient, object, object> target2 = LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sited.Target;
				CallSite <>p__Sited = LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sited;
				if (LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitee == null)
				{
					LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitee = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof(LuaExeManager), new CSharpArgumentInfo[]
					{
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
						CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
					}));
				}
				LuaResult luaResult = target(<>p__Sitec, target2(<>p__Sited, LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitee.Target(LuaExeManager.<ExecLuaFunction>o__SiteContainerb.<>p__Sitee, g, strLuaFunction), GameManager.LuaMgr, client, null));
				result = luaResult;
			}
			return result;
		}

		private Dictionary<string, LuaExeInfo> dictLuaCache = new Dictionary<string, LuaExeInfo>();

		private Lua lua = new Lua();

		private LuaGlobal gEnv = null;

		private static Timer timerCheckDict;

		private static LuaExeManager instance = new LuaExeManager();

		[CompilerGenerated]
		private static class <ExecLuaFunction>o__SiteContainerb
		{
			public static CallSite<Func<CallSite, object, LuaResult>> <>p__Sitec;

			public static CallSite<Func<CallSite, object, LuaManager, GameClient, object, object>> <>p__Sited;

			public static CallSite<Func<CallSite, object, string, object>> <>p__Sitee;
		}
	}
}
