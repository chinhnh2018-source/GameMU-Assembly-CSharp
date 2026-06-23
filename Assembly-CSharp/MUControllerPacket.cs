using System;

public class MUControllerPacket
{
	public MUControllerPacket Copy()
	{
		return (MUControllerPacket)base.MemberwiseClone();
	}

	public float leftStickX;

	public float leftStickY;

	public float rightStickX;

	public float rightStickY;

	public bool leftAsDown;

	public bool rightAsDown;

	public bool butX;

	public bool butY;

	public bool butA;

	public bool butB;

	public bool up;

	public bool down;

	public bool left;

	public bool right;

	public bool leftTop;

	public float leftTrigger;

	public bool rightTop;

	public float rightTrigger;

	public bool pause;
}
