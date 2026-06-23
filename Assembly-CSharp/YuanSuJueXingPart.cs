using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class YuanSuJueXingPart : UserControl
{
	public override void Destroy()
	{
		base.StopCoroutine("MovePosition");
		base.Destroy();
	}

	protected override void InitializeComponent()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_HandlerClose(this, new DPSelectedItemEventArgs());
		};
		base.InitializeComponent();
	}

	public void AddList(int goodid)
	{
		this.m_DraggablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.m_ItemRange = this.m_ListBox.cellWidth;
		this.m_Collection = this.m_ListBox.ItemsSource;
		for (int i = 0; i < this.m_ListVector.Length; i++)
		{
			if (this.m_OnItem == i)
			{
				this.m_ListVector[i] = new Vector3(this.m_ItemRange * (float)i - (float)((this.m_ListVector.Length - 1) / 2) * this.m_ItemRange, -15f, -1f);
			}
			else
			{
				this.m_ListVector[i] = new Vector3(this.m_ItemRange * (float)i - (float)((this.m_ListVector.Length - 1) / 2) * this.m_ItemRange, 0f, -0.5f);
			}
		}
		int[] array = new int[this.m_ListVector.Length];
		int num = -1;
		int num2 = 0;
		Dictionary<int, Dictionary<int, JingLingYuanSuShuXingVO>>.Enumerator enumerator = ConfigYuanSuJueXing.instance.DicJingLingYuanSuShuXingVO.GetEnumerator();
		int num3 = 0;
		while (enumerator.MoveNext())
		{
			YuanSuJueXingItem yuanSuJueXingItem = U3DUtils.NEW<YuanSuJueXingItem>();
			JingLingYuanSuShuXingVO jingLingYuanSuShuXingVO;
			if (Global.Data.roleData.JingLingYuanSuJueXingData != null)
			{
				ConfigYuanSuJueXing instance = ConfigYuanSuJueXing.instance;
				int[] activeIDs = Global.Data.roleData.JingLingYuanSuJueXingData.ActiveIDs;
				KeyValuePair<int, Dictionary<int, JingLingYuanSuShuXingVO>> keyValuePair = enumerator.Current;
				bool flag;
				jingLingYuanSuShuXingVO = instance.GetJingLingYuanSuShuXinKeyType(activeIDs, keyValuePair.Value[1].Tipe, out flag);
				if (flag)
				{
					yuanSuJueXingItem.LV = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						string.Format("Lv.{0}", jingLingYuanSuShuXingVO.MinLevel.ToString())
					});
				}
				else
				{
					yuanSuJueXingItem.LV = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("未激活")
					});
				}
				yuanSuJueXingItem.Name = jingLingYuanSuShuXingVO.Name;
				yuanSuJueXingItem.Content = jingLingYuanSuShuXingVO.tips;
				YuanSuJueXingItem yuanSuJueXingItem2 = yuanSuJueXingItem;
				KeyValuePair<int, Dictionary<int, JingLingYuanSuShuXingVO>> keyValuePair2 = enumerator.Current;
				yuanSuJueXingItem2.Type = keyValuePair2.Value[1].Tipe;
			}
			else
			{
				ConfigYuanSuJueXing instance2 = ConfigYuanSuJueXing.instance;
				KeyValuePair<int, Dictionary<int, JingLingYuanSuShuXingVO>> keyValuePair3 = enumerator.Current;
				jingLingYuanSuShuXingVO = instance2.GetJingLingYuanSuShuXingType(keyValuePair3.Value[1].Tipe)[1];
				yuanSuJueXingItem.Name = jingLingYuanSuShuXingVO.Name;
				yuanSuJueXingItem.Content = jingLingYuanSuShuXingVO.tips;
				yuanSuJueXingItem.LV = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("未激活")
				});
				YuanSuJueXingItem yuanSuJueXingItem3 = yuanSuJueXingItem;
				KeyValuePair<int, Dictionary<int, JingLingYuanSuShuXingVO>> keyValuePair4 = enumerator.Current;
				yuanSuJueXingItem3.Type = keyValuePair4.Value[1].Tipe;
			}
			if (goodid > 0 && ConfigYuanSuJueXing.instance.Goodid((YuanSuJueXingType)jingLingYuanSuShuXingVO.Tipe) == goodid)
			{
				num3 = num2;
			}
			num = Mathf.Max(num, jingLingYuanSuShuXingVO.MinLevel);
			array[num2] = jingLingYuanSuShuXingVO.MinLevel;
			yuanSuJueXingItem.m_Title.text = num2.ToString();
			yuanSuJueXingItem.transform.SetParent(this.m_ListBox.transform, false);
			yuanSuJueXingItem.ItemCount = num2;
			yuanSuJueXingItem.m_Handler = new DPSelectedItemEventHandler(this.OnItemHandel);
			yuanSuJueXingItem.m_HandlerClose = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.m_HandlerClose(this, new DPSelectedItemEventArgs());
			};
			if (yuanSuJueXingItem.GetComponent<UIDragPanelContents>() == null)
			{
				yuanSuJueXingItem.gameObject.AddComponent<UIDragPanelContents>();
			}
			yuanSuJueXingItem.GetComponent<UIDragPanelContents>().draggablePanel = this.m_DraggablePanel;
			yuanSuJueXingItem.transform.transform.localPosition = this.m_ListVector[num2];
			if (num2 == this.m_OnItem)
			{
				yuanSuJueXingItem.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
			}
			else
			{
				yuanSuJueXingItem.transform.localScale = Vector3.one;
			}
			this.m_List.Add(yuanSuJueXingItem);
			num2++;
		}
		if (goodid <= 0)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (num == array[j] && num > 0)
				{
					num3 = j;
				}
			}
		}
		base.StartCoroutine<bool>(this.MovePosition(this.m_OnItem - num3));
	}

	private void OnItemHandel(object s, DPSelectedItemEventArgs e)
	{
		if (this.m_Bool)
		{
			base.StartCoroutine<bool>(this.MovePosition(this.m_OnItem - e.Index));
		}
	}

	private void onDragFinished()
	{
		if (this.m_Bool)
		{
			if (this.m_Spring.transform.localPosition.x < -(this.m_ItemRange / 2f))
			{
				base.StartCoroutine<bool>(this.MovePosition(-1));
			}
			else if (this.m_Spring.transform.localPosition.x > this.m_ItemRange / 2f)
			{
				base.StartCoroutine<bool>(this.MovePosition(1));
			}
		}
	}

	private IEnumerator MovePosition(int key)
	{
		if (key > 1)
		{
			key = -1;
		}
		else if (key < -1)
		{
			key = 1;
		}
		float duration = 0.2f;
		Vector3[] list = new Vector3[this.m_ListVector.Length];
		this.m_OnItem -= key;
		if (this.m_OnItem > this.m_ListVector.Length - 1)
		{
			this.m_OnItem = 0;
		}
		else if (this.m_OnItem < 0)
		{
			this.m_OnItem = this.m_ListVector.Length - 1;
		}
		for (int i = 0; i < list.Length; i++)
		{
			if (i + key > this.m_ListVector.Length - 1)
			{
				list[i] = this.m_ListVector[0];
			}
			else if (i + key < 0)
			{
				list[i] = this.m_ListVector[this.m_ListVector.Length - 1];
			}
			else
			{
				list[i] = this.m_ListVector[i + key];
			}
		}
		for (int j = 0; j < this.m_List.Count; j++)
		{
			if (this.m_StarPanelBool)
			{
				this.m_List[j].transform.localPosition = list[j];
				if (j == this.m_OnItem)
				{
					this.m_List[j].transform.localScale = new Vector3(1.15f, 1.15f, 1f);
					this.m_List[j].OnItemClick = true;
				}
				else
				{
					this.m_List[j].transform.localScale = Vector3.one;
					this.m_List[j].OnItemClick = false;
				}
			}
			else
			{
				this.m_List[j].m_TweenPosition.from = this.m_ListVector[j];
				this.m_List[j].m_TweenPosition.to = list[j];
				if (j == this.m_OnItem)
				{
					this.m_List[j].m_TweenScale.from = Vector3.one;
					this.m_List[j].m_TweenScale.to = new Vector3(1.15f, 1.15f, 1f);
					this.m_List[j].OnItemClick = true;
				}
				else
				{
					this.m_List[j].m_TweenScale.from = new Vector3(1.15f, 1.15f, 1f);
					this.m_List[j].m_TweenScale.to = Vector3.one;
					this.m_List[j].OnItemClick = false;
				}
				this.m_List[j].m_TweenPosition.Reset();
				this.m_List[j].m_TweenScale.Reset();
				this.m_List[j].Duration = duration;
				this.m_List[j].m_TweenPosition.Play(true);
				this.m_List[j].m_TweenScale.Play(true);
			}
		}
		this.m_StarPanelBool = false;
		this.m_ListVector.Clone();
		this.m_ListVector = list;
		this.m_Bool = false;
		yield return new WaitForSeconds(duration);
		this.m_Bool = true;
		this.m_DraggablePanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		yield break;
	}

	[SerializeField]
	private GButton m_BtnClose;

	[SerializeField]
	private ListBox m_ListBox;

	[SerializeField]
	private UIDraggablePanel m_DraggablePanel;

	[SerializeField]
	private SpringPanel m_Spring;

	private List<YuanSuJueXingItem> m_List = new List<YuanSuJueXingItem>();

	private Vector3[] m_ListVector = new Vector3[3];

	private float m_ItemRange = 200f;

	private int m_OnItem = 1;

	private bool m_Bool = true;

	private bool m_StarPanelBool = true;

	private ObservableCollection m_Collection;

	public DPSelectedItemEventHandler m_HandlerClose;
}
