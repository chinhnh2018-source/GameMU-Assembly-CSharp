using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JingLingSkillbuffHint : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
	}

	public override void Update()
	{
		base.Update();
		if (0 >= this.m_ListBox.transform.childCount)
		{
			if (this.m_DeleteTime > 0f)
			{
				this.m_DeleteTime -= Time.deltaTime;
			}
			else
			{
				this.hander(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		}
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_Collection = this.m_ListBox.ItemsSource;
	}

	public void AddSkill(int skillID)
	{
		if (Global.GetJingLingSkillSignURL(skillID).IndexOf("-1.png") >= 0 || Global.GetJingLingSkillSignURL(skillID).IndexOf("NoImage") >= 0)
		{
			return;
		}
		JingLingSkillbuffItem jingLingSkillbuffItem = U3DUtils.NEW<JingLingSkillbuffItem>();
		jingLingSkillbuffItem.Init(skillID);
		jingLingSkillbuffItem.hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			this.m_ListBox.repositionNow = true;
		};
		this.m_DeleteTime = 10f;
		this.m_Collection.Insert(this.m_Collection.Count, jingLingSkillbuffItem);
		this.m_ListBox.repositionNow = true;
	}

	public ListBox m_ListBox;

	public MyUIAnchor m_MyUIAnchor;

	private ObservableCollection m_Collection;

	private float m_DeleteTime;

	public DPSelectedItemEventHandler hander;
}
