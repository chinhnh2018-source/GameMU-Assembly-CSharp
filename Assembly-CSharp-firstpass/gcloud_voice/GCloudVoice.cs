using System;

namespace gcloud_voice
{
	public class GCloudVoice
	{
		public static IGCloudVoice GetEngine()
		{
			if (GCloudVoice.instance == null)
			{
				GCloudVoice.instance = new GCloudVoiceEngine();
			}
			return GCloudVoice.instance;
		}

		private static IGCloudVoice instance;
	}
}
