using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public class ServerEvents
	{
		public EventLevels EventLevel { get; set; }

		public string EventRootPath
		{
			get
			{
				return this._EventRootPath;
			}
			set
			{
				this._EventRootPath = value;
			}
		}

		public string EventPreFileName
		{
			get
			{
				return this._EventPreFileName;
			}
			set
			{
				this._EventPreFileName = value;
			}
		}

		private string FormatNowTimeString()
		{
			return TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss");
		}

		public void AddEvent(string msg, EventLevels eventLevel)
		{
			if (eventLevel >= this.EventLevel)
			{
				string item = string.Format("{0}\t{1}", this.FormatNowTimeString(), msg);
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(item);
				}
			}
		}

		public void AddImporEvent(params object[] list)
		{
			if (EventLevels.Important >= this.EventLevel)
			{
				string text = string.Format("{0}", this.FormatNowTimeString());
				for (int i = 0; i < list.Length; i++)
				{
					text += string.Format("\t{0}", list[i]);
				}
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(text);
				}
			}
		}

		public string EventPath
		{
			get
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this._PathLock)
				{
					string text = dateTime.ToString("yyyy");
					string text2 = dateTime.ToString("MM");
					string text3 = dateTime.ToString("dd");
					if (this._EventPath == string.Empty || text != this._YearID || text2 != this._MonthID || text3 != this._DayID)
					{
						this._YearID = text;
						this._MonthID = text2;
						this._DayID = text3;
						this._EventPath = DataHelper.CurrentDirectory + string.Format("{0}/", this._EventRootPath);
						try
						{
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}Year_{1}/", this._EventPath, this._YearID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}Month_{1}/", this._EventPath, this._MonthID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}{1}/", this._EventPath, this._DayID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
					}
				}
				return this._EventPath;
			}
		}

		public bool WriteEvent()
		{
			string text = null;
			lock (this.EventsQueue)
			{
				if (this.EventsQueue.Count > 0)
				{
					text = this.EventsQueue.Dequeue();
				}
			}
			bool result;
			if (null == text)
			{
				result = false;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				try
				{
					if (this._DayOfYearID != dateTime.DayOfYear || this._StreamWriter == null)
					{
						string path = string.Format("{0}{1}.log", this.EventPath, this._EventPreFileName);
						if (null != this._StreamWriter)
						{
							this._StreamWriter.Flush();
							this._StreamWriter.Close();
							this._StreamWriter.Dispose();
							this._StreamWriter = null;
						}
						this._StreamWriter = File.AppendText(path);
						this._DayOfYearID = dateTime.DayOfYear;
						this._StreamWriter.AutoFlush = true;
					}
					this._StreamWriter.WriteLine(text);
				}
				catch
				{
					this._HourID = -1;
				}
				result = true;
			}
			return result;
		}

		private Queue<string> EventsQueue = new Queue<string>();

		private string _EventRootPath = "events";

		private string _EventPreFileName = "Event";

		private string _EventPath = string.Empty;

		private string _YearID = string.Empty;

		private string _MonthID = string.Empty;

		private string _DayID = string.Empty;

		private object _PathLock = new object();

		private int _DayOfYearID = -1;

		private int _HourID = -1;

		private StreamWriter _StreamWriter = null;
	}
}
