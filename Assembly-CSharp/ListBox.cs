using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class ListBox : Canvas
{
	public ListBox()
	{
		this._Items.CollectionChanged = new CollectionChangedEventHandler(this.CollectionChanged);
	}

	public bool VerticalIcon { get; set; }

	public object ItemsPanel { get; set; }

	public object ItemContainerStyle { get; set; }

	protected override void Awake()
	{
		base.Awake();
		if (null == this._SpringPosition)
		{
			this._SpringPosition = base.gameObject.AddComponent<SpringPosition>();
			this._SpringPosition.enabled = false;
		}
	}

	protected override void Start()
	{
		base.Start();
		this.mStarted = true;
		this.Reposition();
	}

	public override void Update()
	{
		base.Update();
		if (this.repositionNow)
		{
			this.repositionNow = false;
			this.Reposition();
		}
	}

	protected virtual void UISelectionChanged()
	{
	}

	public new double Width
	{
		get
		{
			return this._Width;
		}
		set
		{
			this._Width = value;
		}
	}

	public new double Height
	{
		get
		{
			return this._Height;
		}
		set
		{
			this._Height = value;
		}
	}

	public GameObject LastSelectedItem
	{
		get
		{
			return this._LastSelectedItem;
		}
	}

	public GameObject SelectedItem
	{
		get
		{
			return this._SelectedItem;
		}
	}

	private object CollectionChanged(string type, object obj)
	{
		if (type == "CC_Add")
		{
			this.repositionNow = true;
			if (obj is GameObject)
			{
				GameObject gameObject = obj as GameObject;
				if (null != gameObject)
				{
					if (this.ItemAutoCollider)
					{
						NGUITools.AddListIetmCollider(gameObject);
					}
					UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.ItemClick);
					this._ItemsList.Add(obj);
					NGUITools.AddChild2(base.MyGameObject, gameObject);
					if (this.ChildHasChange != null)
					{
						this.ChildHasChange();
					}
				}
				return gameObject;
			}
		}
		else
		{
			if (type == "CC_Remove")
			{
				this.repositionNow = true;
				if (obj is GameObject)
				{
					GameObject gameObject2 = obj as GameObject;
					if (null != gameObject2)
					{
						int num = this._ItemsList.IndexOf(obj);
						if (num >= 0)
						{
							this._ItemsList.RemoveAt(num);
						}
						NGUITools.Destroy(gameObject2);
						if (gameObject2 == this._SelectedItem)
						{
							this._LastSelectedItem = null;
							this._SelectedItem = null;
							this._SelectedIndex = -1;
							if (this.SelectionChanged != null)
							{
								this.SelectionChanged(this, new MouseEvent("mouseUp", null));
							}
							if (this.ChildHasChange != null)
							{
								this.ChildHasChange();
							}
							this.UISelectionChanged();
						}
					}
				}
				else
				{
					int num2 = (int)obj;
					if (num2 >= 0)
					{
						if (num2 < this._ItemsList.Count)
						{
							GameObject gameObject3 = this._ItemsList[num2] as GameObject;
							if (null != gameObject3)
							{
								this._ItemsList.RemoveAt(num2);
								NGUITools.Destroy(gameObject3);
								if (gameObject3 == this._SelectedItem)
								{
									this._LastSelectedItem = null;
									this._SelectedItem = null;
									this._SelectedIndex = -1;
									if (this.SelectionChanged != null)
									{
										this.SelectionChanged(this, new MouseEvent("mouseUp", null));
									}
									if (this.ChildHasChange != null)
									{
										this.ChildHasChange();
									}
									this.UISelectionChanged();
								}
							}
						}
					}
					else
					{
						while (this._ItemsList.Count > 0)
						{
							GameObject gameObject4 = this._ItemsList[0] as GameObject;
							if (null != gameObject4)
							{
								NGUITools.Destroy(gameObject4);
							}
							if (this.ChildHasChange != null)
							{
								this.ChildHasChange();
							}
							this._ItemsList.RemoveAt(0);
						}
						this._ItemsList.Clear();
						this._LastSelectedItem = null;
						this._SelectedItem = null;
						this._SelectedIndex = -1;
						if (this.SelectionChanged != null)
						{
							this.SelectionChanged(this, new MouseEvent("mouseUp", null));
						}
						this.UISelectionChanged();
					}
				}
				return null;
			}
			if (type == "CC_Get")
			{
				int num3 = (int)obj;
				if (num3 >= 0 && num3 < this._ItemsList.Count)
				{
					return this._ItemsList[num3] as GameObject;
				}
			}
			else
			{
				if (type == "CC_Count")
				{
					return this._ItemsList.Count;
				}
				if (type == "CC_Insert")
				{
					this.repositionNow = true;
					if (obj is CollectionObject)
					{
						CollectionObject collectionObject = obj as CollectionObject;
						if (collectionObject != null)
						{
							if (null != collectionObject.obj)
							{
								if (this.ItemAutoCollider)
								{
									NGUITools.AddListIetmCollider(collectionObject.obj);
								}
								UIEventListener.Get(collectionObject.obj).onClick = new UIEventListener.VoidDelegate(this.ItemClick);
								this._ItemsList.Insert((collectionObject.index < 0) ? 0 : collectionObject.index, collectionObject.obj);
								NGUITools.AddChild2(base.MyGameObject, collectionObject.obj);
								if (this.ChildHasChange != null)
								{
									this.ChildHasChange();
								}
							}
							return collectionObject.obj;
						}
						return null;
					}
				}
			}
		}
		return null;
	}

	public ObservableCollection Items
	{
		get
		{
			return this._Items;
		}
	}

	public ObservableCollection ItemsSource
	{
		get
		{
			return this._Items;
		}
	}

	public static int SortByName(object a, object b)
	{
		return string.Compare((a as GameObject).name, (b as GameObject).name);
	}

	[ContextMenu("Execute")]
	private void Reposition()
	{
		if (!this.mStarted)
		{
			this.repositionNow = true;
			return;
		}
		if (null == UICamera.currentCamera)
		{
			this.repositionNow = true;
			return;
		}
		bool flag = false;
		Transform transform = base.transform;
		int num = 0;
		int num2 = 0;
		if (this.sorted)
		{
			this._ItemsList.Sort(new Comparison<object>(ListBox.SortByName));
			int i = 0;
			int count = this._ItemsList.Count;
			while (i < count)
			{
				Transform transform2 = (this._ItemsList[i] as GameObject).transform;
				if (NGUITools.GetActive(transform2.gameObject) || !this.hideInactive)
				{
					if (transform2.gameObject == this._SelectedItem)
					{
						flag = true;
					}
					float z = transform2.localPosition.z;
					transform2.localPosition = ((this.arrangement != ListBox.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z));
					if (++num >= this.maxPerLine && this.maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
				i++;
			}
		}
		else
		{
			for (int j = 0; j < this._ItemsList.Count; j++)
			{
				GameObject gameObject = this._ItemsList[j] as GameObject;
				if (NGUITools.GetActive(gameObject) || !this.hideInactive)
				{
					if (gameObject == this._SelectedItem)
					{
						flag = true;
					}
					Transform transform3 = gameObject.transform;
					float z2 = transform3.localPosition.z;
					transform3.localPosition = ((this.arrangement != ListBox.Arrangement.Horizontal) ? new Vector3(this.cellWidth * (float)num2, -this.cellHeight * (float)num, z2) : new Vector3(this.cellWidth * (float)num, -this.cellHeight * (float)num2, z2));
					if (++num >= this.maxPerLine && this.maxPerLine > 0)
					{
						num = 0;
						num2++;
					}
				}
			}
		}
		if (null != this._SelectedItem && !flag)
		{
			this._LastSelectedItem = this._SelectedItem;
			this._SelectedItem = null;
		}
		if (this.AutoCollider)
		{
			NGUITools.AddWidgetCollider(base.MyGameObject);
		}
		if (this.mDrag != null)
		{
			this.mDrag.UpdateScrollbars(true);
		}
		if (this.OnReposition != null)
		{
			this.OnReposition();
		}
	}

	public void SpringPositionProc()
	{
		if (this.mDrag == null)
		{
			this.mDrag = NGUITools.FindInParents<UIDraggablePanel>(base.gameObject);
			if (this.mDrag == null)
			{
				return;
			}
		}
		if (this.mDrag.panel == null)
		{
			return;
		}
		Vector4 clipRange = this.mDrag.panel.clipRange;
		Transform cachedTransform = this.mDrag.panel.cachedTransform;
		Vector3 vector = cachedTransform.localPosition;
		vector.x += clipRange.x - clipRange.z / 2f + this.cellWidth / 2f;
		vector.y += clipRange.y - clipRange.w / 2f + this.cellHeight / 2f;
		vector = cachedTransform.parent.TransformPoint(vector);
		Vector3 vector2 = vector - this.mDrag.currentMomentum * (this.mDrag.momentumAmount * 0.1f);
		this.mDrag.currentMomentum = Vector3.zero;
		float num = float.MaxValue;
		Transform transform = null;
		Transform transform2 = base.transform;
		int num2 = 1;
		int i = 0;
		int childCount = transform2.childCount;
		while (i < childCount)
		{
			Transform child = transform2.GetChild(i);
			float num3 = Vector3.SqrMagnitude(child.position - vector2);
			if (num3 < num)
			{
				num = num3;
				transform = child;
			}
			i += num2;
		}
		if (transform != null)
		{
			this.mCenteredObject = transform.gameObject;
			Vector3 vector3 = cachedTransform.InverseTransformPoint(transform.position);
			Vector3 vector4 = cachedTransform.InverseTransformPoint(vector);
			Vector3 vector5 = vector3 - vector4;
			if (this.mDrag.scale.x == 0f)
			{
				vector5.x = 0f;
			}
			if (this.mDrag.scale.y == 0f)
			{
				vector5.y = 0f;
			}
			if (this.mDrag.scale.z == 0f)
			{
				vector5.z = 0f;
			}
			SpringPanel.Begin(this.mDrag.gameObject, cachedTransform.localPosition - vector5, this.springStrength).onFinished = this.onFinished;
		}
		else
		{
			this.mCenteredObject = null;
		}
	}

	public Thickness ItemMargin
	{
		get
		{
			return this._ItemMargin;
		}
		set
		{
			this._ItemMargin = value;
		}
	}

	public bool ForceSelectChanged { get; set; }

	public int SelectedIndex
	{
		get
		{
			return this._SelectedIndex;
		}
		set
		{
			if (this._SelectedIndex != value)
			{
				if (value < 0)
				{
					this._SelectedIndex = value;
				}
				else if (value < this._ItemsList.Count)
				{
					this._SelectedIndex = value;
					GameObject selectedItem = this._ItemsList[this._SelectedIndex] as GameObject;
					this._LastSelectedItem = this._SelectedItem;
					this._SelectedItem = selectedItem;
					if (this._LastSelectedItem != this._SelectedItem)
					{
						if (this.SelectionChanged != null)
						{
							this.SelectionChanged(this, new MouseEvent("mouseUp", null));
						}
						this.UISelectionChanged();
					}
				}
			}
		}
	}

	public void DoSelectItem(GameObject selectedItem)
	{
		if (this._ItemsList == null)
		{
			return;
		}
		int num = this._ItemsList.IndexOf(selectedItem);
		if (num < 0)
		{
			return;
		}
		this.SelectedIndex = num;
	}

	public void ClearLastSelect()
	{
		this._LastSelectedItem = null;
	}

	public bool Replace(int index, object newValue)
	{
		if (!(newValue is GameObject))
		{
			return false;
		}
		if (index < 0 || index >= this._ItemsList.Count)
		{
			return false;
		}
		GameObject gameObject = this._ItemsList[index] as GameObject;
		if (null == gameObject)
		{
			return false;
		}
		if (this.ItemAutoCollider)
		{
			NGUITools.AddWidgetCollider(newValue as GameObject);
		}
		UIEventListener.Get(newValue as GameObject).onClick = new UIEventListener.VoidDelegate(this.ItemClick);
		this._ItemsList[index] = newValue;
		NGUITools.AddChild2(base.MyGameObject, newValue as GameObject);
		if (gameObject == this._SelectedItem)
		{
			this._SelectedItem = (newValue as GameObject);
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged(this, new MouseEvent("mouseUp", null));
			}
			this.UISelectionChanged();
		}
		NGUITools.Destroy(gameObject);
		this.repositionNow = true;
		return true;
	}

	public GameObject GetItemByIndex(int index)
	{
		if (index < 0 || index >= this._ItemsList.Count)
		{
			return null;
		}
		GameObject gameObject = this._ItemsList[index] as GameObject;
		if (null == gameObject)
		{
			return null;
		}
		return gameObject;
	}

	public int BorderThickness
	{
		get
		{
			return this._BorderThickness;
		}
		set
		{
			this._BorderThickness = value;
		}
	}

	private void ItemClick(GameObject go)
	{
		GameObject selectedItem = null;
		if (this.MouseLeftButtonDown != null)
		{
			this.MouseLeftButtonDown(this, new MouseEvent("mouseUp", null)
			{
				target = base.gameObject
			});
		}
		if (this.MouseLeftButtonDownEx != null)
		{
			this.MouseLeftButtonDownEx(this, new MouseEvent("mouseUp", null)
			{
				target = go
			});
		}
		for (int i = 0; i < this._ItemsList.Count; i++)
		{
			GameObject gameObject = this._ItemsList[i] as GameObject;
			if (UICamera.hoveredObject == gameObject)
			{
				selectedItem = gameObject;
				break;
			}
		}
		this._LastSelectedItem = this._SelectedItem;
		this._SelectedItem = selectedItem;
		if (null != this._SelectedItem)
		{
			this._SelectedIndex = this._ItemsList.IndexOf(this._SelectedItem);
		}
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged(this, new MouseEvent("mouseUp", null)
			{
				target = base.gameObject
			});
		}
		this.UISelectionChanged();
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp(this, new MouseEvent("mouseUp", null)
			{
				target = base.gameObject
			});
		}
	}

	public int VerticalScrollBarVisibility
	{
		get
		{
			return this._VerticalScrollBarVisibility;
		}
		set
		{
			this._VerticalScrollBarVisibility = value;
		}
	}

	public string VerticalAlignment
	{
		get
		{
			return this._VerticalAlignment;
		}
		set
		{
			this._VerticalAlignment = value;
		}
	}

	public BitmapData ScrollBarBak
	{
		get
		{
			return this._ScrollBarBak;
		}
		set
		{
			this._ScrollBarBak = value;
		}
	}

	public ListBox.Arrangement arrangement;

	public ListBox.VoidDelegate ChildHasChange;

	public MouseLeftButtonUpEventHandler SelectionChanged;

	public MouseLeftButtonUpEventHandler MouseLeftButtonDown;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUp;

	public MouseLeftButtonUpEventHandler MouseLeftButtonDownEx;

	private GameObject _LastSelectedItem;

	private GameObject _SelectedItem;

	private int _SelectedIndex = -1;

	private ObservableCollection _Items = new ObservableCollection();

	private List<object> _ItemsList = new List<object>();

	public bool AutoCollider;

	public bool ItemAutoCollider = true;

	public int maxPerLine;

	public float cellWidth = 200f;

	public float cellHeight = 200f;

	public bool repositionNow;

	public bool sorted;

	public bool hideInactive = true;

	public int ItemPerPage;

	private bool mStarted;

	public float springStrength = 8f;

	public SpringPanel.OnFinished onFinished;

	private SpringPosition _SpringPosition;

	private UIDraggablePanel mDrag;

	private GameObject mCenteredObject;

	public ListBox.OnRepositionFinish OnReposition;

	private Thickness _ItemMargin = new Thickness(0.0, 0.0, 0.0, 0.0);

	private int _BorderThickness;

	private int _VerticalScrollBarVisibility;

	private string _VerticalAlignment = string.Empty;

	private BitmapData _ScrollBarBak;

	public enum Arrangement
	{
		Horizontal,
		Vertical
	}

	public delegate void VoidDelegate();

	public delegate void OnRepositionFinish();
}
