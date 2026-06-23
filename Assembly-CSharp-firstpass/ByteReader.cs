using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
	public ByteReader(byte[] bytes)
	{
		this.mBuffer = bytes;
	}

	public ByteReader(TextAsset asset)
	{
		this.mBuffer = asset.bytes;
	}

	public bool canRead
	{
		get
		{
			return this.mBuffer != null && this.mOffset < this.mBuffer.Length;
		}
	}

	private static string ReadLine(byte[] buffer, int start, int count)
	{
		return Encoding.UTF8.GetString(buffer, start, count);
	}

	public string ReadLine()
	{
		int num = this.mBuffer.Length;
		while (this.mOffset < num && this.mBuffer[this.mOffset] < 32)
		{
			this.mOffset++;
		}
		int i = this.mOffset;
		if (i < num)
		{
			while (i < num)
			{
				int num2 = (int)this.mBuffer[i++];
				if (num2 == 10 || num2 == 13)
				{
					IL_81:
					string result = ByteReader.ReadLine(this.mBuffer, this.mOffset, i - this.mOffset - 1);
					this.mOffset = i;
					return result;
				}
			}
			i++;
			goto IL_81;
		}
		this.mOffset = num;
		return null;
	}

	public Dictionary<string, string> ReadDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		char[] array = new char[]
		{
			'='
		};
		while (this.canRead)
		{
			string text = this.ReadLine();
			if (text == null)
			{
				break;
			}
			if (!text.StartsWith("//"))
			{
				string[] array2 = text.Split(array, 2, 1);
				if (array2.Length == 2)
				{
					string text2 = array2[0].Trim();
					string text3 = array2[1].Trim().Replace("\\n", "\n");
					dictionary[text2] = text3;
				}
			}
		}
		return dictionary;
	}

	private byte[] mBuffer;

	private int mOffset;
}
