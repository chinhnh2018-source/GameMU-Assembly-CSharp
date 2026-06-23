using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class EmblemCoolDownItem
	{
		public EmblemCoolDownItem()
		{
			try
			{
				this.ID = (int)ConfigSystemParam.GetSystemParamIntByName("EmblemProp");
				if (Global.SetEmbelemUpData(-1) != null)
				{
					this.CDTicks = (long)Global.SetEmbelemUpData(-1).CDTime;
					this.ContinuedTicks = (long)Global.SetEmbelemUpData(-1).DurationTime;
				}
				else
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EmblemTime", ',');
					this.CDTicks = (long)(systemParamIntArrayByName[1] * 1000);
					this.ContinuedTicks = (long)(systemParamIntArrayByName[0] * 1000);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogError<string>(new string[]
				{
					ex.Message
				});
			}
		}

		public bool IsEmblemInCool()
		{
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			return fangZhiJiaSuTime <= this.StartTicks + this.CDTicks + this.ContinuedTicks;
		}

		public void ChangeRoleEmblemItem(long StartTime = -1L, long BufferSecs = -1L)
		{
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			if (this.StartTicks != StartTime)
			{
				this.StartTicks = StartTime;
			}
			if (this.ContinuedTicks != BufferSecs)
			{
				this.ContinuedTicks = BufferSecs;
			}
		}

		public long GetContinuedTicks()
		{
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			if (fangZhiJiaSuTime >= this.StartTicks + this.ContinuedTicks)
			{
				return 0L;
			}
			return this.ContinuedTicks - (fangZhiJiaSuTime - this.StartTicks);
		}

		public long GetCDTicks()
		{
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			long num = fangZhiJiaSuTime - this.StartTicks;
			if (num - this.ContinuedTicks < 0L)
			{
				long result = 0L;
				try
				{
					if (Global.SetEmbelemUpData(-1) != null)
					{
						result = (long)Global.SetEmbelemUpData(-1).CDTime;
					}
					else
					{
						result = (long)(ConfigSystemParam.GetSystemParamIntArrayByName("EmblemTime", ',')[1] * 1000);
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
				return result;
			}
			if (num - this.ContinuedTicks - this.CDTicks >= 0L)
			{
				return 0L;
			}
			return this.CDTicks - (num - this.ContinuedTicks);
		}

		public void SetData(int emblemID, long startTicks, long cDTicks, long continuedTicks, byte cdResetCount = 0)
		{
			this.ID = emblemID;
			this.StartTicks = startTicks;
			this.CDTicks = cDTicks;
			this.ContinuedTicks = continuedTicks;
			this.CdResetCount = cdResetCount;
		}

		public int ID { get; set; }

		public long StartTicks { get; set; }

		public long CDTicks { get; set; }

		public long ContinuedTicks { get; set; }

		public byte HaveLoadDecoration { get; set; }

		public byte CdResetCount { get; set; }
	}
}
