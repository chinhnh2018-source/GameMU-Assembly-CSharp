using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class UIListBox : ListBox
{
	protected override void Awake()
	{
		base.Awake();
		int selectIndex = 0;
		if (null != this._LiftOrTopBtn)
		{
			this._LiftOrTopBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				selectIndex = this.SelectedIndex;
				this.SelectIndexChange(--selectIndex);
				if (this.BtnClick != null)
				{
					this.BtnClick(this, new MouseEvent("mouseUp", null)
					{
						target = this._LiftOrTopBtn.gameObject,
						Index = selectIndex
					});
				}
			};
		}
		if (null != this._RightOrBelowBtn)
		{
			this._RightOrBelowBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				selectIndex = this.SelectedIndex;
				this.SelectIndexChange(++selectIndex);
				if (this.BtnClick != null)
				{
					this.BtnClick(this, new MouseEvent("mouseUp", null)
					{
						target = this._RightOrBelowBtn.gameObject,
						Index = selectIndex
					});
				}
			};
		}
		this._DragPanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.DragFinished);
	}

	private void DragFinished()
	{
		Vector3 localPosition = this._DragPanel.transform.localPosition;
		int num = base.SelectedIndex;
		int num2 = 0;
		if (0 > num)
		{
			num = 0;
		}
		double num3 = (double)((this.arrangement != ListBox.Arrangement.Horizontal) ? this.cellHeight : this.cellWidth);
		if ((double)Mathf.Abs(localPosition.x) - (double)num * num3 > num3 * 0.4)
		{
			num2 = Mathf.RoundToInt(Mathf.Abs(localPosition.x) / (float)num3);
		}
		SpringPanel.Begin(this._DragPanel.gameObject, new Vector3((float)(-(float)num2) * (float)num3, localPosition.y, localPosition.z), 10f);
		base.SelectedIndex = num2;
	}

	protected override void UISelectionChanged()
	{
		base.UISelectionChanged();
		Vector3 localPosition = this._DragPanel.transform.localPosition;
		double num = (double)((this.arrangement != ListBox.Arrangement.Horizontal) ? this.cellHeight : this.cellWidth);
		SpringPanel.Begin(this._DragPanel.gameObject, new Vector3((float)(-(float)base.SelectedIndex) * (float)num, localPosition.y, localPosition.z), 10f);
		this.RefreshBtns();
	}

	private void SelectIndexChange(int selectIndex)
	{
		if (selectIndex < base.Items.Count && selectIndex >= 0)
		{
			base.SelectedIndex = selectIndex;
		}
		this.RefreshBtns();
	}

	protected void RefreshBtns()
	{
		if (null == this._RightOrBelowBtn)
		{
			return;
		}
		if (null == this._LiftOrTopBtn)
		{
			return;
		}
		int selectedIndex = base.SelectedIndex;
		if (1 >= base.Items.Count)
		{
			this._LiftOrTopBtn.isEnabled = false;
			this._RightOrBelowBtn.isEnabled = false;
			return;
		}
		if (0 >= base.SelectedIndex)
		{
			this._LiftOrTopBtn.isEnabled = false;
		}
		else
		{
			this._LiftOrTopBtn.isEnabled = true;
		}
		if (selectedIndex >= base.Items.Count - 1)
		{
			this._RightOrBelowBtn.isEnabled = false;
		}
		else
		{
			this._RightOrBelowBtn.isEnabled = true;
		}
	}

	public MouseLeftButtonUpEventHandler BtnClick;

	public GButton _LiftOrTopBtn;

	public GButton _RightOrBelowBtn;

	public UIDraggablePanel _DragPanel;
}
