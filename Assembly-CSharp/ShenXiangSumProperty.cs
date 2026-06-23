using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenXiangSumProperty : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseCallback != null)
			{
				this.CloseCallback(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void Show(string titleName, int count, string content)
	{
		this.m_Title.Text = Global.GetLang(titleName);
		this.m_ValueList.Text = content;
		this.m_BoxCollider.center = new Vector3(6.53f, -8.7f, -0.5f);
		Vector3 size;
		size..ctor(20.2f, 450f, 1f);
		this.m_BoxCollider.size = size;
	}

	protected override void OnDestroy()
	{
		this.CloseCallback = null;
		this.m_CloseBtn = null;
		this.m_Title = null;
		this.m_ValueList = null;
		this.m_BoxCollider = null;
	}

	public DPSelectedItemEventHandler CloseCallback;

	public GButton m_CloseBtn;

	public TextBlock m_Title;

	public TextBlock m_ValueList;

	public BoxCollider m_BoxCollider;
}
