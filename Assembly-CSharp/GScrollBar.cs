using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class GScrollBar : MonoBehaviour, ISilverLight
{
	public float Min { get; set; }

	public float Max { get; set; }

	public float Percent { get; set; }

	public double Width { get; set; }

	public double Height { get; set; }

	public double ActualWidth
	{
		get
		{
			return 0.0;
		}
	}

	public double ActualHeight
	{
		get
		{
			return 0.0;
		}
	}

	public string Name { get; set; }

	public bool Visibility { get; set; }

	public object Tag { get; set; }

	public bool IsHitTestVisible { get; set; }

	public GameObject Parent { get; set; }

	public double X { get; set; }

	public double Y { get; set; }

	public double Z { get; set; }

	public Thickness Margin { get; set; }

	public void UpdateLayout()
	{
	}

	public UIScrollBar _UIScrollBar;

	public UILabel _UILabel;
}
