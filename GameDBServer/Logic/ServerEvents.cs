using System;
using System.Collections.Generic;
using System.IO;
using GameDBServer.Core;

namespace GameDBServer.Logic
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

		public void AddEvent(string msg, EventLevels eventLevel)
		{
			if (eventLevel >= this.EventLevel)
			{
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(msg);
				}
			}
		}

		public string EventPath
		{
			get
			{
				DateTime now = DateTime.Now;
				lock (this._PathLock)
				{
					string text = now.ToString("yyyy");
					string text2 = now.ToString("MM");
					string text3 = now.ToString("dd");
					if (this._EventPath == string.Empty || text != this._YearID || text2 != this._MonthID || text3 != this._DayID)
					{
						this._YearID = text;
						this._MonthID = text2;
						this._DayID = text3;
						this._EventPath = AppDomain.CurrentDomain.BaseDirectory + string.Format("{0}/", this._EventRootPath);
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
				string value = string.Format("{0}{1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss  "), text);
				try
				{
					if (this._HourID != dateTime.Hour || this._StreamWriter == null)
					{
						string path = string.Format("{0}{1}_{2}.log", this.EventPath, this._EventPreFileName, dateTime.ToString("HH"));
						if (null != this._StreamWriter)
						{
							this._StreamWriter.Flush();
							this._StreamWriter.Close();
							this._StreamWriter.Dispose();
							this._StreamWriter = null;
						}
						this._StreamWriter = File.AppendText(path);
						this._HourID = dateTime.Hour;
						this._StreamWriter.AutoFlush = true;
					}
					this._StreamWriter.WriteLine(value);
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

		private int _HourID = -1;

		private StreamWriter _StreamWriter = null;
	}
}
