using System;
using UnityEngine;

public class MUJoystickController : ManualUpdateBehaviour
{
	public int PadCount()
	{
		return this.m_activePadCount;
	}

	public bool KnownPad(int id)
	{
		bool result = false;
		string text = this.m_activedPadNames[id];
		for (int i = 0; i < this.m_suppurtPadNames.Length; i++)
		{
			if (text.Contains(this.m_suppurtPadNames[i]))
			{
				result = true;
			}
		}
		return result;
	}

	private void Awake()
	{
		MUJoystickController.m_instance = this;
		this.m_padData = new MUControllerPacket[MUJoystickController.MAXCONTROLLERS];
		this.m_padDataOld = new MUControllerPacket[MUJoystickController.MAXCONTROLLERS];
		for (int i = 0; i < MUJoystickController.MAXCONTROLLERS; i++)
		{
			this.m_padData[i] = new MUControllerPacket();
			this.m_padDataOld[i] = new MUControllerPacket();
		}
	}

	public override void ManualUpdate()
	{
		if (!this.beOpen)
		{
			return;
		}
		for (int i = 0; i < MUJoystickController.MAXCONTROLLERS; i++)
		{
			this.m_padDataOld[i] = this.m_padData[i].Copy();
		}
		this.m_activedPadNames = Input.GetJoystickNames();
		if (MUJoystickController.bePCMockAllowed)
		{
			string[] array = new string[this.m_activedPadNames.Length + 1];
			for (int j = 0; j < this.m_activedPadNames.Length; j++)
			{
				array[j] = this.m_activedPadNames[j];
			}
			array[this.m_activedPadNames.Length] = this.m_suppurtPadNames[3];
			this.m_activedPadNames = array;
		}
		this.m_activePadCount = this.m_activedPadNames.Length;
		if (this.m_activePadCount >= MUJoystickController.MAXCONTROLLERS)
		{
			this.m_activePadCount = MUJoystickController.MAXCONTROLLERS;
		}
		for (int k = 0; k < this.m_activePadCount; k++)
		{
			if (this.m_activedPadNames[k].Contains(this.m_suppurtPadNames[0]))
			{
				this.ReadShieldPad(k);
			}
			if (this.m_activedPadNames[k].Contains(this.m_suppurtPadNames[1]))
			{
				this.ReadXbox360(k);
			}
			if (this.m_activedPadNames[k].Contains(this.m_suppurtPadNames[2]))
			{
				this.ReadXbox360(k);
			}
			if (this.m_activedPadNames[k].Contains(this.m_suppurtPadNames[3]))
			{
				this.ReadPCMock(k);
			}
			this.filterAnalogs(k);
		}
	}

	private void filterAnalogs(int id)
	{
		float num = 0.2f;
		if (Mathf.Abs(this.m_padData[id].leftStickX) < num)
		{
			this.m_padData[id].leftStickX = 0f;
		}
		if (Mathf.Abs(this.m_padData[id].leftStickY) < num)
		{
			this.m_padData[id].leftStickY = 0f;
		}
		if (Mathf.Abs(this.m_padData[id].rightStickX) < num)
		{
			this.m_padData[id].rightStickX = 0f;
		}
		if (Mathf.Abs(this.m_padData[id].rightStickY) < num)
		{
			this.m_padData[id].rightStickY = 0f;
		}
	}

	private void ReadXbox360(int id)
	{
		string text = (id + 1).ToString();
		this.m_padData[id].butX = Input.GetButton("joystick " + text + " button 2");
		this.m_padData[id].butY = Input.GetButton("joystick " + text + " button 3");
		this.m_padData[id].butA = Input.GetButton("joystick " + text + " button 0");
		this.m_padData[id].butB = Input.GetButton("joystick " + text + " button 1");
		this.m_padData[id].leftTop = Input.GetButton("joystick " + text + " button 4");
		this.m_padData[id].rightTop = Input.GetButton("joystick " + text + " button 5");
		this.m_padData[id].leftAsDown = Input.GetButton("joystick " + text + " button 8");
		this.m_padData[id].rightAsDown = Input.GetButton("joystick " + text + " button 9");
		this.m_padData[id].leftStickX = Input.GetAxis(text + "_X axis");
		this.m_padData[id].leftStickY = Input.GetAxis(text + "_Y axis");
		this.m_padData[id].rightStickX = Input.GetAxis(text + "_4th axis");
		this.m_padData[id].rightStickY = Input.GetAxis(text + "_5th axis");
		float axis = Input.GetAxis(text + "_6th axis");
		this.m_padData[id].left = (axis < 0f);
		this.m_padData[id].right = (axis > 0f);
		float axis2 = Input.GetAxis(text + "_7th axis");
		this.m_padData[id].up = (axis2 > 0f);
		this.m_padData[id].down = (axis2 < 0f);
		this.m_padData[id].leftTrigger = Input.GetAxis(text + "_9th axis");
		this.m_padData[id].rightTrigger = Input.GetAxis(text + "_10th axis");
		this.m_padData[id].pause = Input.GetButton("joystick " + text + " button 7");
	}

	private void ReadShieldPad(int id)
	{
		string text = (id + 1).ToString();
		this.m_padData[id].butX = Input.GetButton("joystick " + text + " button 2");
		this.m_padData[id].butY = Input.GetButton("joystick " + text + " button 3");
		this.m_padData[id].butA = Input.GetButton("joystick " + text + " button 0");
		this.m_padData[id].butB = Input.GetButton("joystick " + text + " button 1");
		this.m_padData[id].leftTop = Input.GetButton("joystick " + text + " button 4");
		this.m_padData[id].rightTop = Input.GetButton("joystick " + text + " button 5");
		this.m_padData[id].leftAsDown = Input.GetButton("joystick " + text + " button 8");
		this.m_padData[id].rightAsDown = Input.GetButton("joystick " + text + " button 9");
		this.m_padData[id].leftStickX = Input.GetAxis(text + "_X axis");
		this.m_padData[id].leftStickY = Input.GetAxis(text + "_Y axis");
		this.m_padData[id].rightStickX = Input.GetAxis(text + "_3rd axis");
		this.m_padData[id].rightStickY = Input.GetAxis(text + "_4th axis");
		float axis = Input.GetAxis(text + "_5th axis");
		this.m_padData[id].left = (axis < 0f);
		this.m_padData[id].right = (axis > 0f);
		float axis2 = Input.GetAxis(text + "_6th axis");
		this.m_padData[id].up = (axis2 < 0f);
		this.m_padData[id].down = (axis2 > 0f);
		this.m_padData[id].leftTrigger = Input.GetAxis(text + "_13th axis");
		this.m_padData[id].rightTrigger = Input.GetAxis(text + "_12th axis");
		this.m_padData[id].pause = Input.GetButton("joystick " + text + " button 10");
	}

	private void ReadShieldPadPC(int id)
	{
		string text = (id + 1).ToString();
		this.m_padData[id].butX = Input.GetButton("joystick " + text + " button 9");
		this.m_padData[id].butY = Input.GetButton("joystick " + text + " button 8");
		this.m_padData[id].butA = Input.GetButton("joystick " + text + " button 11");
		this.m_padData[id].butB = Input.GetButton("joystick " + text + " button 10");
		this.m_padData[id].leftTop = Input.GetButton("joystick " + text + " button 7");
		this.m_padData[id].rightTop = Input.GetButton("joystick " + text + " button 6");
		this.m_padData[id].leftAsDown = Input.GetButton("joystick " + text + " button 5");
		this.m_padData[id].rightAsDown = Input.GetButton("joystick " + text + " button 4");
		this.m_padData[id].leftStickX = Input.GetAxis(text + "_X axis");
		this.m_padData[id].leftStickY = Input.GetAxis(text + "_Y axis");
		this.m_padData[id].rightStickX = Input.GetAxis(text + "_3rd axis");
		this.m_padData[id].rightStickY = Input.GetAxis(text + "_4th axis");
		float axis = Input.GetAxis(text + "_5th axis");
		this.m_padData[id].left = (axis < 0f);
		this.m_padData[id].right = (axis > 0f);
		float axis2 = Input.GetAxis(text + "_6th axis");
		this.m_padData[id].up = (axis2 > 0f);
		this.m_padData[id].down = (axis2 < 0f);
		this.m_padData[id].leftTrigger = Input.GetAxis(text + "_13th axis");
		this.m_padData[id].rightTrigger = Input.GetAxis(text + "_12th axis");
		this.m_padData[id].pause = Input.GetButton("joystick " + text + " button 12");
	}

	private void ReadPCMock(int id)
	{
		string text = (id + 1).ToString();
		this.m_padData[id].butX = Input.GetKey(117);
		this.m_padData[id].butY = Input.GetKey(105);
		this.m_padData[id].butA = Input.GetKey(106);
		this.m_padData[id].butB = Input.GetKey(107);
		this.m_padData[id].leftTop = Input.GetKey(121);
		this.m_padData[id].rightTop = Input.GetKey(111);
		this.m_padData[id].leftAsDown = Input.GetKey(113);
		this.m_padData[id].rightAsDown = Input.GetKey(101);
		float leftStickX = 0f;
		if (Input.GetKey(276))
		{
			leftStickX = -1f;
		}
		if (Input.GetKey(275))
		{
			leftStickX = 1f;
		}
		float leftStickY = 0f;
		if (Input.GetKey(273))
		{
			leftStickY = -1f;
		}
		if (Input.GetKey(274))
		{
			leftStickY = 1f;
		}
		this.m_padData[id].leftStickX = leftStickX;
		this.m_padData[id].leftStickY = leftStickY;
		float rightStickX = 0f;
		if (Input.GetKey(257))
		{
			rightStickX = -1f;
		}
		if (Input.GetKey(259))
		{
			rightStickX = 1f;
		}
		float rightStickY = 0f;
		if (Input.GetKey(258))
		{
			rightStickY = -1f;
		}
		if (Input.GetKey(256))
		{
			rightStickY = 1f;
		}
		this.m_padData[id].rightStickX = rightStickX;
		this.m_padData[id].rightStickY = rightStickY;
		float axis = Input.GetAxis(text + "_5th axis");
		this.m_padData[id].left = Input.GetKey(260);
		this.m_padData[id].right = Input.GetKey(262);
		float axis2 = Input.GetAxis(text + "_6th axis");
		this.m_padData[id].up = Input.GetKey(264);
		this.m_padData[id].down = Input.GetKey(261);
		this.m_padData[id].leftTrigger = Input.GetAxis(text + "_13th axis");
		this.m_padData[id].rightTrigger = Input.GetAxis(text + "_12th axis");
		this.m_padData[id].pause = Input.GetKey(32);
	}

	private string DebugPadData(int id)
	{
		string text = " ";
		text = text + "X:" + ((!this.GetButton(id, MUControllerButtons.BUTX)) ? "0" : "1");
		text = text + " Y:" + ((!this.GetButton(id, MUControllerButtons.BUTY)) ? "0" : "1");
		text = text + " A:" + ((!this.GetButton(id, MUControllerButtons.BUTA)) ? "0" : "1");
		text = text + " B:" + ((!this.GetButton(id, MUControllerButtons.BUTB)) ? "0" : "1");
		text = text + " L: " + ((!this.GetButton(id, MUControllerButtons.LEFT)) ? "0" : "1");
		text = text + " R:" + ((!this.GetButton(id, MUControllerButtons.RIGHT)) ? "0" : "1");
		text = text + " U:" + ((!this.GetButton(id, MUControllerButtons.UP)) ? "0" : "1");
		text = text + " D:" + ((!this.GetButton(id, MUControllerButtons.DOWN)) ? "0" : "1");
		text = text + " LT:" + ((!this.GetButton(id, MUControllerButtons.LEFTTOP)) ? "0" : "1");
		text = text + " RT:" + ((!this.GetButton(id, MUControllerButtons.RIGHTTOP)) ? "0" : "1");
		text = text + " LAS:" + ((!this.GetButton(id, MUControllerButtons.LAS_BUTTON)) ? "0" : "1");
		text = text + " RAS:" + ((!this.GetButton(id, MUControllerButtons.RAS_BUTTON)) ? "0" : "1");
		text = text + " Pause:" + ((!this.GetButton(id, MUControllerButtons.PAUSE)) ? "0" : "1");
		text += "\n";
		text = text + "LX:" + this.GetAnalog(id, MUControllerAnalogs.LEFTX).ToString();
		text = text + " LY:" + this.GetAnalog(id, MUControllerAnalogs.LEFTY).ToString();
		text = text + " RX:" + this.GetAnalog(id, MUControllerAnalogs.RIGHTX).ToString();
		text = text + " RY:" + this.GetAnalog(id, MUControllerAnalogs.RIGHTY).ToString();
		text = text + " LTRIG:" + this.GetAnalog(id, MUControllerAnalogs.LEFTTRIGGER).ToString();
		text = text + " RTRIG:" + this.GetAnalog(id, MUControllerAnalogs.RIGHTTRIGGER).ToString();
		return text + "\n\n";
	}

	private string DebugPadUnknown(int id)
	{
		string text = (id + 1).ToString();
		string text2 = " ";
		text2 = text2 + " but0:" + ((!Input.GetButton("joystick " + text + " button 0")) ? "0" : "1");
		text2 = text2 + " but1:" + ((!Input.GetButton("joystick " + text + " button 1")) ? "0" : "1");
		text2 = text2 + " but2:" + ((!Input.GetButton("joystick " + text + " button 2")) ? "0" : "1");
		text2 = text2 + " but3:" + ((!Input.GetButton("joystick " + text + " button 3")) ? "0" : "1");
		text2 = text2 + " but4:" + ((!Input.GetButton("joystick " + text + " button 4")) ? "0" : "1");
		text2 = text2 + " but5:" + ((!Input.GetButton("joystick " + text + " button 5")) ? "0" : "1");
		text2 = text2 + " but6:" + ((!Input.GetButton("joystick " + text + " button 6")) ? "0" : "1");
		text2 = text2 + " but7:" + ((!Input.GetButton("joystick " + text + " button 7")) ? "0" : "1");
		text2 = text2 + " but8:" + ((!Input.GetButton("joystick " + text + " button 8")) ? "0" : "1");
		text2 = text2 + " but9:" + ((!Input.GetButton("joystick " + text + " button 9")) ? "0" : "1");
		text2 = text2 + " but10:" + ((!Input.GetButton("joystick " + text + " button 10")) ? "0" : "1");
		text2 = text2 + " but11:" + ((!Input.GetButton("joystick " + text + " button 11")) ? "0" : "1");
		text2 = text2 + " but12:" + ((!Input.GetButton("joystick " + text + " button 12")) ? "0" : "1");
		text2 = text2 + " but13:" + ((!Input.GetButton("joystick " + text + " button 13")) ? "0" : "1");
		text2 = text2 + " but14:" + ((!Input.GetButton("joystick " + text + " button 14")) ? "0" : "1");
		text2 = text2 + " but15:" + ((!Input.GetButton("joystick " + text + " button 15")) ? "0" : "1");
		text2 = text2 + " but16:" + ((!Input.GetButton("joystick " + text + " button 16")) ? "0" : "1");
		text2 = text2 + " but17:" + ((!Input.GetButton("joystick " + text + " button 17")) ? "0" : "1");
		text2 = text2 + " but18:" + ((!Input.GetButton("joystick " + text + " button 18")) ? "0" : "1");
		text2 = text2 + " but19:" + ((!Input.GetButton("joystick " + text + " button 19")) ? "0" : "1");
		text2 += "\n";
		text2 = text2 + " X axis:" + Input.GetAxis(text + "_X axis");
		text2 = text2 + " Y axis:" + Input.GetAxis(text + "_Y axis");
		text2 = text2 + " 3rd axis:" + Input.GetAxis(text + "_3rd axis");
		text2 = text2 + " 4th axis:" + Input.GetAxis(text + "_4th axis");
		text2 = text2 + " 5th axis:" + Input.GetAxis(text + "_5th axis");
		text2 = text2 + " 6th axis:" + Input.GetAxis(text + "_6th axis");
		text2 = text2 + " 7th axis:" + Input.GetAxis(text + "_7th axis");
		text2 = text2 + " 8th axis:" + Input.GetAxis(text + "_8th axis");
		text2 = text2 + " 9th axis:" + Input.GetAxis(text + "_9th axis");
		text2 = text2 + "10th axis:" + Input.GetAxis(text + "_10th axis");
		text2 = text2 + "11th axis:" + Input.GetAxis(text + "_11th axis");
		text2 = text2 + "12th axis:" + Input.GetAxis(text + "_12th axis");
		text2 = text2 + "13th axis:" + Input.GetAxis(text + "_13th axis");
		return text2 + "\n\n";
	}

	private void OnGUI()
	{
	}

	private bool GetButton(int id, MUControllerButtons b)
	{
		switch (b)
		{
		case MUControllerButtons.LAS_BUTTON:
			return this.m_padData[id].leftAsDown;
		case MUControllerButtons.RAS_BUTTON:
			return this.m_padData[id].rightAsDown;
		case MUControllerButtons.BUTX:
			return this.m_padData[id].butX;
		case MUControllerButtons.BUTY:
			return this.m_padData[id].butY;
		case MUControllerButtons.BUTA:
			return this.m_padData[id].butA;
		case MUControllerButtons.BUTB:
			return this.m_padData[id].butB;
		case MUControllerButtons.UP:
			return this.m_padData[id].up;
		case MUControllerButtons.DOWN:
			return this.m_padData[id].down;
		case MUControllerButtons.LEFT:
			return this.m_padData[id].left;
		case MUControllerButtons.RIGHT:
			return this.m_padData[id].right;
		case MUControllerButtons.LEFTTOP:
			return this.m_padData[id].leftTop;
		case MUControllerButtons.RIGHTTOP:
			return this.m_padData[id].rightTop;
		case MUControllerButtons.PAUSE:
			return this.m_padData[id].pause;
		default:
			return false;
		}
	}

	public bool GetButtonDown(int id, MUControllerButtons b)
	{
		switch (b)
		{
		case MUControllerButtons.LAS_BUTTON:
		{
			bool flag = this.m_padData[id].leftAsDown;
			bool flag2 = !this.m_padDataOld[id].leftAsDown;
			return flag && flag2;
		}
		case MUControllerButtons.RAS_BUTTON:
		{
			bool flag = this.m_padData[id].rightAsDown;
			bool flag2 = !this.m_padDataOld[id].rightAsDown;
			return flag && flag2;
		}
		case MUControllerButtons.BUTX:
		{
			bool flag = this.m_padData[id].butX;
			bool flag2 = !this.m_padDataOld[id].butX;
			return flag && flag2;
		}
		case MUControllerButtons.BUTY:
		{
			bool flag = this.m_padData[id].butY;
			bool flag2 = !this.m_padDataOld[id].butY;
			return flag && flag2;
		}
		case MUControllerButtons.BUTA:
		{
			bool flag = this.m_padData[id].butA;
			bool flag2 = !this.m_padDataOld[id].butA;
			return flag && flag2;
		}
		case MUControllerButtons.BUTB:
		{
			bool flag = this.m_padData[id].butB;
			bool flag2 = !this.m_padDataOld[id].butB;
			return flag && flag2;
		}
		case MUControllerButtons.UP:
		{
			bool flag = this.m_padData[id].up;
			bool flag2 = !this.m_padDataOld[id].up;
			return flag && flag2;
		}
		case MUControllerButtons.DOWN:
		{
			bool flag = this.m_padData[id].down;
			bool flag2 = !this.m_padDataOld[id].down;
			return flag && flag2;
		}
		case MUControllerButtons.LEFT:
		{
			bool flag = this.m_padData[id].left;
			bool flag2 = !this.m_padDataOld[id].left;
			return flag && flag2;
		}
		case MUControllerButtons.RIGHT:
		{
			bool flag = this.m_padData[id].right;
			bool flag2 = !this.m_padDataOld[id].right;
			return flag && flag2;
		}
		case MUControllerButtons.LEFTTOP:
		{
			bool flag = this.m_padData[id].leftTop;
			bool flag2 = !this.m_padDataOld[id].leftTop;
			return flag && flag2;
		}
		case MUControllerButtons.RIGHTTOP:
		{
			bool flag = this.m_padData[id].rightTop;
			bool flag2 = !this.m_padDataOld[id].rightTop;
			return flag && flag2;
		}
		case MUControllerButtons.PAUSE:
		{
			bool flag = this.m_padData[id].pause;
			bool flag2 = !this.m_padDataOld[id].pause;
			return flag && flag2;
		}
		default:
			return false;
		}
	}

	public bool GetButtonUp(int id, MUControllerButtons b)
	{
		switch (b)
		{
		case MUControllerButtons.LAS_BUTTON:
		{
			bool flag = !this.m_padData[id].leftAsDown;
			bool flag2 = this.m_padDataOld[id].leftAsDown;
			return flag && flag2;
		}
		case MUControllerButtons.RAS_BUTTON:
		{
			bool flag = !this.m_padData[id].rightAsDown;
			bool flag2 = this.m_padDataOld[id].rightAsDown;
			return flag && flag2;
		}
		case MUControllerButtons.BUTX:
		{
			bool flag = !this.m_padData[id].butX;
			bool flag2 = this.m_padDataOld[id].butX;
			return flag && flag2;
		}
		case MUControllerButtons.BUTY:
		{
			bool flag = !this.m_padData[id].butY;
			bool flag2 = this.m_padDataOld[id].butY;
			return flag && flag2;
		}
		case MUControllerButtons.BUTA:
		{
			bool flag = !this.m_padData[id].butA;
			bool flag2 = this.m_padDataOld[id].butA;
			return flag && flag2;
		}
		case MUControllerButtons.BUTB:
		{
			bool flag = !this.m_padData[id].butB;
			bool flag2 = this.m_padDataOld[id].butB;
			return flag && flag2;
		}
		case MUControllerButtons.UP:
		{
			bool flag = !this.m_padData[id].up;
			bool flag2 = this.m_padDataOld[id].up;
			return flag && flag2;
		}
		case MUControllerButtons.DOWN:
		{
			bool flag = !this.m_padData[id].down;
			bool flag2 = this.m_padDataOld[id].down;
			return flag && flag2;
		}
		case MUControllerButtons.LEFT:
		{
			bool flag = !this.m_padData[id].left;
			bool flag2 = this.m_padDataOld[id].left;
			return flag && flag2;
		}
		case MUControllerButtons.RIGHT:
		{
			bool flag = !this.m_padData[id].right;
			bool flag2 = this.m_padDataOld[id].right;
			return flag && flag2;
		}
		case MUControllerButtons.LEFTTOP:
		{
			bool flag = !this.m_padData[id].leftTop;
			bool flag2 = this.m_padDataOld[id].leftTop;
			return flag && flag2;
		}
		case MUControllerButtons.RIGHTTOP:
		{
			bool flag = !this.m_padData[id].rightTop;
			bool flag2 = this.m_padDataOld[id].rightTop;
			return flag && flag2;
		}
		case MUControllerButtons.PAUSE:
		{
			bool flag = !this.m_padData[id].pause;
			bool flag2 = this.m_padDataOld[id].pause;
			return flag && flag2;
		}
		default:
			return false;
		}
	}

	public float GetAnalog(int id, MUControllerAnalogs c)
	{
		switch (c)
		{
		case MUControllerAnalogs.LEFTX:
			return this.m_padData[id].leftStickX;
		case MUControllerAnalogs.LEFTY:
			return this.m_padData[id].leftStickY;
		case MUControllerAnalogs.RIGHTX:
			return this.m_padData[id].rightStickX;
		case MUControllerAnalogs.RIGHTY:
			return this.m_padData[id].rightStickY;
		case MUControllerAnalogs.LEFTTRIGGER:
			return this.m_padData[id].leftTrigger;
		case MUControllerAnalogs.RIGHTTRIGGER:
			return this.m_padData[id].rightTrigger;
		default:
			return 0f;
		}
	}

	public bool GetButton(MUControllerButtons b)
	{
		for (int i = 0; i < this.m_activePadCount; i++)
		{
			if (this.GetButton(i, b))
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonDown(MUControllerButtons b)
	{
		for (int i = 0; i < this.m_activePadCount; i++)
		{
			if (this.GetButtonDown(i, b))
			{
				return true;
			}
		}
		return false;
	}

	public bool GetButtonUp(MUControllerButtons b)
	{
		for (int i = 0; i < this.m_activePadCount; i++)
		{
			if (this.GetButtonUp(i, b))
			{
				return true;
			}
		}
		return false;
	}

	public float GetAnalog(MUControllerAnalogs c)
	{
		float num = 0f;
		for (int i = 0; i < this.m_activePadCount; i++)
		{
			num += this.GetAnalog(i, c);
		}
		if (num < -1f)
		{
			num = -1f;
		}
		if (num > 1f)
		{
			num = 1f;
		}
		return num;
	}

	public static bool bePCMockAllowed = true;

	private static int MAXCONTROLLERS = 4;

	private static string VERSION = "0.03";

	public bool beOpen = true;

	private int m_activePadCount;

	private string[] m_activedPadNames;

	private MUControllerPacket[] m_padData;

	private MUControllerPacket[] m_padDataOld;

	private string[] m_suppurtPadNames = new string[]
	{
		"NVIDIA Controller",
		"XBOX 360 For Windows",
		"USB",
		"PC Mock Input"
	};

	private static MUJoystickController m_instance;
}
