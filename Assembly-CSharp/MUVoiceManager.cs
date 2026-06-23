using System;
using System.IO;
using gcloud_voice;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class MUVoiceManager
{
	public static MUVoiceManager GetInstance()
	{
		if (MUVoiceManager.instance == null)
		{
			MUVoiceManager.instance = new MUVoiceManager();
		}
		return MUVoiceManager.instance;
	}

	public GCloudVoiceMode GVoiceMode
	{
		get
		{
			return this.mGVoiceMode;
		}
		set
		{
			this.mGVoiceMode = value;
		}
	}

	public int ChatChanel
	{
		get
		{
			return this.mChatChannel;
		}
		set
		{
			this.mChatChannel = value;
		}
	}

	public bool IsRecording
	{
		get
		{
			return this.isRecordingOffLine;
		}
		set
		{
			this.isRecordingOffLine = value;
		}
	}

	public void Pause()
	{
		if (this.mGvoice != null)
		{
			this.mGvoice.Pause();
		}
	}

	public void Resume()
	{
		if (this.mGvoice != null)
		{
			this.mGvoice.Resume();
		}
	}

	public void InitGVoice()
	{
		if (this.mGvoice != null)
		{
			return;
		}
		this.APP_ID = Global.VoiceAPP_ID;
		this.APP_KEY = Global.VoiceAPP_Key;
		if (string.IsNullOrEmpty(this.APP_ID) || string.IsNullOrEmpty(this.APP_KEY))
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南不开语音，语音配置初始化失败！"
			});
			return;
		}
		string persistentPath = PathUtils.GetPersistentPath("soundsDir/");
		if (!Directory.Exists(persistentPath))
		{
			Directory.CreateDirectory(persistentPath);
		}
		this.mSoundSavePath = persistentPath + "localVoice";
		this.mGvoice = GCloudVoice.GetEngine();
		if (Global.Data != null)
		{
			this.OPEN_ID = Global.Data.UserID;
			if (string.IsNullOrEmpty(this.OPEN_ID))
			{
				MUDebug.LogError<string>(new string[]
				{
					"玩家ID有误"
				});
				return;
			}
		}
		int code = this.mGvoice.SetAppInfo(this.APP_ID, this.APP_KEY, this.OPEN_ID);
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		if (Context.IsHaiwai)
		{
			string text = ConfigSystemParam.GetSystemParamByName("GVoiceServerUrl", true);
			if (string.IsNullOrEmpty(text))
			{
				text = "udp://tw.voice.gcloudcs.com:8700";
			}
			code = this.mGvoice.SetServerInfo(text);
			if (!this.GVoiceCodeSuc(code))
			{
				return;
			}
		}
		code = this.mGvoice.Init();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		this.VoiceMode = 2;
		this.mGvoice.ApplyMessageKey(60000);
		this.mGvoice.SetMaxMessageLength(60000);
		this.mGvoice.OnApplyMessageKeyComplete += new IGCloudVoice.ApplyMessageKeyCompleteHandler(this.MGvoice_OnApplyMessageKeyComplete);
		this.mGvoice.OnUploadReccordFileComplete += new IGCloudVoice.UploadReccordFileCompleteHandler(this.MGvoice_OnUploadReccordFileComplete);
		this.mGvoice.OnDownloadRecordFileComplete += new IGCloudVoice.DownloadRecordFileCompleteHandler(this.MGvoice_OnDownloadRecordFileComplete);
		this.mGvoice.OnPlayRecordFilComplete += new IGCloudVoice.PlayRecordFilCompleteHandler(this.MGvoice_OnPlayRecordFilComplete);
		this.mGvoice.OnSpeechToText += new IGCloudVoice.SpeechToTextHandler(this.MGvoice_OnSpeechToText);
		this.mGvoice.OnJoinRoomComplete += new IGCloudVoice.JoinRoomCompleteHandler(this.MGvoice_OnJoinRoomComplete);
		this.mGvoice.OnQuitRoomComplete += new IGCloudVoice.QuitRoomCompleteHandler(this.MGvoice_OnQuitRoomComplete);
	}

	private string RoomId { get; set; }

	private GCloudVoiceRole VoiceRole { get; set; }

	public GCloudVoiceRole SetVoiceRole
	{
		set
		{
			this.VoiceRole = value;
			this.VoiceMode = 0;
		}
	}

	public void SetRoomId(GVoiceSceneData data)
	{
		if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetFacePlate() != null)
		{
			this.RoomId = data.RoomName;
			MUDebug.LogError<string>(new string[]
			{
				"<color=yellow>实时语音房间名 CMD_SPR_GVOICE_SCENE_DATA：</color>" + data.RoomName
			});
			if (this.IsJunTuanRealTimeMap() && Global.Data != null && Global.Data.roleData.JunTuanZhiWu == 1)
			{
				this.SetVoiceRole = 1;
				return;
			}
		}
		if (this.IsZhanMengRealTimeMap() && Global.Data != null && Global.Data.roleData.BHZhiWu == 1)
		{
			this.SetVoiceRole = 1;
			return;
		}
		if (this.IsGetPriorityList && !string.IsNullOrEmpty(this.RoomId))
		{
			if (this.MUVoiceRole == 1)
			{
				this.SetVoiceRole = 1;
			}
			else if (this.MUVoiceRole == 2)
			{
				this.SetVoiceRole = 2;
			}
		}
	}

	public void SetPriorityList(GVoicePriorityData data)
	{
		if (data != null && !string.IsNullOrEmpty(data.RoleIdList))
		{
			MUDebug.LogError<string>(new string[]
			{
				"<color=yellow>实时语音权限名列表——CMD_SPR_GVOICE_GET_PRIORITYS：</color>" + data.RoleIdList
			});
			string[] array = data.RoleIdList.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (ConvertExt.SafeConvertToInt32(array[i]) == Global.Data.RoleID)
				{
					this.MUVoiceRole = 1;
					break;
				}
				this.MUVoiceRole = 2;
			}
		}
		else
		{
			this.MUVoiceRole = 2;
			MUDebug.LogError<string>(new string[]
			{
				"<color=yellow>实时语音权限数据为空 IsGetPriorityList = false data.RoleIdList</color>"
			});
		}
		this.IsGetPriorityList = true;
		if (!string.IsNullOrEmpty(this.RoomId))
		{
			if (this.MUVoiceRole == 1)
			{
				this.SetVoiceRole = 1;
			}
			else if (this.MUVoiceRole == 2)
			{
				this.SetVoiceRole = 2;
			}
		}
	}

	public bool IsInRoom
	{
		get
		{
			return this.mIsInRoom;
		}
		set
		{
			this.mIsInRoom = value;
		}
	}

	public GCloudVoiceMode ChangeMode
	{
		set
		{
			this.mGvoice.SetMode(value);
			if (value == null)
			{
				if (this.VoiceRole == 1)
				{
					if (PlayZone.GlobalPlayZone.GetFacePlate() != null)
					{
						if (this.ForbidMicCallBack != null)
						{
							this.ForbidMicCallBack.Invoke(false);
						}
						if (this.RealTimeVoiceHandler != null)
						{
							this.RealTimeVoiceHandler(true);
						}
						if (this.initMicHandler != null)
						{
							this.initMicHandler(true);
						}
						if (this.initSpeakerHandler != null)
						{
							this.initSpeakerHandler(true);
						}
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"ChangeMode 获取 PlayZone.GlobalPlayZone.GetFacePlate() 有误"
						});
					}
				}
				else if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetFacePlate() != null)
				{
					this.MicStatus = false;
					if (this.RealTimeVoiceHandler != null)
					{
						this.RealTimeVoiceHandler(true);
					}
					if (this.ForbidMicCallBack != null)
					{
						this.ForbidMicCallBack.Invoke(true);
					}
					if (this.initSpeakerHandler != null)
					{
						this.initSpeakerHandler(true);
					}
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"ChangeMode 获取 PlayZone.GlobalPlayZone.GetFacePlate() 有误"
					});
				}
			}
		}
	}

	public GCloudVoiceMode VoiceMode
	{
		get
		{
			return this.mVoiceMode;
		}
		set
		{
			if (value == 2)
			{
				this.mVoiceMode = value;
				int num = this.mGvoice.SetMode(value);
				Debug.LogError("VoiceMode--------------切换到离线语音模式的错误码：" + num);
			}
			else if (value == null && this.EnableRealtime)
			{
				if (string.IsNullOrEmpty(this.RoomId))
				{
					Super.HintMainText("实时语音房间号不能为空", 10, 3);
					return;
				}
				if (this.IsInRoom)
				{
					MUDebug.LogError<string>(new string[]
					{
						"===第二次申请加入语音房间===JoinRealTimeRoom"
					});
					this.mTmpVoiceMode = value;
					this.mIsReEnterRoom = true;
					this.QuiteRealTimeRoom();
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"===第一次申请加入语音房间===JoinRealTimeRoom"
					});
					this.mVoiceMode = value;
					int num2 = this.mGvoice.SetMode(value);
					MUDebug.LogError<string>(new string[]
					{
						"VoiceMode——切换到实时语音模式的错误码：" + num2
					});
					MUDebug.LogError<string>(new string[]
					{
						"===申请加入语音房间===JoinRealTimeRoom"
					});
					this.JoinRealTimeRoom(this.RoomId, this.VoiceRole);
				}
			}
		}
	}

	public bool StartRecord()
	{
		if (!this.MicEnable())
		{
			Super.HintMainText(Global.GetLang("麦克风未开启"), 10, 3);
			return false;
		}
		this.StopPlayingOffLineSound();
		int code = this.mGvoice.StartRecording(this.mSoundSavePath);
		if (!this.GVoiceCodeSuc(code))
		{
			return false;
		}
		MUDebug.LogError<string>(new string[]
		{
			"开始录音"
		});
		this.isRecordingOffLine = true;
		return true;
	}

	public void StopAndUploadRecord()
	{
		MUDebug.LogError<string>(new string[]
		{
			"停止录音并上传"
		});
		this.isRecordingOffLine = false;
		int code = this.mGvoice.StopRecording();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		int code2 = this.mGvoice.UploadRecordedFile(this.mSoundSavePath, this.TimeOutOffLine);
		if (!this.GVoiceCodeSuc(code2))
		{
			return;
		}
	}

	public void CancelRecord()
	{
		MUDebug.LogError<string>(new string[]
		{
			"停止录音"
		});
		this.isRecordingOffLine = false;
		int code = this.mGvoice.StopRecording();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
	}

	public bool PlayOffLineSound(string fileIdFromServer = null)
	{
		this.StopPlayingOffLineSound();
		int code = this.mGvoice.DownloadRecordedFile(fileIdFromServer, this.mSoundSavePath, this.TimeOutOffLine);
		return this.GVoiceCodeSuc(code);
	}

	public bool IsPlayingOffLineSound()
	{
		return this.isPlayingOffLineSound;
	}

	public bool StopPlayingOffLineSound()
	{
		if (this.IsPlayingOffLineSound())
		{
			this.isPlayingOffLineSound = false;
			return this.StopPlayOffLineSound();
		}
		return false;
	}

	private bool StopPlayOffLineSound()
	{
		int code = this.mGvoice.StopPlayFile();
		return this.GVoiceCodeSuc(code);
	}

	private void MGvoice_OnApplyMessageKeyComplete(IGCloudVoice.GCloudVoiceCompleteCode code)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			MUDebug.LogError<string>(new string[]
			{
				"语音初始化成功"
			});
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "初始化失败：", code)
			});
		}
	}

	private void MGvoice_OnUploadReccordFileComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			this.mSoundfileID = fileid;
			this.mSoundSavePath = filepath;
			MUDebug.LogError<string>(new string[]
			{
				"语音上传完成"
			});
			int[] array = new int[]
			{
				0
			};
			float[] array2 = new float[]
			{
				0f
			};
			this.mGvoice.GetFileParam(filepath, array, array2);
			this.mVoiceSeconds = array2[0];
			if (!Context.IsHaiwai)
			{
				int code2 = this.mGvoice.SpeechToText(this.mSoundfileID, 0, this.TimeOutOffLine);
				if (!this.GVoiceCodeSuc(code2))
				{
					return;
				}
			}
			else if (this.GVoiceCompleteSucCode(code))
			{
				if (this.IsInRoom)
				{
					this.ChangeMode = 0;
					MUDebug.LogError<string>(new string[]
					{
						"111在房间内——————切换到实时语音"
					});
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"111没没没没没在房间内——————切换到实时语音"
					});
				}
				string fromRoleName = string.Empty;
				string text = this.SpeechToSomeoneName;
				if (!string.IsNullOrEmpty(text))
				{
					text = text.TrimStart(new char[]
					{
						'/'
					});
					text = text.Trim();
				}
				if (this.ChatChanel < 0)
				{
					Super.HintMainText(Global.GetLang("语音频道有误" + this.ChatChanel), 10, 3);
					MUDebug.LogError<string>(new string[]
					{
						string.Format("{0}{1}", Global.GetLang("频道有误"), this.ChatChanel)
					});
					return;
				}
				if (this.ChatChanel == 3)
				{
					if (Global.Data.roleData.Faction == 0 && Global.Data.roleData.BHName == string.Empty)
					{
						Super.HintMainText(Global.GetLang("加入战盟后才可以使用帮会语音消息..."), 10, 3);
						return;
					}
				}
				else if (this.ChatChanel == 4)
				{
					if (Global.Data.roleData.TeamID <= 0)
					{
						Super.HintMainText(Global.GetLang("组建队伍后，才能给队友发送语音消息..."), 10, 3);
						return;
					}
				}
				else if (this.ChatChanel == 9 && !Global.RoleHaveArmyGroup())
				{
					Super.HintMainText(Global.GetLang("加入军团后，才可以发送语音消息"), 10, 3);
					return;
				}
				if (this.ChatChanel == 9)
				{
					fromRoleName = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
				}
				else
				{
					fromRoleName = Global.FormatRoleName(Global.Data.roleData);
				}
				string text2 = string.Format("{0}#{1}@{2}", this.mSoundfileID, this.mVoiceSeconds, string.Empty);
				GameInstance.Game.SpriteSendChat(this.ChatChanel, fromRoleName, text, text2, ChatType.Voice, 0);
				this.SpeechToSomeoneName = string.Empty;
			}
			else
			{
				if (this.IsInRoom)
				{
					this.ChangeMode = 0;
					MUDebug.LogError<string>(new string[]
					{
						"222在房间内——————切换到实时语音"
					});
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"222没没没没没在房间内——————切换到实时语音"
					});
				}
				Super.HintMainText(Global.GetLang("语音发送失败"), 10, 3);
				MUDebug.LogError<string>(new string[]
				{
					"语音转文字失败！——语音发送失败"
				});
			}
		}
		else
		{
			if (this.IsInRoom)
			{
				this.ChangeMode = 0;
				MUDebug.LogError<string>(new string[]
				{
					"在房间 MGvoice_OnUploadReccordFileComplete——————切换到实时语音"
				});
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"没在房间 MGvoice_OnUploadReccordFileComplete——————切换到实时语音"
				});
			}
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "上传语音失败：", code)
			});
		}
	}

	private void MGvoice_OnDownloadRecordFileComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			if (fileid == null)
			{
				return;
			}
			MUDebug.LogError<string>(new string[]
			{
				"语音下载完成"
			});
			int code2 = this.mGvoice.PlayRecordedFile(filepath);
			if (!this.GVoiceCodeSuc(code2))
			{
				return;
			}
			this.isPlayingOffLineSound = true;
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "下载语音失败：", code)
			});
		}
	}

	private void MGvoice_OnPlayRecordFilComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			this.isPlayingOffLineSound = false;
			MUDebug.LogError<string>(new string[]
			{
				"语音播放完成"
			});
			if (this.IsInRoom)
			{
				this.ChangeMode = 0;
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "语音播放失败：", code)
			});
		}
		if (this.VoicePlayCompleteCallBack != null)
		{
			this.VoicePlayCompleteCallBack.Invoke(true);
		}
		if (!this.CloseGameMusic)
		{
			Global.BackgroundAudio43D.PlayAudio(ConfigSettings.GetMapMusicFileByCode(Global.Data.roleData.MapCode, false), true, false);
			Global.Data.SysSetting.CloseGameMusic = false;
		}
		if (!this.CloseGameAudio)
		{
			Global.Data.SysSetting.CloseGameAudio = false;
		}
	}

	public string SpeechToSomeoneName
	{
		get
		{
			return this.mSpeechToSomeoneName;
		}
		set
		{
			this.mSpeechToSomeoneName = value;
		}
	}

	private void MGvoice_OnSpeechToText(IGCloudVoice.GCloudVoiceCompleteCode code, string fileID, string result)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			if (this.IsInRoom)
			{
				this.ChangeMode = 0;
				MUDebug.LogError<string>(new string[]
				{
					"111在房间内——————切换到实时语音"
				});
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"111没没没没没在房间内——————切换到实时语音"
				});
			}
			string fromRoleName = string.Empty;
			string text = this.SpeechToSomeoneName;
			if (!string.IsNullOrEmpty(text))
			{
				text = text.TrimStart(new char[]
				{
					'/'
				});
				text = text.Trim();
			}
			if (this.ChatChanel < 0)
			{
				Super.HintMainText(Global.GetLang("语音频道有误" + this.ChatChanel), 10, 3);
				MUDebug.LogError<string>(new string[]
				{
					string.Format("{0}{1}", "频道有误", this.ChatChanel)
				});
				return;
			}
			if (this.ChatChanel == 3)
			{
				if (Global.Data.roleData.Faction == 0 && Global.Data.roleData.BHName == string.Empty)
				{
					Super.HintMainText(Global.GetLang("加入战盟后才可以使用帮会语音消息..."), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 4)
			{
				if (Global.Data.roleData.TeamID <= 0)
				{
					Super.HintMainText(Global.GetLang("组建队伍后，才能给队友发送语音消息..."), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 9)
			{
				if (!Global.RoleHaveArmyGroup())
				{
					Super.HintMainText(Global.GetLang("加入军团后，才可以发送语音消息"), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 10 && !Global.RoleHaveComp())
			{
				Super.HintMainText(Global.GetLang("加入势力后，才可以发送语音消息"), 10, 3);
				return;
			}
			if (this.ChatChanel == 9)
			{
				fromRoleName = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
			}
			else
			{
				fromRoleName = Global.FormatRoleName(Global.Data.roleData);
			}
			string text2 = string.Format("{0}#{1}@{2}", fileID, this.mVoiceSeconds, Global.ReplaceVioceToWordFilterFileds(result));
			GameInstance.Game.SpriteSendChat(this.ChatChanel, fromRoleName, text, text2, ChatType.Voice, 0);
			this.SpeechToSomeoneName = string.Empty;
		}
		else
		{
			if (this.IsInRoom)
			{
				this.ChangeMode = 0;
				MUDebug.LogError<string>(new string[]
				{
					"222在房间内——————切换到实时语音"
				});
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"222没没没没没在房间内——————切换到实时语音"
				});
			}
			Super.HintMainText(Global.GetLang("语音发送失败"), 10, 3);
			MUDebug.LogError<string>(new string[]
			{
				"语音转文字失败！——语音发送失败"
			});
		}
	}

	private void JoinRealTimeRoom(string roomID, GCloudVoiceRole voiceRole)
	{
		this.mCurrentVoiceRole = voiceRole;
		this.mRoomID = roomID;
		int code = this.mGvoice.JoinNationalRoom(this.mRoomID, voiceRole, this.TimeOutRealTime);
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
	}

	public void QuiteRealTimeRoom()
	{
		if (string.IsNullOrEmpty(this.mRoomID))
		{
			MUDebug.LogError<string>(new string[]
			{
				"房间ID为空，已经退出语音房间——QuiteRealTimeRoom"
			});
			return;
		}
		int code = this.mGvoice.QuitRoom(this.mRoomID, this.TimeOutRealTime);
		if (!this.GVoiceCodeSuc(code))
		{
			MUDebug.LogError<string>(new string[]
			{
				"退出语音房间失败——QuiteRealTimeRoom"
			});
			return;
		}
		MUDebug.LogError<string>(new string[]
		{
			"退出语音房间——QuiteRealTimeRoom"
		});
	}

	private void MGvoice_OnJoinRoomComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			this.StopReJoinTimer();
			this.joinRoomAgain = 0;
			this.IsInRoom = true;
			MUDebug.LogError<string>(new string[]
			{
				"成功加入房间——MGvoice_OnJoinRoomComplete"
			});
			if (this.RealTimeVoiceHandler != null)
			{
				this.RealTimeVoiceHandler(true);
			}
			this.mRoomID = roomName;
			int code2 = this.mGvoice.OpenMic();
			if (!this.GVoiceCodeSuc(code2))
			{
				if (this.initMicHandler != null)
				{
					this.initMicHandler(false);
				}
				Super.HintMainText(Global.GetLang("麦克开启失败"), 10, 3);
				MUDebug.LogError<string>(new string[]
				{
					"麦克开启失败"
				});
			}
			if (this.mCurrentVoiceRole == 1)
			{
				if (this.ForbidMicCallBack != null)
				{
					this.ForbidMicCallBack.Invoke(false);
				}
				if (this.initMicHandler != null)
				{
					this.initMicHandler(false);
				}
			}
			else if (this.ForbidMicCallBack != null)
			{
				this.ForbidMicCallBack.Invoke(true);
			}
			code2 = this.mGvoice.OpenSpeaker();
			if (!this.GVoiceCodeSuc(code2))
			{
				if (this.initSpeakerHandler != null)
				{
					this.initSpeakerHandler(false);
				}
				Super.HintMainText(Global.GetLang("扬声器开启失败"), 10, 3);
			}
			else if (this.initSpeakerHandler != null)
			{
				this.initSpeakerHandler(true);
			}
		}
		else
		{
			if (this.RealTimeVoiceHandler != null)
			{
				this.RealTimeVoiceHandler(false);
			}
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "加入语音房间失败：", code)
			});
			if (this.joinRoomAgain <= 6)
			{
				this.StartReJoinUITimer();
			}
			else
			{
				this.StopReJoinTimer();
				Super.HintMainText(Global.GetLang("实时语音开启失败"), 10, 3);
				MUDebug.LogError<string>(new string[]
				{
					"结束轮询——joinRoomAgain= " + this.joinRoomAgain
				});
			}
		}
	}

	protected void StartReJoinUITimer()
	{
		if (this.UITimerReJoin == null)
		{
			this.UITimerReJoin = new DispatcherTimer("MUVoiceManagerReJoinTimer");
			this.UITimerReJoin.Interval = TimeSpan.FromSeconds(2.0);
			this.UITimerReJoin.Tick = new DispatcherTimerEventHandler(this.UITimerReJoin_Tick);
		}
		this.UITimerReJoin.Start();
	}

	private void StopReJoinTimer()
	{
		if (this.UITimerReJoin != null)
		{
			this.UITimerReJoin.Tick = null;
			this.UITimerReJoin.Stop();
			this.UITimerReJoin = null;
		}
	}

	protected void UITimerReJoin_Tick(object sender, object e)
	{
		this.joinRoomAgain++;
		this.JoinRealTimeRoom(this.RoomId, this.VoiceRole);
		MUDebug.LogError<string>(new string[]
		{
			"开始轮询——joinRoomAgain= " + this.joinRoomAgain
		});
	}

	protected void StartUITimer()
	{
		if (this.UITimer == null)
		{
			this.UITimer = new DispatcherTimer("MUVoiceManagerTimer");
			this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
			this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		}
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		this.mIsReEnterRoom = false;
		this.mVoiceMode = this.mTmpVoiceMode;
		int num = this.mGvoice.SetMode(this.mTmpVoiceMode);
		MUDebug.LogError<string>(new string[]
		{
			"UITimer_Tick——VoiceMode——切换到实时语音模式的错误码：" + num
		});
		Debug.LogError("UITimer_Tick===申请加入语音房间===JoinRealTimeRoom");
		this.JoinRealTimeRoom(this.RoomId, this.VoiceRole);
		this.StopTimer();
	}

	private void MGvoice_OnQuitRoomComplete(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID)
	{
		if (this.GVoiceCompleteSucCode(code))
		{
			this.IsInRoom = false;
			MUDebug.LogError<string>(new string[]
			{
				"退出语音房间——MGvoice_OnQuitRoomComplete"
			});
			this.ClearAll();
			if (this.mIsReEnterRoom)
			{
				this.StartUITimer();
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "退出语音失败：", code)
			});
		}
	}

	private void OpenMic()
	{
		int code = this.mGvoice.OpenMic();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		this.isMicOpen = true;
		MUDebug.LogError<string>(new string[]
		{
			"麦克已开启——OpenMic"
		});
	}

	private void CloseMic()
	{
		int code = this.mGvoice.CloseMic();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		this.isMicOpen = false;
		MUDebug.LogError<string>(new string[]
		{
			"麦克已关闭——CloseMic"
		});
	}

	public bool MicStatus
	{
		get
		{
			return this.isMicOpen;
		}
		set
		{
			if (value)
			{
				this.OpenMic();
			}
			else
			{
				this.CloseMic();
			}
		}
	}

	private void OpenSpeaker()
	{
		int code = this.mGvoice.OpenSpeaker();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		this.isSpeakerOpen = true;
		MUDebug.LogError<string>(new string[]
		{
			"扬声器已开启——OpenSpeaker"
		});
	}

	private void CloseSpeaker()
	{
		int code = this.mGvoice.CloseSpeaker();
		if (!this.GVoiceCodeSuc(code))
		{
			return;
		}
		this.isSpeakerOpen = false;
		MUDebug.LogError<string>(new string[]
		{
			"扬声器已关闭——CloseSpeaker"
		});
	}

	public bool SpeakerStatus
	{
		get
		{
			return this.isSpeakerOpen;
		}
		set
		{
			if (value)
			{
				this.OpenSpeaker();
			}
			else
			{
				this.CloseSpeaker();
			}
		}
	}

	public void Update()
	{
		if (this.mGvoice != null)
		{
			this.mGvoice.Poll();
		}
	}

	private bool GVoiceCodeSuc(GCloudVoiceErr code)
	{
		bool result = false;
		string text = string.Empty;
		switch (code)
		{
		case 4097:
			text = "some param is null";
			break;
		case 4098:
			text = "you should call SetAppInfo first before call other api";
			break;
		case 4099:
			text = "Init Erro";
			break;
		case 4100:
			text = "now is recording, can't do other operator";
			break;
		case 4101:
			text = "poll buffer is not enough or null";
			break;
		case 4102:
			text = "call some api, but the mode is not correct, maybe you shoud call SetMode first and correct";
			break;
		case 4103:
			text = Global.GetLang(" 传入的参数不对，比如房间名为空或者超长（最大长度127字节）且由a-z,A-Z,0-9,-,_组成。超时范围5000ms-60000ms");
			break;
		case 4104:
			text = "open a file err";
			break;
		case 4105:
			text = "you should call Init before do this operator";
			break;
		case 4106:
			text = "you have not get engine instance, this common in use c# api, but not get gcloudvoice instance first";
			break;
		case 4107:
			text = "this common in c# api, parse poll msg err";
			break;
		case 4108:
			text = "poll, no msg to update";
			break;
		default:
			switch (code)
			{
			case 12289:
				text = "apply authkey api error";
				break;
			case 12290:
				text = "the path can not access ,may be path file not exists or deny to access";
				break;
			case 12291:
				text = "you have not right to access micphone in android";
				break;
			case 12292:
				text = "you have not get authkey, call ApplyMessageKey first";
				break;
			case 12293:
				text = "upload file err";
				break;
			case 12294:
				text = "http is busy,maybe the last upload/download not finish.";
				break;
			case 12295:
				text = "download file err";
				break;
			case 12296:
				text = "open or close speaker tve error";
				break;
			case 12297:
				text = "tve play file error";
				break;
			case 12298:
				text = "Already in applying auth key processing";
				break;
			default:
				switch (code)
				{
				case 8193:
					text = " call some realtime api, but state err, such as OpenMic but you have not Join Room first";
					break;
				case 8194:
					text = "join room failed";
					break;
				case 8195:
					text = "quit room err, the quit roomname not equal join roomname";
					break;
				case 8196:
					text = "当前以听众身份加入的大房间，不能开麦关麦";
					break;
				default:
					switch (code)
					{
					case 20481:
						text = "internal TVE err, our used";
						break;
					case 20482:
						text = "internal Not TVE err, out used";
						break;
					case 20483:
						text = "internal used, you should not get this err num";
						break;
					default:
						if (code != null)
						{
							if (code != 24577)
							{
								if (code == 28673)
								{
									text = "Already in speach to text processing";
								}
							}
							else
							{
								text = "bad server address,should be udp://capi.xxx.xxx.com";
							}
						}
						else
						{
							result = true;
						}
						break;
					}
					break;
				}
				break;
			}
			break;
		}
		MUDebug.LogError<string>(new string[]
		{
			text
		});
		return result;
	}

	private bool GVoiceCompleteSucCode(IGCloudVoice.GCloudVoiceCompleteCode code)
	{
		bool result = false;
		string text = string.Empty;
		switch (code)
		{
		case 1:
			result = true;
			break;
		case 2:
			text = "join room timeout";
			break;
		case 3:
			text = "communication with svr occur some err, such as err data recv from svr";
			break;
		case 4:
			text = "reserved, our internal unknow err";
			break;
		case 5:
			text = "net err,may be can't connect to network";
			break;
		case 6:
			result = true;
			break;
		case 7:
			result = true;
			break;
		case 8:
			text = "apply message authkey timeout";
			break;
		case 9:
			text = "communication with svr occur some err, such as err data recv from svr";
			break;
		case 10:
			text = "reserved,  our internal unknow err";
			break;
		case 11:
			result = true;
			break;
		case 12:
			text = "upload record file occur error";
			break;
		case 13:
			result = true;
			break;
		case 14:
			text = "download record file occur error";
			break;
		case 15:
			result = true;
			break;
		case 16:
			text = "speech to text with timeout";
			break;
		case 17:
			text = "server's error";
			break;
		case 18:
			result = true;
			break;
		}
		MUDebug.LogError<string>(new string[]
		{
			text
		});
		return result;
	}

	public int GetVolume()
	{
		if (this.mGvoice != null)
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format("{0}{1}", "麦克风音量的大小=== ", this.mGvoice.GetMicLevel())
			});
			return this.mGvoice.GetMicLevel();
		}
		return 0;
	}

	public bool MicEnable()
	{
		return true;
	}

	private void ClearAll()
	{
		if (this.initSpeakerHandler != null)
		{
			this.initSpeakerHandler(false);
		}
		if (this.initMicHandler != null)
		{
			this.initMicHandler(false);
		}
		this.mCurrentVoiceRole = 2;
	}

	public bool IsZhanMengRealTimeMap()
	{
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("ZhanMengVoice", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			string[] array = systemParamByName.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				int num = array2.IndexOf(Global.Data.roleData.MapCode.ToString());
				if (num >= 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsJunTuanRealTimeMap()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunTuanVoice", '|');
		if (systemParamIntArrayByName != null && Global.Data != null)
		{
			int num = systemParamIntArrayByName.IndexOf(Global.Data.roleData.MapCode);
			if (num >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSimulatorOpenVoice()
	{
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("VoiceSimulatorOpen", true);
		int num = -1;
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			num = Global.SafeConvertToInt32(systemParamByName);
		}
		return num == 1;
	}

	public bool CanSendVoice(bool showLabel = true)
	{
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("VoiceMessage", ',');
		if (systemParamIntArrayByName.Length >= 3)
		{
			num = systemParamIntArrayByName[0];
			num2 = systemParamIntArrayByName[1];
			num3 = systemParamIntArrayByName[2];
		}
		bool flag = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level >= num * 100 + num2;
		if (!flag)
		{
			if (showLabel)
			{
				Super.HintMainText(string.Format("{0}{1}{2}{3}{4}", new object[]
				{
					Global.GetLang("需要达到"),
					num,
					Global.GetLang("转"),
					num2,
					Global.GetLang("级才可使用")
				}), 10, 3);
			}
			return flag;
		}
		flag = (Global.Data.roleData.VIPLevel >= num3);
		if (!flag)
		{
			if (showLabel)
			{
				Super.HintMainText(string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
				{
					Global.GetLang("需要达到"),
					num,
					Global.GetLang("转"),
					num2,
					Global.GetLang("级并且VIP达到"),
					num3,
					Global.GetLang("级才可使用")
				}), 10, 3);
			}
			return flag;
		}
		return flag;
	}

	public void ExitRealTimeSceneWhenChangeSystemSetting()
	{
		if (this.IsGetRealTimeData)
		{
			this.ExitRealTimeScene();
		}
	}

	public void ExitRealTimeScene()
	{
	}

	public bool IsMuVoiceOpen(string path = "VoiceOpen")
	{
		bool result = false;
		string systemParamByName = ConfigSystemParam.GetSystemParamByName(path, true);
		if (string.IsNullOrEmpty(systemParamByName))
		{
			Super.HintMainText("缺少VoiceOpen配置！", 10, 3);
			return result;
		}
		string[] array = systemParamByName.Split(new char[]
		{
			'|'
		});
		int num = 3;
		if (PlatSDKMgr.PlatName == "YYB")
		{
			num = 4;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Split(new char[]
			{
				','
			}).Length == 2 && array[i].Split(new char[]
			{
				','
			})[0].SafeToInt32(0) == num && array[i].Split(new char[]
			{
				','
			})[1].SafeToInt32(0) == 1)
			{
				result = true;
			}
		}
		return result;
	}

	public void TestSendVoiceChatToServer(float VoiceSeconds)
	{
		string text = "测试语音！fuck 操语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！测试语音！";
		string fromRoleName = string.Empty;
		string text2 = this.SpeechToSomeoneName;
		if (this.ChatChanel == 5 && string.IsNullOrEmpty(text2))
		{
			GChat.AddSystemChatMessage(Global.GetLang("请输入要私聊的角色名称"), ChatTypeIndexes.System);
			Super.HintMainText("请输入要私聊的角色名称", 10, 3);
			return;
		}
		if (!string.IsNullOrEmpty(text2))
		{
			text2 = text2.TrimStart(new char[]
			{
				'/'
			});
			text2 = text2.Trim();
		}
		if (this.ChatChanel < 0)
		{
			Super.HintMainText(Global.GetLang("未设置语音频道"), 10, 3);
			return;
		}
		if (this.ChatChanel != 3)
		{
			if (this.ChatChanel == 3)
			{
				if (Global.Data.roleData.Faction == 0 && Global.Data.roleData.BHName == string.Empty)
				{
					Super.HintMainText(Global.GetLang("加入战盟后才可以使用帮会语音消息..."), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 4)
			{
				if (Global.Data.roleData.TeamID <= 0)
				{
					Super.HintMainText(Global.GetLang("组建队伍后，才能给队友发送语音消息..."), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 9)
			{
				if (!Global.RoleHaveArmyGroup())
				{
					Super.HintMainText(Global.GetLang("加入军团后，才可以发送语音消息"), 10, 3);
					return;
				}
			}
			else if (this.ChatChanel == 10 && !Global.RoleHaveComp())
			{
				Super.HintMainText(Global.GetLang("加入势力后，才可以发送语音消息"), 10, 3);
				return;
			}
		}
		if (this.ChatChanel == 9)
		{
			fromRoleName = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
		}
		else
		{
			fromRoleName = Global.FormatRoleName(Global.Data.roleData);
		}
		string text3 = string.Concat(new object[]
		{
			"url#",
			VoiceSeconds,
			"@",
			Global.ReplaceVioceToWordFilterFileds(text)
		});
		GameInstance.Game.SpriteSendChat(this.ChatChanel, fromRoleName, text2, text3, ChatType.Voice, 0);
		this.SpeechToSomeoneName = string.Empty;
	}

	private static MUVoiceManager instance;

	public Action<bool> HideSpeakerCallBack;

	public Action<bool> VoicePlayCompleteCallBack;

	public Action<bool> ForbidMicCallBack;

	public MUVoiceManager.InitMicHandler initMicHandler;

	public MUVoiceManager.InitSpeakerHandler initSpeakerHandler;

	public MUVoiceManager.InitRealTimeVoiceHandler RealTimeVoiceHandler;

	public MUVoiceManager.InitOffLineVoiceHandler OffLineVoiceHandler;

	private IGCloudVoice mGvoice;

	private string APP_ID = string.Empty;

	private string APP_KEY = string.Empty;

	private string OPEN_ID = string.Empty;

	private GCloudVoiceMode mGVoiceMode = 1;

	private string mSoundSavePath;

	public string mSoundfileID;

	private int TimeOutOffLine = 30000;

	private int TimeOutRealTime = 20000;

	private bool isPlayingOffLineSound;

	private string mRoomID;

	private GCloudVoiceRole mCurrentVoiceRole = 2;

	private int mChatChannel = -1;

	private bool isRecordingOffLine;

	public float RecordVoiceMaxLength_sec = 60f;

	private float mVoiceSeconds;

	public bool IsGetRealTimeData;

	private int MUVoiceRole = 2;

	public bool IsGetPriorityList;

	public bool EnableRealtime;

	private bool mIsInRoom;

	private bool mIsReEnterRoom;

	private GCloudVoiceMode mVoiceMode = 2;

	private GCloudVoiceMode mTmpVoiceMode = 1;

	public bool CloseGameMusic;

	public bool CloseGameAudio;

	private string mSpeechToSomeoneName = string.Empty;

	private int joinRoomAgain;

	private DispatcherTimer UITimerReJoin;

	private DispatcherTimer UITimer;

	private bool isMicOpen;

	private bool isSpeakerOpen;

	public delegate void InitMicHandler(bool status);

	public delegate void InitSpeakerHandler(bool status);

	public delegate void InitRealTimeVoiceHandler(bool status);

	public delegate void InitOffLineVoiceHandler(bool status);
}
