using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class GongNeYuGaoItem : UserControl
{
	private void OnEnable()
	{
		this.ResetPosition();
	}

	public void ResetPosition()
	{
		if (this.bodySprite != null)
		{
			this.bodySprite.transform.localPosition = new Vector3(60f, 0f, -2f);
			this.bodySprite.transform.localScale = new Vector3(64f, 64f, 1f);
		}
	}

	protected override void InitializeComponent()
	{
		this.lab_Tips.text = Global.GetLang("任务开启");
		this.TeXiao.Visibility = false;
		this.isFirst = false;
		UIEventListener.Get(this.iconInfo).onPress = delegate(GameObject go, bool state)
		{
			if (this.setVo != null)
			{
				if (state)
				{
					this.detilePart.gameObject.SetActive(true);
				}
				else
				{
					this.detilePart.gameObject.SetActive(false);
				}
			}
		};
		this.ResetPosition();
	}

	public void UpdateInfo(AnnouncedSetVO vo, bool newOpen = false)
	{
		if (vo != null)
		{
			this.setVo = vo;
			newOpen = (newOpen || !this.isFirst);
			if (newOpen)
			{
				int roleSex = Global.Data.roleData.RoleSex;
				int occupation = Global.Data.roleData.Occupation;
				this.bodySprite.DestroyImmediateTexture();
				this.detile_Textrue.DestroyImmediateTexture();
				if (!string.IsNullOrEmpty(vo.Icon))
				{
					this.bodySprite.URL = string.Format("NetImages/GameRes/Images/Hybrid/{0}", vo.Icon);
				}
				this.ResetPosition();
				string text = string.Empty;
				string[] array = vo.name.Split(new char[]
				{
					'-'
				});
				string text2 = string.Empty;
				if (array.Length == 1)
				{
					text = array[0];
					text2 = string.Empty;
				}
				else if (array.Length > 1)
				{
					text = array[1];
					text2 = array[0];
				}
				else
				{
					text = vo.name;
					text2 = string.Empty;
				}
				this.nameLab.text = text;
				this.lblNameDes.text = text2;
				if (this.isFirst)
				{
				}
				if (!string.IsNullOrEmpty(vo.Note))
				{
					string[] array2 = AnnouncedSetVO.SplitPicName(vo.Note);
					string str = null;
					if (array2.Length == 1)
					{
						str = array2[0];
					}
					else
					{
						switch (occupation)
						{
						case 0:
							str = AnnouncedSetVO.SplitName(array2[0]);
							break;
						case 1:
							str = AnnouncedSetVO.SplitName(array2[1]);
							break;
						case 2:
							str = AnnouncedSetVO.SplitName(array2[2]);
							break;
						case 3:
							str = AnnouncedSetVO.SplitName(array2[3]);
							break;
						case 5:
							str = AnnouncedSetVO.SplitName(array2[4]);
							break;
						}
					}
					SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(Global.SafeConvertToInt32(str));
					if (systemOpenVOByID != null)
					{
						this.bodySprite.URL = string.Format("NetImages/GameRes/Images/Hybrid/{0}", systemOpenVOByID.ImageOne);
						this.detile_Textrue.URL = string.Format("NetImages/GameRes/Images/Hybrid/{0}", systemOpenVOByID.ImageOne);
						this.detile_Name.text = systemOpenVOByID.Name;
						this.detile_Description.text = systemOpenVOByID.Description;
						this.ResetPosition();
					}
				}
			}
			this.isFirst = true;
			int taskSum = AnnouncedSetVO.GetTaskSum(AnnouncedSetVO.curTaskID, vo.TaskEnd, true);
			int taskSum2 = AnnouncedSetVO.GetTaskSum(vo.TaskBegin, vo.TaskEnd, true);
			int num = taskSum2 - taskSum;
			this.tipPartLab.text = string.Format("{0}{1}", taskSum, Global.GetLang("个任务后解锁"));
			this.ResetPosition();
		}
	}

	private void DelayHide()
	{
		this.TipProgressBar.gameObject.SetActive(false);
	}

	public override void Destroy()
	{
		if (base.IsInvoking("DelayHide"))
		{
			base.CancelInvoke("DelayHide");
		}
		this.setVo = null;
	}

	public void OnDisable()
	{
		if (base.IsInvoking("DelayHide"))
		{
			base.CancelInvoke("DelayHide");
		}
	}

	public AnnouncedSetVO setVo;

	private bool isFirst;

	public ShowNetImage bodySprite;

	public UISprite kuanSprite;

	public UILabel nameLab;

	public CAnimation TeXiao;

	public UISprite backSprite;

	public GImgProgressBar TipProgressBar;

	public UILabel tipPartLab;

	public UILabel lblNameDes;

	public GameObject iconInfo;

	public GameObject detilePart;

	public ShowNetImage detile_Textrue;

	public UILabel detile_Name;

	public UILabel detile_Description;

	public UILabel lab_Tips;
}
