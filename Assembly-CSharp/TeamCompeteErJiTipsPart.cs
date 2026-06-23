using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TeamCompeteErJiTipsPart : UserControl
{
	public new string Name { get; set; }

	public int ID { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("提示");
		this.LblContent.Label.text = Global.GetLang(string.Empty);
		this.BtnConfirm.Label.text = Global.GetLang("提升队长");
		this.BtnCancel.Label.text = Global.GetLang("移出战队");
	}

	private void InitEvent()
	{
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ChangeLeaderCallBack != null)
			{
				this.ChangeLeaderCallBack.Invoke(this.ID);
			}
			this.CloseUIWindow();
		};
		this.BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DeleteMemberCallBack != null)
			{
				this.DeleteMemberCallBack.Invoke(this.ID);
			}
			this.CloseUIWindow();
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUIWindow();
		};
	}

	public void InitValue(int id, string name)
	{
		this.ID = id;
		this.Name = name;
		this.LblContent.Label.text = Global.GetLang("是否对该队员进行如下操作？");
	}

	private void CloseUIWindow()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public Action<int> DeleteMemberCallBack;

	public Action<int> ChangeLeaderCallBack;

	public TextBlock LblTitle;

	public TextBlock LblContent;

	public GButton BtnConfirm;

	public GButton BtnCancel;

	public GButton BtnClose;
}
