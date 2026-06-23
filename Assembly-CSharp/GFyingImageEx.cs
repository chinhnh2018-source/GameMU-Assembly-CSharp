using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GFyingImageEx : UserControl
{
	public Component Target
	{
		get
		{
			return this._Target;
		}
		set
		{
			this._Target = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.MainBak.transform.localScale = Super.GetScreenSize();
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
	}

	private void OnSubmit(object sender, MouseEvent args)
	{
		if (this.uiReady && !this.start)
		{
			this.start = true;
			this.MainPanel.SetActive(false);
			this.MainBak.gameObject.SetActive(false);
			this.FlyToTarget();
		}
	}

	public void InitPart()
	{
		this.MainBak.gameObject.SetActive(WindowManage.IsHaveWindow());
		this.MainPanel.SetActive(true);
		this._ImageOne.URL = Global.GetGameResHybridString(this.ImageOne);
		this._ImageTwo.URL = Global.GetGameResHybridString(this.ImageTwo);
		SystemHelpMgr.DoAction(3000, this.IconID);
	}

	public new IEnumerator Start()
	{
		yield return new WaitForSeconds(10f);
		this.OnSubmit(this._Submit, MouseEvent.Empty);
		yield break;
	}

	public void OnUIReady()
	{
		this.uiReady = true;
		if (!string.IsNullOrEmpty(this.Music))
		{
			Global.CloseNPCAllSound(0);
			Super.PlayYinDaoSound(this.Music, false, false);
		}
	}

	public void FlyToTarget()
	{
		Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventMenuIconBoxAddIcon", this.IconID));
		if (null != this.Target)
		{
			TweenPosition tweenPosition = TweenPosition.Begin(this.FlyPanel, 0.75f, base.transform.InverseTransformPoint(this._Target.transform.position));
			tweenPosition.onFinished = new UITweener.OnFinished(this.OnMoveFinished);
		}
		else if (this.DPSelectItem != null)
		{
			this.DPSelectItem.Invoke(this, EventArgs.Empty);
		}
	}

	public void OnMoveFinished(UITweener tween)
	{
		if (null != this._Animation)
		{
			this._Animation.Play();
			if (this.DPSelectItem != null)
			{
				base.StartCoroutine<bool>(this.AnimationState());
			}
		}
	}

	private IEnumerator AnimationState()
	{
		while (this._Animation.isPlaying)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.DPSelectItem.Invoke(this, EventArgs.Empty);
		yield break;
		yield break;
	}

	public UISprite MainBak;

	public UISprite _Bak;

	public ShowNetImage _ImageOne;

	public ShowNetImage _ImageTwo;

	public GButton _Submit;

	public TextBlock _Text;

	public GameObject MainPanel;

	public GameObject FlyPanel;

	public Animation _Animation;

	public NetAudioSource _NetAudioSource;

	[NonSerialized]
	public EventHandler DPSelectItem;

	[NonSerialized]
	public int GongNengID;

	[NonSerialized]
	public int ID;

	[NonSerialized]
	public int PostWizardID;

	[NonSerialized]
	public int IconID;

	[NonSerialized]
	public string ImageOne;

	[NonSerialized]
	public string ImageTwo;

	[NonSerialized]
	public string Music;

	private bool start;

	private bool uiReady;

	private Component _Target;
}
