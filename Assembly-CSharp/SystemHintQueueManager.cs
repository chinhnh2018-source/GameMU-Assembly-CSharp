using System;
using System.Collections.Generic;

public class SystemHintQueueManager : UserControl
{
	public int ShowNum
	{
		get
		{
			return this.m_showNum;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void UpdateState()
	{
		this.m_showNum = 0;
		bool flag = false;
		for (int i = 0; i < this.lstHintQueue.Count; i++)
		{
			bool flag2 = this.lstHintQueue[i].UpdateState();
			if (flag2)
			{
				flag = true;
			}
			if (this.lstHintQueue[i].BeShow())
			{
				this.m_showNum++;
			}
		}
		if (flag)
		{
			this.grid.Reposition();
		}
	}

	public List<SystemHintQueue> lstHintQueue;

	public UIGrid grid;

	private int m_showNum;
}
