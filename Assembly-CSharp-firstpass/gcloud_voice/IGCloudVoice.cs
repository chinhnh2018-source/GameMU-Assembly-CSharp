using System;

namespace gcloud_voice
{
	public abstract class IGCloudVoice
	{
		public abstract event IGCloudVoice.JoinRoomCompleteHandler OnJoinRoomComplete;

		public abstract event IGCloudVoice.QuitRoomCompleteHandler OnQuitRoomComplete;

		public abstract event IGCloudVoice.MemberVoiceHandler OnMemberVoice;

		public abstract event IGCloudVoice.ApplyMessageKeyCompleteHandler OnApplyMessageKeyComplete;

		public abstract event IGCloudVoice.UploadReccordFileCompleteHandler OnUploadReccordFileComplete;

		public abstract event IGCloudVoice.DownloadRecordFileCompleteHandler OnDownloadRecordFileComplete;

		public abstract event IGCloudVoice.PlayRecordFilCompleteHandler OnPlayRecordFilComplete;

		public abstract event IGCloudVoice.SpeechToTextHandler OnSpeechToText;

		public abstract event IGCloudVoice.StatusUpdateHandler OnStatusUpdate;

		public abstract int SetAppInfo(string appID, string appKey, string openID);

		public abstract int SetServerInfo(string URL);

		public abstract int Init();

		public abstract int SetMode(GCloudVoiceMode mode);

		public abstract int Poll();

		public abstract int Pause();

		public abstract int Resume();

		public abstract int JoinTeamRoom(string roomName, int msTimeout);

		public abstract int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout);

		public abstract int JoinFMRoom(string roomName, int msTimeout);

		public abstract int QuitRoom(string roomName, int msTimeout);

		public abstract int OpenMic();

		public abstract int CloseMic();

		public abstract int OpenSpeaker();

		public abstract int CloseSpeaker();

		public abstract int ApplyMessageKey(int msTimeout);

		public abstract int SetMaxMessageLength(int msTime);

		public abstract int StartRecording(string filePath);

		public abstract int StopRecording();

		public abstract int UploadRecordedFile(string filePath, int msTimeout);

		public abstract int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout);

		public abstract int PlayRecordedFile(string downloadFilePath);

		public abstract int StopPlayFile();

		public abstract int SpeechToText(string fileID, int language = 0, int msTimeout = 6000);

		public abstract int ForbidMemberVoice(int member, bool bEnable);

		public abstract int EnableLog(bool enable);

		public abstract int GetMicLevel();

		public abstract int GetSpeakerLevel();

		public abstract int SetSpeakerVolume(int vol);

		public abstract int TestMic();

		public abstract int GetFileParam(string filepath, int[] bytes, float[] seconds);

		public abstract int invoke(uint nCmd, uint nParam1, uint nParam2, uint[] pOutput);

		public abstract int JoinTeamRoom(string roomName, string token, int timestamp, int msTimeout);

		public abstract int JoinNationalRoom(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout);

		public abstract int ApplyMessageKey(string token, int timestamp, int msTimeout);

		public abstract int SpeechToText(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000);

		public enum GCloudVoiceCompleteCode
		{
			GV_ON_JOINROOM_SUCC = 1,
			GV_ON_JOINROOM_TIMEOUT,
			GV_ON_JOINROOM_SVR_ERR,
			GV_ON_JOINROOM_UNKNOWN,
			GV_ON_NET_ERR,
			GV_ON_QUITROOM_SUCC,
			GV_ON_MESSAGE_KEY_APPLIED_SUCC,
			GV_ON_MESSAGE_KEY_APPLIED_TIMEOUT,
			GV_ON_MESSAGE_KEY_APPLIED_SVR_ERR,
			GV_ON_MESSAGE_KEY_APPLIED_UNKNOWN,
			GV_ON_UPLOAD_RECORD_DONE,
			GV_ON_UPLOAD_RECORD_ERROR,
			GV_ON_DOWNLOAD_RECORD_DONE,
			GV_ON_DOWNLOAD_RECORD_ERROR,
			GV_ON_STT_SUCC,
			GV_ON_STT_TIMEOUT,
			GV_ON_STT_APIERR,
			GV_ON_PLAYFILE_DONE,
			GV_ON_ROOM_OFFLINE
		}

		public delegate void JoinRoomCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID);

		public delegate void QuitRoomCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID);

		public delegate void MemberVoiceHandler(int[] members, int count);

		public delegate void ApplyMessageKeyCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code);

		public delegate void UploadReccordFileCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid);

		public delegate void DownloadRecordFileCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid);

		public delegate void PlayRecordFilCompleteHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string filepath);

		public delegate void SpeechToTextHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string fileID, string result);

		public delegate void StatusUpdateHandler(IGCloudVoice.GCloudVoiceCompleteCode code, string roomName, int memberID);
	}
}
