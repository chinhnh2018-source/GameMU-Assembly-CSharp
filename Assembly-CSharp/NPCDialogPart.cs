using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class NPCDialogPart : UserControl
{
	protected void ReSize()
	{
		float num = this._TalkText.Label.transform.localScale.y * this._TalkText.Label.relativeSize.y;
		float y = this._TalkText.Label.transform.localPosition.y;
		if (null != this._Operation_Panel)
		{
			Vector3 localPosition = this._Operation_Panel.transform.localPosition;
			localPosition.y = y - num - 28f;
			this._Operation_Panel.transform.localPosition = localPosition;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Root = this.Container;
		this.thisCtrl = this;
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
	}

	public int NpcExtensionID
	{
		get
		{
			return this._NpcExtensionID;
		}
		set
		{
			this._NpcExtensionID = value;
		}
	}

	public int NpcID
	{
		get
		{
			return this._NpcID;
		}
		set
		{
			this._NpcID = value;
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	private void InitControls()
	{
	}

	public override void Destroy()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		if (null != this._Face)
		{
			this._Face.DestroyImmediateTexture();
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.BackgroundAlpha = 0.8;
		this.Container.BackgroundColor = 1384487U;
		this.InitControls();
	}

	public void GetNewData()
	{
		this.RefreshDataStatus = false;
		int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(this.NpcExtensionID);
		this._Face.ShowImage(Global.GetNPCImageString(npcpicCodeByID));
	}

	private void ShowYuanBaoItem(StackPanel sp, int id, uint textColor, int fontSize)
	{
		fontSize = FontSizeMgr.NPCOperationFontSize;
		int taskYuanBaoCompeteNum = Global.GetTaskYuanBaoCompeteNum(id);
		if (taskYuanBaoCompeteNum <= 0)
		{
			return;
		}
		Image image = new Image();
		image.Visibility = true;
		image.Source = new ImageBrush(Global.GetGameResImage("Images/Plate/ico_yb.png"));
		GTextBlockOutLine gtextBlockOutLine = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);
		gtextBlockOutLine.Visibility = true;
		gtextBlockOutLine.TextLineHeight = 1.0;
		gtextBlockOutLine.TextSize = (double)fontSize;
		gtextBlockOutLine.TextFontColor = new SolidColorBrush(textColor);
		gtextBlockOutLine.Text = StringUtil.substitute(Global.GetLang("花费{0}钻石完成"), new object[]
		{
			taskYuanBaoCompeteNum
		});
		gtextBlockOutLine.mouseEnabled = true;
		gtextBlockOutLine.Tag = new BaseTag(id);
		gtextBlockOutLine.addEventListener("mouseDown", new MouseEventHandler(this.tb_OnYuanBaoMouseDown));
		gtextBlockOutLine.addEventListener("mouseOver", new MouseEventHandler(this.tb_OnMouseEnter));
		gtextBlockOutLine.addEventListener("mouseOut", new MouseEventHandler(this.tb_OnMouseLeave));
		if (null != image)
		{
			sp.Children.Add(image);
		}
		sp.Children.Add(gtextBlockOutLine);
	}

	private void AddContentItem(StackPanel parent, int idtype, int id, string imageName, string text, uint textColor, int fontSize = 12, bool underLine = true, bool showYuanBao = false)
	{
		GLinkButton glinkButton = U3DUtils.NEW<GLinkButton>();
		glinkButton.name = this.ItemCount.ToString("000");
		this._Operation_List.ItemsSource.Add(glinkButton);
		glinkButton.ColiderSizeModule = 2f;
		glinkButton.FontSize = 18;
		glinkButton.Text = text;
		BaseTag tag = new BaseTag(string.Empty, id, idtype);
		glinkButton.Tag = tag;
		if (underLine)
		{
			glinkButton.addEventListener("mouseDown", delegate(MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
					{
						IDType = tag.Type,
						ID = tag.ID
					});
				}
			});
		}
	}

	private void tb_OnMouseDown(MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			GTextBlockOutLine gtextBlockOutLine = e.target.SafeGetComponent<GTextBlockOutLine>();
			int idtype = -1;
			if (gtextBlockOutLine.Parent.SafeGetComponent<StackPanel>().Parent == this.MyTasks)
			{
				idtype = 0;
			}
			else if (gtextBlockOutLine.Parent.SafeGetComponent<StackPanel>().Parent == this.SystemTasks)
			{
				idtype = 1;
			}
			else if (gtextBlockOutLine.Parent.SafeGetComponent<StackPanel>().Parent == this.OtherLinks)
			{
				idtype = 2;
			}
			else if (gtextBlockOutLine.Parent.SafeGetComponent<StackPanel>().Parent == this.ButtonLinks)
			{
				idtype = 3;
			}
			else if (gtextBlockOutLine.Parent.SafeGetComponent<StackPanel>().Parent == this.SalesPanel)
			{
				idtype = 4;
			}
			this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
			{
				IDType = idtype,
				ID = (int)gtextBlockOutLine.Tag
			});
		}
	}

	private void tb_OnYuanBaoMouseDown(MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			GTextBlockOutLine gtextBlockOutLine = e.target.SafeGetComponent<GTextBlockOutLine>();
			int idtype = 1000;
			this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
			{
				IDType = idtype,
				ID = (int)gtextBlockOutLine.Tag
			});
		}
	}

	private void tb_OnMouseEnter(MouseEvent e)
	{
		if (this._underLine)
		{
			e.target.SafeGetComponent<GTextBlockOutLine>().TextUnderLine = true;
		}
		if (Global.Data.GameCursorImageID < 100)
		{
		}
	}

	private void tb_OnMouseLeave(MouseEvent e)
	{
		if (this._underLine)
		{
			e.target.SafeGetComponent<GTextBlockOutLine>().TextUnderLine = false;
		}
		if (Global.Data.GameCursorImageID < 100)
		{
		}
	}

	public bool FindDoingTaskID(int taskID)
	{
		if (Global.Data.roleData.TaskDataList == null)
		{
			return false;
		}
		for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
		{
			if (Global.Data.roleData.TaskDataList[i].DoingTaskID == taskID)
			{
				return true;
			}
		}
		return false;
	}

	private string GetTaskClassName(int n)
	{
		if (n == 0)
		{
			return Global.GetLang("主线");
		}
		if (n == 1)
		{
			return Global.GetLang("支线");
		}
		if (n == 2)
		{
			return Global.GetLang("循环");
		}
		if (n == 3)
		{
			return Global.GetLang("猎杀");
		}
		if (n == 4)
		{
			return Global.GetLang("武学");
		}
		if (n == 5)
		{
			return Global.GetLang("军功");
		}
		if (n == 6)
		{
			return Global.GetLang("魔族");
		}
		if (n == 7)
		{
			return Global.GetLang("战盟");
		}
		return Global.GetLang("未知");
	}

	public void RefreshData(NPCData npcData)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (npcData == null || npcData.NPCID != this.NpcID)
		{
			return;
		}
		if (this.RefreshDataStatus)
		{
			return;
		}
		this.RefreshDataStatus = true;
		bool flag = false;
		this._Operation_List.ItemsSource.Clear();
		this.ItemCount = 0;
		this._TalkText.htmlText = string.Empty;
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(this.NpcID - 2130706432);
		if (npcvobyID != null)
		{
			string sname = npcvobyID.SName;
			this._Title.text = sname;
			RandomAS randomAS = new RandomAS(0);
			string talk = npcvobyID.Talk;
			if (string.Empty != Global.StringTrim(talk))
			{
				string[] array = talk.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					this._TalkText.htmlText = ColorCode.EncodingText(sname + ":\r\n", "00ff00") + array[randomAS.Next(0, array.Length)];
				}
			}
		}
		bool flag2 = false;
		if (Global.Data.roleData.TaskDataList != null && Global.Data.roleData.TaskDataList.Count > 0)
		{
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				int doingTaskID = Global.Data.roleData.TaskDataList[i].DoingTaskID;
				TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(doingTaskID);
				if (taskXmlNodeByID != null)
				{
					bool underLine = true;
					if (this.NpcID - 2130706432 != taskXmlNodeByID.DestNPC)
					{
						if (Super.JugeTaskComplete(taskXmlNodeByID, Super.GetTaskDataByTaskID(doingTaskID).DoingTaskVal1, Super.GetTaskDataByTaskID(doingTaskID).DoingTaskVal2))
						{
							if (!flag2)
							{
								flag2 = (null != Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("NPC对话UI"), doingTaskID, 1, 1));
							}
						}
						else if (!flag2)
						{
							flag2 = (null != Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("NPC对话UI"), doingTaskID, 0, 1));
						}
					}
					else
					{
						int taskClass = taskXmlNodeByID.TaskClass;
						string imageName = string.Empty;
						uint textColor = ColorSL.FromArgb(255, 208, 212, 208);
						string text = taskXmlNodeByID.Title;
						bool showYuanBao = false;
						if (taskClass == 8)
						{
							DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(taskClass);
							int num;
							int num2;
							if (dailyTaskData == null || Global.CanDoPaoHuanTask(taskClass, out num, out num2))
							{
								text = Global.GetPaoHuanTaskTitle(taskXmlNodeByID, text);
							}
							else
							{
								underLine = false;
								text = string.Format(Global.GetLang("{0}环【{1}】已做完"), dailyTaskData.RecNum, text);
							}
						}
						else if (taskClass == 2)
						{
							text = StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
							{
								text,
								Super.GetTaskDataByTaskID(doingTaskID).DoneCount + 1,
								taskXmlNodeByID.MaxRedoing
							});
						}
						if (taskClass != 8)
						{
							if (Super.JugeTaskComplete(taskXmlNodeByID, Super.GetTaskDataByTaskID(doingTaskID).DoingTaskVal1, Super.GetTaskDataByTaskID(doingTaskID).DoingTaskVal2))
							{
								textColor = ColorSL.FromArgb(255, 224, 211, 48);
								text = StringUtil.substitute(Global.GetLang("{0} 【{1}】"), new object[]
								{
									text,
									Global.GetLang("完成")
								});
								imageName = "Images/Plate/duihao.png";
								if (!flag2)
								{
									flag2 = (null != Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("NPC对话UI"), doingTaskID, 1, 1));
								}
							}
							else
							{
								imageName = "Images/Plate/wenhao.png";
								int taketime = taskXmlNodeByID.Taketime;
								if (taketime > 0 && Global.GetCorrectLocalTime() - Global.Data.roleData.TaskDataList[i].AddDateTime >= (long)(taketime * 1000))
								{
									textColor = ColorSL.FromArgb(255, 224, 211, 48);
									text = StringUtil.substitute(Global.GetLang("{0} 【{1}】"), new object[]
									{
										text,
										Global.GetLang("失败")
									});
								}
								string pubStartTime = taskXmlNodeByID.PubStartTime;
								string pubEndTime = taskXmlNodeByID.PubEndTime;
								if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
								{
									double num3 = (double)Global.GetCorrectLocalTime();
									double num4 = (double)Global.SafeConvertToTicks(pubStartTime);
									double num5 = (double)Global.SafeConvertToTicks(pubEndTime);
									if (num3 < num4 || num3 > num5)
									{
										textColor = ColorSL.FromArgb(255, 224, 211, 48);
										text = StringUtil.substitute(Global.GetLang("{0} 【{1}】"), new object[]
										{
											text,
											Global.GetLang("失败")
										});
									}
								}
								int limitLevel = taskXmlNodeByID.LimitLevel;
								if (limitLevel > 0 && Global.Data.roleData.Level < limitLevel)
								{
									textColor = ColorSL.FromArgb(255, 224, 211, 48);
									text = StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
									{
										text,
										Global.Data.roleData.Level,
										limitLevel
									});
								}
								if (!flag2)
								{
									flag2 = (null != Super.AddSystemNaviBoxByPos(this.Container, Global.GetLang("NPC对话UI"), doingTaskID, 0, 1));
								}
								showYuanBao = true;
							}
						}
						if (taskClass != 8 || !flag)
						{
							flag = true;
							this.AddContentItem(this.MyTasks, 0, doingTaskID, imageName, text, textColor, 12, underLine, showYuanBao);
						}
					}
				}
			}
		}
		if (Global.Data.npcData.NewTaskIDs != null && Global.Data.npcData.NewTaskIDs.Count > 0)
		{
			for (int j = 0; j < Global.Data.npcData.NewTaskIDs.Count; j++)
			{
				bool underLine = true;
				int num6 = Global.Data.npcData.NewTaskIDs[j];
				if (!this.FindDoingTaskID(num6))
				{
					TaskVO taskXmlNodeByID2 = ConfigTasks.GetTaskXmlNodeByID(num6);
					if (taskXmlNodeByID2 != null)
					{
						string text2 = taskXmlNodeByID2.Title;
						int taskClass2 = taskXmlNodeByID2.TaskClass;
						if (taskClass2 != 8)
						{
							if (taskClass2 == 8)
							{
								DailyTaskData dailyTaskData2 = Global.FindDailyTaskDataByTaskClass(taskClass2);
								int num7;
								int num8;
								if (dailyTaskData2 == null || Global.CanDoPaoHuanTask(taskClass2, out num7, out num8))
								{
									text2 = taskXmlNodeByID2.Title;
								}
								else
								{
									underLine = false;
									text2 = string.Format(Global.GetLang("{0}环【{1}】已做完"), dailyTaskData2.RecNum, text2);
								}
							}
							else if (taskClass2 == 2)
							{
								text2 = StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
								{
									text2,
									Global.Data.npcData.NewTaskIDsDoneCount[j] + 1,
									taskXmlNodeByID2.MaxRedoing
								});
							}
							if (taskClass2 != 8)
							{
								text2 = StringUtil.substitute(Global.GetLang("{0} 【{1}】"), new object[]
								{
									text2,
									this.GetTaskClassName(taskClass2)
								});
								int maxLevel = taskXmlNodeByID2.MaxLevel;
								if (Global.Data.roleData.Level > maxLevel)
								{
									goto IL_7A2;
								}
							}
							if (taskClass2 != 8 || !flag)
							{
								flag = true;
								this.AddContentItem(this.SystemTasks, 1, num6, "Images/Plate/item.png", text2, ColorSL.FromArgb(255, 0, 255, 0), 12, underLine, false);
							}
						}
					}
				}
				IL_7A2:;
			}
		}
		if (Global.Data.npcData.OperationIDs != null)
		{
			for (int k = 0; k < Global.Data.npcData.OperationIDs.Count; k++)
			{
			}
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("NpcLiPinDuiHuan_Andrid", true);
		if (systemParamByName != string.Empty)
		{
			string[] array2 = systemParamByName.Split(new char[]
			{
				'|'
			});
			for (int l = 0; l < array2.Length; l++)
			{
				string[] array3 = array2[l].Split(new char[]
				{
					','
				});
				if (array3.Length > 0 && array3[0] != string.Empty && Convert.ToInt32(array3[0]) == Global.Data.npcData.ExtensionID)
				{
					for (int m = 1; m < array3.Length; m++)
					{
						Global.Data.npcData.OperationIDs.Remove(Convert.ToInt32(array3[m]));
					}
				}
			}
		}
		XElement gameResXml = Global.GetGameResXml("Config/SystemOperations.Xml");
		if (gameResXml != null)
		{
			if (Context.IsAPPVerify && npcData.ExtensionID == 115 && Context.IsHaiwai)
			{
				return;
			}
			if (Global.Data.npcData.OperationIDs != null && Global.Data.npcData.OperationIDs.Count > 0)
			{
				for (int n = 0; n < Global.Data.npcData.OperationIDs.Count; n++)
				{
					int num9 = Global.Data.npcData.OperationIDs[n];
					if (num9 > 0)
					{
						XElement xelement = Global.GetXElement(gameResXml, "Operation", "ID", num9.ToString());
						if (xelement != null)
						{
							int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
							int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
							int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
							int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
							if (UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt3, xelementAttributeInt2, xelementAttributeInt4) == 0)
							{
								string imageName2 = "Images/Plate/item.png";
								this.AddContentItem(this.OtherLinks, 2, num9, imageName2, Global.GetXElementAttributeStr(xelement, "Title"), ColorSL.FromArgb(255, 219, 248, 122), 12, true, false);
							}
						}
					}
				}
			}
		}
		gameResXml = Global.GetGameResXml("Config/NPCScripts.Xml");
		if (gameResXml != null && Global.Data.npcData.ScriptIDs != null && Global.Data.npcData.ScriptIDs.Count > 0)
		{
			for (int num10 = 0; num10 < Global.Data.npcData.ScriptIDs.Count; num10++)
			{
				int num11 = Global.Data.npcData.ScriptIDs[num10];
				if (num11 > 0)
				{
					XElement xelement2 = Global.GetXElement(gameResXml, "Script", "ID", num11.ToString());
					if (xelement2 != null)
					{
						int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement2, "MinLevel");
						int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement2, "MinZhuanSheng");
						int xelementAttributeInt7 = Global.GetXElementAttributeInt(xelement2, "MaxLevel");
						int xelementAttributeInt8 = Global.GetXElementAttributeInt(xelement2, "MaxZhuanSheng");
						if (UIHelper.AvalidLevel(xelementAttributeInt5, xelementAttributeInt7, xelementAttributeInt6, xelementAttributeInt8) == 0)
						{
							this.AddContentItem(this.ButtonLinks, 3, num11, "Images/Plate/item.png", Global.GetXElementAttributeStr(xelement2, "Title"), ColorSL.FromArgb(255, 219, 248, 122), 12, true, false);
						}
					}
				}
			}
		}
		if (npcvobyID != null)
		{
			RandomAS randomAS2 = new RandomAS(0);
			string[] array4 = npcvobyID.SaleID.Split(new char[]
			{
				','
			});
			for (int num12 = 0; num12 < array4.Length; num12++)
			{
				string imageName3 = "Images/Plate/item.png";
				int num13 = Convert.ToInt32(array4[num12]);
				if (num13 > 0)
				{
					XElement xelement3 = Global.GetXElement(Global.GetGameResXml("Config/NPCSaleList.Xml"), "Sale", "ID", num13.ToString());
					if (xelement3 != null)
					{
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement3, "Description");
						this.AddContentItem(this.SalesPanel, 4, num13, imageName3, xelementAttributeStr, ColorSL.FromArgb(255, 219, 248, 122), 12, true, false);
					}
				}
			}
		}
		this._Operation_List.ItemsSource.DelayUpdate();
		this.ReSize();
		SystemHelpMgr.OnAction(UIObjIDs.NpcDialogPart, HelpStateEvents.Actived, this._NpcExtensionID);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			GameObject at = this._Operation_List.ItemsSource.GetAt(id);
			if (null != at)
			{
				SystemHelpPart.SetMask(at.transform, new Vector4(-6f, -6f, 6f, -6f));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public void InitPartData()
	{
	}

	public void CleanUpChildWindows()
	{
	}

	public GButton _Close;

	public UISprite _Bak;

	public ShowNetImage _Face;

	public GameObject _Operation_Panel;

	public ListBox _Operation_List;

	public TextBlock _Title;

	public GTextBlockOutLine _TalkText;

	private LoadingWindow LoadingWin;

	private bool RefreshDataStatus;

	private Canvas Root;

	private StackPanel MyTasks;

	private StackPanel SystemTasks;

	private StackPanel OtherLinks;

	private StackPanel ButtonLinks;

	private StackPanel SalesPanel;

	private SpriteSL thisCtrl;

	private bool _underLine;

	private int ItemCount;

	private int _NpcExtensionID;

	private int _NpcID;

	public DPSelectedItemEventHandler DPSelectedItem;
}
