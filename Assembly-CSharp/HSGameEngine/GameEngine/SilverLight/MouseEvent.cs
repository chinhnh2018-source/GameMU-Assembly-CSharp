using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class MouseEvent : BaseEventArgs
	{
		public MouseEvent(string e = "mouseUp", MonoBehaviour target = null)
		{
			base.Text = e;
			this._currentTarget = target;
		}

		public MonoBehaviour currentTarget
		{
			get
			{
				return this._currentTarget;
			}
		}

		public int stageX
		{
			get
			{
				return this._stageX;
			}
			set
			{
				this._stageX = value;
			}
		}

		public int stageY
		{
			get
			{
				return this._stageY;
			}
			set
			{
				this._stageY = value;
			}
		}

		public bool buttonDown
		{
			get
			{
				return this._buttonDown;
			}
		}

		public bool ctrlKey
		{
			get
			{
				return this._ctrlKey;
			}
			set
			{
				this._ctrlKey = value;
			}
		}

		public new GameObject target
		{
			get
			{
				return this._target;
			}
			set
			{
				this._target = value;
			}
		}

		public int localX
		{
			get
			{
				return this._localX;
			}
			set
			{
				this._localX = value;
			}
		}

		public int localY
		{
			get
			{
				return this._localY;
			}
			set
			{
				this._localY = value;
			}
		}

		public void stopPropagation()
		{
		}

		public int delta { get; set; }

		public int Index { get; set; }

		public const string CLICK = "click";

		public const string DOUBLE_CLICK = "doubleClick";

		public const string MOUSE_DOWN = "mouseDown";

		public const string MOUSE_MOVE = "mouseMove";

		public const string MOUSE_OUT = "mouseOut";

		public const string MOUSE_OVER = "mouseOver";

		public const string MOUSE_UP = "mouseUp";

		public const string MOUSE_WHEEL = "mouseWheel";

		public const string ROLL_OVER = "ROLL_OVER";

		public const string ROLL_OUT = "ROLL_OUT";

		public new static readonly MouseEvent Empty;

		private MonoBehaviour _currentTarget;

		private int _stageX;

		private int _stageY;

		private bool _buttonDown;

		private bool _ctrlKey;

		private GameObject _target;

		private int _localX;

		private int _localY;
	}
}
