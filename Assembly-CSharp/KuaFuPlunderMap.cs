using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class KuaFuPlunderMap
{
	private CrusadeWorldXML CrusadeWorldXML
	{
		get
		{
			if (this.mCrusadeWorldXML == null)
			{
				this.mCrusadeWorldXML = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWorldXMLInstance();
			}
			return this.mCrusadeWorldXML;
		}
	}

	public KuaFuLueDuoMainInfo KuaFuLueDuoMainInfo
	{
		get
		{
			return this.mKuaFuLueDuoMainInfo;
		}
	}

	public KuaFuLueDuoStateData KuaFuLueDuoStateData
	{
		get
		{
			return this.mKuaFuLueDuoStateData;
		}
		set
		{
			if (value != null)
			{
				bool flag = false;
				if (this.mKuaFuLueDuoStateData != null)
				{
					if (this.mKuaFuLueDuoStateData.Age != value.Age)
					{
						flag = true;
						this.mKuaFuLueDuoStateData = value;
					}
				}
				else
				{
					flag = true;
					this.mKuaFuLueDuoStateData = value;
				}
				this.mKuaFuLueDuoGameStates = this.mKuaFuLueDuoStateData.GameState;
				if (flag && this.dicBuildNpc.ContainsKey(this.mKuaFuLueDuoStateData.ServerID.ToString()))
				{
					this.dicBuildNpc[this.mKuaFuLueDuoStateData.ServerID.ToString()].ShowInf(true);
				}
			}
		}
	}

	public KuaFuLueDuoGameStates KuaFuLueDuoGameStates
	{
		get
		{
			return this.mKuaFuLueDuoGameStates;
		}
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer != null)
		{
			this.mDispatcherTimer.Stop();
			this.mDispatcherTimer.Dispose();
			this.mDispatcherTimer = null;
		}
	}

	private void StartTimeTicks()
	{
		this.StopTimeTicks();
		this.mDispatcherTimer = null;
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		if (this.mLastRefreshDataTicks == 0L)
		{
			this.mLastRefreshDataTicks = Global.GetCorrectLocalTime();
		}
		long num = Global.GetCorrectLocalTime() - this.mLastRefreshDataTicks;
		if (8L <= num / 1000L)
		{
			this.RefreashMainInfoData();
			this.mLastRefreshDataTicks = Global.GetCorrectLocalTime();
		}
	}

	public static KuaFuPlunderMap GetInstance()
	{
		if (KuaFuPlunderMap.instanceMap == null)
		{
			KuaFuPlunderMap.instanceMap = new KuaFuPlunderMap();
		}
		return KuaFuPlunderMap.instanceMap;
	}

	public static void ClearStaticData()
	{
		if (KuaFuPlunderMap.instanceMap != null)
		{
			KuaFuPlunderMap.instanceMap.DestroyMapScene();
			KuaFuPlunderMap.instanceMap = null;
		}
	}

	public void EnterMapScene()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (mapSceneUIClass != this.mKuaFuPlunderBenClass)
		{
			this.MapChange();
		}
		if (mapSceneUIClass != SceneUIClasses.KuaFuPlunderMap)
		{
			return;
		}
		PlayZone.GlobalPlayZone.OpenKuaFuPlunderMapMainWindow();
		this.RefreashMainInfoData();
		this.StartTimeTicks();
	}

	public void RefreashMainInfoData()
	{
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(this.mKuaFuLueDuoMainInfo.ServerListAge, this.mKuaFuLueDuoMainInfo.StateListAge);
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
		}
		if (this.mKuaFuLueDuoStateData != null)
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(this.mKuaFuLueDuoStateData.Age);
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}
	}

	public void DestroyMapScene()
	{
		if (this.localEulerAnglesBak.Key)
		{
			Global.MainCamera.transform.localEulerAngles = (Vector3)this.localEulerAnglesBak.Value[0];
			Global.MainCamera.fieldOfView = (float)this.localEulerAnglesBak.Value[1];
			Global.MainCamera.transform.localPosition = (Vector3)this.localEulerAnglesBak.Value[2];
			Global.MainCamera.transform.localRotation = (Quaternion)this.localEulerAnglesBak.Value[3];
			Global.MainCamera.transform.localPosition = (Vector3)this.localEulerAnglesBak.Value[4];
		}
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.CloseKuaFuPlunderMapMainWindow();
		}
		if (0 < this.dicBuildNpc.Count)
		{
			foreach (KuaFuPlunderMapObj kuaFuPlunderMapObj in this.dicBuildNpc.Values)
			{
				if (null != kuaFuPlunderMapObj.kuaFuPlunderMapObjFace)
				{
					Object.Destroy(kuaFuPlunderMapObj.kuaFuPlunderMapObjFace.gameObject);
				}
				if (kuaFuPlunderMapObj.modelObject != null)
				{
					kuaFuPlunderMapObj.modelObject.Destroy();
				}
			}
			this.dicBuildNpc.Clear();
		}
		if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderMap)
		{
			IConfigbase<ConfigKuaFuPlunder>.Instance.DisposadeCrusadeWorldXml();
		}
		this.StopTimeTicks();
		this.mKuaFuLueDuoMainInfo = null;
		this.mKuaFuLueDuoStateData = null;
	}

	public void MapChange()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (KuaFuPlunderMap.preUIClass == mapSceneUIClass)
		{
			return;
		}
		if (KuaFuPlunderMap.preUIClass == SceneUIClasses.KuaFuPlunderMap)
		{
			KuaFuPlunderMap.preUIClass = Global.GetMapSceneUIClass();
			this.SetGameUIVisiable(false);
			this.DestroyMapScene();
			return;
		}
		if (mapSceneUIClass == SceneUIClasses.KuaFuPlunderMap)
		{
			this.SetGameUIVisiable(true);
		}
		KuaFuPlunderMap.preUIClass = Global.GetMapSceneUIClass();
	}

	public void ResetCamera()
	{
		if (!this.localEulerAnglesBak.Key)
		{
			bool flag = true;
			ArrayList arrayList = new ArrayList(2);
			arrayList.Add(Global.MainCamera.transform.localEulerAngles);
			arrayList.Add(Global.MainCamera.fieldOfView);
			this.localEulerAnglesBak = new KeyValuePair<bool, ArrayList>(flag, arrayList);
		}
		Global.MainCamera.gameObject.SetActive(true);
		Global.MainCamera.transform.position = new Vector3(25.1f, 64.1f, 9.1f);
		Global.MainCamera.transform.localEulerAngles = new Vector3(46.44f, 0f, 0f);
		Global.MainCamera.fieldOfView = 45f;
	}

	private void SetGameUIVisiable(bool enter)
	{
		Transform transform = PlayZone.GlobalPlayZone.transform;
		if (enter)
		{
			this.pzChildDic.Clear();
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.StartsWith("GChildWindow"))
				{
					child.gameObject.SetActive(false);
				}
				else if (child.name.StartsWith("GetGoodsHintPart(Clone)"))
				{
					child.gameObject.SetActive(true);
				}
				else if (child.name.StartsWith("RoleExpProgressBar(Clone)"))
				{
					child.gameObject.SetActive(true);
				}
				else
				{
					this.pzChildDic[child.name] = child.gameObject.activeSelf;
					child.gameObject.SetActive(false);
				}
			}
			Global.Joystick.gameObject.SetActive(false);
		}
		else
		{
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child2 = transform.GetChild(j);
				if (child2.name.StartsWith("GChildWindow"))
				{
					child2.gameObject.SetActive(false);
				}
				else if (this.pzChildDic.ContainsKey(child2.name))
				{
					child2.gameObject.SetActive(this.pzChildDic[child2.name]);
				}
			}
			Global.Joystick.gameObject.SetActive(true);
		}
	}

	private void RefreshCurrentPos()
	{
		if (null != this.mCameraControllerDrag)
		{
			Vector3 cameraPos = this.mCameraControllerDrag.CameraPos;
			if (Vector3.zero == cameraPos)
			{
				this.mCurrentPos = this.mCameraControllerDrag.transform.localPosition;
			}
			else
			{
				Ray ray = Global.MainCamera.ScreenPointToRay(cameraPos);
				float y = cameraPos.y;
				this.mCurrentPos = ray.GetPoint(y);
			}
			this.RefreshMode(this.mRound);
		}
	}

	private bool GetObjIsOwnServer(GameObject go)
	{
		return this.mKuaFuLueDuoStateData != null && go.name.SafeToInt32(0) == this.mKuaFuLueDuoStateData.ServerID;
	}

	public void NpcLoaderComplete(GSprite spr, MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		NPCInfoDisplay component = go.GetComponent<NPCInfoDisplay>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		if (this.dicBuildNpc.ContainsKey(go.name))
		{
			this.dicBuildNpc[go.name].RefreshFaceTag(go);
			this.dicBuildNpc[go.name].RefreshInf(go.name, (int)this.dicBuildNpc[go.name].RoundType);
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(this.dicBuildNpc[go.name].NPCID);
			if (npcvobyID != null)
			{
				KuaFuPlunderMap.ReplaceMaterials(go, npcvobyID.ShaderID);
			}
		}
		if (this.GetObjIsOwnServer(go))
		{
			U3DUtils.AddCameraController(go);
			this.mCameraControllerDrag = go.GetComponent<CameraControllerDrag>();
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeMap", ',');
			if (systemParamIntArrayByName != null && 4 <= systemParamIntArrayByName.Length)
			{
				this.mCameraControllerDrag.SetCamPositionRec(new Vector4((float)systemParamIntArrayByName[0] / 100f, (float)systemParamIntArrayByName[1] / 100f, (float)systemParamIntArrayByName[2] / 100f, (float)systemParamIntArrayByName[3] / 100f));
			}
			else
			{
				this.mCameraControllerDrag.SetCamPositionRec(new Vector4(89f, 70f, 12.1f, 20.8f));
			}
			bool flag = true;
			ArrayList arrayList = new ArrayList(2);
			arrayList.Add(Global.MainCamera.transform.localEulerAngles);
			arrayList.Add(Global.MainCamera.fieldOfView);
			arrayList.Add(Global.MainCamera.transform.localPosition);
			arrayList.Add(Global.MainCamera.transform.localRotation);
			arrayList.Add(Global.MainCamera.transform.localScale);
			this.localEulerAnglesBak = new KeyValuePair<bool, ArrayList>(flag, arrayList);
			this.mCameraControllerDrag.SetCaneraProperty(Vector3.zero, Quaternion.Euler(56.5712f, 0.753479f, 0.8384705f), Vector3.one);
			this.mCameraControllerDrag.RoleDragScreen = delegate(object e, DPSelectedItemEventArgs s)
			{
				this.RefreshCurrentPos();
			};
			this.RefreshCurrentPos();
		}
	}

	public static void ReplaceMaterials(GameObject go, int shaderID)
	{
		if (go == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				Global.GetLang("ReplaceMaterials对象为空,不应该为空")
			});
			return;
		}
		Renderer[] componentsInChildren;
		if (shaderID <= 0)
		{
			componentsInChildren = go.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
					for (int j = 0; j < sharedMaterials.Length; j++)
					{
						Texture texture = sharedMaterials[j].GetTexture("_MainTex");
						if (texture.name.IndexOf("_alpha") >= 0)
						{
							sharedMaterials[j].shader = Shader.Find("Custom/Mobile/Diffuse");
						}
						else
						{
							sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterial.shader.name);
						}
					}
				}
			}
			return;
		}
		componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
		if (componentsInChildren != null)
		{
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				Renderer renderer = componentsInChildren[k];
				if (!(null == renderer))
				{
					if (!(renderer is ParticleRenderer) && !(renderer is ParticleSystemRenderer))
					{
						Material[] array = new Material[renderer.sharedMaterials.Length];
						for (int l = 0; l < renderer.sharedMaterials.Length; l++)
						{
							if (k != 0 && (renderer.sharedMaterials[l].shader.name.StartsWith("FXMaker/Mask") || renderer.sharedMaterials[l].shader.name.StartsWith("Custom/Mobile/Particles") || renderer.sharedMaterials[l].shader.name.StartsWith("Mobile/Particles/Additive Culled") || renderer.sharedMaterials[l].shader.name.StartsWith("Artist/Tint Particle") || renderer.sharedMaterials[l].shader.name.StartsWith("ZombieStyle/MobileRimDiffuseCutoutAlpha")))
							{
								array[l] = renderer.sharedMaterials[l];
							}
							else
							{
								Texture texture2 = renderer.sharedMaterials[l].GetTexture("_MainTex");
								Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[l], shaderID, texture2.name.IndexOf("_alpha") >= 0);
								if (null == materialReflByShaderID)
								{
									array[l] = materialReflByShaderID;
								}
								else
								{
									materialReflByShaderID.SetTexture("_MainTex", texture2);
									array[l] = materialReflByShaderID;
								}
							}
						}
						componentsInChildren[k].materials = array;
					}
				}
			}
		}
	}

	public int GetServerModeDIrByServerID(int ServerID)
	{
		if (this.dicBuildNpc.ContainsKey(ServerID.ToString()))
		{
			return this.dicBuildNpc[ServerID.ToString()].ModeDir;
		}
		return 0;
	}

	public byte GetModalTypeByServerId(int ServerID)
	{
		if (this.dicBuildNpc.ContainsKey(ServerID.ToString()))
		{
			return this.dicBuildNpc[ServerID.ToString()].ModeType;
		}
		return 0;
	}

	public GSprite CreateBuildNPC(int npcID, float posX, float posY, GSpriteTypes spriteType, string resname, string modelObjectName, int dir)
	{
		int mapCode = Global.Data.roleData.MapCode;
		string text = string.Concat(new object[]
		{
			"<NPC ID=\"",
			npcID,
			"\" Code=\"\" PicCode=\"\" ResName=\"",
			resname,
			"\" YouShou=\"\" ZuoShou=\"\" Wing=\"\" FlyPet=\"\" Scale=\"1.1\" ShaderID=\"\" GuaJieDian=\"\" GuaJieTeXiao=\"\" Function=\"药店\" SName=\"莱雅\" MapCode=\"1\" Talk=\"勇士\" Sex=\"1\" Tasks=\"\" Operations=\"0\" SaleID=\"3,31\" LuaScriptFile=\"\" Scripts=\"-1\" Display=\"1\" PlaySound=\"\" Interval=\"\" TakeSound=\"\" Collide=\"\" Obstacle=\"\" IsSafe=\"2\" />"
		});
		XElement npcXmlNode = XElement.Parse(text);
		int num = (int)posX / Global.Data.GameScene.CurrentMapData.GridSizeX;
		int num2 = (int)posY / Global.Data.GameScene.CurrentMapData.GridSizeY;
		return this._AddNewNPCEx(npcXmlNode, (double)posX, (double)posY, dir, spriteType, resname, modelObjectName);
	}

	private GSprite _AddNewNPCEx(XElement npcXmlNode, double posX, double posY, int dir, GSpriteTypes spriteType, string resname, string modelObjectName)
	{
		int roleID = 2130706432 + Global.GetXElementAttributeInt(npcXmlNode, "ID");
		GSprite gsprite = new GSprite();
		gsprite.SpriteType = GSpriteTypes.NPC;
		gsprite.CreateByClient = true;
		string xelementAttributeStr = Global.GetXElementAttributeStr(npcXmlNode, "SName");
		this._LoadSprite(gsprite, roleID, Global.GetXElementAttributeInt(npcXmlNode, "Sex"), modelObjectName, string.Empty, string.Empty, xelementAttributeStr, 0, Global.GetXElementAttributeInt(npcXmlNode, "ID"), 65280U, 65280U, 65280U, 0.0, 0, 0, -1.0, Global.GetXElementAttributeInt(npcXmlNode, "Code"), -1, new Point((int)posX, (int)posY), dir, 0.0, 0.0, 0, false);
		int xelementAttributeInt = Global.GetXElementAttributeInt(npcXmlNode, "ID");
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(100);
		string resName = npcvobyID.ResName;
		if (spriteType == GSpriteTypes.Monster)
		{
			gsprite.ExtensionID = xelementAttributeInt;
			gsprite.SpriteType = spriteType;
			npcvobyID.ResName = resname;
		}
		else if (spriteType == GSpriteTypes.NPC)
		{
			gsprite.ExtensionID = xelementAttributeInt;
			gsprite.SpriteType = spriteType;
			npcvobyID.ResName = resname;
		}
		gsprite.Start();
		gsprite.SpriteType = GSpriteTypes.NPC;
		npcvobyID.ResName = resName;
		gsprite.IsOpposition = false;
		Global.CurrentMapData._MapGrid.MoveObject(-1, -1, gsprite.cx, gsprite.cy, gsprite);
		return gsprite;
	}

	private GSprite _LoadSprite(GSprite sprite, int roleID, int roleSex, string name, string faction, string otherName, string sname, int occupation, int extensionID, uint factionColor, uint otherNameColor, uint snameColor, double life, int pkMode, int pkValue, double currentMagic, int equipmentBody, int equipmentWeapon, Point coordinate, int direction, double lifeTotalWidth, double moveSpeed, int factionID, bool addToCanvas)
	{
		sprite.Name = name;
		sprite.RoleID = roleID;
		sprite.RoleSex = roleSex;
		sprite.FactionID = factionID;
		if (!string.IsNullOrEmpty(faction))
		{
			sprite.VFaction = faction;
		}
		if (!string.IsNullOrEmpty(otherName))
		{
			sprite.VOtherName = otherName;
		}
		sprite.VSName = sname;
		sprite.ShowName = Global.FormatShowName(Global.Data.roleData, 0);
		sprite.Occupation = occupation;
		sprite.ExtensionID = extensionID;
		sprite.SNameBrush = new SolidColorBrush(snameColor);
		sprite.VLife = life;
		sprite.PKMode = (GPKModes)pkMode;
		sprite.CurrentMagic = (int)currentMagic;
		sprite.EquipmentBody = equipmentBody;
		sprite.EquipmentWeapon = equipmentWeapon;
		sprite.cx = coordinate.X;
		sprite.cy = coordinate.Y;
		sprite.OrigCoordinate = coordinate;
		sprite.Direction = direction;
		sprite.LifeTotalWidth = lifeTotalWidth;
		sprite.SpriteMoveSpeed = moveSpeed;
		if (addToCanvas)
		{
			ObjectsManager.Add(sprite);
		}
		sprite.Action = GActions.Stand;
		return sprite;
	}

	public void NoticeGetSeverDataCallBack(KuaFuLueDuoMainInfo data)
	{
		bool flag = false;
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mKuaFuLueDuoMainInfo.ServerListAge != data.ServerListAge)
			{
				flag = true;
				this.mKuaFuLueDuoMainInfo.ServerList = data.ServerList;
				this.mKuaFuLueDuoMainInfo.ServerListAge = data.ServerListAge;
			}
			if (this.mKuaFuLueDuoMainInfo.StateListAge != data.StateListAge)
			{
				flag = true;
				this.mKuaFuLueDuoMainInfo.StateList = data.StateList;
				this.mKuaFuLueDuoMainInfo.JingJiaData = data.JingJiaData;
				this.mKuaFuLueDuoMainInfo.StateListAge = data.StateListAge;
			}
		}
		else if (data != null)
		{
			flag = true;
			this.mKuaFuLueDuoMainInfo = data;
		}
		if (flag)
		{
			KuaFuLueDuoBangHuiJingJiaData jingJiaData = this.mKuaFuLueDuoMainInfo.JingJiaData;
			List<KuaFuLueDuoServerJingJiaState> cloneStateList = this.mKuaFuLueDuoMainInfo.CloneStateList;
			cloneStateList.Sort((KuaFuLueDuoServerJingJiaState a, KuaFuLueDuoServerJingJiaState b) => b.ZiYuan - a.ZiYuan);
			Dictionary<int, KuaFuPlunderMap.ServerData> dictionary = new Dictionary<int, KuaFuPlunderMap.ServerData>();
			for (int i = 0; i < cloneStateList.Count; i++)
			{
				KuaFuPlunderMap.ServerData serverData = default(KuaFuPlunderMap.ServerData);
				serverData.Rank = i + 1;
				serverData.ResValue = cloneStateList[i].ZiYuan;
				dictionary[cloneStateList[i].ServerId] = serverData;
			}
			List<KuaFuLueDuoServerInfo> serverList = this.mKuaFuLueDuoMainInfo.ServerList;
			if (serverList != null)
			{
				for (int j = 0; j < serverList.Count; j++)
				{
					int type = 0;
					if (dictionary.ContainsKey(serverList[j].ServerId))
					{
						if (0 >= dictionary[serverList[j].ServerId].ResValue)
						{
							type = 3;
						}
						else if (0 < dictionary[serverList[j].ServerId].Rank && dictionary[serverList[j].ServerId].Rank <= 10)
						{
							type = 0;
						}
						else if (10 < dictionary[serverList[j].ServerId].Rank && 20 >= dictionary[serverList[j].ServerId].Rank)
						{
							type = 1;
						}
						else if (20 < dictionary[serverList[j].ServerId].Rank && 40 >= dictionary[serverList[j].ServerId].Rank)
						{
							type = 2;
						}
						else
						{
							type = 3;
						}
					}
					if (this.dicBuildNpc.ContainsKey(serverList[j].ServerId.ToString()))
					{
						CrusadeWorldVO vobyId = this.CrusadeWorldXML.GetVOById(j + 1);
						if (vobyId != null)
						{
							this.dicBuildNpc[serverList[j].ServerId.ToString()].RerfreshMode(vobyId.X, vobyId.Y, vobyId.Dir, type);
							int type2 = 0;
							KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaStateDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(serverList[j].ServerId);
							if (kuaFuLueDuoServerJingJiaStateDataByID != null)
							{
								this.dicBuildNpc[serverList[j].ServerId.ToString()].Round = kuaFuLueDuoServerJingJiaStateDataByID.Round;
								if (jingJiaData != null && serverList[j].ServerId == jingJiaData.ServerId && 0 < jingJiaData.ZiJin)
								{
									if (kuaFuLueDuoServerJingJiaStateDataByID.State == 1)
									{
										type2 = 1;
									}
									else if (kuaFuLueDuoServerJingJiaStateDataByID.State == 2)
									{
										type2 = 2;
									}
								}
							}
							this.dicBuildNpc[serverList[j].ServerId.ToString()].RefreshInf(serverList[j].ServerId.ToString(), type2);
						}
					}
					else
					{
						KuaFuPlunderMapObj kuaFuPlunderMapObj = new KuaFuPlunderMapObj();
						kuaFuPlunderMapObj.ServerID = serverList[j].ServerId;
						CrusadeWorldVO vobyId2 = this.CrusadeWorldXML.GetVOById(j + 1);
						if (vobyId2 != null)
						{
							this.dicBuildNpc.Add(kuaFuPlunderMapObj.ServerID.ToString(), kuaFuPlunderMapObj);
							if (this.mKuaFuLueDuoStateData != null)
							{
								kuaFuPlunderMapObj.RerfreshMode(vobyId2.X, vobyId2.Y, vobyId2.Dir, serverList[j].ServerId == this.mKuaFuLueDuoStateData.ServerID, type);
							}
							else
							{
								kuaFuPlunderMapObj.RerfreshMode(vobyId2.X, vobyId2.Y, vobyId2.Dir, false, type);
							}
							kuaFuPlunderMapObj.RefreshInf(serverList[j].ServerId.ToString(), type);
							kuaFuPlunderMapObj.RefreshPos(false);
							int type3 = 0;
							KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaStateDataByID2 = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(serverList[j].ServerId);
							if (kuaFuLueDuoServerJingJiaStateDataByID2 != null)
							{
								kuaFuPlunderMapObj.Round = kuaFuLueDuoServerJingJiaStateDataByID2.Round;
								if (jingJiaData != null && serverList[j].ServerId == jingJiaData.ServerId && 0 < jingJiaData.ZiJin)
								{
									if (kuaFuLueDuoServerJingJiaStateDataByID2.State == 1)
									{
										type3 = 1;
									}
									else if (kuaFuLueDuoServerJingJiaStateDataByID2.State == 2)
									{
										type3 = 2;
									}
								}
							}
							this.dicBuildNpc[serverList[j].ServerId.ToString()].RefreshInf(serverList[j].ServerId.ToString(), type3);
						}
					}
				}
			}
		}
	}

	public void NoticeRefreshData()
	{
	}

	public void RefreshMode(int Round = -1)
	{
		this.mRound = Round;
		foreach (KeyValuePair<string, KuaFuPlunderMapObj> keyValuePair in this.dicBuildNpc)
		{
			if (keyValuePair.Value != null)
			{
				Dictionary<string, KuaFuPlunderMapObj>.Enumerator enumerator;
				KeyValuePair<string, KuaFuPlunderMapObj> keyValuePair2 = enumerator.Current;
				keyValuePair2.Value.CamePos = this.mCurrentPos;
				if (Round != -1)
				{
					KeyValuePair<string, KuaFuPlunderMapObj> keyValuePair3 = enumerator.Current;
					KuaFuPlunderMapObj value = keyValuePair3.Value;
					KeyValuePair<string, KuaFuPlunderMapObj> keyValuePair4 = enumerator.Current;
					value.ShowInf(keyValuePair4.Value.Round == Round);
				}
				else
				{
					KeyValuePair<string, KuaFuPlunderMapObj> keyValuePair5 = enumerator.Current;
					keyValuePair5.Value.ShowInf(true);
				}
			}
		}
	}

	private KeyValuePair<bool, ArrayList> localEulerAnglesBak = new KeyValuePair<bool, ArrayList>(false, null);

	private Vector3 mCurrentPos = Vector3.one;

	private CameraControllerDrag mCameraControllerDrag;

	private static KuaFuPlunderMap instanceMap;

	private DispatcherTimer mDispatcherTimer;

	private CrusadeWorldXML mCrusadeWorldXML;

	private KuaFuLueDuoMainInfo mKuaFuLueDuoMainInfo;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;

	private KuaFuLueDuoGameStates mKuaFuLueDuoGameStates = 4;

	private long mLastRefreshDataTicks;

	public SceneUIClasses mKuaFuPlunderBenClass = SceneUIClasses.KuaFuPlunderMap;

	private static SceneUIClasses preUIClass;

	private Dictionary<string, bool> pzChildDic = new Dictionary<string, bool>();

	private Dictionary<string, KuaFuPlunderMapObj> dicBuildNpc = new Dictionary<string, KuaFuPlunderMapObj>();

	private int mRound = -1;

	private struct ServerData
	{
		public int Rank;

		public int ResValue;
	}
}
