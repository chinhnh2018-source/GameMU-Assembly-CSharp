using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AutoSkillPriorityPart : UserControl
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

	private void InitTextInPrefabs()
	{
		this.m_Title.Text = Global.GetLang("技能优先级设置");
		this.m_ShuoMing.Text = Global.GetLang("设置在越靠左的槽位，技能的施放优先级越高");
		this.m_ShuoMing.Pivot = 0;
		this.m_ShuoMing.X = -218.0;
		this.m_ShuoMing.Y = -50.0;
		this.m_ShuoMing.MaxWidth = 440.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitButtonEvent();
		this.ItemCollection = this.m_ListBox.ItemsSource;
	}

	private void InitButtonEvent()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.refreshSkillDictCallBack != null)
			{
				this.refreshSkillDictCallBack(this.m_SkillPriorityDict);
			}
			if (this.CloseHander != null)
			{
				this.CloseHander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void InitPart(bool isCheck, Dictionary<int, int> _tmpSkillPriorityDict)
	{
		this.m_SkillPriorityDict = _tmpSkillPriorityDict;
		this.ItemCollection.Clear();
		Dictionary<int, int>.Enumerator enumerator = this.m_SkillPriorityDict.GetEnumerator();
		for (int i = 0; i < this.skillPriorityCount; i++)
		{
			int skillID = 0;
			int skillIndex = i + 1;
			AutoSkillPriorityItem autoSkillPriorityItem = U3DUtils.NEW<AutoSkillPriorityItem>();
			autoSkillPriorityItem.InitIndex(isCheck, skillIndex, skillID);
			autoSkillPriorityItem.RefreshSkillList = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ResetCurrentSkillPriorityCallBack(e.ID, e.Index);
			};
			UIPanel component = autoSkillPriorityItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.m_ListBox.gameObject, autoSkillPriorityItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(autoSkillPriorityItem);
			this.autoSkillPriorityItems.Add(autoSkillPriorityItem);
		}
		while (enumerator.MoveNext())
		{
			for (int j = 0; j < this.autoSkillPriorityItems.Count; j++)
			{
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				if (keyValuePair.Key == this.autoSkillPriorityItems[j].m_Index)
				{
					AutoSkillPriorityItem autoSkillPriorityItem2 = this.autoSkillPriorityItems[j];
					KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
					autoSkillPriorityItem2.InitSkillIcon(keyValuePair2.Value);
				}
			}
		}
	}

	private void ResetCurrentSkillPriorityCallBack(int skillID, int skillIndex)
	{
		int num = 0;
		if (this.m_SkillPriorityDict.ContainsValue(skillID))
		{
			foreach (KeyValuePair<int, int> keyValuePair in this.m_SkillPriorityDict)
			{
				if (keyValuePair.Value == skillID)
				{
					Dictionary<int, int>.Enumerator enumerator;
					KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
					num = keyValuePair2.Key;
				}
			}
		}
		if (num > 0)
		{
			this.m_SkillPriorityDict[num] = 0;
			this.m_SkillPriorityDict.Remove(num);
		}
		if (this.m_SkillPriorityDict.ContainsKey(skillIndex))
		{
			this.m_SkillPriorityDict[skillIndex] = skillID;
		}
		else
		{
			this.m_SkillPriorityDict.Add(skillIndex, skillID);
		}
		int count = this.autoSkillPriorityItems.Count;
		for (int i = 0; i < count; i++)
		{
			AutoSkillPriorityItem autoSkillPriorityItem = this.autoSkillPriorityItems[i];
			if (autoSkillPriorityItem.m_SkillID != skillID && autoSkillPriorityItem.m_Index == skillIndex)
			{
				autoSkillPriorityItem.RefreshSkillIcon(true, autoSkillPriorityItem.m_Index, skillID);
			}
			else if (autoSkillPriorityItem.m_SkillID == skillID && autoSkillPriorityItem.m_Index != skillIndex)
			{
				autoSkillPriorityItem.RefreshSkillIcon(false, autoSkillPriorityItem.m_Index, 0);
			}
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, int> keyValuePair3 in this.m_SkillPriorityDict)
		{
			if (keyValuePair3.Value == 0)
			{
				List<int> list2 = list;
				Dictionary<int, int>.Enumerator enumerator2;
				KeyValuePair<int, int> keyValuePair4 = enumerator2.Current;
				list2.Add(keyValuePair4.Key);
			}
		}
		if (list.Count > 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				this.m_SkillPriorityDict.Remove(list[j]);
			}
		}
	}

	public TextBlock m_Title;

	public GButton m_CloseBtn;

	public ListBox m_ListBox;

	public TextBlock m_ShuoMing;

	public DPSelectedItemEventHandler CloseHander;

	public AutoSkillPriorityPart.RefreshSkillDictCallBack refreshSkillDictCallBack;

	private int skillPriorityCount = 4;

	private Dictionary<int, int> m_SkillPriorityDict;

	private List<AutoSkillPriorityItem> autoSkillPriorityItems = new List<AutoSkillPriorityItem>();

	private ObservableCollection _ItemCollection;

	public delegate void RefreshSkillDictCallBack(Dictionary<int, int> tmpDict);
}
