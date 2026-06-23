using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class ChatSymbol
	{
		public string GetHtmlStr()
		{
			string result = string.Empty;
			if (this.IsAnimation)
			{
				result = string.Format("<img src='{0}' fps={1} id='anim'>", this.ImageName, this.AnimationFPS);
			}
			else
			{
				result = string.Format("<img src='{0}'>", this.ImageName);
			}
			return result;
		}

		public uint Id;

		public string ShowName;

		public string ImageName;

		public bool IsAnimation;

		public int AnimationFPS;
	}
}
