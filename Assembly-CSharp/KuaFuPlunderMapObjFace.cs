using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderMapObjFace : UserControl
{
	public Transform Target
	{
		get
		{
			return this.mTrans;
		}
		set
		{
			this.target = value;
			if (null != this.target)
			{
				this.mTrans = base.transform;
				if (this.gameCamera == null)
				{
					this.gameCamera = Global.MainCamera;
					this.mGameCameraTrans = this.gameCamera.transform;
				}
				if (this.uiCamera == null)
				{
					this.uiCamera = Global.UICamera;
				}
			}
		}
	}

	private Vector3 HangPointLocalPosition
	{
		get
		{
			if (!this.initHangPoint)
			{
				Renderer componentInChildren = this.target.GetComponentInChildren<Renderer>();
				if (componentInChildren)
				{
					this.hangPointLocalPosition = new Vector3(0f, Mathf.Clamp(componentInChildren.bounds.center.y + componentInChildren.bounds.extents.y - this.target.position.y, 1.6f, 2.2f), 0f);
				}
				else
				{
					MeshFilter componentInChildren2 = this.target.GetComponentInChildren<MeshFilter>();
					if (componentInChildren2)
					{
						this.hangPointLocalPosition = new Vector3(0f, Mathf.Clamp(componentInChildren2.sharedMesh.bounds.center.y + componentInChildren2.sharedMesh.bounds.extents.y, 1.6f, 2.2f), 0f);
					}
				}
				this.initHangPoint = true;
			}
			return this.hangPointLocalPosition;
		}
	}

	private new bool IsActive
	{
		set
		{
			if (this.isActive == value)
			{
				return;
			}
			this.isActive = value;
			this.mTrans.localPosition = new Vector3(0f, 1000f, 0f);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mTitleIcon.gameObject.SetActive(false);
		this.mHeadLabel.text = string.Empty;
		this.mHeadLabel.transform.localScale = new Vector3(18f, 18f, 0f);
		BoxCollider boxCollider = base.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.size = new Vector3(200f, 200f, 0f);
		boxCollider.center = new Vector3(0f, -boxCollider.size.y / 2f, 0f);
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject g)
		{
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs());
			}
		};
	}

	private new void Update()
	{
		if (!this.target)
		{
			base.transform.localPosition = new Vector3(10000f, 10000f, 10000f);
			return;
		}
		if (this.mLastTargetPos == this.target.position && this.mLastCameraPos == this.mGameCameraTrans.position)
		{
			return;
		}
		this.mLastTargetPos = this.target.position;
		this.mLastCameraPos = this.mGameCameraTrans.position;
		Vector3 vector = this.target.position + this.HangPointLocalPosition;
		vector = this.gameCamera.WorldToScreenPoint(vector);
		if (this.mLastUIPos != vector)
		{
			this.mLastUIPos = vector;
			this.mTrans.position = this.uiCamera.ScreenToWorldPoint(vector);
			vector = this.mTrans.localPosition;
			vector.z = 0f;
			vector.y += (float)Screen.height * this.pixelOffetPercentage;
			this.mTrans.localPosition = vector;
		}
	}

	public void RefreshServerInf(string Name, int type)
	{
		if (type == 0)
		{
			this.mTitleIcon.gameObject.SetActive(false);
		}
		else if (type == 1)
		{
			this.mTitleIcon.spriteName = "YiChuJia";
			this.mTitleIcon.gameObject.SetActive(true);
		}
		else if (type == 2)
		{
			this.mTitleIcon.spriteName = "YiHuoDeZiGe";
			this.mTitleIcon.gameObject.SetActive(true);
		}
		else
		{
			this.mTitleIcon.gameObject.SetActive(false);
		}
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(Name.SafeToInt32(0), out ztBuffServerInfo))
		{
			this.mHeadLabel.text = ztBuffServerInfo.strServerName;
		}
		else
		{
			this.mHeadLabel.text = "s." + Name;
		}
		float num = this.mHeadLabel.relativeSize.x * this.mHeadLabel.transform.localScale.x;
		this.mTitleBg.transform.localScale = new Vector3(num + 110f, this.mTitleBg.transform.localScale.y, 0f);
		base.transform.localPosition = new Vector3(base.transform.localPosition.x - num, base.transform.localPosition.y, base.transform.localPosition.z);
	}

	private const int mLineWidth = 110;

	[SerializeField]
	private UILabel mHeadLabel;

	[SerializeField]
	private UISprite mTitleBg;

	[SerializeField]
	private UISprite mTitleIcon;

	public DPSelectedItemEventHandler Hander;

	private Transform target;

	private Camera gameCamera;

	private Camera uiCamera;

	private float pixelOffetPercentage = 0.02f;

	private Transform mTrans;

	private Transform mGameCameraTrans;

	private Vector3 mLastTargetPos;

	private Vector3 mLastCameraPos;

	private Vector3 mLastUIPos;

	private bool initHangPoint;

	private Vector3 hangPointLocalPosition = Vector3.zero;

	private bool isActive = true;
}
