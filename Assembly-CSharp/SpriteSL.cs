using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class SpriteSL : TTMonoBehaviour, ISilverLight
{
	protected GameObject MyGameObject
	{
		get
		{
			if (this && null == this._MyGameObject)
			{
				this._MyGameObject = base.gameObject;
			}
			return this._MyGameObject;
		}
	}

	protected Transform MyTransform
	{
		get
		{
			if (null == this._MyTransform)
			{
				this._MyTransform = base.transform;
			}
			return this._MyTransform;
		}
	}

	protected virtual void Awake()
	{
		this._X = (double)this.MyTransform.localPosition.x;
		this._Y = (double)this.MyTransform.localPosition.y;
		this._Z = (double)this.MyTransform.localPosition.z;
	}

	protected virtual void Start()
	{
	}

	public virtual void Update()
	{
	}

	public double Width
	{
		get
		{
			return this._Width;
		}
		set
		{
			this._Width = value;
			this.UpdateLayout();
		}
	}

	public double Height
	{
		get
		{
			return this._Height;
		}
		set
		{
			this._Height = value;
			this.UpdateLayout();
		}
	}

	public double ActualWidth
	{
		get
		{
			if (double.IsNaN(this._Width))
			{
				return (double)NGUIMath.CalculateRelativeWidgetBounds(this.MyTransform).size.x;
			}
			return this._Width;
		}
	}

	public double ActualHeight
	{
		get
		{
			if (double.IsNaN(this._Height))
			{
				return (double)NGUIMath.CalculateRelativeWidgetBounds(this.MyTransform).size.y;
			}
			return this._Height;
		}
	}

	public string Name
	{
		get
		{
			return base.name;
		}
		set
		{
			base.name = value;
		}
	}

	public bool Visibility
	{
		get
		{
			return this.MyGameObject && this.MyGameObject.activeSelf;
		}
		set
		{
			if (this.MyGameObject != null && this.MyGameObject.activeSelf != value)
			{
				this.MyGameObject.SetActive(value);
			}
		}
	}

	public object Tag
	{
		get
		{
			return this._Tag;
		}
		set
		{
			this._Tag = value;
		}
	}

	public SpriteSL Children
	{
		get
		{
			return this;
		}
	}

	public void Add(GameObject obj)
	{
		if (null == obj)
		{
			return;
		}
		U3DUtils.AddChild(this.MyGameObject, obj, true);
		this.UpdateLayout();
	}

	public void AddNoUpdate(GameObject obj)
	{
		U3DUtils.AddChild(this.MyGameObject, obj, true);
	}

	public void Add(Component component)
	{
		this.Add(component.gameObject);
	}

	public void AddNoUpdate(Component component)
	{
		this.AddNoUpdate(component);
	}

	public void Remove(GameObject obj, bool toDestroy = true)
	{
		obj.transform.parent = null;
		if (toDestroy)
		{
			this.DestroyObjectCustom(obj, true);
		}
		obj = null;
		this.UpdateLayout();
	}

	public void Remove(Component component, bool toDestroy = true)
	{
		this.Remove(component.gameObject, toDestroy);
	}

	public void RemoveAt(int index, bool toDestroy = true, bool toUnloadUnused = true)
	{
		GameObject gameObject = this.MyTransform.GetChild(index).gameObject;
		if (null != gameObject)
		{
			gameObject.transform.parent = null;
			if (toDestroy)
			{
				this.DestroyObjectCustom(gameObject, toUnloadUnused);
			}
			this.UpdateLayout();
		}
	}

	private void DestroyObjectCustom(GameObject obj, bool toUnloadUnused = true)
	{
		if (obj != null)
		{
			Object.Destroy(obj);
			obj = null;
		}
		if (toUnloadUnused)
		{
			Resources.UnloadUnusedAssets();
		}
	}

	public void Remove(int index)
	{
		this.RemoveAt(index, true, true);
	}

	public void Insert(int index, GameObject obj)
	{
	}

	public int Length()
	{
		return this.MyTransform.childCount;
	}

	public int Count()
	{
		return this.MyTransform.childCount;
	}

	public void Clear()
	{
		while (this.MyTransform.childCount > 0)
		{
			GameObject gameObject = this.MyTransform.GetChild(0).gameObject;
			gameObject.transform.parent = null;
			Object.Destroy(gameObject);
		}
		this.UpdateLayout();
	}

	public GameObject FindName(string name)
	{
		Transform transform = this.MyTransform.FindChild(name);
		if (null != transform)
		{
			return transform.gameObject;
		}
		return null;
	}

	public GameObject getChildAt(int index)
	{
		return this.MyTransform.GetChild(index).gameObject;
	}

	public GameObject this[int i]
	{
		get
		{
			return this.MyTransform.GetChild(i).gameObject;
		}
	}

	public bool IsHitTestVisible
	{
		get
		{
			return this._IsHitTestVisible;
		}
		set
		{
			this._IsHitTestVisible = value;
		}
	}

	public GameObject Parent
	{
		get
		{
			if (null == this.MyTransform.parent)
			{
				return null;
			}
			return this.MyTransform.parent.gameObject;
		}
		set
		{
			Vector3 position = this.MyTransform.position;
			Vector3 localScale = this.MyTransform.localScale;
			Quaternion rotation = this.MyTransform.rotation;
			this.MyTransform.parent = value.transform;
			this.MyTransform.localScale = localScale;
			this.MyTransform.localPosition = position;
			this.MyTransform.localRotation = rotation;
		}
	}

	public void UpdateLayout()
	{
		if (this.NoUpdateLayout)
		{
			return;
		}
		if (null != this.MyTransform.parent)
		{
			SpriteSL component = this.MyTransform.parent.gameObject.GetComponent<SpriteSL>();
			if (null != component)
			{
				component.UpdateLayout();
			}
		}
	}

	public double X
	{
		get
		{
			return (double)base.transform.localPosition.x;
		}
		set
		{
			this._X = value;
			base.transform.localPosition = new Vector3((float)this._X, (float)this.Y, (float)this.Z);
		}
	}

	public double Y
	{
		get
		{
			return (double)base.transform.localPosition.y;
		}
		set
		{
			this._Y = value;
			base.transform.localPosition = new Vector3((float)this.X, (float)this._Y, (float)this.Z);
		}
	}

	public double Z
	{
		get
		{
			return (double)base.transform.localPosition.z;
		}
		set
		{
			this._Z = -value;
			base.transform.localPosition = new Vector3((float)this.X, (float)this.Y, (float)this._Z);
		}
	}

	public Thickness Margin
	{
		get
		{
			return this._Margin;
		}
		set
		{
			this._Margin = value;
			this.UpdateLayout();
		}
	}

	public void Measure(SizeSL availableSize)
	{
		this._DesiredSize = new SizeSL(this.ActualWidth, this.ActualHeight);
	}

	public SizeSL DesiredSize
	{
		get
		{
			return this._DesiredSize;
		}
		set
		{
			this._DesiredSize = value;
		}
	}

	public int numChildren
	{
		get
		{
			return this._numChildren;
		}
		set
		{
			this._numChildren = value;
		}
	}

	public int Cursor
	{
		get
		{
			return this._Cursor;
		}
		set
		{
			this._Cursor = value;
		}
	}

	public double width { get; set; }

	public double height { get; set; }

	public Brush Background { get; set; }

	public uint BackgroundColor { get; set; }

	public double BackgroundAlpha { get; set; }

	public bool mouseChildren { get; set; }

	public bool mouseEnabled { get; set; }

	public bool cacheAsBitmap { get; set; }

	public void addEventListener(string eventName, MouseEventHandler handler)
	{
		if (eventName != null)
		{
			if (SpriteSL.<>f__switch$map0 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("ROLL_OVER", 0);
				dictionary.Add("ROLL_OUT", 1);
				dictionary.Add("mouseDown", 2);
				dictionary.Add("mouseUp", 3);
				dictionary.Add("click", 4);
				SpriteSL.<>f__switch$map0 = dictionary;
			}
			int num;
			if (SpriteSL.<>f__switch$map0.TryGetValue(eventName, ref num))
			{
				switch (num)
				{
				case 0:
				{
					UIEventListener uieventListener = UIEventListener.Get(base.gameObject);
					uieventListener.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onHover, delegate(GameObject go, bool state)
					{
						if (state)
						{
							handler(new MouseEvent("mouseUp", null)
							{
								target = go
							});
						}
					});
					break;
				}
				case 1:
				{
					UIEventListener uieventListener2 = UIEventListener.Get(base.gameObject);
					uieventListener2.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onHover, delegate(GameObject go, bool state)
					{
						if (!state)
						{
							handler(new MouseEvent("mouseUp", null)
							{
								target = go
							});
						}
					});
					break;
				}
				case 2:
				{
					UIEventListener uieventListener3 = UIEventListener.Get(base.gameObject);
					uieventListener3.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener3.onPress, delegate(GameObject go, bool state)
					{
						if (state)
						{
							handler(new MouseEvent("mouseUp", null)
							{
								target = go
							});
						}
					});
					break;
				}
				case 3:
				{
					UIEventListener uieventListener4 = UIEventListener.Get(base.gameObject);
					uieventListener4.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener4.onPress, delegate(GameObject go, bool state)
					{
						if (!state)
						{
							handler(new MouseEvent("mouseUp", null)
							{
								target = go
							});
						}
					});
					break;
				}
				case 4:
				{
					UIEventListener uieventListener5 = UIEventListener.Get(base.gameObject);
					uieventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener5.onClick, delegate(GameObject go)
					{
						handler(new MouseEvent("mouseUp", null)
						{
							target = go
						});
					});
					break;
				}
				}
			}
		}
	}

	public void Insert(int index, Component obj)
	{
	}

	public void swapChildren(object obj1, object obj2)
	{
	}

	protected bool NoUpdateLayout = true;

	private GameObject _MyGameObject;

	protected Transform _MyTransform;

	protected double _Width = double.NaN;

	protected double _Height = double.NaN;

	private object _Tag;

	private bool _IsHitTestVisible = true;

	private double _X;

	private double _Y;

	private double _Z;

	private Thickness _Margin = new Thickness(0.0, 0.0, 0.0, 0.0);

	private SizeSL _DesiredSize = new SizeSL(0.0, 0.0);

	private int _numChildren;

	private int _Cursor;
}
