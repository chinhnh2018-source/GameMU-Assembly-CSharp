using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class BuffBoxMini : UserControl
{
	public ObservableCollection boxIcons
	{
		get
		{
			return this._boxIcons.ItemsSource;
		}
	}

	protected virtual void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			this.RefreshDataList();
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	protected override void InitializeComponent()
	{
		this.m_boxCollider = this.btnClick.GetComponent<BoxCollider>();
		UIEventListener.Get(this.btnClick.gameObject).onClick = delegate(GameObject s)
		{
			(Super.GData.GlobalPlayZone as PlayZone).InitBufferBox();
		};
	}

	public void RefreshDataList()
	{
		if (Global.Data.roleData != null && Global.Data.roleData.BufferDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.BufferDataList.Count; i++)
			{
				this.RefreshData(Global.Data.roleData.BufferDataList[i]);
			}
		}
		this.CheckBufferDataList();
		this.ResetBoxSize();
	}

	private void ResetBoxSize()
	{
		Vector3 size;
		size..ctor(this._boxIcons.cellWidth * (float)this.boxIcons.Count, this._boxIcons.cellHeight, 1f);
		Vector3 center;
		center..ctor((size.x - this._boxIcons.cellWidth) / 2f, 0f, 0f);
		this.m_boxCollider.size = size;
		this.m_boxCollider.center = center;
	}

	public BufferData GetBufferDataByID(int id)
	{
		return Global.Data.roleData.BufferDataList.Find((BufferData info) => info.BufferID == id);
	}

	public void CheckBufferDataList()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		for (int i = 0; i < this.boxIcons.Count; i++)
		{
			MainBuffIcon mainBuffIcon = U3DUtils.AS<MainBuffIcon>(this.boxIcons[i]);
			BufferData buffData = mainBuffIcon.BuffData;
			if (buffData != null)
			{
				if (this.GetBufferDataByID(buffData.BufferID) == null)
				{
					this.boxIcons.RemoveAtNoUpdate(i);
				}
				else if (Global.IsBufferDataOver(buffData, correctLocalTime, false))
				{
					this.boxIcons.RemoveAtNoUpdate(i);
				}
			}
		}
		this.boxIcons.DelayUpdate();
	}

	public void RefreshData(BufferData bufferData)
	{
		if (Global.IsDummyBuffer(bufferData.BufferID))
		{
			return;
		}
		int num = -1;
		int num2 = 0;
		int num3 = -1;
		int count = this.boxIcons.Count;
		int num4 = (count >= 1) ? ((count - 1) / 9 + 1) : 1;
		for (int i = 0; i < count; i++)
		{
			MainBuffIcon mainBuffIcon = U3DUtils.AS<MainBuffIcon>(this.boxIcons[i]);
			if (mainBuffIcon.BuffData.BufferID == bufferData.BufferID)
			{
				mainBuffIcon.SetBuffImageCode(DBBufferBox.GetBufferIconString(bufferData, ref num2, ref num3));
				num = i;
				break;
			}
		}
		if (Global.IsBufferDataOver(bufferData, Global.GetCorrectLocalTime(), false))
		{
			if (num >= 0)
			{
				this.boxIcons.RemoveAt(num);
			}
			return;
		}
		if (num < 0 && this.boxIcons.Count < 6)
		{
			this.AddIcon(bufferData);
		}
	}

	private void AddIcon(BufferData bufferData)
	{
		int num = 0;
		int num2 = -1;
		string bufferIconString = DBBufferBox.GetBufferIconString(bufferData, ref num, ref num2);
		MainBuffIcon mainBuffIcon = U3DUtils.NEW<MainBuffIcon>("MainBuffIcon");
		mainBuffIcon.BuffData = bufferData;
		mainBuffIcon.SetBuffImageCode(bufferIconString);
		this.boxIcons.Add(mainBuffIcon);
		this.boxIcons.DelayUpdate();
	}

	public UIButton BuffIcon;

	public TextBlock BuffNum;

	public GameObject[] BuffAnim;

	public GameObject btnClick;

	private BoxCollider m_boxCollider;

	private float TickInterval = 1f;

	[SerializeField]
	protected ListBox _boxIcons;
}
