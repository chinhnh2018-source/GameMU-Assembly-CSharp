using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenXiangContentPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public void ResetPanel()
	{
		this.m_Panel.transform.localPosition = Vector3.zero;
		this.m_Panel.clipRange = this.origionalRangeClip;
	}

	protected override void InitializeComponent()
	{
		this.origionalPosition = this.m_Panel.transform.localPosition;
		this.origionalRangeClip = this.m_Panel.clipRange;
		this.InitTextInPrefabs();
		this.ItemCollection = this.m_ListBox.ItemsSource;
		this.m_ShuXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_ShenXiangItemList.Count > 0)
			{
				Dictionary<string, float> dictionary = new Dictionary<string, float>();
				for (int i = 0; i < this.m_ShenXiangItemList.Count; i++)
				{
					if (this.m_ShenXiangItemList[i].isOpen)
					{
						string[] array = this.m_ShenXiangItemList[i].m_data.ActivationProperty.Split(new char[]
						{
							'|'
						});
						for (int j = 0; j < array.Length; j++)
						{
							string[] array2 = array[j].Split(new char[]
							{
								','
							});
							string text = array2[0];
							float num = float.Parse(array2[1]);
							if (dictionary.ContainsKey(text))
							{
								Dictionary<string, float> dictionary3;
								Dictionary<string, float> dictionary2 = dictionary3 = dictionary;
								string text3;
								string text2 = text3 = text;
								float num2 = dictionary3[text3];
								dictionary2[text2] = num2 + num;
							}
							else
							{
								dictionary.Add(text, num);
							}
						}
					}
				}
				if (dictionary.Count > 0)
				{
					Dictionary<string, float>.Enumerator enumerator = dictionary.GetEnumerator();
					StringBuilder stringBuilder = new StringBuilder();
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, float> keyValuePair = enumerator.Current;
						string extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair.Key, false);
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							extPropIndexesDescriptionByWord
						}));
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("：")
						}));
						string text4 = "{0:0%}";
						KeyValuePair<string, float> keyValuePair2 = enumerator.Current;
						string text5 = string.Format(text4, keyValuePair2.Value);
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							"+" + text5
						}));
						stringBuilder.Append('\n');
					}
					this.OpenShenQiPropertyPart(Global.GetLang("神像属性总览"), 0, stringBuilder.ToString());
				}
				else
				{
					Super.HintMainText(Global.GetLang("暂无属性总览"), 10, 3);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("暂无属性总览"), 10, 3);
			}
		};
	}

	private void InitTextInPrefabs()
	{
	}

	public void InitValue()
	{
		this.ItemCollection.Clear();
		this.m_ShenXiangItemList.Clear();
		int count = ShenXiangManager.GetShenXiangDataList().Count;
		Dictionary<int, ShenQiXMLData> currentShenQiDataDict = ShenQiManager.GetCurrentShenQiDataDict();
		Dictionary<int, ShenQiXMLData>.Enumerator enumerator = currentShenQiDataDict.GetEnumerator();
		for (int i = 0; i < count; i++)
		{
			ShenXiangItem shenXiangItem = U3DUtils.NEW<ShenXiangItem>();
			ShenXiangData data = ShenXiangManager.GetShenXiangDataList()[i];
			string[] array = data.OpenCondition.Split(new char[]
			{
				'|'
			});
			int num = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (currentShenQiDataDict.Count > 0)
				{
					if (currentShenQiDataDict.ContainsKey(ConvertExt.SafeConvertToInt32(array[j])))
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<int, ShenQiXMLData> keyValuePair = enumerator.Current;
							if (keyValuePair.Key == ConvertExt.SafeConvertToInt32(array[j]))
							{
								KeyValuePair<int, ShenQiXMLData> keyValuePair2 = enumerator.Current;
								if (this.IsManJi(keyValuePair2.Key))
								{
									num++;
									break;
								}
							}
						}
					}
				}
				else
				{
					shenXiangItem.isOpen = false;
				}
			}
			if (num >= array.Length)
			{
				shenXiangItem.isOpen = true;
			}
			else
			{
				shenXiangItem.isOpen = false;
			}
			shenXiangItem.InitValue(data, num);
			UIPanel component = shenXiangItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.m_ListBox.gameObject, shenXiangItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(shenXiangItem);
			this.m_ShenXiangItemList.Add(shenXiangItem);
		}
	}

	private bool IsManJi(int id)
	{
		ShenQiXMLData currentShenQiDataByID = ShenQiManager.GetCurrentShenQiDataByID(id);
		float num = (float)(currentShenQiDataByID.LifeV + currentShenQiDataByID.AddDefense + currentShenQiDataByID.Toughness + currentShenQiDataByID.AddAttack);
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(id);
		float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
		return num >= num2;
	}

	public void OpenShenQiPropertyPart(string titleName, int count, string content)
	{
		if (null == this.shenQiPropertyWindow)
		{
			this.shenQiPropertyWindow = U3DUtils.NEW<GChildWindow>();
			this.shenQiPropertyWindow.ModalType = ChildWindowModalType.Translucent;
			this.shenQiPropertyWindow.IsShowModal = true;
			Super.InitChildWindow(this.shenQiPropertyWindow, Global.GetLang("ShenXiangSumWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.shenQiPropertyWindow);
		}
		if (null == this.shenQiPropertyPart)
		{
			this.shenQiPropertyPart = U3DUtils.NEW<ShenXiangSumProperty>();
			this.shenQiPropertyPart.Show(titleName, count, content);
			this.shenQiPropertyWindow.Body.Add(this.shenQiPropertyPart);
			this.shenQiPropertyPart.CloseCallback = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.IDType == 0)
				{
					this.CloseShenQiPropertyPart();
				}
			};
		}
	}

	private void CloseShenQiPropertyPart()
	{
		if (null != this.shenQiPropertyWindow)
		{
			Object.Destroy(this.shenQiPropertyPart);
			this.shenQiPropertyPart = null;
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.shenQiPropertyWindow);
			this.shenQiPropertyWindow = null;
		}
	}

	protected override void OnDestroy()
	{
		this.m_ShenXiangItemList.Clear();
		this.m_ShuXing = null;
		this.m_LeftArrow = null;
		this.m_RightArrow = null;
		this.m_Panel = null;
		this.m_ListBox = null;
		ShenXiangManager.Clear();
	}

	public GButton m_ShuXing;

	public GButton m_LeftArrow;

	public GButton m_RightArrow;

	public UIPanel m_Panel;

	public ListBox m_ListBox;

	private ObservableCollection _ItemCollection;

	private Vector3 origionalPosition = Vector3.zero;

	private Vector4 origionalRangeClip;

	private List<ShenXiangItem> m_ShenXiangItemList = new List<ShenXiangItem>();

	public GChildWindow shenQiPropertyWindow;

	public ShenXiangSumProperty shenQiPropertyPart;
}
