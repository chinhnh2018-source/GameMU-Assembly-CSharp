using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LuaPublicDialogPart : GBasePart
{
	public override void InitPartSize(int width, int height)
	{
	}

	public override void InitPartData()
	{
	}

	public override void CleanUpChildWindows()
	{
	}

	public int NpcExtensionID
	{
		get
		{
			return this._NpcExtensionID;
		}
		set
		{
			this._NpcExtensionID = value;
		}
	}

	public int NpcID
	{
		get
		{
			return this._NpcID;
		}
		set
		{
			this._NpcID = value;
		}
	}

	protected override void InitializeComponent()
	{
		this._TalkText.Label.EnabledInspectorWrap = true;
		this._Operation_Panel.transform.localPosition = new Vector3(300f, -65f, 0f);
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	protected void OnDisable()
	{
		if (null != this._Face)
		{
			this._Face.DestroyImmediateTexture();
		}
	}

	private void DestoryImg(ShowNetImage img)
	{
		if (null != img && null != img.Texture && img.Texture.mainTexture != null)
		{
			Object.Destroy(img.Texture.mainTexture);
			Object.DestroyImmediate(img.Texture.mainTexture, true);
			img.Texture.mainTexture = null;
		}
	}

	public void RefreshData(LuaCallResultData luaCallResultData)
	{
		if (luaCallResultData == null || luaCallResultData.NPCID != this.NpcID || luaCallResultData.ExtensionID != this.NpcExtensionID)
		{
			return;
		}
		this.CloseModalDialog();
		if ("keepopen" == luaCallResultData.Result)
		{
			return;
		}
		if ("closewindow" == luaCallResultData.Result)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -1,
					ID = -1
				});
			}
			return;
		}
		if (this.isExcutingFunc)
		{
			this.isExcutingFunc = false;
			this.GetNewData();
			return;
		}
		if (this.RefreshDataStatus && luaCallResultData.ForceRefresh <= 0)
		{
			return;
		}
		this.RefreshDataStatus = true;
		if (luaCallResultData.IsSuccess > 0)
		{
			if (luaCallResultData.Result != null)
			{
				this.HtmlText = luaCallResultData.Result;
			}
		}
		else
		{
			this._TalkText.Text = this.GetEquipmentTips();
		}
		if (this.NpcID - 2130706432 == 217)
		{
			base.InitHintDecoration(50007, new Point(86, 293), null);
		}
		else if (this.NpcID - 2130706432 == 215)
		{
			base.InitHintDecoration(50007, new Point(77, 266), null);
		}
		else if (this.NpcID - 2130706432 == 209)
		{
			base.InitHintDecoration(50007, new Point(77, 238), null);
		}
	}

	public string HtmlText
	{
		set
		{
			this.SetText(value);
		}
	}

	private void SetText(string xmlStr)
	{
		XElement xelement = XElement.Parse(xmlStr);
		if (xelement == null)
		{
			return;
		}
		XElement xelement2 = Global.GetXElement(xelement, "Item");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Title");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "Text");
		string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "Event");
		string[] array = xelementAttributeStr3.Split(new char[]
		{
			'|'
		});
		this._Title.Text = xelementAttributeStr;
		this._TalkText.Text = xelementAttributeStr2;
		if (array != null && array.Length == 2)
		{
			GLinkButton item = U3DUtils.NEW<GLinkButton>();
			this._Operation_List.ItemsSource.Add(item);
			item.ColiderSizeModule = 2f;
			item.FontSize = 21;
			item.Text = array[1];
			item.Tag = array[0];
			item.addEventListener("mouseDown", delegate(MouseEvent e)
			{
				this.linkHandle(item.Tag as string);
			});
		}
	}

	private void linkHandle(string evt)
	{
		string[] array = evt.Split(new char[]
		{
			':'
		});
		if (array != null && array.Length == 2)
		{
			evt = array[1];
		}
		if (this.PreDealLinkCmd(evt))
		{
			return;
		}
		GameInstance.Game.SpriteExcuteNpcLuaFunction(Global.Data.roleData.MapCode, this.NpcID, this.NpcExtensionID, 10, evt);
		if (evt.IndexOf("_") != 0)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -1,
					ID = -1
				});
			}
			return;
		}
		if (evt.IndexOf("_look") == 0)
		{
			this.RefreshDataStatus = false;
			return;
		}
		this.isExcutingFunc = true;
	}

	private bool PreDealLinkCmd(string cmdText)
	{
		if ("_mendequipment" == cmdText)
		{
			MouseEvent e = new MouseEvent("mouseUp", null);
			ObjectClickGetingMgr.StartClickGetThing(15, e);
			return true;
		}
		if ("_canclemendequipment" == cmdText)
		{
			ObjectClickGetingMgr.CancelClickGetThing(15);
			return true;
		}
		if ("_requestcitywar" == cmdText)
		{
			if (Global.IsHuangDi(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("【城主】自己不需要申请城战"), 0, -1, -1, 0);
				return true;
			}
			if (Global.Data.roleData.BHZhiWu != 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有首领才能申请城战"), 0, -1, -1, 0);
				return true;
			}
			GameInstance.Game.SpriteCityWarRequest();
			return true;
		}
		else if ("_getcityaward" == cmdText)
		{
			if (!Global.IsWangZuLeader(Global.Data.roleData))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("只有王城归属战盟的首领才能领取"), 0, -1, -1, 0);
				return true;
			}
			GameInstance.Game.SpriteTakeLingDiDailyAward(6);
			return true;
		}
		else
		{
			if (cmdText.IndexOf("_mergewin") == 0)
			{
				string[] array = cmdText.Split(new char[]
				{
					'!'
				});
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventMergeWin", array[1]));
				return true;
			}
			if (cmdText.IndexOf("_mallbuy") == 0)
			{
				string[] array2 = cmdText.Split(new char[]
				{
					'!'
				});
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventOpenMallBuyGoodsID", array2[1]));
				return true;
			}
			if (cmdText.IndexOf("_buynpc") == 0)
			{
				string[] array3 = cmdText.Split(new char[]
				{
					'!'
				});
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventOpenNPCSale", StringUtil.substitute("{0}:{1}", new object[]
				{
					array3[1],
					array3[2]
				})));
				return true;
			}
			if (cmdText.IndexOf("_entermap") == 0)
			{
				string[] array4 = cmdText.Split(new char[]
				{
					'!'
				});
				GameInstance.Game.SpriteGotToMap(Global.SafeConvertToInt32(array4[1]));
				return true;
			}
			return this.HandleLuaCmd != null && this.HandleLuaCmd(this, cmdText);
		}
	}

	public void OnUrlItemClick()
	{
		GameInstance.Game.SpriteExcuteNpcLuaFunction(Global.Data.roleData.MapCode, this.NpcID, this.NpcExtensionID, 10, "cmd");
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -1,
				ID = -1
			});
		}
	}

	public void GetNewData()
	{
		this.RefreshDataStatus = false;
		GameInstance.Game.SpriteClickOnNPCForLuaTalk(Global.Data.roleData.MapCode, this.NpcID, this.NpcExtensionID);
		this.ShowModalDialog();
		if (this.NpcID > 0)
		{
			int npcpicCodeByID = ConfigNPCs.GetNPCPicCodeByID(this.NpcExtensionID);
			this._Face.ShowImage(StringUtil.substitute("NetImages/NPCs/{0}.png", new object[]
			{
				Global.FormatStr("000", npcpicCodeByID)
			}));
		}
	}

	private string GetEquipmentTips()
	{
		return Global.GetLang("这个npc服务器好像没有配置脚本\\n\\n或者脚本配置错误了");
	}

	private void ClickGetThingNotify(ObjectClickEvent evt)
	{
		MouseClickEventArgs mouseClickEventArgs = evt.Tag as MouseClickEventArgs;
		if (mouseClickEventArgs.ClickGetThingType != 15)
		{
			return;
		}
		if (mouseClickEventArgs.Cancel)
		{
			return;
		}
		object sender = mouseClickEventArgs.sender;
		GIcon gicon = sender as GIcon;
		if (null == gicon)
		{
			return;
		}
		GoodsData goodsData = gicon.ItemObject as GoodsData;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy >= 0 && categoriy <= 25)
				{
					GameInstance.Game.SpriteMendEquipment(goodsData.Id);
				}
			}
		}
		mouseClickEventArgs.NextClick = true;
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public HandleLuaHander HandleLuaCmd;

	public GButton CloseBtn;

	public TextBlock _Title;

	public TextBlock _TalkText;

	public ShowNetImage _Face;

	public GameObject _Operation_Panel;

	public ListBox _Operation_List;

	private bool RefreshDataStatus;

	private bool isExcutingFunc;

	private int _NpcExtensionID;

	private int _NpcID = -1;
}
