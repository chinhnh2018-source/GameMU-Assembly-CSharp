using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;

public class FixedListBoxDragDropTarget : SpriteSL
{
	public bool AllowDrop
	{
		get
		{
			return this._AllowDrop;
		}
		set
		{
			this._AllowDrop = value;
			if (value)
			{
				base.BackgroundColor = uint.MaxValue;
				base.BackgroundAlpha = 0.01;
			}
			else
			{
				base.BackgroundColor = 16777215U;
				base.BackgroundAlpha = 1.0;
			}
		}
	}

	public bool ProcessStartDrag(ListBox listBox, GIcon icon, MouseEvent evt)
	{
		if (!this._AllowDrop)
		{
			return false;
		}
		if (this.ItemDragStarting != null)
		{
			this.ItemDragStarting.Invoke(this, new DragDropEventArgs
			{
				DragSource = listBox,
				Data = icon
			});
		}
		FixedListBoxDragDropTarget.Draging = true;
		return true;
	}

	public bool ProcessEndDrag(ListBox listBox, MouseEvent evt)
	{
		if (!this._AllowDrop)
		{
			return false;
		}
		if (!FixedListBoxDragDropTarget.Draging)
		{
			return false;
		}
		if (this.Drop != null)
		{
			this.Drop.Invoke(this, new DragDropEventArgs
			{
				stageX = evt.stageX,
				stageY = evt.stageY
			});
		}
		FixedListBoxDragDropTarget.Draging = false;
		GoodsDragingMgr.CancelGoodsDraging();
		return true;
	}

	private void UserControl_MouseLeftButtonUp(MouseEvent e)
	{
		if (base.Children.Count() > 0)
		{
			ListBox listBox = null;
			for (int i = 0; i < base.numChildren; i++)
			{
				if (base.Children.getChildAt(i).SafeGetComponent<ListBox>() is ListBox)
				{
					listBox = base.Children.getChildAt(i).SafeGetComponent<ListBox>();
					break;
				}
			}
			if (null != listBox)
			{
				this.ProcessEndDrag(listBox, e);
			}
		}
	}

	public static void CancelGoodsDraging(MouseEvent evt, bool force = false)
	{
		if (!force && evt.buttonDown)
		{
			return;
		}
		if (FixedListBoxDragDropTarget.Draging)
		{
			FixedListBoxDragDropTarget.Draging = false;
			GoodsDragingMgr.CancelGoodsDraging();
		}
	}

	public void RemoveHandlers()
	{
	}

	public object Content { get; set; }

	public static bool Draging;

	public EventHandler ItemDragStarting;

	public EventHandler ItemDragCompleted;

	public EventHandler ItemDroppedOnTarget;

	public EventHandler Drop;

	private bool _AllowDrop;
}
