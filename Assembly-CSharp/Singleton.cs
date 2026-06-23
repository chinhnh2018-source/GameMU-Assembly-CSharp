using System;

public class Singleton<T>
{
	protected Singleton()
	{
	}

	public static T Instance
	{
		get
		{
			return Singleton<T>.mInstance;
		}
	}

	protected static readonly T mInstance = Activator.CreateInstance<T>();
}
