using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class NPCInfoDisplay : MonoBehaviour
{
	private void Start()
	{
		if (NPCInfoDisplay.Prefab == null)
		{
			NPCInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/NPCInfo") as GameObject);
		}
		this.ShowDisplayInfo();
	}

	private void OnBecameVisible()
	{
		this.ShowDisplayInfo();
	}

	private void OnBecameInvisible()
	{
		this.HideDisplayInfo();
	}

	private void OnDestroy()
	{
		this.HideDisplayInfo();
		base.CancelInvoke("StartTime");
	}

	private void ShowDisplayInfo()
	{
		if (null != this.NGUIChildObject)
		{
			return;
		}
		if (null == NPCInfoDisplay.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		this.NGUIChildObject = DisplayInfoManager.Instance.CreateNPCInfoDisplay();
		this.NGUIChildObject.GetComponent<UIFollowTarget>().target = this.Target;
		this.NPCName = this.NGUIChildObject.transform.Find("Label_NPCName").gameObject.GetComponent<UILabel>();
		this.NPCName.text = this.NPCNameText;
		this.NPCName.color = new Color(0f, 1f, 0f, 1f);
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		DisplayInfoManager.Instance.DeleteNPCInfoDisplay(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.NPCName = null;
	}

	public int Time
	{
		get
		{
			return this.mTime;
		}
		set
		{
			if (this.mTime > 0)
			{
				this.mTime = value;
				return;
			}
			this.mTime = value;
			base.InvokeRepeating("StartTime", 1f, 1f);
		}
	}

	private void StartTime()
	{
		if (this.mTime > 0)
		{
			this.mTime--;
			if (this.NPCNameText != null)
			{
				int num = this.mTime / 3600;
				int num2 = this.mTime % 3600 / 60;
				int num3 = this.mTime % 3600 % 60;
				string text = num2.ToString();
				string text2 = num3.ToString();
				if (num > 0)
				{
					if (num2 < 10)
					{
						text = string.Format("0{0}", num2.ToString());
					}
					if (num3 < 10)
					{
						text2 = string.Format("0{0}", num3.ToString());
					}
					this.NPCName.text = this.NPCNameText + Environment.NewLine + string.Format("{0}:{1}:{2}", num, text, text2);
				}
				else
				{
					if (num2 < 10)
					{
						text = string.Format("0{0}", num2.ToString());
					}
					if (num3 < 10)
					{
						text2 = string.Format("0{0}", num3.ToString());
					}
					this.NPCName.text = this.NPCNameText + Environment.NewLine + string.Format(Global.GetLang("剩余时间：0:{0}:{1}"), text, text2);
				}
			}
		}
		else
		{
			this.NPCName.text = this.NPCNameText;
			base.CancelInvoke("StartTime");
		}
	}

	private static GameObject Prefab;

	public Transform Target;

	public string NPCNameText;

	private GameObject NGUIChildObject;

	private UILabel NPCName;

	private int mTime = -1;
}
