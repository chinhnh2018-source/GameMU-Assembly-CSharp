using System;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;

public class OtherTaskInfoItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
	}

	public void InitTextContent(string content, TaskData tmpData)
	{
		this.m_Content.Text = content;
		this.taskData = tmpData;
	}

	public TextBlock m_Content;

	public BoxCollider m_BoxCollider;

	public GButton m_btn;

	public TaskData taskData;

	public int TaskClass = -1;

	public int CompletState;
}
