using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ZhanMengLianSaiGuanZhanPopupList : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ObservableCollection RoleItemCollection
	{
		get
		{
			return this._RoleItemCollection;
		}
		set
		{
			this._RoleItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
		this.RoleItemCollection = this.mRoleListBox.ItemsSource;
		this.Init();
		this.InitEvent();
	}

	public void Init()
	{
		this.InitValue();
		this.ShowFreeModelUI();
		if (Global.IsInLangHunLingYuScene())
		{
			NGUITools.SetActive(this.mLblKillDes.gameObject, false);
		}
		else
		{
			NGUITools.SetActive(this.mLblKillDes.gameObject, true);
		}
	}

	private bool ArrowState
	{
		set
		{
			this.mArrow.spriteName = ((!value) ? "arrow_down" : "arrow_up");
		}
	}

	private void InitEvent()
	{
		try
		{
			this.roleListPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.RoleItemCollection.Clear();
					this.mRolelistPanel.gameObject.SetActive(false);
				}
			};
			this.mBtnFreeModel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.roleListPart.selectId > 0)
				{
					this.roleListPart.selectId = 0;
					GameInstance.Game.SendGuanZhanTrackOtherPlayer(-1);
				}
				this.ShowFreeModelUI();
			};
			this.mBtnTrackModel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.IsActiveRolelistPanel())
				{
					return;
				}
				this.ShowTrackModelUI();
				GameInstance.Game.GetGuanZhanRoleMiniDatalist();
			};
			this.mBtnGetRoleList.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.IsActiveRolelistPanel())
				{
					return;
				}
				GameInstance.Game.GetGuanZhanRoleMiniDatalist();
			};
			this.ArrowState = true;
			UIEventListener.Get(this.mBakClick.gameObject).onClick = delegate(GameObject s)
			{
				NGUITools.SetActive(this.mPopupDownList, !this.mPopupDownList.activeSelf);
				this.ArrowState = !this.mPopupDownList.activeSelf;
				if (this.mPopupDownList.activeSelf)
				{
					for (int i = 0; i < this.ItemCollection.Count; i++)
					{
						GameObject at = this.ItemCollection.GetAt(i);
						TextBlock component = at.transform.FindChild("DownLabel").GetComponent<TextBlock>();
						if (component.Text == this.mPopupList.selection)
						{
							component.Label.color = NGUIMath.HexToColorEx(16644061U);
							GameObject gameObject = at.transform.FindChild("Sprite").gameObject;
							if (gameObject != null && !gameObject.activeSelf)
							{
								gameObject.SetActive(true);
							}
						}
					}
				}
			};
			this.mListBox.SelectionChanged = delegate(object result, MouseEvent e)
			{
				GameObject selectedItem = this.mListBox.SelectedItem;
				if (selectedItem == null)
				{
					return;
				}
				Transform transform = selectedItem.transform.FindChild("DownLabel");
				if (transform == null)
				{
					return;
				}
				UILabel label = transform.GetComponent<UILabel>();
				if (label == null)
				{
					return;
				}
				int num = this.mPopupList.items.FindIndex((string ss) => ss == label.text);
				int guanZhanTransferIDByName = this.GetGuanZhanTransferIDByName(label.text);
				if (guanZhanTransferIDByName >= 0)
				{
					this.mPopupList.selection = this.mPopupList.items[num];
					this.GotoPosition(guanZhanTransferIDByName);
				}
				if (this.mPopupDownList.activeSelf)
				{
					for (int i = 0; i < this.ItemCollection.Count; i++)
					{
						GameObject at = this.ItemCollection.GetAt(i);
						TextBlock component = at.transform.FindChild("DownLabel").GetComponent<TextBlock>();
						component.Label.color = NGUIMath.HexToColorEx(10323559U);
						GameObject gameObject = at.transform.FindChild("Sprite").gameObject;
						if (gameObject != null && gameObject.activeSelf)
						{
							gameObject.SetActive(false);
						}
					}
				}
				NGUITools.SetActive(this.mPopupDownList, !this.mPopupDownList.activeSelf);
				this.ArrowState = !this.mPopupDownList.activeSelf;
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<Exception>(new Exception[]
			{
				ex
			});
		}
	}

	public void InitValue()
	{
		this.transfDict.Clear();
		this.mPopupList.items.Clear();
		int typeByGuanZhanScene = this.GetTypeByGuanZhanScene();
		if (typeByGuanZhanScene < 0)
		{
			return;
		}
		this.config = new ZhanMengLianSaiGuanZhanPopupList.ParseGuanZhanTransfer(typeByGuanZhanScene);
		Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> guanZhanTransferDict = this.config.GuanZhanTransferDict;
		if (guanZhanTransferDict == null || guanZhanTransferDict.Count <= 0)
		{
			return;
		}
		ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO guanZhanTransferVO = null;
		foreach (KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair in guanZhanTransferDict)
		{
			if (keyValuePair.Value.MapCode == Global.Data.roleData.MapCode)
			{
				Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO>.Enumerator enumerator;
				KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair2 = enumerator.Current;
				guanZhanTransferVO = keyValuePair2.Value;
				break;
			}
		}
		Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO>.Enumerator enumerator2 = guanZhanTransferDict.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			if (guanZhanTransferVO != null)
			{
				KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair3 = enumerator2.Current;
				if (keyValuePair3.Value.BattlefieldID == guanZhanTransferVO.BattlefieldID)
				{
					List<string> items = this.mPopupList.items;
					KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair4 = enumerator2.Current;
					items.Add(keyValuePair4.Value.ChuanSongName);
					Dictionary<string, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> dictionary = this.transfDict;
					KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair5 = enumerator2.Current;
					string chuanSongName = keyValuePair5.Value.ChuanSongName;
					KeyValuePair<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> keyValuePair6 = enumerator2.Current;
					dictionary.Add(chuanSongName, keyValuePair6.Value);
				}
			}
		}
		if (this.mPopupList.items.Count > 0)
		{
			this.mPopupList.selection = this.mPopupList.items[0];
		}
		for (int i = 0; i < this.mPopupList.items.Count; i++)
		{
			GameObject gameObject = NGUITools.AddChild(this.mListBox.gameObject, this.mLnlDown);
			gameObject.SetActive(true);
			TextBlock component = gameObject.transform.FindChild("DownLabel").GetComponent<TextBlock>();
			component.Text = this.mPopupList.items[i];
			this.ItemCollection.AddNoUpdate(gameObject);
		}
	}

	private void GotoPosition(int index)
	{
		if (index == 0)
		{
			return;
		}
		string[] array = this.config.GetItem(index).Site.Split(new char[]
		{
			'|'
		});
		MUDebug.LogError<string>(new string[]
		{
			string.Concat(new string[]
			{
				"移动到 ",
				array[0],
				" , ",
				array[1],
				Global.GetLang(" 位置")
			})
		});
		int mapCode = this.config.GetItem(index).MapCode;
		GameInstance.Game.SendGuanZhanData(mapCode, Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]));
	}

	private int GetTypeByGuanZhanScene()
	{
		if (Global.IsInZhanMengLianSaiCompetetionMap())
		{
			return 1;
		}
		if (Global.IsInLangHunLingYuScene())
		{
			return 2;
		}
		return -1;
	}

	private int GetGuanZhanTransferIDByName(string name)
	{
		int result = -1;
		if (this.transfDict != null && this.transfDict.Count > 0 && this.transfDict.ContainsKey(name))
		{
			result = this.transfDict[name].ID;
		}
		return result;
	}

	private void ShowFreeModelUI()
	{
		this.mFreedomModelObj.SetActive(true);
		this.mTrackModelObj.SetActive(false);
		this.SetBtnToDisable(this.mBtnFreeModel, false);
		this.SetBtnToDisable(this.mBtnTrackModel, true);
		this.mRolelistPanel.SetActive(false);
	}

	private void ShowTrackModelUI()
	{
		this.mFreedomModelObj.SetActive(false);
		this.mTrackModelObj.SetActive(true);
		this.SetBtnToDisable(this.mBtnFreeModel, true);
		this.SetBtnToDisable(this.mBtnTrackModel, false);
	}

	private void SetBtnToDisable(GButton btn, bool isAn)
	{
		btn.TextColor = ((!isAn) ? Color.white : Color.grey);
		btn.hoverSprite = ((!isAn) ? "AnNiu_GuanZhanXuanZhong" : "AnNiu_GuanZhanAn");
		btn.normalSprite = ((!isAn) ? "AnNiu_GuanZhanXuanZhong" : "AnNiu_GuanZhanAn");
		btn.pressedSprite = ((!isAn) ? "AnNiu_GuanZhanXuanZhong" : "AnNiu_GuanZhanAn");
	}

	private bool IsActiveRolelistPanel()
	{
		return this.mRolelistPanel.activeSelf;
	}

	private new void Update()
	{
		if (ZhanMengLianSaiGuanZhanPopupList.IsTracking && this.leaderTransform != null && this.otherTransform != null)
		{
			if (this.sp == null)
			{
				this.sp = Global.Data.GameScene.FindSprite("Leader");
			}
			this.sp.Coordinate = new Point((int)(this.otherTransform.localPosition.x + 1f) * 100, (int)(this.otherTransform.localPosition.z + 1f) * 100);
		}
		else
		{
			if (ZhanMengLianSaiGuanZhanPopupList.IsTracking && !string.IsNullOrEmpty(this.otherSpriteName) && this.otherTransform == null)
			{
				base.StartCoroutine<bool>(this.SearchOtherRole());
			}
			this.sp = null;
		}
	}

	private IEnumerator SearchOtherRole()
	{
		for (;;)
		{
			this.other = GameObject.Find(this.otherSpriteName);
			if (this.other != null)
			{
				break;
			}
			yield return new WaitForSeconds(0.5f);
		}
		MUDebug.LogError<string>(new string[]
		{
			"追踪到玩家： " + this.otherSpriteName
		});
		this.FindLeader();
		if (this.leader != null)
		{
			this.leaderTransform = this.leader.transform;
			this.leader.GetComponent<CameraController>().enabled = false;
		}
		if (this.other != null)
		{
			CameraController CamCtrl = this.other.GetComponent<CameraController>();
			if (CamCtrl == null)
			{
				CamCtrl = this.other.AddComponent<CameraController>();
				CamCtrl.enabled = true;
			}
			if (CamCtrl.Cam == null)
			{
				CamCtrl.Cam = Global.MainCamera.gameObject;
			}
			if (CamCtrl == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"挂载相机为空"
				});
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"挂载相机成功"
				});
			}
			ZhanMengLianSaiGuanZhanPopupList.CurrentTrackName = this.otherSpriteName;
			this.otherTransform = this.other.transform;
			if (this.otherTransform == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"FUCK otherTransform = null"
				});
			}
			ZhanMengLianSaiGuanZhanPopupList.IsTracking = true;
			this.LastTrackingRoleID = this.mTrackId;
			this.ChangeChengHaoFollowTarget(this.other.transform, StringUtil.substitute("Role_{0}", new object[]
			{
				this.mTrackId
			}));
			this.ShowTrackModelUI();
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"没有找到追踪的玩家！" + this.otherSpriteName
			});
		}
		base.StopCoroutine("SearchOtherRole");
		yield break;
		yield break;
	}

	private void ChangeChengHaoFollowTarget(Transform t, string lstName = null)
	{
		if (this.parent == null)
		{
			this.parent = GameObject.Find("HUDTextRoot");
		}
		if (this.parent == null)
		{
			return;
		}
		int childCount = this.parent.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform transform = this.parent.transform.GetChild(i).transform;
			UIFollowTarget component = transform.GetComponent<UIFollowTarget>();
			if (component != null)
			{
				component.enabled = false;
				component.enabled = true;
				if (component.target.name == this.otherSpriteName)
				{
					this.trackTarget = component;
				}
				if (component.target.name == lstName)
				{
					this.lasttrackTarget = component;
				}
			}
		}
		if (this.trackTarget != null)
		{
			UIFollowTarget component2 = this.trackTarget.GetComponent<UIFollowTarget>();
			component2.enabled = false;
			component2.enabled = true;
			this.trackTarget.gameObject.SetActive(false);
			this.trackTarget.gameObject.SetActive(true);
		}
		if (this.lasttrackTarget != null)
		{
			base.StopCoroutine("RefreshTitle");
			UIFollowTarget component3 = this.lasttrackTarget.GetComponent<UIFollowTarget>();
			component3.enabled = false;
			component3.enabled = true;
			this.lasttrackTarget.gameObject.SetActive(false);
			this.lasttrackTarget.gameObject.SetActive(true);
			base.StartCoroutine<bool>(this.RefreshTitle());
		}
	}

	private IEnumerator RefreshTitle()
	{
		yield return new WaitForSeconds(0.3f);
		if (this.lasttrackTarget != null)
		{
			UIFollowTarget ui = this.lasttrackTarget.GetComponent<UIFollowTarget>();
			ui.enabled = false;
			ui.enabled = true;
			this.lasttrackTarget.gameObject.SetActive(false);
			this.lasttrackTarget.gameObject.SetActive(true);
		}
		if (this.trackTarget != null)
		{
			UIFollowTarget ui2 = this.trackTarget.GetComponent<UIFollowTarget>();
			ui2.enabled = false;
			ui2.enabled = true;
			this.trackTarget.gameObject.SetActive(false);
			this.trackTarget.gameObject.SetActive(true);
		}
		yield break;
	}

	private void FindLeader()
	{
		if (this.leader == null)
		{
			this.leader = GameObject.Find("Leader");
		}
	}

	public void RefershRolelist(GuanZhanData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.RoleMiniDataDict == null || data.RoleMiniDataDict.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		this.mRolelistPanel.SetActive(true);
		this.roleListPart.InitData(data, this.RolelistItemObj);
	}

	public void ChangeCameraByServerInfo(int trackrid, int result)
	{
		MUDebug.LogError<string>(new string[]
		{
			string.Concat(new object[]
			{
				"trackrid ",
				trackrid,
				" result ",
				result
			})
		});
		if (result < 0)
		{
			if (result == -21)
			{
				Super.HintMainText(Global.GetLang("目标角色已离开"), 10, 3);
			}
			else if (result == -12)
			{
				Super.HintMainText(Global.GetLang("当前无法操作"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(result, false, false)), 10, 3);
			}
			GameInstance.Game.GetGuanZhanRoleMiniDatalist();
			return;
		}
		if (Global.Data.OtherRoles.ContainsKey(trackrid))
		{
			base.StopCoroutine("SearchOtherRole");
			if (this.LastTrackingRoleID != trackrid)
			{
			}
			this.mTrackId = trackrid;
			this.otherSpriteName = StringUtil.substitute("Role_{0}", new object[]
			{
				this.mTrackId
			});
			if (base.gameObject.activeSelf)
			{
				base.StartCoroutine<bool>(this.SearchOtherRole());
			}
		}
		else if (trackrid == -1)
		{
			this.FindLeader();
			if (this.leader != null)
			{
				if (this.LastTrackingRoleID > 0)
				{
					MUDebug.LogError<string>(new string[]
					{
						"清空上一次的追踪目标 " + this.LastTrackingRoleID
					});
					this.otherSpriteName = StringUtil.substitute("Role_{0}", new object[]
					{
						this.LastTrackingRoleID
					});
					this.other = GameObject.Find(this.otherSpriteName);
					if (this.other != null)
					{
						CameraController component = this.other.GetComponent<CameraController>();
						if (component != null)
						{
							MUDebug.LogError<string>(new string[]
							{
								"删除 CameraController "
							});
							Object.Destroy(component);
						}
						this.other = null;
					}
				}
				CameraController component2 = this.leader.GetComponent<CameraController>();
				if (component2 != null && !component2.enabled)
				{
					component2.enabled = true;
				}
				ZhanMengLianSaiGuanZhanPopupList.IsTracking = false;
				this.ChangeChengHaoFollowTarget(null, null);
				this.LastTrackingRoleID = 0;
				this.otherSpriteName = string.Empty;
			}
		}
	}

	private void OnDisable()
	{
		this.ItemCollection.Clear();
	}

	protected override void OnDestroy()
	{
		base.StopCoroutine("SearchOtherRole");
		base.StopCoroutine("RefreshTitle");
		ZhanMengLianSaiGuanZhanPopupList.IsTracking = false;
		this.mPopupList = null;
		this.mPopupDownList = null;
		this.mBakClick = null;
		this.mBakPopup = null;
		this.mArrow = null;
		this.mListBox = null;
		this._ItemCollection = null;
		this.mLnlDown = null;
		this.config = null;
	}

	public UIPopupList mPopupList;

	public GameObject mPopupDownList;

	public UISprite mBakClick;

	public UISprite mBakPopup;

	public UISprite mArrow;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public GameObject mLnlDown;

	private ZhanMengLianSaiGuanZhanPopupList.ParseGuanZhanTransfer config;

	public GameObject mFreedomModelObj;

	public GameObject mTrackModelObj;

	public GButton mBtnFreeModel;

	public GButton mBtnTrackModel;

	public GButton mBtnGetRoleList;

	public GameObject RolelistItemObj;

	public GameObject mRolelistPanel;

	public ListBox mRoleListBox;

	private ObservableCollection _RoleItemCollection;

	public ZhanMengLianSaiGuanZhanRoleListPart roleListPart;

	public GameObject LeaderObj;

	public TextBlock mLblKillDes;

	private Dictionary<string, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> transfDict = new Dictionary<string, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO>();

	public static string CurrentTrackName = string.Empty;

	private Transform leaderTransform;

	private Transform otherTransform;

	private GSprite sp;

	public int LastTrackingRoleID;

	public static bool IsTracking = false;

	private GameObject leader;

	private string otherSpriteName = string.Empty;

	private GameObject other;

	private int mTrackId;

	private UIFollowTarget trackTarget;

	private UIFollowTarget lasttrackTarget;

	private new GameObject parent;

	public class GuanZhanTransferVO
	{
		public GuanZhanTransferVO(XElement xml)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Type = Global.GetXElementAttributeInt(xml, "Type");
			this.HuoDongName = Global.GetXElementAttributeStr(xml, "HuoDongName");
			this.ChuanSongName = Global.GetXElementAttributeStr(xml, "ChuanSongName");
			this.MapCode = Global.GetXElementAttributeInt(xml, "MapCode");
			this.Site = Global.GetXElementAttributeStr(xml, "Site");
			this.BattlefieldID = Global.GetXElementAttributeInt(xml, "BattlefieldID");
		}

		public GuanZhanTransferVO(XElement xml, int type)
		{
			this.ID = 0;
			this.Type = 0;
			this.HuoDongName = null;
			this.ChuanSongName = Global.GetLang("传送点");
			this.MapCode = 0;
			this.Site = null;
			this.BattlefieldID = 0;
		}

		public int ID { get; set; }

		public int Type { get; set; }

		public string HuoDongName { get; set; }

		public string ChuanSongName { get; set; }

		public int MapCode { get; set; }

		public string Site { get; set; }

		public int BattlefieldID { get; set; }
	}

	public class ParseGuanZhanTransfer
	{
		public ParseGuanZhanTransfer(int type = 1)
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/GuanZhanTransfer.xml");
			if (isolateResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(isolateResXml, "ChuanSong");
				if (xelementList != null)
				{
					ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO guanZhanTransferVO = new ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO(null, 0);
					this.dic.Add(guanZhanTransferVO.ID, guanZhanTransferVO);
					for (int i = 0; i < xelementList.Count; i++)
					{
						ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO guanZhanTransferVO2 = new ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO(xelementList[i]);
						if (guanZhanTransferVO2.Type == type)
						{
							this.dic.Add(guanZhanTransferVO2.ID, guanZhanTransferVO2);
						}
					}
				}
			}
		}

		public Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> GuanZhanTransferDict
		{
			get
			{
				return this.dic;
			}
		}

		public ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO GetItem(int index)
		{
			if (this.dic == null || this.dic.Count <= 0)
			{
				return null;
			}
			return this.dic[index];
		}

		private const string path = "Config/GuanZhanTransfer.xml";

		private Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO> dic = new Dictionary<int, ZhanMengLianSaiGuanZhanPopupList.GuanZhanTransferVO>();
	}
}
