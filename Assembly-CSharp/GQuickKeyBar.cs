using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class GQuickKeyBar : UserControl
{
	public GQuickKeyBar()
	{
		this.Container.Width = 440.0;
		this.Container.Height = 40.0;
		this.InitControls();
		this.LoadBindQuickKey();
		this.ShortKeyImage = new Image();
		this.ShortKeyImage.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/main_shortcuts2.png"));
		this.ShortKeyImage.Width = 391.0;
		this.ShortKeyImage.Height = 10.0;
		this.Container.Children.Add(this.ShortKeyImage);
		Canvas.SetLeft(this.ShortKeyImage, 2);
		Canvas.SetTop(this.ShortKeyImage, 1);
		Canvas.SetZIndex(this.ShortKeyImage, 1000.0);
		this.ShortKeyImage.addEventListener("mouseDown", new MouseEventHandler(this.MoueDown));
		this.ShortKeyImage.addEventListener("mouseMove", new MouseEventHandler(this.MoueMove));
		this.ShortKeyImage2 = new Image();
		this.ShortKeyImage2.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/main_shortcuts1.png"));
		this.ShortKeyImage2.Width = 182.0;
		this.ShortKeyImage2.Height = 10.0;
		this.ShortKeyImage2.mouseEnabled = false;
		this.Container.Children.Add(this.ShortKeyImage2);
		Canvas.SetLeft(this.ShortKeyImage2, 212);
		Canvas.SetTop(this.ShortKeyImage2, -40);
		Canvas.SetZIndex(this.ShortKeyImage2, 100.0);
		this.ShortKeyImage2.addEventListener("mouseDown", new MouseEventHandler(this.MoueDown));
		this.ShortKeyImage2.addEventListener("mouseMove", new MouseEventHandler(this.MoueMove));
	}

	public List<ObservableCollection> equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public int QuickKeyType
	{
		get
		{
			return this._QuickKeyType;
		}
		set
		{
			this._QuickKeyType = value;
		}
	}

	public QuickKeyItem[] QuickKeyItems
	{
		get
		{
			return this._QuickKeyItems;
		}
		set
		{
			this._QuickKeyItems = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.dragDropTarget1.Name = "dragDropTarget1";
		this.Container.Children.Add(this.dragDropTarget1);
		this.dragDropTarget1.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget1, 1.0);
		this.dragDropTarget1.Children.Add(this.ListBox1);
		this.ListBox1.Width = 34.0;
		this.ListBox1.Height = 34.0;
		this.ListBox1.BorderThickness = 0;
		this.ListBox1.Name = "ListBox1";
		this.dragDropTarget2.Name = "dragDropTarget2";
		this.Container.Children.Add(this.dragDropTarget2);
		this.dragDropTarget2.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget2, 1.0);
		this.dragDropTarget2.Children.Add(this.ListBox2);
		this.ListBox2.Width = 34.0;
		this.ListBox2.Height = 34.0;
		this.ListBox2.BorderThickness = 0;
		this.ListBox2.Name = "ListBox2";
		this.dragDropTarget3.Name = "dragDropTarget3";
		this.Container.Children.Add(this.dragDropTarget3);
		this.dragDropTarget3.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget3, 1.0);
		this.dragDropTarget3.Children.Add(this.ListBox3);
		this.ListBox3.Width = 34.0;
		this.ListBox3.Height = 34.0;
		this.ListBox3.BorderThickness = 0;
		this.ListBox3.Name = "ListBox3";
		this.dragDropTarget4.Name = "dragDropTarget4";
		this.Container.Children.Add(this.dragDropTarget4);
		this.dragDropTarget4.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget4, 1.0);
		this.dragDropTarget4.Children.Add(this.ListBox4);
		this.ListBox4.Width = 34.0;
		this.ListBox4.Height = 34.0;
		this.ListBox4.BorderThickness = 0;
		this.ListBox4.Name = "ListBox4";
		this.dragDropTarget5.Name = "dragDropTarget5";
		this.Container.Children.Add(this.dragDropTarget5);
		this.dragDropTarget5.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget5, 1.0);
		this.dragDropTarget5.Children.Add(this.ListBox5);
		this.ListBox5.Width = 34.0;
		this.ListBox5.Height = 34.0;
		this.ListBox5.BorderThickness = 0;
		this.ListBox5.Name = "ListBox5";
		this.dragDropTarget6.Name = "dragDropTarget6";
		this.Container.Children.Add(this.dragDropTarget6);
		this.dragDropTarget6.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget6, 1.0);
		this.dragDropTarget6.Children.Add(this.ListBox6);
		this.ListBox6.Width = 34.0;
		this.ListBox6.Height = 34.0;
		this.ListBox6.BorderThickness = 0;
		this.ListBox6.Name = "ListBox6";
		this.dragDropTarget7.Name = "dragDropTarget7";
		this.Container.Children.Add(this.dragDropTarget7);
		this.dragDropTarget7.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget7, 1.0);
		this.dragDropTarget7.Children.Add(this.ListBox7);
		this.ListBox7.Width = 34.0;
		this.ListBox7.Height = 34.0;
		this.ListBox7.BorderThickness = 0;
		this.ListBox7.Name = "ListBox7";
		this.dragDropTarget8.Name = "dragDropTarget8";
		this.Container.Children.Add(this.dragDropTarget8);
		this.dragDropTarget8.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget8, 1.0);
		this.dragDropTarget8.Children.Add(this.ListBox8);
		this.ListBox8.Width = 34.0;
		this.ListBox8.Height = 34.0;
		this.ListBox8.BorderThickness = 0;
		this.ListBox8.Name = "ListBox8";
		this.dragDropTarget9.Name = "dragDropTarget9";
		this.Container.Children.Add(this.dragDropTarget9);
		this.dragDropTarget9.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget9, 1.0);
		this.dragDropTarget9.Children.Add(this.ListBox9);
		this.ListBox9.Width = 34.0;
		this.ListBox9.Height = 34.0;
		this.ListBox9.BorderThickness = 0;
		this.ListBox9.Name = "ListBox9";
		this.dragDropTarget10.Name = "dragDropTarget10";
		this.Container.Children.Add(this.dragDropTarget10);
		this.dragDropTarget10.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget10, 1.0);
		this.dragDropTarget10.Children.Add(this.ListBox10);
		this.ListBox10.Width = 34.0;
		this.ListBox10.Height = 34.0;
		this.ListBox10.BorderThickness = 0;
		this.ListBox10.Name = "ListBox10";
		this.dragDropTarget11.Name = "dragDropTarget11";
		this.Container.Children.Add(this.dragDropTarget11);
		this.dragDropTarget9.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget11, 1.0);
		this.dragDropTarget11.Children.Add(this.ListBox11);
		this.ListBox11.Width = 34.0;
		this.ListBox11.Height = 34.0;
		this.ListBox11.Name = "ListBox11";
		this.dragDropTarget12.Name = "dragDropTarget12";
		this.Container.Children.Add(this.dragDropTarget12);
		this.dragDropTarget12.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget12, 1.0);
		this.dragDropTarget12.Children.Add(this.ListBox12);
		this.ListBox12.Width = 34.0;
		this.ListBox12.Height = 34.0;
		this.ListBox12.Name = "ListBox12";
		this.dragDropTarget13.Name = "dragDropTarget13";
		this.Container.Children.Add(this.dragDropTarget13);
		this.dragDropTarget13.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget13, 1.0);
		this.dragDropTarget13.Children.Add(this.ListBox13);
		this.ListBox13.Width = 34.0;
		this.ListBox13.Height = 34.0;
		this.ListBox13.Name = "ListBox13";
		this.dragDropTarget14.Name = "dragDropTarget14";
		this.Container.Children.Add(this.dragDropTarget14);
		this.dragDropTarget14.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget14, 1.0);
		this.dragDropTarget14.Children.Add(this.ListBox14);
		this.ListBox14.Width = 34.0;
		this.ListBox14.Height = 34.0;
		this.ListBox14.Name = "ListBox14";
		this.dragDropTarget15.Name = "dragDropTarget15";
		this.Container.Children.Add(this.dragDropTarget15);
		this.dragDropTarget15.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget15, 1.0);
		this.dragDropTarget15.Children.Add(this.ListBox15);
		this.ListBox15.Width = 34.0;
		this.ListBox15.Height = 34.0;
		this.ListBox15.Name = "ListBox15";
		this.dragDropTarget16.Name = "dragDropTarget16";
		this.Container.Children.Add(this.dragDropTarget16);
		this.dragDropTarget16.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget16, 1.0);
		this.dragDropTarget16.Children.Add(this.ListBox16);
		this.ListBox16.Width = 34.0;
		this.ListBox16.Height = 34.0;
		this.ListBox16.Name = "ListBox16";
		this.dragDropTarget17.Name = "dragDropTarget17";
		this.Container.Children.Add(this.dragDropTarget17);
		this.dragDropTarget17.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget17, 1.0);
		this.dragDropTarget17.Children.Add(this.ListBox17);
		this.ListBox17.Width = 34.0;
		this.ListBox17.Height = 34.0;
		this.ListBox17.Name = "ListBox17";
		this.dragDropTarget18.Name = "dragDropTarget18";
		this.Container.Children.Add(this.dragDropTarget18);
		this.dragDropTarget18.AllowDrop = true;
		Canvas.SetZIndex(this.dragDropTarget18, 1.0);
		this.dragDropTarget18.Children.Add(this.ListBox18);
		this.ListBox18.Width = 34.0;
		this.ListBox18.Height = 34.0;
		this.ListBox18.Name = "ListBox18";
	}

	private int GetListBoxType()
	{
		if (this.QuickKeyType == 0)
		{
			return 3;
		}
		return 4;
	}

	public void InitControls()
	{
		for (int i = 1; i < 19; i++)
		{
			TextBlock textBlock = this.Container.FindName(StringUtil.substitute("Hint{0}", new object[]
			{
				i
			})).SafeGetComponent<TextBlock>();
			if (null != textBlock)
			{
				textBlock.FontSize = FontSizeMgr.QuickKeyBarNumFontSize;
				if (i < 13)
				{
					Canvas.SetLeft(textBlock, (i - 1) * 35 + 2);
					Canvas.SetTop(textBlock, 0);
				}
				else
				{
					Canvas.SetLeft(textBlock, (i - 7) * 35 + 2);
					Canvas.SetTop(textBlock, -40);
				}
				Canvas.SetZIndex(textBlock, 1000.0);
			}
			FixedListBoxDragDropTarget fixedListBoxDragDropTarget = this.Container.FindName(StringUtil.substitute("dragDropTarget{0}", new object[]
			{
				i
			})).SafeGetComponent<FixedListBoxDragDropTarget>();
			if (null != fixedListBoxDragDropTarget)
			{
				if (i < 13)
				{
					Canvas.SetLeft(fixedListBoxDragDropTarget, (i - 1) * 35);
					Canvas.SetTop(fixedListBoxDragDropTarget, 0);
				}
				else
				{
					Canvas.SetLeft(fixedListBoxDragDropTarget, (i - 7) * 35);
					Canvas.SetTop(fixedListBoxDragDropTarget, -41);
				}
			}
			CDCoolDown cdcoolDown = U3DUtils.NEW<CDCoolDown>();
			cdcoolDown.Name = StringUtil.substitute("CoolDown{0}", new object[]
			{
				i
			});
			cdcoolDown.Width = 32.0;
			cdcoolDown.Height = 32.0;
			cdcoolDown.BodyColor = new SolidColorBrush(ColorSL.FromArgb(200, 0, 0, 0));
			cdcoolDown.Opacity = 1.0;
			cdcoolDown.TextFontSize = FontSizeMgr.CDCollDownFontSize;
			cdcoolDown.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
			cdcoolDown.IsHitTestVisible = false;
			cdcoolDown.CoodDownComplete = new EventHandler(this.CoodDownComplete);
			if (i < 13)
			{
				Canvas.SetLeft(cdcoolDown, (i - 1) * 35);
				Canvas.SetTop(cdcoolDown, 0);
			}
			else
			{
				Canvas.SetLeft(cdcoolDown, (i - 7) * 35);
				Canvas.SetTop(cdcoolDown, -40);
			}
			Canvas.SetZIndex(cdcoolDown, 999.0);
			this.Container.Children.Add(cdcoolDown);
			this.CDCooldownList.Add(cdcoolDown);
		}
	}

	public void GoodsMovingEnd(GoodsMovingEvent evt)
	{
		object tag = evt.Tag;
		if (GoodsMovingMgr.GoodsMovingMgrHandled)
		{
			return;
		}
	}

	private int GetListBoxIndexByPos(Point globalPoint)
	{
		for (int i = 1; i < 19; i++)
		{
			FixedListBoxDragDropTarget fixedListBoxDragDropTarget = U3DUtils.AS<FixedListBoxDragDropTarget>(this.Container.FindName(StringUtil.substitute("dragDropTarget{0}", new object[]
			{
				i
			})));
			ListBox listBox = U3DUtils.AS<ListBox>(fixedListBoxDragDropTarget.FindName(StringUtil.substitute("ListBox{0}", new object[]
			{
				i
			})));
			if (!(null == listBox))
			{
				Point point = listBox.globalToLocal(globalPoint);
				if (point.X >= 0 && point.Y >= 0 && (double)point.X < listBox.Width && (double)point.Y < listBox.Height)
				{
					return i;
				}
			}
		}
		return -1;
	}

	private void GoodsMovingEndByBox(ListBox listBox, GoodsData gd)
	{
		bool flag = false;
		QuickKeyItem[] array = this.QuickKeyItems.Clone() as QuickKeyItem[];
		int num = (int)listBox.Tag;
		if (num >= 0 && num < array.Length)
		{
			int goodsUsingModeByGoodsID = Global.GetGoodsUsingModeByGoodsID(gd.GoodsID);
			if (goodsUsingModeByGoodsID == 1)
			{
				array[num] = new QuickKeyItem();
				array[num].ItemType = 1;
				array[num].ID = gd.GoodsID;
				flag = true;
			}
			if (flag)
			{
				string quickKeys = Super.GetQuickKeys(array);
				GameInstance.Game.SpriteModKeys(this.QuickKeyType, quickKeys);
			}
		}
	}

	private void CoodDownComplete(object sender, object e)
	{
		(sender as CDCoolDown).Visibility = false;
	}

	private void dragDropTarget_ItemDragStarting(object sender, object e)
	{
	}

	private void dragDropTarget_ItemDragCompleted(object sender, object e)
	{
	}

	private void dragDropTarget_ItemDroppedOnTarget(object sender, object e)
	{
	}

	private void dragDropTarget_Drop(object sender, object e)
	{
		if (null != Super.GData.DragingItem && null != Super.GData.DragingListBox)
		{
			bool flag = false;
			QuickKeyItem[] array = this.QuickKeyItems.Clone() as QuickKeyItem[];
			Point globalPoint = new Point(0, 0);
			int listBoxIndexByPos = this.GetListBoxIndexByPos(globalPoint);
			if (listBoxIndexByPos >= 1 && listBoxIndexByPos < 19)
			{
				if (Super.GData.DragingItem.BoxTypes == this.GetListBoxType())
				{
					int num = (int)Super.GData.DragingListBox.Tag;
					if (num >= 0 && num < array.Length)
					{
						QuickKeyItem quickKeyItem = array[listBoxIndexByPos];
						array[listBoxIndexByPos] = array[num];
						array[num] = quickKeyItem;
					}
					flag = true;
				}
				else if (Super.GData.DragingItem.BoxTypes == 2)
				{
					SkillData skillDataByID = Global.GetSkillDataByID(Super.GData.DragingItem.ItemCode);
					if (skillDataByID != null)
					{
						MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillDataByID.SkillID);
						if (1 <= skillXmlNode.MagicType && skillXmlNode.MagicType <= 2)
						{
							array[listBoxIndexByPos] = new QuickKeyItem();
							array[listBoxIndexByPos].ItemType = 0;
							array[listBoxIndexByPos].ID = Super.GData.DragingItem.ItemCode;
							flag = true;
						}
					}
				}
				else if (Super.GData.DragingItem.BoxTypes == 1)
				{
					int goodsUsingModeByGoodsID = Global.GetGoodsUsingModeByGoodsID(Super.GData.DragingItem.ItemCode);
					if (goodsUsingModeByGoodsID == 1)
					{
						array[listBoxIndexByPos] = new QuickKeyItem();
						array[listBoxIndexByPos].ItemType = 1;
						array[listBoxIndexByPos].ID = Super.GData.DragingItem.ItemCode;
						flag = true;
					}
				}
				if (flag)
				{
					string quickKeys = Super.GetQuickKeys(array);
					GameInstance.Game.SpriteModKeys(this.QuickKeyType, quickKeys);
				}
			}
		}
	}

	public void RefreshIcon()
	{
		int num = -1;
		for (int i = 0; i < this.equipIcon.Count; i++)
		{
			if (i > 0)
			{
				if (this.equipIcon[i].Length > 0)
				{
					GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[i][0]);
					if (gicon.EnableHint)
					{
						gicon.EnableHint = false;
						num = gicon.ItemCode;
					}
					gicon.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonDown);
					gicon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
				}
			}
		}
		bool flag = false;
		for (int j = 0; j < this.equipIcon.Count; j++)
		{
			if (j > 0)
			{
				this.equipIcon[j].Clear();
				GIcon quickKeyIcon = Super.GetQuickKeyIcon(this.QuickKeyItems[j], this.GetListBoxType());
				if (null != quickKeyIcon)
				{
					if (num != -1 && quickKeyIcon.ItemCode == num)
					{
						flag = true;
					}
					this.equipIcon[j].Add(quickKeyIcon);
					quickKeyIcon.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonDown);
					quickKeyIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
				}
			}
		}
		this.RefreshCoolDown(-1, true);
		if (!flag)
		{
			num = -1;
		}
		this.SetDefaultSkillIcon(num);
	}

	public void RemoveIcon(ListBox listBox, GIcon icon)
	{
		if (null != listBox && null != icon)
		{
			QuickKeyItem[] array = this.QuickKeyItems.Clone() as QuickKeyItem[];
			int num = (int)listBox.Tag;
			if (num >= 0 && num < array.Length)
			{
				if (this.QuickKeyItems[num].ItemType == 0 && Global.Data.GameScene.GetDefaultSkillID() == this.QuickKeyItems[num].ID)
				{
					Global.Data.GameScene.SetDefaultSkillID(-1);
				}
				array[num] = null;
				string quickKeys = Super.GetQuickKeys(array);
				GameInstance.Game.SpriteModKeys(this.QuickKeyType, quickKeys);
			}
		}
	}

	public void RefreshGoodsCount(int goodsID)
	{
		for (int i = 0; i < this.QuickKeyItems.Length; i++)
		{
			if (i > 0)
			{
				if (this.QuickKeyItems[i] != null)
				{
					if (this.QuickKeyItems[i].ItemType == 1 && goodsID == this.QuickKeyItems[i].ID && this.equipIcon[i].Length > 0)
					{
						GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[i][0]);
						if (null != gicon)
						{
							gicon.Text = Global.GetTotalGoodsCountByID(this.QuickKeyItems[i].ID).ToString();
						}
					}
				}
			}
		}
	}

	public void SetDefaultSkillIcon(int skillID)
	{
		for (int i = 0; i < this.QuickKeyItems.Length; i++)
		{
			if (i > 0)
			{
				if (this.QuickKeyItems[i] != null)
				{
					if (this.QuickKeyItems[i].ItemType == 0)
					{
						if (skillID != -1)
						{
							if (skillID != this.QuickKeyItems[i].ID)
							{
								if (this.equipIcon[i].Length > 0)
								{
									GIcon gicon = U3DUtils.AS<GIcon>(this.equipIcon[i][0]);
									if (null != gicon)
									{
										Super.SetSkillIconLiuGuang(gicon, false);
									}
								}
							}
							else if (this.equipIcon[i].Length > 0)
							{
								GIcon gicon2 = U3DUtils.AS<GIcon>(this.equipIcon[i][0]);
								if (null != gicon2 && ConfigMagicInfos.CanSkillByBangDing(gicon2.ItemCode, false))
								{
									Super.SetSkillIconLiuGuang(gicon2, true);
									Global.Data.roleData.DefaultSkillID = gicon2.ItemCode;
								}
							}
						}
						else if (this.equipIcon[i].Length > 0)
						{
							GIcon gicon3 = U3DUtils.AS<GIcon>(this.equipIcon[i][0]);
							if (null != gicon3 && ConfigMagicInfos.CanSkillByBangDing(gicon3.ItemCode, false))
							{
								Super.SetSkillIconLiuGuang(gicon3, true);
								Global.Data.roleData.DefaultSkillID = gicon3.ItemCode;
								break;
							}
						}
					}
				}
			}
		}
	}

	public void RefreshCoolDown(int skillID = -1, bool isDrawTicks = true)
	{
		for (int i = 0; i < this.QuickKeyItems.Length; i++)
		{
			if (i > 0)
			{
				int num = i - 1;
				if (this.QuickKeyItems[i] == null)
				{
					this.CDCooldownList[num].Reset();
					this.CDCooldownList[num].Visibility = false;
				}
				else if (skillID < 0 || this.QuickKeyItems[i].ID == skillID)
				{
					if (this.QuickKeyItems[i].ItemType == 0)
					{
						double num2 = (double)Global.GetSkillCoolDownTicks(this.QuickKeyItems[i].ID);
						if (num2 <= 0.0)
						{
							this.CDCooldownList[num].Visibility = false;
							this.CDCooldownList[num].Reset();
						}
						else
						{
							this.CDCooldownList[num].Visibility = true;
							this.CDCooldownList[num].MyStart((long)((int)num2), true, 100, Global.GetCorrectLocalTime(), false, isDrawTicks);
						}
					}
					else if (this.QuickKeyItems[i].ItemType == 1)
					{
						double num3 = (double)Global.GetGoodsCoolDownTicks(this.QuickKeyItems[i].ID);
						if (num3 <= 0.0)
						{
							this.CDCooldownList[num].Visibility = false;
							this.CDCooldownList[num].Reset();
						}
						else
						{
							this.CDCooldownList[num].Visibility = true;
							this.CDCooldownList[num].MyStart((long)((int)num3), true, 100, 0L, true, true);
						}
					}
				}
			}
		}
	}

	private int FindBlankItem()
	{
		for (int i = 0; i < this.QuickKeyItems.Length; i++)
		{
			if (i > 0)
			{
				if (!this.AnimationIndexsDict.ContainsKey(i))
				{
					if (this.QuickKeyItems[i] == null)
					{
						return i;
					}
					if (this.QuickKeyItems[i].ItemType < 0)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public void AddLearnedSkillReal(int skillID, int index, int magicType)
	{
		QuickKeyItem[] array = this.QuickKeyItems.Clone() as QuickKeyItem[];
		array[index] = new QuickKeyItem();
		array[index].ItemType = 0;
		array[index].ID = skillID;
		string quickKeys = Super.GetQuickKeys(array);
		Global.Data.roleData.MainQuickBarKeys = quickKeys;
		Super.ParseMainQuickKeys(Global.Data.roleData.MainQuickBarKeys, false);
		GameInstance.Game.SpriteModKeys(this.QuickKeyType, quickKeys);
		if ((magicType == 1 || magicType == 2) && ConfigMagicInfos.CanSkillByBangDing(skillID, false))
		{
			Global.Data.GameScene.SetDefaultSkillID(skillID);
		}
	}

	public void AddLearnedSkill(int skillID)
	{
		object skillDataByID = Global.GetSkillDataByID(skillID);
		if (skillDataByID != null)
		{
			int num = this.FindBlankItem();
			if (num >= 0)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
				int magicType = skillXmlNode.MagicType;
				if (magicType > 0 && magicType < 3)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = skillID,
						Index = num,
						MagicType = magicType
					});
				}
			}
		}
	}

	private void IconMouseLeftButtonDown(object sender, MouseEvent e)
	{
		this.MouseLeftButtonState++;
	}

	private void IconMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.MouseLeftButtonState < 1)
		{
			return;
		}
		this.MouseLeftButtonState = 0;
		GIcon gicon = sender as GIcon;
		if (gicon.TipType == 1)
		{
			GoodsData goodsDataByID = Global.GetGoodsDataByID(gicon.ItemCode);
			if (goodsDataByID != null)
			{
				bool flag = Global.GetGoodsUsingModeByGoodsID(goodsDataByID.GoodsID) > 0;
				if (flag)
				{
					if (!Global.GoodsCoolDown(goodsDataByID.GoodsID))
					{
						if (Global.GetCategoriyByGoodsID(goodsDataByID.GoodsID) == 704)
						{
							GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
						}
						else
						{
							GameInstance.Game.SpriteUseGoods(goodsDataByID.Id, goodsDataByID.GoodsID, 1);
						}
					}
					else
					{
						string goodsNameByID = Global.GetGoodsNameByID(goodsDataByID.GoodsID, false);
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
						{
							goodsNameByID
						}), 0, -1, -1, 0);
					}
				}
			}
		}
		else
		{
			Super.UseSkillByID(gicon.ItemCode);
		}
	}

	public void InitPartData()
	{
		if (this.QuickKeyType == 0)
		{
			Super.ParseMainQuickKeys(Global.Data.roleData.MainQuickBarKeys, false);
		}
		else
		{
			Super.ParseOtherQuickKeys(Global.Data.roleData.OtherQuickBarKeys);
		}
		this.equipIcon = new List<ObservableCollection>();
		for (int i = 1; i < 19; i++)
		{
			FixedListBoxDragDropTarget fixedListBoxDragDropTarget = null;
			ListBox listBox = null;
			if (null != listBox && null != fixedListBoxDragDropTarget)
			{
				listBox.BackgroundColor = 4278190080U;
				listBox.BackgroundAlpha = 0.01;
				this.equipIcon[i] = listBox.ItemsSource;
				GIcon quickKeyIcon = Super.GetQuickKeyIcon(this.QuickKeyItems[i], this.GetListBoxType());
				if (null != quickKeyIcon)
				{
					this.equipIcon[i].Clear();
					this.equipIcon[i].Add(quickKeyIcon);
					quickKeyIcon.MouseLeftButtonDown = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonDown);
					quickKeyIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.IconMouseLeftButtonUp);
				}
			}
		}
		for (int i = 1; i < 19; i++)
		{
			FixedListBoxDragDropTarget fixedListBoxDragDropTarget2 = U3DUtils.AS<FixedListBoxDragDropTarget>(this.Container.FindName(StringUtil.substitute("dragDropTarget{0}", new object[]
			{
				i
			})));
			ListBox listBox2 = U3DUtils.AS<ListBox>(fixedListBoxDragDropTarget2.FindName(StringUtil.substitute("ListBox{0}", new object[]
			{
				i
			})));
			if (null != listBox2 && null != fixedListBoxDragDropTarget2)
			{
				listBox2.Tag = i;
				fixedListBoxDragDropTarget2.ItemDragStarting = new EventHandler(this.dragDropTarget_ItemDragStarting);
				fixedListBoxDragDropTarget2.ItemDragCompleted = new EventHandler(this.dragDropTarget_ItemDragCompleted);
				fixedListBoxDragDropTarget2.ItemDroppedOnTarget = new EventHandler(this.dragDropTarget_ItemDroppedOnTarget);
				fixedListBoxDragDropTarget2.Drop = new EventHandler(this.dragDropTarget_Drop);
				fixedListBoxDragDropTarget2.AllowDrop = true;
			}
		}
	}

	public void NewSkillAnimation(Canvas root, int skillID, int index, int magicType, int xPos, int yPos)
	{
		GFyingImage newStillImage = this.GetNewStillImage(skillID);
		if (null != newStillImage)
		{
			FixedListBoxDragDropTarget obj = null;
			int x = (int)Math.Floor(Global.GlobalMainWindow.ActualWidth / 2.0);
			int y = (int)Math.Floor(Global.GlobalMainWindow.ActualHeight / 2.0);
			Point start = new Point(x, y);
			double num = (double)xPos + Canvas.GetLeft(obj);
			double num2 = (double)yPos + Canvas.GetTop(obj);
			Point end = new Point((int)num, (int)num2);
			Canvas.SetLeft(newStillImage, start.X);
			Canvas.SetTop(newStillImage, start.Y);
			newStillImage.Z = 100000.0;
			root.Children.Add(newStillImage);
			this.AnimationMoveStillImage(skillID, index, magicType, newStillImage, start, end);
			this.AnimationIndexsDict.Add(index, 0);
		}
	}

	private void AnimationMoveStillImage(int id, int index, int magicType, GFyingImage flyingImage, Point start, Point end)
	{
		try
		{
			flyingImage.X = (double)start.X;
			flyingImage.Y = (double)start.Y;
		}
		catch (Exception ex)
		{
			if (flyingImage.Parent != null)
			{
				U3DUtils.AS<Canvas>(flyingImage.Parent).Children.Remove(flyingImage, true);
			}
			flyingImage.GoodsImageSource = null;
			GError.AddErrMsg2(StringUtil.substitute(Global.GetLang("执行飞行技能特效时发生了错误"), new object[0]));
			MUDebug.LogException(ex);
		}
	}

	private GFyingImage GetNewStillImage(int skillID)
	{
		GFyingImage gfyingImage = U3DUtils.NEW<GFyingImage>();
		MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
		if (skillXmlNode != null)
		{
			gfyingImage.GoodsImageSource = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
			{
				skillID
			});
		}
		return gfyingImage;
	}

	public void AddGoodsToQuickKeyBar(int goodID)
	{
		if (this.BindQuickKeyGoodsDict.ContainsKey(goodID.ToString()))
		{
			int indexByBindQuickKey = this.GetIndexByBindQuickKey(this.BindQuickKeyGoodsDict[goodID.ToString()]);
			if (indexByBindQuickKey >= 0 && indexByBindQuickKey < this.QuickKeyItems.Length && this.QuickKeyItems[indexByBindQuickKey] == null)
			{
				this.QuickKeyItems[indexByBindQuickKey] = new QuickKeyItem();
				this.QuickKeyItems[indexByBindQuickKey].ItemType = 1;
				this.QuickKeyItems[indexByBindQuickKey].ID = goodID;
				string quickKeys = Super.GetQuickKeys(this.QuickKeyItems);
				GameInstance.Game.SpriteModKeys(this.QuickKeyType, quickKeys);
			}
		}
	}

	private void LoadBindQuickKey()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("BindQuickKeyGoodsList", '|');
		if (systemParamStringArrayByName.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.BindQuickKeyGoodsDict[array[0]] = array[1];
			}
		}
	}

	private int GetIndexByBindQuickKey(string key)
	{
		int result = 0;
		key = key.ToUpper();
		if (key == "Q")
		{
			result = 13;
		}
		else if (key == "W")
		{
			result = 14;
		}
		else if (key == "E")
		{
			result = 15;
		}
		else if (key == "A")
		{
			result = 16;
		}
		else if (key == "S")
		{
			result = 17;
		}
		else if (key == "D")
		{
			result = 18;
		}
		return result;
	}

	private void MoueMove(MouseEvent e)
	{
		if (!GoodsDragingMgr.IsGoodsMoving())
		{
		}
	}

	private void MoueDown(MouseEvent e)
	{
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private Image ShortKeyImage;

	private Image ShortKeyImage2;

	private FixedListBoxDragDropTarget dragDropTarget1 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox1 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget2 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox2 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget3 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox3 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget4 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox4 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget5 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox5 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget6 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox6 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget7 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox7 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget8 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox8 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget9 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox9 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget10 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox10 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget11 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox11 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget12 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox12 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget13 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox13 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget14 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox14 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget15 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox15 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget16 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox16 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget17 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox17 = new ListBox();

	private FixedListBoxDragDropTarget dragDropTarget18 = U3DUtils.NEW<FixedListBoxDragDropTarget>();

	private ListBox ListBox18 = new ListBox();

	private List<CDCoolDown> CDCooldownList = new List<CDCoolDown>();

	private int MouseLeftButtonState;

	private List<ObservableCollection> _equipIcon;

	private int _QuickKeyType;

	private QuickKeyItem[] _QuickKeyItems;

	private Dictionary<int, int> AnimationIndexsDict = new Dictionary<int, int>();

	private Dictionary<string, string> BindQuickKeyGoodsDict = new Dictionary<string, string>();
}
