using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class JiYuanHuoDongRenWuPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Content.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("帮助森林中被袭击的精灵村庄找回被夺走的祭品。")
		});
		this.m_ObservableCollection = this.m_ListBox.ItemsSource;
		this.AddList();
	}

	public void AddList()
	{
		this.m_Ttitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			this.m_JiYuanConfig.data.EraStage.ToString()
		});
		if (this.m_ObservableCollection == null)
		{
			return;
		}
		this.m_Content.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang(this.m_JiYuanConfig.ListEraTask[0].StageDescription)
		});
		this.m_ObservableCollection.Clear();
		for (int i = 0; i < this.m_JiYuanConfig.ListEraTask.Count; i++)
		{
			JiYuanRenWuItem jiYuanRenWuItem = U3DUtils.NEW<JiYuanRenWuItem>();
			jiYuanRenWuItem.Id = this.m_JiYuanConfig.ListEraTask[i].ID;
			jiYuanRenWuItem.SetTitle = this.m_JiYuanConfig.ListEraTask[i].TaskName;
			jiYuanRenWuItem.SetContent = this.m_JiYuanConfig.ListEraTask[i].Description;
			jiYuanRenWuItem.JiYuanKey = this.m_JiYuanConfig.ListEraTask[i].EraStage;
			jiYuanRenWuItem.SetJinDu = this.m_JiYuanConfig.ListEraTask[i].Reward.SafeToInt32(0);
			jiYuanRenWuItem.m_SpYiWanCheng.gameObject.SetActive(false);
			string text = string.Empty;
			for (int j = 0; j < this.m_JiYuanConfig.ListEraTask[i].CompletionCondition.Split(new char[]
			{
				'|'
			}).Length; j++)
			{
				string text2 = string.Empty;
				if (j >= this.m_JiYuanConfig.ListEraTask[i].CompletionCondition.Split(new char[]
				{
					'|'
				}).Length - 1)
				{
					text2 = this.m_JiYuanConfig.ListEraTask[i].CompletionCondition.Split(new char[]
					{
						'|'
					})[j] + ",1,0,0,0,0";
				}
				else
				{
					text2 = this.m_JiYuanConfig.ListEraTask[i].CompletionCondition.Split(new char[]
					{
						'|'
					})[j] + ",1,0,0,0,0|";
				}
				text += text2;
			}
			if (!string.IsNullOrEmpty(text))
			{
				jiYuanRenWuItem.m_ObservableCollection = jiYuanRenWuItem.m_List.ItemsSource;
				Super.LoadGoodsList(text, jiYuanRenWuItem.m_ObservableCollection);
			}
			UIPanel[] componentsInChildren = jiYuanRenWuItem.m_List.GetComponentsInChildren<UIPanel>();
			if (componentsInChildren != null)
			{
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					Object.Destroy(componentsInChildren[k]);
					if (k >= 3)
					{
						Object.Destroy(componentsInChildren[k].gameObject);
					}
				}
			}
			GGoodIcon[] GGoodIcons = jiYuanRenWuItem.m_List.GetComponentsInChildren<GGoodIcon>();
			if (GGoodIcons != null)
			{
				for (int l = 0; l < GGoodIcons.Length; l++)
				{
					int key = l;
					GGoodIcons[key].addEventListener("click", delegate(MouseEvent s)
					{
						GGoodIcon ggoodIcon = s.target.SafeGetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							return;
						}
						GoodsData goodsData = GGoodIcons[key].ItemObject as GoodsData;
						if (goodsData == null)
						{
							return;
						}
						GTipServiceEx.ShowTip(GGoodIcons[key], TipTypes.GoodsText, GoodsOwnerTypes.JiYuanShouJi, goodsData);
					});
				}
				string[] array = text.Split(new char[]
				{
					'|'
				});
				int[] array2 = new int[array.Length];
				for (int m = 0; m < array.Length; m++)
				{
					array2[m] = array[m].Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
				}
				if (this.m_JiYuanConfig.data.EraTaskList == null || this.m_JiYuanConfig.data.EraTaskList.Count <= 0)
				{
					GGoodIcons[0].ContentText.gameObject.SetActive(false);
					GGoodIcons[1].ContentText.gameObject.SetActive(false);
					GGoodIcons[2].ContentText.gameObject.SetActive(false);
					GGoodIcons[0].SecondText.Text = "0/" + array2[0];
					GGoodIcons[1].SecondText.Text = "0/" + array2[1];
					GGoodIcons[2].SecondText.Text = "0/" + array2[2];
				}
				else
				{
					bool flag = true;
					for (int n = 0; n < this.m_JiYuanConfig.data.EraTaskList.Count; n++)
					{
						if (this.m_JiYuanConfig.data.EraTaskList[n].TaskID == this.m_JiYuanConfig.ListEraTask[i].ID)
						{
							if (GGoodIcons.Length == 3)
							{
								GGoodIcons[0].ContentText.gameObject.SetActive(false);
								GGoodIcons[1].ContentText.gameObject.SetActive(false);
								GGoodIcons[2].ContentText.gameObject.SetActive(false);
								GGoodIcons[0].SecondText.Text = this.m_JiYuanConfig.data.EraTaskList[n].TaskVal1 + "/" + array2[0];
								GGoodIcons[1].SecondText.Text = this.m_JiYuanConfig.data.EraTaskList[n].TaskVal2 + "/" + array2[1];
								GGoodIcons[2].SecondText.Text = this.m_JiYuanConfig.data.EraTaskList[n].TaskVal3 + "/" + array2[2];
								if (this.m_JiYuanConfig.data.EraTaskList[n].TaskVal1 >= array2[0] && this.m_JiYuanConfig.data.EraTaskList[n].TaskVal2 >= array2[1] && this.m_JiYuanConfig.data.EraTaskList[n].TaskVal3 >= array2[2])
								{
									jiYuanRenWuItem.m_SpYiWanCheng.gameObject.SetActive(true);
								}
								flag = false;
							}
						}
						if (flag && n >= this.m_JiYuanConfig.data.EraTaskList.Count - 1)
						{
							GGoodIcons[0].ContentText.gameObject.SetActive(false);
							GGoodIcons[1].ContentText.gameObject.SetActive(false);
							GGoodIcons[2].ContentText.gameObject.SetActive(false);
							GGoodIcons[0].SecondText.Text = "0/" + array2[0];
							GGoodIcons[1].SecondText.Text = "0/" + array2[1];
							GGoodIcons[2].SecondText.Text = "0/" + array2[2];
						}
					}
				}
			}
			this.m_ObservableCollection.AddNoUpdate(jiYuanRenWuItem);
		}
		for (int num = 0; num < this.m_ObservableCollection.Count; num++)
		{
			UIPanel component = this.m_ObservableCollection.GetAt(num).GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
	}

	public UILabel m_Ttitle;

	public UILabel m_Content;

	public ListBox m_ListBox;

	public GButton m_BtnLeft;

	public GButton m_BtnRight;

	public JiYuanConfig m_JiYuanConfig;

	public string m_PathParent = "NetImages/GameRes/Images/JiYuanHuoDong/";

	private ObservableCollection m_ObservableCollection;
}
