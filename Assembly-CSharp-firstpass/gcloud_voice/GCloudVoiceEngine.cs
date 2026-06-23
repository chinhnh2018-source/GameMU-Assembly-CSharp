using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace gcloud_voice
{
	public class GCloudVoiceEngine : IGCloudVoice
	{
		public GCloudVoiceEngine()
		{
			int num = GCloudVoiceEngine.GCloudVoice_CreateInstance();
			if (num != 0)
			{
				Debug.Log("Create GCloudVoiceInstance failed!");
			}
			this.pollBuf = new byte[this.pollBufLen];
			this.pollMsg = new GCloudVoiceEngine.NoticeMessage();
			this.memberVoice = new int[100];
		}

		public override event IGCloudVoice.JoinRoomCompleteHandler OnJoinRoomComplete;

		public override event IGCloudVoice.QuitRoomCompleteHandler OnQuitRoomComplete;

		public override event IGCloudVoice.MemberVoiceHandler OnMemberVoice;

		public override event IGCloudVoice.ApplyMessageKeyCompleteHandler OnApplyMessageKeyComplete;

		public override event IGCloudVoice.UploadReccordFileCompleteHandler OnUploadReccordFileComplete;

		public override event IGCloudVoice.DownloadRecordFileCompleteHandler OnDownloadRecordFileComplete;

		public override event IGCloudVoice.PlayRecordFilCompleteHandler OnPlayRecordFilComplete;

		public override event IGCloudVoice.SpeechToTextHandler OnSpeechToText;

		public override event IGCloudVoice.StatusUpdateHandler OnStatusUpdate;

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_CreateInstance();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SetServerInfo([MarshalAs(42)] string URL);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SetAppInfo([MarshalAs(42)] string appID, [MarshalAs(42)] string appKey, [MarshalAs(42)] string openID);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_Init();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SetMode(int mode);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_Poll([MarshalAs(42)] byte[] buf, int length);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_Pause();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_Resume();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_JoinTeamRoom([MarshalAs(42)] string roomName, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_JoinNationalRoom([MarshalAs(42)] string roomName, int role, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_JoinFMRoom([MarshalAs(42)] string roomName, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_QuitRoom([MarshalAs(42)] string roomName, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_OpenMic();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_CloseMic();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_OpenSpeaker();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_CloseSpeaker();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_ApplyMessageKey(int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SetMaxMessageLength(int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_StartRecording([MarshalAs(42)] string filePath);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_StopRecording();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_UploadRecordedFile([MarshalAs(42)] string filePath, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_DownloadRecordedFile([MarshalAs(42)] string fileID, [MarshalAs(42)] string downloadFilePath, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_PlayRecordedFile([MarshalAs(42)] string downloadFilePath);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_StopPlayFile();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SpeechToText([MarshalAs(42)] string fileID, int language, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_ForbidMemberVoice(int member, bool bEnable);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_EnableLog(bool enable);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_GetMicLevel();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_GetSpeakerLevel();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SetSpeakerVolume(int vol);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_TestMic();

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_GetFileParam([MarshalAs(42)] string filepath, [MarshalAs(42)] int[] bytes, [MarshalAs(42)] float[] seconds);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_invoke(uint nCmd, uint nParam1, uint nParam2, [MarshalAs(42)] uint[] pOutput);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_JoinNationalRoom_Token([MarshalAs(42)] string roomName, int role, [MarshalAs(42)] string token, int timestamp, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_JoinTeamRoom_Token([MarshalAs(42)] string roomName, [MarshalAs(42)] string token, int timestamp, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_ApplyMessageKey_Token([MarshalAs(42)] string token, int timestamp, int msTimeout);

		[DllImport("GCloudVoice", CallingConvention = 2)]
		private static extern int GCloudVoice_SpeechToText_Token([MarshalAs(42)] string fileID, [MarshalAs(42)] string token, int timestamp, int msTimeout, int language);

		public override int SetAppInfo(string appID, string appKey, string openID)
		{
			int num = GCloudVoiceEngine.GCloudVoice_SetAppInfo(appID, appKey, openID);
			if (num == 0)
			{
				GCloudVoiceEngine.bIsSetAppInfo = true;
			}
			return num;
		}

		public override int SetServerInfo(string URL)
		{
			return GCloudVoiceEngine.GCloudVoice_SetServerInfo(URL);
		}

		public override int Init()
		{
			if (!GCloudVoiceEngine.bIsSetAppInfo)
			{
				Debug.Log("please set appinfo first");
				return 4098;
			}
			if (!GCloudVoiceEngine.bInit)
			{
				int num = GCloudVoiceEngine.GCloudVoice_Init();
				if (num != 0)
				{
					Debug.Log("Init GCloudVoice failed!");
					return num;
				}
				GCloudVoiceEngine.bInit = true;
			}
			return 0;
		}

		public override int SetMode(GCloudVoiceMode nMode)
		{
			Debug.Log("GCloudVoice_C# API: _SetMode");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			return GCloudVoiceEngine.GCloudVoice_SetMode((int)nMode);
		}

		public GCloudVoiceEngine.NoticeMessage NoticeMessage_ParseFrom(byte[] buf, int buflen)
		{
			int num = 0;
			if (buflen - num < 4)
			{
				Debug.Log("notifymsg,parse error, buf small then sizeof(int)");
				return null;
			}
			this.pollMsg.what = BitConverter.ToInt32(buf, num);
			num += 4;
			this.pollMsg.intArg1 = BitConverter.ToInt32(buf, num);
			num += 4;
			this.pollMsg.intArg2 = BitConverter.ToInt32(buf, num);
			num += 4;
			int num2 = BitConverter.ToInt32(buf, num);
			num += 4;
			if (num2 == 0)
			{
				this.pollMsg.strArg = string.Empty;
			}
			else
			{
				byte[] array = new byte[num2];
				Array.Copy(buf, num, array, 0, num2);
				this.pollMsg.strArg = Encoding.Default.GetString(array);
			}
			num += num2;
			this.pollMsg.datalen = BitConverter.ToInt32(buf, num);
			num += 4;
			if (this.pollMsg.datalen > 0)
			{
				Array.Copy(buf, num, this.pollMsg.custom, 0, this.pollMsg.datalen);
			}
			return this.pollMsg;
		}

		public override int Poll()
		{
			int num = GCloudVoiceEngine.GCloudVoice_Poll(this.pollBuf, this.pollBufLen);
			if (num != 0)
			{
				if (num == 4108)
				{
					return 0;
				}
				return num;
			}
			else
			{
				this.pollMsg.clear();
				GCloudVoiceEngine.NoticeMessage noticeMessage = this.NoticeMessage_ParseFrom(this.pollBuf, this.pollBufLen);
				if (noticeMessage == null)
				{
					return 4107;
				}
				if (noticeMessage.what == 1)
				{
					if (this.OnJoinRoomComplete != null)
					{
						Debug.Log("c# poll callback OnJoinRoomComplete not null");
						this.OnJoinRoomComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, noticeMessage.intArg2);
					}
				}
				else if (noticeMessage.what == 2)
				{
					if (this.OnQuitRoomComplete != null)
					{
						this.OnQuitRoomComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, noticeMessage.intArg2);
					}
				}
				else if (noticeMessage.what == 6)
				{
					if (this.OnApplyMessageKeyComplete != null)
					{
						this.OnApplyMessageKeyComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1);
					}
				}
				else if (noticeMessage.what == 3)
				{
					if (this.OnUploadReccordFileComplete != null)
					{
						this.OnUploadReccordFileComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, (noticeMessage.custom == null) ? string.Empty : Encoding.Default.GetString(noticeMessage.custom, 0, noticeMessage.datalen));
					}
				}
				else if (noticeMessage.what == 4)
				{
					if (this.OnDownloadRecordFileComplete != null)
					{
						this.OnDownloadRecordFileComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, (noticeMessage.custom == null) ? string.Empty : Encoding.Default.GetString(noticeMessage.custom, 0, noticeMessage.datalen));
					}
				}
				else if (noticeMessage.what == 7)
				{
					if (this.OnPlayRecordFilComplete != null)
					{
						this.OnPlayRecordFilComplete((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg);
					}
				}
				else if (noticeMessage.what == 8)
				{
					if (this.OnSpeechToText != null)
					{
						this.OnSpeechToText((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, (noticeMessage.custom == null) ? string.Empty : Encoding.UTF8.GetString(noticeMessage.custom, 0, noticeMessage.datalen));
					}
				}
				else if (noticeMessage.what == 5)
				{
					if (this.OnMemberVoice != null)
					{
						Array.Clear(this.memberVoice, 0, this.memberVoice.Length);
						int intArg = noticeMessage.intArg1;
						for (int i = 0; i < intArg; i++)
						{
							this.memberVoice[2 * i] = BitConverter.ToInt32(this.pollMsg.custom, 2 * i * 4);
							this.memberVoice[2 * i + 1] = BitConverter.ToInt32(this.pollMsg.custom, (2 * i + 1) * 4);
						}
						this.OnMemberVoice(this.memberVoice, intArg);
					}
				}
				else if (noticeMessage.what == 9 && this.OnStatusUpdate != null)
				{
					this.OnStatusUpdate((IGCloudVoice.GCloudVoiceCompleteCode)noticeMessage.intArg1, noticeMessage.strArg, noticeMessage.intArg2);
				}
				return 0;
			}
		}

		public override int Pause()
		{
			Debug.Log("GCloudVoice_C# API: _Pause");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_Pause();
			Debug.Log("GCloudVoice_C# API: _Pause nRet=" + num);
			return num;
		}

		public override int Resume()
		{
			Debug.Log("GCloudVoice_C# API: _Resume");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_Resume();
			Debug.Log("GCloudVoice_C# API: _Resume nRet=" + num);
			return num;
		}

		public override int JoinTeamRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_JoinTeamRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom  nRet=" + num);
			return num;
		}

		public override int JoinTeamRoom(string roomName, string token, int timestamp, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom mstimeout:" + msTimeout);
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_JoinTeamRoom_Token(roomName, token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom  nRet=" + num);
			return num;
		}

		public override int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_JoinNationalRoom(roomName, (int)role, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom  nRet=" + num);
			return num;
		}

		public override int JoinFMRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinFMRoom");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_JoinFMRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinFMRoom  nRet=" + num);
			return num;
		}

		public override int JoinNationalRoom(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_JoinNationalRoom_Token(roomName, (int)role, token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom  nRet=" + num);
			return num;
		}

		public override int QuitRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: QuitRoom");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_QuitRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: QuitRoom  nRet=" + num);
			return num;
		}

		public override int OpenMic()
		{
			Debug.Log("GCloudVoice_C# API: OpenMic");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_OpenMic();
			Debug.Log("GCloudVoice_C# API: OpenMic  nRet=" + num);
			return num;
		}

		public override int CloseMic()
		{
			Debug.Log("GCloudVoice_C# API: CloseMic");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_CloseMic();
			Debug.Log("GCloudVoice_C# API: CloseMic  nRet=" + num);
			return num;
		}

		public override int OpenSpeaker()
		{
			Debug.Log("GCloudVoice_C# API: OpenSpeaker");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_OpenSpeaker();
			Debug.Log("GCloudVoice_C# API: OpenSpeaker  nRet=" + num);
			return num;
		}

		public override int CloseSpeaker()
		{
			Debug.Log("GCloudVoice_C# API: CloseSpeaker");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_CloseSpeaker();
			Debug.Log("GCloudVoice_C# API: CloseSpeaker  nRet=" + num);
			return num;
		}

		public override int ApplyMessageKey(int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_ApplyMessageKey(msTimeout);
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey  nRet=" + num);
			return num;
		}

		public override int ApplyMessageKey(string token, int timestamp, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_ApplyMessageKey_Token(token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey  nRet=" + num);
			return num;
		}

		public override int SetMaxMessageLength(int msTime)
		{
			Debug.Log("GCloudVoice_C# API: SetMaxMessageLength");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_SetMaxMessageLength(msTime);
			Debug.Log("GCloudVoice_C# API: SetMaxMessageLength  nRet=" + num);
			return num;
		}

		public override int StartRecording(string filePath)
		{
			Debug.Log("GCloudVoice_C# API: StartRecording");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_StartRecording(filePath);
			Debug.Log("GCloudVoice_C# API: StartRecording  nRet=" + num);
			return num;
		}

		public override int StopRecording()
		{
			Debug.Log("GCloudVoice_C# API: StopRecording");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_StopRecording();
			Debug.Log("GCloudVoice_C# API: StopRecording  nRet=" + num);
			return num;
		}

		public override int UploadRecordedFile(string filePath, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: UploadRecordedFile");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_UploadRecordedFile(filePath, msTimeout);
			Debug.Log("GCloudVoice_C# API: UploadRecordedFile  nRet=" + num);
			return num;
		}

		public override int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: DownloadRecordedFile");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_DownloadRecordedFile(fileID, downloadFilePath, msTimeout);
			Debug.Log("GCloudVoice_C# API: DownloadRecordedFile  nRet=" + num);
			return num;
		}

		public override int PlayRecordedFile(string downloadFilePath)
		{
			Debug.Log("GCloudVoice_C# API: PlayRecordedFile");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_PlayRecordedFile(downloadFilePath);
			Debug.Log("GCloudVoice_C# API: PlayRecordedFile  nRet=" + num);
			return num;
		}

		public override int StopPlayFile()
		{
			Debug.Log("GCloudVoice_C# API: StopPlayFile");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_StopPlayFile();
			Debug.Log("GCloudVoice_C# API: StopPlayFile  nRet=" + num);
			return num;
		}

		public override int SpeechToText(string fileID, int language = 0, int msTimeout = 6000)
		{
			Debug.Log("GCloudVoice_C# API: SpeechToText");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_SpeechToText(fileID, language, msTimeout);
			Debug.Log("GCloudVoice_C# API: SpeechToText  nRet=" + num);
			return num;
		}

		public override int SpeechToText(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000)
		{
			Debug.Log("GCloudVoice_C# API: SpeechToText");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_SpeechToText_Token(fileID, token, timestamp, language, msTimeout);
			Debug.Log("GCloudVoice_C# API: SpeechToText  nRet=" + num);
			return num;
		}

		public override int ForbidMemberVoice(int member, bool bEnable)
		{
			Debug.Log("GCloudVoice_C# API: ForbidMemberVoice");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_ForbidMemberVoice(member, bEnable);
			Debug.Log("GCloudVoice_C# API: ForbidMemberVoice  nRet=" + num);
			return num;
		}

		public override int EnableLog(bool enable)
		{
			Debug.Log("GCloudVoice_C# API: EnableLog");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_EnableLog(enable);
			Debug.Log("GCloudVoice_C# API: EnableLog  nRet=" + num);
			return num;
		}

		public override int GetMicLevel()
		{
			Debug.Log("GCloudVoice_C# API: GetMicLevel");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_GetMicLevel();
			Debug.Log("GCloudVoice_C# API: GetMicLevel  nRet=" + num);
			return num;
		}

		public override int GetSpeakerLevel()
		{
			Debug.Log("GCloudVoice_C# API: GetSpeakerLevel");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_GetSpeakerLevel();
			Debug.Log("GCloudVoice_C# API: GetSpeakerLevel  nRet=" + num);
			return num;
		}

		public override int SetSpeakerVolume(int vol)
		{
			Debug.Log("GCloudVoice_C# API: SetSpeakerVolume");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_SetSpeakerVolume(vol);
			Debug.Log("GCloudVoice_C# API: SetSpeakerVolume  nRet=" + num);
			return num;
		}

		public override int TestMic()
		{
			Debug.Log("GCloudVoice_C# API: TestMic");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_TestMic();
			Debug.Log("GCloudVoice_C# API: TestMic  nRet=" + num);
			return num;
		}

		public override int GetFileParam(string filepath, int[] bytes, float[] seconds)
		{
			Debug.Log("GCloudVoice_C# API: GetFileParam");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_GetFileParam(filepath, bytes, seconds);
			Debug.Log("GCloudVoice_C# API: GetFileParam  nRet=" + num);
			return num;
		}

		public override int invoke(uint nCmd, uint nParam1, uint nParam2, uint[] pOutput)
		{
			Debug.Log("GCloudVoice_C# API: invoke");
			if (!GCloudVoiceEngine.bInit)
			{
				return 4105;
			}
			int num = GCloudVoiceEngine.GCloudVoice_invoke(nCmd, nParam1, nParam2, pOutput);
			Debug.Log("GCloudVoice_C# API: invoke  nRet=" + num);
			return num;
		}

		public const string LibName = "GCloudVoice";

		private static bool bInit;

		private static bool bIsSetAppInfo;

		private int pollBufLen = 2048;

		private byte[] pollBuf;

		private GCloudVoiceEngine.NoticeMessage pollMsg;

		private int[] memberVoice;

		private enum NoticeMessageType
		{
			MSG_ON_JOINROOM_COMPLETE = 1,
			MSG_ON_QUITROOM_COMPLETE,
			MSG_ON_UPLOADFILE_COMPLETE,
			MSG_ON_DOWNFILE_COMPLETE,
			MSG_ON_MEMBER_VOICE,
			MSG_ON_APPLY_AUKEY_COMPLETE,
			MSG_ON_PLAYFILE_COMPLETE,
			MSG_ON_SPEECH_TO_TEXT,
			MSG_ON_ROOM_OFFLINE
		}

		public class NoticeMessage
		{
			public NoticeMessage()
			{
				this.what = -1;
				this.intArg1 = 0;
				this.intArg2 = 0;
				this.strArg = string.Empty;
				this.datalen = 0;
				this.custom = new byte[2048];
			}

			public void clear()
			{
				this.what = -1;
				this.intArg1 = 0;
				this.intArg2 = 0;
				this.strArg = string.Empty;
				this.datalen = 0;
			}

			public int what;

			public int intArg1;

			public int intArg2;

			public string strArg;

			public byte[] custom;

			public int datalen;
		}
	}
}
