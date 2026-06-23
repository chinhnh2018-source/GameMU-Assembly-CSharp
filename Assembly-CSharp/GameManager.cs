using System;
using HSGameEngine.GameEngine.Logic;

public class GameManager : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.UserControl_Loaded(this);
	}

	public override void Destroy()
	{
	}

	private void UserControl_Loaded(UserControl sender)
	{
		this.GamePlayZone = U3DUtils.NEW<PlayZone>();
		NGUITools.AddChild2(base.MyGameObject, this.GamePlayZone);
	}

	public int onRenderScene()
	{
		try
		{
			return this.GamePlayZone.onRenderScene();
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		return 0;
	}

	public int onUIRenderFrame()
	{
		try
		{
			return this.GamePlayZone.onUIRenderFrame();
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		return 0;
	}

	private PlayZone GamePlayZone;
}
