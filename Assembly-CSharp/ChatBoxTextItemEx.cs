using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChatBoxTextItemEx : MonoBehaviour
{
	public ChatChannelIndexes ChatChannelIndexes { private get; set; }

	public ChatTextItem ChatItem { get; set; }

	public bool ShowFace { private get; set; }

	private void Start()
	{
		this._ContentBg.transform.localScale = Vector3.zero;
		this._FaceRootObj.SetActive(false);
		this._FaceSp.enabled = false;
		UIEventListener.Get(this._FaceRootObj).onClick = delegate(GameObject g)
		{
			UIButtonMessage componentInChildren = this.NGUIHTMLUserName.GetComponentInChildren<UIButtonMessage>();
			if (null != componentInChildren)
			{
				componentInChildren.OnClickEx();
			}
		};
		UIEventListener.Get(this._FaceRootObj).onPress = delegate(GameObject g, bool s)
		{
			UIButtonColor componentInChildren = this.NGUIHTMLUserName.GetComponentInChildren<UIButtonColor>();
			if (null != componentInChildren)
			{
				componentInChildren.OnPress(s);
			}
		};
	}

	public string UserText
	{
		get
		{
			return this.NGUIHTMLUserName.html;
		}
		set
		{
			string text = value.Replace('[', '［');
			text = text.Replace(']', '］');
			if (Global.Data != null && this.ChatItem != null)
			{
				if (this.ChatItem.FromRoleID == Global.Data.RoleID)
				{
					this.NGUIHTMLUserName.OffSetX = 30;
				}
				else
				{
					this.NGUIHTMLUserName.OffSetX = 30;
				}
			}
			this.NGUIHTMLUserName.html = text;
		}
	}

	public string ContentText
	{
		get
		{
			return this.NGUIHTMLContent.html;
		}
		set
		{
			string text = value.Replace('[', '［');
			text = text.Replace(']', '］');
			if (Global.Data != null && this.ChatItem != null)
			{
				if (this.ChatItem.FromRoleID == Global.Data.RoleID)
				{
					this.NGUIHTMLContent.OffSetX = 30;
				}
				else
				{
					this.NGUIHTMLContent.OffSetX = 30;
				}
			}
			this.NGUIHTMLContent.html = text;
			this.Refresh();
		}
	}

	public void Refresh()
	{
		base.StartCoroutine(this.RefreshContentPos());
	}

	private IEnumerator RefreshContentPos()
	{
		yield return new WaitForEndOfFrame();
		Bounds bound;
		for (;;)
		{
			bound = NGUIMath.CalculateRelativeWidgetBounds(this.NGUIHTMLContent.transform);
			if (Vector3.zero != bound.size)
			{
				break;
			}
			yield return null;
		}
		float x = 28f + bound.size.x;
		float y = 10f + bound.size.y;
		if (30f > x)
		{
			x = 30f;
		}
		if (28f > y)
		{
			y = 28f;
		}
		Vector3 pos = new Vector3(20f + bound.center.x, -27f + bound.center.y, 0f);
		this._ContentBg.transform.localPosition = pos;
		this._ContentBg.transform.localScale = new Vector3(x, y, 1f);
		Vector3 quaternion = new Vector3(0f, 0f, 0f);
		if (this.ChatItem != null && Global.Data != null && this.ChatItem.FromRoleID == Global.Data.RoleID)
		{
			quaternion.y = 180f;
		}
		this._ContentBg.transform.localRotation = Quaternion.Euler(quaternion);
		if (Global.Data != null && this.ChatItem != null)
		{
			if (this.ChatItem.FromRoleID == Global.Data.RoleID)
			{
				this._FaceRootObj.transform.localPosition = new Vector3(375f, -37f, -0.001f);
			}
			else
			{
				this._FaceRootObj.transform.localPosition = new Vector3(0f, -37f, -0.001f);
			}
		}
		else
		{
			this._FaceRootObj.gameObject.SetActive(false);
			yield return null;
		}
		if (this.ShowFace)
		{
			this._FaceRootObj.SetActive(true);
			this._FaceSp.enabled = true;
			if (this.ChatItem != null)
			{
				if (this.ChatItem.ChatIndex == ChatTypeIndexes.System)
				{
					this._FaceSp.spriteName = "99_9";
				}
				else
				{
					this._FaceSp.spriteName = this.ChatItem.occupation + "0_0";
				}
			}
			else
			{
				yield return null;
			}
			if (this.ChatItem.ChatIndex < (ChatTypeIndexes)this.ChatIndexSpName.Length)
			{
				this._FaceBg.spriteName = this.ChatIndexSpName[(int)this.ChatItem.ChatIndex];
			}
		}
		else
		{
			this._FaceRootObj.SetActive(false);
		}
		if (this.hander != null)
		{
			this.hander(null, null);
		}
		yield break;
		yield break;
	}

	private const int with1 = 375;

	private const int with2 = 0;

	private const int OffSetXL = 30;

	private const int OffSetXR = 30;

	public DPSelectedItemEventHandler hander;

	[SerializeField]
	private UISprite _ContentBg;

	[SerializeField]
	private UISprite _FaceBg;

	[SerializeField]
	private UISprite _FaceBg1;

	[SerializeField]
	private UISprite _FaceSp;

	[SerializeField]
	private GameObject _FaceRootObj;

	public NGUIHTML NGUIHTMLUserName;

	public NGUIHTML NGUIHTMLContent;

	private string[] ChatIndexSpName = new string[]
	{
		"xitong",
		"fujin",
		"shijie",
		"zhanmeng",
		"duiwu",
		"siliao",
		"gonggao",
		"zhenying",
		"fuben",
		"juntuan",
		"shili",
		"pingtai",
		"zhandui"
	};
}
