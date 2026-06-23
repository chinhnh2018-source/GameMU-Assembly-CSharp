using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Interface;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class Canvas : SpriteSL
	{
		public static void SetLeft(IObject obj, double value)
		{
			obj.cx = (int)value;
		}

		public static void SetLeft(IObject obj, int value)
		{
			obj.cx = value;
		}

		public static void SetLeft(object obj, int value)
		{
		}

		public static void SetTop(object obj, int value)
		{
		}

		public static void SetTop(IObject obj, double value)
		{
			obj.cy = (int)value;
		}

		public static void SetTop(IObject obj, int value)
		{
			obj.cy = value;
		}

		public static double GetLeft(IObject obj)
		{
			return (double)obj.cx;
		}

		public static double GetTop(IObject obj)
		{
			return (double)obj.cy;
		}

		public static double GetZIndex(IObject obj)
		{
			return 0.0;
		}

		public static void SetZIndex(IObject obj, double value)
		{
		}

		public static void SetLeft(MonoBehaviour obj, double value)
		{
		}

		public static void SetLeft(MonoBehaviour obj, int value)
		{
		}

		public static void SetTop(MonoBehaviour obj, double value)
		{
		}

		public static void SetTop(MonoBehaviour obj, int value)
		{
		}

		public static double GetLeft(MonoBehaviour obj)
		{
			return 0.0;
		}

		public static double GetTop(MonoBehaviour obj)
		{
			return 0.0;
		}

		public static double GetZIndex(MonoBehaviour obj)
		{
			return 0.0;
		}

		public static void SetZIndex(MonoBehaviour obj, double value)
		{
		}

		public double alpha { get; set; }

		public object mask { get; set; }

		public double Opacity { get; set; }

		public bool buttonMode { get; set; }

		public ImageURL BackgroundURL { get; set; }

		public StageSL MainStage { get; set; }

		public string HorizontalAlignment { get; set; }

		public Point globalToLocal(Point globalPoint)
		{
			Point result = new Point(globalPoint.X, globalPoint.Y);
			return result;
		}
	}
}
