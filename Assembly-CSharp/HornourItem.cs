using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HornourItem : MonoBehaviour
{
	private void Awake()
	{
		this.gBtn = base.gameObject.GetComponent<GButton>();
		this.gBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.index
			});
		};
	}

	public void SetHornourItemSelectedStatus(bool enable = true, bool Pressed = true)
	{
		this.isEnabled = enable;
		UIControlStatus uicontrolStatus;
		if (enable)
		{
			this.gBtn.isEnabled = true;
			if (Pressed)
			{
				uicontrolStatus = UIControlStatus.UIControlStatus_Highlight;
			}
			else
			{
				uicontrolStatus = UIControlStatus.UIControlStatus_Normal;
			}
		}
		else
		{
			uicontrolStatus = UIControlStatus.UIControlStatus_Disabled;
		}
		this.gBtn.Refresh();
		this.SetThumbnailGray(!enable);
		this.SetSelectStatus(uicontrolStatus);
	}

	public void SetHornourItemSelected(bool selected = true)
	{
		this.SetSelectStatus((!selected) ? UIControlStatus.UIControlStatus_Normal : UIControlStatus.UIControlStatus_Highlight);
	}

	private void SetSelectStatus(UIControlStatus status)
	{
		if (null != this.selectStatus)
		{
			this.selectStatus.spriteName = "hornour" + this.UIControlStatusToString(status);
		}
	}

	private void SetThumbnailGray(bool gray = true)
	{
		if (null != this.thumbnail)
		{
			this.thumbnail.ToGrayBitmap = gray;
		}
	}

	private string UIControlStatusToString(UIControlStatus control_status)
	{
		string result = "_active";
		switch (control_status)
		{
		case UIControlStatus.UIControlStatus_Normal:
			result = "_active";
			break;
		case UIControlStatus.UIControlStatus_Highlight:
			result = "_highlight";
			break;
		case UIControlStatus.UIControlStatus_Disabled:
			result = "_unactive";
			break;
		}
		return result;
	}

	public GButton gBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int index = 1;

	public bool isEnabled;

	public ShowNetImage thumbnail;

	public UISprite selectStatus;
}
