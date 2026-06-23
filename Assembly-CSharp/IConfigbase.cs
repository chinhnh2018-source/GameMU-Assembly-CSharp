using System;

public class IConfigbase<T> where T : ConfigBase, new()
{
	public static T Instance
	{
		get
		{
			if (IConfigbase<T>.mInstance == null)
			{
				IConfigbase<T>.mInstance = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			}
			return IConfigbase<T>.mInstance;
		}
	}

	public void IDisposeInstance()
	{
		IConfigbase<T>.mInstance = default(T);
	}

	protected static T mInstance;
}
