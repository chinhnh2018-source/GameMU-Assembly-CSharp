using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JingLingMapObjFace : MonoBehaviour
{
	public static PlayZone pz
	{
		get
		{
			return PlayZone.GlobalPlayZone;
		}
	}

	protected void OnDestroy()
	{
		JingLingMapObjFace._cout--;
		if (JingLingMapObjFace._cout <= 0)
		{
			Material[] array = Resources.FindObjectsOfTypeAll<Material>();
			foreach (Material material in array)
			{
				if (material.name.StartsWith("JingLingMapH_atlas") || material.name.StartsWith("JingLingMap_atlas"))
				{
					Resources.UnloadAsset(material);
				}
			}
		}
	}

	protected virtual void Awake()
	{
		this.Bak.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBakClick);
		this.btn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClick);
	}

	protected void ResetRootPos()
	{
		this.faceRoot = base.transform.FindChild("GameObject");
		if (this.nObjectIndex == 0)
		{
			this.faceRootPos = new Vector3(-90f, 45f, 0f);
		}
		else if (this.nObjectIndex == 1)
		{
			this.faceRootPos = new Vector3(-81f, 43f, 0f);
		}
		else if (this.nObjectIndex == 2)
		{
			this.faceRootPos = new Vector3(-90f, -10f, 0f);
		}
		else if (this.nObjectIndex == 3)
		{
			this.faceRootPos = new Vector3(-121f, 2f, 0f);
		}
		else if (this.nObjectIndex == 4)
		{
			this.faceRootPos = new Vector3(-63f, 8f, 0f);
		}
		else if (this.nObjectIndex == 5)
		{
			this.faceRootPos = new Vector3(-90f, 4f, 0f);
		}
		else if (this.nObjectIndex == 6)
		{
			this.faceRootPos = new Vector3(-90f, -2f, 0f);
		}
		this.faceRoot.localPosition = this.faceRootPos;
	}

	protected void Start()
	{
		JingLingMapObjFace._cout++;
	}

	protected virtual void OnBakClick(object sender, MouseEvent e)
	{
	}

	protected virtual void OnClick(object sender, MouseEvent e)
	{
		if (this.emEmBuildType != JingLingMapObj.EmBuildType.Home)
		{
			if (this.emEmBuildType != JingLingMapObj.EmBuildType.Task)
			{
				if (this.emEmBuildType != JingLingMapObj.EmBuildType.Boss)
				{
					MUDebug.LogError<string>(new string[]
					{
						"god bless you "
					});
				}
			}
		}
	}

	private static void ItemClick(GameObject go)
	{
	}

	public virtual void UpdateUI()
	{
	}

	public virtual void ResetState()
	{
	}

	public virtual void UITimer_Tick(object sender, object e)
	{
	}

	protected void RemoveModel(bool bLoadModel)
	{
		if (!bLoadModel && !string.IsNullOrEmpty(this.curModelObjectName))
		{
			GameObject gameObject = GameObject.Find(this.curModelObjectName);
			if (gameObject)
			{
				if (this.modelObject != null)
				{
					ObjectsManager.Remove(this.modelObject);
					this.modelObject = null;
				}
				else
				{
					Object.Destroy(gameObject);
					this.modelObject = null;
				}
			}
			else if (this.modelObject != null)
			{
				ObjectsManager.Remove(this.modelObject);
				this.modelObject = null;
			}
		}
	}

	public void Clear()
	{
		if (this.title)
		{
			this.title.gameObject.SetActive(false);
		}
		if (this.titlebak)
		{
			this.titlebak.gameObject.SetActive(false);
		}
		if (this.btn)
		{
			this.btn.gameObject.SetActive(false);
		}
		if (this.bar)
		{
			this.bar.gameObject.SetActive(false);
		}
		if (this.cdlabel)
		{
			this.cdlabel.gameObject.SetActive(false);
		}
		if (this.redlabel)
		{
			this.redlabel.gameObject.SetActive(false);
		}
	}

	public static int _cout;

	public Transform Target;

	public int nObjectIndex;

	public JingLingMapObj.EmBuildType emEmBuildType;

	public int taskId;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Transform faceRoot;

	public Vector3 faceRootPos = default(Vector3);

	public GButton Bak;

	public GameObject ani;

	public TextBlock title;

	public UISprite titlebak;

	public GImgProgressBar bar;

	public GButton btn;

	public Transform tipicon;

	public TextBlock cdlabel;

	public TextBlock redlabel;

	public GSprite modelObject;

	protected string modelObjectName = "jinglingmap_";

	protected string preModelObjectName = string.Empty;

	protected string curModelObjectName = string.Empty;
}
