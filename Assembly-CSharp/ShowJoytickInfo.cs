using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShowJoytickInfo : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (null != Global.Joystick && null != this.label)
		{
			double angle = Global.GetAngle((double)((int)(Global.Joystick.position.x * 100f)), (double)((int)(Global.Joystick.position.y * 100f)));
			this.label.text = string.Format("joystick.position={0}, angle={1}", Global.Joystick.position, angle);
		}
	}

	public UILabel label;
}
