using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SpriteLeadDeco : MonoBehaviour
{
	protected void LateUpdate()
	{
		try
		{
			if (Global.Data != null && Global.Data.roleData != null)
			{
				Vector3 vector = Vector3.zero;
				base.transform.position = LeaderInfo.LeaderPos + new Vector3(0f, 0.2f, 0f);
				int mapCode = Global.Data.roleData.MapCode;
				Vector3 vector2 = Vector3.zero;
				if (!Global.IsAutoFighting())
				{
					if (Global.Data.AutoRoadItemsList != null && Global.Data.AutoRoadItemsList.Count > 0 && Global.Data.AutoRoadItemsList[0].MapID == mapCode)
					{
						RoadItem roadItem = Global.Data.AutoRoadItemsList[0];
						vector..ctor((float)(roadItem.ToPos.X / 100), LeaderInfo.LeaderPos.y, (float)(roadItem.ToPos.Y / 100));
						vector2 = vector - LeaderInfo.LeaderPos;
					}
					else if (Super.GData.LeadInfo.MapCode == Global.Data.roleData.MapCode)
					{
						vector = Super.GData.LeadInfo.TaskTargetPos;
						vector.y = LeaderInfo.LeaderPos.y;
						vector2 = vector - LeaderInfo.LeaderPos;
					}
				}
				if (vector2.sqrMagnitude > 1f)
				{
					base.transform.localScale = Vector3.one;
					base.transform.rotation = Quaternion.LookRotation(vector2, Vector3.up);
				}
				else
				{
					base.transform.localScale = Vector3.zero;
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}
}
