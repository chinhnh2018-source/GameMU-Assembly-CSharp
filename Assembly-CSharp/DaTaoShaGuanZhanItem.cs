using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DaTaoShaGuanZhanItem : UserControl
{
	private int ID { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitEvent();
	}

	private void InitEvent()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			if (this.ClickHandler != null)
			{
				this.ClickHandler.Invoke(this.ID);
			}
		};
	}

	public void InitValue(GuanZhanRoleMiniData data)
	{
		this.ID = data.RoleID;
		this.LblName.Text = data.Name;
		this.LblKillNum.Text = data.Param1.ToString();
		this.LblStatus.Text = ((data.Param2 != 1) ? Global.GetLang("死亡") : Global.GetLang("存活"));
		this.status = data.Param2;
	}

	public bool IsSelect
	{
		set
		{
			this.selectSprt.transform.gameObject.SetActive(value);
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public Action<int> ClickHandler;

	public TextBlock LblName;

	public TextBlock LblKillNum;

	public TextBlock LblStatus;

	public UISprite selectSprt;

	public int status = -1;
}
