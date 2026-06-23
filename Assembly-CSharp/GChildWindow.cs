using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GChildWindow : UserControl
{
	protected override void InitializeComponent()
	{
		MyAnchorCamera component = Global.UICamera.GetComponent<MyAnchorCamera>();
		float num = (float)Screen.width;
		float num2 = (float)Screen.height;
		if (null != component)
		{
			num = (((float)Screen.width <= component.suitableUI_width) ? component.suitableUI_width : ((float)Screen.width));
			num2 = (((float)Screen.height <= component.suitableUI_height) ? component.suitableUI_height : ((float)Screen.height));
		}
		this.ModalBak.transform.localScale = new Vector2(num, num2);
		if (null != this.ModalBak)
		{
			UIEventListener.Get(this.ModalBak).onClick = delegate(GameObject s)
			{
				if (this.ChildWindowModalBakClick != null)
				{
					this.ChildWindowModalBakClick(this, EventArgs.Empty);
				}
			};
		}
		GChildWindow.TotalChildWindowCount++;
	}

	private new void Start()
	{
		this.SetModalCamera(false);
	}

	private void OnEnable()
	{
		Global.PlaySoundAudio(this.OpenWinAudioUrl, false);
		if (this.IsCache)
		{
			this.SetModalCamera(false);
		}
	}

	private void OnDisable()
	{
		Global.PlaySoundAudio(this.CloseWinAudioUrl, false);
		this.SetModalCamera(true);
	}

	public bool Modal
	{
		set
		{
			this.ModalBak.SetActive(value);
		}
	}

	public ChildWindowModalType ModalType
	{
		get
		{
			return this._ModalType;
		}
		set
		{
			this._ModalType = value;
			if (value == ChildWindowModalType.None)
			{
				this.ModalBak.SetActive(false);
			}
			else
			{
				if (value == ChildWindowModalType.BlackBak)
				{
					this.ModalBakSprite.spriteName = "black_bak";
					this.ModalBakSprite.enabled = true;
				}
				else if (value == ChildWindowModalType.TransBak)
				{
					this.ModalBakSprite.enabled = false;
				}
				else if (value == ChildWindowModalType.Translucent || value == ChildWindowModalType.TranslucentGUI)
				{
					this.ModalBakSprite.spriteName = "modal_bak";
					this.ModalBakSprite.enabled = true;
				}
				else if (value == ChildWindowModalType.Translucent2)
				{
					this.ModalBakSprite.spriteName = "mainExpBar_bak";
					this.ModalBakSprite.enabled = true;
				}
				this.ModalBak.SetActive(true);
			}
		}
	}

	public bool IsCache { get; set; }

	public SpriteSL BodyPresenter
	{
		get
		{
			return this.Body;
		}
	}

	public string TitleText { get; set; }

	public string ImgTitle { get; set; }

	public int MessageBoxReturn
	{
		get
		{
			return this._MessageBoxReturn;
		}
	}

	public void SetContent(SpriteSL presenter, SpriteSL obj, double left, double top, bool isInitPos = true)
	{
		if (isInitPos)
		{
			obj.X = 0.0;
			obj.Y = 0.0;
		}
		presenter.Add(obj);
	}

	public double Left
	{
		get
		{
			return this.X;
		}
		set
		{
			this.X = value;
		}
	}

	public double Top
	{
		get
		{
			return this.Y;
		}
		set
		{
			this.Y = value;
		}
	}

	public double ZIndex
	{
		get
		{
			return base.Z;
		}
		set
		{
			base.Z = value;
		}
	}

	public double HeadLeft { get; set; }

	public double HeadTop { get; set; }

	public double HeadWidth
	{
		get
		{
			return this._HeadWidth;
		}
		set
		{
			this._HeadWidth = value;
		}
	}

	public double HeadHeight
	{
		get
		{
			return this._HeadHeight;
		}
		set
		{
			this._HeadHeight = value;
		}
	}

	public ImageBrush HeadBackground { get; set; }

	public double BodyLeft { get; set; }

	public double BodyTop { get; set; }

	public double BodyWidth
	{
		get
		{
			return this._BodyWidth;
		}
		set
		{
			this._BodyWidth = value;
		}
	}

	public double BodyHeight
	{
		get
		{
			return this._BodyHeight;
		}
		set
		{
			this._BodyHeight = value;
		}
	}

	public BitmapData BodyBackground { get; set; }

	public double CloseButtonLeft { get; set; }

	public double CloseButtonTop { get; set; }

	public double CloseButtonWidth { get; set; }

	public double CloseButtonHeight { get; set; }

	public BitmapData CloseButtonFill { get; set; }

	public BitmapData CloseButtonTransformFill { get; set; }

	public string CloseButtonTip { get; set; }

	public double LeftBorderWidth { get; set; }

	public double LeftBorderHeight { get; set; }

	public BitmapData LeftBorderFill { get; set; }

	public double RightBorderWidth { get; set; }

	public double RightBorderHeight { get; set; }

	public double RightBorderLeft { get; set; }

	public double BottomBorderWidth { get; set; }

	public double BottomBorderHeight { get; set; }

	public double BottomBorderLeft { get; set; }

	public double BottomBorderTop { get; set; }

	public BitmapData BottomBorderFill { get; set; }

	public double LeftCornerWidth { get; set; }

	public double LeftCornerHeight { get; set; }

	public double LeftCornerLeft { get; set; }

	public double LeftCornerTop { get; set; }

	public BitmapData LeftCornerFill { get; set; }

	public double RightCornerWidth { get; set; }

	public double RightCornerHeight { get; set; }

	public double RightCornerLeft { get; set; }

	public double RightCornerTop { get; set; }

	public BitmapData RightCornerFill { get; set; }

	public Rectangle GetBoundsRect()
	{
		return new Rectangle(0, 0, 0, 0);
	}

	public Point MapPoint
	{
		get
		{
			return this._MapPoint;
		}
		set
		{
			this._MapPoint = value;
		}
	}

	public bool IsOutViewRange(Point p)
	{
		return this.MapPoint.X != 0 && this.MapPoint.Y != 0 && !Global.InCircle(p, this.MapPoint, (double)Global.Data.MaxUnWatchDistance);
	}

	public void NotifyClose(int boxReturn)
	{
		WindowManage.RemoveWindows(this);
		this._MessageBoxReturn = boxReturn;
		if (this.ChildWindowClose != null)
		{
			if (this.ChildWindowClose(this, EventArgs.Empty))
			{
				MUDebug.Log<string>(new string[]
				{
					"YN Debug:enter if(ChildWindowClose)_Super.CloseChildWindow(Children, this)"
				});
				Super.CloseChildWindow(base.Children, this);
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"YN Debug:enter _else_Super.CloseChildWindow()"
			});
			Super.CloseChildWindow(base.Children, this);
		}
	}

	public void ClearChildWindowCloseEvents()
	{
		this.ChildWindowClose = null;
	}

	public double HeadLeftCornerWidth { get; set; }

	public double HeadLeftCornerHeight { get; set; }

	public double HeadLeftCornerLeft { get; set; }

	public double HeadLeftCornerTop { get; set; }

	public BitmapData HeadLeftCornerFill { get; set; }

	public double HeadRightCornerWidth { get; set; }

	public double HeadRightCornerHeight { get; set; }

	public double HeadRightCornerLeft { get; set; }

	public double HeadRightCornerTop { get; set; }

	public BitmapData HeadRightCornerFill { get; set; }

	public double HeadCenterWidth { get; set; }

	public double HeadCenterHeight { get; set; }

	public double HeadCenterLeft { get; set; }

	public double HeadCenterTop { get; set; }

	public BitmapData HeadCenterFill { get; set; }

	public Point WinTouchPos
	{
		set
		{
			this._WinTouchPos = value;
		}
	}

	public bool IsShowModal
	{
		get
		{
			return this._IsShowModal;
		}
		set
		{
			this._IsShowModal = value;
		}
	}

	public static void PushZ(bool isShowModal = false)
	{
		if (GChildWindow.NestStepZs != null)
		{
			if (GChildWindow.NestStepZs.Count <= 0)
			{
				GChildWindow.ZZ += 4;
			}
			else
			{
				GChildWindow.ZZ += GChildWindow.NestStepZs.Peek();
			}
			int num = (!isShowModal) ? 4 : 1000;
			GChildWindow.NestStepZs.Push(num);
		}
	}

	public static void PopZ()
	{
		if (GChildWindow.NestStepZs != null)
		{
			GChildWindow.NestStepZs.Pop();
			int num = 4;
			if (GChildWindow.NestStepZs.Count > 0)
			{
				num = GChildWindow.NestStepZs.Peek();
			}
			GChildWindow.ZZ -= num;
		}
	}

	private void SetModalCamera(bool state)
	{
	}

	public void ResetLayer(Transform trans, string layerName)
	{
		foreach (Transform transform in trans.GetComponentsInChildren<Transform>())
		{
			transform.gameObject.layer = LayerMask.NameToLayer(layerName);
		}
		trans.localPosition = new Vector3(0f, 0f, trans.localPosition.z);
	}

	public const int ModalZ = -500;

	private const int Step4 = 4;

	private const int Step1000 = 1000;

	public static string KaiTi = "KaiTi_GB2312";

	public static GChildWindow CurrentDragWindow = null;

	public static int ZZ = 100;

	public static Stack<int> NestStepZs = new Stack<int>();

	public SpriteSL Body;

	public GameObject ModalBak;

	public UISprite ModalBakSprite;

	private string OpenWinAudioUrl = "Audio/UI/open_win";

	private string CloseWinAudioUrl = "Audio/UI/close_win";

	public static int TotalChildWindowCount = 0;

	private ChildWindowModalType _ModalType;

	private int _MessageBoxReturn = -1;

	private double _HeadWidth;

	private double _HeadHeight;

	private double _BodyWidth;

	private double _BodyHeight;

	private Point _MapPoint = new Point(0, 0);

	public new ChildWindowCloseEventHandler ChildWindowClose;

	public ChildWindowCloseEventHandler ChildWindowModalBakClick;

	private Point _WinTouchPos;

	private bool _IsShowModal;
}
