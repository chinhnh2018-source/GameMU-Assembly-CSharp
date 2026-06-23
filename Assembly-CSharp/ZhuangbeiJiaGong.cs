using System;
using UnityEngine;

public class ZhuangbeiJiaGong : MonoBehaviour
{
	private void Start()
	{
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
		this._ListBox.Add(SpawnManager.Instantiate(this._ListItem0) as GameObject);
	}

	public ListBox _ListBox;

	public GameObject _ListItem0;
}
