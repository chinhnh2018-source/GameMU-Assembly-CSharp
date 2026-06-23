using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class GTipService
{
	public static void RenderTip()
	{
		if (GTipService.ShowTipState)
		{
			return;
		}
		if (null != GTipService.HookUC && null != GTipService.HookITip && (double)Global.GetCorrectLocalTime() - GTipService.MouseEnterTicks >= 100.0)
		{
			GTipService.ShowTipState = true;
			GTipService.NotifyTip(GTipService.HookUC, new NotifyTipEventArgs
			{
				MouseState = true,
				TipType = (TipTypes)GTipService.HookITip.TipType,
				Tip = GTipService.HookITip.Tip,
				MouseEvent = GTipService.MouseEnterE
			});
		}
	}

	public static void HookTip(SpriteSL uc, GTipSprite tip)
	{
		uc.addEventListener("ROLL_OVER", delegate(MouseEvent e)
		{
			GTipService.HookUC = uc;
			GTipService.HookITip = tip;
			GTipService.MouseEnterE = e;
			GTipService.MouseEnterTicks = (double)Global.GetCorrectLocalTime();
		});
		uc.addEventListener("ROLL_OUT", delegate(MouseEvent e)
		{
			GTipService.NotifyTip(uc, new NotifyTipEventArgs
			{
				MouseState = false,
				TipType = (TipTypes)tip.TipType,
				Tip = tip.Tip,
				MouseEvent = e
			});
			GTipService.HookUC = null;
			GTipService.HookITip = null;
			GTipService.ShowTipState = false;
		});
		uc.addEventListener("mouseDown", delegate(MouseEvent e)
		{
			GTipService.NotifyTip(uc, new NotifyTipEventArgs
			{
				MouseState = false,
				TipType = (TipTypes)tip.TipType,
				Tip = tip.Tip,
				MouseEvent = e
			});
			GTipService.HookUC = null;
			GTipService.HookITip = null;
			GTipService.ShowTipState = false;
		});
	}

	public static void NotifyTip(object sender, object _e)
	{
		NotifyTipEventArgs notifyTipEventArgs = _e as NotifyTipEventArgs;
		if (notifyTipEventArgs.MouseState)
		{
			GTipService.ShowTip(notifyTipEventArgs);
		}
		else
		{
			GTipService.HideTip();
		}
	}

	private static Point CalcPoint(object _e, int width, int height)
	{
		NotifyTipEventArgs notifyTipEventArgs = _e as NotifyTipEventArgs;
		if (notifyTipEventArgs == null)
		{
			return new Point(0, 0);
		}
		Point position = new global::MousePosition(notifyTipEventArgs.MouseEvent).GetPosition(Global.GlobalMainWindow.MainStage);
		if (position.Y >= height + 30)
		{
			double num = Global.GMin((double)(position.Y - height - 30), Global.GlobalMainWindow.ActualHeight - (double)height - 10.0);
			if ((int)Global.GlobalMainWindow.ActualWidth - position.X >= width)
			{
				position = new Point(position.X + 20, (int)num);
			}
			else
			{
				position = new Point(position.X - width - 20, (int)num);
			}
		}
		else
		{
			double num2 = Global.GMin((double)(position.Y + 30), Global.GlobalMainWindow.ActualHeight - (double)height - 10.0);
			if ((int)Global.GlobalMainWindow.ActualWidth - position.X >= width)
			{
				position = new Point(position.X + 20, (int)num2);
			}
			else
			{
				position = new Point(position.X - width - 20, (int)num2);
			}
		}
		return position;
	}

	public static void ShowTip(object _e)
	{
		NotifyTipEventArgs notifyTipEventArgs = _e as NotifyTipEventArgs;
		if (notifyTipEventArgs.TipType == TipTypes.NormalText)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowNoramlTipWindow(notifyTipEventArgs, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.SkillText)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				string[] array = notifyTipEventArgs.Tip.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					GTipService.ShowSkillTipWindow(notifyTipEventArgs, array, false);
				}
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.GoodsText)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				string[] array2 = notifyTipEventArgs.Tip.Split(new char[]
				{
					','
				});
				if (array2.Length == 4)
				{
					Global.TryShowMendPrice(Convert.ToInt32(array2[2]), true);
					Global.TryShowSalePriceToNpc(Convert.ToInt32(array2[2]), true);
					GoodsData goodsOnBody = Super.GetGoodsOnBody(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), -1);
					GoodsData goodsData = null;
					if (goodsOnBody != null)
					{
						goodsData = Super.GetGoodsOnBody(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]), goodsOnBody.Id);
					}
					GTipService.PreShowGoodsTipWindow1(notifyTipEventArgs, array2, false);
					if (goodsOnBody != null)
					{
						string text = StringUtil.substitute("{0},{1},{2},{3}", new object[]
						{
							goodsOnBody.GoodsID,
							0,
							goodsOnBody.Id,
							0
						});
						string[] fields = text.Split(new char[]
						{
							','
						});
						GTipService.PreShowGoodsTipWindow2(notifyTipEventArgs, fields, false);
					}
					if (goodsData != null)
					{
						string text2 = StringUtil.substitute("{0},{1},{2},{3}", new object[]
						{
							goodsData.GoodsID,
							0,
							goodsData.Id,
							0
						});
						string[] fields2 = text2.Split(new char[]
						{
							','
						});
						GTipService.PreShowGoodsTipWindow3(notifyTipEventArgs, fields2, false);
					}
					if (goodsOnBody == null && goodsData == null)
					{
						Point point = GTipService.CalcPoint(notifyTipEventArgs, (int)GTipService.GoodsTipWindow_1.BodyWidth, (int)GTipService.GoodsTipWindow_1.BodyHeight);
						GTipService.ShowGoodsTipWindow1(point.X, point.Y);
					}
					else if (goodsOnBody != null && goodsData == null)
					{
						int width = (int)GTipService.GoodsTipWindow_1.BodyWidth + (int)GTipService.GoodsTipWindow_2.BodyWidth;
						int height = Math.Max((int)GTipService.GoodsTipWindow_1.BodyHeight, (int)GTipService.GoodsTipWindow_2.BodyHeight);
						Point point2 = GTipService.CalcPoint(notifyTipEventArgs, width, height);
						Point position = new global::MousePosition(notifyTipEventArgs.MouseEvent).GetPosition(Global.GlobalMainWindow.MainStage);
						if (point2.X >= position.X)
						{
							GTipService.ShowGoodsTipWindow1(point2.X, point2.Y);
							GTipService.ShowGoodsTipWindow2(point2.X + (int)GTipService.GoodsTipWindow_1.BodyWidth, point2.Y);
						}
						else
						{
							GTipService.ShowGoodsTipWindow1(point2.X + (int)GTipService.GoodsTipWindow_2.BodyWidth, point2.Y);
							GTipService.ShowGoodsTipWindow2(point2.X, point2.Y);
						}
					}
					else if (goodsOnBody != null && goodsData != null)
					{
						int width2 = (int)GTipService.GoodsTipWindow_1.BodyWidth + (int)GTipService.GoodsTipWindow_2.BodyWidth + (int)GTipService.GoodsTipWindow_3.BodyWidth;
						int num = Math.Max((int)GTipService.GoodsTipWindow_1.BodyHeight, (int)GTipService.GoodsTipWindow_2.BodyHeight);
						num = Math.Max((int)GTipService.GoodsTipWindow_3.BodyHeight, num);
						Point point3 = GTipService.CalcPoint(notifyTipEventArgs, width2, num);
						Point position2 = new global::MousePosition(notifyTipEventArgs.MouseEvent).GetPosition(Global.GlobalMainWindow.MainStage);
						if (point3.X >= position2.X)
						{
							GTipService.ShowGoodsTipWindow1(point3.X, point3.Y);
							GTipService.ShowGoodsTipWindow2(point3.X + (int)GTipService.GoodsTipWindow_1.BodyWidth, point3.Y);
							GTipService.ShowGoodsTipWindow3(point3.X + (int)GTipService.GoodsTipWindow_1.BodyWidth + (int)GTipService.GoodsTipWindow_2.BodyWidth, point3.Y);
						}
						else
						{
							GTipService.ShowGoodsTipWindow1(point3.X + (int)GTipService.GoodsTipWindow_2.BodyWidth + (int)GTipService.GoodsTipWindow_3.BodyWidth, point3.Y);
							GTipService.ShowGoodsTipWindow2(point3.X + (int)GTipService.GoodsTipWindow_2.BodyWidth, point3.Y);
							GTipService.ShowGoodsTipWindow3(point3.X, point3.Y);
						}
					}
				}
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.ExternalTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.BufferTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.ExperienceTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.LingLiTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.LifeSliderTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.MagicSliderTip)
		{
			if (notifyTipEventArgs.Tip != null && Global.StringTrim(notifyTipEventArgs.Tip) != string.Empty)
			{
				GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
			}
		}
		else if (notifyTipEventArgs.TipType == TipTypes.BonusTip)
		{
			GTipService.ShowGeneralTipWindow(notifyTipEventArgs, (int)notifyTipEventArgs.TipType, notifyTipEventArgs.Tip);
		}
	}

	public static void HideTip()
	{
		GTipService.HideNoramlTipWindow();
		GTipService.HideSkillTipWindow();
		GTipService.HideGoodsTipWindow1();
		GTipService.HideGoodsTipWindow2();
		GTipService.HideGoodsTipWindow3();
		GTipService.HideGeneralTipWindow();
		Global.TryShowMendPrice(-1, false);
		Global.TryShowSalePriceToNpc(-1, false);
	}

	private static GTextBlockEx GetTextBlock(string text, int fontSize, uint color, double maxWidth, string name = null)
	{
		GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		gtextBlockEx.FontSize = fontSize;
		if (!double.IsNaN(maxWidth))
		{
			gtextBlockEx.TextWidth = maxWidth;
			gtextBlockEx.TextWrapping = TextWrapping.Wrap;
		}
		if (name != null)
		{
			gtextBlockEx.Name = name;
		}
		gtextBlockEx.Text = text;
		return gtextBlockEx;
	}

	private static GTextBlockEx GetTextBlock2(string text, int fontSize, uint color, double maxWidth, string name = null)
	{
		GTextBlockEx gtextBlockEx = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		gtextBlockEx.FontSize = fontSize;
		if (!double.IsNaN(maxWidth))
		{
			gtextBlockEx.TextWidth = maxWidth;
			gtextBlockEx.TextWrapping = TextWrapping.Wrap;
		}
		if (name != null)
		{
			gtextBlockEx.Name = name;
		}
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		return gtextBlockEx;
	}

	private static void BringWindowToTop(SpriteSL win)
	{
		Super.GData.PlayZoneRoot.Children.Remove(win, true);
		Super.GData.PlayZoneRoot.Children.Add(win);
	}

	private static void InitNoramlTipWindow()
	{
		GTipService.NormalTextBlock = new GTextBlockEx(string.Empty, -1, -1, -1, -1, 0);
		GTipService.NormalTextBlock.TextColor = new SolidColorBrush(4294944000U);
		GTipService.NormalTextBlock.TextWrapping = TextWrapping.Wrap;
		GTipService.NormalTextBlock.TextWidth = 300.0;
		SpriteSL spriteSL = new SpriteSL();
		spriteSL.Add(GTipService.NormalTextBlock);
		GTipService.NormalTipWindow = GTipService.ShowTipWindow(0, 0, spriteSL, 5, 5, 0, 0);
	}

	private static void ShowNoramlTipWindow(object e, string tipText)
	{
		if (null == GTipService.NormalTipWindow)
		{
			GTipService.InitNoramlTipWindow();
		}
		if (null == GTipService.NormalTipWindow)
		{
			return;
		}
		GTipService.NormalTextBlock.Text = tipText;
		GTipService.NormalTipWindow.BodyWidth = (double)((int)GTipService.NormalTextBlock.RealSize.Width + 20);
		GTipService.NormalTipWindow.BodyHeight = (double)((int)GTipService.NormalTextBlock.RealSize.Height + 20);
		Point point = GTipService.CalcPoint(e, (int)GTipService.NormalTipWindow.BodyWidth, (int)GTipService.NormalTipWindow.BodyHeight);
		GTipService.NormalTipWindow.Left = Math.Floor((double)point.X);
		GTipService.NormalTipWindow.Top = Math.Floor((double)point.Y);
		GTipService.NormalTipWindow.Visibility = true;
		GTipService.BringWindowToTop(GTipService.NormalTipWindow);
	}

	private static void HideNoramlTipWindow()
	{
		if (null == GTipService.NormalTipWindow)
		{
			return;
		}
		GTipService.NormalTipWindow.Visibility = false;
	}

	private static void InitSkillTipWindow()
	{
		GTipService.SkillWrapPanel = GTipService.GetSkillWrapPanel();
		GTipService.SkillTipWindow = GTipService.ShowTipWindow(0, 0, GTipService.SkillWrapPanel, 10, 10, 0, 0);
	}

	private static void ShowSkillTipWindow(object e, string[] fields, bool caching = false)
	{
		if (null == GTipService.SkillTipWindow)
		{
			GTipService.InitSkillTipWindow();
		}
		if (null == GTipService.SkillTipWindow)
		{
			return;
		}
		GTipService.InitSkillWrapPanel(fields[0], fields[1]);
		GTipService.SkillWrapPanel.Measure(new SizeSL(double.PositiveInfinity, double.PositiveInfinity));
		int num = (int)GTipService.SkillWrapPanel.DesiredSize.Width + 20;
		int num2 = (int)GTipService.SkillWrapPanel.DesiredSize.Height + 20;
		GTipService.SkillTipWindow.BodyWidth = (double)num;
		GTipService.SkillTipWindow.BodyHeight = (double)num2;
		Point point = GTipService.CalcPoint(e, (int)GTipService.SkillTipWindow.BodyWidth, (int)GTipService.SkillTipWindow.BodyHeight);
		GTipService.SkillTipWindow.Left = Math.Floor((double)point.X);
		GTipService.SkillTipWindow.Top = Math.Floor((double)point.Y);
		if (!caching)
		{
			GTipService.SkillTipWindow.Visibility = true;
			GTipService.BringWindowToTop(GTipService.SkillTipWindow);
		}
		else
		{
			GTipService.SkillTipWindow.Visibility = false;
		}
	}

	private static void HideSkillTipWindow()
	{
		if (null == GTipService.SkillTipWindow)
		{
			return;
		}
		GTipService.SkillTipWindow.Visibility = false;
	}

	private static Canvas GetSkillImageDescCanvas(string iconName)
	{
		Canvas canvas = new Canvas();
		canvas.Name = "ImageCanvas";
		canvas.Width = 250.0;
		canvas.Height = 46.0;
		Image image = new Image();
		image.IsHitTestVisible = false;
		image.Width = 41.0;
		image.Height = 41.0;
		image.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/rec.png"));
		Canvas.SetLeft(image, 0);
		Canvas.SetTop(image, 2);
		canvas.Children.Add(image);
		URLImage urlimage = new URLImage();
		urlimage.Name = "SkillIcon";
		urlimage.IsHitTestVisible = false;
		urlimage.Width = 32.0;
		urlimage.Height = 32.0;
		urlimage.Source = ((iconName == null) ? null : new ImageBrush(Global.GetGameResImage(iconName)));
		Canvas.SetLeft(urlimage, 4);
		Canvas.SetTop(urlimage, 6);
		canvas.Children.Add(urlimage);
		GTipService.SkillCtrlsDict[urlimage.Name] = urlimage;
		return canvas;
	}

	private static StackPanel GetSkillWrapPanel()
	{
		StackPanel stackPanel = new StackPanel();
		stackPanel.Orientation = global::Layout.Vertical;
		GTipService.SkillWrapPanel = stackPanel;
		GTextBlockEx gtextBlockEx = GTipService.GetTextBlock2(" ", FontSizeMgr.TipServiceTitleFontSize, uint.MaxValue, double.NaN, "SkillTitle");
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		StackPanel stackPanel2 = new StackPanel();
		stackPanel2.Name = "TitlePanel";
		stackPanel2.Width = 250.0;
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		stackPanel2.Children.Add(gtextBlockEx);
		stackPanel.Children.Add(stackPanel2);
		GTipService.SkillCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, uint.MaxValue, 250.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		int num = 4;
		Canvas skillImageDescCanvas = GTipService.GetSkillImageDescCanvas(null);
		stackPanel.Children.Add(skillImageDescCanvas);
		GTipService.SkillCtrlsDict[skillImageDescCanvas.Name] = skillImageDescCanvas;
		gtextBlockEx = GTipService.GetTextBlock2(" ", HSTextField.defaultFontSize, ColorSL.FromArgb(255, 202, 154, 39), 201.0, "SkillLevel");
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		Canvas.SetLeft(gtextBlockEx, 49);
		Canvas.SetTop(gtextBlockEx, num);
		skillImageDescCanvas.Children.Add(gtextBlockEx);
		num += (int)gtextBlockEx.RealSize.Height + 1;
		gtextBlockEx = GTipService.GetTextBlock2(" ", HSTextField.defaultFontSize, ColorSL.FromArgb(255, 202, 154, 39), 201.0, "SkillType");
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		Canvas.SetLeft(gtextBlockEx, 49);
		Canvas.SetTop(gtextBlockEx, num);
		skillImageDescCanvas.Children.Add(gtextBlockEx);
		num += (int)gtextBlockEx.RealSize.Height + 1;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, uint.MaxValue, 250.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, 4278255360U, 250.0, "HintDesc");
		stackPanel.Children.Add(gtextBlockEx);
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, 4294967040U, 250.0, "Desc");
		stackPanel.Children.Add(gtextBlockEx);
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, uint.MaxValue, 250.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, 4278255360U, 250.0, "NextLevelOrLearn");
		stackPanel.Children.Add(gtextBlockEx);
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, 4294901760U, 250.0, "LearnCondition");
		stackPanel.Children.Add(gtextBlockEx);
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock(" ", HSTextField.defaultFontSize, 4294967040U, 250.0, "Desc1");
		stackPanel.Children.Add(gtextBlockEx);
		GTipService.SkillCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		return stackPanel;
	}

	private static void InitSkillWrapPanel(string skillID, string level)
	{
		StackPanel skillWrapPanel = GTipService.SkillWrapPanel;
		MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(Convert.ToInt32(skillID));
		if (skillXmlNode == null)
		{
			return;
		}
		SkillData skillDataByID = Global.GetSkillDataByID(Convert.ToInt32(skillID));
		string text = StringUtil.substitute("{0}", new object[]
		{
			skillXmlNode.Name
		});
		StackPanel stackPanel = GTipService.SkillCtrlsDict["TitlePanel"] as StackPanel;
		GTextBlockEx gtextBlockEx = GTipService.SkillCtrlsDict["SkillTitle"] as GTextBlockEx;
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		string url = string.Empty;
		int magicIcon = skillXmlNode.MagicIcon;
		if (magicIcon > 0)
		{
			url = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
			{
				magicIcon
			});
		}
		else
		{
			url = StringUtil.substitute("NetImages/GameRes/Images/Skill/0.png", new object[]
			{
				magicIcon
			});
		}
		Canvas canvas = GTipService.SkillCtrlsDict["ImageCanvas"] as Canvas;
		URLImage urlimage = GTipService.SkillCtrlsDict["SkillIcon"] as URLImage;
		urlimage.URL = url;
		int num = (skillDataByID != null) ? skillDataByID.SkillLevel : 1;
		int num2 = Global.GetSkillUsedMagicV(Global.Data.roleData.Occupation, Convert.ToInt32(skillID), num);
		num2 = Global.GMax(0, num2);
		text = StringUtil.substitute(Global.GetLang("技能等级: ｛color=#FF00B702 uline=false tag= text={0}｝\t 魔法消耗: ｛color=#FF00B702 uline=false tag= text={1}｝"), new object[]
		{
			num,
			num2
		});
		gtextBlockEx = (GTipService.SkillCtrlsDict["SkillLevel"] as GTextBlockEx);
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		string lang = Global.GetLang("主动单攻");
		if (skillXmlNode.MagicType == -1)
		{
			lang = Global.GetLang("被动技能");
		}
		else if (skillXmlNode.MagicType == 2)
		{
			lang = Global.GetLang("主动群攻");
		}
		else if (skillXmlNode.MagicType == 3)
		{
			lang = Global.GetLang("自动触发");
		}
		text = StringUtil.substitute(Global.GetLang("技能类型: ｛color=#FF00B702 uline=false tag= text={0}｝"), new object[]
		{
			lang
		});
		gtextBlockEx = (GTipService.SkillCtrlsDict["SkillType"] as GTextBlockEx);
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		string text2 = skillXmlNode.Description;
		string[] array = text2.Split(new char[]
		{
			'|'
		});
		text2 = array[Math.Min(num - 1, array.Length - 1)];
		int num3 = 0;
		int num4 = 0;
		Global.GetUpSkillLearCondition(Convert.ToInt32(skillID), num, out num3, out num4, skillXmlNode);
		if (skillDataByID != null)
		{
			text = Global.GetLang("当前等级属性:");
			gtextBlockEx = (GTipService.SkillCtrlsDict["HintDesc"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			text = text2;
			gtextBlockEx = (GTipService.SkillCtrlsDict["Desc"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
		else
		{
			gtextBlockEx = (GTipService.SkillCtrlsDict["HintDesc"] as GTextBlockEx);
			gtextBlockEx.Visibility = false;
			gtextBlockEx = (GTipService.SkillCtrlsDict["Desc"] as GTextBlockEx);
			gtextBlockEx.Visibility = false;
		}
		if (skillDataByID == null)
		{
			text = Global.GetLang("学习条件:");
			gtextBlockEx = (GTipService.SkillCtrlsDict["NextLevelOrLearn"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			text = StringUtil.substitute(Global.GetLang("需求人物等级:{0}"), new object[]
			{
				num3
			});
			gtextBlockEx = (GTipService.SkillCtrlsDict["LearnCondition"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			text = text2;
			gtextBlockEx = (GTipService.SkillCtrlsDict["Desc1"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
		else if (num >= 3)
		{
			gtextBlockEx = (GTipService.SkillCtrlsDict["NextLevelOrLearn"] as GTextBlockEx);
			gtextBlockEx.Visibility = false;
			gtextBlockEx = (GTipService.SkillCtrlsDict["LearnCondition"] as GTextBlockEx);
			gtextBlockEx.Visibility = false;
			gtextBlockEx = (GTipService.SkillCtrlsDict["Desc1"] as GTextBlockEx);
			gtextBlockEx.Visibility = false;
		}
		else
		{
			text = Global.GetLang("下一等级属性:");
			gtextBlockEx = (GTipService.SkillCtrlsDict["NextLevelOrLearn"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			text = StringUtil.substitute(Global.GetLang("需求人物等级:{0}"), new object[]
			{
				num3
			});
			gtextBlockEx = (GTipService.SkillCtrlsDict["LearnCondition"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			text = text2;
			gtextBlockEx = (GTipService.SkillCtrlsDict["Desc1"] as GTextBlockEx);
			gtextBlockEx.Visibility = true;
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
	}

	private static void InitGoodsTipWindow1()
	{
		GTipService.GoodsWrapPanel_1 = GTipService.GetGoodsWrapPanel(GTipService.GoodsCtrlsDict_1);
		GTipService.GoodsTipWindow_1 = GTipService.ShowTipWindow(0, 0, GTipService.GoodsWrapPanel_1, 10, 10, 0, 0);
	}

	private static void ShowGoodsTipWindow1(int x, int y)
	{
		GTipService.GoodsTipWindow_1.Left = Math.Floor((double)x);
		GTipService.GoodsTipWindow_1.Top = Math.Floor((double)y);
		GTipService.GoodsTipWindow_1.Visibility = true;
		GTipService.BringWindowToTop(GTipService.GoodsTipWindow_1);
	}

	private static void PreShowGoodsTipWindow1(object e, string[] fields, bool caching = false)
	{
		if (null == GTipService.GoodsTipWindow_1)
		{
			GTipService.InitGoodsTipWindow1();
		}
		if (null == GTipService.GoodsTipWindow_1)
		{
			return;
		}
		GTipService.InitGoodsWrapPanel(GTipService.GoodsCtrlsDict_1, GTipService.GoodsWrapPanel_1, fields[0], Convert.ToInt32(fields[1]), Convert.ToInt32(fields[2]), Convert.ToInt32(fields[3]), false);
		GTipService.GoodsWrapPanel_1.Measure(new SizeSL(double.PositiveInfinity, double.PositiveInfinity));
		int num = (int)GTipService.GoodsWrapPanel_1.DesiredSize.Width + 20;
		int num2 = (int)GTipService.GoodsWrapPanel_1.DesiredSize.Height + 20;
		GTipService.GoodsTipWindow_1.BodyWidth = (double)num;
		GTipService.GoodsTipWindow_1.BodyHeight = (double)num2;
		if (caching)
		{
			GTipService.GoodsTipWindow_1.Visibility = false;
		}
	}

	private static void HideGoodsTipWindow1()
	{
		if (null == GTipService.GoodsTipWindow_1)
		{
			return;
		}
		GTipService.GoodsTipWindow_1.Visibility = false;
	}

	private static Canvas GetGoodsImageDescCanvas(Dictionary<string, object> goodsCtrlsDict)
	{
		Canvas canvas = new Canvas();
		canvas.Name = "ImageCanvas";
		canvas.Width = 200.0;
		canvas.Height = 46.0;
		Image image = new Image();
		image.IsHitTestVisible = false;
		image.Width = 41.0;
		image.Height = 41.0;
		image.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/rec.png"));
		Canvas.SetLeft(image, 0);
		Canvas.SetTop(image, 2);
		canvas.Children.Add(image);
		URLImage urlimage = new URLImage();
		urlimage.Name = "GoodsIcon";
		urlimage.IsHitTestVisible = false;
		urlimage.Width = 32.0;
		urlimage.Height = 32.0;
		Canvas.SetLeft(urlimage, 4);
		Canvas.SetTop(urlimage, 6);
		canvas.Children.Add(urlimage);
		goodsCtrlsDict[urlimage.Name] = urlimage;
		return canvas;
	}

	private static Canvas GetJewelCanvas(Dictionary<string, object> goodsCtrlsDict, int index)
	{
		Canvas canvas = new Canvas();
		canvas.Name = StringUtil.substitute("JewelItem{0}", new object[]
		{
			index
		});
		canvas.Width = 170.0;
		canvas.Height = 21.0;
		URLImage urlimage = new URLImage();
		urlimage.Name = StringUtil.substitute("JewelImage{0}", new object[]
		{
			index
		});
		urlimage.IsHitTestVisible = false;
		urlimage.Width = 20.0;
		urlimage.Height = 20.0;
		urlimage.Stretch = global::StretchSL.Fill;
		Canvas.SetLeft(urlimage, 4);
		Canvas.SetTop(urlimage, 2);
		canvas.Children.Add(urlimage);
		goodsCtrlsDict[urlimage.Name] = urlimage;
		GTextBlockEx textBlock = GTipService.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, 170.0, StringUtil.substitute("JewelProp{0}", new object[]
		{
			index
		}));
		Canvas.SetLeft(textBlock, 25);
		Canvas.SetTop(textBlock, (21.0 - textBlock.RealSize.Height) / 1.0);
		canvas.Children.Add(textBlock);
		goodsCtrlsDict[textBlock.Name] = textBlock;
		return canvas;
	}

	private static StackPanel GetEquipPropPanel(Dictionary<string, object> goodsCtrlsDict, string name)
	{
		StackPanel stackPanel = new StackPanel();
		stackPanel.Width = 265.0;
		stackPanel.Height = 15.0;
		stackPanel.Name = StringUtil.substitute("Panel_{0}", new object[]
		{
			name
		});
		GTextBlockEx textBlock = GTipService.GetTextBlock2(" ", HSTextField.defaultFontSize, uint.MaxValue, 265.0, StringUtil.substitute("Text_{0}", new object[]
		{
			name
		}));
		stackPanel.Children.Add(textBlock);
		goodsCtrlsDict[textBlock.Name] = textBlock;
		return stackPanel;
	}

	protected static GTextBlockEx AddBaseAttributeTextBlock(string ctlName, StackPanel stackPl, Dictionary<string, object> goodsCtrlsDict)
	{
		GTextBlockEx textBlock = GTipService.GetTextBlock2(string.Empty, HSTextField.defaultFontSize, uint.MaxValue, 180.0, ctlName);
		stackPl.Children.Add(textBlock);
		Canvas.SetLeft(textBlock, 12);
		textBlock.Margin.Top = 3.0;
		goodsCtrlsDict[textBlock.Name] = textBlock;
		return textBlock;
	}

	private static StackPanel GetGoodsWrapPanel(Dictionary<string, object> goodsCtrlsDict)
	{
		StackPanel stackPanel = new StackPanel();
		stackPanel.Orientation = global::Layout.Vertical;
		string text = " ";
		StackPanel stackPanel2 = new StackPanel();
		stackPanel2.Name = "TitlePanel";
		stackPanel2.Width = 220.0;
		GTextBlockEx gtextBlockEx = GTipService.GetTextBlock2(text, FontSizeMgr.TipServiceTitleFontSize, uint.MaxValue, double.NaN, "GoodsTitle");
		gtextBlockEx.fontBold = true;
		gtextBlockEx.FontSize = 13;
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		int num = 0;
		Canvas goodsImageDescCanvas = GTipService.GetGoodsImageDescCanvas(goodsCtrlsDict);
		stackPanel.Children.Add(goodsImageDescCanvas);
		goodsCtrlsDict[goodsImageDescCanvas.Name] = goodsImageDescCanvas;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, uint.MaxValue, 150.0, "Catetoriy");
		Canvas.SetLeft(gtextBlockEx, 49);
		Canvas.SetTop(gtextBlockEx, num);
		goodsImageDescCanvas.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		num += (int)gtextBlockEx.RealSize.Height + 1;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 150.0, "GoodsLevel");
		Canvas.SetLeft(gtextBlockEx, 49);
		Canvas.SetTop(gtextBlockEx, num);
		goodsImageDescCanvas.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		num += (int)gtextBlockEx.RealSize.Height + 1;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 150.0, "Occupation");
		Canvas.SetLeft(gtextBlockEx, 49);
		Canvas.SetTop(gtextBlockEx, num);
		goodsImageDescCanvas.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		num += (int)gtextBlockEx.RealSize.Height + 1;
		RectangleSL rectangleSL = new RectangleSL();
		rectangleSL.Height = 3.0;
		stackPanel.Children.Add(rectangleSL);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "DescPanel";
		stackPanel2.Width = 200.0;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 200.0, "Desc");
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		rectangleSL = new RectangleSL();
		rectangleSL.Height = 3.0;
		stackPanel.Children.Add(rectangleSL);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "WeightAndStrongPanel";
		stackPanel2.Width = 180.0;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 180.0, "Weight");
		stackPanel2.Children.Add(gtextBlockEx);
		Canvas.SetLeft(gtextBlockEx, 0);
		gtextBlockEx.Margin.Top = 1.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 180.0, "Strong");
		stackPanel2.Children.Add(gtextBlockEx);
		Canvas.SetLeft(gtextBlockEx, 0);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "ForgeAndStarPanel";
		stackPanel2.Width = 180.0;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(Global.GetLang("【强化等级】"), HSTextField.defaultFontSize, uint.MaxValue, 180.0, "ForgeLevelTxt");
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		GImgLevel gimgLevel = U3DUtils.NEW<GImgLevel>();
		gimgLevel.Name = "LevelStarImage";
		gimgLevel.SingleStarWidth = 16.0;
		gimgLevel.Img0_Size = new SizeSL(192.0, 11.0);
		gimgLevel.Img0_Source = Global.GetGameResImage("Images/Plate/level_01.png");
		gimgLevel.Img1_Size = new SizeSL(192.0, 11.0);
		gimgLevel.Img1_Source = Global.GetGameResImage("Images/Plate/level_02.png");
		gimgLevel.Level = 0;
		stackPanel2.Children.Add(gimgLevel);
		Canvas.SetLeft(gimgLevel, 12);
		gimgLevel.Margin.Top = 3.0;
		goodsCtrlsDict[gimgLevel.Name] = gimgLevel;
		int num2 = 180;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipBaseAttributePanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(Global.GetLang("【基础属性】"), HSTextField.defaultFontSize, 4294967040U, 80.0, "BaseProps");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		for (int i = 3; i <= 10; i += 2)
		{
			GTipService.AddBaseAttributeTextBlock("EquipPanel_" + ExtPropIndexes.ExtPropIndexNames[i], stackPanel2, goodsCtrlsDict);
		}
		for (int j = 13; j <= 255; j++)
		{
			GTipService.AddBaseAttributeTextBlock("EquipPanel_" + ExtPropIndexes.ExtPropIndexNames[j], stackPanel2, goodsCtrlsDict);
		}
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipForgeBaseAttributePanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(Global.GetLang("【强化属性】"), HSTextField.defaultFontSize, 4294967040U, 80.0, "EquipForgeBaseAttributeTextBlock");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		GTipService.AddBaseAttributeTextBlock("EquipForgeBaseAttributeMaxAttack", stackPanel2, goodsCtrlsDict);
		GTipService.AddBaseAttributeTextBlock("EquipForgeBaseAttributeMaxMAttack", stackPanel2, goodsCtrlsDict);
		GTipService.AddBaseAttributeTextBlock("EquipForgeBaseAttributeMaxDAttack", stackPanel2, goodsCtrlsDict);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipForgeAcitveAttributePanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(Global.GetLang("【强化激活属性】"), HSTextField.defaultFontSize, 4294967040U, 180.0, "EquipForgeAcitveAttributePanelTextBlock");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		for (int k = 0; k <= 6; k++)
		{
			GTipService.AddBaseAttributeTextBlock("EquipForgeAcitveAttributePanel_" + k, stackPanel2, goodsCtrlsDict);
		}
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipBornAttributePanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(Global.GetLang("【天生属性】"), HSTextField.defaultFontSize, 4294967040U, 180.0, "EquipBornAttributePanelTextBlock");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		GTipService.AddBaseAttributeTextBlock("EquipBornBaseAttributeMaxAttack", stackPanel2, goodsCtrlsDict);
		GTipService.AddBaseAttributeTextBlock("EquipBornBaseAttributeMaxMAttack", stackPanel2, goodsCtrlsDict);
		GTipService.AddBaseAttributeTextBlock("EquipBornBaseAttributeMaxDAttack", stackPanel2, goodsCtrlsDict);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipToSexPanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(string.Empty, HSTextField.defaultFontSize, 4294967040U, 180.0, "EquipToSexPanelTextBlock");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EquipRequisitePanel";
		stackPanel2.Width = (double)num2;
		stackPanel2.Orientation = global::Layout.Vertical;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(string.Empty, HSTextField.defaultFontSize, 4294967040U, 180.0, "EquipRequisitePanelTextBlock");
		stackPanel2.Children.Add(gtextBlockEx);
		gtextBlockEx.Margin.Top = 3.0;
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "OtherPanel";
		stackPanel2.Width = 170.0;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, uint.MaxValue, 180.0, "CDTime");
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		rectangleSL = new RectangleSL();
		rectangleSL.Height = 3.0;
		stackPanel2.Children.Add(rectangleSL);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "PricePanel";
		stackPanel2.Width = 170.0;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 180.0, "Price");
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		rectangleSL = new RectangleSL();
		rectangleSL.Height = 3.0;
		stackPanel2.Children.Add(rectangleSL);
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "BindingPanel";
		stackPanel2.Width = 180.0;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, 4294901760U, double.NaN, "Binding");
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "EndTimePanel";
		stackPanel2.Width = 180.0;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, 4294901760U, double.NaN, "EndTime1");
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, 4294901760U, double.NaN, "EndTime2");
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		stackPanel2 = new StackPanel();
		stackPanel2.Name = "GoodsNumPanel";
		stackPanel2.Width = 180.0;
		stackPanel.Children.Add(stackPanel2);
		goodsCtrlsDict[stackPanel2.Name] = stackPanel2;
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, uint.MaxValue, 180.0, "GoodsNum");
		stackPanel2.Children.Add(gtextBlockEx);
		goodsCtrlsDict[gtextBlockEx.Name] = gtextBlockEx;
		(goodsCtrlsDict["DescPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["WeightAndStrongPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["ForgeAndStarPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipBornAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipRequisitePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipToSexPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["OtherPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["PricePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["BindingPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EndTimePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["GoodsNumPanel"] as StackPanel).Visibility = false;
		return stackPanel;
	}

	private static void InitJewelItem(Dictionary<string, object> goodsCtrlsDict, int index, string jewelGoodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(jewelGoodsID));
		if (goodsXmlNodeByID == null)
		{
			GTextBlockEx gtextBlockEx = goodsCtrlsDict[StringUtil.substitute("JewelProp{0}", new object[]
			{
				index
			})] as GTextBlockEx;
			gtextBlockEx.Text = Global.GetLang("未镶嵌宝石");
			gtextBlockEx.textColor = 4286611584U;
			URLImage urlimage = goodsCtrlsDict[StringUtil.substitute("JewelImage{0}", new object[]
			{
				index
			})] as URLImage;
			urlimage.URL = StringUtil.substitute("NetImages/GameRes/Images/TipJewels/{0}.png", new object[]
			{
				"weiXiangQianBaoShi"
			});
		}
		double[] equipProps = goodsXmlNodeByID.EquipProps;
		if (equipProps.Length > 0)
		{
			string text = string.Empty;
			double[] equipProps2 = goodsXmlNodeByID.EquipProps;
			string title = goodsXmlNodeByID.Title;
			if (equipProps.Length == 182)
			{
				if (equipProps[7] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000物攻:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps[7] + (equipProps[8] - equipProps[7]) / 2.0
					});
				}
				if (equipProps[9] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000魔攻:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps[9] + (equipProps[10] - equipProps[9]) / 2.0
					});
				}
				if (equipProps[4] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000物防:｝  ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps[4]
					});
				}
				if (equipProps[6] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000魔防:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps[6]
					});
				}
				if (equipProps[17] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000暴击:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[17]
					});
				}
				if (equipProps[18] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000暴抗:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[18]
					});
				}
				if (equipProps[19] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text = StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000闪避:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[19]
					});
				}
				if (equipProps[18] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000命中:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[18]
					});
				}
				if (equipProps[13] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text = StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000生命值:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[13]
					});
				}
				if (equipProps[15] != 0.0)
				{
					if (text.Length > 0)
					{
						text += " ";
					}
					text += StringUtil.substitute(Global.GetLang("｛color=#FF02E8FD uline=false tag= text={0}\u3000魔法值:｝ ｛color=#FF02E8FD uline=false tag= text=+{1}｝"), new object[]
					{
						title,
						equipProps2[15]
					});
				}
				if (text.Length > 0)
				{
					URLImage urlimage2 = goodsCtrlsDict[StringUtil.substitute("JewelImage{0}", new object[]
					{
						index
					})] as URLImage;
					urlimage2.URL = StringUtil.substitute("NetImages/GameRes/Images/TipJewels/{0}_x.png", new object[]
					{
						goodsXmlNodeByID.IconCode
					});
					GTextBlockEx gtextBlockEx = goodsCtrlsDict[StringUtil.substitute("JewelProp{0}", new object[]
					{
						index
					})] as GTextBlockEx;
					Super.FormatTextBlockEx2(gtextBlockEx, text);
				}
			}
		}
	}

	private static void InitBaseAttributeTextBlockEx(Dictionary<string, object> goodsCtrlsDict, GoodsData gd, double[] equipFields_1, double[] equipFields_2, int extPropIndexMin, int extPropIndexMax, string extPropName, string txtBlockControlName, StackPanel sp)
	{
		GTextBlockEx gtextBlockEx = goodsCtrlsDict[txtBlockControlName] as GTextBlockEx;
		if (equipFields_1[extPropIndexMin] != 0.0 || equipFields_1[extPropIndexMax] != 0.0)
		{
			int num = (int)Global.GetEquipForgeAddActiveExtraValue(gd, extPropIndexMin);
			int num2 = (int)Global.GetEquipForgeAddActiveExtraValue(gd, extPropIndexMax);
			int num3 = (int)equipFields_1[extPropIndexMin];
			int num4 = (int)equipFields_1[extPropIndexMax];
			int num5 = (int)Global.GetEquipForgeAddBaseValue(gd, extPropIndexMax);
			int num6 = (int)Global.GetEquipBornAddBaseValue(gd, extPropIndexMax);
			string empty = string.Empty;
			if (num > 0)
			{
			}
			string empty2 = string.Empty;
			if (num2 > 0)
			{
			}
			string text = "#FFFFFFFF";
			if (num5 > 0 || num6 > 0)
			{
				text = "#FF0099FF";
			}
			int num7 = num4 + num5 + num6;
			string text2 = "#FFFFFFFF";
			if (extPropIndexMax >= 13)
			{
				text2 = "#FFFF0000";
			}
			string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
			{
				text2,
				extPropName + ": ",
				"#ffffffff",
				num3,
				string.Empty,
				empty,
				"#ffffffff",
				"-",
				text,
				num7,
				string.Empty,
				empty2
			});
			gtextBlockEx.htmlText = colorStringForHtmlText;
			sp.Children.Add(gtextBlockEx);
			gtextBlockEx.Visibility = true;
		}
		else
		{
			gtextBlockEx.Visibility = false;
		}
	}

	private static void InitBaseAttributeTextBlock(Dictionary<string, object> goodsCtrlsDict, GoodsData gd, double[] equipFields_1, double[] equipFields_2, int extPropIndex, string extPropName, string txtBlockControlName, bool isPercent, StackPanel sp)
	{
		GTextBlockEx gtextBlockEx = goodsCtrlsDict[txtBlockControlName] as GTextBlockEx;
		if (equipFields_1[extPropIndex] != 0.0)
		{
			int num;
			if (extPropIndex == 13)
			{
				num = Global.GetEquipForgeAddActiveExtraLifeValue(gd);
			}
			else
			{
				num = (int)Global.GetEquipForgeAddActiveExtraValue(gd, extPropIndex);
			}
			string text = string.Empty;
			if (isPercent)
			{
				text = "%";
			}
			int num2 = (!isPercent) ? 1 : 100;
			int num3 = (int)((double)num2 * equipFields_1[extPropIndex]);
			string empty = string.Empty;
			if (num > 0)
			{
			}
			string text2 = "#FFFFFFFF";
			string text3 = "#FFFFFFFF";
			if (extPropIndex >= 13)
			{
				text2 = "#FF3cff3c";
				text3 = "#FF3cff3c";
			}
			string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
			{
				text2,
				extPropName + ": ",
				text3,
				"+" + num3 + text,
				string.Empty,
				empty
			});
			gtextBlockEx.htmlText = colorStringForHtmlText;
			sp.Children.Add(gtextBlockEx);
			gtextBlockEx.Visibility = true;
		}
		else
		{
			gtextBlockEx.Visibility = false;
		}
	}

	private static bool InitForgeBaseAttributeTextBlock(Dictionary<string, object> goodsCtrlsDict, GoodsData gd, int extPropIndex, string extPropName, string txtBlockControlName, StackPanel sp)
	{
		GTextBlockEx gtextBlockEx = goodsCtrlsDict[txtBlockControlName] as GTextBlockEx;
		if (extPropIndex != 8 && extPropIndex != 10 && extPropIndex != 10)
		{
			return false;
		}
		int num = (int)Global.GetEquipForgeAddBaseValue(gd, extPropIndex);
		if (num > 0)
		{
			string text = StringUtil.substitute("{0} +{1}", new object[]
			{
				extPropName,
				num
			});
			string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
			{
				"#FF0099FF",
				text
			});
			sp.Children.Add(gtextBlockEx);
			gtextBlockEx.htmlText = colorStringForHtmlText;
			gtextBlockEx.Visibility = true;
			return true;
		}
		gtextBlockEx.Visibility = false;
		return false;
	}

	private static void InitForgeActivateAttributeTextBlock(Dictionary<string, object> goodsCtrlsDict, GoodsData gd, string qiangHuaItemStr, StackPanel sp, int countIndex)
	{
		string[] array = qiangHuaItemStr.Split(new char[]
		{
			','
		});
		if (array.Length != 3)
		{
			return;
		}
		int num = Global.SafeConvertToInt32(array[0]);
		string key = array[1].ToLower();
		string text = array[2];
		int num2 = ExtPropIndexes.ExtPropIndexNames.IndexOf(key);
		if (num2 < 0)
		{
			return;
		}
		string text2 = text;
		if (ExtPropIndexes.ExtPropIndexPercents[num2] > 0)
		{
			text2 = StringUtil.substitute("{0}%", new object[]
			{
				(int)(100.0 * Global.SafeConvertToDouble(array[2].Split(new char[]
				{
					'-'
				})[0]))
			});
		}
		string text3 = "#FF999999";
		if (gd.Forge_level >= num)
		{
			text3 = "#FF00ffff";
		}
		string text4 = "EquipForgeAcitveAttributePanel_" + countIndex;
		GTextBlockEx gtextBlockEx = goodsCtrlsDict[text4] as GTextBlockEx;
		if (null == gtextBlockEx)
		{
			GTipService.AddBaseAttributeTextBlock("EquipForgeAcitveAttributePanel_" + countIndex, sp, goodsCtrlsDict);
		}
		gtextBlockEx = (goodsCtrlsDict[text4] as GTextBlockEx);
		string text5 = "+";
		if (text2.IndexOf("-") >= 0)
		{
			text5 = string.Empty;
		}
		string lang = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[num2]);
		string text6 = StringUtil.substitute("{0}+{1}  {2} {3}{4}", new object[]
		{
			Global.GetLang("强化"),
			(num >= 10) ? num.ToString() : (num + " "),
			lang,
			text5,
			text2
		});
		string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
		{
			text3,
			text6
		});
		gtextBlockEx.htmlText = colorStringForHtmlText;
		sp.Children.Add(gtextBlockEx);
		gtextBlockEx.Visibility = true;
	}

	private static bool InitBornBaseAttributeTextBlock(Dictionary<string, object> goodsCtrlsDict, GoodsData gd, int extPropIndex, string extPropName, string txtBlockControlName)
	{
		GTextBlockEx gtextBlockEx = goodsCtrlsDict[txtBlockControlName] as GTextBlockEx;
		if (extPropIndex != 8 && extPropIndex != 10 && extPropIndex != 10)
		{
			return false;
		}
		int num = (int)Global.GetEquipBornAddBaseValue(gd, extPropIndex);
		if (num > 0)
		{
			string text = StringUtil.substitute("{0} +{1}", new object[]
			{
				extPropName,
				num
			});
			string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
			{
				"#FF0099FF",
				text
			});
			gtextBlockEx.htmlText = colorStringForHtmlText;
			gtextBlockEx.Visibility = true;
			return true;
		}
		gtextBlockEx.Visibility = false;
		return false;
	}

	private static void ShowWuPingBiaoTiPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData, bool specialHint)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string text = string.Empty;
		string text2 = string.Empty;
		if (specialHint)
		{
			text2 = StringUtil.substitute(Global.GetLang("（已装备）"), new object[0]);
		}
		string text3 = string.Empty;
		if (goodsData != null)
		{
			if (categoriy < 25 && goodsData.Forge_level > 0)
			{
				text = StringUtil.substitute("+{0}", new object[]
				{
					goodsData.Forge_level
				});
			}
			text3 = ((goodsData.AddPropIndex <= 0) ? string.Empty : (Global.GetLang("\t追") + goodsData.AddPropIndex));
		}
		string empty = string.Empty;
		string text4 = StringUtil.substitute(Global.GetLang("{0}{1}{2}｛color=#FFff6600 uline=false tag= text={3}｝｛color=#FFFF00FF uline=false tag= text={4}｝｛color=#FFFF00FF uline=false tag= text={5}｝"), new object[]
		{
			empty,
			goodsXmlNodeByID.Title,
			(text.Length <= 0) ? string.Empty : " ",
			text,
			text3,
			text2
		});
		uint color = Global.GetGoodsColor(goodsData.GoodsID).Color;
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["GoodsTitle"] as GTextBlockEx;
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		Super.FormatTextBlockEx2(gtextBlockEx, text4);
	}

	private static void ShowWuPingJiChuXingXiPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		int categoriy = goodsXmlNodeByID.Categoriy;
		string goodsColorString = Global.GetGoodsColorString(goodsData.GoodsID);
		uint color = Global.ParseStringColor("#ffaddcae").Color;
		string lang = Global.GetLang("物品");
		if (categoriy < 25)
		{
			lang = Global.GetLang("装备");
		}
		string text = StringUtil.substitute(Global.GetLang("{0}类型: ｛color={1} uline=false tag= text={2}｝"), new object[]
		{
			lang,
			goodsColorString,
			Global.GetGoodsType(categoriy)
		});
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["Catetoriy"] as GTextBlockEx;
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		string text2 = "#FFFFFFFF";
		if (goodsXmlNodeByID.ToOccupation >= 0)
		{
			text2 = "#FFFFFFFF";
			string occupationStrByGoods = Global.GetOccupationStrByGoods(goodsXmlNodeByID.ToOccupation);
			text = StringUtil.substitute(Global.GetLang("适用职业: ｛color={0} uline=false tag= text={1}｝"), new object[]
			{
				text2,
				Global.GetLang(occupationStrByGoods)
			});
			gtextBlockEx = (goodsCtrlsDict["Occupation"] as GTextBlockEx);
			gtextBlockEx.TextColor = new SolidColorBrush(color);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
		else
		{
			text = StringUtil.substitute(Global.GetLang("适用职业: ｛color={0} uline=false tag= text={1}｝"), new object[]
			{
				text2,
				Global.GetLang("通用")
			});
			gtextBlockEx = (goodsCtrlsDict["Occupation"] as GTextBlockEx);
			if (categoriy >= 25)
			{
				text = string.Empty;
			}
			gtextBlockEx.TextColor = new SolidColorBrush(color);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
		if (categoriy < 25)
		{
			text2 = "#FFFFFFFF";
			if (Global.Data.roleData.Level < goodsXmlNodeByID.ToLevel)
			{
				text2 = "#FFFF0000";
			}
			text = StringUtil.substitute(Global.GetLang("装备等级: ｛color={0} uline=false tag= text={1}｝"), new object[]
			{
				text2,
				Global.GMax(goodsXmlNodeByID.ToLevel, 1)
			});
			gtextBlockEx = (goodsCtrlsDict["GoodsLevel"] as GTextBlockEx);
			gtextBlockEx.TextColor = new SolidColorBrush(color);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
		else
		{
			text2 = "#FFFFFFFF";
			if (Global.Data.roleData.Level < goodsXmlNodeByID.ToLevel)
			{
				text2 = "#FFFF0000";
			}
			text = StringUtil.substitute(Global.GetLang("使用等级: ｛color={0} uline=false tag= text={1}｝"), new object[]
			{
				text2,
				Global.GMax(goodsXmlNodeByID.ToLevel, 1)
			});
			gtextBlockEx = (goodsCtrlsDict["GoodsLevel"] as GTextBlockEx);
			gtextBlockEx.TextColor = new SolidColorBrush(color);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
		}
	}

	private static void ShowWuPingMiaoShuPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		StackPanel stackPanel = goodsCtrlsDict["DescPanel"] as StackPanel;
		string description = goodsXmlNodeByID.Description;
		if (description != string.Empty)
		{
			stackPanel.Children.Clear();
			string text = StringUtil.substitute("{0}", new object[]
			{
				description
			});
			GTextBlockEx gtextBlockEx = goodsCtrlsDict["Desc"] as GTextBlockEx;
			gtextBlockEx.TextHeight = double.NaN;
			gtextBlockEx.TextColor = new SolidColorBrush(4294967040U);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			stackPanel.Children.Add(gtextBlockEx);
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowZhongLianHeNaiJiuPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["WeightAndStrongPanel"] as StackPanel;
		uint color = Global.ParseStringColor("#ffaddcae").Color;
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["Weight"] as GTextBlockEx;
		string text = StringUtil.substitute(Global.GetLang("重量:｛color=#FFFFFFFF uline=false tag= text={0}｝"), new object[]
		{
			equipFields_1[0]
		});
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		if ((int)equipFields_1[0] != 0)
		{
			gtextBlockEx.Visibility = true;
		}
		else
		{
			gtextBlockEx.Visibility = false;
		}
		gtextBlockEx = (goodsCtrlsDict["Strong"] as GTextBlockEx);
		string text2 = string.Empty;
		if (goodsData.Strong >= (int)equipFields_1[1])
		{
			text2 = Global.GetLang("(已破损，请修理)");
		}
		text = StringUtil.substitute(Global.GetLang("耐久:｛color=#FFFFFFFF uline=false tag= text={0}/{1}｝｛color=#FFFF0000 uline=false tag= text={2}｝"), new object[]
		{
			(int)(equipFields_1[1] / (double)Global.MaxNotifyEquipStrongValue) - goodsData.Strong / Global.MaxNotifyEquipStrongValue,
			(int)(equipFields_1[1] / (double)Global.MaxNotifyEquipStrongValue),
			text2
		});
		gtextBlockEx.TextColor = new SolidColorBrush(color);
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		if ((int)equipFields_1[1] != 0)
		{
			gtextBlockEx.Visibility = true;
		}
		else
		{
			gtextBlockEx.Visibility = false;
		}
		if ((int)equipFields_1[0] != 0 || (int)equipFields_1[1] != 0)
		{
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowQianHuaDengJiShuXingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["ForgeAndStarPanel"] as StackPanel;
		stackPanel.Visibility = true;
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["ForgeLevelTxt"] as GTextBlockEx;
		string text = string.Empty;
		if (goodsData.Forge_level > 0)
		{
			text = StringUtil.substitute(Global.GetLang("【强化等级】:{0}"), new object[]
			{
				goodsData.Forge_level
			});
		}
		else
		{
			text = Global.GetLang("【强化等级】");
		}
		gtextBlockEx.TextColor = Global.ParseStringColor("#ffc8b464");
		Super.FormatTextBlockEx2(gtextBlockEx, text);
		GImgLevel gimgLevel = goodsCtrlsDict["LevelStarImage"] as GImgLevel;
		if (goodsData != null && goodsData.Forge_level > 0)
		{
			gimgLevel.Level = goodsData.Forge_level;
		}
		else
		{
			gimgLevel.Level = 0;
		}
	}

	private static void ShowJiChuShuXingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData, bool changeTitle = false)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipBaseAttributePanel"] as StackPanel;
		stackPanel.Children.Clear();
		string lang = Global.GetLang("【基础属性】");
		if (changeTitle)
		{
			lang = Global.GetLang("【附加属性】");
		}
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["BaseProps"] as GTextBlockEx;
		gtextBlockEx.TextColor = Global.ParseStringColor("#ffc8b464");
		stackPanel.Children.Add(gtextBlockEx);
		Super.FormatTextBlockEx2(gtextBlockEx, lang);
		for (int i = 3; i <= 10; i += 2)
		{
			GTipService.InitBaseAttributeTextBlockEx(goodsCtrlsDict, goodsData, equipFields_1, equipFields_2, i, i + 1, Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]), "EquipPanel_" + ExtPropIndexes.ExtPropIndexNames[i], stackPanel);
		}
		for (int i = 13; i <= 255; i++)
		{
			GTipService.InitBaseAttributeTextBlock(goodsCtrlsDict, goodsData, equipFields_1, equipFields_2, i, Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[i]), "EquipPanel_" + ExtPropIndexes.ExtPropIndexNames[i], 1 == ExtPropIndexes.ExtPropIndexPercents[i], stackPanel);
		}
		stackPanel.Visibility = true;
	}

	private static void ShowQianHuaShuXingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel;
		stackPanel.Children.Clear();
		string lang = Global.GetLang("【强化属性】");
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["EquipForgeBaseAttributeTextBlock"] as GTextBlockEx;
		gtextBlockEx.TextColor = Global.ParseStringColor("#ffc8b464");
		stackPanel.Children.Add(gtextBlockEx);
		Super.FormatTextBlockEx2(gtextBlockEx, lang);
		bool flag = GTipService.InitForgeBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 8, Global.GetLang("最大物理攻击"), "EquipForgeBaseAttributeMaxAttack", stackPanel);
		flag = (GTipService.InitForgeBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 10, Global.GetLang("最大魔法攻击"), "EquipForgeBaseAttributeMaxMAttack", stackPanel) || flag);
		if (!GTipService.InitForgeBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 10, Global.GetLang("最大道术攻击"), "EquipForgeBaseAttributeMaxDAttack", stackPanel) && !flag)
		{
			stackPanel.Visibility = false;
		}
		else
		{
			stackPanel.Visibility = true;
		}
	}

	private static void ShowQianHuaJiHuoShuXingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel;
		stackPanel.Children.Clear();
		string lang = Global.GetLang("【强化激活属性】");
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["EquipForgeAcitveAttributePanelTextBlock"] as GTextBlockEx;
		gtextBlockEx.TextColor = Global.ParseStringColor("#ffc8b464");
		Super.FormatTextBlockEx2(gtextBlockEx, lang);
		stackPanel.Children.Add(gtextBlockEx);
		string[] equipForgeAddActivateList = Global.GetEquipForgeAddActivateList(goodsData.GoodsID);
		for (int i = 0; i < equipForgeAddActivateList.Length; i++)
		{
			GTipService.InitForgeActivateAttributeTextBlock(goodsCtrlsDict, goodsData, equipForgeAddActivateList[i], stackPanel, i);
		}
		if (equipForgeAddActivateList.Length > 0)
		{
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowTianShengShuXingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipBornAttributePanel"] as StackPanel;
		string lang = Global.GetLang("【天生属性】");
		GTextBlockEx gtextBlockEx = goodsCtrlsDict["EquipBornAttributePanelTextBlock"] as GTextBlockEx;
		gtextBlockEx.TextColor = Global.ParseStringColor("#ffc8b464");
		Super.FormatTextBlockEx2(gtextBlockEx, lang);
		bool flag = GTipService.InitBornBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 8, Global.GetLang("最大物理攻击"), "EquipBornBaseAttributeMaxAttack");
		flag = (GTipService.InitBornBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 10, Global.GetLang("最大魔法攻击"), "EquipBornBaseAttributeMaxMAttack") || flag);
		if (!GTipService.InitBornBaseAttributeTextBlock(goodsCtrlsDict, goodsData, 10, Global.GetLang("最大道术攻击"), "EquipBornBaseAttributeMaxDAttack") && !flag)
		{
			stackPanel.Visibility = false;
		}
		else
		{
			stackPanel.Visibility = true;
		}
	}

	private static void ShowPeiDaiXuQiuPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipRequisitePanel"] as StackPanel;
		string text = string.Empty;
		Dictionary<string, int> goodsToTypeLimitString = Global.GetGoodsToTypeLimitString(goodsData.GoodsID);
		foreach (KeyValuePair<string, int> keyValuePair in goodsToTypeLimitString)
		{
			string key = keyValuePair.Key;
			if (key.Length > 0)
			{
				string text2 = "#FFFFFFFF";
				if (goodsToTypeLimitString[key] == 0)
				{
					text2 = "#FFFF0000";
				}
				if (text.Length <= 0)
				{
					text = Global.GetColorStringForHtmlText(new object[]
					{
						"#FFFFFFFF",
						Global.GetLang("佩戴需求:") + " ",
						text2,
						key
					});
				}
				else
				{
					text += Global.GetColorStringForHtmlText(new object[]
					{
						"#FFFFFFFF",
						",",
						text2,
						key
					});
				}
			}
		}
		if (text.Length > 0)
		{
			GTextBlockEx gtextBlockEx = goodsCtrlsDict["EquipRequisitePanelTextBlock"] as GTextBlockEx;
			gtextBlockEx.htmlText = text;
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowXingBieXuQiuPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EquipToSexPanel"] as StackPanel;
		int goodsToSex = Global.GetGoodsToSex(goodsData.GoodsID);
		if (goodsToSex >= 0)
		{
			string text = "#FFFFFFFF";
			if (Global.Data.roleData.RoleSex != goodsToSex)
			{
				text = "#FFFF0000";
			}
			string colorStringForHtmlText = Global.GetColorStringForHtmlText(new object[]
			{
				"#FFFFFFFF",
				Global.GetLang("性别需求:"),
				text,
				(goodsToSex != 0) ? Global.GetLang("女") : Global.GetLang("男")
			});
			GTextBlockEx gtextBlockEx = goodsCtrlsDict["EquipToSexPanelTextBlock"] as GTextBlockEx;
			gtextBlockEx.htmlText = colorStringForHtmlText;
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowCDShiJianPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["OtherPanel"] as StackPanel;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID.CDTime > 0)
		{
			stackPanel.Visibility = true;
			string text = StringUtil.substitute(Global.GetLang("CD时间: {0} 秒"), new object[]
			{
				goodsXmlNodeByID.CDTime
			});
			GTextBlockEx textBlockEx = goodsCtrlsDict["CDTime"] as GTextBlockEx;
			Super.FormatTextBlockEx2(textBlockEx, text);
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowJiaGePanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData, int goodsOwner)
	{
		StackPanel stackPanel = goodsCtrlsDict["PricePanel"] as StackPanel;
		int num = 0;
		if (goodsOwner == 0 && goodsData != null)
		{
			num = Global.GetGoodsSaleToNpcPrice(goodsData);
		}
		if (num > 0)
		{
			GTextBlockEx textBlockEx = goodsCtrlsDict["Price"] as GTextBlockEx;
			string text = StringUtil.substitute(Global.GetLang("单价: ｛color=#FFFF9900 uline=false tag= text={0}\u3000绑定金币｝"), new object[]
			{
				num
			});
			Super.FormatTextBlockEx2(textBlockEx, text);
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowBangDingPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["BindingPanel"] as StackPanel;
		string text = string.Empty;
		if (goodsData != null && goodsData.Binding > 0)
		{
			text = Global.GetLang("已绑定");
		}
		if (text != string.Empty)
		{
			GTextBlockEx gtextBlockEx = goodsCtrlsDict["Binding"] as GTextBlockEx;
			gtextBlockEx.TextColor = new SolidColorBrush(4294967040U);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowJieShuShiJianPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData)
	{
		StackPanel stackPanel = goodsCtrlsDict["EndTimePanel"] as StackPanel;
		string text = string.Empty;
		if (goodsData != null && Global.IsTimeLimitGoods(goodsData))
		{
			text = Super.FormatGoodsOverTime(goodsData.Endtime);
		}
		if (text != string.Empty)
		{
			GTextBlockEx gtextBlockEx = goodsCtrlsDict["EndTime1"] as GTextBlockEx;
			gtextBlockEx.TextColor = new SolidColorBrush(4294901760U);
			if (!Global.IsGoodsTimeOver(goodsData))
			{
				Super.FormatTextBlockEx2(gtextBlockEx, Global.GetLang("过期失效时间"));
			}
			else
			{
				Super.FormatTextBlockEx2(gtextBlockEx, Global.GetLang("物品已经过期"));
			}
			gtextBlockEx = (goodsCtrlsDict["EndTime2"] as GTextBlockEx);
			gtextBlockEx.TextColor = new SolidColorBrush(4294901760U);
			Super.FormatTextBlockEx2(gtextBlockEx, text);
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void ShowWuPingShuLiangPanel(Dictionary<string, object> goodsCtrlsDict, double[] equipFields_1, double[] equipFields_2, GoodsData goodsData, int goodsOwner)
	{
		StackPanel stackPanel = goodsCtrlsDict["GoodsNumPanel"] as StackPanel;
		int num = 0;
		if (goodsData != null)
		{
			num = goodsData.GCount;
			if (goodsData.Id < 0 && goodsOwner == -1)
			{
				num = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
			}
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID.UsingNum > 1)
		{
			GTextBlockEx textBlockEx = goodsCtrlsDict["GoodsNum"] as GTextBlockEx;
			string text = StringUtil.substitute(Global.GetLang("剩余次数: ｛color=#FFFFFFFF uline=false tag= text={0}｝"), new object[]
			{
				num
			});
			Super.FormatTextBlockEx2(textBlockEx, text);
			stackPanel.Visibility = true;
		}
		else
		{
			stackPanel.Visibility = false;
		}
	}

	private static void HideNonEquimentPanel(Dictionary<string, object> goodsCtrlsDict)
	{
		(goodsCtrlsDict["ForgeAndStarPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipBaseAttributePanel"] as StackPanel).Clear();
		(goodsCtrlsDict["EquipBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Clear();
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Clear();
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Clear();
		(goodsCtrlsDict["EquipForgeBaseAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Clear();
		(goodsCtrlsDict["EquipForgeAcitveAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipBornAttributePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipRequisitePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EquipToSexPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["PricePanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["GoodsNumPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["BindingPanel"] as StackPanel).Visibility = false;
		(goodsCtrlsDict["EndTimePanel"] as StackPanel).Visibility = false;
	}

	private static void InitGoodsWrapPanel(Dictionary<string, object> goodsCtrlsDict, StackPanel wrapPanel, string goodsID, int priceType, int goodsDbID, int goodsOwner, bool specialHint = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID.SafeToInt32(0));
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		GoodsData showGoodsDataBy = Super.GetShowGoodsDataBy(goodsDbID, goodsOwner, goodsID.SafeToInt32(0));
		if (showGoodsDataBy == null)
		{
			return;
		}
		if (showGoodsDataBy != null && (showGoodsDataBy.Forge_level < 0 || showGoodsDataBy.Forge_level > Global.MaxForgeLevel))
		{
			showGoodsDataBy.Forge_level = 0;
		}
		int categoriy = goodsXmlNodeByID.Categoriy;
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID.SafeToInt32(0));
		double[] equipFields_ = goodsEquipPropsDoubleList;
		if (goodsDbID > 0)
		{
			goodsEquipPropsDoubleList[17] = goodsEquipPropsDoubleList[17] + (double)showGoodsDataBy.Lucky;
		}
		GTipService.ShowWuPingBiaoTiPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy, specialHint);
		GTipService.ShowWuPingJiChuXingXiPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		(goodsCtrlsDict["GoodsIcon"] as URLImage).URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			goodsXmlNodeByID.IconCode
		});
		GTipService.ShowWuPingMiaoShuPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		GTipService.ShowZhongLianHeNaiJiuPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		if (categoriy < 25)
		{
			GTipService.ShowQianHuaDengJiShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
			GTipService.ShowJiChuShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy, false);
			GTipService.ShowQianHuaShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
			GTipService.ShowQianHuaJiHuoShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
			GTipService.ShowTianShengShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
			GTipService.ShowPeiDaiXuQiuPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		}
		else
		{
			GTipService.HideNonEquimentPanel(goodsCtrlsDict);
			GTipService.ShowCDShiJianPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
			if (categoriy >= 251 && categoriy <= 253)
			{
				GTipService.ShowJiChuShuXingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy, true);
			}
		}
		GTipService.ShowXingBieXuQiuPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		GTipService.ShowBangDingPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		GTipService.ShowJieShuShiJianPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy);
		GTipService.ShowWuPingShuLiangPanel(goodsCtrlsDict, goodsEquipPropsDoubleList, equipFields_, showGoodsDataBy, goodsOwner);
	}

	private static void InitGoodsTipWindow2()
	{
		GTipService.GoodsWrapPanel_2 = GTipService.GetGoodsWrapPanel(GTipService.GoodsCtrlsDict_2);
		GTipService.GoodsTipWindow_2 = GTipService.ShowTipWindow(0, 0, GTipService.GoodsWrapPanel_2, 10, 10, 0, 0);
	}

	private static void ShowGoodsTipWindow2(int x, int y)
	{
		GTipService.GoodsTipWindow_2.Left = Math.Floor((double)x);
		GTipService.GoodsTipWindow_2.Top = Math.Floor((double)y);
		GTipService.GoodsTipWindow_2.Visibility = true;
		GTipService.BringWindowToTop(GTipService.GoodsTipWindow_2);
	}

	private static void PreShowGoodsTipWindow2(object e, string[] fields, bool caching = false)
	{
		if (null == GTipService.GoodsTipWindow_2)
		{
			GTipService.InitGoodsTipWindow2();
		}
		if (null == GTipService.GoodsTipWindow_2)
		{
			return;
		}
		try
		{
			GTipService.InitGoodsWrapPanel(GTipService.GoodsCtrlsDict_2, GTipService.GoodsWrapPanel_2, fields[0], Convert.ToInt32(fields[1]), Convert.ToInt32(fields[2]), Convert.ToInt32(fields[3]), true);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
		GTipService.GoodsWrapPanel_2.Measure(new SizeSL(double.PositiveInfinity, double.PositiveInfinity));
		int num = (int)GTipService.GoodsWrapPanel_2.DesiredSize.Width + 20;
		int num2 = (int)GTipService.GoodsWrapPanel_2.DesiredSize.Height + 20;
		GTipService.GoodsTipWindow_2.BodyWidth = (double)num;
		GTipService.GoodsTipWindow_2.BodyHeight = (double)num2;
		if (caching)
		{
			GTipService.GoodsTipWindow_2.Visibility = false;
		}
	}

	private static void HideGoodsTipWindow2()
	{
		if (null == GTipService.GoodsTipWindow_2)
		{
			return;
		}
		GTipService.GoodsTipWindow_2.Visibility = false;
	}

	private static void InitGoodsTipWindow3()
	{
		GTipService.GoodsWrapPanel_3 = GTipService.GetGoodsWrapPanel(GTipService.GoodsCtrlsDict_3);
		GTipService.GoodsTipWindow_3 = GTipService.ShowTipWindow(0, 0, GTipService.GoodsWrapPanel_3, 10, 10, 0, 0);
	}

	private static void ShowGoodsTipWindow3(int x, int y)
	{
		GTipService.GoodsTipWindow_3.Left = Math.Floor((double)x);
		GTipService.GoodsTipWindow_3.Top = Math.Floor((double)y);
		GTipService.GoodsTipWindow_3.Visibility = true;
		GTipService.BringWindowToTop(GTipService.GoodsTipWindow_3);
	}

	private static void PreShowGoodsTipWindow3(object e, string[] fields, bool caching = false)
	{
		if (null == GTipService.GoodsTipWindow_3)
		{
			GTipService.InitGoodsTipWindow3();
		}
		if (null == GTipService.GoodsTipWindow_3)
		{
			return;
		}
		try
		{
			GTipService.InitGoodsWrapPanel(GTipService.GoodsCtrlsDict_3, GTipService.GoodsWrapPanel_3, fields[0], Convert.ToInt32(fields[1]), Convert.ToInt32(fields[2]), Convert.ToInt32(fields[3]), true);
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		finally
		{
		}
		GTipService.GoodsWrapPanel_3.Measure(new SizeSL(double.PositiveInfinity, double.PositiveInfinity));
		int num = (int)GTipService.GoodsWrapPanel_3.DesiredSize.Width + 20;
		int num2 = (int)GTipService.GoodsWrapPanel_3.DesiredSize.Height + 20;
		GTipService.GoodsTipWindow_3.BodyWidth = (double)num;
		GTipService.GoodsTipWindow_3.BodyHeight = (double)num2;
		if (caching)
		{
			GTipService.GoodsTipWindow_3.Visibility = false;
		}
	}

	private static void HideGoodsTipWindow3()
	{
		if (null == GTipService.GoodsTipWindow_3)
		{
			return;
		}
		GTipService.GoodsTipWindow_3.Visibility = false;
	}

	private static void InitGeneralTipWindow()
	{
		GTipService.GeneralWrapPanel = new StackPanel();
		GTipService.GeneralWrapPanel.Orientation = global::Layout.Vertical;
		GTipService.GeneralTipWindow = GTipService.ShowTipWindow(0, 0, GTipService.GeneralWrapPanel, 10, 10, 0, 0);
	}

	private static void ShowGeneralTipWindow(object e, int tipType, string tipText)
	{
		if (null == GTipService.GeneralTipWindow)
		{
			GTipService.InitGeneralTipWindow();
		}
		if (null == GTipService.GeneralTipWindow)
		{
			return;
		}
		try
		{
			if (tipType == 4)
			{
				GTipService.InitExternalWrapPanel(tipText);
			}
			else if (tipType == 5)
			{
				GTipService.InitBufferItemTipWrapPanel(tipText);
			}
			else if (tipType == 6)
			{
				GTipService.InitExperienceTipWrapPanel(tipText);
			}
			else if (tipType == 7)
			{
				GTipService.InitLingLiTipWrapPanel(tipText);
			}
			else if (tipType == 8)
			{
				GTipService.InitLifeSliderTipWrapPanel(tipText);
			}
			else if (tipType == 9)
			{
				GTipService.InitMagicSliderTipWrapPanel(tipText);
			}
			else if (tipType == 10)
			{
				GTipService.InitBonusTipWrapPanel(tipText);
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		GTipService.GeneralWrapPanel.Measure(new SizeSL(double.PositiveInfinity, double.PositiveInfinity));
		int num = (int)GTipService.GeneralWrapPanel.DesiredSize.Width + 20;
		int num2 = (int)GTipService.GeneralWrapPanel.DesiredSize.Height + 20;
		GTipService.GeneralTipWindow.BodyWidth = (double)num;
		GTipService.GeneralTipWindow.BodyHeight = (double)num2;
		Point point = GTipService.CalcPoint(e, (int)GTipService.GeneralTipWindow.BodyWidth, (int)GTipService.GeneralTipWindow.BodyHeight);
		GTipService.GeneralTipWindow.Left = Math.Floor((double)point.X);
		GTipService.GeneralTipWindow.Top = Math.Floor((double)point.Y);
		GTipService.GeneralTipWindow.Visibility = true;
		GTipService.BringWindowToTop(GTipService.GeneralTipWindow);
	}

	private static void HideGeneralTipWindow()
	{
		if (null == GTipService.GeneralTipWindow)
		{
			return;
		}
		GTipService.GeneralTipWindow.Visibility = false;
	}

	private static void InitExternalWrapPanel(string tipID)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string systemTipByID = Global.GetSystemTipByID(tipID);
		if (string.IsNullOrEmpty(systemTipByID))
		{
			return;
		}
		char[] array = new char[]
		{
			'$'
		};
		string[] array2 = systemTipByID.Split(array);
		for (int i = 0; i < array2.Length; i++)
		{
			GTextBlockEx textBlock = GTipService.GetTextBlock2(array2[i], HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
			generalWrapPanel.Children.Add(textBlock);
		}
	}

	private static void InitBufferItemTipWrapPanel(string tipID)
	{
		if (string.IsNullOrEmpty(tipID))
		{
			return;
		}
		int num = Global.SafeConvertToInt32(tipID);
		if (num <= 0)
		{
			return;
		}
		BufferData bufferDataByID = Global.GetBufferDataByID(num);
		if (bufferDataByID == null)
		{
			return;
		}
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string text = string.Empty;
		string text2 = string.Empty;
		if (bufferDataByID.BufferID == 1)
		{
			text = Global.GetLang("双倍经验卡：在剩余时间内您打怪的经验收益将会翻倍");
		}
		else if (bufferDataByID.BufferID == 2)
		{
			text = Global.GetLang("双倍绑定金币卡：在剩余时间内您打怪的绑定金币收益将会翻倍");
		}
		else if (bufferDataByID.BufferID == 3)
		{
			text = Global.GetLang("双倍灵力卡：在剩余时间内您打坐的灵力收益将会翻倍");
		}
		else if (bufferDataByID.BufferID == 4)
		{
			text = Global.GetLang("每隔5秒为您补充生命值10000，直至持续时间停止为止。");
		}
		else if (bufferDataByID.BufferID == 5)
		{
			text = Global.GetLang("每隔5秒为您补充法力值10000，直至持续时间停止为止。");
		}
		else if (bufferDataByID.BufferID == 6)
		{
			text = Global.GetLang("一定时间内您的攻击力得到了提高");
		}
		else if (bufferDataByID.BufferID == 7)
		{
			text = Global.GetLang("一定时间内您的防卸力得到了提高");
		}
		else if (bufferDataByID.BufferID == 8)
		{
			text = Global.GetLang("一定时间内您的生命上限得到了提高");
		}
		else if (bufferDataByID.BufferID == 9)
		{
			text = Global.GetLang("一定时间内您的魔法上限得到了提高");
		}
		else if (bufferDataByID.BufferID == 10)
		{
			text = Global.GetLang("当您的灵力值降低时，将会自动恢复您的灵力值，直至剩余容量全部消耗完毕为止。");
		}
		else if (bufferDataByID.BufferID == 11)
		{
			text = StringUtil.substitute(Global.GetLang("【{0}】连斩次数达到{1}次以上，击杀精英和BOSS时造成{2}倍伤害"), new object[]
			{
				Global.GetLianZhanBufferName((int)(bufferDataByID.BufferVal - 2L)),
				(bufferDataByID.BufferVal - 1L) * 100L,
				bufferDataByID.BufferVal
			});
		}
		else if (bufferDataByID.BufferID == 12)
		{
			int battleBufferNum = Global.GetBattleBufferNum((int)bufferDataByID.BufferVal);
			text = StringUtil.substitute(Global.GetLang("【{0}】隋唐战场获得第{1}名，PK时对对方造成的伤害增加{2}%"), new object[]
			{
				Global.GetBattleBufferName(battleBufferNum),
				battleBufferNum + 1,
				bufferDataByID.BufferVal
			});
		}
		else if (bufferDataByID.BufferID == 13)
		{
			text = string.Concat(new string[]
			{
				Global.GetVipTypeNameString(),
				Global.GetLang("，"),
				Global.GetLang("在剩余时间内享受以下特权：\n\n"),
				Global.GetVipPriorityString(Global.GetVipType()),
				"\n"
			});
		}
		else if (bufferDataByID.BufferID == 14)
		{
			text = Global.GetLang("乱世人人垂涎欲滴的超强能量之物,可为您加成：【基础属性】 物攻+10% 魔攻+10% 物防+10% 魔防+10% 生命上限+20% 【特殊属性】 回血能力+200% 回魔能力+200%");
		}
		else if (bufferDataByID.BufferID == 15)
		{
			text = Global.GetLang("皇妃受帝王宠幸,获得帝王之佑守护。 可加成：【基础属性】 物攻+5% 魔攻+5% 物防+5% 魔防+5% 生命上限+10% 【特殊属性】 回血能力+100% 回魔能力+100%");
		}
		else if (bufferDataByID.BufferID == 17)
		{
			text = Global.GetLang("双倍技能卡：在剩余的时间内您获得的技能熟练度点将翻倍");
		}
		else if (bufferDataByID.BufferID == 18)
		{
			text = Global.GetLang("三倍经验卡：在剩余的时间内您打怪可获得3倍经验收益");
		}
		else if (bufferDataByID.BufferID == 36)
		{
			text = Global.GetLang("五倍经验卡：在剩余的时间内您打怪可获得4倍经验收益");
		}
		else if (bufferDataByID.BufferID == 46)
		{
			int num2 = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
			text = StringUtil.substitute(Global.GetLang("{0}倍经验卡：在剩余的时间内您打怪可获得{1}倍经验收益"), new object[]
			{
				num2,
				num2 - 1
			});
		}
		else if (bufferDataByID.BufferID == 48)
		{
			int num3 = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
			int id = (int)(bufferDataByID.BufferVal - (long)num3) / (int)Math.Pow(2.0, 32.0);
			string goodsNameByID = Global.GetGoodsNameByID(id, false);
			string text3 = string.Empty;
			if (num3 > 1)
			{
				text3 = StringUtil.substitute(Global.GetLang("{0}倍"), new object[]
				{
					num3 - 1
				});
			}
			text = StringUtil.substitute(Global.GetLang("{0}：在剩余的时间内您在安全区烤火可获得{1}烤火经验收益"), new object[]
			{
				goodsNameByID,
				text3
			});
		}
		else if (bufferDataByID.BufferID == 19)
		{
			text = Global.GetLang("三倍绑定金币卡：在剩余的时间内您打怪可获得3倍绑定金币收益");
		}
		else if (bufferDataByID.BufferID == 20)
		{
			text = Global.GetLang("挂机保护卡：开启挂机5分钟后，自动进入挂机保护状态，挂机期间无法被其他玩家攻击, 也无法攻击其他玩家");
		}
		else if (bufferDataByID.BufferID == 33)
		{
			int bufferGoodsID = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID);
			text = StringUtil.substitute(Global.GetLang("您当前的武学修为为{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID, false),
				goodsEquipPropsStringForBufferTips
			});
		}
		else if (bufferDataByID.BufferID == 32)
		{
			int bufferGoodsID2 = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips2 = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID2);
			text = StringUtil.substitute(Global.GetLang("您当前已激活{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID2, false),
				goodsEquipPropsStringForBufferTips2
			});
		}
		else if (bufferDataByID.BufferID == 50)
		{
			int bufferGoodsID3 = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips3 = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID3);
			text = StringUtil.substitute(Global.GetLang("您当前已激活{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID3, false),
				goodsEquipPropsStringForBufferTips3
			});
		}
		else if (bufferDataByID.BufferID == 51)
		{
			int bufferGoodsID4 = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips4 = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID4);
			text = StringUtil.substitute(Global.GetLang("您当前已激活{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID4, false),
				goodsEquipPropsStringForBufferTips4
			});
		}
		else if (bufferDataByID.BufferID == 16)
		{
			int bufferGoodsID5 = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips5 = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID5);
			text = StringUtil.substitute(Global.GetLang("{0}可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID5, false),
				goodsEquipPropsStringForBufferTips5
			});
		}
		else if (bufferDataByID.BufferID == 31)
		{
			int bufferGoodsID6 = Global.GetBufferGoodsID(bufferDataByID.BufferID, (int)bufferDataByID.BufferVal);
			string goodsEquipPropsStringForBufferTips6 = Global.GetGoodsEquipPropsStringForBufferTips(bufferGoodsID6);
			text = StringUtil.substitute(Global.GetLang("您当前的人物成就为{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(bufferGoodsID6, false),
				goodsEquipPropsStringForBufferTips6
			});
		}
		else if (bufferDataByID.BufferID == 47)
		{
			int num4 = (int)bufferDataByID.BufferVal;
			string goodsEquipPropsStringForBufferTips7 = Global.GetGoodsEquipPropsStringForBufferTips(num4);
			text = StringUtil.substitute(Global.GetLang("您当前的节日称号为{0},可为您带来如下属性加成：\n\n{1} \n\n"), new object[]
			{
				Global.GetGoodsNameByID(num4, false),
				goodsEquipPropsStringForBufferTips7
			});
		}
		else if (bufferDataByID.BufferID == 27)
		{
			int goodsId = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList = Global.GetGoodsExecMagicdoubleParamsList(goodsId, string.Empty);
			if (goodsExecMagicdoubleParamsList.Length == 4)
			{
				text = StringUtil.substitute(Global.GetLang("生命值回复{0}点/秒\n魔法值回复{1}点/秒\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList[0] / (int)goodsExecMagicdoubleParamsList[3],
					(int)goodsExecMagicdoubleParamsList[1] / (int)goodsExecMagicdoubleParamsList[3]
				});
			}
		}
		else if (bufferDataByID.BufferID == 24)
		{
			int goodsId2 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList2 = Global.GetGoodsExecMagicdoubleParamsList(goodsId2, string.Empty);
			if (goodsExecMagicdoubleParamsList2.Length == 2)
			{
				text = StringUtil.substitute(Global.GetLang("最大物理攻击+{0}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList2[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 25)
		{
			int goodsId3 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList3 = Global.GetGoodsExecMagicdoubleParamsList(goodsId3, string.Empty);
			if (goodsExecMagicdoubleParamsList3.Length == 2)
			{
				text = StringUtil.substitute(Global.GetLang("最大魔法攻击+{0}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList3[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 26)
		{
			int goodsId4 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList4 = Global.GetGoodsExecMagicdoubleParamsList(goodsId4, string.Empty);
			if (goodsExecMagicdoubleParamsList4.Length == 2)
			{
				text = StringUtil.substitute(Global.GetLang("最大道术攻击+{0}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList4[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 22)
		{
			int goodsId5 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList5 = Global.GetGoodsExecMagicdoubleParamsList(goodsId5, string.Empty);
			if (goodsExecMagicdoubleParamsList5.Length == 2)
			{
				text = StringUtil.substitute(Global.GetLang("物理防御+{0}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList5[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 23)
		{
			int goodsId6 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList6 = Global.GetGoodsExecMagicdoubleParamsList(goodsId6, string.Empty);
			if (goodsExecMagicdoubleParamsList6.Length == 2)
			{
				text = StringUtil.substitute(Global.GetLang("魔法防御+{0}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList6[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 39)
		{
			int goodsId7 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList7 = Global.GetGoodsExecMagicdoubleParamsList(goodsId7, string.Empty);
			if (goodsExecMagicdoubleParamsList7.Length == 4)
			{
				text = StringUtil.substitute(Global.GetLang("怒斩·PK王可为您加成如下属性:\n"), new object[0]);
				text += StringUtil.substitute(Global.GetLang("最大物理攻击+{0}\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList7[0]
				});
				text += StringUtil.substitute(Global.GetLang("最大魔法攻击+{0}\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList7[1]
				});
				text += StringUtil.substitute(Global.GetLang("最大道术攻击+{0}\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList7[2]
				});
				text += StringUtil.substitute(Global.GetLang("杀怪经验增加{0}倍\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList7[3]
				});
			}
		}
		else if (bufferDataByID.BufferID == 29)
		{
			text = StringUtil.substitute(Global.GetLang("烤火\n\n"), new object[0]);
		}
		else if (bufferDataByID.BufferID == 21)
		{
			int goodsId8 = (int)bufferDataByID.BufferVal;
			double[] goodsExecMagicdoubleParamsList8 = Global.GetGoodsExecMagicdoubleParamsList(goodsId8, string.Empty);
			if (goodsExecMagicdoubleParamsList8.Length == 3)
			{
				text = StringUtil.substitute(Global.GetLang("在剩余的时间内您可获得如下经验收益：{0}EXP/分钟\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList8[0]
				});
			}
		}
		else if (bufferDataByID.BufferID == 30)
		{
			text = StringUtil.substitute(Global.GetLang("在剩余的时间内击杀BOSS掉落的装备{0}%激活天生属性\n\n"), new object[]
			{
				bufferDataByID.BufferVal
			});
		}
		else if (bufferDataByID.BufferID == 28)
		{
			int bufferSecs = bufferDataByID.BufferSecs;
			double[] goodsExecMagicdoubleParamsList9 = Global.GetGoodsExecMagicdoubleParamsList(bufferSecs, string.Empty);
			if (goodsExecMagicdoubleParamsList9.Length == 4)
			{
				text = StringUtil.substitute(Global.GetLang("每击杀{0}只怪物可额外获取高额经验\n被杀的怪物等级必须大于自身等级\n等级越高经验越高\n杀满{1}只怪替身娃娃将消失\n剩余击杀怪物数:{2}\n\n"), new object[]
				{
					(int)goodsExecMagicdoubleParamsList9[0],
					(int)goodsExecMagicdoubleParamsList9[1],
					bufferDataByID.BufferVal
				});
			}
		}
		else if (bufferDataByID.BufferID == 34)
		{
			text = StringUtil.substitute(Global.GetLang("古墓修炼:古墓安全地图挂机修炼\n\n"), new object[0]);
		}
		if (bufferDataByID.BufferID == 4 || bufferDataByID.BufferID == 5 || bufferDataByID.BufferID == 10)
		{
			text2 = StringUtil.substitute(Global.GetLang("剩余储量：{0}"), new object[]
			{
				bufferDataByID.BufferVal
			});
			if (bufferDataByID.BufferID == 4 || bufferDataByID.BufferID == 5)
			{
				double num5 = (double)(bufferDataByID.BufferVal / 1000L);
				if (num5 < 3600.0)
				{
					text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}分钟{1}秒"), new object[]
					{
						(int)(num5 / 60.0),
						(int)(num5 % 60.0)
					});
				}
				else
				{
					double num6 = num5 % 3600.0;
					text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}小时{1}分钟{2}秒"), new object[]
					{
						(int)(num5 / 3600.0),
						(int)(num6 / 60.0),
						(int)(num6 % 60.0)
					});
				}
			}
		}
		else if (bufferDataByID.BufferID == 34)
		{
			double num7 = 0.0;
			if (bufferDataByID.StartTime == (long)DateTime.Now.DayOfYear)
			{
				num7 = (double)Math.Max(0L, bufferDataByID.BufferVal);
			}
			num7 += (double)Math.Max(0, bufferDataByID.BufferSecs);
			if (num7 < 3600.0)
			{
				text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}分钟{1}秒"), new object[]
				{
					(int)(num7 / 60.0),
					(int)(num7 % 60.0)
				});
			}
			else
			{
				double num8 = num7 % 3600.0;
				text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}小时{1}分钟{2}秒"), new object[]
				{
					(int)(num7 / 3600.0),
					(int)(num8 / 60.0),
					(int)(num8 % 60.0)
				});
			}
		}
		else if (bufferDataByID.BufferID == 48)
		{
			double num9 = (double)bufferDataByID.BufferSecs;
			double num10 = num9 % 3600.0;
			text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}小时{1}分钟{2}秒"), new object[]
			{
				(int)(num9 / 3600.0),
				(int)(num10 / 60.0),
				(int)(num10 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 40)
		{
			double num11 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num12 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num13 = (num11 - num12) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("治疗:持续恢复人物血量\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num13 / 60.0),
				(int)(num13 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 41)
		{
			double num14 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num15 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num16 = (num14 - num15) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("隐身:人物进入隐身状态\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num16 / 60.0),
				(int)(num16 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 42)
		{
			double num17 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num18 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num19 = (num17 - num18) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("中毒:降低防御并造成持续伤害\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num19 / 60.0),
				(int)(num19 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 43)
		{
			double num20 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num21 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num22 = (num20 - num21) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("神圣战甲术:物理防御增强\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num22 / 60.0),
				(int)(num22 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 44)
		{
			double num23 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num24 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num25 = (num23 - num24) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("幽灵盾:魔法防御增强\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num25 / 60.0),
				(int)(num25 % 60.0)
			});
		}
		else if (bufferDataByID.BufferID == 45)
		{
			double num26 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num27 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num28 = (num26 - num27) / 1000.0;
			text2 = StringUtil.substitute(Global.GetLang("魔法盾：吸收一定比例伤害\n剩余时间：{0}分钟{1}秒"), new object[]
			{
				(int)(num28 / 60.0),
				(int)(num28 % 60.0)
			});
		}
		else if (bufferDataByID.BufferType <= 0 && bufferDataByID.BufferID != 28 && bufferDataByID.BufferID != 33 && bufferDataByID.BufferID != 31)
		{
			double num29 = (double)bufferDataByID.BufferSecs * 1000.0;
			double num30 = (double)(Global.GetCorrectLocalTime() - bufferDataByID.StartTime);
			double num31 = Math.Max((num29 - num30) / 1000.0, 0.0);
			if (num31 < 3600.0)
			{
				text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}分钟{1}秒"), new object[]
				{
					(int)(num31 / 60.0),
					(int)(num31 % 60.0)
				});
			}
			else if (num31 < 86400.0)
			{
				double num32 = num31 % 3600.0;
				text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}小时{1}分钟{2}秒"), new object[]
				{
					(int)(num31 / 3600.0),
					(int)(num32 / 60.0),
					(int)(num32 % 60.0)
				});
			}
			else
			{
				double num33 = num31 % 86400.0;
				text2 = StringUtil.substitute(Global.GetLang("剩余时间：{0}天{1}小时"), new object[]
				{
					(int)(num31 / 86400.0),
					(int)(num33 / 3600.0)
				});
			}
		}
		else
		{
			text2 = Global.GetLang("无时间限制");
		}
		GTextBlockEx textBlock = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(text2, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
	}

	private static void InitExperienceTipWrapPanel(string tip)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string lang = Global.GetLang("通过主线任务、日常任务、通天塔、古墓地图挂机等途径提升等级");
		GTextBlockEx textBlock = GTipService.GetTextBlock2(tip, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(lang, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
	}

	private static void InitLingLiTipWrapPanel(string tip)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string lang = Global.GetLang("通过做闭关修炼、打坐等获得的灵力会存储于您的灵力池中。使用玄天鼎可以将聚灵珠(满)中的灵力提取到灵力池中。使用聚灵珠(空)可以吸取灵力池中的灵力交易给其他玩家使用。");
		string lang2 = Global.GetLang("灵力非常重要，冲脉提升属性需要大量的灵力。灵力蓄满不再增长，请及时使用。");
		GTextBlockEx textBlock = GTipService.GetTextBlock2(tip, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(lang, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(lang2, HSTextField.defaultFontSize, 4294901760U, 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
	}

	private static void InitLifeSliderTipWrapPanel(string tip)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string text = StringUtil.substitute(Global.GetLang("当您的生命值低于:｛color=#FFFF0000 uline=false tag= text={0}｝时会自动使用补血药"), new object[]
		{
			tip
		});
		string lang = Global.GetLang("可以左右拖动滑块改变自动补血设置");
		GTextBlockEx textBlock = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(lang, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
	}

	private static void InitMagicSliderTipWrapPanel(string tip)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		string text = StringUtil.substitute(Global.GetLang("当您的魔法值低于:｛color=#FFFF0000 uline=false tag= text={0}｝时会自动使用补魔法的药"), new object[]
		{
			tip
		});
		string lang = Global.GetLang("可以左右拖动滑块改变自动补魔法设置");
		GTextBlockEx textBlock = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		generalWrapPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(lang, HSTextField.defaultFontSize, ColorSL.FromArgb(255, 0, 255, 0), 250.0, null);
		generalWrapPanel.Children.Add(textBlock);
	}

	private static void InitBonusTipWrapPanel(string tip)
	{
		StackPanel generalWrapPanel = GTipService.GeneralWrapPanel;
		generalWrapPanel.Children.Clear();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int occupation = 0;
		string[] array = tip.Split(new char[]
		{
			','
		});
		if (array.Length == 4)
		{
			num = Global.SafeConvertToInt32(array[0]);
			num2 = Global.SafeConvertToInt32(array[1]);
			num3 = Global.SafeConvertToInt32(array[2]);
			occupation = Global.SafeConvertToInt32(array[3]);
		}
		string text = Global.GetLang("【组合套装自动激活列表】");
		GTextBlockEx gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		gtextBlockEx.HorizontalAlignment = global::Layout.Center;
		gtextBlockEx.BodyWidth = 200.0;
		gtextBlockEx.TextWidth = 200.0;
		gtextBlockEx.TextWrapping = TextWrapping.Wrap;
		generalWrapPanel.Children.Add(gtextBlockEx);
		StackPanel stackPanel = new StackPanel();
		stackPanel.Width = 280.0;
		stackPanel.Margin = new Thickness(10.0, 10.0, 0.0, 0.0);
		text = Global.GetLang("高品质套装效果");
		uint color = (num <= 0) ? 4286611584U : 4294967040U;
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, color, 280.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		text = Global.GetLang("激活条件：全身装备品质达到紫色以上，或全身装备品质达到金色");
		color = ((num <= 0) ? 4286611584U : 4294944000U);
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, color, 280.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		Canvas canvas = new Canvas();
		canvas.Width = 200.0;
		canvas.Height = 60.0;
		canvas.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		canvas.Background = new ImageBrush(Global.GetGameResImage("Images/Plate/tip_bak4.png"));
		text = StringUtil.substitute(Global.GetLang("全紫    {0} +{1}%  {2} +{3}%  {4} +{5}%"), new object[]
		{
			Global.GetLang("物防"),
			Global.GetAllQualityDefensePercent(1),
			Global.GetLang("魔防"),
			Global.GetAllQualityDefensePercent(1),
			Global.GetLang("生命上限"),
			Global.GetAllQualityDefensePercent(1) * 10
		});
		color = ((num != 1) ? 4286611584U : 4286578816U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		text = StringUtil.substitute(Global.GetLang("全金    {0} +{1}%  {2} +{3}%  {4} +{5}%"), new object[]
		{
			Global.GetLang("物防"),
			Global.GetAllQualityDefensePercent(2),
			Global.GetLang("魔防"),
			Global.GetAllQualityDefensePercent(2),
			Global.GetLang("生命上限"),
			Global.GetAllQualityDefensePercent(2) * 10
		});
		color = ((num != 2) ? 4286611584U : ColorSL.FromArgb(255, 255, 215, 0));
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 40);
		stackPanel.Children.Add(canvas);
		generalWrapPanel.Children.Add(stackPanel);
		stackPanel = new StackPanel();
		stackPanel.Width = 280.0;
		stackPanel.Margin = new Thickness(10.0, 10.0, 0.0, 0.0);
		text = Global.GetLang("高强化套装效果");
		color = ((num2 <= 0) ? 4286611584U : 4294967040U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		stackPanel.Children.Add(gtextBlockEx);
		text = Global.GetLang("激活条件：全身装备强化达到5星、7星或10星");
		color = ((num2 <= 0) ? 4286611584U : 4294944000U);
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, color, 280.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		canvas = new Canvas();
		canvas.Width = 200.0;
		canvas.Height = 100.0;
		canvas.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		canvas.Background = new ImageBrush(Global.GetGameResImage("Images/Plate/tip_bak3.png"));
		text = StringUtil.substitute(Global.GetLang("5星    {0} +{1}%  {2} +{3}%"), new object[]
		{
			Global.GetLang("物攻"),
			Global.GetAllForgeLevelAttackPercent(1),
			Global.GetLang("魔攻"),
			Global.GetAllForgeLevelAttackPercent(1)
		});
		color = ((num2 != 1) ? 4286611584U : 4294967040U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		text = StringUtil.substitute(Global.GetLang("7星    {0} +{1}%  {2} +{3}%"), new object[]
		{
			Global.GetLang("物攻"),
			Global.GetAllForgeLevelAttackPercent(2),
			Global.GetLang("魔攻"),
			Global.GetAllForgeLevelAttackPercent(2)
		});
		color = ((num2 != 2) ? 4286611584U : 4294967040U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 40);
		text = StringUtil.substitute(Global.GetLang("10星    {0} +{1}%  {2} +{3}%"), new object[]
		{
			Global.GetLang("物攻"),
			Global.GetAllForgeLevelAttackPercent(3),
			Global.GetLang("魔攻"),
			Global.GetAllForgeLevelAttackPercent(3)
		});
		color = ((num2 != 3) ? 4286611584U : 4294967040U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 80);
		stackPanel.Children.Add(canvas);
		generalWrapPanel.Children.Add(stackPanel);
		stackPanel = new StackPanel();
		stackPanel.Width = 280.0;
		stackPanel.Margin = new Thickness(10.0, 10.0, 0.0, 0.0);
		text = Global.GetLang("高阶宝石满镶嵌效果");
		color = ((num3 <= 0) ? 4286611584U : 4294967040U);
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		stackPanel.Children.Add(gtextBlockEx);
		text = Global.GetLang("激活条件：全身装备镶满4级或4级以上宝石");
		color = ((num3 <= 0) ? 4286611584U : 4294944000U);
		gtextBlockEx = GTipService.GetTextBlock(text, HSTextField.defaultFontSize, color, 280.0, null);
		stackPanel.Children.Add(gtextBlockEx);
		canvas = new Canvas();
		canvas.Width = 235.0;
		canvas.Height = 122.0;
		canvas.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
		canvas.Background = new ImageBrush(Global.GetGameResImage("Images/Plate/tip_bak2.png"));
		string allJewelLevelOccupPropName = Global.GetAllJewelLevelOccupPropName(occupation);
		text = StringUtil.substitute(Global.GetLang("4级宝石    {0} +{1}%  {2} +{3}%"), new object[]
		{
			allJewelLevelOccupPropName,
			Global.GetAllJewelLevelOccupPercent(4),
			Global.GetLang("所有属性"),
			Global.GetAllJewelLevelOtherPercent(4)
		});
		color = ((num3 != 4) ? 4286611584U : ColorSL.FromArgb(255, 11, 229, 237));
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		text = StringUtil.substitute(Global.GetLang("5级宝石    {0} +{1}%  {2} +{3}%"), new object[]
		{
			allJewelLevelOccupPropName,
			Global.GetAllJewelLevelOccupPercent(5),
			Global.GetLang("所有属性"),
			Global.GetAllJewelLevelOtherPercent(5)
		});
		color = ((num3 != 5) ? 4286611584U : ColorSL.FromArgb(255, 11, 229, 237));
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 35);
		text = StringUtil.substitute(Global.GetLang("6级宝石    {0} +{1}%  {2} +{3}%"), new object[]
		{
			allJewelLevelOccupPropName,
			Global.GetAllJewelLevelOccupPercent(6),
			Global.GetLang("所有属性"),
			Global.GetAllJewelLevelOtherPercent(6)
		});
		color = ((num3 != 6) ? 4286611584U : ColorSL.FromArgb(255, 11, 229, 237));
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 70);
		text = StringUtil.substitute(Global.GetLang("7级宝石    {0} +{1}%  {2} +{3}%"), new object[]
		{
			allJewelLevelOccupPropName,
			Global.GetAllJewelLevelOccupPercent(7),
			Global.GetLang("所有属性"),
			Global.GetAllJewelLevelOtherPercent(7)
		});
		color = ((num3 != 7) ? 4286611584U : ColorSL.FromArgb(255, 11, 229, 237));
		gtextBlockEx = GTipService.GetTextBlock2(text, HSTextField.defaultFontSize, color, double.NaN, null);
		canvas.Children.Add(gtextBlockEx);
		Canvas.SetTop(gtextBlockEx, 105);
		stackPanel.Children.Add(canvas);
		generalWrapPanel.Children.Add(stackPanel);
	}

	private static StackPanel GetBonusTipItem(string name, string condition, string bonus)
	{
		StackPanel stackPanel = new StackPanel();
		stackPanel.Width = 280.0;
		stackPanel.Margin = new Thickness(10.0, 10.0, 0.0, 0.0);
		GTextBlockEx textBlock = GTipService.GetTextBlock2(name, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		stackPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(condition, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		stackPanel.Children.Add(textBlock);
		textBlock = GTipService.GetTextBlock2(bonus, HSTextField.defaultFontSize, 4294944000U, double.NaN, null);
		stackPanel.Children.Add(textBlock);
		return stackPanel;
	}

	private static NoBorderWindow ShowTipWindow(int cx, int cy, SpriteSL fe, int contentLeft, int contentTop, int cwidth, int cheight)
	{
		NoBorderWindow noBorderWindow = U3DUtils.NEW<NoBorderWindow>();
		noBorderWindow.Left = (double)cx;
		noBorderWindow.Top = (double)cy;
		noBorderWindow.BodyLeft = 0.0;
		noBorderWindow.BodyTop = 0.0;
		noBorderWindow.BodyWidth = (double)cwidth;
		noBorderWindow.BodyHeight = (double)cheight;
		noBorderWindow.BodyBackBrush = new SolidColorBrush(990488U);
		noBorderWindow.BodyBackOpacity = 0.9;
		Super.InitNoBorderWindow(noBorderWindow);
		noBorderWindow.ZIndex = 9999.0;
		Super.GData.PlayZoneRoot.Children.Add(noBorderWindow);
		noBorderWindow.SetContent(noBorderWindow.BodyPresenter, fe, (double)contentLeft, (double)contentTop);
		return noBorderWindow;
	}

	private static void ShowTipWindow2(int cx, int cy, SpriteSL fe, int contentLeft, int contentTop, int cwidth, int cheight)
	{
		if (null != GTipService.TipWindow2)
		{
			Super.CloseNoBorderWindow(Super.GData.PlayZoneRoot, GTipService.TipWindow2);
			GTipService.TipWindow2 = null;
		}
		GTipService.TipWindow2 = U3DUtils.NEW<NoBorderWindow>();
		GTipService.TipWindow2.Left = (double)cx;
		GTipService.TipWindow2.Top = (double)cy;
		GTipService.TipWindow2.BodyLeft = 0.0;
		GTipService.TipWindow2.BodyTop = 0.0;
		GTipService.TipWindow2.BodyWidth = (double)cwidth;
		GTipService.TipWindow2.BodyHeight = (double)cheight;
		GTipService.TipWindow2.BodyBackBrush = new SolidColorBrush(ColorSL.FromArgb(255, 28, 40, 48));
		GTipService.TipWindow2.BodyBackOpacity = 0.9;
		Super.InitNoBorderWindow(GTipService.TipWindow2);
		GTipService.TipWindow2.ZIndex = 9999.0;
		Super.GData.PlayZoneRoot.Children.Add(GTipService.TipWindow2);
		GTipService.TipWindow2.SetContent(GTipService.TipWindow2.BodyPresenter, fe, (double)contentLeft, (double)contentTop);
	}

	private static void ShowTipWindow3(int cx, int cy, SpriteSL fe, int contentLeft, int contentTop, int cwidth, int cheight)
	{
		if (null != GTipService.TipWindow3)
		{
			Super.CloseNoBorderWindow(Super.GData.PlayZoneRoot, GTipService.TipWindow3);
			GTipService.TipWindow3 = null;
		}
		GTipService.TipWindow3 = U3DUtils.NEW<NoBorderWindow>();
		GTipService.TipWindow3.Left = (double)cx;
		GTipService.TipWindow3.Top = (double)cy;
		GTipService.TipWindow3.BodyLeft = 0.0;
		GTipService.TipWindow3.BodyTop = 0.0;
		GTipService.TipWindow3.BodyWidth = (double)cwidth;
		GTipService.TipWindow3.BodyHeight = (double)cheight;
		GTipService.TipWindow3.BodyBackBrush = new SolidColorBrush(ColorSL.FromArgb(255, 28, 40, 48));
		GTipService.TipWindow3.BodyBackOpacity = 0.9;
		Super.InitNoBorderWindow(GTipService.TipWindow3);
		GTipService.TipWindow3.ZIndex = 9999.0;
		Super.GData.PlayZoneRoot.Children.Add(GTipService.TipWindow3);
		GTipService.TipWindow2.SetContent(GTipService.TipWindow3.BodyPresenter, fe, (double)contentLeft, (double)contentTop);
	}

	public static void InitCachingTipWindows()
	{
		string[] fields = new string[]
		{
			"1",
			"1"
		};
		GTipService.ShowSkillTipWindow(null, fields, true);
		string text = StringUtil.substitute("{0},{1},{2},{3}", new object[]
		{
			100001,
			1,
			-1,
			-1
		});
		fields = text.Split(new char[]
		{
			','
		});
		GTipService.PreShowGoodsTipWindow1(null, fields, true);
		GTipService.PreShowGoodsTipWindow2(null, fields, true);
		GTipService.PreShowGoodsTipWindow3(null, fields, true);
	}

	private static SpriteSL HookUC = null;

	private static GTipSprite HookITip = null;

	private static MouseEvent MouseEnterE = null;

	private static double MouseEnterTicks = 0.0;

	private static bool ShowTipState = false;

	private static NoBorderWindow NormalTipWindow = null;

	private static GTextBlockEx NormalTextBlock = null;

	private static NoBorderWindow SkillTipWindow = null;

	private static StackPanel SkillWrapPanel = null;

	private static Dictionary<string, object> SkillCtrlsDict = new Dictionary<string, object>();

	private static NoBorderWindow GoodsTipWindow_1 = null;

	private static StackPanel GoodsWrapPanel_1 = null;

	private static Dictionary<string, object> GoodsCtrlsDict_1 = new Dictionary<string, object>();

	private static NoBorderWindow GoodsTipWindow_2 = null;

	private static StackPanel GoodsWrapPanel_2 = null;

	private static Dictionary<string, object> GoodsCtrlsDict_2 = new Dictionary<string, object>();

	private static NoBorderWindow GoodsTipWindow_3 = null;

	private static StackPanel GoodsWrapPanel_3 = null;

	private static Dictionary<string, object> GoodsCtrlsDict_3 = new Dictionary<string, object>();

	private static NoBorderWindow GeneralTipWindow = null;

	private static StackPanel GeneralWrapPanel = null;

	private static NoBorderWindow TipWindow2 = null;

	private static NoBorderWindow TipWindow3 = null;
}
