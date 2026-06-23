using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	private void OnSubmit()
	{
		try
		{
			if (this.onSubmit != null)
			{
				this.onSubmit(base.gameObject);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnClick()
	{
		try
		{
			if (this.onClick != null)
			{
				this.onClick(base.gameObject);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnDoubleClick()
	{
		try
		{
			if (this.onDoubleClick != null)
			{
				this.onDoubleClick(base.gameObject);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnHover(bool isOver)
	{
		try
		{
			if (this.onHover != null)
			{
				this.onHover(base.gameObject, isOver);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnPress(bool isPressed)
	{
		try
		{
			if (this.onPress != null)
			{
				this.onPress(base.gameObject, isPressed);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnSelect(bool selected)
	{
		try
		{
			if (this.onSelect != null)
			{
				this.onSelect(base.gameObject, selected);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnScroll(float delta)
	{
		try
		{
			if (this.onScroll != null)
			{
				this.onScroll(base.gameObject, delta);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		try
		{
			if (this.onDrag != null)
			{
				this.onDrag(base.gameObject, delta);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnDrop(GameObject go)
	{
		try
		{
			if (this.onDrop != null)
			{
				this.onDrop(base.gameObject, go);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnInput(string text)
	{
		try
		{
			if (this.onInput != null)
			{
				this.onInput(base.gameObject, text);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void OnKey(KeyCode key)
	{
		try
		{
			if (this.onKey != null)
			{
				this.onKey(base.gameObject, key);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	public static UIEventListener Get(GameObject go)
	{
		UIEventListener uieventListener = go.GetComponent<UIEventListener>();
		if (uieventListener == null)
		{
			uieventListener = go.AddComponent<UIEventListener>();
		}
		return uieventListener;
	}

	public object parameter;

	public UIEventListener.VoidDelegate onSubmit;

	public UIEventListener.VoidDelegate onClick;

	public UIEventListener.VoidDelegate onDoubleClick;

	public UIEventListener.BoolDelegate onHover;

	public UIEventListener.BoolDelegate onPress;

	public UIEventListener.BoolDelegate onSelect;

	public UIEventListener.FloatDelegate onScroll;

	public UIEventListener.VectorDelegate onDrag;

	public UIEventListener.ObjectDelegate onDrop;

	public UIEventListener.StringDelegate onInput;

	public UIEventListener.KeyCodeDelegate onKey;

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void StringDelegate(GameObject go, string text);

	public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
}
