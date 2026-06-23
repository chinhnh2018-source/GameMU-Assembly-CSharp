using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public interface ISilverLight
	{
		double Width { get; set; }

		double Height { get; set; }

		double ActualWidth { get; }

		double ActualHeight { get; }

		string Name { get; set; }

		bool Visibility { get; set; }

		object Tag { get; set; }

		bool IsHitTestVisible { get; set; }

		GameObject Parent { get; set; }

		double X { get; set; }

		double Y { get; set; }

		double Z { get; set; }

		Thickness Margin { get; set; }

		void UpdateLayout();
	}
}
