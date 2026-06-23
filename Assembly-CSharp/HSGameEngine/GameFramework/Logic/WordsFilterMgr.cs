using System;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameFramework.Logic
{
	public class WordsFilterMgr
	{
		public static void ExecWordsFilter(string content, CheckWordsResultEventHandler func)
		{
			if ("TengXun" != Context.PingTaiName)
			{
				if (func != null)
				{
					int num = (!Global.IncludeReplaceFilterFileds(content)) ? 0 : 1;
					string msg = content;
					if (num > 0)
					{
						msg = Global.ReplaceFilterFileds(content);
					}
					ExecWordsFilterEventArgs args = new ExecWordsFilterEventArgs
					{
						ret = 0,
						is_lost = 0,
						is_dirty = num,
						msg = msg
					};
					func(content, args);
				}
				return;
			}
			new TengXunWordsFilter
			{
				TengXunReturn = delegate(object s, ExecWordsFilterEventArgs e)
				{
					if (func != null)
					{
						func(content, e);
					}
				}
			}.SendRequest(content, string.Empty, string.Empty);
		}
	}
}
