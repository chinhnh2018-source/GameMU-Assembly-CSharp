using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using UnityEngine;

public class DownLoadMapController : UserControl
{
	public static DownLoadMapController GetInstance()
	{
		return DownLoadMapController.instance;
	}

	private new void Awake()
	{
		DownLoadMapController.instance = this;
	}

	public void StartDownloadMap()
	{
	}

	private void DownloadNextMap()
	{
		this.StartDownloadMap();
	}

	private void StopDownloadMap()
	{
		base.StopCoroutine("DownloadMap");
	}

	public void StopDownloadMapByExternal()
	{
	}

	private bool IsWIFI()
	{
		return 2 == Application.internetReachability;
	}

	private IEnumerator DownloadMap()
	{
		List<DownLoadData> downLoadInfos = new List<DownLoadData>();
		List<int> monsertidList = Global.GetMonsterListByMapCod(this.MapCode);
		if (monsertidList != null)
		{
			for (int i = 0; i < monsertidList.Count; i++)
			{
				DownLoadData data = new DownLoadData();
				data.type = DownLoadType.Monster;
				data.Key = this.MapCode;
				data.Value = monsertidList[i];
				if (!downLoadInfos.Contains(data))
				{
					downLoadInfos.Add(data);
				}
			}
		}
		List<int> mNPCIdList = Global.GetNPCListByMapCod(this.MapCode);
		if (mNPCIdList != null && mNPCIdList.Count > 0)
		{
			for (int j = 0; j < mNPCIdList.Count; j++)
			{
				DownLoadData data2 = new DownLoadData();
				data2.type = DownLoadType.NPC;
				data2.Key = this.MapCode;
				data2.Value = mNPCIdList[j];
				if (!downLoadInfos.Contains(data2))
				{
					downLoadInfos.Add(data2);
				}
			}
		}
		if (monsertidList != null && monsertidList.Count > 0)
		{
			for (int k = 0; k < monsertidList.Count; k++)
			{
				List<int> mDecoIdList = Global.GetMonsterDecoId(monsertidList[k]);
				if (mDecoIdList != null && mDecoIdList.Count > 0)
				{
					for (int l = 0; l < mDecoIdList.Count; l++)
					{
						DownLoadData data3 = new DownLoadData();
						data3.type = DownLoadType.Decoration;
						data3.Key = this.MapCode;
						data3.Value = mDecoIdList[l];
						if (!downLoadInfos.Contains(data3))
						{
							downLoadInfos.Add(data3);
						}
					}
				}
			}
		}
		DownLoadData mapData = new DownLoadData();
		mapData.Key = this.MapCode;
		mapData.Value = this.MapCode;
		mapData.type = DownLoadType.Map;
		if (!downLoadInfos.Contains(mapData))
		{
			downLoadInfos.Add(mapData);
		}
		float fileSize = 0f;
		float ReadySize = 0f;
		for (int m = 0; m < downLoadInfos.Count; m++)
		{
			if (downLoadInfos[m].type == DownLoadType.Map)
			{
				SettingMapVO vo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[m].Key);
				fileSize += (float)vo.FileSize;
			}
			else if (downLoadInfos[m].type == DownLoadType.Monster)
			{
				fileSize += 800f;
			}
			else if (downLoadInfos[m].type == DownLoadType.NPC)
			{
				fileSize += 800f;
			}
			else if (downLoadInfos[m].type == DownLoadType.Decoration)
			{
				fileSize += 80f;
			}
		}
		if ((float)(QMQJJava.GetLeftAvailableMemery() * 1024L * 1024L) < fileSize)
		{
			string needSpace = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				fileSize + "KB"
			});
			string speceType = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
			});
			string hintStr = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace, speceType);
			Super.HintMainText(hintStr, 10, 3);
			yield break;
		}
		for (int n = 0; n < downLoadInfos.Count; n++)
		{
			int donwLoadCode = downLoadInfos[n].Value;
			string fileName = string.Empty;
			string filePath = string.Empty;
			string loadMapurl = string.Empty;
			filePath = Global.ReadXmlConfigStr(Global.NetVersionXML, string.Empty, "URL");
			float mCurrentFileSize = 0f;
			if (downLoadInfos[n].type == DownLoadType.Monster)
			{
				MonsterVO monsterV = ConfigMonsters.GetMonsterXmlNodeByID(downLoadInfos[n].Value);
				if (monsterV != null)
				{
					mCurrentFileSize = 800f;
					fileName = monsterV.ResName;
					loadMapurl = string.Format("{0}{1}/{2}", filePath, "Monster", fileName);
				}
			}
			else if (downLoadInfos[n].type == DownLoadType.NPC)
			{
				NPCInfoVO npcVO = ConfigNPCs.GetNPCVOByID(downLoadInfos[n].Value);
				if (npcVO != null)
				{
					mCurrentFileSize = 800f;
					fileName = npcVO.ResName;
					loadMapurl = string.Format("{0}{1}/{2}", filePath, "NPC", fileName);
				}
			}
			else if (downLoadInfos[n].type == DownLoadType.Decoration)
			{
				DecorationVO DecoVO = ConfigDecoration.GetDecorationVOByCode(downLoadInfos[n].Value);
				if (DecoVO != null)
				{
					mCurrentFileSize = 80f;
					fileName = DecoVO.ResName;
					loadMapurl = string.Format("{0}{1}/{2}", filePath, "Decoration", fileName);
				}
			}
			else if (downLoadInfos[n].type == DownLoadType.Map)
			{
				SettingMapVO mapVo = ConfigSettings.GetSettingMapVOByCode(downLoadInfos[n].Value);
				if (mapVo != null)
				{
					fileName = mapVo.ResName;
					mCurrentFileSize = (float)mapVo.FileSize;
					loadMapurl = string.Format("{0}{1}/{2}", filePath, "Map", fileName);
				}
			}
			if (Global.DownLoadInfos.Contains(fileName))
			{
				ReadySize += mCurrentFileSize;
			}
			else
			{
				if (this.StopDownload)
				{
					MUDebug.LogError<string>(new string[]
					{
						"using (www = new WWW(loadMapurl))之外，停止静默下载==="
					});
					this.www = null;
					if (monsertidList != null)
					{
						monsertidList.Clear();
					}
					if (downLoadInfos != null)
					{
						downLoadInfos.Clear();
					}
					yield break;
				}
				bool isDownLoadOK = false;
				string newUrl = loadMapurl + "?v=" + Context.ResSwfVer;
				MUDebug.Log<string>(new string[]
				{
					Global.GetLang("静默新地址 ") + newUrl
				});
				using (this.www = new WWW(newUrl))
				{
					yield return this.www;
					if (!string.IsNullOrEmpty(this.www.error))
					{
						MUDebug.LogError<string>(new string[]
						{
							this.www.error
						});
						this.www = null;
						MUDebug.LogError<string>(new string[]
						{
							"静默下载资源有误： " + this.www.error
						});
						yield break;
					}
					if (this.StopDownload)
					{
						MUDebug.LogError<string>(new string[]
						{
							string.Concat(new object[]
							{
								"===停止静默下载,，当前地图相关资源没有下载完成。总个数：",
								downLoadInfos.Count,
								Global.GetLang(" 已下载："),
								n,
								Global.GetLang("个，"),
								Global.GetLang("未下载："),
								downLoadInfos.Count - n,
								Global.GetLang("个")
							})
						});
						this.www = null;
						if (monsertidList != null)
						{
							monsertidList.Clear();
						}
						if (downLoadInfos != null)
						{
							downLoadInfos.Clear();
						}
						yield break;
					}
					if (this.www.bytes.Length > 0 && string.IsNullOrEmpty(this.www.error))
					{
						string path = string.Empty;
						if (downLoadInfos[n].type == DownLoadType.Monster)
						{
							path = PathUtils.GetPersistentPath(string.Format("{0}/{1}", "Monster", fileName));
						}
						else if (downLoadInfos[n].type == DownLoadType.NPC)
						{
							path = PathUtils.GetPersistentPath(string.Format("{0}/{1}", "NPC", fileName));
						}
						else if (downLoadInfos[n].type == DownLoadType.Decoration)
						{
							path = PathUtils.GetPersistentPath(string.Format("{0}/{1}", "Decoration", fileName));
						}
						else if (downLoadInfos[n].type == DownLoadType.Map)
						{
							path = PathUtils.GetPersistentPath(string.Format("{0}/{1}", "Map", fileName));
						}
						if (!this.StopDownload)
						{
							isDownLoadOK = Global.SaveBytesToFile(path, this.www.bytes);
							MUDebug.LogError<string>(new string[]
							{
								"静默下载的单个资源大小 " + this.www.size
							});
						}
					}
					if (!isDownLoadOK)
					{
						string needSpace2 = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							fileSize + "KB"
						});
						string speceType2 = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							(!QMQJJava.CheckSDCard()) ? Global.GetLang("内部机身存储") : Global.GetLang("SD卡存储")
						});
						string hintStr2 = string.Format(Global.GetLang("本次更新至少需{0}的可用{1}空间,您的内存不足"), needSpace2, speceType2);
						Super.HintMainText(hintStr2, 10, 3);
						yield break;
					}
					if (!this.StopDownload)
					{
						Global.SaveDownloadInfo(fileName);
					}
				}
			}
		}
		if (monsertidList != null)
		{
			monsertidList.Clear();
		}
		if (downLoadInfos != null)
		{
			downLoadInfos.Clear();
		}
		if (!this.StopDownload)
		{
			Global.AddDownloadedMap(this.MapCode);
			this.DownloadNextMap();
		}
		yield break;
	}

	public static void LogError(string log, string value = "")
	{
		string text = null;
		if (string.IsNullOrEmpty(value) && log.Contains("="))
		{
			string[] array = log.Split(new char[]
			{
				'='
			});
			if (array.Length >= 3)
			{
				string[] array2 = array[1].Split(new char[]
				{
					'/'
				});
				string text2 = array2[array2.Length - 1];
				text = string.Concat(new string[]
				{
					array[0],
					"  =  ",
					text2,
					"  =  ",
					array[2]
				});
			}
			DownLoadMapController.SaveInfo(text + "   " + value);
		}
		else
		{
			DownLoadMapController.SaveInfo(log);
		}
	}

	public static void SaveInfo(string info)
	{
		string persistentPath = PathUtils.GetPersistentPath("SaveDownloadFenBaoResLog.txt");
		FileInfo fileInfo = new FileInfo(persistentPath);
		StreamWriter streamWriter;
		if (!fileInfo.Exists)
		{
			streamWriter = fileInfo.CreateText();
		}
		else
		{
			streamWriter = fileInfo.AppendText();
		}
		streamWriter.Write(info + "\n");
		streamWriter.Close();
		streamWriter.Dispose();
	}

	private static DownLoadMapController instance;

	public bool StopDownload;

	private bool IsFinishDownloadAllRes;

	private WWW www;

	private int MapCode;

	private class MapDownloadData
	{
		public int mapCode;

		public string mapName;

		public int totalSize;

		public List<DownLoadData> downloadQueue = new List<DownLoadData>();
	}
}
