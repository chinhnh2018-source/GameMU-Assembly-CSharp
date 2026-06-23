using System;
using System.Collections.Generic;
using UnityEngine;

public class WCJManager : MonoBehaviour
{
	public List<T> RandomSortList<T>(List<T> ListT)
	{
		List<T> list = new List<T>();
		for (int i = 0; i < ListT.Count; i++)
		{
			T t = ListT[i];
			int num = this.rand.Next(0, list.Count + 1);
			list.Insert(num, t);
		}
		return list;
	}

	private void Start()
	{
	}

	private void EnableGameObject()
	{
		if (this.TotalNum < this.arrowTotal)
		{
			if (this.list.Count > this.TotalNum)
			{
				this.list[this.TotalNum].transform.localRotation = Quaternion.Euler(Vector3.zero);
				float num = this.anglesList[this.TotalNum % 5];
				this.list[this.TotalNum].transform.Rotate(this.list[this.TotalNum].transform.up, num, 0);
				this.list[this.TotalNum].SetActive(true);
			}
			this.TotalNum++;
		}
		if (this.TotalNum >= this.arrowTotal)
		{
			base.CancelInvoke("EnableGameObject");
		}
	}

	public void OnEnable()
	{
		this.TotalNum = 0;
		for (int i = 0; i < this.list.Count; i++)
		{
			this.list[i].SetActive(false);
		}
		if (this.anglesList != null && this.anglesList.Count == 0)
		{
			for (int j = 0; j < 5; j++)
			{
				this.anglesList.Add(-30f + (float)j * 15f);
			}
		}
		this.anglesList = this.RandomSortList<float>(this.anglesList);
		base.InvokeRepeating("EnableGameObject", 0.01f, this.frequency);
	}

	public void OnDisable()
	{
	}

	public float frequency = 0.1f;

	public List<GameObject> list = new List<GameObject>();

	private int TotalNum;

	public int arrowTotal = 5;

	private Random rand = new Random();

	private List<float> anglesList = new List<float>();
}
