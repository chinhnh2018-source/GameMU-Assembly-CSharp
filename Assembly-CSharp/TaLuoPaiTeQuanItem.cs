using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class TaLuoPaiTeQuanItem : UserControl
{
	public string str
	{
		get
		{
			return this.m_str;
		}
		set
		{
			this.m_Count = 0;
			this.m_str = string.Empty;
			this.m_ListSp.Clear();
			GameObject gameObject = U3DUtils.Clone(this.shuxing.gameObject, this.m_sp.gameObject);
			for (int i = 0; i < value.Length; i++)
			{
				string text = value.get_Chars(i).ToString();
				if (text.Equals("&"))
				{
					gameObject.transform.localPosition = new Vector3(0f, (float)(-30 * this.m_Count), 0f);
					this.m_Count++;
					value.Remove(i, 1);
					this.m_ListSp.Add(gameObject);
				}
				else
				{
					this.m_str += text;
				}
			}
		}
	}

	public TaLuoPaiItem TaLuoPaiItem
	{
		get
		{
			return this.m_Item;
		}
		set
		{
			this.m_Item = value;
		}
	}

	public UILabel UILabel
	{
		get
		{
			return this.m_UILabel;
		}
		set
		{
			this.UILabel = value;
		}
	}

	public GameObject shuxing;

	public UILabel m_UILabel;

	public TaLuoPaiItem m_Item;

	public UISprite m_sp;

	private string m_str = string.Empty;

	private int m_Count;

	private List<GameObject> m_ListSp = new List<GameObject>();
}
