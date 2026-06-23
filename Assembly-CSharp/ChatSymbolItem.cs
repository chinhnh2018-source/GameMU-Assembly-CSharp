using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChatSymbolItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_sprite = base.gameObject.GetComponent<UISprite>();
		this.m_spriteAnimation = base.gameObject.GetComponent<UISpriteAnimation>();
		this.m_spriteAnimation.enabled = false;
		base.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnClickHandler);
	}

	public void OnClickHandler(GameObject ob)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -1,
				Data = this.m_chatSymbol.ShowName
			});
		}
	}

	public void RefreshItem(ChatSymbol chatSymbol)
	{
		this.m_chatSymbol = chatSymbol;
		if (this.m_chatSymbol.IsAnimation)
		{
			this.m_sprite.spriteName = this.m_chatSymbol.ImageName + "0";
		}
		else
		{
			this.m_sprite.spriteName = this.m_chatSymbol.ImageName;
		}
		this.m_sprite.transform.localScale = new Vector3(40f, 40f, 1f);
	}

	private UISprite m_sprite;

	private UISpriteAnimation m_spriteAnimation;

	private ChatSymbol m_chatSymbol;

	public DPSelectedItemEventHandler DPSelectedItem;
}
