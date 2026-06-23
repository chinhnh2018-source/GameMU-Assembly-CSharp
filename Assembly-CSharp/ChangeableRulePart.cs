using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ChangeableRulePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitXMlData();
		this.mObservableCollection = this.mListBoxRuleListbox.ItemsSource;
	}

	private void InitXMlData()
	{
		List<string> list = new List<string>();
		list.Add("Config/LeagueSuperIntro.xml");
		list.Add("Config/LeagueNewIntro.xml");
		list.Add("Config/LeagueMapIntro.xml");
		this.mXmlData.Add(1, list);
		List<string> list2 = new List<string>();
		list2.Add("Config/CrusadeIntroMatch.xml");
		list2.Add("Config/CrusadeIntroWar.xml");
		this.mXmlData.Add(2, list2);
	}

	private void InitPrefabText()
	{
	}

	private void onDragFinished()
	{
		Vector3 localPosition = this.mDragPanelRole.transform.localPosition;
		if (Math.Abs(Math.Abs(localPosition.x) - 960f * (float)this.CurrentSelectedPage) > 288f)
		{
			if (localPosition.x > -960f * (float)this.CurrentSelectedPage)
			{
				this.CurrentSelectedPage--;
				if (this.CurrentSelectedPage <= 0)
				{
					this.CurrentSelectedPage = 0;
				}
			}
			else
			{
				this.CurrentSelectedPage++;
				if (this.CurrentSelectedPage > this.mMaxPage)
				{
					this.CurrentSelectedPage = this.mMaxPage;
				}
			}
		}
		Vector3 vector;
		vector..ctor(-960f * (float)this.CurrentSelectedPage, this.mDragPanelRole.transform.localPosition.y, this.mDragPanelRole.transform.localPosition.z);
		SpringPanel.Begin(this.mDragPanelRole.gameObject, vector, 5f);
		this.RefreshBtns();
	}

	private void InitTexture()
	{
		try
		{
			this.mBgImage.URL = "NetImages/GameRes/Images/Plate/role_bak3.jpg.qj";
			this.mBgImage.ImageDownloaded = delegate(object s)
			{
				this.mBgImage.transform.localScale = new Vector3((float)this.mBgImage.ItsSizeWidth, (float)this.mBgImage.ItsSizeHeight, 1f);
			};
			this.mBgImage1.URL = "NetImages/GameRes/Images/Plate/ChangeRuleBg.jpg";
			this.mBgImage1.ImageDownloaded = delegate(object s)
			{
				this.mBgImage1.transform.localScale = new Vector3((float)this.mBgImage1.ItsSizeWidth, (float)this.mBgImage1.ItsSizeHeight, 1f);
			};
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mDragPanelRole.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
			this.mBtnClos3.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			if (null == this.mSpBtn1.GetComponent<BoxCollider>())
			{
				this.mSpBtn1.gameObject.AddComponent<BoxCollider>();
			}
			if (null == this.mSpBtn2.GetComponent<BoxCollider>())
			{
				this.mSpBtn2.gameObject.AddComponent<BoxCollider>();
			}
			UIEventListener.Get(this.mSpBtn2.gameObject).onClick = delegate(GameObject e)
			{
				this.CurrentSelectedPage--;
				this.ShowPage(this.CurrentSelectedPage);
			};
			UIEventListener.Get(this.mSpBtn1.gameObject).onClick = delegate(GameObject e)
			{
				this.CurrentSelectedPage++;
				this.ShowPage(this.CurrentSelectedPage);
			};
		}
		catch (Exception ex)
		{
		}
	}

	private void RefreshBtns()
	{
		this.mSpBtn1.gameObject.SetActive(true);
		this.mSpBtn2.gameObject.SetActive(true);
		if (this.CurrentSelectedPage > this.mMaxPage - 1)
		{
			if (this.CurrentSelectedPage >= this.mMaxPage)
			{
				this.CurrentSelectedPage = this.mMaxPage;
			}
			this.mSpBtn1.gameObject.SetActive(false);
		}
		if (this.CurrentSelectedPage < 1)
		{
			if (this.CurrentSelectedPage <= 0)
			{
				this.CurrentSelectedPage = 0;
			}
			this.mSpBtn2.gameObject.SetActive(false);
		}
		if (this.mChangeabelRulePartType == ChangeableRulePart.ChangeabelRulePartType.ZhanmengLianSai)
		{
			if (this.CurrentSelectedPage == this.mMaxPage)
			{
				this.mTitleSp.spriteName = "RuleTitle2";
			}
			else
			{
				this.mTitleSp.spriteName = "RuleTitle1";
			}
		}
		else
		{
			this.mTitleSp.spriteName = "GuiZeXiangQing";
		}
	}

	public void RefreshData(ChangeableRulePart.ChangeabelRulePartType type, int page)
	{
		this.mChangeabelRulePartType = type;
		if (this.mXmlData.ContainsKey((int)type))
		{
			List<ChangeableRulePart.RuleXml> list = new List<ChangeableRulePart.RuleXml>();
			List<string> list2 = this.mXmlData[(int)type];
			if (list2 != null && 0 < list2.Count)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					XElement gameResXml = Global.GetGameResXml(list2[i]);
					if (gameResXml != null)
					{
						ChangeableRulePart.RuleXml ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
						list.Add(ruleXml);
					}
				}
			}
			this.mMaxPage = list.Count - 1;
			int j = 0;
			int count = list.Count;
			while (j < count)
			{
				ChangeableRuleItem changeableRuleItem = U3DUtils.NEW<ChangeableRuleItem>();
				changeableRuleItem.RuleType = type;
				changeableRuleItem.SetInf(list[j].list.GetEnumerator(), j + 1);
				this.mObservableCollection.AddNoUpdate(changeableRuleItem);
				changeableRuleItem.DragPanel = this.mDragPanelRole;
				j++;
			}
		}
		this.ShowPage(page);
	}

	private void ShowPage(int page)
	{
		this.CurrentSelectedPage = page;
		this.RefreshBtns();
		Vector3 vector;
		vector..ctor(-960f * (float)this.CurrentSelectedPage, this.mDragPanelRole.transform.localPosition.y, this.mDragPanelRole.transform.localPosition.z);
		SpringPanel.Begin(this.mDragPanelRole.gameObject, vector, 5f);
	}

	private const float mDistance = 960f;

	[SerializeField]
	private UISprite mTitleSp;

	[SerializeField]
	private GButton mBtnClos3;

	[SerializeField]
	private UIDraggablePanel mDragPanelRole;

	[SerializeField]
	private ListBox mListBoxRuleListbox;

	[SerializeField]
	private UISprite mSpBtn1;

	[SerializeField]
	private UISprite mSpBtn2;

	[SerializeField]
	private ShowNetImage mBgImage;

	[SerializeField]
	private ShowNetImage mBgImage1;

	private ObservableCollection mObservableCollection;

	private ChangeableRulePart.ChangeabelRulePartType mChangeabelRulePartType = ChangeableRulePart.ChangeabelRulePartType.ZhanmengLianSai;

	private Dictionary<int, List<string>> mXmlData = new Dictionary<int, List<string>>();

	private int mMaxPage;

	private int CurrentSelectedPage;

	public DPSelectedItemEventHandler Hander;

	public enum ChangeabelRulePartType
	{
		ZhanmengLianSai = 1,
		KuaFuPluder
	}

	public class RuleXml
	{
		public RuleXml(XElement Xml)
		{
			List<XElement> xelementList = Global.GetXElementList(Xml, "HelpIntro");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					ChangeableRulePart.RuleVO ruleVO = new ChangeableRulePart.RuleVO(xelementList[i]);
					this.list.Add(ruleVO);
				}
			}
		}

		public List<ChangeableRulePart.RuleVO> list = new List<ChangeableRulePart.RuleVO>();
	}

	public class RuleVO
	{
		public RuleVO(XElement xml)
		{
			if (xml != null)
			{
				this.ID = Global.GetXElementAttributeInt(xml, "ID");
				this.Intro = Global.GetXElementAttributeStr(xml, "Intro");
				this.Bold = Global.GetXElementAttributeInt(xml, "Bold");
			}
		}

		public int ID;

		public string Intro;

		public int Bold;
	}
}
