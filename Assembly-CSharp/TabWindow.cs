using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TabWindow : UserControl
{
	protected override void InitializeComponent()
	{
		this._Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs());
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
		base.InitializeComponent();
		this.TexturePic.material.shader = Shader.Find("Unlit/Transparent Colored");
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this._GTabControl.TabBtns[this.RuntimeTabIndexes[0]], default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this._GTabControl.TabBtns[this.RuntimeTabIndexes[1]], default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected override void OnDestroy()
	{
		int num = this.ChildParts.Length;
		for (int i = num - 1; i >= 0; i--)
		{
			if (null != this.ChildParts[i])
			{
				Object.DestroyImmediate(this.ChildParts[i].gameObject);
				this.ChildParts[i] = null;
			}
		}
		base.OnDestroy();
	}

	public void InitPartData(int npcID = 0, int taskID = 0, int npcExtensionID = 0, int mapCode = 0, bool newTask = false, int type = 0)
	{
		for (int i = 0; i < 4; i++)
		{
			this.RuntimeTabIndexes[i] = -1;
		}
		this._GTabControl.BeforeTabBtnClick = delegate(object s, MouseEvent e)
		{
			if (e.Index == this._GTabControl.SelectIndex)
			{
				return;
			}
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (e.Index == this.RuntimeTabIndexes[1] && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.RiChangRenWu, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.RiChangRenWu, trigger, param, param2, true);
				return;
			}
			this._GTabControl.SetTab(s as GameObject);
		};
		this._GTabControl.OnTabBtnClick = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Index == this.RuntimeTabIndexes[0])
			{
				if (null == this.ChildParts[0])
				{
					this._mainTaskPart = U3DUtils.NEW<MainTaskPart>();
					this._GTabControl.AddPageContent(this._mainTaskPart.gameObject, e.Index);
					this._mainTaskPart.RefreshTask(-1);
					this.ChildParts[0] = null;
					this._mainTaskPart.DPSelectedItem = delegate(object s0, DPSelectedItemEventArgs e0)
					{
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(s0, e0);
						}
					};
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[0]);
				this.ChildParts[0] = null;
			}
			if (e.Index == this.RuntimeTabIndexes[1])
			{
				if (null == this.ChildParts[1])
				{
					this._riChangTaskPart = U3DUtils.NEW<RiChangTaskPart>();
					this._GTabControl.AddPageContent(this._riChangTaskPart.gameObject, e.Index);
					this.ChildParts[1] = null;
					this._riChangTaskPart.DPSelectedItem = delegate(object s0, DPSelectedItemEventArgs e0)
					{
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(s0, e0);
						}
					};
					int num2;
					int num3;
					if (Global.CanDoPaoHuanTask(8, out num2, out num3))
					{
						PlayZone.GlobalPlayZone.ClickNPC(119, 1);
					}
					else
					{
						this._riChangTaskPart.RefreshTask(-1);
					}
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[1]);
				this.ChildParts[1] = null;
			}
			if (e.Index == this.RuntimeTabIndexes[3])
			{
				if (null == this.ChildParts[3])
				{
					this._taoFaTaskPart = U3DUtils.NEW<TaoFaTaskPart>();
					this._GTabControl.AddPageContent(this._taoFaTaskPart.gameObject, e.Index);
					this.ChildParts[3] = null;
					this._taoFaTaskPart.DPSelectedItem = delegate(object s0, DPSelectedItemEventArgs e0)
					{
						if (this.DPSelectedItem != null)
						{
							this.DPSelectedItem(s0, e0);
						}
					};
					int num4;
					int num5;
					if (Global.CanDoPaoHuanTask(9, out num4, out num5))
					{
						PlayZone.GlobalPlayZone.ClickNPC(120, 1);
					}
					else
					{
						this._taoFaTaskPart.InitPartData(-1, false);
					}
				}
			}
			else
			{
				this._GTabControl.AddPageContent(null, this.RuntimeTabIndexes[3]);
				this.ChildParts[3] = null;
			}
		};
		int num = 0;
		this._TitleSprite.spriteName = "TitleRenWu";
		this.RuntimeTabIndexes[0] = num;
		this._GTabControl.SetTabButtonName(Global.GetLang("主 线"), num);
		num++;
		this._GTabControl.AddTabPage(2);
		this.RuntimeTabIndexes[1] = num;
		this._GTabControl.SetTabButtonName(Global.GetLang("日 常"), num);
		if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.RiChangRenWu))
		{
			this._GTabControl.SetMaskBtn(num, GongNengIDs.RiChangRenWu);
		}
		num++;
		this.ShowPage(type, 0);
	}

	public void ShowPage(int site, int param = 0)
	{
		int num = this.RuntimeTabIndexes[site];
		if (num < 0)
		{
			this._GTabControl.SetActivePage(0);
			return;
		}
		this._GTabControl.SetActivePage(num);
	}

	public GButton _Close;

	public UISprite _TitleSprite;

	public UserControl[] ChildParts = new UserControl[8];

	public GTabControl _GTabControl;

	[NonSerialized]
	public MainTaskPart _mainTaskPart;

	[NonSerialized]
	public RiChangTaskPart _riChangTaskPart;

	[NonSerialized]
	public TaoFaTaskPart _taoFaTaskPart;

	[NonSerialized]
	public ZhiXianTaskPart _ZhiXianTaskPart;

	public GameObject[] _ActivityTipIcons;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UITexture TexturePic;

	private int[] RuntimeTabIndexes = new int[4];
}
