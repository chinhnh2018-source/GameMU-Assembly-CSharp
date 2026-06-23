using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AngelTempleAwardPart2 : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnOk.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.ItemList.ItemsSource;
		this.BtnOk.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
	}

	public void InitPartData(int paiMing, int awardGold, int awardShengWang, string luckPaiMingName, string goodsString, long startTick = 0L, bool success = true)
	{
		this.StartTicks = startTick;
		if (this.StartTicks > Global.GetCorrectLocalTime())
		{
			this.DelayShow = true;
			this.ShowMainPart(false);
		}
		else
		{
			this.DelayShow = false;
			this.ShowMainPart(true);
		}
		this.TimeLimit = 20L;
		this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit);
		this.ShowAnim(success);
		string text;
		if (paiMing > 0)
		{
			text = string.Format(Global.GetLang("伤害排名：{0}        "), paiMing);
		}
		else
		{
			text = Global.GetLang("伤害排名：无");
		}
		if (awardShengWang > 0)
		{
			text += string.Format(Global.GetLang("        声望：{0}"), awardShengWang);
		}
		if (!string.IsNullOrEmpty(luckPaiMingName) && luckPaiMingName.Length > 2)
		{
			text += string.Format("        {0}", luckPaiMingName);
		}
		this.TxtName.Text = text;
		this.ItemCollection.Clear();
		UIHelper.AddAwardGoods(this.ItemCollection, goodsString);
		Transform transform = this.ItemList.transform;
		transform.localPosition = this.PosX(transform.localPosition, (float)(Mathf.Max(this.ItemList.Count() - 1, 0) * -39));
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			long second = (Global.GetCorrectLocalTime() - this.StartTicks) / 1000L;
			if (second > 0L)
			{
				if (this.DelayShow)
				{
					this.ShowMainPart(true);
				}
				if (this.TimeLimit > second)
				{
					this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit - second);
				}
				else
				{
					this.OnSubmit(null, null);
				}
			}
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	public void ShowMainPart(bool show)
	{
		if (show)
		{
			if (this.DelayShow)
			{
				this.DelayShow = false;
				this.StartTicks = Global.GetCorrectLocalTime();
			}
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void ShowAnim(bool success)
	{
		this.Anim[0].gameObject.SetActive(false);
		this.Anim[1].gameObject.SetActive(false);
		if (success)
		{
			this.Anim[0].gameObject.SetActive(true);
		}
		else
		{
			this.Anim[1].gameObject.SetActive(true);
		}
	}

	private Vector3 PosX(Vector3 v, float x)
	{
		v.x = x;
		return v;
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(null, null);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox ItemList;

	public GButton BtnOk;

	public TextBlock TxtTime;

	public TextBlock TxtName;

	public Animator[] Anim;

	private float TickInterval = 0.25f;

	private long StartTicks;

	private long TimeLimit = 60L;

	private bool DelayShow;

	private ObservableCollection _ItemCollection;
}
