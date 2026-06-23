using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GGoodIcon : GTipSprite
{
	public int OutSizeX { get; set; }

	public int OutSizeY { get; set; }

	protected override void InitializeComponent()
	{
		this._Collider = base.GetComponent<BoxCollider>();
		base.addEventListener("mouseUp", new MouseEventHandler(this.mouseEventLeftButtonUp));
		NGUITools.SetActive(this.BackgroundSprite0, false);
		NGUITools.SetActive(this.BackgroundSprite1, false);
		NGUITools.SetActive(this.BackgroundSprite15, false);
		NGUITools.SetActive(this.BackgroundSprite2, false);
		NGUITools.SetActive(this.BindingSprite, false);
		NGUITools.SetActive(this.NoUseSprite, false);
		NGUITools.SetActive(this.ZhanLiSprite, false);
		NGUITools.SetActive(this.EndTimeSprite, false);
	}

	public void ResetUI()
	{
		NGUITools.SetActive(this.BackgroundSprite0, false);
		NGUITools.SetActive(this.BackgroundSprite1, false);
		NGUITools.SetActive(this.BackgroundSprite15, false);
		NGUITools.SetActive(this.BackgroundSprite2, false);
		NGUITools.SetActive(this.BindingSprite, false);
		NGUITools.SetActive(this.NoUseSprite, false);
		NGUITools.SetActive(this.ZhanLiSprite, false);
		NGUITools.SetActive(this.EndTimeSprite, false);
		if (this.TeXiao != null)
		{
			this.TeXiao.gameObject.SetActive(false);
		}
	}

	private void mouseEventLeftButtonUp(MouseEvent evt)
	{
		if (this.MouseLeftButtonUp != null)
		{
			this.MouseLeftButtonUp(this, evt);
		}
	}

	public double LastMouseDownTicks
	{
		get
		{
			return this._LastMouseDownTicks;
		}
		set
		{
			this._LastMouseDownTicks = value;
		}
	}

	public GGoodIcon Clone()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = this.Width;
		ggoodIcon.Height = this.Height;
		ggoodIcon.EnableIcon = this.EnableIcon;
		ggoodIcon.Tip = base.Tip;
		ggoodIcon.TipType = base.TipType;
		ggoodIcon.Tag = this.Tag;
		ggoodIcon.ItemCode = this.ItemCode;
		ggoodIcon.ItemObject = this.ItemObject;
		ggoodIcon.BoxTypes = this.BoxTypes;
		ggoodIcon.Text = this.Text;
		ggoodIcon.TextColor = this.TextColor;
		return ggoodIcon;
	}

	private void AutoCollider()
	{
		this._Collider.isTrigger = true;
		this._Collider.center = Vector3.zero;
		this._Collider.size = new Vector3((float)this.Width, (float)this.Height, 0f);
	}

	private void AutoImgScale()
	{
		if (null != this.GoodImg)
		{
			this.GoodImg.Width = this.Width;
			this.GoodImg.Height = this.Height;
		}
		if (null != this.BackgroundImg)
		{
			this.BackgroundImg.Width = this.Width;
			this.BackgroundImg.Height = this.Height;
		}
	}

	public ImageURL BodyURL
	{
		get
		{
			return this._BodyURL;
		}
		set
		{
			this._BodyURL = value;
			if (value != null)
			{
				this.GoodImg.URL = this._BodyURL.ImageSource;
				this.GoodImg.ImageDownloaded = delegate(object s)
				{
					if (this.isAutoSize)
					{
						Transform transform = this.GoodImg.transform;
						if (this.GoodImg.ItsSizeWidth % 2 != 0)
						{
							this.GoodImg.transform.localPosition = new Vector3(0.5f, transform.localPosition.y, transform.localPosition.z);
						}
						if (this.GoodImg.ItsSizeHeight % 2 != 0)
						{
							this.GoodImg.transform.localPosition = new Vector3(transform.localPosition.x, -0.5f, transform.localPosition.z);
						}
						this.GoodImg.Width = (double)this.GoodImg.ItsSizeWidth;
						this.GoodImg.Height = (double)this.GoodImg.ItsSizeHeight;
						this.Width = (double)this.GoodImg.ItsSizeWidth;
						this.Height = (double)this.GoodImg.ItsSizeHeight;
						this._Collider.size = new Vector3((float)this.GoodImg.ItsSizeWidth, (float)this.GoodImg.ItsSizeHeight, 0f);
						float num = (float)this.GoodImg.ItsSizeWidth;
						float num2 = (float)this.GoodImg.ItsSizeHeight;
						if (this.isAutoInnerSize)
						{
							this.BackgroundSprite1.transform.localScale = new Vector3(num - 2f, num2 - 2f, 1f);
							this.BackgroundSprite2.transform.localScale = new Vector3(num, num2, 1f);
						}
						if (this.isAutoItemPos)
						{
							this.BindingSprite.transform.localPosition = new Vector3(-(num / 2f - 14f), -(num2 / 2f - 14f), -0.5f);
							this.ContentText.transform.localPosition = new Vector3(num / 2f - 4f, num2 / 2f - 14f, -2f);
							this.EndTimeSprite.transform.localPosition = new Vector3(-(num / 2f - 12f), -(num2 / 2f - 14f), -0.03f);
						}
						if (this.DPImageDownloadedItem != null)
						{
							this.DPImageDownloadedItem(this, new DPSelectedItemEventArgs
							{
								ID = this.GoodImg.ItsSizeWidth,
								Type = this.GoodImg.ItsSizeHeight
							});
						}
					}
				};
				this.GoodImg.ImageDownloadedErr = new URLImage.ImageDownloadedEventHandler(this.GoodImg.ShowDownloadedErrImage);
				if (!this.isAutoSize)
				{
					this.AutoImgScale();
					this.AutoCollider();
				}
			}
		}
	}

	public ImageURL BackURL
	{
		get
		{
			return this._BackURL;
		}
		set
		{
			this._BackURL = value;
			if (value != null)
			{
				this.BackgroundImg.URL = this._BackURL.ImageSource;
				this.BackgroundImg.ImageDownloaded = delegate(object s)
				{
					if (this.isAutoSize)
					{
						float num = (float)this.BackgroundImg.ItsSizeWidth;
						float num2 = (float)this.BackgroundImg.ItsSizeHeight;
						this.BackgroundImg.Width = (double)num;
						this.BackgroundImg.Height = (double)num2;
						this._Collider.size = new Vector3(num, num2, 0f);
					}
				};
				this.AutoCollider();
				this.AutoImgScale();
			}
		}
	}

	public string BackSpriteName0
	{
		get
		{
			return this.BackgroundSprite0.spriteName;
		}
		set
		{
			this.BackgroundSprite0.spriteName = value;
			if (this.OutSizeX > 0 && this.OutSizeY > 0)
			{
				Vector3 localScale = this.BackgroundSprite0.transform.localScale;
				localScale.x = (float)this.OutSizeX;
				localScale.y = (float)this.OutSizeY;
				this.BackgroundSprite0.transform.localScale = localScale;
			}
			NGUITools.SetActive(this.BackgroundSprite0, true);
		}
	}

	public string BackSpriteName1
	{
		get
		{
			return this.BackgroundSprite1.spriteName;
		}
		set
		{
			this.BackgroundSprite1.spriteName = value;
			if (this.Width > 0.0 && this.Height > 0.0)
			{
				Vector3 localScale = this.BackgroundSprite1.transform.localScale;
				localScale.x = (float)((int)this.Width);
				localScale.y = (float)((int)this.Height);
				this.BackgroundSprite1.transform.localScale = localScale;
			}
			NGUITools.SetActive(this.BackgroundSprite1, true);
		}
	}

	public string BackSpriteName15
	{
		get
		{
			return this.BackgroundSprite15.spriteName;
		}
		set
		{
			if (null == this.BackgroundSprite15)
			{
				return;
			}
			this.BackgroundSprite15.spriteName = value;
			if (this.Width > 0.0 && this.Height > 0.0)
			{
				Vector3 localScale = this.BackgroundSprite1.transform.localScale;
				localScale.x = (float)((int)this.Width);
				localScale.y = (float)((int)this.Height);
				this.BackgroundSprite15.transform.localScale = localScale;
				if (base.gameObject.GetComponentInParent<ParcelPart>())
				{
					localScale.x -= 5f;
					localScale.y -= 5f;
					this.BackgroundSprite15.transform.localScale = localScale;
				}
				this.GoodImg.transform.localPosition = new Vector3(this.GoodImg.transform.localPosition.x, this.GoodImg.transform.localPosition.y, -1f);
			}
			NGUITools.SetActive(this.BackgroundSprite15, true);
		}
	}

	public string BackSpriteName2
	{
		get
		{
			return this.BackgroundSprite2.spriteName;
		}
		set
		{
			this.BackgroundSprite2.spriteName = value;
			NGUITools.SetActive(this.BackgroundSprite2, true);
			if (Global.IsShengqi(this._ItemObject as GoodsData) && this.BackgroundSprite2.spriteName.Equals("iconState_sell"))
			{
				this.BackgroundSprite2.transform.localPosition = new Vector3(this.BackgroundSprite2.transform.localPosition.x, this.BackgroundSprite2.transform.localPosition.y, -2f);
			}
			else
			{
				this.BackgroundSprite2.transform.localPosition = new Vector3(this.BackgroundSprite2.transform.localPosition.x, this.BackgroundSprite2.transform.localPosition.y, -0.02f);
			}
		}
	}

	public bool BackgroundSprite1Visible
	{
		set
		{
			this.BackgroundSprite1.gameObject.SetActive(value);
		}
	}

	public bool NoUseSpriteVisible
	{
		set
		{
			this.NoUseSprite.gameObject.SetActive(value);
		}
	}

	public int ItemCategory
	{
		get
		{
			return this._ItemCategory;
		}
		set
		{
			this._ItemCategory = value;
		}
	}

	public int ItemCode
	{
		get
		{
			return this._ItemCode;
		}
		set
		{
			this._ItemCode = value;
		}
	}

	public object ItemObject
	{
		get
		{
			return this._ItemObject;
		}
		set
		{
			this._ItemObject = value;
			this.Text = string.Empty;
			GoodsData goodsData = this._ItemObject as GoodsData;
			if (goodsData != null)
			{
				this.SetGoodsData(goodsData);
			}
		}
	}

	private void SetGoodsData(GoodsData data)
	{
		if (data != null)
		{
			this.NumType = GGoodIcon.GoodsNumType.Normal;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				this.TextColor = this._NormalTextColor;
				this.TextSize = this.NormalSize;
				this.GetExecMagicKeyAndVal(goodsXmlNodeByID.ExecMagic, out empty, out empty2);
				if (empty != null && empty2 != null)
				{
					string text = empty;
					if (text != null)
					{
						if (GGoodIcon.<>f__switch$map3 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(20);
							dictionary.Add("NEW_ADD_YINLIANG", 0);
							dictionary.Add("NEW_ADD_MONEY", 0);
							dictionary.Add("ADD_DJ", 0);
							dictionary.Add("ADD_BINDYUANBAO", 0);
							dictionary.Add("ADD_XINGHUN", 0);
							dictionary.Add("NEW_PACK_JINGYUAN", 0);
							dictionary.Add("NEW_ADD_CHENGJIU", 0);
							dictionary.Add("ADD_SHENGWANG", 0);
							dictionary.Add("ADD_VIPEXP", 0);
							dictionary.Add("ADDYSFM", 0);
							dictionary.Add("ADD_LINGJING", 0);
							dictionary.Add("ADD_BANGGONG", 0);
							dictionary.Add("ADD_LANGHUN", 0);
							dictionary.Add("ADD_ZAIZAO", 0);
							dictionary.Add("ADD_SHENLIJINGHUA", 0);
							dictionary.Add("ADD_MEILIDIANSHU", 0);
							dictionary.Add("ADD_JINGLINGSHENJI", 0);
							dictionary.Add("ADD_FUWENZHICHEN", 0);
							dictionary.Add("ADD_HUNJING", 0);
							dictionary.Add("DYNAMIC_COUNT", 1);
							GGoodIcon.<>f__switch$map3 = dictionary;
						}
						int num;
						if (GGoodIcon.<>f__switch$map3.TryGetValue(text, ref num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									this.NumType = GGoodIcon.GoodsNumType.Specail;
									if (data.GCount > 1)
									{
										this.Text = this.GetShowVal(data.GCount.ToString());
										this.ContentText.textColor = 13480843U;
										this.ContentText.FontSize = 14;
									}
								}
							}
							else
							{
								this.Text = empty2;
								this.ContentText.textColor = 13480843U;
								this.ContentText.FontSize = 14;
							}
						}
					}
				}
			}
		}
	}

	private void GetExecMagicKeyAndVal(string execMagic, out string val1, out string val2)
	{
		int num = execMagic.IndexOf("(");
		int num2 = execMagic.LastIndexOf(")");
		if (num != -1 && num2 != -1)
		{
			val1 = execMagic.Substring(0, num);
			val2 = this.GetShowVal(execMagic.Substring(num + 1, num2 - num - 1));
		}
		else
		{
			val1 = null;
			val2 = null;
		}
	}

	public string GetShowVal(string val)
	{
		string empty = string.Empty;
		return val;
	}

	public int BoxTypes
	{
		get
		{
			return this._BoxTypes;
		}
		set
		{
			this._BoxTypes = value;
		}
	}

	public int ItemNum
	{
		get
		{
			return this._ItemNum;
		}
		set
		{
			this._ItemNum = value;
		}
	}

	public new bool EnableIcon
	{
		get
		{
			return this._EnableIcon;
		}
		set
		{
			this._EnableIcon = value;
			if (this._EnableIcon)
			{
				this.GoodImg.ToGrayBitmap = false;
				this.ContentText.textColor = this._NormalTextColor;
			}
			else
			{
				this.GoodImg.ToGrayBitmap = true;
				this.ContentText.textColor = this._DisableTextColor;
			}
		}
	}

	public bool EnableEvent
	{
		set
		{
			if (this._Collider == null)
			{
				this._Collider = base.GetComponent<BoxCollider>();
			}
			this._Collider.enabled = value;
		}
	}

	public int PaddingX
	{
		get
		{
			return this._PaddingX;
		}
		set
		{
			this._PaddingX = value;
		}
	}

	public int PaddingY
	{
		get
		{
			return this._PaddingY;
		}
		set
		{
			this._PaddingY = value;
		}
	}

	public void RefreshIconPos(int type = -1)
	{
		Vector3 localPosition = this.BindingSprite.transform.localPosition;
		int num = 0;
		if (this.BindingSprite.gameObject.activeSelf)
		{
			num++;
		}
		if (type == 0)
		{
			this.NoUseSprite.spriteName = "iconState_nouse2";
			this.NoUseSprite.transform.transform.localScale = new Vector3(64f, 64f, 1f);
			this.NoUseSprite.transform.localPosition = new Vector3(0f, 0f, localPosition.z);
			num++;
		}
		else if (type == 1)
		{
			this.NoUseSprite.spriteName = "iconState_nouse1";
			this.NoUseSprite.transform.transform.localScale = new Vector3(24f, 24f, 1f);
			this.NoUseSprite.transform.localPosition = new Vector3(localPosition.x, localPosition.y + (float)(num * 20), localPosition.z);
			num++;
		}
	}

	public int TimerTicks
	{
		get
		{
			return this._TimerTicks;
		}
		set
		{
			this._TimerTicks = value;
		}
	}

	public bool OldEnableHint
	{
		get
		{
			return this._OldEnableHint;
		}
		set
		{
			this._OldEnableHint = value;
		}
	}

	public bool EnableHint
	{
		get
		{
			return this._EnableHint;
		}
		set
		{
			this._EnableHint = value;
			this.HintState = 0;
			if (this._EnableHint)
			{
				if (this._HintTimer != null)
				{
					this._HintTimer.Tick = null;
					this._HintTimer.Stop();
					this._HintTimer = null;
				}
				this._HintTimer = new DispatcherTimer("GIcon_HintTimer");
				if (this._TimerTicks == 0)
				{
					this._TimerTicks = 400;
				}
				this._HintTimer.Interval = TimeSpan.FromMilliseconds((double)this._TimerTicks);
				this._HintTimer.Tick = new DispatcherTimerEventHandler(this.HintTimer_Tick);
				this._HintTimer.Start();
			}
			else if (this._HintTimer != null)
			{
				this._HintTimer.Tick = null;
				this._HintTimer.Stop();
				this._HintTimer = null;
			}
		}
	}

	private void HintTimer_Tick(object sender, object e)
	{
		this.ProcessHint();
	}

	private void ProcessHint()
	{
		if (!this.EnableHint)
		{
			return;
		}
		if (!this.EnableIcon)
		{
			return;
		}
		IconTypes iconType = this.IconType;
		if (iconType == IconTypes.Transform)
		{
			if (this.HintState == 0)
			{
				this.GoodImg.Source = this._GoodImg;
			}
			else
			{
				this.GoodImg.Source = null;
			}
			this.HintState = ((this.HintState <= 0) ? 1 : 0);
		}
	}

	private static BitmapData GetHintBitmapImage(string name)
	{
		if (GGoodIcon._HintImageDict.ContainsKey(name))
		{
			return GGoodIcon._HintImageDict.GetValue(name);
		}
		BitmapData gameResImage = Global.GetGameResImage(name);
		GGoodIcon._HintImageDict[name] = gameResImage;
		return gameResImage;
	}

	public string TextHorizontalAlignment
	{
		get
		{
			return this._TextHorizontalAlignment;
		}
		set
		{
			this._TextHorizontalAlignment = value;
			this.RepositionContentText(this.ContentText, this._TextHorizontalAlignment, this._TextVerticalAlignment);
		}
	}

	public string TextVerticalAlignment
	{
		get
		{
			return this._TextVerticalAlignment;
		}
		set
		{
			this._TextVerticalAlignment = value;
			this.RepositionContentText(this.ContentText, this._TextHorizontalAlignment, this._TextVerticalAlignment);
		}
	}

	private void RepositionContentText(TextBlock textBlock, string textHorizontalAlignment, string TextVerticalAlignment)
	{
		if (null != this.GoodImg && !string.IsNullOrEmpty(textBlock.text))
		{
			if (textHorizontalAlignment == global::Layout.Left)
			{
				textBlock.Pivot = 3;
				textBlock.X = -Math.Floor(this.Width / 2.0) + (double)this.PaddingX;
			}
			else if (textHorizontalAlignment == global::Layout.Center)
			{
				textBlock.Pivot = 4;
				textBlock.X = 0.0;
			}
			else
			{
				textBlock.Pivot = 5;
				textBlock.X = Math.Floor(this.Width / 2.0) - (double)this.PaddingX;
			}
			if (TextVerticalAlignment == global::Layout.Top)
			{
				textBlock.Pivot = 1;
				textBlock.Y = this.Height / 2.0 - (double)this.PaddingY;
			}
			else if (TextVerticalAlignment == global::Layout.Center)
			{
				textBlock.Pivot = 4;
				textBlock.Y = 0.0;
			}
			else if (TextVerticalAlignment == global::Layout.Bottom)
			{
				textBlock.Pivot = 7;
				textBlock.Y = -Math.Floor(this.Height / 2.0) + (double)this.PaddingY;
			}
		}
	}

	public string Text
	{
		get
		{
			return this.ContentText.text;
		}
		set
		{
			if (this.ContentText != null)
			{
				this.ContentText.text = value;
			}
		}
	}

	public uint TextColor
	{
		get
		{
			return this.ContentText.textColor;
		}
		set
		{
			this.ContentText.textColor = value;
			this._NormalTextColor = value;
		}
	}

	public uint DisableTextColor
	{
		set
		{
			this._DisableTextColor = value;
		}
	}

	public int TextSize
	{
		set
		{
			this.NormalSize = value;
			if (this.NumType == GGoodIcon.GoodsNumType.Normal)
			{
				this.ContentText.FontSize = value;
			}
		}
	}

	public uint TextShadowColor
	{
		get
		{
			return this.ContentText.fontBorder;
		}
		set
		{
			this.ContentText.fontBorder = value;
		}
	}

	public string STextHorizontalAlignment
	{
		set
		{
			this.SecondText.HorizontalAlignment = value;
			this.RepositionContentText(this.SecondText, this.SecondText.HorizontalAlignment, this.SecondText.VerticalAlignment);
		}
	}

	public string STextVerticalAlignment
	{
		set
		{
			this.SecondText.VerticalAlignment = value;
			this.RepositionContentText(this.SecondText, this.SecondText.HorizontalAlignment, this.SecondText.VerticalAlignment);
		}
	}

	public bool STextVisibility
	{
		get
		{
			return this.SecondText.Visibility;
		}
		set
		{
			this.SecondText.Visibility = value;
		}
	}

	public string SText
	{
		get
		{
			return this.SecondText.Text;
		}
		set
		{
			if (this.NumType == GGoodIcon.GoodsNumType.Normal)
			{
				this.SecondText.Text = value;
			}
		}
	}

	public uint STextShadowColor
	{
		set
		{
			this.SecondText.fontBorder = value;
		}
	}

	public uint STextColor
	{
		set
		{
			this.SecondText.textColor = value;
		}
	}

	public int STextSize
	{
		set
		{
			this.SecondText.FontSize = value;
		}
	}

	protected void InitHintDecoration(int decoCode, Point pos)
	{
		this.ClearHintDecoration();
		this.HintDeco = Global.GetDecoration(decoCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
		this.Container.Children.Add(this.HintDeco.The3DGameObject);
		this.HintDeco.Start();
	}

	protected new void ClearHintDecoration()
	{
		if (this.HintDeco != null)
		{
			this.HintDeco.Destroy();
			this.HintDeco = null;
		}
	}

	public int HintDecoType
	{
		get
		{
			return this.CurrentHintDecoType;
		}
		set
		{
			this.CurrentHintDecoType = 0;
			if (this.CurrentHintDecoType == 0)
			{
				this.InitHintDecoration(50005, new Point((int)this.Width / 2, 0));
			}
			else if (this.CurrentHintDecoType == 1)
			{
				this.InitHintDecoration(50004, new Point((int)this.Width / 2, (int)this.Height));
			}
			else if (this.CurrentHintDecoType == 2)
			{
				this.InitHintDecoration(50007, new Point(0, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 3)
			{
				this.InitHintDecoration(50006, new Point((int)this.Width, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 4)
			{
				this.InitHintDecoration(518, new Point(30, -85));
			}
			else if (this.CurrentHintDecoType == 5)
			{
				this.InitHintDecoration(533, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 6)
			{
				this.InitHintDecoration(531, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 7)
			{
				this.InitHintDecoration(532, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 8)
			{
				this.InitHintDecoration(560, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 9)
			{
				this.InitHintDecoration(534, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else if (this.CurrentHintDecoType == 10)
			{
				this.InitHintDecoration(537, new Point((int)this.Width / 2, (int)this.Height / 2));
			}
			else
			{
				this.ClearHintDecoration();
			}
		}
	}

	public void SetBackHide(bool flag)
	{
		this.BackgroundSprite0.gameObject.SetActive(flag);
		this.BackgroundSprite1.gameObject.SetActive(flag);
		this.BackgroundSprite15.gameObject.SetActive(flag);
		this.TeXiao.gameObject.SetActive(flag);
		this.NoUseSprite.gameObject.SetActive(flag);
	}

	private const int MinBackSprite15Width = 70;

	private const int MinBackSprite15Height = 70;

	[HideInInspector]
	public ShowNetImage BackgroundImg;

	public UISprite BackgroundSprite0;

	public UISprite BackgroundSprite1;

	public UISprite BackgroundSprite15;

	public UISprite BackgroundSprite2;

	public CAnimation TeXiao;

	public UISprite BindingSprite;

	public UISprite NoUseSprite;

	public UISprite EndTimeSprite;

	public UISprite ZhanLiSprite;

	public ShowNetImage EquipedSprite;

	public UILabel petLevel;

	public ShowNetImage GoodImg;

	public TextBlock ContentText;

	public TextBlock SecondText;

	public bool isAutoSize;

	public bool isAutoInnerSize;

	public bool isAutoItemPos;

	public MouseLeftButtonUpEventHandler MouseLeftButtonUp;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler DPImageDownloadedItem;

	private IconTypes IconType;

	private ImageBrush _GoodImg;

	private BoxCollider _Collider;

	private int _PaddingX = 2;

	private int _PaddingY = 2;

	public GameObject RedTipObject;

	private GGoodIcon.GoodsNumType NumType;

	private double _LastMouseDownTicks;

	private ImageURL _BodyURL;

	private ImageURL _BackURL;

	private int _ItemCategory = 50;

	private int _ItemCode;

	private object _ItemObject;

	private int _BoxTypes = -1;

	private int _ItemNum;

	private bool _EnableIcon = true;

	private int HintState;

	private int _TimerTicks = 400;

	private bool _OldEnableHint;

	private bool _EnableHint;

	private DispatcherTimer _HintTimer;

	private static Dictionary<string, BitmapData> _HintImageDict = new Dictionary<string, BitmapData>();

	private string _TextHorizontalAlignment = global::Layout.Center;

	private string _TextVerticalAlignment = global::Layout.Center;

	private uint _NormalTextColor = 16777215U;

	private uint _DisableTextColor = 8421504U;

	private int NormalSize = 16;

	protected new GDecoration HintDeco;

	protected int CurrentHintDecoType = -1;

	public enum GoodsNumType
	{
		Normal,
		Specail
	}
}
