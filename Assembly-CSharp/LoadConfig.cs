using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class LoadConfig : TTMonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(this);
		this.StartDownloadGameRes();
	}

	private void StartDownloadGameRes()
	{
		base.StartCoroutine<bool>(this.InitGameRes(), new TTMonoBehaviour.CoroutineExceptionHandler(this.CoroutineException));
		if (YongHuXieYi.xmlEle == null)
		{
			base.StartCoroutine<bool>(this.InitExtralRes());
		}
	}

	private void CoroutineException()
	{
		GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载程序参数文件失败...")));
	}

	private IEnumerator InitGameRes()
	{
		string url = Global.WebPath(StringUtil.substitute("GameSite/{0}/{1}", new object[]
		{
			Global.Data.GamePingTaiID,
			"GameRes.unity3d"
		}));
		WWW www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载程序参数文件失败...")));
			yield break;
		}
		AssetBundleManager.AddAssetBundle("GameRes", www.assetBundle);
		url = Global.WebPath(StringUtil.substitute("GameSite/{0}/{1}", new object[]
		{
			Global.Data.GamePingTaiID,
			"GameRes_VO.unity3d"
		}));
		bool bExistVOFile = false;
		www.Dispose();
		www = null;
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载程序参数文件失败...")));
		}
		else
		{
			AssetBundleManager.AddAssetBundle("GameRes_VO", www.assetBundle);
			bExistVOFile = true;
		}
		ConfigGoods.PreCacheGoodsXmlNodes();
		ConfigMonsters.PreCacheMonsterXmlNodes();
		ConfigNPCs.PreCacheNPCVOs();
		LoadURLConfigManager.GetInstance().ParseXml();
		Global.LoadLangDict(false);
		this.count++;
		if (bExistVOFile)
		{
			AssetBundleManager.RemoveAssetBundle("GameRes_VO");
			if (null != www.assetBundle)
			{
				www.assetBundle.Unload(true);
			}
		}
		www.Dispose();
		www = null;
		url = Global.WebPath(StringUtil.substitute("GameSite/{0}/ServerRes/{1}/{2}", new object[]
		{
			Global.Data.GamePingTaiID,
			Global.IsolateResID,
			"IsolateRes.unity3d"
		}));
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载分区参数文件失败...")));
			yield break;
		}
		AssetBundleManager.AddAssetBundle("IsolateRes", www.assetBundle);
		url = Global.WebPath(StringUtil.substitute("GameSite/{0}/ServerRes/{1}/{2}", new object[]
		{
			Global.Data.GamePingTaiID,
			Global.IsolateResID,
			"IsolateRes_VO.unity3d"
		}));
		bExistVOFile = false;
		www.Dispose();
		www = null;
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载分区参数文件失败...")));
		}
		else
		{
			AssetBundleManager.AddAssetBundle("IsolateRes_VO", www.assetBundle);
			bExistVOFile = true;
		}
		ConfigTasks.PreCacheTaskXmlNodesEx();
		ActivityTipManager.InitActivityItemTree();
		if (bExistVOFile)
		{
			AssetBundleManager.RemoveAssetBundle("IsolateRes_VO");
			www.assetBundle.Unload(true);
		}
		www.Dispose();
		www = null;
		this.count++;
		url = Global.WebPath(StringUtil.substitute("MonsterSpeed.xml", new object[0]));
		www = new WWW(url);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			GError.AddErrMsg(string.Format("{0}", Global.GetLang("因为网络原因下载怪物速度文件失败...")));
			yield break;
		}
		DataObject.Instance.Clear();
		string content = Global.GetUTF8StringFromBytes(www.bytes);
		XElement xml = XElement.Parse(content);
		List<XElement> monsterList = Global.GetXElementList(xml, "Monster");
		for (int i = 0; i < monsterList.Count; i++)
		{
			DataObject.Instance.SetMonsterSpeed(Global.GetXElementAttributeInt(monsterList[i], "ID"), (float)Global.GetXElementAttributeDouble(monsterList[i], "Speed"));
		}
		www.Dispose();
		www = null;
		this.count++;
		if (this.count > 2)
		{
			Object.Destroy(base.gameObject);
		}
		yield break;
	}

	private IEnumerator InitExtralRes()
	{
		string _configurl = Global.WebPath(StringUtil.substitute("UserTreaty.xml", new object[0]));
		WWW www = new WWW(_configurl);
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			MUDebug.LogError<string>(new string[]
			{
				"no UserTreaty.xml"
			});
			yield break;
		}
		string content = Global.GetUTF8StringFromBytes(www.bytes);
		YongHuXieYi.xmlEle = XElement.Parse(content);
		www.Dispose();
		www = null;
		yield break;
	}

	public static void ClearConfigData()
	{
		AssetBundle assetBundle = AssetBundleManager.GetAssetBundle("GameRes");
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
		}
		AssetBundleManager.RemoveAssetBundle("GameRes");
		assetBundle = AssetBundleManager.GetAssetBundle("GameRes_VO");
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
		}
		AssetBundleManager.RemoveAssetBundle("GameRes_VO");
		assetBundle = AssetBundleManager.GetAssetBundle("IsolateRes");
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
		}
		AssetBundleManager.RemoveAssetBundle("IsolateRes");
		assetBundle = AssetBundleManager.GetAssetBundle("IsolateRes_VO");
		if (assetBundle != null)
		{
			assetBundle.Unload(true);
		}
		AssetBundleManager.RemoveAssetBundle("IsolateRes_VO");
		ConfigManager.LoadConfig();
		ConfigMonsters.ClearData();
		ConfigNPCs.ClearData();
		ConfigTasks.ClearData();
		ActivityTipManager.ClearData();
		ConfigSystemParam.ClearData();
		DataObject.Instance.Clear();
		ConfigGoodsObtain.ClearData();
		ConfigArmyGroupLegions.ClearData();
		ConfigChengJiu.ClearData();
		ConfigExtPropIndexes.ClearData();
		ConfigFashion.ClearData();
		ConfigGoodsObtain.ClearData();
		ConfigMagicInfos.ClearData();
		ConfigMagics.ClearData();
		ConfigReplaceGoodVO.ClearData();
		ConfigSettings.ClearData();
		ConfigSpecialPrompt.ClearData();
		ConfigSystemOpen.ClearData();
		ConfigYuanSuJueXing.instance.ClearData();
		Global.ClearXmlDataOnLoadConfig();
		ParseJuHunConfig.ClearXMLData();
		LoversWishPart.ClearXMLDataOnLoadConfig();
		if (null != PlayZone.GlobalPlayZone)
		{
			PlayZone.GlobalPlayZone.ClearXmlData();
		}
		RedPointManager.ClearData();
		RiChangPaTaPart.ClearXMLData();
		ShenQiManager.Clear();
		ShiLiData.CleanShiLiXMLData();
		JueXingData.CleanJueXingXMLData();
		SpiritTrackPart.ClearXMLData();
		ZuduiFubenKuaFuPart.XElementMoRiShenPan = null;
		Super.AutoSystemChatItemsArray = null;
		Super.ClearXMLData();
		SystemHelpMgr.ClearXMLData();
		ConfigBaoXiangTips.ClearData();
		AnnouncedSetVO.ClearXMLData();
		ConfigVersionSystemOpen.ClearData();
		TeamCompeteDataManager.ClearInivteDatas();
		TeamCompeteDataManager.ClearRequestJoinDatas();
		TeamCompeteDataManager.Clear();
		DaTaoShaDataManager.Clear();
	}

	public int count;
}
