using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteBattlePart : UserControl
{
	private string LeftBakURL
	{
		set
		{
			this.mLeftBak.URL = value;
		}
	}

	private string RightBakURL
	{
		set
		{
			this.mRightBak.URL = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.DisplayNullMemberInfo(this.left);
		this.DisplayNullMemberInfo(this.right);
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
	}

	public void InitValue(List<ZhanDuiZhengBaZhanDuiData> datas)
	{
		ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = null;
		ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData2 = null;
		if (datas != null && datas.Count > 0)
		{
			if (datas.Count == 2)
			{
				zhanDuiZhengBaZhanDuiData = datas[0];
				zhanDuiZhengBaZhanDuiData2 = datas[1];
			}
			if (datas.Count == 1)
			{
				zhanDuiZhengBaZhanDuiData = datas[0];
			}
		}
		this.DisplayTitleInfo(0, zhanDuiZhengBaZhanDuiData, zhanDuiZhengBaZhanDuiData2);
		this.DisplayTitleInfo(1, zhanDuiZhengBaZhanDuiData2, zhanDuiZhengBaZhanDuiData);
		this.DisplayMemberInfo(0, zhanDuiZhengBaZhanDuiData, zhanDuiZhengBaZhanDuiData2);
		this.DisplayMemberInfo(1, zhanDuiZhengBaZhanDuiData2, zhanDuiZhengBaZhanDuiData);
		this.PlayEffect(zhanDuiZhengBaZhanDuiData, zhanDuiZhengBaZhanDuiData2);
	}

	private void PlayEffect(ZhanDuiZhengBaZhanDuiData data1, ZhanDuiZhengBaZhanDuiData data2)
	{
		if (data1 == null)
		{
			this.LoadParticalEffect(this.leftParticlePoint, this.failureParticalPath, true);
			this.LeftBakURL = this.TaoTaiURLPath;
			this.LoadParticalEffect(this.rightParticlePoint, this.successParticalPath, false);
			this.RightBakURL = this.JinJiURLPath;
			return;
		}
		if (data2 == null)
		{
			this.LoadParticalEffect(this.leftParticlePoint, this.successParticalPath, true);
			this.LeftBakURL = this.JinJiURLPath;
			this.LoadParticalEffect(this.rightParticlePoint, this.failureParticalPath, false);
			this.RightBakURL = this.TaoTaiURLPath;
			return;
		}
		if (data1.Grade < data2.Grade)
		{
			this.LoadParticalEffect(this.leftParticlePoint, this.successParticalPath, true);
			this.LeftBakURL = this.JinJiURLPath;
			this.LoadParticalEffect(this.rightParticlePoint, this.failureParticalPath, false);
			this.RightBakURL = this.TaoTaiURLPath;
		}
		else if (data1.Grade > data2.Grade)
		{
			this.LoadParticalEffect(this.leftParticlePoint, this.failureParticalPath, true);
			this.LeftBakURL = this.TaoTaiURLPath;
			this.LoadParticalEffect(this.rightParticlePoint, this.successParticalPath, false);
			this.RightBakURL = this.JinJiURLPath;
		}
		else
		{
			this.LoadVSEffect();
		}
	}

	private void LoadVSEffect()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>(this.VSEffectPath));
		NGUITools.AddChild2(this.VSEffectRoot, gameObject);
	}

	private void LoadParticalEffect(GameObject parent, string path, bool isLeft)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>(path));
		NGUITools.AddChild2(parent, gameObject);
		gameObject.transform.localPosition = new Vector3((float)((!isLeft) ? -5 : 5), 0f, 0f);
		this.ParticleMoveToEnd(parent, isLeft);
	}

	private void ParticleMoveToEnd(GameObject moveObj, bool isLeft)
	{
		Vector3 vector = (!isLeft) ? this.RightEndPos : this.LeftEndPos;
		if (!isLeft)
		{
			this.isShowEffectRoot = true;
		}
		else
		{
			this.isShowEffectRoot = false;
		}
		iTween.MoveTo(moveObj, iTween.Hash(new object[]
		{
			"position",
			vector,
			"time",
			this.intervalTime,
			"islocal",
			true
		}));
		this.OnTweenToTartetResultComplete();
	}

	private void OnTweenToTartetResultComplete()
	{
		if (this.isShowEffectRoot)
		{
			this.LoadResultEffect();
		}
	}

	private void LoadResultEffect()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(Resources.Load<GameObject>(this.bakPath));
		NGUITools.AddChild2(this.SplitRoot, gameObject);
	}

	private void DisplayTitleInfo(int whichSide, ZhanDuiZhengBaZhanDuiData data, ZhanDuiZhengBaZhanDuiData data2)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.titleInfoObj);
		gameObject.SetActive(true);
		gameObject.transform.SetParent(this.TitlePositions[whichSide].transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		TextBlock component = gameObject.transform.Find("LblTeamName").GetComponent<TextBlock>();
		component.Text = ((data != null) ? TeamCompeteDataManager.ServerTeamName(data.ZoneId, data.ZhanDuiName) : string.Empty);
		TextBlock component2 = gameObject.transform.Find("LblBattleValue").GetComponent<TextBlock>();
		if (data == null)
		{
			component2.Text = Global.GetLang("战力：0");
		}
		else
		{
			component2.Text = Global.GetLang("战力：") + data.ZhanLi.ToString();
		}
		TextBlock component3 = gameObject.transform.Find("LblDuanWei").GetComponent<TextBlock>();
		if (data == null)
		{
			component3.Text = Global.GetLang("段位：");
		}
		else
		{
			component3.Text = Global.GetLang("段位：") + TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId);
		}
		Transform transform = gameObject.transform.Find("gray");
		transform.transform.localPosition = new Vector3(transform.transform.localPosition.x - (float)((whichSide != this.left) ? 64 : 0), transform.transform.localPosition.y, transform.transform.localPosition.z);
		transform.transform.localEulerAngles = new Vector3(0f, (float)((whichSide != this.left) ? 180 : 0), 0f);
		if (data == null)
		{
			this.SetGray(transform, true);
		}
		else if (data2 != null)
		{
			if (data.Grade < data2.Grade)
			{
				this.SetGray(transform, false);
			}
			else
			{
				this.SetGray(transform, true);
			}
		}
		else
		{
			this.SetGray(transform, false);
		}
	}

	private void SetGray(Transform gray, bool isGray)
	{
		gray.gameObject.SetActive(isGray);
	}

	private void DisplayNullMemberInfo(int whichSide)
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>((whichSide != this.left) ? this.rightInfoObj : this.leftInfoObj);
			gameObject.SetActive(true);
			gameObject.transform.SetParent(this.infoParentObj.transform);
			Vector3 localPosition = this.Positions[i].transform.localPosition;
			gameObject.transform.localPosition = new Vector3(localPosition.x * (float)((whichSide != this.left) ? -1 : 1), localPosition.y, -1f);
			gameObject.transform.localScale = Vector3.one;
			TextBlock component = gameObject.transform.Find("LblTeamName").GetComponent<TextBlock>();
			component.Text = null;
			TextBlock component2 = gameObject.transform.Find("LblBattleValue").GetComponent<TextBlock>();
			component2.Text = null;
			if (whichSide == this.left)
			{
				this.leftMemberObj.Add(gameObject);
			}
			else
			{
				this.rightMemberObj.Add(gameObject);
			}
		}
	}

	private void DisplayMemberInfo(int whichSide, ZhanDuiZhengBaZhanDuiData data, ZhanDuiZhengBaZhanDuiData data2)
	{
		if (data == null && data2 == null)
		{
			return;
		}
		List<RoleOccuNameZhanLi> list = null;
		if (data != null)
		{
			list = data.MemberList;
		}
		List<GameObject> list2 = (whichSide != this.left) ? this.rightMemberObj : this.leftMemberObj;
		for (int i = 0; i < list2.Count; i++)
		{
			RoleOccuNameZhanLi roleOccuNameZhanLi = null;
			if (list != null && list.Count > 0 && i <= list.Count - 1)
			{
				roleOccuNameZhanLi = list[i];
			}
			GameObject gameObject = list2[i];
			Vector3 localPosition = this.Positions[i].transform.localPosition;
			gameObject.transform.localPosition = new Vector3(localPosition.x * (float)((whichSide != this.left) ? -1 : 1), localPosition.y, localPosition.z);
			gameObject.transform.localScale = Vector3.one;
			bool flag = data2 != null && (data == null || data.Grade > data2.Grade);
			UISprite component = gameObject.transform.Find("Bg").GetComponent<UISprite>();
			component.spriteName = ((!flag) ? ((whichSide != this.left) ? "blue" : "red") : "gray");
			if (component.spriteName == "gray")
			{
				component.transform.localEulerAngles = new Vector3(0f, (float)((whichSide != this.left) ? 180 : 0), 0f);
			}
			ShowNetImage component2 = gameObject.transform.Find("img").GetComponent<ShowNetImage>();
			component2.URL = ((roleOccuNameZhanLi != null) ? TeamCompeteDataManager.GetCircleTouXiangPathByOccu(roleOccuNameZhanLi.Occupation) : null);
			TextBlock component3 = gameObject.transform.Find("LblTeamName").GetComponent<TextBlock>();
			component3.Text = ((roleOccuNameZhanLi != null) ? roleOccuNameZhanLi.RoleName : null);
			TextBlock component4 = gameObject.transform.Find("LblBattleValue").GetComponent<TextBlock>();
			if (roleOccuNameZhanLi == null)
			{
				component4.Text = null;
			}
			else
			{
				component4.Text = Global.GetLang("战力：") + roleOccuNameZhanLi.ZhanLi.ToString();
			}
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public GButton BtnClose;

	public GameObject titleInfoObj;

	public GameObject infoParentObj;

	public GameObject leftInfoObj;

	public GameObject rightInfoObj;

	public Transform[] TitlePositions;

	public Transform[] Positions;

	public GameObject VSEffectRoot;

	public GameObject leftParticlePoint;

	public GameObject rightParticlePoint;

	public ShowNetImage mLeftBak;

	public ShowNetImage mRightBak;

	[Range(0f, 1f)]
	public float intervalTime;

	public Vector3 LeftEndPos = default(Vector3);

	public Vector3 RightEndPos = default(Vector3);

	private int left;

	private int right = 1;

	private string VSEffectPath = "UITeXiao/Perfabs/zhanduizhengba/zhanduizhengba_vs";

	private string bakPath = "UITeXiao/Perfabs/zhanduizhengba/zhanduizhengba_baodian";

	private string successParticalPath = "UITeXiao/Perfabs/zhanduizhengba/zhanduizhengba_wei_jinji";

	private string failureParticalPath = "UITeXiao/Perfabs/zhanduizhengba/zhanduizhengba_wei_taotai";

	private string JinJiURLPath = "NetImages/GameRes/Images/TeamCompete/jinji.png.qj";

	private string TaoTaiURLPath = "NetImages/GameRes/Images/TeamCompete/taotai.png.qj";

	public GameObject SplitRoot;

	private bool isShowEffectRoot;

	private List<GameObject> leftMemberObj = new List<GameObject>();

	private List<GameObject> rightMemberObj = new List<GameObject>();
}
