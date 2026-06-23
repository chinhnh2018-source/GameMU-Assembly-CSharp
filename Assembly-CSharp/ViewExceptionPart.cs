using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Umeng;
using UnityEngine;

public class ViewExceptionPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.uiCollider = this.TxtList.gameObject.GetComponent<UICollider>();
		this.TextListPanelTrans = this.TextListPanel.transform;
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.BtnCopy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!string.IsNullOrEmpty(this.TxtList.Text))
			{
				Analytics.ShowFB(this.TxtList.Text);
				this.HintPart.AddTextItem(1, Global.GetLang("发送成功"));
				GException.ClearAllExceptionMsgList();
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.BtnPre.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			List<string> exceptionMsgList = GException.GetExceptionMsgList();
			this.CurrentExceptionIndex--;
			this.CurrentExceptionIndex = Global.GMax(0, this.CurrentExceptionIndex);
			if (exceptionMsgList.Count > 0)
			{
				this.TxtList.Text = exceptionMsgList[this.CurrentExceptionIndex];
				this.TxtTitle.Text = StringUtil.substitute(Global.GetLang("总共:{0}, 当前:{1}"), new object[]
				{
					exceptionMsgList.Count,
					this.CurrentExceptionIndex + 1
				});
			}
		};
		this.BtnNext.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			List<string> exceptionMsgList = GException.GetExceptionMsgList();
			this.CurrentExceptionIndex++;
			this.CurrentExceptionIndex = Global.GMin(this.CurrentExceptionIndex, exceptionMsgList.Count - 1);
			if (exceptionMsgList.Count > 0)
			{
				this.TxtList.Text = exceptionMsgList[this.CurrentExceptionIndex];
				this.TxtTitle.Text = StringUtil.substitute(Global.GetLang("总共:{0}, 当前:{1}"), new object[]
				{
					exceptionMsgList.Count,
					this.CurrentExceptionIndex + 1
				});
			}
		};
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.TxtList.Text = string.Empty;
		List<string> exceptionMsgList = GException.GetExceptionMsgList();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = exceptionMsgList.Count - 1; i >= 0; i--)
		{
			stringBuilder.AppendLine(string.Format(Global.GetLang("{0}、{1}\n"), i + 1, exceptionMsgList[i]));
		}
		if (exceptionMsgList.Count > 0)
		{
			this.CurrentExceptionIndex = -1;
			this.TxtList.Text = stringBuilder.ToString();
			this.TxtTitle.Text = StringUtil.substitute(Global.GetLang("总共:{0}条"), new object[]
			{
				exceptionMsgList.Count
			});
		}
		this.RefreshCollider();
	}

	public void RefreshCollider()
	{
		this.uiCollider.UpdataCollider();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public TextBlock TxtTitle;

	public TextBlock TxtList;

	public GButton CloseBtn;

	public GButton BtnPre;

	public GButton BtnNext;

	public GButton BtnCopy;

	public GetGoodsHintPart HintPart;

	public UIPanel TextListPanel;

	private Transform TextListPanelTrans;

	private UICollider uiCollider;

	private int CurrentExceptionIndex;
}
