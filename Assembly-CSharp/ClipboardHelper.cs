using System;
using System.Reflection;
using UnityEngine;

public class ClipboardHelper
{
	private static PropertyInfo GetSystemCopyBufferProperty()
	{
		if (ClipboardHelper.m_systemCopyBufferProperty == null)
		{
			Type typeFromHandle = typeof(GUIUtility);
			ClipboardHelper.m_systemCopyBufferProperty = typeFromHandle.GetProperty("systemCopyBuffer", 40);
			if (ClipboardHelper.m_systemCopyBufferProperty == null)
			{
				throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
			}
		}
		return ClipboardHelper.m_systemCopyBufferProperty;
	}

	public static string clipBoard
	{
		get
		{
			return GUIUtility.systemCopyBuffer;
		}
		set
		{
			GUIUtility.systemCopyBuffer = value;
		}
	}

	private static PropertyInfo m_systemCopyBufferProperty;
}
