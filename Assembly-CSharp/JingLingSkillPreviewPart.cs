using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Tmsk.Xml;

public class JingLingSkillPreviewPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitXml();
		this.InitHandler();
		this.m_CheckBoxs[0].isChecked = false;
		this.m_CheckBoxs[1].isChecked = false;
		this.CheckBoxClickChange(null, null);
	}

	private void InitPrefabText()
	{
		this.m_TitleText.text = Global.GetLang("技能预览");
		this.m_CheckBoxs[0]._Lable.text = Global.GetLang("绿色");
		this.m_CheckBoxs[1]._Lable.text = Global.GetLang("蓝色");
		this.m_CheckBoxs[2]._Lable.text = Global.GetLang("紫色");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		for (int i = 0; i < this.m_CheckBoxs.Length; i++)
		{
			this.m_CheckBoxs[i].CheckChanged = new BaseEventHandler2(this.CheckBoxClickChange);
		}
		this.m_Collection = this.m_ListBox.ItemsSource;
	}

	private void InitXml()
	{
		this.xml_Magics = Global.GetGameResXml("Config/Magics.xml");
		if (this.xml_Magics != null)
		{
			List<XElement> list = Global.GetXElementList(this.xml_Magics, "Magic").FindAll((XElement s) => s.Attribute("SkillType").Value == "201");
			for (int i = 0; i < list.Count; i++)
			{
				JingLingSkillPreviewPart.XmlMagicsData xmlMagicsData = new JingLingSkillPreviewPart.XmlMagicsData();
				xmlMagicsData.skillID = Global.GetXElementAttributeInt(list[i], "ID");
				xmlMagicsData.Name = Global.GetXElementAttributeStr(list[i], "Name");
				xmlMagicsData.MagicScripts = Global.GetXElementAttributeStr(list[i], "MagicScripts");
				xmlMagicsData.Description = Global.GetXElementAttributeStr(list[i], "Description");
				xmlMagicsData.MagicColor = Global.GetXElementAttributeInt(list[i], "MagicColor");
				this.m_DicMagicsInf.Add(xmlMagicsData.skillID, xmlMagicsData);
			}
		}
		XElement gameResXml = Global.GetGameResXml("Config/PetSkillLevelup.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Levelup");
			for (int j = 0; j < xelementList.Count; j++)
			{
				if (xelementList != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[j], "Level");
					if (this.m_MaxSkillsLev <= xelementAttributeInt)
					{
						this.m_MaxSkillsLev = xelementAttributeInt;
					}
				}
			}
		}
	}

	private void CheckBoxClickChange(object sender, BaseEventArgs e)
	{
		bool[] array = new bool[3];
		for (int i = 0; i < this.m_CheckBoxs.Length; i++)
		{
			this.m_CheckBoxs[i].isChecked = this.m_CheckBoxs[i].Equals(sender);
			array[i] = this.m_CheckBoxs[i].isChecked;
		}
		if (sender == null)
		{
			for (int j = 0; j < this.m_CheckBoxs.Length; j++)
			{
				if (j == 2)
				{
					array[j] = true;
					this.m_CheckBoxs[j].isChecked = true;
				}
				else
				{
					array[j] = false;
					this.m_CheckBoxs[j].isChecked = false;
				}
			}
		}
		base.StartCoroutine<bool>(this.AddItem(array));
	}

	private IEnumerator AddItem(bool[] color)
	{
		if (null == this.m_SP)
		{
			this.m_SP = this.m_ListBox.transform.parent.GetComponent<SpringPanel>();
		}
		this.m_Collection.Clear();
		Dictionary<int, JingLingSkillPreviewPart.XmlMagicsData>.Enumerator en_Dic = this.m_DicMagicsInf.GetEnumerator();
		int index = 0;
		while (en_Dic.MoveNext())
		{
			KeyValuePair<int, JingLingSkillPreviewPart.XmlMagicsData> keyValuePair = en_Dic.Current;
			JingLingSkillPreviewPart.XmlMagicsData d = keyValuePair.Value;
			if (color[d.MagicColor - 1])
			{
				string Content = d.Description;
				string color_ = (d.MagicColor != 1) ? ((d.MagicColor != 2) ? "b266ff" : "3681f3") : "17e43e";
				string name = Global.GetColorStringForNGUIText(new object[]
				{
					color_,
					d.Name
				});
				JingLingSkillPreviewItem item = U3DUtils.NEW<JingLingSkillPreviewItem>();
				UIDragPanelContents pc = item.gameObject.AddComponent<UIDragPanelContents>();
				pc.draggablePanel = this.m_ListBox.GetComponentInParent<UIDraggablePanel>();
				string str_Attack = string.Format("{0}%({1})%", Global.GetJIngLingSkillAddAttack(d.MagicScripts, 1), Global.GetJIngLingSkillAddAttack(d.MagicScripts, this.m_MaxSkillsLev));
				string str_C = string.Format(Content, str_Attack);
				item.SetSkillInf(d.skillID, name, Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					str_C
				}));
				this.m_Collection.AddNoUpdate(item);
			}
			if (index > 5)
			{
				yield return null;
				this.m_ListBox.repositionNow = true;
			}
		}
		if (null != this.m_SP)
		{
			this.m_SP.target.y = -35f;
			this.m_SP.enabled = true;
		}
		yield break;
	}

	public UILabel m_TitleText;

	public GCheckBox[] m_CheckBoxs;

	public ListBox m_ListBox;

	public GButton M_CloseBtn;

	private XElement xml_Magics;

	private Dictionary<int, JingLingSkillPreviewPart.XmlMagicsData> m_DicMagicsInf = new Dictionary<int, JingLingSkillPreviewPart.XmlMagicsData>();

	private ObservableCollection m_Collection;

	private int m_MaxSkillsLev;

	private SpringPanel m_SP;

	public class XmlMagicsData
	{
		public int skillID;

		public int MagicColor;

		public string Name;

		public string Description;

		public string MagicScripts;
	}
}
