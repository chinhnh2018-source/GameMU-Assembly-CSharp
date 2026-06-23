using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChatBoxEmotionContainer : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitChatSymbolControl();
		this.ResetInfo();
		this.btnEmotion.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(ChatBoxEmotionType.Emotion);
		};
		this.btnBag.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(ChatBoxEmotionType.Bag);
		};
		this.btnRebornBag.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(ChatBoxEmotionType.RebornBag);
		};
		this.btnFriend.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectType(ChatBoxEmotionType.Friend);
		};
		this.bagContainer.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedEmotionItem != null)
			{
				e.ID = 2;
				this.DPSelectedEmotionItem(s, e);
			}
		};
		this.rebornBagContainer.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedEmotionItem != null)
			{
				e.ID = 2;
				this.DPSelectedEmotionItem(s, e);
			}
		};
		this.friendContainer.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (this.DPSelectedEmotionItem != null)
			{
				e.ID = 3;
				this.DPSelectedEmotionItem(s, e);
			}
		};
		UIEventListener.Get(this.closeBg).onClick = new UIEventListener.VoidDelegate(this.OnClose);
		if (SceneUIClasses.RebornMap.IsTheScene())
		{
			this.btnBag.gameObject.SetActive(false);
		}
		else if (Global.InKuafuHuodongSingle())
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.IsInKuafuHuodongZhenYing())
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.InKuafuFuben())
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || Global.GetMapSceneUIClass() == SceneUIClasses.ZhengDuoZhiDi)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else if (Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattleMiDong)
		{
			this.btnFriend.gameObject.SetActive(false);
			this.btnBag.gameObject.SetActive(false);
			this.btnRebornBag.gameObject.SetActive(false);
		}
		else
		{
			this.btnFriend.gameObject.SetActive(true);
			this.btnBag.gameObject.SetActive(true);
			this.btnRebornBag.gameObject.SetActive(true);
		}
		this.gridButton.Reposition();
	}

	public void ResetInfo()
	{
		this.OnSelectType(ChatBoxEmotionType.Emotion);
	}

	private void OnSelectType(ChatBoxEmotionType type)
	{
		if (this.m_currentSelectType == type)
		{
			return;
		}
		this.m_currentSelectType = type;
		this.OnSetButtonState(type);
	}

	private void OnSetButtonState(ChatBoxEmotionType selectedType)
	{
		this.SetButtonSelected(this.btnEmotion, selectedType == ChatBoxEmotionType.Emotion);
		this.SetButtonSelected(this.btnBag, selectedType == ChatBoxEmotionType.Bag);
		this.SetButtonSelected(this.btnRebornBag, selectedType == ChatBoxEmotionType.RebornBag);
		this.SetButtonSelected(this.btnFriend, selectedType == ChatBoxEmotionType.Friend);
		this.containerEmotion.SetActive(selectedType == ChatBoxEmotionType.Emotion);
		this.containerBag.SetActive(selectedType == ChatBoxEmotionType.Bag);
		this.containerRebornBag.SetActive(selectedType == ChatBoxEmotionType.RebornBag);
		this.containerFriend.SetActive(selectedType == ChatBoxEmotionType.Friend);
	}

	private GButton GetButtonByType(ChatBoxEmotionType selectedType)
	{
		GButton result = this.btnEmotion;
		switch (selectedType)
		{
		case ChatBoxEmotionType.Emotion:
			result = this.btnEmotion;
			break;
		case ChatBoxEmotionType.Bag:
			result = this.btnBag;
			break;
		case ChatBoxEmotionType.RebornBag:
			result = this.btnRebornBag;
			break;
		case ChatBoxEmotionType.Friend:
			result = this.btnFriend;
			break;
		}
		return result;
	}

	private void SetButtonSelected(GButton btn, bool beSelected)
	{
		if (beSelected)
		{
			btn.target.spriteName = "chatTab_hover";
			btn.hoverSprite = "chatTab_hover";
			btn.normalSprite = "chatTab_hover";
			btn.pressedSprite = "chatTab_hover";
		}
		else
		{
			btn.target.spriteName = "chatTab_normal";
			btn.hoverSprite = "chatTab_normal";
			btn.normalSprite = "chatTab_normal";
			btn.pressedSprite = "chatTab_normal";
		}
		Transform transform = btn.transform.FindChild("Content");
		if (null != transform)
		{
			UISprite component = transform.GetComponent<UISprite>();
			if (beSelected)
			{
				if (component.spriteName.EndsWith("2"))
				{
					component.spriteName = component.spriteName.Replace("2", "1");
				}
			}
			else if (component.spriteName.EndsWith("1"))
			{
				component.spriteName = component.spriteName.Replace("1", "2");
			}
		}
	}

	private void InitChatSymbolControl()
	{
		this.m_symbolTable.Clear();
		Dictionary<uint, ChatSymbol>.Enumerator enumerator = ChatSymbolConfig.Singleton.ChatSymbolDict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ChatSymbolItem chatSymbolItem = U3DUtils.NEW<ChatSymbolItem>();
			U3DUtils.AddChild(this.m_symbolTable.gameObject, chatSymbolItem.gameObject, false);
			ChatSymbolItem chatSymbolItem2 = chatSymbolItem;
			KeyValuePair<uint, ChatSymbol> keyValuePair = enumerator.Current;
			chatSymbolItem2.RefreshItem(keyValuePair.Value);
			chatSymbolItem.transform.localPosition = new Vector3(0f, 0f, -1f);
			chatSymbolItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedEmotionItem != null)
				{
					e.ID = 1;
					this.DPSelectedEmotionItem(s, e);
				}
			};
		}
		this.m_symbolTable.Reposition();
	}

	private void OnClose(GameObject go)
	{
		if (this.DPSelectedEmotionItem != null)
		{
			this.DPSelectedEmotionItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
	}

	private const string chatTab_hover = "chatTab_hover";

	private const string chatTab_normal = "chatTab_normal";

	public UIGrid gridButton;

	public GButton btnEmotion;

	public GButton btnBag;

	public GButton btnRebornBag;

	public GButton btnFriend;

	public UITable m_symbolTable;

	public GameObject containerEmotion;

	public GameObject containerBag;

	public GameObject containerRebornBag;

	public GameObject containerFriend;

	public ChatBoxBagContainer bagContainer;

	public ChatBoxBagContainer rebornBagContainer;

	public ChatBoxFriendPart friendContainer;

	public DPSelectedItemEventHandler DPSelectedEmotionItem;

	public GameObject closeBg;

	private ChatBoxEmotionType m_currentSelectType;
}
