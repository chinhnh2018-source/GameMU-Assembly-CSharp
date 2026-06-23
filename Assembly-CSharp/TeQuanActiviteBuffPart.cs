using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeQuanActiviteBuffPart : UserControl, BaseTeQuanActivityPart
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private TeQuanJianZengJiaChengItem GetItem(int Index)
	{
		TeQuanJianZengJiaChengItem teQuanJianZengJiaChengItem = null;
		GameObject at = this.mViewObc.GetAt(Index);
		if (null != at)
		{
			teQuanJianZengJiaChengItem = at.GetComponent<TeQuanJianZengJiaChengItem>();
			if (null == teQuanJianZengJiaChengItem)
			{
				this.mViewObc.RemoveAt(Index);
			}
			else
			{
				teQuanJianZengJiaChengItem.gameObject.SetActive(true);
			}
		}
		if (null == teQuanJianZengJiaChengItem)
		{
			teQuanJianZengJiaChengItem = U3DUtils.NEW<TeQuanJianZengJiaChengItem>();
			this.mViewObc.AddNoUpdate(teQuanJianZengJiaChengItem);
		}
		return teQuanJianZengJiaChengItem;
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mViewObc = this._ViewListBox.ItemsSource;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshPart(SpecPriorityActInfo inf)
	{
		for (int i = 0; i < this.mViewObc.Count; i++)
		{
			GameObject at = this.mViewObc.GetAt(i);
			if (null != at)
			{
				at.SetActive(false);
			}
		}
		if (inf != null)
		{
			BetterList<TeQuanBuffVO> teQuanBuffVOsByTeQuanId = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanBuffVOsByTeQuanId(inf.TeQuanID);
			if (teQuanBuffVOsByTeQuanId != null && 0 < teQuanBuffVOsByTeQuanId.size)
			{
				for (int j = 0; j < teQuanBuffVOsByTeQuanId.size; j++)
				{
					if (teQuanBuffVOsByTeQuanId[j] != null)
					{
						TeQuanJianZengJiaChengItem item = this.GetItem(j);
						if (null != item)
						{
							item.SetData(teQuanBuffVOsByTeQuanId[j]);
							item.Hander = new DPSelectedItemEventHandler(this.ItemClickCallBack);
						}
					}
				}
				this._ViewListBox.repositionNow = true;
				SpringPanel.Begin(this._ViewDragpanel.gameObject, new Vector3(0f, 140f, 0f), 20f);
			}
		}
	}

	private void ItemClickCallBack(object sender, DPSelectedItemEventArgs args)
	{
		if (this.Hander != null)
		{
			this.Hander(null, new DPSelectedItemEventArgs
			{
				Type = 0
			});
		}
	}

	public int ID { get; set; }

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel _MiaoShuLabel;

	[SerializeField]
	private UIDraggablePanel _ViewDragpanel;

	[SerializeField]
	private ListBox _ViewListBox;

	private ObservableCollection mViewObc;
}
