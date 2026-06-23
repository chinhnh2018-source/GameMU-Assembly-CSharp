using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class FamilyRequestPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnAgreeAll.Text = Global.GetLang("全部同意");
		this.btnRefuseAll.Text = Global.GetLang("全部拒绝");
	}

	protected override void InitializeComponent()
	{
		this.Root = this.Container;
		this.thisCtrl = this;
		this.InitTextInPrefabs();
		this.ItemCollection = this.lbPlayers.ItemsSource;
		this.btnAgreeAll.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.AgreeAllRequest();
		};
		this.btnRefuseAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RefuseAllRequest();
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 10
				});
			}
		};
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

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

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		this.ProcessBangHuiUIItemQueue();
	}

	private void ShowRequest(BangHuiUIItem bangHuiUIItem, int bangHuiCmd)
	{
		FamilyRequestItem familyRequestItem = U3DUtils.NEW<FamilyRequestItem>();
		familyRequestItem.ID = this.RequestID++;
		familyRequestItem.CurrentBangHuiUIItem = bangHuiUIItem;
		familyRequestItem.InitButtons();
		familyRequestItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.RemoveItemByID(e.ID);
		};
		this.ItemCollection.AddNoUpdate(familyRequestItem);
	}

	private void RemoveItemByID(int id)
	{
		BangHuiUIItem bangHuiUIItem = null;
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			if (U3DUtils.AS<FamilyRequestItem>(this.ItemCollection[i]).ID == id)
			{
				bangHuiUIItem = U3DUtils.AS<FamilyRequestItem>(this.ItemCollection[i]).CurrentBangHuiUIItem;
				this.ItemCollection.RemoveAt(i);
				break;
			}
		}
		if (bangHuiUIItem != null)
		{
			int num = -1;
			List<BangHuiUIItem> bangHuiUIItemQueue = Super.GData.BangHuiUIItemQueue;
			for (int j = 0; j < bangHuiUIItemQueue.Count; j++)
			{
				if (bangHuiUIItemQueue[j].OtherRoleID == bangHuiUIItem.OtherRoleID)
				{
					num = j;
					break;
				}
			}
			if (num != -1)
			{
				Super.GData.BangHuiUIItemQueue.RemoveAt(num);
			}
		}
	}

	public int GetRequestCount()
	{
		return this.ItemCollection.Count;
	}

	public void AgreeAllRequest()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			BangHuiUIItem currentBangHuiUIItem = U3DUtils.AS<FamilyRequestItem>(this.ItemCollection[i]).CurrentBangHuiUIItem;
			if (currentBangHuiUIItem.BangHuiCmd != 1)
			{
				GameInstance.Game.SpriteAgreeToBHMember(currentBangHuiUIItem.OtherRoleID, currentBangHuiUIItem.BHID, currentBangHuiUIItem.BHName, 1);
				break;
			}
			GameInstance.Game.SpriteAddBHMember(currentBangHuiUIItem.BHID, currentBangHuiUIItem.OtherRoleID, currentBangHuiUIItem.OtherRoleName, 0);
		}
		this.ItemCollection.Clear();
		Super.GData.BangHuiUIItemQueue.Clear();
	}

	public void RefuseAllRequest()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			BangHuiUIItem currentBangHuiUIItem = U3DUtils.AS<FamilyRequestItem>(this.ItemCollection[i]).CurrentBangHuiUIItem;
			if (currentBangHuiUIItem.BangHuiCmd == 1)
			{
				GameInstance.Game.SpriteRefuseApplyToBHMember(currentBangHuiUIItem.BHID, currentBangHuiUIItem.OtherRoleID);
			}
			else
			{
				GameInstance.Game.SpriteAgreeToBHMember(currentBangHuiUIItem.OtherRoleID, currentBangHuiUIItem.BHID, currentBangHuiUIItem.BHName, 0);
			}
		}
		this.ItemCollection.Clear();
		Super.GData.BangHuiUIItemQueue.Clear();
	}

	private void ProcessBangHuiUIItemQueue()
	{
		if (Super.GData.BangHuiUIItemQueue == null)
		{
			return;
		}
		for (int i = 0; i < Super.GData.BangHuiUIItemQueue.Count; i++)
		{
			BangHuiUIItem bangHuiUIItem = Super.GData.BangHuiUIItemQueue[i];
			this.ShowRequest(bangHuiUIItem, bangHuiUIItem.BangHuiCmd);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void lbPlayers_SelectionChanged(object sender, MouseEvent e)
	{
		if (null != this.CurrentSelectedItem)
		{
			this.CurrentSelectedItem.BodyBackground = null;
		}
		FamilyRequestItem familyRequestItem = U3DUtils.AS<FamilyRequestItem>(this.lbPlayers.SelectedItem);
		if (null == familyRequestItem)
		{
			return;
		}
		familyRequestItem.BodyHeight = 20.0;
		familyRequestItem.BodyWidth = 251.0;
		familyRequestItem.BodyBackground = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetGameResImage("Images/Plate/listItem_active.png"), 251.0, 20.0, 5.0, 5.0));
		this.CurrentSelectedItem = familyRequestItem;
	}

	public GButton btnAgreeAll;

	public GButton btnRefuseAll;

	public GButton btnClose;

	private FamilyRequestItem CurrentSelectedItem;

	private int RequestID;

	private Canvas Root;

	public ListBox lbPlayers;

	private SpriteSL thisCtrl;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler DPSelectedItem;
}
