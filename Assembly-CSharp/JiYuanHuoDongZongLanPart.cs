using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JiYuanHuoDongZongLanPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void InitVlaue()
	{
		this.m_ObservableCollection = this.m_Listbox.ItemsSource;
		this.m_ListWidth = this.m_Listbox.cellWidth;
		this.AddJiYuanZongLanPanel();
	}

	public void AddJiYuanZongLanPanel()
	{
		this.m_Bool = true;
		this.m_LingQuCount = 0;
		this.m_DraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("纪元时间：") + DateTime.Parse(this.m_JiYuanConfig.EraUI.StartTime).ToString(Global.GetLang("yyyy年MM月dd日")) + "-" + DateTime.Parse(this.m_JiYuanConfig.EraUI.EndTime).ToString(Global.GetLang("yyyy年MM月dd日"))
		});
		this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(this.m_JiYuanConfig.StageDescription)
		});
		this.m_ShowLogo.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_ShowLogo.transform.localPosition = new Vector3(17f, -8f, -0.5f);
		if (!string.IsNullOrEmpty(this.m_JiYuanConfig.EraUI.Logo))
		{
			string bundleID = MuAssetManager.GetBundleID("UIModel", this.m_JiYuanConfig.EraUI.Logo);
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(this.m_JiYuanConfig.EraUI.Logo, bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			if (emptyLoader != null)
			{
				U3DUtils.AddChild(this.m_ShowLogo.gameObject, emptyLoader, true);
				emptyLoader.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		this.m_ShowDiTu.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ThemeBackground;
		if (this.m_List.Count > 0)
		{
			for (int i = 0; i < this.m_List.Count; i++)
			{
				Object.Destroy(this.m_List[i].gameObject);
			}
			this.m_List.Clear();
		}
		Dictionary<int, EraReward>.Enumerator enumerator = this.m_JiYuanConfig.DicEraRewardJieDuan.GetEnumerator();
		while (enumerator.MoveNext())
		{
			JiYuanHuoDongZongLanPart.<AddJiYuanZongLanPanel>c__AnonStorey268 <AddJiYuanZongLanPanel>c__AnonStorey = new JiYuanHuoDongZongLanPart.<AddJiYuanZongLanPanel>c__AnonStorey268();
			<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this = this;
			JiYuanZongLanItem jiYuanZongLanItem = U3DUtils.NEW<JiYuanZongLanItem>();
			jiYuanZongLanItem.m_ShowBack.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardBackground;
			UILabel labJiYuan = jiYuanZongLanItem.m_LabJiYuan;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num = 1;
			KeyValuePair<int, EraReward> keyValuePair = enumerator.Current;
			array[num] = keyValuePair.Value.Progress.ToString();
			labJiYuan.text = Global.GetColorStringForNGUIText(array);
			JiYuanZongLanItem jiYuanZongLanItem2 = jiYuanZongLanItem;
			KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
			jiYuanZongLanItem2.Progress = keyValuePair2.Value.Progress;
			ObservableCollection itemsSource = jiYuanZongLanItem.m_ListBox.ItemsSource;
			string text = string.Empty;
			KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
			text = keyValuePair3.Value.LeaderReward;
			if (!string.IsNullOrEmpty(text))
			{
				Super.LoadGoodsList(text, itemsSource);
				UIPanel[] componentsInChildren = jiYuanZongLanItem.m_ListBox.GetComponentsInChildren<UIPanel>();
				if (componentsInChildren != null)
				{
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (componentsInChildren[j].GetComponent<UIDragPanelContents>() == null)
						{
							componentsInChildren[j].gameObject.AddComponent<UIDragPanelContents>();
						}
						componentsInChildren[j].gameObject.AddComponent<UIDragPanelContents>().draggablePanel = this.m_DraggablePanel;
						Object.Destroy(componentsInChildren[j]);
					}
				}
			}
			jiYuanZongLanItem.m_ListBox.transform.localPosition = new Vector3(jiYuanZongLanItem.m_ListBox.transform.localPosition.x + (float)((itemsSource.Count - 1) * -40), jiYuanZongLanItem.m_ListBox.transform.localPosition.y, jiYuanZongLanItem.m_ListBox.transform.localPosition.z);
			KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
			int id = keyValuePair4.Value.ID;
			int num2 = -1;
			if (this.m_JiYuanConfig.data.EraAwardStateDict != null && this.m_JiYuanConfig.data.EraAwardStateDict.ContainsKey(id))
			{
				num2 = this.m_JiYuanConfig.data.EraAwardStateDict[id];
			}
			JiYuanHuoDongZongLanPart.<AddJiYuanZongLanPanel>c__AnonStorey268 <AddJiYuanZongLanPanel>c__AnonStorey2 = <AddJiYuanZongLanPanel>c__AnonStorey;
			KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
			<AddJiYuanZongLanPanel>c__AnonStorey2.keyID = keyValuePair5.Value.ID;
			jiYuanZongLanItem.m_Btn.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				Global.GetLang("领取")
			});
			int eraStage = (int)this.m_JiYuanConfig.data.EraStage;
			KeyValuePair<int, EraReward> keyValuePair6 = enumerator.Current;
			if (eraStage < keyValuePair6.Value.Progress)
			{
				jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
				jiYuanZongLanItem.m_Btn.Text = string.Empty;
				jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("本团进度：0%")
				});
				jiYuanZongLanItem.m_SpSuo.gameObject.SetActive(true);
			}
			else
			{
				int eraStage2 = (int)this.m_JiYuanConfig.data.EraStage;
				KeyValuePair<int, EraReward> keyValuePair7 = enumerator.Current;
				if (eraStage2 == keyValuePair7.Value.Progress)
				{
					if (this.m_JiYuanConfig.data.EraStateProcess < 100)
					{
						jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton1;
						jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("本团进度：") + this.m_JiYuanConfig.data.EraStateProcess + "%"
						});
					}
					else if (num2 == 0 || num2 == -1)
					{
						jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("本团进度：100%")
						});
						jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
						jiYuanZongLanItem.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
						{
							<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this.DPSelectedItem(<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this, new DPSelectedItemEventArgs
							{
								ID = <AddJiYuanZongLanPanel>c__AnonStorey.keyID
							});
						};
						KeyValuePair<int, EraReward> keyValuePair8 = enumerator.Current;
						this.M_OneLingQuKey = keyValuePair8.Value.Progress;
						this.m_LingQuCount++;
						this.DPSelectedGanTan(this, new DPSelectedItemEventArgs
						{
							ShowFlagUpdate = true
						});
					}
					else if (num2 == 2)
					{
						jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("本团进度：100%")
						});
						jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
						jiYuanZongLanItem.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
						{
							<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this.DPSelectedItem(<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this, new DPSelectedItemEventArgs
							{
								ID = <AddJiYuanZongLanPanel>c__AnonStorey.keyID
							});
						};
					}
					else if (num2 == 1)
					{
						jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + "YiLingQu.png";
						jiYuanZongLanItem.m_ShowBtn.transform.localScale = new Vector3(92f, 66f, 1f);
						jiYuanZongLanItem.m_Btn.Text = string.Empty;
						jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("本团进度：100%")
						});
						if (jiYuanZongLanItem.GetComponent<BoxCollider>() != null)
						{
							Object.Destroy(jiYuanZongLanItem.GetComponent<BoxCollider>());
						}
					}
				}
				else if (num2 == 0 || num2 == -1)
				{
					jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本团进度：100%")
					});
					jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
					jiYuanZongLanItem.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this.DPSelectedItem(<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this, new DPSelectedItemEventArgs
						{
							ID = <AddJiYuanZongLanPanel>c__AnonStorey.keyID
						});
					};
					KeyValuePair<int, EraReward> keyValuePair9 = enumerator.Current;
					this.M_OneLingQuKey = keyValuePair9.Value.Progress;
					this.m_LingQuCount++;
					this.DPSelectedGanTan(this, new DPSelectedItemEventArgs
					{
						ShowFlagUpdate = true
					});
				}
				else if (num2 == 2)
				{
					jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本团进度：100%")
					});
					jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
					jiYuanZongLanItem.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this.DPSelectedItem(<AddJiYuanZongLanPanel>c__AnonStorey.<>f__this, new DPSelectedItemEventArgs
						{
							ID = <AddJiYuanZongLanPanel>c__AnonStorey.keyID
						});
					};
				}
				else if (num2 == 1)
				{
					jiYuanZongLanItem.m_ShowBtn.URL = this.m_PathParent + "YiLingQu.png";
					jiYuanZongLanItem.m_ShowBtn.transform.localScale = new Vector3(92f, 66f, 1f);
					jiYuanZongLanItem.m_Btn.Text = string.Empty;
					jiYuanZongLanItem.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本团进度：100%")
					});
					if (jiYuanZongLanItem.GetComponent<BoxCollider>() != null)
					{
						Object.Destroy(jiYuanZongLanItem.GetComponent<BoxCollider>());
					}
				}
			}
			jiYuanZongLanItem.transform.SetParent(this.m_Listbox.transform, false);
			this.m_List.Add(jiYuanZongLanItem);
			if (jiYuanZongLanItem.GetComponent<UIPanel>() == null)
			{
				jiYuanZongLanItem.gameObject.AddComponent<UIPanel>();
			}
			if (jiYuanZongLanItem.m_Btn.GetComponent<UIDragPanelContents>() == null)
			{
				jiYuanZongLanItem.m_Btn.gameObject.AddComponent<UIDragPanelContents>();
			}
			jiYuanZongLanItem.m_Btn.gameObject.AddComponent<UIDragPanelContents>().draggablePanel = this.m_DraggablePanel;
		}
		this.RefreshList();
	}

	public void RefreshItem(int Progress, int value)
	{
		for (int i = 0; i < this.m_List.Count; i++)
		{
			if (this.m_List[i].Progress == Progress)
			{
				if (value == 0)
				{
					this.m_List[i].m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本团进度：100%")
					});
					this.m_List[i].m_ShowBtn.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.AwardButton2;
					this.m_List[i].m_SpSuo.gameObject.SetActive(false);
				}
				else if (value == 1)
				{
					this.m_List[i].m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本团进度：100%")
					});
					this.m_List[i].m_ShowBtn.URL = this.m_PathParent + "YiLingQu.png";
					this.m_List[i].m_Btn.Text = string.Empty;
					this.m_List[i].m_ShowBtn.transform.localScale = new Vector3(92f, 66f, 1f);
					this.m_List[i].m_SpSuo.gameObject.SetActive(false);
					if (this.m_List[i].GetComponent<BoxCollider>() != null)
					{
						Object.Destroy(this.m_List[i].GetComponent<BoxCollider>());
					}
					this.m_LingQuCount--;
				}
			}
		}
		if (this.m_LingQuCount > 0)
		{
			this.DPSelectedGanTan(this, new DPSelectedItemEventArgs
			{
				ShowFlagUpdate = true
			});
		}
		else
		{
			this.DPSelectedGanTan(this, new DPSelectedItemEventArgs
			{
				ShowFlagUpdate = false
			});
		}
	}

	private void RefreshList()
	{
		for (int i = 0; i < this.m_List.Count; i++)
		{
			float num = (float)i * this.m_ListWidth;
			float num2 = (float)(2 - this.m_JiYuanConfig.data.EraStage);
			if (this.M_OneLingQuKey != -1)
			{
				num2 = (float)(2 - this.M_OneLingQuKey);
			}
			if (num + this.m_ListWidth * num2 > (float)(this.m_List.Count - 1) * this.m_ListWidth)
			{
				this.m_List[i].transform.localPosition = new Vector3(num + this.m_ListWidth * num2 - (float)this.m_List.Count * this.m_ListWidth, 0f, 0f);
			}
			else if (num + this.m_ListWidth * num2 < 0f)
			{
				this.m_List[i].transform.localPosition = new Vector3(num + this.m_ListWidth * (float)this.m_List.Count + this.m_ListWidth * num2, 0f, 0f);
			}
			else
			{
				this.m_List[i].transform.localPosition = new Vector3(num + this.m_ListWidth * num2, 0f, 0f);
			}
			if (this.m_List[i].transform.localPosition.x == this.m_ListWidth)
			{
				this.m_List[i].transform.localPosition = new Vector3(this.m_ListWidth, 0f, -3f);
			}
			if (this.m_List[i].transform.localPosition.x != this.m_ListWidth)
			{
				this.m_List[i].transform.localScale = new Vector3(0.8f, 0.8f, 1f);
			}
			if (this.m_List[i].transform.localPosition.x == this.m_ListWidth * (float)(this.m_List.Count - 1))
			{
				this.m_List[i].transform.gameObject.SetActive(false);
			}
		}
	}

	private IEnumerator Enumerator()
	{
		yield return new WaitForSeconds(0.5f);
		this.m_Spring.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.m_Bool = true;
		yield break;
	}

	private void onDragFinished()
	{
		if (this.m_Bool)
		{
			if (this.m_Spring.transform.localPosition.x - this.m_Position < -80f)
			{
				this.MovePosition(-1);
				this.m_Bool = false;
				this.m_Position = 0f;
				base.StartCoroutine<bool>(this.Enumerator());
			}
			else if (this.m_Spring.transform.localPosition.x - this.m_Position > 80f)
			{
				this.MovePosition(1);
				this.m_Bool = false;
				this.m_Position = 0f;
				base.StartCoroutine<bool>(this.Enumerator());
			}
		}
	}

	private void MovePosition(int key = -1)
	{
		for (int i = 0; i < this.m_List.Count; i++)
		{
			this.m_List[i].gameObject.SetActive(true);
			this.m_List[i].m_TweenPosition.from = this.m_List[i].transform.localPosition;
			float num = this.m_List[i].transform.localPosition.x + (float)key * this.m_ListWidth;
			float z = this.m_List[i].transform.localPosition.z;
			this.m_List[i].m_TweenPosition.Reset();
			this.m_List[i].m_TweenScale.Reset();
			if (num > this.m_ListWidth * (float)(this.m_List.Count - 1))
			{
				this.m_List[i].m_TweenPosition.from = new Vector3(this.m_ListWidth * 2f, 0f, 0f);
				num = 0f;
			}
			else if (num < 0f)
			{
				num = this.m_ListWidth * (float)(this.m_List.Count - 1);
				this.m_List[i].m_TweenPosition.from = new Vector3(0f, 0f, 0f);
			}
			if (num == this.m_ListWidth)
			{
				this.m_List[i].m_TweenPosition.from = new Vector3(this.m_List[i].transform.localPosition.x, 0f, -3f);
				this.m_List[i].m_TweenPosition.to = new Vector3(num, 0f, -3f);
				this.m_List[i].m_TweenScale.from = new Vector3(0.8f, 0.8f, 1f);
				this.m_List[i].m_TweenScale.to = new Vector3(1f, 1f, 1f);
			}
			else
			{
				this.m_List[i].m_TweenPosition.to = new Vector3(num, 0f, 0f);
				this.m_List[i].m_TweenScale.from = new Vector3(1f, 1f, 1f);
				this.m_List[i].m_TweenScale.to = new Vector3(0.8f, 0.8f, 1f);
			}
			if (num >= this.m_ListWidth * (float)(this.m_List.Count - 1))
			{
				this.m_List[i].gameObject.SetActive(false);
				this.m_List[i].transform.localPosition = new Vector3(this.m_ListWidth * 3f, 0f, 0f);
			}
			if (num == this.m_ListWidth * (float)(this.m_List.Count - 2) && key == -1)
			{
				this.m_List[i].m_TweenPosition.from = new Vector3(0f, 0f, 0f);
			}
			this.m_List[i].m_TweenPosition.Play(true);
			this.m_List[i].m_TweenScale.Play(true);
		}
	}

	public SpringPanel m_Spring;

	public ShowNetImage m_ShowLogo;

	public ShowNetImage m_ShowDiTu;

	public UILabel m_LabTime;

	public UILabel m_LabContent;

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public ListBox m_Listbox;

	public UIDraggablePanel m_DraggablePanel;

	public JiYuanConfig m_JiYuanConfig;

	public string m_PathParent = "NetImages/GameRes/Images/JiYuanHuoDong/";

	public List<JiYuanZongLanItem> m_List = new List<JiYuanZongLanItem>();

	private float m_Position;

	public ObservableCollection m_ObservableCollection;

	public DPSelectedItemEventHandler DPSelectedItem;

	public DPSelectedItemEventHandler DPSelectedGanTan;

	private bool m_Bool = true;

	public int m_LingQuCount;

	private int M_OneLingQuKey = -1;

	private Dictionary<int, int> m_EraReward = new Dictionary<int, int>();

	private float m_ListWidth;
}
