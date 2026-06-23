using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class UserControl : Canvas
{
	public virtual void OnActive(bool active)
	{
	}

	public bool IsActive
	{
		get
		{
			return base.enabled && base.gameObject.activeInHierarchy;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.Container = this;
		this.InitializeComponent();
	}

	protected virtual void OnDestroy()
	{
		if (this.subPartDic != null)
		{
			this.subPartDic.Clear();
			this.subWindowDic.Clear();
		}
		base.StopAllCoroutines();
		this.Destroy();
	}

	protected virtual void InitializeComponent()
	{
	}

	public virtual void Destroy()
	{
	}

	public void NotifyClose()
	{
		if (this.ChildWindowClose != null)
		{
			if (this.ChildWindowClose(this, EventArgs.Empty))
			{
				this.Visibility = false;
			}
		}
		else
		{
			this.Visibility = false;
		}
	}

	public Point Coordinate
	{
		get
		{
			return this._Coordinate;
		}
		set
		{
			this._Coordinate = value;
			this.X = (double)value.X;
			this.Y = (double)value.Y;
		}
	}

	public UserControl parent
	{
		get
		{
			if (null != base.transform.parent)
			{
				return base.transform.parent.GetComponent<UserControl>();
			}
			return null;
		}
		set
		{
			if (null != value)
			{
				U3DUtils.AddChild(value.gameObject, base.gameObject, false);
			}
			else
			{
				base.gameObject.transform.parent = null;
			}
		}
	}

	public ImageURL BodyBackgroundURL { get; set; }

	public bool Onlydouble { get; set; }

	public bool EnableIcon { get; set; }

	public double scaleX { get; set; }

	public double scaleY { get; set; }

	public Canvas stage
	{
		get
		{
			return this.Container;
		}
	}

	public void startDrag(bool bDrag, Rectangle rect)
	{
	}

	public void stopDrag()
	{
	}

	protected void InitHintDecoration(int decoCode, Point pos, SpriteSL addToParent = null)
	{
		if (null == addToParent)
		{
			addToParent = this.Container;
		}
		this.ClearHintDecoration();
		this.HintDeco = Global.GetDecoration(decoCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
		this.HintDeco.Start();
	}

	protected virtual void ClearHintDecoration()
	{
		if (this.HintDeco != null)
		{
			Global.RemoveObject(this.HintDeco, true);
			this.HintDeco = null;
		}
	}

	public T OpenWindow<T>(bool isShowModal = true) where T : UserControl
	{
		string text = typeof(T).ToString();
		UserControl part = null;
		GChildWindow gchildWindow = null;
		this.subPartDic.TryGetValue(text, ref part);
		this.subWindowDic.TryGetValue(text, ref gchildWindow);
		if (gchildWindow == null)
		{
			gchildWindow = U3DUtils.NEW<GChildWindow>();
			gchildWindow.IsShowModal = isShowModal;
			gchildWindow.Modal = true;
			gchildWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(gchildWindow, text);
			Super.GData.GlobalPlayZone.Children.Add(gchildWindow);
		}
		if (part == null)
		{
			part = U3DUtils.NEW<T>();
			part.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.CloseWindow(part.GetType().ToString());
				return true;
			};
		}
		gchildWindow.SetContent(gchildWindow.BodyPresenter, part, 0.0, 0.0, true);
		this.subPartDic[text] = part;
		this.subWindowDic[text] = gchildWindow;
		return part as T;
	}

	public void CloseWindow(string className)
	{
		UserControl userControl = null;
		GChildWindow gchildWindow = null;
		this.subPartDic.TryGetValue(className, ref userControl);
		this.subWindowDic.TryGetValue(className, ref gchildWindow);
		if (null != userControl)
		{
			userControl.transform.parent = null;
			Object.Destroy(userControl.gameObject);
			userControl = null;
			this.subPartDic.Remove(className);
		}
		if (null != gchildWindow)
		{
			Super.CloseChildWindow(base.Children, gchildWindow);
			Super.GData.GlobalPlayZone.Children.Remove(gchildWindow, true);
			gchildWindow = null;
			this.subWindowDic.Remove(className);
		}
	}

	public Canvas Container;

	public GDecoration HintDeco;

	public ChildWindowCloseEventHandler ChildWindowClose;

	protected Point _Coordinate = new Point(0, 0);

	private Dictionary<string, UserControl> subPartDic = new Dictionary<string, UserControl>();

	private Dictionary<string, GChildWindow> subWindowDic = new Dictionary<string, GChildWindow>();
}
