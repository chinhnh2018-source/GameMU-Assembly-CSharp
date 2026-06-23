using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Logic
{
	public class SpecialPromptManager
	{
		public static SpecialPromptManager GetInstance()
		{
			if (SpecialPromptManager.mSpecialPromptManager == null)
			{
				SpecialPromptManager.mSpecialPromptManager = new SpecialPromptManager();
			}
			return SpecialPromptManager.mSpecialPromptManager;
		}

		public void ProcesspecialPrompt()
		{
			SpecialPromptVO specialPromptVO = null;
			Dictionary<int, SpecialPromptVO>.Enumerator sprcialPrompeGetEnumerator = ConfigSpecialPrompt.GetSprcialPrompeGetEnumerator();
			while (sprcialPrompeGetEnumerator.MoveNext())
			{
				KeyValuePair<int, SpecialPromptVO> keyValuePair = sprcialPrompeGetEnumerator.Current;
				if (ConfigSpecialPrompt.GetActivityCanShowByID(keyValuePair.Key))
				{
					KeyValuePair<int, SpecialPromptVO> keyValuePair2 = sprcialPrompeGetEnumerator.Current;
					SpecialPromptVO value = keyValuePair2.Value;
					if (value != null && value.NeedTask <= Global.Data.roleData.CompletedMainTaskID)
					{
						if (specialPromptVO == null)
						{
							specialPromptVO = value;
						}
						else if (value.ShowLevel < specialPromptVO.ShowLevel)
						{
							specialPromptVO = value;
						}
					}
				}
			}
			if (specialPromptVO != null)
			{
				if (null == this.mSpecialPromptTipItem)
				{
					this.mSpecialPromptTipItem = U3DUtils.NEW<SpecialPromptTipItem>();
					PlayZone.GlobalPlayZone.stage.Add(this.mSpecialPromptTipItem);
					if (specialPromptVO.ID != this.mSpecialPromptTipItem.SpecialPromptID)
					{
						this.mSpecialPromptTipItem.RefreshInf(specialPromptVO);
					}
				}
				if (this.mSpecialPromptTipItem.ShowItem)
				{
					this.mSpecialPromptTipItem.UpDate();
					if (specialPromptVO.ID != this.mSpecialPromptTipItem.SpecialPromptID)
					{
						this.mSpecialPromptTipItem.RefreshInf(specialPromptVO);
					}
				}
				else if (specialPromptVO.ID != this.mSpecialPromptTipItem.SpecialPromptID)
				{
					this.mSpecialPromptTipItem.RefreshInf(specialPromptVO);
				}
			}
		}

		private static SpecialPromptManager mSpecialPromptManager;

		private SpecialPromptTipItem mSpecialPromptTipItem;
	}
}
