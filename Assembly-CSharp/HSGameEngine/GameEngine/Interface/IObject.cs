using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

namespace HSGameEngine.GameEngine.Interface
{
	public interface IObject
	{
		string Name { get; set; }

		bool InitStatus { get; }

		object Tag { get; set; }

		GameObject The3DGameObject { get; }

		IObject Root { get; }

		IObject Children { get; }

		Transform Parent { get; set; }

		Point OrigCoordinate { get; set; }

		Point Coordinate { get; set; }

		int cx { get; set; }

		int cy { get; set; }

		int CenterX { get; set; }

		int CenterY { get; set; }

		RenderTypes RenderType { get; set; }

		void Start();

		bool ShowObject();

		bool CurrentObjectState { get; }

		bool HideObject();

		void Destroy();

		IObject FindName(string name);

		void Add(IObject obj);

		void OnFrameRender();

		GSpriteTypes SpriteType { get; set; }

		MonsterTypes MonsterType { get; set; }

		FakeRoleTypes FakeRoleType { get; set; }

		GActions Action { get; set; }

		int Direction { get; set; }

		Quaternion Rotation { get; set; }

		double MoveSpeed { get; set; }
	}
}
