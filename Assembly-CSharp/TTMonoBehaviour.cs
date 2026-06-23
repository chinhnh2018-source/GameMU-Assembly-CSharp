using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTMonoBehaviour : MonoBehaviour
{
	private TTMonoBehaviour.LockQueue LockedCoroutineQueue { get; set; }

	public TTMonoBehaviour.Coroutine<T> StartCoroutine<T>(IEnumerator coroutine)
	{
		TTMonoBehaviour.Coroutine<T> coroutine2 = new TTMonoBehaviour.Coroutine<T>();
		coroutine2.coroutine = base.StartCoroutine(coroutine2.InternalRoutine(coroutine));
		return coroutine2;
	}

	public TTMonoBehaviour.Coroutine<T> StartCoroutine<T>(IEnumerator coroutine, TTMonoBehaviour.CoroutineExceptionHandler exceptionHandler)
	{
		TTMonoBehaviour.Coroutine<T> coroutine2 = new TTMonoBehaviour.Coroutine<T>();
		coroutine2.coroutine = base.StartCoroutine(coroutine2.InternalRoutine(coroutine));
		coroutine2.coroutineException = exceptionHandler;
		return coroutine2;
	}

	public TTMonoBehaviour.Coroutine<T> StartCoroutine<T>(IEnumerator coroutine, string lockID, float waitTime = 10f)
	{
		if (this.LockedCoroutineQueue == null)
		{
			this.LockedCoroutineQueue = new TTMonoBehaviour.LockQueue();
		}
		TTMonoBehaviour.Coroutine<T> coroutine2 = new TTMonoBehaviour.Coroutine<T>(lockID, waitTime, this.LockedCoroutineQueue);
		coroutine2.coroutine = base.StartCoroutine(coroutine2.InternalRoutine(coroutine));
		return coroutine2;
	}

	public class Coroutine<T>
	{
		public Coroutine()
		{
			this.lockable = false;
		}

		public Coroutine(string lockID, float waitTime, TTMonoBehaviour.LockQueue lockedCoroutines)
		{
			this.lockable = true;
			this.lockID = lockID;
			this.lockedCoroutines = lockedCoroutines;
			this.waitTime = waitTime;
		}

		public T Value
		{
			get
			{
				if (this.e != null)
				{
					throw this.e;
				}
				return this.returnVal;
			}
		}

		public IEnumerator InternalRoutine(IEnumerator coroutine)
		{
			if (this.lockable && this.lockedCoroutines != null)
			{
				if (this.lockedCoroutines.Contains(this.lockID))
				{
					if (this.waitTime == 0f)
					{
						yield break;
					}
					float starttime = Time.time;
					float counter = 0f;
					this.lockedCoroutines.Add(this.lockID, coroutine);
					while (!this.lockedCoroutines.First(this.lockID, coroutine) && Time.time - starttime < this.waitTime)
					{
						yield return null;
						counter += Time.deltaTime;
					}
					if (counter >= this.waitTime)
					{
						string error = string.Concat(new object[]
						{
							base.GetType().Name,
							": coroutine ",
							this.lockID,
							" bailing! due to timeout: ",
							counter
						});
						MUDebug.LogError<string>(new string[]
						{
							error
						});
						this.e = new Exception(error);
						this.lockedCoroutines.Remove(this.lockID, coroutine);
						if (this.coroutineException != null)
						{
							this.coroutineException();
						}
						yield break;
					}
				}
				else
				{
					this.lockedCoroutines.Add(this.lockID, coroutine);
				}
			}
			object yielded;
			for (;;)
			{
				try
				{
					if (!coroutine.MoveNext())
					{
						if (this.lockable)
						{
							this.lockedCoroutines.Remove(this.lockID, coroutine);
						}
						yield break;
					}
				}
				catch (Exception ex)
				{
					Exception e = ex;
					this.e = e;
					MUDebug.LogException(e);
					if (this.lockable)
					{
						this.lockedCoroutines.Remove(this.lockID, coroutine);
					}
					if (this.coroutineException != null)
					{
						this.coroutineException();
					}
					yield break;
				}
				yielded = coroutine.Current;
				if (yielded != null && yielded.GetType() == typeof(T))
				{
					break;
				}
				yield return coroutine.Current;
			}
			this.returnVal = (T)((object)yielded);
			if (this.lockable)
			{
				this.lockedCoroutines.Remove(this.lockID, coroutine);
			}
			yield break;
			yield break;
		}

		private T returnVal;

		private Exception e;

		private string lockID;

		private float waitTime;

		private TTMonoBehaviour.LockQueue lockedCoroutines;

		private bool lockable;

		public Coroutine coroutine;

		public TTMonoBehaviour.CoroutineExceptionHandler coroutineException;
	}

	public class LockQueue
	{
		public LockQueue()
		{
			this.LockedCoroutines = new Dictionary<string, List<IEnumerator>>();
		}

		private Dictionary<string, List<IEnumerator>> LockedCoroutines { get; set; }

		public bool Contains(string lockID)
		{
			return this.LockedCoroutines.ContainsKey(lockID);
		}

		public bool First(string lockID, IEnumerator coroutine)
		{
			bool result = false;
			if (this.Contains(lockID) && this.LockedCoroutines[lockID].Count > 0)
			{
				result = (this.LockedCoroutines[lockID][0] == coroutine);
			}
			return result;
		}

		public void Add(string lockID, IEnumerator coroutine)
		{
			if (!this.LockedCoroutines.ContainsKey(lockID))
			{
				this.LockedCoroutines.Add(lockID, new List<IEnumerator>());
			}
			if (!this.LockedCoroutines[lockID].Contains(coroutine))
			{
				this.LockedCoroutines[lockID].Add(coroutine);
			}
		}

		public bool Remove(string lockID, IEnumerator coroutine)
		{
			bool result = false;
			if (this.LockedCoroutines.ContainsKey(lockID))
			{
				if (this.LockedCoroutines[lockID].Contains(coroutine))
				{
					result = this.LockedCoroutines[lockID].Remove(coroutine);
				}
				if (this.LockedCoroutines[lockID].Count == 0)
				{
					result = this.LockedCoroutines.Remove(lockID);
				}
			}
			return result;
		}
	}

	public delegate void CoroutineExceptionHandler();
}
