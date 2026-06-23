using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class StallNamePart : UserControl
{
	protected override void InitializeComponent()
	{
		if (null != this.m_LblStallName)
		{
			UIEventListener.Get(this.m_LblStallName.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick);
		}
	}

	public string StallName
	{
		get
		{
			return this.m_strStallName;
		}
		set
		{
			this.ShowStallName(value);
			this.m_strStallName = value;
		}
	}

	private void OnClick(GameObject go)
	{
		if (null != this.m_LblStallName)
		{
			this.m_LblStallName.text = this.m_strStallName;
		}
	}

	public void ShowStallName(string strName = "")
	{
		GameObject gameObject = null;
		if (string.Empty == strName)
		{
			strName = string.Format(Global.GetLang("{0}的货摊"), this.m_strStallName);
		}
		if (null == this.m_ObjStallNamePrefab)
		{
			this.m_ObjStallNamePrefab = (Resources.Load("Prefabs/UI/StallNamePart") as GameObject);
		}
		if (null != this.m_ObjStallNamePrefab && null == gameObject)
		{
			gameObject = NGUITools.AddChild(HUDTextRoot.go, this.m_ObjStallNamePrefab);
			gameObject.AddComponent<UIFollowTarget>().target = this.m_Target;
			StallNamePart component = gameObject.gameObject.GetComponent<StallNamePart>();
			if (null != component)
			{
				component.m_LblStallName.text = strName;
			}
		}
	}

	public UILabel m_LblStallName;

	public Transform m_Target;

	public string m_strStallName = string.Empty;

	public GameObject m_ObjStallNamePrefab;
}
