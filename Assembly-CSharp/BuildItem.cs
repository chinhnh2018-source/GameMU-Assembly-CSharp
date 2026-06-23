using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class BuildItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mConfigBuildXml = new ConfigBuildXml();
		NGUITools.SetActive(this.ProgressBar.gameObject, false);
		NGUITools.SetActive(this.BuildBtn.gameObject, false);
		this.mProgressBarlabel = this.ProgressBar.transform.GetChild(2).GetComponent<UILabel>();
		this.BuildSelfBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(s, new DPSelectedItemEventArgs
				{
					Type = 1,
					ID = this.mBuildData.BuildID
				});
				if (4 >= this.ID)
				{
					if (this.mConfigbuildTaskXml.IsOldTask(this.mBuildData.BuildID, this.Build_TaskId) && 0 < this.Build_TaskId)
					{
						Super.HintMainText(Global.GetLang("需完成并领取奖励后才能进入"), 10, 3);
					}
					else
					{
						this.ShowTask(this.ID);
					}
				}
			}
		};
		UIEventListener.Get(this.BuildSelfBtn.gameObject).onDrag = delegate(GameObject e, Vector2 s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					Type = 11,
					ID = this.mBuildData.BuildID,
					Title = s.x.ToString()
				});
			}
		};
		this.BuildBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.mBuildState == BuildState.WanCheng)
			{
				GameInstance.Game.SendGetBuildAward(this.buildData.BuildID);
				Super.ShowNetWaiting(null);
			}
		};
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.BuildTaskNeedTime_S = 0;
		this.BuildRemainingTime_S = 0f;
	}

	private string GetBuildOutPutSignSpriteName()
	{
		if (this.mBuildAward == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildLevel.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					if (Global.GetXElementAttributeInt(xelement, "BuildID") == this.mBuildData.BuildID && Global.GetXElementAttributeInt(xelement, "Level") == this.mBuildData.Lev)
					{
						this.mBuildAward = new BuildAward();
						this.mBuildAward.AwardExp = (uint)Global.GetXElementAttributeInt(xelement, "Exp");
						this.mBuildAward.AwardMoney = (uint)Global.GetXElementAttributeInt(xelement, "Money");
						this.mBuildAward.AwardMoJing = Global.GetXElementAttributeFloat(xelement, "MoJing");
						this.mBuildAward.AwardXingHun = Global.GetXElementAttributeFloat(xelement, "XingHun");
						this.mBuildAward.AwardChengJiu = Global.GetXElementAttributeFloat(xelement, "ChengJiu");
						this.mBuildAward.AwardShengWang = Global.GetXElementAttributeFloat(xelement, "ShengWang");
						this.mBuildAward.AwardYuanSu = Global.GetXElementAttributeFloat(xelement, "YuanSu");
						this.mBuildAward.AwardYingGuang = (uint)Global.GetXElementAttributeInt(xelement, "YingGuang");
						this.mBuildAward.LevelNeedEXP = (uint)Global.GetXElementAttributeInt(xelement, "UpNeedExp");
						this.mBuildAward.Lev = Global.GetXElementAttributeInt(xelement, "Level");
						break;
					}
				}
			}
		}
		if (this.mBuildAward != null)
		{
			if (this.mBuildAward.AwardChengJiu != 0f)
			{
				return "chengJiu";
			}
			if (this.mBuildAward.AwardMoJing != 0f)
			{
				return "moJing";
			}
			if (this.mBuildAward.AwardXingHun != 0f)
			{
				return "xingHun";
			}
			if (this.mBuildAward.AwardShengWang != 0f)
			{
				return "shengWang";
			}
			if (this.mBuildAward.AwardYuanSu != 0f)
			{
				return string.Empty;
			}
			if (this.mBuildAward.AwardYingGuang != 0U)
			{
				return string.Empty;
			}
		}
		return string.Empty;
	}

	private Vector3 GetBuildColliderCenterPos(byte index)
	{
		switch (index)
		{
		case 0:
			return new Vector3(27f, -78f, 0.0001f);
		case 1:
			return new Vector3(27f, -72f, 0.0001f);
		case 2:
			return new Vector3(27f, -74f, 0.0001f);
		case 3:
			return new Vector3(27f, -72f, 0.0001f);
		case 4:
			return new Vector3(27f, -77f, 0.0001f);
		case 5:
			return new Vector3(27f, -6f, 0.0001f);
		case 6:
			return new Vector3(27f, -98f, 0.0001f);
		default:
			return Vector3.zero;
		}
	}

	private Vector3 GetBuildColliderSize(byte index)
	{
		switch (index)
		{
		case 0:
			return new Vector3(100f, 215f, 1f);
		case 1:
			return new Vector3(107f, 205f, 1f);
		case 2:
			return new Vector3(100f, 205f, 1f);
		case 3:
			return new Vector3(125f, 203f, 1f);
		case 4:
			return new Vector3(156f, 223f, 1f);
		case 5:
			return new Vector3(120f, 156f, 1f);
		case 6:
			return new Vector3(120f, -156f, 1f);
		default:
			return Vector3.zero;
		}
	}

	private Vector3 GetBuildPos(byte Index)
	{
		switch (Index)
		{
		case 0:
			return new Vector3(-402f, -15f, -2f);
		case 1:
			return new Vector3(-34f, -15f, -2f);
		case 2:
			return new Vector3(-566f, 35f, -2f);
		case 3:
			return new Vector3(-225f, 35f, -2f);
		case 4:
			return new Vector3(253f, -6f, -2f);
		case 5:
			return new Vector3(88f, 115f, -2f);
		case 6:
			return new Vector3(478f, 44f, -2f);
		default:
			return Vector3.zero;
		}
	}

	private Vector3 GetNetImagePos(byte Index)
	{
		switch (Index)
		{
		case 0:
		case 1:
		case 2:
		case 3:
		case 4:
			return new Vector3(29f, -100f, -100f);
		case 5:
			return new Vector3(29f, -42f, -100f);
		case 6:
			return new Vector3(29f, -100f, -100f);
		default:
			return Vector3.zero;
		}
	}

	private void SetProgressBar(float value, string str = "00:00:00", bool bShow = true)
	{
		NGUITools.SetActive(this.ProgressBar.gameObject, true);
		if (bShow)
		{
			this.ProgressBar.sliderValue = value;
			this.mProgressBarlabel.text = Global.GetLang(str);
		}
	}

	private void SetBtn(string Btnstr, bool bShow = true)
	{
		if (bShow)
		{
			this.BuildBtn.Label.text = Global.GetLang(Btnstr);
			Transform child = this.BuildBtn.transform.GetChild(2);
			if (null != child)
			{
				NGUITools.SetActive(child.gameObject, false);
			}
		}
		NGUITools.SetActive(this.BuildBtn.gameObject, bShow);
	}

	private void SetBtnStr(GButton btn, string str)
	{
		if (null != btn && null != btn.target)
		{
			btn.normalSprite = str;
			btn.hoverSprite = str;
			btn.pressedSprite = str;
			btn.disabledSprite = str;
		}
	}

	private int[] StringToDateTime(string timeStr)
	{
		int[] array = new int[6];
		string text = string.Empty;
		byte b = 0;
		byte b2 = 0;
		while ((int)b2 <= timeStr.Length)
		{
			if (timeStr.Length == (int)b2)
			{
				int.TryParse(text, ref array[(int)b]);
			}
			else if (timeStr.get_Chars((int)b2).ToString() != " " && timeStr.get_Chars((int)b2).ToString() != "-" && timeStr.get_Chars((int)b2).ToString() != ":")
			{
				text += timeStr.get_Chars((int)b2).ToString();
			}
			else
			{
				string text2 = text;
				int[] array2 = array;
				byte b3 = b;
				b = b3 + 1;
				int.TryParse(text2, ref array2[(int)b3]);
				text = string.Empty;
			}
			b2 += 1;
		}
		return array;
	}

	private string TimeToString(int second)
	{
		int num = second % 60;
		int num2 = (second - num) / 3600;
		int number = (second - num) / 60 - num2 * 60;
		return string.Concat(new string[]
		{
			this.GetTwoOrderNumber(num2),
			":",
			this.GetTwoOrderNumber(number),
			":",
			this.GetTwoOrderNumber(num)
		});
	}

	private string GetTwoOrderNumber(int number)
	{
		if (10 > number)
		{
			return "0" + number.ToString();
		}
		return number.ToString();
	}

	private float GetProgressbarValue()
	{
		if (0 >= this.BuildTaskNeedTime_S)
		{
			return 0f;
		}
		return this.BuildRemainingTime_S / (float)this.BuildTaskNeedTime_S;
	}

	private long GetReRemainingTime()
	{
		if (this.mBuildDevelopTime != null && this.mBuildDevelopTime.mHaveTime)
		{
			DateTime correctDateTime = TimeManager.GetCorrectDateTime();
			int num = correctDateTime.Hour - this.mBuildDevelopTime.mTimeArray[3];
			int num2 = correctDateTime.Minute - this.mBuildDevelopTime.mTimeArray[4];
			int num3 = correctDateTime.Second - this.mBuildDevelopTime.mTimeArray[5];
			if (0 > num)
			{
				num += 24;
			}
			return (long)(num * 60 * 60 + num2 * 60 + num3);
		}
		return 0L;
	}

	private void ShowTask(int ID)
	{
		if (null != this.mBuildTaskWindow)
		{
			this.mBuildTaskWindow.Visibility = true;
		}
		else
		{
			this.mBuildTaskWindow = U3DUtils.NEW<GChildWindow>();
			this.mBuildTaskWindow.transform.localPosition = new Vector3(0f, 0f, -300f);
			this.mBuildTaskWindow.transform.SetParent(base.transform.parent.parent, false);
			this.mBuildTaskWindow.ModalType = ChildWindowModalType.Translucent;
		}
		this.mBuildTask = U3DUtils.NEW<BuildTaskPart>();
		this.mBuildTaskWindow.Body.Add(this.mBuildTask);
		this.mBuildTask.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.mBuildTask.Hander = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0)
			{
				this.CloseBuildTaskWindow();
			}
		};
		this.mBuildTask.MBuildID = ID;
		this.mBuildTask.InitXml();
		this.UpdateTaskInfo();
	}

	private void CloseBuildTaskWindow()
	{
		if (null != this.mBuildTask)
		{
			Object.Destroy(this.mBuildTaskWindow.gameObject);
			this.mBuildTask = null;
			this.mBuildTaskWindow = null;
		}
	}

	public void UpdateTaskInfo()
	{
		if (null != this.mBuildTask)
		{
			this.mBuildTask.InitTask(this.buildData, this.taskId, this.BuildURLStr, this.MaxLev);
		}
	}

	public override void Update()
	{
		base.Update();
		if (5 > this.buildData.BuildID)
		{
			this.UpDateBuildLifeTime(Time.deltaTime);
		}
	}

	public void UpDateBuildLifeTime(float dTime)
	{
		if (this.buildState == BuildState.GongZuo)
		{
			this.BuildRemainingTime_S += dTime;
			if (this.BuildRemainingTime_S < (float)this.BuildTaskNeedTime_S)
			{
				this.SetProgressBar(this.GetProgressbarValue(), this.TimeToString(this.BuildTaskNeedTime_S - (int)this.BuildRemainingTime_S), true);
				this.SetBtn(this.BtnStr[0], false);
			}
			else
			{
				this.SetProgressBar(0.999f, "00:00:00", true);
				this.SetBtn(this.BtnStr[0], true);
				this.buildState = BuildState.WanCheng;
			}
		}
	}

	public UIDraggablePanel MPC
	{
		set
		{
			if (null == this.mPC)
			{
				this.mPC = base.GetComponent<UIDragPanelContents>();
			}
			if (null == this.mPC)
			{
				this.mPC = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			this.mPC.draggablePanel = value;
		}
	}

	public int HuaFeiUsermoney
	{
		get
		{
			return 0;
		}
	}

	public bool EnabledBuildBtn
	{
		set
		{
			BoxCollider component = this.BuildBtn.GetComponent<BoxCollider>();
			Transform child = this.BuildBtn.transform.GetChild(2);
			if (null != child)
			{
				NGUITools.SetActive(child.gameObject, false);
			}
		}
	}

	public string BuildURLStr
	{
		get
		{
			if (null != this.mBgShowNetImage)
			{
				return this.mBgShowNetImage.URL;
			}
			return string.Empty;
		}
		set
		{
			if (null != this.mBgShowNetImage)
			{
				this.mBgShowNetImage.URL = value;
				this.mBgShowNetImage.ImageDownloaded = delegate(object o)
				{
					if (null != this.mBgShowNetImage.Texture && null != this.mBgShowNetImage.Texture.mainTexture)
					{
						this.mBgShowNetImage.transform.localScale = new Vector3((float)this.mBgShowNetImage.ItsSizeWidth, (float)this.mBgShowNetImage.ItsSizeHeight, 1f);
					}
				};
				NGUITools.SetActive(this.mBgShowNetImage.gameObject, true);
			}
		}
	}

	public string BuildTitleText
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				this.TitleLabel.text = value;
			}
		}
	}

	public int MaxLev
	{
		get
		{
			return this.mMaxLev;
		}
		set
		{
			this.mMaxLev = value;
		}
	}

	public BuildState buildState
	{
		get
		{
			return this.mBuildState;
		}
		set
		{
			this.mBuildState = value;
			if (this.mBuildState == BuildState.GongZuo)
			{
				this.SetBtn(this.BtnStr[0], false);
				this.SetProgressBar(this.GetProgressbarValue(), Global.GetLang(this.TimeToString(this.BuildTaskNeedTime_S - (int)this.BuildRemainingTime_S)), true);
				NGUITools.SetActive(this.GanTanHao, false);
				this.CloseBuildTaskWindow();
			}
			else if (this.mBuildState == BuildState.KongXian)
			{
				NGUITools.SetActive(this.ProgressBar.gameObject, false);
				NGUITools.SetActive(this.BuildBtn.gameObject, false);
				NGUITools.SetActive(this.GanTanHao, false);
			}
			else
			{
				this.SetProgressBar(0.99f, "00:00:00", true);
				this.SetBtn(this.BtnStr[0], true);
				NGUITools.SetActive(this.GanTanHao, true);
			}
			if (this.buildData != null && this.buildData.BuildID == 7)
			{
				NGUITools.SetActive(this.ProgressBar.gameObject, false);
				NGUITools.SetActive(this.BuildBtn.gameObject, false);
				NGUITools.SetActive(this.GanTanHao, false);
				if (this.mBuildState == BuildState.GongZuo)
				{
					this.BuildURLStr = this.path_Task + "Build_LaoFangTask_1.png";
				}
				else if (this.mBuildState == BuildState.WanCheng)
				{
					this.BuildURLStr = this.path_Task + "Build_LaoFangTask_2.png";
				}
				else if (this.mBuildState == BuildState.KongXian)
				{
				}
			}
			this.UpdateTaskInfo();
		}
	}

	public int ID
	{
		get
		{
			return this.mBuildData.BuildID;
		}
		set
		{
			this.mConfigbuildTaskXml = new BuildItem.ConfigbuildTaskXml(value);
			this.mBuildData.BuildID = value;
			string empty = string.Empty;
			if (BuildMainPart.LDName.TryGetValue(this.mBuildData.BuildID, ref empty))
			{
				if (1 > this.mBuildData.Lev)
				{
					this.TitleLabel.text = Global.GetLang(empty);
				}
				else
				{
					this.TitleLabel.text = Global.GetLang(empty) + "  " + this.mBuildData.Lev.ToString();
				}
			}
			string buildOutPutSignSpriteName = this.GetBuildOutPutSignSpriteName();
			if (!string.IsNullOrEmpty(buildOutPutSignSpriteName))
			{
				this.SignSp.spriteName = buildOutPutSignSpriteName;
			}
			if (this.mConfigbuildTaskXml.BuildHaveTask(this.mBuildData.BuildID))
			{
			}
			this.SetBtnStr(this.BuildSelfBtn, this.SignSp.spriteName);
			this.SignSp.transform.localScale = this.SignlabelSize[BuildMainPart.BuildIDToArrayId(this.mBuildData.BuildID)];
			base.transform.localPosition = this.GetBuildPos((byte)BuildMainPart.BuildIDToArrayId(this.mBuildData.BuildID));
		}
	}

	private GameObject LoadTeXiao(string name)
	{
		Object @object;
		if (5 > this.mBuildData.BuildID)
		{
			@object = Resources.Load(this.Path_TeXiao + name);
		}
		else
		{
			@object = Resources.Load("UITeXiao/Perfabs/" + name);
		}
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(this.ImageRoot.transform, false);
			this.ImageRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("MUUI");
			return gameObject;
		}
		return null;
	}

	private void AddShengJiTeXiao()
	{
		Object @object = Resources.Load("UITeXiao/Perfabs/shengji/lingyu_shengji");
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(this.ImageRoot.transform, false);
			this.ImageRoot.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.layer = LayerMask.NameToLayer("MUUI");
			DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
			delayDestroy.delayTime = 2f;
			Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (!gameObject.Equals(componentsInChildren[i].gameObject))
					{
						componentsInChildren[i].localPosition -= new Vector3(0f, 0f, -120f);
					}
				}
			}
		}
	}

	public int Lev
	{
		get
		{
			return this.mBuildData.Lev;
		}
		set
		{
			if (this.mBuildData.Lev != value)
			{
				if (this.mBuildData.Lev != 0 && this.mConfigbuildTaskXml.BuildHaveTask(this.mBuildData.BuildID))
				{
					this.AddShengJiTeXiao();
				}
				GameInstance.Game.SendGetBuildLevelAwardState();
			}
			this.mBuildData.Lev = value;
			string empty = string.Empty;
			if (BuildMainPart.LDName.TryGetValue(this.mBuildData.BuildID, ref empty))
			{
				if (this.mConfigbuildTaskXml.BuildHaveTask(this.mBuildData.BuildID))
				{
					if (1 > this.mBuildData.Lev)
					{
						this.TitleLabel.text = Global.GetLang(empty);
					}
					else
					{
						this.TitleLabel.text = Global.GetLang(empty) + "  Lv " + this.mBuildData.Lev.ToString();
					}
				}
				else
				{
					this.TitleLabel.text = Global.GetLang(empty);
					this.TitleLabel.pivot = 4;
					this.TitleLabel.transform.localPosition = new Vector3(0f, this.TitleLabel.transform.localPosition.y, this.TitleLabel.transform.localPosition.z);
					Transform transform = this.TitleLabel.transform.parent.FindChild("Sprite");
					if (null != transform)
					{
						UISprite component = transform.GetComponent<UISprite>();
						if (null != component)
						{
							component.transform.localScale = new Vector3((float)this.TitleLabel.relativePixelWidth + 100f, component.transform.localScale.y, component.transform.localScale.z);
						}
					}
				}
			}
			string[] buildPicArrayByBuildID = this.mConfigBuildXml.GetBuildPicArrayByBuildID(this.ID);
			int[] buildLevelArrayByBuildID = this.mConfigBuildXml.GetBuildLevelArrayByBuildID(this.ID);
			BoxCollider component2 = this.BuildSelfBtn.GetComponent<BoxCollider>();
			if (null != component2)
			{
				component2.size = this.GetBuildColliderSize((byte)BuildMainPart.BuildIDToArrayId(this.ID));
				component2.center = this.GetBuildColliderCenterPos((byte)BuildMainPart.BuildIDToArrayId(this.ID));
			}
			int num = 0;
			if (buildLevelArrayByBuildID.Length == 1)
			{
				num = 0;
			}
			else if ((buildLevelArrayByBuildID[0] <= this.Lev && buildLevelArrayByBuildID[1] > this.mBuildData.Lev) || this.mBuildData.Lev == 0)
			{
				num = 0;
			}
			else if (buildLevelArrayByBuildID[1] <= this.mBuildData.Lev && buildLevelArrayByBuildID[2] > this.mBuildData.Lev)
			{
				num = 1;
			}
			else if (buildLevelArrayByBuildID[2] <= this.mBuildData.Lev && buildLevelArrayByBuildID[3] > this.mBuildData.Lev)
			{
				num = 2;
			}
			else if (buildLevelArrayByBuildID[3] <= this.mBuildData.Lev)
			{
				num = 3;
			}
			if (null != this.TeXiaoObj)
			{
				Object.Destroy(this.TeXiaoObj);
			}
			if (5 > this.mBuildData.BuildID)
			{
				this.TeXiaoObj = this.LoadTeXiao(string.Format(this.Name_TeXiao[BuildMainPart.BuildIDToArrayId(this.ID)], num + 1));
			}
			else
			{
				this.TeXiaoObj = this.LoadTeXiao(this.Name_TeXiao[BuildMainPart.BuildIDToArrayId(this.ID)]);
			}
			if (null != this.TeXiaoObj)
			{
				this.mBgShowNetImage = this.TeXiaoObj.GetComponentInChildren<ShowNetImage>();
				this.TeXiaoObj.transform.parent.localPosition = this.GetNetImagePos((byte)BuildMainPart.BuildIDToArrayId(this.ID));
				this.mBgShowNetImage.transform.name = "Texture";
				if (null != this.mBgShowNetImage)
				{
					this.BuildURLStr = string.Format("{0}{1}.png", this.path_Task, buildPicArrayByBuildID[num]);
				}
			}
			string buildOutPutSignSpriteName = this.GetBuildOutPutSignSpriteName();
			if (!string.IsNullOrEmpty(buildOutPutSignSpriteName))
			{
				this.SignSp.spriteName = buildOutPutSignSpriteName;
			}
		}
	}

	public int EXP
	{
		get
		{
			if (this.mBuildData != null)
			{
				return this.mBuildData.Exp;
			}
			return 0;
		}
		set
		{
			this.mBuildData.Exp = value;
		}
	}

	public int[] taskArray
	{
		get
		{
			return new int[]
			{
				this.mBuildData.Task1,
				this.mBuildData.Task2,
				this.mBuildData.Task3
			};
		}
		set
		{
			this.UpdateTaskInfo();
			if (3 <= value.Length)
			{
				this.mBuildData.Task1 = value[0];
				this.mBuildData.Task2 = value[1];
				this.mBuildData.Task3 = value[2];
			}
		}
	}

	public string DevelopTime
	{
		get
		{
			return this.mBuildData.DevelopTime;
		}
		set
		{
			this.mBuildData.DevelopTime = value;
			DateTime correctDateTime = TimeManager.GetCorrectDateTime();
			int[] array = this.StringToDateTime(this.mBuildData.DevelopTime);
			if ("0000-00-00 00:00:00" != this.mBuildData.DevelopTime)
			{
				this.mBuildDevelopTime.mHaveTime = true;
				byte b = 0;
				while ((int)b < array.Length)
				{
					this.mBuildDevelopTime.mTimeArray[(int)b] = array[(int)b];
					b += 1;
				}
			}
			else
			{
				this.mBuildDevelopTime.mHaveTime = false;
			}
			if (1 <= correctDateTime.Year - array[0] || 1 <= correctDateTime.Month - array[1] || 1 < correctDateTime.Day - array[2])
			{
				int num = array[0];
				DateTime minValue = DateTime.MinValue;
				if (num <= minValue.Year)
				{
					this.buildState = BuildState.KongXian;
				}
				else
				{
					this.buildState = BuildState.WanCheng;
				}
			}
			else
			{
				this.BuildRemainingTime_S = (float)this.GetReRemainingTime();
				if (0f <= this.BuildRemainingTime_S - (float)this.BuildTaskNeedTime_S && this.BuildTaskNeedTime_S != 0)
				{
					this.buildState = BuildState.WanCheng;
				}
				else if (0f > this.BuildRemainingTime_S - (float)this.BuildTaskNeedTime_S && this.BuildTaskNeedTime_S != 0)
				{
					this.buildState = BuildState.GongZuo;
				}
				else
				{
					this.buildState = BuildState.KongXian;
				}
			}
		}
	}

	public int Build_TaskId
	{
		get
		{
			return this.taskId;
		}
		set
		{
			this.taskId = value;
			BuildtaskInF buildTaskXmlData = this.mConfigbuildTaskXml.GetBuildTaskXmlData(this.mBuildData.BuildID, this.taskId);
			if (buildTaskXmlData != null)
			{
				this.BuildTaskNeedTime_S = (int)(buildTaskXmlData.TaskTime * 60U);
			}
			this.DevelopTime = this.mBuildData.DevelopTime;
		}
	}

	public BuildData buildData
	{
		get
		{
			return this.mBuildData;
		}
	}

	[SerializeField]
	private GButton BuildSelfBtn;

	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private GameObject ImageRoot;

	[SerializeField]
	private GImgProgressBar ProgressBar;

	[SerializeField]
	private GButton BuildBtn;

	[SerializeField]
	private UISprite SignSp;

	[SerializeField]
	private GameObject GanTanHao;

	private BuildState mBuildState;

	private BuildItem.BuildTimeData mBuildDevelopTime = new BuildItem.BuildTimeData();

	private BuildData mBuildData = new BuildData();

	private ConfigBuildXml mConfigBuildXml;

	private int taskId;

	private float BuildRemainingTime_S;

	private int BuildTaskNeedTime_S;

	private string[] BtnStr = new string[]
	{
		Global.GetLang("领取奖励"),
		Global.GetLang("开启"),
		Global.GetLang("一键完成")
	};

	private Vector3[] SignlabelSize = new Vector3[]
	{
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f),
		new Vector3(27f, 27f, 1f)
	};

	private string path_Task = "NetImages/GameRes/Images/Build/BuildTask/";

	private string Path_TeXiao = "UITeXiao/Perfabs/lingdi/";

	private string[] Name_TeXiao = new string[]
	{
		"zhanshenta/ZhanShenTa_0{0}",
		"mofata/MoFaZhiTa_0{0}",
		"zhanxingta/ZhanXingTai_0{0}",
		"shilianzhimen/ShiLianZhiMen_0{0}",
		"jinglingshenji/ShiJieZhanYi_effect",
		"jinglingshenji/ShenJi_effect",
		"jinglingshenji/LaoFang_effect"
	};

	private UILabel mProgressBarlabel;

	private GChildWindow mBuildTaskWindow;

	private BuildTaskPart mBuildTask;

	private int mMaxLev;

	private ShowNetImage mBgShowNetImage;

	private GameObject TeXiaoObj;

	private BuildItem.ConfigbuildTaskXml mConfigbuildTaskXml;

	private BuildAward mBuildAward;

	private UIDragPanelContents mPC;

	public DPSelectedItemEventHandler Hander;

	private class BuildTimeData
	{
		public BuildTimeData()
		{
			this.mHaveTime = false;
			this.mTimeArray = new int[6];
		}

		public bool mHaveTime;

		public int[] mTimeArray;
	}

	private class ConfigbuildTaskXml
	{
		public ConfigbuildTaskXml(int BuildID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildTask.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
				int num = xelementList.Count;
				if (16 < num)
				{
					num = 16;
				}
				for (int i = 0; i < num; i++)
				{
					XElement xelement = xelementList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "BuildID");
					if (BuildID == xelementAttributeInt)
					{
						BuildtaskInF buildtaskInF = new BuildtaskInF();
						buildtaskInF.BuildID = xelementAttributeInt;
						buildtaskInF.TaskID = Global.GetXElementAttributeInt(xelement, "ID");
						buildtaskInF.TaskName = Global.GetXElementAttributeStr(xelement, "Name");
						buildtaskInF.Quality = Global.GetXElementAttributeFloat(xelement, "Quality");
						buildtaskInF.Sum = Global.GetXElementAttributeFloat(xelement, "Sum");
						buildtaskInF.ExpNum = Global.GetXElementAttributeFloat(xelement, "ExpNum");
						buildtaskInF.TaskTime = (uint)Global.GetXElementAttributeInt(xelement, "Time");
						if (this.TaskInF.ContainsKey(buildtaskInF.BuildID))
						{
							this.TaskInF[buildtaskInF.BuildID].Add(buildtaskInF);
						}
						else
						{
							List<BuildtaskInF> list = new List<BuildtaskInF>();
							list.Add(buildtaskInF);
							this.TaskInF.Add(buildtaskInF.BuildID, list);
						}
					}
					else if (!this.TaskInF.ContainsKey(xelementAttributeInt))
					{
						this.TaskInF.Add(xelementAttributeInt, null);
					}
				}
			}
		}

		public BuildtaskInF GetBuildTaskXmlDataByIndex(int BuildID, int index)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null && index < buildTaskXmlDataList.Count)
			{
				return buildTaskXmlDataList[index];
			}
			return null;
		}

		public BuildtaskInF GetBuildTaskXmlData(int BuildID, int TaskID)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null)
			{
				for (int i = 0; i < buildTaskXmlDataList.Count; i++)
				{
					if (TaskID == buildTaskXmlDataList[i].TaskID)
					{
						return buildTaskXmlDataList[i];
					}
				}
			}
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildTask.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
				for (int j = 0; j < xelementList.Count; j++)
				{
					XElement xelement = xelementList[j];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "BuildID");
					if (BuildID == xelementAttributeInt && TaskID == Global.GetXElementAttributeInt(xelement, "ID"))
					{
						return new BuildtaskInF
						{
							BuildID = xelementAttributeInt,
							TaskID = Global.GetXElementAttributeInt(xelement, "ID"),
							TaskName = Global.GetXElementAttributeStr(xelement, "Name"),
							Quality = Global.GetXElementAttributeFloat(xelement, "Quality"),
							Sum = Global.GetXElementAttributeFloat(xelement, "Sum"),
							ExpNum = Global.GetXElementAttributeFloat(xelement, "ExpNum"),
							TaskTime = (uint)Global.GetXElementAttributeInt(xelement, "Time")
						};
					}
				}
			}
			return null;
		}

		public List<BuildtaskInF> GetBuildTaskXmlDataList(int BuildID)
		{
			if (this.TaskInF.ContainsKey(BuildID))
			{
				return this.TaskInF[BuildID];
			}
			return null;
		}

		public bool IsOldTask(int BuildID, int TaskID)
		{
			List<BuildtaskInF> buildTaskXmlDataList = this.GetBuildTaskXmlDataList(BuildID);
			if (buildTaskXmlDataList != null)
			{
				for (int i = 0; i < buildTaskXmlDataList.Count; i++)
				{
					if (TaskID == buildTaskXmlDataList[i].TaskID)
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool BuildHaveTask(int BuildID)
		{
			return this.TaskInF.ContainsKey(BuildID);
		}

		private Dictionary<int, List<BuildtaskInF>> TaskInF = new Dictionary<int, List<BuildtaskInF>>();
	}
}
