using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class FirstWorldPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("开始征程");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.thisCtrl = this;
		this._Submit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int firstMainTaskID = Global.GetFirstMainTaskID();
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(firstMainTaskID);
			if (taskXmlNodeByID == null)
			{
				return;
			}
			int mapCode = -1;
			int npcType = -1;
			int npcID = -1;
			Super.GetTaskDestNPCID(taskXmlNodeByID, ref mapCode, ref npcType, ref npcID);
			Super.AutoFindRoad(mapCode, npcType, npcID, 1);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this.thisCtrl, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 0
				});
			}
		};
		if (Global.IsInZhuTiFuActivity())
		{
			this._Bak.URL = Global.GetZhuTiFuNetImg("HuanYing", this._Bak.URL);
		}
		if (Global.IsTuiGuangFenBao)
		{
			this._Bak.URL = "NetImages/GameRes/Images/Plate/firstTask_tuiguang.png.qj";
		}
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
	}

	public void CleanUpChildWindows()
	{
	}

	private void Container_MouseLeftButtonUp(MouseEvent e)
	{
	}

	private void UserControl_MouseLeftButtonUp(MouseEvent e)
	{
	}

	private void UserControl_MouseLeftButtonDown(MouseEvent e)
	{
	}

	private void Container_MouseLeftButtonDown(MouseEvent e)
	{
	}

	public ShowNetImage _Bak;

	public GButton _Submit;

	public DPSelectedItemEventHandler DPSelectedItem;

	private SpriteSL thisCtrl;
}
