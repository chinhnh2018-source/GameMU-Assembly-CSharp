using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengLianSaiHelpPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("查看规则")
			});
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		try
		{
			this.mBtnHanders = new ZhanMengLianSaiHelpPart.BtnHander[3];
			this.mBtnHanders[0] = new ZhanMengLianSaiHelpPart.BtnHander(this.mObjBtn1, 1);
			this.mBtnHanders[0].Hander = new DPSelectedItemEventHandler(this.BtnClick);
			this.mBtnHanders[1] = new ZhanMengLianSaiHelpPart.BtnHander(this.mObjBtn2, 2);
			this.mBtnHanders[1].Hander = new DPSelectedItemEventHandler(this.BtnClick);
			this.mBtnHanders[2] = new ZhanMengLianSaiHelpPart.BtnHander(this.mObjBtn3, 3);
			this.mBtnHanders[2].Hander = new DPSelectedItemEventHandler(this.BtnClick);
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
		}
	}

	private void BtnClick(object sender, DPSelectedItemEventArgs args)
	{
		PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
		{
			ID = 1508,
			Index = 1,
			MyID = args.ID - 1
		});
	}

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private GameObject mObjBtn1;

	[SerializeField]
	private GameObject mObjBtn2;

	[SerializeField]
	private GameObject mObjBtn3;

	[SerializeField]
	private UILabel mTitleLabel;

	private ZhanMengLianSaiHelpPart.BtnHander[] mBtnHanders;

	public DPSelectedItemEventHandler Hander;

	private class BtnHander
	{
		public BtnHander(GameObject root, int id)
		{
			this.mImage = root.transform.FindChild("BG").GetComponent<ShowNetImage>();
			this.mBoxCollider = root.GetComponent<BoxCollider>();
			if (null == this.mBoxCollider)
			{
				this.mBoxCollider = root.AddComponent<BoxCollider>();
			}
			this.mBoxCollider.size = new Vector3(220f, 321f, 0f);
			this.mBoxCollider.center = new Vector3(0f, 0f, 0f);
			this.mID = id;
			if (id == 1)
			{
				this.URL = "NetImages/GameRes/Images/Plate/LianSaiRuleSuper.jpg";
			}
			else if (id == 2)
			{
				this.URL = "NetImages/GameRes/Images/Plate/LianSaiRuleNew.jpg";
			}
			else
			{
				this.URL = "NetImages/GameRes/Images/Plate/LianSaiRuleZhanChang.jpg";
			}
			UIEventListener.Get(root).onClick = delegate(GameObject e)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						ID = this.mID
					});
				}
			};
		}

		public string URL
		{
			set
			{
				try
				{
					this.mImage.URL = value;
					this.mImage.ImageDownloaded = delegate(object o)
					{
						this.mBoxCollider.size = new Vector3((float)this.mImage.ItsSizeWidth, (float)this.mImage.ItsSizeHeight, 0f);
						this.mBoxCollider.center = new Vector3(0f, 0f, 0f);
					};
				}
				catch (Exception ex)
				{
				}
			}
		}

		public DPSelectedItemEventHandler Hander;

		private ShowNetImage mImage;

		private BoxCollider mBoxCollider;

		private int mID;
	}
}
