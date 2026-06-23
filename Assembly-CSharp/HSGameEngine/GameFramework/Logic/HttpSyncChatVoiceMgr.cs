using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public class HttpSyncChatVoiceMgr
	{
		private HttpSyncChatVoiceMgr()
		{
		}

		public static HttpSyncChatVoiceMgr GetInstance()
		{
			if (HttpSyncChatVoiceMgr._instance == null)
			{
				HttpSyncChatVoiceMgr._instance = new HttpSyncChatVoiceMgr();
			}
			return HttpSyncChatVoiceMgr._instance;
		}

		public void AddOneTask(VoiceRequestParam requestParam)
		{
			this.DownloadList.Add(requestParam);
			this.StartOneTask();
		}

		public void StopAllTask()
		{
			this.NeedStop = true;
			this.DownloadList.Clear();
		}

		private void StartOneTask()
		{
			this.NeedStop = false;
			if (!this.IsDowning && this.DownloadList.Count > 0)
			{
				this.IsDowning = true;
				VoiceRequestParam voiceRequestParam = this.DownloadList[0];
				HttpService.Instance.Load(voiceRequestParam.url, new Action<WWW, object>(this.OnOneTaskDone), voiceRequestParam.postData, voiceRequestParam.param, 0);
			}
		}

		private void OnOneTaskDone(WWW wwwObj, object param)
		{
			if (this.NeedStop)
			{
				return;
			}
			VoiceRequestParam voiceRequestParam = this.DownloadList[0];
			if (voiceRequestParam.callback != null)
			{
				voiceRequestParam.callback.Invoke(wwwObj, param);
			}
			this.DownloadList.RemoveAt(0);
			if (this.DownloadList.Count > 0)
			{
				voiceRequestParam = this.DownloadList[0];
				HttpService.Instance.Load(voiceRequestParam.url, new Action<WWW, object>(this.OnOneTaskDone), voiceRequestParam.postData, voiceRequestParam.param, 0);
			}
			else
			{
				this.DownloadList.Clear();
				this.IsDowning = false;
			}
		}

		private static HttpSyncChatVoiceMgr _instance;

		private bool IsDowning;

		private bool NeedStop;

		private List<VoiceRequestParam> DownloadList = new List<VoiceRequestParam>();
	}
}
