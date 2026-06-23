using System;
using UnityEngine;

public class TeleportData : MonoBehaviour
{
	public int Key
	{
		get
		{
			return this._Key;
		}
		set
		{
			this._Key = value;
		}
	}

	public int To
	{
		get
		{
			return this._To;
		}
		set
		{
			this._To = value;
		}
	}

	public float ToX
	{
		get
		{
			return this._ToX;
		}
		set
		{
			this._ToX = value;
		}
	}

	public float ToY
	{
		get
		{
			return this._ToY;
		}
		set
		{
			this._ToY = value;
		}
	}

	public int ToDirection
	{
		get
		{
			return this._ToDirection;
		}
		set
		{
			this._ToDirection = value;
		}
	}

	public static int TotalObjectCount;

	public static int ShowObjectCount;

	private int _Key;

	private int _To;

	private float _ToX;

	private float _ToY;

	private int _ToDirection;
}
