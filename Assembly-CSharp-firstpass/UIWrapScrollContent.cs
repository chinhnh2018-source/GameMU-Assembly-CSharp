using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIWrapScrollContent : MonoBehaviour
{
	public int PianIndexScroll
	{
		get
		{
			return this.pianIndexScroll;
		}
		set
		{
			if (value < this.pianIndexScroll)
			{
				for (int i = 0; i < this.pianIndexScroll - value; i++)
				{
					this.PutLastInFisrt();
				}
			}
			else
			{
				for (int j = 0; j < value - this.pianIndexScroll; j++)
				{
					this.PutFisrtInLast();
				}
			}
			this.pianIndexScroll = value;
		}
	}

	private void Start()
	{
		this._UIDraggablePanel = base.GetComponentInParent<UIDraggablePanel>();
	}

	private void Update()
	{
		if (this.children == null || this.children.Length < 5)
		{
			base.StartCoroutine(this.Innit());
		}
		if (this.children != null && this.children.Length > 5 && this.direction == UIWrapScrollContent.Direction.Vertical)
		{
			this.PianYiValue = this._UIDraggablePanel.transform.position.y - this.posValueStart;
			this.PianIndexScroll = (int)(this.PianYiValue / this.ChildLengh);
		}
	}

	private IEnumerator Innit()
	{
		this.children = new Transform[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.children[i] = base.transform.GetChild(i);
		}
		if (this.direction == UIWrapScrollContent.Direction.Vertical)
		{
			this.ChildLengh = Math.Abs(this.children[2].position.y - this.children[1].position.y);
			this.posValueStart = this._UIDraggablePanel.transform.position.y;
		}
		this.PutLastInFisrt();
		yield return new WaitForSeconds(0.11f);
		this.PutLastInFisrt();
		yield break;
	}

	private void PutLastInFisrt()
	{
		List<Transform> list = Enumerable.ToList<Transform>(this.children);
		if (this.direction == UIWrapScrollContent.Direction.Vertical)
		{
			list = Enumerable.ToList<Transform>(Enumerable.OrderByDescending<Transform, float>(list, (Transform s) => s.position.y));
			list[list.Count - 1].position = new Vector3(list[0].position.x, list[0].position.y + this.ChildLengh, list[0].position.z);
		}
	}

	private void PutFisrtInLast()
	{
		List<Transform> list = Enumerable.ToList<Transform>(this.children);
		if (this.direction == UIWrapScrollContent.Direction.Vertical)
		{
			list = Enumerable.ToList<Transform>(Enumerable.OrderByDescending<Transform, float>(list, (Transform s) => s.position.y));
			list[0].position = new Vector3(list[0].position.x, list[list.Count - 1].position.y - this.ChildLengh, list[0].position.z);
		}
	}

	public UIDraggablePanel _UIDraggablePanel;

	private float ChildLengh;

	private float posValueStart;

	private float PianYiValue;

	private int pianIndexScroll;

	public UIWrapScrollContent.Direction direction = UIWrapScrollContent.Direction.Vertical;

	public Transform[] children;

	public enum Direction
	{
		Horizontal,
		Vertical
	}
}
