using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LuolanchengzhanResultPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.staticTxt[0].text = Global.GetLang("战功奖励:");
		this.staticTxt[1].text = Global.GetLang("奖励经验:");
		this.btnFinish.Text = Global.GetLang("离开");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (this.bak)
		{
			this.bak.localScale = Super.GetScreenSize();
		}
		this.btnFinish.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		};
		this.countDown = Global.GetLuolanchengzhanClearSecs();
		base.InvokeRepeating("TimeProc", 0f, 1f);
	}

	public void Init(LuoLanChengZhanResultInfo item)
	{
		this.lblZhangongJiangli.text = string.Empty + item.ZhanGongAward;
		this.lblJingyanJiangli.text = string.Empty + item.ExpAward;
		this.lblHuoshengZhanmeng.text = "[" + item.BHName + Global.GetLang("]占领罗兰城堡");
		if (item.BHID == Global.Data.roleData.Faction)
		{
			this.AnimWin.gameObject.SetActive(true);
		}
		else
		{
			this.AnimLose.gameObject.SetActive(true);
		}
	}

	protected void TimeProc()
	{
		if (this.countDown < 0)
		{
			this.lblTime.gameObject.SetActive(false);
			base.CancelInvoke("TimeProc");
			Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
			{
				NpcID = 1000000,
				ScriptID = 10,
				Hint = 0
			}));
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
		this.lblTime.text = StringUtil.substitute("{0}" + Global.GetLang("秒后关闭"), new object[]
		{
			this.countDown
		});
		this.countDown--;
	}

	public UILabel lblZhangongJiangli;

	public UILabel lblJingyanJiangli;

	public UILabel lblHuoshengZhanmeng;

	public UILabel lblTime;

	public GButton btnFinish;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int countDown = 10;

	public Animator AnimWin;

	public Animator AnimLose;

	public TextBlock[] staticTxt;

	public Transform bak;
}
