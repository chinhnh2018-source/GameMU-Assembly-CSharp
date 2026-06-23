using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SelectDatePart : UserControl
{
	protected override void InitializeComponent()
	{
		this.YearTxt.text = Global.GetLang("年");
		this.MonthTxt.text = Global.GetLang("月");
		this.DayTxt.text = Global.GetLang("日");
		this.OkBtn.Text = Global.GetLang("确定");
		this.CancelBtn.Text = Global.GetLang("取消");
		this.OkBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			int num = correctDateTime.Year * 12 * 30 + correctDateTime.Month * 30 + correctDateTime.Day;
			int num2 = this.GetYear() * 12 * 30 + this.GetMonth() * 30 + this.GetDay();
			if (num > num2)
			{
				Super.HintMainText(Global.GetLang("选择日期不得早于今天！"), 10, 3);
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.CancelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
		this.MonthCenter.onFinished = new SpringPanel.OnFinished(this.OnMonthChangeFinished);
		this.SetDataList();
		base.StartCoroutine<bool>(this.DelayShowTime());
	}

	private void SetTargetPos()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		float num = (float)(correctDateTime.Day - this.startDay - 1) * (this.DayItems[this.startDay + 1].transform.localPosition.y - this.DayItems[this.startDay].transform.localPosition.y);
		this.DayScroll.MoveRelativeEx(new Vector3(0f, -num, 0f));
		float num2 = (float)(correctDateTime.Month - this.startMonth - 1) * (this.MonthItems[this.startMonth + 1].transform.localPosition.y - this.MonthItems[this.startMonth].transform.localPosition.y);
		this.MonthScroll.MoveRelativeEx(new Vector3(0f, -num2, 0f));
		float num3 = (float)(correctDateTime.Year - this.nowYear - 1) * (this.YearItems[this.nowYear + 1].transform.localPosition.y - this.YearItems[this.nowYear].transform.localPosition.y);
		this.YearScroll.MoveRelativeEx(new Vector3(0f, -num3, 0f));
		this.MonthCenter.enabled = true;
		this.YearCenter.enabled = true;
	}

	private IEnumerator DelayShowTime()
	{
		yield return new WaitForSeconds(0.05f);
		this.SetTargetPos();
		yield break;
	}

	private void SetDataList()
	{
		for (int i = 0; i < this.YearTable.transform.childCount; i++)
		{
			DayTxtItem component = this.YearTable.transform.GetChild(i).GetComponent<DayTxtItem>();
			component.value.text = (i + this.nowYear).ToString();
			this.YearItems.Add(i + this.nowYear, component);
		}
		for (int j = 0; j < this.MonthTable.transform.childCount; j++)
		{
			DayTxtItem component2 = this.MonthTable.transform.GetChild(j).GetComponent<DayTxtItem>();
			component2.value.text = (j + this.startMonth).ToString();
			this.MonthItems.Add(j + this.startMonth, component2);
		}
		for (int k = 0; k < this.DayTable.transform.childCount; k++)
		{
			DayTxtItem component3 = this.DayTable.transform.GetChild(k).GetComponent<DayTxtItem>();
			component3.value.text = (k + this.startDay).ToString();
			this.DayItems.Add(k + this.startDay, component3);
		}
	}

	public string GetDateTime()
	{
		string text = string.Empty;
		DayTxtItem component = this.YearCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			text = component.value.text + Global.GetLang("年");
		}
		component = this.MonthCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			text = text + component.value.text + Global.GetLang("月");
		}
		component = this.DayCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			text = text + component.value.text + Global.GetLang("日");
		}
		return text;
	}

	public int GetYear()
	{
		int result = 0;
		DayTxtItem component = this.YearCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			result = Global.SafeConvertToInt32(component.value.text);
		}
		return result;
	}

	public int GetMonth()
	{
		int result = 0;
		DayTxtItem component = this.MonthCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			result = Global.SafeConvertToInt32(component.value.text);
		}
		return result;
	}

	public int GetDay()
	{
		int result = 0;
		DayTxtItem component = this.DayCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			result = Global.SafeConvertToInt32(component.value.text);
		}
		return result;
	}

	private void OnMonthChangeFinished()
	{
		DayTxtItem component = this.MonthCenter.centeredObject.GetComponent<DayTxtItem>();
		if (component != null)
		{
			switch (Global.SafeConvertToInt32(component.value.text))
			{
			case 1:
			case 3:
			case 5:
			case 7:
			case 8:
			case 10:
			case 12:
				this.Set28Day(true);
				break;
			case 2:
				if (this.IsRunNian())
				{
					this.Set29Day();
				}
				else
				{
					this.Set28Day(false);
				}
				break;
			case 4:
			case 6:
			case 9:
			case 11:
				this.Set28Day(true);
				this.Set31Day(false);
				break;
			}
		}
		if (!this.DayCenter.enabled)
		{
			this.DayCenter.enabled = true;
		}
		this.DayCenter.Recenter();
	}

	private void Set31Day(bool show)
	{
		this.DayItems[31].gameObject.SetActive(show);
	}

	private void Set28Day(bool show)
	{
		this.DayItems[29].gameObject.SetActive(show);
		this.DayItems[30].gameObject.SetActive(show);
		this.DayItems[31].gameObject.SetActive(show);
	}

	private void Set29Day()
	{
		this.DayItems[30].gameObject.SetActive(false);
		this.DayItems[31].gameObject.SetActive(false);
	}

	private bool IsRunNian()
	{
		DayTxtItem component = this.YearCenter.centeredObject.GetComponent<DayTxtItem>();
		if (!(component != null))
		{
			return false;
		}
		int num = Global.SafeConvertToInt32(component.value.text);
		if (num % 100 == 0)
		{
			return num % 400 == 0;
		}
		return num % 4 == 0;
	}

	public UILabel YearTxt;

	public UILabel MonthTxt;

	public UILabel DayTxt;

	public UITable YearTable;

	public UITable MonthTable;

	public UITable DayTable;

	public UICenterOnChild YearCenter;

	public UICenterOnChild MonthCenter;

	public UICenterOnChild DayCenter;

	public UIDraggablePanel DayScroll;

	public UIDraggablePanel MonthScroll;

	public UIDraggablePanel YearScroll;

	public GButton OkBtn;

	public GButton CancelBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	private Dictionary<int, DayTxtItem> YearItems = new Dictionary<int, DayTxtItem>();

	private Dictionary<int, DayTxtItem> MonthItems = new Dictionary<int, DayTxtItem>();

	private Dictionary<int, DayTxtItem> DayItems = new Dictionary<int, DayTxtItem>();

	private int nowYear = 2015;

	private int startMonth = 1;

	private int startDay = 1;
}
