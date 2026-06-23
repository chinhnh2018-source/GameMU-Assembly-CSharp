using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;
using UnityEngine;

public class BuildMessageBox : UserControl
{
	private void InitPrefabsText()
	{
		try
		{
			this.m_HuanFeiLabel[0].text = Global.GetLang("花费");
			this.m_MessageTitle.text = BuildFintColor.Yellow + Global.GetLang("雇佣工人") + BuildFintColor.End;
			this.m_AwardSignSp[0].spriteName = "BuildTaskExp";
			this.m_AwardSignSp[2].spriteName = "TaskGold";
			this.m_HuanFeiLabel[0].transform.localPosition = new Vector3(-106f, 0f, 0f);
		}
		catch (Exception ex)
		{
			Debug.LogError("越南预制可能报空！");
		}
	}

	private void InitBtnHandler()
	{
		this.m_Btn[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.Destroy();
		};
		if (null != this.m_Btn[1].Label)
		{
			this.m_Btn[1].Label.text = Global.GetLang("确定");
		}
		this.m_Btn[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_MessageType == BuildMessageBoxType.YiJianWanCheng)
			{
				GameInstance.Game.SendQuickFinishTask(this.m_BuildID);
			}
			else if (this.m_MessageType == BuildMessageBoxType.TiShi)
			{
				GameInstance.Game.SendOpenQueue();
			}
			this.Destroy();
		};
		if (null != this.m_Btn[2].Label)
		{
			this.m_Btn[2].Label.text = Global.GetLang("取消");
		}
		this.m_Btn[2].MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.Destroy();
		};
	}

	private string WipeOffStrLast0(string str)
	{
		for (int i = str.Length - 1; i >= 0; i--)
		{
			if ('\0' < str.get_Chars(i))
			{
				break;
			}
			str.Remove(i);
		}
		return str;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabsText();
		if (null != this.m_mask)
		{
			this.m_mask.alpha = 0.7f;
		}
		this.xml_BuildTask = Global.GetGameResXml("Config/Manor/BuildTask.xml");
		this.xml_BuildLevel = Global.GetGameResXml("Config/Manor/BuildLevel.xml");
		this.InitBtnHandler();
	}

	public override void Destroy()
	{
		base.Destroy();
		if (null != base.gameObject)
		{
			GameObject gameObject = null;
			if (!(null == base.transform.parent) && !(null == base.transform.parent.parent))
			{
				gameObject = base.transform.parent.parent.gameObject;
			}
			NGUITools.Destroy(base.gameObject);
			if (null != gameObject)
			{
				NGUITools.Destroy(gameObject);
			}
		}
	}

	public void RefreshContent(int TaskID, int BuildID, int BuildLev, int Price, BuildMessageBoxType type = BuildMessageBoxType.YiJianWanCheng)
	{
		this.m_MessageType = type;
		if (type == BuildMessageBoxType.YiJianWanCheng)
		{
			this.m_MessageTitle.text = BuildFintColor.Yellow + Global.GetLang("一键完成") + BuildFintColor.End;
			this.m_HuanFeiLabel[2].text = Global.GetLang("完成当前任务可获得：");
			NGUITools.SetActive(this.m_HuanFeiLabel[3], false);
			this.m_BuildID = BuildID;
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			int num4 = 0;
			int num5 = 0;
			float num6 = 0f;
			int num7 = 0;
			if (this.xml_BuildTask != null)
			{
				List<XElement> xelementList = Global.GetXElementList(this.xml_BuildTask, "BuildTask");
				foreach (XElement xelement in xelementList)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					if (TaskID == xelementAttributeInt)
					{
						num = Global.GetXElementAttributeFloat(xelement, "Quality");
						num2 = Global.GetXElementAttributeFloat(xelement, "Sum");
						num3 = Global.GetXElementAttributeFloat(xelement, "ExpNum");
						num7 = Global.GetXElementAttributeInt(xelement, "Time");
					}
				}
			}
			if (this.xml_BuildLevel != null)
			{
				List<XElement> xelementList2 = Global.GetXElementList(this.xml_BuildLevel, "BuildTask");
				foreach (XElement xelement2 in xelementList2)
				{
					if (Global.GetXElementAttributeInt(xelement2, "BuildID") == BuildID && Global.GetXElementAttributeInt(xelement2, "Level") == BuildLev)
					{
						num4 = Global.GetXElementAttributeInt(xelement2, "Exp");
						num5 = Global.GetXElementAttributeInt(xelement2, "Money");
						float[] array = new float[]
						{
							Global.GetXElementAttributeFloat(xelement2, "MoJing"),
							Global.GetXElementAttributeFloat(xelement2, "XingHun"),
							Global.GetXElementAttributeFloat(xelement2, "ChengJiu"),
							Global.GetXElementAttributeFloat(xelement2, "ShengWang")
						};
						byte b = 0;
						while ((int)b < array.Length)
						{
							if (array[(int)b] != 0f)
							{
								num6 = array[(int)b];
								break;
							}
							b += 1;
						}
						break;
					}
				}
			}
			for (int i = 0; i < this.m_Awardlabel.Length; i++)
			{
				NGUITools.SetActive(this.m_Awardlabel[i].gameObject, true);
			}
			for (int j = 0; j < this.m_Awardlabel.Length; j++)
			{
				NGUITools.SetActive(this.m_AwardSignSp[j].gameObject, true);
			}
			this.m_AwardSignSp[1].spriteName = BuildMainPart.GetLdHintTitileName()[BuildID - 1];
			this.m_Awardlabel[0].text = Global.GetLang(string.Concat(new string[]
			{
				(num4 * num7).ToString(),
				this.m_ContentColor[(int)(num - 1f)],
				"  X",
				this.WipeOffStrLast0(num3.ToString()),
				"{-}"
			}));
			this.m_Awardlabel[1].text = Global.GetLang(string.Concat(new string[]
			{
				(num6 * (float)num7).ToString(),
				this.m_ContentColor[(int)(num - 1f)],
				"  X",
				this.WipeOffStrLast0((num2 - num3).ToString()),
				"{-}"
			}));
			this.m_Awardlabel[2].text = Global.GetLang(string.Concat(new string[]
			{
				(num5 * num7).ToString(),
				this.m_ContentColor[(int)(num - 1f)],
				"  X",
				this.WipeOffStrLast0((num2 - num3).ToString()),
				"{-}"
			}));
		}
		else if (type == BuildMessageBoxType.TiShi)
		{
			this.m_MessageTitle.text = BuildFintColor.Yellow + Global.GetLang("雇佣工人") + BuildFintColor.End;
			this.m_HuanFeiLabel[2].text = Global.GetLang("雇佣一个临时工人临");
			NGUITools.SetActive(this.m_HuanFeiLabel[3], true);
			this.m_HuanFeiLabel[3].text = Global.GetLang("时工人只能完成一次开发");
			for (int k = 0; k < this.m_Awardlabel.Length; k++)
			{
				NGUITools.SetActive(this.m_Awardlabel[k].gameObject, false);
			}
			for (int l = 0; l < this.m_Awardlabel.Length; l++)
			{
				NGUITools.SetActive(this.m_AwardSignSp[l].gameObject, false);
			}
		}
		this.m_HuanFeiLabel[1].text = BuildFintColor.Green + Price.ToString() + BuildFintColor.End;
	}

	public UILabel m_MessageTitle;

	public UISprite m_mask;

	public UILabel[] m_HuanFeiLabel = new UILabel[4];

	public UILabel[] m_Awardlabel = new UILabel[3];

	public UISprite[] m_AwardSignSp = new UISprite[3];

	public GButton[] m_Btn = new GButton[3];

	private XElement xml_BuildTask;

	private XElement xml_BuildLevel;

	private int m_BuildID;

	private BuildMessageBoxType m_MessageType;

	private string[] m_ContentColor = new string[]
	{
		"{f0f0f0}",
		"{17e43e}",
		"{3681f3}",
		"{b266ff}"
	};
}
