using System;
using UnityEngine;

public class ObservableCollection
{
	private object NotifyCollectionChanged(string type, object obj)
	{
		if (this.CollectionChanged != null)
		{
			return this.CollectionChanged(type, obj);
		}
		return null;
	}

	public void Clear()
	{
		this.NotifyCollectionChanged("CC_Remove", -1);
	}

	public void Add(GameObject go)
	{
		this.NotifyCollectionChanged("CC_Add", go);
	}

	public void Add(Component component)
	{
		this.NotifyCollectionChanged("CC_Add", component.gameObject);
	}

	public void AddNoUpdate(GameObject go)
	{
		this.NotifyCollectionChanged("CC_Add", go);
	}

	public void AddNoUpdate(Component component)
	{
		this.NotifyCollectionChanged("CC_Add", component.gameObject);
	}

	public void DelayUpdate()
	{
	}

	public void RemoveAt(int index)
	{
		this.NotifyCollectionChanged("CC_Remove", index);
	}

	public void RemoveAtNoUpdate(int index)
	{
		this.NotifyCollectionChanged("CC_Remove", index);
	}

	public void Remove(object value)
	{
		this.NotifyCollectionChanged("CC_Remove", value);
	}

	public void RemoveNoUpdate(object value)
	{
		this.NotifyCollectionChanged("CC_Remove", value);
	}

	public GameObject GetAt(int index)
	{
		return this.NotifyCollectionChanged("CC_Get", index) as GameObject;
	}

	public GameObject this[int index]
	{
		get
		{
			return this.NotifyCollectionChanged("CC_Get", index) as GameObject;
		}
	}

	public int Count
	{
		get
		{
			return (int)this.NotifyCollectionChanged("CC_Count", null);
		}
	}

	public int Length
	{
		get
		{
			return (int)this.NotifyCollectionChanged("CC_Count", null);
		}
	}

	public void Insert(int index, Component component)
	{
		CollectionObject obj = new CollectionObject(index, component.gameObject);
		this.NotifyCollectionChanged("CC_Insert", obj);
	}

	public const string CC_Remove = "CC_Remove";

	public const string CC_Add = "CC_Add";

	public const string CC_Get = "CC_Get";

	public const string CC_Count = "CC_Count";

	public const string CC_Insert = "CC_Insert";

	public CollectionChangedEventHandler CollectionChanged;
}
