using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;

public class ChatBoxFriendPart : UserControl
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

	protected override void InitializeComponent()
	{
		UIDraggablePanel component = this.dragPanel.GetComponent<UIDraggablePanel>();
		component.onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.m_soltWidth = (int)this.dragPanel.GetComponent<UIPanel>().clipRange.z;
		if (Global.Data.FriendDataList != null && null != this.listBox)
		{
			this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
			this.ItemCollection = this.listBox.ItemsSource;
			this.ItemCollection.Clear();
			for (int i = 0; i < Global.Data.FriendDataList.Count; i++)
			{
				if (Global.Data.FriendDataList[i].FriendType == 0)
				{
					ChatBoxFriendItem chatBoxFriendItem = U3DUtils.NEW<ChatBoxFriendItem>();
					chatBoxFriendItem._FriendFcae.spriteName = Global.CalcOriginalOccupationID(Global.Data.FriendDataList[i].Occupation) + "0_0";
					chatBoxFriendItem.LblFriendName.text = string.Concat(new string[]
					{
						"LV",
						Global.Data.FriendDataList[i].OtherLevel.ToString(),
						"[",
						Global.Data.FriendDataList[i].FriendChangeLifeLev.ToString(),
						Global.GetLang("转]")
					});
					chatBoxFriendItem.FriendName = Global.Data.FriendDataList[i].OtherRoleName;
					chatBoxFriendItem.LblFriendLevel.text = Global.Data.FriendDataList[i].OtherRoleName;
					this.ItemCollection.AddNoUpdate(chatBoxFriendItem);
				}
			}
			this.ItemCollection.DelayUpdate();
			if (this.ItemCollection.Count <= 0)
			{
				this.m_pageNum = 1;
			}
			else
			{
				this.m_pageNum = (this.ItemCollection.Count - 1) / 5 + 1;
			}
			if (this.m_pageNum == 1)
			{
				component.restrictWithinPanel = true;
			}
			else
			{
				component.restrictWithinPanel = false;
			}
			this.pageController.SetPageNum(this.m_pageNum, 0);
		}
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		if (this.DPSelectedItem != null)
		{
			ChatBoxFriendItem chatBoxFriendItem = U3DUtils.AS<ChatBoxFriendItem>(this.listBox.SelectedItem);
			if (null != chatBoxFriendItem)
			{
				this.m_strSelectedFriendName = chatBoxFriendItem.FriendName;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 3,
					Title = this.m_strSelectedFriendName
				});
			}
		}
	}

	private void onDragFinished()
	{
		if (this.m_pageNum == 1)
		{
			return;
		}
		if (Math.Abs(Math.Abs(this.dragPanel.transform.localPosition.x) - (float)(this.m_soltWidth * this.m_currentSelectedPage)) > (float)this.pageOffSize)
		{
			if (this.dragPanel.transform.localPosition.x > (float)((0 - this.m_soltWidth) * this.m_currentSelectedPage))
			{
				this.m_currentSelectedPage--;
				if (this.m_currentSelectedPage <= 0)
				{
					this.m_currentSelectedPage = 0;
				}
			}
			else
			{
				this.m_currentSelectedPage++;
				if (this.m_currentSelectedPage >= this.m_pageNum)
				{
					this.m_currentSelectedPage = this.m_pageNum - 1;
				}
			}
		}
		this.dragPanel.target.x = (float)((0 - this.m_soltWidth) * this.m_currentSelectedPage);
		this.dragPanel.enabled = true;
		this.pageController.SetCurrentPage(this.m_currentSelectedPage);
	}

	private void ItemSelected()
	{
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public string m_strSelectedFriendName = string.Empty;

	public ListBox listBox = new ListBox();

	public SpringPanel dragPanel;

	public ChatBoxPageController pageController;

	public int pageOffSize = 100;

	private int m_currentSelectedPage;

	private int m_pageNum = 1;

	private int m_soltWidth = 820;

	private ObservableCollection _ItemCollection;
}
