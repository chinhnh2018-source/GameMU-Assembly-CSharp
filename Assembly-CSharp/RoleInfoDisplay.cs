using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RoleInfoDisplay : MonoBehaviour
{
	public Transform Target
	{
		get
		{
			return this._Target;
		}
		set
		{
			this._Target = value;
			if (null != this._Target)
			{
				if (null != this._UIFollowTarget)
				{
					this._UIFollowTarget.target = this.Target;
				}
				else if (null != this.NGUIChildObject)
				{
					this._UIFollowTarget = this.NGUIChildObject.GetComponent<UIFollowTarget>();
					if (null != this._UIFollowTarget)
					{
						this._UIFollowTarget.target = this.Target;
					}
				}
			}
		}
	}

	public string RoleNameText
	{
		get
		{
			return this._RoleNameText;
		}
		set
		{
			this._RoleNameText = value;
			if (null != this.RoleName)
			{
				this.RoleName.text = this._RoleNameText;
			}
		}
	}

	public Color RoleNameColor
	{
		get
		{
			if (null != this.RoleName)
			{
				return this.RoleName.color;
			}
			return Color.white;
		}
		set
		{
			this._RoleNameColor = value;
			if (null != this.RoleName)
			{
				this.RoleName.color = this._RoleNameColor;
			}
		}
	}

	public string BanghuiNameText
	{
		set
		{
			this._BanghuiNameText = value;
			if (null != this.BanghuiName)
			{
				if (!string.IsNullOrEmpty(this._BanghuiNameText))
				{
					this.BanghuiName.text = this._BanghuiNameText;
					this.BanghuiName.gameObject.SetActive(true);
				}
				else
				{
					this.BanghuiName.gameObject.SetActive(false);
				}
				this.Refresh();
			}
		}
	}

	public Color BanghuiNameColor
	{
		set
		{
			this._BanghuiNameColor = value;
			if (null != this.BanghuiName)
			{
				this.BanghuiName.color = this._BanghuiNameColor;
			}
		}
	}

	public string OtherNameText
	{
		get
		{
			return this._OtherNameText;
		}
		set
		{
			this._OtherNameText = value;
			if (null != this.OtherName)
			{
				this.OtherName.text = this._OtherNameText;
				if (null != this.BaitanSprite && null != this.OtherName)
				{
					string text = "baitan";
					if (this._OtherNameText != null && this._OtherNameText != string.Empty && null != this.BaitanSprite.atlas && this.BaitanSprite.atlas.GetSprite(text) != null)
					{
						this.BaitanSprite.spriteName = text;
						this.BaitanSprite.gameObject.SetActive(true);
					}
					else
					{
						this.BaitanSprite.spriteName = "none";
						this.BaitanSprite.gameObject.SetActive(false);
					}
					this.Refresh();
				}
				else if (this.BaitanSprite != null)
				{
					this.BaitanSprite.spriteName = "none";
					this.BaitanSprite.gameObject.SetActive(false);
				}
			}
		}
	}

	public string TeamSpriteName
	{
		set
		{
			this._TeamSpriteName = value;
			if (null != this.TeamSprite)
			{
				if (null != this.TeamSprite.atlas && this.TeamSprite.atlas.GetSprite(this._TeamSpriteName) != null)
				{
					this.TeamSprite.spriteName = this._TeamSpriteName;
					this.TeamSprite.gameObject.SetActive(true);
				}
				else
				{
					this.TeamSprite.gameObject.SetActive(false);
				}
				this.Refresh();
			}
		}
	}

	public int CompType
	{
		set
		{
			this._compType = value;
			string text = string.Empty;
			if (value <= 0)
			{
				text = string.Empty;
				if (null != this.ShiLiSprite)
				{
					this.ShiLiSprite.gameObject.SetActive(false);
				}
			}
			else
			{
				text = "shili" + value;
				if (null != this.ShiLiSprite)
				{
					if (null != this.ShiLiSprite.atlas && this.ShiLiSprite.atlas.GetSprite(text) != null)
					{
						this.ShiLiSprite.spriteName = text;
						this.ShiLiSprite.gameObject.SetActive(true);
						this.ShiLiSprite.transform.localScale = new Vector3(40f, 40f, 1f);
					}
					else
					{
						this.ShiLiSprite.gameObject.SetActive(false);
					}
				}
			}
			this.Refresh();
		}
	}

	public int RolePlatform
	{
		set
		{
			if (Global.isHaiWai)
			{
				return;
			}
			if (!SceneUIClasses.RebornMap.IsTheScene())
			{
				return;
			}
			this.mRolePlatform = value;
			if (value <= 0)
			{
				if (null != this.ShiLiSprite)
				{
					this.ShiLiSprite.gameObject.SetActive(false);
				}
			}
			else
			{
				string text = "platform_" + value;
				if (null != this.ShiLiSprite)
				{
					if (null != this.ShiLiSprite.atlas && this.ShiLiSprite.atlas.GetSprite(text) != null)
					{
						this.ShiLiSprite.spriteName = text;
						this.ShiLiSprite.gameObject.SetActive(true);
						this.ShiLiSprite.transform.localScale = new Vector3(21f, 21f, 1f);
					}
					else
					{
						this.ShiLiSprite.gameObject.SetActive(false);
					}
				}
			}
			this.Refresh();
		}
	}

	public string PKKingSpriteName
	{
		set
		{
			this._PKKingSpriteName = value;
		}
	}

	public string ChengjiuSpriteFlag
	{
		set
		{
			this._ChengjiuSpriteFlag = value;
			if (null != this.ChengjiuSprite)
			{
				if (null != this.ChengjiuSprite.atlas && this.ChengjiuSprite.atlas.GetSprite(this._ChengjiuSpriteFlag) != null)
				{
					this.ChengjiuSprite.spriteName = this._ChengjiuSpriteFlag;
					this.ChengjiuSprite.gameObject.SetActive(true);
					this.ChengjiuSprite.MakePixelPerfect();
				}
				else
				{
					this.ChengjiuSprite.gameObject.SetActive(false);
				}
				this.Refresh();
			}
		}
	}

	public string JunxianSpriteName
	{
		set
		{
			this._JunxianSpriteName = value;
			if (null != this.JunxianSprite)
			{
				if (null != this.JunxianSprite.atlas && this.JunxianSprite.atlas.GetSprite(this._JunxianSpriteName) != null)
				{
					this.JunxianSprite.spriteName = this._JunxianSpriteName;
					this.JunxianSprite.gameObject.SetActive(true);
				}
				else
				{
					this.JunxianSprite.gameObject.SetActive(false);
				}
				this.Refresh();
			}
		}
	}

	public string LuoLanSpriteName
	{
		set
		{
			this._LuolanSpriteName = value;
		}
	}

	public string TitleName
	{
		set
		{
			this._titleName = value;
			if (null != this.fashionTitle)
			{
				this.fashionTitle.URL = this._titleName;
				this.fashionTitle.AutoResize = true;
				this.fashionTitle.gameObject.SetActive(!string.IsNullOrEmpty(this._titleName));
				this.Refresh();
			}
		}
	}

	public List<int> roledataBufferFashionID
	{
		set
		{
			if (value.Count >= 40)
			{
				this._curBuffFashionID = value[40];
				this.Refresh();
			}
			else
			{
				this._curBuffFashionID = 0;
			}
		}
	}

	public List<BufferData> roledataBufferDataList
	{
		set
		{
			if (value == null || value.Count <= 0)
			{
				return;
			}
			if (this._roledataBufferDic == null)
			{
				this._roledataBufferDic = new Dictionary<int, BufferData>();
			}
			this._roledataBufferDic.Clear();
			for (int i = 0; i < value.Count; i++)
			{
				if (Global.SpecialTitleBuffer(value[i].BufferID, true))
				{
					BufferData bufferData = new BufferData();
					bufferData.BufferID = value[i].BufferID;
					bufferData.StartTime = value[i].StartTime;
					bufferData.BufferSecs = value[i].BufferSecs;
					bufferData.BufferVal = value[i].BufferVal;
					bufferData.BufferType = value[i].BufferType;
					this._roledataBufferDic.Add(bufferData.BufferID, bufferData);
				}
			}
			if (this._roledataBufferDic.Count > 0)
			{
				this.Refresh();
			}
		}
	}

	public int fakeRoleBuffFashionID
	{
		get
		{
			return this._fakeRoleBuffFashionID;
		}
		set
		{
			this._fakeRoleBuffFashionID = value;
		}
	}

	public string ZhongShenSpriteName
	{
		set
		{
			this._ZhongShenSpriteName = value;
		}
	}

	private void Start()
	{
		if (RoleInfoDisplay.Prefab == null)
		{
			RoleInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/RoleInfo") as GameObject);
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
	}

	private void ShowDisplayInfo()
	{
		if (null != this.NGUIChildObject)
		{
			return;
		}
		if (null == RoleInfoDisplay.Prefab)
		{
			return;
		}
		if (null == this.Target)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		if (Global.CanGuanZhan() && this.Target.name == "Leader")
		{
			return;
		}
		this.NGUIChildObject = DisplayInfoManager.Instance.CreateRoleInfoDisplay();
		this._UIFollowTarget = this.NGUIChildObject.AddComponent<UIFollowTarget>();
		this._UIFollowTarget.target = this.Target;
		this._UIFollowTarget.HorseHeght = this.mhorseHeght;
		this._UIFollowTarget.BianShenHeight = this._bianShenHeight;
		base.StartCoroutine(this.ShowComponent());
	}

	private IEnumerator ShowComponent()
	{
		yield return 1;
		if (this.NGUIChildObject != null)
		{
			Transform trans = this.NGUIChildObject.transform;
			if (trans != null)
			{
				this.RoleName = trans.Find("Label_RoleName").gameObject.GetComponent<UILabel>();
				this.RoleName.text = this.RoleNameText;
				this.RoleNameColor = this._RoleNameColor;
				this.BanghuiName = trans.Find("Label_BanghuiName").gameObject.GetComponent<UILabel>();
				this.BanghuiNameText = this._BanghuiNameText;
				this.BanghuiNameColor = this._BanghuiNameColor;
				this.OtherName = trans.Find("Label_OtherName").gameObject.GetComponent<UILabel>();
				this.BaitanSprite = trans.Find("Sprite_Baitan").gameObject.GetComponent<UISprite>();
				this.OtherNameText = this._OtherNameText;
				this.ChengjiuSprite = trans.Find("Sprite_Chengjiu").gameObject.GetComponent<UISprite>();
				this.ChengjiuSpriteFlag = this._ChengjiuSpriteFlag;
				this.TeamSprite = trans.Find("Sprite_Team").gameObject.GetComponent<UISprite>();
				this.TeamSpriteName = this._TeamSpriteName;
				this.PKKingSprite = trans.Find("Sprite_PKKing").gameObject.GetComponent<UISprite>();
				this.PKKingSpriteName = this._PKKingSpriteName;
				this.JunxianSprite = trans.Find("Sprite_Junxian").gameObject.GetComponent<UISprite>();
				this.JunxianSpriteName = this._JunxianSpriteName;
				this.LuoLanSprite = trans.Find("Sprite_LuoLan").gameObject.GetComponent<UISprite>();
				this.LuoLanSpriteName = this._LuolanSpriteName;
				this.fashionTitle = trans.Find("Fashion_Title").gameObject.GetComponent<ShowNetImage>();
				this.TitleName = this._titleName;
				this.TeShuChengHao = trans.Find("Fashion_Title2").gameObject.GetComponent<ShowNetImage>();
				this.ZhongShenSprite = trans.Find("Sprite_ZhongShen").gameObject.GetComponent<UISprite>();
				this.ZhongShenSpriteName = this._ZhongShenSpriteName;
				this.ShiLiSprite = trans.Find("Sprite_ShiLi").gameObject.GetComponent<UISprite>();
				this.CompType = this._compType;
				this.RolePlatform = this.mRolePlatform;
			}
		}
		this.Refresh();
		yield break;
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		DisplayInfoManager.Instance.DeleteRoleInfoDisplay(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.RoleName = null;
		this.BanghuiName = null;
		this.TeamSprite = null;
		this.ChengjiuSprite = null;
	}

	private void Refresh()
	{
		if (this.RoleName == null)
		{
			return;
		}
		try
		{
			int num = (int)(this.RoleName.relativeSize.x * this.RoleName.transform.localScale.x / 2f);
			float num2 = 0f;
			if (null != this.BanghuiName && this.BanghuiName.gameObject.activeSelf)
			{
				num2 += 18f;
				this.BanghuiName.transform.localPosition = this.PosY(this.BanghuiName.transform.localPosition, num2);
			}
			if (null != this.ChengjiuSprite && this.ChengjiuSprite.gameObject.activeSelf)
			{
				num2 += 20.5f;
				this.ChengjiuSprite.transform.localPosition = this.Pos(this.ChengjiuSprite.transform.localPosition, 0.5f, num2);
			}
			if (null != this.TeamSprite && this.TeamSprite.gameObject.activeSelf)
			{
				this.TeamSprite.transform.localPosition = this.Pos(this.TeamSprite.transform.localPosition, (float)(-(float)num) - 15f, 0.5f);
			}
			if (null != this.fashionTitle && this.fashionTitle.gameObject.activeSelf)
			{
				num2 += 35f;
				this.fashionTitle.transform.localPosition = this.Pos(this.fashionTitle.transform.localPosition, 0.5f, num2);
			}
			if (null != this.JunxianSprite && this.JunxianSprite.gameObject.activeSelf)
			{
				this.JunxianSprite.transform.localPosition = this.Pos(this.JunxianSprite.transform.localPosition, Mathf.Round((float)num + 15f), 0.5f);
			}
			if (null != this.ShiLiSprite && this.ShiLiSprite.gameObject.activeSelf)
			{
				this.ShiLiSprite.transform.localPosition = this.Pos(this.ShiLiSprite.transform.localPosition, (float)(-(float)num) - 15f, 0.5f);
			}
			if (null != this.OtherName && this.OtherName.gameObject.activeSelf && !string.IsNullOrEmpty(this.OtherName.text))
			{
				num2 += 28f;
				this.OtherName.transform.localPosition = this.PosY(this.OtherName.transform.localPosition, num2);
				if (null != this.BaitanSprite && this.BaitanSprite.gameObject.activeSelf)
				{
					this.BaitanSprite.transform.localPosition = this.PosY(this.BaitanSprite.transform.localPosition, num2);
				}
			}
			if (this.TeShuChengHao != null)
			{
				int num3 = -1;
				if (this.fakeRoleBuffFashionID != 0)
				{
					for (int i = 0; i < Global.TeShuTitleListXml.Count; i++)
					{
						SpecialTitle specialTitle = Global.TeShuTitleListXml[i];
						if (this.fakeRoleBuffFashionID == specialTitle.BuffID)
						{
							num3 = i;
							break;
						}
					}
				}
				else
				{
					for (int j = 0; j < Global.TeShuTitleListXml.Count; j++)
					{
						SpecialTitle specialTitle2 = Global.TeShuTitleListXml[j];
						if (specialTitle2.BuffID == this._curBuffFashionID)
						{
							num3 = j;
							break;
						}
					}
					if (num3 > -1)
					{
						if (this._roledataBufferDic != null && this._roledataBufferDic.ContainsKey(this._curBuffFashionID))
						{
							BufferData bufferData = this._roledataBufferDic[this._curBuffFashionID];
							if (Global.IsTitleBufferDataOver(bufferData, 0L, false))
							{
								num3 = -1;
							}
						}
						else
						{
							num3 = -1;
						}
					}
				}
				if (num3 > -1)
				{
					string iconCode = Global.TeShuTitleListXml[num3].IconCode;
					this.TeShuChengHao.URL = "NetImages/GameRes/Images/ChengHaoTeShu/" + iconCode + ".png";
					this.TeShuChengHao.AutoResize = true;
					num2 += 40f;
					this.TeShuChengHao.transform.localPosition = this.Pos(this.TeShuChengHao.transform.localPosition, 0.5f, num2);
					this.TeShuChengHao.gameObject.SetActive(true);
				}
				else
				{
					this.TeShuChengHao.gameObject.SetActive(false);
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private Vector3 PosY(Vector3 v, float y)
	{
		v.y = y;
		return v;
	}

	private Vector3 Pos(Vector3 v, float x, float y)
	{
		v.x = x;
		v.y = y;
		return v;
	}

	public void ChangeUIFollowTargetHorseHeght(float horseHeght)
	{
		this.mhorseHeght = horseHeght;
		if (null != this.NGUIChildObject)
		{
			UIFollowTarget component = this.NGUIChildObject.GetComponent<UIFollowTarget>();
			if (null != component)
			{
				component.HorseHeght = horseHeght;
				component.target = this.Target;
			}
		}
	}

	public void ChangBianShenHeight(float bianShenHeight)
	{
		this._bianShenHeight = bianShenHeight;
		if (null != this.NGUIChildObject)
		{
			UIFollowTarget component = this.NGUIChildObject.GetComponent<UIFollowTarget>();
			if (null != component)
			{
				component.BianShenHeight = this._bianShenHeight;
			}
		}
	}

	private static GameObject Prefab;

	private Transform _Target;

	private int mRolePlatform = -1;

	private UIFollowTarget _UIFollowTarget;

	private GameObject NGUIChildObject;

	private UILabel RoleName;

	private UILabel BanghuiName;

	private UILabel OtherName;

	private UISprite TeamSprite;

	private UISprite ChengjiuSprite;

	private UISprite PKKingSprite;

	private UISprite JunxianSprite;

	private UISprite BaitanSprite;

	private UISprite LuoLanSprite;

	private ShowNetImage fashionTitle;

	private UISprite ZhongShenSprite;

	private ShowNetImage TeShuChengHao;

	private UISprite ShiLiSprite;

	private Vector2 _RoleLife = Vector2.one;

	private Color _RoleNameColor = Color.white;

	private Color _BanghuiNameColor = Color.white;

	private string _RoleNameText;

	private string _OtherNameText;

	private string _BanghuiNameText;

	private string _TeamSpriteName;

	private string _ChengjiuSpriteFlag;

	private string _PKKingSpriteName;

	private string _JunxianSpriteName;

	private string _LuolanSpriteName;

	private string _titleName;

	private string _ZhongShenSpriteName;

	private int _compType;

	private int _curBuffFashionID;

	private Dictionary<int, BufferData> _roledataBufferDic;

	private int _fakeRoleBuffFashionID;

	private float mhorseHeght;

	private float _bianShenHeight;
}
