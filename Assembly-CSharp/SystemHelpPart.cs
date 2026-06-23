using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class SystemHelpPart : UserControl
{
	public override void Destroy()
	{
		this.ClearHintDecoration();
	}

	private void OnClose()
	{
		SystemHelpPart.GClose();
	}

	public static SystemHelpPart GGetInstance()
	{
		if (null == SystemHelpPart._Instance)
		{
			SystemHelpPart._Instance = U3DUtils.NEW<SystemHelpPart>();
		}
		return SystemHelpPart._Instance;
	}

	public static void GClose()
	{
		if (null != SystemHelpPart._Instance)
		{
			Object.Destroy(SystemHelpPart._Instance.gameObject);
			SystemHelpPart._Instance = null;
		}
	}

	public static SystemHelpPart GShow()
	{
		if (null == SystemHelpPart._Instance)
		{
			SystemHelpPart.GGetInstance();
		}
		if (null != SystemHelpPart._Instance)
		{
			SystemHelpPart._Instance.gameObject.SetActive(true);
		}
		return SystemHelpPart._Instance;
	}

	public static void GHide()
	{
		if (null != SystemHelpPart._Instance)
		{
			SystemHelpPart._Instance.gameObject.SetActive(false);
		}
	}

	public SystemHelpPart Show()
	{
		if (null != SystemHelpPart._Instance)
		{
			base.gameObject.SetActive(true);
		}
		return this;
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	public string HintText
	{
		get
		{
			return this._HintText.Text;
		}
		set
		{
			this._HintText.Text = value;
		}
	}

	protected override void InitializeComponent()
	{
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClose);
		this._Skip.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
		};
		this.ImageSubmit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnImageClicked);
		SystemHelpPart.OldZhanLi = 0;
		SystemHelpPart.NewZhanLi = 0;
	}

	protected virtual void OnEnable()
	{
		this._Direction.gameObject.SetActive(false);
		if (this.TargetPosition.sqrMagnitude != 0f)
		{
			base.StartCoroutine_Auto(this.UpdateDirection());
		}
	}

	private void OnClose(object sender, object args)
	{
		this.SetMaskBak(false, default(Bounds));
	}

	public static void ShowDirectionWizard(bool show, [Optional] Vector3 target)
	{
		if (SystemHelpPart._Instance != null)
		{
			SystemHelpPart._Instance._Direction.gameObject.SetActive(show);
			if (show)
			{
				SystemHelpPart._Instance.TargetPosition = target;
				SystemHelpPart._Instance.StartCoroutine_Auto(SystemHelpPart._Instance.UpdateDirection());
			}
		}
	}

	public IEnumerator UpdateDirection()
	{
		while (Mathf.Abs(this.TargetPosition.x - LeaderInfo.LeaderPos.x) + Mathf.Abs(this.TargetPosition.z - LeaderInfo.LeaderPos.z) >= 10f)
		{
			yield return new WaitForSeconds(0.2f);
		}
		SystemHelpPart._Instance._Direction.gameObject.SetActive(false);
		this.TargetPosition = Vector3.zero;
		yield break;
	}

	public void SetMaskBak(bool show, [Optional] Bounds b)
	{
		if (show)
		{
			Vector2 mainUISize = Super.GetMainUISize();
			this._ImageRectMask.transform.localScale = new Vector3(mainUISize.x, 1f, mainUISize.y);
			this._BakTop.transform.localScale = new Vector3(mainUISize.x, mainUISize.y / 2f - b.max.y + 1f, 1f);
			this._BakTop.transform.localPosition = new Vector3(0f, b.max.y - 1f, 0f);
			this._BakBottom.transform.localScale = new Vector3(mainUISize.x, b.min.y + mainUISize.y / 2f + 1f, 1f);
			this._BakBottom.transform.localPosition = new Vector3(0f, b.min.y + 1f, 0f);
			this._BakLeft.transform.localScale = new Vector3(mainUISize.x / 2f + b.min.x, b.size.y + 1f, 1f);
			this._BakLeft.transform.localPosition = new Vector3(b.min.x, b.center.y, 0f);
			this._BakRight.transform.localScale = new Vector3(mainUISize.x / 2f - b.max.x, b.size.y + 1f, 1f);
			this._BakRight.transform.localPosition = new Vector3(b.max.x, b.center.y, 0f);
			this._Panel.gameObject.SetActive(true);
			Vector4 vector;
			vector..ctor(0.5f - b.min.x / mainUISize.x, 0.5f + b.max.y / mainUISize.y, 0.5f - b.max.x / mainUISize.x, 0.5f + b.min.y / mainUISize.y);
			this._ImageRectMask.material.SetVector("_LimitRect", vector);
			this._AnimPos.localPosition = new Vector3(b.center.x, b.center.y, -10f);
			if (100f >= Vector3.Distance(this._AnimPos.localPosition, this._Skip.transform.localPosition))
			{
				this._Skip.transform.localPosition = new Vector3(this._Skip.transform.localPosition.x, 230f, this._Skip.transform.localPosition.z);
			}
			else
			{
				this._Skip.transform.localPosition = new Vector3(this._Skip.transform.localPosition.x, -230f, this._Skip.transform.localPosition.z);
			}
		}
		else
		{
			this._Panel.gameObject.SetActive(false);
		}
	}

	public static bool IsMaskShowing()
	{
		return null != SystemHelpPart._Instance && SystemHelpPart._Instance._Panel.gameObject.activeInHierarchy;
	}

	public static void HideMask()
	{
		if (null != SystemHelpPart._Instance)
		{
			SystemHelpPart.MaskTarget = null;
			SystemHelpPart.Padding = Vector4.zero;
			SystemHelpPart._Instance.SetMaskBak(false, default(Bounds));
			SystemHelpPart.LastMaskPos = SystemHelpPart.InvalidPos;
		}
	}

	public static void SetMask([Optional] Bounds b)
	{
		SystemHelpPart.MaskTarget = null;
		SystemHelpPart.Padding = Vector4.zero;
		if (null != SystemHelpPart._Instance)
		{
			SystemHelpPart._Instance.SetMaskBak(true, b);
		}
	}

	public static void SetMask(Component target, [Optional] Vector4 padding)
	{
		SystemHelpPart.SetMaskEx(target, padding, 0);
	}

	public static void SetMaskEx(Component target, [Optional] Vector4 padding, int type = 0)
	{
		SystemHelpPart.WaitMaskTarget = true;
		SystemHelpPart.MaskTarget = target;
		SystemHelpPart.Padding = padding;
		if (null != SystemHelpPart._Instance)
		{
			SystemHelpPart._Instance._Panel.gameObject.SetActive(true);
			if (type >= SystemHelpPart._Instance._HelpAnim.Length)
			{
				type = 0;
			}
			if (SystemHelpPart._Instance._HelpAnim[type] == null)
			{
				CAnimation canimation = null;
				if (type == 0)
				{
					canimation = U3DUtils.NEW<CAnimation>("AnimClick");
				}
				else if (type == 1)
				{
					canimation = U3DUtils.NEW<CAnimation>("JiNengHuaChu");
				}
				else if (type == 2)
				{
					canimation = U3DUtils.NEW<CAnimation>("AnimArrow2");
				}
				SystemHelpPart._Instance._HelpAnim[type] = canimation;
				U3DUtils.AddChild(SystemHelpPart._Instance._AnimPos.gameObject, canimation.gameObject, true);
			}
			for (int i = 0; i < SystemHelpPart._Instance._HelpAnim.Length; i++)
			{
				if (SystemHelpPart._Instance._HelpAnim[i] != null)
				{
					SystemHelpPart._Instance._HelpAnim[i].Visibility = (i == type);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (null != SystemHelpPart.MaskTarget)
		{
			if (SystemHelpPart.MaskTarget && SystemHelpPart.MaskTarget.gameObject.activeInHierarchy)
			{
				SystemHelpPart.WaitMaskTarget = false;
				if (SystemHelpPart.LastMaskPos != SystemHelpPart.MaskTarget.transform.position)
				{
					Bounds b;
					if (SystemHelpPart.MaskTarget is GGoodIcon)
					{
						Vector3 vector = base.transform.InverseTransformPoint(SystemHelpPart.MaskTarget.transform.position);
						b..ctor(new Vector3(vector.x, vector.y), new Vector3(66f, 66f));
					}
					else if (SystemHelpPart.MaskTarget is GButton)
					{
						GButton gbutton = SystemHelpPart.MaskTarget as GButton;
						b = UIHelper.CalculateRelativeWidgetBounds(SystemHelpPart._Instance.transform, SystemHelpPart.MaskTarget.transform, true);
						if ((b.size.x < 4f || b.size.y < 4f) && null != gbutton.GetComponent<Collider>())
						{
							BoxCollider boxCollider = gbutton.GetComponent<Collider>() as BoxCollider;
							Vector3 size = b.size;
							size.x = boxCollider.size.x + boxCollider.center.x * SystemHelpPart.MaskTarget.transform.localScale.x;
							size.y = boxCollider.size.y + boxCollider.center.y * SystemHelpPart.MaskTarget.transform.localScale.y;
							b.size = size;
						}
					}
					else
					{
						b = UIHelper.CalculateRelativeWidgetBounds(SystemHelpPart._Instance.transform, SystemHelpPart.MaskTarget.transform, true);
					}
					SystemHelpPart.LastMaskPos = SystemHelpPart.MaskTarget.transform.position;
					if (SystemHelpPart.Padding.sqrMagnitude > 0f)
					{
						b..ctor(b.center + new Vector3(SystemHelpPart.Padding.x + SystemHelpPart.Padding.z, SystemHelpPart.Padding.y + SystemHelpPart.Padding.z) / 2f, b.size + new Vector3(SystemHelpPart.Padding.z - SystemHelpPart.Padding.x, SystemHelpPart.Padding.z - SystemHelpPart.Padding.y));
					}
					this.SetMaskBak(true, b);
				}
			}
			else if (!SystemHelpPart.WaitMaskTarget)
			{
				SystemHelpPart.MaskTarget = null;
				SystemHelpPart.HideMask();
				SystemHelpPart.LastMaskPos = SystemHelpPart.InvalidPos;
			}
		}
		if (null != SystemHelpPart.HintTarget)
		{
			if (SystemHelpPart.HintTarget)
			{
				this._HintGo.SetActive(SystemHelpPart.HintTarget.gameObject.activeInHierarchy);
				if (SystemHelpPart.LastHintPos != SystemHelpPart.HintTarget.transform.position)
				{
					SystemHelpPart.LastHintPos = SystemHelpPart.HintTarget.transform.position;
					Bounds bounds = UIHelper.CalculateRelativeWidgetBounds(SystemHelpPart._Instance.transform, SystemHelpPart.HintTarget.transform, true);
					Vector3 targetPos;
					targetPos..ctor(bounds.center.x, bounds.center.y - bounds.extents.y);
					this.ShowHint(true, targetPos, Dircetions.DR_UP);
				}
			}
			else
			{
				SystemHelpPart.HintTarget = null;
				this.ShowHint(false, default(Vector3), Dircetions.DR_UP);
				SystemHelpPart.LastHintPos = SystemHelpPart.InvalidPos;
			}
		}
		if (SystemHelpPart.TimeOut != 0f)
		{
			SystemHelpPart.TimeOut -= Time.deltaTime;
			if (SystemHelpPart.TimeOut < 0.5f && SystemHelpPart.TimeOut > 0f)
			{
				this._HintNoTargetGo.alpha = SystemHelpPart.TimeOut * 2f;
			}
			else if (SystemHelpPart.TimeOut <= 0f)
			{
				SystemHelpPart.TimeOut = 0f;
				this._HintNoTargetText.Text = null;
				this.ShowHintNoTarget(false);
			}
		}
		if (Time.time - SystemHelpPart.LastZhanLiTime >= SystemHelpPart.ZhanLiChangeTime)
		{
			SystemHelpPart.LastZhanLiTime = Time.time;
			if (this._ZhanLiPanel.gameObject.activeInHierarchy)
			{
				if (SystemHelpPart.OldZhanLi == SystemHelpPart.NewZhanLi)
				{
					TweenAlpha tweenAlpha = TweenAlpha.Begin(this._ZhanLiPanel.gameObject, 0.4f, 0f);
					tweenAlpha.delay = 1f;
					tweenAlpha.onFinished = delegate(UITweener tw)
					{
						tw.gameObject.SetActive(false);
					};
					SystemHelpPart.ZhanLiChangeTime = 5f;
				}
				else
				{
					SystemHelpPart.ZhanLiChangeTime = 0.05f;
					int num2;
					int num = Math.DivRem(SystemHelpPart.NewZhanLi, SystemHelpPart.Log10ZhanLi, ref num2);
					int num4;
					int num3 = Math.DivRem(SystemHelpPart.OldZhanLi, SystemHelpPart.Log10ZhanLi, ref num4);
					if (num2 == num4)
					{
						SystemHelpPart.Log10ZhanLi *= 10;
					}
					else if (SystemHelpPart.OldZhanLi < SystemHelpPart.NewZhanLi)
					{
						num4 += SystemHelpPart.Log10ZhanLi / 10;
						num4 %= SystemHelpPart.Log10ZhanLi;
						SystemHelpPart.OldZhanLi = num3 * SystemHelpPart.Log10ZhanLi + num4;
					}
					else
					{
						num4 -= SystemHelpPart.Log10ZhanLi / 10;
						num4 %= SystemHelpPart.Log10ZhanLi;
						SystemHelpPart.OldZhanLi = num3 * SystemHelpPart.Log10ZhanLi + num4;
					}
					this._ZhanLiValue.Text = SystemHelpPart.OldZhanLi.ToString();
					Vector3 localPosition = this._ZhanLiSprite.transform.localPosition;
					localPosition.x = (float)((int)Math.Log10((double)Math.Abs(SystemHelpPart.OldZhanLi))) * 7.5f + 30f;
					this._ZhanLiSprite.transform.localPosition = localPosition;
					Vector3 localPosition2 = this._ZhanLiWord.transform.localPosition;
					localPosition2.x = 0f - (float)((int)Math.Log10((double)Math.Abs(SystemHelpPart.OldZhanLi))) * 7f - 20f;
					this._ZhanLiWord.transform.localPosition = localPosition2;
				}
			}
		}
	}

	private void ShowHintNoTarget(bool show)
	{
		this._HintNoTargetGo.alpha = 1f;
		this._HintNoTargetGo.gameObject.SetActive(show);
	}

	private void ShowHint(bool show, [Optional] Vector3 targetPos, Dircetions dir = Dircetions.DR_UP)
	{
		this._HintGo.SetActive(show);
		if (show)
		{
			this._HintGo.transform.localPosition = targetPos;
		}
	}

	public static void ShowHintText(bool show, string text = null, Component target = null, Dircetions dir = Dircetions.DR_UP)
	{
		SystemHelpPart._Instance.HintText = text;
		if (show)
		{
			SystemHelpPart.HintTarget = target;
			SystemHelpPart.HintDirection = dir;
		}
		else
		{
			SystemHelpPart.HintTarget = null;
			SystemHelpPart.LastHintPos = SystemHelpPart.InvalidPos;
		}
	}

	public static void ShowHintTextNoTarget(bool show, string text = null, int timeOut = 3)
	{
		if (timeOut > 0)
		{
			SystemHelpPart.TimeOut = (float)timeOut + 0.5f;
		}
		else
		{
			SystemHelpPart.TimeOut = 0f;
		}
		SystemHelpPart._Instance._HintNoTargetText.Text = text;
		SystemHelpPart._Instance.ShowHintNoTarget(show);
	}

	private void FixedUpdate()
	{
		if (SystemHelpPart.CountDownSecond > 0)
		{
			if (Time.fixedTime - SystemHelpPart.CountDownStartTime < (float)SystemHelpPart.CountDownElapse)
			{
				return;
			}
			if (this._CountDownAnim.isPlaying)
			{
				return;
			}
			if (Time.fixedTime - SystemHelpPart.CountDownStartTime < (float)SystemHelpPart.CountDownSecond + 0.6f)
			{
				SystemHelpPart.CountDownElapse++;
				int num = Mathf.RoundToInt(SystemHelpPart.CountDownStartTime + (float)SystemHelpPart.CountDownSecond - Time.fixedTime);
				if (num > 0)
				{
					this._CountDownSprite.spriteName = "time" + num.ToString();
					Super.PlayYinDaoSound("CountDown.mp3", true, false);
				}
				else if (SystemHelpPart.CountDownShowGo)
				{
					this._CountDownSprite.spriteName = "FightingStart";
					Super.PlayYinDaoSound("Go.mp3", true, false);
				}
				else
				{
					this._CountDownSprite.spriteName = "none";
				}
				this._CountDownSprite.MakePixelPerfect();
				this._CountDownAnim.Play("DaoJiShi_anim");
			}
			else if (Time.fixedTime - SystemHelpPart.CountDownStartTime > (float)(SystemHelpPart.CountDownSecond + 1))
			{
				this._CountDownPanel.gameObject.SetActive(false);
				SystemHelpPart.CountDownSecond = 0;
				if (SystemHelpPart.CountDownHandler != null)
				{
					SystemHelpPart.CountDownHandler.Invoke(this, EventArgs.Empty);
				}
			}
		}
	}

	public static void ZhanLiChangeTo(int newZhanLi, int oldZhanLi = -1)
	{
		if (Global.Data.roleData.CompletedMainTaskID == 106)
		{
			SystemHelpPart.OldZhanLi = newZhanLi;
			SystemHelpPart.NewZhanLi = newZhanLi;
			return;
		}
		int num = 1 + (int)Math.Log10((double)SystemHelpPart.NewZhanLi);
		SystemHelpPart.Log10ZhanLi = 10;
		SystemHelpPart.ZhanLiChangeTime = 0f;
		SystemHelpPart.LastZhanLiTime = 0f;
		SystemHelpPart.NewZhanLi = newZhanLi;
		if (oldZhanLi >= 0)
		{
			SystemHelpPart.OldZhanLi = oldZhanLi;
			SystemHelpPart._Instance._ZhanLiValue.text = oldZhanLi.ToString();
		}
		if (SystemHelpPart.OldZhanLi != SystemHelpPart.NewZhanLi)
		{
			SystemHelpPart._Instance._ZhanLiEffect.SetActive(false);
			SystemHelpPart._Instance._ZhanLiPanel.alpha = 1f;
			SystemHelpPart._Instance._ZhanLiPanel.gameObject.SetActive(true);
			if (SystemHelpPart.OldZhanLi != SystemHelpPart.NewZhanLi)
			{
				SystemHelpPart._Instance._ZhanLiSprite.gameObject.SetActive(true);
			}
			else
			{
				SystemHelpPart._Instance._ZhanLiSprite.gameObject.SetActive(false);
			}
			if (SystemHelpPart.OldZhanLi < SystemHelpPart.NewZhanLi)
			{
				SystemHelpPart._Instance._ZhanLiSprite.spriteName = "ZhanLiUp";
				SystemHelpPart._Instance._ZhanLiEffect.SetActive(true);
			}
			else
			{
				SystemHelpPart._Instance._ZhanLiSprite.spriteName = "ZhanLiDown";
				SystemHelpPart._Instance._ZhanLiEffect.SetActive(false);
			}
			SystemHelpPart.NewZhanLi -= SystemHelpPart.OldZhanLi;
			SystemHelpPart.OldZhanLi = 0;
		}
	}

	public static void Countdown(int second, EventHandler handler, bool showGo = true)
	{
		if (SystemHelpPart.CountDownSecond != second)
		{
			SystemHelpPart._Instance._CountDownPanel.gameObject.SetActive(true);
			SystemHelpPart.CountDownShowGo = showGo;
			SystemHelpPart.CountDownSecond = second;
			SystemHelpPart.CountDownElapse = 0;
			SystemHelpPart.CountDownStartTime = Time.fixedTime;
			SystemHelpPart.CountDownHandler = handler;
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	private string PrepareText(string text, out List<string> textList, out List<int> textPosList)
	{
		textList = new List<string>();
		textPosList = new List<int>();
		int num = 0;
		int num2 = text.IndexOf('<', num);
		int num3 = 0;
		while (num2 != -1)
		{
			int num4 = text.IndexOf('>', num2);
			if (num4 == -1)
			{
				break;
			}
			string text2 = text.Substring(num2, num4 + 1 - num2);
			textList.Add(Global.StringTrim(text2));
			textPosList.Add(num2 - num3);
			num3 += text2.Length;
			num = num4 + 1;
			num2 = text.IndexOf('<', num);
		}
		for (int i = 0; i < textList.Count; i++)
		{
			text = Global.StringReplaceAll(text, textList[i], string.Empty);
		}
		return text;
	}

	private void FormatText(string text)
	{
		List<string> list = null;
		List<int> list2 = null;
		text = this.PrepareText(text, out list, out list2);
		List<SystemHelpKeysData> list3 = new List<SystemHelpKeysData>();
		if (list != null && list2 != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				string text2 = list[i];
				string[] array = text2.Split(new char[]
				{
					'"'
				});
				if (array.Length == 3)
				{
					string text3 = array[1];
					string text4 = StringUtil.substitute("Images/keys/{0}", new object[]
					{
						text3
					});
					text4 = Global.StringReplaceAll(text4, "/", "_");
					text4 = Global.StringReplaceAll(text4, ".", "_");
					text4 = Global.GameResPath(text4);
					text4 = "A_" + text4;
					list3.Add(new SystemHelpKeysData
					{
						ID = list2[i],
						Deco1 = text4
					});
				}
			}
		}
		if (list3.Count <= 0)
		{
		}
	}

	public bool InitPartData(int mode)
	{
		return false;
	}

	public void InitPart(ISystemHelpPart target, int id)
	{
		Component component = target as Component;
		if (null != component)
		{
			U3DUtils.AddChild(component.gameObject, this._Panel.gameObject, false);
			this._Panel.gameObject.SetActive(false);
		}
	}

	public void HidePart(ISystemHelpPart target, int id)
	{
		Component component = target as Component;
		if (null != component)
		{
			this._Panel.gameObject.SetActive(false);
		}
	}

	public void ShowPart(ISystemHelpPart target, int id)
	{
		Component component = target as Component;
		if (null != component)
		{
			this._Panel.gameObject.SetActive(true);
		}
	}

	public static void ShowDiagram(string url, int taskID = -1, bool show = true)
	{
		SystemHelpPart.TaskID = taskID;
		if (!string.IsNullOrEmpty(url))
		{
			int num = (!show) ? 4000 : 0;
			Vector3 localPosition;
			localPosition..ctor((float)num, 0f, -420f);
			SystemHelpPart._Instance.ImagePanel.transform.localPosition = localPosition;
			SystemHelpPart._Instance.ImagePanel.gameObject.SetActive(true);
			SystemHelpPart._Instance.Image.URL = url;
		}
		else
		{
			SystemHelpPart._Instance.ImagePanel.gameObject.SetActive(false);
			SystemHelpPart._Instance.Image.URL = null;
		}
	}

	private static int TaskID { get; set; }

	private void OnImageClicked(object s, MouseEvent e)
	{
		if (SystemHelpPart.TaskID == 101)
		{
			SystemHelpPart.Countdown(3, delegate(object s0, EventArgs e0)
			{
				Super.PlayYinDaoSound("xinshouyindao2.mp3", false, false);
				Global.Data.GameScene.SetGuangMuState(1, 0, false, -1);
				Global.Data.GameScene.AddYinDaoZhiYin(new Vector2(22f, 62.5f), new Vector2(51f, 62.5f));
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("请按照箭头移动离开安全区"), 3);
			}, true);
		}
		SystemHelpPart.ShowDiagram(null, -1, true);
	}

	private static SystemHelpPart _Instance = null;

	public GButton _Close;

	public GButton _Skip;

	public UIPanel _Panel;

	public BoxCollider _BakTop;

	public BoxCollider _BakBottom;

	public BoxCollider _BakLeft;

	public BoxCollider _BakRight;

	public MeshRenderer _ImageRectMask;

	public UIPanel _Direction;

	public UISprite _GO;

	public UISprite _TO;

	public CAnimation[] _HelpAnim;

	public Transform _AnimPos;

	public UIPanel _HintNoTargetGo;

	public TextBlock _HintNoTargetText;

	public static float TimeOut = 0f;

	public GameObject _HintGo;

	public TextBlock _HintText;

	public UIPanel _ZhanLiPanel;

	public UISprite _ZhanLiSprite;

	public UISprite _ZhanLiWord;

	public TextBlock _ZhanLiValue;

	public GameObject _ZhanLiEffect;

	private static int Log10ZhanLi;

	private static int OldZhanLi;

	private static int NewZhanLi;

	public UIPanel _CountDownPanel;

	public UISprite _CountDownSprite;

	public Animation _CountDownAnim;

	public GameObject ImagePanel;

	public ShowNetImage Image;

	public GButton ImageSubmit;

	public EventHandler ToClose;

	public List<int> WaittingModes;

	public SystemHelpPartStates State;

	public ISystemHelpPart mPlayZone;

	private static Component MaskTarget = null;

	private static Vector4 Padding;

	private static bool WaitMaskTarget = true;

	private static Component HintTarget = null;

	private static Dircetions HintDirection = Dircetions.DR_UP;

	private Vector3 TargetPosition = Vector3.zero;

	private static Vector3 InvalidPos = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

	private static Vector3 LastMaskPos = SystemHelpPart.InvalidPos;

	private static Vector3 LastHintPos = SystemHelpPart.InvalidPos;

	private static float ZhanLiChangeTime = 0f;

	private static float LastZhanLiTime = 0f;

	private static float CountDownStartTime = 0f;

	private static int CountDownSecond = 0;

	private static int CountDownElapse = 0;

	private static bool CountDownShowGo = true;

	private static EventHandler CountDownHandler;
}
