using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TabButtonOpen : MonoBehaviour
{
	private void Start()
	{
		if (this.TabId == GongNengIDs.None)
		{
			return;
		}
		this.SetTabState(this.TabId, "roleTab_normal", null, null);
	}

	public void SetTabState(GongNengIDs tabId, string disableSpriteName = "roleTab_normal", UISprite tabBak = null, UILabel tabLabel = null)
	{
		this.TabId = tabId;
		if (tabId == GongNengIDs.None)
		{
			return;
		}
		GButton component = base.GetComponent<GButton>();
		if (tabBak != null)
		{
			this.TabBak = tabBak;
		}
		else if (!this.TabBak && component)
		{
			this.TabBak = component.target;
		}
		if (tabLabel != null)
		{
			this.TabLabel = tabLabel;
		}
		else if (!this.TabLabel && component)
		{
			this.TabLabel = component.Label;
		}
		GongNengYuGaoIconType gongNengYuGaoIconType = GongNengYuGaoIconType.None;
		if (!GongnengYugaoMgr.IsGongNengOpened(this.TabId, ref gongNengYuGaoIconType, ref this.HintStr))
		{
			if (gongNengYuGaoIconType == GongNengYuGaoIconType.Hidden)
			{
				base.gameObject.SetActive(false);
			}
			else if (gongNengYuGaoIconType == GongNengYuGaoIconType.GrayAndLock)
			{
				component.normalSprite = disableSpriteName;
				component.hoverSprite = disableSpriteName;
				component.pressedSprite = disableSpriteName;
				component.disabledSprite = disableSpriteName;
				if (!this.SprLock && this.TabBak)
				{
					this.SprLock = Object.Instantiate<GameObject>(this.TabBak.gameObject);
					this.ClearChild(this.SprLock.transform);
					this.SprLock.transform.parent = this.TabBak.transform.parent;
					this.SprLock.transform.localPosition = new Vector3(0f, 0f, -0.5f);
					this.SprLock.transform.localScale = component.target.transform.localScale;
					UISprite component2 = this.SprLock.GetComponent<UISprite>();
					component2.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("JiaSuo_atlas", true));
					if (component2)
					{
						component2.spriteName = "AnNiu_Suo";
						component2.depth = this.TabLabel.depth + 10;
					}
					component2.MakePixelPerfect();
					this.SprLock.transform.localPosition = new Vector3(0f, 0f, -1.5f);
				}
				this.SprLock.SetActive(true);
				if (this.TabLabel)
				{
					if (this.newLabel == null)
					{
						GameObject gameObject = Object.Instantiate<GameObject>(this.TabLabel.gameObject);
						this.ClearChild(gameObject.transform);
						gameObject.transform.parent = this.TabLabel.transform.parent;
						gameObject.transform.localPosition = new Vector3(0f, 0f, -0.2f);
						gameObject.transform.localScale = new Vector3(22f, 22f, 22f);
						this.newLabel = gameObject.GetComponent<UILabel>();
						this.newLabel.color = Color.gray;
					}
					this.TabLabel.gameObject.SetActive(false);
				}
				UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
				{
					Super.HintMainText(this.HintStr, 10, 3);
				};
				this.times = 0;
				base.InvokeRepeating("DelayInit", 0.05f, 0.2f);
			}
		}
		else if (gongNengYuGaoIconType == GongNengYuGaoIconType.Hidden)
		{
			base.gameObject.SetActive(true);
		}
	}

	private void DelayInit()
	{
		this.times++;
		if (this.times >= 15)
		{
			base.CancelInvoke("DelayInit");
		}
		if (this.newLabel != null)
		{
			this.newLabel.text = this.TabLabel.text;
		}
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			Super.HintMainText(this.HintStr, 10, 3);
		};
	}

	private void ClearChild(Transform trf)
	{
		for (int i = trf.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(trf.GetChild(i).gameObject);
		}
	}

	public GongNengIDs TabId = GongNengIDs.None;

	public UISprite TabBak;

	public UILabel TabLabel;

	private GameObject SprLock;

	private UILabel newLabel;

	private string HintStr = string.Empty;

	private int times;
}
